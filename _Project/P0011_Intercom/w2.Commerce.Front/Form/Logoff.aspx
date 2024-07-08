<%--
=========================================================================================================
  Module      : ログオフ画面(Logoff.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page language="c#" Inherits="Form_Logoff" CodeFile="~/Form/Logoff.aspx.cs" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="w2.Cryptography" %>
<%@ Import Namespace="w2.Domain.User" %>
<script runat="server">

	//ここだけ環境ごとに変更
	private const string INTERCOM_SETTINGS_DIR_PATH = @"C:\inetpub\wwwroot\R5044_Kagome.Develop\Web\w2.Commerce.Front\Extern\";
	private const string QKEY_MAIL = "ml";
	private const string QKEY_P = "p";
	private const string QKEY_BURL = "bu";
	private const string QKEY_KBN = "mt";
	

	//------------------------------------------------------
	// 設定ファイルからロードするもの
	//------------------------------------------------------	
	//暗号化DLL
	private string ENC_DLL_NAME = "";
	//暗号化インスタンス
	private string ENC_INSTANCE_NAME = "";
	//複合化メソッド
	private string ENC_METHOD_ENCRYPT_NAME = "";
	//複合化パス
	private string ENC_METHOD_ENCRYPT_PASS = "";
	//インターコムシングルサインオン画面
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

		ENC_DLL_NAME = IntercomSettings.GetAppStringSetting("ENC_DLL_NAME");
		ENC_INSTANCE_NAME = IntercomSettings.GetAppStringSetting("ENC_INSTANCE_NAME");
		ENC_METHOD_ENCRYPT_NAME = IntercomSettings.GetAppStringSetting("ENC_METHOD_ENCRYPT_NAME");
		ENC_METHOD_ENCRYPT_PASS = IntercomSettings.GetAppStringSetting("ENC_METHOD_ENCRYPT_PASS");
		ASPX_IC_SSO_URL = IntercomSettings.GetAppStringSetting("ASPX_IC_SSO_URL");
	}
	
	///=============================================================================================
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	///=============================================================================================
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (this.IsLoggedIn == false)
		{
			Response.Redirect(Constants.PATH_ROOT);
		}
		//設定ファイルロード
		Load_IntercomConfig();
		
		//ユーザIDとパスワードとっとく
		string userid = this.LoginUserId;
		string pass = new UserService().Get(this.LoginUserId).Password;
						
		//------------------------------------------------------
		// カート情報削除
		//------------------------------------------------------
		// ユーザカートリスト情報取得
		CartObjectList colUserCartList = CartObjectList.GetUserCartList(this.LoginUserId, Constants.FLG_ORDER_ORDER_KBN_PC);

		//------------------------------------------------------
		// ユーザログオフ
		//------------------------------------------------------
		// セッションクリア
		Session.Contents.RemoveAll();

		//------------------------------------------------------
		// 次のページの制御
		//------------------------------------------------------
		string strNextUrl = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NEXT_URL]);
		if (strNextUrl.Length == 0)
		{
			strNextUrl = this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT;
		}

		//------------------------------------------------------
		// セッション張り直しのためのデータ格納（セッションハイジャック対策）
		//------------------------------------------------------
		SessionSecurityManager.SaveSesstionContetnsToDatabaseForChangeSessionId(Request, Response, Session);

		//------------------------------------------------------
		// 元のページへリダイレクト（セッション復元）
		//------------------------------------------------------
		//Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_RESTORE_SESSION
		//	+ "?" + Constants.REQUEST_KEY_NEXT_URL + "=" + Server.UrlEncode(strNextUrl));

		//------------------------------------------------------
		//クエリストリングを暗号化してURL作成
		//------------------------------------------------------

		string ssourl = "";
			
		try
		{

			//パスワード複合化
			Cryptographer crypt = new Cryptographer(Convert.ToBase64String(Constants.ENCRYPTION_USER_PASSWORD_KEY)
				, Convert.ToBase64String(Constants.ENCRYPTION_USER_PASSWORD_IV));
			string decpass = crypt.Decrypt(pass);

			//暗号化ライブラリのロード
			//パス
			string dllpath = Path.GetDirectoryName(Constants.PHYSICALDIRPATH_PLUGINS_STORAGE_LOCATION)
				+ @"\" + ENC_DLL_NAME;
			//アセンブリロード
			Assembly asm = null;
			asm = Assembly.LoadFile(dllpath);
			//暗号化インスタンス生成
			object cry = asm.CreateInstance(ENC_INSTANCE_NAME);
			//暗号化メソッドのMethodInfo
			MethodInfo mi = cry.GetType().GetMethod(ENC_METHOD_ENCRYPT_NAME);
			//クエリストリングに渡すものを暗号化
			string encUserID = (string)mi.Invoke(cry, new object[] { userid, ENC_METHOD_ENCRYPT_PASS });
			string encpass = (string)mi.Invoke(cry, new object[] { decpass, ENC_METHOD_ENCRYPT_PASS });
			string encurl = (string)mi.Invoke(cry, new object[] { this.SecurePageProtocolAndHost + Constants.PATH_ROOT, ENC_METHOD_ENCRYPT_PASS });
			string enckbn = (string)mi.Invoke(cry, new object[] { "1", ENC_METHOD_ENCRYPT_PASS });

			//クエリストリングを暗号化してURL作成
			ssourl = ASPX_IC_SSO_URL
				+ "?" + QKEY_MAIL + "=" + HttpUtility.UrlEncode(encUserID)
				+ "&" + QKEY_P + "=" + HttpUtility.UrlEncode(encpass)
				+ "&" + QKEY_BURL + "=" + HttpUtility.UrlEncode(encurl)
				+ "&" + QKEY_KBN + "=" + HttpUtility.UrlEncode(enckbn);

			asm = null;
		
		}
		catch(Exception ex)
		{ 
			//エラーの場合はTOP表示
			Response.Redirect(Constants.PATH_ROOT);
		}

		//テスト用画面
		Response.Redirect(ssourl);
	}
</script>

