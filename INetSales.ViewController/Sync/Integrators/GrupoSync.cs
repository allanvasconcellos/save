using System;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;

namespace INetSales.ViewController.Sync.Integrators
{
    public class GrupoSync : Sync<GrupoDto>
    {
        public GrupoSync(ConfiguracaoDto configuracao)
            : base(DbHelper.GetOnline<IGrupoDb>(), DbHelper.GetOffline<IOfflineGrupoDb>(), "GrupoIntegra", configuracao)
        {
            IntervaloIntegracao = configuracao.IntervaloSyncGrupo;
        }

        protected override void DoExecuteSync(DateTime dataUltimaIntegracao, DateTime inicioIntegracao, UsuarioDto usuario)
        {
            DoGenericSync(inicioIntegracao);
        }

		protected override bool PreInsert(GrupoDto dto)
		{
			if (dto.GrupoPai != null) {
				dto.GrupoPai = Offline.FindByCodigo(dto.GrupoPai.Codigo);
			}
			return true;
		}

		protected override bool PreUpdate (GrupoDto dtoOnline, GrupoDto dtoOffline)
		{
			if (dtoOnline.GrupoPai != null) {
				dtoOnline.GrupoPai = Offline.FindByCodigo (dtoOnline.GrupoPai.Codigo);
			}
			return true;
		}
    }
}