/*
=========================================================================================================
  Module      : 商品ランキングアイテム設定マスタモデル (ProductRankingItemModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.ProductRanking
{
	/// <summary>
	/// 商品ランキングアイテム設定マスタモデル
	/// </summary>
	public partial class ProductRankingItemModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>商品表示価格</summary>
		public decimal DisplayPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE] = value; }
		}
		/// <summary>商品表示特別価格</summary>
		public decimal? DisplaySpecialPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] = value; }
		}
		/// <summary>商品名</summary>
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_NAME] = value; }
		}
		#endregion
	}
}
