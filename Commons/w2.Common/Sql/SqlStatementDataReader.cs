/*
=========================================================================================================
  Module      : SQLステートメントデータリーダーモジュール(SqlStatementDataReader.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data.SqlClient;

namespace w2.Common.Sql
{
	///*********************************************************************************************
	/// <summary>
	/// SqlAccessor、SqlStatementを利用してSQLでSELECTしたデータを連続して読み取る
	/// </summary>
	///*********************************************************************************************
	public class SqlStatementDataReader : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sqlAccessor">SqlAccessor</param>
		/// <param name="sqlStatement">SqlStatement</param>
		public SqlStatementDataReader(SqlAccessor sqlAccessor, SqlStatement sqlStatement)
			: this(sqlAccessor, sqlStatement, null, false)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sqlAccessor">SqlAccessor</param>
		/// <param name="sqlStatement">SqlStatement</param>
		/// <param name="blOpenConnection">コネクションを自動で開くか</param>
		public SqlStatementDataReader(SqlAccessor sqlAccessor, SqlStatement sqlStatement, bool blOpenConnection)
			: this(sqlAccessor, sqlStatement, null, blOpenConnection)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sqlAccessor">SqlAccessor</param>
		/// <param name="sqlStatement">SqlStatement</param>
		/// <param name="dicInput">SQLパラメタ</param>
		public SqlStatementDataReader(SqlAccessor sqlAccessor, SqlStatement sqlStatement, IDictionary dicInput)
			: this(sqlAccessor, sqlStatement, dicInput, false)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="sqlAccessor">SqlAccessor</param>
		/// <param name="sqlStatement">SqlStatement</param>
		/// <param name="dicInput">SQLパラメタ</param>
		/// <param name="blOpenConnection">コネクションを自動で開くか</param>
		public SqlStatementDataReader(SqlAccessor sqlAccessor, SqlStatement sqlStatement, IDictionary dicInput, bool blOpenConnection)
		{
			// 必要あればコネクションオープン
			if (blOpenConnection)
			{
				sqlAccessor.OpenConnection();
			}

			try
			{
				var begin = DateTime.Now;

				// ステートメント実行＆SqlDataReader取得
				using (SqlCommand sqlCommand = sqlStatement.CreateSqlCommand(sqlStatement.Statement, dicInput))
				{
					sqlCommand.Connection = sqlAccessor.Connection.GetRealConnection();
					sqlCommand.Transaction = sqlAccessor.Transaction?.GetRealTransaction();
					this.SqlDataReader = sqlCommand.ExecuteReader();

					if (Constants.LOGGING_PERFORMANCE_SQL_ENABLED) sqlStatement.WritePerformanceLog(begin);
				}
			}
			catch (Exception ex)
			{
				throw new w2Exception(sqlStatement.CreateSqlExecErrorMessage(dicInput), ex);
			}
		}

		/// <summary>
		/// 角かっこつきインデックス
		/// </summary>
		/// <param name="strFieldName">フィールド名</param>
		/// <returns>値</returns>
		public object this[string strFieldName]
		{
			get { return this.SqlDataReader[strFieldName]; }
		}

		/// <summary>
		/// 角かっこつきインデックス
		/// </summary>
		/// <param name="iIndex">フィールドインデックス</param>
		/// <returns>値</returns>
		public object this[int iIndex]
		{
			get { return this.SqlDataReader[iIndex]; }
		}

		/// <summary>
		/// 一行読み込み
		/// </summary>
		/// <returns>読み込み成功/失敗</returns>
		public bool Read()
		{
			return this.SqlDataReader.Read();
		}

		/// <summary>
		/// フィールド名取得
		/// </summary>
		/// <param name="iIndex">フィールドインデックス</param>
		/// <returns>フィールド名</returns>
		public string GetName(int iIndex)
		{
			return this.SqlDataReader.GetName(iIndex);
		}

		/// <summary>
		/// フィールド型取得
		/// </summary>
		/// <param name="index">フィールドインデックス</param>
		/// <returns>フィールド型</returns>
		public Type GetFieldType(int index)
		{
			return this.SqlDataReader.GetFieldType(index);
		}

		/// <summary>
		/// SqlDataReader.Close() 実行
		/// </summary>
		public void Close()
		{
			this.SqlDataReader.Close();
		}

		/// <summary>
		/// SqlDataReader.Dispose() 実行
		/// </summary>
		public void Dispose()
		{
			this.SqlDataReader.Dispose();
		}

		/// <summary>SqlDataReader</summary>
		public SqlDataReader SqlDataReader { get; private set; }
		/// <summary>フィールドカウント</summary>
		public int FieldCount { get { return this.SqlDataReader.FieldCount; } }
		/// <summary>行存在判定</summary>
		public bool HasRows { get { return this.SqlDataReader.HasRows; } }
		/// <summary>行存在判定</summary>
		public bool IsClosed { get { return this.SqlDataReader.IsClosed; } }
	}
}
