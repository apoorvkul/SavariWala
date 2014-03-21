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
using System.Reactive;
using System.Reactive.Linq;


namespace SavariWala.AndroidApp
{
	[Activity (Label = "@string/reqBookingSrc")]			
	public class ReqBookingSrcActivity : MapPointsActivityBase
	{
		public ReqBookingSrcActivity()
			: base(Resource.Layout.ReqBookingSrc, Resource.Id.viewSrcPoint, 
				new LatLng(AppCommon.Inst.LocationProvider.CurLoc.Lat, AppCommon.Inst.LocationProvider.CurLoc.Lng))
		{}

		void OnPointDirectionsAdded (object sender, DirAddedEvtArg e)
		{
			if (e.Version == _pointsVer)
				AddDirection (e.PointDir);
		}

		protected int _pointsVer;

		void OnPointsReset (object sender, PointsResetEvtArg e)
		{
			_pointsVer = e.Version;
			ResetPoints(e.PointDirections);
		}

		protected override void OnCreate (Bundle bundle)
		{
			AppCommon.Inst.NearestPointProvider.DirectionAdded += OnPointDirectionsAdded;
			AppCommon.Inst.NearestPointProvider.PointsReset += OnPointsReset;
			_pointsVer = AppCommon.Inst.NearestPointProvider.Version; 
			ResetPoints(AppCommon.Inst.NearestPointProvider.PointDirections);
			base.OnCreate (bundle);
		}

		protected override void OnPointSelected(Place place)
		{
			AppCommon.Inst.CurrentReq = new Request { Src = place };
			AppCommon.Inst.NearestPointProvider.DirectionAdded -= OnPointDirectionsAdded;
			AppCommon.Inst.NearestPointProvider.PointsReset -= OnPointsReset;
			this.StartNextActivity<ReqDetailsActivity> ();
		}
	}
}

