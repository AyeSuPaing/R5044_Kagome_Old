/*
=========================================================================================================
  Module      : 時間別レポートページ(TimeReport.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using w2.App.Common.Extensions.Currency;

public partial class Form_OrderConditionReport_TimeReport : BasePage
{
	#region 表示定数
	protected int m_iCurrentYear = DateTime.Now.Year;
	protected int m_iCurrentMonth = DateTime.Now.Month;

	protected const string KBN_REPORT_TYPE_MONTHTIME_REPORT = "3"; // 時間(月)表示
	protected const string KBN_REPORT_TYPE_DAYTIME_REPORT = "2"; // 時間(日)表示
	protected const string KBN_REPORT_TYPE_MONTHLY_REPORT = "1"; // 月表示
	protected const string KBN_REPORT_TYPE_DAILY_REPORT = "0"; // 日表示
	protected const string KBN_SALES_TYPE_ORDER_REPORT = "1"; // 注文基準
	protected const string KBN_SALES_TYPE_SHIP_REPORT = "0"; // 出荷基準

	/// <summary>時間上限設定</summary>
	protected const int CONST_TIME_VALUE = 24;

	protected const string KBN_OPERATION_TYPE_AMOUNT = "0"; // 金額表示
	protected const string KBN_OPERATION_TYPE_COUNT = "1"; // 件数表示

	protected const string KBN_AGGREGATE_UNIT_ORDER = "order"; // 合計（注文単位）

	protected const string FIELD_ORDERCONDITION_TARGET_TIME = "tgt_time"; //時間
	protected const string FIELD_ORDERCONDITION_ORDERED_AMOUNT = "ordered_amount"; // 売上金額(注文基準)
	protected const string FIELD_ORDERCONDITION_ORDERED_COUNT = "ordered_count"; // 売上件数(注文基準)
	protected const string FIELD_ORDERCONDITION_ORDERED_AMOUNT_AVG = "ordered_amount_avg"; // 売上平均購入単価(注文基準)
	protected const string FIELD_ORDERCONDITION_CANCELED_AMOUNT = "canceled_amount"; // キャンセル金額
	protected const string FIELD_ORDERCONDITION_CANCELED_COUNT = "canceled_count"; // キャンセル件数
	protected const string FIELD_ORDERCONDITION_SHIPPED_AMOUNT = "shipped_amount"; // 売上金額(出荷基準)
	protected const string FIELD_ORDERCONDITION_SHIPPED_COUNT = "shipped_count"; // 売上件数(出荷基準)
	protected const string FIELD_ORDERCONDITION_SHIPPED_AMOUNT_AVG = "shipped_amount_avg"; // 売上平均購入単価(出荷基準)
	protected const string FIELD_ORDERCONDITION_RETURNED_AMOUNT = "returned_amount"; // 返品金額
	protected const string FIELD_ORDERCONDITION_RETURNED_COUNT = "returned_count"; // 返品件数
	protected const string FIELD_ORDERCONDITION_SUBTOTAL_ORDERED_AMOUNT = "subtotal_ordered_amount"; // 売上小計金額(注文基準)
	protected const string FIELD_ORDERCONDITION_SUBTOTAL_ORDERED_COUNT = "subtotal_ordered_count"; // 売上小計件数(注文基準)
	protected const string FIELD_ORDERCONDITION_SUBTOTAL_SHIPPED_AMOUNT = "subtotal_shipped_amount"; // 売上小計金額(出荷基準)
	protected const string FIELD_ORDERCONDITION_SUBTOTAL_SHIPPED_COUNT = "subtotal_shipped_count"; // 売上小計件数(出荷基準)

	// 抽出条件定数
	private const string FIELD_ORDERCONDITION_ORDER_PAYMENT_KBNS = "@@ order_payment_kbns @@"; // 決済種別(複数)
	private const string FIELD_ORDERCONDITION_ORDER_KBNS = "@@ order_kbns @@"; // 注文区分(複数)
	private const string FIELD_ORDERCONDITION_COUNTRY_ISO_CODE_KBNS = "@@ country_iso_code_kbns @@"; // 国ISOコード区分(複数)
	private const string FIELD_ORDERCONDITION_ORDER_TYPES = "@@ order_types @@"; // 通常注文/定期注文(複数)
	private const string FIELD_ORDERCONDITION_AMOUNT_FIELD_NAME = "@@ amount_field_name @@"; // 金額項目名
	private const string FIELD_ORDERCONDITION_COUNT_FIELD_NAME = "@@ count_field_name @@"; // 数項目名
	private const string FIELD_ORDERCONDITION_TABLE_NAME = "@@ table_name @@"; // テーブル名
	private const string FIELD_ORDERCONDITION_SALES_KBN_TARGET = "@@ ym @@"; // 日別月別指定
	private const string FIELD_ORDERCONDITION_ITEM_SEARCH_CONDITION = "@@ item_search_condition @@"; // 商品検索条件

	/// <summary>ValueTextパラメータ：商品セール区分（通常）</summary>
	public const string VALUETEXT_PARAM_PRODUCTSALE_KBN_NOMAL = "NM";
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			this.SettingReportType = Request.QueryString["ReportSettingType"];
			this.ReportSalesType = Request.QueryString["ReportSalesType"];
			this.ReportTaxType = Request.QueryString["ReportTaxType"];
			this.aggregateUnit = Request.QueryString["aggregateUnit"];
			this.DisplayDataList = new ArrayList();
			// パラメタ取得・設定

			ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] = m_iCurrentYear;
			ViewState[Constants.REQUEST_KEY_CURRENT_MONTH] = m_iCurrentMonth;

			//表示情報設定
			var reportInfo = new StringBuilder();
			var reportInfoOrderConditionDays = new StringBuilder();
			var displayTypeName = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TYPE,
				this.HasOrderAggregateUnit
					? Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TYPE_ORDER
					: Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TYPE_PRODUCT);
			if (this.SettingReportType == KBN_REPORT_TYPE_DAILY_REPORT)
			{
				var periodFormat = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_PERIOD,
					KBN_REPORT_TYPE_DAYTIME_REPORT);
				reportInfo.Append(
					string.Format(
						periodFormat,
						displayTypeName,
						Request.QueryString["year"],
						Request.QueryString["month"],
						Request.QueryString["day"]));
			}
			else if (this.SettingReportType == KBN_REPORT_TYPE_MONTHLY_REPORT)
			{
				var periodFormat = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_PERIOD,
					KBN_REPORT_TYPE_MONTHTIME_REPORT);
				reportInfo.Append(
					string.Format(
						periodFormat,
						displayTypeName,
						Request.QueryString["year"],
						Request.QueryString["month"]));
			}

			lbReportInfo.Text = reportInfo + ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TAX,
				this.IsIncludedTax
					? Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TAX_INCLUDED
					: Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DISPLAY_KBN_TAX_EXCLUDED);
			lbReportInfoOrderConditionDays.Text = reportInfoOrderConditionDays.ToString();

			// 集計データ取得
			getData();

			// リピーターのデータバインド
			rDataList.DataBind();
		}
	}

	/// <summary>
	/// 売上状況取得
	/// </summary>
	/// <param name="input">検索パラメータ</param>
	/// <param name="isExchangeCancel">交換キャンセルか</param>
	/// <returns>売上状況</returns>
	private DataView GetOrderCondition(Hashtable input, bool isExchangeCancel = false)
	{
		var statementName = "";
		if (this.SettingReportType == KBN_REPORT_TYPE_DAILY_REPORT)
		{
			statementName = "GetOrderConditionDayTime";
		}
		else if (this.SettingReportType == KBN_REPORT_TYPE_MONTHLY_REPORT)
		{
			statementName = "GetOrderConditionMonthTime";
		}

		var amountFieldName = GetAmountFieldName();

		using (var sqlAccessor = new SqlAccessor())
		using (var sqlStatement = new SqlStatement("OrderConditionReport", statementName))
		{
			// 決済種別を置換
			sqlStatement.Statement = sqlStatement.Statement.Replace(
				FIELD_ORDERCONDITION_ORDER_PAYMENT_KBNS,
				input["order_payment_kbns"].ToString());
			// 購買区分を置換
			sqlStatement.Statement = sqlStatement.Statement.Replace(
				FIELD_ORDERCONDITION_ORDER_KBNS,
				input["order_kbns"].ToString());
			// 国ISOコード区分を置換
			sqlStatement.Statement = sqlStatement.Statement.Replace(
				FIELD_ORDERCONDITION_COUNTRY_ISO_CODE_KBNS,
				input["country_iso_code_kbns"].ToString());
			// 通常注文/定期注文
			sqlStatement.Statement = sqlStatement.Statement.Replace(
				FIELD_ORDERCONDITION_ORDER_TYPES,
				input["order_types"].ToString());
			// 金額項目名
			sqlStatement.Statement = sqlStatement.Statement.Replace(
				FIELD_ORDERCONDITION_AMOUNT_FIELD_NAME,
				amountFieldName.ToString());
			// 件数項目名
			sqlStatement.Statement = sqlStatement.Statement.Replace(
				FIELD_ORDERCONDITION_COUNT_FIELD_NAME,
				GetCountFieldName(isExchangeCancel));
			// テーブル名
			sqlStatement.Statement = sqlStatement.Statement.Replace(
				FIELD_ORDERCONDITION_TABLE_NAME,
				GetTableNameStatement(this.HasOrderAggregateUnit, isExchangeCancel));
			// 日別月別指定
			sqlStatement.Statement = sqlStatement.Statement.Replace(
				FIELD_ORDERCONDITION_SALES_KBN_TARGET,
				(this.SettingReportType == KBN_REPORT_TYPE_DAILY_REPORT) ? "ymd" : "ym");
			var isInputProductInfo = ((this.ProductId != "") || (this.BrandSearch != "") || (this.ProductSaleKbn != "")
				|| (this.ProductSaleName != ""));
			input["day"] = Request.QueryString["day"];
			input["month"] = Request.QueryString["month"];
			var timeFrom = StringUtility.ToEmpty(Request.QueryString[Constants.REQUEST_KEY_REPORT_TIME_FROM]);
			input[Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_TIME_FROM] = string.IsNullOrEmpty(timeFrom)
				? Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DEFAULT_START_TIME
				: timeFrom;
			var timeTo = StringUtility.ToEmpty(Request.QueryString[Constants.REQUEST_KEY_REPORT_TIME_TO]);
			input[Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_TIME_TO] = string.IsNullOrEmpty(timeTo)
				? Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DEFAULT_END_TIME
				: timeTo;
			var statement = GetProductSearchConditionStatement(
				this.HasOrderAggregateUnit,
				isInputProductInfo,
				this.ProductSaleKbn);
			sqlStatement.Statement = sqlStatement.Statement.Replace(
				FIELD_ORDERCONDITION_ITEM_SEARCH_CONDITION,
				statement);
			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
		}
	}

	/// <summary>
	/// データ取得・加工
	/// </summary>
	private void getData()
	{
		var input = (Hashtable)Session[Constants.SESSION_KEY_PARAM];

		// 売上状況取得
		var detailData = (DataView)null;
		var detailDataExchangeCancel = (DataView)null;
		detailData = GetOrderCondition(input);
		if (this.ReportSalesType == KBN_SALES_TYPE_SHIP_REPORT)
		{
			detailDataExchangeCancel = GetOrderCondition(input, true);
		}

		// データ格納
		var priceTotal = 0m;
		var countTotal = 0;
		var minusPriceTotal = 0m;
		var minusCountTotal = 0;
		var subtotalPriceTotal = 0m;
		var subtotalCountTotal = 0;

		var  dataCount = 0;
		for (var loop = 0; loop < CONST_TIME_VALUE; loop++)
		{
			var dataFlg = false;
			if (dataCount < detailData.Count)
			{
				if (this.SettingReportType != null)
				{
					dataFlg = true;
				}
			}

			// 売上取得
			var alLine = new ArrayList();
			if (this.ReportSalesType == KBN_SALES_TYPE_ORDER_REPORT)
			{
				// 注文基準
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_ORDERED_AMOUNT,
					dataFlg,
					KBN_OPERATION_TYPE_AMOUNT);
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_ORDERED_COUNT,
					dataFlg,
					KBN_OPERATION_TYPE_COUNT);
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_ORDERED_AMOUNT_AVG,
					dataFlg,
					KBN_OPERATION_TYPE_AMOUNT);
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_CANCELED_AMOUNT,
					dataFlg,
					KBN_OPERATION_TYPE_AMOUNT);
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_CANCELED_COUNT,
					dataFlg,
					KBN_OPERATION_TYPE_COUNT);
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_SUBTOTAL_ORDERED_AMOUNT,
					dataFlg,
					KBN_OPERATION_TYPE_AMOUNT);
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_SUBTOTAL_ORDERED_COUNT,
					dataFlg,
					KBN_OPERATION_TYPE_COUNT);

				if (dataFlg)
				{
					priceTotal += Convert.ToInt32(detailData[dataCount][FIELD_ORDERCONDITION_ORDERED_AMOUNT]);
					countTotal += (int)detailData[dataCount][FIELD_ORDERCONDITION_ORDERED_COUNT];
					minusPriceTotal += Convert.ToInt32(detailData[dataCount][FIELD_ORDERCONDITION_CANCELED_AMOUNT]);
					minusCountTotal += (int)detailData[dataCount][FIELD_ORDERCONDITION_CANCELED_COUNT];
					subtotalPriceTotal += Convert.ToInt32(
					detailData[dataCount][FIELD_ORDERCONDITION_SUBTOTAL_ORDERED_AMOUNT]);
					subtotalCountTotal += (int)detailData[dataCount][FIELD_ORDERCONDITION_SUBTOTAL_ORDERED_COUNT];
				}

			}
			else if (this.ReportSalesType == KBN_SALES_TYPE_SHIP_REPORT)
			{
				// 出荷基準
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_SHIPPED_AMOUNT,
					dataFlg,
					KBN_OPERATION_TYPE_AMOUNT);
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_SHIPPED_COUNT,
					dataFlg,
					KBN_OPERATION_TYPE_COUNT);
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_SHIPPED_AMOUNT_AVG,
					dataFlg,
					KBN_OPERATION_TYPE_AMOUNT);
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_RETURNED_AMOUNT,
					dataFlg,
					KBN_OPERATION_TYPE_AMOUNT,
					detailDataExchangeCancel[dataCount]);
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_RETURNED_COUNT,
					dataFlg,
					KBN_OPERATION_TYPE_COUNT,
					detailDataExchangeCancel[dataCount]);
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_SUBTOTAL_SHIPPED_AMOUNT,
					dataFlg,
					KBN_OPERATION_TYPE_AMOUNT,
					detailDataExchangeCancel[dataCount]);
				CreateDispData(
					alLine,
					detailData[dataCount],
					FIELD_ORDERCONDITION_SUBTOTAL_SHIPPED_COUNT,
					dataFlg,
					KBN_OPERATION_TYPE_COUNT,
					detailDataExchangeCancel[dataCount]);

				if (dataFlg)
				{
					priceTotal += (decimal)detailData[dataCount][FIELD_ORDERCONDITION_SHIPPED_AMOUNT];
					countTotal += (int)detailData[dataCount][FIELD_ORDERCONDITION_SHIPPED_COUNT];
					minusPriceTotal += (decimal)detailData[dataCount][FIELD_ORDERCONDITION_RETURNED_AMOUNT]
						+ (decimal)detailDataExchangeCancel[dataCount][FIELD_ORDERCONDITION_RETURNED_AMOUNT];
					minusCountTotal += (int)detailData[dataCount][FIELD_ORDERCONDITION_RETURNED_COUNT]
						+ (int)detailDataExchangeCancel[dataCount][FIELD_ORDERCONDITION_RETURNED_COUNT];
					subtotalPriceTotal += (decimal)detailData[dataCount][FIELD_ORDERCONDITION_SUBTOTAL_SHIPPED_AMOUNT];
					subtotalCountTotal += (int)detailData[dataCount][FIELD_ORDERCONDITION_SUBTOTAL_SHIPPED_COUNT];
				}
			}

			var tableData = new Hashtable();
			tableData[FIELD_ORDERCONDITION_TARGET_TIME] = loop.ToString();
			tableData.Add("data", alLine);
			this.DisplayDataList.Add(tableData);

			if (dataFlg == true)
			{
				dataCount++;
			}
		}

		// 平均購入金額設定
		var avgTotal = 0m;
		avgTotal = (priceTotal / (countTotal == 0 ? 1 : countTotal));

		// 合計
		lbPriceTotal.Text = priceTotal.ToPriceString(true);
		lbCountTotal.Text = StringUtility.ToNumeric(countTotal);
		lbAvgTotal.Text = avgTotal.ToPriceString(true);
		lbMinusPriceTotal.Text = minusPriceTotal.ToPriceString(true);
		lbMinusCountTotal.Text = StringUtility.ToNumeric(minusCountTotal);
		lbSubtotalPriceTotal.Text = subtotalPriceTotal.ToPriceString(true);
		lbSubtotalCountTotal.Text = StringUtility.ToNumeric(subtotalCountTotal);

		// 「月日平均購入金額 日 Or 月」
		var date = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN,
			(this.SettingReportType == KBN_REPORT_TYPE_DAILY_REPORT)
				? Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN_DAY
				: Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN_MONTH);
		// 「平均購入金額 Or 平均商品金額」
		tdDateAvgInfo.InnerText = date + ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_AVERAGE_KBN,
			this.HasOrderAggregateUnit
				? Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_AVERAGE_KBN_PURCHASE
				: Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_AVERAGE_KBN_PRODUCT);
		var averagePrice = (subtotalPriceTotal / (dataCount == 0 ? 1 : dataCount));
		tdDateAvgPriceInfo.InnerText = averagePrice.ToPriceString(true);
	}

	/// <summary>
	/// １カラム分表示データ作成
	/// </summary>
	/// <param name="addList">追加先リスト</param>
	/// <param name="addSource">追加元DataView</param>
	/// <param name="subjectColumns">対象カラム</param>
	/// <param name="insertData">実データ追加可否</param>
	/// <param name="typeOperation">表示タイプ</param>
	/// <param name="exchangeCancelSource">交換キャンセル</param>
	/// <returns>表示データの配列</returns>
	private void CreateDispData(
		ArrayList addList,
		DataRowView addSource,
		string subjectColumns,
		bool insertData,
		string typeOperation,
		DataRowView exchangeCancelSource = null)
	{
		if (insertData)
		{
			// 金額表示の場合
			if (typeOperation == KBN_OPERATION_TYPE_AMOUNT)
			{

				var exchangeCancel = (exchangeCancelSource != null) ? (decimal?)exchangeCancelSource[subjectColumns] : 0;
				addList.Add((Convert.ToInt32(addSource[subjectColumns]) + exchangeCancel).ToPriceString(true));
			}
			// 件数表示
			else if (typeOperation == KBN_OPERATION_TYPE_COUNT)
			{
				var exchangeCancel = (exchangeCancelSource != null) ? (int?)exchangeCancelSource[subjectColumns] : 0;
				addList.Add(
					string.Format(
						((this.HasOrderAggregateUnit)
							? ReplaceTag("@@DispText.common_message.unit_of_quantity@@")
							: ReplaceTag("@@DispText.common_message.unit_of_quantity2@@")),
						StringUtility.ToNumeric((int?)addSource[subjectColumns] + exchangeCancel)));
			}
		}
		else
		{
			addList.Add("－");
		}
	}

	/// <summary>
	/// 商品検索条件SQL文を取得
	/// </summary>
	/// <param name="hasOrderAggregateUnit">集計単位（注文基準か？）</param>
	/// <param name="isInputProductInfo">商品検索の入力値があるか？</param>
	/// <param name="productSaleKbn">商品セール区分選択値</param>
	/// <returns>商品検索条件SQL文</returns>
	private string GetProductSearchConditionStatement(
		bool hasOrderAggregateUnit,
		bool isInputProductInfo,
		string productSaleKbn)
	{
		return ((hasOrderAggregateUnit) || (isInputProductInfo == false))
			? ""
			: (productSaleKbn == VALUETEXT_PARAM_PRODUCTSALE_KBN_NOMAL)
				? "AND  (@product_id = '' OR w2_OrderItem.product_id = @product_id)  "
					+ "AND  (@brand_id = '' OR w2_OrderItem.brand_id = @brand_id)  "
					+ "AND  (w2_OrderItem.productsale_id = '')"
				: "AND  (@product_id = '' OR w2_OrderItem.product_id = @product_id)  "
					+ "AND  (@brand_id = '' OR w2_OrderItem.brand_id = @brand_id)  "
					+ "AND  (@productsale_kbn = '' OR w2_ProductSale.productsale_kbn = @productsale_kbn)  "
					+ "AND  (@productsale_id = '' OR w2_OrderItem.productsale_id = @productsale_id)";
	}

	/// <summary>
	/// 件数項目名
	/// </summary>
	/// <param name="isExchangeCancel">交換キャンセルか</param>
	/// <returns>件数項目名</returns>
	private string GetCountFieldName(bool isExchangeCancel = false)
	{
		var tableName = this.HasOrderAggregateUnit ? "w2_Order" : "w2_OrderItem";
		var countFieldName = string.Format(
			this.HasOrderAggregateUnit ? "COUNT({0}.order_id)" : "SUM({0}.item_quantity)",
			isExchangeCancel ? "OrderItems" : tableName);
		return countFieldName;
	}

	/// <summary>
	/// テーブル名SQL文を取得
	/// </summary>
	/// <param name="hasOrderAggregateUnit">集計単位（注文基準か？）</param>
	/// <param name="isExchangeCansel">交換キャンセルか</param>
	/// <returns>テーブル名SQL文</returns>
	private string GetTableNameStatement(bool hasOrderAggregateUnit, bool isExchangeCansel = false)
	{
		var tableName = "FROM  w2_Order  ";

		if (hasOrderAggregateUnit == false)
		{
			tableName += "LEFT JOIN  w2_OrderItem  ON  w2_Order.order_id = w2_OrderItem.order_id  "
				+ "LEFT JOIN  w2_ProductSale  ON  w2_OrderItem.productsale_id = w2_ProductSale.productsale_id  ";
		}
		else if (isExchangeCansel)
		{
			tableName += "LEFT JOIN  w2_OrderItem  ON  w2_Order.order_id = w2_OrderItem.order_id  ";
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			tableName += "LEFT JOIN w2_OrderOwner ON  w2_Order.order_id = w2_OrderOwner.order_id  ";
		}

		return tableName;
	}

	/// <summary>
	/// 金額項目名
	/// </summary>
	/// <param name="isExchangeCancel">交換キャンセルか</param>
	/// <returns></returns>
	private StringBuilder GetAmountFieldName(bool isExchangeCancel = false)
	{
		var amountFieldName = new StringBuilder();
		var itemtable = isExchangeCancel
			? "OrderItems"
			: "w2_OrderItem";

		if ((this.HasOrderAggregateUnit && (isExchangeCancel == false)))
		{
			amountFieldName.Append("w2_Order.order_price_total")
				.Append(this.IsIncludedTax ? "" : " - w2_Order.order_price_tax");
		}
		else
		{
			amountFieldName.Append(
				String.Format(
					(this.IsIncludedTax)
						? "{0}.product_price_pretax * {0}.item_quantity"
						: "{0}.product_price_pretax  * {0}.item_quantity - {0}.item_price_tax",
					itemtable));
		}

		return amountFieldName;
	}

	/// <summary>
	/// 商品別販売個数ランキングレポートURL作成（合計）
	/// </summary>
	/// <returns>商品別販売個数ランキングレポート</returns>
	protected string CreateProductSaleRankingReportlUrlForTotal()
	{
		var orderDetailUrl = new StringBuilder();
		orderDetailUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_PRODUCT_SALE_RANKING_REPORT);
		orderDetailUrl.Append("?").Append("total_flg").Append("=").Append("1");
		orderDetailUrl.Append("&").Append(Constants.REQUEST_KEY_TARGET_YEAR).Append("=").Append(HttpUtility.UrlEncode(m_iCurrentYear.ToString()));
		orderDetailUrl.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=1");
		orderDetailUrl.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));

		return orderDetailUrl.ToString();
	}

	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, System.EventArgs e)
	{
		var records = new StringBuilder();

		// タイトル作成
		var titleParams = new List<string>();
		titleParams.Add(
			string.Format(
				// 「時間別売上レポート {0} {1}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_TIME_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_TIME_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_STATUS_REPORT),
				lbReportInfo.Text,
				lbReportInfoOrderConditionDays.Text));
		records.Append(CreateRecordCsv(titleParams));
		titleParams.Clear();
		records.Append(CreateRecordCsv(titleParams));

		// ヘッダ作成
		var headerParams = new List<string>();
		headerParams.Add(
			string.Format(
				// 「{0}年」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TIME_KBN_YEAR),
				m_iCurrentYear));
		// 「件数 Or 個数」
		var orderUnit = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
			Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_COUNT_KBN,
			this.HasOrderAggregateUnit
				? Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_COUNT_KBN_RESULT_QUANTITY
				: Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_COUNT_KBN_ITEM_QUANTITY);
		if (this.SettingReportType == KBN_SALES_TYPE_ORDER_REPORT)
		{
			headerParams.Add(
				// 「売上(注文基準)：金額」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_ORDER_BASIS_AMOUNT));

			headerParams.Add(
				string.Format(
					// 「売上(注文基準)：{0}」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_ORDER_BASIS),
					orderUnit));

			headerParams.Add(
				// 「売上(注文基準)：平均購入単価」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_ORDER_BASIS_AVERAGE));

			headerParams.Add(
				// 「キャンセル：金額」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_CANCEL_AMOUNT));
			headerParams.Add(
				string.Format(
					// 「キャンセル：{0}」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_CANCEL),
					orderUnit));

			headerParams.Add(
				// 「小計：金額」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SUBTOTAL_AMOUNT));
			headerParams.Add(
				string.Format(
					// 「小計：{0}」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SUBTOTAL),
					orderUnit));
		}
		else if (this.SettingReportType == KBN_SALES_TYPE_SHIP_REPORT)
		{
			headerParams.Add(
				// 「売上(出荷基準)：金額」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SHIPPING_STANDARD_AMOUNT));
			headerParams.Add(
				string.Format(
					// 「売上(出荷基準)：{0}」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SHIPPING_STANDARD),
					orderUnit));

			headerParams.Add(
				// 「売上(出荷基準)：平均購入単価」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SHIPPING_STANDARD_AVERAGE));

			headerParams.Add(
				// 「返品：金額」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_RETURN_AMOUNT));
			headerParams.Add(
				string.Format(
					// 「返品：{0}」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_RETURN),
					orderUnit));

			headerParams.Add(
				// 「小計：金額」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
					Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SUBTOTAL_AMOUNT));
			headerParams.Add(
				string.Format(
					// 「小計：{0}」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN,
						Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_SALE_KBN_SUBTOTAL),
					orderUnit));
		}

		var fileName = new StringBuilder();
		fileName.Append("OrderConditionReportList_").Append(m_iCurrentYear.ToString()).Append(
		(this.SettingReportType == KBN_REPORT_TYPE_DAILY_REPORT ? m_iCurrentMonth.ToString("00") : ""));
		records.Append(CreateRecordCsv(headerParams));

		// データ作成
		var dataParams = new List<string>();
		// 合計
		dataParams.Add(
			// 「合計」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_LIST_TOTAL));
		dataParams.Add(lbPriceTotal.Text.Replace(@"¥", ""));
		dataParams.Add(lbCountTotal.Text);
		dataParams.Add(lbAvgTotal.Text.Replace(@"¥", ""));
		dataParams.Add(lbMinusPriceTotal.Text.Replace(@"¥", ""));
		dataParams.Add(lbMinusCountTotal.Text);
		dataParams.Add(lbSubtotalPriceTotal.Text.Replace(@"¥", ""));
		dataParams.Add(lbSubtotalCountTotal.Text);
		records.Append(CreateRecordCsv(dataParams));
		// 月別・日別
		foreach (Hashtable ht in this.DisplayDataList)
		{
			dataParams.Clear();
			dataParams.Add((string)ht[FIELD_ORDERCONDITION_TARGET_TIME]);
			foreach (string str in (ArrayList)ht["data"])
			{
				dataParams.Add(str.Replace(@"¥", "").Replace("件", ""));
			}

			records.Append(CreateRecordCsv(dataParams));
		}

		// ファイル出力
		OutPutFileCsv(fileName.ToString(), records.ToString());
	}
	/// <summary>集計単位</summary>
	protected bool HasOrderAggregateUnit
	{
		get { return (this.aggregateUnit == KBN_AGGREGATE_UNIT_ORDER); }
	}
	/// <summary>税込表示か</summary>
	protected bool IsIncludedTax
	{
		get { return (this.ReportTaxType == Constants.KBN_ORDERCONDITIONREPORT_TAX_INCLUDED); }
		set { this.IsIncludedTax = value; }
	}
	/// <summary>レポートデータ</summary>
	protected ArrayList DisplayDataList
	{
		get { return (ArrayList)ViewState["DisplayDataList"]; }
		set { ViewState["DisplayDataList"] = value; }
	}
	private ArrayList m_alDispData = new ArrayList();

	/// <summary>レポート種別(日・月)</summary>
	protected string SettingReportType
	{
		get { return (string)ViewState["SettingReportType"]; }
		set { ViewState["SettingReportType"] = value; }
	}
	/// <summary>レポート商品種別</summary>
	protected string ReportSalesType { get; set; }
	/// <summary>税込み表示制御</summary>
	protected string ReportTaxType { get; set; }
	/// <summary>ブランドID判別</summary>
	protected string BrandSearch { get; set; }
	/// <summary>セール区分</summary>
	protected string ProductSaleKbn { get; set; }
	/// <summary>セール名</summary>
	protected string ProductSaleName { get; set; }
	/// <summary>商品ID</summary>
	protected string ProductId { get; set; }
	/// <summary>集計単位</summary>
	protected string aggregateUnit { get; set; }
}