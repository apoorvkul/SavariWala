package mono.com.google.android.gms.games.multiplayer.turnbased;


public class OnTurnBasedMatchesLoadedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.games.multiplayer.turnbased.OnTurnBasedMatchesLoadedListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onTurnBasedMatchesLoaded:(ILcom/google/android/gms/games/multiplayer/turnbased/LoadMatchesResponse;)V:GetOnTurnBasedMatchesLoaded_ILcom_google_android_gms_games_multiplayer_turnbased_LoadMatchesResponse_Handler:Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchesLoadedListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchesLoadedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", OnTurnBasedMatchesLoadedListenerImplementor.class, __md_methods);
	}


	public OnTurnBasedMatchesLoadedListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == OnTurnBasedMatchesLoadedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchesLoadedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onTurnBasedMatchesLoaded (int p0, com.google.android.gms.games.multiplayer.turnbased.LoadMatchesResponse p1)
	{
		n_onTurnBasedMatchesLoaded (p0, p1);
	}

	private native void n_onTurnBasedMatchesLoaded (int p0, com.google.android.gms.games.multiplayer.turnbased.LoadMatchesResponse p1);

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
