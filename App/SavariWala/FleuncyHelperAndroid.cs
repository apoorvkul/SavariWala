using System;
using Android.App;
using Android.Content;

namespace SavariWala.AndroidApp
{
	// Holder for extentsion methods improving programming fluency
	public static class FleuncyHelperAndroid
	{

		public static void StartNextActivity<TActivity>(this Activity curAct)
		{
			curAct.StartActivity (new Intent (curAct, typeof(TActivity)));
		}
	}
}

