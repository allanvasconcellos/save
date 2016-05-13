using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using INetSales.AndroidUi.Controls;
using INetSales.Objects.Dtos;
using INetSales.ViewController;
using INetSales.ViewController.Controllers;
using INetSales.ViewController.Views;

namespace INetSales.AndroidUi.Activities.Pedido
{
    [Activity(Label = "SAVE - SISTEMA DE AUTOMAÇÃO DE VENDAS", Theme = "@style/INetTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new[] { ActivityFlags.ListaPedidoCategory })]
	public class ListaPedidoActivity : BaseActivity, IPedidoListaView
    {
        private PedidoProcessadoController _controller;
		private ConfiguracaoDto _configuracao;
        private IEnumerable<PedidoDto> _pedidos;

        protected override void OnBeginView(Bundle bundle)
        {
            ApplicationController.Initialize(this);
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);

			menu.Add(Menu.None, 0, 0, "Boleto");
			if (!_configuracao.IsPreVenda) {
				menu.Add(Menu.None, 1, 1, "Nota Fiscal");
			}
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var info = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            PedidoDto selecionado = _pedidos.ElementAt(info.Position);
            if (item.ItemId == 0)
            {
                _controller.GerarBoletoPedido(selecionado);
                return true;
            }
            _controller.GerarNfePedido(selecionado);
            return true;
        }

        #region Implementation of IListaPedidoView

		public void Initialize(PedidoProcessadoController controller, ConfiguracaoDto configuracao)
        {
			_configuracao = configuracao;
            _controller = controller;
            SetContentView(Resource.Layout.PedidoList);
        }

        public void ShowPedidos(IEnumerable<PedidoDto> pedidos)
        {
            var listView = FindViewById<ListView>(Resource.Id.lvPedidos);
            _pedidos = pedidos;
            BuildList.Use(listView)
                .Render(_pedidos, 
                    (p, pedido) =>
                    {
                        Color textColor = pedido.IsPendingUpload ? Color.White : Color.Black;
                        Color backColor = pedido.IsPendingUpload ? Color.Gray : Color.Transparent;
                        var layout = BuildLayout.Create(this, Orientation.Vertical)
                            .SetText(String.Format("Pedido: {0} - {1}", pedido.Codigo, GetDescricaoTipoPedido(pedido.Tipo)), 15, 5, 0, 0,
                                    t =>
                                    {
                                        t.SetTypeface(null, TypefaceStyle.Bold);
                                        t.SetTextSize(ComplexUnitType.Px, 14);
                                        t.SetTextColor(textColor);
                                    })
							.SetText(String.Format("OC: {0}", pedido.OrdemCompra), 15, 5, 0, 0,
								t =>
								{
									t.SetTextSize(ComplexUnitType.Px, 14);
									t.SetTextColor(textColor);
								})
                            .SetText(String.Format("Data: {0:dd/MM/yyyy HH:mm}", pedido.DataCriacao ), 15, 0, 0, 0, 
                                    t =>
                                    {
                                        t.SetTextSize(ComplexUnitType.Px, 13);
                                        t.SetTextColor(textColor);
                                    })
                            .SetText(String.Format("Cliente: {0}", pedido.Cliente != null ? pedido.Cliente.RazaoSocial : "---"), 15, 0, 0, 5,
                                    t =>
                                    {
                                        t.SetTextSize(ComplexUnitType.Px, 13);
                                        t.SetTextColor(textColor);
                                    })
                            .SetText(String.Format("Enviado? {0}", pedido.IsPendingUpload ? "Não" : "Sim"), 15, 0, 0, 5,
                                    t =>
                                    {
                                        t.SetTextSize(ComplexUnitType.Px, 13);
                                        t.SetTextColor(textColor);
                                    })
                            .SetText(String.Format("Processado? {0}", !String.IsNullOrEmpty(pedido.UrlLocalNFe) || !String.IsNullOrEmpty(pedido.UrlLocalBoleto)
                                                        ? "Sim"
                                                        : "Não"), 15, 0, 0, 5,
                                    t =>
                                    {
                                        t.SetTextSize(ComplexUnitType.Px, 13);
                                        t.SetTextColor(textColor);
                                    })
                            .Build();
                        layout.SetBackgroundColor(backColor);
                        return layout;
                    })
                .Build();
            listView.ItemClick += (sender, e) => ListPedidoClick((ListView)sender, pedidos.ElementAt(e.Position), e);
            listView.LongClick += (sender, e) => { };
            RegisterForContextMenu(listView);
        }

        private string GetDescricaoTipoPedido(TipoPedidoEnum tipo)
        {
            switch (tipo)
            {
                case TipoPedidoEnum.Venda:
                    return "Venda";
                case TipoPedidoEnum.Bonificacao:
                    return "Bonificação";
                case TipoPedidoEnum.Remessa:
                    return "Remessa";
                case TipoPedidoEnum.Sos:
                    return "SOS";
                default:
                    throw new NotImplementedException();
            }
        }

        public void ShowBoleto(string urlBoleto)
        {
            //ActivityHelper.AbrirPdf(this, urlBoleto);
			var parameters = new Dictionary<string, object> { { ActivityFlags.TextoParam, urlBoleto } };
			this.LaunchActivity(ActivityFlags.PrintDisplay, parameters);
        }

        public void ShowNotaFiscal(string urlNfe)
        {
            //ActivityHelper.AbrirPdf(this, urlNfe);
			var parameters = new Dictionary<string, object> { { ActivityFlags.TextoParam, urlNfe } };
			this.LaunchActivity(ActivityFlags.PrintDisplay, parameters);
        }

		public void ShowOC (string numero)
		{
		}

        #endregion

        private void ListPedidoClick(ListView listView, PedidoDto pedido, AdapterView.ItemClickEventArgs args)
        {
            if(pedido.IsPendingUpload)
            {
                MakeQuestion("Deseja reenviar o pedido?", 
                    () => // ok
                    {
                        _controller.EnviarPedidoPendente(pedido);
                    },
                    () => {});
                return;
            }
            if (!pedido.HasCondicaoBoleto) // Chama só a nota fiscal.
            {
                MakeQuestion("Deseja gerar a NFe?",
                    () => // ok
                    {
                        _controller.GerarNfePedido(pedido);
                    },
                    () => { });
                return;
            }
            listView.ShowContextMenuForChild(args.View);
        }
    }
}