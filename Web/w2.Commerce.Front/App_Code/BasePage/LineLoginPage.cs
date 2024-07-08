/*
=========================================================================================================
  Module      : LINEログイン 基底ページ(LineLoginPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Line.Util;
using w2.Common.Web;

/// <summary>
/// LINEログイン 基底ページ
/// </summary>
public class LineLoginPage : BasePage
{
	/// <summary>
	/// 初期処理
	/// </summary>
	protected void InitPage()
	{
		if (!IsPostBack)
		{
			var state = Request["state"];
			if ((string.IsNullOrEmpty(Request["error"]) == false)
				|| (state != Session.SessionID)) Response.Redirect(this.NextUrl);
			var code = Request["code"];

			// LINEアカウント認証
			this.LineUserId = LineUtil.AuthenticationLine(code);
			if (this.LineUserId == null) Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
	}

	/// <summary>
	/// エラーでソーシャルログイン 連携画面に戻る
	/// </summary>
	/// <param name="status">ステータス</param>
	/// <param name="reason">原因</param>
	public  void ReturnSocialLoginCooperationPageWithError(string status, string reason)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_SOCIAL_LOGIN_COOPERATION)
			.AddParam("status", status)
			.AddParam("reason", reason)
			.CreateUrl();
		Response.Redirect(url);
	}

	#region プロパティ
	/// <summary>遷移先URL</summary>
	protected string NextUrl
	{
		get
		{
			var nextUrl = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_NEXT_URL]);
			if (nextUrl.Length == 0) nextUrl = Constants.PATH_ROOT;
			return nextUrl;
		}
	}
	#endregion
}
