/*
=========================================================================================================
  Module      : 購買区分レポート一覧ページ処理(OrderKbnReportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
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
using System.Xml;

public partial class Form_OrderKbnReport_OrderKbnReportList : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 購買構成分析結果取得
		//------------------------------------------------------
		List<KbnAnalysisUtility.KbnAnalysisTable> lKAT = KbnAnalysisUtility.GetKbnAnalysisData("OrderKbnAnalysis");

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		rResults.DataSource = lKAT;
		rResults.DataBind();
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
			"{0}　{1}",
			// 「購買区分レポート」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST,
				Constants.VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_PURCHASING_CATEGORY_REPORT),
			DateTimeUtility.ToStringForManager(
				DateTime.Now,
				DateTimeUtility.FormatType.LongDateHourMinute2Letter)));
		sbRecords.Append(CreateRecordCsv(lTitleParams));
		StringBuilder sbFileName = new StringBuilder();
		sbFileName.Append("OrderKbnReportList_").Append(DateTime.Now.ToString("yyyyMMdd"));

		//------------------------------------------------------
		// データ作成
		//------------------------------------------------------
		foreach (RepeaterItem riResults in rResults.Items)
		{
			// サブタイトル表示
			List<string> lDataParams = new List<string>();
			sbRecords.Append(CreateRecordCsv(lDataParams));
			lDataParams.Add(((Literal)riResults.FindControl("lTitle")).Text);
			sbRecords.Append(CreateRecordCsv(lDataParams));

			// サブヘッダ表示
			lDataParams.Clear();
			lDataParams.Add(
				// 「項目：」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_ITEM));

			lDataParams.Add(
				// 「構成比」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_COMPOSITION_RATIO));
			lDataParams.Add(
				// 「人数 Or 件数」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST,
					Constants.VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_NUMBER_KBN,
						(riResults.ItemIndex == 0)
							? Constants.VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_NUMBER_KBN_PEOPLE
							: Constants.VALUETEXT_PARAM_ORDER_KBN_REPORT_LIST_NUMBER_KBN_RESULT));
			sbRecords.Append(CreateRecordCsv(lDataParams));
			lDataParams.Clear();

			// データ表示
			foreach (RepeaterItem riResult2 in ((Repeater)riResults.FindControl("Result2")).Items)
			{
				lDataParams.Clear();
				lDataParams.Add(((Literal)riResult2.FindControl("lName")).Text);
				lDataParams.Add(((Literal)riResult2.FindControl("lRate")).Text);
				lDataParams.Add(((Literal)riResult2.FindControl("lCount")).Text);
				sbRecords.Append(CreateRecordCsv(lDataParams));
			}
		}

		//------------------------------------------------------
		// ファイル出力
		//------------------------------------------------------
		OutPutFileCsv(sbFileName.ToString(), sbRecords.ToString());
	}
}
