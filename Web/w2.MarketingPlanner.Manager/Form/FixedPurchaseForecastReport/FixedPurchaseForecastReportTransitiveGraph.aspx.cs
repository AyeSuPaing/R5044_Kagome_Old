/*
=========================================================================================================
  Module      : 定期売上予測推移グラフ(FixedPurchaseForecastReportTransitiveGraph .cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using w2.Common.Helper;
using w2.Common.Web;
using w2.Domain.FixedPurchaseForecast.Helper;

public partial class Form_FixedPurchaseForecastReport_FixedPurchaseForecastReportTransitiveGraph : BasePage
{
	private const string TARGET_YEAR_MONTH = "yearMonth";
	private const string TARGET_DATA_PRICE = "price";
	private const string TARGET_DATA_STOCK = "stock";

	private const string KBN_CHART_BAR = "bar";
	private const string KBN_CHART_LINE = "line";

	private const string KBN_VIEW_NUMBER = "0";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.ProductList = (FixedPurchaseForecastProductListSearchResult[])Session[Constants.SESSIONPARAM_KEY_FIXED_PURCHASE_FORECAST_INFO];

		var productId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_PRODUCT_ID]);
		var displayKbn = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_FIXEDPURCHASE_FORECAST_DISPLAY_KBN]);

		var targetProduct = (displayKbn == "product")
			? ProductList.Where(forcast => forcast.ProductId == productId).ToArray().FirstOrDefault()
			: ProductList.Where(forcast => forcast.VariationId == productId).ToArray().FirstOrDefault();

		lProductId.Text = HtmlSanitizer.HtmlEncode((displayKbn == "product") ? targetProduct.ProductId : targetProduct.VariationId);
		lProductName.Text = HtmlSanitizer.HtmlEncode(targetProduct.ProductName);

		var targetTable = new ArrayList();
		foreach (var target in targetProduct.Item)
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
		DataBind();
	}

	/// <summary>商品単位一覧</summary>
	protected FixedPurchaseForecastProductListSearchResult[] ProductList { get; private set; }
	/// <summary>対象のデータ値</summary>
	protected string TargetValue { get; private set; }
	/// <summary>チャートタイプ</summary>
	protected string ChartType { get; private set; }
	/// <summary>数値の表示可否</summary>
	protected bool IsViewNumber { get; private set; }
}