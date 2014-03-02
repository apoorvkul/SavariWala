using System;
using System.Collections.Generic;

namespace SavariWala.Common
{
	public class UserData
	{
		public enum UserTypeEnum { Passenger, Driver };
		public UserTypeEnum UserType { get; set; }
		public string UserName { get; set; }
	}
}

