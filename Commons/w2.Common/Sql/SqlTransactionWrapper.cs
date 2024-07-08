/*
=========================================================================================================
  Module      : SqlTransactionWrapper (SqlTransactionWrapper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using System.Data.SqlClient;

namespace w2.Common.Sql
{
	/// <summary>
	/// SqlTransactionWrapper
	/// </summary>
	public class SqlTransactionWrapper : ISqlTransaction
	{
		/// <summary>トランザクションの実態</summary>
		private readonly SqlTransaction _transaction;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="transaction">トランザクション</param>
		public SqlTransactionWrapper(SqlTransaction transaction) => _transaction = transaction;

		/// <summary>
		/// トランザクションコミット
		/// </summary>
		public void Commit() => _transaction.Commit();

		/// <summary>
		/// トランザクションロールバック
		/// </summary>
		public void Rollback() => _transaction.Rollback();

		/// <summary>
		/// トランザクションDispose
		/// </summary>
		public void Dispose() => _transaction.Dispose();

		/// <summary>
		/// トランザクションの実態を取得
		/// </summary>
		/// <returns>トランザクションの実態</returns>
		public SqlTransaction GetRealTransaction()
		{
			return _transaction;
		}
	}
}
