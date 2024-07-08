/*
=========================================================================================================
  Module      : シルバーエッグアイジェント用処理(SilvereggAigentRecommend.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

/// <summary>
/// シルバーエッグアイジェント用処理
/// </summary>
public class SilvereggAigentRecommend
{
	/// <summary>
	/// レコメンド対象ページIDを取得
	/// </summary>
	/// <returns>レコメンド対象ページID</returns>
	public static string GetRecommendPageId()
	{
		if (Constants.RECOMMEND_ENGINE_KBN == Constants.RecommendEngine.Silveregg)
		{
			var recommendPageId = "";
			var device = ((Constants.SMARTPHONE_OPTION_ENABLED) && (SmartPhoneUtility.CheckSmartPhone(HttpContext.Current.Request.UserAgent))) ? "sp_" : "pc_";

			if ((HttpContext.Current.Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_DEFAULT, true, null)) || (HttpContext.Current.Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_DEFAULT_BRAND_TOP, true, null)))
			{
				recommendPageId = device + "top";
			}
			else if (HttpContext.Current.Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_PRODUCT_LIST, true, null))
			{
				recommendPageId = device + "list";
			}
			else if (HttpContext.Current.Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_PRODUCT_DETAIL, true, null))
			{
				recommendPageId = device + "pddt";
			}
			else if ((HttpContext.Current.Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_CART_LIST, true, null)))
			{
				recommendPageId = device + "cart";
			}
			else if (HttpContext.Current.Request.Url.AbsolutePath.Contains(Constants.PAGE_FRONT_ORDER_COMPLETE))
			{
				recommendPageId = device + "thky";
			}
			else if (HttpContext.Current.Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_MYPAGE, true, null))
			{
				recommendPageId = device + "mypg";
			}
			else if (HttpContext.Current.Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_FAVORITE_LIST, true, null))
			{
				recommendPageId = device + "fav";
			}
			else if (HttpContext.Current.Request.Url.AbsolutePath.EndsWith(Constants.PAGE_FRONT_USER_PRODUCT_ARRIVAL_MAIL_LIST, true, null))
			{
				recommendPageId = device + "ariv";
			}
			return recommendPageId;
		}
		return "";
	}

	/// <summary>
	/// シルバーエッグへ商品IDを連携するか判定
	/// </summary>
	/// <param name="recommendCode">入力されたレコメンドコード</param>
	/// <returns>判定結果</returns>
	public static bool SetIsRequestProductId(string recommendCode)
	{
		if (recommendCode == null) return false;

		// 商品詳細・カートリスト・注文完了の場合は商品IDを連携する
		switch (recommendCode.Substring(2, 3))
		{
			case "311":
			case "312":
			case "313":
			case "314":
			case "321":
			case "411":
			case "412":
			case "413":
			case "414":
			case "421":
			case "511":
			case "512":
			case "513":
			case "514":
			case "521":
				return true;

			default:
				return false;
		}
	}

	/// <summary>
	/// シルバーエッグへ注文情報を連携するか判定
	/// </summary>
	/// <param name="recommendCode">入力されたレコメンドコード</param>
	/// <returns>判定結果</returns>
	public static bool SetIsRequestOrderList(string recommendCode)
	{
		if (recommendCode == null) return false;

		// 注文完了の場合は注文情報を連携する
		switch (recommendCode.Substring(2, 3))
		{
			case "511":
			case "512":
			case "513":
			case "514":
			case "521":
				return true;

			default:
				return false;
		}
	}
}