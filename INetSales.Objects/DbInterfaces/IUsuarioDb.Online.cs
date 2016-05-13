using INetSales.Objects.Dtos;

namespace INetSales.Objects.DbInterfaces
{
    public interface IUsuarioDb : IDb<UsuarioDto>, IOnlineDb
    {
    }
}