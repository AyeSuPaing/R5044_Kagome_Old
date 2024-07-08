/*
=========================================================================================================
  Module      : アクセスランキングレポート一覧ページ処理(AccessRankingList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Calendar = System.Web.UI.WebControls.Calendar;

public partial class Form_AccessRanking_AccessRankingList : BasePage
{
	public const string REQUEST_KEY_PCMOBILE = "pcm";
	
	public const string KBN_PCMOBILE_PC = "pc";
	public const string KBN_PCMOBILE_MOBILE = "mobile";
	public const string KBN_PCMOBILE_SMART_PHONE = "smart_phone";
	
	public const string KBN_RANKING_ACCESSPAGE = "1";
	public const string KBN_RANKING_REFERRERDOMAIN = "2";
	public const string KBN_RANKING_SEARCHENGINE = "3";
	public const string KBN_RANKING_SEARCHWORDS = "4";
	public const string KBN_RANKING_MOBILECAREER = "5";
	public const string KBN_RANKING_MOBILENAME = "6";
	public const string KBN_RANKING_ACCESSPAGE_SP = "7";
	public const string KBN_RANKING_REFERRERDOMAIN_SP = "8";
	public const string KBN_RANKING_SEARCHENGINE_SP = "9";
	public const string KBN_RANKING_SEARCHWORDS_SP = "10";

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
			// PC or Mobile
			if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
			{
				rblPCMobile.Items.RemoveAt(2);
			}

			foreach (ListItem li in rblPCMobile.Items)
			{
				li.Selected = (li.Value == ((Constants.CONST_IS_MOBILE_SITE_ONLY) ? KBN_PCMOBILE_MOBILE : KBN_PCMOBILE_PC));
			}

			//------------------------------------------------------
			// ラジオボタンの値初期化
			//------------------------------------------------------
			if (Request[REQUEST_KEY_PCMOBILE] != null)
			{
				try
				{
					foreach (ListItem li in rblPCMobile.Items)
					{
						li.Selected = (li.Value == Request[REQUEST_KEY_PCMOBILE]);
					}
				}
				catch
				{
					// パラメタエラー
				}
			}

			//------------------------------------------------------
			// ランキングタイプ
			//------------------------------------------------------
			InitializeRankingTypeDropDown(rblPCMobile.SelectedValue);

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
	}

	/// <summary>
	/// ランキングタイプドロップダウン作成
	/// </summary>
	/// <param name="strPCMobile"></param>
	private void InitializeRankingTypeDropDown(string strPCMobile)
	{
		rblRankingType.Items.Clear();

		// Get list access ranking
		var listRankingType = ValueText.GetValueItemList(
			Constants.VALUETEXT_PARAM_ACCESS_RANKING,
			string.Format(
				"{0}_{1}",
				Constants.VALUETEXT_PARAM_PRODUCT_RANKING_REPORT_RANKING_TYPE,
				strPCMobile));

		foreach (ListItem item in listRankingType)
		{
			rblRankingType.Items.Add(item);
		}

		switch (strPCMobile)
		{
			case KBN_PCMOBILE_PC:
				foreach (ListItem li in rblRankingType.Items)
				{
					li.Selected = (li.Value == KBN_RANKING_ACCESSPAGE);
				}
				break;

			case KBN_PCMOBILE_MOBILE:
				foreach (ListItem li in rblRankingType.Items)
				{
					li.Selected = (li.Value == KBN_RANKING_MOBILECAREER);
				}
				break;

			case KBN_PCMOBILE_SMART_PHONE:
				foreach (ListItem li in rblRankingType.Items)
				{
					li.Selected = (li.Value == KBN_RANKING_ACCESSPAGE_SP);
				}
				break;
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
		string accessKbn = "";

		switch (rblPCMobile.SelectedValue)
		{
			case KBN_PCMOBILE_PC:
				accessKbn = "1";
				break;

			case KBN_PCMOBILE_MOBILE:
				accessKbn = "2";
				break;

			case KBN_PCMOBILE_SMART_PHONE:
				accessKbn = "3";
				break;
		}

		//------------------------------------------------------
		// ランキング結果取得
		//------------------------------------------------------
		switch (rblRankingType.SelectedValue)
		{
			case KBN_RANKING_ACCESSPAGE:
			case KBN_RANKING_ACCESSPAGE_SP:
				m_rtAccessRankingTable = RankingUtility.GetAccessPageRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd, accessKbn);
				break;

			case KBN_RANKING_REFERRERDOMAIN:
			case KBN_RANKING_REFERRERDOMAIN_SP:
				m_rtAccessRankingTable = RankingUtility.GetReferrerDomainRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd, accessKbn);
				break;

			case KBN_RANKING_SEARCHENGINE:
			case KBN_RANKING_SEARCHENGINE_SP:
				m_rtAccessRankingTable = RankingUtility.GetSearchEngineRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd, accessKbn);
				break;

			case KBN_RANKING_SEARCHWORDS:
			case KBN_RANKING_SEARCHWORDS_SP:
				m_rtAccessRankingTable = RankingUtility.GetSearchWordRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd, accessKbn);
				break;

			case KBN_RANKING_MOBILECAREER:
				m_rtAccessRankingTable = RankingUtility.GetMobileCareerRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd);
				break;

			case KBN_RANKING_MOBILENAME:
				m_rtAccessRankingTable = RankingUtility.GetMobileNameRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd);
				break;
		}

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		rRankingList.DataSource = m_rtAccessRankingTable.Rows;
		rRankingList.DataBind();
		trNoData.Visible = (m_rtAccessRankingTable.TotalRows == 0);
		lValueName.Text = WebSanitizer.HtmlEncode(m_rtAccessRankingTable.ValueName);
		lUnit.Text = WebSanitizer.HtmlEncode(m_rtAccessRankingTable.Unit);

		//------------------------------------------------------
		// ページャ作成
		//------------------------------------------------------
		StringBuilder sbNextUrl = new StringBuilder(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_ACCESS_PAGE_RANKING_LIST);
		sbNextUrl.Append("?").Append(REQUEST_KEY_PCMOBILE).Append("=").Append(rblPCMobile.SelectedValue);
		sbNextUrl.Append("&").Append(Constants.REQUEST_KEY_DISPLAY_KBN).Append("=").Append(rblRankingType.SelectedValue);
		sbNextUrl.Append("&").Append(Constants.REQUEST_KEY_TARGET_YEAR).Append("=").Append(ViewState[KEY_VS_YEAR]);
		sbNextUrl.Append("&").Append(Constants.REQUEST_KEY_TARGET_MONTH).Append("=").Append(ViewState[KEY_VS_MONTH]);
		sbNextUrl.Append("&").Append(Constants.REQUEST_KEY_TARGET_DAY).Append("=").Append(ViewState[KEY_VS_DAY]);

		lbPager1.Text = WebPager.CreateListPager(m_rtAccessRankingTable.TotalRows, m_iPageNumber, sbNextUrl.ToString(), Constants.CONST_DISP_CONTENTS_ACSPAGERANKING_LIST);
	}

	/// <summary>
	/// アクセスランキング情報表示
	/// </summary>
	/// <returns></returns>
	protected string GetAccessRankingInfo()
	{
		var dateString = ((int)ViewState[KEY_VS_DAY] != 0)
			? DateTimeUtility.ToStringForManager(
				new DateTime((int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY]),
				DateTimeUtility.FormatType.LongDate1Letter)
			: DateTimeUtility.ToStringForManager(
				new DateTime((int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], 1),
				DateTimeUtility.FormatType.LongYearMonth);
		var result = string.Format("{0}　{1}", dateString, rblRankingType.Items.FindByValue(rblRankingType.SelectedValue));
		return result;
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
	/// PC or Mobileラジオボタン選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblPCMobile_SelectedIndexChanged(object sender, EventArgs e)
	{
		// ページ番号初期化
		m_iPageNumber = 1;

		string strSelectedValue = null;
		if (rblPCMobile.SelectedValue == KBN_PCMOBILE_PC)
		{
			switch (rblRankingType.SelectedValue)
			{
				case KBN_RANKING_MOBILECAREER:
				case KBN_RANKING_MOBILENAME:
				case KBN_RANKING_ACCESSPAGE_SP:
				case KBN_RANKING_REFERRERDOMAIN_SP:
				case KBN_RANKING_SEARCHENGINE_SP:
				case KBN_RANKING_SEARCHWORDS_SP:
					strSelectedValue = KBN_RANKING_ACCESSPAGE;
					break;

				default:
					strSelectedValue = rblRankingType.SelectedValue;
					break;
			}
		}
		else if (rblPCMobile.SelectedValue == KBN_PCMOBILE_MOBILE)
		{
			switch (rblRankingType.SelectedValue)
			{
				case KBN_RANKING_ACCESSPAGE:
				case KBN_RANKING_REFERRERDOMAIN:
				case KBN_RANKING_SEARCHENGINE:
				case KBN_RANKING_SEARCHWORDS:
				case KBN_RANKING_ACCESSPAGE_SP:
				case KBN_RANKING_REFERRERDOMAIN_SP:
				case KBN_RANKING_SEARCHENGINE_SP:
				case KBN_RANKING_SEARCHWORDS_SP:
					strSelectedValue = KBN_RANKING_MOBILECAREER;
					break;

				default:
					strSelectedValue = rblRankingType.SelectedValue;
					break;
			}
		}
		else if (rblPCMobile.SelectedValue == KBN_PCMOBILE_SMART_PHONE)
		{
			switch (rblRankingType.SelectedValue)
			{
				case KBN_RANKING_ACCESSPAGE:
				case KBN_RANKING_REFERRERDOMAIN:
				case KBN_RANKING_SEARCHENGINE:
				case KBN_RANKING_SEARCHWORDS:
				case KBN_RANKING_MOBILECAREER:
				case KBN_RANKING_MOBILENAME:
					strSelectedValue = KBN_RANKING_ACCESSPAGE_SP;
					break;

				default:
					strSelectedValue = rblRankingType.SelectedValue;
					break;
			}
		}

		InitializeRankingTypeDropDown(rblPCMobile.SelectedValue);
		foreach (ListItem li in rblRankingType.Items)
		{
			li.Selected = (li.Value == strSelectedValue);
		}

		// ランキングデータをページにデータバインド
		BindRankingData();
	}

	/// <summary>
	/// CSVダウンロード用ファイル名作成
	/// </summary>
	/// <returns></returns>
	protected string GetAccessRankingYMD()
	{
		StringBuilder sbResult = new StringBuilder();

		if ((int)ViewState[KEY_VS_YEAR] != 0)
		{
			sbResult.Append(ViewState[KEY_VS_YEAR].ToString());
		}
		if ((int)ViewState[KEY_VS_MONTH] != 0)
		{
			int iMonth = 0;
			if (int.TryParse(ViewState[KEY_VS_MONTH].ToString(), out iMonth))
			{
				sbResult.Append(iMonth.ToString("00"));
			}
		}
		if ((int)ViewState[KEY_VS_DAY] != 0)
		{
			int iDate = 0;
			if (int.TryParse(ViewState[KEY_VS_DAY].ToString(), out iDate))
			{
				sbResult.Append(iDate.ToString("00"));
			}
		}

		return sbResult.ToString();
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
			// 「アクセスランキング　{0}」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_RANKING_LIST,
				Constants.VALUETEXT_PARAM_ACCESS_RANKING_LIST_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_RANKING),
			GetAccessRankingInfo()));
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
				Constants.VALUETEXT_PARAM_ACCESS_RANKING_LIST,
				Constants.VALUETEXT_PARAM_ACCESS_RANKING_LIST_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_RANKING_LIST_RANK));
		lHeaderParams.Add(lValueName.Text);
		lHeaderParams.Add(lUnit.Text);
		lHeaderParams.Add(
			// 「構成比」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_RANKING_LIST,
				Constants.VALUETEXT_PARAM_ACCESS_RANKING_LIST_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_RANKING_LIST_COMPOSITION_RATIO));
		StringBuilder sbFileName = new StringBuilder();
		sbFileName.Append("AccessRankingList_").Append(GetAccessRankingYMD());
		sbRecords.Append(CreateRecordCsv(lHeaderParams));

		//------------------------------------------------------
		// データ作成
		//------------------------------------------------------
		int iBgn = 1;
		int iEnd = int.MaxValue;
		string accessKbn = "";
		switch (rblPCMobile.SelectedValue)
		{
			case KBN_PCMOBILE_PC:
				accessKbn = "1";
				break;

			case KBN_PCMOBILE_MOBILE:
				accessKbn = "2";
				break;

			case KBN_PCMOBILE_SMART_PHONE:
				accessKbn = "3";
				break;
		}
		RankingUtility.RankingTable rtAccessRankingTable = null;
		switch (rblRankingType.SelectedValue)
		{
			case KBN_RANKING_ACCESSPAGE:
			case KBN_RANKING_ACCESSPAGE_SP:
				rtAccessRankingTable = RankingUtility.GetAccessPageRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd, accessKbn);
				break;

			case KBN_RANKING_REFERRERDOMAIN:
			case KBN_RANKING_REFERRERDOMAIN_SP:
				rtAccessRankingTable = RankingUtility.GetReferrerDomainRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd, accessKbn);
				break;

			case KBN_RANKING_SEARCHENGINE:
			case KBN_RANKING_SEARCHENGINE_SP:
				rtAccessRankingTable = RankingUtility.GetSearchEngineRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd, accessKbn);
				break;

			case KBN_RANKING_SEARCHWORDS:
			case KBN_RANKING_SEARCHWORDS_SP:
				rtAccessRankingTable = RankingUtility.GetSearchWordRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd, accessKbn);
				break;

			case KBN_RANKING_MOBILECAREER:
				rtAccessRankingTable = RankingUtility.GetMobileCareerRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd);
				break;

			case KBN_RANKING_MOBILENAME:
				rtAccessRankingTable = RankingUtility.GetMobileNameRankingReport(this.LoginOperatorDeptId, (int)ViewState[KEY_VS_YEAR], (int)ViewState[KEY_VS_MONTH], (int)ViewState[KEY_VS_DAY], iBgn, iEnd);
				break;
		}
		foreach (RankingUtility.RankingRow rr in rtAccessRankingTable.Rows)
		{
			List<string> lDataParams = new List<string>();
			lDataParams.Add(rr.Rank.ToString());
			lDataParams.Add(rr.RowName.ToString());
			lDataParams.Add(rr.Count.ToString());
			lDataParams.Add(KbnAnalysisUtility.GetRateString(rr.Count, rr.TotalCount, 1));
			sbRecords.Append(CreateRecordCsv(lDataParams));
		}

		//------------------------------------------------------
		// ファイル出力
		//------------------------------------------------------
		OutPutFileCsv(sbFileName.ToString(), sbRecords.ToString());
	}
}
