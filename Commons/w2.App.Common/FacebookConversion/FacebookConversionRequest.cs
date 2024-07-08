/*
=========================================================================================================
  Module      : Facebook Conversion Request (FacebookConversionRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace w2.App.Common.FacebookConversion
{
	/// <summary>
	/// Facebook conversion request
	/// </summary>
	[Serializable]
	public class FacebookConversionRequest
	{
		/// <summary>Data</summary>
		[JsonProperty("data")]
		public FacebookConversionDataRequest[] Data { get; set; }
		/// <summary>Test event code</summary>
		[JsonProperty("test_event_code")]
		public string TestEventCode { get; set; }
	}

	/// <summary>
	/// Facebook conversion data request
	/// </summary>
	[Serializable]
	public class FacebookConversionDataRequest
	{
		/// <summary>Event name</summary>
		[JsonProperty("event_name")]
		public string EventName { get; set; }
		/// <summary>Event time</summary>
		[JsonProperty("event_time")]
		public int EventTime { get; set; }
		/// <summary>User data</summary>
		[JsonProperty("user_data")]
		public UserData UserDataFacebook { get; set; }
		/// <summary>Custom data</summary>
		[JsonProperty("custom_data")]
		public CustomData CustomDataFacebook { get; set; }
		/// <summary>Event source url</summary>
		[JsonProperty("event_source_url")]
		public string EventSourceUrl { get; set; }
		/// <summary>Opt out</summary>
		[JsonProperty("opt_out")]
		public bool OptOut { get; set; }
		/// <summary>Event id</summary>
		[JsonProperty("event_id")]
		public string EventId { get; set; }
		/// <summary>Action source</summary>
		[JsonProperty("action_source")]
		public string ActionSource { get; set; }
		/// <summary>Data processing options</summary>
		[JsonProperty("data_processing_options")]
		public string[] DataProcessingOptions { get; set; }
		/// <summary>Data processing options country</summary>
		[JsonProperty("data_processing_options_country")]
		public int? DataProcessiongOptionsCountry { get; set; }
		/// <summary>Data processing options state</summary>
		[JsonProperty("data_processing_options_state")]
		public int? DataProcessingOptionsState { get; set; }

		/// <summary>
		/// User data
		/// </summary>
		[Serializable]
		public class UserData
		{
			/// <summary>Mail address</summary>
			[JsonProperty("em")]
			public string MailAddress { get; set; }
			/// <summary>Phone</summary>
			[JsonProperty("ph")]
			public string Phone { get; set; }
			/// <summary>Gender</summary>
			[JsonProperty("ge")]
			public string Gender { get; set; }
			/// <summary>Birthday</summary>
			[JsonProperty("db")]
			public string BirthDay { get; set; }
			/// <summary>Last name</summary>
			[JsonProperty("ln")]
			public string LastName { get; set; }
			/// <summary>First name</summary>
			[JsonProperty("fn")]
			public string FirstName { get; set; }
			/// <summary>City</summary>
			[JsonProperty("ct")]
			public string City { get; set; }
			/// <summary>State</summary>
			[JsonProperty("st")]
			public string State { get; set; }
			/// <summary>Zip</summary>
			[JsonProperty("zp")]
			public string Zip { get; set; }
			/// <summary>Country</summary>
			[JsonProperty("country")]
			public string Country { get; set; }
			/// <summary>External id</summary>
			[JsonProperty("external_id")]
			public string ExternalId { get; set; }
			/// <summary>Client ip address</summary>
			[JsonProperty("client_ip_address")]
			public string ClientIpAddress { get; set; }
			/// <summary>Client user agent</summary>
			[JsonProperty("client_user_agent")]
			public string ClientUserAgent { get; set; }
			/// <summary>Click id</summary>
			[JsonProperty("fbc")]
			public string ClickId { get; set; }
			/// <summary>Browser id</summary>
			[JsonProperty("fbp")]
			public string BrowserId { get; set; }
			/// <summary>Subscription id</summary>
			[JsonProperty("subscription_id")]
			public string SubscriptionId { get; set; }
			/// <summary>Lead id</summary>
			[JsonProperty("lead_id")]
			public string LeadId { get; set; }
			/// <summary>FB login id</summary>
			[JsonProperty("fb_login_id")]
			public string FbLoginId { get; set; }
		}

		/// <summary>
		/// Custom data
		/// </summary>
		[Serializable]
		public class CustomData
		{
			/// <summary>Currency</summary>
			[JsonProperty("currency")]
			public string Currency { get; set; }
			/// <summary>Value</summary>
			[JsonProperty("value")]
			public decimal? Value { get; set; }
			/// <summary>Content name</summary>
			[JsonProperty("content_name")]
			public string ContentName { get; set; }
			/// <summary>Content type</summary>
			[JsonProperty("content_type")]
			public string ContentType { get; set; }
			/// <summary>Content category</summary>
			[JsonProperty("content_category")]
			public string ContentCategory { get; set; }
			/// <summary>Content ids</summary>
			[JsonProperty("content_ids")]
			public string ContentIds { get; set; }
			/// <summary>Contents</summary>
			[JsonProperty("contents")]
			public List<Contents> Contents { get; set; }
			/// <summary>Status</summary>
			[JsonProperty("status")]
			public string Status { get; set; }
			/// <summary>Order id</summary>
			[JsonProperty("order_id")]
			public string OrderId { get; set; }
		}

		/// <summary>
		/// Contents
		/// </summary>
		[Serializable]
		public class Contents
		{
			/// <summary>Product id</summary>
			[JsonProperty("id")]
			public string ProductId { get; set; }
			/// <summary>Quantity</summary>
			[JsonProperty("quantity")]
			public string Quantity { get; set; }
			/// <summary>Item price</summary>
			[JsonProperty("item_price")]
			public decimal ItemPrice { get; set; }
			/// <summary>Delivery category</summary>
			[JsonProperty("delivery_category")]
			public string DeliveryCategory { get; set; }
		}
	}
}
