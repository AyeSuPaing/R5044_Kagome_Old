/*
=========================================================================================================
  Module      : SQLステートメントモジュール(SqlStatement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using w2.Common.Helper;
using w2.Common.Helper.Attribute;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.Common.Sql
{
	///**************************************************************************************
	/// <summary>
	///	SqlAccessorを利用してSQLを発行する
	/// </summary>
	///**************************************************************************************
	public class SqlStatement : IDisposable
	{
		/// <summary>デフォルトの動的パラメタ置換文字列</summary>
		const string DEFAULT_DYNAMIC_PARAMS_REPLACE_STRING = "@@ params @@";
		/// <summary>デフォルトの動的パラメタプリフィックス</summary>
		const string DEFAULT_DYNAMIC_PARAM_PREFIX = "p";
		/// <summary>デフォルトのSQLタイムアウト値設定</summary>
		const int DEFAULT_SQL_COMMAND_TIMEOUT = 60;

		/// <summary>SQLパラメタ構造体</summary>
		[Serializable]
		public struct SqlParam
		{
			/// <summary>名称</summary>
			public string Name;
			/// <summary>型</summary>
			public SqlDbType Type;
			/// <summary>サイズ(サイズなしはnull)</summary>
			public int? Size;
			/// <summary>decimalの最大桁数</summary>
			public byte? Precision;
			/// <summary>decimalの小数点以下の桁数</summary>
			public byte? Scale;
		}

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		private SqlStatement()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statementReader">ステートメントリーダー</param>
		private SqlStatement(SqlStatementReader statementReader)
			: this()
		{
			this.StatementReader = statementReader;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="pageName">ステートメントページ名</param>
		/// <param name="statementName">ステートメント名</param>
		public SqlStatement(string pageName, string statementName)
			: this(Constants.PHYSICALDIRPATH_SQL_STATEMENT, pageName, statementName)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statementDirPath">ステートメントXMLディレクトリ物理パス</param>
		/// <param name="pageName">ステートメントページ名</param>
		/// <param name="statementName">ステートメント名</param>
		public SqlStatement(string statementDirPath, string pageName, string statementName)
			: this(new SqlStatementReader(statementDirPath, pageName, statementName))
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		///<param name="resourceManager">リソースマネージャー</param>
		///<param name="resourceName">リソース名</param>
		///<param name="pageName">ステートメントページ名</param>
		///<param name="statementName">ステートメント名</param>
		public SqlStatement(ResourceManager resourceManager, string resourceName, string pageName, string statementName)
			: this(new SqlStatementReader(resourceManager, resourceName, pageName, statementName))
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statement">SQLステートメント</param>
		public SqlStatement(string statement)
			: this(new SqlStatementReader(statement))
		{
		}
		#endregion

		/// <summary>
		/// ステートメント置換（サブステートメント埋め込みも行う）
		/// </summary>
		/// <param name="oldValue">置換前文字列</param>
		/// <param name="newValue">置換後文字列</param>
		public void ReplaceStatement(string oldValue, string newValue)
		{
			this.StatementReader.ReplaceStatementString(oldValue, newValue);
		}

		/// <summary>
		/// 入力パラメタ追加
		/// </summary>
		/// <param name="name">パラメタ名</param>
		/// <param name="type">型</param>
		/// <param name="size">サイズ</param>
		public void AddInputParameters(string name, SqlDbType type, int? size)
		{
			AddInputParameters(name, type, size.HasValue ? size.ToString() : null);
		}
		/// <summary>
		/// 入力パラメタ追加
		/// </summary>
		/// <param name="strName">パラメタ名</param>
		/// <param name="sdtSqlDbType">型</param>
		/// <param name="strSize">サイズ</param>
		public void AddInputParameters(string strName, SqlDbType sdtSqlDbType, string strSize = null)
		{
			if (this.StatementReader.InputParamDefines == null)
			{
				this.StatementReader.InputParamDefines = new Dictionary<string, SqlParam>();
			}
			if (this.StatementReader.InputParamDefines.ContainsKey(strName) == false)
			{
				this.StatementReader.InputParamDefines.Add(strName, CreateSqlParam(strName, sdtSqlDbType, strSize));
			}
		}

		#region -GetParamDefinition パラメタ情報取得
		// HACK:MasterImportで参照されているので仕方なくpublic staticで定義
		/// <summary>
		/// パラメタ情報取得
		/// </summary>
		/// <param name="targetStatement">ターゲットステートメント</param>
		/// <param name="xPath">XPATH</param>
		/// <returns>パラメタ情報（なし：null）</returns>
		public static Dictionary<string, SqlParam> GetParamDefinition(XElement targetStatement, string xPath)
		{
			var sqlParams = new Dictionary<string, SqlParam>();
			foreach (var element in targetStatement.XPathSelectElements(xPath))
			{
				try
				{
					var param = CreateSqlParam(
						element.Attribute("Name").Value,
						GetSqlDbType(element.Attribute("Type").Value),
						(element.Attribute("Size") != null) ? element.Attribute("Size").Value : null);
					if (sqlParams.ContainsKey(param.Name)) continue;

					sqlParams.Add(param.Name, param);
				}
				catch (Exception ex)
				{
					throw new w2Exception("ノード「" + element.Value + "」がSqlParamへ変換できませんでした。", ex);
				}
			}

			return sqlParams;
		}
		#endregion

		/// <summary>
		/// パラメタ作成
		/// </summary>
		/// <param name="strName">パラメタ名</param>
		/// <param name="sdtSqlDbType">型</param>
		/// <param name="strSize">サイズ</param>
		public static SqlParam CreateSqlParam(string strName, SqlDbType sdtSqlDbType, string strSize)
		{
			SqlParam spSqlParam = new SqlParam();
			spSqlParam.Name = strName;
			spSqlParam.Type = sdtSqlDbType;
			if (strSize != null)
			{
				if (sdtSqlDbType == SqlDbType.Decimal)
				{
					var splitted = strSize.Split(',');
					spSqlParam.Precision = byte.Parse(splitted[0]);
					spSqlParam.Scale = byte.Parse(splitted[1]);
				}
				else if (strSize.ToUpper() != "MAX")
				{
					spSqlParam.Size = int.Parse(strSize);
				}
			}

			return spSqlParam;
		}

		/// <summary>
		/// SqlCommandオブジェクト生成（パラメタ設定）
		/// </summary>
		/// <param name="statement">ステートメント</param>
		/// <param name="input">入力パラメタ</param>
		/// <returns>SqlCommand</returns>
		public SqlCommand CreateSqlCommand(string statement, IDictionary input)
		{
			this.Statement = statement;
			var sqlCommand = new SqlCommand();
			sqlCommand.CommandText = statement;
			// コマンドタイムアウトが指定しない場合、デフォルトのSQLタイムアウト値を設定する
			sqlCommand.CommandTimeout = (this.CommandTimeout > 0) ? this.CommandTimeout : DEFAULT_SQL_COMMAND_TIMEOUT;

			if (this.Statement.Contains("<@@"))
			{
				this.Statement = SqlStatementTagReplacer.ReplaceTags(this.Statement, input ?? new Hashtable());
				sqlCommand.CommandText = this.Statement;
			}

			if (input != null)
			{
				sqlCommand.CommandText = this.Statement;
				try
				{
					// 入力パラメタ作成
					if (this.UseLiteralSql)
					{
						sqlCommand.CommandText = CreateParamDefineStatement(input) + sqlCommand.CommandText;
					}
					else
					{
						SetInputParameters(sqlCommand, input);
					}

					// 出力パラメタ作成
					SetOutputParameters(sqlCommand);
				}
				catch (FormatException fe)
				{
					throw new w2Exception("SQL引数の値が不正です。", fe);
				}
			}

			return sqlCommand;
		}

		#region -CreateParamDefineStatement パラメタ定義ステートメント作成
		/// <summary>
		/// パラメタ定義ステートメント作成
		/// </summary>
		/// <param name="input">入力情報</param>
		private string CreateParamDefineStatement(IDictionary input)
		{
			if (input == null) return "";

			var statement = new StringBuilder();

			// DECLARE文作成
			foreach (string key in input.Keys)
			{
				if (this.StatementReader.InputParamDefines.ContainsKey(key) == false) continue;

				statement.Append(string.Format("DECLARE @{0} {1}", key, this.StatementReader.InputParamDefines[key].Type));
				if ((this.StatementReader.InputParamDefines[key].Size.HasValue)
					|| (new SqlDbType[] { SqlDbType.Char, SqlDbType.VarChar, SqlDbType.NChar, SqlDbType.NVarChar }).Contains(this.StatementReader.InputParamDefines[key].Type))
				{
					statement.Append(string.Format("({0})", this.StatementReader.InputParamDefines[key].Size.HasValue ? this.StatementReader.InputParamDefines[key].Size.ToString() : "MAX"));
				}
				statement.Append(Environment.NewLine);
			}
			// SET文作成
			foreach (string key in input.Keys)
			{
				if (this.StatementReader.InputParamDefines.ContainsKey(key) == false) continue;

				var value = ((input[key] != null) && (input[key] != DBNull.Value)) ? string.Format("'{0}'", StringUtility.ToEmpty(input[key]).Replace("'", "''")) : "null";

				// 特殊文字(★♥♠♦等)は文字列4,000を超えてエラーが発生するので、UniCodeを指定して処理
				var isUnicode = this.StatementReader.InputParamDefines[key].Type == SqlDbType.NVarChar;
				var format = isUnicode ? "SET @{0} = N{1}" : "SET @{0} = {1}";

				statement.Append(string.Format(format, key, value));
				statement.Append(Environment.NewLine);
			}
			return statement.ToString();
		}
		#endregion

		#region -SetOutputParameters 出力パラメタセット
		/// <summary>
		/// 出力パラメタセット
		/// </summary>
		/// <param name="sqlCommand">SQLコマンド</param>
		private void SetOutputParameters(SqlCommand sqlCommand)
		{
			this.StatementReader.SetOutputParameters(sqlCommand);
		}
		#endregion

		#region -SetInputParameters 入力パラメタセット
		/// <summary>
		/// 入力パラメタセット
		/// </summary>
		/// <param name="sqlCommand">SQLコマンド</param>
		/// <param name="input">入力情報</param>
		private void SetInputParameters(SqlCommand sqlCommand, IDictionary input)
		{
			this.StatementReader.SetInputParameters(sqlCommand, input);
		}
		#endregion

		/// <summary>
		/// ステートメント実行・データセット取得（SQLパラメタあり）
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="input">入力パラメタ</param>
		/// <returns>デフォルトデータビュー</returns>
		public DataView SelectSingleStatement(SqlAccessor sqlAccessor, IDictionary input = null)
		{
			return SelectStatement(sqlAccessor, input).Tables[0].DefaultView;
		}

		/// <summary>
		/// ステートメント発行・データセット取得（SQLパラメタあり・コネクションオープンクローズあり）
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="input">入力パラメタ</param>
		/// <returns>デフォルトデータビュー</returns>
		public DataView SelectSingleStatementWithOC(SqlAccessor sqlAccessor, IDictionary input = null)
		{
			return SelectStatementWithOC(sqlAccessor, input).Tables[0].DefaultView;
		}

		/// <summary>
		/// ステートメント発行・データセット取得（コネクションオープンクローズあり）
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="input">入力パラメタ</param>
		/// <returns>データセット</returns>
		public DataSet SelectStatementWithOC(SqlAccessor sqlAccessor, IDictionary input = null)
		{
			sqlAccessor.OpenConnection();
			try
			{
				return SelectStatement(sqlAccessor, input);
			}
			finally
			{
				sqlAccessor.CloseConnection();
			}
		}

		/// <summary>
		/// ステートメント実行・影響行数取得（コネクションオープンクローズあり）
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="input">入力パラメタ</param>
		/// <returns>更新行数</returns>
		public int ExecStatementWithOC(SqlAccessor sqlAccessor, IDictionary input = null)
		{
			sqlAccessor.OpenConnection();
			try
			{
				return ExecStatement(sqlAccessor, input);
			}
			finally
			{
				sqlAccessor.CloseConnection();
			}
		}

		/// <summary>
		/// ステートメント実行・データセット取得
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="input">入力パラメタ</param>
		/// <returns>データセット</returns>
		public DataSet SelectStatement(SqlAccessor sqlAccessor, IDictionary input = null)
		{
			try
			{
				var begin = DateTime.Now;

				using (SqlCommand sqlCommand = CreateSqlCommand(this.Statement, input))
				{
					var ds = sqlAccessor.Select(sqlCommand);

					if (Constants.LOGGING_PERFORMANCE_SQL_ENABLED) WritePerformanceLog(begin);

					return ds;
				}
			}
			catch (Exception ex)
			{
				throw new w2Exception(CreateSqlExecErrorMessage(input), ex);
			}
		}

		/// <summary>
		/// ステートメント実行・影響件数取得
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>更新行数</returns>
		public int ExecStatement(SqlAccessor sqlAccessor)
		{
			return ExecStatement(sqlAccessor, null);
		}

		/// <summary>
		/// ステートメント実行・影響件数取得（SQLパラメタあり）
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="input">入力パラメタ</param>
		/// <returns>更新行数</returns>
		public int ExecStatement(SqlAccessor sqlAccessor, IDictionary input)
		{
			try
			{
				var begin = DateTime.Now;

				using (SqlCommand sqlCommand = CreateSqlCommand(this.Statement, input))
				{
					var result = sqlAccessor.Execute(sqlCommand);

					if (Constants.LOGGING_PERFORMANCE_SQL_ENABLED) WritePerformanceLog(begin);

					return result;
				}
			}
			catch (Exception ex)
			{
				throw new w2Exception(CreateSqlExecErrorMessage(input), ex);
			}
		}

		/// <summary>
		/// パフォーマンスログ書き込み
		/// </summary>
		/// <param name="begin">開始時間</param>
		public void WritePerformanceLog(DateTime begin)
		{
			PerformanceLogger.WriteForSql(begin, DateTime.Now, this.StatementReader.PageName + "->" + this.StatementReader.StatementName);
		}

		/// <summary>
		/// ステートメントに動的パラメタをセットする
		/// </summary>
		/// <param name="input">入力情報</param>
		/// <param name="parameters">入力パラメタ配列</param>
		/// <param name="dbType">パラメタDBタイプ</param>
		/// <param name="size">パラメタサイズ</param>
		public void SetDymamicParameters(Hashtable input, object[] parameters, SqlDbType dbType, int? size = null)
		{
			SetDymamicParameters(
				DEFAULT_DYNAMIC_PARAMS_REPLACE_STRING,
				DEFAULT_DYNAMIC_PARAM_PREFIX,
				input,
				parameters,
				dbType,
				size);
		}
		/// <summary>
		/// ステートメントに動的パラメタセットする
		/// </summary>
		/// <param name="replaceString">置換文字列</param>
		/// <param name="paramPrefix">パラメタのプリフィックス</param>
		/// <param name="input">入力情報</param>
		/// <param name="parameters">入力パラメタ配列</param>
		/// <param name="dbType">パラメタDBタイプ</param>
		/// <param name="size">パラメタサイズ</param>
		public void SetDymamicParameters(string replaceString, string paramPrefix, Hashtable input, object[] parameters, SqlDbType dbType, int? size = null)
		{
			var paramList = new List<string>();
			for (var i = 0; i < parameters.Length; i++)
			{
				var param = paramPrefix + i;
				input[param] = parameters[i];
				this.AddInputParameters(param, dbType, size);
				paramList.Add("@" + param);
			}
			this.Statement = this.Statement.Replace(replaceString, string.Join(",", paramList));
		}

		/// <summary>
		/// SQL実行エラーメッセージ取得
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public string CreateSqlExecErrorMessage(IDictionary input)
		{
			var message = new StringBuilder();
			message.Append("ステートメント").Append(this.StatementReader.XmlPath).Append("で例外が発生しました。").Append(Environment.NewLine);
			message.Append("---デバッグ用SQLパラメタ----").Append(Environment.NewLine);
			message.Append(CreateParamDefineStatement(input)).Append(Environment.NewLine);
			message.Append("-------").Append(Environment.NewLine);
			message.Append(this.Statement).Append(Environment.NewLine);
			message.Append("-------").Append(Environment.NewLine);
			return message.ToString();
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			this.StatementReader = null;
		}

		#region -GetSqlDbType SqlDbType取得
		/// <summary>
		/// SqlDbType取得
		/// </summary>
		/// <param name="dbTypeString">DBタイプ文字列</param>
		private static SqlDbType GetSqlDbType(string dbTypeString)
		{
			switch (dbTypeString.ToUpper())
			{
				case "BIGINT":
					return SqlDbType.BigInt;
				case "BINARY":
					return SqlDbType.Binary;
				case "BIT":
					return SqlDbType.Bit;
				case "CHAR":
					return SqlDbType.Char;
				case "DATETIME":
					return SqlDbType.DateTime;
				case "DECIMAL":
					return SqlDbType.Decimal;
				case "FLOAT":
					return SqlDbType.Float;
				case "IMAGE":
					return SqlDbType.Image;
				case "INT":
					return SqlDbType.Int;
				case "MONEY":
					return SqlDbType.Money;
				case "NCHAR":
					return SqlDbType.NChar;
				case "NTEXT":
					return SqlDbType.NText;
				case "NVARCHAR":
					return SqlDbType.NVarChar;
				case "REAL":
					return SqlDbType.Real;
				case "SMALLDATETIME":
					return SqlDbType.SmallDateTime;
				case "SMALLINT":
					return SqlDbType.SmallInt;
				case "SMALLMONEY":
					return SqlDbType.SmallMoney;
				case "TEXT":
					return SqlDbType.Text;
				case "TIMESTAMP":
					return SqlDbType.Timestamp;
				case "TINYINT":
					return SqlDbType.TinyInt;
				case "UNIQUEIDENTIFIER":
					return SqlDbType.UniqueIdentifier;
				case "VARBINARY":
					return SqlDbType.VarBinary;
				case "VARCHAR":
					return SqlDbType.VarChar;
				case "VARIANT":
					return SqlDbType.Variant;
				default:
					throw new w2Exception("指定されたDbTypeが取得できません。");
			}
		}
		#endregion

		/// <summary>SQLステートメントリーダー</summary>
		public SqlStatementReader StatementReader { get; private set; }
		/// <summary>SQLステートメント</summary>
		public string Statement
		{
			get { return this.StatementReader.Statement; }
			set { this.StatementReader.Statement = value; }
		}
		/// <summary>コマンドタイムアウト</summary>
		public int CommandTimeout { get; set; }
		/// <summary>リテラルSQLを利用するか（trueにするとパラメタがバインドされず、クエリの先頭でDALCARE宣言される）</summary>
		public bool UseLiteralSql { get; set; }
	}
}
