using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SavariWala.Common;
using SavariWala.Common.Locale;

namespace Savariwala.Ios
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// TODO Hardcoding API key for now. Get from manifest
			AppCommon.Create ("AIzaSyDoMGciKq9fjSpc2KxIh5HFlY_dW6rfue4", 
				"AIzaSyBK3goB0EEh36HN_CiG-OW2GjnBdl2j1SQ",
				new ErrorTranslatorEnglish (),
				new LocationProvider ());

			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
