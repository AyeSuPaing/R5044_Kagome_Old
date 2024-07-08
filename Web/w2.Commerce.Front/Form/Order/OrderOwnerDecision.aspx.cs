/*
=========================================================================================================
  Module      : 注文者決定画面処理(OrderOwnerDecision.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common.Amazon.Util;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Auth.RakutenIDConnect;
using w2.App.Common.Order;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.User;
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserDefaultOrderSetting;

public partial class Form_Order_OrderOwnerDecision : OrderPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	#region ラップ済みコントロール宣言
	WrappedTextBox WtbLoginIdInMailAddr { get { return GetWrappedControl<WrappedTextBox>("tbLoginIdInMailAddr"); } }
	WrappedTextBox WtbLoginId { get { return GetWrappedControl<WrappedTextBox>("tbLoginId", ""); } }
	WrappedTextBox WtbPassword { get { return GetWrappedControl<WrappedTextBox>("tbPassword", ""); } }
	WrappedHtmlGenericControl WdLoginErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("dLoginErrorMessage", ""); } }
	protected WrappedCheckBox WcbAutoCompleteLoginIdFlg { get { return GetWrappedControl<WrappedCheckBox>("cbAutoCompleteLoginIdFlg", false); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);

			//プレビュー時、AmazonRequestが初期化されず例外が発生するのを防ぐ
			this.AmazonRequest = AmazonCv2Redirect.SignPayloadForOneTime();
			return;
		}

		// HTTPS通信チェック（HTTP通信の場合、ショッピングカートへ）
		CheckHttps();

		// カート存在チェック（カートが存在しない場合、エラーページへ）
		CheckCartExists();

		// Amazonボタン初期化
		InitAmazonButton();

		if (!IsPostBack)
		{
			// 画面遷移の正当性チェック
			CheckOrderUrlSession();

			// ユーザセッションチェック（ログイン済みの場合は配送先指定へ）
			if (this.IsLoggedIn)
			{
				var userDefaultOrderSetting = Constants.TWOCLICK_OPTION_ENABLE ? new UserDefaultOrderSettingService().Get(this.LoginUserId) : null;

				// 注文同梱対象がある場合、注文同梱選択画面へ遷移
				if (Constants.ORDER_COMBINE_OPTION_ENABLED && OrderCombineUtility.ExistCombinableOrder(this.LoginUserId, this.CartList, true))
				{
					// 画面遷移の正当性チェックのため遷移先ページURLを設定
					Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_COMBINE_SELECT_LIST;

					// 画面遷移
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_COMBINE_SELECT_LIST);
				}
				// デフォルト注文方法がある場合、デフォルト注文方法をカートに設定し画面遷移
				else if (userDefaultOrderSetting != null)
				{
					new UserDefaultOrderManager(this.CartList, userDefaultOrderSetting, this.IsAddRecmendItem)
						.SetNextPageAndDefaultOrderSetting(false);

					this.Process.SelectedShippingMethod = (this.CartList.Items.Count != 0)
						? this.CartList.Items[0].Shippings.Select(x => x.ShippingMethod).ToArray()
						: null;

					Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = this.CartList.CartNextPage;
					Response.Redirect(Constants.PATH_ROOT + this.CartList.CartNextPage);
				}
				else
				{
					if (Constants.NOVELTY_OPTION_ENABLED)
					{
						// カートノベルティリスト作成
						var cartNoveltyList = new CartNoveltyList(this.CartList);

						// Add Product Grant Novelty
						var cartItemCountBefore = this.CartList.Items.Count;
						AddProductGrantNovelty(cartNoveltyList);

						// For case add Novelty to other cart
						if (cartItemCountBefore != this.CartList.Items.Count) cartNoveltyList = new CartNoveltyList(this.CartList);

						// 付与条件外のカート内の付与アイテムを削除
						this.CartList.RemoveNoveltyGrantItem(cartNoveltyList);
						// カートに追加された付与アイテムを含むカートノベルティを削除
						cartNoveltyList.RemoveCartNovelty(this.CartList);
					}

					// 画面遷移の正当性チェックのため遷移先ページURLを設定
					Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;

					// 画面遷移
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING);
				}
			}

			// 入力フォームにログインIDに設定
			SetLoginIdToInputForm(UserCookieManager.GetLoginIdFromCookie());

			if (Constants.SOCIAL_LOGIN_ENABLED)
			{
				var socialLogin = (SocialLoginModel)Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL];
				// 遷移元ページがコールバックページの場合は連携したとみなす
				if ((socialLogin != null)
					&& (socialLogin.TransitionSourcePath == ("~/" + Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK)))
				{
					Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;
				}
			}

			// コンポーネント初期化
			InitComponents();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitComponents()
	{
		//------------------------------------------------------
		// ログインID／パスワードはEnterでログイン
		//------------------------------------------------------
		// Enter押下でサブミット ※FireFoxでは関数内からevent.keyCodeが呼べないらしい
		var strOnKeypress = Constants.OMOTION_ENABLED
			? "if (event.keyCode==13 && onClientClickLogin()){__doPostBack('" + lbLogin.UniqueID + "',''); return false;}"
			: "if (event.keyCode==13){__doPostBack('" + lbLogin.UniqueID + "',''); return false;}";

		this.WtbLoginId.Attributes["onkeypress"] = strOnKeypress;
		this.WtbPassword.Attributes["onkeypress"] = strOnKeypress;
	}

	/// <summary>
	/// Amazonボタン初期化
	/// </summary>
	private void InitAmazonButton()
	{
		if (this.IsVisibleAmazonLoginButton)
		{
			this.AmazonRequest = AmazonCv2Redirect.SignPayloadForSignIn();
		}

		if (this.IsVisibleAmazonPayButton)
		{
			this.AmazonRequest = this.CartList.HasFixedPurchase
				? AmazonCv2Redirect.SignPayloadForReccuring(this.CartList.PriceCartListTotal)
				: AmazonCv2Redirect.SignPayloadForOneTime();
		}
	}

	/// <summary>
	/// 次へリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbOrderShipping_Click(object sender, EventArgs e)
	{
		if (Constants.RAKUTEN_LOGIN_ENABLED)
		{
			// 楽天IDConnectアクション情報のセッション削除
			SessionManager.RakutenIdConnectActionInfo = null;
		}

		// 新しいセッションを生成し、配送先入力画面へ遷移
		RedirectToOrderShippingWithNewSession();
	}

	/// <summary>
	/// ログインボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbLogin_Click(object sender, EventArgs e)
	{
		string loginId = Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED
			? this.WtbLoginIdInMailAddr.Text
			: this.WtbLoginId.Text;
		string password = this.WtbPassword.Text;

		// Check account is locked
		if (LoginAccountLockManager.GetInstance().IsAccountLocked(Request.UserHostAddress, loginId, password))
		{
			// Set login account locked error message
			var loginAccountLockedErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_LOGIN_ACCOUNT_LOCK);
			this.WdLoginErrorMessage.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(loginAccountLockedErrorMessage);
			return;
		}

		// ログイン処理
		var loggedUser = new UserService().TryLogin(loginId, password, Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED);
		if (loggedUser == null)
		{
			// Set login denied error message
			var loginDeniedErrorMessage = GetLoginDeniedErrorMessage(loginId, password);
			this.WdLoginErrorMessage.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(loginDeniedErrorMessage);
			return;
		}

		// ログイン成功処理実行＆次の画面へ遷移（ログイン向け）
		ExecLoginSuccessProcessAndGoNextForLogin(
			loggedUser,
			this.RawUrl,
			this.WcbAutoCompleteLoginIdFlg.Checked,
			this.IsRakutenIdConnectLogin ? LoginType.RakutenConnect : LoginType.Normal,	// ★
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUserRegist_Click(object sender, EventArgs e)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_USER_REGIST_REGULATION);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_NEXT_URL).Append("=").Append(HttpUtility.UrlEncode(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING));

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// かんたん会員登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUserEasyRegist_Click(object sender, EventArgs e)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_EASY_REGIST_INPUT)
			.AddParam(Constants.REQUEST_KEY_NEXT_URL, Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_OWNER_DECISION)
			.CreateUrl();

		Response.Redirect(url);
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

		// ユーザーが見つかればログインさせ、見つからなければ同じ画面にリダイレクト
		var user = PayPalUtility.Account.GetUserByPayPalCustomerId(SessionManager.PayPalLoginResult.CustomerId);
		if (user != null)
		{
			// ログイン成功処理実行＆次の画面へ遷移（ログイン向け）
			ExecLoginSuccessProcessAndGoNextForLogin(
				user,
				this.RawUrl,
				false,
				LoginType.PayPal,
				UpdateHistoryAction.Insert);
		}

		// 該当ユーザーが見つからない場合は会員登録画面へ
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_REGULATION)
			.AddParam(Constants.REQUEST_KEY_NEXT_URL, Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// エラー画面遷移
	/// </summary>
	/// <param name="errorMessage">エラーメッセージキー</param>
	/// <param name="backUrl">戻りURL</param>
	private void RedirectErrorPage(string errorMessage, string backUrl)
	{
		Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;

		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);

		// 戻りURLがnull以外の場合、戻りURLを遷移用URLに付与
		//（エラー画面から戻った際にキャッシュが削除されていて表示出来ない可能性がある為、明示的に戻りURLを設定）
		if (backUrl != null)
		{
			sbUrl.Append("?").Append(Constants.REQUEST_KEY_BACK_URL).Append("=").Append(HttpUtility.UrlEncode(backUrl));
		}

		Response.Redirect(sbUrl.ToString());
	}

	/// <summary>
	/// 入力フォームにログインIDに設定
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
	/// 楽天IDConnectリクエストクリック（ログイン）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRakutenIdConnectRequestAuth_Click(object sender, EventArgs e)
	{
		var nextUrl = NextUrlValidation(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_OWNER_DECISION);
		var beforeUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_OWNER_DECISION;
		this.Process.RedirectRakutenIdConnect(ActionType.Login, nextUrl, beforeUrl);
	}

	/// <summary>
	/// シングルサインオン用のログイン先URLを作成
	/// </summary>
	/// <param name="url">Login先URL</param>
	/// <returns>ログイン先URL(コールバックURL付)</returns>
	protected string CreateLoginPageUrlForSingleSignOn(string url)
	{
		return new UrlCreator(url)
			.AddParam(Constants.REQUEST_KEY_FRONT_NEXT_URL, Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_OWNER_DECISION)
			.CreateUrl();
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

	#region プロパティ
	/// <summary>楽天IDConnectによるログイン？</summary>
	private bool IsRakutenIdConnectLogin
	{
		get { return (Request[Constants.REQUEST_KEY_RAKUTEN_ID_CONNECT_LOGIN] == "1"); }
	}
	/// <summary>Amazon支払いボタンを表示するかどうか</summary>
	protected bool IsVisibleAmazonPayButton
	{
		get
		{
			return ((Constants.AMAZON_PAYMENT_OPTION_ENABLED)
				&& (this.CartList.Items.Count == 1)
				&& (this.CartList.Items.All(x => x.CanUseAmazonPayment())));
		}
	}
	/// <summary>Amazonログインボタンを表示するかどうか</summary>
	protected bool IsVisibleAmazonLoginButton
	{
		get
		{
			return ((Constants.AMAZON_LOGIN_OPTION_ENABLED)
				&& (this.IsVisibleAmazonPayButton == false));
		}
	}
	/// <summary>AmazonコールバックURL</summary>
	protected string AmazonCallBackUrl
	{
		get
		{
			if (this.IsVisibleAmazonPayButton)
			{
				return AmazonUtil.CreateCallbackUrl(Constants.PAGE_FRONT_AMAZON_ORDER_CALLBACK);
			}
			else
			{
				return AmazonUtil.CreateCallbackUrl(Constants.PAGE_FRONT_AMAZON_LOGIN_CALLBACK);
			}
		}
	}
	/// <summary>アマゾンリクエスト</summary>
	protected AmazonCv2Redirect AmazonRequest { get; set; }
	#endregion
}
