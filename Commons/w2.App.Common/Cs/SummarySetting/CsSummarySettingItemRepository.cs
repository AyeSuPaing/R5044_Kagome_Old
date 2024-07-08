/*
=========================================================================================================
  Module      : 集計区分アイテムリポジトリ(CsSummarySettingItemRepository.cs)
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

namespace w2.App.Common.Cs.SummarySetting
{
	/// <summary>
	/// 集計区分アイテムリポジトリ
	/// </summary>
	public class CsSummarySettingItemRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsSummarySettingItem";

		#region +GetAllBySummarySettingNo 集計区分内アイテム全て取得
		/// <summary>
		/// 集計区分内アイテム全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="summarySettingNo">集計区分番号</param>
		/// <returns>集計区分アイテムデータ</returns>
		public DataView GetAllBySummarySettingNo(string deptId, int summarySettingNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAllBySummarySettingNo"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSSUMMARYSETTINGITEM_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSSUMMARYSETTINGITEM_SUMMARY_SETTING_NO, summarySettingNo);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetAll 全て取得
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>集計区分アイテムデータ</returns>
		public DataView GetAll(string deptId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_CSSUMMARYSETTINGITEM_DEPT_ID, deptId);
				return statement.SelectSingleStatementWithOC(accessor, input);
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

		#region +DeleteAll 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="ht">情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteAll(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "DeleteAll"))
			{
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion
	}
}
