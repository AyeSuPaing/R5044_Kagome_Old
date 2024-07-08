/*
=========================================================================================================
  Module      :定期追加商品選択画面 (FixedPurchaseProductAdd.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W２ Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Design;
using w2.App.Common.Global.Region;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Pdf.PdfCreater;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Extensions;
using w2.Domain.FixedPurchaseProductChangeSetting;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.SeoMetadatas;

public partial class Form_FixedPurchase_FixedPurchaseProductAdd : ProductListPage
{
	/// <summary>リピートプラスONEページ遷移必須判定</summary>
	public override bool RepeatPlusOneNeedsRedirect { get { return Constants.REPEATPLUSONE_OPTION_ENABLED; } }
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Http; } } // Httpアクセス
	/// <summary>エラーメッセージキー</summary>
	private const string ERROE_MESSAGE_KEY = "ErrorMessage";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// ログインチェック
		CheckLoggedIn();

		if (!IsPostBack)
		{
			// 定期情報詳細画面以外から来た場合はTOPページに遷移する
			if (SessionManager.IsRedirectFromFixedPurchaseDetail == false)
			{
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_DEFAULT);
			}

			// リクエストよりパラメタ取得（商品一覧共通処理）
			GetParameters();

			// ブランドチェック
			BrandCheck();

			// 商品一覧表示用データ取得
			var info = GetProductListInfosFromCacheOrDb();

			this.ProductMasterList = info.Products;
			this.ProductVariationMasterList = info.ProductVariations;
			this.ProductVariationList = info.ProductGroupedVariations;
			this.ProductCategoryList = info.ChildCategories;

			// 購入可否判定のエラー制御
			this.ProductMasterHashtableList = this.ProductMasterList.ToHashtableList();
			var errorCheckedVariation = GetVariationAddCartList(this.ProductVariationMasterList);

			// 選択済み追加商品の削除
			this.ProductVariationMasterListForDisplayError = errorCheckedVariation.Where(
				variation =>
					((string[])Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_VARIATION_IDS]).Contains(
						(string)variation[Constants.FIELD_PRODUCT_VARIATION_ID]) == false).ToList();

			// エラー表示用商品情報の紐づけ
			SetLinkingErrorDisplayProduct();

			// 定期商品重複購入制限エラー設定
			SetErrorMessageProductOrderLimitItem();

			// 商品の総計取得
			var iTotalProductCounts = (this.ProductMasterList.Count != 0) ? (int)this.ProductMasterList[0].Row["row_count"] : 0;

			this.IsUserFixedPurchaseAble = (CheckFixedPurchaseLimitedUserLevel(this.ShopId, this.ProductId));

			// 商品検索文字列保存処理
			if ((Constants.W2MP_PRODUCT_SEARCHWORD_RANKING_ENABLED) && (this.DoRegisterProductSearchWord))
			{
				ProductSearchWordRegister.Regist(
					this.ShopId,
					((Constants.SMARTPHONE_OPTION_ENABLED && SmartPhoneUtility.CheckSmartPhone(Request.UserAgent)) ? Constants.FLG_PRODUCTSEARCHWORDHISTORY_ACCESS_KBN_SMARTPHONE : Constants.FLG_PRODUCTSEARCHWORDHISTORY_ACCESS_KBN_PC),
					this.SearchWord,
					iTotalProductCounts);
			}

			Session[Constants.SESSION_KEY_DO_REGISTER_PRODUCT_SEARCHWORD] = null;

			// 商品が０個であれば情報表示
			if (this.ProductMasterList.Count == 0)
			{
				this.AlertMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
			}

			// ページャ作成（商品一覧処理で総件数を取得）
			Dictionary<string, string> nextUrlParameter = new Dictionary<string, string>(this.RequestParameter);
			// パラメーター削除
			nextUrlParameter.Remove(Constants.REQUEST_KEY_PAGE_NO);
			nextUrlParameter.Remove(ProductCommon.URL_KEY_CATEGORY_NAME);
			// パラメーターセット
			nextUrlParameter.Add(ProductCommon.URL_KEY_CATEGORY_NAME, this.CategoryName);
			var strNextUrl = ProductCommon.CreateProductListUrl(nextUrlParameter, Constants.PAGE_FRONT_FIXED_PURCHASE_PRODUCT_ADD);

			// パスを参照して、PCサイト、スマートフォンサイト用のページャを切り替えて出力する
			this.PagerHtml = WebPager.CreateProductListPager(iTotalProductCounts, this.PageNumber, strNextUrl.ToString(), this.DisplayCount);

			// お気に入りデータを画面に設定
			SetFavoriteDataForDisplay(this.ProductMasterList);

			this.PaginationTag = WebPager.CreatePaginationTag(iTotalProductCounts, this.PageNumber, strNextUrl, this.DisplayCount);

			// パラメータから商品タグを取得
			var productTags = GetTagSettingListForSeo(this.RequestParameter);
			this.ProductTag = (productTags != null) ? string.Join(" ", productTags.Cast<DataRowView>().Select(item => item["tag_name"]).ToList()) : string.Empty;

			// 投入済みのバリエーションID
			//this.SelectedProducts = (Dictionary<string, string>)Session[Constants
			//	.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_AND_VARIATION_IDS];

			// 定期変更後商品取得
			if (this.IsChangeProduct) this.AfterChangeItems = GetAfterChangeItems();

			// 画面全体でデータバインド
			this.DataBind();
		}

		// SEOメタデータ情報取得・設定(V5.13以前に構築された環境用)
		if (!this.IsPostBack && Constants.SEOTAG_IN_PRODUCTLIST_ENABLED && Constants.SEOTAGDISPSETTING_OPTION_ENABLED)
		{
			var utility = new SeoUtility(this.Request, this.Page, this.Session);
			this.ViewState["title"] = utility.SeoData.HtmlTitle;
			this.SeoDescription = utility.SeoData.MetadataDesc;
			this.SeoKeywords = utility.SeoData.MetadataKeywords;
			this.SeoComment = utility.SeoData.Comment;
			this.Header.DataBind();
			this.Title = (string)this.ViewState["title"];
		}
	}

	/// <summary>
	/// 商品一覧情報を取得（キャッシュorDBから）
	/// </summary>
	/// <returns>商品一覧情報</returns>
	private ProductListInfos GetProductListInfosFromCacheOrDb()
	{
		// キャッシュキー作成
		// ・現在の日付以外のパラメタをキャッシュキーとする
		// ・会員ランクもキーに追加する
		var cacheKey = "ProductList " + string.Join(",", this.RequestParameter.Keys
			.Where(key => key != "current_date")
			.Select(key => key + "=" + this.RequestParameter[key]).ToArray());
		cacheKey += "MemberRankId=" + this.MemberRankId;
		cacheKey += ",UserFixedPurchaseMemberFlg=" + this.UserFixedPurchaseMemberFlg;

		// キャッシュまたはDBから取得
		var expire = (this.SearchWord.Trim() == "") ? Constants.PRODUCTLIST_CACHE_EXPIRE_MINUTES : 0;
		var data = DataCacheManager.GetInstance().GetData(
			cacheKey,
			expire,
			CreateProductListInfosFromDb);
		return data;
	}

	/// <summary>
	/// DBから商品一覧情報作成
	/// </summary>
	/// <returns>商品一覧情報</returns>
	private ProductListInfos CreateProductListInfosFromDb()
	{
		// 投入済みの商品ID
		var productIds = string.Join("','", (string[])Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_IDS]);
		// 投入済みのバリエーションID
		var variationIds = string.Join("','", (string[])Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_VARIATION_IDS]);
		var products = GetProductsList(this.RequestParameter, productIds, variationIds);
		var productVariations = (Constants.PRODUCTLIST_VARIATION_DISPLAY_ENABLED) ? GetVariationList(products, this.MemberRankId) : new DataView();
		var groupedVariationList = (Constants.PRODUCTLIST_VARIATION_DISPLAY_ENABLED) ? GetGroupedVariationList(productVariations) : null;

		var productInfos = new ProductListInfos(
			products,
			productVariations,
			groupedVariationList,
			childCategories: null);

		// 翻訳情報＋言語コードとかを設定してキャッシュさせる
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			var productTranslation = GetProductTranslationData(products);
			var productVariationTranslation = GetProductVariationTranslationData(productVariations);

			productInfos.SetProductListTranslationData(
				productTranslation,
				productVariationTranslation);
		}
		return productInfos;
	}

	/// <summary>
	/// 許可されたある小カテゴリリスト取得
	/// </summary>
	/// <param name="memberRankId">会員ランクID</param>
	/// <returns>小カテゴリリスト</returns>
	private List<DataRowView> GetAllowedChildCategories(string memberRankId)
	{
		var childCategories = GetChildCategories();

		var result = childCategories.Cast<DataRowView>().Where(c =>
				(Constants.MEMBER_RANK_OPTION_ENABLED == false)
				|| ((string)c[Constants.FIELD_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG] == Constants.FLG_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG_VALID)
				|| MemberRankOptionUtility.CheckMemberRankPermission(memberRankId, (string)c[Constants.FIELD_PRODUCTCATEGORY_MEMBER_RANK_ID]))
			.ToList();
		return result;
	}

	/// <summary>
	/// 小カテゴリリスト取得
	/// </summary>
	/// <returns>小カテゴリリスト</returns>
	private DataView GetChildCategories()
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("Product", "GetChildCategories"))
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, this.ShopId},
				{Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID, this.CategoryId},
				{Constants.FIELD_PRODUCT_BRAND_ID1, this.BrandId}
			};
			var dv = statement.SelectSingleStatementWithOC(accessor, ht);
			return dv;
		}
	}

	/// <summary>
	/// 商品リピータコマンド
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void ProductMasterList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		// 商品一覧で選択された商品IDをプロパティにセット
		this.ProductId = e.CommandArgument.ToString();

		// カート投入処理
		switch (e.CommandName)
		{
			case "FavoriteAdd":
				base.AddToFavorite(this.ShopId, this.ProductId, this.CategoryId, this.VariationId);
				break;
		}
	}

	/// <summary>
	/// 商品一覧取得SQLパラメタ作成
	/// </summary>
	/// <returns>SQLパラメタ</returns>
	public override Hashtable CreateGetProductListSqlParam()
	{
		// 検索ワード分割（5ワードを超えた場合エラーページへ遷移）
		char[] delimiterChars = { ' ', '　' }; // 半角全角スペース
		var searchWords = this.SearchWord.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
		if (searchWords.Length > 5)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCTSEARCH_WORDS_NUMOVER);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// パラメタ作成
		var ht = new Hashtable();
		ht.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.ShopId);
		ht.Add("category_id_like_escaped", StringUtility.SqlLikeStringSharpEscape(this.CategoryId));
		for (int iLoop = 0; iLoop < 5; iLoop++)
		{
			ht.Add("search_word_like_escaped" + (iLoop + 1), (iLoop < searchWords.Length)
				? StringUtility.SqlLikeStringSharpEscape(searchWords[iLoop])
				: "");
		}
		ht.Add("subscription_box_search_word_like_escaped", this.SubscriptionBoxSearchWord);
		ht.Add("current_date", DateTime.Now);
		int sortKbn;
		if (int.TryParse(this.SortKbn, out sortKbn))
		{
			ht.Add("sort_kbn", sortKbn);
		}
		else
		{
			ht.Add("sort_kbn", Constants.KBN_SORT_PRODUCT_LIST_DEFAULT);
		}
		ht.Add("campaign_icon_no", this.CampaignIcon);
		ht.Add("brand_id", this.BrandId);
		ht.Add("bgn_row_num", this.DisplayCount * (this.PageNumber - 1) + 1);
		ht.Add("end_row_num", this.DisplayCount * this.PageNumber);
		decimal minPrice;
		ht.Add("min_price", decimal.TryParse(this.MinPrice, out minPrice) ? (object)minPrice : DBNull.Value);
		decimal maxPrice;
		ht.Add("max_price", decimal.TryParse(this.MaxPrice, out maxPrice) ? (object)maxPrice : DBNull.Value);
		ht.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, this.MemberRankId);
		// 定期会員フラグ
		ht.Add(Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG, this.UserFixedPurchaseMemberFlg);
		// 特別価格有無検索
		ht.Add("disp_only_sp_price", this.DisplayOnlySpPrice);
		// 在庫有無検索
		ht.Add("stock_existence_disp_kbn", this.UndisplayNostock);
		ht.Add("fixed_purchase_filter", Constants.KBN_PRODUCT_LIST_FIXED_PURCHASE_FILTER_FIXED_PURCHASE);
		ht.Add("subscription_box_filter", Constants.KBN_PRODUCT_LIST_SUBSCRIPTION_BOX_FILTER_NORMAL);
		// 商品グループID
		ht.Add("product_group_id", this.ProductGroupId);
		// 商品カラーID
		ht.Add("product_color_id", this.ProductColorId);
		// セール対象
		ht.Add(Constants.SQL_PARAMETR_PRODUCT_SALE_ONLY, this.SaleFilter);
		// 現在時刻
		ht.Add(Constants.SQL_PARAMETR_CURRENT_TIME, DateTime.Now);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ht.Add("language_code", RegionManager.GetInstance().Region.LanguageCode);
			ht.Add("language_locale_id", RegionManager.GetInstance().Region.LanguageLocaleId);
		}
		// 配送種別
		ht.Add(Constants.FIELD_PRODUCT_SHIPPING_TYPE, (string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_SHIPPING_ID]);
		// 決済種別
		ht.Add("payment_id", (string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PAYMENT_ID]);

		// 定期商品変更
		if (this.IsChangeProduct)
		{
			ht.Add("fixed_purchase_product_change_id", (string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_ID]);
		}

		return ht;
	}

	/// <summary>
	/// 商品リストCanonicalタグURL作成
	/// </summary>
	/// <returns>CanonicalタグURL</returns>
	protected string CreateProductListCanonicalUrl()
	{
		var canonicalUrlParameter = new Dictionary<string, string>(this.RequestParameter);
		canonicalUrlParameter.Remove(ProductCommon.URL_KEY_CATEGORY_NAME);
		canonicalUrlParameter.Add(ProductCommon.URL_KEY_CATEGORY_NAME, GetCanonicalText());
		var canonicalUrl = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + ProductCommon.CreateProductListUrl(canonicalUrlParameter);
		return canonicalUrl;
	}

	/// <summary>
	/// カノニカルタグ用テキスト取得
	/// </summary>
	/// <returns>カノニカルタグ用テキスト</returns>
	protected string GetCanonicalText()
	{
		if (string.IsNullOrEmpty(this.CategoryName)) return string.Empty;

		// カテゴリ一覧取得
		var categoryList = ProductCommon.GetParentAndCurrentCategories(
			this.ShopId,
			this.CategoryId,
			this.UserFixedPurchaseMemberFlg);

		var canonicalTexts = categoryList.Cast<DataRowView>().Where(drv =>
				((string)drv[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID] == this.CategoryId)
				&& (string.IsNullOrEmpty((string)drv[Constants.FIELD_PRODUCTCATEGORY_CANONICAL_TEXT]) == false))
			.Select(drv => drv[Constants.FIELD_PRODUCTCATEGORY_CANONICAL_TEXT]).ToArray();

		return (canonicalTexts.Length == 0) ? this.CategoryName : canonicalTexts[0].ToString();
	}

	/// <summary>
	/// SEOテキスト取得
	/// </summary>
	/// <returns>SEOテキスト</returns>
	public string GetSeoText()
	{
		var seoTextBeforeConvert = new StringBuilder(
			new SeoMetadatasService().Get(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.FLG_SEOMETADATAS_DATA_KBN_PRODUCT_LIST).SeoText);

		var seoTextAfterConvert = ProductCommon.GetSeoTextForProductList(
			this.ShopId,
			this.CategoryId,
			this.BrandId,
			this.ProductGroupId,
			this.MinPrice,
			this.MaxPrice,
			this.Color,
			this.SearchWord,
			this.CampaignIcon,
			this.ProductTag,
			seoTextBeforeConvert);
		return seoTextAfterConvert;
	}

	/// <summary>
	/// 商品追加ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbAddProduct_OnClick(object sender, EventArgs e)
	{
		var repeater = (this.IsDispImageKbnOn == false)
			&& (this.IsPc || (this.IsSmartPhone && DesignCommon.UseResponsive))
				? GetParentRepeaterItem((LinkButton)sender, "rProductList")
				: GetParentRepeaterItem((LinkButton)sender, "rProductsListView");
		var whfProductId = GetWrappedControl<WrappedHiddenField>(repeater, "hfProductId");
		var whfLoopIndex = ((LinkButton)sender).CommandArgument;
		var productId = whfProductId.Value;
		var wddlVariationSelect = GetWrappedControl<WrappedDropDownList>(repeater, "ddlVariationSelect");
		var variationId = wddlVariationSelect.SelectedValue;

		// 商品情報取得
		var products = new ProductService().Get(this.ShopId, productId);
		var productOptionSettingList = new ProductOptionSettingList(products.ProductOptionSettings);

		var productOptionTexts = productOptionSettingList.GetDisplayProductOptionSettingSelectValues();

		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_ID] = productId;
		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_VARIATION_ID] = string.IsNullOrEmpty(variationId) ? productId : variationId;
		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_PRODUCT_OPTION] = productOptionTexts;
		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_BACK] = true;

		var url = PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(
			(string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_FIXED_PURCHASE_ID]);

		Response.Redirect(url);
	}

	/// <summary>
	/// 商品変更ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbChangeProduct_OnClick(object sender, EventArgs e)
	{
		var repeater = (this.IsDispImageKbnOn == false)
			&& (this.IsPc || (this.IsSmartPhone && DesignCommon.UseResponsive))
				? GetParentRepeaterItem((LinkButton)sender, "rProductList")
				: GetParentRepeaterItem((LinkButton)sender, "rProductsListView");
		var whfProductId = GetWrappedControl<WrappedHiddenField>(repeater, "hfProductId");
		var productId = whfProductId.Value;
		var wddlVariationSelect = GetWrappedControl<WrappedDropDownList>(repeater, "ddlVariationSelect");
		var variationId = wddlVariationSelect.SelectedValue;

		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_AFTER_PRODUCT_ID] = productId;
		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_VARIATION_ID] = string.IsNullOrEmpty(variationId) ? productId : variationId;
		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_BACK] = true;

		var url = PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(
			(string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_FIXED_PURCHASE_ID]);

		Response.Redirect(url);
	}

	/// <summary>
	/// バリエーション選択方法の表示制御：スタンダードまたはドロップダウンリスト
	/// </summary>
	/// <remarks>スタンダードとDDLの違い
	/// スタンダード：売切れ状態のときのみ「在庫なし」を表示
	/// ドロップダウンリスト：在庫状況に応じて在庫文言を表示</remarks>
	protected ListItemCollection SetVariationSelectForDropDownList(string productId)
	{
		var variationList = new ListItemCollection();
		var variation = ProductCommon.GetProductInfo(this.ShopId, productId, this.MemberRankId, this.UserFixedPurchaseMemberFlg);
		foreach (DataRowView drvProduct in variation)
		{
			var products = (string[])Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_VARIATION_IDS];
			if (products.Any(p => p == (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID])) continue;

			if (this.IsChangeProduct && (this.AfterChangeItems != null))
			{
				var targetChangeItem = this.AfterChangeItems.FirstOrDefault(changeItem => changeItem.ProductId == productId);
				if ((targetChangeItem != null)
					&& (targetChangeItem.ItemUnitType == Constants.FLG_FIXEDPURCHASEAFTERPRODUCTCHANGEITEM_ITEM_UNIT_TYPE_VARIATION)
					&& (targetChangeItem.VariationId != (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID])) continue;
			}

			// ドロップダウン作成
			StringBuilder sbListItemtext = new StringBuilder();
			sbListItemtext.Append(CreateVariationName(drvProduct, "", "", Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION));
			ListItem liVariation = new ListItem(sbListItemtext.ToString(), (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
			variationList.Add(liVariation);
		}

		return variationList;
	}

	/// <summary>
	/// バリエーション選択方法の表示制御：スタンダードまたはドロップダウンリスト
	/// </summary>
	/// <remarks>スタンダードとDDLの違い
	/// スタンダード：売切れ状態のときのみ「在庫なし」を表示
	/// ドロップダウンリスト：在庫状況に応じて在庫文言を表示</remarks>
	protected ListItemCollection SetProductOptionPriceForDropDownList()
	{
		var variationList = new ListItemCollection();
		var variation = ProductCommon.GetProductInfo(this.ShopId, "", this.MemberRankId, this.UserFixedPurchaseMemberFlg);
		foreach (DataRowView drvProduct in variation)
		{
			StringBuilder sbListItemtext = new StringBuilder();
			sbListItemtext.Append(CreateVariationName(drvProduct, "", "", Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION));
			ListItem liVariation = new ListItem(sbListItemtext.ToString(), (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
			variationList.Add(liVariation);
		}

		return variationList;
	}

	/// <summary>
	/// 戻る
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_OnClick(object sender, EventArgs e)
	{
		if (this.IsChangeProduct)
		{
			Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_BACK] = true;
		}
		else
		{
			Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_BACK] = true;
		}

		var url = PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(
			(string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_FIXED_PURCHASE_ID]);

		Response.Redirect(url);
	}

	/// <summary>
	/// 定期変更後商品取得
	/// </summary>
	/// <returns></returns>
	private FixedPurchaseAfterChangeItemModel[] GetAfterChangeItems()
	{
		var models = new FixedPurchaseProductChangeSettingService().GetAfterChangeItems(
			StringUtility.ToEmpty((string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_ID]));
		return models;
	}

	/// <summary>
	/// 商品追加ボタンを表示するか
	/// </summary>
	/// <param name="index">商品を特定するためのインデックス</param>
	/// <returns>商品追加ボタンを表示するか</returns>
	protected bool DisplayTheAddProductButton(int index)
	{
		var result = (this.IsChangeProduct == false) && (string.IsNullOrEmpty((string)GetVariationErrorMessage(index)));
		return result;
	}

	/// <summary>
	/// 商品変更ボタンを表示するか
	/// </summary>
	/// <param name="index">商品を特定するためのインデックス</param>
	/// <returns>商品変更ボタンを表示するか</returns>
	protected bool DisplayTheChangeProductButton(int index)
	{
		var result = (this.IsChangeProduct) && (string.IsNullOrEmpty((string)GetVariationErrorMessage(index)));
		return result;
	}

	/// <summary>
	/// バリエーション商品エラー情報を取得する
	/// </summary>
	/// <param name="index">商品を特定するためのインデックス</param>
	/// <returns></returns>
	protected object GetVariationErrorMessage(int index)
	{
		var result = ProductVariationMasterListForDisplayError[index].Keys.Contains(ERROE_MESSAGE_KEY)
			? ProductVariationMasterListForDisplayError[index][ERROE_MESSAGE_KEY]
			: string.Empty;
		return result;
	}

	/// <summary>
	/// 過去に定期購入した制限あり商品の場合エラーメッセージを設定する
	/// </summary>
	public void SetErrorMessageProductOrderLimitItem()
	{
		if ((Constants.PRODUCT_ORDER_LIMIT_ENABLED == false)
			|| (Constants.PRODUCT_ORDER_LIMIT_KBN_CAN_BUY == Constants.ProductOrderLimitKbn.ProductOrderLimitOff))
		{
			return;
		}

		var orderItems = new OrderService().GetOrderHistoryList(this.LoginUserId)
			.SelectMany(order => order.Items).Distinct()
			.Select(orderItem => orderItem.ProductId).ToArray();
		foreach (var product in this.ProductVariationMasterListForDisplayError)
		{
			var productId = (string)GetKeyValue(product, Constants.FIELD_PRODUCT_PRODUCT_ID);
			if (IsFixedPurchaseProductLimit(productId) && orderItems.Contains(productId))
			{
				var tmpErrorProductName = string.Format(
					"「 {0}{1} 」",
					GetProductName(productId),
					GetKeyValue(product, Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1));
				var errorMessage = string.Format(
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_NOT_PRODUCT_ORDER_LIMIT_FOR_FIXED_PURCHASE_PRODUCT_ADD),
					tmpErrorProductName);
				product.Add(ERROE_MESSAGE_KEY, errorMessage);
			}
		}
	}

	/// <summary>
	/// 商品名取得
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>商品名</returns>
	protected string GetProductName(string productId)
	{
		foreach (var product in this.ProductMasterHashtableList)
		{
			var tmpProductId = product.Contains(Constants.FIELD_PRODUCT_PRODUCT_ID)
				? (string)product[Constants.FIELD_PRODUCT_PRODUCT_ID]
				: string.Empty;
			if (productId == tmpProductId)
			{
				var productName = product.Contains(Constants.FIELD_PRODUCT_NAME)
					? (string)product[Constants.FIELD_PRODUCT_NAME]
					: string.Empty;
				return productName;
			}
		}
		return string.Empty;
	}

	/// <summary>
	/// 定期購入に制限があるか
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>定期購入に制限があるか</returns>
	protected bool IsFixedPurchaseProductLimit(string productId)
	{
		foreach (var product in this.ProductMasterHashtableList)
		{
			var tmpProductId = product.Contains(Constants.FIELD_PRODUCT_PRODUCT_ID)
				? (string)product[Constants.FIELD_PRODUCT_PRODUCT_ID]
				: string.Empty;
			if ((productId == tmpProductId) && product.Contains(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG))
			{
				var isOrderLimit = (string)product[Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG]
					== Constants.FLG_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG_VALID;
				return isOrderLimit;
			}
		}
		return false;
	}

	/// <summary>
	/// エラー表示用商品情報の紐づけ
	/// </summary>
	/// <remarks>
	/// 商品マスターリストの同一商品数を表示するバリエーション情報と紐づける
	/// 定期購入情報で選択済みの商品の場合紐づけ対象外になる
	/// HACK：そもそもバリエーション単位の画面なのだからリピーターで指定する表示オブジェクトをProductMasterListではなくバリエーション単位のものに変えたい…
	/// </remarks>
	/// <returns>画面表示商品とエラー表示用商品情報紐づけ結果</returns>
	protected void SetLinkingErrorDisplayProduct()
	{
		var variations = this.ProductVariationMasterList.ToHashtableList();
		var targetVariationIds = new List<string>();
		foreach (var row in this.ProductMasterHashtableList)
		{
			var productId = (string)row[Constants.FIELD_PRODUCT_PRODUCT_ID];
			var productIdCount = this.ProductMasterHashtableList
				.Where(product => (string)product[Constants.FIELD_PRODUCT_PRODUCT_ID] == productId).ToArray().Length;
			var variationGroupIndex = 0;
			for (int i = 0; i < productIdCount; i++)
			{
				// 商品IDで紐づくバリエーションのリスト
				var linkedVariations = variations.Where(
					variation => (string)variation[Constants.FIELD_PRODUCT_PRODUCT_ID] == productId).ToArray();
				var variationId = (string)linkedVariations[variationGroupIndex][Constants.FIELD_PRODUCT_VARIATION_ID];

				// 定期購入情報で選択済みの商品の場合バリエーションインデックスをインクリメントして再取得
				if (((string[])Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_VARIATION_IDS]).Contains(variationId))
				{
					variationGroupIndex++;
					variationId = (string)this.ProductVariationList[productId][variationGroupIndex][Constants.FIELD_PRODUCT_VARIATION_ID];
				}

				if (targetVariationIds.Contains(variationId) == false) targetVariationIds.Add(variationId);

				variationGroupIndex++;
			}
		}

		this.ProductVariationMasterListForDisplayError = this.ProductVariationMasterListForDisplayError.Where(
			errorVariation => targetVariationIds.Any(
				target => (string)errorVariation[Constants.FIELD_PRODUCT_VARIATION_ID] == target)).ToList();
	}

	#region プロパティ
	/// <summary>カテゴリ名</summary>
	protected string CategoryName { get; set; }
	/// <summary>商品マスターリスト</summary>
	protected DataView ProductMasterList { get; set; }
	/// <summary>商品マスターハッシュテーブルリスト</summary>
	protected List<Hashtable> ProductMasterHashtableList { get; set; }
	/// <summary>商品バリエーションリスト</summary>
	protected DataView ProductVariationMasterList { get; set; }
	/// <summary>エラー表示用商品バリエーションリスト</summary>
	protected List<Dictionary<string, object>> ProductVariationMasterListForDisplayError { get; set; }
	/// <summary>商品画像用バリエーションリスト</summary>
	protected Dictionary<string, List<DataRowView>> ProductVariationList { get; set; }
	/// <summary>商品子カテゴリリスト</summary>
	protected List<DataRowView> ProductCategoryList { get; set; }
	/// <summary>ページャHTML</summary>
	protected string PagerHtml { get; set; }
	/// <summary>ページネーションタグ</summary>
	protected string PaginationTag { get; set; }
	/// <summary>アラートメッセージ</summary>
	protected string AlertMessage { get; set; }
	/// <summary>商品検索ワードランキング用に検索文字列を保存するか</summary>
	private bool DoRegisterProductSearchWord { get { return (bool?)Session[Constants.SESSION_KEY_DO_REGISTER_PRODUCT_SEARCHWORD] ?? false; } }
	/// <summary>Seoテキスト</summary>
	public string SeoText
	{
		get { return GetSeoText(); }
	}
	/// <summary>Seoテキスト</summary>
	public Dictionary<string, string> SelectedProducts
	{
		get { return (Dictionary<string, string>)ViewState["SelectedProducts"]; }
		set { ViewState["SelectedProducts"] = value; }
	}
	/// <summary>定期変更後商品</summary>
	private FixedPurchaseAfterChangeItemModel[] AfterChangeItems { get; set; }
	/// <summary>商品変更か</summary>
	protected bool IsChangeProduct
	{
		get
		{
			return Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED
				&& (string.IsNullOrEmpty((string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_ID]) == false);
		}
	}
	#endregion
}
