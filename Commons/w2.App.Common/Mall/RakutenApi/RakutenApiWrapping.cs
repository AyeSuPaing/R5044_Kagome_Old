/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API ラッピングモデル (RakutenApiSettlement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API ラッピングモデル
	/// </summary>
	public class RakutenApiWrapping
	{
		/// <summary>ラッピングタイトル</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_WRAPPING_MODEL_TITLE)]
		public int? Title { get; set; }
		/// <summary>ラッピング名</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_WRAPPING_MODEL_NAME)]
		public string Name { get; set; }
		/// <summary>料金</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_WRAPPING_MODEL_PRICE)]
		public decimal Price { get; set; }
		/// <summary>税込別</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_WRAPPING_MODEL_INCLUDE_TAX_FLAG)]
		public int? IncludeTaxFlag { get; set; }
		/// <summary>ラッピング削除フラグ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_WRAPPING_MODEL_DELETE_WRAPPING_FLAG)]
		public int? DeleteWrappingFlag { get; set; }
		/// <summary>ラッピング税率</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_WRAPPING_MODEL_TAX_RATE)]
		public decimal TaxRate { get; set; }
		/// <summary>ラッピング税額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_WRAPPING_MODEL_TAX_PRICE)]
		public decimal TaxPrice { get; set; }
	}
}
