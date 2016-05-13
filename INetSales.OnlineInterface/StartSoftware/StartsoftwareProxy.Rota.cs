using System;
using System.Collections.Generic;
using System.Xml;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;

namespace INetSales.OnlineInterface.StartSoftware
{
    public partial class StartsoftwareProxy : IRotaDb
    {
        #region Implementation of IDb<RotaDto>

        IEnumerable<RotaDto> IDb<RotaDto>.GetAll(UsuarioDto usuario)
        {
            throw new NotImplementedException();
        }

        public void Save(RotaDto dto)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Implementation of IRotaDb

        public IEnumerable<Pasta> GetPastas(UsuarioDto usuario)
        {
            var pastas = new List<Pasta>();
            TratarInvokeWeb("GetRota",
            () =>
                {
                    string sendInfo = String.Format("<produtos chave=\"{0}\"><login_vendedor>{1}</login_vendedor></produtos>",
                            _configuracao.ChaveIntegracao ?? ChaveIntegracao, usuario.Codigo);
                    return _comp.getRoteiro(sendInfo);
                },
            (xmlDoc, result) =>
                {
                    pastas.AddRange(LoadPastas(xmlDoc));
                    return String.Empty;
                });
            return pastas;
        }

        #endregion

        private IEnumerable<Pasta> LoadPastas(XmlDocument mainDoc)
        {
            var pastas = new List<Pasta>();

            var idPastaNodes = mainDoc.GetElementsByTagName("id_pasta");
            var clientesNodes = mainDoc.GetElementsByTagName("clientes");
            var nomePastaNodes = mainDoc.GetElementsByTagName("nm_pasta");
            var livreNodes = mainDoc.GetElementsByTagName("st_livre");
            for (int i = 0; i < idPastaNodes.Count; ++i)
            {
                var pasta = new Pasta();
                pasta.Codigo = idPastaNodes[i].InnerText;
                pasta.Indice = i + 1;
                pasta.Nome = nomePastaNodes[i].InnerText;
                var clientes = new List<ClienteDto>();
                bool livre = livreNodes[i].InnerText.ToLower().Equals("sim");
                foreach(XmlNode clienteNode in clientesNodes[i].ChildNodes)
                {
                    var cliente = new ClienteDto
                    {
                        Codigo = clienteNode["id_clifor"].InnerText,
                        OrdemRoteiro = Convert.ToInt32(clienteNode["ordem_visita"].InnerText),
                        IsPermitidoForaDia = livre,
                        IsAtivoRoteiro = true,
                    };
                    clientes.Add(cliente);
                }
                pasta.Clientes = clientes;
                pastas.Add(pasta);
            }
            return pastas;
        }
    }
}