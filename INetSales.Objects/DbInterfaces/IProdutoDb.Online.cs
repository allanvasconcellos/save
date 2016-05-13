using System.Collections.Generic;
using INetSales.Objects.Dtos;

namespace INetSales.Objects.DbInterfaces
{
    public interface IProdutoDb : IDb<ProdutoDto>, IOnlineDb
    {
    }
}