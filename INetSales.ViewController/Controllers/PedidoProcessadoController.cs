using System;
using System.Linq;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;
using INetSales.ViewController.Sync;
using INetSales.ViewController.Sync.Integrators;
using INetSales.ViewController.Views;

namespace INetSales.ViewController.Controllers
{
    public class PedidoProcessadoController : BaseController<IPedidoProcessadoView>
    {
        private PedidoDto _pedidoFinalizado;

        public PedidoProcessadoController(IPedidoProcessadoView listView, IApplication application)
            : base(listView, application, null)
        {
        }

        internal void InitializeListaView()
        {
			View.Initialize(this, DbHelper.GetOfflineConfiguracaoAtiva());
            View.UpdateViewTitle(CurrentTitle);
            FiltrarDefault();
        }

        private void FiltrarDefault()
        {
            Filtrar(DateTime.Today.AddDays(-45), DateTime.Today.AddDays(1));
        }

        public void Filtrar(DateTime dataInicio, DateTime dataFinal)
        {
            var listaView = (IPedidoListaView) View;
            var offline = DbHelper.GetOffline<IOfflinePedidoDb>();
            var pedidos = offline.GetPedidosPorData(Session.UsuarioLogado, dataInicio, dataFinal);
            listaView.ShowPedidos(pedidos
                //.Where(p => !p.IsPendingUpload)
                .OrderByDescending(p => p.DataCriacao));
        }

        internal void InitializeFinalizadoView()
        {
            var finalizadoView = (IPedidoFinalizadoView)View;
            // Retornar o id do pedido finalizado.
            int pedidoIdFinalizado = Convert.ToInt32(Application.GetValue(PedidoController.paramPedidoFinalizado));
            var pedidoDb = DbHelper.GetOffline<IOfflinePedidoDb>();
            var produtoDb = DbHelper.GetOffline<IOfflineProdutoDb>();

            _pedidoFinalizado = pedidoDb.Find(pedidoIdFinalizado);

			View.Initialize(this, DbHelper.GetOfflineConfiguracaoAtiva());

            AtualizarTitle(_pedidoFinalizado);

            var produtos = produtoDb.GetProdutos(_pedidoFinalizado);

            // Processa valor de desconto junto com o valor total.
            foreach (var produtoPedido in produtos)
            {
				produtoPedido.ValorPedidoSemDesconto = produtoPedido.ValorUnitario*Convert.ToDouble(produtoPedido.QuantidadePedido);
                if (produtoPedido.Desconto > 0)
                {
                    produtoPedido.ValorTotalDesconto = (produtoPedido.Desconto/100)*
						(produtoPedido.ValorUnitario*Convert.ToDouble(produtoPedido.QuantidadePedido));
                }
                produtoPedido.ValorTotalPedido = produtoPedido.ValorPedidoSemDesconto - produtoPedido.ValorTotalDesconto;
            }

            finalizadoView.ShowPedidoInfo(_pedidoFinalizado, produtos.Sum(p => p.QuantidadePedido), produtos.Sum(p => p.ValorTotalPedido));
            finalizadoView.ShowProdutosPedido(produtos);

			// Pedido remessa
			if (_pedidoFinalizado.Tipo != TipoPedidoEnum.Remessa) {
				View.ShowOC (_pedidoFinalizado.OrdemCompra);
			}

            // Tentar enviar pedido pendente de envio
            if(_pedidoFinalizado.IsPendingUpload)
            {
                _pedidoFinalizado.Produtos = produtos;
                EnviarPedidoPendente(_pedidoFinalizado);
            }

            TratarPedido(finalizadoView);
        }

        private void TratarPedido(IPedidoFinalizadoView finalizadoView)
        {
            if (_pedidoFinalizado.IsCancelado || _pedidoFinalizado.IsRejeitado)
            {
                finalizadoView.DesabilitarEnvio();
                finalizadoView.DesabilitarGerarNfe();
                finalizadoView.DesabilitarGerarBoleto();

                if(_pedidoFinalizado.IsCancelado)
                {
                    finalizadoView.ShowInfoCancelado();
                }
                else
                {
                    finalizadoView.ShowInfoRejeitado();
                }
            }
            else if (!_pedidoFinalizado.IsPendingUpload)
            {
                finalizadoView.DesabilitarEnvio();
                finalizadoView.HabilitarGerarNfe();
                finalizadoView.HabilitarGerarBoleto();
                if (_pedidoFinalizado.Tipo == TipoPedidoEnum.Remessa || _pedidoFinalizado.Tipo == TipoPedidoEnum.Sos)
                {
                    finalizadoView.DesabilitarGerarNfe();
                }

                if (!_pedidoFinalizado.HasCondicaoBoleto)
                {
                    finalizadoView.DesabilitarGerarBoleto();
                }
            }
            else
            {
                finalizadoView.DesabilitarGerarNfe();
                finalizadoView.DesabilitarGerarBoleto();
            }

            Application.SetValue(PedidoController.paramPedidoFinalizado, null);
        }

        private void AtualizarTitle(PedidoDto pedidoFinalizado)
        {
            string titleCliente = String.Empty;
            switch (pedidoFinalizado.Tipo)
            {
                case TipoPedidoEnum.Venda:
                case TipoPedidoEnum.Bonificacao:
                    titleCliente = String.Format("{0}|Cliente: {1}|Roteiro: Ordem - {2} / Dia - {3:dd/MM/yyyy}",
                                                 CurrentTitle,
                                                 pedidoFinalizado.Cliente.RazaoSocial,
                                                 pedidoFinalizado.Cliente.OrdemRoteiro,
                                                 //pedidoFinalizado.Roteiro.Dia);
                                                 DateTime.Today);
                    break;
                case TipoPedidoEnum.Remessa:
                case TipoPedidoEnum.Sos:
                    titleCliente = CurrentTitle;
                    break;
            }
            View.UpdateViewTitle(titleCliente.Split('|'));
        }

        public void EnviarPedidoPendente(PedidoDto pedido)
        {
            var manager = new IntegratorManager();
            var progress = View.ShowProgressView("Enviando...");
            manager.Enqueue(new PedidoUpload(pedido));
            ExecuteOnBackgroundView(() =>
            {
                manager.Execute(new ProgressCompleteManager(progress), 
                    integrator =>
                    {
                        if(integrator.HasError)
                        {
                            string message = 
                                !String.IsNullOrEmpty(integrator.MessageOnline) ? integrator.MessageOnline : 
                                integrator.IsTimeout ? "A comunicação expirou, tente reenviar o pedido mais tarde" :
                                integrator.IsSemConexao ? "Sem conexão" :
                                "Erro ao enviar o pedido";
                            View.ExecuteOnUI(() => View.ShowModalMessage("Erro ao enviar pedido", message));
                        }
                    });
                View.ExecuteOnUI(
                    () =>
                    {
                        if (View is IPedidoFinalizadoView)
                        {
                            TratarPedido((IPedidoFinalizadoView) View);
                        }
                        if (View is IPedidoListaView)
                        {
                            FiltrarDefault();
                        }
                    });
                progress.Close();
            });
        }

        public void GerarBoletoPedido(PedidoDto pedido)
        {
            if (pedido.IsPendingUpload)
            {
                return;
            }

            try
            {
                if (String.IsNullOrEmpty(pedido.UrlLocalBoleto))
                {
                    BaixarBoleto(pedido);
                    return;
                }
                View.MakeQuestion("Deseja reprocessar o boleto?", () => BaixarBoleto(pedido), () => View.ShowBoleto(pedido.UrlLocalBoleto));
            }
            catch (Exception ex)
            {
                View.ShowModalMessage("Boleto", "Erro ao gerar o boleto");
                if (ExceptionPolicy.Handle(ex))
                {
                    throw;
                }
            }
        }

        private void BaixarBoleto(PedidoDto pedido)
        {
            var pedidoDbOnline = DbHelper.GetOnline<IPedidoDb>();
            var progress = View.ShowProgressView(String.Empty);
            ExecuteOnBackgroundView(() =>
            {
                if (!pedidoDbOnline.ProcessarBoleto(pedido) || String.IsNullOrEmpty(pedido.UrlHttpBoleto))
                {
                    progress.Close();
                    View.ExecuteOnUI(() => View.ShowModalMessage("Boleto", "Boleto indisponivel") );
                    return;
                }

                var offline = DbHelper.GetOffline<IOfflinePedidoDb>();
                var stream = Application.Download(pedido.UrlHttpBoleto);
                string fullName;
                Application.SaveStreamOnApplicationDisk(String.Format("{0}_{1:ddMMyyyyHHmmss}_BOLETO.pdf", pedido.Codigo, pedido.DataCriacao),
                                                        stream,
                                                        out fullName);
                stream.Close();
				Application.SetPermissionForAll(fullName);
                pedido.UrlLocalBoleto = fullName;
                offline.Save(pedido);
                progress.Close();
                View.ShowBoleto(pedido.UrlLocalBoleto);
            }, ex => // Erro
            {
                progress.Close();
                var onlineEx = ex as OnlineException;
                if (onlineEx != null && onlineEx.IsMessageErp)
                {
                    View.ExecuteOnUI(() => View.ShowModalMessage("Erro ao gerar o boleto", onlineEx.Message));
                }
                else
                {
                    View.ExecuteOnUI(() => View.ShowModalMessage("Boleto", "Erro ao gerar o boleto"));
                }
            });
        }

        public void GerarNfePedido(PedidoDto pedido)
        {
            if (!pedido.IsPendingUpload)
            {
                try
                {
                    if (String.IsNullOrEmpty(pedido.UrlLocalNFe))
                    {
                        BaixarNFe(pedido);
                        return;
                    }
                    View.MakeQuestion("Deseja reprocessar a NFe?", () => BaixarNFe(pedido),  () => View.ShowNotaFiscal(pedido.UrlLocalNFe));
                }
                catch (Exception ex)
                {
                    View.ShowModalMessage("Nota fiscal", "Erro ao gerar NFe", null);
                    if (ExceptionPolicy.Handle(ex))
                    {
                        throw;
                    }
                }
            }
            else
            {
                View.ShowModalMessage("Nota fiscal", "Pedido n鉶 enviado para o ERP", null);
            }
        }

        private void BaixarNFe(PedidoDto pedido)
        {
            var pedidoDbOnline = DbHelper.GetOnline<IPedidoDb>();
            var progress = View.ShowProgressView(String.Empty);
                        
            ExecuteOnBackgroundView(() =>
                {
                    pedidoDbOnline.ProcessarNFe(pedido);

                    if (!pedido.StatusEmissao.HasNFeEmitida)
                    {
                        progress.Close();
                        View.ExecuteOnUI(() =>
                                            View.ShowModalMessage("Nota fiscal", String.Format("{0}-{1}", pedido.StatusEmissao.CodigoStatusEmissao, pedido.StatusEmissao.DescricaoStatusEmissao), null));
                        return;
                    }

                    var offline = DbHelper.GetOffline<IOfflinePedidoDb>();
                    var stream = Application.Download(pedido.UrlHttpNFe);
                    string fullName;
                    Application.SaveStreamOnApplicationDisk(String.Format("{0}_{1:ddMMyyyyHHmmss}_NFE.pdf", pedido.Codigo, pedido.DataCriacao),
                                                            stream,
                                                            out fullName);
                    stream.Close();
					Application.SetPermissionForAll(fullName);
                    pedido.UrlLocalNFe = fullName;
                    offline.Save(pedido);
                    progress.Close();
                    View.ShowNotaFiscal(pedido.UrlLocalNFe);
                }, ex => // Error
                {
                    progress.Close();
                    var onlineEx = ex as OnlineException;
                    if (onlineEx != null && onlineEx.IsMessageErp)
                    {
                        View.ExecuteOnUI(() => View.ShowModalMessage("Erro ao gerar a NFe", onlineEx.Message));
                    }
                    else
                    {
                        View.ExecuteOnUI(() => View.ShowModalMessage("Nota Fiscal", "Erro ao gerar a NFe"));
                    }
                });
        }

        protected override void DoClose(Action actionToClose)
        {
            if (_pedidoFinalizado != null && (_pedidoFinalizado.Tipo == TipoPedidoEnum.Venda))
            {
                ApplicationController.RegisterClientePesquisa(_pedidoFinalizado.Cliente);
                //View.Next();
            }
            View.CloseView();
        }
    }
}