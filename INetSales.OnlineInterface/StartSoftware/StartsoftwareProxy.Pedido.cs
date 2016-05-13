using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using System.Xml;

namespace INetSales.OnlineInterface.StartSoftware
{
    public partial class StartsoftwareProxy : IPedidoDb
    {
        private string GetCodeTipoPedido(TipoPedidoEnum tipo)
        {
            switch (tipo)
            {
                case TipoPedidoEnum.Venda:
                    return "P";
                case TipoPedidoEnum.Bonificacao:
                    return "B";
                case TipoPedidoEnum.Remessa:
                    return "R";
                case TipoPedidoEnum.Sos:
                    return "R";
                default:
                    throw new NotImplementedException();
            }
        }

        private string TratarEmissaoRetorno(PedidoDto pedido, string codigo, string descricao)
        {
            var statusEmissao = new StatusEmissaoPedido();
            //codigo = "108";
            //descricao = "Serviço Paralisado Momentaneamente (curto prazo)";
            Logger.Info(false, "Pedido {0} retorno - código: {1} - descrição: {2}", pedido.Codigo, codigo, descricao);
            statusEmissao.CodigoStatusEmissao = codigo;
            statusEmissao.DescricaoStatusEmissao = descricao;
            switch (codigo.Trim())
            {
			case "001":
				break;
			case "539":
                pedido.IsRejeitado = true;
				return String.Format("{0}-{1}", codigo, descricao);
            case "002":
                statusEmissao.HasNFeEmitida = true;
                break;
            case "108":
            case "109":
                break;
            case "003":
                pedido.IsCancelado = true;
                break;
            case "099":
                return String.Format("{0}-{1}", codigo, descricao);
            }
            pedido.StatusEmissao = statusEmissao;
            return String.Empty;
        }

        #region Implementation of IPedidoDb

        IEnumerable<PedidoDto> IDb<PedidoDto>.GetAll(UsuarioDto usuario)
        {
            throw new NotImplementedException();
        }

        public void Save(PedidoDto dto)
        {
            Logger.Info(false, "Iniciando envio de pedido {0} do tipo {1}", dto.Codigo, dto.Tipo);

            TratarInvokeWeb("Pedido", 
            () =>
            {
	                var builder = new StringBuilder();
	                string descricaoPedido = String.Format("Pedido {0} gerado pelo sistema Inet Sales.", dto.Codigo);
	                var numberFormat = new CultureInfo(CultureInfo.CurrentCulture.Name).NumberFormat;
	                numberFormat.NumberDecimalSeparator = ".";

	                builder.AppendFormat("<pedidos chave=\"{0}\">", _configuracao.ChaveIntegracao ?? ChaveIntegracao);
	                builder.AppendFormat("<login_vendedor>{0}</login_vendedor>", dto.Usuario.Codigo);
	                builder.AppendFormat("<marca>{0}</marca>", _configuracao.CampoMarca);
	                builder.AppendFormat("<especie>{0}</especie>", _configuracao.CampoEspecie);
	                builder.Append("      <pedido>");
	                builder.AppendFormat("  <nr_pedido_origem>{0}</nr_pedido_origem>", dto.Codigo);
					builder.AppendFormat("  <nr_pedido_compra>{0}</nr_pedido_compra>", dto.OrdemCompra);
	                builder.AppendFormat("  <ds_observacao>{0}</ds_observacao> ", descricaoPedido);
	                builder.AppendFormat("  <id_condicao_pagamento>{0}</id_condicao_pagamento>", dto.Pagamentos != null && dto.Pagamentos.Count() > 0 ? dto.Pagamentos.First().Condicao.Codigo : String.Empty);
	                builder.AppendFormat("  <id_clifor_transportador>{0}</id_clifor_transportador>", String.Empty);
	                builder.AppendFormat("  <id_tabela_preco>{0}</id_tabela_preco>", _configuracao.CodigoTabelaPreco ?? CodigoTabelaPreco);
	                builder.AppendFormat("  <vl_frete>{0}</vl_frete>", "0");
	                builder.AppendFormat("  <status>{0}</status>", "1");
	                builder.AppendFormat("  <tp_lancamento>{0}</tp_lancamento>", GetCodeTipoPedido(dto.Tipo));
					if(_configuracao.IsPreVenda)
						builder.AppendFormat("  <st_gerar_nfe>{0}</st_gerar_nfe>", "N");
					else
						// Informe S para gerar NFe automaticamente (requer configuracao), na remessa e SOS é N
						builder.AppendFormat("  <st_gerar_nfe>{0}</st_gerar_nfe>", dto.Tipo == TipoPedidoEnum.Remessa || dto.Tipo == TipoPedidoEnum.Sos ? "N" : "S");
					builder.AppendFormat("  <qtd_volume>{0}</qtd_volume>", Convert.ToString(dto.Produtos.Sum(p => p.QuantidadePedido), numberFormat));
	                builder.Append("        <itens>");
	                foreach (var produto in dto.Produtos)
	                {
	                    builder.Append("        <item>");
	                    // número que é cadastrado no cadastro de produto no campo "Referencia", pois 
	                    // geralmente os pedidos que chegam através deste método são pedidos de 
	                    // ecommerce e o controle de código de produto do ecommerce é outro.
	                    builder.AppendFormat("      <nr_referencia>{0}</nr_referencia>", produto.Codigo);
	                    builder.AppendFormat("      <vl_unitario>{0}</vl_unitario>", Convert.ToString(produto.ValorUnitario, numberFormat));
	                    if(produto.ValorTotalDesconto > 0)
	                    {
							builder.AppendFormat("      <vl_desconto>{0}</vl_desconto>", Convert.ToString(produto.ValorTotalDesconto / Convert.ToDouble(produto.QuantidadePedido), numberFormat));
	                    }
	                    else
	                    {
	                        builder.AppendFormat("      <vl_desconto>{0}</vl_desconto>", "0");
	                    }
						builder.AppendFormat("      <qtd_produto>{0}</qtd_produto>", Convert.ToString(produto.QuantidadePedido, numberFormat));
	                    builder.Append("        </item>");
	                }
	                builder.Append("        </itens>");
	                builder.Append("        <cliente>");
	                builder.AppendFormat("      <nr_cnpj_cpf>{0}</nr_cnpj_cpf>", dto.Cliente != null ? dto.Cliente.Documento : _configuracao.CnpjEmpresa);
	                builder.Append("        </cliente>");
	                builder.Append("     </pedido>");
	                builder.Append("     </pedidos>");
	                Logger.Info(false, "Xml enviado para envio do pedido {0} - {1}", dto.Codigo, builder.ToString());
	                string retorno = _comp.setPedido(builder.ToString());
	                //string retorno = "ok";
	                if (!String.IsNullOrEmpty(retorno))
	                {
	                    if (retorno.ToLower().Equals("ok"))
	                    {
	                        Logger.Warn(false, "Pedido {0} - Retorno \"ok\"", dto.Codigo);
	                        return String.Format("<retorno><codigo>{0}</codigo><msg>{1}</msg></retorno>", "001", retorno);
	                    }
	                    if (!retorno.ToLower().Contains("<retorno><codigo>"))
	                    {
	                        Logger.Warn(false, "Pedido {0} - Retorno sem formato xml: {1}", dto.Codigo, retorno);
	                        return String.Format("<retorno><codigo>{0}</codigo><msg>{1}</msg></retorno>", "099", retorno);
	                    }
	                }
	                return retorno;
            },
            (doc, r) =>
                {
                    var codigoNodes = doc.GetElementsByTagName("codigo");
                    var msgNodes = doc.GetElementsByTagName("msg");
                    if (codigoNodes.Count > 0)
                    {
                        return TratarEmissaoRetorno(dto, codigoNodes[0].InnerText, msgNodes[0].InnerText);
                    }
                    return String.Format("Código de retorno não enviado - Resultado: {0}", r);
                });

            Logger.Info(false, "Pedido {0} do tipo {1} enviado com sucesso", dto.Codigo, dto.Tipo);
        }

        public void ProcessarNFe(PedidoDto pedido)
        {
            TratarInvokeWeb("ProcessarNFe", 
            () =>
            {
                var builder = new StringBuilder();
                builder.AppendFormat("<pedidos chave=\"{0}\">", _configuracao.ChaveIntegracao ?? ChaveIntegracao);
                builder.Append("      <pedido>");
                builder.AppendFormat("  <nr_pedido_origem>{0}</nr_pedido_origem>", pedido.Codigo); // Numero enviado na tag nr_pedido_origem quando do registro do pedido.
                builder.Append("      </pedido>");
                builder.Append("      </pedidos>");
                Logger.Info(false, "Xml enviado para ProcessarNFe - Pedido {0} - {1}", pedido.Codigo, builder.ToString());
                string retorno = _comp.consultaProcessamentoNFePedido(builder.ToString());
                if (!String.IsNullOrEmpty(retorno))
                {
                    if (retorno.ToLower().Equals("ok"))
                    {
                        Logger.Warn(false, "Pedido {0} - Retorno \"ok\"", pedido.Codigo);
                        return String.Format("<retorno><codigo>{0}</codigo><msg>{1}</msg></retorno>", "001", retorno);
                    }
                    if (!retorno.ToLower().Contains("<retorno><codigo>"))
                    {
                        Logger.Warn(false, "Pedido {0} - Retorno sem formato xml: {1}", pedido.Codigo, retorno);
                        return String.Format("<retorno><codigo>{0}</codigo><msg>{1}</msg></retorno>", "099", retorno);
                    }
                }
                return retorno;
            },
            (doc, r) =>
                {
                    var codigoNodes = doc.GetElementsByTagName("codigo");
                    var msgNodes = doc.GetElementsByTagName("msg");
                    var urlNodes = doc.GetElementsByTagName("url");
                    if (codigoNodes.Count > 0)
                    {
                        string retorno = TratarEmissaoRetorno(pedido, codigoNodes[0].InnerText, msgNodes[0].InnerText);
                        if (pedido.StatusEmissao != null && pedido.StatusEmissao.HasNFeEmitida)
                        {
                            pedido.UrlHttpNFe = String.Format("{0}/{1}", _configuracao.UrlWebService.Replace("IntegraWS?wsdl", String.Empty), urlNodes[0].InnerText);
                        }
                        return retorno;

                    }
                    return String.Format("Código de retorno não enviado - Resultado: {0}", r);
                });
        }

        public bool ProcessarBoleto(PedidoDto pedido)
        {
            TratarInvokeWeb("ProcessarBoleto",
                () =>
                    {
                        var builder = new StringBuilder();
                        builder.AppendFormat("<pedidos chave=\"{0}\">",
                                                _configuracao.ChaveIntegracao ?? ChaveIntegracao);
                        builder.Append("      <pedido>");
                        builder.AppendFormat("  <nr_pedido_origem>{0}</nr_pedido_origem>", pedido.Codigo);
                        builder.Append("      </pedido>");
                        builder.Append("      </pedidos>");
                        Logger.Info(false, "Xml enviado para ProcessarBoleto - Pedido {0} - {1}", pedido.Codigo, builder.ToString());
                        string retorno = _comp.getBoleto(builder.ToString());
                        if (!String.IsNullOrEmpty(retorno))
                        {
                            if (retorno.ToLower().Equals("ok"))
                            {
                                Logger.Warn(false, "Pedido {0} - Retorno \"ok\"", pedido.Codigo);
                                return String.Format("<retorno><codigo>{0}</codigo><msg>{1}</msg></retorno>", "001", retorno);
                            }
                            if (!retorno.ToLower().Contains("<retorno><codigo>"))
                            {
                                Logger.Warn(false, "Pedido {0} - Retorno sem formato xml: {1}", pedido.Codigo, retorno);
                                return String.Format("<retorno><codigo>{0}</codigo><msg>{1}</msg></retorno>", "099", retorno);
                            }
                        }
                        return retorno;
                    },
                (doc, xml) =>
                    {
                        var codigoNodes = doc.GetElementsByTagName("codigo_retorno");
                        var msgNodes = doc.GetElementsByTagName("msg");
                        var urlNodes = doc.GetElementsByTagName("url");
                        if (codigoNodes.Count > 0)
                        {
                            string retorno = TratarEmissaoRetorno(pedido, codigoNodes[0].InnerText, msgNodes[0].InnerText);
                            string codigo = codigoNodes[0].InnerText;
                            if ((pedido.StatusEmissao != null && pedido.StatusEmissao.HasNFeEmitida) || codigo.ToLower().Equals("1"))
                            {
                                pedido.UrlHttpBoleto = String.Format("{0}/{1}",
                                                                         _configuracao.UrlWebService.Replace(
                                                                             "IntegraWS?wsdl", String.Empty),
                                                                         urlNodes[0].InnerText);
                                return String.Empty;
                            }
                            Logger.Warn(false, "Boleto não processada para o pedido {0}. retorno: {1} - {2}",
                                        pedido.Codigo, codigo, msgNodes[0].InnerText);
                            return retorno;
                        }
                        return String.Format("Código de retorno não enviado - Resultado: {0}", xml);
                    });
            return true;
        }

        #endregion
    }
}