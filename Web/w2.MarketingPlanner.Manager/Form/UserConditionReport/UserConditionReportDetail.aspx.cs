/*
=========================================================================================================
  Module      : 顧客状況詳細レポートページ処理(UserConditionReportDetail.aspx.cs)
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
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Form_UserConditionReport_UserConditionReportDetail : BasePage
{
	protected ArrayList m_alDispData = new ArrayList();

	protected int m_iCurrentYear = DateTime.Now.Year;
	protected int m_iCurrentMonth = DateTime.Now.Month;

	const int FLG_POTENTIAL_NEW_CHECKED				= 0x0001;
	const int FLG_POTENTIAL_ALL_CHECKED				= 0x0002;
	const int FLG_POTENTIAL_ACTIVE_CHECKED			= 0x0004;
	const int FLG_POTENTIAL_UNACTIVE_CHECKED		= 0x0008;
	const int FLG_RECOGNIZE_NEW_CHECKED				= 0x0020;
	const int FLG_RECOGNIZE_ALL_CHECKED				= 0x0040;
	const int FLG_RECOGNIZE_ACTIVE_CHECKED			= 0x0080;
	const int FLG_RECOGNIZE_UNACTIVE_CHECKED		= 0x0100;
	const int FLG_RECOGNIZE_LEAVE_CHECKED			= 0x0200;

	const string REQUEST_KEY_COLUMN_CHECKED			= "chk";
	const string REQUEST_KEY_DATE_TYPE				= "dtype";

	protected const string KBN_DISP_MONTHLY_REPORT	= "1";
	protected const string KBN_DISP_DAILY_REPORT	= "0";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		// 年月取得
		try
		{
			if (ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] != null)
			{
				m_iCurrentYear = (int)ViewState[Constants.REQUEST_KEY_CURRENT_YEAR];
				m_iCurrentMonth = (int)ViewState[Constants.REQUEST_KEY_CURRENT_MONTH];
			}
			else
			{
				m_iCurrentYear = int.Parse(Request[Constants.REQUEST_KEY_CURRENT_YEAR]);
				m_iCurrentMonth = int.Parse(Request[Constants.REQUEST_KEY_CURRENT_MONTH]);
			}
		}
		catch
		{
			m_iCurrentYear = DateTime.Now.AddDays(-1).Year;
			m_iCurrentMonth = DateTime.Now.AddDays(-1).Month;
		}
		// 年月はビューステートへ格納
		ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] = m_iCurrentYear;
		ViewState[Constants.REQUEST_KEY_CURRENT_MONTH] = m_iCurrentMonth;
		
		if (!IsPostBack)
		{
			try
			{
				// チェックボックス選択状態取得・設定
				if (Request[REQUEST_KEY_COLUMN_CHECKED] != null)
				{
					int iChecked = int.Parse(Request[REQUEST_KEY_COLUMN_CHECKED]);
					cblPotential.Items[0].Selected = ((iChecked & FLG_POTENTIAL_NEW_CHECKED) != 0);
					cblPotential.Items[1].Selected = ((iChecked & FLG_POTENTIAL_ALL_CHECKED) != 0);
					cblPotential.Items[2].Selected = ((iChecked & FLG_POTENTIAL_ACTIVE_CHECKED) != 0);
					cblPotential.Items[3].Selected = ((iChecked & FLG_POTENTIAL_UNACTIVE_CHECKED) != 0);
					//cbPotentialIncdec.Checked = ((iChecked & FLG_POTENTIAL_INCDEC_CHECKED) != 0);
					cblRecognize.Items[0].Selected = ((iChecked & FLG_RECOGNIZE_NEW_CHECKED) != 0);
					cblRecognize.Items[1].Selected = ((iChecked & FLG_RECOGNIZE_ALL_CHECKED) != 0);
					cblRecognize.Items[2].Selected = ((iChecked & FLG_RECOGNIZE_ACTIVE_CHECKED) != 0);
					cblRecognize.Items[3].Selected = ((iChecked & FLG_RECOGNIZE_UNACTIVE_CHECKED) != 0);
					cblRecognize.Items[4].Selected = ((iChecked & FLG_RECOGNIZE_LEAVE_CHECKED) != 0);
					//cbRecognizeIncdec.Checked = ((iChecked & FLG_RECOGNIZE_INCDEC_CHECKED) != 0);
				}
			}
			catch
			{
			}

			// ラジオボタン選択状態取得・設定
			try
			{
				int iChecked = int.Parse(Request[REQUEST_KEY_DATE_TYPE]);
				foreach (ListItem li in rblReportType.Items)
				{
					li.Selected = (li.Value == iChecked.ToString());
				}
			}
			catch
			{
			}
		}

		//------------------------------------------------------
		// コンポーネント初期化
		//------------------------------------------------------
		Initialize();

		//------------------------------------------------------
		// 集計データ取得
		//------------------------------------------------------
		GetData();

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		DataBind();
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void Initialize()
	{
		//------------------------------------------------------
		// カレンダ設定
		//------------------------------------------------------
		// 基準カレンダパラメタ設定
		int iCalParam = 0;
		iCalParam += (cblPotential.Items[0].Selected)		? FLG_POTENTIAL_NEW_CHECKED: 0;
		iCalParam += (cblPotential.Items[1].Selected)		? FLG_POTENTIAL_ALL_CHECKED: 0;
		iCalParam += (cblPotential.Items[2].Selected)	? FLG_POTENTIAL_ACTIVE_CHECKED: 0;
		iCalParam += (cblPotential.Items[3].Selected)	? FLG_POTENTIAL_UNACTIVE_CHECKED: 0;
		//iCalParam += (cbPotentialIncdec.Checked)	? FLG_POTENTIAL_INCDEC_CHECKED: 0;
		iCalParam += (cblRecognize.Items[0].Selected)		? FLG_RECOGNIZE_NEW_CHECKED: 0;
		iCalParam += (cblRecognize.Items[1].Selected)		? FLG_RECOGNIZE_ALL_CHECKED: 0;
		iCalParam += (cblRecognize.Items[2].Selected)	? FLG_RECOGNIZE_ACTIVE_CHECKED: 0;
		iCalParam += (cblRecognize.Items[3].Selected)	? FLG_RECOGNIZE_UNACTIVE_CHECKED: 0;
		iCalParam += (cblRecognize.Items[4].Selected)		? FLG_RECOGNIZE_LEAVE_CHECKED: 0;
		//iCalParam += (cbRecognizeIncdec.Checked)	? FLG_RECOGNIZE_INCDEC_CHECKED: 0;
		string strParamForCurrent = REQUEST_KEY_COLUMN_CHECKED + "=" + iCalParam.ToString() + "&" + REQUEST_KEY_DATE_TYPE + "=" + rblReportType.SelectedValue;
		// 基準カレンダ設定
		lblCurrentCalendar.Text = CalendarUtility.CreateHtmlYMCalendar(m_iCurrentYear, m_iCurrentMonth, Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USER_CONDITION_REPORT_DETAIL, strParamForCurrent, Constants.REQUEST_KEY_CURRENT_YEAR, Constants.REQUEST_KEY_CURRENT_MONTH);

		//------------------------------------------------------
		// 表タイトル表示設定
		//------------------------------------------------------
		// 潜在・新規獲得
		tdPotentialNewTitle.Visible = cblPotential.Items[0].Selected;
		tdPotentialNewTitle1.Visible = cblPotential.Items[0].Selected;
		tdPotentialNewTitle2.Visible = cblPotential.Items[0].Selected;
		// 潜在・全体
		tdPotentialAllTitle.Visible = cblPotential.Items[1].Selected;
		tdPotentialAllTitle1.Visible = cblPotential.Items[1].Selected;
		tdPotentialAllTitle2.Visible = cblPotential.Items[1].Selected;
		// 潜在・アクティブ
		tdPotentialActiveTitle.Visible = cblPotential.Items[2].Selected;
		tdPotentialActiveTitle1.Visible = cblPotential.Items[2].Selected;
		tdPotentialActiveTitle2.Visible = cblPotential.Items[2].Selected;
		// 潜在・休眠
		tdPotentialUnActiveTitle.Visible = cblPotential.Items[3].Selected;
		tdPotentialUnActiveTitle1.Visible = cblPotential.Items[3].Selected;
		tdPotentialUnActiveTitle2.Visible = cblPotential.Items[3].Selected;
		// 潜在・増減
		//tdPotentialIncdecTitle.Visible = cbPotentialIncdec.Checked;
		// 認知・新規獲得
		tdRecognizeNewTitle.Visible = cblRecognize.Items[0].Selected;
		tdRecognizeNewTitle1.Visible = cblRecognize.Items[0].Selected;
		tdRecognizeNewTitle2.Visible = cblRecognize.Items[0].Selected;
		// 認知・全体
		tdRecognizeAllTitle.Visible = cblRecognize.Items[1].Selected;
		tdRecognizeAllTitle1.Visible = cblRecognize.Items[1].Selected;
		tdRecognizeAllTitle2.Visible = cblRecognize.Items[1].Selected;
		// 認知・アクティブ
		tdRecognizeActiveTitle.Visible = cblRecognize.Items[2].Selected;
		tdRecognizeActiveTitle1.Visible = cblRecognize.Items[2].Selected;
		tdRecognizeActiveTitle2.Visible = cblRecognize.Items[2].Selected;
		// 認知・休眠
		tdRecognizeUnactiveTitle.Visible = cblRecognize.Items[3].Selected;
		tdRecognizeUnactiveTitle1.Visible = cblRecognize.Items[3].Selected;
		tdRecognizeUnactiveTitle2.Visible = cblRecognize.Items[3].Selected;
		// 認知・退会
		tdRecognizeLeaveTitle.Visible = cblRecognize.Items[4].Selected;
		tdRecognizeLeaveTitle1.Visible = cblRecognize.Items[4].Selected;
		tdRecognizeLeaveTitle2.Visible = cblRecognize.Items[4].Selected;
		// 認知・増減
		//tdRecognizeIncdecTitle.Visible = cbRecognizeIncdec.Checked;

		//------------------------------------------------------
		// 選択日付表示設定
		//------------------------------------------------------
		if (rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT)
		{
			tdReportInfo.InnerText = string.Format(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT,
					Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DISPLAY_KBN,
					KBN_DISP_DAILY_REPORT),
				DateTimeUtility.ToStringForManager(
					new DateTime(m_iCurrentYear, m_iCurrentMonth, 1),
					DateTimeUtility.FormatType.LongYearMonth));
		}
		else if (rblReportType.SelectedValue == KBN_DISP_MONTHLY_REPORT)
		{
			tdReportInfo.InnerText = string.Format(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT,
					Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DISPLAY_KBN,
					KBN_DISP_MONTHLY_REPORT),
				m_iCurrentYear.ToString()); 
		}
	}

	/// <summary>
	/// データ取得・加工
	/// </summary>
	private void GetData()
	{
		DataView dvDetail = null;

		Hashtable htInput = new Hashtable();
		htInput.Add(Constants.FIELD_DISPUSERANALYSIS_TGT_YEAR, m_iCurrentYear.ToString());
		htInput.Add(Constants.FIELD_DISPUSERANALYSIS_TGT_MONTH, m_iCurrentMonth.ToString());
		htInput.Add(Constants.FIELD_DISPUSERANALYSIS_DEPT_ID, this.LoginOperatorDeptId);

		//------------------------------------------------------
		// データ取得
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			string strStatementName = null;
			// 日毎？
			if (rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT)
			{
				strStatementName = "GetUserAnalysisDetailDay";
			}
			else
			{
				strStatementName = "GetUserAnalysisDetailMonth";
			}

			using (SqlStatement sqlStatement = new SqlStatement("UserConditionAnalysis", strStatementName))
			{
				dvDetail = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}
		}

		// データ数取得
		int iDataLength = dvDetail.Count;

		//------------------------------------------------------
		// 月の最終日を取得
		//------------------------------------------------------
		int iLastIndex = 0;
		// 日毎？
		if (rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT)
		{
			DateTime dtTemp = new DateTime(m_iCurrentYear, m_iCurrentMonth, 1);
			iLastIndex = dtTemp.AddMonths(1).AddDays(-1).Day;
		}
			// 月毎？
		else
		{
			iLastIndex = 12;
		}

		int iDataCount = 0;
		ArrayList alLineBefore = null;
		//------------------------------------------------------
		// データ格納
		//------------------------------------------------------
		for (int iLoop = 1; iLoop <= iLastIndex; iLoop++)
		{
			// データ取得可否判定
			bool blGetData = false;
			if (iDataCount < iDataLength)
			{
				int iMonth = int.Parse(dvDetail[iDataCount]["tgt_month"].ToString());	//★
				int iDay = int.Parse(dvDetail[iDataCount]["tgt_day"].ToString());	//★

				// 該当日付データ？
				if (((iLoop == iDay) && (rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT))
					|| ((iLoop == iMonth) && (rblReportType.SelectedValue != KBN_DISP_DAILY_REPORT)))
				{
					blGetData = true;
				}
			}

			Hashtable htData = new Hashtable();
			ArrayList alLine = new ArrayList();

			// 日付
			if (rblReportType.SelectedValue == KBN_DISP_DAILY_REPORT)
			{
				// 日毎？
				htData[Constants.FIELD_DISPUSERANALYSIS_TGT_DAY] = string.Format("{0}{1}",
					iLoop,
					(string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE))
						? ReplaceTag("@@DispText.schedule_unit.Day@@")
						: string.Empty);
			}
			else
			{
				// 月毎？
				htData[Constants.FIELD_DISPUSERANALYSIS_TGT_DAY] = string.Format("{0}{1}",
					iLoop,
					(string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE))
						? ReplaceTag("@@DispText.schedule_unit.Month@@")
						: string.Empty);
			}

			// 潜在・新規獲得
			if (cblPotential.Items[0].Selected == true)
			{
				alLine.Add(CreateDispData(dvDetail, iDataCount, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_NEW, blGetData, alLineBefore, alLine.Count));
			}
			// 潜在・全体
			if (cblPotential.Items[1].Selected == true)
			{
				alLine.Add(CreateDispData(dvDetail, iDataCount, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL, blGetData, alLineBefore, alLine.Count));
			}
			// 潜在・アクティブ
			if (cblPotential.Items[2].Selected == true)
			{
				alLine.Add(CreateDispData(dvDetail, iDataCount, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE, blGetData, alLineBefore, alLine.Count));
			}
			// 潜在・休眠
			if (cblPotential.Items[3].Selected == true)
			{
				alLine.Add(CreateDispData(dvDetail, iDataCount, "potential_unactive_total", blGetData, alLineBefore, alLine.Count));
			}
			// 認知・新規獲得
			if (cblRecognize.Items[0].Selected == true)
			{
				alLine.Add(CreateDispData(dvDetail, iDataCount, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_NEW, blGetData, alLineBefore, alLine.Count));
			}
			// 認知・全体
			if (cblRecognize.Items[1].Selected == true)
			{
				alLine.Add(CreateDispData(dvDetail, iDataCount, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL, blGetData, alLineBefore, alLine.Count));
			}
			// 認知・アクティブ
			if (cblRecognize.Items[2].Selected == true)
			{
				alLine.Add(CreateDispData(dvDetail, iDataCount, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE, blGetData, alLineBefore, alLine.Count));
			}
			// 認知・休眠
			if (cblRecognize.Items[3].Selected == true)
			{
				alLine.Add(CreateDispData(dvDetail, iDataCount, "recognize_unactive_total", blGetData, alLineBefore, alLine.Count));
			}
			// 退会・全体
			if (cblRecognize.Items[4].Selected == true)
			{
				alLine.Add(CreateDispData(dvDetail, iDataCount, Constants.FIELD_DISPUSERANALYSIS_LEAVE_ALL, blGetData, alLineBefore, alLine.Count));
			}

			htData.Add("data", alLine);

			// 追加
			m_alDispData.Add(htData);

			if (blGetData == true)
			{
				iDataCount++;
			}

			// 前列比較用に待避
			alLineBefore = alLine;
		}
	}

	/// <summary>
	/// １カラム分表示データ作成
	/// </summary>
	/// <param name="iIndex">対象行(0～)</param>
	/// <param name="dvSource">追加元DataView</param>
	/// <param name="strColumns">対象カラム</param>
	/// <param name="blInsertData">実データ追加可否</param>
	/// <param name="alBefore">前の列データ（比較用）</param>
	/// <returns>表示データの配列</returns>
	private Hashtable CreateDispData(DataView dvSource, int iIndex , string strColumns, bool blInsertData, ArrayList alBefore, int iCompareArrayIndex)
	{
		Hashtable htData = new Hashtable();

		if (blInsertData == true)
		{
			DataRowView drv = dvSource[iIndex];

			// データ追加
			htData.Add("data", drv[strColumns].ToString() );

			// 比率追加
			if (alBefore != null)
			{
				htData.Add("rate", Form_UserConditionReport_UserConditionReportList.DispIncreasingRate(drv[strColumns], ((Hashtable)alBefore[iCompareArrayIndex])["data"]));
			}
			else
			{
				htData.Add("rate", "－" );
			}
		}
		else
		{
			htData.Add("data", "" );
			htData.Add("rate", "－" );
		}

		return htData;
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
			// 「ユーザー状況レポート {0}」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER_STATUS_REPORT),
			tdReportInfo.InnerText.Trim()));
		sbRecords.Append(CreateRecordCsv(lTitleParams));
		lTitleParams.Clear();
		sbRecords.Append(CreateRecordCsv(lTitleParams));

		//------------------------------------------------------
		// ヘッダ作成
		//------------------------------------------------------
		List<string> lHeaderParams = new List<string>();
		lHeaderParams.Add(
			// 「ユーザ区分」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER_KBN));
		lHeaderParams.Add(
			// 「ユーザ状況」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER_STATUS));
		lHeaderParams.Add(
			// 「人数/増加率」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_TITLE,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_NUMBER_RATE_INCREASE));
		int iCount = 1;
		foreach (Hashtable ht in m_alDispData)
		{
			lHeaderParams.Add((rblReportType.SelectedValue == "0")
				? DateTimeUtility.ToStringForManager(
					new DateTime(m_iCurrentYear, m_iCurrentMonth, iCount),
					DateTimeUtility.FormatType.LongMonthDay)
				// 「{0}月」
				: string.Format(ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2,
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_TIME_KBN,
					Constants.VALUETEXT_PARAM_MAIL_CLICK_REPORT_DETAIL_2_TIME_KBN_MONTH), iCount));
			iCount++;
		}
		StringBuilder sbFileName = new StringBuilder();
		sbFileName.Append("UserConditionReportDetail_").Append(m_iCurrentYear.ToString() + (rblReportType.SelectedValue == "0" ? m_iCurrentMonth.ToString("00") : ""));
		sbRecords.Append(CreateRecordCsv(lHeaderParams));

		//------------------------------------------------------
		// データ作成
		//------------------------------------------------------
		// 「ユーザ区分・ユーザ状況」情報取得
		List<string> lUserKbn = new List<string>();
		List<string> lUserState = new List<string>();
		for (int iPotential = 0; iPotential < cblPotential.Items.Count; iPotential++)
		{
			if (cblPotential.Items[iPotential].Selected)
			{
				switch (iPotential)
				{
					case 0:
						lUserKbn.Add(
							// 「潜在ユーザ」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_POTENTIAL_USER));
						lUserState.Add(
							// 「新規獲得」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_TITLE,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_NEW_ACQUISITION));
						break;
					case 1:
						lUserKbn.Add(
							// 「潜在ユーザ」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_POTENTIAL_USER));
						lUserState.Add(
							// 「全体数」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_TITLE,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_TOTAL_NUMBER));
						break;
					case 2:
						lUserKbn.Add(
							// 「潜在ユーザ」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_POTENTIAL_USER));
						lUserState.Add(
							// 「アクティブユーザ」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_ACTIVE_USER));
						break;
					case 3:
						lUserKbn.Add(
							// 「潜在ユーザ」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_POTENTIAL_USER));
						lUserState.Add(
							// 「休暇ユーザ」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_USER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_VACATION_USER));
						break;
					default:
						break;
				}
			}
		}
		for (int iRecognize = 0; iRecognize < cblRecognize.Items.Count; iRecognize++)
		{
			if (cblRecognize.Items[iRecognize].Selected)
			{
				switch (iRecognize)
				{
					case 0:
						lUserKbn.Add(
							// 「認知顧客」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_CUSTOMER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_COGNITIVE_CUSTOMER));
						lUserState.Add(
							// 「新規獲得」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_TITLE,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_NEW_ACQUISITION));
						break;
					case 1:
						lUserKbn.Add(
							// 「認知顧客」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_CUSTOMER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_COGNITIVE_CUSTOMER));
						lUserState.Add(
							// 「全体数」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_TITLE,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_TOTAL_NUMBER));
						break;
					case 2:
						lUserKbn.Add(
							// 「認知顧客」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_CUSTOMER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_COGNITIVE_CUSTOMER));
						lUserState.Add(
							// 「アクティブ顧客」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_CUSTOMER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_ACTIVE_CUSTOMER));
						break;
					case 3:
						lUserKbn.Add(
							// 「認知顧客」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_CUSTOMER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_COGNITIVE_CUSTOMER));
						lUserState.Add(
							// 「休眠顧客」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_CUSTOMER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_DORMANT_CUSTOMER));
						break;
					case 4:
						lUserKbn.Add(
							// 「認知顧客」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_CUSTOMER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_COGNITIVE_CUSTOMER));
						lUserState.Add(
							// 「退会顧客」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_CUSTOMER,
								Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_WITHDRAWAL_CUSTOMER));
						break;
					default:
						break;
				}
			}
		}

		// 縦軸ループ
		for (int iRow = 0; iRow < ((ArrayList)(((Hashtable)(m_alDispData[0]))["data"])).Count; iRow++ )
		{
			List<string> lDataParams = new List<string>();
			List<string> lRateParams = new List<string>();
			lDataParams.Add(lUserKbn[iRow]);
			lDataParams.Add(lUserState[iRow]);
			lDataParams.Add(
				// 「人数」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
					Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_TITLE,
					Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_NUMBER_PEOPLE));
			lRateParams.Add(lUserKbn[iRow]);
			lRateParams.Add(lUserState[iRow]);
			lRateParams.Add(
				// 「増減率」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL,
					Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_TITLE,
					Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_DETAIL_RATE_CHANGE));
			// 横軸ループ
			for (int iCol = 0; iCol < m_alDispData.Count; iCol++)
			{
				// 人数
				string strData = (string)(((Hashtable)(((ArrayList)((Hashtable)m_alDispData[iCol])["data"])[iRow]))["data"]);
				lDataParams.Add((strData != "") ? StringUtility.ToNumeric(strData) : "");
				// 増加率
				string strRate = (string)(((Hashtable)(((ArrayList)((Hashtable)m_alDispData[iCol])["data"])[iRow]))["rate"]);
				lRateParams.Add(strRate);
			}
			sbRecords.Append(CreateRecordCsv(lDataParams));
			sbRecords.Append(CreateRecordCsv(lRateParams));
		}

		//------------------------------------------------------
		// ファイル出力
		//------------------------------------------------------
		OutPutFileCsv(sbFileName.ToString(), sbRecords.ToString());
	}
}

