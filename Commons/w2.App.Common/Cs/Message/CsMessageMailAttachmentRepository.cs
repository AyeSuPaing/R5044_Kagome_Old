/*
=========================================================================================================
  Module      : メッセージメール添付ファイルリポジトリ(CsMessageMailAttachmentRepository.cs)
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
	/// メッセージメール添付ファイルリポジトリ
	/// </summary>
	public class CsMessageMailAttachmentRepository : RepositoryBase 
	{
		private const string XML_KEY_NAME = "CsMessageMailAttachment";

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <param name="fileNo">ファイルNO</param>
		/// <returns>メッセージメール添付ファイル</returns>
		public DataView Get(string deptId, string mailId, int fileNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEMAILATTACHMENT_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEMAILATTACHMENT_MAIL_ID, mailId);
				ht.Add(Constants.FIELD_CSMESSAGEMAILATTACHMENT_FILE_NO, fileNo);

				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetAll メールに紐づくものすべて取得
		/// <summary>
		/// メールに紐づくものすべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <returns>メッセージメール添付ファイルリスト</returns>
		public DataView GetAll(string deptId, string mailId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEMAILATTACHMENT_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEMAILATTACHMENT_MAIL_ID, mailId);

				var dv = statement.SelectSingleStatementWithOC(accessor, ht);
				return dv;
			}
		}
		#endregion

		#region +GetRegisterFileNo 登録用のファイルNO取得
		/// <summary>
		/// 登録用のファイルNO取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録用ファイルNO</returns>
		public int GetRegisterFileNo(string deptId, string mailId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetRegisterFileNo"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEMAILATTACHMENT_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEMAILATTACHMENT_MAIL_ID, mailId);
				var result = (int)statement.SelectSingleStatement(accessor, ht)[0][0];
				return result;
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

		#region +UpdateTempIdToFormalId 仮IDを正式IDに更新
		/// <summary>
		/// 仮IDを正式IDに更新
		/// </summary>
		/// <param name="ht">情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateTempIdToFormalId(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "UpdateTempIdToFormalId"))
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
		/// <param name="fileNo">ファイルNO</param>
		public void Delete(string deptId, string mailId, int fileNo)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEMAILATTACHMENT_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEMAILATTACHMENT_MAIL_ID, mailId);
				ht.Add(Constants.FIELD_CSMESSAGEMAILATTACHMENT_FILE_NO, fileNo);

				int updated = statement.ExecStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +DeleteAll 全て削除
		/// <summary>
		/// 全て削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailId">メールID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteAll(string deptId, string mailId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "DeleteAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMESSAGEMAILATTACHMENT_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMESSAGEMAILATTACHMENT_MAIL_ID, mailId);

				int updated = statement.ExecStatement(accessor, ht);
			}
		}
		#endregion
	}
}
