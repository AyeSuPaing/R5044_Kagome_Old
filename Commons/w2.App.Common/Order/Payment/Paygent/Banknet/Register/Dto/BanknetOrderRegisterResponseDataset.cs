/*
=========================================================================================================
  Module      : Paygent Banknet Order Register Response Dataset (BanknetOrderRegisterResponseDataset.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.Banknet.Register.Dto
{
	/// <summary>
	/// Paygent Banknet order register response dataset
	/// </summary>
	public class BanknetOrderRegisterResponseDataset : IPaygentResponse
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
		/// <summary>マーチャント取引ID</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_TRADING_ID)]
		public string TradingId { get; set; }
		/// <summary>ASP画面有効期限</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_ASP_LIMIT_DATE)]
		public string AspLimitDate {  get; set; }
		/// <summary>支払発生期限</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_ASP_PAYMENT_LIMIT_DATE)]
		public string AspPaymentLimitDate {  get; set; }
		/// <summary>銀行ネット決済ASP画面URL</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_ASP_URL)]
		public string AspUrl {  get; set; }
		/// <summary>請求金額</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_AMOUNT)]
		public string Amount {  get; set; }
		/// <summary>請求内容カナ</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CLAIM_KANA)]
		public string ClaimKana {  get; set; }
		/// <summary>請求内容漢字</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CLAIM_KANJI)]
		public string ClaimKanji {  get; set; }
		/// <summary>取引発生日時</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_TRADE_GENERATION_DATE)]
		public string TradeGenerationDate {  get; set; }
	}
}
