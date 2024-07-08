/*
=========================================================================================================
  Module      :Paidy Checkout 商品情報 (PaidyAuthorizationResponseItems.cs)
  ･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Checkout.Dto
{
	/// <summary>
	/// Paidy Checkout 商品情報
	/// </summary>
	public class PaidyCheckoutResponseItems
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="id">商品ID</param>
		/// <param name="quantity">個数</param>
		/// <param name="title">商品名</param>
		/// <param name="unitPrice">商品単価</param>
		/// <param name="description">商品説明</param>
		public PaidyCheckoutResponseItems(
			string id,
			int quantity,
			string title,
			double unitPrice,
			string description)
		{
			this.Id = id;
			this.Quantity = quantity;
			this.Title = title;
			this.UnitPrice = unitPrice;
			this.Description = description;
		}

		/// <summary>商品ID</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_ID)]
		public string Id { get; private set; }
		/// <summary>個数</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_QUANTITY)]
		public int Quantity { get; private set; }
		/// <summary>商品名</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_TITLE)]
		public string Title { get; private set; }
		/// <summary>商品単価</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_UNIT_PRICE)]
		public double UnitPrice { get; private set; }
		/// <summary>商品説明</summary>
		[JsonProperty(PaygentConstants.PAIDY_CHECKOUT_RESPONSE_DESCRIPTION)]
		public string Description { get; private set; }
	}
}
