/*
=========================================================================================================
  Module      : 商品税率カテゴリサービス (ProductTaxCategoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using w2.Common.Sql;

namespace w2.Domain.ProductTaxCategory
{
	/// <summary>
	/// 商品税率カテゴリサービス
	/// </summary>
	public class ProductTaxCategoryService : ServiceBase, IProductTaxCategoryService
	{
		#region +GetAllTaxCategory 全ての商品税率カテゴリを取得
		/// <summary>
		/// 全ての商品税率カテゴリを取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// トランザクションを内包するアクセサ
		/// 指定しない場合はメソッド内でトランザクションが完結
		/// <returns>モデル</returns>
		public ProductTaxCategoryModel[] GetAllTaxCategory(SqlAccessor accessor = null)
		{
			using (var repository = new ProductTaxCategoryRepository(accessor))
			{
				return repository.GetAllTaxCategory();
			}
		}
		#endregion
		
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="taxCategoryId">税率カテゴリID</param>
		/// <returns>モデル</returns>
		public ProductTaxCategoryModel Get(string taxCategoryId)
		{
			using (var repository = new ProductTaxCategoryRepository())
			{
				var model = repository.Get(taxCategoryId);
				return model;
			}
		}
		#endregion
		
		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(ProductTaxCategoryModel model)
		{
			using (var repository = new ProductTaxCategoryRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion
		
		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int Update(ProductTaxCategoryModel model)
		{
			using (var repository = new ProductTaxCategoryRepository())
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion
		
		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="taxCategoryId">税率カテゴリID</param>
		public void Delete(string taxCategoryId)
		{
			using (var repository = new ProductTaxCategoryRepository())
			{
				repository.Delete(taxCategoryId);
			}
		}
		#endregion

		#region +GetMasterExportSettingFieldNames マスタ出力用税項目名取得
		/// <summary>
		/// マスタ出力用税項目名取得
		/// </summary>
		/// <returns>マスタ出力用税項目名</returns>
		public string[] GetMasterExportSettingFieldNames()
		{
			var models = GetAllTaxCategory().GroupBy(x => x.TaxRate);
			var fields = models.SelectMany(
				model =>
				{
					var fieldNames = new List<string>();
					fieldNames.Add(Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE + "_" + model.Key.ToString().Replace(".", ""));
					fieldNames.Add(Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE + "_" + model.Key.ToString().Replace(".", ""));
					fieldNames.Add(Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE + "_" + model.Key.ToString().Replace(".", ""));
					fieldNames.Add(Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE + "_" + model.Key.ToString().Replace(".", ""));
					fieldNames.Add(Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_TOTAL_BY_RATE + "_" + model.Key.ToString().Replace(".", ""));
					fieldNames.Add(Constants.FIELD_ORDERPRICEBYTAXRATE_TAX_PRICE_BY_RATE + "_" + model.Key.ToString().Replace(".", ""));
					return fieldNames;
				}).ToArray();
			return fields;
		}
		#endregion
	}
}
