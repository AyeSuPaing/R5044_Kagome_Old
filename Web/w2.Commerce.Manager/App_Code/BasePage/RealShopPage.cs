/*
=========================================================================================================
  Module      : リアル店舗情報共通ページ(RealShopPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Text;
using System.Web;

/// <summary>
/// リアル店舗の概要の説明です
/// </summary>
public partial class RealShopPage : BasePage
{

	/// <summary>
	/// リアル店舗情報一覧URL作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>リアル店舗情報一覧URL</returns>
	protected static string CreateRealShopListUrl(Hashtable parameters)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_REALSHOP_LIST);
		url.Append("?").Append(Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID]));
		url.Append("&").Append(Constants.REQUEST_KEY_REALSHOP_NAME).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_REALSHOP_NAME]));
		url.Append("&").Append(Constants.REQUEST_KEY_REALSHOP_NAME_KANA).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_REALSHOP_NAME_KANA]));
		url.Append("&").Append(Constants.REQUEST_KEY_REALSHOP_TEL).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_REALSHOP_TEL]));
		url.Append("&").Append(Constants.REQUEST_KEY_REALSHOP_MAIL_ADDR).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_REALSHOP_MAIL_ADDR]));
		url.Append("&").Append(Constants.REQUEST_KEY_REALSHOP_FAX).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_REALSHOP_FAX]));
		url.Append("&").Append(Constants.REQUEST_KEY_REALSHOP_ADDR).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_REALSHOP_ADDR]));
		url.Append("&").Append(Constants.REQUEST_KEY_REALSHOP_ZIP).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_REALSHOP_ZIP]));
		url.Append("&").Append(Constants.REQUEST_KEY_REALSHOP_VALID_FLG).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_REALSHOP_VALID_FLG]));
		url.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode((string)parameters[Constants.REQUEST_KEY_SORT_KBN]));
		return url.ToString();
	}

	/// <summary>
	/// リアル店舗覧遷移URL作成
	/// </summary>
	/// <param name="parameters">検索情報</param>
	/// <param name="pageNo">ページ番号</param>
	/// <returns>リアル店舗覧遷移URL</returns>
	public static string CreateRealShopListUrl(Hashtable parameters, int pageNo)
	{
		return CreateRealShopListUrl(parameters) + "&" + Constants.REQUEST_KEY_PAGE_NO + "=" + pageNo.ToString();
	}

	/// <summary>
	/// リアル店舗情報一覧登録編集URL作成
	/// </summary>
	/// <param name="realShopId">リアル店舗ID</param>
	/// <param name="actionStatus">アクションステータス</param>
	/// <returns>リアル店舗情報一覧登録編集URL</returns>
	public string CreateRealShopRegistUrl(string realShopId, string actionStatus)
	{
		StringBuilder realShopRegistUrl = new StringBuilder();
		realShopRegistUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_REALSHOP_REGISTER);
		realShopRegistUrl.Append("?").Append(Constants.REQUEST_KEY_REALSHOP_REAL_SHOP_ID).Append("=").Append(HttpUtility.UrlEncode(realShopId));
		realShopRegistUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(actionStatus);
		return realShopRegistUrl.ToString();
	}
}