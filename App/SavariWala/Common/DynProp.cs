using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SavariWala.Common
{
	public class DynProp<T>
	{
		T _value;
		public T Value {
			get {
				return _value;
			}
			set {
				var e = new EvtArgs { OldValue = Value };
				_value = value;
				e.NewValue = Value;
				OnEvent (e);
			}
		}

		public class EvtArgs : EventArgs
		{
			public T OldValue { get; set; }
			public T NewValue { get; set; }
		}

		public event EventHandler<EvtArgs> Event;

		protected virtual void OnEvent (EvtArgs e)
		{
			var handler = Event;
			if (handler != null)
				handler (this, e);
		}

		public DynProp(T init)
		{
			_value = init;
		}
	}
}

