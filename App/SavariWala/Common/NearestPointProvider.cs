using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		public MapPoint Point { get; set; }
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
			AppCommon.Inst.LocationProvider.CurLocInfo.Event += OnCurLocChanged;
			AppCommon.Inst.IsPassenger.Event += (s, e) => {
				if (e.NewValue && !e.OldValue)
					ThreadPool.QueueUserWorkItem (x => FetchNearestPoints ());
			};
		}

		GeoLoc lastLoc;
		GeoLoc LastLoc {
			get {
				return lastLoc;
			}
			set {
				lastLoc = value;
				if(AppCommon.Inst.IsPassenger.Value) 
					ThreadPool.QueueUserWorkItem(x => FetchNearestPoints ());
			}
		}

		void FetchNearestPoints ()
		{
			List<MapPoint> pts;
			using (var client = new MapPointProvider.Client (AppCommon.Inst.GetThriftProtocol (AppCommon.PortOffsets.MapPointProvider))) {
				pts = client.getMapPoint (true, LastLoc);
			}

			var ptDirs = pts.Take(3).Select (x => new PointDirection { Point = x }).ToList();
			var version = _version;
			PointDirections = ptDirs;

			// Draw routes only if current accuracy radius is less than a quarter of route distances
			// No point drawing non-sense route
			var curAccuracy = AppCommon.Inst.LocationProvider.CurLocInfo.Value.Accuracy;
			if (curAccuracy.HasValue && pts.Count > 0 && CommonUtils.GeoDistance (pts[0].Loc, LastLoc) > 3 * curAccuracy.Value) {
				foreach (var ptDir in ptDirs) {
					AppCommon.Inst.DirectionsProvider.GetRoutesAsync (
						str => {
							ptDir.Direction =JsonConvert.DeserializeObject<JsDirection> (str);
							OnEvent(DirectionAdded, new DirAddedEvtArg {Version = version, PointDir = ptDir });
						}, AppCommon.Inst.LocationProvider.CurLocInfo.Value.Loc, ptDir.Point.Loc, DirectionsProvider.Walking);
				}
			}
		}

		double VICINITY_THRESHOLD = 100; // 100m

		int _lastAccuracy = int.MaxValue;

		bool HasAccuracyImproved (int? accuracy)
		{
			return accuracy.HasValue && accuracy.Value < _lastAccuracy / 2;
		}

		void OnCurLocChanged (object sender, DynProp<LocInfo>.EvtArgs e)
		{
			if ((LastLoc != null) && (  // if not the first update and
				!HasAccuracyImproved(e.NewValue.Accuracy) ||  
				CommonUtils.GeoDistance (LastLoc, e.NewValue.Loc) < VICINITY_THRESHOLD))  
				return;	
			LastLoc = e.NewValue.Loc;
			_lastAccuracy = e.NewValue.Accuracy ?? int.MaxValue;
		}
	}
}