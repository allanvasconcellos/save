using System;
using System.Collections.Generic;
using System.Linq;
using INetSales.Objects;
using INetSales.Objects.Dtos;
using INetSales.ViewController.Lib;

namespace INetSales.ViewController.Sync
{
    public abstract class Upload<TUploader> : Integrator
        where TUploader : class, IDto, IUploader
    {
        private readonly IEnumerable<TUploader> _uploaders;
        protected IDb<TUploader> Online { get; set; }
        protected IOfflineDb<TUploader> Offline { get; set; }

        protected Upload(IDb<TUploader> online, IOfflineDb<TUploader> offline, params TUploader[] uploaders)
        {
            _uploaders = uploaders;
            Online = online;
            Offline = offline;
        }

        protected virtual bool PreUpload(TUploader uploader)
        {
            return true;
        }

        protected virtual bool PosUpload(TUploader uploader)
        {
            return true;
        }

        public string MessageOnline { get; private set; }

        public override void DoExecute(UsuarioDto usuario = null)
        {
            var uploadDb = Offline as IUploadDb;
            DateTime inicioSync = DateTime.Now;
            if (uploadDb != null)
            {
                var uploaders = _uploaders;
                if (uploaders.Count() <= 0)
                {
                    uploaders = uploadDb.GetUploadersWithPendind()
                        .Cast<TUploader>();
                }
                foreach (var uploader in uploaders)
                {
                    if (!uploader.IsDesabilitado)
                    {
                        try
                        {
                            if (uploader.Id <= 0)
                            {
                                uploader.DataCriacao = inicioSync;
                                Offline.Save(uploader);
                            }
                            PreUpload(uploader);
                            Online.Save(uploader);
                            uploader.DataAlteracao = inicioSync;
                            uploader.IsPendingUpload = false;
                            uploader.DataLastUpload = inicioSync;
                            Offline.Save(uploader);
                            HasError = false;
                            PosUpload(uploader);
                        }
                        catch (OnlineException ex)
                        {
                            RegisterError(ex, 0);
                            if (ExceptionPolicy.Handle(ex))
                            {
                                throw;
                            }
                        }
                        catch (Exception ex)
                        {
                            HasError = true;
                            if (ExceptionPolicy.Handle(ex))
                            {
                                throw;
                            }
                        }
                    }
                }
            }
        }

    }
}