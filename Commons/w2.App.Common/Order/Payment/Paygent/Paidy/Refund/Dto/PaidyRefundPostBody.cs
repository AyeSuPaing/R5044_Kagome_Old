/*
=========================================================================================================
  Module      : Paygent API Paidy返金電文 リクエストボディ(PaidyRefundPostBody.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.Order.Payment.Paygent.Foundation.SharedDto;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Refund.Dto
{
	/// <summary>
	/// Paygent API Paidy返金電文 リクエストボディ
	/// </summary>
	[Serializable]
	public class PaidyRefundPostBody
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paymentId">決済ID</param>
		/// <param name="tradingId">マーチャント取引ID</param>
		/// <param name="amount">返金額</param>
		public PaidyRefundPostBody(
			string paymentId,
			decimal amount)
		{
			var sharedParameter = new PaygentSharedApiParameter();
			this.MerchantId = sharedParameter.MerchantId;
			this.ConnectId = sharedParameter.ConnectId;
			this.ConnectPassword = sharedParameter.ConnectPassword;
			this.TelegramKind = PaygentConstants.PAYGENT_TELEGRAM_KIND_PAIDY_REFUND;
			this.TelegramVersion = sharedParameter.TelegramVersion;
			this.PaymentId = paymentId;
			this.Amount = amount.ToString("0");
		}

		/// <summary>
		/// リクエストパラメータの生成
		/// </summary>
		/// <returns>リクエストパラメータ</returns>
		public Dictionary<string, string> GenerateKeyValues()
		{
			var parameters = new Dictionary<string, string>
			{
				{ "Content-Type", "application/x-www-form-urlencoded" },
				{ PaygentConstants.PAYGENT_API_REQUEST_MERCHANT_ID, this.MerchantId },
				{ PaygentConstants.PAYGENT_API_REQUEST_CONNECT_ID, this.ConnectId },
				{ PaygentConstants.PAYGENT_API_REQUEST_CONNECT_PASSWORD, this.ConnectPassword },
				{ PaygentConstants.PAYGENT_API_REQUEST_TELEGRAM_KIND, this.TelegramKind },
				{ PaygentConstants.PAYGENT_API_REQUEST_TELEGRAM_VERSION, this.TelegramVersion },
				{ PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_ID, this.PaymentId },
				{ PaygentConstants.PAYGENT_API_REQUEST_AMOUNT, this.Amount }
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
		/// <summary>返金額</summary>
		private string Amount { get; set; }
	}
}
