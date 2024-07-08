/*
=========================================================================================================
  Module      : リアル店舗在庫情報共通ページ(RealShopProductStockPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// リアル店舗在庫情報共通の概要の説明です
/// </summary>
public class RealShopProductStockPage : BasePage
{
	/// <summary>
	/// リアル店舗商品在庫一覧遷移URL作成
	/// </summary>
	/// <param name="search">検索情報</param>
	/// <returns>リアル店舗商品在庫一覧遷移URL</returns>
	public static string CreateRealShopProductStockListUrl(Hashtable search)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_REALSHOPPRODUCTSTOCK_LIST);
		url.Append("?").Append(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID).Append("=").Append(HttpUtility.UrlEncode((string)search[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID]));
		url.Append("&").Append(Constants.REQUEST_KEY_REALSHOP_NAME).Append("=").Append(HttpUtility.UrlEncode((string)search[Constants.REQUEST_KEY_REALSHOP_NAME]));
		url.Append("&").Append(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode((string)search[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_PRODUCT_ID]));
		url.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode((string)search[Constants.REQUEST_KEY_SORT_KBN]));
		url.Append("&").Append(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_VARIATION_ID).Append("=").Append(HttpUtility.UrlEncode((string)search[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_VARIATION_ID]));
		url.Append("&").Append(Constants.REQUEST_KEY_PRODUCT_NAME).Append("=").Append(HttpUtility.UrlEncode((string)search[Constants.REQUEST_KEY_PRODUCT_NAME]));
		url.Append("&").Append(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK).Append("=").Append(HttpUtility.UrlEncode((string)search[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK]));
		url.Append("&").Append(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_TYPE).Append("=").Append(HttpUtility.UrlEncode((string)search[Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK_TYPE]));

		return url.ToString();
	}
	
	/// <summary>
	/// リアル店舗商品在庫一覧遷移URL作成
	/// </summary>
	/// <param name="search">検索情報</param>
	/// <param name="pageNumber">表示開始記事番号</param>
	/// <returns>リアル店舗商品在庫一覧遷移URL</returns>
	public static string CreateRealShopProductStockListUrl(Hashtable search, int pageNumber)
	{
		return CreateRealShopProductStockListUrl(search) + "&" + Constants.REQUEST_KEY_PAGE_NO + "=" + pageNumber.ToString();
	}
	
	/// <summary>
	/// データバインド用リアル店舗商品在庫詳細URL作成
	/// </summary>
	/// <param name="realShopId">リアル店舗ID</param>
	/// <returns>リアル店舗商品在庫詳細URL</returns>
	public static string CreateRealShopProductStockDetailUrl(string realShopId)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_REALSHOPPRODUCTSTOCK_LIST);
		url.Append("?").Append(Constants.REQUEST_KEY_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID).Append("=").Append(HttpUtility.UrlEncode(realShopId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_UPDATE);
		return url.ToString();
	}
}