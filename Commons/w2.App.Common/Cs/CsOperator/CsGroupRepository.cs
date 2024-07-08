/*
=========================================================================================================
  Module      : CSグループリポジトリ(CsGroupRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using w2.Common.Sql;

namespace w2.App.Common.Cs.CsOperator
{
	public class CsGroupRepository : RepositoryBase 
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsGroup";

		#region +GetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>CSグループ情報</returns>
		public DataView GetAll(string deptId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMAILASSIGNSETTING_DEPT_ID, deptId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="groupId">CSグループID</param>
		/// <returns>CSグループ情報</returns>
		public DataView Get(string deptId, string groupId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSGROUP_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSGROUP_CS_GROUP_ID, groupId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetValidAll 有効なグループ取得
		/// <summary>
		/// 有効なグループ取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>CSグループ情報</returns>
		public DataView GetValidAll(string deptId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetValidAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMAILASSIGNSETTING_DEPT_ID, deptId);
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
		/// <param name="accessor">SQLアクセサ</param>
		public void Update(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Update"))
			{
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="groupId">CSグループID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string deptId, string groupId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSGROUP_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSGROUP_CS_GROUP_ID, groupId);
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion
	}
}
