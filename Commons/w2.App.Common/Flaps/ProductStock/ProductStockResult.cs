/*
=========================================================================================================
  Module      : FLAPS在庫結果モデル (ProductStock.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;

namespace w2.App.Common.Flaps.ProductStock
{
	/// <summary>
	/// FLAPS在庫結果モデル
	/// </summary>
	[JsonObject(Constants.FLAPS_API_RESULT_PRODUCT_STOCK)]
	public class ProductStockResult : ResultBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductStockResult()
		{
			this.Message = string.Empty;
			this.GoodsStock = new[] { new ProductStockItem() };
		}

		/// <summary>メッセージ</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_STOCK_MESSAGE)]
		public string Message { get; set; }
		/// <summary>商品在庫情報</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_STOCK_GOODS_STOCK)]
		public ProductStockItem[] GoodsStock { get; set; }
	}
}