/*
=========================================================================================================
  Module      : シリアルキー共通ページ(SerialKeyPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Web;
using System.Text;

/// <summary>
/// SerialKeyPage の概要の説明です
/// </summary>
public partial class SerialKeyPage : BasePage
{
	/// <summary>
	/// シリアルキー一覧遷移URL作成
	/// </summary>
	/// <param name="searchParam">検索情報</param>
	/// <param name="pageNumber">表示開始記事番号</param>
	/// <returns>シリアルキー一覧遷移URL</returns>
	protected string CreateSerialKeyListUrl(Hashtable searchParam, int pageNumber)
	{
		return CreateSerialKeyListUrl(searchParam) + "&" + Constants.REQUEST_KEY_PAGE_NO + "=" + pageNumber.ToString();
	}
	/// <summary>
	/// シリアルキー一覧遷移URL作成
	/// </summary>
	/// <param name="searchParam">検索情報</param>
	/// <returns>シリアルキー一覧遷移URL</returns>
	protected string CreateSerialKeyListUrl(Hashtable searchParam)
	{
		StringBuilder urlResult = new StringBuilder(CreateSerialKeyListUrlWithoutPageNo(searchParam));
		if (searchParam != null)
		{
			if (searchParam.ContainsKey(Constants.REQUEST_KEY_PAGE_NO))
			{
				urlResult.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(HttpUtility.UrlEncode((string)searchParam[Constants.REQUEST_KEY_PAGE_NO]));
			}
		}

		return urlResult.ToString();
	}
	/// <summary>
	/// シリアルキー一覧遷移URL作成
	/// </summary>
	/// <param name="searchParam">検索情報</param>
	/// <returns>シリアルキー一覧遷移URL</returns>
	protected string CreateSerialKeyListUrlWithoutPageNo(Hashtable searchParam)
	{
		StringBuilder urlResult = new StringBuilder();
		urlResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_SERIALKEY_LIST);
		if (searchParam != null)
		{
			urlResult.Append("?").Append(Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY).Append("=").Append(HttpUtility.UrlEncode((string)searchParam[Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY]));
			urlResult.Append("&").Append(Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode((string)searchParam[Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID]));
			urlResult.Append("&").Append(Constants.REQUEST_KEY_SERIALKEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode((string)searchParam[Constants.REQUEST_KEY_SERIALKEY_USER_ID]));
			urlResult.Append("&").Append(Constants.REQUEST_KEY_SERIALKEY_ORDER_ID).Append("=").Append(HttpUtility.UrlEncode((string)searchParam[Constants.REQUEST_KEY_SERIALKEY_ORDER_ID]));
			urlResult.Append("&").Append(Constants.REQUEST_KEY_SERIALKEY_STATUS).Append("=").Append(HttpUtility.UrlEncode((string)searchParam[Constants.REQUEST_KEY_SERIALKEY_STATUS]));
			urlResult.Append("&").Append(Constants.REQUEST_KEY_SERIALKEY_VALID_FLG).Append("=").Append(HttpUtility.UrlEncode((string)searchParam[Constants.REQUEST_KEY_SERIALKEY_VALID_FLG]));
			urlResult.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode((string)searchParam[Constants.REQUEST_KEY_SORT_KBN]));
		}

		return urlResult.ToString();
	}

	/// <summary>
	/// データバインド用シリアルキー編集URL作成
	/// </summary>
	/// <param name="serialKey">シリアルキー</param>
	/// <param name="productId">商品ID</param>
	/// <param name="actionStatus">アクションステータス</param>
	/// <returns>シリアルキー詳細URL</returns>
	protected string CreateSerialKeyRegistUrl(string serialKey, string productId, string actionStatus)
	{
		StringBuilder urlResult = new StringBuilder();
		urlResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_SERIALKEY_REGISTER);
		urlResult.Append("?").Append(Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY).Append("=").Append(HttpUtility.UrlEncode(serialKey));
		urlResult.Append("&").Append(Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(productId));
		urlResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(actionStatus);

		return urlResult.ToString();
	}

	/// <summary>
	/// シリアルキー詳細URL作成
	/// </summary>
	/// <param name="serialKey">シリアルキー</param>
	/// <param name="productId">商品ID</param>
	/// <returns>シリアルキー詳細URL</returns>
	protected string CreateSerialKeyDetailUrl(string serialKey, string productId)
	{
		StringBuilder urlResult = new StringBuilder();
		urlResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_SERIALKEY_CONFIRM);
		urlResult.Append("?").Append(Constants.REQUEST_KEY_SERIALKEY_SERIAL_KEY).Append("=").Append(HttpUtility.UrlEncode(serialKey));
		urlResult.Append("&").Append(Constants.REQUEST_KEY_SERIALKEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(productId));
		urlResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return urlResult.ToString();
	}

	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>商品情報詳細URL</returns>
	protected string CreateProductDetailUrl(string productId)
	{
		StringBuilder urlResult = new StringBuilder();
		urlResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_PRODUCT_CONFIRM);
		urlResult.Append("?").Append(Constants.REQUEST_KEY_PRODUCT_ID).Append("=").Append(HttpUtility.UrlEncode(productId));
		urlResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL));
		urlResult.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));

		return urlResult.ToString();
	}

	/// <summary>
	/// 受注詳細詳細URL作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>注文情報詳細URL</returns>
	protected string CreateOrderDetailUrl(string orderId)
	{
		StringBuilder urlResult = new StringBuilder();
		urlResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_ORDER_CONFIRM);
		urlResult.Append("?").Append(Constants.REQUEST_KEY_ORDER_ID).Append("=").Append(HttpUtility.UrlEncode(orderId));
		urlResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(Constants.ACTION_STATUS_DETAIL));
		urlResult.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));

		return urlResult.ToString();
	}

	/// <summary>
	/// ユーザ詳細URL作成
	/// </summary>
	/// <param name="userId">ユーザID</param>
	/// <returns>ユーザ詳細URL</returns>
	protected string CreateUserDetailUrl(string userId)
	{
		StringBuilder urlResult = new StringBuilder();
		urlResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_CONFIRM);
		urlResult.Append("?").Append(Constants.REQUEST_KEY_USER_ID).Append("=").Append(HttpUtility.UrlEncode(userId));
		urlResult.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);
		urlResult.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(Constants.KBN_WINDOW_POPUP);

		return urlResult.ToString();
	}
}
