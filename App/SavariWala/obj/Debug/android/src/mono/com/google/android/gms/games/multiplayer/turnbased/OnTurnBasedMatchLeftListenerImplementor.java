package mono.com.google.android.gms.games.multiplayer.turnbased;


public class OnTurnBasedMatchLeftListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.games.multiplayer.turnbased.OnTurnBasedMatchLeftListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onTurnBasedMatchLeft:(ILcom/google/android/gms/games/multiplayer/turnbased/TurnBasedMatch;)V:GetOnTurnBasedMatchLeft_ILcom_google_android_gms_games_multiplayer_turnbased_TurnBasedMatch_Handler:Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchLeftListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchLeftListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", OnTurnBasedMatchLeftListenerImplementor.class, __md_methods);
	}


	public OnTurnBasedMatchLeftListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == OnTurnBasedMatchLeftListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchLeftListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onTurnBasedMatchLeft (int p0, com.google.android.gms.games.multiplayer.turnbased.TurnBasedMatch p1)
	{
		n_onTurnBasedMatchLeft (p0, p1);
	}

	private native void n_onTurnBasedMatchLeft (int p0, com.google.android.gms.games.multiplayer.turnbased.TurnBasedMatch p1);

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
