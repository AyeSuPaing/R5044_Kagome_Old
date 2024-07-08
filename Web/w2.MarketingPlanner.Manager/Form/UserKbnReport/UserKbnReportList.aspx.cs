/*
=========================================================================================================
  Module      : ユーザー区分レポート一覧ページ処理(UserKbnReportList.aspx.cs)
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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public partial class Form_UserKbnReport_UserKbnReportList : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 顧客区分取得・設定
		//------------------------------------------------------
		Hashtable htInput = new Hashtable();
		if (!IsPostBack)
		{
			InitializeComponents();
			htInput.Add(Constants.FIELD_USER_USER_KBN, System.DBNull.Value);
			htInput.Add(Constants.FIELD_USER_MALL_ID, "");
		}
		else
		{
			// サイト
			htInput.Add(Constants.FIELD_USER_MALL_ID, ddlSiteName.SelectedValue);

			// 顧客区分：全て選択以外の場合
			if (rlUserKbn.SelectedValue != "")
			{
				htInput.Add(Constants.FIELD_USER_USER_KBN, rlUserKbn.SelectedValue);
			}
			else
			{
				htInput.Add(Constants.FIELD_USER_USER_KBN, System.DBNull.Value);
			}
		}

		//------------------------------------------------------
		// 顧客構成分析結果取得
		//------------------------------------------------------
		List<KbnAnalysisUtility.KbnAnalysisTable> lUserHead = KbnAnalysisUtility.GetKbnAnalysisData("UserKbnAnalysisHead", htInput);
		List<KbnAnalysisUtility.KbnAnalysisTable> lUser = KbnAnalysisUtility.GetKbnAnalysisData("UserKbnAnalysis", htInput);

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		rHeadResults.DataSource = lUserHead;
		rHeadResults.DataBind();

		rResults.DataSource = lUser;
		rResults.DataBind();

		if (!IsPostBack)
		{
			// 初期設定(全て)
			rlUserKbn.Items[0].Selected = true;
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 顧客区分選択ラジオボタン作成
		rlUserKbn.Items.Add(new ListItem("全体",""));
		rlUserKbn.Items.AddRange(
			ValueText.GetValueItemArray(Constants.TABLE_USER, Constants.VALUE_TEXT_KEY_USER_USER_KBN_ALL).Select(
				i =>
				{
					i.Attributes.Add(
						"title",
						ValueText.GetValueText(
							Constants.TABLE_USER,
							Constants.VALUE_TEXT_FIELD_USER_KBN_TOOLTIP,
							i.Value));
					return i;
				}).ToArray());
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_USER, Constants.FIELD_USER_USER_KBN))
		{
			// モバイルデータの表示と非表示OFF時はMB_USERとMB_GEST区分を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& ((li.Value == Constants.FLG_USER_USER_KBN_MOBILE_USER)
					|| (li.Value == Constants.FLG_USER_USER_KBN_MOBILE_GUEST))) continue;
			rlUserKbn.Items.Add( li );
		}

		// サイト
		DataView dvMallCooperationSetting = null;
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallLiaise", "GetMallCooperationSettingListFromShopId"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, this.LoginOperatorShopId);

			dvMallCooperationSetting = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
		}
		ddlSiteName.Items.Add(new ListItem("全体", ""));
		ddlSiteName.Items.AddRange(ValueText.GetValueItemArray("SiteName", "OwnSiteName"));
		foreach (DataRowView drvMallCooperationSetting in dvMallCooperationSetting)
		{
			ddlSiteName.Items.Add(
				new ListItem(
					CreateSiteNameForList(
						(string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID],
						(string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME]),
						(string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID]));
		}
	}

	/// <summary>
	/// サイト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlSiteName_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		// Page_Load()で処理を行っています。
	}

	/// <summary>
	/// 顧客区分選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rlUserKbn_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		// Page_Load()で処理を行っています。
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
			// 「ユーザー区分レポート　{0}」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_USER_KBN_REPORT),
			DateTimeUtility.ToStringForManager(
				DateTime.Now,
				DateTimeUtility.FormatType.LongDateHourMinute2Letter)));
		sbRecords.Append(CreateRecordCsv(lTitleParams));
		StringBuilder sbFileName = new StringBuilder();
		sbFileName.Append("UserKbnReportList_").Append(DateTime.Now.ToString("yyyyMMdd"));

		//------------------------------------------------------
		// データ作成
		//------------------------------------------------------
		// 顧客拡張項目
		foreach (RepeaterItem riHeadResults in rHeadResults.Items)
		{
			// サブタイトル作成
			List<string> lDataParams = new List<string>();
			sbRecords.Append(CreateRecordCsv(lDataParams));
			lDataParams.Add(((Literal)riHeadResults.FindControl("lUserHeaderTitle")).Text);
			sbRecords.Append(CreateRecordCsv(lDataParams));

			// サブヘッダ作成
			lDataParams.Clear();
			lDataParams.Add(
				// 「項目」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST,
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_ITEM));
			lDataParams.Add(
				// 「構成比」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST,
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_COMPOSITION_RATIO));
			lDataParams.Add(
				// 「人数」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST,
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_NUMBER_PEOPLE));
			sbRecords.Append(CreateRecordCsv(lDataParams));

			foreach (RepeaterItem riHeadResult in ((Repeater)riHeadResults.FindControl("HeadResult2")).Items)
			{
				lDataParams.Clear();
				lDataParams.Add(((Literal)riHeadResult.FindControl("lUserHeaderName")).Text);
				lDataParams.Add(((Literal)riHeadResult.FindControl("lUserHeaderRate")).Text);
				lDataParams.Add(((Literal)riHeadResult.FindControl("lUserHeaderCount")).Text);
				sbRecords.Append(CreateRecordCsv(lDataParams));
			}
		}
		// 顧客項目
		foreach (RepeaterItem riResults in rResults.Items)
		{
			// サブタイトル作成
			List<string> lDataParams = new List<string>();
			sbRecords.Append(CreateRecordCsv(lDataParams));
			lDataParams.Add(((ListItem)rlUserKbn.SelectedItem).Text + "：" + ((Literal)riResults.FindControl("lUserTitle")).Text);
			sbRecords.Append(CreateRecordCsv(lDataParams));

			// サブヘッダ作成
			lDataParams.Clear();
			lDataParams.Add(
				// 「項目」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST,
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_ITEM));
			lDataParams.Add(
				// 「構成比」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST,
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_COMPOSITION_RATIO));
			lDataParams.Add(
				// 「人数」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST,
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_USER_KBN_REPORT_LIST_NUMBER_PEOPLE));
			sbRecords.Append(CreateRecordCsv(lDataParams));

			foreach (RepeaterItem riResult2 in ((Repeater)riResults.FindControl("Result2")).Items)
			{
				lDataParams.Clear();
				lDataParams.Add(((Literal)riResult2.FindControl("lUserName")).Text);
				lDataParams.Add(((Literal)riResult2.FindControl("lUserRate")).Text);
				lDataParams.Add(((Literal)riResult2.FindControl("lUserCount")).Text);
				sbRecords.Append(CreateRecordCsv(lDataParams));
			}
		}

		//------------------------------------------------------
		// ファイル出力
		//------------------------------------------------------
		OutPutFileCsv(sbFileName.ToString(), sbRecords.ToString());
	}
}