//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

namespace INetSales.OnlineInterface.br.com.integratornet {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("XamarinStudio", "4.0.0.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="PackageServiceSoap", Namespace="http://tempuri.org/")]
    public partial class PackageService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        /// <remarks/>
        public PackageService() {
            this.Url = "http://integratornet.com.br/inetsales/packageservice.asmx";
        }
        
        public PackageService(string url) {
            this.Url = url;
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/TryGetUrlAndroidPackage", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool TryGetUrlAndroidPackage(string lastVersion, out string urlPackage, out string version) {
            object[] results = this.Invoke("TryGetUrlAndroidPackage", new object[] {
                        lastVersion});
            urlPackage = ((string)(results[1]));
            version = ((string)(results[2]));
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginTryGetUrlAndroidPackage(string lastVersion, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("TryGetUrlAndroidPackage", new object[] {
                        lastVersion}, callback, asyncState);
        }
        
        /// <remarks/>
        public bool EndTryGetUrlAndroidPackage(System.IAsyncResult asyncResult, out string urlPackage, out string version) {
            object[] results = this.EndInvoke(asyncResult);
            urlPackage = ((string)(results[1]));
            version = ((string)(results[2]));
            return ((bool)(results[0]));
        }
    }
}
