/*
=========================================================================================================
  Module      : 商品在庫モデル (ProductStock.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.Product
{
	/// <summary>
	/// 商品在庫モデル
	/// </summary>
	public class ProductStock
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="variationId"></param>
		/// <param name="stockQuantity"></param>
		public ProductStock(string variationId, string stockQuantity)
		{
			this.VariationId = variationId;
			this.StockQuantity = stockQuantity;
		}

		/// <summary>バリエーションID</summary>
		public string VariationId { get; private set; }
		/// <summary>在庫数量</summary>
		public string StockQuantity { get; private set; }
	}
}
