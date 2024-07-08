/*
=========================================================================================================
  Module      : 商品ブランドリポジトリ (ProductBrandRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ProductBrand
{
	/// <summary>
	/// 商品ブランドリポジトリ
	/// </summary>
	public class ProductBrandRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ProductBrand";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductBrandRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductBrandRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(Hashtable param)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", param);
			return (int)dv[0][0];
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
			var dv = Get(XML_KEY_NAME, "Search", param);
			return dv.Cast<DataRowView>().Select(drv => new ProductBrandModel(drv)).ToArray();
		}
		#endregion

		#region ~GetValidBrandList
		/// <summary>
		/// Get valid brand list
		/// </summary>
		/// <returns>Array of product brand model</returns>
		internal ProductBrandModel[] GetValidBrandList()
		{
			var data = Get(XML_KEY_NAME, "GetValidBrandList");

			var result = data.Cast<DataRowView>().Select(row => new ProductBrandModel(row)).ToArray();
			return result;
		}
		#endregion

		#region ~GetByBrandIds ブランドを取得
		/// <summary>
		/// ブランドを取得
		/// </summary>
		/// <param name="brandIds">ブランドID</param>
		/// <returns>モデル</returns>
		internal ProductBrandModel[] GetByBrandIds(params string[] brandIds)
		{
			if (brandIds.Length == 0) return new ProductBrandModel[0];

			var replace = new KeyValuePair<string, string>(
				"@@ ids @@",
				string.Join(
					",",
					brandIds.Select(id => string.Format("'{0}'", id.Replace("'", "''")))));

			var dv = Get(XML_KEY_NAME, "GetByBrandIds", replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new ProductBrandModel(drv)).ToArray();
		}
		#endregion

		#region +GetProductBrands
		/// <summary>
		/// Get product brands
		/// </summary>
		/// <returns>Product brands</returns>
		public ProductBrandModel[] GetProductBrands()
		{
			var result = Get(XML_KEY_NAME, "GetProductBrands");
			return result.Cast<DataRowView>()
				.Select(row => new ProductBrandModel(row))
				.ToArray();
		}
		#endregion
	}
}
