using System;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using INetSales.AndroidUi.Controls;
using INetSales.ViewController.Controllers;
using System.Collections.Generic;

namespace INetSales.AndroidUi.Activities.Main
{
    public partial class MenuRoteiroFragment : Fragment
    {
        private readonly int _position;
        private readonly RoteiroController _controller;

        public MenuRoteiroFragment(int position, RoteiroController controller)
        {
            _position = position;
            _controller = controller;
        }

        public override View OnCreateView(LayoutInflater p0, ViewGroup p1, Bundle p2)
        {
            switch (_position)
            {
                case 0:
                    return CreateView00(p0);
                case 1:
                    return CreateView01(p0);
                default:
                    throw new NotImplementedException();
            }
        }

        private void ConfigurarClick()
        {
            _controller.Configurar(new AcessoModalView(Activity));
        }

        private void VersaoClick(object sender, EventArgs e)
        {
            _controller.AtualizarVersao();
        }

        private void LogoffClick()
        {
            _controller.Logoff();
        }

        private void RelatorioClick(object sender, EventArgs e)
        {
			var modal = new MultipleActionModalView(Activity);
			var parmsPedidoOrcamento = new Dictionary<string, object> ();
			parmsPedidoOrcamento.Add (ActivityFlags.TipoRelatorioErpParam, TipoRelatorioErp.PedidoOrcamento);
			var parmsFaturamentoCategoria = new Dictionary<string, object> ();
			parmsFaturamentoCategoria.Add (ActivityFlags.TipoRelatorioErpParam, TipoRelatorioErp.FaturamentoCategoria);
			var parmsClienteDevedores = new Dictionary<string, object> ();
			parmsClienteDevedores.Add (ActivityFlags.TipoRelatorioErpParam, TipoRelatorioErp.ClienteDevedores);
			modal.Show ("Relat¨®rios",
				new MultipleActionModalView.ActionInfo (this.Activity.Resources.GetString (Resource.String.relatorio_pedido_orcamento), 
					() => Activity.LaunchActivity (ActivityFlags.RelatorioPedidoOrcamentoCategory, parmsPedidoOrcamento)),
				new MultipleActionModalView.ActionInfo (this.Activity.Resources.GetString (Resource.String.relatorio_faturamento_categorio_produto), 
					() => Activity.LaunchActivity (ActivityFlags.RelatorioPedidoOrcamentoCategory, parmsFaturamentoCategoria)),
				new MultipleActionModalView.ActionInfo (this.Activity.Resources.GetString (Resource.String.relatorio_cliente_devedores), 
					() => Activity.LaunchActivity (ActivityFlags.RelatorioPedidoOrcamentoCategory, parmsClienteDevedores)),
				new MultipleActionModalView.ActionInfo(this.Activity.Resources.GetString(Resource.String.relatorio_saldo_estoque), 
					() => Activity.LaunchActivity(ActivityFlags.RelatorioForcaVendaCategory))			
			);
        }

        private void OpenClienteMenu(object sender)
        {
            var menuActivity = Activity as IMenuActivity;
            menuActivity.SetTipoMenu(TipoMenuEnum.Cliente);
            Activity.OpenContextMenu((View)sender);
        }

        private void OpenSincronizarMenu()
        {
            var modal = new MultipleActionModalView(Activity);
            modal.Show("Sincronizar",
                new MultipleActionModalView.ActionInfo("Sincronizar todos", () => _controller.SincronizarTodos()),
                new MultipleActionModalView.ActionInfo("Usuário", () => _controller.SincronizarUsuario()),
                new MultipleActionModalView.ActionInfo("Produto", () => _controller.SincronizarProduto()),
                new MultipleActionModalView.ActionInfo("Cliente", () => _controller.SincronizarCliente()),
                new MultipleActionModalView.ActionInfo("Rota", () => _controller.SincronizarRota()),
                new MultipleActionModalView.ActionInfo("Configuração", () => _controller.SincronizarConfiguracao()),
                new MultipleActionModalView.ActionInfo("Reenviar pendentes", () => _controller.ReenviarPendentes()));
        }
    }
}