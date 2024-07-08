/*
=========================================================================================================
  Module      : Product Search Request(ProductSearchRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Botchan
{
	/// <summary>
	/// Product search request
	/// </summary>
	[Serializable]
	public class ProductSearchRequest
	{
		/// <summary>Search product list</summary>
		[JsonProperty("search_list")]
		public SearchProduct[] SearchProductList { get; set; }
		/// <summary>Auth text</summary>
		[JsonProperty("auth_text")]
		public string AuthText { get; set; }
	}

	/// <summary>
	/// Search product list
	/// </summary>
	[JsonObject("search_list")]
	public class SearchProduct
	{
		/// <summary>Product id</summary>
		[JsonProperty("product_id")]
		public string ProductId { get; set; }
	}
}