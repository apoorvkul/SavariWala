using System;
using System.Linq;
using Android.App;
using Android.Content;
using System.Collections.Generic;
using Thrift;
using SavariWala.Common;

namespace SavariWala.AndroidApp
{
	public class Utils
	{
		static public void Alert (Context ctx, string title, string message, bool CancelButton, Action<Result> callback = null)
		{
			AlertDialog.Builder builder = new AlertDialog.Builder(ctx);
			builder.SetTitle(title);
			builder.SetIcon(Resource.Drawable.Icon);
			builder.SetMessage(message);

			builder.SetPositiveButton("Ok", (sender, e) => {
				if(callback != null) callback(Result.Ok);
			});

			if (CancelButton) {
				builder.SetNegativeButton("Cancel", (sender, e) => {
					if(callback != null) callback(Result.Canceled);
				});
			}

			builder.Show();
		}
	
		static public void AlertOnEx (Context ctx, string title, string message, Action action)//, List<Type> exTypes = null)
		{
			try 
			{
				action ();
			} 
			catch //(Exception e) 
			{
				//if (exTypes == null || exTypes.Contains (typeof(e)))
					Utils.Alert (ctx, title, message, false); 
			}
		}
	
		public static void HandleTException (Context ctx, TException ex)
		{
			var errMsg = AppCommon.Inst.ErrorTranslator.GetErrMessage (ex);
			if(errMsg != null)
				Utils.Alert (ctx, errMsg.Item1, errMsg.Item2, false);
			else
				AppCommon.Inst.Log.Error (ex.ToString ());
		}
	}
}

