/*
=========================================================================================================
  Module      : インシデントレポートリポジトリ(IncidentReportRepository.cs)
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

namespace w2.App.Common.Cs.Reports
{
	/// <summary>
	/// インシデントレポートリポジトリ
	/// </summary>
	public class IncidentReportRepository : ReportRepositoryBase
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public IncidentReportRepository()
			: base("CsReportIncident")
		{
		}
		#endregion

		#region +GetStatusCount ステータス毎の現在の件数取得
		/// <summary>
		/// ステータス毎の現在の件数取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>結果</returns>
		public DataView GetStatusCount(string deptId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetStatusCount"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENT_DEPT_ID, deptId);

				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetActionCountByTerm 期間内のアクション件数取得
		/// <summary>
		/// 期間内のアクション件数取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="begin">開始日時</param>
		/// <param name="end">終了日時</param>
		/// <returns>結果</returns>
		public DataView GetActionCountByTerm(string deptId, DateTime begin, DateTime end)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetActionCountByTerm"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENT_DEPT_ID, deptId);
				ht.Add("begin", begin);
				ht.Add("end", end);

				var dv = statement.SelectSingleStatement(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetVocReport VOC毎レポート取得
		/// <summary>
		/// VOC毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetVocReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetVocReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetSummaryKbnReport 集計区分毎レポート取得
		/// <summary>
		/// 集計区分毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetSummaryKbnReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetSummaryKbnReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion
	}
}
