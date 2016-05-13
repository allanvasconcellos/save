using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace INetSales.AndroidUi.Controls
{
    public class BuildTableLayout
    {
        private readonly TableLayout _layoutMain;
        private readonly TableLayout _layoutCurrent;
        private int _firstRowIndex;
        private int _currentRowIndex;
        private int _currentPositionRow;

        #region Static Members

        public static BuildTableLayout Create(Context context, Orientation orientation)
        {
            var layout = new TableLayout(context);
            layout.Orientation = orientation;
            return new BuildTableLayout(layout);
        }

        public static BuildTableLayout Use(TableLayout layout, Orientation orientation)
        {
            layout.Orientation = orientation;
            return new BuildTableLayout(layout);
        }

        #endregion

        private BuildTableLayout(TableLayout layoutMain)
        {
            _layoutMain = layoutMain;
            _layoutCurrent = _layoutMain;

            for (int i = 0; i < _layoutCurrent.ChildCount; i++)
            {
                var child = _layoutCurrent.GetChildAt(i);
                if(child.Tag != null && child.Tag.Equals("row"))
                {
                    if (_firstRowIndex == 0)
                    {
                        _firstRowIndex = i;
                        _currentRowIndex = _firstRowIndex;
                        _currentPositionRow = 0;
                    }
                    _currentRowIndex++;
                    _currentPositionRow++;
                }
            }
        }

        public BuildTableLayout SetHeader(Action<TableRow> renderHeader)
        {
            var header = new TableRow(_layoutMain.Context);
            renderHeader(header);
            _layoutCurrent.AddView(header);
            return this;
        }

        public BuildTableLayout SetRow<TItem>(IEnumerable<TItem> itens, Action<int, TItem, TableRow> renderItem, Action<TextView> renderTextEmpty)
            where TItem : class
        {
            int position = 0;
            if (itens != null && itens.Count() > 0)
            {
                foreach (var item in itens)
                {
                    var row = new TableRow(_layoutMain.Context);
                    renderItem(position++, item, row);
                    _layoutCurrent.AddView(row);
                }
            }
            else
            {
                var view = new TextView(_layoutMain.Context);
                renderTextEmpty(view);
                _layoutCurrent.AddView(view);
            }
            return this;
        }

        public BuildTableLayout SetRow(Action<int, TableRow> renderItem)
        {
            var item = new TableRow(_layoutMain.Context);
            item.Tag = "row";
            if(_firstRowIndex == 0) // Index inicial
            {
                _firstRowIndex = _layoutCurrent.ChildCount;
                _currentRowIndex = _firstRowIndex;
                _currentPositionRow = 0;
            }
            renderItem(_currentPositionRow++, item);
            _layoutCurrent.AddView(item, _currentRowIndex == _firstRowIndex ? _currentRowIndex++ : _currentRowIndex++ - 1);
            return this;
        }

        public BuildTableLayout SetFooter(Action<TableRow> renderFotter)
        {
            var fotter = new TableRow(_layoutMain.Context);
            renderFotter(fotter);
            _layoutCurrent.AddView(fotter);
            return this;
        }

        public BuildTableLayout SetLineSeparator(Action<View> controlLine = null)
        {
            var line = new View(_layoutMain.Context);
            var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
            lp.Height = 2;
            line.SetBackgroundDrawable(_layoutMain.Context.Resources.GetDrawable(Resource.Color.background_line_default));
            _layoutCurrent.AddView(line, lp);
            if(controlLine != null)
            {
                controlLine(line);
            }
            return this;
        }

        public LinearLayout Build(Action<TableLayout> layoutControl = null)
        {
            if (layoutControl != null)
            {
                layoutControl(_layoutMain);
            }
            return _layoutMain;
        }
    }
}