/*
=========================================================================================================
  Module      : Paygent API 決済情報差分通知電文結果モデルス(PaygentDifferenceNotificationRequestDataset.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.Paygent.DifferenceNotification.Dto
{
	/// <summary>
	/// Paygent difference notification request dataset
	/// </summary>
	public class PaygentDifferenceNotificationRequestDataset
	{
		/// <summary>決済通知ID</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_NOTICE_ID)]
		public string PaymentNoticeId { get; set; }
		/// <summary>変更日時</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_CHANGE_DATE)]
		public string ChangeDate { get; set; }
		/// <summary>決済ID</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_ID)]
		public string PaymentId { get; set; }
		/// <summary>マーチャント取引ID</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_TRADING_ID)]
		public string TradingId { get; set; }
		/// <summary>決済種別CD</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_TYPE)]
		public string PaymentType { get; set; }
		/// <summary>サイトID</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_SITE_ID)]
		public string SiteId { get; set; }
		/// <summary>7 決済ステータス</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_STATUS)]
		public string PaymentStatus { get; set; }
		/// <summary>決済金額</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_AMOUNT)]
		public string PaymentAmount { get; set; }
		/// <summary>Paidy決済ID<summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_PAIDY_PAYMENT_ID)]
		public string PaidyPaymentId { get; set; }
		/// <summary>取引発生日時</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_INIT_DATE)]
		public string PaymentInitDate { get; set; }
		/// <summary>オーソリ日時</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_AUTHORIZED_DATE)]
		public string AuthorizedDate { get; set; }
		/// <summary>キャンセル日時</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_CANCEL_DATE)]
		public string CancelDate { get; set; }
		/// <summary> 支払日時</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_DATE)]
		public string PaymentDate { get; set; }
		/// <summary> イベント</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_EVENT)]
		public string Event { get; set; }
		/// <summary>イベント結果</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_EVENT_RESULT)]
		public string EventResult { get; set; }
		/// <summary>エラーコード</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_EVENT_ERROR_CODE)]
		public string EventErrorCode { get; set; }
		/// <summary>ハッシュ値</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_REQUEST_HC)]
		public string Hc { get; set; }
		/// <summary>処理結果</summary>
		[JsonProperty(PaygentConstants.PAYGENT_API_RESPONSE_RESULT)]
		public string Result { get; set; }
	}
}
