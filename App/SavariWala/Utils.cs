using System;
using Android.App;
using Android.Content;

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
	}
}

