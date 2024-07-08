﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.261
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// このソース コードは Microsoft.VSDesigner、バージョン 4.0.30319.261 によって自動生成されました。
// 
#pragma warning disable 1591

namespace w2.Plugin.P0011_Intercom.ReSync.Intercom_User {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using System.Data;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="w2iUserSyncServiceSoap", Namespace="http://tempuri.org/")]
    public partial class w2iUserSyncService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback UserSyncOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public w2iUserSyncService() {
            this.Url = global::w2.Plugin.P0011_Intercom.ReSync.Properties.Settings.Default.w2_Plugin_P0011_Intercom_ReSync_jp_co_intercom_secure_w2iUserSyncService;
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
        public event UserSyncCompletedEventHandler UserSyncCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/UserSync", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet UserSync(System.Data.DataSet prmDataSet) {
            object[] results = this.Invoke("UserSync", new object[] {
                        prmDataSet});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void UserSyncAsync(System.Data.DataSet prmDataSet) {
            this.UserSyncAsync(prmDataSet, null);
        }
        
        /// <remarks/>
        public void UserSyncAsync(System.Data.DataSet prmDataSet, object userState) {
            if ((this.UserSyncOperationCompleted == null)) {
                this.UserSyncOperationCompleted = new System.Threading.SendOrPostCallback(this.OnUserSyncOperationCompleted);
            }
            this.InvokeAsync("UserSync", new object[] {
                        prmDataSet}, this.UserSyncOperationCompleted, userState);
        }
        
        private void OnUserSyncOperationCompleted(object arg) {
            if ((this.UserSyncCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.UserSyncCompleted(this, new UserSyncCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    public delegate void UserSyncCompletedEventHandler(object sender, UserSyncCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class UserSyncCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal UserSyncCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataSet Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataSet)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591