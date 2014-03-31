using System;
using MonoTouch.CoreLocation;
using SavariWala;
using MonoTouch.UIKit;

namespace Savariwala.Ios
{
	public class Utils
	{
		public static CLLocationCoordinate2D GetCLLoc(GeoLoc loc)
		{
			return new CLLocationCoordinate2D (loc.Lat, loc.Lng);
		}

		public static void AlertOnEx (string title, string message, Action action)
		{
			try {
				action ();
			} catch { //(Exception e) 
				new UIAlertView (title, message, null, "OK", null).Show ();
			}
		}
	}
}

