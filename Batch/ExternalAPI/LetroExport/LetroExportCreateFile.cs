/*
=========================================================================================================
  Module      : LetroExport 連携ファイル作成クラス(LetroExportCreateFile.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using ExternalAPI.FreeExport;
using ExternalAPI.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Web;

namespace ExternalAPI.LetroExport
{
	/// <summary>
	/// LetroExport 連携ファイル作成クラス
	/// </summary>
	public class LetroExportCreateFile
	{
		/// <summary>Param key: last execute datetime</summary>
		private const string CONST_PARAM_KEY_LAST_EXECUTE_DATETIME = "last_execute_datetime";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="exportSetting">LetroExport 実行設定内容</param>
		public LetroExportCreateFile(ExportSetting exportSetting)
		{
			this.ExportSetting = exportSetting;
		}

		/// <summary>
		/// 連携ファイル作成
		/// </summary>
		/// <param name="workExportFilePath">作業ディレクトリ 連携ファイルパス</param>
		/// <param name="lastExecuteDatetime">Last execute datetime</param>
		public void CreateFile(string workExportFilePath, DateTime? lastExecuteDatetime)
		{
			using (var exportFileStreamWriter = new StreamWriter(
				workExportFilePath,
				false,
				Encoding.GetEncoding(this.ExportSetting.FileSetting.EncodingType)))
			{
				// 連携ファイル header設定
				if (this.ExportSetting.FileSetting.HeaderExport)
				{
					var exportHeader = string.Join(
						this.ExportSetting.FileSetting.Separator.Value,
						this.ExportSetting.SearchSetting.Fields
							.Where(field => field.ExportFlg)
							.Select(field => AddQuotationMark(field.HeaderName)));
					exportFileStreamWriter.WriteLine(exportHeader);
				}

				var input = new Hashtable
				{
					{ CONST_PARAM_KEY_LAST_EXECUTE_DATETIME, lastExecuteDatetime },
				};

				using (var accessor = new SqlAccessor())
				using (var statement = new SqlStatement(this.ExportSetting.SearchSetting.ExportSql.Sql))
				{
					// 連携ファイル作成用クエリの作成
					statement.CommandTimeout = this.ExportSetting.SearchSetting.ExportSql.ExecTimeOut;
					statement.ReplaceStatement(
						"@@ fields @@",
						string.Join(
							",\r\n",
							this.ExportSetting.SearchSetting.Fields.Select(field => string.Format("({0})", field.Value))));
					statement.AddInputParameters(CONST_PARAM_KEY_LAST_EXECUTE_DATETIME, SqlDbType.DateTime);

					accessor.OpenConnection();
					using (var reader = new SqlStatementDataReader(accessor, statement, input))
					{
						while (reader.Read())
						{
							// 連携ファイル書き込み
							var exportUnitLine = GetExportFileUnitData(reader);
							exportFileStreamWriter.WriteLine(exportUnitLine);
						}
					}
				}
			}
		}

		/// <summary>
		/// 連携ファイル 書き込みレコード作成
		/// </summary>
		/// <param name="dataReader">SQLステートメントデータリーダー 1レコード分のDBデータ内容</param>
		/// <returns>書き込みレコード</returns>
		private string GetExportFileUnitData(SqlStatementDataReader dataReader)
		{
			var result = new List<string>();
			for (var i = 0; i < dataReader.FieldCount; i++)
			{
				if (this.ExportSetting.SearchSetting.Fields[i].ExportFlg == false) continue;
				// 設定ファイル内ConvertStringを実行 半角・全角対応
				var convertTypeString = this.ExportSetting.SearchSetting.Fields[i].ConvertString;
				var value = ConvertHelper.ApplyConvertByConvertType(dataReader[i].ToString(), convertTypeString);

				// 設定ファイルStartBytePositionを実行 バイト長で切り出し
				if ((this.ExportSetting.SearchSetting.Fields[i].StartBytePosition > 0)
					&& (this.ExportSetting.SearchSetting.Fields[i].ByteLength > 0))
				{
					value = StringUtility.GetWithSpecifiedByteLength(
						value,
						this.ExportSetting.SearchSetting.Fields[i].StartBytePosition,
						this.ExportSetting.SearchSetting.Fields[i].ByteLength,
						Encoding.GetEncoding(this.ExportSetting.FileSetting.EncodingType));
				}

				var convertTypeUrl = this.ExportSetting.SearchSetting.Fields[i].ConvertUrl;
				switch (convertTypeUrl)
				{
					case ConvertUrl.None:
						break;

					case ConvertUrl.ProductImageHead:
						if (string.IsNullOrEmpty(value) == false)
						{
							var imageName = string.Format("{0}{1}", value, Constants.PRODUCTIMAGE_FOOTER_L);
							value = string.Format(
								@"{0}{1}{2}/{3}",
								Constants.URL_FRONT_PC_SECURE,
								Constants.PATH_PRODUCTIMAGES,
								Constants.CONST_DEFAULT_SHOP_ID,
								HttpUtility.UrlEncode(imageName));
						}
						break;

					case ConvertUrl.ProductDetail:
						if (string.IsNullOrEmpty(value) == false)
						{
							value = new UrlCreator(Constants.URL_FRONT_PC_SECURE + Constants.PAGE_FRONT_PRODUCT_DETAIL)
								.AddParam(Constants.REQUEST_KEY_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID)
								.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, value)
								.CreateUrl();
						}
						break;

					default:
						throw new ArgumentOutOfRangeException(
							nameof(convertTypeUrl),
							convertTypeUrl,
							null);
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
			var result = string.Format("{0}{1}{0}", this.ExportSetting.FileSetting.QuotationMark, value);
			return result;
		}

		/// <summary>LetroExport 実行設定内容</summary>
		private ExportSetting ExportSetting { get; set; }
	}
}
