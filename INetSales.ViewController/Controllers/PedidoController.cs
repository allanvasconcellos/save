using System;
using System.Collections.Generic;
using System.Linq;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;
using INetSales.ViewController.Views;

namespace INetSales.ViewController.Controllers
{
    public partial class PedidoController : BaseController<IPedidoView>
    {
        public PedidoDto PedidoCorrente { get; private set; }
        private List<ProdutoDto> _produtosSelecionados;
        private IPagamentoChildView _pagamentoView;
        public const string paramPedidoFinalizado = "paramPedidoFinalizado";

        public PedidoController(IPedidoView view, IApplication application)
            : base(view, application, null)
        {
        }

        public bool HasProdutoSelecionado
        {
            get
            {
                return _produtosSelecionados != null && _produtosSelecionados.Count > 0;
            }
        }

        internal void InitializeViewClienteNovo(ClienteDto clienteNovo)
        {
            var clienteNovoView = (IPedidoClienteNovoView) View;
            View.Initialize(this);

            _produtosSelecionados = new List<ProdutoDto>();
            clienteNovo.OrdemRoteiro = 1;
            PedidoCorrente = new PedidoDto
            {
                Cliente = clienteNovo,
                Usuario = Session.UsuarioLogado,
                Produtos = _produtosSelecionados,
                Tipo = TipoPedidoEnum.Venda, // Senão tiver pedido no roteiro do cliente, continuará sendo venda.
                Pagamentos = new List<PagamentoDto>(),
                Roteiro = new RotaDto {Dia = DateTime.Now, Usuario = Session.UsuarioLogado, },
            };

            AtualizarTitle(PedidoCorrente);

            clienteNovoView.DesabilitarFinalizarPedido();
            clienteNovoView.DesabilitarPagamento();
            MostrarProdutosEstocados();
        }

        internal void InitializeViewPedidoRoteiro(ClienteDto clientePedido, DateTime diaRoteiro)
        {
            View.Initialize(this);

            _produtosSelecionados = new List<ProdutoDto>();
            PedidoCorrente = new PedidoDto
                                 {
                                     Cliente = clientePedido,
                                     Usuario = Session.UsuarioLogado,
                                     Roteiro = clientePedido.RoteiroCorrente,
                                     Produtos = _produtosSelecionados,
                                     Tipo = TipoPedidoEnum.Venda, // Senão tiver pedido no roteiro do cliente, continuará sendo venda.
                                     Pagamentos = new List<PagamentoDto>(),
                                 };
            AtualizarTitle(PedidoCorrente);
            if (clientePedido.HasPedidoRoteiro)
            {
                // Mostrar as opções de tipo de pedido (Venda, Bonificação)
                View.ShowSelecaoTipoPedido(
                    () => // Tipo pedido venda selecionado
                    {
                        PedidoCorrente.Tipo = TipoPedidoEnum.Venda;
                    },
                    () => // Tipo pedido bonificação selecionado
                    {
                        PedidoCorrente.Tipo = TipoPedidoEnum.Bonificacao;
                        View.DesabilitarPagamento();
                    });
            }

            View.DesabilitarFinalizarPedido();
            View.DesabilitarPagamento();
            MostrarProdutosEstocados();
        }

        private void MostrarProdutosEstocados()
        {
            var offline = DbHelper.GetOffline<IOfflineProdutoDb>();
            Logger.Info(false, "Performance - Iniciou GetProdutosEstocados - {0}", Session.UsuarioLogado.Username);
            var produtos = offline.GetProdutosEstocados(Session.UsuarioLogado);
            Logger.Info(false, "Performance - Terminou GetProdutosEstocados - {0}", Session.UsuarioLogado.Username);
            foreach (var produto in produtos)
            {
                produto.QuantidadePedido = 0;
            }
            View.AtualizarProdutosSelecao(produtos);
        }

        private void MostrarTodosProdutos()
        {
            var offline = DbHelper.GetOffline<IOfflineProdutoDb>();
            var produtos = offline.GetAllProdutos();
            View.AtualizarProdutosSelecao(produtos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="quantidade"></param>
        /// <returns>Retorna o saldo do produto pedido.</returns>
		public ProdutoDto AtualizarQuantidadePedido(ProdutoDto produto, decimal quantidade)
        {
            if (PedidoCorrente.Tipo == TipoPedidoEnum.Remessa || PedidoCorrente.Tipo == TipoPedidoEnum.Sos)
            {
                return AtualizarQuantidadePedidoRemessa(produto, quantidade);
            }

            var produtoPedido = _produtosSelecionados.FirstOrDefault(p => p.Id == produto.Id);
            if (produtoPedido == null)
            {
                produtoPedido = produto;
                _produtosSelecionados.Add(produtoPedido);
            }
            
            if(quantidade > produtoPedido.QuantidadeDisponivel) // Se a quantidade disponivel chegar ao limite.
            {
                View.ShowMessage("Ultrapassou a quantidade disponivel");
            }
            else if (quantidade > 0) // Atualiza
            {
                produtoPedido.QuantidadePedido = quantidade;
                produtoPedido.SaldoAtual = produtoPedido.QuantidadeDisponivel - produtoPedido.QuantidadePedido;
            }
            else // Se quantidade pedido é 0, remover da lista.
            {
                produto.QuantidadePedido = quantidade;
                produto.SaldoAtual = produto.QuantidadeDisponivel;
                _produtosSelecionados.Remove(produtoPedido);
            }
            AtualizarValorTotal(produtoPedido);

            View.AtualizarProdutosSelecionados(PedidoCorrente.Produtos);
            if (HasProdutoSelecionado)
            {
                View.PermitirFinalizarPedido();
                if (PedidoCorrente.Tipo == TipoPedidoEnum.Venda)
                {
                    View.PermitirPagamento();
                    CalcularValores();
                }
            }
            else
            {
                View.DesabilitarPagamento();
                View.DesabilitarFinalizarPedido();
            }
            return produtoPedido;
        }

        private void AtualizarTitle(PedidoDto pedido)
        {
            string titleCliente = String.Empty;
            switch (pedido.Tipo)
            {
                case TipoPedidoEnum.Venda:
                case TipoPedidoEnum.Bonificacao:
                    titleCliente = String.Format("{0}|Cliente: {1}|Roteiro: Ordem - {2} / Dia - {3:dd/MM/yyyy}",
                                                        CurrentTitle,
                                                        pedido.Cliente.RazaoSocial,
                                                        pedido.Cliente.OrdemRoteiro,
                                                        pedido.Roteiro.Dia);
                    break;
                case TipoPedidoEnum.Remessa:
                case TipoPedidoEnum.Sos:
                    titleCliente = CurrentTitle;
                    break;
            }
            View.UpdateViewTitle(titleCliente.Split('|'));
        }

		public void InserirOC (string oc)
		{
			PedidoCorrente.OrdemCompra = oc;
		}

        protected override void DoClose(Action actionToClose)
        {
            if (HasProdutoSelecionado)
            {
                View.MakeQuestion("Deseja cancelar o pedido?", actionToClose, () => { });
                return;
            }
            actionToClose();
        }
    }
}