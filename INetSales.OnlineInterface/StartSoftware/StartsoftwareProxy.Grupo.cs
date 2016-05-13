using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;

namespace INetSales.OnlineInterface.StartSoftware
{
    public partial class StartsoftwareProxy : IGrupoDb
    {
        #region Implementation of IDb<GrupoDto>

        IEnumerable<GrupoDto> IDb<GrupoDto>.GetAll(UsuarioDto usuario)
        {
            var grupos = new List<GrupoDto>();
            TratarInvokeWeb("GetGrupo"
            // Invoke
            , () =>
                {
                    string sendInfo = String.Format("<produtos chave=\"{0}\"></produtos>",
                                                    _configuracao.ChaveIntegracao ?? ChaveIntegracao);
                    //string sendInfo = String.Format("<produtos chave=\"{0}\"><login_vendedor>{1}</login_vendedor></produtos>", _configuracao.ChaveIntegracao ?? ChaveIntegracao, usuario.Codigo);
                    return _comp.getCategoria(sendInfo);
                },
            // Tratamento
            (xml, r) =>
                {

                    var idCategoriaNodes = xml.GetElementsByTagName("id_categoria");
                    var nomeNodes = xml.GetElementsByTagName("nm_categoria");
                    var paiNodes = xml.GetElementsByTagName("id_categoria_pai");
                    for (int i = 0; i < idCategoriaNodes.Count; ++i)
                    {
                        var grupo = new GrupoDto
                                        {
                                            Codigo = idCategoriaNodes[i].InnerText,
                                            Nome = nomeNodes[i].InnerText,
                                        };
                        if (!String.IsNullOrEmpty(paiNodes[i].InnerText.Trim()))
                        {
                            grupo.GrupoPai = new GrupoDto() {Codigo = paiNodes[i].InnerText.Trim(),};
                        }
                        grupos.Add(grupo);
                    }
                    return String.Empty;
                },
                // Mensagem de erro
                () => "Erro na tentativa de obter os grupos");
            return grupos.OrderBy(g => g.GrupoPai != null);
        }

        public void Save(GrupoDto dto)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}