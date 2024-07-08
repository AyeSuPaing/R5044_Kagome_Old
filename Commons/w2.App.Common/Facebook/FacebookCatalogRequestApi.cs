/*
=========================================================================================================
  Module      : Facebook Catalog Request Api (FacebookCatalogRequestApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace w2.App.Common.Facebook
{
	/// <summary>
	/// Facebook Catalog Request Api
	/// </summary>
	[Serializable]
	public class FacebookCatalogRequestApi
	{
		/// <summary>Access token</summary>
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }
		/// <summary>Requests</summary>
		[JsonProperty("requests")]
		public FacebookCatalogRequestFieldApi[] Requests { get; set; }
	}

	/// <summary>
	/// Facebook Catalog Request Field Api
	/// </summary>
	[Serializable]
	public class FacebookCatalogRequestFieldApi
	{
		/// <summary>Method</summary>
		[JsonProperty("method")]
		public string Method { get; set; }
		/// <summary>Retailer id</summary>
		[JsonProperty("retailer_id")]
		public string RetailerId { get; set; }
		/// <summary>Data</summary>
		[JsonProperty("data")]
		public FacebookCatalogDataRequestApi Data { get; set; }
	}

	/// <summary>
	/// Facebook Catalog Data Request Api
	/// </summary>
	[Serializable]
	public class FacebookCatalogDataRequestApi
	{
		/// <summary>Additional image urls</summary>
		[JsonProperty("additional_image_urls")]
		public List<string> AdditionalImageUrls { get; set; }
		/// <summary>Color</summary>
		[JsonProperty("color")]
		public string Color { get; set; }
		/// <summary>Condition</summary>
		[JsonProperty("condition")]
		public string Condition { get; set; }
		/// <summary>Currency</summary>
		[JsonProperty("currency")]
		public string Currency { get; set; }
		/// <summary>Custom label 0</summary>
		[JsonProperty("custom_label_0")]
		public string CustomLabel0 { get; set; }
		/// <summary>Custom label 1</summary>
		[JsonProperty("custom_label_1")]
		public string CustomLabel1 { get; set; }
		/// <summary>Custom label 2</summary>
		[JsonProperty("custom_label_2")]
		public string CustomLabel2 { get; set; }
		/// <summary>Custom label 3</summary>
		[JsonProperty("custom_label_3")]
		public string CustomLabel3 { get; set; }
		/// <summary>Custom label 4</summary>
		[JsonProperty("custom_label_4")]
		public string CustomLabel4 { get; set; }
		/// <summary>Description</summary>
		[JsonProperty("description")]
		public string Description { get; set; }
		/// <summary>Gender</summary>
		[JsonProperty("gender")]
		public string Gender { get; set; }
		/// <summary>Gtin</summary>
		[JsonProperty("gtin")]
		public string Gtin { get; set; }
		/// <summary>Brand</summary>
		[JsonProperty("brand")]
		public string Brand { get; set; }
		/// <summary>Image url</summary>
		[JsonProperty("image_url")]
		public string ImageUrl { get; set; }
		/// <summary>Inventory</summary>
		[JsonProperty("inventory")]
		public int? Inventory { get; set; }
		/// <summary>Name</summary>
		[JsonProperty("name")]
		public string Name { get; set; }
		/// <summary>Pattern</summary>
		[JsonProperty("pattern")]
		public string Pattern { get; set; }
		/// <summary>Price</summary>
		[JsonProperty("price")]
		public int Price { get; set; }
		/// <summary>Product type</summary>
		[JsonProperty("product_type")]
		public string ProductType { get; set; }
		/// <summary>Retailer product group id</summary>
		[JsonProperty("retailer_product_group_id")]
		public string RetailerProductGroupId { get; set; }
		/// <summary>Sale price</summary>
		[JsonProperty("sale_price")]
		public int? SalePrice { get; set; }
		/// <summary>Sale price start date</summary>
		[JsonProperty("sale_price_start_date")]
		public string SalePriceStartDate { get; set; }
		/// <summary>Sale price end date</summary>
		[JsonProperty("sale_price_end_date")]
		public string SalePriceEndDate { get; set; }
		/// <summary>Size</summary>
		[JsonProperty("size")]
		public string Size { get; set; }
		/// <summary>Url</summary>
		[JsonProperty("url")]
		public string Url { get; set; }
		/// <summary>Vendor id</summary>
		[JsonProperty("vendor_id")]
		public string VendorId { get; set; }
	}
}