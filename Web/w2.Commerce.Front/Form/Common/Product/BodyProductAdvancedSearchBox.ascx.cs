/*
=========================================================================================================
  Module      : 商品詳細検索ボックス出力コントローラ処理(BodyProductAdvancedSearchBox.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ProductListDispSetting;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.App.Common.Web.WebCustomControl;
using w2.App.Common.Web.WrappedContols;

public partial class Form_Common_Product_BodyProductAdvancedSearchBox : ProductUserControl
{
	#region ラップ済みコントロール宣言
	protected WrappedTextBox WtbSearchWord { get { return GetWrappedControl<WrappedTextBox>("tbSearchWord"); } }
	protected WrappedDropDownList WddlCategories { get { return GetWrappedControl<WrappedDropDownList>("ddlCategories"); } }
	protected WrappedDropDownList WddlColors { get { return GetWrappedControl<WrappedDropDownList>("ddlColors"); } }
	protected WrappedTextBox WtbMinPrice { get { return GetWrappedControl<WrappedTextBox>("tbMinPrice"); } }
	protected WrappedTextBox WtbMaxPrice { get { return GetWrappedControl<WrappedTextBox>("tbMaxPrice"); } }
	protected WrappedCheckBox WcbStockOnly { get { return GetWrappedControl<WrappedCheckBox>("cbStockOnly"); } }
	protected WrappedHiddenField WhfBandId { get { return GetWrappedControl<WrappedHiddenField>("hfBrandId", string.Empty); } }
	protected WrappedRepeater WrBrandList { get { return GetWrappedControl<WrappedRepeater>("rBrandList"); } }
	protected WrappedTextBox WtbSubscriptionBoxSearchWord { get { return GetWrappedControl<WrappedTextBox>("tbSubscriptionBoxSearchWord"); } }
	# endregion

	// 用意済コントロール
	private List<string> ARRANGEMENT_CONTROL = new List<string> { "ddlCategories", "ddlColors", "tbSearchWord", "tbMinPrice", "tbMaxPrice", "cbStockOnly", "tbSubscriptionBoxSearchWord" };
	protected WrappedRepeater WrStockList { get { return GetWrappedControl<WrappedRepeater>("rStockList"); } }

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
			List<ProductCategoryTreeNode> rootCategoryNodes = GetRootCategoryNodes(this.ShopId);

			// ドロップダウン作成
			this.WddlCategories.AddItems(GetProductCategoryListForDropDownList(rootCategoryNodes));
			this.WddlColors.AddItems(GetColorListForDropDownList());

			var stockSettings = Constants.GLOBAL_OPTION_ENABLE
				? NameTranslationCommon.SetProductListDispSettingTranslationData(
					ProductListDispSettingUtility.StockSetting,
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId)
				: ProductListDispSettingUtility.StockSetting;

			this.WrStockList.DataSource = stockSettings;
			this.WrStockList.DataBind();

			//------------------------------------------------------
			// カスタムコントロールに初期値をセット
			//------------------------------------------------------
			foreach (Control ctl in dvProductAdvancedSearch.Controls)
			{
				if (ARRANGEMENT_CONTROL.Contains(ctl.ID))
				{
					continue;
				}
				string requestKey = "_" + ctl.ID;
				// テキストボックス
				if (ctl.GetType() == typeof(TextBox))
				{
					((TextBox)ctl).Text = (string)Request[requestKey];
				}
				// ドロップダウンリスト
				else if (ctl.GetType() == typeof(DropDownList))
				{
					((DropDownList)ctl).SelectedValue = Request[requestKey];
				}
				// ラジオボタンリスト
				else if (ctl.GetType() == typeof(RadioButtonList))
				{
					((RadioButtonList)ctl).SelectedValue = StringUtility.ToEmpty(Request[requestKey]);
				}
				// チェックボックスリスト
				else if (ctl.GetType() == typeof(CheckBoxList))
				{
					if (StringUtility.ToEmpty(Request[requestKey]) != "")
					{
						SetSearchCheckBoxValue((CheckBoxList)ctl, ((string)Request[requestKey]).Split(','));
					}
				}
				// テキストボックス（スマートフォン）
				else if (ctl.GetType() == typeof(ExtendedTextBox))
				{
					((ExtendedTextBox)ctl).Text = (string)Request[requestKey];
				}
			}
			//------------------------------------------------------
			// 用意済みコントロールに初期値をセット
			//------------------------------------------------------
			this.WtbSearchWord.Text = this.SearchWord;
			this.WtbSubscriptionBoxSearchWord.Text = this.SubscriptionBoxSearchWord;
			this.WtbMinPrice.Text = this.MinPrice;
			this.WtbMaxPrice.Text = this.MaxPrice;
			this.WcbStockOnly.Checked = (this.UndisplayNostock == Constants.KBN_PRODUCT_LIST_UNDISPLAY_NOSTOCK_PRODUCT_UNDISPLAY_NOSTOCK);

			// Set Brand Search Information
			SetBrandSearchInfo();

			DataBind();
		}
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearch_Click(object sender, EventArgs e)
	{
		// 検索コントロールの入力値を取得
		Dictionary<string, string> productListUrlParameter = new Dictionary<string, string>();
		// TODO:チェックボックスとラジオボタンも対応したい
		foreach (Control ctl in dvProductAdvancedSearch.Controls)
		{
			if (ARRANGEMENT_CONTROL.Contains(ctl.ID))
			{
				continue;
			}
			// テキストボックス
			if (ctl.GetType() == typeof(TextBox))
			{
				if (((TextBox)ctl).Text.Trim().Length != 0)
				{
					productListUrlParameter.Add("_" + ctl.ID, ((TextBox)ctl).Text);
				}
			}
			// ドロップダウンリスト
			else if (ctl.GetType() == typeof(DropDownList))
			{
				if (((DropDownList)ctl).SelectedValue.Length != 0)
				{
					productListUrlParameter.Add("_" + ctl.ID, ((DropDownList)ctl).SelectedValue);
				}
			}
			// ラジオボタンリスト
			else if (ctl.GetType() == typeof(RadioButtonList))
			{
				if (((RadioButtonList)ctl).SelectedValue.Length != 0)
				{
					productListUrlParameter.Add("_" + ctl.ID, ((RadioButtonList)ctl).SelectedValue);
				}
			}
			// チェックボックスリスト
			else if (ctl.GetType() == typeof(CheckBoxList))
			{
				string values = CreateSearchStringParts(((CheckBoxList)ctl).Items);
				if (values.Length != 0)
				{
					productListUrlParameter.Add("_" + ctl.ID, values);
				}
			}
			// テキストボックス（スマートフォン）
			else if (ctl.GetType() == typeof(ExtendedTextBox))
			{
				if (((ExtendedTextBox)ctl).Text.Trim().Length != 0)
				{
					productListUrlParameter.Add("_" + ctl.ID, ((ExtendedTextBox)ctl).Text);
				}
			}
		}

		// パラメーターセット
		productListUrlParameter.Add(Constants.REQUEST_KEY_SHOP_ID, this.ShopId);

		// For option search brand enabled
		if (this.HasControlAndOptionBrandEnabled)
		{
			productListUrlParameter.Add(Constants.REQUEST_KEY_BRAND_ID, this.WhfBandId.Value);
			productListUrlParameter.Add(Constants.REQUEST_KEY_BRAND_SEARCH_ALL, string.Empty);
		}
		else
		{
			productListUrlParameter.Add(Constants.REQUEST_KEY_BRAND_ID, this.BrandId);
		}
		productListUrlParameter.Add(Constants.REQUEST_KEY_PRODUCT_ID, this.ProductId);
		productListUrlParameter.Add(Constants.REQUEST_KEY_VARIATION_ID, this.VariationId);
		productListUrlParameter.Add(Constants.REQUEST_KEY_PRODUCT_GROUP_ID, this.ProductGroupId);
		productListUrlParameter.Add(Constants.REQUEST_KEY_SORT_KBN, this.SortKbn);
		productListUrlParameter.Add(Constants.REQUEST_KEY_CAMPAINGN_ICOM, this.CampaignIcon);
		productListUrlParameter.Add(Constants.REQUEST_KEY_DISP_IMG_KBN, this.DispImageKbn);
		productListUrlParameter.Add(Constants.REQUEST_KEY_DISP_PRODUCT_COUNT, this.DisplayCount.ToString());
		productListUrlParameter.Add(Constants.REQUEST_KEY_DISP_ONLY_SP_PRICE, this.DisplayOnlySpPrice);

		//------------------------------------------------------
		// 価格帯の入力値取得及び入力チェック
		//------------------------------------------------------
		Hashtable htInput = new Hashtable();
		// 旧デザイン用
		if (this.WtbMinPrice.HasInnerControl) htInput.Add("min_price", StringUtility.ToHankaku(this.WtbMinPrice.Text.Trim()));
		if (this.WtbMaxPrice.HasInnerControl) htInput.Add("max_price", StringUtility.ToHankaku(this.WtbMaxPrice.Text.Trim()));
		if (this.WcbStockOnly.HasInnerControl) htInput.Add("udns", GetStockDispKbn(this.WcbStockOnly));
		// 新デザイン用
		if (Request.Form["price"] != null)
		{
			var prices = Request.Form["price"].Split(',');
			htInput["min_price"] = StringUtility.ToHankaku(prices[0].Trim());
			htInput["max_price"] = StringUtility.ToHankaku(prices[1].Trim());
		}
		if (Request.Form["udns"] != null) htInput["udns"] = Request.Form["udns"];

		// 入力チェックエラーの場合はエラーページへ遷移する
		string errorMessages = Validator.Validate("SearchForPrice", htInput);
		if (errorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		productListUrlParameter.Add(Constants.REQUEST_KEY_CATEGORY_ID, this.WddlCategories.SelectedValue);
		productListUrlParameter.Add(Constants.REQUEST_KEY_SEARCH_WORD, StringUtility.StrTrim(this.WtbSearchWord.Text.Trim(), Constants.CONST_PRODUCT_SEARCH_WORD_MAX_LENGTH));
		productListUrlParameter.Add(Constants.REQUEST_KEY_MIN_PRICE, (string)htInput["min_price"]);
		productListUrlParameter.Add(Constants.REQUEST_KEY_MAX_PRICE, (string)htInput["max_price"]);
		productListUrlParameter.Add(Constants.REQUEST_KEY_PAGE_NO, "-1");
		productListUrlParameter.Add(Constants.REQUEST_KEY_UNDISPLAY_NOSTOCK_PRODUCT, (string)htInput["udns"]);
		productListUrlParameter.Add(Constants.REQUEST_KEY_FIXED_PURCHASE_FILTER, (Request.Form["fpfl"] ?? Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_ALL));
		productListUrlParameter.Add(Constants.REQUEST_KEY_PRODUCT_COLOR_ID, this.WddlColors.SelectedValue);
		productListUrlParameter.Add(Constants.REQUEST_KEY_PRODUCT_SALE_FILTER, (Request.Form[Constants.REQUEST_KEY_PRODUCT_SALE_FILTER] ?? Constants.KBN_PRODUCT_LIST_SALE_ALL));
		productListUrlParameter.Add(
			Constants.REQUEST_KEY_SUBSCRIPTION_BOX_SEARCH_WORD,
			StringUtility.StrTrim(
				this.WtbSubscriptionBoxSearchWord.Text.Trim(),
				Constants.CONST_PRODUCT_SEARCH_WORD_MAX_LENGTH));
		if (Constants.W2MP_PRODUCT_SEARCHWORD_RANKING_ENABLED)
		{
			Session[Constants.SESSION_KEY_DO_REGISTER_PRODUCT_SEARCHWORD] = true;
		}
		productListUrlParameter.Add(ProductCommon.URL_KEY_CATEGORY_NAME, (this.WddlCategories.SelectedValue != "") ? this.WddlCategories.SelectedItem.Text : "");
		productListUrlParameter.Add(ProductCommon.URL_KEY_BRAND_NAME, this.BrandName);
		// URL生成
		var urlString = string.IsNullOrEmpty(this.TargetUrl)
			? ProductCommon.CreateProductListUrl(productListUrlParameter)
			: ProductCommon.CreateProductListUrl(productListUrlParameter, this.TargetUrl);

		// 画面遷移
		Response.Redirect(urlString);
	}

	/// <summary>
	/// チェックボックスリストのチェック設定を行う
	/// </summary>
	/// <param name="targetList">対象チェックボックスリスト</param>
	/// <param name="values">値配列</param>
	protected void SetSearchCheckBoxValue(CheckBoxList targetList, string[] values)
	{
		foreach (string value in values)
		{
			ListItem li = targetList.Items.FindByValue(value);
			if (li != null) li.Selected = true;
		}
	}
	/// <summary>
	/// 検索条件設定入力抽出データ取得
	/// </summary>
	/// <param name="collectionList">コレクションリスト</param>
	/// <returns>複数選択条件</returns>
	protected string CreateSearchStringParts(ListItemCollection collectionList)
	{
		StringBuilder searchSettingString = new StringBuilder();
		foreach (ListItem li in collectionList)
		{
			if (li.Selected)
			{
				searchSettingString.Append(searchSettingString.Length != 0 ? "," : "").Append(li.Value);
			}
		}
		return searchSettingString.ToString();
	}

	/// <summary>
	/// コントロールの属性設定
	/// </summary>
	protected void SetAttributes()
	{
		this.WtbSearchWord.Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + lbSearch.UniqueID + "',''); return false;}";
		this.WtbSubscriptionBoxSearchWord.Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + lbSearch.UniqueID + "',''); return false;}";
		this.WtbMinPrice.Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + lbSearch.UniqueID + "',''); return false;}";
		this.WtbMaxPrice.Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + lbSearch.UniqueID + "',''); return false;}";
		foreach (Control ctl in dvProductAdvancedSearch.Controls)
		{
			if (ARRANGEMENT_CONTROL.Contains(ctl.ID))
			{
				continue;
			}
			if (ctl.GetType() == typeof(TextBox))
			{
				((TextBox)ctl).Attributes["onkeypress"] = "if (event.keyCode==13){__doPostBack('" + lbSearch.UniqueID + "',''); return false;}";
			}
		}
	}

	/// <summary>
	/// 価格検索コントロールのHTML属性作成（ValueとCheck）
	/// </summary>
	/// <param name="mustExchangePrice">基軸通貨への変換するか</param>
	/// <param name="minPrice">最小値</param>
	/// <param name="maxPrice">最大値</param>
	/// <returns>価格検索コントロールのHTML属性（ValueとCheck）</returns>
	protected string CreateAttributesForPriceSearchControl(bool mustExchangePrice, decimal? minPrice = null, decimal? maxPrice = null)
	{
		// 基軸通貨で変換が必要なら変換を行い、文字列形に変換する
		var minValue = (minPrice.HasValue == false)
			? string.Empty
			: (mustExchangePrice
				? CurrencyManager.ExchangePriceToKeyCurrency(minPrice.Value, this.CurrentCurrencyCode).ToString()
				: minPrice.ToString());
		var maxValue = (maxPrice.HasValue == false)
			? string.Empty
			: (mustExchangePrice
				? CurrencyManager.ExchangePriceToKeyCurrency(maxPrice.Value, this.CurrentCurrencyCode).ToString()
				: maxPrice.ToString());
		// 価格検索コントロールのHTML属性作成（ValueとCheck）
		var result = string.Format("value={0},{1} {2}",
			minValue,
			maxValue,
			((this.MinPrice == minValue) && (this.MaxPrice == maxValue)) ? "checked" : string.Empty);
		return result;
	}

	/// <summary>
	/// Set Brand Search Information
	/// </summary>
	private void SetBrandSearchInfo()
	{
		if (this.HasControlBrand == false) return;

		// Has brand in view state?
		if (this.HasBrands == null)
		{
			this.HasBrands = (ProductBrandUtility.GetProductBrandCount() > 0);
		}

		// For option brand enabled
		if (this.HasOptionBrandEnabled)
		{
			// Get data for Brand List
			this.WrBrandList.DataSource = ProductBrandUtility.GetProductBrandList();

			// Set value for Brand Id hidden field
			this.WhfBandId.Value = this.BrandId;
		}
	}

	/// <summary>Has Control Brand</summary>
	protected bool HasControlBrand
	{
		get
		{
			return (this.WrBrandList.HasInnerControl && this.WhfBandId.HasInnerControl);
		}
	}
	/// <summary>Has Option Brand Enabled</summary>
	protected bool HasOptionBrandEnabled
	{
		get
		{
			return (Constants.PRODUCT_BRAND_ENABLED && this.HasBrands.Value);
		}
	}
	/// <summary>Has Control And Option Brand Enabled</summary>
	protected bool HasControlAndOptionBrandEnabled
	{
		get
		{
			return (this.HasControlBrand && this.HasOptionBrandEnabled);
		}
	}
	/// <summary>遷移先Url</summary>
	public string TargetUrl
	{
		get { return (string)ViewState["TargetUrl"]; }
		set { ViewState["TargetUrl"] = value; }
	}
	/// <summary>定期商品追加画面時</summary>
	public bool IsFixedPurchaseProductAdd { get; set; }
	/// <summary>在庫選択肢を表示するか</summary>
	protected bool DisplayStockFilter
	{
		get
		{
			return (ProductListDispSettingUtility.StockSetting.Length > 1) && (this.IsFixedPurchaseProductAdd == false);
		}
	}
	/// <summary>通常・定期選択肢を表示するか</summary>
	protected bool DisplayFixedPurchaseFilter
	{
		get
		{
			return Constants.FIXEDPURCHASE_OPTION_ENABLED && (IsFixedPurchaseProductAdd == false);
		}
	}
	/// <summary>頒布会選択肢を表示するか</summary>
	protected bool DisplaySubscriptionBoxFilter
	{
		get
		{
			return Constants.FIXEDPURCHASE_OPTION_ENABLED && Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (this.IsFixedPurchaseProductAdd == false);
		}
	}
	/// <summary>ブランド選択肢を表示するか</summary>
	protected bool DisplayBrandFilter
	{
		get
		{
			return this.HasOptionBrandEnabled && (this.IsFixedPurchaseProductAdd == false);
		}
	}
}
