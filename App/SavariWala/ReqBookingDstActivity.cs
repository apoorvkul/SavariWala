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
			FetchNearestPoints (false);
		}

		protected override void OnPointSelected(MapPoint place)
		{
			AppCommon.Inst.CurrentReq.Dst = place; 
			this.StartNextActivity<ReqConfirmActivity>();
		}
	}
}

