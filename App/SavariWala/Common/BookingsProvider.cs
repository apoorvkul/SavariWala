using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SavariWala.Common
{
	public class BookingsProvider
	{
		bool _first = true;
		public BookingsProvider()
		{
			AppCommon.Inst.LocationProvider.CurLocInfo.Event += OnCurLocChanged;
		}

		void FetchMatchingBooking ()
		{
			var loc = AppCommon.Inst.LocationProvider.CurLocInfo.Value.Loc;
			using (var client = new RequestHandler.Client (AppCommon.Inst.GetThriftProtocol (AppCommon.PortOffsets.RequestHandler))){
				client.matchBooking (AppCommon.Inst.LocationProvider.CurLocInfo.Value.Loc);
			}
		}

		void OnCurLocChanged (object sender, DynProp<LocInfo>.EvtArgs e)
		{
			if (_first)
				FetchMatchingBooking ();
			// TODO: Maintain drivers state. If he/she has kept app open without picking up booking 
			// and is driving around, we should update the matched bookings beyond certain distance threshold
		}
	}
}

