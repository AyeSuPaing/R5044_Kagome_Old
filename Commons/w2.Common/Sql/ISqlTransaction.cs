/*
=========================================================================================================
  Module      : SqlTransactionインターフェース (ISqlTransaction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data.SqlClient;

namespace w2.Common.Sql
{
	/// <summary>
	/// SqlTransactionインターフェース
	/// </summary>
	public interface ISqlTransaction : IDisposable
	{
		/// <summary>
		/// トランザクションコミット
		/// </summary>
		void Commit();

		/// <summary>
		/// トランザクションロールバック
		/// </summary>
		void Rollback();

		/// <summary>
		/// トランザクションの実態を取得
		/// </summary>
		/// <returns>トランザクションの実態</returns>
		SqlTransaction GetRealTransaction();
	}
}
