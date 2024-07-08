/*
=========================================================================================================
  Module      : ソーシャルログイン 基底ページ(SocialLoginPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.User.SocialLogin.Util;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

///*********************************************************************************************
/// <summary>
/// ソーシャルログイン 基底ページ
/// </summary>
///*********************************************************************************************
public class SocialLoginPage : BasePage
{
	/// <summary>リクエストキー：ステータス</summary>
	public const string REQUEST_KEY_STATUS = "status";
	/// <summary>リクエストキー：認証トークン</summary>
	public const string REQUEST_KEY_TOKEN = "token";

	/// <summary>
	/// リダイレクト
	/// </summary>
	/// <param name="nextUrl4Register">ユーザー登録時の遷移先</param>
	/// <param name="nextUrl4Login">ログイン認証後の遷移先</param>
	protected void Redirect(string nextUrl4Register, string nextUrl4Login)
	{
		if (!IsPostBack)
		{
			var status = Request[REQUEST_KEY_STATUS];

			// 直接のアクセスや、認証に失敗した場合はログイン画面へリダイレクト
			if (status != SocialLoginUtil.STATUS_VALUE_AUTHORIZED)
			{
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN);
			}

			var token = Request[REQUEST_KEY_TOKEN];
			var socialLogin = SocialLoginUtil.GetUser(token, false, true, false);

			// ソーシャル連携済の場合は、遷移元URLを設定する（パスワードのスキップで使用）
			if (socialLogin != null)
			{
				socialLogin.TransitionSourcePath = this.AppRelativeVirtualPath;
				Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL] = socialLogin;
			}

			if (socialLogin == null || string.IsNullOrEmpty(socialLogin.W2UserId))
			{
				Response.Redirect(nextUrl4Register);
			}

			// 認証
			var user = new UserService().Get(socialLogin.W2UserId);
			if (user != null)
			{
				// ログイン成功処理
				ExecLoginSuccessProcessAndGoNextForSocialPlusLogin(
					user,
					socialLogin,
					nextUrl4Login,
					UpdateHistoryAction.Insert);
			}
		}
	}

	#region プロパティ
	/// <summary>遷移先URL</summary>
	protected string NextUrl
	{
		get
		{
			var transformedNextUrl = (StringUtility.ToEmpty(SessionManager.NextUrl));
			var nextUrl = Request[Constants.REQUEST_KEY_RETURN_URL];
			if ((transformedNextUrl != Constants.PATH_ROOT) &&
				(transformedNextUrl != Constants.PATH_ROOT + Constants.PAGE_FRONT_DEFAULT) ||
				string.IsNullOrEmpty(nextUrl))
			{
				nextUrl = transformedNextUrl;
			}
			if (nextUrl.Length == 0) nextUrl = Constants.PATH_ROOT;

			return nextUrl;
		}
	}
	#endregion
}
