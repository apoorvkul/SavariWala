package savariwala.androidapp;


public abstract class PlaceSearchActivityBase
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("SavariWala.AndroidApp.PlaceSearchActivityBase, SavariWala, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", PlaceSearchActivityBase.class, __md_methods);
	}


	public PlaceSearchActivityBase () throws java.lang.Throwable
	{
		super ();
		if (getClass () == PlaceSearchActivityBase.class)
			mono.android.TypeManager.Activate ("SavariWala.AndroidApp.PlaceSearchActivityBase, SavariWala, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

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
