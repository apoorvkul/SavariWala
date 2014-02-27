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
using Android.Gms.Maps;
using Android.Gms.Maps.Model;


namespace SavariWala.AndroidApp
{
	[Activity (Label = "RequestBookingActivity")]			
	public class RequestBookingActivity : Activity
	{
		private GoogleMap _map;
		private MapFragment _mapFragment;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.RequestBooking);
			InitMapFragment();

			SetupMapIfNeeded(); // It's not gauranteed that the map will be available at this point.
		}
	
		protected override void OnResume()
		{
			base.OnResume();
			SetupMapIfNeeded();
		}

		private void InitMapFragment()
		{
			_mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
			if (_mapFragment == null)
			{
				var mapOptions = new GoogleMapOptions()
					.InvokeMapType(GoogleMap.MapTypeNormal)
					.InvokeZoomControlsEnabled(false)
					.InvokeCompassEnabled(true);

				var fragTx = FragmentManager.BeginTransaction();
				_mapFragment = MapFragment.NewInstance(mapOptions);
				fragTx.Add(Resource.Id.map, _mapFragment, "map");
				fragTx.Commit();
			}
		}

		private void SetupMapIfNeeded()
		{
			if (_map == null)
			{
				_map = _mapFragment.Map;
				if (_map != null)
				{
					// TODO [Chandu] List for possibly nearest pickup points (latitute, longitude) should come from Server

					// TODO Marker is currently dummy, 0m from current location
					// TODO App should find walking route and distance and hightlight top 3 on map
					var curLoc = App.Inst.LocationProvider.Location;
					var pickupLoc = new LatLng(curLoc.Latitude, curLoc.Longitude);
					var marker = new MarkerOptions();
					marker.SetPosition(pickupLoc);
					marker.SetTitle("Dummy Pickup Point");
					_map.AddMarker(marker);

					// We create an instance of CameraUpdate, and move the map to it.
					var cameraUpdate = CameraUpdateFactory.NewLatLngZoom(pickupLoc, 15);
					_map.MoveCamera(cameraUpdate);
				}
			}
		}
	}

}

