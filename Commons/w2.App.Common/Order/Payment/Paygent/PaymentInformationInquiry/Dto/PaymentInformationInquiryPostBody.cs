﻿/*
=========================================================================================================
  Module      : Payment Information Inquiry Post Body(PaymentInformationInquiryPostBody.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.Order.Payment.Paygent.Foundation;
using w2.App.Common.Order.Payment.Paygent.Foundation.SharedDto;

namespace w2.App.Common.Order.Payment.Paygent.PaymentInformationInquiry.Dto
{
	/// <summary>
	/// Payment Information Inquiry Post Body
	/// </summary>
	[Serializable]
	public class PaymentInformationInquiryPostBody : IPaygentPostBody
	{
		/// <summary>電文種別ID：決済情報照会電文</summary>
		private const string TELEGRAM_KIND = "094";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paymentId">決済ID</param>
		/// <param name="trandingId">マーチャント取引ID</param>
		public PaymentInformationInquiryPostBody(string paymentId, string trandingId)
		{
			var sharedParameter = new PaygentSharedApiParameter();
			this.MerchantId = sharedParameter.MerchantId;
			this.ConnectId = sharedParameter.ConnectId;
			this.ConnectPassword = sharedParameter.ConnectPassword;
			this.TelegramKind = TELEGRAM_KIND;
			this.TelegramVersion = sharedParameter.TelegramVersion;
			this.PaymentId = paymentId;
			this.TrandingId = trandingId;
		}

		/// <summary>
		/// リクエストパラメータ生成
		/// </summary>
		/// <returns>リクエストパラメータ</returns>
		public Dictionary<string, string> GenerateKeyValues()
		{
			var parameters = new Dictionary<string, string>
			{
				{
					PaygentConstants.PAYGENT_API_REQUEST_CONTENT_TYPE,
					PaygentConstants.PAYGENT_API_REQUEST_CONTENT_TYPE_FORM_URLENCODED
				},
				{ PaygentConstants.PAYGENT_API_REQUEST_MERCHANT_ID, this.MerchantId },
				{ PaygentConstants.PAYGENT_API_REQUEST_CONNECT_ID, this.ConnectId },
				{ PaygentConstants.PAYGENT_API_REQUEST_CONNECT_PASSWORD, this.ConnectPassword },
				{ PaygentConstants.PAYGENT_API_REQUEST_TELEGRAM_KIND, this.TelegramKind },
				{ PaygentConstants.PAYGENT_API_REQUEST_TELEGRAM_VERSION, this.TelegramVersion },
				{ PaygentConstants.PAYGENT_API_REQUEST_TRADING_ID, this.TrandingId },
				{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_ID, this.PaymentId }
			};
			return parameters;
		}

		/// <summary>マーチャントID</summary>
		private string MerchantId { get; set; }
		/// <summary>接続ID</summary>
		private string ConnectId { get; set; }
		/// <summary>接続パスワード</summary>
		private string ConnectPassword { get; set; }
		/// <summary>電文種別ID</summary>
		private string TelegramKind { get; set; }
		/// <summary>電文バージョン番号</summary>
		private string TelegramVersion { get; set; }
		/// <summary>決済ID</summary>
		private string PaymentId { get; set; }
		/// <summary>マーチャント取引ID</summary>
		private string TrandingId { get; set; }
	}
}
