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
using INetSales.Objects;
using INetSales.Objects.Dtos;
using INetSales.ViewController;
using INetSales.ViewController.Controllers;
using INetSales.ViewController.Views;

namespace INetSales.AndroidUi.Activities.Pedido
{
    [Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { ActivityFlags.PedidoFinalizadoCategory })]
	public class PedidoFinalizadoActivity : BaseActivity, IPedidoFinalizadoView
    {
        private PedidoDto _pedido;

        #region Overrides of BaseActivity

        protected override void OnBeginView(Bundle bundle)
        {
            ApplicationController.Initialize(this);
        }

        public override void Next()
        {
			this.LaunchActivity(ActivityFlags.MainCategory);
        }

        #endregion

        #region Implementation of IPedidoFinalizadoView

		public void Initialize(PedidoProcessadoController controller, ConfiguracaoDto configuracao)
        {
            SetContentView(Resource.Layout.PedidoFinalizado);
            var btnNotaFiscal = FindViewById<Button>(Resource.Id.btnNotaFiscal);
            var btnBoleto = FindViewById<Button>(Resource.Id.btnBoleto);
            var btnUploadPedido = FindViewById<Button>(Resource.Id.btnUploadPedido);
            var btnFecharPedidoFinalizado = FindViewById<Button>(Resource.Id.btnFecharPedidoFinalizado);
			var tvOC = FindViewById<TextView>(Resource.Id.tvOC);
			tvOC.Visibility = ViewStates.Invisible;
			btnNotaFiscal.Enabled = true;
			btnBoleto.Enabled = true;
			if (configuracao.IsPreVenda) {
				btnNotaFiscal.Visibility = ViewStates.Invisible;
				btnNotaFiscal.Enabled = false;
				btnBoleto.Enabled = false;
				btnBoleto.Visibility = ViewStates.Invisible;
			}
            
            btnUploadPedido.Enabled = true;

            btnUploadPedido.Click += (sender, e) => controller.EnviarPedidoPendente(_pedido);
            btnFecharPedidoFinalizado.Click += (sender, e) => controller.Close(() => { });

            btnNotaFiscal.Click += (sender, e) => controller.GerarNfePedido(_pedido);
            btnBoleto.Click += (sender, e) => controller.GerarBoletoPedido(_pedido);
        }

        public void ShowBoleto(string urlBoleto)
        {
            //ActivityHelper.AbrirPdf(this, urlBoleto);
			var parameters = new Dictionary<string, object> { { ActivityFlags.TextoParam, urlBoleto } };
			this.LaunchActivity(ActivityFlags.PrintDisplay, parameters);
        }

        public void ShowNotaFiscal(string urlNfe)
        {
            ///ActivityHelper.AbrirPdf(this, urlNfe);
			var parameters = new Dictionary<string, object> { { ActivityFlags.TextoParam, urlNfe } };
			this.LaunchActivity(ActivityFlags.PrintDisplay, parameters);
        }

        public void ShowPedidoInfo(PedidoDto pedido, decimal totalPedido, double totalValor)
        {
            var tvResultadoPedido = FindViewById<TextView>(Resource.Id.tvResultadoPedido);
            var tvNumeroPedido = FindViewById<TextView>(Resource.Id.tvNumeroPedido);
            var tvQuantidadePedidoFinalizado = FindViewById<TextView>(Resource.Id.tvQuantidadePedidoFinalizado);
            var tvTotalValorFinalizado = FindViewById<TextView>(Resource.Id.tvTotalValorFinalizado);

            tvResultadoPedido.Text = String.Format("Pedido de {0}", pedido.Tipo.ToDescription());

            _pedido = pedido;
            
            tvNumeroPedido.Text = String.Format("N鷐ero: {0}", pedido.Codigo);


            tvQuantidadePedidoFinalizado.Text = String.Format("Quantidade Pedido: {0}", totalPedido);
            tvTotalValorFinalizado.Text = String.Empty;
            tvTotalValorFinalizado.Visibility = ViewStates.Gone;
            if (totalValor > 0)
            {
                tvTotalValorFinalizado.Text = String.Format("Total Valor: {0:C}", totalValor);
                tvTotalValorFinalizado.Visibility = ViewStates.Visible;
            }
        }

        public void ShowInfoCancelado()
        {
            var tvResultadoPedido = FindViewById<TextView>(Resource.Id.tvResultadoPedido);
            tvResultadoPedido.Text = "Pedido cancelado";
        }

        public void ShowInfoRejeitado()
        {
            var tvResultadoPedido = FindViewById<TextView>(Resource.Id.tvResultadoPedido);
            tvResultadoPedido.Text = "Pedido rejeitado";
        }

		public void ShowOC (string numero)
		{
			var tvOC = FindViewById<TextView>(Resource.Id.tvOC);
			tvOC.Text = String.Format("OC: {0}", numero);
			tvOC.Visibility = ViewStates.Visible;
		}

        public void ShowProdutosPedido(IEnumerable<ProdutoDto> produtos)
        {
            var lvProdutosPedido = FindViewById<ListView>(Resource.Id.lvProdutosPedido);
            BuildList.Use(lvProdutosPedido)
                .Render(produtos, (position, produto) =>
                {
                    var layout = BuildLayout.Create(this, Orientation.Vertical)
                                              .SetText(String.Format("Produto: {0}", produto.Nome), 10, 5, 0, 0, t => t.SetTypeface(null, TypefaceStyle.Bold))
                                              .SetText(String.Format("Quantidade Pedido: {0}", produto.QuantidadePedido), 10)
                                              .SetText(String.Format("Quantidade Disponivel: {0}", produto.SaldoAtual), 10)
                                              .SetText(String.Format("Valor Pedido: {0:C}", produto.ValorPedidoSemDesconto), 10)
                                              .SetText(String.Format("Valor Desconto: {0:C}", produto.ValorTotalDesconto), 10)
                                              .SetText(String.Format("Valor Total: {0:C}", produto.ValorTotalPedido), 10)
                                              .Build();
                    return layout;
                })
                .Build();
        }

        public void DesabilitarEnvio()
        {
            var btnUploadPedido = FindViewById<Button>(Resource.Id.btnUploadPedido);
            btnUploadPedido.Enabled = false;
        }

        public void DesabilitarGerarNfe()
        {
            var btnNotaFiscal = FindViewById<Button>(Resource.Id.btnNotaFiscal);
            btnNotaFiscal.Enabled = false;
        }

        public void DesabilitarGerarBoleto()
        {
            var btnBoleto = FindViewById<Button>(Resource.Id.btnBoleto);
            btnBoleto.Enabled = false;
        }

        public void HabilitarGerarNfe()
        {
            var btnNotaFiscal = FindViewById<Button>(Resource.Id.btnNotaFiscal);
            btnNotaFiscal.Enabled = true;
        }

        public void HabilitarGerarBoleto()
        {
            var btnBoleto = FindViewById<Button>(Resource.Id.btnBoleto);
            btnBoleto.Enabled = true;
        }

        #endregion

    }
}