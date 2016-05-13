using System;
using System.Collections.Generic;
using System.Linq;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;

namespace INetSales.ViewController.Controllers
{
    public partial class PedidoController
    {
        public void FinalizarPedido()
        {
            if (!HasProdutoSelecionado)
            {
                View.ShowMessage("Nenhum produto selecionado");
                return;
            }

            if (PedidoCorrente.Tipo == TipoPedidoEnum.Venda)
            {
                // Verifica se existe forma de pagamento informada.
                if (PedidoCorrente.Pagamentos.Count() <= 0
                    || PedidoCorrente.Pagamentos.Sum(f => f.ValorFinal) <= 0)
                {
                    View.ShowMessage("Nenhuma forma de pagamento informada");
                    return;
                }
            }
            else // Senao for venda, nao tem pagamento
            {
                PedidoCorrente.Pagamentos = new List<PagamentoDto>();
            }

            // TODO: verificar se as informacoes extras de cliente foram informadas.

            View.MakeQuestion("Deseja finalizar o pedido?", () =>
                {
                    // Salvar o pedido, e realizar o upload do pedido
                    if (SalvarLocalPedido())
                    {
                        View.ShowMessage("Pedido salvo");

                        Application.SetValue(paramPedidoFinalizado, PedidoCorrente.Id.ToString());
                        View.CloseView();
                        View.Next();
                        return;
                    }
                    View.ShowMessage("Ocorreu um erro ao salvar o pedido");
                }, () => { });
        }

        private bool SalvarLocalPedido()
        {
            var pedidoDb = DbHelper.GetOffline<IOfflinePedidoDb>();
            var rotaDb = DbHelper.GetOffline<IOfflineRotaDb>();
            var produtoDb = DbHelper.GetOffline<IOfflineProdutoDb>();
            PedidoCorrente.DataCriacao = DateTime.Now;
            PedidoCorrente.IsPendingUpload = true;

            // Remover os pagamentos com valores zerados.
            var pagamentos = new List<PagamentoDto>(PedidoCorrente.Pagamentos);
            pagamentos.RemoveAll(p => p.ValorFinal <= 0);
            PedidoCorrente.Pagamentos = pagamentos;

            // Roteiro
            if (PedidoCorrente.Cliente != null && PedidoCorrente.Cliente.RoteiroCorrente != null)
            {
                PedidoCorrente.Roteiro = PedidoCorrente.Cliente.RoteiroCorrente;
            }

            try
            {
                pedidoDb.Save(PedidoCorrente);

                // PRODUTO/ESTOQUE
                // Ir descontando o saldo.
                foreach (var produto in PedidoCorrente.Produtos)
                {
                    pedidoDb.InserirProdutoPedido(PedidoCorrente, produto);

                    // Parar de processar para não descontar o saldo do produto se for remessa ou SOS.
                    if (PedidoCorrente.Tipo == TipoPedidoEnum.Remessa || PedidoCorrente.Tipo == TipoPedidoEnum.Sos)
                    {
                        continue;
                    }

                    produto.QuantidadeDisponivel = produto.QuantidadeDisponivel - produto.QuantidadePedido;
                    produtoDb.AtualizarSaldo(produto, PedidoCorrente.Usuario, produto.QuantidadeDisponivel);
                }

                // PAGAMENTOS
                foreach (var pagamento in PedidoCorrente.Pagamentos)
                {
                    pagamento.PedidoId = PedidoCorrente.Id;
                    pedidoDb.InserirPagamentoPedido(PedidoCorrente, pagamento);
                }

                if (PedidoCorrente.Cliente != null && PedidoCorrente.Roteiro != null)
                {
                    rotaDb.IndicarPedidoCliente(PedidoCorrente.Roteiro, PedidoCorrente.Cliente);
                }

                // Gerar código pedido
                //PedidoCorrente.Codigo = String.Format("{0}{1}", Math.Abs(Application.GetDevideId().GetHashCode()), PedidoCorrente.Id);
                PedidoCorrente.Codigo = String.Format("{0:yyyyMMddHHmmss}{1}", PedidoCorrente.DataCriacao, PedidoCorrente.Usuario.CodigoSecundario);
                pedidoDb.Save(PedidoCorrente);

                return true;
            }
            catch (Exception ex)
            {
                if (ExceptionPolicy.Handle(ex))
                {
                    throw;
                }
            }
            return false;
        }
    }
}