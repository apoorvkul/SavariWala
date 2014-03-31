using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace Savariwala.Ios
{
	partial class ReqSrcController : UIViewController
	{
		MapView mapView;

		public ReqSrcController (IntPtr handle) : base (handle)
		{
		}

		public override void LoadView ()
		{
			base.LoadView ();


			var src = AppCommon.Inst.

			CameraPosition camera = CameraPosition.FromCamera (37.797865, -122.402526, 6);

			mapView = MapView.FromCamera (RectangleF.Empty, camera);
			mapView.MyLocationEnabled = true;

			var xamMarker = new Marker () {
				Title = "Xamarin HQ",
				Snippet = "Where the magic happens.",
				Position = new CLLocationCoordinate2D (37.797865, -122.402526),
				Map = mapView
			};

			mapView.SelectedMarker = xamMarker;

			View = mapView;
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			mapView.StartRendering ();
		}

		public override void ViewWillDisappear (bool animated)
		{	
			mapView.StopRendering ();
			base.ViewWillDisappear (animated);
		}
	}
}
