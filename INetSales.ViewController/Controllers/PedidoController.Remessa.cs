using System;
using System.Collections.Generic;
using System.Linq;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Views;

namespace INetSales.ViewController.Controllers
{
    public partial class PedidoController
    {
        internal void InitializeViewRemessa()
        {
            var remessaView = (IRemessaView)View;
            View.Initialize(this);

            // Adiciona o cliente ao titulo
            string titleRemessa = String.Format("{0}|Pedido de {1}",
                CurrentTitle,
                remessaView.IsSos ? "SOS" : "Remessa");
            View.UpdateViewTitle(titleRemessa.Split('|'));

            _produtosSelecionados = new List<ProdutoDto>();
            PedidoCorrente = new PedidoDto
            {
                Usuario = Session.UsuarioLogado,
                Produtos = _produtosSelecionados,
                Tipo = remessaView.IsSos ? TipoPedidoEnum.Sos : TipoPedidoEnum.Remessa,
                Pagamentos = new List<PagamentoDto>(),
            };

            View.DesabilitarFinalizarPedido();
            View.DesabilitarPagamento();
			View.DesabilitarOC();
            MostrarTodosProdutos();
        }

        public ProdutoDto AtualizarQuantidadePedidoRemessa(ProdutoDto produto, decimal quantidade)
        {
            var produtoPedido = _produtosSelecionados.FirstOrDefault(p => p.Id == produto.Id);
            if (produtoPedido == null)
            {
                produtoPedido = produto;
                _produtosSelecionados.Add(produtoPedido);
            }

            if (quantidade > 0) // Atualiza
            {
                produtoPedido.QuantidadePedido = quantidade;
            }
            else // Se quantidade pedido é 0, remover da lista.
            {
                produto.QuantidadePedido = quantidade;
                _produtosSelecionados.Remove(produtoPedido);
            }

            View.AtualizarProdutosSelecionados(PedidoCorrente.Produtos);
            if (HasProdutoSelecionado)
            {
                View.PermitirFinalizarPedido();
            }
            else
            {
                View.DesabilitarPagamento();
                View.DesabilitarFinalizarPedido();
            }
            return produtoPedido;
        }
    }
}