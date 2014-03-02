package mono.com.google.android.gms.games.multiplayer.turnbased;


public class OnTurnBasedMatchLoadedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.games.multiplayer.turnbased.OnTurnBasedMatchLoadedListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onTurnBasedMatchLoaded:(ILcom/google/android/gms/games/multiplayer/turnbased/TurnBasedMatch;)V:GetOnTurnBasedMatchLoaded_ILcom_google_android_gms_games_multiplayer_turnbased_TurnBasedMatch_Handler:Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchLoadedListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchLoadedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", OnTurnBasedMatchLoadedListenerImplementor.class, __md_methods);
	}


	public OnTurnBasedMatchLoadedListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == OnTurnBasedMatchLoadedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchLoadedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onTurnBasedMatchLoaded (int p0, com.google.android.gms.games.multiplayer.turnbased.TurnBasedMatch p1)
	{
		n_onTurnBasedMatchLoaded (p0, p1);
	}

	private native void n_onTurnBasedMatchLoaded (int p0, com.google.android.gms.games.multiplayer.turnbased.TurnBasedMatch p1);

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
