using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;
using INetSales.Objects;

namespace INetSales.ViewController.Sync.Integrators
{
    public class ConfiguracaoSync : Integrator
    {
        #region Overrides of Integrator

        public override void DoExecute(UsuarioDto usuario = null)
        {
            Logger.Info(false, "Iniciando sincronização de configuração");
            var onlineDb = DbHelper.GetOnline<IConfiguracaoDb>();
            var offlineDb = DbHelper.GetOffline<IOfflineConfiguracaoDb>();
            var configuracao = offlineDb.GetConfiguracaoAtiva();
            configuracao.UrlWebService = onlineDb.GetDefaultUrlErp();
            offlineDb.Save(configuracao);
            Logger.Info(false, "Url salva: {0}", configuracao.UrlWebService);
        }

        #endregion
    }
}