/*
=========================================================================================================
  Module      : 特集ページカテゴリ一覧出力コントローラ処理(BodyFeaturePageCategoryLinks.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.FeaturePageCategory;
using w2.App.Common.Web.WrappedContols;

public partial class Form_Common_FeaturePage_BodyFeaturePageCategoryLinks : FeaturePageUserControl
{
	#region ラップ済コントロール宣言
	WrappedRepeater WrFeaturePageCategoryList { get { return GetWrappedControl<WrappedRepeater>("rFeaturePageCategoryList"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (Constants.FEATUREPAGESETTING_OPTION_ENABLED == false) return;

		if (!this.IsPostBack)
		{
			this.FeaturePageCategoryData = new FeaturePageCategoryService().Get(this.FeaturePageParentCategoryId);

			this.WrFeaturePageCategoryList.DataSource = new FeaturePageCategoryService()
				.GetChildCategoriesByParentCategoryId(this.FeaturePageParentCategoryId);
			this.WrFeaturePageCategoryList.DataBind();
		}
	}
	/// <summary>特集ページカテゴリ情報</summary>
	protected FeaturePageCategoryModel FeaturePageCategoryData { get; set; }
	/// <summary>特集ページカテゴリ情報が存在するか？</summary>
	protected bool HasFeaturePageCategoryData { get { return (this.FeaturePageCategoryData != null); } }
}