﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.235
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace w2.Plugin.P0011_Intercom.Webservice.w2Test.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://tt4.w2solution.com/Test/P0011_Intercom/_WebService/w2Service.asmx")]
        public string w2_Plugin_P0011_Intercom_Webservice_w2Test_com_w2solution_tt4_w2Service {
            get {
                return ((string)(this["w2_Plugin_P0011_Intercom_Webservice_w2Test_com_w2solution_tt4_w2Service"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://reddragon.intercom.co.jp/iw2WebService/w2iUserSyncService.asmx")]
        public string w2_Plugin_P0011_Intercom_Webservice_w2Test_jp_co_intercom_reddragon_UserSync_w2iUserSyncService {
            get {
                return ((string)(this["w2_Plugin_P0011_Intercom_Webservice_w2Test_jp_co_intercom_reddragon_UserSync_w2iU" +
                    "serSyncService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://localhost/P0011_Intercom/PrecompiledWebService/w2Service.asmx")]
        public string w2_Plugin_P0011_Intercom_Webservice_w2Test_localhost_w2Service {
            get {
                return ((string)(this["w2_Plugin_P0011_Intercom_Webservice_w2Test_localhost_w2Service"]));
            }
        }
    }
}