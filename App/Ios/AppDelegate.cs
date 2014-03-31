using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using Facebook;
using SavariWala.Common;
using Google.Maps;

namespace Savariwala.Ios
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow _window;
		DialogViewController _dvcController;
		UINavigationController _navController;

		public static AppDelegate Inst { get; private set; }
		public UIStoryboard StoryBoard { get; private set; }

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			Inst = this;
			_window = new UIWindow (UIScreen.MainScreen.Bounds);

			var root = new RootElement ("Savariwala" ) {
				new Section ("Actions") {
					new FacebookLoginElement (AppCommon.FbAppId, AppCommon.ExtendedPermissions),
				}
			};

			_dvcController = new DialogViewController (root);
			_navController = new UINavigationController (_dvcController);

			_window.RootViewController = _navController;
			_window.MakeKeyAndVisible ();

			MapServices.ProvideAPIKey (AppCommon.Inst.GoogleApiKeyNative);

			return true;
		}

		public void FacebookLoggedIn (bool didLogIn, string accessToken, string userId, Exception error)
		{
			if (didLogIn) {
				AppCommon.Inst.InitUser (userId);
				StoryBoard = UIStoryboard.FromName (AppCommon.Inst.IsPassenger.Value ? "Passenger" : "Driver", null);
				_navController.PushViewController(StoryBoard.InstantiateInitialViewController () as UIViewController, true);
			} else {
				new UIAlertView ("Failed to Log In", "Reason: " + error.Message, null, "Ok", null).Show();
			}
		}
	}
}

