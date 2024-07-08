/*
=========================================================================================================
  Module      : ポイント最新レポート一覧ページ処理(PointReportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using w2.Common.Extensions;

public partial class Form_PointReport_PointReportList : BasePage
{
	protected const string POINT_KBN_ALL = "ALL";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			InitializeComponent(sender, e);
			LoadPointReport(sender, e);
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void InitializeComponent(object sender, EventArgs e)
	{
		// ポイント区分
		rblPointKbn.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_USERPOINT, Constants.FIELD_USERPOINT_POINT_KBN));
		rblPointKbn.SelectedValue = POINT_KBN_ALL;
	}

	/// <summary>
	/// ポイントレポート読み込み
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void LoadPointReport(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 各累計ポイント結果取得
		//------------------------------------------------------
		this.AmountPoint = GetAmountPointHashtable();

		//------------------------------------------------------
		// ポイント最新レポート結果取得
		//------------------------------------------------------
		List<KbnAnalysisUtility.KbnAnalysisTable> analysisResult = KbnAnalysisUtility.GetKbnAnalysisData(
			"PointAnalysis",
			new Hashtable
			{
				{ Constants.FIELD_USERPOINT_POINT_KBN, rblPointKbn.SelectedValue }
			});

		//------------------------------------------------------
		// データバインド
		//------------------------------------------------------
		rResults.DataSource = analysisResult;
		rResults.DataBind();
	}

	/// <summary>
	/// 各累計ポイント情報データビュー取得
	/// </summary>
	/// <returns>各累計ポイント情報データビュー</returns>
	private static DataView GetAmountPointDataView(Hashtable htInput)
	{
		// 変数宣言
		DataView dvResult;

		using(var sqlAccessor = new SqlAccessor())
		using(var sqlStatement = new SqlStatement("PointReport", "GetAmountPoint"))
		{
			// SQLステートメント実行
			dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		return dvResult;
	}

	/// <summary>
	/// 各累計ポイント情報ハッシュテーブル取得
	/// </summary>
	/// <returns>各累計ポイント情報ハッシュテーブル</returns>
	private Hashtable GetAmountPointHashtable()
	{
		var dv = GetAmountPointDataView(
			new Hashtable
			{
				{ Constants.FIELD_USERPOINT_POINT_KBN, rblPointKbn.SelectedValue }
			});

		var ht = dv[0].ToHashtable();
		return ht;
	}

	/// <summary>
	/// ポイント区分選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblPointKbn_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		LoadPointReport(sender, e);
	}

	/// <summary>ポイント数</summary>
	protected Hashtable AmountPoint
	{
		get { return (Hashtable)ViewState["amount_point"]; }
		set { ViewState["amount_point"] = value; }
	}
}