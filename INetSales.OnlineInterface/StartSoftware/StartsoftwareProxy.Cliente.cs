using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using System.Globalization;
using System.Diagnostics;

namespace INetSales.OnlineInterface.StartSoftware
{
    public partial class StartsoftwareProxy : IClienteDb
    {
        private TipoPessoaEnum GetTipoPessoaCliente(string codigo)
        {
            if(codigo.Equals("F"))
            {
                return TipoPessoaEnum.Fisica;
            }
            if(codigo.Equals("J"))
            {
                return TipoPessoaEnum.Juridica;
            }
            return TipoPessoaEnum.Indefinido;
        }

        private string GetCodeTipoPessoaCliente(TipoPessoaEnum tipo)
        {
            switch (tipo)
            {
                case TipoPessoaEnum.Juridica:
                    return "J";
                case TipoPessoaEnum.Fisica:
                    return "F";
                default:
                    throw new InvalidOperationException();
            }
        }

        #region Implementation of IDb<ClienteDto>

        IEnumerable<ClienteDto> IDb<ClienteDto>.GetAll(UsuarioDto usuario)
        {
            var clientes = new List<ClienteDto>();

            TratarInvokeWeb("GetCliente"
            // Invoke
            , () =>
            {
                string sendInfo = String.Format("<produtos chave=\"{0}\"><login_vendedor>{1}</login_vendedor></produtos>", 
                                                _configuracao.ChaveIntegracao ?? ChaveIntegracao, usuario.Codigo);
                return _comp.getCliente(sendInfo);
            },
            // Tratamento
            (xml, r) =>
                {
                    var codigoNodes = xml.GetElementsByTagName("id_cliente");
                    var razaoNodes = xml.GetElementsByTagName("nm_razaosocial");
                    var fantasiaNodes = xml.GetElementsByTagName("nm_fantasia");
                    var documentoNodes = xml.GetElementsByTagName("nr_cnpj_cpf");
                    var tipoPessoaNodes = xml.GetElementsByTagName("tp_pessoa");
                    var usuarioNodes = xml.GetElementsByTagName("login_vendedor");
					var financeiroNodes = xml.GetElementsByTagName("pendencia_financeira");
					var pendenciasNodes = xml.GetElementsByTagName("pendencias");
                    var enderecosNodes = xml.GetElementsByTagName("enderecos");
					var permiteBoleto = xml.GetElementsByTagName("st_permitir_boleto");
					var permiteBoletoNodes = xml.GetElementsByTagName("st_permitir_boleto"); // INET.031
					int countPendencia = 0;
                    if(codigoNodes.Count <= 0)
                    {
                        Logger.Warn(true, "Nenhum cliente retornado para o usuário {0}", usuario.Codigo);
                        return String.Empty;
                    }
                    Logger.Info(true, "{0} cliente(s) retornados para o usuário {1}", codigoNodes.Count, usuario.Codigo);
                    for (int i = 0; i < codigoNodes.Count; ++i)
                    {
                        var clienteRetornado = new ClienteDto
                                                   {
                                                       Codigo = codigoNodes[i].InnerText,
                                                       RazaoSocial = razaoNodes[i].InnerText,
                                                       NomeFantasia = fantasiaNodes[i].InnerText,
                                                       Documento = documentoNodes[i].InnerText,
                                                       TipoPessoa = GetTipoPessoaCliente(tipoPessoaNodes[i].InnerText),
                                                       Usuario =
                                                           new UsuarioDto
                                                               {
                                                                   Codigo = usuarioNodes[i].InnerText,
                                                               },
							HasPendencia = financeiroNodes[i].InnerText == "N" ? false : true,
							IsPermitidoBoleto = permiteBoleto [i].InnerText != "N",
                                                   };
                        foreach (XmlNode enderecoNode in enderecosNodes[i].ChildNodes)
                        {
                            if (!enderecoNode["st_endereco_entrega"].InnerText.ToUpper().Equals("S")) continue;
                            int numero = 0;
                            clienteRetornado.EnderecoRua = enderecoNode["ds_endereco"].InnerText;
                            if (!Int32.TryParse(enderecoNode["numero"].InnerText, out numero))
                            {
                                Logger.Warn(true, "O número \"{0}\" de endereço \"{1}\" para o cliente \"{2}\" não é válido",
                                            enderecoNode["numero"].InnerText, clienteRetornado.EnderecoRua, clienteRetornado.Codigo);
                            }
                            clienteRetornado.EnderecoNumero = numero;
                            clienteRetornado.Cidade = enderecoNode["ds_cidade"].InnerText;
                            clienteRetornado.Bairro = enderecoNode["bairro"].InnerText;
                            clienteRetornado.Estado = enderecoNode["sg_estado"].InnerText;
                            //clienteRetornado.Cep = enderecoNode["sg_estado"].InnerText;
                            break;
                        }

						// Pendencia
						List<PendenciaDto> pendencias = new List<PendenciaDto> ();
						if (pendenciasNodes != null && pendenciasNodes[countPendencia] != null && clienteRetornado.HasPendencia)
						{
							var numberFormat = new CultureInfo(CultureInfo.CurrentCulture.Name).NumberFormat;
							numberFormat.NumberDecimalSeparator = ".";
							foreach (XmlNode pendenciaNode in pendenciasNodes[countPendencia++].ChildNodes)
							{
								PendenciaDto pendenciaInfo = new PendenciaDto();
								double valorTotal, valorEmAberto = 0;
								DateTime dataEmissao, dataVencimento;
								pendenciaInfo.Codigo = pendenciaNode["id_parcela"].InnerText;
								pendenciaInfo.Documento = pendenciaNode["nr_documento"].InnerText;
								pendenciaInfo.LinkPagamento = pendenciaNode["link_pagamento"] != null ? pendenciaNode["link_pagamento"].InnerText : String.Empty;
								if (!Double.TryParse(pendenciaNode["valor_total"].InnerText, NumberStyles.Currency, numberFormat, out valorTotal))
								{
									Logger.Warn(true, "O valor total \"{0}\" da pendencia \"{1}\" para o cliente \"{2}\" não é válido",
										pendenciaNode["valor_total"].InnerText, pendenciaNode["id_parcela"].InnerText, clienteRetornado.Codigo);
									continue;
								}
								if (!Double.TryParse(pendenciaNode["valor_em_aberto"].InnerText, NumberStyles.Currency, numberFormat, out valorEmAberto))
								{
									Logger.Warn(true, "O valor em aberto \"{0}\" da pendencia \"{1}\" para o cliente \"{2}\" não é válido",
										pendenciaNode["valor_em_aberto"].InnerText, pendenciaNode["id_parcela"].InnerText, clienteRetornado.Codigo);
									continue;
								}
								pendenciaInfo.ValorTotal = valorTotal;
								pendenciaInfo.ValorEmAberto = valorEmAberto;
								System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("pt-BR");
								if (!DateTime.TryParse(pendenciaNode["dt_emissao"].InnerText, culture, DateTimeStyles.None, out dataEmissao))
								{
									Logger.Warn(true, "A data da emissão \"{0}\" da pendencia \"{1}\" para o cliente \"{2}\" não é válido",
										pendenciaNode["dt_emissao"].InnerText, pendenciaNode["id_parcela"].InnerText, clienteRetornado.Codigo);
									//continue;
								}
								else {
									pendenciaInfo.DataEmissao = dataEmissao;
								}
								if (!DateTime.TryParse(pendenciaNode["dt_vencimento"].InnerText, culture, DateTimeStyles.None, out dataVencimento))
								{
									Logger.Warn(true, "A data de vencimento \"{0}\" da pendencia \"{1}\" para o cliente \"{2}\" não é válido",
										pendenciaNode["dt_vencimento"].InnerText, pendenciaNode["id_parcela"].InnerText, clienteRetornado.Codigo);
									//continue;
								}
								else {
									pendenciaInfo.DataVencimento = dataVencimento;
								}
								pendencias.Add(pendenciaInfo);
							}
						}

						clienteRetornado.Pendencias = pendencias;

                        clientes.Add(clienteRetornado);
                    }
                    return String.Empty;
                }, 
            // Mensagem de erro
            () => String.Format("Erro na tentativa de obter clientes para o usuário {0}", usuario.Codigo));

            return clientes;
        }

        void IDb<ClienteDto>.Save(ClienteDto dto)
        {
            TratarInvokeWeb("Cliente Save",
            () =>
            {
                var builder = new StringBuilder();
                builder.AppendFormat("<clientes chave=\"{0}\">", _configuracao.ChaveIntegracao ?? ChaveIntegracao);
                builder.Append("      <cliente>");
                builder.AppendFormat("  <nr_cnpj_cpf>{0}</nr_cnpj_cpf>", dto.Documento);
                builder.AppendFormat("  <tp_pessoa>{0}</tp_pessoa>", GetCodeTipoPessoaCliente(dto.TipoPessoa));
                builder.AppendFormat("  <nm_cliente>{0}</nm_cliente>", dto.NomeFantasia);
                builder.AppendFormat("  <email_cliente>{0}</email_cliente>", dto.Email);
                builder.Append("        <endereco>");
                builder.AppendFormat("      <rua>{0}</rua>", dto.EnderecoRua);
                builder.AppendFormat("      <numero>{0}</numero>", dto.EnderecoNumero);
                builder.AppendFormat("      <bairro>{0}</bairro>", dto.Bairro);
                builder.AppendFormat("      <cep>{0}</cep>", dto.Cep);
                builder.AppendFormat("      <telefone>{0}</telefone>", dto.Telefone);
                builder.AppendFormat("      <cidade>{0}</cidade>", dto.Cidade);
                builder.Append("        </endereco>");
                builder.Append("     </cliente>");
                builder.Append("     </clientes>");
                Logger.Info(false, "Xml enviado para envio do cliente {0} - {1}", dto.Codigo, builder.ToString());
                string result = _comp.setCliente(builder.ToString());
                Logger.Info(false, "Resultado Save Cliente - {0}", result);
                if(!String.IsNullOrEmpty(result) && result.StartsWith("ok"))
                {
                    return String.Format("<retorno><codigo>{0}</codigo><msg>{1}</msg></retorno>", "001", String.Empty);
                }
                return result;
            },
            (xml, r) =>
            {
                return String.Empty;
            });
        }

        #endregion
    }
}