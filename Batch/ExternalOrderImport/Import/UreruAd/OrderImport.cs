/*
=========================================================================================================
  Module      : つくーるAPI連携：注文情報登録 (OrderImport.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.ShopShipping;
using w2.Domain.UserCreditCard;
using w2.App.Common;
using w2.App.Common.Mail;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Register;
using w2.Commerce.Batch.ExternalOrderImport.Entity;
using w2.Domain.ProductTaxCategory;
using w2.Domain.SubscriptionBox;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.Commerce.Batch.ExternalOrderImport.Import.UreruAd
{
	/// <summary>
	/// つくーるAPI連携：注文情報登録
	/// </summary>
	public class OrderImport : UreruAdImportBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseData">レスポンスデータ</param>
		/// <param name="accessor">SQLアクセサ</param>
		public OrderImport(UreruAdResponseDataItem responseData, SqlAccessor accessor)
			: base(responseData, accessor)
		{
			CreateCartObject();
			this.UserPoint = new UserPointImport(responseData, accessor);
			this.FixedPurchase = new FixedPurchaseImport(responseData, accessor);
		}

		/// <summary>
		/// 登録
		/// </summary>
		public override void Import()
		{
			this.Order = CreateImportData();
			if (this.ResponseData.Cart.Items.Any())
			{
				// 商品同梱
				using (var bundler = new ProductBundler(
					new List<CartObject>() { this.ResponseData.Cart },
					this.ResponseData.User.UserId,
					this.Order.AdvcodeFirst,
					this.Order.AdvcodeNew))
				{
					var bundledItemList = bundler.CartList.First().Items
						.Where(item => string.IsNullOrEmpty(item.ProductBundleId) == false)
						.Select((product, index) => new OrderItemModel
						{
							OrderId = this.Order.OrderId,
							OrderItemNo = this.Order.Items.Count() + 1 + index++,
							ShopId = Constants.CONST_DEFAULT_SHOP_ID,
							ProductId = product.ProductId,
							VariationId = product.VariationId,
							ProductName = product.ProductName,
							ProductPrice = product.Price,
							ProductPriceOrg = product.PriceOrg,
							ProductTaxRate = product.TaxRate,
							ProductPricePretax = product.PricePretax,
							ItemQuantity = product.Count,
							ItemQuantitySingle = product.CountSingle,
							ItemPrice = product.Price * product.Count,
							ItemPriceSingle = product.Price * product.CountSingle,
							ProductBundleId = product.ProductBundleId,
							ItemPriceTax = TaxCalculationUtility.GetTaxPrice(product.Price, product.TaxRate, Constants.TAX_EXCLUDED_FRACTION_ROUNDING) * product.Count,
							FixedPurchaseProductFlg = Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
							CooperationId1 = product.CooperationId[0],
							CooperationId2 = product.CooperationId[1],
							CooperationId3 = product.CooperationId[2],
							CooperationId4 = product.CooperationId[3],
							CooperationId5 = product.CooperationId[4],
							CooperationId6 = product.CooperationId[5],
							CooperationId7 = product.CooperationId[6],
							CooperationId8 = product.CooperationId[7],
							CooperationId9 = product.CooperationId[8],
							CooperationId10 = product.CooperationId[9],
							ShippingSizeKbn = product.ShippingSizeKbn,
							BundleItemDisplayType = product.BundleItemDisplayType,
						})
						.ToList();
					bundledItemList.AddRange(this.Order.Items);
					this.Order.Items = bundledItemList.ToArray();
				}
			}
			// 商品同梱
			this.ResponseData.Cart = new ProductBundler(
				new List<CartObject>()
				{
					this.ResponseData.Cart
				},
				this.ResponseData.User.UserId,
				this.ResponseData.User.AdvcodeFirst,
				this.ResponseData.GetAdvCode()).CartList.First();

			this.Order.OrderItemCount = this.Order.Items.Count();
			this.Order.OrderProductCount = this.Order.Items.Sum(item => item.ItemQuantity);
			this.Order.OrderPriceSubtotal = this.Order.SubscriptionBoxFixedAmount ?? this.Order.Items.Sum(item => item.ItemPrice);

			if (IsSubscriptionBoxFixedAmount())
			{
				var taxRate = GetSubscriptionBoxTaxRate();
				this.Order.OrderPriceSubtotalTax = (taxRate != null)
					? TaxCalculationUtility.GetTaxPrice(
						(this.Order.SubscriptionBoxFixedAmount.GetValueOrDefault()),
						taxRate.GetValueOrDefault(),
						Constants.TAX_EXCLUDED_FRACTION_ROUNDING)
					: 0m;
			}
			else
			{
				this.Order.OrderPriceSubtotalTax = this.Order.Items.Sum(item => item.ItemPriceTax);
			}
			CreatePriceByTaxRate();

			var service = new OrderService();
			service.InsertOrder(this.Order, UpdateHistoryAction.DoNotInsert, this.Accessor);

			if ((this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				|| (this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				|| (this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2))
			{
				service.UpdateExternalPaymentInfoForAuthSuccess(
				this.Order.OrderId,
				Constants.FLG_LASTCHANGED_BATCH,
				UpdateHistoryAction.DoNotInsert,
				this.Accessor);
			}

			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				// ポイントOP利用時はOrderCommon側で注文ID,ユーザーIDをセット
				this.UserPoint.Import();
			}
			else
			{
				this.ResponseData.Cart.OrderId = this.ResponseData.OrderId;
				this.ResponseData.Cart.OrderUserId = this.ResponseData.User.UserId;
			}

			if (this.ResponseData.IsFixedPurchase)
			{
				service.UpdateFixedPurchaseIdAndFixedPurchaseOrderCount(
					this.Order.OrderId,
					this.Order.FixedPurchaseId,
					this.Order.FixedPurchaseOrderCount.Value,
					Constants.FLG_LASTCHANGED_BATCH,
					UpdateHistoryAction.DoNotInsert,
					this.Accessor);
				this.ResponseData.FixedPurchaseItemList = this.Order.Items
					.Where(item => item.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON)
					.Select(item =>
						new FixedPurchaseItem
						{
							ProductId = item.ProductId,
							VariationId = item.VariationId,
							SupplierId = item.SupplierId,
							ItemQuantity = item.ItemQuantity,
							ItemQuantitySingle = item.ItemQuantitySingle,
							ItemPrice = item.ItemPrice,
							ItemPriceSingle = item.ItemPriceSingle
						}).ToList();
				this.FixedPurchase.Import();

				// 定期継続分析登録
				var orderForAnalysis = new Hashtable
				{
					{ Constants.FIELD_ORDER_FIXED_PURCHASE_ID, this.Order.FixedPurchaseId },
					{ Constants.FIELD_ORDER_ORDER_ID, this.Order.OrderId }
				};
				OrderPreorderRegister.InsertFixedPurchaseRepeatAnalysis(orderForAnalysis, this.ResponseData.Cart, Constants.FLG_LASTCHANGED_BATCH, this.Accessor);
			}
			// リアルタイム累計購入回数更新処理
			var order = new Hashtable
			{
				{Constants.FIELD_ORDER_USER_ID, this.Order.UserId},
				{Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME, this.Order.OrderCountOrder - 1},
			};
			OrderCommon.UpdateRealTimeOrderCount(order, Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_ORDER, this.Accessor);

			// 更新履歴登録
			new UpdateHistoryService().InsertForOrder(this.Order.OrderId, Constants.FLG_LASTCHANGED_BATCH, this.Accessor);
		}

		/// <summary>
		/// インポートデータ生成
		/// </summary>
		/// <returns>注文情報</returns>
		private OrderModel CreateImportData()
		{
			this.ResponseData.OrderId = OrderCommon.CreateOrderId(Constants.CONST_DEFAULT_SHOP_ID);
			this.ResponseData.FixedPurchaseId = this.ResponseData.IsFixedPurchase
					? OrderCommon.CreateFixedPurchaseId(Constants.CONST_DEFAULT_SHOP_ID)
					: string.Empty;
			var paymentKbn = ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, this.ResponseData.PaymentMethod);
			// リアルタイム累計購入回数取得
			var user = new UserService().Get(this.ResponseData.User.UserId, this.Accessor);
			var model = new OrderModel
			{
				OrderId = this.ResponseData.OrderId,
				UserId = this.ResponseData.User.UserId,
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				OrderKbn = ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN, this.ResponseData.Type.ToUpper()),
				MallId = Constants.FLG_USER_MALL_ID_URERU_AD,
				OrderPaymentKbn = paymentKbn,
				OrderStatus = (this.ResponseData.ExternalImportStatus == Constants.FLG_ORDER_EXTERNAL_IMPORT_STATUS_SUCCESS && paymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					? Constants.FLG_ORDER_ORDER_STATUS_ORDERED
					: Constants.FLG_ORDER_ORDER_STATUS_TEMP,
				OrderDate = this.ResponseData.Created,
				OrderStockreservedStatus = Constants.FLG_ORDER_ORDER_STOCKRESERVED_STATUS_UNRESERVED,
				OrderShippedStatus = Constants.FLG_ORDER_ORDER_SHIPPED_STATUS_UNSHIPPED,
				OrderItemCount = ((string.IsNullOrEmpty(this.ResponseData.LandingProductCode) ? 0 : 1) + (string.IsNullOrEmpty(this.ResponseData.UpsellProductCode) ? 0 : 1)),
				OrderProductCount = (this.ResponseData.LandingProductQty.GetValueOrDefault(0) + this.ResponseData.UpsellProductQty.GetValueOrDefault(0)),
				OrderPriceSubtotal = this.ResponseData.ProductTotalInc.GetValueOrDefault(0),
				OrderPriceRegulation = this.ResponseData.Discount.GetValueOrDefault(0) * -1,
				OrderPriceShipping = this.ResponseData.ShippingCost.GetValueOrDefault(0),
				OrderPriceExchange = this.ResponseData.Commission.GetValueOrDefault(0),
				OrderPriceTotal = this.ResponseData.TotalInc.GetValueOrDefault(0),
				OrderPointAdd = Constants.W2MP_POINT_OPTION_ENABLED
					? (this.ResponseData.Cart.BuyPoint + this.ResponseData.Cart.FirstBuyPoint)
					: 0,
				CardInstruments = ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_CARD_INSTRUMENTS, this.ResponseData.PaymentMethod),
				CardKbn = string.Empty,
				CardTranId = this.ResponseData.GetCardTranId(),
				ShippingId = StringUtility.ToEmpty(this.ResponseData.ShippingId),
				FixedPurchaseId = this.ResponseData.FixedPurchaseId,
				AdvcodeFirst = this.ResponseData.User.AdvcodeFirst,
				AdvcodeNew = this.ResponseData.GetAdvCode(),
				CareerId = string.Empty,
				RemoteAddr = this.ResponseData.IpAddress,
				Memo = string.Empty,
				ManagementMemo = this.ResponseData.Note,
				DateCreated = this.ResponseData.Created.GetValueOrDefault(DateTime.Now),
				DateChanged = this.ResponseData.Created.GetValueOrDefault(DateTime.Now),
				LastChanged = Constants.FLG_LASTCHANGED_BATCH,
				MemberRankId = this.ResponseData.User.MemberRankId,
				UserAgent = this.ResponseData.UserAgent,
				OrderTaxRate = 0, // 使用しない
				CardInstallmentsCode = (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					? OrderCommon.GetCreditInstallmentsDefaultValue()
					: string.Empty,
				FixedPurchaseOrderCount = this.ResponseData.IsFixedPurchase
					? 1
					: (int?)null,
				PaymentOrderId = this.ResponseData.GetPaymentOrderId(paymentKbn),
				ExternalOrderId = this.ResponseData.Id,
				ExternalImportStatus = this.ResponseData.ExternalImportStatus,
				ExternalPaymentAuthDate = this.ResponseData.Created,
				Items = CreateOrderItems(),
				Owner = CreateOrderOwner(),
				Shippings = new[] { CreateOrderShipping() },
				LastAuthFlg = Constants.FLG_ORDER_LAST_AUTH_FLG_ON,
				LastBilledAmount = this.ResponseData.TotalInc.GetValueOrDefault(0),
				RelationMemo = string.Format("つくーる連携：商品割引{0}円", this.ResponseData.Discount.GetValueOrDefault(0)),
				ShippingTaxRate = Constants.CONST_SHIPPING_TAXRATE,
				PaymentTaxRate = Constants.CONST_PAYMENT_TAXRATE,
				OrderCountOrder = ((user != null) ? user.OrderCountOrderRealtime + 1 : 1), // リアルタイム累計購入回数取得
				InvoiceBundleFlg = OrderCommon.IsInvoiceBundleServiceUsable(paymentKbn)
					? Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON
					: Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF,
				SubscriptionBoxCourseId = this.ResponseData.GetSubscriptionBoxCourseId(),
				SubscriptionBoxFixedAmount = GetSubscriptionBoxFixedAmount(),
				OrderSubscriptionBoxOrderCount = string.IsNullOrEmpty(this.ResponseData.GetSubscriptionBoxCourseId()) ? 0 : 1,
				CreditBranchNo = (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					? this.ResponseData.CreditBranchNo
					: (int?)null,
			};
			if ((paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				&& (string.IsNullOrEmpty(this.ResponseData.NpTransactionId)))
			{
				model.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_TEMP;
			}

			model.OrderPriceSubtotal = IsSubscriptionBoxFixedAmount()
				? GetSubscriptionBoxFixedAmount().GetValueOrDefault(0)
				: model.Items.Sum(item => item.ItemPrice);

			if (IsSubscriptionBoxFixedAmount())
			{
				model.OrderPriceTotal = model.LastBilledAmount = GetSubscriptionBoxFixedAmount().GetValueOrDefault(0)
					+ this.ResponseData.ShippingCost.GetValueOrDefault(0)
					+ this.ResponseData.Commission.GetValueOrDefault(0);
			}

			return model;
		}

		/// <summary>
		/// 注文商品情報作成
		/// </summary>
		/// <returns>注文商品情報</returns>
		private OrderItemModel[] CreateOrderItems()
		{
			var landingPrice = this.ResponseData.LandingProductPriceInc.GetValueOrDefault(0);
			var landingQty = this.ResponseData.LandingProductQty.GetValueOrDefault(0);

			var landingProduct = CreateOrderItem(
				1,
				this.ResponseData.LandingProductCode,
				this.ResponseData.LandingProductName,
				this.ResponseData.LandingProduct,
				landingPrice,
				landingQty);

			var upsellPrice = this.ResponseData.UpsellProductPriceInc.GetValueOrDefault(0);
			var upsellQty = this.ResponseData.UpsellProductQty.GetValueOrDefault(0);
			var upsellProduct = CreateOrderItem(
				(landingProduct == null) ? 1 : 2,
				this.ResponseData.UpsellProductCode,
				this.ResponseData.UpsellProductName,
				this.ResponseData.UpsellProduct,
				upsellPrice,
				upsellQty);

			var products = new[] { landingProduct, upsellProduct }.Where(product => product != null);
			var orderItems = (products.Any())
				? products.ToArray()
				: new[] { CreateOrderItem(1, string.Empty, string.Empty, new ProductModel(), 0, 0) };
			return orderItems;
		}

		/// <summary>
		/// 注文商品情報作成
		/// </summary>
		/// <param name="orderItemNo">注文商品枝番</param>
		/// <param name="productCode">商品コード</param>
		/// <param name="productName">商品名</param>
		/// <param name="product">商品情報</param>
		/// <param name="price">商品価格</param>
		/// <param name="qty">購入数</param>
		/// <returns>注文商品情報</returns>
		private OrderItemModel CreateOrderItem(int orderItemNo, string productCode, string productName, ProductModel product, decimal price, int qty)
		{
			if (string.IsNullOrEmpty(productCode) && (product == null)) return null;

			var productTaxCategory = new ProductTaxCategoryService().Get(product.TaxCategoryId);
			var productPriceTax = (product != null)
				? TaxCalculationUtility.GetTaxPrice(
					price,
					productTaxCategory.TaxRate,
					Constants.TAX_EXCLUDED_FRACTION_ROUNDING,
					true)
				: 0m;
			var productPricePrescribed = TaxCalculationUtility.GetPrescribedPrice(price, productPriceTax, true);
			var itemPriceTax = productPriceTax * qty;
			var itemPrice = productPricePrescribed * qty;
			var orderItem = new OrderItemModel
			{
				OrderId = this.ResponseData.OrderId,
				OrderItemNo = orderItemNo,
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				ProductId = this.ResponseData.GetProductId(productCode),
				VariationId = this.ResponseData.GetVariationId(productCode),
				SupplierId = (product != null) ? StringUtility.ToEmpty(product.SupplierId) : string.Empty,
				ProductName = productName,
				ProductNameKana = (product != null) ? StringUtility.ToEmpty(product.NameKana) : string.Empty,
				ProductPrice = productPricePrescribed,
				ProductPriceOrg = productPricePrescribed,
				ProductTaxRate = (product != null) ? productTaxCategory.TaxRate : 0m,
				ProductPricePretax = price,
				ProductTaxIncludedFlg = TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag(),
				ItemQuantity = qty,
				ItemQuantitySingle = qty,
				ItemPrice = itemPrice,
				ItemPriceSingle = itemPrice,
				ItemPriceTax = itemPriceTax,
				ProductSetId = string.Empty,
				ItemMemo = string.Empty,
				ProductOptionTexts = string.Empty,
				BrandId = string.Empty,
				DownloadUrl = string.Empty,
				ProductsaleId = string.Empty,
				CooperationId1 = (product.HasProductVariation) ? product.VariationCooperationId1 : product.CooperationId1,
				CooperationId2 = (product.HasProductVariation) ? product.VariationCooperationId2 : product.CooperationId2,
				CooperationId3 = (product.HasProductVariation) ? product.VariationCooperationId3 : product.CooperationId3,
				CooperationId4 = (product.HasProductVariation) ? product.VariationCooperationId4 : product.CooperationId4,
				CooperationId5 = (product.HasProductVariation) ? product.VariationCooperationId5 : product.CooperationId5,
				CooperationId6 = (product.HasProductVariation) ? product.VariationCooperationId6 : product.CooperationId6,
				CooperationId7 = (product.HasProductVariation) ? product.VariationCooperationId7 : product.CooperationId7,
				CooperationId8 = (product.HasProductVariation) ? product.VariationCooperationId8 : product.CooperationId8,
				CooperationId9 = (product.HasProductVariation) ? product.VariationCooperationId9 : product.CooperationId9,
				CooperationId10 = (product.HasProductVariation) ? product.VariationCooperationId10 : product.CooperationId10,
				NoveltyId = string.Empty,
				RecommendId = string.Empty,
				FixedPurchaseProductFlg = (((product != null) && (product.FixedPurchaseFlg != null))
					&& (product.FixedPurchaseFlg != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID))
					? Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON
					: Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_OFF,
				ProductBundleId = string.Empty,
				ShippingSizeKbn = product.ShippingSizeKbn,
				BundleItemDisplayType = product.BundleItemDisplayType,
			};
			return orderItem;
		}

		/// <summary>
		/// 注文者情報作成
		/// </summary>
		/// <returns>注文者情報</returns>
		private OrderOwnerModel CreateOrderOwner()
		{
			var owner = new OrderOwnerModel
			{
				OrderId = this.ResponseData.OrderId,
				OwnerKbn = this.ResponseData.User.UserKbn,
				OwnerName = this.ResponseData.User.Name,
				OwnerName1 = this.ResponseData.User.Name1,
				OwnerName2 = this.ResponseData.User.Name2,
				OwnerNameKana = this.ResponseData.User.NameKana,
				OwnerNameKana1 = this.ResponseData.User.NameKana1,
				OwnerNameKana2 = this.ResponseData.User.NameKana2,
				OwnerMailAddr = this.ResponseData.User.MailAddr,
				OwnerMailAddr2 = string.Empty,
				OwnerZip = this.ResponseData.User.Zip,
				OwnerAddr1 = this.ResponseData.User.Addr1,
				OwnerAddr2 = this.ResponseData.User.Addr2,
				OwnerAddr3 = this.ResponseData.User.Addr3,
				OwnerAddr4 = this.ResponseData.User.Addr4,
				OwnerCompanyName = string.Empty,
				OwnerCompanyPostName = string.Empty,
				OwnerTel1 = this.ResponseData.User.Tel1,
				OwnerSex = this.ResponseData.User.Sex,
				OwnerBirth = this.ResponseData.User.Birth,
				DateCreated = this.ResponseData.Created.GetValueOrDefault(DateTime.Now),
				DateChanged = this.ResponseData.Created.GetValueOrDefault(DateTime.Now)
			};
			return owner;
		}

		/// <summary>
		/// 配送先情報作成
		/// </summary>
		/// <returns>配送先情報</returns>
		private OrderShippingModel CreateOrderShipping()
		{
			var shipping = new OrderShippingModel
			{
				OrderId = this.ResponseData.OrderId,
				ShippingName = this.ResponseData.User.Name,
				ShippingNameKana = this.ResponseData.User.NameKana,
				ShippingZip = this.ResponseData.User.Zip,
				ShippingAddr1 = this.ResponseData.User.Addr1,
				ShippingAddr2 = this.ResponseData.User.Addr2,
				ShippingAddr3 = this.ResponseData.User.Addr3,
				ShippingAddr4 = this.ResponseData.User.Addr4,
				ShippingTel1 = this.ResponseData.User.Tel1,
				ShippingTel2 = string.Empty,
				ShippingTel3 = string.Empty,
				ShippingFax = string.Empty,
				ShippingCompany = "0",
				ShippingDate = null,
				ShippingTime = string.Empty,
				ShippingCheckNo = string.Empty,
				DelFlg = "0",
				DateCreated = this.ResponseData.Created.HasValue
					? this.ResponseData.Created.Value
					: DateTime.Now,
				DateChanged = this.ResponseData.Created.HasValue
					? this.ResponseData.Created.Value
					: DateTime.Now,
				ShippingName1 = this.ResponseData.User.Name1,
				ShippingName2 = this.ResponseData.User.Name2,
				ShippingNameKana1 = this.ResponseData.User.NameKana1,
				ShippingNameKana2 = this.ResponseData.User.NameKana2,
				SenderName = string.Empty,
				SenderName1 = string.Empty,
				SenderName2 = string.Empty,
				SenderNameKana = string.Empty,
				SenderNameKana1 = string.Empty,
				SenderNameKana2 = string.Empty,
				SenderZip = string.Empty,
				SenderAddr1 = string.Empty,
				SenderAddr2 = string.Empty,
				SenderAddr3 = string.Empty,
				SenderAddr4 = string.Empty,
				SenderTel1 = string.Empty,
				WrappingPaperType = string.Empty,
				WrappingPaperName = string.Empty,
				WrappingBagType = string.Empty,
				ShippingCompanyName = StringUtility.ToEmpty(this.ResponseData.Cart.Shippings.First().CompanyName),
				ShippingCompanyPostName = StringUtility.ToEmpty(this.ResponseData.Cart.Shippings.First().CompanyPostName),
				SenderCompanyName = string.Empty,
				SenderCompanyPostName = string.Empty,
				AnotherShippingFlg = Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID,
				ShippingMethod = StringUtility.ToEmpty(this.ResponseData.Cart.Shippings.First().ShippingMethod),
				DeliveryCompanyId = StringUtility.ToEmpty(this.ResponseData.Cart.Shippings.First().DeliveryCompanyId),
				ScheduledShippingDate = OrderCommon.CalculateScheduledShippingDateBasedOnToday(
					Constants.CONST_DEFAULT_SHOP_ID,
					null,
					StringUtility.ToEmpty(this.ResponseData.Cart.Shippings.First().ShippingMethod),
					StringUtility.ToEmpty(this.ResponseData.Cart.Shippings.First().DeliveryCompanyId),
					this.ResponseData.User.AddrCountryIsoCode,
					this.ResponseData.User.Addr1,
					this.ResponseData.User.Zip)
			};
			return shipping;
		}

		/// <summary>
		/// カートオブジェクト生成
		/// </summary>
		private void CreateCartObject()
		{
			var orderKbn = ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN, this.ResponseData.Type.ToUpper());
			var shippingType = (this.ResponseData.LandingProduct != null)
				? this.ResponseData.LandingProduct.ShippingType
				: (this.ResponseData.UpsellProduct != null)
					? this.ResponseData.UpsellProduct.ShippingType
					: null;
			var memberRankId = MemberRankOptionUtility.GetMemberRankId(this.ResponseData.User.UserId, this.Accessor);
			var cart = new CartObject(
				this.ResponseData.User.UserId,
				orderKbn,
				Constants.CONST_DEFAULT_SHOP_ID,
				shippingType,
				false,
				false,
				memberRankId,
				accessor: this.Accessor);

			var landingProduct = CreateCartProduct(
				this.ResponseData.LandingProduct,
				this.ResponseData.LandingProductPriceInc.GetValueOrDefault(0),
				this.ResponseData.LandingProductQty.GetValueOrDefault(0));
			if (landingProduct != null) AddProductToCart(cart, landingProduct);

			var upsellProduct = CreateCartProduct(
				this.ResponseData.UpsellProduct,
				this.ResponseData.UpsellProductPriceInc.GetValueOrDefault(0),
				this.ResponseData.UpsellProductQty.GetValueOrDefault(0));
			if (upsellProduct != null) AddProductToCart(cart, upsellProduct);

			var cartList = new CartObjectList(
				this.ResponseData.User.UserId,
				orderKbn,
				false,
				memberRankId: memberRankId,
				accessor: this.Accessor);

			cart.PriceRegulation = this.ResponseData.Discount.GetValueOrDefault(0);
			cartList.AddCartVirtural(cart);

			if (cartList.Items.Any() == false) return;

			var payment = new CartPayment
			{
				PaymentId = ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, this.ResponseData.PaymentMethod),
				PriceExchange = this.ResponseData.Commission.GetValueOrDefault(0)
			};
			if (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				var userCreditCard = new UserCreditCardService().Get(
					this.ResponseData.User.UserId,
					this.ResponseData.CreditBranchNo,
					this.Accessor);
				if (userCreditCard != null) payment.UserCreditCard = new UserCreditCard(userCreditCard, this.Accessor);
			}
			cartList.Items.ForEach(cartObject => cartObject.Payment = payment);

			cartList.SetOwner(new CartOwner(
				this.ResponseData.User.UserKbn,
				this.ResponseData.User.Name,
				this.ResponseData.User.Name1,
				this.ResponseData.User.Name2,
				this.ResponseData.User.NameKana,
				this.ResponseData.User.NameKana1,
				this.ResponseData.User.NameKana2,
				this.ResponseData.User.MailAddr,
				string.Empty,
				string.Empty,
				this.ResponseData.User.Zip1,
				this.ResponseData.User.Zip2,
				this.ResponseData.User.Addr1,
				this.ResponseData.User.Addr2,
				this.ResponseData.User.Addr3,
				this.ResponseData.User.Addr4,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				this.ResponseData.User.Tel1_1,
				this.ResponseData.User.Tel1_2,
				this.ResponseData.User.Tel1_3,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				(this.ResponseData.User.MailFlg == Constants.FLG_USER_MAILFLG_OK),
				this.ResponseData.User.Sex,
				this.ResponseData.User.Birth,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty));
			cartList.Items.First().Shippings.First().UpdateShippingAddr(
				this.ResponseData.User.Name1,
				this.ResponseData.User.Name2,
				this.ResponseData.User.NameKana1,
				this.ResponseData.User.NameKana2,
				string.Empty,
				this.ResponseData.User.Zip1,
				this.ResponseData.User.Zip2,
				this.ResponseData.User.Addr1,
				this.ResponseData.User.Addr2,
				this.ResponseData.User.Addr3,
				this.ResponseData.User.Addr4,
				this.ResponseData.User.Addr5,
				this.ResponseData.User.AddrCountryIsoCode,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				this.ResponseData.User.Tel1_1,
				this.ResponseData.User.Tel1_2,
				this.ResponseData.User.Tel1_3,
				true,
				CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER);
			cartList.Items.First().UpdateAnotherShippingFlag();
			cartList.CalculateAllCart();

			// 再計算で割引が適用されてしまうため再設定
			cartList.Items.First().Shippings.First().UpdateCartPriceShipping(this.ResponseData.ShippingCost.GetValueOrDefault(0));
			var priceTotal = IsSubscriptionBoxFixedAmount()
				? (GetSubscriptionBoxFixedAmount().GetValueOrDefault(0)
					+ this.ResponseData.ShippingCost.GetValueOrDefault(0)
					+ this.ResponseData.Commission.GetValueOrDefault(0))
				: this.ResponseData.TotalInc.GetValueOrDefault(0);
			cartList.Items.First().SetPriceTotal(priceTotal);

			this.ResponseData.Cart = cartList.Items.First();
		}

		/// <summary>
		/// カート商品情報生成
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="price">商品価格</param>
		/// <param name="qty">商品購入数</param>
		/// <returns>カート商品情報</returns>
		private CartProduct CreateCartProduct(ProductModel product, decimal price, int qty)
		{
			if (product == null) return null;

			var addCartKbn = this.ResponseData.IsFixedPurchase
				? Constants.AddCartKbn.FixedPurchase
				: Constants.AddCartKbn.Normal;
			var cartProduct = new CartProduct(
				new ProductService().GetProductVariationAtDataRowView(
					Constants.CONST_DEFAULT_SHOP_ID,
					product.ProductId,
					product.VariationId,
					this.ResponseData.User.MemberRankId),
				addCartKbn,
				string.Empty,
				qty,
				false);
			cartProduct.SetPrice(price);
			return cartProduct;
		}

		/// <summary>
		/// 商品追加
		/// </summary>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="product">カート商品</param>
		private void AddProductToCart(CartObject cart, CartProduct product)
		{
			if ((product == null) || string.IsNullOrEmpty(product.ShippingType)) return;

			var shopShipping = ShopShippingService.GetAsStatic(cart.ShopId, product.ShippingType);
			cart.Shippings.First().ShippingMethod = (product.ShippingSizeKbn == Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL)
				? Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL
				: Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;

			var compayList = (product.ShippingSizeKbn == Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL)
				? shopShipping.CompanyListMail
				: shopShipping.CompanyListExpress;
			cart.Shippings.First().DeliveryCompanyId = GetDefaultDeliveryCompanyId(compayList);

			cart.AddVirtural(product, false);
		}

		/// <summary>
		/// デフォルト配送サービスの取得
		/// </summary>
		/// <param name="model">配送種別</param>
		/// <returns>デフォルト配送サービスID</returns>
		private string GetDefaultDeliveryCompanyId(ShopShippingCompanyModel[] model)
		{
			var result = model
				.Where(companyList => companyList.DefaultDeliveryCompany == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_DEFAULT_DELIVERY_COMPANY_VALID)
				.Select(companyList => companyList.DeliveryCompanyId).First();

			return result;
		}

		/// <summary>
		/// 決済実行
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string ExecPayment()
		{
			var message = string.Empty;
			if ((this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				|| ((this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
					&& (string.IsNullOrEmpty(this.Order.CardTranId))))
			{
				message = new OrderPaymentRegisterExternal(
					this.Order,
					this.ResponseData.Cart,
					Constants.FLG_LASTCHANGED_BATCH).ExecPayment();
			}
			return message;
		}

		/// <summary>
		/// 後払い与信エラーメール送信（ユーザー向け）
		/// </summary>
		public void SendMailForCvsDefAuthError()
		{
			if ((this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				|| (this.Order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY))
			{
				var input = new MailTemplateDataCreaterForOrder(true).GetOrderMailDatas(this.Order.OrderId);
				using (var mailSender = new MailSendUtility(
					Constants.CONST_DEFAULT_SHOP_ID,
					Constants.CONST_MAIL_ID_URERU_AD_IMPORT_FOR_USER,
					this.Order.UserId,
					input,
					true,
					App.Common.Constants.MailSendMethod.Auto,
					userMailAddress: this.Order.Owner.OwnerMailAddr))
				{
					mailSender.AddTo(StringUtility.ToEmpty(this.Order.Owner.OwnerMailAddr));
					if (mailSender.SendMail() == false)
					{
						FileLogger.WriteError(
							CommerceMessages.GetMessages(Constants.ERRMSG_MANAGER_AUTH_ERROR_MAIL_SEND_FAILED)
								.Replace("@@ 1 @@", this.Order.OrderId),
							mailSender.MailSendException);
					}
				}
			}
		}
		/// <summary>
		/// 税率毎価格情報作成
		/// </summary>
		private void CreatePriceByTaxRate()
		{
			var stackedDiscountAmount = 0m;
			var priceTotal = this.Order.SubscriptionBoxFixedAmount
				?? this.Order.Items.Sum(item => item.ProductPricePretax * item.ItemQuantity);
			// 調整金額適用対象金額取得
			var shippingPrice = this.Order.OrderPriceShipping;
			var paymentPrice = this.Order.OrderPriceExchange;
			priceTotal += paymentPrice;
			priceTotal += shippingPrice;
			var itemInfo = new List<Hashtable>();
			var shippingRegulationPrice = 0m;
			var paymentRegulationPrice = 0m;
			if (priceTotal != 0)
			{
				itemInfo.AddRange(this.Order.Items.Select(
					item => new Hashtable
					{
						{ "itemPriceRegulation", (item.ProductPricePretax * item.ItemQuantity) / priceTotal * this.Order.OrderPriceRegulation },
						{ "item", item}
					}));
				stackedDiscountAmount = itemInfo.Sum(item => (decimal)item["itemPriceRegulation"]);

				shippingRegulationPrice = Math.Floor(shippingPrice / priceTotal * this.Order.OrderPriceRegulation);
				stackedDiscountAmount += shippingRegulationPrice;

				paymentRegulationPrice = Math.Floor(paymentPrice / priceTotal * this.Order.OrderPriceRegulation);
				stackedDiscountAmount += paymentRegulationPrice;
			}

			// 調整金額の端数重み付けを行う
			var fractionAmount = this.Order.OrderPriceRegulation - stackedDiscountAmount;
			if (fractionAmount != 0)
			{
				var weightItem = itemInfo.FirstOrDefault(
					item => (((OrderItemModel)item["item"]).ProductPricePretax
						* ((OrderItemModel)item["item"]).ItemQuantity) > 0);
				if (weightItem != null)
				{
					weightItem["itemPriceRegulation"] = (decimal)weightItem["itemPriceRegulation"] + fractionAmount;
				}
				else if (shippingPrice != 0)
				{
					shippingRegulationPrice += fractionAmount;
				}
				else
				{
					paymentRegulationPrice += fractionAmount;
				}
			}

			// 税率毎の購入金額を算出する
			var priceInfo = new List<Hashtable>();
			// 頒布会定額コースであれば定額価格を格納
			if (IsSubscriptionBoxFixedAmount())
			{
				priceInfo.Add(
					new Hashtable
					{
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, GetSubscriptionBoxTaxRate().GetValueOrDefault(0) },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE, this.Order.SubscriptionBoxFixedAmount.GetValueOrDefault(0) },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, 0m },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE, 0m },
					});
			}
			else
			{
				priceInfo.AddRange(itemInfo
					.Select(item => new Hashtable
					{
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , ((OrderItemModel)item["item"]).ProductTaxRate },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 
							(((OrderItemModel)item["item"]).ProductPricePretax * ((OrderItemModel)item["item"]).ItemQuantity) + (decimal)item["itemPriceRegulation"] },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , 0m },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
					}).ToList());
			}

			priceInfo.Add(new Hashtable
			{
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , this.Order.ShippingTaxRate },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , shippingPrice + shippingRegulationPrice },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
			});
			priceInfo.Add(new Hashtable
			{ 
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , this.Order.PaymentTaxRate },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , paymentPrice + paymentRegulationPrice },
			});

			var groupedItem = priceInfo.GroupBy(item => new
			{
				taxRate = (decimal)item[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE]
			});
			var priceByTaxRate = groupedItem.Select(
				item => new OrderPriceByTaxRateModel
				{
					KeyTaxRate = item.Key.taxRate,
					PriceSubtotalByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]),
					PriceShippingByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]),
					PricePaymentByRate = item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE]),
					PriceTotalByRate = item.Sum(itemKey =>
						((decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE])),
					TaxPriceByRate = TaxCalculationUtility.GetTaxPrice(item.Sum(itemKey =>
							((decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE])),
						(decimal)item.Key.taxRate,
						Constants.TAX_EXCLUDED_FRACTION_ROUNDING,
						true)
				}).ToArray();
			foreach (var orderPriceByTaxRateModel in priceByTaxRate)
			{
				orderPriceByTaxRateModel.OrderId = this.Order.OrderId;
			}
			this.Order.OrderPriceByTaxRates = priceByTaxRate;
			this.Order.OrderPriceTax = priceByTaxRate.Sum(price => price.TaxPriceByRate);
		}

		/// <summary>
		/// 頒布会定額コースか
		/// </summary>
		/// <returns>頒布会定額コースであればTrue</returns>
		protected bool IsSubscriptionBoxFixedAmount()
		{
			var subscriptionBoxCourseId = this.ResponseData.GetSubscriptionBoxCourseId();

			if (string.IsNullOrEmpty(subscriptionBoxCourseId)) return false;
			var subscriptionBox = new SubscriptionBoxService().GetByCourseId(subscriptionBoxCourseId);
			if (subscriptionBox == null) return false;
			var result = (subscriptionBox.FixedAmountFlg == App.Common.Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE);
			return result;
		}

		/// <summary>
		/// 頒布会定額コースの税率を取得
		/// </summary>
		/// <returns>頒布会定額コースの税率</returns>
		protected decimal? GetSubscriptionBoxTaxRate()
		{
			if (IsSubscriptionBoxFixedAmount() == false) return null;

			var subscriptionBoxCourseId = this.ResponseData.GetSubscriptionBoxCourseId();
			var subscriptionBox = new SubscriptionBoxService().GetByCourseId(subscriptionBoxCourseId);
			if (subscriptionBox == null) return null;
			var result = new ProductTaxCategoryService().Get(subscriptionBox.TaxCategoryId).TaxRate;
			return result;
		}

		/// <summary>
		/// 頒布会コースの定額価格を取得
		/// </summary>
		/// <returns>定額価格</returns>
		protected decimal? GetSubscriptionBoxFixedAmount()
		{
			if (IsSubscriptionBoxFixedAmount() == false) return null;

			var subscriptionBoxCourseId = this.ResponseData.GetSubscriptionBoxCourseId();
			var subscriptionBox = new SubscriptionBoxService().GetByCourseId(subscriptionBoxCourseId);
			var result = subscriptionBox.FixedAmount.GetValueOrDefault(0);
			return result;
		}

		#region プロパティ
		/// <summary>注文情報</summary>
		private OrderModel Order { get; set; }
		/// <summary>ユーザーポイント情報登録クラス</summary>
		private UserPointImport UserPoint { get; set; }
		/// <summary>定期購入情報登録クラス</summary>
		private FixedPurchaseImport FixedPurchase { get; set; }
		#endregion
	}
}
