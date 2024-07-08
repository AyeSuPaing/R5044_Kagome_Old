/*
=========================================================================================================
  Module      : SqlStatementクラス(SqlStatement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SqlStatement の概要の説明です
/// </summary>
public class SqlStatement : w2.Common.Sql.SqlStatement
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="strPageName">ステートメントページ名</param>
	/// <param name="strStatementName">ステートメント名</param>
	public SqlStatement(string strPageName, string strStatementName)
		: base(strPageName, strStatementName)
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="strStatementDirPath">ステートメントXMLディレクトリ物理パス</param>
	/// <param name="strPageName">ステートメントページ名</param>
	/// <param name="strStatementName">ステートメント名</param>
	public SqlStatement(string strStatementDirPath, string strPageName, string strStatementName)
		: base(strStatementDirPath, strPageName, strStatementName)
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="strStatement">SQLステートメント</param>
	public SqlStatement(string strStatement)
		: base(strStatement)
	{
	}
}
