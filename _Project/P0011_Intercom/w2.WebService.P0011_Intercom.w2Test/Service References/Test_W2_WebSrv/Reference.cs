﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.235
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace w2.Plugin.P0011_Intercom.Webservice.w2Test.Test_W2_WebSrv {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="Test_W2_WebSrv.Iw2Service")]
    public interface Iw2Service {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Iw2Service/UserSyncExecute", ReplyAction="http://tempuri.org/Iw2Service/UserSyncExecuteResponse")]
        System.Data.DataSet UserSyncExecute(System.Data.DataSet ds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Iw2Service/CreateOnetimePassword", ReplyAction="http://tempuri.org/Iw2Service/CreateOnetimePasswordResponse")]
        System.Data.DataSet CreateOnetimePassword(System.Data.DataSet ds);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface Iw2ServiceChannel : w2.Plugin.P0011_Intercom.Webservice.w2Test.Test_W2_WebSrv.Iw2Service, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class Iw2ServiceClient : System.ServiceModel.ClientBase<w2.Plugin.P0011_Intercom.Webservice.w2Test.Test_W2_WebSrv.Iw2Service>, w2.Plugin.P0011_Intercom.Webservice.w2Test.Test_W2_WebSrv.Iw2Service {
        
        public Iw2ServiceClient() {
        }
        
        public Iw2ServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public Iw2ServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Iw2ServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public Iw2ServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Data.DataSet UserSyncExecute(System.Data.DataSet ds) {
            return base.Channel.UserSyncExecute(ds);
        }
        
        public System.Data.DataSet CreateOnetimePassword(System.Data.DataSet ds) {
            return base.Channel.CreateOnetimePassword(ds);
        }
    }
}
