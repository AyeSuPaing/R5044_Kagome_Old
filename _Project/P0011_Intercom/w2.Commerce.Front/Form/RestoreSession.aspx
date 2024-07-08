<%--
=========================================================================================================
  Module      : セッション復元ページ(RestoreSession.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Application : w2.Commerce.Front
  BaseVersion : V5.0
  Author      : M.Ochiai
  email       : product@w2solution.co.jp
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
  URL         : http://www.w2solution.co.jp/
=========================================================================================================
PKG-V5.0[PF0001] 2010/09/10 M.Ochiai        v5.0用に分離
PKG-V5.0[PF00XX] 2011/08/22 F.Nagaki        インターコム用に外のページにも飛べるように
--%>
<%@ Page Language="C#" AutoEventWireup="true" Inherits="BasePage" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<script runat="server">
	
	//ここだけ環境ごとに変更
	private const string INTERCOM_SETTINGS_DIR_PATH = @"C:\inetpub\wwwroot\R5044_Kagome.Develop\Web\w2.Commerce.Front\Extern\";
	
	//------------------------------------------------------
	// 設定ファイルからロードするもの
	//------------------------------------------------------	
	//暗号化DLL
	private string ASPX_IC_SSO_URL = "";

	/// <summary>
	/// 設定ファイルロード
	/// </summary>
	private void Load_IntercomConfig()
	{
		//各種設定ファイルからのロード
		//設定ファイルロード
		w2.App.Common.ConfigurationSetting IntercomSettings = new w2.App.Common.ConfigurationSetting(
			INTERCOM_SETTINGS_DIR_PATH,
			w2.App.Common.ConfigurationSetting.ReadKbn.C300_MarketingPlanner,
			w2.App.Common.ConfigurationSetting.ReadKbn.C300_Pc);

		ASPX_IC_SSO_URL = IntercomSettings.GetAppStringSetting("ASPX_IC_SSO_URL");

		//設定ファイルロード
		Load_IntercomConfig();
		
	}
	
	///=============================================================================================
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	///=============================================================================================
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// セッション情報復元
		//------------------------------------------------------
		SessionSecurityManager.RestoreSessionFromDatabaseForChangeSessionId(Context);

		//------------------------------------------------------
		// 画面遷移
		//------------------------------------------------------
		string strNextUrl = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NEXT_URL]);
		if (strNextUrl == "")
		{
			strNextUrl = Constants.PATH_ROOT;
		}

		/******************************インターコム対応********************************/
		/*インターコムの場合はリダイレクトさせる*/
		if (strNextUrl.IndexOf(ASPX_IC_SSO_URL) > -1)
		{
			Response.Redirect(strNextUrl);
		}
		/******************************インターコム対応********************************/

		// 他サイトへ飛ぼうとしていたらURLをルートへ書き換える（踏み台対策）
		if ((strNextUrl.StartsWith(Constants.PATH_ROOT) == false)
			&& (strNextUrl.Contains(Uri.SchemeDelimiter + Request.Url.Host + "/") == false))
		{
			strNextUrl = Constants.PATH_ROOT;
		}

		Response.Redirect(strNextUrl);
	}
</script>
