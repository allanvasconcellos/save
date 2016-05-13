using System;
using System.Linq;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;

namespace INetSales.AndroidUi.Controls
{
    public class BuildList
    {
        private readonly LinearLayout _layoutMain;
        private readonly ListView _list;
        private IListAdapter _adapter;
        private Func<View> _renderRodape;

        #region Static Members

        public static BuildList Use(ListView list)
        {
            var layout = new LinearLayout(list.Context);
            layout.Orientation = Orientation.Vertical;
            return new BuildList(layout, list);
        }

        #endregion

        private BuildList(LinearLayout layoutMain, ListView list)
        {
            _layoutMain = layoutMain;
            _list = list;
        }

        public BuildList Render<TItem>(IEnumerable<TItem> itens, Func<int, TItem, View> render) where TItem : class
        {
            _adapter = new ListAdapter<TItem>(itens)
                           {
                               BindingGetView = (position, item) => position == itens.Count() + 1 && _renderRodape != null ? 
                                   _renderRodape() : render(position, item)
                           };
            //_layoutMain.AddView(_list);

            return this;
        }

        public BuildList RenderRodape(Func<View> render)
        {
            _renderRodape = render;
            return this;
        }

        public LinearLayout Build()
        {
            _list.Adapter = _adapter;
            return _layoutMain;
        }
    }
}