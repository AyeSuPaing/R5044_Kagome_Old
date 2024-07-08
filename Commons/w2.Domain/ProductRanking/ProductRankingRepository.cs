/*
=========================================================================================================
  Module      : 商品ランキング設定リポジトリ (ProductRankingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ProductRanking
{
	/// <summary>
	/// 商品ランキング設定リポジトリ
	/// </summary>
	internal class ProductRankingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ProductRanking";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ProductRankingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ProductRankingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="rankingId">商品ランキングID</param>
		/// <returns>モデル</returns>
		internal ProductRankingModel Get(string shopId, string rankingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTRANKING_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTRANKING_RANKING_ID, rankingId},
			};
			var dv = Get(XML_KEY_NAME, "GetProductRanking", ht);
			if (dv.Count == 0) return null;
			return new ProductRankingModel(dv[0]);
		}
		#endregion

		#region +GetAll 取得（全て）
		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <returns>モデル</returns>
		public ProductRankingModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetProductRankingAll", null);
			return dv.Cast<DataRowView>().Select(drv => new ProductRankingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetItems アイテム取得
		/// <summary>
		/// アイテム取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="rankingId">商品ランキングID</param>
		/// <returns>モデル</returns>
		internal ProductRankingItemModel[] GetItems(string shopId, string rankingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTRANKINGITEM_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTRANKINGITEM_RANKING_ID, rankingId}
			};
			var dv = Get(XML_KEY_NAME, "GetProductRankingItem", ht);
			return dv.Cast<DataRowView>().Select(drv => new ProductRankingItemModel(drv)).ToArray();
		}
		#endregion

		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="rankingId">商品ランキングID</param>
		/// <param name="totalType">集計タイプ</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(string shopId, string rankingId, string totalType, string validFlg)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTRANKING_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTRANKING_RANKING_ID + "_like_escaped", rankingId},
				{Constants.FIELD_PRODUCTRANKING_TOTAL_TYPE, totalType},
				{Constants.FIELD_PRODUCTRANKING_VALID_FLG, validFlg},
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
		/// <param name="rankingId">商品ランキングID</param>
		/// <param name="totalType">集計タイプ</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="sortKbn">並び順</param>
		/// <param name="beginNum">取得開始番号</param>
		/// <param name="endNum">取得終了番号</param>
		/// <returns>件数</returns>
		internal ProductRankingModel[] Search(string shopId, string rankingId, string totalType, string validFlg, string sortKbn, int beginNum, int endNum)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTRANKING_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTRANKING_RANKING_ID + "_like_escaped", rankingId},
				{Constants.FIELD_PRODUCTRANKING_TOTAL_TYPE, totalType},
				{Constants.FIELD_PRODUCTRANKING_VALID_FLG, validFlg},
				{"sort_kbn", sortKbn},
				{Constants.FIELD_COMMON_BEGIN_NUM, beginNum},
				{Constants.FIELD_COMMON_END_NUM, endNum},
			};
			var dv = Get(XML_KEY_NAME, "Search", ht);
			return dv.Cast<DataRowView>().Select(drv => new ProductRankingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetItemsForDisplay 表示用アイテム取得
		/// <summary>
		/// 表示用アイテム取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="rankingId">商品ランキングID</param>
		/// <returns>モデル</returns>
		internal ProductRankingItemModel[] GetItemsForDisplay(string shopId, string rankingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTRANKINGITEM_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTRANKINGITEM_RANKING_ID, rankingId}
			};
			var dv = Get(XML_KEY_NAME, "GetProductRankingItemForDisplay", ht);
			return dv.Cast<DataRowView>().Select(drv => new ProductRankingItemModel(drv)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(ProductRankingModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +InsertItem ランキング登録
		/// <summary>
		/// ランキング登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertItem(ProductRankingItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertItem", model.DataSource);
		}
		#endregion

		#region +InsertDisplayProductRanking 商品表示情報登録
		/// <summary>
		/// 商品表示情報登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertDisplayProductRanking(ProductRankingModel model)
		{
			Exec(XML_KEY_NAME, "CreateDisplayProductRanking", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(ProductRankingModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="rankingId">商品ランキングID</param>
		public int Delete(string shopId, string rankingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTRANKING_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTRANKING_RANKING_ID, rankingId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region +DeleteItemAll ランキング削除
		/// <summary>
		/// ランキング削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="rankingId">商品ランキングID</param>
		public int DeleteItemAll(string shopId, string rankingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTRANKING_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTRANKING_RANKING_ID, rankingId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteItemAll", ht);
			return result;
		}
		#endregion
	}
}
