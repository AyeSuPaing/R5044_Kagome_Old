/*
=========================================================================================================
  Module      : FLAPS商品結果モデル (ProductResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;

namespace w2.App.Common.Flaps.Product
{
	/// <summary>
	/// FLAPS商品結果
	/// </summary>
	[JsonObject(Constants.FLAPS_API_RESULT_PRODUCT)]
	public class ProductResult : ResultBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductResult()
		{
			this.FinishFetch = 0;
			this.Goods = new[] { new ProductItem() };
		}

		/// <summary>フェッチ状態</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_FINISH_FETCH)]
		public int FinishFetch { get; set; }
		/// <summary>商品情報</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_GOODS)]
		public ProductItem[] Goods { get; set; }
	}
}
