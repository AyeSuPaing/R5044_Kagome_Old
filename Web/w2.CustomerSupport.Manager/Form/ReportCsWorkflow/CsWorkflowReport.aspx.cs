/*
=========================================================================================================
  Module      : CS業務フロー集計ページ処理(CsWorkflowReport.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Cs.Reports;

public partial class Form_ReportCsWorkflow_CsWorkflowReport : ReportPageCs
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
		rblReportType.Items[0].Selected = true;
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
		var service = new CsWorkflowReportService(new CsWorkflowReportRepository());

		ucCsWorkflowDateTimePickerPeriod.SetDefaultDateIfEmpty();
		// 日付取得
		DateTime begin;
		DateTime end;
		ucCsWorkflowDateTimePickerPeriod.GetDateInput(out begin, out end);

		Hashtable param = GetDateSqlParamsForReport(begin, end);

		ReportMatrixRowModelForCsWorkflow[] list = null;
		switch (rblReportType.SelectedValue)
		{
			case "GroupOperator":
				list = service.GetOperatorReport(this.LoginOperatorDeptId, param);
				break;
		}
		rList.DataSource = list;
		rList.DataBind();

		tblResult.Visible = true;
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