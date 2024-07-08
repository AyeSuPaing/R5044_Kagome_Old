/*
=========================================================================================================
  Module      : 商品税率カテゴリサービスのインタフェース(IProductTaxCategoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.ProductTaxCategory
{
	/// <summary>
	/// 商品税率カテゴリサービスのインタフェース
	/// </summary>
	public interface IProductTaxCategoryService : IService
	{
		/// <summary>
		/// 全ての商品税率カテゴリを取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// トランザクションを内包するアクセサ
		/// 指定しない場合はメソッド内でトランザクションが完結
		/// <returns>モデル</returns>
		ProductTaxCategoryModel[] GetAllTaxCategory(SqlAccessor accessor = null);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="taxCategoryId">税率カテゴリID</param>
		/// <returns>モデル</returns>
		ProductTaxCategoryModel Get(string taxCategoryId);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(ProductTaxCategoryModel model);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		int Update(ProductTaxCategoryModel model);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="taxCategoryId">税率カテゴリID</param>
		void Delete(string taxCategoryId);

		/// <summary>
		/// マスタ出力用税項目名取得
		/// </summary>
		/// <returns>マスタ出力用税項目名</returns>
		string[] GetMasterExportSettingFieldNames();
	}
}