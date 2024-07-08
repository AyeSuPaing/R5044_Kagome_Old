/*
=========================================================================================================
  Module      : EC Pay Convenience Store Request(ECPayConvenienceStoreRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Order.Payment.ECPay
{
	/// <summary>
	/// EC Pay Convenience Store Request Class
	/// </summary>
	public class ECPayConvenienceStoreRequest
	{
		/// <summary>
		/// Create Parameters
		/// </summary>
		/// <returns>Parameters</returns>
		public List<KeyValuePair<string, string>> CreateParameters()
		{
			var parameters = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>(ECPayConstants.PARAM_MERCHANT_ID, this.MerchantId),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_ALL_PAY_LOGISTICS_ID, this.AllPayLogisticsId),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_MERCHANT_TRADE_NO, this.MerchantTradeNo),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_MERCHANT_TRADE_DATE, this.MerchantTradeDate),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_LOGISTICS_TYPE, this.LogisticsType),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_LOGISTICS_SUB_TYPE, this.LogisticsSubType),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_GOODS_AMOUNT, this.GoodsAmount),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_COLLECTION_AMOUNT, this.CollectionAmount),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_IS_COLLECTION, this.IsCollection),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_GOODS_NAME, this.GoodsName),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_SENDER_NAME, this.SenderName),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_SENDER_PHONE, this.SenderPhone),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_SENDER_CELL_PHONE, this.SenderCellPhone),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_RECEIVER_NAME, this.ReceiverName),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_RECEIVER_PHONE, this.ReceiverPhone),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_RECEIVER_CELL_PHONE, this.ReceiverCellPhone),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_RECEIVER_EMAIL, this.ReceiverEmail),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_SERVER_REPLY_URL, this.ServerReplyUrl),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_PLATFORM_ID, this.PlatformId),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_SENDER_ZIP_CODE, this.SenderZipCode),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_SENDER_ADDRESS, this.SenderAddress),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_RECEIVER_ZIP_CODE, this.ReceiverZipCode),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_RECEIVER_ADDRESS, this.ReceiverAddress),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_TEMPERATURE, this.Temperature),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_DISTANCE, this.Distance),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_SPECIFICATION, this.Specification),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_RECEIVER_STORE_ID, this.ReceiverStoreId),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_SERVICE_TYPE, this.ServiceType),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_CVS_LOGISTICS_C2C_REPLY_URL, this.LogisticsC2CReplyUrl),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_CVS_STORE_TYPE, this.StoreType),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_CVS_STATUS, this.Status),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_CVS_STORE_ID, this.StoreId),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_CVS_PAYMENT_NO, this.CvsPaymentNo),
				new KeyValuePair<string, string>(ECPayConstants.PARAM_CVS_VALIDATION_NO, this.CvsValidationNo)
			};

			// Remove null data and sort key
			parameters = parameters
				.Where(item => (item.Value != null))
				.OrderBy(item => item.Key)
				.ToList();
			return parameters;
		}

		/// <summary>
		/// Get Parameters
		/// </summary>
		/// <returns>Url Parameters</returns>
		public List<KeyValuePair<string, string>> GetParameters()
		{
			var parameters = CreateParameters();
			parameters.Add(new KeyValuePair<string, string>(ECPayConstants.PARAM_CHECK_MAC_VALUE, this.CheckMacValue));
			return parameters;
		}

		/// <summary>Merchant Id</summary>
		public string MerchantId { get; set; }
		/// <summary>All Pay Logistics Id</summary>
		public string AllPayLogisticsId { get; set; }
		/// <summary>Merchant Trade No</summary>
		public string MerchantTradeNo { get; set; }
		/// <summary>Merchant Trade Date</summary>
		public string MerchantTradeDate { get; set; }
		/// <summary>Logistics Type</summary>
		public string LogisticsType { get; set; }
		/// <summary>Logistics Sub-Type</summary>
		public string LogisticsSubType { get; set; }
		/// <summary>Goods Amount</summary>
		public string GoodsAmount { get; set; }
		/// <summary>Collection Amount</summary>
		public string CollectionAmount { get; set; }
		/// <summary>Is Collection</summary>
		public string IsCollection { get; set; }
		/// <summary>Goods Name</summary>
		public string GoodsName { get; set; }
		/// <summary>Sender Name</summary>
		public string SenderName { get; set; }
		/// <summary>Sender Phone</summary>
		public string SenderPhone { get; set; }
		/// <summary>Sender Cell Phone</summary>
		public string SenderCellPhone { get; set; }
		/// <summary>Receiver Name</summary>
		public string ReceiverName { get; set; }
		/// <summary>Receiver Phone</summary>
		public string ReceiverPhone { get; set; }
		/// <summary>Receiver Cell Phone</summary>
		public string ReceiverCellPhone { get; set; }
		/// <summary>Receiver Email</summary>
		public string ReceiverEmail { get; set; }
		/// <summary>Server Reply Url</summary>
		public string ServerReplyUrl { get; set; }
		/// <summary>Platform Id</summary>
		public string PlatformId { get; set; }
		/// <summary>Check Mac Value</summary>
		public string CheckMacValue { get; set; }
		/// <summary>Sender Zip Code</summary>
		public string SenderZipCode { get; set; }
		/// <summary>Sender Address</summary>
		public string SenderAddress { get; set; }
		/// <summary>Receiver Zip Code</summary>
		public string ReceiverZipCode { get; set; }
		/// <summary>Receiver Address</summary>
		public string ReceiverAddress { get; set; }
		/// <summary>Temperature</summary>
		public string Temperature { get; set; }
		/// <summary>Distance</summary>
		public string Distance { get; set; }
		/// <summary>Specification</summary>
		public string Specification { get; set; }
		/// <summary>Receiver Store Id</summary>
		public string ReceiverStoreId { get; set; }
		/// <summary>Service Type</summary>
		public string ServiceType { get; set; }
		/// <summary>Logistics C2C Reply Url</summary>
		public string LogisticsC2CReplyUrl { get; set; }
		/// <summary>Store Type</summary>
		public string StoreType { get; set; }
		/// <summary>Status</summary>
		public string Status { get; set; }
		/// <summary>Store Id</summary>
		public string StoreId { get; set; }
		/// <summary>Cvs Payment No</summary>
		public string CvsPaymentNo { get; set; }
		/// <summary>Cvs Validation No</summary>
		public string CvsValidationNo { get; set; }
	}
}
