/*
=========================================================================================================
  Module      : 顧客区分、ユーザー拡張項目レポート一覧ページ処理(UserExtendReportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.Domain.User.Helper;

public partial class Form_UserKbnReport_UserExtendReportList : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 顧客区分取得・設定
		//------------------------------------------------------
		Hashtable input = new Hashtable();
		input[Constants.FIELD_USER_MALL_ID] = "";
		// TODO
		//input[Constants.FIELD_USERPROPERTYSETTING_DEPT_ID] = this.LoginOperatorDeptId;

		if (!IsPostBack)
		{
			InitializeComponents();
			input.Add(Constants.FIELD_USER_USER_KBN, DBNull.Value);
		}
		else
		{
			// 顧客区分：全て選択以外の場合
			if (rlUserKbn.SelectedValue != "")
			{
				input.Add(Constants.FIELD_USER_USER_KBN, rlUserKbn.SelectedValue);
			}
			else
			{
				input.Add(Constants.FIELD_USER_USER_KBN, DBNull.Value);
			}
		}

		//------------------------------------------------------
		// 顧客構成分析結果取得
		//------------------------------------------------------
		List<KbnAnalysisUtility.KbnAnalysisTable> lUserHead = KbnAnalysisUtility.GetKbnAnalysisData("UserKbnAnalysisHead", input);
		List<KbnAnalysisUtility.KbnAnalysisTable> lUserExtends = new List<KbnAnalysisUtility.KbnAnalysisTable>();

		//------------------------------------------------------
		// ユーザー拡張項目のレポート取得用
		//------------------------------------------------------
		var userExtendSettings =  new UserExtendSettingList(this.LoginOperatorId);
		// ユーザー拡張項目の項目数だけループする
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		{
			sqlAccessor.OpenConnection();
			// 入力方法がテキストボックスの場合は集計対象外
			foreach (var userExtendSetting in userExtendSettings.Items.Where(item => item.InputType != Constants.FLG_USEREXTENDSETTING_INPUT_TYPE_TEXT))
			{
				// 分析データ取得
				DataView analysis = null;
				using (SqlStatement sqlStatement = new SqlStatement(Constants.PHYSICALDIRPATH_KBN_ANALYSIS_XML, "UserExtendKbnAnalysis", "UserExtendKbnAnalysis"))
				{
					// ※ここで「要素が無い」ことは考慮していない(入力制御で処理できているから)
					// ------------------------------------------------------
					// Statementの置換文字列生成(集計結果のソート順を一定化)
					// ------------------------------------------------------
					// 現在有効な要素の区分値(value)を変数テーブルに追加していく (SQLServer2005も考慮してINSERT文を単純に連続して記述する)
					StringBuilder insertValues = new StringBuilder();
					foreach (KeyValuePair<string,string> item in userExtendSetting.GetKeyValuePair())
					{
						insertValues.Append("INSERT INTO @name_only (name) VALUES ").Append("('").Append(item.Key).Append("'); ");
					}

					// ------------------------------------------------------
					// Statementの置換文字列生成(有効な値のみを取得するため)
					// ------------------------------------------------------
					// 現在有効な要素の区分値(value)をカンマ区切りで並べる（これ以外は「上記以外」として算出される） ex. ABC,DEF,GHI
					StringBuilder validFields = new StringBuilder();
					userExtendSetting.GetKeyValuePair().ForEach(item => validFields.Append(validFields.Length == 0 ? "" : ",").Append("'").Append(item.Key).Append("'"));

					// ------------------------------------------------------
					// Statement置換
					// ------------------------------------------------------
					// 対象フィールドを指定する
					sqlStatement.Statement = sqlStatement.Statement.Replace("@@ target_user_extend_field @@", userExtendSetting.SettingId);
					// 集計結果のソート順を一定化するため一時テーブルにnameレコードをINSERTする処理で利用
					sqlStatement.Statement = sqlStatement.Statement.Replace("@@ insert_values @@", insertValues.ToString());
					// 有効な値のみを取得対象にする処理で利用
					sqlStatement.Statement = sqlStatement.Statement.Replace("@@ valid_fields @@", validFields.ToString());
					
					analysis = sqlStatement.SelectSingleStatement(sqlAccessor, input);
				}

				// 入力方式を表示する
				string inputTypeText = ValueText.GetValueText(Constants.TABLE_USEREXTENDSETTING, Constants.FIELD_USEREXTENDSETTING_INPUT_TYPE, userExtendSetting.InputType);

				// 集計件数が０件でも表示する（ただし中身はカウント０）
				// // 既存のユーザ分析データに追加
				KbnAnalysisUtility.KbnAnalysisTable kat = 
					KbnAnalysisUtility.CreateKbnAnalysisTable(
					    userExtendSetting.SettingName + (inputTypeText.Length >0 ? (" ("+inputTypeText+")") : ""), // 項目名と入力タイプを表示する
						"",
						(analysis.Count != 0 ? (double.Parse(analysis[0]["total"].ToString())) : 0), // DataViewが空なら０
						analysis,
						userExtendSetting.GetKeyValuePair()); // このListに沿って表示するフィールド名を書き換えます

				lUserExtends.Add(kat);
			}
		}

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		rHeadResults.DataSource = lUserHead;
		rHeadResults.DataBind();

		rResults.DataSource = lUserExtends;
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
		foreach (ListItem listItem in ValueText.GetValueItemList(Constants.TABLE_USEREXTEND, Constants.FIELD_USER_USER_KBN))
		{
			// モバイルデータの表示と非表示OFF時はモバイル会員を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& (listItem.Value == Constants.FLG_USER_USER_KBN_MOBILE_USER)) continue;
			rlUserKbn.Items.Add(listItem);
		}
	}

	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, EventArgs e)
	{
		StringBuilder records = new StringBuilder();

		//------------------------------------------------------
		// タイトル作成
		//------------------------------------------------------
		List<string> titleList = new List<string>();
		titleList.Add(string.Format(
			// 「ユーザー区分レポート　{0}」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST,
				Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_USER_KBN_REPORT),
			DateTimeUtility.ToStringForManager(
				DateTime.Now,
				DateTimeUtility.FormatType.LongDateHourMinute2Letter)));
		records.Append(CreateRecordCsv(titleList));

		//------------------------------------------------------
		// データ作成
		//------------------------------------------------------
		// 顧客拡張項目
		foreach (RepeaterItem riHeadResults in rHeadResults.Items)
		{
			// サブタイトル作成
			List<string> dataList = new List<string>();
			records.Append(CreateRecordCsv(dataList));
			dataList.Add(((Literal)riHeadResults.FindControl("lUserHeaderTitle")).Text);
			records.Append(CreateRecordCsv(dataList));

			// サブヘッダ作成
			dataList.Clear();
			dataList.Add(
				// 「項目」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST,
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_ITEM));
			dataList.Add(
				// 「構成比」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST,
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_COMPOSITION_RATIO));
			dataList.Add(
				// 「人数」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST,
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_NUMBER_PEOPLE));
			records.Append(CreateRecordCsv(dataList));

			foreach (RepeaterItem riHeadResult in ((Repeater)riHeadResults.FindControl("HeadResult2")).Items)
			{
				dataList.Clear();
				dataList.Add(((Literal)riHeadResult.FindControl("lUserHeaderName")).Text);
				dataList.Add(((Literal)riHeadResult.FindControl("lUserHeaderRate")).Text);
				dataList.Add(((Literal)riHeadResult.FindControl("lUserHeaderCount")).Text);
				records.Append(CreateRecordCsv(dataList));
			}
		}
		// 顧客項目
		foreach (RepeaterItem riResults in rResults.Items)
		{
			// サブタイトル作成
			List<string> dataList = new List<string>();
			records.Append(CreateRecordCsv(dataList));
			dataList.Add(((ListItem)rlUserKbn.SelectedItem).Text + "：" + ((Literal)riResults.FindControl("lUserTitle")).Text);
			records.Append(CreateRecordCsv(dataList));

			// サブヘッダ作成
			dataList.Clear();
			dataList.Add(
				// 「項目」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST,
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_ITEM));
			dataList.Add(
				// 「構成比」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST,
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_COMPOSITION_RATIO));
			dataList.Add(
				// 「人数」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST,
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_TITLE,
					Constants.VALUETEXT_PARAM_USER_EXTEND_REPORT_LIST_NUMBER_PEOPLE));
			records.Append(CreateRecordCsv(dataList));

			foreach (RepeaterItem riResult2 in ((Repeater)riResults.FindControl("Result2")).Items)
			{
				dataList.Clear();
				dataList.Add(((Literal)riResult2.FindControl("lUserName")).Text);
				dataList.Add(((Literal)riResult2.FindControl("lUserRate")).Text);
				dataList.Add(((Literal)riResult2.FindControl("lUserCount")).Text);
				records.Append(CreateRecordCsv(dataList));
			}
		}

		//------------------------------------------------------
		// ファイル出力
		//------------------------------------------------------
		OutPutFileCsv("UserKbnReportList_" + DateTime.Now.ToString("yyyyMMdd"), records.ToString());
	}
}