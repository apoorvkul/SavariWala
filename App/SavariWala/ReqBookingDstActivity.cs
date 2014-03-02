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


namespace SavariWala.AndroidApp
{
	[Activity (Label = "@string/reqBookingDst")]			
	public class ReqBookingDstActivity : MapPointsActivityBase
	{
		protected override List<Place> MarkerPoints {
			get 
			{ 
				var lst = new List<Place> ();
				lst.Add(new Place{ 
					Name = "Dummy Destination", Address = "Dummy Address", Loc = AppCommon.Inst.Destination
				});
				return lst;
			}
			set { }
		}

		public ReqBookingDstActivity()
			: base(Resource.Layout.ReqBookingDst, Resource.Id.viewDstPoint, 
				new LatLng(AppCommon.Inst.Destination.Lat, AppCommon.Inst.Destination.Lng))
		{
		}

		protected override void OnPointSelected(Place place)
		{
			AppCommon.Inst.CurrentReq.Dst = place; 
			this.StartNextActivity<ReqConfirmActivity>();
		}
	}
}

