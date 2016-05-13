using System;
using System.Linq;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;

namespace INetSales.ViewController.Sync.Integrators
{
    public class PesquisaSync : Integrator
    {
        #region Overrides of Integrator

        public override void DoExecute(UsuarioDto usuario = null)
        {
            //using (var session = DbHelper.GetOfflineDbSession())
            //{
                var onlineDb = DbHelper.GetOnline<IPesquisaDb>();
                var offlineDb = DbHelper.GetOffline<IOfflinePesquisaDb>();
                var produtoDb = DbHelper.GetOffline<IOfflineProdutoDb>();
                var produtosParaPesquisa = onlineDb.GetProdutosPrecoPesquisa(DateTime.MinValue);
                var produtosObtidos = produtosParaPesquisa
                    .Select(produtoPesquisa => produtoDb.FindByCodigo(produtoPesquisa.Codigo))
                    .Where(produto => produto != null).ToList();
                if (produtosObtidos.Count > 0)
                {
                    offlineDb.DesativarProdutosPreco();
                    offlineDb.InserirPrecoPesquisa(produtosObtidos);
                }
            //}
        }

        #endregion
    }
}