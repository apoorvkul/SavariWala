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
using SavariWala.Common;

namespace SavariWala.AndroidApp
{
	[Activity (Label = "@string/seachStartActivity")]			
	public class SearchStartActivity : PlaceSearchActivityBase
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.SearchStart);

			var lv = FindViewById<ListView> (Resource.Id.listViewSrcs);
			var textStart = FindViewById<AutoCompleteTextView> (Resource.Id.textSeachStart);
			textStart.TextChanged += AutoCompletePlaces;
			textStart.EditorAction += (s, e)=>  HandleEditorAction(textStart, lv, e);
		}

		protected override void OnItemClicked (MapPoint place)
		{
			AppCommon.Inst.StartPoint = place.Loc;
			this.StartNextActivity<ReqBookingSrcActivity> ();
		}
	}
}

