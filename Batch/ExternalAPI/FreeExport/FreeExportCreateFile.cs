/*
=========================================================================================================
  Module      : FreeExport 連携ファイル作成クラス(FreeExportCreateFile.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using ExternalAPI.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.Common.Util;

namespace ExternalAPI.FreeExport
{
	/// <summary>
	/// FreeExport 連携ファイル作成クラス
	/// </summary>
	public class FreeExportCreateFile
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="exportSetting">FreeExport 実行設定内容</param>
		public FreeExportCreateFile(ExportSetting exportSetting)
		{
			this.ExportSetting = exportSetting;
		}

		/// <summary>
		/// 連携ファイル作成
		/// </summary>
		/// <param name="workExportFilePath">作業ディレクトリ 連携ファイルパス</param>
		/// <param name="workUpdateDateFilePath">作業ディレクトリ DB更新用ファイルパス</param>
		public void CreateFile(string workExportFilePath, string workUpdateDateFilePath)
		{
			using (var exportFileStreamWriter = new StreamWriter(
				workExportFilePath,
				false,
				EncodingHelper.GetEncoding(this.ExportSetting.FileSetting.EncodingType)))
			using (var updateSqlFileStreamWriter = new StreamWriter(workUpdateDateFilePath, false, Encoding.UTF8))
			{
				// 連携ファイル header設定
				if (this.ExportSetting.FileSetting.HeaderExport)
				{
					var exportHeader = string.Join(
						this.ExportSetting.FileSetting.Separator.Value,
						this.ExportSetting.SearchSetting.Fields.Where(f => f.ExportFlg).Select(
							f => AddQuotationMark(f.HeaderName)));
					exportFileStreamWriter.WriteLine(exportHeader);
				}

				// DB更新用ファイル header設定
				if (this.IsCreateUpdateDateFile)
				{
					var updateHeader = string.Join(
						",",
						this.ExportSetting.SearchSetting.Fields.Where(f => string.IsNullOrEmpty(f.UpdateSqlKey) == false)
							.Select(f => f.UpdateSqlKey));
					updateSqlFileStreamWriter.WriteLine(updateHeader);
				}

				using (var accessor = new SqlAccessor())
				using (var statement = new SqlStatement(this.ExportSetting.SearchSetting.ExportSql.Sql))
				{
					// 連携ファイル作成用クエリの作成
					statement.CommandTimeout = this.ExportSetting.SearchSetting.ExportSql.ExecTimeOut;
					statement.Statement = statement.Statement.Replace(
						"@@ fields @@",
						string.Join(
							",\r\n",
							this.ExportSetting.SearchSetting.Fields.Select(f => string.Format("({0})", f.Value))));

					accessor.OpenConnection();
					using (var reader = new SqlStatementDataReader(accessor, statement))
					{
						while (reader.Read())
						{
							// 連携ファイル書き込み
							var exportUnitLine = GetExportFileUnitData(reader);
							exportFileStreamWriter.WriteLine(exportUnitLine);

							if (this.IsCreateUpdateDateFile)
							{
								// DB更新用ファイル書き込み
								var updateUnitLine = GetUpdateCsv(reader);
								updateSqlFileStreamWriter.WriteLine(updateUnitLine);
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 連携ファイル 書き込みレコード作成
		/// </summary>
		/// <param name="ssdrDataReader">SQLステートメントデータリーダー 1レコード分のDBデータ内容</param>
		/// <returns>書き込みレコード</returns>
		private string GetExportFileUnitData(SqlStatementDataReader ssdrDataReader)
		{
			var result = new List<string>();
			for (var i = 0; i < ssdrDataReader.FieldCount; i++)
			{
				if (this.ExportSetting.SearchSetting.Fields[i].ExportFlg == false) continue;
				// 設定ファイル内ConvertStringを実行 半角・全角対応
				var convertTypeString = this.ExportSetting.SearchSetting.Fields[i].ConvertString;
				var value = ConvertHelper.ApplyConvertByConvertType(ssdrDataReader[i].ToString(), convertTypeString);

				// 設定ファイルStartBytePositionを実行 バイト長で切り出し
				if ((this.ExportSetting.SearchSetting.Fields[i].StartBytePosition > 0)
					&& (this.ExportSetting.SearchSetting.Fields[i].ByteLength > 0))
				{
					value = StringUtility.GetWithSpecifiedByteLength(
						value,
						this.ExportSetting.SearchSetting.Fields[i].StartBytePosition,
						this.ExportSetting.SearchSetting.Fields[i].ByteLength,
						EncodingHelper.GetEncoding(this.ExportSetting.FileSetting.EncodingType));
				}

				result.Add(AddQuotationMark(value));
			}

			return string.Join(this.ExportSetting.FileSetting.Separator.Value, result);
		}

		/// <summary>
		/// 引用符を追加
		/// </summary>
		/// <param name="value">引用符の追加対象</param>
		/// <returns>引用符を追加した内容</returns>
		private string AddQuotationMark(string value)
		{
			if (this.ExportSetting.FileSetting.EscapeTarget.Any(et => value.Contains(et)) == false)
			{
				return string.Format("{0}{1}{0}", this.ExportSetting.FileSetting.QuotationMark, value);
			}

			var escapedValue = this.ExportSetting.FileSetting.EscapeTarget.Any(et => et == "\"")
				? value.Replace("\"", "\"\"")
				: value;

			string result;
			if (this.ExportSetting.FileSetting.QuotationMark == "\"")
			{
				result = string.Format("{0}{1}{0}", this.ExportSetting.FileSetting.QuotationMark, escapedValue);
			}
			else
			{
				result = string.Format("\"{0}{1}{0}\"", this.ExportSetting.FileSetting.QuotationMark, escapedValue);
			}

			return result;
		}

		/// <summary>
		/// DB更新用ファイルの書き込みレコード作成
		/// </summary>
		/// <param name="ssdrDataReader">SQLステートメントデータリーダー 1レコード分のDBデータ内容</param>
		/// <returns>DB更新用ファイルの書き込みレコード</returns>
		private string GetUpdateCsv(SqlStatementDataReader ssdrDataReader)
		{
			var result = new List<string>();
			for (var i = 0; i < ssdrDataReader.FieldCount; i++)
			{
				var updateSqlKey = this.ExportSetting.SearchSetting.Fields[i].UpdateSqlKey;
				if (string.IsNullOrEmpty(updateSqlKey)) continue;

				result.Add(ssdrDataReader[i].ToString());
			}

			return string.Join(",", result);
		}

		/// <summary>FreeExport 実行設定内容</summary>
		private ExportSetting ExportSetting { get; set; }
		/// <summary>
		/// DB更新用ファイルの更新有無
		/// 設定ファイル内に更新内容が存在しない場合は更新しない
		/// </summary>
		private bool IsCreateUpdateDateFile
		{
			get { return (string.IsNullOrEmpty(this.ExportSetting.SearchSetting.UpdateSql.Sql) == false); }
		}
	}
}
