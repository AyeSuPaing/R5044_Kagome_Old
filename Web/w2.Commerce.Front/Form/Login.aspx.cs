/*
=========================================================================================================
  Module      : ログイン画面処理(Login.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web.Services;
using Newtonsoft.Json;
using w2.App.Common.Amazon;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Order.Payment.PayPal;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.App.Common.Auth.RakutenIDConnect;
using w2.App.Common.Line.Util;
using w2.App.Common.User;
using w2.App.Common.User.OMotion;
using w2.App.Common.User.SocialLogin;
using w2.App.Common.User.SocialLogin.Util;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.TempDatas;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using w2.App.Common;
using w2.App.Common.SMS;

public partial class Form_Login : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }    // httpsアクセス

	#region ラップ済コントロール宣言
	WrappedTextBox WtbLoginIdInMailAddr { get { return GetWrappedControl<WrappedTextBox>("tbLoginIdInMailAddr"); } }
	WrappedTextBox WtbLoginId { get { return GetWrappedControl<WrappedTextBox>("tbLoginId"); } }
	WrappedTextBox WtbPassword { get { return GetWrappedControl<WrappedTextBox>("tbPassword"); } }
	WrappedHtmlGenericControl WdvMessages { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvMessages"); } }
	protected WrappedCheckBox WcbAutoCompleteLoginIdFlg { get { return GetWrappedControl<WrappedCheckBox>("cbAutoCompleteLoginIdFlg", false); } }
	protected WrappedHtmlGenericControl WdLoginErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("dLoginErrorMessage"); } }
	protected WrappedHtmlGenericControl Wh2Login { get { return GetWrappedControl<WrappedHtmlGenericControl>("h2Login"); } }
	protected WrappedHtmlGenericControl Wh2SocialLogin { get { return GetWrappedControl<WrappedHtmlGenericControl>("h2SocialLogin"); } }
	protected WrappedHtmlGenericControl WpLogin { get { return GetWrappedControl<WrappedHtmlGenericControl>("pLogin"); } }
	protected WrappedHtmlGenericControl WpSocialLogin { get { return GetWrappedControl<WrappedHtmlGenericControl>("pSocialLogin"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>
	/// ・初期遷移時
	///		→Request[Constants.REQUEST_KEY_LOGIN_ACTION] にConstants.KBN_REQUEST_LOGINPAGE_LOGINACTION が
	///		  設定されていた場合はログイン処理実行。そうでない場合はログインページ表示
	/// </remarks>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// ログインチェック（ログイン済みの場合、トップ画面へ）
		//------------------------------------------------------
		if (this.IsLoggedIn)
		{
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT);
		}

		//------------------------------------------------------
		// HTTPS通信チェック（HTTPのとき、トップ画面へ）
		//------------------------------------------------------
		CheckHttps(this.SecurePageProtocolAndHost + Request.Url.AbsolutePath);

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 遷移先パラメタ取得
			//------------------------------------------------------
			this.NextUrl = NextUrlValidation(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NEXT_URL]));

			// 遷移先補正
			if ((this.NextUrl == "")
				|| (this.NextUrl.IndexOf(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR) != -1)
				|| (this.NextUrl.IndexOf(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_WITHDRAWAL_COMPLETE) != -1))
			{
				this.NextUrl = this.SecurePageProtocolAndHost + Constants.PATH_ROOT;
			}

			if (Constants.RAKUTEN_LOGIN_ENABLED)
			{
				// 楽天IDConnectによる自動ログイン
				ExecRakutenIdConnectLogin(sender, e);
			}

			// 遷移先格納
			Session[Constants.SESSION_KEY_NEXT_URL] = this.NextUrl;

			//------------------------------------------------------
			// ログインページ表示
			//------------------------------------------------------
			// ログインチェック(未ログイン状態でログイン必須ページにアクセス)から遷移してきた場合、エラーメッセージを表示
			this.WdvMessages.Visible = false;
			if (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ERRORPAGE_FRONT_ERRORKBN]) != "")
			{
				this.WdvMessages.Visible = true;
				this.ErrorMessage = WebMessages.GetMessages(Request[Constants.REQUEST_KEY_ERRORPAGE_FRONT_ERRORKBN]);
			}
			else if (Session[Constants.SESSION_KEY_ERROR_MSG_FOR_LOGINPAGE] != null)
			{
				this.WdvMessages.Visible = true;
				this.ErrorMessage = (string)Session[Constants.SESSION_KEY_ERROR_MSG_FOR_LOGINPAGE];

				Session[Constants.SESSION_KEY_ERROR_MSG_FOR_LOGINPAGE] = null;
			}

			// Amazon、PayPalのセッションを破棄する
			if (this.IsAmazonLoggedIn && Constants.AMAZON_LOGIN_OPTION_ENABLED)
			{
				Session.Remove(AmazonConstants.SESSION_KEY_AMAZON_MODEL);
			}
			else if (SessionManager.PayPalLoginResult != null)
			{
				Session.Remove(Constants.SESSION_KEY_PAYPAL_LOGIN_RESULT);
				Session.Remove(Constants.SESSION_KEY_PAYPAL_COOPERATION_INFO);
			}

			// 入力フォームにログインIDに設定
			SetLoginIdToInputForm(UserCookieManager.GetLoginIdFromCookie());

			// Enter押下でサブミット ※FireFoxでは関数内からevent.keyCodeが呼べないらしい
			var onKeypressScript = Constants.OMOTION_ENABLED
				? "if (event.keyCode==13 && onClientClickLogin()){__doPostBack('" + lbLogin.UniqueID + "',''); return false;}"
				: "if (event.keyCode==13){__doPostBack('" + lbLogin.UniqueID + "',''); return false;}";
			this.WtbLoginId.Attributes["onkeypress"] =
				this.WtbPassword.Attributes["onkeypress"] =
				onKeypressScript;

			SessionManager.TemporaryStoreSocialLogin = null;
			// ソーシャルログインの表示制御
			if ((SessionManager.SocialLogin != null) || (String.IsNullOrEmpty(SessionManager.LineProviderUserId) == false)) SocialLoginDisplayControl();

		}

		// Amazonログインボタンペイロード取得
		if (Constants.AMAZON_LOGIN_OPTION_ENABLED && Constants.AMAZON_PAYMENT_CV2_ENABLED)
		{
			var urlCreator = new UrlCreator(Constants.PAGE_FRONT_AMAZON_LOGIN_CALLBACK);
			urlCreator.AddParam(Constants.REQUEST_KEY_NEXT_URL, this.NextUrl);
			this.AmazonRequest = AmazonCv2Redirect.SignPayloadForSignIn(urlCreator.CreateUrl());
		}
	}

	/// <summary>
	/// 入力フォームにログインIDを設定
	/// </summary>
	/// <param name="loginId">Cookieから取得したログインID</param>
	private void SetLoginIdToInputForm(string loginId)
	{
		if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
		{
			this.WtbLoginIdInMailAddr.Text = loginId;
		}
		else
		{
			this.WtbLoginId.Text = loginId;
		}
		this.WcbAutoCompleteLoginIdFlg.Checked = (loginId != "");
	}

	/// <summary>
	/// ログインボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbLogin_Click(object sender, EventArgs e)
	{
		Login(LoginType.Normal);
	}

	/// <summary>
	/// ペイパルログイン完了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPayPalAuthComplete_Click(object sender, System.EventArgs e)
	{
		// ペイパル認証情報をセッションにセット
		var payPalScriptsForm = GetWrappedControl<WrappedPayPalPayScriptsFormControl>("ucPaypalScriptsForm");
		SetPaypalInfoToSession(payPalScriptsForm);

		// ユーザーが見つかればログインさせ、見つからなければ会員登録画面へ
		var user = PayPalUtility.Account.GetUserByPayPalCustomerId(SessionManager.PayPalLoginResult.CustomerId);
		if (user != null)
		{
			// ログイン成功処理実行＆次の画面へ遷移（ログイン向け）
			ExecLoginSuccessProcessAndGoNextForLogin(
				user,
				this.NextUrl,
				false,
				LoginType.PayPal,
				UpdateHistoryAction.Insert);
		}

		// 該当ユーザーが見つからない場合は会員登録画面へ
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_INPUT)
			.AddParam(Constants.REQUEST_KEY_NEXT_URL, this.NextUrl).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 楽天IDConnectリクエストクリック（ログイン）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRakutenIdConnectRequestAuth_Click(object sender, EventArgs e)
	{
		this.Process.RedirectRakutenIdConnect(
			ActionType.Login,
			Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN,
			this.NextUrl);
	}

	/// <summary>
	/// 楽天IDConnectによる自動ログイン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ExecRakutenIdConnectLogin(object sender, EventArgs e)
	{
		// 自動ログインを行う？
		if ((SessionManager.RakutenIdConnectActionInfo != null)
			&& this.IsRakutenIdConnectLogin)
		{
			var user = SessionManager.RakutenIdConnectActionInfo.User;

			if (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED)
			{
				this.WtbLoginIdInMailAddr.Text = user.LoginId;
			}
			else
			{
				this.WtbLoginId.Text = user.LoginId;
			}
			this.WtbPassword.Text = user.PasswordDecrypted;

			// アクション情報削除
			SessionManager.RakutenIdConnectActionInfo = null;

			// ログイン成功処理実行＆次の画面へ遷移（ログイン向け）
			ExecLoginSuccessProcessAndGoNextForLogin(
				user,
				this.NextUrl,
				this.WcbAutoCompleteLoginIdFlg.Checked,
				LoginType.RakutenConnect,
				UpdateHistoryAction.Insert);
		}
	}

	/// <summary>
	/// ログイン処理
	/// </summary>
	/// <param name="loginType">ログイン種別</param>
	private void Login(LoginType loginType)
	{
		// オプション設定によって、ログインIDにセットする値が変わる
		var loginId = Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED
			? this.WtbLoginIdInMailAddr.Text
			: this.WtbLoginId.Text;

		// Check account is locked
		if (LoginAccountLockManager.GetInstance().IsAccountLocked(Request.UserHostAddress, loginId, this.WtbPassword.Text))
		{
			// Set login account locked error message
			var loginAccountLockedErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_LOGIN_ACCOUNT_LOCK);
			this.WdLoginErrorMessage.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(loginAccountLockedErrorMessage);
			return;
		}

		// ログイン判定処理
		var loggedUser = new UserService().TryLogin(loginId, this.WtbPassword.Text, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED);
		if (loggedUser == null)
		{
			// Set login denied error message
			var loginDeniedErrorMessage = GetLoginDeniedErrorMessage(loginId, this.WtbPassword.Text);
			this.WdLoginErrorMessage.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(loginDeniedErrorMessage);
			return;
		}

		// ソーシャルログインID連携
		if (SessionManager.TemporaryStoreSocialLogin != null)
		{
			SessionManager.SocialLogin = SessionManager.TemporaryStoreSocialLogin;
			SocialLoginIdCooperation(loggedUser.UserId);

			// ソーシャルプラス側と連携情報を同期
			SocialLoginUtil.SyncSocialProviderInfo(null, this.LoginUserId);
		}

		//LINE直接連携
		if ((string.IsNullOrEmpty(Constants.SOCIAL_PROVIDER_ID_LINE) == false)
			&& (Constants.SOCIAL_LOGIN_ENABLED == false)
			&& (string.IsNullOrEmpty(SessionManager.LineProviderUserId) == false))
		{
			LineUtil.UpdateUserExtendForLineUserId(loggedUser.UserId,
				SessionManager.LineProviderUserId,
				UpdateHistoryAction.DoNotInsert);
		}

		// ログイン成功処理実行＆次の画面へ遷移（ログイン向け）
		ExecLoginSuccessProcessAndGoNextForLogin(
			loggedUser,
			this.NextUrl,
			this.WcbAutoCompleteLoginIdFlg.Checked,
			loginType,
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// シングルサインオン用のログイン先URLを作成
	/// </summary>
	/// <param name="url">Login先URL</param>
	/// <returns>ログイン先URL(コールバックURL付)</returns>
	protected string CreateLoginPageUrlForSingleSignOn(string url)
	{
		return new UrlCreator(url)
			.AddParam(Constants.REQUEST_KEY_FRONT_NEXT_URL, this.NextUrl)
			.CreateUrl();
	}

	/// <summary>
	/// ソーシャルログインの表示制御
	/// </summary>
	private void SocialLoginDisplayControl()
	{
		this.Wh2Login.Visible = this.WpLogin.Visible = false;
		this.Wh2SocialLogin.Visible = this.WpSocialLogin.Visible = true;
		SessionManager.TemporaryStoreSocialLogin = SessionManager.SocialLogin;
		SessionManager.SocialLogin = null;
	}

	/// <summary>
	/// ソーシャルログインID連携
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	private void SocialLoginIdCooperation(string userId)
	{
		var socialLogin = SessionManager.SocialLogin;
		this.LoginUserId = userId;

		// 既存ユーザーの場合の処理
		var existingUser = SocialLoginUtil.MergeIfExists(this.LoginUserId, socialLogin.SPlusUserId);
		if (existingUser)
		{
			// ユーザーID紐付け
			var authUser = new SocialLoginMap();
			authUser.Exec(Constants.SOCIAL_LOGIN_API_KEY, socialLogin.SPlusUserId, this.LoginUserId, true);
		}
	}

	/// <summary>
	/// Create set omotion client id js script
	/// </summary>
	/// <returns>js script</returns>
	protected string CreateSetOmotionClientIdJsScript()
	{
		var scripts = "SetOmotionClientId("
			+ "'" + (Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? this.WtbLoginIdInMailAddr.ClientID : this.WtbLoginId.ClientID) + "', "
			+ "'" + this.WtbPassword.ClientID + "', "
			+ "'" + this.WdLoginErrorMessage.ClientID + "', "
			+ "'" + GetWrappedControl<WrappedLinkButton>("lbLogin").ClientID + "'"
			+ ");";
		return scripts;
	}

	/// <summary>
	/// Try login
	/// </summary>
	/// <param name="ipAddress">Ip address</param>
	/// <param name="loginId">Login id</param>
	/// <param name="password">Password</param>
	/// <returns>result(user id, message)</returns>
	[WebMethod]
	public static string TryLogin(string ipAddress, string loginId, string password)
	{
		// Check account is locked
		if (LoginAccountLockManager.GetInstance().IsAccountLocked(ipAddress, loginId, password))
		{
			// Set login account locked error message
			var message = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_LOGIN_ACCOUNT_LOCK);
			return JsonConvert.SerializeObject(
				new
				{
					Message = WebSanitizer.HtmlEncodeChangeToBr(message),
					UserId = "",
				});
		}

		// ログイン判定処理
		var loggedUser = new UserService().TryLogin(loginId, password, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED);
		if (loggedUser == null)
		{
			// ログイン試行回数を更新
			LoginAccountLockManager.GetInstance().UpdateLockPossibleTrialLoginCount(ipAddress, loginId, password);
			// ログイン試行可能回数を超えていた場合はログイン試行可能回数エラーのメッセージを設定
			if (LoginAccountLockManager.GetInstance().IsAccountLocked(ipAddress, loginId, password))
			{
				var message = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_LOGIN_ACCOUNT_LOCK);
				return JsonConvert.SerializeObject(
					new
					{
						Message = WebSanitizer.HtmlEncodeChangeToBr(message),
						UserId = "",
					});
			}
			// それ以外の場合はログイン失敗のメッセージを設定
			else
			{
				var message = WebMessages.GetMessages(Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? WebMessages.ERRMSG_FRONT_USER_LOGIN_IN_MAILADDR_ERROR : WebMessages.ERRMSG_FRONT_USER_LOGIN_ERROR);
				return JsonConvert.SerializeObject(
					new
					{
						Message = WebSanitizer.HtmlEncodeChangeToBr(message),
						UserId = "",
					});
			}
		}

		return JsonConvert.SerializeObject(
			new
			{
				Message = "",
				UserId = loggedUser.UserId,
			});
	}

	/// <summary>
	/// Try o-motion
	/// </summary>
	/// <param name="loginId">Login id</param>
	/// <returns>result</returns>
	[WebMethod]
	public static bool TryOmotion(string loginId)
	{
		var authoriId = CookieManager.GetValue(OMotionUtility.GetAuthoriIdCookieKey());
		var omotionService = new OMotionApiService(authoriId, loginId);

		var result = omotionService.Authori();
		omotionService.AuthoriLoginSuccess(result);

		return result;
	}

	/// <summary>
	/// Send o-motion feedback
	/// </summary>
	/// <param name="loginId">Login id</param>
	/// <param name="value">Value</param>
	/// <returns>result</returns>
	[WebMethod]
	public static bool SendOmotionFeedback(string loginId, bool value)
	{
		var authoriId = CookieManager.GetValue(OMotionUtility.GetAuthoriIdCookieKey());
		var omotionService = new OMotionApiService(authoriId, loginId);

		// 成功だったらもう一度送る
		if (value)
		{
			omotionService.AuthoriLoginSuccess(true);
		}

		var result = omotionService.AuthoriFeedback(value ? "OK" : "NG");

		return result;
	}

	/// <summary>
	/// Send authentication code
	/// </summary>
	/// <param name="userId">User id</param>
	/// <param name="isSendMail">Is send mail</param>
	/// <returns></returns>
	[WebMethod]
	public static string SendAuthenticationCode(string userId, bool isSendMail)
	{
		var user = new UserService().Get(userId);
		var authenticationCode = CreateAuthenticationCode();

		// Check if is valid to send authentication code
		var authCodeSendCount = 0;
		var sendFailCount = 0;
		var errorMessage = SMSHelper.CheckBeforeSendAuthenticationCode(
			userId,
			userId,
			ref sendFailCount,
			ref authCodeSendCount);

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			return JsonConvert.SerializeObject(
				new
				{
					Result = false,
					Message = errorMessage,
					AuthenticationCodeReceiver = string.Empty,
				});
		}

		var authenticationCodeReceiver = string.Empty;

		if (isSendMail)
		{
			var mailData = new Hashtable
			{
				{ Constants.CONST_AUTHENTICATION_CODE, authenticationCode },
			};

			// Save authentication code
			new TempDatasService().Save(
				TempDatasService.TempType.AuthCode,
				userId,
				authenticationCode);

			SendMail(user.MailAddr, mailData);

			authenticationCodeReceiver = CreateMailAddrMasked(user.MailAddr);
		}
		else
		{
			var userAddressCountryIsoCode = Constants.GLOBAL_OPTION_ENABLE
				? user.AddrCountryIsoCode
				: Constants.COUNTRY_ISO_CODE_JP;
			var phoneNumber = SMSHelper.GetSmsToPhoneNumber(user.Tel1, userAddressCountryIsoCode);
			if (string.IsNullOrEmpty(phoneNumber))
			{
				return JsonConvert.SerializeObject(
					new
					{
						Result = false,
						Message = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_NOT_EXISTS_AUTHENTICATION_CODE_RECEIVER),
						AuthenticationCodeReceiver = authenticationCodeReceiver,
					});
			}

			// Send sms
			var tempDatasService = new TempDatasService();
			var isSendSmsSuccess = SMSHelper.SendSmsAuthenticationCode(authenticationCode, phoneNumber);

			if (isSendSmsSuccess == false)
			{
				return JsonConvert.SerializeObject(
					new
					{
						Result = false,
						Message = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_NOT_EXISTS_AUTHENTICATION_CODE_RECEIVER),
						AuthenticationCodeReceiver = authenticationCodeReceiver,
					});
			}

			// Save authentication code
			tempDatasService.Save(
				TempDatasService.TempType.AuthCode,
				userId,
				authenticationCode);

			authenticationCodeReceiver = CreateTelMasked(phoneNumber, user.Tel1);
		}

		return JsonConvert.SerializeObject(
			new
			{
				Result = true,
				Message = string.Empty,
				AuthenticationCodeReceiver = authenticationCodeReceiver,
			});
	}

	/// <summary>
	/// Create tel masked
	/// </summary>
	/// <param name="sendTel">Send tel</param>
	/// <param name="userTel">User tel</param>
	/// <returns>Tel masked</returns>
	private static string CreateTelMasked(string sendTel, string userTel)
	{
		var telCountryNumber = sendTel.Replace(userTel.Replace("-", "").Substring(1), "");
		var telLast = (sendTel.Length > 4) ? sendTel.Substring(sendTel.Length - 4, 4) : sendTel;
		return string.Format("+{0}{1}", telCountryNumber, telLast.PadLeft(sendTel.Length - telCountryNumber.Length, '*'));
	}

	/// <summary>
	/// Create mail addr masked
	/// </summary>
	/// <param name="mailAddr">Mail addr</param>
	/// <returns>Mail addr masked</returns>
	private static string CreateMailAddrMasked(string mailAddr)
	{
		var pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";
		var result = Regex.Replace(mailAddr, pattern, m => new string('*', m.Length));
		return result;
	}

	/// <summary>
	/// Create authentication code
	/// </summary>
	/// <returns>Authentication code</returns>
	private static string CreateAuthenticationCode()
	{
		var random = new Random();
		var authCode = new string(Enumerable.Repeat("0123456789", 6)
			.Select(item => item[random.Next(item.Length)])
			.ToArray());

		return authCode;
	}

	/// <summary>
	/// Send mail
	/// </summary>
	/// <param name="mailAddr">Mail addr</param>
	/// <param name="mailData">Mail data</param>
	private static void SendMail(string mailAddr, Hashtable mailData)
	{
		using (var mailSend = new MailSendUtility(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.CONST_MAIL_ID_SEND_SMS_AUTHENTICATION,
			string.Empty,
			mailData,
			true,
			Constants.MailSendMethod.Auto,
			userMailAddress: StringUtility.ToEmpty(mailAddr)))
		{
			mailSend.AddTo(StringUtility.ToEmpty(mailAddr));
			mailSend.SendMail();
		}
	}

	/// <summary>
	/// Check authentication code
	/// </summary>
	/// <param name="userId">User id</param>
	/// <param name="authenticationCode">Authentication code</param>
	/// <returns>result</returns>
	[WebMethod]
	public static string CheckAuthenticationCode(string userId, string authenticationCode)
	{
		var message = string.Empty;
		var checkAuthenticationCode = SMSHelper.CheckAuthenticationCode(
			authenticationCode,
			userId,
			userId,
			ref message);

		var result = (checkAuthenticationCode == Constants.FLG_AUTHENTICATION_RESULT_SUCCSESS);

		return JsonConvert.SerializeObject(
			new
			{
				Result = result,
				Message = message,
			});
	}

	/// <summary>楽天IDConnectによるログイン？</summary>
	private bool IsRakutenIdConnectLogin
	{
		get { return (Request[Constants.REQUEST_KEY_RAKUTEN_ID_CONNECT_LOGIN] == "1"); }
	}
	/// <summary>次ページURL</summary>
	protected string NextUrl
	{
		get { return (string)ViewState["NextUrl"]; }
		set { ViewState["NextUrl"] = value; }
	}
	/// <summary>エラーメッセージ</summary>
	protected string ErrorMessage { get; set; }
	/// <summary>アマゾンリクエスト</summary>
	protected AmazonCv2Redirect AmazonRequest { get; set; }
}
