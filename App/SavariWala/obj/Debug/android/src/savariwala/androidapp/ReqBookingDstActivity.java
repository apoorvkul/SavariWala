package savariwala.androidapp;


public class ReqBookingDstActivity
	extends savariwala.androidapp.MapPointsActivityBase
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("SavariWala.AndroidApp.ReqBookingDstActivity, SavariWala, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ReqBookingDstActivity.class, __md_methods);
	}


	public ReqBookingDstActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ReqBookingDstActivity.class)
			mono.android.TypeManager.Activate ("SavariWala.AndroidApp.ReqBookingDstActivity, SavariWala, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public ReqBookingDstActivity (int p0, int p1, com.google.android.gms.maps.model.LatLng p2) throws java.lang.Throwable
	{
		super ();
		if (getClass () == ReqBookingDstActivity.class)
			mono.android.TypeManager.Activate ("SavariWala.AndroidApp.ReqBookingDstActivity, SavariWala, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:System.Int32, mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e:Android.Gms.Maps.Model.LatLng, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0, p1, p2 });
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
