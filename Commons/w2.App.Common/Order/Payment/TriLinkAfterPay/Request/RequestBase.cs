/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) リクエスト基底クラス(RequestBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System.Collections.Generic;
using w2.App.Common.Global.Region.Currency;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Request
{
	/// <summary>
	/// 後付款(TriLink後払い) リクエスト基底クラス
	/// </summary>
	public abstract class RequestBase
	{
		/// <summary>
		/// 送金額を取得
		/// </summary>
		/// <param name="order">注文</param>
		/// <returns>送金額</returns>
		public static decimal GetSendingAmount(OrderModel order)
		{
			return CurrencyManager.GetSendingAmount(order.LastBilledAmount, order.SettlementAmount, order.SettlementCurrency);
		}

		#region プロパティ
		/// <summary>リクエストURL</summary>
		[JsonIgnore]
		public string RequestUrl { get; protected set; }
		/// <summary>リクエスト追加Headerリスト</summary>
		[JsonIgnore]
		public List<KeyValuePair<string, string>> AddRequestHeaders { get; protected set; }
		#endregion
	}
}