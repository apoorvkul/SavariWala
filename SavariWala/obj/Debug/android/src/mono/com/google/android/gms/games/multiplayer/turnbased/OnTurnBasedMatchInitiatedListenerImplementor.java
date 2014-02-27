package mono.com.google.android.gms.games.multiplayer.turnbased;


public class OnTurnBasedMatchInitiatedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.games.multiplayer.turnbased.OnTurnBasedMatchInitiatedListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onTurnBasedMatchInitiated:(ILcom/google/android/gms/games/multiplayer/turnbased/TurnBasedMatch;)V:GetOnTurnBasedMatchInitiated_ILcom_google_android_gms_games_multiplayer_turnbased_TurnBasedMatch_Handler:Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchInitiatedListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchInitiatedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", OnTurnBasedMatchInitiatedListenerImplementor.class, __md_methods);
	}


	public OnTurnBasedMatchInitiatedListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == OnTurnBasedMatchInitiatedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchInitiatedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onTurnBasedMatchInitiated (int p0, com.google.android.gms.games.multiplayer.turnbased.TurnBasedMatch p1)
	{
		n_onTurnBasedMatchInitiated (p0, p1);
	}

	private native void n_onTurnBasedMatchInitiated (int p0, com.google.android.gms.games.multiplayer.turnbased.TurnBasedMatch p1);

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
