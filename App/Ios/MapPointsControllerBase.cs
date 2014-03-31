using System;
using Google.Maps;
using SavariWala.AndroidApp;
using System.Collections.Generic;
using SavariWala;
using MonoTouch.UIKit;
using System.Drawing;
using SavariWala.Common;
using MonoTouch.CoreLocation;

namespace Savariwala.Ios
{
	public abstract class MapPointsControllerBase : UITableViewController
	{
		UIButton _btnLeft, _btnRight, _btnSelStart;
		UIView _mapContainer;

		protected int _pointsVer;
		protected WalkingRoutesMapperImpl _wRoutesMapperImpl;
		protected WalkingRoutesMapperImpl _walkingRoutesMapperImpl;

		protected abstract void OnPointSelected(SavariWala.MapPoint place);

		public MapPointsControllerBase (IntPtr handle, GeoLoc mapCenter) : base (handle)
		{}

		protected void Init(UIButton btnLeft, UIButton btnRight, UIButton btnSelStart, UIView mapContainer)
		{
			_btnLeft = btnLeft;
			_btnRight = btnRight;
			_btnSelStart = btnSelStart;
			_mapContainer = mapContainer;
			_wRoutesMapperImpl = new WalkingRoutesMapperImpl (this);
		}

		protected class WalkingRoutesMapperImpl : WalkingRoutesMapper
		{
			public MapView MapView { get; private set; }
			MapPointsControllerBase _outer;
			protected int _curMarkerPt = 0;
			List<Polyline> _routeLines = new List<Polyline>();

			public SavariWala.MapPoint SelectedPoint {
				get { return _pointDrawingInfos[_curMarkerPt].Point; }
			}

			public WalkingRoutesMapperImpl(MapPointsControllerBase outer)
			{
				_outer = outer;
			}

			protected override void OnUiThread (Action a)
			{
				_outer.InvokeOnMainThread(() => a());
			}

			protected override void DrawPolyline (List<GeoLoc> points)
			{
				var path = new MutablePath ();
				points.ForEach (x => path.AddCoordinate (Utils.GetCLLoc(x)));
				var polyline = new Polyline { Path = path, Geodesic = true };

				if (points == _highlighedPoint.Polyline) {
					polyline.StrokeColor = UIColor.Blue;
					polyline.StrokeWidth = 7;
				} else {
					polyline.StrokeColor = UIColor.Cyan;
					polyline.StrokeWidth = 3;
				}
				polyline.Map = MapView;
				_routeLines.Add (polyline);
			}

			protected override void MoveCamera (SavariWala.GeoLoc mapCentre)
			{
				if(MapView == null)
					MapView = MapView.FromCamera (RectangleF.Empty, CameraPosition.FromCamera (Utils.GetCLLoc(MapCentre), 15));
				else MapView.Camera = CameraPosition.FromCamera(Utils.GetCLLoc(mapCentre), 15);
			}

			List<Marker> _markers;

			protected override void HighlightRoutes ()
			{
				_outer._btnRight.Enabled = _highlighedIndex != (_pointDrawingInfos.Count - 1);
				_outer._btnLeft.Enabled  = _highlighedIndex != 0;

				_outer._btnSelStart.SetTitle (_pointDrawingInfos [_highlighedIndex].Point.Name,
					UIControlState.Normal);

				MapView.Clear ();
				_markers.Clear ();
				_routeLines.Clear();

				foreach (var pdi in _pointDrawingInfos)
				{
					// draw markers
					var mPlace = pdi.Point;
					var marker = new Marker () {
						Title = mPlace.Name,
						Position = Utils.GetCLLoc(mPlace.Loc),
						Map = MapView
					};

					if (pdi == _highlighedPoint)
						MapView.SelectedMarker = marker;

					_markers.Add (marker);

					DrawPolyline(pdi.Polyline);
				}
			}
		}

		void OnPointDirectionsAdded (object sender, DirAddedEvtArg e)
		{
			if (e.Version == _pointsVer)
				_wRoutesMapperImpl.AddDirection (e.PointDir);
		}
			
		void OnPointsReset (object sender, PointsResetEvtArg e)
		{
			_pointsVer = e.Version;
			_wRoutesMapperImpl.ResetPoints(e.PointDirections);
		}

		public override void LoadView ()
		{
			base.LoadView ();

			_wRoutesMapperImpl.RefreshMap ();
			_mapContainer.AddSubview (_wRoutesMapperImpl.MapView);

			_btnSelStart.TouchUpInside += (s, e) => OnPointSelected(_wRoutesMapperImpl.SelectedPoint);
			_btnLeft.TouchUpInside += (s, e) => _wRoutesMapperImpl.MoveHighlight(false);
			_btnRight.TouchUpInside += (s, e) => _wRoutesMapperImpl.MoveHighlight(true);

			_wRoutesMapperImpl.Inited = true;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			_wRoutesMapperImpl.MapView.StartRendering ();
		}

		public override void ViewWillDisappear (bool animated)
		{	
			_wRoutesMapperImpl.MapView.StopRendering ();
			base.ViewWillDisappear (animated);
		}
	}
}

