/*
=========================================================================================================
  Module      : メッセージ依頼リポジトリ(CsMessageRequestRepository.cs)
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
	/// メッセージ依頼リポジトリ
	/// </summary>
	public class CsMessageRequestRepository : RepositoryBase 
	{
		private const string XML_KEY_NAME = "CsMessageRequest";

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="requestNo">リクエストNO</param>
		/// <returns>データ</returns>
		public DataView Get(string deptId, string incidentId, int messageNo, int requestNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
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

		#region +GetDraft 下書き依頼取得
		/// <summary>
		/// 下書き依頼取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <returns>データ</returns>
		public DataView GetDraft(string deptId, string incidentId, int messageNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetDraft"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_INCIDENT_ID, incidentId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_MESSAGE_NO, messageNo);

				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetAll メッセージのリクエストを全て取得
		/// <summary>
		/// メッセージのリクエストを全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <returns>データ</returns>
		public DataView GetAll(string deptId, string incidentId, int messageNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_INCIDENT_ID, incidentId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_MESSAGE_NO, messageNo);

				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetRegisterRequestNo 登録用のメッセージ依頼NO取得
		/// <summary>
		/// 登録用のメッセージ依頼NO取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <return>メッセージ依頼NO</return>
		public int GetRegisterRequestNo(string deptId, string incidentId, int messageNo, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetRegisterRequestNo"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_INCIDENT_ID, incidentId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_MESSAGE_NO, messageNo);
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

		#region +UpdateWorkingOperator 作業中オペレータ更新
		/// <summary>
		/// 作業中オペレータ更新
		/// </summary>
		/// <param name="ht">データ</param>
		/// <returns>更新件数</returns>
		public int UpdateWorkingOperator(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateWorkingOperator"))
			{
				var result = statement.ExecStatementWithOC(accessor, ht);
				return result;
			}
		}
		#endregion

		#region +UpdateRequestStatus 依頼ステータス更新
		/// <summary>
		/// 依頼ステータス更新
		/// </summary>
		/// <param name="ht">データ</param>
		/// <returns>更新件数</returns>
		public int UpdateRequestStatus(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateRequestStatus"))
			{
				var result = statement.ExecStatementWithOC(accessor, ht);
				return result;
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
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_INCIDENT_ID + "_before", beforeIncidentId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_MESSAGE_NO + "_before", beforeMessegeNo);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_INCIDENT_ID + "_after", afterIncidentId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_MESSAGE_NO + "_after", afterMessegeNo);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_LAST_CHANGED, lastChanged);

				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +Delete メッセージリクエスト削除
		/// <summary>
		/// メッセージリクエスト削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="requestNo">リクエストNO</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string deptId, string incidentId, int messageNo, int requestNo, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_INCIDENT_ID, incidentId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_MESSAGE_NO, messageNo);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_REQUEST_NO, requestNo);

				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +DeleteAll メッセージのリクエストすべて削除
		/// <summary>
		/// メッセージのリクエストすべて削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="messageNo">メッセージNO</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteAll(string deptId, string incidentId, int messageNo, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "DeleteAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_INCIDENT_ID, incidentId);
				ht.Add(Constants.FIELD_CSMESSAGEREQUEST_MESSAGE_NO, messageNo);

				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion
	}
}
