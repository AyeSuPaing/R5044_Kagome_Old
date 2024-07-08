/*
=========================================================================================================
  Module      : 外部レコメンド連携カテゴリレコメンドトユーザコントロール処理(BodyCategoryRecommendByRecommendEngine.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using w2.App.Common.Option;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;

public class CategoryRecommendByRecommendEngineUserControl : ProductUserControl
{
	#region ラップ済コントロール宣言
	WrappedRepeater WrRecommendCategories { get { return GetWrappedControl<WrappedRepeater>("rRecommendCategories"); } }
	WrappedRepeater WrRecommendCategoryBreadcrumbs { get { return GetWrappedControl<WrappedRepeater>("rRecommendCategoryBreadcrumbs"); } }
	WrappedHtmlGenericControl WdivRecommendTitle { get { return GetWrappedControl<WrappedHtmlGenericControl>("divRecommendTitle"); } }
	WrappedHtmlGenericControl WdivAlternativeRecommend { get { return GetWrappedControl<WrappedHtmlGenericControl>("divAlternativeRecommend"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 非同期表示の場合、処理を抜ける
		//------------------------------------------------------
		if (this.IsAsync)
		{
			return;
		}

		//------------------------------------------------------
		// 非同期処理
		//------------------------------------------------------
		// 外部レコメンドを使用しない場合
		// 代替レコメンド表示
		this.WdivRecommendTitle.Visible = false;
		this.WdivAlternativeRecommend.Visible = true;
	}

	#region レコナイズ削除予定記述
	/// <summary>
	/// トラッキングログ送信ページURL作成
	/// </summary>
	/// <param name="objRecommendCategory">レコメンドカテゴリ</param>
	/// <returns>トラッキングログ送信ページURL</returns>
	protected string CreateSendTrackingLogUrl(object objRecommendCategory)
	{
		return CreateSendTrackingLogUrl(objRecommendCategory, Constants.RecommendKbn.CategoryRecommend);
	}

	/// <summary>表示区分（外部から設定可能）</summary>
	public string DispKbn { get; set; }
	/// <summary>レコメンドコード（外部から設定可能）</summary>
	public string RecommendCode { get; set; }
	/// <summary>アイテムコード（外部から設定可能）</summary>
	public string ItemCode
	{
		get { return (string)ViewState["ItemCode"]; }
		set
		{
			ViewState["ItemCode"] = value;
		}
	}
	/// <summary>商品最大表示数（外部から設定可能）</summary>
	public int MaxDispCount { get; set; }
	#endregion
}