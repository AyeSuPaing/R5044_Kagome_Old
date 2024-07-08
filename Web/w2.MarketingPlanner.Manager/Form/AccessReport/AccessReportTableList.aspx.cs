/*
=========================================================================================================
  Module      : アクセスレポートテーブル一覧ページ処理(AccessReportTableList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Form_AccessReport_AccessReportTableList : BasePage
{
	protected int m_iCurrentYear = DateTime.Now.AddDays(-1).Year;
	protected int m_iCurrentMonth = DateTime.Now.AddDays(-1).Month;

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

	protected Hashtable m_htTotal = new Hashtable();

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
			if ((Request[Constants.REQUEST_KEY_CURRENT_YEAR] != null) && (Request[Constants.REQUEST_KEY_CURRENT_MONTH] != null))
			{
				int iYear = 0;
				int iMonth = 0;
				if ((int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_YEAR], out iYear)) && (int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_MONTH], out iMonth)))
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
				string strReportType = Request[REQUEST_KEY_REPORT_TYPE];
				foreach (ListItem li in rblReportType.Items)
				{
					li.Selected = (li.Value == strReportType);
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
		// カレンダ設定
		//------------------------------------------------------
		// 基準カレンダ設定
		lblCurrentCalendar.Text = CalendarUtility.CreateHtmlYMCalendar(m_iCurrentYear, m_iCurrentMonth, Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ACCESS_TABLE_REPORT, REQUEST_KEY_REPORT_TYPE + "=" + rblReportType.SelectedValue, Constants.REQUEST_KEY_CURRENT_YEAR, Constants.REQUEST_KEY_CURRENT_MONTH);

		//------------------------------------------------------
		// 集計データ加工（月初～月末までのデータを用意）
		//------------------------------------------------------
		// まずは当月の最終日を取得
		DateTime dTtemp = new DateTime(m_iCurrentYear, m_iCurrentMonth, 1);
		int iLastDay = dTtemp.AddMonths(1).AddDays(-1).Day;

		ArrayList alDispDatas = GetAccessReportTable(iLastDay);

		//------------------------------------------------------
		// 合計データ作成
		//------------------------------------------------------
		int iPvNum = 0;
		int iUserNum = 0;
		int iSessionNum = 0;
		int iRepuserNum = 0;
		int iNewUserNum = 0;
		int iAvePVSessionNum = 0;
		foreach (Hashtable htData in alDispDatas)
		{
			iPvNum += int.Parse(htData[KBN_DISP_PV_NUM].ToString());
			iUserNum += int.Parse(htData[KBN_DISP_USER_NUM].ToString());
			iSessionNum += int.Parse(htData[KBN_DISP_SESSION_NUM].ToString());
			iRepuserNum += int.Parse(htData[KBN_DISP_REP_USER_NUM].ToString());
			iNewUserNum += int.Parse(htData[KBN_DISP_NEW_USER_NUM].ToString());
			iAvePVSessionNum += int.Parse(htData[KBN_DISP_AVE_PVSESSION_NUM].ToString());
		}
		m_htTotal.Add(KBN_DISP_PV_NUM, iPvNum.ToString());
		m_htTotal.Add(KBN_DISP_USER_NUM, iUserNum.ToString());
		m_htTotal.Add(KBN_DISP_SESSION_NUM, iSessionNum.ToString());
		m_htTotal.Add(KBN_DISP_REP_USER_NUM, iRepuserNum.ToString());
		m_htTotal.Add(KBN_DISP_NEW_USER_NUM, iNewUserNum.ToString());
		m_htTotal.Add(KBN_DISP_AVE_PVSESSION_NUM, (iAvePVSessionNum / alDispDatas.Count).ToString());

		lDataPvNumTotal.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(m_htTotal[KBN_DISP_PV_NUM]));
		lDataUserNumTotal.Text = ((rblReportType.SelectedValue == KBN_REPORT_TYPE_PC) || (rblReportType.SelectedValue == KBN_REPORT_TYPE_SMART_PHONE) || (rblReportType.SelectedValue == KBN_REPORT_TYPE_ALL)) ? WebSanitizer.HtmlEncode(StringUtility.ToNumeric(m_htTotal[KBN_DISP_USER_NUM])) : "-";
		lDataNewUserNumTotal.Text = ((rblReportType.SelectedValue == KBN_REPORT_TYPE_PC) || (rblReportType.SelectedValue == KBN_REPORT_TYPE_SMART_PHONE) || (rblReportType.SelectedValue == KBN_REPORT_TYPE_ALL)) ? WebSanitizer.HtmlEncode(StringUtility.ToNumeric(m_htTotal[KBN_DISP_NEW_USER_NUM])) : "-";
		lDataRepUserNumTotal.Text = ((rblReportType.SelectedValue == KBN_REPORT_TYPE_PC) || (rblReportType.SelectedValue == KBN_REPORT_TYPE_SMART_PHONE) || (rblReportType.SelectedValue == KBN_REPORT_TYPE_ALL)) ? WebSanitizer.HtmlEncode(StringUtility.ToNumeric(m_htTotal[KBN_DISP_REP_USER_NUM])) : "-";
		lDataSessionNumTotal.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(m_htTotal[KBN_DISP_SESSION_NUM]));
		lDataAvePvSessionNumTotal.Text = WebSanitizer.HtmlEncode(StringUtility.ToNumeric(m_htTotal[KBN_DISP_AVE_PVSESSION_NUM]));

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		rAccessReportTable.DataSource = alDispDatas;
		rAccessReportTable.DataBind();
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
			li.Selected = (li.Value == KBN_REPORT_TYPE_PC);
		}
	}

	/// <summary>
	/// 集計データ取得
	/// </summary>
	/// <param name="strDispKbn">表示区分</param>
	private DataView GetData(string strDispKbn)
	{
		DataView dvResult = null;

		string strStatementName = null;
		switch (strDispKbn)
		{
			case KBN_DISP_PV_NUM:
				strStatementName = "GetPVs";
				break;
			case KBN_DISP_USER_NUM:
				strStatementName = "GetUserNum";
				break;
			case KBN_DISP_SESSION_NUM:
				strStatementName = "GetSessionNum";
				break;
			case KBN_DISP_NEW_USER_NUM:
				strStatementName = "GetNewUserNum";
				break;
			case KBN_DISP_REP_USER_NUM:
				strStatementName = "GetRepUserNum";
				break;
			case KBN_DISP_AVE_PVSESSION_NUM:
				strStatementName = "GetAveragePVSession";
				break;
		}

		// SQL発行
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("AccessAnalysis", strStatementName))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_DISPACCESSANALYSIS_TGT_YEAR, m_iCurrentYear.ToString());
			htInput.Add(Constants.FIELD_DISPACCESSANALYSIS_TGT_MONTH, m_iCurrentMonth.ToString().PadLeft(2, '0'));
			htInput.Add(Constants.FIELD_DISPACCESSANALYSIS_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add("report_type", rblReportType.SelectedValue);

			dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		return dvResult;
	}

	/// <summary>
	/// アクセスレポート情報取得
	/// </summary>
	/// <param name="iLastDay"></param>
	/// <returns></returns>
	private ArrayList GetAccessReportTable(int iLastDay)
	{
		// 変数宣言
		ArrayList alResult = new ArrayList();

		GetAccessReportTableData(alResult, iLastDay, KBN_DISP_PV_NUM);
		GetAccessReportTableData(alResult, iLastDay, KBN_DISP_USER_NUM);
		GetAccessReportTableData(alResult, iLastDay, KBN_DISP_SESSION_NUM);
		GetAccessReportTableData(alResult, iLastDay, KBN_DISP_REP_USER_NUM);
		GetAccessReportTableData(alResult, iLastDay, KBN_DISP_NEW_USER_NUM);
		GetAccessReportTableData(alResult, iLastDay, KBN_DISP_AVE_PVSESSION_NUM);

		return alResult;
	}

	/// <summary>
	/// 各表示区分毎のアクセスレポート情報取得
	/// </summary>
	/// <param name="alTable"></param>
	/// <param name="iLastDay">は当月の最終日</param>
	/// <param name="strDispKbn">表示区分</param>
	private void GetAccessReportTableData(ArrayList alTable, int iLastDay, string strDispKbn)
	{
		//------------------------------------------------------
		// 集計データ取得
		//------------------------------------------------------
		DataView dvDisplayData = GetData(strDispKbn);

		// データ加工
		int iDRVCount = 0;
		for (int iDayLoop = 1; iDayLoop <= iLastDay; iDayLoop++)
		{
			string strValue = null;
			if ((iDRVCount < dvDisplayData.Count) && (iDayLoop == int.Parse(dvDisplayData[iDRVCount][Constants.FIELD_DISPACCESSANALYSIS_TGT_DAY].ToString())))
			{
				strValue = dvDisplayData[iDRVCount]["counts"].ToString();
				iDRVCount++;
			}
			else
			{
				strValue = "0";
			}

			// 表データ用
			if (alTable.Count != iLastDay)
			{
				Hashtable htTableData = new Hashtable();
				htTableData.Add(strDispKbn, strValue);
				alTable.Add(htTableData);
			}
			else
			{
				((Hashtable)alTable[iDayLoop - 1]).Add(strDispKbn, strValue);
			}
		}
	}

	/// <summary>
	/// 曜日取得
	/// </summary>
	/// <param name="alChart">年</param>
	/// <param name="iLastDay">月</param>
	/// <param name="strDispKbn">日</param>
	protected string GetWeekValue(int iYear, int iMonth, int iDay)
	{
		DateTime dt = new DateTime(iYear, iMonth, iDay);
		return dt.ToString("ddd");
	}

	/// <summary>
	/// 曜日用リストCSSクラス取得
	/// </summary>
	/// <param name="alChart">年</param>
	/// <param name="iLastDay">月</param>
	/// <param name="strDispKbn">日</param>
	protected string GetWeekClass(int iYear, int iMonth, int iDay)
	{
		string strrResult = "list_item_bg1";
		DateTime dt = new DateTime(iYear, iMonth, iDay);

		if (dt.DayOfWeek == DayOfWeek.Saturday)
		{
			strrResult = "list_item_bg_wk_sat";
		}
		else if (dt.DayOfWeek == DayOfWeek.Sunday)
		{
			strrResult = "list_item_bg_wk_sun";
		}

		return strrResult;
	}

	/// <summary>
	/// アクセスランキング情報表示
	/// </summary>
	/// <returns></returns>
	protected string GetAccessReportTableInfo()
	{
		var result = string.Format(
			"{0}　【{1}】",
			DateTimeUtility.ToStringForManager(
				new DateTime(m_iCurrentYear, m_iCurrentMonth, 1),
				DateTimeUtility.FormatType.LongYearMonth),
			rblReportType.Items.FindByValue(rblReportType.SelectedValue));
		return result;
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
		lTitleParams.Add(string.Format("{0}　{1}",
			// 「アクセスレポート」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT),
			GetAccessReportTableInfo()));
		sbRecords.Append(CreateRecordCsv(lTitleParams));
		lTitleParams.Clear();
		sbRecords.Append(CreateRecordCsv(lTitleParams));

		//------------------------------------------------------
		// ヘッダ作成
		//------------------------------------------------------
		List<string> lHeaderParams = new List<string>();
		lHeaderParams.Add(
			// 「レポート区分」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_REPORT_CLASSIFICATION));
		foreach (RepeaterItem ri in rAccessReportTable.Items)
		{
			lHeaderParams.Add(
				DateTimeUtility.ToStringForManager(
					new DateTime(m_iCurrentYear, m_iCurrentMonth, ri.ItemIndex + 1),
					DateTimeUtility.FormatType.DayWeekOfDay));
		}
		lHeaderParams.Add(
			// 「合計」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_TOTAL));
		StringBuilder sbFileName = new StringBuilder();
		sbFileName.Append("AccessReportTableList_").Append(m_iCurrentYear.ToString()).Append(m_iCurrentMonth.ToString("00"));
		sbRecords.Append(CreateRecordCsv(lHeaderParams));

		//------------------------------------------------------
		// データ作成
		//------------------------------------------------------
		List<string> lDataPvNumParams = new List<string>();
		List<string> lDataUserNumParams = new List<string>();
		List<string> lDataNewUserNumParams = new List<string>();
		List<string> lDataRepUserNumParams = new List<string>();
		List<string> lDataSessionNumParams = new List<string>();
		List<string> lDataAvePvSessionNumParams = new List<string>();
		var suffix = (rblReportType.SelectedValue == KBN_REPORT_TYPE_ALL)
			// 「（PC・スマフォ）」
			? ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ACCESS_REPORT,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_TITLE,
				Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_PC_SMARTPHONE)
			: string.Empty;

		if (rAccessReportTable.Items.Count > 0)
		{
			lDataPvNumParams.Add(
				// 「ページビュー数」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ACCESS_REPORT,
					Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_TITLE,
					Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_NUMBER_PAGE_VIEW));
			lDataUserNumParams.Add(string.Format(
				// 「訪問ユーザ数{0}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ACCESS_REPORT,
					Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_TITLE,
					Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_NUMBER_VISITING_USER),
				suffix));
			lDataNewUserNumParams.Add(string.Format(
				// 「新規訪問ユーザ数{0}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ACCESS_REPORT,
					Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_TITLE,
					Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_NUMBER_NEW_VISITOR),
				suffix));
			lDataRepUserNumParams.Add(string.Format(
				// 「リピート訪問ユーザ数{0}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ACCESS_REPORT,
					Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_TITLE,
					Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_NUMBER_REPEAT_VISITING_USER),
				suffix));
			lDataSessionNumParams.Add(
				// 「訪問数」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ACCESS_REPORT,
					Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_TITLE,
					Constants.VALUETEXT_PARAM_ACCESS_REPORT_LIST_NUMBER_VISIT));
			lDataAvePvSessionNumParams.Add(
				// 「1回の訪問あたりの平均ページビュー数」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ACCESS_REPORT,
					Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_TITLE,
					Constants.VALUETEXT_PARAM_ACCESS_REPORT_TABLE_LIST_AVERAGE_PAGE_VIEW_PER_VISIT));
			foreach (RepeaterItem ri in rAccessReportTable.Items)
			{
				lDataPvNumParams.Add(((Literal)ri.FindControl("lDataPvNum")).Text);
				lDataUserNumParams.Add(((Literal)ri.FindControl("lDataUserNum")).Text);
				lDataNewUserNumParams.Add(((Literal)ri.FindControl("lDataNewUserNum")).Text);
				lDataRepUserNumParams.Add(((Literal)ri.FindControl("lDataRepUserNum")).Text);
				lDataSessionNumParams.Add(((Literal)ri.FindControl("lDataSessionNum")).Text);
				lDataAvePvSessionNumParams.Add(((Literal)ri.FindControl("lDataAvePvSessionNum")).Text);
			}
			lDataPvNumParams.Add(lDataPvNumTotal.Text);
			lDataUserNumParams.Add(lDataUserNumTotal.Text);
			lDataNewUserNumParams.Add(lDataNewUserNumTotal.Text);
			lDataRepUserNumParams.Add(lDataRepUserNumTotal.Text);
			lDataSessionNumParams.Add(lDataSessionNumTotal.Text);
			lDataAvePvSessionNumParams.Add(lDataAvePvSessionNumTotal.Text);
		}
		sbRecords.Append(CreateRecordCsv(lDataPvNumParams));

		// モバイルの時は一部データは表示しない
		if (rblReportType.SelectedValue != KBN_REPORT_TYPE_MOBILE)
		{
			sbRecords.Append(CreateRecordCsv(lDataUserNumParams));
			sbRecords.Append(CreateRecordCsv(lDataNewUserNumParams));
			sbRecords.Append(CreateRecordCsv(lDataRepUserNumParams));
		}
		
		sbRecords.Append(CreateRecordCsv(lDataSessionNumParams));
		sbRecords.Append(CreateRecordCsv(lDataAvePvSessionNumParams));

		//------------------------------------------------------
		// ファイル出力
		//------------------------------------------------------
		OutPutFileCsv(sbFileName.ToString(), sbRecords.ToString());
	}
}
