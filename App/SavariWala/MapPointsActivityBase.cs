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

namespace SavariWala.AndroidApp
{

	abstract public class MapPointsActivityBase : Activity
	{
		protected MapFragment _mapFragment;
		int _layoutId;
		int _mapPointTextId;
		int _curMarketPt = 0;
		int _highlighedIndex = 0;
		PointDrawingInfo _highlighedPoint = new PointDrawingInfo();
		List<Marker> _markers = new List<Marker>();
		bool _inited = false;

		protected LatLng MapCentre { get; set; }

		class PointDrawingInfo
		{
			public Place Point { get; set; }
			public JsDirection Dir { get; set; }
			public List<LatLng> Polyline { get; set; }
		}

		List<PointDrawingInfo> _pointDrawingInfos = new List<PointDrawingInfo>();

		protected void ResetPoints (List<PointDirection> pointDirections)
		{
			if (pointDirections == null || pointDirections.Count == 0)
				return;
			_pointDrawingInfos = pointDirections.Select (x => new PointDrawingInfo { Point = x.Point, Dir = x.Direction }).ToList ();
			_highlighedIndex = 0;
			_highlighedPoint = _pointDrawingInfos [0];
			if (_inited) RunOnUiThread(() => RefreshMap ());
		}

		protected void AddDirection (PointDirection pointDir)
		{
			var matchedPt = _pointDrawingInfos.Find (x => x.Point == pointDir.Point);
			if (matchedPt == null)
				return;
			matchedPt.Dir = pointDir.Direction;
			matchedPt.Polyline = DecodePoly (pointDir.Direction.routes [0].overview_polyline.points);

			RunOnUiThread(() => DrawPolyline (matchedPt.Polyline));
		}

		protected abstract void OnPointSelected(Place place);

		public MapPointsActivityBase(int layoutId, int mapPointTextId, LatLng mapCentre)
		{
			_layoutId = layoutId;
			_mapPointTextId = mapPointTextId;
			MapCentre = mapCentre;
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (_layoutId);

			_mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
			if (_mapFragment == null)
			{
				var mapOptions = new GoogleMapOptions()
					.InvokeMapType(GoogleMap.MapTypeNormal)
					.InvokeZoomControlsEnabled(false)
					.InvokeCompassEnabled(true);

				var fragTx = FragmentManager.BeginTransaction();
				_mapFragment = MapFragment.NewInstance(mapOptions);
				fragTx.Add(Resource.Id.map, _mapFragment, "map");
				fragTx.Commit();
			}
			RefreshMap ();
			var btnLt = FindViewById<Button> (Resource.Id.buttonLt);
			var mapPointText = FindViewById<Button> (_mapPointTextId);
			var btnRt = FindViewById<Button> (Resource.Id.buttonRt);

			mapPointText.Click += (s, e) => {  OnPointSelected(_pointDrawingInfos[_curMarketPt].Point); };
			btnLt.Click += (s, e) => MoveHighlight(false);
			btnRt.Click += (s, e) => MoveHighlight(true);

			_inited = true;
		}

		protected override void OnResume()
		{
			base.OnResume();
			RefreshMap ();
		}

		void MoveHighlight (bool right)
		{
			var newIndex = _highlighedIndex + (right ? 1 : -1);
			if (0 <= newIndex && newIndex < _pointDrawingInfos.Count) {
				_highlighedIndex = newIndex;
				_highlighedPoint = _pointDrawingInfos [_highlighedIndex];
				HighlightRoutes ();
			}
		}

		List<Polyline> _routeLines = new List<Polyline>();
		void HighlightRoutes ()
		{
			var btnLt = FindViewById<Button> (Resource.Id.buttonLt);
			var btnRt = FindViewById<Button> (Resource.Id.buttonRt);

			btnRt.Enabled = _highlighedIndex != (_pointDrawingInfos.Count - 1);
			btnLt.Enabled = _highlighedIndex != 0;

			_routeLines.ForEach (x => x.Remove ());
			_routeLines.Clear();

			_markers.ForEach (x => x.Remove ());
			_markers.Clear ();

			var mapPointText = FindViewById<Button> (_mapPointTextId);
			mapPointText.Text = _pointDrawingInfos [_highlighedIndex].Point.Name;

			var map = _mapFragment.Map;

			foreach (var pdi in _pointDrawingInfos)
			{
				// draw markers
				Place mPlace = pdi.Point;
				var marker = new MarkerOptions ();
				if(pdi != _highlighedPoint) marker.InvokeAlpha (0.5f);
				marker.SetPosition (new LatLng(mPlace.Loc.Lat, mPlace.Loc.Lng));
				marker.SetTitle (mPlace.Name);
				_markers.Add(map.AddMarker (marker));

				DrawPolyline(pdi.Polyline);
			}
		}

		void DrawPolyline (List<LatLng> polyLine)
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
			polyLine.ForEach (x => options.Add (x));
			_routeLines.Add(map.AddPolyline(options));
		}

		protected void RefreshMap()
		{
			var map = _mapFragment.Map;
			if (map != null)
			{
					// We create an instance of CameraUpdate, and move the map to it.
					var cameraUpdate = CameraUpdateFactory.NewLatLngZoom(MapCentre, 15);
					map.MoveCamera(cameraUpdate);

				if (_pointDrawingInfos.Count == 0)
						return;

					_highlighedIndex = 0;
					HighlightRoutes ();
			}
		}

		private List<LatLng> DecodePoly(string encoded) {
			List<LatLng> poly = new List<LatLng>();
			int index = 0, len = encoded.Length;
			int lat = 0, lng = 0;

			while (index < len) {
				int b, shift = 0, result = 0;
				do {
					b = encoded[index++] - 63;
					result |= (b & 0x1f) << shift;
					shift += 5;
				} while (b >= 0x20);
				int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
				lat += dlat;

				shift = 0;
				result = 0;
				do {
					b = encoded[index++] - 63;
					result |= (b & 0x1f) << shift;
					shift += 5;
				} while (b >= 0x20);
				int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
				lng += dlng;

				LatLng p = new LatLng((((double) lat / 1E5)),
					(((double) lng / 1E5)));
				poly.Add(p);
			}

			return poly;
		}

	}
}
