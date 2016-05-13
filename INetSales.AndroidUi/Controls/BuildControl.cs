using System;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace INetSales.AndroidUi.Controls
{
    public class BuildControl<TView> where TView : TextView
    {
        private TView _control;
        private static BuildControl<TView> _instance;
        
        public static BuildControl<TView> Create(Context context)
        {
            var constructor = typeof (TView).GetConstructor(new[] {typeof(Context)});
            var newView = (TView)constructor.Invoke(new object[] {context});
            _instance = new BuildControl<TView> {_control = newView};
            return _instance;
        }

        public BuildControl<TView> SetText(string text)
        {
            _instance._control.Text = text;
            return _instance;
        }

        public BuildControl<TView> SetMargins(int left, int top, int right, int bottom)
        {
            var lp = _instance._control.LayoutParameters as LinearLayout.LayoutParams;
            if(lp == null)
            {
                lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);   
            }
            lp.SetMargins(left, top, right, bottom);
            _instance._control.LayoutParameters = lp;
            return _instance;
        }

        public BuildControl<TView> SetText(string format, params object[] args)
        {
            return SetText(String.Format(format, args));
        }

        public TView Build()
        {
            return _instance._control;
        }
    }

    public class ItemMenu
    {
        public int Id { get; private set; }
        public string Display { get; private set; }

        public static ItemMenu New = new ItemMenu() { Id = 1, Display = "Novo"};
        public static ItemMenu Edit = new ItemMenu() { Id = 2, Display = "Editar" };
        public static ItemMenu Remove = new ItemMenu() { Id = 3, Display = "Remover" };
    }

    public class BuildContextMenu
    {
        private IContextMenu _menu;
        private static BuildContextMenu _instance;

        public static BuildContextMenu Create(IContextMenu menu)
        {
            _instance = new BuildContextMenu() {_menu = menu};
            return _instance;
        }

        public IContextMenu Build(params ItemMenu[] itens)
        {
            int index = 0;
            foreach (var itemMenu in itens)
            {
                _instance._menu.Add(Menu.None, itemMenu.Id, index++, itemMenu.Display);
            }
            return _instance._menu;
        }
    }
}