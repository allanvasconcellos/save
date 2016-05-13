using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.ViewController;
using INetSales.ViewController.Controllers;
using INetSales.ViewController.Models;
using INetSales.ViewController.Views;

namespace INetSales.AndroidUi.Activities
{
    [Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { ActivityFlags.RelatorioForcaVendaCategory })]
    public class RelatorioForcaVendaActivity : BaseActivity, IRelatorioForcaVendaView
    {
        protected override void OnBeginView(Bundle bundle)
        {
            ApplicationController.Initialize(this);
        }

		private LinearLayout GetDadosVendaLayout(decimal quantidadeRecebida, decimal quantidadeVendida, decimal quantidadeDisponivel,
                                                 decimal valorTotalDinheiro, decimal valorTotalBoleto, decimal valorTotalCheque)
        {
            var layout = LayoutInflater.Inflate(Resource.Layout.ItemDadosVenda, null);
            var tvQuantidadeVendida = layout.FindViewById<TextView>(Resource.Id.tvQuantidadeVendida);
            var tvQuantidadeRecebida = layout.FindViewById<TextView>(Resource.Id.tvQuantidadeRecebida);
            var tvQuantidadeDisponivel = layout.FindViewById<TextView>(Resource.Id.tvQuantidadeDisponivel);
            var tvValorTotalDinheiro = layout.FindViewById<TextView>(Resource.Id.tvValorTotalDinheiro);
            var tvValorTotalCheque = layout.FindViewById<TextView>(Resource.Id.tvValorTotalCheque);
            var tvValorTotalBoleto = layout.FindViewById<TextView>(Resource.Id.tvValorTotalBoleto);
            var layoutItemDadosVenda = layout.FindViewById<LinearLayout>(Resource.Id.layoutItemDadosVenda);

            tvQuantidadeVendida.Text = quantidadeVendida.ToString();
            tvQuantidadeRecebida.Text = quantidadeRecebida.ToString();
            tvQuantidadeDisponivel.Text = quantidadeDisponivel.ToString();
            tvValorTotalDinheiro.Text = valorTotalDinheiro.ToString("C");
            tvValorTotalCheque.Text = valorTotalBoleto.ToString("C");
            tvValorTotalBoleto.Text = valorTotalCheque.ToString("C");

            return layoutItemDadosVenda;
        }

        #region Implementation of IRelatorioForcaVendaView

        public void InitializeSintetico(RelatorioController controller)
        {
            SetContentView(Resource.Layout.RelatorioSinteticoForcaVenda);
        }

		public void ShowTotais(decimal quantidadeRecebida, decimal quantidadeVendida, decimal quantidadeDisponivel,
                               decimal valorTotalDinheiro, decimal valorTotalBoleto, decimal valorTotalCheque)
        {
            var view = GetDadosVendaLayout(quantidadeRecebida, quantidadeVendida, quantidadeDisponivel,
                                           valorTotalDinheiro, valorTotalBoleto, valorTotalCheque);
            var recipiente = FindViewById<LinearLayout>(Resource.Id.layoutDadosVendaGrupos);
            recipiente.AddView(view);
        }

        public void ShowGrupos(IEnumerable<GrupoInfoModel> grupos)
        {
            var list = FindViewById<ListView>(Resource.Id.lvGruposQuantidadeValor);
            list.Clickable = false;

            BuildList.Use(list)
                .Render(grupos, (p, g) =>
                                    {
                                        var dadosView = GetDadosVendaLayout(g.QuantidadeRecebida, g.QuantidadeVendida,
                                                                            g.QuantidadeDisponivel, g.ValorPagoDinheiro,
                                                                            g.ValorPagoBoleto, g.ValorPagoCheque);
                                        var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
                                                                               ViewGroup.LayoutParams.WrapContent);
                                        lp.SetMargins(270, 0, 0, 0);
                                        lp.Gravity = GravityFlags.Right;
                                        var layout = BuildLayout.Create(this, Orientation.Horizontal)
                                            .SetText(g.Nome, 10, 20, 0, 0, c =>
                                                                               {
                                                                                   c.SetWidth(100);
                                                                                   c.SetTypeface(null, TypefaceStyle.Bold);
                                                                               })
                                            .SetView(dadosView, lp)
                                            .Build();
                                        return layout;
                                    })
                .Build();
        }

        public void ShowProdutos(IEnumerable<ProdutoInfoModel> produtos)
        {
			var list = FindViewById<ListView>(Resource.Id.lvGruposQuantidadeValor);
			list.Clickable = false;

			BuildList.Use(list)
				.Render(produtos, (p, g) =>
					{
						var dadosView = GetDadosVendaLayout(g.QuantidadeRecebida, g.QuantidadeVendida,
							g.QuantidadeDisponivel, g.ValorPagoDinheiro,
							g.ValorPagoBoleto, g.ValorPagoCheque);
						var lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent,
							ViewGroup.LayoutParams.WrapContent);
						lp.SetMargins(270, 0, 0, 0);
						lp.Gravity = GravityFlags.Right;
						var layout = BuildLayout.Create(this, Orientation.Horizontal)
							.SetText(g.Nome, 10, 20, 0, 0, c =>
								{
									c.SetWidth(100);
									c.SetTypeface(null, TypefaceStyle.Bold);
								})
							.SetView(dadosView, lp)
							.Build();
						return layout;
					})
				.Build();
        }

        #endregion
    }
}