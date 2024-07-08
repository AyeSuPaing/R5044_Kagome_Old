/*
=========================================================================================================
  Module      : 商品ブランドサービスのインターフェース (IProductBrandService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.ProductBrand
{
	/// <summary>
	/// 商品ブランドサービスのインターフェース
	/// </summary>
	public interface IProductBrandService : IService
	{
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(Hashtable param);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>モデル列</returns>
		ProductBrandModel[] Search(Hashtable param);

		/// <summary>
		/// Get valid brand list
		/// </summary>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Array of product brand model</returns>
		ProductBrandModel[] GetValidBrandList(SqlAccessor accessor = null);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="brandIds">ブランドID</param>
		/// <returns>モデル</returns>
		ProductBrandModel[] GetByBrandIds(string[] brandIds);

		/// <summary>
		/// Get product brands
		/// </summary>
		/// <returns>Product brands</returns>
		ProductBrandModel[] GetProductBrands();
	}
}
