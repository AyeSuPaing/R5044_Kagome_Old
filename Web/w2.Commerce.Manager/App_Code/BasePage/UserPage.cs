/*
=========================================================================================================
  Module      : ユーザー共通ページ(UserPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.Common.Web;

/// <summary>
/// UserPage の概要の説明です
/// </summary>
public class UserPage : BasePage
{
	/// <summary>
	/// ユーザーポイント履歴一覧URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <param name="isPopup">ポップアップ表示？</param>
	/// <returns> ユーザーポイント履歴一覧URL</returns>
	public static string CreateUserPointHistoryUrl(string userId, bool isPopup = false)
	{
		var url = new UrlCreator(Constants.PATH_ROOT_MP + Constants.PAGE_W2MP_MANAGER_USERPOINTHISTORY_LIST)
			.AddParam(Constants.REQUEST_KEY_USER_ID, userId)
			.AddParam(Constants.REQUEST_KEY_POINT_KBN, Constants.FLG_USERPOINT_POINT_KBN_BASE)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, isPopup ? Constants.KBN_WINDOW_POPUP : string.Empty)
			.CreateUrl();

		return url;
	}
}