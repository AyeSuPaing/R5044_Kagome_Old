/*
=========================================================================================================
  Module      : CS業務フローレポートリポジトリ(CsWorkflowReportRepository.cs)
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
	/// CS業務フローレポートリポジトリ
	/// </summary>
	public class CsWorkflowReportRepository
	{
		private const string XML_KEY_NAME = "CsReportWorkflow";

		#region +GetOperatorRequestReport オペレータ別依頼レポート取得
		/// <summary>
		/// オペレータ別依頼レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetOperatorRequestReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetOperatorRequestReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetOperatorRequestCancelledReport オペレータ別依頼取り下げ済レポート取得
		/// <summary>
		/// オペレータ別依頼取り下げ済レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetOperatorRequestCancelledReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetOperatorRequestCancelledReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetOperatorRequestResultReport オペレータ別依頼結果レポート取得
		/// <summary>
		/// オペレータ別依頼結果レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetOperatorRequestResultReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetOperatorRequestResultReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion
	}
}
