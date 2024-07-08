/*
=========================================================================================================
  Module      : 領収書出力クラス(ReceiptCreater.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Reporting.WebForms;
using w2.App.Common.Global.Config;

namespace w2.App.Common.Pdf.PdfCreater
{
	/// <summary>
	/// 領収書出力クラス
	/// </summary>
	public class ReceiptCreater : BasePdfCreater
	{
		// １ファイルに出力する注文情報件数の上限値
		public const int CONST_OUTPUT_ORDER_MAX_COUNT = 500;
		// データソース名
		public const string CONST_DATASOURCE_NAME_RECEIPT = "Receipt";
		// 領収書構成ファイル格納ディレクトリ名
		private const string CONST_RECEIPT = "Receipt";
		// 構成要素ファイル名
		private const string CONST_COMPOSITION_FILENAME_BASE = @"Base.xml";
		private const string CONST_COMPOSITION_FILENAME_CODE = @"Code.xml";
		private const string CONST_COMPOSITION_FILENAME_HEADERFOOTER = @"HeaderFooter.xml";
		private const string CONST_COMPOSITION_FILENAME_IMAGES = @"Images.xml";
		private const string CONST_COMPOSITION_FILENAME_BODY = @"Body.xml";
		private const string CONST_COMPOSITION_FILENAME_VARIABLES = @"Variables.xml";
		private const string CONST_COMPOSITION_FILENAME_BODY_HEADER = @"Body\Header.xml";

		/// <summary>
		/// PDF出力（管理側で利用）
		/// </summary>
		/// <param name="outputStream">出力用ストリーム</param>
		/// <param name="searchKbn">検索区分</param>
		/// <param name="param">検索パラメタ</param>
		public void Create(Stream outputStream, string searchKbn, Hashtable param)
		{
			// ステートメント名取得
			var statementName = GetStatementName(searchKbn);
			// レポート用データソース作成
			var orders = ConvertProductOptionTexts(GetOrderData(statementName, param));
			var dataSource = new ReportDataSource(CONST_DATASOURCE_NAME_RECEIPT, orders.Table);
			// 実行
			RenderReport(dataSource, outputStream);
		}
		/// <summary>
		/// PDF作成（OrderPdfCreater.exeで利用）
		/// </summary>
		/// <param name="searchKbn">検索区分</param>
		/// <param name="param">検索パラメタ</param>
		/// <param name="sessionId">セッションID</param>
		public void Create(string searchKbn, Hashtable param, string sessionId)
		{
			// レポート用データソース取得
			var devidedOrders = GetDevidedOrderDatas(GetStatementName(searchKbn), param);
			// 最大行数毎にPDFファイルを出力する
			var fileBaseName = TempDirPath + CONST_RECEIPT + DateTime.Now.ToString("yyyyMMddHHmmss");
			var fileNums = 0;
			foreach (var orders in devidedOrders)
			{
				fileNums++;
				// 出力ファイル設定
				var tempFilePath = string.Format(
					"{0}_{1}.pdf.{2}",
					fileBaseName,
					fileNums.ToString("00000"),
					sessionId);

				// 出力処理
				using (var fs = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.Write))
				{
					// データソース作成
					var dataSource = new ReportDataSource(CONST_DATASOURCE_NAME_RECEIPT, orders);
					// 実行
					RenderReport(dataSource, fs);
					// ストリームを閉じてSessionIdを付与してある拡張子を消してファイルの移動をする
					fs.Close();
				}

				// 一時ファイルを正式な場所へ移動する
				File.Move(tempFilePath, ExportDirPath + Path.GetFileNameWithoutExtension(tempFilePath));
			}
		}

		/// <summary>
		/// 添付PDF作成（OrderPdfCreater.exeで利用）
		/// </summary>
		/// <param name="searchKbn">検索区分</param>
		/// <param name="param">検索パラメタ</param>
		/// <param name="fileName">PDFファイル名</param>
		/// <returns>出力ファイルパス</returns>
		public string CreateMailFile(string searchKbn, Hashtable param, string fileName)
		{
			// レポート用データソース取得
			var devidedOrders = GetDevidedOrderDatas(GetStatementName(searchKbn), param);

			if (Directory.Exists(TempDirPath) == false)
			{
				Directory.CreateDirectory(TempDirPath + @"\");
			}
			var fileBaseName = string.Format(
				"{0}{1}_{2}_{3}_{4}",
				TempDirPath,
				CONST_RECEIPT,
				((DateTime)param[Constants.FIELD_ORDERITEM_DATE_CREATED]).ToString("yyyyMMddHHmmss"),
				fileName,
				decimal.ToInt32((decimal)param[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL]));
			var filePath = "";
			// 最大行数毎にPDFファイルを出力する
			foreach (var orders in devidedOrders)
			{
				// 出力ファイル設定
				var tempFilePath = string.Format(
					"{0}.pdf.{1}",
					fileBaseName,
					DateTime.Now.ToString("yyyyMMddHHmmss"));

				// 出力処理
				using (var fs = new FileStream(tempFilePath, FileMode.OpenOrCreate, FileAccess.Write))
				{
					// データソース作成
					var dataSource = new ReportDataSource(CONST_DATASOURCE_NAME_RECEIPT, orders);
					// 実行
					RenderReport(dataSource, fs);
					// ストリームを閉じてSessionIdを付与してある拡張子を消してファイルの移動をする
					fs.Close();
				}

				// 一時ファイルを正式なファイル名変更
				var dirPath = TempDirPath + Path.GetFileNameWithoutExtension(tempFilePath);
				if (File.Exists(dirPath))
				{
					var files = Directory.GetFiles(TempDirPath, "*");
					dirPath = string.Format(
						"{0}_{1}.pdf",
						fileBaseName,
						files.Count(file => file.IndexOf(fileBaseName) >= 0));
				}	
				File.Move(tempFilePath, dirPath);
				filePath = dirPath;
			}
			return filePath;
		}

		/// <summary>
		/// 注文情報取得
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <param name="pdfKbn">PDF出力区分</param>
		/// <returns>注文情報/returns>
		public DataView GetOrders(Hashtable param, string pdfKbn)
		{
			var orders = ConvertProductOptionTexts(GetOrderData(GetStatementName(pdfKbn), param));
			return orders;
		}

		/// <summary>
		/// 最大行数毎に分割された注文情報リスト取得
		/// </summary>
		/// <param name="statementName">ステートメント名</param>
		/// <param name="param">検索パラメタ</param>
		/// <returns>分割済み注文リスト</returns>
		protected List<DataTable> GetDevidedOrderDatas(string statementName, Hashtable param)
		{
			// 注文情報取得
			var orders = ConvertProductOptionTexts(GetOrderData(statementName, param));
			// 最大行数毎に取得したデータを分割する
			string orderIdTmp = null;
			var groupCount = 0;
			DataTable tableOrders = null;
			var devidedOrders = new List<DataTable>();
			foreach (DataRowView orderItem in orders)
			{
				if (orderIdTmp != (string)orderItem[Constants.FIELD_ORDER_ORDER_ID])
				{
					// 比較キー更新
					orderIdTmp = (string)orderItem[Constants.FIELD_ORDER_ORDER_ID];
					// 最大行数毎にデータを分割
					if ((groupCount % CONST_OUTPUT_ORDER_MAX_COUNT) == 0)
					{
						// 新しい入れ物作成
						tableOrders = orders.Table.Clone();
						devidedOrders.Add(tableOrders);
					}
					// 注文IDの件数をカウント
					groupCount++;
				}
				tableOrders.Rows.Add(orderItem.Row.ItemArray);
			}
			return devidedOrders;
		}

		/// <summary>
		/// 注文明細出力処理
		/// </summary>
		/// <param name="dataSource">レポートデータソース</param>
		/// <param name="sOutPutStream">出力ストリーム</param>
		protected void RenderReport(ReportDataSource dataSource, Stream sOutPutStream)
		{
			try
			{
				// レポート設定
				var report = CreateReport();
				report.DataSources.Add(dataSource);
				// PDFとして出力
				Warning[] warning;
				string mimeType;
				string fileNameExtension;
				string encoding;
				string[] stream;
				var bytes = report.Render(
					"PDF",
					null,
					PageCountMode.Actual,
					out mimeType,
					out encoding,
					out fileNameExtension,
					out stream,
					out warning);
				sOutPutStream.Write(bytes, 0, bytes.Length);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("領収書の出力中にエラーが発生しました。", ex);
			}
		}

		/// <summary>
		/// LocalReportのインスタンスを取得
		/// </summary>
		/// <returns>初期設定済みReport</returns>
		protected LocalReport CreateReport()
		{
			var report = new LocalReport();
			report.LoadReportDefinition(new StringReader(CreateReportComposition()));
			report.SetParameters(GetReportParameters());
			return report;
		}

		/// <summary>
		/// レポート構成データを生成
		/// </summary>
		/// <returns>構成データ</returns>
		protected string CreateReportComposition()
		{
			// ベース読み込み
			var reportComposition = CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_BASE);
			// 各要素を取得し、ベースに追加
			reportComposition.Root.Add(CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_CODE).Root.Elements());
			reportComposition.Root.Add(CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_HEADERFOOTER).Root.Elements());
			reportComposition.Root.Add(CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_IMAGES).Root.Elements());
			reportComposition.Root.Add(CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_VARIABLES).Root.Elements());
			// レイアウトのみ更に分割されている為別処理
			reportComposition.Root.Add(CreateReportBodyParts().Root.Elements());
			return reportComposition.ToString();
		}

		/// <summary>
		/// レポート構成用パーツを生成する
		/// </summary>
		/// <param name="compositionFileName">構成用ファイル名</param>
		/// <returns>構成用パーツ</returns>
		protected XDocument CreateReportCompositionParts(string compositionFileName)
		{
			// ファイル読み込み
			var compositionFilePath = CreateTemplateFilePath(compositionFileName);
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
		/// <returns>BODY部分パーツ</returns>
		protected XDocument CreateReportBodyParts()
		{
			// ファイル読み込み
			var body = CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_BODY);
			// 名称が"BodyHeader"の要素を取得（Body部分のヘッダ挿入用）
			var bodyHeader = body.Descendants(body.Root.GetDefaultNamespace() + "Rectangle")
				.Where(element => element.Attribute("Name").Value == "BodyHeader")
				.ElementAt(0);
			// Body部分のヘッダ要素に分割された要素を挿入
			bodyHeader.Add(CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_BODY_HEADER).Root.Elements());
			return body;
		}

		/// <summary>
		/// テンプレートファイルパスを取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>ファイルパス</returns>
		protected string CreateTemplateFilePath(string fileName)
		{
			// ファイル存在チェック
			if (File.Exists(CreateCustomerResourceFilePath(Constants.PROJECT_NO, fileName)))
			{
				// 案件別構成ファイルパス設定
				return CreateCustomerResourceFilePath(Constants.PROJECT_NO, fileName);
			}
			// 案件別に用意されていない場合はデフォルト構成ファイルパス設定
			return CreateCustomerResourceFilePath(CONST_COMPOSITION_DEFAULT_DIRECTORY, fileName);
		}

		/// <summary>
		/// カスタマーリソースファイルパス作成
		/// </summary>
		/// <param name="storeDir">保存先ディレクトリ</param>
		/// <param name="fileName">ファイル名</param>
		/// <returns>カスタマーリソースファイルパス</returns>
		protected string CreateCustomerResourceFilePath(string storeDir, string fileName)
		{
			var filePath = Path.Combine(
				Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE,
				CONST_RECEIPT,
				storeDir,
				fileName);
			return filePath;
		}

		/// <summary>
		/// レポートパラメータ取得
		/// </summary>
		/// <returns></returns>
		protected List<ReportParameter> GetReportParameters()
		{
			var reportParameters = new List<ReportParameter>
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
					Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED.ToString())
			};
			return reportParameters;
		}
	}
}
