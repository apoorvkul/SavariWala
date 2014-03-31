using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using SavariWala.Common;
using SavariWala.Common.Js;
using Newtonsoft.Json;

namespace SavariWala.AndroidApp
{

	abstract public class MapPointsActivityBase : Activity
	{
		int _layoutId;
		protected WalkingRoutesMapperImpl _wRoutesMapperImpl;

		protected abstract void OnPointSelected(MapPoint place);

		protected class WalkingRoutesMapperImpl : WalkingRoutesMapper
		{
			MapPointsActivityBase _outer;
			protected MapFragment _mapFragment;
			protected int _curMarkerPt = 0;
			List<Polyline> _routeLines = new List<Polyline>();
			List<Marker> _markers = new List<Marker>();

			public int MapPointTextId { get; set; }

			public WalkingRoutesMapperImpl(MapPointsActivityBase outer)
			{
				_outer = outer;
			}

			public void createMapFragment()
			{
				_mapFragment = _outer.FragmentManager.FindFragmentByTag("map") as MapFragment;
				if (_mapFragment == null)
				{
					var mapOptions = new GoogleMapOptions()
						.InvokeMapType(GoogleMap.MapTypeNormal)
						.InvokeZoomControlsEnabled(false)
						.InvokeCompassEnabled(true);

					var fragTx = _outer.FragmentManager.BeginTransaction();
					_mapFragment = MapFragment.NewInstance(mapOptions);
					fragTx.Add(Resource.Id.map, _mapFragment, "map");
					fragTx.Commit();
				}
			}

			public MapPoint SelectedPoint {
				get { return _pointDrawingInfos[_curMarkerPt].Point; }
			}

			protected override void OnUiThread (Action a)
			{
				_outer.RunOnUiThread (a);
			}

			protected override void DrawPolyline (List<GeoLoc> polyLine)
			{
				if (polyLine == null || polyLine.Count == 0)
					return;

				var map = _mapFragment.Map;
				// draw routes
				PolylineOptions options = new PolylineOptions ().Geodesic (true);
				if(polyLine == _highlighedPoint.Polyline){
					options.InvokeWidth (7); 
					options.InvokeColor (System.Drawing.Color.Blue.ToArgb());
				} else {
					options.InvokeWidth (3); 
					options.InvokeColor (System.Drawing.Color.LightBlue.ToArgb());
				}
				polyLine.ForEach (x => options.Add (Utils.GetLatLng(x)));
				_routeLines.Add(map.AddPolyline(options));
			}

			protected override void MoveCamera (GeoLoc mapCenter)
			{
				var map = _mapFragment.Map;
				var cameraUpdate = CameraUpdateFactory.NewLatLngZoom(Utils.GetLatLng(mapCenter), 15);
				map.MoveCamera(cameraUpdate);
			}

			protected override void HighlightRoutes ()
			{
				var btnLt = _outer.FindViewById<Button> (Resource.Id.buttonLt);
				var btnRt = _outer.FindViewById<Button> (Resource.Id.buttonRt);

				btnRt.Enabled = _highlighedIndex != (_pointDrawingInfos.Count - 1);
				btnLt.Enabled = _highlighedIndex != 0;

				_routeLines.ForEach (x => x.Remove ());
				_routeLines.Clear();

				_markers.ForEach (x => x.Remove ());
				_markers.Clear ();

				var mapPointText = _outer.FindViewById<Button> (MapPointTextId);
				mapPointText.Text = _pointDrawingInfos [_highlighedIndex].Point.Name;

				var map = _mapFragment.Map;

				foreach (var pdi in _pointDrawingInfos)
				{
					// draw markers
					MapPoint mPlace = pdi.Point;
					var marker = new MarkerOptions ();
					if(pdi != _highlighedPoint) marker.InvokeAlpha (0.5f);
					marker.SetPosition (new LatLng(mPlace.Loc.Lat, mPlace.Loc.Lng));
					marker.SetTitle (mPlace.Name);
					_markers.Add(map.AddMarker (marker));

					DrawPolyline(pdi.Polyline);
				}
			}
		}

		public MapPointsActivityBase(int layoutId, int mapPointTextId, GeoLoc mapCentre)
		{
			_layoutId = layoutId;
			_wRoutesMapperImpl = new WalkingRoutesMapperImpl (this) { MapCentre = mapCentre, MapPointTextId = mapPointTextId };
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (_layoutId);

			_wRoutesMapperImpl.createMapFragment ();
			_wRoutesMapperImpl.RefreshMap ();

			var btnLt = FindViewById<Button> (Resource.Id.buttonLt);
			var mapPointText = FindViewById<Button> (_wRoutesMapperImpl.MapPointTextId);
			var btnRt = FindViewById<Button> (Resource.Id.buttonRt);

			mapPointText.Click += (s, e) => {  OnPointSelected(_wRoutesMapperImpl.SelectedPoint); };
			btnLt.Click += (s, e) => _wRoutesMapperImpl.MoveHighlight(false);
			btnRt.Click += (s, e) => _wRoutesMapperImpl.MoveHighlight(true);

			_wRoutesMapperImpl.Inited = true;
		}

		protected override void OnResume()
		{
			base.OnResume();
			_wRoutesMapperImpl.RefreshMap ();
		}
	}
}
