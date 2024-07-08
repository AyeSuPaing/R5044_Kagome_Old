/*
=========================================================================================================
  Module      : Atm Order Register Response(AtmOrderRegisterResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.ATM.Register.Dto
{
	/// <summary>
	/// Atm order register response
	/// </summary>
	[Serializable]
	public class AtmOrderRegisterResponse : IPaygentResponse
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
		/// <summary>収納機関番号</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_PAY_CENTER_NUMBER)]
		public string PayCenterNumber { get; set; }
		/// <summary>お客様番号</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CUSTOMER_NUMBER)]
		public string CustomerNumber { get; set; }
		/// <summary>確認番号</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CONF_NUMBER)]
		public string ConfNumber { get; set; }
		/// <summary>支払期限日</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_LIMIT_DATE)]
		public string PaymentLimitDate { get; set; }
	}
}
