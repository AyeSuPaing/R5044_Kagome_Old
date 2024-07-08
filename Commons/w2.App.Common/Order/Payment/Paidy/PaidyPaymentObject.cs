/*
=========================================================================================================
  Module      : Paidy Payment Object (PaidyPaymentObject.cs)
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
	/// Paidy payment object
	/// </summary>
	[Serializable]
	public class PaidyPaymentObject
	{
		/// <summary>ID</summary>
		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }
		/// <summary>Token ID</summary>
		[JsonProperty(PropertyName = "token_id")]
		public string TokenId { get; set; }
		/// <summary>Amount</summary>
		[JsonProperty(PropertyName = "amount")]
		public int Amount { get; set; }
		/// <summary>Currency</summary>
		[JsonProperty(PropertyName = "currency")]
		public string Currency { get; set; }
		/// <summary>Description</summary>
		[JsonProperty(PropertyName = "description")]
		public string Description { get; set; }
		/// <summary>Store name</summary>
		[JsonProperty(PropertyName = "store_name")]
		public string StoreName { get; set; }
		/// <summary>Buyer data</summary>
		[JsonProperty(PropertyName = "buyer_data")]
		public PaidyBuyerDataObject BuyerData { get; set; }
		/// <summary>Buyer data</summary>
		[JsonProperty(PropertyName = "order")]
		public PaidyOrderObject Order { get; set; }
		/// <summary>Shipping address</summary>
		[JsonProperty(PropertyName = "shipping_address")]
		public PaidyShippingAddressObject ShippingAddress { get; set; }
		/// <summary>Status</summary>
		[JsonProperty(PropertyName = "status")]
		public string Status { get; set; }
		/// <summary>Capture</summary>
		[JsonProperty(PropertyName = "captures")]
		public List<PaidyCaptureObject> Captures { get; set; }
		/// <summary>Refunds</summary>
		[JsonProperty(PropertyName = "refunds")]
		public List<PaidyRefundObject> Refunds { get; set; }

		/// <summary>
		/// Paidy buyer data object
		/// </summary>
		[Serializable]
		public class PaidyBuyerDataObject
		{
			/// <summary>User ID</summary>
			[JsonProperty(PropertyName = "user_id")]
			public string UserId { get; set; }
			/// <summary>Age</summary>
			[JsonProperty(PropertyName = "age")]
			public int Age { get; set; }
			/// <summary>Order count</summary>
			[JsonProperty(PropertyName = "order_count")]
			public int OrderCount { get; set; }
			/// <summary>Loan to value</summary>
			[JsonProperty(PropertyName = "ltv")]
			public int Ltv { get; set; }
			/// <summary>Last order amount</summary>
			[JsonProperty(PropertyName = "last_order_amount")]
			public int LastOrderAmount { get; set; }
			/// <summary>Last order at</summary>
			[JsonProperty(PropertyName = "last_order_at")]
			public int LastOrderAt { get; set; }
			/// <summary>Subscription Counter</summary>
			[JsonProperty(PropertyName = "subscription_counter")]
			public int SubscriptionCounter { get; set; }
			/// <summary>Gender</summary>
			[JsonProperty(PropertyName = "gender")]
			public string Gender { get; set; }
			/// <summary>Number Of Points</summary>
			[JsonProperty(PropertyName = "number_of_points")]
			public int NumberOfPoints { get; set; }
			/// <summary>Order Amount Last 3 Months</summary>
			[JsonProperty(PropertyName = "order_amount_last3months")]
			public int OrderAmountLast3Months { get; set; }
			/// <summary>Order Count Last 3 Months</summary>
			[JsonProperty(PropertyName = "order_count_last3months")]
			public int OrderCountLast3Months { get; set; }
			/// <summary>Billing address</summary>
			[JsonProperty(PropertyName = "billing_address")]
			public PaidyBillingAddressObject BillingAddress { get; set; }
		}

		/// <summary>
		/// Paidy order item object
		/// </summary>
		[Serializable]
		public class PaidyOrderItemObject
		{
			/// <summary>ID</summary>
			[JsonProperty(PropertyName = "id")]
			public string Id { get; set; }
			/// <summary>Quantity</summary>
			[JsonProperty(PropertyName = "quantity")]
			public int Quantity { get; set; }
			/// <summary>Title</summary>
			[JsonProperty(PropertyName = "title")]
			public string Title { get; set; }
			/// <summary>Unit price</summary>
			[JsonProperty(PropertyName = "unit_price")]
			public int UnitPrice { get; set; }
			/// <summary>Description</summary>
			[JsonProperty(PropertyName = "description")]
			public string Description { get; set; }
		}

		/// <summary>
		/// Paidy order object
		/// </summary>
		[Serializable]
		public class PaidyOrderObject
		{
			/// <summary>List of order item objects</summary>
			[JsonProperty(PropertyName = "items")]
			public List<PaidyOrderItemObject> Items { get; set; }
			/// <summary>Order reference</summary>
			[JsonProperty(PropertyName = "order_ref")]
			public string OrderRef { get; set; }
			/// <summary>Shipping</summary>
			[JsonProperty(PropertyName = "shipping")]
			public int Shipping { get; set; }
			/// <summary>Tax</summary>
			[JsonProperty(PropertyName = "tax")]
			public int Tax { get; set; }
		}

		/// <summary>
		/// Paidy shipping address object
		/// </summary>
		[Serializable]
		public class PaidyShippingAddressObject
		{
			/// <summary>Line 1</summary>
			[JsonProperty(PropertyName = "line1")]
			public string Line1 { get; set; }
			/// <summary>Line 2</summary>
			[JsonProperty(PropertyName = "line2")]
			public string Line2 { get; set; }
			/// <summary>City</summary>
			[JsonProperty(PropertyName = "city")]
			public string City { get; set; }
			/// <summary>State</summary>
			[JsonProperty(PropertyName = "state")]
			public string State { get; set; }
			/// <summary>Zip</summary>
			[JsonProperty(PropertyName = "zip")]
			public string Zip { get; set; }
		}

		/// <summary>
		/// Paidy billing address object
		/// </summary>
		[Serializable]
		public class PaidyBillingAddressObject
		{
			/// <summary>Line 1</summary>
			[JsonProperty(PropertyName = "line1")]
			public string Line1 { get; set; }
			/// <summary>Line 2</summary>
			[JsonProperty(PropertyName = "line2")]
			public string Line2 { get; set; }
			/// <summary>City</summary>
			[JsonProperty(PropertyName = "city")]
			public string City { get; set; }
			/// <summary>State</summary>
			[JsonProperty(PropertyName = "state")]
			public string State { get; set; }
			/// <summary>Zip</summary>
			[JsonProperty(PropertyName = "zip")]
			public string Zip { get; set; }
		}

		/// <summary>
		/// Paidy capture object
		/// </summary>
		[Serializable]
		public class PaidyCaptureObject
		{
			/// <summary>ID</summary>
			[JsonProperty(PropertyName = "id")]
			public string Id { get; set; }
			/// <summary>CreateAt</summary>
			[JsonProperty(PropertyName = "created_at")]
			public string CreateAt { get; set; }
			/// <summary>Amount</summary>
			[JsonProperty(PropertyName = "amount")]
			public string Amount { get; set; }
			/// <summary>Tax</summary>
			[JsonProperty(PropertyName = "tax")]
			public string Tax { get; set; }
			/// <summary>Shipping</summary>
			[JsonProperty(PropertyName = "shipping")]
			public string Shipping { get; set; }
			/// <summary>List of order item objects</summary>
			[JsonProperty(PropertyName = "items")]
			public List<PaidyOrderItemObject> Items { get; set; }
		}

		/// <summary>
		/// Paidy refund object
		/// </summary>
		[Serializable]
		public class PaidyRefundObject
		{
			/// <summary>ID</summary>
			[JsonProperty(PropertyName = "id")]
			public string Id { get; set; }
			/// <summary>CreateAt</summary>
			[JsonProperty(PropertyName = "created_at")]
			public string CreateAt { get; set; }
			/// <summary>CaptureId</summary>
			[JsonProperty(PropertyName = "capture_id")]
			public string CaptureId { get; set; }
			/// <summary>Amount</summary>
			[JsonProperty(PropertyName = "amount")]
			public string Amount { get; set; }
			/// <summary>Reason</summary>
			[JsonProperty(PropertyName = "reason")]
			public string Reason { get; set; }
		}
	}
}
