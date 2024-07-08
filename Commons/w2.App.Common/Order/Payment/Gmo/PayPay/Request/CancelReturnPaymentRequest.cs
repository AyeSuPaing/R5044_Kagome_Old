/*
=========================================================================================================
  Module      : キャンセルリクエスト (CancelReturnPaymentRequest.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Paypay.Request
{
	/// <summary>
	/// キャンセルリクエスト
	/// </summary>
	public class CancelReturnPaymentRequest : PaypayGmoRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CancelReturnPaymentRequest()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		public CancelReturnPaymentRequest(OrderModel order)
		{
			this.AccessId = order.CardTranId;
			this.AccessPass = order.CardTranPass;
			this.PaymentOrderId = order.PaymentOrderId;
			this.CancelAmount = order.OrderPriceTotal.ToString("0");
		}

		/// <summary>
		/// コンストラクタ（一部返金）
		/// </summary>
		/// <param name="order"> Order model</param>
		/// <param name="refundAmount">Refund amount</param>
		public CancelReturnPaymentRequest(OrderModel order, decimal refundAmount)
		{
			this.PaymentOrderId = order.PaymentOrderId;
			this.AccessId = order.CardTranId;
			this.AccessPass = order.CardTranPass;
			this.CancelAmount = refundAmount.ToString("0");
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
		[JsonProperty("cancelAmount")]
		public string CancelAmount { get; set; }
	}
}
