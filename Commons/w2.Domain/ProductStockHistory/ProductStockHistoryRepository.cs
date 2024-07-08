/*
=========================================================================================================
  Module      : 商品在庫履歴リポジトリ (ProductStockHistoryRepository.cs)
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

namespace w2.Domain.ProductStockHistory
{
	/// <summary>
	/// 商品在庫履歴リポジトリ
	/// </summary>
	internal class ProductStockHistoryRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ProductStockHistory";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ProductStockHistoryRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ProductStockHistoryRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion
		/*
		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(ProductStockHistoryListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion
		*/
		/*
		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal ProductStockHistoryListSearchResult[] Search(ProductStockHistoryListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new ProductStockHistoryListSearchResult(drv)).ToArray();
		}
		#endregion
		*/
		/*
		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="historyNo">履歴NO</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <returns>モデル</returns>
		internal ProductStockHistoryModel Get(long historyNo, string shopId, string productId, string variationId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTSTOCKHISTORY_HISTORY_NO, historyNo},
				{Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, productId},
				{Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, variationId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new ProductStockHistoryModel(dv[0]);
		}
		#endregion
		*/

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Insert(ProductStockHistoryModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}
		#endregion

		/*
		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(ProductStockHistoryModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion
		*/
		/*
		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="historyNo">履歴NO</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		internal int Delete(long historyNo, string shopId, string productId, string variationId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTSTOCKHISTORY_HISTORY_NO, historyNo},
				{Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, shopId},
				{Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, productId},
				{Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, variationId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion
		*/
	}
}
