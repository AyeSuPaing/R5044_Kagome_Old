/*
=========================================================================================================
  Module      : 注文関連ファイル出力ページ処理(OrderFileExport.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using w2.App.Common.DataExporters;
using w2.App.Common.Global.Config;
using w2.App.Common.Order;
using w2.App.Common.Order.Workflow;
using w2.App.Common.OrderExtend;
using w2.Domain.MasterExportSetting.Helper;

/// <summary>
/// 注文関連ファイル出力ページ処理
/// </summary>
public partial class Form_OrderFileExport_OrderFileExport : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender">エベント元</param>
	/// <param name="e">エベント引数</param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 対象データ取得
		//------------------------------------------------------
		// 対象ステートメント取得
		var orderPage = Request[Constants.REQUEST_KEY_ORDERFILE_ORDERPAGE];
		
		if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_INTERACTION_DATA_LINK]) == false)
		{
			int fileIndex;
			if (int.TryParse(Request[Constants.REQUEST_KEY_INTERACTION_DATA_LINK], out fileIndex))
			{
				var searchParams = (Hashtable)Session[Constants.SESSION_KEY_PARAM];

				// 指定ファイルの連携用データ設定を取得
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();

					using (var statement = new SqlStatement("Order", "GetIDOnlyForOrderWorkflow"))
					{
						statement.Statement = statement.Statement.Replace(
							OrderSearchParam.WORKFLOWSETTING_WHERE,
							StringUtility.ToEmpty(searchParams[OrderSearchParam.WORKFLOWSETTING_WHERE]));
						statement.Statement = OrderExtendCommon.ReplaceOrderExtendFieldName(
							statement.Statement,
							Constants.TABLE_ORDER,
							StringUtility.ToEmpty(searchParams[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]));
						searchParams.Remove(OrderSearchParam.WORKFLOWSETTING_WHERE);
						statement.ExecStatement(accessor, searchParams);
					}

					// Create data exporter
					var dataExporter = DataExporterCreater.CreateDataExporter(Constants.PROJECT_NO, fileIndex, accessor);
					dataExporter.Process(Response);
				}
			}
		}
		else
		{
			var setting = new OrderFileExportShippingLabel().GetShippingLabelExportSettingList()[int.Parse(Request[Constants.REQUEST_KEY_SHIPPING_LABEL_LINK])];
			var sqlStatementName = (orderPage == Constants.PAGE_MANAGER_ORDER_LIST) ? setting.GetOrderListPageSqlStatementName() : setting.GetOrderWorkFlowPageSqlStatementName();

			// SQL発行
			DataView order = null;
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("Order", sqlStatementName))
			{
				var htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
				if (setting.UnitType == "SQL")
				{
					var sqlStatement = (orderPage == Constants.PAGE_MANAGER_ORDER_LIST)
						? setting.SqlExport
						: setting.SqlExport.Replace("ORDER_SEARCH_WHERE", "ORDERWORKFLOW_SEARCH_WHERE");
					statement.Statement = sqlStatement;
				}
				statement.ReplaceStatement("@@ fields @@", setting.SelectSql);
				statement.Statement = statement.Statement.Replace("@@ where @@", StringUtility.ToEmpty(htParam["@@ where @@"]));
				statement.ReplaceStatement("@@ orderby @@", OrderCommon.GetOrderSearchOrderByStringForOrderListAndWorkflow((string)htParam["sort_kbn"]));
				statement.ReplaceStatement("@@ multi_order_id @@", OrderCommon.GetOrderSearchMultiOrderId(htParam));
				statement.ReplaceStatement("@@ order_shipping_addr1 @@", StringUtility.ToEmpty(htParam[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]));
				statement.Statement = OrderExtendCommon.ReplaceOrderExtendFieldName(statement.Statement, Constants.TABLE_ORDER, StringUtility.ToEmpty(htParam[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]));
				order = statement.SelectSingleStatementWithOC(accessor, (Hashtable)Session[Constants.SESSION_KEY_PARAM]);
			}

			//------------------------------------------------------
			// マスタ情報出力
			//------------------------------------------------------
			Response.ContentEncoding = setting.FileEncoding;
			Response.ContentType = (setting.FormatType == "csv")
				? (setting.Separator == OrderFileExportShippingLabel.ShippingLabelExportSetting.TAB)
					? "text/tab-separated-values" : "application/csv" 
				: (Constants.MASTEREXPORT_EXCELFORMAT == ".xlsx")
					? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/vnd.ms-excel";
			Response.AppendHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(setting.FileName, Encoding.UTF8));

			//------------------------------------------------------
			// データ加工
			//------------------------------------------------------
			if (setting.FormatType == "csv")
			{
				Response.Write((new OrderFileExportShippingLabel()).CreateCsv(order, setting));
			}
			else if (setting.FormatType == "Excel")
			{
				// Excel出力処理を行う
				var excelTemplateSetting = new ExcelTemplateSetting(
					Path.Combine(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER, @"Contents\MasterExportExcelTemplate\TemplateBase.xml"),
					Path.Combine(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER, @"Contents\MasterExportExcelTemplate\TemplateElements.xml")
				);
				var metaMasterExport = new ExportToExcel(excelTemplateSetting);

				var formatDate =
					(Constants.GLOBAL_OPTION_ENABLE
						&& (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false))
						? GlobalConfigUtil.GetDateTimeFormatText(
							Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE,
							DateTimeUtility.FormatType.ShortDateHourMinuteSecondNoneServerTime)
						: string.Empty;

				metaMasterExport.Create(order, Response.OutputStream, formatDate);
			}
		}
		Response.End();
	}
}
