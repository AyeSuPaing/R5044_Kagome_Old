/*
=========================================================================================================
  Module      : 商品タグリポジトリ (ProductTagRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ProductTag
{
	/// <summary>
	/// 商品タグリポジトリ
	/// </summary>
	internal class ProductTagRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ProductTag";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ProductTagRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProductTagRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetProductTag 商品タグ情報取得
		/// <summary>
		/// 商品タグ情報取得
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <returns>商品タグ情報</returns>
		internal ProductTagModel GetProductTag(string productId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTTAG_PRODUCT_ID, productId},
			};
			var dv = Get(XML_KEY_NAME, "GetProductTag", ht);
			return dv.Cast<DataRowView>().Select(drv => new ProductTagModel(drv)).FirstOrDefault();
		}
		#endregion

		#region ~GetProductTagSetting 商品タグ設定取得
		/// <summary>
		/// 商品タグ設定取得
		/// </summary>
		/// <returns>商品タグ設定</returns>
		internal ProductTagSettingModel[] GetProductTagSetting()
		{
			var dv = Get(XML_KEY_NAME, "GetProductTagSetting");
			return dv.Cast<DataRowView>().Select(drv => new ProductTagSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetTagSettingList 商品タグ設定一覧取得
		/// <summary>
		/// 商品タグ設定一覧取得
		/// </summary>
		/// <returns>商品タグ設定一覧</returns>
		internal DataView GetTagSettingList()
		{
			var dv = Get(XML_KEY_NAME, "GetTagSettingList");
			return dv;
		}
		#endregion

		#region +Insert
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <returns>Number of rows affected</returns>
		public int Insert(ProductTagModel model)
		{
			var hasProductTagIds = ((model.ProductTagIds != null) && (model.ProductTagIds.Length != 0));
			var replacer = new KeyValuePair<string, string>[]
			{
				new KeyValuePair<string, string>(
					"@@ TagIds @@",
					hasProductTagIds
						? string.Format("{0},", string.Join(",", model.ProductTagIds))
						: string.Empty),
				new KeyValuePair<string, string>(
					"@@ TagValues @@",
					hasProductTagIds
						? string.Format(
							"{0},",
							string.Join(
								",",
								model.ProductTagValues.Select(item =>
									string.Format("N'{0}'", item.Replace("'", "''")))))
						: string.Empty),
			};

			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource, replaces: replacer);
			return result;
		}
		#endregion

		#region +Delete
		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="productId">Product ID</param>
		/// <returns>Number of rows affected</returns>
		public int Delete(string productId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTPRICE_PRODUCT_ID, productId }
			};

			var result = Exec(XML_KEY_NAME, "Delete", input);
			return result;
		}
		#endregion
	}
}
