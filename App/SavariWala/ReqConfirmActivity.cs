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
using Thrift;

namespace SavariWala.AndroidApp
{
	[Activity (Label = "@string/reqConfirm")]			
	public class ReqConfirmActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.ReqConfirm);

			var req = AppCommon.Inst.CurrentReq;

			var textStartPt = FindViewById<TextView> (Resource.Id.textStartPt);
			textStartPt.Text = req.Src.ToString ();

			var textEndPt = FindViewById<TextView> (Resource.Id.textEndPt);
			textEndPt.Text = req.Dst.ToString ();

			var textStartTime = FindViewById<TextView> (Resource.Id.textStartTm);
			textStartTime.Text = req.StartTime.ToString ("f");


			SetupConfirm (false);
		}

		void SetupConfirm (bool reset)
		{
			var btnAction = FindViewById<TextView> (Resource.Id.btnAction);
			btnAction.Click += (sender, e) => {
				btnAction.Enabled = false;
				AppCommon.Inst.ConfirmBookingReq (
					() => RunOnUiThread(() => SetupCancel ()),
					(ex) => Utils.HandleTException (this, ex));
			};

			if(reset)
			{
				var textPending = FindViewById<TextView> (Resource.Id.textPending);
				textPending.Visibility = ViewStates.Invisible;

				btnAction.Text = Resources.GetString(Resource.String.submit);
				this.Window.SetTitle(Resources.GetString(Resource.String.reqConfirm));
				btnAction.Enabled = true;
			}
		}
			
		void SetupCancel ()
		{
			var textPending = FindViewById<TextView> (Resource.Id.textPending);
			textPending.Visibility = ViewStates.Visible;

			var btnAction = FindViewById<Button> (Resource.Id.btnAction);
			btnAction.Text = Resources.GetString(Resource.String.cancel);
			btnAction.Enabled = true;
			this.Window.SetTitle(Resources.GetString(Resource.String.reqCancel));

			btnAction.Click += (sender, e) => {
				btnAction.Enabled = false;
				AppCommon.Inst.CancelBookingReq(
					() => RunOnUiThread(() => SetupConfirm (true)),
					(ex) => Utils.HandleTException (this, ex));
			};
		}
	}
}

