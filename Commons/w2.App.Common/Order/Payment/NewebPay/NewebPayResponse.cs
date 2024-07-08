/*
=========================================================================================================
  Module      : Neweb Pay Response(NewebPayResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.NewebPay
{
	/// <summary>
	/// NewebPay Payment Response
	/// </summary>
	public class NewebPayResponse
	{
		/// <summary>
		/// Get Payment Memo
		/// </summary>
		/// <returns>Memo</returns>
		private string GetPaymentMemo()
		{
			var result = string.Empty;
			if (this.IsAtm)
			{
				result = string.Format(
					"銀行番号: {0}, 支払い口座: {1}, 支払い期限: {2}",
					this.Result.PayBankCode,
					this.Result.CodeNo,
					this.Result.ExpireDate);
			}
			else if (this.IsCvs)
			{
				result = string.Format(
					"支払い番号: {0}, 支払い期限: {1}",
					this.Result.CodeNo,
					this.Result.ExpireDate);
			}
			else if (this.IsBarcode)
			{
				result = string.Format(
					"バーコード: {0}",
					NewebPayUtility.CreateBarcodeUrl(
						this.Result.Barcode1,
						this.Result.Barcode2,
						this.Result.Barcode3));
			}
			return result;
		}

		/// <summary>Status</summary>
		[JsonProperty(PropertyName = "Status")]
		public string Status { get; set; }
		/// <summary>Message</summary>
		[JsonProperty(PropertyName = "Message")]
		public string Message { get; set; }
		/// <summary>Merchant ID</summary>
		[JsonProperty(PropertyName = "MerchantID")]
		public string MerchantID { get; set; }
		/// <summary>Trade Info</summary>
		[JsonProperty(PropertyName = "TradeInfo")]
		public string TradeInfo { get; set; }
		/// <summary>Trade Sha</summary>
		[JsonProperty(PropertyName = "TradeSha")]
		public string TradeSha { get; set; }
		/// <summary>Version</summary>
		[JsonProperty(PropertyName = "Version")]
		public string Version { get; set; }
		/// <summary>Result</summary>
		[JsonProperty(PropertyName = "Result")]
		public NewebPayResponseResultDetail Result { get; set; }
		/// <summary>Result Refund</summary>
		[JsonProperty(PropertyName = "ResultRefund")]
		public NewebPayResponseResultDetail[] ResultRefund { get; set; }
		/// <summary>Amount</summary>
		[JsonProperty(PropertyName = "Amt")]
		public int Amount { get; set; }
		/// <summary>Trade No</summary>
		[JsonProperty(PropertyName = "TradeNo")]
		public string TradeNo { get; set; }
		/// <summary>Merchant Orde rNo</summary>
		[JsonProperty(PropertyName = "MerchantOrderNo")]
		public string MerchantOrderNo { get; set; }
		/// <summary>Check Code</summary>
		[JsonProperty(PropertyName = "CheckCode")]
		public string CheckCode { get; set; }
		/// <summary>Is Credit</summary>
		[JsonIgnore]
		public bool IsCredit
		{
			get
			{
				var result = ((string.IsNullOrEmpty(this.Result.PaymentType) == false)
					&& this.Result.PaymentType.StartsWith(Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT));
				return result;
			}
		}
		/// <summary>Is Atm</summary>
		[JsonIgnore]
		public bool IsAtm
		{
			get
			{
				var result = ((string.IsNullOrEmpty(this.Result.PaymentType) == false)
					&& this.Result.PaymentType.StartsWith(Constants.FLG_PAYMENT_TYPE_NEWEBPAY_ATM));
				return result;
			}
		}
		/// <summary>Is Cvs</summary>
		[JsonIgnore]
		public bool IsCvs
		{
			get
			{
				var result = ((string.IsNullOrEmpty(this.Result.PaymentType) == false)
					&& (this.Result.PaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CVS));
				return result;
			}
		}
		/// <summary>Is Barcode</summary>
		[JsonIgnore]
		public bool IsBarcode
		{
			get
			{
				var result = ((string.IsNullOrEmpty(this.Result.PaymentType) == false)
					&& this.Result.PaymentType.StartsWith(Constants.FLG_PAYMENT_TYPE_NEWEBPAY_BARCODE));
				return result;
			}
		}
		/// <summary>Is CvsCom</summary>
		[JsonIgnore]
		public bool IsCvsCom
		{
			get
			{
				var result = ((string.IsNullOrEmpty(this.Result.PaymentType) == false)
					&& (this.Result.PaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CVSCOM));
				return result;
			}
		}
		/// <summary>Is Success</summary>
		[JsonIgnore]
		public bool IsSuccess
		{
			get
			{
				return (this.Status == NewebPayConstants.CONST_STATUS_SUCCESS);
			}
		}
		/// <summary>Payment Memo</summary>
		[JsonIgnore]
		public string PaymentMemo { get { return GetPaymentMemo(); } }
		/// <summary>Cart Id</summary>
		[JsonIgnore]
		public string CartId { get; set; }
	}

	/// <summary>
	/// NewebPay Payment Response Result
	/// </summary>
	public class NewebPayResponseResultDetail
	{
		/// <summary>Merchant ID</summary>
		[JsonProperty(PropertyName = "MerchantID")]
		public string MerchantID { get; set; }
		/// <summary>Amount</summary>
		[JsonProperty(PropertyName = "Amt")]
		public int Amount { get; set; }
		/// <summary>Trade No</summary>
		[JsonProperty(PropertyName = "TradeNo")]
		public string TradeNo { get; set; }
		/// <summary>Merchant Orde rNo</summary>
		[JsonProperty(PropertyName = "MerchantOrderNo")]
		public string MerchantOrderNo { get; set; }
		/// <summary>Payment Type</summary>
		[JsonProperty(PropertyName = "PaymentType")]
		public string PaymentType { get; set; }
		/// <summary>Resond Type</summary>
		[JsonProperty(PropertyName = "RespondType")]
		public string RespondType { get; set; }
		/// <summary>Code No</summary>
		[JsonProperty(PropertyName = "CodeNo")]
		public string CodeNo { get; set; }
		/// <summary>Payment Time</summary>
		[JsonProperty(PropertyName = "PayTime")]
		public string PayTime { get; set; }
		/// <summary>Store Code</summary>
		[JsonProperty(PropertyName = "StoreCode")]
		public string StoreCode { get; set; }
		/// <summary>Store Name</summary>
		[JsonProperty(PropertyName = "StoreName")]
		public string StoreName { get; set; }
		/// <summary>Store Addr</summary>
		[JsonProperty(PropertyName = "StoreAddr")]
		public string StoreAddr { get; set; }
		/// <summary>Store Type</summary>
		[JsonProperty(PropertyName = "StoreType")]
		public string StoreType { get; set; }
		/// <summary>Barcode 1</summary>
		[JsonProperty(PropertyName = "Barcode_1")]
		public string Barcode1 { get; set; }
		/// <summary>Barcode 2</summary>
		[JsonProperty(PropertyName = "Barcode_2")]
		public string Barcode2 { get; set; }
		/// <summary>Barcode 3</summary>
		[JsonProperty(PropertyName = "Barcode_3")]
		public string Barcode3 { get; set; }
		/// <summary>Expire Date</summary>
		[JsonProperty(PropertyName = "ExpireDate")]
		public string ExpireDate { get; set; }
		/// <summary>Pay Bank Code</summary>
		[JsonProperty(PropertyName = "BankCode")]
		public string PayBankCode { get; set; }
		/// <summary>Card 6 No</summary>
		[JsonProperty(PropertyName = "Card6No")]
		public string Card6No { get; set; }
		/// <summary>Card 4 No</summary>
		[JsonProperty(PropertyName = "Card4No")]
		public string Card4No { get; set; }
		/// <summary>Return Code</summary>
		[JsonProperty(PropertyName = "ReturnCode")]
		public string ReturnCode { get; set; }
		/// <summary>Return Message</summary>
		[JsonProperty(PropertyName = "ReturnMessage")]
		public string ReturnMessage { get; set; }
		/// <summary>Payment Url</summary>
		[JsonProperty(PropertyName = "PaymentUrl")]
		public string PaymentUrl { get; set; }
		/// <summary>Token Use Status</summary>
		[JsonProperty(PropertyName = "TokenUseStatus")]
		public string TokenUseStatus { get; set; }
		/// <summary>Payer Account 5 Code</summary>
		[JsonProperty(PropertyName = "PayerAccount5Code")]
		public string PayerAccount5Code { get; set; }
	}
}
