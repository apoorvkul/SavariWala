package mono.com.google.android.gms.games.leaderboard;


public class OnLeaderboardScoresLoadedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.games.leaderboard.OnLeaderboardScoresLoadedListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onLeaderboardScoresLoaded:(ILcom/google/android/gms/games/leaderboard/Leaderboard;Lcom/google/android/gms/games/leaderboard/LeaderboardScoreBuffer;)V:GetOnLeaderboardScoresLoaded_ILcom_google_android_gms_games_leaderboard_Leaderboard_Lcom_google_android_gms_games_leaderboard_LeaderboardScoreBuffer_Handler:Android.Gms.Games.LeaderBoard.IOnLeaderboardScoresLoadedListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Games.LeaderBoard.IOnLeaderboardScoresLoadedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", OnLeaderboardScoresLoadedListenerImplementor.class, __md_methods);
	}


	public OnLeaderboardScoresLoadedListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == OnLeaderboardScoresLoadedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Games.LeaderBoard.IOnLeaderboardScoresLoadedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onLeaderboardScoresLoaded (int p0, com.google.android.gms.games.leaderboard.Leaderboard p1, com.google.android.gms.games.leaderboard.LeaderboardScoreBuffer p2)
	{
		n_onLeaderboardScoresLoaded (p0, p1, p2);
	}

	private native void n_onLeaderboardScoresLoaded (int p0, com.google.android.gms.games.leaderboard.Leaderboard p1, com.google.android.gms.games.leaderboard.LeaderboardScoreBuffer p2);

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
