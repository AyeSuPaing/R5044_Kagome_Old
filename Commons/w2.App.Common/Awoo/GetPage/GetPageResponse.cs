/*
=========================================================================================================
  Module      : GetPageResponse (GetPageResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace w2.App.Common.Awoo.GetPage
{
	/// <summary>
	/// GetPageResponse
	/// </summary>
	[Serializable]
	public class GetPageResponse
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
		/// <summary>pageInfo</summary>
		[JsonProperty("pageInfo")]
		public PageInfo PageInfo { get; set; }
		/// <summary>categoriesTags</summary>
		[JsonProperty("categoriesTags")]
		public List<string> CategoriesTags { get; set; }
		/// <summary>suggestionTags</summary>
		[JsonProperty("suggestionTags")]
		public List<SuggestionTags> SuggestionTags { get; set; }
		/// <summary>products</summary>
		[JsonProperty("products")]
		public List<Products> Products { get; set; }
		/// <summary>productsTotal</summary>
		[JsonProperty("productsTotal")]
		public int ProductsTotal { get; set; }
	}

	/// <summary>
	/// PageInfo
	/// </summary>
	[Serializable]
	public class PageInfo
	{
		/// <summary>title</summary>
		[JsonProperty("title")]
		public string Title { get; set; }
		/// <summary>description</summary>
		[JsonProperty("description")]
		public string Description { get; set; }
		/// <summary>openGraph</summary>
		[JsonProperty("openGraph")]
		public OpenGraph OpenGraph { get; set; }
		/// <summary>canonical</summary>
		[JsonProperty("canonical")]
		public string Canonical { get; set; }
		/// <summary>schema</summary>
		[JsonProperty("schema")]
		public Schema Schema { get; set; }
		/// <summary>intro</summary>
		[JsonProperty("intro")]
		public string Intro { get; set; }
		/// <summary>h1</summary>
		[JsonProperty("h1")]
		public string H1 { get; set; }
	}

	/// <summary>
	/// OpenGraph
	/// </summary>
	[Serializable]
	public class OpenGraph
	{
		/// <summary>title</summary>
		[JsonProperty("title")]
		public string Title { get; set; }
		/// <summary>description</summary>
		[JsonProperty("description")]
		public string Description { get; set; }
		/// <summary>siteName</summary>
		[JsonProperty("siteName")]
		public string SiteName { get; set; }
		/// <summary>url</summary>
		[JsonProperty("url")]
		public string Url { get; set; }
		/// <summary>image</summary>
		[JsonProperty("image")]
		public string Image { get; set; }
		/// <summary>type</summary>
		[JsonProperty("type")]
		public string Type { get; set; }
	}

	/// <summary>
	/// Schema
	/// </summary>
	[Serializable]
	public class Schema
	{
		/// <summary>breadcrumbs</summary>
		[JsonProperty("breadcrumbs")]
		public string Breadcrumbs { get; set; }
	}

	/// <summary>
	/// SuggestionTags
	/// </summary>
	[Serializable]
	public class SuggestionTags
	{
		/// <summary>text</summary>
		[JsonProperty("text")]
		public string Text { get; set; }
		/// <summary>link</summary>
		[JsonProperty("link")]
		public string Link { get; set; }
	}

	/// <summary>
	/// Products
	/// </summary>
	[Serializable]
	public class Products
	{
		/// <summary>productId</summary>
		[JsonProperty("productId")]
		public string ProductId { get; set; }
		/// <summary>productName</summary>
		[JsonProperty("productName")]
		public string ProductName { get; set; }
		/// <summary>url</summary>
		[JsonProperty("url")]
		public string Url { get; set; }
		/// <summary>productImageUrl</summary>
		[JsonProperty("productImageUrl")]
		public string ProductImageUrl { get; set; }
		/// <summary>productBrand</summary>
		[JsonProperty("productBrand")]
		public string ProductBrand { get; set; }
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
	}
}
