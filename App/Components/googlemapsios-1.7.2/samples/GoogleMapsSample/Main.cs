using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace GoogleMapsSample
{
	public class Application
	{
		static void Main (string[] args)
		{
			// TODO Hardcoding API key for now. Get from manifest
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}
