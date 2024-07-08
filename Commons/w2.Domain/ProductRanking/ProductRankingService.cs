/*
=========================================================================================================
  Module      : 商品ランキング設定サービス (ProductRankingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;

namespace w2.Domain.ProductRanking
{
	/// <summary>
	/// 商品ランキング設定サービス
	/// </summary>
	public class ProductRankingService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="rankingId">商品ランキングID</param>
		/// <returns>モデル</returns>
		public ProductRankingModel Get(string shopId, string rankingId)
		{
			using (var repository = new ProductRankingRepository())
			{
				var model = repository.Get(shopId, rankingId);
				if (model == null) return null;

				var items = repository.GetItems(shopId, rankingId);
				model.Items = items;
				return model;
			}
		}
		#endregion

		#region +GetAll 取得（全て）
		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <returns>モデル</returns>
		public ProductRankingModel[] GetAll()
		{
			using (var repository = new ProductRankingRepository())
			{
				// 取得
				var models = repository.GetAll();
				foreach (var model in models)
				{
					model.Items = repository.GetItems(model.ShopId, model.RankingId);
				}
				return models;
			}
		}
		#endregion

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="rankingId">商品ランキングID</param>
		/// <param name="totalType">集計タイプ</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(string shopId, string rankingId, string totalType, string validFlg)
		{
			using (var repository = new ProductRankingRepository())
			{
				var count = repository.GetSearchHitCount(shopId, rankingId, totalType, validFlg);
				return count;
			}
		}
		#endregion

		#region +Search 検索
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
		public ProductRankingModel[] Search(string shopId, string rankingId, string totalType, string validFlg, string sortKbn, int beginNum, int endNum)
		{
			using (var repository = new ProductRankingRepository())
			{
				var results = repository.Search(shopId, rankingId, totalType, validFlg, sortKbn, beginNum, endNum);
				return results;
			}
		}
		#endregion

		#region +GetForDisplay 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="rankingId">商品ランキングID</param>
		/// <returns>モデル</returns>
		public ProductRankingModel GetForDisplay(string shopId, string rankingId)
		{
			using (var repository = new ProductRankingRepository())
			{
				var model = repository.Get(shopId, rankingId);
				if (model == null) return null;

				var items = repository.GetItemsForDisplay(shopId, rankingId);
				model.Items = items;
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(ProductRankingModel model)
		{
			using (var repository = new ProductRankingRepository())
			{
				repository.Insert(model);
				model.Items.ToList().ForEach(item => repository.InsertItem(item));
				if (model.TotalType == Constants.FLG_PRODUCTRANKING_TOTAL_TYPE_MANUAL)
				{
					repository.InsertDisplayProductRanking(model);
				}
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int Update(ProductRankingModel model)
		{
			using (var repository = new ProductRankingRepository())
			{
				var result = repository.Update(model);
				repository.DeleteItemAll(model.ShopId, model.RankingId);
				model.Items.ToList().ForEach(item => repository.InsertItem(item));
				if (model.TotalType == Constants.FLG_PRODUCTRANKING_TOTAL_TYPE_MANUAL)
				{
					repository.InsertDisplayProductRanking(model);
				}

				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="rankingId">商品ランキングID</param>
		public void Delete(string shopId, string rankingId)
		{
			using (var repository = new ProductRankingRepository())
			{
				repository.Delete(shopId, rankingId);
				repository.DeleteItemAll(shopId, rankingId);
			}
		}
		#endregion
	}
}
