/*
=========================================================================================================
  Module      : 注文同梱入力クラス(OrderCombineInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using w2.App.Common.Order;
using w2.App.Common.Amazon;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;

/// <summary>
/// 注文同梱選択画面入力クラス
/// </summary>
public class OrderCombineSelectInput
{
	/// <summary>
	/// 次ページ遷移先取得
	/// </summary>
	/// <param name="urlRefferrer">URLリファラ</param>
	/// <param name="isCombine">同梱有無</param>
	/// <param name="needInputShippingWhenCombined">同梱時の配送情報の入力が必要か</param>
	/// <param name="parentPaymentKbn">親注文決済種別</param>
	/// <returns>次ページURL</returns>
	public static string GetNextPage(
		Uri urlRefferrer,
		bool isCombine,
		bool needInputShippingWhenCombined,
		string parentPaymentKbn = null)
	{
		var urlRef = (urlRefferrer == null) ? "" : urlRefferrer.ToString().ToLower();
		if (urlRef.Contains(Constants.PAGE_FRONT_ORDER_PAYMENT.ToLower()))
		{
			return Constants.PAGE_FRONT_ORDER_COMBINE_SELECT_LIST;
		}
		else if (urlRef.Contains(Constants.PAGE_FRONT_ORDER_CONFIRM.ToLower()))
		{
			return Constants.PAGE_FRONT_ORDER_CONFIRM;
		}

		return isCombine && (needInputShippingWhenCombined == false)
			? ((parentPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT
					|| parentPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
				&& Constants.AMAZON_PAYMENT_OPTION_ENABLED)
					? Constants.PAGE_FRONT_ORDER_AMAZON_PAYMENT_INPUT
					: Constants.PAGE_FRONT_ORDER_CONFIRM
			: ((HttpContext.Current.Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL] != null)
				&& Constants.AMAZON_PAYMENT_OPTION_ENABLED
				&& SessionManager.OrderCombineFromAmazonPayButton)
					? Constants.PAGE_FRONT_ORDER_AMAZON_PAYMENT_INPUT
					: Constants.PAGE_FRONT_ORDER_SHIPPING;
	}
}
