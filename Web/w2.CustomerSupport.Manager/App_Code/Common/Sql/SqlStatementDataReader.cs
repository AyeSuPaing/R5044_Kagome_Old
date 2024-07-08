/*
=========================================================================================================
  Module      : SqlStatementDataReaderクラス(SqlStatementDataReader.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SqlStatementDataReader の概要の説明です
/// </summary>
public class SqlStatementDataReader : w2.Common.Sql.SqlStatementDataReader
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="sqlAccessor">SqlAccessor</param>
	/// <param name="sqlStatement">SqlStatement</param>
	public SqlStatementDataReader(SqlAccessor sqlAccessor, SqlStatement sqlStatement)
		: base(sqlAccessor, sqlStatement, null, false)
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="sqlAccessor">SqlAccessor</param>
	/// <param name="sqlStatement">SqlStatement</param>
	/// <param name="blOpenConnection">コネクションを自動で開くか</param>
	public SqlStatementDataReader(SqlAccessor sqlAccessor, SqlStatement sqlStatement, bool blOpenConnection)
		: base(sqlAccessor, sqlStatement, null, blOpenConnection)
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="sqlAccessor">SqlAccessor</param>
	/// <param name="sqlStatement">SqlStatement</param>
	/// <param name="dicInput">SQLパラメタ</param>
	public SqlStatementDataReader(SqlAccessor sqlAccessor, SqlStatement sqlStatement, IDictionary dicInput)
		: base(sqlAccessor, sqlStatement, dicInput, false)
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
		: base(sqlAccessor, sqlStatement, dicInput, blOpenConnection)
	{
	}
}
