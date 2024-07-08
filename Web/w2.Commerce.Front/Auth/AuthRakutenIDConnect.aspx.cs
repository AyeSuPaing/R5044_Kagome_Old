/*
=========================================================================================================
  Module      : 楽天IDConnect連携画面処理(AuthRakutenIDConnect.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Auth.RakutenIDConnect;
using w2.App.Common.Order.Payment;
using w2.Common.Web;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

public partial class Auth_AuthRakutenIDConnect : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 楽天IDConnectアクション情報存在チェック
		CheckActionInfoExists();

		// 楽天IDConnect（ログイン認証→トークン取得→楽天会員情報取得）実行
		ExecRakutenIdConnect();

		// 各アクションに応じた処理実施
		switch (SessionManager.RakutenIdConnectActionInfo.Type)
		{
			// ログイン？
			case ActionType.Login:
				ExecLogin(true);
				break;

			// 新規会員登録？
			case ActionType.UserRegister:
				ExecUserRegister();
				break;

			// 楽天OpenID連携？
			case ActionType.Connect:
				ExecConnect();
				break;

			default:
				// トップページへ
				Response.Redirect(Constants.PATH_ROOT);
				break;
		}
	}

	#region メソッド
	/// <summary>
	/// 楽天IDConnectアクション情報存在チェック
	/// </summary>
	private void CheckActionInfoExists()
	{
		// 楽天IDConnectアクション情報が存在しない場合はトップページへ
		if (SessionManager.RakutenIdConnectActionInfo == null)
		{
			Response.Redirect(Constants.PATH_ROOT);
		}
	}

	/// <summary>
	/// 楽天IDConnect（ログイン認証→トークン取得→楽天会員情報取得）実行
	/// 各プロパティに値をセット
	/// </summary>
	private void ExecRakutenIdConnect()
	{
		// 楽天IDConnectログイン認証レスポンスデータ取得
		this.AuthResponseData = new RakutenIDConnectAuthResponseData(Request);

		// ログイン認証失敗 or ステートが異なる場合はログインページへ
		if ((this.AuthResponseData.IsSuccess == false)
			|| (this.AuthResponseData.State != SessionManager.RakutenIdConnectActionInfo.State))
		{
			// エラーログ出力
			var error = this.AuthResponseData.Error;
			var errorDescription = this.AuthResponseData.ErrorDescription;
			if (this.AuthResponseData.IsSuccess)
			{
				error = "state_error";
				errorDescription = "state is invalid";
			}
			RakutenIDConnectLogger.WriteErrorLog(error, errorDescription, PaymentFileLogger.PaymentProcessingType.Unknown);

			var beforeUrl = SessionManager.RakutenIdConnectActionInfo.BeforeUrl;
			var nextUrl = SessionManager.RakutenIdConnectActionInfo.NextUrl;

			// アクション情報削除
			SessionManager.RakutenIdConnectActionInfo = null;

			// ログインページへ
			var url = new UrlCreator(beforeUrl)
				.AddParam(Constants.REQUEST_KEY_NEXT_URL, nextUrl)
				.CreateUrl();
			Response.Redirect(url);
		}

		// デバッグログ
		RakutenIDConnectLogger.WriteDebugLog("auth", string.Format("?{0}", Request.QueryString), PaymentFileLogger.PaymentProcessingType.Unknown);

		// トークン取得失敗 or ノンスが異なるの場合はトップページへ
		var tokenAPi = new RakutenIDConnectTokenApi();
		this.TokenResponseData = tokenAPi.Exec(this.AuthResponseData.Code);
		if ((this.TokenResponseData.IsSuccess == false)
			|| ((string.IsNullOrEmpty(Constants.RAKUTEN_ID_CONNECT_MOCK_URL))
				&& (this.TokenResponseData.Nonce != SessionManager.RakutenIdConnectActionInfo.Nonce)))
		{
			// エラーログ出力
			var error = this.TokenResponseData.Error;
			var errorDescription = this.TokenResponseData.ErrorDescription;
			if (this.TokenResponseData.IsSuccess)
			{
				error = "nonce_error";
				errorDescription = "nonce is invalid";
			}
			RakutenIDConnectLogger.WriteErrorLog(error, errorDescription, PaymentFileLogger.PaymentProcessingType.Unknown);

			// アクション情報削除
			SessionManager.RakutenIdConnectActionInfo = null;

			// TOPページへ
			Response.Redirect(Constants.PATH_ROOT);
		}

		// ログイン or 楽天OpenID連携の場合は楽天会員情報を取得しない
		if ((SessionManager.RakutenIdConnectActionInfo.Type == ActionType.Login)
			|| (SessionManager.RakutenIdConnectActionInfo.Type == ActionType.Connect))
		{
			return;
		}

		// 楽天会員情報取得失敗の場合はトップページへ
		var userInfoApi =
			new RakutenIDConnectUserInfoApi(this.TokenResponseData.AccessToken);
		this.UserInfoResponseData = userInfoApi.Exec();
		if (this.UserInfoResponseData.IsSuccess == false)
		{
			// エラーログ出力
			RakutenIDConnectLogger.WriteErrorLog(this.UserInfoResponseData.Error, this.UserInfoResponseData.ErrorDescription, PaymentFileLogger.PaymentProcessingType.Unknown);

			// アクション情報削除
			SessionManager.RakutenIdConnectActionInfo = null;
			Response.Redirect(Constants.PATH_ROOT);
		}
	}

	/// <summary>
	/// ログイン
	/// </summary>
	/// <param name="withRegister">ユーザーが存在しない場合に新規会員登録へ遷移するか</param>
	private void ExecLogin(bool withRegister = false)
	{
		// 楽天OpenIDでユーザー取得
		var userService = new UserService();
		var user = userService.GetByExtendColumn(Constants.RAKUTEN_ID_CONNECT_OPEN_ID, this.TokenResponseData.OpenId);

		// ユーザーが存在しない場合は新規会員登録
		if ((user == null) && withRegister)
		{
			this.Process.RedirectRakutenIdConnect(ActionType.UserRegister);
			return;
		}
		// ユーザーが存在しない場合はエラーページへ
		if (user == null)
		{
			RedirectErrorPage(WebMessages.ERRMSG_RAKUTEN_ID_CONNECT_NOT_CONNECTED);
		}

		// ユーザーが存在する場合はユーザー情報をセット
		SessionManager.RakutenIdConnectActionInfo.SetUser(user);

		// ログインページへ遷移しログイン実施
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN)
			.AddParam(Constants.REQUEST_KEY_NEXT_URL, SessionManager.RakutenIdConnectActionInfo.NextUrl)
			.AddParam(Constants.REQUEST_KEY_RAKUTEN_ID_CONNECT_LOGIN, "1")
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 会員登録
	/// </summary>
	private void ExecUserRegister()
	{
		// 楽天OpenIDがかぶらないかチェック
		CheckOpenID();

		// 楽天会員メールアドレスでユーザー取得
		var user = new UserService().GetUserByMailAddr(this.UserInfoResponseData.Email);
		// ユーザーが存在する場合はエラーページへ
		if ((user != null) && user.IsMember)
		{
			RedirectErrorPage(WebMessages.ERRMSG_RAKUTEN_ID_CONNECT_USER_REGISTERED);
		}

		// ユーザーが存在しない場合は楽天会員情報をセット
		SessionManager.RakutenIdConnectActionInfo.SetRakutenIDConnectUserInfoResponseData(this.UserInfoResponseData);

		var pageUrl = (SessionManager.RakutenIdConnectActionInfo.IsLandingCart)
			? SessionManager.RakutenIdConnectActionInfo.NextUrl
			: Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_INPUT;
		var url = new UrlCreator(pageUrl)
			.AddParam(Constants.REQUEST_KEY_RAKUTEN_REGIST, Constants.FLG_RAKUTEN_USER_REGIST)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 楽天OpenID連携
	/// </summary>
	private void ExecConnect()
	{
		var userService = new UserService();

		// 楽天OpenIDがかぶらないかチェック
		CheckOpenID();

		// ログインIDでユーザー取得
		var user = userService.Get(this.LoginUserId);

		// 楽天OpenID更新
		userService.ModifyUserExtend(
			user.UserId,
			model =>
			{
				model.UserExtendDataValue[Constants.RAKUTEN_ID_CONNECT_OPEN_ID] = this.TokenResponseData.OpenId;
			},
			UpdateHistoryAction.DoNotInsert);
		// アクション情報削除
		var nextUrl = SessionManager.RakutenIdConnectActionInfo.NextUrl;
		SessionManager.RakutenIdConnectActionInfo = null;

		// ソーシャルログイン連携ページへ
		Response.Redirect(nextUrl);
	}

	/// <summary>
	/// エラーページへ遷移
	/// </summary>
	/// <param name="messageKey">メッセージキー</param>
	private void RedirectErrorPage(string messageKey)
	{
		// 戻りURL取得
		var beforeUrl = SessionManager.RakutenIdConnectActionInfo.BeforeUrl;

		// アクション情報削除
		SessionManager.RakutenIdConnectActionInfo = null;

		// エラーページへ
		Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(messageKey);
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
			.AddParam(Constants.REQUEST_KEY_BACK_URL, beforeUrl)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 楽天OpenIDをチェック
	/// </summary>
	private void CheckOpenID()
	{
		// ユーザーが存在する場合はエラーページへ
		var user = new UserService().GetByExtendColumn(Constants.RAKUTEN_ID_CONNECT_OPEN_ID, this.TokenResponseData.OpenId);
		if (user != null)
		{
			RedirectErrorPage(WebMessages.ERRMSG_RAKUTEN_ID_CONNECT_CONNECTED);
		}
	}

	#endregion

	#region プロパティ
	/// <summary>楽天IDConnectログイン認証レスポンスデータ</summary>
	public RakutenIDConnectAuthResponseData AuthResponseData { get; set; }
	/// <summary>楽天IDConnectトークンレスポンスデータ</summary>
	public RakutenIDConnectTokenResponseData TokenResponseData { get; set; }
	/// <summary>楽天IDConnectユーザー情報取得レスポンスデータ</summary>
	public RakutenIDConnectUserInfoResponseData UserInfoResponseData { get; set; }
	#endregion
}