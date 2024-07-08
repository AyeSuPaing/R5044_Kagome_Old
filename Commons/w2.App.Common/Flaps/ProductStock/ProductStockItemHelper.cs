/*
=========================================================================================================
  Module      : FLAPS商品在庫情報クラス (ProductStockItemHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Domain.ProductStock;

namespace w2.App.Common.Flaps.ProductStock
{
	/// <summary>
	/// FLAPS商品在庫情報クラス
	/// </summary>
	public partial class ProductStockItem
	{
		/// <summary>
		/// 商品在庫DB更新処理
		/// </summary>
		/// <returns>更新結果</returns>
		public bool Update()
		{
			var productStockService = new ProductStockService();
			var productStock = productStockService.Get(
				Constants.CONST_DEFAULT_SHOP_ID,
				this.GoodsStyleCode,
				this.GoodsCode);

			// 更新
			if (productStock != null)
			{
				var updatemsg = string.Format("{0} {1}", "Update", productStock.VariationId);
				Console.WriteLine(updatemsg);
				productStock.Stock = this.Qty;
				productStock.LastChanged = Constants.FLG_LASTCHANGED_FLAPS;
				var updateResult = productStockService.FlapsUpdate(productStock);
				return updateResult;
			}

			// 挿入
			var insertmsg = string.Format("{0} {1}", "Insert", this.GoodsCode);
			Console.WriteLine(insertmsg);
			var newProductStock = new ProductStockModel
			{
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				ProductId = this.GoodsStyleCode,
				VariationId = this.GoodsCode,
				Stock = this.Qty,
				LastChanged = Constants.FLG_LASTCHANGED_FLAPS,
			};
			var insertResult = productStockService.FlapsInsert(newProductStock);
			return insertResult;
		}
	}
}

