/*
=========================================================================================================
  Module      : 回答例リポジトリ(CsAnswerTemplateRepository.cs)
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
	/// 回答例リポジトリ
	/// </summary>
	public class CsAnswerTemplateRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		const string XML_KEY_NAME = "CsAnswerTemplate";

		#region +Search 全件検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="answerTitle">回答例タイトル</param>
		/// <param name="answerMailTitle">回答例件名</param> 
		/// <param name="answerText">回答例テキスト</param>
		/// <param name="validFlg">有効フラグ</param>
		/// <param name="beginRow">開始NO</param>
		/// <param name="endRow">終了NO</param>
		/// <returns>検索結果</returns>
		public DataView Search(
			string deptId,
			string categoryId,
			string answerTitle,
			string answerMailTitle,
			string answerText,
			string validFlg,
			int beginRow,
			int endRow)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement(XML_KEY_NAME, "Search"))
			{
				var input = new Hashtable
				{
					{ Constants.FIELD_CSANSWERTEMPLATE_DEPT_ID, deptId },
					{ Constants.FIELD_CSANSWERTEMPLATE_ANSWER_CATEGORY_ID, categoryId },
					{ Constants.FIELD_CSANSWERTEMPLATE_ANSWER_TITLE, answerTitle },
					{ Constants.FIELD_CSANSWERTEMPLATE_ANSWER_MAIL_TITLE, answerMailTitle },
					{ Constants.FIELD_CSANSWERTEMPLATE_ANSWER_TEXT, answerText },
					{ Constants.FIELD_CSANSWERTEMPLATE_VALID_FLG, validFlg },
					{ "bgn_row_num", beginRow },
					{ "end_row_num", endRow },
				};
				var result = statement.SelectSingleStatementWithOC(accessor, input);
				return result;
			}
		}
		#endregion

		#region +SearchValid 有効なものから検索
		/// <summary>
		/// 有効なものから検索
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="categoryId">カテゴリID</param>
		/// <param name="keyword">キーワード</param>
		/// <returns>検索結果</returns>
		public DataView SearchValid(string deptId, string categoryId, string keyword)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "SearchValid"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSANSWERTEMPLATE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSANSWERTEMPLATE_ANSWER_CATEGORY_ID, categoryId);
				ht.Add("keyword", keyword);
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, ht);
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="answerId">回答例ID</param>
		/// <returns>リスト</returns>
		public DataView Get(string deptId, string answerId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "Get"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSANSWERTEMPLATE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSANSWERTEMPLATE_ANSWER_ID, answerId);
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, ht);
			}
		}
		#endregion

		#region +Register 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="ht">回答例情報</param>
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
		/// <param name="ht">回答例情報</param>
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
		/// <param name="answerId">回答例ID</param>
		public void Delete(string deptId, string answerId)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(XML_KEY_NAME, "Delete"))
			{
				Hashtable ht = new Hashtable();
				ht.Add(Constants.FIELD_CSANSWERTEMPLATE_DEPT_ID, deptId);
				ht.Add(Constants.FIELD_CSANSWERTEMPLATE_ANSWER_ID, answerId);
				sqlStatement.ExecStatementWithOC(sqlAccessor, ht);
			}
		}
		#endregion
	}
}
