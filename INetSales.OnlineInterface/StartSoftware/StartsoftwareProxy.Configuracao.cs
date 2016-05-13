using System;
using System.Collections.Generic;
using System.Net;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;

namespace INetSales.OnlineInterface.StartSoftware
{
    public partial class StartsoftwareProxy : IConfiguracaoDb
    {
        #region Implementation of IDb<ConfiguracaoDto>

        public IEnumerable<ConfiguracaoDto> GetAll(UsuarioDto usuario)
        {
            throw new NotImplementedException();
        }

        public void Save(ConfiguracaoDto dto)
        {
            throw new NotImplementedException();
        }

        public string GetDefaultUrlErp()
        {
            try
            {
                Logger.Info(false, "Iniciando GetDefaultUrlErp em {0}", _compWebController.Url);
                return _compWebController.ObterUrlDefaultInterfaceErp();
            }
            catch (WebException ex)
            {
                ThrowInvokeError(null, ex, ex.Status == WebExceptionStatus.Timeout ? OnlineReturnType.Timeout : OnlineReturnType.SemConexao);
            }
            catch (Exception ex)
            {
                ThrowInvokeError(null, ex, OnlineReturnType.Indefinido);
            }
            return String.Empty;
        }

        #endregion
    }
}