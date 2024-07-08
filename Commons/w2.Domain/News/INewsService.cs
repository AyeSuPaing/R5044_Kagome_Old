/*
=========================================================================================================
  Module      : 新着情報サービスのインターフェース (INewsService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.News.Helper;

namespace w2.Domain.News
{
	/// <summary>
	/// 新着情報サービスのインターフェース
	/// </summary>
	public interface INewsService : IService
	{
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(NewsListSearchCondition condition);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		NewsListSearchResult[] Search(NewsListSearchCondition condition);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="newsId">新着ID</param>
		/// <returns>モデル</returns>
		NewsModel Get(string shopId, string newsId);

		/// <summary>
		/// Get the news list count
		/// </summary>
		/// <param name="shopId">The shop identifier</param>
		/// <param name="brandId">The brand identifier</param>
		/// <param name="mobileDispFlg">The mobile disp FLG.</param>
		/// <returns>The news list count</returns>
		int GetNewsListCount(string shopId, string brandId, string mobileDispFlg);

		/// <summary>
		/// Get news list
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="brandId">Brand id</param>
		/// <param name="mobileDispFlg">Mobile disp flg</param>
		/// <param name="beginRowNumber">Begin row number</param>
		/// <param name="endRowNumber">End row number</param>
		/// <returns>Model</returns>
		NewsModel[] GetNewsList(string shopId, string brandId, string mobileDispFlg, int beginRowNumber = 0, int endRowNumber = 0);

		/// <summary>
		/// Get top news list
		/// </summary>
		/// <returns>List top news</returns>
		NewsModel[] GetTopNewsList();

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(NewsModel model);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		int Update(NewsModel model);

		/// <summary>
		/// Update display order
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="newsId">News id</param>
		void UpdateDisplayOrder(string shopId, string newsId);

		/// <summary>
		/// Update display flag
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="newsId">News id</param>
		/// <param name="dispFlg">Disp flg</param>
		bool UpdateDispFlg(string shopId, string newsId, string dispFlg);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="newsId">新着ID</param>
		void Delete(string shopId, string newsId);
	}
}
