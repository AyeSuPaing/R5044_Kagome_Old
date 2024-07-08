/*
=========================================================================================================
  Module      : 新着情報リポジトリ (NewsRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.News.Helper;

namespace w2.Domain.News
{
	/// <summary>
	/// 新着情報リポジトリ
	/// </summary>
	public class NewsRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "News";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public NewsRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public NewsRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(NewsListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
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
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new NewsListSearchResult(drv)).ToArray();
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
			var ht = new Hashtable
			{
				{Constants.FIELD_NEWS_SHOP_ID, shopId},
				{Constants.FIELD_NEWS_NEWS_ID, newsId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new NewsModel(dv[0]);
		}
		#endregion

		#region +GetNewsList

		/// <summary>
		/// Get the news list count.
		/// </summary>
		/// <param name="shopId">The shop identifier.</param>
		/// <param name="brandId">The brand identifier.</param>
		/// <param name="mobileDispFlg">The mobile disp FLG.</param>
		/// <returns>The news list count</returns>
		public int GetNewsListCount(string shopId, string brandId, string mobileDispFlg)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_NEWS_SHOP_ID, shopId},
				{Constants.FIELD_NEWS_BRAND_ID, brandId},
				{Constants.FIELD_NEWS_MOBILE_DISP_FLG, mobileDispFlg}
			};

			var result = Get(XML_KEY_NAME, "GetNewsListCount", input);
			return (int)result[0][0];
		}

		/// <summary>
		/// Get news list
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="brandId">Brand id</param>
		/// <param name="mobileDispFlg">Mobile disp flg</param>
		/// <param name="beginRowNumber">Begin row number</param>
		/// <param name="endRowNumber">End row number</param>
		/// <returns>List news </returns>
		public NewsModel[] GetNewsList(string shopId, string brandId, string mobileDispFlg, int beginRowNumber = 0, int endRowNumber = 0)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_NEWS_SHOP_ID, shopId},
				{Constants.FIELD_NEWS_BRAND_ID, brandId},
				{Constants.FIELD_NEWS_MOBILE_DISP_FLG, mobileDispFlg},
				{"bgn_row_num", beginRowNumber},
				{"end_row_num", endRowNumber}
			};

			var data = Get(XML_KEY_NAME, "GetNewsList", input);
			if (data.Count == 0) return null;

			return data.Cast<DataRowView>().Select(item => new NewsModel(item)).ToArray();
		}
		#endregion

		#region +GetTopNewsList
		/// <summary>
		/// Get top news list
		/// </summary>
		/// <returns>List top news </returns>
		public NewsModel[] GetTopNewsList()
		{
			var input = new Hashtable();
			var data = Get(XML_KEY_NAME, "GetTopNewsList", input);

			return data.Cast<DataRowView>().Select(item => new NewsModel(item)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(NewsModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(NewsModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +UpdateDisplayOrder
		/// <summary>
		/// Update display order
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="newsId">News id</param>
		/// <returns>Number of result data</returns>
		public int UpdateDisplayOrder(string shopId, string newsId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_NEWS_SHOP_ID, shopId},
				{Constants.FIELD_NEWS_NEWS_ID, newsId}
			};
			var result = Exec(XML_KEY_NAME, "UpdateDisplayOrder", input);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="newsId">新着ID</param>
		public int Delete(string shopId, string newsId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_NEWS_SHOP_ID, shopId},
				{Constants.FIELD_NEWS_NEWS_ID, newsId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion
	}
}
