/*
=========================================================================================================
  Module      : CSオペレータリポジトリ(CsOperatorRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using w2.Common.Sql;

namespace w2.App.Common.Cs.CsOperator
{
	public class CsOperatorRepository : RepositoryBase 
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsOperator";

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>オペレータ情報</returns>
		public DataView Get(string deptId, string operatorId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				var dv = Get(deptId, operatorId, accessor);
				return dv;
			}
		}
		#endregion
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>オペレータ情報</returns>
		public DataView Get(string deptId, string operatorId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSOPERATOR_DEPT_ID, deptId);
				ht.Add(Database.Common.Constants.FIELD_CSOPERATOR_OPERATOR_ID, operatorId);
				return statement.SelectSingleStatement(accessor, ht);
			}
		}
		#endregion

		#region +GetAll 全件取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>オペレータ情報</returns>
		public DataView GetAll(string deptId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSOPERATOR_DEPT_ID, deptId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetApprovalValidAll 承認可能で有効なオペレータすべて取得
		/// <summary>
		///  承認可能で有効なオペレータすべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>オペレータ情報</returns>
		public DataView GetApprovalValidAll(string deptId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetApprovalValidAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSOPERATOR_DEPT_ID, deptId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetMailSendableValidAll メール送信可能で有効なオペレータすべて取得
		/// <summary>
		///  メール送信可能で有効なオペレータすべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>オペレータ情報</returns>
		public DataView GetMailSendableValidAll(string deptId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetMailSendableValidAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSOPERATOR_DEPT_ID, deptId);
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
		/// <param name="operatorId">オペレータID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string deptId, string operatorId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSOPERATOR_DEPT_ID, deptId);
				ht.Add(Database.Common.Constants.FIELD_CSOPERATOR_OPERATOR_ID, operatorId);
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +DeleteCheck 削除チェック
		/// <summary>
		/// 削除チェック
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>オペレータID</returns>
		public DataView DeleteCheck(string deptId, string operatorId, SqlAccessor accessor)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "DeleteCheck"))
			{
				var ht = new Hashtable
				{
					{ Database.Common.Constants.FIELD_CSOPERATOR_DEPT_ID, deptId },
					{ Database.Common.Constants.FIELD_CSOPERATOR_OPERATOR_ID, operatorId }
				};
				var dv = statement.SelectSingleStatement(accessor, ht);
				return dv;
			}
		}
		/// <summary>
		/// 削除チェック
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>オペレータID</returns>
		public DataView DeleteCheck(string deptId, string operatorId)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				var dv = DeleteCheck(deptId, operatorId, accessor);
				return dv;
			}
		}
		#endregion
	}
}
