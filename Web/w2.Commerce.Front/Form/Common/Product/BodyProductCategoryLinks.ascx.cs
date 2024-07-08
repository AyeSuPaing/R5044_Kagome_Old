/*
=========================================================================================================
  Module      : 商品カテゴリリスト出力コントローラ処理(BodyProductCategoryLinks.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.Linq;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.Domain.ProductCategory;

public partial class Form_Common_Product_BodyProductCategoryLinks : ProductUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 親カテゴリリスト表示
			//------------------------------------------------------
			// 親カテゴリリストデータビュー取得（親カテゴリから順に格納されているDataView取得）
			// （プロダクトIDが格納されている場合(=商品詳細表示)、その商品の商品のカテゴリIDを元にリスト表示する）
			DataView categories = null;
			if (this.ProductId != "")
			{
				categories = GetParentCategoriesFromProductId(this.ShopId, this.ProductId, this.UserFixedPurchaseMemberFlg);
			}
			else
			{
				categories = ProductCommon.GetParentAndCurrentCategories(this.ShopId, this.CategoryId, this.UserFixedPurchaseMemberFlg);
			}

			// 翻訳情報設定
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				categories = SetProductCategoryTranslationData(categories);
			}

			// データバインド
			rCategoriesLink.DataSource = categories;
			rCategoriesLink.DataBind();
		}

	}

	/// <summary>
	/// 商品カテゴリ翻訳情報
	/// </summary>
	/// <param name="categoryData">商品カテゴリ情報</param>
	/// <returns>翻訳後商品カテゴリ情報</returns>
	private DataView SetProductCategoryTranslationData(DataView categoryData)
	{
		var categories = categoryData.Cast<DataRowView>().Select(
			drv => new ProductCategoryModel
			{
				CategoryId = (string)drv[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID]
			}).ToArray();
		categoryData = (DataView)NameTranslationCommon.Translate(categoryData, categories);
		return categoryData;
	}
}