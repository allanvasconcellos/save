using System;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;

namespace INetSales.ViewController.Sync.Integrators
{
    public class ClienteSync : Sync<ClienteDto>
    {
        public ClienteSync(ConfiguracaoDto configuracao)
            : base(DbHelper.GetOnline<IClienteDb>(), DbHelper.GetOffline<IOfflineClienteDb>(), "ClienteIntegra", configuracao)
        {
            IntervaloIntegracao = configuracao.IntervaloSyncCliente;
        }

        protected override bool PreInsert(ClienteDto dto)
        {
            Logger.Debug("Inserindo cliente {0}", dto.Codigo);
            var clienteToUpdate = Offline.FindByCodigo(dto.Codigo);
            if(clienteToUpdate != null)
            {
                Logger.Debug("Atualizando cliente {0} no insert", dto.Codigo);
                dto.Id = clienteToUpdate.Id;
            }
            AtualizarUsuario(dto);
            return true;
        }

        protected override bool PreUpdate(ClienteDto dtoOnline, ClienteDto dtoOffline)
        {
            Logger.Debug("Atualizando cliente {0}", dtoOnline.Codigo);
            AtualizarUsuario(dtoOnline);
            return true;
        }

		private void AtualizarUsuario(ClienteDto clienteOnline)
        {
            var usuarioDb = DbHelper.GetOffline<IOfflineUsuarioDb>();
            //dto.HasRota = true;
            if (clienteOnline.Usuario != null && !clienteOnline.Usuario.Equals(Session.UsuarioLogado))
            {
				// Condição senão for igual ao usuário logado
                string codigo = clienteOnline.Usuario.Codigo;
                clienteOnline.Usuario = usuarioDb.FindByCodigo(codigo);
            }
            else
            {
				// Condição para igual ao usuário logado, não precisa ir no banco, obtem da sessão.
                clienteOnline.Usuario = Session.UsuarioLogado;
            }
        }
    }
}