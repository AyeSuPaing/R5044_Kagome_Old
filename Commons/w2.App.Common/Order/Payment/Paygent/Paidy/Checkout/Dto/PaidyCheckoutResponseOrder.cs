/*
=========================================================================================================
  Module      :Paidy Checkout 注文情報 (PaidyAuthorizationResponseOrder.cs)
  ･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Checkout.Dto
{
	/// <summary>
	/// Paidy Checkout 注文情報
	/// </summary>
	public class PaidyCheckoutResponseOrder
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="items">購入商品</param>
		/// <param name="orderRef">カートID or 注文ID</param>
		/// <param name="shipping">配送料</param>
		/// <param name="tax">消費税額</param>
		public PaidyCheckoutResponseOrder(
			PaidyCheckoutResponseItems[] items,
			string orderRef,
			double shipping,
			double tax)
		{
			this.Items = items;
			this.OrderRef = orderRef;
			this.Shipping = shipping;
			this.Tax = tax;
		}

		/// <summary>購入商品</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_ITEMS)]
		public PaidyCheckoutResponseItems[] Items { get; private set; }
		/// <summary>カートID or 注文ID</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_ORDER_REF)]
		public string OrderRef { get; private set; }
		/// <summary>配送料</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_SHIPPING)]
		public double Shipping { get; private set; }
		/// <summary>消費税額</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_TAX)]
		public double Tax { get; private set; }
	}
}
