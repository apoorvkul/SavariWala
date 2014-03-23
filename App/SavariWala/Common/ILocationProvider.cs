using System;

namespace SavariWala.Common
{

	public class LocInfo
	{
		public GeoLoc Loc { get; set;}
		public int? Accuracy { get; set;}
	}
		
	public interface ILocationProvider
	{
		DynProp<LocInfo> CurLocInfo { get; set;}

		void Connect();
	}
}

