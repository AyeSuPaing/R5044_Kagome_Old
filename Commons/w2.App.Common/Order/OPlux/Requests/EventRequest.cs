/*
=========================================================================================================
  Module      : Event Request(EventRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.OPlux.Requests.Helper;

namespace w2.App.Common.Order.OPlux.Requests
{
	/// <summary>
	/// Event request
	/// </summary>
	public class EventRequest
	{
		/// <summary>審査モデル</summary>
		[RequestKey("model_id")]
		public string ModelId { get; set; }
		/// <summary>加盟店管理ID</summary>
		[RequestKey("event_id_for_shop")]
		public string EventIdForShop { get; set; }
		/// <summary>イベント種別</summary>
		[RequestKey("event_type")]
		public string EventType { get; set; }
		/// <summary>IPアドレス</summary>
		[RequestKey("ip_address_only")]
		public string IpAddressOnly { get; set; }
		/// <summary>デバイス情報</summary>
		[RequestKey("device_info")]
		public string DeviceInfo { get; set; }
		/// <summary>Cookie only</summary>
		[RequestKey("cookie_only")]
		public string CookieOnly { get; set; }
		/// <summary>EC</summary>
		[RequestKey("ec")]
		public ECRequest EC { get; set; }

		/// <summary>
		/// EC request
		/// </summary>
		public class ECRequest
		{
			/// <summary>Buy datetime</summary>
			[RequestKey("buy_datetime")]
			public DateTime BuyDatetime { get; set; }
			/// <summary>Settle</summary>
			[RequestKey("settle")]
			public SettleRequest Settle { get; set; }
			/// <summary>Customers</summary>
			[RequestKey("customers")]
			public CustomersRequest Customers { get; set; }
			/// <summary>Tenant</summary>
			[RequestKey("tenant")]
			public TenantRequest Tenant { get; set; }
		}

		/// <summary>
		/// Settle request
		/// </summary>
		public class SettleRequest
		{
			/// <summary>利用上限金額</summary>
			[RequestKey("limit_price")]
			public decimal LimitPrice { get; set; }
			/// <summary>決済ステータス</summary>
			[RequestKey("status")]
			public string Status { get; set; }
			/// <summary>決済日時</summary>
			[RequestKey("datetime")]
			public DateTime Date { get; set; }
			/// <summary>決済金額</summary>
			[RequestKey("amount")]
			public decimal Amount { get; set; }
			/// <summary>貨幣コード</summary>
			[RequestKey("currency")]
			public string Currency { get; set; }
			/// <summary>決済方法</summary>
			[RequestKey("method")]
			public string Method { get; set; }
			/// <summary>Credit card</summary>
			[RequestKey("credit_card")]
			public CreditCardRequest CreditCard { get; set; }
			/// <summary>Pay first</summary>
			[RequestKey("pay_first")]
			public PayFirstRequest PayFirst { get; set; }
		}

		/// <summary>
		/// Credit card request
		/// </summary>
		public class CreditCardRequest
		{
			/// <summary>カードBINコード</summary>
			[RequestKey("bincode")]
			public string Bincode { get; set; }
		}

		/// <summary>
		/// Pay first request
		/// </summary>
		public class PayFirstRequest
		{
			/// <summary>初回支払期限日</summary>
			[RequestKey("pay_deadline_date", FormatValue = "yyyy/MM/dd")]
			public DateTime? PayDeadlineDate { get; set; }
		}

		/// <summary>
		/// Tenant request
		/// </summary>
		public class TenantRequest
		{
			/// <summary>店子名</summary>
			[RequestKey("tenant_name")]
			public string TenantName { get; set; }
		}
	}
}
