﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.235
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace w2.Plugin.P0011_Intercom.Webservice.w2Test.IcRef {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="IcRef.IIntercomService")]
    public interface IIntercomService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIntercomService/SyncUserData", ReplyAction="http://tempuri.org/IIntercomService/SyncUserDataResponse")]
        System.Data.DataSet SyncUserData(System.Data.DataSet ds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIntercomService/SerialDelete", ReplyAction="http://tempuri.org/IIntercomService/SerialDeleteResponse")]
        System.Data.DataSet SerialDelete(System.Data.DataSet ds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIntercomService/SerialCheck", ReplyAction="http://tempuri.org/IIntercomService/SerialCheckResponse")]
        System.Data.DataSet SerialCheck(System.Data.DataSet ds);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IIntercomService/GetUserRecommendData", ReplyAction="http://tempuri.org/IIntercomService/GetUserRecommendDataResponse")]
        System.Data.DataSet GetUserRecommendData(System.Data.DataSet ds);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IIntercomServiceChannel : w2.Plugin.P0011_Intercom.Webservice.w2Test.IcRef.IIntercomService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class IntercomServiceClient : System.ServiceModel.ClientBase<w2.Plugin.P0011_Intercom.Webservice.w2Test.IcRef.IIntercomService>, w2.Plugin.P0011_Intercom.Webservice.w2Test.IcRef.IIntercomService {
        
        public IntercomServiceClient() {
        }
        
        public IntercomServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public IntercomServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IntercomServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public IntercomServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Data.DataSet SyncUserData(System.Data.DataSet ds) {
            return base.Channel.SyncUserData(ds);
        }
        
        public System.Data.DataSet SerialDelete(System.Data.DataSet ds) {
            return base.Channel.SerialDelete(ds);
        }
        
        public System.Data.DataSet SerialCheck(System.Data.DataSet ds) {
            return base.Channel.SerialCheck(ds);
        }
        
        public System.Data.DataSet GetUserRecommendData(System.Data.DataSet ds) {
            return base.Channel.GetUserRecommendData(ds);
        }
    }
}
