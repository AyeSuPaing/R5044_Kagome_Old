/*
=========================================================================================================
  Module      : 商品検索ボックス出力コントローラ処理(BodyProductSearchBox.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using w2.App.Common.Option;
using w2.App.Common.Web.WrappedContols;

public partial class Form_Common_Product_BodyProductSearchBox : ProductUserControl
{
	#region ラップ済みコントロール宣言
	WrappedDropDownList WddlCategories { get { return GetWrappedControl<WrappedDropDownList>("ddlCategories"); } }
	protected WrappedTextBox WtbSearchWord { get { return GetWrappedControl<WrappedTextBox>("tbSearchWord"); } }
	protected WrappedTextBox WtbMinPrice { get { return GetWrappedControl<WrappedTextBox>("tbMinPrice"); } }
	protected WrappedTextBox WtbMaxPrice { get { return GetWrappedControl<WrappedTextBox>("tbMaxPrice"); } }
	protected WrappedCheckBox WcbStockOnly { get { return GetWrappedControl<WrappedCheckBox>("cbStockOnly"); } }
	# endregion	
	
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			//------------------------------------------------------
			// ドロップダウンを動的に生成
			//------------------------------------------------------
			// ルートカテゴリ一覧をＤＢから取得
			List<ProductCategoryTreeNode> rootCategoryNodeList = GetRootCategoryNodes(this.ShopId);

			// ドロップダウン作成
			this.WddlCategories.AddItems(GetProductCategoryListForDropDownList(rootCategoryNodeList));

			//------------------------------------------------------
			// 検索文字、最小価格、最大価格セット
			//------------------------------------------------------
			this.WtbSearchWord.Text = this.SearchWord;
			this.WtbMinPrice.Text = this.MinPrice;
			this.WtbMaxPrice.Text = this.MaxPrice;
			this.WcbStockOnly.Checked = (this.UndisplayNostock == Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_UNDISPLAY_NOSTOCK);
		}
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearch_Click(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 価格帯の入力値取得及び入力チェック
		//------------------------------------------------------
		Hashtable htInput = new Hashtable();
		htInput.Add("min_price", StringUtility.ToHankaku(WtbMinPrice.Text.Trim()));
		htInput.Add("max_price", StringUtility.ToHankaku(WtbMaxPrice.Text.Trim()));

		// 入力チェックエラーの場合はエラーページへ遷移する
		string errorMessages = Validator.Validate("SearchForPrice", htInput);
		if (errorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		string url = ProductPage.CreateProductListUrl(
			this.ShopId,
			this.WddlCategories.SelectedValue,
			StringUtility.StrTrim(this.WtbSearchWord.Text.Trim(), Constants.CONST_PRODUCT_SEARCH_WORD_MAX_LENGTH),
			"",	// ワード検索はグループ設定をクリア
			this.CampaignIcon,
			(string)htInput["min_price"],
			(string)htInput["max_price"],
			this.SortKbn,
			this.BrandId,
			this.DispImageKbn,
			"",
			(this.WddlCategories.SelectedValue != "") ? this.WddlCategories.SelectedItem.Text : "",
			this.BrandName,
			GetStockDispKbn(WcbStockOnly),
			this.FixedPurchaseFilter,
			this.DisplayCount,
			this.SaleFilter);
		if (Constants.W2MP_PRODUCT_SEARCHWORD_RANKING_ENABLED)
		{
			Session[Constants.SESSION_KEY_DO_REGISTER_PRODUCT_SEARCHWORD] = true;
		}
		Response.Redirect(url);
	}
}
