using System;
using SavariWala.Common.Js;
using System.Collections.Generic;
using SavariWala.Common;
using System.Linq;
using Newtonsoft.Json;

namespace SavariWala.AndroidApp
{
	public abstract class WalkingRoutesMapper
	{
		protected int _highlighedIndex = 0;
		protected PointDrawingInfo _highlighedPoint = new PointDrawingInfo();

		public GeoLoc MapCentre { get; set; }

		protected class PointDrawingInfo
		{
			public MapPoint Point { get; set; }
			public JsDirection Dir { get; set; }
			public List<GeoLoc> Polyline { get; set; }
		}

		protected List<PointDrawingInfo> _pointDrawingInfos = new List<PointDrawingInfo>();

		protected abstract void OnUiThread (Action a);
		protected abstract void DrawPolyline (List<GeoLoc> polyLine);
		protected abstract void MoveCamera (GeoLoc mapCentre);
		protected abstract void HighlightRoutes ();

		public void ResetPoints (List<PointDirection> pointDirections)
		{
			if (pointDirections == null || pointDirections.Count == 0)
				return;
			_pointDrawingInfos = pointDirections.Select (x => new PointDrawingInfo { Point = x.Point, Dir = x.Direction }).ToList ();
			_highlighedIndex = 0;
			_highlighedPoint = _pointDrawingInfos [0];
			if (Inited) OnUiThread(() => RefreshMap ());
		}

		public void AddDirection (PointDirection pointDir)
		{
			var matchedPt = _pointDrawingInfos.Find (x => x.Point == pointDir.Point);
			if (matchedPt == null)
				return;
			matchedPt.Dir = pointDir.Direction;
			matchedPt.Polyline = DecodePoly (pointDir.Direction.routes [0].overview_polyline.points);

			OnUiThread(() => DrawPolyline (matchedPt.Polyline));
		}

		public bool Inited {get; set; }

		protected  WalkingRoutesMapper()
		{
			Inited = false;
		}
			
		public void MoveHighlight (bool right)
		{
			var newIndex = _highlighedIndex + (right ? 1 : -1);
			if (0 <= newIndex && newIndex < _pointDrawingInfos.Count) {
				_highlighedIndex = newIndex;
				_highlighedPoint = _pointDrawingInfos [_highlighedIndex];
				HighlightRoutes ();
			}
		}

		public void RefreshMap()
		{
			MoveCamera (MapCentre);

			if (_pointDrawingInfos.Count == 0)
				return;

			_highlighedIndex = 0;
			HighlightRoutes ();
		}

		public void FetchNearestPoints (bool isSrc)
		{
			List<MapPoint> pts;
			using (var client = new MapPointProvider.Client (AppCommon.Inst.GetThriftProtocol (AppCommon.PortOffsets.MapPointProvider))) {
				pts = client.getMapPoint (isSrc, MapCentre);
			}
			var ptDirs = pts.Take(3).Select (x => new PointDirection { Point = x }).ToList();
			ResetPoints (ptDirs);

			foreach (var pd in ptDirs) {
				AppCommon.Inst.DirectionsProvider.GetRoutesAsync (
					str => AddDirection (new PointDirection { Point = pd.Point, 
						Direction = JsonConvert.DeserializeObject<JsDirection> (str)}),
					pd.Point.Loc, new GeoLoc { Lat = MapCentre.Lat, Lng = MapCentre.Lng }, DirectionsProvider.Walking);
			}
		}

		protected List<GeoLoc> DecodePoly(string encoded) {
			var poly = new List<GeoLoc>();
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

				var p = new GeoLoc{ Lat = (((double) lat / 1E5)),
					Lng = (((double) lng / 1E5))};
				poly.Add(p);
			}

			return poly;
		}
	}
}

