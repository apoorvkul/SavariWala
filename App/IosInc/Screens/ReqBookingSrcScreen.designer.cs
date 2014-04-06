// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Savariwala.Ios
{
	[Register ("ReqBookingSrcScreen")]
	partial class ReqBookingSrcScreen
	{
		[Outlet]
		MonoTouch.UIKit.UIButton BtnLeft { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton BtnRight { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton BtnSelStart { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView MapContainer { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BtnLeft != null) {
				BtnLeft.Dispose ();
				BtnLeft = null;
			}

			if (BtnRight != null) {
				BtnRight.Dispose ();
				BtnRight = null;
			}

			if (BtnSelStart != null) {
				BtnSelStart.Dispose ();
				BtnSelStart = null;
			}

			if (MapContainer != null) {
				MapContainer.Dispose ();
				MapContainer = null;
			}
		}
	}
}
