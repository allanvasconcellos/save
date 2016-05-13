using System;
using System.Collections.Generic;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;

namespace INetSales.OnlineInterface.StartSoftware
{
    public partial class StartsoftwareProxy : IRamoDb
    {
        #region Implementation of IDb<RamoDto>

        IEnumerable<RamoDto> IDb<RamoDto>.GetAll(UsuarioDto usuario)
        {
            var ramos = new List<RamoDto>();

            TratarInvokeWeb("GetAllRamo",
            () =>
            {
                string sendInfo = String.Format("<produtos chave=\"{0}\"></produtos>", _configuracao.ChaveIntegracao ?? ChaveIntegracao);
                return _comp.getRamoNegocio(sendInfo);
            },
            (doc, xml) =>
            {
                var codigoNodes = doc.GetElementsByTagName("id_ramo_negocio");
                var nomeNodes = doc.GetElementsByTagName("nm_ramo_negocio");
                for (int i = 0; i < codigoNodes.Count; ++i)
                {
                    var ramoRetornado = new RamoDto
                                            {
                        Codigo = codigoNodes[i].InnerText,
                        Nome = nomeNodes[i].InnerText,
                    };
                    ramos.Add(ramoRetornado);
                }
                return String.Empty;
            });

            return ramos;
        }

        void IDb<RamoDto>.Save(RamoDto dto)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}