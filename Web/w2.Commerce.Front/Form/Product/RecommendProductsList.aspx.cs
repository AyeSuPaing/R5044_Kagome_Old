/*
=========================================================================================================
  Module      : おすすめタグ・商品一覧画面処理(RecommendProductsList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Awoo;
using w2.App.Common.Awoo.GetPage;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Helper;
using w2.Common.Web;
using w2.Domain.Favorite;
using w2.Domain.Product;

public partial class Form_Product_RecommendProductsList : ProductPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Http; } } // Httpアクセス

	WrappedRepeater WrTopProductList { get { return GetWrappedControl<WrappedRepeater>("rTopProductList"); } }
	WrappedLabel WlbPagination { get { return GetWrappedControl<WrappedLabel>("lbPagination"); } }
	WrappedUpdatePanel WupProductDetailModal { get { return GetWrappedControl<WrappedUpdatePanel>("upProductDetailModal"); } }
	protected WrappedHiddenField WhfPageNumber { get { return GetWrappedControl<WrappedHiddenField>("hfPageNumber"); } }
	protected WrappedLinkButton WlbInfiniteLoadingLowerNextButton { get { return GetWrappedControl<WrappedLinkButton>("lbInfiniteLoadingLowerNextButton"); } }
	protected WrappedLinkButton WlbInfiniteLoadingUpperNextButton { get { return GetWrappedControl<WrappedLinkButton>("lbInfiniteLoadingUpperNextButton"); } }
	protected WrappedHiddenField WhfDisplayPageNumberMax { get { return GetWrappedControl<WrappedHiddenField>("hfDisplayPageNumberMax"); } }
	WrappedHtmlGenericControl WdvModalBackground { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvModalBackground"); } }
	protected WrappedRepeater WrpProductModal { get { return GetWrappedControl<WrappedRepeater>("rpProductModal"); } }
	WrappedHiddenField WhfIsRedirectAfterAddProduct { get { return GetWrappedControl<WrappedHiddenField>("hfIsRedirectAfterAddProduct", ""); } }

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// Awoo連携オプションOFFの場合、TOPへ遷移
			if (Constants.AWOO_OPTION_ENABLED == false)
			{
				Response.Redirect(Constants.PATH_ROOT);
			}

			SetByRequestParameter();

			// AwooApiからページ取得
			var response = AwooApiFacade.GetPage(new GetPageRequest()
			{
				Tags = this.Tags.Split(','),
				Page = this.PageNo,
				Limit = Constants.AWOO_PAGE_PRODUCT_LIMIT,
				Sort = this.Sort,
			});
			// ページが存在しない場合はエラー表示
			if (response == null)
			{
				this.AlertMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
				this.DataBind();
				return;
			}

			SetPageInfo(response.Result.PageInfo);
			this.SuggestionTags = response.Result.SuggestionTags;

			// レスポンスの商品IDから商品一覧画面に表示可能な商品情報を取得
			GetDisplayableProducts(response.Result.Products);

			// 表示可能な商品情報がDBから取得できず、かつページが1しかない場合エラーメッセージを表示
			if ((this.RecommendedProducts.Any() == false)
				&& (response.Result.ProductsTotal <= Constants.AWOO_PAGE_PRODUCT_LIMIT))
			{
				this.AlertMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
				this.DataBind();
				return;
			}
			if (this.RecommendedProducts.Any())
			{
				var productId = new List<string> { (string)this.ProductMasterList[0][Constants.FIELD_PRODUCT_PRODUCT_ID] };
				this.ProductIdForModal = productId[0];
				this.WrpProductModal.DataSource = productId;
			}
			this.WdvModalBackground.Attributes["style"] = "display:none;";

			// 商品の総計取得
			this.TotalProductCount = response.Result.ProductsTotal;

			// お気に入り登録数を取得
			var favoriteCountList = new FavoriteService().GetFavoriteTotalByProduct(
				Constants.CONST_DEFAULT_SHOP_ID,
				this.RecommendedProducts.Select(product => product.ProductId).ToArray());
			this.FavoriteCountList = favoriteCountList;

			// ページャ作成
			string strNextUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_RECOMMEND_PRODUCTS_LIST)
				.AddParam(Constants.REQUEST_KEY_TAGS, this.Tags)
				.AddParam(Constants.REQUEST_KEY_SORT_KBN, this.Sort.ToText())
				.CreateUrl();
			this.PagerHtml = WebPager.CreateRecommendProductsListPager(response.Result.ProductsTotal, this.PageNo, strNextUrl, Constants.AWOO_PAGE_PRODUCT_LIMIT);
			this.PaginationTag = WebPager.CreatePaginationTag(response.Result.ProductsTotal, this.PageNo, strNextUrl, Constants.AWOO_PAGE_PRODUCT_LIMIT);

			this.DataBind();

			// ページングコンテンツを画面に設定
			if (this.TotalProductCount > 0)
			{
				this.WlbPagination.Text = this.PagerHtml;
				SetTopProductListData();
			}
		}
	}

	/// <summary>
	/// ページ初期化
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Init(object sender, EventArgs e)
	{
		base.Page_Init(sender, e);
		ValidateAndRedirectIfInvalid();
	}

	/// <summary>
	/// ページ読み込みが有効なリクエストかどうか確認
	/// </summary>
	private void ValidateAndRedirectIfInvalid()
	{
		if (this.IsValidPageRequest == null)
		{
			this.IsValidPageRequest = this.IsPc;
			return;
		}

		if (this.IsPc == this.IsValidPageRequest) return;

		this.IsValidPageRequest = this.IsPc;
		Response.Redirect(Request.RawUrl);
	}

	/// <summary>
	/// リクエストパラメータから画面プロパティをセット
	/// </summary>
	private void SetByRequestParameter()
	{
		ProductSortType sort;
		EnumHelper.TryParseToEnum((string)Request["sort"], out sort);
		this.SortContents = ValueText.GetValueItemList(
			Constants.VALUETEXT_PARAM_PRODUCT_RECOMMEND_PRODUCT_LIST,
			Constants.VALUETEXT_PARAM_PRODUCT_RECOMMEND_PRODUCT_LIST_SORT_KBN);
		int requestPageNo;
		var pageNo = int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out requestPageNo) ? requestPageNo : 1;

		this.Sort = sort;
		this.Tags = StringUtility.ToEmpty(Request["tags"]);
		this.PageNo = pageNo;
	}

	/// <summary>
	/// APIレスポンスからページ情報をセット
	/// </summary>
	/// <param name="pageInfo">ページ情報</param>
	private void SetPageInfo(PageInfo pageInfo)
	{
		this.H1 = pageInfo.H1;
		this.Description = (string.IsNullOrEmpty(pageInfo.Intro) == false)
			? pageInfo.Intro
			: pageInfo.Description;
		this.CanonicalTag = pageInfo.Canonical;
		// SEO設定
		this.Page.Title = pageInfo.Title;
		this.Page.MetaDescription = pageInfo.Description;
	}

	/// <summary>
	/// 商品マスタ情報から表示可能な商品情報を取得
	/// </summary>
	/// <param name="recommendedProducts">Awooおすすめ商品</param>
	private void GetDisplayableProducts(List<Products> recommendedProducts)
	{
		// DBから商品マスタ情報を取得
		var info = CreateProductListInfos(recommendedProducts.Select(product => product.ProductId).ToArray());

		var displayableProducts = new List<Products>();
		if ((this.Sort == ProductSortType.FinalPriceAsc) || (this.Sort == ProductSortType.FinalPriceDesc))
		{
			// 価格での並び替えはDBの最新情報で改めて行い、その順でソートする
			foreach (DataRowView o in info.Products)
			{
				var displayableProduct =
					recommendedProducts.FirstOrDefault(product => product.ProductId == (string)o[Constants.FIELD_PRODUCT_PRODUCT_ID]);
				if (displayableProduct == null) continue;
				displayableProducts.Add(displayableProduct);
			}
		}
		else
		{
			displayableProducts = recommendedProducts
				.Where(product => info.Products
					.Cast<DataRowView>()
					.Any(o => product.ProductId == (string)o[Constants.FIELD_PRODUCT_PRODUCT_ID]))
				.ToList();
		}

		// 表示可能な商品情報を設定
		this.RecommendedProducts = displayableProducts.ToList();
		this.ProductMasterList = info.Products;
		this.ProductVariationList = info.ProductGroupedVariations;
	}

	/// <summary>
	/// 商品一覧情報作成
	/// </summary>
	/// <param name="productIds">商品ID</param>
	/// <returns>商品一覧情報</returns>
	private ProductListInfos CreateProductListInfos(string[] productIds)
	{
		var products = GetProductsList(productIds);
		var productVariations = Constants.PRODUCTLIST_VARIATION_DISPLAY_ENABLED
			? GetVariationList(products, this.MemberRankId)
			: new DataView();
		var groupedVariationList = Constants.PRODUCTLIST_VARIATION_DISPLAY_ENABLED
			? GetGroupedVariationList(productVariations)
			: null;

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			// 翻訳情報セット
			var productList = products.Cast<DataRowView>().Select(drv => new ProductModel(drv)).ToArray();
			var productTranslation = (DataView)NameTranslationCommon.Translate(products, productList);
			return new ProductListInfos(
				productTranslation,
				null,
				groupedVariationList,
				null);
		}
		return new ProductListInfos(
			products,
			null,
			groupedVariationList,
			null);
	}

	/// <summary>
	/// 商品一覧取得
	/// </summary>
	/// <param name="productIds">商品ID</param>
	/// <returns>商品一覧データビュー</returns>
	private DataView GetProductsList(string[] productIds)
	{
		// 価格での並び替えはDBの最新価格でソートしなおす
		var sortKbn = "";
		switch (this.Sort)
		{
			case ProductSortType.FinalPriceAsc:
				sortKbn = Constants.KBN_SORT_PRODUCT_LIST_PRICE_ASC;
				break;

			case ProductSortType.FinalPriceDesc:
				sortKbn = Constants.KBN_SORT_PRODUCT_LIST_PRICE_DESC;
				break;

			default:
				sortKbn = "";
				break;
		}
		// SQLパラメタ作成
		var ht = new Hashtable()
		{
			{ Constants.FIELD_PRODUCT_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID },
			{ Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, this.MemberRankId },
			{ Constants.FIELD_USER_FIXED_PURCHASE_MEMBER_FLG, this.UserFixedPurchaseMemberFlg },
			{ "sort_kbn", sortKbn }
		};

		var productWhere = productIds.Any()
			? string.Format(
				"AND w2_Product.product_id IN ({0})",
				string.Join(
					",",
					productIds.Select(id => "'" + id + "'")))
			: "AND 1 != 1";

		if (Constants.SETTING_PRODUCT_LIST_SEARCH_KBN)
		{
			var productListByVariationGroup = new ProductService().GetProductListByGroupAwoo(
				ht,
				productWhere);
			return productListByVariationGroup;
		}

		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("Product", "GetProductListAwoo"))
		{
			statement.Statement = statement.Statement.Replace("@@ product_ids_where_in @@", productWhere);

			var productList = statement.SelectSingleStatementWithOC(accessor, ht);
			return productList;
		}
	}

	/// <summary>
	/// 並び替えのドロップダウン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlSort_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var selectedSort = this.ddlSort.SelectedValue;
		string strNextUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_RECOMMEND_PRODUCTS_LIST)
			.AddParam(Constants.REQUEST_KEY_TAGS, this.Tags)
			.AddParam(Constants.REQUEST_KEY_SORT_KBN, selectedSort)
			.CreateUrl();
		Response.Redirect(strNextUrl);
	}

	/// <summary>
	/// 商品マスタ情報を取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>商品マスタ</returns>
	protected DataRowView GetProductMaster(Products product)
	{
		var master = this.ProductMasterList.Cast<DataRowView>()
			.Single(drv => (string)drv[Constants.FIELD_PRODUCT_PRODUCT_ID] == product.ProductId);
		return master;
	}

	/// <summary>
	/// お気に入り登録数を取得
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>お気に入り登録数</returns>
	protected int GetFavoriteCount(string productId)
	{
		var favoriteCount = ((this.FavoriteCountList != null) && this.FavoriteCountList.ContainsKey(productId))
			? (int)this.FavoriteCountList[productId]
			: 0;

		return favoriteCount;
	}

	/// <summary>
	/// タグ検索URLの作成
	/// </summary>
	/// <param name="suggestionTag">おすすめタグ情報</param>
	/// <returns>URL</returns>
	protected string CreateSuggestionTagUrl(SuggestionTags suggestionTag)
	{
		return Constants.PATH_ROOT
			+ Constants.PAGE_RECOMMEND_PRODUCTS_LIST
			+ "?"
			+ Constants.REQUEST_KEY_TAGS
			+ "="
			+ suggestionTag.Link;
	}

	/// <summary>
	/// ページングコンテンツを画面に設定
	/// </summary>
	private void SetTopProductListData()
	{
		var pageCount = (int)Math.Ceiling((double)this.TotalProductCount / Constants.AWOO_PAGE_PRODUCT_LIMIT);

		// ページ番号更新
		this.WhfPageNumber.Value = this.PageNo.ToString();

		// 全ページ分のページングコンテンツ作成
		this.ProductListPaginationContentList = Enumerable.Range(1, pageCount)
			.Select(pageNo => new ProductListPaginationContent(pageNo))
			.ToArray();

		this.WrTopProductList.DataSource = this.ProductListPaginationContentList;
		this.WrTopProductList.DataBind();

		// 商品情報セット
		SetProductsByDisplayType();
	}

	/// <summary>
	/// 内部リピータのイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void InnerRepeater_ItemCommand(object sender, RepeaterCommandEventArgs e)
	{
		switch (e.CommandName)
		{
			case "btnOpenModalOrAddCart":
				btnOpenModalOrAddCart_OnClick(sender, e);
				break;
		}
	}

	/// <summary>
	/// ローディングイベント発火時のボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbInfiniteLoadingNextButton_OnClick(object sender, EventArgs e)
	{
		switch (((LinkButton)sender).CommandArgument)
		{
			case LOADING_TYPE_LOWER:
				if (this.PageNo >= this.ProductListPaginationContentList.Length) return;
				this.PageNo = GetLowerNextLoadPageNo(this.PageNo);
				break;

			case LOADING_TYPE_UPPER:
				if (this.PageNo <= 1) return;
				this.PageNo = GetUpperNextLoadPageNo(this.PageNo);
				break;
		}

		// ページ番号更新
		this.WhfPageNumber.Value = this.PageNo.ToString();

		// AwooApiからページ取得
		var response = AwooApiFacade.GetPage(new GetPageRequest()
		{
			Tags = this.Tags.Split(','),
			Page = this.PageNo,
			Limit = Constants.AWOO_PAGE_PRODUCT_LIMIT,
			Sort = this.Sort,
		});
		// ページが存在しない場合はエラー表示
		if (response == null)
		{
			this.AlertMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
			this.DataBind();
			return;
		}

		var recommendedProducts = response.Result.Products;

		var info = CreateProductListInfos(recommendedProducts.Select(product => product.ProductId).ToArray());

		var displayableProducts = new List<Products>();
		if ((this.Sort == ProductSortType.FinalPriceAsc) || (this.Sort == ProductSortType.FinalPriceDesc))
		{
			// 価格での並び替えはDBの最新情報で改めて行い、その順でソートする
			foreach (DataRowView o in info.Products)
			{
				var displayableProduct =
					recommendedProducts.FirstOrDefault(product => product.ProductId == (string)o[Constants.FIELD_PRODUCT_PRODUCT_ID]);
				if (displayableProduct == null) continue;
				displayableProducts.Add(displayableProduct);
			}
		}
		else
		{
			displayableProducts = recommendedProducts
				.Where(product => info.Products
					.Cast<DataRowView>()
					.Any(o => product.ProductId == (string)o[Constants.FIELD_PRODUCT_PRODUCT_ID]))
				.ToList();
		}

		this.RecommendedProducts = displayableProducts.ToList();
		this.ProductMasterList = info.Products;
		this.ProductVariationList = info.ProductGroupedVariations;

		var nextUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_RECOMMEND_PRODUCTS_LIST)
			.AddParam(Constants.REQUEST_KEY_TAGS, this.Tags)
			.AddParam(Constants.REQUEST_KEY_SORT_KBN, this.Sort.ToText())
			.CreateUrl();

		this.WlbPagination.Text = WebPager.CreateRecommendProductsListPager(
			this.TotalProductCount,
			this.PageNo,
			nextUrl,
			Constants.AWOO_PAGE_PRODUCT_LIMIT);

		var favoriteCountList = new FavoriteService().GetFavoriteTotalByProduct(
			Constants.CONST_DEFAULT_SHOP_ID,
			this.RecommendedProducts.Select(product => product.ProductId).ToArray());

		this.FavoriteCountList = favoriteCountList;

		// 商品情報破棄
		DiscardInfiniteLoadProductContent(((LinkButton)sender).CommandArgument);
		// 商品情報セット
		SetProductsByDisplayType();
	}

	/// <summary>
	/// 表示タイプ別に商品情報セット
	/// </summary>
	private void SetProductsByDisplayType()
	{
		if (this.WrTopProductList.Items.Count == 0) return;

		var productListRepeater = new Repeater();
		if (Constants.USE_INFINITE_LOAD_PRODUCT_LIST == false)
		{
			productListRepeater = (Repeater)this.WrTopProductList.Items[this.PageNo - 1].FindControl("rProductsListView");
		}
		else
		{
			productListRepeater = (Repeater)this.WrTopProductList.Items[this.PageNo - 1].FindControl("rProductsWindowShopping");
		}
		productListRepeater.DataSource = this.RecommendedProducts;
		productListRepeater.DataBind();

		// 該当の商品情報要素をロード済みに変更
		GetProductListPaginationContent(this.PageNo).ToLoaded();
	}

	/// <summary>
	/// 表示中の商品情報を破棄する
	/// </summary>
	/// <param name="commandArgument">コマンド引数</param>
	private void DiscardInfiniteLoadProductContent(string commandArgument)
	{
		var displayProductCount = this.ProductListPaginationContentList.Count(x => x.IsLoaded) * Constants.AWOO_PAGE_PRODUCT_LIMIT;
		if (displayProductCount < Constants.DISPLAY_PRODUCT_COUNT_FOR_INFINITE_LOAD) return;

		var discardPageNumber = 0;
		var displayInfiniteLoadPageContentList = this.ProductListPaginationContentList.Where(x => x.IsLoaded).Select(x => x.PageNo).ToArray();
		switch (commandArgument)
		{
			case LOADING_TYPE_LOWER:
				discardPageNumber = displayInfiniteLoadPageContentList.Min();
				break;

			case LOADING_TYPE_UPPER:
				discardPageNumber = displayInfiniteLoadPageContentList.Max();
				break;
		}

		var productListRepeater = new Repeater();
		if (Constants.USE_INFINITE_LOAD_PRODUCT_LIST)
		{
			productListRepeater = (Repeater)this.WrTopProductList.Items[discardPageNumber - 1].FindControl("rProductsWindowShopping");
		}
		else
		{
			productListRepeater = (Repeater)this.WrTopProductList.Items[discardPageNumber - 1].FindControl("rProductsListView");
		}
		productListRepeater.Dispose();
		productListRepeater.DataBind();

		// 表示されているページ番号の最大値を隠しフィールドに設定
		this.WhfDisplayPageNumberMax.Value = WebSanitizer.HtmlEncode(displayInfiniteLoadPageContentList.Max());

		// 該当の商品情報要素を未ロードに変更
		GetProductListPaginationContent(discardPageNumber).UnLoaded();
	}

	/// <summary>
	/// 注文するボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOpenModalOrAddCart_OnClick(object sender, CommandEventArgs e)
	{
		var productId = new[] { e.CommandArgument.ToString() };
		var product = this.Process.GetProduct(this.ShopId, productId[0], productId[0]);

		if (product.Count != 0)
		{
			// バリエーション無し、定期無し、ギフト無しのとき、直接カート投入
			OrderErrorcode productError = OrderCommon.CheckProductStatus(product[0], 1, Constants.AddCartKbn.Normal, this.LoginUserId);
			if (productError == OrderErrorcode.NoError)
			{
				if (((string)product[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
					&& ((string)product[0][Constants.FIELD_PRODUCT_GIFT_FLG] == Constants.FLG_PRODUCT_GIFT_FLG_INVALID))
				{
					this.ProductId = productId[0];
					this.VariationId = productId[0];
					AddCart(Constants.AddCartKbn.Normal, "1", this.WhfIsRedirectAfterAddProduct.Value);
					return;
				}
			}
		}

		this.ProductIdForModal = productId[0];
		this.WdvModalBackground.Attributes["style"] = "";
		this.WrpProductModal.DataSource = productId;
		this.WrpProductModal.DataBind();
		this.WupProductDetailModal.Update();
		ScriptManager.RegisterStartupScript(this, GetType(), "openModal", "openModal()", true);
	}

	/// <summary>
	/// モーダル閉じるボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCloseProductDetailModal_OnClick(object sender, CommandEventArgs e)
	{
		if (e.CommandName == "Close")
		{
			this.WdvModalBackground.Attributes["style"] = "display:none;";
			this.WupProductDetailModal.Update();
		}
	}

	/// <summary>
	/// 商品詳細モーダル初期表示用商品ID取得
	/// </summary>
	/// <returns>商品ID</returns>

	protected string GetProductIdForModal()
	{
		if ((this.RecommendedProducts == null) || (this.RecommendedProducts.Count == 0)) return string.Empty;
		return this.RecommendedProducts[0].ProductId;
	}

	/// <summary>タグ</summary>
	protected string Tags
	{
		get { return (string)ViewState["Tags"]; }
		set { ViewState["Tags"] = value; }
	}
	/// <summary>並び替え種別</summary>
	protected ProductSortType Sort
	{
		get { return (ProductSortType)ViewState["Sort"]; }
		set { ViewState["Sort"] = value; }
	}
	/// <summary>ページNo</summary>
	protected int PageNo
	{
		get { return (int)ViewState["PageNo"]; }
		set { ViewState["PageNo"] = value; }
	}
	/// <summary>Awooおすすめタグリスト</summary>
	protected List<SuggestionTags> SuggestionTags { get; set; }
	/// <summary>Awooおすすめ商品IDリスト</summary>
	protected List<Products> RecommendedProducts { get; set; }
	/// <summary>おすすめ商品が存在するか</summary>
	protected bool HasDisplayProduct { get { return this.RecommendedProducts != null && this.RecommendedProducts.Count != 0; } }
	/// <summary>H1</summary>
	protected string H1 { get; set; }
	/// <summary>Description</summary>
	protected string Description { get; set; }
	/// <summary>カノニカルタグ</summary>
	protected string CanonicalTag { get; set; }
	/// <summary>商品マスターリスト</summary>
	protected DataView ProductMasterList { get; set; }
	/// <summary>商品画像用バリエーションリスト</summary>
	protected Dictionary<string, List<DataRowView>> ProductVariationList { get; set; }
	/// <summary>ページャHTML</summary>
	protected string PagerHtml { get; set; }
	/// <summary>ページネーションタグ</summary>
	protected string PaginationTag { get; set; }
	/// <summary>並び替えアイテム</summary>
	protected ListItemCollection SortContents { get; set; }
	/// <summary>お気に入り登録件数情報</summary>
	protected Dictionary<string, int> FavoriteCountList { get; set; }
	/// <summary>アラートメッセージ</summary>
	protected string AlertMessage { get; set; }
	/// <summary>不正なページ読み込みでないか</summary>
	private bool? IsValidPageRequest
	{
		get { return (bool?)Session[Constants.SESSION_KEY_PRODUCT_LIST_IS_VALID_PAGE_REQUEST]; }
		set { Session[Constants.SESSION_KEY_PRODUCT_LIST_IS_VALID_PAGE_REQUEST] = value; }
	}
	/// <summary>モーダル表示用商品ID</summary>
	protected string ProductIdForModal
	{
		get { return this.Process.ProductIdForModal; }
		set { this.Process.ProductIdForModal = value; }
	}
	/// <summary>無限ロードが可能か</summary>
	protected bool IsInfiniteLoad
	{
		get { return (Constants.USE_INFINITE_LOAD_PRODUCT_LIST && (this.TotalProductCount > 0) && (this.IsPreview == false)); }
	}
}
