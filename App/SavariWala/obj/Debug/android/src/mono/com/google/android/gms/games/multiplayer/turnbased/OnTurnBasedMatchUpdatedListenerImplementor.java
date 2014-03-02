package mono.com.google.android.gms.games.multiplayer.turnbased;


public class OnTurnBasedMatchUpdatedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.games.multiplayer.turnbased.OnTurnBasedMatchUpdatedListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onTurnBasedMatchUpdated:(ILcom/google/android/gms/games/multiplayer/turnbased/TurnBasedMatch;)V:GetOnTurnBasedMatchUpdated_ILcom_google_android_gms_games_multiplayer_turnbased_TurnBasedMatch_Handler:Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchUpdatedListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchUpdatedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", OnTurnBasedMatchUpdatedListenerImplementor.class, __md_methods);
	}


	public OnTurnBasedMatchUpdatedListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == OnTurnBasedMatchUpdatedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchUpdatedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onTurnBasedMatchUpdated (int p0, com.google.android.gms.games.multiplayer.turnbased.TurnBasedMatch p1)
	{
		n_onTurnBasedMatchUpdated (p0, p1);
	}

	private native void n_onTurnBasedMatchUpdated (int p0, com.google.android.gms.games.multiplayer.turnbased.TurnBasedMatch p1);

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
