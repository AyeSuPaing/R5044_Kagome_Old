/*
=========================================================================================================
  Module      : インシデントVOCリポジトリ(CsIncidentVocRepository.cs)
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

namespace w2.App.Common.Cs.IncidentVoc
{
	/// <summary>
	/// インシデントVOCリポジトリ
	/// </summary>
	public class CsIncidentVocRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsIncidentVoc";

		#region +Search 一覧取得
		/// <summary>
		/// 一覧取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="bgnRow">開始行番号</param>
		/// <param name="endRow">終了行番号</param>
		/// <returns>VOCデータ</returns>
		public DataView Search(string deptId, int bgnRow, int endRow)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Search"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENTVOC_DEPT_ID, deptId);
				ht.Add("bgn_row_num", bgnRow);
				ht.Add("end_row_num", endRow);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="vocId">VOCID</param>
		/// <returns>VOCデータ</returns>
		public DataView Get(string deptId, string vocId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENTVOC_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSINCIDENTVOC_VOC_ID, vocId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetValidAll 有効なものすべて取得
		/// <summary>
		/// 有効なものすべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>VOCデータ</returns>
		public DataView GetValidAll(string deptId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetValidAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENTVOC_DEPT_ID, deptId);
			    return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="input">登録用データ</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Register(Hashtable input, SqlAccessor accessor)
		{
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "Register"))
			{
				sqlStatement.ExecStatement(accessor, input);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="input">更新用データ</param>
		public void Update(Hashtable input)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "Update"))
			{
				sqlStatement.ExecStatementWithOC(sqlAccessor, input);
			}
		}
		#endregion

		#region +CheckDeletable 削除チェック
		/// <summary>
		/// 削除チェック
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="vocId">VOCID</param>
		/// <returns>削除可能かどうか</returns>
		public bool CheckDeletable(string deptId, string vocId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "CheckDeletable"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENTVOC_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSINCIDENTVOC_VOC_ID, vocId);
				
				DataView dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return ((int)dv[0]["count"] == 0);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="vocId">VOCID</param>
		public void Delete(string deptId, string vocId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENTVOC_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSINCIDENTVOC_VOC_ID, vocId);
				sqlStatement.ExecStatementWithOC(sqlAccessor, ht);
			}
		}
		#endregion
	}
}
