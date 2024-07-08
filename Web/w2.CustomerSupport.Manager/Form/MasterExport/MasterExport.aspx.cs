/*
=========================================================================================================
  Module      : マスタ情報出力ページ処理(MasterExport.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using w2.App.Common.Global.Config;
using w2.Domain.CsMessage;
using w2.App.Common.MasterExport;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;

/// <summary>
/// MasterExport の概要の説明です。
/// </summary>
public partial class Form_MasterExport_MasterExport : BasePage
{
	/// <summary>出力ファイル種別</summary>
	public enum FileType
	{
		Csv,
		Excel,
	}

	protected const string XML_MASTEREXPORTSETTING_ROOT = "MasterExportSetting";	// マスタ出力定義ルート名
	protected const string XML_MASTEREXPORTSETTING_NAME = "name";						// 英語名

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		// マスタ情報出力
		OutPutMaster(this.LoginOperatorShopId);
	}

	/// <summary>
	/// マスタ情報出力
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	private void OutPutMaster(string strShopId)
	{
		//------------------------------------------------------
		// マスタ出力定義情報取得
		//------------------------------------------------------
		// マスタ情報チェック
		Hashtable htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		if (CheckMasterInfo(htParam) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// マスタ区分取得
		string strMasterKbn = (string)htParam[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN];

		// マスタ出力定義情報取得
		var exportSetting = GetMasterExportSettingForCsMessage(strShopId, strMasterKbn);
		if (exportSetting == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTEREXPORTSETTING_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		else
		{
			//------------------------------------------------------
			// マスタデータ取得・出力
			//------------------------------------------------------
			exportSetting.MasterKbn = (string)htParam[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN];

			// HTTPヘッダセット(ファイル名など）
			var fileExtension = (exportSetting.ExportFileType == Constants.FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_CSV)
				? ".csv"
				: Constants.MASTEREXPORT_EXCELFORMAT;
			Response.AppendHeader("Content-Disposition", "attachment; filename=" + strMasterKbn + DateTime.Now.ToString("yyyyMMdd") + fileExtension);

			if (exportSetting.ExportFileType == Constants.FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_CSV)
			{
				// CSV出力時に文字コードを設定
				Response.ContentEncoding = Encoding.GetEncoding(Constants.MASTEREXPORT_CSV_ENCODE);
			}

			// マスタ情報出力
			OutPutMasterInner(htParam, exportSetting);

			// 出力ストップ
			Response.End();
		}
	}

	/// <summary>
	/// 各マスタ検索情報有無チェック
	/// </summary>
	/// <param name="htParam">各マスタ検索情報</param>
	/// <returns>各マスタ検索情報有無</returns>
	private bool CheckMasterInfo(Hashtable htParam)
	{
		// 変数宣言
		bool blResult = false;

		if (htParam != null)
		{
			switch (StringUtility.ToEmpty(htParam[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN]))
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_CSMESSAGE:			// メッセージマスタ
					blResult = true;
					break;
			}
		}

		return blResult;
	}

	/// <summary>
	/// 各マスタ情報取得(CSV形式文字列)
	/// </summary>
	/// <param name="htParam">各マスタ検索情報</param>
	/// <param name="setting">マスタ出力定義情報</param>
	/// <returns>各マスタ情報取得(CSV形式文字列)</returns>
	private void OutPutMasterInner(Hashtable htParam, MasterExportSettingModel setting)
	{
		//------------------------------------------------------
		// SQLステートメント情報取得
		//------------------------------------------------------
		Hashtable htOutputFieldInfo = MasterExportSettingUtility.GetMasterExportSettingFields(setting.MasterKbn);
		var fieldNamesCsvSplit = StringUtility.SplitCsvLine(setting.Fields);

		// CSV出力フォーマットとCSVフォーマット設定の不整合発生時はエラーとする
		var missedField = fieldNamesCsvSplit.FirstOrDefault(field => (htOutputFieldInfo.ContainsKey(field) == false));
		if (missedField != null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_CSV_OUTPUT_FIELD_ERROR)
				.Replace("@@ 1 @@", missedField)
				.Replace("@@ 2 @@", ValueText.GetValueText(Constants.TABLE_MASTEREXPORTSETTING, Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN, setting.MasterKbn));
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 取得するフィールド列作成
		var sqlFieldNames = string.Join(
			",",
			fieldNamesCsvSplit.Select(
				f => htOutputFieldInfo[f].ToString().Contains("AS")
					? htOutputFieldInfo[f]
					: htOutputFieldInfo[f] + " AS " + f));

		// 出力
		var fileType = (setting.ExportFileType == Constants.FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_CSV)
			? FileType.Csv
			: FileType.Excel;

		if (Export(setting, htParam, sqlFieldNames, Response.OutputStream, fileType) == false)
		{
			// 最大件数オーバーエラー
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTEREXPORT_EXCEL_OVER_CAPACITY);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
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
	public bool Export(MasterExportSettingModel setting, Hashtable sqlParams, string sqlFieldNames, Stream outputStream, FileType type)
	{
		var excelTemplateSetting = new ExcelTemplateSetting(
			Path.Combine(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER, @"Contents\MasterExportExcelTemplate\TemplateBase.xml"),
			Path.Combine(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER, @"Contents\MasterExportExcelTemplate\TemplateElements.xml")
		);

		var replacePrice = CultureInfo.CreateSpecificCulture(Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId).NumberFormat.CurrencyDecimalSeparator;

		//基軸通貨 小数点以下の有効桁数
		var digitsByKeyCurrency = Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.CurrencyDecimalDigits
			?? CultureInfo.CreateSpecificCulture(Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId).NumberFormat.CurrencyDecimalDigits;

		// 日付フォーマット設定※グローバル対応なしの場合、デフォルトフォーマットで日付を抽出
		var formatDate = (Constants.GLOBAL_OPTION_ENABLE && (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false))
			? GlobalConfigUtil.GetDateTimeFormatText(
				Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE,
				DateTimeUtility.FormatType.ShortDateHourMinuteSecondNoneServerTime)
			: string.Empty;

		switch (setting.MasterKbn)
		{
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_CSMESSAGE: // アフィリエイト連携ログマスタ表示
				if (type == FileType.Csv) return new CsMessageService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new CsMessageService().ExportToExcel(setting,
					sqlParams,
					sqlFieldNames,
					excelTemplateSetting,
					outputStream,
					formatDate,
					digitsByKeyCurrency,
					replacePrice);
				break;
		}
		throw new Exception("未対応のマスタ区分：" + setting.MasterKbn);
	}

	/// <summary>
	/// マスタ出力定義情報取得
	/// </summary>
	/// <param name="strShopId">店舗ID</param>
	/// <param name="strMasterKbn">マスタ区分</param>
	/// <returns>マスタ出力定義情報</returns>
	private MasterExportSettingModel GetMasterExportSettingForCsMessage(string strShopId, string strMasterKbn)
	{
		// 変数宣言
		var dvResult = new MasterExportSettingModel();

		// メッセージ一覧？
		if (strMasterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_CSMESSAGE)
		{
			// マスタ出力定義ファイルを元に、列名を抽出しておく
			var columnList = new List<string>();
			var xdMasterExportSetting = new XmlDocument();
			xdMasterExportSetting.Load(AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_MASTEREXPORTSETTING_SETTING);
			foreach (XmlNode xnField in xdMasterExportSetting.SelectSingleNode(XML_MASTEREXPORTSETTING_ROOT + "/" + strMasterKbn).ChildNodes)
			{
				if (xnField.NodeType == XmlNodeType.Comment)
				{
					continue;
				}
				columnList.Add(xnField.Attributes[XML_MASTEREXPORTSETTING_NAME].Value);
			}

			if (columnList.Count == 0)
			{
				return null;
			}

			dvResult.MasterKbn = "CsMessage";
			dvResult.ExportFileType = "CSV";
			dvResult.Fields = string.Join(",", columnList);
		}

		return dvResult;
	}
}