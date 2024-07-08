/*
=========================================================================================================
  Module      : 商品のカラー検索ユーザコントロール(ProductColorSearchBoxUserControl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using w2.App.Common.DataCacheController;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.App.Common.Web.WrappedContols;

/// <summary>
/// 商品のカラー検索ユーザコントロール
/// </summary>
public partial class Form_Common_Product_ProductColorSearchBox : ProductUserControl
{
	#region Wrapped Control
	WrappedRepeater WrColorSearch { get { return GetWrappedControl<WrappedRepeater>("rColorSearch"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// 検索カラー一覧初期化
			this.WrColorSearch.DataSource = DataCacheControllerFacade.GetProductColorCacheController().GetProductColorList();
			// 全画面のデータバインド
			DataBind();
		}
	}

	/// <summary>
	/// カラー＆サイズで検索するメソッド
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearch_Click(object sender, EventArgs e)
	{
		// パラメータ取得
		GetParameters();

		// カラー検索パラメータ設定
		this.RequestParameter[Constants.REQUEST_KEY_PRODUCT_COLOR_ID] = ((LinkButton)sender).CommandArgument;

		// パラメータ取得
		var urlParameter = new Dictionary<string, string>(this.RequestParameter);

		// ページNoパラメータ削除
		urlParameter.Remove(Constants.REQUEST_KEY_PAGE_NO);
		urlParameter.Add(ProductCommon.URL_KEY_CATEGORY_NAME, "");

		// 商品一覧のURLを生成する
		var urlProductList = ProductCommon.CreateProductListUrl(urlParameter);

		Response.Redirect(urlProductList);
	}

	/// <summary>表示するか</summary>
	public bool IsVisible
	{
		get
		{
			return (this.WrColorSearch.DataSource != null)
				&& (((ProductColor[])this.WrColorSearch.DataSource).Length > 0);
		}
	}
}