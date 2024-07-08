/*
=========================================================================================================
  Module      : インシデントリポジトリ(CsIncidentRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using System.Data;
using w2.Common.Sql;
using w2.Common.Util;
using System.Collections;
using w2.App.Common.Cs.Search;
using System.Collections.Generic;

namespace w2.App.Common.Cs.Incident
{
	/// <summary>
	/// インシデントリポジトリ
	/// </summary>
	public class CsIncidentRepository : RepositoryBase
	{
		private const string XML_KEY_NAME = "CsIncident";

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="statuses">ステータス配列</param>
		/// <param name="categoryId">インシデントカテゴリID</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="tagrgetOperatorGroupType">対象はオペレータかグループか（1:オペレータ、2-1:グループ、2-2:グループだがグループ未振り分けも対象、0:すべて）</param>
		/// <param name="beginRow">開始インデックス</param>
		/// <param name="endRow">終了インデックス</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort Type</param>
		/// <param name="csGroupId">グループID</param>
		/// <param name="searchParameter">Search parameter</param>
		/// <returns>インシデント配列</returns>
		public DataView Search(
			string deptId,
			string operatorId,
			string[] statuses,
			string categoryId,
			string validFlg,
			string tagrgetOperatorGroupType,
			int beginRow,
			int endRow,
			string sortField,
			string sortType,
			string csGroupId,
			SearchParameter searchParameter)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Search"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENT_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSINCIDENT_OPERATOR_ID, operatorId);

				// Create statement for keyword
				if ((searchParameter != null) && (searchParameter.Keyword != ""))
				{
					statement.ReplaceStatement("@@ keywords_search @@", "[[ SearchMessageKeyword ]]");
					statement.ReplaceStatement("@@ table_temp_for_fuzzy_search @@", "[[ InsertWhereLikeStringForIncident ]]");
					ht.Add("keyword_likestring", string.Format("%{0}%", StringUtility.SqlLikeStringSharpEscape(searchParameter.Keyword)));
				}
				else
				{
					statement.ReplaceStatement("@@ keywords_search @@", string.Empty);
					statement.ReplaceStatement("@@ table_temp_for_fuzzy_search @@", string.Empty);
				}

				// Create statement for status
				string statusParams = string.Join(",", (from i in Enumerable.Range(0, statuses.Length) select "@" + Constants.FIELD_CSINCIDENT_STATUS + i.ToString()));
				statement.ReplaceStatement("@@ statuses @@", statusParams);
				for (int i = 0; i < statuses.Length; i++)
				{
					statement.AddInputParameters(Constants.FIELD_CSINCIDENT_STATUS + i.ToString(), SqlDbType.NVarChar, 10);
					ht.Add(Constants.FIELD_CSINCIDENT_STATUS + i.ToString(), statuses[i]);
				}

				// Replace statement sort for incident
				SearchSqlParameter.ReplaceStatementSortForIncident(sortField, sortType, statement);

				ht.Add(Constants.FIELD_CSINCIDENT_INCIDENT_CATEGORY_ID, categoryId);
				ht.Add(Constants.FIELD_CSINCIDENT_INCIDENT_CATEGORY_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(categoryId));
				ht.Add(Constants.FIELD_CSINCIDENT_VALID_FLG, validFlg);
				ht.Add("target_operator_group_type", tagrgetOperatorGroupType);
				ht.Add("bgn_row_num", beginRow);
				ht.Add("end_row_num", endRow);
				ht.Add(Constants.FIELD_CSINCIDENT_CS_GROUP_ID, csGroupId);
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <returns>インシデント情報</returns>
		public DataView Get(string deptId, string incidentId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENT_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSINCIDENT_INCIDENT_ID, incidentId);

				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetByMessageId メッセージIDを指定して取得
		/// <summary>
		/// メッセージIDを指定して取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="messageId">メッセージID</param>
		/// <returns>インシデント情報</returns>
		public DataView GetByMessageId(string deptId, string messageId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetByMessageId"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEMAIL_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEMAIL_MESSAGE_ID, messageId);

				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +CountWarning 滞留警告メール送信用に件数集計
		/// <summary>
		/// 滞留警告メール送信用に件数集計
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="warningLimitDays">滞留日数</param>
		/// <returns>個人/グループタスクの滞留件数</returns>
		public DataSet CountWarning(string deptId, int warningLimitDays)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "CountWarning"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENT_DEPT_ID, deptId);
				ht.Add("day_count", warningLimitDays * -1);
				var ds = statement.SelectStatementWithOC(accessor, ht);
				return ds;
			}
		}
		#endregion

		#region +UpdateLockStatusForLock ロック取得用ロックステータス更新
		/// <summary>
		/// ロック取得用ロックステータス更新
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="lockStatusBefore">変更前ロックステータス</param>
		/// <param name="lockStatusAfter">変更後ロックステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>成功したか</returns>
		public bool UpdateLockStatusForLock(string deptId, string incidentId, string operatorId, string lockStatusBefore, string lockStatusAfter, string lastChanged)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateLockStatusForLock"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENT_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSINCIDENT_INCIDENT_ID, incidentId);
				ht.Add(Constants.FIELD_CSINCIDENT_LOCK_STATUS, lockStatusAfter);
				ht.Add("lock_status_before", lockStatusBefore);
				ht.Add(Constants.FIELD_CSINCIDENT_LOCK_OPERATOR_ID, operatorId);
				ht.Add(Constants.FIELD_CSINCIDENT_LAST_CHANGED, lastChanged);

				var updated = statement.ExecStatementWithOC(accessor, ht);
				return (updated > 0);
			}
		}
		#endregion

		#region +UpdateLockStatusForUnlock ロック解除用ロックステータス更新
		/// <summary>
		/// ロック解除用ロックステータス更新
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>成功したか</returns>
		public bool UpdateLockStatusForUnlock(string deptId, string incidentId, string lastChanged)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateLockStatusForUnlock"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENT_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSINCIDENT_INCIDENT_ID, incidentId);
				ht.Add(Constants.FIELD_CSINCIDENT_LAST_CHANGED, lastChanged);

				var updated = statement.ExecStatementWithOC(accessor, ht);
				return (updated > 0);
			}
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="ht">情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Register(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Register"))
			{
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="ht">情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Update(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Update"))
			{
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +UpdateValidFlg 有効フラグ更新
		/// <summary>
		/// 有効フラグ更新
		/// </summary>
		/// <param name="ht">情報</param>
		public void UpdateValidFlg(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateValidFlg"))
			{
				statement.ExecStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +UpdateStatus ステータス更新
		/// <summary>
		/// ステータス更新
		/// </summary>
		/// <param name="ht">情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateStatus(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateStatus"))
			{
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +UpdateLastReceivedDate 最終受信日時更新
		/// <summary>
		/// 最終受信日時更新
		/// </summary>
		/// <param name="ht">情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateLastReceivedDate(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateLastReceivedDate"))
			{
				statement.ExecStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +UpdateLastReceivedDate 最終送信日時更新
		/// <summary>
		/// 最終送信日時更新
		/// </summary>
		/// <param name="ht">情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateLastSendDate(Hashtable ht)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement(XML_KEY_NAME, "UpdateLastSendDate"))
			{
				statement.ExecStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +UpdateCompleteDate 対応完了日更新
		/// <summary>
		/// 対応完了日更新
		/// </summary>
		/// <param name="ht">情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateCompleteDate(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateCompleteDate"))
			{
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +UpdateIncidentByMailAssignSetting
		/// <summary>
		/// Update incident by mail assign setting
		/// </summary>
		/// <param name="input">Input</param>
		public void UpdateIncidentByMailAssignSetting(Hashtable input)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement(XML_KEY_NAME, "UpdateIncidentByMailAssignSetting"))
			{
				if (string.IsNullOrEmpty((string)input["status"]))
				{
					statement.ReplaceStatement("w2_CsIncident.status = @status,", "");
				}
				statement.ExecStatementWithOC(accessor, input);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string deptId, string incidentId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGE_INCIDENT_ID, incidentId);

				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion
	}
}
