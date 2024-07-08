/*
=========================================================================================================
  Module      : Neweb Pay Request(NewebPayRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Order.Payment.NewebPay
{
	/// <summary>
	/// NewebPay Request Class
	/// </summary>
	[Serializable]
	public class NewebPayRequest
	{
		/// <summary>
		/// Create Parameters
		/// </summary>
		/// <returns>Parameters</returns>
		public List<KeyValuePair<string, string>> CreateParameters()
		{
			var parameters = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>(NewebPayConstants.PARAM_MERCHANT_ID, this.MerchantId),
				new KeyValuePair<string, string>(NewebPayConstants.PARAM_TRADE_INFO, this.TradeInfo),
				new KeyValuePair<string, string>(NewebPayConstants.PARAM_TRADE_SHA, this.TradeSha),
				new KeyValuePair<string, string>(NewebPayConstants.PARAM_VERSION, this.Version),
				new KeyValuePair<string, string>(NewebPayConstants.PARAM_POST_DATA, this.PostData),
			};

			// Remove null data and sort key
			parameters = parameters
				.Where(item => (item.Value != null))
				.OrderBy(item => item.Key)
				.ToList();
			return parameters;
		}

		/// <summary>Merchant Id</summary>
		[JsonProperty(PropertyName = "MerchantID")]
		public string MerchantId { get; set; }
		/// <summary>Merchant Id Cancel And Sale Refund</summary>
		[JsonProperty(PropertyName = "MerchantID_")]
		public string MerchantIdCancelAndSaleRefund { get; set; }
		/// <summary>Trade Info</summary>
		[JsonProperty(PropertyName = "TradeInfo")]
		public string TradeInfo { get; set; }
		/// <summary>Trade Sha</summary>
		[JsonProperty(PropertyName = "TradeSha")]
		public string TradeSha { get; set; }
		/// <summary>Version</summary>
		[JsonProperty(PropertyName = "Version")]
		public string Version { get; set; }
		/// <summary>Post Data</summary>
		[JsonProperty(PropertyName = "PostData_")]
		public string PostData { get; set; }
	}

	/// <summary>
	/// NewebPay Parameter Request Class
	/// </summary>
	[Serializable]
	public class NewebPayParameterRequest
	{
		/// <summary>Merchant Id</summary>
		[JsonProperty(PropertyName = "MerchantID")]
		public string MerchantId { get; set; }
		/// <summary>Resond Type</summary>
		[JsonProperty(PropertyName = "RespondType")]
		public string RespondType { get; set; }
		/// <summary>Time Stamp</summary>
		[JsonProperty(PropertyName = "TimeStamp")]
		public string TimeStamp { get; set; }
		/// <summary>Version</summary>
		[JsonProperty(PropertyName = "Version")]
		public string Version { get; set; }
		/// <summary>Merchant Order No</summary>
		[JsonProperty(PropertyName = "MerchantOrderNo")]
		public string MerchantOrderNo { get; set; }
		/// <summary>Amount</summary>
		[JsonProperty(PropertyName = "Amt")]
		public int Amt { get; set; }
		/// <summary>Item Description</summary>
		[JsonProperty(PropertyName = "ItemDesc")]
		public string ItemDesc { get; set; }
		/// <summary>Return Url</summary>
		[JsonProperty(PropertyName = "ReturnURL")]
		public string ReturnURL { get; set; }
		/// <summary>Notify Url</summary>
		[JsonProperty(PropertyName = "NotifyURL")]
		public string NotifyURL { get; set; }
		/// <summary>Customer Url</summary>
		[JsonProperty(PropertyName = "CustomerURL")]
		public string CustomerURL { get; set; }
		/// <summary>Client Back Url</summary>
		[JsonProperty(PropertyName = "ClientBackURL")]
		public string ClientBackURL { get; set; }
		/// <summary>Email</summary>
		[JsonProperty(PropertyName = "Email")]
		public string Email { get; set; }
		/// <summary>Login Type</summary>
		[JsonProperty(PropertyName = "LoginType")]
		public int LoginType { get; set; }
		/// <summary>Credit Card One Time Flg</summary>
		[JsonProperty(PropertyName = "CREDIT")]
		public int Credit { get; set; }
		/// <summary>Line Pay Flg</summary>
		[JsonProperty(PropertyName = "LINEPAY")]
		public int LinePay { get; set; }
		/// <summary>Credit Card Installment Flg</summary>
		[JsonProperty(PropertyName = "InstFlag")]
		public string InstFlag { get; set; }
		/// <summary>Union Pay Flg</summary>
		[JsonProperty(PropertyName = "UNIONPAY")]
		public int UnionPay { get; set; }
		/// <summary>Web ATM Flg</summary>
		[JsonProperty(PropertyName = "WEBATM")]
		public int WebATM { get; set; }
		/// <summary>Atm Flg</summary>
		[JsonProperty(PropertyName = "VACC")]
		public int Vacc { get; set; }
		/// <summary>Convenience Store Code Flg</summary>
		[JsonProperty(PropertyName = "CVS")]
		public int CVS { get; set; }
		/// <summary>Barcode Flg</summary>
		[JsonProperty(PropertyName = "BARCODE")]
		public int Barcode { get; set; }
		/// <summary>Convenience Store Receive Flg</summary>
		[JsonProperty(PropertyName = "CVSCOM")]
		public int CVSCOM { get; set; }
		/// <summary>Token Term</summary>
		[JsonProperty(PropertyName = "TokenTerm")]
		public string TokenTerm { get; set; }
		/// <summary>Trade No</summary>
		[JsonProperty(PropertyName = "TradeNo")]
		public string TradeNo { get; set; }
		/// <summary>Index Type</summary>
		[JsonProperty(PropertyName = "IndexType")]
		public string IndexType { get; set; }
		/// <summary>Close Type</summary>
		[JsonProperty(PropertyName = "CloseType")]
		public int CloseType { get; set; }
		/// <summary>Is Ali Pay</summary>
		public bool IsCredit
		{
			get
			{
				return (this.Credit == NewebPayConstants.FLG_NEWEBPAY_PAYMENT_TYPE_ON);
			}
		}
		/// <summary>Is Atm</summary>
		public bool IsAtm
		{
			get
			{
				return (this.Vacc == NewebPayConstants.FLG_NEWEBPAY_PAYMENT_TYPE_ON);
			}
		}
		/// <summary>Is Cvs</summary>
		public bool IsCvs
		{
			get
			{
				return (this.CVS == NewebPayConstants.FLG_NEWEBPAY_PAYMENT_TYPE_ON);
			}
		}
		/// <summary>Is Barcode</summary>
		public bool IsBarcode
		{
			get
			{
				return (this.Barcode == NewebPayConstants.FLG_NEWEBPAY_PAYMENT_TYPE_ON);
			}
		}
	}
}