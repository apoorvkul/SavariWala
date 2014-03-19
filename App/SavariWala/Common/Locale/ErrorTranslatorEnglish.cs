using System;
using Thrift;

namespace SavariWala.Common.Locale
{
	public class ErrorTranslatorEnglish : ErrorTranslator
	{
		public override Tuple<string, string> GetErrMessage (TException ex)
		{
			var se = ex as ServerError;
			if (se == null)
				return Tuple.Create ("No Internet Connectivity", "Please ensure Wifi/data network is enabled and reachable"); 
			switch(se.Err) {
			case ErrorCode.InvalidArg: 
				return Tuple.Create("Invalid Input", "Please check and enter valid entries");
			case ErrorCode.UserNotFound:
				return Tuple.Create("Please Register", "User not registered. Please sign up.");
			default:
				return null;
			}
		}

	}
}

