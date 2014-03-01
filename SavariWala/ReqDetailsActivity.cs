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
using Android.Views.InputMethods;
using SavariWala.Common;
using System.Json;

namespace SavariWala.AndroidApp
{
	[Activity (Label = "@string/reqDetails")]			
	public class ReqDetailsActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.ReqDetails);
			var textDest = FindViewById<AutoCompleteTextView> (Resource.Id.textDest);
			//var textStartTime = FindViewById<Button> (Resource.Id.textStartTime);

			textDest.TextChanged += AutoCompletePlaces;

			textDest.EditorAction += (sender, e) => {
				if (e.ActionId == ImeAction.Go) {
					var curLoc = AppCommon.Inst.CurLoc;
					AppCommon.Inst.PlacesProvider.SearchAsync (x => RunOnUiThread(() => ShowSearchResults(x)),
						textDest.Text, curLoc.Lat, curLoc.Lng);
					e.Handled = true; 
				}
			};
		}

		void AutoCompletePlaces (object sender, Android.Text.TextChangedEventArgs e)
		{
			var curLoc = AppCommon.Inst.CurLoc;
			var textDst = (AutoCompleteTextView)sender;
			AppCommon.Inst.PlacesProvider.AutoCompleteAsync (
				lst => RunOnUiThread(() => textDst.Adapter = new ArrayAdapter (this, Resource.Layout.ListItem, lst)),
				textDst.Text, textDst.SelectionEnd, curLoc.Lat, curLoc.Lng);
		}

		void ShowSearchResults (List<Place> results)
		{
			var lstViewDst = FindViewById<ListView> (Resource.Id.listViewDst);
			lstViewDst.Adapter = new GenericListAdapter<Place> (
				this, results, Android.Resource.Layout.TwoLineListItem,
				(v,item) => {
					v.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Name;
					v.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.Address;
				});
			lstViewDst.ItemClick += (s, e) => { 
				AppCommon.Inst.Destination = results[e.Position].Loc;
				this.StartNextActivity<ReqBookingDstActivity>();
				//StartActivity(new Intent(this, typeof(ReqBookingDstActivity)));
			};
		}
	}
}
