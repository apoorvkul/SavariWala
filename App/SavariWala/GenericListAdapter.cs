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
	public class GenericListAdapter<TItem> : BaseAdapter<TItem> 
	{
		List<TItem> items_;
		Activity context_;
		Action<View, TItem> binder_;
		int id_;

		public GenericListAdapter(Activity context, List<TItem> items, 
			int id, Action<View, TItem> binder)
			: base()
		{
			context_ = context;
			items_ = items;
			id_ = id;
			binder_ = binder;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override TItem this[int position]
		{   
			get { return items_[position]; } 
		}
		public override int Count {
			get { return items_.Count; } 
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView;
			if (view == null)
				view = context_.LayoutInflater.Inflate(id_, null);

			binder_(view, items_[position]);

			//View view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);
			//view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;
			//view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.SubHeading;

			//View view = context.LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, null);
			//view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;
			//view.FindViewById<ImageView>(Android.Resource.Id.Icon).SetImageResource(item.ImageResourceId);

			//View view = context.LayoutInflater.Inflate(Android.Resource.Layout.TwoLineListItem, null);
			//view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = item.Heading;
			//view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = item.SubHeading;

			return view;
		}
	}
}

