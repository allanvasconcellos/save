using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views;
using Android.Widget;
using INetSales.Objects;

namespace INetSales.AndroidUi.Controls
{
    public class DtoAdapter<TDto> : BaseAdapter<TDto>
        where TDto : IDto
    {
        private List<TDto> _dtos;
        public IEnumerable<TDto> Content
        {
            get { return _dtos.AsEnumerable(); }
        }

        /// <summary>
        /// 1a - Posição
        /// 2a - Convert View
        /// 3a - View Group
        /// 4a - item dto
        /// </summary>
        public Func<int, View, ViewGroup, TDto, View> BindingGetView { get; set; }

        public DtoAdapter()
            : this(null)
        {
        }

        public DtoAdapter(IEnumerable<TDto> dtos)
        {
            _dtos = new List<TDto>(dtos ?? new TDto[] {});
        }

        public void UpdateContent(IEnumerable<TDto> dtos)
        {
            _dtos = new List<TDto>(dtos);
            NotifyDataSetChanged();
        }

        public override long GetItemId(int position)
        {
            return _dtos[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var dto = _dtos.ElementAt(position);
            return BindingGetView(position, convertView, parent, dto);
        }

        public override int Count
        {
            get { return _dtos.Count; }
        }

        public override TDto this[int position]
        {
            get { return _dtos[position]; }
        }
    }
}