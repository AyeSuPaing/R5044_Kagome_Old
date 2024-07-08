/*
=========================================================================================================
  Module      : ネクストエンジン受注一括登録パターン情報取得API レスポンス (NextEngineGetUploadPatternApiResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.NextEngine.Response
{
	/// <summary>
	/// ネクストエンジン連携API 受注一括登録パターン情報取得APIレスポンスデータ
	/// </summary>
	public class NextEngineGetUploadPatternApiResponse : NextEngineApiResponseBase
	{
		/// <summary>検索結果の連想配列(JSONのオブジェクト型)</summary>
		[JsonProperty(NextEngineConstants.RESPONSE_PARAM_DATA)]
		public NEUploadPattern[] Data { get; set; }
	}

	/// <summary>
	/// ネクストエンジン連携API ネクストエンジンアップロードパターンモデルクラス
	/// </summary>
	public class NEUploadPattern
	{
		/// <summary>受注一括登録パターンID</summary>
		[JsonProperty("receive_order_upload_pattern_id")]
		public string PatternId { get; set; }
		/// <summary>受注一括登録フォーマットパターンID</summary>
		[JsonProperty("receive_order_upload_pattern_format_id")]
		public string FormatId { get; set; }
		/// <summary>店舗ID</summary>
		[JsonProperty("receive_order_upload_pattern_shop_id")]
		public string ShopId { get; set; }
		/// <summary>削除フラグ(1:有効 1以外:無効)</summary>
		[JsonProperty("receive_order_upload_pattern_deleted_flag")]
		public string DeleteFlg { get; set; }
	}
}
