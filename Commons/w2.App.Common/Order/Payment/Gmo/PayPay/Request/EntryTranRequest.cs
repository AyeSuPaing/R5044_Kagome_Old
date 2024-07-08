/*
=========================================================================================================
  Module      : 取引登録リクエスト (EntryTranRequest.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Paypay.Request
{
	/// <summary>
	/// 取引登録リクエスト
	/// </summary>
	public class EntryTranRequest : PaypayGmoRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public EntryTranRequest()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="isExecOrderNow">Is exec order now</param>
		public EntryTranRequest(OrderModel order, bool isExecOrderNow = false)
		{
			this.Amount = order.OrderPriceTotal.ToString("0");
			this.PaymentOrderId = (order.IsFixedPurchaseOrder
					&& (isExecOrderNow == false))
				? order.FixedPurchaseId
				: order.PaymentOrderId;
			this.JobCode = isExecOrderNow
				? PaypayConstants.FLG_PAYPAY_STATUS_CAPTURE
				: Constants.PAYMENT_PAYPAY_JOB_CODE;
			this.PaymentType = isExecOrderNow
				? PaypayConstants.FLG_PAYPAY_PAYMENT_TYPE
				: string.Empty;
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
		/// <summary>処理区分</summary>
		[JsonProperty("jobCd")]
		public string JobCode { get; set; }
		/// <summary>注文ID（ｗ２の決済注文ID）</summary>
		[JsonProperty("orderID")]
		public string PaymentOrderId { get; set; }
		/// <summary>利用金額</summary>
		[JsonProperty("amount")]
		public string Amount { get; set; }
		/// <summary>決済タイプ</summary>
		[JsonProperty("paymentType")]
		public string PaymentType { get; set; }
	}
}
