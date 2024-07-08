/*
=========================================================================================================
  Module      : 特集ページカテゴリサービス (FeaturePageCategoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;

namespace w2.Domain.FeaturePageCategory
{
	/// <summary>
	/// 特集ページカテゴリサービス
	/// </summary>
	public class FeaturePageCategoryService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>モデル</returns>
		public FeaturePageCategoryModel Get(string categoryId)
		{
			using (var repository = new FeaturePageCategoryRepository())
			{
				var model = repository.Get(categoryId);
				return model;
			}
		}
		#endregion

		#region +GetChildCategoriesByParentCategoryId 親カテゴリに紐づく子カテゴリをすべて取得
		/// <summary>
		/// 親カテゴリに紐づく子カテゴリをすべて取得
		/// </summary>
		/// <param name="parentCategoryId">親カテゴリID</param>
		/// <returns>親カテゴリに紐づく子カテゴリ一覧</returns>
		public FeaturePageCategoryModel[] GetChildCategoriesByParentCategoryId(string parentCategoryId)
		{
			using (var repository = new FeaturePageCategoryRepository())
			{
				var model = repository.GetChildCategoriesByParentCategoryId(parentCategoryId);
				return model;
			}
		}
		#endregion

		#region +GetParentCategories 指定カテゴリIDと、その上位のカテゴリ(TOPカテゴリまで)のリストを取得する
		/// <summary>
		/// カテゴリツリーを取得
		/// </summary>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>カテゴリーツリー</returns>
		public FeaturePageCategoryModel[] GetParentCategories(string categoryId)
		{
			using (var repository = new FeaturePageCategoryRepository())
			{
				var models = repository.GetParentCategories(categoryId);
				return models;
			}
		}
		#endregion

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(string shopId, string categoryId, string validFlg)
		{
			using (var repository = new FeaturePageCategoryRepository())
			{
				var count = repository.GetSearchHitCount(shopId, categoryId, validFlg);
				return count;
			}
		}
		#endregion
		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="categoryName">カテゴリ名</param>
		/// <param name="beginNum">取得開始番号</param>
		/// <param name="endNum">取得終了番号</param>
		/// <returns>件数</returns>
		public FeaturePageCategoryModel[] Search(string shopId, string categoryId, string categoryName, int beginNum, int endNum)
		{
			using (var repository = new FeaturePageCategoryRepository())
			{
				var results = repository.Search(shopId, categoryId, categoryName, beginNum, endNum);
				return results;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(Hashtable model)
		{
			using (var repository = new FeaturePageCategoryRepository())
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
		public int Update(Hashtable model)
		{
			using (var repository = new FeaturePageCategoryRepository())
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
		/// <param name="ht">特集ページカテゴリ情報</param>
		public void Delete(Hashtable ht)
		{
			using (var repository = new FeaturePageCategoryRepository())
			{
				repository.Delete(ht);
			}
		}
		#endregion

		#region GetRootCategoryItem 最上位カテゴリ取得

		/// <summary>
		/// 最上位カテゴリ取得
		/// </summary>
		/// <returns>最上位カテゴリ一覧</returns>
		public FeaturePageCategoryModel[] GetRootCategoryItem()
		{
			using (var repository = new FeaturePageCategoryRepository())
			{
				var results = repository.GetRootCategoryItem();
				return results;
			}
		}
		#endregion

		#region GetMatchingCategoryId 一致するカテゴリIDがあるか確認
		/// <summary>
		/// 一致するカテゴリIDがあるか確認
		/// </summary>
		/// <param name="shopId">特集ページID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>一致したカテゴリ数</returns>
		public int GetMatchingCategoryId(string shopId, string categoryId)
		{
			using (var repository = new FeaturePageCategoryRepository())
			{
				var result = repository.GetMatchingCategoryId(shopId, categoryId);
				return result;
			}
		}
		#endregion
	}
}