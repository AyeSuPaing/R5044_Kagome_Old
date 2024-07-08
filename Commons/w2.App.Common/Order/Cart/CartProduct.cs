/*
=========================================================================================================
  Module      : カート商品情報クラス(CartProduct.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.ContentsLog;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.SubscriptionBox;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>
	/// カート商品情報クラス
	/// </summary>
	///*********************************************************************************************
	[Serializable]
	public class CartProduct
	{
		#region 列挙体
		/// <summary>商品注文数の検証結果</summary>
		public enum ValidateQuantityResult
		{
			/// <summary>上限値超え</summary>
			OverMaxSellQuantity,
			/// <summary>下限値下回り</summary>
			LowerMinSellQuantity
		}
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drvProduct">商品情報</param>
		/// <param name="addCartKbn">カート投入区分</param>
		/// <param name="strProductSaleId">商品セールID</param>
		/// <param name="iProductCount">商品数</param>
		/// <param name="blUpdateCartDb">DB利用可否</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="contentsLogModel">コンテンツログ</param>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="nextShippingDate">次回配送日</param>
		public CartProduct(
			DataRowView drvProduct,
			Constants.AddCartKbn addCartKbn,
			string strProductSaleId,
			int iProductCount,
			bool blUpdateCartDb,
			string fixedPurchaseId = "",
			ContentsLogModel contentsLogModel = null,
			string subscriptionBoxCourseId = "",
			string lastChanged = "",
			DateTime nextShippingDate = default(DateTime))
		{
			this.UpdateCartDb = blUpdateCartDb;

			//------------------------------------------------------
			// 基本情報セット
			//------------------------------------------------------
			this.ShopId = (string)drvProduct[Constants.FIELD_PRODUCT_SHOP_ID];
			this.ProductId = (string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID];
			this.VariationId = (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID];
			this.SupplierId = (string)drvProduct[Constants.FIELD_PRODUCT_SUPPLIER_ID];
			this.ProductName = (string)drvProduct[Constants.FIELD_PRODUCT_NAME];
			this.ProductNameKana = (string)drvProduct[Constants.FIELD_PRODUCT_NAME_KANA];
			this.OutlineKbn = (string)drvProduct[Constants.FIELD_PRODUCT_OUTLINE_KBN];
			this.Outline = (string)drvProduct[Constants.FIELD_PRODUCT_OUTLINE];
			this.DisplayKbn = (string)drvProduct[Constants.FIELD_PRODUCT_DISPLAY_KBN];
			this.CategoryId1 = (string)drvProduct[Constants.FIELD_PRODUCT_CATEGORY_ID1];
			this.CategoryId2 = (string)drvProduct[Constants.FIELD_PRODUCT_CATEGORY_ID2];
			this.CategoryId3 = (string)drvProduct[Constants.FIELD_PRODUCT_CATEGORY_ID3];
			this.CategoryId4 = (string)drvProduct[Constants.FIELD_PRODUCT_CATEGORY_ID4];
			this.CategoryId5 = (string)drvProduct[Constants.FIELD_PRODUCT_CATEGORY_ID5];
			this.IconFlg = new string[10];
			this.IconFlg[0] = (string)drvProduct[Constants.FIELD_PRODUCT_ICON_FLG1];
			this.IconFlg[1] = (string)drvProduct[Constants.FIELD_PRODUCT_ICON_FLG2];
			this.IconFlg[2] = (string)drvProduct[Constants.FIELD_PRODUCT_ICON_FLG3];
			this.IconFlg[3] = (string)drvProduct[Constants.FIELD_PRODUCT_ICON_FLG4];
			this.IconFlg[4] = (string)drvProduct[Constants.FIELD_PRODUCT_ICON_FLG5];
			this.IconFlg[5] = (string)drvProduct[Constants.FIELD_PRODUCT_ICON_FLG6];
			this.IconFlg[6] = (string)drvProduct[Constants.FIELD_PRODUCT_ICON_FLG7];
			this.IconFlg[7] = (string)drvProduct[Constants.FIELD_PRODUCT_ICON_FLG8];
			this.IconFlg[8] = (string)drvProduct[Constants.FIELD_PRODUCT_ICON_FLG9];
			this.IconFlg[9] = (string)drvProduct[Constants.FIELD_PRODUCT_ICON_FLG10];
			this.IconTermEnd = new DateTime?[10];
			this.IconTermEnd[0] = (drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END1] is DBNull) ? null : (DateTime?)drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END1];
			this.IconTermEnd[1] = (drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END2] is DBNull) ? null : (DateTime?)drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END2];
			this.IconTermEnd[2] = (drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END3] is DBNull) ? null : (DateTime?)drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END3];
			this.IconTermEnd[3] = (drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END4] is DBNull) ? null : (DateTime?)drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END4];
			this.IconTermEnd[4] = (drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END5] is DBNull) ? null : (DateTime?)drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END5];
			this.IconTermEnd[5] = (drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END6] is DBNull) ? null : (DateTime?)drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END6];
			this.IconTermEnd[6] = (drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END7] is DBNull) ? null : (DateTime?)drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END7];
			this.IconTermEnd[7] = (drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END8] is DBNull) ? null : (DateTime?)drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END8];
			this.IconTermEnd[8] = (drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END9] is DBNull) ? null : (DateTime?)drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END9];
			this.IconTermEnd[9] = (drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END10] is DBNull) ? null : (DateTime?)drvProduct[Constants.FIELD_PRODUCT_ICON_TERM_END10];
			this.PointKbn1 = (string)drvProduct[Constants.FIELD_PRODUCT_POINT_KBN1];
			this.Point1 = (decimal)drvProduct[Constants.FIELD_PRODUCT_POINT1];
			this.PointKbn2 = (string)drvProduct[Constants.FIELD_PRODUCT_POINT_KBN2];
			this.Point2 = (decimal)drvProduct[Constants.FIELD_PRODUCT_POINT2];
			this.MemberRankPointExcludeFlg = (string)drvProduct[Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG];
			this.TaxIncludedFlg = TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag();
			this.TaxRate = (decimal)drvProduct[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_RATE];
			this.TaxRoundType = Constants.TAX_EXCLUDED_FRACTION_ROUNDING;
			this.VariationName1 = (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
			this.VariationName2 = (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
			this.VariationName3 = (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
			this.ProductMaxSellQuantity = (int)drvProduct[Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY];
			this.ProductVariationImageHead = (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD];
			this.ProductVariationMobileImage = (string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE];
			this.ShippingType = (string)drvProduct[Constants.FIELD_PRODUCT_SHIPPING_TYPE];
			this.ShippingSizeKbn = (string)drvProduct[Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN];
			this.StockManagementKbn = (string)drvProduct[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN];
			this.ProductSaleId = strProductSaleId;
			this.ReturnExchangeMessage = (string)drvProduct[Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE];
			this.ReturnExchangeMessageMobile = (string)drvProduct[Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE_MOBILE];
			this.BrandId = (string)drvProduct[Constants.FIELD_PRODUCT_BRAND_ID1];
			this.BrandId2 = (string)drvProduct[Constants.FIELD_PRODUCT_BRAND_ID2];
			this.BrandId3 = (string)drvProduct[Constants.FIELD_PRODUCT_BRAND_ID3];
			this.BrandId4 = (string)drvProduct[Constants.FIELD_PRODUCT_BRAND_ID4];
			this.BrandId5 = (string)drvProduct[Constants.FIELD_PRODUCT_BRAND_ID5];
			this.IsPluralShippingPriceFree = (string)drvProduct[Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG];
			this.IsDigitalContents = ((string)drvProduct[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG] == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID);
			this.DownloadUrl = ((string)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL]);
			this.SerialKeys = new List<string>();
			this.CooperationId = new List<string>();
			for (int index = 1; index <= Constants.COOPERATION_ID_COLUMNS_COUNT; index++)
			{
				this.CooperationId.Add((string)drvProduct["variation_" + Constants.HASH_KEY_COOPERATION_ID + index]);
			}
			this.IsMemberRankDiscount = Constants.MEMBER_RANK_OPTION_ENABLED
				? (((string)drvProduct[Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG]
					== Constants.FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_VALID))
				: false;
			this.FixedPurchaseId = fixedPurchaseId;

			this.LimitedPaymentIds = StringUtility.ToEmpty(drvProduct[Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS]).Split(',');

			this.BundleItemDisplayType = (string)drvProduct[Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE];
			this.ProductType = (string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_TYPE];
			this.ProductBundleId = string.Empty;
			this.ApplyOrder = Constants.FLG_PRODUCTBUNDLE_APPLY_ORDER_DEFAULT;
			this.MultipleApplyFlg = Constants.FLG_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG_INVALID;
			this.LimitedFixedPurchaseKbn1Setting = ((string)drvProduct[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING]);
			this.LimitedFixedPurchaseKbn3Setting = ((string)drvProduct[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING]);
			this.LimitedFixedPurchaseKbn4Setting = ((string)drvProduct[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING]);
			this.SubscriptionBoxFlag = ((string)drvProduct[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG]);
			this.SubscriptionBoxCourseId = subscriptionBoxCourseId;
			this.RecommendProductId = (string)drvProduct[Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID];
			this.HasVariation = ProductCommon.HasVariation(drvProduct);
			this.IsProductLimit = (addCartKbn == Constants.AddCartKbn.Normal)
				&& ((string)drvProduct[Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG] == Constants.FLG_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG_VALID);
			this.IsFixedPurchaseProductLimit =
				(addCartKbn == Constants.AddCartKbn.FixedPurchase)
				&& (((string)drvProduct[Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG] == Constants.FLG_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG_VALID)
					&& ((string.IsNullOrEmpty(StringUtility.ToEmpty(drvProduct[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE])) == false)
						|| (string.IsNullOrEmpty(StringUtility.ToEmpty(drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE])) == false)));
			this.FixedPurchaseCancelableCount = (int)drvProduct[Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT];
			this.ProductSizeFactor = (int)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR];
			this.LastChanged = lastChanged;
			this.NextShippingDate = nextShippingDate;
			this.DisplaySellFlg = 
				StringUtility.ToValueIfNullOrEmpty(
					drvProduct[Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG].ToString(), Constants.FLG_PRODUCT_DISPLAY_SELL_FLG_UNDISP);
			this.SellFrom = StringUtility.ToEmpty(drvProduct[Constants.FIELD_PRODUCT_SELL_FROM]);
			this.SellTo = StringUtility.ToEmpty(drvProduct[Constants.FIELD_PRODUCT_SELL_TO]);
			this.StorePickUpFlg = ((string)drvProduct[Constants.FIELD_PRODUCT_STOREPICKUP_FLG]);
			this.ExcludeFreeShippingFlg = StringUtility.ToEmpty(drvProduct[Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG]);

			//------------------------------------------------------
			// カート投入区分設定
			//------------------------------------------------------
			this.AddCartKbn = addCartKbn;

			//------------------------------------------------------
			// 価格設定
			//------------------------------------------------------
			SetPrice(drvProduct);
			if (IsSubscriptionBox && (string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false))
			{
				SetPriceForSubscriptionBox();
			}

			//------------------------------------------------------
			// 商品数セット
			//------------------------------------------------------
			this.CountSingle = iProductCount;
			this.Count = iProductCount;
			this.CountSingleBeforeDivide = iProductCount;
			this.CountBeforeDivide = iProductCount;

			//------------------------------------------------------
			// 商品タグ情報セット
			//------------------------------------------------------
			this.ProductTag = ProductTagUtility.GetProductTagData(this.ProductId);

			// 重量
			this.Weight = (int)drvProduct[Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM];

			// コンテンツログ
			this.ContentsLog = contentsLogModel;

			// Fixed Purchase Next Shipping
			this.FixedPurchaseNextShippingProductId = (string)drvProduct[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID];
			this.FixedPurchaseNextShippingVariationId = (string)drvProduct[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID];
			this.FixedPurchaseNextShippingItemQuantity = (int)drvProduct[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY];
			this.NextShippingItemFixedPurchaseKbn = (string)drvProduct[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN];
			this.NextShippingItemFixedPurchaseSetting = (string)drvProduct[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING];

			//------------------------------------------------------
			// 商品小計再計算
			//------------------------------------------------------
			Calculate();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drvProduct">商品情報</param>
		/// <param name="addCartKbn">カート投入区分</param>
		/// <param name="strProductSaleId">商品セールID</param>
		/// <param name="iProductCount">商品数</param>
		/// <param name="blUpdateCartDb">DB利用可否</param>
		/// <param name="posl">商品付帯情報</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="contentsLogModel">コンテンツログ</param>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="nextShippingDate">次回配送日</param>
		public CartProduct(
			DataRowView drvProduct,
			Constants.AddCartKbn addCartKbn,
			string strProductSaleId,
			int iProductCount,
			bool blUpdateCartDb,
			ProductOptionSettingList posl,
			string fixedPurchaseId = "",
			ContentsLogModel contentsLogModel = null,
			string subscriptionBoxCourseId = "",
			string lastChanged = "",
			DateTime nextShippingDate = default(DateTime))
			: this(drvProduct, addCartKbn, strProductSaleId, iProductCount, blUpdateCartDb, fixedPurchaseId, contentsLogModel, subscriptionBoxCourseId, lastChanged, nextShippingDate)
		{
			// 商品付帯情報設定
			this.ProductOptionSettingList = (posl.Items.Count == 0) && Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED
				? new ProductOptionSettingList(this.ShopId, this.ProductId)
				: posl;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drvProduct">商品情報</param>
		/// <param name="addCartKbn">カート投入区分</param>
		/// <param name="strProductSaleId">商品セールID</param>
		/// <param name="iProductCount">商品数</param>
		/// <param name="blUpdateCartDb">DB利用可否</param>
		/// <param name="strProductOptionTexts">商品付帯情報</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="cartId">カートID</param>
		/// <param name="contentsLogModel">コンテンツログ</param>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="nextShippingDate">次回配送日</param>
		/// <param name="fixedPurchaseItemOrderCountInput">定期商品購入回数（注文基準）</param>
		/// <param name="isOrderCombine">注文同梱フラグ</param>
		public CartProduct(
			DataRowView drvProduct,
			Constants.AddCartKbn addCartKbn,
			string strProductSaleId,
			int iProductCount,
			bool blUpdateCartDb,
			string strProductOptionTexts,
			string fixedPurchaseId = "",
			string cartId = null,
			ContentsLogModel contentsLogModel = null,
			string subscriptionBoxCourseId = "",
			string lastChanged = "",
			DateTime nextShippingDate = default(DateTime),
			string fixedPurchaseItemOrderCountInput = null,
			bool isOrderCombine = false)
			: this(drvProduct, addCartKbn, strProductSaleId, iProductCount, blUpdateCartDb, fixedPurchaseId, contentsLogModel, subscriptionBoxCourseId, lastChanged, nextShippingDate)
		{
			// 商品付帯情報設定
			if (isOrderCombine)
			{
				this.ProductOptionSettingList = new ProductOptionSettingList(this.ShopId, this.ProductId);
				this.ProductOptionSettingList.SetDefaultValueFromProductOptionTexts(
					strProductOptionTexts,
					isOrderCombine,
					true);
			}
			else
			{
				this.ProductOptionSettingList = new ProductOptionSettingList(this.ShopId, this.ProductId);
				var displayProductOptionTexts = ProductOptionSettingHelper.GetDisplayProductOptionTexts(strProductOptionTexts);
				this.ProductOptionSettingList.SetDefaultValueFromProductOptionTexts(displayProductOptionTexts);
			}

			this.CartId = cartId;
			this.FixedPurchaseItemOrderCountInput = fixedPurchaseItemOrderCountInput;
		}

		/// <summary>
		/// 商品セット情報セット
		/// </summary>
		/// <param name="cpsProductSet"></param>
		/// <param name="iProductSetItemNo"></param>
		public void SetProductSet(CartProductSet cpsProductSet, int iProductSetItemNo, decimal dSetItemPrice)
		{
			//------------------------------------------------------
			// 各種情報セット
			//------------------------------------------------------
			this.IsSetItem = true;
			this.ProductSet = cpsProductSet;
			this.ProductSetItemNo = iProductSetItemNo;
			this.Price = dSetItemPrice;
			this.PriceTax = TaxCalculationUtility.GetTaxPrice(this.Price, this.TaxRate, Constants.TAX_EXCLUDED_FRACTION_ROUNDING);

			//------------------------------------------------------
			// 商品小計再計算
			//------------------------------------------------------
			Calculate();
		}

		/// <summary>
		/// 商品数更新
		/// </summary>
		public void SetProductCount(string strCartId, int iProductCount)
		{
			//------------------------------------------------------
			// プロパティセット
			//------------------------------------------------------
			this.CountSingle = iProductCount;

			//------------------------------------------------------
			// DB更新
			//------------------------------------------------------
			if (this.UpdateCartDb)
			{
				using (SqlAccessor sqlAccessor = new SqlAccessor())
				using (SqlStatement sqlStatement = new SqlStatement("Cart", "UpdateProductCount"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_CART_CART_ID, strCartId);
					htInput.Add(Constants.FIELD_CART_SHOP_ID, this.ShopId);
					htInput.Add(Constants.FIELD_CART_PRODUCT_ID, this.ProductId);
					htInput.Add(Constants.FIELD_CART_VARIATION_ID, this.VariationId);
					htInput.Add(Constants.FIELD_CART_FIXED_PURCHASE_FLG, this.IsFixedPurchase ? Constants.FLG_CART_FIXED_PURCHASE_FLG_ON : Constants.FLG_CART_FIXED_PURCHASE_FLG_OFF);
					htInput.Add(Constants.FIELD_CART_PRODUCTSALE_ID, this.ProductSaleId);
					htInput.Add(Constants.FIELD_CART_PRODUCT_COUNT, this.CountSingle);
					htInput.Add(Constants.FIELD_CART_PRODUCT_OPTION_TEXTS, ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues());
					htInput.Add(Constants.FIELD_CART_GIFT_ORDER_FLG, this.IsGift ? Constants.FLG_CART_GIFT_ORDER_FLG_FLG_ON : Constants.FLG_CART_GIFT_ORDER_FLG_FLG_OFF);

					sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
				}
			}

			//------------------------------------------------------
			// 商品小計再計算
			//------------------------------------------------------
			Calculate();
		}

		/// <summary>
		/// ギフト商品の配送先割振前の商品数を保存
		/// </summary>
		public void SetCountBeforeDivide()
		{
			this.CountBeforeDivide = this.Count;
			this.CountSingleBeforeDivide = this.CountSingle;
		}

		/// <summary>
		/// カートの商品セールID更新
		/// </summary>
		/// <param name="cartId">カートID</param>
		/// <param name="productSaleId">商品セールID</param>
		public void UpdateProductSaleId(string cartId, string productSaleId)
		{
			// プロパティセット
			this.ProductSaleId = productSaleId;

			// DB更新
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Cart", "UpdateProductSaleId"))
			{
				var input = new Hashtable
				{
					{Constants.FIELD_CART_CART_ID, cartId},
					{Constants.FIELD_CART_SHOP_ID, this.ShopId},
					{Constants.FIELD_CART_PRODUCT_ID, this.ProductId},
					{Constants.FIELD_CART_VARIATION_ID, this.VariationId},
					{Constants.FIELD_CART_PRODUCTSALE_ID, this.ProductSaleId},
				};

				sqlStatement.ExecStatementWithOC(sqlAccessor, input);
			}
		}

		/// <summary>
		/// 商品価格更新
		/// </summary>
		/// <param name="product">商品情報</param>
		public void SetPrice(DataRowView product)
		{
			// 定期購入IDなし
			var isFirst = string.IsNullOrEmpty(this.FixedPurchaseId);
			if (isFirst == false)
			{
				var service = DomainFacade.Instance.FixedPurchaseService;
				if (Constants.FIXEDPURCHASE_ORDER_DISCOUNT_METHOD == Constants.FLG_FIXEDPURCHASE_PRODUCT_COUNT)
				{
					// もしくは定期台帳に登録されていない商品、もしくは定期商品注文回数が0回で初回適用とし、それ以外は2回目以降と判定する
					var fixedPurchaseItems = service.GetAllItem(this.FixedPurchaseId);
					var fixedPurchaseItem = fixedPurchaseItems
						.FirstOrDefault(i => ((i.ProductId == this.ProductId)
							&& (i.VariationId == this.VariationId)));
					isFirst = ((fixedPurchaseItem == null)
						|| ((fixedPurchaseItem != null)
							&& (fixedPurchaseItem.ItemOrderCount == 0)));
				}
				else
				{
					// 購入回数（注文基準）が0で初回、定期購入IDありで2回目以降と判定する
					var fixedPurchase = service.Get(this.FixedPurchaseId);
					isFirst = (fixedPurchase.OrderCount == 0);
				}
			}

			SetPrice(product, isFirst);
		}

		/// <summary>
		/// 商品価格更新
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="isFirstFixedPurchaseOrder">定期初回購入有無</param>
		public void SetPrice(DataRowView product, bool isFirstFixedPurchaseOrder)
		{
			// 同梱商品は必ず0円として扱う
			if (string.IsNullOrEmpty(this.ProductBundleId) == false)
			{
				this.PriceOrg = this.Price = 0;
				return;
			}

			this.PriceOrg = (decimal)product[Constants.FIELD_PRODUCTVARIATION_PRICE];
			// 特別価格？
			if (product[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] != DBNull.Value)
			{
				this.Price = (decimal)product[Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE];
			}
			else
			{
				// 通常価格設定
				this.Price = (decimal)product[Constants.FIELD_PRODUCTVARIATION_PRICE];
			}

			// 会員ランク？
			if ((product.Row.Table.Columns.Contains(Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION))
				&& (product[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] != DBNull.Value))
			{
				this.PriceMemberRank = (decimal)product[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION];
				this.Price = this.PriceMemberRank.Value;
			}
			// 商品セール？
			else if (this.ProductSaleId != "")
			{
				decimal? dProductSalePrice = OrderCommon.GetProductSalePrice(this.ShopId, this.ProductSaleId, this.ProductId, this.VariationId);
				if (dProductSalePrice != null)
				{
					this.Price = dProductSalePrice.Value;
				}
				else
				{
					// HACK:定期購入バッチで最新のセールIDを利用する場合は、他価格が優先されても無条件に残ったままになるのでここで打消しが必要。さらにProductSale.xmlが定期バッチに必要。
					// HACK:セールIDを途中で書き換えると商品数更新部分で不整合起きてFrontで注文進めない。
					//this.ProductSaleId = "";
				}
			}

			// 定期購入商品/頒布会商品の場合に、定期購入価格をセット
			if (this.IsFixedPurchase || this.IsSubscriptionBox)
			{
				// 定期購入IDがないもしくは定期初回購入の場合定期初回価格、それ以外は定期通常価格をセット
				if ((string.IsNullOrEmpty(this.FixedPurchaseId)) || (isFirstFixedPurchaseOrder))
				{
					if (product[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE] != DBNull.Value)
					{
						this.Price = (decimal)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE];
					}
					else if (product[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] != DBNull.Value)
					{
						this.Price = (decimal)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE];
					}
				}
				else
				{
					if (product[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] != DBNull.Value)
					{
						this.Price = (decimal)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE];
					}
				}
			}
			this.PriceTax = TaxCalculationUtility.GetTaxPrice(this.Price, this.TaxRate, Constants.TAX_EXCLUDED_FRACTION_ROUNDING);

		}

		/// <summary>
		/// 商品価格更新
		/// </summary>
		/// <param name="dPrice">商品価格</param>
		public void SetPrice(decimal dPrice)
		{
			//------------------------------------------------------
			// 価格設定
			//------------------------------------------------------
			this.Price = dPrice;
			if (this.IsSetItem == false)
			{
				this.PriceOrg = dPrice;
			}
			this.PriceTax = TaxCalculationUtility.GetTaxPrice(this.Price, this.TaxRate, Constants.TAX_EXCLUDED_FRACTION_ROUNDING);

			//------------------------------------------------------
			// 商品小計再計算
			//------------------------------------------------------
			Calculate();
		}

		/// <summary>
		/// 頒布会商品価格更新
		/// </summary>
		public void SetPriceForSubscriptionBox()
		{
			// 頒布会商品で頒布会キャンペーン期間の場合に頒布会キャンペーン期間価格をセット
			if (this.IsSubscriptionBox)
			{
				var selectedSubscriptionBox = DataCacheControllerFacade
					.GetSubscriptionBoxCacheController()
					.Get(this.SubscriptionBoxCourseId);

				var subscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
					x => (x.ProductId == this.ProductId)
						&& (x.VariationId == this.VariationId)
						&& x.IsInTerm(DateTime.Now));

				// キャンペーン期間か判定
				var isCampaignPeriod = (this.LastChanged != Constants.FLG_LASTCHANGED_BATCH)
					? OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxItem)
					: OrderCommon.IsSubscriptionBoxCampaignPeriodByNextShippingDate(subscriptionBoxItem, this.NextShippingDate);

				if (subscriptionBoxItem != null)
				{
					// キャンペーン期間であればキャンペーン期間価格を適用
					var campaignPrice = 0m;
					decimal.TryParse(subscriptionBoxItem.CampaignPrice.ToPriceString(), out campaignPrice);
					if (isCampaignPeriod) this.Price = campaignPrice;
				}

				this.SubscriptionBoxFixedAmount = selectedSubscriptionBox.FixedAmount;
			}
			this.PriceTax = TaxCalculationUtility.GetTaxPrice(this.Price, this.TaxRate, Constants.TAX_EXCLUDED_FRACTION_ROUNDING);

			//------------------------------------------------------
			// 商品小計再計算
			//------------------------------------------------------
			Calculate();
		}

		/// <summary>
		/// 商品小計再計算
		/// </summary>
		public void Calculate()
		{
			//------------------------------------------------------
			// 商品小計計算 = 商品単価（セット単価） x 商品数   （実際はさらに x セット数 がかかる）
			//------------------------------------------------------
			this.PriceSubtotalSingle = (this.Price + this.TotalOptionPrice) * this.CountSingle;
			this.PriceSubtotalSinglePretax = (this.PricePretax + this.OptionPricePretax) * this.CountSingle;
			this.PriceSubtotalSingleTax = (this.PriceTax + this.OptionPriceTax) * this.CountSingle;

			//------------------------------------------------------
			// 総商品小計計算
			//------------------------------------------------------
			// 通常再計算
			if ((this.IsSetItem == false) || (this.ProductSet == null))
			{
				// 総商品数計算 = 商品数
				this.Count = this.CountSingle;

				// 総商品小計 ＝ 商品小計
				this.PriceSubtotal = this.PriceSubtotalSingle;
				this.PriceSubtotalPretax = this.PriceSubtotalSinglePretax;
				this.PriceSubtotalTax = this.PriceSubtotalSingleTax;
			}
			// セット単位で再計算
			else
			{
				// セット商品再計算
				this.ProductSet.Calculate();

				// 総商品数 ＝ 商品数 x セット数
				this.Count = this.CountSingle * this.ProductSet.ProductSetCount;

				// 総商品小計 ＝ セット価格 x 総商品数
				this.PriceSubtotal = (this.Price + this.TotalOptionPrice) * this.Count;
				this.PriceSubtotalPretax = (this.PricePretax + this.OptionPricePretax) * this.Count;
				this.PriceSubtotalTax = (this.PriceTax + this.OptionPriceTax) * this.Count;
			}
		}

		/// <summary>
		/// セット商品情報セット
		/// </summary>
		private void SetCartProductSet(CartProductSet cpsProductSet)
		{
			// セット再計算
			cpsProductSet.Calculate();

			// セット商品情報セット
			this.ProductSet = cpsProductSet;
		}

		/// <summary>
		/// 商品タグ情報取得
		/// </summary>
		/// <param name="key">商品タグキー</param>
		/// <returns></returns>
		public string GetProductTag(string key)
		{
			return this.ProductTag.Contains(key) ? (string)this.ProductTag[key] : "";
		}

		/// <summary>
		/// 商品詳細リンク有効か?
		/// </summary>
		/// <returns>有効：true、無効：false</returns>
		public bool IsProductDetailLinkValid()
		{
			// 表示区分が「2：商品一覧×、商品詳細×」
			if (this.DisplayKbn == Constants.FLG_PRODUCT_DISPLAY_UNDISP_ALL) return false;

			// ノベルティの場合
			if (Constants.NOVELTY_OPTION_ENABLED && this.NoveltyId != "") return false;

			// リピートプラスONEの場合
			if (Constants.REPEATPLUSONE_OPTION_ENABLED) return false;

			return true;
		}

		/// <summary>
		/// 商品詳細URL取得
		/// </summary>
		public string CreateProductDetailUrl()
		{
			if (this.IsProductNovelty || Constants.REPEATPLUSONE_OPTION_ENABLED) return String.Empty;
			return ProductCommon.CreateProductDetailUrl(this.ShopId, "", "", "", this.ProductId, this.VariationId, this.ProductJointName, "");
		}

		/// <summary>
		/// カート商品情報オブジェクト複製
		/// </summary>
		/// <returns>複製したカート商品情報オブジェクト</returns>
		public CartProduct Clone()
		{
			var clone = (CartProduct)MemberwiseClone();
			return clone;
		}

		/// <summary>
		/// カート商品情報から注文商品情報作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="itemQuantity">注文数</param>
		/// <param name="itemQuantitySingle">注文数(セット未考慮)</param>
		/// <param name="orderShippingNo">配送先枝番</param>
		/// <param name="orderItemNo">注文商品枝番</param>
		/// <param name="orderSetPromotionNo">注文セットプロモーション枝番</param>
		/// <param name="orderSetPromotionItemNo">注文セットプロモーション商品枝番</param>
		/// <returns>注文商品情報</returns>
		public OrderItemModel CreateOrderItem(
			OrderModel order,
			int itemQuantity,
			int itemQuantitySingle,
			int orderShippingNo,
			int orderItemNo,
			int? orderSetPromotionNo,
			int? orderSetPromotionItemNo)
		{
			var orderItem = new OrderItemModel
			{
				OrderId = order.OrderId,
				OrderItemNo = orderItemNo,
				OrderShippingNo = orderShippingNo,
				ShopId = order.ShopId,
				ProductId = this.ProductId,
				VariationId = this.VariationId,
				SupplierId = this.SupplierId,
				ProductName = this.ProductJointName,
				ProductNameKana = this.ProductNameKana,
				ProductPrice = this.Price,
				ProductPriceOrg = this.PriceOrg,
				ProductPoint = (double)this.Point1,
				ProductTaxIncludedFlg = this.TaxIncludedFlg,
				ProductTaxRate = this.TaxRate,
				ProductTaxRoundType = this.TaxRoundType,
				ProductPricePretax = this.PricePretax,
				ProductPriceShip = null,
				ProductPriceCost = null,
				ItemQuantity = itemQuantity,
				ItemQuantitySingle = itemQuantitySingle,
				ItemPrice = PriceCalculator.GetItemPrice(this.PriceIncludedOptionPrice, itemQuantity),
				ItemPriceTax = PriceCalculator.GetItemPrice(this.PriceTax + this.OptionPriceTax, itemQuantity),
				ItemPriceSingle = PriceCalculator.GetItemPrice(this.PricePretax + this.OptionPricePretax, itemQuantitySingle),
				ProductPointKbn = this.PointKbn1,
				ProductSetId = this.IsSetItem ? this.ProductSet.ProductSetId : string.Empty,
				ProductSetNo = this.IsSetItem ? this.ProductSet.ProductSetNo : (int?)null,
				ProductSetCount = this.IsSetItem ? this.ProductSet.ProductSetCount : (int?)null,
				DateCreated = order.DateCreated,
				DateChanged = order.DateChanged,
				ProductOptionTexts = (this.ProductOptionSettingList != null)
					? this.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues()
					: string.Empty,
				BrandId = this.BrandId,
				DownloadUrl = this.DownloadUrl,
				ProductsaleId = this.ProductSaleId,
				CooperationId1 = this.CooperationId[0],
				CooperationId2 = this.CooperationId[1],
				CooperationId3 = this.CooperationId[2],
				CooperationId4 = this.CooperationId[3],
				CooperationId5 = this.CooperationId[4],
				CooperationId6 = this.CooperationId[5],
				CooperationId7 = this.CooperationId[6],
				CooperationId8 = this.CooperationId[7],
				CooperationId9 = this.CooperationId[8],
				CooperationId10 = this.CooperationId[9],
				OrderSetpromotionNo = orderSetPromotionNo,
				OrderSetpromotionItemNo = orderSetPromotionItemNo,
				NoveltyId = this.NoveltyId,
				RecommendId = this.RecommendId,
				FixedPurchaseItemOrderCount = this.IsFixedPurchase
					? (int?)1
					: null,
				FixedPurchaseProductFlg = this.IsFixedPurchase
					? Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON
					: Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
				ProductBundleId = this.ProductBundleId,
				BundleItemDisplayType = this.BundleItemDisplayType,
				ItemDiscountedPrice = (orderSetPromotionNo == null) ? this.DiscountedPriceUnAllocatedToSet : this.DiscountedPrice[int.Parse(orderSetPromotionNo.ToString())]
			};
			return orderItem;
		}

		/// <summary>
		/// 商品結合翻訳名設定
		/// </summary>
		/// <remarks>プロパティに翻訳名称を設定する</remarks>
		private string GetProductJointTranslationName()
		{
			// マスタに登録されている名称を取得
			var productJointName = GetProductJointNameDefault();

			// フロントからカート投入した時には言語コードが設定されていないため、Regionから取得する
			string languageCode = string.Empty;
			string languageLocaleId = string.Empty;
			if ((string.IsNullOrEmpty(this.LanguageCode) || string.IsNullOrEmpty(this.LanguageLocaleId)) && (HttpContext.Current != null))
			{
				languageCode = RegionManager.GetInstance().Region.LanguageCode;
				languageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId;
			}
			else
			{
				languageCode = this.LanguageCode;
				languageLocaleId = this.LanguageLocaleId;
			}

			// 翻訳名取得
			var translationSettings = this.HasVariation
				? NameTranslationCommon.GetProductAndVariationTranslationSettingsByVariationId(
					this.ProductId,
					this.VariationId,
					languageCode,
					languageLocaleId)
				: NameTranslationCommon.GetProductAndVariationTranslationSettingsByProductId(
				this.ProductId,
				languageCode,
				languageLocaleId);

			if (translationSettings.Any() == false) return productJointName;

			var productJointTranslationName = NameTranslationCommon.CreateProductJointTranslationName(
				translationSettings,
				m_productName,
				this.VariationName1,
				this.VariationName2,
				this.VariationName3,
				this.HasVariation);

			return productJointTranslationName;
		}

		/// <summary>
		/// 商品結合名デフォルト値取得
		/// </summary>
		/// <returns>商品結合名デフォルト値</returns>
		/// <remarks>商品／バリエーションマスタに登録されている名称を取得</remarks>
		private string GetProductJointNameDefault()
		{
			var product = new ProductService().GetProductVariation(
				this.ShopId,
				this.ProductId,
				this.VariationId,
				string.Empty);

			if (product == null) return string.Empty;

			var productJointName = product.Name;
			if (product.HasProductVariation)
			{
				productJointName += ProductCommon.CreateVariationName(
					product.VariationName1,
					product.VariationName2,
					product.VariationName3);
			}

			return productJointName;
		}

		/// <summary>
		/// 商品翻訳名取得
		/// </summary>
		/// <returns>商品翻訳名</returns>
		private string GetProductTranslationName()
		{
			// マスタに登録されている名称を取得
			var productNameDefault = GetProductNameDefault();

			// 翻訳名取得
			try
			{
				var productTranslationName = NameTranslationCommon.GetTranslationName(
					this.ProductId,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PRODUCT,
					Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PRODUCT_NAME,
					productNameDefault,
					RegionManager.GetInstance().Region.LanguageCode,
					RegionManager.GetInstance().Region.LanguageLocaleId);

				return productTranslationName;
			}
			catch (Exception)
			{
				return productNameDefault;
			}
		}

		/// <summary>
		/// 商品名デフォルト値取得
		/// </summary>
		/// <returns>商品名デフォルト値</returns>
		/// <remarks>商品マスタに登録されている名称を取得</remarks>
		private string GetProductNameDefault()
		{
			var product = new ProductService().GetProductVariation(
				this.ShopId,
				this.ProductId,
				this.VariationId,
				string.Empty);

			if (product == null) return string.Empty;

			return product.Name;
		}

		/// <summary>
		/// カート内の商品間で購入制限対象か
		/// </summary>
		/// <param name="target">チェック対象</param>
		/// <returns>カート内の商品間で購入制限対象か</returns>
		public bool IsCartProductLimit(CartProduct target)
		{
			if (this.ProductId != target.ProductId) return false;
			if (this.AddCartKbn != target.AddCartKbn) return false;
			return this.IsOrderLimitProduct;
		}

		/// <summary>
		/// Can Switch Product Fixed Purchase Next Shipping Second Time
		/// </summary>
		/// <returns>True: Can Switch Product Fixed Purchase Next Shipping Second Time</returns>
		public bool CanSwitchProductFixedPurchaseNextShippingSecondTime()
		{
			if ((Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED == false)
				|| string.IsNullOrEmpty(this.FixedPurchaseNextShippingProductId)
				|| (this.FixedPurchaseNextShippingItemQuantity <= 0)
				|| string.IsNullOrEmpty(this.NextShippingItemFixedPurchaseKbn)
				|| string.IsNullOrEmpty(this.NextShippingItemFixedPurchaseSetting)) return false;

			return true;
		}

		/// <summary>
		/// 注文可能数を伝えるメッセージを取得
		/// </summary>
		/// <param name="quantityResult">商品注文数の検証結果</param>
		/// <returns>メッセージ</returns>
		public string GetMesageForSellQuantity(ValidateQuantityResult quantityResult)
		{
			var sellQuantityMessage = string.Empty;
			switch (quantityResult)
			{
				// 上限値を超えた場合
				case ValidateQuantityResult.OverMaxSellQuantity:
					sellQuantityMessage = (IsMaxAndMinSameValue)
						? OrderCommon.GetErrorMessage(
							OrderErrorcode.MaxSellQuantitySameAsMinSellQuantity,
							ProductMinSellQuantity.ToString())
						: OrderCommon.GetErrorMessage(
							OrderErrorcode.DisplayOnMaxSellQuantityCaseOfMaxSellQuantityError,
							ProductMaxSellQuantity.ToString());
					break;

				// 下限値を下回った場合
				case ValidateQuantityResult.LowerMinSellQuantity:
					sellQuantityMessage = (IsMaxAndMinSameValue)
						? OrderCommon.GetErrorMessage(
							OrderErrorcode.MaxSellQuantitySameAsMinSellQuantity,
							ProductMinSellQuantity.ToString())
						: OrderCommon.GetErrorMessage(
							OrderErrorcode.DisplayOnMaxSellQuantityCaseOfMaxSellQuantityError,
							ProductMinSellQuantity.ToString());
					break;
			}
			return sellQuantityMessage;
		}

		/// <summary>
		/// 頒布会定額コースか
		/// </summary>
		/// <returns>定額であればTrue</returns>
		public bool IsSubscriptionBoxFixedAmount()
		{
			if ((this.IsSubscriptionBox == false) || string.IsNullOrEmpty(this.SubscriptionBoxCourseId)) return false;
			var subscriptionBox = new SubscriptionBoxService().GetByCourseId(this.SubscriptionBoxCourseId);
			var result = (subscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE);
			return result;
		}

		/// <summary>
		/// 頒布会選択可能期間表示するか
		/// </summary>
		/// <returns>設定されているであればTrue</returns>
		public static bool IsDisplaySubscriptionBoxSelectTime(string shopId, string subscriptionBoxCourseId = "", string productId = "", string variationId = "")
		{
			return string.IsNullOrEmpty(GetSubscriptionBoxSelectTermBr(shopId, subscriptionBoxCourseId, productId, variationId)) ? false : true;
		}


		/// <summary>
		/// 商品頒布会キャンベーン期間取得(改行)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="SubscriptionBoxCourseId">頒布会ID</param>
		/// <returns>キャンベーン期間</returns>
		public static string GetSubscriptionBoxTermBr(string shopId, string subscriptionBoxCourseId = "", string productId="", string variationId="", string linefeed = "～\r\n")
		{
			if (string.IsNullOrEmpty(subscriptionBoxCourseId)) return string.Empty;

			var selectedSubscriptionBox = DataCacheControllerFacade
				.GetSubscriptionBoxCacheController()
				.Get(subscriptionBoxCourseId);

			var subscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
				x => (x.ProductId == productId) && (x.VariationId == variationId));

			// 頒布会キャンペーン期間取得
			if (OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxItem) == false) return string.Empty;

			var beginDate = DateTimeUtility.ToStringFromRegion(subscriptionBoxItem.CampaignSince, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			var endDate = DateTimeUtility.ToStringFromRegion(subscriptionBoxItem.CampaignUntil, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			var subscriptionBoxTerm = beginDate + linefeed + endDate;
			return subscriptionBoxTerm;
		}

		/// <summary>
		/// 商品頒布会選択可能期間取得(改行)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="SubscriptionBoxCourseId">頒布会ID</param>
		/// <returns>選択可能期間</returns>
		public static string GetSubscriptionBoxSelectTermBr(string shopId, string subscriptionBoxCourseId = "", string productId = "", string variationId = "", string linefeed = "～\r\n")
		{
			if (string.IsNullOrEmpty(subscriptionBoxCourseId)) return string.Empty;

			var selectedSubscriptionBox = DataCacheControllerFacade
				.GetSubscriptionBoxCacheController()
				.Get(subscriptionBoxCourseId);

			var subscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
				x => (x.ProductId == productId) && (x.VariationId == variationId));

			// 頒布会選択可能期間取得
			if (OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxItem) == false) return string.Empty;

			var beginDate = DateTimeUtility.ToStringFromRegion(subscriptionBoxItem.SelectableSince, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			var endDate = DateTimeUtility.ToStringFromRegion(subscriptionBoxItem.SelectableUntil, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			if (string.IsNullOrEmpty(beginDate + endDate)) return null;
			var subscriptionBoxSelectTerm = beginDate + linefeed + endDate;
			return subscriptionBoxSelectTerm;
		}

		/// <summary>
		/// 商品頒布会キャンベーン期間か
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="SubscriptionBoxCourseId">頒布会ID</param>
		/// <returns>キャンベーン期間か</returns>
		public static bool IsSubscriptionBoxCampaignPeriod(string shopId, string subscriptionBoxCourseId = "", string productId = "", string variationId = "")
		{
			if (string.IsNullOrEmpty(subscriptionBoxCourseId)) return false;

			var selectedSubscriptionBox = DataCacheControllerFacade
				.GetSubscriptionBoxCacheController()
				.Get(subscriptionBoxCourseId);

			var subscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
				x => (x.ProductId == productId) && (x.VariationId == variationId));

			var isSubscriptionBoxCampaignPeriod = (OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxItem));
			return isSubscriptionBoxCampaignPeriod;
		}

		/// <summary>
		/// 頒布会コース表示名を取得
		/// </summary>
		/// <returns>頒布会コース表示名</returns>
		public string GetSubscriptionDisplayName()
		{
			if (this.IsSubscriptionBox == false) return string.Empty;

			var subscriptionBox = new SubscriptionBoxService().GetByCourseId(this.SubscriptionBoxCourseId);
			return subscriptionBox.DisplayName;
		}

		/// <summary>
		/// 対象商品と同じ商品か
		/// </summary>
		/// <param name="targetProduct">対象商品</param>
		/// <returns>同じ商品か</returns>
		/// <remarks>CartObject.GetSameProductと同じ条件</remarks>
		public bool CheckSameProduct(CartProduct targetProduct)
		{
			var result = ((this.ShopId == targetProduct.ShopId)
				&& (this.ProductId == targetProduct.ProductId)
				&& (this.VariationId == targetProduct.VariationId)
				&& (this.SubscriptionBoxCourseId == targetProduct.SubscriptionBoxCourseId)
				&& (this.IsFixedPurchase == targetProduct.IsFixedPurchase)
				&& (this.IsSubscriptionBox == targetProduct.IsSubscriptionBox)
				&& (this.IsGift == targetProduct.IsGift)
				&& ((this.ProductOptionSettingList != null ? this.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() : null)
					== (targetProduct.ProductOptionSettingList != null ? targetProduct.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() : null))
				&& ((this.ProductSet != null ? this.ProductSet.ProductSetId : null) == (targetProduct.ProductSet != null ? targetProduct.ProductSet.ProductSetId : null))
				&& ((this.ProductSet != null ? this.ProductSet.ProductSetNo : -1) == (targetProduct.ProductSet != null ? targetProduct.ProductSet.ProductSetNo : -1)));
			return result;
		}

		/// <summary>
		/// 対象商品と同じ商品か（商品付帯情報を比較しない）
		/// </summary>
		/// <param name="targetProduct">対象商品</param>
		/// <returns>カート内の同一商品(なければnull)</returns>
		public bool CheckSameProductWithoutOptionSetting(CartProduct targetProduct)
		{
			var result = ((this.ShopId == targetProduct.ShopId)
				&& (this.ProductId == targetProduct.ProductId)
				&& (this.VariationId == targetProduct.VariationId)
				&& (this.IsFixedPurchase == targetProduct.IsFixedPurchase)
				&& (this.IsGift == targetProduct.IsGift)
				&& ((this.ProductSet != null ? this.ProductSet.ProductSetId : null) == (targetProduct.ProductSet != null ? targetProduct.ProductSet.ProductSetId : null))
				&& ((this.ProductSet != null ? this.ProductSet.ProductSetNo : -1) == (targetProduct.ProductSet != null ? targetProduct.ProductSet.ProductSetNo : -1)));
			return result;
		}

		/// <summary>店舗ID</summary>
		public string ShopId { get; private set; }
		/// <summary>カートID</summary>
		public string CartId { get; set; }
		/// <summary>選択カートID</summary>
		public string CartIdSelect { get; set; }
		/// <summary>商品ID</summary>
		public string ProductId { get; private set; }
		/// <summary>バリエーションID</summary>
		public string VariationId { get; private set; }
		/// <summary>バリエーションID（商品ID除外）</summary>
		public string VId
		{
			get
			{
				if (this.VariationId.StartsWith(this.ProductId)) return this.VariationId.Substring(this.ProductId.Length);
				return "";
			}
		}
		/// <summary>サプライヤID</summary>
		public string SupplierId { get; private set; }
		/// <summary>商品結合名：商品名＋（あればバリエーション名）</summary>
		public string ProductJointName
		{
			get
			{
				var productJointName = Constants.GLOBAL_OPTION_ENABLE
					? GetProductJointTranslationName()
					: m_productName + (string.IsNullOrEmpty(this.VariationId)
						? ""
						: ProductCommon.CreateVariationName(
							this.VariationName1,
							this.VariationName2,
							this.VariationName3));
				if (this.IsFixedPurchase) productJointName = string.Format(
					"{0}{1}",
					this.IsSubscriptionBox
						? Constants.PRODUCT_SUBSCRIPTION_BOX_STRING
						: Constants.PRODUCT_FIXED_PURCHASE_STRING,
					productJointName);
				return productJointName;
			}
		}
		/// <summary>商品名</summary>
		public string ProductName
		{
			get
			{
				var productName = Constants.GLOBAL_OPTION_ENABLE ? GetProductTranslationName() : m_productName;
				if (this.IsFixedPurchase) productName = string.Format(
					"{0}{1}",
					this.IsSubscriptionBox
						? Constants.PRODUCT_SUBSCRIPTION_BOX_STRING
						: Constants.PRODUCT_FIXED_PURCHASE_STRING,
					productName);
				return productName;
			}
			private set { m_productName = value; }
		}
		private string m_productName = null;
		/// <summary>商品名かな</summary>
		public string ProductNameKana { get; private set; }
		/// <summary>商品概要HTML区分</summary>
		public string OutlineKbn { get; private set; }
		/// <summary>商品概要</summary>
		public string Outline { get; private set; }
		/// <summary>表示区分</summary>
		public string DisplayKbn { get; private set; }
		/// <summary>カテゴリID1</summary>
		public string CategoryId1 { get; private set; }
		/// <summary>カテゴリID2</summary>
		public string CategoryId2 { get; private set; }
		/// <summary>カテゴリID3</summary>
		public string CategoryId3 { get; private set; }
		/// <summary>カテゴリID4</summary>
		public string CategoryId4 { get; private set; }
		/// <summary>カテゴリID5</summary>
		public string CategoryId5 { get; private set; }
		/// <summary>アイコンフラグ</summary>
		public string[] IconFlg { get; private set; }
		/// <summary>アイコン有効期限</summary>
		public DateTime?[] IconTermEnd { get; private set; }
		/// <summary>ポイント１区分</summary>
		public string PointKbn1 { get; private set; }
		/// <summary>ポイント１</summary>
		public decimal Point1 { get; private set; }
		/// <summary>定期ポイント区分</summary>
		public string PointKbn2 { get; private set; }
		/// <summary>定期ポイント</summary>
		public decimal Point2 { get; private set; }
		/// <summary>税込フラグ</summary>
		public string TaxIncludedFlg { get; private set; }
		/// <summary>税率</summary>
		public decimal TaxRate { get; set; }
		/// <summary>税計算方法</summary>
		public string TaxRoundType { get; private set; }
		/// <summary>税込販売価格</summary>
		public decimal PricePretax
		{
			get
			{
				return TaxCalculationUtility.GetPriceTaxIncluded(
				this.Price,
				this.PriceTax);
			}
		}
		/// <summary>バリエーション名１</summary>
		public string VariationName1 { get; private set; }
		/// <summary>バリエーション名１</summary>
		public string VariationName2 { get; private set; }
		/// <summary>バリエーション名3</summary>
		public string VariationName3 { get; private set; }
		/// <summary>価格</summary>
		public decimal Price { get; private set; }
		/// <summary>価格（値引き前）</summary>
		public decimal PriceOrg { get; private set; }
		/// <summary>商品税額</summary>
		public decimal PriceTax { get; private set; }
		/// <summary>会員ランク価格</summary>
		public decimal? PriceMemberRank { get; private set; }
		/// <summary>会員ランク割引対象</summary>
		public bool IsMemberRankDiscount { get; private set; }
		/// <summary>商品バリエーション画像ヘッダ</summary>
		public string ProductVariationImageHead { get; private set; }
		/// <summary>商品バリエーションモバイル画像</summary>
		public string ProductVariationMobileImage { get; private set; }
		/// <summary>最大同時購入可能数</summary>
		public decimal ProductMaxSellQuantity { get; private set; }
		/// <summary>配送種別</summary>
		public string ShippingType { get; private set; }
		/// <summary>配送サイズ区分</summary>
		public string ShippingSizeKbn { get; private set; }
		/// <summary>在庫管理方法</summary>
		public string StockManagementKbn { get; private set; }
		/// <summary>返品交換ＰＣ文言</summary>
		public string ReturnExchangeMessage { get; private set; }
		/// <summary>返品交換モバイル文言</summary>
		public string ReturnExchangeMessageMobile { get; private set; }
		/// <summary>商品数（セット未考慮）</summary>
		/// <remarks>改ざんがあったので念のため対策</remarks>
		public int CountSingle
		{
			get { return m_iCountSingle; }
			set { m_iCountSingle = (value > 0) ? value : 1; }
		}
		private int m_iCountSingle;
		/// <summary>商品数（商品数ｘセット数）</summary>
		/// <remarks>改ざんがあったので念のため対策</remarks>
		public int Count
		{
			get { return m_iCount; }
			set { m_iCount = (value > 0) ? value : 1; }
		}
		private int m_iCount;
		/// <summary>ギフト商品の配送先割振前の商品数（セット未考慮）</summary>
		public int CountSingleBeforeDivide { get; private set; }
		/// <summary>ギフト商品の配送先割振前の商品数（商品数ｘセット数）</summary>
		public int CountBeforeDivide { get; private set; }
		/// <summary>セット単価割引額</summary>
		public decimal SetDiscountUnitPrice
		{
			get { return this.PriceOrg - this.Price; }
		}
		/// <summary>セット明細割引額</summary>
		public decimal SetDiscountItemPrice
		{
			get { return this.SetDiscountUnitPrice * this.Count; }
		}
		/// <summary>商品小計（商品単価ｘ商品数ｘセット数）</summary>
		public decimal PriceSubtotal { get; private set; }
		/// <summary>商品小計（セット未考慮）</summary>
		public decimal PriceSubtotalSingle { get; private set; }
		/// <summary>税込商品小計（商品単価ｘ商品数ｘセット数）</summary>
		public decimal PriceSubtotalPretax { get; private set; }
		/// <summary>税込商品小計（セット未考慮）</summary>
		public decimal PriceSubtotalSinglePretax { get; private set; }
		/// <summary>小計税額（商品単価ｘ商品数ｘセット数）</summary>
		public decimal PriceSubtotalTax { get; private set; }
		/// <summary>小計税額（セット未考慮）</summary>
		public decimal PriceSubtotalSingleTax { get; private set; }
		/// <summary>商品小計(割引金額の按分処理適用後)</summary>
		public decimal PriceSubtotalAfterDistribution { get; set; }
		/// <summary>調整金額(按分した商品分)</summary>
		public decimal ItemPriceRegulation { get; set; }
		/// <summary>商品小計(調整金額・割引金額の按分処理適用後)</summary>
		public decimal PriceSubtotalAfterDistributionAndRegulation
		{
			get { return this.PriceSubtotalAfterDistribution + this.ItemPriceRegulation; }
		}
		/// <summary>カート投入区分</summary>
		public Constants.AddCartKbn AddCartKbn { get; set; }
		/// <summary>定期購入設定</summary>
		public bool IsFixedPurchase
		{
			get
			{
				if (this.IsSubscriptionBox) return true;
				return (Constants.FIXEDPURCHASE_OPTION_ENABLED && (this.AddCartKbn == Constants.AddCartKbn.FixedPurchase));
			}
		}
		/// <summary>Is Subscription Box </summary>
		public bool IsSubscriptionBox
		{
			get { return (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && ((this.AddCartKbn == Constants.AddCartKbn.SubscriptionBox) || string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false)); }
		}
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId { get; private set; }
		/// <summary>ギフト購入設定</summary>
		public bool IsGift
		{
			get { return (Constants.GIFTORDER_OPTION_ENABLED && (this.AddCartKbn == Constants.AddCartKbn.GiftOrder)); }
		}
		/// <summary>商品セットアイテムフラグ</summary>
		public bool IsSetItem { get; private set; }
		/// <summary>商品セット情報</summary>
		public CartProductSet ProductSet { get; private set; }
		/// <summary>商品セットアイテムNo（1～）</summary>
		public int ProductSetItemNo { get; private set; }
		/// <summary>商品セールID</summary>
		public string ProductSaleId { get; private set; }
		/// <summary>ブランドID</summary>
		public string BrandId { get; private set; }
		/// <summary>ブランドID2</summary>
		public string BrandId2 { get; private set; }
		/// <summary>ブランドID3</summary>
		public string BrandId3 { get; private set; }
		/// <summary>ブランドID4</summary>
		public string BrandId4 { get; private set; }
		/// <summary>ブランドID5</summary>
		public string BrandId5 { get; private set; }
		/// <summary>商品付帯情報</summary>
		public ProductOptionSettingList ProductOptionSettingList { get; set; }
		/// <summary>DB更新設定</summary>
		public bool UpdateCartDb { get; set; }
		/// <summary>配送料無料複数個フラグ</summary>
		public string IsPluralShippingPriceFree { get; protected set; }
		/// <summary>デジタルコンテンツフラグ</summary>
		public bool IsDigitalContents { get; private set; }
		/// <summary>ダウンロードURL</summary>
		public string DownloadUrl { get; private set; }
		/// <summary>DC対応: シリアルキー一覧（メールテンプレート用）</summary>
		/// <remarks>注文処理ExecOrder()途中で格納されます。</remarks>
		public List<string> SerialKeys { get; private set; }
		/// <summary>DC対応: シリアルキーステータス（メールテンプレート用）</summary>
		/// <remarks>注文処理ExecOrder()途中で格納されます。</remarks>
		public bool IsDelivered { get; set; }
		/// <summary>商品タグ情報</summary>
		public Hashtable ProductTag { get; set; }
		/// <summary>商品連携ID</summary>
		public List<string> CooperationId { get; private set; }
		/// <summary>セットプロモーション割り当て数（セットプロモーション枝番, 数量）</summary>
		public Dictionary<int, int> QuantityAllocatedToSet
		{
			get { return m_quantityAllocatedToSet; }
		}
		private Dictionary<int, int> m_quantityAllocatedToSet = new Dictionary<int, int>();
		/// <summary>セットプロモーションに割り当てられていない商品数</summary>
		public int QuantitiyUnallocatedToSet
		{
			get { return this.CountSingle - this.QuantityAllocatedToSet.Values.Sum(); }
		}
		/// <summary>ノベルティID</summary>
		public string NoveltyId
		{
			get { return StringUtility.ToEmpty(m_noveltyId); }
			set { m_noveltyId = value; }
		}
		private string m_noveltyId = null;
		/// <summary>ノベルティ?</summary>
		public bool IsNovelty
		{
			get { return (this.NoveltyId != ""); }
		}
		/// <summary>ノベルティ商品?</summary>
		public bool IsProductNovelty
		{
			get { return (Constants.NOVELTY_OPTION_ENABLED && this.NoveltyId != ""); }
		}
		/// <summary>レコメンドID</summary>
		public string RecommendId
		{
			get { return StringUtility.ToEmpty(m_recommendId); }
			set { m_recommendId = value; }
		}
		private string m_recommendId = null;
		/// <summary>キャンペーン（ノベルティ、クーポンなど）適用比較対象金額用商品小計(割引金額の按分処理適用後)</summary>
		public decimal PriceSubtotalAfterDistributionForCampaign { get; set; }
		/// <summary>利用不可決済ID</summary>
		public string[] LimitedPaymentIds { get; private set; }
		/// <summary>同梱商品明細表示フラグ</summary>
		public string BundleItemDisplayType { get; set; }
		/// <summary>商品区分</summary>
		public string ProductType { get; set; }
		/// <summary>商品同梱ID</summary>
		public string ProductBundleId { get; set; }
		/// <summary>同梱商品？</summary>
		public bool IsBundle
		{
			get { return (string.IsNullOrEmpty(this.ProductBundleId) == false); }
		}
		/// <summary>明細表示フラグ</summary>
		public string OrderHistoryDisplayType
		{
			get
			{
				return
					((string.IsNullOrEmpty(this.ProductBundleId) == false) && (this.BundleItemDisplayType == Constants.FLG_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE_INVALID))
						? Constants.FLG_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE_INVALID
						: Constants.FLG_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE_VALID;
			}
		}
		/// <summary>同梱設定適用優先順</summary>
		public int ApplyOrder { get; set; }
		/// <summary>重複適用フラグ</summary>
		public string MultipleApplyFlg { get; set; }
		/// <summary>定期会員割引率の商品適用あり判定</summary>
		public bool IsApplyFixedPurchaseMemberDiscount
		{
			get
			{
				return ((this.QuantityAllocatedToSet.Any() == false)
					&& (this.IsFixedPurchase == false)
					&& (this.IsNovelty == false));
			}
		}
		/// <summary>利用不可配送月間隔</summary>
		public string LimitedFixedPurchaseKbn1Setting { get; set; }
		/// <summary>利用不可配送日間隔</summary>
		public string LimitedFixedPurchaseKbn3Setting { get; set; }
		/// <summary>利用不可配送週間隔</summary>
		public string LimitedFixedPurchaseKbn4Setting { get; set; }
		/// <summary>Subscription Box Flag</summary>
		public string SubscriptionBoxFlag { get; set; }
		/// <summary>Subscription Box Course Id</summary>
		public string SubscriptionBoxCourseId { get; set; }
		/// <summary>レコメンド用商品ID</summary>
		public string RecommendProductId { get; set; }
		/// <summary>注文同梱有無</summary>
		public bool IsOrderCombine { set; get; }
		/// <summary>注文同梱元注文ID</summary>
		public string OrderCombineOrgOrderId { set; get; }
		/// <summary>注文同梱によって増えた数量(注文同梱子注文数量 セットプロモーションに割り当てられていない数量)</summary>
		public int AddedQuantitySingleByOrderCombine { set; get; }
		/// <summary>レコメンド商品か</summary>
		public bool IsRecommendItem
		{
			get { return (string.IsNullOrEmpty(this.RecommendId) == false); }
		}
		/// <summary>重量（g）</summary>
		public int Weight { get; set; }
		/// <summary>重量小計（g） 個数分の重量</summary>
		public int WeightSubTotal { get { return this.Weight * this.CountSingle; } }
		/// <summary>商品結合翻訳名</summary>
		public string ProductJointTranslationName
		{
			get
			{
				if (Constants.GLOBAL_OPTION_ENABLE == false) return this.ProductJointName;
				return GetProductJointTranslationName();
			}
		}
		/// <summary>言語コード</summary>
		public string LanguageCode { get; set; }
		/// <summary>言語ロケールID</summary>
		public string LanguageLocaleId { get; set; }
		/// <summary>バリエーションが存在するか</summary>
		public bool HasVariation { get; private set; }
		/// <summary>通常購入制限対象か</summary>
		public bool IsProductLimit { get; set; }
		/// <summary>定期購入制限対象か</summary>
		public bool IsFixedPurchaseProductLimit { get; set; }
		/// <summary>購入制限対象か</summary>
		public bool IsOrderLimitProduct
		{
			get { return this.IsProductLimit || this.IsFixedPurchaseProductLimit; }
		}
		/// <summary>定期購入解約可能回数</summary>
		public int FixedPurchaseCancelableCount { get; private set; }
		/// <summary>コンテンツログ</summary>
		public ContentsLogModel ContentsLog { get; private set; }
		/// <summary>定期購入2回目以降配送商品ID</summary>
		public string FixedPurchaseNextShippingProductId { get; set; }
		/// <summary>定期購入2回目以降配送商品バリエーションID</summary>
		public string FixedPurchaseNextShippingVariationId { get; set; }
		/// <summary>定期購入2回目以降配送商品注文個数</summary>
		public int FixedPurchaseNextShippingItemQuantity { get; set; }
		/// <summary>Next Shipping Item Fixed Purchase Kbn</summary>
		public string NextShippingItemFixedPurchaseKbn { get; set; }
		/// <summary>Next Shipping Item Fixed Purchase Setting</summary>
		public string NextShippingItemFixedPurchaseSetting { get; set; }
		/// <summary>商品サイズ係数</summary>
		public int ProductSizeFactor { get; set; }
		/// <summary>定期購入値引き設定(値)</summary>
		public decimal? FixedPurchaseDiscountValue { get; set; }
		/// <summary>定期購入値引き設定(種別)</summary>
		public string FixedPurchaseDiscountType { get; set; }
		/// <summary>定期購入割引対象商品か</summary>
		public bool IsFixedPurchaseDiscountItem
		{
			get { return (string.IsNullOrEmpty(this.FixedPurchaseDiscountType) == false); }
		}
		/// <summary>最小同時購入可能数</summary>
		public decimal ProductMinSellQuantity { get; set; }
		/// <summary>最大同時購入可能数と最小同時購入可能数が同じ値か</summary>
		public bool IsMaxAndMinSameValue
		{
			get { return (ProductMaxSellQuantity == ProductMinSellQuantity); }
		}
		/// <summary>明細金額（セットプロモーション枝番、割引後価格）</summary>
		public Dictionary<int, decimal> DiscountedPrice
		{
			get { return m_discountedPrice; }
		}
		private Dictionary<int, decimal> m_discountedPrice = new Dictionary<int, decimal>();
		/// <summary>セットプロモーションに割り当てられない明細金額（割引後価格）</summary>
		public decimal DiscountedPriceUnAllocatedToSet { get; set; }
		/// <summary>セール期間を表示するか</summary>
		public bool IsDispSaleTerm
		{
			get
			{
				return (string.IsNullOrEmpty(this.ProductSaleId) == false)
					&& Constants.CORRESPONDENCE_SPECIFIEDCOMMERCIALTRANSACTIONS_ENABLE;
			}
		}
		/// <summary>Is display product tag cart product message</summary>
		public bool IsDisplayProductTagCartProductMessage
		{
			get
			{
				var result = (string.IsNullOrEmpty(GetProductTag("tag_cart_product_message")) == false);
				return result;
			}
		}
		/// <summary>Product tag cart product message</summary>
		public string ProductTagCartProductMessage
		{
			get
			{
				var result = GetProductTag("tag_cart_product_message");
				return result;
			}
		}
		/// <summary>Product join name if product detail link is invalid</summary>
		public string ProductJoinNameIfProductDetailLinkIsInvalid
		{
			get
			{
				var result = this.IsProductDetailLinkValid()
					? this.ProductJointName
					: string.Empty;
				return result;
			}
		}
		/// <summary>最終更新者</summary>
		public string LastChanged { get; set; }
		/// <summary>次回配送日</summary>
		public DateTime NextShippingDate { get; set; }
		/// <summary>定期商品購入回数（※2回目以降の定期購入情報表示時のみに使う)</summary>
		public int? FixedPurchaseItemOrderCount { get; set; }
		/// <summary>オプション価格合計（商品単位)</summary>
		public decimal TotalOptionPrice
		{
			get
			{
				var result = Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED && (this.ProductOptionSettingList != null)
					? this.ProductOptionSettingList.SelectedOptionTotalPrice
					: 0m;
				return result;
			}
		}
		/// <summary>付帯情報の税額</summary>
		public decimal OptionPriceTax
		{
			get
			{
				return TaxCalculationUtility.GetTaxPrice(this.TotalOptionPrice, this.TaxRate, Constants.TAX_EXCLUDED_FRACTION_ROUNDING);
			}
		}
		/// <summary>付帯情報の価格を込めた商品価格</summary>
		public decimal PriceIncludedOptionPrice
		{
			get
			{
				return this.TotalOptionPrice + this.Price;
			}
		}
		/// <summary>税込付帯情報価格</summary>
		public decimal OptionPricePretax
		{
			get
			{
				return TaxCalculationUtility.GetPriceTaxIncluded(
					this.TotalOptionPrice,
					this.OptionPriceTax);
			}
		}
		/// <summary>定期商品購入回数（注文基準）（受注情報画面で入力された値、定期割引額、ポイント計算に使う）</summary>
		public string FixedPurchaseItemOrderCountInput { get; set; }
		/// <summary>会員ランクポイント付与率除外設定フラグ</summary>
		public string MemberRankPointExcludeFlg { get; set; }
		/// <summary>頒布会定額価格</summary>
		public decimal? SubscriptionBoxFixedAmount { get; set; }
		/// <summary>販売期間表示フラグ</summary>
		private string DisplaySellFlg { get; set; }
		/// <summary>販売期間表示か</summary>
		public bool IsDisplaySell
		{
			get { return (this.DisplaySellFlg == Constants.FLG_PRODUCT_DISPLAY_SELL_FLG_DISP) && Constants.CORRESPONDENCE_SPECIFIEDCOMMERCIALTRANSACTIONS_ENABLE; }
		}
		/// <summary>販売開始期間</summary>
		public string SellFrom { get; set; }
		/// <summary>販売終了期間</summary>
		public string SellTo { get; set; }
		/// <summary>店舗受取可能フラグ</summary>
		public string StorePickUpFlg { get; set; }
		/// <summary>配送料無料時の請求料金利用フラグ</summary>
		public string ExcludeFreeShippingFlg { get; set; }
		/// <summary>配送料無料適応外文言表示か</summary>
		public bool IsDisplayExcludeFreeShippingText
		{
			get { return Constants.FREE_SHIPPING_FEE_OPTION_ENABLED && this.ExcludeFreeShippingFlg == Constants.FLG_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG_VALID; }
		}
	}
}
