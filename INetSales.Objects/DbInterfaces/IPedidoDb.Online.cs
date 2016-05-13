using System;
using System.Collections.Generic;
using INetSales.Objects.Dtos;

namespace INetSales.Objects.DbInterfaces
{
    public interface IPedidoDb : IDb<PedidoDto>, IOnlineDb
    {
        void ProcessarNFe(PedidoDto pedido);

        bool ProcessarBoleto(PedidoDto pedido);
    }
}