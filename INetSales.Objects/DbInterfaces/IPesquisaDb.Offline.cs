using System.Collections.Generic;
using INetSales.Objects.Dtos;

namespace INetSales.Objects.DbInterfaces
{
    public interface IOfflinePesquisaDb : IOfflineDb
    {
        void InserirPrecoPesquisa(IEnumerable<ProdutoDto> produtos);

        IEnumerable<ProdutoDto> ObterProdutosPrecoPesquisa();

        IEnumerable<GrupoDto> ObterGruposPesquisa();

        void DesativarProdutosPreco();
    }
}