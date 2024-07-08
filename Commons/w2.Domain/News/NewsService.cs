/*
=========================================================================================================
  Module      : 新着情報サービス (NewsService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Transactions;
using w2.Domain.News.Helper;

namespace w2.Domain.News
{
	/// <summary>
	/// 新着情報サービス
	/// </summary>
	public class NewsService : ServiceBase, INewsService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(NewsListSearchCondition condition)
		{
			using (var repository = new NewsRepository())
			{
				var count = repository.GetSearchHitCount(condition);
				return count;
			}
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public NewsListSearchResult[] Search(NewsListSearchCondition condition)
		{
			using (var repository = new NewsRepository())
			{
				var results = repository.Search(condition);
				return results;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="newsId">新着ID</param>
		/// <returns>モデル</returns>
		public NewsModel Get(string shopId, string newsId)
		{
			using (var repository = new NewsRepository())
			{
				var model = repository.Get(shopId, newsId);
				return model;
			}
		}
		#endregion

		#region +GetNewsList
		/// <summary>
		/// Get the news list count
		/// </summary>
		/// <param name="shopId">The shop identifier</param>
		/// <param name="brandId">The brand identifier</param>
		/// <param name="mobileDispFlg">The mobile disp FLG.</param>
		/// <returns>The news list count</returns>
		public int GetNewsListCount(string shopId, string brandId, string mobileDispFlg)
		{
			using (var repository = new NewsRepository())
			{
				return repository.GetNewsListCount(shopId, brandId, mobileDispFlg);
			}
		}

		/// <summary>
		/// Get news list
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="brandId">Brand id</param>
		/// <param name="mobileDispFlg">Mobile disp flg</param>
		/// <param name="beginRowNumber">Begin row number</param>
		/// <param name="endRowNumber">End row number</param>
		/// <returns>Model</returns>
		public NewsModel[] GetNewsList(string shopId, string brandId, string mobileDispFlg, int beginRowNumber = 0, int endRowNumber = 0)
		{
			using (var repository = new NewsRepository())
			{
				return repository.GetNewsList(shopId, brandId, mobileDispFlg, beginRowNumber, endRowNumber);
			}
		}
		#endregion

		#region +GetTopNewsList
		/// <summary>
		/// Get top news list
		/// </summary>
		/// <returns>List top news</returns>
		public NewsModel[] GetTopNewsList()
		{
			using (var repository = new NewsRepository())
			{
				return repository.GetTopNewsList();
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(NewsModel model)
		{
			using (var repository = new NewsRepository())
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
		public int Update(NewsModel model)
		{
			using (var repository = new NewsRepository())
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +UpdateDisplayOrder
		/// <summary>
		/// Update display order
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="newsId">News id</param>
		public void UpdateDisplayOrder(string shopId, string newsId)
		{
			using (var scope = new TransactionScope())
			using (var repository = new NewsRepository())
			{
				repository.UpdateDisplayOrder(shopId, newsId);
				scope.Complete();
			}
		}
		#endregion

		#region +UpdateDispFlg
		/// <summary>
		/// Update display flag
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="newsId">News id</param>
		/// <param name="dispFlg">Disp flg</param>
		public bool UpdateDispFlg(string shopId, string newsId, string dispFlg)
		{
			using (var scope = new TransactionScope())
			using (var repository = new NewsRepository())
			{
				var model = repository.Get(shopId, newsId);
				model.DispFlg = dispFlg;

				var updated = repository.Update(model);
				scope.Complete();

				return (updated > 0);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="newsId">新着ID</param>
		public void Delete(string shopId, string newsId)
		{
			using (var repository = new NewsRepository())
			{
				repository.Delete(shopId, newsId);
			}
		}
		#endregion
	}
}
