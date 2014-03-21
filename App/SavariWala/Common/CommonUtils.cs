using System;

namespace SavariWala.Common
{
	public class CommonUtils
	{
		const double MetrePerDegLat =  111.2 * 1000;

		public static double GeoDistance (GeoLoc pt1, GeoLoc pt2)
		{
			double MetrePerDegLng = MetrePerDegLat * Math.Abs(Math.Cos (pt1.Lat));
			return MetrePerDegLat *  Math.Abs(pt1.Lat - pt2.Lat) + MetrePerDegLng * Math.Abs(pt1.Lng - pt2.Lng);
		}
	}
}

