/*
=========================================================================================================
  Module      : リポジトリ基底クラス(RepositoryBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using w2.Common.Sql;

namespace w2.App.Common
{
	/// <summary>
	/// レポジトリ基底クラス
	/// </summary>
	public abstract class RepositoryBase
	{
		/// <summary>
		/// Selectステートメント実行
		/// </summary>
		/// <param name="statementPageName">ステートメントのページ名</param>
		/// <param name="statementName">ステートメントの名前</param>
		/// <param name="input">パラメタ情報</param>
		/// <returns>Select結果を内包するデータビュー</returns>
		protected DataView ExecuteSelectStatement(string statementPageName, string statementName, Hashtable input)
		{
			DataView dv = null;

			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				dv = ExecuteSelectStatement(statementPageName, statementName, accessor, input);
				accessor.CloseConnection();
			}

			return dv;
		}

		/// <summary>
		/// Selectステートメント実行
		/// </summary>
		/// <param name="statementPageName">ステートメントのページ名</param>
		/// <param name="statementName">ステートメントの名前</param>
		/// <param name="accessor">SQLアクセッサー</param>
		/// <param name="input">パラメタ情報</param>
		/// <returns>Select結果を内包するデータビュー</returns>
		protected DataView ExecuteSelectStatement(string statementPageName, string statementName, SqlAccessor accessor, Hashtable input)
		{
			DataView dv = null;

			using (SqlStatement statement = new SqlStatement(statementPageName, statementName))
			{
				dv = statement.SelectSingleStatement(accessor, input);
			}

			return dv;
		}


		/// <summary>
		/// ステートメント実行
		/// </summary>
		/// <param name="statementPageName">ステートメントのページ名</param>
		/// <param name="statementName">ステートメントの名前</param>
		/// <param name="input">パラメタ情報</param>
		/// <returns>実行結果影響行数</returns>
		protected int ExecuteNonQuery(string statementPageName, string statementName, Hashtable input)
		{
			int rtn = 0;

			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				rtn = ExecuteNonQuery(statementPageName, statementName, accessor, input);
				accessor.CloseConnection();
			}

			return rtn;
		}

		/// <summary>
		/// ステートメント実行
		/// </summary>
		/// <param name="statementPageName">ステートメントのページ名</param>
		/// <param name="statementName">ステートメントの名前</param>
		/// <param name="accessor">SQLアクセッサー</param>
		/// <param name="input">パラメタ情報</param>
		/// <returns>実行結果影響行数</returns>
		protected int ExecuteNonQuery(string statementPageName, string statementName, SqlAccessor accessor, Hashtable input)
		{
			int rtn = 0;

			using (SqlStatement statement = new SqlStatement(statementPageName, statementName))
			{
				rtn = statement.ExecStatement(accessor, input);
			}

			return rtn;
		}
	}
}
