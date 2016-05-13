using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace INetSales.AndroidUi.Controls
{
    public class BuildLayout
    {
        private readonly LinearLayout _layoutMain;
        private LinearLayout _layoutCurrent;

        #region Static Members

        public static BuildLayout Use(LinearLayout layout, Context context)
        {
            return new BuildLayout(layout);
        }

        public static BuildLayout Create(Context context, Orientation orientation)
        {
            var layout = new LinearLayout(context);
            layout.Orientation = orientation;
            return new BuildLayout(layout);
        }

        #endregion

        private BuildLayout(LinearLayout layoutMain)
        {
            _layoutMain = layoutMain;
            _layoutCurrent = _layoutMain;
        }

        public BuildLayout ForEach<T>(IEnumerable<T> lista, Action<T, BuildLayout, int> it)
        {
            int i = 0;
            foreach (var value in lista)
            {
                it(value, this, i++);
            }
            return this;
        }

        public BuildLayout SetImage(int imageId, int left = 0, int top = 0, int right = 0, int bottom = 0, Action<ImageView> controlImage = null)
        {
            var context = _layoutMain.Context;
            var img = context.Resources.GetDrawable(imageId);
            var view = new ImageView(context);
            view.SetImageDrawable(img);
            if (left > 0 || top > 0 || right > 0 || bottom > 0)
            {
                var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                       ViewGroup.LayoutParams.WrapContent);
                lp.SetMargins(left, top, right, bottom);
                view.LayoutParameters = lp;
            }
            _layoutCurrent.AddView(view);
            if (controlImage != null)
            {
                controlImage(view);
            }
            return this;
        }

        public BuildLayout SetRadio(IEnumerable<ControlItem> radioItens, int left = 0, int top = 0, int right = 0, int bottom = 0, Action<RadioGroup> controlRadio = null)
        {
            var group = new RadioGroup(_layoutMain.Context);
            foreach (var controlItem in radioItens)
            {
                var radio = new RadioButton(_layoutMain.Context);
                radio.Text = controlItem.Descricao;
                radio.Id = controlItem.Id;
                radio.Checked = controlItem.IsDefault;
                group.AddView(radio);
            }
            if (left > 0 || top > 0 || right > 0 || bottom > 0)
            {
                var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent,
                                                       ViewGroup.LayoutParams.FillParent);
                lp.SetMargins(left, top, right, bottom);
                group.LayoutParameters = lp;
            }
            _layoutCurrent.AddView(group);
            if (controlRadio != null)
            {
                controlRadio(group);
            }
            return this;
        }

        public BuildLayout SetText(string text, int left = 0, int top = 0, int right = 0, int bottom = 0, Action<TextView> controlText = null)
        {
            var control = new TextView(_layoutMain.Context);
            control.Text = text;

            if(left > 0 || top > 0 || right > 0 || bottom > 0)
            {
                var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                       ViewGroup.LayoutParams.WrapContent);
                lp.SetMargins(left, top, right, bottom);
                control.LayoutParameters = lp;
            }
            _layoutCurrent.AddView(control);

            if(controlText != null)
            {
                controlText(control);
            }

            return this;
        }

        public BuildLayout SetEdit(string text, int left = 0, int top = 0, int right = 0, int bottom = 0, Action<EditText> controlText = null)
        {
            var control = new EditText(_layoutMain.Context);
            control.Text = text;

            if (left > 0 || top > 0 || right > 0 || bottom > 0)
            {
                var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                       ViewGroup.LayoutParams.WrapContent);
                lp.SetMargins(left, top, right, bottom);
                control.LayoutParameters = lp;
            }
            _layoutCurrent.AddView(control);

            if (controlText != null)
            {
                controlText(control);
            }

            return this;
        }

        public BuildLayout SetProgress(int left = 0, int top = 0, int right = 0, int bottom = 0, Action<ProgressBar> controlProgress = null)
        {
            var control = new ProgressBar(_layoutMain.Context);

            if (left > 0 || top > 0 || right > 0 || bottom > 0)
            {
                var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                       ViewGroup.LayoutParams.WrapContent);
                lp.SetMargins(left, top, right, bottom);
                control.LayoutParameters = lp;
            }
            _layoutCurrent.AddView(control);

            if (controlProgress != null)
            {
                controlProgress(control);
            }

            return this;
        }

        public BuildLayout SetButton(string text, int left = 0, int top = 0, int right = 0, int bottom = 0, Action<TextView> controlButton = null)
        {
            var control = new Button(_layoutMain.Context);
            control.Text = text;

            if (left > 0 || top > 0 || right > 0 || bottom > 0)
            {
                var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent,
                                                       ViewGroup.LayoutParams.FillParent);
                lp.SetMargins(left, top, right, bottom);
                control.LayoutParameters = lp;
            }
            _layoutCurrent.AddView(control);

            if (controlButton != null)
            {
                controlButton(control);
            }

            return this;
        }

        public BuildLayout SetList(int width = 0, int height = 0, Action<ListView> controlList = null)
        {
            var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent,
                                                   ViewGroup.LayoutParams.FillParent);
            lp.Width = width > 0 ? width : lp.Width;
            lp.Height = height > 0 ? height : lp.Height;
            var listControl = new ListView(_layoutMain.Context);
            listControl.LayoutParameters = lp;

            _layoutCurrent.AddView(listControl);
            if (controlList != null)
            {
                controlList(listControl);
            }
            return this;
        }

        public BuildLayout SetList<TItem>(IEnumerable<TItem> itens, int width = 0, int height = 0, Func<int, TItem, View> renderItem = null, Action<ListView> controlList = null)
            where TItem : class
        {
            var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent,
                                                   ViewGroup.LayoutParams.WrapContent);
            lp.Width = width > 0 ? width : lp.Width;
            lp.Height = height > 0 ? height : lp.Height;
            var listControl = new ListView(_layoutMain.Context);
            var adapter = new ListAdapter<TItem>(itens) {BindingGetView = renderItem};
            listControl.Adapter = adapter;
            listControl.LayoutParameters = lp;

            _layoutCurrent.AddView(listControl);

            if (controlList != null)
            {
                controlList(listControl);
            }

            return this;
        }

        public BuildLayout SetTable<TItem>(IEnumerable<TItem> itens, Action<TableRow> renderHeader, Action<int, TItem, TableRow> renderItem, Action<TableRow> renderFooter, Action<TableLayout> controlTable = null)
            where TItem : class
        {
            var layout = BuildTableLayout.Create(_layoutMain.Context, Orientation.Vertical)
                .SetHeader(renderHeader)
                .SetLineSeparator()
                .SetRow(itens, renderItem, 
                    textEmpty =>
                        {
                            textEmpty.Text = "Nenhum item";
                        })
                .SetLineSeparator()
                .SetFooter(renderFooter)
                .Build();
            _layoutCurrent.AddView(layout);
            return this;
        }

        public BuildLayout SetLineSeparator(Action<View> controlLine = null)
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

        public BuildLayout SetLayout(Orientation orientation)
        {
            var layout = new LinearLayout(_layoutMain.Context);
            layout.Orientation = orientation;
            _layoutMain.AddView(layout);
            _layoutCurrent = layout;
            return this;
        }

        public BuildLayout SetView(View view, ViewGroup.LayoutParams lp = null)
        {
            if(view.Parent != null)
            {
                if (view.Parent is ViewGroup)
                {
                    var parent = view.Parent as ViewGroup;
                    parent.RemoveView(view);
                }
            }
            if (lp != null)
            {
                _layoutCurrent.AddView(view, lp);
            }
            else
            {
                _layoutCurrent.AddView(view);
            }
            return this;
        }

        public LinearLayout Build(Action<LinearLayout> layoutControl = null)
        {
            if (layoutControl != null)
            {
                layoutControl(_layoutMain);
            }
            return _layoutMain;
        }
    }
}