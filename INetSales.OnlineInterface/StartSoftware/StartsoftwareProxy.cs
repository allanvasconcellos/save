using System;
using System.Net;
using System.Xml;
using INetSales.Objects;
using INetSales.Objects.Dtos;
using INetSales.OnlineInterface.br.com.integratornet;
using INetSales.OnlineInterface.br.com.integratornet2;
using INetSales.OnlineInterface.inet.integratornet.com.br;

namespace INetSales.OnlineInterface.StartSoftware
{
    public partial class StartsoftwareProxy
    {
        private readonly IntegraWS _comp;
        private readonly PackageService _compINetSales;
        private readonly MobileService _compWebController;
        private readonly ConfiguracaoDto _configuracao;
        private const string ChaveIntegracao = "cd829da5-bcc4-47f4-97b4-3dad157eb419";
        private const string CodigoTabelaPreco = "939"; 

        public StartsoftwareProxy()
        {
            _comp = new IntegraWS();
            _comp.Timeout = Convert.ToInt32(TimeSpan.FromSeconds(20).TotalMilliseconds);
            _compINetSales = new PackageService();
            _compWebController = new MobileService();
        }

        public StartsoftwareProxy(ConfiguracaoDto configuracao) : this()
        {
            _configuracao = configuracao ?? new ConfiguracaoDto();
            _comp.Url = _configuracao.UrlWebService;
			#if DEBUG
			_comp.Url = "http://inet.integratornet.com.br:8080/1.7.5/IntegraWS?wsdl";
			#endif
        }

        private void TratarInvokeWeb(string invoker, Func<string> invokeWeb, Func<XmlDocument, string, string> tratamentoResultado, Func<string> messageError = null)
        {
            string result = String.Empty;
            string retornoTratamento = String.Empty;
            var doc = new XmlDocument();

            // Chama a interface
            try
            {
                Logger.Info(false, "Url webservice - {0}", _comp.Url);
                Logger.Info(false, "Inicio invoke - {0}", invoker);
                result = invokeWeb();
                Logger.Info(false, "Termino invoke - {0}", invoker);
            }
            catch(WebException ex)
            {
                ThrowInvokeError(messageError, ex, ex.Status == WebExceptionStatus.Timeout ? OnlineReturnType.Timeout : OnlineReturnType.SemConexao);
            }
            catch (Exception ex)
            {
                ThrowInvokeError(messageError, ex, OnlineReturnType.Indefinido);
            }

            // Carrega o xml retornado pela interface
            try
            {
                if (!String.IsNullOrEmpty(result))
                {
					result = result
						.Replace("&", "&amp;")
						//.Replace(">", "&gt;")
						//.Replace("<", "&lt;")
						.Replace("<br>", String.Empty);
                }
                doc.LoadXml(result);
            }
            catch (Exception ex)
            {
                string loadXmlErrorMsg = String.Format("Load Xml Error - Message: {0}", messageError != null ? messageError() : String.Empty);
                if (messageError != null)
                {
                    Logger.Error(false, loadXmlErrorMsg);
                }
                Logger.Error(false, String.Format("Resultado não processado - {0}", result));
                throw new OnlineException(loadXmlErrorMsg, OnlineReturnType.Indefinido, ex);
            }

            // Tratar o xml retornado pela interface.
            try
            {
                retornoTratamento = tratamentoResultado(doc, result);
            }
            catch (Exception ex)
            {
                string tratamentoErrorMsg = String.Format("Tratamento Error - Message: {0}", messageError != null ? messageError() : String.Empty);
                if (messageError != null)
                {
                    Logger.Error(false, tratamentoErrorMsg);
                }
                throw new OnlineException(tratamentoErrorMsg, OnlineReturnType.Indefinido, ex);
            }

            if (!String.IsNullOrEmpty(retornoTratamento))
            {
                throw new OnlineException(retornoTratamento, OnlineReturnType.ErpMessage);
            }
        }

        private void ThrowInvokeError(Func<string> messageError, Exception ex, OnlineReturnType returnType)
        {
            string invokeErrorMsg = String.Format("Invoke Web Error - Message: {0}", messageError != null ? messageError() : ex.Message);
            if (messageError != null)
            {
                Logger.Error(false, invokeErrorMsg);
            }
            throw new OnlineException(invokeErrorMsg, returnType, ex);
        }
    }
}