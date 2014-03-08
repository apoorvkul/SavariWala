package savariwala.androidapp;


public class LocationProvider
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.location.LocationListener,
		com.google.android.gms.common.GooglePlayServicesClient.ConnectionCallbacks,
		com.google.android.gms.common.GooglePlayServicesClient.OnConnectionFailedListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onLocationChanged:(Landroid/location/Location;)V:GetOnLocationChanged_Landroid_location_Location_Handler:Android.Gms.Location.ILocationListenerInvoker, GooglePlayServicesLib\n" +
			"n_onConnected:(Landroid/os/Bundle;)V:GetOnConnected_Landroid_os_Bundle_Handler:Android.Gms.Common.IGooglePlayServicesClientConnectionCallbacksInvoker, GooglePlayServicesLib\n" +
			"n_onDisconnected:()V:GetOnDisconnectedHandler:Android.Gms.Common.IGooglePlayServicesClientConnectionCallbacksInvoker, GooglePlayServicesLib\n" +
			"n_onConnectionFailed:(Lcom/google/android/gms/common/ConnectionResult;)V:GetOnConnectionFailed_Lcom_google_android_gms_common_ConnectionResult_Handler:Android.Gms.Common.IGooglePlayServicesClientOnConnectionFailedListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("SavariWala.AndroidApp.LocationProvider, SavariWala, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", LocationProvider.class, __md_methods);
	}


	public LocationProvider () throws java.lang.Throwable
	{
		super ();
		if (getClass () == LocationProvider.class)
			mono.android.TypeManager.Activate ("SavariWala.AndroidApp.LocationProvider, SavariWala, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onLocationChanged (android.location.Location p0)
	{
		n_onLocationChanged (p0);
	}

	private native void n_onLocationChanged (android.location.Location p0);


	public void onConnected (android.os.Bundle p0)
	{
		n_onConnected (p0);
	}

	private native void n_onConnected (android.os.Bundle p0);


	public void onDisconnected ()
	{
		n_onDisconnected ();
	}

	private native void n_onDisconnected ();


	public void onConnectionFailed (com.google.android.gms.common.ConnectionResult p0)
	{
		n_onConnectionFailed (p0);
	}

	private native void n_onConnectionFailed (com.google.android.gms.common.ConnectionResult p0);

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
