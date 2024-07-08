/*
=========================================================================================================
  Module      : Payment Information Inquiry Response Dataset(PaymentInformationInquiryResponseDataset.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.PaymentInformationInquiry.Dto
{
	/// <summary>
	/// Payment Information Inquiry Response Dataset
	/// </summary>
	public class PaymentInformationInquiryResponseDataset : IPaygentResponse
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
		/// <summary>申込コンビニ企業CD</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CVS_COMPANY_ID)]
		public string CvsCompanyId { get; set; }
		/// <summary>決済金額</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_AMOUNT)]
		public string PaymentAmount { get; set; }
		/// <summary>利用者電話番号</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CUSTOMER_TEL)]
		public string CustomerTel { get; set; }
		/// <summary>サービス区分</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_SERVICE_TYPE)]
		public string ServiceType { get; set; }
		/// <summary>キャンセル受信日時</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CANCEL_DATE)]
		public string CancelDate { get; set; }
		/// <summary>支払日時</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_DATE)]
		public string PaymentDate { get; set; }
		/// <summary>確定受信日時</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CONFIRM_NOTICE_DATE)]
		public string ConfirmNoticeDate { get; set; }
		/// <summary>支払期限日</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_LIMIT_DATE)]
		public string PaymentLimitDate { get; set; }
		/// <summary>主券枚数</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_MAIN_TICKET_NUM)]
		public string MainTicketNum { get; set; }
		/// <summary>取引発生日時</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_INIT_DATE)]
		public string PaymentInitDate { get; set; }
		/// <summary>現地支払日時</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_USER_PAYMENT_DATE)]
		public string UserPaymentDate { get; set; }
		/// <summary>速報受信日時</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_EARLY_NOTICE_DATE)]
		public string EarlyNoticeDate { get; set; }
		/// <summary>利用者姓ｶﾅ</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CUSTOMER_FAMILY_NAME_KANA)]
		public string CustomerFamilyNameKana { get; set; }
		/// <summary>利用者名ｶﾅ</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CUSTOMER_NAME_KANA)]
		public string CustomerNameKana { get; set; }
		/// <summary>利用者姓</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CUSTOMER_FAMILY_NAME)]
		public string CustomerFamilyName { get; set; }
		/// <summary>利用者名</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_CUSTOMER_NAME)]
		public string CustomerName { get; set; }
		/// <summary>決済ステータス</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS)]
		public string PaymentStatus { get; set; }
		/// <summary>決済種別CD</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_TYPE)]
		public string PaymentType { get; set; }
		/// <summary>発券開始日</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_TICKET_START_DATE)]
		public string TicketStartDate { get; set; }
		/// <summary>発券終了日</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_TICKET_END_DATE)]
		public string TicketEndDate { get; set; }
		/// <summary>決済ベンダ受付番号</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_RECEIPT_NUMBER)]
		public string ReceiptNumber { get; set; }
		/// <summary>チケット枚数</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_TICKET_NUM)]
		public string TicketNum { get; set; }
		/// <summary>サイトID</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_SITE_ID)]
		public string SiteId { get; set; }
		/// <summary>副券枚数</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_SUB_TICKET_NUM)]
		public string SubTicketNum { get; set; }
	}
}
