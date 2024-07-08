/*
=========================================================================================================
  Module      : メール振分設定リポジトリ(CsMailAssignSettingRepository.cs)
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
	/// メール振分設定リポジトリ
	/// </summary>
	public class CsMailAssignSettingRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsMailAssignSetting";

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailAssignId">メール振分設定ID</param>
		/// <returns>メール振分設定</returns>
		public DataView Get(string deptId, string mailAssignId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSMAILASSIGNSETTING_DEPT_ID, deptId);
				ht.Add(Database.Common.Constants.FIELD_CSMAILASSIGNSETTING_MAIL_ASSIGN_ID, mailAssignId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		#endregion

		#region +GetAll 全件取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>メール振分設定一覧</returns>
		public DataView GetAll(string deptId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Database.Common.Constants.FIELD_CSMAILASSIGNSETTING_DEPT_ID, deptId);
				return statement.SelectSingleStatementWithOC(accessor, ht);
			}
		}
		/// <summary>
		/// Get all by mail assign ids
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="mailAssignIds">Mail assign ids</param>
		/// <returns>メール振分設定一覧</returns>
		public DataView GetAll(string deptId, string[] mailAssignIds)
		{
			var input = new Hashtable() { {Database.Common.Constants.FIELD_CSMAILASSIGNSETTING_DEPT_ID, deptId} };

			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement(XML_KEY_NAME, "GetAllByMailAssignIds"))
			{
				// Replace condition for mail assign ids
				var conditionMailAssignId = string.Format("'{0}'", string.Join("','", mailAssignIds));

				var conditionReplace = string.Format("AND {0}.{1} IN ({2})",
					Constants.TABLE_CSMAILASSIGNSETTING,
					Constants.FIELD_CSMAILASSIGNSETTING_MAIL_ASSIGN_ID,
					conditionMailAssignId);

				statement.ReplaceStatement("@@ mail_assign_ids @@", conditionReplace);

				return statement.SelectSingleStatementWithOC(accessor, input);
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
		/// <param name="ht">更新データ</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Update(Hashtable ht, SqlAccessor accessor)
		{
			using (SqlStatement statement = new SqlStatement(XML_KEY_NAME, "Update"))
			{
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion

		#region +UpdateMailAssignSettingByRemoveOperator 移動になったオペレータでメール振り分け設定更新
		/// <summary>
		/// 移動になったオペレータでメール振り分け設定更新
		/// </summary>
		/// <param name="deptId">ログインオペレータ識別ID</param>
		/// <param name="groupId">グループID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateMailAssignSettingByRemoveOperator(string deptId, string groupId, string operatorId, SqlAccessor accessor)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "UpdateMailAssignSettingByRemoveOperator"))
			{
				var ht = new Hashtable()
				{
					{Constants.FIELD_CSMAILASSIGNSETTING_DEPT_ID, deptId},
					{Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_CS_GROUP_ID, groupId},
					{Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_OPERATOR_ID, operatorId},
				};
				statement.ExecStatement(accessor, ht);
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
				ht.Add(Constants.FIELD_CSMAILASSIGNSETTING_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSMAILASSIGNSETTING_MAIL_ASSIGN_ID, mailAssignId);
				statement.ExecStatement(accessor, ht);
			}
		}
		#endregion
	}
}
