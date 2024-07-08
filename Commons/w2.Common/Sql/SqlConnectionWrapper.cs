/*
=========================================================================================================
  Module      : SqlConnectionWrapper (SqlConnectionWrapper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using System.Data.SqlClient;
using System.Data;

namespace w2.Common.Sql
{
	/// <summary>
	/// SqlConnectionWrapper
	/// </summary>
	public class SqlConnectionWrapper : ISqlConnection
	{
		/// <summary>SqlConnectionの実態</summary>
		private readonly SqlConnection _connection;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strSqlConnectionString">接続文字列</param>
		public SqlConnectionWrapper(string strSqlConnectionString)
		{
			_connection = new SqlConnection(strSqlConnectionString);
		}

		/// <summary>
		/// コネクションオープン
		/// </summary>
		public void Open() => _connection.Open();

		/// <summary>
		/// コネクションクローズ
		/// </summary>
		public void Close() => _connection.Close();

		/// <summary>
		/// トランザクション開始
		/// </summary>
		/// <returns>トランザクション</returns>
		public ISqlTransaction BeginTransaction() => new SqlTransactionWrapper(_connection.BeginTransaction());
		/// <summary>
		/// トランザクション開始：分離レベル指定
		/// </summary>
		/// <param name="ilIsolationLevel">トランザクション分離レベル</param>
		/// <returns>トランザクション</returns>
		public ISqlTransaction BeginTransaction(IsolationLevel ilIsolationLevel) => new SqlTransactionWrapper(_connection.BeginTransaction(ilIsolationLevel));

		/// <summary>
		/// コネクションDispose
		/// </summary>
		public void Dispose() => _connection.Dispose();

		/// <summary>
		/// コネクションの実態を取得
		/// </summary>
		/// <returns>コネクションの実態</returns>
		public SqlConnection GetRealConnection()
		{
			return _connection;
		}

		/// <summary>コネクション状態</summary>
		public ConnectionState State => _connection.State;
	}
}
