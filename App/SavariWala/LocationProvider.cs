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
using Android.Gms.Common;
using Android.Gms.Location;
using ILocationListener = Android.Gms.Location.ILocationListener;
using SavariWala.Common;
using SavariWala.AndroidApp;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SavariWala.AndroidApp
{
	public class LocationProvider : Java.Lang.Object, ILocationListener,
	IGooglePlayServicesClientConnectionCallbacks, 
	IGooglePlayServicesClientOnConnectionFailedListener, ILocationProvider
	{
		protected override void Dispose (bool disposing)
		{
			if(locClient_ != null && disposing) locClient_.Dispose ();
			base.Dispose (disposing);
		}

		LocationClient locClient_;

		public LocationProvider()
		{
			CurLocInfo = new DynProp<LocInfo> (new LocInfo {Loc = new GeoLoc()});
		}

		public void Connect()
		{
			locClient_ = new LocationClient (App.Inst, this, this);
			locClient_.Connect ();
		}

		public void OnLocationChanged (global::Android.Locations.Location p0)
		{
			var li = new LocInfo { Loc = new GeoLoc { Lat = p0.Latitude, Lng = p0.Longitude } };
			if (p0.HasAccuracy) li.Accuracy = (int)p0.Accuracy;
			CurLocInfo.Value = li;
		}

		public void OnConnected (Bundle p0)
		{
			OnLocationChanged(locClient_.LastLocation);
		}

		public void OnDisconnected ()
		{}

		public void OnConnectionFailed (ConnectionResult p0)
		{
			//	Utils.Alert ("Location Services", "Connection Failed", false);
		}

		public void OnPause ()
		{
			if (locClient_.IsConnected) locClient_.Disconnect ();
		}

		public DynProp<LocInfo> CurLocInfo { get; set; }
	}
}


/*
		public event PropertyChangedEventHandler PropertyChanged;

		void onPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		*/


