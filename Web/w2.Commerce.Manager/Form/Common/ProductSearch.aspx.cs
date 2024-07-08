/*
=========================================================================================================
  Module      : 商品検索ページ処理(ProductSearch.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order;
using w2.Common.Web;
using w2.Domain.ShopShipping;
using w2.Domain.ProductTaxCategory;

public partial class Form_Common_ProductSearch : ProductPage
{
	/// <summary>商品税カテゴリキャッシュ</summary>
	private readonly Dictionary<string, ProductTaxCategoryModel> m_productTaxCategoriesCache = new Dictionary<string, ProductTaxCategoryModel>();

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 検索区分取得
			switch (Request[Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN])
			{
				case Constants.KBN_PRODUCT_SEARCH_PRODUCT:
				case Constants.KBN_PRODUCT_SEARCH_VARIATION:
				case Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT:
				case Constants.KBN_PRODUCT_SEARCH_SUBSCRIPTION_BOX:
					this.ProductSearchKbn = Request[Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN];
					break;

				default:
					// 検索区分が不正の場合エラーページへ
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					return;
			}

			// パラメータ取得
			Hashtable parameters = GetParameters();

			// 配送料種別取得用商品IDリストが存在する?
			var productIds = (string)parameters[Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE_PRODUCT_IDS];
			if (string.IsNullOrEmpty(productIds) == false)
			{
				// 商品の配送種別が全て同じ場合、検索条件に配送種別を付与しリダイレクト
				var shippingType = "";
				var products = ProductCommon.GetProductsInfo(this.LoginOperatorShopId, productIds.Split(','));
				foreach (DataRowView product in products)
				{
					if (shippingType == "")
					{
						shippingType = (string)product[Constants.FIELD_PRODUCT_SHIPPING_TYPE];
						continue;
					}
					if (shippingType != (string)product[Constants.FIELD_PRODUCT_SHIPPING_TYPE])
					{
						shippingType = "";
						break;
					}
				}
				parameters[Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE] = shippingType;
				Response.Redirect(CreateProductSearchListUrl(parameters));
				Response.End();
			}

			// コンポーネント初期化
			InitializeComponents();

			// 画面に検索値セット
			SetValues(parameters);

			if (this.IsNotSearchDefault) return;

			// 頒布会コースIDがある場合検索区分を頒布会に変更する
			if (string.IsNullOrEmpty((string)parameters[Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID]) == false
				&& (string)parameters[Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID] != ",")
			{
				this.ProductSearchKbn = Constants.KBN_PRODUCT_SEARCH_SUBSCRIPTION_BOX;
			}

			// 商品一覧取得
			var productInfo = GetProductList(parameters);

			// 商品該当件数取得
			int totalProductCounts = productInfo.Count == 0 ? 0 : (int)productInfo[0]["row_count"];

			// エラー表示
			StringBuilder errorMessage = new StringBuilder();
			if (totalProductCounts > Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST)
			{
				// 上限件数より多い場合、エラー表示
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_OVER_HIT_LIST));
				errorMessage.Replace("@@ 1 @@", StringUtility.ToNumeric(totalProductCounts));
				errorMessage.Replace("@@ 2 @@", StringUtility.ToNumeric(Constants.CONST_DISP_CONTENTS_OVER_HIT_LIST));
			}
			else if (totalProductCounts == 0)
			{
				// 該当件数なしの場合、エラー表示
				errorMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
			}

			tdErrorMessage.InnerHtml = errorMessage.ToString();
			trListError.Visible = (errorMessage.Length != 0);

			if (trListError.Visible == false)
			{
				// 商品一覧データバインド
				rList.DataSource = productInfo;
				rList.DataBind();

				// ページャ作成
				lbPager1.Text = WebPager.CreateDefaultListPager(
					totalProductCounts,
					this.CurrentPageNo,
					CreateProductSearchListUrl(parameters));
			}
		}
	}

	/// <summary>
	/// パラメータ取得
	/// </summary>
	/// <returns>パラメータが格納されたHashtable</returns>
	private Hashtable GetParameters()
	{
		var parameters = new Hashtable
		{
			{ Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID, StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID]) },
			{ Constants.REQUEST_KEY_PRODUCT_NAME, StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_PRODUCT_NAME]) },
			{ Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE, StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE]) },
			{ Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT, StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT]) },
			{ Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID, StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID]) },
			{ Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE_PRODUCT_IDS, StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE_PRODUCT_IDS]) },
			{ Constants.REQUEST_KEY_PRODUCT_VALID_FLG, StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_PRODUCT_VALID_FLG]) },
			{ Constants.REQUEST_KEY_PRODUCT_FIXED_PURCHASE_FLG, StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_PRODUCT_FIXED_PURCHASE_FLG]) },
			{ Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG, StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG]) },
			{ Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID, StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID]) },
			{ Constants.REQUEST_KEY_PRODUCT_SEARCH_CALLER, StringUtility.ToEmpty(this.Request[Constants.REQUEST_KEY_PRODUCT_SEARCH_CALLER])},
		};

		// 頒布会コースID
		this.SubscriptionBoxId = (string)parameters[Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID];

		// ページ番号（ページャ動作時のみもちまわる）
		int currentPageNumber;
		if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out currentPageNumber) == false)
		{
			currentPageNumber = 1;
		}
		this.CurrentPageNo = currentPageNumber;

		return parameters;
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 配送種別ドロップダウン作成
		ddlShippingType.Items.Add(new ListItem("", ""));
		foreach (var shopShipping in new ShopShippingService().GetShippingInfoByShopId(this.LoginOperatorShopId))
		{
			ddlShippingType.Items.Add(new ListItem(shopShipping.ShopShippingName, shopShipping.ShippingId));
		}

		// 定期購入ドロップダウン作成
		var isFixedPurchaseFlgOnly = (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_FIXED_PURCHASE_FLG])
			== Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY);
		if (isFixedPurchaseFlgOnly == false)
		{
			ddlFixedPurchase.Items.Add(new ListItem(string.Empty, string.Empty));
		}

		var fixedPurchaseItems = ValueText.GetValueItemArray(Constants.TABLE_PRODUCT, "fixed_purchase");
		foreach (var item in fixedPurchaseItems)
		{
			// Condition for products after second subscription
			if (isFixedPurchaseFlgOnly
				&& (item.Value != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_VALID))
			{
				continue;
			}
			ddlFixedPurchase.Items.Add(item);
		}

		// 有効フラグドロップダウン作成
		ddlValidFlg.Items.Add(new ListItem("", ""));
		ddlValidFlg.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_PRODUCT, Constants.FIELD_PRODUCT_VALID_FLG));
	}

	/// <summary>
	/// 検索値を画面にセット
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	private void SetValues(Hashtable parameters)
	{
		tbProductId.Text = (string)parameters[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID];
		tbName.Text = (string)parameters[Constants.REQUEST_KEY_PRODUCT_NAME];
		foreach (ListItem li in ddlShippingType.Items)
		{
			if (li.Value == (string)parameters[Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE]) li.Selected = true;
		}
		foreach (ListItem li in ddlFixedPurchase.Items)
		{
			if (li.Value == (string)parameters[Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT]) li.Selected = true;
		}

		// 有効フラグ
		ddlValidFlg.Items.Cast<ListItem>().ToList().ForEach(li =>
			li.Selected = (li.Value == (string)parameters[Constants.REQUEST_KEY_PRODUCT_VALID_FLG]));
	}

	/// <summary>
	/// 検索クリアURL取得
	/// </summary>
	/// <returns>クリアURL</returns>
	protected string GetClearSearchUrl()
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN, this.ProductSearchKbn);

		if (this.IsSubscriptionBox)
		{
			urlCreator
				.AddParam(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID, this.SubscriptionBoxId)
				.AddParam(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG, Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_SEARCH_FLG_VALID);
		}

		var url = urlCreator.CreateUrl();
		return url;
	}

	/// <summary>
	/// 商品一覧情報取得
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>商品一覧情報</returns>
	private DataView GetProductList(Hashtable parameters)
	{
		string statementName = "";
		switch (this.ProductSearchKbn)
		{
			case Constants.KBN_PRODUCT_SEARCH_PRODUCT:
				statementName = "GetProductList";
				break;

			case Constants.KBN_PRODUCT_SEARCH_VARIATION:
			case Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT:
				statementName = "GetVariationList";
				break;

			case Constants.KBN_PRODUCT_SEARCH_SUBSCRIPTION_BOX:
				statementName = "GetSubscriptionBoxItemVariationList";
				break;
		}

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("ProductSearch", statementName))
		{
			var productList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, GetSqlParams(parameters));
			if ((string)parameters[Constants.REQUEST_KEY_PRODUCT_SEARCH_CALLER]
				== Constants.REQUEST_KEY_NAME_PRODUCT_SALE)
			{
				productList.RowFilter = string.Format("{0} IN ({1},{2})",
					Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG,
					Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID,
					Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_VALID);
			}
			return productList;
		}
    }

	/// <summary>
	/// SQLパラメータ取得
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>SQLパラメータ</returns>
	private Hashtable GetSqlParams(Hashtable parameters)
	{
		Hashtable sqlParams = new Hashtable();
		sqlParams.Add(Constants.FIELD_PRODUCT_SHOP_ID, this.LoginOperatorShopId);
		sqlParams.Add(Constants.FIELD_PRODUCT_PRODUCT_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(parameters[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID]));
		sqlParams.Add(Constants.FIELD_PRODUCT_NAME + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(parameters[Constants.REQUEST_KEY_PRODUCT_NAME]));
		sqlParams.Add(Constants.FIELD_PRODUCT_SHIPPING_TYPE, StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE]));
		sqlParams.Add("fixed_purchase", StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT]));
		sqlParams.Add(Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID]));
		sqlParams.Add(Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, "");
		sqlParams.Add("is_order_product", (this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT) ? "1" : "");
		sqlParams.Add("bgn_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.CurrentPageNo - 1) + 1);
		sqlParams.Add("end_row_num", Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.CurrentPageNo);
		sqlParams.Add(Constants.FIELD_PRODUCT_VALID_FLG, StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_PRODUCT_VALID_FLG]));
		sqlParams.Add(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG, StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG]));
		sqlParams.Add(Constants.FIELD_SUBSCRIPTIONBOXITEM_SUBSCRIPTION_BOX_COURSE_ID, StringUtility.ToEmpty(parameters[Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID]));

		return sqlParams;
	}

	/// <summary>
	/// ページ遷移URL作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <param name="pageNumber">ページ番号</param>
	/// <returns>ページ遷移URL</returns>
	private string CreateProductSearchListUrl(Hashtable parameters, int pageNumber)
	{
		StringBuilder url = new StringBuilder();
		url.Append(CreateProductSearchListUrl(parameters));
		url.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(pageNumber);

		return url.ToString();
	}
	/// <summary>
	/// ページ遷移URL作成
	/// </summary>
	/// <param name="parameters">検索パラメータ</param>
	/// <returns>ページ遷移URL</returns>
	private string CreateProductSearchListUrl(Hashtable parameters)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN, this.ProductSearchKbn)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID, (string)parameters[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_NAME, (string)parameters[Constants.REQUEST_KEY_PRODUCT_NAME])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE, (string)parameters[Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT, (string)parameters[Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT])
			.AddParam(Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID, (string)parameters[Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, (string)parameters[Constants.REQUEST_KEY_PRODUCT_VALID_FLG])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_FIXED_PURCHASE_FLG, (string)parameters[Constants.REQUEST_KEY_PRODUCT_FIXED_PURCHASE_FLG])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID, (string)parameters[Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG, (string)parameters[Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SEARCH_CALLER, (string)parameters[Constants.REQUEST_KEY_PRODUCT_SEARCH_CALLER]);
		return url.CreateUrl();
	}

	/// <summary>
	/// JavaScriptのパラメータ作成
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>JavaScriptのパラメータ</returns>
	/// <remarks>選択した商品を親画面にセットするJavaScriptのパラメータ</remarks>
	protected string CreateJavaScriptSetProductInfo(DataRowView product)
	{
		StringBuilder javaScriptParam = new StringBuilder();

		var taxCategoryId = (string)product[Constants.FIELD_PRODUCT_TAX_CATEGORY_ID];
		if (m_productTaxCategoriesCache.ContainsKey(taxCategoryId) == false)
		{
			m_productTaxCategoriesCache.Add(
				taxCategoryId,
				new ProductTaxCategoryService().Get((string)product[Constants.FIELD_PRODUCT_TAX_CATEGORY_ID]));
		}
		var productTaxCategory = m_productTaxCategoriesCache[taxCategoryId];

		// 商品ID
		javaScriptParam.Append("'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_PRODUCT_ID]).Replace("'", "\\'")).Append("',");
		// 仕入先ID
		javaScriptParam.Append("'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_SUPPLIER_ID]).Replace("'", "\\'")).Append("',");
		if (this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_PRODUCT)
		{
			// バリエーションID
			javaScriptParam.Append("'',");
			// 商品名
			javaScriptParam.Append("'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_NAME]).Replace("'", "\\'")).Append("',");
			// 表示価格
			javaScriptParam.Append("'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_DISPLAY_PRICE]).ToPriceString()).Append("',");
			// 特別価格
			javaScriptParam.Append("'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE]).ToPriceString()).Append("',");
			// 有効価格（セール価格、会員ランク価格考慮）
			javaScriptParam.Append("'',");
			// セールID
			javaScriptParam.Append("'',");
			// 定期購入フラグ
			javaScriptParam.Append("'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG])).Append("'");
		}
		else
		{
			// バリエーションID
			javaScriptParam.Append("'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTVARIATION_V_ID]).Replace("'", "\\'")).Append("',");
			// 商品名
			javaScriptParam.Append("'").Append(StringUtility.ToEmpty(CreateProductAndVariationName(product)).Replace("'", "\\'")).Append("',");
			// 表示価格
			javaScriptParam.Append("'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTVARIATION_PRICE]).ToPriceString()).Append("',");
			// 特別価格
			javaScriptParam.Append("'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE]).ToPriceString()).Append("',");
			// 有効価格（セール価格、会員ランク価格考慮）
			javaScriptParam.Append("'").Append(StringUtility.ToEmpty(GetProductValidityPrice(product).ToPriceString())).Append("',");
			// セールID
			javaScriptParam.Append("'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID])).Append("',");
			// 定期購入フラグ
			javaScriptParam.Append("'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG])).Append("'");
		}

		if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
		{
			// 利用不可配送間隔月
			javaScriptParam.Append(",'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING])).Append("'");
			// 利用不可配送間隔日
			javaScriptParam.Append(",'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING])).Append("'");
			// 利用不可配送間隔週
			javaScriptParam.Append(",'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING])).Append("'");
		}
		else
		{
			// paramの引数調整
			javaScriptParam.Append(",''");
			javaScriptParam.Append(",''");
			javaScriptParam.Append(",''");
		}
		// 税率
		javaScriptParam.Append(",'").Append(StringUtility.ToEmpty(productTaxCategory.TaxRate)).Append("'");

		// 税率
		javaScriptParam.Append(",'").Append(StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_SHIPPING_TYPE])).Append("'");

		return javaScriptParam.ToString();
	}

	/// <summary>
	/// 検索実行イベントハンドラ
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ProductSearch(object sender, EventArgs e)
	{
		Hashtable parameters = GetParameters();
		parameters[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID] = tbProductId.Text.Trim();
		parameters[Constants.REQUEST_KEY_PRODUCT_NAME] = tbName.Text.Trim();
		parameters[Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE] = ddlShippingType.SelectedValue;
		parameters[Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT] = ddlFixedPurchase.SelectedValue;
		parameters[Constants.REQUEST_KEY_PRODUCT_VALID_FLG] = ddlValidFlg.SelectedValue;

		// 検索用パラメタ作成し、同じ画面にリダイレクト
		Response.Redirect(CreateProductSearchListUrl(parameters, 1));
	}

	/// <summary>
	/// 有効価格取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>有効価格</returns>
	protected decimal GetProductValidityPrice(DataRowView product)
	{
		return (this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_PRODUCT) ? 0 : OrderPage.GetProductValidityPrice(product);
	}

	/// <summary>
	/// 在庫数を管理方法に対応した表示形式で戻す
	/// </summary>
	/// <param name="product">商品データ</param>
	/// <returns>在庫管理設定に則った在庫</returns>
	protected string GetManagedStock(DataRowView product)
	{
		if (this.ProductSearchKbn == Constants.KBN_PRODUCT_SEARCH_PRODUCT) return "";

		switch (product[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN].ToString())
		{
			// 在庫管理する 在庫0以下でも購入できる
			case Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYOK:
				return "(" + StringUtility.ToNumeric(product[Constants.FIELD_PRODUCTSTOCK_STOCK]) + ")";

			// 在庫管理する 在庫0以下では購入できない
			case Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYNG:
				return StringUtility.ToNumeric(product[Constants.FIELD_PRODUCTSTOCK_STOCK]);

			// 在庫管理しない
			default:
				return "－";
		}
	}

	/// <summary>
	/// Has Search Paramter
	/// </summary>
	protected bool HasSearchParameter()
	{
		if (Request.QueryString.AllKeys.Any() == false) return false;

		foreach (var key in Request.QueryString.AllKeys.Where(x =>
			(x != Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN)
			&& (x != Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE)
			&& (x != Constants.REQUEST_KEY_PRODUCT_FIXEDPURCHASE_PRODUCT)
			&& (x != Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID)
			&& (x != Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE_PRODUCT_IDS)))
		{
			if (Request.QueryString[key] != String.Empty) return true;
		}

		return false;
	}

	#region フロントと共通化したい：有効状態取得
	/// <summary>
	/// 定期購入初回価格有効状態取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	public static bool IsProductFixedPurchaseFirsttimePriceValid(object product, bool targetVariation = false)
	{
		var fixedPurchaseFirsttimePrice =
			StringUtility.ToNumeric(
				GetKeyValue(
					product,
					targetVariation ? Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE : Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE));

		return (fixedPurchaseFirsttimePrice != String.Empty);
	}

	/// <summary>
	/// 定期購入通常価格有効状態取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	public static bool IsProductFixedPurchasePriceValid(object product, bool targetVariation = false)
	{
		var fixedPurchasePrice =
			StringUtility.ToNumeric(
				GetKeyValue(
					product,
					targetVariation ? Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE : Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE));

		return (fixedPurchasePrice != String.Empty);
	}

	/// <summary>
	/// 商品会員ランク価格有効状態取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	public static bool GetProductMemberRankPriceValid(object product, bool targetVariation = false)
	{
		return (ProductPrice.GetProductPriceType(product, targetVariation) == ProductPrice.PriceTypes.MemberRankPrice);
	}

	/// <summary>
	/// 商品セール有効状態取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>有効状態</returns>
	/// <remarks>
	/// こちらはSQL発行時に商品マスタかバリエーションかの判定を行っているので
	/// バリエーションかどうかの判定を行う必要は無い。
	/// </remarks>
	public static bool GetProductTimeSalesValid(object product)
	{
		return (ProductPrice.GetProductPriceType(product, true) == ProductPrice.PriceTypes.TimeSale);
	}

	/// <summary>
	/// 商品特別価格有効状態取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	public static bool GetProductSpecialPriceValid(object product, bool targetVariation = false)
	{
		return (ProductPrice.GetProductPriceType(product, targetVariation) == ProductPrice.PriceTypes.Special);
	}

	/// <summary>
	/// 商品通常価格有効状態取得
	/// </summary>
	/// <param name="objProduct">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>有効状態</returns>
	public static bool GetProductNormalPriceValid(object objProduct, bool targetVariation = false)
	{
		return (ProductPrice.GetProductPriceType(objProduct, targetVariation) == ProductPrice.PriceTypes.Normal);
	}
	#endregion

	#region フロントと共通化したい：金額取得
	/// <summary>
	/// 商品価格数値取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>商品価格</returns>
	public static string GetProductPriceNumeric(object product, bool targetVariation = false)
	{
		return ProductCommon.GetProductPriceNumeric(product, targetVariation);
	}

	/// <summary>
	/// 商品「特別価格」数値取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>特別価格</returns>
	public static string GetProductSpecialPriceNumeric(object product, bool targetVariation = false)
	{
		return StringUtility.ToNumeric(
			GetKeyValue(product,
				targetVariation ? Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE : Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE));
	}

	/// <summary>
	/// 商品「会員ランク価格」数値取得（バリエーション共用）
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>会員ランク価格</returns>
	public static string GetProductMemberRankPrice(object product, bool targetVariation = false)
	{
		return StringUtility.ToNumeric(
			GetKeyValue(product,
				targetVariation ? Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION : Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE));
	}

	/// <summary>
	/// 商品「タイムセール価格」数値取得（バリエーション共用）
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>タイムセールス価格</returns>
	public static string GetProductTimeSalePriceNumeric(object product)
	{
		return StringUtility.ToNumeric(GetKeyValue(product, Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE));
	}

	/// <summary>
	/// 商品「闇市価格」数値取得（バリエーション共用）
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>闇市価格</returns>
	public static string GetProductClosedMarketPriceNumeric(object product)
	{
		return StringUtility.ToNumeric(GetKeyValue(product, "real_sale_price"));
	}
	/// <summary>
	/// 商品「定期購入初回価格」数値取得（バリエーション共用）
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>定期初回購入価格価格</returns>
	public static string GetProductFixedPurchaseFirsttimePrice(object product, bool targetVariation = false)
	{
		var fixedPurchaseFirsttimePrice = StringUtility.ToNumeric(GetKeyValue(product, targetVariation
			? Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE : Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE));

		return fixedPurchaseFirsttimePrice;
	}
	/// <summary>
	/// 商品「定期購入通常価格」数値取得（バリエーション共用）
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="targetVariation">バリエーション対象</param>
	/// <returns>定期購入通常価格</returns>
	public static string GetProductFixedPurchasePrice(object product, bool targetVariation = false)
	{
		var fixedPurchasePrice = StringUtility.ToNumeric(ProductPrice.GetFixedPurchasePrice(product, targetVariation));
		return fixedPurchasePrice;
	}
	#endregion

	/// <summary>カレントページNO</summary>
	protected int CurrentPageNo
	{
		get { return (int)ViewState["CurrentPageNo"]; }
		set { ViewState["CurrentPageNo"] = value; }
	}
	/// <summary>商品検索区分</summary>
	protected string ProductSearchKbn
	{
		get { return (string)ViewState["ProductSearchKbn"]; }
		set { ViewState["ProductSearchKbn"] = value; }
	}
	/// <summary>Display NotSearch Default</summary>
	protected override bool IsNotSearchDefault
	{
		get { return (HasSearchParameter() == false) && (Constants.DISPLAY_NOT_SEARCH_DEFAULT); }
	}
	/// <summary>頒布会コースからの検索か</summary>
	private bool IsSubscriptionBox
	{
		get { return (string.IsNullOrEmpty(this.SubscriptionBoxId) == false); }
	}
	/// <summary>頒布会コースID</summary>
	private string SubscriptionBoxId
	{
		get { return (string)ViewState["SubscriptionBoxId"]; }
		set { ViewState["SubscriptionBoxId"] = value; }
	}
}