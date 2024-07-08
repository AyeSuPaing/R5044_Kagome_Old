/*
=========================================================================================================
  Module      : 特集ページカテゴリサービスリポジトリ (FeaturePageCategoryRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.FeaturePageCategory
{
	/// <summary>
	/// 特集ページカテゴリサービスリポジトリ
	/// </summary>
	internal class FeaturePageCategoryRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "FeaturePageCategory";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal FeaturePageCategoryRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal FeaturePageCategoryRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>モデル</returns>
		internal FeaturePageCategoryModel Get(string categoryId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FEATURE_PAGE_CATEGORY_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID },
				{ Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_ID, categoryId }
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			var result = (dv.Count == 0) ? null : new FeaturePageCategoryModel(dv[0]);
			return result;
		}
		#endregion

		#region +GetChildCategoriesByParentCategoryId 親カテゴリに紐づく子カテゴリをすべて取得
		/// <summary>
		/// 親カテゴリに紐づく子カテゴリをすべて取得
		/// </summary>
		/// <param name="parentCategoryId">親カテゴリID</param>
		/// <returns>親カテゴリに紐づく子カテゴリ一覧</returns>
		internal FeaturePageCategoryModel[] GetChildCategoriesByParentCategoryId(string parentCategoryId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FEATURE_PAGE_CATEGORY_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID },
				{ Constants.FIELD_FEATURE_PAGE_CATEGORY_PARENT_CATEGORY_ID, parentCategoryId }
			};
			var dv = Get(XML_KEY_NAME, "GetChildCategoriesByParentCategoryId", ht);
			var result = dv.Cast<DataRowView>().Select(item => new FeaturePageCategoryModel(item)).ToArray();
			return result;
		}
		#endregion

		#region ~GetParentCategories 指定カテゴリIDと、その上位のカテゴリ(TOPカテゴリまで)のリストを取得する
		/// <summary>
		/// 指定カテゴリIDと、その上位のカテゴリ(TOPカテゴリまで)のリストを取得する
		/// </summary>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>カテゴリーツリー</returns>
		internal FeaturePageCategoryModel[] GetParentCategories(string categoryId)
			{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FEATURE_PAGE_CATEGORY_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID },
				{Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_ID, categoryId},
			};
			var dv = Get(XML_KEY_NAME, "GetParentCategories", ht);
			return dv.Cast<DataRowView>().Select(drv => new FeaturePageCategoryModel(drv)).ToArray();
		}
		#endregion

		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="sortCategoryId">検索カテゴリID</param>
		/// <param name="categoryName">検索カテゴリ名</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(string shopId, string sortCategoryId, string categoryName)
			{
			var ht = new Hashtable
			{
				{Constants.FIELD_FEATURE_PAGE_CATEGORY_SHOP_ID, shopId},
				{Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_ID, sortCategoryId},
				{Constants.FIELD_FEATURE_PAGE_CATEGORY_NAME, categoryName},
			};
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetSearch 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="categoryName">カテゴリ名</param>
		/// <param name="beginNum">取得開始番号</param>
		/// <param name="endNum">取得終了番号</param>
		/// <returns>件数</returns>
		internal FeaturePageCategoryModel[] Search(string shopId, string categoryId, string categoryName, int beginNum, int endNum)
			{
			var ht = new Hashtable
			{
				{Constants.FIELD_FEATURE_PAGE_CATEGORY_SHOP_ID, shopId},
				{Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_ID, categoryId},
				{Constants.FIELD_FEATURE_PAGE_CATEGORY_NAME, categoryName},
				{Constants.FIELD_COMMON_BEGIN_NUM, beginNum},
				{Constants.FIELD_COMMON_END_NUM, endNum},
			};
			var dv = Get(XML_KEY_NAME, "Search", ht);
			return dv.Cast<DataRowView>().Select(drv => new FeaturePageCategoryModel(drv)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(Hashtable model)
		{
			Exec(XML_KEY_NAME, "Insert", model);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(Hashtable model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="ht">特集ページカテゴリ情報</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(Hashtable ht)
		{
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region +GetRootCategoryItem 最上位カテゴリを全て取得
		/// <summary>
		/// 最上位カテゴリを全て取得
		/// </summary>
		/// <returns>最上位カテゴリ情報</returns>
		public FeaturePageCategoryModel[] GetRootCategoryItem()
		{
			var dv = Get(XML_KEY_NAME, "GetRootCategoryItem");
			return dv.Cast<DataRowView>().Select(item => new FeaturePageCategoryModel(item)).ToArray();
		}
		#endregion

		#region +GetMatchingCategoryId 一致するカテゴリIDがあるか確認
		/// <summary>
		/// 一致するカテゴリIDがあるか確認
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>一致したカテゴリ数</returns>
		public int GetMatchingCategoryId(string shopId, string categoryId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FEATURE_PAGE_CATEGORY_SHOP_ID, shopId},
				{ Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_ID, categoryId },
			};
			var dv = Get(XML_KEY_NAME, "GetMatchingCategoryId", ht);
			return (int)dv[0][0];
		}
		#endregion
	}
}