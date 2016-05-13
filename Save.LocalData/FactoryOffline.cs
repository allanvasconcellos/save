using System;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using Save.LocalData.Repositories;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Ioc;
using System.IO;

namespace Save.LocalData
{
    public static class FactoryOffline
    {
		public const string DB_NAME = "SAVE.db";

		static FactoryOffline()
		{
			string dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), FactoryOffline.DB_NAME);
			SqliteOrmData.Initialize (dbpath);

			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			// Data Map
			SimpleIoc.Default.Register<IData<UsuarioDto>>(() => new SqliteOrmData<UsuarioDto>(dbpath));
			SimpleIoc.Default.Register<IData<ConfiguracaoDto>>(() => new SqliteOrmData<ConfiguracaoDto>(dbpath));
			SimpleIoc.Default.Register<IData<CondicaoPagamentoDto>>(() => new SqliteOrmData<CondicaoPagamentoDto>(dbpath));
			SimpleIoc.Default.Register<IData<IntegraDto>>(() => new SqliteOrmData<IntegraDto>(dbpath));
			SimpleIoc.Default.Register<IData<GrupoDto>>(() => new SqliteOrmData<GrupoDto>(dbpath));
			SimpleIoc.Default.Register<IData<ProdutoDto>>(() => new SqliteOrmData<ProdutoDto>(dbpath));
			SimpleIoc.Default.Register<IData<ProdutoPedidoDto>>(() => new SqliteOrmData<ProdutoPedidoDto>(dbpath));
			SimpleIoc.Default.Register<IData<ProdutoSaldoDto>>(() => new SqliteOrmData<ProdutoSaldoDto>(dbpath));
			SimpleIoc.Default.Register<IData<ProdutoHistoricoDto>>(() => new SqliteOrmData<ProdutoHistoricoDto>(dbpath));
			SimpleIoc.Default.Register<IData<ClienteDto>>(() => new SqliteOrmData<ClienteDto>(dbpath));
			SimpleIoc.Default.Register<IData<ClienteRotaDto>>(() => new SqliteOrmData<ClienteRotaDto>(dbpath));
			SimpleIoc.Default.Register<IData<RotaDto>>(() => new SqliteOrmData<RotaDto>(dbpath));
			SimpleIoc.Default.Register<IData<PendenciaDto>>(() => new SqliteOrmData<PendenciaDto>(dbpath));
			SimpleIoc.Default.Register<IData<PedidoDto>>(() => new SqliteOrmData<PedidoDto>(dbpath));
			SimpleIoc.Default.Register<IData<PagamentoDto>>(() => new SqliteOrmData<PagamentoDto>(dbpath));
			SimpleIoc.Default.Register<IData<PagamentoChequeDto>>(() => new SqliteOrmData<PagamentoChequeDto>(dbpath));
		}

        public static TOffline Get<TOffline>(ConfiguracaoDto configuracao, IDbSession session = null)
            where TOffline : class, IOfflineDb
        {
            if (typeof(TOffline).Equals(typeof(IOfflineRotaDb)))
            {
				return ((IOfflineDb)new RotaRepository(GetData<RotaDto>())) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflinePedidoDb)))
            {
				return ((IOfflineDb)new PedidoRepository(GetData<PedidoDto>())) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineProdutoDb)))
            {
				return ((IOfflineDb)new ProdutoRepository(GetData<ProdutoDto>())) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineClienteDb)))
            {
				return ((IOfflineDb)new ClienteRepository(GetData<ClienteDto>())) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineGrupoDb)))
            {
				return ((IOfflineDb)new GrupoRepository(GetData<GrupoDto>())) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineIntegraDb)))
            {
				return ((IOfflineDb)new IntegraRepository(GetData<IntegraDto>())) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineCondicaoPagamentoDb)))
            {
				return ((IOfflineDb)new CondicaoPagamentoRepository(GetData<CondicaoPagamentoDto>())) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineConfiguracaoDb)))
            {
				return ((IOfflineDb)new ConfiguracaoRepository(GetData<ConfiguracaoDto>())) as TOffline;
            }
            if (typeof(TOffline).Equals(typeof(IOfflineUsuarioDb)))
            {
				return ((IOfflineDb)new UsuarioRepository(GetData<UsuarioDto>())) as TOffline;
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

		public static IData<TDto> GetData<TDto>()
			where TDto: class, IDto, new()
        {
			return ServiceLocator.Current.GetInstance<IData<TDto>> ();
        }

        public static void Close()
        {
        }
    }
}