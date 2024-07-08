/*
=========================================================================================================
  Module      : 更新履歴共通ページ(UpdateHistoryPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using w2.Common.Web;

/// <summary>
/// UpdateHistoryPage の概要の説明です
/// </summary>
public class UpdateHistoryPage : BasePage
{
	/// <summary>
	/// 更新履歴情報確認URL作成
	/// </summary>
	/// <param name="updateHistoryKbn">更新履歴区分</param>
	/// <param name="userId">ユーザーID</param>
	/// <param name="masterId">マスタID</param>
	/// <returns>更新履歴情報確認URL</returns>
	public static string CreateUpdateHistoryConfirmUrl(string updateHistoryKbn, string userId, string masterId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_UPDATEHISTORY_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_UPDATEHISTORY_UPDATE_HISTORY_KBN, updateHistoryKbn)
			.AddParam(Constants.REQUEST_KEY_UPDATEHISTORY_USER_ID, userId)
			.AddParam(Constants.REQUEST_KEY_UPDATEHISTORY_MASTER_ID, masterId).CreateUrl();
		return url;
	}

	#region プロパティ
	/// <summary>リクエスト：更新履歴区分</summary>
	protected string RequestUpdateHistoryKbn
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_UPDATEHISTORY_UPDATE_HISTORY_KBN]).Trim(); }
	}
	/// <summary>リクエスト：ユーザID</summary>
	protected string RequestUserId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_UPDATEHISTORY_USER_ID]).Trim(); }
	}
	/// <summary>リクエスト：マスタID</summary>
	protected string RequestMasterId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_UPDATEHISTORY_MASTER_ID]).Trim(); }
	}
	#endregion
}