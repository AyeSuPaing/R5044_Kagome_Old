/*
=========================================================================================================
  Module      : SQLアクセサモジュール(SqlAccessor.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Data.SqlClient;

namespace w2.Common.Sql
{
	///**************************************************************************************
	/// <summary>
	/// SQL接続を管理する
	/// </summary>
	/// <remarks>
	/// <para>
	///		例1:通常SQL発行
	///		<code>
	///			using (SqlAccessor sqlAccessor = new SqlAccessor(strConnectionString))
	///			{
	///				sqlAccessor.OpenConnection();
	///				int iUpdated = sqlAccessor.Execute("UPDATE w2_User SET del_flg = '0'");
	///			}
	///		</code>
	/// </para>
	/// <para>
	///		例2:SqlStatementを利用してSQL発行
	///		<code>
	///			using (SqlAccessor sqlAccessor = new SqlAccessor(strConnectionString))
	///			using (SqlStatement sqlStatement = new SqlStatement("User", "DeleteUserAll"))
	///			{
	///				sqlStatement.ExecStatementWithOC("UPDATE w2_User SET del_flg = '0'");
	///			}
	///		</code>
	/// </para>
	/// </remarks>
	///**************************************************************************************
	public class SqlAccessor : IDisposable
	{
		/*------------------------------------------------------------------------
		 * ①通常SQL発行
		 * →new SqlAccessor(ConnXmlFilePath)
		 * →sqla.OpenConnection()
		 * →sqla.BeginTransaction()
		 * →sqla.Execute(strSQLStatement) or ExecuteStoredSQL(strSQLStatement) or Select(strSQLStatement)
		 * →sqla.CommitTransaction() or RollbackTransaction()
		 * →sqla.CloseConnection() （Dispose() でもCloseConnection()等を実行）
		 * 
		 * 
		 * ②SqlStatementを用いて取得・発行
		 * →new SqlAccessor(ConnXmlFilePath)
		 * →new SqlStatement(strStatementFilePath, strPageName, statementName);
		 * →sqls.ExecStoredSQL(sqlAcc, htInputParam); 
		 * →sqla.CloseConnection() （Dispose() でもCloseConnection()等を実行）
		 *-----------------------------------------------------------------------*/

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SqlAccessor()
			: this(Constants.STRING_SQL_CONNECTION)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strSqlConnectionString">SQL接続文字列</param>
		public SqlAccessor(string strSqlConnectionString)
		{
			// SQLコネクション作成
			this.Connection = SqlConnectionLocator.GetConnection(strSqlConnectionString);

			// トランザクション分離レベル設定
			this.IsoLevel = IsolationLevel.ReadCommitted;
		}

		/// <summary>
		/// SQLコネクションオープン
		/// </summary>
		public void OpenConnection()
		{
			if (this.Connection.State != ConnectionState.Open)
			{
				this.Connection.Open();
			}
		}

		/// <summary>
		/// SQLコネクションクローズ
		/// </summary>
		public void CloseConnection()
		{
			if (this.Connection.State != ConnectionState.Closed)
			{
				this.Connection.Close();
			}
		}

		/// <summary>
		/// トランザクションスタート
		/// </summary>
		public void BeginTransaction()
		{
			BeginTransaction(this.IsoLevel);
		}
		/// <summary>
		/// トランザクションスタート
		/// </summary>
		/// <param name="ilIsolationLevel">トランザクション分離レベル</param>
		public void BeginTransaction(System.Data.IsolationLevel ilIsolationLevel)
		{
			this.IsoLevel = ilIsolationLevel;

			if (this.Transaction == null)
			{
				this.Transaction = this.Connection.BeginTransaction(this.IsoLevel);
			}
		}

		/// <summary>
		/// トランザクションコミット
		/// </summary>
		/// <remarks>コミット後再びトランザクションを開始します</remarks>
		public void CommitTransaction()
		{
			// トランザクションコミット
			if (this.Transaction != null)
			{
				this.Transaction.Commit();
				this.Transaction = null;
			}

			// トランザクション開始
			BeginTransaction();
		}

		/// <summary>
		/// トランザクションロールバック
		/// </summary>
		public void RollbackTransaction()
		{
			// トランザクションロールバック
			if (this.Transaction != null)
			{
				this.Transaction.Rollback();
				this.Transaction = null;
			}

			// トランザクション開始
			BeginTransaction();
		}

		/// <summary>
		/// SQL文発行・データビュー取得（コネクションオープンクローズあり）
		/// </summary>
		/// <param name="sqlCommand">SQLコマンド</param>
		/// <returns>データビュー</returns>
		public DataView SelectSingleWithOC(SqlCommand sqlCommand)
		{
			this.OpenConnection();

			try
			{
				return SelectSingle(sqlCommand);
			}
			finally
			{
				this.CloseConnection();
			}
		}

		/// <summary>
		/// SQL文発行・データビュー取得
		/// </summary>
		/// <param name="sqlCommand">SQLコマンド</param>
		/// <returns>データビュー</returns>
		public DataView SelectSingle(SqlCommand sqlCommand)
		{
			return Select(sqlCommand).Tables["Table"].DefaultView;
		}

		/// <summary>
		/// SQL文発行・データセット取得
		/// </summary>
		/// <param name="sqlCommand">SQLコマンド</param>
		/// <returns>データセット</returns>
		public DataSet Select(SqlCommand sqlCommand)
		{
			// コネクション・トランザクションセット
			sqlCommand.Connection = this.Connection.GetRealConnection();
			sqlCommand.Transaction = this.Transaction?.GetRealTransaction();

			// SQL発行、データ取得
			SqlDataAdapter sqlDA = new SqlDataAdapter();
			sqlDA.SelectCommand = sqlCommand;

			DataSet dsResult = new DataSet();
			sqlDA.Fill(dsResult);

			return dsResult;
		}

		/// <summary>
		/// SQL実行(影響件数取得）
		/// </summary>
		/// <param name="sqlCommand">SQLコマンド</param>
		/// <returns>影響を受けた行数</returns>
		public int ExecuteWithOC(SqlCommand sqlCommand)
		{
			this.OpenConnection();

			try
			{
				return Execute(sqlCommand);
			}
			finally
			{
				this.CloseConnection();
			}
		}

		/// <summary>
		/// SQL実行（入力パラメタあり）(影響件数取得）
		/// </summary>
		/// <param name="sqlCommand">SQLコマンド</param>
		/// <returns>影響を受けた行数</returns>
		public int Execute(SqlCommand sqlCommand)
		{
			// コネクション・トランザクションセット
			sqlCommand.Connection = this.Connection.GetRealConnection();
			sqlCommand.Transaction = this.Transaction?.GetRealTransaction();

			// SQL発行(影響件数取得）
			return sqlCommand.ExecuteNonQuery();
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			this.Transaction = null;

			CloseConnection();
			this.Connection = null;
		}

		/// <summary>SQLコネクション</summary>
		public ISqlConnection Connection { get; private set; }
		/// <summary>SQLトランザクション</summary>
		public ISqlTransaction Transaction { get; private set; }
		/// <summary>トランザクション分離レベル</summary>
		public IsolationLevel IsoLevel { get; private set; }
	}
}
