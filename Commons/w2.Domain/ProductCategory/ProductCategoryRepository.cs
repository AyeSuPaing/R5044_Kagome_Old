/*
=========================================================================================================
  Module      : 商品カテゴリリポジトリ (ProductCategoryRepository.cs)
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

namespace w2.Domain.ProductCategory
{
	/// <summary>
	/// 商品カテゴリリポジトリ
	/// </summary>
	internal class ProductCategoryRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ProductCategory";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ProductCategoryRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ProductCategoryRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetAll 全て取得
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <returns>モデル</returns>
		public ProductCategoryModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			return dv.Cast<DataRowView>().Select(drv => new ProductCategoryModel(drv)).ToArray();
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>モデル</returns>
		public ProductCategoryModel Get(string categoryId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID, categoryId}
			};

			var dv = Get(XML_KEY_NAME, "Get", input);
			return dv.Count == 0 ? null : new ProductCategoryModel(dv[0]);
		}
		#endregion

		#region +GetByCategoryIds カテゴリを取得
		/// <summary>
		/// カテゴリを取得
		/// </summary>
		/// <param name="categoryIds">カテゴリID</param>
		/// <returns>モデル</returns>
		public ProductCategoryModel[] GetByCategoryIds(params string[] categoryIds)
		{
			if (categoryIds.Length == 0) return new ProductCategoryModel[0];

			var replace = new KeyValuePair<string, string>(
				"@@ ids @@",
				string.Join(
					",",
					categoryIds.Select(id => string.Format("'{0}'", id.Replace("'", "''")))));

			var dv = Get(XML_KEY_NAME, "GetByCategoryIds", replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new ProductCategoryModel(drv)).ToArray();
		}
		#endregion

		#region +CheckCategoryByFixedPurchaseMemberFlg
		/// <summary>
		/// Check Category By Fixed Purchase Member Flg
		/// </summary>
		/// <param name="categoryId">The category Id</param>
		/// <param name="fixedPurchaseMemberFlg">The Fixed Purchase Member Flg</param>
		/// <returns>Is Access (True: Allow/ Flase: Deny</returns>
		public bool CheckCategoryByFixedPurchaseMemberFlg(string categoryId, string fixedPurchaseMemberFlg)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID, categoryId},
				{ Constants.FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG, fixedPurchaseMemberFlg}
			};

			var result = Get(XML_KEY_NAME, "CheckCategoryByFixedPurchaseMemberFlg", input);

			return (result.Count > 0);
		}
		# endregion

		#region ~CheckValidProductCategory 有効なカテゴリID存在チェック
		/// <summary>
		/// 有効なカテゴリID存在チェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productCategoryIdsList">商品カテゴリIDリスト</param>
		/// <returns>有効なカテゴリID</returns>
		internal string[] CheckValidProductCategory(
			string shopId,
			string[] productCategoryIdsList)
		{
			var ht = new Hashtable()
			{
				{ Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, shopId },
			};

			var replace = new KeyValuePair<string, string>(
				"@@ category_ids @@",
				string.Join(",", productCategoryIdsList.Select(categoryId => "'" + categoryId + "'").ToArray()));

			var dv = Get(XML_KEY_NAME, "CheckValidProductCategory", ht, replaces: replace);
			return (dv.Count > 0)
				? dv.Cast<DataRowView>().Select(drv => (string)drv[0]).ToArray()
				: new string[0];
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, "GetProductCategoryMaster", input, replaces);
			return reader;
		}

		/// <summary>
		/// マスタ取得（Excel出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>DataView</returns>
		public DataView GetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, "GetProductCategoryMaster", input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckProductCategoryFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckProductCategoryFields", input, replaces: replaces);
		}
		#endregion
	}
}
