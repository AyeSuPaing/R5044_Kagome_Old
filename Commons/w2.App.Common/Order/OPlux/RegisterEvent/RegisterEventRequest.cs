/*
=========================================================================================================
  Module      : Register Event Request(RegisterEventRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.OPlux.Requests;
using w2.App.Common.Order.OPlux.Requests.Helper;

namespace w2.App.Common.Order.OPlux.RegisterEvent
{
	/// <summary>
	/// Register event request
	/// </summary>
	[RequestKey("request")]
	public class RegisterEventRequest : BaseOPluxRequest
	{
		/// <summary>バージョン</summary>
		[RequestKey("version")]
		public string Version { get; set; }
		/// <summary>加盟店ID</summary>
		[RequestKey("shop_id")]
		public string ShopId { get; set; }
		/// <summary>認証用シグネチャ</summary>
		[RequestKey("signiture")]
		public string Signiture { get; set; }
		/// <summary>認証用ハッシュ関数</summary>
		[RequestKey("hash_method")]
		public string HashMethod { get; set; }
		/// <summary>審査申請日時</summary>
		[RequestKey("request_datetime")]
		public DateTime RequestDatetime { get; set; }
		/// <summary>Event</summary>
		[RequestKey("event")]
		public EventRequest Event { get; set; }
	}
}
