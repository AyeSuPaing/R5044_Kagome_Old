/*
=========================================================================================================
  Module      : SqlAccessorクラス(SqlAccessor.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SqlAccessor の概要の説明です
/// </summary>
public class SqlAccessor : w2.Common.Sql.SqlAccessor
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public SqlAccessor()
		: base()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="strSqlConnectionString">SQL接続文字列</param>
	public SqlAccessor(string strSqlConnectionString)
		: base(strSqlConnectionString)
	{
	}
}
