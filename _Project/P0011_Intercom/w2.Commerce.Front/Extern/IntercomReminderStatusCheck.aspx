<%--
=========================================================================================================
  Module      : インターコム用リマインダページのHTTPステータスチェック(IntercomReminderStatusCheck.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Application : w2.Commerce.Front
  BaseVersion : V5.0
  Author      : M.Ochiai
  email       : product@w2solution.co.jp
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
  URL         : http://www.w2solution.co.jp/
=========================================================================================================
PKG-V5.0[PF0001] 2010/09/10 M.Ochiai        v5.0用に分離
--%>

<%@ Page Title="無名のページ" Language="C#" Inherits="BasePage" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Net.Security" %>
<%@ Import Namespace="System.Security.Cryptography.X509Certificates" %>
<script runat="server">
	//ここだけ環境ごとに変更
	private const string INTERCOM_SETTINGS_DIR_PATH = @"C:\inetpub\wwwroot\R5044_Kagome.Develop\Web\w2.Commerce.Front\Extern\";
	
	private const string ERR_MSG_STATUS_NG = "パスワードリマインダー機能が現在ご利用いただけません。<br/>誠に申し訳ございませんが、しばらく経ってからご利用いただくか<br/>トップページより再度アクセスをお願い申し上げます。";
	
	//------------------------------------------------------
	// 設定ファイルからロードするもの
	//------------------------------------------------------	
	//暗号化DLL
	private string INTERCOM_PW_REMINDER_URL = "";

	private string INTERCOM_PW_REMINDER_OK_STATUS_CD = "";
	
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

		INTERCOM_PW_REMINDER_URL = IntercomSettings.GetAppStringSetting("INTERCOM_PW_REMINDER_URL");
		INTERCOM_PW_REMINDER_OK_STATUS_CD = IntercomSettings.GetAppStringSetting("INTERCOM_PW_REMINDER_OK_STATUS_CD");

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
		//設定ファイルロード
		Load_IntercomConfig();
		
		//検査対象のURL
		string targetUrl = INTERCOM_PW_REMINDER_URL;

		//HTTPステータスチェック
		if (HttpStatusChk(targetUrl))
		{
			//HTTPステータスが許可ステータスと一致する場合
			//リマインダエージへリダイレクト

			Response.Redirect(targetUrl);
			
		}
		else
		{
			//HTTPステータスが許可ステータスと一致しない場合
			//エラーページへリダイレクト

			Session[Constants.SESSION_KEY_ERROR_MSG] = ERR_MSG_STATUS_NG;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
		
	}

	/// <summary>
	/// URLの正当性チェック
	/// </summary>
	/// <param name="targetUrl">チェック対象のURL</param>
	/// <returns></returns>
	private bool HttpStatusChk(string targetUrl)
	{
		//URLがNullの場合はNG
		if (targetUrl == null)
		{
			return false;
		}

		//URLがEmptyの場合はNG
		if (targetUrl == "")
		{
			return false;
		}

		HttpWebRequest req;

		try
		{
			//URLリクエスト
			req = (HttpWebRequest)WebRequest.Create(targetUrl);
		}
		catch
		{
			return false;
		}


		HttpWebResponse res = null;
		HttpStatusCode statusCode;

		//許可するステータスコード
		string[] okStatusStrs = INTERCOM_PW_REMINDER_OK_STATUS_CD.Split(",".ToCharArray());

		//チェック結果・初期値False
		bool chkVal = false;

		//SSL証明を無視するようにコールバックに登録
		//テスト環境用の対応
		//HttpWebResponseでSSLは見たくないのでそのための対応
		ServicePointManager.ServerCertificateValidationCallback =
				new System.Net.Security.RemoteCertificateValidationCallback(validationCallBack);

		try
		{
			res = (HttpWebResponse)req.GetResponse();
			statusCode = res.StatusCode;

		}
		catch (WebException ex)
		{

			res = (HttpWebResponse)ex.Response;
			if (res != null)
			{
				//結果が返ってこない
				statusCode = res.StatusCode;
			}
			else
			{
				//ここの場合はサーバー接続不可の場合は404扱い
				statusCode = HttpStatusCode.NotFound;
			}
		}
		finally
		{
			if (res != null)
			{
				res.Close();
			}

		}

		//HTTPステータスチェック
		if (okStatusStrs.Contains(statusCode.ToString()))
		{
			//HTTPステータスが許可ステータスと一致する場合
			//正常なURLと判断
			chkVal = true;


		}
		else
		{
			//HTTPステータスが許可ステータスと一致しない場合
			//異常なURLと判断
			chkVal = false;
		}

		return chkVal;
	}


	/// <summary>
	/// SSL証明のエラーを無視するためのコールバック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="certificate"></param>
	/// <param name="chain"></param>
	/// <param name="sslPolicyErrors"></param>
	/// <returns></returns>
	private static bool validationCallBack(
	object sender,
	X509Certificate certificate,
	X509Chain chain,
	SslPolicyErrors sslPolicyErrors)
	{
		return true;　// true：許可
	}
	
</script>
