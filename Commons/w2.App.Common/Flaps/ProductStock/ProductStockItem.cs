/*
=========================================================================================================
  Module      : FLAPS商品在庫モデル (ProductStockItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;

namespace w2.App.Common.Flaps.ProductStock
{
	/// <summary>
	/// FLAPS商品在庫モデル
	/// </summary>
	[JsonObject(Constants.FLAPS_API_RESULT_PRODUCT_STOCK)]
	public partial class ProductStockItem
	{
		/// <summary>
		/// コンストラクタ
		/// /// </summary>
		public ProductStockItem()
		{
			this.SalePointCode = string.Empty;
			this.GoodsSerNo = string.Empty;
			this.GoodsCode = string.Empty;
			this.Qty = 0;
			this.GoodsStyleCode = string.Empty;
			this.ColorCode = string.Empty;
		}

		/// <summary>ショップカウンターコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_STOCK_SALE_POINT_CODE)]
		public string SalePointCode { get; set; }
		/// <summary>商品唯一番号</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_STOCK_GOODS_SER_NO)]
		public string GoodsSerNo { get; set; }
		/// <summary>商品コード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_STOCK_GOODS_CODE)]
		public string GoodsCode { get; set; }
		/// <summary>在庫数</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_STOCK_QTY)]
		public int Qty { get; set; }
		/// <summary>商品スタイルコード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_STOCK_GOODS_STYLE_CODE)]
		public string GoodsStyleCode { get; set; }
		/// <summary>色コード</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_PRODUCT_STOCK_COLOR_CODE)]
		public string ColorCode { get; set; }
	}
}