/*
=========================================================================================================
  Module      : 商品情報確認ページ処理(ProductConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Flaps;
using w2.App.Common.Input;
using w2.App.Common.Option;
using w2.App.Common.Preview;
using w2.App.Common.Product;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.Product;
using w2.Domain.ProductSubImageSetting;

/// <summary>
/// 商品情報確認ページ処理
/// </summary>
public partial class Form_Product_ProductConfirm : ProductPage
{
	/// <summary>
	/// カート投入URLの種類
	/// </summary>
	protected enum AddCartType
	{
		/// <summary>通常購入</summary>
		Normal,
		/// <summary>定期購入</summary>
		FixedPurchase,
		/// <summary>ギフト購入</summary>
		Gift
	}

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			ClearBrowserCache();

			// 画面制御
			InitializeComponents();

			// Set and bind display data
			Display();

			SetHtmlForPreviewList();
		}
	}

	/// <summary>
	/// Set and bind display data
	/// </summary>
	private void Display()
	{
		this.ProductTranslationData = new NameTranslationSettingModel[0];
		this.ProductVariationTranslationData = new NameTranslationSettingModel[0];

		switch (this.ActionStatus)
		{
			// 確認画面表示処理
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
			case Constants.ACTION_STATUS_COPY_INSERT:
				// 処理区分チェック
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

				// セッション型チェック(ポップアップ同時起動対策)
				if ((Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO] is ProductInput) == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				// Get and set data
				this.ProductInput = (ProductInput)Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO];
				this.UploadImageInput = (UploadImageInput)Session[Constants.SESSIONPARAM_KEY_UPLODED_MASTER];
				SetProductExtend();
				this.ProductOptionSettingLists = new ProductOptionSettingList(this.ProductInput.ProductOptionSettings);
				this.MallExhibitsConfig = GetProductMallExhibitsConfigFromInput();
				this.MemberRankPrices = GetProductMemberRankPriceFromInput();
				this.ProductVariations = (this.ProductInput.HasProductVariation)
					? this.ProductInput.ProductVariations
					: new ProductVariationInput[0];
				this.ProductTags = GetProductTagSettingsFromInput();
				this.RelatedCsProducts = GetRelatedCsProducts();
				this.RelatedUsProducts = GetRelatedUsProducts();

				if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
					 && (this.RequestProductId != this.ProductInput.ProductId))
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				// Get and set data for product fixed purchase discount settings if this product has setting discount
				if (Constants.FIXED_PURCHASE_DISCOUNT_PRICE_OPTION_ENABLE && this.ProductInput.HasProductFixedPurchaseDiscountSettings)
				{
					this.ProductFixedPurchaseDiscountSettings = this.ProductInput.ProductFixedPurchaseDiscountSettings.ToList();
					rFixedPurchaseDiscountOrderCount.DataSource = this.ProductFixedPurchaseDiscountSettings;
					rFixedPurchaseDiscountProductCount.DataSource = this.ProductFixedPurchaseDiscountSettings[0].ProductCountDiscounts;
					dvProductFixedPurchaseDiscountSection.Visible = true;
				}

				// Get and set data for product tag if this product has setting
				if (Constants.PRODUCT_TAG_OPTION_ENABLE)
				{
					this.DisplayTagArea = DomainFacade.Instance.ProductTagService.GetProductTagSetting()
						.Any(item => item.TagValidFlg == Constants.FLG_PRODUCTTAGSETTING_VALID_FLG_VALID);
				}

				// 名称翻訳設定情報取得
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					SetTranslationData();
				}

				if (Constants.PRODUCT_CTEGORY_OPTION_ENABLE && this.HasProductCategory)
				{
					this.ProductInput.ProductCategoryName1 = GetCategoryName(this.ProductInput.CategoryId1);
					this.ProductInput.ProductCategoryName2 = GetCategoryName(this.ProductInput.CategoryId2);
					this.ProductInput.ProductCategoryName3 = GetCategoryName(this.ProductInput.CategoryId3);
					this.ProductInput.ProductCategoryName4 = GetCategoryName(this.ProductInput.CategoryId4);
					this.ProductInput.ProductCategoryName5 = GetCategoryName(this.ProductInput.CategoryId5);
				}

				if (Constants.PRODUCT_BRAND_ENABLED && this.HasProductBrand)
				{
					this.ProductInput.ProductBrandName1 = GetBrandName(this.ProductInput.BrandId1);
					this.ProductInput.ProductBrandName2 = GetBrandName(this.ProductInput.BrandId2);
					this.ProductInput.ProductBrandName3 = GetBrandName(this.ProductInput.BrandId3);
					this.ProductInput.ProductBrandName4 = GetBrandName(this.ProductInput.BrandId4);
					this.ProductInput.ProductBrandName5 = GetBrandName(this.ProductInput.BrandId5);
				}

				if (Constants.PRODUCT_STOCK_OPTION_ENABLE)
				{
					this.ProductInput.ProductStockMessageName = GetProductStockMessageName(this.ProductInput.StockMessageId);
				}

				rProductSubImage.DataSource = this.UploadImageInput.SubImages;
				break;

			case Constants.ACTION_STATUS_DETAIL:
			case Constants.ACTION_STATUS_COMPLETE:
				// Clear sessions
				Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO] = null;
				Session[Constants.SESSIONPARAM_KEY_UPLODED_MASTER] = null;
				Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;
				Session[Constants.SESSION_KEY_ERROR_MSG] = null;

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
				this.UploadImageInput = GetProductImageInput();
				SetProductExtend();
				this.ProductOptionSettingLists = new ProductOptionSettingList(this.ProductInput.ProductOptionSettings);
				this.MallExhibitsConfig = GetProductMallExhibitsConfig(this.LoginOperatorShopId, this.RequestProductId);
				this.MemberRankPrices = GetProductMemberRankPrice();
				this.ProductVariations = this.ProductInput.ProductVariations;
				this.ProductTags = GetProductTagSettings();
				this.RelatedCsProducts = GetRelatedCsProducts();
				this.RelatedUsProducts = GetRelatedUsProducts();

				if (Constants.FIXED_PURCHASE_DISCOUNT_PRICE_OPTION_ENABLE)
				{
					var productFixedPurchaseDiscountSettingModel =
						DomainFacade.Instance.ProductFixedPurchaseDiscountSettingService.GetByShopIdAndProductId(
							this.LoginOperatorShopId,
							this.RequestProductId);
					if (productFixedPurchaseDiscountSettingModel != null)
					{
						this.ProductFixedPurchaseDiscountSettings =
							ProductFixedPurchaseDiscountSettingHeader.CreateProductFixedPurchaseDiscountSettingHeader(
								productFixedPurchaseDiscountSettingModel);
						rFixedPurchaseDiscountOrderCount.DataSource = this.ProductFixedPurchaseDiscountSettings;
						rFixedPurchaseDiscountProductCount.DataSource = this.ProductFixedPurchaseDiscountSettings[0].ProductCountDiscounts;
						dvProductFixedPurchaseDiscountSection.Visible = true;
					}
				}

				if (Constants.PRODUCT_TAG_OPTION_ENABLE)
				{
					this.DisplayTagArea = DomainFacade.Instance.ProductTagService.GetProductTagSetting()
						.Any(item => item.TagValidFlg == Constants.FLG_PRODUCTTAGSETTING_VALID_FLG_VALID);
				}

				// 名称翻訳設定情報取得
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					SetTranslationData();
				}

				rProductSubImage.DataSource = this.UploadImageInput.SubImages.Where(image => (string.IsNullOrEmpty(image.FileName) == false));
				break;

			default:
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}

		if (Constants.PRODUCT_BRAND_ENABLED)
		{
			this.BrandId1 = this.ProductInput.BrandId1;
		}

		DataBind();
	}

	/// <summary>
	/// プレビュー対象HTMLリスト設定
	/// （商品概要、商品詳細説明1～4、モバイル商品概要、モバイル商品詳細説明1～2）
	/// </summary>
	private void SetHtmlForPreviewList()
	{
		this.HtmlForPreviewList = new List<string>
		{ 
			this.ProductInput.Outline,
			this.ProductInput.DescDetail1,
			this.ProductInput.DescDetail2,
			this.ProductInput.DescDetail3,
			this.ProductInput.DescDetail4
		};
	}

	/// <summary>
	/// 翻訳設定情報設定
	/// </summary>
	private void SetTranslationData()
	{
		var searchCondition = new NameTranslationSettingSearchCondition
		{
			MasterId1 = this.RequestProductId,
		};
		var translationSettings = DomainFacade.Instance.NameTranslationSettingService
			.GetProductAndVariationTranslationSettings(searchCondition);
		this.ProductTranslationData = translationSettings
			.Where(translationSetting => (translationSetting.DataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT))
			.ToArray();
		this.ProductVariationTranslationData = translationSettings
			.Where(translationSetting => (translationSetting.DataKbn == Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCTVARIATION))
			.ToArray();
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				btnInsert.Visible = true;
				dvConfirm.Visible = true;
				btnBack.Visible = (this.IsPopUp == false);
				break;

			case Constants.ACTION_STATUS_UPDATE:
				btnUpdate.Visible = true;
				dvConfirm.Visible = true;
				btnBack.Visible = (this.IsPopUp == false);
				break;

			case Constants.ACTION_STATUS_COMPLETE:
				lMessage.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ERROR_MSG]));
				divComp.Visible = (string.IsNullOrEmpty(lMessage.Text) == false);
				btnBack.Visible = false;
				btnEdit.Visible = true;
				btnCopyInsert.Visible = true;
				btnDelete.Visible = true;
				dvDateCreated.Visible = true;
				dvDateChanged.Visible = true;
				dvLastChanged.Visible = true;
				dvDetail.Visible = true;
				dvMenu.Visible = true;
				btnNewRegist.Visible = (bool)(Session[Constants.SESSION_KEY_SHOW_CONTINUE_REGISTER_BUTTON] ?? false);

				// ポップアップ表示の場合は、「一覧へ戻る」ボタンを非表示へ
				btnBackList.Visible = (this.IsPopUp == false);
				break;

			case Constants.ACTION_STATUS_DETAIL:
				btnBack.Visible = false;
				btnEdit.Visible = true;
				btnCopyInsert.Visible = true;
				btnDelete.Visible = true;
				dvDateCreated.Visible = true;
				dvDateChanged.Visible = true;
				dvLastChanged.Visible = true;
				dvDetail.Visible = true;
				dvMenu.Visible = true;

				// ポップアップ表示の場合は、「一覧へ戻る」ボタンを非表示へ
				btnBackList.Visible = (this.IsPopUp == false);
				break;
		}

		if (Constants.PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE)
		{
			var subImageSetting = DomainFacade.Instance.ProductSubImageSettingService.GetProductSubImageSettings(
				this.LoginOperatorShopId,
				Constants.PRODUCTSUBIMAGE_MAXCOUNT);
			this.SubImageSettings = subImageSetting
				.Select(setting => new KeyValuePair<int, string>(
					setting.ProductSubImageNo.Value,
					setting.ProductSubImageName))
				.ToArray();
		}
		else
		{
			this.SubImageSettings = new KeyValuePair<int, string>[0];
		}
	}

	/// <summary>
	/// 定期配送区分の設定が表示される判定
	/// </summary>
	/// <param name="fixedPurchaseKbn">項目のキー</param>
	/// <returns>表示可否</returns>
	protected bool CheckProductLimitedFixedPurchaseDisplay(string fixedPurchaseKbn)
	{
		var shopShipping = DomainFacade.Instance.ShopShippingService.Get(
			this.LoginOperatorShopId,
			this.ProductInput.ShippingType);
		if (shopShipping == null) return false;

		var result = false;
		switch (fixedPurchaseKbn)
		{
			case Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING:
				result = (shopShipping.IsValidFixedPurchaseKbn1Flg
					|| shopShipping.IsValidFixedPurchaseKbn2Flg);
				break;

			case Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING:
				result = shopShipping.IsValidFixedPurchaseKbn3Flg;
				break;

			case Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING:
				result = shopShipping.IsValidFixedPurchaseKbn4Flg;
				break;
		}
		return result;
	}

	/// <summary>
	/// データバインド用商品在庫一覧URL作成
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>商品在庫一覧URL</returns>
	protected string CreateProductStockDetailUrl(string productId)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCTSTOCK_LIST)
			.AddParam(Constants.REQUEST_KEY_SEARCH_KEY, Constants.KBN_SEARCHKEY_PRODUCTSTOCK_LIST_PRODUCT_ID)
			.AddParam(Constants.REQUEST_KEY_SEARCH_WORD, productId)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);
		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// Get product stock message name
	/// </summary>
	/// <param name="stockMessageId">Stock message id</param>
	/// <returns>Product stock message name</returns>
	protected string GetProductStockMessageName(string stockMessageId)
	{
		if (string.IsNullOrEmpty(stockMessageId)) return GetDefaultDisplayText();

		var productStockMessage = DomainFacade.Instance.ProductStockMessageService.Get(this.LoginOperatorShopId, stockMessageId);
		return (productStockMessage != null) ? productStockMessage.StockMessageName : string.Empty;
	}

	/// <summary>
	/// Get product tag settings
	/// </summary>
	/// <returns>Product tag settings</returns>
	protected List<Hashtable> GetProductTagSettings()
	{
		var tagData = ProductTagUtility.GetProductTagData(this.RequestProductId);
		var productTagSettings = DomainFacade.Instance.ProductTagService.GetProductTagSetting();
		var productTags = productTagSettings
			.Select(item =>
				new Hashtable
				{
					{ Constants.FIELD_PRODUCTTAGSETTING_TAG_ID, item.TagId },
					{ Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME, item.TagName },
					{ Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG, item.TagValidFlg },
					{ Constants.FIELD_PRODUCTTAGSETTING_TAG_ID + "_value", StringUtility.ToEmpty(tagData[item.TagId]) }
				}).ToList();
		return productTags;
	}

	/// <summary>
	/// Get product tag settings from input
	/// </summary>
	/// <returns>Product tag settings</returns>
	protected List<Hashtable> GetProductTagSettingsFromInput()
	{
		var productTags = this.ProductInput.ProductTag.ProductTagIds
			.Select((item, index) => new Hashtable
				{
					{ Constants.FIELD_PRODUCTTAGSETTING_TAG_ID, this.ProductInput.ProductTag.ProductTagIds[index] },
					{ Constants.FIELD_PRODUCTTAGSETTING_TAG_NAME, this.ProductInput.ProductTag.ProductTagNames[index] },
					{ Constants.FIELD_PRODUCTTAGSETTING_TAG_VALID_FLG, Constants.FLG_PRODUCTTAGSETTING_VALID_FLG_VALID },
					{ Constants.FIELD_PRODUCTTAGSETTING_TAG_ID + "_value", this.ProductInput.ProductTag.ProductTagValues[index] }
				})
			.ToList();
		return productTags;
	}

	/// <summary>
	/// Get product member rank price
	/// </summary>
	/// <returns>Product member rank price</returns>
	protected List<Hashtable> GetProductMemberRankPrice()
	{
		var memberRankPrices = DomainFacade.Instance.ProductPriceService.GetAll(
			this.LoginOperatorShopId,
			this.RequestProductId);
		var result = memberRankPrices.Select(memberRankPrice =>
			new Hashtable
			{
				{ Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, memberRankPrice.MemberRankId },
				{ Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME, memberRankPrice.MemberRankName },
				{ Constants.FIELD_PRODUCTPRICE_VARIATION_ID, memberRankPrice.VariationId },
				{ Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE, memberRankPrice.MemberRankPrice }
			});
		return result.ToList();
	}

	/// <summary>
	/// Get product member rank price
	/// </summary>
	/// <returns>Product member rank price</returns>
	protected List<Hashtable> GetProductMemberRankPriceFromInput()
	{
		var memberRanks = MemberRankOptionUtility.GetMemberRankList();
		var result = new List<Hashtable>();

		if (this.ProductInput.HasProductPrices)
		{
			result = this.ProductInput.ProductPrices
				.Select(productPrice =>
					new Hashtable
					{
						{ Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, productPrice.MemberRankId },
						{
							Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME,
							memberRanks.FirstOrDefault(memberRankPrice => (memberRankPrice.MemberRankId == productPrice.MemberRankId)).MemberRankName
						},
						{ Constants.FIELD_PRODUCTPRICE_VARIATION_ID, string.Empty },
						{ Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE, productPrice.MemberRankPrice }
					})
				.ToList();
		}

		if (this.ProductInput.HasProductVariations)
		{
			var productVariations = this.ProductInput.ProductVariations;
			foreach (var productVariation in productVariations)
			{
				if (productVariation.HasProductPrices)
				{
					var variationMemberRankPrices = productVariation.ProductPrices
						.Select(productPrice =>
							new Hashtable
							{
								{ Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, productPrice.MemberRankId },
								{
									Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME,
									memberRanks.FirstOrDefault(memberRankPrice => (memberRankPrice.MemberRankId == productPrice.MemberRankId)).MemberRankName
								},
								{ Constants.FIELD_PRODUCTPRICE_VARIATION_ID, productPrice.VariationId },
								{ Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE, productPrice.MemberRankPrice }
							})
						.ToList();
					result.AddRange(variationMemberRankPrices);
				}
			}
		}

		return result;
	}

	/// <summary>
	/// Get product mall exhibits config from input
	/// </summary>
	/// <returns>Product mall exhibits config</returns>
	protected List<KeyValuePair<string, string>> GetProductMallExhibitsConfigFromInput()
	{
		var results = new List<KeyValuePair<string, string>>();
		if (this.ProductInput.HasMallExhibitsConfig == false) return results;

		var mallExhibitsConfigs = ValueText.GetValueItemList(
			Constants.TABLE_MALLCOOPERATIONSETTING,
			Constants.FIELD_MALLCOOPERATIONSETTING_MALL_EXHIBITS_CONFIG);
		var mallCooperationSettings = DomainFacade.Instance.MallCooperationSettingService.GetAll(this.LoginOperatorShopId);
		for (var index = 0; index < mallExhibitsConfigs.Count; index++)
		{
			foreach (var model in mallCooperationSettings)
			{
				var currentExhibitsFlg = this.ProductInput.MallExhibitsConfig.GetExhibitsFlg(index + 1);
				if ((model.MallExhibitsConfig == mallExhibitsConfigs[index].Value)
					&& (currentExhibitsFlg == Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_ON))
				{
					results.Add(
						new KeyValuePair<string, string>(
							model.MallExhibitsConfig,
							currentExhibitsFlg));
					break;
				}
			}
		}

		return results;
	}

	/// <summary>
	/// Get first description select variation kbn
	/// </summary>
	/// <param name="selectVariationKbn">Select variation kbn</param>
	/// <returns>The first description select variation kbn</returns>
	protected string GetFirstDescriptionSelectVariationKbn(string selectVariationKbn)
	{
		if (string.IsNullOrEmpty(selectVariationKbn)) return string.Empty;

		var selectVariationKbnText = ValueText.GetValueText(
			Constants.TABLE_PRODUCT,
			Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN,
			selectVariationKbn);
		return selectVariationKbnText.Split('(')[0];
	}

	/// <summary>
	/// Get second description select variation kbn
	/// </summary>
	/// <param name="selectVariationKbn">Select variation kbn</param>
	/// <returns>The second description select variation kbn</returns>
	protected string GetSecondDescriptionSelectVariationKbn(string selectVariationKbn)
	{
		if (string.IsNullOrEmpty(selectVariationKbn)) return string.Empty;

		var selectVariationKbnText = ValueText.GetValueText(
			Constants.TABLE_PRODUCT,
			Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN,
			selectVariationKbn);
		var selectVariationKbnTexts = selectVariationKbnText.Split('(');
		return (selectVariationKbnTexts.Length > 1)
			? selectVariationKbnText.Split('(')[1].Replace(")", string.Empty)
			: string.Empty;
	}

	/// <summary>
	/// Set product extend
	/// </summary>
	private void SetProductExtend()
	{
		var productExtendSettings = DomainFacade.Instance.ProductExtendSettingService.GetAll(this.LoginOperatorShopId);
		this.ProductExtends = productExtendSettings
			.Select(item => new Hashtable
				{
					{ Constants.FIELD_PRODUCTEXTENDSETTING_EXTEND_NAME, item.Name },
					{ "extend", this.ProductInput.HasProductExtend
						? StringUtility.ChangeToBrTag(this.ProductInput.ProductExtend.GetProductExtendValueWithDataSource(
							string.Format(
								"extend{0}",
								item.ExtendNo)))
						: string.Empty },
				})
			.ToList();
	}

	/// <summary>
	/// 配送間隔日・月利用不可の文言作成
	/// </summary>
	/// <param name="baseSetting">ベース設定値</param>
	/// <param name="unit">単位</param>
	/// <returns>配送間隔日・月利用不可の文言</returns>
	protected string CreateLimitedFixedPurchaseIntervalDisplay(string baseSetting, string unit)
	{
		if (string.IsNullOrEmpty(baseSetting)) return GetDefaultDisplayText();

		var unitText = ValueText.GetValueText(
			Constants.TABLE_PRODUCT,
			LIMITED_FIXED_PURCHASE_KBN_SETTING,
			unit);
		var result = string.Format("{0}{1}", string.Join(unitText + ", ", baseSetting.Split(',')), unitText);
		return result;
	}

	/// <summary>
	/// 定期購入制限ユーザー管理レベルを表示
	/// </summary>
	/// <param name="userLevelIds">制限されるユーザー管理レベルId</param>
	/// <returns>制限されるユーザー管理レベル名</returns>
	protected string GetFixedPurchaseLimitedUserLevelDisplay(string userLevelIds)
	{
		if (string.IsNullOrEmpty(userLevelIds)) return GetDefaultDisplayText();

		var userLevelNames = DomainFacade.Instance.UserManagementLevelService
			.GetUserManagementLevelNamesByUserManagementLevelIds(userLevelIds.Split(','));
		return string.Join(", ", userLevelNames);
	}

	/// <summary>
	/// Get the limited payment display
	/// </summary>
	/// <param name="limitedPaymentIds">The limited payment ids</param>
	/// <returns>The limited payment name</returns>
	protected string GetLimitedPaymentDisplay(string limitedPaymentIds)
	{
		if (string.IsNullOrEmpty(limitedPaymentIds)) return GetDefaultDisplayText();

		var payments = GetPaymentValidList(this.LoginOperatorShopId);
		var paymentIdList = limitedPaymentIds.Split(',');
		var paymentLimited = payments
			.Cast<DataRowView>().Where(item => paymentIdList.Contains((string)item[Constants.FIELD_PAYMENT_PAYMENT_ID]));

		return string.Join(",", paymentLimited.Select(item => (string)item[Constants.FIELD_PAYMENT_PAYMENT_NAME]));
	}

	/// <summary>
	/// 定期購入割引値引き可否
	/// </summary>
	/// <param name="productFixedPurchaseDiscountAvailability">値引き</param>
	/// <returns>定期購入割引値引き可否</returns>
	protected bool ProductFixedPurchaseDiscountAvailability(
		ProductFixedPurchaseDiscountSettingDetail productFixedPurchaseDiscountAvailability)
	{
		return (string.IsNullOrEmpty(productFixedPurchaseDiscountAvailability.DiscountValue) == false);
	}

	/// <summary>
	/// 定期購入割引ポイント特典可否
	/// </summary>
	/// <param name="productFixedPurchaseDiscountPointRewardAvailability">ポイント特典</param>
	/// <returns>定期購入割引ポイント特典可否</returns>
	protected bool ProductFixedPurchaseDiscountPointRewardAvailability(
		ProductFixedPurchaseDiscountSettingDetail productFixedPurchaseDiscountPointRewardAvailability)
	{
		return (string.IsNullOrEmpty(productFixedPurchaseDiscountPointRewardAvailability.PointValue) == false);
	}

	/// <summary>
	/// Get category name
	/// </summary>
	/// <param name="categoryId">Category id</param>
	/// <returns>Category name</returns>
	protected string GetCategoryName(string categoryId)
	{
		if (string.IsNullOrEmpty(categoryId)) return string.Empty;

		var category = DomainFacade.Instance.ProductCategoryService.Get(categoryId);
		return (category != null)
			? category.Name
			: categoryId;
	}

	/// <summary>
	/// Get brand name
	/// </summary>
	/// <param name="brandId">Brand id</param>
	/// <returns>Brand name</returns>
	protected string GetBrandName(string brandId)
	{
		if (string.IsNullOrEmpty(brandId)) return string.Empty;

		return ProductBrandUtility.GetProductBrandName(brandId);
	}

	/// <summary>
	/// 会員ランク価格表示用文字列生成
	/// </summary>
	/// <param name="productMemberPrice">会員ランク価格</param>
	/// <returns>会員ランク価格表示文字列</returns>
	protected string DisplayProductMemberPrice(object productMemberPrice)
	{
		var result = GetDefaultDisplayText();
		if (string.IsNullOrEmpty(StringUtility.ToEmpty(productMemberPrice))) return result;

		return productMemberPrice.ToPriceString(true);
	}

	/// <summary>
	/// 会員ランク価格取得（データバインド用）
	/// </summary>
	/// <returns>Member rank prices</returns>
	protected List<Hashtable> GetMemberRankPricesForDatabind()
	{
		return GetMemberRankPricesVariationListForDatabind(string.Empty);
	}

	/// <summary>
	/// バリエーションの会員ランク価格取得（データバインド用）
	/// </summary>
	/// <param name="variationId">Variation id</param>
	/// <returns>Member rank prices</returns>
	protected List<Hashtable> GetMemberRankPricesVariationListForDatabind(string variationId)
	{
		var results = new List<Hashtable>();
		var memberRankList = MemberRankOptionUtility.GetMemberRankList();
		foreach (var memberRank in memberRankList)
		{
			var isAddDefault = true;
			foreach (var memberRankPrice in this.MemberRankPrices)
			{
				if ((StringUtility.ToEmpty(GetKeyValue(memberRankPrice, Constants.FIELD_PRODUCTPRICE_VARIATION_ID)) == variationId)
					&& (memberRank.MemberRankId == StringUtility.ToEmpty(GetKeyValue(memberRankPrice, Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID))))
				{
					results.Add(memberRankPrice);
					isAddDefault = false;
				}
			}

			if (isAddDefault)
			{
				var productPrice = new Hashtable
					{
						{ Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, memberRank.MemberRankId },
						{ Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME, memberRank.MemberRankName },
						{ Constants.FIELD_PRODUCTPRICE_VARIATION_ID, variationId },
						{ Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE, DBNull.Value }
					};
				results.Add(productPrice);
			}
		}

		return results.ToList();
	}

	/// <summary>
	/// ポイント区分により文字を取得
	/// </summary>
	/// <param name="pointKbn">ポイント区分</param>
	/// <returns>表示文字</returns>
	protected string GetPointKbn(string pointKbn)
	{
		switch (pointKbn)
		{
			case Constants.FLG_PRODUCT_POINT_KBN2_INVALID:
				return string.Empty;

			case Constants.FLG_PRODUCT_POINT_KBN2_NUM:
				return "pt";

			case Constants.FLG_PRODUCT_POINT_KBN2_RATE:
				return "%";

			default:
				return string.Empty;
		}
	}

	/// <summary>
	/// カート投入URLを作成
	/// </summary>
	/// <param name="addCartType">カート投入URLの種類</param>
	/// <param name="variationId">バリエーションID（nullはバリエーションがない場合）</param>
	/// <param name="prdcnt">カート商品追加数</param>
	/// <returns>カート投入URL</returns>
	protected string CreateAddCartUrl(AddCartType addCartType, string variationId = null, int prdcnt = 1)
	{
		const string SEQUENTIAL_ORDER = "1";

		var urlCreator =
			new UrlCreator(Constants.PROTOCOL_HTTPS
					+ Constants.SITE_DOMAIN
					+ Constants.PATH_ROOT_FRONT_PC
					+ Constants.PAGE_FRONT_CART_LIST)
				.AddParam(Constants.REQUEST_KEY_CART_ACTION, SEQUENTIAL_ORDER)
				.AddParam(Constants.REQUEST_KEY_FRONT_PRODUCT_ID + SEQUENTIAL_ORDER, this.RequestProductId)
				.AddParam(
					Constants.REQUEST_KEY_FRONT_VARIATION_ID + SEQUENTIAL_ORDER,
					string.IsNullOrEmpty(variationId) ? this.RequestProductId : variationId)
				.AddParam(Constants.REQUEST_KEY_PRODUCT_COUNT + SEQUENTIAL_ORDER, prdcnt.ToString());

		switch (addCartType)
		{
			case AddCartType.FixedPurchase:
				urlCreator.AddParam(Constants.REQUEST_KEY_FIXED_PURCHASE + SEQUENTIAL_ORDER, "1");
				break;

			case AddCartType.Gift:
				urlCreator.AddParam(Constants.REQUEST_KEY_GIFT_ORDER + SEQUENTIAL_ORDER, "1");
				break;
		}

		var url = urlCreator.CreateUrl();
		return url;
	}

	/// <summary>
	/// データバインド用商品在庫情報作成
	/// </summary>
	/// <param name="stockManagementKbn">在庫管理方法</param>
	/// <param name="stock">商品在庫数</param>
	/// <returns>商品在庫情報作成</returns>
	protected string CreateStockInfo(string stockManagementKbn, string stock)
	{
		// 在庫管理あり
		if (stockManagementKbn != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
		{
			return (string.IsNullOrEmpty(stock) == false)
				? stock
				: ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT,
					Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM,
					Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM_DATA_INCONSISTENCY);
		}

		// 在庫管理なし
		return GetDefaultDisplayText();
	}

	/// <summary>
	/// Create preview product url
	/// </summary>
	/// <param name="previewSite">Preview site</param>
	/// <returns>Preview product url</returns>
	protected string CreatePreviewProductUrl(string previewSite)
	{
		// 商品詳細プレビュー情報登録
		// プレビュー用に商品IDのパラメタ取得
		// ※新規登録時のフローはリクエストに商品IDが存在しないため取得
		var productId = string.Empty;
		var brandId1 = string.Empty;
		var guidString = string.Empty;
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				// 商品詳細プレビュー情報登録
				InsertProductDetailPreview(this.ActionStatus, this.ProductInput);
				productId = this.ProductInput.ProductId;
				brandId1 = Constants.PRODUCT_BRAND_ENABLED ? this.ProductInput.BrandId1 : string.Empty;
				guidString = this.UploadImageInput.Guid;
				break;

			case Constants.ACTION_STATUS_DETAIL:
			case Constants.ACTION_STATUS_COMPLETE:
				// 商品情報取得
				using (var accessor = new SqlAccessor())
				{
					// 商品詳細プレビュー情報取得
					var dvProduct = ProductPreview.GetProductDetailPreviewData(
						this.LoginOperatorShopId,
						this.RequestProductId,
						accessor);

					// 商品詳細プレビュー情報登録
					ProductPreview.InsertProductDetailPreview(
						this.LoginOperatorShopId,
						this.RequestProductId,
						dvProduct.Table);
				}
				productId = this.RequestProductId;
				brandId1 = Constants.PRODUCT_BRAND_ENABLED ? this.BrandId1 : string.Empty;
				break;
		}

		// プレビュー表示を有効
		var previewUrl = CreateUrlForProductDetailPreview(
			previewSite,
			productId,
			brandId1,
			guidString);
		return previewUrl;
	}

	/// <summary>
	/// Get product value to display
	/// </summary>
	/// <param name="productValue">Product value</param>
	/// <param name="fieldName">Field name</param>
	/// <returns>A string to display</returns>
	protected string GetProductValueToDisplay(string productValue, string fieldName = "")
	{
		switch (fieldName)
		{
			case Constants.FIELD_PRODUCT_PRODUCT_TYPE:
			case Constants.FIELD_PRODUCT_GIFT_FLG:
			case Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG:
			case Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE:
			case Constants.FIELD_PRODUCT_VALID_FLG:
			case Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG:
			case Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG:
			case Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG:
			case Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG:
			case Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG:
			case Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG:
			case Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG:
			case Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG:
			case Constants.FIELD_PRODUCT_AGE_LIMIT_FLG:
			case Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG:
			case Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG:
			case Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG:
			case Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG:
			case Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG:
			case Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG:
			case Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG:
			case Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN:
			case Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG:
			case Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN:
			case Constants.FIELD_PRODUCT_STOREPICKUP_FLG:
			case Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG:
				return ValueText.GetValueText(Constants.TABLE_PRODUCT, fieldName, productValue);

			case Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING:
			case Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING:
			case Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING:
				return CreateLimitedFixedPurchaseIntervalDisplay(
					productValue,
					fieldName);

			case Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT:
			case Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT:
				return (string.IsNullOrEmpty(productValue) == false)
					? productValue + ReplaceTag("@@DispText.product_option.ProductCount@@")
					: GetDefaultDisplayText();

			case Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG:
			case Constants.FIELD_PRODUCTVARIATION_VARIATION_ANDMALL_RESERVATION_FLG:
				return ValueText.GetValueText(Constants.TABLE_PRODUCTVARIATION, fieldName, productValue);

			case Constants.FIELD_PRODUCT_OUTLINE:
			case Constants.FIELD_PRODUCT_DESC_DETAIL1:
			case Constants.FIELD_PRODUCT_DESC_DETAIL2:
			case Constants.FIELD_PRODUCT_DESC_DETAIL3:
			case Constants.FIELD_PRODUCT_DESC_DETAIL4:
				return (string.IsNullOrEmpty(productValue) == false)
					? GetEncodedHtmlDisplayMessage(productValue)
					: GetDefaultDisplayText();

			default:
				return (string.IsNullOrEmpty(productValue) == false)
					? productValue
					: GetDefaultDisplayText();
		}
	}

	/// <summary>
	/// Get default display text
	/// </summary>
	/// <returns>Display text: no setting</returns>
	private string GetDefaultDisplayText()
	{
		return ReplaceTag("@@DispText.product_option.ProductEmptyValue@@");
	}

	/// <summary>
	/// Get related cross-selling products
	/// </summary>
	/// <returns>Related cross-selling products</returns>
	protected ProductModel[] GetRelatedCsProducts()
	{
		var productIds = new[]
		{
			this.ProductInput.RelatedProductIdCs1,
			this.ProductInput.RelatedProductIdCs2,
			this.ProductInput.RelatedProductIdCs3,
			this.ProductInput.RelatedProductIdCs4,
			this.ProductInput.RelatedProductIdCs5,
		};
		productIds = productIds.Where(id => (string.IsNullOrEmpty(id) == false))
			.ToArray();
		if (productIds.Length == 0) return new ProductModel[0];

		var products = DomainFacade.Instance.ProductService.GetProductsByProductIds(this.LoginOperatorShopId, productIds);
		return products;
	}

	/// <summary>
	/// Get related upsell products
	/// </summary>
	/// <returns>Related upsell products</returns>
	protected ProductModel[] GetRelatedUsProducts()
	{
		var productIds = new[]
		{
			this.ProductInput.RelatedProductIdUs1,
			this.ProductInput.RelatedProductIdUs2,
			this.ProductInput.RelatedProductIdUs3,
			this.ProductInput.RelatedProductIdUs4,
			this.ProductInput.RelatedProductIdUs5,
		};
		productIds = productIds.Where(id => (string.IsNullOrEmpty(id) == false))
			.ToArray();
		if (productIds.Length == 0) return new ProductModel[0];

		var products = DomainFacade.Instance.ProductService.GetProductsByProductIds(this.LoginOperatorShopId, productIds);
		return products;
	}

	/// <summary>
	/// バリエーションIDに変更があるか
	/// </summary>
	/// <returns>バリエーションIDに変更があるか</returns>
	protected bool IsChangedVariationId()
	{
		var oldVariationIds = new ProductService().GetProductVariationsByProductId(this.RequestProductId).Select(pv => pv.VariationId).ToArray();
		if (oldVariationIds.Any() == false) return false;

		var sameProductVariationsCount = this.ProductVariations.Count(item => oldVariationIds.Contains(item.VariationId));
		return sameProductVariationsCount < oldVariationIds.Length;
	}

	#region +Image methods
	/// <summary>
	/// Get image source
	/// </summary>
	/// <param name="fileName">File name</param>
	/// <param name="isMainImage">Is main image</param>
	/// <returns>Image source</returns>
	protected string GetImageSource(string fileName, bool isMainImage = true)
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_COMPLETE:
			case Constants.ACTION_STATUS_DETAIL:
				return ProductImage.GetHtmlImageSource(
					fileName,
					this.LoginOperatorShopId,
					SiteType.Pc,
					isMainImage ? ImageType.Normal : ImageType.Sub);

			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
			case Constants.ACTION_STATUS_COPY_INSERT:
				var filePath = Constants.PRODUCT_IMAGE_HEAD_ENABLED == false
					? string.IsNullOrEmpty(fileName)
						? string.Empty
						: File.Exists(Path.Combine(this.UploadImageTempPathAbsolute, fileName))
							? Path.Combine(this.UploadImageTempPathRelative, fileName)
							: string.Empty
					: ProductImage.GetProductImagePath(
						fileName,
						this.LoginOperatorShopId,
						SiteType.Pc,
						isMainImage ? ImageType.Normal : ImageType.Sub);


				return ProductImage.GetImageSource(filePath);

			default:
				return string.Empty;
		}
	}

	/// <summary>
	/// Get product image input
	/// </summary>
	/// <returns>An upload image input</returns>
	private UploadImageInput GetProductImageInput()
	{
		var input = new UploadImageInput();
		input.MainImage = new UploadProductImageInput
		{
			FileName = this.ProductInput.ImageHead + Constants.PRODUCTIMAGE_FOOTER_LL
		};

		var subImageSettings = (Constants.PRODUCT_SUBIMAGE_SETTING_OPTION_ENABLE)
			? DomainFacade.Instance.ProductSubImageSettingService.GetProductSubImageSettings(
				this.LoginOperatorShopId,
				Constants.PRODUCTSUBIMAGE_MAXCOUNT)
			: new ProductSubImageSettingModel[0];

		var subImages = new List<UploadProductSubImageInput>();
		foreach (var setting in subImageSettings)
		{
			subImages.Add(new UploadProductSubImageInput
			{
				FileName = string.Format(
					"{0}{1}{2:00}{3}",
					this.ProductInput.ImageHead,
					Constants.PRODUCTSUBIMAGE_FOOTER,
					setting.ProductSubImageNo.Value,
					Constants.PRODUCTIMAGE_FOOTER_LL),
				ImageNo = setting.ProductSubImageNo.Value
			});
		}

		input.SubImages = subImages.ToArray();
		var variationImages = new List<UploadProductVariationImageInput>();
		foreach (var variation in this.ProductInput.ProductVariations)
		{
			variationImages.Add(new UploadProductVariationImageInput
			{
				FileName = (variation.VariationImageHead + Constants.PRODUCTIMAGE_FOOTER_LL)
			});
		}
		input.VariationImages = variationImages.ToArray();

		return input;
	}

	/// <summary>
	/// Execute upload product image process
	/// </summary>
	private void ExecUploadProductImageProcess()
	{
		// 商品画像名ヘッダ使用オプションを利用の場合は処理を行わない
		if (Constants.PRODUCT_IMAGE_HEAD_ENABLED) return;

		if (this.ActionStatus == Constants.ACTION_STATUS_UPDATE) DeleteProductOldImage();

		// Move image to main path
		MoveUploadedImagesFromTempToMainPath();

		// Move image to sub path
		MoveUploadedImagesFromTempToSubPath();

		DeleteTemporaryImages();
	}

	/// <summary>
	/// Move uploaded images from temporary path to main path
	/// </summary>
	private void MoveUploadedImagesFromTempToMainPath()
	{
		var filePaths = GetProductImagePaths(this.UploadImageTempPathAbsolute);
		MoveFileToDirection(this.UploadImageMainPath, filePaths);
	}

	/// <summary>
	/// Move uploaded images from temporary path to sub path
	/// </summary>
	private void MoveUploadedImagesFromTempToSubPath()
	{
		var filePaths = GetProductSubImagePaths(this.UploadImageTempPathAbsolute);
		MoveFileToDirection(this.UploadImageSubPath, filePaths);
	}

	/// <summary>
	/// Move file to direction
	/// </summary>
	/// <param name="dirPath">Direction path</param>
	/// <param name="filePaths">File paths</param>
	private void MoveFileToDirection(string dirPath, string[] filePaths)
	{
		foreach (var path in filePaths)
		{
			var file = new FileInfo(path);
			var destFileName = Path.Combine(dirPath, file.Name);
			file.CopyTo(destFileName, true);
			file.Delete();
		}
	}

	/// <summary>
	/// Get product image paths
	/// </summary>
	/// <param name="directoryPath">Directory path</param>
	/// <returns>Product image path array</returns>
	private string[] GetProductImagePaths(string directoryPath)
	{
		var baseImagePattern = string.Format("\\\\{{0}}_(LL|L|M|S){0}$", Constants.JPG_FILE_EXTENSION);

		var escapedImageHead = ReplaceSpecialCharacters(this.ProductInput.ImageHead);
		var mainImagePattern = string.Format(
			baseImagePattern,
			escapedImageHead);
		var imageFilePaths = Directory.GetFiles(
			directoryPath,
			string.Format(
				"{0}*{1}",
				this.ProductInput.ImageHead,
				Constants.JPG_FILE_EXTENSION));
		var targetImageFilePaths = imageFilePaths
			.Where(file => Regex.IsMatch(file, mainImagePattern, RegexOptions.IgnoreCase))
			.ToList();

		if (this.ProductInput.HasProductVariations == false) return targetImageFilePaths.ToArray();

		foreach (var variation in this.ProductInput.ProductVariations)
		{
			var escapedVariationImageHead = ReplaceSpecialCharacters(variation.VariationImageHead);
			var variationImagePattern = string.Format(
				baseImagePattern,
				escapedVariationImageHead);

			var variationImagePaths = Directory.GetFiles(
				directoryPath,
				string.Format(
					"{0}*{1}",
					variation.VariationImageHead,
					Constants.JPG_FILE_EXTENSION));
			targetImageFilePaths.AddRange(
				variationImagePaths.Where(file => Regex.IsMatch(file, variationImagePattern, RegexOptions.IgnoreCase)));
		}

		return targetImageFilePaths.ToArray();
	}

	/// <summary>
	/// Get product sub image paths
	/// </summary>
	/// <param name="directoryPath">Directory path</param>
	/// <returns>Product sub image path array</returns>
	private string[] GetProductSubImagePaths(string directoryPath)
	{
		var imageFilePathList = new List<string>();
		var imageHead = ReplaceSpecialCharacters(this.ProductInput.ImageHead);

		foreach (var subImage in this.UploadImageInput.SubImages)
		{
			var subImagePattern = string.Format(
				"\\\\{0}{1}{2}_(LL|L|M){3}$",
				imageHead,
				Constants.PRODUCTSUBIMAGE_FOOTER,
				subImage.ImageNo.ToString(Constants.PRODUCTSUBIMAGE_NUMBERFORMAT),
				Constants.JPG_FILE_EXTENSION);

			var imageFilePaths= Directory.GetFiles(directoryPath)
				.Where(file => new Regex(subImagePattern, RegexOptions.IgnoreCase).IsMatch(file));

			imageFilePathList.AddRange(imageFilePaths);
		}

		return imageFilePathList.ToArray();
	}

	/// <summary>
	/// Delete product old image
	/// </summary>
	private void DeleteProductOldImage()
	{
		var imageFilePaths = GetProductImagePaths(this.UploadImageMainPath).ToList();
		imageFilePaths.AddRange(GetProductSubImagePaths(this.UploadImageSubPath));

		foreach (var imageFilePath in imageFilePaths)
		{
			var isReadOnly = new FileInfo(imageFilePath).IsReadOnly;
			if (isReadOnly) File.SetAttributes(imageFilePath, FileAttributes.Normal);

			File.Delete(imageFilePath);
		}
	}

	/// <summary>
	/// Delete temporary images
	/// </summary>
	private void DeleteTemporaryImages()
	{
		Directory.Delete(this.UploadImageTempPathAbsolute, true);

		var dirPath = Path.Combine(
			Constants.PHYSICALDIRPATH_CONTENTS_ROOT,
			Constants.PATH_TEMP.Replace("/", @"\"),
			"ProductImages");
		var tempPaths = Directory.GetDirectories(dirPath);
		foreach (var path in tempPaths)
		{
			var dirInfo = new DirectoryInfo(path);
			if (dirInfo.Name.Length < 8) continue;

			var date = ObjectUtility.TryParseExacDateTime(dirInfo.Name.Substring(0, 8), "yyyyMMdd", DateTime.Now);
			var diff = DateTime.Now - date;
			if (diff.TotalDays >= 2)
			{
				Directory.Delete(path, true);
			}
		}
	}

	/// <summary>
	/// Get sub image setting name
	/// </summary>
	/// <param name="subImageNo">Sub image no</param>
	/// <returns>Sub image setting name</returns>
	protected string GetSubImageSettingName(int subImageNo)
	{
		var subImage = this.SubImageSettings.FirstOrDefault(setting => (setting.Key == subImageNo));
		return subImage.Value;
	}
	#endregion

	#region +Event methods
	/// <summary>
	/// 製品オプション値リピータにバインドします。
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rptProductOptionValue_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		if ((e.Item.ItemType != ListItemType.Item)
			&& (e.Item.ItemType != ListItemType.AlternatingItem)) return;

		var isDisplay = GetProductDefaultSettingDisplayField("product_option_setting" + (e.Item.ItemIndex + 1));
		e.Item.FindControl("dvOptionHeader").Visible = e.Item.FindControl("dvOptionContent").Visible = isDisplay;

		((Label)e.Item.FindControl("lbDefaultSettingComment")).Text =
			GetProductDefaultSettingComment("product_option_setting" + (e.Item.ItemIndex + 1));
		var itemName = this.ProductOptionSettingLists.GetProductOptionSettingValueName(e.Item.ItemIndex, false);
		((Label)e.Item.FindControl("lbItemName")).Text = GetProductValueToDisplay(itemName);
		if (string.IsNullOrEmpty(itemName))
		{
			((Label)e.Item.FindControl("lbItemName")).CssClass = "empty-value";
		}

		var displayFormat = this.ProductOptionSettingLists.GetProductOptionSettingDispKbn(e.Item.ItemIndex);
		((Label)e.Item.FindControl("lbDisplayFormat")).Text = GetProductValueToDisplay(displayFormat);
		if (string.IsNullOrEmpty(displayFormat))
		{
			((Label)e.Item.FindControl("lbDisplayFormat")).CssClass = "empty-value";
		}

		var pos = this.ProductOptionSettingLists.Items[e.Item.ItemIndex];
		if (pos.DisplayKbn == Constants.PRODUCTOPTIONVALUES_DISP_KBN_TEXTBOX)
		{
			((Label)e.Item.FindControl("lblDefaultForTb")).Text = GetProductValueToDisplay(pos.DefaultValue);
			if (string.IsNullOrEmpty(pos.DefaultValue))
			{
				((Label)e.Item.FindControl("lblDefaultForTb")).CssClass = "empty-value";
			}

			// 必須有無
			((Label)e.Item.FindControl("lblNecessary")).Text = (pos.IsNecessary)
				//「必須入力」
				? ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT,
					Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM,
					Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM_REQUIRED_INPUT)
				//「任意入力」
				: ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_PRODUCT,
					Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM,
					Constants.VALUETEXT_PARAM_PRODUCT_CONFIRM_ARBITRARY_INPUT);

			// 入力チェック種別
			((Label)(e.Item.FindControl("lblCheckType"))).Text =
				ValueText.GetValueText(
					Constants.TABLE_USEREXTENDSETTING,
					"validation_type",
					pos.Type);

			// 固定長
			var isFixedLength = (string.IsNullOrEmpty(pos.Length) == false);
			((Label)e.Item.FindControl("lblFixedLength")).Text = isFixedLength
				? ValueText.GetValueItemList(Constants.TABLE_USEREXTENDSETTING, "fixedlength")[1].Text
				: ValueText.GetValueItemList(Constants.TABLE_USEREXTENDSETTING, "fixedlength")[0].Text;
			((Label)e.Item.FindControl("lblLength")).Text = pos.Length;
			((Label)e.Item.FindControl("lblLengthMin")).Text = pos.LengthMin;
			((Label)e.Item.FindControl("lblLengthMax")).Text = pos.LengthMax;

			// 変動（mix-max）
			((Label)e.Item.FindControl("lblNumMin")).Text = pos.MinValue;
			((Label)e.Item.FindControl("lblNumMax")).Text = pos.MaxValue;

			// 入力方法の入力エリア表示制御
			var isVisible = false;
			switch (pos.Type)
			{
				// 半角数字/数値以外
				case Validator.STRTYPE_FULLWIDTH:
				case Validator.STRTYPE_FULLWIDTH_HIRAGANA:
				case Validator.STRTYPE_FULLWIDTH_KATAKANA:
				case Validator.STRTYPE_HALFWIDTH:
				case Validator.STRTYPE_HALFWIDTH_ALPHNUMSYMBOL:
					isVisible = isFixedLength
						|| (string.IsNullOrEmpty(pos.LengthMin) == false)
						|| (string.IsNullOrEmpty(pos.LengthMax) == false);

					(e.Item.FindControl("dvLengthInputArea")).Visible = isVisible;
					(e.Item.FindControl("dvFixedLengthInputArea")).Visible = isFixedLength;
					(e.Item.FindControl("dvMaxMinLengthInputArea")).Visible =
						(isFixedLength == false)
						&& ((string.IsNullOrEmpty(pos.LengthMin) == false)
							|| (string.IsNullOrEmpty(pos.LengthMax) == false));
					break;

				// 半角数字/数値
				case Validator.STRTYPE_HALFWIDTH_NUMBER:
					isVisible =
						(string.IsNullOrEmpty(pos.MinValue) == false)
						|| (string.IsNullOrEmpty(pos.MaxValue) == false);
					(e.Item.FindControl("dvNumberRangeInputArea")).Visible = isVisible;
					break;

				default:
					break;
			}

			e.Item.FindControl("dvSettingTextbox").Visible = true;
		}
		else
		{
			e.Item.FindControl("dvSettingNonTextbox").Visible = isDisplay;
			var settingValue = this.ProductOptionSettingLists.GetProductOptionSettingValue(e.Item.ItemIndex);
			((Label)e.Item.FindControl("lbSettingValue")).Text = GetProductValueToDisplay(settingValue);
			if (string.IsNullOrEmpty(settingValue))
			{
				((Label)e.Item.FindControl("lbSettingValue")).CssClass = "empty-value";
			}
		}
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, System.EventArgs e)
	{
		switch (this.ActionStatus)
		{
			// 詳細
			case Constants.ACTION_STATUS_DETAIL:
				// 一覧画面へ
				Response.Redirect(CreateProductListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCT_SEARCH_INFO]));
				break;

			// 新規、コピー新規、更新
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				// 編集画面へ
				Response.Redirect(CreateProductRegistUrl(this.RequestProductId, this.ActionStatus, this.IsShowBackButton));
				break;
		}
	}

	/// <summary>
	/// 一覧へ戻るボタン処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBackList_Click(object sender, EventArgs e)
	{
		Response.Redirect(CreateProductListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCT_SEARCH_INFO]));
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(
			CreateProductRegistUrl(this.RequestProductId, Constants.ACTION_STATUS_UPDATE, true));
	}

	/// <summary>
	/// Click the button to new register
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnNewRegist_Click(object sender, System.EventArgs e)
	{
		// セッション初期化
		Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO] = null;
		Session[Constants.SESSIONPARAM_KEY_PRODUCTVARIATION_INFO] = null;
		Session[Constants.SESSIONPARAM_KEY_PRODUCTEXTEND_INFO] = null;

		// 画面遷移
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK2] = null;

		// 新規登録画面へ
		Response.Redirect(
			CreateProductRegistUrl(string.Empty, Constants.ACTION_STATUS_INSERT, true));
	}

	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;

		// 登録画面へ
		Response.Redirect(
			CreateProductRegistUrl(
				this.RequestProductId,
				Constants.ACTION_STATUS_COPY_INSERT,
				true));
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// 頒布会フラグがONで頒布会に含まれている商品は削除できない
		if (this.CanSubscriptionBox)
		{
			var subscriptionBoxItems = DomainFacade.Instance.SubscriptionBoxService
				.GetSubscriptionItemByProductId(
					this.LoginOperatorShopId,
					this.RequestProductId);
			if (subscriptionBoxItems.Length != 0)
			{
				dvErrorMessage.Visible = true;
				lErrorMessage.Text = GetEncodedHtmlDisplayMessage(
					WebMessages.GetMessages(WebMessages.ERRMSG_DELETE_PRODUCT_INCLUDE_SUBSCRIPTION_BOX));
				return;
			}
		}

		// DB削除
		using (var accessor = new SqlAccessor())
		{
			try
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 商品情報削除
				var productService = DomainFacade.Instance.ProductService;
				productService.Delete(this.LoginOperatorShopId, this.RequestProductId, accessor);

				// 商品バリエーション情報削除
				productService.DeleteProductVariationAll(
					this.LoginOperatorShopId,
					this.RequestProductId,
					accessor);

				// 商品価格情報削除
				DomainFacade.Instance.ProductPriceService.DeleteAll(
					this.LoginOperatorShopId,
					this.RequestProductId,
					accessor);

				// 商品タグ情報削除
				DomainFacade.Instance.ProductTagService.Delete(this.RequestProductId, accessor);

				// 商品拡張項目情報削除
				DomainFacade.Instance.ProductExtendService.Delete(
					this.LoginOperatorShopId,
					this.RequestProductId, accessor);

				// モール出品設定情報削除
				DomainFacade.Instance.MallExhibitsConfigService.Delete(
					this.LoginOperatorShopId,
					this.RequestProductId,
					accessor);

				// 在庫管理をする場合
				if (this.ProductInput.StockManagementKbn != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
				{
					// 商品在庫情報削除
					DeleteProductStockAll(this.LoginOperatorShopId, this.RequestProductId, accessor);
				}

				// 商品定期割引情報削除
				DomainFacade.Instance.ProductFixedPurchaseDiscountSettingService.DeleteByShopIdAndProductId(
					this.LoginOperatorShopId,
					this.RequestProductId,
					accessor);

				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				Session[Constants.SESSION_KEY_ERROR_MSG] = ex.ToString();
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}
		}

		if (Constants.PRODUCT_IMAGE_HEAD_ENABLED == false)
		{
			DeleteProductOldImage();
		}

		// 一覧画面へ戻る
		Response.Redirect(CreateProductListUrl((Hashtable)Session[Constants.SESSIONPARAM_KEY_PRODUCT_SEARCH_INFO]));
	}

	/// <summary>
	/// 登録する/更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsertUpdate_Click(object sender, System.EventArgs e)
	{
		// セッション型チェック(ポップアップ同時起動対策)
		if (((Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO] is ProductInput) == false)
			|| ((Session[Constants.SESSIONPARAM_KEY_UPLODED_MASTER] is UploadImageInput) == false))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		this.ProductInput = (ProductInput)Session[Constants.SESSIONPARAM_KEY_PRODUCT_INFO];
		this.UploadImageInput = (UploadImageInput)Session[Constants.SESSIONPARAM_KEY_UPLODED_MASTER];

		// 商品IDの整合性チェック
		if ((this.ActionStatus == Constants.ACTION_STATUS_UPDATE)
			&& (this.RequestProductId != this.ProductInput.ProductId))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		if (this.ProductInput.HasProductVariation
			&& (this.ProductInput.ProductId != this.ProductInput.ProductVariations[0].ProductId))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_POPUP_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// Insert/Update process
		using (var accessor = new SqlAccessor())
		{
			try
			{
				// Insert update product image
				ExecUploadProductImageProcess();

				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var model = this.ProductInput.CreateModel();
				InsertUpdateProduct(this.ActionStatus, model, accessor, true);

				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR);
				Response.Redirect(new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR).CreateUrl());
			}
		}

		// 登録・更新完了メッセージをセット
		Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_REGIST_UPDATE_SUCCESS);
		Session[Constants.SESSION_KEY_SHOW_CONTINUE_REGISTER_BUTTON] = (this.ActionStatus == Constants.ACTION_STATUS_UPDATE) ? false : true;

		// 詳細表示画面へ遷移
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_CONFIRM)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, this.ProductInput.ProductId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_COMPLETE);
		Response.Redirect(urlCreator.CreateUrl());
	}

	/// <summary>
	/// ERPから最新情報取得クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGetLatestInfoFromErp_OnClick(object sender, EventArgs e)
	{
		if (Constants.FLAPS_OPTION_ENABLE == false) return;

		var result = new FlapsIntegrationFacade().CaptureChangedProducts(this.RequestProductId);

		divComp.Visible = true;
		lMessage.Text = WebMessages.GetMessages(
			WebMessages.ERRMSG_MANAGER_PRODUCT_REGIST_FLAPS_UPDATE_RESULT,
			new[] { result.Total.ToString(), result.Success.ToString() });
	}

	/// <summary>
	/// Replace special characters
	/// </summary>
	/// <param name="input">Input</param>
	/// <returns>Replaced input</returns>
	private string ReplaceSpecialCharacters(string input)
	{
		if (string.IsNullOrEmpty(input)) return string.Empty;

		var result = input
			.Replace(@"(", @"\(")
			.Replace(@")", @"\)");

		return result;
	}
	#endregion

	#region +Properties
	/// <summary>Product input</summary>
	protected ProductInput ProductInput
	{
		get { return (ProductInput)ViewState["product_input"]; }
		set { ViewState["product_input"] = value; }
	}
	/// <summary>Related us products</summary>
	protected ProductModel[] RelatedUsProducts
	{
		get { return (ProductModel[])(ViewState["related_us_products"] ?? new ProductModel[0]); }
		set { ViewState["related_us_products"] = value; }
	}
	/// <summary>Related cs products</summary>
	protected ProductModel[] RelatedCsProducts
	{
		get { return (ProductModel[])(ViewState["related_cs_products"] ?? new ProductModel[0]); }
		set { ViewState["related_cs_products"] = value; }
	}
	/// <summary>Upload image input</summary>
	protected UploadImageInput UploadImageInput
	{
		get { return (UploadImageInput)ViewState["upload_image_input"]; }
		set { ViewState["upload_image_input"] = value; }
	}
	/// <summary>モール出品設定</summary>
	protected List<KeyValuePair<string, string>> MallExhibitsConfig
	{
		get { return (List<KeyValuePair<string, string>>)ViewState["mall_exhibits_config"]; }
		set { ViewState["mall_exhibits_config"] = value; }
	}
	/// <summary>Product extends</summary>
	protected List<Hashtable> ProductExtends
	{
		get { return (List<Hashtable>)ViewState["product_extends"]; }
		set { ViewState["product_extends"] = value; }
	}
	/// <summary>商品付帯情報一覧</summary>
	protected ProductOptionSettingList ProductOptionSettingLists
	{
		get { return (ProductOptionSettingList)ViewState["product_option_setting_lists"]; }
		set { ViewState["product_option_setting_lists"] = value; }
	}
	/// <summary>定期購入が可能な場合</summary>
	protected bool CanFixedPurchase
	{
		get
		{
			var result = (this.ProductInput.FixedPurchaseFlg != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID);
			return result;
		}
	}
	/// <summary>定期購入割引設定</summary>
	protected List<ProductFixedPurchaseDiscountSettingHeader> ProductFixedPurchaseDiscountSettings { get; set; }
	/// <summary>Has product category</summary>
	protected bool HasProductCategory
	{
		get
		{
			var result = ((string.IsNullOrEmpty(this.ProductInput.CategoryId1) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.CategoryId2) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.CategoryId3) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.CategoryId4) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.CategoryId5) == false));
			return result;
		}
	}
	/// <summary>Has product brand</summary>
	protected bool HasProductBrand
	{
		get
		{
			var result = ((string.IsNullOrEmpty(this.ProductInput.BrandId1) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.BrandId2) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.BrandId3) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.BrandId4) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.BrandId5) == false));
			return result;
		}
	}
	/// <summary>Has related cs product</summary>
	protected bool HasRelatedCsProduct
	{
		get
		{
			var result = ((string.IsNullOrEmpty(this.ProductInput.RelatedProductIdCs1) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.RelatedProductIdCs2) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.RelatedProductIdCs3) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.RelatedProductIdCs4) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.RelatedProductIdCs5) == false));
			return result;
		}
	}
	/// <summary>Has related us product</summary>
	protected bool HasRelatedUsProduct
	{
		get
		{
			var result = ((string.IsNullOrEmpty(this.ProductInput.RelatedProductIdUs1) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.RelatedProductIdUs2) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.RelatedProductIdUs3) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.RelatedProductIdUs4) == false)
				|| (string.IsNullOrEmpty(this.ProductInput.RelatedProductIdUs5) == false));
			return result;
		}
	}
	/// <summary>カート投入URLを表示</summary>
	protected bool DisplayAddCartUrl
	{
		get
		{
			switch (this.ActionStatus)
			{
				case Constants.ACTION_STATUS_UPDATE:
				case Constants.ACTION_STATUS_COMPLETE:
				case Constants.ACTION_STATUS_INSERT:
				case Constants.ACTION_STATUS_COPY_INSERT:
					return false;

				default:
					return true;
			}
		}
	}
	/// <summary>通常購入用カート投入URLを表示</summary>
	protected bool DisplayNormalAddCartUrl
	{
		get
		{
			if ((this.ProductInput.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY)
				|| (this.ProductInput.GiftFlg == Constants.FLG_PRODUCT_GIFT_FLG_ONLY)) return false;
			return true;
		}
	}
	/// <summary> 定期購入用カート投入URLを表示 </summary>
	protected bool DisplayFixedPurchaseAddCartUrl
	{
		get
		{
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED == false) return false;

			var isNotGiftOnly = (this.ProductInput.GiftFlg != Constants.FLG_PRODUCT_GIFT_FLG_ONLY);
			var isValidFixedPurchase = (this.ProductInput.FixedPurchaseFlg != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID);
			var isNotSubscriptionOnly = (this.ProductInput.SubscriptionBoxFlg != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY);

			return (isNotGiftOnly
				&& isValidFixedPurchase
				&& isNotSubscriptionOnly);
		}
	}
	/// <summary>ギフト購入用カート投入URLを表示</summary>
	protected bool DisplayGiftAddCartUrl
	{
		get
		{
			if ((Constants.GIFTORDER_OPTION_ENABLED == false)
				|| (this.ProductInput.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY)
				|| (this.ProductInput.GiftFlg == Constants.FLG_PRODUCT_GIFT_FLG_INVALID)) return false;
			return true;
		}
	}
	/// <summary>頒布会可能な場合</summary>
	protected bool CanSubscriptionBox
	{
		get
		{
			var result = (this.ProductInput.SubscriptionBoxFlg != Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID);
			return result;
		}
	}
	/// <summary>ブランドID1</summary>
	private string BrandId1
	{
		get { return (string)ViewState[Constants.FIELD_PRODUCT_BRAND_ID1]; }
		set { ViewState[Constants.FIELD_PRODUCT_BRAND_ID1] = value; }
	}
	/// <summary>商品翻訳設定情報</summary>
	protected NameTranslationSettingModel[] ProductTranslationData
	{
		get { return (NameTranslationSettingModel[])ViewState["product_translation_data"]; }
		set { ViewState["product_translation_data"] = value; }
	}
	/// <summary>商品バリエーション翻訳設定情報</summary>
	protected NameTranslationSettingModel[] ProductVariationTranslationData
	{
		get { return (NameTranslationSettingModel[])ViewState["product_variation_translation_data"]; }
		set { ViewState["product_variation_translation_data"] = value; }
	}
	/// <summary>会員ランク価格マスタ（バリエーション）</summary>
	protected List<Hashtable> MemberRankPrices
	{
		get { return (List<Hashtable>)ViewState["member_rank_prices"]; }
		set { ViewState["member_rank_prices"] = value; }
	}
	/// <summary>商品バリエーション情報</summary>
	protected ProductVariationInput[] ProductVariations
	{
		get { return (ProductVariationInput[])ViewState["product_variations"]; }
		set { ViewState["product_variations"] = value; }
	}
	/// <summary>商品商品タグ情報</summary>
	protected List<Hashtable> ProductTags
	{
		get { return (List<Hashtable>)ViewState["product_tags"]; }
		set { ViewState["product_tags"] = value; }
	}
	/// <summary>An upload image temporary path</summary>
	private string UploadImageTempPathRelative
	{
		get
		{
			return Path.Combine(
				Constants.PATH_ROOT_FRONT_PC,
				Constants.PATH_TEMP,
				"ProductImages",
				this.UploadImageInput.Guid);
		}
	}
	/// <summary>An upload image temporary path</summary>
	private string UploadImageTempPathAbsolute
	{
		get
		{
			return Path.Combine(
				Constants.PHYSICALDIRPATH_CONTENTS_ROOT,
				Constants.PATH_TEMP.Replace("/", @"\"),
				"ProductImages",
				this.UploadImageInput.Guid);
		}
	}
	/// <summary>An upload image main path</summary>
	private string UploadImageMainPath
	{
		get
		{
			return Path.Combine(
				Constants.PHYSICALDIRPATH_CONTENTS_ROOT,
				Constants.PATH_PRODUCTIMAGES.Replace("/", @"\"),
				this.LoginOperatorShopId);
		}
	}
	/// <summary>An upload image sub path</summary>
	private string UploadImageSubPath
	{
		get
		{
			return Path.Combine(
				Constants.PHYSICALDIRPATH_CONTENTS_ROOT,
				Constants.PATH_PRODUCTSUBIMAGES.Replace("/", @"\"),
				this.LoginOperatorShopId);
		}
	}
	/// <summary>Display product stock</summary>
	protected bool DisplayProductStock
	{
		get
		{
			switch (this.ActionStatus)
			{
				case Constants.ACTION_STATUS_DETAIL:
				case Constants.ACTION_STATUS_COMPLETE:
					return (this.ProductVariations.Length == 0);

				case Constants.ACTION_STATUS_UPDATE:
					return false;

				default:
					return true;
			}
		}
	}
	/// <summary>Is display dixed purchase area</summary>
	protected bool IsDisplayFixedpurchaseArea
	{
		get
		{
			return (GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT)
				|| GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS)
				|| (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED
					&& GetProductDefaultSettingDisplayField(PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING))
				|| (Constants.FIXED_PURCHASE_DISCOUNT_PRICE_OPTION_ENABLE && (dvProductFixedPurchaseDiscountSection.Visible))
				|| (Constants.PRODUCT_ORDER_LIMIT_ENABLED
					&& GetProductDefaultSettingDisplayField(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG)));
		}
	}
	/// <summary>Sub image settings</summary>
	protected KeyValuePair<int, string>[] SubImageSettings
	{
		get { return (KeyValuePair<int, string>[])ViewState["sub_image_settings"]; }
		set { ViewState["sub_image_settings"] = value; }
	}
	#endregion
}
