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

	abstract public class MapPointsActivityBase : Activity
	{
		protected GoogleMap _map;
		protected MapFragment _mapFragment;

		int _layoutId;
		int _mapPointTextId;
		LatLng _mapCentre;
		int _curMarketPt = 0;
	
		protected abstract List<Place> MarkerPoints { get; set; }

		protected abstract void OnPointSelected(Place place);

		public MapPointsActivityBase(int layoutId, int mapPointTextId, LatLng mapCentre)
		{
			_layoutId = layoutId;
			_mapPointTextId = mapPointTextId;
			_mapCentre = mapCentre;
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (_layoutId);

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

			RefreshMap (_mapCentre, MarkerPoints);

			var btnLt = FindViewById<Button> (Resource.Id.buttonLt);
			var mapPointText = FindViewById<Button> (_mapPointTextId);
			var btnRt = FindViewById<Button> (Resource.Id.buttonRt);

			// For now till server side is not available
			mapPointText.Click += (s, e) => {  OnPointSelected(MarkerPoints[_curMarketPt]); };
			mapPointText.Text = "Dummy Pickup Point";
			btnLt.Enabled = false;
			btnRt.Enabled = false;
		}

		protected override void OnResume()
		{
			base.OnResume();
			// TODO [Chandu] List for possibly nearest pickup points (latitute, longitude) should come from Server
			// TODO Marker is currently dummy, 0m from current location
			RefreshMap (_mapCentre, MarkerPoints);
		}

		protected void RefreshMap(LatLng curLoc, List<Place> points)
		{
			if (_map == null)
			{
				_map = _mapFragment.Map;
				if (_map != null)
				{
					// TODO App should find walking route and distance and hightlight top 3 on map
					foreach (var point in points) 
					{
						var marker = new MarkerOptions ();
						marker.SetPosition (new LatLng(point.Loc.Lat, point.Loc.Lng));
						//marker.SetTitle (point.Name); TODO: Till server is up, we use dummy:
						marker.SetTitle (point.Name);
						_map.AddMarker (marker);
					}

					// We create an instance of CameraUpdate, and move the map to it.
					var cameraUpdate = CameraUpdateFactory.NewLatLngZoom(curLoc, 15);
					_map.MoveCamera(cameraUpdate);
				}
			}
		}
	}
}
