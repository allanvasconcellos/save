using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;

namespace INetSales.OnlineInterface.StartSoftware
{
    public partial class StartsoftwareProxy : IUsuarioDb
    {
        #region Implementation of IDb<UsuarioDto>

        IEnumerable<UsuarioDto> IDb<UsuarioDto>.GetAll(UsuarioDto usuario)
        {
            var usuarios = new List<UsuarioDto>();

            TratarInvokeWeb("Usuario",
            () =>
            {
                string sendInfo = String.Format("<produtos chave=\"{0}\"></produtos>", _configuracao.ChaveIntegracao ?? ChaveIntegracao);
                return _comp.getUsuario(sendInfo);
            },
            (xml, r) =>
            {
                var idNodes = xml.GetElementsByTagName("id_usuario");
                var nomeNodes = xml.GetElementsByTagName("nm_usuario");
                var loginNodes = xml.GetElementsByTagName("login");
                var senhaNodes = xml.GetElementsByTagName("senha");
                var tipoNodes = xml.GetElementsByTagName("tipo");
                for (int i = 0; i < nomeNodes.Count; ++i)
                {
                    var usuarioRetornado = new UsuarioDto
                    {
                        CodigoSecundario = idNodes[i].InnerText,
                        Nome = nomeNodes[i].InnerText,
                        Username = loginNodes[i].InnerText,
                        SenhaHash = senhaNodes[i].InnerText,
                        Codigo = loginNodes[i].InnerText,
                    };
                    if (!String.IsNullOrEmpty(tipoNodes[i].InnerText) &&
                        tipoNodes[i].InnerText.ToLower().Equals("administrador"))
                    {
                        usuarioRetornado.IsAdm = true;
                    }
                    usuarios.Add(usuarioRetornado);
                }
                return String.Empty;
            });

            return usuarios;
        }

        public void Save(UsuarioDto dto)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}