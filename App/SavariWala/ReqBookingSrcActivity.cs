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
		private bool _sensor;
		public ReqBookingSrcActivity()
			: base(Resource.Layout.ReqBookingSrc, Resource.Id.viewSrcPoint, GetSrc())
		{
			_sensor = AppCommon.Inst.StartPoint == null;
			AppCommon.Inst.StartPoint = null;			
			if (!_sensor) {
				FetchNearestPoints (_sensor);
			}

		}

		public override bool OnOptionsItemSelected(IMenuItem menuItem)
		{
			switch (menuItem.ItemId) {
			case Resource.Id.driverView:
				// Switch to driver view
			default:
				break;
			}
			return true;
		}

		public override bool OnPrepareOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.ReqBookingSrcMenu, menu);
			base.OnCreateOptionsMenu(menu);
			return true;
		}

		private static LatLng GetSrc ()
		{
			var curLoc = AppCommon.Inst.LocationProvider.CurLocInfo.Value.Loc;
			return AppCommon.Inst.StartPoint == null ? new LatLng (curLoc.Lat, curLoc.Lng) :
				new LatLng (AppCommon.Inst.StartPoint.Lat, AppCommon.Inst.StartPoint.Lng);
		}

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
			if (_sensor) {
				AppCommon.Inst.NearestPointProvider.DirectionAdded += OnPointDirectionsAdded;
				AppCommon.Inst.NearestPointProvider.PointsReset += OnPointsReset;
				_pointsVer = AppCommon.Inst.NearestPointProvider.Version; 
				ResetPoints (AppCommon.Inst.NearestPointProvider.PointDirections);

				base.OnCreate (bundle);
				var searchBtn = FindViewById<Button> (Resource.Id.seachBtn);
				searchBtn.Click += (s, e) => this.StartNextActivity<SearchStartActivity> ();
			}
			else base.OnCreate (bundle);
		}

		protected override void OnPointSelected(MapPoint place)
		{
			AppCommon.Inst.CurrentReq = new Request { Src = place };
			AppCommon.Inst.NearestPointProvider.DirectionAdded -= OnPointDirectionsAdded;
			AppCommon.Inst.NearestPointProvider.PointsReset -= OnPointsReset;
			this.StartNextActivity<ReqDetailsActivity> ();
		}
	}
}

