/*
=========================================================================================================
  Module      : マスタ情報出力ページ処理(MasterExport.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using w2.App.Common.Global.Config;
using w2.App.Common.MasterExport;
using w2.Domain.AdvCode;
using w2.Domain.Affiliate;
using w2.Domain.Coupon;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.Point;
using w2.Domain.TargetList;
using w2.Domain.User.Helper;

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
		Hashtable parameters = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		if (CheckMasterInfo(parameters) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		// マスタ区分取得
		string masterKbn = (string)parameters[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN];
		var settingIndex = (int)parameters[Constants.MASTEREXPORTSETTING_SELECTED_SETTING_ID];

		// マスタ出力定義情報取得
		var exportSettings = new MasterExportSettingService().GetAllByMaster(strShopId, masterKbn);
		if (exportSettings.Length <= settingIndex)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_MASTEREXPORTSETTING_NO_DATA);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		//------------------------------------------------------
		// マスタデータ取得・出力
		//------------------------------------------------------
		var exportSetting = exportSettings[settingIndex];
		exportSetting.MasterKbn = (string)parameters[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN];

		// HTTPヘッダセット(ファイル名など）
		var fileExtension = (exportSetting.ExportFileType == Constants.FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_CSV)
			? ".csv"
			: Constants.MASTEREXPORT_EXCELFORMAT;
		string fileSuffFix = (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA) ? DateTime.Now.ToString("yyyyMMddHHmmss") : DateTime.Now.ToString("yyyyMMdd");
		Response.AppendHeader("Content-Disposition", "attachment; filename=" + masterKbn + fileSuffFix + fileExtension);

		if (exportSetting.ExportFileType == Constants.FLG_MASTEREXPORTSETTING_EXPORT_FILE_TYPE_CSV)
		{
			// CSV出力時に文字コードを設定
			Response.ContentEncoding = Encoding.GetEncoding(Constants.MASTEREXPORT_CSV_ENCODE);
		}

		// マスタ情報出力
		OutPutMasterInner(parameters, exportSetting);

		// 出力ストップ
		Response.End();
	}

	/// <summary>
	/// 各マスタ検索情報有無チェック
	/// </summary>
	/// <param name="parameters">各マスタ検索情報</param>
	/// <returns>各マスタ検索情報有無</returns>
	private bool CheckMasterInfo(Hashtable parameters)
	{
		if (parameters == null) return false;

		switch (StringUtility.ToEmpty(parameters[Constants.FIELD_MASTEREXPORTSETTING_MASTER_KBN]))
		{
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA:			// ターゲットリストデータ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_AFFILIATECOOPLOG:			// アフィリエイト連携ログ表示
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE:					// 広告コード
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT:				// ユーザーポイント情報
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT_DETAIL:			// ユーザーポイント情報(詳細)
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE_MEDIA_TYPE: 		// 広告媒体区分マスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON: 					// クーポンマスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERCOUPON: 				// ユーザクーポンマスタ
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON_USE_USER:			// クーポン利用ユーザー情報
				return true;

			default:
				return false;
		}
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
		var htOutputFieldInfo = GetMasterExportSettingFields(setting.MasterKbn);
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
			fieldNamesCsvSplit.Select(f => htOutputFieldInfo[f]));

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
	/// マスタ出力設定のフィールド群取得
	/// </summary>
	/// <param name="masterKbn">マスタ区分</param>
	/// <returns>ハッシュテーブル</returns>
	private Hashtable GetMasterExportSettingFields(string masterKbn)
	{
		//------------------------------------------------------
		// フィールド名変換（共通）
		//------------------------------------------------------
		Hashtable result = MasterExportSettingUtility.GetMasterExportSettingFields(masterKbn);

		//------------------------------------------------------
		// フィールド名変換（ユーザーマスタの場合)
		//------------------------------------------------------
		if (masterKbn == Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA)
		{
			// ユーザー拡張項目のフィールド名を取得できるよう、動的部分のHashtable作成
			UserExtendSettingList userExtendSettingList = new UserExtendSettingList(this.LoginOperatorId);
			userExtendSettingList.Items.ForEach(userExtendSetting =>
				result.Add(userExtendSetting.SettingId, Constants.TABLE_USEREXTEND + "." + userExtendSetting.SettingId));

			// ユーザー属性
			var userAttributeFields = MasterExportSettingUtility.GetMasterExportSettingFields(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA_USERATTRIBUTE);
			foreach (var key in userAttributeFields.Keys)
			{
				result[key] = userAttributeFields[key];
			}
		}

		return result;
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
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_AFFILIATECOOPLOG: // アフィリエイト連携ログマスタ表示
				if (type == FileType.Csv) return new AffiliateService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new AffiliateService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_TARGETLISTDATA: // ターゲットリストデータ
				if (type == FileType.Csv) return new TargetListService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new TargetListService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT: // ユーザーポイント
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPOINT_DETAIL: // ユーザーポイント詳細
				if (type == FileType.Csv) return new PointService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new PointService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;

			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE: // 広告コード
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE_MEDIA_TYPE: // 広告媒体区分情報
				if (type == FileType.Csv) return new AdvCodeService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, this.LoginOperatorShopId, this.LoginOperatorId, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new AdvCodeService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, this.LoginOperatorShopId, this.LoginOperatorId, digitsByKeyCurrency, replacePrice);
				break;

			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON: // クーポン
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERCOUPON: // ユーザクーポン
			case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COUPON_USE_USER: // クーポン利用ユーザー情報
				if (type == FileType.Csv) return new CouponService().ExportToCsv(setting, sqlParams, sqlFieldNames, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				if (type == FileType.Excel) return new CouponService().ExportToExcel(setting, sqlParams, sqlFieldNames, excelTemplateSetting, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				break;
		}
		throw new Exception("未対応のマスタ区分：" + setting.MasterKbn);
	}
}
