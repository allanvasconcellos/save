using System;
using System.Linq;
using Android.Widget;
using Android.Runtime;
using System.Collections.Generic;
using Android.Content;
using Java.Lang;
using System.Collections;
using Android.Views;

namespace INetSales.AndroidUi.Controls
{
	public class AdapterEventArgs<T> : EventArgs
	{
		public int Position { get; }
		public View ConvertView { get; }
		public ViewGroup Parent { get; }
		public T Item { get; }

		public AdapterEventArgs(int position, T item, View convertView, ViewGroup parent)
		{
			Position = position;
			ConvertView = convertView;
			Parent = parent;
			Item = item;
		}
	}

	public class FilterableAdapter<T> : BaseAdapter<T>, IFilterable
	{
		private List<T> filteredItems;
		private ItemFilter<T> mFilter;

		public FilterableAdapter ()
		{
			mFilter = new ItemFilter<T>(this);
			filteredItems = new List<T> ();
		}

		public delegate View BindingEventHandler(object sender, AdapterEventArgs<T> args);
		public event BindingEventHandler OnBindingView;
		private View DoBindingView(int position, T item, View convertView, ViewGroup parent)
		{
			if (OnBindingView != null) {
				var args = new AdapterEventArgs<T> (position, item, convertView, parent);
				return OnBindingView (this, args);
			}
			return null;
		}

		public delegate IEnumerable<T> FilterEventHandler(object sender, string filtro);
		public event FilterEventHandler OnFilter;
		private IEnumerable<T> DoFilter(string filtro)
		{
			if (OnFilter != null) {
				return OnFilter (this, filtro);
			}
			return null;
		}

		public Filter Filter {
			get {
				return mFilter;
			}
		}

		#region implemented abstract members of BaseAdapter

		public override T this[int position]
		{
			get { return filteredItems[position]; }
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override Android.Views.View GetView (int position, View convertView, ViewGroup parent)
		{
			var item = filteredItems [position];
			return DoBindingView(position, item, convertView, parent);
		}

		public override int Count {
			get {
				return filteredItems.Count;
			}
		}

		#endregion

		private class ItemFilter<TItem> : Filter
		{
			FilterableAdapter<TItem> adapter;

			public ItemFilter(FilterableAdapter<TItem> adapter)
			{
				this.adapter = adapter;
			}

			protected override FilterResults PerformFiltering (ICharSequence constraint)
			{
				string filterString = constraint.ToString().ToLower();
				FilterResults results = new FilterResults();

				JavaObject<IEnumerable<TItem>> tempItems = new JavaObject<IEnumerable<TItem>> (adapter.DoFilter (filterString));;

				results.Values = tempItems;
				results.Count = tempItems.Value.Count();

				return results;
			}

			protected override void PublishResults(ICharSequence constraint, FilterResults results) {
				adapter.filteredItems = new List<TItem>(((JavaObject<IEnumerable<TItem>>) results.Values).Value);
				adapter.NotifyDataSetChanged();
			}
		}

		private class JavaObject<TObject> : Java.Lang.Object
		{
			public JavaObject (TObject obj)
			{
				this.Value = obj;
			}

			public TObject Value { get; private set; }
		}

	}
}

