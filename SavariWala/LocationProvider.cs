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
	IGooglePlayServicesClientOnConnectionFailedListener, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		void onPropertyChanged([CallerMemberName] string propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		protected override void Dispose (bool disposing)
		{
			if(locClient_ != null && disposing) locClient_.Dispose ();
			base.Dispose (disposing);
		}

		LocationClient locClient_;

		public void Connect()
		{
			locClient_ = new LocationClient (App.Inst, this, this);
			locClient_.Connect ();
		}

		public void OnLocationChanged (global::Android.Locations.Location p0)
		{
			AppCommon.Inst.CurLoc = new GeoLoc { Lat =  p0.Latitude, Lng = p0.Longitude };
		}

		public void OnConnected (Bundle p0)
		{
			var loc = locClient_.LastLocation;
			AppCommon.Inst.CurLoc = new GeoLoc { Lat =  loc.Latitude, Lng = loc.Longitude };
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
	}
}

