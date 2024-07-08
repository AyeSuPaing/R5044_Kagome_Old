/*
=========================================================================================================
  Module      : モール連携更新ログサービス (MallCooperationUpdateLogService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;
using w2.Common.Sql;

namespace w2.Domain.MallCooperationUpdateLog
{
	/// <summary>
	/// モール連携更新ログサービス
	/// </summary>
	public class MallCooperationUpdateLogService : ServiceBase
	{
		/*
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(MallCooperationUpdateLogListSearchCondition condition)
		{
			using (var repository = new MallCooperationUpdateLogRepository())
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
		public MallCooperationUpdateLogListSearchResult[] Search(MallCooperationUpdateLogListSearchCondition condition)
		{
			using (var repository = new MallCooperationUpdateLogRepository())
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
		/// <param name="logNo">ログNO</param>
		/// <returns>モデル</returns>
		public MallCooperationUpdateLogModel Get(long logNo)
		{
			using (var repository = new MallCooperationUpdateLogRepository())
			{
				var model = repository.Get(logNo);
				return model;
			}
		}
		#endregion
		*/
		/*
		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(MallCooperationUpdateLogModel model)
		{
			using (var repository = new MallCooperationUpdateLogRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion
		*/
		/*
		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int Update(MallCooperationUpdateLogModel model)
		{
			using (var repository = new MallCooperationUpdateLogRepository())
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
		/// <param name="logNo">ログNO</param>
		public void Delete(long logNo)
		{
			using (var repository = new MallCooperationUpdateLogRepository())
			{
				var result = repository.Delete(logNo);
				return result;
			}
		}
		#endregion
		*/

		#region +GetTargetStockUpdate 在庫連携対象取得
		/// <summary>
		/// 在庫連携対象取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <param name="amazonSkuColumn">AmazonSKU取得列</param>
		/// <param name="fulfilmentColumn">出荷作業日数取得列</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>在庫連携対象リスト</returns>
		public MallCooperationUpdateLogModel[] GetTargetStockUpdate(string shopId, string mallId, string amazonSkuColumn, string fulfilmentColumn, SqlAccessor accessor = null)
		{
			using (var repository = new MallCooperationUpdateLogRepository(accessor))
			{
				var result = repository.GetTargetStockUpdate(shopId, mallId, amazonSkuColumn, fulfilmentColumn);
				return result;
			}
		}
		#endregion

		#region +GetLohacoTargetStockUpdate Lohaco在庫連携対象取得
		/// <summary>
		/// Lohaco在庫連携対象取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>Lohaco在庫連携対象リスト</returns>
		public MallCooperationUpdateLogModel[] GetLohacoTargetStockUpdate(string shopId, string mallId, SqlAccessor accessor = null)
		{
			using (var repository = new MallCooperationUpdateLogRepository(accessor))
			{
				var result = repository.GetLohacoTargetStockUpdate(shopId, mallId);
				return result;
			}
		}
		#endregion

		#region +UpdateExcludedStockUpdate 在庫連携処理対象外更新
		/// <summary>
		/// 在庫連携処理対象外更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <param name="accessor">アクセサ</param>
		public void UpdateExcludedStockUpdate(string shopId, string mallId, SqlAccessor accessor = null)
		{
			using (var repository = new MallCooperationUpdateLogRepository(accessor))
			{
				var result = repository.UpdateExcludedStockUpdate(shopId, mallId);
			}
		}
		#endregion

		#region +UpdateActionStatus 処理ステータス更新
		/// <summary>
		/// 処理ステータス更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="updateLogNoList">更新対象ログNoリスト</param>
		/// <param name="accessor">アクセサ</param>
		public void UpdateActionStatus(MallCooperationUpdateLogModel model, string updateLogNoList, SqlAccessor accessor = null)
		{
			using (var repository = new MallCooperationUpdateLogRepository(accessor))
			{
				var result = repository.UpdateActionStatus(model, updateLogNoList);
			}
		}
		#endregion
	}
}
