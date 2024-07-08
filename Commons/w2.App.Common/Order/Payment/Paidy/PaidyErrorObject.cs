/*
=========================================================================================================
  Module      : Paidy Error Object (PaidyErrorObject.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace w2.App.Common.Order.Payment.Paidy
{
	/// <summary>
	/// Paidy error object
	/// </summary>
	[Serializable]
	public class PaidyErrorObject
	{
		/// <summary>Reference</summary>
		[JsonProperty(PropertyName = "reference")]
		public string Reference { get; set; }
		/// <summary>Status</summary>
		[JsonProperty(PropertyName = "status")]
		public string Status { get; set; }
		/// <summary>Code</summary>
		[JsonProperty(PropertyName = "code")]
		public string Code { get; set; }
		/// <summary>Title</summary>
		[JsonProperty(PropertyName = "title")]
		public string Title { get; set; }
		/// <summary>Description</summary>
		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }
		/// <summary>Details</summary>
		[JsonProperty(PropertyName = "details")]
		public List<PaidyErrorDetailsObject> Details { get; set; }

		/// <summary>
		/// Paidy error details object
		/// </summary>
		[Serializable]
		public class PaidyErrorDetailsObject
		{
			/// <summary>Kind</summary>
			[JsonProperty(PropertyName = "kind")]
			public string Kind { get; set; }
			/// <summary>Field</summary>
			[JsonProperty(PropertyName = "field")]
			public string Field { get; set; }
			/// <summary>Message</summary>
			[JsonProperty(PropertyName = "message")]
			public string Message { get; set; }
		}
	}
}