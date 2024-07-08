/*
=========================================================================================================
  Module      : 出力基底クラス(ActionExportBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using w2.App.Common.Extensions.Currency;
using w2.Commerce.Batch.CustomerRingsExport.Util;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.Commerce.Batch.CustomerRingsExport.Action
{
	public abstract class ActionExportBase
	{
		/// <summary>SQLタイムアウト値設定</summary>
		private const int SQL_COMMAND_TIMEOUT = 600;
		/// <summary>出力するフィールド設定 </summary>
		private const string EXPORT_FIELDS = "EXPORT_FIELDS";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="define">出力ファイル定義</param>
		protected ActionExportBase(FileDefines.FileDefine define)
		{
			this.Define = define;
			this.Target = define.Name;
			this.StartDateTime = DateTime.Now;
			this.FileChecker = new Checker(define.Name);
			this.LastExecuteDateTime = this.FileChecker.GetLastExecuteDateTime();
			this.Count = 0;
			this.IsSuccess = false;
			this.MailMessage = new StringBuilder();

			this.StatementFile = define.Name;
			this.FileName = string.Format("{0}_{1}.csv", define.Name, DateTime.Now.ToString("yyyyMMddHHmmss"));

			// 出力先ディレクトリ作成
			if (Directory.Exists(this.ExportDirectory) == false)
			{
				Directory.CreateDirectory(this.ExportDirectory);
			}
			// バックアップディレクトリ作成
			if (Directory.Exists(this.ExportDirectory + @"\Backup") == false)
			{
				Directory.CreateDirectory(this.ExportDirectory + @"\Backup");
			}

			WriteLogAndMessage("処理区分：" + this.Target);
			WriteLogAndMessage("出力開始時間：" + this.StartDateTime);
		}

		/// <summary>
		/// 処理実行
		/// </summary>
		public virtual void Execute()
		{
			var csvLineCount = 0;
			using (var writer = new FileWriter(Path.Combine(this.ExportDirectory, this.FileName), FileWriter.ENCODING_UTF8, true))
			{
				// 出力データ取得
				foreach (var csvLine in GetExportContent(GetExportData()))
				{
					writer.Write(csvLine);

					// 進捗
					csvLineCount++;
					if (csvLineCount % 1000 == 0)
					{
						Console.WriteLine("出力済みCSV行数: {0} {1}", this.Target, csvLineCount);
					}
				}
			}
			this.Count = csvLineCount - 1;

			//　tmpファイル更新
			this.FileChecker.CreateEndFile(this.StartDateTime);

			// 終了時間
			this.EndDateTime = DateTime.Now;

			WriteLogAndMessage("出力終了時間：" + this.EndDateTime);
			WriteLogAndMessage("ファイル名：" + this.FileName);
			WriteLogAndMessage("件数：" + this.Count);

			// FTPSアップロード
			if (FtpsUpload() == false) return;

			// 成功結果の更新
			this.IsSuccess = true;
		}

		/// <summary>
		/// 出力データ取得（全件か、差分か判定して出力）
		/// </summary>
		/// <returns>出力データ</returns>
		protected virtual IEnumerable<OrderedDictionary> GetExportData()
		{
			return this.LastExecuteDateTime.HasValue ? GetExportDataDifferential() : GetExportDataAll();
		}

		/// <summary>
		/// 出力データ取得（全件）
		/// </summary>
		/// <returns>出力データ</returns>
		protected IEnumerable<OrderedDictionary> GetExportDataAll()
		{
			return GetExportReader("ExportAll");
		}

		/// <summary>
		/// 出力データ取得（差分）
		/// </summary>
		/// <returns>出力データ</returns>
		protected IEnumerable<OrderedDictionary> GetExportDataDifferential()
		{
			// パラメータ
			Hashtable parameter = new Hashtable
			{
				{ "date_changed", this.LastExecuteDateTime }
			};
			return GetExportReader("ExportDifferential", parameter);
		}

		/// <summary>
		/// 出力データ取得（Reader）
		/// </summary>
		/// <param name="statementName">ステートメント名</param>
		/// <param name="parameter">パラメータ</param>
		/// <returns>出力データ</returns>
		protected IEnumerable<OrderedDictionary> GetExportReader(string statementName, Hashtable parameter = null)
		{
			var xdQuerySetting = CreateExportFieldsList(this.StatementFile);
			var strExportFieldsValue = xdQuerySetting
				.Element(this.StatementFile)
				.Element(EXPORT_FIELDS)
				.Value;
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement(this.StatementFile, statementName))
			{
				statement.CommandTimeout = SQL_COMMAND_TIMEOUT;
				statement.Statement = statement.Statement.Replace("@@ " + EXPORT_FIELDS + " @@", strExportFieldsValue);
				statement.Statement = statement.Statement.Replace("@@ fields @@", this.ExportFieldString);
				using (SqlStatementDataReader reader = new SqlStatementDataReader(accessor, statement, parameter, true))
				{
					// ヘッダー
					{
						var header = new OrderedDictionary();
						foreach (int i in Enumerable.Range(0, reader.FieldCount))
						{
							header.Add(i.ToString(), reader.GetName(i));
						}
						yield return header;
					}

					// データ
					while (reader.Read())
					{
						var row = new OrderedDictionary();
						foreach (int i in Enumerable.Range(0, reader.FieldCount))
						{
							// 金額でかつnullではない場合、決済通貨に合わせて小数点以下を補正
							var value = ((reader.GetFieldType(i) == typeof(decimal))
								&& (reader[i] != DBNull.Value))
									? reader[i].ToPriceDecimal().Value
									: reader[i];

							// Fieldが重複しているとエラーが出るので、定義するときに注意する。
							row.Add(reader.GetName(i), value);
						}
						yield return row;
					}
				}
			}
		}

		/// <summary>
		/// 出力用パーツを生成する
		/// </summary>
		/// <param name="exportFieldsListFileName">ExportFieldsが存在するファイル名</param>
		/// <returns>出力用パーツ</returns>
		private XDocument CreateExportFieldsList(string exportFieldsListFileName)
		{
			var filePath = Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE
				+ @"CustomerRingsExport\{0}\ExportFieldsList\"
				+ exportFieldsListFileName
				+ ".xml";
			var storeDirectory = File.Exists(string.Format(filePath, Constants.PROJECT_NO)) ? Constants.PROJECT_NO : "Default";
			var exportFieldsListFilePath = string.Format(filePath, storeDirectory);

			try
			{
				return XDocument.Load(exportFieldsListFilePath);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("ファイル「" + exportFieldsListFilePath + "」の読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// 出力データ取得
		/// </summary>
		/// <returns>出力データ（リスト）</returns>
		private IEnumerable<string> GetExportContent(IEnumerable<OrderedDictionary> content)
		{
			foreach (var row in content)
			{
				// 1行目はヘッダー部分
				if (this.Define.ExportHeader == false) continue;

				yield return GetExportLine(row.Values.Cast<object>().ToArray<object>());
			}
		}

		/// <summary>
		/// 行データ取得
		/// </summary>
		/// <param name="content">出力データ</param>
		/// <returns>出力データ（1行）</returns>
		private string GetExportLine(object[] content)
		{
			return string.Concat(
				string.Join(
					this.Define.FieldSeparator,
					content.Select(item => string.Format(
						"{0}{1}{2}",
						this.Define.Enclosed,
						this.Define.IsEscapeCsv
							? StringUtility.EscapeCsvColumn(StringUtility.ToEmpty(item), this.Define.NewLineReplaceString)
							: StringUtility.ToEmpty(item),
						this.Define.Enclosed))),
				"\r\n");
		}

		/// <summary>
		/// FTPSアップロード
		/// </summary>
		/// <returns>結果</returns>
		protected bool FtpsUpload()
		{
			bool result = true;
			WriteLogAndMessage("転送開始時間：" + DateTime.Now);

			var fileNames = new StringBuilder();

			// ディレクトリ上にあるすべてのファイルをアップロードする
			foreach (var file in Directory.GetFiles(this.ExportDirectory, string.Format("{0}_*.csv", this.Target)))
			{
				var directory = file.Split('\\');
				var fileName = directory[directory.Length - 1];

				// ファイルアップロード
				var ftps = new FtpsUploadUtility();
				if (ftps.ExecUpload("ftp://" + Constants.CUSTOMER_RINGS_HOST + "/" + fileName, file, Constants.CUSTOMER_RINGS_USER_NAME, Constants.CUSTOMER_RINGS_PASSWORD))
				{
					fileNames.Append(fileName).Append("\r\n");

					// バックアップに移動
					MoveFile(file, Path.Combine(this.ExportDirectory, @"Backup", fileName));
				}
				else
				{
					result = false;
				}
			}

			WriteLogAndMessage("転送終了時間：" + DateTime.Now);
			WriteLogAndMessage("転送ファイル：" + fileNames.ToString());

			return result;
		}

		/// <summary>
		/// ファイル移動
		/// </summary>
		/// <param name="sourceFilePath">移動元のファイルパス</param>
		/// <param name="destFilePath">移動先のファイルパス</param>
		protected void MoveFile(string sourceFilePath, string destFilePath)
		{
			// 移動先に同じファイルがあれば、削除
			if (File.Exists(destFilePath))
			{
				File.Delete(destFilePath);
			}

			// ファイル移動
			File.Move(sourceFilePath, destFilePath);
		}

		/// <summary>
		/// ログ出力とメッセージ追加
		/// </summary>
		/// <param name="message">メッセージ</param>
		protected void WriteLogAndMessage(string message)
		{
			FileLogger.WriteInfo(message);
			this.MailMessage.Append(message).Append("\r\n");
		}

		#region Property
		/// <summary>処理ターゲット</summary>
		protected string Target { get; private set; }
		/// <summary>前回実行時間</summary>
		protected DateTime? LastExecuteDateTime { get; private set; }
		/// <summary>開始時間</summary>
		protected DateTime StartDateTime { get; private set; }
		/// <summary>終了時間</summary>
		protected DateTime EndDateTime { get; set; }
		/// <summary>処理件数</summary>
		protected int Count { get; set; }
		/// <summary>ファイルチェッククラス</summary>
		protected Checker FileChecker { get; private set; }
		/// <summary>出力先ディレクトリ</summary>
		protected string ExportDirectory { get { return Path.Combine(Constants.EXPORT_DIRECTORY, this.Target); } }
		/// <summary>ステートメントファイル</summary>
		protected string StatementFile { get; private set; }
		/// <summary>ファイル名</summary>
		protected string FileName { get; private set; }
		/// <summary>出力フィールド文字列</summary>
		public string ExportFieldString { get; set; }
		/// <summary>成功かどうか</summary>
		public bool IsSuccess { get; set; }
		/// <summary>メールメッセージ </summary>
		public StringBuilder MailMessage { get; set; }
		/// <summary>ファイル出力定義</summary>
		protected FileDefines.FileDefine Define { get; private set; }
		#endregion
	}
}
