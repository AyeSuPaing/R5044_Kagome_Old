/*
=========================================================================================================
  Module      : ポイントサービスクラス (PointService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.Order;
using w2.Domain.Point.Helper;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.Point
{
	/// <summary>
	/// ポイントサービスクラス
	/// </summary>
	public class PointService : ServiceBase, IPointService
	{
		/// <summary>ポイントモデル（1度取得したらメモリにKEEP）</summary>
		private static volatile PointModel[] m_pointModel = null;
		/// <summary>LockObject</summary>
		private static readonly object m_lockObj = new object();

		#region +GetPointMaster w2ポイントマスタ取得
		/// <summary>
		/// w2ポイントマスタ取得
		/// </summary>
		/// <returns>
		/// メモリに貯めてるものを取得。メモリになければDBから取得
		/// </returns>
		public PointModel[] GetPointMaster()
		{
			// ダブルチェックロッキング入れてみる（1回目のIFは高速なはず・・・）
			if (m_pointModel == null)
			{
				lock (m_lockObj)
				{
					if (m_pointModel == null)
					{
						using (var repository = new PointRepository())
						{
							var dv = repository.GetPointMaster();
							m_pointModel = dv;
						}
					}
				}
			}
			return m_pointModel;
		}
		#endregion

		#region +GetUserPoint ユーザーのポイント情報を取得
		/// <summary>
		/// ユーザーのポイント情報を取得
		/// </summary>
		/// <param name="userId">取得条件となるユーザーID</param>
		/// <param name="cartId">カートID（後から増えたため、アクセサの後ろ）</param>
		/// <param name="accessor">
		/// トランザクションを内包するアクセサ
		/// 指定しない場合はメソッド内でトランザクションが完結
		/// </param>
		/// <returns>
		/// ユーザーのポイント情報。
		/// ポイントの情報が取れなければ0配列
		/// </returns>
		public UserPointModel[] GetUserPoint(string userId, string cartId, SqlAccessor accessor = null)
		{
			// アクセサの指定がある場合は使いまわす
			using (var repository = new PointRepository(accessor))
			{
				var userPoint = repository.GetUserPoint(userId, cartId);
				return userPoint;
			}
		}

		/// <summary>
		/// 注文に利用可能ポイントな合計ポイントを取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns></returns>
		public decimal GetUserPointUsableTotal(string userId, SqlAccessor accessor = null)
		{
			var pointUsable = GetUserPoint(userId, string.Empty, accessor).Where(x => x.IsUsableForOrder).Sum(x => x.Point);
			return pointUsable;
		}
		#endregion

		#region +GetUserPointByOrderId ユーザーポイント情報取得(本・仮ポイント)
		/// <summary>
		/// ユーザーポイント情報取得(本・仮ポイント)
		/// </summary>
		/// <param name="strUserId">ユーザID</param>
		/// <param name="strOrderId">注文ID</param>
		/// <returns>ユーザーポイント情報取得</returns>
		public UserPointModel[] GetUserPointByOrderId(string strUserId, string strOrderId)
		{
			var model = GetUserPoint(strUserId, string.Empty)
				.Where(x => ((x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP)
					|| ((x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP) && (x.Kbn1 == strOrderId))))
				.ToArray();

			return model;
		}
		#endregion

		#region +GetUserPointList ユーザーのポイント情報をヘルパクラスでラップして取得
		/// <summary>
		/// ユーザーのポイント情報をヘルパクラスでラップして取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ポイント情報</returns>
		public UserPointList GetUserPointList(string userId, SqlAccessor accessor = null)
		{
			var userPointList = new UserPointList(GetUserPoint(userId, string.Empty, accessor));
			return userPointList;
		}
		#endregion

		#region +RegisterUserPoint ユーザーポイント登録
		/// <summary>
		/// ユーザーポイント登録
		/// </summary>
		/// <param name="model">ユーザーポイントモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>成功したか</returns>
		public bool RegisterUserPoint(UserPointModel model, UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var result = RegisterUserPoint(model, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return result;
			}
		}
		#endregion
		#region +RegisterUserPoint ユーザーポイント登録
		/// <summary>
		/// ユーザーポイント登録
		/// </summary>
		/// <param name="model">ユーザーポイントモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>成功したか</returns>
		public bool RegisterUserPoint(UserPointModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			// 登録
			var result = RegisterUserPoint(model, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(model.UserId, model.LastChanged, accessor);
			}
			return result;
		}
		#endregion
		#region -RegisterUserPoint ユーザーポイント登録
		/// <summary>
		/// ユーザーポイント登録
		/// </summary>
		/// <param name="model">ユーザーポイントモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		private bool RegisterUserPoint(UserPointModel model, SqlAccessor accessor)
		{
			using (var repository = new PointRepository(accessor))
			{
				var updated = repository.RegisterUserPoint(model);
				return (updated > 0);
			}
		}
		#endregion

		#region +UpdateUserPoint ユーザーポイント更新
		/// <summary>
		/// ユーザーポイント更新
		/// </summary>
		/// <param name="model">更新するユーザーポイントモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateUserPoint(UserPointModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor = null)
		{
			// 更新
			var updated = UpdateUserPoint(model, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(model.UserId, model.LastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -UpdateUserPoint ユーザーポイント更新
		/// <summary>
		/// ユーザーポイント更新
		/// </summary>
		/// <param name="model">更新するユーザーポイントモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		private int UpdateUserPoint(UserPointModel model, SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				return repository.UpdateUserPoint(model);
			}
		}
		#endregion

		#region +DeleteUserPoint ユーザーポイント削除
		/// <summary>
		/// ユーザーポイント削除
		/// </summary>
		/// <param name="model">更新するユーザーポイントモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int DeleteUserPoint(UserPointModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor = null)
		{
			// 更新
			var deleted = DeleteUserPoint(model, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(model.UserId, model.LastChanged, accessor);
			}
			return deleted;
		}
		#endregion
		#region -DeleteUserPoint ユーザーポイント削除
		/// <summary>
		/// ユーザーポイント削除
		/// </summary>
		/// <param name="model">更新するユーザーポイントモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		private int DeleteUserPoint(UserPointModel model, SqlAccessor accessor)
		{
			using (var repository = new PointRepository(accessor))
			{
				return repository.DeleteUserPoint(model);
			}
		}
		#endregion

		#region +GetPointRule ポイントルールを取得
		/// <summary>
		/// ポイントルールを取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="pointRuleId">取得対象のポイントルールID</param>
		/// <returns>
		/// 取得できなかった場合はNull
		/// </returns>
		public PointRuleModel GetPointRule(string deptId, string pointRuleId)
		{
			using (var repository = new PointRepository())
			{
				var pointRule = repository.GetPointRule(deptId, pointRuleId);

				if (pointRule == null)
				{
					return null;
				}

				pointRule.RuleDate = repository.GetPointRuleDate(deptId, pointRuleId);
				return pointRule;
			}
		}
		#endregion

		#region +GetAllPointRule 全ポイントルール取得
		/// <summary>
		/// 全ポイントルール取得
		/// </summary>
		public PointRuleModel[] GetAllPointRules()
		{
			using (var repository = new PointRepository())
			{
				return repository.GetAllPointRules();
			}
		}
		#endregion

		#region +RegisterPointRule ポイントルール登録
		/// <summary>
		/// ポイントルール登録
		/// </summary>
		/// <param name="model">登録するModel</param>
		/// <returns>登録した件数</returns>
		public int RegisterPointRule(PointRuleModel model)
		{
			int rtn;

			using (var tranSaction = new TransactionScope())
			using (var repository = new PointRepository())
			{
				rtn = repository.RegisterPointRule(model);

				// ポイントルール日付の登録も同時に
				foreach (var ruleDate in model.RuleDate)
				{
					ruleDate.PointRuleId = model.PointRuleId;
					repository.RegisterPointRuleDate(ruleDate);
				}

				tranSaction.Complete();
			}
			return rtn;
		}
		#endregion

		#region +UpdatePointRule ポイントルール更新
		/// <summary>
		/// UpdatePointRule
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <returns>更新↓件数</returns>
		public int UpdatePointRule(PointRuleModel model)
		{
			int rtn;

			using (var tranSaction = new TransactionScope())
			using (var repository = new PointRepository())
			{
				rtn = repository.UpdatePointRule(model);

				// ポイントルール日付の削除・更新も同時に
				repository.DeletePointRuleDate(model.DeptId, model.PointRuleId);
				foreach (var ruleDate in model.RuleDate)
				{
					repository.RegisterPointRuleDate(ruleDate);
				}

				tranSaction.Complete();
			}

			return rtn;
		}
		#endregion

		#region +DeletePointRule ポイントルール削除
		/// <summary>
		/// UpdatePointRule
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <returns>削除した件数</returns>
		public int DeletePointRule(PointRuleModel model)
		{
			int rtn;

			using (var tranSaction = new TransactionScope())
			using (var repository = new PointRepository())
			{
				rtn = repository.DeletePointRule(model);
				// ポイントルール日付の削除も同時に
				repository.DeletePointRuleDate(model.DeptId, model.PointRuleId);

				tranSaction.Complete();
			}

			return rtn;
		}
		#endregion

		#region +PointRuleListSearchResult ポイントルールリスト検索
		/// <summary>
		/// ポイントルールリスト検索
		/// </summary>
		/// <param name="cond">ポイントルールリスト検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		public PointRuleListSearchResult[] PointRuleListSearch(PointRuleListSearchCondition cond)
		{
			var search = new PointRuleListSearch();
			return search.Search(cond);
		}
		#endregion

		#region +PointTransitionReportSearch ポイント推移レポート検索
		/// <summary>
		/// ポイント推移レポート検索
		/// </summary>
		/// <param name="cond">ポイント推移レポート検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		public PointTransitionReportResult[] PointTransitionReportSearch(PointTransitionReportCondition cond)
		{
			var report = new PointTransitionReport();
			return report.Search(cond);
		}
		#endregion

		#region +GetSearchHitCountUserPointHistorySummary ユーザーポイント履歴（概要）検索数取得
		/// <summary>
		/// ユーザーポイント履歴（概要）検索数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索ヒット数</returns>
		public int GetSearchHitCountUserPointHistorySummary(UserPointHistorySummarySearchCondition condition)
		{
			var result = new UserPointHistorySearch().GetSearchHitCountUserPointHistorySummary(condition);
			return result;
		}
		#endregion

		#region SearchUserPointHistorySummary ユーザーポイント履歴（概要）検索
		/// <summary>
		/// ユーザーポイント履歴（概要）検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果</returns>
		public UserPointHistorySummarySearchResult[] SearchUserPointHistorySummary(UserPointHistorySummarySearchCondition condition)
		{
			var result = new UserPointHistorySearch().SearchSummary(condition);
			return result;
		}
		#endregion

		#region +UserPointHistorySearch ユーザーポイント履歴検索
		/// <summary>
		/// ユーザーポイント履歴検索
		/// </summary>
		/// <param name="cond">ユーザーポイント履歴検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		public UserPointHistorySearchResult[] UserPointHistorySearch(UserPointHistorySearchCondition cond)
		{
			var search = new UserPointHistorySearch();
			return search.Search(cond);
		}
		#endregion

		#region +UserPointListSearch ユーザーポイント検索
		/// <summary>
		/// ユーザーポイント検索
		/// </summary>
		/// <param name="cond">ユーザーポイント検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		public UserPointSearchResult[] UserPointListSearch(UserPointSearchCondition cond)
		{
			var search = new UserPointListSearch();
			return search.Search(cond);
		}
		#endregion

		#region +GetCountOfUserPointListSearch ユーザーポイント検索件数取得
		/// <summary>
		/// ユーザーポイント検索件数取得
		/// </summary>
		/// <param name="cond">ユーザーポイント検索条件クラス</param>
		/// <returns>検索件数</returns>
		public int GetCountOfUserPointListSearch(UserPointSearchCondition cond)
		{
			var search = new UserPointListSearch();
			return search.GetCountOfSearchUserPoint(cond);
		}
		#endregion

		#region +GetUserPointHistory ユーザーポイント履歴取得
		/// <summary>
		/// ユーザーポイント履歴取得
		/// </summary>
		/// <param name="userId">対象ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>取得した履歴</returns>
		public UserPointHistoryModel[] GetUserPointHistories(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				var history = repository.GetUserPointHistories(userId);
				return history;
			}
		}
		#endregion

		#region +GetUserHistoriesByPointIncKbn ユーザーポイント履歴取得(ポイント加算区分)
		/// <summary>
		/// ユーザーポイント履歴取得(ポイント加算区分)
		/// </summary>
		/// <param name="pointIncKbn">ポイント加算区分</param>
		/// <param name="userId">対象ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>取得した履歴</returns>
		public UserPointHistoryModel[] GetUserHistoriesByPointIncKbn(string pointIncKbn, string userId, SqlAccessor accessor)
		{
			using (var repository = new PointRepository(accessor))
			{
				var history = repository.GetUserHistoriesByPointIncKbn(pointIncKbn, userId);
				return history;
			}
		}
		#endregion

		#region +GetUserPointHistoriesNotRestoredByOrderId 戻し処理対象のユーザーポイント履歴を取得
		/// <summary>
		/// 注文IDに紐づく、まだ戻し処理が行われていない（消費されている状態）の
		/// ユーザーポイント履歴を取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>ユーザーポイント履歴</returns>
		/// <remarks>V5.13以前のユーザーポイント履歴は含まれない</remarks>
		public UserPointHistoryModel[] GetUserPointHistoriesNotRestoredByOrderId(string userId, string orderId, SqlAccessor accessor = null)
		{
			// IsRestored == falseとするとV5.13以前の履歴まで巻き込むのでダメ
			var history = GetUserPointHistories(userId, accessor)
				.Where(x => (x.RestoredFlg == Constants.FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_NOT_RESTORED)
							&& (x.PointType == Constants.FLG_USERPOINTHISTORY_POINT_TYPE_COMP)
							&& (x.Kbn1 == orderId))
				.ToArray();
			return history;
		}
		#endregion

		#region +DeleteUserPointHistoryByOrderId 指定注文IDのユーザーポイント履歴削除
		/// <summary>
		/// 指定注文IDのユーザーポイント履歴削除
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="kbn1">区分値1（注文ID）</param>
		/// <param name="pointKbn">ポイント区分</param>
		/// <param name="lastChanged">最終更新者（履歴用）</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除件数</returns>
		public int DeleteUserPointHistoryByOrderId(
			string userId,
			string kbn1,
			string pointKbn,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 削除
			var updated = DeleteUserPointHistoryByOrderId(userId, kbn1, pointKbn, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -DeleteUserPointHistoryByOrderId 指定注文IDのユーザーポイント履歴削除
		/// <summary>
		/// 指定注文IDのユーザーポイント削除
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="kbn1">区分値1（注文ID）</param>
		/// <param name="pointKbn">ポイント区分</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除件数</returns>
		private int DeleteUserPointHistoryByOrderId(string userId, string kbn1, string pointKbn, SqlAccessor accessor)
		{
			using (var repository = new PointRepository(accessor))
			{
				var updated = repository.DeleteUserPointHistoryByOrderID(userId, kbn1, pointKbn);
				return updated;
			}
		}
		#endregion

		#region +RegisterHistory ユーザーポイント履歴登録
		/// <summary>
		/// ユーザーポイント履歴登録
		/// </summary>
		/// <param name="model">ユーザーポイント履歴モデル</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>登録件数</returns>
		public int RegisterHistory(UserPointHistoryModel model, SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				// グループ番号が設定されていなければここで採番
				if (model.IsHistoryGroupNoAlreadySet == false)
				{
					model.HistoryGroupNo = IssueHistoryGroupNoForUser(model.UserId, accessor);
				}

				return repository.RegisterHistory(model);
			}
		}
		#endregion

		#region +GetTargetUserTempPointToReal 本ポイント移行対象の仮ポイントを取得
		/// <summary>
		/// 本ポイント移行対象の仮ポイントを取得
		/// </summary>
		/// <param name="daysFromShippingForBasePoint">出荷後何日で本ポイントへ移行するか(通常ポイント)</param>
		/// <param name="daysFromShippingForLimitedTermPoint">出荷後何日で本ポイントへ移行するか(期間限定ポイント)</param>
		/// <returns></returns>
		public UserPointModel[] GetTargetUserTempPointToReal(int daysFromShippingForBasePoint, int daysFromShippingForLimitedTermPoint)
		{
			using (var repository = new PointRepository())
			{
				var userTempPoint = repository.GetTargetUserTempPointToReal(daysFromShippingForBasePoint, daysFromShippingForLimitedTermPoint);
				return userTempPoint;
			}
		}
		#endregion

		#region +GetTargetUserTempPointToReal 有効期限切れポイント取得
		/// <summary>
		/// 有効期限切れポイント取得
		/// </summary>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ</param>
		/// <returns></returns>
		public UserPointModel[] GetExpiredUserPoints(SqlAccessor sqlAccessor)
		{
			using (var repository = new PointRepository(sqlAccessor))
			{
				var userTempPoint = repository.GetExpiredUserPoints();
				return userTempPoint;
			}
		}
		#endregion

		#region +DeleteExpiredPoint 有効期限が切れたポイント削除
		/// <summary>
		/// 有効期限が切れたポイント削除
		/// </summary>
		/// <param name="model">ユーザーポイント履歴モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="isRegistHistory">履歴を登録するか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>削除件数</returns>
		public int DeleteExpiredPoint(
			UserPointModel model,
			string lastChanged,
			bool isRegistHistory,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				// ユーザーポイント削除
				var result = repository.DeleteUserPoint(model);

				if (isRegistHistory == false) return result;

				// ポイント削除履歴
				// 履歴をとる（本ポイントの加算）
				// 本ポイント加算用の履歴情報を作る
				RegisterHistory(
					new UserPointHistoryModel
					{
						UserId = model.UserId,
						DeptId = model.DeptId,
						PointRuleId = string.Empty,
						PointRuleKbn = string.Empty,
						PointKbn = model.PointKbn,
						PointType = model.PointType,
						// 有効期限切れ削除
						PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_POINT_EXPIRED,
						// 削除するので反転
						PointInc = model.Point * -1,
						UserPointExp = model.PointExp,
						PointExpExtend = string.Empty,
						Kbn1 = model.Kbn1,
						Kbn2 = string.Empty,
						Kbn3 = string.Empty,
						Kbn4 = string.Empty,
						Kbn5 = string.Empty,
						Memo = "有効期限切れ：" + DateTime.Now.ToString("yyyy/MM/dd"),
						LastChanged = lastChanged
					},
					accessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(model.UserId, lastChanged, accessor);
				}
				return result;
			}
		}
		#endregion

		#region +GetUpdLockForUserPoint ユーザポイント更新ロック取得(ポイントバッチ利用)
		/// <summary>
		/// ユーザポイント更新ロック取得(ポイントバッチ利用)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ</param>
		public void GetUpdLockForUserPoint(string userId, SqlAccessor sqlAccessor)
		{
			// トランザクションを使いまわすして更新ロックかける
			using (var repository = new PointRepository(sqlAccessor))
			{
				repository.GetUpdLockForUserPoint(userId);
			}
		}
		#endregion

		#region +GetUpdLockForUserPointHistory ユーザポイント履歴更新ロック取得(ポイントバッチ利用)
		/// <summary>
		/// ユーザポイント履歴更新ロック取得(ポイントバッチ利用)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ</param>
		public void GetUpdLockForUserPointHistory(string userId, SqlAccessor sqlAccessor)
		{
			// トランザクションを使いまわすして更新ロックかける
			using (var repository = new PointRepository(sqlAccessor))
			{
				repository.GetUpdLockForUserPointHistory(userId);
			}
		}
		#endregion

		#region +GetHightPriorityCampaignRule 優先度の高いポイントキャンペーン取得
		/// <summary>
		/// 優先度の高いポイントキャンペーン取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="currentDateTime">対象とする現在日</param>
		/// <returns>優先度の高いポイントキャンペーン</returns>
		public PointRuleModel[] GetHightPriorityCampaignRule(string deptId, DateTime currentDateTime)
		{
			using (var repository = new PointRepository())
			{
				var pointRule = repository.GetHightPriorityCampaignRule(deptId, currentDateTime);
				return pointRule;
			}
		}
		#endregion

		#region +GetBasicRule 基本ルール取得
		/// <summary>
		/// 基本ルール取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="currentDateTime">対象とする現在日</param>
		/// <returns>基本ルール</returns>
		public PointRuleModel[] GetBasicRule(string deptId, DateTime currentDateTime)
		{
			using (var repository = new PointRepository())
			{
				var pointRule = repository.GetBasicRule(deptId, currentDateTime);
				return pointRule;
			}
		}
		#endregion

		#region +PointRuleScheduleListSearchResult ポイントルールスケジュールリスト検索
		/// <summary>
		/// ポイントルールスケジュールリスト検索
		/// </summary>
		/// <param name="cond">ポイントルールスケジュールリスト検索条件クラス</param>
		/// <returns>
		/// 検索結果
		/// 検索結果が0件の場合は0配列
		/// </returns>
		public PointRuleScheduleListSearchResult[] PointRuleScheduleListSearch(PointRuleScheduleListSearchCondition cond)
		{
			var search = new PointRuleScheduleListSearch();
			return search.Search(cond);
		}
		#endregion

		#region +GetPointRuleSchedule ポイントルールスケジュール取得
		/// <summary>
		/// ポイントルールスケジュール取得
		/// </summary>
		/// <param name="pointRuleScheduleId">ポイントルールスケジュールID</param>
		/// <returns>モデル</returns>
		public PointRuleScheduleModel GetPointRuleSchedule(string pointRuleScheduleId)
		{
			using (var repository = new PointRepository())
			{
				var model = repository.GetPointRuleSchedule(pointRuleScheduleId);
				return model;
			}
		}
		#endregion

		#region +GetPointRuleScheduleByPointRuleId
		/// <summary>
		/// Get Point Rule Schedule By Point Rule Id
		/// </summary>
		/// <param name="pointRuleId">Point Rule Id</param>
		/// <returns>Point Rule Schedules</returns>
		public PointRuleScheduleModel[] GetPointRuleScheduleByPointRuleId(string pointRuleId)
		{
			using (var repository = new PointRepository())
			{
				var model = repository.GetPointRuleScheduleByPointRuleId(pointRuleId);

				return model;
			}
		}
		#endregion

		#region +InsertPointRuleSchedule ポイントルールスケジュール登録
		/// <summary>
		/// ポイントルールスケジュール登録
		/// </summary>
		/// <param name="model">登録するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>登録した件数</returns>
		public int InsertPointRuleSchedule(PointRuleScheduleModel model, SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				var result = repository.InsertPointRuleSchedule(model);

				return result;
			}
		}
		#endregion

		#region +UpdatePointRuleSchedule ポイントルールスケジュール更新
		/// <summary>
		/// ポイントルールスケジュール更新
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>更新した件数</returns>
		public int UpdatePointRuleSchedule(PointRuleScheduleModel model, SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				var result = repository.UpdatePointRuleSchedule(model);

				return result;
			}
		}
		#endregion

		#region +DeletePointRuleSchedule ポイントルールスケジュール削除
		/// <summary>
		/// ポイントルールスケジュール削除
		/// </summary>
		/// <param name="pointRuleScheduleId">ポイントルールスケジュールID</param>
		/// <returns>削除した件数</returns>
		public int DeletePointRuleSchedule(string pointRuleScheduleId)
		{
			using (var repository = new PointRepository())
			{
				var result = repository.DeletePointRuleSchedule(pointRuleScheduleId);

				return result;
			}
		}
		#endregion

		#region +UpdatePointRuleScheduleStatus ポイントルールスケジュールステータス更新
		/// <summary>
		/// ポイントルールスケジュールステータス更新
		/// </summary>
		/// <param name="model">更新するModel</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>更新した件数</returns>
		public int UpdatePointRuleScheduleStatus(PointRuleScheduleModel model, SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				var result = repository.UpdatePointRuleScheduleStatus(model);

				return result;
			}
		}
		#endregion

		// ************************↓↓ ポイント操作系 ↓↓ ************************

		#region +PointOperation オペレータによるポイント調整を実施
		/// <summary>
		/// オペレータによるポイント調整を実施
		/// </summary>
		/// <param name="pointOperationContents">ポイント操作内容</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>ポイント操作件数</returns>
		public int PointOperation(
			PointOperationContents pointOperationContents,
			UpdateHistoryAction updateHistoryAction)
		{
			var operation = new PointOperation();
			return operation.Execute(pointOperationContents, this, updateHistoryAction);
		}
		#endregion

		#region 仮ポイントから本ポイントへ
		/// <summary>
		/// 仮ポイントから本ポイントへ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		public int TempToRealPoint(
			string userId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = TempToRealPoint(userId, orderId, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		/// <summary>
		/// 仮ポイントから本ポイントへ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int TempToRealPoint(
			string userId,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = 0;
			var tempPoints = GetUserPoint(userId, string.Empty, accessor)
				.Where(x => (x.OrderId == orderId) && (x.IsPointTypeTemp))
				.ToArray();
			foreach (var tempPoint in tempPoints)
			{
				updated += TempToRealPoint(tempPoint, lastChanged, updateHistoryAction, accessor);
			}

			return updated;
		}
		#endregion
		#region +TempToRealPoint 仮ポイントから本ポイントへ
		/// <summary>
		/// 仮ポイントから本ポイントへ
		/// </summary>
		/// <param name="tempPoint">仮ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>
		/// 更新件数
		/// 0件の場合は楽観ロックでNG
		/// </returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		public int TempToRealPoint(UserPointModel tempPoint, string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = TempToRealPoint(tempPoint, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +TempToRealPoint 仮ポイントから本ポイントへ
		/// <summary>
		/// 仮ポイントから本ポイントへ
		/// </summary>
		/// <param name="tempPoint">仮ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>
		/// 更新件数
		/// 0件の場合は楽観ロックでNG
		/// </returns>
		public int TempToRealPoint(
			UserPointModel tempPoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 戻り値
			var rtn = 0;

			var updPoint = new UserPointModel
			{
				UserId = tempPoint.UserId,
				PointKbn = tempPoint.PointKbn,
				DeptId = tempPoint.DeptId,
				PointRuleId = tempPoint.PointRuleId,
				Point = tempPoint.Point,
				OrderId = tempPoint.OrderId,
				LastChanged = lastChanged,
				DateChanged = DateTime.Now,
				EffectiveDate = null,
				PointExp = tempPoint.PointExp
			};

			if (tempPoint.IsBasePoint)
			{
				// 通常本ポイントで不要な項目は空に
				updPoint.PointIncKbn = string.Empty;
				updPoint.PointRuleKbn = string.Empty;
				updPoint.PointRuleId = string.Empty;
				updPoint.OrderId = string.Empty;

				// 期限計算（長い方を使う）
				if (updPoint.PointExp != null)
				{
					updPoint.PointExp = (updPoint.PointExp < tempPoint.PointExp)
						? tempPoint.PointExp
						: updPoint.PointExp;
				}
			}
			else
			{
				// 発行時のポイントルール取得
				var rule = GetPointRule(Constants.CONST_DEFAULT_DEPT_ID, tempPoint.PointRuleId);

				updPoint.EffectiveDate = rule.CalculateEffectiveDatetime(true);
				updPoint.PointExp = rule.CalculateExpiryDatetimeForLimitedTermPoint(true);
			}

			// 更新
			RegisterOrAddUserPoint(updPoint, accessor);
			rtn++;

			// 履歴をとる（仮ポイントの減算）
			// 仮ポイント減算用の履歴情報を作る
			RegisterHistory(
				new UserPointHistoryModel
				{
					UserId = tempPoint.UserId,
					DeptId = tempPoint.DeptId,
					PointRuleId = tempPoint.PointRuleId,
					PointRuleKbn = tempPoint.PointRuleKbn,
					PointKbn = tempPoint.PointKbn,
					PointType = Constants.FLG_USERPOINTHISTORY_POINT_TYPE_TEMP,
					PointIncKbn = tempPoint.PointIncKbn,
					PointInc = tempPoint.Point * -1,
					UserPointExp = tempPoint.PointExp,
					Kbn1 = tempPoint.Kbn1,
					Kbn2 = string.Empty,
					Kbn3 = string.Empty,
					Kbn4 = string.Empty,
					Kbn5 = string.Empty,
					Memo = "本ポイント移行",
					LastChanged = lastChanged
				},
				accessor);

			// 仮ポイント削除
			DeleteUserPoint(tempPoint, accessor);

			// 履歴をとる（本ポイントの加算）
			// 本ポイント加算用の履歴情報を作る
			RegisterHistory(
				new UserPointHistoryModel
				{
					UserId = tempPoint.UserId,
					DeptId = tempPoint.DeptId,
					PointRuleId = tempPoint.PointRuleId,
					PointRuleKbn = tempPoint.PointRuleKbn,
					PointKbn = tempPoint.PointKbn,
					PointType = Constants.FLG_USERPOINTHISTORY_POINT_TYPE_COMP,
					PointIncKbn = tempPoint.PointIncKbn,
					PointInc = tempPoint.Point,
					UserPointExp = tempPoint.PointExp,
					Kbn1 = tempPoint.Kbn1,
					Kbn2 = string.Empty,
					Kbn3 = string.Empty,
					Kbn4 = string.Empty,
					Kbn5 = string.Empty,
					Memo = "本ポイント移行",
					LastChanged = lastChanged
				},
				accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(tempPoint.UserId, lastChanged, accessor);
			}

			return rtn;
		}
		#endregion

		#region -UsePointForBuy 購入時のポイント利用
		/// <summary>
		/// 購入時のポイント利用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="usePoint">利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="cartId">カートID</param>
		/// <returns>
		/// 更新件数
		/// 0件の場合は楽観ロックでNG
		/// </returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		public int UsePointForBuy(
			string deptId,
			string userId,
			string orderId,
			decimal usePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string cartId)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UsePointForBuy(
					deptId,
					userId,
					orderId,
					usePoint,
					lastChanged,
					updateHistoryAction,
					cartId,
					accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +UsePointForBuy 購入時のポイント利用
		/// <summary>
		/// 購入時のポイント利用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="usePoint">利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="cartId">カートID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>
		/// 更新件数
		/// 0件の場合は楽観ロックでNG
		/// </returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		public int UsePointForBuy(
			string deptId,
			string userId,
			string orderId,
			decimal usePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string cartId,
			SqlAccessor accessor = null)
		{
			// 更新
			var updated = UsePointForBuy(deptId, userId, orderId, usePoint, lastChanged, cartId, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}

			return updated;
		}
		#endregion
		#region -UsePointForBuy 購入時のポイント利用
		/// <summary>
		/// 購入時のポイント利用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="usePoint">利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="cartId">カートID</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ</param>
		/// <returns>
		/// 更新件数
		/// 0件の場合は楽観ロックでNG
		/// </returns>
		private int UsePointForBuy(
			string deptId,
			string userId,
			string orderId,
			decimal usePoint,
			string lastChanged,
			string cartId,
			SqlAccessor sqlAccessor)
		{
			var updated = ExpendUserPoint(
				deptId,
				userId,
				orderId,
				usePoint,
				lastChanged,
				cartId,
				sqlAccessor,
				pointIncKbn: Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_USE_POINT);
			return updated;
		}
		#endregion

		#region +CancelUsedPointForBuy 購入時の利用ポイントの戻し
		/// <summary>
		/// 利用ポイントの戻し
		/// </summary>
		/// <param name="userId">対象ユーザーID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="usedPoint">戻す利用したポイント数</param>
		/// <param name="orderId">ポイント戻しの原因となったキャンセル注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="shouldRestoreExpiredPoint">期限切れのポイントを戻すか</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>True:更新成功または更新対象なし False:楽観ロックNG</returns>
		/// <remarks>
		/// アクセサの指定がない場合はメソッド内でトランザクション完結
		/// </remarks>
		/// <returns>成功したか</returns>
		public bool CancelUsedPointForBuy(
			string userId,
			string deptId,
			decimal usedPoint,
			string orderId,
			string lastChanged,
			bool shouldRestoreExpiredPoint,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			bool isSuccess;
			RestoreExpendedUserPointByOrderId(
				userId,
				orderId,
				lastChanged,
				shouldRestoreExpiredPoint,
				restoreToBasePoint: false,
				accessor: accessor,
				isSuccess: out isSuccess,
				pointIncKbn: Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_CANCEL_USED_POINT);

			// 更新履歴登録
			if (isSuccess && updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}

			return isSuccess;
		}
		#endregion

		#region +UpsertUserPoint ユーザーポイントUpSert
		/// <summary>
		/// ユーザーポイントUpSert
		/// </summary>
		/// <param name="model">Updsert内容</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>UpSert件数</returns>
		public int UpsertUserPoint(UserPointModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			// 登録更新
			var updated = UpsertUserPoint(model, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(model.UserId, model.LastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region +UpsertUserPoint ユーザーポイントUpSert
		/// <summary>
		/// ユーザーポイントUpSert
		/// </summary>
		/// <param name="model">Updsert内容</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>UpSert件数</returns>
		private int UpsertUserPoint(UserPointModel model, SqlAccessor accessor)
		{
			using (var repository = new PointRepository(accessor))
			{
				var updated = repository.UpsertUserPoint(model);

				return updated;
			}
		}
		#endregion

		#region +CancelAddedPointForBuy 購入時の付与ポイントの戻し
		/// <summary>
		/// 付与ポイントの戻し
		/// </summary>
		/// <param name="userId">対象ユーザーID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="addedPoint">戻す付与したポイント数（本ポイント又は仮ポイントから減算するポイント数）</param>
		/// <param name="orderId">ポイント戻しの原因となったキャンセル注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>True:更新成功または更新対象なし False:楽観ロックNG</returns>
		/// <remarks>
		/// 購入時ポイントがすでに本ポイントに移行している場合は本ポイントより減算
		/// まだ仮ポイント状態の場合は仮ポイントを削除する
		/// アクセサの指定がない場合はメソッド内でトランザクション完結
		/// </remarks>
		public bool CancelAddedPointForBuy(
			string userId,
			string deptId,
			decimal addedPoint,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = RevokeGrantedUserPointByOrderId(userId, orderId, addedPoint, lastChanged, accessor, "注文キャンセル");
			if (updated <= 0)
			{
				return false;
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}

			return true;
		}
		#endregion

		#region +RecalcOrderUsePoint 利用ポイントを再計算
		/// <summary>
		/// 利用ポイントを再計算
		/// </summary>
		/// <returns>更新件数 ０は楽観ロックによる更新失敗</returns>
		public int RecalcOrderUsePoint(
			string userId,
			string oldOrderId,
			string newOrderId,
			decimal newUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = RecalcOrderUsePoint(
				userId,
				oldOrderId,
				newOrderId,
				newUsePoint,
				lastChanged,
				accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}

			return updated;
		}
		#endregion
		#region -RecalcOrderUsePoint 利用ポイントを再計算
		/// <summary>
		/// 利用ポイント再計算
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="oldOrderId">復元のキーになる注文ID</param>
		/// <param name="newOrderId">消費処理で使用する注文ID</param>
		/// <param name="newUsePoint">利用ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響件数</returns>
		public int RecalcOrderUsePoint(
			string userId,
			string oldOrderId,
			string newOrderId,
			decimal newUsePoint,
			string lastChanged,
			SqlAccessor accessor)
		{
			var isSuccess = false;
			var userPointForUse = RestoreExpendedUserPointByOrderId(
				userId,
				oldOrderId,
				lastChanged,
				true, // 期限切れのポイントを復元する
				false, // 通常本ポイントに復元はしない。
				accessor,
				out isSuccess);

			var updated = userPointForUse.Length;
			updated += ExpendUserPoint(
				Constants.CONST_DEFAULT_DEPT_ID,
				userId,
				newOrderId,
				newUsePoint,
				lastChanged,
				string.Empty,
				accessor,
				Constants.CONST_USERPOINTHISTORY_DEFAULT_GROUP_NO,
				Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_USE_POINT,
				userPointForUse);

			return updated;
		}
		#endregion

		#region +RecalcOrderUsePointForReturnExchange 利用ポイントを再計算(返品交換用)
		/// <summary>
		/// 利用ポイントを再計算(返品交換用)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="oldOrderId">復元のキーになる注文ID</param>
		/// <param name="newOrderId">消費処理で使用する注文ID</param>
		/// <param name="newUsePoint">利用ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響件数</returns>
		/// <remarks>消費されたポイントを全て通常本ポイントとして加算する。</remarks>
		public int RecalcOrderUsePointForReturnExchange(
			string userId,
			string oldOrderId,
			string newOrderId,
			decimal newUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = RecalcOrderUsePointForReturnExchange(
				userId,
				oldOrderId,
				newOrderId,
				newUsePoint,
				lastChanged,
				accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}

			return updated;
		}
		/// <summary>
		/// 利用ポイント再計算(返品交換用)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="oldOrderId">復元のキーになる注文ID</param>
		/// <param name="newOrderId">消費処理で使用する注文ID</param>
		/// <param name="newUsePoint">利用ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響件数</returns>
		/// <remarks>消費されたポイントを全て通常本ポイントとして加算する。</remarks>
		public int RecalcOrderUsePointForReturnExchange(
			string userId,
			string oldOrderId,
			string newOrderId,
			decimal newUsePoint,
			string lastChanged,
			SqlAccessor accessor)
		{
			var isSuccess = false;
			var userPointForUse = RestoreExpendedUserPointByOrderId(
				userId,
				oldOrderId,
				lastChanged,
				true, // 期限切れのポイントを復元する
				true, // 通常本ポイントに復元する
				accessor,
				out isSuccess);

			var updated = ExpendUserPoint(
				Constants.CONST_DEFAULT_DEPT_ID,
				userId,
				newOrderId,
				newUsePoint,
				lastChanged,
				string.Empty,
				accessor,
				pointIncKbn: Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURNEXCHANGE_ADJUST_ORDER_POINT_USE,
				userPointForUse: userPointForUse);

			return updated;
		}
		#endregion

		#region +RollbackUserPointForBuyFailure 購入失敗時のポイントロールバック
		/// <summary>
		/// 購入失敗時のポイントロールバック
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">失敗した注文ID</param>
		/// <param name="orderPointUse">ロールバックする利用ポイント数</param>
		/// <param name="orderPointAdd">ロールバックする付与ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>履歴削除件数</returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		public int RollbackUserPointForBuyFailure(
			string userId,
			string orderId,
			decimal orderPointUse,
			decimal orderPointAdd,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = RollbackUserPointForBuyFailure(
					userId,
					orderId,
					orderPointUse,
					orderPointAdd,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +RollbackUserPointForBuyFailure 購入失敗時のポイントロールバック
		/// <summary>
		/// 購入失敗時のポイントロールバック
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">失敗した注文ID</param>
		/// <param name="orderPointUse">ロールバックする利用ポイント数</param>
		/// <param name="orderPointAdd">ロールバックする付与ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor"></param>
		/// <returns>履歴削除件数</returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		public int RollbackUserPointForBuyFailure(
			string userId,
			string orderId,
			decimal orderPointUse,
			decimal orderPointAdd,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 付与ポイント取消
			RevokeGrantedUserPointByOrderId(userId, orderId, orderPointAdd, lastChanged, accessor);
			// 利用ポイント復元
			var isSuccess = false;
			RestoreExpendedUserPointByOrderId(
				userId,
				orderId,
				lastChanged,
				false,
				false,
				accessor,
				out isSuccess,
				Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_CANCEL_USED_POINT);

			// 関連履歴削除
			var cnt = DeleteUserPointHistoryByOrderId(userId, orderId, Constants.FLG_USERPOINT_POINT_KBN_BASE, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}

			return cnt;
		}
		#endregion

		#region +IssuePointByRule ポイントルールを基にしてポイント発行
		/// <summary>
		/// ポイントルールを基にしてポイント発行
		/// </summary>
		/// <param name="rule">ポイントルール</param>
		/// <param name="userId">発行するユーザーID</param>
		/// <param name="orderId">発行元となった注文ID</param>
		/// <param name="issuePoint">発行するポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="productId">発行元になった商品ID</param>
		/// <returns>登録件数</returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		public int IssuePointByRule(
			PointRuleModel rule,
			string userId,
			string orderId,
			decimal issuePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			string productId = "")
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 付与
				var updated = IssuePointByRule(
					rule,
					userId,
					orderId,
					issuePoint,
					lastChanged,
					updateHistoryAction,
					accessor,
					productId);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
				}

				accessor.CommitTransaction();
				return updated;
			}
		}
		/// <summary>
		/// ポイントルールを基にしてポイント発行
		/// </summary>
		/// <param name="rule">ポイントルール</param>
		/// <param name="userId">発行するユーザーID</param>
		/// <param name="orderId">発行元となった注文ID</param>
		/// <param name="issuePoint">発行するポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="productId">発行元になった商品ID</param>
		/// <returns>登録件数</returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		public int IssuePointByRule(
			PointRuleModel rule,
			string userId,
			string orderId,
			decimal issuePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			string productId = "")
		{
			// ポイント発行
			var updated = IssuePointByRule(rule, userId, orderId, issuePoint, lastChanged, accessor, productId);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(userId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -IssuePointByRule ポイントルールを基にしてポイント発行
		/// <summary>
		/// ポイントルールを基にしてポイント発行
		/// </summary>
		/// <param name="rule">ポイントルール</param>
		/// <param name="userId">発行するユーザーID</param>
		/// <param name="orderId">発行元となった注文ID</param>
		/// <param name="productId">発行元となった商品ID</param>
		/// <param name="issuePoint">発行するポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ</param>
		/// <returns>登録件数</returns>
		private int IssuePointByRule(
			PointRuleModel rule,
			string userId,
			string orderId,
			decimal issuePoint,
			string lastChanged,
			SqlAccessor sqlAccessor,
			string productId)
		{
			var rtn = 0;

			// ポイントマスタ取得
			var master = GetPointMaster().FirstOrDefault(i => (i.PointKbn == rule.PointKbn));
			if (master == null)
			{
				throw new Exception("ポイントマスタの取得に失敗したためポイント発行は行えませんでした。");
			}

			// ポイントルールがログイン時発行の場合、既に当日分を発行している場合は発行させない
			if (rule.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_LOGIN)
			{
				// 本日ログインのポイント履歴を取得
				var todayLoginPointHistory = GetUserPointHistories(userId)
					.Where(i => (i.PointIncKbn == Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_LOGIN)
							&& (i.PointRuleKbn == rule.PointRuleKbn)
							&& (DateTime.Now.Date <= i.DateCreated)
							&& (i.DateCreated < DateTime.Now.Date.AddDays(1)))
					.ToArray();

				if (todayLoginPointHistory.Any())
				{
					return 0;
				}
			}

			// 有効期限を計算しておく
			var limitedTermPointExp = rule.CalculateExpiryDatetimeForLimitedTermPoint();
			var basePointExp = master.IsValidPointExpKbn
				? rule.CalculateExpExtendDatetime(true)
				: null;

			// ルールをもとにポイントモデルを作成
			var updPoint = new UserPointModel
			{
				UserId = userId,
				PointKbn = rule.PointKbn,
				DeptId = rule.DeptId,
				PointRuleId = rule.PointRuleId,
				PointRuleKbn = rule.PointRuleKbn,
				PointType = rule.NeedsToUseTempPoint
					? Constants.FLG_USERPOINT_POINT_TYPE_TEMP
					: Constants.FLG_USERPOINT_POINT_TYPE_COMP,
				PointIncKbn = rule.PointIncKbn,
				Point = issuePoint,
				PointExp = rule.IsBasePoint
					? basePointExp
					: limitedTermPointExp,
				OrderId = (rule.IsLimitedTermPoint || rule.NeedsToUseTempPoint)
					? orderId
					: string.Empty,
				LastChanged = lastChanged,
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				EffectiveDate = rule.IsLimitedTermPoint
					? rule.CalculateEffectiveDatetime()
					: null
			};

			var baseCompPoint = GetUserPoint(userId, string.Empty, sqlAccessor)
				.FirstOrDefault(p => (p.IsBasePoint && p.IsPointTypeComp));

			if (updPoint.IsBasePoint && updPoint.IsPointTypeComp)
			{
				// 通常本ポイントで不要な項目は空に
				updPoint.PointIncKbn = string.Empty;
				updPoint.PointRuleKbn = string.Empty;
				updPoint.PointRuleId = string.Empty;
				updPoint.OrderId = string.Empty;

				if (baseCompPoint != null)
				{
					updPoint.PointExp = (baseCompPoint.PointExp < updPoint.PointExp)
						? updPoint.PointExp
						: baseCompPoint.PointExp;
				}
			}

			// 更新
			RegisterOrAddUserPoint(updPoint, sqlAccessor);
			rtn++;

			// 期間限定ポイントでも本ポイントの期限延長をする
			if (rule.IsLimitedTermPoint && basePointExp.HasValue && (baseCompPoint != null))
			{
				if ((baseCompPoint.PointExp < basePointExp))
				{
					baseCompPoint.PointExp = basePointExp;
					var updated = UpsertUserPoint(baseCompPoint, sqlAccessor);
					if (updated <= 0)
					{
						throw new Exception("本ポイントの期限延長処理に失敗。");
					}
				}
			}

			// 履歴をとる
			RegisterHistory(
				new UserPointHistoryModel
				{
					UserId = userId,
					DeptId = updPoint.DeptId,
					PointRuleId = rule.PointRuleId,
					PointRuleKbn = rule.PointRuleKbn,
					PointKbn = rule.PointKbn,
					PointType = (rule.UseTempFlg == Constants.FLG_POINTRULE_USE_TEMP_FLG_VALID)
						? Constants.FLG_USERPOINT_POINT_TYPE_TEMP
						: Constants.FLG_USERPOINT_POINT_TYPE_COMP,
					PointIncKbn = rule.PointIncKbn,
					PointInc = issuePoint,
					UserPointExp = updPoint.PointExp,
					PointExpExtend = rule.PointExpExtend,
					OrderId = orderId,
					ProductId = productId,
					LastChanged = lastChanged,
					EffectiveDate = updPoint.EffectiveDate,
				},
				sqlAccessor);

			return rtn;
		}
		#endregion

		#region +AdjustPointExchangeOrder 交換注文時のポイント調整（仮ポイント用）
		/// <summary>
		/// 返品交換時の付与ポイント調整（仮ポイント用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="pointKbnNo">ポイント区分No</param>
		/// <param name="orderId">交換先の注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="adjustPoint">調整ポイント数</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>
		/// 更新件数
		/// 0件の場合は楽観ロックでNG
		/// </returns>
		/// <remarks>アクセサの指定がない場合はメソッド内でトランザクション完結</remarks>
		public int AdjustOrderPointAddForPointTemp(
			string deptId,
			string userId,
			int pointKbnNo,
			string orderId,
			decimal adjustPoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = AdjustOrderPointAddForPointTemp(
				deptId,
				userId,
				pointKbnNo,
				orderId,
				adjustPoint,
				lastChanged,
				accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -AdjustOrderPointAddForPointTemp 返品交換時の付与ポイント調整（仮ポイント用）
		/// <summary>
		/// 返品交換時の付与ポイント調整（仮ポイント用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="pointKbnNo">ポイント区分No</param>
		/// <param name="orderId">返品元の注文ID</param>
		/// <param name="adjustPoint">調整ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しない場合はメソッド内でトランザクション完結)</param>
		/// <returns>更新件数</returns>
		private int AdjustOrderPointAddForPointTemp(
			string deptId,
			string userId,
			int pointKbnNo,
			string orderId,
			decimal adjustPoint,
			string lastChanged,
			SqlAccessor sqlAccessor)
		{
			var rtn = 0;
			// トランザクションを使いまわす
			using (var repository = new PointRepository(sqlAccessor))
			{
				// ユーザーポイント
				var userPoiont = repository.GetUserPoint(userId, string.Empty);

				// 仮ポイント
				var tempPoint = userPoiont
					.FirstOrDefault(p => (p.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP)
										&& (p.PointKbnNo == pointKbnNo)
										&& (p.OrderId == orderId));

				if (tempPoint == null)
				{
					return rtn;
				}

				// 注文の仮ポイントを更新
				// ポイント調整数加算
				tempPoint.Point += adjustPoint;
				tempPoint.LastChanged = lastChanged;

				rtn = repository.UpdateUserPoint(tempPoint);

				// 更新件数が0件の場合は楽観ロックNG
				if (rtn == 0)
				{
					return rtn;
				}

				// 履歴をとる
				// 本ポイント戻し用の履歴情報を作る
				var restoreHistory = new UserPointHistoryModel
				{
					UserId = userId,
					DeptId = deptId,
					PointRuleId = string.Empty,
					PointRuleKbn = string.Empty,
					PointKbn = tempPoint.PointKbn,
					PointType = Constants.FLG_USERPOINTHISTORY_POINT_TYPE_TEMP,
					PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURNEXCHANGE_ADJUST_ORDER_POINT_ADD,
					PointInc = adjustPoint,
					// 有効期限はマックスを使う（元SQLからこうなっていたが・・・）
					UserPointExp = (tempPoint.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE) ? userPoiont.Max(x => x.PointExp) : null,
					PointExpExtend = (tempPoint.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE)
						? UserPointHistoryModel.DEFAULT_POINT_EXP_EXTEND_STRING 
						: UserPointHistoryModel.GetPointExpExtendFormtString(1, 0, 0),
					Kbn1 = orderId,
					Kbn2 = string.Empty,
					Kbn3 = string.Empty,
					Kbn4 = string.Empty,
					Kbn5 = string.Empty,
					Memo = string.Empty,
					LastChanged = lastChanged
				};
				RegisterHistory(restoreHistory, sqlAccessor);

				// 注文の付与ポイントの補正
				// DB上の注文IDに紐付いてるポイントの合計で補正
				// Hack：リファクタリングが進めば、注文サービスに返品メソッドができ、そちらに処理一式が寄せられと思う
				var sumtempPoint = repository.GetUserPoint(userId, string.Empty)
					.Where(x => (x.OrderId == orderId))
					.Sum(x => x.Point);
				var sv = new OrderService();
				rtn = sv.AdjustAddPoint(orderId, sumtempPoint, lastChanged, UpdateHistoryAction.DoNotInsert, sqlAccessor);
			}
			return rtn;
		}
		#endregion

		#region -AdjustOrderPointAddForPointComp 返品交換時の付与ポイント調整（本ポイント用）
		/// <summary>
		/// 返品交換時の通常 付与ポイント調整（本ポイント用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">返品元の注文ID</param>
		/// <param name="adjustBasePoint">通常 調整ポイント数</param>
		/// <param name="errorMessageFormat">エラー時 メッセージフォーマット</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しない場合はメソッド内でトランザクション完結)</param>
		/// <param name="baseUpdateCount">out 更新レコード数</param>
		/// <returns>エラー時 エラーメッセージ内容</returns>
		public string AdjustOrderBasePointAddForPointComp(
			string deptId,
			string userId,
			string orderId,
			decimal adjustBasePoint,
			string errorMessageFormat,
			string lastChanged,
			SqlAccessor sqlAccessor,
			out int baseUpdateCount)
		{
			// トランザクションを使いまわす
			using (var repository = new PointRepository(sqlAccessor))
			{
				// ユーザーポイント
				var pointModels = repository.GetUserPoint(userId, string.Empty);
				baseUpdateCount = 0;
				// 期間限定ポイントでマイナスしきれなかった分は通常ポイントよりマイナス
				var abp = adjustBasePoint;
				if (abp == 0) return string.Empty;

				var basePointModels =
					pointModels
						.Where(p => p.IsBasePoint)
						.ToArray();

				var compBasePoint = basePointModels
					.FirstOrDefault(p => p.IsPointTypeComp && p.IsBasePoint);
				if (compBasePoint != null)
				{
					// 更新
					var updateBasePoint = compBasePoint.Point + abp;

					// 所持している 通常本ポイントがマイナスとなる場合はエラー
					if (updateBasePoint < 0) return string.Format(errorMessageFormat, compBasePoint.Point, abp, updateBasePoint);

					compBasePoint.Point = updateBasePoint;
					compBasePoint.LastChanged = lastChanged;
					baseUpdateCount += repository.UpdateUserPoint(compBasePoint);
				}
				else
				{
					var master = GetPointMaster()
						.FirstOrDefault(i => (i.DeptId == deptId) && i.IsPointKbnBase);
					if (master == null)
					{
						throw new Exception("ポイントマスタの取得に失敗したためポイント発行は行えませんでした。");
					}

					// 所持している 通常本ポイントがマイナスとなる場合はエラー
					if (abp < 0) return string.Format(errorMessageFormat, 0, abp, abp);

					// 追加
					compBasePoint = new UserPointModel
					{
						UserId = userId,
						PointKbn = Constants.FLG_USERPOINT_POINT_KBN_BASE,
						PointKbnNo = IssuePointKbnNoForUser(userId, sqlAccessor),
						DeptId = deptId,
						PointRuleId = string.Empty,
						PointRuleKbn = string.Empty,
						PointType = Constants.FLG_USERPOINTHISTORY_POINT_TYPE_COMP,
						PointIncKbn =
							Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURNEXCHANGE_ADJUST_ORDER_POINT_ADD,
						Point = abp,
						// 本ポイントがない場合は一年後
						PointExp = (master.PointExpKbn == Constants.FLG_POINT_POINT_EXP_KBN_VALID)
							? (DateTime?)new DateTime(
								DateTime.Now.Year,
								DateTime.Now.Month,
								DateTime.Now.Day,
								23,
								59,
								59,
								997).AddYears(1)
							: null,
						Kbn1 = orderId,
						Kbn2 = string.Empty,
						Kbn3 = string.Empty,
						Kbn4 = string.Empty,
						Kbn5 = string.Empty,
						LastChanged = lastChanged
					};
					baseUpdateCount += repository.RegisterUserPoint(compBasePoint);
				}

				// 更新件数が0件の場合は楽観ロックNG
				if (baseUpdateCount == 0) return string.Empty;
				// 履歴をとる
				// 本ポイント更新用履歴情報を作る
				var restoreHistory = new UserPointHistoryModel
				{
					UserId = userId,
					DeptId = deptId,
					PointRuleId = string.Empty,
					PointRuleKbn = string.Empty,
					PointKbn = Constants.FLG_USERPOINT_POINT_KBN_BASE,
					PointType = Constants.FLG_USERPOINTHISTORY_POINT_TYPE_COMP,
					PointIncKbn =
						Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURNEXCHANGE_ADJUST_ORDER_POINT_ADD,
					PointInc = abp,
					// 有効期限はマックスを使う（元SQLからこうなっていたが・・・）
					UserPointExp = basePointModels.Max(x => x.PointExp),
					PointExpExtend = UserPointHistoryModel.GetPointExpExtendFormtString(1, 0, 0),
					Kbn1 = orderId,
					Kbn2 = string.Empty,
					Kbn3 = string.Empty,
					Kbn4 = string.Empty,
					Kbn5 = string.Empty,
					Memo = string.Empty,
					LastChanged = lastChanged
				};
				RegisterHistory(restoreHistory, sqlAccessor);
			}

			return string.Empty;
		}
		/// <summary>
		/// 返品交換時の期間限定 付与ポイント調整（本ポイント用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">返品元の注文ID</param>
		/// <param name="adjustLimitPoint">期間限定 調整ポイント数</param>
		/// <param name="errorMessageFormat">エラー時 メッセージフォーマット</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しない場合はメソッド内でトランザクション完結)</param>
		/// <param name="limitUpdateCount">out 更新レコード数</param>
		/// <returns>エラー時 エラーメッセージ</returns>
		public string AdjustOrderLimitPointAddForPointComp(
			string deptId,
			string userId,
			string orderId,
			decimal adjustLimitPoint,
			string errorMessageFormat,
			string lastChanged,
			SqlAccessor sqlAccessor,
			out int limitUpdateCount)
		{
			// トランザクションを使いまわす
			using (var repository = new PointRepository(sqlAccessor))
			{
				// ユーザーポイント
				var pointModels = repository.GetUserPoint(userId, string.Empty);

				limitUpdateCount = 0;
				var alp = adjustLimitPoint;
				if (alp == 0) return string.Empty;

				var limitPointModels =
					pointModels
						.Where(p => p.IsLimitedTermPoint)
						.ToArray();

				var compLimitPointList = limitPointModels
					.Where(p => p.IsPointTypeComp && p.IsLimitedTermPoint)
					.ToArray();
				if (compLimitPointList.Any() == false) return string.Format(errorMessageFormat, 0, alp, alp);

				var updateLimitPoint = 0m;
				var updateAlp = alp;
				foreach (var compLimitPoint in compLimitPointList)
				{
					// 更新
					updateLimitPoint = compLimitPoint.Point + updateAlp;
					compLimitPoint.LastChanged = lastChanged;
					if (updateLimitPoint > 0)
					{
						compLimitPoint.Point = updateLimitPoint;
						limitUpdateCount += repository.UpdateUserPoint(compLimitPoint);
					}
					else
					{
						limitUpdateCount += repository.DeleteUserPoint(compLimitPoint);
					}

					if (updateLimitPoint >= 0) break;

					updateAlp = updateLimitPoint;
				}

				// 所持している 期間限定ポイントがマイナスとなる場合はエラー
				if (updateLimitPoint < 0)
				{
					return string.Format(errorMessageFormat, compLimitPointList.Sum(p => p.Point), alp, updateLimitPoint);
				}
				// 更新件数が0件の場合は楽観ロックNG
				if (limitUpdateCount == 0) return string.Empty;

				// 履歴をとる
				// 本ポイント更新用履歴情報を作る
				var restoreHistory = new UserPointHistoryModel
				{
					UserId = userId,
					DeptId = deptId,
					PointRuleId = string.Empty,
					PointRuleKbn = string.Empty,
					PointKbn = Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT,
					PointType = Constants.FLG_USERPOINTHISTORY_POINT_TYPE_COMP,
					PointIncKbn =
						Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURNEXCHANGE_ADJUST_ORDER_POINT_ADD,
					PointInc = alp,
					UserPointExp = null,
					PointExpExtend = UserPointHistoryModel.GetPointExpExtendFormtString(0, 0, 0),
					Kbn1 = orderId,
					Kbn2 = string.Empty,
					Kbn3 = string.Empty,
					Kbn4 = string.Empty,
					Kbn5 = string.Empty,
					Memo = string.Empty,
					LastChanged = lastChanged
				};
				RegisterHistory(restoreHistory, sqlAccessor);
			}
			return string.Empty;
		}
		#endregion

		#region -RestoreExpendedUserPointByOrderId 注文に使われたポイントを履歴から復元
		/// <summary>
		/// 注文に使われたポイントを履歴から復元
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="shouldRestoreExpiredPoint">期限切れのポイントを戻すか</param>
		/// <param name="restoreToBasePoint">通常本ポイントに加算するか</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="pointIncKbn">ポイント加算区分</param>
		/// <param name="isSuccess">成功したか</param>
		/// <returns>復元された本ポイント</returns>
		private UserPointModel[] RestoreExpendedUserPointByOrderId(
			string userId,
			string orderId,
			string lastChanged,
			bool shouldRestoreExpiredPoint,
			bool restoreToBasePoint,
			SqlAccessor accessor,
			out bool isSuccess,
			string pointIncKbn = null)
		{
			isSuccess = false;
			var userPointUpdated = new List<UserPointModel>();
			var actionExecutedDatetime = DateTime.Now;

			// 履歴グループ番号採番
			var groupNo = IssueHistoryGroupNoForUser(userId, accessor);

			bool isBeforeMigration;
			var histories = PointOperationHelper.GetUserPointHistoriesForRestore(
				userId,
				orderId,
				lastChanged,
				out isBeforeMigration,
				accessor);

			foreach (var pointHistory in histories)
			{
				var shouldCompensateNegativePoint = GetUserPoint(userId, string.Empty, accessor)
					.Any(p => p.IsBasePoint && p.IsPointTypeComp && (p.Point < 0));

				// 履歴からユーザーポイント作成
				var userPoint = PointOperationHelper.CreateUserPointFromHistory(pointHistory, lastChanged, restoreToBasePoint);

				var newHistory = UserPointHistoryModel.DeepClone(pointHistory);
				newHistory.LastChanged = lastChanged;
				newHistory.RestoredFlg = Constants.FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_RESTORED;
				newHistory.HistoryGroupNo = groupNo;
				newHistory.DateCreated = actionExecutedDatetime;

				// 期限内のポイント(期限未設定ふくむ)は戻す
				if (((userPoint.PointExp == null) && (userPoint.IsLimitedTermPoint))
					|| (actionExecutedDatetime <= userPoint.PointExp)
					|| shouldRestoreExpiredPoint)
				{
					// マイナスになっている通常ポイントを補填
					if (shouldCompensateNegativePoint)
					{
						// 通常本ポイント取得
						userPoint = GetUserPoint(userId, string.Empty, accessor)
							.First(x => (x.IsBasePoint && x.IsPointTypeComp));

						userPoint.Point += (pointHistory.PointInc * -1);
						if (userPoint.Point > 0)
						{
							pointHistory.PointInc = (userPoint.Point * -1);
							UpsertUserPoint(userPoint, accessor);
							userPoint.Point = 0;
						}
						else
						{
							pointHistory.PointInc = 0;
							UpsertUserPoint(userPoint, accessor);
						}
					}

					if ((pointHistory.PointInc * -1) > 0)
					{
						userPoint = RegisterOrAddUserPoint(userPoint, accessor, true);
					}

					// 同一のポイント情報は消しておく
					userPointUpdated.RemoveAll(x => (userPoint.UserId == x.UserId) && (userPoint.PointKbnNo == x.PointKbnNo));
					// 返却用のリストに追加
					userPointUpdated.Add(userPoint);

					newHistory.PointIncKbn = pointIncKbn ?? Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_RECALCULATION_RESTORE;
					newHistory.PointInc *= -1;
				}
				// 期限切れの場合は期限切れ消滅の履歴だけ残す
				else
				{
					newHistory.Memo = string.Format("有効期限切れ：{0:yyyy/MM/dd}", actionExecutedDatetime);
					newHistory.PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_POINT_EXPIRED;
				}

				// 新しく履歴を登録する前に、復元に使った履歴を復元済としてマークする
				if (isBeforeMigration == false) MarkRestoredFlg(pointHistory, accessor);

				// ポイント復元が行われている場合、履歴登録する
				if (userPointUpdated.Count > 0) RegisterHistory(newHistory, accessor);

				isSuccess = true;
			}

			return userPointUpdated.ToArray();
		}
		#endregion

		#region -RevokeGrantedUserPointByOrderId 注文で付与されたポイントを取り消す
		/// <summary>
		/// 注文で付与されたポイントを取り消す
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="addedPoint">付与したポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <param name="memo">メモ（履歴に記載）</param>
		/// <returns>影響件数</returns>
		/// <remarks>
		/// 仮ポイントと期間限定ポイントを物理削除する
		/// 取消合計が付与ポイントを満たさない場合は通常本ポイントから減算を行う
		/// </remarks>
		public int RevokeGrantedUserPointByOrderId(
			string userId,
			string orderId,
			decimal addedPoint,
			string lastChanged,
			SqlAccessor accessor,
			string memo = "")
		{
			var updated = 0;
			var actionExecutedDatetime = DateTime.Now;

			using (var repository = new PointRepository(accessor))
			{
				var userPoints = repository.GetUserPoint(userId, string.Empty);

				var userPointWithOrders = userPoints.Where(x => (x.OrderId == orderId));
				var revokedBasePoint = 0m;
				var revokedLimitedTermPoint = 0m;

				// 注文に紐付いている期間限定ポイントと仮ポイントをすべて削除
				foreach (var userPointWithOrder in userPointWithOrders)
				{
					if ((revokedBasePoint + revokedLimitedTermPoint) == addedPoint) break;

					updated += repository.DeleteUserPoint(userPointWithOrder);

					// 処理済みポイントを加算(仮ポイントは通常ポイントとして加算)
					if (userPointWithOrder.IsBasePoint)
					{
						revokedBasePoint += userPointWithOrder.Point;
					}
					else if (userPointWithOrder.IsLimitedTermPoint)
					{
						revokedLimitedTermPoint += userPointWithOrder.Point;
					}

					// 履歴をとる
					RegisterHistory(
						new UserPointHistoryModel
						{
							UserId = userId,
							DeptId = Constants.CONST_DEFAULT_DEPT_ID,
							PointRuleId = string.Empty,
							PointRuleKbn = string.Empty,
							PointKbn = userPointWithOrder.PointKbn,
							PointType = userPointWithOrder.PointType,
							PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_CANCEL_ADDED_POINT,
							PointInc = userPointWithOrder.Point * -1,
							UserPointExp = userPointWithOrder.PointExp,
							PointExpExtend = UserPointHistoryModel.DEFAULT_POINT_EXP_EXTEND_STRING,
							Kbn1 = orderId,
							Kbn2 = string.Empty,
							Kbn3 = string.Empty,
							Kbn4 = string.Empty,
							Kbn5 = string.Empty,
							Memo = memo,
							DateCreated = actionExecutedDatetime,
							LastChanged = lastChanged
						},
						accessor);
				}

				// 付与したポイントを取り消し切れた場合
				if ((revokedBasePoint + revokedLimitedTermPoint) == addedPoint)
				{
					return updated;
				}

				var userPointHistory = GetUserPointHistoryByOrderId(userId, orderId, accessor);

				// 本ポイントは本ポイントから戻す
				var baseUserPointHistory = userPointHistory
					.Where(x => x.IsBasePoint
						&& x.IsPointTypeComp
						&& (x.IsAddedPoint
							|| x.PointIncKbn == Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_KBN_ORDER))
					.ToArray();
				if (baseUserPointHistory.Any())
				{
					// 付与された本ポイント数を取得
					var addedBasePoint = baseUserPointHistory.Sum(x => x.PointInc);

					// ユーザー保持本ポイント取得
					var basePoint = userPoints.FirstOrDefault(x => (x.IsBasePoint && x.IsPointTypeComp));

					basePoint = basePoint ?? new UserPointModel
					{
						UserId = userId,
						PointExp = DateTime.Now.AddYears(1),
						DateChanged = DateTime.Now,
					};
					basePoint.LastChanged = lastChanged;

					var revokePoint = addedBasePoint - revokedBasePoint;
					basePoint.Point = basePoint.Point - revokePoint;
					updated += repository.UpsertUserPoint(basePoint);
					revokedBasePoint += revokePoint;

					// 履歴をとる
					RegisterHistory(
						new UserPointHistoryModel
						{
							UserId = userId,
							DeptId = "0",
							PointKbn = basePoint.PointKbn,
							PointType = Constants.FLG_USERPOINTHISTORY_POINT_TYPE_COMP,
							PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_CANCEL_ADDED_POINT,
							PointInc = (revokePoint * -1),
							UserPointExp = basePoint.PointExp,
							PointExpExtend = UserPointHistoryModel.DEFAULT_POINT_EXP_EXTEND_STRING,
							OrderId = orderId,
							Memo = memo,
							LastChanged = lastChanged,
							EffectiveDate = basePoint.EffectiveDate,
						},
						accessor);
				}

				// 期間限定ポイントは期間限定ポイントから戻す
				var limitedTermPointHistory = userPointHistory
					.Where(x => x.IsLimitedTermPoint && x.IsAddedPoint)
					.ToArray();
				if (limitedTermPointHistory.Any())
				{
					// 付与された期間限定ポイント数を取得
					var addedLimitedTermPoint = limitedTermPointHistory.Sum(x => x.PointInc);

					// ユーザー期間限定ポイント数を取得
					var limitedTermPoints = userPoints
						.Where(x => x.IsLimitedTermPoint && (x.OrderId != orderId))
						.OrderBy(x => x.PointExp)
						.ToArray();

					foreach (var limitedTermPoint in limitedTermPoints)
					{
						var revokePoint = addedLimitedTermPoint - revokedLimitedTermPoint;
						var updateLimitTermPoint = limitedTermPoint.Point - revokePoint;
						limitedTermPoint.LastChanged = lastChanged;
						if (updateLimitTermPoint > 0)
						{
							limitedTermPoint.Point = updateLimitTermPoint;
							updated += repository.UpdateUserPoint(limitedTermPoint);
							revokedLimitedTermPoint += revokePoint;
						}
						else
						{
							updated += repository.DeleteUserPoint(limitedTermPoint);
							revokedLimitedTermPoint += limitedTermPoint.Point;
						}

						// 履歴をとる
						RegisterHistory(
							new UserPointHistoryModel
							{
								UserId = userId,
								DeptId = "0",
								PointKbn = limitedTermPoint.PointKbn,
								PointType = Constants.FLG_USERPOINTHISTORY_POINT_TYPE_COMP,
								PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_CANCEL_ADDED_POINT,
								PointInc = (((updateLimitTermPoint > 0) ? revokePoint : limitedTermPoint.Point) * -1),
								UserPointExp = limitedTermPoint.PointExp,
								PointExpExtend = UserPointHistoryModel.DEFAULT_POINT_EXP_EXTEND_STRING,
								OrderId = orderId,
								Memo = limitedTermPoint.OrderId + "で付与されたポイントから減算",
								LastChanged = lastChanged,
								EffectiveDate = limitedTermPoint.EffectiveDate,
							},
							accessor);

						if (updateLimitTermPoint >= 0) break;
					}
				}
			}

			return updated;
		}
		#endregion

		#region -MarkRestoredFlg ユーザーポイント履歴を復元処理済でマークする
		/// <summary>
		/// ユーザーポイント履歴を復元処理済でマークする
		/// </summary>
		/// <param name="history">ユーザーポイント履歴</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>成功したか</returns>
		private bool MarkRestoredFlg(UserPointHistoryModel history, SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				var updated = repository.MarkRestoredFlg(history);
				return (updated > 0);
			}
		}
		#endregion

		#region -ExpendUserPoint ポイント消費の処理
		/// <summary>
		/// ポイント消費の処理
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="usePoint">利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ</param>
		/// <returns>
		/// 更新件数
		/// 0件の場合は楽観ロックでNG
		/// </returns>
		private int ExpendUserPoint(
			string userId,
			decimal usePoint,
			string lastChanged,
			SqlAccessor sqlAccessor)
		{
			var updated = ExpendUserPoint(
				Constants.CONST_DEFAULT_DEPT_ID,
				userId,
				string.Empty,
				usePoint,
				lastChanged,
				string.Empty,
				sqlAccessor);
			return updated;
		}
		#endregion
		#region -ExpendUserPoint ポイント消費の処理
		/// <summary>
		/// ポイント消費の処理
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="usePoint">利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="cartId">カートID</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ</param>
		/// <param name="historyGroupNo">履歴グループ番号</param>
		/// <param name="pointIncKbn">ポイント加算区分</param>
		/// <param name="userPointForUse">消費対象ポイント</param>
		/// <returns>更新件数</returns>
		private int ExpendUserPoint(
			string deptId,
			string userId,
			string orderId,
			decimal usePoint,
			string lastChanged,
			string cartId,
			SqlAccessor sqlAccessor,
			int historyGroupNo = Constants.CONST_USERPOINTHISTORY_DEFAULT_GROUP_NO,
			string pointIncKbn = null,
			UserPointModel[] userPointForUse = null
			)
		{
			// 戻り値
			var updatedCountTotal = 0;

			if (userPointForUse != null && userPointForUse.Length > 0)
			{
				userPointForUse = userPointForUse
					.OrderBy(p => p.PointExp)
					.ThenBy(p => p.PointKbnNo)
					.ToArray();
			}

			// 消費対象が渡されてなければDBから取得
			var userPointUsable = (userPointForUse != null && userPointForUse.Length > 0)
				? userPointForUse
				: GetUserPoint(userId, string.Empty, sqlAccessor)
					.Where(p => (p.IsUsableForOrder) && (p.Point > 0))
					.ToArray();

			// ポイント履歴グループ番号採番
			if (historyGroupNo == Constants.CONST_USERPOINTHISTORY_DEFAULT_GROUP_NO)
			{
				historyGroupNo = IssueHistoryGroupNoForUser(userId, sqlAccessor);
			}

			// 有効期限が近いものから減算していく
			var usePointTemp = usePoint;
			foreach (var userPointItem in userPointUsable.OrderBy(p => p.PointExp))
			{
				if (usePointTemp <= 0) break;

				// 配列の方が変更されないようにクローン作成
				var userPoint = UserPointModel.DeepClone(userPointItem);

				var used = (userPoint.Point < usePointTemp)
					? userPoint.Point
					: usePointTemp;

				usePointTemp -= used;
				userPoint.Point -= used;
				userPoint.LastChanged = lastChanged;

				// 0になったポイントは物理削除する
				var updated = ((userPoint.Point == 0) && userPoint.IsLimitedTermPoint)
					? DeleteUserPoint(userPoint, sqlAccessor)
					: UpdateUserPoint(userPoint, sqlAccessor);
				updatedCountTotal += updated;

				if (updated == 0)
				{
					throw new Exception("ポイント消費処理に失敗。");
				}

				// 履歴をとる
				RegisterHistory(
					new UserPointHistoryModel
					{
						UserId = userId,
						DeptId = deptId,
						PointRuleId = userPoint.PointRuleId,
						PointRuleKbn = userPoint.PointRuleKbn,
						PointKbn = userPoint.PointKbn,
						PointIncKbn = pointIncKbn ?? Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_USE_POINT,
						PointInc = (used * -1),
						EffectiveDate = userPoint.EffectiveDate,
						UserPointExp = userPoint.PointExp,
						Kbn1 = orderId,
						LastChanged = lastChanged,
						DateCreated = DateTime.Now,
						RestoredFlg = Constants.FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_NOT_RESTORED,
						HistoryGroupNo = historyGroupNo,
						CartId = cartId,
					},
					sqlAccessor);
			}

			// 渡されてるユーザーポイントが消費ポイントに対して足りなければ
			// 消費処理を再び呼び出す
			if ((userPointForUse != null) && (usePointTemp > 0))
			{
				var updated = ExpendUserPoint(
					deptId,
					userId,
					orderId,
					usePointTemp,
					lastChanged,
					string.Empty,
					sqlAccessor,
					historyGroupNo);
				updatedCountTotal += updated;

				if (updated > 0) usePointTemp = 0;
			}

			// ユーザーの保有ポイントが足りなかった場合は通常ポイントをマイナスにする。
			if (usePointTemp > 0)
			{
				SubtractFromBasePoint(userId, usePointTemp, lastChanged, orderId, sqlAccessor);
			}

			// 余った期限切れのポイントを削除
			if (userPointForUse != null && (userPointForUse.Sum(x => x.Point) > usePoint))
			{
				DeleteSurplusExpiredUserPoint(deptId, userId, orderId, lastChanged, sqlAccessor, "期限切れ削除");
			}

			return updatedCountTotal;
		}
		#endregion

		#region -DeleteSurplusExpiredUserPoint 余った期限切れポイント削除
		/// <summary>
		/// 余った期限切れポイント削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">アクセサ</param>
		/// <param name="memo">メモ</param>
		private void DeleteSurplusExpiredUserPoint(
			string deptId,
			string userId,
			string orderId,
			string lastChanged,
			SqlAccessor sqlAccessor,
			string memo)
		{
			var expiredPoints = GetUserPoint(userId, string.Empty, sqlAccessor)
				.Where(x => x.PointExp.HasValue && (x.PointExp <= DateTime.Now));
			foreach (var expiredPoint in expiredPoints)
			{
				// 削除を行い、期限切れとして履歴を取る。
				DeleteUserPoint(expiredPoint, sqlAccessor);
				RegisterHistory(
					new UserPointHistoryModel
					{
						UserId = userId,
						DeptId = deptId,
						PointRuleId = expiredPoint.PointRuleId,
						PointRuleKbn = expiredPoint.PointRuleKbn,
						PointKbn = expiredPoint.PointKbn,
						PointType = Constants.FLG_USERPOINTHISTORY_POINT_TYPE_COMP,
						PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_POINT_EXPIRED,
						PointInc = (expiredPoint.Point * -1),
						UserPointExp = expiredPoint.PointExp,
						PointExpExtend = UserPointHistoryModel.DEFAULT_POINT_EXP_EXTEND_STRING,
						Kbn1 = orderId,
						Memo = memo,
						LastChanged = lastChanged,
						DateCreated = DateTime.Now,
						EffectiveDate = expiredPoint.EffectiveDate,
						RestoredFlg = Constants.FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_RESTORED
					},
					sqlAccessor);
			}
		}
		#endregion

		#region +SubtractFromBasePoint 通常本ポイントから減算
		/// <summary>
		/// 通常本ポイントから減算
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="usePoint">減算するポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">アクセサ</param>
		/// <param name="orderId">注文ID</param>
		private void SubtractFromBasePoint(
			string userId,
			decimal usePoint,
			string lastChanged,
			string orderId,
			SqlAccessor sqlAccessor)
		{
			var userPoint = GetUserPoint(userId, string.Empty, sqlAccessor).FirstOrDefault(p => p.IsBasePoint && p.IsPointTypeComp);
			if (userPoint == null)
			{
				userPoint = new UserPointModel
				{
					UserId = userId,
					DeptId = Constants.CONST_DEFAULT_DEPT_ID,
					PointKbnNo = IssuePointKbnNoForUser(userId, sqlAccessor),
					PointRuleId = string.Empty,
					PointRuleKbn = string.Empty,
					PointKbn = Constants.FLG_USERPOINT_POINT_KBN_BASE,
					PointType = Constants.FLG_USERPOINT_POINT_TYPE_COMP,
					PointIncKbn = string.Empty,
					Point = usePoint * -1,
					PointExp = DateTime.Now.AddYears(1),
					LastChanged = lastChanged,
					DateChanged = DateTime.Now,
					EffectiveDate = null,
				};
			}
			else
			{
				userPoint.Point -= usePoint;
			}

			var rtn = UpsertUserPoint(userPoint, sqlAccessor);
			if (rtn > 0)
			{
				RegisterHistory(
					new UserPointHistoryModel
					{
						UserId = userId,
						DeptId = Constants.CONST_DEFAULT_DEPT_ID,
						PointRuleId = userPoint.PointRuleId,
						PointRuleKbn = userPoint.PointRuleKbn,
						PointKbn = userPoint.PointKbn,
						PointType = Constants.FLG_USERPOINTHISTORY_POINT_TYPE_COMP,
						PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_USE_POINT,
						PointInc = userPoint.Point,
						UserPointExp = userPoint.PointExp,
						PointExpExtend = UserPointHistoryModel.DEFAULT_POINT_EXP_EXTEND_STRING,
						Kbn1 = orderId,
						Memo = string.Empty,
						LastChanged = lastChanged,
						DateCreated = DateTime.Now,
						EffectiveDate = userPoint.EffectiveDate,
						RestoredFlg = Constants.FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_NOT_RESTORED
					},
					sqlAccessor);
			}
		}
		#endregion

		#region +ExecutePointIntegration ユーザー統合時にポイントを代表ユーザーに統合する
		/// <summary>
		/// ユーザー統合時にポイントを代表ユーザーに統合する
		/// </summary>
		/// <param name="contents">ポイント統合内容</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLパラメータ</param>
		/// <returns>ユーザーポイント履歴枝番</returns>
		/// <remarks>仮・本ポイントが移行されます</remarks>
		public int ExecutePointIntegration(
			PointIntegrationContents contents,
			UpdateHistoryAction updateHistoryAction,
			string lastChanged,
			SqlAccessor accessor)
		{
			// 統合
			var logNo = new PointIntegration().Execute(contents, updateHistoryAction, accessor);
			return logNo;
		}
		#endregion

		#region +CancelPointIntegration ユーザー統合時に代表ユーザーに統合したポイントを元に戻す
		/// <summary>
		/// ユーザー統合時に代表ユーザーに統合したポイントを元に戻す
		/// </summary>
		/// <param name="contents">ポイント統合キャンセル内容</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLパラメータ</param>
		/// <remarks>
		/// 仮・本ポイントが戻されます。
		/// ※仮ポイントを代表ユーザーに統合し、代表ユーザー側で本ポイントに移行されたポイントは戻りません。
		/// </remarks>
		public int CancelPointIntegration(
			PointIntegrationCancelContents contents,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = new PointIntegration().Cancel(contents, updateHistoryAction, accessor);
			return updated;
		}
		#endregion

		#region +GetUseablePointFromPrice 利用可能金額から利用可能ポイント数に変換
		/// <summary>
		/// 利用可能金額から利用可能ポイント数に変換
		/// </summary>
		/// <param name="useablePrice">最大利用可能金額</param>
		/// <param name="pointKbn">ポイント区分</param>
		/// <returns>利用可能ポイント数</returns>
		public decimal GetUseablePointFromPrice(decimal useablePrice, string pointKbn)
		{
			using (var respository = new PointRepository())
			{
				var pointMaster = respository.GetPointMaster().FirstOrDefault(x => x.IsPointKbnBase);
				return OrderPointUseHelper.GetUseablePointFromPrice(useablePrice, pointKbn, pointMaster);
			}
		}
		#endregion

		#region +GetOrderPointUsePrice 注文ポイント利用額取得
		/// <summary>
		/// 注文ポイント利用額取得
		/// </summary>
		/// <param name="orderPointUse">利用ポイント</param>
		/// <param name="pointKbn">ポイント区分</param>
		/// <returns>注文ポイント利用額</returns>
		public decimal GetOrderPointUsePrice(decimal orderPointUse, string pointKbn)
		{
			using (var respository = new PointRepository())
			{
				var pointMaster = respository.GetPointMaster().FirstOrDefault(x => x.IsPointKbnBase);
				return OrderPointUseHelper.GetOrderPointUsePrice(orderPointUse, pointMaster);
			}
		}
		#endregion

		#region +ApplyNextShippingUsePointToUserPoint ユーザポイントに次回購入の利用ポイントを適用
		/// <summary>
		/// ユーザポイントに次回購入の利用ポイントを適用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="oldUsePoint">（更新前）利用ポイント数</param>
		/// <param name="newUsePoint">（更新後）利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		public bool ApplyNextShippingUsePointToUserPoint(
			string deptId,
			string fixedPurchaseId,
			string userId,
			decimal oldUsePoint,
			decimal newUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 更新
			var result = ApplyNextShippingUsePointToUserPointExec(
				deptId,
				fixedPurchaseId,
				userId,
				oldUsePoint,
				newUsePoint,
				lastChanged,
				updateHistoryAction,
				accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}
			return result;
		}
		#endregion

		#region +ApplyNextShippingUsePointToUserPointExec ユーザポイントに次回購入の利用ポイントを適用
		/// <summary>
		/// ユーザポイントに次回購入の利用ポイントを適用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="oldUsePoint">（更新前）利用ポイント数</param>
		/// <param name="newUsePoint">（更新後）利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		private bool ApplyNextShippingUsePointToUserPointExec(string deptId, string fixedPurchaseId, string userId, decimal oldUsePoint, decimal newUsePoint, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			using (var repository = new PointRepository(accessor))
			{
				// ユーザーポイント（本）取得
				var pointList = repository.GetUserPoint(userId, string.Empty).Where(x => (x.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE)
					&& (x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP)).ToArray();
				DateTime? expiredDate = null;
				if (pointList.Any()) expiredDate = pointList.First().PointExp;

				// ポイント履歴を作成
				var history = new UserPointHistoryModel()
				{
					UserId = userId,
					DeptId = deptId,
					PointRuleKbn = string.Empty,
					PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_UPDATE_NEXT_SHIPPING_USE_POINT,
					PointInc = oldUsePoint - newUsePoint,
					UserPointExp = expiredDate,
					Kbn2 = fixedPurchaseId,
					LastChanged = lastChanged,
				};

				var result = false;
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					result = (RegisterHistory(history, accessor) > 0);
					if (result == false) return false;
				}

				// 有効期限切れ、または、ユーザポイントレコードなし
				if ((pointList.Any() == false) || (expiredDate < DateTime.Now))
				{
					if (updateHistoryAction == UpdateHistoryAction.Insert)
					{
						// 有効期限切れのポイント履歴を作成
						history.PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_POINT_EXPIRED;
						history.PointInc = -(oldUsePoint - newUsePoint);
						history.Kbn2 = string.Empty;
						result = (RegisterHistory(history, accessor) > 0);
						if (result == false) return false;
					}
				}
				else
				{
					// 次回購入の利用ポイントをユーザポイントに適用
					var point = pointList.First();
					point.Point += oldUsePoint - newUsePoint;
					point.LastChanged = lastChanged;
					result = (UpdateUserPoint(point, accessor) > 0);
					if (result == false) return false;
				}
				return true;
			}
		}
		#endregion

		#region +ReturnNextShippingUsePointToUserPoint 定期購入解約時、利用ポイント数をユーザポイントに戻す
		/// <summary>
		/// 定期購入解約時、利用ポイント数をユーザポイントに戻す
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="usePoint">利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>成功したか</returns>
		public bool ReturnNextShippingUsePointToUserPoint(
			string deptId,
			string userId,
			string fixedPurchaseId,
			decimal usePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 更新
			var result = ReturnNextShippingUsePointToUserPoint(deptId, userId, fixedPurchaseId, usePoint, lastChanged, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}
			return result;
		}
		#endregion
		#region +ReturnNextShippingUsePointToUserPoint 定期購入解約時、利用ポイント数をユーザポイントに戻す
		/// <summary>
		/// 定期購入解約時、利用ポイント数をユーザポイントに戻す
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="usePoint">利用ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>成功したか</returns>
		private bool ReturnNextShippingUsePointToUserPoint(string deptId, string userId, string fixedPurchaseId, decimal usePoint, string lastChanged, SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				// ユーザーポイントの取得
				var userPointList = repository.GetUserPoint(userId, string.Empty)
					.Where(x => (x.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE)
							&& (x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP))
					.ToArray();
				DateTime? expiredDate = null;
				if (userPointList.Any()) expiredDate = userPointList.First().PointExp;

				// 定期購入解約で次回購入の利用ポイントをユーザポイントに戻るポイント履歴の作成
				var history = new UserPointHistoryModel()
				{
					UserId = userId,
					DeptId = deptId,
					PointRuleKbn = string.Empty,
					PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_CANCEL_NEXT_SHIPPING_USE_POINT,
					PointInc = usePoint,
					UserPointExp = expiredDate,
					Kbn2 = fixedPurchaseId,
					LastChanged = lastChanged,
				};
				var result = (RegisterHistory(history, accessor) > 0);
				if (result == false) return false;

				// 有効期限切れ、または、ユーザポイントレコードなし
				if ((userPointList.Any() == false) || (expiredDate < DateTime.Now))
				{
					// ポイント消滅履歴の作成
					history.PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_POINT_EXPIRED;
					history.PointInc = -usePoint;
					history.UserPointExp = expiredDate;
					history.Kbn2 = string.Empty;
					result = (RegisterHistory(history, accessor) > 0);
				}
				else
				{
					// 有効な本ポイントレコードがあれば、ユーザポイントを更新
					var compPoint = userPointList.First();
					compPoint.Point += usePoint;
					result = (repository.UpdateUserPoint(compPoint) > 0);
				}
				return result;
			}
		}
		#endregion

		#region +ApplyNextShippingUsePointToOrder 設定された次回購入の利用ポイント履歴を生成した注文に適用
		/// <summary>
		/// 設定された次回購入の利用ポイント履歴を生成した注文に適用
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderUsePoint">注文の利用ポイント</param>
		/// <param name="nextShippingUsePoint">次回購入の利用ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>成功したか</returns>
		public bool ApplyNextShippingUsePointToOrder(
			string deptId,
			string userId,
			string fixedPurchaseId,
			string orderId,
			decimal orderUsePoint,
			decimal nextShippingUsePoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			using (var repository = new PointRepository(accessor))
			{
				// ユーザーポイントの取得
				var pointList = repository.GetUserPoint(userId, string.Empty)
						.Where(x => (x.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE)
								&& (x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP))
						.ToArray();

				// DateTime? expiredDate = null;
				// if (pointList.Any()) expiredDate = pointList.First().PointExp;
				var expiredDate = pointList.Any() ? pointList.First().PointExp : null;

				// 通常注文側のポイント履歴を作る
				// 注文に利用ポイントが0ptに適用された場合、注文のポイント利用履歴を作成しない
				if (orderUsePoint > 0)
				{
					var failProcessing = RegisterUsePointHistory(
						userId,
						deptId,
						lastChanged,
						orderUsePoint,
						expiredDate,
						orderId,
						fixedPurchaseId,
						accessor);
					if (failProcessing != string.Empty) throw new Exception(failProcessing + "に失敗しました。");
				}

				// 利用ポイント数が注文時の利用可能ポイント数より大きい場合、適用されなかった分のポイントはお客様のポイントに戻す
				if (nextShippingUsePoint > orderUsePoint)
				{
					var failProcessing = ReturnUnUsePoint(
						pointList.ToArray(),
						userId,
						deptId,
						orderId,
						fixedPurchaseId,
						lastChanged,
						nextShippingUsePoint,
						orderUsePoint,
						expiredDate,
						updateHistoryAction,
						accessor);
					if (failProcessing != string.Empty) throw new Exception(failProcessing + "に失敗しました。");
				}
				return true;
			}
		}
		#endregion

		#region -RegisterUsePointHistory ポイント利用履歴登録（定期と生成された注文）
		/// <summary>
		/// ポイント利用履歴登録（定期と生成された注文）
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="pointInc">ポイント加算数</param>
		/// <param name="userPointExp">ユーザ最新有効期限</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>失敗処理の名前（成功の場合、空に返却）</returns>
		private string RegisterUsePointHistory(
			string userId,
			string deptId,
			string lastChanged,
			decimal pointInc,
			DateTime? userPointExp,
			string orderId,
			string fixedPurchaseId,
			SqlAccessor accessor)
		{
			// 定期側のポイント利用履歴を作る
			var history = new UserPointHistoryModel()
			{
				UserId = userId,
				DeptId = deptId,
				PointRuleId = string.Empty,
				PointRuleKbn = string.Empty,
				PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_ADJUST_USE_POINT_FROM_FIXED_PURCHASE_TO_ORDER,
				PointInc = pointInc,
				UserPointExp = userPointExp,
				Kbn1 = orderId,
				Kbn2 = fixedPurchaseId,
				LastChanged = lastChanged,
			};
			// 登録できていれば成功
			var result = (RegisterHistory(history, accessor) > 0);
			if (result == false) return "1-3-1-A.定期台帳側のポイント移行履歴INSERT処理";

			// 生成された注文側のポイント利用履歴を
			history.PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_USE_POINT;
			history.PointInc = -pointInc;
			history.RestoredFlg = Constants.FLG_USERPOINTHISTORY_POINT_RESTORED_FLG_NOT_RESTORED;
			// 登録できていれば成功
			result = (RegisterHistory(history, accessor) > 0);
			if (result == false) return "1-3-1-B.生成された通常注文のポイント利用履歴INSERT処理";

			return string.Empty;
		}
		#endregion

		#region -ReturnUnUsePoint 利用されなかったポイント分をユーザポイントに戻す
		/// <summary>
		/// 利用されなかったポイント分をユーザポイントに戻す
		/// </summary>
		/// <param name="userPointList">ユーザポイント列</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="nextShippingUsePoint">次回購入の利用ポイント</param>
		/// <param name="orderUsePoint">注文の利用ポイント</param>
		/// <param name="expiredDate">ユーザ最新有効期限</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>失敗処理の名前（成功の場合、空に返却）</returns>
		private string ReturnUnUsePoint(
			UserPointModel[] userPointList,
			string userId,
			string deptId,
			string orderId,
			string fixedPurchaseId,
			string lastChanged,
			decimal nextShippingUsePoint,
			decimal orderUsePoint,
			DateTime? expiredDate,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// ユーザポイントに適用されなかった分のポイントを戻す
			var history = new UserPointHistoryModel()
			{
				UserId = userId,
				DeptId = deptId,
				PointRuleId = string.Empty,
				PointRuleKbn = string.Empty,
				PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURN_NOT_USE_POINT,
				PointInc = nextShippingUsePoint - orderUsePoint,
				UserPointExp = expiredDate,
				Kbn1 = orderId,
				Kbn2 = fixedPurchaseId,
				LastChanged = lastChanged,
			};
			// 登録できていれば成功
			var result = (RegisterHistory(history, accessor) > 0);
			if (result == false) return "1-3-1-C.適用されなかった分のポイントはお客様のポイントに戻す";

			// 有効期限切れ、または、ユーザポイントレコードなし
			if ((userPointList.Any() == false) || (userPointList.First().PointExp < DateTime.Now))
			{
				// ユーザポイントに更新せず、ポイント消滅履歴の作成
				history.PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_POINT_EXPIRED;
				history.PointInc = orderUsePoint - nextShippingUsePoint;
				history.UserPointExp = expiredDate;
				history.Kbn1 = string.Empty;
				history.Kbn2 = string.Empty;
				// 登録できていれば成功
				result = (RegisterHistory(history, accessor) > 0);
				if (result == false) return "1-3-1-D.有効期限切れポイント履歴の作成";
			}
			else
			{
				// 有効な本ポイントレコードがあれば、ユーザポイントの更新
				var compPoint = userPointList.First();
				compPoint.Point += nextShippingUsePoint - orderUsePoint;
				result = (UpdateUserPoint(compPoint, updateHistoryAction, accessor) > 0);
				if (result == false) return "1-3-1-E.ポイント利用戻しをユーザポイントに更新";
			}
			return string.Empty;
		}
		#endregion

		#region +AdjustOrderPointAddForPointTempAtUsePointChange 利用ポイント変更時の購入時付与仮ポイント調整
		/// <summary>
		/// 利用ポイント変更時の購入時付与仮ポイント調整
		/// </summary>
		/// <param name="userId">対象ユーザーID</param>
		/// <param name="orderId">対象注文ID</param>
		/// <param name="isFirstBuy">初回購入の注文か</param>
		/// <param name="pointFirstBuy">初回購入発行ポイント数</param>
		/// <param name="pointOrder">購入時発行ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>処理件数</returns>
		public int AdjustOrderPointAddForPointTempAtUsePointChange(
			string userId,
			string orderId,
			bool isFirstBuy,
			decimal pointFirstBuy,
			decimal pointOrder,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// トランザクションを使いまわす
			using (var repository = new PointRepository(accessor))
			{
				// ユーザーポイント
				var userPoint = repository.GetUserPoint(userId, string.Empty);

				var updateCount = 0;

				// 初回購入発行の仮ポイント更新
				if (isFirstBuy)
				{
					var tempPointFirstBuy = userPoint
						.Where(x => x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP
							&& x.Kbn1 == orderId
							&& x.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY);

					foreach (var point in tempPointFirstBuy)
					{
						updateCount += AdjustTempPoint(
							point,
							pointFirstBuy,
							point.PointExp,
							lastChanged,
							accessor);
					}
				}

				// 購入時発行の仮ポイント更新
				var tempPoint = userPoint
					.Where(x => x.PointType == Constants.FLG_USERPOINT_POINT_TYPE_TEMP
						&& x.Kbn1 == orderId
						&& x.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BUY);

				foreach (var point in tempPoint)
				{
					updateCount += AdjustTempPoint(
						point,
						pointOrder,
						userPoint.Max(x => x.PointExp),
						lastChanged,
						accessor);
				}

				if (updateCount == 0) return 0;

				// 注文の付与ポイントの補正
				// DB上の注文IDの仮ポイントの合計で補正
				// Hack：リファクタリングが進めば、注文サービスに利用ポイント数調整メソッドができ、そちらに処理一式が寄せられると思う
				var sumtempPoint = repository.GetUserPoint(userId, string.Empty)
					.Where(x => x.Kbn1 == orderId)
					.Sum(x => x.Point);
				updateCount = new OrderService().AdjustAddPoint(orderId, sumtempPoint, lastChanged, UpdateHistoryAction.DoNotInsert, accessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
					new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
				}

				return updateCount;
			}
		}

		/// <summary>
		/// 仮ポイント調整
		/// </summary>
		/// <param name="pointModel">ポイントモデル</param>
		/// <param name="pointAdd">付与ポイント</param>
		/// <param name="pointExp">ポイント期限</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>処理件数</returns>
		private int AdjustTempPoint(UserPointModel pointModel, decimal pointAdd, DateTime? pointExp, string lastChanged, SqlAccessor accessor = null)
		{
			if (pointModel == null) return 0;

			// 注文の仮ポイントを更新
			// ポイント調整数加算
			pointModel.Point += (pointAdd - pointModel.Point);
			pointModel.LastChanged = lastChanged;

			var rtn = UpdateUserPoint(pointModel, accessor);

			// 更新件数が0件の場合は楽観ロックNG
			if (rtn == 0) return rtn;

			// 履歴をとる
			// 本ポイント戻し用の履歴情報を作る
			var restoreHistory = new UserPointHistoryModel()
			{
				UserId = pointModel.UserId,
				DeptId = pointModel.DeptId,
				PointRuleId = string.Empty,
				PointRuleKbn = string.Empty,
				PointKbn = Constants.FLG_USERPOINTHISTORY_POINT_KBN_BASE,
				PointType = Constants.FLG_USERPOINTHISTORY_POINT_TYPE_TEMP,
				PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_KBN_ORDER,
				PointInc = pointModel.Point,
				UserPointExp = pointExp,
				PointExpExtend = UserPointHistoryModel.DEFAULT_POINT_EXP_EXTEND_STRING,
				Kbn1 = pointModel.Kbn1,
				Kbn2 = string.Empty,
				Kbn3 = string.Empty,
				Kbn4 = string.Empty,
				Kbn5 = string.Empty,
				Memo = "付与予定の仮ポイントを調整",
				LastChanged = lastChanged
			};
			RegisterHistory(restoreHistory, accessor);
			return rtn;
		}
		#endregion

		#region +CheckTargetListUsed ターゲットリストで使われているか
		/// <summary>
		/// ターゲットリストで使われているか
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>ポイントルールスケジュールモデル</returns>
		public PointRuleScheduleModel[] CheckTargetListUsed(string targetId)
		{
			using (var repository = new PointRepository())
			{
				var result = repository.CheckTargetListUsed(targetId);

				return result;
			}
		}
		#endregion

		#region +AdjustOrderPointUseForPointComp 返品交換時の利用ポイント調整（本ポイント用）
		/// <summary>
		/// 返品交換時の利用ポイント調整（本ポイント用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">返品元の注文ID</param>
		/// <param name="adjustPoint">調整ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">トランザクションを内包するアクセサ(指定しない場合はメソッド内でトランザクション完結)</param>
		/// <returns>更新件数</returns>
		public int AdjustOrderPointUseForPointComp(
			string deptId,
			string userId,
			string orderId,
			decimal adjustPoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 調整
			var updated = AdjustOrderPointUseForPointComp(deptId, userId, orderId, adjustPoint, lastChanged, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(userId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -AdjustOrderPointUseForPointComp 返品交換時の利用ポイント調整（本ポイント用）
		/// <summary>
		/// 返品交換時の利用ポイント調整（本ポイント用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">返品元の注文ID</param>
		/// <param name="adjustPoint">調整ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しない場合はメソッド内でトランザクション完結)</param>
		/// <returns>更新件数</returns>
		private int AdjustOrderPointUseForPointComp(
			string deptId,
			string userId,
			string orderId,
			decimal adjustPoint,
			string lastChanged,
			SqlAccessor sqlAccessor)
		{
			int rtn = 0;
			// トランザクションを使いまわす
			using (var repository = new PointRepository(sqlAccessor))
			{
				// ユーザーポイント
				var userPoiont =
					repository.GetUserPoint(userId, string.Empty)
						.Where(x => (x.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE))
						.ToArray();

				// 本ポイントあり？
				var compPoint = userPoiont.FirstOrDefault(p => p.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP);
				if (compPoint != null)
				{
					// 更新
					compPoint.Point += adjustPoint;
					compPoint.LastChanged = lastChanged;
					rtn = repository.UpdateUserPoint(compPoint);
				}

				// 更新件数が0件の場合は楽観ロックNG
				if (rtn == 0)
				{
					return rtn;
				}

				// 履歴をとる
				// 本ポイント更新用履歴情報を作る
				var restoreHistory = new UserPointHistoryModel
				{
					UserId = userId,
					DeptId = deptId,
					PointRuleId = string.Empty,
					PointRuleKbn = string.Empty,
					PointKbn = Constants.FLG_USERPOINT_POINT_KBN_BASE,
					PointType = Constants.FLG_USERPOINTHISTORY_POINT_TYPE_COMP,
					PointIncKbn = Constants.FLG_USERPOINTHISTORY_POINT_INC_KBN_RETURNEXCHANGE_ADJUST_ORDER_POINT_USE,
					PointInc = adjustPoint,
					// 有効期限はマックスを使う（元SQLからこうなっていたが・・・）
					UserPointExp = userPoiont.Max(x => x.PointExp),
					PointExpExtend = UserPointHistoryModel.GetPointExpExtendFormtString(1, 0, 0),
					Kbn1 = orderId,
					Kbn2 = string.Empty,
					Kbn3 = string.Empty,
					Kbn4 = string.Empty,
					Kbn5 = string.Empty,
					Memo = string.Empty,
					LastChanged = lastChanged
				};
				RegisterHistory(restoreHistory, sqlAccessor);
			}
			return rtn;
		}
		#endregion

		#region +GetUserPointHistoryByOrderId 注文IDで履歴取得
		/// <summary>
		/// 注文IDでユーザーポイント履歴取得
		/// </summary>
		/// <param name="userId">ユーザーIS</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>ユーザーポイント履歴</returns>
		public UserPointHistoryModel[] GetUserPointHistoryByOrderId(
			string userId,
			string orderId,
			SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				var models = repository.GetUserPointHistoryByOrderId(userId, orderId);
				return models;
			}
		}
		#endregion

		#region +GetHistoriesByGroupNo 履歴グループ番号で取得
		/// <summary>
		/// 履歴グループ番号で取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="historyGroupNo">履歴グループ番号</param>
		/// <returns>ユーザーポイント履歴モデル</returns>
		public UserPointHistoryModel[] GetHistoriesByGroupNo(string userId, int historyGroupNo)
		{
			using (var repository = new PointRepository())
			{
				var models = repository.GetHistoriesByGroupNo(userId, historyGroupNo);
				return models;
			}
		}
		#endregion

		#region -IssueHistoryGroupNoForUser 履歴グループ番号を採番
		/// <summary>
		/// 履歴グループ番号を採番
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>履歴グループ番号</returns>
		public int IssueHistoryGroupNoForUser(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				var groupNo = repository.IssueHistoryGroupNoForUser(userId);
				return groupNo;
			}
		}
		#endregion

		#region +IssuePointKbnNoForUser ポイント枝番を採番
		/// <summary>
		/// ポイント枝番を採番
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>枝番</returns>
		public int IssuePointKbnNoForUser(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				var pointKbnNo = repository.IssuePointKbnNoForUser(userId);
				return pointKbnNo;
			}
		}
		#endregion

		#region -RegisterOrAddUserPoint ユーザーポイント登録、または加算
		/// <summary>
		/// ユーザーポイント登録、または加算
		/// </summary>
		/// <param name="userPointSrc">ポイント情報</param>
		/// <param name="accessor">アクセサ</param>
		/// <param name="shouldGetLatest">最終的なポイント情報を取得する必要があるか</param>
		/// <returns>影響件数</returns>
		/// <remarks>
		/// 「加算」となる条件
		///     通常ポイント → 保有していれば加算
		/// 期間限定ポイント → 利用期間と発行ルールIDが同じ物に加算
		/// </remarks>
		/// <returns>最終的なポイント情報</returns>
		private UserPointModel RegisterOrAddUserPoint(
			UserPointModel userPointSrc,
			SqlAccessor accessor,
			bool shouldGetLatest = false)
		{
			var userPoints = GetUserPoint(userPointSrc.UserId, string.Empty, accessor);

			UserPointModel userPointUpd;
			// 既存のポイント取得
			switch (userPointSrc.PointKbn)
			{
				// 通常本ポイント
				case Constants.FLG_USERPOINT_POINT_KBN_BASE:
					userPointUpd = userPoints
						.FirstOrDefault(p => (p.IsPointTypeComp && p.IsBasePoint));
					break;

				// 期間限定ポイント
				case Constants.FLG_USERPOINT_POINT_KBN_LIMITED_TERM_POINT:
					userPointUpd = userPoints
						.FirstOrDefault(
							p => (p.IsPointTypeComp)
								&& (p.IsLimitedTermPoint)
								&& (p.EffectiveDate == userPointSrc.EffectiveDate)
								&& (p.PointExp == userPointSrc.PointExp)
								&& (p.PointRuleId == userPointSrc.PointRuleId)
								&& (p.OrderId == userPointSrc.OrderId));
					break;

				default:
					throw new Exception("不明なポイント区分：" + userPointSrc.PointKbn);
			}

			// 条件に一致するポイント情報がある場合、加算
			if ((userPointUpd != null) && userPointSrc.IsPointTypeComp)
			{
				userPointUpd.Point += userPointSrc.Point;
				userPointUpd.EffectiveDate = userPointSrc.EffectiveDate;
				userPointUpd.LastChanged = userPointSrc.LastChanged;

				if ((userPointUpd.PointExp != null)
					&& (userPointSrc.PointExp != null)
					&& (userPointUpd.PointExp < userPointSrc.PointExp))
				{
					userPointUpd.PointExp = userPointSrc.PointExp;
				}
			}
			// 条件に一致するポイント情報がない場合、新規発行（枝番を新規採番）
			else
			{
				userPointUpd = UserPointModel.DeepClone(userPointSrc);
				userPointUpd.PointKbnNo = IssuePointKbnNoForUser(userPointSrc.UserId, accessor);
			}

			// 更新
			var updated = UpsertUserPoint(userPointUpd, UpdateHistoryAction.DoNotInsert, accessor);
			if (updated <= 0)
			{
				throw new Exception("ポイント更新処理に失敗。");
			}

			// 呼び出し元で戻り値をそのままUPSERTなどに用いる場合があるが、
			// メソッド内で生成したポイント情報をUPSERTしても楽観ロックに引っかかる為、
			// 明示的に指定された場合は最新のポイント情報を取得して返却する
			if (shouldGetLatest)
			{
				userPointUpd = GetUserPoint(userPointSrc.UserId, string.Empty, accessor)
					.First(p => (p.PointKbnNo == userPointUpd.PointKbnNo));
			}

			return userPointUpd;
		}
		#endregion

		#region +GetUserPointHistoriesForRestore 復元処理用にポイント履歴取得
		/// <summary>
		/// 復元処理用にポイント履歴取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="isBeforeMigration">VUP前の履歴であったか？</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>ユーザーポイント履歴</returns>
		public UserPointHistoryModel[] GetUserPointHistoriesForRestore(
			string userId,
			string orderId,
			string lastChanged,
			out bool isBeforeMigration,
			SqlAccessor accessor)
		{
			var histories = PointOperationHelper.GetUserPointHistoriesForRestore(
				userId,
				orderId,
				lastChanged,
				out isBeforeMigration,
				accessor);
			return histories;
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <returns>チェックOKか</returns>
		public bool CheckUserPointDetailFieldsForGetMaster(string sqlFieldNames)
		{
			try
			{
				using (var repository = new PointRepository())
				{
					repository.CheckUserPointDetailFieldsForGetMaster(
						new Hashtable(),
						new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteWarn(ex);
				return false;
			}
			return true;
		}

		/// <summary>
		///  CSVへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か</returns>
		public bool ExportToCsv(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			using (var accessor = new SqlAccessor())
			using (var repository = new PointRepository(accessor))
			using (var reader = repository.GetMasterWithReader(
				input,
				statementName,
				new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames)))
			{
				new MasterExportCsv().Exec(setting, reader, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
			}
			return true;
		}

		/// <summary>
		/// Excelへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="excelTemplateSetting">Excelテンプレート設定</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か（件数エラーの場合は失敗）</returns>
		public bool ExportToExcel(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			ExcelTemplateSetting excelTemplateSetting,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			using (var repository = new PointRepository())
			{
				var dv = repository.GetMaster(input, statementName, new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				if (dv.Count >= 20000) return false;

				new MasterExportExcel().Exec(setting, excelTemplateSetting, dv, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				return true;
			}
		}

		/// <summary>
		/// マスタ区分で該当するStatementNameを取得
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>StatementName</returns>
		private string GetStatementNameInfo(string masterKbn)
		{
			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT: // ユーザーポイント
					return "GetUserPointListExport";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT_DETAIL: // ユーザーポイント(詳細)
					return "GetUserPointDetailListExport";
			}
			throw new Exception("未対応のマスタ区分：" + masterKbn);
		}
		#endregion

		#region +AdjustPointByCrossPoint
		/// <summary>
		/// Adjust point by Cross Point
		/// </summary>
		/// <param name="model">User point model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of cases affected</returns>
		public int AdjustPointByCrossPoint(UserPointModel model, SqlAccessor accessor)
		{
			using (var repository = new PointRepository(accessor))
			{
				// User point info
				var userPoint = repository.GetUserPoint(model.UserId, string.Empty)
					.Where(item => (item.PointKbn == Constants.FLG_USERPOINT_POINT_KBN_BASE))
					.ToArray();

				// Real point
				var realPoint = userPoint
					.FirstOrDefault(item => (item.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP));

				// Create point info after change
				UserPointModel updPoint;
				if (realPoint != null)
				{
					updPoint = realPoint;
					updPoint.Point = model.Point;
					updPoint.PointExp = model.PointExp;
					updPoint.LastChanged = model.LastChanged;
				}
				else
				{
					updPoint = new UserPointModel
					{
						UserId = model.UserId,
						DeptId = model.DeptId,
						Point = model.Point,
						LastChanged = model.LastChanged,
						PointExp = model.PointExp,
						DateChanged = DateTime.Now,
					};
				}
				var result = UpsertUserPoint(updPoint, UpdateHistoryAction.DoNotInsert, accessor);

				return result;
			}
		}
		#endregion

		#region +Issue point by Cross Point
		/// <summary>
		/// Issue point by Cross Point
		/// </summary>
		/// <param name="model">User point model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <param name="isRegister">Is register</param>
		/// <returns>Number of cases affected</returns>
		public int IssuePointByCrossPoint(
			UserPointModel model,
			SqlAccessor accessor,
			bool isRegister = true)
		{
			if (isRegister)
			{
				RegisterOrAddUserPoint(model, accessor);
			}

			var history = new UserPointHistoryModel
			{
				UserId = model.UserId,
				DeptId = Constants.CONST_DEFAULT_DEPT_ID,
				PointRuleId = model.PointRuleId,
				PointRuleKbn = model.PointRuleKbn,
				PointKbn = model.PointKbn,
				PointType = model.PointType,
				PointIncKbn = model.PointIncKbn,
				PointInc = model.Point,
				UserPointExp = model.PointExp,
				PointExpExtend = string.Empty,
				OrderId = model.OrderId,
				LastChanged = model.LastChanged,
			};

			var result = RegisterHistory(history, accessor);
			return result;
		}
		#endregion

		#region +GetUserPointHistoriesOnFront ユーザーポイント履歴取得（Front表示用）
		/// <summary>
		/// ユーザーポイント履歴取得（Front表示用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ユーザーポイント履歴情報</returns>
		public UserPointHistoryContainer[] GetUserPointHistoriesOnFront(string userId)
		{
			using (var repository = new PointRepository())
			{
				var histories = repository.GetUserPointHistoriesOnFront(userId);
				return histories;
			}
		}
		#endregion

		#region +GetNextHistoryNo ユーザーポイント履歴Noを取得
		/// <summary>
		/// ユーザーポイント履歴Noを取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>ユーザー履歴No</returns>
		public int GetNextHistoryNo(string userId, SqlAccessor sqlAccessor = null)
		{
			using (var repository = new PointRepository(sqlAccessor))
			{
				var result = repository.GetNextHistoryNo(userId);
				return result;
			}
		}
		#endregion

		#region +UpdateUserPointHistoryForIntegration ユーザーポイント履歴更新(ユーザー統合用)
		/// <summary>
		/// ユーザーポイント履歴更新(ユーザー統合用)
		/// </summary>
		/// <param name="userId">ユーザーID(新)</param>
		/// <param name="userIdOld">ユーザーID(旧)</param>
		/// <param name="historyNo">履歴No(新)</param>
		/// <param name="historyNoOld">履歴No(旧)</param>
		/// <param name="historyGroupNo">履歴グループ番号</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateUserPointHistoryForIntegration(
			string userId,
			string userIdOld,
			int historyNo,
			int historyNoOld,
			int historyGroupNo,
			SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				var result = repository.UpdateUserPointHistoryForIntegration(
					userId,
					userIdOld,
					historyNo,
					historyNoOld,
					historyGroupNo);
				return result;
			}
		}
		#endregion

		#region +DeleteUserPointHistory ユーザーポイント履歴削除
		/// <summary>
		/// ユーザーポイント履歴削除
		/// </summary>
		/// <param name="userPointHistory">ユーザーポイント履歴</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>件数</returns>
		public int DeleteUserPointHistory(UserPointHistoryModel userPointHistory, SqlAccessor accessor = null)
		{
			using (var repository = new PointRepository(accessor))
			{
				var result = repository.DeleteUserPointHistory(userPointHistory);
				return result;
			}
		}
		#endregion
	}
}
