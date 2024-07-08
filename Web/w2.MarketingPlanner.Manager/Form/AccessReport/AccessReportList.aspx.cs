/*
=========================================================================================================
  Module      : アクセスレポート一覧ページ処理(AccessReportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using w2.Common.Helper;

public partial class Form_AccessReport_AccessReportList : BasePage
{
	protected ArrayList m_alTableData = new ArrayList();

	protected int m_iCurrentYear = DateTime.Now.AddDays(-1).Year;
	protected int m_iCurrentMonth = DateTime.Now.AddDays(-1).Month;

	private string m_strHorizonalLabel;
	private string m_strVerticalLabel;
	private string m_strVerticalUnit;
	private string m_strStatementName;

	protected const string REQUEST_KEY_REPORT_TYPE = "rt";
	protected const string REQUEST_KEY_DATA_KBN = "dk";
	protected const string REQUEST_KEY_CHART_TYPE = "ct";

	protected const string KBN_REPORT_TYPE_ALL = "all";
	protected const string KBN_REPORT_TYPE_PC = "pc";
	protected const string KBN_REPORT_TYPE_MOBILE = "mobile";
	protected const string KBN_REPORT_TYPE_SMART_PHONE = "smart_phone";

	protected const string KBN_DISP_PV_NUM = "0";
	protected const string KBN_DISP_USER_NUM = "1";
	protected const string KBN_DISP_SESSION_NUM = "2";
	protected const string KBN_DISP_REP_USER_NUM = "3";
	protected const string KBN_DISP_NEW_USER_NUM = "4";
	protected const string KBN_DISP_AVE_PVSESSION_NUM = "5";

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
			// コンポーネント初期化
			//------------------------------------------------------
			InitializeCompornent();

			//------------------------------------------------------
			// リクエストパラメタ取得
			//------------------------------------------------------
			// 対象年月
			if ((Request[Constants.REQUEST_KEY_CURRENT_YEAR] != null)
				&& (Request[Constants.REQUEST_KEY_CURRENT_MONTH] != null))
			{
				var iYear = 0;
				var iMonth = 0;
				if ((int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_YEAR], out iYear)) && (int.TryParse(
					Request[Constants.REQUEST_KEY_CURRENT_MONTH],
					out iMonth)))
				{
					m_iCurrentYear = iYear;
					m_iCurrentMonth = iMonth;
				}
			}

			ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] = m_iCurrentYear;
			ViewState[Constants.REQUEST_KEY_CURRENT_MONTH] = m_iCurrentMonth;

			// レポートタイプ
			if (Request[REQUEST_KEY_REPORT_TYPE] != null)
			{
				var strReportType = Request[REQUEST_KEY_REPORT_TYPE];
				foreach (ListItem li in rblReportType.Items)
				{
					li.Selected = (li.Value == strReportType);
				}
			}

			// グラフタイプ
			if (Request[REQUEST_KEY_CHART_TYPE] != null)
			{
				var strChartType = Request[REQUEST_KEY_CHART_TYPE];
				foreach (ListItem li in rblChartType.Items)
				{
					li.Selected = (li.Value == strChartType); // 折れ線グラフか棒グラフの選択が可能
				}
			}

			// データタイプ
			if (Request[REQUEST_KEY_DATA_KBN] != null)
			{
				rbPV.Checked = rbUsers.Checked = rbSessions.Checked =
					rbRepeatUsers.Checked = rbNewUsers.Checked = rbAveragePVSession.Checked = false;

				switch (Request[REQUEST_KEY_DATA_KBN])
				{
					case KBN_DISP_PV_NUM:
						rbPV.Checked = true;
						break;
					case KBN_DISP_USER_NUM:
						rbUsers.Checked = true;
						break;
					case KBN_DISP_SESSION_NUM:
						rbSessions.Checked = true;
						break;
					case KBN_DISP_REP_USER_NUM:
						rbRepeatUsers.Checked = true;
						break;
					case KBN_DISP_NEW_USER_NUM:
						rbNewUsers.Checked = true;
						break;
					case KBN_DISP_AVE_PVSESSION_NUM:
						rbAveragePVSession.Checked = true;
						break;
					default:
						rbPV.Checked = true; // デフォルトはＰＶ数
						break;
				}
			}
		}
		else
		{
			//------------------------------------------------------
			// ビューステート取得
			//------------------------------------------------------
			if (ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] != null)
			{
				m_iCurrentYear = (int)ViewState[Constants.REQUEST_KEY_CURRENT_YEAR];
				m_iCurrentMonth = (int)ViewState[Constants.REQUEST_KEY_CURRENT_MONTH];
			}
		}

		//------------------------------------------------------
		// ラジオボタン制御
		//------------------------------------------------------
		switch (rblReportType.SelectedValue)
		{
			case KBN_REPORT_TYPE_ALL:
			case KBN_REPORT_TYPE_PC:
			case KBN_REPORT_TYPE_SMART_PHONE:
				// PCだったら訪問ユーザ数OK
				rbUsers.Enabled = rbNewUsers.Enabled = rbRepeatUsers.Enabled = true;
				break;

			case KBN_REPORT_TYPE_MOBILE:
				// PCでなければ訪問ユーザ数NG （もしチェックされていたらPVをチェックする）
				rbUsers.Enabled = rbNewUsers.Enabled = rbRepeatUsers.Enabled = false;
				if (rbUsers.Checked || rbNewUsers.Checked || rbRepeatUsers.Checked)
				{
					rbPV.Checked = true;
					rbUsers.Checked = rbNewUsers.Checked = rbRepeatUsers.Checked = false;
				}
				break;
		}

		//------------------------------------------------------
		// レポート設定情報取得
		//------------------------------------------------------
		string strDispKbn = null;
		if (rbPV.Checked)
		{
			strDispKbn = KBN_DISP_PV_NUM;
			// 「日付」
			m_strHorizonalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_DATE);
			// 「ページビュー数」
			m_strVerticalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_NUMBER_PAGE_VIEW);
			m_strVerticalUnit = "PV";
			m_strStatementName = "GetPVs";
		}
		else if (rbUsers.Checked)
		{
			strDispKbn = KBN_DISP_USER_NUM;
			// 「日付」
			m_strHorizonalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_DATE);
			// 「ユーザ数」
			m_strVerticalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_NUMBER_USER);
			// 「人」
			m_strVerticalUnit = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_MAN);
			m_strStatementName = "GetUserNum";
		}
		else if (rbSessions.Checked)
		{
			strDispKbn = KBN_DISP_SESSION_NUM;
			// 「日付」
			m_strHorizonalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_DATE);
			// 「訪問数」
			m_strVerticalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_NUMBER_VISIT);
			// 「回」
			m_strVerticalUnit = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_TIME);
			m_strStatementName = "GetSessionNum";
		}
		else if (rbRepeatUsers.Checked)
		{
			strDispKbn = KBN_DISP_REP_USER_NUM;
			// 「日付」
			m_strHorizonalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_DATE);
			// 「ユーザ数」
			m_strVerticalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_NUMBER_USER);
			// 「人」
			m_strVerticalUnit = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_MAN);
			m_strStatementName = "GetRepUserNum";
		}
		else if (rbNewUsers.Checked)
		{
			strDispKbn = KBN_DISP_NEW_USER_NUM;
			// 「日付」
			m_strHorizonalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_DATE);
			// 「ユーザ数」
			m_strVerticalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_NUMBER_USER);
			// 「人」
			m_strVerticalUnit = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_MAN);
			m_strStatementName = "GetNewUserNum";
		}
		else if (rbAveragePVSession.Checked)
		{
			strDispKbn = KBN_DISP_AVE_PVSESSION_NUM;
			// 「日付」
			m_strHorizonalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_DATE);
			// 「ページビュー数」
			m_strVerticalLabel = ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_NUMBER_PAGE_VIEW);
			m_strVerticalUnit = "PV";
			m_strStatementName = "GetAveragePVSession";
		}

		//------------------------------------------------------
		// カレンダの遷移URLセット（
		//------------------------------------------------------
		// 基準カレンダパラメタ設定
		var sbParam = new StringBuilder();
		sbParam.Append(REQUEST_KEY_REPORT_TYPE).Append("=").Append(rblReportType.SelectedValue);
		sbParam.Append("&").Append(REQUEST_KEY_DATA_KBN).Append("=").Append(strDispKbn);
		sbParam.Append("&").Append(REQUEST_KEY_CHART_TYPE).Append("=").Append(rblChartType.SelectedValue);

		lblCurrentCalendar.Text = CalendarUtility.CreateHtmlYMCalendar(
			m_iCurrentYear,
			m_iCurrentMonth,
			Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ACCESS_REPORT,
			sbParam.ToString(),
			Constants.REQUEST_KEY_CURRENT_YEAR,
			Constants.REQUEST_KEY_CURRENT_MONTH);

		//------------------------------------------------------
		// 集計データ取得
		//------------------------------------------------------
		DataView dvDisplayData;
		using (var sqlAccessor = new SqlAccessor())
		using (var sqlStatement = new SqlStatement("AccessAnalysis", m_strStatementName))
		{
			var htInput = new Hashtable();
			htInput.Add(Constants.FIELD_DISPACCESSANALYSIS_TGT_YEAR, m_iCurrentYear.ToString());
			htInput.Add(Constants.FIELD_DISPACCESSANALYSIS_TGT_MONTH, m_iCurrentMonth.ToString().PadLeft(2, '0'));
			htInput.Add(Constants.FIELD_DISPACCESSANALYSIS_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add("report_type", rblReportType.SelectedValue);

			dvDisplayData = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

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
				dvDisplayData[iDRVCount][Constants.FIELD_DISPACCESSANALYSIS_TGT_DAY].ToString())))
			{
				strValue = dvDisplayData[iDRVCount]["counts"].ToString();
				iDRVCount++;
			}
			else
			{
				strValue = "0";
			}

			// 表データ用
			var htTableData = new Hashtable();
			htTableData.Add(TABLE_HEADER_DAY, iDayLoop.ToString());
			htTableData.Add(TABLE_DATA_VALUE, strValue);
			m_alTableData.Add(htTableData);
		}

		//------------------------------------------------------
		// アクセスレポートグラフ作成
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
		CreateFunction(m_iCurrentMonth, jsonValues, chartType, m_strHorizonalLabel, m_strVerticalLabel, m_strVerticalUnit, isViewNumber);

		//------------------------------------------------------
		// 表データ設定
		//------------------------------------------------------
		rTableHeader.DataSource = m_alTableData;
		rTableHeader.DataBind();

		rTableData.DataSource = m_alTableData;
		rTableData.DataBind();
	}

	/// <summary>
	/// テーブル日付CLASS取得
	/// </summary>
	/// <param name="strDateString"></param>
	/// <returns>テーブル日付CLASS</returns>
	protected string CreateTableDayClassName(string strDateString)
	{
		string strResult;
		switch (DateTime.Parse(strDateString).DayOfWeek)
		{
			case DayOfWeek.Saturday:
				strResult = "list_item_bg_wk_sat";
				break;

			case DayOfWeek.Sunday:
				strResult = "list_item_bg_wk_sun";
				break;

			default:
				strResult = "list_item_bg";
				break;
		}

		return strResult;
	}

	/// <summary>
	/// 表示区分文字列取得
	/// </summary>
	/// <returns>表示区分文字列</returns>
	protected string CreateDisplayKbnStrinig()
	{
		string strResult = null;

		if (rbPV.Checked)
		{
			strResult = rbPV.Text;
		}
		else if (rbUsers.Checked)
		{
			strResult = rbUsers.Text;
		}
		else if (rbSessions.Checked)
		{
			strResult = rbSessions.Text;
		}
		else if (rbRepeatUsers.Checked)
		{
			strResult = rbRepeatUsers.Text;
		}
		else if (rbNewUsers.Checked)
		{
			strResult = rbNewUsers.Text;
		}
		else if (rbAveragePVSession.Checked)
		{
			strResult = rbAveragePVSession.Text;
		}

		return strResult;
	}

	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, EventArgs e)
	{
		var sbRecords = new StringBuilder();

		//------------------------------------------------------
		// タイトル作成
		//------------------------------------------------------
		var sbHeader = new StringBuilder();
		var lTitleParams = new List<string>();
		sbHeader.Append(
			DateTimeUtility.ToStringForManager(
				new DateTime(m_iCurrentYear, m_iCurrentMonth, 1),
				DateTimeUtility.FormatType.LongYearMonth));
		sbHeader.AppendFormat("{0}【{1}】 {2}",
			// 「アクセスレポート」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT),
			rblReportType.Items.FindByValue(rblReportType.SelectedValue).Text,
			CreateDisplayKbnStrinig());
		lTitleParams.Add(sbHeader.ToString());
		sbRecords.Append(CreateRecordCsv(lTitleParams));
		lTitleParams.Clear();
		sbRecords.Append(CreateRecordCsv(lTitleParams));

		//------------------------------------------------------
		// ヘッダ作成
		//------------------------------------------------------
		var lHeaderParams = new List<string>();
		foreach (RepeaterItem ri in rTableHeader.Items)
		{
			lHeaderParams.Add(((Literal)ri.FindControl("lHeader")).Text);
		}

		var sbFileName = new StringBuilder();
		sbFileName.Append("AccessReportList_").Append(m_iCurrentYear.ToString()).Append(m_iCurrentMonth.ToString("00"));
		sbRecords.Append(CreateRecordCsv(lHeaderParams));

		//------------------------------------------------------
		// データ作成
		//------------------------------------------------------
		var lDataParams = new List<string>();
		if (rTableData.Items.Count > 0)
		{
			foreach (RepeaterItem ri in rTableData.Items)
			{
				lDataParams.Add(((Literal)ri.FindControl("lData")).Text);
			}
		}

		sbRecords.Append(CreateRecordCsv(lDataParams));

		//------------------------------------------------------
		// ファイル出力
		//------------------------------------------------------
		OutPutFileCsv(sbFileName.ToString(), sbRecords.ToString());
	}

	/// <summary>
	/// グラフ作成のFunction呼び出し
	/// </summary>
	/// <param name="month">月</param>
	/// <param name="values">データ値</param>
	/// <param name="chartType">グラフの種類</param>
	/// <param name="xAxisTitle">x軸のラベルタイトル</param>
	/// <param name="label">x軸の各目盛りのラベル</param>
	/// <param name="valueUnit">データ値の単位</param>
	/// <param name="isViewNumber">数値の表示の可否</param>
	private void CreateFunction(int month, string values, string chartType, string xAxisTitle,string label, string valueUnit, bool isViewNumber)
	{
		lCreateScript.Text = "<script>CreateCharts('" + month + "', " + values + ", '" + chartType + "', '" + xAxisTitle + "', '"
			+ label + "', '" + valueUnit + "', '" + isViewNumber + "', 'report')</script>";
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeCompornent()
	{
		// レポートタイプラジオボタン作成
		rblReportType.Items.Add(new ListItem(
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_REPORT_TYPE,
				KBN_REPORT_TYPE_ALL),
			KBN_REPORT_TYPE_ALL));
		rblReportType.Items.Add(new ListItem(
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_REPORT_TYPE,
				KBN_REPORT_TYPE_PC),
			KBN_REPORT_TYPE_PC));
		rblReportType.Items.Add(new ListItem(
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_REPORT_TYPE,
				KBN_REPORT_TYPE_SMART_PHONE),
			KBN_REPORT_TYPE_SMART_PHONE));
		if (Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED) rblReportType.Items.Add(
			new ListItem(
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ACCESS_REPORT,
					Constants.VALUETEXT_PARAM_ACCESS_REPORT_REPORT_TYPE,
					KBN_REPORT_TYPE_MOBILE),
				KBN_REPORT_TYPE_MOBILE));
		foreach (ListItem li in rblReportType.Items)
		{
			li.Selected = (li.Value == KBN_REPORT_TYPE_ALL);
		}
	}
}