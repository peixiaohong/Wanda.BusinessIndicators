﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码是由 Microsoft.VSDesigner 4.0.30319.42000 版自动生成。
// 
#pragma warning disable 1591

namespace Plugin.SSO.SSO.WebServices {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="HrmUserServiceHttpBinding", Namespace="http://localhost/services/HrmUserService")]
    public partial class HrmUserService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback checkUserOperationCompleted;
        
        private System.Threading.SendOrPostCallback getTokenOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public HrmUserService() {
            this.Url = global::Plugin.SSO.Properties.Settings.Default._04_SSO_SSO_WebServices_HrmUserService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event checkUserCompletedEventHandler checkUserCompleted;
        
        /// <remarks/>
        public event getTokenCompletedEventHandler getTokenCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://localhost/services/HrmUserService", ResponseNamespace="http://localhost/services/HrmUserService", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("out")]
        public bool checkUser([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string in0, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string in1) {
            object[] results = this.Invoke("checkUser", new object[] {
                        in0,
                        in1});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void checkUserAsync(string in0, string in1) {
            this.checkUserAsync(in0, in1, null);
        }
        
        /// <remarks/>
        public void checkUserAsync(string in0, string in1, object userState) {
            if ((this.checkUserOperationCompleted == null)) {
                this.checkUserOperationCompleted = new System.Threading.SendOrPostCallback(this.OncheckUserOperationCompleted);
            }
            this.InvokeAsync("checkUser", new object[] {
                        in0,
                        in1}, this.checkUserOperationCompleted, userState);
        }
        
        private void OncheckUserOperationCompleted(object arg) {
            if ((this.checkUserCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.checkUserCompleted(this, new checkUserCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://localhost/services/HrmUserService", ResponseNamespace="http://localhost/services/HrmUserService", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("out", IsNullable=true)]
        public string getToken([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string in0, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string in1) {
            object[] results = this.Invoke("getToken", new object[] {
                        in0,
                        in1});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getTokenAsync(string in0, string in1) {
            this.getTokenAsync(in0, in1, null);
        }
        
        /// <remarks/>
        public void getTokenAsync(string in0, string in1, object userState) {
            if ((this.getTokenOperationCompleted == null)) {
                this.getTokenOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetTokenOperationCompleted);
            }
            this.InvokeAsync("getToken", new object[] {
                        in0,
                        in1}, this.getTokenOperationCompleted, userState);
        }
        
        private void OngetTokenOperationCompleted(object arg) {
            if ((this.getTokenCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getTokenCompleted(this, new getTokenCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void checkUserCompletedEventHandler(object sender, checkUserCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class checkUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal checkUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void getTokenCompletedEventHandler(object sender, getTokenCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getTokenCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getTokenCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591