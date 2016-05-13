using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using INetSales.Objects;
using INetSales.Objects.DbInterfaces;
using INetSales.Objects.Dtos;
using System.Globalization;

namespace INetSales.OnlineInterface.StartSoftware
{
	public partial class StartsoftwareProxy : IRelatorioDb
    {
        #region IRelatorioDb implementation

		public bool GetLinkFaturamentoProdutoPorCategoria(DateTime? inicio, DateTime? fim, UsuarioDto usuario, bool isPedido, out string link)
        {
			link = String.Empty;
			string linkText = link;
			TratarInvokeWeb("GetLinkFaturamentoProdutoPorCategoria"
				// Invoke
				, () =>
				{
					StringBuilder sendInfo = new StringBuilder();
					sendInfo.AppendFormat("<pedidos chave=\"{0}\">", _configuracao.ChaveIntegracao ?? ChaveIntegracao);
					if(usuario != null)
					{
						sendInfo.AppendFormat("<login_vendedor>{0}</login_vendedor>", usuario.Username);
					}
					if(inicio.HasValue)
					{
						sendInfo.AppendFormat("<dt_inicial>{0}</dt_inicial>", inicio.Value.ToString("yyyy/MM/dd"));
					}
					if(fim.HasValue)
					{
						sendInfo.AppendFormat("<dt_final>{0}</dt_final>", fim.Value.ToString("yyyy/MM/dd"));
					}
					sendInfo.Append("<pedido>");
					sendInfo.AppendFormat("<tp_lancamento>{0}</tp_lancamento>", isPedido ? "P" : "B");
					sendInfo.Append("</pedido>");
					sendInfo.Append("</pedidos>");

					string result = _comp.getRelatorioFaturamentoProdutoPorCategoria(sendInfo.ToString());
					//return String.Format("<url>{0}</url>", result);
					return result;
				},
				// Tratamento
				(xml, r) =>
				{
					string message = TentarProcessarRetornoRelatorio(xml, out linkText);
					if(!String.IsNullOrEmpty(message))
					{
						Logger.Warn(true, "Error ao gerar relatorio para o usuario {0}, mensagem {1}, link retornado {2}", 
							usuario != null ? usuario.Codigo : String.Empty, message, linkText);
						return message;
					}
					Logger.Info(true, "Relatorio gerado para o usuario {0}", usuario.Codigo);
					return String.Empty;
				}, 
				// Mensagem de erro
				() => String.Format("Erro na tentativa de obter relatorio de faturamento por categoria para o usuario {0}", usuario.Codigo));

			link = linkText;
			return !String.IsNullOrEmpty(link);
        }

        public bool GetLinkPedidoOrcamento(DateTime? inicio, DateTime? fim, UsuarioDto usuario, bool isPedido, out string link)
        {
            link = String.Empty;
            string linkText = link;
            TratarInvokeWeb("GetLinkPedidoOrcamento"
                // Invoke
                , () =>
                {
					string sendInfo = String.Empty;
					sendInfo += String.Format("<pedidos chave=\"{0}\">", _configuracao.ChaveIntegracao ?? ChaveIntegracao);
					if(usuario != null)
					{
						sendInfo += String.Format("<login_vendedor>{0}</login_vendedor>", usuario.Username);
					}
					if(inicio.HasValue)
					{
						sendInfo += String.Format("<dt_inicial>{0:yyyy/MM/dd}</dt_inicial>", inicio.Value);
					}
					if(fim.HasValue)
					{
						sendInfo += String.Format("<dt_final>{0:yyyy/MM/dd}</dt_final>", fim.Value);
					}
					sendInfo += "<pedido>";
					sendInfo += String.Format("<tp_lancamento>{0}</tp_lancamento>", isPedido ? "P" : "B");
					sendInfo += "</pedido>";
					sendInfo += "</pedidos>";

					string result = _comp.getRelatorioPedido(sendInfo);
					//return String.Format("<url>{0}</url>", result);
					return result;
                },
                // Tratamento
                (xml, r) =>
                {
                    string message = TentarProcessarRetornoRelatorio(xml, out linkText);
                    if(!String.IsNullOrEmpty(message))
                    {
                        Logger.Warn(true, "Error ao gerar relatorio para o usuario {0}, mensagem {1}, link retornado {2}", 
                            usuario != null ? usuario.Codigo : String.Empty, message, linkText);
                        return message;
                    }
                    Logger.Info(true, "Relatorio gerado para o usuario {0}", usuario.Codigo);
                    return String.Empty;
                }, 
                // Mensagem de erro
                () => String.Format("Erro na tentativa de obter relatorio de pedidos e orcamento para o usuario {0}", usuario.Codigo));

            link = linkText;
            return !String.IsNullOrEmpty(link);
        }

		public bool GetLinkClienteDevedores (System.DateTime? inicio, System.DateTime? fim, INetSales.Objects.Dtos.UsuarioDto usuario, bool isPedido, out string link)
		{
			link = String.Empty;
			string linkText = link;
			TratarInvokeWeb("GetLinkClienteDevedores"
				// Invoke
				, () =>
				{
					string sendInfo = String.Empty;
					sendInfo += String.Format("<pedidos chave=\"{0}\">", _configuracao.ChaveIntegracao ?? ChaveIntegracao);
					if(usuario != null)
					{
						sendInfo += String.Format("<login_vendedor>{0}</login_vendedor>", usuario.Username);
					}
					if(inicio.HasValue)
					{
						sendInfo += String.Format("<dt_inicial>{0:yyyy/MM/dd}</dt_inicial>", inicio.Value);
					}
					if(fim.HasValue)
					{
						sendInfo += String.Format("<dt_final>{0:yyyy/MM/dd}</dt_final>", fim.Value);
					}
					sendInfo += "<pedido>";
					sendInfo += String.Format("<tp_lancamento>{0}</tp_lancamento>", isPedido ? "P" : "B");
					sendInfo += "</pedido>";
					sendInfo += "</pedidos>";

					string result = _comp.getRelatorioContas(sendInfo);
					//return String.Format("<url>{0}</url>", result);
					return result;
				},
				// Tratamento
				(xml, r) =>
				{
					string message = TentarProcessarRetornoRelatorio(xml, out linkText);
					if(!String.IsNullOrEmpty(message))
					{
						Logger.Warn(true, "Error ao gerar relatorio para o usuario {0}, mensagem {1}, link retornado {2}", 
							usuario != null ? usuario.Codigo : String.Empty, message, linkText);
						return message;
					}
					Logger.Info(true, "Relatorio gerado para o usuario {0}", usuario.Codigo);
					return String.Empty;
				}, 
				// Mensagem de erro
				() => String.Format("Erro na tentativa de obter relatorio de clientes devedores para o usuario {0}", usuario.Codigo));

			link = linkText;
			return !String.IsNullOrEmpty(link);
		}

        private string TentarProcessarRetornoRelatorio(XmlDocument doc, out string linkRelatorio)
        {
            var codigoRetornoNodes = doc.GetElementsByTagName("codigo_retorno");
            var msgNodes = doc.GetElementsByTagName("msg");
            var urlNodes = doc.GetElementsByTagName("url");
            Uri uri = null;
            linkRelatorio = String.Empty;
            if(codigoRetornoNodes[0].InnerText.Equals("0"))
            {
                return msgNodes[0].InnerText;
            }
            linkRelatorio = urlNodes[0].InnerText;
			linkRelatorio = _comp.Url.Replace ("IntegraWS?wsdl", "") + linkRelatorio;
            if (!Uri.TryCreate(linkRelatorio, UriKind.Absolute, out uri))
            {
                return "A url é inválido";
            }
            return String.Empty;
        }

        #endregion
    }
}