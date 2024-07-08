/*
=========================================================================================================
  Module      : レコメンド設定リポジトリ (RecommendRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.Recommend.Helper;

namespace w2.Domain.Recommend
{
	/// <summary>
	/// レコメンド設定リポジトリ
	/// </summary>
	internal class RecommendRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "Recommend";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal RecommendRepository(SqlAccessor accessor = null)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(RecommendListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal RecommendListSearchResult[] Search(RecommendListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new RecommendListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>モデル</returns>
		internal RecommendModel Get(string shopId, string recommendId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_RECOMMEND_SHOP_ID, shopId},
				{Constants.FIELD_RECOMMEND_RECOMMEND_ID, recommendId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			return (dv.Count != 0) ? new RecommendModel(dv[0]) : null;
		}
		#endregion

		#region ~GetAll 全て取得
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル列</returns>
		internal RecommendModel[] GetAll(string shopId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_RECOMMEND_SHOP_ID, shopId}
			};
			var dv = Get(XML_KEY_NAME, "GetAll", ht);
			return dv.Cast<DataRowView>()
				.Select(drv => new RecommendModel(drv)).ToArray();
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(RecommendModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(RecommendModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>影響を受けた件数</returns>
		internal int Delete(string shopId, string recommendId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_RECOMMEND_SHOP_ID, shopId},
				{Constants.FIELD_RECOMMEND_RECOMMEND_ID, recommendId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region ~GetApplyConditionItemsAll レコメンド適用条件アイテム取得（全て）
		/// <summary>
		/// レコメンド適用条件アイテム取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>モデル列</returns>
		internal RecommendApplyConditionItemModel[] GetApplyConditionItemsAll(string shopId, string recommendId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_SHOP_ID, shopId},
				{Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_ID, recommendId}
			};
			var dv = Get(XML_KEY_NAME, "GetApplyConditionItemsAll", ht);
			return dv.Cast<DataRowView>()
				.Select(drv => new RecommendApplyConditionItemModel(drv))
				.OrderBy(i => i.RecommendApplyConditionItemSortNo).ToArray();

		}
		#endregion

		#region ~GetApplyConditionItemsAllContainer レコメンド適用条件アイテム取得（全て）（詳細表示用）
		/// <summary>
		/// レコメンド適用条件アイテム取得（全て）（詳細表示用）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>モデル列</returns>
		internal RecommendApplyConditionItemContainer[] GetApplyConditionItemsAllContainer(string shopId, string recommendId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_SHOP_ID, shopId},
				{Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_ID, recommendId}
			};
			var dv = Get(XML_KEY_NAME, "GetApplyConditionItemsAllContainer", ht);
			
			// 重複削除
			var result = new List<RecommendApplyConditionItemContainer>();
			var items = dv.Cast<DataRowView>()
				.Select(drv => new RecommendApplyConditionItemContainer(drv))
				.OrderBy(i => i.RecommendApplyConditionItemSortNo).ToArray();
			foreach (var item in items)
			{
				if (result.Any(i => ((i.RecommendApplyConditionItemSortNo == item.RecommendApplyConditionItemSortNo)
					&& (i.RecommendApplyConditionType == item.RecommendApplyConditionType))) == false)
				{
					result.Add(item);
				}
			}

			return result.ToArray();
		}
		#endregion

		#region ~InsertApplyConditionItem レコメンド適用条件アイテム登録
		/// <summary>
		/// レコメンド適用条件アイテム登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertApplyConditionItem(RecommendApplyConditionItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertApplyConditionItem", model.DataSource);
		}
		#endregion

		#region ~DeleteApplyConditionItemsAll レコメンド適用条件アイテム削除（全て）
		/// <summary>
		/// レコメンド適用条件アイテム削除（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteApplyConditionItemsAll(string shopId, string recommendId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_SHOP_ID, shopId},
				{Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_ID, recommendId}
			};
			var result = Exec(XML_KEY_NAME, "DeleteApplyConditionItemsAll", ht);
			return result;
		}
		#endregion

		#region ~GetUpsellTargetItem レコメンドアップセル対象アイテム取得 ※店舗ID＆レコメンドIDのみ指定
		/// <summary>
		/// レコメンドアップセル対象アイテム取得 ※店舗ID＆レコメンドIDのみ指定
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>モデル列</returns>
		internal RecommendUpsellTargetItemModel GetUpsellTargetItem(string shopId, string recommendId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_RECOMMENDUPSELLTARGETITEM_SHOP_ID, shopId},
				{Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_ID, recommendId}
			};
			var dv = Get(XML_KEY_NAME, "GetUpsellTargetItem", ht);
			if (dv.Count == 0) return null;
			return new RecommendUpsellTargetItemModel(dv[0]);
		}
		#endregion

		#region ~GetUpsellTargetItemContainer レコメンドアップセル対象アイテム取得 ※店舗ID＆レコメンドIDのみ指定（詳細表示用）
		/// <summary>
		/// レコメンドアップセル対象アイテム取得 ※店舗ID＆レコメンドIDのみ指定（詳細表示用）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>モデル列</returns>
		internal RecommendUpsellTargetItemContainer GetUpsellTargetItemContainer(string shopId, string recommendId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_RECOMMENDUPSELLTARGETITEM_SHOP_ID, shopId},
				{Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_ID, recommendId}
			};
			var dv = Get(XML_KEY_NAME, "GetUpsellTargetItemContainer", ht);
			if (dv.Count == 0) return null;
			return new RecommendUpsellTargetItemContainer(dv[0]);
		}
		#endregion

		#region ~InsertUpsellTargetItem レコメンドアップセル対象アイテム登録
		/// <summary>
		/// レコメンドアップセル対象アイテム登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertUpsellTargetItem(RecommendUpsellTargetItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertUpsellTargetItem", model.DataSource);
		}
		#endregion

		#region ~DeleteUpsellTargetItem レコメンドアップセル対象アイテム削除 ※店舗ID＆レコメンドIDのみ指定
		/// <summary>
		/// レコメンドアップセル対象アイテム削除 ※店舗ID＆レコメンドIDのみ指定
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteUpsellTargetItem(string shopId, string recommendId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_RECOMMENDUPSELLTARGETITEM_SHOP_ID, shopId},
				{Constants.FIELD_RECOMMENDUPSELLTARGETITEM_RECOMMEND_ID, recommendId}
			};
			var result = Exec(XML_KEY_NAME, "DeleteUpsellTargetItem", ht);
			return result;
		}
		#endregion

		#region ~GetItemsAll レコメンドアイテム取得（全て）
		/// <summary>
		/// レコメンドアイテム取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>モデル列</returns>
		internal RecommendItemModel[] GetItemsAll(string shopId, string recommendId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_RECOMMENDITEM_SHOP_ID, shopId},
				{Constants.FIELD_RECOMMENDITEM_RECOMMEND_ID, recommendId}
			};
			var dv = Get(XML_KEY_NAME, "GetItemsAll", ht);
			return dv.Cast<DataRowView>()
				.Select(drv => new RecommendItemModel(drv))
				.OrderBy(i => i.RecommendItemSortNo).ToArray();
		}
		#endregion

		#region ~GetItemsAllContainer レコメンドアイテム取得（全て）（詳細表示用）
		/// <summary>
		/// レコメンドアイテム取得（全て）（詳細表示用）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>モデル列</returns>
		internal RecommendItemContainer[] GetItemsAllContainer(string shopId, string recommendId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_RECOMMENDITEM_SHOP_ID, shopId},
				{Constants.FIELD_RECOMMENDITEM_RECOMMEND_ID, recommendId}
			};
			var dv = Get(XML_KEY_NAME, "GetItemsAllContainer", ht);
			return dv.Cast<DataRowView>()
				.Select(drv => new RecommendItemContainer(drv))
				.OrderBy(i => i.RecommendItemSortNo).ToArray();
		}
		#endregion

		#region ~InsertItem レコメンドアイテム登録
		/// <summary>
		/// レコメンドアイテム登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertItem(RecommendItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertItem", model.DataSource);
		}
		#endregion

		#region ~DeleteItemsAll レコメンドアイテム削除（全て）
		/// <summary>
		/// レコメンドアップセル対象アイテム削除（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteItemsAll(string shopId, string recommendId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_RECOMMENDITEM_SHOP_ID, shopId},
				{Constants.FIELD_RECOMMENDITEM_RECOMMEND_ID, recommendId}
			};
			var result = Exec(XML_KEY_NAME, "DeleteItemsAll", ht);
			return result;
		}
		#endregion

		#region ~GetRecommendHistory レコメンド表示履歴取得
		/// <summary>
		/// レコメンド表示履歴取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="recommendHistoryNo">履歴枝番</param>
		/// <returns>レコメンド表示履歴</returns>
		internal RecommendHistoryModel GetRecommendHistory(string shopId, string recommendId, string userId, int recommendHistoryNo)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_RECOMMENDHISTORY_SHOP_ID, shopId },
				{ Constants.FIELD_RECOMMENDHISTORY_RECOMMEND_ID, recommendId },
				{ Constants.FIELD_RECOMMENDHISTORY_USER_ID, userId },
				{ Constants.FIELD_RECOMMENDHISTORY_RECOMMEND_HISTORY_NO, recommendHistoryNo }
			};
			var dv = Get(XML_KEY_NAME, "GetRecommendHistory", ht);
			return (dv.Count != 0) ? new RecommendHistoryModel(dv[0]) : null;
		}
		#endregion

		#region ~GetRecommendHistoryByUserId ユーザーIDからレコメンド表示履歴を取得
		/// <summary>
		/// ユーザーIDからレコメンド表示履歴を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>レコメンド表示履歴</returns>
		internal RecommendHistoryModel[] GetRecommendHistoryByUserId(string shopId, string userId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_RECOMMENDHISTORY_SHOP_ID, shopId },
				{ Constants.FIELD_RECOMMENDHISTORY_USER_ID, userId }
			};
			var dv = Get(XML_KEY_NAME, "GetRecommendHistoryByUserId", ht);
			return dv.Cast<DataRowView>().Select(drv => new RecommendHistoryModel(drv)).ToArray();
		}
		#endregion

		#region ~InsertRecommendHistory レコメンド表示履歴登録
		/// <summary>
		/// レコメンド表示履歴登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertRecommendHistory(RecommendHistoryModel model)
		{
			Exec(XML_KEY_NAME, "InsertRecommendHistory", model.DataSource);
		}
		#endregion

		#region ~UpdateRecommendHistory レコメンド表示履歴更新
		/// <summary>
		/// レコメンド表示履歴更新
		/// </summary>
		/// <param name="model">モデル</param>
		internal void UpdateRecommendHistory(RecommendHistoryModel model)
		{
			Exec(XML_KEY_NAME, "UpdateRecommendHistory", model.DataSource);
		}
		#endregion

		#region ~GetRecommendReport レコメンドレポート検索表示
		/// <summary>
		/// レコメンドレポート検索表示
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal RecommendListSearchResult[] GetRecommendReport(RecommendListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetRecommendReport", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new RecommendListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~GetRecommendReportHitCount 件数取得（レコメンドレポート検索）
		/// <summary>
		/// 件数取得（レコメンドレポート検索）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int GetRecommendReportHitCount(RecommendListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetRecommendReportHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetRecommendReportPVNumberTotal 合計PV数取得（レコメンドレポート検索）
		/// <summary>
		/// 合計PV数取得（レコメンドレポート検索）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>合計PV数</returns>
		internal int GetRecommendReportPvNumberTotal(RecommendListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetPvNumberTotal", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetRecommendReportCVNumberTotal 合計CV数取得（レコメンドレポート検索）
		/// <summary>
		/// 合計CV数取得（レコメンドレポート検索）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>合計CV数</returns>
		internal int GetRecommendReportCvNumberTotal(RecommendListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetCvNumberTotal", condition.CreateHashtableParams());
			return (int)dv[0][0];
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
			var ht = new Hashtable
			{
				{ Constants.FIELD_RECOMMEND_REPORT_GRAPH_DATE_FROM, dateBgn },
				{ Constants.FIELD_RECOMMEND_REPORT_GRAPH_DATE_TO, dateEnd },
				{ Constants.FIELD_RECOMMENDHISTORY_SHOP_ID, shopId },
				{ Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_ID, recommendId },
			};
			var dv = Get(XML_KEY_NAME, "GetGraphPv", ht);
			return dv.Cast<DataRowView>().Select(drv => new RecommendReportGraphDateModel(drv)).ToArray();
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
			var ht = new Hashtable
			{
				{ Constants.FIELD_RECOMMEND_REPORT_GRAPH_DATE_FROM, dateBgn },
				{ Constants.FIELD_RECOMMEND_REPORT_GRAPH_DATE_TO, dateEnd },
				{ Constants.FIELD_RECOMMENDHISTORY_SHOP_ID, shopId },
				{ Constants.FIELD_RECOMMENDAPPLYCONDITIONITEM_RECOMMEND_ID, recommendId },
			};
			var dv = Get(XML_KEY_NAME, "GetGraphCv", ht);
			return dv.Cast<DataRowView>().Select(drv => new RecommendReportGraphDateModel(drv)).ToArray();
		}
		#endregion
	}
}