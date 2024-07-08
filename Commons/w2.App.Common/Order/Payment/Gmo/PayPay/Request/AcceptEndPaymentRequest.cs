/*
=========================================================================================================
  Module      : Accept End Payment Request (AcceptEndPaymentRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paypay.Request
{
	/// <summary>
	/// Accept end payment request
	/// </summary>
	public class AcceptEndPaymentRequest : PaypayGmoRequest
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cardTranId">Card tran id</param>
		/// <param name="cardTranPass">Card tran pass</param>
		/// <param name="paymentOrderId">payment order id</param>
		/// <param name="externalPaymentAgreementId">External payment agreement id</param>
		public AcceptEndPaymentRequest(
			string cardTranId,
			string cardTranPass,
			string paymentOrderId,
			string externalPaymentAgreementId)
		{
			this.AccessId = cardTranId;
			this.AccessPass = cardTranPass;
			this.OrderId = paymentOrderId;
			this.PaypayAcceptCode = externalPaymentAgreementId;
		}

		/// <summary>ショップID</summary>
		[JsonProperty("shopID")]
		public string ShopId
		{
			get { return Constants.PAYMENT_PAYPAY_SHOP_ID; }
		}
		/// <summary>マーチャントID</summary>
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
		/// <summary>オーダーID</summary>
		[JsonProperty("orderID")]
		public string OrderId { get; set; }
		/// <summary>PayPay承諾番号</summary>
		[JsonProperty("paypayAcceptCode")]
		public string PaypayAcceptCode { get; set; }
	}
}
