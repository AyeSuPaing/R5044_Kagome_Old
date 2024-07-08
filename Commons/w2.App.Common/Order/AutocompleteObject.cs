/*
=========================================================================================================
  Module      : Autocomplete Object (AutocompleteObject.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order
{
	/// <summary>
	/// Autocomplete object class
	/// </summary>
	public class AutocompleteObject
	{
		/// <summary>
		/// Advertisement code object
		/// </summary>
		[Serializable]
		public class AdvCodeObject
		{
			/// <summary>Id</summary>
			[JsonProperty("id")]
			public string Id { get; set; }
			/// <summary>Name</summary>
			[JsonProperty("name")]
			public string Name { get; set; }
		}

		/// <summary>
		/// User object
		/// </summary>
		[Serializable]
		public class UserObject
		{
			/// <summary>Id</summary>
			[JsonProperty("id")]
			public string Id { get; set; }
			/// <summary>Name</summary>
			[JsonProperty("name")]
			public string Name { get; set; }
			/// <summary>Phone</summary>
			[JsonProperty("phone")]
			public string Phone { get; set; }
			/// <summary>Address</summary>
			[JsonProperty("address")]
			public string Address { get; set; }
		}

		/// <summary>
		/// Product object
		/// </summary>
		[Serializable]
		public class ProductObject
		{
			/// <summary>Product Id</summary>
			[JsonProperty("id")]
			public string ProductId { get; set; }
			/// <summary>Supplier Id</summary>
			[JsonProperty("supplierId")]
			public string SupplierId { get; set; }
			/// <summary>Variation Id</summary>
			[JsonProperty("variationId")]
			public string VariationId { get; set; }
			/// <summary>Name</summary>
			[JsonProperty("name")]
			public string Name { get; set; }
			/// <summary>Display price</summary>
			[JsonProperty("displayPrice")]
			public string DisplayPrice { get; set; }
			/// <summary>Special price</summary>
			[JsonProperty("specialPrice")]
			public string SpecialPrice { get; set; }
			/// <summary>Unit price</summary>
			[JsonProperty("unitPrice")]
			public string UnitPrice { get; set; }
			/// <summary>Unit price by key currency</summary>
			[JsonProperty("unitPriceByKeyCurrency")]
			public string UnitPriceByKeyCurrency { get; set; }
			/// <summary>Sale Id</summary>
			[JsonProperty("saleId")]
			public string SaleId { get; set; }
			/// <summary>Fixed purchase Id</summary>
			[JsonProperty("fixedPurchaseId")]
			public string FixedPurchaseId { get; set; }
			/// <summary>Limited fixed purchase kbn 1 setting</summary>
			[JsonProperty("limitedFixedPurchaseKbn1Setting")]
			public string LimitedFixedPurchaseKbn1Setting { get; set; }
			/// <summary>Limited fixedpur chase kbn 3 setting</summary>
			[JsonProperty("limitedFixedPurchaseKbn3Setting")]
			public string LimitedFixedPurchaseKbn3Setting { get; set; }
			/// <summary>Quantity</summary>
			[JsonProperty("quantity")]
			public string Quantity { get; set; }
		}

		/// <summary>
		/// Coupon object
		/// </summary>
		[Serializable]
		public class CouponObject
		{
			/// <summary>Id</summary>
			[JsonProperty("id")]
			public string Id { get; set; }
			/// <summary>Name</summary>
			[JsonProperty("name")]
			public string Name { get; set; }
			/// <summary>Type</summary>
			[JsonProperty("type")]
			public string Type { get; set; }
			/// <summary>Discount</summary>
			[JsonProperty("discount")]
			public string Discount { get; set; }
			/// <summary>Expire date</summary>
			[JsonProperty("expireDate")]
			public string ExpirationDate { get; set; }
		}
	}
}
