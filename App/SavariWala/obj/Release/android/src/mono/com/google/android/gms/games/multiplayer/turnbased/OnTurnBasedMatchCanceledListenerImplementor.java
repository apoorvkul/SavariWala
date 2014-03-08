package mono.com.google.android.gms.games.multiplayer.turnbased;


public class OnTurnBasedMatchCanceledListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.games.multiplayer.turnbased.OnTurnBasedMatchCanceledListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onTurnBasedMatchCanceled:(ILjava/lang/String;)V:GetOnTurnBasedMatchCanceled_ILjava_lang_String_Handler:Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchCanceledListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchCanceledListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", OnTurnBasedMatchCanceledListenerImplementor.class, __md_methods);
	}


	public OnTurnBasedMatchCanceledListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == OnTurnBasedMatchCanceledListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Games.MultiPlayer.TurnBased.IOnTurnBasedMatchCanceledListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onTurnBasedMatchCanceled (int p0, java.lang.String p1)
	{
		n_onTurnBasedMatchCanceled (p0, p1);
	}

	private native void n_onTurnBasedMatchCanceled (int p0, java.lang.String p1);

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
