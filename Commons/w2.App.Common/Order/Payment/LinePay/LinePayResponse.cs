/*
=========================================================================================================
  Module      : Line Pay Response (LinePayResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Payment.LinePay
{
	/// <summary>
	/// LINE Pay response object
	/// </summary>
	[Serializable]
	public class LinePayResponse
	{
		/// <summary>
		/// ログ向けメッセージ作成
		/// </summary>
		/// <returns>ログ向けメッセージ</returns>
		public string CreateMessageForLog()
		{
			if (this.IsSuccess) return "";
			return string.Format("CODE[{0}] {1}", this.ReturnCode, this.ReturnMessage);
		}

		/// <summary>Return Code</summary>
		[JsonProperty(PropertyName = "returnCode")]
		public string ReturnCode { get; set; }
		/// <summary>Return Message</summary>
		[JsonProperty(PropertyName = "returnMessage")]
		public string ReturnMessage { get; set; }
		/// <summary>Info</summary>
		[JsonProperty(PropertyName = "info")]
		public Info Info { get; set; }
		/// <summary>Is Success</summary>
		public bool IsSuccess
		{
			get { return (this.ReturnCode == Constants.FLG_PAYMENT_LINEPAY_CODE_DEFAULT); }
		}
	}

	/// <summary>
	/// Info
	/// </summary>
	[Serializable]
	[JsonObject("info")]
	public class Info
	{
		/// <summary>Payment Url</summary>
		[JsonProperty(PropertyName = "paymentUrl")]
		public PaymentUrl PaymentUrl { get; set; }
		/// <summary>Transaction Id</summary>
		[JsonProperty(PropertyName = "transactionId")]
		public string TransactionId { get; set; }
		/// <summary>Payment AccessToken</summary>
		[JsonProperty(PropertyName = "paymentAccessToken")]
		public string PaymentAccessToken { get; set; }
		/// <summary>Transaction Date</summary>
		[JsonProperty(PropertyName = "transactionDate")]
		public string TransactionDate { get; set; }
		/// <summary>Authorization Expire Date</summary>
		[JsonProperty(PropertyName = "authorizationExpireDate")]
		public string AuthorizationExpireDate { get; set; }
		/// <summary>Reg Key</summary>
		[JsonProperty(PropertyName = "regKey")]
		public string RegKey { get; set; }
	}

	/// <summary>
	/// Payment Url
	/// </summary>
	[Serializable]
	[JsonObject("paymentUrl")]
	public class PaymentUrl
	{
		/// <summary>Web Url</summary>
		[JsonProperty(PropertyName = "web")]
		public string WebUrl { get; set; }
		/// <summary>App Url</summary>
		[JsonProperty(PropertyName = "app")]
		public string AppUrl { get; set; }
	}
}
