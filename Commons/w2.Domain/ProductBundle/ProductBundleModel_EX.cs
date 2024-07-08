/*
=========================================================================================================
  Module      : 商品同梱テーブルモデル (ProductBundleModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;

namespace w2.Domain.ProductBundle
{
	/// <summary>
	/// 商品同梱テーブルモデル
	/// </summary>
	public partial class ProductBundleModel
	{
		#region メソッド
		/// <summary>
		/// 対象商品 商品ID
		/// </summary>
		/// <returns></returns>
		public string[] GetTargetProductIds()
		{
			var targetIds = this.TargetProductIdsList
				.Where(id => ((string.IsNullOrEmpty(id) == false) && (id.IndexOf(",") < 0)))
				.ToArray();
			return targetIds;
		}

		/// <summary>
		/// 対象商品 バリエーションID
		/// </summary>
		/// <returns></returns>
		public string[] GetTargetProductVariationIds()
		{
			var targetIds = this.TargetProductIdsList
				.Where(id => ((string.IsNullOrEmpty(id) == false) && (id.IndexOf(",") >= 0)))
				.ToArray();
			return targetIds;
		}

		/// <summary>
		/// 対象商品 カテゴリID
		/// </summary>
		/// <returns></returns>
		public string[] GetTargetProductCategoryIds()
		{
			var targetCategoryIds = this.TargetProductCategoryList
				.Where(id => (string.IsNullOrEmpty(id) == false))
				.ToArray();
			return targetCategoryIds;
		}

		/// <summary>
		/// 対象外商品 商品ID
		/// </summary>
		/// <returns>商品ID</returns>
		public string[] GetExceptProductIds()
		{
			var exceptIds = this.ExceptProductIdsList
				.Where(id => ((string.IsNullOrEmpty(id) == false) && (id.IndexOf(",") < 0)))
				.ToArray();
			return exceptIds;
		}

		/// <summary>
		/// 対象外商品 バリエーションID
		/// </summary>
		/// <returns></returns>
		public string[] GetExceptProductVariationIds()
		{
			var exceptIds = this.ExceptProductIdsList
				.Where(id => ((string.IsNullOrEmpty(id) == false) && (id.IndexOf(",") >= 0)))
				.ToArray();
			return exceptIds;
		}

		/// <summary>
		/// 対象外商品 カテゴリID
		/// </summary>
		/// <returns></returns>
		public string[] GetExceptProductCategoryIds()
		{
			var exceptIds = this.ExceptProductCategoryList
				.Where(id => (string.IsNullOrEmpty(id) == false))
				.ToArray();
			return exceptIds;
		}
		#endregion

		#region プロパティ
		/// <summary>対象商品</summary>
		public string[] TargetProductIdsList
		{
			get { return this.TargetProductIds.Trim().Replace("\r\n", "\n").Split('\n'); }
		}
		/// <summary>アイテムリスト</summary>
		public ProductBundleItemModel[] Items
		{
			get { return (ProductBundleItemModel[])this.DataSource["EX_Items"]; }
			set { this.DataSource["EX_Items"] = value; }
		}
		/// <summary>対象外商品リスト</summary>
		public string[] ExceptProductIdsList
		{
			get { return this.ExceptProductIds.Trim().Replace("\r\n", "\n").Split('\n'); }
		}
		/// <summary>対象商品カテゴリリスト</summary>
		public string[] TargetProductCategoryList
		{
			get { return this.TargetProductCategoryIds.Trim().Replace("\r\n", "\n").Split('\n'); }
		}
		/// <summary>対象外商品カテゴリリスト</summary>
		public string[] ExceptProductCategoryList
		{
			get { return this.ExceptProductCategoryIds.Trim().Replace("\r\n", "\n").Split('\n'); }
		}
		/// <summary>過去注文した回数</summary>
		public int OrderedCount { get; set; }
		#endregion
	}
}
