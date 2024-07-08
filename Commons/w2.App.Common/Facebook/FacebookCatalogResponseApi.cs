/*
=========================================================================================================
  Module      : Facebook Catalog Response Api (FacebookCatalogResponseApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Facebook
{
	/// <summary>
	/// Facebook Catalog Response Api
	/// </summary>
	[Serializable]
	public class FacebookCatalogResponseApi
	{
		/// <summary>Handles</summary>
		[JsonProperty("handles")]
		public string[] Handles { get; set; }
		/// <summary>Errors</summary>
		[JsonProperty("error")]
		public FacebookCatalogErrorApi Error { get; set; }
		/// <summary>Validation statuses</summary>
		[JsonProperty("validation_status")]
		public FacebookCatalogValidationStatusApi[] ValidationStatuses { get; set; }
	}

	/// <summary>
	/// Facebook Catalog Error Api
	/// </summary>
	[Serializable]
	public class FacebookCatalogErrorApi
	{
		/// <summary>Message</summary>
		[JsonProperty("message")]
		public string Message { get; set; }
		/// <summary>Type</summary>
		[JsonProperty("type")]
		public string Type { get; set; }
		/// <summary>Code</summary>
		[JsonProperty("code")]
		public int Code { get; set; }
		/// <summary>Fbtrace id</summary>
		[JsonProperty("fbtrace_id")]
		public string FbtraceId { get; set; }
	}

	/// <summary>
	/// Facebook Catalog Validation Status Api
	/// </summary>
	[Serializable]
	public class FacebookCatalogValidationStatusApi
	{
		/// <summary>Retailer id</summary>
		[JsonProperty("retailer_id")]
		public string RetailerId { get; set; }
		/// <summary>Errors</summary>
		[JsonProperty("errors")]
		public Error[] Errors { get; set; }
		/// <summary>Warnings</summary>
		[JsonProperty("warnings")]
		public Warning[] Warnings { get; set; }
	}

	/// <summary>
	/// Error
	/// </summary>
	[Serializable]
	public class Error
	{
		/// <summary>Message</summary>
		[JsonProperty("message")]
		public string Message { get; set; }
	}

	/// <summary>
	/// Warning
	/// </summary>
	[Serializable]
	public class Warning
	{
		/// <summary>Message</summary>
		[JsonProperty("message")]
		public string Message { get; set; }
	}
}