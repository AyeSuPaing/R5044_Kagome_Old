/*
=========================================================================================================
  Module      : メール振分設定アイテムリポジトリ(CsMailAssignSettingItemRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using w2.Common.Sql;

namespace w2.App.Common.MailAssignSetting
{
	/// <summary>
	/// メール振分設定アイテムリポジトリ
	/// </summary>
	public class CsMailAssignSettingItemRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsMailAssignSettingItem";

		#region +GetAll すべて取得
		/// <summary>
		/// すべて取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailAssignId">メール振分設定ID</param>
		/// <returns>メール振分設定アイテム一覧データビュー</returns>
		public DataView GetAll(string deptId, string mailAssignId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSMAILASSIGNSETTINGITEM_DEPT_ID, deptId);
				ht.Add(Database.Common.Constants.FIELD_CSMAILASSIGNSETTINGITEM_MAIL_ASSIGN_ID, mailAssignId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetRegistItemNo 登録用アイテムNO取得
		/// <summary>
		/// 登録用アイテムNO取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailAssignId">メール振分設定ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録用の振分設定枝番</returns>
		public int GetRegistItemNo(string deptId, string mailAssignId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetRegistItemNo"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMAILASSIGNSETTINGITEM_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMAILASSIGNSETTINGITEM_MAIL_ASSIGN_ID, mailAssignId);
				return (int)statement.SelectSingleStatement(accessor, ht)[0][0];
			}
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="ht">登録データ</param>
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
		/// <param name="ht">登録データ</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int Update(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Update"))
			{
				return statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailAssignId">メール振分設定ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string deptId, string mailAssignId, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMAILASSIGNSETTINGITEM_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMAILASSIGNSETTINGITEM_MAIL_ASSIGN_ID, mailAssignId);
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +DeleteAfter 以降削除
		/// <summary>
		/// 以降削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailAssignId">メール振分設定ID</param>
		/// <param name="mailAssignItemNo">メール振分アイテム番号</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteAfter(string deptId, string mailAssignId, int mailAssignItemNo, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "DeleteAfter"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSMAILASSIGNSETTINGITEM_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMAILASSIGNSETTINGITEM_MAIL_ASSIGN_ID, mailAssignId);
				ht.Add(Constants.FIELD_CSMAILASSIGNSETTINGITEM_ITEM_NO, mailAssignItemNo);
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion
	}
}
