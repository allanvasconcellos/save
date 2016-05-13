using System.Collections.Generic;
using INetSales.Objects.Dtos;

namespace INetSales.Objects.DbInterfaces
{
    public interface IOfflineProdutoDb : IOfflineDb<ProdutoDto>
    {
        /// <summary>
        /// Retorna os produtos estocados do usuário.
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="grupo">Inclui o filtro de grupo.</param>
        /// <returns></returns>
        IEnumerable<ProdutoDto> GetProdutosEstocados(UsuarioDto usuario, GrupoDto grupo = null);

        IEnumerable<ProdutoDto> GetProdutos(PedidoDto pedido);

		IEnumerable<ProdutoDto> GetAllProdutos();

		bool InserirHistorico(ProdutoDto produto, UsuarioDto usuario, decimal quantidadeAntiga, decimal quantidadeNova, double valor, string motivo);

		void AtualizarSaldo(ProdutoDto produto, UsuarioDto usuario, decimal saldoAtualizado);
    }
}