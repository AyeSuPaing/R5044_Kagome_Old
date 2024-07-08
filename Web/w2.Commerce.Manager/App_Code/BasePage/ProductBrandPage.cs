/*
=========================================================================================================
  Module      : ブランド共通ページ(ProductBrandPage.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;

/// <summary>
/// ProductBrandPage の概要の説明です
/// </summary>
public class ProductBrandPage : BasePage
{
	/// <summary>
	/// ブランド一覧遷移URL作成
	/// </summary>
	/// <param name="htParam">検索情報</param>
	/// <param name="iPageNumber">表示開始記事番号</param>
	/// <returns>ブランド一覧URL</returns>
	protected static string CreateListUrl(Hashtable htParam, int iPageNumber)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(CreateListUrl());
		sbResult.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(iPageNumber.ToString());

		return sbResult.ToString();
	}

	/// <summary>
	/// ブランド一覧URL作成
	/// </summary>
	/// <returns>ブランド一覧URL</returns>
	protected static string CreateListUrl()
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTBRAND_LIST);

		return sbResult.ToString();
	}

	/// <summary>
	/// ブランド詳細URL作成
	/// </summary>
	/// <param name="strBrandId">ブランドID</param>
	/// <returns>ブランド詳細URL</returns>
	protected static string CreateProductBrandDetailUrl(string strBrandId)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTBRAND_CONFIRM);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_PRODUCTBRAND_BRAND_ID).Append("=").Append(HttpUtility.UrlEncode(strBrandId));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return sbResult.ToString();
	}

	/// <summary>
	/// ブランド編集URL作成
	/// </summary>
	/// <param name="strBrandId">ブランドID</param>
	/// <param name="strActionStatus">アクションステータス</param>
	/// <returns>ブランド編集URL</returns>
	protected static string CreateProductBrandRegistUrl(string strBrandId, string strActionStatus)
	{
		StringBuilder sbResult = new StringBuilder();
		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCTBRAND_REGISTER);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_PRODUCTBRAND_BRAND_ID).Append("=").Append(HttpUtility.UrlEncode(strBrandId));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(strActionStatus);

		return sbResult.ToString();
	}
}