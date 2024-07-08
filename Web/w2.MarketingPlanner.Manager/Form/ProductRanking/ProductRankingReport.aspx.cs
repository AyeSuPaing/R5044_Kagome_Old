/*
=========================================================================================================
  Module      : 商品ランキングレポートページ処理(ProductRankingReport.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Region.Currency;
using w2.Common.Helper;

public partial class Form_ProductRanking_ProductRankingReport : BasePage
{
	protected ArrayList m_alTableData = new ArrayList();

	protected int m_iCurrentYear = DateTime.Now.AddDays(-1).Year;
	protected int m_iCurrentMonth = DateTime.Now.AddDays(-1).Month;

	protected string m_strProductType = Constants.KBN_RANKING_PRODUCT_SEARCH_WORD;
	protected string m_strProductValue = "";
	protected string m_strProductAccessKbn = "";

	private const string REQUEST_KEY_TYPE_CHART_TYPE = "ct";

	protected const string KBN_CHART_COLUMN = "0";
	protected const string KBN_CHART_LINE = "1";

	protected const string KBN_VIEW_NUMBER = "0";

	protected const string TABLE_HEADER_DAY = "day";
	protected const string TABLE_DATA_VALUE = "value";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// パラメタ取得
			//------------------------------------------------------
			// 年月取得
			if (Request[Constants.REQUEST_KEY_CURRENT_YEAR] != null)
			{
				m_iCurrentYear = int.Parse(Request[Constants.REQUEST_KEY_CURRENT_YEAR]);
				m_iCurrentMonth = int.Parse(Request[Constants.REQUEST_KEY_CURRENT_MONTH]);
			}

			ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] = m_iCurrentYear;
			ViewState[Constants.REQUEST_KEY_CURRENT_MONTH] = m_iCurrentMonth;

			// グラフタイプ選択状態取得・設定
			if (Request[REQUEST_KEY_TYPE_CHART_TYPE] != null)
			{
				foreach (ListItem li in rblChartType.Items)
				{
					li.Selected = (li.Value == int.Parse(Request[REQUEST_KEY_TYPE_CHART_TYPE]).ToString());
				}
			}

			// 税込・抜表示区分
			if (Request[Constants.REQUEST_KEY_TAX_DISPLAY_TYPE] != null)
			{
				this.IsIncludedTax = (Request[Constants.REQUEST_KEY_TAX_DISPLAY_TYPE]
					!= Constants.KBN_ORDERCONDITIONREPORT_TAX_EXCLUDE);
			}

			// 商品ランキング表示情報
			if (Request[Constants.REQUEST_KEY_RANKING_PRODUCT_TYPE] == Constants.KBN_RANKING_PRODUCT_SEARCH_WORD
				|| Request[Constants.REQUEST_KEY_RANKING_PRODUCT_TYPE] == Constants.KBN_RANKING_PRODUCT_BUY_COUNT
				|| Request[Constants.REQUEST_KEY_RANKING_PRODUCT_TYPE] == Constants.KBN_RANKING_PRODUCT_BUY_PRICE)
			{
				m_strProductType = (string)Request[Constants.REQUEST_KEY_RANKING_PRODUCT_TYPE];
				m_strProductValue = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RANKING_PRODUCT_VALUE]);
				m_strProductAccessKbn =
					StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RANKING_PRODUCT_ACCESS_KBN]);
			}
			else
			{
				// エラーページへ（システムエラー）
				Response.Redirect(
					Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR + "?"
					+ Constants.REQUEST_KEY_ERRORPAGE_ERRORKBN + "="
					+ HttpUtility.UrlEncode(WebMessages.ERRMSG_SYSTEM_ERROR));
			}

			ViewState[Constants.REQUEST_KEY_RANKING_PRODUCT_TYPE] = m_strProductType;
			ViewState[Constants.REQUEST_KEY_RANKING_PRODUCT_VALUE] = m_strProductValue;
			ViewState[Constants.REQUEST_KEY_RANKING_PRODUCT_ACCESS_KBN] = m_strProductAccessKbn;
		}
		else
		{
			// ビューステートから復元
			m_iCurrentYear = (int)ViewState[Constants.REQUEST_KEY_CURRENT_YEAR];
			m_iCurrentMonth = (int)ViewState[Constants.REQUEST_KEY_CURRENT_MONTH];
			m_strProductType = (string)ViewState[Constants.REQUEST_KEY_RANKING_PRODUCT_TYPE];
			m_strProductValue = (string)ViewState[Constants.REQUEST_KEY_RANKING_PRODUCT_VALUE];
			m_strProductAccessKbn = (string)ViewState[Constants.REQUEST_KEY_RANKING_PRODUCT_ACCESS_KBN];
		}

		//------------------------------------------------------
		// レポート情報取得
		//------------------------------------------------------
		var alReportInfo = GetReportInfo();

		//------------------------------------------------------
		// コンポーネント初期化
		//------------------------------------------------------
		Initialize();

		//------------------------------------------------------
		// 集計データ取得
		//------------------------------------------------------
		var dvDisplayData = GetData();

		//------------------------------------------------------
		// 集計データ加工（月初～月末までのデータを用意）
		//------------------------------------------------------
		// まずは当月の最終日を取得
		var dTtemp = new DateTime(m_iCurrentYear, m_iCurrentMonth, 1);
		var iLastDay = dTtemp.AddMonths(1).AddDays(-1).Day;

		// データ加工
		var iDRVCount = 0;
		for (var iDayLoop = 1; iDayLoop <= iLastDay; iDayLoop++)
		{
			string strValue;
			if ((iDRVCount < dvDisplayData.Count) && (iDayLoop == int.Parse(
				dvDisplayData[iDRVCount][Constants.FIELD_DISPSUMMARYANALYSIS_TGT_DAY].ToString())))
			{
				strValue = dvDisplayData[iDRVCount][(m_strProductType == Constants.KBN_RANKING_PRODUCT_BUY_PRICE)
					? "price"
					: "counts"].ToString();
				decimal totalPrice;
				decimal totalTax;
				if ((m_strProductType == Constants.KBN_RANKING_PRODUCT_BUY_PRICE) && (this.IsIncludedTax == false)
					&& (decimal.TryParse(strValue, out totalPrice)) && (decimal.TryParse(
						dvDisplayData[iDRVCount]["price_tax"].ToString(),
						out totalTax)))
				{
					strValue = (totalPrice - totalTax).ToString();
				}

				iDRVCount++;
			}
			else
			{
				strValue = "0";
			}

			var htTableData = new Hashtable();
			htTableData.Add(TABLE_HEADER_DAY, iDayLoop.ToString());
			htTableData.Add(TABLE_DATA_VALUE, strValue);
			m_alTableData.Add(htTableData);
		}

		//------------------------------------------------------
		// 商品ランキング月間推移グラフ作成
		//------------------------------------------------------
		// 数値表示するかチェック
		var isViewNumber = (rblCheckNumber.SelectedValue == KBN_VIEW_NUMBER);
		var jsonValues = SerializeHelper.SerializeJson(m_alTableData);
		string chartType = "";
		switch (rblChartType.SelectedValue)
		{
			case KBN_CHART_COLUMN:
				chartType = "bar";
				break;
			case KBN_CHART_LINE:
				chartType = "line";
				break;
		}
		CreateFunction(m_iCurrentMonth, jsonValues, chartType, (string)alReportInfo[0], (string)alReportInfo[1], (string)alReportInfo[2], isViewNumber);

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		DataBind();
	}

	/// <summary>
	/// 商品ランキングレポートURL作成
	/// </summary>
	/// <returns>商品ランキングレポートURL</returns>
	protected string CreateProductRankingReportUrl(string strYear, string strMonth)
	{
		var sbResult = new StringBuilder();

		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_PRODUCT_RANKING_REPORT);
		sbResult.Append("?");
		sbResult.Append(Constants.REQUEST_KEY_CURRENT_YEAR).Append("=").Append(HttpUtility.UrlEncode(strYear));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_CURRENT_MONTH).Append("=").Append(HttpUtility.UrlEncode(strMonth));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_RANKING_PRODUCT_TYPE).Append("=").Append(
			HttpUtility.UrlEncode((string)ViewState[Constants.REQUEST_KEY_RANKING_PRODUCT_TYPE]));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_RANKING_PRODUCT_VALUE).Append("=").Append(
			HttpUtility.UrlEncode((string)ViewState[Constants.REQUEST_KEY_RANKING_PRODUCT_VALUE]));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_RANKING_PRODUCT_ACCESS_KBN).Append("=").Append(
			HttpUtility.UrlEncode((string)ViewState[Constants.REQUEST_KEY_RANKING_PRODUCT_ACCESS_KBN]));
		sbResult.Append("&");
		sbResult.Append(REQUEST_KEY_TYPE_CHART_TYPE).Append("=")
			.Append(HttpUtility.UrlEncode(rblChartType.SelectedValue));
		sbResult.Append("&");
		sbResult.Append(Constants.REQUEST_KEY_TAX_DISPLAY_TYPE).Append("=").Append(
			HttpUtility.UrlEncode(
				this.IsIncludedTax
					? Constants.KBN_ORDERCONDITIONREPORT_TAX_INCLUDED
					: Constants.KBN_ORDERCONDITIONREPORT_TAX_EXCLUDE));

		return sbResult.ToString();
	}

	/// <summary>
	/// 前の月を選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbMonthBefore_Click(object sender, EventArgs e)
	{
		var dtDate = DateTime.Parse(
			ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] + "/" + ViewState[Constants.REQUEST_KEY_CURRENT_MONTH]
			+ "/01");
		dtDate = dtDate.AddMonths(-1);

		Response.Redirect(CreateProductRankingReportUrl(dtDate.Year.ToString(), dtDate.Month.ToString("00")));
	}

	/// <summary>
	/// 次の月を選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbMonthNext_Click(object sender, EventArgs e)
	{
		var dtDate = DateTime.Parse(
			ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] + "/" + ViewState[Constants.REQUEST_KEY_CURRENT_MONTH]
			+ "/01");
		dtDate = dtDate.AddMonths(1);

		Response.Redirect(CreateProductRankingReportUrl(dtDate.Year.ToString(), dtDate.Month.ToString("00")));
	}

	/// <summary>税込表示か</summary>
	protected bool IsIncludedTax
	{
		get { return (bool)ViewState[Constants.REQUEST_KEY_TAX_DISPLAY_TYPE]; }
		set { ViewState[Constants.REQUEST_KEY_TAX_DISPLAY_TYPE] = value; }
	}

	/// <summary>
	/// グラフ作成のFunction呼び出し
	/// </summary>
	/// <param name="month">月</param>
	/// <param name="values">データ値</param>
	/// <param name="chartType">グラフの種類</param>
	/// <param name="xAxisTitle">X軸のラベルタイトル</param>
	/// <param name="label">x軸の各目盛りのラベル</param>
	/// <param name="valueUnit">データ値の単位</param>
	/// <param name="isViewNumber">数値の表示の可否</param>
	private void CreateFunction(int month, string values, string chartType, string xAxisTitle, string label, string valueUnit, bool isViewNumber)
	{
		lScriptFunction.Text = "<script>CreateCharts('" + month + "', " + values + ", '" + chartType + "', '" + xAxisTitle + "', '" + label
			+ "', '" + valueUnit + "', '" + isViewNumber + "', 'ranking')</script>";
	}


	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void Initialize()
	{
		var sbInfo = new StringBuilder();
		var sbDate = new StringBuilder();
		string strAccessKbn;

		//------------------------------------------------------
		// 商品ランキング情報設定
		//------------------------------------------------------
		sbDate.Append(ViewState[Constants.REQUEST_KEY_CURRENT_YEAR]).Append("年");
		sbDate.Append(ViewState[Constants.REQUEST_KEY_CURRENT_MONTH]).Append("月");

		switch (m_strProductAccessKbn)
		{
			case "1":
				strAccessKbn = "PC";
				break;
			case "2":
				strAccessKbn =
					// 「モバイル」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
						Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_ACCESS_KBN,
						Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_ACCESS_KBN_MOBILE);
				break;
			case "3":
				strAccessKbn =
					// 「スマフォ」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
						Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_ACCESS_KBN,
						Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_ACCESS_KBN_SMARTPHONE);
				break;
			default:
				strAccessKbn =
					// 「全体」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
						Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_ACCESS_KBN,
						Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_ACCESS_KBN_ALL);
				break;
		}

		if (m_strProductType == Constants.KBN_RANKING_PRODUCT_SEARCH_WORD)
		{
			sbInfo.Append(
				// 「商品検索ワードランキング」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RANKING_INFO,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_PRODUTC_SEARCH_WORD_RANKING));
			sbInfo.Append(
				// 「検索ワード:」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RANKING_INFO,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_SEARCH_WORD)).Append(m_strProductValue);
			sbInfo.Append(
				// 「対象:」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RANKING_INFO,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_TARGET)).Append(m_strProductValue).Append(strAccessKbn);
		}
		else if (m_strProductType == Constants.KBN_RANKING_PRODUCT_BUY_COUNT)
		{
			sbInfo.Append(
				// 「商品販売個数ランキング」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RANKING_INFO,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_PRODUCT_SALES_QUANTITY));
			sbInfo.Append(string.Format("{0}:", ReplaceTag("@@Product.product_id.name@@")))
				.Append(m_strProductValue);

			sbInfo.Append(
				// 「対象:」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RANKING_INFO,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_TARGET)).Append(strAccessKbn);
		}
		else if (m_strProductType == Constants.KBN_RANKING_PRODUCT_BUY_PRICE)
		{
			sbInfo.Append(
				// 「商品販売金額ランキング」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RANKING_INFO,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_PRODUCT_AMOUNT_QUANTITY));
			sbInfo.Append(string.Format("{0}:", ReplaceTag("@@Product.product_id.name@@")))
				.Append(m_strProductValue);

			sbInfo.Append(
				// 「対象:」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RANKING_INFO,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_TARGET)).Append(strAccessKbn);
			sbInfo.Append(this.IsIncludedTax
				? string.Format("({0})", ReplaceTag("@@DispText.tax_type.included@@"))
				: string.Format("({0})", ReplaceTag("@@DispText.tax_type.excluded@@")));
		}

		lbDate.Text = sbDate.ToString();
		lbRankingInfo.Text = sbInfo.ToString();
	}

	/// <summary>
	/// 選択レポート情報取得
	/// </summary>
	/// <returns>ラベル文字格納配列</returns>
	/// <remarks>戻り値は横軸、縦軸の順に格納</remarks>
	private ArrayList GetReportInfo()
	{
		var alResult = new ArrayList();

		if (m_strProductType == Constants.KBN_RANKING_PRODUCT_SEARCH_WORD)
		{
			alResult.Add(
				// 「日付」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_REPORT_INFO,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_DATE));

			alResult.Add(
				// 「検索回数」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_REPORT_INFO,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_NUMBER_SEARCH));
			alResult.Add(string.Format(
				ReplaceTag("@@DispText.common_message.times@@"),
				string.Empty));
		}
		else if (m_strProductType == Constants.KBN_RANKING_PRODUCT_BUY_COUNT)
		{
			alResult.Add(
				// 「日付」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_REPORT_INFO,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_DATE));
			alResult.Add(
				// 「販売個数」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_REPORT_INFO,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_NUMBER_UNITS));
			alResult.Add(string.Format(
				ReplaceTag("@@DispText.common_message.unit_of_quantity2@@"),
				string.Empty));
		}
		else if (m_strProductType == Constants.KBN_RANKING_PRODUCT_BUY_PRICE)
		{
			alResult.Add(
				// 「日付」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_REPORT_INFO,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_DATE));
			alResult.Add(
				// 「販売金額」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_REPORT_INFO,
					Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_NUMBER_SALES));
			alResult.Add(CurrencyManager.KeyCurrencyUnit);
		}

		return alResult;
	}

	/// <summary>
	/// 集計データ取得
	/// </summary>
	private DataView GetData()
	{
		DataView dvResult;

		string strStatementName = null;
		switch (m_strProductType)
		{
			case Constants.KBN_RANKING_PRODUCT_SEARCH_WORD:
				strStatementName = "GetSearchWordNum";
				break;
			case Constants.KBN_RANKING_PRODUCT_BUY_COUNT:
				strStatementName = "GetBuyCountNum";
				break;
			case Constants.KBN_RANKING_PRODUCT_BUY_PRICE:
				strStatementName = "GetBuyPriceNum";
				break;
		}

		// 入力パラメタ設定
		var htInput = new Hashtable();
		htInput.Add(Constants.FIELD_DISPSUMMARYANALYSIS_TGT_YEAR, m_iCurrentYear.ToString());
		htInput.Add(Constants.FIELD_DISPSUMMARYANALYSIS_TGT_MONTH, m_iCurrentMonth.ToString().PadLeft(2, '0'));
		htInput.Add(Constants.FIELD_DISPSUMMARYANALYSIS_DEPT_ID, this.LoginOperatorDeptId);
		htInput.Add("access_kbn", m_strProductAccessKbn);

		htInput.Add(
			(m_strProductType == Constants.KBN_RANKING_PRODUCT_SEARCH_WORD)
				? Constants.FIELD_DISPSUMMARYANALYSIS_VALUE_NAME
				: Constants.FIELD_DISPSUMMARYANALYSIS_RESERVED7,
			m_strProductValue);

		// SQL発行
		using (var sqlAccessor = new SqlAccessor())
		using (var sqlStatement = new SqlStatement("ProductSearchAnalysis", strStatementName))
		{
			dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		return dvResult;
	}
}