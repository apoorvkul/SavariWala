package mono.com.google.android.gms.games.leaderboard;


public class OnPlayerLeaderboardScoreLoadedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.games.leaderboard.OnPlayerLeaderboardScoreLoadedListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onPlayerLeaderboardScoreLoaded:(ILcom/google/android/gms/games/leaderboard/LeaderboardScore;)V:GetOnPlayerLeaderboardScoreLoaded_ILcom_google_android_gms_games_leaderboard_LeaderboardScore_Handler:Android.Gms.Games.LeaderBoard.IOnPlayerLeaderboardScoreLoadedListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Games.LeaderBoard.IOnPlayerLeaderboardScoreLoadedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", OnPlayerLeaderboardScoreLoadedListenerImplementor.class, __md_methods);
	}


	public OnPlayerLeaderboardScoreLoadedListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == OnPlayerLeaderboardScoreLoadedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Games.LeaderBoard.IOnPlayerLeaderboardScoreLoadedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onPlayerLeaderboardScoreLoaded (int p0, com.google.android.gms.games.leaderboard.LeaderboardScore p1)
	{
		n_onPlayerLeaderboardScoreLoaded (p0, p1);
	}

	private native void n_onPlayerLeaderboardScoreLoaded (int p0, com.google.android.gms.games.leaderboard.LeaderboardScore p1);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
