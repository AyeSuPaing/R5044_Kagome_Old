/*
=========================================================================================================
  Module      : CSオペレータ権限リポジトリ(CsOperatorAuthorityRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using w2.Common.Sql;

namespace w2.App.Common.Cs.CsOperator
{
	public class CsOperatorAuthorityRepository : RepositoryBase 
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsOperatorAuthority";

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorAuthorityId">オペレータ権限ID</param>
		/// <returns>オペレータ権限情報</returns>
		public DataView Get(string deptId, string operatorAuthorityId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSOPERATORAUTHORITY_DEPT_ID, deptId);
				ht.Add(Database.Common.Constants.FIELD_CSOPERATORAUTHORITY_OPERATOR_AUTHORITY_ID, operatorAuthorityId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetAll 全件取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>オペレータ権限情報</returns>
		public DataView GetAll(string deptId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSOPERATORAUTHORITY_DEPT_ID, deptId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetRegisterId 登録用ID取得
		/// <summary>
		/// 登録用ID取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録用ID</returns>
		public string GetRegisterId(string deptId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetRegisterId"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSOPERATORAUTHORITY_DEPT_ID, deptId);
				return statement.SelectSingleStatement(accessor, ht)[0][0].ToString();
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
		/// <param name="operatorAuthorityId">オペレータ権限ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string deptId, string operatorAuthorityId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSOPERATORAUTHORITY_DEPT_ID, deptId);
				ht.Add(Database.Common.Constants.FIELD_CSOPERATORAUTHORITY_OPERATOR_AUTHORITY_ID, operatorAuthorityId);
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion
	}
}
