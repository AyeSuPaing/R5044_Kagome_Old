/*
=========================================================================================================
  Module      : 楽天受注情報取得API連携のskuモデルクラス (SkuModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天受注情報取得API SKUモデル
	/// </summary>
	[JsonObject("SkuModelList")]
	public class SkuModel
	{
		// SKU管理番号 (variantId)
		[JsonProperty("variantId")]
		public string VariantId { get; set; }

		// システム連携用SKU番号 (merchantDefinedSkuId)
		[JsonProperty("merchantDefinedSkuId")]
		public string MerchantDefinedSkuId { get; set; }

		// SKU情報 (skuInfo)
		[JsonProperty("skuInfo")]
		public string SkuInfo { get; set; }
	}
}
