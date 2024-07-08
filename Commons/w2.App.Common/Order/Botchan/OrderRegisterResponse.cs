/*
=========================================================================================================
  Module      : Order Register Response(OrderRegisterResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace w2.App.Common.Order.Botchan
{
	/// <summary>
	/// Order register response
	/// </summary>
	[Serializable]
	public class OrderRegisterResponse
	{
		/// <summary>Result</summary>
		[JsonProperty("result")]
		public bool Result { get; set; }
		/// <summary>Message id</summary>
		[JsonProperty("message_id")]
		public string MessageId { set; get; }
		/// <summary>Message</summary>
		[JsonProperty("message")]
		public string Message { set; get; }
		/// <summary>DataResult</summary>
		[JsonProperty("data")]
		public Data DataResult { get; set; }

		/// <summary>
		/// DataResult
		/// </summary>
		[Serializable]
		public class Data
		{
			/// <summary>Order id</summary>
			[JsonProperty("order_id")]
			public string OrderId { get; set; }
			/// <summary>Order product list</summary>
			[JsonProperty("order_product_list")]
			public OrderProductList[] OrderProducts { get; set; }
			/// <summary>Status</summary>
			[JsonProperty("status")]
			public string Status { get; set; }
			/// <summary>クレジットカード登録名補完フラグ</summary>
			[JsonProperty("credit_name_complement_flg")]
			public string CreditNameComplementFlg { get; set; }
		}

		/// <summary>
		/// Order product list object
		/// </summary>
		[Serializable]
		public class OrderProductList
		{
			/// <summary>Product id</summary>
			[DefaultValue("")]
			[JsonProperty("product_id")]
			public string ProductId { get; set; }
			/// <summary>Variation id</summary>
			[DefaultValue("")]
			[JsonProperty("variation_id")]
			public string VariationId { get; set; }
			/// <summary>Item quantity</summary>
			[DefaultValue("")]
			[JsonProperty("item_quantity")]
			public string ItemQuantity { get; set; }
			/// <summary>Product sale id</summary>
			[DefaultValue("")]
			[JsonProperty("product_sale_id")]
			public string ProductSaleId { get; set; }
			/// <summary>Product name</summary>
			[DefaultValue("")]
			[JsonProperty("product_name")]
			public string ProductName { get; set; }
			/// <summary>Item price</summary>
			[DefaultValue("")]
			[JsonProperty("item_price")]
			public string ItemPrice { get; set; }
			/// <summary>Product tax rate</summary>
			[DefaultValue("")]
			[JsonProperty("product_tax_rate")]
			public string ProductTaxRate { get; set; }
			/// <summary>定期商品か</summary>
			[DefaultValue("")]
			[JsonProperty("is_fixed_purchase")]
			public bool IsFixedPurchase { get; set; }
		}
	}
}
