/*
=========================================================================================================
  Module      : ソーシャルログイン連携画面処理(SocialLoginCooperation.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Common.Web;
using w2.App.Common.Amazon.Util;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Auth.RakutenIDConnect;
using w2.App.Common.Line.Util;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.User.SocialLogin;
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;
using w2.App.Common.Web.WrappedContols;

/// <summary>
/// ソーシャルログイン連携画面
/// </summary>
public partial class Form_User_SocialLoginCooperation : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			if (Request["status"] != SocialLoginUtil.STATUS_VALUE_AUTHORIZED)
			{
				this.WlErrorMessage.Text = SocialLoginUtil.GetErrorMessage(Request["reason"]);
			}
			// ソーシャルプラス側と連携情報を同期
			if (Constants.SOCIAL_LOGIN_ENABLED) SocialLoginUtil.SyncSocialProviderInfo(null, this.LoginUserId);

			// 楽天のプロパティセット
			SetRakutenProperty();

			SetCooperation();
		}

		if (Constants.AMAZON_LOGIN_OPTION_ENABLED && Constants.AMAZON_PAYMENT_CV2_ENABLED)
		{
			this.AmazonRequest = AmazonCv2Redirect.SignPayloadForSignIn(Constants.PAGE_FRONT_AMAZON_COOPERATION_CALLBACK);
		}
		this.DataBind();
	}

	/// <summary>
	/// ソーシャルログイン連携
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbConnect_Click(object sender, EventArgs e)
	{
		if (Constants.SOCIAL_LOGIN_ENABLED)
		{
			var providerType = (SocialLoginApiProviderType)Enum
				.Parse(typeof(SocialLoginApiProviderType),
				((LinkButton)sender).CommandArgument,
				true);
			var url = SocialLoginUtil.AuthenticateOrAssociate(this.LoginUserId, providerType, Request.Url.Authority);
			if (string.IsNullOrEmpty(url) == false) Response.Redirect(url);

		}
		else if(w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED && (Constants.SOCIAL_LOGIN_ENABLED == false))
		{
			Session[Constants.SESSION_KEY_NEXT_URL] = Constants.PATH_ROOT + Constants.PAGE_FRONT_SOCIAL_LOGIN_COOPERATION;
			var lineUrl = LineUtil.CreateConnectLineUrl(Session.SessionID);
			if (string.IsNullOrEmpty(lineUrl) == false) Response.Redirect(lineUrl);
		}
	}

	/// <summary>
	/// ソーシャルログイン連携解除
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisconnect_Click(object sender, EventArgs e)
	{
		var providerType = (SocialLoginApiProviderType)Enum.Parse(
			typeof(SocialLoginApiProviderType),
			((LinkButton)sender).CommandArgument,
			true);
		var sld = new SocialLoginDissociate();
		sld.Exec(Constants.SOCIAL_LOGIN_API_KEY,
			null,
			this.LoginUserId,
			providerType,
			true,
			true);

		// ソーシャルプラス側と連携情報を同期
		SocialLoginUtil.SyncSocialProviderInfo(null, this.LoginUserId);
		SessionManager.LineProviderUserId = string.Empty;

			// クエリを削除
		if (string.IsNullOrEmpty(Request.Url.Query) == false)
		{
			Response.Redirect(Request.RawUrl.Replace(Request.Url.Query, ""));
		}
		//LINE連携の解除
		if (w2.App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED) LineUtil.UnConnect(this.LoginUserId);
		// ソーシャル連携状態を最新の状態に更新
		SetCooperation();
	}

	/// <summary>
	/// PayPal認証完了
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbPayPalAuthComplete_Click(object sender, EventArgs e)
	{
		// ソーシャル連携状態を最新の状態に更新(postbackでリダイレクトしないイベントの場合必要)
		SetCooperation();

		// PayPal認証情報をセッションに格納（存在していたらエラー表示）
		var payPalScriptsForm = GetWrappedControl<WrappedPayPalPayScriptsFormControl>("ucPaypalScriptsForm");
		var user = PayPalUtility.Account.GetUserByPayPalCustomerId(payPalScriptsForm.PayPalPayerId);
		var isCooperateWithSomeone = (user != null) && (user.UserId != this.LoginUserId);
		if (isCooperateWithSomeone)
		{
			this.WlErrorMessage.Text = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_COOPERATE_WITH_SOMEONE));
			this.DataBind();
			return;
		}

		// 認証情報をセッションへ格納
		SetPaypalInfoToSession(payPalScriptsForm);

		// アカウント紐づけ
		PayPalUtility.Account.UpdateUserExtendForPayPal(
			this.LoginUserId,
			SessionManager.PayPalLoginResult,
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// PayPal連携解除ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisconnectPayPal_Click(object sender, EventArgs e)
	{
		// ソーシャル連携状態を最新の状態に更新(postbackでリダイレクトしないイベントの場合必要)
		SetCooperation();

		// ユーザーの紐付け解除
		PayPalUtility.Account.UpdateUserExtendForPayPal(
			this.LoginUserId,
			null,
			UpdateHistoryAction.Insert);

		// セッションから紐づけ解除
		SessionManager.PayPalLoginResult = null;
		SessionManager.PayPalCooperationInfo = null;
		this.GetCartObjectList().PayPalCooperationInfo = null;
	}

	/// <summary>
	/// 楽天IDConnectリクエストクリック（楽天OpenID紐づけ）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRakutenIdConnectLinked_Click(object sender, EventArgs e)
	{
		var beforeUrl = Constants.PATH_ROOT + Constants.PAGE_FRONT_SOCIAL_LOGIN_COOPERATION;
		var nextUrl = new UrlCreator(beforeUrl)
			.AddParam(Constants.REQUEST_KEY_RAKUTEN_ID_CONNECT_LINK, "1")
			.CreateUrl();
		this.Process.RedirectRakutenIdConnect(ActionType.Connect, beforeUrl, nextUrl);
	}

	/// <summary>
	/// 楽天IDConnectリクエストクリック（楽天OpenID紐づけ解除）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRakutenIdConnectNotLinked_Click(object sender, EventArgs e)
	{
		var service = new UserService();
		// 楽天OpenIDをブランク（空文字）更新
		service.ModifyUserExtend(
			this.LoginUserId,
			model =>
			{
				model.UserExtendDataValue[Constants.RAKUTEN_ID_CONNECT_OPEN_ID] = string.Empty;
			},
			UpdateHistoryAction.DoNotInsert);

		// 楽天OpenID紐づけ解除メッセージ表示
		this.IsDisplayRakutenIdConnectNotLinkedMessages = true;

		// セッションログイン情報更新
		var user = service.Get(this.LoginUserId);
		SetLoginUserData(user, UpdateHistoryAction.Insert);
		SessionManager.LoginUserRakutenOpenId = string.Empty;

		// Myページへ解除メッセージ表示
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_SOCIAL_LOGIN_COOPERATION)
			.AddParam(Constants.REQUEST_KEY_RAKUTEN_ID_CONNECT_LINK, "2")
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 楽天のプロパティセット
	/// </summary>
	protected void SetRakutenProperty()
	{
		this.IsDisplayRakutenIdConnectLinkedMessages = false;
		this.IsDisplayRakutenIdConnectNotLinkedMessages = false;

		// 楽天OpenID紐づけメッセージ表示？
		this.IsDisplayRakutenIdConnectLinkedMessages = (Request[Constants.REQUEST_KEY_RAKUTEN_ID_CONNECT_LINK] == "1");
		if (this.IsDisplayRakutenIdConnectLinkedMessages)
		{
			// セッションログイン情報更新
			var service = new UserService();
			var user = service.Get(this.LoginUserId);
			SetLoginUserData(user, UpdateHistoryAction.Insert);
			SessionManager.LoginUserRakutenOpenId = user.GetUserExtend().UserExtendDataValue[Constants.RAKUTEN_ID_CONNECT_OPEN_ID];
		}

		// 楽天OpenID紐づけ解除メッセージ表示？
		this.IsDisplayRakutenIdConnectNotLinkedMessages = (Request[Constants.REQUEST_KEY_RAKUTEN_ID_CONNECT_LINK] == "2");
	}

	/// <summary>
	/// Amazonログイン連携解除
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisconnectAmazon_Click(object sender, EventArgs e)
	{
		// ソーシャル連携状態を最新の状態に更新(postbackでリダイレクトしないイベントの場合必要)
		SetCooperation();

		// ユーザー拡張項目からAmazonユーザーID削除
		var userExtend = new UserService().GetUserExtend(this.LoginUserId);
		AmazonUtil.RemoveAmazonUserIdFromUserExtend(userExtend, this.LoginUserId, UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// ソーシャルログイン連携状態をセット
	/// </summary>
	protected void SetCooperation()
	{
		this.Providers = SocialLoginUtil.GetProviders(null, this.LoginUserId);
		this.IsCooperatedAmazon = string.IsNullOrEmpty(AmazonUtil.GetAmazonUserIdByUseId(this.LoginUserId)) == false;
		this.IsCooperatedLine = string.IsNullOrEmpty(LineUtil.GetLineUserIdByLoginUserId(this.LoginUserId)) == false;
		// ライン連携済みかつメールアドレスが存在しない場合はライン直接連携で自動ログインしたユーザ
		this.IsLineDirectAutoLoginUser = this.IsCooperatedLine && string.IsNullOrEmpty(this.LoginUserMail);
	}

	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }
	/// <summary>プロバイダ</summary>
	public List<SocialLoginApiProviderType> Providers { get; private set; }
	/// <summary>Amazonログイン連携済みかどうか</summary>
	public bool IsCooperatedAmazon { get; private set; }
	/// <summary>LINEログイン連携済みかどうか</summary>
	protected bool IsCooperatedLine { get; private set; }
	/// <summary>ログインユーザーの住所が日本か</summary>
	protected bool IsLoginUserAddrJp
	{
		get
		{
			if (this.IsLoggedIn == false) return false;
			return IsCountryJp(this.LoginUser.AddrCountryIsoCode);
		}
	}
	/// <summary>アマゾンリクエスト</summary>
	protected AmazonCv2Redirect AmazonRequest { get; set; }
	/// <summary>ライン連携で自動ログインされたユーザか</summary>
	public bool IsLineDirectAutoLoginUser { get; private set; }

	#region ラップ済コントロール宣言
	/// <summary>エラーメッセージ</summary>
	protected WrappedLiteral WlErrorMessage { get { return GetWrappedControl<WrappedLiteral>("lErrorMessage"); } }
	/// <summary>楽天OpenID紐づけ済み？</summary>
	public bool IsRakutenIdConnectLinked
	{
		get { return (string.IsNullOrEmpty(SessionManager.LoginUserRakutenOpenId) == false); }
	}
	/// <summary>楽天OpenID紐づけメッセージ表示？</summary>
	public bool IsDisplayRakutenIdConnectLinkedMessages { get; set; }
	/// <summary>楽天OpenID紐づけ解除メッセージ表示？</summary>
	public bool IsDisplayRakutenIdConnectNotLinkedMessages { get; set; }
	#endregion
}
