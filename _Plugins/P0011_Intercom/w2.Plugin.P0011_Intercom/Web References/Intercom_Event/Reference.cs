﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// このソース コードは Microsoft.VSDesigner、バージョン 4.0.30319.42000 によって自動生成されました。
// 
#pragma warning disable 1591

namespace w2.Plugin.P0011_Intercom.Intercom_Event {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Data;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="RecommendServiceSoap", Namespace="http://tempuri.org/")]
    public partial class RecommendService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetEventOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetEventProductOperationCompleted;
        
        private System.Threading.SendOrPostCallback AuthSerialOperationCompleted;
        
        private System.Threading.SendOrPostCallback AuthSerialOverallOperationCompleted;
        
        private System.Threading.SendOrPostCallback ConsumeSerialOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetConsumeSerialOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetEventListOperationCompleted;
        
        private System.Threading.SendOrPostCallback DeleteConsumedSerialOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public RecommendService() {
            this.Url = global::w2.Plugin.P0011_Intercom.Properties.Settings.Default.w2_Plugin_P0011_Intercom_Intercom_Event_RecommendService;
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
        public event GetEventCompletedEventHandler GetEventCompleted;
        
        /// <remarks/>
        public event GetEventProductCompletedEventHandler GetEventProductCompleted;
        
        /// <remarks/>
        public event AuthSerialCompletedEventHandler AuthSerialCompleted;
        
        /// <remarks/>
        public event AuthSerialOverallCompletedEventHandler AuthSerialOverallCompleted;
        
        /// <remarks/>
        public event ConsumeSerialCompletedEventHandler ConsumeSerialCompleted;
        
        /// <remarks/>
        public event GetConsumeSerialCompletedEventHandler GetConsumeSerialCompleted;
        
        /// <remarks/>
        public event GetEventListCompletedEventHandler GetEventListCompleted;
        
        /// <remarks/>
        public event DeleteConsumedSerialCompletedEventHandler DeleteConsumedSerialCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetEvent", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetEvent(System.Data.DataSet prmDataSet) {
            object[] results = this.Invoke("GetEvent", new object[] {
                        prmDataSet});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void GetEventAsync(System.Data.DataSet prmDataSet) {
            this.GetEventAsync(prmDataSet, null);
        }
        
        /// <remarks/>
        public void GetEventAsync(System.Data.DataSet prmDataSet, object userState) {
            if ((this.GetEventOperationCompleted == null)) {
                this.GetEventOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetEventOperationCompleted);
            }
            this.InvokeAsync("GetEvent", new object[] {
                        prmDataSet}, this.GetEventOperationCompleted, userState);
        }
        
        private void OnGetEventOperationCompleted(object arg) {
            if ((this.GetEventCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetEventCompleted(this, new GetEventCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetEventProduct", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetEventProduct(System.Data.DataSet prmDataSet) {
            object[] results = this.Invoke("GetEventProduct", new object[] {
                        prmDataSet});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void GetEventProductAsync(System.Data.DataSet prmDataSet) {
            this.GetEventProductAsync(prmDataSet, null);
        }
        
        /// <remarks/>
        public void GetEventProductAsync(System.Data.DataSet prmDataSet, object userState) {
            if ((this.GetEventProductOperationCompleted == null)) {
                this.GetEventProductOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetEventProductOperationCompleted);
            }
            this.InvokeAsync("GetEventProduct", new object[] {
                        prmDataSet}, this.GetEventProductOperationCompleted, userState);
        }
        
        private void OnGetEventProductOperationCompleted(object arg) {
            if ((this.GetEventProductCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetEventProductCompleted(this, new GetEventProductCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/AuthSerial", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet AuthSerial(System.Data.DataSet prmDataSet) {
            object[] results = this.Invoke("AuthSerial", new object[] {
                        prmDataSet});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void AuthSerialAsync(System.Data.DataSet prmDataSet) {
            this.AuthSerialAsync(prmDataSet, null);
        }
        
        /// <remarks/>
        public void AuthSerialAsync(System.Data.DataSet prmDataSet, object userState) {
            if ((this.AuthSerialOperationCompleted == null)) {
                this.AuthSerialOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAuthSerialOperationCompleted);
            }
            this.InvokeAsync("AuthSerial", new object[] {
                        prmDataSet}, this.AuthSerialOperationCompleted, userState);
        }
        
        private void OnAuthSerialOperationCompleted(object arg) {
            if ((this.AuthSerialCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AuthSerialCompleted(this, new AuthSerialCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/AuthSerialOverall", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet AuthSerialOverall(System.Data.DataSet prmDataSet) {
            object[] results = this.Invoke("AuthSerialOverall", new object[] {
                        prmDataSet});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void AuthSerialOverallAsync(System.Data.DataSet prmDataSet) {
            this.AuthSerialOverallAsync(prmDataSet, null);
        }
        
        /// <remarks/>
        public void AuthSerialOverallAsync(System.Data.DataSet prmDataSet, object userState) {
            if ((this.AuthSerialOverallOperationCompleted == null)) {
                this.AuthSerialOverallOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAuthSerialOverallOperationCompleted);
            }
            this.InvokeAsync("AuthSerialOverall", new object[] {
                        prmDataSet}, this.AuthSerialOverallOperationCompleted, userState);
        }
        
        private void OnAuthSerialOverallOperationCompleted(object arg) {
            if ((this.AuthSerialOverallCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AuthSerialOverallCompleted(this, new AuthSerialOverallCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ConsumeSerial", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet ConsumeSerial(System.Data.DataSet prmDataSet) {
            object[] results = this.Invoke("ConsumeSerial", new object[] {
                        prmDataSet});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void ConsumeSerialAsync(System.Data.DataSet prmDataSet) {
            this.ConsumeSerialAsync(prmDataSet, null);
        }
        
        /// <remarks/>
        public void ConsumeSerialAsync(System.Data.DataSet prmDataSet, object userState) {
            if ((this.ConsumeSerialOperationCompleted == null)) {
                this.ConsumeSerialOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConsumeSerialOperationCompleted);
            }
            this.InvokeAsync("ConsumeSerial", new object[] {
                        prmDataSet}, this.ConsumeSerialOperationCompleted, userState);
        }
        
        private void OnConsumeSerialOperationCompleted(object arg) {
            if ((this.ConsumeSerialCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConsumeSerialCompleted(this, new ConsumeSerialCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetConsumeSerial", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetConsumeSerial(System.Data.DataSet prmDataSet) {
            object[] results = this.Invoke("GetConsumeSerial", new object[] {
                        prmDataSet});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void GetConsumeSerialAsync(System.Data.DataSet prmDataSet) {
            this.GetConsumeSerialAsync(prmDataSet, null);
        }
        
        /// <remarks/>
        public void GetConsumeSerialAsync(System.Data.DataSet prmDataSet, object userState) {
            if ((this.GetConsumeSerialOperationCompleted == null)) {
                this.GetConsumeSerialOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetConsumeSerialOperationCompleted);
            }
            this.InvokeAsync("GetConsumeSerial", new object[] {
                        prmDataSet}, this.GetConsumeSerialOperationCompleted, userState);
        }
        
        private void OnGetConsumeSerialOperationCompleted(object arg) {
            if ((this.GetConsumeSerialCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetConsumeSerialCompleted(this, new GetConsumeSerialCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetEventList", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet GetEventList(System.Data.DataSet prmDataSet) {
            object[] results = this.Invoke("GetEventList", new object[] {
                        prmDataSet});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void GetEventListAsync(System.Data.DataSet prmDataSet) {
            this.GetEventListAsync(prmDataSet, null);
        }
        
        /// <remarks/>
        public void GetEventListAsync(System.Data.DataSet prmDataSet, object userState) {
            if ((this.GetEventListOperationCompleted == null)) {
                this.GetEventListOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetEventListOperationCompleted);
            }
            this.InvokeAsync("GetEventList", new object[] {
                        prmDataSet}, this.GetEventListOperationCompleted, userState);
        }
        
        private void OnGetEventListOperationCompleted(object arg) {
            if ((this.GetEventListCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetEventListCompleted(this, new GetEventListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/DeleteConsumedSerial", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataSet DeleteConsumedSerial(System.Data.DataSet prmDataSet) {
            object[] results = this.Invoke("DeleteConsumedSerial", new object[] {
                        prmDataSet});
            return ((System.Data.DataSet)(results[0]));
        }
        
        /// <remarks/>
        public void DeleteConsumedSerialAsync(System.Data.DataSet prmDataSet) {
            this.DeleteConsumedSerialAsync(prmDataSet, null);
        }
        
        /// <remarks/>
        public void DeleteConsumedSerialAsync(System.Data.DataSet prmDataSet, object userState) {
            if ((this.DeleteConsumedSerialOperationCompleted == null)) {
                this.DeleteConsumedSerialOperationCompleted = new System.Threading.SendOrPostCallback(this.OnDeleteConsumedSerialOperationCompleted);
            }
            this.InvokeAsync("DeleteConsumedSerial", new object[] {
                        prmDataSet}, this.DeleteConsumedSerialOperationCompleted, userState);
        }
        
        private void OnDeleteConsumedSerialOperationCompleted(object arg) {
            if ((this.DeleteConsumedSerialCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.DeleteConsumedSerialCompleted(this, new DeleteConsumedSerialCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void GetEventCompletedEventHandler(object sender, GetEventCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetEventCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetEventCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void GetEventProductCompletedEventHandler(object sender, GetEventProductCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetEventProductCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetEventProductCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void AuthSerialCompletedEventHandler(object sender, AuthSerialCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AuthSerialCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal AuthSerialCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void AuthSerialOverallCompletedEventHandler(object sender, AuthSerialOverallCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AuthSerialOverallCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal AuthSerialOverallCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void ConsumeSerialCompletedEventHandler(object sender, ConsumeSerialCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConsumeSerialCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ConsumeSerialCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void GetConsumeSerialCompletedEventHandler(object sender, GetConsumeSerialCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetConsumeSerialCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetConsumeSerialCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void GetEventListCompletedEventHandler(object sender, GetEventListCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetEventListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetEventListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    public delegate void DeleteConsumedSerialCompletedEventHandler(object sender, DeleteConsumedSerialCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.2556.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DeleteConsumedSerialCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal DeleteConsumedSerialCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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