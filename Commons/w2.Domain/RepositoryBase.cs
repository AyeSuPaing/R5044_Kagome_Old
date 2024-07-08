/*
=========================================================================================================
  Module      : リポジトリ基底クラス(RepositoryBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using w2.Common.Extensions;
using w2.Common.Sql;

namespace w2.Domain
{
	/// <summary>
	/// リポジトリ基底クラス
	/// </summary>
	public class RepositoryBase : IDisposable
	{
		/// <summary>
		/// ステートメントが記載されているXMLファイルが格納されているディレクトリのパス
		/// </summary>
		protected static class XmlPath
		{
			/// <summary>デバッグ用ファイルパス</summary>
			public static string Debug
				= Path.Combine(Directory.GetParent(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE).Parent.FullName, "w2.Domain", "_Repository");
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RepositoryBase(SqlAccessor accessor = null)
		{
			if (accessor == null)
			{
				this.Accessor = new SqlAccessor();
				this.Accessor.OpenConnection();
			}
			else
			{
				this.Accessor = accessor;
				this.HasExternalAccesssor = true;
			}
		}

		/// <summary>
		/// データを取得する
		/// </summary>
		/// <param name="pageName">対象テーブル名（XMLファイル名）</param>
		/// <param name="statementName">ステートメント名</param>
		/// <param name="input">入力値</param>
		/// <param name="dymamicParametersTuple"></param>
		/// <param name="replaces">置換値</param>
		/// <returns>データ</returns>
		/// <remarks>
		/// リリースビルド時はDLLリソースからXMLを読み込むが、
		/// デバッグビルド時は開発効率化のためローカルXMLを読み込む。
		/// </remarks>
		protected DataView Get(
			string pageName,
			string statementName,
			Hashtable input = null,
			Tuple<object[], SqlDbType, int?> dymamicParametersTuple = null,
			params KeyValuePair<string, string>[] replaces)
		{
			using (var statement = GetSqlStatement(pageName, statementName, replaces))
			{
				if (this.CommandTimeout.HasValue) statement.CommandTimeout = this.CommandTimeout.Value;

				if (dymamicParametersTuple != null)
				{
					statement.SetDymamicParameters(
						input,
						dymamicParametersTuple.Item1,
						dymamicParametersTuple.Item2,
						dymamicParametersTuple.Item3);
				}
				var dv = statement.SelectSingleStatement(this.Accessor, input);
				return dv;
			}
		}

		/// <summary>
		/// データを取得する
		/// </summary>
		/// <param name="statementInfos">ステートメント情報リスト</param>
		/// <param name="input">入力値</param>
		/// <param name="replaces">置換値</param>
		/// <returns>データ</returns>
		/// <remarks>
		/// リリースビルド時はDLLリソースからXMLを読み込むが、
		/// デバッグビルド時は開発効率化のためローカルXMLを読み込む。
		/// </remarks>
		public DataSet GetWithChilds(KeyValuePair<string, string>[] statementInfos, Hashtable input = null, params KeyValuePair<string, string>[] replaces)
		{
			var sqlStatements = statementInfos.Select(item => GetSqlStatement(item.Key, item.Value)).ToArray();

			using (var statementForExec = GetSqlStatement(statementInfos[0].Key, statementInfos[0].Value))
			{
				if (this.CommandTimeout.HasValue) statementForExec.CommandTimeout = this.CommandTimeout.Value;
				statementForExec.Statement = string.Join("\r\n", sqlStatements.Select(s => s.Statement));
				statementForExec.StatementReader.StatementName = string.Join(",", statementInfos.Select(kvp => string.Format("{0}^>{1}", kvp.Key, kvp.Value)));
				sqlStatements.ToList().ForEach(
					statement =>
					{
						statement.StatementReader.InputParamDefines.ToList().ForEach(
							p =>
							{
								if (statementForExec.StatementReader.InputParamDefines.ContainsKey(p.Key) == false)
								{
									statementForExec.AddInputParameters(p.Value.Name, p.Value.Type, p.Value.Size);
								}
							});
					});

				foreach (var replace in replaces)
				{
					statementForExec.Statement = statementForExec.Statement.Replace(replace.Key, replace.Value);
				}
				var ds = statementForExec.SelectStatement(this.Accessor, input);
				return ds;
			}
		}

		/// <summary>
		/// ストリームによりデータを取得する（遅延評価）
		/// </summary>
		/// <param name="pageName">対象テーブル名（XMLファイル名）</param>
		/// <param name="statementName">ステートメント名</param>
		/// <param name="input">入力値</param>
		/// <param name="replaces">置換値</param>
		/// <returns>データ</returns>
		/// <remarks>
		/// リリースビルド時はDLLリソースからXMLを読み込むが、
		/// デバッグビルド時は開発効率化のためローカルXMLを読み込む。
		/// </remarks>
		protected IEnumerable<Hashtable> GetByDataReader(string pageName, string statementName, Hashtable input = null, params KeyValuePair<string, string>[] replaces)
		{
			using (var statement = GetSqlStatement(pageName, statementName))
			{
				if (this.CommandTimeout.HasValue) statement.CommandTimeout = this.CommandTimeout.Value;
				foreach (var replace in replaces)
				{
					statement.Statement = statement.Statement.Replace(replace.Key, replace.Value);
				}

				using (var reader = new SqlStatementDataReader(this.Accessor, statement, true))
				{
					while (reader.Read())
					{
						var dict = Enumerable.Range(0, reader.FieldCount).ToDictionary(
							i => reader.GetName(i),
							i => ((reader[i] == DBNull.Value) ? null : reader[i]));
						var ht = new Hashtable(dict);
						yield return ht;
					}
				}
			}
		}

		/// <summary>
		/// リーダーを通してデータを取得する ※リーダー閉じ忘れ注意※
		/// </summary>
		/// <param name="pageName">対象テーブル名（XMLファイル名）</param>
		/// <param name="statementName">ステートメント名</param>
		/// <param name="input">入力値</param>
		/// <param name="replaces">置換値</param>
		/// <returns>データ</returns>
		/// <remarks>
		/// リリースビルド時はDLLリソースからXMLを読み込むが、
		/// デバッグビルド時は開発効率化のためローカルXMLを読み込む。
		/// </remarks>
		protected SqlStatementDataReader GetWithReader(string pageName, string statementName, Hashtable input = null, params KeyValuePair<string, string>[] replaces)
		{
			using (var statement = GetSqlStatement(pageName, statementName, replaces))
			{
				if (this.CommandTimeout.HasValue) statement.CommandTimeout = this.CommandTimeout.Value;

				// CsMessageのときはSQLパラメータリストに追加
				var paramList = (List<SqlStatement.SqlParam>)input["sqlParamList"];
				if (paramList != null)
				{
					foreach (SqlStatement.SqlParam param in paramList)
					{
						statement.AddInputParameters(param.Name, param.Type, param.Size);
					}
				}

				var reader = new SqlStatementDataReader(this.Accessor, statement, input, true);
				return reader;
			}
		}

		/// <summary>
		/// リテラルSQLでデータを取得する
		/// </summary>
		/// <param name="pageName">対象テーブル名（XMLファイル名）</param>
		/// <param name="statementName">ステートメント名</param>
		/// <param name="input">入力値</param>
		/// <param name="dymamicParametersTuple"></param>
		/// <param name="replaces">置換値</param>
		/// <returns>データ</returns>
		/// <remarks>
		/// <para>
		/// リリースビルド時はDLLリソースからXMLを読み込むがデバッグビルド時は開発効率化のためローカルXMLを読み込む。
		/// </para>
		/// <para>
		/// 多い件数を取得する差異は通常使用しているパラメタライズドクエリよりリテラルSQLを使用したほうが処理は早くなるが
		/// フロントで使用する際はSQLインジェクションの危険性もあるためセキュリティに気を付けて使用すること
		/// </para>
		/// </remarks>
		protected DataView GetUseLiteralSql(
			string pageName,
			string statementName,
			Hashtable input = null,
			Tuple<object[], SqlDbType, int?> dymamicParametersTuple = null,
			params KeyValuePair<string, string>[] replaces)
		{
			using (var statement = GetSqlStatement(pageName, statementName, replaces))
			{
				statement.UseLiteralSql = true;
				if (this.CommandTimeout.HasValue) statement.CommandTimeout = this.CommandTimeout.Value;

				if (dymamicParametersTuple != null)
				{
					statement.SetDymamicParameters(
						input,
						dymamicParametersTuple.Item1,
						dymamicParametersTuple.Item2,
						dymamicParametersTuple.Item3);
				}
				var dv = statement.SelectSingleStatement(this.Accessor, input);
				return dv;
			}
		}

		/// <summary>
		/// 実行する
		/// </summary>
		/// <param name="pageName">対象テーブル名（XMLファイル名）</param>
		/// <param name="statementName">ステートメント名</param>
		/// <param name="input">入力値</param>
		/// <param name="replaces">置換値</param>
		/// <returns>データ</returns>
		/// <remarks>
		/// リリースビルド時はDLLリソースからXMLを読み込むが、
		/// デバッグビルド時は開発効率化のためローカルXMLを読み込む。
		/// </remarks>
		protected int Exec(string pageName, string statementName, Hashtable input = null, params KeyValuePair<string, string>[] replaces)
		{
			using (var statement = GetSqlStatement(pageName, statementName, replaces))
			{
				var updated = statement.ExecStatement(this.Accessor, input);
				return updated;
			}
		}

		/// <summary>
		/// SqlStatementを取得する ※閉じ忘れ注意※
		/// </summary>
		/// <param name="pageName">対象テーブル名（XMLファイル名）</param>
		/// <param name="statementName">ステートメント名</param>
		/// <param name="replaces">置換値</param>
		/// <returns>データ</returns>
		/// <remarks>
		/// リリースビルド時はDLLリソースからXMLを読み込むが、
		/// デバッグビルド時は開発効率化のためローカルXMLを読み込む。
		/// </remarks>
		protected SqlStatement GetSqlStatement(string pageName, string statementName, params KeyValuePair<string, string>[] replaces)
		{
#if DEBUG
			var statement = new SqlStatement(XmlPath.Debug, pageName, statementName);
#else
			var statement = new SqlStatement(Properties.Resources.ResourceManager, pageName, pageName, statementName);
#endif
			foreach (var replace in replaces)
			{
				statement.Statement = statement.Statement.Replace(replace.Key, replace.Value);
			}
			return statement;
		}

		/// <summary>
		/// 配列の値からIN句を生成する
		/// </summary>
		/// <param name="tableName">対象テーブル名</param>
		/// <param name="fieldName">対象フィールド名</param>
		/// <param name="selectableValue">対象値配列</param>
		/// <returns>配列の値に対応したIN句</returns>
		protected string CreateWhereInStatementReplaces(
			string tableName,
			string fieldName,
			IEnumerable<string> selectableValue)
		{
			var targetField = string.Format("{0}.{1}", tableName, fieldName);
			return CreateWhereInStatementReplaces(targetField, selectableValue);
		}
		/// <summary>
		/// 配列の値からIN句を生成する
		/// </summary>
		/// <param name="fieldName">対象フィールド名</param>
		/// <param name="selectableValue">対象値配列</param>
		/// <returns>配列の値に対応したIN句</returns>
		protected string CreateWhereInStatementReplaces(string fieldName, IEnumerable<string> selectableValue)
		{
			if (selectableValue.Any() == false) return string.Empty;

			var statement = string.Format(
				"{0} IN ({1})",
				fieldName,
				selectableValue.Select(value => string.Format("'{0}'", value)).JoinToString(","));
			return statement;
		}

		/// <summary>
		/// 開放処理
		/// </summary>
		public void Dispose()
		{
			if (this.HasExternalAccesssor == false) this.Accessor.Dispose();
		}

		/// <summary>タイムアウト（秒）</summary>
		public int? CommandTimeout { get; set; }
		/// <summary>SQLアクセサ</summary>
		public SqlAccessor Accessor { get; private set; }
		/// <summary>外部から渡されたアクセサを持つか</summary>
		private bool HasExternalAccesssor { get; set; }
	}
}
