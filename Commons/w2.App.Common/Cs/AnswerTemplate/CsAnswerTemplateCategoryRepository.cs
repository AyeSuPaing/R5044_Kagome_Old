/*
=========================================================================================================
  Module      :回答例カテゴリリポジトリ(CsAnswerTemplateCategoryRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using w2.Common.Sql;

namespace w2.App.Common.Cs.AnswerTemplate
{
	/// <summary>
	/// 回答例カテゴリリポジトリ
	/// </summary>
	public class CsAnswerTemplateCategoryRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsAnswerTemplateCategory";

		#region +SearchByCategoryId カテゴリID指定サーチ（指定がなければ全件）
		/// <summary>
		/// カテゴリID指定サーチ（指定がなければ全件）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">回答カテゴリID</param>
		/// <returns>カテゴリリスト</returns>
		public DataView SearchByCategoryId(string deptId, string categoryId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "SearchByCategoryId"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSANSWERTEMPLATECATEGORY_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSANSWERTEMPLATECATEGORY_CATEGORY_ID, categoryId);
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, ht);
			}
		}
		#endregion 

		#region +GetAll 全件取得(ドロップダウンリスト用)
		/// <summary>
		/// 全件取得(ドロップダウンリスト用)
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>カテゴリリスト</returns>
		public DataView GetAll(string deptId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSANSWERTEMPLATECATEGORY_DEPT_ID, deptId);
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, ht);
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>カテゴリ情報</returns>
		public DataView Get(string deptId, string categoryId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSANSWERTEMPLATECATEGORY_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSANSWERTEMPLATECATEGORY_CATEGORY_ID, categoryId);
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, ht);
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
		/// +Update 更新
		/// </summary>
		/// <param name="ht">更新用データ</param>
		public void Update(Hashtable ht)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "Update"))
			{
				sqlStatement.ExecStatementWithOC(sqlAccessor, ht);
			}
		}
		#endregion

		#region +GetChildCategories 子カテゴリ取得（削除チェック用）
		/// <summary>
		/// 子カテゴリ取得（削除チェック用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>子カテゴリリスト</returns>
		public DataView GetChildCategories(string deptId, string categoryId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "GetChildCategories"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSANSWERTEMPLATECATEGORY_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSANSWERTEMPLATECATEGORY_CATEGORY_ID, categoryId);
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, ht);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		public void Delete(string deptId, string categoryId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSANSWERTEMPLATECATEGORY_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSANSWERTEMPLATECATEGORY_CATEGORY_ID, categoryId);
				sqlStatement.ExecStatementWithOC(sqlAccessor, ht);
			}
		}
		#endregion
	}
}
