/*
=========================================================================================================
  Module      : マスタ出力のヘルパクラス(MasterExportHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Global.Config;
using w2.App.Common.MasterExport;
using w2.App.Common.Util;
using w2.Cms.Manager.Codes.Common;
using w2.Common.Util;
using w2.Domain.Coordinate;
using w2.Domain.CoordinateCategory;
using w2.Domain.Helper;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.ShortUrl;
using w2.Domain.Staff;

namespace w2.Cms.Manager.Codes
{
	/// <summary>
	/// マスタ出力のヘルパクラス
	/// </summary>
	public class MasterExportHelper
	{
		#region 定数
		/// <summary>抽出条件文</summary>
		private const string SQLSTATEMENT_WHERE = "@@ where @@";
		#endregion

		#region 列挙体
		/// <summary>出力タイプ</summary>
		public enum ExportType
		{
			/// <summary>CSV</summary>
			Csv,
			/// <summary>エクセル</summary>
			Excel
		}
		#endregion

		#region +CreateExportFilesDdlItems ファイル出力ドロップダウン用のアイテム生成
		/// <summary>
		/// ファイル出力ドロップダウン用のアイテム生成
		/// </summary>
		/// <returns>ドロップダウン用リストアイテム</returns>
		public static SelectListItem[] CreateExportFilesDdlItems(string masterKbn)
		{
			var masterExportSetting = new MasterExportSettingService().GetAllByMaster(Constants.CONST_DEFAULT_SHOP_ID, masterKbn);
			var valueText = ValueText.GetValueText(Constants.TABLE_MASTEREXPORTSETTING,
				Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN,
				masterKbn);

			var result = masterExportSetting
				.Select(
					item => new SelectListItem
					{
						Text = (item.SettingId == Constants.MASTEREXPORTSETTING_MASTER_SETTING_ID)
							? string.Format("{0}){0}", valueText)
							: string.Format("{0}){1}", valueText, item.SettingName),
						Value = string.Format("{0}-{1}", StringUtility.ToEmpty(item.SettingId), masterKbn),
					})
				.ToArray();

			return result;
		}
		#endregion

		#region +CreateExportData エクスポートデータ生成
		/// <summary>
		/// エクスポートデータ生成
		/// </summary>
		/// <param name="shopid">店舗ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="settingId">設定ID</param>
		/// <param name="searchCondition">検索条件</param>
		/// <returns>エクスポートするファイルのデータ</returns>
		public static ExportFileData CreateExportData(string shopid, string masterKbn, int settingId, BaseDbMapModel searchCondition)
		{
			var rtn = new ExportFileData(shopid, masterKbn, settingId, searchCondition);
			return rtn.CreateData();
		}
		/// <summary>
		/// エクスポートデータ生成
		/// </summary>
		/// <param name="shopid">店舗ID</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="settingId">設定ID</param>
		/// <param name="searchHash">検索条件</param>
		/// <returns>エクスポートするファイルのデータ</returns>
		public static ExportFileData CreateExportData(string shopid, string masterKbn, int settingId, Hashtable searchHash)
		{
			var rtn = new ExportFileData(shopid, masterKbn, settingId, searchHash);
			return rtn.CreateData();
		}
		#endregion

		/// <summary>
		/// エクスポートファイルデータクラス
		/// </summary>
		public class ExportFileData : IDisposable
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			private ExportFileData()
			{
			}
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="shopid">店舗ID</param>
			/// <param name="masterKbn">マスタ区分</param>
			/// <param name="settingId">設定ID</param>
			/// <param name="searchHash">検索条件</param>
			public ExportFileData(string shopid, string masterKbn, int settingId, Hashtable searchHash)
			{
				this.ShopId = shopid;
				this.ParamData = searchHash;
				this.MasterKbn = masterKbn;
				this.SettingId = settingId;
				this.Error = "";
				this.FileName = "";
				this.ContentType = "";
				this.FIleType = ExportType.Csv;
				this.Stream = new MemoryStream();
			}
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="shopid">店舗ID</param>
			/// <param name="masterKbn">マスタ区分</param>
			/// <param name="settingId">設定ID</param>
			/// <param name="searchCondition">検索条件</param>
			public ExportFileData(string shopid, string masterKbn, int settingId, BaseDbMapModel searchCondition)
				: this(shopid, masterKbn, settingId, searchCondition.CreateHashtableParams())
			{
			}

			/// <summary>
			/// データ生成
			/// </summary>
			/// <returns>生成したデータ</returns>
			public ExportFileData CreateData()
			{
				//------------------------------------------------------
				// マスタ出力定義情報取得
				//------------------------------------------------------
				// マスタ情報チェック
				if (CheckMasterInfo(this.ParamData) == false)
				{
					this.Error = WebMessages.InconsistencyError;
					return this;
				}

				// マスタ出力定義情報取得
				var exportSettings = GetMasterExportSettingDataView(this.ShopId, this.MasterKbn);
				if (exportSettings.Length <= this.SettingId)
				{
					this.Error = WebMessages.MasterExportSettingNoData;
					return this;
				}

				// マスタデータ取得・出力
				var exportSetting = exportSettings[this.SettingId];

				var exTension = "";
				if (exportSetting.ExportFileType == Constants.FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_CSV)
				{
					this.FIleType = ExportType.Csv;
					this.ContentType = "text/csv";
					exTension = ".csv";
				}
				if (exportSetting.ExportFileType == Constants.FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_XLS)
				{
					this.FIleType = ExportType.Excel;
					this.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
					exTension = Constants.MASTEREXPORT_EXCELFORMAT;
				}

				this.FileName = this.MasterKbn + DateTime.Now.ToString("yyyyMMdd") + exTension;

				// マスタ情報出力
				OutPutMasterInner(this.ParamData, exportSetting);

				this.Stream.Flush();
				this.Stream.Position = 0;

				return this;
			}

			/// <summary>
			/// 各マスタ検索情報有無チェック
			/// </summary>
			/// <param name="parameters">各マスタ検索情報</param>
			/// <returns>各マスタ検索情報有無</returns>
			private bool CheckMasterInfo(Hashtable parameters)
			{
				if (parameters == null) return false;

				switch (StringUtility.ToEmpty(this.MasterKbn))
				{
					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL:
					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE:
					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_ITEM:
					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_CATEGORY:
					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STAFF:
						return true;

					default:
						return false;
				}
			}

			/// <summary>
			/// マスタ出力定義情報データビュー取得
			/// </summary>
			/// <param name="shopId">店舗ID</param>
			/// <param name="masterKbn">マスタ区分</param>
			/// <returns>マスタ出力定義情報データビュー</returns>
			private MasterExportSettingModel[] GetMasterExportSettingDataView(string shopId, string masterKbn)
			{
				var dvResult = new MasterExportSettingService().GetAllByMaster(shopId, masterKbn);
				return dvResult;
			}

			/// <summary>
			/// 各マスタ情報取得(CSV形式文字列)
			/// </summary>
			/// <param name="searchParam">各マスタ検索情報</param>
			/// <param name="setting">マスタ出力定義情報</param>
			/// <returns>各マスタ情報取得(CSV形式文字列)</returns>
			protected void OutPutMasterInner(Hashtable searchParam, MasterExportSettingModel setting)
			{
				var outputFieldInfo = MasterExportSettingUtility.GetMasterExportSettingFields(this.MasterKbn);
				var fieldsCsvSplited = StringUtility.SplitCsvLine(setting.Fields);

				// CSV出力フォーマットとCSVフォーマット設定の不整合発生時はエラーとする
				var missedField = fieldsCsvSplited.FirstOrDefault(field => (outputFieldInfo.ContainsKey(field) == false));
				if (missedField != null)
				{
					this.Error = WebMessages.CsvOutputFieldError;
					return;
				}

				// 取得するフィールド列作成
				var sqlFieldNames = string.Join(
					",",
					fieldsCsvSplited.Select(f => outputFieldInfo[f]));

				// 出力
				var fileType = (setting.ExportFileType == Constants.FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_CSV)
					? ExportType.Csv
					: ExportType.Excel;

				var response = System.Web.HttpContext.Current.Response;
				if (Export(setting, searchParam, sqlFieldNames, response.OutputStream, fileType) == false)
				{
					// 最大件数オーバーエラー
					if (string.IsNullOrEmpty(this.Error))
					{
						this.Error = WebMessages.MasterExportExcelOverCapacity;
					}
				}
			}

			/// <summary>
			/// 実行
			/// </summary>
			/// <param name="setting">出力設定</param>
			/// <param name="sqlParams">検索パラメタ</param>
			/// <param name="sqlFieldNames">フィールド名（カンマ区切り）</param>
			/// <param name="outputStream">出力ストリーム</param>
			/// <param name="type">出力ファイル種別</param>
			public bool Export(MasterExportSettingModel setting, Hashtable sqlParams, string sqlFieldNames, Stream outputStream, ExportType type)
			{
				var excelTemplateSetting = new ExcelTemplateSetting(
					Path.Combine(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER, @"Contents\MasterExportExcelTemplate\TemplateBase.xml"),
					Path.Combine(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER, @"Contents\MasterExportExcelTemplate\TemplateElements.xml")
				);

				// 日付フォーマット設定※グローバル対応なしの場合、デフォルトフォーマットで日付を抽出
				var formatDate = (Constants.GLOBAL_OPTION_ENABLE && (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false))
					? GlobalConfigUtil.GetDateTimeFormatText(
						Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE,
						DateTimeUtility.FormatType.ShortDateHourMinuteSecondNoneServerTime)
					: Constants.CONST_SHORTDATETIME_2LETTER_FORMAT;

				var replacePrice = CultureInfo.CreateSpecificCulture(Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId).NumberFormat.CurrencyDecimalSeparator;

				//基軸通貨 小数点以下の有効桁数
				var digitsByKeyCurrency = Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.CurrencyDecimalDigits
					?? CultureInfo.CreateSpecificCulture(Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId).NumberFormat.CurrencyDecimalDigits;

				switch (setting.MasterKbn)
				{
					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_SHORTURL: // ショートURL
						if (type == ExportType.Csv) return new ShortUrlService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
						if (type == ExportType.Excel) return new ShortUrlService().ExportToExcel(setting,
							sqlParams,
							sqlFieldNames,
							excelTemplateSetting,
							outputStream,
							formatDate,
							replacePrice,
							digitsByKeyCurrency);
						break;

					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE: // コーディネート
					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_ITEM: // コーディネートアイテム
						if (type == ExportType.Csv) return new CoordinateService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
						if (type == ExportType.Excel) return new CoordinateService().ExportToExcel(setting,
							sqlParams,
							sqlFieldNames,
							excelTemplateSetting,
							outputStream,
							formatDate,
							replacePrice,
							digitsByKeyCurrency);
						break;

					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_CATEGORY: // コーディネートカテゴリ
						if (type == ExportType.Csv) return new CoordinateCategoryService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
						if (type == ExportType.Excel) return new CoordinateCategoryService().ExportToExcel(setting,
							sqlParams,
							sqlFieldNames,
							excelTemplateSetting,
							outputStream,
							formatDate,
							replacePrice,
							digitsByKeyCurrency);
						break;

					case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_STAFF: // スタッフ
						if (type == ExportType.Csv) return new StaffService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
						if (type == ExportType.Excel) return new StaffService().ExportToExcel(setting,
							sqlParams,
							sqlFieldNames,
							excelTemplateSetting,
							outputStream,
							formatDate,
							replacePrice,
							digitsByKeyCurrency);
						break;
				}
				this.Error = WebMessages.CsvOutputKbnError + setting.MasterKbn;
				return false;
			}

			/// <summary>
			/// ActionResult生成
			/// </summary>
			/// <returns>ActionResult</returns>
			public ActionResult CreateActionResult()
			{
				// ストリームセットして返す
				return new FileStreamResult(this.Stream, this.ContentType) { FileDownloadName = this.FileName };
			}

			/// <summary>
			/// Dispose
			/// </summary>
			public void Dispose()
			{
				if (this.Stream != null) this.Stream.Dispose();
			}

			/// <summary>店舗ID</summary>
			public string ShopId { get; private set; }
			/// <summary>パラメタデータ</summary>
			public Hashtable ParamData { get; private set; }
			/// <summary>マスタ区分</summary>
			public string MasterKbn { get; private set; }
			/// <summary>設定ID</summary>
			public int SettingId { get; private set; }
			/// <summary>エラー。空の場合はエラーなし</summary>
			public string Error { get; private set; }
			/// <summary>パラメタデータ</summary>
			public string FileName { get; private set; }
			/// <summary>コンテンツタイプ</summary>
			public string ContentType { get; private set; }
			/// <summary>ファイルタイプ</summary>
			public ExportType FIleType { get; private set; }
			/// <summary>ストリーム</summary>
			public MemoryStream Stream { get; private set; }
		}
	}
}