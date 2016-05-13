using System;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;

namespace INetSales.ViewController.Sync.Integrators
{
    public class ProdutoSync : Sync<ProdutoDto>
    {
        private readonly bool _syncTodos;

        public ProdutoSync(ConfiguracaoDto configuracao, bool syncTodos = false)
            : base(DbHelper.GetOnline<IProdutoDb>(), DbHelper.GetOffline<IOfflineProdutoDb>(), "ProdutoIntegra", configuracao)
        {
            _syncTodos = syncTodos;
            IntervaloIntegracao = configuracao.IntervaloSyncProduto;
        }

        protected override void DoExecuteSync(DateTime dataUltimaIntegracao, DateTime inicioIntegracao, UsuarioDto usuario)
        {
            DoGenericSync(inicioIntegracao, _syncTodos ? null : usuario);
        }

        protected override bool PreInsert(ProdutoDto dto)
        {
            if(_syncTodos)
            {
                dto.QuantidadeDisponivel = 0;
            }
            AtualizarGrupo(dto);
            return dto.Grupo != null;
        }

        protected override bool PosInsert(ProdutoDto dto, UsuarioDto usuario)
        {
            if (!_syncTodos)
            {
                var produtoDb = Offline as IOfflineProdutoDb;
                produtoDb.AtualizarSaldo(dto, usuario, dto.QuantidadeDisponivel);
                produtoDb.InserirHistorico(dto, usuario, 0, dto.QuantidadeDisponivel, dto.ValorUnitario, String.Empty);
            }
            return true;
        }

        protected override bool PreUpdate(ProdutoDto dtoOnline, ProdutoDto dtoOffline)
        {
            if(_syncTodos)
            {
                return false; // não deixa atualizar, quando vai sincronizar todos os produtos
            }
            if(dtoOnline.Grupo != null)
            {
                if (dtoOffline.Grupo != null && dtoOnline.Grupo.Equals(dtoOffline.Grupo))
                {
                    dtoOnline.Grupo.Id = dtoOffline.Grupo.Id;
                    return true;
                }
                AtualizarGrupo(dtoOnline);
            }
            return true;
        }

        protected override bool PosUpdate(ProdutoDto dto, ProdutoDto old, UsuarioDto usuario)
        {
            var produtoDb = Offline as IOfflineProdutoDb;
            if (dto.QuantidadeDisponivel != old.QuantidadeDisponivel)
            {
                produtoDb.AtualizarSaldo(dto, usuario, dto.QuantidadeDisponivel);
                produtoDb.InserirHistorico(dto, usuario, old.QuantidadeDisponivel, dto.QuantidadeDisponivel, dto.ValorUnitario, String.Empty);
            }
            else if(dto.ValorUnitario != old.ValorUnitario)
            {
                produtoDb.InserirHistorico(dto, usuario, old.QuantidadeDisponivel, dto.QuantidadeDisponivel, dto.ValorUnitario, String.Empty);
            }
            return true;
        }

        protected override bool PreDisable(ProdutoDto dto, UsuarioDto usuario)
        {
            if (dto.QuantidadeDisponivel > 0 && !_syncTodos)
            {
                var produtoDb = Offline as IOfflineProdutoDb;
                produtoDb.AtualizarSaldo(dto, usuario, 0);
                produtoDb.InserirHistorico(dto, usuario, dto.QuantidadeDisponivel, 0, dto.ValorUnitario, "DESATIVADO");
            }
            return false; // Não deixo prosseguir com a rotina generica.
        }

        private void AtualizarGrupo(ProdutoDto dto)
        {
            var grupoDb = DbHelper.GetOffline<IOfflineGrupoDb>();
            if (dto.Grupo != null)
            {
                var grupo = grupoDb.FindByCodigo(dto.Grupo.Codigo) ?? grupoDb.GetGrupoDefault(); // TODO: O GetGrupoDefault é somente temporario.
                dto.Grupo = grupo;
            }
        }
    }
}