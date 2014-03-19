using System;
using System.Net;

namespace SavariWala.Common
{
	public class RestApiProviderBase
	{
		public void DownloadStringAsync (Action<string> handler, string url)
		{
			var wc = new WebClient();
			wc.DownloadStringCompleted += (s, e) =>
			{
				try
				{
					AppCommon.Inst.Log.Debug("{0}: {1}", url, e.Result);
					handler(e.Result);
				}
				catch (Exception ex)
				{
					AppCommon.Inst.Log.Error("Error Fetching Results from {0}: {1}", url, ex); 
				}
			};

			wc.DownloadStringAsync (new Uri (url));
		}
	}
}

