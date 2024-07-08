/*
=========================================================================================================
  Module      : 商品入力クラス (ProductInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common;
using w2.App.Common.Input;
using w2.App.Common.Product;
using w2.Common.Util;
using w2.Domain.Product;
using w2.Domain.ProductFixedPurchaseDiscountSetting;
using w2.Domain.ProductPrice;
using w2.Domain.ProductStock;

/// <summary>
/// 商品入力クラス
/// </summary>
[Serializable]
public class ProductInput : InputBase<ProductModel>
{
	/// <summary>Error key: product options</summary>
	public const string CONST_ERROR_KEY_PRODUCT_OPTIONS = "error_product_options";
	/// <summary>Error key: product stocks</summary>
	public const string CONST_ERROR_KEY_PRODUCT_STOCKS = "error_product_stocks";
	/// <summary>Error key: Product variation duplication id</summary>
	public const string CONST_ERROR_KEY_PRODUCT_VARIATION_DUPLICATION_ID = "product_variation_duplication_id";

	/// <summary>Field: Product display to check</summary>
	public const string CONST_FIELD_PRODUCT_DISPLAY_TO_CHECK = Constants.FIELD_PRODUCT_DISPLAY_TO + "_check";
	/// <summary>Field: Product sell to check</summary>
	public const string CONST_FIELD_PRODUCT_SELL_TO_CHECK = Constants.FIELD_PRODUCT_SELL_TO + "_check";
	/// <summary>Field: Base product variation cooperation id</summary>
	public const string CONST_FIELD_BASE_PRODUCT_VARIATION_COOPERATION_ID = "variation_cooperation_id";

	/// <summary>Product field name</summary>
	public const string CONST_PRODUCT_FIELD_NAME = "product_field_name";
	/// <summary>Replace key</summary>
	public const string CONST_REPLACE_KEY = "@@ 1 @@";

	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public ProductInput()
		: base()
	{
		this.ProductVariations = new ProductVariationInput[0];
		this.ProductPrices = new ProductPriceInput[0];
		this.ProductFixedPurchaseDiscountSettings = new ProductFixedPurchaseDiscountSettingHeader[0];
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ProductInput(ProductModel model)
		: this()
	{
		this.ShopId = model.ShopId;
		this.SupplierId = model.SupplierId;
		this.ProductId = model.ProductId;
		this.CooperationId1 = model.CooperationId1;
		this.CooperationId2 = model.CooperationId2;
		this.CooperationId3 = model.CooperationId3;
		this.CooperationId4 = model.CooperationId4;
		this.CooperationId5 = model.CooperationId5;
		this.MallExProductId = model.MallExProductId;
		this.MakerId = model.MakerId;
		this.MakerName = model.MakerName;
		this.Name = model.Name;
		this.NameKana = model.NameKana;
		this.Name2 = model.Name2;
		this.Name2Kana = model.Name2Kana;
		this.SeoKeywords = model.SeoKeywords;
		this.Catchcopy = model.Catchcopy;
		this.CatchcopyMobile = model.CatchcopyMobile;
		this.OutlineKbn = model.OutlineKbn;
		this.Outline = model.Outline;
		this.OutlineKbnMobile = model.OutlineKbnMobile;
		this.OutlineMobile = model.OutlineMobile;
		this.DescDetailKbn1 = model.DescDetailKbn1;
		this.DescDetail1 = model.DescDetail1;
		this.DescDetailKbn2 = model.DescDetailKbn2;
		this.DescDetail2 = model.DescDetail2;
		this.DescDetailKbn3 = model.DescDetailKbn3;
		this.DescDetail3 = model.DescDetail3;
		this.DescDetailKbn4 = model.DescDetailKbn4;
		this.DescDetail4 = model.DescDetail4;
		this.ReturnExchangeMessage = model.ReturnExchangeMessage;
		this.DescDetailKbn1Mobile = model.DescDetailKbn1Mobile;
		this.DescDetail1Mobile = model.DescDetail1Mobile;
		this.DescDetailKbn2Mobile = model.DescDetailKbn2Mobile;
		this.DescDetail2Mobile = model.DescDetail2Mobile;
		this.ReturnExchangeMessageMobile = model.ReturnExchangeMessageMobile;
		this.Note = model.Note;
		this.DisplayPrice = StringUtility.ToEmpty(model.DisplayPrice);
		this.DisplaySpecialPrice = StringUtility.ToEmpty(model.DisplaySpecialPrice);
		this.TaxIncludedFlg = model.TaxIncludedFlg;
		this.TaxRate = StringUtility.ToEmpty(model.TaxRate);
		this.TaxRoundType = model.TaxRoundType;
		this.PricePretax = StringUtility.ToEmpty(model.PricePretax);
		this.PriceShipping = StringUtility.ToEmpty(model.PriceShipping);
		this.ShippingType = model.ShippingType;
		this.ShippingSizeKbn = model.ShippingSizeKbn;
		this.PriceCost = StringUtility.ToEmpty(model.PriceCost);
		this.PointKbn1 = model.PointKbn1;
		this.Point1 = StringUtility.ToEmpty(model.Point1);
		this.PointKbn2 = model.PointKbn2;
		this.Point2 = StringUtility.ToEmpty(model.Point2);
		this.PointKbn3 = model.PointKbn3;
		this.Point3 = StringUtility.ToEmpty(model.Point3);
		this.MemberRankPointExcludeFlg = StringUtility.ToEmpty(model.MemberRankPointExcludeFlg);
		this.CampaignFrom = StringUtility.ToEmpty(model.CampaignFrom);
		this.CampaignTo = StringUtility.ToEmpty(model.CampaignTo);
		this.CampaignPointKbn = model.CampaignPointKbn;
		this.CampaignPoint = StringUtility.ToEmpty(model.CampaignPoint);
		this.DisplayFrom = StringUtility.ToEmpty(model.DisplayFrom);
		this.DisplayTo = StringUtility.ToEmpty(model.DisplayTo);
		this.SellFrom = StringUtility.ToEmpty(model.SellFrom);
		this.SellTo = StringUtility.ToEmpty(model.SellTo);
		this.BeforeSaleDisplayKbn = model.BeforeSaleDisplayKbn;
		this.AfterSaleDisplayKbn = model.AfterSaleDisplayKbn;
		this.MaxSellQuantity = StringUtility.ToEmpty(model.MaxSellQuantity);
		this.StockManagementKbn = model.StockManagementKbn;
		this.StockDispKbn = model.StockDispKbn;
		this.StockMessageId = model.StockMessageId;
		this.Url = model.Url;
		this.InquireEmail = model.InquireEmail;
		this.InquireTel = model.InquireTel;
		this.DisplayKbn = model.DisplayKbn;
		this.CategoryId1 = model.CategoryId1;
		this.CategoryId2 = model.CategoryId2;
		this.CategoryId3 = model.CategoryId3;
		this.CategoryId4 = model.CategoryId4;
		this.CategoryId5 = model.CategoryId5;
		this.RelatedProductIdCs1 = model.RelatedProductIdCs1;
		this.RelatedProductIdCs2 = model.RelatedProductIdCs2;
		this.RelatedProductIdCs3 = model.RelatedProductIdCs3;
		this.RelatedProductIdCs4 = model.RelatedProductIdCs4;
		this.RelatedProductIdCs5 = model.RelatedProductIdCs5;
		this.RelatedProductIdUs1 = model.RelatedProductIdUs1;
		this.RelatedProductIdUs2 = model.RelatedProductIdUs2;
		this.RelatedProductIdUs3 = model.RelatedProductIdUs3;
		this.RelatedProductIdUs4 = model.RelatedProductIdUs4;
		this.RelatedProductIdUs5 = model.RelatedProductIdUs5;
		this.ImageHead = model.ImageHead;
		this.ImageMobile = model.ImageMobile;
		this.IconFlg1 = model.IconFlg1;
		this.IconTermEnd1 = StringUtility.ToEmpty(model.IconTermEnd1);
		this.IconFlg2 = model.IconFlg2;
		this.IconTermEnd2 = StringUtility.ToEmpty(model.IconTermEnd2);
		this.IconFlg3 = model.IconFlg3;
		this.IconTermEnd3 = StringUtility.ToEmpty(model.IconTermEnd3);
		this.IconFlg4 = model.IconFlg4;
		this.IconTermEnd4 = StringUtility.ToEmpty(model.IconTermEnd4);
		this.IconFlg5 = model.IconFlg5;
		this.IconTermEnd5 = StringUtility.ToEmpty(model.IconTermEnd5);
		this.IconFlg6 = model.IconFlg6;
		this.IconTermEnd6 = StringUtility.ToEmpty(model.IconTermEnd6);
		this.IconFlg7 = model.IconFlg7;
		this.IconTermEnd7 = StringUtility.ToEmpty(model.IconTermEnd7);
		this.IconFlg8 = model.IconFlg8;
		this.IconTermEnd8 = StringUtility.ToEmpty(model.IconTermEnd8);
		this.IconFlg9 = model.IconFlg9;
		this.IconTermEnd9 = StringUtility.ToEmpty(model.IconTermEnd9);
		this.IconFlg10 = model.IconFlg10;
		this.IconTermEnd10 = StringUtility.ToEmpty(model.IconTermEnd10);
		this.MobileDispFlg = model.MobileDispFlg;
		this.UseVariationFlg = model.UseVariationFlg;
		this.ReservationFlg = model.ReservationFlg;
		this.FixedPurchaseFlg = model.FixedPurchaseFlg;
		this.CheckFixedProductOrderFlg = model.CheckFixedProductOrderFlg;
		this.CheckProductOrderFlg = model.CheckProductOrderFlg;
		this.MallCooperatedFlg = model.MallCooperatedFlg;
		this.ValidFlg = model.ValidFlg;
		this.DelFlg = model.DelFlg;
		this.SearchKeyword = model.SearchKeyword;
		this.MemberRankDiscountFlg = model.MemberRankDiscountFlg;
		this.DisplayMemberRank = model.DisplayMemberRank;
		this.BuyableMemberRank = model.BuyableMemberRank;
		this.GoogleShoppingFlg = model.GoogleShoppingFlg;
		this.ProductOptionSettings = model.ProductOptionSettings;
		this.ArrivalMailValidFlg = model.ArrivalMailValidFlg;
		this.ReleaseMailValidFlg = model.ReleaseMailValidFlg;
		this.ResaleMailValidFlg = model.ResaleMailValidFlg;
		this.SelectVariationKbn = model.SelectVariationKbn;
		this.SelectVariationKbnMobile = model.SelectVariationKbnMobile;
		this.BrandId1 = model.BrandId1;
		this.BrandId2 = model.BrandId2;
		this.BrandId3 = model.BrandId3;
		this.BrandId4 = model.BrandId4;
		this.BrandId5 = model.BrandId5;
		this.UseRecommendFlg = model.UseRecommendFlg;
		this.GiftFlg = model.GiftFlg;
		this.AgeLimitFlg = model.AgeLimitFlg;
		this.PluralShippingPriceFreeFlg = model.PluralShippingPriceFreeFlg;
		this.DigitalContentsFlg = model.DigitalContentsFlg;
		this.DownloadUrl = model.DownloadUrl;
		this.DisplaySellFlg = model.DisplaySellFlg;
		this.DisplayPriority = StringUtility.ToEmpty(model.DisplayPriority);
		this.LimitedPaymentIds = model.LimitedPaymentIds;
		this.FixedPurchaseFirsttimePrice = StringUtility.ToEmpty(model.FixedPurchaseFirsttimePrice);
		this.FixedPurchasePrice = StringUtility.ToEmpty(model.FixedPurchasePrice);
		this.BundleItemDisplayType = model.BundleItemDisplayType;
		this.ProductType = model.ProductType;
		this.LimitedFixedPurchaseKbn1Setting = model.LimitedFixedPurchaseKbn1Setting;
		this.LimitedFixedPurchaseKbn3Setting = model.LimitedFixedPurchaseKbn3Setting;
		this.LimitedFixedPurchaseKbn4Setting = model.LimitedFixedPurchaseKbn4Setting;
		this.CooperationId6 = model.CooperationId6;
		this.CooperationId7 = model.CooperationId7;
		this.CooperationId8 = model.CooperationId8;
		this.CooperationId9 = model.CooperationId9;
		this.CooperationId10 = model.CooperationId10;
		this.AndmallReservationFlg = model.AndmallReservationFlg;
		this.DisplayOnlyFixedPurchaseMemberFlg = model.DisplayOnlyFixedPurchaseMemberFlg;
		this.SellOnlyFixedPurchaseMemberFlg = model.SellOnlyFixedPurchaseMemberFlg;
		this.ProductWeightGram = StringUtility.ToEmpty(model.ProductWeightGram);
		this.TaxCategoryId = model.TaxCategoryId;
		this.FixedPurchasedCancelableCount = StringUtility.ToEmpty(model.FixedPurchasedCancelableCount);
		this.FixedPurchasedLimitedUserLevelIds = model.FixedPurchasedLimitedUserLevelIds;
		this.FixedPurchaseNextShippingProductId = StringUtility.ToEmpty(model.FixedPurchaseNextShippingProductId);
		this.FixedPurchaseNextShippingVariationId = model.FixedPurchaseNextShippingVariationId;
		this.FixedPurchaseNextShippingItemQuantity = StringUtility.ToEmpty(model.FixedPurchaseNextShippingItemQuantity);
		this.FixedPurchaseLimitedSkippedCount = StringUtility.ToEmpty(model.FixedPurchaseLimitedSkippedCount);
		this.NextShippingItemFixedPurchaseKbn = model.NextShippingItemFixedPurchaseKbn;
		this.NextShippingItemFixedPurchaseSetting = model.NextShippingItemFixedPurchaseSetting;
		this.RecommendProductId = model.RecommendProductId;
		this.LastChanged = model.LastChanged;
		this.ProductColorId = model.ProductColorId;
		this.ProductSizeFactor = StringUtility.ToEmpty(model.ProductSizeFactor);
		this.ProductPrices = model.HasProductPrices
			? model.ProductPrices.Select(item => new ProductPriceInput(item)).ToArray()
			: new ProductPriceInput[0];
		this.ProductVariations = model.HasProductVariations
			? model.ProductVariations.Select(item => new ProductVariationInput(item)).ToArray()
			: new ProductVariationInput[0];
		this.ProductTag = model.HasProductTag
			? new ProductTagInput(model.ProductTag)
			: null;
		this.ProductExtend = model.HasProductExtend
			? new ProductExtendInput(model.ProductExtend)
			: null;
		this.MallExhibitsConfig = model.HasMallExhibitsConfig
			? new MallExhibitsConfigInput(model.MallExhibitsConfig)
			: null;
		this.ProductFixedPurchaseDiscountSettings = model.HasProductFixedPurchaseDiscountSettings
			? ProductFixedPurchaseDiscountSettingHeader
				.CreateProductFixedPurchaseDiscountSettingHeader(model.ProductFixedPurchaseDiscountSettings)
				.ToArray()
			: new ProductFixedPurchaseDiscountSettingHeader[0];
		this.AddCartUrlLimitFlg = model.AddCartUrlLimitFlg;
		this.ProductStocks = model.HasProductStocks
			? model.ProductStocks.Select(item => new ProductStockInput(item)).ToArray()
			: new ProductStockInput[0];
		this.DateCreated = StringUtility.ToEmpty(model.DateCreated);
		this.DateChanged = StringUtility.ToEmpty(model.DateChanged);
		this.ShopShippingName = model.ShopShippingName;
		this.ProductTaxCategoryName = model.ProductTaxCategoryName;
		this.ProductStockMessageName = model.ProductStockMessageName;
		this.ProductCategoryName1 = model.ProductCategoryName1;
		this.ProductCategoryName2 = model.ProductCategoryName2;
		this.ProductCategoryName3 = model.ProductCategoryName3;
		this.ProductCategoryName4 = model.ProductCategoryName4;
		this.ProductCategoryName5 = model.ProductCategoryName5;
		this.ProductBrandName1 = model.ProductBrandName1;
		this.ProductBrandName2 = model.ProductBrandName2;
		this.ProductBrandName3 = model.ProductBrandName3;
		this.ProductBrandName4 = model.ProductBrandName4;
		this.ProductBrandName5 = model.ProductBrandName5;
		this.SubscriptionBoxFlg = model.SubscriptionBoxFlg;
		this.StorePickupFlg = model.StorePickupFlg;
		this.ExcludeFreeShippingFlg = model.ExcludeFreeShippingFlg;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="source">ソース</param>
	public ProductInput(Hashtable source)
		: this(new ProductModel())
	{
		// Copy input source values to current source
		foreach (DictionaryEntry item in source)
		{
			// If the value is null, then continue
			if (item.Value == null) continue;

			this.DataSource[item.Key] = item.Value;
		}
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ProductModel CreateModel()
	{
		var model = new ProductModel
		{
			ShopId = StringUtility.ToEmpty(this.ShopId),
			SupplierId = StringUtility.ToEmpty(this.SupplierId),
			ProductId = StringUtility.ToEmpty(this.ProductId),
			CooperationId1 = StringUtility.ToEmpty(this.CooperationId1),
			CooperationId2 = StringUtility.ToEmpty(this.CooperationId2),
			CooperationId3 = StringUtility.ToEmpty(this.CooperationId3),
			CooperationId4 = StringUtility.ToEmpty(this.CooperationId4),
			CooperationId5 = StringUtility.ToEmpty(this.CooperationId5),
			MallExProductId = StringUtility.ToEmpty(this.MallExProductId),
			MakerId = StringUtility.ToEmpty(this.MakerId),
			MakerName = StringUtility.ToEmpty(this.MakerName),
			Name = StringUtility.ToEmpty(this.Name),
			NameKana = StringUtility.ToEmpty(this.NameKana),
			Name2 = StringUtility.ToEmpty(this.Name2),
			Name2Kana = StringUtility.ToEmpty(this.Name2Kana),
			SeoKeywords = StringUtility.ToEmpty(this.SeoKeywords),
			Catchcopy = StringUtility.ToEmpty(this.Catchcopy),
			CatchcopyMobile = StringUtility.ToEmpty(this.CatchcopyMobile),
			OutlineKbn = StringUtility.ToEmpty(this.OutlineKbn),
			Outline = StringUtility.ToEmpty(this.Outline),
			OutlineKbnMobile = StringUtility.ToEmpty(this.OutlineKbnMobile),
			OutlineMobile = StringUtility.ToEmpty(this.OutlineMobile),
			DescDetailKbn1 = StringUtility.ToEmpty(this.DescDetailKbn1),
			DescDetail1 = StringUtility.ToEmpty(this.DescDetail1),
			DescDetailKbn2 = StringUtility.ToEmpty(this.DescDetailKbn2),
			DescDetail2 = StringUtility.ToEmpty(this.DescDetail2),
			DescDetailKbn3 = StringUtility.ToEmpty(this.DescDetailKbn3),
			DescDetail3 = StringUtility.ToEmpty(this.DescDetail3),
			DescDetailKbn4 = StringUtility.ToEmpty(this.DescDetailKbn4),
			DescDetail4 = StringUtility.ToEmpty(this.DescDetail4),
			ReturnExchangeMessage = StringUtility.ToEmpty(this.ReturnExchangeMessage),
			DescDetailKbn1Mobile = StringUtility.ToEmpty(this.DescDetailKbn1Mobile),
			DescDetail1Mobile = StringUtility.ToEmpty(this.DescDetail1Mobile),
			DescDetailKbn2Mobile = StringUtility.ToEmpty(this.DescDetailKbn2Mobile),
			DescDetail2Mobile = StringUtility.ToEmpty(this.DescDetail2Mobile),
			ReturnExchangeMessageMobile = StringUtility.ToEmpty(this.ReturnExchangeMessageMobile),
			Note = StringUtility.ToEmpty(this.Note),
			DisplayPrice = ObjectUtility.TryParseDecimal(this.DisplayPrice, 0m),
			DisplaySpecialPrice = ObjectUtility.TryParseDecimal(this.DisplaySpecialPrice),
			TaxIncludedFlg = StringUtility.ToEmpty(this.TaxIncludedFlg),
			TaxRate = ObjectUtility.TryParseDecimal(this.TaxRate, 0m),
			TaxRoundType = StringUtility.ToEmpty(this.TaxRoundType),
			PricePretax = ObjectUtility.TryParseDecimal(this.PricePretax, 0m),
			PriceShipping = ObjectUtility.TryParseDecimal(this.PriceShipping),
			ShippingType = StringUtility.ToEmpty(this.ShippingType),
			ShippingSizeKbn = StringUtility.ToEmpty(this.ShippingSizeKbn),
			PriceCost = ObjectUtility.TryParseDecimal(this.PriceCost),
			PointKbn1 = StringUtility.ToEmpty(this.PointKbn1),
			PointKbn2 = StringUtility.ToEmpty(this.PointKbn2),
			PointKbn3 = StringUtility.ToEmpty(this.PointKbn3),
			Point1 = ObjectUtility.TryParseDecimal(this.Point1, 0m),
			Point2 = ObjectUtility.TryParseDecimal(this.Point2, 0m),
			Point3 = ObjectUtility.TryParseDecimal(this.Point3, 0m),
			CampaignFrom = ObjectUtility.TryParseDateTime(this.CampaignFrom),
			CampaignTo = ObjectUtility.TryParseDateTime(this.CampaignTo),
			CampaignPointKbn = StringUtility.ToEmpty(this.CampaignPointKbn),
			CampaignPoint = ObjectUtility.TryParseInt(this.CampaignPoint, 0),
			DisplayFrom = ObjectUtility.TryParseDateTime(this.DisplayFrom, DateTime.Now),
			DisplayTo = ObjectUtility.TryParseDateTime(this.DisplayTo),
			SellFrom = ObjectUtility.TryParseDateTime(this.SellFrom, DateTime.Now),
			SellTo = ObjectUtility.TryParseDateTime(this.SellTo),
			BeforeSaleDisplayKbn = this.BeforeSaleDisplayKbn,
			AfterSaleDisplayKbn = this.AfterSaleDisplayKbn,
			MaxSellQuantity = ObjectUtility.TryParseInt(this.MaxSellQuantity, 0),
			StockManagementKbn = StringUtility.ToEmpty(this.StockManagementKbn),
			StockDispKbn = StringUtility.ToEmpty(this.StockDispKbn),
			StockMessageId = StringUtility.ToEmpty(this.StockMessageId),
			Url = StringUtility.ToEmpty(this.Url),
			InquireEmail = StringUtility.ToEmpty(this.InquireEmail),
			InquireTel = StringUtility.ToEmpty(this.InquireTel),
			DisplayKbn = StringUtility.ToEmpty(this.DisplayKbn),
			CategoryId1 = StringUtility.ToEmpty(this.CategoryId1),
			CategoryId2 = StringUtility.ToEmpty(this.CategoryId2),
			CategoryId3 = StringUtility.ToEmpty(this.CategoryId3),
			CategoryId4 = StringUtility.ToEmpty(this.CategoryId4),
			CategoryId5 = StringUtility.ToEmpty(this.CategoryId5),
			RelatedProductIdCs1 = StringUtility.ToEmpty(this.RelatedProductIdCs1),
			RelatedProductIdCs2 = StringUtility.ToEmpty(this.RelatedProductIdCs2),
			RelatedProductIdCs3 = StringUtility.ToEmpty(this.RelatedProductIdCs3),
			RelatedProductIdCs4 = StringUtility.ToEmpty(this.RelatedProductIdCs4),
			RelatedProductIdCs5 = StringUtility.ToEmpty(this.RelatedProductIdCs5),
			RelatedProductIdUs1 = StringUtility.ToEmpty(this.RelatedProductIdUs1),
			RelatedProductIdUs2 = StringUtility.ToEmpty(this.RelatedProductIdUs2),
			RelatedProductIdUs3 = StringUtility.ToEmpty(this.RelatedProductIdUs3),
			RelatedProductIdUs4 = StringUtility.ToEmpty(this.RelatedProductIdUs4),
			RelatedProductIdUs5 = StringUtility.ToEmpty(this.RelatedProductIdUs5),
			ImageHead = StringUtility.ToEmpty(this.ImageHead),
			ImageMobile = StringUtility.ToEmpty(this.ImageMobile),
			IconFlg1 = StringUtility.ToEmpty(this.IconFlg1),
			IconFlg2 = StringUtility.ToEmpty(this.IconFlg2),
			IconFlg3 = StringUtility.ToEmpty(this.IconFlg3),
			IconFlg4 = StringUtility.ToEmpty(this.IconFlg4),
			IconFlg5 = StringUtility.ToEmpty(this.IconFlg5),
			IconFlg6 = StringUtility.ToEmpty(this.IconFlg6),
			IconFlg7 = StringUtility.ToEmpty(this.IconFlg7),
			IconFlg8 = StringUtility.ToEmpty(this.IconFlg8),
			IconFlg9 = StringUtility.ToEmpty(this.IconFlg9),
			IconFlg10 = StringUtility.ToEmpty(this.IconFlg10),
			IconTermEnd1 = ObjectUtility.TryParseDateTime(this.IconTermEnd1),
			IconTermEnd2 = ObjectUtility.TryParseDateTime(this.IconTermEnd2),
			IconTermEnd3 = ObjectUtility.TryParseDateTime(this.IconTermEnd3),
			IconTermEnd4 = ObjectUtility.TryParseDateTime(this.IconTermEnd4),
			IconTermEnd5 = ObjectUtility.TryParseDateTime(this.IconTermEnd5),
			IconTermEnd6 = ObjectUtility.TryParseDateTime(this.IconTermEnd6),
			IconTermEnd7 = ObjectUtility.TryParseDateTime(this.IconTermEnd7),
			IconTermEnd8 = ObjectUtility.TryParseDateTime(this.IconTermEnd8),
			IconTermEnd9 = ObjectUtility.TryParseDateTime(this.IconTermEnd9),
			IconTermEnd10 = ObjectUtility.TryParseDateTime(this.IconTermEnd10),
			MobileDispFlg = StringUtility.ToEmpty(this.MobileDispFlg),
			UseVariationFlg = StringUtility.ToEmpty(this.UseVariationFlg),
			ReservationFlg = StringUtility.ToEmpty(this.ReservationFlg),
			FixedPurchaseFlg = StringUtility.ToEmpty(this.FixedPurchaseFlg),
			CheckFixedProductOrderFlg = StringUtility.ToEmpty(this.CheckFixedProductOrderFlg),
			CheckProductOrderFlg = StringUtility.ToEmpty(this.CheckProductOrderFlg),
			MallCooperatedFlg = StringUtility.ToEmpty(this.MallCooperatedFlg),
			ValidFlg = StringUtility.ToEmpty(this.ValidFlg),
			DelFlg = StringUtility.ToEmpty(this.DelFlg),
			SearchKeyword = StringUtility.ToEmpty(this.SearchKeyword),
			MemberRankDiscountFlg = StringUtility.ToEmpty(this.MemberRankDiscountFlg),
			DisplayMemberRank = StringUtility.ToEmpty(this.DisplayMemberRank),
			BuyableMemberRank = StringUtility.ToEmpty(this.BuyableMemberRank),
			GoogleShoppingFlg = StringUtility.ToEmpty(this.GoogleShoppingFlg),
			ProductOptionSettings = StringUtility.ToEmpty(this.ProductOptionSettings),
			ArrivalMailValidFlg = StringUtility.ToEmpty(this.ArrivalMailValidFlg),
			ReleaseMailValidFlg = StringUtility.ToEmpty(this.ReleaseMailValidFlg),
			ResaleMailValidFlg = StringUtility.ToEmpty(this.ResaleMailValidFlg),
			SelectVariationKbn = StringUtility.ToEmpty(this.SelectVariationKbn),
			SelectVariationKbnMobile = StringUtility.ToEmpty(this.SelectVariationKbnMobile),
			BrandId1 = StringUtility.ToEmpty(this.BrandId1),
			BrandId2 = StringUtility.ToEmpty(this.BrandId2),
			BrandId3 = StringUtility.ToEmpty(this.BrandId3),
			BrandId4 = StringUtility.ToEmpty(this.BrandId4),
			BrandId5 = StringUtility.ToEmpty(this.BrandId5),
			UseRecommendFlg = StringUtility.ToEmpty(this.UseRecommendFlg),
			GiftFlg = StringUtility.ToEmpty(this.GiftFlg),
			AgeLimitFlg = StringUtility.ToEmpty(this.AgeLimitFlg),
			PluralShippingPriceFreeFlg = StringUtility.ToEmpty(this.PluralShippingPriceFreeFlg),
			DigitalContentsFlg = StringUtility.ToEmpty(this.DigitalContentsFlg),
			DownloadUrl = StringUtility.ToEmpty(this.DownloadUrl),
			DisplaySellFlg = StringUtility.ToEmpty(this.DisplaySellFlg),
			DisplayPriority = ObjectUtility.TryParseInt(this.DisplayPriority, 0),
			LimitedPaymentIds = StringUtility.ToEmpty(this.LimitedPaymentIds),
			FixedPurchaseFirsttimePrice = ObjectUtility.TryParseDecimal(this.FixedPurchaseFirsttimePrice),
			FixedPurchasePrice = ObjectUtility.TryParseDecimal(this.FixedPurchasePrice),
			BundleItemDisplayType = StringUtility.ToEmpty(this.BundleItemDisplayType),
			ProductType = StringUtility.ToEmpty(this.ProductType),
			LimitedFixedPurchaseKbn1Setting = StringUtility.ToEmpty(this.LimitedFixedPurchaseKbn1Setting),
			LimitedFixedPurchaseKbn3Setting = StringUtility.ToEmpty(this.LimitedFixedPurchaseKbn3Setting),
			LimitedFixedPurchaseKbn4Setting = StringUtility.ToEmpty(this.LimitedFixedPurchaseKbn4Setting),
			CooperationId6 = StringUtility.ToEmpty(this.CooperationId6),
			CooperationId7 = StringUtility.ToEmpty(this.CooperationId7),
			CooperationId8 = StringUtility.ToEmpty(this.CooperationId8),
			CooperationId9 = StringUtility.ToEmpty(this.CooperationId9),
			CooperationId10 = StringUtility.ToEmpty(this.CooperationId10),
			AndmallReservationFlg = StringUtility.ToEmpty(this.AndmallReservationFlg),
			DisplayOnlyFixedPurchaseMemberFlg = StringUtility.ToEmpty(this.DisplayOnlyFixedPurchaseMemberFlg),
			SellOnlyFixedPurchaseMemberFlg = StringUtility.ToEmpty(this.SellOnlyFixedPurchaseMemberFlg),
			ProductWeightGram = ObjectUtility.TryParseInt(this.ProductWeightGram, 0),
			TaxCategoryId = StringUtility.ToEmpty(this.TaxCategoryId),
			FixedPurchasedCancelableCount = ObjectUtility.TryParseInt(this.FixedPurchasedCancelableCount, 0),
			FixedPurchasedLimitedUserLevelIds = StringUtility.ToEmpty(this.FixedPurchasedLimitedUserLevelIds),
			FixedPurchaseNextShippingProductId = StringUtility.ToEmpty(this.FixedPurchaseNextShippingProductId),
			FixedPurchaseNextShippingVariationId = StringUtility.ToEmpty(this.FixedPurchaseNextShippingVariationId),
			FixedPurchaseNextShippingItemQuantity = ObjectUtility.TryParseInt(this.FixedPurchaseNextShippingItemQuantity, 0),
			FixedPurchaseLimitedSkippedCount = ObjectUtility.TryParseInt(this.FixedPurchaseLimitedSkippedCount),
			NextShippingItemFixedPurchaseKbn = StringUtility.ToEmpty(this.NextShippingItemFixedPurchaseKbn),
			NextShippingItemFixedPurchaseSetting = StringUtility.ToEmpty(this.NextShippingItemFixedPurchaseSetting),
			RecommendProductId = StringUtility.ToEmpty(this.RecommendProductId),
			ProductColorId = StringUtility.ToEmpty(this.ProductColorId),
			ProductSizeFactor = ObjectUtility.TryParseInt(this.ProductSizeFactor, 1),
			LastChanged = StringUtility.ToEmpty(this.LastChanged),
			DateChanged = DateTime.Now,
			ProductPrices = this.HasProductPrices
				? this.ProductPrices.Select(item => item.CreateModel()).ToArray()
				: new ProductPriceModel[0],
			ProductVariations = this.ProductVariations.Select(item => item.CreateModel()).ToArray(),
			ProductTag = this.HasProductTag
				? this.ProductTag.CreateModel()
				: null,
			ProductExtend = this.HasProductExtend
				? this.ProductExtend.CreateModel()
				: null,
			MallExhibitsConfig = this.HasMallExhibitsConfig
				? this.MallExhibitsConfig.CreateModel()
				: null,
			ProductStocks = CreateProductStockModels(),
			ProductFixedPurchaseDiscountSettings = this.HasProductFixedPurchaseDiscountSettings
				? ProductFixedPurchaseDiscountSettingHeader.CreateProductFixedPurchaseDiscountSetting(
					this.ProductFixedPurchaseDiscountSettings.ToList(),
					this.LastChanged)
				: new ProductFixedPurchaseDiscountSettingModel[0],
			AddCartUrlLimitFlg = StringUtility.ToEmpty(this.AddCartUrlLimitFlg),
			SubscriptionBoxFlg = (this.FixedPurchaseFlg != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
				? StringUtility.ToEmpty(this.SubscriptionBoxFlg)
				: Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID,
			MemberRankPointExcludeFlg = StringUtility.ToEmpty(this.MemberRankPointExcludeFlg),
			StorePickupFlg = this.StorePickupFlg,
			ExcludeFreeShippingFlg = this.ExcludeFreeShippingFlg
		};
		return model;
	}

	/// <summary>
	/// Create product stock models
	/// </summary>
	/// <returns>product stock models</returns>
	private ProductStockModel[] CreateProductStockModels()
	{
		if (this.IsStockUnmanaged) return new ProductStockModel[0];

		var stocks = new List<ProductStockModel>();
		if (this.HasProductVariations == false)
		{
			stocks.Add(
				new ProductStockModel
				{
					ShopId = StringUtility.ToEmpty(this.ShopId),
					ProductId = StringUtility.ToEmpty(this.ProductId),
					VariationId = StringUtility.ToEmpty(this.ProductId),
					Stock = ObjectUtility.TryParseInt(this.Stock, 0),
					StockAlert = ObjectUtility.TryParseInt(this.StockAlert, 0),
					UpdateMemo = StringUtility.ToEmpty(this.UpdateMemo),
					LastChanged = StringUtility.ToEmpty(this.LastChanged),
				});
			return stocks.ToArray();
		}

		foreach (var variation in this.ProductVariations)
		{
			stocks.Add(
				new ProductStockModel
				{
					ShopId = StringUtility.ToEmpty(this.ShopId),
					ProductId = StringUtility.ToEmpty(this.ProductId),
					VariationId = StringUtility.ToEmpty(variation.VariationId),
					Stock = ObjectUtility.TryParseInt(this.Stock, 0),
					StockAlert = ObjectUtility.TryParseInt(this.StockAlert, 0),
					UpdateMemo = StringUtility.ToEmpty(this.UpdateMemo),
					LastChanged = StringUtility.ToEmpty(this.LastChanged),
				});
		}
		return stocks.ToArray();
	}

	/// <summary>
	/// Create product option setting list
	/// </summary>
	/// <returns>A array of product option setting list</returns>
	public string[] CreateProductOptionSettingList()
	{
		var result = new List<string>();

		if (string.IsNullOrEmpty(this.ProductOptionSettings))
		{
			result.Add(this.ProductOptionSetting1);
			result.Add(this.ProductOptionSetting2);
			result.Add(this.ProductOptionSetting3);
			result.Add(this.ProductOptionSetting4);
			result.Add(this.ProductOptionSetting5);
		}
		else
		{
			var optionSettings = new ProductOptionSettingList(this.ProductOptionSettings)
				.Items.Select(item => item.GetProductOptionSettingString()).ToArray();
			result.AddRange(optionSettings);
		}

		return result.ToArray();
	}

	/// <summary>
	/// Validate
	/// </summary>
	/// <param name="actionStatus">Action status</param>
	/// <returns>Error messages</returns>
	public Dictionary<string, string> Validate(string actionStatus)
	{
		var input = (Hashtable)this.DataSource.Clone();

		// For case not setting stock managed
		if (this.IsStockUnmanaged)
		{
			input.Remove(Constants.FIELD_PRODUCTSTOCK_STOCK);
			input.Remove(Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT);
			input.Remove(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO);
		}

		// For input product
		var errorMessages = Validator.ValidateAndGetErrorContainer(
			((actionStatus == Constants.ACTION_STATUS_UPDATE) || (actionStatus == Constants.PRODUCT_PROCESS_ACTION_KBN_UPDATE_PREVIEW))
				? "ProductModify"
				: "ProductRegist",
			input);
		if (CheckProductIdIsUsedAsVariationId())
		{
			var errorMessage = MessageManager.GetMessages(
				CommerceMessages.ERRMSG_MANAGER_PRODUCT_ID_IS_USED_AS_VARIATION_ID_ERROR);

			if (errorMessages.ContainsKey(Constants.FIELD_PRODUCT_PRODUCT_ID))
			{
				errorMessages[Constants.FIELD_PRODUCT_PRODUCT_ID] += ("\r\n" + errorMessage);
			}
			else
			{
				errorMessages.Add(Constants.FIELD_PRODUCT_PRODUCT_ID, errorMessage);
			}
		}

		// Check shipping type
		var shippingTypeError = CheckShippingType();
		if (string.IsNullOrEmpty(shippingTypeError) == false)
		{
			if (errorMessages.ContainsKey(Constants.FIELD_PRODUCT_SHIPPING_TYPE))
			{
				errorMessages[Constants.FIELD_PRODUCT_SHIPPING_TYPE] += "<br />" + shippingTypeError;
			}
			else
			{
				errorMessages.Add(Constants.FIELD_PRODUCT_SHIPPING_TYPE, shippingTypeError);
			}
		}

		// Check fixed purchase next shipping setting
		var fixedPurchaseNextShippingErrorMessages = new StringBuilder(CheckSettingFixedPurchaseNextShipping());
		if ((fixedPurchaseNextShippingErrorMessages.Length > 0)
			|| errorMessages.ContainsKey(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID)
			|| errorMessages.ContainsKey(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID)
			|| errorMessages.ContainsKey(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY))
		{
			// Concatenating related errors together
			if (errorMessages.ContainsKey(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID))
			{
				fixedPurchaseNextShippingErrorMessages.AppendFormat(
					"<br />{0}",
					errorMessages[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID]);
				errorMessages.Remove(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID);
			}
			if (errorMessages.ContainsKey(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID))
			{
				fixedPurchaseNextShippingErrorMessages.AppendFormat(
					"<br />{0}",
					errorMessages[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID]);
				errorMessages.Remove(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID);
			}
			if (errorMessages.ContainsKey(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY))
			{
				fixedPurchaseNextShippingErrorMessages.AppendFormat(
					"<br />{0}",
					errorMessages[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY]);
				errorMessages.Remove(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY);
			}

			errorMessages.Add(
				ProductPage.PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SETTING,
				fixedPurchaseNextShippingErrorMessages.ToString());
		}

		// Check product option
		var productOptionError = CheckProductOptionValue();
		if (string.IsNullOrEmpty(productOptionError) == false)
		{
			errorMessages.Add(
				CONST_ERROR_KEY_PRODUCT_OPTIONS,
				productOptionError);
		}

		// For case has setting product price
		if (this.HasProductPrices)
		{
			var productPriceErrors = CheckProductPrice();
			if (productPriceErrors.Count != 0)
			{
				errorMessages.Add(
					Constants.FIELD_PRODUCT_PRODUCTPRICE_EXTEND,
					BasePageHelper.ConvertObjectToJsonString(productPriceErrors));
			}
		}

		// For case has setting product tag
		if (this.HasProductTag)
		{
			var productTagErrors = CheckProductTag();
			if (productTagErrors.Count != 0)
			{
				errorMessages.Add(
					Constants.FIELD_PRODUCT_PRODUCTTAG_EXTEND,
					BasePageHelper.ConvertObjectToJsonString(productTagErrors));
			}
		}

		// For case has setting product tag
		if (this.HasProductExtend)
		{
			var productExtendErrors = CheckProductExtend();
			if (productExtendErrors.Count != 0)
			{
				errorMessages.Add(
					Constants.FIELD_PRODUCT_PRODUCTEXTEND_EXTEND,
					BasePageHelper.ConvertObjectToJsonString(productExtendErrors));
			}
		}

		// For cas has product fixed purchase discount settings
		if (this.HasProductFixedPurchaseDiscountSettings)
		{
			var productFixedPurchaseDiscountSettingError = CheckProductFixedPurchaseDiscountSetting();
			if (string.IsNullOrEmpty(productFixedPurchaseDiscountSettingError) == false)
			{
				errorMessages.Add(
					Constants.FIELD_PRODUCT_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_EXTEND,
					productFixedPurchaseDiscountSettingError);
			}
		}

		// For case has setting product variation
		if (this.HasProductVariation && this.HasProductVariations)
		{
			var productVariationErrors = CheckProductVariation();
			if (productVariationErrors.Count != 0)
			{
				errorMessages.Add(
					Constants.FIELD_PRODUCT_PRODUCTVARIATION_EXTEND,
					BasePageHelper.ConvertObjectToJsonString(productVariationErrors));
			}
		}

		// For case has setting product stock managed
		if ((this.IsStockUnmanaged == false)
			&& (errorMessages.ContainsKey(Constants.FIELD_PRODUCTSTOCK_STOCK)
				|| errorMessages.ContainsKey(Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT)
				|| errorMessages.ContainsKey(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO)))
		{
			// Concatenating related errors together
			var stockError = string.Empty;
			if (errorMessages.ContainsKey(Constants.FIELD_PRODUCTSTOCK_STOCK))
			{
				stockError += "<br />" + errorMessages[Constants.FIELD_PRODUCTSTOCK_STOCK];
				errorMessages.Remove(Constants.FIELD_PRODUCTSTOCK_STOCK);
			}
			if (errorMessages.ContainsKey(Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT))
			{
				stockError += "<br />" + errorMessages[Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT];
				errorMessages.Remove(Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT);
			}
			if (errorMessages.ContainsKey(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO))
			{
				stockError += "<br />" + errorMessages[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO];
				errorMessages.Remove(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO);
			}

			errorMessages.Add(
				CONST_ERROR_KEY_PRODUCT_STOCKS,
				stockError);
		}

		var sellFromDate = this.SellFrom.Split(' ')[0];
		var sellFromTime = this.SellFrom.Split(' ')[1];
		if (CreatedDisplayDate(sellFromDate, sellFromTime) == null)
		{
			errorMessages[Constants.FIELD_PRODUCT_SELL_FROM] =
				MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DATE)
					.Replace(
						CONST_REPLACE_KEY,
						ValueText.GetValueText(
							Constants.TABLE_PRODUCT,
							CONST_PRODUCT_FIELD_NAME,
							Constants.FIELD_PRODUCT_SELL_FROM));
		}

		var displayFromDate = this.DisplayFrom.Split(' ')[0];
		var displayFromTime = this.DisplayFrom.Split(' ')[1];
		if (CreatedDisplayDate(displayFromDate, displayFromTime) == null)
		{
			errorMessages[Constants.FIELD_PRODUCT_DISPLAY_FROM] =
				MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DATE)
					.Replace(
						CONST_REPLACE_KEY,
						ValueText.GetValueText(
							Constants.TABLE_PRODUCT,
							CONST_PRODUCT_FIELD_NAME,
							Constants.FIELD_PRODUCT_DISPLAY_FROM));
		}

		if ((this.IconFlg1 == Constants.FLG_PRODUCT_ICON_ON) && (CheckIconTermEndDate(this.IconTermEnd1) == false))
		{
			errorMessages[Constants.FIELD_PRODUCT_ICON_TERM_END1] =
				MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DATE)
					.Replace(
						CONST_REPLACE_KEY,
						ValueText.GetValueText(
							Constants.TABLE_PRODUCT,
							CONST_PRODUCT_FIELD_NAME,
							Constants.FIELD_PRODUCT_ICON_TERM_END1));
		}

		if ((this.IconFlg2 == Constants.FLG_PRODUCT_ICON_ON) && (CheckIconTermEndDate(this.IconTermEnd2) == false))
		{
			errorMessages[Constants.FIELD_PRODUCT_ICON_TERM_END2] =
				MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DATE)
					.Replace(
						CONST_REPLACE_KEY,
						ValueText.GetValueText(
							Constants.TABLE_PRODUCT,
							CONST_PRODUCT_FIELD_NAME,
							Constants.FIELD_PRODUCT_ICON_TERM_END2));
		}

		if ((this.IconFlg3 == Constants.FLG_PRODUCT_ICON_ON) && (CheckIconTermEndDate(this.IconTermEnd3) == false))
		{
			errorMessages[Constants.FIELD_PRODUCT_ICON_TERM_END3] =
				MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DATE)
					.Replace(
						CONST_REPLACE_KEY,
						ValueText.GetValueText(
							Constants.TABLE_PRODUCT,
							CONST_PRODUCT_FIELD_NAME,
							Constants.FIELD_PRODUCT_ICON_TERM_END3));
		}

		if ((this.IconFlg4 == Constants.FLG_PRODUCT_ICON_ON) && (CheckIconTermEndDate(this.IconTermEnd4) == false))
		{
			errorMessages[Constants.FIELD_PRODUCT_ICON_TERM_END4] =
				MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DATE)
					.Replace(
						CONST_REPLACE_KEY,
						ValueText.GetValueText(
							Constants.TABLE_PRODUCT,
							CONST_PRODUCT_FIELD_NAME,
							Constants.FIELD_PRODUCT_ICON_TERM_END4));
		}

		if ((this.IconFlg5 == Constants.FLG_PRODUCT_ICON_ON) && (CheckIconTermEndDate(this.IconTermEnd5) == false))
		{
			errorMessages[Constants.FIELD_PRODUCT_ICON_TERM_END5] =
				MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DATE)
					.Replace(
						CONST_REPLACE_KEY,
						ValueText.GetValueText(
							Constants.TABLE_PRODUCT,
							CONST_PRODUCT_FIELD_NAME,
							Constants.FIELD_PRODUCT_ICON_TERM_END5));
		}

		if ((this.IconFlg6 == Constants.FLG_PRODUCT_ICON_ON) && (CheckIconTermEndDate(this.IconTermEnd6) == false))
		{
			errorMessages[Constants.FIELD_PRODUCT_ICON_TERM_END6] =
				MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DATE)
					.Replace(
						CONST_REPLACE_KEY,
						ValueText.GetValueText(
							Constants.TABLE_PRODUCT,
							CONST_PRODUCT_FIELD_NAME,
							Constants.FIELD_PRODUCT_ICON_TERM_END6));
		}

		if ((this.IconFlg7 == Constants.FLG_PRODUCT_ICON_ON) && (CheckIconTermEndDate(this.IconTermEnd7) == false))
		{
			errorMessages[Constants.FIELD_PRODUCT_ICON_TERM_END7] =
				MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DATE)
					.Replace(
						CONST_REPLACE_KEY,
						ValueText.GetValueText(
							Constants.TABLE_PRODUCT,
							CONST_PRODUCT_FIELD_NAME,
							Constants.FIELD_PRODUCT_ICON_TERM_END7));
		}

		if ((this.IconFlg8 == Constants.FLG_PRODUCT_ICON_ON) && (CheckIconTermEndDate(this.IconTermEnd8) == false))
		{
			errorMessages[Constants.FIELD_PRODUCT_ICON_TERM_END8] =
				MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DATE)
					.Replace(
						CONST_REPLACE_KEY,
						ValueText.GetValueText(
							Constants.TABLE_PRODUCT,
							CONST_PRODUCT_FIELD_NAME,
							Constants.FIELD_PRODUCT_ICON_TERM_END8));
		}

		if ((this.IconFlg9 == Constants.FLG_PRODUCT_ICON_ON) && (CheckIconTermEndDate(this.IconTermEnd9) == false))
		{
			errorMessages[Constants.FIELD_PRODUCT_ICON_TERM_END9] =
				MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DATE)
					.Replace(
						CONST_REPLACE_KEY,
						ValueText.GetValueText(
							Constants.TABLE_PRODUCT,
							CONST_PRODUCT_FIELD_NAME,
							Constants.FIELD_PRODUCT_ICON_TERM_END9));
		}

		if ((this.IconFlg10 == Constants.FLG_PRODUCT_ICON_ON) && (CheckIconTermEndDate(this.IconTermEnd10) == false))
		{
			errorMessages[Constants.FIELD_PRODUCT_ICON_TERM_END10] =
				MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DATE)
					.Replace(
						CONST_REPLACE_KEY,
						ValueText.GetValueText(
							Constants.TABLE_PRODUCT,
							CONST_PRODUCT_FIELD_NAME,
							Constants.FIELD_PRODUCT_ICON_TERM_END10));
		}

		return errorMessages;
	}

	/// <summary>
	/// Check product price
	/// </summary>
	/// <returns>Error messages</returns>
	private Dictionary<string, string> CheckProductPrice()
	{
		var errorMessages = new Dictionary<string, string>();
		foreach (var item in this.ProductPrices)
		{
			var errorMessage = item.Validate();
			if (string.IsNullOrEmpty(errorMessage)) continue;

			errorMessages.Add(item.MemberRankId, errorMessage);
		}
		return errorMessages;
	}

	/// <summary>
	/// Check product variation
	/// </summary>
	/// <returns>Error messages</returns>
	private Dictionary<string, string> CheckProductVariation()
	{
		// バリエーションID重複チェック
		var errorMessages = new Dictionary<string, string>();

		if (CheckDuplicationVariationId())
		{
			errorMessages.Add(
				CONST_ERROR_KEY_PRODUCT_VARIATION_DUPLICATION_ID,
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCTVARIATION_VARIATION_ID_DUPLICATION_ERROR));
		}

		// 商品ID＋バリエーションIDが既に「商品ID」として使用されるかチェック
		if (CheckVariationIdIsUsedAsProductId())
		{
			var errorMessage = WebMessages.GetMessages(
				WebMessages.ERRMSG_MANAGER_PRODUCTVARIATION_VARIATION_ID_IS_USED_AS_PRODUCT_ID_ERROR);
			errorMessages[CONST_ERROR_KEY_PRODUCT_VARIATION_DUPLICATION_ID] = errorMessage;
		}

		// 入力チェック＆重複チェック
		for (var index = 0; index < this.ProductVariations.Length; index++)
		{
			var variationErrors = this.ProductVariations[index].Validate(index);
			if (variationErrors.Count != 0)
			{
				errorMessages.Add(
					Constants.FIELD_PRODUCT_PRODUCTVARIATION_EXTEND + index,
					BasePageHelper.ConvertObjectToJsonString(variationErrors));
			}
		}
		return errorMessages;
	}

	/// <summary>
	/// バリエーションID重複チェック
	/// </summary>
	/// <returns>重複している：true　重複していない：false</returns>
	private bool CheckDuplicationVariationId()
	{
		var itemGroupCount = this.ProductVariations
			.GroupBy(item => item.VariationId)
			.Count(group => (group.Count() > 1));
		return (itemGroupCount > 0);
	}

	/// <summary>
	/// 商品IDが他商品の「商品ID + バリエーションID」と重複しているか
	/// </summary>
	/// <returns>重複しているか</returns>
	private bool CheckProductIdIsUsedAsVariationId()
	{
		var productService = new ProductService();
		var isUsed = productService.CheckProductIdIsUsedAsVariationId(this.ShopId, this.ProductId);

		return isUsed;
	}

	/// <summary>
	/// 商品ID＋バリエーションIDが既に「商品ID」として使用されるかチェック
	/// </summary>
	/// <returns>使用されるか</returns>
	private bool CheckVariationIdIsUsedAsProductId()
	{
		var productService = new ProductService();
		var isUsed = this.ProductVariations
			.Any(variation => productService.CheckVariationIdIsUsedAsProductId(variation.ShopId, variation.VariationId));

		return isUsed;
	}

	/// <summary>
	/// Check product tag
	/// </summary>
	/// <returns>Error messages as dictionary</returns>
	private Dictionary<string, string> CheckProductTag()
	{
		var errorMessages = this.ProductTag.Validate();

		return errorMessages;
	}

	/// <summary>
	/// Check product extend
	/// </summary>
	/// <returns>Error messages as dictionary</returns>
	private Dictionary<string, string> CheckProductExtend()
	{
		var errorMessages = this.ProductExtend.Validate();

		return errorMessages;
	}

	/// <summary>
	/// Check product option value
	/// </summary>
	/// <returns>Error message</returns>
	private string CheckProductOptionValue()
	{
		if (string.IsNullOrEmpty(this.ProductOptionSettings)) return string.Empty;
		if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED)
		{
			var errorSpecialCharacter = ValidateInvalidCharsForProductOpusionSettings(this.ProductOptionSettings);
			if (string.IsNullOrEmpty(errorSpecialCharacter) == false) return errorSpecialCharacter;
		}

		var productOptionSettingList = new ProductOptionSettingList(
			this.ProductOptionSettings);
		var errorMessage = new ProductPage().CheckProductOptionValue(productOptionSettingList);
		return errorMessage;
	}

	/// <summary>
	/// 商品付帯情報設定文字列で利用不可な文字が含まれているかチェック
	/// </summary>
	/// <param name="productOptionSettings">商品付帯情報設定文字列</param>
	/// <returns>エラーメッセージ</returns>
	private string ValidateInvalidCharsForProductOpusionSettings(string productOptionSettings)
	{
		var prohibitedCharacters = new[] { "(", ")", "-", "_" };
		var message = new StringBuilder();

		// 引数に「[[T@@イニシャル@@Necessary=0@@Type=HALFWIDTH_ALPHNUMSYMBOL@@Length=3]]」が渡された場合など、
		// 「HALFWIDTH_ALPHNUMSYMBOL」に「_」が含まれてしまうため、チェック時は除去
		var replacedOptionSetting = productOptionSettings
			.Replace(Validator.STRTYPE_FULLWIDTH_HIRAGANA, string.Empty)
			.Replace(Validator.STRTYPE_FULLWIDTH_KATAKANA, string.Empty)
			.Replace(Validator.STRTYPE_DATE_PAST, string.Empty)
			.Replace(Validator.STRTYPE_DATE_FUTURE, string.Empty)
			.Replace(Validator.STRTYPE_HALFWIDTH_ALPHNUM, string.Empty)
			.Replace(Validator.STRTYPE_HALFWIDTH_ALPHNUMSYMBOL, string.Empty)
			.Replace(Validator.STRTYPE_HALFWIDTH_NUMBER, string.Empty);
		foreach (var character in prohibitedCharacters)
		{
			if (replacedOptionSetting.Contains(character))
			{
				message.Append(MessageManager.GetMessages(MessageManager.INPUTCHECK_PROHIBITED_CHARACTERS)
					.Replace("@@ 1 @@", character))
					.Append("<br />");
			}
		}

		// 上記では@が禁止文字として検出出来ないので、下記正規表現で検証する
		// @@が4つ並んでいる場合は、未入力の可能性があるため、ここでは検知しない
		var pattern = new Regex(Constants.REGEX_PATTERN_PRODUCT_OPTION_BASIC);
		var optionMatches = pattern.Matches(productOptionSettings);
		foreach(var option in optionMatches)
		{
			if ((Regex.IsMatch(((Match)option).Value, Constants.REGEX_PATTERN_PRODUCT_OPTION_INCLUDE_ATMARK) == false)
				 && (((Match)option).Value.Contains("@@@@") == false))
			{
				message.Append(MessageManager.GetMessages(MessageManager.INPUTCHECK_PROHIBITED_CHARACTERS)
					.Replace("@@ 1 @@", "@"))
					.Append("<br />");
			}
		}

		return message.ToString();
	}


	/// <summary>
	/// Check product fixed purchase discount setting
	/// </summary>
	/// <returns>Error message</returns>
	private string CheckProductFixedPurchaseDiscountSetting()
	{
		var errorMessage = new ProductPage().CheckProductFixedPurchaseDiscountSetting(
			this.ProductFixedPurchaseDiscountSettings.ToList());
		return errorMessage;
	}

	/// <summary>
	/// Check shipping type
	/// </summary>
	/// <returns>Error message</returns>
	private string CheckShippingType()
	{
		var errorMessage = new ProductPage().CheckShippingType(
			this.ShopId,
			this.ShippingType,
			this.FixedPurchaseFlg);
		return errorMessage;
	}

	/// <summary>
	/// Check setting fixed purchase next shipping
	/// </summary>
	/// <returns>Error message</returns>
	private string CheckSettingFixedPurchaseNextShipping()
	{
		var errorMessage = new ProductPage().CheckInputProductForFixedPurchaseNextShippingSetting(this.DataSource);
		return errorMessage;
	}

	/// <summary>
	/// Create display date
	/// </summary>
	/// <param name="date">Date</param>
	/// <param name="time">Time</param>
	/// <returns>An date</returns>
	protected DateTime? CreatedDisplayDate(string date, string time)
	{
		if (string.IsNullOrEmpty(date)) return null;
		DateTime dateTime;
		if (string.IsNullOrEmpty(time) == false) date = string.Format("{0} {1}", date, time);

		if (DateTime.TryParse(date, out dateTime) == false) return null;

		return dateTime;
	}

	/// <summary>
	/// Check icon term end date
	/// </summary>
	/// <param name="iconTermEndDate">Icon term end date</param>
	/// <returns>True: Is date, otherwise: false</returns>
	protected bool CheckIconTermEndDate(string iconTermEndDate)
	{
		var date = (iconTermEndDate.Contains('/')) ? iconTermEndDate.Split(' ')[0] : string.Empty;
		var time = (iconTermEndDate.Contains(' ')) ? iconTermEndDate.Split(' ')[1] : string.Empty;

		return (CreatedDisplayDate(date, time) != null);
	}

	/// <summary>
	/// Adjust image heads
	/// </summary>
	/// <remarks>Use for the oldest registered products</remarks>
	public void AdjustImageHeads()
	{
		if (this.ImageHead != this.ProductId)
		{
			this.ImageHead = this.ProductId;
		}

		if (this.HasProductVariations)
		{
			foreach (var variation in this.ProductVariations)
			{
				if (variation.VariationId == variation.VariationImageHead) continue;

				variation.VariationImageHead = string.Format(
					"{0}{1}{2}",
					variation.ProductId,
					Constants.PRODUCTVARIATIONIMAGE_FOOTER,
					variation.VId);
			}
		}
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_SHOP_ID)]
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_SHOP_ID] = value; }
	}
	/// <summary>サプライヤID</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_SUPPLIER_ID)]
	public string SupplierId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SUPPLIER_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_SUPPLIER_ID] = value; }
	}
	/// <summary>商品ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRODUCT_ID)]
	public string ProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_ID] = value; }
	}
	/// <summary>商品連携ID1</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_COOPERATION_ID1)]
	public string CooperationId1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID1]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID1] = value; }
	}
	/// <summary>商品連携ID2</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_COOPERATION_ID2)]
	public string CooperationId2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID2]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID2] = value; }
	}
	/// <summary>商品連携ID3</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_COOPERATION_ID3)]
	public string CooperationId3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID3]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID3] = value; }
	}
	/// <summary>商品連携ID4</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_COOPERATION_ID4)]
	public string CooperationId4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID4]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID4] = value; }
	}
	/// <summary>商品連携ID5</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_COOPERATION_ID5)]
	public string CooperationId5
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID5]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID5] = value; }
	}
	/// <summary>モール拡張商品ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID)]
	public string MallExProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_MALL_EX_PRODUCT_ID] = value; }
	}
	/// <summary>メーカーID</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_MAKER_ID)]
	public string MakerId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MAKER_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_MAKER_ID] = value; }
	}
	/// <summary>メーカー名</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_MAKER_NAME)]
	public string MakerName
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MAKER_NAME]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_MAKER_NAME] = value; }
	}
	/// <summary>商品名</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_NAME)]
	public string Name
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_NAME] = value; }
	}
	/// <summary>商品名かな</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_NAME_KANA)]
	public string NameKana
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME_KANA]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_NAME_KANA] = value; }
	}
	/// <summary>商品名2</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_NAME2)]
	public string Name2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME2]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_NAME2] = value; }
	}
	/// <summary>商品名2かな</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_NAME2_KANA)]
	public string Name2Kana
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NAME2_KANA]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_NAME2_KANA] = value; }
	}
	/// <summary>SEOキーワード</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_SEO_KEYWORDS)]
	public string SeoKeywords
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SEO_KEYWORDS]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_SEO_KEYWORDS] = value; }
	}
	/// <summary>キャッチコピー</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_CATCHCOPY)]
	public string Catchcopy
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATCHCOPY]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_CATCHCOPY] = value; }
	}
	/// <summary>モバイルキャッチコピー</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_CATCHCOPY_MOBILE)]
	public string CatchcopyMobile
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATCHCOPY_MOBILE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_CATCHCOPY_MOBILE] = value; }
	}
	/// <summary>商品概要HTML区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_OUTLINE_KBN)]
	public string OutlineKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_KBN] = value; }
	}
	/// <summary>商品概要</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_OUTLINE)]
	public string Outline
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_OUTLINE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_OUTLINE] = value; }
	}
	/// <summary>モバイル商品概要HTML区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_OUTLINE_KBN_MOBILE)]
	public string OutlineKbnMobile
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_KBN_MOBILE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_KBN_MOBILE] = value; }
	}
	/// <summary>モバイル商品概要</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_OUTLINE_MOBILE)]
	public string OutlineMobile
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_MOBILE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_OUTLINE_MOBILE] = value; }
	}
	/// <summary>商品詳細１HTML区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1)]
	public string DescDetailKbn1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1] = value; }
	}
	/// <summary>商品詳細１</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DESC_DETAIL1)]
	public string DescDetail1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL1]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL1] = value; }
	}
	/// <summary>商品詳細2HTML区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2)]
	public string DescDetailKbn2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2] = value; }
	}
	/// <summary>商品詳細2</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DESC_DETAIL2)]
	public string DescDetail2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL2]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL2] = value; }
	}
	/// <summary>商品詳細3HTML区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3)]
	public string DescDetailKbn3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN3] = value; }
	}
	/// <summary>商品詳細3</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DESC_DETAIL3)]
	public string DescDetail3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL3]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL3] = value; }
	}
	/// <summary>商品詳細4HTML区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4)]
	public string DescDetailKbn4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN4] = value; }
	}
	/// <summary>商品詳細4</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DESC_DETAIL4)]
	public string DescDetail4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL4]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL4] = value; }
	}
	/// <summary>返品交換文言</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE)]
	public string ReturnExchangeMessage
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE] = value; }
	}
	/// <summary>モバイル商品詳細１HTML区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1_MOBILE)]
	public string DescDetailKbn1Mobile
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1_MOBILE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN1_MOBILE] = value; }
	}
	/// <summary>モバイル商品詳細１</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DESC_DETAIL1_MOBILE)]
	public string DescDetail1Mobile
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL1_MOBILE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL1_MOBILE] = value; }
	}
	/// <summary>モバイル商品詳細2HTML区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2_MOBILE)]
	public string DescDetailKbn2Mobile
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2_MOBILE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL_KBN2_MOBILE] = value; }
	}
	/// <summary>モバイル商品詳細2</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DESC_DETAIL2_MOBILE)]
	public string DescDetail2Mobile
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL2_MOBILE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL2_MOBILE] = value; }
	}
	/// <summary>モバイル返品交換文言</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE_MOBILE)]
	public string ReturnExchangeMessageMobile
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE_MOBILE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE_MOBILE] = value; }
	}
	/// <summary>備考</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_NOTE)]
	public string Note
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NOTE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_NOTE] = value; }
	}
	/// <summary>商品表示価格</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DISPLAY_PRICE)]
	public string DisplayPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE] = value; }
	}
	/// <summary>商品表示特別価格</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE)]
	public string DisplaySpecialPrice
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] = value; }
	}
	/// <summary>税込フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG)]
	public string TaxIncludedFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_TAX_INCLUDED_FLG] = value; }
	}
	/// <summary>税率</summary>
	/// <remarks>使用しない</remarks>  
	[JsonProperty(Constants.FIELD_PRODUCT_TAX_RATE)]
	public string TaxRate
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_TAX_RATE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_TAX_RATE] = value; }
	}
	/// <summary>税計算方法</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_TAX_ROUND_TYPE)]
	public string TaxRoundType
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_TAX_ROUND_TYPE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_TAX_ROUND_TYPE] = value; }
	}
	/// <summary>税込販売価格</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRICE_PRETAX)]
	public string PricePretax
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRICE_PRETAX]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRICE_PRETAX] = value; }
	}
	/// <summary>配送料</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRICE_SHIPPING)]
	public string PriceShipping
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRICE_SHIPPING]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRICE_SHIPPING] = value; }
	}
	/// <summary>配送料種別</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_SHIPPING_TYPE)]
	public string ShippingType
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_TYPE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_TYPE] = value; }
	}
	/// <summary>配送サイズ区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN)]
	public string ShippingSizeKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN] = value; }
	}
	/// <summary>商品原価</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRICE_COST)]
	public string PriceCost
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRICE_COST]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRICE_COST] = value; }
	}
	/// <summary>付与ポイント区分１</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_POINT_KBN1)]
	public string PointKbn1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN1]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN1] = value; }
	}
	/// <summary>付与ポイント１</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_POINT1)]
	public string Point1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_POINT1]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_POINT1] = value; }
	}
	/// <summary>付与ポイント区分２</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_POINT_KBN2)]
	public string PointKbn2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN2]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN2] = value; }
	}
	/// <summary>付与ポイント２</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_POINT2)]
	public string Point2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_POINT2]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_POINT2] = value; }
	}
	/// <summary>付与ポイント区分３</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_POINT_KBN3)]
	public string PointKbn3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN3]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_POINT_KBN3] = value; }
	}
	/// <summary>付与ポイント３</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_POINT3)]
	public string Point3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_POINT3]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_POINT3] = value; }
	}
	/// <summary>キャンペーン期間(FROM)</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_CAMPAIGN_FROM)]
	public string CampaignFrom
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_FROM]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_FROM] = value; }
	}
	/// <summary>キャンペーン期間(TO)</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_CAMPAIGN_TO)]
	public string CampaignTo
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_TO]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_TO] = value; }
	}
	/// <summary>キャンペーン付与ポイント区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_CAMPAIGN_POINT_KBN)]
	public string CampaignPointKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_POINT_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_POINT_KBN] = value; }
	}
	/// <summary>キャンペーン付与ポイント</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_CAMPAIGN_POINT)]
	public string CampaignPoint
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_POINT]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_CAMPAIGN_POINT] = value; }
	}
	/// <summary>表示期間(FROM)</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DISPLAY_FROM)]
	public string DisplayFrom
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_FROM]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_FROM] = value; }
	}
	/// <summary>表示期間(TO)</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DISPLAY_TO)]
	public string DisplayTo
	{
		get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_TO]); }
		set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_TO] = value; }
	}
	/// <summary>販売期間(FROM)</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_SELL_FROM)]
	public string SellFrom
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SELL_FROM]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_SELL_FROM] = value; }
	}
	/// <summary>販売期間(TO)</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_SELL_TO)]
	public string SellTo
	{
		get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCT_SELL_TO]); }
		set { this.DataSource[Constants.FIELD_PRODUCT_SELL_TO] = value; }
	}
	/// <summary>販売期間前表示フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_BEFORE_SALE_DISPLAY_KBN)]
	public string BeforeSaleDisplayKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BEFORE_SALE_DISPLAY_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_BEFORE_SALE_DISPLAY_KBN] = value; }
	}
	/// <summary>販売期間後表示フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_AFTER_SALE_DISPLAY_KBN)]
	public string AfterSaleDisplayKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_AFTER_SALE_DISPLAY_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_AFTER_SALE_DISPLAY_KBN] = value; }
	}
	/// <summary>販売可能数</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY)]
	public string MaxSellQuantity
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY] = value; }
	}
	/// <summary>在庫管理方法</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN)]
	public string StockManagementKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] = value; }
	}
	/// <summary>在庫表示区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_STOCK_DISP_KBN)]
	public string StockDispKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_STOCK_DISP_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_STOCK_DISP_KBN] = value; }
	}
	/// <summary>商品在庫文言ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID)]
	public string StockMessageId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_STOCK_MESSAGE_ID] = value; }
	}
	/// <summary>紹介URL</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_URL)]
	public string Url
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_URL]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_URL] = value; }
	}
	/// <summary>問い合わせ用メールアドレス</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_INQUIRE_EMAIL)]
	public string InquireEmail
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_INQUIRE_EMAIL]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_INQUIRE_EMAIL] = value; }
	}
	/// <summary>問い合わせ用電話番号</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_INQUIRE_TEL)]
	public string InquireTel
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_INQUIRE_TEL]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_INQUIRE_TEL] = value; }
	}
	/// <summary>検索時表示区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DISPLAY_KBN)]
	public string DisplayKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_KBN] = value; }
	}
	/// <summary>カテゴリID1</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_CATEGORY_ID1)]
	public string CategoryId1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID1]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID1] = value; }
	}
	/// <summary>カテゴリID2</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_CATEGORY_ID2)]
	public string CategoryId2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID2]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID2] = value; }
	}
	/// <summary>カテゴリID3</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_CATEGORY_ID3)]
	public string CategoryId3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID3]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID3] = value; }
	}
	/// <summary>カテゴリID4</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_CATEGORY_ID4)]
	public string CategoryId4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID4]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID4] = value; }
	}
	/// <summary>カテゴリID5</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_CATEGORY_ID5)]
	public string CategoryId5
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID5]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID5] = value; }
	}
	/// <summary>関連商品ID1（クロスセル）</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1)]
	public string RelatedProductIdCs1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS1] = value; }
	}
	/// <summary>関連商品ID2（クロスセル）</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2)]
	public string RelatedProductIdCs2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS2] = value; }
	}
	/// <summary>関連商品ID3（クロスセル）</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3)]
	public string RelatedProductIdCs3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS3] = value; }
	}
	/// <summary>関連商品ID4（クロスセル）</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4)]
	public string RelatedProductIdCs4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS4] = value; }
	}
	/// <summary>関連商品ID5（クロスセル）</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5)]
	public string RelatedProductIdCs5
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_CS5] = value; }
	}
	/// <summary>関連商品ID1（アップセル）</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1)]
	public string RelatedProductIdUs1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US1] = value; }
	}
	/// <summary>関連商品ID2（アップセル）</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US2)]
	public string RelatedProductIdUs2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US2]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US2] = value; }
	}
	/// <summary>関連商品ID3（アップセル）</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US3)]
	public string RelatedProductIdUs3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US3]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US3] = value; }
	}
	/// <summary>関連商品ID4（アップセル）</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US4)]
	public string RelatedProductIdUs4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US4]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US4] = value; }
	}
	/// <summary>関連商品ID5（アップセル）</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US5)]
	public string RelatedProductIdUs5
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US5]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RELATED_PRODUCT_ID_US5] = value; }
	}
	/// <summary>商品画像名ヘッダ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_IMAGE_HEAD)]
	public string ImageHead
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_IMAGE_HEAD]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_IMAGE_HEAD] = value; }
	}
	/// <summary>モバイル商品画像名</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_IMAGE_MOBILE)]
	public string ImageMobile
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_IMAGE_MOBILE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_IMAGE_MOBILE] = value; }
	}
	/// <summary>アイコンフラグ1</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_FLG1)]
	public string IconFlg1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG1]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG1] = value; }
	}
	/// <summary>アイコン有効期限1</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_TERM_END1)]
	public string IconTermEnd1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END1]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END1] = value; }
	}
	/// <summary>アイコンフラグ2</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_FLG2)]
	public string IconFlg2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG2]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG2] = value; }
	}
	/// <summary>アイコン有効期限2</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_TERM_END2)]
	public string IconTermEnd2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END2]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END2] = value; }
	}
	/// <summary>アイコンフラグ3</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_FLG3)]
	public string IconFlg3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG3]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG3] = value; }
	}
	/// <summary>アイコン有効期限3</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_TERM_END3)]
	public string IconTermEnd3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END3]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END3] = value; }
	}
	/// <summary>アイコンフラグ4</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_FLG4)]
	public string IconFlg4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG4]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG4] = value; }
	}
	/// <summary>アイコン有効期限4</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_TERM_END4)]
	public string IconTermEnd4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END4]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END4] = value; }
	}
	/// <summary>アイコンフラグ5</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_FLG5)]
	public string IconFlg5
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG5]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG5] = value; }
	}
	/// <summary>アイコン有効期限5</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_TERM_END5)]
	public string IconTermEnd5
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END5]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END5] = value; }
	}
	/// <summary>アイコンフラグ6</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_FLG6)]
	public string IconFlg6
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG6]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG6] = value; }
	}
	/// <summary>アイコン有効期限6</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_TERM_END6)]
	public string IconTermEnd6
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END6]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END6] = value; }
	}
	/// <summary>アイコンフラグ7</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_FLG7)]
	public string IconFlg7
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG7]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG7] = value; }
	}
	/// <summary>アイコン有効期限7</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_TERM_END7)]
	public string IconTermEnd7
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END7]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END7] = value; }
	}
	/// <summary>アイコンフラグ8</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_FLG8)]
	public string IconFlg8
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG8]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG8] = value; }
	}
	/// <summary>アイコン有効期限8</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_TERM_END8)]
	public string IconTermEnd8
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END8]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END8] = value; }
	}
	/// <summary>アイコンフラグ9</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_FLG9)]
	public string IconFlg9
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG9]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG9] = value; }
	}
	/// <summary>アイコン有効期限9</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_TERM_END9)]
	public string IconTermEnd9
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END9]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END9] = value; }
	}
	/// <summary>アイコンフラグ10</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_FLG10)]
	public string IconFlg10
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG10]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_FLG10] = value; }
	}
	/// <summary>アイコン有効期限10</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ICON_TERM_END10)]
	public string IconTermEnd10
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END10]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ICON_TERM_END10] = value; }
	}
	/// <summary>モバイル表示フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_MOBILE_DISP_FLG)]
	public string MobileDispFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MOBILE_DISP_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_MOBILE_DISP_FLG] = value; }
	}
	/// <summary>複数バリエーション使用フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_USE_VARIATION_FLG)]
	public string UseVariationFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_USE_VARIATION_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_USE_VARIATION_FLG] = value; }
	}
	/// <summary>予約商品フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RESERVATION_FLG)]
	public string ReservationFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RESERVATION_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RESERVATION_FLG] = value; }
	}
	/// <summary>定期購入フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG)]
	public string FixedPurchaseFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] = value; }
	}
	/// <summary>定期商品購入制限チェックフラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG)]
	public string CheckFixedProductOrderFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG] = value; }
	}
	/// <summary>通常商品購入制限チェックフラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG)]
	public string CheckProductOrderFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG] = value; }
	}
	/// <summary>モール連携済フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_MALL_COOPERATED_FLG)]
	public string MallCooperatedFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MALL_COOPERATED_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_MALL_COOPERATED_FLG] = value; }
	}
	/// <summary>有効フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_VALID_FLG)]
	public string ValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_VALID_FLG] = value; }
	}
	/// <summary>削除フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DEL_FLG)]
	public string DelFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DEL_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DEL_FLG] = value; }
	}
	/// <summary>検索キーワード</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_SEARCH_KEYWORD)]
	public string SearchKeyword
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SEARCH_KEYWORD]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_SEARCH_KEYWORD] = value; }
	}
	/// <summary>会員ランク割引対象フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG)]
	public string MemberRankDiscountFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG] = value; }
	}
	/// <summary>閲覧可能会員ランク</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK)]
	public string DisplayMemberRank
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_MEMBER_RANK] = value; }
	}
	/// <summary>購入可能会員ランク</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK)]
	public string BuyableMemberRank
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK] = value; }
	}
	/// <summary>Googleショッピング連携フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG)]
	public string GoogleShoppingFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_GOOGLE_SHOPPING_FLG] = value; }
	}
	/// <summary>商品付帯情報設定</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS)]
	public string ProductOptionSettings
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_OPTION_SETTINGS] = value; }
	}
	/// <summary>再入荷通知メール有効フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG)]
	public string ArrivalMailValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ARRIVAL_MAIL_VALID_FLG] = value; }
	}
	/// <summary>販売開始通知メール有効フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG)]
	public string ReleaseMailValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RELEASE_MAIL_VALID_FLG] = value; }
	}
	/// <summary>再販売通知メール有効フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG)]
	public string ResaleMailValidFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RESALE_MAIL_VALID_FLG] = value; }
	}
	/// <summary>PC用商品詳細バリエーション選択方法</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN)]
	public string SelectVariationKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN] = value; }
	}
	/// <summary>モバイル用商品バリエーション選択方法</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN_MOBILE)]
	public string SelectVariationKbnMobile
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN_MOBILE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_SELECT_VARIATION_KBN_MOBILE] = value; }
	}
	/// <summary>ブランドID1</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_BRAND_ID1)]
	public string BrandId1
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID1]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID1] = value; }
	}
	/// <summary>ブランドID2</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_BRAND_ID2)]
	public string BrandId2
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID2]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID2] = value; }
	}
	/// <summary>ブランドID3</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_BRAND_ID3)]
	public string BrandId3
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID3]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID3] = value; }
	}
	/// <summary>ブランドID4</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_BRAND_ID4)]
	public string BrandId4
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID4]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID4] = value; }
	}
	/// <summary>ブランドID5</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_BRAND_ID5)]
	public string BrandId5
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID5]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID5] = value; }
	}
	/// <summary>外部レコメンド利用フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG)]
	public string UseRecommendFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_USE_RECOMMEND_FLG] = value; }
	}
	/// <summary>ギフト購入フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_GIFT_FLG)]
	public string GiftFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_GIFT_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_GIFT_FLG] = value; }
	}
	/// <summary>年齢制限フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_AGE_LIMIT_FLG)]
	public string AgeLimitFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_AGE_LIMIT_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_AGE_LIMIT_FLG] = value; }
	}
	/// <summary>配送料金複数個無料フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG)]
	public string PluralShippingPriceFreeFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG] = value; }
	}
	/// <summary>デジタルコンテンツ商品フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG)]
	public string DigitalContentsFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG] = value; }
	}
	/// <summary>ダウンロードURL</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DOWNLOAD_URL)]
	public string DownloadUrl
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DOWNLOAD_URL]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DOWNLOAD_URL] = value; }
	}
	/// <summary>販売期間表示フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG)]
	public string DisplaySellFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG] = value; }
	}
	/// <summary>表示優先順</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DISPLAY_PRIORITY)]
	public string DisplayPriority
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRIORITY]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRIORITY] = value; }
	}
	/// <summary>決済利用不可</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS)]
	public string LimitedPaymentIds
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS] = value; }
	}
	/// <summary>定期初回購入価格</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE)]
	public string FixedPurchaseFirsttimePrice
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE] = value; }
	}
	/// <summary>定期購入価格</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE)]
	public string FixedPurchasePrice
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE] = value; }
	}
	/// <summary>同梱商品明細表示フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE)]
	public string BundleItemDisplayType
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE] = value; }
	}
	/// <summary>商品区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRODUCT_TYPE)]
	public string ProductType
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_TYPE]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_TYPE] = value; }
	}
	/// <summary>利用不可配送周期月</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING)]
	public string LimitedFixedPurchaseKbn1Setting
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING] = value; }
	}
	/// <summary>利用不可配送周期日</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING)]
	public string LimitedFixedPurchaseKbn3Setting
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING] = value; }
	}
	/// <summary>利用不可配送周期週</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING)]
	public string LimitedFixedPurchaseKbn4Setting
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING] = value; }
	}
	/// <summary>商品連携ID6</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_COOPERATION_ID6)]
	public string CooperationId6
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID6]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID6] = value; }
	}
	/// <summary>商品連携ID7</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_COOPERATION_ID7)]
	public string CooperationId7
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID7]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID7] = value; }
	}
	/// <summary>商品連携ID8</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_COOPERATION_ID8)]
	public string CooperationId8
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID8]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID8] = value; }
	}
	/// <summary>商品連携ID9</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_COOPERATION_ID9)]
	public string CooperationId9
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID9]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID9] = value; }
	}
	/// <summary>商品連携ID10</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_COOPERATION_ID10)]
	public string CooperationId10
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID10]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_COOPERATION_ID10] = value; }
	}
	/// <summary>＆mallの予約商品フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG)]
	public string AndmallReservationFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ANDMALL_RESERVATION_FLG] = value; }
	}
	/// <summary>定期会員限定フラグ（閲覧）</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG)]
	public string DisplayOnlyFixedPurchaseMemberFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_ONLY_FIXED_PURCHASE_MEMBER_FLG] = value; }
	}
	/// <summary>定期会員限定フラグ（購入）</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG)]
	public string SellOnlyFixedPurchaseMemberFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_SELL_ONLY_FIXED_PURCHASE_MEMBER_FLG] = value; }
	}
	/// <summary>商品重量(g）</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM)]
	public string ProductWeightGram
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_WEIGHT_GRAM] = value; }
	}
	/// <summary>税率カテゴリID</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_TAX_CATEGORY_ID)]
	public string TaxCategoryId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_TAX_CATEGORY_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_TAX_CATEGORY_ID] = value; }
	}
	/// <summary>定期解約可能回数</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT)]
	public string FixedPurchasedCancelableCount
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT] = value; }
	}
	/// <summary>定期購入利用不可ユーザー管理レベル</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS)]
	public string FixedPurchasedLimitedUserLevelIds
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS] = value; }
	}
	/// <summary>定期購入2回目以降配送商品ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID)]
	public string FixedPurchaseNextShippingProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID] = value; }
	}
	/// <summary>定期購入2回目以降配送商品バリエーションID</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID)]
	public string FixedPurchaseNextShippingVariationId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID] = value; }
	}
	/// <summary>定期購入2回目以降配送商品注文個数</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY)]
	public string FixedPurchaseNextShippingItemQuantity
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY] = value; }
	}
	/// <summary>定期購入スキップ制限回数</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT)]
	public string FixedPurchaseLimitedSkippedCount
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_SKIPPED_COUNT] = value; }
	}
	/// <summary>定期購入2回目以降配送商品 定期購入区分</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN)]
	public string NextShippingItemFixedPurchaseKbn
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN] = value; }
	}
	/// <summary>定期購入2回目以降配送商品 定期購入設定</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING)]
	public string NextShippingItemFixedPurchaseSetting
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING] = value; }
	}
	/// <summary>レコメンド用商品ID</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID)]
	public string RecommendProductId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID] = value; }
	}
	/// <summary>商品カラーID</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID)]
	public string ProductColorId
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_COLOR_ID] = value; }
	}
	/// <summary>頒布会購入フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG)]
	public string SubscriptionBoxFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG] = value; }
	}
	/// <summary>商品在庫数</summary>
	[JsonProperty(Constants.FIELD_PRODUCTSTOCK_STOCK)]
	public string Stock
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK] = value; }
	}
	/// <summary>商品在庫安全基準</summary>
	[JsonProperty(Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT)]
	public string StockAlert
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT] = value; }
	}
	/// <summary>在庫更新メモ</summary>
	[JsonProperty(Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO)]
	public string UpdateMemo
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO]; }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO] = value; }
	}
	/// <summary>最終更新者</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_LAST_CHANGED)]
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_LAST_CHANGED] = value; }
	}
	/// <summary>商品サイズ係数</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR)]
	public string ProductSizeFactor
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR] = value; }
	}
	/// <summary>Product prices</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRODUCTPRICE_EXTEND)]
	public ProductPriceInput[] ProductPrices
	{
		get { return (ProductPriceInput[])this.DataSource[Constants.FIELD_PRODUCT_PRODUCTPRICE_EXTEND]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCTPRICE_EXTEND] = value; }
	}
	/// <summary>Has product prices</summary>
	public bool HasProductPrices
	{
		get { return ((this.ProductPrices != null) && (this.ProductPrices.Length != 0)); }
	}
	/// <summary>Product variations</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRODUCTVARIATION_EXTEND)]
	public ProductVariationInput[] ProductVariations
	{
		get { return (ProductVariationInput[])this.DataSource[Constants.FIELD_PRODUCT_PRODUCTVARIATION_EXTEND]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCTVARIATION_EXTEND] = value; }
	}
	/// <summary>Has product variations</summary>
	public bool HasProductVariations
	{
		get { return ((this.ProductVariations != null) && (this.ProductVariations.Length != 0)); }
	}
	/// <summary>Product tag</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRODUCTTAG_EXTEND)]
	public ProductTagInput ProductTag
	{
		get { return (ProductTagInput)this.DataSource[Constants.FIELD_PRODUCT_PRODUCTTAG_EXTEND]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCTTAG_EXTEND] = value; }
	}
	/// <summary>Has product tag</summary>
	public bool HasProductTag
	{
		get { return (this.ProductTag != null); }
	}
	/// <summary>Product extend</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRODUCTEXTEND_EXTEND)]
	public ProductExtendInput ProductExtend
	{
		get { return (ProductExtendInput)this.DataSource[Constants.FIELD_PRODUCT_PRODUCTEXTEND_EXTEND]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCTEXTEND_EXTEND] = value; }
	}
	/// <summary>Has product extend</summary>
	public bool HasProductExtend
	{
		get { return (this.ProductExtend != null); }
	}
	/// <summary>Mall exhibits config</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_MALLEXHIBITSCONFIG_EXTEND)]
	public MallExhibitsConfigInput MallExhibitsConfig
	{
		get { return (MallExhibitsConfigInput)this.DataSource[Constants.FIELD_PRODUCT_MALLEXHIBITSCONFIG_EXTEND]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_MALLEXHIBITSCONFIG_EXTEND] = value; }
	}
	/// <summary>Has mall exhibits config</summary>
	public bool HasMallExhibitsConfig
	{
		get { return (this.MallExhibitsConfig != null); }
	}
	/// <summary>Product fixed purchase discount settings</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_EXTEND)]
	public ProductFixedPurchaseDiscountSettingHeader[] ProductFixedPurchaseDiscountSettings
	{
		get { return (ProductFixedPurchaseDiscountSettingHeader[])this.DataSource[Constants.FIELD_PRODUCT_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_EXTEND]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCTFIXEDPURCHASEDISCOUNTSETTING_EXTEND] = value; }
	}
	/// <summary>Has product fixed purchase discount settings</summary>
	public bool HasProductFixedPurchaseDiscountSettings
	{
		get { return ((this.ProductFixedPurchaseDiscountSettings != null) && (this.ProductFixedPurchaseDiscountSettings.Length != 0)); }
	}
	/// <summary>Product option setting 1</summary>
	public string ProductOptionSetting1
	{
		get { return (string)this.DataSource[ProductOptionSettingHelper.GetProductOptionSettingKey(1)]; }
		set { this.DataSource[ProductOptionSettingHelper.GetProductOptionSettingKey(1)] = value; }
	}
	/// <summary>Product option setting 2</summary>
	public string ProductOptionSetting2
	{
		get { return (string)this.DataSource[ProductOptionSettingHelper.GetProductOptionSettingKey(2)]; }
		set { this.DataSource[ProductOptionSettingHelper.GetProductOptionSettingKey(2)] = value; }
	}
	/// <summary>Product option setting 3</summary>
	public string ProductOptionSetting3
	{
		get { return (string)this.DataSource[ProductOptionSettingHelper.GetProductOptionSettingKey(3)]; }
		set { this.DataSource[ProductOptionSettingHelper.GetProductOptionSettingKey(3)] = value; }
	}
	/// <summary>Product option setting 4</summary>
	public string ProductOptionSetting4
	{
		get { return (string)this.DataSource[ProductOptionSettingHelper.GetProductOptionSettingKey(4)]; }
		set { this.DataSource[ProductOptionSettingHelper.GetProductOptionSettingKey(4)] = value; }
	}
	/// <summary>Product option setting 5</summary>
	public string ProductOptionSetting5
	{
		get { return (string)this.DataSource[ProductOptionSettingHelper.GetProductOptionSettingKey(5)]; }
		set { this.DataSource[ProductOptionSettingHelper.GetProductOptionSettingKey(5)] = value; }
	}
	/// <summary>在庫管理しない商品？</summary>
	public bool IsStockUnmanaged
	{
		get { return (this.StockManagementKbn == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED); }
	}
	/// <summary>バリエーションが存在するか？</summary>
	public bool HasProductVariation
	{
		get { return (this.UseVariationFlg == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE); }
	}
	/// <summary>Brand id 1 for check</summary>
	public string BrandId1ForCheck
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID1 + "_for_check"]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_BRAND_ID1 + "_for_check"] = value; }
	}
	/// <summary>カート投入URL制限フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG)]
	public string AddCartUrlLimitFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG] = value; }
	}
	/// <summary>Product stocks</summary>
	public ProductStockInput[] ProductStocks
	{
		get { return (ProductStockInput[])this.DataSource[Constants.FIELD_PRODUCT_PRODUCTSTOCK_EXTEND]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCTSTOCK_EXTEND] = value; }
	}
	/// <summary>Has product stock</summary>
	public bool HasProductStock
	{
		get { return ((this.ProductStocks != null) && this.ProductStocks.Length != 0); }
	}
	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_DATE_CHANGED] = value; }
	}
	/// <summary>Product id old</summary>
	public string ProductIdOld
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_ID + "_old"]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_ID + "_old"] = value; }
	}
	/// <summary>Product stock message name</summary>
	public string ProductStockMessageName
	{
		get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME]); }
		set { this.DataSource[Constants.FIELD_PRODUCTSTOCKMESSAGE_STOCK_MESSAGE_NAME] = value; }
	}
	/// <summary>Shop shipping name</summary>
	[JsonProperty(Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME)]
	public string ShopShippingName
	{
		get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME]); }
		set { this.DataSource[Constants.FIELD_SHOPSHIPPING_SHOP_SHIPPING_NAME] = value; }
	}
	/// <summary>Product category name 1</summary>
	public string ProductCategoryName1
	{
		get { return StringUtility.ToEmpty(this.DataSource["category_name1"]); }
		set { this.DataSource["category_name1"] = value; }
	}
	/// <summary>Product category name 2</summary>
	public string ProductCategoryName2
	{
		get { return StringUtility.ToEmpty(this.DataSource["category_name2"]); }
		set { this.DataSource["category_name2"] = value; }
	}
	/// <summary>Product category name 3</summary>
	public string ProductCategoryName3
	{
		get { return StringUtility.ToEmpty(this.DataSource["category_name3"]); }
		set { this.DataSource["category_name3"] = value; }
	}
	/// <summary>Product category name 4</summary>
	public string ProductCategoryName4
	{
		get { return StringUtility.ToEmpty(this.DataSource["category_name4"]); }
		set { this.DataSource["category_name4"] = value; }
	}
	/// <summary>Product category name 5</summary>
	public string ProductCategoryName5
	{
		get { return StringUtility.ToEmpty(this.DataSource["category_name5"]); }
		set { this.DataSource["category_name5"] = value; }
	}
	/// <summary>Product brand name 1</summary>
	public string ProductBrandName1
	{
		get { return StringUtility.ToEmpty(this.DataSource["brand_name1"]); }
		set { this.DataSource["brand_name1"] = value; }
	}
	/// <summary>Product brand name 2</summary>
	public string ProductBrandName2
	{
		get { return StringUtility.ToEmpty(this.DataSource["brand_name2"]); }
		set { this.DataSource["brand_name2"] = value; }
	}
	/// <summary>Product brand name 3</summary>
	public string ProductBrandName3
	{
		get { return StringUtility.ToEmpty(this.DataSource["brand_name3"]); }
		set { this.DataSource["brand_name3"] = value; }
	}
	/// <summary>Product brand name 4</summary>
	public string ProductBrandName4
	{
		get { return StringUtility.ToEmpty(this.DataSource["brand_name4"]); }
		set { this.DataSource["brand_name4"] = value; }
	}
	/// <summary>Product brand name 5</summary>
	public string ProductBrandName5
	{
		get { return StringUtility.ToEmpty(this.DataSource["brand_name5"]); }
		set { this.DataSource["brand_name5"] = value; }
	}
	/// <summary>Product tax category name</summary>
	[JsonProperty(Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_NAME)]
	public string ProductTaxCategoryName
	{
		get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_NAME]); }
		set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_NAME] = value; }
	}
	/// <summary>Guid string</summary>
	public string GuidString { get; set; }
	/// <summary>Is product option value input basic type</summary>
	[JsonProperty("product_option_value_input_basic_type")]
	public string ProductOptionValueInputBasicType
	{
		get { return (string)this.DataSource["product_option_value_input_basic_type"]; }
		set { this.DataSource["product_option_value_input_basic_type"] = value; }
	}
	/// <summary>会員ランクポイント付与率除外設定フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG)]
	public string MemberRankPointExcludeFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG] = value; }
	}
	/// <summary>店舗受取可能フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_STOREPICKUP_FLG)]
	public string StorePickupFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_STOREPICKUP_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_STOREPICKUP_FLG] = value; }
	}
	/// <summary>配送料無料適用外フラグ</summary>
	[JsonProperty(Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG)]
	public string ExcludeFreeShippingFlg
	{
		get { return (string)this.DataSource[Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG]; }
		set { this.DataSource[Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG] = value; }
	}
	#endregion
}
