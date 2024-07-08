/*
=========================================================================================================
  Module      : ソーシャルログイン 連携コールバック画面(CooperationCallback.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.User.SocialLogin;
using w2.App.Common.User.SocialLogin.Util;
using w2.Common.Web;

/// <summary>
/// ソーシャルログイン 連携コールバック画面
/// </summary>
public partial class Form_User_SocialLogin_CooperationCallback : SocialLoginPage
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
			// 認証されない場合はエラーとする
			if (Request["status"] != SocialLoginUtil.STATUS_VALUE_AUTHORIZED)
			{
				ReturnSocialLoginCooperationPageWithError(Request["status"], Request["reason"]);
			}

			var token = Request[REQUEST_KEY_TOKEN];

			// ユーザ情報の取得
			var socialLogin = SocialLoginUtil.GetUser(token, false, true, false);

			// 既に他ユーザと連携済の場合はエラーとする
			if (socialLogin == null || ((string.IsNullOrEmpty(socialLogin.W2UserId) == false) && (socialLogin.W2UserId != this.LoginUserId)))
			{
				ReturnSocialLoginCooperationPageWithError(
					"failed",
					"This social media has already been associated with another user.");
			}

			socialLogin.W2UserId = this.LoginUserId;
			Session[Constants.SESSION_KEY_SOCIAL_LOGIN_MODEL] = socialLogin;

			// ユーザーID紐付け
			var authUser = new SocialLoginMap();
			authUser.Exec(Constants.SOCIAL_LOGIN_API_KEY, socialLogin.SPlusUserId, socialLogin.W2UserId, true);

			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_SOCIAL_LOGIN_COOPERATION);
		}
	}

	/// <summary>
	/// エラーでソーシャルログイン 連携画面に戻る
	/// </summary>
	/// <param name="status">ステータス</param>
	/// <param name="reason">原因</param>
	private void ReturnSocialLoginCooperationPageWithError(string status, string reason)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_SOCIAL_LOGIN_COOPERATION)
			.AddParam("status", status)
			.AddParam("reason", reason)
			.CreateUrl();
		Response.Redirect(url);
	}
}