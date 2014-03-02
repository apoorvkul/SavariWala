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

			AppCommon.Inst.CurrentReq.StartTime = DateTime.Now.Add (TimeSpan.FromMinutes (0));

			var textStartTime = FindViewById<EditText> (Resource.Id.textStartTime);
			textStartTime.Text = DateTime.Now.TimeOfDay.ToString ("c").Substring (0, 5) +
				"(Omins from now)";
			//TODO: Only current booking for now
			textStartTime.Enabled = false;

			var textDest = FindViewById<AutoCompleteTextView> (Resource.Id.textDest);
			textDest.TextChanged += AutoCompletePlaces;
			textDest.EditorAction += (sender, e) => {
				if (e.ActionId == ImeAction.Go) {
					var curLoc = AppCommon.Inst.CurLoc;
					AppCommon.Inst.PlacesProvider.SearchAsync (x => RunOnUiThread(() => ShowSearchResults(x)),
						textDest.Text, curLoc.Lat, curLoc.Lng);
					// Hide keyboard
					var imm = (InputMethodManager) GetSystemService(Context.InputMethodService);
					imm.HideSoftInputFromWindow(textDest.WindowToken, 0);
					e.Handled = true; 
				}
			};
		}

		void AutoCompletePlaces (object sender, Android.Text.TextChangedEventArgs e)
		{
			var curLoc = AppCommon.Inst.CurLoc;
			var textDst = (AutoCompleteTextView)sender;
			AppCommon.Inst.PlacesProvider.AutoCompleteAsync (
				lst => RunOnUiThread(() => textDst.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1, lst)),
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
			};
		}
	}
}
