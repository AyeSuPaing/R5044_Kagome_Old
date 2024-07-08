/*
=========================================================================================================
  Module      : 商品在庫増減値クラス(ProductStockAdjustCheck.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.Domain.ProductStock.Helper
{
	/// <summary>
	/// 商品在庫増減値クラス
	/// </summary>
	public class ProductStockAdjustCheck
	{
		/// <summary>店舗ID</summary>
		public string ShopId { get; set; }
		/// <summary>商品ID</summary>
		public string ProductId { get; set; }
		/// <summary>バリエーションID</summary>
		public string VariationId { get; set; }
		/// <summary>商品名</summary>
		public string ProductName { get; set; }
		/// <summary>在庫増減数</summary>
		public int AdjustmentQuantity { get; set; }
		/// <summary>在庫管理方法</summary>
		public string StockManagementKbn { get; set; }
	}
}
