using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SavariWala.Common;
using MonoTouch.CoreLocation;
using SavariWala;

namespace Savariwala.Ios
{
	class LocationProvider : ILocationProvider
	{
		CLLocationManager _locMgr;

		public DynProp<LocInfo> CurLocInfo { get; set; }

		public LocationProvider (){
			CurLocInfo = new DynProp<LocInfo> (new LocInfo { Loc =  new GeoLoc()});
			_locMgr = new CLLocationManager();
		}

		public void Connect ()
		{
			if (!CLLocationManager.LocationServicesEnabled) 
			{
				new UIAlertView ("Location Services Disables", "Please enable location services", null, "OK", null).Show();
				return;
			}

			_locMgr.DesiredAccuracy = 1; // meters
			if (UIDevice.CurrentDevice.CheckSystemVersion (6, 0)) 
				_locMgr.LocationsUpdated += (sender, e) => UpdateCurLoc(e.Locations [e.Locations.Length - 1]); 
			else _locMgr.UpdatedLocation += (sender, e) => UpdateCurLoc(e.NewLocation); 
			_locMgr.StartUpdatingLocation();
		}

		void UpdateCurLoc (CLLocation clLoc)
		{
			CurLocInfo.Value = new LocInfo { Loc = Utils.GetGeoLoc (clLoc), Accuracy = (int)clLoc.HorizontalAccuracy };
		}
	}
}
