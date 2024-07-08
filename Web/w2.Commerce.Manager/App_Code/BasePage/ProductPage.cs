/*
=========================================================================================================
  Module      : 商品共通ページ(ProductPage.cs)
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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.Preview;
using w2.App.Common.Product;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.Product;
using w2.Domain.ProductFixedPurchaseDiscountSetting;
using w2.Domain.ProductPrice;
using w2.Domain.ProductStock;
using w2.Domain.ProductStockHistory;
using w2.Domain.ProductVariation;

/// <summary>
/// ProductPage の概要の説明です
/// </summary>
public partial class ProductPage : BasePage
{
	/// <summary>Product Fixed Purchase Next Shipping Product Setting</summary>
	public const string PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING = "product_fixed_purchase_next_shipping_product_setting";
	/// <summary>Limited fixed purchase kbn setting</summary>
	public const string LIMITED_FIXED_PURCHASE_KBN_SETTING = "limited_fixed_purchase_kbn_setting";

	/// <summary>
	/// 商品名＋バリエーション名作成
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>商品名＋バリエーション名</returns>
	/// <remarks>バリエーションありなしを自動判定します。</remarks>
	public string CreateProductAndVariationName(DataRowView product)
	{
		return product[Constants.FIELD_PRODUCT_NAME] + CreateVariationName(product);
	}
	/// <summary>
	/// バリエーション名作成
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>バリエーション名</returns>
	/// <remarks>バリエーションありなしを自動判定します。</remarks>
	protected string CreateVariationName(DataRowView product)
	{
		if (product[Constants.FIELD_PRODUCT_USE_VARIATION_FLG] == DBNull.Value) return string.Empty;
		if ((string)product[Constants.FIELD_PRODUCT_USE_VARIATION_FLG] == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE)
		{
			return CreateVariationName(
				StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]),
				StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2]),
				StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]));
		}

		return string.Empty;
	}
	/// <summary>
	/// バリエーション名作成
	/// </summary>
	/// <param name="variationName1">バリエーション名１</param>
	/// <param name="variationName2">バリエーション名２</param>
	/// <param name="variationName3">バリエーション名3</param>
	/// <returns>バリエーション名</returns>
	protected string CreateVariationName(
		string variationName1,
		string variationName2,
		string variationName3)
	{
		return CreateVariationName(
			variationName1,
			variationName2,
			variationName3,
			Constants.CONST_PRODUCTVARIATIONNAME_PARENTHESIS_LEFT,
			Constants.CONST_PRODUCTVARIATIONNAME_PARENTHESIS_RIGHT,
			Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION);
	}
	/// <summary>
	/// バリエーション名作成
	/// </summary>
	/// <param name="variationName1">バリエーション名１</param>
	/// <param name="variationName2">バリエーション名２</param>
	/// <param name="variationName3">バリエーション名3</param>
	/// <param name="parenthesisLeft">左括弧</param>
	/// <param name="parenthesisRight">右括弧</param>
	/// <param name="punctuation">区切り文字</param>
	/// <returns>バリエーション名</returns>
	protected string CreateVariationName(
		string variationName1,
		string variationName2,
		string variationName3,
		string parenthesisLeft,
		string parenthesisRight,
		string punctuation)
	{
		return ProductCommon.CreateVariationName(
			variationName1,
			variationName2,
			variationName3,
			parenthesisLeft,
			parenthesisRight,
			punctuation);
	}

	/// <summary>
	/// Insert update product
	/// </summary>
	/// <param name="actionStatus">Action status</param>
	/// <param name="product">Product model</param>
	/// <param name="isAddProductStockHistory">Is add product stock history</param>
	/// <param name="accessor">SQL Accessor</param>
	public void InsertUpdateProduct(
		string actionStatus,
		ProductModel product,
		SqlAccessor accessor,
		bool isAddProductStockHistory = false)
	{
		// For case update action
		if (actionStatus == Constants.ACTION_STATUS_UPDATE)
		{
			// Update product process
			UpdateProduct(product, accessor, isAddProductStockHistory);
			return;
		}

		// Insert product process
		InsertProduct(product, accessor, isAddProductStockHistory);
	}

	/// <summary>
	/// Insert product
	/// </summary>
	/// <param name="product">Product model</param>
	/// <param name="isAddProductStockHistory">Is add product stock history</param>
	/// <param name="accessor">SQL Accessor</param>
	public void InsertProduct(
		ProductModel product,
		SqlAccessor accessor,
		bool isAddProductStockHistory = false)
	{
		// 商品情報登録
		DomainFacade.Instance.ProductService.Insert(product, accessor);

		// For case has set product prices
		if (product.HasProductPrices)
		{
			// Insert-update product price process
			InsertUpdateProductPrices(product.ProductPrices, accessor);
		}

		// バリエーションを使用する場合
		if (product.HasProductVariation)
		{
			// Insert-update product variation process
			InsertUpdateProductVariations(product.ProductVariations, accessor);
		}

		// Insert product tag process
		if (product.HasProductTag)
		{
			DomainFacade.Instance.ProductTagService.Insert(product.ProductTag, accessor);
		}

		// Insert product extend process
		if (product.HasProductExtend)
		{
			DomainFacade.Instance.ProductExtendService.Insert(product.ProductExtend, accessor);
		}

		// Insert mall exhibit config process
		if (Constants.MALLCOOPERATION_OPTION_ENABLED)
		{
			DomainFacade.Instance.MallExhibitsConfigService.Insert(product.MallExhibitsConfig, accessor);
		}

		// Insert product stocks process
		if (product.IsStockUnmanaged == false)
		{
			InsertProductStocks(product.ProductStocks, accessor, isAddProductStockHistory);
		}

		// Insert product fixed purchase discount setting
		if (product.HasProductFixedPurchaseDiscountSettings)
		{
			InsertProductFixedPurchaseDiscountSetting(
				product.ProductFixedPurchaseDiscountSettings,
				accessor);
		}
	}

	/// <summary>
	/// Update product
	/// </summary>
	/// <param name="product">Product model</param>
	/// <param name="isAddProductStockHistory">Is add product stock history</param>
	/// <param name="accessor">SQL Accessor</param>
	public void UpdateProduct(
		ProductModel product,
		SqlAccessor accessor,
		bool isAddProductStockHistory = false)
	{
		// 商品情報登録
		var productService = DomainFacade.Instance.ProductService;
		productService.UpdateForModify(product, product.LastChanged, accessor);

		// For case has set product prices
		if (product.HasProductPrices)
		{
			// Insert-update product price process
			InsertUpdateProductPrices(product.ProductPrices, accessor);
		}
		else
		{
			// Delete product price all for product
			DeleteProductPrice(
				product.ShopId,
				product.ProductId,
				string.Empty,
				string.Empty,
				accessor);
		}

		// バリエーションを使用する場合
		if (product.HasProductVariation)
		{
			// Insert-update product variation process
			InsertUpdateProductVariations(product.ProductVariations, accessor);
		}

		// Get delete product variations and delete in database
		var ignoredVariationIds = product.HasProductVariations
			? product.ProductVariations.Select(item => item.VariationId).ToArray()
			: new[] { product.ProductId };
		var deleteProductVariations = productService.GetDeleteProductVariations(
			product.ShopId,
			product.ProductId,
			ignoredVariationIds,
			accessor);
		if (deleteProductVariations.Length != 0)
		{
			// Delete product variations
			productService.DeleteProductVariations(
				product.ShopId,
				product.ProductId,
				ignoredVariationIds,
				accessor);

			// Delete all product price for variations
			foreach (var item in deleteProductVariations)
			{
				DeleteProductPrice(
					item.ShopId,
					item.ProductId,
					item.VariationId,
					string.Empty,
					accessor);
			}
		}

		// Update product tag process
		var productTagService = DomainFacade.Instance.ProductTagService;
		productTagService.Delete(product.ProductId, accessor);
		if (product.HasProductTag)
		{
			productTagService.Insert(product.ProductTag, accessor);
		}

		// Update product extend process
		var productExtendService = DomainFacade.Instance.ProductExtendService;
		productExtendService.Delete(
			product.ShopId,
			product.ProductId,
			accessor);
		if (product.HasProductExtend)
		{
			productExtendService.Insert(product.ProductExtend, accessor);
		}

		// Update mall exhibit config process
		if (Constants.MALLCOOPERATION_OPTION_ENABLED)
		{
			var mallExhibitConfigService = DomainFacade.Instance.MallExhibitsConfigService;
			mallExhibitConfigService.Delete(
				product.ShopId,
				product.ProductId,
				accessor);

			mallExhibitConfigService.Insert(product.MallExhibitsConfig, accessor);
		}

		// Update product stocks process
		if (product.IsStockUnmanaged == false)
		{
			// Insert new product stocks
			InsertProductStocks(
				product.ProductStocks,
				accessor,
				isAddProductStockHistory);

			// Delete stock for deleted product or variation
			DomainFacade.Instance.ProductStockService.DeleteProductStock(
				product.ShopId,
				product.ProductId,
				ignoredVariationIds,
				accessor);

			// Insert stock history for deleted product or variation
			foreach (var item in deleteProductVariations)
			{
				var productStockHistory = new ProductStockHistoryModel
				{
					ShopId = item.ShopId,
					ProductId = item.ProductId,
					VariationId = item.VariationId,
					ActionStatus = Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_PRODUCTVAIRATION_DELETE,
					LastChanged = product.LastChanged,
				};
				DomainFacade.Instance.ProductStockHistoryService.Insert(productStockHistory, accessor);
			}
		}
		else
		{
			// Delete all product stock
			DeleteProductStockAll(
				product.ShopId,
				product.ProductId,
				accessor);
		}

		// Update product fixed purchase discount setting
		DomainFacade.Instance.ProductFixedPurchaseDiscountSettingService.DeleteByShopIdAndProductId(
			product.ShopId,
			product.ProductId,
			accessor);
		if (product.HasProductFixedPurchaseDiscountSettings)
		{
			InsertProductFixedPurchaseDiscountSetting(
				product.ProductFixedPurchaseDiscountSettings,
				accessor);
		}
	}

	/// <summary>
	/// Insert update product prices
	/// </summary>
	/// <param name="productPrices">Product price models</param>
	/// <param name="accessor">SQL accessor</param>
	private void InsertUpdateProductPrices(ProductPriceModel[] productPrices, SqlAccessor accessor)
	{
		if ((productPrices == null) || (productPrices.Length == 0)) return;

		var productPriceService = DomainFacade.Instance.ProductPriceService;
		foreach (var item in productPrices)
		{
			if (item.IsSetProductPrice)
			{
				var isExist = productPriceService.IsExist(
					item.ShopId,
					item.ProductId,
					item.VariationId,
					item.MemberRankId,
					accessor);
				if (isExist)
				{
					productPriceService.UpdateForModify(item, item.LastChanged, accessor);
				}
				else
				{
					productPriceService.Insert(item, accessor);
				}
			}
			else
			{
				DeleteProductPrice(
					item.ShopId,
					item.ProductId,
					item.VariationId,
					item.MemberRankId,
					accessor);
			}
		}
	}

	/// <summary>
	/// Delete product price
	/// </summary>
	/// <param name="shopId">Shop id</param>
	/// <param name="productId">Product id</param>
	/// <param name="variationId">Variation id</param>
	/// <param name="memberRankId">MemberRank id</param>
	/// <param name="accessor">SQL Accessor</param>
	private void DeleteProductPrice(
		string shopId,
		string productId,
		string variationId,
		string memberRankId,
		SqlAccessor accessor)
	{
		DomainFacade.Instance.ProductPriceService.Delete(
			shopId,
			productId,
			variationId,
			memberRankId,
			accessor);
	}

	/// <summary>
	/// Insert update product variations
	/// </summary>
	/// <param name="productVariations">Product variation models</param>
	/// <param name="accessor">SQL accessor</param>
	private void InsertUpdateProductVariations(
		ProductVariationModel[] productVariations,
		SqlAccessor accessor)
	{
		if ((productVariations == null) || (productVariations.Length == 0)) return;

		var productService = DomainFacade.Instance.ProductService;
		foreach (var variation in productVariations)
		{
			// Insert-update product variation process
			var isVariationExist = productService.IsProductVariationExist(
				variation.ShopId,
				variation.ProductId,
				variation.VariationId,
				accessor);
			if (isVariationExist)
			{
				productService.UpdateProductVariationForModify(
					variation,
					variation.LastChanged,
					accessor);
			}
			else
			{
				productService.InsertProductVariation(variation, accessor);
			}

			if (variation.HasProductPrices == false) continue;

			// Insert-update product price for variation process
			InsertUpdateProductPrices(variation.ProductPrices, accessor);
		}
	}

	/// <summary>
	/// Insert product stocks
	/// </summary>
	/// <param name="productStocks">Product stock models</param>
	/// <param name="isAddProductStockHistory">Is add product stock history</param>
	/// <param name="accessor">SQL accessor</param>
	private void InsertProductStocks(
		ProductStockModel[] productStocks,
		SqlAccessor accessor,
		bool isAddProductStockHistory = false)
	{
		if ((productStocks == null) || (productStocks.Length == 0)) return;

		var productStockService = DomainFacade.Instance.ProductStockService;
		foreach (var stock in productStocks)
		{
			var isExist = productStockService.IsExist(
				stock.ShopId,
				stock.ProductId,
				stock.VariationId,
				accessor);
			if (isExist) continue;

			productStockService.Insert(stock, accessor);

			if (isAddProductStockHistory == false) continue;

			// For case has insert stock history
			var productStockHistory = new ProductStockHistoryModel
			{
				ShopId = stock.ShopId,
				ProductId = stock.ProductId,
				VariationId = stock.VariationId,
				ActionStatus = Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_OPERATION,
				AddStock = stock.Stock,
				UpdateMemo = stock.UpdateMemo,
				LastChanged = stock.LastChanged,
			};

			DomainFacade.Instance.ProductStockHistoryService.Insert(productStockHistory, accessor);
		}
	}

	/// <summary>
	/// Delete product stock all
	/// </summary>
	/// <param name="shopId">Shop id</param>
	/// <param name="productId">Product id</param>
	/// <param name="accessor">SQL accessor</param>
	protected void DeleteProductStockAll(string shopId, string productId, SqlAccessor accessor)
	{
		DomainFacade.Instance.ProductStockService.DeleteProductStockAll(
			shopId,
			productId,
			accessor);
	}

	/// <summary>
	/// Insert product fixed purchase discount setting
	/// </summary>
	/// <param name="pfpDiscountSettings">Product fixed purchase discount setting models</param>
	/// <param name="accessor">SQL accessor</param>
	private void InsertProductFixedPurchaseDiscountSetting(
		ProductFixedPurchaseDiscountSettingModel[] pfpDiscountSettings,
		SqlAccessor accessor)
	{
		var pfpDiscountSettingService = DomainFacade.Instance.ProductFixedPurchaseDiscountSettingService;
		foreach (var pfpDiscountSetting in pfpDiscountSettings)
		{
			pfpDiscountSettingService.Insert(pfpDiscountSetting, accessor);
		}
	}

	/// <summary>
	/// 商品詳細プレビュー情報登録（編集、確認画面で利用）
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	/// <param name="productInput">Product input</param>
	public void InsertProductDetailPreview(
		string actionStatus,
		ProductInput productInput)
	{
		// プレビュー用データを以下の手順で取得
		// 1.登録・更新処理を実行
		// 2.対象データを取得
		// 3.「1.」の処理をロールバック
		DataTable product = null;
		var model = productInput.CreateModel();

		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();

			try
			{
				// 商品情報登録・更新
				InsertUpdateProduct(actionStatus, model, accessor);

				// 商品情報取得
				var tmpProduct = ProductPreview.GetProductDetailPreviewData(
					model.ShopId,
					model.ProductId,
					accessor);

				// 該当なしの場合、エラーページへ
				if (tmpProduct.Count == 0)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				// データ取得
				product = tmpProduct.Table;
			}
			finally
			{
				// 登録処理ロールバック
				accessor.RollbackTransaction();
			}
		}

		// 商品詳細プレビュー情報登録
		ProductPreview.InsertProductDetailPreview(model.ShopId, model.ProductId, product);
	}

	/// <summary>
	/// 商品一覧遷移URL作成
	/// </summary>
	/// <param name="htParam">検索情報</param>
	/// <returns>商品一覧遷移URL</returns>
	protected static string CreateProductListUrl(Hashtable htParam)
	{
		var result = new StringBuilder(CreateProductListUrlWithoutPageNo(htParam));
		if (htParam != null)
		{
			if (htParam.ContainsKey(Constants.REQUEST_KEY_PAGE_NO))
			{
				result.AppendFormat("&{0}={1}", Constants.REQUEST_KEY_PAGE_NO, HttpUtility.UrlEncode((string)htParam[Constants.REQUEST_KEY_PAGE_NO]));
			}
		}

		return result.ToString();
	}
	/// <summary>
	/// 商品一覧遷移URL作成
	/// </summary>
	/// <param name="htParam">検索情報</param>
	/// <returns>商品一覧遷移URL</returns>
	protected static string CreateProductListUrlWithoutPageNo(Hashtable htParam)
	{
		var urlCreator = new w2.Common.Web.UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_LIST);
		if (htParam == null) return urlCreator.CreateUrl();

		urlCreator.AddParam(Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID, (string)htParam[Constants.REQUEST_KEY_PRODUCT_PRODUCT_ID])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_NAME, (string)htParam[Constants.REQUEST_KEY_PRODUCT_NAME])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SUPPLIER_ID, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SUPPLIER_ID]);

		Enumerable.Range(1, Constants.COOPERATION_ID_COLUMNS_COUNT).ToList().ForEach(i =>
			{
				var key = Constants.REQUEST_KEY_PRODUCT_COOPERATION_ID_HEAD + i.ToString();
				urlCreator.AddParam(key, (string)htParam[key]);
			});

		urlCreator.AddParam(Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SHIPPING_TYPE])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SHIPPING_SIZE_KBN, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SHIPPING_SIZE_KBN])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_DISPLAY_KBN, (string)htParam[Constants.REQUEST_KEY_PRODUCT_DISPLAY_KBN])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SELL_KBN, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SELL_KBN])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SEARCH_DISPLAY_KBN, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SEARCH_DISPLAY_KBN])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, (string)htParam[Constants.REQUEST_KEY_PRODUCT_VALID_FLG])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_CATEGORY_ID, (string)htParam[Constants.REQUEST_KEY_PRODUCT_CATEGORY_ID])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_MOBILE_DISP_FLG, (string)htParam[Constants.REQUEST_KEY_PRODUCT_MOBILE_DISP_FLG])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ICON_FLG1, (string)htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG1])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ICON_FLG2, (string)htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG2])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ICON_FLG3, (string)htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG3])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ICON_FLG4, (string)htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG4])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ICON_FLG5, (string)htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG5])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ICON_FLG6, (string)htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG6])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ICON_FLG7, (string)htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG7])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ICON_FLG8, (string)htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG8])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ICON_FLG9, (string)htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG9])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ICON_FLG10, (string)htParam[Constants.REQUEST_KEY_PRODUCT_ICON_FLG10])
			.AddParam(Constants.REQUEST_KEY_SORT_KBN, (string)htParam[Constants.REQUEST_KEY_SORT_KBN])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_MEMBER_RANK_DISCOUNT_FLG, (string)htParam[Constants.REQUEST_KEY_PRODUCT_MEMBER_RANK_DISCOUNT_FLG])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_DISP_MEMBER_RANK, (string)htParam[Constants.REQUEST_KEY_PRODUCT_DISP_MEMBER_RANK])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_BUYABLE_MEMBER_RANK, (string)htParam[Constants.REQUEST_KEY_PRODUCT_BUYABLE_MEMBER_RANK])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_BRAND_ID, (string)htParam[Constants.REQUEST_KEY_PRODUCT_BRAND_ID])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_DISPLAY_PRIORITY, (string)htParam[Constants.REQUEST_KEY_PRODUCT_DISPLAY_PRIORITY])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_FROM, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_FROM])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_FROM, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_FROM])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_TO, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_DATE_TO])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_TO, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SELL_FROM_TIME_TO])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_FROM, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_FROM])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_FROM, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_FROM])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_TO, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SELL_TO_DATE_TO])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_TO, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SELL_TO_TIME_TO])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_LIMITED_PAYMENT, (string)htParam[Constants.REQUEST_KEY_PRODUCT_LIMITED_PAYMENT])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE, (string)htParam[Constants.REQUEST_KEY_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_PRODUCT_TYPE, (string)htParam[Constants.REQUEST_KEY_PRODUCT_PRODUCT_TYPE])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_COLOR_ID, (string)htParam[Constants.REQUEST_KEY_PRODUCT_COLOR_ID])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_TAX_CATEGORY_ID, (string)htParam[Constants.REQUEST_KEY_PRODUCT_TAX_CATEGORY_ID])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG, (string)htParam[Constants.REQUEST_KEY_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG])
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG, (string)htParam[Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG]);

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// データバインド用商品編集URL作成
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="actionStatus">The action status</param>
	/// <param name="isShowBackButton">Is show back button</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateProductRegistUrl(string productId, string actionStatus, bool isShowBackButton = false)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_NEW_PRODUCT_REGISTER)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, productId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, actionStatus)
			.AddParam(
				Constants.REQUEST_KEY_HIDE_BACK_BUTTON,
				isShowBackButton
					? Constants.KBN_BACK_BUTTON_SHOWED
					: Constants.KBN_BACK_BUTTON_HIDDEN)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// Create product regist default setting url
	/// </summary>
	/// <returns>Product regist default setting url</returns>
	protected string CreateProductRegistDefaultSettingUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_REGISTER)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateProductDetailUrl(string productId)
	{
		return CreateProductDetailUrl(productId, false);
	}
	/// <summary>
	/// 商品詳細URL作成
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <param name="isPopUpAction">ポップアップさせるかどうか</param>
	/// <returns>商品詳細URL</returns>
	protected string CreateProductDetailUrl(string productId, bool isPopUpAction)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, productId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL);
		if (isPopUpAction)
		{
			urlCreator.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);
		}

		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// データバインド用文字列作成（モール出品設定情報）
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="mallExhibitsConfigList">モール出品設定値リスト</param>
	/// <returns>String value</returns>
	protected string CreateMallNameList(string shopId, List<KeyValuePair<string, string>> mallExhibitsConfigList)
	{
		var mallNameList = new StringBuilder();
		var mallCooperationSettings = DomainFacade.Instance.MallCooperationSettingService.GetAll(shopId);
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_MALLCOOPERATIONSETTING, Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG))
		{
			foreach (var kvpMallExhibitsConfig in mallExhibitsConfigList)
			{
				if ((kvpMallExhibitsConfig.Key == li.Value)
					&& (kvpMallExhibitsConfig.Value == Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON))
				{
					foreach (var model in mallCooperationSettings)
					{
						if (model.MallExhibitsConfig == li.Value)
						{
							if (mallNameList.Length != 0)
							{
								mallNameList.Append(",");
							}
							mallNameList.Append(model.MallName);
							break;
						}
					}
					break;
				}
			}
		}

		return mallNameList.ToString();
	}

	/// <summary>
	/// 商品詳細プレビュー用URL作成
	/// </summary>
	/// <param name="targetSite">対象サイト</param>
	/// <param name="productId">商品ID</param>
	/// <param name="brandId1">ブランドID1</param>
	/// <param name="guidString">Guid string</param>
	/// <returns>プレビュー用URL</returns>
	public string CreateUrlForProductDetailPreview(
		string targetSite,
		string productId,
		string brandId1 = null,
		string guidString = null)
	{
		// PCプレビュー？
		if (targetSite == "PC")
		{
			var urlPcCreator = new UrlCreator(Constants.URL_FRONT_PC + Constants.PAGE_FRONT_PRODUCT_DETAIL)
				.AddParam(Constants.REQUEST_KEY_FRONT_SHOP_ID, this.LoginOperatorShopId)
				.AddParam(Constants.REQUEST_KEY_FRONT_PRODUCT_ID, productId)
				.AddParam(Constants.REQUEST_KEY_FRONT_PREVIEW_HASH, ProductPreview.CreateProductDetailHash());
			if (string.IsNullOrEmpty(brandId1) == false)
			{
				urlPcCreator.AddParam(Constants.REQUEST_KEY_PRODUCT_BRAND_ID, brandId1);
			}
			if (string.IsNullOrEmpty(guidString) == false)
			{
				urlPcCreator.AddParam(Constants.REQUEST_KEY_FRONT_PREVIEW_GUID_STRING, guidString);
			}
			return urlPcCreator.CreateUrl();
		}
		// モバイルプレビュー？
		else if (targetSite == "Mobile")
		{
			var urlMobileCreator = new UrlCreator(Constants.URL_FRONT_MOBILE + Constants.PAGE_FRONT_PRODUCT_DETAIL)
				.AddParam(Constants.REQUEST_KEY_MOBILEFRONT_PAGE_ID, Constants.PAGEID_MFRONT_PRODUCT_DETAIL)
				.AddParam(Constants.REQUEST_KEY_FRONT_SHOP_ID, this.LoginOperatorShopId)
				.AddParam(Constants.REQUEST_KEY_MOBILEFRONT_PRODUCT_ID, productId)
				.AddParam(Constants.REQUEST_KEY_FRONT_PREVIEW_HASH, ProductPreview.CreateProductDetailHash());
			return urlMobileCreator.CreateUrl();
		}
		// スマートフォンプレビュー？
		else if (targetSite == "SmartPhone")
		{
			var spUrl = string.Format("{0}SmartPhone/{1}", Constants.URL_FRONT_PC, Constants.PAGE_FRONT_PRODUCT_DETAIL);
			var urlPcCreator = new UrlCreator(spUrl)
				.AddParam(Constants.REQUEST_KEY_FRONT_SHOP_ID, this.LoginOperatorShopId)
				.AddParam(Constants.REQUEST_KEY_FRONT_PRODUCT_ID, productId)
				.AddParam(Constants.REQUEST_KEY_FRONT_PREVIEW_HASH, ProductPreview.CreateProductDetailHash());
			if (string.IsNullOrEmpty(brandId1) == false)
			{
				urlPcCreator.AddParam(Constants.REQUEST_KEY_PRODUCT_BRAND_ID, brandId1);
			}
			if (string.IsNullOrEmpty(guidString) == false)
			{
				urlPcCreator.AddParam(Constants.REQUEST_KEY_FRONT_PREVIEW_GUID_STRING, guidString);
			}
			return urlPcCreator.CreateUrl();
		}

		return string.Empty;
	}

	/// <summary>
	/// Get Product Name For Fixed Purchase Next Shipping Setting
	/// </summary>
	/// <param name="product">Product</param>
	/// <returns>Product Name</returns>
	protected string GetProductNameForFixedPurchaseNextShippingSetting(object product)
	{
		if (product == null) return string.Empty;

		var shopId = StringUtility.ToEmpty(GetKeyValue(product, Constants.FIELD_PRODUCT_SHOP_ID));
		var productId = StringUtility.ToEmpty(GetKeyValue(product, Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID));
		var variationId = StringUtility.ToEmpty(GetKeyValue(product, Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID));
		if (string.IsNullOrEmpty(productId)) return string.Empty;

		var productInfo = ProductCommon.GetProductVariationInfo(
			shopId,
			productId,
			variationId,
			string.Empty);

		if (productInfo.Count == 0) return string.Empty;

		return CreateProductAndVariationName(productInfo[0]);
	}

	/// <summary>
	/// Created Variation Id For Display
	/// </summary>
	/// <param name="product">Product</param>
	/// <returns>Variation Id For Display</returns>
	protected string CreatedVariationIdForDisplay(object product)
	{
		var result = string.Empty;
		var productId = StringUtility.ToEmpty(GetKeyValue(product, Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID));
		var variationId = StringUtility.ToEmpty(GetKeyValue(product, Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID));
		if (string.IsNullOrEmpty(productId) && string.IsNullOrEmpty(variationId)) return result;

		if ((productId != variationId) && (variationId.Length > productId.Length))
		{
			result = variationId.Substring(productId.Length);
		}

		return result;
	}

	/// <summary>
	/// Create Next Shipping Item Fixed Purchase Setting Message
	/// </summary>
	/// <param name="nextShippingItemFixedPurchaseKbn">Next Shipping Item Fixed Purchase Kbn</param>
	/// <param name="nextShippingItemFixedPurchaseSetting">Next Shipping Item Fixed Purchase Setting</param>
	/// <returns>Next Shipping Item Fixed Purchase Setting Message</returns>
	protected string CreateNextShippingItemFixedPurchaseSettingMessage(
		string nextShippingItemFixedPurchaseKbn,
		string nextShippingItemFixedPurchaseSetting)
	{
		var message = OrderCommon.CreateFixedPurchaseSettingMessage(
			nextShippingItemFixedPurchaseKbn,
			nextShippingItemFixedPurchaseSetting);
		return message;
	}

	/// <summary>
	/// Create Next Shipping Item Fixed Purchase Setting Message
	/// </summary>
	/// <param name="product">Product</param>
	/// <returns>Next Shipping Item Fixed Purchase Setting Message</returns>
	protected string CreateNextShippingItemFixedPurchaseSettingMessage(object product)
	{
		if (product == null) return string.Empty;

		var nextShippingItemFixedPurchaseKbn = StringUtility.ToEmpty(
			GetKeyValue(product, Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN));
		var nextShippingItemFixedPurchaseSetting = StringUtility.ToEmpty(
			GetKeyValue(product, Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING));
		var message = CreateNextShippingItemFixedPurchaseSettingMessage(
			nextShippingItemFixedPurchaseKbn,
			nextShippingItemFixedPurchaseSetting);
		return message;
	}

	#region 入力チェック系処理
	/// <summary>
	/// 商品付帯情報入力チェック
	/// </summary>
	/// <param name="productOptionSettings">商品付帯情報</param>
	/// <returns>エラーメッセージ</returns>
	public string CheckProductOptionValue(ProductOptionSettingList productOptionSettings)
	{
		var errorMessages = new StringBuilder();
		var loop = 1;
		foreach (ProductOptionSetting pos in productOptionSettings)
		{
			var optionName = string.Format(
				"{0}{1}",
				ReplaceTag("@@DispText.product_option.ProductOptionNamePrefix@@"),
				loop);

			// 項目名、初期値、設定値には全角スペースと全角：は入力させない
			if (StringUtility.ToEmpty(pos.ValueName).Contains('　')
				|| StringUtility.ToEmpty(pos.ValueName).Contains('：')
				|| StringUtility.ToEmpty(pos.DefaultValue).Contains('　')
				|| StringUtility.ToEmpty(pos.DefaultValue).Contains('：')
				|| ((pos.SettingValues != null)
					&& (pos.SettingValues.Any(s => s.Contains("　"))
						|| pos.SettingValues.Any(s => s.Contains("：")))))
			{
				errorMessages.AppendFormat(
					"{0}<br />",
					WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_SETTING)
						.Replace("@@ 1 @@", optionName));
			}

			// 付帯価格ありのケースでpatternを分岐
			var productOptionSettingText = pos.GetProductOptionSettingString();
			var errorMessage = Validator.CheckRegExpError(
				optionName,
				productOptionSettingText,
				Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED && (pos.IsDropDownPrice || pos.IsCheckBoxPrice)
					? Constants.REGEX_PATTERN_PRODUCT_OPTION_STRICT
					: @"\[\[(?:S|C|T)@@[^@]+(?:@@[^@]+)*\]\]",
				string.Empty);

			// チェックボックス設定値が設定されていない場合
			if (pos.IsCheckBox
				&& (pos.SettingValues != null)
				&& (pos.SettingValues.Count == 0))
			{
				errorMessages.AppendFormat(
					"{0}<br />",
					WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_NO_OPTIONS_SET_ERROR)
						.Replace("@@ 1 @@", optionName));
			}

			// ドロップダウンリストの設定値が設定されていないまたはデフォルト値のみの場合
			if (pos.IsSelectMenu && (pos.SettingValues != null) && ((pos.SettingValues.Count == 0)
					|| ((pos.SettingValues.Count == 1) && (string.IsNullOrEmpty(pos.DefaultValue) == false))))
			{
				errorMessages.AppendFormat(
					"{0}<br />",
					WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_NO_OPTIONS_SET_ERROR)
						.Replace("@@ 1 @@", optionName));
			}

			if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED
				&& string.IsNullOrEmpty(errorMessage))
			{
				var optionPriceSettings = productOptionSettingText.Replace(",", string.Empty);
				var arrayDistinct = optionPriceSettings
					.Replace(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SETTING_VALUE, ",")
					.Replace(Constants.PRODUCT_OPTION_TERMINATING_CHARACTER, string.Empty)
					.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				var isOptionPrice = arrayDistinct[0].Contains(Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_DROPDOWNMENU)
					|| arrayDistinct[0].Contains(Constants.PRODUCTOPTIONVALUES_DISP_KBN_PRICE_CHECKBOX);
				if (isOptionPrice)
				{
					var optionText = arrayDistinct.Skip(2).ToArray();

					foreach (var text in optionText)
					{
						if (Regex.IsMatch(text, Constants.REGEX_PATTERN_OPTION_PRICE_SEARCH_PATTERN) == false)
						{
							errorMessage = MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_NUMBER_OPTION_PRICE).Replace("@@ 1 @@", optionName);
							break;
						}
					}

					for (var index = 0; index < optionText.Length; index++)
					{
						var temStr = optionText[index];
						if (temStr.Contains(Constants.PRODUCT_OPTION_PRICE_PREFIX_FOR_DB))
						{
							optionText[index] = temStr.Substring(0, temStr.IndexOf(Constants.PRODUCT_OPTION_PRICE_PREFIX_FOR_DB, StringComparison.Ordinal));
						}
					}

					if (optionText.Length != optionText.Distinct().Count())
					{
						errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_DUPLICATE_VALUE_SETTING).Replace("@@ 1 @@", optionName);
					}
				}
			}

			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				errorMessages.AppendFormat(
					"{0}<br />",
					errorMessage);
			}

			loop++;
		}

		return errorMessages.ToString();
	}

	/// <summary>
	/// 定期購入割引設定入力チェック
	/// </summary>
	/// <param name="pfpDiscountSettings">定期割引設定</param>
	/// <returns>エラーメッセージ</returns>
	public string CheckProductFixedPurchaseDiscountSetting(List<ProductFixedPurchaseDiscountSettingHeader> pfpDiscountSettings)
	{
		var errorMessages = new StringBuilder();
		var numberDispTextOfLine = ValueText.GetValueText(
			Constants.TABLE_PRODUCTFIXEDPURCHASEDISCOUNTSETTING,
			Constants.VALUETEXT_PARAM_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_NUMBER_DISP_TEXT,
			Constants.VALUETEXT_PARAM_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_NUMBER_DISP_TEXT_LINE);
		var numberDispTextOfColumn = ValueText.GetValueText(
			Constants.TABLE_PRODUCTFIXEDPURCHASEDISCOUNTSETTING,
			Constants.VALUETEXT_PARAM_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_NUMBER_DISP_TEXT,
			Constants.VALUETEXT_PARAM_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_NUMBER_DISP_TEXT_COLUMN);

		// 個数入力チェック
		var productCountList = new List<Hashtable>();
		foreach (var col in pfpDiscountSettings[0].ProductCountDiscounts)
		{
			productCountList.Add
			(
				new Hashtable
				{
					{ Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_PRODUCT_COUNT, col.ProductCount }
				}
			);
		}

		var colCountForProductCountCheck = 1;
		foreach (var productCount in productCountList)
		{
			errorMessages.Append(
				Validator.Validate("ProductFixedPurchaseDiscountSettingRegist", productCount)
					.Replace(
						"@@ ColNo @@",
						colCountForProductCountCheck.ToString() + numberDispTextOfColumn));
			colCountForProductCountCheck++;
		}

		// 購入回数入力チェック対象リスト
		var orderCountList = new List<Hashtable>();

		// 値引き、付与ポイント入力形式チェック対象リスト
		var discountSettingList = new List<List<Hashtable>>();

		foreach (var row in pfpDiscountSettings)
		{
			orderCountList.Add
			(
				new Hashtable
				{
					{ Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_ORDER_COUNT, row.OrderCount }
				}
			);

			var discountSettingRow = new List<Hashtable>();
			foreach (var col in row.ProductCountDiscounts)
			{
				discountSettingRow.Add
				(
					new Hashtable
					{
						{
							Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_VALUE + "_price",
							(col.DiscountType == Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_YEN) ? col.DiscountValue : null
						},
					{
							Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_VALUE + "_percent",
							(col.DiscountType == Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_PERCENT) ? col.DiscountValue : null
						},
						{ Constants.FIELD_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_VALUE, col.PointValue }
					}
				);
			}

			discountSettingList.Add(discountSettingRow);
		}

		// 購入回数入力チェック
		var rowCountForOrderCountCheck = 1;
		foreach (var orderCount in orderCountList)
		{
			errorMessages.Append(
				Validator.Validate("ProductFixedPurchaseDiscountSettingRegist", orderCount)
					.Replace(
						"@@ RowNo @@",
						rowCountForOrderCountCheck.ToString() + numberDispTextOfLine));
			rowCountForOrderCountCheck++;
		}

		// 値引き、付与ポイント入力形式チェック
		var rowCountForSettingFormatCheck = 1;
		foreach (var discountSettingRow in discountSettingList)
		{
			var settingColCount = 1;
			foreach (var discountSettingCol in discountSettingRow)
			{
				errorMessages.Append(
					Validator.Validate("ProductFixedPurchaseDiscountSettingRegist", discountSettingCol)
						.Replace(
							"@@ ColNoRowNo @@",
							rowCountForSettingFormatCheck.ToString()
								+ numberDispTextOfLine
								+ settingColCount.ToString()
								+ numberDispTextOfColumn));
				settingColCount++;
			}
			rowCountForSettingFormatCheck++;
		}

		// 回数行に対する割引設定入力チェック
		var rowCountForDiscountSettingCheck = 1;
		foreach (var row in pfpDiscountSettings)
		{
			var inputtedCellCount = row.ProductCountDiscounts
				.Count(col =>
					((string.IsNullOrEmpty(col.DiscountValue) == false)
					|| (string.IsNullOrEmpty(col.PointValue) == false)));

			if (inputtedCellCount == 0)
			{
				errorMessages.Append(
					string.Format(
						CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_PRODUCT_MASTER_FIXED_PURCHASE_DISCOUNT_ROW_COUNT_EMPTY),
						rowCountForDiscountSettingCheck));
			}

			rowCountForDiscountSettingCheck++;
		}

		// 個数列に対する割引設定入力チェック
		var colDiscountSettingCounts = new int[pfpDiscountSettings[0].ProductCountDiscounts.Count];
		foreach (var row in pfpDiscountSettings)
		{
			var colCount = 0;
			foreach (var col in row.ProductCountDiscounts)
			{
				if ((string.IsNullOrEmpty(col.DiscountValue) == false)
					|| (string.IsNullOrEmpty(col.PointValue) == false))
				{
					colDiscountSettingCounts[colCount]++;
				}
				colCount++;
			}
		}

		var colCountForDiscountSettingCheck = 1;
		foreach (var colDiscountSettingCount in colDiscountSettingCounts)
		{
			if (colDiscountSettingCount == 0)
			{
				errorMessages.AppendFormat(
					"{0}<br />",
					WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_NOT_SETTING_DISCOUNT_FOR_PRODUCT_FIXEDPURCHASE_DISCOUNT_SETTING)
						.Replace("@@ 1 @@", colCountForDiscountSettingCheck.ToString()));
			}
			colCountForDiscountSettingCheck++;
		}

		if (IsDuplicationOrderCount(pfpDiscountSettings))
		{
			errorMessages.AppendFormat(
				"{0}<br />",
				WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_DUPLICATION_ORDER_COUNT_FOR_PRODUCT_FIXEDPURCHASE_DISCOUNT_SETTING));
		}

		if (IsDuplicationProductCount(pfpDiscountSettings[0].ProductCountDiscounts))
		{
			errorMessages.AppendFormat(
				"{0}<br />",
				WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_DUPLICATION_PRODUCT_COUNT_FOR_PRODUCT_FIXEDPURCHASE_DISCOUNT_SETTING));
		}

		// 割引設定パーセントの場合の最大値(100)チェック
		var rowCount = 1;
		int parsedDiscountValue;
		int parsedPointValue;
		foreach (var row in pfpDiscountSettings)
		{
			var colCount = 1;
			foreach (var col in row.ProductCountDiscounts)
			{
				if ((col.DiscountType == Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_DISCOUNT_TYPE_PERCENT)
					&& (int.TryParse(col.DiscountValue, out parsedDiscountValue))
					&& (parsedDiscountValue > 100))
				{
					errorMessages.AppendFormat(
						"{0}<br />",
						WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_MAX_NUMBER_SETTING_DISCOUNT_FOR_PRODUCT_FIXEDPURCHASE_DISCOUNT_SETTING)
							.Replace("@@ 1 @@", rowCount + numberDispTextOfLine + colCount));
				}

				if ((col.PointType == Constants.FLG_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_POINT_TYPE_PERCENT)
					&& (int.TryParse(col.PointValue, out parsedPointValue))
					&& (parsedPointValue > 100))
				{
					errorMessages.AppendFormat(
						"{0}<br />",
						WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_MAX_POINT_SETTING_DISCOUNT_FOR_PRODUCT_FIXEDPURCHASE_DISCOUNT_SETTING)
							.Replace("@@ 1 @@", rowCount + numberDispTextOfLine + colCount));
				}

				colCount++;
			}
			rowCount++;
		}

		return errorMessages.ToString();
	}

	/// <summary>
	/// 定期購入割引 購入回数重複チェック
	/// </summary>
	/// <param name="pfpDiscountSettings">定期購入割引設定</param>
	/// <returns>重複している：true　重複していない：false</returns>
	private bool IsDuplicationOrderCount(List<ProductFixedPurchaseDiscountSettingHeader> pfpDiscountSettings)
	{
		var duplicateCount = pfpDiscountSettings
			.GroupBy(row => row.OrderCount)
			.Count(group => group.Count() > 1);
		return (duplicateCount > 0);
	}

	/// <summary>
	/// 定期購入割引 個数重複チェック
	/// </summary>
	/// <param name="pfpDiscountProductCount">定期購入割引 個数設定</param>
	/// <returns>重複している：true　重複していない：false</returns>
	private bool IsDuplicationProductCount(List<ProductFixedPurchaseDiscountSettingDetail> pfpDiscountProductCount)
	{
		var duplicateCount = pfpDiscountProductCount
			.GroupBy(col => col.ProductCount)
			.Count(group => group.Count() > 1);
		return (duplicateCount > 0);
	}

	/// <summary>
	/// 配送種別をチェックする
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="shippingId">配送種別ID</param>
	/// <param name="fixedPurchaseFlg">Fixed purchase flag</param>
	/// <returns>エラーメッセージ</returns>
	public string CheckShippingType(
		string shopId,
		string shippingId,
		string fixedPurchaseFlg)
	{
		var errorMessage = string.Empty;

		// 配送種別の存在チェック
		var shopShipping = DomainFacade.Instance.ShopShippingService.Get(shopId, shippingId);
		if (shopShipping == null)
		{
			errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_SHOP_SHIPPING_NOT_EXIST_ERROR);
		}
		// 定期購入フラグが「可能」「定期購入のみ」の場合は該当商品の配送種別をチェックする
		else if ((fixedPurchaseFlg != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
			&& (shopShipping.IsValidFixedPurchaseKbnFlg == false))
		{
			errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_PRODUCT_SHOP_SHIPPING_ERROR);
		}

		return errorMessage;
	}

	/// <summary>
	/// Check Input Product For Fixed Purchase Next Shipping Setting
	/// </summary>
	/// <param name="product">Product</param>
	/// <returns>Error message</returns>
	public string CheckInputProductForFixedPurchaseNextShippingSetting(Hashtable product)
	{
		if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED == false) return string.Empty;

		var nextShippingProductChecker = new ProductFixedPurchaseNextShippingSettingChecker(
			StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_SHOP_ID]),
			StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_PRODUCT_ID]),
			StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_SHIPPING_TYPE]),
			StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID]),
			StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID]),
			StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY]),
			StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN]),
			StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING]));

		var errorMessages = nextShippingProductChecker.Check();

		var result = string.Join(
			"<br />",
			errorMessages.Select(
				kv =>
					(kv.Key == CommerceMessages.ERRMSG_FIXED_PURCHASE_NEXT_PRODUCT_INVALID)
					? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_OR_PRODUCTVARIATION_NOT_EXIST)
					: kv.Value));

		return result;
	}
	#endregion

	/// <summary>商品タグ領域表示可否</summary>
	protected bool DisplayTagArea
	{
		get { return (ViewState["DisplayTagArea"] != null) ? (bool)ViewState["DisplayTagArea"] : false; }
		set { ViewState["DisplayTagArea"] = value; }
	}
	/// <summary>Request product ID</summary>
	protected string RequestProductId
	{
		get { return Request[Constants.REQUEST_KEY_PRODUCT_ID]; }
	}
	/// <summary>Is show back button</summary>
	protected bool IsShowBackButton
	{
		get { return (Request[Constants.REQUEST_KEY_HIDE_BACK_BUTTON] == Constants.KBN_BACK_BUTTON_SHOWED); }
	}
}
