/*
=========================================================================================================
  Module      : Paygent API Paidyオーソリキャンセル電文 結果モデル(PaidyAuthorizationCancellationResponseDataset.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.AuthorizationCancellation.Dto
{
	/// <summary>
	/// Paygent API Paidyオーソリキャンセル電文 結果モデル
	/// </summary>
	[Serializable]
	public class PaidyAuthorizationCancellationResponseDataset : IPaygentResponse
	{
		/// <summary>処理結果</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_RESULT)]
		public string Result { get; set; }
		/// <summary>レスポンスコード</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CODE)]
		public string ResponseCode { get; set; }
		/// <summary>レスポンス詳細</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_DETAIL)]
		public string ResponseDetail { get; set; }
		/// <summary>決済ID</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_ID)]
		public string PaymentId { get; set; }
		/// <summary>マーチャント取引ID</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_TRADING_ID)]
		public string TradingId { get; set; }
	}
}

