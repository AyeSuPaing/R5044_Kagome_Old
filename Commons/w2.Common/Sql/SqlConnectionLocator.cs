/*
=========================================================================================================
  Module      : SqlConnectionロケータ (SqlConnectionLocator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using System.Threading;

namespace w2.Common.Sql
{
	/// <summary>
	/// SqlConnectionロケータ
	/// </summary>
	public class SqlConnectionLocator
	{
		/// <summary>SqlConnectionの実態</summary>
		private static ThreadLocal<ISqlConnection> _connection = new ThreadLocal<ISqlConnection>(() => null);

		/// <summary>
		/// コネクションセット
		/// </summary>
		/// <param name="connection">コネクション</param>
		public static void SetConnection(ISqlConnection connection)
		{
			_connection.Value = connection;
		}

		/// <summary>
		/// コネクション取得
		/// </summary>
		/// <param name="strSqlConnectionString">接続文字列</param>
		/// <returns>コネクション</returns>
		public static ISqlConnection GetConnection(string strSqlConnectionString)
		{
			return _connection.Value != null
				? _connection.Value
				: new SqlConnectionWrapper(strSqlConnectionString);
		}
	}
}
