/*
=========================================================================================================
  Module      : 商品ランキング設定マスタモデル (ProductRankingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;

namespace w2.Domain.ProductRanking
{
	/// <summary>
	/// 商品ランキング設定マスタモデル
	/// </summary>
	public partial class ProductRankingModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>カテゴリIDリスト</summary>
		public string[] CategoryIdList
		{
			get { return this.ExcludeCategoryIds.Replace("\r\n", "\n").Split('\n').Where(s => string.IsNullOrEmpty(s) == false).ToArray(); }
		}
		/// <summary>商品ランキングアイテム</summary>
		public ProductRankingItemModel[] Items
		{
			get { return (ProductRankingItemModel[])this.DataSource["EX_Items"]; }
			set { this.DataSource["EX_Items"] = value; }
		}
		#endregion
	}
}
