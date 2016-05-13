using System;
using System.Linq;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;

namespace INetSales.ViewController.Sync.Integrators
{
    public class PedidoUpload : Upload<PedidoDto>
    {
        public PedidoUpload(params PedidoDto[] uploaders)
            : base(DbHelper.GetOnline<IPedidoDb>(), DbHelper.GetOffline<IOfflinePedidoDb>(), uploaders)
        {
        }

        protected override bool PreUpload(PedidoDto uploader)
        {
            if (uploader.Produtos == null || uploader.Produtos.Count() <= 0)
            {
                var produtoDb = DbHelper.GetOffline<IOfflineProdutoDb>();
                uploader.Produtos = produtoDb.GetProdutos(uploader);
                foreach (var produtoPedido in uploader.Produtos)
                {
					produtoPedido.ValorPedidoSemDesconto = produtoPedido.ValorUnitario * Convert.ToDouble(produtoPedido.QuantidadePedido);
                    if (produtoPedido.Desconto > 0)
                    {
                        produtoPedido.ValorTotalDesconto = (produtoPedido.Desconto / 100) *
							(produtoPedido.ValorUnitario * Convert.ToDouble(produtoPedido.QuantidadePedido));
                    }
                    produtoPedido.ValorTotalPedido = produtoPedido.ValorPedidoSemDesconto * produtoPedido.ValorTotalDesconto;
                }
            }
            return true;
        }
    }
}