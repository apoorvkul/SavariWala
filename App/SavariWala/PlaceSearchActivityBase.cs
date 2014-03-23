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

namespace SavariWala.AndroidApp
{
	public abstract class PlaceSearchActivityBase : Activity
	{
		// Test Cases:
		//	_dst is retained on screen reorientation/back button?

		// Bugs: Validation response delayed and no keyboard displayed after validation failure

		// TODO: isShared

		protected void HandleEditorAction (AutoCompleteTextView tv, ListView lv, TextView.EditorActionEventArgs e) {
			if (e.ActionId == ImeAction.Go) {
				var curLoc = AppCommon.Inst.LocationProvider.CurLocInfo.Value.Loc;
				AppCommon.Inst.PlacesProvider.SearchAsync (
					x => RunOnUiThread (() => ShowSearchResults (x, lv)), tv.Text, curLoc.Lat, curLoc.Lng);
				// Hide keyboard
				var imm = (InputMethodManager)GetSystemService (Context.InputMethodService);
				imm.HideSoftInputFromWindow (tv.WindowToken, 0);
				e.Handled = true;
			}
		}

		protected void AutoCompletePlaces (object sender, Android.Text.TextChangedEventArgs e)
		{
			var curLoc = AppCommon.Inst.LocationProvider.CurLocInfo.Value.Loc;
			var textDst = (AutoCompleteTextView)sender;
			AppCommon.Inst.PlacesProvider.AutoCompleteAsync (
				lst => RunOnUiThread(() => textDst.Adapter = new ArrayAdapter (this, Android.Resource.Layout.SimpleListItem1, lst)),
				textDst.Text, textDst.SelectionEnd, curLoc.Lat, curLoc.Lng);
		}

		protected abstract  void OnItemClicked (MapPoint place);

		void ShowSearchResults (List<MapPoint> results, ListView lv)
		{

			lv.Adapter = new GenericListAdapter<MapPoint> (
				this, results, Android.Resource.Layout.TwoLineListItem,
				(v,item) => {
					v.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Name;
					v.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.Address;
				});
			lv.ItemClick += (s, e) => OnItemClicked (results [e.Position]);
		}
	}
}

