/*
=========================================================================================================
  Module      : 商品ランキングレポート一覧ページ処理(ProductRankingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using Calendar = System.Web.UI.WebControls.Calendar;

public partial class Form_ProductRanking_ProductRankingList : BasePage
{
	protected RankingUtility.RankingTable m_rtAccessRankingTable = null;
	protected int m_iPageNumber = 1;

	private const string KEY_VS_YEAR = "selected_year";
	private const string KEY_VS_MONTH = "selected_month";
	private const string KEY_VS_DAY = "selected_day";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// 指定した言語ロケールIDにより、カルチャーを変換する
		if (Constants.GLOBAL_OPTION_ENABLE && string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false)
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE);
		}

		//------------------------------------------------------
		// パラメータ取得
		//------------------------------------------------------
		try
		{
			m_iPageNumber = int.Parse(Request[Constants.REQUEST_KEY_PAGE_NO]);
		}
		catch
		{
			// エラーの場合は初期値適用
		}

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			Initialize();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void Initialize()
	{
		//------------------------------------------------------
		// ラジオボタンの値初期化
		//------------------------------------------------------
		// 表示区分
		if (Request[Constants.REQUEST_KEY_DISPLAY_KBN] != null)
		{
			try
			{
				foreach (ListItem li in rblRankingType.Items)
				{
					li.Selected = (li.Value == Request[Constants.REQUEST_KEY_DISPLAY_KBN]);
				}
			}
			catch
			{
				// パラメタエラー
			}
		}

		// アクセス区分
		if (Request[Constants.REQUEST_KEY_ACCESS_KBN] != null)
		{
			try
			{
				foreach (ListItem li in rblAccessKbn.Items)
				{
					li.Selected = (li.Value == Request[Constants.REQUEST_KEY_ACCESS_KBN]);
				}
			}
			catch
			{
				// パラメタエラー
			}
		}

		// 消費税
		if (Request[Constants.REQUEST_KEY_TAX_DISPLAY_TYPE] != null)
		{
			foreach (ListItem li in rblTaxType.Items)
			{
				li.Selected = (li.Value == Request[Constants.REQUEST_KEY_TAX_DISPLAY_TYPE]);
			}
		}

		//------------------------------------------------------
		// カレンダの初期値セット
		//------------------------------------------------------
		string strYear = Request[Constants.REQUEST_KEY_TARGET_YEAR];
		string strMonth = Request[Constants.REQUEST_KEY_TARGET_MONTH];
		string strDay = Request[Constants.REQUEST_KEY_TARGET_DAY];

		try
		{
			// パラメタあり？
			if ((strYear != null))
			{
				// 日付選択？
				if (strDay != "0")
				{
					// 日付選択
					SelectDay(int.Parse(strYear), int.Parse(strMonth), int.Parse(strDay));
				}
				// 月選択とみなす
				else
				{
					SelectMonth(int.Parse(strYear), int.Parse(strMonth));
				}
			}
			else
			{
				// パラメタなしの場合は昨日の月全体を選択
				DateTime dtYesterday = DateTime.Now.AddDays(-1);
				SelectMonth(dtYesterday.Year, dtYesterday.Month);
			}
		}
		catch
		{
			// パラメタエラーは昨日の月全体を選択
			DateTime dtYesterday = DateTime.Now.AddDays(-1);
			SelectMonth(dtYesterday.Year, dtYesterday.Month);
		}
	}

	/// <summary>
	/// 月選択
	/// </summary>
	/// <param name="iYear">対象年</param>
	/// <param name="iMonth">対象月</param>
	private void SelectMonth(int iYear, int iMonth)
	{
		// 選択値設定
		ViewState[KEY_VS_YEAR] = iYear;
		ViewState[KEY_VS_MONTH] = iMonth;
		ViewState[KEY_VS_DAY] = 0;

		// 月表示
		lbTgtMonth.CssClass = "calendar_selected_bg";
		lbTgtMonth.Text = DateTimeUtility.ToStringForManager(
			new DateTime(iYear, iMonth, 1),
			DateTimeUtility.FormatType.LongYearMonth);

		// 日表示
		calTarget.SelectedDates.Remove(calTarget.SelectedDate);
		calTarget.VisibleDate = new DateTime(iYear, iMonth, 1);

		// ランキングデータデータバインド
		BindRankingData();
	}

	/// <summary>
	/// 日選択
	/// </summary>
	/// <param name="iYear">対象年</param>
	/// <param name="iMonth">対象月</param>
	/// <param name="iDay">対象日</param>
	private void SelectDay(int iYear, int iMonth, int iDay)
	{
		// 選択値設定
		ViewState[KEY_VS_YEAR] = iYear;
		ViewState[KEY_VS_MONTH] = iMonth;
		ViewState[KEY_VS_DAY] = iDay;

		// 月表示
		lbTgtMonth.CssClass = "calendar_unselected_bg";
		lbTgtMonth.Text = DateTimeUtility.ToStringForManager(
			new DateTime(iYear, iMonth, 1),
			DateTimeUtility.FormatType.LongYearMonth);

		// 日表示
		try
		{
			calTarget.SelectedDate = new DateTime(iYear, iMonth, iDay);
		}
		catch
		{
			calTarget.SelectedDate = new DateTime(iYear, iMonth, 1).AddMonths(1).AddDays(-1);
			ViewState[KEY_VS_DAY] = calTarget.SelectedDate.Day;
		}
		calTarget.VisibleDate = calTarget.SelectedDate;

		// ランキングデータデータバインド
		BindRankingData();
	}

	/// <summary>
	/// 前/次の月へ移動
	/// </summary>
	/// <param name="iYear">選択月の年</param>
	/// <param name="iMonth">選択月の月</param>
	private void MoveMonth(int iYear, int iMonth)
	{
		if ((int)ViewState[KEY_VS_DAY] == 0)
		{
			// 画面設定（月選択）
			SelectMonth(iYear, iMonth);
		}
		else
		{
			// 画面設定（日選択）
			SelectDay(iYear, iMonth, (int)ViewState[KEY_VS_DAY]);
		}
	}

	/// <summary>
	/// ランキングデータをページにデータバインド
	/// </summary>
	private void BindRankingData()
	{
		int iBgn = Constants.CONST_DISP_CONTENTS_ACSPAGERANKING_LIST * (m_iPageNumber - 1) + 1;
		int iEnd = Constants.CONST_DISP_CONTENTS_ACSPAGERANKING_LIST * m_iPageNumber;
		string strAccessKbn = rblAccessKbn.SelectedValue;	// アクセス区分

		// モバイルデータの表示と非表示OFF時はモバイルを表示しないようにする
		if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false) rblAccessKbn.Items[3].Attributes.Add("style", "display:none;");

		//------------------------------------------------------
		// ランキング結果取得
		//------------------------------------------------------
		if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_SEARCH_WORD)
		{
			m_rtAccessRankingTable = RankingUtility.GetProductSearchWordRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd, strAccessKbn);
		}
		else if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_COUNT)
		{
			m_rtAccessRankingTable = RankingUtility.GetProductBuyCountRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd, strAccessKbn);
		}
		else if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_PRICE)
		{
			m_rtAccessRankingTable = RankingUtility.GetProductBuyPriceRankingReport(
				this.LoginOperatorDeptId,
				(int)ViewState[KEY_VS_YEAR],
				(int)ViewState[KEY_VS_MONTH],
				(int)ViewState[KEY_VS_DAY],
				iBgn,
				iEnd,
				strAccessKbn,
				this.AmountFieldName);
		}

		//------------------------------------------------------
		// データ表示
		//------------------------------------------------------
		lbRankingInfo.Text = WebSanitizer.HtmlEncode(string.Format(
			// 「{0}　対象:{1}」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_LIST,
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_LIST_TITLE,
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_LIST_TARGET),
			GetAccessRankingInfo(),
			rblAccessKbn.SelectedItem.Text));
		if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_PRICE)
		{
			lbRankingInfo.Text = string.Format(
				"{0}（{1}）",
				lbRankingInfo.Text,
				this.IsIncludedTax
					? ReplaceTag("@@DispText.tax_type.included@@")
					: ReplaceTag("@@DispText.tax_type.excluded@@"));
		}
		lbValueName.Text = WebSanitizer.HtmlEncode(m_rtAccessRankingTable.ValueName);
		lbUnitName.Text = WebSanitizer.HtmlEncode(m_rtAccessRankingTable.Unit);
		lbExtraName1.Text = lbExtraName2.Text = WebSanitizer.HtmlEncode(m_rtAccessRankingTable.ExtraName);

		trNoData.Visible = (m_rtAccessRankingTable.TotalRows == 0);

		rResult.DataSource = m_rtAccessRankingTable.Rows;
		rResult.DataBind();

		//------------------------------------------------------
		// ページャ作成
		//------------------------------------------------------
		StringBuilder sbNextUrl = new StringBuilder(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_PRODUCT_RANKING_LIST);
		sbNextUrl.Append("?").Append(Constants.REQUEST_KEY_DISPLAY_KBN).Append("=").Append(rblRankingType.SelectedValue);
		sbNextUrl.Append("&").Append(Constants.REQUEST_KEY_ACCESS_KBN).Append("=").Append(rblAccessKbn.SelectedValue);
		sbNextUrl.Append("&").Append(Constants.REQUEST_KEY_TAX_DISPLAY_TYPE).Append("=").Append(rblTaxType.SelectedValue);
		sbNextUrl.Append("&").Append(Constants.REQUEST_KEY_TARGET_YEAR).Append("=").Append(ViewState[KEY_VS_YEAR]);
		sbNextUrl.Append("&").Append(Constants.REQUEST_KEY_TARGET_MONTH).Append("=").Append(ViewState[KEY_VS_MONTH]);
		sbNextUrl.Append("&").Append(Constants.REQUEST_KEY_TARGET_DAY).Append("=").Append(ViewState[KEY_VS_DAY]);

		lbPager1.Text = WebPager.CreateListPager(m_rtAccessRankingTable.TotalRows, m_iPageNumber, sbNextUrl.ToString(), Constants.CONST_DISP_CONTENTS_ACSPAGERANKING_LIST);
	}

	/// <summary>
	/// アクセスランキング情報表示
	/// </summary>
	/// <returns></returns>
	private string GetAccessRankingInfo()
	{
		// 変数宣言
		var result = ((int)ViewState[KEY_VS_DAY] != 0)
			? DateTimeUtility.ToStringForManager(
				new DateTime((int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY]),
				DateTimeUtility.FormatType.LongDate1Letter)
			: DateTimeUtility.ToStringForManager(
				new DateTime((int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], 1),
				DateTimeUtility.FormatType.LongYearMonth);

		if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_SEARCH_WORD)
		{
			result += "：";
			result += ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RANKING_TYPE,
				Constants.KBN_RANKING_PRODUCT_SEARCH_WORD);
		}
		else if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_COUNT)
		{
			result += "：";
			result += ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RANKING_TYPE,
				Constants.KBN_RANKING_PRODUCT_BUY_COUNT);
		}
		else if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_PRICE)
		{
			result += "：";
			result += ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT,
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RANKING_TYPE,
				Constants.KBN_RANKING_PRODUCT_BUY_PRICE);
		}

		return result;
	}

	/// <summary>
	/// データバインド用商品ランキングレポートURL用パラメタ作成
	/// </summary>
	/// <param name="strKbn">検索区分</param>
	/// <param name="strValue">検索ワード OR 商品バリエーションID</param>
	/// <returns>商品ランキングレポートURL用パラメタ</returns>
	protected string CreateProductRankingReportUrlParam(string strKbn, string strValue)
	{
		System.Text.StringBuilder sbResult = new System.Text.StringBuilder();

		sbResult.Append("'").Append(((int)ViewState[KEY_VS_YEAR]).ToString()).Append("'");
		sbResult.Append(",");
		sbResult.Append("'").Append(((int)ViewState[KEY_VS_MONTH]).ToString()).Append("'");
		sbResult.Append(",");
		sbResult.Append("'").Append(strKbn).Append("'");
		sbResult.Append(",");
		sbResult.Append("'").Append(strValue.Replace("'", "\\'").Replace("\t", "\\t")).Append("'");
		sbResult.Append(",");
		sbResult.Append(rblAccessKbn.SelectedValue);
		sbResult.Append(",");
		sbResult.Append("'").Append(rblTaxType.SelectedValue).Append("'");
		//todo:okada

		return sbResult.ToString();
	}

	/// <summary>
	/// ランキングタイプ変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblRankingType_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		// ページ番号初期化
		m_iPageNumber = 1;

		// ランキングデータをページにデータバインド
		BindRankingData();
	}

	/// <summary>
	/// 消費税の変更 ランキングデータをページにデータバインド
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblTaxType_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		BindRankingData();
	}

	/// <summary>
	/// カレンダー日付選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void calTarget_SelectionChanged(object sender, System.EventArgs e)
	{
		// ページ番号初期化
		m_iPageNumber = 1;

		// 選択日取得
		DateTime dtSelected = ((Calendar)sender).SelectedDate;

		// 画面設定（日選択）
		SelectDay(dtSelected.Year, dtSelected.Month, dtSelected.Day);
	}

	/// <summary>
	/// カレンダー月選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbTgtMonth_Click(object sender, System.EventArgs e)
	{
		// ページ番号初期化
		m_iPageNumber = 1;

		// 画面設定（月選択）
		SelectMonth((int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH]);
	}

	/// <summary>
	/// 前の月を選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbMonthBefore_Click(object sender, System.EventArgs e)
	{
		// ページ番号初期化
		m_iPageNumber = 1;

		// 前の月へ
		DateTime dtBeforeMonth = new DateTime((int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], 1).AddMonths(-1);
		MoveMonth(dtBeforeMonth.Year, dtBeforeMonth.Month);
	}

	/// <summary>
	/// 次の月を選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbMonthNext_Click(object sender, System.EventArgs e)
	{
		// ページ番号初期化
		m_iPageNumber = 1;

		// 次の月へ
		DateTime dtNextMonth = new DateTime((int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], 1).AddMonths(1);
		MoveMonth(dtNextMonth.Year, dtNextMonth.Month);
	}

	/// <summary>
	///　カレンダー日付描画毎に呼び出されるイベント 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void calTarget_DayRender(object sender, System.Web.UI.WebControls.DayRenderEventArgs e)
	{
		// 前月・来月の日はリンク削除
		if (((Calendar)sender).VisibleDate.Month != e.Day.Date.Month)
		{
			e.Cell.Controls.Clear();
		}
	}

	/// <summary>
	/// アクセスランキング情報表示
	/// </summary>
	/// <returns></returns>
	private string GetAccessRankingDate()
	{
		// 変数宣言
		string strResult = String.Empty;

		strResult += (int)ViewState[KEY_VS_YEAR] != 0 ? ViewState[KEY_VS_YEAR].ToString() : "";
		strResult += (int)ViewState[KEY_VS_MONTH] != 0 ? ((int)ViewState[KEY_VS_MONTH]).ToString("00") : "";
		strResult += (int)ViewState[KEY_VS_DAY] != 0 ? ((int)ViewState[KEY_VS_DAY]).ToString("00") : "";

		return strResult;
	}


	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, System.EventArgs e)
	{
		StringBuilder sbRecords = new StringBuilder();

		//------------------------------------------------------
		// タイトル作成
		//------------------------------------------------------
		List<string> lTitleParams = new List<string>();
		lTitleParams.Add(string.Format(
			// 「商品ランキング {0}」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_LIST,
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_LIST_TITLE,
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_LIST_PRODUCT_RANKING),
			lbRankingInfo.Text));
		sbRecords.Append(CreateRecordCsv(lTitleParams));
		lTitleParams.Clear();
		sbRecords.Append(CreateRecordCsv(lTitleParams));

		//------------------------------------------------------
		// ヘッダ作成
		//------------------------------------------------------
		List<string> lHeaderParams = new List<string>();
		lHeaderParams.Add(
			// 「ランク」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_LIST,
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_LIST_TITLE,
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_LIST_RANK));
		if (rblRankingType.SelectedValue != Constants.KBN_RANKING_PRODUCT_SEARCH_WORD)
		{
			lHeaderParams.Add(lbExtraName1.Text);
		}
		lHeaderParams.Add(lbValueName.Text);
		if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_SEARCH_WORD)
		{
			lHeaderParams.Add(lbExtraName2.Text);
		}
		lHeaderParams.Add(lbUnitName.Text);
		lHeaderParams.Add(
			// 「構成比」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_LIST,
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_LIST_TITLE,
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_LIST_COMPOSITION_RATIO));
		StringBuilder sbFileName = new StringBuilder();
		sbFileName.Append("ProductRankingList_").Append(GetAccessRankingDate());
		sbRecords.Append(CreateRecordCsv(lHeaderParams));

		//------------------------------------------------------
		// データ作成
		//------------------------------------------------------
		int iBgn = 1;
		int iEnd = int.MaxValue;
		RankingUtility.RankingTable rtAccessRankingTable = null;
		if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_SEARCH_WORD)
		{
			rtAccessRankingTable = RankingUtility.GetProductSearchWordRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd, rblAccessKbn.SelectedValue);
		}
		else if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_COUNT)
		{
			rtAccessRankingTable = RankingUtility.GetProductBuyCountRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd, rblAccessKbn.SelectedValue);
		}
		else if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_PRICE)
		{
			rtAccessRankingTable = RankingUtility.GetProductBuyPriceRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd, rblAccessKbn.SelectedValue, this.AmountFieldName);
		}

		foreach (RankingUtility.RankingRow rr in rtAccessRankingTable.Rows)
		{
			List<string> lDataParams = new List<string>();
			lDataParams.Add(rr.Rank.ToString());
			if (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_SEARCH_WORD)
			{
				lDataParams.Add(rr.RowName);
				lDataParams.Add(rr.Extra.ToString());
			}
			else
			{
				lDataParams.Add(rr.Extra.ToString());
				lDataParams.Add(rr.RowName);
			}
			lDataParams.Add((rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_PRICE) ? rr.Price.ToPriceString() : rr.Count.ToString());
			lDataParams.Add(KbnAnalysisUtility.GetRateString((rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_PRICE) ? rr.Price : rr.Count, (rblRankingType.SelectedValue == Constants.KBN_RANKING_PRODUCT_BUY_PRICE) ? rr.TotalPrice : rr.TotalCount, 1));
			sbRecords.Append(CreateRecordCsv(lDataParams));
		}

		//------------------------------------------------------
		// ファイル出力
		//------------------------------------------------------
		OutPutFileCsv(sbFileName.ToString(), sbRecords.ToString());
	}

	/// <summary>税込表示か</summary>
	protected bool IsIncludedTax
	{
		get
		{
			return (rblTaxType.SelectedValue == Constants.KBN_ORDERCONDITIONREPORT_TAX_INCLUDED);
		}
	}
	/// <summary>金額フィールド名</summary>
	protected string AmountFieldName
	{
		get
		{
			if (this.IsIncludedTax)
			{
				return (Constants.MANAGEMENT_INCLUDED_TAX_FLAG
					? Constants.FIELD_DISPSUMMARYANALYSIS_PRICE
					: (Constants.FIELD_DISPSUMMARYANALYSIS_PRICE + "+" + Constants.FIELD_DISPSUMMARYANALYSIS_PRICE_TAX));
			}
			return (Constants.MANAGEMENT_INCLUDED_TAX_FLAG
				? (Constants.FIELD_DISPSUMMARYANALYSIS_PRICE + "-" + Constants.FIELD_DISPSUMMARYANALYSIS_PRICE_TAX)
				: Constants.FIELD_DISPSUMMARYANALYSIS_PRICE);
		}
	}
}
