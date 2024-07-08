/*
=========================================================================================================
  Module      : 納品書出力クラス(OrderInvoiceCreater.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using w2.App.Common.Global.Config;
using w2.App.Common.Global.Translation;
using w2.App.Common.Pdf.Settings.Invoice;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.SubscriptionBox;

namespace w2.App.Common.Pdf.PdfCreater
{
	/// <summary>
	/// 納品書出力クラス
	/// </summary>
	public class OrderInvoiceCreater : BasePdfCreater
	{
		// １ファイルに出力する注文情報件数の上限値
		public const int CONST_OUTPUT_ORDER_MAX_COUNT = 500;

		// データソース名
		public const string CONST_DATASOURCE_NAME_ORDER_INVOICE = "OrderInvoice";

		// 構成要素ファイル名
		private const string CONST_COMPOSITION_FILENAME_BASE = @"Base.xml";
		private const string CONST_COMPOSITION_FILENAME_CODE = @"Code.xml";
		private const string CONST_COMPOSITION_FILENAME_HEADERFOOTER = @"HeaderFooter.xml";
		private const string CONST_COMPOSITION_FILENAME_IMAGES = @"Images.xml";
		private const string CONST_COMPOSITION_FILENAME_BODY = @"Body.xml";
		private const string CONST_COMPOSITION_FILENAME_VARIABLES = @"Variables.xml";
		private const string CONST_COMPOSITION_FILENAME_BODY_HEADER = @"Body\Header.xml";

		// 納品書構成ファイル格納ディレクトリ名
		private const string CONST_INVOICE = "Invoice";

		/// <summary>
		/// Constructor
		/// </summary>
		public OrderInvoiceCreater()
		{
			// Get setting from XML invoice setting file
			this.Setting = new InvoiceSetting(GetPhysicalInvoiceSettingPath());
		}

		/// <summary>
		/// PDF出力（管理側で利用）
		/// </summary>
		/// <param name="outPutStream">出力用ストリーム</param>
		/// <param name="searchKbn">検索区分</param>
		/// <param name="param">検索パラメタ</param>
		public void Create(Stream outPutStream, string searchKbn, Hashtable param)
		{
			// ステートメント名取得
			var statementName = GetStatementName(searchKbn);

			// レポート用データソース作成
			var orders = ConvertProductOptionTexts(GetOrderData(statementName, param));
			orders.Table.Columns.Add(InvoiceConstants.CONST_FIELD_ORDER_SUBSCRIPTION_BOX_MAX_COUNT, typeof(int));
			foreach (DataRowView order in orders)
			{
				// Convert some fields for order
				ConvertFieldsOrderData(order);
				AddSubscriptionBoxCount(order);
			}

			var dataSource = new ReportDataSource(CONST_DATASOURCE_NAME_ORDER_INVOICE, orders.Table);

			// 実行
			RenderReport(dataSource, outPutStream);
		}

		/// <summary>
		/// PDF作成（OrderPdfCreater.exeで利用）
		/// </summary>
		/// <param name="searchKbn">検索区分</param>
		/// <param name="param">検索パラメタ</param>
		/// <param name="sessionId">セッションID</param>
		public void Create(string searchKbn, Hashtable param, string sessionId)
		{
			// 対応言語のリスト
			var availableLanguages = GetAvailableLanguages();
			// ステートメント名取得
			var statement = GetStatementName(searchKbn);
			// レポート用データソース取得
			var devidedOrders = GetDevidedOrderDatas(statement, param, availableLanguages);

			// PDFファイル出力
			foreach (var item in devidedOrders)
			{
				// データがない場合、Skip
				if (item.Value.Count == 0) continue;

				// 最大行数毎にPDFファイルを出力する
				var fileNums = 0;
				foreach (var orders in item.Value)
				{
					fileNums++;
					// データソース作成
					var dataSource = new ReportDataSource(CONST_DATASOURCE_NAME_ORDER_INVOICE, orders);
					// 出力ファイル設定
					var tempFilePath = TempDirPath + this.FileBaseName + "_" + fileNums.ToString("00000")
						+ (Constants.GLOBAL_OPTION_ENABLE ? item.Key : "") + ".pdf." + sessionId;
					// 出力処理
					using (var fs = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.Write))
					{
						// 実行
						RenderReport(dataSource, fs, item.Key);
						// ストリームを閉じてSessionIdを付与してある拡張子を消してファイルの移動をする
						fs.Close();
					}

					// 一時ファイルを正式な場所へ移動する
					File.Move(tempFilePath, ExportDirPath + Path.GetFileNameWithoutExtension(tempFilePath));
				}
			}
		}

		/// <summary>
		/// 言語別かつ最大行数毎に分割された注文情報リスト取得
		/// </summary>
		/// <param name="statementName">ステートメント名</param>
		/// <param name="param">検索パラメタ</param>
		/// <param name="languages">言語の配列</param>
		/// <returns>言語ごとの分割済み注文リスト</returns>
		protected Dictionary<string, List<DataTable>> GetDevidedOrderDatas(
			string statementName,
			Hashtable param,
			string[] languages)
		{
			// 初期化
			var devidedOrders = new Dictionary<string, List<DataTable>>();
			var groupCounts = new Dictionary<string, int>();
			var index = new Dictionary<string, int>();
			foreach (var language in languages)
			{
				devidedOrders.Add(language,  new List<DataTable>());
				groupCounts.Add(language, 0);
				index.Add(language, 0);
			}

			// 注文情報取得
			var orderDatas = ConvertProductOptionTexts(GetOrderData(statementName, param));
			orderDatas.Table.Columns.Add(InvoiceConstants.CONST_FIELD_ORDER_SUBSCRIPTION_BOX_MAX_COUNT, typeof(int));

			string orderIdBefore = null;
			foreach (DataRowView order in orderDatas)
			{
				// 注文者の言語コードにより、納品書の言語を指定する
				var ownerLanguageCode = (string)order[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_CODE];
				var languageCode = (Constants.GLOBAL_OPTION_ENABLE == false)
					? languages[0]
					: (languages.Contains(ownerLanguageCode)
						? ownerLanguageCode
						: Constants.DEFAULT_INVOICE_LANGUAGE_CODE);
				var localeId = GlobalConfigUtil.GetLanguageLocaleId(languageCode);

				if (orderIdBefore != (string)order[Constants.FIELD_ORDER_ORDER_ID])
				{
					// 比較キー更新
					orderIdBefore = (string)order[Constants.FIELD_ORDER_ORDER_ID];

					// 最大行数毎にデータを分割
					if ((groupCounts[languageCode] % CONST_OUTPUT_ORDER_MAX_COUNT) == 0)
					{
						// 言語別の新しい入れ物追加
						devidedOrders[languageCode].Add(orderDatas.Table.Clone());
						// 言語別のリストインデックス計算
						index[languageCode] = groupCounts[languageCode] / CONST_OUTPUT_ORDER_MAX_COUNT;
					}

					// 言語別の注文IDの件数をカウント
					groupCounts[languageCode]++;
				}

				// グローバル対応の場合には、翻訳した名称で受注情報を変更する
				if(Constants.GLOBAL_OPTION_ENABLE) ChangeOrderDataWithTranslationNames(order, languageCode, localeId);

				// Convert some fields for order
				ConvertFieldsOrderData(order);
				// 頒布会回数を追加する。
				AddSubscriptionBoxCount(order);

				// 言語別のデータリストにデータ追加
				devidedOrders[languageCode][index[languageCode]].Rows.Add(order.Row.ItemArray);
			}
			return devidedOrders;
		}

		/// <summary>
		/// 納品書出力処理
		/// </summary>
		/// <param name="dataSource">レポートデータソース</param>
		/// <param name="outPutStream">出力ストリーム</param>
		/// <param name="languageCode">言語コード</param>
		protected void RenderReport(ReportDataSource dataSource, Stream outPutStream, string languageCode = null)
		{
			try
			{
				// Set language code for invoice setting
				if (string.IsNullOrEmpty(languageCode) == false) this.Setting.LanguageCode = languageCode;

				// 日本語の場合には、空に変換する
				if (languageCode == Constants.LANGUAGE_CODE_JAPANESE) languageCode = "";
				// レポート設定
				var report = CreateReport(languageCode);
				report.DataSources.Add(dataSource);

				Warning[] warning;
				string mimeType;
				string fileNameExtension;
				string encoding;
				string[] stream;
				// PDFとして出力
				var bytes = report.Render(
					"PDF",
					null,
					PageCountMode.Actual,
					out mimeType,
					out encoding,
					out fileNameExtension,
					out stream,
					out warning);
				outPutStream.Write(bytes, 0, bytes.Length);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("納品書の出力中にエラーが発生しました。", ex);
			}
		}

		/// <summary>
		/// LocalReportのインスタンスを取得
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <returns>初期設定済みReport</returns>
		protected LocalReport CreateReport(string languageCode = null)
		{
			var report = new LocalReport();

			report.LoadReportDefinition(new StringReader(CreateReportComposition(languageCode)));
			report.SetParameters(GetReportParameters());

			return report;
		}

		/// <summary>
		/// レポート構成データを生成
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <returns>構成データ</returns>
		protected string CreateReportComposition(string languageCode = null)
		{
			// レポート構成データ生成処理
			// ベース読み込み
			var reportComposition = CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_BASE);

			// 各要素を取得し、ベースに追加
			// Replace max display items in code element by new setting
			var compositionCodeReplaced = this.Setting.ReplaceMaxDisplayItemsInCodeElementBySetting(
				CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_CODE));
			reportComposition.Root.Add(compositionCodeReplaced.Elements());

			// Replace old header and footer by new setting
			var compositionHeaderFooterReplaced = this.Setting.ReplaceHeaderAndFooterElementBySetting(
				CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_HEADERFOOTER, languageCode));
			reportComposition.Root.Add(compositionHeaderFooterReplaced.Elements());

			// Replace old image data by new image setting
			var compositionImagesReplaced = this.Setting.ReplaceImageDataElementBySetting(
				CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_IMAGES));
			reportComposition.Root.Add(compositionImagesReplaced.Elements());
			reportComposition.Root.Add(
				CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_VARIABLES).Root.Elements());

			// レイアウトのみ更に分割されている為別処理
			reportComposition.Root.Add(CreateReportBodyParts(languageCode).Elements());

			return reportComposition.ToString();
		}

		/// <summary>
		/// レポート構成用パーツを生成する
		/// </summary>
		/// <param name="compositionFileName">構成用ファイル名</param>
		/// <param name="languageCode">言語コード</param>
		/// <returns>構成用パーツ</returns>
		protected XDocument CreateReportCompositionParts(string compositionFileName, string languageCode = null)
		{
			// ファイル読み込み
			var compositionFilePath = CreateTemplateFilePath(compositionFileName, languageCode);
			try
			{
				return XDocument.Load(compositionFilePath);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("ファイル「" + compositionFilePath + "」の読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// ボディ部パーツ生成
		/// </summary>
		/// <param name="languageCode">言語コード</param>
		/// <returns>BODY部分パーツ</returns>
		protected XElement CreateReportBodyParts(string languageCode = null)
		{
			// ファイル読み込み
			var body = this.Setting.ReplaceDetailAndPriceInBodyElementBySetting(
				CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_BODY, languageCode));

			// 名称が"BodyHeader"の要素を取得（Body部分のヘッダ挿入用）
			var bodyHeader = (from element in body.Descendants(body.GetDefaultNamespace() + "Rectangle")
					where element.Attribute("Name").Value == "BodyHeader"
					select element)
				.ElementAt(0);

			// Replace body header element by new setting
			var compositionBodyHeaderReplaced = this.Setting.ReplaceBodyHeaderElementBySetting(
				CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_BODY_HEADER, languageCode));

			// Body部分のヘッダ要素に分割された要素を挿入
			bodyHeader.Add(compositionBodyHeaderReplaced.Elements());

			return body;
		}

		/// <summary>
		/// テンプレートファイルパスを取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <param name="languageCode">言語コード</param>
		/// <returns>ファイルパス</returns>
		protected string CreateTemplateFilePath(string fileName, string languageCode = null)
		{
			// ファイル存在チェック
			if (File.Exists(CreateCustomerResourceFilePath(Constants.PROJECT_NO, fileName, languageCode)))
			{
				// 案件別かつ言語別構成ファイルパス設定
				return CreateCustomerResourceFilePath(Constants.PROJECT_NO, fileName, languageCode);
			}

			if (File.Exists(CreateCustomerResourceFilePath(Constants.PROJECT_NO, fileName)))
			{
				// 案件別構成ファイルパス設定
				return CreateCustomerResourceFilePath(Constants.PROJECT_NO, fileName);
			}

			// 案件別に用意されていない場合はデフォルト構成ファイルパス設定
			return File.Exists(
				CreateCustomerResourceFilePath(CONST_COMPOSITION_DEFAULT_DIRECTORY, fileName, languageCode))
				? CreateCustomerResourceFilePath(CONST_COMPOSITION_DEFAULT_DIRECTORY, fileName, languageCode)
				: CreateCustomerResourceFilePath(CONST_COMPOSITION_DEFAULT_DIRECTORY, fileName);
		}

		/// <summary>
		/// カスタマーリソースファイルパス作成
		/// </summary>
		/// <param name="storeDir">保存先ディレクトリ</param>
		/// <param name="fileName">ファイル名</param>
		/// <param name="languageCode">言語コード</param>
		/// <returns>カスタマーリソースファイルパス</returns>
		protected string CreateCustomerResourceFilePath(string storeDir, string fileName, string languageCode = null)
		{
			var filePath = Path.Combine(
				Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE,
				CONST_INVOICE,
				storeDir,
				StringUtility.ToEmpty(languageCode),
				fileName);
			return filePath;
		}

		/// <summary>
		/// レポートパラメータ取得
		/// </summary>
		/// <returns></returns>
		protected List<ReportParameter> GetReportParameters()
		{
			var reportParametors = new List<ReportParameter>
			{
				new ReportParameter("point_option_enabled", Constants.W2MP_POINT_OPTION_ENABLED.ToString()),
				new ReportParameter("coupon_option_enabled", Constants.W2MP_COUPON_OPTION_ENABLED.ToString()),
				new ReportParameter("member_rank_option_enabled", Constants.MEMBER_RANK_OPTION_ENABLED.ToString()),
				new ReportParameter("setpromotion_option_enabled", Constants.SETPROMOTION_OPTION_ENABLED.ToString()),
				new ReportParameter("gift_order_option_enabled", Constants.GIFTORDER_OPTION_ENABLED.ToString()),
				new ReportParameter("corporation_option_enabled", Constants.DISPLAY_CORPORATION_ENABLED.ToString()),
				new ReportParameter("fixed_purchase_option_enabled", Constants.FIXEDPURCHASE_OPTION_ENABLED.ToString()),
				new ReportParameter("management_included_tax_flag", Constants.MANAGEMENT_INCLUDED_TAX_FLAG.ToString()),
				new ReportParameter("currency_format", Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.PdfFormat),
				new ReportParameter("currency_localeId", Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId),
				new ReportParameter(
					"scheduled_shipping_date_visible",
					(GlobalConfigUtil.UseLeadTime() && Constants.SCHEDULED_SHIPPING_DATE_VISIBLE).ToString()),
				new ReportParameter(
					"product_option_settings_price_grant_enabled",
					Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED.ToString()),
			};

			return reportParametors;
		}

		/// <summary>
		/// 扱っている納品書の言語取得
		/// </summary>
		/// <returns>言語の配列</returns>
		private string[] GetAvailableLanguages()
		{
			// 日本語で初期化する
			var result = new List<string> { Constants.LANGUAGE_CODE_JAPANESE };
			// グローバル対応なしの場合、日本語の要素のみで返す
			if (Constants.GLOBAL_OPTION_ENABLE == false) return result.ToArray();

			// 言語フォルダが配置されているフォルダーを指定※案件別のフォルダーを優先する
			var invoiceProjectDir = Path.Combine(
				Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE,
				CONST_INVOICE,
				Constants.PROJECT_NO);
			var subDirectories = Directory.GetDirectories(
				Directory.Exists(invoiceProjectDir)
					? invoiceProjectDir
					: Path.Combine(
						Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE,
						CONST_INVOICE,
						CONST_COMPOSITION_DEFAULT_DIRECTORY));
			result.AddRange(subDirectories.Select(subDir => new DirectoryInfo(subDir).Name).Where(dirName => dirName != "Body"));
			return result.ToArray();
		}

		/// <summary>
		/// 翻訳した名称で注文情報変更
		/// </summary>
		/// <param name="data">注文情報</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="localeId">ロケールID</param>
		private void ChangeOrderDataWithTranslationNames(DataRowView data, string languageCode, string localeId)
		{
			// 決済方法の名を取得
			data[Constants.FIELD_PAYMENT_PAYMENT_NAME] = NameTranslationCommon.GetTranslationName(
				(string)data[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN],
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
				Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PAYMENT_PAYMENT_NAME,
				(string)data[Constants.FIELD_PAYMENT_PAYMENT_NAME],
				languageCode,
				localeId);

			// 商品名を取得
			data[Constants.FIELD_ORDERITEM_PRODUCT_NAME] = NameTranslationCommon.GetOrderItemProductTranslationName(
				(string)data[Constants.FIELD_ORDERITEM_PRODUCT_NAME],
				(string)data[Constants.FIELD_ORDERITEM_PRODUCT_ID],
				(string)data[Constants.FIELD_ORDERITEM_VARIATION_ID],
				languageCode,
				localeId);

			// セットプロモーション名を取得
			data[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME] = NameTranslationCommon.GetTranslationName(
				StringUtility.ToEmpty(data[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID]),
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION,
				Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_SETPROMOTION_DISP_NAME,
				StringUtility.ToEmpty(data[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME]),
				languageCode,
				localeId);
		}

		/// <summary>
		/// Convert fields order data
		/// </summary>
		/// <param name="data">Data</param>
		private void ConvertFieldsOrderData(DataRowView data)
		{
			// Convert order payment kbn field
			var orderPaymentKbn = StringUtility.ToEmpty(data[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]);
			var orderPaymentKbnText = ValueText.GetValueText(
				Constants.TABLE_PAYMENT,
				Constants.VALUETEXT_PARAM_PAYMENT_TYPE,
				orderPaymentKbn);
			data[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] = orderPaymentKbnText;
		}

		/// <summary>
		/// 頒布会回数を追加する
		/// </summary>
		/// <param name="orderData">注文情報</param>
		private void AddSubscriptionBoxCount(DataRowView orderData)
		{
			// 頒布会情報取得
			var subscriptionBox = new SubscriptionBoxService().GetByCourseId((string)orderData[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID]);
			var maxCount = subscriptionBox != null
			? subscriptionBox.DefaultOrderProducts.Max(defaultItem => defaultItem.Count).Value
			: 0;
			orderData[InvoiceConstants.CONST_FIELD_ORDER_SUBSCRIPTION_BOX_MAX_COUNT] = maxCount;
		}

		/// <summary>
		/// Get physical invoice setting path
		/// </summary>
		/// <returns>Physical invoice setting path</returns>
		private string GetPhysicalInvoiceSettingPath()
		{
			var settingFilePath = Path.Combine(
				Constants.PHYSICALDIRPATH_FRONT_PC,
				Constants.FILE_XML_FRONT_INVOICE_SETTING);
			return settingFilePath;
		}

		/// <summary>出力ファイルベースファイル名</summary>
		protected string FileBaseName
		{
			get { return CONST_INVOICE + DateTime.Now.ToString("yyyyMMdd"); }
		}
		/// <summary>Invoice setting</summary>
		private InvoiceSetting Setting { get; set; }
	}
}
