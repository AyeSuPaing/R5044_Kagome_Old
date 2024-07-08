/*
=========================================================================================================
  Module      : インシデント集計区分値リポジトリ(CsIncidentSummaryValueRepository.cs)
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

namespace w2.App.Common.Cs.Incident
{
	/// <summary>
	/// インシデント集計区分値リポジトリ
	/// </summary>
	public class CsIncidentSummaryValueRepository : RepositoryBase 
	{
		private const string XML_KEY_NAME = "CsIncidentSummaryValue";

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="summaryNo">集計区分NO</param>
		/// <param name="asccesor">SQLアクセサ</param>
		/// <returns>データ</returns>
		public DataView Get(string deptId, string incidentId, int summaryNo, SqlAccessor asccesor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_CSINCIDENTSUMMARYVALUE_DEPT_ID, deptId);
				input.Add(Constants.FIELD_CSINCIDENTSUMMARYVALUE_INCIDENT_ID, incidentId);
				input.Add(Constants.FIELD_CSINCIDENTSUMMARYVALUE_SUMMARY_NO, summaryNo);

				return statement.SelectSingleStatement(asccesor, input);
			}
		}
		#endregion

		#region +GetList リスト取得
		/// <summary>
		/// リスト取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <returns>データ</returns>
		public DataView GetList(string deptId, string incidentId)
		{
			using (SqlAccessor asccesor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetList"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_CSMESSAGE_DEPT_ID, deptId);
				input.Add(Constants.FIELD_CSMESSAGE_INCIDENT_ID, incidentId);

				return statement.SelectSingleStatementWithOC(asccesor, input);
			}
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="ht">データ</param>
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
		/// <param name="ht">データ</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Update(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Update"))
			{
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +DeleteAll 全て削除
		/// <summary>
		/// 全て削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="incidentId">インシデントID</param>
		/// <param name="asccesor">SQLアクセサ</param>
		public void DeleteAll(string deptId, string incidentId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "DeleteAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENTSUMMARYVALUE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSINCIDENTSUMMARYVALUE_INCIDENT_ID, incidentId);

				int updated = statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

	}
}
