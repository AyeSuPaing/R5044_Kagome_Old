/*
=========================================================================================================
  Module      : メール送信元リポジトリ(CsMailFromRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using w2.Common.Sql;

namespace w2.App.Common.Cs.MailFrom
{
	public class CsMailFromRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsMailFrom";

		#region +Search 一覧取得
		/// <summary>
		/// 一覧取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="beginRow">開始行</param>
		/// <param name="endRow">終了行</param>
		/// <returns>メール送信元一覧</returns>
		public DataView Search(string deptId, int beginRow, int endRow)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Search"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMAILFROM_DEPT_ID, deptId);
				ht.Add("bgn_row_num", beginRow);
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
		/// <param name="fromId">メール送信元ID</param>
		/// <returns>メール送信元情報</returns>
		public DataView Get(string deptId, string fromId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMAILFROM_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMAILFROM_MAIL_FROM_ID, fromId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetValidAll 有効なものを取得
		/// <summary>
		/// 有効なものを取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>メール送信元一覧</returns>
		public DataView GetValidAll(string deptId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetValidAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMAILFROM_DEPT_ID, deptId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="ht">登録用データ</param>
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
		/// <param name="ht">更新用データ</param>
		public void Update(Hashtable ht)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Update"))
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
		/// <param name="fromId">メール送信元ID</param>
		public void Delete(string deptId, string fromId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMAILFROM_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMAILFROM_MAIL_FROM_ID, fromId);
				statement.ExecStatementWithOC(accessor, ht);
			}
		}
		#endregion
	}
}
