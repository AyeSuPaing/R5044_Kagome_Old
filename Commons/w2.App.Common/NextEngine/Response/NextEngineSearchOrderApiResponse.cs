/*
=========================================================================================================
  Module      : ネクストエンジン受注伝票検索API レスポンス (NextEngineSearchOrderApiResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.NextEngine.Response
{
	/// <summary>
	/// ネクストエンジン連携API 受注伝票検索APIレスポンスデータ
	/// </summary>
	public class NextEngineSearchOrderApiResponse : NextEngineApiResponseBase
	{
		/// <summary>検索結果件数</summary>
		[JsonProperty(NextEngineConstants.RESPONSE_PARAM_COUNT)]
		public string Count { get; set; }
		/// <summary>検索結果連想配列</summary>
		[JsonProperty(NextEngineConstants.RESPONSE_PARAM_DATA)]
		public NEOrder[] Data { get; set; }
	}

	/// <summary>
	/// ネクストエンジン連携API ネクストエンジン受注伝票モデルクラス
	/// </summary>
	public class NEOrder
	{
		/// <summary>伝票番号</summary>
		[JsonProperty("receive_order_id")]
		public string NEOrderId { get; set; }
		/// <summary>受注番号</summary>
		[JsonProperty("receive_order_shop_cut_form_id")]
		public string OrderId { get; set; }
		/// <summary>受注キャンセル区分</summary>
		[JsonProperty("receive_order_cancel_type_id")]
		public string CancelTypeId { get; set; }
		/// <summary>受注キャンセル日</summary>
		[JsonProperty("receive_order_cancel_date")]
		public string CancelDate { get; set; }
		/// <summary>受注状態区分</summary>
		[JsonProperty("receive_order_order_status_id")]
		public string OrderStatusId { get; set; }
		/// <summary>受注状態名</summary>
		[JsonProperty("receive_order_order_status_name")]
		public string OrderStatusName { get; set; }
		/// <summary>発送方法区分</summary>
		[JsonProperty("receive_order_delivery_id")]
		public string DeliveryId { get; set; }
		/// <summary>発送方法名</summary>
		[JsonProperty("receive_order_delivery_name")]
		public string DeliveryName { get; set; }
		/// <summary>発送伝票番号</summary>
		[JsonProperty("receive_order_delivery_cut_form_id")]
		public string DeliveryCutFormId { get; set; }
		/// <summary>出荷確定日</summary>
		[JsonProperty("receive_order_send_date")]
		public string OrderSendDate { get; set; }
		/// <summary>最終更新日</summary>
		[JsonProperty("receive_order_last_modified_date")]
		public string LastModifiedDate { get; set; }
	}
}
