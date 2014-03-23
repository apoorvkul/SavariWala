using System;
using System.Collections.Generic;


namespace SavariWala.Common
{
	[Serializable]
	public class AppData
	{
		public List<UserData> KnownUserDatas {get; set;}
		public bool IsLastUserPassenger { get; set;}

		public AppData()
		{
			KnownUserDatas = new List<UserData> ();
			IsLastUserPassenger = true;
		}
	}
}

