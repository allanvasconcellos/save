using System;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;
using INetSales.Objects;

namespace INetSales.ViewController.Sync.Integrators
{
    public class UsuarioSync : Sync<UsuarioDto>
    {
        public UsuarioSync(ConfiguracaoDto configuracao)
            : base(DbHelper.GetOnline<IUsuarioDb>(), DbHelper.GetOffline<IOfflineUsuarioDb>(), "UsuarioIntegra", configuracao)
        {
            //IntervaloIntegracao = configuracao.IntervaloSyncUsuario;
            IntervaloIntegracao = TimeSpan.FromSeconds(30);
        }

        protected override void DoExecuteSync(DateTime dataUltimaIntegracao, DateTime inicioIntegracao, UsuarioDto usuario)
        {
            DoGenericSync(inicioIntegracao);
        }

        protected override bool PreInsert(UsuarioDto dto)
        {
			Logger.Debug("Usuário {0} - {1}", dto.Codigo, dto.SenhaHash);
            dto.IsSyncPending = true;
            return true;
        }

        protected override bool PreUpdate(UsuarioDto dtoOnline, UsuarioDto dtoOffline)
        {
            dtoOnline.IsSyncPending = dtoOffline.IsSyncPending;
            return true;
        }
    }
}