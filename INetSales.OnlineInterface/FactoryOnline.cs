using INetSales.Objects;
using INetSales.Objects.Dtos;
using INetSales.OnlineInterface.StartSoftware;

namespace INetSales.OnlineInterface
{
    public static class FactoryOnline
    {
        public static TOnline Get<TOnline>(ConfiguracaoDto configuracao) where TOnline : class, IOnlineDb
        {
            return new StartsoftwareProxy(configuracao) as TOnline;
        }
    }
}