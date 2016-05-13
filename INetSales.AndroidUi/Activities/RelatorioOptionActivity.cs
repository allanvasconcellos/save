using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;

namespace INetSales.AndroidUi.Activities
{
    [Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme")]
    public class RelatorioOptionFragment : BaseActivity
    {
        public enum TipoRelatorio : byte
        {
            Indefinido = 0,
            Sintetico,
            Analitico,
        }

        public enum VisaoRelatorio : byte
        {
            Indefinido = 0,
            VisaoQuantidade,
            VisaoQuantidadeValor,
            VisaoFinanceiro,
        }

        protected override void OnBeginView(Bundle bundle)
        {
            var view = LayoutInflater.Inflate(Resource.Layout.Relatorios, null);

            var btnRelQuantidades = view.FindViewById<Button>(Resource.Id.btnRelQuantidades);
            var btnRelQtdValor = view.FindViewById<Button>(Resource.Id.btnRelQtdValor);
            var btnRelVendas = view.FindViewById<Button>(Resource.Id.btnRelVendas);

            btnRelQuantidades.Click += (s, e) => ShowAskTipoRelatorio(ShowRelQuantidades);
            btnRelQtdValor.Click += (s, e) => ShowAskTipoRelatorio(ShowRelQuantidadesValor);
            btnRelVendas.Click += (s, e) => ShowAskTipoRelatorio(ShowRelVendas);

        }

        private void ShowRelQuantidades(TipoRelatorio tipo)
        {
            //Toast.MakeText(Activity, "Relatório de quantidades - Tipo: " + tipo, ToastLength.Long).Show();
            var parameters = new Dictionary<string, object> { { ActivityFlags.TipoRelatorioParam, tipo } };
            this.LaunchActivity(ActivityFlags.RelQtdCategory, parameters);
        }

        private void ShowRelQuantidadesValor(TipoRelatorio tipo)
        {
            //Toast.MakeText(Activity, "Relatório de quantidades e valor - Tipo: " + tipo, ToastLength.Long).Show();
            var parameters = new Dictionary<string, object> { { ActivityFlags.TipoRelatorioParam, tipo } };
            this.LaunchActivity(ActivityFlags.RelQtdValorCategory, parameters);
        }

        private void ShowRelVendas(TipoRelatorio tipo)
        {
            Toast.MakeText(this, "Relatório de vendas - Tipo: " + tipo, ToastLength.Long).Show();
        }

        private void ShowAskTipoRelatorio(Action<TipoRelatorio> callbackReturn)
        {
            var layout = BuildLayout.Create(this, Orientation.Vertical)
                .SetButton("Sintético", 0, 0, 0, 0, b =>
                {
                    b.Gravity = GravityFlags.CenterVertical;
                    b.Click += (s, e) => callbackReturn(TipoRelatorio.Sintetico);
                })
                .SetButton("Análitico", 0, 0, 0, 0, b =>
                {
                    b.Gravity = GravityFlags.CenterVertical;
                    b.Click += (s, e) => callbackReturn(TipoRelatorio.Analitico);
                })
                .Build();

            this.ShowDialog("Tipo de Relatório", layout);
        }
    }
}