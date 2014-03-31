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
	[Register ("ReqDetailsScreen")]
	partial class ReqDetailsScreen
	{
		[Outlet]
		MonoTouch.UIKit.UITableView TblAutoComplete { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField TxtDest { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField TxtNumPax { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField TxtTimeStart { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (TblAutoComplete != null) {
				TblAutoComplete.Dispose ();
				TblAutoComplete = null;
			}

			if (TxtNumPax != null) {
				TxtNumPax.Dispose ();
				TxtNumPax = null;
			}

			if (TxtTimeStart != null) {
				TxtTimeStart.Dispose ();
				TxtTimeStart = null;
			}

			if (TxtDest != null) {
				TxtDest.Dispose ();
				TxtDest = null;
			}
		}
	}
}
