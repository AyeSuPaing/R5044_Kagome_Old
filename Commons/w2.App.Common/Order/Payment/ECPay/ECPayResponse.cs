/*
=========================================================================================================
  Module      : EC Pay Respone(ECPayRespone.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.ECPay
{
	/// <summary>
	/// Ec Pay Payment Respone
	/// </summary>
	public class ECPayResponse
	{
		#region +Method
		/// <summary>
		/// Get Payment Memo
		/// </summary>
		/// <returns>Memo</returns>
		private string GetPaymentMemo()
		{
			var result = string.Empty;
			if (this.IsPaymentTypeATM)
			{
				result = string.Format(
					"銀行番号: {0}, 支払い口座: {1}, 支払い期限: {2}",
					this.BankCode,
					this.VAccount,
					this.ExpireDate);
			}
			else if (this.IsPaymentTypeCVS)
			{
				result = string.Format(
					"支払い番号: {0}, 支払い期限: {1}",
					this.PaymentNo,
					this.ExpireDate);
			}
			else if (this.IsPaymentTypeBarcode)
			{
				result = string.Format(
					"バーコード: {0}",
					ECPayUtility.CreateBarcodeUrl(
						this.Barcode1,
						this.Barcode2,
						this.Barcode3));
			}
			return result;
		}
		#endregion

		#region +Properties
		/// <summary>Merchant Id</summary>
		[JsonProperty(PropertyName = "MerchantID")]
		public string MerchantId { get; set; }
		/// <summary>Merchant Trade No</summary>
		[JsonProperty(PropertyName = "MerchantTradeNo")]
		public string MerchantTradeNo { get; set; }
		/// <summary>Trade No </summary>
		[JsonProperty(PropertyName = "TradeNo")]
		public string TradeNo { get; set; }
		/// <summary>Return Code</summary>
		[JsonProperty(PropertyName = "RtnCode")]
		public string ReturnCode { get; set; }
		/// <summary>Return Message</summary>
		[JsonProperty(PropertyName = "RtnMsg")]
		public string ReturnMessage { get; set; }
		/// <summary>Payment Date</summary>
		[JsonProperty(PropertyName = "PaymentDate")]
		public string PaymentDate { get; set; }
		/// <summary>Payment Type</summary>
		[JsonProperty(PropertyName = "PaymentType")]
		public string PaymentType { get; set; }
		/// <summary>Bar Code 1</summary>
		[JsonProperty(PropertyName = "Barcode1")]
		public string Barcode1 { get; set; }
		/// <summary>Bar Code 2</summary>
		[JsonProperty(PropertyName = "Barcode2")]
		public string Barcode2 { get; set; }
		/// <summary>Bar Code 3</summary>
		[JsonProperty(PropertyName = "Barcode3")]
		public string Barcode3 { get; set; }
		/// <summary>Payment No</summary>
		[JsonProperty(PropertyName = "PaymentNo")]
		public string PaymentNo { get; set; }
		/// <summary>Expire Date</summary>
		[JsonProperty(PropertyName = "ExpireDate")]
		public string ExpireDate { get; set; }
		/// <summary>Bank Code</summary>
		[JsonProperty(PropertyName = "BankCode")]
		public string BankCode { get; set; }
		/// <summary>V Account</summary>
		[JsonProperty(PropertyName = "vAccount")]
		public string VAccount { get; set; }
		/// <summary>Request No</summary>
		[JsonProperty(PropertyName = "no")]
		public string RequestNo { get; set; }
		/// <summary>Is Payment Type Credit Card</summary>
		public bool IsPaymentTypeCreditCard
		{
			get
			{
				var result = ((string.IsNullOrEmpty(this.PaymentType) == false)
					&& this.PaymentType.StartsWith(Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT));
				return result;
			}
		}
		/// <summary>Is Payment Type ATM</summary>
		public bool IsPaymentTypeATM
		{
			get
			{
				var result = ((string.IsNullOrEmpty(this.PaymentType) == false)
					&& this.PaymentType.StartsWith(Constants.FLG_PAYMENT_TYPE_ECPAY_ATM));
				return result;
			}
		}
		/// <summary>Is Payment Type CVS</summary>
		public bool IsPaymentTypeCVS
		{
			get
			{
				var result = ((string.IsNullOrEmpty(this.PaymentType) == false)
					&& this.PaymentType.StartsWith(Constants.FLG_PAYMENT_TYPE_ECPAY_CVS));
				return result;
			}
		}
		/// <summary>Is Payment Type BARCODE</summary>
		public bool IsPaymentTypeBarcode
		{
			get
			{
				var result = ((string.IsNullOrEmpty(this.PaymentType) == false)
					&& this.PaymentType.StartsWith(Constants.FLG_PAYMENT_TYPE_ECPAY_BARCODE));
				return result;
			}
		}
		/// <summary>Is Success</summary>
		public bool IsSuccess
		{
			get
			{
				if (this.RequestNo == ECPayConstants.CONST_RETURN_URL_PARAMETER_NO)
					return (this.ReturnCode == ECPayConstants.CONST_CODE_SUCCESS);

				var result = false;
				if (this.IsPaymentTypeATM)
				{
					result = (this.ReturnCode == ECPayConstants.CONST_CODE_PAYMENT_INFO_URL_ATM_SUCCESS);
				}
				else if (this.IsPaymentTypeCVS || this.IsPaymentTypeBarcode)
				{
					result = (this.ReturnCode == ECPayConstants.CODE_PAYMENT_INFO_URL_BARCODE_OR_CVS_SUCCESS);
				}
				else
				{
					result = (this.ReturnCode == ECPayConstants.CONST_CODE_SUCCESS);
				}
				return result;
			}
		}
		/// <summary>Payment Memo</summary>
		public string PaymentMemo { get { return GetPaymentMemo(); } }
		#endregion
	}
}