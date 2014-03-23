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
	public class ReqDetailsActivity : PlaceSearchActivityBase
	{
		// Test Cases:
		//	_dst is retained on screen reorientation/back button?

		// Bugs: Validation response delayed and no keyboard displayed after validation failure

		// TODO: isShared

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.ReqDetails);

			AppCommon.Inst.CurrentReq.StartTime = DateTime.Now.AddMinutes (0);
			var textStartTime = FindViewById<EditText> (Resource.Id.textStartTime);
			textStartTime.Text = DateTime.Now.TimeOfDay.ToString ("c").Substring (0, 5) +
				"(0 mins from now)";

			textStartTime.Click += (sender, e) => Utils.AlertOnEx(this, 
				"Invalid Input", "Please enter valid entries", 
				() => AppCommon.Inst.CurrentReq.StartTime = DateTime.Parse (textStartTime.Text));

			AppCommon.Inst.CurrentReq.Details.NumPax = 1;
			var textNumPax = FindViewById<EditText> (Resource.Id.textNumPax);
			textStartTime.Click += (sender, e) => Utils.AlertOnEx (this, 
				"Invalid Input", "Please enter valid entries", 
				() => AppCommon.Inst.CurrentReq.Details.NumPax = int.Parse (textNumPax.Text));

			var lv = FindViewById<ListView> (Resource.Id.listViewDst);
			var textDest = FindViewById<AutoCompleteTextView> (Resource.Id.textDest);
			textDest.TextChanged += AutoCompletePlaces;
			textDest.EditorAction += (s, e)=>  HandleEditorAction(textDest, lv, e);
			textDest.RequestFocus ();
		}

		protected override void OnItemClicked (MapPoint place)
		{
			AppCommon.Inst.Destination = place.Loc;
			this.StartNextActivity<ReqBookingDstActivity>();
		}
	}
}
