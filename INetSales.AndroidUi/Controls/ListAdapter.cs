using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;
using INetSales.Objects;

namespace INetSales.AndroidUi.Controls
{
    public class ControlItem
    {
        public int Id { get; set; }

        public string Descricao { get; set; }

        public bool IsDefault { get; set; }
    }

    public class ListAdapter<TItem> : BaseAdapter<TItem>
        where TItem : class
    {
        private readonly List<TItem> _list;

        public ListAdapter(IEnumerable<TItem> list)
        {
            _list = new List<TItem>(list);
        }

        public Func<int, TItem, View> BindingGetView { get; set; }

        #region Overrides of BaseAdapter

        public override long GetItemId(int position)
        {
            var item = this[position];
            if(item is ControlItem)
            {
                return (item as ControlItem).Id;
            }
            if(item is IDto)
            {
                return (item as IDto).Id;
            }
            throw new NotImplementedException();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var dto = _list.ElementAt(position);
            return BindingGetView(position, dto);
        }

        public override int Count
        {
            get { return _list.Count; }
        }

        #endregion

        #region Overrides of BaseAdapter<AdapterItem>

        public override TItem this[int position]
        {
            get { return _list[position]; }
        }

        #endregion
    }
}