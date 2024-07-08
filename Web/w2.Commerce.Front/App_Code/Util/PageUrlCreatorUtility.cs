/*
=========================================================================================================
  Module      : 画面URL生成ユーティリティ(PageUrlCreatorUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.Common.Web;

/// <summary>
/// 画面URL生成ユーティリティ
/// </summary>
public class PageUrlCreatorUtility
{
	/// <summary>
	/// 定期購入詳細URLの作成
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <returns>定期購入詳細URL</returns>
	public static string CreateFixedPurchaseDetailUrl(string fixedPurchaseId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_FIXED_PURCHASE_DETAIL)
			.AddParam(Constants.REQUEST_KEY_FIXED_PURCHASE_ID, fixedPurchaseId)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// Amazon Payウィジェットコールバック画面のURL作成
	/// </summary>
	/// <param name="isSmartPhone">TRUE: スマートフォン FALSE: PC</param>
	/// <param name="isReadOnly">TRUE: 入力 FALSE: 詳細</param>
	/// <param name="orderId">注文ID</param>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <returns>コールバック画面URL</returns>
	public static string CreateAmazonPayWidgetCallbackUrl(bool isSmartPhone, bool isReadOnly, string orderId = null, string fixedPurchaseId = null)
	{
		var baseUrl = Constants.PATH_ROOT;
		var key = string.Empty;
		var value = string.Empty;

		// PCの場合はウィジェット画面を返す、SPの場合は遷移型のため購入履歴詳細、または、定期購入情報詳細を返す
		if (isSmartPhone == false) return CreateAmazonPayWidgetUrl(isReadOnly, orderId, fixedPurchaseId);

		if (string.IsNullOrEmpty(orderId) == false)
		{
			baseUrl += Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL;
			key = Constants.REQUEST_KEY_ORDER_ID;
			value = orderId;
		}
		else if (string.IsNullOrEmpty(fixedPurchaseId) == false)
		{
			baseUrl += Constants.PAGE_FRONT_FIXED_PURCHASE_DETAIL;
			key = Constants.REQUEST_KEY_FIXED_PURCHASE_ID;
			value = fixedPurchaseId;
		}

		var urlCreator = new UrlCreator(baseUrl);
		urlCreator.AddParam(key, value);	
		var url = urlCreator.CreateUrl();
		return url;
	}

	/// <summary>
	/// Amazon Payウィジェット画面のURL作成
	/// </summary>
	/// <param name="isReadOnly">TRUE: 入力ウィジェット FALSE: 詳細ウィジェット</param>
	/// <param name="orderId">注文ID</param>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <returns>ウィジェット画面URL</returns>
	public static string CreateAmazonPayWidgetUrl(bool isReadOnly, string orderId = null, string fixedPurchaseId = null)
	{
		var baseUrl = Constants.PATH_ROOT + (isReadOnly
			? Constants.PAGE_FRONT_COMMON_AMAZON_PAY_DETAIL_WIDGET
			: Constants.PAGE_FRONT_COMMON_AMAZON_PAY_INPUT_WIDGET);
		var urlCreator = new UrlCreator(baseUrl);
		if (string.IsNullOrEmpty(orderId) == false)
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId);
		}
		else if (string.IsNullOrEmpty(fixedPurchaseId) == false)
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_FIXED_PURCHASE_ID, fixedPurchaseId);
		}
		var url = urlCreator.CreateUrl();
		return url;
	}

	/// <summary>URL
	/// エラー画面URLの作成
	/// </summary>
	/// <param name="backUrl">戻り用URL</param>
	/// <returns>エラー画面URL</returns>
	public static string CreateErrorPageUrl(string backUrl)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR).AddParam(Constants.REQUEST_KEY_BACK_URL, backUrl).CreateUrl();
		return url;
	}
}