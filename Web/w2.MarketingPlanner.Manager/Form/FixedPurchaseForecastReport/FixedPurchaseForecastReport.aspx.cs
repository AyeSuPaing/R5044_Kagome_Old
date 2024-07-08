/*
=========================================================================================================
  Module      : 定期売上予測レポート(FixedPurchaseForecastReport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.Common.Extensions;
using w2.Common.Helper;
using w2.Common.Web;
using w2.Domain.FixedPurchaseForecast;
using w2.Domain.FixedPurchaseForecast.Helper;
using w2.Domain.ShopShipping;

public partial class Form_FixedPurchaseForecastReport_FixedPurchaseForecastReport : BasePage
{
	#region 定数
	protected const string FIXEDPURCHASEFORECASTREPORT_CALCULATION_SHIPPED_DATE = "SHIPPED_DATE";
	protected const string FIXEDPURCHASEFORECASTREPORT_CALCULATION_SCHEDULED_SHIPPING_DATE = "SCHEDULED_SHIPPING_DATE";

	protected const string KBN_DISPLAY_PRODUCT = "product";
	protected const string KBN_DISPLAY_PRODUCT_VARIATION = "productVariation";
	protected const string KBN_DISPLAY_MONTHLY = "monthly";

	protected const string TARGET_YEAR_MONTH = "yearMonth";
	protected const string TARGET_DATA_PRICE = "price";
	protected const string TARGET_DATA_STOCK = "stock";

	protected const string KBN_CHART_BAR = "bar";
	protected const string KBN_CHART_LINE = "line";

	protected const string KBN_VIEW_NUMBER = "0";
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
			InitializeDisplayType();
			SetSalesForecastCalculationCriteria();
			SetDisplayKbn(Request);

			if (this.IsMonthlyDisplay == false)
			{
				InitializeShippingType();
			}

			Display();
		}
	}

	/// <summary>
	/// 売上予測算出基準セット
	/// </summary>
	private void SetSalesForecastCalculationCriteria()
	{
		var defaultValue = Constants.FIXEDPURCHASEFORECASTREPORT_CALCULATION_DEFAULT_VALUE;
		switch (defaultValue)
		{
			case FIXEDPURCHASEFORECASTREPORT_CALCULATION_SCHEDULED_SHIPPING_DATE:
				rblSalesForecastCalculationCriteria.SelectedIndex = Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE ? 1 : 0;
				break;

			default:
				rblSalesForecastCalculationCriteria.SelectedIndex = 0;
				break;
		}
	}

	/// <summary>
	/// 表示区分セット
	/// </summary>
	/// <param name="hrRequest">パラメタが格納されたHttpRequest</param>
	private void SetDisplayKbn(HttpRequest hrRequest)
	{
		var requestKbn = StringUtility.ToEmpty(hrRequest[Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_DISPLAY_KBN]);
		switch (requestKbn)
		{
			case KBN_DISPLAY_PRODUCT:
				this.DisplayKbn = KBN_DISPLAY_PRODUCT;
				rblDisplayType.SelectedIndex = 1;
				break;

			case KBN_DISPLAY_PRODUCT_VARIATION:
				this.DisplayKbn = KBN_DISPLAY_PRODUCT_VARIATION;
				rblDisplayType.SelectedIndex = 2;
				break;

			default:
				this.DisplayKbn = KBN_DISPLAY_MONTHLY;
				rblDisplayType.SelectedIndex = 0;
				break;
		}
	}

	/// <summary>
	/// パラメタ取得
	/// </summary>
	/// <returns>パラメタが格納されたHashtable</returns>
	private FixedPurchaseForecastListSearchCondition GetParameters()
	{
		return new FixedPurchaseForecastListSearchCondition
		{
			ShopId = this.LoginOperatorShopId,
			ProductId = Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_PRODUCT_ID],
			ProductName = Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_PRODUCT_NAME],
			ShippingType = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_SHIPPING_NAME]),
			CategoryId = Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_CATEGORY_ID]
		};
	}

	/// <summary>
	/// 検索条件を画面にセット
	/// </summary>
	/// <param name="serarchCondition">検索条件</param>
	private void SetSearchConditionForDisplay(FixedPurchaseForecastListSearchCondition serarchCondition)
	{
		tbProductId.Text = serarchCondition.ProductId;
		tbProductName.Text = serarchCondition.ProductName;
		tbCategoryId.Text = serarchCondition.CategoryId;
		ddlShippingType.SelectedValue = serarchCondition.ShippingType;
	}

	/// <summary>
	/// 表示タイプ初期化
	/// </summary>
	private void InitializeDisplayType()
	{
		rblSalesForecastCalculationCriteria.Items.Add(
			new ListItem(ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_SALES_FORECAST_CALCULATION_CRITERIA,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_SALES_FORECAST_CALCULATION_CRITERIA_SHIPPED_DATE),
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_SALES_FORECAST_CALCULATION_CRITERIA_SHIPPED_DATE));
		rblSalesForecastCalculationCriteria.Items.Add(
			new ListItem(ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_SALES_FORECAST_CALCULATION_CRITERIA,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_SALES_FORECAST_CALCULATION_CRITERIA_SCHEDULED_SHIPPING_DATE),
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_SALES_FORECAST_CALCULATION_CRITERIA_SCHEDULED_SHIPPING_DATE));
		rblDisplayType.Items.Add(
			new ListItem(ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_DISPLAY_KBN,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_DISPLAY_KBN_MONTHLY),
					KBN_DISPLAY_MONTHLY));
		rblDisplayType.Items.Add(
			new ListItem(ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_DISPLAY_KBN,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_DISPLAY_KBN_PRODUCT),
					KBN_DISPLAY_PRODUCT));
		rblDisplayType.Items.Add(
			new ListItem(ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT, 
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_DISPLAY_KBN,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_DISPLAY_KBN_PRODUCT_VARIATION),
					KBN_DISPLAY_PRODUCT_VARIATION));
	}

	/// <summary>
	/// 配送種別初期化
	/// </summary>
	private void InitializeShippingType()
	{
		ddlShippingType.Items.Add(new ListItem("", ""));
		var shopShippings = new ShopShippingService().GetAll(this.LoginOperatorShopId);
		ddlShippingType.Items.AddRange(
			shopShippings.Select(shipping => new ListItem(shipping.ShopShippingName, shipping.ShippingId)).ToArray());
	}

	/// <summary>
	/// 表示処理
	/// </summary>
	private void Display()
	{
		switch (rblDisplayType.SelectedValue)
		{
			case KBN_DISPLAY_PRODUCT:
				ChangeDisplayProductOrProductVariation();
				this.ProductList = DisplayProduct();
				break;

			case KBN_DISPLAY_PRODUCT_VARIATION:
				ChangeDisplayProductOrProductVariation();
				this.ProductList = DisplayProductVariation();
				break;

			default:
				ChangeDisplayMonthly();
				DisplayMonthly();
				CreateGraph();
				break;
		}

		if (this.IsMonthlyDisplay == false)
		{
			InitializeShippingType();

			rProductList.DataSource = this.ProductList;

			// 推移グラフ表示用
			Session[Constants.SESSIONPARAM_KEY_FIXED_PURCHASE_FORECAST_INFO] = this.ProductList;
		}
		CreateDisplayDate();

		DataBind();
	}

	/// <summary>
	/// グラフ選択変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblChartType_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		CreateGraph();

		DisplayMonthly();

		DataBind();
	}

	/// <summary>
	/// 数値表示選択変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblCheckNumber_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		CreateGraph();

		DisplayMonthly();

		DataBind();
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		var url = CreateFixedPurchaseForecastReportListUrl(GetSearchInfo(), 1);
		Response.Redirect((url));
	}

	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, System.EventArgs e)
	{
		var target = new DataView();
		var header = string.Empty;
		var contents = string.Empty;

		switch (this.DisplayKbn)
		{
			case KBN_DISPLAY_PRODUCT:
				target = GetCsvProductExportData(false);
				header = CreateProductHeaderRecord(target);
				contents = CreateContentsRecordFromProduct(target);
				break;

			case KBN_DISPLAY_PRODUCT_VARIATION:
				target = GetCsvProductExportData(true);
				header = CreateProductHeaderRecord(target);
				contents = CreateContentsRecordFromProduct(target);
				break;
				
			default:
				target = GetCsvMonthlyExportData();
				header = CreateMonthlyHeaderRecord(target);
				contents = CreateContentsRecordFromMonthly(target);
				break;
		}

		var fileName = string.Format("FixedPurchaseForecastReport_{0}", DateTime.Now.ToString("yyyyMMddHHmm"));

		OutPutFileCsv(fileName, header + "\r\n" + contents);
	}

	/// <summary>
	/// ヘッダーレコード作成(商品単位orバリエーション単位)
	/// </summary>
	/// <param name="dataView">データ</param>
	/// <returns>ヘッダー文字列</returns>
	private string CreateProductHeaderRecord(DataView dataView)
	{
		var column = dataView.GetColumn();
		column = column.Where(c => (c != "row_count")).ToArray();
		var result = string.Join(",", column);

		return result;
	}

	/// <summary>
	/// ヘッダーレコード作成(月単位)
	/// </summary>
	/// <param name="dataView">データ</param>
	/// <returns>ヘッダー文字列</returns>
	private string CreateMonthlyHeaderRecord(DataView dataView)
	{
		var column = dataView.GetColumn();
		var result = string.Join(",", column);

		return result;
	}

	/// <summary>
	/// csv出力データ取得(商品単位orバリエーション単位)
	/// </summary>
	/// <param name="isVariation">バリエーション単位で取得するか</param>
	/// <returns>表示用データ</returns>
	private DataView GetCsvProductExportData(bool isVariation)
	{
		var searchCondition = GetParameters();
		var frequencyField = rblSalesForecastCalculationCriteria.SelectedValue == Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_SALES_FORECAST_CALCULATION_CRITERIA_SHIPPED_DATE
			? Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY
			: Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY_BY_SCHEDULED_SHIPPING_DATE;

		return isVariation
			? new FixedPurchaseForecastService().SearchTargetProductVariationForCsvAtDataView(searchCondition, frequencyField)
			: new FixedPurchaseForecastService().SearchTargetProductForCsvAtDataView(searchCondition, frequencyField);
	}

	/// <summary>
	/// csv出力データ取得(月単位)
	/// </summary>
	/// <param name="frequencyField">頻度</param>
	/// <returns>表示用データ</returns>
	private DataView GetCsvMonthlyExportData()
	{
		var frequencyField = rblSalesForecastCalculationCriteria.SelectedValue == Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_SALES_FORECAST_CALCULATION_CRITERIA_SHIPPED_DATE
			? Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY
			: Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY_BY_SCHEDULED_SHIPPING_DATE;
		return new FixedPurchaseForecastService().GetMonthlyForCsvAtDataView(frequencyField);
	}

	/// <summary>
	/// コンテンツレコード作成(商品単位orバリエーション単位)
	/// </summary>
	/// <param name="products">一覧</param>
	/// <returns>コンテンツ文字列</returns>
	private string CreateContentsRecordFromProduct(DataView products)
	{
		var result = new StringBuilder();

		foreach (DataRowView product in products)
		{
			result.Append(StringUtility.EscapeCsvColumn((product[Constants.FIELD_FIXEDPURCHASEFORECAST_TARGET_MONTH]).ToString()));
			result.Append(",").Append(StringUtility.EscapeCsvColumn((string)product[Constants.FIELD_FIXEDPURCHASEFORECAST_SHOP_ID]));
			result.Append(",").Append(StringUtility.EscapeCsvColumn((string)product[Constants.FIELD_FIXEDPURCHASEFORECAST_PRODUCT_ID]));
			if (this.DisplayKbn == KBN_DISPLAY_PRODUCT_VARIATION)
			{
				result.Append(",").Append(StringUtility.EscapeCsvColumn((string)product[Constants.FIELD_FIXEDPURCHASEFORECAST_VARIATION_ID]));
			}
			result.Append(",").Append(StringUtility.EscapeCsvColumn((string)product["product_name"]));
			result.Append(",").Append(StringUtility.EscapeCsvColumn(product["target_month_later_sales"].ToString()));
			result.Append(",").Append(StringUtility.EscapeCsvColumn(product["target_month_later_stock"].ToString()));

			result.Append("\r\n");
		}

		return result.ToString();
	}

	/// <summary>
	/// コンテンツレコード作成(月単位)
	/// </summary>
	/// <param name="monthly">一覧</param>
	/// <returns>コンテンツ文字列</returns>
	private string CreateContentsRecordFromMonthly(DataView monthly)
	{
		var result = new StringBuilder();

		foreach (DataRowView month in monthly)
		{
			result.Append(StringUtility.EscapeCsvColumn((month[Constants.FIELD_FIXEDPURCHASEFORECAST_TARGET_MONTH]).ToString()));
			result.Append(",").Append(StringUtility.EscapeCsvColumn(month["target_month_later_sales"].ToString()));
			result.Append(",").Append(StringUtility.EscapeCsvColumn(month["target_month_later_stock"].ToString()));
			result.Append("\r\n");
		}

		return result.ToString();
	}

	/// <summary>
	/// 表示タイプ変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblDisplayType_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		Display();
	}

	/// <summary>
	/// 商品単位表示
	/// </summary>
	private FixedPurchaseForecastProductListSearchResult[] DisplayProduct()
	{
		this.DisplayKbn = KBN_DISPLAY_PRODUCT;

		var searchCondition = GetParameters();
		SetSearchConditionForDisplay(searchCondition);

		var productLists = GetProductList(searchCondition, this.PageNo, false);

		var totalCounts = 0;
		var nextUrl = CreateFixedPurchaseForecastReportListUrl(searchCondition);

		if (productLists != null && productLists.Length > 0)
		{
			totalCounts = productLists[0].RowCount;
			this.IsSerachNoHit = false;
		}
		else
		{
			trListError.Visible = true;
			tProductList.Visible = false;
			this.IsSerachNoHit = true;

			tdErrorMessage.InnerHtml = HtmlSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
		}
		lbPager.Text = WebPager.CreateDefaultListPager(totalCounts, this.PageNo, nextUrl);

		return productLists;
	}

	/// <summary>
	/// バリエーション単位表示
	/// </summary>
	private FixedPurchaseForecastProductListSearchResult[] DisplayProductVariation()
	{
		this.DisplayKbn = KBN_DISPLAY_PRODUCT_VARIATION;
		var searchCondition = GetParameters();
		SetSearchConditionForDisplay(searchCondition);

		var products = GetProductList(searchCondition, this.PageNo, true);

		int totalCounts = 0;
		var nextUrl = CreateFixedPurchaseForecastReportListUrl(searchCondition);

		if ((products != null) && (products.Length > 0))
		{
			totalCounts = products[0].RowCount;
			this.IsSerachNoHit = false;
		}
		else
		{
			trListError.Visible = true;
			tProductList.Visible = false;
			this.IsSerachNoHit = true;

			tdErrorMessage.InnerHtml = HtmlSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
		}
		lbPager.Text = WebPager.CreateDefaultListPager(totalCounts, this.PageNo, nextUrl);

		return products;
	}

	/// <summary>
	/// 月単位表示
	/// </summary>
	private void DisplayMonthly()
	{
		this.DisplayKbn = KBN_DISPLAY_MONTHLY;
		var service = new FixedPurchaseForecastService();
		var frequencyField = rblSalesForecastCalculationCriteria.SelectedValue == Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_SALES_FORECAST_CALCULATION_CRITERIA_SHIPPED_DATE
			? Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY
			: Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY_BY_SCHEDULED_SHIPPING_DATE;
		this.MonthlyList = service.GetMonthly(frequencyField);
		rMonthly.DataSource = this.MonthlyList;

		CreateGraph();
	}

	/// <summary>
	/// グラフ作成
	/// </summary>
	private void CreateGraph()
	{
		var targetTable = new List<Hashtable>();
		foreach (var target in MonthlyList)
		{
			var targetData = new Hashtable();
			targetData.Add(TARGET_YEAR_MONTH, target.TargetMonth.ToString("yy/MM"));
			targetData.Add(TARGET_DATA_PRICE, target.Price);
			targetData.Add(TARGET_DATA_STOCK, target.Stock);
			targetTable.Add(targetData);
		}

		this.IsViewNumber = (rblCheckNumber.SelectedValue == KBN_VIEW_NUMBER);
		this.TargetValue = SerializeHelper.SerializeJson(targetTable);
		this.ChartType = (rblChartType.SelectedValue == KBN_CHART_LINE) ? KBN_CHART_LINE : KBN_CHART_BAR;
	}

	/// <summary>
	/// 表示用日付作成
	/// </summary>
	private void CreateDisplayDate()
	{
		if ((this.IsSerachNoHit == false) && (this.IsMonthlyDisplay == false))
		{
			this.DisplayMonthlyList = this.ProductList[0].Item.Select(model => DateTimeUtility.ToStringForManager(model.TargetMonth, DateTimeUtility.FormatType.LongYearMonth)).ToArray();
		}
	}


	/// <summary>
	/// 商品単位またはバリエーション単位表示切替
	/// </summary>
	private void ChangeDisplayProductOrProductVariation()
	{
		trSearchBox.Visible = true;
		trProductTable.Visible = true;
		trMonthlyTable.Visible = false;
	}

	/// <summary>
	/// 月単位表示表示切替
	/// </summary>
	private void ChangeDisplayMonthly()
	{
		trSearchBox.Visible = false;
		trProductTable.Visible = false;
		trMonthlyTable.Visible = true;
	}

	/// <summary>
	/// 検索情報取得
	/// </summary>
	/// <returns>検索情報</returns>
	private FixedPurchaseForecastListSearchCondition GetSearchInfo()
	{
		return new FixedPurchaseForecastListSearchCondition
		{
			ShopId = this.LoginOperatorShopId,
			ProductId = tbProductId.Text.Trim(),
			ProductName = tbProductName.Text.Trim(),
			ShippingType = ddlShippingType.SelectedValue,
			CategoryId = tbCategoryId.Text.Trim()
		};
	}

	/// <summary>
	/// 定期予測売上一覧遷移URL作成
	/// </summary>
	/// <param name="searchCondition">検索情報</param>
	/// <param name="pageNumber">表示開始記事番号</param>
	/// <returns>クーポン一覧遷移URL</returns>
	private string CreateFixedPurchaseForecastReportListUrl(FixedPurchaseForecastListSearchCondition searchCondition, int pageNumber)
	{
		var url = CreateFixedPurchaseForecastReportListBaseUrl(searchCondition)
				.AddParam(Constants.REQUEST_KEY_PAGE_NO, pageNumber.ToString())
			.CreateUrl();

		return url;
	}

	/// <summary>
	/// 定期予測売上一覧遷移URL作成
	/// </summary>
	/// <param name="searchCondition">検索条件</param>
	/// <returns>クーポン一覧遷移URL</returns>
	private string CreateFixedPurchaseForecastReportListUrl(FixedPurchaseForecastListSearchCondition searchCondition)
	{
		var url = CreateFixedPurchaseForecastReportListBaseUrl(searchCondition).CreateUrl();
		return url;
	}

	/// <summary>
	/// 定期予測売上一覧遷移ベースURL作成
	/// </summary>
	/// <param name="searchCondition">検索条件</param>
	/// <returns>クーポン一覧遷移URL</returns>
	private UrlCreator CreateFixedPurchaseForecastReportListBaseUrl(FixedPurchaseForecastListSearchCondition searchCondition)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_FIXED_PURCHASE_FORECAST_REPORT)
			.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_PRODUCT_ID, searchCondition.ProductId)
			.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_PRODUCT_NAME, searchCondition.ProductName)
			.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_SHIPPING_NAME, searchCondition.ShippingType)
			.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_CATEGORY_ID, searchCondition.CategoryId)
			.AddParam(Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_DISPLAY_KBN, this.DisplayKbn);

		return url;
	}

	/// <summary>
	/// 定期予測出荷取得(商品単位or商品バリエーション単位)
	/// </summary>
	/// <param name="searchCondiction">検索条件情報</param>
	/// <param name="pageNumber">表示開始記事番号</param>
	/// <param name="isVariation">バリエーション単位で取得するか</param>
	/// <returns>定期出荷予測一覧</returns>
	private FixedPurchaseForecastProductListSearchResult[] GetProductList(FixedPurchaseForecastListSearchCondition searchCondiction, int pageNumber, bool isVariation)
	{
		// 開始行～終了行の検索条件を設定
		var begin = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (pageNumber - 1) + 1;
		var end = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * pageNumber;

		searchCondiction.BeginRowNumber = begin;
		searchCondiction.EndRowNumber = end;
		var frequencyField = rblSalesForecastCalculationCriteria.SelectedValue == Constants.VALUETEXT_PARAM_FIXED_PURCHASE_FORECAST_REPORT_SALES_FORECAST_CALCULATION_CRITERIA_SHIPPED_DATE
			? Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY
			: Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY_BY_SCHEDULED_SHIPPING_DATE;
		return isVariation ? new FixedPurchaseForecastService().SearchTargetProductVariation(searchCondiction, frequencyField) : new FixedPurchaseForecastService().SearchTargetProduct(searchCondiction, frequencyField);
	}

	/// <summary>月単位表示か</summary>
	protected bool IsMonthlyDisplay
	{
		get { return (this.DisplayKbn == KBN_DISPLAY_MONTHLY); }
	}
	/// <summary>月単位表示か</summary>
	protected string DisplayKbn
	{
		get { return (string)ViewState["fixed_purchase_forecast_display_kbn"]; }
		private set { ViewState["fixed_purchase_forecast_display_kbn"] = value; }
	}
	/// <summary>商品単位一覧</summary>
	protected FixedPurchaseForecastProductListSearchResult[] ProductList { get; private set; }
	/// <summary>月単位一覧</summary>
	protected FixedPurchaseForecastItemSearchResult[] MonthlyList
	{
		get { return (FixedPurchaseForecastItemSearchResult[])ViewState["fixed_purchase_forecast_monthly_list"]; }
		private set { ViewState["fixed_purchase_forecast_monthly_list"] = value; }
	}
	/// <summary>表示用日付一覧</summary>
	protected string[] DisplayMonthlyList { get; private set; }
	/// <summary>検索ヒットしてないか</summary>
	protected bool IsSerachNoHit { get; private set; }
	/// <summary>対象のデータ値</summary>
	protected string TargetValue { get; private set; }
	/// <summary>チャートタイプ</summary>
	protected string ChartType { get; private set; }
	/// <summary>数値の表示可否</summary>
	protected bool IsViewNumber { get; private set; }
	/// <summary>ページ番号</summary>
	private int PageNo
	{
		get
		{
			int result;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out result)
				? result
				: 1;
		}
	}
}
