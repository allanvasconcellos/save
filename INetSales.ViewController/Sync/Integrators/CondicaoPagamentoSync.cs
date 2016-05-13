using System;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;

namespace INetSales.ViewController.Sync.Integrators
{
    public class CondicaoPagamentoSync : Sync<CondicaoPagamentoDto>
    {
        public CondicaoPagamentoSync(ConfiguracaoDto configuracao)
            : base(DbHelper.GetOnline<ICondicaoPagamentoDb>(), DbHelper.GetOffline<IOfflineCondicaoPagamentoDb>(), "CondicaoPagamentoIntegra", configuracao)
        {
            IntervaloIntegracao = TimeSpan.Zero;
        }
    }
}
