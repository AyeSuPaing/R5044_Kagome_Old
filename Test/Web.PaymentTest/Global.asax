<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Configuration" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e) 
    {
		// アプリケーション初期化時にWebRequestのSSLプロトコルバージョン設定
		System.Net.ServicePointManager.SecurityProtocol
			= System.Net.SecurityProtocolType.Tls12
			| System.Net.SecurityProtocolType.Tls11
			| System.Net.SecurityProtocolType.Tls
			| System.Net.SecurityProtocolType.Ssl3;

		//------------------------------------------------------
		// アプリケーション設定読み込み
		//------------------------------------------------------
		// アプリケーション名
		Constants.APPLICATION_NAME = WebConfigurationManager.AppSettings["Application_Name"];

		// アプリケーション共通の設定			
		w2.App.Common.ConfigurationSetting csSetting
			= new w2.App.Common.ConfigurationSetting(
				WebConfigurationManager.AppSettings["ConfigFileDirPath"],
				w2.App.Common.ConfigurationSetting.ReadKbn.C000_AppCommon,
				w2.App.Common.ConfigurationSetting.ReadKbn.C100_SiteCommon,
				w2.App.Common.ConfigurationSetting.ReadKbn.C200_CommonManager,
				w2.App.Common.ConfigurationSetting.ReadKbn.C300_ComerceManager);
	}
    
    void Application_End(object sender, EventArgs e) 
    {
        //  アプリケーションのシャットダウンで実行するコードです

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // ハンドルされていないエラーが発生したときに実行するコードです

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // 新規セッションを開始したときに実行するコードです

    }

    void Session_End(object sender, EventArgs e) 
    {
        // セッションが終了したときに実行するコードです 
        // メモ: Session_End イベントは、Web.config ファイル内で sessionstate モードが
        // InProc に設定されているときのみ発生します。session モードが StateServer か、または 
        // SQLServer に設定されている場合、イベントは発生しません。

    }
       
</script>
