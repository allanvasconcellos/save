using System;
using System.Collections.Generic;
using INetSales.Objects.Dtos;

namespace INetSales.Objects.DbInterfaces
{
    public interface IPesquisaDb : IOnlineDb
    {
        IEnumerable<ProdutoDto> GetProdutosPrecoPesquisa(DateTime lastQuery);

        void EnviarPesquisa(UsuarioDto usuario, ClienteDto cliente, Dictionary<TipoPesquisaPergunta, bool> perguntas, Dictionary<string, bool> categorias, Dictionary<string, double> precos);
    }
}