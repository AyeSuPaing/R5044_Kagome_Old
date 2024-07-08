/*
=========================================================================================================
  Module      : 商品在庫履歴サービス (ProductStockHistoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;
using w2.Common.Sql;

namespace w2.Domain.ProductStockHistory
{
	/// <summary>
	/// 商品在庫履歴サービス
	/// </summary>
	public class ProductStockHistoryService : ServiceBase, IProductStockHistoryService
	{
		/*
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(ProductStockHistoryListSearchCondition condition)
		{
			using (var repository = new ProductStockHistoryRepository())
			{
				var count = repository.GetSearchHitCount(condition);
				return count;
			}
		}
		#endregion
		*/
		/*
		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public ProductStockHistoryListSearchResult[] Search(ProductStockHistoryListSearchCondition condition)
		{
			using (var repository = new ProductStockHistoryRepository())
			{
				var results = repository.Search(condition);
				return results;
			}
		}
		#endregion
		*/
		/*
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="historyNo">履歴NO</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <returns>モデル</returns>
		public ProductStockHistoryModel Get(long historyNo, string shopId, string productId, string variationId)
		{
			using (var repository = new ProductStockHistoryRepository())
			{
				var model = repository.Get(historyNo, shopId, productId, variationId);
				return model;
			}
		}
		#endregion
		*/

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Insert(ProductStockHistoryModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ProductStockHistoryRepository(accessor))
			{
				var result = repository.Insert(model);
				return result;
			}
		}
		#endregion

		/*
		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int Update(ProductStockHistoryModel model)
		{
			using (var repository = new ProductStockHistoryRepository())
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion
		*/
		/*
		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="historyNo">履歴NO</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		public void Delete(long historyNo, string shopId, string productId, string variationId)
		{
			using (var repository = new ProductStockHistoryRepository())
			{
				var result = repository.Delete(historyNo, shopId, productId, variationId);
				return result;
			}
		}
		#endregion
		*/
	}
}
