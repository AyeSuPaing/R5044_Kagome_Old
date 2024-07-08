/*
=========================================================================================================
  Module      : GetTagsResponse (FacebookCatalogResponseApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace w2.App.Common.Awoo.GetTags
{
	/// <summary>
	/// GetTagsResponse
	/// </summary>
	[Serializable]
	public class GetTagsResponse
	{
		/// <summary>errcode</summary>
		[JsonProperty("errcode")]
		public int Errcode { get; set; }
		/// <summary>errmsg</summary>
		[JsonProperty("errmsg")]
		public string Errmsg { get; set; }
		/// <summary>result</summary>
		[JsonProperty("result")]
		public Result Result { get; set; }
	}

	/// <summary>
	/// Result
	/// </summary>
	[Serializable]
	public class Result
	{
		/// <summary>tags</summary>
		[JsonProperty("tags")]
		public List<Tags> Tags { get; set; }
		/// <summary>products</summary>
		[JsonProperty("products")]
		public Products Products { get; set; }
	}

	/// <summary>
	/// Tags
	/// </summary>
	[Serializable]
	public class Tags
	{
		/// <summary>text</summary>
		[JsonProperty("text")]
		public string Text { get; set; }
		/// <summary>link</summary>
		[JsonProperty("link")]
		public string Link { get; set; }
		/// <summary>fullLink</summary>
		[JsonProperty("fullLink")]
		public string FullLink { get; set; }
	}

	/// <summary>
	/// Products
	/// </summary>
	[Serializable]
	public class Products
	{
		/// <summary>directionsv</summary>
		[JsonProperty("directions:v")]
		public DirectionProducts DirectionsV { get; set; }
		/// <summary>directionsh</summary>
		[JsonProperty("directions:h")]
		public DirectionProducts DirectionsH { get; set; }
	}

	/// <summary>
	/// DirectionProducts
	/// </summary>
	[Serializable]
	public class DirectionProducts
	{
		/// <summary>products</summary>
		[JsonProperty("products")]
		public List<ProductDetail> Products { get; set; }

	}

	/// <summary>
	/// ProductDetail
	/// </summary>
	[Serializable]
	public class ProductDetail
	{
		/// <summary>productId</summary>
		[JsonProperty("productId")]
		public string ProductId { get; set; }
		/// <summary>url</summary>
		[JsonProperty("url")]
		public string Url { get; set; }
		/// <summary>productImageUrl</summary>
		[JsonProperty("productImageUrl")]
		public string ProductImageUrl { get; set; }
		/// <summary>productName</summary>
		[JsonProperty("productName")]
		public string ProductName { get; set; }
		/// <summary>productPrice</summary>
		[JsonProperty("productPrice")]
		public int ProductPrice { get; set; }
		/// <summary>productPriceCurrency</summary>
		[JsonProperty("productPriceCurrency")]
		public string ProductPriceCurrency { get; set; }
		/// <summary>productSalePrice</summary>
		[JsonProperty("productSalePrice")]
		public int ProductSalePrice { get; set; }
		/// <summary>productSalePriceCurrency</summary>
		[JsonProperty("productSalePriceCurrency")]
		public string ProductSalePriceCurrency { get; set; }
		/// <summary>productType</summary>
		[JsonProperty("productType")]
		public string ProductType { get; set; }
		/// <summary>productBrand</summary>
		[JsonProperty("productBrand")]
		public string ProductBrand { get; set; }
	}
}
