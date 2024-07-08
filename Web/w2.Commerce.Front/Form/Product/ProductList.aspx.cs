/*
=========================================================================================================
  Module      : 商品一覧画面処理(ProductList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.App.Common.Web.Page;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.SeoMetadatas;
using w2.Domain.SetPromotion;
using w2.Domain.SubscriptionBox;
using ProductListDispSettingUtility = ProductListDispSetting.ProductListDispSettingUtility;

public partial class Form_Product_ProductList : ProductListPage
{
	/// <summary>リピートプラスONEページ遷移必須判定</summary>
	public override bool RepeatPlusOneNeedsRedirect { get { return Constants.REPEATPLUSONE_OPTION_ENABLED; } }
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Http; } }	// Httpアクセス

	#region ラップ済みコントロール宣言
	WrappedHiddenField WhfIsRedirectAfterAddProduct { get { return GetWrappedControl<WrappedHiddenField>("hfIsRedirectAfterAddProduct", ""); } }
	WrappedRepeater WrTopProductList { get { return GetWrappedControl<WrappedRepeater>("rTopProductList"); } }
	WrappedLabel WlbPagination  { get { return GetWrappedControl<WrappedLabel>("lbPagination"); } }
	WrappedUpdatePanel WupProductDetailModal { get { return GetWrappedControl<WrappedUpdatePanel>("upProductDetailModal"); } }
	WrappedHtmlGenericControl WdvModalBackground { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvModalBackground"); } }
	protected WrappedHiddenField WhfPageNumber { get { return GetWrappedControl<WrappedHiddenField>("hfPageNumber"); } }
	protected WrappedLinkButton WlbInfiniteLoadingLowerNextButton { get { return GetWrappedControl<WrappedLinkButton>("lbInfiniteLoadingLowerNextButton"); } }
	protected WrappedLinkButton WlbInfiniteLoadingUpperNextButton { get { return GetWrappedControl<WrappedLinkButton>("lbInfiniteLoadingUpperNextButton"); } }
	protected WrappedHiddenField WhfDisplayPageNumberMax { get { return GetWrappedControl<WrappedHiddenField>("hfDisplayPageNumberMax"); } }
	protected WrappedRepeater WrpProductModal { get { return GetWrappedControl<WrappedRepeater>("rpProductModal"); } }
	#endregion

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
			// リクエストよりパラメタ取得（商品一覧共通処理）
			//------------------------------------------------------
			GetParameters();

			//------------------------------------------------------
			// 表示可能カテゴリかチェック（表示不能の場合ログインページへ）
			//------------------------------------------------------
			this.CategoryName = CheckDisplayableCategory(this.MemberRankId);

			//------------------------------------------------------
			// ブランドチェック
			//------------------------------------------------------
			BrandCheck();

			//------------------------------------------------------
			// 商品一覧情報取得
			//------------------------------------------------------
			// UNDONE:ブックマーク遷移で詳細検索が動かない ＆ 宣言部分の詳細検索パーツが削除できない 動作が残っている
			// 商品データ取得
			//if (ProductCommon.searchModes.Count == 0)
			//{
			//    // アプリケーションプールのリサイクルが行われることによって、
			//    // static変数のsearchModesに格納された商品詳細検索条件情報がクリアされてしまう。
			//    // 商品詳細検索のパーツのメソッドを呼んで、商品詳細検索条件情報を作成する。
			//    Form_Common_Product_BodyProductAdvancedSearchBox advancedSearchBox =
			//        (Form_Common_Product_BodyProductAdvancedSearchBox)LoadControl("../../" + Constants.PAGE_FRONT_ADVANCEDSEARCHBOX);
			//    advancedSearchBox.CreateSearchModes();
			//}
			// 商品一覧表示用データ取得
			var info = GetProductListInfosFromCacheOrDb();

			this.ProductMasterList = info.Products;
			this.ProductVariationMasterList = info.ProductVariations;
			this.ProductVariationList = info.ProductGroupedVariations;
			this.ProductCategoryList = info.ChildCategories;

			// 商品の総計取得
			this.TotalProductCount = (this.ProductMasterList.Count != 0) ? (int)this.ProductMasterList[0].Row["row_count"] : 0;

			this.IsUserFixedPurchaseAble = (CheckFixedPurchaseLimitedUserLevel(this.ShopId, this.ProductId));
			//------------------------------------------------------
			// 商品検索文字列保存処理
			//------------------------------------------------------
			if ((Constants.W2MP_PRODUCT_SEARCHWORD_RANKING_ENABLED) && (this.DoRegisterProductSearchWord))
			{
				ProductSearchWordRegister.Regist(
					this.ShopId,
					((Constants.SMARTPHONE_OPTION_ENABLED && SmartPhoneUtility.CheckSmartPhone(Request.UserAgent)) ? Constants.FLG_PRODUCTSEARCHWORDHISTORY_ACCESS_KBN_SMARTPHONE : Constants.FLG_PRODUCTSEARCHWORDHISTORY_ACCESS_KBN_PC),
					this.SearchWord,
					this.TotalProductCount);
			}

			Session[Constants.SESSION_KEY_DO_REGISTER_PRODUCT_SEARCHWORD] = null;

			//------------------------------------------------------
			// 商品が０個であれば情報表示
			//------------------------------------------------------
			if (this.ProductMasterList.Count == 0)
			{
				this.AlertMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
			}
			else
			{
				var productId = new List<string> { (string)this.ProductMasterList[0][Constants.FIELD_PRODUCT_PRODUCT_ID] };
				this.ProductIdForModal = productId[0];
				this.WrpProductModal.DataSource = productId;
			}
			this.WdvModalBackground.Attributes["style"] = "display:none;";

			//------------------------------------------------------
			// ページャ作成（商品一覧処理で総件数を取得）
			//------------------------------------------------------
			Dictionary<string, string> nextUrlParameter = new Dictionary<string, string>(this.RequestParameter);
			// パラメーター削除
			nextUrlParameter.Remove(Constants.REQUEST_KEY_PAGE_NO);
			nextUrlParameter.Remove(ProductCommon.URL_KEY_CATEGORY_NAME);
			// パラメーターセット
			nextUrlParameter.Add(ProductCommon.URL_KEY_CATEGORY_NAME, this.CategoryName);
			string strNextUrl = ProductCommon.CreateProductListUrl(nextUrlParameter);

			// パスを参照して、PCサイト、スマートフォンサイト用のページャを切り替えて出力する
			this.PagerHtml = WebPager.CreateProductListPager(this.TotalProductCount, this.PageNumber, strNextUrl.ToString(), this.DisplayCount);
			this.WlbPagination.Text = this.PagerHtml;

			// お気に入りデータを画面に設定
			SetFavoriteDataForDisplay(this.ProductMasterList);

			this.PaginationTag = WebPager.CreatePaginationTag(this.TotalProductCount, this.PageNumber, strNextUrl, this.DisplayCount);

			// パラメータから商品タグを取得
			var productTags = GetTagSettingListForSeo(this.RequestParameter);
			this.ProductTag = (productTags != null) ? string.Join(" ", productTags.Cast<DataRowView>().Select(item => item["tag_name"]).ToList()) : string.Empty;

			//------------------------------------------------------
			// 画面全体でデータバインド
			//------------------------------------------------------
			this.DataBind();

			// ページングコンテンツを画面に設定
			if (this.ProductMasterList.Count != 0)
			{
				SetTopProductListData();
			}
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
	/// ページプリレンダー
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_PreRender(object sender, RepeaterCommandEventArgs e)
	{
		if (IsPostBack || (e.CommandName != Constants.COMMAND_NAME_SMART_ARRIVALMAIL))
		{
			// ミニカートから商品を削除した時等に表示を更新する
			var info = GetProductListInfosFromCacheOrDb(this.PageNumber);
			this.ProductMasterList = info.Products;
			this.ProductVariationMasterList = info.ProductVariations;
			this.ProductVariationList = info.ProductGroupedVariations;
			this.ProductCategoryList = info.ChildCategories;
			SetFavoriteDataForDisplay(this.ProductMasterList);
			DataBind();
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
	/// 商品一覧情報を取得（キャッシュorDBから）
	/// </summary>
	/// <param name="pageNumber">ページ番号</param>
	/// <returns>商品一覧情報</returns>
	private ProductListInfos GetProductListInfosFromCacheOrDb(int? pageNumber = null)
	{
		const string REQUEST_PARAM_KEY_PAGE_NUMBER = "pno";

		// リクエストパラメータのうち、自動追加から除外するリクエストキーを指定
		var excludeParameterKeys = new[]
		{
			"current_date",
			REQUEST_PARAM_KEY_PAGE_NUMBER,
		};

		// 指定番号 → リクエスト情報 → 最初のページ の優先順で特定
		var pno = pageNumber.HasValue
			? pageNumber.Value.ToString()
			: this.RequestParameter.ContainsKey(REQUEST_PARAM_KEY_PAGE_NUMBER)
				? this.RequestParameter[REQUEST_PARAM_KEY_PAGE_NUMBER]
				: "1";

		// キャッシュキー作成
		// ・現在の日付以外のパラメタをキャッシュキーとする
		// ・会員ランクもキーに追加する
		var cacheKey = "ProductList " + string.Join(",", this.RequestParameter.Keys
			.Where(key => excludeParameterKeys.Contains(key) == false)
			.Select(key => key + "=" + this.RequestParameter[key]).ToArray());
		cacheKey += REQUEST_PARAM_KEY_PAGE_NUMBER + "=" + pno;
		cacheKey += ",MemberRankId=" + this.MemberRankId;
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
		var products = GetProductsList(this.RequestParameter);
		var productVariations = (Constants.PRODUCTLIST_VARIATION_DISPLAY_ENABLED) ? GetVariationList(products, this.MemberRankId) : new DataView();
		var groupedVariationList = (Constants.PRODUCTLIST_VARIATION_DISPLAY_ENABLED) ? GetGroupedVariationList(productVariations) : null;
		var childCategories = GetAllowedChildCategories(this.MemberRankId);

		var productInfos = new ProductListInfos(
			products,
			productVariations,
			groupedVariationList,
			childCategories);

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
	/// 商品一覧取得SQLパラメタ作成
	/// </summary>
	/// <returns>SQLパラメタ</returns>
	public override Hashtable CreateGetProductListSqlParam()
	{
		// 検索ワード分割（5ワードを超えた場合エラーページへ遷移）
		char[] delimiterChars = {' ', '　'}; // 半角全角スペース
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
		ht.Add("fixed_purchase_filter", this.FixedPurchaseFilter);
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
		return ht;
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
	/// モバイル用URL取得
	/// </summary>
	/// <returns></returns>
	protected string GetMobileUrl()
	{
		return ProductCommon.CreateMobileProductListUrl(
					Request.Url.Authority, 
					this.ShopId, 
					this.CategoryId, 
					"",
					this.CampaignIcon,
					"",
					"",
					ProductListDispSettingUtility.SortDefault,
					ProductListDispSettingUtility.DispImgKbnDefault,
					this.BrandId);
	}

	/// <summary>
	/// 表示可能カテゴリかチェックを行い、現在のカテゴリ名を返します。
	/// チェックNGであれば、適切なページにリダイレクトします。
	/// </summary>
	/// <param name="memberRankId">会員ランクID</param>
	/// <returns>カテゴリ名</returns>
	private string CheckDisplayableCategory(string memberRankId)
	{
		String categoryName = "";

		// カテゴリ一覧取得
		DataView categoryList =	ProductCommon.GetParentAndCurrentCategories(
			this.ShopId,
			this.CategoryId,
			this.UserFixedPurchaseMemberFlg);

		//ひとつでもNG表示ものがあればNG
		foreach (DataRowView category in categoryList)
		{
			if (MemberRankOptionUtility.CheckMemberRankPermission(
				memberRankId,
				(string)category[Constants.FIELD_PRODUCTCATEGORY_MEMBER_RANK_ID]) == false)
			{
				string errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCTCATEGORY_MEMBERRANK_CANT_DISPLAY_ERROR).Replace("@@ 1 @@", (string)category[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME]);
				if (this.IsLoggedIn)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
				else
				{
					// ログインページにエラーメッセージを残す
					Session[Constants.SESSION_KEY_ERROR_MSG_FOR_LOGINPAGE] = errorMessage;

					// ログインページへ（エラーメッセージ、ログイン後遷移URLをわたす）
					StringBuilder sbUrl = new StringBuilder();
					sbUrl.Append(this.SecurePageProtocolAndHost).Append(Constants.PATH_ROOT).Append(Constants.PAGE_FRONT_LOGIN);
					sbUrl.Append("?").Append(Constants.REQUEST_KEY_NEXT_URL).Append("=").Append(HttpUtility.UrlEncode(this.RawUrl));

					Response.Redirect(sbUrl.ToString());
				}
			}

			// 現在のカテゴリ名を取得
			if ((string)category[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID] == this.CategoryId)
			{
				categoryName = (Constants.GLOBAL_OPTION_ENABLE)
					? NameTranslationCommon.GetCategoryNameTranslationName(
						this.CategoryId,
						(string)category[Constants.FIELD_PRODUCTCATEGORY_NAME])
					: (string)category[Constants.FIELD_PRODUCTCATEGORY_NAME];
			}
		}

		return categoryName;
	}

	/// <summary>
	/// テーブル表示用データ作成
	/// </summary>
	protected List<List<DataRowView>> CreateProductListForTable(int rowCount)
	{
		// テーブル表示用商品データリスト
		List<List<DataRowView>> listProductTable = new List<List<DataRowView>>();

		//------------------------------------------------------
		// テーブル表示用Repeaterにデータソース設定
		//------------------------------------------------------
		if (this.ProductMasterList.Count > 0)
		{
			// 親の設定
			// 親には行数を持たせる
			List<int> listParent = new List<int>();
			int iParentCount = this.ProductMasterList.Count / rowCount + (((this.ProductMasterList.Count % rowCount) > 0) ? 1 : 0);
			for (int iLoop = 0; iLoop < iParentCount; iLoop++)
			{
				listParent.Add(iLoop);
			}

			// 子の設定
			foreach (int rownum in listParent)
			{
				List<DataRowView> listTableChild = new List<DataRowView>();
				int count = 0;
				foreach (DataRowView drv in this.ProductMasterList)
				{
					if ((count < this.ProductMasterList.Count) &&
						(count >= rownum * rowCount) &&
						(count < ((rownum + 1) * rowCount))
						)
					{
						listTableChild.Add(drv);
					}
					count++;
				}
				listProductTable.Add(listTableChild);
			}
		}
		return listProductTable;
	}

	/// <summary>
	/// 商品情報に該当するバリエーションリスト取得
	/// </summary>
	/// <param name="productData">商品情報</param>
	/// <returns></returns>
	/// <remarks></remarks>
	protected List<Dictionary<string, object>> GetProductListVariation(DataRowView productData)
	{
		if (Constants.PRODUCTLIST_VARIATION_DISPLAY_ENABLED == false) return new List<Dictionary<string, object>>();

		this.ProductVariationMasterList.RowFilter
			= Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID + " = '" + (string)GetProductData(productData, Constants.FIELD_PRODUCT_PRODUCT_ID) + "'";
		this.IsUserFixedPurchaseAble = (CheckFixedPurchaseLimitedUserLevel(this.ShopId, (string)GetProductData(productData, Constants.FIELD_PRODUCT_PRODUCT_ID)) == false);
		
		return GetVariationAddCartList(this.ProductVariationMasterList);
	}

	/// <summary>
	/// リピータコマンド（カート投入バリエーション一覧）
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void AddCartVariationList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		// カート投入バリエーション一覧で選択されたバリエーションIDをプロパティにセット
		this.ProductId = ((HiddenField)e.Item.FindControl("hfProductId")).Value;
		this.VariationId = ((HiddenField)e.Item.FindControl("hfVariationId")).Value;
		var wddlSubscriptionBox = GetWrappedControl<WrappedDropDownList>(e.Item, "ddlSubscriptionBox");
		// 注文数を1固定、複数バリエーション選択カート投入
		string cartAddProductCount = "1";
		
		switch (e.CommandName)
		{
			// カート投入処理
			case "CartAdd":
				var addCartType = IsAddCartByGiftType()
					? Constants.AddCartKbn.GiftOrder
					: Constants.AddCartKbn.Normal;
				AddCart(addCartType, cartAddProductCount, WhfIsRedirectAfterAddProduct.Value);
				break;

			case "CartAddFixedPurchase":
				AddCart(Constants.AddCartKbn.FixedPurchase, cartAddProductCount, WhfIsRedirectAfterAddProduct.Value);
				break;

			case "CartAddSubscriptionBox":
				var subscriptionBoxDetailUrl =
					new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_SUBSCRIPTIONBOX_DETAIL)
						.AddParam(Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID, wddlSubscriptionBox.SelectedValue)
						.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, this.ProductId)
						.AddParam(Constants.REQUEST_KEY_VARIATION_ID, this.VariationId)
						.AddParam(Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_FOR_COURSE_LIST, Constants.REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_PRODUCT_DETAIL)
						.AddParam(Constants.REQUEST_KEY_ITEM_QUANTITY, "1")
						.CreateUrl();

				Response.Redirect(subscriptionBoxDetailUrl);
				break;

			case "AddFavorite":
				AddToFavorite(this.ShopId, this.ProductId, this.CategoryId, this.VariationId);
				break;

			case "CartAddGift":
				AddCart(Constants.AddCartKbn.GiftOrder, cartAddProductCount, WhfIsRedirectAfterAddProduct.Value);
				break;

			// 通知登録関連処理
			case "ArrivalMail":
				// 入荷通知メール一覧画面へ
				Response.Redirect(CreateRegistUserProductArrivalMailUrl(
					this.ShopId,
					this.ProductId,
					this.VariationId,
					((HiddenField)e.Item.FindControl("hfArrivalMailKbn")).Value,
					this.RawUrl));
				break;

			case Constants.COMMAND_NAME_SMART_ARRIVALMAIL:
				ViewArrivalMailForm(e);
				break;
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
	/// 対象の商品バリエーションを含むセットプロモーションを取得
	/// </summary>
	/// <param name="variationInfo">商品バリエーション情報</param>
	/// <returns>該当商品バリエーションを含むセットプロモーション</returns>
	protected SetPromotionModel[] GetSetPromotionByVariation(Dictionary<string, object> variationInfo)
	{
		var setPromotionList = GetSetPromotionByVariation(
			(string)variationInfo[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID],
			(string)variationInfo[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID],
			this.ProductVariationMasterList);
		return setPromotionList;
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
	/// rAddCartVariationList ItemDataBound Handler
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rAddCartVariationList_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{
			var item = e.Item;
			var wddlSubscriptionBox = GetWrappedControl<WrappedDropDownList>(item, "ddlSubscriptionBox");
			if (wddlSubscriptionBox != null)
			{
				var wlbCartAddSubscriptionBoxList = GetWrappedControl<WrappedLinkButton>(item, "lbCartAddSubscriptionBoxList");
				var whfProduct = GetWrappedControl<WrappedHiddenField>(item, "hfProduct");
				var whfVariation = GetWrappedControl<WrappedHiddenField>(item, "hfVariation");
				var whfShopId = GetWrappedControl<WrappedHiddenField>(item, "htShopId");
				var whfSubscriptionBoxFlg = GetWrappedControl<WrappedHiddenField>(item, "hfSubscriptionBoxFlg");
				var whfFixedPurchase = GetWrappedControl<WrappedHiddenField>(item, "hfFixedPurchase");
				var whfCanFixedPurchase = GetWrappedControl<WrappedHiddenField>(item, "hfCanFixedPurchase");
				var wgcNotSubscriptionBoxOnly = GetWrappedControl<WrappedHtmlGenericControl>(item, "phNotSubscriptionBoxOnly");

				wgcNotSubscriptionBoxOnly.Visible = whfSubscriptionBoxFlg.Value != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY;

				var isSubscriptionBoxFlgEnable = ((whfSubscriptionBoxFlg.Value != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID)
					&& (whfFixedPurchase.Value != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID) && (whfCanFixedPurchase.Value == "True") && (this.IsUserFixedPurchaseAble));
				if (isSubscriptionBoxFlgEnable)
				{
					var subscriptionBoxes = GetAvailableSubscriptionBoxesByProductId(whfShopId.Value, whfProduct.Value, whfVariation.Value);
					var subscriptionBoxedForDisplay = subscriptionBoxes
						.Where(subscriptionBox => IsSelectableProduct(subscriptionBox, whfProduct.Value, whfVariation.Value)).ToArray();

					wddlSubscriptionBox.Visible = (subscriptionBoxedForDisplay.Length > 1) && isSubscriptionBoxFlgEnable;
					wddlSubscriptionBox.DataSource = subscriptionBoxedForDisplay;
					wddlSubscriptionBox.DataBind();
					if (subscriptionBoxedForDisplay.Length > 0)
					{
						var whfSubscriptionBoxDisplayName = GetWrappedControl<WrappedHiddenField>(item, "hfSubscriptionBoxDisplayName");
						whfSubscriptionBoxDisplayName.Value = subscriptionBoxedForDisplay.First().DisplayName;
						wddlSubscriptionBox.SelectedIndex = 0;
						wlbCartAddSubscriptionBoxList.Visible = true;
					}
					else
					{
						wlbCartAddSubscriptionBoxList.Visible = false;
					}
				}
				else
				{
					wddlSubscriptionBox.Visible = wlbCartAddSubscriptionBoxList.Visible = false;
				}
			}
		}
	}

	/// <summary>
	/// ページングコンテンツを画面に設定
	/// </summary>
	private void SetTopProductListData()
	{
		var pageCount = (int)Math.Ceiling((double)this.TotalProductCount / this.DisplayCount);

		// ページ番号更新
		this.WhfPageNumber.Value = this.PageNumber.ToString();

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
		if (this.ProductListPaginationContentList == null) return;
		switch (((LinkButton)sender).CommandArgument)
		{
			case LOADING_TYPE_LOWER:
				if (this.PageNumber >= this.ProductListPaginationContentList.Length) return;
				this.PageNumber = GetLowerNextLoadPageNo(this.PageNumber);
				break;

			case LOADING_TYPE_UPPER:
				if (this.PageNumber <= 1) return;
				this.PageNumber = GetUpperNextLoadPageNo(this.PageNumber);
				break;
		}

		// ページ番号更新
		this.WhfPageNumber.Value = this.PageNumber.ToString();

		var info = GetProductListInfosFromCacheOrDb(this.PageNumber);

		this.ProductMasterList = info.Products;
		this.ProductVariationMasterList = info.ProductVariations;
		this.ProductVariationList = info.ProductGroupedVariations;

		var nextUrlParameter = new Dictionary<string, string>(this.RequestParameter);

		// パラメーター削除
		nextUrlParameter.Remove(Constants.REQUEST_KEY_PAGE_NO);
		nextUrlParameter.Remove(ProductCommon.URL_KEY_CATEGORY_NAME);

		// パラメーターセット
		nextUrlParameter.Add(ProductCommon.URL_KEY_CATEGORY_NAME, this.CategoryName);
		var nextUrl = ProductCommon.CreateProductListUrl(nextUrlParameter);
		this.WlbPagination.Text = WebPager.CreateProductListPager(
			this.TotalProductCount,
			this.PageNumber,
			nextUrl,
			this.DisplayCount);

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
		if (this.IsDispImageKbnOn && this.IsPc)
		{
			productListRepeater = (Repeater)this.WrTopProductList.Items[this.PageNumber - 1].FindControl("rProductsListView");
		}
		else if (this.IsDispImageKbnWindowsShopping || this.IsSmartPhone)
		{
			productListRepeater = (Repeater)this.WrTopProductList.Items[this.PageNumber - 1].FindControl("rProductsWindowShopping");
		}
		productListRepeater.DataSource = this.ProductMasterList;
		productListRepeater.DataBind();

		// 該当の商品情報要素をロード済みに変更
		GetProductListPaginationContent(this.PageNumber).ToLoaded();
	}

	/// <summary>
	/// 表示中の商品情報を破棄する
	/// </summary>
	/// <param name="commandArgument">コマンド引数</param>
	private void DiscardInfiniteLoadProductContent(string commandArgument)
	{
		var displayProductCount = this.ProductListPaginationContentList.Count(x => x.IsLoaded) * this.DisplayCount;
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
		if (this.IsDispImageKbnOn)
		{
			productListRepeater = (Repeater)this.WrTopProductList.Items[discardPageNumber - 1].FindControl("rProductsListView");
		}
		if (this.IsDispImageKbnWindowsShopping)
		{
			productListRepeater = (Repeater)this.WrTopProductList.Items[discardPageNumber - 1].FindControl("rProductsWindowShopping");
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

	/// <summary>バリエーションを持っているか</summary>
	protected new bool HasVariation
	{
		get { return (bool)ViewState["HasVariation"]; }
		private set { ViewState["HasVariation"] = value; }
	}
	/// <summary>カテゴリ名</summary>
	protected string CategoryName { get; set; }
	/// <summary>商品マスターリスト</summary>
	protected DataView ProductMasterList { get; set; }
	/// <summary>商品バリエーションリスト</summary>
	protected DataView ProductVariationMasterList { get; set; }
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
	/// <summary>カラーでの検索か否か</summary>
	private bool IsColorSerch { get { return (string.IsNullOrEmpty(Request.QueryString[Constants.REQUEST_KEY_PRODUCT_COLOR_ID]) == false); } }
	/// <summary>Seoテキスト</summary>
	public string SeoText
	{
		get { return GetSeoText(); }
	}
	/// <summary>不正なページ読み込みでないか</summary>
	private bool? IsValidPageRequest
	{
		get { return (bool?)Session[Constants.SESSION_KEY_PRODUCT_LIST_IS_VALID_PAGE_REQUEST]; }
		set { Session[Constants.SESSION_KEY_PRODUCT_LIST_IS_VALID_PAGE_REQUEST] = value; }
	}
	/// <summary>モーダル表示用商品ID</summary>
	private string ProductIdForModal
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
