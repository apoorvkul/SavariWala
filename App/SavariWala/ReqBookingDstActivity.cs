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
using Newtonsoft.Json;
using SavariWala.Common.Js;


namespace SavariWala.AndroidApp
{
	[Activity (Label = "@string/reqBookingDst")]			
	public class ReqBookingDstActivity : MapPointsActivityBase
	{

		public ReqBookingDstActivity()
			: base(Resource.Layout.ReqBookingDst, Resource.Id.viewDstPoint, 
				new LatLng(AppCommon.Inst.Destination.Lat, AppCommon.Inst.Destination.Lng))
		{
			FetchNearestPoints (AppCommon.Inst.Destination);
		}

		void FetchNearestPoints (GeoLoc dst)
		{
			List<MapPoint> pts;
			using (var client = new MapPointProvider.Client (AppCommon.Inst.GetThriftProtocol (AppCommon.PortOffsets.MapPointProvider))) {
				pts = client.getMapPoint (false, dst.Lat, dst.Lng);
			}
			var ptDirs = pts.Take(3).Select (x => new PointDirection { Point = 
					new Place{ Name = x.Description, Loc = new GeoLoc { Lat = x.Latitude, Lng = x.Longitude } } }).ToList();
			ResetPoints (ptDirs);

			foreach (var pd in ptDirs) {
				AppCommon.Inst.DirectionsProvider.GetRoutesAsync (
					str => AddDirection (new PointDirection { Point = pd.Point, 
						Direction = JsonConvert.DeserializeObject<JsDirection> (str)}),
					pd.Point.Loc, AppCommon.Inst.Destination, DirectionsProvider.Walking);
			}
		}

		protected override void OnPointSelected(Place place)
		{
			AppCommon.Inst.CurrentReq.Dst = place; 
			this.StartNextActivity<ReqConfirmActivity>();
		}
	}
}

