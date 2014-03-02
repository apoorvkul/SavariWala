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
	[Activity (Label = "@string/reqBookingSrc")]			
	public class ReqBookingSrcActivity : MapPointsActivityBase
	{
		protected override List<Place> MarkerPoints {
			get 
			{ 
				var lst = new List<Place> ();
				lst.Add( new Place{ 
					Name = "Dummy Point", Address = "Dummy Address", Loc = AppCommon.Inst.CurLoc 
				});
				return lst;
			} 
			set { }
		}

		public ReqBookingSrcActivity()
			: base(Resource.Layout.ReqBookingSrc, Resource.Id.viewSrcPoint, 
				new LatLng(AppCommon.Inst.CurLoc.Lat, AppCommon.Inst.CurLoc.Lng))
		{
		}

		protected override void OnPointSelected(Place place)
		{

			AppCommon.Inst.CurrentReq = new Request { Src = place }; 
			this.StartNextActivity<ReqDetailsActivity> ();
		}
	}
}

