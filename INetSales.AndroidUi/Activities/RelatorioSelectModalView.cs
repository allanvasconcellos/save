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
using INetSales.AndroidUi.Controls;

namespace INetSales.AndroidUi.Activities
{
    public class RelatorioSelectedEventArgs : EventArgs
    {
        public RelatorioSelectedEventArgs(TipoRelatorioEnum tipo, RelatorioEnum relatorio)
        {
            Tipo = tipo;
            Relatorio = relatorio;
        }

        public TipoRelatorioEnum Tipo { get; private set; }
        public RelatorioEnum Relatorio { get; private set; }
    }

    public class RelatorioSelectModalView
    {
        private readonly Activity _activity;
        private readonly IEnumerable<ControlItem> _relatoriosSelect;

        public RelatorioSelectModalView(Activity activity)
        {
            _activity = activity;
            //_relatoriosSelect = relatoriosSelect;
        }

        public void Show()
        {
            var radioItens = new List<ControlItem>()
            {
                new ControlItem() { Id = 1, Descricao = "Análitico", },
                new ControlItem() { Id = 2, Descricao = "Sintético", },
            };
            _activity.ShowDialog("Relatórios", dialog =>
            {
                var layout = BuildLayout.Create(_activity, Orientation.Vertical)
                    .SetRadio(radioItens, 0, 10, 0, 10, r => r.SetGravity(GravityFlags.Center))
                    .SetList(0, 0, l =>
                    {
                        var adapter = new ListAdapter<ControlItem>(_relatoriosSelect);
                        adapter.BindingGetView = (p, item) =>
                        {
                            var relatorioItemLayout =
                                BuildLayout.Create(_activity, Orientation.Horizontal)
                                .SetText(item.Descricao, 10, 10, 0, 10)
                                    .Build();
                            return relatorioItemLayout;
                        };
                        l.Adapter = adapter;
                        l.ItemSelected += (sender, e) =>
                        {
                            var relatorioItem = _relatoriosSelect.ElementAt(e.Position);
                            dialog.Cancel();
                        };
                    })
                    .SetButton("Fechar", 0, 20, 0, 0, b =>
                    {
                        b.Gravity = GravityFlags.CenterHorizontal;
                        b.Click += (sender, e) => dialog.Cancel();
                    })
                    .Build();
                return layout;
            });
        }

        public event EventHandler<RelatorioSelectedEventArgs> OnSelect;

        public void InvokeOnSelect(RelatorioSelectedEventArgs e)
        {
            EventHandler<RelatorioSelectedEventArgs> handler = OnSelect;
            if (handler != null) handler(this, e);
        }
    }
}