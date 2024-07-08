/*
=========================================================================================================
  Module      : ClassifyProductTypeResponse (ClassifyProductTypeResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace w2.App.Common.Awoo.ClassifyProductType
{
	/// <summary>
	/// ClassifyProductTypeResponse
	/// </summary>
	[Serializable]
	public class ClassifyProductTypeResponse
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
}
