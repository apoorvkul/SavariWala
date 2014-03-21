using System;

namespace SavariWala.Common
{
	public class GeoLocEvtArgs : EventArgs
	{
		public GeoLoc GeoLoc { get ; set; }
		public Nullable<int> Accuracy { get ; set; }
	}

	public interface ILocationProvider
	{
		event EventHandler<GeoLocEvtArgs> CurLocChanged;

		GeoLoc CurLoc { get; set;}
		Nullable<int> CurAccuracy { get; set;}

		void Connect();
	}
}

