/*
=========================================================================================================
  Module      : トータルピッキングリスト出力クラス(TotalPickingListCreater.cs)
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
using w2.Common;
using w2.Common.Sql;
using w2.Common.Util;
using w2.App.Common.Order;
using w2.App.Common.OrderExtend;

namespace w2.App.Common.Pdf.PdfCreater
{
	/// <summary>
	/// トータルピッキングリスト作成クラス
	/// </summary>
	public class TotalPickingListCreater : BasePdfCreater
	{
		// １ファイルに出力する注文商品件数の上限値
		public const int CONST_OUTPUT_ORDER_MAX_COUNT = 10000;

		// データソース名
		public const string CONST_DATASOURCE_NAME_TOTAL_ORDER_ITEMS = "TotalOrderItems";

		// 構成要素ファイル名
		private const string CONST_COMPOSITION_FILENAME_BASE = @"TotalPickingList.rdlc";

		/// <summary>
		/// PDF出力（管理側で利用）
		/// </summary>
		/// <param name="sOutPutStream">出力用ストリーム</param>
		/// <param name="strSearchKbn">検索区分</param>
		/// <param name="htParam">検索パラメタ</param>
		public void Create(Stream sOutPutStream, string strSearchKbn, Hashtable htParam)
		{
			// 受注商品情報取得
			DataView dvOrders = GetOrderItemData(GetStatementName(strSearchKbn), htParam);
			ReportDataSource rdsDataSource = new ReportDataSource(CONST_DATASOURCE_NAME_TOTAL_ORDER_ITEMS, dvOrders.Table);
			
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
			List<DataTable> lDevidedOrderItems = GetDevidedOrderItemDatas(strStatement, htParam);

			//------------------------------------------------------
			// PDFファイル出力
			//------------------------------------------------------
			// 最大行数毎にPDFファイルを出力する
			string strTempFilePath = null;
			int iFileNums = 0;
			foreach (DataTable dtOrderItems in lDevidedOrderItems)
			{
				iFileNums++;

				// データソース作成
				ReportDataSource rdsDataSource = new ReportDataSource(CONST_DATASOURCE_NAME_TOTAL_ORDER_ITEMS, dtOrderItems);

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
		/// 最大行数毎に分割された注文商品リスト取得
		/// </summary>
		/// <param name="strStatementName">ステートメント名</param>
		/// <param name="htParam">検索パラメタ</param>
		/// <returns>分割済み注文リスト</returns>
		protected List<DataTable> GetDevidedOrderItemDatas(string strStatementName, Hashtable htParam)
		{
			//------------------------------------------------------
			// 注文情報取得
			//------------------------------------------------------
			DataView dvOrders = GetOrderItemData(strStatementName, htParam);

			//------------------------------------------------------
			// 注文情報データ分割処理
			//------------------------------------------------------
			// 最大ページ数毎に取得したデータを分割する
			int iGroupCount = 0;
			DataTable dtOrderItems = dvOrders.Table.Clone();
			List<DataTable> lDevidedOrderItems = new List<DataTable>();
			foreach (DataRowView drv in dvOrders)
			{
				if (iGroupCount > CONST_OUTPUT_ORDER_MAX_COUNT - 1)
				{
					lDevidedOrderItems.Add(dtOrderItems);
					iGroupCount = 0;
					dtOrderItems = dvOrders.Table.Clone();
				}

				dtOrderItems.Rows.Add(drv.Row.ItemArray);
				iGroupCount++;
			}
			if (dtOrderItems.Rows.Count > 0)
			{
				lDevidedOrderItems.Add(dtOrderItems);
			}

			return lDevidedOrderItems;
		}

		/// <summary>
		/// ピッキングリスト出力処理
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
				throw new ApplicationException("ピッキングリストの出力中にエラーが発生しました。", ex);
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

			return lrReport;
		}

		/// <summary>
		/// レポート構成データを生成
		/// </summary>
		/// <returns>構成データ</returns>
		protected string CreateReportComposition()
		{
			return CreateReportCompositionParts(CONST_COMPOSITION_FILENAME_BASE).ToString();
		}

		/// <summary>
		/// レポート構成用パーツを生成する
		/// </summary>
		/// <param name="strCompositionFileName">構成用ファイル名</param>
		/// <param name="tagReplacer">タグ置換処理</param>
		/// <returns>構成用パーツ</returns>
		protected XDocument CreateReportCompositionParts(string strCompositionFileName)
		{
			// ファイル読み込み
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
		/// テンプレートファイルパスを取得
		/// </summary>
		/// <param name="strFileName">ファイル名</param>
		/// <returns>ファイルパス</returns>
		protected string CreateTemplateFilePath(string strFileName)
		{
			// ファイル存在チェック
			if (File.Exists(CreateCustomerResourceFilePath(Constants.PROJECT_NO ,strFileName)))
			{
				// 案件別構成ファイルパス設定
				return CreateCustomerResourceFilePath(Constants.PROJECT_NO, strFileName);
			}
			else
			{
				// 案件別に用意されていない場合はデフォルト構成ファイルパス設定
				return CreateCustomerResourceFilePath(CONST_COMPOSITION_DEFAULT_DIRECTORY ,strFileName);
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
			return Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + @"PickingList\" + strStoreDir + @"\" + strFileName;
		}

		/// <summary>
		/// 注文情報取得
		/// </summary>
		/// <param name="strStatement">SQL Statement名</param>
		/// <param name="htParam">検索用パラメータ</param>
		/// <returns>注文情報一覧</returns>
		protected DataView GetOrderItemData(string strStatement, Hashtable htParam)
		{
			// QuerySettingファイル読み込み
			XDocument xdQuerySetting = CreateReportCompositionParts("Xml/PickingListSetting.xml");
			XElement xElement = xdQuerySetting.Element(Path.GetFileNameWithoutExtension(CONST_COMPOSITION_FILENAME_BASE)).Element("QuerySetting");
			string strSelect = xElement.Element("Select").Value;
			string strGroupBy = (xElement.Element("GroupBy").Value != "") ? "GROUP BY " + xElement.Element("GroupBy").Value : "";
			string strHaving = (xElement.Element("Having").Value != "") ? "HAVING " + xElement.Element("Having").Value : "";
			string strOrderBy = (xElement.Element("OrderBy").Value != "") ? "ORDER BY " + xElement.Element("OrderBy").Value : "";

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Order", strStatement))
			{
				// クエリ置換（「@@ where @@」など）
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ where @@", StringUtility.ToEmpty(htParam["@@ where @@"]));
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ select @@", strSelect);
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ groupby @@", strGroupBy);
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ having @@", strHaving);
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ orderby @@", strOrderBy);
				sqlStatement.ReplaceStatement("@@ multi_order_id @@", OrderCommon.GetOrderSearchMultiOrderId(htParam));
				sqlStatement.ReplaceStatement("@@ order_shipping_addr1 @@", StringUtility.ToEmpty(htParam[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]));
				sqlStatement.Statement = OrderExtendCommon.ReplaceOrderExtendFieldName(sqlStatement.Statement, Constants.TABLE_ORDER, StringUtility.ToEmpty(htParam[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]));

				sqlStatement.CommandTimeout = SqlServerTimeout;

				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htParam);
			}
		}

		/// <summary>
		/// ピッキングリスト商品アイテム総数取得
		/// </summary>
		/// <param name="searchKbn">検索区分</param>
		/// <param name="parameters">検索用パラメータ</param>
		/// <returns>ピッキングリスト商品アイテム総数</returns>
		public int GetOrderItemCount(string searchKbn, Hashtable parameters)
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Order", GetStatementName(searchKbn)))
			{
				// QuerySettingファイル読み込み
				XDocument querySetting = CreateReportCompositionParts("Xml/PickingListSetting.xml");
				XElement element = querySetting.Element(Path.GetFileNameWithoutExtension(CONST_COMPOSITION_FILENAME_BASE)).Element("QuerySetting");

				// クエリ置換（「@@ where @@」など）
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ where @@", StringUtility.ToEmpty(parameters["@@ where @@"]));
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ select @@", "Count(*)");
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ groupby @@",
					(element.Element("GroupBy").Value != "") ? "GROUP BY " + element.Element("GroupBy").Value : "");
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ having @@",
					(element.Element("Having").Value != "") ? "HAVING " + element.Element("Having").Value : "");
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ orderby @@", "");
				sqlStatement.ReplaceStatement("@@ multi_order_id @@", OrderCommon.GetOrderSearchMultiOrderId(parameters));
				sqlStatement.ReplaceStatement("@@ order_shipping_addr1 @@", StringUtility.ToEmpty(parameters[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]));
				sqlStatement.Statement = OrderExtendCommon.ReplaceOrderExtendFieldName(sqlStatement.Statement, Constants.TABLE_ORDER, StringUtility.ToEmpty(parameters[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]));

				sqlStatement.CommandTimeout = SqlServerTimeout;
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, parameters).Count;
			}
		}

		/// <summary>
		/// ステートメント設定
		/// </summary>
		protected new string GetStatementName(string strSearchKbn)
		{
			switch (strSearchKbn)
			{
				// 受注情報一覧
				case Constants.KBN_PDF_OUTPUT_ORDER:
					return "GetOrderItemListForPickingList";

				// 注文ワークフロー一覧
				case Constants.KBN_PDF_OUTPUT_ORDER_WORKFLOW:
					return "GetOrderItemWorkflowListForPickingList";

				default:
					// エラーページへ
					throw new ApplicationException("該当検索区分は存在しません");
			}
		}

		/// <summary>出力ファイルベースファイル名</summary>
		protected string FileBaseName
		{
			get { return "TotalPickingList" + DateTime.Now.ToString("yyyyMMdd"); }
		}
	}
}
