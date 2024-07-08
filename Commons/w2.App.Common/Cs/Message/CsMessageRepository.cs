/*
=========================================================================================================
  Module      : メッセージリポジトリ(CsMessageRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Cs.Search;
using w2.Common.Sql;

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージリポジトリ
	/// </summary>
	public class CsMessageRepository : RepositoryBase 
	{
		private const string XML_KEY_NAME = "CsMessage";

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <returns>メッセージ</returns>
		public DataView Get(string deptId, string incidentId, int messageNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGE_INCIDENT_ID, incidentId);
				ht.Add(Constants.FIELD_CSMESSAGE_MESSAGE_NO, messageNo);

				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetCount 件数取得
		/// <summary>
		/// 件数取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>件数</returns>
		public DataView GetCount(string deptId, string operatorId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetCount"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGE_OPERATOR_ID, operatorId);

				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetCountByRequest 件数取得（依頼メッセージ単位）
		/// <summary>
		/// 件数取得（依頼メッセージ単位）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>件数（依頼メッセージ単位）</returns>
		public DataView GetCountByRequest(string deptId, string operatorId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetCountByRequest"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_REQUEST_OPERATOR_ID, operatorId);

				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +CountByRequestItem 件数取得（依頼メッセージアイテム単位）
		/// <summary>
		/// 件数取得（依頼メッセージアイテム単位）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>件数（依頼メッセージアイテム単位）</returns>
		public DataView GetCountByRequestItem(string deptId, string operatorId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetCountByRequestItem"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_APPR_OPERATOR_ID, operatorId);

				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +SearchValidByCreateOperatorAndStatus 作成オペレータとステータスを指定して有効なものを検索
		/// <summary>
		/// 作成オペレータとステータスを指定して有効なものを検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="messageStatus">メッセージステータス</param>
		/// <param name="beginRow">開始インデックス</param>
		/// <param name="endRow">終了インデックス</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="searchParameter">Search parameter</param>
		/// <returns>リスト</returns>
		public DataView SearchValidByCreateOperatorAndStatus(string deptId, string operatorId, string messageStatus, int beginRow, int endRow, string sortField, string sortType, SearchParameter searchParameter)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "SearchMessage"))
			{
				statement.ReplaceStatement("@@ where @@", "[[ SearchValidByCreateOeratorAndStatus_Where ]]");

				// Create statement for sort
				SearchSqlParameter.ReplaceStatementSortForMessage(sortField, sortType, statement);

				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGE_OPERATOR_ID, operatorId);
				ht.Add(Constants.FIELD_CSMESSAGE_MESSAGE_STATUS, messageStatus);
				ht.Add("bgn_row_num", beginRow);
				ht.Add("end_row_num", endRow);

				// Create statement for keyword
				new SearchSqlParameter(searchParameter).ReplaceStatementSearchKeyWord(statement, ht, searchParameter);

				var result = statement.SelectSingleStatement(accessor, ht);
				return result;
			}
		}
		#endregion

		#region +SearchByCreateOperatorAndStatusAndValidFlg 作成オペレータとステータス、有効フラグを指定して検索
		/// <summary>
		/// 作成オペレータとステータス、有効フラグを指定して検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">依頼オペレータID</param>
		/// <param name="messageStatuses">メッセージステータス配列</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="beginRow">開始インデックス</param>
		/// <param name="endRow">終了インデックス</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="isSetExMail">Is set EX_Mail</param>
		/// <param name="searchParameter">Search parameter</param>
		/// <returns>リスト</returns>
		public DataView SearchByCreateOperatorAndStatusAndValidFlg(string deptId, string operatorId, string[] messageStatuses, string validFlg, int beginRow, int endRow, string sortField, string sortType, bool isSetExMail, SearchParameter searchParameter)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "SearchMessage"))
			{
				statement.ReplaceStatement("@@ where @@", "[[ SearchByCreateOeratorAndStatusAndValidFlg_Where ]]");

				// Create statement for sort
				SearchSqlParameter.ReplaceStatementSortForMessage(sortField, sortType, statement, false);

				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGE_OPERATOR_ID, operatorId);
				ht.Add(Constants.FIELD_CSMESSAGE_VALID_FLG, validFlg);
				ht.Add("bgn_row_num", beginRow);
				ht.Add("end_row_num", endRow);

				var paramNameList = new List<string>();
				for (int i = 0; i < messageStatuses.Length; i++)
				{
					string paramName = "p" + i;
					statement.AddInputParameters(paramName, SqlDbType.NVarChar, 30);
					ht.Add(paramName, messageStatuses[i]);
					paramNameList.Add("@" + paramName);
				}

				statement.ReplaceStatement("@@ params @@", string.Join(",", paramNameList.ToArray()));

				// Create statement for keyword
				new SearchSqlParameter(searchParameter).ReplaceStatementSearchKeyWord(statement, ht, searchParameter);

				var result = statement.SelectSingleStatement(accessor, ht);
				return result;
			}
		}
		#endregion

		#region +SearchValidByReplyOperator 回答オペレータを指定して有効なものを検索
		/// <summary>
		/// 回答オペレータを指定して有効なものを検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="replyOperatorId">返信オペレータID</param>
		/// <param name="beginRow">開始インデックス</param>
		/// <param name="endRow">終了インデックス</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="searchParameter">Search parameter</param>
		/// <returns>リスト</returns>
		public DataView SearchValidByReplyOperator(string deptId, string replyOperatorId, int beginRow, int endRow, string sortField, string sortType, SearchParameter searchParameter)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "SearchMessage"))
			{
				statement.ReplaceStatement("@@ where @@", "[[ SearchValidByReplyOperator_Where ]]");

				// Create statement for sort
				SearchSqlParameter.ReplaceStatementSortForMessage(sortField, sortType, statement);

				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGE_REPLY_OPERATOR_ID, replyOperatorId);
				ht.Add("bgn_row_num", beginRow);
				ht.Add("end_row_num", endRow);

				// Create statement for keyword
				new SearchSqlParameter(searchParameter).ReplaceStatementSearchKeyWord(statement, ht, searchParameter);

				var result = statement.SelectSingleStatement(accessor, ht);
				return result;
			}
		}
		#endregion

		#region +SearchValidRequestByApprovalOperatorAndStatus 承認オペレータとステータスを指定して有効な依頼メッセージを検索
		/// <summary>
		/// 承認オペレータとステータスを指定して有効な依頼メッセージを検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="apprOperatorId">承認オペレータID</param>
		/// <param name="messageStatus">メッセージステータス</param>
		/// <param name="beginRow">開始インデックス</param>
		/// <param name="endRow">終了インデックス</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="searchParameter">Search parameter</param>
		/// <returns>リスト</returns>
		public DataView SearchValidRequestByApprovalOperatorAndStatus(string deptId, string apprOperatorId, string messageStatus, int beginRow, int endRow, string sortField, string sortType, SearchParameter searchParameter)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "SearchRequest"))
			{
				statement.ReplaceStatement("@@ where @@", "[[ SearchValidRequestByApprovalOeratorAndStatus_Where ]]");

				// Create statement for sort
				SearchSqlParameter.ReplaceStatementSortForMessage(sortField, sortType, statement);

				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGE_MESSAGE_STATUS, messageStatus);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_APPR_OPERATOR_ID, apprOperatorId);
				ht.Add("bgn_row_num", beginRow);
				ht.Add("end_row_num", endRow);

				// Create statement for keyword
				new SearchSqlParameter(searchParameter).ReplaceStatementSearchKeyWord(statement, ht, searchParameter);

				var result = statement.SelectSingleStatement(accessor, ht);
				return result;
			}
		}
		#endregion

		#region +SearchValidRequestByRequestOperator 依頼オペレータを指定して有効な依頼メッセージを検索
		/// <summary>
		/// 依頼オペレータを指定して有効な依頼メッセージを検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">依頼オペレータID</param>
		/// <param name="messageStatus">メッセージステータス</param>
		/// <param name="beginRow">開始インデックス</param>
		/// <param name="endRow">終了インデックス</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="searchParameter">Search parameter</param>
		/// <returns>リスト</returns>
		public DataView SearchValidRequestByRequestOperator(string deptId, string operatorId, string[] messageStatus, int beginRow, int endRow, string sortField, string sortType, SearchParameter searchParameter)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "SearchRequest"))
			{
				statement.ReplaceStatement("@@ where @@", "[[ SearchValidRequestByRequestOerator_Where ]]");

				// Replace statement for sort
				SearchSqlParameter.ReplaceStatementSortForMessage(sortField, sortType, statement);

				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGE_OPERATOR_ID, operatorId);
				ht.Add("bgn_row_num", beginRow);
				ht.Add("end_row_num", endRow);

				List<string> statusParamNames = new List<string>();
				for (int i = 0; i < messageStatus.Length; i++)
				{
					statusParamNames.Add("@" + Constants.FIELD_CSMESSAGE_MESSAGE_STATUS + i.ToString());
					statement.AddInputParameters(Constants.FIELD_CSMESSAGE_MESSAGE_STATUS + i.ToString(), SqlDbType.NVarChar, 10);
					ht.Add(Constants.FIELD_CSMESSAGE_MESSAGE_STATUS + i.ToString(), messageStatus[i]);
				}
				statement.ReplaceStatement("@@ message_statuses @@", string.Join(",", statusParamNames.ToArray()));

				// Create statement for keyword
				new SearchSqlParameter(searchParameter).ReplaceStatementSearchKeyWord(statement, ht, searchParameter);

				var result = statement.SelectSingleStatement(accessor, ht);
				return result;
			}
		}
		#endregion

		#region +SearchAdvanced メッセージを詳細検索
		/// <summary>
		/// メッセージを詳細検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="sp">検索パラメータ</param>
		/// <param name="beginRow">開始行</param>
		/// <param name="endRow">終了行</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <param name="byIncident">インシデント単位で取得するか</param>
		/// <returns>検索結果の一覧</returns>
		public DataView SearchAdvanced(string deptId, SearchParameter sp, int beginRow, int endRow, string sortField, string sortType, bool byIncident = false)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, byIncident ? "SearchAdvancedByIncident" : "SearchAdvanced"))
			{
				// 追加条件があればパフォーマンス向上のためリテラルSQLを採用する
				// （大量データを検索する際、パラメタライズクエリではパフォーマンスが落ちてしまうため）
				statement.UseLiteralSql = sp.HasAdditionalConditions;

				// 詳細検索条件の組み立て
				SearchSqlParameter searchSqlParam = new SearchSqlParameter(sp);
				searchSqlParam.InputValues.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				foreach (SqlStatement.SqlParam param in searchSqlParam.SqlParams)
				{
					statement.AddInputParameters(param.Name, param.Type, param.Size);
				}
				statement.ReplaceStatement("@@ keywords_where @@", searchSqlParam.WhereStatement.ToString());

				// Replace statement for sort
				if (byIncident)
				{
					SearchSqlParameter.ReplaceStatementSortForIncident(sortField, sortType, statement);
				}
				else
				{
					SearchSqlParameter.ReplaceStatementSortForMessage(sortField, sortType, statement);
				}

				searchSqlParam.InputValues.Add("bgn_row_num", beginRow);
				searchSqlParam.InputValues.Add("end_row_num", endRow);

				return statement.SelectSingleStatement(accessor, searchSqlParam.InputValues);
			}
		}
		#endregion

		#region +GetAll 全て取得
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <returns>メッセージリスト情報</returns>
		public DataView GetAll(string deptId, string incidentId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGE_INCIDENT_ID, incidentId);

				// Replace statement sort for message default
				statement.ReplaceStatement("@@ orderby @@", "ORDER BY message_no DESC");

				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		/// <summary>
		/// 全て取得 (Sort)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="sortField">Sort field</param>
		/// <param name="sortType">Sort type</param>
		/// <returns>メッセージリスト情報</returns>
		public DataView GetAll(string deptId, string incidentId, string sortField, string sortType)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				input.Add(Constants.FIELD_CSMESSAGE_INCIDENT_ID, incidentId);

				// Replace statement sort for message
				SearchSqlParameter.ReplaceStatementSortForMessage(sortField, sortType, statement, false);

				return statement.SelectSingleStatementWithOC(accessor, input);
			}
		}
		#endregion

		#region +GetLastMessageByIncidentIds 複数インシデントIDでインシデント最後のメッセージを取得
		/// <summary>
		/// 複数インシデントIDでインシデント最後のメッセージを取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentIdList">インシデントIDリスト</param>
		/// <returns>インシデント最後のメッセージリスト</returns>
		public CsMessageModel[] GetLastMessageByIncidentIds(string deptId, string[] incidentIdList)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement(XML_KEY_NAME, "GetLastMessageByIncidentIds"))
			{
				var incidentIds = string.Join(",", incidentIdList.Select(id => string.Format("'{0}'", id)));
				statement.ReplaceStatement("@@ incident_ids @@", incidentIds);

				var searchParam = new Hashtable
				{
					{ Constants.FIELD_CSMESSAGE_DEPT_ID, deptId }
				};

				var result = statement.SelectSingleStatementWithOC(accessor, searchParam);
				return result.Cast<DataRowView>().Select(item => new CsMessageModel(item)).ToArray();
			}
		}
		#endregion

		#region +GetAllReceiveMail
		/// <summary>
		/// Get all receive mail
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>メッセージリスト情報</returns>
		public DataView GetAllReceiveMail(string deptId)
		{
			var input = new Hashtable() { {Constants.FIELD_CSMESSAGE_DEPT_ID, deptId} };

			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement(XML_KEY_NAME, "GetAllReceiveMail"))
			{
				return statement.SelectSingleStatementWithOC(accessor, input);
			}
		}
		#endregion

		#region +GetRegisterMessageNo 登録用のメッセージNO取得
		/// <summary>
		/// 登録用のメッセージNO取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int GetRegisterMessageNo(string deptId, string incidentId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetRegisterMessageNo"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGE_INCIDENT_ID, incidentId);
				var result = (int)statement.SelectSingleStatement(accessor, ht)[0][0];
				return result;
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
		public void Update(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				Update(ht, accessor);
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
				int updated = statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +UpdateMessageStatus メッセージステータス更新
		/// <summary>
		/// メッセージステータス更新
		/// </summary>
		/// <param name="ht">データ</param>
		public void UpdateMessageStatus(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateMessageStatus"))
			{
				statement.ExecStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +UpdateIncidentId インシデントID更新
		/// <summary>
		/// インシデントID更新
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="beforeIncidentId">更新前インシデントID</param>
		/// <param name="beforeMessegeNo">更新前メッセージNO</param>
		/// <param name="afterIncidentId">更新後インシデントID</param>
		/// <param name="afterMessegeNo">更新後メッセージNO</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateIncidentId(string deptId, string beforeIncidentId, int beforeMessegeNo, string afterIncidentId, int afterMessegeNo, string lastChanged, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateIncidentId"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGE_INCIDENT_ID + "_before", beforeIncidentId);
				ht.Add(Constants.FIELD_CSMESSAGE_MESSAGE_NO + "_before", beforeMessegeNo);
				ht.Add(Constants.FIELD_CSMESSAGE_INCIDENT_ID + "_after", afterIncidentId);
				ht.Add(Constants.FIELD_CSMESSAGE_MESSAGE_NO + "_after", afterMessegeNo);
				ht.Add(Constants.FIELD_CSMESSAGE_LAST_CHANGED, lastChanged);

				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +UpdateValidFlg 有効フラグ更新
		/// <summary>
		/// 有効フラグ更新
		/// </summary>
		/// <param name="ht">データ</param>
		public void UpdateValidFlg(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateValidFlg"))
			{
				statement.ExecStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		public void Delete(string deptId, string incidentId, int messageNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				Delete(deptId, incidentId, messageNo, accessor);
			}
		}
		#endregion
		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string deptId, string incidentId, int messageNo, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGE_INCIDENT_ID, incidentId);
				ht.Add(Constants.FIELD_CSMESSAGE_MESSAGE_NO, messageNo);

				int updated = statement.ExecStatement(accessor, ht);
			}
		}
		#endregion
	}
}
