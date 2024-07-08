/*
=========================================================================================================
  Module      : Atone Response (AtoneResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Linq;

namespace w2.App.Common.Order.Payment.Atone
{
	/// <summary>
	/// Atone response object
	/// </summary>
	[Serializable]
	public class AtoneResponse
	{
		#region Property When Success
		/// <summary>Transaction id</summary>
		[JsonProperty(PropertyName = "id")]
		public string TranId { get; set; }
		/// <summary>Sales Settled Datetime</summary>
		[JsonProperty(PropertyName = "sales_settled_datetime")]
		public string SalesSettledDatetime { get; set; }
		/// <summary>Refunds</summary>
		[JsonProperty(PropertyName = "refunds")]
		public Refund[] Refunds { get; set; }
		/// <summary>オーソリ成功か</summary>
		public bool IsAuthorizationSuccess
		{
			get { return (this.AuthorizationResult == AuthorizationResult.Ok); }
		}
		/// <summary>オーソリ結果</summary>
		[JsonProperty(PropertyName = "authorization_result")]
		public AuthorizationResult AuthorizationResult { get; set; }
		/// <summary>オーソリ結果NG理由文言</summary>
		public string AuthorizationResultNgReasonMessage
		{
			get
			{
				switch (this.AuthorizationResultNgReason)
				{
					case Atone.AuthorizationResultNgReason.Exceed:
						return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ATONE_AUTHORIZATION_RESULT_NG_EXCEED);

					case Atone.AuthorizationResultNgReason.Other:
						return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ATONE_AUTHORIZATION_RESULT_NG_OTHER);

					default:
						return "";
				}
			}
		}
		/// <summary>オーソリ結果NG理由</summary>
		[JsonProperty(PropertyName = "authorization_result_ng_reason")]
		public AuthorizationResultNgReason? AuthorizationResultNgReason { get; set; }
		#endregion

		#region Property When Error
		/// <summary>Errors</summary>
		[JsonProperty(PropertyName = "errors")]
		public Error[] Errors { get; set; }
		/// <summary>Message</summary>
		[JsonProperty(PropertyName = "message")]
		public string Message { get; set; }
		#endregion

		/// <summary>Is Success</summary>
		public bool IsSuccess
		{
			get
			{
				return (this.Errors == null) || (this.Errors.Length == 0);
			}
		}
		/// <summary>Error Message When Call Api</summary>
		public string ErrorMessageWhenCallApi
		{
			get
			{
				var result = this.IsSuccess
					? string.Empty
					: string.Join(",", this.Errors.SelectMany(error => error.Messages));
				return result;
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

	/// <summary>オーソリ結果</summary>
	public enum AuthorizationResult
	{
		/// <summary>OK</summary>
		Ok = 1,
		/// <summary>NG</summary>
		Ng = 2,
	}

	/// <summary>オーソリ結果NG理由</summary>
	public enum AuthorizationResultNgReason
	{
		/// <summary>金額超過</summary>
		Exceed = 1,
		/// <summary>その他</summary>
		Other = 9,
	}
}
