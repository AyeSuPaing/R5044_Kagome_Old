/*
=========================================================================================================
  Module      : コーディネートカテゴリリスト出力コントローラ処理(BodyCoordinateCategoryLinks.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Global.Translation;
using w2.Domain.CoordinateCategory;

public partial class Form_Common_Coordinate_BodyCoordinateCategoryLinks : CoordinateUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!this.IsPostBack)
		{
			// 親カテゴリリスト表示
			var categories = new CoordinateCategoryService().GetParentCategories(this.CoordinateCategoryId);

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// 翻訳情報設定
				categories = NameTranslationCommon.SetCoordinateCategoryTranslationData(categories);
			}

			// データバインド
			rCategoriesLink.DataSource = categories;
			rCategoriesLink.DataBind();
		}

	}
}