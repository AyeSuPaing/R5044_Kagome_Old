/*
=========================================================================================================
  Module      : FLAPS商品マテリアルモデル (ProductMaterial.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;

namespace w2.App.Common.Flaps.Product
{
	/// <summary>
	/// FLAPS商品マテリアル
	/// </summary>
	[JsonObject(Constants.FLAPS_API_RESULT_PRODUCT)]
	public class ProductMaterial
	{
		/// <summary>
		/// コンストラクタ
		/// /// </summary>
		public ProductMaterial()
		{
			this.MaterialName = string.Empty;
			this.MaterialCtrName = string.Empty;
			this.MaterialPercent = string.Empty;
		}

		/// <summary>マテリア名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_MATERIAL_NAME)]
		public string MaterialName { get; set; }
		/// <summary>マテリアctr名</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_MATERIAL_CTR_NAME)]
		public string MaterialCtrName { get; set; }
		/// <summary>マテリアパーセント</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_MATERIAL_PERCENT)]
		public string MaterialPercent { get; set; }
	}
}
