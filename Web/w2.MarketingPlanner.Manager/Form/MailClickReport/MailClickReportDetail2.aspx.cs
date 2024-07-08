/*
=========================================================================================================
  Module      : メールクリックレポート詳細ページ処理(MailClickReportDetail2.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

public partial class Form_MailClickReport_MailClickReportDetail2 : BasePage
{
	string m_strMailTextId = null;
	string m_strMailDistId = null;
	string m_strActionNo = null;
	string m_strMailClickId = null;

	protected int m_iCurrentYear = DateTime.Now.Year;
	protected int m_iCurrentMonth = DateTime.Now.Month;

	const string REQUEST_KEY_DATE_TYPE = "dtype";
	protected const string KBN_DISP_MONTHLY_REPORT = "1";
	protected const string KBN_DISP_DAILY_REPORT = "0";

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
			m_strMailTextId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MAILTEXT_ID]);
			m_strMailDistId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MAILDIST_ID]);
			m_strActionNo = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ACTION_NO]);
			m_strMailClickId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_MAILCLICK_ID]);

			ViewState[Constants.REQUEST_KEY_MAILTEXT_ID] = m_strMailTextId;
			ViewState[Constants.REQUEST_KEY_MAILDIST_ID] = m_strMailDistId;
			ViewState[Constants.REQUEST_KEY_ACTION_NO] = m_strActionNo;
			ViewState[Constants.REQUEST_KEY_MAILCLICK_ID] = m_strMailClickId;

			// 年月取得
			if (int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_YEAR], out m_iCurrentYear) == false)
			{
				m_iCurrentYear = DateTime.Now.Year;
			}
			if (int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_MONTH], out m_iCurrentMonth) == false)
			{
				m_iCurrentMonth = DateTime.Now.Month;
			}
			ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] = m_iCurrentYear;
			ViewState[Constants.REQUEST_KEY_CURRENT_MONTH] = m_iCurrentMonth;

			// レポート種別
			string strReportType = StringUtility.ToEmpty(Request[REQUEST_KEY_DATE_TYPE]);
			strReportType = (strReportType.Length == 0) ? KBN_DISP_DAILY_REPORT : strReportType;
			foreach (ListItem li in rblReportType.Items)
			{
				li.Selected = (li.Value == strReportType);
			}

			//------------------------------------------------------
			// 集計データ表示
			//------------------------------------------------------
			DisplayReport();
		}
		else
		{
			m_strMailTextId = (string)ViewState[Constants.REQUEST_KEY_MAILTEXT_ID];
			m_strMailDistId = (string)ViewState[Constants.REQUEST_KEY_MAILDIST_ID];
			m_strActionNo = (string)ViewState[Constants.REQUEST_KEY_ACTION_NO];
			m_strMailClickId = (string)ViewState[Constants.REQUEST_KEY_MAILCLICK_ID];

			m_iCurrentYear = (int)ViewState[Constants.REQUEST_KEY_CURRENT_YEAR];
			m_iCurrentMonth = (int)ViewState[Constants.REQUEST_KEY_CURRENT_MONTH];
		}
	}

	/// <summary>
	/// レポート表示
	/// </summary>
	private void DisplayReport()
	{
		//------------------------------------------------------
		// カレンダ設定
		//------------------------------------------------------
		StringBuilder sbParam = new StringBuilder();
		sbParam.Append(Constants.REQUEST_KEY_MAILTEXT_ID).Append("=").Append(HttpUtility.UrlEncode(m_strMailTextId));
		sbParam.Append("&").Append(Constants.REQUEST_KEY_MAILDIST_ID).Append("=").Append(HttpUtility.UrlEncode(m_strMailDistId));
		sbParam.Append("&").Append(Constants.REQUEST_KEY_ACTION_NO).Append("=").Append(HttpUtility.UrlEncode(m_strActionNo));
		sbParam.Append("&").Append(Constants.REQUEST_KEY_MAILCLICK_ID).Append("=").Append(HttpUtility.UrlEncode(m_strMailClickId));
		sbParam.Append("&").Append(REQUEST_KEY_DATE_TYPE).Append("=").Append(HttpUtility.UrlEncode(rblReportType.SelectedValue));
		lblCurrentCalendar.Text = CalendarUtility.CreateHtmlYMCalendar(m_iCurrentYear, m_iCurrentMonth, Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILCLICKREPORT_DETAIL2, sbParam.ToString(), Constants.REQUEST_KEY_CURRENT_YEAR, Constants.REQUEST_KEY_CURRENT_MONTH);

		//------------------------------------------------------
		// 表示情報設定
		//------------------------------------------------------
		if (rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT)
		{
			var formatInfoText = string.Format(
				"{0}　{1}",
				// 「{0} 分」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2,
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_REPORT_KBN,
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_REPORT_KBN_MINUTES),
				// 「日別レポート」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2,
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_REPORT_KBN,
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_REPORT_KBN_DAILY_REPORT));
			tdReportInfo.InnerText = string.Format(
				formatInfoText,
				DateTimeUtility.ToStringForManager(
					new DateTime(m_iCurrentYear, m_iCurrentMonth, 1),
					DateTimeUtility.FormatType.LongYearMonth));
		}
		else if (rblReportType.SelectedValue == KBN_DISP_MONTHLY_REPORT)
		{

			var formatInfoText = string.Format(
				"{0}　{1}",
				// 「{0} 年分」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2,
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_REPORT_KBN,
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_REPORT_KBN_YEAR),
				// 「月別レポート」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2,
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_REPORT_KBN,
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_REPORT_KBN_YEARLY_MONTHLY_REPORT));
			tdReportInfo.InnerText = string.Format(formatInfoText, m_iCurrentYear);
		}

		//------------------------------------------------------
		// 集計データ取得
		//------------------------------------------------------
		DataView dvDetail = null;
		string strStatementName = (rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT) ? "GetMailClickTransitionDay" : "GetMailClickTransitionMonth";
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MailClickReport", strStatementName))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILCLICKLOG_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add(Constants.FIELD_MAILCLICKLOG_MAILTEXT_ID, m_strMailTextId);
			htInput.Add(Constants.FIELD_MAILCLICKLOG_MAILDIST_ID, m_strMailDistId);
			htInput.Add(Constants.FIELD_MAILCLICKLOG_ACTION_NO, m_strActionNo);
			htInput.Add(Constants.FIELD_MAILCLICKLOG_MAILCLICK_ID, m_strMailClickId);
			htInput.Add("tgt_year", m_iCurrentYear);
			htInput.Add("tgt_month", m_iCurrentMonth);

			dvDetail = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		//------------------------------------------------------
		// 月の最終日を取得
		//------------------------------------------------------
		int iLastIndex = 0;
		if (rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT)	// 日毎？
		{
			DateTime dtTemp = new DateTime(m_iCurrentYear, m_iCurrentMonth, 1);
			iLastIndex = dtTemp.AddMonths(1).AddDays(-1).Day;
		}
		else // 月毎？
		{
			iLastIndex = 12;
		}

		int iDataCount = 0;
		List<MailClickReportLine> lMailClickReportLines = new List<MailClickReportLine>();
		//------------------------------------------------------
		// データ格納
		//------------------------------------------------------
		for (int iLoop = 1; iLoop <= iLastIndex; iLoop++)
		{
			var formatTitle = string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
				? ((rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT)
					// 「{0}日」
					? ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2,
						Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_TIME_KBN,
						Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_TIME_KBN_DAY)
					// 「{0}月」
					: ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2,
						Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_TIME_KBN,
						Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_TIME_KBN_MONTH))
				: "{0}";
			var strTitle = string.Format(formatTitle, iLoop);

			// データ取得可否判定
			bool blGetData = false;
			if (iDataCount < dvDetail.Count)
			{
				if (((rblReportType.SelectedValue == KBN_DISP_MONTHLY_REPORT) && (iLoop == (int)dvDetail[iDataCount]["tgt_month"]))
					|| ((rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT) && (iLoop == (int)dvDetail[iDataCount]["tgt_day"])))
				{
					blGetData = true;
				}
			}

			if (blGetData == true)
			{
				lMailClickReportLines.Add(new MailClickReportLine(strTitle, (int)dvDetail[iDataCount]["click_counts"], (int)dvDetail[iDataCount]["click_users"]));
				iDataCount++;
			}
			else
			{
				lMailClickReportLines.Add(new MailClickReportLine(strTitle, 0, 0));
			}
		}

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		rDataList.DataSource = lMailClickReportLines;
		rDataList.DataBind();
	}

	/// <summary>
	/// レポートタイプ切替変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblReportType_SelectedIndexChanged(object sender, EventArgs e)
	{
		DisplayReport();
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		StringBuilder sbUrl= new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MAILCLICKREPORT_DETAIL);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_MAILTEXT_ID).Append("=").Append(HttpUtility.UrlEncode(m_strMailTextId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_MAILDIST_ID).Append("=").Append(HttpUtility.UrlEncode(m_strMailDistId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_NO).Append("=").Append(HttpUtility.UrlEncode(m_strActionNo));

		Response.Redirect(sbUrl.ToString());
	}

	///*********************************************************************************************
	/// <summary>
	/// メールクリックレポートクラス
	/// </summary>
	///*********************************************************************************************
	public class MailClickReportLine
	{
		string m_strTitle = null;
		int m_iClickCounts = 0;
		int m_iClickUsers = 0;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strTitle"></param>
		/// <param name="iiClickCounts"></param>
		/// <param name="iClickUsers"></param>
		public MailClickReportLine(string strTitle, int iiClickCounts, int iClickUsers)
		{
			m_strTitle = strTitle;
			m_iClickCounts = iiClickCounts;
			m_iClickUsers = iClickUsers;
		}

		/// <summary>プロパティ：タイトル</summary>
		public string Title
		{
			get { return m_strTitle; }
		}

		/// <summary>プロパティ：クリック数</summary>
		public int ClickCounts
		{
			get { return m_iClickCounts; }
		}

		/// <summary>プロパティ：クリックユーザ数</summary>
		public int ClickUsers
		{
			get { return m_iClickUsers; }
		}
	}
}
