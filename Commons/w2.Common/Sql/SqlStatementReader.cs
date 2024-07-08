/*
=========================================================================================================
  Module      : SQLステートメントリーダー(SqlStatementReader.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Resources;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using w2.Common.Util;

namespace w2.Common.Sql
{
	/// <summary>
	/// SQLステートメントリーダー
	/// </summary>
	public class SqlStatementReader
	{
		#region 列挙体・定数
		// ステートメントタグ
		private const string NODENAME_STATEMENT = "Statement";
		#endregion

		/// <summary>XMLドキュメントキャッシュ</summary>
		private static Dictionary<string, XDocument> m_xmlDocsCache = new Dictionary<string, XDocument>();
		/// <summary>スレッドセーフ保つためのロックオブジェクト</summary>
		private static readonly object m_lockObject = new object();
		/// <summary>XMLファイルパターン（ワイルドカード）</summary>
		private const string XML_FILE_PATTERN = @"*.xml";

		#region コンストラクタ
		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static SqlStatementReader()
		{
			// ディレクトリ名補正
			lock (Constants.PHYSICALDIRPATH_SQL_STATEMENT)
			{
				if (Constants.PHYSICALDIRPATH_SQL_STATEMENT.EndsWith(@"\") == false)
				{
					Constants.PHYSICALDIRPATH_SQL_STATEMENT += @"\";
				}
			}
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statementDirPath">SQLステートメントディレクトリパス</param>
		/// <param name="pageName">ステートメントページ名</param>
		/// <param name="statementName">ステートメント名</param>
		internal SqlStatementReader(string statementDirPath, string pageName, string statementName)
		{
			// AddObserverIfNotAdded監視追加
			AddObserverIfNotAdded(statementDirPath);

			this.StatementDirPath = statementDirPath;
			this.PageName = pageName;
			this.StatementName = statementName;

			ReadStatement();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="resourceManager">リソースマネージャ</param>
		/// <param name="resourceName">リソース名</param>
		/// <param name="pageName">ステートメントページ名</param>
		/// <param name="statementName">ステートメント名</param>
		internal SqlStatementReader(ResourceManager resourceManager, string resourceName, string pageName, string statementName)
		{
			this.ResourceManager = resourceManager;
			this.ResourceName = resourceName;
			this.PageName = pageName;
			this.StatementName = statementName;

			ReadStatement();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statement">SQLステートメント</param>
		internal SqlStatementReader(string statement)
		{
			this.Statement = statement;
		}
		#endregion

		/// <summary>
		/// 追加されていなければ監視追加
		/// </summary>
		/// <param name="dirPath">ディレクトリパス</param>
		private static void AddObserverIfNotAdded(string dirPath)
		{
			if (Directory.Exists(dirPath))
			{
				var observer = FileUpdateObserver.GetInstance();
				if (observer.Contains(dirPath, XML_FILE_PATTERN) == false) observer.AddObservation(dirPath, XML_FILE_PATTERN, ResetCache);
			}
		}

		/// <summary>
		/// キャッシュリセット
		/// </summary>
		private static void ResetCache()
		{
			lock (m_lockObject)
			{
				m_xmlDocsCache = new Dictionary<string, XDocument>();
			}
		}

		#region -ReadStatement ステートメント読み込み（サブステートメントも展開）
		/// <summary>
		/// ステートメント読み込み（サブステートメントも展開）
		/// </summary>
		private void ReadStatement()
		{
			if (this.ResourceManager != null)
			{
				var xml = GetXml(this.ResourceManager, this.ResourceName);
				ReadMainStatement(xml, this.StatementName);
				ReplaceSubStatement();
			}
			else if (this.StatementDirPath != null)
			{
				var xml = GetXml(this.StatementDirPath, this.PageName);
				ReadMainStatement(xml, this.StatementName);
				ReplaceSubStatement();
			}
			else
			{
				throw new Exception("ReadStatementに失敗しました。");
			}
		}
		#endregion

		#region -ReadMainStatement メインのステートメント読み込み
		/// <summary>
		/// メインのステートメント読み込み
		/// </summary>
		/// <param name="statementXml">ステートメントXML</param>
		/// <param name="statementName">ステートメント名</param>
		private void ReadMainStatement(XDocument statementXml, string statementName)
		{
			try
			{
				// ステートメント取得
				var statementNode = statementXml.Root.Element(statementName);
				this.Statement = statementNode.Element(NODENAME_STATEMENT).Value;

				// 入出力パラメタ取得
				this.InputParamDefines = SqlStatement.GetParamDefinition(statementNode, "./Parameter/Input");
				this.OutParamDefines = SqlStatement.GetParamDefinition(statementNode, "./Parameter/Output");
			}
			catch (Exception ex)
			{
				throw new w2Exception(string.Format("XML「{0}.xml」中ノード「{0}/{1}」の読み込みに失敗しました。", this.PageName, this.StatementName), ex);
			}
		}
		#endregion

		#region -ReplaceSubStatement サブステートメント置換
		/// <summary>
		/// サブステートメント置換
		/// </summary>
		private void ReplaceSubStatement()
		{
			if (this.Statement.Contains("[[") == false) return;

			if (this.ResourceManager != null)
			{
				var xml = GetXml(this.ResourceManager, this.ResourceName + "_Sub");
				ReplaceSubStatement(xml, this.PageName + "_Sub");
			}
			else if (this.StatementDirPath != null)
			{
				var xml = GetXml(this.StatementDirPath, this.PageName + "_Sub");
				ReplaceSubStatement(xml, this.PageName + "_Sub");
			}
		}
		/// <summary>
		/// サブステートメント置換
		/// </summary>
		/// <param name="subStatementXml">サブステートメントXML</param>
		/// <param name="pageNameSub">サブステートメントXML名</param>
		/// <remarks>「[[」がなくなるまで再帰処理</remarks>
		private void ReplaceSubStatement(XDocument subStatementXml, string pageNameSub)
		{
			string pageNameAndStatementName = null;
			try
			{
				// サブステートメントは「[[ ABCDE ]]」で表される。
				foreach (Match mSubStatementTag in Regex.Matches(this.Statement, @"\[\[ .*? \]\]"))
				{
					pageNameAndStatementName = mSubStatementTag.Value.Substring(3, mSubStatementTag.Value.Length - 6);
					var targetSubStatement = subStatementXml.Root.Element(pageNameAndStatementName);

					this.Statement = this.Statement.Replace(
						mSubStatementTag.Value,
						targetSubStatement.Element(NODENAME_STATEMENT).Value);

					// サブステートメントからも入出力パラメタ取得
					this.InputParamDefines =
						SqlStatement.GetParamDefinition(targetSubStatement, "./Parameter/Input")
							.Union(this.InputParamDefines)
							.ToDictionary(k => k.Key, v => v.Value);
					this.OutParamDefines =
						SqlStatement.GetParamDefinition(targetSubStatement, "./Parameter/Output")
							.Union(this.OutParamDefines)
							.ToDictionary(k => k.Key, v => v.Value);
				}

				// 「[[」が残っていれば再帰呼び出し
				if (this.Statement.Contains("[["))
				{
					ReplaceSubStatement(subStatementXml, pageNameSub);
				}
			}
			catch (Exception ex)
			{
				// XML読み込み時に例外発生
				throw new w2Exception("XML「" + pageNameSub + ".xml」中ノード「" + pageNameAndStatementName + "」の読み込みに失敗しました。", ex);
			}
		}
		#endregion

		#region -GetXml XML取得
		/// <summary>
		/// XML取得
		/// </summary>
		/// <param name="statementDirPath">ステートメントディレクトリパス</param>
		/// <param name="pageName">ステートメントページ名</param>
		/// <returns>ステートメントXML</returns>
		private XDocument GetXml(string statementDirPath, string pageName)
		{
			var filePath = Path.Combine(statementDirPath, pageName + ".xml");
			try
			{
				lock (m_lockObject)
				{
					// なければステートメントXML読み込み
					if (m_xmlDocsCache.ContainsKey(filePath) == false)
					{
						var xmlDoc = XDocument.Load(filePath);
						m_xmlDocsCache.Add(filePath, xmlDoc);
					}
					return m_xmlDocsCache[filePath];
				}
			}
			catch (IOException)
			{
				throw new w2Exception("XML「" + filePath + "」の読み込みに失敗しました。");
			}
		}
		/// <summary>
		/// XML取得
		/// </summary>
		///<param name="resourceManager">リソースマネージャー</param>
		///<param name="resourceName">リソース名</param>
		/// <returns>ステートメントXML</returns>
		private XDocument GetXml(ResourceManager resourceManager, string resourceName)
		{
			var dicKey = resourceManager.BaseName + "->" + resourceName;
			try
			{
				lock (m_lockObject)
				{
					// なければステートメントXML読み込み
					if (m_xmlDocsCache.ContainsKey(dicKey) == false)
					{
						var xmlString = resourceManager.GetString(resourceName);
						if (xmlString == null) throw new w2Exception("リソースファイル「" + resourceName + "」の読み込みに失敗しました。");
						var xmlDoc = XDocument.Parse(xmlString);
						m_xmlDocsCache.Add(dicKey, xmlDoc);
					}
					return m_xmlDocsCache[dicKey];
				}
			}
			catch (IOException)
			{
				throw new w2Exception("リソースファイル「" + resourceName + "」のXML変換に失敗しました。");
			}
		}
		#endregion

		#region ~SetOutputParameters 出力パラメタセット
		/// <summary>
		/// 出力パラメタセット
		/// </summary>
		/// <param name="sqlCommand">SQLコマンド</param>
		internal void SetOutputParameters(SqlCommand sqlCommand)
		{
			if (this.OutParamDefines == null) return;

			foreach (var param in this.OutParamDefines.Values)
			{
				var parameter = new SqlParameter("@" + param.Name, param.Type);
				if (param.Size.HasValue) parameter.Size = param.Size.Value;
				if (param.Precision.HasValue) parameter.Precision = param.Precision.Value;
				if (param.Scale.HasValue) parameter.Scale = param.Scale.Value;
				parameter.Direction = ParameterDirection.Output;

				sqlCommand.Parameters.Add(parameter);
			}
		}
		#endregion

		#region ~SetInputParameters 入力パラメタセット
		/// <summary>
		/// 入力パラメタセット
		/// </summary>
		/// <param name="sqlCommand">SQLコマンド</param>
		/// <param name="input">入力情報</param>
		internal void SetInputParameters(SqlCommand sqlCommand, IDictionary input)
		{
			if (this.InputParamDefines == null) return;

			foreach (var param in this.InputParamDefines.Values)
			{
				// 入力ハッシュにデータがあるもののみパラメタ追加（カラ定義可能）
				if (input.Contains(param.Name))
				{
					object value = StringUtility.ToValue(input[param.Name], DBNull.Value);
					// Decimal型の値を数字に置換する（カンマ付きの値がDecimal型に入っているとSQLServerでエラーが発生するため）
					if (param.Type == SqlDbType.Decimal)
					{
						decimal temp;
						if (decimal.TryParse(StringUtility.ToEmpty(value), out temp)) value = temp;
					}

					// サイズ指定あり？
					if (param.Size.HasValue)
					{
						var paramter = new SqlParameter("@" + param.Name, param.Type, param.Size.Value);
						paramter.Value = value;
						sqlCommand.Parameters.Add(paramter);
					}
					// サイズ指定なしの場合は型指定なしで登録する（intの場合でも""でパラメタが渡ってくる場合があるため）
					else
					{
						sqlCommand.Parameters.Add(new SqlParameter("@" + param.Name, value));
					}
				}
			}
		}
		#endregion

		#region ~ReplaceStatementString
		/// <summary>
		/// ステートメント置換（サブステートメント埋め込みも行う）
		/// </summary>
		/// <param name="oldValue">置換前文字列</param>
		/// <param name="newValue">置換後文字列</param>
		internal void ReplaceStatementString(string oldValue, string newValue)
		{
			this.Statement = this.Statement.Replace(oldValue, newValue);

			// サブステートメント置換も行う
			ReplaceSubStatement();
		}
		#endregion

		#region プロパティ
		/// <summary>リソースマネージャ（リソースを利用する場合にセット）</summary>
		internal ResourceManager ResourceManager { get; set; }
		/// <summary>リソース名（リソースを利用する場合にセット）</summary>
		internal string ResourceName { get; set; }
		/// <summary>ステートメントディレクトリパス（外部XMLファイルを利用する場合にセット）</summary>
		internal string StatementDirPath { get; set; }
		/// <summary>ステートメントページ名</summary>
		internal string PageName { get; set; }
		/// <summary>ステートメント名</summary>
		public string StatementName { get; set; }
		/// <summary>ステートメント</summary>
		internal string Statement { get; set; }
		/// <summary>SQLステートメントパス</summary>
		internal string XmlPath
		{
			get
			{
				return string.Format("{0}/{1}", this.PageName, this.StatementName);
			}
		}
		/// <summary>Inputパラメタ定義</summary>
		public Dictionary<string, SqlStatement.SqlParam> InputParamDefines { get; set; }
		/// <summary>Outputパラメタ定義</summary>
		public Dictionary<string, SqlStatement.SqlParam> OutParamDefines { get; set; }
		#endregion
	}
}
