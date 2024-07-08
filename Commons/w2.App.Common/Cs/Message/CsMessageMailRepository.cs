/*
=========================================================================================================
  Module      : メッセージメールリポジトリ(CsMessageMailRepository.cs)
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

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージメールリポジトリ
	/// </summary>
	public class CsMessageMailRepository : RepositoryBase 
	{
		private const string XML_KEY_NAME = "CsMessageMail";

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <returns>メール情報</returns>
		public DataView Get(string deptId, string mailId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_CSMESSAGEMAIL_DEPT_ID, deptId);
				input.Add(Constants.FIELD_CSMESSAGEMAIL_MAIL_ID, mailId);

				var dv = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
				return dv;
			}
		}
		#endregion

		#region +GetWithAttachment 取得(メール情報 & メール添付情報)
		/// <summary>
		/// 取得(メール情報 & メール添付情報)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <returns>メール情報 & メール添付情報</returns>
		public DataView GetWithAttachment(string deptId, string mailId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "GetWithAttachment"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_CSMESSAGEMAIL_DEPT_ID, deptId);
				input.Add(Constants.FIELD_CSMESSAGEMAIL_MAIL_ID, mailId);

				var dv = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
				return dv;
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

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		public void Delete(string deptId, string mailId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEMAIL_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEMAIL_MAIL_ID, mailId);

				int updated = statement.ExecStatement(accessor, ht);
			}
		}
		#endregion
	}
}
