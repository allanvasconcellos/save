using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;

namespace INetSales.ViewController.Sync.Integrators
{
    public class RamoSync : Sync<RamoDto>
    {
        public RamoSync(ConfiguracaoDto configuracao) :
            base(DbHelper.GetOnline<IRamoDb>(), DbHelper.GetOffline<IOfflineRamoDb>(), "RamoIntegra", configuracao)
        {
            IntervaloIntegracao = configuracao.IntervaloSyncRamo;
        }
    }
}