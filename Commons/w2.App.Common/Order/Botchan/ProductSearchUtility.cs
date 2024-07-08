/*
=========================================================================================================
  Module      : Product Search Utility(ProductSearchUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Botchan;
using w2.Common.Util;
using w2.Domain.Product;

namespace w2.App.Common.Order.Botchan
{
	/// <summary>
	/// Product search utility
	/// </summary>
	public class ProductSearchUtility : BotChanUtility
	{
		/// <summary>
		/// 商品取得
		/// </summary>
		/// <param name="shopId">ショップ番号</param>
		/// <param name="productIds">商品番号配列</param>
		/// <param name="errorTypeList">エラーリスト</param>
		/// <param name="memo">メモー</param>
		/// <returns>商品レスポンス</returns>
		public static ProductSearchResponse GetProducts(
			string shopId,
			string[] productIds,
			ref List<BotchanMessageManager.MessagesCode> errorTypeList,
			ref string memo)
		{
			if (errorTypeList.Count > 0) return null;

			var products = new ProductService().GetProducts(
				shopId,
				productIds);
			var productInformationList = new List<ProductSearchResponse.ProductInformation>();

			foreach (var product in products)
			{
				var productInformation = new ProductSearchResponse.ProductInformation
				{
					ProductInfo = CreateProductInfo(product),
					ProductStockInfo = CreateProductStockInfo(product),
				};
				productInformationList.Add(productInformation);
			}

			if (productInformationList.Count == 0)
			{
				errorTypeList.Add(BotchanMessageManager.MessagesCode.FRONT_PRODUCT_NO_ITEM);
				memo = MessageManager.GetMessages(BotchanMessageManager.MessagesCode.FRONT_PRODUCT_NO_ITEM.ToString());
				return null;
			}

			var productSearch = new ProductSearchResponse
			{
				Result = true,
				Message = Constants.BOTCHAN_API_SUCCESS_MESSAGE,
				MessageId = Constants.BOTCHAN_API_SUCCESS_MESSAGE_ID,
				Data = productInformationList.ToArray(),
			};
			return productSearch;
		}

		/// <summary>
		/// Create product info
		/// </summary>
		/// <param name="productModel">Product model</param>
		/// <returns>Product info</returns>
		private static ProductSearchResponse.Product CreateProductInfo(ProductModel productModel)
		{
			var product = new ProductSearchResponse.Product
			{
				ShopId = StringUtility.ToEmpty(productModel.ShopId),
				SupplierId = StringUtility.ToEmpty(productModel.SupplierId),
				ProductId = StringUtility.ToEmpty(productModel.ProductId),
				CooperationId1 = StringUtility.ToEmpty(productModel.CooperationId1),
				CooperationId2 = StringUtility.ToEmpty(productModel.CooperationId2),
				CooperationId3 = StringUtility.ToEmpty(productModel.CooperationId3),
				CooperationId4 = StringUtility.ToEmpty(productModel.CooperationId4),
				CooperationId5 = StringUtility.ToEmpty(productModel.CooperationId5),
				MallExProductId = StringUtility.ToEmpty(productModel.MallExProductId),
				MakerId = StringUtility.ToEmpty(productModel.MakerId),
				MakerName = StringUtility.ToEmpty(productModel.MakerName),
				Name = StringUtility.ToEmpty(productModel.Name),
				NameKana = StringUtility.ToEmpty(productModel.NameKana),
				Name2 = StringUtility.ToEmpty(productModel.Name2),
				DisplayPrice = ConvertStringToDecimal(StringUtility.ToEmpty(productModel.DisplayPrice)),
				DisplaySpecialPrice = productModel.DisplaySpecialPrice,
				TaxIncludedFlg = StringUtility.ToEmpty(productModel.TaxIncludedFlg),
				TaxRate = ConvertStringToDecimal(StringUtility.ToEmpty(productModel.TaxRate)),
				TaxRoundType = StringUtility.ToEmpty(productModel.TaxRoundType),
				PricePretax = ConvertStringToDecimal(StringUtility.ToEmpty(productModel.PricePretax)),
				PriceShipping = productModel.PriceShipping,
				ShippingType = StringUtility.ToEmpty(productModel.ShippingType),
				ShippingSizeKbn = StringUtility.ToEmpty(productModel.ShippingSizeKbn),
				PointKbn1 = StringUtility.ToEmpty(productModel.PointKbn1),
				Point1 = ConvertStringToDecimal(StringUtility.ToEmpty(productModel.Point1)),
				PointKbn2 = StringUtility.ToEmpty(productModel.PointKbn2),
				Point2 = ConvertStringToDecimal(StringUtility.ToEmpty(productModel.Point2)),
				PointKbn3 = StringUtility.ToEmpty(productModel.PointKbn3),
				Point3 = ConvertStringToDecimal(StringUtility.ToEmpty(productModel.Point3)),
				CampaignFrom = StringUtility.ToEmpty(productModel.CampaignFrom),
				CampaignTo = StringUtility.ToEmpty(productModel.CampaignTo),
				CampaignPointKbn = StringUtility.ToEmpty(productModel.CampaignPointKbn),
				CampaignPoint = ConvertStringToInt(StringUtility.ToEmpty(productModel.CampaignPoint)),
				MaxSellQuantity = ConvertStringToInt(StringUtility.ToEmpty(productModel.MaxSellQuantity)),
				RelatedProductIdCs1 = StringUtility.ToEmpty(productModel.RelatedProductIdCs1),
				RelatedProductIdCs2 = StringUtility.ToEmpty(productModel.RelatedProductIdCs2),
				RelatedProductIdCs3 = StringUtility.ToEmpty(productModel.RelatedProductIdCs3),
				RelatedProductIdCs4 = StringUtility.ToEmpty(productModel.RelatedProductIdCs4),
				RelatedProductIdCs5 = StringUtility.ToEmpty(productModel.RelatedProductIdCs5),
				RelatedProductIdUs1 = StringUtility.ToEmpty(productModel.RelatedProductIdUs1),
				RelatedProductIdUs2 = StringUtility.ToEmpty(productModel.RelatedProductIdUs2),
				RelatedProductIdUs3 = StringUtility.ToEmpty(productModel.RelatedProductIdUs3),
				RelatedProductIdUs4 = StringUtility.ToEmpty(productModel.RelatedProductIdUs4),
				RelatedProductIdUs5 = StringUtility.ToEmpty(productModel.RelatedProductIdUs5),
				ReservationFlg = StringUtility.ToEmpty(productModel.ReservationFlg),
				FixedPurchaseFlg = StringUtility.ToEmpty(productModel.FixedPurchaseFlg),
				DateCreated = (DateTime)productModel.DateCreated,
				DateChanged = (DateTime)productModel.DateChanged,
				MemberRankDiscountFlg = StringUtility.ToEmpty(productModel.MemberRankDiscountFlg),
				DisplayMemberRank = StringUtility.ToEmpty(productModel.DisplayMemberRank),
				BrandId1 = StringUtility.ToEmpty(productModel.BrandId1),
				BrandId2 = StringUtility.ToEmpty(productModel.BrandId2),
				BrandId3 = StringUtility.ToEmpty(productModel.BrandId3),
				BrandId4 = StringUtility.ToEmpty(productModel.BrandId4),
				BrandId5 = StringUtility.ToEmpty(productModel.BrandId5),
				GiftFlg = StringUtility.ToEmpty(productModel.GiftFlg),
				AgeLimitFlg = StringUtility.ToEmpty(productModel.AgeLimitFlg),
				PluralShippingPriceFreeFlg = StringUtility.ToEmpty(productModel.PluralShippingPriceFreeFlg),
				DigitalContentsFlg = StringUtility.ToEmpty(productModel.DigitalContentsFlg),
				DownloadUrl = StringUtility.ToEmpty(productModel.DownloadUrl),
				DisplaySellFlg = StringUtility.ToEmpty(productModel.DisplaySellFlg),
				LimitedPaymentIds = StringUtility.ToEmpty(productModel.LimitedPaymentIds),
				FixedPurchaseFirstTimePrice = productModel.FixedPurchaseFirsttimePrice,
				FixedPurchasePrice = productModel.FixedPurchasePrice,
				BundleItemDisplayType = StringUtility.ToEmpty(productModel.BundleItemDisplayType),
				ProductType = StringUtility.ToEmpty(productModel.ProductType),
				LimitedFixedPurchaseKbn1Setting = StringUtility.ToEmpty(productModel.LimitedFixedPurchaseKbn1Setting),
				LimitedFixedPurchaseKbn3Setting = StringUtility.ToEmpty(productModel.LimitedFixedPurchaseKbn3Setting),
				LimitedFixedPurchaseKbn4Setting = StringUtility.ToEmpty(productModel.LimitedFixedPurchaseKbn4Setting),
				RecommendProductId = StringUtility.ToEmpty(productModel.RecommendProductId),
				CooperationId6 = StringUtility.ToEmpty(productModel.CooperationId6),
				CooperationId7 = StringUtility.ToEmpty(productModel.CooperationId7),
				CooperationId8 = StringUtility.ToEmpty(productModel.CooperationId8),
				CooperationId9 = StringUtility.ToEmpty(productModel.CooperationId9),
				CooperationId10 = StringUtility.ToEmpty(productModel.CooperationId10),
				FixedPurchaseProductOrderLimitFlg = StringUtility.ToEmpty(productModel.CheckFixedProductOrderFlg),
				DisplayOnlyFixedPurchaseMemberFlg = StringUtility.ToEmpty(productModel.DisplayOnlyFixedPurchaseMemberFlg),
				SellOnlyFixedPurchaseMemberFlg = StringUtility.ToEmpty(productModel.SellOnlyFixedPurchaseMemberFlg),
				ProductWeightGram = ConvertStringToInt(StringUtility.ToEmpty(productModel.ProductWeightGram)),
				TaxCategoryId = StringUtility.ToEmpty(productModel.TaxCategoryId),
				ProductOrderLimitFlgFp = StringUtility.ToEmpty(productModel.CheckProductOrderFlg),
				FixedPurchaseCancelableCount = ConvertStringToInt(StringUtility.ToEmpty(productModel.FixedPurchasedCancelableCount)),
				FixedPurchaseLimitedUserLevelIds = StringUtility.ToEmpty(productModel.FixedPurchasedLimitedUserLevelIds),
				FixedPurchaseNextShippingProductId = StringUtility.ToEmpty(productModel.FixedPurchaseNextShippingProductId),
				FixedPurchaseNextShippingVariationId = StringUtility.ToEmpty(productModel.FixedPurchaseNextShippingVariationId),
				FixedPurchaseNextShippingItemQuantity = ConvertStringToInt(StringUtility.ToEmpty(productModel.FixedPurchaseNextShippingItemQuantity)),
				FixedPurchaseLimitedSkippedCount = productModel.FixedPurchaseLimitedSkippedCount,
			};
			return product;
		}

		/// <summary>
		/// Create product stock info
		/// </summary>
		/// <param name="productModel">Product model</param>
		/// <returns>Product stock info</returns>
		private static ProductSearchResponse.ProductStock CreateProductStockInfo(ProductModel productModel)
		{
			var productStock = new ProductSearchResponse.ProductStock
			{
				ShopId = StringUtility.ToEmpty(productModel.ShopId),
				ProductId = StringUtility.ToEmpty(productModel.ProductId),
				VariationId = StringUtility.ToEmpty(productModel.VariationId),
				Stock = productModel.ProductStockStock,
				RealStock = productModel.ProductStockRealstock,
				RealStockB = productModel.ProductStockRealstockB,
				RealStockC = productModel.ProductStockRealstockC,
				RealStockReserved = productModel.ProductStockRealstockReserved,
				DateCreated = (DateTime)productModel.ProductStockDateCreated,
				DateChanged = (DateTime)productModel.ProductStockDateChanged,
			};
			return productStock;
		}

		/// <summary>
		/// Convert string to decimal
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>Decimal value type</returns>
		private static decimal ConvertStringToDecimal(string value)
		{
			var defaultValue = 0m;
			if (decimal.TryParse(value, out defaultValue) == false) return defaultValue;
			return decimal.Parse(value);
		}

		/// <summary>
		/// Convert string to int
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>Int value type</returns>
		private static int ConvertStringToInt(string value)
		{
			var defaultValue = 0;
			if (int.TryParse(value, out defaultValue) == false) return defaultValue;
			return int.Parse(value);
		}
	}
}
