using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Json;

namespace SavariWala.Common
{
	public class DirectionsProvider : RestApiProviderBase
	{
		private const string DirectionApiUrlFmt = 
			"http://maps.googleapis.com/maps/api/directions/json?origin={0},{1}&destination={2},{3}&sensor={4}"; //&alternatives=true";

		public void GetRoutesAsync (Action<string> callback, GeoLoc src, GeoLoc dst)
		{
			DownloadStringAsync(r => callback(r), 
				String.Format(DirectionApiUrlFmt, src.Lat, src.Lng, dst.Lat, dst.Lng, "true"));
		}

	}
}