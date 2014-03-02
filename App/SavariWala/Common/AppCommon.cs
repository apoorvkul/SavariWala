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

namespace SavariWala.Common
{
	public class Request 
	{
		public Place Src { get; set; }
		public Place Dst { get; set; }
		public DateTime StartTime { get; set; }
		public bool Shared { get; set; }
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
			
		public bool IsLoggedIn { get; private set; }
		public string FbAccessToken { get; set;}
		public UserData UserData { get; private set;}
		public AppData AppData { get; private set; }
		public Logger Log { get; private set; }
		public string GoogleApiKeyNative { get; private set; }
		public string GoogleApiKeyWeb { get; private set; }
		public PlacesProvider PlacesProvider { get; private set; }
		public Request CurrentReq { get; set; }
		public GeoLoc CurLoc { get; set; }
		public GeoLoc Destination { get; set; }

		public AppCommon(string googleApiKeyNative, string googleApiKeyWeb)
		{
			Inst = this;
			IsLoggedIn = false;
			GoogleApiKeyNative = googleApiKeyNative;
			GoogleApiKeyWeb = googleApiKeyWeb;
			Log = new Logger ();
			LoadAppData ();
			PlacesProvider = new PlacesProvider ();
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

		public void InitUser(string userName, bool loggedIn)
		{
			UserData = AppData.KnownUserDatas.Find ((userData) => userData.UserName == userName);
			if (UserData == null) {
				// TODO Fetch from server
				UserData = new UserData { UserName = userName, UserType = UserData.UserTypeEnum.Passenger };
			}
		}


	}
}

