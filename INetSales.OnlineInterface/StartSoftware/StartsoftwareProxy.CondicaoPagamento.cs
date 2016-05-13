using System;
using System.Collections.Generic;
using System.Xml;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;

namespace INetSales.OnlineInterface.StartSoftware
{
    public partial class StartsoftwareProxy : ICondicaoPagamentoDb
    {
        #region Implementation of IDb<CondicaoPagamentoDto>

        IEnumerable<CondicaoPagamentoDto> IDb<CondicaoPagamentoDto>.GetAll(UsuarioDto usuario)
        {
            var condicoes = new List<CondicaoPagamentoDto>();
            TratarInvokeWeb("GetCondicaoPagamento"
            // Invoke
            , () =>
            {
                string sendInfo = String.Format("<produtos chave=\"{0}\"></produtos>",
                                                _configuracao.ChaveIntegracao ?? ChaveIntegracao);
                return _comp.getCondicaoPagamento(sendInfo);
            },
            // Tratamento
            (xml, r) =>
            {
                var idCondicaoNodes = xml.GetElementsByTagName("id_condicao_pagamento");
                var descricaoNodes = xml.GetElementsByTagName("ds_condicao_pagamento");
                var stCondicaoNodes = xml.GetElementsByTagName("st_condicao_pagamento"); // INET.032
                var tpCondicaoNodes = xml.GetElementsByTagName("tp_avista_prazo"); // INET.032
                for (int i = 0; i < idCondicaoNodes.Count; ++i)
                {
                    var condicao = new CondicaoPagamentoDto()
                                        {
                                            Codigo = idCondicaoNodes[i].InnerText,
                                            Descricao = descricaoNodes[i].InnerText,
                                        };
                    //A: Avista
                    //P: Prazo
                    //C: Cheque
                    //B: Boleto
                    // INET.031 - Vericando quais condićões são boletos
                    if (tpCondicaoNodes[i].InnerText.ToUpper().Equals("B"))
                    //if (condicao.Descricao.ToLower().Contains("boleto"))
                    {
                        condicao.IsBoleto = true;
                    }
                    // P - Publica
                    // R - Privada
                    if(stCondicaoNodes[i].InnerText.Equals("P")) // INET.032
                    {
                        condicao.IsPublica = true;
                    }
                    condicoes.Add(condicao);
                }
                return String.Empty;
            },
            // Mensagem de erro
            () => "Erro na tentativa de obter condi珲es de pagamento");

            return condicoes;
        }

        public void Save(CondicaoPagamentoDto dto)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}