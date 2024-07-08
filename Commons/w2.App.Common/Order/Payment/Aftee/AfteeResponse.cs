/*
=========================================================================================================
  Module      : Aftee Response (AfteeResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Payment.Aftee
{
	/// <summary>
	/// Aftee response object
	/// </summary>
	[Serializable]
	public class AfteeResponse
	{
		#region Property When Success
		/// <summary>Transaction id</summary>
		[JsonProperty(PropertyName = "id")]
		public string TranId { get; set; }
		/// <summary>Sales Settled Datetime</summary>
		[JsonProperty(PropertyName = "sales_settled_datetime")]
		public string SalesSettledDatetime { get; set; }
		#endregion

		#region Property When Error
		/// <summary>Errors</summary>
		[JsonProperty(PropertyName = "errors")]
		public Error[] Errors { get; set; }
		/// <summary>Message</summary>
		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }
		/// <summary>Refunds</summary>
		[JsonProperty(PropertyName = "refunds")]
		public Refund Refunds { get; set; }
		#endregion

		/// <summary>Is Success</summary>
		public bool IsSuccess
		{
			get
			{
				return (this.Errors == null) || (this.Errors.Length == 0);
			}
		}
	}

	/// <summary>
	/// Error
	/// </summary>
	[Serializable]
	public class Error
	{
		/// <summary>Code</summary>
		[JsonProperty(PropertyName = "code")]
		public string Code { get; set; }
		/// <summary>Messages</summary>
		[JsonProperty(PropertyName = "messages")]
		public string[] Messages { get; set; }
		/// <summary>Code</summary>
		[JsonProperty(PropertyName = "params")]
		public string[] Params { get; set; }
	}

	/// <summary>
	/// Refund
	/// </summary>
	[Serializable]
	public class Refund
	{
		/// <summary>Object Type</summary>
		[JsonProperty(PropertyName = "object")]
		public string ObjectType { get; set; }
		/// <summary>Amount refund</summary>
		[JsonProperty(PropertyName = "amount_refund")]
		public int AmountRefund { get; set; }
		/// <summary>Refund Datetime</summary>
		[JsonProperty(PropertyName = "refund_datetime")]
		public string RefundDatetime { get; set; }
		/// <summary>Refund reason</summary>
		[JsonProperty(PropertyName = "refund_reason")]
		public string RefundReason { get; set; }
	}
}
