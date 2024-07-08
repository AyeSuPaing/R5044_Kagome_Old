/*
=========================================================================================================
  Module      : 取引照会リクエスト (SearchTradeRequest.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Paypay.Request
{
	/// <summary>
	/// 取引照会リクエスト
	/// </summary>
	public class SearchTradeRequest : PaypayGmoRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SearchTradeRequest()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		public SearchTradeRequest(OrderModel order)
		{
			this.PaymentOrderId = order.PaymentOrderId;
		}

		/// <summary>店舗ID</summary>
		[IdPassProperty("ShopID")]
		public string ShopId
		{
			get { return Constants.PAYMENT_PAYPAY_SHOP_ID; }
		}
		/// <summary>店舗パスワード</summary>
		[IdPassProperty("ShopPass")]
		public string ShopPassword
		{
			get { return Constants.PAYMENT_PAYPAY_SHOP_PASSWORD; }
		}
		/// <summary>注文ID（ｗ２側の決済注文ID）</summary>
		[IdPassProperty("OrderID")]
		public string PaymentOrderId { get; set; }
		/// <summary>決済方法</summary>
		[IdPassProperty("PayType")]
		public string PayType
		{
			get { return PaypayConstants.FLG_PAYTYPE_PAYPAY; }
		}
		/// <summary>API種別</summary>
		[IdPassIgnore]
		[JsonIgnore]
		protected override ApiType ApiType
		{
			get { return ApiType.IdPass; }
		}
	}
}
