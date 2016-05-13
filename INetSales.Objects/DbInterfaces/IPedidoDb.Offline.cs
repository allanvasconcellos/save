using System;
using System.Collections.Generic;
using INetSales.Objects.Dtos;

namespace INetSales.Objects.DbInterfaces
{
    public interface IOfflinePedidoDb : IOfflineDb<PedidoDto>
    {
        /// <summary>
        /// Verifica se existe pedido pendente.
        /// </summary>
        bool VerificarPedidoPendente();

        IEnumerable<PedidoDto> GetPedidosPorData(UsuarioDto usuario, DateTime dataInicio, DateTime dataFinal);

        decimal GetTotalValorPagoBoleto(ProdutoDto produtoDto);

		decimal GetTotalValorPagoCheque(ProdutoDto produtoDto);

		decimal GetTotalValorPagoDinheiro(ProdutoDto produtoDto);

        /// <summary>
        /// Inserir o produto para o pedido.
        /// </summary>
        /// <param name="pedido"></param>
        /// <param name="produto"></param>
        void InserirProdutoPedido(PedidoDto pedido, ProdutoDto produto);

        /// <summary>
        /// Inserir um pagamento para o pedido.
        /// </summary>
        /// <param name="pedido"></param>
        /// <param name="pagamento"></param>
        void InserirPagamentoPedido(PedidoDto pedido , PagamentoDto pagamento);
    }
}