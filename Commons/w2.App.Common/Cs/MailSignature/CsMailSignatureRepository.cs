/*
=========================================================================================================
  Module      : メール署名リポジトリ(CsMailSignatureRepository.cs)
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

namespace w2.App.Common.Cs.MailSignature
{
	public class CsMailSignatureRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsMailSignature";

		#region +Search 一覧取得
		/// <summary>
		/// 一覧取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="bgnRow">開始行</param>
		/// <param name="endRow">終了行</param>
		/// <returns>メール署名一覧データ</returns>
		public DataView Search(string deptId, string operatorId, int bgnRow, int endRow)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "Search"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMAILSIGNATURE_DEPT_ID, deptId);		// 識別ID
				ht.Add(Constants.FIELD_CSMAILSIGNATURE_OWNER_ID, operatorId);	// オペレータID
				ht.Add("bgn_row_num", bgnRow);									// 表示開始記事番号
				ht.Add("end_row_num", endRow);									// 表示終了記事番号
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, ht);
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="signatureId">メール署名ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>メール署名データ</returns>
		public DataView Get(string deptId, string signatureId, string operatorId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatements = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMAILSIGNATURE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMAILSIGNATURE_MAIL_SIGNATURE_ID, signatureId);
				ht.Add(Constants.FIELD_CSMAILSIGNATURE_OWNER_ID, operatorId);
				return sqlStatements.SelectSingleStatementWithOC(sqlAccessor, ht);
			}
		}
		#endregion

		#region +GetUsableAll 指定オペレータにとって使用可能なものを取得
		/// <summary>
		/// 指定オペレータにとって使用可能なものを取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>メール署名データ</returns>
		public DataView GetUsableAll(string deptId, string operatorId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetUsableAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMAILSIGNATURE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMAILSIGNATURE_OWNER_ID, operatorId);
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
		/// <param name="signatureId">メール署名ID</param>
		public void Delete(string deptId, string signatureId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMAILSIGNATURE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMAILSIGNATURE_MAIL_SIGNATURE_ID, signatureId);
				statement.ExecStatementWithOC(accessor, ht);
			}
		}
		#endregion
	}
}
