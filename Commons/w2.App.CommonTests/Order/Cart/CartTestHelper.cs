using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Moq;
using w2.App.Common.Order;
using w2.App.Common.User;
using w2.Domain;
using w2.Domain.MemberRank;
using w2.Domain.ProductSet;
using w2.Domain.SetPromotion;
using static w2.App.Common.Order.CartShipping;
using static w2.App.CommonTests.Order.Cart.CartObjectTests;
using w2.Domain.ShopShipping;
using Constants = w2.App.Common.Constants;

namespace w2.App.CommonTests.Order.Cart
{
	/// <summary>
	/// カートテストヘルパー
	/// </summary>
	public class CartTestHelper
	{
		public const string DEFAULT_USER_ID = "U001";

		/// <summary>
		/// カート作成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="isDigitalContentsOnly">デジタルコンテンツのみフラグ</param>
		/// <returns>カート</returns>
		public static CartObject CreateCart(string userId = DEFAULT_USER_ID, string memberRankId = "", bool isDigitalContentsOnly = false)
		{
			var cart = new CartObject(
				userId: userId,
				cartId: "C001",
				orderKbn: Constants.FLG_ORDER_ORDER_KBN_PC,
				shopId: Constants.CONST_DEFAULT_SHOP_ID,
				shippingType: "0",
				isDigitalContentsOnly: isDigitalContentsOnly,
				updateCartDb: false,
				memberRankId: memberRankId);

			var mock = new Mock<IMemberRankService>();
			mock.Setup(s => s.GetMemberRankList()).Returns(new MemberRankModel[0]);
			DomainFacade.Instance.MemberRankService = mock.Object;

			return cart;
		}

		/// <summary>
		/// カート作成（複数配送先）
		/// </summary>
		/// <param name="parameters">パラメータ</param>
		/// <returns>カート</returns>
		public static CartObject CreateMultipleShippingCart(CreateCartShippingParams[] parameters)
		{
			var cart = CartTestHelper.CreateCart();
			cart.Owner = CartTestHelper.CreateCartOwner();
			cart.Shippings.Clear();
			var productDictionary = new Dictionary<string, CartProduct>();
			var productParams = parameters.SelectMany(p => p.ProductParams);
			foreach (var param in productParams.GroupBy(p => p.Id))
			{
				var quantity = param.Sum(p => p.Count);
				var cartProduct = CartTestHelper.CreateCartProduct(
					productPrice: param.Min(p => p.ProductPrice),
					productCount: quantity,
					addCartKbn: Constants.AddCartKbn.GiftOrder);
				cart.Items.Add(cartProduct);
				productDictionary.Add(param.Key, cartProduct);
			}

			foreach (var shippingParam in parameters)
			{
				var cartShipping = new CartShipping(cart);
				cartShipping.UpdateShippingAddr(cart.Owner, true);
				foreach (var productCountsParam in shippingParam.ProductParams)
				{
					var cartProduct = productDictionary[productCountsParam.Id];
					cartShipping.ProductCounts.Add(new ProductCount(cartProduct, productCountsParam.Count));
				}

				cart.Shippings.Add(cartShipping);
			}

			return cart;
		}

		/// <summary>
		/// カート注文者作成
		/// </summary>
		/// <returns>カート注文者</returns>
		public static CartOwner CreateCartOwner(
			string zip = "104-0061",
			string addr1 = "東京都",
			string addr2 = "中央区銀座４丁目",
			string addr3 = "１４番１１号",
			string addr4 = "七十七銀座ビル　7階",
			string addrCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
			string addrCountryName = "")
		{
			var zipCode = new ZipCode(zip);
			var owner = new CartOwner(
				Constants.FLG_ORDEROWNER_OWNER_KBN_PC_GUEST,
				"ｗ２太郎",
				"ｗ２",
				"太郎",
				"だぶるつたろう",
				"だぶるつ",
				"たろう",
				"bh@w2.xyz",
				"",
				zip : zipCode.Zip,
				zip1 : zipCode.Zip1,
				zip2 : zipCode.Zip2,
				addr1 : addr1,
				addr2 : addr2,
				addr3 : addr3,
				addr4 : addr4,
				addr5 : "",
				addrCountryIsoCode: addrCountryIsoCode,
				addrCountryName : addrCountryName,
				"ｗ２ソリューション株式会社",
				"ＰＳＤ",
				"03-5148-9633",
				"03",
				"5148",
				"9633",
				"010-1234-5678",
				"010",
				"1234",
				"5678",
				true,
				Constants.FLG_ORDEROWNER_OWNER_SEX_MALE,
				DateTime.Parse("2005/9/2").Date,
				"",
				"",
				"",
				"",
				"");
			return owner;
		}

		/// <summary>
		/// セットプロモーション情報を作成
		/// </summary>
		/// <param name="cart">商品価格</param>
		/// <param name="cartProductList">割り当て商品リスト</param>
		/// <param name="allocateToSetPromotionQuantity">セットプロモーション割り当て個数</param>
		/// <param name="productDiscountKbn">商品割引区分</param>
		/// <param name="discountAmount">割引設定値</param>
		/// <param name="isDiscountTypeProductDiscount">商品割引フラグ</param>
		/// <param name="isDiscountTypeShippingChargeFree">配送料無料フラグ</param>
		/// <returns>カートセットプロモーション情報</returns>
		public static CartSetPromotion CreateCartSetPromotion(
			CartObject cart,
			List<CartProduct> cartProductList,
			int allocateToSetPromotionQuantity,
			string productDiscountKbn,
			decimal? discountAmount,
			bool isDiscountTypeProductDiscount = false,
			bool isDiscountTypeShippingChargeFree = false)
		{
			var nextCartSetPromotionNo = cart.SetPromotions.Items.Count + 1;

			var setpromotionItemList = new List<CartSetPromotion.Item>();
			foreach (var cartProduct in cartProductList)
			{
				setpromotionItemList.Add(new CartSetPromotion.Item(cartProduct, allocateToSetPromotionQuantity));
				cartProduct.QuantityAllocatedToSet.Add(nextCartSetPromotionNo, allocateToSetPromotionQuantity);
			}

			var setpromotionmodel = new SetPromotionModel
			{
				ProductDiscountKbn = productDiscountKbn,
				ProductDiscountSetting = discountAmount,
				IsDiscountTypeProductDiscount = isDiscountTypeProductDiscount,
			};

			var cartsetPromotion = new CartSetPromotion(cart, setpromotionmodel, setpromotionItemList);
			cartsetPromotion.SetCount = 1;
			cartsetPromotion.IsDiscountTypeShippingChargeFree = isDiscountTypeShippingChargeFree;
			return cartsetPromotion;
		}

		/// <summary>
		/// カート商品作成
		/// </summary>
		/// <param name="productPrice">商品価格</param>
		/// <param name="productCount">商品数量</param>
		/// <param name="taxRate">消費税率</param>
		/// <param name="memberRankDiscountFlg">会員ランク割引フラグ</param>
		/// <param name="addCartKbn">カート追加区分</param>
		/// <param name="isDigitalContents">デジタルコンテンツフラグ</param>
		/// <param name="excludeFreeShippingFlg">配送料無料複数個フラグ</param>
		/// <param name="isPluralShippingPriceFree">配送料無料時の請求料金利用フラグ</param>
		/// <param name="weight">重さ</param>
		/// <param name="shippingSizeKbn">配送サイズ区分</param>
		/// <returns>カート商品</returns>
		public static CartProduct CreateCartProduct(
			decimal productPrice = 1000m,
			int productCount = 1,
			decimal taxRate = 10m,
			bool memberRankDiscountFlg = false,
			Constants.AddCartKbn addCartKbn = Constants.AddCartKbn.Normal,
			bool isDigitalContents = false,
			string excludeFreeShippingFlg = "",
			string isPluralShippingPriceFree = "",
			string weight = "0",
			string shippingSizeKbn = "")
		{
			var drv = CreateProductDataRowView(productPrice, taxRate, memberRankDiscountFlg);
			drv[Constants.FIELD_PRODUCT_SHOP_ID] = "0";
			drv[Constants.FIELD_PRODUCT_PRODUCT_ID] = "test001";
			drv[Constants.FIELD_PRODUCT_VARIATION_ID] = "test001001";
			drv[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG]
				= isDigitalContents
					? Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID
					: Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_INVALID;
			drv[Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG] = isPluralShippingPriceFree;
			drv[Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG] = excludeFreeShippingFlg;
			drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM] = weight;
			drv[Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN] = shippingSizeKbn;

			var mock = new Mock<Domain.ProductTag.IProductTagService>();
			mock.Setup(s => s.GetProductTag(It.IsAny<string>())).Returns(new Domain.ProductTag.ProductTagModel
			{
				ProductId = (string)drv[Constants.FIELD_PRODUCT_PRODUCT_ID],
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				LastChanged = "テスト用",
			});
			DomainFacade.Instance.ProductTagService = mock.Object;

			var cartProduct = new CartProduct(
				drv,
				addCartKbn,
				string.Empty,
				productCount,
				false);
			return cartProduct;
		}

		/// <summary>
		/// カート商品セット作成
		/// </summary>
		/// <param name="maxSellQuantity">販売可能セット数</param>
		/// <param name="productSetCount">セット数</param>
		/// <param name="productSetNo">商品セットNo</param>
		/// <returns>カート商品セット</returns>
		public static CartProductSet CreateCartProductSet(
			int maxSellQuantity = 100,
			int productSetCount = 1,
			int productSetNo = 1)
		{
			var dt = new DataTable();
			dt.Columns.Add(Constants.FIELD_PRODUCTSET_SHOP_ID);
			dt.Columns.Add(Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID);
			dt.Columns.Add(Constants.FIELD_PRODUCTSET_PRODUCT_SET_NAME);
			dt.Columns.Add(Constants.FIELD_PRODUCTSET_MAX_SELL_QUANTITY, typeof(int));
			var dr = dt.NewRow();
			dr[Constants.FIELD_PRODUCTSET_SHOP_ID] = "0";
			dr[Constants.FIELD_PRODUCTSET_PRODUCT_SET_ID] = "set1";
			dr[Constants.FIELD_PRODUCTSET_PRODUCT_SET_NAME] = "商品セット1";
			dr[Constants.FIELD_PRODUCTSET_MAX_SELL_QUANTITY] = maxSellQuantity;
			dt.Rows.Add(dr);
			var drv = dt.DefaultView[dt.Rows.IndexOf(dr)];
			var cartProductSet = new CartProductSet(drv, productSetCount, productSetNo, false);
			return cartProductSet;
		}

		/// <summary>
		/// カート商品セット作成
		/// </summary>
		/// <param name="productSet">カート商品セット</param>
		/// <param name="setItemPrice">セット商品価格</param>
		/// <param name="productCount">商品個数</param>
		/// <returns>カート商品セット</returns>
		public static void CreateAndAddProductToProductSet(
			CartProductSet productSet,
			decimal setItemPrice = 1000m,
			int productCount = 1)
		{
			var mock = new Mock<IProductSetService>();
			mock.Setup(s => s.GetProductSetItem(
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string>())).Returns(new ProductSetItemModel
				{
					SetitemPrice = setItemPrice
				});
			DomainFacade.Instance.ProductSetService = mock.Object;

			productSet.AddProductVirtual(CreateProductDataRowView(), productCount);
		}

		/// <summary>
		/// 商品DataRowView生成
		/// </summary>
		/// <param name="productPrice">商品価格</param>
		/// <param name="taxRate">消費税率</param>
		/// <param name="memberRankDiscountFlg">会員ランク割引フラグ</param>
		/// <returns>商品DataRowView</returns>
		public static DataRowView CreateProductDataRowView(
			decimal productPrice = 1000m,
			decimal taxRate = 10m,
			bool memberRankDiscountFlg = false)
		{
			var dt = new DataTable();
			dt.Columns.Add(Constants.FIELD_PRODUCT_SHOP_ID);
			dt.Columns.Add(Constants.FIELD_PRODUCT_PRODUCT_ID);
			dt.Columns.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_ID);
			dt.Columns.Add(Constants.FIELD_PRODUCT_SUPPLIER_ID);
			dt.Columns.Add(Constants.FIELD_PRODUCT_NAME);
			dt.Columns.Add(Constants.FIELD_PRODUCT_NAME_KANA);
			dt.Columns.Add(Constants.FIELD_PRODUCT_OUTLINE_KBN);
			dt.Columns.Add(Constants.FIELD_PRODUCT_OUTLINE);
			dt.Columns.Add(Constants.FIELD_PRODUCT_DISPLAY_KBN);
			dt.Columns.Add(Constants.FIELD_PRODUCT_CATEGORY_ID1);
			dt.Columns.Add(Constants.FIELD_PRODUCT_CATEGORY_ID2);
			dt.Columns.Add(Constants.FIELD_PRODUCT_CATEGORY_ID3);
			dt.Columns.Add(Constants.FIELD_PRODUCT_CATEGORY_ID4);
			dt.Columns.Add(Constants.FIELD_PRODUCT_CATEGORY_ID5);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_FLG1);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_FLG2);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_FLG3);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_FLG4);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_FLG5);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_FLG6);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_FLG7);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_FLG8);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_FLG9);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_FLG10);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_TERM_END1);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_TERM_END2);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_TERM_END3);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_TERM_END4);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_TERM_END5);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_TERM_END6);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_TERM_END7);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_TERM_END8);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_TERM_END9);
			dt.Columns.Add(Constants.FIELD_PRODUCT_ICON_TERM_END10);
			dt.Columns.Add(Constants.FIELD_PRODUCTVARIATION_PRICE, typeof(decimal));
			dt.Columns.Add(Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE, typeof(decimal));
			dt.Columns.Add(Constants.FIELD_PRODUCT_POINT_KBN1);
			dt.Columns.Add(Constants.FIELD_PRODUCT_POINT1, typeof(decimal));
			dt.Columns.Add(Constants.FIELD_PRODUCT_POINT_KBN2);
			dt.Columns.Add(Constants.FIELD_PRODUCT_POINT2, typeof(decimal));
			dt.Columns.Add(Constants.FIELD_PRODUCTTAXCATEGORY_TAX_RATE, typeof(decimal));
			dt.Columns.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1);
			dt.Columns.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2);
			dt.Columns.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3);
			dt.Columns.Add(Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY, typeof(int));
			dt.Columns.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD);
			dt.Columns.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE);
			dt.Columns.Add(Constants.FIELD_PRODUCT_SHIPPING_TYPE);
			dt.Columns.Add(Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN);
			dt.Columns.Add(Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN);
			dt.Columns.Add(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE);
			dt.Columns.Add(Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE_MOBILE);
			dt.Columns.Add(Constants.FIELD_PRODUCT_BRAND_ID1);
			dt.Columns.Add(Constants.FIELD_PRODUCT_BRAND_ID2);
			dt.Columns.Add(Constants.FIELD_PRODUCT_BRAND_ID3);
			dt.Columns.Add(Constants.FIELD_PRODUCT_BRAND_ID4);
			dt.Columns.Add(Constants.FIELD_PRODUCT_BRAND_ID5);
			dt.Columns.Add(Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG);
			dt.Columns.Add(Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG);
			dt.Columns.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL);
			foreach (var i in Enumerable.Range(1, Constants.COOPERATION_ID_COLUMNS_COUNT))
			{
				dt.Columns.Add("variation_" + Constants.HASH_KEY_COOPERATION_ID + i);
			}
			dt.Columns.Add(Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG);
			dt.Columns.Add(Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS);
			dt.Columns.Add(Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE);
			dt.Columns.Add(Constants.FIELD_PRODUCT_PRODUCT_TYPE);
			dt.Columns.Add(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING);
			dt.Columns.Add(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING);
			dt.Columns.Add(Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING);
			dt.Columns.Add(Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID);
			dt.Columns.Add(Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG);
			dt.Columns.Add(Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT, typeof(int));
			dt.Columns.Add(Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR, typeof(int));
			dt.Columns.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM, typeof(int));
			dt.Columns.Add(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID);
			dt.Columns.Add(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID);
			dt.Columns.Add(Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY, typeof(int));
			dt.Columns.Add(Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN);
			dt.Columns.Add(Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING);
			dt.Columns.Add(Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE, typeof(decimal));
			dt.Columns.Add(Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG);
			dt.Columns.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE, typeof(decimal));
			dt.Columns.Add(Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE, typeof(decimal));
			dt.Columns.Add(Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG);
			dt.Columns.Add(Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG);
			dt.Columns.Add(Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG);
			dt.Columns.Add(Constants.FIELD_PRODUCT_SELL_FROM);
			dt.Columns.Add(Constants.FIELD_PRODUCT_SELL_TO);
			dt.Columns.Add(Constants.FIELD_PRODUCT_STOREPICKUP_FLG);
			dt.Columns.Add(Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG);
			dt.Columns.Add(Constants.FIELD_PRODUCTSTOCK_STOCK, typeof(int));

			var dr = dt.NewRow();
			dr[Constants.FIELD_PRODUCT_SHOP_ID] = string.Empty;
			dr[Constants.FIELD_PRODUCT_PRODUCT_ID] = string.Empty;
			dr[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] = string.Empty;
			dr[Constants.FIELD_PRODUCT_SUPPLIER_ID] = string.Empty;
			dr[Constants.FIELD_PRODUCT_NAME] = string.Empty;
			dr[Constants.FIELD_PRODUCT_NAME_KANA] = string.Empty;
			dr[Constants.FIELD_PRODUCT_OUTLINE_KBN] = string.Empty;
			dr[Constants.FIELD_PRODUCT_OUTLINE] = string.Empty;
			dr[Constants.FIELD_PRODUCT_DISPLAY_KBN] = string.Empty;
			dr[Constants.FIELD_PRODUCT_CATEGORY_ID1] = string.Empty;
			dr[Constants.FIELD_PRODUCT_CATEGORY_ID2] = string.Empty;
			dr[Constants.FIELD_PRODUCT_CATEGORY_ID3] = string.Empty;
			dr[Constants.FIELD_PRODUCT_CATEGORY_ID4] = string.Empty;
			dr[Constants.FIELD_PRODUCT_CATEGORY_ID5] = string.Empty;
			dr[Constants.FIELD_PRODUCT_ICON_FLG1] = string.Empty;
			dr[Constants.FIELD_PRODUCT_ICON_FLG2] = string.Empty;
			dr[Constants.FIELD_PRODUCT_ICON_FLG3] = string.Empty;
			dr[Constants.FIELD_PRODUCT_ICON_FLG4] = string.Empty;
			dr[Constants.FIELD_PRODUCT_ICON_FLG5] = string.Empty;
			dr[Constants.FIELD_PRODUCT_ICON_FLG6] = string.Empty;
			dr[Constants.FIELD_PRODUCT_ICON_FLG7] = string.Empty;
			dr[Constants.FIELD_PRODUCT_ICON_FLG8] = string.Empty;
			dr[Constants.FIELD_PRODUCT_ICON_FLG9] = string.Empty;
			dr[Constants.FIELD_PRODUCT_ICON_FLG10] = string.Empty;
			dr[Constants.FIELD_PRODUCTVARIATION_PRICE] = productPrice;
			dr[Constants.FIELD_PRODUCT_POINT_KBN1] = string.Empty;
			dr[Constants.FIELD_PRODUCT_POINT1] = 10m;
			dr[Constants.FIELD_PRODUCT_POINT_KBN2] = string.Empty;
			dr[Constants.FIELD_PRODUCT_POINT2] = 10m;
			dr[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_RATE] = taxRate;
			dr[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1] = string.Empty;
			dr[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2] = string.Empty;
			dr[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3] = string.Empty;
			dr[Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY] = 0;
			dr[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] = string.Empty;
			dr[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_MOBILE] = string.Empty;
			dr[Constants.FIELD_PRODUCT_SHIPPING_TYPE] = string.Empty;
			dr[Constants.FIELD_PRODUCT_SHIPPING_SIZE_KBN] = string.Empty;
			dr[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] = string.Empty;
			dr[Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE] = string.Empty;
			dr[Constants.FIELD_PRODUCT_RETURN_EXCHANGE_MESSAGE_MOBILE] = string.Empty;
			dr[Constants.FIELD_PRODUCT_BRAND_ID1] = string.Empty;
			dr[Constants.FIELD_PRODUCT_BRAND_ID2] = string.Empty;
			dr[Constants.FIELD_PRODUCT_BRAND_ID3] = string.Empty;
			dr[Constants.FIELD_PRODUCT_BRAND_ID4] = string.Empty;
			dr[Constants.FIELD_PRODUCT_BRAND_ID5] = string.Empty;
			dr[Constants.FIELD_PRODUCT_PLURAL_SHIPPING_PRICE_FREE_FLG] = string.Empty;
			dr[Constants.FIELD_PRODUCT_DIGITAL_CONTENTS_FLG] = string.Empty;
			dr[Constants.FIELD_PRODUCTVARIATION_VARIATION_DOWNLOAD_URL] = string.Empty;
			foreach (var i in Enumerable.Range(1, Constants.COOPERATION_ID_COLUMNS_COUNT))
			{
				dr["variation_" + Constants.HASH_KEY_COOPERATION_ID + i] = string.Empty;
			}
			dr[Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG] = memberRankDiscountFlg
				? Constants.FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_VALID
				: Constants.FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_INVALID;
			dr[Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS] = string.Empty;
			dr[Constants.FIELD_PRODUCT_BUNDLE_ITEM_DISPLAY_TYPE] = string.Empty;
			dr[Constants.FIELD_PRODUCT_PRODUCT_TYPE] = string.Empty;
			dr[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING] = string.Empty;
			dr[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING] = string.Empty;
			dr[Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING] = string.Empty;
			dr[Constants.FIELD_PRODUCT_RECOMMEND_PRODUCT_ID] = string.Empty;
			dr[Constants.FIELD_PRODUCT_CHECK_PRODUCT_ORDER_LIMIT_FLG] = string.Empty;
			dr[Constants.FIELD_PRODUCT_FIXED_PURCHASE_CANCELABLE_COUNT] = 0;
			dr[Constants.FIELD_PRODUCT_PRODUCT_SIZE_FACTOR] = 0;
			dr[Constants.FIELD_PRODUCTVARIATION_VARIATION_WEIGHT_GRAM] = 0;
			dr[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_ID] = string.Empty;
			dr[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_VARIATION_ID] = string.Empty;
			dr[Constants.FIELD_PRODUCT_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY] = 0;
			dr[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_KBN] = string.Empty;
			dr[Constants.FIELD_PRODUCT_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING] = string.Empty;
			dr[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE] = DBNull.Value;
			dr[Constants.FIELD_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG] = Constants.FLG_PRODUCT_CHECK_FIXED_PRODUCT_ORDER_LIMIT_FLG_VALID;
			dr[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] = DBNull.Value;
			dr[Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE] = DBNull.Value;
			dr[Constants.FIELD_PRODUCT_MEMBER_RANK_POINT_EXCLUDE_FLG] = string.Empty;
			dr[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG] = string.Empty;
			dr[Constants.FIELD_PRODUCT_DISPLAY_SELL_FLG] = string.Empty;
			dr[Constants.FIELD_PRODUCT_SELL_FROM] = string.Empty;
			dr[Constants.FIELD_PRODUCT_SELL_TO] = string.Empty;
			dr[Constants.FIELD_PRODUCT_STOREPICKUP_FLG] = string.Empty;
			dr[Constants.FIELD_PRODUCT_EXCLUDE_FREE_SHIPPING_FLG] = string.Empty;
			dr[Constants.FIELD_PRODUCTSTOCK_STOCK] = 0;

			dt.Rows.Add(dr);
			var drv = dt.DefaultView[dt.Rows.IndexOf(dr)];
			return drv;
		}

		/// <summary>
		/// カート決済作成
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="priceExchange">決済手数料</param>
		/// <returns>カート決済</returns>
		public static CartPayment CreateCartPayment(
		string paymentId = Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
		decimal priceExchange = 1000m)
		{
			return new CartPayment
			{
				PaymentId = paymentId,
				PriceExchange = priceExchange,
			};
		}

		/// <summary>
		/// 店舗種別モデル取得する
		/// </summary>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="sizeMailShippingPrice">メール便配送料</param>
		/// <param name="sizeLShippingPrice">Lサイズ配送料</param>
		/// <returns>店舗種別モデル</returns>
		public static ShopShippingModel GetShopShippingModel(
			string shippingId = "0",
			string deliveryCompanyId = "TEST",
			decimal sizeMailShippingPrice = 10m,
			decimal sizeLShippingPrice = 100m)
		{
			return new ShopShippingModel
			{
				ShippingId = shippingId,
				CompanyList = new ShopShippingCompanyModel[]
				{
					new ShopShippingCompanyModel
					{
						DeliveryCompanyId = deliveryCompanyId,
						ShippingKbn = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS,
						DefaultDeliveryCompany = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_VALID,
					},
					new ShopShippingCompanyModel
					{
						DeliveryCompanyId = deliveryCompanyId,
						ShippingKbn = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL,
						DefaultDeliveryCompany = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_VALID,
					},
				},
				ZoneList = new ShopShippingZoneModel[]
				{
					new ShopShippingZoneModel()
					{
						DeliveryCompanyId = deliveryCompanyId,
						SizeMailShippingPrice = sizeMailShippingPrice,
						SizeLShippingPrice = sizeLShippingPrice,
					}
				},
				CompanyPostageSettings = new ShippingDeliveryPostageModel[]
				{
					new ShippingDeliveryPostageModel()
					{
						DeliveryCompanyId = deliveryCompanyId,
					}
				},
			};
		}
	}
}
