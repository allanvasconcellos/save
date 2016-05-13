using System;
using System.Collections.Generic;
using System.Linq;
using INetSales.Objects.DbInterfaces;
using INetSales.ViewController.Lib;
using INetSales.ViewController.Models;
using INetSales.ViewController.Views;

namespace INetSales.ViewController.Controllers
{
    public partial class RelatorioController : BaseController<IRelatorioView>
    {
        public RelatorioController(IRelatorioView listView, IApplication application)
            : base(listView, application, null)
        {
        }

        internal void InitializeSinteticoForcaVenda()
        {
            var forcaVendaView = (IRelatorioForcaVendaView) View;
            forcaVendaView.InitializeSintetico(this);
            View.UpdateViewTitle(CurrentTitle);
            var resultadoRelat = GerarQuantidadeProduto();

            forcaVendaView.ShowTotais(resultadoRelat.Sum(g => g.QuantidadeRecebida), resultadoRelat.Sum(g => g.QuantidadeVendida), resultadoRelat.Sum(g => g.QuantidadeDisponivel),
                                                                           resultadoRelat.Sum(g => g.ValorPagoDinheiro), resultadoRelat.Sum(g => g.ValorPagoBoleto), 
                                                                           resultadoRelat.Sum(g => g.ValorPagoCheque));

            forcaVendaView.ShowProdutos(resultadoRelat);
        }

        private IEnumerable<GrupoInfoModel> GerarQuantidadeAnalitico()
        {
            var usuarioEstoque = Session.UsuarioLogado;
            var grupoDb = DbHelper.GetOffline<IOfflineGrupoDb>();
            var pedidoDb = DbHelper.GetOffline<IOfflinePedidoDb>();
            var gruposRelatorio = new List<GrupoInfoModel>();
            var grupos = grupoDb.GetGruposEstocados(usuarioEstoque);

            try
            {

                foreach (var grupoDto in grupos)
                {
                    var grupoRelatorio = new GrupoInfoModel();
                    var produtosRelatorio = new List<ProdutoInfoModel>();
                    grupoRelatorio.Nome = grupoDto.Nome;

                    foreach (var produtoDto in grupoDto.Produtos)
                    {
                        var produtoRelatorio = new ProdutoInfoModel();
                        produtoRelatorio.Nome = produtoDto.Nome;
                        produtoRelatorio.QuantidadeRecebida = produtoDto.QuantidadeDisponivel;
                        produtoRelatorio.QuantidadeVendida = produtoDto.QuantidadeTotalPedido;
                        //produtoRelatorio.QuantidadeDevolver = produtoRelatorio.QuantidadeRecebida -
                        //                                      produtoRelatorio.QuantidadeVendida;
                        produtoRelatorio.ValorPagoBoleto = pedidoDb.GetTotalValorPagoBoleto(produtoDto);
                        produtoRelatorio.ValorPagoCheque = pedidoDb.GetTotalValorPagoCheque(produtoDto);
                        produtoRelatorio.ValorPagoDinheiro = pedidoDb.GetTotalValorPagoDinheiro(produtoDto);
                        produtosRelatorio.Add(produtoRelatorio);
                    }

                    grupoRelatorio.Produtos = produtosRelatorio;
                    gruposRelatorio.Add(grupoRelatorio);
                }
            }
            catch(Exception ex)
            {
                if(ExceptionPolicy.Handle(ex))
                {
                    throw;
                }
            }

            return gruposRelatorio;
        }

        private IEnumerable<GrupoInfoModel> GerarQuantidadeSintetico()
        {
            var gruposRelatorio = GerarQuantidadeAnalitico();
            foreach (var grupoInfo in gruposRelatorio)
            {
                //grupoInfo.QuantidadeRecebida = grupoInfo.Produtos.Sum(p => p.QuantidadeRecebida);
                grupoInfo.QuantidadeDisponivel = grupoInfo.Produtos.Sum(p => p.QuantidadeRecebida);
                grupoInfo.QuantidadeVendida = grupoInfo.Produtos.Sum(p => p.QuantidadeVendida);
                grupoInfo.ValorPagoBoleto = grupoInfo.Produtos.Sum(p => p.ValorPagoBoleto);
                grupoInfo.ValorPagoCheque = grupoInfo.Produtos.Sum(p => p.ValorPagoCheque);
                grupoInfo.ValorPagoDinheiro = grupoInfo.Produtos.Sum(p => p.ValorPagoDinheiro);
                grupoInfo.Produtos = null;
            }
            return gruposRelatorio;
        }

		private IEnumerable<ProdutoInfoModel> GerarQuantidadeProduto()
		{
			var usuarioEstoque = Session.UsuarioLogado;
			var grupoDb = DbHelper.GetOffline<IOfflineGrupoDb>();
			var pedidoDb = DbHelper.GetOffline<IOfflinePedidoDb>();
			var produtosRelatorio = new List<ProdutoInfoModel>();
			var grupos = grupoDb.GetGruposEstocados(usuarioEstoque);

			try
			{
				
				foreach (var grupoDto in grupos)
				{
					foreach (var produtoDto in grupoDto.Produtos)
					{
						var produtoRelatorio = new ProdutoInfoModel();
						produtoRelatorio.Nome = produtoDto.Nome;
						produtoRelatorio.QuantidadeRecebida = produtoDto.QuantidadeDisponivel;
						produtoRelatorio.QuantidadeVendida = produtoDto.QuantidadeTotalPedido;
						produtoRelatorio.QuantidadeDisponivel = produtoRelatorio.QuantidadeRecebida -
						                                      produtoRelatorio.QuantidadeVendida;
						produtoRelatorio.ValorPagoBoleto = pedidoDb.GetTotalValorPagoBoleto(produtoDto);
						produtoRelatorio.ValorPagoCheque = pedidoDb.GetTotalValorPagoCheque(produtoDto);
						produtoRelatorio.ValorPagoDinheiro = pedidoDb.GetTotalValorPagoDinheiro(produtoDto);
						produtosRelatorio.Add(produtoRelatorio);
					}
				}

			}
			catch(Exception ex)
			{
				if(ExceptionPolicy.Handle(ex))
				{
					throw;
				}
			}
			return produtosRelatorio;
		}

    }
}