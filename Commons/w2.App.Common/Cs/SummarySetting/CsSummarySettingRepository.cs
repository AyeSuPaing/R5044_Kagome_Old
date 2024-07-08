/*
=========================================================================================================
  Module      : 集計区分リポジトリ(CsSummarySettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using w2.Common.Sql;

namespace w2.App.Common.Cs.SummarySetting
{
	/// <summary>
	/// 集計区分リポジトリ
	/// </summary>
	public class CsSummarySettingRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsSummarySetting";

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>集計区分データ</returns>
		public DataView Search(string deptId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement("CsSummarySetting", "Search"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSSUMMARYSETTING_DEPT_ID, deptId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="summarySettingNo">集計区分番号</param>
		/// <returns>集計区分データ</returns>
		public DataView Get(string deptId, int summarySettingNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement("CsSummarySetting", "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSSUMMARYSETTING_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSSUMMARYSETTING_SUMMARY_SETTING_NO, summarySettingNo);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetAll 全て取得
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>集計区分データ</returns>
		public DataView GetAll(string deptId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement SqlStatement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_CSSUMMARYSETTING_DEPT_ID, deptId);
				return SqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
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
	}
}
