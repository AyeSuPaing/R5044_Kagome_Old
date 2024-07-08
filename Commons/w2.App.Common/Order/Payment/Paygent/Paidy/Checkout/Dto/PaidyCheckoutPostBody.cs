/*
=========================================================================================================
  Module      :Paidy Checkout 送信モデル (PaidyCheckoutPostBody.cs)
  ･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Checkout.Dto
{
	/// <summary>
	/// Paidy Checkout 送信モデル
	/// </summary>
	public class PaidyCheckoutPostBody
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="amount">決済総額</param>
		/// <param name="currency">通貨コード</param>
		/// <param name="storeName">店舗名</param>
		/// <param name="buyer">購入者情報</param>
		/// <param name="buyerData">購入者履歴情報</param>
		/// <param name="order">注文情報</param>
		/// <param name="shippingAddress">配送先情報</param>
		/// <param name="description">注文概要</param>
		public PaidyCheckoutPostBody(
			double amount,
			string currency,
			string storeName,
			PaidyCheckoutResponseBuyer buyer,
			PaidyCheckoutResponseBuyerData buyerData,
			PaidyCheckoutResponseOrder order,
			PaidyCheckoutResponseShippingAddress shippingAddress,
			string description)
		{
			this.Amount = amount;
			this.Currency = currency;
			this.StoreName = storeName;
			this.Buyer = buyer;
			this.BuyerData = buyerData;
			this.Order = order;
			this.ShippingAddress = shippingAddress;
			this.Description = description;
		}

		/// <summary>決済総額</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_AMOUNT)]
		public double Amount { get; private set; }
		/// <summary>通貨コード</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_CURRENCY)]
		public string Currency { get; private set; }
		/// <summary>店舗名</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_STORE_NAME)]
		public string StoreName { get; private set; }
		/// <summary>購入者情報</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_BUYER)]
		public PaidyCheckoutResponseBuyer Buyer { get; private set; }
		/// <summary>購入者履歴情報</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_BUYER_DATA)]
		public PaidyCheckoutResponseBuyerData BuyerData { get; private set; }
		/// <summary>注文情報</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_ORDER)]
		public PaidyCheckoutResponseOrder Order { get; private set; }
		/// <summary>配送先情報</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_SHIPPING_ADDRESS)]
		public PaidyCheckoutResponseShippingAddress ShippingAddress { get; private set; }
		/// <summary>注文概要</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_DESCRIPTION)]
		public string Description { get; private set; }
	}
}
