/*
=========================================================================================================
  Module      : 特集ページカテゴリパンくずリスト出力コントローラ処理(BodyFeaturePageCategoryBreadcrumbs.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using w2.Domain.FeaturePageCategory;
using w2.App.Common.Web.WrappedContols;

public partial class Form_Common_FeaturePage_BodyFeaturePageCategoryBreadcrumbs : FeaturePageUserControl
{
	#region ラップ済コントロール宣言
	WrappedRepeater WrFeaturePageCategoryBreadcrumbs { get { return GetWrappedControl<WrappedRepeater>("rFeaturePageCategoryBreadcrumbs"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (Constants.FEATUREPAGESETTING_OPTION_ENABLED == false) return;

		if (!this.IsPostBack)
		{
			this.WrFeaturePageCategoryBreadcrumbs.DataSource = new FeaturePageCategoryService().GetParentCategories(this.FeaturePageCategoryId);
			this.WrFeaturePageCategoryBreadcrumbs.DataBind();
		}
	}
}