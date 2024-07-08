/*
=========================================================================================================
  Module      : レコメンド設定サービス (RecommendService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using System.Transactions;
using w2.Common.Sql;
using w2.Domain.Product;
using w2.Domain.Recommend.Helper;

namespace w2.Domain.Recommend
{
	/// <summary>
	/// レコメンド設定サービス
	/// </summary>
	public class RecommendService : ServiceBase, IRecommendService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(RecommendListSearchCondition condition)
		{
			using (var repository = new RecommendRepository())
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
		public RecommendListSearchResult[] Search(RecommendListSearchCondition condition)
		{
			using (var repository = new RecommendRepository())
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
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public RecommendModel Get(string shopId, string recommendId, SqlAccessor accessor = null)
		{
			using (var repository = new RecommendRepository(accessor))
			{
				var model = repository.Get(shopId, recommendId);
				if (model == null) return null;

				this.SetChildModel(model, repository);

				return model;
			}
		}
		#endregion

		#region +GetContainer 取得（表示用）
		/// <summary>
		/// 取得（表示用）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>モデル</returns>
		public RecommendContainer GetContainer(string shopId, string recommendId)
		{
			using (var repository = new RecommendRepository())
			{
				var model = repository.Get(shopId, recommendId);
				if (model == null) return null;

				// レコメンド設定セット
				var container = new RecommendContainer(model.DataSource);

				// レコメンド適用条件アイテムセット
				container.ApplyConditionItems =
					repository.GetApplyConditionItemsAllContainer(container.ShopId, container.RecommendId);

				// アップセル？
				if (container.IsUpsell)
				{
					// レコメンドアップセル対象アイテム
					container.UpsellTargetItem = repository.GetUpsellTargetItemContainer(container.ShopId, container.RecommendId);
				}

				// レコメンドアイテム
				container.Items = (container.IsUpsell || container.IsCrosssell)
					? repository.GetItemsAllContainer(container.ShopId, container.RecommendId)
					: new RecommendItemContainer[0];

				return container;
			}
		}
		#endregion

		#region +GetAll 全て取得
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル列</returns>
		public RecommendModel[] GetAll(string shopId)
		{
			using (var repository = new RecommendRepository())
			{
				var models = repository.GetAll(shopId);
				foreach (var model in models)
				{
					this.SetChildModel(model, repository);
				}
				return models;
			}
		}
		#endregion

		#region -SetChildModel 子モデルをセット（レコメンド適用条件アイテム・レコメンドアップセル対象アイテム・レコメンドアイテム）
		/// <summary>
		/// 子モデルをセット(レコメンド適用条件アイテム・レコメンドアップセル対象アイテム・レコメンドアイテム)
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="repository">リポジトリ</param>
		private void SetChildModel(RecommendModel model, RecommendRepository repository)
		{
			// レコメンド適用条件アイテムセット
			model.ApplyConditionItems = repository.GetApplyConditionItemsAll(model.ShopId, model.RecommendId);
			// アップセル？
			if (model.IsUpsell)
			{
				// レコメンドアップセル対象アイテム
				model.UpsellTargetItem = repository.GetUpsellTargetItem(model.ShopId, model.RecommendId);
			}
			// レコメンドアイテム
			model.Items = repository.GetItemsAll(model.ShopId, model.RecommendId);
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(RecommendModel model)
		{
			using (var repository = new RecommendRepository())
			{
				// 登録
				repository.Insert(model);

				// レコメンド適用条件アイテム登録
				foreach (var applyConditionItem in model.ApplyConditionItems)
				{
					repository.InsertApplyConditionItem(applyConditionItem);
				}

				// レコメンドアップセル対象アイテム登録
				if (model.IsUpsell)
				{
					repository.InsertUpsellTargetItem(model.UpsellTargetItem);
				}

				// レコメンドアイテム登録
				foreach (var item in model.Items)
				{
					repository.InsertItem(item);
				}
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(RecommendModel model, SqlAccessor accessor = null)
		{
			using (var repository = new RecommendRepository(accessor))
			{
				// 更新
				var result = repository.Update(model);

				// レコメンド適用条件アイテム登録（DELETE → INSERT）
				repository.DeleteApplyConditionItemsAll(model.ShopId, model.RecommendId);
				foreach (var applyConditionItem in model.ApplyConditionItems)
				{
					repository.InsertApplyConditionItem(applyConditionItem);
				}

				// レコメンドアップセル対象アイテム登録（DELETE → INSERT）
				repository.DeleteUpsellTargetItem(model.ShopId, model.RecommendId);
				if (model.IsUpsell)
				{
					repository.InsertUpsellTargetItem(model.UpsellTargetItem);
				}

				// レコメンドアイテム登録（DELETE → INSERT）
				repository.DeleteItemsAll(model.ShopId, model.RecommendId);
				foreach (var item in model.Items)
				{
					repository.InsertItem(item);
				}

				return result;
			}
		}
		#endregion

		#region +UpdatePriority 適用優先順更新
		/// <summary>
		/// 適用優先順更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="priority">適用優先順</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdatePriority(string shopId, string recommendId, int priority, string lastChanged, SqlAccessor accessor)
		{
			// 最新データ取得
			var model = this.Get(shopId, recommendId, accessor);

			// 適用優先順をセット
			model.Priority = priority;

			// 最終更新者をセット
			model.LastChanged = lastChanged;

			// 更新
			return this.Update(model, accessor);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		public int Delete(string shopId, string recommendId)
		{
			using (var scope = new TransactionScope())
			using (var repository = new RecommendRepository())
			{
				// 削除
				var result = repository.Delete(shopId, recommendId);
				// レコメンド適用条件アイテム
				repository.DeleteApplyConditionItemsAll(shopId, recommendId);
				// レコメンドアップセル対象アイテム
				repository.DeleteUpsellTargetItem(shopId, recommendId);
				// レコメンドアイテム
				repository.DeleteItemsAll(shopId, recommendId);

				// トランザクション完了
				scope.Complete();

				return result;
			}
		}
		#endregion

		#region +GetRecommendHistory レコメンド表示履歴取得
		/// <summary>
		/// レコメンド表示履歴取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="recommendHistoryNo">履歴枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>レコメンド表示履歴</returns>
		public RecommendHistoryModel GetRecommendHistory(string shopId, string recommendId, string userId, int recommendHistoryNo, SqlAccessor accessor = null)
		{
			using (var repository = new RecommendRepository(accessor))
			{
				var model = repository.GetRecommendHistory(shopId, recommendId, userId, recommendHistoryNo);
				return model;
			}
		}
		#endregion

		#region +GetRecommendHistoryNo
		/// <summary>
		/// Get max history no number
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="userId">User ID</param>
		/// <param name="recommendId">Recommend ID</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Max history no</returns>
		public int GetMaxRecommendHistoryNo(
			string shopId,
			string userId,
			string recommendId,
			SqlAccessor accessor = null)
		{
			var histories = GetRecommendHistoryByUserId(shopId, userId, accessor);
			var historyNo = histories.Any(history => history.RecommendId == recommendId)
				? histories.Where(history => history.RecommendId == recommendId)
					.Max(history => history.RecommendHistoryNo) + 1
				: 1;
			return historyNo;
		}
		#endregion

		#region +GetRecommendHistoryByUserId ユーザーIDからレコメンド表示履歴を取得
		/// <summary>
		/// ユーザーIDからレコメンド表示履歴を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>レコメンド表示履歴</returns>
		public RecommendHistoryModel[] GetRecommendHistoryByUserId(string shopId, string userId, SqlAccessor accessor = null)
		{
			using (var repository = new RecommendRepository(accessor))
			{
				var models = repository.GetRecommendHistoryByUserId(shopId, userId);
				return models;
			}
		}
		#endregion

		#region GetNewRecommendHistoryNoAndInsertRecommendHistory 履歴枝番を採番してレコメンド表示履歴登録
		/// <summary>
		/// 履歴枝番を採番してレコメンド表示履歴登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>履歴枝番</returns>
		public int GetNewRecommendHistoryNoAndInsertRecommendHistory(RecommendHistoryModel model)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				model.RecommendHistoryNo = GetMaxRecommendHistoryNo(model.ShopId, model.UserId, model.RecommendId, accessor);
				InsertRecommendHistory(model, accessor);

				accessor.CommitTransaction();

				return model.RecommendHistoryNo;
			}
		}
		#endregion

		#region +InsertRecommendHistory レコメンド表示履歴登録
		/// <summary>
		/// レコメンド表示履歴登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertRecommendHistory(RecommendHistoryModel model, SqlAccessor accessor = null)
		{
			using (var repository = new RecommendRepository(accessor))
			{
				repository.InsertRecommendHistory(model);
			}
		}
		#endregion

		#region +UpdateRecommendHistory レコメンド表示履歴更新
		/// <summary>
		/// レコメンド表示履歴更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateRecommendHistory(RecommendHistoryModel model, SqlAccessor accessor = null)
		{
			using (var repository = new RecommendRepository())
			{
				repository.UpdateRecommendHistory(model);
			}
		}
		#endregion

		#region +UpdateDispOrderedFlg レコメンド表示履歴の購入フラグを購入済に更新（注文ID追加可能）
		/// <summary>
		/// レコメンド表示履歴の購入フラグを購入済に更新（注文ID追加可能）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="recommendHistoryNo">履歴枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="orderId">注文ID</param>
		public void UpdateBuyOrderedFlg(string shopId, string recommendId, string userId, int recommendHistoryNo, string lastChanged, string orderId = null)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var model = GetRecommendHistory(shopId, recommendId, userId, recommendHistoryNo, accessor);
				if (model == null) return;

				model.OrderedFlg = Constants.FLG_RECOMMENDHISTORY_ORDERED_FLG_BUY;
				model.LastChanged = lastChanged;

				if (string.IsNullOrEmpty(orderId) == false)
				{
					model.TargetOrderId = orderId;
				}

				UpdateRecommendHistory(model, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +IsConsistent 整合性チェック
		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>整合性が取れている場合true</returns>
		public bool IsConsistent(string shopId, string recommendId)
		{
			var recommend = Get(shopId, recommendId);
			if (recommend == null) return false;

			if (recommend.IsUpsell && (recommend.UpsellTargetItem == null))
			{
				return false;
			}

			var productService = new ProductService();
			// 適用条件商品
			var result = recommend
				.ApplyConditionItems
				.All(aci => productService.IsExistingProduct(aci.ShopId, aci.RecommendApplyConditionItemProductId));
			// レコメンド商品
			result &= recommend
				.Items
				.All(i => productService.IsExistingProduct(i.ShopId, i.RecommendItemProductId));
			// アップセル対象商品
			if (recommend.IsUpsell)
			{
				result &= productService.IsExistingProduct(recommend.UpsellTargetItem.ShopId, recommend.UpsellTargetItem.RecommendUpsellTargetItemProductId);
			}
			return result;
		}
		#endregion

		#region +GetRecommendReport レコメンドレポート検索表示
		/// <summary>
		/// レコメンドレポート検索表示
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public RecommendListSearchResult[] GetRecommendReport(RecommendListSearchCondition condition)
		{
			using (var repository = new RecommendRepository())
			{
				var results = repository.GetRecommendReport(condition);
				return results;
			}
		}
		#endregion

		#region +GetRecommendReportHitCount 件数取得（レコメンドレポート検索）
		/// <summary>
		/// 件数取得（レコメンドレポート検索）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetRecommendReportHitCount(RecommendListSearchCondition condition)
		{
			using (var repository = new RecommendRepository())
			{
				var count = repository.GetRecommendReportHitCount(condition);
				return count;
			}
		}
		#endregion

		#region +GetRecommendReportPVNumberTotal 合計PV数取得（レコメンドレポート検索）
		/// <summary>
		/// 合計PV数取得（レコメンドレポート検索）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>合計PV数</returns>
		public int GetRecommendReportPvNumberTotal(RecommendListSearchCondition condition)
		{
			using (var repository = new RecommendRepository())
			{
				var count = repository.GetRecommendReportPvNumberTotal(condition);
				return count;
			}
		}
		#endregion

		#region +GetRecommendReportCVNumberTotal 合計CV数取得（レコメンドレポート検索）
		/// <summary>
		/// 合計CV数取得（レコメンドレポート検索）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>合計CV数</returns>
		public int GetRecommendReportCvNumberTotal(RecommendListSearchCondition condition)
		{
			using (var repository = new RecommendRepository())
			{
				var count = repository.GetRecommendReportCvNumberTotal(condition);
				return count;
			}
		}
		#endregion

		#region +GetRecommendReportGraphPV PV数取得（レコメンドレポートグラフ表示）
		/// <summary>
		/// PV数取得（レコメンドレポートグラフ表示）
		/// </summary>
		/// <param name="dateBgn">開始時間</param>
		/// <param name="dateEnd">終了時間</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>レポートグラフ用日付別集計値</returns>
		public RecommendReportGraphDateModel[] GetRecommendReportGraphPv(
			DateTime dateBgn,
			DateTime dateEnd,
			string shopId,
			string recommendId)
		{
			using (var repository = new RecommendRepository())
			{
				var results = repository.GetRecommendReportGraphPv(dateBgn, dateEnd, shopId, recommendId);
				return results;
			}
		}
		#endregion

		#region +GetRecommendReportGraphCV CV数取得（レコメンドレポートグラフ表示）
		/// <summary>
		/// CV数取得（レコメンドレポートグラフ表示）
		/// </summary>
		/// <param name="dateBgn">開始時間</param>
		/// <param name="dateEnd">終了時間</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>レポートグラフ用日付別集計値</returns>
		public RecommendReportGraphDateModel[] GetRecommendReportGraphCv(
			DateTime dateBgn,
			DateTime dateEnd,
			string shopId,
			string recommendId)
		{
			using (var repository = new RecommendRepository())
			{
				var results = repository.GetRecommendReportGraphCv(dateBgn, dateEnd, shopId, recommendId);
				return results;
			}
		}
		#endregion
	}
}