/*
=========================================================================================================
  Module      : 定期区分レポート一覧ページ処理(FixedPurchaseKbnReportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

public partial class Form_FixedPurchaseKbnReport_FixedPurchaseKbnReportList : BasePage
{
	protected const string REGULAR_ALL = "all";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		// 定期購入ステータス取得・設定
		var htInput = GetAndSetFixedPurchaseStatus();

		// 定期分析結果取得
		var analysisHeadResult = KbnAnalysisUtility.GetKbnAnalysisData("FixedPurchaseKbnAnalysisHead", htInput);
		var analysisResult = KbnAnalysisUtility.GetKbnAnalysisData("FixedPurchaseKbnAnalysis", htInput);

		// データバインド
		rHeadResults.DataSource = analysisHeadResult;
		rHeadResults.DataBind();
		rResults.DataSource = analysisResult;
		rResults.DataBind();
	}

	/// <summary>
	/// 定期購入ステータス取得・設定
	/// </summary>
	/// <returns>定期購入ステータス</returns>
	private Hashtable GetAndSetFixedPurchaseStatus()
	{
		var htInput = new Hashtable();
		if (!IsPostBack)
		{
			InitializeComponents();
			htInput.Add(Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, System.DBNull.Value);
			htInput.Add(Constants.FIELD_FIXEDPURCHASE_REGULAR_STATUS, REGULAR_ALL);

			// 初期設定(全て)
			rlFixedPurchaseStatus.Items[0].Selected = true;
			rlRegularType.Items[0].Selected = true;
		}
		else
		{
			if (string.IsNullOrEmpty(rlFixedPurchaseStatus.SelectedValue) == false)
			{
				htInput.Add(Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, rlFixedPurchaseStatus.SelectedValue);
			}
			else
			{
				htInput.Add(Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS, System.DBNull.Value);
			}

			if (string.IsNullOrEmpty(rlRegularType.SelectedValue) == false)
			{
				if (rlRegularType.SelectedValue == "subscription_box")
				{

				}

				htInput.Add(Constants.FIELD_FIXEDPURCHASE_REGULAR_STATUS, rlRegularType.SelectedValue);
			}
			else
			{
				htInput.Add(Constants.FIELD_FIXEDPURCHASE_REGULAR_STATUS, REGULAR_ALL);
			}
		}

		return htInput;
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		rlRegularType.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_REGULAR_STATUS));
		// 定期購入ステータス選択ラジオボタン作成
		rlFixedPurchaseStatus.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_FIXEDPURCHASE,
				Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
					? Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_STATUS
					: Constants.VALUETEXT_PARAM_FIXED_PURCHASE_STATUS_INVALID_SUBSCRIPTION_BOX));
	}

	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, System.EventArgs e)
	{
		var records = new StringBuilder();
		var titleParams = new List<string>();
		var fileName = new StringBuilder();
		
		// タイトル作成
		titleParams.Add(string.Format(
			// 「定期購入区分レポート　{0}」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST_FIXED_PURCHASE_KBN_REPORT),
			DateTimeUtility.ToStringForManager(
				DateTime.Now,
				DateTimeUtility.FormatType.LongDateHourMinute2Letter)));
		records.Append(CreateRecordCsv(titleParams));
		fileName.Append("FixedPurchaseKbnReportList_").Append(DateTime.Now.ToString("yyyyMMdd"));

		// データ作成
		foreach (RepeaterItem riHeadResults in rHeadResults.Items)
		{
			records.Append(ExportDatas(riHeadResults));
		}

		foreach (RepeaterItem riResults in rResults.Items)
		{
			records.Append(ExportDatas(riResults));
		}

		// ファイル出力
		OutPutFileCsv(fileName.ToString(), records.ToString());
	}

	/// <summary>
	/// データ出力
	/// </summary>
	/// <param name="ri">出力結果</param>
	/// <returns>出力データ</returns>
	private StringBuilder ExportDatas(RepeaterItem ri)
	{
		// サブタイトル表示
		var dataParams = new List<string>();
		var records = new StringBuilder();
		records.Append(CreateRecordCsv(dataParams));
		dataParams.Add(((Literal)ri.FindControl("lTitle")).Text);
		records.Append(CreateRecordCsv(dataParams));

		// サブヘッダ表示
		dataParams.Clear();
		dataParams.Add(
			// 「項目」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST_ITEM));

		dataParams.Add(
			// 「構成比」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST_COMPOSITION_RATIO));

		dataParams.Add(
			// 「件数」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_FIXED_PURCHASE_KBN_REPORT_LIST_NUMBER));
		records.Append(CreateRecordCsv(dataParams));
		dataParams.Clear();

		// データ表示
		foreach (RepeaterItem riResult2 in ((Repeater)ri.FindControl("Result2")).Items)
		{
			dataParams.Clear();
			dataParams.Add(((Literal)riResult2.FindControl("lName")).Text);
			dataParams.Add(((Literal)riResult2.FindControl("lRate")).Text);
			dataParams.Add(((Literal)riResult2.FindControl("lCount")).Text);
			records.Append(CreateRecordCsv(dataParams));
		}

		return records;
	}
}
