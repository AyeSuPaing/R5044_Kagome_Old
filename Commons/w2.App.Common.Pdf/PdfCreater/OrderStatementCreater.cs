/*
=========================================================================================================
  Module      : 受注明細出力クラス(OrderStatementCreater.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
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
using w2.App.Common.DataCacheController;
using w2.App.Common.Order;
using w2.App.Common.OrderExtend;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.Pdf.PdfCreater
{
	/// <summary>
	/// 受注明細書作成クラス
	/// </summary>
	public class OrderStatementCreater : BasePdfCreater
	{
		// １ファイルに出力する注文情報件数の上限値
		public const int CONST_OUTPUT_ORDER_MAX_COUNT = 500;

		// データソース名
		public const string CONST_DATASOURCE_NAME_ORDER_STATEMENT = "OrderStatement";

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
		/// <param name="sOutPutStream">出力用ストリーム</param>
		/// <param name="strSearchKbn">検索区分</param>
		/// <param name="htParam">検索パラメタ</param>
		public void Create(Stream sOutPutStream, string strSearchKbn, Hashtable htParam)
		{
			// ステートメント名取得
			string strStatementName = GetStatementName(strSearchKbn);

			// レポート用データソース作成
			DataView dvOrders = ConvertProductOptionTexts(GetOrderData(strStatementName, htParam));

			SetOrderExtendData(dvOrders);

			ReportDataSource rdsDataSource = new ReportDataSource(CONST_DATASOURCE_NAME_ORDER_STATEMENT, dvOrders.Table);

			// 実行
			RenderReport(rdsDataSource, sOutPutStream);
		}

		/// <summary>
		/// PDF作成（OrderPdfCreater.exeで利用）
		/// </summary>
		/// <param name="strSearchKbn">検索区分</param>
		/// <param name="htParam">検索パラメタ</param>
		/// <param name="strSessionId">セッションID</param>
		public void Create(string strSearchKbn, Hashtable htParam, string strSessionId)
		{
			//------------------------------------------------------
			// データソース（注文情報）取得
			//------------------------------------------------------
			// ステートメント名取得
			string strStatement = GetStatementName(strSearchKbn);

			// レポート用データソース取得
			List<DataTable> lDevidedOrders = GetDevidedOrderDatas(strStatement, htParam);

			//------------------------------------------------------
			// PDFファイル出力
			//------------------------------------------------------
			// 最大行数毎にPDFファイルを出力する
			string strTempFilePath = null;
			int iFileNums = 0;
			foreach (DataTable dtOrders in lDevidedOrders)
			{
				iFileNums++;

				// データソース作成
				ReportDataSource rdsDataSource = new ReportDataSource(CONST_DATASOURCE_NAME_ORDER_STATEMENT, dtOrders);

				// 出力ファイル設定
				strTempFilePath = TempDirPath + FileBaseName + "_" + iFileNums.ToString("00000") + ".pdf." + strSessionId;

				// 出力処理
				using (FileStream fs = new FileStream(strTempFilePath, FileMode.OpenOrCreate, FileAccess.Write))
				{
					// 実行
					RenderReport(rdsDataSource, fs);

					// ストリームを閉じてSessionIdを付与してある拡張子を消してファイルの移動をする
					fs.Close();
				}

				// 一時ファイルを正式な場所へ移動する
				File.Move(strTempFilePath, ExportDirPath + Path.GetFileNameWithoutExtension(strTempFilePath));
			}
		}

		/// <summary>
		/// 最大行数毎に分割された注文情報リスト取得
		/// </summary>
		/// <param name="strStatementName">ステートメント名</param>
		/// <param name="htParam">検索パラメタ</param>
		/// <returns>分割済み注文リスト</returns>
		protected List<DataTable> GetDevidedOrderDatas(string strStatementName, Hashtable htParam)
		{
			//------------------------------------------------------
			// 注文情報取得
			//------------------------------------------------------
			DataView dvOrders = ConvertProductOptionTexts(GetOrderData(strStatementName, htParam));

			SetOrderExtendData(dvOrders);

			//------------------------------------------------------
			// 注文情報データ分割処理
			//------------------------------------------------------
			// 最大行数毎に取得したデータを分割する
			string strOrderIdTmp = null;
			int iGroupCount = 0;
			DataTable dtOrders = null;
			List<DataTable> lDevidedOrders = new List<DataTable>();
			foreach (DataRowView drvOrderItem in dvOrders)
			{
				if (strOrderIdTmp != (string)drvOrderItem[Constants.FIELD_ORDER_ORDER_ID])
				{
					// 比較キー更新
					strOrderIdTmp = (string)drvOrderItem[Constants.FIELD_ORDER_ORDER_ID];

					// 最大行数毎にデータを分割
					if ((iGroupCount % CONST_OUTPUT_ORDER_MAX_COUNT) == 0)
					{
						// 新しい入れ物作成
						dtOrders = dvOrders.Table.Clone();
						lDevidedOrders.Add(dtOrders);
					}

					// 注文IDの件数をカウント
					iGroupCount++;
				}

				dtOrders.Rows.Add(drvOrderItem.Row.ItemArray);
			}
			
			return lDevidedOrders;
		}

		/// <summary>
		/// 注文明細出力処理
		/// </summary>
		/// <param name="rdsDataSource">レポートデータソース</param>
		/// <param name="sOutPutStream">出力ストリーム</param>
		protected void RenderReport(ReportDataSource rdsDataSource, Stream sOutPutStream)
		{
			Warning[] warning;
			string mimeType;
			string fileNameExtension;
			string encoding;
			string[] stream;

			try
			{
				// レポート設定
				LocalReport lrReport = CreateReport();
				lrReport.DataSources.Add(rdsDataSource);

				// PDFとして出力
				byte[] bytes = lrReport.Render("PDF", null, PageCountMode.Actual, out mimeType, out encoding, out fileNameExtension, out stream, out warning);
				sOutPutStream.Write(bytes, 0, bytes.Length);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("受注明細書の出力中にエラーが発生しました。", ex);
			}
		}

		/// <summary>
		/// LocalReportのインスタンスを取得
		/// </summary>
		/// <returns>初期設定済みReport</returns>
		protected LocalReport CreateReport()
		{
			LocalReport lrReport = new LocalReport();

			lrReport.LoadReportDefinition(new StringReader(CreateReportComposition()));
			lrReport.SetParameters(GetReportParameters());

			return lrReport;
		}

		/// <summary>
		/// レポート構成データを生成
		/// </summary>
		/// <returns>構成データ</returns>
		protected string CreateReportComposition()
		{
			//------------------------------------------------------
			// レポート構成データ生成処理
			//------------------------------------------------------
			// ベース読み込み
			XDocument xdReportComposition = CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_BASE);

			// 各要素を取得し、ベースに追加
			xdReportComposition.Root.Add(CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_CODE).Root.Elements());
			xdReportComposition.Root.Add(CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_HEADERFOOTER).Root.Elements());
			xdReportComposition.Root.Add(CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_IMAGES).Root.Elements());
			xdReportComposition.Root.Add(CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_VARIABLES).Root.Elements());

			// レイアウトのみ更に分割されている為別処理
			xdReportComposition.Root.Add(CreateReportBodyParts().Root.Elements());

			return xdReportComposition.ToString();
		}

		/// <summary>
		/// レポート構成用パーツを生成する
		/// </summary>
		/// <param name="strCompositionFileName">構成用ファイル名</param>
		/// <param name="tagReplacer">タグ置換処理</param>
		/// <returns>構成用パーツ</returns>
		protected XDocument CreateReportCompositionParts(string strCompositionFileName)
		{
			//------------------------------------------------------
			// ファイル読み込み
			//------------------------------------------------------
			string strCompositionFilePath = CreateTemplateFilePath(strCompositionFileName);
			try
			{
				return XDocument.Load(strCompositionFilePath);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("ファイル「" + strCompositionFilePath + "」の読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// ボディ部パーツ生成
		/// </summary>
		/// <returns>BODY部分パーツ</returns>
		protected XDocument CreateReportBodyParts()
		{
			//------------------------------------------------------
			// ファイル読み込み
			//------------------------------------------------------
			XDocument xdBody = CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_BODY);

			// 名称が"BodyHeader"の要素を取得（Body部分のヘッダ挿入用）
			XElement xeBodyHeader = (from element in xdBody.Descendants(xdBody.Root.GetDefaultNamespace() + "Rectangle")
									 where element.Attribute("Name").Value == "BodyHeader"
									 select element).ElementAt(0);

			// Body部分のヘッダ要素に分割された要素を挿入
			xeBodyHeader.Add(CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_BODY_HEADER).Root.Elements());

			return xdBody;
		}

		/// <summary>
		/// テンプレートファイルパスを取得
		/// </summary>
		/// <param name="strFileName">ファイル名</param>
		/// <returns>ファイルパス</returns>
		protected string CreateTemplateFilePath(string strFileName)
		{
			// ファイル存在チェック
			if (File.Exists(CreateCustomerResourceFilePath(Constants.PROJECT_NO, strFileName)))
			{
				// 案件別構成ファイルパス設定
				return CreateCustomerResourceFilePath(Constants.PROJECT_NO, strFileName);
			}
			else
			{
				// 案件別に用意されていない場合はデフォルト構成ファイルパス設定
				return CreateCustomerResourceFilePath(CONST_COMPOSITION_DEFAULT_DIRECTORY, strFileName);
			}
		}

		/// <summary>
		/// カスタマーリソースファイルパス作成
		/// </summary>
		/// <param name="strStoreDir">保存先ディレクトリ</param>
		/// <param name="strFileName">ファイル名</param>
		/// <returns>カスタマーリソースファイルパス</returns>
		protected string CreateCustomerResourceFilePath(string strStoreDir, string strFileName)
		{
			return Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + @"OrderStatement\" + strStoreDir + @"\" + strFileName;
		}

		/// <summary>
		/// レポートパラメータ取得
		/// </summary>
		/// <returns></returns>
		protected List<ReportParameter> GetReportParameters()
		{
			List<ReportParameter> reportParametors = new List<ReportParameter>();
			reportParametors.Add(new ReportParameter("point_option_enabled", Constants.W2MP_POINT_OPTION_ENABLED.ToString()));
			reportParametors.Add(new ReportParameter("coupon_option_enabled", Constants.W2MP_COUPON_OPTION_ENABLED.ToString()));
			reportParametors.Add(new ReportParameter("member_rank_option_enabled", Constants.MEMBER_RANK_OPTION_ENABLED.ToString()));
			reportParametors.Add(new ReportParameter("setpromotion_option_enabled", Constants.SETPROMOTION_OPTION_ENABLED.ToString()));
			reportParametors.Add(new ReportParameter("gift_order_option_enabled", Constants.GIFTORDER_OPTION_ENABLED.ToString()));
			reportParametors.Add(new ReportParameter("corporation_option_enabled", Constants.DISPLAY_CORPORATION_ENABLED.ToString()));
			reportParametors.Add(new ReportParameter("fixed_purchase_option_enabled", Constants.FIXEDPURCHASE_OPTION_ENABLED.ToString()));
			reportParametors.Add(new ReportParameter("currency_format", Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.PdfFormat));
			reportParametors.Add(new ReportParameter("currency_localeId", Constants.GLOBAL_CONFIGS.GlobalSettings.KeyCurrency.LocaleId));
			reportParametors.Add(new ReportParameter("management_included_tax_flag", Constants.MANAGEMENT_INCLUDED_TAX_FLAG.ToString()));
			reportParametors.Add(new ReportParameter("global_option_enabled", Constants.GLOBAL_OPTION_ENABLE.ToString()));
			reportParametors.Add(new ReportParameter(
				"product_option_settings_price_grant_enabled",
				Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED.ToString()));
			return reportParametors;
		}

		/// <summary>出力ファイルベースファイル名</summary>
		protected string FileBaseName
		{
			get { return "OrderStatement" + DateTime.Now.ToString("yyyyMMdd"); }
		}

		/// <summary>
		/// 注文データに注文拡張項目をセット
		/// </summary>
		/// <param name="orderData">注文データ</param>
		private void SetOrderExtendData(DataView orderData)
		{
			if (Constants.ORDER_EXTEND_OPTION_ENABLED == false) return;

			foreach (DataRowView order in orderData)
			{
				var orderExtend = OrderExtendCommon.ConvertOrderExtend(order);
				var orderExtendFrontDisplay = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData
					.SettingModelsForFront.Select(
						s =>
						{
							var value = (orderExtend.ContainsKey(s.SettingId))
								? OrderExtendCommon.GetValueDisplayName(
									s.InputType,
									s.InputDefault,
									orderExtend[s.SettingId].Value)
								: string.Empty;
							var result = s.SettingName + " : " + value;
							return result;
						}).ToArray();

				order[Constants.FIELD_ORDER_MEMO] = order[Constants.FIELD_ORDER_MEMO]
					+ Environment.NewLine
					+ string.Join(Environment.NewLine, orderExtendFrontDisplay);
			}

		}
	}
}
