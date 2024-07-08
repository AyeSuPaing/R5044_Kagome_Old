/*
=========================================================================================================
  Module      : メッセージ集計ページ処理(MessageReport.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Cs.Reports;

public partial class Form_ReportMessage_MessageReport : ReportPageCs
{
	#region #Page_Load ページロード
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			Initialize();
		}
	}
	#endregion

	#region -Initialize 初期化
	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// デフォルトセット
		rbReportTypeOperator.Checked = true;
	}
	#endregion

	#region #btnSearch_Click 検索ボタンクリック
	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		DispReportList();
	}
	#endregion

	#region -DispReportList レポート一覧表示
	/// <summary>
	/// レポート一覧表示
	/// </summary>
	private void DispReportList()
	{
		var service = new MessageReportService(new MessageReportRepository());

		// 日付が空ならデフォルト値セット
		ucCsWorkflowDateTimePickerPeriod.SetDefaultDateIfEmpty();

		// 日付取得
		DateTime begin;
		DateTime end;
		ucCsWorkflowDateTimePickerPeriod.GetDateInput(out begin, out end);

		Hashtable param = GetDateSqlParamsForReport(begin, end);

		ReportMatrixRowModelForMessage[] list = null;
		ReportRowModel[] listTmp = null;
		if (rbReportTypeOperator.Checked)
		{
			listTmp = service.GetOperatorReport(param);
			list = ConvertReportRowToForMatrix(listTmp);
		}
		else if (rbReportTypeMonth.Checked)
		{
			var monthList = GetMonthList(begin, end);
			listTmp = service.GetMonthReport(monthList, param);
			list = ConvertReportRowToForMatrix(listTmp);
			list.ToList().ForEach(
				m => m.Name = m.Month + (string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) ? " 月" : ""));
		}
		else if (rbReportTypeMonthDay.Checked)
		{
			var monthDayList = GetMonthDayList(begin, end);
			listTmp = service.GetMonthDayReport(monthDayList, param);
			list = ConvertReportRowToForMatrix(listTmp);
			list.ToList().ForEach(
				m => m.Name = DateTimeUtility.ToStringForManager(
					m.MonthDay,
					DateTimeUtility.FormatType.LongDate1Letter));
		}
		else if (rbReportTypeWeekday.Checked)
		{
			var weekdayList = GetWeekdayList(begin, end);
			listTmp = service.GetWeekdayReport(weekdayList, param);
			list = ConvertReportRowToForMatrix(listTmp);
			list.ToList().ForEach(m => m.Name = ValueText.GetValueText(CS_REPORT, WEEK_DAY_LIST, m.Weekday.Value));
		}
		else if (rbReportTypeTime.Checked)
		{
			var hourList = Enumerable.Range(0, 24).ToArray();
			listTmp = service.GetTimeReport(hourList, param);
			list = ConvertReportRowToForMatrix(listTmp);
			list.ToList().ForEach(m => m.Name = m.Hour.Value + ":00 ～ " + (m.Hour.Value + 1) + ":00");
		}
		else if (rbReportTypeWeekdayTime.Checked)
		{
			var weekdayList = GetWeekdayList(begin, end);
			var hourList = Enumerable.Range(0, 24).ToArray();
			listTmp = service.GetWeekdayTimeReport(weekdayList, hourList, param);
			list = ConvertReportRowToForMatrix(listTmp);
			list.ToList().ForEach(m => {
				m.IsIndent = (m.Count.HasValue);
				m.Name = (m.Count.HasValue)
					? m.Hour.Value + ":00 ～ " + (m.Hour.Value + 1) + ":00"
					: ValueText.GetValueText(CS_REPORT, WEEK_DAY_LIST, m.Weekday.Value);
			});
		}
		rList.DataSource = list;
		rList.DataBind();

		tblResult.Visible = true;
	}
	#endregion

	#region #GetReportTypeString レポートタイプ文字列取得
	/// <summary>
	/// レポートタイプ文字列取得
	/// </summary>
	/// <returns>レポートタイプ文字列</returns>
	protected string GetReportTypeString()
	{
		if (rbReportTypeOperator.Checked) return rbReportTypeOperator.Text;
		if (rbReportTypeMonth.Checked) return rbReportTypeMonth.Text;
		if (rbReportTypeMonthDay.Checked) return rbReportTypeMonthDay.Text;
		if (rbReportTypeWeekday.Checked) return rbReportTypeWeekday.Text;
		if (rbReportTypeTime.Checked) return rbReportTypeTime.Text;
		if (rbReportTypeWeekdayTime.Checked) return rbReportTypeWeekdayTime.Text;
		return "";
	}
	#endregion

	#region -ConvertReportRowToForMatrix レポート行モデルをマトリクス用へ変換
	/// <summary>
	/// レポート行モデルをマトリクス用へ変換
	/// </summary>
	/// <param name="listTmp">レポート行モデルリスト</param>
	/// <returns>マトリクス行モデル</returns>
	private ReportMatrixRowModelForMessage[] ConvertReportRowToForMatrix(ReportRowModel[] listTmp)
	{
		var list = new List<ReportMatrixRowModelForMessage>();

		ReportMatrixRowModelForMessage row = null;
		string keyName = null;
		foreach (var rowTmp in listTmp)
		{
			if (keyName != rowTmp.Name)
			{
				if (row != null) list.Add(row);
				row = new ReportMatrixRowModelForMessage(rowTmp.DataSource);
				keyName = rowTmp.Name;
			}
			bool isReceive = (rowTmp.DirectionKbn == Constants.FLG_CSMESSAGE_DIRECTION_KBN_RECEIVE);
			switch (rowTmp.MediaKbn)
			{
				case Constants.FLG_CSMESSAGE_MEDIA_KBN_TEL:
					if (isReceive)
					{
						row.TelReceiveCount = rowTmp.Count;
					}
					else
					{
						row.TelSendCount = rowTmp.Count;
					}
					break;

				case Constants.FLG_CSMESSAGE_MEDIA_KBN_MAIL:
					if (isReceive)
					{
						row.MailReceiveCount = rowTmp.Count;
					}
					else
					{
						row.MailSendCount = rowTmp.Count;
					}
					break;

				case Constants.FLG_CSMESSAGE_MEDIA_KBN_OTHERS:
					if (isReceive)
					{
						row.OthersReceiveCount = rowTmp.Count;
					}
					else
					{
						row.OthersSendCount = rowTmp.Count;
					}
					break;

				default:
					// 一件も件数が無い場合はここに来る。表示を「0」表示するために0値格納
					if (row.Name != "") row.TelReceiveCount = 0;
					break;
			}
		}
		if (row != null) list.Add(row);

		return list.ToArray();
	}
	#endregion

	#region -GetDateSqlParamsForReport レポート用日付SQLパラメタ取得
	/// <summary>
	/// レポート用日付SQLパラメタ取得
	/// </summary>
	/// <param name="begin">開始日</param>
	/// <param name="end">終了日</param>
	/// <returns>レポート用日付SQLパラメタ</returns>
	private Hashtable GetDateSqlParamsForReport(DateTime begin, DateTime end)
	{
		Hashtable result = new Hashtable();
		var dateTimeEnd = DateTimeUtility.ToStringForManager(
			end.AddMilliseconds(998),
			DateTimeUtility.FormatType.LongFullDateTimeNoneServerTime);

		result.Add(Constants.FIELD_CSINCIDENT_DEPT_ID, this.LoginOperatorDeptId);
		result.Add(Constants.FIELD_CSMESSAGE_INQUIRY_REPLY_DATE + "_bgn", begin);
		result.Add(Constants.FIELD_CSMESSAGE_INQUIRY_REPLY_DATE + "_end", dateTimeEnd);

		return result;
	}
	#endregion
}