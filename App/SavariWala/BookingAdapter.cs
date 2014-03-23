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

namespace SavariWala.AndroidApp
{
	public class BookingAdapter : BaseAdapter<PooledBooking>
	{
		List<PooledBooking> items;
		Activity context;

		public BookingAdapter(Activity context, List<PooledBooking> items)
			: base()
		{
			this.context = context;
			this.items = items;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override PooledBooking this[int position]
		{
			get { return items[position]; }
		}

		public override int Count
		{
			get { return items.Count; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];
			View view = convertView;
			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout.BookingList, null);
			view.FindViewById<TextView>(Resource.Id.tvFrom).Text = item.Src.Locality;
			view.FindViewById<TextView>(Resource.Id.tvTo).Text = item.Dst.Locality;
			view.FindViewById<TextView>(Resource.Id.tvNumPax).Text = item.NumPax.ToString();
			view.FindViewById<TextView>(Resource.Id.tvTime).Text = item.StartTime.ToString();
			return view;
		}
	}
}

