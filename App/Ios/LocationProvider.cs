using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SavariWala.Common;

namespace Savariwala.Ios
{
	class LocationProvider : ILocationProvider
	{
		#region ILocationProvider implementation
		public void Connect ()
		{
			throw new NotImplementedException ();
		}
		public DynProp<LocInfo> CurLocInfo {
			get {
				throw new NotImplementedException ();
			}
			set {
				throw new NotImplementedException ();
			}
		}
		#endregion
	}
}
