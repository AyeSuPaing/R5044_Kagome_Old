/*
=========================================================================================================
  Module      : 商品税率カテゴリリポジトリ (ProductTaxCategoryRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ProductTaxCategory
{
	/// <summary>
	/// 商品税率カテゴリリポジトリ
	/// </summary>
	internal class ProductTaxCategoryRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ProductTaxCategory";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ProductTaxCategoryRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ProductTaxCategoryRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetAllTaxCategory 全ての商品税率カテゴリを取得
		/// <summary>
		/// 全ての商品税率カテゴリを取得
		/// </summary>
		/// <returns>モデルの配列</returns>
		internal ProductTaxCategoryModel[] GetAllTaxCategory()
		{
			var dv = Get(XML_KEY_NAME, "GetAllTaxCategory");
			var models = dv.Cast<DataRowView>().Select(drv => new ProductTaxCategoryModel(drv)).ToArray();
			return models;
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="taxCategoryId">税率カテゴリID</param>
		/// <returns>モデル</returns>
		internal ProductTaxCategoryModel Get(string taxCategoryId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_ID, taxCategoryId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new ProductTaxCategoryModel(dv[0]);
		}
		#endregion
		
		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(ProductTaxCategoryModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(ProductTaxCategoryModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="taxCategoryId">税率カテゴリID</param>
		internal int Delete(string taxCategoryId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_ID, taxCategoryId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion
	}
}
