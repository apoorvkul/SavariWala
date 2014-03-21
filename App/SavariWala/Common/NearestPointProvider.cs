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
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using Thrift.Transport;
using Thrift.Protocol;
using Thrift;
using System.Threading;
using Newtonsoft.Json;
using SavariWala.Common.Js;

namespace SavariWala.Common
{
	public class PointDirection
	{
		public Place Point { get; set; }
		public JsDirection Direction { get; set; }
	}

	public class PointsResetEvtArg : EventArgs
	{

		public int Version { get; set; }
		public List<PointDirection> PointDirections { get; set; }
	}

	public class DirAddedEvtArg : EventArgs
	{
		public int Version { get; set; }
		public PointDirection PointDir { get; set; }
	}

	public class NearestPointProvider
	{
		public event EventHandler<PointsResetEvtArg> PointsReset;
		public event EventHandler<DirAddedEvtArg> DirectionAdded;

		int _version = 0;
		public int Version {
			get {
				return _version;
			}
		}

		void OnEvent<T> (EventHandler<T> handler, T args)
		{
			if (handler != null) handler (this, args);
		}

		List<PointDirection> pointDirections = new List<PointDirection>();
		public List<PointDirection> PointDirections {
			get {
				return pointDirections;
			}
			private set {
				pointDirections = value;
				OnEvent (PointsReset, new PointsResetEvtArg { Version = ++_version, PointDirections = this.PointDirections });
			}
		}

		public NearestPointProvider()
		{
			AppCommon.Inst.LocationProvider.CurLocChanged += OnCurLocChanged;
		}

		GeoLoc lastLoc;
		GeoLoc LastLoc {
			get {
				return lastLoc;
			}
			set {
				lastLoc = value;
				ThreadPool.QueueUserWorkItem(x => FetchNearestPoints ());
			}
		}

		void FetchNearestPoints ()
		{
			List<MapPoint> pts;
			using (var client = new MapPointProvider.Client (AppCommon.Inst.GetThriftProtocol (AppCommon.PortOffsets.MapPointProvider))) {
				pts = client.getMapPoint (true, LastLoc.Lat, LastLoc.Lng);
			}

			var ptDirs = pts.Take(3).Select (x => new PointDirection { Point = 
					new Place { Name = x.Description, Loc = new GeoLoc { Lat = x.Latitude, Lng = x.Longitude } } }).ToList();
			var version = _version;
			PointDirections = ptDirs;

			// Draw routes only if current accuracy radius is less than a quarter of route distances
			// No point drawing non-sense route
			var curAccuracy = AppCommon.Inst.LocationProvider.CurAccuracy;
			if (curAccuracy.HasValue && pts.Count > 0 &&
				CommonUtils.GeoDistance (new GeoLoc { Lat = pts [0].Latitude, Lng = pts [0].Longitude }, LastLoc) 
					> 3 * curAccuracy.Value) {
				foreach (var ptDir in ptDirs) {
					AppCommon.Inst.DirectionsProvider.GetRoutesAsync (
						str => {
							ptDir.Direction =JsonConvert.DeserializeObject<JsDirection> (str);
							OnEvent(DirectionAdded, new DirAddedEvtArg {Version = version, PointDir = ptDir });
						}, AppCommon.Inst.LocationProvider.CurLoc, ptDir.Point.Loc, DirectionsProvider.Walking);
				}
			}
		}

		double VICINITY_THRESHOLD = 100; // 100m

		int _lastAccuracy = int.MaxValue;
		void OnCurLocChanged (object sender, GeoLocEvtArgs e)
		{
			if ((LastLoc != null) && (  // if not the first update and
				e.Accuracy.HasValue && e.Accuracy > _lastAccuracy/2 ||  // either accuracy hasn't improved substantially
				CommonUtils.GeoDistance (LastLoc, e.GeoLoc) < VICINITY_THRESHOLD))  // or we haven't walked far
				return;	// dont update
			LastLoc = e.GeoLoc;
			_lastAccuracy = e.Accuracy.HasValue ? e.Accuracy.Value : int.MaxValue;
		}
	}
}