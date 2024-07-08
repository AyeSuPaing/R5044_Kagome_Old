/*
=========================================================================================================
  Module      : EC Pay Request(ECPayRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.Order.Payment.ECPay
{
	/// <summary>
	/// EC Pay Request
	/// </summary>
	[Serializable]
	public class ECPayRequest
	{
		/// <summary>Merchant Id</summary>
		[JsonProperty(PropertyName = "MerchantID")]
		public string MerchantId { get; set; }
		/// <summary>Merchant Trade No</summary>
		[JsonProperty(PropertyName = "MerchantTradeNo")]
		public string MerchantTradeNo { get; set; }
		/// <summary>Merchant Trade Date</summary>
		[JsonProperty(PropertyName = "MerchantTradeDate")]
		public string MerchantTradeDate { get; set; }
		/// <summary>Payment Type</summary>
		[JsonProperty(PropertyName = "PaymentType")]
		public string PaymentType { get; set; }
		/// <summary>Total Amount</summary>
		[JsonProperty(PropertyName = "TotalAmount")]
		public string TotalAmount { get; set; }
		/// <summary>Traded Desc</summary>
		[JsonProperty(PropertyName = "TradeDesc")]
		public string TradeDesc { get; set; }
		/// <summary>Item Name</summary>
		[JsonProperty(PropertyName = "ItemName")]
		public string ItemName { get; set; }
		/// <summary>Return Url</summary>
		[JsonProperty(PropertyName = "ReturnURL")]
		public string ReturnUrl { get; set; }
		/// <summary>Choose Payment</summary>
		[JsonProperty(PropertyName = "ChoosePayment")]
		public string ChoosePayment { get; set; }
		/// <summary>Check Mac Value</summary>
		[JsonProperty(PropertyName = "CheckMacValue")]
		public string CheckMacValue { get; set; }
		/// <summary>Client Back Url</summary>
		[JsonProperty(PropertyName = "ClientBackURL")]
		public string ClientBackUrl { get; set; }
		/// <summary>Encrypt Type</summary>
		[JsonProperty(PropertyName = "EncryptType")]
		public string EncryptType { get; set; }
		/// <summary>Payment Info Url</summary>
		[JsonProperty(PropertyName = "PaymentInfoURL")]
		public string PaymentInfoUrl { get; set; }
		/// <summary>Credit Installment</summary>
		[JsonProperty(PropertyName = "CreditInstallment")]
		public string CreditInstallment { get; set; }
		/// <summary>Trade No</summary>
		[JsonProperty(PropertyName = "TradeNo")]
		public string TradeNo { get; set; }
		/// <summary>Action</summary>
		[JsonProperty(PropertyName = "Action")]
		public string Action { get; set; }
		/// <summary>Platform Id</summary>
		[JsonProperty(PropertyName = "PlatformID")]
		public string PlatformId { get; set; }
		/// <summary>Binding Card</summary>
		[JsonProperty(PropertyName = "BindingCard")]
		public string BindingCard { get; set; }
		/// <summary>Merchant Member Id</summary>
		[JsonProperty(PropertyName = "MerchantMemberID")]
		public string MerchantMemberId { get; set; }
	}
}
