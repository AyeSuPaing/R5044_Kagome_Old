/*
=========================================================================================================
  Module      : 共通ヘッダ出力コントローラ処理(BodyHeaderMain.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Amazon;
using w2.App.Common.Product;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;

public partial class Form_Common_BodyHeaderMain : ProductUserControl
{
	#region ラップ済コントロール宣言
	WrappedDropDownList WddlCategories { get { return GetWrappedControl<WrappedDropDownList>("ddlCategories"); } }
	protected WrappedTextBox WtbSearchWord { get { return GetWrappedControl<WrappedTextBox>("tbSearchWord"); } }
	protected WrappedButton WlbSearch { get { return GetWrappedControl<WrappedButton>("lbSearch"); } }
	# endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			if (this.IsOrderPage == false)
			{
				//------------------------------------------------------
				// ドロップダウンを動的に生成
				//------------------------------------------------------
				// ルートカテゴリ一覧をＤＢから取得
				List<ProductCategoryTreeNode> rootCategoryNodes = GetRootCategoryNodes(this.ShopId);

				// ドロップダウン作成
				this.WddlCategories.AddItems(GetProductCategoryListForDropDownList(rootCategoryNodes));

				//------------------------------------------------------
				// 検索文字セット
				//------------------------------------------------------
				this.WtbSearchWord.Text = this.SearchWord;
			}
		}
	}

	/// <summary>
	/// 画面リロード処理（HTML描画時に実行される）
	/// </summary>
	protected void Reload()
	{
		var cartObjectList = GetCartObjectList();
		if (cartObjectList.Items.Count != 0)
		{
			this.ProductPriceSubtotal = cartObjectList.ItemTotalPrice;
			this.ProductCount = cartObjectList.ItemTotalCount;
		}
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearch_Click(object sender, EventArgs e)
	{
		string url = ProductPage.CreateProductListUrl(
		this.ShopId, this.WddlCategories.SelectedValue,
		StringUtility.StrTrim(this.WtbSearchWord.Text.Trim(), Constants.CONST_PRODUCT_SEARCH_WORD_MAX_LENGTH),
		"",	// ワード検索はグループ設定をクリア
		this.CampaignIcon,
		"",	// ワード検索は価格をクリア
		"",	// ワード検索は価格をクリア
		this.SortKbn,
		this.BrandId,
		this.DispImageKbn,
		this.DisplayOnlySpPrice,
		(this.WddlCategories.SelectedValue != "") ? this.WddlCategories.SelectedItem.Text : "",
		this.BrandName,
		this.UndisplayNostock,
		this.FixedPurchaseFilter,
		this.DisplayCount,
		this.SaleFilter);

		if (Constants.W2MP_PRODUCT_SEARCHWORD_RANKING_ENABLED)
		{
			Session[Constants.SESSION_KEY_DO_REGISTER_PRODUCT_SEARCHWORD] = true;
		}
		Response.Redirect(url);
	}

	/// <summary>
	/// 新規会員登録クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUserRegist_Click(object sender, EventArgs e)
	{
		LoginCooperationSessionRemove();
		var url =  new UrlCreator(
			this.SecurePageProtocolAndHost 
			+ Constants.PATH_ROOT
			+ Constants.PAGE_FRONT_USER_REGIST_REGULATION).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// かんたん会員登録クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUserEasyRegist_Click(object sender, EventArgs e)
	{
		LoginCooperationSessionRemove();
		var url = new UrlCreator(
			this.SecurePageProtocolAndHost
			+ Constants.PATH_ROOT
			+ Constants.PAGE_FRONT_USER_EASY_REGIST_INPUT).CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// ログイン連携セッションの削除
	/// </summary>
	private void LoginCooperationSessionRemove()
	{
		if (this.IsAmazonLoggedIn && Constants.AMAZON_LOGIN_OPTION_ENABLED)
		{
			Session.Remove(AmazonConstants.SESSION_KEY_AMAZON_MODEL);
		}
		else if (SessionManager.PayPalLoginResult != null)
		{
			Session.Remove(Constants.SESSION_KEY_PAYPAL_LOGIN_RESULT);
			Session.Remove(Constants.SESSION_KEY_PAYPAL_COOPERATION_INFO);
		}
	}

	/// <summary>
	/// デフォルトのブランドID取得
	/// </summary>
	/// <returns>デフォルトのブランドIDまたはnull</returns>
	private string GetDefaultBrandId()
	{
		var defaultBrand = ProductBrandUtility.GetDefaultBrand();
		var result = ((defaultBrand != null) && (defaultBrand.Count > 0))
			? (string)defaultBrand[0][Constants.FIELD_PRODUCTBRAND_BRAND_ID]
			: null;
		return result;
	}

	/// <summary>
	/// 最初のブランドID取得
	/// </summary>
	/// <returns>最初のブランドIDまたはnull</returns>
	private string GetFirstBrandId()
	{
		var brands = ProductBrandUtility.GetProductBrandList();
		var result = ((brands != null) && (brands.Count > 0))
			? (string)brands[0][Constants.FIELD_PRODUCTBRAND_BRAND_ID]
			: null;
		return result;
	}

	/// <summary>遷移後URL</summary>
	protected string NextUrl
	{
		get
		{
			// 注文完了画面だったら、次は強制的にTOPへ飛ばす
			if (Request.Url.AbsolutePath == (Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_COMPLETE))
			{
				return Constants.PATH_ROOT;
			}

			// 要求ページがログイン画面ではない場合、要求ページのURI絶対パスを遷移後URLとして返却
			if (Request.Url.AbsolutePath != (Constants.PATH_ROOT + Constants.PAGE_FRONT_LOGIN))
			{
				return this.RawUrl;
			}

			// 既に遷移後URLが存在する場合、存在する遷移後URLを返却　そうでない場合、TOPページを遷移後URLとして返却
			return (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NEXT_URL]) != "") ? NextUrlValidation(Request[Constants.REQUEST_KEY_NEXT_URL]) : Constants.PATH_ROOT; 
		}
	}
	/// <summary>カート内小計金額</summary>
	protected decimal ProductPriceSubtotal
	{
		get { return (ViewState["ProductPriceSubtotal"] != null) ? (decimal)ViewState["ProductPriceSubtotal"] : 0; }
		private set { ViewState["ProductPriceSubtotal"] = value; }
	}
	/// <summary>カート内商品数</summary>
	protected int ProductCount
	{
		get { return (ViewState["ProductCount"] != null) ? (int)ViewState["ProductCount"] : 0; }
		private set { ViewState["ProductCount"] = value; }
	}
	/// <summary>期間限定ポイントが使用可能か</summary>
	protected bool IsLimitedTermPointUsable
	{
		get { return Constants.CROSS_POINT_OPTION_ENABLED == false; }
	}
}
