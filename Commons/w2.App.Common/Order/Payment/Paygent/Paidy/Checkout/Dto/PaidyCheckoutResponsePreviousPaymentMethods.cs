/*
=========================================================================================================
  Module      :Paidy Checkout 過去使用決済種別 (PaidyAuthorizationResponsePreviousPaymentMethods.cs)
  ･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Checkout.Dto
{
	/// <summary>
	/// Paidy Checkout 過去使用決済種別
	/// </summary>
	public class PaidyCheckoutResponsePreviousPaymentMethods
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="creditCardUsed">クレジット決済を利用したことがあるか</param>
		/// <param name="cashOnDeliveryUsed">代金引換を利用したことがあるか</param>
		/// <param name="convenienceStorePrepaymentUsed">コンビニ決済を利用したことがあるか</param>
		/// <param name="carrierPaymentUsed">キャリア決済を利用したことがあるか</param>
		/// <param name="bankTransferUsed">銀行振り込みを利用したことがあるか</param>
		/// <param name="rakutenPayUsed">楽天Payを利用したことがあるか</param>
		/// <param name="linePayUsed">LINEPayを利用したことがあるか</param>
		/// <param name="amazonPayUsed">AmazonPayを利用したことがあるか</param>
		/// <param name="npPostpayUsed">NP後払いを利用したことがあるか</param>
		/// <param name="otherPostpayUsed">その他後払い決済を利用したことがあるか</param>
		public PaidyCheckoutResponsePreviousPaymentMethods(
			bool creditCardUsed,
			bool cashOnDeliveryUsed,
			bool convenienceStorePrepaymentUsed,
			bool carrierPaymentUsed,
			bool bankTransferUsed,
			bool rakutenPayUsed,
			bool linePayUsed,
			bool amazonPayUsed,
			bool npPostpayUsed,
			bool otherPostpayUsed)
		{
			this.CreditCardUsed = creditCardUsed;
			this.CashOnDeliveryUsed = cashOnDeliveryUsed;
			this.ConvenienceStorePrepaymentUsed = convenienceStorePrepaymentUsed;
			this.CarrierPaymentUsed = carrierPaymentUsed;
			this.BankTransferUsed = bankTransferUsed;
			this.RakutenPayUsed = rakutenPayUsed;
			this.LinePayUsed = linePayUsed;
			this.AmazonPayUsed = amazonPayUsed;
			this.NpPostpayUsed = npPostpayUsed;
			this.OtherPostpayUsed = otherPostpayUsed;
		}

		/// <summary>クレジット決済を利用したことがあるか</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_CREDIT_CARD_USED)]
		public bool CreditCardUsed { get; private set; }
		/// <summary>代金引換を利用したことがあるか</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_CASH_ON_DELIVERY_USED)]
		public bool CashOnDeliveryUsed { get; private set; }
		/// <summary>コンビニ決済を利用したことがあるか</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_CONVENIENCE_STORE_PREPAYMENT_USED)]
		public bool ConvenienceStorePrepaymentUsed { get; private set; }
		/// <summary>キャリア決済を利用したことがあるか</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_CARRIER_PAYMENT_USED)]
		public bool CarrierPaymentUsed { get; private set; }
		/// <summary>銀行振り込みを利用したことがあるか</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_BANK_TRANSFER_USED)]
		public bool BankTransferUsed { get; private set; }
		/// <summary>楽天Payを利用したことがあるか</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_RAKUTEN_PAY_USED)]
		public bool RakutenPayUsed { get; private set; }
		/// <summary>LINEPayを利用したことがあるか</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_LINE_PAY_USED)]
		public bool LinePayUsed { get; private set; }
		/// <summary>AmazonPayを利用したことがあるか</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_AMAZON_PAY_USED)]
		public bool AmazonPayUsed { get; private set; }
		/// <summary>NP後払いを利用したことがあるか</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_NP_POSTPAY_USED)]
		public bool NpPostpayUsed { get; private set; }
		/// <summary>その他後払い決済を利用したことがあるか</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_OTHER_POSTPAY_USED)]
		public bool OtherPostpayUsed { get; private set; }
	}
}
