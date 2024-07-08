/*
=========================================================================================================
  Module      : セットプロモーション共通ページ(SetPromotionPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// SetPromotionPage の概要の説明です
/// </summary>
public class SetPromotionPage : ProductPage
{
	/// <summary>
	/// セットプロモーション一覧URL作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <param name="pageNo">ページ番号</param>
	/// <returns>セットプロモーション一覧URL</returns>
	protected string CreateListUrl(Hashtable parameters, int pageNo)
	{
		StringBuilder listUrl = new StringBuilder();
		listUrl.Append(CreateListUrl(parameters));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(pageNo);

		return listUrl.ToString();
	}
	/// <summary>
	/// セットプロモーション一覧URL作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>セットプロモーション一覧URL</returns>
	protected string CreateListUrl(Hashtable parameters)
	{
		StringBuilder listUrl = new StringBuilder();
		listUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_SETPROMOTION_LIST);
		listUrl.Append("?").Append(Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_ID).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_ID]));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_NAME).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_NAME]));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SETPROMOTION_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SETPROMOTION_PRODUCT_ID]));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SETPROMOTION_CATEGORY_ID).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SETPROMOTION_CATEGORY_ID]));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SETPROMOTION_STATUS).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SETPROMOTION_STATUS]));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_FROM).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_FROM]));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_TO).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_DATE_TO]));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_FROM).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_FROM]));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_TO).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SETPROMOTION_BEGIN_TIME_TO]));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SETPROMOTION_END_DATE_FROM).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SETPROMOTION_END_DATE_FROM]));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SETPROMOTION_END_DATE_TO).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SETPROMOTION_END_DATE_TO]));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SETPROMOTION_END_TIME_FROM).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SETPROMOTION_END_TIME_FROM]));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SETPROMOTION_END_TIME_TO).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SETPROMOTION_END_TIME_TO]));
		listUrl.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(parameters[Constants.REQUEST_KEY_SORT_KBN]);

		return listUrl.ToString();
	}

	/// <summary>
	/// セットプロモーション詳細URL作成
	/// </summary>
	/// <param name="setPromotionId">セットプロモーションID</param>
	/// <param name="actionStatus">アクションステータス</param>
	/// <returns>セットプロモーション詳細URL</returns>
	protected string CreateDetailUrl(string setPromotionId, string actionStatus)
	{
		StringBuilder detailUrl = new StringBuilder();
		detailUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_SETPROMOTION_REGISTER);
		detailUrl.Append("?").Append(Constants.REQUEST_KEY_SETPROMOTION_SETPROMOTION_ID).Append("=").Append(HttpUtility.UrlEncode(setPromotionId));
		detailUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(actionStatus));

		return detailUrl.ToString();
	}
}