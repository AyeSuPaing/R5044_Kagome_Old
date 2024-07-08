/*
=========================================================================================================
  Module      : シングルサインオン受入口(Entrance.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using w2.App.Common.User;
using w2.Cryptography;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.User.Helper;

public partial class Form_Entrance : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
		string nextUrl = GetNextUrl(Request);

		// 正当性が確認できない場合、トップページへ遷移
		if (Validate(Request) == false) RedirectTopPage();

		// ログイン済みの場合は指定されたURLへ遷移
		if (this.IsLoggedIn) Response.Redirect(nextUrl);

		string loginId = null;
		string password = null;
		try
		{
			// 復号化処理
			Cryptographer crypt = new Cryptographer(Constants.ENCRYPTION_SINGLE_SIGN_ON_KEY, Constants.ENCRYPTION_SINGLE_SIGN_ON_IV);
			loginId = crypt.Decrypt(Request[Constants.REQUEST_KEY_SINGLE_SIGN_ON_LOGINID]);
			password = crypt.Decrypt(Request[Constants.REQUEST_KEY_SINGLE_SIGN_ON_PASSWORD]);
		}
		catch (Exception ex)
		{
			string warnMessage = w2.Common.Logger.FileLogger.CreateExceptionMessage("SingleSignOn時の復号化処理に失敗しました。", ex);
			WriteWarnLog(warnMessage, Request);
			RedirectTopPage();
		}

		// 全部OKならログイン処理開始（更新履歴とともに）
		Login(loginId, password,nextUrl, UpdateHistoryAction.Insert);
    }

	/// <summary>
	/// ログイン処理
	/// </summary>
	/// <param name="nextUrl">遷移先URL</param>
	/// <param name="loginId">ログインID</param>
	/// <param name="password">パスワード</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	private void Login(string loginId, string password, string nextUrl, UpdateHistoryAction updateHistoryAction)
	{
		// アカウントロックチェック（アカウントロックがされている場合は、エラー画面へ遷移）
		if (LoginAccountLockManager.GetInstance().IsAccountLocked(Request.UserHostAddress, loginId, password))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_LOGIN_ACCOUNT_LOCK);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		var loggedUser = new UserService().TryLogin(loginId, password, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED);
		if (loggedUser == null)
		{
			// ログイン失敗時はエラー画面に遷移
			Session[Constants.SESSION_KEY_ERROR_MSG] = GetLoginDeniedErrorMessage(loginId, password);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// ログイン成功処理＆次の画面へ遷移（ログイン向け）
		ExecLoginSuccessProcessAndGoNextForLogin(loggedUser, nextUrl, false, LoginType.Normal, UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 正当性チェック
	/// </summary>
	/// <param name="request">HTTPリクエスト</param>
	protected bool Validate(HttpRequest request)
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
	protected bool HasValidParam(HttpRequest request)
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
	protected bool IsSecureConnection(HttpRequest request)
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
	protected bool IsValidReferrer(Uri referrer)
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
	protected void RedirectTopPage()
	{
		Response.Redirect(Constants.PATH_ROOT, true);
	}

	/// <summary>
	/// 次の遷移先URLを取得
	/// </summary>
	/// <returns>次の遷移先URL</returns>
	protected string GetNextUrl(HttpRequest request)
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
	protected void WriteWarnLog(string warnActionMessage, HttpRequest request)
	{
		string warnMessage = warnActionMessage + "\n";
		warnMessage += CreateAccessInfoForLog(request);
		w2.Common.Logger.FileLogger.WriteWarn(warnMessage);
	}

	/// <summary>
	/// ログ出力用アクセス情報を取得
	/// </summary>
	/// <returns>ログ出力用アクセス情報</returns>
	protected string CreateAccessInfoForLog(HttpRequest request)
	{
		string accessInfo = "\tアクセス情報は以下の通りです\n";
		accessInfo += "\t  HostIP           ： " + request.UserHostAddress + "\n";
		accessInfo += "\t  Riferrer         ： " + request.UrlReferrer + "\n";
		accessInfo += "\t  UserAgent        ： " + request.UserAgent + "\n";
		accessInfo += "\t  EncryptedLoginID ： " + StringUtility.ToEmpty(request[Constants.REQUEST_KEY_SINGLE_SIGN_ON_LOGINID]) + "\n";
		accessInfo += "\t  EncryptedPassword： " + StringUtility.ToEmpty(request[Constants.REQUEST_KEY_SINGLE_SIGN_ON_PASSWORD]) + "\n";
		return accessInfo;
	}
}