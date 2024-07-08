/*
=========================================================================================================
  Module      : CSオペレータ所属グループリポジトリ(CsOperatorGroupRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using w2.Common.Sql;

namespace w2.App.Common.Cs.CsOperator
{
	public class CsOperatorGroupRepository : RepositoryBase 
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsOperatorGroup";

		#region +GetAll 全て取得
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>全ての組み合わせ</returns>
		public DataView GetAll(string deptId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSGROUP_DEPT_ID, deptId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetGroups グループ取得（オペレータ指定）
		/// <summary>
		/// グループ取得（オペレータ指定）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>グループリスト</returns>
		public DataView GetGroups(string deptId, string operatorId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetGroups"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSGROUP_DEPT_ID, deptId);
				ht.Add(Database.Common.Constants.FIELD_CSOPERATORGROUP_OPERATOR_ID, operatorId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetOperators オペレータ取得（グループ指定）
		/// <summary>
		/// オペレータ取得（グループ指定）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="groupId">グループID</param>
		/// <returns>オペレータリスト</returns>
		public DataView GetOperators(string deptId, string groupId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetOperators"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSGROUP_DEPT_ID, deptId);
				ht.Add(Database.Common.Constants.FIELD_CSGROUP_CS_GROUP_ID, groupId);
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

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="groupId">グループID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string deptId, string groupId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSOPERATORGROUP_DEPT_ID, deptId);
				ht.Add(Database.Common.Constants.FIELD_CSOPERATORGROUP_CS_GROUP_ID, groupId);
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion
	}
}
