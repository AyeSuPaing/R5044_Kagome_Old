/*
=========================================================================================================
  Module      : 実売上リクエスト (SalesPaymentRequest.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Paypay.Request
{
	/// <summary>
	/// 実売上リクエスト
	/// </summary>
	public class SalesPaymentRequest : PaypayGmoRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SalesPaymentRequest()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		public SalesPaymentRequest(OrderModel order)
		{
			this.AccessId = order.CardTranId;
			this.AccessPass = order.CardTranPass;
			this.PaymentOrderId = order.PaymentOrderId;
			this.Amount = order.OrderPriceTotal.ToString("0");
		}

		/// <summary>店舗ID</summary>
		[JsonProperty("shopID")]
		public string ShopId
		{
			get { return Constants.PAYMENT_PAYPAY_SHOP_ID; }
		}
		/// <summary>店舗パスワード</summary>
		[JsonProperty("shopPass")]
		public string ShopPassword
		{
			get { return Constants.PAYMENT_PAYPAY_SHOP_PASSWORD; }
		}
		/// <summary>取引ID</summary>
		[JsonProperty("accessID")]
		public string AccessId { get; set; }
		/// <summary>取引パスワード</summary>
		[JsonProperty("accessPass")]
		public string AccessPass { get; set; }
		/// <summary>注文ID（ｗ２の決済注文ID）</summary>
		[JsonProperty("orderID")]
		public string PaymentOrderId { get; set; }
		/// <summary>利用金額</summary>
		[JsonProperty("amount")]
		public string Amount { get; set; }
	}
}
