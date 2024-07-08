/*
=========================================================================================================
  Module      : 商品ブランドサービス (ProductBrandService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.ProductBrand
{
	/// <summary>
	/// 商品ブランドサービス
	/// </summary>
	public class ProductBrandService : ServiceBase, IProductBrandService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(Hashtable param)
		{
			using (var repository = new ProductBrandRepository())
			{
				var count = repository.GetSearchHitCount(param);
				return count;
			}
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>モデル列</returns>
		public ProductBrandModel[] Search(Hashtable param)
		{
			using (var repository = new ProductBrandRepository())
			{
				var models = repository.Search(param);
				return models;
			}
		}
		#endregion

		#region +GetValidBrandList
		/// <summary>
		/// Get valid brand list
		/// </summary>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Array of product brand model</returns>
		public ProductBrandModel[] GetValidBrandList(SqlAccessor accessor = null)
		{
			using (var repository = new ProductBrandRepository(accessor))
			{
				return repository.GetValidBrandList();
			}
		}
		#endregion

		#region +GetByBrandIds 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="brandIds">ブランドID</param>
		/// <returns>モデル</returns>
		public ProductBrandModel[] GetByBrandIds(string[] brandIds)
		{
			using (var repository = new ProductBrandRepository())
			{
				var models = repository.GetByBrandIds(brandIds);
				return models;
			}
		}
		# endregion

		#region +GetProductBrands
		/// <summary>
		/// Get product brands
		/// </summary>
		/// <returns>Product brands</returns>
		public ProductBrandModel[] GetProductBrands()
		{
			using (var repository = new ProductBrandRepository())
			{
				var models = repository.GetProductBrands();
				return models;
			}
		}
		#endregion
	}
}
