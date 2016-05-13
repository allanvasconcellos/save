using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;
using System.Diagnostics;

namespace INetSales.ViewController.Sync
{
    public abstract class Sync<TDto> : Integrator
        where TDto : class, IDto
    {
        protected IDb<TDto> Online { get; set; }
        protected IOfflineDb<TDto> Offline { get; set; }
        protected TimeSpan IntervaloIntegracao { get; set; }
        protected ConfiguracaoDto Configuracao { get; set; }

        public string CodigoIntegracao { get; set; }

        protected Sync(IDb<TDto> online, IOfflineDb<TDto> offline, string codigoSync, ConfiguracaoDto configuracao)
        {
            Online = online;
            Offline = offline;

            _updated = new List<IDto>();
            _inserted = new List<IDto>();
            _disabled = new List<IDto>();

            CodigoIntegracao = codigoSync;
            Configuracao = configuracao;
        }

        protected virtual bool PreInsert(TDto dto)
        {
            return true;
        }

        protected virtual bool PosInsert(TDto dto, UsuarioDto usuario)
        {
            return true;
        }

        protected virtual bool PreUpdate(TDto dtoOnline, TDto dtoOffline)
        {
            return true;
        }

        protected virtual bool PosUpdate(TDto dto, TDto old, UsuarioDto usuario)
        {
            return true;
        }

        protected virtual bool PreDisable(TDto dto, UsuarioDto usuario)
        {
            return true;
        }

        /// <summary>
        /// Mantem atualizado a base online com a offline de forma generica.
        /// </summary>
        protected void DoGenericSync(DateTime inicioIntegracao, UsuarioDto usuario = null)
        {
            IEnumerable<TDto> dtosOffline = Offline.GetAll(usuario);
            IEnumerable<TDto> dtosOnline = Online.GetAll(usuario);

            // Se encontrar o dto offline na lista de online.
            // Atualização
            foreach (var update in dtosOffline
                .Join(dtosOnline, off => off.Codigo, onl => onl.Codigo,
                      (off, onl) => new { DtoOffline = off, DtoOnline = onl }))
            {
                update.DtoOnline.Id = update.DtoOffline.Id;
                if (PreUpdate(update.DtoOnline, update.DtoOffline))
                {
                    Logger.Debug("Atualizando item {0} - Sincronização {1}", update.DtoOnline.Codigo, CodigoIntegracao);
                    try
                    {
                        update.DtoOnline.DataAlteracao = inicioIntegracao;
                        update.DtoOnline.IsDesabilitado = false;
						update.DtoOnline.DataCriacao = update.DtoOffline.DataCriacao;
						update.DtoOnline.Codigo = update.DtoOffline.Codigo;
                        Offline.Save(update.DtoOnline);
                        PosUpdate(update.DtoOnline, update.DtoOffline, usuario);
                        _updated.Add(update.DtoOnline);
                    }
                    catch (Exception ex)
                    {
                        ExceptionPolicy.Handle(ex);
                        HasError = true;
                    }
                }
            }

            // Se não encontrar o dto online na lista de offline.
            // Inserir
            foreach (var insert in dtosOnline.Where(onl => !dtosOffline.Contains(onl)))
            {
                if (PreInsert(insert))
                {
					//Debug.Assert (insert.Codigo.Equals ("424350"));
                    Logger.Debug("Inserindo item {0} - Sincronização {1}", insert.Codigo, CodigoIntegracao);
                    try
                    {
                        insert.DataCriacao = inicioIntegracao;
                        Offline.Save(insert);
                        PosInsert(insert, usuario);
                        _inserted.Add(insert);
                    }
                    catch (Exception ex)
                    {
                        ExceptionPolicy.Handle(ex);
                        HasError = true;
                    }
                }
            }

            // Se não encontrar o dto offline na lista de online.
            // Desabilitar
            foreach (var disable in dtosOffline.Where(off => !dtosOnline.Contains(off)))
            {
                if (PreDisable(disable, usuario))
                {
                    Logger.Debug("Desabilitando item {0} - Sincronização {1}", disable.Codigo, CodigoIntegracao);
                    disable.DataAlteracao = inicioIntegracao;
                    disable.IsDesabilitado = true;
                    Offline.Save(disable);
                    _disabled.Add(disable);
                }
            }

            Logger.Info(true, "{0} inseridos", _inserted.Count);
            Logger.Info(true, "{0} atualizados", _updated.Count);
            Logger.Info(true, "{0} desabilitados", _disabled.Count);
        }

        protected virtual void DoExecuteSync(DateTime dataUltimaIntegracao, DateTime inicioIntegracao, UsuarioDto usuario)
        {
            DoGenericSync(inicioIntegracao, usuario);
        }

        public override void DoExecute(UsuarioDto usuario = null)
        {
            var inicioIntegracao = DateTime.Now;

            Logger.Info(true, "Iniciando sync - {0} - Hora: {1}", CodigoIntegracao, inicioIntegracao);
            Logger.Info(true, "Intervalo - {0}", IntervaloIntegracao);

            ///////////////////
            // Popula informações de integração
            var integraDb = DbHelper.GetOffline<IOfflineIntegraDb>();
            var infoIntegracao = integraDb.FindByCodigo(CodigoIntegracao);
            if (infoIntegracao == null)
            {
                infoIntegracao = new IntegraDto
                                     {
                                         DataInicio = inicioIntegracao,
                                         Intervalo = IntervaloIntegracao,
                                         Codigo = CodigoIntegracao
                                     };
                integraDb.Save(infoIntegracao);
            }
            /////////////////

            Logger.Info(true, "Ultima integração - {0} ", infoIntegracao.DataUltimaIntegracao);

            if (!infoIntegracao.DataUltimaIntegracao.HasValue) // Primeira executação
            {
                DoExecuteSync(DateTime.MinValue, inicioIntegracao, usuario);
            }
            else if (inicioIntegracao - infoIntegracao.DataUltimaIntegracao >= IntervaloIntegracao) // Só executa se tive passado o intervalo.
            {
                DoExecuteSync(infoIntegracao.DataUltimaIntegracao.Value, inicioIntegracao, usuario);
            }

            infoIntegracao.DataUltimaIntegracao = inicioIntegracao;
            infoIntegracao.Intervalo = IntervaloIntegracao;
            integraDb.Save(infoIntegracao);

            Logger.Info(true, "Terminando sync - {0} - Hora: {1}", CodigoIntegracao, DateTime.Now);
        }

        #region Dados Sync

        private readonly List<IDto> _updated;
        public IEnumerable<IDto> Updated
        {
            get { return _updated; }
        }

        private readonly List<IDto> _inserted;
        public IEnumerable<IDto> Inserted
        {
            get { return _inserted; }
        }

        private readonly List<IDto> _disabled;
        public IEnumerable<IDto> Disabled
        {
            get { return _disabled; }
        }

        #endregion

        public bool IsExecuted { get; protected set; }
    }
}
