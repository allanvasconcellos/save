using System;
using System.Collections.Generic;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Controllers;

namespace INetSales.ViewController.Views
{
    public interface IPedidoView : IView
    {
        void Initialize(PedidoController controller);

        void AtualizarProdutosSelecao(IEnumerable<ProdutoDto> produtos);

        void AtualizarProdutosSelecionados(IEnumerable<ProdutoDto> produtos);

        void AtualizarValores(double valorTotalSolicitado, double valorTotalDesconto, double valorFinal);

        void ShowSelecaoTipoPedido(Action selecaoVenda, Action selecaoBonificao);

        void PermitirPagamento();
        
        void DesabilitarPagamento();

		void DesabilitarOC();

        void PermitirFinalizarPedido();

        void DesabilitarFinalizarPedido();
    }
}