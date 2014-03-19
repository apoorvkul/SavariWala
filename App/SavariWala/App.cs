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
using SavariWala.Common;
using System.Threading;
using SavariWala.Common.Locale;

namespace SavariWala.AndroidApp
{
	// Application will have 2 singletons:
	//	1. App (owning platform specific objects) 
	//	2. AppCommon (ownig cross platfrom objects)
	// Cross platorm objects should access other cross platorm objects through AppCommon
	// Platform specific objects can access all other object via App or AppCommon
	[Application]
	public class App : Application
	{

		public static App Inst { get; private set;}

		public LocationProvider LocationProvider { get; private set; }

		public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{}

		public override void OnCreate()
		{
			Inst = this;
			base.OnCreate ();

			// TODO Hardcoding API key for now. Get from manifest
			new AppCommon ("AIzaSyBjclPxE1Y_XkiaFIFDQCMr1hdSaFHw124", "AIzaSyBK3goB0EEh36HN_CiG-OW2GjnBdl2j1SQ") {
				ErrorTranslator = new ErrorTranslatorEnglish ()
			};
			LocationProvider = new LocationProvider ();
			ThreadPool.QueueUserWorkItem ((x) => LocationProvider.Connect ());


		}
	}
}

