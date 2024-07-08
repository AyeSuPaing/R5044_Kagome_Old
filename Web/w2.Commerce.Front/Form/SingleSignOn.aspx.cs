/*
=========================================================================================================
  Module      : シングルサインオン連携画面処理(SingleSignOn.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.SingleSignOn;
using w2.Domain.UpdateHistory.Helper;

public partial class Form_SingleSignOn : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// シングルサインオン処理実行
		var facade = new SingleSignOnCreatorFacade(this.Context);
		var result = facade.CreateSingleSignOn().Execute();

		// 何もしない場合はTOPページへ
		if (result.IsNone)
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_DEFAULT);
		}
		// エラーの場合はエラーページへ
		if (result.IsFailure)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = string.IsNullOrEmpty(result.Messages)
				? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_USER_LOGIN_FOR_SINGLESIGNON)
				: result.Messages;

			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// ログイン処理実行
		var nextUrl = string.IsNullOrEmpty(result.NextUrl)
			? Constants.PATH_ROOT + Constants.PAGE_FRONT_DEFAULT
			: result.NextUrl;
		ExecLoginSuccessActionAndGoNextInner(result.User, nextUrl, UpdateHistoryAction.Insert);
	}
}