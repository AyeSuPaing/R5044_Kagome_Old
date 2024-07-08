/*
=========================================================================================================
  Module      : SqlConnectionインターフェース (ISqlConnection.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data.SqlClient;
using System.Data;

namespace w2.Common.Sql
{
	/// <summary>
	/// SqlConnectionインターフェース
	/// </summary>
	public interface ISqlConnection : IDisposable
	{
		/// <summary>
		/// コネクションオープン
		/// </summary>
		void Open();

		/// <summary>
		/// コネクションクローズ
		/// </summary>
		void Close();

		/// <summary>
		/// トランザクション開始
		/// </summary>
		/// <returns>トランザクション</returns>
		ISqlTransaction BeginTransaction();
		/// <summary>
		/// トランザクション開始：分離レベル指定
		/// </summary>
		/// <param name="ilIsolationLevel">トランザクション分離レベル</param>
		/// <returns>トランザクション</returns>
		ISqlTransaction BeginTransaction(IsolationLevel ilIsolationLevel);

		/// <summary>
		/// コネクションの実態を取得
		/// </summary>
		/// <returns>コネクションの実態</returns>
		SqlConnection GetRealConnection();

		/// <summary>コネクション状態</summary>
		ConnectionState State { get; }
	}
}
