//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

namespace INetSales.OnlineInterface.br.com.integratornet2 {
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
    [System.Web.Services.WebServiceBindingAttribute(Name="BasicHttpBinding_IMobileService", Namespace="http://tempuri.org/")]
    public partial class MobileService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        /// <remarks/>
        public MobileService() {
            this.Url = "http://integratornet.com.br/inetsales_service/MobileService.svc";
        }
        
        public MobileService(string url) {
            this.Url = url;
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IMobileService/ObterUrlDefaultInterfaceErp", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string ObterUrlDefaultInterfaceErp() {
            object[] results = this.Invoke("ObterUrlDefaultInterfaceErp", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginObterUrlDefaultInterfaceErp(System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("ObterUrlDefaultInterfaceErp", new object[0], callback, asyncState);
        }
        
        /// <remarks/>
        public string EndObterUrlDefaultInterfaceErp(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/IMobileService/TryUrlAndroidPatch", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public void TryUrlAndroidPatch([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string filterVersion, out bool TryUrlAndroidPatchResult, [System.Xml.Serialization.XmlIgnoreAttribute()] out bool TryUrlAndroidPatchResultSpecified, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] out string urlPatch, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] out string currentVersion) {
            object[] results = this.Invoke("TryUrlAndroidPatch", new object[] {
                        filterVersion});
            TryUrlAndroidPatchResult = ((bool)(results[0]));
            TryUrlAndroidPatchResultSpecified = ((bool)(results[1]));
            urlPatch = ((string)(results[2]));
            currentVersion = ((string)(results[3]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginTryUrlAndroidPatch(string filterVersion, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("TryUrlAndroidPatch", new object[] {
                        filterVersion}, callback, asyncState);
        }
        
        /// <remarks/>
        public void EndTryUrlAndroidPatch(System.IAsyncResult asyncResult, out bool TryUrlAndroidPatchResult, out bool TryUrlAndroidPatchResultSpecified, out string urlPatch, out string currentVersion) {
            object[] results = this.EndInvoke(asyncResult);
            TryUrlAndroidPatchResult = ((bool)(results[0]));
            TryUrlAndroidPatchResultSpecified = ((bool)(results[1]));
            urlPatch = ((string)(results[2]));
            currentVersion = ((string)(results[3]));
        }
    }
}
