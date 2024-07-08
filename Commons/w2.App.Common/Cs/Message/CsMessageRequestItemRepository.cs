/*
=========================================================================================================
  Module      : メッセージ依頼アイテムリポジトリ(CsMessageRequestItemRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Sql;

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージ依頼アイテムリポジトリ
	/// </summary>
	public class CsMessageRequestItemRepository : RepositoryBase 
	{
		private const string XML_KEY_NAME = "CsMessageRequestItem";

		#region +GetAll すべて取得
		/// <summary>
		/// すべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="requestNo">リクエストNO</param>
		/// <returns>リスト</returns>
		public DataView GetAll(string deptId, string incidentId, int messageNo, int requestNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_INCIDENT_ID, incidentId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_MESSAGE_NO, messageNo);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_REQUEST_NO, requestNo);

				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetByApprovalOeratorAndResultStatus 承認オペレータと結果から依頼アイテム取得
		/// <summary>
		/// 承認オペレータと結果から依頼アイテム取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">承認オペレータID</param>
		/// <param name="resultStatus">結果ステータス</param>
		/// <returns>リスト</returns>
		public DataView GetByApprovalOeratorAndResultStatus(string deptId, string operatorId, string resultStatus)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetByApprovalOeratorAndResultStatus"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_APPR_OPERATOR_ID, operatorId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_RESULT_STATUS, resultStatus);

				var result = statement.SelectSingleStatement(accessor, ht);
				return result;
			}
		}
		#endregion

		#region +GetRegisterBranchNo 登録用のメッセージ依頼NO取得
		/// <summary>
		/// 登録用のメッセージ依頼枝番取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="requestNo">依頼NO</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int GetRegisterBranchNo(string deptId, string incidentId, int messageNo, int requestNo, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetRegisterBranchNo"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_INCIDENT_ID, incidentId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_MESSAGE_NO, messageNo);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_REQUEST_NO, requestNo);
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

		#region +UpdateResult 結果更新
		/// <summary>
		/// 結果更新
		/// </summary>
		/// <param name="model">データ</param>
		public void UpdateResult(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateResult"))
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

		#region +UpdateIncidentIdAll インシデントIDすべて更新
		/// <summary>
		/// インシデントIDすべて更新
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="beforeIncidentId">更新前インシデントID</param>
		/// <param name="beforeMessegeNo">更新前メッセージNO</param>
		/// <param name="afterIncidentId">更新後インシデントID</param>
		/// <param name="afterMessegeNo">更新後メッセージNO</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateIncidentIdAll(string deptId, string beforeIncidentId, int beforeMessegeNo, string afterIncidentId, int afterMessegeNo, string lastChanged, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateIncidentIdAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_INCIDENT_ID + "_before", beforeIncidentId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_MESSAGE_NO + "_before", beforeMessegeNo);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_INCIDENT_ID + "_after", afterIncidentId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_MESSAGE_NO + "_after", afterMessegeNo);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_LAST_CHANGED, lastChanged);

				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +DeleteAllMessageRequestItems メッセージのリクエストアイテムすべて削除
		/// <summary>
		/// メッセージのリクエストアイテムすべて削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteAllMessageRequestItems(string deptId, string incidentId, int messageNo, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "DeleteAllMessageRequestItems"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_INCIDENT_ID, incidentId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_MESSAGE_NO, messageNo);

				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +DeleteAll すべて削除
		/// <summary>
		/// すべて削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="requestNo">リクエストNO</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteAll(string deptId, string incidentId, int messageNo, int requestNo, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "DeleteAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_INCIDENT_ID, incidentId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_MESSAGE_NO, messageNo);
				ht.Add(Constants.FIELD_CSMESSAGEREQUESTITEM_REQUEST_NO, requestNo);

				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion
		
	}
}
