/*
=========================================================================================================
  Module      : Paygent CVS Order Register Response Dataset(OrderRegisterResponseDataset.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.Cvs.Register.Dto
{
	/// <summary>
	/// Paygent CVS order register response dataset
	/// </summary>
	public class OrderRegisterResponseDataset : IPaygentResponse
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
		/// <summary>決済ベンダ受付番号</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_RECEIPT_NUMBER)]
		public string ReceiptNumber { get; set; }
		/// <summary>結果URL情報</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_RECEIPT_PRINT_URL)]
		public string ReceiptPrintUrl { get; set; }
		/// <summary>利用可能コンビニ企業CD</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CVS_COMPANY_ID)]
		public string CsvCompanyId { get; set; }
		/// <summary>決済ステータス</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS)]
		public string PaymentStatus { get; set; }
		/// <summary>支払期限日</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_LIMIT_DATE)]
		public string PaymentLimitDate { get; set; }
	}
}
