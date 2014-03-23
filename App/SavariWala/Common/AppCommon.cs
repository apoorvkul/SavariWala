using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using Thrift.Transport;
using Thrift.Protocol;
using Thrift;
using System.Threading;
using System.Reactive.Linq;

namespace SavariWala.Common
{
	public class Request 
	{
		private static DateTime Epoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public MapPoint Src { get; set; }
		public MapPoint Dst { get; set; }
		DateTime _startTime;
		public DateTime StartTime {
			get {
				return _startTime;
			}
			set {
				_startTime = value;
				Details.StartTime = (long)_startTime.ToUniversalTime ().Subtract (Epoch).TotalMilliseconds;
			}
		}
		public BookingParams Details { get; private set; }

		public Request() 
		{
			Details = new BookingParams ();
		}
	}

	public class AppCommon 
	{
		public const string FbAppId = "256081284574018";

		/// <summary>
		/// Extended permissions is a comma separated list of permissions to ask the user.
		/// </summary>
		/// <remarks>
		/// For extensive list of available extended permissions refer to 
		/// https://developers.facebook.com/docs/reference/api/permissions/
		/// </remarks>
		public const string ExtendedPermissions = "user_about_me,read_stream,publish_stream";

		public static AppCommon Inst { get; private set; }
			
		// TODO Server connection details should come from some configs 
		public string ServerAddr { get { return "192.168.0.12"; } }
		public int ServerPort { get { return 9090; } }

		public bool IsLoggedIn { get; private set; }
		public string FbAccessToken { get; set;}
		public UserData UserData { get; private set;}
		public AppData AppData { get; private set; }
		public Logger Log { get; private set; }
		public string GoogleApiKeyNative { get; private set; }
		public string GoogleApiKeyWeb { get; private set; }
		public PlacesProvider PlacesProvider { get; private set; }
		public Request CurrentReq { get; set; }
		public GeoLoc Destination { get; set; }
		public DirectionsProvider DirectionsProvider { get; private set; }
		public Int64 BookingId { get; private set; }
		public ErrorTranslator ErrorTranslator { get; set; }
		public ILocationProvider LocationProvider { get; private set; }
		public NearestPointProvider NearestPointProvider { get; private set; }
		public DynProp<bool>IsPassenger { get; private set; }
		public GeoLoc StartPoint { get; set; }

		public bool DisableServer { get { return true; } }

		public static void Create (string googleApiKeyNative, string googleApiKeyWeb, 
			ErrorTranslator errorTranslator, ILocationProvider locationProvider)
		{
			new AppCommon (googleApiKeyNative, googleApiKeyWeb, locationProvider) { 
				ErrorTranslator = errorTranslator, 
			};

			// Post construction inits
			ThreadPool.QueueUserWorkItem ((x) => Inst.LocationProvider.Connect ());
		}

		private AppCommon(string googleApiKeyNative, string googleApiKeyWeb, ILocationProvider locationProvider)
		{
			Inst = this;
			IsLoggedIn = false;
			GoogleApiKeyNative = googleApiKeyNative;
			GoogleApiKeyWeb = googleApiKeyWeb;

			Log = new Logger ();
			IsPassenger = new DynProp<bool> (true); 
			LoadAppData ();
			PlacesProvider = new PlacesProvider ();
			DirectionsProvider = new DirectionsProvider ();
			LocationProvider = locationProvider;
			NearestPointProvider = new NearestPointProvider ();

			IsPassenger.Value = AppData.IsLastUserPassenger;
		}

		private void LoadAppData()
		{
			try
			{
				using (var store = IsolatedStorageFile.GetUserStoreForApplication())
				{
					using (var fstream = store.OpenFile("AppData.xml", FileMode.OpenOrCreate , FileAccess.Read))
					{
						AppData = (AppData) new XmlSerializer(typeof(AppData)).Deserialize(fstream);
					}
				}
			}
			catch(Exception ex)
			{
				AppData = new AppData ();
				Log.Error (ex.ToString ());
			}
		}

		public static void ExceptionSafe(Action action, Action<TException> onTException)
		{
			try
			{
				action();
			}
			catch(TException e) {
				onTException (e);
			}
			catch (Exception e){ AppCommon.Inst.Log.Warn (e.ToString ());}
		}
			
		public void ConfirmBookingReq (Action onComplete, Action<TException> onTException)
		{
			DirectionsProvider.GetRoutesAsync ((json) => {
				ExceptionSafe(() => {
					CurrentReq.Details.RouteJson = json;
					using (var client = new RequestHandler.Client(GetThriftProtocol(PortOffsets.RequestHandler)))
					{
						BookingId = client.submitBooking(CurrentReq.Details);
					}
					onComplete();
				}, onTException);
			}, CurrentReq.Src.Loc, CurrentReq.Dst.Loc);
		}

		public void CancelBookingReq (Action onComplete, Action<TException> onTException)
		{
			ExceptionSafe(() => {
				using (var client = new RequestHandler.Client(GetThriftProtocol(PortOffsets.RequestHandler)))
				{
					client.cancel(BookingId);
				}
				onComplete();
			}, onTException);
		}

		public enum PortOffsets
		{
			UsersManager = 0,
			MapPointProvider = 1,
			RequestHandler = 2
		}
		public TProtocol GetThriftProtocol(PortOffsets offset) {
			var transport = new TSocket(ServerAddr, ServerPort + (int)offset);
			transport.Open();
			return new TBinaryProtocol (transport);  // TODO TCompactProtocol once server is ready for it
		}

		public void InitUser(string fbUserId)
		{
			if (DisableServer) {
				UserData = new UserData { FbUserId = fbUserId, UserName = "Disconnected User", IsPassenger = true };
				return;
			}

			UserData = AppData.KnownUserDatas.Find ((userData) => userData.FbUserId == fbUserId);
			if (UserData == null) {

				using (var client = new UsersManager.Client(GetThriftProtocol(PortOffsets.UsersManager)))
				{
					var user = client.getUser(fbUserId);
					UserData = new UserData { UserName = user.UserName, IsPassenger = user.IsPassenger };
				}
			}
			IsPassenger.Value = UserData.IsPassenger;
		}
	}
}