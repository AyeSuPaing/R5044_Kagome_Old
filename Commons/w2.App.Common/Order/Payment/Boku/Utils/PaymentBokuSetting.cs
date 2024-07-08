/*
=========================================================================================================
  Module      : Payment Boku Setting(PaymentBokuSetting.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.Boku.Utils
{
	/// <summary>
	/// Payment Boku Setting
	/// </summary>
	public class PaymentBokuSetting
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="merchantId">Merchant id</param>
		/// <param name="merchantRequestId">Merchant request id</param>
		/// <param name="timeout">Time out</param>
		/// <param name="skipRetryFlg">Skip retry flag</param>
		/// <param name="sendNotificationFlg">Send notification flag</param>
		/// <param name="keyId">Key id</param>
		/// <param name="apiKey">API key</param>
		/// <param name="merchantConsumerId">Merchant consumer id</param>
		public PaymentBokuSetting(
			string merchantId,
			string merchantRequestId,
			int timeout,
			bool skipRetryFlg,
			bool sendNotificationFlg,
			string keyId,
			string apiKey,
			string merchantConsumerId)
		{
			this.MerchantId = merchantId;
			this.MerchantRequestId = string.Format(
				merchantRequestId,
				DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(0, 100).ToString());
			this.Timeout = StringUtility.ToEmpty(timeout);
			this.SkipRetryFlg = skipRetryFlg;
			this.SendNotificationFlg = sendNotificationFlg;
			this.KeyId = keyId;
			this.APIKey = apiKey;
			this.MerchantConsumerId = merchantConsumerId;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Get default setting from config
		/// </summary>
		/// <returns>Payment boku setting</returns>
		internal static PaymentBokuSetting GetDefaultSetting()
		{
			return new PaymentBokuSetting(
				Constants.PAYMENT_BOKU_MERCHANT_ID,
				Constants.PAYMENT_BOKU_MERCHANT_REQUEST_ID,
				Constants.PAYMENT_BOKU_TIMEOUT_REQUEST,
				Constants.PAYMENT_BOKU_SKIP_RETRY_FLG,
				Constants.PAYMENT_BOKU_SEND_NOTIFICATION_FLG,
				Constants.PAYMENT_BOKU_KEY_ID,
				Constants.PAYMENT_BOKU_API_KEY,
				Constants.PAYMENT_BOKU_MERCHANT_CONSUMER_ID);
		}
		#endregion

		#region Properties
		/// <summary>Merchant id</summary>
		public string MerchantId { get; private set; }
		/// <summary>Merchant request id</summary>
		public string MerchantRequestId { get; private set; }
		/// <summary>Merchant consumer id</summary>
		public string MerchantConsumerId { get; private set; }
		/// <summary>Time out</summary>
		public string Timeout { get; private set; }
		/// <summary>Skip retry flag</summary>
		public bool SkipRetryFlg { get; private set; }
		/// <summary>Send notification flag</summary>
		public bool SendNotificationFlg { get; private set; }
		/// <summary>Key id</summary>
		public string KeyId { get; private set; }
		/// <summary>API key</summary>
		public string APIKey { get; private set; }
		#endregion
	}
}
