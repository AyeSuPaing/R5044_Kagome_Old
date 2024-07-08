/*
=========================================================================================================
  Module      : インシデントカテゴリマスタリポジトリ(CsIncidentCategoryRepository.cs)
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

namespace w2.App.Common.Cs.IncidentCategory
{
	/// <summary>
	/// インシデントカテゴリマスタリポジトリ
	/// </summary>
	public class CsIncidentCategoryRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsIncidentCategory";

		#region +SearchByCategoryId カテゴリID指定サーチ（指定がなければ全件）
		/// <summary>
		/// カテゴリID指定サーチ（指定がなければ全件）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>インシデントカテゴリリスト</returns>
		public DataView SearchByCategoryId(string deptId, string categoryId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "SearchByCategoryId"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_CSINCIDENTCATEGORY_DEPT_ID, deptId);
				input.Add(Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_ID, categoryId);
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			}
		}
		#endregion

		#region +GetAll 全件取得（ドロップダウン等で利用、有効フラグは階層考慮）
		/// <summary>
		/// 全件取得（ドロップダウン等で利用、有効フラグは階層考慮）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>インシデントカテゴリリスト</returns>
		internal DataView GetAll(string deptId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "GetAll"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_CSINCIDENTCATEGORY_DEPT_ID, deptId);
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>インシデントカテゴリ</returns>
		public DataView Get(string deptId, string categoryId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENTCATEGORY_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_ID, categoryId);
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, ht);
			}
		}
		#endregion

		#region +GetCategoryTrailList カテゴリ階層リスト（ルートカテゴリまでのリスト）取得
		/// <summary>
		/// カテゴリ階層リスト（ルートカテゴリまでのリスト）取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>インシデントカテゴリリスト</returns>
		public DataView GetCategoryTrailList(string deptId, string categoryId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "GetCategoryTrailList"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSINCIDENTCATEGORY_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_ID, categoryId);
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
			using (SqlStatement SqlStatement = new SqlStatement(XML_KEY_NAME, "Update"))
			{
				SqlStatement.ExecStatementWithOC(sqlAccessor, ht);
			}
		}
		#endregion

		#region +GetIncidents インシデント取得（削除チェック用）
		/// <summary>
		/// インシデント取得（削除チェック用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>カテゴリ情報</returns>
		public DataView GetIncidents(string deptId, string categoryId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "GetIncidents"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_CSINCIDENTCATEGORY_DEPT_ID, deptId);
				input.Add(Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_ID, categoryId);
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			}
		}
		#endregion

		#region +GetChildCategories 子カテゴリ取得（削除チェック用）
		/// <summary>
		/// 子カテゴリ取得（削除チェック用）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>カテゴリ情報</returns>
		public DataView GetChildCategories(string deptId, string categoryId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "GetChildCategories"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_CSINCIDENTCATEGORY_DEPT_ID, deptId);
				input.Add(Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_ID, categoryId);
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
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
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_CSINCIDENTCATEGORY_DEPT_ID, deptId);
				input.Add(Constants.FIELD_CSINCIDENTCATEGORY_CATEGORY_ID, categoryId);
				sqlStatement.ExecStatementWithOC(sqlAccessor, input);
			}
		}
		#endregion
	}
}
