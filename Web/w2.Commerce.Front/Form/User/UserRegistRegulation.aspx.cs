/*
=========================================================================================================
  Module      : 会員登録規約画面処理(UserRegistRegulation.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.Amazon;

public partial class Form_User_UserRegistRegulation : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		SessionManager.TemporaryUserId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TEMPORARY_USER_ID]);

		// ログインチェック（ログイン済みの場合、トップ画面へ）
		// LINEミニアプリからの登録の場合は許可する
		if (this.IsLoggedIn && (SessionManager.HasTemporaryUserId == false))
		{
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT);
		}

		if (!IsPostBack)
		{
			// リクエスト情報にNextUrlが存在した場合、セッションNextUrlを格納
			// （会員完了画面時にBeforeUrl遷移ボタンの表示を行うため）
			if (Request[Constants.REQUEST_KEY_NEXT_URL] != null)
			{
				Session[Constants.SESSION_KEY_NEXT_URL] = NextUrlValidation(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NEXT_URL]));
			}
			// CrossPointアプリからPOST送信された場合セッションに値を格納する
			if (Constants.CROSS_POINT_OPTION_ENABLED
				&& (Request.Form[Constants.CROSS_POINT_REQUEST_PARAM_APP_KEY] != null))
			{
				SessionManager.AppKeyForCrossPoint = Request.Form[Constants.CROSS_POINT_REQUEST_PARAM_APP_KEY];
				SessionManager.MemberIdForCrossPoint = Request.Form[Constants.CROSS_POINT_REQUEST_PARAM_MEMBER_ID];
				SessionManager.PinCodeForCrossPoint = Request.Form[Constants.CROSS_POINT_REQUEST_PARAM_PIN_CD];
			}

			if (Constants.SOCIAL_LOGIN_ENABLED)
			{
				if (SessionManager.TemporaryStoreSocialLogin != null)
				{
					SessionManager.SocialLogin = SessionManager.TemporaryStoreSocialLogin;
				}
				// コールバックページor入力画面からの遷移時のみソーシャルログインセッションを保持
				var socialLogin = (SocialLoginModel)Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL];
				if ((socialLogin != null)
					&& ((socialLogin.TransitionSourcePath.EndsWith(Constants.PAGE_FRONT_SOCIAL_LOGIN_LOGIN_CALLBACK))
						|| (socialLogin.TransitionSourcePath.EndsWith(Constants.PAGE_FRONT_SOCIAL_LOGIN_ORDER_CALLBACK))
						|| (socialLogin.TransitionSourcePath.EndsWith(Constants.PAGE_FRONT_USER_REGIST_INPUT))))
				{
					socialLogin.TransitionSourcePath = this.AppRelativeVirtualPath;
				}
				else
				{
					Session.Remove(Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL);
				}
			}

			// Amazonログインオプションが有効でない場合はセッションのAmazonアカウント情報を破棄
			if (Constants.AMAZON_LOGIN_OPTION_ENABLED == false)
			{
				Session.Remove(AmazonConstants.SESSION_KEY_AMAZON_MODEL);
			}
		}
	}

	/// <summary>
	/// 規約に同意するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbAgree_Click(object sender, EventArgs e)
	{
		// 会員新規登録入力画面へ遷移する（HTTPS遷移）
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_REGIST_INPUT);
	}
}

