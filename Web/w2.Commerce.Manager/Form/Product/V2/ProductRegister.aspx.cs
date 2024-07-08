/*
=========================================================================================================
  Module      : 商品情報登録ページ (New)(ProductRegister.aspx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Input;
using w2.App.Common.Manager;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Workflow;
using w2.App.Common.Product;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.FixedPurchase;
using w2.Domain.MenuAuthority.Helper;
using w2.Domain.Product;
using w2.Domain.ShopOperator;

/// <summary>
/// 商品情報登録ページ (New)
/// </summary>
public partial class Form_Product_V2_ProductRegister : ProductPage
{
	/// <summary>Display priority default value</summary>
	protected const int CONST_DISPLAY_PRIORITY_DEFAULT = 999;
	/// <summary>Max sell quantity default value</summary>
	protected const int CONST_MAX_SELL_QUANTITY_DEFAULT = 100;
	/// <summary>付帯情報インデックス番号置換タグ</summary>
	private const string CONST_PRODUCT_OPTION_INDEX_NUMBER_REPLACE_TAG = "{num}";
	/// <summary>商品付帯情報基本設定</summary>
	protected static readonly string CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING = string.Format(
		".product-register-option-toggle-content-basic-setting:eq({0})",
		CONST_PRODUCT_OPTION_INDEX_NUMBER_REPLACE_TAG);
	/// <summary>商品付帯情報基本設定：リスト</summary>
	protected static readonly string CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_LIST = string.Format(
		".product-register-option-toggle-content-basic-setting-style-list:eq({0})",
		CONST_PRODUCT_OPTION_INDEX_NUMBER_REPLACE_TAG);
	protected static readonly string CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_LIST_PRICE = string.Format(
		".product-register-option-toggle-content-basic-setting-style-list-price:eq({0})",
		CONST_PRODUCT_OPTION_INDEX_NUMBER_REPLACE_TAG);
	/// <summary>商品付帯情報基本設定：テキスト</summary>
	protected static readonly string CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_TEXT = string.Format(
		".product-register-option-toggle-content-basic-setting-style-text:eq({0})",
		CONST_PRODUCT_OPTION_INDEX_NUMBER_REPLACE_TAG);
	/// <summary>商品付帯情報基本設定：半角数字</summary>
	protected static readonly string CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_STYLE_NUMBER = string.Format(
		".product-register-option-toggle-content-basic-setting-style-number:eq({0})",
		CONST_PRODUCT_OPTION_INDEX_NUMBER_REPLACE_TAG);
	/// <summary>商品付帯情報基本設定：入力文字数制限(範囲指定)</summary>
	protected static readonly string CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_CHECK_STYLE_TEXT_RANGE = string.Format(
		".product-register-option-toggle-content-basic-setting-check-style-text-range:eq({0})",
		CONST_PRODUCT_OPTION_INDEX_NUMBER_REPLACE_TAG);
	/// <summary>商品付帯情報基本設定：入力文字数制限(固定)</summary>
	protected static readonly string CONST_PRODUCT_REGISTER_OPTION_TOGGLE_CONTENT_BASIC_SETTING_CHECK_STYLE_TEXT_FIXED = string.Format(
		".product-register-option-toggle-content-basic-setting-check-style-text-fixed:eq({0})",
		CONST_PRODUCT_OPTION_INDEX_NUMBER_REPLACE_TAG);
	/// <summary>商品付帯情報項目名</summary>
	protected static readonly string CONST_PRODUCT_OPTION_VALUE_NAME = string.Format("tbProductOptionValue{0}Name", CONST_PRODUCT_OPTION_INDEX_NUMBER_REPLACE_TAG);
	/// <summary>入力文字数制限１</summary>
	protected static readonly string CONST_FIXED_LENGTH_FIRST = string.Format("rblFixedLength{0}-1", CONST_PRODUCT_OPTION_INDEX_NUMBER_REPLACE_TAG);
	/// <summary>入力文字数制限２</summary>
	protected static readonly string CONST_FIXED_LENGTH_SECOND = string.Format("rblFixedLength{0}-2", CONST_PRODUCT_OPTION_INDEX_NUMBER_REPLACE_TAG);

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// Clear cache of browser
			ClearBrowserCache();

			// Display initialize components
			InitializeComponents();

			if (this.IsRedirectFromMenu && (this.IsBackFromConfirm == false))
			{
				Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO] = null;
				Session[Constants.SESSIONPARAM_KEY_PRODUCTVARIATION_INFO] = null;
				Session[Constants.SESSIONPARAM_KEY_PRODUCTEXTEND_INFO] = null;
				Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;
				Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;
				Session[Constants.SESSIONPARAM_KEY_UPLODED_MASTER] = null;
			}

			// 処理区分チェック
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			// Set and bind display data
			Display();
		}
	}

	/// <summary>
	/// Initialize components
	/// </summary>
	private void InitializeComponents()
	{
		// 商品カラー
		this.ProductColors = new List<ProductColor>(
			new[] { new ProductColor { Id = string.Empty, DispName = string.Empty } }
				.Concat(DataCacheControllerFacade.GetProductColorCacheController().GetProductColorList()));

		// Get product variation default setting
		this.ProductVariationDefaultSetting = CreateReportJsonString(new ProductVariationInput(GetProductVariationDefaultSettingValue()));

		ViewState[Constants.SESSION_KEY_PARAM_FOR_BACK2] = Session[Constants.SESSION_KEY_PARAM_FOR_BACK2];
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;
	}

	/// <summary>
	/// Set and bind display data
	/// </summary>
	private void Display()
	{
		// Get product data from session if session has set data
		if ((Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO] != null)
			&& (Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO] is ProductInput))
		{
			this.ProductInput = (ProductInput)Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO];
		}

		// Get product image data from session if session has set data
		if ((Session[Constants.SESSIONPARAM_KEY_UPLODED_MASTER] != null)
			&& (Session[Constants.SESSIONPARAM_KEY_UPLODED_MASTER] is UploadImageInput))
		{
			this.UploadImageInput = (UploadImageInput)Session[Constants.SESSIONPARAM_KEY_UPLODED_MASTER];
		}

		// Set display by action status
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
				if (this.ProductInput == null)
				{
					// Get product default setting
					this.ProductInput = new ProductInput(GetProductDefaultSettingValue());
				}

				// Set display by action status
				// Get product option setting
				this.ProductOptionSettingList = CreateReportJsonString(this.ProductInput.CreateProductOptionSettingList());
				this.ProductSelectVariationKbn = this.ProductInput.SelectVariationKbn;

				// When the product has setting mall exhibits config, then get product mall exhibits config
				if (this.ProductInput.HasMallExhibitsConfig)
				{
					this.MallExhibitsConfig = CreateReportJsonString(
						GetProductMallExhibitsConfig()
							.Select(item => new KeyValueItem(item.Key, item.Value))
							.ToArray());
				}

				// When the product has setting fixed purchase discount, then get product fixed purchase discount
				if (this.ProductInput.HasProductFixedPurchaseDiscountSettings)
				{
					this.ProductFixedPurchaseDiscountSetting = CreateReportJsonString(this.ProductInput.ProductFixedPurchaseDiscountSettings);
				}
				break;

			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				if (this.ProductInput == null)
				{
					// Get product and check product if exists
					var product = DomainFacade.Instance.ProductService.GetProductDetail(
						this.LoginOperatorShopId,
						this.RequestProductId);
					if (product == null)
					{
						Session[Constants.SESSION_KEY_ERROR_MSG] =
							WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
						Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					}
					this.ProductInput = new ProductInput(product);

					// Get product mall exhibits config
					var mallExhibitsConfigs =
						GetProductMallExhibitsConfig(this.LoginOperatorShopId, this.RequestProductId)
							.Select(item => new KeyValueItem(item.Key, item.Value))
							.ToArray();
					this.MallExhibitsConfig = CreateReportJsonString(mallExhibitsConfigs);

					// Get product fixed purchase discount and check setting if exists
					var productFixedPurchaseDiscountSettingModel =
						DomainFacade.Instance.ProductFixedPurchaseDiscountSettingService.GetByShopIdAndProductId(
							this.LoginOperatorShopId,
							this.RequestProductId);
					if (productFixedPurchaseDiscountSettingModel != null)
					{
						this.ProductFixedPurchaseDiscountSetting = CreateReportJsonString(
							ProductFixedPurchaseDiscountSettingHeader.CreateProductFixedPurchaseDiscountSettingHeader(
								productFixedPurchaseDiscountSettingModel));
					}
				}
				else
				{
					// When the product has setting mall exhibits config, then get product mall exhibits config
					if (this.ProductInput.HasMallExhibitsConfig)
					{
						this.MallExhibitsConfig = CreateReportJsonString(
							GetProductMallExhibitsConfig()
								.Select(item => new KeyValueItem(item.Key, item.Value))
								.ToArray());
					}

					// When the product has setting fixed purchase discount, then get product fixed purchase discount
					if (this.ProductInput.HasProductFixedPurchaseDiscountSettings)
					{
						this.ProductFixedPurchaseDiscountSetting = CreateReportJsonString(this.ProductInput.ProductFixedPurchaseDiscountSettings);
					}

					ViewState[Constants.SESSION_KEY_PARAM_FOR_BACK2] = 1;
				}

				// Get product option setting
				this.ProductOptionSettingList = CreateReportJsonString(
					this.ProductInput.CreateProductOptionSettingList());

				this.ProductSelectVariationKbn = this.ProductInput.SelectVariationKbn;
				break;

			default:
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}

		// 付帯情報設定
		var productOptionCount = Enumerable.Range(1, Constants.PRODUCTOPTIONVALUES_MAX_COUNT)
			.Where(item => GetProductDefaultSettingDisplayField(ProductOptionSettingHelper.GetProductOptionSettingKey(item)))
			.ToList();
		rProductOption.DataSource = productOptionCount;
		rProductOption.DataBind();
	}

	#region API methods
	/// <summary>
	/// Get product sub image settings
	/// </summary>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetProductSubImageSettings()
	{
		if (Constants.PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE == false)
		{
			return CreateReportJsonString(string.Empty);
		}

		var settings = DomainFacade.Instance.ProductSubImageSettingService.GetProductSubImageSettings(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.PRODUCTSUBIMAGE_MAXCOUNT);
		var responseData = settings.Select(setting =>
			new
			{
				ImageNo = setting.ProductSubImageNo,
				ImageName = setting.ProductSubImageName
			})
			.ToArray();

		return CreateReportJsonString(responseData);
	}

	/// <summary>
	/// Get select variation kbn settings
	/// </summary>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetSelectVariationKbnSettings()
	{
		return GetJsonReponseByValueText(
			Constants.TABLE_PRODUCT,
			Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN);
	}

	/// <summary>
	/// Get mall exhibits configs
	/// </summary>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetMallExhibitsConfigs()
	{
		var loginShopOperator = GetLoginShopOperator();
		var mallExhibitsConfigs = ValueText.GetValueItemList(
			Constants.TABLE_MALLCOOPERATIONSETTING,
			Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG);
		var mallCooperationSettings = DomainFacade.Instance.MallCooperationSettingService.GetAll(loginShopOperator.ShopId);
		var results = new List<KeyValueItem>();

		foreach (ListItem li in mallExhibitsConfigs)
		{
			foreach (var model in mallCooperationSettings)
			{
				if (model.MallExhibitsConfig == li.Value)
				{
					results.Add(new KeyValueItem(model.MallName, model.MallExhibitsConfig));
					break;
				}
			}
		}
		return CreateReportJsonString(results);
	}

	/// <summary>
	/// Get limit payments
	/// </summary>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetLimitPayments()
	{
		var loginShopOperator = GetLoginShopOperator();
		var paymentList = GetPaymentValidList(loginShopOperator.ShopId);
		var results = paymentList
			.Cast<DataRowView>()
			.Select(
				payment =>
					new KeyValueItem
					{
						Key = (string)payment[Constants.FIELD_PAYMENT_PAYMENT_NAME],
						Value = (string)payment[Constants.FIELD_PAYMENT_PAYMENT_ID]
					})
			.Where(item => (item.Value != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY))
			.ToArray();

		return GetJsonReponse(results);
	}

	/// <summary>
	/// Get limited fixed purchase kbn1 setting
	/// </summary>
	/// <param name="shippingType">Shipping type</param>
	/// <param name="productFieldName">Product field name</param>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetLimitedFixedPurchaseKbnSetting(string shippingType, string productFieldName)
	{
		var loginShopOperator = GetLoginShopOperator();
		var shopShipping = DomainFacade.Instance.ShopShippingService.GetShopShipping(loginShopOperator.ShopId, shippingType);
		if (shopShipping == null) return CreateReportJsonString(string.Empty);

		var showKbnSetting = false;
		var setting = string.Empty;

		switch (productFieldName)
		{
			case Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING:
				showKbnSetting = (shopShipping.IsValidFixedPurchaseKbn1Flg
					|| shopShipping.IsValidFixedPurchaseKbn2Flg);
				setting = shopShipping.FixedPurchaseKbn1Setting;
				break;

			case Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING:
				showKbnSetting = shopShipping.IsValidFixedPurchaseKbn3Flg;
				setting = shopShipping.FixedPurchaseKbn3Setting;
				break;

			case Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING:
				showKbnSetting = shopShipping.IsValidFixedPurchaseKbn4Flg;
				setting = shopShipping.FixedPurchaseKbn4Setting1;
				break;
		}

		if ((shopShipping.IsValidFixedPurchaseKbnFlg == false)
			|| (showKbnSetting == false))
		{
			return CreateReportJsonString(string.Empty);
		}

		var unit = ValueText.GetValueText(
			Constants.TABLE_PRODUCT,
			LIMITED_FIXED_PURCHASE_KBN_SETTING,
			productFieldName);
		var result = new List<KeyValueItem>();

		setting.Split(',')
			.Distinct()
			.Where(item => (string.IsNullOrEmpty(item) == false))
			.ToList()
			.ForEach(item =>
				result.Add(
					new KeyValueItem(
						string.Format(
							"{0}{1} ",
							item.Replace("(", string.Empty).Replace(")", string.Empty),
							unit),
						item)));

		return CreateReportJsonString(result);
	}

	/// <summary>
	/// Get member ranks
	/// </summary>
	/// <param name="addEmptyValue">Add empty value</param>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetMemberRanks(bool addEmptyValue)
	{
		var memberRanks = MemberRankOptionUtility.GetMemberRankList();
		var results = memberRanks.Select(item =>
			new KeyValueItem(
				item.MemberRankId,
				item.MemberRankName))
			.ToArray();

		return GetJsonReponse(results, addEmptyValue);
	}

	/// <summary>
	/// Get shipping types
	/// </summary>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetShippingTypes()
	{
		var shopId = GetLoginShopOperator().ShopId;
		var shippings = DomainFacade.Instance.ShopShippingService.GetAllShopShippings(shopId);
		var results = shippings.Select(item =>
			new KeyValueItem(
				item.ShippingId,
				item.ShopShippingName))
			.ToArray();

		return GetJsonReponse(results, (shippings.Length != 0));
	}

	/// <summary>
	/// Get shipping sizes
	/// </summary>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetShippingSizes()
	{
		return GetJsonReponseByValueText(
			Constants.TABLE_PRODUCT,
			Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN,
			true);
	}

	/// <summary>
	/// Get stock manangement kbns
	/// </summary>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetStockManagementKbns()
	{
		return GetJsonReponseByValueText(
			Constants.TABLE_PRODUCT,
			Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN);
	}

	/// <summary>
	/// Get product stock messages
	/// </summary>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetProductStockMessages()
	{
		var productStockMessages = DomainFacade.Instance.ProductStockMessageService
			.GetProductStockMessages(GetLoginShopOperator().ShopId);
		var results = productStockMessages
			.Select(item =>
				new KeyValueItem(
					item.StockMessageId,
					item.StockMessageName))
			.ToArray();

		return GetJsonReponse(results, true);
	}

	/// <summary>
	/// Get product tax categories
	/// </summary>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetProductTaxCategories()
	{
		var productTaxCategories = DomainFacade.Instance.ProductTaxCategoryService.GetAllTaxCategory();
		var result = productTaxCategories
			.Select(item =>
				new
				{
					TaxCategoryId = item.TaxCategoryId,
					TaxCategoryName = item.TaxCategoryName,
					TaxRate = item.TaxRate.ToString("F")
				})
			.ToArray();

		return CreateReportJsonString(result);
	}

	/// <summary>
	/// Get related products
	/// </summary>
	/// <param name="productIds">商品ID一覧</param>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetRelatedProducts(string[] productIds)
	{
		var products = productIds.Any()
			? DomainFacade.Instance.ProductService.GetProductsByProductIds(GetLoginShopOperator().ShopId, productIds)
			: Array.Empty<ProductModel>();
		var result = products
			.Select(item =>
				new
				{
					productId = item.ProductId,
					encodedProductName = HtmlSanitizer.HtmlEncode(item.Name),
					productImg = ProductImage.GetHtmlImageTag(
						item,
						ProductType.Product,
						SiteType.Pc,
						Constants.PRODUCTIMAGE_FOOTER_S)
				})
			.ToArray();

		return CreateReportJsonString(result);
	}

	/// <summary>
	/// 指定した値（商品ID・商品名称）で部分一致の商品情報取得
	/// </summary>
	/// <param name="searchWord">検索用値</param>
	/// <returns>JSON文字列に変換する商品情報</returns>
	[WebMethod]
	public static string GetProductsLikeIdOrName(string searchWord)
	{
		var products = DomainFacade.Instance.ProductService.GetProductsLikeIdOrName(searchWord);
		var result = products
			.Select(item =>
				new
				{
					productId = item.ProductId,
					encodedProductName = HtmlSanitizer.HtmlEncode(item.Name),
					productImg = ProductImage.GetHtmlImageTag(
						item,
						ProductType.Product,
						SiteType.Pc,
						Constants.PRODUCTIMAGE_FOOTER_S)
				})
			.ToArray();

		return CreateReportJsonString(result);
	}

	/// <summary>
	///商品情報取得
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>JSON文字列に変換する商品情報</returns>
	[WebMethod]
	public static string GetProduct(string productId)
	{
		var loginShopOperator = GetLoginShopOperator();
		var product = DomainFacade.Instance.ProductService.Get(loginShopOperator.ShopId, productId);
		if (product == null) return CreateReportJsonString(string.Empty);
		var result = new
		{
			productId = product.ProductId,
			encodedProductName = HtmlSanitizer.HtmlEncode(product.Name),
			productImg = ProductImage.GetHtmlImageTag(
				product,
				ProductType.Product,
				SiteType.Pc,
				Constants.PRODUCTIMAGE_FOOTER_S)
		};

		return CreateReportJsonString(result);
	}

	/// <summary>
	/// Get product categories
	/// </summary>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetProductCategories()
	{
		var productCategories = DomainFacade.Instance.ProductCategoryService.GetAll()
			.Select(item =>
				new
				{
					id = item.CategoryId,
					label = item.Name,
					indent = ((item.CategoryId.Length / Constants.CONST_CATEGORY_ID_LENGTH - 1) * 10)
				})
			.ToArray();

		return CreateReportJsonString(productCategories);
	}

	/// <summary>
	/// Get product brands
	/// </summary>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetProductBrands()
	{
		var brands = DomainFacade.Instance.ProductBrandService.GetProductBrands()
			.Select(item =>
				new
				{
					id = item.BrandId,
					label = item.BrandName
				})
			.ToArray();

		return CreateReportJsonString(brands);
	}

	/// <summary>
	/// Get product tag settings
	/// </summary>
	/// <param name="productId">Product id</param>
	/// <returns>A JSON string</returns>
	[WebMethod]
	public static string GetProductTagSettings(string productId)
	{
		var tagData = ProductTagUtility.GetProductTagData(productId);
		var productTags = DomainFacade.Instance.ProductTagService.GetProductTagSetting()
			.Select(item =>
				new
				{
					TagId = item.TagId,
					TagName = item.TagName,
					TagValue = StringUtility.ToEmpty(tagData[item.TagId])
				})
			.ToArray();

		return CreateReportJsonString(productTags);
	}

	/// <summary>
	/// Get master upload settings
	/// </summary>
	/// <returns>An upload settings as json string</returns>
	[WebMethod]
	public static string GetMasterUploadSettings()
	{
		var xdoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + Constants.FILE_XML_MASTER_UPLOAD_SETTING);
		var uploadSettings = xdoc.Descendants(Constants.XML_MASTERUPLOADSETTING_MASTER_ELEMENT)
			.Select(item =>
				new
				{
					Name = item.Element(Constants.XML_MASTERUPLOADSETTING_NAME_ELEMENT).Value,
					Directory = item.Element(Constants.XML_MASTERUPLOADSETTING_DIRECTORY_ELEMENT).Value
				});

		var results = new List<KeyValueItem>();
		foreach (var uploadSetting in uploadSettings)
		{
			switch (uploadSetting.Directory)
			{
				case Constants.TABLE_PRODUCTTAG:
					if (Constants.PRODUCT_TAG_OPTION_ENABLE)
					{
						results.Add(new KeyValueItem(uploadSetting.Name, uploadSetting.Directory));
					}
					break;

				case Constants.TABLE_PRODUCTCATEGORY:
					if (Constants.PRODUCT_CTEGORY_OPTION_ENABLE)
					{
						results.Add(new KeyValueItem(uploadSetting.Name, uploadSetting.Directory));
					}
					break;

				case Constants.TABLE_PRODUCTSTOCK:
					if (Constants.PRODUCT_SALE_OPTION_ENABLED)
					{
						results.Add(new KeyValueItem(uploadSetting.Name, uploadSetting.Directory));
					}
					break;

				case Constants.TABLE_PRODUCTSALEPRICE:
					if (Constants.PRODUCT_STOCK_OPTION_ENABLE)
					{
						results.Add(new KeyValueItem(uploadSetting.Name, uploadSetting.Directory));
					}
					break;

				case Constants.TABLE_PRODUCT:
				case Constants.TABLE_PRODUCTVARIATION:
				case Constants.TABLE_PRODUCTREVIEW:
				case Constants.MASTERUPLOADSETTING_ACTION_KBN_DELETEPRODUCTIMAGE:
					results.Add(new KeyValueItem(uploadSetting.Name, uploadSetting.Directory));
					break;
			}
		}

		return CreateReportJsonString(results);
	}

	/// <summary>
	/// Get user management levels
	/// </summary>
	/// <returns>An user management levels as json string</returns>
	[WebMethod]
	public static string GetUserManagementLevels()
	{
		var models = DomainFacade.Instance.UserManagementLevelService.GetAllList();
		var results = models
			.Select(model =>
				new KeyValueItem(
					model.UserManagementLevelName,
					model.UserManagementLevelId))
			.ToArray();

		return GetJsonReponse(results);
	}

	/// <summary>
	/// Get product join name
	/// </summary>
	/// <param name="productId">Product id</param>
	/// <param name="variationId">Variation Id</param>
	/// <returns>A product join name as json string</returns>
	[WebMethod]
	public static string GetProductJoinName(string productId, string variationId)
	{
		var loginShopOperator = GetLoginShopOperator();
		var product = DomainFacade.Instance.ProductService.GetProductVariationAtDataRowView(
			loginShopOperator.ShopId,
			productId,
			string.IsNullOrEmpty(variationId)
				? productId
				: (productId + variationId),
			string.Empty);

		if (product == null) return string.Empty;

		var productName = new ProductPage().CreateProductAndVariationName(product);
		return CreateReportJsonString(productName);
	}

	/// <summary>
	/// Create next shipping fixed purchase setting message
	/// </summary>
	/// <param name="fixedPurchaseKbn">Fixed purchase kbn</param>
	/// <param name="fixedPurchaseSetting">Fixed purchase setting</param>
	/// <returns>A next shipping fixed purchase setting message as json string</returns>
	[WebMethod]
	public static string CreateNextShippingFixedPurchaseSettingMessage(
		string fixedPurchaseKbn,
		string fixedPurchaseSetting)
	{
		var message = OrderCommon.CreateFixedPurchaseSettingMessage(
			fixedPurchaseKbn,
			fixedPurchaseSetting);

		return CreateReportJsonString(message);
	}

	/// <summary>
	/// Get product extends
	/// </summary>
	/// <returns>A JSON string</returns>
	protected string GetProductExtends()
	{
		var models = DomainFacade.Instance.ProductExtendSettingService.GetAll(this.LoginOperatorShopId);
		var results = models
			.Select(model =>
				new
				{
					ExtendNo = model.ExtendNo,
					ExtendName = model.Name,
					ExtendDiscrition = model.Discription,
					Extend = (this.ProductInput.HasProductExtend)
						? GetEscapeValue(this.ProductInput.ProductExtend.GetProductExtendValueWithDataSource(
							string.Format(
								"extend{0}",
								 model.ExtendNo)))
						: string.Empty
				})
			.ToArray();

		return CreateReportJsonString(results);
	}

	/// <summary>
	/// Get product colors
	/// </summary>
	/// <returns>Product colors</returns>
	protected string GetProductColors()
	{
		var result = this.ProductColors
			.Select(color =>
				new
				{
					DispName = color.DispName,
					Id = color.Id,
					ImageUrl = ProductColorUtility.GetColorImageUrl(color.Id)
				})
			.ToArray();

		return CreateReportJsonString(result);
	}

	/// <summary>
	/// Get product member rank price
	/// </summary>
	/// <returns>Product member rank price</returns>
	protected string GetProductMemberRankPrice()
	{
		var memberRankPrices = DomainFacade.Instance.ProductPriceService.GetAll(
			this.LoginOperatorShopId,
			this.RequestProductId);
		var result = memberRankPrices
			.Select(price =>
				new
				{
					member_rank_id = price.MemberRankId,
					member_rank_price = price.MemberRankPrice.ToPriceString(),
					variation_id = price.VariationId
				})
			.ToList();

		if (this.IsBackFromConfirm)
		{
			if (this.ProductInput.HasProductPrices)
			{
				result = this.ProductInput.ProductPrices
					.Select(price =>
						new
						{
							member_rank_id = price.MemberRankId,
							member_rank_price = price.MemberRankPrice.ToPriceString(),
							variation_id = price.VariationId
						})
					.ToList();
			}

			if (this.ProductInput.HasProductVariations)
			{
				foreach (var productVariation in this.ProductInput.ProductVariations)
				{
					if (productVariation.HasProductPrices)
					{
						var productPrice = productVariation.ProductPrices
							.Select(price =>
								new
								{
									member_rank_id = price.MemberRankId,
									member_rank_price = price.MemberRankPrice.ToPriceString(),
									variation_id = price.VariationId
								})
							.ToList();
						result.AddRange(productPrice);
					}
				}
			}
		}

		return CreateReportJsonString(result);
	}

	/// <summary>
	/// Get product variations
	/// </summary>
	/// <returns>Product variations</returns>
	protected string GetProductVariation()
	{
		if (this.ProductInput.HasProductVariation == false) return CreateReportJsonString(new ProductVariationInput[0]);

		var variations = this.ProductInput.ProductVariations;
		return CreateReportJsonString(variations);
	}

	/// <summary>
	/// Get product images
	/// </summary>
	/// <returns>Product images</returns>
	protected string GetProductImages()
	{
		var result = new List<object>();
		var mainImage = (this.UploadImageInput != null)
			? this.UploadImageInput.MainImage
			: new UploadProductImageInput();

		result.Add(new
		{
			imageNo = 0,
			sourceIndex = 0,
			source = Constants.PRODUCT_IMAGE_HEAD_ENABLED == false ? GetProductMainImageSource() : string.Empty,
			delFlg = mainImage.DelFlg,
			fileName = mainImage.FileName
		});

		var subImageSettings = DomainFacade.Instance.ProductSubImageSettingService.GetProductSubImageSettings(
			this.LoginOperatorShopId,
			Constants.PRODUCTSUBIMAGE_MAXCOUNT);

		foreach (var setting in subImageSettings)
		{
			UploadProductSubImageInput subImage;
			if (this.UploadImageInput != null)
			{
				subImage = this.UploadImageInput.GetSubImage(setting.ProductSubImageNo.Value);
			}
			else
			{
				subImage = new UploadProductSubImageInput();
			}
			result.Add(new
			{
				imageNo = setting.ProductSubImageNo,
				sourceIndex = setting.ProductSubImageNo,
				source = Constants.PRODUCT_IMAGE_HEAD_ENABLED == false ? GetProductSubImageSource(setting.ProductSubImageNo.Value) : string.Empty,
				delFlg = subImage.DelFlg,
				fileName = subImage.FileName
			});
		}

		return CreateReportJsonString(result);
	}

	/// <summary>
	/// Get product main image source
	/// </summary>
	/// <returns>Product main image source</returns>
	private string GetProductMainImageSource()
	{
		// Back from confirm
		if (this.IsBackFromConfirm)
		{
			if ((this.UploadImageInput == null) || (this.UploadImageInput.MainImage.DelFlg)) return string.Empty;
			return ProductImage.GetImageSource(
				GetTempFilePath(
					this.UploadImageInput.MainImage.FileName,
					this.UploadImageInput.Guid),
				false);
		}
		// First load
		else
		{
			switch (this.ActionStatus)
			{
				case Constants.ACTION_STATUS_INSERT:
					return string.Empty;

				case Constants.ACTION_STATUS_COPY_INSERT:
				case Constants.ACTION_STATUS_UPDATE:
					return ProductImage.GetHtmlImageSource(
						this.ProductInput.ImageHead + Constants.PRODUCTIMAGE_FOOTER_LL,
						this.LoginOperatorShopId,
						SiteType.Pc,
						isNowPrinting: false);

				default:
					return string.Empty;
			}
		}
	}

	/// <summary>
	/// Get product sub image source
	/// </summary>
	/// <param name="imageNo"></param>
	/// <returns></returns>
	private string GetProductSubImageSource(int imageNo)
	{
		// Back from confirm
		if (this.IsBackFromConfirm)
		{
			if (this.UploadImageInput != null)
			{
				var subImage = this.UploadImageInput.GetSubImage(imageNo);
				if (subImage == null) return string.Empty;
				return subImage.DelFlg
					? string.Empty
					: ProductImage.GetImageSource(GetTempFilePath(subImage.FileName, this.UploadImageInput.Guid), false);
			}
			return string.Empty;
		}
		// First load
		else
		{
			switch (this.ActionStatus)
			{
				case Constants.ACTION_STATUS_INSERT:
					return string.Empty;

				case Constants.ACTION_STATUS_COPY_INSERT:
				case Constants.ACTION_STATUS_UPDATE:
					return ProductImage.GetHtmlImageSource(
						string.Format(
							"{0}{1}{2:00}{3}",
							this.ProductInput.ImageHead,
							Constants.PRODUCTSUBIMAGE_FOOTER,
							imageNo,
							Constants.PRODUCTIMAGE_FOOTER_LL),
						this.LoginOperatorShopId,
						SiteType.Pc,
						ImageType.Sub,
						false);

				default:
					return string.Empty;
			}
		}
	}

	/// <summary>
	/// Get product variation iamge source
	/// </summary>
	/// <param name="imageHead">Image head</param>
	/// <returns>Variation image source</returns>
	private string GetProductVariationImageSource(string imageHead)
	{
		// Back from confirm
		if (this.IsBackFromConfirm)
		{
			if (this.UploadImageInput != null)
			{
				var variationImage = this.UploadImageInput.VariationImages.FirstOrDefault(im => im.ImageHead == imageHead);
				return variationImage.DelFlg
					? string.Empty
					: ProductImage.GetImageSource(GetTempFilePath(variationImage.FileName, this.UploadImageInput.Guid), false);
			}
			return string.Empty;
		}
		// First load
		else
		{
			switch (this.ActionStatus)
			{
				case Constants.ACTION_STATUS_INSERT:
					return string.Empty;

				case Constants.ACTION_STATUS_COPY_INSERT:
				case Constants.ACTION_STATUS_UPDATE:
					return ProductImage.GetHtmlImageSource(
						imageHead + Constants.PRODUCTIMAGE_FOOTER_LL,
						this.LoginOperatorShopId,
						SiteType.Pc,
						isNowPrinting: false);

				default:
					return string.Empty;
			}
		}
	}

	/// <summary>
	/// Get temp file path
	/// </summary>
	/// <param name="fileName">File name</param>
	/// <param name="guid">Guid</param>
	/// <returns>Temp file path</returns>
	private string GetTempFilePath(string fileName, string guid)
	{
		if (string.IsNullOrEmpty(fileName)) return string.Empty;

		var path = Path.Combine(this.UploadImageTempPath, guid, fileName);
		return path;
	}

	/// <summary>
	/// Get variation images
	/// </summary>
	/// <returns>Variation images</returns>
	protected string GetVariationImages()
	{
		var variations = this.ProductInput.ProductVariations;
		var data = new List<object>();
		foreach (var variation in variations)
		{
			var variationImage = (this.UploadImageInput != null)
				? this.UploadImageInput.GetVariationImage(variation.VariationImageHead)
				: new UploadProductVariationImageInput();

			data.Add(new
				{
					source = Constants.PRODUCT_IMAGE_HEAD_ENABLED == false
						? GetProductVariationImageSource(variation.VariationImageHead)
						: string.Empty,
				delFlg = variationImage.DelFlg,
					fileName = variationImage.FileName
				});
		}

		return CreateReportJsonString(data);
	}

	/// <summary>
	/// Get product mall exhibits config
	/// </summary>
	/// <returns>Product mall exhibits config</returns>
	protected List<KeyValuePair<string, string>> GetProductMallExhibitsConfig()
	{
		if (this.ProductInput.HasMallExhibitsConfig == false) return new List<KeyValuePair<string, string>>();

		var mallExhibitsConfigs = ValueText.GetValueItemList(
			Constants.TABLE_MALLCOOPERATIONSETTING,
			Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG);

		var models = DomainFacade.Instance.MallCooperationSettingService.GetAll(this.LoginOperatorShopId);
		var results = new List<KeyValuePair<string, string>>();
		for (var index = 0; index < mallExhibitsConfigs.Count; index++)
		{
			foreach (var model in models)
			{
				var productExhibitsFlg = this.ProductInput.MallExhibitsConfig.GetExhibitsFlg(index + 1);
				if ((model.MallExhibitsConfig == mallExhibitsConfigs[index].Value)
					&& (productExhibitsFlg == Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON))
				{
					results.Add(
						new KeyValuePair<string, string>(
							model.MallExhibitsConfig,
							productExhibitsFlg));
					break;
				}
			}
		}

		return results;
	}

	/// <summary>
	/// Create product tag from input
	/// </summary>
	/// <returns>A string of product tag</returns>
	protected string CreateProductTagFromInput()
	{
		if (this.ProductInput.HasProductTag == false) return CreateReportJsonString(string.Empty);

		var length = this.ProductInput.ProductTag.ProductTagIds.Length;
		var productTags = new List<KeyValueItem>();
		for (var index = 0; index < length; index++)
		{
			var productTag = new KeyValueItem
			{
				Key = this.ProductInput.ProductTag.ProductTagIds[index],
				Value = this.ProductInput.ProductTag.ProductTagValues[index].Replace("\t", "\\t")
			};
			productTags.Add(productTag);
		}

		return CreateReportJsonString(productTags);
	}

	/// <summary>
	/// バリエーションIDが頒布会に含まれているか確認する。
	/// </summary>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>含まれているか</returns>
	protected bool CheckVariationIdIncludeSubscriptionBox(string variationId)
	{
		var results = DomainFacade.Instance.SubscriptionBoxService
			.GetSubscriptionItemByProductVariationId(
				this.LoginOperatorShopId,
				this.ProductInput.ProductId,
				variationId);
		return (results.Length != 0);
	}

	/// <summary>
	/// Get variation ids include subscription box string
	/// </summary>
	/// <returns>Variation ids include subscription box</returns>
	protected string GetVariationIdsIncludeSubscriptionBoxString()
	{
		var results = this.ProductInput.ProductVariations
			.Select(variation =>
				new KeyValueItem(
					variation.VId,
					CheckVariationIdIncludeSubscriptionBox(variation.VariationId).ToString()))
			.ToArray();
		return CreateReportJsonString(results);
	}

	/// <summary>
	/// 有効な定期台帳があるときに配送種別の変更を許可していいかチェックする
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>有効な定期台帳がある場合True</returns>
	protected bool CheckActiveFixedPurchase(string productId)
	{
		if ((this.ProductInput.FixedPurchaseFlg != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
			&& (this.ActionStatus == Constants.ACTION_STATUS_UPDATE))
		{
			var product = new ProductService().Get(this.ProductInput.ShopId, this.ProductInput.ProductId);
			if (this.ProductInput.ShippingType == product.ShippingType) return false;
			var validFixedPurchase = new FixedPurchaseService().HasActiveFixedPurchaseInfoByProductId(productId);
			if (validFixedPurchase) return true;
		}

		return false;
	}
	#endregion

	#region Helper Methods
	/// <summary>
	/// Get json response by value text
	/// </summary>
	/// <param name="tableName">The table name</param>
	/// <param name="fieldName">The field name</param>
	/// <param name="addEmptyValue">Add empty value</param>
	/// <returns>A json response</returns>
	public static string GetJsonReponseByValueText(
		string tableName,
		string fieldName,
		bool addEmptyValue = false)
	{
		var pairs = ValueText.GetValueKvpArray(tableName, fieldName);
		var result = new List<KeyValueItem>();

		if (addEmptyValue)
		{
			result.Add(new KeyValueItem(string.Empty, string.Empty));
		}

		foreach (var pair in pairs)
		{
			result.Add(new KeyValueItem(pair.Key, pair.Value));
		}
		return CreateReportJsonString(result);
	}

	/// <summary>
	/// Get json response
	/// </summary>
	/// <param name="keyValueItems">The key value items</param>
	/// <param name="addEmptyValue">Add empty value</param>
	/// <returns>A json response</returns>
	public static string GetJsonReponse(
		KeyValueItem[] keyValueItems,
		bool addEmptyValue = false)
	{
		var result = new List<KeyValueItem>();

		if (addEmptyValue)
		{
			result.Add(new KeyValueItem(string.Empty, string.Empty));
		}

		foreach (var item in keyValueItems)
		{
			result.Add(new KeyValueItem(item.Key, item.Value));
		}

		return CreateReportJsonString(result.ToArray());
	}

	/// <summary>
	/// Create opent popup script
	/// </summary>
	/// <param name="url">Url</param>
	/// <param name="name">Popup name</param>
	/// <param name="specs">Popup specifications</param>
	/// <param name="callback">Callback method</param>
	/// <returns></returns>
	private string CreateOpenPopupScript(string url, string name, string specs, string callback)
	{
		var script = string.Format(
			"openPopup(\'{0}\', \'{1}\', \'{2}\', {3});",
			url,
			name,
			specs,
			string.IsNullOrEmpty(callback) ? "''" : callback);

		return script;
	}

	/// <summary>
	/// Create product stock message list popup script
	/// </summary>
	/// <param name="callback">Callback method</param>
	/// <returns>The script open product stock message list popup</returns>
	protected string CreateOpenProductStockMessageListPopupScript(string callback = "")
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCKMESSAGE_LIST)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();

		var script = CreateOpenPopupScript(
			url,
			"product_stock_message",
			"width=850,height=700,top=120,left=420,status=NO,scrollbars=yes",
			callback);

		return script;
	}

	/// <summary>
	/// Create member rank list popup script
	/// </summary>
	/// <param name="callback">Callback method</param>
	/// <returns>The script open member rank list popup</returns>
	protected string CreateOpenMemberRankListPopupScript(string callback = "")
	{
		var url = SingleSignOnUrlCreator.CreateForWebForms(
			MenuAuthorityHelper.ManagerSiteType.Mp,
			new UrlCreator(Constants.PATH_ROOT_MP + Constants.PAGE_W2MP_MANAGER_MEMBER_RANK_LIST)
				.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
				.CreateUrl());

		var script = CreateOpenPopupScript(
			url,
			"member_rank",
			"width=850,height=700,top=120,left=420,status=NO,scrollbars=yes",
			callback);

		return script;
	}

	/// <summary>
	/// Create shipping list popup script
	/// </summary>
	/// <param name="callback">Callback method</param>
	/// <returns>The script open shipping list popup</returns>
	protected string CreateOpenShippingListPopupScript(string callback = "")
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_SHIPPING_LIST)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();

		var script = CreateOpenPopupScript(
			url,
			"shipping_type",
			"width=850,height=700,top=120,left=420,status=NO,scrollbars=yes",
			callback);

		return script;
	}

	/// <summary>
	/// Create open product search popup script
	/// </summary>
	/// <param name="callback">Callback method</param>
	/// <returns>The script open shipping list popup</returns>
	protected string CreateOpenProductSearchPopupScript(string callback = "")
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN, Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, Constants.FLG_PRODUCT_VALID_FLG_VALID)
			.CreateUrl();

		var script = CreateOpenPopupScript(
			url,
			"product_search",
			"width=850,height=700,top=120,left=420,status=NO,scrollbars=yes",
			callback);

		return script;
	}

	/// <summary>
	/// Create open product sub image setting list popup script
	/// </summary>
	/// <param name="callback">Callback method</param>
	/// <returns>The script open product sub image setting list popup</returns>
	protected string CreateOpenProductSubImageSettingListPopupScript(string callback = "")
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSUBIMAGESETTING_LIST)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();

		var script = CreateOpenPopupScript(
			url,
			"product_sub_image_setting",
			"width=850,height=700,top=120,left=420,status=NO,scrollbars=yes",
			callback);

		return script;
	}

	/// <summary>
	/// Create open product tag setting list popup script
	/// </summary>
	/// <param name="callback">Callback method</param>
	/// <returns>The script open product tag setting popup</returns>
	protected string CreateOpenProductTagSettingPopupScript(string callback = "")
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTTAGSETTING_LIST)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();

		var script = CreateOpenPopupScript(
			url,
			"product_tag_setting",
			"width=850,height=700,top=120,left=420,status=NO,scrollbars=yes",
			callback);

		return script;
	}

	/// <summary>
	/// Create open product category register popup script
	/// </summary>
	/// <param name="callback">Callback method</param>
	/// <returns>The script open product category register popup</returns>
	protected string CreateOpenProductCategoryRegisterPopupScript(string callback = "")
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTCATEGORY_REGISTER)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();

		var script = CreateOpenPopupScript(
			url,
			"product_category_register",
			"width=850,height=700,top=120,left=420,status=NO,scrollbars=yes",
			callback);

		return script;
	}

	/// <summary>
	/// Create open product variation matrix register popup script
	/// </summary>
	/// <param name="callback">Callback method</param>
	/// <returns>The script open product variation matrix register popup</returns>
	protected string CreateOpenProductVariationMatrixRegisterPopupScript(string callback = "")
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_VARIATION_MATRIX_REGISTER)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();

		var script = CreateOpenPopupScript(
			url,
			"product_variation_matrix_register",
			"width=850,height=615,top=120,left=320,status=NO,scrollbars=yes",
			callback);

		return script;
	}

	/// <summary>
	/// Create seo metadatas modify popup script
	/// </summary>
	/// <param name="callback">Callback method</param>
	/// <returns>The script open seo metadatas modify popup</returns>
	protected string CreateOpenSeoMetadatasModifyPopupScript(string callback = "")
	{
		var url = SingleSignOnUrlCreator.CreateForWebForms(
			MenuAuthorityHelper.ManagerSiteType.Cms,
			string.Format("{0}{1}/PopupModify",
				Constants.PATH_ROOT_CMS,
				Constants.CONTROLLER_W2CMS_MANAGER_SEO_METADATAS));

		var script = CreateOpenPopupScript(
			url,
			"seo_setting",
			"width=850,height=700,top=120,left=420,status=NO,scrollbars=yes",
			callback);

		return script;
	}

	/// <summary>
	/// Create open product brand list popup script
	/// </summary>
	/// <param name="callback">Callback method</param>
	/// <returns>The script open product brand list popup</returns>
	protected string CreateOpenProductBrandListPopupScript(string callback = "")
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTBRAND_LIST)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();

		var script = CreateOpenPopupScript(
			url,
			"produt_brand_setting",
			"width=850,height=615,top=120,left=320,status=NO,scrollbars=yes",
			callback);

		return script;
	}

	/// <summary>
	/// Create open payment list popup script
	/// </summary>
	/// <param name="callback">Callback method</param>
	/// <returns>The script open payment list popup</returns>
	protected string CreateOpenPaymentListPopupScript(string callback = "")
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAYMENT_LIST)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();

		var script = CreateOpenPopupScript(
			url,
			"payment_list",
			"width=900,height=615,top=120,left=320,status=NO,scrollbars=yes",
			callback);

		return script;
	}

	/// <summary>
	/// Get login shop operator
	/// </summary>
	/// <returns>Login shop operator model</returns>
	private static ShopOperatorModel GetLoginShopOperator()
	{
		var loginShopOperator
			= (ShopOperatorModel)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR]
				?? new ShopOperatorModel();
		return loginShopOperator;
	}

	/// <summary>
	/// Create Modify Fixed Purchase Setting Url
	/// </summary>
	/// <param name="product">Product</param>
	/// <returns>Modify Fixed Purchase Setting Url</returns>
	protected string CreateModifyFixedPurchaseSettingUrl(ProductInput product)
	{
		var modifyFixedPurchaseSettingUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_FIXEDPURCHASE_SHIPPING_PATTERN);
		var shopId = (string.IsNullOrEmpty(product.ShopId) == false)
			? product.ShopId
			: this.LoginOperatorShopId;
		modifyFixedPurchaseSettingUrl.AddParam(Constants.REQUEST_KEY_SHOP_ID, shopId);
		return modifyFixedPurchaseSettingUrl.CreateUrl();
	}

	/// <summary>
	/// Get escape value
	/// </summary>
	/// <param name="value">Value</param>
	/// <returns>A string have escape value</returns>
	protected string GetEscapeValue(string value)
	{
		if (string.IsNullOrEmpty(value)) return string.Empty;

		var result = value
			.Replace("\r\n", "\n")
			.Replace("\r", "\n")
			.Replace("\n", "\r\n")
			.Replace("\r\n", "\\n");
		return result;
	}

	/// <summary>
	/// Get date
	/// </summary>
	/// <param name="value">Value</param>
	/// <param name="defaultValue">Default value</param>
	/// <returns>A string of date</returns>
	protected string GetDate(string value, string defaultValue)
	{
		var result = string.IsNullOrEmpty(value)
			? defaultValue
			: StringUtility.ToDateString(value, "yyyy/MM/dd");
		return result;
	}

	/// <summary>
	/// Get time
	/// </summary>
	/// <param name="value">Value</param>
	/// <param name="defaultValue">Default value</param>
	/// <returns>A string of time</returns>
	protected string GetTime(string value, string defaultValue)
	{
		var result = string.IsNullOrEmpty(value)
			? defaultValue
			: StringUtility.ToDateString(value, "HH:mm:ss");
		return result;
	}

	/// <summary>
	/// Encode backslash
	/// </summary>
	/// <param name="input">Input</param>
	/// <returns>Encode input</returns>
	protected string EncodeBackslash(string input)
	{
		if (string.IsNullOrEmpty(input)) return string.Empty;

		return input.Replace(@"\", HttpUtility.UrlEncode(@"\"));
	}

	/// <summary>
	/// 文字列に指定の番号を付与
	/// </summary>
	/// <param name="value">対象文字列</param>
	/// <param name="num">番号</param>
	/// <returns>付与後の文字列</returns>
	protected string GrantNumberToString(string value, object num)
	{
		var result = value.Contains(CONST_PRODUCT_OPTION_INDEX_NUMBER_REPLACE_TAG)
			? value.Replace(CONST_PRODUCT_OPTION_INDEX_NUMBER_REPLACE_TAG, num.ToString())
			: value + num;
		return WebSanitizer.HtmlEncode(result);
	}
	#endregion

	#region +Properties
	/// <summary>Product register base url</summary>
	protected string ProductRegisterBaseUrl
	{
		get { return (Constants.PATH_ROOT + Constants.PAGE_MANAGER_NEW_PRODUCT_REGISTER); }
	}
	/// <summary>Product handling process url</summary>
	protected string ProductHandlingProcessUrl
	{
		get { return (Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_REGIST_OR_UPDATE_PROCESS); }
	}
	/// <summary>商品カラーリスト</summary>
	protected List<ProductColor> ProductColors { get; set; }
	/// <summary>Product input</summary>
	protected ProductInput ProductInput { get; set; }
	/// <summary>Upload image input</summary>
	protected UploadImageInput UploadImageInput { get; set; }
	/// <summary>Product variation default setting</summary>
	protected string ProductVariationDefaultSetting { get; set; }
	/// <summary>Product option setting list</summary>
	protected string ProductOptionSettingList { get; set; }
	/// <summary>Mall exhibits config</summary>
	protected string MallExhibitsConfig { get; set; }
	/// <summary>Product fixedpurchase discount setting</summary>
	protected string ProductFixedPurchaseDiscountSetting { get; set; }
	/// <summary>Product select variation kbn</summary>
	protected string ProductSelectVariationKbn { get; set; }
	/// <summary>確認画面から戻ってきたか</summary>
	protected bool IsBackFromConfirm
	{
		get { return (ViewState[Constants.SESSION_KEY_PARAM_FOR_BACK2] != null); }
	}
	/// <summary>An upload image temporary path</summary>
	private string UploadImageTempPath
	{
		get
		{
			return Path.Combine(
				Constants.PATH_ROOT_FRONT_PC,
				Constants.PATH_TEMP,
				"ProductImages");
		}
	}
	/// <summary>Guid string</summary>
	protected string GuidString
	{
		get
		{
			if (this.UploadImageInput == null) return string.Empty;
			return this.UploadImageInput.Guid;
		}
	}
	/// <summary>Is check resize image</summary>
	protected bool IsCheckResizeImage
	{
		get
		{
			if (this.UploadImageInput != null) return this.UploadImageInput.AutoResize;
			return false;
		}
	}
	/// <summary>Is redirect from menu</summary>
	protected bool IsRedirectFromMenu
	{
		get { return (this.IsShowBackButton == false); }
	}
	#endregion
}
