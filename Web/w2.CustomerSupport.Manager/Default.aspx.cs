/*
=========================================================================================================
  Module      : ログインページ(Default.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Text;
using System.Web.Services;
using w2.App.Common.Cs.CsOperator;
using w2.App.Common.Manager;
using w2.App.Common.Manager.Menu;
using w2.App.Common.OperationLog;
using w2.Common.Web;
using w2.Domain;

/// <summary>
/// ログインページ
/// </summary>
public partial class Default : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// ログイン済みでNextUrlあるなら遷移
			if (string.IsNullOrEmpty(this.LoginOperatorId) == false) RedirectToNextUrl();

			// ログイン？
			if (Request[Constants.REQUEST_KEY_MANAGER_LOGIN_FLG] == "1")
			{
				// 各パラメタ取得取得
				GetParameters();

				// ログイン実行（エラーメッセージはログイン画面出力）
				LoginAction();
			}
			else
			{
				// セッション初期化
				InitSession();

				// Showing error message when login session expired
				if (this.IsLoginExpiredFlgOn)
				{
					spErrorMessage.InnerHtml = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_LOGIN_SESSION_EXPIRED));
				}
			}
		}
	}

	/// <summary>
	/// セッション初期化
	/// </summary>
	private void InitSession()
	{
		// オペレータログオフ
		Session.Clear();
		CookieManager.RemoveCookie(Constants.REQUEST_KEY_COOKIE_NAME_EC, Constants.REQUEST_REMOVE_COOKIE_PATH);
		CookieManager.RemoveCookie(Constants.REQUEST_KEY_COOKIE_NAME_MP, Constants.REQUEST_REMOVE_COOKIE_PATH);
		CookieManager.RemoveCookie(Constants.REQUEST_KEY_COOKIE_NAME_CS, Constants.REQUEST_REMOVE_COOKIE_PATH);
		CookieManager.RemoveCookie(Constants.REQUEST_KEY_COOKIE_NAME_CMS, Constants.REQUEST_REMOVE_COOKIE_PATH);
	}

	/// <summary>
	/// ログイン実行（エラーメッセージはログイン画面出力）
	/// </summary>
	private void LoginAction()
	{
		// ログイン？
		if (Request[Constants.REQUEST_KEY_MANAGER_LOGIN_FLG] == "1")
		{
			// 共通エラーページはテンプレートを含んでいるため表示したくない。
			// そのため、try～catchで囲んでエラー集約ハンドラへ渡さないようにする
			try
			{
				var loginManager = new ShopOperatorLoginManager();

				// ログイン実行
				var loginResult = loginManager.TryLogin(this.ShopId, this.LoginId, this.Password);

				// ログイン成功
				if (loginResult.IsSuccess)
				{
					loginResult.MenuAccessInfo = new OperatorMenuManager().GetOperatorMenuList(
						Constants.ManagerSiteType,
						loginResult.ShopOperator,
						ManagerMenuCache.Instance);

					// メニュー権限なしの場合
					if (loginResult.MenuAccessInfo.LargeMenus.Length == 0)
					{
						// エラー文言を画面へ表示
						spErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_LOGIN_UNACCESSABLEUSER_ERROR);
					}
					else
					{
						// ShopOperator格納
						Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR] = loginResult.ShopOperator;
						// オペレータメニューをセッションへ格納
						Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU] = loginResult.MenuAccessInfo.LargeMenus.ToList();
						// オペレータメニュー権限をセッションへ格納
						Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU_ACCESS_LEVEL] =
							loginResult.ShopOperator.GetMenuAccessLevel(Constants.ManagerSiteType);

						// 表示件数更新（同期処理）
						MenuTaskCountManager.RefreshAllCount(this.LoginOperatorDeptId, this.LoginOperatorId);

						// CS権限情報取得
						var operatorGroupService = new CsOperatorGroupService(new CsOperatorGroupRepository());
						var groups = operatorGroupService.GetGroups(this.LoginOperatorDeptId, this.LoginOperatorId);
						this.LoginOperatorCsGroupIds = (from g in groups select g.CsGroupId).ToArray();
						var csOperatorService = new CsOperatorService(new CsOperatorRepository());
						var csOperator = csOperatorService.Get(this.LoginOperatorDeptId, this.LoginOperatorId);
						this.LoginOperatorCsInfo = csOperator;

						DomainFacade.Instance.ShopOperatorService.UpdateRemoteAddress(
							this.ShopId,
							this.LoginId,
							loginManager.GetIpAddress());

						loginManager.WriteLoginLog(this.LoginId, OperationKbn.SUCCESS);

						// 次URLがあれば遷移
						if (string.IsNullOrEmpty(this.NextUrl) == false)
						{
							RedirectToNextUrlForLoginExpired();
						}

						// ログインオペレーターがスーパーユーザー、またはトップページへのアクセス権限がある場合
						if (loginResult.ShopOperator.IsSuperUser(Constants.ManagerSiteType)
							|| loginResult.MenuAccessInfo.LargeMenus.Any(
								lagerMenu => lagerMenu.SmallMenus.Any(
									smallMenu => smallMenu.TopPage.Equals(Constants.PAGE_W2CS_MANAGER_TOP_PAGE_PAGE))))
						{
							// トップメニューへ遷移（念のためHTTPS遷移を行う。）
							// サポートサイト有効の場合はサポート情報、無効の場合はオペレーションメニューへ
							StringBuilder topUrl = new StringBuilder(
								Constants.PROTOCOL_HTTPS + Request.Url.Authority + Constants.PATH_ROOT);
							topUrl.Append(
								Constants.COOPERATION_SUPPORT_SITE
									? Constants.PAGE_MANAGER_SUPPORT_INFORMATION
									: Constants.PAGE_W2CS_MANAGER_TOP_PAGE);
							if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_CUSTOMER_TELNO]) == false)
							{
								topUrl.Append("?").Append(Constants.REQUEST_KEY_CUSTOMER_TELNO).Append("=")
									.Append(this.CustomerTelNo);
							}

							Response.Redirect(topUrl.ToString());
						}
						else
						{
							foreach (MenuLarge ml in loginResult.MenuAccessInfo.LargeMenus)
							{
								foreach (MenuSmall ms in ml.SmallMenus)
								{
									if (ms.IsAuthorityDefaultDispPage)
									{
										Response.Redirect(Constants.PROTOCOL_HTTPS + Request.Url.Authority + ms.Href);
									}
								}
							}
						}
					}
				}
				// 有効フラグが無効の場合
				else if (loginResult.IsErrorLoginCountLimit)
				{
					// エラー文言を画面へ表示
					spErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_LOGIN_LIMITED_COUNT_ERROR);
				}
				else
				{
					// エラー文言を画面へ表示
					var errorMessage = loginManager.IncreaseLoginErrorCount(this.LoginId);
					spErrorMessage.InnerHtml = string.IsNullOrEmpty(errorMessage)
						? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHOP_OPERATOR_LOGIN_ERROR)
						: errorMessage;
				}
			}
			catch (System.Threading.ThreadAbortException)
			{
				// なにもしない
			}
#if !DEBUG
			catch (Exception ex)
			{
				// エラー文言を画面へ表示
				spErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);

				// ログ出力
				AppLogger.WriteError(ex);
			}
#endif
		}
	}

	/// <summary>
	/// パラメタ取得
	/// </summary>
	private void GetParameters()
	{
		this.ShopId = Constants.CONST_DEFAULT_SHOP_ID;
		this.LoginId = Request[Constants.REQUEST_KEY_MANAGER_LOGIN_ID];
		this.Password = Request[Constants.REQUEST_KEY_MANAGER_PASSWORD];
		this.NextUrl = Request[Constants.REQUEST_KEY_MANAGER_NEXTURL];
		this.CustomerTelNo = Request[Constants.REQUEST_KEY_CUSTOMER_TELNO];

		// 他サイトへ遷移しようとしていたらURL補正（オープンリダイレクトアタック考慮）
		this.NextUrl = UrlValidator.GetAltUrlIfOtherHostUsed(Request.Url.Host, this.NextUrl, null);
	}

	#region -RedirectToNextUrl 次URL遷移
	/// <summary>
	/// 次URL遷移
	/// </summary>
	private void RedirectToNextUrl()
	{
		string nextUrl = Request[Constants.REQUEST_KEY_MANAGER_NEXTURL];
		if ((nextUrl != null) && (nextUrl.StartsWith("/") == false)) nextUrl = null;	// フィッシング防止のため/開始のみ許可
		if (string.IsNullOrEmpty(nextUrl) == false) Response.Redirect(nextUrl);
	}
	#endregion

	/// <summary>
	/// Redirect to next url for login expired
	/// </summary>
	private void RedirectToNextUrlForLoginExpired()
	{
		if ((this.IsLoginExpiredFlgOn == false)
			|| IsSingleSignOnUrl(this.NextUrl)
			|| ManagerMenuCache.Instance.IsSmallMenu(this.NextUrl, this.LoginOperatorMenu))
		{
			Response.Redirect(this.NextUrl);
		}
	}

	/// <summary>
	/// Is single sign on url
	/// </summary>
	/// <param name="nextUrl">Next url</param>
	/// <returns>True: If this next url is single sign on url</returns>
	private bool IsSingleSignOnUrl(string nextUrl)
	{
		var result = nextUrl.Contains(Constants.PAGE_MANAGER_WEBFORMS_SINGLE_SIGN_ON);
		return result;
	}

	/// <summary>
	/// Login
	/// </summary>
	/// <param name="loginId">ログインID</param>
	/// <param name="password">パスワード</param>
	/// <returns>A status code or error message</returns>
	[WebMethod]
	public static string Login(string loginId, string password)
	{
		var result = new ShopOperatorLoginManager().LoginWith2StepAuthentication(loginId, password);
		return result;
	}

	/// <summary>
	/// Authentication
	/// </summary>
	/// <param name="loginId">ログインID</param>
	/// <param name="authenticationCode">認証コード</param>
	/// <returns>A status code or error message</returns>
	[WebMethod]
	public static string Authentication(string loginId, string authenticationCode)
	{
		var result = new ShopOperatorLoginManager().CheckAuthenticationCode(
			loginId,
			authenticationCode);
		return result;
	}

	/// <summary>
	/// Resend code
	/// </summary>
	/// <param name="loginId">ログインID</param>
	/// <returns>An error message or empty string.</returns>
	[WebMethod]
	public static string ResendCode(string loginId)
	{
		var result = new ShopOperatorLoginManager().ResendCode(loginId);
		return result;
	}

	/// <summary>店舗ID</summary>
	private string ShopId { get; set; }
	/// <summary>ログインID</summary>
	private string LoginId { get; set; }
	/// <summary>パスワード</summary>
	private string Password { get; set; }
	/// <summary>遷移先URL</summary>
	private string NextUrl { get; set; }
	/// <summary>顧客の電話番号</summary>
	private string CustomerTelNo { get; set; }
	/// <summary>Is login expired flag on</summary>
	private bool IsLoginExpiredFlgOn
	{
		get
		{
			return (Request[Constants.REQUEST_KEY_MANAGER_LOGIN_EXPIRED_FLG] == Constants.REQUEST_KEY_MANAGER_LOGIN_EXPIRED_FLG_VALID);
		}
	}
}
