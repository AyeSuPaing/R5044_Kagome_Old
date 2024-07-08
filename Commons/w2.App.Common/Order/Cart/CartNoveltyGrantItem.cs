/*
=========================================================================================================
  Module      : カートノベルティ付与アイテムクラス(CartNoveltyGrantItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.App.Common.Option;
using w2.Domain.Novelty;

namespace w2.App.Common.Order
{
	/// <summary>
	/// カートノベルティ付与アイテムクラス
	/// </summary>
	[Serializable]
	public class CartNoveltyGrantItem : NoveltyGrantItemModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">付与アイテムモデル</param>
		/// <param name="product">商品情報</param>
		public CartNoveltyGrantItem(NoveltyGrantItemModel model, DataRowView product)
		{
			// 各プロパティにセット
			this.DataSource = model.DataSource;
			this.VariationId = product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID].ToString();
			this.Name = product[Constants.FIELD_PRODUCT_NAME].ToString();
			this.JointName = ProductCommon.CreateProductJointName(product);
			this.Price = ProductCommon.GetProductPriceNumeric(product, true);

			// その他商品データを格納
			this.ProductInfo = new Hashtable();
			foreach (DataColumn column in product.DataView.Table.Columns)
			{
				this.ProductInfo[column.ColumnName] = product[column.ColumnName];
			}
		}
		#endregion

		#region プロパティ
		/// <summary>バリエーションID</summary>
		public string VariationId { get; private set; }
		/// <summary>商品名</summary>
		public string Name { get; private set; }
		/// <summary>商品 + バリエーション名</summary>
		public string JointName { get; private set; }
		/// <summary>価格</summary>
		public string Price { get; set; }
		/// <summary>商品情報</summary>
		public Hashtable ProductInfo { get; private set; }
		#endregion
	}
}