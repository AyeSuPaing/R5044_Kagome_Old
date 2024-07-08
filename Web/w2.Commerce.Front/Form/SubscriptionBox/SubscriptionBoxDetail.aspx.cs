/*
=========================================================================================================
  Module      : 頒布会詳細画面処理(SubscriptionBoxDetail.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using w2.App.Common.Option;
using w2.App.Common.Order;
using System.Linq;
using System.Web.UI;
using w2.App.Common.Global.Region.Currency;
using System.Web.UI.HtmlControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Product;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.Product;
using w2.Domain.SubscriptionBox;

/// <summary>
/// 頒布会詳細画面処理クラス
/// </summary>
public partial class Form_SubscriptionBox_SubscriptionBoxDetail : ProductPage
{
	/// <summary>商品付帯情報設定</summary>
	private static ProductOptionSettingList s_productOptionSettingList = null;
	/// <summary>頒布会商品リピーター</summary>
	WrappedRepeater WrSubscriptionBoxDeliveryTiming { get { return GetWrappedControl<WrappedRepeater>("rSubscriptionBoxDeliveryTiming"); } }

	/// <summary>
	/// ページロード
	/// </summary>
	protected void Page_Load(object sender, EventArgs e)
	{
		if ((Constants.FIXEDPURCHASE_OPTION_ENABLED == false) || (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED == false))
		{
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_404_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		var subscriptionBoxService = new SubscriptionBoxService();
		this.SubscriptionBox = subscriptionBoxService.GetByCourseId(this.SubscriptionBoxCourseId);
		if (this.SubscriptionBox == null)
		{
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SUBSCRIPTION_BOX_ID_INVALID);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		if (!IsPostBack)
		{
			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_FROM_LIST_OR_POSTBACK]))
			{
				this.SubscriputionBoxProductList = new List<SubscriptionBoxDefaultItemModel>();
				this.SubscriputionBoxProductListModify = new List<SubscriptionBoxDefaultItemModel>();
				this.TakeOverProductBranchNo = 0;
				this.SubscriptionBoxDuringItemList = null;
				this.SubscriptionBoxDuringItemListCopy = new List<SubscriptionBoxDefaultItemModel>();

				var url = new UrlCreator(this.Request.RawUrl)
					.AddParam(Constants.REQUEST_KEY_FROM_LIST_OR_POSTBACK, "1")
					.CreateUrl();
				Response.Redirect(url);
			}
			else
			{
				this.SubscriptionBoxDuringItemList = this.SubscriptionBoxDuringItemListCopy;
				DataBindDuringItemList();
			}

			if (this.SubscriptionBoxDuringItemList == null)
			{
				this.SubscriptionBoxDuringItemList = new List<SubscriptionBoxDefaultItemModel>();
			}

			this.ProductId = Request[Constants.REQUEST_KEY_PRODUCT_ID];
			this.VariationId = Request[Constants.REQUEST_KEY_VARIATION_ID];

			if (SessionManager.ProductOptionSettingList != null)
			{
				s_productOptionSettingList = SessionManager.ProductOptionSettingList;
				SessionManager.ProductOptionSettingList = null;
			}

			this.SubscriptionBox.DefaultOrderProducts =
				subscriptionBoxService.GetDisplayItems(this.SubscriptionBoxCourseId);
			
			// 商品詳細画面からの遷移の場合、選択可能な商品かを確認する
			if (Request[Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_FOR_COURSE_LIST] == Constants.REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_PRODUCT_DETAIL)
			{
				var isSelectableProduct = IsSelectableProduct(this.ProductId, this.VariationId);

				if (isSelectableProduct == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}

			// 前ページのURLを取得する
			var beforeUrl = string.Empty;
			if (Request.UrlReferrer != null)
			{
				beforeUrl = Request.UrlReferrer.AbsolutePath.Split('/').Last();
			}

			if (string.IsNullOrEmpty(beforeUrl) || (Request.Url.AbsolutePath.EndsWith(beforeUrl) == false))
			{
				this.SubscriputionBoxProductList = new List<SubscriptionBoxDefaultItemModel>();
				var subscriptionFirstItem = (this.SubscriptionBox.IsNumberTime)
					? this.SubscriptionBox.DefaultOrderProducts
						.Where(item => (item.Count == 1))
						.ToArray()
					: this.SubscriptionBox.DefaultOrderProducts
						.Where(
							item => ((item.TermSince <= DateTime.Now)
								&& (item.TermUntil >= DateTime.Now)))
						.ToArray();
				if ((Request[Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_FOR_COURSE_LIST]
						== Constants.REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_PRODUCT_DETAIL)
					&& subscriptionFirstItem.Any(
						p => (p.VariationId == this.VariationId)
							&& (p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID))
					&& (subscriptionFirstItem.Any(
							p => p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID)
						&& subscriptionFirstItem.Any(
							p => p.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID)))
				{
					this.SubscriputionBoxProductList.Add(
						new SubscriptionBoxDefaultItemModel
						{
							SubscriptionBoxCourseId = this.SubscriptionBoxCourseId,
							Count = null,
							TermSince = null,
							TermUntil = null,
							ShopId = this.ShopId,
							ProductId = this.ProductId,
							ItemQuantity = int.Parse(Request[Constants.REQUEST_KEY_ITEM_QUANTITY]),
							VariationId = this.VariationId,
							BranchNo = 0,
							NecessaryProductFlg = Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID
						});
					BindItemModifyData();
				}
				this.SubscriputionBoxProductListModify = this.SubscriputionBoxProductList;
			}

			if (this.SubscriputionBoxProductListModify == null)
			{
				this.SubscriputionBoxProductList = SessionManager.SubscriputionBoxProductListForTemp;
				this.SubscriputionBoxProductListModify = SessionManager.SubscriputionBoxProductListModifyForTemp;

				SessionManager.SubscriputionBoxProductListForTemp = null;
				SessionManager.SubscriputionBoxProductListModifyForTemp = null;
			}

			var selectableFirstProductCount = this.SubscriptionBox.DefaultOrderProducts
				.Count(item => this.SubscriptionBox.IsNumberTime
					? (item.IsInSelectableTerm(DateTime.Now) && item.Count == 1)
					: item.IsInSelectableTerm(DateTime.Now));

			this.IsWithinSelectionPeriodFirstProduct = (selectableFirstProductCount >= 1);

			this.WrSubscriptionBoxDeliveryTiming.DataSource = GetRepeaterCount();

			this.WrSubscriptionBoxDeliveryTiming.DataBind();

			DataBind();
		}
	}

	/// <summary>
	/// 周回回数の取得
	/// </summary>
	/// <returns>周回文言</returns>
	private List<string> GetRepeaterCount()
	{
		var repeaterList = (this.SubscriptionBox.IsNumberTime)
			? this.SubscriptionBox.DefaultOrderProducts
				.Where(item => item.IsInSelectableTerm(DateTime.Now))
				.Select(item => StringUtility.ToEmpty(item.Count))
				.Distinct()
				.Take((Constants.DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_PRODUCT_LIST == 0)
					? this.SubscriptionBox.DefaultOrderProducts.Length
					: Constants.DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_PRODUCT_LIST)
				.ToList()
			: this.SubscriptionBox.DefaultOrderProducts
				.Where(item => item.IsInTerm(DateTime.Now))
				.OrderBy(item => item.TermSince)
				.Select(
					item => string.Format(
						"{0}～{1}",
						StringUtility.ToEmpty(item.TermSince),
						StringUtility.ToEmpty(item.TermUntil)))
				.Distinct()
				.Take(
					(Constants.DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_PRODUCT_LIST == 0)
						? this.SubscriptionBox.DefaultOrderProducts.Length
						: Constants.DISP_LIST_CONTENTS_COUNT_SUBSCRIPTION_BOX_PRODUCT_LIST)
				.ToList();
		repeaterList.Remove(string.Empty);
		return repeaterList;
	}

	/// <summary>
	/// 商品リストを取得
	/// </summary>
	/// <param name="deliveryTiming">配送タイミング</param>
	/// <returns>商品リスト</returns>
	protected List<SubscriptionBoxDefaultItemModel> GetItemsModel(string deliveryTiming)
	{
		this.DeliveryTiming = deliveryTiming;
		var timing = deliveryTiming.Split('～');
		
		// 商品詳細からの遷移時 且つ デフォルト商品に含まれていない場合はリストに追加する
		var shouldAddProductDetailProduct = (IsFromProductDetail
			&& (this.SubscriptionBox.DefaultOrderProducts
				.All(defaultProduct =>
					((defaultProduct.ProductId == this.ProductId)
						&& (defaultProduct.VariationId == this.VariationId)) == false)));

		List<SubscriptionBoxDefaultItemModel> productList;
		if ((this.SubscriptionBox.IsNumberTime == false) && (timing.Length > 1))
		{
			productList = this.SubscriptionBox.DefaultOrderProducts
				.Where(
					product => ((((DateTime)product.TermSince).Date == (DateTime.Parse(timing[0]).Date))
						&& (((DateTime)product.TermUntil).Date == (DateTime.Parse(timing[1]).Date))))
				.ToList();

			productList = (productList.Any(p => p.IsNecessary))
				? productList.Where(p => p.IsNecessary)
					.ToList()
				: productList;

			if (shouldAddProductDetailProduct)
			{
				productList.Add(CreateSubscriptionBoxDefaultItemModelFromProductDetail());
			}

			if ((string.IsNullOrEmpty(productList[0].VariationId) == false) && (string.IsNullOrEmpty(productList[0].ProductId) == false)) return productList;

			for (var i = (productList[0].BranchNo - 1); i > 0; i--)
			{
				var preList = this.SubscriptionBox.DefaultOrderProducts.First(item => (item.BranchNo == i));

				if ((string.IsNullOrEmpty(preList.ProductId) == false) && (string.IsNullOrEmpty(preList.VariationId) == false))
				{
					productList = this.SubscriptionBox.DefaultOrderProducts
						.Where(item => (item.TermSince == preList.TermSince)
							&& (item.TermUntil == preList.TermUntil))
						.ToList();

					productList = (productList.Any(p => p.IsNecessary))
							? productList.Where(p => p.IsNecessary)
								.ToList()
							: productList;

					return productList;
				}
			}

			return productList;
		}

		productList = this.SubscriptionBox.DefaultOrderProducts
			.Where(product => (product.Count == int.Parse(timing[0]))
				&& product.IsInSelectableTerm(DateTime.Now))
			.ToList();

		if (shouldAddProductDetailProduct)
		{
			productList.Add(CreateSubscriptionBoxDefaultItemModelFromProductDetail());
		}

		if (string.IsNullOrEmpty(productList[0].VariationId) && string.IsNullOrEmpty(productList[0].ProductId))
		{
			for (int i = ((int)productList[0].Count - 1); i > 0; i--)
			{
				productList = this.SubscriptionBox.DefaultOrderProducts
					.Where(item => (item.Count == i)
						&& item.IsInSelectableTerm(DateTime.Now))
					.ToList();

				productList = (productList.Any(p => p.IsNecessary))
					? productList
						.Where(p => p.IsNecessary)
						.ToList()
					: productList;

				if ((string.IsNullOrEmpty(productList[0].ProductId) == false) && (string.IsNullOrEmpty(productList[0].VariationId) == false))
				{
					return productList;
				}
			}
		}

		if ((Request[Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_FOR_COURSE_LIST] == Constants.REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_PRODUCT_DETAIL)
			&& (int.Parse(timing[0]) == 1))
		{
			productList = productList.Where(
				p => (p.IsNecessary)
					|| (p.VariationId == this.VariationId)).ToList();
			return productList;
		}

		productList = (productList.Any(p => p.IsNecessary))
			? productList.Where(p => p.IsNecessary).ToList()
			: productList;
		return productList;
	}

	/// <summary>
	/// カート投入処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartAddSubscriptionBox_Click(object sender, EventArgs e)
	{
		var itemQuantity = Request[Constants.REQUEST_KEY_ITEM_QUANTITY];

		if ((Request[Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_FOR_COURSE_LIST] == Constants.REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_PRODUCT_DETAIL)
				&& (s_productOptionSettingList != null))
		{
			AddCart(
				Constants.AddCartKbn.SubscriptionBox,
				itemQuantity,
				Constants.KBN_REDIRECT_AFTER_ADDPRODUCT_CARTLIST,
				s_productOptionSettingList,
				null,
				this.SubscriptionBoxCourseId);
		}

		CheckValidateSelectedItem();
		foreach (RepeaterItem deliveryRepeaterItem in rSubscriptionBoxDeliveryTiming.Items)
		{
			var deliveryTiming = GetWrappedControl<WrappedHiddenField>(deliveryRepeaterItem, "hfDeliveryTiming");
			var timing = deliveryTiming.Value.Split('～');
			var isTakeOverProductsOrder = false;

			if ((timing.Length > 1)
				&& (Request[Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_FOR_COURSE_LIST] == Constants.REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_COURSE_LIST)
				&& (DateTime.Parse(timing[0]) <= DateTime.Now)
				&& (DateTime.Parse(timing[1]) > DateTime.Now))
			{
				var wrDuringTakeOverProductsRepeater = GetWrappedControl<WrappedRepeater>(deliveryRepeaterItem, "rSubscriptionBoxDuringItemList");
				foreach (RepeaterItem takeOverRepeaterItem in wrDuringTakeOverProductsRepeater.Items)
				{
					var wddlTakeOverProductsList = GetWrappedControl<WrappedDropDownList>(
						takeOverRepeaterItem,
						"ddlTakeOverProductsList");
					var wtbQuantity = GetWrappedControl<WrappedTextBox>(takeOverRepeaterItem, "tbQuantity");

					var orderProducts = this.SubscriptionBox.SelectableProducts
						.Where(item => item.VariationId == wddlTakeOverProductsList.SelectedValue).ToArray();

					var orderProduct = new SubscriptionBoxDefaultItemModel()
					{
						ShopId = orderProducts[0].ShopId,
						ProductId = orderProducts[0].ProductId,
						VariationId = orderProducts[0].VariationId,
						ItemQuantity = int.Parse(wtbQuantity.Text.Trim())
					};
					AddProductToCart(orderProduct);
					isTakeOverProductsOrder = true;
				}

				if (isTakeOverProductsOrder)
				{
					var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST).CreateUrl();
					Response.Redirect(url);
				}
			}
		}

		AddFirstProductsToCart();
	}

	/// <summary>
	/// 選択商品の有効性チェック
	/// </summary>
	private void CheckValidateSelectedItem()
	{
		foreach (RepeaterItem deliveryRepeaterItem in rSubscriptionBoxDeliveryTiming.Items)
		{
			var whfDeliveryTiming = GetWrappedControl<WrappedHiddenField>(deliveryRepeaterItem, "hfDeliveryTiming");
			var timing = whfDeliveryTiming.Value.Split('～');

			if ((timing.Length > 1)
				&& (Request[Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_FOR_COURSE_LIST] == Constants.REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_COURSE_LIST)
				&& (DateTime.Parse(timing[0]) <= DateTime.Now)
				&& (DateTime.Parse(timing[1]) > DateTime.Now))
			{
				var wrDuringTakeOverProductsRepeater = GetWrappedControl<WrappedRepeater>(deliveryRepeaterItem, "rSubscriptionBoxDuringItemList");
				foreach (RepeaterItem takeOverProductRepeaterItem in wrDuringTakeOverProductsRepeater.Items)
				{
					var wddlTakeOverProductsList = GetWrappedControl<WrappedDropDownList>(takeOverProductRepeaterItem, "ddlTakeOverProductsList");
					if (string.IsNullOrEmpty(wddlTakeOverProductsList.SelectedValue))
					{
						this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
						var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR).CreateUrl();
						Response.Redirect(url);
					}
					var wtbQuantity = GetWrappedControl<WrappedTextBox>(takeOverProductRepeaterItem, "tbQuantity");

					var errorMessage = Validator.CheckHalfwidthNumberError("注文数", wtbQuantity.Text.Trim());
					errorMessage += Validator.CheckNecessaryError("注文数", wtbQuantity.Text.Trim());

					if (string.IsNullOrEmpty(errorMessage) == false)
					{
						this.Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
						var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR).CreateUrl();
						Response.Redirect(url);
					}
				}
			}
		}
	}

	/// <summary>
	/// 一回目の商品をカート全て投入処理
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	protected void AddFirstProductsToCart()
	{
		var subscriptionBoxService = new SubscriptionBoxService();
		this.SubscriptionBox = subscriptionBoxService.GetByCourseId(this.SubscriptionBoxCourseId);

		var fisrtCountProducts = GetFirstProducts();

		var errorMessage = string.Empty;
		foreach (var product in fisrtCountProducts)
		{
			// 商品情報取得
			var dvProduct = ProductCommon.GetProductVariationInfo(
				(string.IsNullOrEmpty(product.ShopId) == false) ? product.ShopId : this.ShopId,
				product.ProductId,
				product.VariationId,
				null);
			if (dvProduct.Count == 0)
			{
				this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// 商品在庫エラーは詳細画面に文言表示
			var productError = OrderCommon.CheckProductStatus(dvProduct[0], product.ItemQuantity, Constants.AddCartKbn.SubscriptionBox, this.LoginUserId);
			if (productError != OrderErrorcode.NoError)
			{
				switch (productError)
				{
					case OrderErrorcode.SellMemberRankError:
						errorMessage = OrderCommon.GetErrorMessage(
							productError,
							CreateProductJointName(dvProduct[0]),
							MemberRankOptionUtility.GetMemberRankName((string)dvProduct[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]));
						break;

					default:
						errorMessage = OrderCommon.GetErrorMessage(productError, CreateProductJointName(dvProduct[0]));
						break;
				}

				this.Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			if (string.IsNullOrEmpty(errorMessage))
			{
				if (Constants.CART_LIST_LP_OPTION == false)
				{
					// カート投入（商品付帯情報は何も選択されていない状態でカート投入）
					var cartList = GetCartObjectList();

					cartList.AddProduct(
						dvProduct[0],
						Constants.AddCartKbn.SubscriptionBox,
						StringUtility.ToEmpty(dvProduct[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]),
						product.ItemQuantity,
						new ProductOptionSettingList(),
						null,
						null,
						null,
						this.SubscriptionBoxCourseId);

					cartList.CartListShippingMethodUserUnSelected();
				}
				else
				{
					var cartListAbsoluteUri = new Uri(Request.Url.AbsoluteUri);
					var url = Constants.PATH_ROOT
						+ Constants.PAGE_FRONT_CART_LIST_LP
						+ cartListAbsoluteUri.Query;

					Response.Redirect(url);
				}
			}

		}

		// エラー画面へ
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
	}

	/// <summary>
	/// 商品をカート投入処理
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	protected void AddProductToCart(SubscriptionBoxDefaultItemModel fisrtCountProducts)
	{
		var subscriptionBoxService = new SubscriptionBoxService();
		this.SubscriptionBox = subscriptionBoxService.GetByCourseId(this.SubscriptionBoxCourseId);

		var errorMessage = string.Empty;

		// 商品情報取得
		var dvProduct = ProductCommon.GetProductVariationInfo(
			(string.IsNullOrEmpty(fisrtCountProducts.ShopId) == false) ? fisrtCountProducts.ShopId : this.ShopId,
			fisrtCountProducts.ProductId,
			fisrtCountProducts.VariationId,
			null);
		if (dvProduct.Count == 0)
		{
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_NO_ITEM);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		// 商品在庫エラーは詳細画面に文言表示
		var productError = OrderCommon.CheckProductStatus(
			dvProduct[0],
			fisrtCountProducts.ItemQuantity,
			Constants.AddCartKbn.SubscriptionBox,
			this.LoginUserId);
		if (productError != OrderErrorcode.NoError)
		{
			errorMessage += OrderCommon.GetErrorMessage(
				productError,
				CreateProductJointName(dvProduct[0]),
				MemberRankOptionUtility.GetMemberRankName(
					(string)dvProduct[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]));
			errorMessage += "\n";

			switch (productError)
			{
				case OrderErrorcode.SellMemberRankError:
					errorMessage += OrderCommon.GetErrorMessage(
						productError,
						CreateProductJointName(dvProduct[0]),
						MemberRankOptionUtility.GetMemberRankName(
							(string)dvProduct[0][Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]));
					break;

				default:
					errorMessage += OrderCommon.GetErrorMessage(productError, CreateProductJointName(dvProduct[0]));
					break;
			}

			this.Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		if (string.IsNullOrEmpty(errorMessage))
		{
			// カート投入（商品付帯情報は何も選択されていない状態でカート投入）
			var cartList = GetCartObjectList();

			cartList.AddProduct(
				dvProduct[0],
				Constants.AddCartKbn.SubscriptionBox,
				StringUtility.ToEmpty(dvProduct[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]),
				fisrtCountProducts.ItemQuantity,
				new ProductOptionSettingList(),
				null,
				null,
				null,
				this.SubscriptionBoxCourseId);

			cartList.CartListShippingMethodUserUnSelected();
		}
		else
		{
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

	}

	/// <summary>
	/// 初回配送商品の取得
	/// </summary>
	/// <returns>初回配送商品</returns>
	private SubscriptionBoxDefaultItemModel[] GetFirstProducts()
	{
		var firstProducts = ProductPage.GetFirstProducts(this.SubscriptionBox);
		return firstProducts;
	}

	/// <summary>
	/// 選択商品を追加
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddProduct_Click(object sender, EventArgs e)
	{
		this.SubscriptionBoxDuringItemList
			.Add(new SubscriptionBoxDefaultItemModel()
			{
				TakeOverProductBranchNo = this.TakeOverProductBranchNo,
				ItemQuantity = 1
			});

		this.TakeOverProductBranchNo++;
		CreateCopyTakeOverProductList();
		this.SubscriptionBoxDuringItemListCopy = this.SubscriptionBoxDuringItemList;

		Response.Redirect(Request.RawUrl);
	}

	/// <summary>
	/// 選択商品削除
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rProductChange_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		var deleteProduct = this.SubscriptionBoxDuringItemList[e.Item.ItemIndex];
		if (e.CommandName == "DeleteRow") this.SubscriptionBoxDuringItemList.Remove(deleteProduct);

		CreateCopyTakeOverProductList();

		Response.Redirect(Request.RawUrl);
	}

	/// <summary>
	/// 期間内の選択可能商品リストの生成と取得
	/// </summary>
	/// <returns>期間内の選択可能商品リスト</returns>
	protected ListItemCollection GetDuringItemsList()
	{
		var list = new ListItemCollection();

		list.Add(new ListItem(ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_SUBSCRIPTIONBOX,
			Constants.VALUETEXT_PARAM_SUBSCRIPTION_BOX_TAKE_OVER,
			string.Empty), string.Empty));

		list.AddRange(this.SubscriptionBox.SelectableProducts
			.Where(item => (item.SelectableSince <= DateTime.Now)
				&& (item.SelectableUntil > DateTime.Now)
				&& (string.IsNullOrEmpty(item.ProductId) == false)
				&& (string.IsNullOrEmpty(item.VariationId) == false))
			.Select(item => new ListItem(
				string.IsNullOrEmpty(item.VariationName1)
					? item.Name
					: CreateProductName(
						item.ProductId,
						item.Name,
						item.VariationName1,
						item.VariationName2,
						item.VariationName3),
				item.VariationId))
			.ToArray());
		return list;
	}

	/// <summary>
	/// 引継ぎ商品か判定
	/// </summary>
	/// <param name="productsList">商品リスト</param>
	/// <returns>引継ぎ商品か?</returns>
	protected bool CheckTakeOverProduct(List<SubscriptionBoxDefaultItemModel> productsList)
	{
		if (string.IsNullOrEmpty(productsList[0].ProductId)
			&& string.IsNullOrEmpty(productsList[0].VariationId))
		{
			this.IsTakeOverProduct = true;
			return true;
		}

		this.IsTakeOverProduct = false;
		return false;
	}

	/// <summary>
	/// 引継ぎ商品の期間内か判定
	/// </summary>
	/// <param name="deliveryTiming">配送タイミング</param>
	/// <returns>引継ぎ商品か?</returns>
	protected bool CheckDuringTakeOverProduct(string deliveryTiming)
	{
		var timing = deliveryTiming.Split('～');

		var result = ((timing.Length > 1)
			&& (DateTime.Parse(timing[0]) <= DateTime.Now)
			&& (DateTime.Parse(timing[1]) > DateTime.Now)
			&& this.IsTakeOverProduct);

		return result;
	}

	/// <summary>
	/// 選択可能商品ドロップダウンリストの選択値変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlValidDefaultItemList_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		CreateCopyTakeOverProductList();
		Response.Redirect(Request.RawUrl);
	}

	/// <summary>
	/// 引継ぎ商品リストのデータバインド
	/// </summary>
	private void DataBindDuringItemList()
	{
		var wrSubscriptionBoxDeliveryTiming = this.GetWrappedControl<WrappedRepeater>("rSubscriptionBoxDeliveryTiming");
		if (wrSubscriptionBoxDeliveryTiming.Items.Count > 0)
		{
			var deliveryRepeaterItem = wrSubscriptionBoxDeliveryTiming.Items[0];

			var wrDuringTakeOverProductsRepeater = GetWrappedControl<WrappedRepeater>(deliveryRepeaterItem, "rSubscriptionBoxDuringItemList");
			wrDuringTakeOverProductsRepeater.DataSource = this.SubscriptionBoxDuringItemList;
			wrDuringTakeOverProductsRepeater.DataBind();
		}

	}

	/// <summary>
	/// 引継ぎ商品の商品リストを複製
	/// </summary>
	private void CreateCopyTakeOverProductList()
	{
		if (this.SubscriptionBoxDuringItemList.Count == 0) return;

		foreach (RepeaterItem deliveryTimingItem in rSubscriptionBoxDeliveryTiming.Items)
		{
			var whfDeliveryTiming = GetWrappedControl<WrappedHiddenField>(deliveryTimingItem, "hfDeliveryTiming");
			var timing = whfDeliveryTiming.Value.Split('～');

			if ((timing.Length > 1)
				&& (Request[Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_FOR_COURSE_LIST] == Constants.REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_COURSE_LIST)
				&& (DateTime.Parse(timing[0]) <= DateTime.Now)
				&& (DateTime.Parse(timing[1]) > DateTime.Now))
			{
				var wrDuringTakeOverProductsRepeater = GetWrappedControl<WrappedRepeater>(deliveryTimingItem, "rSubscriptionBoxDuringItemList");
				foreach (RepeaterItem takeOverItem in wrDuringTakeOverProductsRepeater.Items)
				{
					foreach (var duringItem in this.SubscriptionBoxDuringItemList)
					{
						var wddlTakeOverProductsList = GetWrappedControl<WrappedDropDownList>(takeOverItem, "ddlTakeOverProductsList");
						var product = this.SubscriptionBox.SelectableProducts
							.Where(item => item.VariationId == wddlTakeOverProductsList.SelectedValue)
							.ToList();
						var whfTakeOverProductBranchNo = GetWrappedControl<WrappedHiddenField>(
							takeOverItem,
							"hfTakeOverProductBranchNo");
						if (duringItem.TakeOverProductBranchNo != int.Parse(whfTakeOverProductBranchNo.Value)) continue;

						if ((product.Count >= 1) && (string.IsNullOrEmpty(product[0].VariationId) == false))
						{
							var quantity = ((TextBox)takeOverItem.FindControl("tbQuantity")).Text.Trim();
							int quantityResult = 1;
							int.TryParse(quantity, out quantityResult);
							duringItem.VariationId = product[0].VariationId;
							duringItem.ItemQuantity = quantityResult;

							duringItem.Price = product[0].Price;
							duringItem.ProductId = product[0].ProductId;
							duringItem.ShopId = product[0].ShopId;
						}
						else
						{
							duringItem.VariationId = wddlTakeOverProductsList.SelectedValue;
						}
					}
				}
			}
		}
		this.SubscriptionBoxDuringItemListCopy = this.SubscriptionBoxDuringItemList;

	}

	/// <summary>
	/// 商品詳細ページURLの作成
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="subscriptionBoxDefaultItem">デフォルト商品</param>
	/// <returns>URL</returns>
	protected string GetProductDetailUrl(string productId, SubscriptionBoxDefaultItemModel subscriptionBoxDefaultItem)
	{
		return (string.IsNullOrEmpty(productId) == false)
			? CreateProductDetailUrl(subscriptionBoxDefaultItem, true)
			: string.Empty;
	}

	/// <summary>
	/// 商品名の作成
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="name">商品名</param>
	/// <param name="variationName1">バリエーション名1</param>
	/// <param name="variationName2">バリエーション名2</param>
	/// <param name="variationName3">バリエーション名3</param>
	/// <returns>商品名</returns>
	public string CreateProductName(
		string productId,
		string name,
		string variationName1,
		string variationName2,
		string variationName3)
	{
		if (string.IsNullOrEmpty(productId)) return string.Empty;

		var productName = name + (string.IsNullOrEmpty(variationName1)
			? ""
			: ProductCommon.CreateVariationName(
				variationName1,
				variationName2,
				variationName3));

		return productName;
	}

	/// <summary>
	/// 有効価格の取得
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <param name="price">価格</param>
	/// <param name="isNotCampaignPeriodPrice">true:キャンペーン期間でない、false:キャンペーン期間</param>
	/// <returns>有効価格</returns>
	protected string GetValidPrice(string productId, string variationId, decimal? price ,bool isNotCampaignPeriodPrice = false)
	{
		if (string.IsNullOrEmpty(productId)) return string.Empty;

		var product = GetProduct(Constants.CONST_DEFAULT_SHOP_ID, productId, variationId);

		if (product.Count <= 0) return string.Empty;

		var timing = this.DeliveryTiming.Split('～');

		var firstTimePrice =
			(product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE] == DBNull.Value)
				? string.Empty
				: product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE].ToString();

		var fixedPurchasePrice =
			(product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] == DBNull.Value)
				? string.Empty
				: product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE].ToString();

		var subscriptionBoxItem = this.SubscriptionBox.SelectableProducts.FirstOrDefault(
			x => (x.ProductId == productId)
				&& (x.VariationId == variationId));

		// キャンペーン期間であればキャンペーン期間価格を適用
		if (OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxItem) && (isNotCampaignPeriodPrice == false))
		{ 
			return CurrencyManager.ToPrice(subscriptionBoxItem.CampaignPrice.ToPriceString());
		}

		var validPrice = string.Empty;
		switch (this.SubscriptionBox.OrderItemDeterminationType)
		{
			case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME:
				validPrice = ((timing[0] == "1") && (string.IsNullOrEmpty(firstTimePrice) == false))
					? CurrencyManager.ToPrice(firstTimePrice)
					: (string.IsNullOrEmpty(fixedPurchasePrice) == false)
						? CurrencyManager.ToPrice(fixedPurchasePrice)
						: CurrencyManager.ToPrice(price);

				return validPrice;
				break;

			case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD:
				validPrice = ((DateTime.Parse(timing[0]) <= DateTime.Now)
						&& (DateTime.Parse(timing[1]) > DateTime.Now)
						&& (string.IsNullOrEmpty(firstTimePrice) == false))
					? CurrencyManager.ToPrice(firstTimePrice)
					: (string.IsNullOrEmpty(fixedPurchasePrice) == false)
						? CurrencyManager.ToPrice(fixedPurchasePrice)
						: CurrencyManager.ToPrice(price);

				return validPrice;
				break;

			default:
				validPrice = CurrencyManager.ToPrice(price);
				return validPrice;
		}
	}

	/// <summary>
	/// 商品名取得
	/// </summary>
	/// <param name="shopId">ショップI</param>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>商品名</returns>
	protected string GetProductName(string shopId, string productId, string variationId)
	{
		if (string.IsNullOrEmpty(productId)) return string.Empty;
		var result = new ProductService().GetProductVariation(shopId, productId, variationId, this.MemberRankId).Name;
		return result;
	}

	/// <summary>
	/// 頒布会商品リストの作成
	/// </summary>
	/// <returns>商品リスト</returns>
	protected ListItemCollection GetSubscriptionBoxProductList(string productId, string varitionId, string courseId)
	{
		var subscriptionBox = new SubscriptionBoxService().GetByCourseId(courseId);

		var subscriptionBoxFirstDefaultItem = (subscriptionBox.IsNumberTime) 
			? subscriptionBox.DefaultOrderProducts
				.Where(item => (item.Count == 1) && item.IsInSelectableTerm(DateTime.Now)).ToArray() 
			: subscriptionBox.DefaultOrderProducts
				.Where(item => item.IsInTerm(DateTime.Now)).ToArray(); 
		var optionalProducts = subscriptionBoxFirstDefaultItem
			.Where(product => product.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID)
			.ToArray();
		var list = new ListItemCollection
		{
			new ListItem("", this.ShopId + "//")
		};
		// 任意商品がなかった場合何も表示しない
		if (optionalProducts.Any() == false)
		{
			this.SubscriputionBoxProductList = new List<SubscriptionBoxDefaultItemModel>();
			this.SubscriputionBoxProductListModify = new List<SubscriptionBoxDefaultItemModel>();
			return list;
		}

		foreach (var selectableProduct in subscriptionBox.SelectableProducts)
		{
			selectableProduct.ProductName = GetProductName(
				this.ShopId,
				selectableProduct.ProductId,
				selectableProduct.VariationId);
		}
		var optionalListItems = optionalProducts
			.SelectMany(item => subscriptionBox.SelectableProducts, (defaultOrderProduct, selectableProduct) => new { defaultOrderProduct, selectableProduct })
			.Where(product => (product.defaultOrderProduct.VariationId == product.selectableProduct.VariationId))
			.Select(result => new ListItem(
				result.selectableProduct.ProductName,
				string.Format(
					"{0}/{1}/{2}",
					this.ShopId,
					result.defaultOrderProduct.ProductId,
					result.defaultOrderProduct.VariationId)))
			.ToArray();
		list.AddRange(optionalListItems);
		return list;
	}

	/// <summary>
	/// 商品を追加
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddAnyProduct_Click(object sender, EventArgs e)
	{
		GetWrappedControl<WrappedHtmlGenericControl>("sErrorQuantity").InnerText = "";
		this.SubscriputionBoxProductList.Add(
			new SubscriptionBoxDefaultItemModel
			{
				SubscriptionBoxCourseId = this.SubscriptionBoxCourseId,
				Count = null,
				TermSince = null,
				TermUntil = null,
				ShopId = this.ShopId,
				ProductId = string.Empty,
				ItemQuantity = 1,
				VariationId = string.Empty,
				BranchNo = 0,
				NecessaryProductFlg = Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID
			});
		BindItemModifyData();
	}

	/// <summary>
	/// 任意商品の更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateProduct_Click(object sender, EventArgs e)
	{
		// 個数チェック
		var isQuantityError = this.SubscriputionBoxProductList.Any(product => product.ItemQuantity == 0);
		if (isQuantityError)
		{
			GetWrappedControl<WrappedHtmlGenericControl>("sErrorQuantity").InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_QUANTITY_UPDATE_ALERT);
			return;
		}
		var subscriptionBox = new SubscriptionBoxService().GetByCourseId(this.SubscriptionBoxCourseId);

		var subscriptionBoxFirstDefaultItem = (subscriptionBox.IsNumberTime)
			? subscriptionBox.DefaultOrderProducts
				.Where(item => (item.Count == 1))
				.ToArray()
			: subscriptionBox.DefaultOrderProducts
				.Where(
					item => ((item.TermSince <= DateTime.Now)
						&& (item.TermUntil >= DateTime.Now)))
				.ToArray();

		var necessaryProducts = subscriptionBoxFirstDefaultItem
			.Where(product => (product.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID))
			.ToArray();

		var totalQuantity = necessaryProducts.Sum(product => product.ItemQuantity);
		totalQuantity += this.SubscriputionBoxProductList
			.SelectMany(optionalProduct => necessaryProducts, (optionalOrderProduct, necessaryProduct) => new { optionalOrderProduct, necessaryProduct })
			.Where(product => (product.optionalOrderProduct.VariationId == product.necessaryProduct.VariationId))
			.Sum(result => result.optionalOrderProduct.ItemQuantity);
		var wsErrorMessage = OrderCommon.CheckLimitProductOrderForSubscriptionBox(this.SubscriptionBoxCourseId, totalQuantity);
		if (string.IsNullOrEmpty(wsErrorMessage) == false)
		{
			GetWrappedControl<WrappedHtmlGenericControl>("sErrorQuantity").InnerText = wsErrorMessage;
			return;
		}

		// 重複チェック
		var variationIds = new List<string>();
		foreach (var product in this.SubscriputionBoxProductList)
		{
			if (string.IsNullOrEmpty(product.VariationId)) continue;
			if (variationIds.Contains(product.VariationId))
			{
				GetWrappedControl<WrappedHtmlGenericControl>("sErrorQuantity").InnerText = "error";
				return;
			}
			variationIds.Add(product.VariationId);
		}

		foreach (RepeaterItem repeaterItem in this.WrSubscriptionBoxDeliveryTiming.Controls)
		{
			((HtmlGenericControl)repeaterItem.FindControl("dvListProduct")).Visible = true;
			((HtmlGenericControl)repeaterItem.FindControl("dvModifySubscription")).Visible = false;
		}
		this.SubscriputionBoxProductListModify = this.SubscriputionBoxProductList
			.Where(p => (string.IsNullOrEmpty(p.ProductId) == false))
			.ToList();
		this.SubscriputionBoxProductList = this.SubscriputionBoxProductListModify;
		Response.Redirect(Request.Url.AbsoluteUri);
	}

	/// <summary>
	/// 商品削除
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rAnyProductChange_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if (e.CommandName == "DeleteRow") this.SubscriputionBoxProductList.RemoveAt(e.Item.ItemIndex);
		BindItemModifyData();
	}

	/// <summary>
	/// 再計算
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ReCalculation(object sender, EventArgs e)
	{
		GetWrappedControl<WrappedHtmlGenericControl>("sErrorQuantity").InnerText = "";
		var repeater = new RepeaterItemCollection(null);
		foreach (RepeaterItem repeaterItem in this.WrSubscriptionBoxDeliveryTiming.Controls)
		{
			repeater = ((Repeater)repeaterItem.FindControl("rItemModify")).Items;
			if (repeater != new RepeaterItemCollection(null)) break;
		}

		var productList = new List<string>();
		var quantityList = new List<int>();
		foreach (RepeaterItem roii in repeater)
		{
			var quantityString = ((TextBox)roii.FindControl("tbQuantityUpdate")).Text;
			productList.Add(((DropDownList)roii.FindControl("ddlProductName")).SelectedValue);

			int quantity;
			if (string.IsNullOrEmpty(quantityString) || (int.TryParse(quantityString, out quantity) == false) || (quantity < 1))
			{
				quantity = 1;
			}

			quantityList.Add(quantity);
		}

		var count = 0;
		foreach (var list in this.SubscriputionBoxProductList)
		{
			if (productList[count] == null) break;
			list.ItemQuantity = quantityList[count];
			list.ShopId = this.ShopId;
			list.ProductId = productList[count].Split('/')[1];
			list.VariationId = productList[count].Split('/')[2];
			count++;
		}
		BindItemModifyData();
	}

	/// <summary>
	/// 金額取得
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <param name="count">個数</param>
	/// <returns>金額</returns>
	protected decimal SubscriptionBoxPrice(string productId, string variationId, int count)
	{
		if (string.IsNullOrEmpty(productId) || string.IsNullOrEmpty(variationId)) return 0;

		var subscriptionBoxItem = this.SubscriptionBox.SelectableProducts.FirstOrDefault(
			x => (x.ProductId == productId)
				&& (x.VariationId == variationId));

		// キャンペーン期間であればキャンペーン期間価格を適用
		if (OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxItem))
		{
			return decimal.Parse(subscriptionBoxItem.CampaignPrice.ToPriceString());
		}

		var price = new ProductService().GetProductVariation(this.ShopId, productId, variationId, this.MemberRankId).Price * count;
		return price;
	}

	/// <summary>
	/// 商品変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnChangeProduct_Click(object sender, EventArgs e)
	{
		this.HasOptionalProdects = (this.HasOptionalProdects == false);
		foreach (RepeaterItem repeaterItem in this.WrSubscriptionBoxDeliveryTiming.Controls)
		{
			var whgcListProduct = GetWrappedControl<WrappedHtmlGenericControl>(repeaterItem, "dvListProduct");
			var whgcModifySubscription = GetWrappedControl<WrappedHtmlGenericControl>(repeaterItem, "dvModifySubscription");
			whgcListProduct.Visible = false;
			whgcModifySubscription.Visible = true;
		}
		BindItemModifyData();
	}

	/// <summary>
	/// 任意商品データバインド
	/// </summary>
	protected void BindItemModifyData()
	{
		foreach (RepeaterItem repeaterItem in this.WrSubscriptionBoxDeliveryTiming.Items)
		{
			var wrItemModify = GetWrappedControl<WrappedRepeater>(repeaterItem, "rItemModify");
			wrItemModify.DataSource = this.SubscriputionBoxProductList;
			wrItemModify.DataBind();
		}
	}

	/// <summary>
	/// 必須商品判定
	/// </summary>
	/// <param name="courseId">頒布会コース</param>
	/// <returns>結果</returns>
	protected bool CanNecessaryProducts(string courseId)
	{
		if (string.IsNullOrEmpty(courseId)) return false;
		var subscriptionBox = new SubscriptionBoxService().GetByCourseId(courseId);

		var subscriptionBoxFirstDefaultItem = (subscriptionBox.IsNumberTime)
			? subscriptionBox.DefaultOrderProducts
				.Where(item => (item.Count == 1)
					&& item.IsInSelectableTerm(DateTime.Now))	
				.ToArray()
			: subscriptionBox.DefaultOrderProducts
				.Where(item => item.IsInTerm(DateTime.Now))
				.ToArray();

		if (subscriptionBoxFirstDefaultItem.All(item => item.IsNecessary)) return false;
		var result = subscriptionBoxFirstDefaultItem.Any(item => item.IsNecessary);
		return result;
	}

	/// <summary>
	/// 頒布会選択商品欄の表示判定
	/// </summary>
	/// <param name="rItem">リピーターアイテム</param>
	/// <returns>表示するか</returns>
	protected bool IsDispSelectProductList(RepeaterItem rItem)
	{
		if ((((IDataItemContainer)rItem).DisplayIndex != 0)) return false;

		var rProductsList = GetWrappedControl<WrappedRepeater>(rItem, "rProductsList");
		var productListItems = (List<SubscriptionBoxDefaultItemModel>)rProductsList.DataSource;
		return ((productListItems != null) && productListItems.Any());
	}

	/// <summary>
	/// 指定した商品が選択可能な商品か？
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns></returns>
	private bool IsSelectableProduct(string productId, string variationId)
	{
		var isSelectableProduct = ProductPage.IsSelectableProduct(this.SubscriptionBox, productId, variationId);
		return isSelectableProduct;
	}

	/// <summary>
	/// 商品詳細ページの情報からSubscriptionBoxDefaultItemModel生成
	/// </summary>
	/// <returns>SubscriptionBoxDefaultItemModel</returns>
	private SubscriptionBoxDefaultItemModel CreateSubscriptionBoxDefaultItemModelFromProductDetail()
	{
		var addSubscriptionBoxItem = this.SubscriptionBox.SelectableProducts
			.FirstOrDefault(selectableProduct =>
				(selectableProduct.ProductId == this.ProductId)
				&& (selectableProduct.VariationId == this.VariationId)
				&& selectableProduct.IsInTerm(DateTime.Now));

		// 遷移元の商品が選択可能商品に存在していない場合Nullを返す（異常な遷移の場合に発生する）
		if (addSubscriptionBoxItem == null) return null;

		var subscriptionBoxDefaultItem = new SubscriptionBoxDefaultItemModel()
		{
			SubscriptionBoxCourseId = this.SubscriptionBoxCourseId,
			Count = 0,
			TermSince = addSubscriptionBoxItem.SelectableSince,
			TermUntil = addSubscriptionBoxItem.SelectableUntil,
			ShopId = this.ShopId,
			ProductId = this.ProductId,
			ItemQuantity = int.Parse(Request[Constants.REQUEST_KEY_ITEM_QUANTITY]),
			VariationId = this.VariationId,
			BranchNo = addSubscriptionBoxItem.BranchNo,
			NecessaryProductFlg = Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_INVALID,
			Name = addSubscriptionBoxItem.Name,
			VariationName1 = addSubscriptionBoxItem.VariationName1,
			VariationName2 = addSubscriptionBoxItem.VariationName2,
			VariationName3 = addSubscriptionBoxItem.VariationName3
		};
		return subscriptionBoxDefaultItem;
	}

	/// <summary>
	/// 商品IDとバリエーションIDからキャンペーン期間かどうか
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>true:キャンペーン期間、false:キャンペーン期間でない</returns>
	protected bool IsSubscriptionBoxCampaignPeriodByProductIdAndVariationId(string productId, string variationId)
	{
		var subscriptionBoxItem = this.SubscriptionBox.SelectableProducts.FirstOrDefault(
			x => (x.ProductId == productId)
				&& (x.VariationId == variationId));
		return OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxItem);
	}

	/// <summary>頒布会コースID</summary>
	protected string SubscriptionBoxCourseId
	{
		get { return Request[Constants.REQUEST_KEY_SUBSCRIPTION_BOX_COURSE_ID]; }
	}
	/// <summary>頒布会</summary>
	protected SubscriptionBoxModel SubscriptionBox { get; set; }
	/// <summary>頒布会引継ぎ商品リスト</summary>
	protected List<SubscriptionBoxDefaultItemModel> SubscriptionBoxDuringItemList
	{
		get { return (List<SubscriptionBoxDefaultItemModel>)this.ViewState["SubscriptionBoxDuringItemList"]; }
		set { this.ViewState["SubscriptionBoxDuringItemList"] = value; }
	}
	/// <summary>頒布会引継ぎ商品リスト複製</summary>
	protected List<SubscriptionBoxDefaultItemModel> SubscriptionBoxDuringItemListCopy
	{
		get { return (List<SubscriptionBoxDefaultItemModel>)this.ViewState["SubscriptionBoxDuringItemListCopy"]; }
		set { this.ViewState["SubscriptionBoxDuringItemCopy"] = value; }
	}
	/// <summary>引継ぎ商品の枝番</summary>
	protected int TakeOverProductBranchNo
	{
		get { return (int?)ViewState["SubscriptionBoxDuringItemListCopy"] ?? 0; }
		set { this.ViewState["SubscriptionBoxDuringItemCopy"] = value; }
	}
	/// <summary>引継ぎ商品か？</summary>
	protected bool IsTakeOverProduct { set; get; }
	/// <summary>定額設定されている？</summary>
	protected bool IsFixedAmount
	{
		get { return ((this.SubscriptionBox.FixedAmount != null) && (this.SubscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE)); }
	}
	/// <summary>頒布会商品一覧リスト</summary>
	public List<SubscriptionBoxDefaultItemModel> SubscriputionBoxProductList
	{
		get { return (List<SubscriptionBoxDefaultItemModel>)Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_LIST]; }
		set { Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_LIST] = value; }
	}
	/// <summary>頒布会更新商品一覧リスト</summary>
	public List<SubscriptionBoxDefaultItemModel> SubscriputionBoxProductListModify
	{
		get { return (List<SubscriptionBoxDefaultItemModel>)Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_OPTIONAL_LIST]; }
		set { Session[Constants.SESSION_KEY_SUBSCRIPTION_BOX_OPTIONAL_LIST] = value; }
	}
	/// <summary>表示</summary>
	protected bool HasOptionalProdects
	{
		get { return (bool?)ViewState["HasOptionalProdects"] ?? true; }
		set { ViewState["HasOptionalProdects"] = value; }
	}
	/// <summary>配達タイミング</summary>
	private string DeliveryTiming { get; set; }
	/// <summary>商品詳細からの遷移か</summary>
	private bool IsFromProductDetail
	{
		get
		{
			return (Request[Constants.REQUEST_KEY_CART_SUBSCRIPTION_BOX_FOR_COURSE_LIST]
				== Constants.REDIRECT_TO_SUBSCRIPTION_BOX_DETAIL_FROM_PRODUCT_DETAIL);
		}
	}
	/// <summary>1回目商品が選択可能期間内か</summary>
	protected bool IsWithinSelectionPeriodFirstProduct { get; set; }
}
