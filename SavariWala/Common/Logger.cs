using System;

namespace SavariWala.Common
{
	// Basic Logger, some selective server upload/alerting functionality should be added
	public class Logger
	{
		public enum Severity { Fatal, Error, Info, Debug };

		public static Logger Inst { get; private set;}  

		public Logger ()
		{
		
		}

		public void Log (Severity sev, string msg)
		{
			Console.WriteLine (sev.ToString () + ": " + msg);
		}

		public void Fatal(Object o) 
		{
			Log (Severity.Fatal, o.ToString ());
		}


		public void Error(Object o) 
		{
			Log (Severity.Error, o.ToString ());
		}


		public void Info(Object o) 
		{
			Log (Severity.Info, o.ToString ());
		}


		public void Debug(Object o) 
		{
			Log (Severity.Debug, o.ToString ());
		}
			
	}
}

