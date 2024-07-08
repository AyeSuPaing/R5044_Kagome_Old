/*
=========================================================================================================
  Module      : 顧客状況詳細レポートページ処理(UserConditionReportList.aspx.cs)
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

public partial class Form_UserConditionReport_UserConditionReportList : BasePage
{
	protected int m_iCurrentYear = DateTime.Now.AddDays(-1).Year;
	protected int m_iCurrentMonth = DateTime.Now.AddDays(-1).Month;
	protected int m_iTargetYear = DateTime.Now.AddDays(-1).Year;
	protected int m_iTargetMonth = DateTime.Now.AddDays(-1).Month;

	protected DataRowView m_drvCurrentData = null;
	protected DataRowView m_drvTargetData = null;


	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// パラメータ取得
		//------------------------------------------------------
		try
		{
			// 日付チェック・格納
			DateTime.Parse(Request[Constants.REQUEST_KEY_CURRENT_YEAR] + "/" + Request[Constants.REQUEST_KEY_CURRENT_MONTH] + "/1");
			m_iCurrentYear = int.Parse(Request.Params.Get(Constants.REQUEST_KEY_CURRENT_YEAR));
			m_iCurrentMonth = int.Parse(Request.Params.Get(Constants.REQUEST_KEY_CURRENT_MONTH));
		}
		catch
		{
			// 日付変換エラー（当月が対象になる）
		}

		try
		{
			// 日付チェック・格納
			DateTime.Parse(Request[Constants.REQUEST_KEY_TARGET_YEAR] + "/" + Request[Constants.REQUEST_KEY_TARGET_MONTH] + "/1");
			m_iTargetYear = int.Parse(Request.Params.Get(Constants.REQUEST_KEY_TARGET_YEAR));
			m_iTargetMonth = int.Parse(Request.Params.Get(Constants.REQUEST_KEY_TARGET_MONTH));
		}
		catch
		{
			// 日付変換エラー（カレントの前月を表示）
			DateTime dtTarget = new DateTime(m_iCurrentYear, m_iCurrentMonth, 1).AddMonths(-1);
			m_iTargetYear = dtTarget.Year;
			m_iTargetMonth = dtTarget.Month;
		}

		//------------------------------------------------------
		// カレンダ設定
		//------------------------------------------------------
		// 基準カレンダパラメタ設定
		string strParamForCurrent = Constants.REQUEST_KEY_TARGET_YEAR + "=" + m_iTargetYear.ToString() + "&" + Constants.REQUEST_KEY_TARGET_MONTH + "=" + m_iTargetMonth.ToString();
		// 基準カレンダ設定
		lblCurrentCalendar.Text = CalendarUtility.CreateHtmlYMCalendar(m_iCurrentYear, m_iCurrentMonth, Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USER_CONDITION_REPORT_LIST, strParamForCurrent, Constants.REQUEST_KEY_CURRENT_YEAR, Constants.REQUEST_KEY_CURRENT_MONTH);

		// 対象カレンダパラメタ設定
		string strParamForTarget = Constants.REQUEST_KEY_CURRENT_YEAR + "=" + m_iCurrentYear.ToString() + "&" + Constants.REQUEST_KEY_CURRENT_MONTH + "=" + m_iCurrentMonth.ToString();
		// 対象カレンダ設定
		lblTargetCalendar.Text = CalendarUtility.CreateHtmlYMCalendar(m_iTargetYear, m_iTargetMonth, Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_USER_CONDITION_REPORT_LIST, strParamForTarget, Constants.REQUEST_KEY_TARGET_YEAR, Constants.REQUEST_KEY_TARGET_MONTH);
	
		//------------------------------------------------------
		// 顧客分析結果取得
		//------------------------------------------------------
		GetUserAnalysisData();

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		DataBind();
	}

	/// <summary>
	/// 顧客分析結果取得
	/// </summary>
	private void GetUserAnalysisData()
	{
		// 日次データ取得
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("UserConditionAnalysis","GetUserAnalysisList"))
		{
			// 基準カレンダ分析結果取得
			Hashtable htInputCurrent = new Hashtable();
			htInputCurrent.Add(Constants.FIELD_DISPUSERANALYSIS_TGT_YEAR, m_iCurrentYear.ToString());
			htInputCurrent.Add(Constants.FIELD_DISPUSERANALYSIS_TGT_MONTH , m_iCurrentMonth.ToString().PadLeft(2,'0'));
			htInputCurrent.Add(Constants.FIELD_DISPUSERANALYSIS_DEPT_ID, this.LoginOperatorDeptId);
			DataView dvCurrentData = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInputCurrent);
			if (dvCurrentData.Count != 0)
			{
				m_drvCurrentData = dvCurrentData[0];
			}

			// 対象カレンダ分析結果取得
			Hashtable htInputTaraget = new Hashtable();
			htInputTaraget.Add(Constants.FIELD_DISPUSERANALYSIS_TGT_YEAR, m_iTargetYear.ToString());
			htInputTaraget.Add(Constants.FIELD_DISPUSERANALYSIS_TGT_MONTH, m_iTargetMonth.ToString().PadLeft(2, '0'));
			htInputTaraget.Add(Constants.FIELD_DISPUSERANALYSIS_DEPT_ID, this.LoginOperatorDeptId);
			DataView dvTargetData = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInputTaraget);
			if (dvTargetData.Count != 0)
			{
				m_drvTargetData  = dvTargetData[0];
			}
		}
	}

	/// <summary>
	/// 指定数値データ表示（単位付き）
	/// </summary>
	/// <param name="drvDisp">表示するデータのDataRowView</param>
	/// <param name="strColumnName">表示するデータのカラム名</param>
	/// <param name="strUnit">単位</param>
	/// <returns>データ</returns>
	protected string DispNumeric(DataRowView drvDisp, string strColumnName, string strUnit)
	{
		string strResult = "　";

		if (drvDisp != null)
		{
			if (drvDisp[strColumnName] != System.DBNull.Value)
			{
				strResult = StringUtility.ToNumeric(drvDisp[strColumnName]) + strUnit;
			}
		}

		return strResult;
	}

	/// <summary>
	/// 指定データ表示
	/// </summary>
	/// <param name="drvDisp">表示するデータのDataRowView</param>
	/// <param name="strColumnName">表示するデータのカラム名</param>
	/// <returns>データ</returns>
	protected string DispData(DataRowView drvDisp, string strColumnName)
	{
		string strResult = "　";

		if (drvDisp != null)
		{
			if (drvDisp[strColumnName] != System.DBNull.Value)
			{
				strResult = drvDisp[strColumnName].ToString();
			}
		}

		return strResult; 
	}

	/// <summary>
	/// 上昇率表示
	/// </summary>
	/// <param name="objCurrent">対象データ</param>
	/// <param name="objTarget">比較データ</param>
	/// <returns>比較対象データ→比較データの上昇率</returns>
	public static string DispIncreasingRate(object objCurrent, object objTarget)
	{
		string strResult = "－";
		int iIncRate = 0;

		double dCurrent = 0;
		double dTarget = 0;

		try
		{
			dCurrent = double.Parse(objCurrent.ToString());
			dTarget = double.Parse(objTarget.ToString());

			if (dTarget == 0)
			{
				if (dCurrent == 0)
				{
					strResult = "0 % →";
				}
				else
				{
					strResult = "－ % ↑";
				}
			}
			else
			{
				iIncRate = (int)AnalysisUtility.GetIncreasingRate(dCurrent, dTarget);

				if (iIncRate > 0)
				{
					strResult = iIncRate.ToString() + " % ↑";
				}
				else if (iIncRate == 0)
				{
					strResult = "0 % →";
				}
				else if (iIncRate < 0)
				{
					strResult = (-1 * iIncRate).ToString() + " % ↓";
				}
			}
		}
		catch (Exception)
		{
			// パラメタが数値変換できない場合
		}

		return strResult; 
	}

	/// <summary>
	/// 割合表示
	/// </summary>
	/// <param name="objCurrent">比較データ</param>
	/// <param name="objTarget">比較対象データ</param>
	/// <returns>比較データ÷比較対象データの割合</returns>
	public static string DispRate(object objCurrent, object objTarget)
	{
		string strResult = null;
		string strRate = AnalysisUtility.GetRateString(objCurrent, objTarget, 1);

		if (strRate != null)
		{
			strResult = strRate + "％";
		}
		else
		{
			strResult = "－％";
		}

		return strResult; 
	}

	/// <summary>
	/// 割合表示（CSV出力用）
	/// </summary>
	/// <param name="objCurrent">比較データ</param>
	/// <param name="objTarget">比較対象データ</param>
	/// <returns>比較データ÷比較対象データの割合</returns>
	public static string DispRateCsv(object objCurrent, object objTarget)
	{
		string strResult = null;
		string strRate = AnalysisUtility.GetRateString(objCurrent, objTarget, 1);

		if (strRate != null)
		{
			strResult = strRate;
		}
		else
		{
			strResult = "－";
		}

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
		var currentYearMonth = DateTimeUtility.ToStringForManager(
			new DateTime(m_iCurrentYear, m_iCurrentMonth, 1),
			DateTimeUtility.FormatType.LongYearMonth);
		var targetYearMonth = DateTimeUtility.ToStringForManager(
			new DateTime(m_iTargetYear, m_iTargetMonth, 1),
			DateTimeUtility.FormatType.LongYearMonth);
		lTitleParams.Add(string.Format(
			// 「ユーザー状況レポート　対象月：{0}　比較月：{1}」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER_STATUS_REPORT),
			currentYearMonth,
			targetYearMonth));
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
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER_KBN));
		lHeaderParams.Add(
			// 「ユーザ状況」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER_STATUS));
		lHeaderParams.Add(string.Format(
			// 「{0}(人)」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_MAN),
			currentYearMonth));
		lHeaderParams.Add(targetYearMonth + "（％）");
 		StringBuilder sbFileName = new StringBuilder();
		sbFileName.Append("UserConditionReportList_").Append(m_iCurrentYear.ToString() + m_iCurrentMonth.ToString("00") + "_" + m_iTargetYear.ToString() + m_iTargetMonth.ToString("00"));
		sbRecords.Append(CreateRecordCsv(lHeaderParams));

		//------------------------------------------------------
		// データ作成
		//------------------------------------------------------
		List<string> lDataParams = new List<string>();
		// 潜在ユーザ：新規獲得数
		lHeaderParams.Add(string.Format(
			// 「{0}（増減）」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_INCREASE_DECREASE),
			currentYearMonth));
		lHeaderParams.Add(string.Format(
			// 「{0}(人)」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_MAN),
			targetYearMonth));
		lDataParams.Add(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_NEW + "_total", ""));
		lDataParams.Add("");
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_NEW + "_total"), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_NEW + "_total")));
		lDataParams.Add(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_NEW + "_total", ""));
		lDataParams.Add("");
		sbRecords.Append(CreateRecordCsv(lDataParams));
		// 潜在ユーザ
		lDataParams.Clear();
		lDataParams.Add(
			// 「潜在ユーザ」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_POTENTIAL_USER));
		lDataParams.Add(
			// 「新規獲得数」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_NUMBER_NEW_ACQUISITION));
		lDataParams.Add(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL, ""));
		lDataParams.Add("");
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL)));
		lDataParams.Add(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL, ""));
		lDataParams.Add("");
		sbRecords.Append(CreateRecordCsv(lDataParams));
		lDataParams.Clear();
		lDataParams.Add(
			// 「潜在ユーザ」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_POTENTIAL_USER));
		lDataParams.Add(
			// 「アクティブユーザ」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_ACTIVE_USER));
		lDataParams.Add(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE, ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL)));
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE)));
		lDataParams.Add(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE, ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ACTIVE), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL)));
		sbRecords.Append(CreateRecordCsv(lDataParams));
		lDataParams.Clear();
		lDataParams.Add(
			// 「潜在ユーザ」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_POTENTIAL_USER));
		lDataParams.Add(
			// 「休眠ユーザ」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_DORMANT_USER));
		lDataParams.Add(DispNumeric(m_drvCurrentData, "potential_unactive_total", ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvCurrentData, "potential_unactive_total"), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL)));
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, "potential_unactive_total"), DispData(m_drvTargetData, "potential_unactive_total")));
		lDataParams.Add(DispNumeric(m_drvTargetData, "potential_unactive_total", ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvTargetData, "potential_unactive_total"), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL)));
		sbRecords.Append(CreateRecordCsv(lDataParams));
		lDataParams.Clear();
		lDataParams.Add(
			// 「潜在ユーザ」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_POTENTIAL_USER));
		lDataParams.Add(
			// 「休暇ユーザ：休眠レベル１」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_VACATION_USER1));
		lDataParams.Add(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE1, ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE1), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL)));
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE1), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE1)));
		lDataParams.Add(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE1, ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE1), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL)));
		sbRecords.Append(CreateRecordCsv(lDataParams));
		lDataParams.Clear();
		lDataParams.Add(
			// 「潜在ユーザ」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_POTENTIAL_USER));
		lDataParams.Add(
			// 「休暇ユーザ：休眠レベル２」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_VACATION_USER2));
		lDataParams.Add(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE2, ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE2), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL)));
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE2), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE2)));
		lDataParams.Add(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE2, ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_UNACTIVE2), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_POTENTIAL_ALL)));
		sbRecords.Append(CreateRecordCsv(lDataParams));
		// 認知顧客：新規獲得数
		lDataParams.Clear();
		lDataParams.Add(
			// 「認知顧客」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_CUSTOMER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_COGNITIVE_CUSTOMER));
		lDataParams.Add(
			// 「新規獲得」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_NUMBER_NEW_ACQUISITION));
		lDataParams.Add(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_NEW + "_total", ""));
		lDataParams.Add("");
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_NEW + "_total"), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_NEW + "_total")));
		lDataParams.Add(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_NEW + "_total", ""));
		lDataParams.Add("");
		sbRecords.Append(CreateRecordCsv(lDataParams));
		// 認知顧客
		lDataParams.Clear();
		lDataParams.Add(
			// 「認知顧客」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_CUSTOMER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_COGNITIVE_CUSTOMER));
		lDataParams.Add(
			// 「全体数」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_TOTAL_NUMBER));
		lDataParams.Add(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL, ""));
		lDataParams.Add("");
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL)));
		lDataParams.Add(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL, ""));
		lDataParams.Add("");
		sbRecords.Append(CreateRecordCsv(lDataParams));
		lDataParams.Clear();
		lDataParams.Add(
			// 「認知顧客」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_CUSTOMER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_COGNITIVE_CUSTOMER));
		lDataParams.Add(
			// 「アクティブユーザ」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_ACTIVE_USER));
		lDataParams.Add(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE, ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL)));
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE)));
		lDataParams.Add(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE, ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ACTIVE), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL)));
		sbRecords.Append(CreateRecordCsv(lDataParams));
		lDataParams.Clear();
		lDataParams.Add(
			// 「認知顧客」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_CUSTOMER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_COGNITIVE_CUSTOMER));
		lDataParams.Add(
			// 「休眠ユーザ」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_DORMANT_USER));
		lDataParams.Add(DispNumeric(m_drvCurrentData, "recognize_unactive_total", ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvCurrentData, "recognize_unactive_total"), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL)));
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, "recognize_unactive_total"), DispData(m_drvTargetData, "recognize_unactive_total")));
		lDataParams.Add(DispNumeric(m_drvTargetData, "recognize_unactive_total", ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvTargetData, "recognize_unactive_total"), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL)));
		sbRecords.Append(CreateRecordCsv(lDataParams));
		lDataParams.Clear();
		lDataParams.Add(
			// 「認知顧客」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_CUSTOMER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_COGNITIVE_CUSTOMER));
		lDataParams.Add(
			// 「休暇ユーザ：休眠レベル１」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_VACATION_USER1));
		lDataParams.Add(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE1, ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE1), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL)));
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE1), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE1)));
		lDataParams.Add(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE1, ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE1), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL)));
		sbRecords.Append(CreateRecordCsv(lDataParams));
		lDataParams.Clear();
		lDataParams.Add(
			// 「認知顧客」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_CUSTOMER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_COGNITIVE_CUSTOMER));
		lDataParams.Add(
			// 「休暇ユーザ：休眠レベル２」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_USER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_VACATION_USER2));
		lDataParams.Add(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE2, ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE2), DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL)));
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE2), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE2)));
		lDataParams.Add(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE2, ""));
		lDataParams.Add(DispRateCsv(DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_UNACTIVE2), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_RECOGNIZE_ALL)));
		sbRecords.Append(CreateRecordCsv(lDataParams));
		// 退会顧客：当月退会者数 
		lDataParams.Clear();
		lDataParams.Add(
			// 「退会顧客」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_CUSTOMER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_WITHDRAWAL_CUSTOMER));
		lDataParams.Add(
			// 「当月退会者数」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_NUMBER_WITHDRAWAL_THIS_MONTH));
		lDataParams.Add(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_NEW + "_total", ""));
		lDataParams.Add("");
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_NEW + "_total"), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_NEW + "_total")));
		lDataParams.Add(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_NEW + "_total", ""));
		lDataParams.Add("");
		sbRecords.Append(CreateRecordCsv(lDataParams));
		// 退会顧客
		lDataParams.Clear();
		lDataParams.Add(
			// 「退会顧客」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_CUSTOMER,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_WITHDRAWAL_CUSTOMER));
		lDataParams.Add(
			// 「退会者数」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_USER_CONDITION_REPORT_LIST_NUMBER_WITHDRAWAL));
		lDataParams.Add(DispNumeric(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_ALL, ""));
		lDataParams.Add("");
		lDataParams.Add(DispIncreasingRate(DispData(m_drvCurrentData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_ALL), DispData(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_ALL)));
		lDataParams.Add(DispNumeric(m_drvTargetData, Constants.FIELD_DISPUSERANALYSIS_LEAVE_ALL, ""));
		lDataParams.Add("");
		sbRecords.Append(CreateRecordCsv(lDataParams));

		//------------------------------------------------------
		// ファイル出力
		//------------------------------------------------------
		OutPutFileCsv(sbFileName.ToString(), sbRecords.ToString());
	}
}
