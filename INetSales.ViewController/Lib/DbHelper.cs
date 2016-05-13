using System;
using System.Collections.Generic;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.OnlineInterface;
using Save.LocalData;

namespace INetSales.ViewController.Lib
{
    public static class DbHelper
    {
        public static TOffline GetOffline<TOffline>(IDbSession session = null)
            where TOffline : class, IOfflineDb
        {
            return FactoryOffline.Get<TOffline>(GetOfflineConfiguracaoAtiva(), session);
        }

        public static TOnline GetOnline<TOnline>()
            where TOnline : class, IOnlineDb
        {
            return FactoryOnline.Get<TOnline>(GetOfflineConfiguracaoAtiva());
        }

        public static ConfiguracaoDto GetOfflineConfiguracaoAtiva(IDbSession session = null)
        {
            return GetOfflineConfiguracaoDb(session).GetConfiguracaoAtiva();
        }

        public static IOfflineConfiguracaoDb GetOfflineConfiguracaoDb(IDbSession session = null)
        {
            return FactoryOffline.Get<IOfflineConfiguracaoDb>(null, session);
        }

//        public static IDbSession GetOfflineDbSession()
//        {
//            return FactoryOffline.GetDbSession();
//        }
    }
}