/*
=========================================================================================================
  Module      : おすすめタグ出力コントローラ処理(BodyRecommendTagsRelatedCategory.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.Awoo;
using w2.App.Common.Awoo.ClassifyProductType;

public partial class Form_Common_Product_BodyRecommendTagsRelatedCategory : ProductUserControl
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// Awoo連携オプションOFF、もしくはカテゴリ指定がない場合何もしない
			if ((Constants.AWOO_OPTION_ENABLED == false)
				|| (string.IsNullOrEmpty(this.CategoryName))) return;

			var response = AwooApiFacade.GetTagsByClassifyProductType(new ClassifyProductTypeRequest()
			{
				ProductType = this.CategoryName
			});

			if (response == null) return;

			this.Tags = response.Result.Tags;
			this.DataBind();
		}
	}

	/// <summary>
	/// おすすめタグリンクURL取得
	/// </summary>
	/// <param name="tag">タグ</param>
	/// <returns>Variation price tax</returns>
	protected string CreateRecommendProductsUrl(Tags tag)
	{
		return Constants.PATH_ROOT
			+ Constants.PAGE_RECOMMEND_PRODUCTS_LIST
			+ "?"
			+ Constants.REQUEST_KEY_TAGS
			+ "="
			+ tag.Link;
	}

	/// <summary>カテゴリ名</summary>
	public string CategoryName { get; set; }
	/// <summary>おすすめタグリスト</summary>
	protected List<Tags> Tags { get; set; }
}
