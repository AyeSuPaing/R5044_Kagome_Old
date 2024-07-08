/*
=========================================================================================================
  Module      : 基底レポートリポジトリ(ReportRepositoryBase.cs)
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
	/// 基底レポートリポジトリ
	/// </summary>
	public abstract class ReportRepositoryBase : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		protected readonly string XML_KEY_NAME = "";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="xmlKeyName">XMLキー名</param>
		public ReportRepositoryBase(string xmlKeyName)
		{
			XML_KEY_NAME = xmlKeyName;
		}

		#region +GetGroupReport グループ毎レポート取得
		/// <summary>
		/// グループ毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetGroupReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetGroupReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetOperatorReport オペレータ毎レポート取得
		/// <summary>
		/// オペレータ毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetOperatorReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetOperatorReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetGroupOperatorReport グループ-オペレータ毎レポート取得
		/// <summary>
		/// グループ-オペレータ毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetGroupOperatorReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetGroupOperatorReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetCategoryReport カテゴリ毎レポート取得
		/// <summary>
		/// カテゴリ毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetCategoryReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetCategoryReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetMonthReport 月毎レポート取得
		/// <summary>
		/// 月毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetMonthReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetMonthReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetMonthDayReport 月-日毎レポート取得
		/// <summary>
		/// 月-日毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetMonthDayReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetMonthDayReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion
		
		#region +GetWeekdayReport 曜日毎レポート取得
		/// <summary>
		/// 曜日毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetWeekdayReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetWeekdayReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetTimeReport 時間帯毎レポート取得
		/// <summary>
		/// 時間帯毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetTimeReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetTimeReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetWeekdayTimeReport 曜日-時間帯毎レポート取得
		/// <summary>
		/// 曜日-時間帯毎レポート取得
		/// </summary>
		/// <param name="ht">パラメタ</param>
		/// <returns>結果</returns>
		public DataView GetWeekdayTimeReport(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetWeekdayTimeReport"))
			{
				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion
	}
}
