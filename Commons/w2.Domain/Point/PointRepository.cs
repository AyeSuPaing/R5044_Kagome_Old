/*
=========================================================================================================
  Module      : ポイントリポジトリ (PointRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Point.Helper;

namespace w2.Domain.Point
{
	/// <summary>
	/// ポイントリポジトリ
	/// </summary>
	internal class PointRepository : RepositoryBase
	{
		/// <summary>キー名</summary>
		private const string XML_KEY_NAME = "Point";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal PointRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal PointRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~SearchUserPoint ユーザーポイント検索
		/// <summary>
		/// ユーザーポイント検索
		/// </summary>
		/// <param name="cond">検索条件</param>
		/// <returns>
		/// 検索結果
		/// 0件の場合は0配列
		/// </returns>
		internal UserPointSearchResult[] SearchUserPoint(UserPointSearchCondition cond)
		{
			var dv = base.Get(XML_KEY_NAME, "SearchUserPoint", cond.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new UserPointSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~GetSearchHitCountUserPointHistorySummary ユーザーポイント履歴（概要）検索数取得
		/// <summary>
		/// ユーザーポイント履歴（概要）検索数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索ヒット数</returns>
		public int GetSearchHitCountUserPointHistorySummary(UserPointHistorySummarySearchCondition condition)
		{
			var param = condition.CreateHashtableParams();
			var dv = Get(
				XML_KEY_NAME,
				"GetSearchHitCountUserPointHistorySummary",
				param);

			return (int)dv[0][0];
		}
		#endregion

		#region ~GetCountOfSearchUserPoint ユーザーポイント検索結果数取得
		/// <summary>
		/// ユーザーポイント検索結果数取得
		/// </summary>
		/// <param name="cond">検索条件</param>
		/// <returns>検索結果件数</returns>
		internal int GetCountOfSearchUserPoint(UserPointSearchCondition cond)
		{
			var dv = base.Get(XML_KEY_NAME, "GetCountOfSearchUserPoint", cond.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~SearchUserPointHistorySummary ユーザーポイント履歴（概要）検索
		/// <summary>
		/// ユーザーポイント履歴（概要）検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果</returns>
		public UserPointHistorySummarySearchResult[] SearchUserPointHistorySummary(UserPointHistorySummarySearchCondition condition)
		{
			var dv = Get(
				XML_KEY_NAME,
				"SearchUserPointHistorySummary",
				condition.CreateHashtableParams());

			return dv.Cast<DataRowView>().Select(drv => new UserPointHistorySummarySearchResult(drv)).ToArray();
		}
		#endregion

		#region ~SearchUserPoint ユーザーポイント履歴検索
		/// <summary>
		/// ユーザーポイント履歴検索
		/// </summary>
		/// <param name="cond">検索条件</param>
		/// <returns>
		/// 検索結果
		/// 0件の場合は0配列
		/// </returns>
		internal UserPointHistorySearchResult[] SearchUserPointHistory(UserPointHistorySearchCondition cond)
		{
			var dv = base.Get(XML_KEY_NAME, "SearchUserPointHistory", cond.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new UserPointHistorySearchResult(drv)).ToArray();
		}
		#endregion

		#region ~SearchPointRuleList ポイントルールリスト検索
		/// <summary>
		/// ポイントルールリスト検索
		/// </summary>
		/// <param name="cond">検索条件</param>
		/// <returns>
		/// 検索結果
		/// 0件の場合は0配列
		/// </returns>
		internal PointRuleListSearchResult[] SearchPointRuleList(PointRuleListSearchCondition cond)
		{
			var dv = base.Get(XML_KEY_NAME, "SearchPointRuleList", cond.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new PointRuleListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~UpsertUserPoint ユーザーポイントUpSert
		/// <summary>
		/// ユーザーポイントUpSert
		/// </summary>
		/// <param name="model">Updsert内容</param>
		/// <returns>UpSert件数</returns>
		/// <remarks>あれば更新・なければ登録（Merge)</remarks>
		internal int UpsertUserPoint(UserPointModel model)
		{
			var updated = Exec(XML_KEY_NAME, "UpsertUserPoint", model.DataSource);
			return updated;
		}
		#endregion

		#region ~RegisterUserPoint ユーザーポイント追加
		/// <summary>
		/// ユーザーポイント追加
		/// </summary>
		/// <param name="model">登録内容</param>
		/// <returns>登録件数</returns>
		internal int RegisterUserPoint(UserPointModel model)
		{
			return Exec(XML_KEY_NAME, "RegisterUserPoint", model.DataSource);
		}
		#endregion

		#region ~UpdateUserPoint ユーザーポイント更新
		/// <summary>
		/// ユーザーポイント更新
		/// </summary>
		/// <param name="model">登録内容</param>
		/// <returns>更新件数</returns>
		internal int UpdateUserPoint(UserPointModel model)
		{
			return base.Exec(XML_KEY_NAME, "UpdateUserPoint", model.DataSource);
		}
		#endregion

		#region ~RegisterHistory 履歴登録
		/// <summary>
		/// 履歴登録
		/// </summary>
		/// <param name="model">登録内容</param>
		/// <returns>枝番</returns>
		internal int RegisterHistory(UserPointHistoryModel model)
		{
			return (int)base.Get(XML_KEY_NAME, "RegisterHistory", model.DataSource)[0][Constants.FIELD_USERPOINTHISTORY_HISTORY_NO];
		}
		#endregion

		#region ~GetUserPoint ユーザーポイント情報
		/// <summary>
		/// ユーザーポイント情報
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="cartId">カートID</param>
		/// <returns>
		/// ユーザーポイント情報
		/// ポイントの情報が取れなければ0配列
		/// </returns>
		internal UserPointModel[] GetUserPoint(string userId, string cartId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_USERPOINT_USER_ID, userId },
				{ Constants.FIELD_USERPOINTHISTORY_CART_ID, cartId },
			};
			var dv = Get(XML_KEY_NAME, "GetUserPoint", input);
			var result = dv
				.Cast<DataRowView>()
				.Select(drv => new UserPointModel(drv))
				.ToArray();
			return result;
		}
		#endregion

		#region ~DeleteUserPoint ユーザーポイント削除
		/// <summary>
		/// ユーザーポイント削除
		/// </summary>
		/// <param name="model">削除内容</param>
		/// <returns>削除件数</returns>
		internal int DeleteUserPoint(UserPointModel model)
		{
			return base.Exec(XML_KEY_NAME, "DeleteUserPoint", model.DataSource);
		}
		#endregion

		#region ~DeleteUserPointByOrderId 注文IDに紐づくユーザーポイント削除
		internal int DeleteUserPointByOrderId(string orderId)
		{
			return Exec(
				XML_KEY_NAME,
				"DeleteUserPointByOrderId",
				new Hashtable
				{
					{ Constants.FIELD_USERPOINT_KBN1, orderId },
				});
		}
		#endregion

		#region ~GetPointRule ポイントルール取得
		/// <summary>
		/// ポイントルール取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="pointRuleId">ポイントルールID</param>
		/// <returns>
		/// ポイントルール
		/// 取得できなかった場合はNull
		/// </returns>
		internal PointRuleModel GetPointRule(string deptId, string pointRuleId)
		{
			var dv = base.Get(XML_KEY_NAME, "GetPointRule", new Hashtable { { Constants.FIELD_POINTRULE_DEPT_ID, deptId }, { Constants.FIELD_POINTRULE_POINT_RULE_ID, pointRuleId } });
			if (dv.Count == 0)
			{
				return null;
			}
			return new PointRuleModel(dv.Cast<DataRowView>().FirstOrDefault());
		}
		#endregion

		#region ~GetAllPointRule 全ポイントルール取得
		/// <summary>
		/// 全ポイントルール取得
		/// </summary>
		/// <returns>
		/// ポイントルール
		/// </returns>
		internal PointRuleModel[] GetAllPointRules()
		{
			var dv = base.Get(XML_KEY_NAME, "GetAllPointRules");
			return dv.Cast<DataRowView>().Select(drv => new PointRuleModel(drv)).ToArray();
		}
		#endregion

		#region ~GetPointRuleMaxKey ポイントルールIDのMaxを取得
		/// <summary>
		/// ポイントルールIDのMaxを取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>ポイントルールIDのMax</returns>
		internal string GetPointRuleMaxKey(string deptId)
		{
			return StringUtility.ToEmpty(base.Get(XML_KEY_NAME, "GetPointRuleMaxKey", new Hashtable { { Constants.FIELD_POINTRULE_DEPT_ID, deptId } })[0][0]);
		}
		#endregion

		#region ~RegisterPointRule ポイントルール登録
		/// <summary>
		/// ポイントルール登録
		/// </summary>
		/// <param name="model">登録内容</param>
		/// <returns>登録件数</returns>
		internal int RegisterPointRule(PointRuleModel model)
		{
			return base.Exec(XML_KEY_NAME, "RegisterPointRule", model.DataSource);
		}
		#endregion

		#region ~UpdatePointRule ポイントルール更新
		/// <summary>
		/// ポイントルール更新
		/// </summary>
		/// <param name="model">更新内容</param>
		/// <returns>更新件数</returns>
		internal int UpdatePointRule(PointRuleModel model)
		{
			return base.Exec(XML_KEY_NAME, "UpdatePointRule", model.DataSource);
		}
		#endregion

		#region ~DeletePointRule ポイントルール削除
		/// <summary>
		/// ポイントルール削除
		/// </summary>
		/// <param name="model">削除内容</param>
		/// <returns>削除件数</returns>
		internal int DeletePointRule(PointRuleModel model)
		{
			return base.Exec(XML_KEY_NAME, "DeletePointRule", model.DataSource);
		}
		#endregion

		#region ~GetPointRuleDate ポイントルール日付取得
		/// <summary>
		/// ポイントルール日付取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="pointRuleId">ポイントルールID</param>
		/// <returns>
		/// ポイントルール日付
		/// 取得できなかったら0配列
		/// </returns>
		internal PointRuleDateModel[] GetPointRuleDate(string deptId, string pointRuleId)
		{
			var dv = base.Get(XML_KEY_NAME, "GetPointRuleDate", new Hashtable { { Constants.FIELD_POINTRULEDATE_DEPT_ID, deptId }, { Constants.FIELD_POINTRULEDATE_POINT_RULE_ID, pointRuleId } });
			return dv.Cast<DataRowView>().Select(i => new PointRuleDateModel(i)).ToArray();
		}
		#endregion

		#region ~RegisterPointRuleDate ポイントルール日付登録
		/// <summary>
		/// ポイントルール日付登録
		/// </summary>
		/// <param name="model">登録内容</param>
		/// <returns>登録件数</returns>
		internal int RegisterPointRuleDate(PointRuleDateModel model)
		{
			return base.Exec(XML_KEY_NAME, "RegisterPointRuleDate", model.DataSource);
		}
		#endregion

		#region ~DeletePointRuleDate ポイントルール日付削除
		/// <summary>
		/// ポイントルール日付削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="pointRuleId">ポイントルールID</param>
		/// <returns>削除件数</returns>
		internal int DeletePointRuleDate(string deptId, string pointRuleId)
		{
			return base.Exec(XML_KEY_NAME, "DeletePointRuleDate", new Hashtable { { Constants.FIELD_POINTRULEDATE_DEPT_ID, deptId }, { Constants.FIELD_POINTRULEDATE_POINT_RULE_ID, pointRuleId } });
		}
		#endregion

		#region ~PointTransitionReportDay 日別ポイント推移レポート
		/// <summary>
		/// 日別ポイント推移レポート
		/// </summary>
		/// <param name="cond">レポート検索条件</param>
		/// <returns>
		/// ポイント推移レポート
		/// 結果が0件の場合は0配列
		/// </returns>
		internal PointTransitionReportResult[] PointTransitionReportDay(PointTransitionReportCondition cond)
		{
			var dv = base.Get(XML_KEY_NAME, "PointTransitionReportDay", cond.CreateHashtableParams());

			return dv.Cast<DataRowView>().Select(drv => new PointTransitionReportResult(drv, cond.ReportType)).ToArray();
		}
		#endregion

		#region ~PointTransitionReportMonth 月別ポイント推移レポート
		/// <summary>
		/// 日別ポイント推移レポート
		/// </summary>
		/// <param name="cond">レポート検索条件</param>
		/// <returns>
		/// ポイント推移レポート
		/// 結果が0件の場合は0配列
		/// </returns>
		internal PointTransitionReportResult[] PointTransitionReportMonth(PointTransitionReportCondition cond)
		{
			var dv = base.Get(XML_KEY_NAME, "PointTransitionReportMonth", cond.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new PointTransitionReportResult(drv, cond.ReportType)).ToArray();
		}
		#endregion

		#region ~GetPointMaster ポイントマスタ取得
		/// <summary>
		/// ポイントマスタ取得
		/// </summary>
		/// <returns>ポイントマスタ</returns>
		internal PointModel[] GetPointMaster()
		{
			var dv = base.Get(XML_KEY_NAME, "GetPointMaster");
			return dv.Cast<DataRowView>().Select(drv => new PointModel(drv)).ToArray();
		}
		#endregion

		#region ~GetUserPointHistory ユーザーポイント履歴情報
		/// <summary>
		/// ユーザーポイント履歴情報
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>
		/// ユーザーポイント履歴情報
		/// 履歴情報が取れなければ0配列
		/// </returns>
		internal UserPointHistoryModel[] GetUserPointHistories(string userId)
		{
			var dv = base.Get(XML_KEY_NAME, "GetUserPointHistories", new Hashtable { { Constants.FIELD_USERPOINT_USER_ID, userId } });
			return dv
					.Cast<DataRowView>()
					.Select(drv => new UserPointHistoryModel(drv))
					.ToArray();
		}
		#endregion

		#region ~GetUserHistoriesByPointIncKbn ユーザーポイント履歴取得(ポイント加算区分)
		/// <summary>
		/// ユーザーポイント履歴情報(ポイント加算区分)
		/// </summary>
		/// <param name="pointIncKbn">ポイント加算区分</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ユーザーポイント履歴情報</returns>
		internal UserPointHistoryModel[] GetUserHistoriesByPointIncKbn(string pointIncKbn, string userId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_USERPOINTHISTORY_POINT_INC_KBN, pointIncKbn },
				{ Constants.FIELD_USERPOINTHISTORY_USER_ID, userId },
			};
			var dv = Get(XML_KEY_NAME, "GetUserHistoriesByPointIncKbn", input);

			return dv
				.Cast<DataRowView>()
				.Select(drv => new UserPointHistoryModel(drv))
				.ToArray();
		}
		#endregion

		#region GetUserPointHistoryByOrderId ~指定注文IDのユーザーポイント履歴取得
		internal UserPointHistoryModel[] GetUserPointHistoryByOrderId(string userId, string kbn1)
		{
			var drv = Get(
				XML_KEY_NAME,
				"GetUserPointHistoryByOrderId",
				new Hashtable
				{
					{ Constants.FIELD_USERPOINT_USER_ID, userId },
					{ Constants.FIELD_USERPOINT_KBN1, kbn1 },
				});
			return drv.Cast<DataRowView>().Select(x => new UserPointHistoryModel(x)).ToArray();
		}
		#endregion

		#region ~DeleteUserPoint 指定注文IDのユーザーポイント削除
		/// <summary>
		/// 指定注文IDのユーザーポイント削除
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="kbn1">区分値1（注文ID）</param>
		/// <param name="pointKbn">ポイント区分</param>
		/// <returns>削除件数</returns>
		internal int DeleteUserPointHistoryByOrderID(string userId, string kbn1, string pointKbn)
		{
			return Exec(
				XML_KEY_NAME,
				"DeleteUserPointHistoryByOrderID",
				new Hashtable
				{
					{ Constants.FIELD_USERPOINTHISTORY_USER_ID, userId },
					{ Constants.FIELD_USERPOINTHISTORY_KBN1, kbn1 },
					{ Constants.FIELD_USERPOINTHISTORY_POINT_KBN, pointKbn },
				});
		}
		#endregion

		#region #MarkRestoredFlgByOrderId ユーザーポイント履歴を復元処理済でマークする
		/// <summary>
		/// ユーザーポイント履歴を復元処理済でマークする
		/// </summary>
		/// <param name="history">ユーザーポイント履歴</param>
		/// <returns>影響件数</returns>
		internal int MarkRestoredFlg(UserPointHistoryModel history)
		{
			return Exec(XML_KEY_NAME, "MarkRestoredFlgByOrderId", history.DataSource);
		}
		#endregion

		#region ~GetTargetUserTempPointToReal 本ポイント移行対象の仮ポイントを取得
		/// <summary>
		/// 本ポイント移行対象の仮ポイントを取得
		/// </summary>
		/// <param name="daysFromShippingForBasePoint">出荷後何日で本ポイントへ移行するか(通常ポイント)</param>
		/// <param name="daysFromShippingForLimitedTermPoint">出荷後何日で本ポイントへ移行するか(期間限定ポイント)</param>
		/// <returns>本ポイント移行対象となる仮ポイント</returns>
		internal UserPointModel[] GetTargetUserTempPointToReal(int daysFromShippingForBasePoint, int daysFromShippingForLimitedTermPoint)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetTargetUserTempPointToReal",
				new Hashtable
				{
					{ "point_temp_to_comp_days", daysFromShippingForBasePoint },
					{ "point_temp_to_comp_limited_term_point_days", daysFromShippingForLimitedTermPoint }
				});
			return dv.Cast<DataRowView>().Select(drv => new UserPointModel(drv)).ToArray();
		}
		#endregion

		#region ~GetExpiredUserPoints 有効期限切れポイント取得
		/// <summary>
		/// 有効期限切れポイント取得
		/// </summary>
		/// <returns>有効期限が切れたポイント</returns>
		internal UserPointModel[] GetExpiredUserPoints()
		{
			var dv = base.Get(XML_KEY_NAME, "GetExpiredUserPoints");
			return dv
					.Cast<DataRowView>()
					.Select(drv => new UserPointModel(drv))
					.ToArray();
		}
		#endregion

		#region ~DeleteExpiredPoint 有効期限切れポイント削除
		/// <summary>
		/// 有効期限切れポイント削除
		/// </summary>
		/// <returns>DB操作件数</returns>
		internal int DeleteExpiredPoint(UserPointModel model)
		{
			return base.Exec(XML_KEY_NAME, "DeleteExpiredPoint");
		}
		#endregion

		#region ~GetUpdLockForUserPoint ユーザポイント更新ロック取得(ポイントバッチ利用)
		/// <summary>
		/// ユーザポイント更新ロック取得(ポイントバッチ利用)
		/// </summary>
		internal void GetUpdLockForUserPoint(string userId)
		{
			base.Exec(XML_KEY_NAME, "GetUpdLockForUserPoint", new Hashtable { { Constants.FIELD_USERPOINT_USER_ID, userId } });
		}
		#endregion

		#region ~GetUpdLockForUserPointHistory ユーザポイント履歴更新ロック取得(ポイントバッチ利用)
		/// <summary>
		/// ユーザポイント履歴更新ロック取得(ポイントバッチ利用)
		/// </summary>
		internal void GetUpdLockForUserPointHistory(string userId)
		{
			base.Exec(XML_KEY_NAME, "GetUpdLockForUserPointHistory", new Hashtable { { Constants.FIELD_USERPOINTHISTORY_USER_ID, userId } });
		}
		#endregion

		#region ~GetHightPriorityCampaignRule 優先度の高いポイントキャンペーン取得
		/// <summary>
		/// 優先度の高いポイントキャンペーン取得
		/// </summary>
		/// <returns>優先度の高いポイントキャンペーン</returns>
		internal PointRuleModel[] GetHightPriorityCampaignRule(string deptId, DateTime currentDateTime)
		{
			var ht = new Hashtable { { Constants.FIELD_POINTRULE_DEPT_ID, deptId }, { "current_date", currentDateTime } };
			var dv = base.Get(XML_KEY_NAME, "GetHightPriorityCampaignRule", ht);

			// キャンペーンルールとキャンペーンルール日付をJOINしているので親子構造にする
			string ruleId = string.Empty;
			PointRuleModel model = null;
			var rtn = new List<PointRuleModel>();
			foreach (DataRowView drv in dv)
			{
				if (ruleId != StringUtility.ToEmpty(drv[Constants.FIELD_POINTRULE_POINT_RULE_ID]))
				{
					ruleId = StringUtility.ToEmpty(drv[Constants.FIELD_POINTRULE_POINT_RULE_ID]);
					model = new PointRuleModel(drv);
				}
				model.RuleDate = (model.RuleDate != null)
					? model.RuleDate.Concat(new[] { new PointRuleDateModel(drv) }).ToArray()
					: new[] { new PointRuleDateModel(drv) };

				rtn.Add(model);
			}
			return rtn.ToArray();
		}
		#endregion

		#region ~GetBasicRule 基本ルール取得
		/// <summary>
		/// 優先度の高いポイントキャンペーン取得
		/// </summary>
		/// <returns>優先度の高いポイントキャンペーン</returns>
		internal PointRuleModel[] GetBasicRule(string deptId, DateTime currentDateTime)
		{
			var ht = new Hashtable { { Constants.FIELD_POINTRULE_DEPT_ID, deptId }, { "current_date", currentDateTime } };
			var dv = base.Get(XML_KEY_NAME, "GetBasicRule", ht);
			return dv.Cast<DataRowView>().Select(drv => new PointRuleModel(drv)).ToArray();
		}
		#endregion

		#region ~SearchPointRuleScheduleList ポイントルールスケジュールリスト検索
		/// <summary>
		/// ポイントルールスケジュールリスト検索
		/// </summary>
		/// <param name="cond">検索条件</param>
		/// <returns>ポイントルールスケジュールリスト</returns>
		internal PointRuleScheduleListSearchResult[] SearchPointRuleScheduleList(PointRuleScheduleListSearchCondition cond)
		{
			var dv = base.Get(XML_KEY_NAME, "SearchPointRuleSchedule", cond.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new PointRuleScheduleListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~GetPointRuleSchedule ポイントルールスケジュール取得
		/// <summary>
		/// ポイントルールスケジュール取得
		/// </summary>
		/// <param name="pointRuleScheduleId">ポイントルールスケジュールID</param>
		/// <returns>ポイントルールスケジュール</returns>
		internal PointRuleScheduleModel GetPointRuleSchedule(string pointRuleScheduleId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_POINTRULESCHEDULE_POINT_RULE_SCHEDULE_ID, pointRuleScheduleId},
			};
			var dv = base.Get(XML_KEY_NAME, "GetPointRuleSchedule", ht);
			return new PointRuleScheduleModel(dv.Cast<DataRowView>().FirstOrDefault());
		}
		#endregion

		#region ~GetPointRuleScheduleByPointRuleId
		/// <summary>
		/// Get Point Rule Schedule By Point Rule Id
		/// </summary>
		/// <param name="pointRuleId">Point Rule Id</param>
		/// <returns>Point Rule Schedules</returns>
		internal PointRuleScheduleModel[] GetPointRuleScheduleByPointRuleId(string pointRuleId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_POINTRULESCHEDULE_POINT_RULE_ID, pointRuleId}
			};
			var data = base.Get(XML_KEY_NAME, "GetPointRuleScheduleByPointRuleId", input);

			return data.Cast<DataRowView>().Select(item => new PointRuleScheduleModel(item)).ToArray();
		}
		#endregion

		#region ~InsertPointRuleSchedule ポイントルールスケジュール登録
		/// <summary>
		/// ポイントルールスケジュール登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録件数</returns>
		internal int InsertPointRuleSchedule(PointRuleScheduleModel model)
		{
			return base.Exec(XML_KEY_NAME, "InsertPointRuleSchedule", model.DataSource);
		}
		#endregion

		#region ~UpdatePointRuleSchedule ポイントルールスケジュール更新
		/// <summary>
		/// ポイントルールスケジュール更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>更新件数</returns>
		internal int UpdatePointRuleSchedule(PointRuleScheduleModel model)
		{
			return base.Exec(XML_KEY_NAME, "UpdatePointRuleSchedule", model.DataSource);
		}
		#endregion

		#region ~DeletePointRuleSchedule ポイントルールスケジュール削除
		/// <summary>
		/// ポイントルールスケジュール削除
		/// </summary>
		/// <param name="pointRuleScheduleId">ポイントルールスケジュールID</param>
		/// <returns>削除件数</returns>
		internal int DeletePointRuleSchedule(string pointRuleScheduleId)
		{
			return base.Exec(XML_KEY_NAME, "DeletePointRuleSchedule", new Hashtable { { Constants.FIELD_POINTRULESCHEDULE_POINT_RULE_SCHEDULE_ID, pointRuleScheduleId } });
		}
		#endregion

		#region ~UpdatePointRuleScheduleStatus ポイントルールスケジュールステータス更新
		/// <summary>
		/// ポイントルールスケジュールステータス更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>更新件数</returns>
		internal int UpdatePointRuleScheduleStatus(PointRuleScheduleModel model)
		{
			return base.Exec(XML_KEY_NAME, "UpdatePointRuleScheduleStatus", model.DataSource);
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
			var ht = new Hashtable
			{
				{Constants.FIELD_TARGETLIST_TARGET_ID, targetId},
			};
			var dv = base.Get(XML_KEY_NAME, "CheckTargetListUsed", ht);
			return dv.Cast<DataRowView>().Select(drv => new PointRuleScheduleModel(drv)).ToArray();
		}
		#endregion

		#region ~GetHistoriesByGroupNo 履歴グループ番号で取得
		/// <summary>
		/// 履歴グループ番号で取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="historyGroupNo">履歴グループ番号</param>
		/// <returns>ユーザーポイント履歴</returns>
		public UserPointHistoryModel[] GetHistoriesByGroupNo(string userId, int historyGroupNo)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetHistoriesByGroupNo",
				new Hashtable
				{
					{ Constants.FIELD_USERPOINTHISTORY_USER_ID, userId },
					{ Constants.FIELD_USERPOINTHISTORY_HISTORY_GROUP_NO, historyGroupNo },
				});

			return dv.Cast<DataRowView>().Select(drv => new UserPointHistoryModel(drv)).ToArray();
		}
		#endregion

		#region ~IssueHistoryGroupNoForUser 履歴グループ番号採番
		/// <summary>
		/// 履歴グループ番号採番
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>利用可能な履歴グループ番号</returns>
		internal int IssueHistoryGroupNoForUser(string userId)
		{
			var dv = Get(
				XML_KEY_NAME,
				"IssueHistoryGroupNoForUser",
				new Hashtable
				{
					{ Constants.FIELD_USERPOINTHISTORY_USER_ID, userId }
				});

			return (int)dv[0][0];
		}
		#endregion

		#region ~IssuePointKbnNoForUser ポイント枝番を採番
		/// <summary>
		/// ポイント枝番を採番
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		internal int IssuePointKbnNoForUser(string userId)
		{
			var dv = Get(
				XML_KEY_NAME,
				"IssuePointKbnNoForUser",
				new Hashtable
				{
					{ Constants.FIELD_USERPOINT_USER_ID, userId }
				});

			return (int)dv[0][0];
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckUserPointDetailFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckUserPointDetailFields", input, replaces: replaces);
		}

		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, statementName, input, replaces);
			return reader;
		}

		/// <summary>
		/// マスタ取得（Excel出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>DataView</returns>
		public DataView GetMaster(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, statementName, input, replaces: replaces);
			return dv;
		}
		#endregion

		#region +GetUserPointHistoriesOnFront ユーザーポイント履歴情報取得（Front表示用）
		/// <summary>
		/// ユーザーポイント履歴情報取得（Front表示用）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ユーザーポイント履歴情報</returns>
		public UserPointHistoryContainer[] GetUserPointHistoriesOnFront(string userId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_USERPOINTHISTORY_USER_ID, userId },
			};

			var dv = Get(XML_KEY_NAME, "GetUserPointHistoriesOnFront", input);
			var result = dv.Cast<DataRowView>().Select(drv => new UserPointHistoryContainer(drv)).ToArray();
			return result;
		}
		#endregion

		#region ~GetNextHistoryNo ユーザーポイント履歴Noを取得
		/// <summary>
		/// ユーザーポイント履歴Noを取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ユーザー履歴No</returns>
		internal int GetNextHistoryNo(string userId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERCOUPONHISTORY_USER_ID, userId }
			};
			var dv = Get(XML_KEY_NAME, "GetNextHistoryNo", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region ~UpdateUserPointHistoryForIntegration ユーザーポイント履歴更新(ユーザー統合用)
		/// <summary>
		/// ユーザーポイント履歴更新(ユーザー統合用)
		/// </summary>
		/// <param name="userId">ユーザーID(新)</param>
		/// <param name="userIdOld">ユーザーID(旧)</param>
		/// <param name="historyNo">履歴No(新)</param>
		/// <param name="historyNoOld">履歴No(旧)</param>
		/// <param name="historyGroupNo">履歴グループ番号</param>
		/// <returns>更新件数</returns>
		internal int UpdateUserPointHistoryForIntegration(
			string userId,
			string userIdOld,
			int historyNo,
			int historyNoOld,
			int historyGroupNo)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERPOINTHISTORY_USER_ID, userId },
				{ Constants.FIELD_USERPOINTHISTORY_USER_ID + "_old", userIdOld },
				{ Constants.FIELD_USERPOINTHISTORY_HISTORY_NO, historyNo },
				{ Constants.FIELD_USERPOINTHISTORY_HISTORY_NO + "_old", historyNoOld },
				{ Constants.FIELD_USERPOINTHISTORY_HISTORY_GROUP_NO, historyGroupNo },
			};
			var result = Exec(XML_KEY_NAME, "UpdateUserPointHistoryForIntegration", ht);
			return result;
		}
		#endregion

		#region +DeleteUserPointHistory ユーザーポイント履歴削除
		/// <summary>
		/// ユーザーポイント履歴削除
		/// </summary>
		/// <param name="userPointHistory">ユーザーポイント履歴</param>
		/// <returns>件数</returns>
		internal int DeleteUserPointHistory(UserPointHistoryModel userPointHistory)
		{
			var result = Exec(XML_KEY_NAME, "DeleteUserPointHistory", userPointHistory.DataSource);
			return result;
		}
		#endregion
	}
}
