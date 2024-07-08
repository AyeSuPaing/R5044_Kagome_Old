/*
=========================================================================================================
  Module      : 会員退会入力画面処理(UserWithdrawalInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.Domain.FixedPurchase;

public partial class Form_User_UserWithdrawalInput : BasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
	}

	/// <summary>
	/// 退会リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbWithdrawal_Click(object sender, EventArgs e)
	{
		if (IsWithdrawalLimit(this.LoginUserId))
		{
			Response.Redirect((Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_WITHDRAWAL_INPUT));
			return;
		}

		// 退会処理
		var errorMessage = string.Empty;
		UserUtility.Withdrawal(
			this.LoginUserId,
			Constants.FLG_LASTCHANGED_USER,
			out errorMessage,
			UpdateHistoryAction.Insert);

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}
		// ログインIDをCookieから削除
		UserCookieManager.CreateCookieForLoginId("", false);

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_USER_WITHDRAWAL_COMPLETE);
	}

	/// <summary>
	/// 退会制限チェック
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <returns>制限あり</returns>
	protected bool IsWithdrawalLimit(string userId)
	{
		var isWithdrawalLimited = new FixedPurchaseService().HasActiveFixedPurchaseInfo(userId);
		return isWithdrawalLimited;
	}
}
