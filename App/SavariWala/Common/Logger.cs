using System;

namespace SavariWala.Common
{
	// Basic Logger, some selective server upload/alerting functionality should be added
	public class Logger
	{
		public enum Severity { Fatal, Error, Warn, Info, Debug };

		public static Logger Inst { get; private set;}  

		public Logger ()
		{
		}

		public void Log (Severity sev, string msg)
		{
			Console.WriteLine (sev.ToString () + ": " + msg);
		}

		public void Fatal(string format, params Object[] obj) 
		{
			Log (Severity.Fatal, obj.Length > 0? String.Format (format, obj) : format );
		}
			
		public void Error(string format, params Object[] obj) 
		{
			Log (Severity.Error, obj.Length > 0? String.Format (format, obj) : format );
		}
			
		public void Warn(string format, params Object[] obj) 
		{
			Log (Severity.Warn, obj.Length > 0? String.Format (format, obj) : format );
		}

		public void Info(string format, params Object[] obj) 
		{
			Log (Severity.Info, obj.Length > 0? String.Format (format, obj) : format );
		}
			
		public void Debug(string format, params Object[] obj) 
		{
			Log (Severity.Debug, obj.Length > 0? String.Format (format, obj) : format );
		}
			
	}
}

