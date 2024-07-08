<%--
=========================================================================================================
  Module      : シングルサインオン受入口(Entrance.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
--%>
<%@ Page Language="C#" AutoEventWireup="true" Inherits="BasePage" validateRequest="false" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="w2.Cryptography" %>
<%@ Import Namespace="w2.App.Common.User" %>
<%@ Import Namespace="w2.App.Common.Order" %>
<%@ Import Namespace="w2.Domain.User" %>
<%@ Import Namespace="w2.Domain.UpdateHistory.Helper" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.Net.Security" %>
<%@ Import Namespace="System.Security.Cryptography.X509Certificates" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">
 
	//ここだけ環境ごとに変更
	private const string INTERCOM_SETTINGS_DIR_PATH = @"C:\inetpub\wwwroot\R5044_Kagome.Develop\Web\w2.Commerce.Front\Extern\";
	
	//------------------------------------------------------
	// 各種定数
	//------------------------------------------------------
	//処理区分シングルサインオン
	private const string PROC_TYPE_SSO = "0";
	//処理区分サインオフ
	private const string PROC_TYPE_SSOFF = "2";
	//処理区分のクエリストリングキー
	private const string QUERY_KEY_PROC = "proc";
	//w2ユーザIDのクエリストリングキー
	private const string QUERY_KEY_W2_USER = "w2id";
	//インターコムユーザIDのクエリストリングキー
	private const string QUERY_KEY_IC_USER = "icid";
	//ワンタイムパスワードのクエリストリングキー
	private const string QUERY_KEY_PASS = "pass";
	//次画面URLのクエリストリングキー
	private const string QUERY_KEY_N_URL = "nurl";

	//------------------------------------------------------
	// 設定ファイルからロードするもの
	//------------------------------------------------------	
	//暗号化DLL
	private string ENC_DLL_NAME = "";
	//暗号化インスタンス
	private string ENC_INSTANCE_NAME = "";
	//複合化メソッド
	private string ENC_METHOD_DECRYPT_NAME = "";
	//複合化パス
	private string ENC_METHOD_DECRYPT_PASS = "";
	//戻りURLに飛ばせない場合の遷移先画面
	private string SSO_NG_REDIRECT_PAGE = "";
	//URLチェック時の許可HTTPステータス
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

		ENC_DLL_NAME = IntercomSettings.GetAppStringSetting("ENC_DLL_NAME");
		ENC_INSTANCE_NAME = IntercomSettings.GetAppStringSetting("ENC_INSTANCE_NAME");
		ENC_METHOD_DECRYPT_NAME = IntercomSettings.GetAppStringSetting("ENC_METHOD_DECRYPT_NAME");
		ENC_METHOD_DECRYPT_PASS = IntercomSettings.GetAppStringSetting("ENC_METHOD_DECRYPT_PASS");
		SSO_NG_REDIRECT_PAGE = IntercomSettings.GetAppStringSetting("SSO_NG_REDIRECT_PAGE");
		INTERCOM_PW_REMINDER_OK_STATUS_CD = IntercomSettings.GetAppStringSetting("INTERCOM_PW_REMINDER_OK_STATUS_CD");
	}
	
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void Page_Load(object sender, EventArgs e)
    {
		
		//設定ファイルロード
		Load_IntercomConfig();
		
		//クエリストリングから処理区分
		string procType = this.Context.Request[QUERY_KEY_PROC];

		//------------------------------------------------------
		//暗号化がかかっている処理区分を複合化
		//------------------------------------------------------
		//暗号化ライブラリのロード
		//パス
		string dllpath = Path.GetDirectoryName(Constants.PHYSICALDIRPATH_PLUGINS_STORAGE_LOCATION)
			+ @"\" + ENC_DLL_NAME;
		//アセンブリロード
		Assembly asm = null;
		asm = Assembly.LoadFile(dllpath);
		//複合化インスタンス生成
		object cry = asm.CreateInstance(ENC_INSTANCE_NAME);
		//複合化メソッドのMethodInfo
		MethodInfo mi = cry.GetType().GetMethod(ENC_METHOD_DECRYPT_NAME);
		//クエリストリングから取得した処理区分複合化
		string decprocType = (string)mi.Invoke(cry, new object[] { procType, ENC_METHOD_DECRYPT_PASS });
			
		//処理区分判断
		switch (decprocType)
		{
			case PROC_TYPE_SSO:
				//シングルサインオン
				this.ExecuteSso();
				break;
			case PROC_TYPE_SSOFF:
				//サインオフ
				this.ExecuteSsoff();
				break;
			default:
				//区分が不正な場合
				//指定の画面へ
				//Response.Redirect(Constants.PATH_ROOT);
				Response.Redirect(SSO_NG_REDIRECT_PAGE);
				break;
		}
    }
	
	/// <summary>
	/// サインオン処理
	/// </summary>
	private void ExecuteSso()
	{
		//クエリストリング情報取り出し
		string w2id = Request.QueryString[QUERY_KEY_W2_USER];
		string icid = Request.QueryString[QUERY_KEY_IC_USER];
		string pass = Request.QueryString[QUERY_KEY_PASS];
		string nurl = Request.QueryString[QUERY_KEY_N_URL];

		ExecuteSso(w2id, icid, pass, nurl);
	}
	private void ExecuteSso(string w2id, string icid, string onetimepass, string nurl)
	{
		string nextUrl = nurl; //GetNextUrl(Request);

		//各種複合後の変数
		string decw2id = "";
		string decicid = "";
		string deconetimepass = "";
		string decnurl = "";

		//DBから取得するログインID
		string loginid = "";
		//DBから取得するパスワード
		string password = "";
		//複合化かけたパスワード
		string decpassword = "";
		
		//最終的に遷移する画面のURL
		//初期値はNGの場合の指定画面
		string redirectURL = SSO_NG_REDIRECT_PAGE;
		
		//処理フラグ 初期値False
		//正常に処理がいけばTrueログイン処理のながれへ、NGの場合は戻りURLか指定URLへリダイレクト
		bool procFlag = false;

		try
		{

			//暗号化ライブラリのロード
			//パス
			string dllpath = Path.GetDirectoryName(Constants.PHYSICALDIRPATH_PLUGINS_STORAGE_LOCATION)
				+ @"\" + ENC_DLL_NAME;

			//アセンブリロード
			Assembly asm = null;
			asm = Assembly.LoadFile(dllpath);
			object cry = asm.CreateInstance(ENC_INSTANCE_NAME);

			MethodInfo mi = cry.GetType().GetMethod(ENC_METHOD_DECRYPT_NAME);

			decnurl = (string)mi.Invoke(cry, new object[] { nurl, ENC_METHOD_DECRYPT_PASS });
			decw2id = (string)mi.Invoke(cry, new object[] { w2id, ENC_METHOD_DECRYPT_PASS });
			decicid = (string)mi.Invoke(cry, new object[] { icid, ENC_METHOD_DECRYPT_PASS });
			deconetimepass = (string)mi.Invoke(cry, new object[] { onetimepass, ENC_METHOD_DECRYPT_PASS });

			//ユーザID、ワンタイムパスが正しいか検証のためSQL発行
			string connStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["w2Database"].ToString();

			//実行結果のデータテーブル
			DataTable usertab = new DataTable("usertab");

			//ユーザID、ワンタイムパスワードを条件に、w2_userテーブルを検索
			using (SqlConnection conn = new SqlConnection(connStr))
			{
				//実行SQL
				string sqlStr
					= " SELECT * FROM w2_user "
					+ " WHERE user_id = @w2user_id "	//w2ユーザIDの条件
					+ " AND attribute1 = @icuser_id "	//インターコムユーザIDの条件
					+ " AND attribute2 = @onetimepass "	//ワンタイムパスワードの条件
					+ " AND convert(datetime,attribute3) > getdate() "	//ワンタイムパスワード有効期限（発行から一分以内のみ有効）
					+ " AND attribute4 = @attribute4 ";	//ワンタイムパスワード有効・無効フラグ（一回でも利用すると無効（0）、利用されていなければ有効（1）

				//SqlCommand
				SqlCommand selCmd = new SqlCommand(sqlStr, conn);

				//SettingParam
				selCmd.Parameters.AddWithValue("@w2user_id", decw2id);
				selCmd.Parameters.AddWithValue("@icuser_id", decicid);
				selCmd.Parameters.AddWithValue("@onetimepass", deconetimepass);
				selCmd.Parameters.AddWithValue("@attribute4", '1');

				//ConnectionSet
				selCmd.Connection = conn;

				//Adapter
				SqlDataAdapter adp = new SqlDataAdapter(selCmd);

				//fill
				adp.Fill(usertab);
			}

			//取得件数確認
			if (usertab.Rows.Count == 0)
			{
				//検証失敗のためログインしないでインターコムに戻す
				//戻りURLが取れない場合は指定URLへ戻す
				if (decnurl == null || decnurl == "")
				{
					redirectURL = SSO_NG_REDIRECT_PAGE;
				}
				else
				{
					redirectURL = decnurl;
				}

				//処理フラグFalse
				procFlag = false;
			}
			else
			{
				//上記でユーザ情報がうまく取得できたら対象ユーザのワンタイムパスワード有効フラグを寝かす
				using (SqlConnection conn = new SqlConnection(connStr))
				{
					//実行SQL
					string sqlStr
						= " UPDATE w2_user "
						+ " SET attribute4 = @attribute4 "
						+ " WHERE user_id = @w2user_id "	//w2ユーザIDの条件
						+ " AND attribute1 = @icuser_id ";	//インターコムユーザIDの条件

					//SqlCommand
					SqlCommand updCmd = new SqlCommand(sqlStr, conn);

					//SettingParam
					updCmd.Parameters.AddWithValue("@w2user_id", decw2id);
					updCmd.Parameters.AddWithValue("@icuser_id", decicid);
					updCmd.Parameters.AddWithValue("@attribute4", '0');

					//ConnectionSet
					updCmd.Connection = conn;

					//ConnectionOpen
					updCmd.Connection.Open();

					//Execute
					updCmd.ExecuteNonQuery();


					//ConnectionClose()
					updCmd.Connection.Close();
				}

				//ログインID取り出し
				loginid = usertab.Rows[0]["login_id"].ToString();
				//パスワード取り出し（w2での暗号化かかったまま、連携での暗号ロジックとは別）
				password = usertab.Rows[0]["password"].ToString();


				// ログイン済みの場合はインターコムに戻す
				if (this.IsLoggedIn)
				{
					if (decnurl == null || decnurl == "")
					{
						redirectURL = SSO_NG_REDIRECT_PAGE;
					}
					else
					{
						redirectURL = decnurl;
					}
					
					//処理フラグFalse
					procFlag = false;
				}
				else
				{
					//戻りURLが取れない場合は指定URLへ戻す
					if (decnurl == null || decnurl == "")
					{
						redirectURL = SSO_NG_REDIRECT_PAGE;
						procFlag = false;
					}
					else
					{

						//ログインしていない場合はログイン処理させるためにパスワード複合化

						// 復号化処理
						//ここはw2のログインで利用している通常の暗号・複合ロジックを利用
						Cryptographer crypt = new Cryptographer(Convert.ToBase64String(Constants.ENCRYPTION_USER_PASSWORD_KEY), Convert.ToBase64String(Constants.ENCRYPTION_USER_PASSWORD_IV));
						decpassword = crypt.Decrypt(password);

						//最終的に遷移する画面URLを設定
						redirectURL = decnurl;

						//処理フラグTrue、Trueにするのはここだけ
						procFlag = true;
					}
				}
		
			}
		}
		catch (Exception ex)
		{
			string warnMessage = w2.Common.Logger.FileLogger.CreateExceptionMessage("SingleSignOn時の復号化処理に失敗しました。", ex);
			WriteWarnLog(warnMessage, Request);
			
			//インターコムへ戻す
			if (decnurl == null || decnurl == "")
			{
				//URLが取れなかった場合はデフォルトの場所へ遷移
				redirectURL = SSO_NG_REDIRECT_PAGE;
			}
			else
			{
				//URLがちゃんと取れた場合は指定URLへ遷移
				redirectURL = decnurl;
			}

			//処理フラグFalse
			procFlag = false;
		}

		if (procFlag == true)
		{
			// 全部OKならログイン処理開始
			Login(loginid, decpassword, decnurl);
		}
		else
		{
			//ログイン処理しない場合はリダイレクト
			if (HttpStatusChk(redirectURL))
			{
				//URLが正常なものの場合は指定URLへ
				Response.Redirect(redirectURL);
			}
			else
			{
				//URLが異常なものの場合はデフォルト指定の場所へ遷移
				Response.Redirect(SSO_NG_REDIRECT_PAGE);
			}
			
		}
		
	}
		
	/// <summary>
	/// サインオフ処理
	/// </summary>
	private void ExecuteSsoff()
	{
		//クエリストリング情報取り出し
		//次画面URL
		string nurl = Request.QueryString[QUERY_KEY_N_URL];

		//複合化した次画面URL
		string decnurl = "";

		//------------------------------------------------------
		//暗号化がかかっている文字を複合化
		//------------------------------------------------------
		//暗号化ライブラリのロード
		//パス
		string dllpath = Path.GetDirectoryName(Constants.PHYSICALDIRPATH_PLUGINS_STORAGE_LOCATION)
			+ @"\" + ENC_DLL_NAME;

		//アセンブリロード
		Assembly asm = null;
		asm = Assembly.LoadFile(dllpath);
		//複合化インスタンス生成
		object cry = asm.CreateInstance(ENC_INSTANCE_NAME);
		//複合化メソッドのMethodInfo
		MethodInfo mi = cry.GetType().GetMethod(ENC_METHOD_DECRYPT_NAME);
		//クエリストリングから取り出した次画面URLを複合化
		decnurl = (string)mi.Invoke(cry, new object[] { nurl, ENC_METHOD_DECRYPT_PASS });
		
		//最終的に遷移する画面のURL
		//初期値はNGの場合の指定画面
		string redirectURL = SSO_NG_REDIRECT_PAGE;
		
		//ログイン時のみ
		if (this.IsLoggedIn)
		{	
			try
			{
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
				// セッション張り直しのためのデータ格納（セッションハイジャック対策）
				//------------------------------------------------------
				SessionSecurityManager.SaveSesstionContetnsToDatabaseForChangeSessionId(Request, Response, Session);


				//インターコムへ戻す
				if (decnurl == null || decnurl == "")
				{
					//URLが取れなかった場合は指定ページへ遷移
					redirectURL = SSO_NG_REDIRECT_PAGE;
				}
				else
				{
					//URLがちゃんと取れた場合は指定URLへ遷移
					redirectURL = decnurl;
				}

			}
			catch
			{
				//インターコムへ戻す
				if (decnurl == null || decnurl == "")
				{
					//URLが取れなかった場合は指定ページへ遷移
					redirectURL = SSO_NG_REDIRECT_PAGE;
				}
				else
				{
					//URLがちゃんと取れた場合は指定URLへ遷移
					redirectURL = decnurl;
				}
			}
		}
		else
		{
			//インターコムへ戻す
			if (decnurl == null || decnurl == "")
			{
				//URLが取れなかった場合は指定ページへ遷移
				redirectURL = SSO_NG_REDIRECT_PAGE;
			}
			else
			{
				//URLがちゃんと取れた場合は指定URLへ遷移
				redirectURL = decnurl;
			}
		}

		Response.Redirect(redirectURL);
	}

	/// <summary>
	/// URLの正当性チェック
	/// </summary>
	/// <param name="targetUrl">チェック対象のURL</param>
	/// <returns></returns>
	private bool HttpStatusChk(string targetUrl)
	{
		if (targetUrl == null)
		{
			return false;
		}

		if (targetUrl == "")
		{
			return false;
		}

		HttpWebRequest req;

		try
		{
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
	
	
	/// <summary>
	/// ログイン処理
	/// </summary>
	/// <param name="nextUrl"></param>
	/// <param name="loginId"></param>
	/// <param name="password"></param>
	private void Login(string loginId, string password, string nextUrl)
	{
		// アカウントロックチェック（アカウントロックがされている場合は、エラー画面へ遷移）
		if (LoginAccountLockManager.GetInstance().IsAccountLocked(Request.UserHostAddress, loginId, password))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_LOGIN_ACCOUNT_LOCK);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		var loggedUser = new UserService().TryLogin(loginId, password, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED);
		if (loggedUser != null)
		{
			if (Constants.USER_COOPERATION_ENABLED)
			{
				 // ユーザーログインイベント
			  	var userCooperationPlugin = new UserCooperationPlugin(Constants.FLG_LASTCHANGED_USER);
			  	userCooperationPlugin.Login(loggedUser);
			}

			//ログイン情報をセッションに格納
			SetLoginUserData(loggedUser, UpdateHistoryAction.Insert);

			//次画面へ遷移
			//Response.Redirect(nextUrl);
			
			if (HttpStatusChk(nextUrl))
			{
				Response.Redirect(nextUrl);
			}
			else
			{
				//異常なURLの場合は指定画面へ遷移
				Response.Redirect(SSO_NG_REDIRECT_PAGE);
			}
		}
		else
		{
			//ログイン失敗時はインターコムへ戻す
			//インターコムへ戻す
			if (HttpStatusChk(nextUrl))
			{
				Response.Redirect(nextUrl);
			}
			else
			{
				//異常なURLの場合は指定画面へ遷移
				Response.Redirect(SSO_NG_REDIRECT_PAGE);
			}
				
			
			// ログイン失敗時はエラー画面に遷移
			//Session[Constants.SESSION_KEY_ERROR_MSG] = GetLoginDeniedErrorMessage(loginId, password);

			//Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
	}
	
	/// <summary>
	/// 正当性チェック
	/// </summary>
	/// <param name="request">HTTPリクエスト</param>
	protected new bool Validate(HttpRequest request)
	{
		if (HasValidParam(request) == false) return false;

		if (IsSecureConnection(request) == false) return false;

		if (IsValidReferrer(request.UrlReferrer) == false) return false;

		return true;
	}

	/// <summary>
	/// 有効なパラメタが付与されているか
	/// </summary>
	/// <returns>有効フラグ</returns>
	protected new bool HasValidParam(HttpRequest request)
	{
		if (request[Constants.REQUEST_KEY_SINGLE_SIGN_ON_LOGINID] == null ||
			request[Constants.REQUEST_KEY_SINGLE_SIGN_ON_PASSWORD] == null)
		{
			WriteWarnLog("必要なパラメタが付与されていません。\nゲストユーザである可能性があります。", request);
			return false;
		}

		if (request[Constants.REQUEST_KEY_SINGLE_SIGN_ON_LOGINID] == "" ||
			request[Constants.REQUEST_KEY_SINGLE_SIGN_ON_PASSWORD] == "")
		{
			WriteWarnLog("ログインID、またはパスワードが空の状態でのアクセスです。\nゲストユーザである可能性があります。", request);
		}
		
		return true;
	}

	/// <summary>
	/// HTTPSなアクセスかどうか
	/// </summary>
	/// <returns>セキュアフラグ</returns>
	protected new bool IsSecureConnection(HttpRequest request)
	{
		if (request.Url.AbsoluteUri.StartsWith(Constants.PROTOCOL_HTTPS) == false)
		{
			WriteWarnLog("HTTP(非セキュア通信)でのアクセスがありました。", request);
			return false;
		}
		return true;
	}

	/// <summary>
	/// 有効なリファラかどうか
	/// </summary>
	/// <param name="referrer">リファラ</param>
	/// <returns>有効フラグ</returns>
	protected new bool IsValidReferrer(Uri referrer)
	{
#if DEBUG
		referrer = new Uri("http://localhost/debug");
#endif
		if (referrer == null)
		{
			WriteWarnLog("リファラが空の状態でのアクセスがありました。", Request);
			return false;
		}

		if (Constants.ALLOW_SINGLE_SIGN_ON_URL_REFERRER.Exists(url => url.Host == referrer.Host) == false)
		{
			WriteWarnLog("許可されていないリファラからのアクセスがありました。", Request);
			return false;
		}

		return true;
	}

	/// <summary>
	/// トップページへ遷移
	/// </summary>
	protected new void RedirectTopPage()
	{
		Response.Redirect(Constants.PATH_ROOT, true);
	}

	/// <summary>
	/// 次の遷移先URLを取得
	/// </summary>
	/// <returns>次の遷移先URL</returns>
	protected new string GetNextUrl(HttpRequest request)
	{
		string nextUrl = request[Constants.REQUEST_KEY_NEXT_URL];
		if (nextUrl == null) nextUrl = Constants.PATH_ROOT;
		return nextUrl;
	}

	/// <summary>
	/// 警告のログを出力する
	/// </summary>
	/// <param name="warnActionMessage">警告メッセージ</param>
	/// <param name="request">Httpリクエスト</param>
	protected new void WriteWarnLog(string warnActionMessage, HttpRequest request)
	{
		string warnMessage = warnActionMessage + "\n";
		warnMessage += CreateAccessInfoForLog(request);
		w2.Common.Logger.FileLogger.WriteWarn(warnMessage);
	}

	/// <summary>
	/// ログ出力用アクセス情報を取得
	/// </summary>
	/// <returns>ログ出力用アクセス情報</returns>
	protected new string CreateAccessInfoForLog(HttpRequest request)
	{
		string accessInfo = "\tアクセス情報は以下の通りです\n";
		accessInfo += "\t  HostIP           ： " + request.UserHostAddress + "\n";
		accessInfo += "\t  Riferrer         ： " + request.UrlReferrer + "\n";
		accessInfo += "\t  UserAgent        ： " + request.UserAgent + "\n";
		accessInfo += "\t  EncryptedLoginID ： " + StringUtility.ToEmpty(request[Constants.REQUEST_KEY_SINGLE_SIGN_ON_LOGINID]) + "\n";
		accessInfo += "\t  EncryptedPassword： " + StringUtility.ToEmpty(request[Constants.REQUEST_KEY_SINGLE_SIGN_ON_PASSWORD]) + "\n";
		return accessInfo;
	}
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title></title>
</head>
<body>
	<form id="form1" runat="server">
	<div>

	</div>
	</form>
</body>
</html>


