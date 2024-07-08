/*
=========================================================================================================
  Module     頒布会初回選択画面処理 : (SubscriptionBoxFirstSelection.aspx.cs)
 ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright  W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.SubscriptionBox;

public partial class Form_SubscriptionBox_SubscriptionBoxFirstSelection : ProductListPage
{
	/// <summary>リピートプラスONEページ遷移必須判定</summary>
	public override bool RepeatPlusOneNeedsRedirect { get { return Constants.REPEATPLUSONE_OPTION_ENABLED; } }
	/// <summary>ページアクセスタイプ Httpアクセス</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Http; } }

	#region ラップ済みコントロール宣言
	private WrappedRepeater WrLeft { get { return GetWrappedControl<WrappedRepeater>("rLeft"); } }
	private WrappedRepeater WrRight { get { return GetWrappedControl<WrappedRepeater>("rRight"); } }
	private WrappedLabel WlDiffCount { get { return GetWrappedControl<WrappedLabel>("lDiffCount"); } }
	private WrappedLabel WlNowPrice { get { return GetWrappedControl<WrappedLabel>("lNowPrice"); } }
	private WrappedLabel WlUnderMinAmount { get { return GetWrappedControl<WrappedLabel>("lUnderMinAmount"); } }
	private WrappedLabel WlOrverMaxAmount { get { return GetWrappedControl<WrappedLabel>("lOrverMaxAmount"); } }
	private WrappedLabel WlOverMaxQuantity { get { return GetWrappedControl<WrappedLabel>("lOverMaxQuantity"); } }
	private WrappedLabel WlUnderMinNumberOfProducts { get { return GetWrappedControl<WrappedLabel>("lUnderMinNumberOfProducts"); } }
	private WrappedLabel WlOrverMaxNumberOfProducts { get { return GetWrappedControl<WrappedLabel>("lOrverMaxNumberOfProducts"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 定期OPか頒布会OPがOFFだったらエラーページ
		if ((Constants.FIXEDPURCHASE_OPTION_ENABLED == false) || (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED == false))
		{
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// 存在しない頒布会コースIDだったらエラーページ
		var subscriptionBoxService = new SubscriptionBoxService();
		this.SubscriptionBox = subscriptionBoxService.GetByCourseId(this.SubscriptionBoxCourseId);
		if (this.SubscriptionBox == null)
		{
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SUBSCRIPTION_BOX_ID_INVALID);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		if (!IsPostBack)
		{
			// リクエストよりパラメタ取得（商品一覧共通処理）
			GetParameters();

			// ブランドチェック
			BrandCheck();

			// 商品一覧表示用データ取得
			SetProductList();
			SetInitializedItemQuantity();
			ViewState["MasterList"] = this.ProductMasterList;

			var subscriptionBoxDefaultItems = subscriptionBoxService.GetFirstDefaultItem(this.SubscriptionBoxCourseId);
			this.SubscriptionBoxDefaultItem = new Hashtable();
			foreach (var product in subscriptionBoxDefaultItems)
			{
				SubscriptionBoxDefaultItem.Add(product.ProductId, product);
			}

			// ページャ作成（商品一覧処理で総件数を取得）
			var nextUrlParameter = new Dictionary<string, string>(this.RequestParameter);

			// パラメーター削除
			nextUrlParameter.Remove(Constants.REQUEST_KEY_PAGE_NO);
			nextUrlParameter.Remove(ProductCommon.URL_KEY_CATEGORY_NAME);

			// 画面全体でデータバインド
			this.DataBind();
		}
	}

	/// <summary>
	/// 画面左側のリピーター内でボタン押下時の処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rLeft_ItemCommnad(object sender, RepeaterCommandEventArgs e)
	{
		var productId = (e.CommandArgument).ToString();
		switch (e.CommandName)
		{
			case "Plus":
				AddSelectedItemQuantity(productId);
				break;

			case "Minus":
				SubSelectedItemQuantity(productId);
				break;

			default:
				break;
		}

		UpdateItemQuantity(productId);
		BindItems();
	}

	/// <summary>
	/// 画面右側のリピーター内でボタン押下時の処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rRight_ItemCommnad(object sender, RepeaterCommandEventArgs e)
	{
		var productId = (e.CommandArgument).ToString();
		switch (e.CommandName)
		{
			case "Plus":
				AddSelectedItemQuantity(productId);
				break;

			case "Minus":
				SubSelectedItemQuantity(productId);
				break;

			case "Delete":
				DeleteSelectedItemQuantity(productId);
				break;
		}

		UpdateItemQuantity(productId);
		BindItems();
	}

	/// <summary>
	/// 選択商品数を+１する
	/// </summary>
	/// <param name="productId">商品ID</param>
	private void AddSelectedItemQuantity(string productId)
	{
		var product = (SubscriptionBoxDefaultItemModel)this.SubscriptionBoxDefaultItem[productId];
		var necessaryCount = product.ItemQuantity;
		var itemQuantity = GetItemQuantity(productId);
		if (itemQuantity == 0)
		{
			itemQuantity = IsNecessary(this.SubscriptionBoxDefaultItem[productId]) ? necessaryCount : 0;
		}

		SetItemQuantity(productId, itemQuantity + 1);
	}

	/// <summary>
	/// 選択商品数を-１する
	/// </summary>
	/// <param name="productId">商品ID</param>
	private void SubSelectedItemQuantity(string productId)
	{
		var product = (SubscriptionBoxDefaultItemModel)this.SubscriptionBoxDefaultItem[productId];
		var necessaryCount = product.ItemQuantity;

		var itemQuantity = GetItemQuantity(productId);
		if (itemQuantity == 0)
		{
			itemQuantity = IsNecessary(this.SubscriptionBoxDefaultItem[productId]) ? necessaryCount : 0;
		}

		if (itemQuantity > 0)
		{
			if ((IsNecessary(this.SubscriptionBoxDefaultItem[productId]) == false)
				|| (necessaryCount < itemQuantity))
			{
				itemQuantity--;
			}
		}

		SetItemQuantity(productId, itemQuantity);
	}

	/// <summary>
	/// 選択商品数を０にする
	/// </summary>
	/// <param name="productId">商品ID</param>
	private void DeleteSelectedItemQuantity(string productId)
	{
		var itemQuantity = GetItemQuantity(productId);
		if ((itemQuantity != 0)
			&& (IsNecessary(this.SubscriptionBoxDefaultItem[productId]) == false))
		{
			SetItemQuantity(productId, quantity: 0);
		}
	}

	/// <summary>
	/// 画面の更新
	/// </summary>
	private void BindItems()
	{
		WlDiffCount.Text = CalculateRemainingQuantity().ToString();
		WlNowPrice.Text = CurrencyManager.ToPrice(GetTotalAmount());
		WlUnderMinAmount.Text = CurrencyManager.ToPrice(CalculateRemainingMinAmount());
		WlUnderMinNumberOfProducts.Text = WebSanitizer.HtmlEncode(CalculateRemainingMinNumberOfProducts());
		WlOrverMaxAmount.Text = CurrencyManager.ToPrice(CalculateRemainingMaxAmount());
		WlOverMaxQuantity.Text = WebSanitizer.HtmlEncode(GetTotalItemQuantity() - this.MaxBoxCount);
		WlOrverMaxNumberOfProducts.Text = WebSanitizer.HtmlEncode(GetTotalNumberOfProducts() - this.MaxNumberOfProducts);

		dAddCartButton.Visible = this.CanAddCart;

		WrLeft.DataBind();
		WrRight.DataBind();
	}

	/// <summary>
	/// 商品数の初期設定
	/// </summary>
	private void SetInitializedItemQuantity()
	{
		var productList = this.ProductMasterList;
		if (productList == null) return;

		var selectedSubscriptionBoxItems = SessionManager.CartList != null
			? SessionManager.CartList.Items.FirstOrDefault(
				cart => cart.IsFirstSelectionSubscriptionBox
					&& (cart.SubscriptionBoxCourseId == this.SubscriptionBoxCourseId))
			: null;

		foreach (DataRow product in productList.Rows)
		{
			// 選択済みの頒布会商品がある場合は、数量を引き継ぐ
			if (selectedSubscriptionBoxItems != null)
			{
				var productId = StringUtility.ToEmpty(product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_PRODUCT_ID]);
				var subscriptionBoxItem = selectedSubscriptionBoxItems.Items.FirstOrDefault(
					cart => cart.ProductId == productId);

				if (subscriptionBoxItem != null)
				{
					product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY] = subscriptionBoxItem.CountSingle;
					SetItemQuantity(productId, subscriptionBoxItem.CountSingle);
					continue;
				}
			}

			var necessaryFlg = product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_NECESSARY_PRODUCT_FLG].ToString();
			if (necessaryFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID)
			{
				product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY] = 0;
			}
		}
		this.ProductMasterList = productList;
	}

	/// <summary>
	/// 商品数を更新する
	/// </summary>
	/// <param name="productId">商品ID</param>
	private void UpdateItemQuantity(string productId)
	{
		var productList = ViewState["MasterList"] as DataTable;

		if (productList == null) return;
		foreach (DataRow product in productList.Rows)
		{
			var bindProductID = product[Constants.FIELD_PRODUCT_PRODUCT_ID].ToString();
			if (bindProductID == productId)
			{
				product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY] = GetItemQuantity(productId);
			}
		}
		this.ProductMasterList = productList;
	}

	/// <summary>
	/// 最低購入金額と合計金額の差分を取得
	/// </summary>
	/// <returns>最低購入金額と合計金額の差分</returns>
	protected int CalculateRemainingMinAmount()
	{
		var totalDiffAmount = this.MinAmount - GetTotalAmount();
		return totalDiffAmount;
	}

	/// <summary>
	/// 合計金額の超過分を取得
	/// </summary>
	/// <returns>合計金額の超過分</returns>
	protected int CalculateRemainingMaxAmount()
	{
		if (this.MaxAmount == 0) return 0;

		var totalAmount = GetTotalAmount();
		var totalDiffAmount = (this.MaxAmount - totalAmount) < 0
			? (this.MaxAmount - totalAmount) * -1
			: 0;

		return totalDiffAmount;
	}

	/// <summary>
	/// 最低購入数量と合計数量の差分を取得
	/// </summary>
	/// <returns>最低購入数量と合計数量の差分</returns>
	protected int CalculateRemainingQuantity()
	{
		var diffBoxCount = this.MinBoxCount - GetTotalItemQuantity();

		return diffBoxCount;
	}

	/// <summary>
	/// 最低購入種類数と合計種類数の差分を取得
	/// </summary>
	/// <returns>最低購入数量と合計数量の差分</returns>
	protected int CalculateRemainingMinNumberOfProducts()
	{
		var diffBoxCount = this.MinNumberOfProducts - GetTotalNumberOfProducts();

		return diffBoxCount;
	}

	/// <summary>
	/// 最大購入種類数と合計種類数の差分を取得
	/// </summary>
	/// <returns>最大購入数量と合計数量の差分</returns>
	protected int CalculateRemainingMaxNumberOfProducts()
	{
		if (this.MaxNumberOfProducts == 0) return 0;

		var totalDiffNumberOfProducts = this.MaxNumberOfProducts - GetTotalNumberOfProducts() < 0
			? (this.MaxNumberOfProducts - GetTotalNumberOfProducts()) * -1
			: 0;

		return totalDiffNumberOfProducts;
	}

	/// <summary>
	/// 合計商品選択数を取得
	/// </summary>
	/// <returns>合計商品選択数</returns>
	protected int GetTotalItemQuantity()
	{
		var totalItemQuantity = 0;
		var productIdList = this.SubscriptionBoxDefaultItem.Keys;
		foreach (string productId in productIdList)
		{
			var itemQuantity = GetItemQuantity(productId);
			if (itemQuantity != 0)
			{
				totalItemQuantity += itemQuantity;
			}
			else
			{
				totalItemQuantity += GetNecessaryBoxCount(productId);
			}
		}
		return totalItemQuantity;
	}

	/// <summary>
	/// 合計商品種類数を取得
	/// </summary>
	/// <returns>合計商品種類数</returns>
	private int GetTotalNumberOfProducts()
	{
		var totalNumberOfProducts = 0;
		var productIdList = this.SubscriptionBoxDefaultItem.Keys;
		foreach (string productId in productIdList)
		{
			var itemQuantity = GetItemQuantity(productId);
			if (itemQuantity != 0)
			{
				totalNumberOfProducts += 1;
			}
			else
			{
				totalNumberOfProducts += GetNecessaryBoxCount(productId);
			}
		}
		return totalNumberOfProducts;
	}

	/// <summary>
	/// 合計商品金額を取得
	/// </summary>
	/// <returns>合計商品金額</returns>
	protected int GetTotalAmount()
	{
		var totalAmount = 0;
		var productList = ViewState["MasterList"] as DataTable;

		if (productList == null) return 0;
		foreach (DataRow product in productList.Rows)
		{
			var productId = product[Constants.FIELD_PRODUCT_PRODUCT_ID].ToString();
			var variationId = product[Constants.FIELD_PRODUCT_VARIATION_ID].ToString();
			var itemQuantity = Convert.ToInt32(product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY]);

			var itemPriceWithYen = GetValidPrice(productId, variationId);
			var itemPrice = int.Parse(System.Text.RegularExpressions.Regex.Replace(itemPriceWithYen, @"[^\d]", string.Empty));

			totalAmount += itemQuantity * itemPrice;
		}
		return totalAmount;
	}

	/// <summary>
	/// 初回選択商品を取得し、プロパティにセット
	/// </summary>
	private void SetProductList()
	{
		var productList = GetProductListInfosFromCacheOrDb();

		foreach (DataRowView product in productList.Products)
		{
			var productFromDb = GetProduct(
				StringUtility.ToEmpty(product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SHOP_ID]),
				StringUtility.ToEmpty(product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_PRODUCT_ID]),
				StringUtility.ToEmpty(product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_VARIATION_ID]));

			var isInStock = OrderCommon.CheckProductStatus(
					productFromDb[0],
					Convert.ToInt32(product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY]),
					Constants.AddCartKbn.SubscriptionBox,
					this.LoginUserId);

			if (isInStock != OrderErrorcode.NoError)
			{
				if (IsNecessary(product))
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SUBSCRIPTION_BOX_SOLDOUT_NECESSARY_PRODUCT);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
				product.Delete();
			}
		}
		productList.Products.Table.AcceptChanges();
		this.ProductMasterList = productList.Products.ToTable();
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

		var expire = string.IsNullOrEmpty(this.SearchWord.Trim()) ? Constants.PRODUCTLIST_CACHE_EXPIRE_MINUTES : 0;
		var productList = DataCacheManager.GetInstance().GetData(
			cacheKey,
			expire,
			CreateProductListInfosFromDb);
		return productList;
	}

	/// <summary>
	/// DBから商品一覧情報作成
	/// </summary>
	/// <returns>商品一覧情報</returns>
	private ProductListInfos CreateProductListInfosFromDb()
	{
		var products = new SubscriptionBoxService().GetFirstSelectionItems(this.SubscriptionBoxCourseId);
		var productVariations = Constants.PRODUCTLIST_VARIATION_DISPLAY_ENABLED ? GetVariationList(products, this.MemberRankId) : new DataView();
		var groupedVariationList = Constants.PRODUCTLIST_VARIATION_DISPLAY_ENABLED ? GetGroupedVariationList(productVariations) : null;

		var productList = new ProductListInfos(
			products,
			productVariations,
			groupedVariationList,
			childCategories: null);

		return productList;
	}

	/// <summary>
	/// 商品IDとバリエーションIDから有効価格の取得
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>有効価格</returns>
	private string GetValidPrice(string productId, string variationId)
	{
		if (string.IsNullOrEmpty(productId)) return string.Empty;

		var product = GetProduct(Constants.CONST_DEFAULT_SHOP_ID, productId, variationId);

		if (product.Count <= 0) return string.Empty;

		var subscriptionBoxItem = this.SubscriptionBox.SelectableProducts.FirstOrDefault(
			selectableProduct => (selectableProduct.ProductId == productId)
				&& (selectableProduct.VariationId == variationId));

		// キャンペーン期間であればキャンペーン期間価格を適用
		if (OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxItem))
		{
			return CurrencyManager.ToPrice(subscriptionBoxItem.CampaignPrice.ToPriceString());
		}

		var firstTimePrice = StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE]);
		var fixedPurchasePrice = StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE]);
		var specialPrice = StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE]);
		var validPrice = StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCTVARIATION_PRICE]);

		if (string.IsNullOrEmpty(firstTimePrice) == false)
		{
			validPrice = firstTimePrice;
		}
		else if (string.IsNullOrEmpty(fixedPurchasePrice) == false)
		{
			validPrice = fixedPurchasePrice;
		}
		else if (string.IsNullOrEmpty(specialPrice) == false)
		{
			validPrice = specialPrice;
		}

		return CurrencyManager.ToPrice(validPrice);
	}

	/// <summary>
	/// 商品情報から有効価格の取得
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <param name="isNotCampaignPeriodPrice">true:キャンペーン期間でない、false:キャンペーン期間</param>
	/// <returns>有効価格</returns>
	protected string GetValidPrice(object product)
	{
		var productId = (string)ProductPage.GetKeyValue(product, Constants.FIELD_PRODUCT_PRODUCT_ID);
		var variationId = (string)ProductPage.GetKeyValue(product, Constants.FIELD_PRODUCT_VARIATION_ID);

		return GetValidPrice(productId, variationId);
	}

	/// <summary>
	/// カートに追加押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbAddCartButton_Click(object sender, EventArgs e)
	{
		var productList = ViewState["MasterList"] as DataTable;

		if (productList == null) return;
		var selectedProductList = new List<SubscriptionBoxDefaultItemModel>();

		foreach (DataRow product in productList.Rows)
		{
			if ((int)(product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY]) > 0)
			{
				selectedProductList.Add(
					new SubscriptionBoxDefaultItemModel
					{
						SubscriptionBoxCourseId = this.SubscriptionBoxCourseId,
						Count = 1,
						TermSince = null,
						TermUntil = null,
						ShopId = product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_SHOP_ID].ToString(),
						ProductId = product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_PRODUCT_ID].ToString(),
						ItemQuantity = Convert.ToInt32(product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY]),
						VariationId = product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_VARIATION_ID].ToString(),
						BranchNo = Convert.ToInt32(product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_BRANCH_NO]),
						NecessaryProductFlg = product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_NECESSARY_PRODUCT_FLG].ToString()
					}
				);
			}
		}
		var url = CreateProductsToCartUrl(selectedProductList);
		Response.Redirect(url);
	}

	/// <summary>
	/// 選択された商品入れたカート投入URLを返却
	/// </summary>
	/// <param name="fisrtCountProducts">選択された商品</param>
	/// <returns>カート投入URL</returns>
	private string CreateProductsToCartUrl(List<SubscriptionBoxDefaultItemModel> fisrtCountProducts)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + (Constants.CART_LIST_LP_OPTION ? Constants.PAGE_FRONT_CART_LIST_LP : Constants.PAGE_FRONT_CART_LIST))
			.AddParam(Constants.REQUEST_KEY_CART_ACTION, Constants.KBN_REQUEST_CART_ACTION_ADD_SUBSCRIPTIONBOX);
		var index = fisrtCountProducts.Count > 1 ? 1 : 0;

		foreach (var fisrtCountProduct in fisrtCountProducts)
		{
			var subscriptionBoxService = new SubscriptionBoxService();
			this.SubscriptionBox = subscriptionBoxService.GetByCourseId(this.SubscriptionBoxCourseId);

			var errorMessage = string.Empty;

			// 商品情報取得
			var product = ProductCommon.GetProductVariationInfo(
				string.IsNullOrEmpty(fisrtCountProduct.ShopId) == false ? fisrtCountProduct.ShopId : this.ShopId,
				fisrtCountProduct.ProductId,
				fisrtCountProduct.VariationId,
				null);
			if (product.Count == 0)
			{
				this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// 商品在庫エラーは詳細画面に文言表示
			var productError = OrderCommon.CheckProductStatus(
				product[0],
				fisrtCountProduct.ItemQuantity,
				Constants.AddCartKbn.SubscriptionBox,
				this.LoginUserId);
			if (productError != OrderErrorcode.NoError)
			{
				errorMessage += OrderCommon.GetErrorMessage(
					productError,
					CreateProductJointName(product[0]),
					MemberRankOptionUtility.GetMemberRankName(
						(string)product[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]));
				errorMessage += "\n";

				switch (productError)
				{
					case OrderErrorcode.SellMemberRankError:
						errorMessage += OrderCommon.GetErrorMessage(
							productError,
							CreateProductJointName(product[0]),
							MemberRankOptionUtility.GetMemberRankName(
								(string)product[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]));
						break;

					default:
						errorMessage += OrderCommon.GetErrorMessage(productError, CreateProductJointName(product[0]));
						break;
				}

				this.Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
			var suffix = fisrtCountProducts.Count > 1 ? index.ToString() : string.Empty;

			urlCreator.AddParam(Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_COURSE_ID + suffix, this.SubscriptionBoxCourseId);

			if (string.IsNullOrEmpty(errorMessage))
			{
				urlCreator.AddParam(Constants.REQUEST_KEY_PRODUCT_ID + (index == 0 ? string.Empty : index.ToString()), fisrtCountProduct.ProductId)
					.AddParam(Constants.REQUEST_KEY_VARIATION_ID + (index == 0 ? string.Empty : index.ToString()), fisrtCountProduct.VariationId)
					.AddParam(Constants.REQUEST_KEY_PRODUCT_COUNT + (index == 0 ? string.Empty : index.ToString()), fisrtCountProduct.ItemQuantity.ToString());
			}
			else
			{
				this.Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
			index++;
		}
		urlCreator.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX, "1");
		SessionManager.IsOnlyAddCartFirstTime = true;
		return urlCreator.CreateUrl();
	}

	/// <summary>
	///	必須商品数を取得
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>商品数</returns>
	private int GetNecessaryBoxCount(string productId)
	{
		var productList = ViewState["MasterList"] as DataTable;
		if (productList == null) return 0;
		var result = 0;
		foreach (DataRow product in productList.Rows)
		{
			var bindProductId = product[Constants.FIELD_PRODUCT_PRODUCT_ID].ToString();
			if (bindProductId == productId)
			{
				result = Convert.ToInt32(product[Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY]);
			}
		}
		return result;
	}

	/// <summary>
	/// 右側に商品を表示するかどうか
	/// </summary>
	/// <param name="product"></param>
	/// <returns>表示するかどうか</returns>
	protected bool IsDisplay(object product)
	{
		var itemQuantity = ProductPage.GetKeyValue(product, Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_ITEM_QUANTITY);
		var result = Convert.ToInt32(itemQuantity) > 0;
		return result;
	}

	/// <summary>
	/// 必須商品かどうか
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>必須商品かどうか</returns>
	protected bool IsNecessary(object product)
	{
		var necessaryFlg = ProductPage.GetKeyValue(product, Constants.FIELD_SUBSCRIPTIONBOXDEFAULTITEM_NECESSARY_PRODUCT_FLG);
		return (necessaryFlg.ToString() == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID);
	}

	/// <summary>
	/// 商品一覧取得SQLパラメタ作成
	/// </summary>
	/// <returns>SQLパラメタ</returns>
	public override Hashtable CreateGetProductListSqlParam()
	{
		// 検索ワード分割（5ワードを超えた場合エラーページへ遷移）
		// 半角全角スペース
		char[] delimiterChars = { ' ', '　' };
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
				: string.Empty);
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
	/// 商品の数量を取得
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns></returns>
	private int GetItemQuantity(string productId)
	{
		var quantity = ViewState["subscriptionProduct_" + productId];
		return quantity != null ? (int)quantity : 0;
	}

	/// <summary>
	/// 商品の数量をセット
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="quantity">数量</param>
	private void SetItemQuantity(string productId, int quantity)
	{
		ViewState["subscriptionProduct_" + productId] = quantity;
	}

	#region プロパティ
	/// <summary>頒布会コースID</summary>
	private string SubscriptionBoxCourseId
	{
		get { return Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID]; }
	}
	/// <summary>頒布会</summary>
	private SubscriptionBoxModel SubscriptionBox { get; set; }
	/// <summary>商品マスターリスト</summary>
	protected DataTable ProductMasterList { get; set; }
	/// <summary>頒布会デフォルト商品</summary>
	private Hashtable SubscriptionBoxDefaultItem
	{
		get { return (Hashtable)Session["SubscriptionBoxDefaultItem"]; }
		set { Session["SubscriptionBoxDefaultItem"] = value; }
	}
	/// <summary>最低購入数量</summary>
	protected int MinBoxCount
	{
		get
		{
			return this.SubscriptionBox.MinimumPurchaseQuantity != null
				? (int)this.SubscriptionBox.MinimumPurchaseQuantity
				: 0;
		}
	}
	/// <summary>最大購入数量</summary>
	protected int MaxBoxCount
	{
		get
		{
			return this.SubscriptionBox.MaximumPurchaseQuantity != null
				? (int)this.SubscriptionBox.MaximumPurchaseQuantity
				: 0;
		}
	}
	/// <summary>商品合計金額下限（税込）</summary>
	private int MinAmount
	{
		get
		{
			return this.SubscriptionBox.MinimumAmount != null
				? (int)this.SubscriptionBox.MinimumAmount
				: 0;
		}
	}
	/// <summary>商品合計金額上限（税込）</summary>
	protected int MaxAmount
	{
		get
		{
			return this.SubscriptionBox.MaximumAmount != null
				? (int)this.SubscriptionBox.MaximumAmount
				: 0;
		}
	}
	/// <summary>最低購入種類</summary>
	private int MinNumberOfProducts
	{
		get
		{
			return this.SubscriptionBox.MinimumNumberOfProducts != null
				? (int)this.SubscriptionBox.MinimumNumberOfProducts
				: 0;
		}
	}
	/// <summary>最大購入種類</summary>
	private int MaxNumberOfProducts
	{
		get
		{
			return this.SubscriptionBox.MaximumNumberOfProducts != null
				? (int)this.SubscriptionBox.MaximumNumberOfProducts
				: 0;
		}
	}
	/// <summary>カート投入可能か</summary>
	protected bool CanAddCart
	{
		get
		{
			// 合計商品選択数が0より大きい
			// かつ、合計商品金額が商品合計金額下限（税込）以上
			// かつ、合計商品金額が商品合計金額上限（税込）以下
			// かつ、合計商品選択数が最低購入数量以上
			// かつ、合計商品種類数が最低購入種類以上
			// かつ、合計商品種類数が最大購入種類以下
			return (GetTotalItemQuantity() > 0)
				&& ((this.MinAmount == 0) || (this.MinAmount - GetTotalAmount() <= 0))
				&& ((this.MaxAmount == 0) || (this.MaxAmount - GetTotalAmount() >= 0))
				&& ((this.MinBoxCount == 0) || (this.MinBoxCount - GetTotalItemQuantity() <= 0))
				&& ((this.MaxBoxCount == 0) || (this.MaxBoxCount - GetTotalItemQuantity() >= 0))
				&& ((this.MinNumberOfProducts == 0) || (this.MinNumberOfProducts - GetTotalNumberOfProducts() <= 0))
				&& ((this.MaxNumberOfProducts == 0) || (this.MaxNumberOfProducts - GetTotalNumberOfProducts() >= 0));
		}
	}
	#endregion
}
