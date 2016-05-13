using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using INetSales.ViewController;
using INetSales.ViewController.Controllers;
using INetSales.ViewController.Models;

namespace INetSales.AndroidUi.Activities
{
    [Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { ActivityFlags.RelQtdCategory })]
    public class RelatorioQuantidadeActivity : BaseActivity
    {
        protected override void OnBeginView(Bundle bundle)
        {
            // Construir layout
            var tipoRelatorio = Intent.GetExtra<RelatorioOptionFragment.TipoRelatorio>(ActivityFlags.TipoRelatorioParam);
            switch (tipoRelatorio)
            {
                case RelatorioOptionFragment.TipoRelatorio.Analitico:
                    GerarRelatorioAnalitico();
                    break;
                case RelatorioOptionFragment.TipoRelatorio.Sintetico:
                    GerarRelatorioSintetico();
                    break;
                default:
                    throw new InvalidOperationException();
            }

            //this.InitializeActivity();
        }

        private void GerarRelatorioAnalitico()
        {
            //var view = LayoutInflater.Inflate(Resource.Layout.RelatorioQuantidadeTemplate, null);
            //var controller = new RelatorioController();
            //var table = view.FindViewById<TableLayout>(Resource.Id.tbTemplate);

            //try
            //{
            //    for (int i = 0; i < table.ChildCount; i++)
            //    {
            //        table.GetChildAt(i).Visibility = ViewStates.Invisible;
            //    }

            //    foreach (var grupoInfo in controller.GerarQuantidadeAnalitico())
            //    {
            //        table.AddView(GetHeader(grupoInfo.Nome), table.ChildCount);
            //        table.AddView(GetLineSeparator(), table.ChildCount);
            //        foreach (var produtoInfo in grupoInfo.Produtos)
            //        {
            //            table.AddView(GetProdutoDetail(produtoInfo), table.ChildCount);
            //        }
            //        table.AddView(GetLineSeparator(), table.ChildCount);
            //    }
            //    SetContentView(view);
            //}
            //catch (Exception ex)
            //{
            //    if(ExceptionPolicy.Handle(ex))
            //    {
            //        ShowModalMessage("Error", "Contacte o administrador do sistema", () => { });
            //    }
            //}
        }

        private void GerarRelatorioSintetico()
        {
            //var view = LayoutInflater.Inflate(Resource.Layout.RelatorioQuantidadeTemplate, null);
            //var controller = new RelatorioController();
            //var table = view.FindViewById<TableLayout>(Resource.Id.tbTemplate);

            //try
            //{
            //    for (int i = 0; i < table.ChildCount; i++)
            //    {
            //        table.GetChildAt(i).Visibility = ViewStates.Invisible;
            //    }

            //    table.AddView(GetHeader(String.Empty), table.ChildCount);
            //    table.AddView(GetLineSeparator(), table.ChildCount);
            //    foreach (var grupoInfo in controller.GerarRelatorioQuantidadeSintetico())
            //    {
            //        table.AddView(GetGrupoDetail(grupoInfo), table.ChildCount);
            //        table.AddView(GetLineSeparator(), table.ChildCount);
            //    }
            //    SetContentView(view);
            //}
            //catch (Exception ex)
            //{
            //    if (ExceptionPolicy.Handle(ex))
            //    {
            //        ShowModalMessage("Error", "Contacte o administrador do sistema", () => { });
            //    }
            //}
        }

        private View GetLineSeparator()
        {
            var view = LayoutInflater.Inflate(Resource.Layout.RelatorioQuantidadeTemplate, null);
            var line = view.FindViewById(Resource.Id.lineQuantidade);

            if (line.Parent != null && line.Parent is TableLayout)
            {
                (line.Parent as TableLayout).RemoveView(line);
            }

            return line;
        }

        private TableRow GetHeader(string title)
        {
            var view = LayoutInflater.Inflate(Resource.Layout.RelatorioQuantidadeTemplate, null);
            var headerRow = view.FindViewById<TableRow>(Resource.Id.trQuantidadeHeader);

            ((TextView) headerRow.GetChildAt(0)).Text = title;

            if (headerRow.Parent != null && headerRow.Parent is TableLayout)
            {
                (headerRow.Parent as TableLayout).RemoveView(headerRow);
            }

            return headerRow;
        }

        private TableRow GetGrupoDetail(GrupoInfoModel grupo)
        {
            var view = LayoutInflater.Inflate(Resource.Layout.RelatorioQuantidadeTemplate, null);
            var detailRow = view.FindViewById<TableRow>(Resource.Id.trQuantidadeDetail);

            ((TextView)detailRow.GetChildAt(0)).Text = grupo.Nome;
            ((TextView)detailRow.GetChildAt(1)).Text = grupo.QuantidadeRecebida.ToString();
            ((TextView)detailRow.GetChildAt(2)).Text = grupo.QuantidadeVendida.ToString();
            ((TextView)detailRow.GetChildAt(3)).Text = grupo.QuantidadeDisponivel.ToString();

            if (detailRow.Parent != null && detailRow.Parent is TableLayout)
            {
                (detailRow.Parent as TableLayout).RemoveView(detailRow);
            }

            return detailRow;
        }

        private TableRow GetProdutoDetail(ProdutoInfoModel produto)
        {
            var view = LayoutInflater.Inflate(Resource.Layout.RelatorioQuantidadeTemplate, null);
            var detailRow = view.FindViewById<TableRow>(Resource.Id.trQuantidadeDetail);

            ((TextView)detailRow.GetChildAt(0)).Text = produto.Nome;
            ((TextView)detailRow.GetChildAt(1)).Text = produto.QuantidadeRecebida.ToString();
            ((TextView)detailRow.GetChildAt(2)).Text = produto.QuantidadeVendida.ToString();
            ((TextView)detailRow.GetChildAt(3)).Text = produto.QuantidadeDevolver.ToString();

            if (detailRow.Parent != null && detailRow.Parent is TableLayout)
            {
                (detailRow.Parent as TableLayout).RemoveView(detailRow);
            }

            return detailRow;
        }
    }
}