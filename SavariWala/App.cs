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

namespace SavariWala.AndroidApp
{
	[Application]
	public class App : Application
	{

		public static App Inst { get; private set;}

		private AppCommon appCommon_;
		public static AppCommon Common { 
			get { 
				return Inst.appCommon_; 
			} 
		}

		public LocationProvider LocationProvider { get; private set; }

		public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{}

		public override void OnCreate()
		{
			Inst = this;
			appCommon_ = new AppCommon();
			LocationProvider = new LocationProvider ();
			ThreadPool.QueueUserWorkItem ((x) => LocationProvider.Connect ());

			base.OnCreate ();
		}
	}
}

