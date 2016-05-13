using System;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.OfflineInterface.Dal;
using INetSales.OfflineInterface.AndroidDb;

namespace INetSales.OfflineInterface
{
    public static class FactoryOffline
    {
        public static TOffline Get<TOffline>(ConfiguracaoDto configuracao, IDbSession session = null)
            where TOffline : class, IOfflineDb
        {
            if (typeof(TOffline).Equals(typeof(IOfflineRotaDb)))
            {
				var context = session as IDbContext<RotaDto> ?? GetDbContext();
                return ((IOfflineDb)new RotaDal(context)) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflinePedidoDb)))
            {
                return ((IOfflineDb)new PedidoDal(context)) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineProdutoDb)))
            {
                return ((IOfflineDb)new ProdutoDal(context)) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineClienteDb)))
            {
                return ((IOfflineDb)new ClienteDal(context)) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineGrupoDb)))
            {
                return ((IOfflineDb)new GrupoDal(context)) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineIntegraDb)))
            {
                return ((IOfflineDb)new IntegraDal(context)) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineCondicaoPagamentoDb)))
            {
                return ((IOfflineDb)new CondicaoPagamentoDal(context)) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineConfiguracaoDb)))
            {
                return ((IOfflineDb)new ConfiguracaoDal(context)) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflinePesquisaDb)))
            {
                //return ((IOfflineDb)new PesquisaDal(context)) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineUsuarioDb)))
            {
                return ((IOfflineDb)new UsuarioDal(context)) as TOffline;
            }
//            if (typeof(TOffline).Equals(typeof(IOfflineLogDb)))
//            {
//                return ((IOfflineDb)new LogDal(context)) as TOffline;
//            }
            //if (typeof(TOffline).Equals(typeof(IOfflineRamoDb)))
            //{
            //    return ((IOfflineDb)new RamoDal(context)) as TOffline;
            //}
            throw new NotImplementedException();
        }

		public static IDbContext GetDbContext()
        {
            //Logger.Debug();
            return new AndroidDbContext();
        }

        public static void Close()
        {
            AndroidDbContext.CloseAll();
        }
    }
}