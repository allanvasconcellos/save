using System;
using INetSales.ViewController.Views;
using INetSales.ViewController.Lib;
using INetSales.Objects.DbInterfaces;
using System.Globalization;
using INetSales.Objects;

namespace INetSales.ViewController.Controllers
{
	public partial class RelatorioController
    {
        internal void InitializePedidoOrcamento()
        {
            var pedidoOrcamentoView = (IRelatorioPedidoOrcamentoView) View;
            pedidoOrcamentoView.Initialize(this);
        }

        public void GerarRelatorioPedidoOrcamento(string inicio, string fim, bool isPedido)
        {
            DateTime? inicioDate = null;
            DateTime? fimDate = null;
            DateTime temp;
			var pedidoOrcamentoView = (IRelatorioPedidoOrcamentoView) View;
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");
            var relatorioDb = DbHelper.GetOnline<IRelatorioDb>();
            if (DateTime.TryParse(inicio, culture, DateTimeStyles.None, out temp))
            {
                inicioDate = temp;
            }
            if (DateTime.TryParse(fim, culture, DateTimeStyles.None, out temp))
            {
                fimDate = temp;
            }
            string link = String.Empty;
			try {
	            if (!relatorioDb.GetLinkPedidoOrcamento(inicioDate, fimDate, Session.UsuarioLogado, isPedido, out link))
	            {
	                string completeMessage = String.Format("Erro ao gerar relatório\nMensagem: {0}\nUrl:{1}", "", link);
	                View.ShowModalMessage("Erro", completeMessage);
	                return;
	            }
				pedidoOrcamentoView.MostrarRelatorioPedidoOrcamento (link);
			}
			catch(OnlineException ex) {
				View.ShowModalMessage ("Error", ex.Message);
			}
        }
    }
}

