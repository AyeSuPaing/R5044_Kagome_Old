/*
=========================================================================================================
  Module      : 注文共通処理クラス(OrderCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using w2.App.Common.Api;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global;
using w2.App.Common.Global.Config;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Global.Translation;
using w2.App.Common.Input.Order;
using w2.App.Common.Mail;
using w2.App.Common.NextEngine;
using w2.App.Common.NextEngine.Helper;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order.Payment.Aftee;
using w2.App.Common.Order.Payment.Atobaraicom.Shipping;
using w2.App.Common.Order.Payment.Atone;
using w2.App.Common.Order.Payment.DSKDeferred.Helper;
using w2.App.Common.Order.Payment.DSKDeferred.Shipment;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.GMO;
using w2.App.Common.Order.Payment.GMO.BillingConfirmation;
using w2.App.Common.Order.Payment.GMO.GetCreditStatus;
using w2.App.Common.Order.Payment.GMO.Helper;
using w2.App.Common.Order.Payment.GMO.Shipment;
using w2.App.Common.Order.Payment.GMOAtokara;
using w2.App.Common.Order.Payment.JACCS.ATODENE;
using w2.App.Common.Order.Payment.JACCS.ATODENE.Helper;
using w2.App.Common.Order.Payment.JACCS.ATODENE.Shipping;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.Paypay;
using w2.App.Common.Order.Payment.Score;
using w2.App.Common.Order.Payment.Score.Delivery;
using w2.App.Common.Order.Payment.Score.Helper;
using w2.App.Common.Order.Payment.TriLinkAfterPay;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Request;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Payment.YamatoKwc;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;
using w2.App.Common.Product;
using w2.App.Common.Properties;
using w2.App.Common.Util;
using w2.App.Common.Web;
using w2.App.Common.Web.Page;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Extensions;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Web;
using w2.Common.Wrapper;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.FixedPurchaseRepeatAnalysis;
using w2.Domain.Holiday.Helper;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.Order;
using w2.Domain.Payment;
using w2.Domain.Point;
using w2.Domain.Product;
using w2.Domain.ProductSale;
using w2.Domain.ProductTaxCategory;
using w2.Domain.SerialKey;
using w2.Domain.SetPromotion;
using w2.Domain.ShopShipping;
using w2.Domain.SubscriptionBox;
using w2.Domain.TwFixedPurchaseInvoice;
using w2.Domain.TwInvoice;
using w2.Domain.TwOrderInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.User.Helper;
using w2.Domain.UserDefaultOrderSetting;
using w2.Domain.UserShipping;
using TransactionElement = w2.App.Common.Order.Payment.GMO.Shipment.TransactionElement;
using Validator = w2.Common.Util.Validator;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>
	/// 注文共通処理クラス
	/// </summary>
	///*********************************************************************************************
	public partial class OrderCommon
	{
		/// <summary>調整後付与ポイント（仮ポイント）</summary>
		public const string CONST_ORDER_POINT_ADD_TEMP = Constants.FIELD_ORDER_ORDER_POINT_ADD + "_temp";
		/// <summary>調整付与ポイント（仮 or 本ポイント）</summary>
		public const string CONST_ORDER_POINT_ADD_ADJUSTMENT = Constants.FIELD_ORDER_ORDER_POINT_ADD + "_adjustment";
		/// <summary>調整後付与 通常ポイント（本ポイント）</summary>
		public const string CONST_ORDER_BASE_POINT_ADD_COMP = Constants.FIELD_ORDER_ORDER_POINT_ADD + "_base_comp";
		/// <summary>調整後付与 期間限定ポイント（本ポイント）</summary>
		public const string CONST_ORDER_LIMIT_POINT_ADD_COMP = Constants.FIELD_ORDER_ORDER_POINT_ADD + "_limit_comp";
		/// <summary>調整後利用ポイント</summary>
		public const string CONST_ORDER_ORDER_POINT_USE_ADJUSTMENT = Constants.FIELD_ORDER_ORDER_POINT_USE + "_adjustment";
		/// <summary>Product Id For None Product Item</summary>
		public const string CONST_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM = "xxxxx";
		/// <summary>Ec pay invoice api message</summary>
		public const string ECPAY_INVOICE_API_MESSAGE = "ecPayInvoiceApiMessage";
		/// <summary>Constant custommer customer name</summary>
		private const string CONST_CUSTOMMER_CUSTOMER_NAME = "customer_name";
		/// <summary>Constant custommer customer family name</summary>
		private const string CONST_CUSTOMMER_CUSTOMER_FAMILY_NAME = "customer_family_name";
		/// <summary>Constant custommer customer given name</summary>
		private const string CONST_CUSTOMMER_CUSTOMER_GIVEN_NAME = "customer_given_name";
		/// <summary>Constant custommer customer name kana</summary>
		private const string CONST_CUSTOMMER_CUSTOMER_NAME_KANA = "customer_name_kana";
		/// <summary>Constant custommer customer family name kana</summary>
		private const string CONST_CUSTOMMER_CUSTOMER_FAMILY_NAME_KANA = "customer_family_name_kana";
		/// <summary>Constant custommer customer given name kana</summary>
		private const string CONST_CUSTOMMER_CUSTOMER_GIVEN_NAME_KANA = "customer_given_name_kana";
		/// <summary>Constant custommer phone number</summary>
		private const string CONST_CUSTOMMER_PHONE_NUMBER = "phone_number";
		/// <summary>Constant custommer sex division</summary>
		private const string CONST_CUSTOMMER_SEX_DIVISION = "sex_division";
		/// <summary>Constant custommer company name</summary>
		private const string CONST_CUSTOMMER_COMPANY_NAME = "company_name";
		/// <summary>Constant custommer department</summary>
		private const string CONST_CUSTOMMER_DEPARTMENT = "department";
		/// <summary>Constant custommer zip code</summary>
		private const string CONST_CUSTOMMER_ZIP_CODE = "zip_code";
		/// <summary>Constant custommer address</summary>
		private const string CONST_CUSTOMMER_ADDRESS = "address";
		/// <summary>Constant custommer tel</summary>
		private const string CONST_CUSTOMMER_TEL = "tel";
		/// <summary>Constant custommer email</summary>
		private const string CONST_CUSTOMMER_EMAIL = "email";
		/// <summary>Constant custommer total purchase count</summary>
		private const string CONST_CUSTOMMER_TOTAL_PURCHASE_COUNT = "total_purchase_count";
		/// <summary>Constant custommer total purchase amount</summary>
		private const string CONST_CUSTOMMER_TOTAL_PURCHASE_AMOUNT = "total_purchase_amount";
		/// <summary>Constant custommer birthday</summary>
		private const string CONST_CUSTOMMER_BIRTHDAY = "birthday";

		/// <summary>
		/// 注文情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文情報</returns>
		/// <remarks>ユーザー履歴はここから取得してはいけない。セキュリティ上user_idも指定すべき</remarks>
		public static DataView GetOrder(string orderId)
		{
			using (SqlAccessor accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				return GetOrder(orderId, accessor);
			}
		}
		/// <summary>
		/// 注文情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文情報</returns>
		/// <remarks>ユーザー履歴はここから取得してはいけない。セキュリティ上user_idも指定すべき</remarks>
		public static DataView GetOrder(string orderId, SqlAccessor accessor)
		{
			var order = new OrderService().GetOrderInDataView(orderId, accessor);
			return order;
		}

		/// <summary>
		/// 注文一覧データセットを表示分だけ取得
		/// </summary>
		/// <param name="search">検索情報</param>
		/// <param name="pageNumber">表示開始記事番号</param>
		/// <returns>注文一覧データセット</returns>
		public static DataView GetOrderListForWorkflow(Hashtable search, int pageNumber = 1)
		{
			var order = new OrderService().GetOrderWorkflowListInDataView(search, pageNumber);
			return order;
		}

		/// <summary>
		/// 注文一覧データの件数を取得
		/// </summary>
		/// <param name="search">検索情報</param>
		/// <returns>注文一覧データの件数</returns>
		public static DataView GetOrderListForWorkflowCount(Hashtable search, int pageNumber = 1)
		{
			var order = new OrderService().GetOrderWorkflowListCountInDataView(search, pageNumber);
			return order;
		}

		/// <summary>
		/// 最終与信の注文情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文情報</returns>
		public static OrderModel GetLastAuthOrder(string orderId, SqlAccessor accessor = null)
		{
			var orderService = new OrderService();
			var order = orderService.Get(orderId, accessor);
			if (order == null) return null;

			// すべての関連注文取得
			var orderIdOrg = string.IsNullOrEmpty(order.OrderIdOrg) ? order.OrderId : order.OrderIdOrg;
			var relatedOrders = orderService.GetRelatedOrders(orderIdOrg, accessor);
			// 最終与信フラグONを持っている注文取得
			var lastAuthOrder = relatedOrders.FirstOrDefault(o => o.LastAuthFlg == Constants.FLG_ORDER_LAST_AUTH_FLG_ON);
			// VUP前の注文の場合、当注文戻す
			if (lastAuthOrder == null) return order;
			return lastAuthOrder;
		}

		/// <summary>
		/// Get Order Coupon
		/// </summary>
		/// <param name="returnOrder">Return Order</param>
		/// <returns>Order Coupon</returns>
		public static OrderCoupon GetOrderCoupon(Order returnOrder)
		{
			OrderCoupon orderCoupon = null;

			// ユーザークーポン履歴取得（利用 or 利用キャンセルのみ）
			var userCouponHistoires =
				new CouponService().GetHistoiresByOrderId(
						returnOrder.UserId,
						returnOrder.OrderId
					)
					.Where(item =>
						(item.HistoryKbn == Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE)
						|| (item.HistoryKbn == Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE_CANCEL))
					.OrderByDescending(data => data.HistoryNo).ToArray();

			// クーポンを利用している場合はクーポン情報を格納し取消処理を実施
			if (userCouponHistoires.Length != 0)
			{
				var userCouponHistory = userCouponHistoires[0];
				if (userCouponHistory.HistoryKbn == Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE)
				{
					orderCoupon = returnOrder.Coupon;
				}
			}

			return orderCoupon;
		}

		/// <summary>
		/// 商品在庫購入可能チェック
		/// </summary>
		/// <param name="cartProduct">カート商品</param>
		/// <returns>商品在庫購入可能可否</returns>
		public static bool CheckProductStockBuyable(CartProduct cartProduct)
		{
			var productVariation = DomainFacade.Instance.ProductService.GetProductVariationAtDataRowView(
				cartProduct.ShopId,
				cartProduct.ProductId,
				cartProduct.VariationId,
				null);
			if (productVariation != null)
			{
				return CheckProductStockBuyable(productVariation, cartProduct.Count);
			}

			return false;
		}

		/// <summary>
		/// 商品在庫購入可能チェック
		/// </summary>
		/// <param name="productVariation">商品バリエーション</param>
		/// <param name="buyCount">購入数</param>
		/// <returns>商品在庫購入可能可否</returns>
		public static bool CheckProductStockBuyable(DataRowView productVariation, int buyCount)
		{
			// （在庫マスタに商品がない場合は0とする）
			var iStock = (int)StringUtility.ToValue(productVariation[Constants.FIELD_PRODUCTSTOCK_STOCK], 0);

			// 在庫管理方法が「在庫０以下の場合、表示する。購入不可」かつ「在庫が0以下」
			if (((string)productVariation[Constants.FIELD_PRODUCT_STOCK_MANAGEMENT_KBN] == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYNG)
				&& (buyCount > iStock))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// 商品在庫購入可能チェック
		/// </summary>
		/// <param name="useVariation">バリエーションあり</param>
		/// <param name="variationSelected">バリエーション選択済み</param>
		/// <param name="productVariationList">バリエーションリスト</param>
		/// <param name="selectedProductVariation">選択されたバリエーション</param>
		/// <returns>購入可能なものがあるか</returns>
		public static bool CheckProductStockBuyable(
			bool useVariation,
			bool variationSelected,
			DataView productVariationList,
			DataRowView selectedProductVariation)
		{
			// バリエーション管理 && バリエーション選択なし
			if ((useVariation) && (variationSelected == false))
			{
				foreach (DataRowView productVariation in productVariationList)
				{
					if (CheckProductStockBuyable(productVariation, 1))
					{
						return true;
					}
				}
			}
			else
			{
				return CheckProductStockBuyable(selectedProductVariation, 1);
			}

			return false;
		}

		/// <summary>
		/// 商品セール価格取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>新注文ID</returns>
		public static decimal? GetProductSalePrice(string shopId, string productSaleId, string productId, string variationId)
		{
			DataView productSalePrice;
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("ProductSale", "GetProductSalePrice"))
			{
				var htInput = new Hashtable
				{
					{ Constants.FIELD_PRODUCTSALE_SHOP_ID, shopId },
					{ Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID, productSaleId },
					{ Constants.FIELD_PRODUCTSALEPRICE_PRODUCT_ID, productId },
					{ Constants.FIELD_PRODUCTSALEPRICE_VARIATION_ID, variationId }
				};

				productSalePrice = statement.SelectSingleStatementWithOC(accessor, htInput);
			}

			return (productSalePrice.Count != 0) ? (decimal?)productSalePrice[0][Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE] : null;
		}

		/// <summary>
		/// 商品セール期間取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <returns>セール期間</returns>
		public static string GetProductSaleTerm(string shopId, string productSaleId = "")
		{
			if (string.IsNullOrEmpty(productSaleId)) return string.Empty;

			var productSale = new ProductSaleService().Get(shopId, productSaleId);
			var beginDate = DateTimeUtility.ToStringFromRegion(productSale.DateBgn, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			var endDate = DateTimeUtility.ToStringFromRegion(productSale.DateEnd, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			var productSaleTerm = beginDate + "~" + endDate;
			return productSaleTerm;
		}

		/// <summary>
		/// セットプロモーション期間取得
		/// </summary>
		/// <param name="setPromotionId">セットプロモーションID</param>
		/// <returns>セットプロモーション期間</returns>
		public static string GetSetPromotionTerm(string setPromotionId = "")
		{
			if (string.IsNullOrEmpty(setPromotionId)) return string.Empty;
			var setPromotion = new SetPromotionService().Get(setPromotionId);
			var beginDate = DateTimeUtility.ToStringFromRegion(setPromotion.BeginDate, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			var endDate = DateTimeUtility.ToStringFromRegion(setPromotion.EndDate, DateTimeUtility.FormatType.LongDateHourMinuteNoneServerTime);
			var setPromotionTerm = beginDate + "~" + endDate;
			return setPromotionTerm;
		}

		/// <summary>
		/// 注文ID生成
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>新注文ID</returns>
		public static string CreateOrderId(string shopId)
		{
			return string.Format(Constants.FORMAT_ORDER_ID, CreateNewNumberingId(shopId, Constants.NUMBER_KEY_ORDER_ID));
		}

		/// <summary>
		/// 決済注文ID生成
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>新決済注文ID</returns>
		public static string CreatePaymentOrderId(string shopId)
		{
			return string.Format(Constants.FORMAT_PAYMENT_ORDER_ID, NumberingUtility.CreateNewNumber(shopId, Constants.NUMBER_KEY_PAYMENT_ORDER_ID));
		}

		/// <summary>
		/// ヤマトKWCトークン決済向け会員ID作成
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>新決済注文ID</returns>
		public static string CreateYamatoKwcMemberId(string shopId)
		{
			return string.Format(Constants.FORMAT_PAYMENT_YAMATOKWC_MEMBER_ID, NumberingUtility.CreateNewNumber(shopId, Constants.NUMBER_KEY_YAMATOKWC_MEMBER_ID));
		}

		/// <summary>
		/// 注文情報取得
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="orderId">受注ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="careerId">キャリアID</param>
		/// <param name="mobileUid">モバイルUID</param>
		/// <param name="remoteAddr">リモートアドレス</param>
		/// <param name="advcodeFirst">初回広告コード</param>
		/// <param name="advcodeNew">最新広告コード</param>
		/// <param name="lastChanged">更新ユーザー</param>
		/// <returns>注文情報</returns>
		public static Hashtable CreateOrderInfo(
			CartObject cart,
			string orderId,
			string userId,
			string careerId,
			string mobileUid,
			string remoteAddr,
			string advcodeFirst,
			string advcodeNew,
			string lastChanged)
		{
			cart.OrderId = orderId;
			cart.OrderUserId = userId;

			var order = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, cart.OrderId },
				{ Constants.FIELD_ORDER_USER_ID, userId },
				{ Constants.FIELD_ORDER_ORDER_STATUS, Constants.FLG_ORDER_ORDER_STATUS_TEMP },
				{ Constants.FIELD_ORDER_ORDER_DATE, DateTime.Now },
				{ Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, cart.Payment.PaymentId },
				{ Constants.FIELD_ORDER_ORDER_KBN, cart.OrderKbn },
				{ Constants.FIELD_ORDER_CAREER_ID, careerId },
				{ Constants.FIELD_ORDER_MOBILE_UID, mobileUid },
				{ Constants.FIELD_ORDER_REMOTE_ADDR, remoteAddr },
				{ Constants.FIELD_ORDER_ADVCODE_FIRST, advcodeFirst },
				{ Constants.FIELD_ORDER_ADVCODE_NEW, advcodeNew },
				{ Constants.FIELD_ORDER_LAST_CHANGED, lastChanged },
				{ Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS, (cart.OrderCombineParentOrderId ?? "") },
				{ Constants.FIELD_ORDER_ORDER_PRICE_REGULATION, cart.PriceRegulation },
				{
					Constants.FIELD_ORDER_MANAGEMENT_MEMO,
				GetNotFirstTimeFixedPurchaseManagementMemo(
					cart.ManagementMemo,
					cart.ProductOrderLmitOrderIds,
						cart.HasNotFirstTimeByCart)
				},
				{ Constants.FIELD_ORDER_SHIPPING_MEMO, cart.ShippingMemo ?? "" },
				{ Constants.FIELD_ORDER_RELATION_MEMO, (cart.RelationMemo ?? "") },
				{ Constants.FIELD_ORDER_RECEIPT_FLG, cart.ReceiptFlg ?? Constants.FLG_ORDER_RECEIPT_FLG_OFF },
				{ Constants.FIELD_ORDER_RECEIPT_ADDRESS, cart.ReceiptAddress ?? "" },
				{ Constants.FIELD_ORDER_RECEIPT_PROVISO, cart.ReceiptProviso ?? "" },
				{ Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID, cart.SubscriptionBoxCourseId ?? string.Empty },
				{ Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT, cart.SubscriptionBoxFixedAmount },
				{ Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT, string.IsNullOrEmpty(cart.SubscriptionBoxCourseId) ? 0 : 1 },
			};
			if (Constants.PRODUCT_ORDER_LIMIT_ENABLED)
			{
				order.Add(Constants.FIELD_ORDER_EXTEND_STATUS39,
					(cart.HasNotFirstTimeOrderIdList || cart.HasNotFirstTimeByCart)
						? Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON
						: Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF);
				order.Add(Constants.FIELD_ORDER_EXTEND_STATUS_DATE39, DateTime.Now);
			}
			order.Add(Constants.FIELD_ORDEROWNER_DISP_CURRENCY_CODE, cart.Owner.DispCurrencyCode);
			order.Add(Constants.FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID, cart.Owner.DispCurrencyLocaleId);

			if (Constants.ORDER_EXTEND_OPTION_ENABLED && (cart.OrderExtend != null))
			{
				foreach (var oe in cart.OrderExtend)
				{
					order.Add(oe.Key, oe.Value);
				}
			}

			return order;
		}

		/// <summary>
		/// 商品DataRowView取得
		/// </summary>
		/// <param name="cartProducts">カート商品一覧</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="isFixedPurchase">定期購入ありなし</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <returns>変換した文字列</returns>
		public static DataRowView GetCartProductFromDataView(
			DataView cartProducts,
			string shopId,
			string productId,
			string variationId,
			bool isFixedPurchase,
			string productSaleId)
		{
			foreach (DataRowView cartProduct in cartProducts)
			{
				if (((string)cartProduct[Constants.FIELD_CART_SHOP_ID] == shopId)
					&& (((string)cartProduct[Constants.FIELD_CART_PRODUCT_ID]).Trim() == productId.Trim())
					&& (((string)cartProduct[Constants.FIELD_CART_VARIATION_ID]).Trim() == variationId.Trim())
					&& (isFixedPurchase == ((string)cartProduct[Constants.FIELD_CART_FIXED_PURCHASE_FLG] == Constants.FLG_CART_FIXED_PURCHASE_FLG_ON))
					&& ((string)cartProduct[Constants.FIELD_CART_PRODUCTSALE_ID] == productSaleId))
				{
					return cartProduct;
				}
			}
			return null;
		}

		/// <summary>
		/// 配送種別情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送料設定ID</param>
		/// <returns>配送種別情報</returns>
		public static ShopShippingModel GetShopShipping(string shopId, string shippingId)
		{
			var shipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(shippingId);
			return shipping;
		}

		/// <summary>
		/// 利用可能決済種別情報一覧取得
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="isCheckUsablePriceMin">下限金額チェックか</param>
		/// <param name="isMultiCart">複数カートか</param>
		/// <returns>Valid payment list by user management level, gift option</returns>
		public static PaymentModel[] GetValidPaymentList(
			CartObject cart,
			string userId,
			bool isCheckUsablePriceMin = true,
			bool isMultiCart = false)
		{
			var validPaymentListAll = GetValidPaymentList(
				cart.ShopId,
				cart.PriceSubtotal,
				cart.PriceCartTotalWithoutPaymentPrice,
				cart.GetShipping().PaymentSelectionFlg,
				cart.GetShipping().PermittedPaymentIds,
				cart.HasFixedPurchase,
				cart.SetPromotions.IsPaymentChargeFree,
				isCheckUsablePriceMin);

			//注文者区分とユーザー管理レベルで利用不可決済種別を除外
			var validPaymentListOutManageLevelAndOrderOwnerKbn = CheckPaymentManagementLevelAndOrderOwnerKbnNotUse(validPaymentListAll, userId);

			var validPaymentListResult = CheckPaymentGift(validPaymentListOutManageLevelAndOrderOwnerKbn, cart.Shippings.Count);

			// Not show payment atone in list payment
			if (Constants.PAYMENT_ATONEOPTION_ENABLED == false)
			{
				validPaymentListResult = validPaymentListResult
					.Where(data => data.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					.ToArray();
			}

			// Not show payment atone in list aftee
			if (Constants.PAYMENT_AFTEEOPTION_ENABLED == false)
			{
				validPaymentListResult = validPaymentListResult
					.Where(data => data.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					.ToArray();
			}

			// Not show payment line pay in list payment
			if (Constants.PAYMENT_LINEPAY_OPTION_ENABLED == false)
			{
				validPaymentListResult = validPaymentListResult
					.Where(data => (data.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY))
					.ToArray();
			}

			// Not show payment np after pay in list payment
			if ((Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED == false)
				|| cart.IsDigitalContentsOnly)
			{
				validPaymentListResult = validPaymentListResult
					.Where(data => (data.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY))
					.ToArray();
			}

			// Not show payment EcPay in list payment
			if ((Constants.ECPAY_PAYMENT_OPTION_ENABLED == false)
				|| cart.HasFixedPurchase
				|| isMultiCart
				|| cart.HasDigitalContents)
			{
				validPaymentListResult = validPaymentListResult
					.Where(data => (data.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY))
					.ToArray();
			}

			// 台湾ファミマコンビニ受取の配送サービス利用の場合のみコンビニ受取払いを表示
			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
				&& (cart.GetShipping().DeliveryCompanyId != Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID))
			{
				validPaymentListResult = validPaymentListResult
					.Where(data => (data.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE))
					.ToArray();
			}

			// Not Show Payment NewebPay In List Payment
			if ((Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED == false)
				|| cart.HasFixedPurchase
				|| isMultiCart
				|| cart.HasDigitalContents)
			{
				validPaymentListResult = validPaymentListResult
					.Where(data => (data.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
					.ToArray();
			}

			// Not show payment PayPay SBPS in list payment
			if ((Constants.PAYMENT_PAYPAYOPTION_ENABLED == false)
				|| (cart.HasFixedPurchase
					&& (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.SBPS)))
			{
				validPaymentListResult = validPaymentListResult
					.Where(data => (data.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY))
					.ToArray();
			}

			// Not show payment boku in list payment
			if (Constants.PAYMENT_BOKU_OPTION_ENABLED == false)
			{
				validPaymentListResult = validPaymentListResult
					.Where(data => (data.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
					.ToArray();
			}

			// GMOアトカラ無効化
			if ((Constants.PAYMENT_GMOATOKARA_ENABLED == false) || Constants.GLOBAL_OPTION_ENABLE)
			{
				validPaymentListResult = validPaymentListResult
					.Where(data => (data.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA))
					.ToArray();
			}

			if (PaygentUtility.CanUsePaidyPayment(cart) == false)
			{
				validPaymentListResult = validPaymentListResult
					.Where(data => data.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					.ToArray();
			}

			if (PaygentUtility.CanUseBanknetPayment(cart) == false)
			{
				validPaymentListResult = validPaymentListResult
					.Where(data => data.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET)
					.ToArray();
			}

			if (PaygentUtility.CanUseAtmPayment(cart) == false)
			{
				validPaymentListResult = validPaymentListResult
					.Where(data => data.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_ATM)
					.ToArray();
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// 翻訳情報取得
				var paymentIds = validPaymentListResult.Select(payment => payment.PaymentId).ToList();
				var searchCondition = new NameTranslationSettingSearchCondition
				{
					DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
					MasterId1List = paymentIds,
					LanguageCode = RegionManager.GetInstance().Region.LanguageCode,
					LanguageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId,
				};
				var translationSettings =
					DomainFacade.Instance.NameTranslationSettingService.GetTranslationSettingsByMultipleMasterId1(searchCondition);

				NameTranslationCommon.SetTranslationDataToModel(
					validPaymentListResult,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
					translationSettings);
			}
			else
			{
				// Delete Payment Global
				validPaymentListResult = DeletePaymentGlobal(validPaymentListResult);
			}
			return validPaymentListResult;
		}

		/// <summary>
		/// 利用可能決済種別情報一覧取得(注文データ用)
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <param name="paymentSelectionFlg">決済選択の任意利用フラグ</param>
		/// <param name="permittedPaymentIds">決済選択の可能リスト</param>
		/// <param name="userId">ユーザID</param>
		/// <returns>利用可能決済種別一覧</returns>
		public static PaymentModel[] GetValidPaymentListForOrder(
			OrderModel order,
			string paymentSelectionFlg,
			string permittedPaymentIds,
			string userId)
		{
			if ((Constants.PAYMENT_AFTEEOPTION_ENABLED == false)
				&& permittedPaymentIds.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE))
			{
				permittedPaymentIds =
					permittedPaymentIds.Replace(Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE, string.Empty);
			}

			if ((Constants.PAYMENT_ATONEOPTION_ENABLED == false)
				&& permittedPaymentIds.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_ATONE))
			{
				permittedPaymentIds =
					permittedPaymentIds.Replace(Constants.FLG_PAYMENT_PAYMENT_ID_ATONE, string.Empty);
			}

			if ((Constants.PAYMENT_LINEPAY_OPTION_ENABLED == false)
				&& permittedPaymentIds.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY))
			{
				permittedPaymentIds = permittedPaymentIds.Replace(Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY, string.Empty);
			}

			// Not show payment np after pay in list payment
			if (((Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED == false)
					|| order.IsDigitalContents)
				&& permittedPaymentIds.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY))
			{
				permittedPaymentIds = permittedPaymentIds.Replace(Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY, string.Empty);
			}

			if ((Constants.ECPAY_PAYMENT_OPTION_ENABLED == false)
				&& permittedPaymentIds.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				|| (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY))
			{
				permittedPaymentIds = permittedPaymentIds.Replace(Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY, string.Empty);
			}

			if ((Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED == false)
				&& permittedPaymentIds.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				|| (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
			{
				permittedPaymentIds = permittedPaymentIds.Replace(
					Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY,
					string.Empty);
			}

			// Not show payment PayPay in list payment
			if ((Constants.PAYMENT_PAYPAYOPTION_ENABLED == false)
				&& permittedPaymentIds.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY))
			{
				permittedPaymentIds = permittedPaymentIds.Replace(Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY, string.Empty);
			}

			if ((Constants.PAYMENT_BOKU_OPTION_ENABLED == false)
				&& permittedPaymentIds.Contains(Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
			{
				permittedPaymentIds = permittedPaymentIds.Replace(
					Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU,
					string.Empty);
			}

			var validPaymentListAll = GetValidPaymentList(
				order.ShopId,
				order.OrderPriceSubtotal,
				GetPriceCartTotalWithoutPaymentPrice(order),
				paymentSelectionFlg,
				permittedPaymentIds,
				order.IsContainPaymentChargeFreeSet,
				string.IsNullOrEmpty(order.FixedPurchaseId) == false);

			//注文者区分とユーザー管理レベルで利用不可決済種別を除外
			var validPaymentListOutManageLevelAndOrderOwnerKbn = CheckPaymentManagementLevelAndOrderOwnerKbnNotUse(validPaymentListAll, userId);

			var validPaymentListResult = CheckPaymentGift(validPaymentListOutManageLevelAndOrderOwnerKbn, order.Shippings.Length);

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// 翻訳情報取得
				var paymentIds = validPaymentListResult.Select(payment => payment.PaymentId).ToList();
				var searchCondition = new NameTranslationSettingSearchCondition()
				{
					DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
					MasterId1List = paymentIds,
					LanguageCode = RegionManager.GetInstance().Region.LanguageCode,
					LanguageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId,
				};
				var translationSettings =
					new NameTranslationSettingService().GetTranslationSettingsByMultipleMasterId1(searchCondition);

				NameTranslationCommon.SetTranslationDataToModel(
					validPaymentListResult,
					Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
					translationSettings);
			}
			else
			{
				// Delete Payment Global
				validPaymentListResult = DeletePaymentGlobal(validPaymentListResult);
			}
			return validPaymentListResult;
		}

		/// <summary>
		/// 利用可能決済種別情報一覧に利用できないユーザ管理レベルIDと注文者区分のものは除外し返す
		/// </summary>
		/// <param name="validPaymentList">利用可能決済種別情報一覧</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>除外した一覧</returns>
		public static PaymentModel[] CheckPaymentManagementLevelAndOrderOwnerKbnNotUse(PaymentModel[] validPaymentList, string userId)
		{
			if ((validPaymentList.Length == 0)) return validPaymentList;

			var userKbn = Constants.FLG_USER_USER_KBN_ALL_GUEST;
			var userManagementLevelId = string.Empty;

			if (string.IsNullOrEmpty(userId) == false)
			{
				var userInfo = DomainFacade.Instance.UserService.Get(userId);
				if (userInfo == null)
				{
					userManagementLevelId = string.Empty;
				}
				else
				{
					userKbn = userInfo.UserKbn;
					userManagementLevelId = userInfo.UserManagementLevelId;
				}
			}

			//注文者区分で使用不可決済方法を除外
			var resultDataView = CheckPaymentOrderOwnerKbnNotUse(validPaymentList, userKbn);

			//ユーザ管理レベルIDで使用不可決済方法を除外
			resultDataView = CheckPaymentUserManagementLevelNotUse(resultDataView, userManagementLevelId);

			return resultDataView;
		}

		/// <summary>
		/// 利用可能決済種別情報一覧に利用できない注文者区分を除外し返す
		/// </summary>
		/// <param name="validPaymentList">利用可能決済種別情報一覧</param>
		/// <param name="userKbn">注文者区分</param>
		/// <returns>除外した一覧</returns>
		private static PaymentModel[] CheckPaymentOrderOwnerKbnNotUse(PaymentModel[] validPaymentList, string userKbn)
		{
			var allOwnerKbnString = UserService.IsUser(userKbn)
				? Constants.FLG_USER_USER_KBN_ALL_USER
				: Constants.FLG_USER_USER_KBN_ALL_GUEST;

			var resultPaymentList = new List<PaymentModel>();
			foreach (var validPayment in validPaymentList)
			{
				// 使用者不可注文者区分内にuserKbnと同じ分類になる全てのユーザー、全てのゲストがあるかチェック
				// ある場合はスキップ
				var orderOwnerKbnList = validPayment.OrderOwnerKbnNotUse.Split(',');
				if (orderOwnerKbnList.Any(orderOwnerKbn => (orderOwnerKbn == allOwnerKbnString)))
					continue;

				// ない場合は使用不可注文者にuserKbnと一致する項目があるかチェックし、決済種別を取得
				if (orderOwnerKbnList.Contains(userKbn) == false)
					resultPaymentList.Add(validPayment);
			}

			return resultPaymentList.ToArray();
		}

		/// <summary>
		/// 利用可能決済種別情報一覧に利用できないユーザ管理レベルIDのものは除外し返す
		/// </summary>
		/// <param name="validPaymentList">利用可能決済種別情報一覧</param>
		/// <param name="userManagementLevelId">ユーザ管理レベルID</param>
		/// <returns>除外した一覧</returns>
		private static PaymentModel[] CheckPaymentUserManagementLevelNotUse(PaymentModel[] validPaymentList, string userManagementLevelId)
		{
			if (userManagementLevelId == string.Empty) return validPaymentList;
			var resultDataView = validPaymentList.Where(
				payment => payment.UserManagementLevelNotUse.Split(',')
					.Where(userManagementLevel => (userManagementLevel != string.Empty))
					.Contains(userManagementLevelId) == false).ToArray();

			return resultDataView;
		}

		/// <summary>
		/// Delete Payment Global
		/// </summary>
		/// <param name="validPaymentList">Valid Payment List</param>
		/// <returns>Payment Included</returns>
		private static PaymentModel[] DeletePaymentGlobal(PaymentModel[] validPaymentList)
		{
			var paymentIds = new[]
			{
				Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY,
				Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY,
				Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY,
			};
			var result = validPaymentList
				.Where(validPayment => (paymentIds.Contains(validPayment.PaymentId) == false))
				.ToArray();
			return result;
		}

		/// <summary>
		/// 利用可能決済種別情報一覧にギフトで配送先が複数ある場合は代引きを不可にし返す
		/// </summary>
		/// <param name="validPaymentList">利用可能決済種別情報一覧</param>
		/// <param name="shippingCount">配送先数</param>
		/// <returns>除外した一覧</returns>
		private static PaymentModel[] CheckPaymentGift(PaymentModel[] validPaymentList, int shippingCount)
		{
			// ギフト注文で配送先が複数ある場合は代引き不可
			if (Constants.GIFTORDER_OPTION_ENABLED && (shippingCount > 1))
			{
				var resultDataView = validPaymentList
					.Where(payment => (payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT))
					.ToArray();
				return resultDataView;
			}
			return validPaymentList;
		}

		/// <summary>
		/// 利用可能決済種別情報一覧取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="cartPriceSubtotal">カート内容商品小計</param>
		/// <param name="cartPriceTotalWithoutPayment">カート合計金額</param>
		/// <param name="paymentSelectionFlg">決済種別任意利用</param>
		/// <param name="permittedPaymentIds">決済種別リスト</param>
		/// <param name="hasFixedPurchase">定期購入あり？</param>
		/// <param name="isPaymentChargeFree">決済手数料無料フラグ</param>
		/// <param name="isCheckUsablePriceMin">下限金額チェックか</param>
		/// <returns>決済手数料情報</returns>
		public static PaymentModel[] GetValidPaymentList(
			string shopId,
			decimal cartPriceSubtotal,
			decimal cartPriceTotalWithoutPayment,
			string paymentSelectionFlg,
			string permittedPaymentIds,
			bool hasFixedPurchase,
			bool isPaymentChargeFree,
			bool isCheckUsablePriceMin = true)
		{
			var permittedPaymentIdList = string.Join(",", permittedPaymentIds);
			var isPaymentSelection = (paymentSelectionFlg == Constants.FLG_SHOPSHIPPING_PAYMENT_SELECTION_FLG_VALID);
			var restrictFixedPurchasePayment = (hasFixedPurchase);

			var paymentList = DataCacheControllerFacade.GetPaymentCacheController().GetValidAllWithPrice();

			// 配送種別・定期購入等で制限されている決済種別を除外
			paymentList = paymentList.Where(payment =>
				(((payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
					|| (cartPriceTotalWithoutPayment == 0))
				&& ((isPaymentSelection == false)
					|| permittedPaymentIdList.Contains(payment.PaymentId))
				&& ((restrictFixedPurchasePayment == false)
					|| (Constants.CAN_FIXEDPURCHASE_PAYMENTIDS.Contains(payment.PaymentId)))
				&& (payment.PaymentId.StartsWith("M") == false)
				&& (Constants.AMAZON_PAYMENT_OPTION_ENABLED
					|| payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				&& ((payment.MobileDispFlg == Constants.FLG_PAYMENT_MOBILE_DISP_FLG_BOTH_PC_AND_MOBILE)
					|| (payment.MobileDispFlg == Constants.FLG_PAYMENT_MOBILE_DISP_FLG_PC))
				&& (Constants.ECPAY_PAYMENT_OPTION_ENABLED
					|| (payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY))
				&& (Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED
					|| (payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
				&& (Constants.PAYMENT_PAYPAYOPTION_ENABLED
					|| (payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)))).ToArray();

			// AmazonPayCV1CV2切り替え
			paymentList = paymentList.Where(p =>
				(p.PaymentId != (Constants.AMAZON_PAYMENT_CV2_ENABLED
					? Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT
					: Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2))).ToArray();

			// Disable GMO payment if config == false
			if (Constants.PAYMENT_GMO_POST_ENABLED == false)
			{
				paymentList = paymentList.Where(p =>
					(p.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
					&& (p.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)).ToArray();
			}

			// 決済手数料を含めた合計価格に対して適用可能な決済種別を取得
			paymentList = paymentList
				.Where(payment => (cartPriceTotalWithoutPayment == 0)
					? payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT
					: payment.PriceList.Any(priceInfo => IsPaymentPriceInRange(
						cartPriceTotalWithoutPayment,
						isPaymentChargeFree,
						isCheckUsablePriceMin,
						priceInfo,
						payment)))
				.OrderBy(x => x.DisplayOrder)
			.ToArray();

			var validPaymentList = CurrencyManager.RemovePaymentsByKeyCurrency(paymentList);
			return validPaymentList;
		}

		/// <summary>
		/// カート合計金額が利用可能金額の範囲内であるか
		/// </summary>
		/// <param name="cartPriceTotalWithoutPayment">カート合計金額（決済手数料を除く）</param>
		/// <param name="isPaymentChargeFree">決済手数料無料フラグ</param>
		/// <param name="isCheckUsablePriceMin">下限金額チェックか</param>
		/// <param name="paymentPriceInfo">決済手数料情報</param>
		/// <param name="paymentInfo">決済種別情報</param>
		/// <returns>true：利用可能金額の範囲内、false：利用可能金額の範囲外</returns>
		private static bool IsPaymentPriceInRange(
			decimal cartPriceTotalWithoutPayment,
			bool isPaymentChargeFree,
			bool isCheckUsablePriceMin,
			PaymentPriceModel paymentPriceInfo,
			PaymentModel paymentInfo)
		{
			var priceTotalWithPayment = isPaymentChargeFree
				? cartPriceTotalWithoutPayment
	: cartPriceTotalWithoutPayment + paymentPriceInfo.PaymentPrice;

			var isPriceInMaxRange = ((paymentInfo.UsablePriceMax.HasValue == false)
				|| (priceTotalWithPayment <= paymentInfo.UsablePriceMax));
			var isPriceInMinRange = ((isCheckUsablePriceMin == false)
				|| ((paymentInfo.UsablePriceMin.HasValue == false) || (priceTotalWithPayment >= paymentInfo.UsablePriceMin)));
			var result = (isPriceInMaxRange && isPriceInMinRange);
			return result;
		}

		/// <summary>
		/// 決済情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>決済手数料情報</returns>
		/// <remarks>基本的に取得できるはずなのでDataRowView取得としている</remarks>
		public static DataRowView GetPayment(string shopId, string paymentId)
		{
			DataView payment;
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("Order", "GetPayment"))
			{
				var htInput = new Hashtable
				{
					{ Constants.FIELD_PAYMENT_SHOP_ID, shopId },
					{ Constants.FIELD_PAYMENT_PAYMENT_ID, paymentId }
				};

				payment = statement.SelectSingleStatementWithOC(accessor, htInput);
			}

			return (payment.Count != 0) ? payment[0] : null;
		}

		/// <summary>
		/// 決済手数料取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="priceSubtotal">商品合計金額</param>
		/// <param name="priceTotalWithoutPaymentPrice">決済手数料を除いた合計金額</param>
		/// <returns>決済手数料</returns>
		public static decimal GetPaymentPrice(
			string shopId,
			string paymentId,
			decimal priceSubtotal,
			decimal priceTotalWithoutPaymentPrice)
		{
			var paymentPrice = GetPaymentPriceInfo(shopId,
				paymentId,
				priceSubtotal,
				priceTotalWithoutPaymentPrice);

			var result = (paymentPrice != null)
				? paymentPrice.PaymentPrice
				: 0m;

			return result;
		}

		/// <summary>
		/// 決済手数料情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="priceSubtotal">商品合計金額</param>
		/// <param name="priceTotalWithoutPaymentPrice">決済手数料を除いた合計金額</param>
		/// <returns>決済手数料情報</returns>
		public static PaymentPriceModel GetPaymentPriceInfo(
			string shopId,
			string paymentId,
			decimal priceSubtotal,
			decimal priceTotalWithoutPaymentPrice)
		{
			var payment = DomainFacade.Instance.PaymentService.Get(shopId, paymentId);
			if (payment == null) return null;
			if (payment.PaymentPriceKbn == Constants.FLG_PAYMENT_PAYMENT_PRICE_KBN_SINGULAR) return payment.PriceList.FirstOrDefault();

			var targetPrice = Constants.CALCULATE_PAYMENT_PRICE_ONLY_SUBTOTAL
				? priceSubtotal
			: priceTotalWithoutPaymentPrice;
			return payment.PriceList.FirstOrDefault(
				price => ((price.TgtPriceBgn <= targetPrice) && (price.TgtPriceEnd >= targetPrice)));
		}

		/// <summary>
		/// Get items has limited payment in valid payment
		/// </summary>
		/// <param name="cart">The cart object</param>
		/// <param name="validPayments">The valid payment ids</param>
		/// <returns> The items has limited payment</returns>
		public static List<string> GetItemsHasLimitedPayment(CartObject cart, List<string> validPayments)
		{
			var productNameInvalid = new List<string>();

			if ((cart.Payment == null) && (validPayments.Count == 0)) return productNameInvalid;

			IEnumerable<DataRowView> productsHasLimitedPayment = null;
			var productsForCart = cart.GetCartProductsEither();

			if (validPayments.Count > 0)
			{
				// Find product has set limit payment id in [validPayments]
				productsHasLimitedPayment = productsForCart.Cast<DataRowView>()
					.Where(item => item[Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS].ToString().Split(',').Intersect(validPayments).Any());
			}
			else if ((cart.Payment != null) && (string.IsNullOrEmpty(cart.Payment.PaymentId) == false))
			{
				// Find product has set limit payment id contains payment of cart
				productsHasLimitedPayment = productsForCart.Cast<DataRowView>()
					.Where(product => product[Constants.FIELD_PRODUCT_LIMITED_PAYMENT_IDS].ToString().Contains(cart.Payment.PaymentId));
			}

			// Get products name has limited payment
			if ((productsHasLimitedPayment != null) && productsHasLimitedPayment.Any())
			{
				productNameInvalid = productsHasLimitedPayment.Select(ProductCommon.CreateProductJointName).Distinct().ToList();
			}

			return productNameInvalid;
		}

		/// <summary>
		/// Get the limited payments messages for cart.
		/// </summary>
		/// <param name="cart">The cart.</param>
		/// <param name="paymentValidList">The payment valid list.</param>
		/// <returns>The limited payments messages for cart</returns>
		public static string GetLimitedPaymentsMessagesForCart(CartObject cart, PaymentModel[] paymentValidList)
		{
			var paymentIdValid = paymentValidList
				.Select(payment => payment.PaymentId.ToString()).ToList();
			var itemHasLimitedPayment = GetItemsHasLimitedPayment(cart, paymentIdValid);

			if (itemHasLimitedPayment.Any())
			{
				return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PRODUCT_LIMITED_PAYMENT)
					.Replace("@@ 1 @@", String.Join(",", itemHasLimitedPayment.Distinct()));
			}

			return string.Empty;
		}

		/// <summary>
		/// Get Limited Payments
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="limitedPaymentIds">Limited Payment Ids</param>
		/// <returns>Payments Information</returns>
		public static PaymentModel[] GetLimitedPayments(string shopId, string[] limitedPaymentIds)
		{
			if (limitedPaymentIds.Any() == false) return new PaymentModel[] { };

			var result = new PaymentService().GetValidAll(shopId)
				.Where(p => limitedPaymentIds.Any(lp => lp == p.PaymentId))
				.ToArray();

			return result;
		}

		/// <summary>
		/// Get the payments un limit by product.
		/// </summary>
		/// <param name="cart">The cart.</param>
		/// <param name="paymentList">The payment list.</param>
		/// <returns>The payments un limit by product</returns>
		public static PaymentModel[] GetPaymentsUnLimitByProduct(CartObject cart, PaymentModel[] paymentList)
		{
			if (paymentList.Length == 0) return paymentList;

			var productInfo = DomainFacade.Instance.ProductService.GetCartProducts(cart.CartId);

			// DBに商品情報がない場合、引数のカートを参照する
			var limitedPayments = (productInfo.Length == 0)
				? cart.Items
					.SelectMany(product => product.LimitedPaymentIds)
					.Distinct()
					.ToArray()
				: productInfo
					.SelectMany(product =>
						product.LimitedPaymentIds
							.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
					.Distinct()
					.ToArray();

			var result = (limitedPayments.Any() == false)
				? paymentList
				: paymentList
					.Where(item => limitedPayments.Contains(item.PaymentId) == false)
					.ToArray();

			return result;
		}

		/// <summary>
		/// 決済金額範囲検証
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="paymentKbn">支払い区分</param>
		/// <param name="message">メッセージ</param>
		/// <returns>決済可否</returns>
		public static bool ValidatePaymentPriceRange(CartObject cart, string paymentKbn, out string message)
		{
			message = CheckPaymentPriceEnabled(cart, paymentKbn);
			return message == "";
		}

		/// <summary>
		/// 決済情報金額範囲チェック
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>エラーメッセージリスト</returns>
		public static string CheckPaymentPriceEnabled(CartObject cart, string paymentId)
		{
			// 「決済無し」チェック
			if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
			{
				if (cart.PriceCartTotalWithoutPaymentPrice != 0)
				{
					return GetErrorMessage(OrderErrorcode.PaymentUsablePriceOverUnselectableError, CurrencyManager.ToPrice(1m) + " 以上");
				}
			}
			else if (cart.PriceCartTotalWithoutPaymentPrice == 0)
			{
				// (決済手数料を含まない)カート支払金額が0円の場合は「決済無し」を選択させたい
				return GetErrorMessage(OrderErrorcode.PaymentUsablePriceOverUnselectableError, CurrencyManager.ToPrice(0m));
			}
			else
			{
				var payment = DomainFacade.Instance.PaymentService.Get(cart.ShopId, paymentId);

				if (payment == null) return string.Empty;
				// 金額範囲チェック
				var priceMin = payment.UsablePriceMin;
				var priceMax = payment.UsablePriceMax;
				if (((priceMin.HasValue) && (cart.PriceTotal < priceMin.Value))
					|| ((priceMax.HasValue) && (cart.PriceTotal > priceMax.Value)))
				{
					return GetErrorMessage(
						OrderErrorcode.PaymentUsablePriceOutOfRangeError,
						CurrencyManager.ToPrice(cart.PriceTotal),
						CurrencyManager.ToPrice(priceMin),
						CurrencyManager.ToPrice(priceMax));
				}
			}
			return "";
		}

		#region +UpdateOrderStatus 注文ステータスのUPDATE処理とシリアルキーの引き渡し処理 [フロント用]
		/// <summary>
		/// 注文情報での決済情報金額範囲チェック
		/// </summary>
		/// <param name="order">カート</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>エラーメッセージリスト</returns>
		public static string CheckPaymentPriceEnabledForOrder(OrderModel order, string paymentId)
		{
			var orderPriceTotalWithoutPayment = OrderCommon.GetPriceCartTotalWithoutPaymentPrice(order);

			// 「決済無し」チェック
			if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)
			{
				if (orderPriceTotalWithoutPayment != 0)
				{
					return GetErrorMessage(OrderErrorcode.PaymentUsablePriceOverUnselectableError, CurrencyManager.ToPrice(1m) + " 以上");
				}
			}
			else if (orderPriceTotalWithoutPayment == 0)
			{
				// (決済手数料を含まない)カート支払金額が0円の場合は「決済無し」を選択させたい
				return GetErrorMessage(OrderErrorcode.PaymentUsablePriceOverUnselectableError, CurrencyManager.ToPrice(0m));
			}
			else
			{
				var payment = GetPayment(order.ShopId, paymentId);

				// 金額範囲チェック
				var priceMin = payment[Constants.FIELD_PAYMENT_USABLE_PRICE_MIN];
				var priceMax = payment[Constants.FIELD_PAYMENT_USABLE_PRICE_MAX];
				if (((priceMin != DBNull.Value) && (order.OrderPriceTotal < (decimal)priceMin))
					|| ((priceMax != DBNull.Value) && (order.OrderPriceTotal > (decimal)priceMax)))
				{
					return GetErrorMessage(
						OrderErrorcode.PaymentUsablePriceOutOfRangeError,
						StringUtility.ToPrice(order.OrderPriceTotal),
						StringUtility.ToPrice(priceMin),
						StringUtility.ToPrice(priceMax));
				}
			}
			return "";
		}

		/// <summary>
		/// 支払回数更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="installmentsCode">支払回数コード</param>
		/// <param name="installments">支払回数名称</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		public static int UpdateOrderInstallmentsCode(
			string orderId,
			string installmentsCode,
			string installments,
			UpdateHistoryAction updateHistoryAction)
		{
			var updated = new OrderService().UpdateOrderInstallmentsCode(orderId, installmentsCode, installments, updateHistoryAction);
			return updated;
		}

		/// <summary>
		/// 注文ステータスのUPDATE処理とシリアルキーの引き渡し処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="isExternalPayment">外部決済あり？</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="isExternalPaymentAuthResultHold">外部決済で与信結果がHOLDなのか</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		/// <remarks>注文ステータスを仮注文から注文済みに更新</remarks>
		/// <remarks>入金済みなら、シリアルキーステータスを引当済みから引渡済みに更新</remarks>
		/// <remarks>入金済みなら、注文ステータスを出荷完了に更新</remarks>
		public static int UpdateForOrderComplete(
			Hashtable order,
			CartObject cart,
			bool isExternalPayment,
			UpdateHistoryAction updateHistoryAction,
			bool isExternalPaymentAuthResultHold = false,
			SqlAccessor accessor = null)
		{
			// 更新
			var updated = UpdateForOrderComplete(order, cart, isExternalPayment, isExternalPaymentAuthResultHold, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					StringUtility.ToEmpty(order[Constants.FIELD_ORDER_LAST_CHANGED]),
					accessor);
			}
			return updated;
		}

		/// <summary>
		/// 注文ステータスのUPDATE処理とシリアルキーの引き渡し処理 [フロント用]
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="isExternalPayment">外部決済あり？</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		/// <remarks>注文ステータスを仮注文から注文済みに更新</remarks>
		/// <remarks>入金済みなら、シリアルキーステータスを引当済みから引渡済みに更新</remarks>
		/// <remarks>入金済みなら、注文ステータスを出荷完了に更新</remarks>
		public static int UpdateOrderStatus(
			Hashtable order,
			CartObject cart,
			bool isExternalPayment,
			UpdateHistoryAction updateHistoryAction = UpdateHistoryAction.DoNotInsert,
			SqlAccessor accessor = null)
		{
			var updated = UpdateOrderStatus(
				(string)order[Constants.FIELD_ORDER_ORDER_ID],
				cart,
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_LAST_CHANGED]),
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_ID]),
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_PASS]),
				(order.Contains(Constants.FIELD_ORDER_CREDIT_BRANCH_NO) && (order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] != DBNull.Value))
					? (int?)order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]
					: null,
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_MEMO]),
				(string)order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS],
				StringUtility.ToEmpty((string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]),
				StringUtility.ToEmpty((string)order[Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS]),
				isExternalPayment);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					StringUtility.ToEmpty(order[Constants.FIELD_ORDER_LAST_CHANGED]),
					accessor);
			}
			return updated;
		}
		#endregion

		#region +UpdateForOrderComplete 注文ステータスのUPDATE処理とシリアルキーの引き渡し処理
		/// <summary>
		/// 注文ステータスのUPDATE処理とシリアルキーの引き渡し処理
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="orderTranId">決済取引ID</param>
		/// <param name="creditBrunchNo">登録クレジットカード枝番（カードでない場合はnull）</param>
		/// <param name="paymentMemo">決済メモ（指定しない場合はnull）</param>
		/// <param name="orderPaymentStatus">入金ステータス（指定しない場合はnull)</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="isExternalPayment">外部決済あり？</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="timeout">注文ステータス更新のタイムアウト(秒)</param>
		/// <returns>更新結果</returns>
		/// <remarks>注文ステータスを仮注文から注文済みに更新</remarks>
		/// <remarks>入金済みなら、シリアルキーステータスを引当済みから引渡済みに更新</remarks>
		/// <remarks>入金済みなら、注文ステータスを出荷完了に更新</remarks>
		public static int UpdateForOrderComplete(
			string orderId,
			CartObject cart,
			string lastChanged,
			string orderTranId,
			int? creditBrunchNo,
			string paymentMemo,
			string orderPaymentStatus,
			string paymentOrderId,
			string onlinePaymentStatus,
			bool isExternalPayment,
			UpdateHistoryAction updateHistoryAction,
			int? timeout = null)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				var updated = UpdateForOrderComplete(
					orderId,
					cart,
					lastChanged,
					orderTranId,
					creditBrunchNo,
					paymentMemo,
					orderPaymentStatus,
					paymentOrderId,
					onlinePaymentStatus,
					isExternalPayment,
					updateHistoryAction,
					accessor,
					timeout);
				return updated;
			}
		}
		#endregion

		#region +UpdateForOrderComplete 注文ステータスのUPDATE処理とシリアルキーの引き渡し処理
		/// <summary>
		/// 注文ステータスのUPDATE処理とシリアルキーの引き渡し処理
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="orderTranId">決済取引ID</param>
		/// <param name="creditBrunchNo">登録クレジットカード枝番（カードでない場合はnull）</param>
		/// <param name="paymentMemo">決済メモ（指定しない場合はnull）</param>
		/// <param name="orderPaymentStatus">入金ステータス（指定しない場合はnull)</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="isExternalPayment">外部決済あり？</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="timeout">注文ステータス更新のタイムアウト(秒)</param>
		/// <returns>更新結果</returns>
		/// <remarks>注文ステータスを仮注文から注文済みに更新</remarks>
		/// <remarks>入金済みなら、シリアルキーステータスを引当済みから引渡済みに更新</remarks>
		/// <remarks>入金済みなら、注文ステータスを出荷完了に更新</remarks>
		public static int UpdateForOrderComplete(
			string orderId,
			CartObject cart,
			string lastChanged,
			string orderTranId,
			int? creditBrunchNo,
			string paymentMemo,
			string orderPaymentStatus,
			string paymentOrderId,
			string onlinePaymentStatus,
			bool isExternalPayment,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			int? timeout = null)
		{
			// 更新
			var updated = UpdateForOrderComplete(
				orderId,
				cart,
				cart.Payment.PaymentId,
				lastChanged,
				orderTranId,
				creditBrunchNo,
				paymentMemo,
				orderPaymentStatus,
				paymentOrderId,
				onlinePaymentStatus,
				isExternalPayment,
				accessor,
				timeout);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion

		#region -UpdateForOrderComplete 注文ステータスのUPDATE処理とシリアルキーの引き渡し処理
		/// <summary>
		/// 注文ステータスのUPDATE処理とシリアルキーの引き渡し処理 [フロント用]
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="isExternalPayment">外部決済あり？</param>
		/// <param name="isExternalPaymentAuthResultHold">外部決済で与信結果がHOLDなのか</param>
		/// <returns>処理件数</returns>
		/// <remarks>注文ステータスを仮注文から注文済みに更新</remarks>
		/// <remarks>入金済みなら、シリアルキーステータスを引当済みから引渡済みに更新</remarks>
		/// <remarks>入金済みなら、注文ステータスを出荷完了に更新</remarks>
		private static int UpdateForOrderComplete(
			Hashtable order,
			CartObject cart,
			bool isExternalPayment,
			bool isExternalPaymentAuthResultHold = false)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				var result = UpdateForOrderComplete(
					order,
					cart,
					isExternalPayment,
					isExternalPaymentAuthResultHold,
					accessor);
				return result;
			}
		}

		/// <summary>
		/// 注文ステータスのUPDATE処理とシリアルキーの引き渡し処理 [フロント用]
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="isExternalPayment">外部決済あり？</param>
		/// <param name="isExternalPaymentAuthResultHold">外部決済で与信結果がHOLDなのか</param>
		/// <returns>処理件数</returns>
		/// <remarks>注文ステータスを仮注文から注文済みに更新</remarks>
		/// <remarks>入金済みなら、シリアルキーステータスを引当済みから引渡済みに更新</remarks>
		/// <remarks>入金済みなら、注文ステータスを出荷完了に更新</remarks>
		private static int UpdateForOrderComplete(
			Hashtable order,
			CartObject cart,
			bool isExternalPayment,
			bool isExternalPaymentAuthResultHold = false,
			SqlAccessor accessor = null)
		{
			var externalAuthDate =
				(order[Constants.FLG_ORDER_PAYMENT_API_SKIP] != null) && (bool)order[Constants.FLG_ORDER_PAYMENT_API_SKIP]
				? (DateTime?)order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_AUTH_DATE]
				: null;
			var result = UpdateForOrderComplete(
				(string)order[Constants.FIELD_ORDER_ORDER_ID],
				cart,
				cart.Payment.PaymentId,
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_LAST_CHANGED]),
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CARD_TRAN_ID]),
				(order.Contains(Constants.FIELD_ORDER_CREDIT_BRANCH_NO)
						&& (order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO] != DBNull.Value))
					? (int?)order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]
					: null,
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_MEMO]),
				(string)order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS],
				StringUtility.ToEmpty((string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]),
				StringUtility.ToEmpty((string)order[Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS]),
				isExternalPayment,
				accessor,
				externalAuthDate: externalAuthDate,
				isExternalPaymentAuthResultHold: isExternalPaymentAuthResultHold,
				creditStatus: (string)order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS]);
			return result;
		}

		/// <summary>
		/// 注文ステータスのUPDATE処理とシリアルキーの引き渡し処理
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="orderTranId">決済取引ID</param>
		/// <param name="creditBrunchNo">登録クレジットカード枝番（カードでない場合はnull）</param>
		/// <param name="paymentMemo">決済メモ（指定しない場合はnull）</param>
		/// <param name="orderPaymentStatus">入金ステータス（指定しない場合はnull)</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="isExternalPayment">外部決済あり？</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="timeout">注文ステータス更新のタイムアウト(秒)</param>
		/// <param name="externalAuthDate">与信日時</param>
		/// <param name="isExternalPaymentAuthResultHold">外部決済で与信結果がHOLDなのか</param>
		/// <param name="creditStatus">与信状況</param>
		/// <returns>更新結果</returns>
		/// <remarks>注文ステータスを仮注文から注文済みに更新</remarks>
		/// <remarks>入金済みなら、シリアルキーステータスを引当済みから引渡済みに更新</remarks>
		/// <remarks>入金済みなら、注文ステータスを出荷完了に更新</remarks>
		public static int UpdateForOrderComplete(
			string orderId,
			CartObject cart,
			string paymentId,
			string lastChanged,
			string orderTranId,
			int? creditBrunchNo,
			string paymentMemo,
			string orderPaymentStatus,
			string paymentOrderId,
			string onlinePaymentStatus,
			bool isExternalPayment,
			SqlAccessor accessor,
			int? timeout = null,
			DateTime? externalAuthDate = null,
			bool isExternalPaymentAuthResultHold = false,
			string creditStatus = "")
		{
			// 注文ステータス更新
			var orderPaymentStatusIsConfirm = ((orderPaymentStatus == null)
				|| (orderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM));

			// 入金確認日時設定
			var orderPaymentDate = (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT) ? null : (DateTime?)DateTime.Now.Date;
			orderPaymentDate = (orderPaymentStatusIsConfirm) ? null : orderPaymentDate;

			// 注文ステータス確定（仮クレジットカードの場合は仮注文とする）
			var orderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDERED;
			var isPreviousCreditCardOrTempPaygentPaidy = ((paymentId == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)
				|| PaygentUtility.CheckIsPaidyPaygentPayment(paymentId));
			if (isPreviousCreditCardOrTempPaygentPaidy) orderStatus = Constants.FLG_ORDER_ORDER_STATUS_TEMP;

			var externalPaymentStatus = isExternalPayment
				? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP
				: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;

			if (isExternalPayment)
			{
				// *決済後入金ステータスを「入金済」にするか（SBPSキャリア決済、Amazon payは自分のオプション持っているので、決済方法分岐判定が必要）
				switch (paymentId)
				{
					case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT:
					case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2:
						externalPaymentStatus = Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
							: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS:
					case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS:
					case Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS:
					case Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS:
					case Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS:
					case Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_SBPS:
						externalPaymentStatus = Constants.PAYMENT_SETTING_SBPS_PAYMENT_STATUS_COMPLETE
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
							: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
						if (cart.IsDigitalContentsOnly)
						{
							externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
							onlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
						}
						else if (isExternalPayment
							&& Constants.PAYMENT_ATONE_TEMPORARYREGISTRATION_ENABLED)
						{
							externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
						}
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
						if (cart.IsDigitalContentsOnly)
						{
							externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
							onlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
						}
						else if (isExternalPayment
							&& Constants.PAYMENT_AFTEE_TEMPORARYREGISTRATION_ENABLED)
						{
							externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
						}
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
						orderStatus = Constants.FLG_ORDER_ORDER_STATUS_TEMP;
						externalPaymentStatus = (cart.Payment.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT)
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_WAIT
							: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
						orderStatus = Constants.FLG_ORDER_ORDER_STATUS_TEMP;
						externalPaymentStatus = (cart.Payment.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT)
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_WAIT
							: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
						if (isExternalPaymentAuthResultHold)
						{
							externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
						}
						else
						{
							if (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score)
							{
								externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
								break;
							}

							var paymentCvsStatusCompleteFlg = (cart.HasDigitalContents && Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
							? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
							: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;

							externalPaymentStatus = paymentCvsStatusCompleteFlg
								? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
								: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
						}
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY:
						if (Constants.PAYMENT_LINEPAY_PAYMENTCAPTURENOW)
						{
							externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
							onlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
						}
						else
						{
							var linePaymentStatusCompleteFlg = (cart.HasDigitalContents && Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
								? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
								: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;

							externalPaymentStatus = linePaymentStatusCompleteFlg
								? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
								: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
						}
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
						switch (Constants.PAYMENT_PAYPAY_KBN)
						{
							case Constants.PaymentPayPayKbn.GMO:
								externalPaymentStatus = ((Constants.PAYMENT_PAYPAY_JOB_CODE == PaypayConstants.FLG_PAYPAY_STATUS_CAPTURE)
										|| cart.HasFixedPurchase)
									? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
									: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP;
								break;

							case Constants.PaymentPayPayKbn.VeriTrans:
								externalPaymentStatus = ((Constants.PAYMENT_PAYPAY_JOB_CODE == PaypayConstants.FLG_PAYPAY_STATUS_CAPTURE)
										|| cart.HasFixedPurchase)
									? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
									: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;

								onlinePaymentStatus = ((Constants.PAYMENT_PAYPAY_JOB_CODE == PaypayConstants.FLG_PAYPAY_STATUS_CAPTURE)
										|| cart.HasFixedPurchase)
									? Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED
									: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE;
								break;
						}
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE:
						if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten)
						{
							externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
						}
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
						externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO:
					case Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE:
					case Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA:
						externalPaymentStatus = creditStatus;
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET:
					case Constants.FLG_PAYMENT_PAYMENT_ID_ATM:
						externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_UNCONFIRMED;
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY:
						if (Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
						{
							externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
						}
						break;

					case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
						// ペイジェントクレカの場合のみの処理
						if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent)
						{
							// 同時売上モードオンか
							var isPaygentCreditPaymentWithAuth = (cart.HasDigitalContents
								? (Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS == Constants.PaygentCreditCardPaymentMethod.Capture)
								: (Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD == Constants.PaygentCreditCardPaymentMethod.Capture));
							// 同時売上モードがオンの場合はこの時点で売上確定のステータス更新を行う
							if (isPaygentCreditPaymentWithAuth)
							{
								// オンライン決済ステータスを「売上確定済み」に
								onlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
								// 外部決済ステータスを「売上確定済み」に
								externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;

								// オーソリ後に入金ステータスを入金済みにするか判定
								var isPaygentCreditPaymentStatusComplete = cart.HasDigitalContents
									? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
									: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;
								// 入金ステータスを更新
								if (isPaygentCreditPaymentStatusComplete) {
									orderPaymentStatus = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE;
									orderPaymentStatusIsConfirm = false;
								}
							}
							break;
						}
						// ペイジェントクレカ以外はdefaultへ
						goto default;

					default:
						var paymentStatusCompleteFlg = (cart.HasDigitalContents && Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
							? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
							: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;

						externalPaymentStatus = paymentStatusCompleteFlg
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
							: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
						break;
				}
			}

			// 注文ステータス更新

			DateTime? inputExternalAuthDate = null;
			if (externalAuthDate != null)
			{
				inputExternalAuthDate = externalAuthDate;
			}
			else if (isExternalPayment)
			{
				inputExternalAuthDate = (DateTime?)DateTime.Now;
			}
			var updatedCount = new OrderService().UpdateOrderStatusForOrderComplete(
				orderId,
				orderStatus,
				StringUtility.ToEmpty(orderTranId),
				creditBrunchNo,
				orderPaymentStatusIsConfirm ? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM : orderPaymentStatus,
				orderPaymentDate,
				StringUtility.ToEmpty(paymentMemo),
				externalPaymentStatus,
				inputExternalAuthDate,
				lastChanged,
				StringUtility.ToEmpty(paymentOrderId),
				StringUtility.ToEmpty(onlinePaymentStatus),
				UpdateHistoryAction.DoNotInsert,
				timeout,
				accessor: accessor);

			// シリアルキー引き渡し
			if ((cart != null)
				&& cart.IsDigitalContentsOnly
				&& (orderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE))
			{
				DeliverSerialKeyForOrderComplete(orderId, lastChanged);

				foreach (var cp in cart.Items)
				{
					cp.IsDelivered = true;
				}
			}
			return updatedCount;
		}

		/// <summary>
		/// 注文完了向けシリアルキー引き渡し
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		public static void DeliverSerialKeyForOrderComplete(string orderId, string lastChanged)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				DeliverSerialKeyForOrderComplete(orderId, lastChanged, accessor);
			}
		}
		/// <summary>
		/// 注文完了向けシリアルキー引き渡し
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="updateHistoryAction">履歴登録するか</param>
		public static void DeliverSerialKeyForOrderComplete(string orderId, string lastChanged, SqlAccessor accessor, UpdateHistoryAction updateHistoryAction = UpdateHistoryAction.Insert)
		{
			new SerialKeyService().DeliverSerialKey(orderId, lastChanged, accessor);
			new OrderService().Modify(
				orderId,
				order =>
				{
					order.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP;
					order.OrderShippedDate = DateTime.Now;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
		}

		/// <summary>
		/// Update order status
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="orderStatus">Order status</param>
		/// <param name="orderPaymentStatus">Order payment status</param>
		/// <param name="orderPaymentDate">Order payment date</param>
		/// <returns>Updated count</returns>
		public static int UpdateOrderStatus(string orderId, string orderStatus, string orderPaymentStatus, DateTime? orderPaymentDate)
		{
			return new OrderService().Modify(
				orderId,
				order =>
				{
					order.OrderStatus = orderStatus;
					order.OrderPaymentStatus = orderPaymentStatus;
					order.OrderPaymentDate = orderPaymentDate;
					order.LastChanged = LAST_CHANGED_API;
					order.DateChanged = DateTime.Now;
					if (orderStatus == Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP)
					{
						order.OrderShippedDate = DateTime.Now;
					}
					if (orderStatus == Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP)
					{
						order.OrderDeliveringDate = DateTime.Now;
						order.OrderShippedDate = Validator.IsDate(order.OrderShippedDate) ? order.OrderShippedDate : DateTime.Now;
					}
					if (orderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
					{
						order.OrderCancelDate = DateTime.Now;
					}
				},
				UpdateHistoryAction.Insert);
		}

		/// <summary>
		/// 注文ステータスのUPDATE処理とシリアルキーの引き渡し処理 [フロント用]
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="orderTranId">決済取引ID</param>
		/// <param name="orderTranPassword">決済取引パスワード</param>
		/// <param name="creditBrunchNo">登録クレジットカード枝番（カードでない場合はnull）</param>
		/// <param name="paymentMemo">決済メモ（指定しない場合はnull）</param>
		/// <param name="orderPaymentStatus">入金ステータス（指定しない場合はnull)</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="isExternalPayment">外部決済あり？</param>
		/// <param name="timeout">注文ステータス更新のタイムアウト(秒)</param>
		/// <returns>更新結果</returns>
		/// <remarks>注文ステータスを仮注文から注文済みに更新</remarks>
		/// <remarks>入金済みなら、シリアルキーステータスを引当済みから引渡済みに更新</remarks>
		/// <remarks>入金済みなら、注文ステータスを出荷完了に更新</remarks>
		private static int UpdateOrderStatus(
			string orderId,
			CartObject cart,
			string lastChanged,
			string orderTranId,
			string orderTranPassword,
			int? creditBrunchNo,
			string paymentMemo,
			string orderPaymentStatus,
			string paymentOrderId,
			string onlinePaymentStatus,
			bool isExternalPayment,
			int? timeout = null)
		{
			// 注文ステータス更新
			var orderPaymentStatusIsConfirm = ((orderPaymentStatus == null)
				|| (orderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM));

			// 入金確認日時設定
			var orderPaymentDate = (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT) ? null : (DateTime?)DateTime.Now.Date;
			orderPaymentDate = (orderPaymentStatusIsConfirm) ? null : orderPaymentDate;

			// 注文ステータス更新
			var orderService = new OrderService();
			var updatedCount = orderService.UpdateOrderStatusForOrderComplete(
				orderId,
				Constants.FLG_ORDER_ORDER_STATUS_ORDERED,
				StringUtility.ToEmpty(orderTranId),
				creditBrunchNo,
				orderPaymentStatusIsConfirm ? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM : orderPaymentStatus,
				orderPaymentDate,
				StringUtility.ToEmpty(paymentMemo),
				isExternalPayment
					? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP
					: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE,
				isExternalPayment ? (DateTime?)DateTime.Now : null,
				lastChanged,
				StringUtility.ToEmpty(paymentOrderId),
				StringUtility.ToEmpty(onlinePaymentStatus),
				UpdateHistoryAction.DoNotInsert,
				timeout,
				orderTranPassword);

			// シリアルキー更新
			if (cart.IsDigitalContentsOnly
				&& (orderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE))
			{
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();

					var ht = new Hashtable
					{
						{Constants.FIELD_ORDER_ORDER_ID, orderId},
						{Constants.FIELD_ORDER_LAST_CHANGED, lastChanged},
					};
					using (var statement = new SqlStatement("SerialKey", "DeliverSerialKey"))
					{
						statement.ExecStatement(accessor, ht);
					}
					new OrderService().Modify(
						orderId,
						order =>
						{
							order.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP;
							order.OrderShippedDate = DateTime.Now;
							order.LastChanged = lastChanged;
						},
				UpdateHistoryAction.Insert);
				}
				foreach (var cp in cart.Items)
				{
					cp.IsDelivered = true;
				}
			}

			return updatedCount;
		}
		#endregion

		/// <summary>
		/// 配送希望日取得
		/// </summary>
		/// <param name="csShipping">配送方法情報/param>
		/// <returns>配送希望日</returns>
		[Obsolete("[V5.1] CartShipping#GetShippingDate() を使用してください")]
		public static string GetShippingDate(CartShipping csShipping)
		{
			if (csShipping != null)
			{
				// 配送希望日入力可 & 配送希望日指定済みの場合
				if (csShipping.SpecifyShippingDateFlg && csShipping.ShippingDate.HasValue)
				{
					return DateTimeUtility.ToStringFromRegion(
						csShipping.ShippingDate.Value,
						DateTimeUtility.FormatType.LongDateWeekOfDay2Letter);
				}
			}

			return "指定なし";	// デザイン文言なのでコードと分離したいがメール文言でも利用したいためここで指定する
		}

		/// <summary>
		/// 配送希望時間帯取得
		/// </summary>
		/// <param name="csShipping">配送方法情報/param>
		/// <returns>配送希望時間帯</returns>
		[Obsolete("[V5.1] CartShipping#GetShippingTime() を使用してください")]
		public static string GetShippingTime(CartShipping csShipping)
		{
			if (csShipping != null)
			{
				// 配送希望時間帯入力可 & 配送希望時間帯指定済みの場合
				if (csShipping.SpecifyShippingTimeFlg && (csShipping.ShippingTimeMessage != null))
				{
					return csShipping.ShippingTimeMessage;
				}
			}

			return "指定なし";  // デザイン文言なのでコードと分離したいがメール文言でも利用したいためここで指定する
		}

		/// <summary>
		/// カートのDELETE処理
		/// </summary>
		/// <param name="cartId">カートID</param>
		/// <returns>処理件数</returns>
		public static int DeleteCart(string cartId)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("Cart", "DeleteCart"))
			{
				var input = new Hashtable
				{
					{ Constants.FIELD_CART_CART_ID, cartId }
				};

				return statement.ExecStatementWithOC(accessor, input);
			}
		}

		/// <summary>
		/// 仮注文情報削除処理＆商品在庫戻し処理＆ゲスト登録削除処理＆ アドレス帳情報削除
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="isUserDelete">登録ユーザー削除処理有無</param>
		/// <param name="shippingNo">ユーザー配送先枝番</param>
		/// <param name="isUser">会員か否か</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="isOrderCreatedByFixedPurchase">注文実行種別が定期購入バッチか否か</param>
		/// <remarks>
		/// 仮注文削除、在庫戻し、ゲスト登録削除（必要なときのみ）、ポイント戻し処理を行う。
		/// 例外が発生した場合はロールバックし、仮注文情報は残す。
		/// </remarks>
		public static void RollbackPreOrder(
			Hashtable order,
			CartObject cart,
			bool isUserDelete,
			int shippingNo,
			bool isUser,
			UpdateHistoryAction updateHistoryAction,
			bool isOrderCreatedByFixedPurchase = false)
		{
			using (var accessor = new SqlAccessor())
			{
				// トランザクション開始
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = true;
				string transactionName = null;

				try
				{
					// ネクストエンジン連携でキャンセル処理を行うため、DB情報削除する前に一度情報を取得
					var rollBackOrderModel = new OrderService().Get((string)order[Constants.FIELD_ORDER_ORDER_ID], accessor);

					// Ｅ１－１．仮注文削除処理
					if (updated)
					{
						transactionName = "E1-1-1.注文情報DELETE処理";
						updated = (DeleteOrder(order, accessor) > 0);
					}
					if (updated)
					{
						transactionName = "E1-1-2.注文者情報DELETE処理";
						updated = (DeleteOrderOwner(order, accessor) > 0);
					}
					if (updated)
					{
						transactionName = "E1-1-3.注文配送先情報DELETE処理";
						updated = (DeleteOrderShipping(order, accessor) > 0);
					}
					if (updated)
					{
						transactionName = "E1-1-4.注文商品情報DELETE処理";
						updated = (DeleteOrderItem(order, accessor) > 0);
					}
					if (updated && cart.IsDigitalContentsOnly)
					{
						transactionName = "E1-1-4s.シリアルキーUNRESERVE処理";
						updated = (UnreserveSerialKeys(order, accessor) > 0);
					}
					if (updated)
					{
						transactionName = "E1-1-5.税率毎価格情報DELETE処理";
						updated = (DeleteOrderPriceByTaxRate(order, accessor) > 0);
					}
					if (updated
						&& (cart.SetPromotions.Items.Count > 0))
					{
						transactionName = "E1-1-6.注文セットプロモーションDELETE処理";
						updated = (DeleteSetPromotionAll(order, accessor) > 0);
					}

					// Ｅ１－２．在庫情報戻し処理
					if (updated)
					{
						transactionName = "E1-2.商品在庫情報UPDATE処理";
						updated = UpdateProductStock(order, cart, false, accessor);
					}
					if (updated)
					{
						transactionName = "E1-2s.商品在庫情報UPDATE処理(商品在庫履歴)";
						InsertProductStockHistory((string)order[Constants.FIELD_ORDER_ORDER_ID], cart, false, accessor);
					}

					// Ｅ１－３．ポイント情報戻し処理
					//		・ポイント履歴のorder_idで戻す値を制御している
					if (updated)
					{
						transactionName = "E1-3.ポイント情報戻し処理";
						updated = RollbackUserPoint(order, cart, isUser, accessor);
					}
					if (updated
						&& (cart.FixedPurchase != null)
						&& (cart.FixedPurchase.NextShippingUsePoint != 0))
					{
						transactionName = "E1-3-2.定期次回利用ポイント情報戻し処理";
						updated = new FixedPurchaseService().ApplyNextShippingUsePointChange(
							Constants.CONST_DEFAULT_DEPT_ID,
							cart.FixedPurchase,
							cart.FixedPurchase.NextShippingUsePoint,
							(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
							UpdateHistoryAction.DoNotInsert,
							accessor);
					}

					if (Constants.W2MP_COUPON_OPTION_ENABLED)
					{
						// Ｅ１－４．クーポン情報戻し処理
						if (updated)
						{
							transactionName = "E1-4-1.ユーザ利用クーポン情報戻し処理";
							updated = RollbackUseCoupons(order, cart, accessor);
						}
						if (updated)
						{
							transactionName = "E1-4-2.ユーザクーポン情報戻し処理(発行クーポン)";
							updated = RollbackPublishedCoupons(
								order,
								isUser,
								string.Empty,
								// 履歴落とさないので最終更新者はダミー
								UpdateHistoryAction.DoNotInsert,
								accessor);
						}

						if (updated && Constants.INTRODUCTION_COUPON_OPTION_ENABLED)
						{
							var userId = DomainFacade.Instance.UserService.GetReferredUserId(cart.CartUserId);
							if (string.IsNullOrEmpty(userId) == false)
							{

								transactionName = "E1-4-3.紹介ユーザークーポン情報戻し処理（発行クーポン）";
								updated = RollbackPublishedCoupons(
									order,
									isUser,
									String.Empty,
									UpdateHistoryAction.DoNotInsert,
									accessor,
									userId);
							}
						}

						if (updated
							&& (cart.FixedPurchase != null)
							&& (cart.Coupon != null)
							&& (cart.IsBotChanOrder == false)
							&& isOrderCreatedByFixedPurchase)
						{
							transactionName = "E1-4-4.定期次回利用クーポン戻し処理";
							updated = new FixedPurchaseService().UpdateNextShippingUseCoupon(
								cart.FixedPurchase.FixedPurchaseId,
								cart.Coupon.CouponId,
								cart.Coupon.CouponNo,
								(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
								UpdateHistoryAction.DoNotInsert,
								accessor);

							var couponDetailInfo = new UserCouponDetailInfo()
							{
								CouponType = cart.Coupon.CouponType,
								DeptId = cart.Coupon.DeptId,
								CouponNo = cart.Coupon.CouponNo,
								CouponId = cart.Coupon.CouponId,
								UserCouponCount = cart.Coupon.UserCouponCount,
								CouponCode = cart.Coupon.CouponCode
							};

							var couponService = new CouponService();
							// 変更したクーポンを適用（利用可能回数を減らす／利用済みにする）
							if (updated)
							{
								updated = couponService.ApplyNextShippingUseCouponToFixedPurchase(
									couponDetailInfo,
									cart.FixedPurchase.UserId,
									cart.FixedPurchase.FixedPurchaseId,
									(Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE == Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
										? cart.Owner.MailAddr
										: cart.FixedPurchase.UserId,
									(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
									UpdateHistoryAction.Insert,
									accessor);
							}
						}
					}

					// Ｅ１－５．登録ユーザー情報削除処理
					if (updated && isUserDelete)
					{
						transactionName = "E1-5.登録ユーザー情報削除処理";
						updated = DeleteUser(order, accessor);
					}

					// Ｅ１－６． アドレス帳情報戻し処理
					if (updated)
					{
						transactionName = "E1-6.アドレス帳情報削除処理";
						updated = DeleteUserShipping(
							cart,
							(string)order[Constants.FIELD_ORDER_USER_ID],
							shippingNo,
							isUser,
							"",
							// 履歴を落とさないので最終更新者はダミー
							UpdateHistoryAction.DoNotInsert,
							accessor);
					}

					// Ｅ１－７． 定期購入情報戻し処理
					if (Constants.FIXEDPURCHASE_OPTION_ENABLED && cart.Items.Any(item => item.IsFixedPurchase))
					{
						var fixedPurchaseService = new FixedPurchaseService();
						var isTemporaryFixedPurchase = fixedPurchaseService.CheckTemporaryRegistration(
							(string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID],
							accessor);
						if (updated)
						{
							transactionName = "E1-7-1.定期購入情報削除処理";
							updated = fixedPurchaseService.DeletePrefixedPurchaseAndHistory(
								(string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID],
								accessor);
							//親注文が定期なら、定期IDを取得できないため、同梱履歴は注文IDからロールバックする
							if (cart.IsCombineParentOrderHasFixedPurchase && updated)
							{
								fixedPurchaseService.DeleteHistoryByOrderId(
									(string)order[Constants.FIELD_ORDER_ORDER_ID],
									accessor);
							}
						}

						// 新規定期購入の場合には定期購入情報削除処理で定期継続分析情報が削除されたため、この処理は不要となる
						if (updated && (isTemporaryFixedPurchase == false))
						{
							transactionName = "E1-7-2.定期継続分析削除処理";
							updated = DeleteFixedPurchaseRepeatAnalysis(order, accessor);
						}

						// 定期会員フラグ戻し処理
						if (updated && isUser && (isUserDelete == false) && Constants.MEMBER_RANK_OPTION_ENABLED)
						{
							var hasFixedPurchase = fixedPurchaseService.HasActiveFixedPurchaseInfo(
								(string)order[Constants.FIELD_ORDER_USER_ID],
								accessor);
							if (hasFixedPurchase == false)
							{
								transactionName = "E1-7-3.定期会員フラグ更新処理";
								updated = UpdateFixedPurchaseMemberFlg(
									order,
									Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF,
									accessor);
							}
						}
					}

					// Ｅ１－９． リアルタイム累計購入回数戻し処理
					if (updated & (isUserDelete == false))
					{
						transactionName = "E1-9.リアルタイム累計購入回数";
						updated = UpdateRealTimeOrderCount(order, Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_ROLLBACK, accessor);
					}

					// Ｅ１－１０． 注文電子発票情報削除処理
					if (updated
						&& OrderCommon.DisplayTwInvoiceInfo()
						&& cart.Shippings.Any(shipping => shipping.IsShippingAddrTw))
					{
						transactionName = "E1-10-1.注文電子発票情報削除処理";
						updated = new TwOrderInvoiceService().Delete((string)order[Constants.FIELD_ORDER_ORDER_ID], accessor);
						if (updated
							&& Constants.FIXEDPURCHASE_OPTION_ENABLED
							&& cart.Items.Any(item => item.IsFixedPurchase))
						{
							transactionName = "E1-10-2.定期購入電子発票情報削除処理";
							updated = new TwFixedPurchaseInvoiceService().Delete(
								(string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID],
								accessor);
						}
					}

					// １－Ｘ．成功/失敗時処理
					if (updated)
					{
						// 更新履歴登録
						if (updateHistoryAction == UpdateHistoryAction.Insert)
						{
							new UpdateHistoryService().InsertEmptyForOrder(
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_USER_ID],
								(string)order[Constants.FIELD_ORDER_LAST_CHANGED], accessor);
							new UpdateHistoryService().InsertForUser(
								(string)order[Constants.FIELD_ORDER_USER_ID],
								(string)order[Constants.FIELD_ORDER_LAST_CHANGED], accessor);
						}

						// NOTE: ネクストエンジン連携は外部連携なので、DB更新処理ロールバックできるように最後で実行
						transactionName = "E1-11-1.ネクストエンジン仮注文連携キャンセル処理";
						var nextEngineTempOrderSync = NextEngineTempOrderSync.GetNextEngineTempOrderSync(order);
						var cancelErrorMessage = nextEngineTempOrderSync.CancelSynchronizedTempOrder(rollBackOrderModel);
						if (string.IsNullOrEmpty(cancelErrorMessage.Item1) == false)
						{
							var mailMessage = new StringBuilder();
							mailMessage.AppendLine(NextEngineConstants.ERROR_MESSAGE_FAIL_TMP_CNSL);
							mailMessage.AppendLine(NextEngineConstants.ERROR_MESSAGE_FAIL_TMP_CNSL_FOR_ADMIN);
							mailMessage.AppendLine(string.Format(NextEngineConstants.ERROR_MESSAGE_FORMAT_TARGET,
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								(string)order[Constants.FIELD_ORDER_USER_ID]));

							NextEngineApi.MailSend(new Hashtable { { "message", mailMessage.ToString() }, });

							var logMessage = new StringBuilder();
							logMessage.AppendLine(NextEngineConstants.ERROR_MESSAGE_FAIL_TMP_CNSL);
							logMessage.AppendLine(string.Format(NextEngineConstants.ERROR_MESSAGE_FORMAT_TARGET,
									(string)order[Constants.FIELD_ORDER_ORDER_ID],
									(string)order[Constants.FIELD_ORDER_USER_ID]));

							FileLogger.WriteError(logMessage.ToString());

							throw new ApplicationException(transactionName + "に失敗しました。");
						}

						nextEngineTempOrderSync.SetTempOrderSyncFlg(false);

						// トランザクションコミット
						accessor.CommitTransaction();
					}
					else
					{
						throw new ApplicationException(transactionName + "に失敗しました。");
					}
				}
				catch (Exception)
				{
					// トランザクションロールバック
					accessor.RollbackTransaction();
					throw;
				}
			}
		}

		/// <summary>
		/// 注文情報のDELETE処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		private static int DeleteOrder(Hashtable order, SqlAccessor accessor)
		{
			using (var statement = new SqlStatement("Order", "DeleteOrder"))
			{
				return statement.ExecStatement(accessor, order);
			}
		}

		/// <summary>
		/// 注文者情報のDELETE処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		private static int DeleteOrderOwner(Hashtable order, SqlAccessor accessor)
		{
			using (var statement = new SqlStatement("Order", "DeleteOrderOwner"))
			{
				return statement.ExecStatement(accessor, order);
			}
		}

		/// <summary>
		/// 注文配送先情報のDELETE処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		private static int DeleteOrderShipping(Hashtable order, SqlAccessor accessor)
		{
			using (var statement = new SqlStatement("Order", "DeleteOrderShipping"))
			{
				return statement.ExecStatement(accessor, order);
			}
		}

		/// <summary>
		/// 税率毎価格情報のDELETE処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		private static int DeleteOrderPriceByTaxRate(Hashtable order, SqlAccessor sqlAccessor)
		{
			using (var statement = new SqlStatement("Order", "DeleteOrderPriceByTaxRate"))
			{
				return statement.ExecStatement(sqlAccessor, order);
			}
		}

		/// <summary>
		/// 注文商品情報のDELETE処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		private static int DeleteOrderItem(Hashtable order, SqlAccessor accessor)
		{
			using (var statement = new SqlStatement("Order", "DeleteOrderItem"))
			{
				return statement.ExecStatement(accessor, order);
			}
		}

		/// <summary>
		/// シリアルキーのUNRESERVE処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		private static int UnreserveSerialKeys(Hashtable order, SqlAccessor accessor)
		{
			var updatedCount = new SerialKeyService().UnreserveSerialKey(
				(string)order[Constants.FIELD_ORDER_ORDER_ID],
				(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
				accessor);
			return updatedCount;
		}

		/// <summary>
		/// 注文セットプロモーションDELETE処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		private static int DeleteSetPromotionAll(Hashtable order, SqlAccessor accessor)
		{
			return new OrderService().DeleteSetPromotionAll((string)order[Constants.FIELD_ORDER_ORDER_ID], accessor);
		}

		/// <summary>
		/// ユーザーポイント戻し処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="isUser">会員か</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>実行結果</returns>
		private static bool RollbackUserPoint(Hashtable order, CartObject cart, bool isUser, SqlAccessor accessor)
		{
			if (Constants.W2MP_POINT_OPTION_ENABLED == false) return true;
			if (isUser == false) return true;

			// 現仕様では、ユーザポイント情報の戻し処理では、有効期限の戻し処理は行わない
			// また、発行時に登録したユーザポイント履歴情報も削除する
			// ※ユーザポイント履歴一覧で参照しにくいため
			// 付与していない可能性もあるので、条件を「>=0」にする
			var sv = new PointService();
			var isSuccess = ((sv.RollbackUserPointForBuyFailure(
				(string)order[Constants.FIELD_ORDER_USER_ID],
				(string)order[Constants.FIELD_ORDER_ORDER_ID],
				cart.UsePoint,
				cart.AddPoint,
				(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
				UpdateHistoryAction.DoNotInsert,
				accessor)) >= 0);

			// CrossPoint連携時は取消用のAPIを叩く（失敗していた場合falseを返すことで処理全体をロールバックさせる）
			if (isSuccess && Constants.CROSS_POINT_OPTION_ENABLED)
			{
				var crossPointApiInput = new PointApiInput()
				{
					MemberId = (string)order[Constants.FIELD_ORDER_USER_ID],
					OrderDate = (DateTime)order[Constants.FIELD_ORDER_ORDER_DATE],
					PosNo = Constants.CROSS_POINT_POS_NO,
					OrderId = (string)order[Constants.FIELD_ORDER_ORDER_ID],
				};
				var apiResult = new CrossPointPointApiService().Delete(crossPointApiInput.GetParam(PointApiInput.RequestType.Delete));
				isSuccess = apiResult.IsSuccess;
			}
			return isSuccess;
		}

		/// <summary>
		/// 利用クーポンロールバック
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>実行結果</returns>
		private static bool RollbackUseCoupons(Hashtable order, CartObject cart, SqlAccessor accessor)
		{
			var updated = true;
			var couponService = new CouponService();
			var transactionName = "E1-4-1.利用注文クーポン情報DELETE処理";
			// 注文クーポン情報削除
			new OrderService().DeleteOrderCoupon(
				StringUtility.ToEmpty(order[Constants.FIELD_ORDERCOUPON_ORDER_ID]),
				(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
				UpdateHistoryAction.DoNotInsert,
				accessor);

			if (cart.Coupon == null) return true;
			transactionName = "E1-4-3.ユーザークーポン情報戻し処理(利用回数制限ありクーポン利用)";
			if (cart.Coupon.IsCouponLimit())
			{
				updated = couponService.UpdateUserCouponUseFlg(
					cart.OrderUserId,
					cart.Coupon.DeptId,
					cart.Coupon.CouponId,
					cart.Coupon.CouponNo,
					false,
					(DateTime)order[Constants.FIELD_ORDER_ORDER_DATE],
					(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}
			if (updated == false) throw new Exception(transactionName + "でエラーが発生しました。");

			transactionName = "E1-4-4.ユーザークーポン情報戻し処理(回数制限クーポンの利用回数)";
			if (cart.Coupon.IsCouponAllLimit())
			{
				updated = couponService.UpdateCouponCountUp(
					cart.Coupon.DeptId,
					cart.Coupon.CouponId,
					cart.Coupon.CouponCode,
					(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
					accessor);
			}
			if (updated == false) throw new Exception(transactionName + "でエラーが発生しました。");

			transactionName = "E1-4-5.クーポン利用ユーザー情報戻し処理(ブラックリスト型クーポン利用)";
			if (cart.Coupon.IsBlacklistCoupon())
			{
				var couponUseUser = (Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE == Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
						? cart.Owner.MailAddr
						: cart.OrderUserId;
				updated = (couponService.DeleteCouponUseUser(
					cart.Coupon.CouponId,
					couponUseUser,
					accessor) > 0);
			}
			if (updated == false) throw new Exception(string.Format("{0}でエラーが発生しました。", transactionName));

			transactionName = "E1-4-6.ユーザークーポン情報戻し処理(会員限定回数制限ありクーポン利用)";
			if (cart.Coupon.IsCouponLimitedForRegisteredUser())
			{
				updated = CouponOptionUtility.UpdateUserCouponCount(
					cart.Coupon.DeptId,
					cart.OrderUserId,
					cart.Coupon.CouponId,
					cart.Coupon.CouponNo,
					accessor,
					true);
			}
			if (updated == false) throw new Exception(string.Format("{0}でエラーが発生しました。", transactionName));

			transactionName = "E1-4-7.ユーザクーポン履歴情報戻し処理(ユーザークーポンの使用履歴)";
			var couponHistory = new UserCouponHistoryModel()
			{
				DeptId = cart.Coupon.DeptId,
				UserId = (string)order[Constants.FIELD_ORDER_USER_ID],
				OrderId = (string)order[Constants.FIELD_ORDER_ORDER_ID]
			};
			var result = couponService.DeleteUserCouponHistory(couponHistory, accessor);
			updated = (result >= 0);	// ない可能性もあるので？、戻り値の条件を「>=0」にする

			if (updated == false) throw new Exception(transactionName + "でエラーが発生しました。");

			return true;
		}

		/// <summary>
		/// ユーザー発行クーポン情報戻し処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="isUser">会員か</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// A<param name="userIdOfReferralCode">紹介コードを発行したユーザーID</param>
		/// <returns>実行結果</returns>
		private static bool RollbackPublishedCoupons(
			Hashtable order,
			bool isUser,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			string userIdOfReferralCode = "")
		{
			if (isUser == false) return true;
			var userId = string.IsNullOrEmpty(userIdOfReferralCode)
				? (string)order[Constants.FIELD_ORDER_USER_ID]
				: userIdOfReferralCode;

			var couponService = new CouponService();
			var result = couponService.DeleteUserCouponByOrderId(
				userId,
				(string)order[Constants.FIELD_ORDER_ORDER_ID],
				lastChanged,
				updateHistoryAction,
				accessor);

			if (result >= 0)
			{
				var couponHistory = new UserCouponHistoryModel
				{
					DeptId = Constants.W2MP_DEPT_ID,
					UserId = userId,
					OrderId = (string)order[Constants.FIELD_ORDER_ORDER_ID]
				};
				result = couponService.DeleteUserCouponHistory(couponHistory, accessor);
			}

			return (result >= 0); // 発行していない可能性もあるので、戻り値の条件を「>=0」にする
		}

		/// <summary>
		/// 登録ユーザー情報削除
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>実行結果</returns>
		private static bool DeleteUser(Hashtable order, SqlAccessor accessor)
		{
			var result = new UserService().Delete(
				(string)order[Constants.FIELD_ORDER_USER_ID],
				(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
				UpdateHistoryAction.DoNotInsert,
				accessor);
			return result;
		}

		/// <summary>
		/// アドレス帳情報戻し処理処理
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shippingNo">ユーザー配送先NO</param>
		/// <param name="isUser">会員か</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>実行結果</returns>
		private static bool DeleteUserShipping(
			CartObject cart,
			string userId,
			int shippingNo,
			bool isUser,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			if (isUser && cart.GetShipping().UserShippingRegistFlg)
			{
				var updated = new UserShippingService().Delete(userId, shippingNo, lastChanged, updateHistoryAction, accessor);

				// デフォルト配送先に設定されていた場合は同時に削除する。
				var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(userId, accessor);
				if ((userDefaultOrderSetting != null)
					&& (shippingNo == userDefaultOrderSetting.UserShippingNo))
				{
					userDefaultOrderSetting.UserShippingNo = null;
					new UserDefaultOrderSettingService().Update(userDefaultOrderSetting, accessor);
				}
				return updated;
			}
			return true;
		}

		#region -DeleteFixedPurchaseRepeatAnalysis 定期購入継続分析削除
		/// <summary>
		/// 定期購入継続分析削除
		/// </summary>
		/// <param name="order">削除対象の注文情報</param>
		/// <param name="accessor">トランザクションを内包するアクセサ</param>
		/// <returns>処理結果</returns>
		private static bool DeleteFixedPurchaseRepeatAnalysis(Hashtable order, SqlAccessor accessor)
		{
			var result = new FixedPurchaseRepeatAnalysisService().DeleteByOrder(
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_LAST_CHANGED]),
				accessor);
			return result > 0;
		}
		#endregion

		/// <summary>
		/// 商品在庫更新
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="decrease">在庫引当か</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns></returns>
		public static bool UpdateProductStock(Hashtable order, CartObject cart, bool decrease, SqlAccessor accessor)
		{
			// デッドロックを考慮してID順に並び替える
			var items = new List<CartProduct>(cart.Items);
			items.Sort((product1, product2) => string.Compare(product1.VariationId, product2.VariationId));

			// 注文同梱での在庫引き当ての場合、同梱元の注文については既に在庫確保済みのため、確保済みの個数を考慮するため同梱元注文取得
			var orderService = new OrderService();
			var combineOrders = (string.IsNullOrEmpty(cart.OrderCombineParentOrderId))
				? null
				: cart.OrderCombineParentOrderId.Split(',').Select(id => orderService.Get(id, accessor)).ToArray();

			// ループしながら在庫更新
			using (var statementGetStock = new SqlStatement("ProductStock", "GetProductStockWithUpdLockForOrder"))
			using (var statementUpdateStock = new SqlStatement("ProductStock", "UpdateProductStockForOrder"))
			{
				var outOfStockItemNames = new List<string>();
				foreach (CartProduct cp in items)
				{
					var input = new Hashtable
					{
						{ Constants.FIELD_PRODUCTSTOCK_SHOP_ID, cp.ShopId },
						{ Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, cp.ProductId },
						{ Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, cp.VariationId },
						{ Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, order[Constants.FIELD_ORDER_LAST_CHANGED] }
					};

					var stockAdd = cp.Count * (decrease ? -1 : 1);
					input.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY, stockAdd);

					// 更新ロックをかけながら在庫チェック
					if (decrease
						&& ((cp.StockManagementKbn == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPNG_BUYNG)
							|| (cp.StockManagementKbn == Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYNG)))
					{
						DataView stock = statementGetStock.SelectSingleStatement(accessor, input);

						// 注文同梱元注文での同一商品の購入数を取得
						var combineOrderItemQuantity = 0;
						if (combineOrders != null)
						{
							combineOrderItemQuantity = combineOrders.SelectMany(o => o.Shippings[0].Items)
								.Where(i => ((cp.ShopId == i.ShopId) && (cp.ProductId == i.ProductId) && (cp.VariationId == i.VariationId)))
								.Sum(i => i.ItemQuantity) * (decrease ? -1 : 0);
						}

						// 在庫が満たない場合は例外発生(注文同梱されている場合、注文同梱元注文の購入数を除外する)
						if ((int)stock[0][Constants.FIELD_PRODUCTSTOCK_STOCK] + stockAdd - combineOrderItemQuantity < 0)
						{
							outOfStockItemNames.Add(cp.ProductJointName);
							continue;
						}
					}

					// 在庫更新
					if (cp.StockManagementKbn != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
					{
						if (statementUpdateStock.ExecStatement(accessor, input) <= 0) return false;
					}
				}
				if (outOfStockItemNames.Any()) throw new ProductStockException(outOfStockItemNames.ToArray());
			}

			return true;
		}

		/// <summary>
		/// 商品在庫履歴挿入
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="cart">カート情報</param>
		/// <param name="isDecrease">在庫減らすか</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void InsertProductStockHistory(string orderId, CartObject cart, bool isDecrease, SqlAccessor accessor)
		{
			// 在庫管理方法が「0.在庫管理をしない 」以外の場合に対して履歴挿入
			using (var statement = new SqlStatement("ProductStock", "InsertProductStockHistoryForOrder"))
			{
				foreach (var cp in cart.Items.FindAll(item => item.StockManagementKbn != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED))
				{
					var input = new Hashtable
				{
						{ Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, orderId },
						{ Constants.FIELD_PRODUCTSTOCKHISTORY_SHOP_ID, cp.ShopId },
						{ Constants.FIELD_PRODUCTSTOCKHISTORY_PRODUCT_ID, cp.ProductId },
						{ Constants.FIELD_PRODUCTSTOCKHISTORY_VARIATION_ID, cp.VariationId },
						{ Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS, Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER },
						{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, ((isDecrease) ? -1 : 1) * cp.Count },
						{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK, 0 },
						{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED, 0 },
						{ Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_STOCK, null },
						{ Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK, null },
						{ Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_REALSTOCK_RESERVED, null },
						{ Constants.FIELD_PRODUCTSTOCKHISTORY_LAST_CHANGED, "" }
					};

					statement.ExecStatement(accessor, input);
				}
			}
		}

		/// <summary>
		/// ユーザポイント付与
		/// </summary>
		/// <param name="order">注文情報Hashtable</param>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="pointIncKbn">ポイント加算区分</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">トランザクションを構成するSqlAccessor</param>
		/// <returns>更新件数</returns>
		public static int AddUserPoint(
			Hashtable order,
			CartObject cart,
			string pointIncKbn,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var pointRules = PointOptionUtility.GetPointRulePriorityHigh(pointIncKbn);

			// 利用ポイント・付与ポイントの両方が0の際もCrossPointへ連携を行う
			string errorMessage;
			if (Constants.CROSS_POINT_OPTION_ENABLED
				&& (pointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BUY))
			{
				if (CrossPointUtility.RegisterCrossPoint(
						order,
						cart,
						out errorMessage,
						accessor) == false)
				{
					// CrossPoint連携失敗時は例外を返すことで仮注文作成自体をロールバックさせる
					throw new Exception(errorMessage);
				}
			}
			var updated = 0;

			// cart.OrderUserId -> order[user_id] -> ""の優先順位でuseridを取得
			var userId = (string.IsNullOrEmpty(cart.OrderUserId) == false)
				? cart.OrderUserId
				: (string.IsNullOrEmpty(order[Constants.FIELD_ORDER_USER_ID].ToString()) == false)
					? order[Constants.FIELD_ORDER_USER_ID].ToString()
					: string.Empty;

			// 注文関連ファイル取込を使った場合固定値を渡す為
			// ルールに関係なく1回加算
			// Botchanからの注文時、同じパラメーターである
			// "order_point_add"を利用しており、想定外に処理に入ってしまうため、Botからの注文は除外
			if (order.ContainsKey(Constants.FIELD_ORDER_ORDER_POINT_ADD)
				&& (cart.IsBotChanOrder == false))
			{
				var userPoint = new UserPointModel
				{
					UserId = userId,
					PointKbn = Constants.FLG_USERPOINT_POINT_KBN_BASE,
					DeptId = Constants.CONST_DEFAULT_DEPT_ID,
					PointRuleId = "",
					PointRuleKbn = Constants.FLG_USERPOINT_POINT_RULE_KBN_BASIC,
					PointType = Constants.FLG_USERPOINT_POINT_TYPE_TEMP,
					PointIncKbn = pointIncKbn,
					Point = (decimal)order[Constants.FIELD_ORDER_ORDER_POINT_ADD],
					PointExp = DateTime.Now.AddYears(1),
					OrderId = cart.OrderId,
					LastChanged = Constants.FLG_LASTCHANGED_BATCH,
					DateChanged = DateTime.Now,
				};
				updated = DomainFacade.Instance.PointService.IssuePointByCrossPoint(userPoint, accessor);

				return updated;
			}

			foreach (var pointRule in pointRules)
			{
				string pointRuleIncType;
				var addPoint = PointOptionUtility.GetOrderPointAdd(
					cart,
					pointIncKbn,
					out pointRuleIncType,
					cart.FixedPurchase,
					pointRule,
					accessor);

				if (Constants.CROSS_POINT_OPTION_ENABLED
					&& (pointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BUY))
				{
					var userPoint = new UserPointModel
					{
						UserId = userId,
						PointKbn = Constants.FLG_USERPOINT_POINT_KBN_BASE,
						DeptId = Constants.CONST_DEFAULT_DEPT_ID,
						PointRuleId = pointRule.PointRuleId,
						PointRuleKbn = Constants.FLG_USERPOINT_POINT_RULE_KBN_BASIC,
						PointType = Constants.FLG_USERPOINT_POINT_TYPE_TEMP,
						PointIncKbn = Constants.FLG_POINTRULE_POINT_INC_KBN_BUY,
						Point = addPoint,
						PointExp = DateTime.Now.AddYears(1),
						OrderId = cart.OrderId,
						LastChanged = Constants.FLG_LASTCHANGED_USER,
						DateChanged = DateTime.Now,
					};
					updated += DomainFacade.Instance.PointService.IssuePointByCrossPoint(userPoint, accessor);
				}
				else if (Constants.CROSS_POINT_OPTION_ENABLED
					&& (pointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY))
				{
					var userPoint = new UserPointModel
					{
						UserId = userId,
						PointKbn = Constants.FLG_USERPOINT_POINT_KBN_BASE,
						DeptId = Constants.CONST_DEFAULT_DEPT_ID,
						PointRuleId = pointRule.PointRuleId,
						PointRuleKbn = Constants.FLG_USERPOINT_POINT_RULE_KBN_BASIC,
						PointType = Constants.FLG_USERPOINT_POINT_TYPE_TEMP,
						PointIncKbn = Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY,
						Point = addPoint,
						PointExp = DateTime.Now.AddYears(1),
						OrderId = cart.OrderId,
						LastChanged = Constants.FLG_LASTCHANGED_USER,
						DateChanged = DateTime.Now,
					};
					updated += DomainFacade.Instance.PointService.IssuePointByCrossPoint(userPoint, accessor);
				}
				else
				{
					updated += PointOptionUtility.InsertUserPoint(
						userId,
						cart.OrderId,
						pointRule.PointRuleId,
						addPoint,
						(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
						updateHistoryAction,
						accessor);
				}
			}
			return updated;
		}

		/// <summary>
		/// 新規登録時ユーザーポイント付与
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void InsertUserPointAtUserRegistration(UserModel user, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			var pointRules = PointOptionUtility.GetPointRulePriorityHigh(Constants.FLG_POINTRULE_POINT_INC_KBN_USER_REGISTER);

			var totalGrantedPoint = 0;
			foreach (var pointRule in pointRules)
			{
				totalGrantedPoint += (int)pointRule.IncNum;
				new PointOptionUtility().InsertUserRegisterUserPoint(
					user.UserId,
					pointRule.PointRuleId,
					user.LastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				if (Constants.CROSS_POINT_OPTION_ENABLED)
				{
					CrossPointUtility.UpdateCrossPointApiWithWebErrorMessage(
						user,
						pointRule.IncNum,
						CrossPointUtility.GetValue(
							Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID,
							pointRule.PointIncKbn));
				}
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForUser(user.UserId, user.LastChanged);
			}
		}

		/// <summary>
		/// 定期購入ID生成
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>新定期購入ID</returns>
		public static string CreateFixedPurchaseId(string shopId)
		{
			return string.Format(Constants.FORMAT_FIXEDPURCHASE_ID, CreateNewNumberingId(shopId, Constants.NUMBER_KEY_FIXED_PURCHASE_ID));
		}

		/// <summary>
		/// 注文失敗時ログメッセージ作成
		/// </summary>
		/// <param name="transactionName">処理名</param>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="message">メッセージ</param>
		/// <returns>作成メッセージ</returns>
		public static string CreateOrderFailedLogMessage(
			string transactionName,
			Hashtable order,
			CartObject cart,
			string message = null)
		{
			var errorMessage = new StringBuilder();
			errorMessage.Append(transactionName).Append("失敗：[");
			errorMessage.Append("user_id=").Append((string)order[Constants.FIELD_ORDER_USER_ID]).Append(",");
			errorMessage.Append("order_id=").Append((string)order[Constants.FIELD_ORDER_ORDER_ID]).Append(",");
			if (string.IsNullOrEmpty((string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]) == false)
			{
				errorMessage.Append("paymentOrderId=").Append(order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID]).Append(",");
			}
			if (cart != null)
			{
				errorMessage.Append("owner_kbn=").Append(cart.Owner.OwnerKbn).Append(",");
				errorMessage.Append("cart_id=").Append(cart.CartId).Append(",");
				errorMessage.Append("shop_id=").Append(cart.ShopId).Append("]");
			}

			if (message != null) errorMessage.Append(" ").Append(message);

			return errorMessage.ToString();
		}

		/// <summary>
		/// 楽天3Dセキュア注文失敗時ログメッセージ作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="message">メッセージ</param>
		/// <returns>作成メッセージ</returns>
		public static string CreateRakuten3DSecureOrderFailedLogMessage(
			Hashtable order,
			string message)
		{
			string errorMessage = string.Format("order_id={0} {1}", (string)order[Constants.FIELD_ORDER_ORDER_ID], message);

			return errorMessage;
		}

		/// <summary>
		/// 注文完了アラート時ログメッセージ作成
		/// </summary>
		/// <param name="transactionName">処理名</param>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <returns>作成メッセージ</returns>
		public static string CreateOrderSuccessAlertLogMessage(string transactionName, Hashtable order, CartObject cart)
		{
			var errorMessage = new StringBuilder();
			errorMessage.Append("注文成功→").Append(transactionName).Append("失敗：[");
			errorMessage.Append("user_id=").Append((string)order[Constants.FIELD_ORDER_USER_ID]).Append(",");
			errorMessage.Append("order_id=").Append((string)order[Constants.FIELD_ORDER_ORDER_ID]).Append(",");
			errorMessage.Append("owner_kbn=").Append(cart.Owner.OwnerKbn).Append(",");
			errorMessage.Append("cart_id=").Append(cart.CartId).Append(",");
			errorMessage.Append("shop_id=").Append(cart.ShopId).Append("]");

			return errorMessage.ToString();
		}

		/// <summary>
		/// 与信向け決済連携メモ作成
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="paymentTranId">決済連携ID</param>
		/// <param name="priceTotal">注文金額合計</param>
		/// <returns>決済連携メモ</returns>
		public static string CreateOrderPaymentMemoForAuth(
			string orderId,
			string paymentOrderId,
			string paymentId,
			string paymentTranId,
			decimal priceTotal)
		{
			var memo = OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
				string.IsNullOrEmpty(paymentOrderId)
				? orderId
				: paymentOrderId,
				paymentId,
				paymentTranId,
				"与信",
				priceTotal);
			return memo;
		}

		#region +UpdateExternalPaymentStatusesAndMemoForCancel キャンセル向け外部決済ステータス系＆メモ更新
		/// <summary>
		/// キャンセル向け外部決済ステータス系＆メモ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void UpdateExternalPaymentStatusesAndMemoForCancel(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var orderOld = new OrderService().Get(orderId, accessor);

			// Convert Amount For Ec Pay And Neweb Pay
			if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				|| (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
			{
				var cancelLastBilledAmount = CurrencyManager.GetSettlementAmount(
					orderOld.LastBilledAmount,
					orderOld.SettlementRate,
					orderOld.SettlementCurrency);
				orderOld.LastBilledAmount = cancelLastBilledAmount;
			}

			UpdateExternalPaymentStatusesAndMemoForCancel(orderOld, lastChanged, updateHistoryAction, accessor);
		}
		/// <summary>
		/// キャンセル向け外部決済ステータス系＆メモ更新
		/// </summary>
		/// <param name="orderOld">旧注文情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void UpdateExternalPaymentStatusesAndMemoForCancel(
			OrderModel orderOld,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			UpdateExternalPaymentStatusesAndMemoForCancel(
				orderOld.OrderId,
				orderOld.OrderPaymentKbn,
				orderOld.PaymentOrderId,
				orderOld.CardTranId,
				CurrencyManager.GetSendingAmount(
				orderOld.LastBilledAmount,
					orderOld.SettlementAmount,
					orderOld.SettlementCurrency),
				orderOld.IsExchangeOrder,
				lastChanged,
				"キャンセル",
				updateHistoryAction,
				accessor);
		}
		/// <summary>
		/// キャンセル向け外部決済ステータス系＆メモ更新
		/// </summary>
		/// <param name="orderId">注文情報</param>
		/// <param name="orderPaymentKbn">決済種別ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="cardTranId">カード取引NO</param>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <param name="isExchangeOrder">交換注文か</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="actionName">キャンセル表記</param>
		public static void UpdateExternalPaymentStatusesAndMemoForCancel(
			string orderId,
			string orderPaymentKbn,
			string paymentOrderId,
			string cardTranId,
			decimal lastBilledAmount,
			bool isExchangeOrder,
			string lastChanged,
			string actionName,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var orderService = new OrderService();

			// オンライン決済ステータス更新
			orderService.UpdateOnlinePaymentStatus(
				orderId,
				Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED,
				lastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);
			// 交換注文でなければ決済連携系更新
			if (isExchangeOrder == false)
			{
				// 外部決済ステータス更新
				orderService.UpdateExternalPaymentInfo(
					orderId,
					Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL,
					false,	// 外部決済与信日時は更新しない
					DateTime.Now,
					string.Empty,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);
				// 決済連携メモ追記
				orderService.AddPaymentMemo(
					orderId,
					OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
						string.IsNullOrEmpty(paymentOrderId) ? orderId : paymentOrderId,
						orderPaymentKbn,
						cardTranId,
						actionName,
						lastBilledAmount),
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);
			}

			// 更新履歴
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
			}
		}
		#endregion

		/// <summary>
		/// クレジットカード選択可否(ログイン済み、ZEUS決済、１枚以上の登録)
		/// </summary>
		/// <param name="isLoggedIn">ログイン状態</param>
		/// <param name="registedCardCount">登録済みカード枚数</param>
		public static bool GetRegistedCreditCardSelectable(bool isLoggedIn, int registedCardCount)
		{
			return (isLoggedIn
				&& CreditCardRegistable
				&& (registedCardCount > 0));
		}

		/// <summary>
		/// クレジットカード登録が可能か
		/// </summary>
		/// <param name="isLoggedIn">ログインしているかどうか</param>
		/// <param name="userId">ユーザID</param>
		/// <returns>クレジットカード登録可否</returns>
		public static bool GetCreditCardRegistable(bool isLoggedIn, string userId)
		{
			// 該当ユーザーのクレジットカード情報一覧を取得
			var userCreditCards = UserCreditCard.GetUsable(userId);

			// 登録可否取得
			return GetCreditCardRegistable(isLoggedIn, userCreditCards.Length);
		}
		/// <summary>
		/// クレジットカード登録が可能か
		/// </summary>
		/// <param name="isUser">会員か</param>
		/// <param name="registedCardCount">登録済みカード枚数</param>
		/// <param name="resistCardCount">登録しようとしているカード枚数</param>
		public static bool GetCreditCardRegistable(bool isUser, int registedCardCount, int resistCardCount = 1)
		{
			return (isUser
				&& CreditCardRegistable
				&& ((registedCardCount + resistCardCount) <= Constants.MAX_NUM_REGIST_CREDITCARD));
		}

		/// <summary>
		/// 有効な定期配送パターンか
		/// </summary>
		/// <param name="limitedSetting">利用不可配送間隔情報</param>
		/// <param name="fieldName">対象項目名（定期購入区分1設定or定期購入区分3設定or定期購入区分4設定）</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="fixedPurchaseSetting">定期配送設定値</param>
		/// <returns>有効か</returns>
		public static bool IsEffectiveFixedPurchaseSetting(IEnumerable<string> limitedSetting, string fieldName, string shippingId, string fixedPurchaseSetting)
		{
			// 定期購入区分1設定の場合は、月間隔取得
			// 定期購入区分3設定の場合は、日間隔取得
			// 定期購入区分4設定の場合は、週間隔取得
			var value = ((fieldName == Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING)
				|| (fieldName == Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING1))
				? fixedPurchaseSetting.Split(',')[0]
				: fixedPurchaseSetting;
			var limitedValue = GetFixedPurchaseIntervalSettingExceptLimitedValue(limitedSetting.ToList(), fieldName, shippingId);
			var isEffective = ((string.IsNullOrEmpty(limitedValue) == false) && limitedValue.Split(',').Contains(value));
			return isEffective;
		}

		/// <summary>
		/// 利用不可値を除く定期配送間隔情報取得
		/// </summary>
		/// <param name="limitedSetting">利用不可配送間隔情報</param>
		/// <param name="fieldName">対象項目名（定期購入区分1設定or定期購入区分3設定or定期購入区分4設定）</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>利用不可値を除く定期配送間隔情報</returns>
		public static string GetFixedPurchaseIntervalSettingExceptLimitedValue(List<string> limitedSetting, string fieldName, string shippingId)
		{
			// 配送種別の該当配送区分設定情報を取得
			var shopShipping = GetShopShipping(Constants.CONST_DEFAULT_SHOP_ID, shippingId);

			if ((fieldName == Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING2)
				&& ((shopShipping.DataSource[fieldName] == null)
				|| string.IsNullOrEmpty(shopShipping.DataSource[fieldName].ToString())))
				return string.Join(",", ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST).Select(x => x.Text));
			var setting = shopShipping.DataSource[fieldName].ToString();

			// 利用不可配送間隔情報がなければ、配送種別の配送間隔情報で戻る
			if (limitedSetting.Count == 0) return setting;

			// 利用不可配送間隔に考慮し、配送間隔を作成
			var resultSetting = setting.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			limitedSetting.ForEach(item => resultSetting = resultSetting.Except(
				item.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)).ToArray());

			return string.Join(",", resultSetting);
		}

		/// <summary>
		/// 定期購入区分1設定値（Xか月ごとorX日間隔）ドロップダウンリスト作成
		/// </summary>
		/// <param name="baseSetting">ベース設定値</param>
		/// <param name="selectedValue">デフォルト選択値</param>
		/// <param name="isIntervalDays">日付選択肢か</param>
		/// <returns>定期購入区分1または区分3設定値ドロップダウンリスト</returns>
		public static ListItem[] GetKbn1FixedPurchaseIntervalListItems(string baseSetting, string selectedValue, bool isIntervalDays = false)
		{
			return GetFixedPurchaseIntervalListItems(baseSetting, selectedValue, true, isIntervalDays);
		}

		/// <summary>
		/// 定期購入区分3設定値（Xか月ごとorX日間隔）ドロップダウンリスト作成
		/// </summary>
		/// <param name="baseSetting">ベース設定値</param>
		/// <param name="selectedValue">デフォルト選択値</param>
		/// <returns>定期購入区分1または区分3設定値ドロップダウンリスト</returns>
		public static ListItem[] GetKbn3FixedPurchaseIntervalListItems(string baseSetting, string selectedValue)
		{
			return GetFixedPurchaseIntervalListItems(baseSetting, selectedValue, false);
		}
		/// <summary>
		/// 定期購入区分4設定値（X週間隔）ドロップダウンリスト作成
		/// </summary>
		/// <param name="baseSetting">ベース設定値</param>
		/// <param name="selectedValue">デフォルト選択値</param>
		/// <returns>定期購入区分4ドロップダウンリスト</returns>
		public static ListItem[] GetKbn4Setting1FixedPurchaseIntervalListItems(string baseSetting, string selectedValue)
		{
			var fixedPurchaseIntervalListItems = GetKbn4FixedPurchaseIntervalListItems(baseSetting, selectedValue, true);
			return fixedPurchaseIntervalListItems;
		}

		/// <summary>
		/// 定期購入区分4設定値（曜日指定）ドロップダウンリスト作成
		/// </summary>
		/// <param name="baseSetting">ベース設定値</param>
		/// <param name="selectedValue">デフォルト選択値</param>
		/// <returns>定期購入区分4ドロップダウンリスト</returns>
		public static ListItem[] GetKbn4Setting2FixedPurchaseIntervalListItems(string baseSetting, string selectedValue)
		{
			var fixedPurchaseIntervalListItems = GetKbn4FixedPurchaseIntervalListItems(baseSetting, selectedValue, false);
			return fixedPurchaseIntervalListItems;
		}

		/// <summary>
		/// 定期購入区分1または区分3設定値（Xか月ごとorX日間隔）ドロップダウンリスト作成
		/// </summary>
		/// <param name="baseSetting">ベース設定値</param>
		/// <param name="selectedValue">デフォルト選択値</param>
		/// <param name="isIntervalMonths">定期購入区分１か（TRUE：定期購入区分１；FALSE：定期購入区分３）</param>
		/// <param name="isIntervalDays">日付選択肢か</param>
		/// <returns>定期購入区分1または区分3設定値ドロップダウンリスト</returns>
		private static ListItem[] GetFixedPurchaseIntervalListItems(
			string baseSetting,
			string selectedValue,
			bool isIntervalMonths,
			bool isIntervalDays = false
			)
		{
			var usebleBaseSetting = Regex.Replace(baseSetting, @"\(.*?\)", "");
			var items = usebleBaseSetting.Split(',')
				.Where(s => string.IsNullOrEmpty(s) == false)
				.Select(s => new ListItem(s, s)).Distinct().ToList();

			// デフォルト選択値がリストになければ追加
			if ((string.IsNullOrEmpty(selectedValue) == false)
				&& (items.Any(s => (s.Value == selectedValue)) == false))
			{
				items.Add(new ListItem(selectedValue, selectedValue));
			}

			if (isIntervalDays)
			{
				items = items.OrderBy(item =>
				{
					var result = 0;
					if ((int.TryParse(item.Text, out result) == false) || (item.Text == Constants.DATE_PARAM_END_OF_MONTH_VALUE))
					{
						result = Constants.DATE_PARAM_END_OF_MONTH_TEXT;
						item.Value = Constants.DATE_PARAM_END_OF_MONTH_VALUE;
						item.Text = CommonPage.ReplaceTag("@@DispText.fixed_purchase_kbn.endOfMonth@@");
					}
					else
					{
						result = int.Parse(item.Value);
					}
					return result;
				}).ToList();
			}
			else
			{
				items.Sort((x, y) => int.Parse(x.Value).CompareTo(int.Parse(y.Value)));
			}

			if (isIntervalMonths
				|| (Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG == false))
			{
				items.Insert(0, new ListItem("", ""));
			}
			return items.ToArray();
		}

		/// <summary>
		/// 定期購入区分4設定値（週ごとor曜日指定）ドロップダウンリスト作成
		/// </summary>
		/// <param name="baseSetting">ベース設定値</param>
		/// <param name="selectedValue">デフォルト選択値</param>
		/// <param name="isWeekInterval">週間隔か曜日指定か（TRUE：週間隔；FALSE：曜日指定）</param>
		/// <returns>定期購入区分4設定値ドロップダウンリスト</returns>
		private static ListItem[] GetKbn4FixedPurchaseIntervalListItems(
			string baseSetting,
			string selectedValue,
			bool isWeekInterval)
		{
			var dayOfWeeks = ValueText.GetValueItemArray(
				Constants.TABLE_SHOPSHIPPING,
				Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST)
				.Where(dow => (string.IsNullOrEmpty(dow.Value) == false));

			var usableBaseSettings = Regex.Replace(baseSetting, @"\([0-9]*\)", string.Empty).Split(',');
			var fixedPurchaseIntervalItems = isWeekInterval
				? usableBaseSettings
					.Where(s => (string.IsNullOrEmpty(s) == false))
					.Select(s => new ListItem(s, s)).Distinct().ToList()
				: dayOfWeeks.Where(dayOfWeek => usableBaseSettings.Any(s => s.Contains(dayOfWeek.Value))).ToList();

			// デフォルト選択値がリストになければ追加
			if ((string.IsNullOrEmpty(selectedValue) == false)
				&& (fixedPurchaseIntervalItems.Any(s => (s.Value == selectedValue)) == false))
			{
				var addItem = isWeekInterval
					? new ListItem(selectedValue, selectedValue)
					: dayOfWeeks.ElementAtOrDefault(int.Parse(selectedValue));
				fixedPurchaseIntervalItems.Add(addItem);
			}

			fixedPurchaseIntervalItems.Sort((x, y) => int.Parse(x.Value).CompareTo(int.Parse(y.Value)));
			fixedPurchaseIntervalItems.Insert(0, new ListItem(string.Empty, string.Empty));

			return fixedPurchaseIntervalItems.ToArray();
		}

		/// <summary>
		/// 指定定期購入配送間隔日・月のチェック
		/// </summary>
		/// <param name="items">利用不可定期購入配送間隔（日・月）の含まれるハッシュテーブルリスト</param>
		/// <param name="selectedValue">選択中の値</param>
		/// <param name="fieldName">チェックする項目名</param>
		/// <returns>選択している値が規定の配送パターンかどうか</returns>
		public static bool CheckSpecificFixedPurchaseIntervalMonthAndDay(List<Hashtable> items, string selectedValue, string fieldName)
		{
			if (items.Count < 1) return true;

			// 利用不可配送間隔日/月を取得
			var limitedInterval = items
				.Where(item => string.IsNullOrEmpty((string)item[fieldName]) == false)
				.Select(item => (string)item[fieldName]).Distinct().ToList();

			var result = limitedInterval.All(
				item => item.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(selectedValue) == false);

			return result;
		}

		/// <summary>
		/// 定期購入設定文言作成
		/// </summary>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <returns>定期購入設定文言</returns>
		public static string CreateFixedPurchaseSettingMessage(FixedPurchaseModel fixedPurchase)
		{
			return CreateFixedPurchaseSettingMessage(fixedPurchase.FixedPurchaseKbn, fixedPurchase.FixedPurchaseSetting1);
		}
		/// <summary>
		/// 定期購入設定文言作成
		/// </summary>
		/// <param name="fixedPurchaseKbn">定期購入区分</param>
		/// <param name="fixedPurchaseSetting1">定期購入設定１</param>
		/// <returns>定期購入設定文言</returns>
		public static string CreateFixedPurchaseSettingMessage(string fixedPurchaseKbn, string fixedPurchaseSetting1)
		{
			switch (fixedPurchaseKbn)
			{
				// 月間隔日付指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
					var monthAndDay = fixedPurchaseSetting1.Split(',');
					var month = (monthAndDay.Length > 1) ? monthAndDay[0] : "1";
					var day = (monthAndDay.Length > 1) ? monthAndDay[1] : fixedPurchaseSetting1;
					// 値が「月末」だった場合言語切り替え
					day = ((Regex.IsMatch(day, @"^[0-9]+$") == false) || (day == Constants.DATE_PARAM_END_OF_MONTH_VALUE))
						? CommonPage.ReplaceTag("@@DispText.fixed_purchase_kbn.endOfMonth@@")
						: day;
					var monthlyDateFormat = CommonPage.ReplaceTag("@@DispText.fixed_purchase_kbn.monthlyDate@@");
					return string.Format(monthlyDateFormat, month, day);

				// 月間隔・週・曜日指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
					var monthWeekDay = fixedPurchaseSetting1.Split(',');
					var monthValue = (monthWeekDay.Length > 2) ? monthWeekDay[0] : "1";
					var week = (monthWeekDay.Length > 2) ? monthWeekDay[1] : monthWeekDay[0];
					var weekDay = (monthWeekDay.Length > 2) ? monthWeekDay[2] : monthWeekDay[1];
					week = ValueText.GetValueText(
						Constants.TABLE_SHOPSHIPPING,
						Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST,
						week);
					weekDay = ValueText.GetValueText(Constants.TABLE_SHOPSHIPPING,
						Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST,
						weekDay);
					var monthlyWeekDayFormat = CommonPage.ReplaceTag("@@DispText.fixed_purchase_kbn.monthlyWeekAndDay@@");
					return string.Format(monthlyWeekDayFormat, monthValue, week, weekDay);

				// 配送間隔指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
					var intervalByDaysFormat = CommonPage.ReplaceTag("@@DispText.fixed_purchase_kbn.intervalByDays@@");
					return string.Format(intervalByDaysFormat, fixedPurchaseSetting1);

				// 月間隔・週・曜日指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
					var everyNWeekList = fixedPurchaseSetting1.Split(',');
					var weekValue = everyNWeekList[0];
					var dayOfWeekValue = everyNWeekList[1];
					dayOfWeekValue = ValueText.GetValueText(
						Constants.TABLE_SHOPSHIPPING,
						Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST,
						dayOfWeekValue);
					var weekAndDayOfWeekFormat = CommonPage.ReplaceTag("@@DispText.fixed_purchase_kbn.weekAndDayOfWeek@@");

					var weekAndDayOfWeek = string.Format(weekAndDayOfWeekFormat, weekValue, dayOfWeekValue);
					return weekAndDayOfWeek;

				default:
					return "";
			}
		}

		/// <summary>
		/// 注文情報更新ロック取得処理
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void GetUpdlockFromOrderTables(string orderId, SqlAccessor accessor)
		{
			using (var statement = new SqlStatement("SELECT * FROM w2_Order WITH (UPDLOCK) WHERE order_id = '" + orderId.Replace("'", "''") + "'"))
			{
				statement.ExecStatement(accessor);
			}
			using (var statement = new SqlStatement("SELECT * FROM w2_OrderOwner WITH (UPDLOCK) WHERE order_id = '" + orderId.Replace("'", "''") + "'"))
			{
				statement.ExecStatement(accessor);
			}
			using (var statement = new SqlStatement("SELECT * FROM w2_OrderShipping WITH (UPDLOCK) WHERE order_id = '" + orderId.Replace("'", "''") + "'"))
			{
				statement.ExecStatement(accessor);
			}
			using (var statement = new SqlStatement("SELECT * FROM w2_OrderItem WITH (UPDLOCK) WHERE order_id = '" + orderId.Replace("'", "''") + "'"))
			{
				statement.ExecStatement(accessor);
			}
		}

		/// <summary>
		/// 購入商品毎に紐付くシリアルキーリストを取得
		/// </summary>
		/// <param name="orderItem">購入商品情報</param>
		/// <returns>購入後に有効なシリアルキー情報</returns>
		public static DataView GetSerialKeyList(DataRowView orderItem)
		{
			var dv = new DataView();
			if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
			{
				dv = new SerialKeyService().GetDeliveredSerialKeysInDataView(
					(string)orderItem[Constants.FIELD_ORDERITEM_ORDER_ID],
					(int)orderItem[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO],
					Constants.DIGITAL_CONTENTS_SERIAL_KEY_VALID_DAYS);

				foreach (DataRowView drv in dv)
				{
					drv[Constants.FIELD_SERIALKEY_SERIAL_KEY] =
						SerialKeyUtility.GetFormattedKeyString(SerialKeyUtility.DecryptSerialKey(StringUtility.ToEmpty(drv[Constants.FIELD_SERIALKEY_SERIAL_KEY])));
				}
			}
			return dv;
		}
		/// <summary>
		/// 購入商品毎に紐付くシリアルキーリストを取得
		/// </summary>
		/// <param name="orderItem">購入商品情報</param>
		/// <returns>購入後に有効なシリアルキー情報</returns>
		public static DataView GetSerialKeyList(OrderItemInput orderItem)
		{
			var dv = new DataView();
			if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
			{
				dv = new SerialKeyService().GetDeliveredSerialKeysInDataView(
					orderItem.OrderId,
					int.Parse(orderItem.OrderItemNo),
					Constants.DIGITAL_CONTENTS_SERIAL_KEY_VALID_DAYS);

				foreach (DataRowView drv in dv)
				{
					drv[Constants.FIELD_SERIALKEY_SERIAL_KEY] =
						SerialKeyUtility.GetFormattedKeyString(SerialKeyUtility.DecryptSerialKey(StringUtility.ToEmpty(drv[Constants.FIELD_SERIALKEY_SERIAL_KEY])));
				}
			}
			return dv;
		}

		/// <summary>
		/// 新ID生成
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="key">取得対象キー</param>
		/// <returns>新ID</returns>
		private static string CreateNewNumberingId(string shopId, string key)
		{
			string result;
			switch (Constants.ORDER_NUMBERING_TYPE)
			{
				case Constants.OrderNumberingType.Sequence:
					result = NumberingUtility.CreateNewNumber(shopId, key).ToString().PadLeft(Constants.CONST_ORDER_ID_LENGTH, '0');
					break;

				case Constants.OrderNumberingType.DailySequence:
				default:
					result = DateTime.Now.ToString("yyyyMMdd") + NumberingUtility.CreateNewNumber(shopId, key, NumberingUtility.ResetTiming.Day).ToString().PadLeft(Constants.CONST_ORDER_ID_LENGTH, '0');
					break;
			}
			return result;
		}

		/// <summary>
		/// アプリケーションが出荷情報登録可能か
		/// </summary>
		/// <returns>出荷情報登録可能か</returns>
		public static bool CanShipmentEntry()
		{
			return ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc)
				|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.YamatoKa)
				|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo)
				|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene)
				|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk)
				|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom)
				|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score)
				|| (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Veritrans)
				|| (string.IsNullOrEmpty(Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_DELIVERY_COMPANY_CODE) == false)
				|| Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED);
		}
		/// <summary>
		/// 出荷情報登録可能か
		/// </summary>
		/// <param name="orderPaymentKbn">決済種別ID</param>
		/// <returns>出荷情報登録可能か</returns>
		public static bool CanShipmentEntry(string orderPaymentKbn)
		{
			return (((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc) && (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT))
				|| ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.YamatoKa) && (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
				|| ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo) && (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
				|| ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene) && (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
				|| ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom) && (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
				|| ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.YamatoKa) && (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF))
				|| ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk) && (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
				|| ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score) && (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
				|| ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Veritrans) && (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA));
		}

		/// <summary>
		/// クレジットカード会社名取得
		/// </summary>
		/// <param name="value">会社コード</param>
		/// <returns>会社名</returns>
		public static string GetCreditCardCompanyName(object value)
		{
			return ValueText.GetValueText(Constants.TABLE_ORDER, CreditCompanyValueTextFieldName, value);
		}

		/// <summary>
		/// 定期会員フラグ更新
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="fixedPurchaseMemberFlg">定期会員フラグ</param>
		/// <param name="accessor">SQLアクセサ</param>
		private static bool UpdateFixedPurchaseMemberFlg(
			Hashtable order,
			string fixedPurchaseMemberFlg,
			SqlAccessor accessor)
		{
			var updated = new UserService().UpdateFixedPurchaseMemberFlg(
				(string)order[Constants.FIELD_ORDER_USER_ID],
				fixedPurchaseMemberFlg,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert,
				accessor);
			return updated;
		}

		/// <summary>
		/// デフォルト支払回数の値を取得
		/// </summary>
		/// <returns>ValueTextの支払回数の値</returns>
		public static string GetCreditInstallmentsDefaultValue()
		{
			switch (Constants.PAYMENT_CARD_KBN)
			{
				case Constants.PaymentCard.SBPS:
					return Constants.FIELD_CREDIT_INSTALLMENTS_SBPS_VALUE;

				case Constants.PaymentCard.YamatoKwc:
					return Constants.FIELD_CREDIT_INSTALLMENTS_YAMATOKWC_VALUE;

				case Constants.PaymentCard.Zeus:
					return Constants.FIELD_CREDIT_INSTALLMENTS_VALUE;

				case Constants.PaymentCard.EScott:
					return Constants.FIELD_CREDIT_INSTALLMENTS_ESCOTT_VALUE;

				case Constants.PaymentCard.VeriTrans:
					return Constants.FIELD_CREDIT_INSTALLMENTS_VERITRANS_VALUE;

				case Constants.PaymentCard.Rakuten:
					return Constants.FIELD_CREDIT_INSTALLMENTS_RAKUTEN_VALUE;

				case Constants.PaymentCard.Gmo:
					return Constants.FIELD_CREDIT_INSTALLMENTS_GMO_VALUE;

				case Constants.PaymentCard.Paygent:
					return Constants.FIELD_CREDIT_INSTALLMENTS_PAYGENT_VALUE;

				default:
					throw new Exception(string.Format("未対応のカード区分：{0}", Constants.PAYMENT_CARD_KBN));
			}
		}

		/// <summary>
		/// クレジットカード与信後決済か？
		/// </summary>
		/// <param name="hasDigitalContents">デジタルコンテンツ含む</param>
		public static bool IsCreditPaymentAffterAuth(bool hasDigitalContents)
		{
			var enabledDigitalContents = (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && hasDigitalContents);
			switch (Constants.PAYMENT_CARD_KBN)
			{
				case Constants.PaymentCard.Zeus:
					var zeusPaymentMethod = enabledDigitalContents
						? Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS
						: Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD;
					return (zeusPaymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH);
				case Constants.PaymentCard.SBPS:
					var sbpsPaymentMethod = enabledDigitalContents
						? Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS
						: Constants.PAYMENT_SETTING_SBPS_CREDIT_PAYMENTMETHOD;
					return (sbpsPaymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH);
				case Constants.PaymentCard.EScott:
					var escottPaymentMethod = enabledDigitalContents
						? Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD_FORDIGITALCONTENTS
						: Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_PAYMENTMETHOD;
					return (escottPaymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH);

				default:
					return false;
			}
		}

		/// <summary>
		/// 元注文ID取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>元注文ID(取得できない場合はnull）</returns>
		public static string GetOrderIdOrg(string orderId)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("Order", "GetOrderIdOrg"))
			{
				var input = new Hashtable
				{
					{Constants.FIELD_ORDER_ORDER_ID, orderId}
				};
				var dv = statement.SelectSingleStatement(accessor, input);
				return (dv.Count != 0) ? (string)dv[0][0] : null;
			}
		}

		#region +GetOrderQuantity 注文個数取得
		/// <summary>
		/// 注文個数取得
		/// </summary>
		/// <returns>パラメタに指定された条件で取得したProductStockテーブルのデータを持つDataView</returns>
		public DataView GetOrderQuantity()
		{
			DataView dataView;

			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement(Resources.ResourceManager, "Order", "Order", "GetOrderQuantity"))
			{
				var input = new Hashtable
				{
					{Constants.FIELD_PRODUCTSTOCK_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID},
				};
				dataView = statement.SelectSingleStatementWithOC(accessor, input);
			}

			return dataView;
			// 他エラーはcatchせずそのまま呼び出し上位で捉える
		}
		#endregion

		/// <summary>
		/// 出荷所要営業日数を考慮し、配送希望日のドロップダウンリスト取得
		/// </summary>
		/// <param name="shopShipping">配送種別情報</param>
		/// <param name="isFront">フロントからか</param>
		/// <returns>配送希望日のリスト</returns>
		public static ListItemCollection GetListItemShippingDate(ShopShippingModel shopShipping, bool isFront = false)
		{
			var startAddDayCount = shopShipping.ShippingDateSetBegin.GetValueOrDefault(0);
			var maxAddDayCount = shopShipping.ShippingDateSetEnd.GetValueOrDefault(0);

			var startDateAfterHoliday = HolidayUtil.GetDateOfBusinessDay(
				DateTime.Now,
				shopShipping.BusinessDaysForShipping,
				true);

			var shippingDateList = new ListItemCollection();
			if (Constants.DISPLAY_DEFAULTSHIPPINGDATE_ENABLED)
			{
				shippingDateList.Add(
					new ListItem(
						isFront ? CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@") : string.Empty,
						string.Empty));
			}
			for (var loop = startAddDayCount; loop < (startAddDayCount + maxAddDayCount); loop++)
			{
				var target = startDateAfterHoliday.AddDays(loop);
				shippingDateList.Add(new ListItem(
					isFront
						? DateTimeUtility.ToStringFromRegion(target, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)
						: DateTimeUtility.ToStringForManager(
							target,
							DateTimeUtility.FormatType.LongDateWeekOfDay1Letter),
					target.ToString("yyyy/MM/dd")));
			}

			return shippingDateList;
		}

		/// <summary>
		/// Get Shipping Date  For Convenience Store
		/// </summary>
		/// <param name="isFront">isFront</param>
		/// <returns>List of desired delivery dates</returns>
		public static ListItem[] GetShippingDateForConvenienceStore(bool isFront = false)
		{
			var shippingDates = new List<ListItem>();

			if (Constants.RECEIVING_STORE_TWPELICAN_CVS_SHIPPING_DATE.Length == 0) return new ListItem[0];

			var maxAddDayCount = int.Parse(Constants.RECEIVING_STORE_TWPELICAN_CVS_SHIPPING_DATE[1]);
			var beginAddDayCount = int.Parse(Constants.RECEIVING_STORE_TWPELICAN_CVS_SHIPPING_DATE[0]);
			if (Constants.DISPLAY_DEFAULTSHIPPINGDATE_ENABLED)
			{
				shippingDates.Add(
					new ListItem(
						isFront ? CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@") : string.Empty,
						string.Empty));
			}

			for (var index = beginAddDayCount; index < (maxAddDayCount + beginAddDayCount); index++)
			{
				var target = DateTime.Now.AddDays(index);
				shippingDates.Add(new ListItem(
					isFront
						? DateTimeUtility.ToStringFromRegion(target, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)
						: DateTimeUtility.ToStringForManager(
							target,
							DateTimeUtility.FormatType.LongDateWeekOfDay1Letter),
					target.ToString("yyyy/MM/dd")));
			}

			return shippingDates.ToArray();
		}

		#region +GetOrderPointUsable ポイント利用可能額取得
		/// <summary>
		/// ポイント利用可能額取得
		/// </summary>
		/// <param name="orderPriceSubtotal">商品合計金額</param>
		/// <param name="orderPriceRegulationTotal">調整金額合計</param>
		/// <param name="memberRankDiscount">会員ランク割引額</param>
		/// <param name="useCouponPriceForProduct">クーポン商品割引額</param>
		/// <param name="setPromotionProductDiscountAmount">セットプロモーション商品割引額</param>
		/// <param name="orderFixedPurchaseMemberDiscountAmount">定期会員割引額</param>
		/// <param name="orderFixedPurchaseDiscountPrice">定期購入割引額</param>
		/// <returns>ポイント利用可能額</returns>
		public static decimal GetOrderPointUsable(
			decimal orderPriceSubtotal,
			decimal orderPriceRegulationTotal,
			decimal memberRankDiscount,
			decimal useCouponPriceForProduct,
			decimal setPromotionProductDiscountAmount,
			decimal orderFixedPurchaseMemberDiscountAmount,
			decimal orderFixedPurchaseDiscountPrice)
		{
			var pointUsable = orderPriceSubtotal
				+ orderPriceRegulationTotal
				- memberRankDiscount
				- useCouponPriceForProduct
				- setPromotionProductDiscountAmount
				- orderFixedPurchaseMemberDiscountAmount
				- orderFixedPurchaseDiscountPrice;
			return pointUsable;
		}
		#endregion

		#region +GetPriceCartTotalWithoutPaymentPrice カート支払い金額合計取得
		/// <summary>
		/// カート支払い金額合計取得
		/// </summary>
		/// <param name="order">受注情報</param>
		/// <returns>カート支払い金額合計</returns>
		public static decimal GetPriceCartTotalWithoutPaymentPrice(OrderModel order)
		{
			// 商品合計(税込) + 配送料(税込) + 調整金額 - 会員ランク割引額 - クーポン割引額 - ポイント利用料 - セットプロモーション商品割引額 - セットプロモーション配送料割引額 - 定期会員割引額 - 定期購入割引額
			var priceCartTotalWithoutPaymentPrice =
				TaxCalculationUtility.GetPriceTaxIncluded(order.OrderPriceSubtotal, order.OrderPriceSubtotalTax)
				+ order.OrderPriceShipping
				+ order.OrderPriceRegulationTotal
				- PriceCalculator.GetOrderPriceDiscountTotal(
					order.MemberRankDiscountPrice,
					order.SetpromotionProductDiscountAmount,
					order.SetpromotionShippingChargeDiscountAmount,
					0m, // 決済手数料割引は含めない
					order.OrderPointUseYen,
					order.OrderCouponUse,
					order.FixedPurchaseMemberDiscountAmount,
					order.FixedPurchaseDiscountPrice);
			return priceCartTotalWithoutPaymentPrice;
		}
		#endregion

		#region +IsExternalPayment 外部決済か判定
		/// <summary>
		/// 外部決済か判定
		/// </summary>
		/// <param name="orderPaymentKbn">支払い区分</param>
		/// <returns>外部決済の場合TRUE、それ以外の場合FALSE</returns>
		public static bool IsExternalPayment(string orderPaymentKbn)
		{
			var result = ((orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL_SBPS)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
				|| (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA));

			return result;
		}
		#endregion

		#region +GetOrderSearchOrderByStringForOrderListAndWorkflow 検索向けORDER BY文字列取得
		/// <summary>
		/// 検索向けORDER BY文字列取得
		/// </summary>
		/// <param name="sortKbn">ソート区分</param>
		/// <returns>ORDER BY文字列</returns>
		public static string GetOrderSearchOrderByStringForOrderListAndWorkflow(string sortKbn)
		{
			const string ORDER_BY = "ORDER BY";
			switch (sortKbn)
			{
				case "0":
					return ORDER_BY + " w2_Order.order_id ASC";

				case "1":
					return ORDER_BY + " w2_Order.order_id DESC";

				case "2":
					return ORDER_BY + " w2_Order.order_date ASC, w2_Order.order_id ASC";

				case "3":
					return ORDER_BY + " w2_Order.order_date DESC, w2_Order.order_id ASC";

				case "4":
					return ORDER_BY + " w2_Order.date_created ASC, w2_Order.order_id ASC";

				case "5":
					return ORDER_BY + " w2_Order.date_created DESC, w2_Order.order_id ASC";

				case "6":
					return ORDER_BY + " w2_Order.date_changed ASC, w2_Order.order_id ASC";

				case "7":
					return ORDER_BY + " w2_Order.date_changed DESC, w2_Order.order_id ASC";

				case "100":
					return ORDER_BY + " w2_Order.order_return_exchange_receipt_date ASC, w2_Order.order_id ASC";

				case "101":
					return ORDER_BY + " w2_Order.order_return_exchange_receipt_date DESC, w2_Order.order_id ASC";

				default:
					return ORDER_BY + " w2_Order.order_id ASC";
			}
		}
		#endregion

		#region +GetOrderSearchOrderByStringForOrderItemListAndWorkflow 検索向けORDER BY文字列取得(OrderItemNo追加)
		/// <summary>
		/// 検索向けORDER BY文字列取得(OrderItemNo追加)
		/// </summary>
		/// <param name="sortKbn">ソート区分</param>
		/// <param name="statementKey">XMLファイル名</param>
		/// <param name="statementValue">SQL Statement名</param>
		/// <returns>ORDER BY文字列</returns>
		public static string GetOrderSearchOrderByStringForOrderItemListAndWorkflow(string sortKbn, string statementKey, string statementValue)
		{
			// ORDER BY文字列への「order_item_no」追加判定
			var isOrderItemList = (statementValue == "GetOrderItemMaster")
				|| (statementValue == "GetOrderItemWorkflowMaster")
				|| (statementValue == "GetOrderItemPdf")
				|| (statementValue == "GetOrderItemWorkflowPdf");

			var orderbyString = GetOrderSearchOrderByStringForOrderListAndWorkflow(sortKbn) + (isOrderItemList ? ", w2_OrderItem.order_item_no" : "");
			return orderbyString;
		}
		#endregion

		#region +ShipmentEntry 出荷情報登録（外部連携）
		/// <summary>
		/// 出荷情報登録（外部連携）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="orderPaymentKbn">決済種別</param>
		/// <param name="shippingCheckNoNew">新送り状番号</param>
		/// <param name="shippingCheckNoOld">元送り状番号</param>
		/// <param name="cardTranId">
		/// 決済取引ID
		/// Gmo後払いの場合はGmo取引ID
		/// Atodeneの場合はお問い合わせ番号
		/// </param>
		/// <param name="deliveryCompanyType">出荷連携配送会社</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">Sql accessor</param>
		/// <returns>エラーメッセージ</returns>
		public static string ShipmentEntry(
			string orderId,
			string paymentOrderId,
			string orderPaymentKbn,
			string shippingCheckNoNew,
			string shippingCheckNoOld,
			string cardTranId,
			string deliveryCompanyType,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor = null)
		{
			// 外部連携：ヤマトKWC 出荷情報登録
			if (((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc)
				&& (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)))
			{
				var errorMessage = ShipmentEntryYamatoKwc(
					paymentOrderId,
					orderPaymentKbn,
					shippingCheckNoNew,
					PaymentYamatoKwcDeliveryServiceCode.GetDeliveryServiceCode(deliveryCompanyType));
				return errorMessage;
			}

			// 外部連携：ヤマト（後払い）出荷情報依頼
			if ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.YamatoKa)
				&& (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
			{
				var errorMessage = ShipmentEntryYamatoDef(
					orderId,
					paymentOrderId,
					shippingCheckNoNew,
					shippingCheckNoOld,
					deliveryCompanyType,
					updateHistoryAction,
					sqlAccessor);
				return errorMessage;
			}

			// GMO後払い：出荷報告
			if ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Gmo)
				&& (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
			{
				var errorMessage = ShipmentGmoDef(orderId, cardTranId, shippingCheckNoNew, updateHistoryAction, deliveryCompanyType, sqlAccessor);
				return errorMessage;
			}

			// Atodene後払い：出荷報告
			if ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene)
				&& (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
			{
				var errorMessage = ShipmentAtodene(
					cardTranId,
					shippingCheckNoNew,
					orderId,
					PaymentAtodeneDeliveryServiceCode.GetDeliveryServiceCode(deliveryCompanyType),
					updateHistoryAction,
					sqlAccessor);
				return errorMessage;
			}

			// Atobaraicom後払い：出荷報告
			if ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom)
				&& (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
			{
				var errorMessage = ShipmentAtobaraicom(
					cardTranId,
					shippingCheckNoNew,
					orderId,
					orderPaymentKbn,
					deliveryCompanyType,
					updateHistoryAction,
					sqlAccessor);
				return errorMessage;
			}

			// DSK後払い：出荷報告
			if ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk)
				&& (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
			{
				var errorMessage = ShipmentDskDeferred(
					orderId,
					cardTranId,
					shippingCheckNoNew,
					PaymentDskDeferredDeliveryServiceCode.GetDeliveryServiceCode(deliveryCompanyType),
					updateHistoryAction,
					sqlAccessor);
				return errorMessage;
			}

			// スコア後払い：出荷報告
			if ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Score)
				&& (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
			{
				var errorMessage = ShipmentScoreDef(
					orderId,
					cardTranId,
					shippingCheckNoNew,
					PaymentScoreDeferredDeliveryServiceCode.GetDeliveryServiceCode(deliveryCompanyType),
					updateHistoryAction,
					sqlAccessor);
				return errorMessage;
			}

			// ベリトランス後払い：出荷報告
			if ((Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Veritrans)
				&& (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF))
			{
				var errorMessage = ShipmentVeritransDef(
					orderId,
					shippingCheckNoNew,
					updateHistoryAction,
					sqlAccessor);
				return errorMessage;
			}

			// 後付款(TriLink後払い)出荷報告処理
			if (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				var errorMessage = ShippingEntryTriLinkAfterPay(orderId, cardTranId, shippingCheckNoNew, updateHistoryAction, sqlAccessor);
				return errorMessage;
			}

			// NP後払い：出荷報告
			if (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				var errorMessage = ShipmentNPAfterPay(
					orderId,
					cardTranId,
					shippingCheckNoNew,
					deliveryCompanyType,
					updateHistoryAction,
					sqlAccessor);
				return errorMessage;
			}

			// GMOアトカラ：出荷報告
			if (orderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)
			{
				var errorMessage = ShipmentGmoAtokara(
					orderId,
					cardTranId,
					shippingCheckNoNew,
					deliveryCompanyType,
					updateHistoryAction,
					sqlAccessor);
				return errorMessage;
			}
			return "";
		}
		#endregion

		#region +ShipmentEntryYamatoKwc ヤマトKWC 出荷情報登録
		/// <summary>
		/// ヤマトKWC 出荷情報登録
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="orderPaymentKbn">決済種別</param>
		/// <param name="shippingCheckNoNew">新送り状番号</param>
		/// <param name="deliveryServiceCode">配送サービスコード</param>
		/// <returns>エラーメッセージ</returns>
		public static string ShipmentEntryYamatoKwc(string paymentOrderId, string orderPaymentKbn, string shippingCheckNoNew, string deliveryServiceCode)
		{
			var resultInfo = new PaymentYamatoKwcTradeInfoApi().Exec(paymentOrderId);
			if (resultInfo.Success == false)
			{
				return " 取引情報照会に失敗しました。" + resultInfo.ErrorInfoForLog;
			}
			if (resultInfo.ResultDatas.Count == 0)
			{
				return " 取引情報が取得できませんでした。";
			}

			var resultData = resultInfo.ResultDatas[0];
			var slipNoOld = PaymentYamatoKwcDeliveryServiceCode.CheckDeliveryServiceCodeYamato(resultData.DeliveryServiceCode) ? resultData.SlipNo : resultData.ExtraSlipNo;
			if (string.IsNullOrEmpty(slipNoOld) == false)
			{
				var resultCansel = new PaymentYamatoKwcShipmentCancelApi().Exec(paymentOrderId, slipNoOld);
				if (resultCansel.Success == false)
				{
					return " 出荷情報取消に失敗しました。" + resultCansel.ErrorInfoForLog;
				}
			}

			if (string.IsNullOrEmpty(shippingCheckNoNew)) return "";

			var resultEntry = new PaymentYamatoKwcShipmentEntryApi().Exec(paymentOrderId, shippingCheckNoNew, deliveryServiceCode);
			if (resultEntry.Success == false)
			{
				return " 出荷情報登録に失敗しました。" + resultEntry.ErrorInfoForLog;
			}
			return "";
		}
		#endregion

		#region +ShipmentEntryYamatoDef ヤマト決済（後払い）出荷情報登録
		/// <summary>
		/// ヤマト決済（後払い）出荷情報登録
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="shippingCheckNoNew">新送り状番号</param>
		/// <param name="shippingCheckNoOld">元送り状番号</param>
		/// <param name="deliveryCompanyType">出荷連携配送会社</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">Sql accessor</param>
		/// <returns>エラーメッセージ</returns>
		public static string ShipmentEntryYamatoDef(
			string orderId,
			string paymentOrderId,
			string shippingCheckNoNew,
			string shippingCheckNoOld,
			string deliveryCompanyType,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor = null)
		{
			var service = new OrderService();
			var processDiv = PaymentYamatoKaUtility.CreateProcessDiv(shippingCheckNoNew, shippingCheckNoOld, deliveryCompanyType);
			var shipmentApi = new PaymentYamatoKaShipmentEntryApi();
			if (shipmentApi.Exec(
				paymentOrderId,
				shippingCheckNoNew,
				processDiv,
				processDiv == PaymentYamatoKaProcessDiv.Update
					? PaymentYamatoKaUtility.CreateYamatoKaShipYmd()
					: null,
				shippingCheckNoOld) == false)
			{
				if (sqlAccessor == null)
				{
					service.UpdateExternalPaymentStatusShipmentError(
						orderId,
						shipmentApi.ResponseData.ErrorMessages,
						Constants.FLG_LASTCHANGED_BATCH,
						updateHistoryAction);
				}
				else
				{
					service.UpdateExternalPaymentStatusShipmentError(
					orderId,
					shipmentApi.ResponseData.ErrorMessages,
					Constants.FLG_LASTCHANGED_BATCH,
					updateHistoryAction,
					sqlAccessor);
				}
				return string.Format("{0}({1})", shipmentApi.ResponseData.ErrorMessages, shipmentApi.ResponseData.ErrorCode);
			}
			else
			{
				if (sqlAccessor == null)
				{
					// 成功？
					service.UpdateExternalPaymentStatusShipmentComplete(orderId, Constants.FLG_LASTCHANGED_BATCH, updateHistoryAction);
				}
				else
				{
					// 成功？
					service.UpdateExternalPaymentStatusShipmentComplete(orderId, Constants.FLG_LASTCHANGED_BATCH, updateHistoryAction, sqlAccessor);
				}
			}
			return string.Empty;
		}
		#endregion

		#region +ShipmentGmoDef Gmo後払い出荷報告
		/// <summary>
		/// Gmo後払い出荷報告
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="gmoTransactionId">Gmo取引ID</param>
		/// <param name="shippingCheckNo">配送伝票番号</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="deliveryCompanyType">出荷連携配送会社</param>
		/// <param name="sqlAccessor">Sql accessor</param>
		/// <returns>
		/// エラーメッセージ
		/// 成功の場合はEmpty
		/// </returns>
		public static string ShipmentGmoDef(
			string orderId,
			string gmoTransactionId,
			string shippingCheckNo,
			UpdateHistoryAction updateHistoryAction,
			string deliveryCompanyType,
			SqlAccessor sqlAccessor = null)
		{
			var request = new GmoRequestShipment();
			request.Transaction = new TransactionElement();
			request.Transaction.GmoTransactionId = gmoTransactionId;
			request.Transaction.Pdcompanycode = PaymentGmoDeliveryServiceCode.GetDeliveryServiceCode(deliveryCompanyType);
			request.Transaction.Slipno = shippingCheckNo;

			var facade = new GmoDeferredApiFacade();
			var result = facade.Shipment(request);
			if (result.Result != ResultCode.OK)
			{
				return string.Join("\r\n", result.Errors.Error.Select(x => string.Format("{0}:{1}", x.ErrorCode, x.ErrorMessage)).ToArray());
			}

			// 外部決済ステータスを更新
			var sv = new OrderService();
			if (sqlAccessor == null)
			{
				sv.UpdateExternalPaymentStatusShipmentComplete(orderId, Constants.FLG_LASTCHANGED_BATCH, updateHistoryAction);
			}
			else
			{
				sv.UpdateExternalPaymentStatusShipmentComplete(orderId, Constants.FLG_LASTCHANGED_BATCH, updateHistoryAction, sqlAccessor);
			}

			return "";
		}
		#endregion

		#region +ShipmentAtodene Atodene出荷報告
		/// <summary>
		/// Atodene出荷報告
		/// </summary>
		/// <param name="tranId">お問い合わせ番号</param>
		/// <param name="shippingCheckNo">配送伝票番号</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="deliveryServiceCode">出荷連携配送会社コード</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">Sql accessor</param>
		/// <returns>
		/// エラーメッセージ
		/// 成功時はEmpty
		/// </returns>
		public static string ShipmentAtodene(
			string tranId,
			string shippingCheckNo,
			string orderId,
			string deliveryServiceCode,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor = null)
		{
			// アダプタ生成してAPIたたく
			var adp = new AtodeneShippingAdapter(tranId, shippingCheckNo, deliveryServiceCode);
			var res = adp.Execute();

			if (res.Result != AtodeneConst.RESULT_OK)
			{
				if ((res.Errors != null) && (res.Errors.Error != null))
				{
					return string.Join("\r\n", res.Errors.Error.Select(e => string.Format("{0}:{1}", e.ErrorCode, e.ErrorMessage)).ToArray());
				}
			}

			var service = new OrderService();
			if (sqlAccessor == null)
			{
				// エラーなければ外部決済ステータス更新
				service.UpdateExternalPaymentStatusShipmentComplete(orderId, Constants.FLG_LASTCHANGED_BATCH, updateHistoryAction);
			}
			else
			{
				// エラーなければ外部決済ステータス更新
				service.UpdateExternalPaymentStatusShipmentComplete(orderId, Constants.FLG_LASTCHANGED_BATCH, updateHistoryAction, sqlAccessor);
			}

			return "";
		}
		#endregion

		#region +ShipmentAtobaraicom Atobaraicom出荷報告
		/// <summary>
		/// Atobaraicom出荷報告
		/// </summary>
		/// <param name="tranId">お問い合わせ番号</param>
		/// <param name="shippingCheckNo">配送伝票番号</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="deliveryCompanyType">出荷連携配送会社</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">Sql accessor</param>
		/// <returns>
		/// エラーメッセージ
		/// 成功時はEmpty
		/// </returns>
		public static string ShipmentAtobaraicom(
			string tranId,
			string shippingCheckNo,
			string orderId,
			string orderPaymentKbn,
			string deliveryCompanyType,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor = null)
		{
			var order = GetOrder(orderId, sqlAccessor);
			var paymentOrderId = (string)order[0][Constants.FIELD_ORDER_PAYMENT_ORDER_ID];
			var deliveryCompany = new DeliveryCompanyService().Get((string)order[0][Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID]);
			var deliveryId = (deliveryCompany != null)
				? deliveryCompany.DeliveryCompanyTypePostPayment
				: string.Empty;
			var invoiceBundleFlg = (string)order[0][Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG];
			// アダプタ生成してAPIたたく
			var res = new AtobaraicomShippingRegistrationApi().ExecShippingRegistration(
				paymentOrderId,
				shippingCheckNo,
				deliveryId,
				invoiceBundleFlg);

			if (res.IsSuccess == false)
			{
				return string.Join("\r\n", res.Messages.Select(e => string.Format("{0}:{1}", e.Message.Code, e.Message.MessageText)).ToArray());
			}

			var service = new OrderService();

			// エラーなければ外部決済ステータス更新
			service.UpdateExternalPaymentStatusShipmentComplete(orderId, Constants.FLG_LASTCHANGED_BATCH, updateHistoryAction, sqlAccessor);

			return "";
		}
		#endregion

		#region +ShipmentDskDeferred DSK後払い出荷報告
		/// <summary>
		/// DSK後払い出荷報告
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="tranId">お問い合わせ番号</param>
		/// <param name="shippingCheckNo">配送伝票番号</param>
		/// <param name="deliveryServiceCode">出荷連携配送会社コード</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">Sql accessor</param>
		/// <returns>エラーメッセージ</returns>
		public static string ShipmentDskDeferred(
			string orderId,
			string tranId,
			string shippingCheckNo,
			string deliveryServiceCode,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor = null)
		{
			var order = new OrderService().Get(orderId, sqlAccessor);

			// デジタルコンテンツの場合は配送コードは「その他」にする
			if (order.IsDigitalContents)
			{
				deliveryServiceCode = PaymentDskDeferredDeliveryServiceCode.DSK_DEFERRED_DELIVERY_SERVICE_CODE_OTHER;
			}

			var adapter = new DskDeferredShipmentAdapter(order, tranId, shippingCheckNo, deliveryServiceCode);
			var response = adapter.Execute();

			if ((response.IsResultOk == false) && (response.Errors != null) && (response.Errors.Error != null))
			{
				return string.Join("\r\n", response.Errors.Error.Select(e => string.Format("{0}:{1}", e.ErrorCode, e.ErrorMessage)).ToArray());
			}

			// エラーなければ外部決済ステータス更新
			new OrderService().UpdateExternalPaymentStatusShipmentComplete(orderId, Constants.FLG_LASTCHANGED_BATCH, updateHistoryAction, sqlAccessor);

			return "";
		}
		#endregion

		#region +ShipmentScoreDef スコア後払い出荷報告
		/// <summary>
		/// スコア後払い出荷報告
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="nissenTransactionId">スコア注文ID</param>
		/// <param name="shippingCheckNo">配送伝票番号</param>
		/// <param name="deliveryServiceCode">出荷連携配送会社コード</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">Sql accessor</param>
		/// <returns>
		/// エラーメッセージ
		/// 成功の場合はEmpty
		/// </returns>
		public static string ShipmentScoreDef(
			string orderId,
			string nissenTransactionId,
			string shippingCheckNo,
			string deliveryServiceCode,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor = null)
		{
			var service = new OrderService();
			var order = service.Get(orderId, sqlAccessor);

			var request = new ScoreDeliveryRegisterRequest
			{
				Transaction =
				{
					NissenTransactionId = nissenTransactionId,
					ShopTransactionId = order.PaymentOrderId,
					BilledAmount = order.LastBilledAmount.ToPriceString()
				},
				PdRequest =
				{
					Pdcompanycode = deliveryServiceCode,
					Slipno = shippingCheckNo,
					Address1 = order.Shippings[0].ShippingAddr1,
					Address2 = order.Shippings[0].ShippingAddr2,
					Address3 = order.Shippings[0].ShippingAddr3 + order.Shippings[0].ShippingAddr4
				}
			};

			var facade = new ScoreApiFacade();
			var result = facade.DeliveryRegister(request);
			if (result.Result != ScoreResult.Ok.ToText())
			{
				return string.Join("\r\n", result.Errors.ErrorList.Select(err => $"{err.ErrorCode}:{err.ErrorMessage}").ToArray());
			}

			service.UpdateExternalPaymentStatusShipmentComplete(orderId, Constants.FLG_LASTCHANGED_BATCH, updateHistoryAction, sqlAccessor);
			return string.Empty;
		}
		#endregion

		#region +ShippingEntryTriLinkAfterPay 後付款(TriLink後払い)出荷報告処理
		/// <summary>
		/// 後付款(TriLink後払い)出荷報告処理
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="shippingCheckNo">配送伝票番号</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">Sql accessor</param>
		/// <returns>エラーメッセージ</returns>
		public static string ShippingEntryTriLinkAfterPay(
			string orderId,
			string cardTranId,
			string shippingCheckNo,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor = null)
		{
			var response = TriLinkAfterPayApiFacade.CompleteShipment(
				new TriLinkAfterPayShipmentCompleteRequest(
					Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_DELIVERY_COMPANY_CODE,
					StringUtility.ToEmpty(shippingCheckNo),
					StringUtility.ToEmpty(cardTranId)));
			var errorMessage = TriLinkAfterPayApiFacade.CheckResponseErrorMessage(response);
			if (string.IsNullOrEmpty(errorMessage))
			{
				// 外部決済ステータスを更新
				var sv = new OrderService();
				if (sqlAccessor == null)
				{
					sv.UpdateExternalPaymentStatusShipmentComplete(orderId, Constants.FLG_LASTCHANGED_BATCH, updateHistoryAction);
				}
				else
				{
					sv.UpdateExternalPaymentStatusShipmentComplete(orderId, Constants.FLG_LASTCHANGED_BATCH, updateHistoryAction, sqlAccessor);
				}
			}
			return errorMessage;
		}
		#endregion

		#region +ShipmentNPAfterPay
		/// <summary>
		/// Shipment NP After Pay
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="cardTranId">Card Tran Id</param>
		/// <param name="shippingCheckNo">Shipping Check No</param>
		/// <param name="deliveryCompanyType">Delivery Company Type</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="sqlAccessor">Sql accessor</param>
		/// <returns>Error Message</returns>
		public static string ShipmentNPAfterPay(
			string orderId,
			string cardTranId,
			string shippingCheckNo,
			string deliveryCompanyType,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor = null)
		{
			// Check Bill Issued Date
			var billIssuedDate = string.Empty;
			var orderService = new OrderService();
			var order = orderService.Get(orderId, sqlAccessor);
			var errorMessage = NPAfterPayUtility.CheckBillIssuedDate(order, out billIssuedDate);
			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			// Execute Shipment
			var requestData = NPAfterPayUtility.CreateShipmentRequestData(
				cardTranId,
				shippingCheckNo,
				deliveryCompanyType,
				billIssuedDate);
			var result = NPAfterPayApiFacade.ShipmentOrder(requestData);

			if (result.IsSuccess == false)
			{
				errorMessage = result.GetApiErrorMessage();
				return errorMessage;
			}

			if (sqlAccessor == null)
			{
				// Update External Payment Status Shipment Complete
				orderService.UpdateExternalPaymentStatusShipmentComplete(
					orderId,
					Constants.FLG_LASTCHANGED_BATCH,
					updateHistoryAction);
			}
			else
			{
				// Update External Payment Status Shipment Complete
				orderService.UpdateExternalPaymentStatusShipmentComplete(
					orderId,
					Constants.FLG_LASTCHANGED_BATCH,
					updateHistoryAction,
					sqlAccessor);
			}
			return string.Empty;
		}
		#endregion

		#region +ShipmentGmoAtokara
		/// <summary>
		/// GMOアトカラ：出荷報告
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="cardTranId">Card Tran Id</param>
		/// <param name="shippingCheckNo">Shipping Check No</param>
		/// <param name="deliveryCompanyType">Delivery Company Type</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="sqlAccessor">Sql accessor</param>
		/// <returns>Error Message</returns>
		public static string ShipmentGmoAtokara(
			string orderId,
			string cardTranId,
			string shippingCheckNo,
			string deliveryCompanyType,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor = null)
		{
			var orderService = new OrderService();
			var order = orderService.Get(orderId, sqlAccessor);

			var shipmentApi = new PaymentGmoAtokaraShipmentApi();

			if (shipmentApi.Exec(order.CardTranId, deliveryCompanyType, shippingCheckNo, DateTime.Now.ToString("yyyy/MM/dd")) == false)
			{
				var errorMessage = shipmentApi.GetErrorMessage();
				return errorMessage;
			}

			orderService.UpdateExternalPaymentStatusShipmentComplete(
				orderId,
				Constants.FLG_LASTCHANGED_BATCH,
				updateHistoryAction);

			return string.Empty;
		}
		#endregion

		#region +ShipmentVeritransDef ベリトランス後払い出荷報告
		/// <summary>
		/// ベリトランス後払い出荷報告
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="slipNo">配送伝票番号</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="sqlAccessor">Sql Accessor</param>
		/// <returns>エラーメッセージ</returns>
		public static string ShipmentVeritransDef(
			string orderId,
			string slipNo,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor = null)
		{
			var service = new OrderService();
			var order = service.Get(orderId, sqlAccessor);

			var result = new PaymentVeritransCvsDef().DeliveryRegister(order, slipNo);
			if (result.Mstatus != VeriTransConst.RESULT_STATUS_OK)
			{
				return result.MerrMsg;
			}

			// エラーなければ外部決済ステータス更新
			service.UpdateExternalPaymentStatusShipmentComplete(orderId, Constants.FLG_LASTCHANGED_BATCH, updateHistoryAction, sqlAccessor);
			return string.Empty;
		}
		#endregion

		/// <summary>
		/// 配送方法：メール便が利用可能か判定
		/// </summary>
		/// <param name="cartProducts">カート商品</param>
		/// <returns>判定結果 TRUE:メール便利用可能、FALSE:メール便利用不可</returns>
		public static bool IsAvailableShippingKbnMail(List<CartProduct> cartProducts)
		{
			var canShippingMail = (GetShippingMethod(cartProducts) == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL);
			return canShippingMail;
		}

		/// <summary>
		/// 配送方法：メール便が利用可能か判定
		/// </summary>
		/// <param name="orderItems">注文商品</param>
		/// <returns>判定結果 TRUE：メール便利用可能、FALSE：メール便利用不可</returns>
		public static bool IsAvailableShippingKbnMail(IEnumerable<OrderItemModel> orderItems)
		{
			var canShippingMail = (GetShippingMethod(orderItems) == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL);
			return canShippingMail;
		}

		/// <summary>
		/// 配送方法：メール便が利用可能か判定
		/// </summary>
		/// <param name="fixedPurchaseItems">定期購入商品</param>
		/// <returns>判定結果 TRUE:メール便利用可能、FALSE:メール便利用不可</returns>
		public static bool IsAvailableShippingKbnMail(FixedPurchaseItemModel[] fixedPurchaseItems)
		{
			var canShippingMail = (GetShippingMethod(fixedPurchaseItems) == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL);
			return canShippingMail;
		}

		/// <summary>
		/// 管理メモ追記
		/// </summary>
		/// <param name="beforeMemo">修正前メモ</param>
		/// <param name="orderIdList">管理メモ記載対象の注文IDリスト</param>
		/// <param name="byCartCheckResult">カート間チェックの結果</param>
		/// <param name="beforeExtendStatus">変更前の注文拡張ステータス</param>
		/// <param name="afterExtendStatus">変更後の注文拡張ステータス</param>
		/// <returns>追記後のメモ</returns>
		public static string GetNotFirstTimeFixedPurchaseManagementMemo(
			string beforeMemo,
			string[] orderIdList,
			bool byCartCheckResult,
			string beforeExtendStatus = "",
			string afterExtendStatus = "")
		{
			// 商品購入制限利用可否がOFFの場合は、修正前メモを返す
			if (Constants.PRODUCT_ORDER_LIMIT_ENABLED == false) return beforeMemo;

			var newManagementMemos = new List<string>();
			if (string.IsNullOrEmpty(beforeMemo) == false) newManagementMemos.Add(beforeMemo);

			if (beforeExtendStatus != afterExtendStatus)
			{
				newManagementMemos.Add(string.Format(
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_UPDATE_EXTENDSTATUS39),
					DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
					ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTEND_STATUS39, beforeExtendStatus),
					ValueText.GetValueText(Constants.TABLE_ORDER, Constants.FIELD_ORDER_EXTEND_STATUS39, afterExtendStatus)));
			}

			// 購入履歴が存在する場合
			if (orderIdList.Length > 0)
			{
				newManagementMemos.Add(string.Format(
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_NOT_FIRSTTIME_CHECK_BY_ORDER),
					DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
					string.Join(", ", orderIdList)));
			}
			// カート間で重複する場合
			else if (byCartCheckResult)
			{
				newManagementMemos.Add(string.Format(
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_NOT_FIRSTTIME_CHECK_BY_CART),
					DateTime.Now.ToString("yyyy/MM/dd HH:mm")));
			}
			var result = string.Join(Environment.NewLine, newManagementMemos);

			return result;
		}

		/// <summary>
		/// 再与信可能なサイト区分？
		/// </summary>
		/// <param name="siteKbn">サイト区分</param>
		/// <returns>再与信可能？</returns>
		public static bool IsPermitReauthOrderSiteKbn(string siteKbn)
		{
			var isPermit = Constants.PAYMENT_REAUTH_ORDER_SITE_KBN.Contains(siteKbn);
			return isPermit;
		}

		/// <summary>
		/// 再与信可能な注文決済区分？
		/// </summary>
		/// <param name="paymentKbn">注文決済区分</param>
		/// <returns>再与信可能？</returns>
		public static bool CanPaymentReauth(string paymentKbn)
		{
			var canPaymentReauth = (((paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN))
				|| ((paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN))
				|| (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY));
			return canPaymentReauth;
		}

		/// <summary>
		/// 定期購入初回配送日取得(配送希望日がない場合、現在日時をもとに計算)
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="fixedPurchaseDaysRequired">配送所要日数</param>
		/// <param name="shippingDate">配送希望日</param>
		/// <param name="shippingMethod">配送方法</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="shippingCountryIsoCode">選択国ISOコード</param>
		/// <param name="prefecture">都道府県</param>
		/// <param name="zip">郵便番号</param>
		/// <returns>初回配送日</returns>
		public static DateTime GetFirstShippingDateBasedOnToday(
			string shopId,
			int fixedPurchaseDaysRequired,
			DateTime? shippingDate,
			string shippingMethod,
			string deliveryCompanyId,
			string shippingCountryIsoCode,
			string prefecture,
			string zip)
		{
			var model = new OrderShippingModel
			{
				ShippingDate = shippingDate,
				ShippingMethod = shippingMethod,
				DeliveryCompanyId = deliveryCompanyId,
				ShippingCountryIsoCode = shippingCountryIsoCode,
				ShippingAddr1 = GlobalAddressUtil.IsCountryJp(shippingCountryIsoCode)
					? prefecture
					: string.Empty,
				ShippingAddr2 = GlobalAddressUtil.IsCountryTw(shippingCountryIsoCode)
					? prefecture
					: string.Empty,
				ShippingZip = zip
			};
			var firstShippingDate = GetFirstShippingDate(shopId, fixedPurchaseDaysRequired, model);
			return firstShippingDate;
		}
		/// <summary>
		/// 定期購入初回配送日取得(配送希望日がない場合、注文完了日をもとに計算)
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="fixedPurchaseDaysRequired">配送所要日数</param>
		/// <param name="shippingDate">配送希望日</param>
		/// <param name="shippingMethod">配送方法</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="shippingCountryIsoCode">選択国ISOコード</param>
		/// <param name="prefecture">都道府県</param>
		/// <param name="zip">郵便番号</param>
		/// <param name="firstOrderDate">初回注文完了日</param>
		/// <returns>初回配送日</returns>
		public static DateTime GetFirstShippingDateBasedOnFisrtOrderDate(
			string shopId,
			int fixedPurchaseDaysRequired,
			DateTime? shippingDate,
			string shippingMethod,
			string deliveryCompanyId,
			string shippingCountryIsoCode,
			string prefecture,
			string zip,
			DateTime firstOrderDate)
		{
			var model = new OrderShippingModel
			{
				ShippingDate = shippingDate,
				ShippingMethod = shippingMethod,
				DeliveryCompanyId = deliveryCompanyId,
				ShippingCountryIsoCode = shippingCountryIsoCode,
				ShippingAddr1 = GlobalAddressUtil.IsCountryJp(shippingCountryIsoCode)
					? prefecture
					: string.Empty,
				ShippingAddr2 = GlobalAddressUtil.IsCountryTw(shippingCountryIsoCode)
					? prefecture
					: string.Empty,
				ShippingZip = zip
			};
			var firstShippingDate = GetFirstShippingDate(shopId, fixedPurchaseDaysRequired, model, firstOrderDate);
			return firstShippingDate;
		}

		/// <summary>
		/// 定期購入初回配送日取得
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="fixedPurchaseDaysRequired">配送所要日数</param>
		/// <param name="model">注文配送先モデル</param>
		/// <param name="firstOrderDate">初回注文完了日(nullの場合は現在日時をもとに計算される)</param>
		/// <returns>初回配送日</returns>
		private static DateTime GetFirstShippingDate(
			string shopId,
			int fixedPurchaseDaysRequired,
			OrderShippingModel model,
			DateTime? firstOrderDate = null)
		{
			if (model.ShippingDate.HasValue) return model.ShippingDate.Value;

			// 出荷予定日オプションがオフまたは出荷予定日が取得できない場合は出荷予定日を用いずに定期購入初回配送日を計算する
			var tmpScheduledShippingDate = (firstOrderDate.HasValue)
				? CalculateScheduledShippingDateBasedOnFirstOrderDate(shopId, model, firstOrderDate)
				: CalculateScheduledShippingDateBasedOnToday(shopId, model);
			if ((Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE == false)
				|| (tmpScheduledShippingDate.HasValue == false))
			{
				var dt = CalculateFirstShippingDateWithoutScheduledShippingDate(
					fixedPurchaseDaysRequired,
					firstOrderDate);
				return dt;
			}

			var totalLeadTime = HolidayUtil.GetTotalLeadTime(
				shopId,
				model.DeliveryCompanyId,
				(Constants.TW_COUNTRY_SHIPPING_ENABLE
						&& GlobalAddressUtil.IsCountryTw(model.ShippingCountryIsoCode))
					? model.ShippingAddr2
					: model.ShippingAddr1,
				model.ShippingZip.Replace("-", ""));

			var firstShippingDate = tmpScheduledShippingDate.Value.AddDays(totalLeadTime);
			firstShippingDate = (firstShippingDate < DateTime.Today)
				? DateTime.Parse(Constants.CONST_DEFAULT_DATETIME_VALUE)
				: firstShippingDate;
			return firstShippingDate;
		}

		/// <summary>
		/// 最短出荷日を取得(本日をもとに計算)
		/// </summary>
		/// <param name="shopId">The shop ID</param>
		/// <param name="model">The order shipping model</param>
		/// <returns>A scheduled shipping date</returns>
		public static DateTime? CalculateScheduledShippingDateBasedOnToday(
			string shopId,
			OrderShippingModel model)
		{
			var scheduledShippingDate = CalculateScheduledShippingDate(shopId, model);
			return scheduledShippingDate;
		}
		/// <summary>
		/// 最短出荷日を取得(本日をもとに計算)
		/// </summary>
		/// <param name="shopId">The shop ID</param>
		/// <param name="shippingDate">A shipping date</param>
		/// <param name="shippingMethod">A shipping method</param>
		/// <param name="deliveryCompanyId">A delivery company ID</param>
		/// <param name="countryIsoCode">国ISOコード</param>
		/// <param name="prefecture">The prefecture</param>
		/// <param name="zip">The zip code</param>
		/// <param name="isUseShortestShippingDate">最短出荷日を利用する(True:利用)</param>
		/// <returns>A scheduled shipping date</returns>
		public static DateTime? CalculateScheduledShippingDateBasedOnToday(
			string shopId,
			DateTime? shippingDate,
			string shippingMethod,
			string deliveryCompanyId,
			string countryIsoCode,
			string prefecture,
			string zip,
			bool isUseShortestShippingDate = true)
		{
			var scheduledshippingDate = CalculateScheduledShippingDate(
				shopId,
				shippingDate,
				shippingMethod,
				deliveryCompanyId,
				countryIsoCode,
				prefecture,
				zip,
				null,
				isUseShortestShippingDate);
			return scheduledshippingDate;
		}

		/// <summary>
		/// 出荷予定日を取得(初回注文完了日をもとに計算)
		/// </summary>
		/// <param name="shopId">The shop ID</param>
		/// <param name="model">The order shipping model</param>
		/// <param name="firstOrderDate">初回注文完了日</param>
		/// <returns>A scheduled shipping date</returns>
		public static DateTime? CalculateScheduledShippingDateBasedOnFirstOrderDate(
			string shopId,
			OrderShippingModel model,
			DateTime? firstOrderDate)
		{
			var scheduledShippingDate = CalculateScheduledShippingDate(shopId, model, firstOrderDate);
			return scheduledShippingDate;
		}
		/// <summary>
		/// 出荷予定日を取得(初回注文完了日をもとに計算)
		/// </summary>
		/// <param name="shopId">The shop ID</param>
		/// <param name="shippingDate">A shipping date</param>
		/// <param name="shippingMethod">A shipping method</param>
		/// <param name="deliveryCompanyId">A delivery company ID</param>
		/// <param name="countryIsoCode">国ISOコード</param>
		/// <param name="prefecture">The prefecture</param>
		/// <param name="zip">The zip code</param>
		/// <param name="firstOrderDate">初回注文完了日</param>
		/// <returns>A scheduled shipping date</returns>
		public static DateTime? CalculateScheduledShippingDateBasedOnFirstOrderDate(
			string shopId,
			DateTime? shippingDate,
			string shippingMethod,
			string deliveryCompanyId,
			string countryIsoCode,
			string prefecture,
			string zip,
			DateTime? firstOrderDate)
		{
			var scheduledshippingDate = CalculateScheduledShippingDate(
				shopId,
				shippingDate,
				shippingMethod,
				deliveryCompanyId,
				countryIsoCode,
				prefecture,
				zip,
				firstOrderDate);
			return scheduledshippingDate;
		}

		/// <summary>
		/// Calculate scheduled shipping date
		/// </summary>
		/// <param name="shopId">The shop ID</param>
		/// <param name="model">The order shipping model</param>
		/// <param name="firstOrderDate">初回注文完了日</param>
		/// <returns>A scheduled shipping date</returns>
		private static DateTime? CalculateScheduledShippingDate(
			string shopId,
			OrderShippingModel model,
			DateTime? firstOrderDate = null)
		{
			if (model == null) return null;

			DateTime? scheduledShippingDate;
			var isTaiwanCountryShippingEnable = (Constants.TW_COUNTRY_SHIPPING_ENABLE
				&& GlobalAddressUtil.IsCountryTw(model.ShippingCountryIsoCode));
			if (firstOrderDate.HasValue)
			{
				scheduledShippingDate = CalculateScheduledShippingDateBasedOnFirstOrderDate(
					shopId,
					model.ShippingDate,
					model.ShippingMethod,
					model.DeliveryCompanyId,
					model.ShippingCountryIsoCode,
					isTaiwanCountryShippingEnable
						? model.ShippingAddr2
						: model.ShippingAddr1,
					model.ShippingZip.Replace("-", string.Empty),
					firstOrderDate);
			}
			else
			{
				scheduledShippingDate = CalculateScheduledShippingDateBasedOnToday(
					shopId,
					model.ShippingDate,
					model.ShippingMethod,
					model.DeliveryCompanyId,
					model.ShippingCountryIsoCode,
					isTaiwanCountryShippingEnable
						? model.ShippingAddr2
						: model.ShippingAddr1,
					model.ShippingZip.Replace("-", string.Empty));
			}
			return scheduledShippingDate;
		}
		/// <summary>
		/// Calculate scheduled shipping date
		/// </summary>
		/// <param name="shopId">The shop ID</param>
		/// <param name="shippingDate">A shipping date</param>
		/// <param name="shippingMethod">A shipping method</param>
		/// <param name="deliveryCompanyId">A delivery company ID</param>
		/// <param name="countryIsoCode">国ISOコード</param>
		/// <param name="prefecture">The prefecture</param>
		/// <param name="zip">The zip code</param>
		/// <param name="firstOrderDate">初回注文完了日</param>
		/// <param name="isUseShortestShippingDate">最短出荷日を利用する(True:利用)</param>
		/// <returns>A scheduled shipping date</returns>
		private static DateTime? CalculateScheduledShippingDate(
			string shopId,
			DateTime? shippingDate,
			string shippingMethod,
			string deliveryCompanyId,
			string countryIsoCode,
			string prefecture,
			string zip,
			DateTime? firstOrderDate = null,
			bool isUseShortestShippingDate = true)
		{
			if ((GlobalConfigUtil.UseLeadTime() == false)
				|| ((GlobalAddressUtil.IsCountryJp(countryIsoCode) == false)
					&& (GlobalAddressUtil.IsCountryTw(countryIsoCode) == false))
				|| IsLeadTimeFlgOff(deliveryCompanyId))
			{
				return null;
			}

			if (shippingDate.HasValue == false)
			{
				var shortestShippingDate = (firstOrderDate.HasValue)
					? HolidayUtil.GetShortestShippingDateBasedOnFirstOrderDate(firstOrderDate, deliveryCompanyId)
					: HolidayUtil.GetShortestShippingDateBasedOnToday(deliveryCompanyId);
				return shortestShippingDate;
			}

			var totalLeadTime = HolidayUtil.GetTotalLeadTime(
				shopId,
				deliveryCompanyId,
				prefecture,
				zip);
			var scheduledshippingDate = HolidayUtil.CalculatorScheduledShippingDate(shippingDate.Value, totalLeadTime, deliveryCompanyId, isUseShortestShippingDate);
			return scheduledshippingDate;
		}

		/// <summary>
		/// 出荷予定日を使用せず定期購入初回配送日を算出する
		/// </summary>
		/// <param name="fixedPurchaseDaysRequired">配送所要日数</param>
		/// <param name="firstOrderDate">初回注文完了日</param>
		/// <returns>初回配送日</returns>
		private static DateTime CalculateFirstShippingDateWithoutScheduledShippingDate(
			int fixedPurchaseDaysRequired,
			DateTime? firstOrderDate)
		{
			var firstShippingDate = (firstOrderDate.HasValue)
				? ((DateTime)firstOrderDate).AddDays(fixedPurchaseDaysRequired)
				: new FixedPurchaseService().CalculateFirstShippingDate(null, fixedPurchaseDaysRequired);
			firstShippingDate = (firstShippingDate < DateTime.Today)
				? DateTime.Parse(Constants.CONST_DEFAULT_DATETIME_VALUE)
				: firstShippingDate;
			return firstShippingDate;
		}

		/// <summary>
		/// リードタイムフラグがOFFか
		/// </summary>
		/// <param name="deliveryCompanyId">会社ID</param>
		/// <returns>offならばtrue</returns>
		public static bool IsLeadTimeFlgOff(string deliveryCompanyId)
		{
			var deliveryCompany = DomainFacade.Instance.DeliveryCompanyService.Get(deliveryCompanyId);

			return ((deliveryCompany == null)
				|| (deliveryCompany.DeliveryLeadTimeSetFlg == Constants.FLG_DELIVERYCOMPANY_DELIVERY_LEAD_TIME_SET_FLG_INVALID));
		}

		/// <summary>
		/// 総リードタイムを取得
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="deliveryCompanyId">配送サービスID</param>
		/// <param name="prefecture">県</param>
		/// <param name="zip">郵便番号</param>
		/// <returns>A scheduled shipping date</returns>
		public static int GetTotalLeadTime(string shopId, string deliveryCompanyId, string prefecture, string zip)
		{
			if (IsLeadTimeFlgOff(deliveryCompanyId)) return 0;
			var totalLeadTime = HolidayUtil.GetTotalLeadTime(shopId, deliveryCompanyId, prefecture, zip);
			return totalLeadTime;
		}

		/// <summary>
		/// 再計算（カート不使用）
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <returns>エラーメッセージ</returns>
		public static string SetCalculateTax(OrderModel order)
		{
			var errormessage = new StringBuilder();
			// 商品金額合計の初期化
			InitializeSubtotal(order);

			// セットプロモーション計算
			errormessage.Append(CalculateSetPromotion(order));

			// 定期購入割引額計算
			errormessage.Append(CalculateFixedPurchaseDiscountPrice(order));

			// 会員ランク割引額計算
			errormessage.Append(CalculateMemberRankDiscountPrice(order));

			// 定期会員割引額計算
			errormessage.Append(CalculateFixedPurchaseMemberDiscount(order));

			// クーポン系計算
			errormessage.Append(CalculateCouponFamily(order));

			// 使用ポイント割引額計算
			errormessage.Append(CalculatePointDisCountProductPrice(order));

			// 調整金額計算
			CalculatePriceRegulation(order);

			// 税額計算
			CalculateTaxPrice(order);

			return errormessage.ToString();
		}

		#region InitializeSubtotal 商品金額合計再計算
		/// <summary>
		/// 商品金額合計再計算
		/// </summary>
		/// <param name="order">注文入力情報</param>
		public static void InitializeSubtotal(OrderModel order)
		{
			var subscriptionBoxFixedAmountList = new Dictionary<string, decimal>();

			foreach (var shipping in order.Shippings)
			{
				foreach (var item in shipping.Items)
				{
					// 商品小計(按分適用後)の初期化
					item.PriceSubtotalAfterDistribution = TaxCalculationUtility.CheckShippingPlace(shipping.ShippingCountryIsoCode, shipping.ShippingAddr5)
						? PriceCalculator.GetItemPrice(item.ProductPricePretax, item.ItemQuantity)
						: PriceCalculator.GetItemPrice(item.ProductPrice, item.ItemQuantity);
					item.ItemPriceRegulation = 0m;

					// 頒布会定額商品の按分適用後金額の初期化
					if (item.IsSubscriptionBoxFixedAmount
						&& (subscriptionBoxFixedAmountList.ContainsKey(item.SubscriptionBoxCourseId) == false))
					{
						subscriptionBoxFixedAmountList.Add(item.SubscriptionBoxCourseId, (decimal)item.SubscriptionBoxFixedAmount);
					}
				}
			}

			order.ShippingPriceDiscountAmount = 0m;
			order.PaymentPriceDiscountAmount = 0m;
			order.SubscriptionBoxFixedAmountList = subscriptionBoxFixedAmountList;
		}
		#endregion

		#region CalculateSetPromotion セットプロモーション割引
		/// <summary>
		/// セットプロモーション割引を注文情報の商品小計に按分
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <returns>エラーメッセージ</returns>
		private static string CalculateSetPromotion(OrderModel order)
		{
			var errorMessage = new StringBuilder();
			// 商品金額割引の場合は商品按分価格を設定
			foreach (var setPromotion in order.SetPromotions.Where(sp => (sp.ProductDiscountFlg == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON)))
			{
				var stackedDiscountAmount = 0m;
				// セットプロモーション対象商品に割引を按分
				var targetItems = order.Shippings
					.SelectMany(shipping => shipping.Items)
					.Where(item => (item.OrderSetpromotionNo == setPromotion.OrderSetpromotionNo))
					.ToArray();

				if (targetItems.Length == 0) continue;

				foreach (var item in targetItems)
				{
					var itemPrice = PriceCalculator.GetItemPrice(item.ProductPricePretax, item.ItemQuantity);
					var discountPrice = PriceCalculator.GetDistributedPrice(
						setPromotion.ProductDiscountAmount,
						itemPrice,
						setPromotion.UndiscountedProductSubtotal).ToPriceDecimal().Value;

					stackedDiscountAmount += discountPrice;
					item.PriceSubtotalAfterDistribution -= discountPrice;
				}

				var fractionDiscountPrice = setPromotion.ProductDiscountAmount - stackedDiscountAmount;
				var weightItem = targetItems
					.OrderByDescending(item => item.PriceSubtotalAfterDistribution).First();
				weightItem.PriceSubtotalAfterDistribution -= fractionDiscountPrice;

				if (setPromotion.UndiscountedProductSubtotal < setPromotion.ProductDiscountAmount)
				{
					errorMessage.Append(GetDiscountLimitMessage(
						setPromotion.OrderSetpromotionNo + "：" + setPromotion.SetpromotionName));
				}
			}

			// 配送料、決済手数料の割引額を取得
			order.PaymentPriceDiscountAmount = order.SetPromotions.Any(setPromotion => setPromotion.IsDiscountTypePaymentChargeFree)
				? order.OrderPriceExchange
				: 0m;
			order.ShippingPriceDiscountAmount = order.SetPromotions.Any(setPromotion => setPromotion.IsDiscountTypeShippingChargeFree)
				? order.OrderPriceShipping
				: 0m;

			return errorMessage.ToString();
		}
		#endregion

		#region CalculateFixedPurchaseDiscountPrice 定期購入割引額計算
		/// <summary>
		/// 定期購入割引額計算
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <returns>エラーメッセージ</returns>
		private static string CalculateFixedPurchaseDiscountPrice(OrderModel order)
		{
			var targetItems = order.Shippings
				.SelectMany(shipping => shipping.Items.Where(p => p.IsFixedPurchaseDiscountItem))
				.ToArray();
			var discountTotal = order.FixedPurchaseDiscountPrice;

			var calculatedPriceByDiscountSetting = 0m;
			foreach (var item in targetItems)
			{
				var discount = PriceCalculator.GetFixedPurchaseDiscountPrice(
					item.FixedPurchaseDiscountType,
					item.FixedPurchaseDiscountValue,
					item.PriceSubtotalAfterDistribution,
					item.ItemQuantity);

				item.PriceSubtotalAfterDistribution -= discount;

				calculatedPriceByDiscountSetting += discount;
			}

			var differenceFromEnteredPrice = discountTotal - calculatedPriceByDiscountSetting;

			var message = ProrateDiscountPrice(
				order,
				targetItems,
				differenceFromEnteredPrice,
				"定期購入割引");

			return message;
		}
		#endregion

		#region CalculateMemberRankDiscountPrice 会員ランク割引額計算
		/// <summary>
		/// 会員ランク割引額計算
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <returns>エラーメッセージ</returns>
		private static string CalculateMemberRankDiscountPrice(OrderModel order)
		{
			if (Constants.MEMBER_RANK_OPTION_ENABLED == false) return "";

			var discount = order.MemberRankDiscountPrice;

			//------------------------------------------------------
			// 会員ランク割引対象金額取得
			//------------------------------------------------------
			var targetItems = order.Shippings
				.SelectMany(shipping => shipping.Items.Where(item => (Constants.MEMBER_RANK_OPTION_ENABLED
					&& MemberRankOptionUtilityWrapper.Instance.IsDiscountTarget(item.ShopId, item.ProductId))))
				.ToArray();

			var message = ProrateDiscountPrice(order, targetItems, discount, "会員ランク割引");

			return message;
		}
		#endregion

		#region CalculateFixedPurchaseDiscount 定期会員割引を商品小計に按分
		/// <summary>
		/// 定期会員割引を注文情報の商品小計に按分
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>通常商品にのみ按分・定期商品には按分しない</remarks>
		private static string CalculateFixedPurchaseMemberDiscount(OrderModel order)
		{
			var discount = order.FixedPurchaseMemberDiscountAmount;

			var targetItems = order.Shippings
				.SelectMany(shipping => shipping.Items
					.Where(product => (product.OrderSetpromotionNo == null)
						&& (product.IsFixedPurchaseItem == false)
						&& (string.IsNullOrEmpty(product.NoveltyId))))
				.ToArray();

			var message = ProrateDiscountPrice(order, targetItems, discount, "定期会員割引");

			return message;
		}
		#endregion

		#region CalculateCouponFamily クーポン系計算
		/// <summary>
		/// クーポン系計算
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <returns>エラーメッセージ</returns>
		private static string CalculateCouponFamily(OrderModel order)
		{
			// クーポン情報が設定されている場合
			if ((Constants.W2MP_COUPON_OPTION_ENABLED == false)
				|| (order.Coupons.FirstOrDefault() == null)
				|| string.IsNullOrEmpty(order.Coupons.First().CouponId)) return "";

			var orderCoupon = order.Coupons.First();
			var discount = order.OrderCouponUse;

			var coupon = DomainFacade.Instance.CouponService.GetCoupon(orderCoupon.DeptId, orderCoupon.CouponId);

			var targetItems = order.Shippings.SelectMany(shipping => shipping.Items)
				.Where(item =>
				{
					var product = DomainFacade.Instance.ProductService.GetProductVariation(
						item.ShopId,
						item.ProductId,
						item.VariationId,
						order.MemberRankId);
					return CouponOptionUtilityWrapper.Instance.IsCouponApplyProduct(coupon, product);
				}).ToArray();

			order.ShippingPriceDiscountAmount -= (order.OrderCouponUse - discount);

			var message = ProrateDiscountPrice(order, targetItems, discount, "クーポン割引");

			return message;
		}
		#endregion

		#region CalculatePointDisCountProductPrice 商品ごとのポイント割引後の金額を計算
		/// <summary>
		/// 商品ごとのポイント割引後の金額を計算
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <returns>エラーメッセージ</returns>
		private static string CalculatePointDisCountProductPrice(OrderModel order)
		{
			if (Constants.W2MP_POINT_OPTION_ENABLED == false) return "";

			var message = ProrateDiscountPrice(order,
				order.Shippings.SelectMany(shipping => shipping.Items).ToArray(),
				order.OrderPointUse,
				"ポイント割引");

			return message;
		}
		#endregion

		#region 調整金額を商品・配送料・決済手数料で按分計算
		/// <summary>
		/// 調整金額を商品・配送料・決済手数料で按分計算
		/// </summary>
		/// <param name="order">注文入力情報</param>
		private static void CalculatePriceRegulation(OrderModel order)
		{
			var priceRegulation = order.OrderPriceRegulation;
			if (priceRegulation == 0) return;

			var itemsUsePriceSubtotal = order.Shippings
				.SelectMany(shipping => shipping.Items.Where(item => item.IsSubscriptionBoxFixedAmount == false))
				.ToArray();

			var priceTotal = itemsUsePriceSubtotal.Sum(item => item.PriceSubtotalAfterDistribution);
			var priceTotalSubscriptionBoxFixedAmount = order.SubscriptionBoxFixedAmountList.Sum(item => item.Value);
			priceTotal += priceTotalSubscriptionBoxFixedAmount;

			// 調整金額適用対象金額取得
			var paymentPrice = order.OrderPriceExchange - order.PaymentPriceDiscountAmount;
			var paymentRegulationPrice = 0m;
			var shippingPrice = order.OrderPriceShipping - order.ShippingPriceDiscountAmount;
			var shippingRegulationPrice = 0m;
			priceTotal += paymentPrice;
			priceTotal += shippingPrice;

			var stackedRegulationPrice = 0m;
			if (priceTotal != 0)
			{
				// 商品調整金額を設定
				foreach (var item in itemsUsePriceSubtotal)
				{
					// 計算方法：商品小計 - (商品小計 / 商品合計 * 調整金額)
					// ※端数切捨て
					var regulationPrice = PriceCalculator.GetDistributedPrice(
						priceRegulation,
						item.PriceSubtotalAfterDistribution,
						priceTotal).ToPriceDecimal().Value;
					item.ItemPriceRegulation = regulationPrice;
					stackedRegulationPrice += regulationPrice;
				}

				// 頒布会定額で調整金額を設定
				foreach (var courseId in order.SubscriptionBoxFixedAmountList.Keys.ToArray())
				{
					var regulationPrice = PriceCalculator.GetDistributedPrice(
						priceRegulation,
						order.SubscriptionBoxFixedAmountList[courseId],
						priceTotal);

					order.SubscriptionBoxFixedAmountList[courseId] += regulationPrice.ToPriceDecimal().Value;
					stackedRegulationPrice += regulationPrice;
				}

				shippingRegulationPrice = PriceCalculator.GetDistributedPrice(
					priceRegulation,
					shippingPrice,
					priceTotal).ToPriceDecimal().Value;
				stackedRegulationPrice += shippingRegulationPrice;

				paymentRegulationPrice = PriceCalculator.GetDistributedPrice
					(priceRegulation,
					paymentPrice,
					priceTotal).ToPriceDecimal().Value;
				stackedRegulationPrice += paymentRegulationPrice;
			}

			// 実際の調整金額と商品毎の調整金額の合計を合わせるために、端数分を重み付けする
			var weightItem = itemsUsePriceSubtotal
				.OrderByDescending(item => item.PriceSubtotalAfterDistribution)
				.FirstOrDefault();
			var weightBox = order.SubscriptionBoxFixedAmountList
				.OrderByDescending(item => item.Value)
				.FirstOrDefault();

			// 頒布会定額ではない商品の金額が一番高いかつ金額が0ではない
			if (IsAddFractionPriceToProduct(weightItem, weightBox) && (weightItem.PriceSubtotalAfterDistribution != 0))
			{
				weightItem.ItemPriceRegulation += (priceRegulation - stackedRegulationPrice);
			}
			// 頒布会定額の金額が一番高いかつ金額がではない
			else if (weightBox.Value != 0)
			{
				order.SubscriptionBoxFixedAmountList[weightBox.Key] += (priceRegulation - stackedRegulationPrice);
			}
			// 配送料が0ではない
			else if (shippingPrice != 0)
			{
				shippingRegulationPrice += priceRegulation - stackedRegulationPrice;
			}
			// 上記いずれにも該当しない
			else
			{
				paymentRegulationPrice += priceRegulation - stackedRegulationPrice;
			}
			// 調整金額は符号逆にする
			order.ShippingPriceDiscountAmount -= shippingRegulationPrice;
			order.PaymentPriceDiscountAmount -= paymentRegulationPrice;
		}
		#endregion

		#region 受注商品に割引金額を按分
		/// <summary>
		/// 受注商品に割引金額を按分
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="items">対象商品</param>
		/// <param name="discountTotal">割引き金額</param>
		/// <param name="discountName">割引き名称</param>
		/// <returns>エラーメッセージ</returns>
		private static string ProrateDiscountPrice(OrderModel order, OrderItemModel[] items, decimal discountTotal, string discountName)
		{
			if (discountTotal == 0) return "";

			var errorMessage = "";
			var stackedDiscountAmount = 0m;

			// 按分の対象商品
			var targetItems = items.Any()
				? items
				: order.Shippings
					.SelectMany(shipping => shipping.Items)
					.ToArray();
			// 頒布会定額を除く商品
			var targetItemsExcludedFixedAmount = targetItems
				.Where(item => item.IsSubscriptionBoxFixedAmount == false)
				.ToArray();
			// 対象商品のうち、定額である頒布会のID（IDの重複なし）
			var targetSubscriptionBoxIds = targetItems
				.Where(item => item.IsSubscriptionBoxFixedAmount)
				.Select(item => item.SubscriptionBoxCourseId)
				.Distinct()
				.ToArray();
			// 対象になる定額の頒布会IDと金額
			var targetSubscriptionBoxFixedAmountList = order.SubscriptionBoxFixedAmountList
				.Where(fixedAmount => targetSubscriptionBoxIds.Contains(fixedAmount.Key))
				.ToArray();

			// 頒布会定額以外の商品合計金額
			var targetPriceTotalExcludedFixedAmount = targetItemsExcludedFixedAmount.Sum(item => item.PriceSubtotalAfterDistribution);
			// 頒布会定額のみの商品合計金額
			var targetPriceTotalFixedAmountOnly = targetSubscriptionBoxFixedAmountList.Sum(list => list.Value);
			// 頒布会定額を含む対象商品の合計金額
			var targetPriceTotal = targetPriceTotalExcludedFixedAmount + targetPriceTotalFixedAmountOnly;

			// 頒布会定額以外の商品に設定
			foreach (var item in targetItemsExcludedFixedAmount)
			{
				var discountPrice = (targetPriceTotal != 0)
					? PriceCalculator.GetDistributedPrice(
						discountTotal,
						item.PriceSubtotalAfterDistribution,
						targetPriceTotal).ToPriceDecimal().Value
					: 0m;
				item.PriceSubtotalAfterDistribution -= discountPrice;
				stackedDiscountAmount += discountPrice;
			}

			// 頒布会定額に設定
			foreach (var courseId in targetSubscriptionBoxIds)
			{
				var discountPrice = (targetPriceTotal != 0)
					? PriceCalculator.GetDistributedPrice(
						discountTotal,
						order.SubscriptionBoxFixedAmountList[courseId],
						targetPriceTotal).ToPriceDecimal().Value
					: 0m;
				order.SubscriptionBoxFixedAmountList[courseId] -= discountPrice;
				stackedDiscountAmount += discountPrice;
			}

			var fractionDiscountPrice = discountTotal - stackedDiscountAmount;
			var weightItem = targetItems
				.OrderByDescending(item => item.PriceSubtotalAfterDistribution)
				.FirstOrDefault();
			var weightBox = order.SubscriptionBoxFixedAmountList.OrderByDescending(box => box.Value).FirstOrDefault();

			if (IsAddFractionPriceToProduct(weightItem, weightBox))
			{
				weightItem.PriceSubtotalAfterDistribution -= fractionDiscountPrice;
			}
			else
			{
				order.SubscriptionBoxFixedAmountList[weightBox.Key] -= fractionDiscountPrice;
			}

			if (targetPriceTotal < discountTotal)
			{
				errorMessage = GetDiscountLimitMessage(discountName);
			}

			return errorMessage;
		}
		#endregion

		#region 割引上限額超過メッセージを取得
		/// <summary>
		/// 割引上限額超過メッセージを取得
		/// </summary>
		/// <param name="discountName">割引き名称</param>
		/// <returns>割引上限額超過エラーメッセージ</returns>
		private static string GetDiscountLimitMessage(string discountName)
		{
			return string.Format(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_DISCOUNT_LIMIT_ERROR), discountName);
		}
		#endregion

		#region CalculateTaxPrice 税額を計算
		/// <summary>
		/// 税額を計算
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <param name="taxExcludedFractionRounding">税率丸め方法</param>
		/// <returns>税額報</returns>
		private static void CalculateTaxPrice(OrderModel order, string taxExcludedFractionRounding = "")
		{
			if (string.IsNullOrEmpty(taxExcludedFractionRounding))
			{
				taxExcludedFractionRounding = Constants.TAX_EXCLUDED_FRACTION_ROUNDING;
			}

			var priceByTaxRate = new List<Hashtable>();
			// 税率毎の購入金額を算出する
			priceByTaxRate.AddRange(order.Shippings
				.SelectMany(shipping => shipping.Items.Where(
						item => (item.IsSubscriptionBoxFixedAmount == false) && (string.IsNullOrEmpty(item.ProductId) == false)),
					(value, item) => new { CountryIsoCode = value.ShippingCountryIsoCode, addr5 = value.ShippingAddr5, item = item })
				.GroupBy(productInfo => new
				{
					TaxRate = productInfo.item.ProductTaxRate,
					IsTaxable = TaxCalculationUtility.CheckShippingPlace(productInfo.CountryIsoCode, productInfo.addr5)
				})
				.Select(groupedInfo => new Hashtable
				{
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , groupedInfo.Key.TaxRate },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE ,
						groupedInfo.Sum(productInfo => productInfo.item.PriceSubtotalAfterDistribution + productInfo.item.ItemPriceRegulation) },
					{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE , groupedInfo.Key.IsTaxable
						? groupedInfo.Sum(productInfo => productInfo.item.PriceSubtotalAfterDistribution + productInfo.item.ItemPriceRegulation)
						: 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, 0m },
				}).ToList());

			// 頒布会定額コース商品のみで計算
			if (order.HasSubscriptionBoxFixedAmountItem)
			{
				var itemsGroupByFixedAmountCourse = order.Shippings
					.SelectMany(shipping => shipping.Items
						.Where(item => item.IsSubscriptionBoxFixedAmount))
					.GroupBy(item => item.SubscriptionBoxCourseId);
				foreach (var courseGroup in itemsGroupByFixedAmountCourse)
				{
					var subscriptionBox = DataCacheControllerFacade.GetSubscriptionBoxCacheController().Get(courseGroup.Key);
					var taxRate = DomainFacade.Instance.ProductTaxCategoryService.Get(subscriptionBox.TaxCategoryId).TaxRate;
					var fixedAmount = order.SubscriptionBoxFixedAmountList[courseGroup.Key];

					priceByTaxRate.Add(
						new Hashtable
						{
							{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, taxRate },
							{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE, fixedAmount },
							{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE, fixedAmount },
							{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, 0m },
							{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE, 0m },
							{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, 0m },
						});
				}
			}

			priceByTaxRate.Add(new Hashtable
			{
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , order.ShippingTaxRate },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 0m },
				{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE,
					order.OrderPriceShipping - order.ShippingPriceDiscountAmount },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, 0m },
			});
			priceByTaxRate.Add(new Hashtable
			{
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , order.PaymentTaxRate },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 0m },
				{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE ,
					order.OrderPriceExchange - order.PaymentPriceDiscountAmount },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, 0m },
			});
			priceByTaxRate.AddRange(
				order.OrderPriceByTaxRates.Select(
					priceInfoByTaxRate => new Hashtable
					{
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, priceInfoByTaxRate.KeyTaxRate },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE, 0m },
						{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE , TaxCalculationUtility.CheckShippingPlace(order.Shippings[0].ShippingCountryIsoCode, order.Shippings[0].ShippingAddr5)
							? 0m
							: priceInfoByTaxRate.ReturnPriceCorrectionByRate
						},
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, 0m },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE, 0m },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, priceInfoByTaxRate.ReturnPriceCorrectionByRate },
					}));
			order.OrderPriceByTaxRates = priceByTaxRate.GroupBy(
				price => new
				{
					taxRate = price[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE]
				}).Select(
				item => new OrderPriceByTaxRateModel()
				{
					OrderId = order.OrderId,
					KeyTaxRate = (decimal)item.Key.taxRate,
					PriceSubtotalByRate = item.Sum(
						itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]),
					PriceShippingByRate = item.Sum(
						itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]),
					PricePaymentByRate =
						item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE]),
					ReturnPriceCorrectionByRate =
						item.Sum(itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE]),
					PriceTotalByRate = item.Sum(
						itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE]),
					TaxPriceByRate = TaxCalculationUtility.GetTaxPrice(
						item.Sum(
							itemKey => ((decimal)itemKey[TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE]
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE])),
						(decimal)item.Key.taxRate,
						taxExcludedFractionRounding,
						true),
					DateCreated = DateTime.Now,
					DateChanged = DateTime.Now
				}).ToArray();

			order.OrderPriceTax = order.OrderPriceByTaxRates.Sum(price => price.TaxPriceByRate);
			order.OrderPriceSubtotalTax = order.Shippings
				.Where(shipping => TaxCalculationUtility.CheckShippingPlace(
					shipping.ShippingCountryIsoCode,
					shipping.ShippingAddr5)).SelectMany(shipping => shipping.Items).Sum(item => item.ItemPriceTax);
		}
		#endregion

		/// <summary>
		/// 税率毎価格情報計算(返品交換用)
		/// </summary>
		/// <param name="returnAndExchangeItems">返品交換対象交換商品情報</param>
		/// <param name="priceCorrectionByTax">税率毎金額補正情報</param>
		/// <param name="pointPriceByTax">税率毎ポイント価格情報</param>
		/// <param name="allReturnFixedAmountCourseIds">全返品する頒布会定額コースID配列</param>
		/// <returns>税率毎価格情報</returns>
		public static List<OrderPriceByTaxRateModel> CalculateReturnPriceInfoByTaxRate(
			List<ReturnOrderItem> returnAndExchangeItems,
			List<OrderPriceByTaxRateModel> priceCorrectionByTax,
			List<OrderPriceByTaxRateModel> pointPriceByTax,
			string[] allReturnFixedAmountCourseIds)
		{
			// 頒布会定額コース商品を除く分
			var itemPriceIsDutyFree = returnAndExchangeItems.First().IsDutyFree;
			var groupedItem = returnAndExchangeItems
				.Where(item => item.IsSubscriptionBoxFixedAmount == false)
				.GroupBy(
					item => new
					{
						item.ProductTaxRate,
					});
			var priceByTaxRate = groupedItem
				.Select(
					item => new Hashtable
					{
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, item.Key.ProductTaxRate },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE,
							item.Sum(itemKey => itemKey.ItemPriceIncludedTax) },
						{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE,
							itemPriceIsDutyFree ? 0m : item.Sum(itemKey => itemKey.ItemPriceIncludedTax) },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, 0m },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE, 0m },
						{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, 0m },
					})
				.ToList();

			// 頒布会定額コース分
			foreach (var subscriptionBoxCourseId in allReturnFixedAmountCourseIds)
			{
				var subscriptionBox = DataCacheControllerFacade.GetSubscriptionBoxCacheController()
					.Get(subscriptionBoxCourseId);
				var taxRate = new ProductTaxCategoryService().Get(subscriptionBox.TaxCategoryId).TaxRate;
				var fixedAmountByTaxRate = new Hashtable
				{
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, taxRate },
					{
						Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE,
						returnAndExchangeItems.FirstOrDefault(
								item => item.SubscriptionBoxCourseId == subscriptionBoxCourseId)
							?.SubscriptionBoxFixedAmount.GetValueOrDefault() * -1
					},
					{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE, 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE, 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, 0m },
				};
				priceByTaxRate.Add(fixedAmountByTaxRate);
			}

			priceByTaxRate.AddRange(
				priceCorrectionByTax.Select(
					regulationPrice => new Hashtable
				{
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, regulationPrice.KeyTaxRate },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE, regulationPrice.PriceSubtotalByRate },
					{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE, itemPriceIsDutyFree ? 0m : regulationPrice.ReturnPriceCorrectionByRate },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE, 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, regulationPrice.ReturnPriceCorrectionByRate },
				}));
			priceByTaxRate.AddRange(
				pointPriceByTax.Select(
					pointPrice => new Hashtable
				{
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, pointPrice.KeyTaxRate },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE, pointPrice.PriceSubtotalByRate },
					{ TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE, itemPriceIsDutyFree ? 0m : pointPrice.PriceSubtotalByRate },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE, 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE, 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE, 0m },
				}));

			var returnExchangePriceByTaxRateModel = priceByTaxRate.GroupBy(
				price => new
				{
					taxRate = price[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE]
				}).Select(
				item => new OrderPriceByTaxRateModel()
				{
					KeyTaxRate = (decimal)item.Key.taxRate,
					PriceSubtotalByRate = item.Sum(
						itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]),
					PriceShippingByRate = item.Sum(
						itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]),
					PricePaymentByRate = item.Sum(
						itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE]),
					ReturnPriceCorrectionByRate = item.Sum(
						itemKey => (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE]),
					PriceTotalByRate = item.Sum(
						itemKey => ((decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE]
							+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE])),
					TaxPriceByRate = TaxCalculationUtility.GetTaxPrice(
						item.Sum(
							itemKey => (((decimal)itemKey[TaxCalculationUtility.HASH_KEY_TAXABLE_ITEM_PRICE])
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]
								+ (decimal)itemKey[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE])),
						(decimal)item.Key.taxRate,
						Constants.TAX_EXCLUDED_FRACTION_ROUNDING,
						true)
				}).ToList();

			return returnExchangePriceByTaxRateModel;
		}

		/// <summary>
		/// 税率毎ポイント調整情報計算(返品交換用)
		/// </summary>
		/// <param name="adjustmentPointPrice">ポイント調整金額</param>
		/// <param name="returnAndExchangeItems">返品交換対象商品情報</param>
		/// <param name="cart">返品交換後カート情報</param>
		/// <returns>税率毎ポイント調整金額情報</returns>
		public static List<OrderPriceByTaxRateModel> CalculateAdjustmentPointPriceByTaxRate(
			decimal adjustmentPointPrice,
			List<ReturnOrderItem> returnAndExchangeItems,
			CartObject cart)
		{
			var pointPriceByTaxRate = new List<OrderPriceByTaxRateModel>();

			if (adjustmentPointPrice == 0) return pointPriceByTaxRate;

			var cartForCalculate = cart.Copy();

			cartForCalculate.CalculatePriceSubTotal();

			cartForCalculate.ProrateDiscountPrice(cartForCalculate.Items, adjustmentPointPrice, false);

			if (cartForCalculate.Items.Any() == false)
			{
				pointPriceByTaxRate.Add(
					new OrderPriceByTaxRateModel()
					{
						KeyTaxRate = returnAndExchangeItems.First().ProductTaxRate,
						PriceSubtotalByRate = adjustmentPointPrice * -1
					});
				return pointPriceByTaxRate;
			}

			pointPriceByTaxRate.AddRange(cartForCalculate.Items.GroupBy(item => item.TaxRate).Select(
				item => new OrderPriceByTaxRateModel()
				{
					KeyTaxRate = item.Key,
					PriceSubtotalByRate = item.Sum(
						product => ((cart.Shippings[0].IsDutyFree ? product.PriceSubtotal : product.PriceSubtotalPretax)
							- product.PriceSubtotalAfterDistribution) * -1)
				}));

			return pointPriceByTaxRate;
		}

		/// <summary>
		/// 税率毎金額補正情報計算(返品交換用)
		/// </summary>
		/// <param name="cart">返品交換後カート情報</param>
		/// <param name="lastBilledAmountByTaxRate">返品交換前の税率毎価格情報</param>
		/// <param name="pointPriceByTaxRate">税率毎ポイント価格情報</param>
		/// <param name="returnAndExchangeItems">返品交換対象商品</param>
		/// <param name="allReturnFixedAmountCourseIds">全返品する頒布会定額コースID配列</param>
		/// <returns>税率毎金額補正情報</returns>
		public static List<OrderPriceByTaxRateModel> CalculatePriceCorrectionByTaxRate(
			CartObject cart,
			List<OrderPriceByTaxRateModel> lastBilledAmountByTaxRate,
			List<OrderPriceByTaxRateModel> pointPriceByTaxRate,
			List<ReturnOrderItem> returnAndExchangeItems,
			string[] allReturnFixedAmountCourseIds)
		{
			var correctionPriceByTaxRate = CalculateDifferencePriceByTaxRate(
				lastBilledAmountByTaxRate,
				cart.PriceInfoByTaxRate.Select(priceInfoByTaxRate => priceInfoByTaxRate.CreateModel()).ToList());

			var shippingAndPaymentFee = cart.PriceShipping;
			if (cart.Payment != null)
			{
				shippingAndPaymentFee += cart.Payment.PriceExchange;
			}
			var adjustmentToProduct = ((shippingAndPaymentFee % correctionPriceByTaxRate.Count) != 0)
				? Math.Floor(shippingAndPaymentFee / correctionPriceByTaxRate.Count)
				: (shippingAndPaymentFee / correctionPriceByTaxRate.Count);
			var adjustment = 0m;

			foreach (var correctionPriceInfo in correctionPriceByTaxRate)
			{
				var targetPointPriceByTaxRate = pointPriceByTaxRate.FirstOrDefault(
					pointPriceInfo => pointPriceInfo.KeyTaxRate == correctionPriceInfo.KeyTaxRate);
				var pointPrice = (targetPointPriceByTaxRate != null)
					? targetPointPriceByTaxRate.PriceSubtotalByRate
					: 0m;

				// 頒布会定額コース商品を除いた分を計算
				var returnExchangePriceSubtotal = returnAndExchangeItems
					.Where(
						roi => (roi.ProductTaxRate == correctionPriceInfo.KeyTaxRate)
							&& (roi.IsSubscriptionBoxFixedAmount == false))
					.Sum(roi => roi.ItemPriceIncludedTax);

				// 頒布会定額コース商品分を計算
				foreach (var subscriptionBoxCourseId in allReturnFixedAmountCourseIds)
				{
					var subscriptionBox = DataCacheControllerFacade.GetSubscriptionBoxCacheController()
						.Get(subscriptionBoxCourseId);
					var subscriptionBoxTaxRate = DomainFacade.Instance.ProductTaxCategoryService.Get(subscriptionBox.TaxCategoryId).TaxRate;

					if (subscriptionBoxTaxRate != correctionPriceInfo.KeyTaxRate) continue;

					returnExchangePriceSubtotal += returnAndExchangeItems
						.FirstOrDefault(item => item.SubscriptionBoxCourseId == subscriptionBoxCourseId)
						.SubscriptionBoxFixedAmount.GetValueOrDefault() * -1;
				}

				correctionPriceInfo.ReturnPriceCorrectionByRate = (correctionPriceInfo.PriceSubtotalByRate
					+ correctionPriceInfo.ReturnPriceCorrectionByRate
					+ correctionPriceInfo.PriceShippingByRate
					+ correctionPriceInfo.PricePaymentByRate
					+ returnExchangePriceSubtotal
					+ pointPrice) * -1;
				correctionPriceInfo.PriceSubtotalByRate =
					correctionPriceInfo.PriceShippingByRate =
						correctionPriceInfo.PricePaymentByRate =
							correctionPriceInfo.PriceTotalByRate =
								correctionPriceInfo.TaxPriceByRate = 0m;

				if (cart.IsReturnAllItems
					&& cart.IsPaymentAtoneOrAftee
					&& (cart.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED))
				{
					adjustment += adjustmentToProduct;
					correctionPriceInfo.ReturnPriceCorrectionByRate -= adjustmentToProduct;
				}
			}

			if ((adjustment != 0m)
				&& (correctionPriceByTaxRate.Count > 0)
				&& ((shippingAndPaymentFee - adjustment) > 0))
			{
				correctionPriceByTaxRate[0].ReturnPriceCorrectionByRate -= (shippingAndPaymentFee - adjustment);
			}

			return correctionPriceByTaxRate;
		}

		/// <summary>
		/// 変更前と変更後の税率毎価格情報の差額を計算する
		/// </summary>
		/// <param name="orderPriceByTaxRateBefore">変更前税率毎価格情報</param>
		/// <param name="orderPriceByTaxRateAfter">変更後税率毎価格情報</param>
		/// <returns>税率毎差額情報</returns>
		public static List<OrderPriceByTaxRateModel> CalculateDifferencePriceByTaxRate(
			List<OrderPriceByTaxRateModel> orderPriceByTaxRateBefore,
			List<OrderPriceByTaxRateModel> orderPriceByTaxRateAfter)
		{
			var orderPriceByTaxRateSummaryForCalculate = new List<OrderPriceByTaxRateModel>();
			var minusOrderPriceByTaxRateAfter = orderPriceByTaxRateAfter.Select(
				info => new OrderPriceByTaxRateModel
				{
					KeyTaxRate = info.KeyTaxRate,
					PriceSubtotalByRate = -info.PriceSubtotalByRate,
					PricePaymentByRate = -info.PricePaymentByRate,
					PriceShippingByRate = -info.PriceShippingByRate,
					ReturnPriceCorrectionByRate = -info.ReturnPriceCorrectionByRate,
					PriceTotalByRate = -info.PriceTotalByRate,
					TaxPriceByRate = -info.TaxPriceByRate
				});
			orderPriceByTaxRateSummaryForCalculate.AddRange(orderPriceByTaxRateBefore);
			orderPriceByTaxRateSummaryForCalculate.AddRange(minusOrderPriceByTaxRateAfter);
			var differencePriceByTaxRate = orderPriceByTaxRateSummaryForCalculate
				.GroupBy(orderPriceByTaxRate => orderPriceByTaxRate.KeyTaxRate)
				.Select(groupedPriceByTaxRateSummary => new OrderPriceByTaxRateModel()
				{
					KeyTaxRate = groupedPriceByTaxRateSummary.Key,
					PriceSubtotalByRate = groupedPriceByTaxRateSummary.Sum(itemKey => itemKey.PriceSubtotalByRate),
					PriceShippingByRate = groupedPriceByTaxRateSummary.Sum(itemKey => itemKey.PriceShippingByRate),
					PricePaymentByRate = groupedPriceByTaxRateSummary.Sum(itemKey => itemKey.PricePaymentByRate),
					ReturnPriceCorrectionByRate = groupedPriceByTaxRateSummary.Sum(itemKey => itemKey.ReturnPriceCorrectionByRate),
					PriceTotalByRate = groupedPriceByTaxRateSummary.Sum(itemKey => itemKey.PriceTotalByRate),
					TaxPriceByRate = groupedPriceByTaxRateSummary.Sum(itemKey => itemKey.TaxPriceByRate),
				}).ToList();

			return differencePriceByTaxRate;
		}

		#region -CreateOrderPriceByTaxRate 税率毎価格情報作成(配送先考慮しない)
		/// <summary>
		/// 税率毎価格情報作成
		/// </summary>
		/// <param name="order">受注情報</param>
		/// <returns>税率毎価格情報</returns>
		public static OrderPriceByTaxRateModel[] CreateOrderPriceByTaxRate(OrderModel order)
		{
			var stackedDiscountAmount = 0m;
			var priceTotal = order.Items.Sum(item => PriceCalculator.GetItemPrice(item.ProductPricePretax, item.ItemQuantity));
			// 調整金額適用対象金額取得
			var shippingPrice = order.OrderPriceShipping;
			var paymentPrice = order.OrderPriceExchange;
			priceTotal += paymentPrice;
			priceTotal += shippingPrice;

			var itemInfo = new List<Hashtable>();
			if (priceTotal != 0)
			{
				itemInfo.AddRange(order.Items.Select(
					item => new Hashtable
					{
						{ "itemPriceRegulation", Math.Floor(PriceCalculator.GetDistributedPrice(order.OrderPriceRegulation, PriceCalculator.GetItemPrice(item.ProductPricePretax, item.ItemQuantity), priceTotal)) },
						{ "item", item}
					}));
				stackedDiscountAmount = itemInfo.Sum(item => (decimal)item["itemPriceRegulation"]);
			}
			var shippingRegulationPrice = Math.Floor(PriceCalculator.GetDistributedPrice(order.OrderPriceRegulation, shippingPrice, priceTotal));
			stackedDiscountAmount += shippingRegulationPrice;

			var paymentRegulationPrice = Math.Floor(PriceCalculator.GetDistributedPrice(order.OrderPriceRegulation, paymentPrice, priceTotal));
			stackedDiscountAmount += paymentRegulationPrice;

			var fractionAmount = order.OrderPriceRegulation - stackedDiscountAmount;

			if (fractionAmount != 0)
			{
				var weightItem = itemInfo.FirstOrDefault(
					item => (PriceCalculator.GetItemPrice(((OrderItemModel)item["item"]).ProductPricePretax, ((OrderItemModel)item["item"]).ItemQuantity)) > 0);
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

			var priceInfo = new List<Hashtable>();
			// 税率毎の購入金額を算出する
			priceInfo.AddRange(itemInfo
				.Select(item => new Hashtable
				{
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , ((OrderItemModel)item["item"]).ProductTaxRate },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE ,
						PriceCalculator.GetItemPrice(((OrderItemModel)item["item"]).ProductPricePretax, ((OrderItemModel)item["item"]).ItemQuantity) + (decimal)item["itemPriceRegulation"] },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , 0m },
					{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
				}).ToList());

			priceInfo.Add(new Hashtable
			{
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , order.ShippingTaxRate },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE , 0m },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE , shippingPrice + shippingRegulationPrice },
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE , 0m },
			});
			priceInfo.Add(new Hashtable
			{
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE , order.PaymentTaxRate },
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
				orderPriceByTaxRateModel.OrderId = order.OrderId;
			}
			return priceByTaxRate;
		}
		#endregion

		/// <summary>
		/// Get Order Search Multi Order Id
		/// </summary>
		/// <param name="input">Input</param>
		/// <returns>String Multi Order Id</returns>
		public static string GetOrderSearchMultiOrderId(Hashtable input)
		{
			var result = StringUtility.ToEmpty(input[Constants.FIELD_ORDER_ORDER_ID + "_like_escaped"]).Replace("'", "''").Replace(",", "','");

			return result;
		}

		/// <summary>
		/// 外部決済連携ログ格納処理
		/// </summary>
		/// <param name="isSuccess"></param>
		/// <param name="orderId"></param>
		/// <param name="externalPaymentLog"></param>
		/// <param name="lastChanged"></param>
		/// <param name="updateHistoryAction"></param>
		/// <param name="accessor"></param>
		/// <returns>変更結果の行数。Appendできれば1</returns>
		public static int AppendExternalPaymentCooperationLog(
			bool isSuccess,
			string orderId,
			string externalPaymentLog,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			var organizedLog = string.Format(
				"{0:yyyy/MM/dd HH:mm:ss}\t[{1}]\t{2}\t最終更新者:{3}\r\n",
				DateTime.Now,
				isSuccess ? "成功" : "失敗",
				externalPaymentLog.Replace("\r\n", "\t"),
				lastChanged);

			var result = new OrderService().AppendExternalPaymentCooperationLog(
				orderId,
				organizedLog,
				lastChanged,
				updateHistoryAction,
				accessor);

			return result;
		}

		/// <summary>
		/// ユーザーリアルタイム累計購入回数更新処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="actionName">アクション名</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>結果</returns>
		public static bool UpdateRealTimeOrderCount(Hashtable order, string actionName, SqlAccessor accessor = null)
		{
			var userId = (string)order[Constants.FIELD_ORDER_USER_ID];
			var orderCountOld = (order.Contains(Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME))
				? (int)order[Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME]
				: new UserService().Get(userId, accessor).OrderCountOrderRealtime;
			var orderCountNew = orderCountOld;
			switch (actionName)
			{
				//注文処理
				case Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_ORDER:
					orderCountNew = orderCountOld + 1;
					break;

				//キャンセル処理
				case Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_CANCEL:
					orderCountNew = orderCountOld - 1;
					break;

				//注文同梱処理
				case Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_COMBINE:
					var cancelCount = (int)order["cancelCount"];
					orderCountNew = orderCountOld - cancelCount;
					break;

				//返品交換処理
				case Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_RETURN_EXCHANGE:
					orderCountNew = orderCountOld;
					break;

				//ロールバック処理
				case Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_ROLLBACK:
					orderCountNew = orderCountOld;
					break;
			}
			var result = new UserService().UpdateRealTimeOrderCount(userId, orderCountNew, accessor);
			return (result > 0);
		}

		/// <summary>
		/// 請求書同梱サービスが利用可能か？
		/// </summary>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>利用可能であればTRUE</returns>
		public static bool IsInvoiceBundleServiceUsable(string paymentId = null)
		{
			// For case order use NP after pay
			if ((string.IsNullOrEmpty(paymentId) == false)
				&& (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY))
			{
				return Constants.PAYMENT_NP_AFTERPAY_INVOICEBUNDLE;
			}

			if ((string.IsNullOrEmpty(paymentId) == false)
				&& (paymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)) return false;

			switch (Constants.PAYMENT_CVS_DEF_KBN)
			{
				case Constants.PaymentCvsDef.Atodene:
					return Constants.PAYMENT_SETTING_ATODENE_USE_INVOICE_BUNDLE_SERVICE;

				case Constants.PaymentCvsDef.Gmo:
					return Constants.PAYMENT_SETTING_GMO_DEFERRED_INVOICEBUNDLE;

				case Constants.PaymentCvsDef.Dsk:
					return Constants.PAYMENT_SETTING_DSK_DEFERRED_USE_INVOICE_BUNDLE;

				case Constants.PaymentCvsDef.YamatoKa:
					return false;

				case Constants.PaymentCvsDef.Atobaraicom:
					return Constants.PAYMENT_SETTING_ATOBARAICOM_USE_INVOICE_BUNDLE_SERVICE;

				case Constants.PaymentCvsDef.Score:
					return Constants.PAYMENT_SETTING_SCORE_DEFERRED_USE_INVOICE_BUNDLE;

				case Constants.PaymentCvsDef.Veritrans:
					return Constants.PAYMENT_SETTING_VERITRANS_USE_INVOICE_BUNDLE;
			}

			return false;
		}

		/// <summary>
		/// 住所情報から請求書同梱フラグを判定する
		/// </summary>
		/// <returns>請求書同梱フラグ</returns>
		public static string JudgmentInvoiceBundleFlg(OrderModel model)
		{
			if (model.IsGiftOrder) return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;

			// For case order use NP after pay
			if (model.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				return model.JudgmentNPAfterPayInvoiceBundleFlg();
			}

			switch (Constants.PAYMENT_CVS_DEF_KBN)
			{
				case Constants.PaymentCvsDef.Atodene:
					return model.JudgmentInvoiceBundleFlg();

				case Constants.PaymentCvsDef.Gmo:
					return model.JudgmentGmoInvoiceBundleFlg();

				case Constants.PaymentCvsDef.Dsk:
					return model.JudgmentInvoiceBundleFlg();

				case Constants.PaymentCvsDef.YamatoKa:
					return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;

				case Constants.PaymentCvsDef.Atobaraicom:
					return model.JudgmentAtobaraicomInvoiceBundleFlg();

				case Constants.PaymentCvsDef.Score:
					return model.JudgmentInvoiceBundleFlg();

				case Constants.PaymentCvsDef.Veritrans:
					return model.JudgmenVeritransInvoiceBundleFlg();
			}

			return Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF;
		}

		/// <summary>
		/// 同梱時の再与信の対象となるかどうか
		/// </summary>
		/// <param name="newOrder">新しい受注情報</param>
		/// <param name="exculdeOrderIds">同梱対象の受注ID</param>
		/// <param name="isOrderCombined">同梱かどうか</param>
		/// <param name="combinedReauthOrders">同梱される再与信対象の受注情報</param>
		/// <returns></returns>
		public static bool IsAtodeneReauthByCombine(
			Hashtable newOrder,
			string[] exculdeOrderIds,
			bool isOrderCombined,
			out List<OrderModel> combinedReauthOrders)
		{
			combinedReauthOrders = null;
			if ((exculdeOrderIds == null) || (isOrderCombined == false))
			{
				return false;
			}

			combinedReauthOrders = new OrderService().GetMulitpleOrdersByOrderIdsAndPaymentKbn(
				exculdeOrderIds.ToList(),
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF).ToList();
			var result = IsAtodeneReauthByNewOrder(
				combinedReauthOrders,
				(string)newOrder[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]);

			return result;
		}

		/// <summary>
		/// 新しい受注による再与信の対象かどうか
		/// </summary>
		/// <param name="sourceOrders">元の受注</param>
		/// <param name="newOrderPaymentKbn">新しい受注の決済区分</param>
		/// <param name="reauthOrders">再与信対象となる受注</param>
		/// <returns></returns>
		public static bool IsAtodeneReauthByNewOrder(
			List<OrderModel> sourceOrders,
			string newOrderPaymentKbn)
		{
			if (sourceOrders.Any()
				&& (newOrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atodene))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 配送方法を判定するかどうか
		/// </summary>
		/// <param name="paymentKbn">支払方法</param>
		/// <returns>配送方法を判定するかどうか</returns>
		/// <remarks>支払い方法が代金引換ではない かつ サイズ係数閾値判定 に自動判定処理を実行</remarks>
		public static bool IsDecideDeliveryMethod(string paymentKbn)
		{
			var result = (paymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT);
			return result;
		}

		/// <summary>クレジットカード会社が選択可能か</summary>
		public static bool CreditCompanySelectable
		{
			get
			{
				return (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc);
			}
		}
		/// <summary>クレジットカード会社ValueTextフィールド名</summary>
		public static string CreditCompanyValueTextFieldName
		{
			get
			{
				switch (Constants.PAYMENT_CARD_KBN)
				{
					case Constants.PaymentCard.YamatoKwc:
					default:
						return "credit_company_yamato_kwc_api";
				}
			}
		}
		/// <summary>クレジット分割払いが可能か</summary>
		public static bool CreditInstallmentsSelectable
		{
			get
			{
				var result = (((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS) && Constants.PAYMENT_SETTING_SBPS_CREDIT_DIVIDE)
						|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc) && Constants.PAYMENT_SETTING_YAMATO_KWC_CREDIT_DIVIDE)
						|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus) && Constants.PAYMENT_SETTING_ZEUS_DIVIDE)
						|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.EScott) && Constants.PAYMENT_SETTING_SONYPAYMENT_ESCOTT_DIVID)
						|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo)
						|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
						|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) && Constants.PAYMENT_SETTING_RAKUTEN_CREDIT_DIVIDE)
						|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent));
				return result;
			}
		}
		/// <summary>クレジット分割払いValueTextフィールド名</summary>
		public static string CreditInstallmentsValueTextFieldName
		{
			get
			{
				switch (Constants.PAYMENT_CARD_KBN)
				{
					case Constants.PaymentCard.SBPS:
						return Constants.FIELD_CREDIT_INSTALLMENTS_SBPS;

					case Constants.PaymentCard.YamatoKwc:
						return Constants.FIELD_CREDIT_INSTALLMENTS_YAMATOKWC;

					case Constants.PaymentCard.EScott:
						return Constants.FIELD_CREDIT_INSTALLMENTS_ESCOTT;

					case Constants.PaymentCard.VeriTrans:
						return Constants.FIELD_CREDIT_INSTALLMENTS_VERITRANS;

					case Constants.PaymentCard.Rakuten:
						return Constants.FIELD_CREDIT_INSTALLMENTS_RAKUTEN;

					case Constants.PaymentCard.Paygent:
						return Constants.FIELD_CREDIT_INSTALLMENTS_PAYGENT;

					default:
						return Constants.FIELD_CREDIT_INSTALLMENTS;
				}
			}
		}
		/// <summary>クレジットカード セキュリティコード利用可能か</summary>
		public static bool CreditSecurityCodeEnable
		{
			get
			{
				var result = (((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus) && Constants.PAYMENT_SETTING_ZEUS_SECURITYCODE)
					|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo) && Constants.PAYMENT_SETTING_GMO_SECURITYCODE)
					|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS) && Constants.PAYMENT_SETTING_SBPS_CREDIT_SECURITYCODE)
					|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc) && Constants.PAYMENT_SETTING_YAMATO_KWC_CREDIT_SECURITYCODE)
					|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zcom) && Constants.PAYMENT_SETTING_CREDIT_ZCOM_SECURITYCODE)
					|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.EScott)
					|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans) && Constants.PAYMENT_SETTING_CREDIT_VERITRANS4G_SECURITYCODE)
					|| ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten) && Constants.PAYMENT_SETTING_CREDIT_RAKUTEN_SECURITYCODE)
					|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent) && Constants.PAYMENT_SETTING_CREDIT_PAYGENT_SECURITYCODE);
				return result;
			}
		}
		/// <summary>クレジットカード保存機能が利用可能か</summary>
		public static bool CreditCardRegistable
		{
			get
			{
				var result = ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus)
					|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS)
					|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc)
					|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo)
					|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zcom)
					|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.EScott)
					|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
					|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
					|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent));
				return result;
			}
		}
		/// <summary>トークン決済利用するか</summary>
		public static bool CreditTokenUse
		{
			get
			{
				var isFront = (Constants.CONFIGURATION_SETTING.ReadKbnList.Any(s => s == ConfigurationSetting.ReadKbn.C300_Pc));

				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus)
				{
					return (isFront || SessionManager.UsePaymentTabletZeus);
				}
				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS)
				{
					return isFront;
				}
				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo)
				{
					return isFront;
				}
				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc)
				{
					return isFront;
				}
				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.EScott)
				{
					return true;
				}
				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
				{
					return isFront;
				}
				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
				{
					return isFront;
				}
				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent)
				{
					return isFront;
				}
				return false;
			}
		}
		/// <summary>永久トークン利用するか</summary>
		public static bool CreditTokenizedPanUse
		{
			get
			{
				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.SBPS)
				{
					return (Constants.CONFIGURATION_SETTING.ReadKbnList.Any(s => s == ConfigurationSetting.ReadKbn.C200_CommonManager));
				}
				return false;
			}
		}
		/// <summary>PayTgを利用するか</summary>
		public static bool IsUserPayTg
		{
			get
			{
				var result = (Constants.PAYMENT_SETTING_PAYTG_ENABLED
					&& ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
						|| (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)));
				return result;
			}
		}
		/// <summary>クレジットカード仮登録が必要カード区分か（すべて） ※仮クレカを利用するかはこの条件に加え自社サイト注文の場合のみ</summary>
		public static bool NeedsRegisterProvisionalCreditCardCardKbn
		{
			get
			{
				return NeedsRegisterProvisionalCreditCardCardKbnZeus
					|| NeedsRegisterProvisionalCreditCardCardKbnExceptZeus;
			}
		}	/// <summary>クレジットカード仮登録が必要カード区分か(ZEUS) ※仮クレカを利用するかはこの条件に加え自社サイト注文の場合のみ</summary>
		public static bool NeedsRegisterProvisionalCreditCardCardKbnZeus
		{
			get
			{
				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus) return true;
				return false;	// 他に影響を与えないようにする
			}
		}
		/// <summary>クレジットカード仮登録が必要なカード区分か(ZEUS以外) ※仮クレカを利用するかはこの条件に加え自社サイト注文の場合のみ</summary>
		public static bool NeedsRegisterProvisionalCreditCardCardKbnExceptZeus
		{
			get
			{
				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo) return true;
				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc) return true;
				if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.EScott) return true;
				return false;	// 他に影響を与えないようにする
			}
		}
		/// <summary>クレジット決済はZEUS利用か</summary>
		public static bool IsPaymentCardTypeZeus
		{
			get { return (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Zeus); }
		}
		/// <summary>クレジット決済はGMO利用か</summary>
		public static bool IsPaymentCardTypeGmo
		{
			get { return (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Gmo); }
		}
		/// <summary>クレジット決済はヤマトKWC利用か</summary>
		public static bool IsPaymentCardTypeYamatoKwc
		{
			get { return (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.YamatoKwc); }
		}
		/// <summary>クレジット決済はe-SCOTT利用か</summary>
		public static bool IsPaymentCardTypeEScott
		{
			get { return (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.EScott); }
		}
		/// <summary>コンビニ決済 支払先ValueTextフィールド名</summary>
		public static string CvsCodeValueTextFieldName
		{
			get
			{
				switch (Constants.PAYMENT_CVS_KBN)
				{
					// SBPSコンビニ支払先
					case Constants.PaymentCvs.SBPS:
						return "sbps_cvs_dsk_type";

					// ヤマトKWCコンビニ支払先
					case Constants.PaymentCvs.YamatoKwc:
						return "yamatokwc_cvs_type";

					// 電算システムコンビニ支払先
					case Constants.PaymentCvs.Dsk:
						return PaymentDskCvs.FIELD_CVS_TYPE;

					// Gmo convenience store payment
					case Constants.PaymentCvs.Gmo:
						return Constants.PAYMENT_GMO_CVS_TYPE;

					// Rakuten convenience store payment
					case Constants.PaymentCvs.Rakuten:
						return Constants.PAYMENT_RAKUTEN_CVS_TYPE;

					// Zeus convenience store payment
					case Constants.PaymentCvs.Zeus:
						return Constants.PAYMENT_ZEUS_CVS_TYPE;

					// Paygentコンビニ支払い先
					case Constants.PaymentCvs.Paygent:
						return Constants.PAYMENT_PAYGENT_CVS_TYPE;

					default:
						throw new Exception("未対応のコンビニタイプ：" + Constants.PAYMENT_CVS_KBN);
				}
			}
		}
		/// <summary>Is payment cvs type zeus</summary>
		public static bool IsPaymentCvsTypeZeus
		{
			get { return Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Zeus; }
		}
		/// <summary>コンビニタイプがPaygentかどうか</summary>
		public static bool IsPaymentCvsTypePaygent
		{
			get { return Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Paygent; }
		}

		/// <summary>
		/// 注文ステータス更新ステートメント取得
		/// </summary>
		/// <param name="status">ステータス値</param>
		/// <returns>ステータス更新ステートメント</returns>
		public static string GetUpdateOrderStatusStatement(string status)
		{
			switch (status)
			{
				// 注文済み
				case Constants.FLG_ORDER_ORDER_STATUS_ORDERED:
					return "UpdateOrderStatusComp";

				// 受注承認
				// 在庫引当済み
				case Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED:
					return "UpdateOrderStatusConfirm";

				// 出荷手配済み
				case Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED:
					return Constants.REALSTOCK_OPTION_ENABLED ? "UpdateOrderStatusReservedStock" : null;

				// 出荷完了
				case Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED:
					return "UpdateOrderStatusShipping";

				// 配送完了
				case Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP:
					return "UpdateOrderStatusForwardComplete";

				// キャンセル
				case Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP:
					return "UpdateOrderStatusDelivering";

				// 仮注文キャンセル
				case Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED:
					return "UpdateOrderStatusCancel";

				case Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED:
					return "UpdateOrderStatusTempOrderCancel";

				default:
					return null;
			}
		}

		/// <summary>
		/// 注文入金ステータス更新ステートメント取得
		/// </summary>
		/// <param name="status">ステータス値</param>
		/// <returns>ステータス更新ステートメント</returns>
		public static string GetUpdateOrderPaymentStatusStatement(string status)
		{
			switch (status)
			{
				// 入金確認待ち
				// 入金済
				case Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM:
					return "UpdateOrderPaymentStatusConfirm";

				// 一部入金済へ変更
				case Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE:
					return "UpdateOrderPaymentStatusComplete";

				case Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_SHORTAGE:
					return "UpdateOrderPaymentStatusShortage";

				default:
					return null;
			}
		}

		/// <summary>
		/// 注文返品交換ステータス更新ステートメント取得
		/// </summary>
		/// <param name="status">ステータス値</param>
		/// <returns>ステータス更新ステートメント</returns>
		public static string GetUpdateOrderReturnExchangeStatusStatement(string status)
		{
			switch (status)
			{
				// 返品受付
				// 返品商品到着
				case Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_RECEIPT:
					return "UpdateOrderReturnExchangeStatusReceipt";

				// 返品処理計上
				case Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_ARRIVAL:
					return "UpdateOrderReturnExchangeStatusArrival";

				case Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE:
					return "UpdateOrderReturnExchangeStatusComplete";

				default:
					return null;
			}
		}

		/// <summary>
		/// 返品交換向け注文登録
		/// </summary>
		/// <param name="order">Order input</param>
		/// <param name="returnOrder">Return Order</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void OrderRegisterForReturnExchange(
			Hashtable order,
			Order returnOrder,
			UpdateHistoryAction updateHistoryAction,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			new OrderService().InsertOrderForOrderReturnExchange(order, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(returnOrder.OrderId, loginOperatorName, accessor);
			}
		}

		/// <summary>
		/// Regist Order Owner For Return Exchange
		/// </summary>
		/// <param name="order">Order input</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// /// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void RegistOrderOwnerForReturnExchange(
			Hashtable order,
			UpdateHistoryAction updateHistoryAction,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			new OrderService().InsertOrderOwnerForOrderReturnExchange(order, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(
					StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
					loginOperatorName, accessor);
			}
		}

		/// <summary>
		/// Regist Order Set Promotion For Return Exchange
		/// </summary>
		/// <param name="order">Order input</param>
		/// <param name="returnOrder">Return Order</param>
		/// <param name="listReturnOrderItem">List Return Order Item</param>
		/// <param name="exchangeOrderItems">Exchange Order Items</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void RegistOrderSetPromotionForReturnExchange(
			Hashtable order,
			Order returnOrder,
			List<ReturnOrderItem> listReturnOrderItem,
			List<ReturnOrderItem> exchangeOrderItems,
			UpdateHistoryAction updateHistoryAction,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			foreach (var orderSetPromotion in returnOrder.SetPromotions)
			{
				var targetItems = new List<ReturnOrderItem>();
				targetItems.AddRange(listReturnOrderItem.FindAll(item => item.OrderSetPromotionNo == orderSetPromotion.OrderSetPromotionNo));
				if (exchangeOrderItems != null)
				{
					targetItems.AddRange(exchangeOrderItems.FindAll(item => item.OrderSetPromotionNo == orderSetPromotion.OrderSetPromotionNo));
				}

				if (targetItems.Count != 0)
				{
					var orderSetPromotionInput = new Hashtable
					{
						{ Constants.FIELD_ORDERSETPROMOTION_ORDER_ID, order[Constants.FIELD_ORDER_ORDER_ID] },
						{ Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO, orderSetPromotion.OrderSetPromotionNo },
						{ Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID, orderSetPromotion.SetPromotionId },
						{ Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME, orderSetPromotion.SetPromotionName },
						{ Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME, orderSetPromotion.SetPromotionDispName },
						{ Constants.FIELD_ORDERSETPROMOTION_UNDISCOUNTED_PRODUCT_SUBTOTAL, targetItems.Sum(item => item.ItemPrice) },
						{ Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG, orderSetPromotion.IsDiscountTypeProductDiscount
							? Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON
							: Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_OFF },
						{ Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT, 0 },
						{ Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG, orderSetPromotion.IsDiscountTypeShippingChargeFree
							? Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON
							: Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_OFF },
						{ Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT, 0 },
						{ Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG, orderSetPromotion.IsDiscountTypePaymentChargeFree
							? Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON
							: Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_OFF },
						{ Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT, 0 },
					};
					new OrderService().InsertOrderSetPromotion(orderSetPromotionInput, accessor);
				}
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(
					StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
					loginOperatorName,
					accessor);
			}
		}

		/// <summary>
		/// 税率毎価格情報登録
		/// </summary>
		/// <param name="order">Order input</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void ExecuteRegistOrderPriceInfoByTaxRate(
			Hashtable order,
			SqlAccessor accessor)
		{
			foreach (var info in (List<OrderPriceByTaxRateModel>)order[Constants.TABLE_ORDERPRICEBYTAXRATE])
			{
				info.OrderId = (string)order[Constants.FIELD_ORDER_ORDER_ID];
				info.DateCreated = DateTime.Now;
				info.DateChanged = DateTime.Now;
				new OrderService().InsertOrderPriceInfoByTaxRate(info.DataSource, accessor);
			}
		}

		/// <summary>
		/// Update Shipped Changed Kbn For Return Exchange
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void UpdateShippedChangedKbnForReturnExchange(
			string orderId,
			UpdateHistoryAction updateHistoryAction,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			var order = new Hashtable()
			{
				{ Constants.FIELD_ORDER_ORDER_ID_ORG, orderId },
				{ Constants.FIELD_ORDER_LAST_CHANGED, loginOperatorName },
			};
			new OrderService().UpdateShippedChangedKbn(order, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, loginOperatorName, accessor);
			}
		}

		/// <summary>
		/// Update Coupon For Return Exchange
		/// </summary>
		/// <param name="orderNew">Order new</param>
		/// <param name="returnOrder">Return Order</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Update coupon result</returns>
		public static string UpdateCouponForReturnExchange(
			Hashtable orderNew,
			Order returnOrder,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			var transaction = string.Empty;
			try
			{
				if (Constants.W2MP_COUPON_OPTION_ENABLED)
				{
					if (orderNew[Constants.TABLE_ORDERCOUPON] != null)
					{
						var orderCoupon = (OrderCoupon)orderNew[Constants.TABLE_ORDERCOUPON];

						var success = false;
						if (orderCoupon.IsCouponLimit)
						{
							// ユーザクーポン情報(変更前)を未使用にする
							transaction = "1-3-1.ユーザクーポン情報(利用クーポン)UPDATE処理";
							success = UpdateUnUseUserCouponForReturnExchange(returnOrder, UpdateHistoryAction.DoNotInsert, loginOperatorName, accessor);
						}
						else if (orderCoupon.IsCouponAllLimit)
						{
							// クーポン利用回数(変更前) プラス１して数を戻す
							transaction = "1-3-2.クーポン情報(クーポン利用回数)UPDATE処理";
							success = UpdateCouponCountUpForReturnExchange(returnOrder, loginOperatorName, accessor);
						}
						else if (orderCoupon.IsBlacklistCoupon)
						{
							// クーポン利用ユーザを未利用に更新する
							transaction = "1-3-2.クーポン利用ユーザ情報(未利用)UPDATE処理";
							success = DeleteCouponUseUserForReturnExchange(returnOrder, accessor);
						}
						if (success)
						{
							transaction = "1-3-3.ユーザクーポン履歴情報(利用クーポン)INSERT処理";
							InsertUserCouponHistoryForReturnExchange(
								returnOrder,
								Constants.FLG_USERCOUPONHISTORY_HISTORY_KBN_USE_CANCEL,
								Constants.FLG_USERCOUPONHISTORY_ACTION_KBN_ORDER,
								1,
								decimal.Parse(returnOrder.OrderCouponUse) * -1,
								loginOperatorName,
								accessor);
						}
					}
				}
			}
			catch
			{
				return transaction;
			}

			return string.Empty;
		}

		/// <summary>
		/// Update Un Use User Coupon For Return Exchange
		/// </summary>
		/// <param name="returnOrder">Return Order</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理結果</returns>
		private static bool UpdateUnUseUserCouponForReturnExchange(Order returnOrder, UpdateHistoryAction updateHistoryAction, string loginOperatorName, SqlAccessor accessor)
		{
			var result = UpdateUnUseUserCoupon(
				returnOrder.UserId,
				returnOrder.Coupon.DeptId,
				returnOrder.Coupon.CouponId,
				int.Parse(returnOrder.Coupon.CouponNo),
				updateHistoryAction,
				loginOperatorName,
				accessor);

			return result;
		}

		/// <summary>
		/// ユーザクーポン情報更新(使用済み→未使用)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポン}ID</param>
		/// <param name="couponNo">枝番</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理結果</returns>
		public static bool UpdateUnUseUserCoupon(
			string userId,
			string deptId,
			string couponId,
			int couponNo,
			UpdateHistoryAction updateHistoryAction,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			var userCoupon = new UserCouponModel
			{
				UserId = userId,
				DeptId = deptId,
				CouponId = couponId,
				CouponNo = couponNo,
				LastChanged = loginOperatorName,
			};
			var result = new CouponService().UpdateUnUseUserCoupon(userCoupon, updateHistoryAction, accessor);
			return (result > 0);
		}

		/// <summary>
		/// Update Coupon Count Up For Return Exchange
		/// </summary>
		/// <param name="returnOrder">Return Order</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理結果</returns>
		private static bool UpdateCouponCountUpForReturnExchange(Order returnOrder, string loginOperatorName, SqlAccessor accessor)
		{
			var result = UpdateCouponCountUp(
				returnOrder.Coupon.DeptId,
				returnOrder.Coupon.CouponId,
				returnOrder.Coupon.CouponCode,
				loginOperatorName,
				accessor);

			return result;
		}

		/// <summary>
		/// クーポン残り利用回数をプラス１する
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポン}ID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理結果</returns>
		public static bool UpdateCouponCountUp(
			string deptId,
			string couponId,
			string couponCode,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			var result = new CouponService().UpdateCouponCountUp(deptId, couponId, couponCode, loginOperatorName, accessor);
			return result;
		}

		/// <summary>
		/// Delete Coupon Use User For Return Exchange
		/// </summary>
		/// <param name="returnOrder">Return Order</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理結果</returns>
		private static bool DeleteCouponUseUserForReturnExchange(Order returnOrder, SqlAccessor accessor)
		{
			var result = DeleteCouponUseUser(
				returnOrder.Coupon.CouponId,
				returnOrder.UserId,
				returnOrder.Owner.OwnerMailAddr,
				accessor);

			return result;
		}

		/// <summary>
		/// クーポン利用ユーザー情報削除
		/// </summary>
		/// <param name="couponId">クーポンID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理結果</returns>
		public static bool DeleteCouponUseUser(string couponId, string userId, string mailAddr, SqlAccessor accessor)
		{
			var couponUseUser = (Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE == Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
				? mailAddr
				: userId;
			var result = (new CouponService().DeleteCouponUseUser(couponId, couponUseUser, accessor) > 0);
			return result;
		}

		/// <summary>
		/// Insert User Coupon History For Return Exchange
		/// </summary>
		/// <param name="returnOrder">Return Order</param>
		/// <param name="historyKbn">履歴区分</param>
		/// <param name="actionKbn">アクション区分</param>
		/// <param name="couponInc">加算数</param>
		/// <param name="couponPrice">クーポン金額</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理結果</returns>
		private static bool InsertUserCouponHistoryForReturnExchange(
			Order returnOrder,
			string historyKbn,
			string actionKbn,
			int couponInc,
			decimal couponPrice,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			var result = InsertUserCouponHistory(
				returnOrder.UserId,
				returnOrder.Coupon.DeptId,
				returnOrder.Coupon.CouponId,
				returnOrder.Coupon.CouponCode,
				returnOrder.OrderId,
				historyKbn,
				actionKbn,
				couponInc,
				couponPrice,
				loginOperatorName,
				accessor);

			return result;
		}

		/// <summary>
		/// ユーザークーポン履歴登録
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponCode">クーポンコード</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="historyKbn">履歴区分</param>
		/// <param name="actionKbn">アクション区分</param>
		/// <param name="couponInc">加算数</param>
		/// <param name="couponPrice">クーポン金額</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理結果</returns>
		public static bool InsertUserCouponHistory(
			string userId,
			string deptId,
			string couponId,
			string couponCode,
			string orderId,
			string historyKbn,
			string actionKbn,
			int couponInc,
			decimal couponPrice,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			var couponHistory = new UserCouponHistoryModel
			{
				UserId = userId,
				DeptId = deptId,
				CouponId = couponId,
				CouponCode = couponCode,
				OrderId = orderId,
				HistoryKbn = historyKbn,
				ActionKbn = actionKbn,
				CouponInc = couponInc,
				CouponPrice = couponPrice,
				LastChanged = loginOperatorName
			};
			var result = new CouponService().InsertUserCouponHistory(couponHistory, accessor);
			return (result > 0);
		}

		/// <summary>
		/// Update Point For Return Exchange
		/// </summary>
		/// <param name="orderNew">Order New</param>
		/// <param name="orderId">Order id</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">Accessor</param>
		/// <param name="errorMessage">out エラーメッセージ</param>
		/// <returns>Update point result</returns>
		public static string UpdatePointForReturnExchange(
			Hashtable orderNew,
			string orderId,
			string userId,
			UpdateHistoryAction updateHistoryAction,
			string loginOperatorName,
			SqlAccessor accessor,
			out string errorMessage)
		{
			var transaction = string.Empty;
			errorMessage = string.Empty;
			var pointService = new PointService();
			try
			{
				if (Constants.W2MP_POINT_OPTION_ENABLED)
				{
					transaction = "1-4-1.返品交換時の付与ポイント調整（仮ポイント）のUPDATE処理";
					// 返品交換時の付与ポイント調整（仮ポイント）
					foreach (var orderPointAddTempList in ((List<List<Hashtable>>)orderNew[CONST_ORDER_POINT_ADD_TEMP]))
					{
						foreach (var orderPointAddTemp in orderPointAddTempList)
						{
							// 0なら更新しない
							if ((decimal)orderPointAddTemp[CONST_ORDER_POINT_ADD_ADJUSTMENT] != 0)
							{
								pointService.AdjustOrderPointAddForPointTemp(
									Constants.CONST_DEFAULT_DEPT_ID,
									StringUtility.ToEmpty(orderPointAddTemp[Constants.FIELD_USERPOINT_USER_ID]),
									int.Parse(StringUtility.ToEmpty(orderPointAddTemp[Constants.FIELD_USERPOINT_POINT_KBN_NO])),
									orderId,
									(decimal)(orderPointAddTemp[CONST_ORDER_POINT_ADD_ADJUSTMENT]),
									loginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									accessor);
							}
						}
					}

					// 返品交換時の付与ポイント調整（本ポイント）
					if ((orderNew[CONST_ORDER_BASE_POINT_ADD_COMP] != null)
						|| (orderNew[CONST_ORDER_LIMIT_POINT_ADD_COMP] != null))
					{
						transaction = "1-4-2.返品交換時の付与ポイント調整（本ポイント用）のUPDATE処理";

						var orderBasePointAddAdjustment = 0m;
						var lastOrderBasePointAdd = 0m;
						if (orderNew[CONST_ORDER_BASE_POINT_ADD_COMP] != null)
						{
							var orderBasePointAddComp = (Hashtable)orderNew[CONST_ORDER_BASE_POINT_ADD_COMP];
							orderBasePointAddAdjustment =
								(decimal)(orderBasePointAddComp[CONST_ORDER_POINT_ADD_ADJUSTMENT]);
							lastOrderBasePointAdd = (decimal)(orderBasePointAddComp[Constants.FIELD_USERPOINT_POINT]);
						}

						var orderLimitPointAddAdjustment = 0m;
						var lastOrderLimitPointAdd = 0m;
						if (orderNew[CONST_ORDER_LIMIT_POINT_ADD_COMP] != null)
						{
							var orderLimitPointAddComp = (Hashtable)orderNew[CONST_ORDER_LIMIT_POINT_ADD_COMP];
							orderLimitPointAddAdjustment =
								(decimal)(orderLimitPointAddComp[CONST_ORDER_POINT_ADD_ADJUSTMENT]);
							lastOrderLimitPointAdd = (decimal)(orderLimitPointAddComp[Constants.FIELD_USERPOINT_POINT]);
						}

						if ((orderBasePointAddAdjustment != 0)
							|| (orderLimitPointAddAdjustment != 0))
						{
							var baseUpdateCount = 0;
							var basePointErrorMessage = pointService.AdjustOrderBasePointAddForPointComp(
								Constants.CONST_DEFAULT_DEPT_ID,
								userId,
								orderId,
								orderBasePointAddAdjustment,
								CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_RETURN_EXCHANGE_ADD_BASE_POINT_ERROR),
								loginOperatorName,
								accessor,
								out baseUpdateCount);

							var limitUpdateCount = 0;
							var limitPointErrorMessage = pointService.AdjustOrderLimitPointAddForPointComp(
								Constants.CONST_DEFAULT_DEPT_ID,
								userId,
								orderId,
								orderLimitPointAddAdjustment,
								CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_RETURN_EXCHANGE_ADD_LIMIT_POINT_ERROR),
								loginOperatorName,
								accessor,
								out limitUpdateCount);

							if (string.IsNullOrEmpty(basePointErrorMessage + limitPointErrorMessage) == false)
							{
								errorMessage = basePointErrorMessage + limitPointErrorMessage;
								return transaction;
							}

							if ((baseUpdateCount + limitUpdateCount) > 0)
							{
								// 更新履歴登録
								if (updateHistoryAction == UpdateHistoryAction.Insert)
								{
									new UpdateHistoryService().InsertForUser(userId, loginOperatorName, accessor);
								}
							}

							// 注文の付与ポイントの補正
							new OrderService().AdjustAddPoint(
								orderId,
								(lastOrderBasePointAdd + lastOrderLimitPointAdd),
								loginOperatorName,
								UpdateHistoryAction.DoNotInsert,
								accessor);
						}
					}

					// 返品交換時の利用ポイント調整（本ポイント）
					if (((decimal)orderNew[CONST_ORDER_ORDER_POINT_USE_ADJUSTMENT] != 0))
					{
						transaction = "1-4-3.返品交換時の利用ポイント再計算処理";

						// HACK:コード重複してる
						if (((decimal)orderNew[CONST_ORDER_ORDER_POINT_USE_ADJUSTMENT] < 0))
						{
							// 利用ポイントを減らした場合は減らした分を本ポイントに戻す為、専用のメソッドを呼ぶ
							pointService.RecalcOrderUsePointForReturnExchange(
								StringUtility.ToEmpty(orderNew[Constants.FIELD_ORDER_USER_ID]),
								orderId,
								orderId,
								(decimal)orderNew[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE],
								loginOperatorName,
								updateHistoryAction,
								accessor);
						}
						else
						{
							// 利用ポイントを増やした場合は通常の計算処理を行う
							pointService.RecalcOrderUsePoint(
								StringUtility.ToEmpty(orderNew[Constants.FIELD_ORDER_USER_ID]),
								orderId,
								orderId,
								(decimal)orderNew[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE],
								loginOperatorName,
								updateHistoryAction,
								accessor);
						}
					}

					// 更新履歴登録
					if (updateHistoryAction == UpdateHistoryAction.Insert)
					{
						new UpdateHistoryService().InsertForUser(
							StringUtility.ToEmpty(orderNew[Constants.FIELD_ORDER_USER_ID]),
							loginOperatorName,
							accessor);
					}
				}
			}
			catch
			{
				return transaction;
			}

			return string.Empty;
		}

		/// <summary>
		/// 関連注文更新
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="orderNew">Order New</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void UpdateRelatedOrder(
			string orderId,
			Hashtable orderNew,
			UpdateHistoryAction updateHistoryAction,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			new OrderService().UpdateRelatedOrdersLastAmount(
				orderId,
				(string)orderNew[Constants.FIELD_ORDER_ORDER_ID],
				decimal.Parse(StringUtility.ToEmpty(orderNew[Constants.FIELD_ORDER_LAST_BILLED_AMOUNT])),
				decimal.Parse(StringUtility.ToEmpty(orderNew[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE])),
				decimal.Parse(StringUtility.ToEmpty(orderNew[Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN])),
				loginOperatorName,
				updateHistoryAction,
				accessor);
		}

		/// <summary>
		/// Regist Order Shipping For Return Exchange
		/// </summary>
		/// <param name="order">Order input</param>
		/// <param name="orderOld">Order old</param>
		/// <param name="orderShippingNo">Order Shipping No</param>
		/// <param name="accessor">Accessor</param>
		public static void RegistOrderShippingForReturnExchange(Hashtable order, OrderModel orderOld, string orderShippingNo, SqlAccessor accessor)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDERSHIPPING_ORDER_ID, order[Constants.FIELD_ORDER_ORDER_ID] },
				{ Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO, int.Parse(StringUtility.ToEmpty(orderShippingNo)) },
				{ Constants.FIELD_ORDER_ORDER_ID_ORG, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID_ORG]) },
				{ "order_shipping_no_org", orderShippingNo },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE, null },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME, string.Empty },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG, orderOld.Shippings[0].ShippingReceivingStoreFlg },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID, orderOld.Shippings[0].ShippingReceivingStoreId },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_EXTERNAL_DELIVERTY_STATUS, orderOld.Shippings[0].ShippingExternalDelivertyStatus },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS, orderOld.Shippings[0].ShippingStatus },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_UPDATE_DATE, orderOld.Shippings[0].ShippingStatusUpdateDate },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_MAIL_DATE, orderOld.Shippings[0].ShippingReceivingMailDate },
				{ Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE, string.Empty },
				{ Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID, orderOld.Shippings[0].StorePickupRealShopId },
			};
			new OrderService().InsertOrderShippingForOrderReturnExchange(input, accessor);
		}

		/// <summary>
		/// Calculate Cart For Return Exchange
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="returnOrder">Return order</param>
		/// <param name="user">User</param>
		public static void CalculateCartForReturnExchange(CartObject cart, Order returnOrder, UserModel user)
		{
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
			{
				// 定期会員判定セット
				cart.IsFixedPurchaseMember = (user.FixedPurchaseMemberFlg == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON);

				// 定期購入情報セット
				if (returnOrder.IsFixedPurchaseOrder)
				{
					// 定期購入回数を注文情報の「定期購入回数(注文時点)-1」に変更
					// ※cart.Calculateで+1しているため、-1している
					var fixedPurchase = new FixedPurchaseService().Get(returnOrder.FixedPurchaseId);
					fixedPurchase.OrderCount = returnOrder.FixedPurchaseOrderCount.Value - 1;

					// 定期商品購入も同様に変更
					foreach (var item in fixedPurchase.Shippings[0].Items)
					{
						var returnExchangeOrder = returnOrder.Items
							.FirstOrDefault(product => ((product.ProductId == item.ProductId)
								&& (product.VariationId == item.VariationId)));
						var returnExchangeOrderItemCount = (returnExchangeOrder == null)
							? 0
							: (returnExchangeOrder.FixedPurchaseItemOrderCount ?? 0);
						item.ItemOrderCount = returnExchangeOrderItemCount - 1;
					}
					cart.FixedPurchase = fixedPurchase;
				}
			}

			cart.EnteredShippingPrice = decimal.Parse(returnOrder.OrderPriceShipping);
			cart.EnteredPaymentPrice = decimal.Parse(returnOrder.OrderPriceExchange);

			// 再計算
			cart.Calculate(false);

			// 決済種別情報セット
			// ※元注文の決済手数料を引き継ぐ
			var payment = new CartPayment()
			{
				PaymentId = returnOrder.OrderPaymentKbn,
				PaymentName = returnOrder.PaymentName,
				PriceExchange = decimal.Parse(returnOrder.OrderPriceExchange)
			};
			cart.Payment = payment;

			// 配送先情報セット
			// ※元注文の配送料＆配送方法＆運送会社を引き継ぐ
			var shipping = new CartShipping(cart);
			shipping.PriceShipping = decimal.Parse(returnOrder.OrderPriceShipping);
			cart.SetShippingAddressAndShippingDateTime(shipping);

			// セットプロモーションで配送料無料、決済手数料が無料の場合が適用されている場合、各金額をセットする
			// ※元注文の決済手数料と配送料を引き継ぐ必要があるため、ここでセットしている
			if (cart.SetPromotions.IsPaymentChargeFree)
			{
				cart.SetPromotions.PaymentChargeDiscountAmount = cart.Payment.PriceExchange;
			}
			if (cart.SetPromotions.IsShippingChargeFree)
			{
				cart.SetPromotions.ShippingChargeDiscountAmount = cart.Shippings[0].PriceShipping;
			}

			// 初回購入ポイントセット
			// ※cart.Calculateでセットされないため
			if (Constants.W2MP_POINT_OPTION_ENABLED
				&& UserService.IsUser(user.UserKbn)
				&& cart.IsOrderGrantablePoint)
			{
				if (DomainFacade.Instance.OrderService.CheckOrderFirstBuy(cart.CartUserId, returnOrder.OrderId))
				{
					string firstBuyPointKbnTmp = null;
					cart.FirstBuyPoint = PointOptionUtility.GetOrderPointAdd(cart, Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY, out firstBuyPointKbnTmp, cart.FixedPurchase);
				}
			}
		}

		/// <summary>
		/// Regist Return Order Item For Return Exchange
		/// </summary>
		/// <param name="order">Order input</param>
		/// <param name="orderItemNo">Order item no</param>
		/// <param name="orderShippingNo">Order shipping no</param>
		/// <param name="orderSetPromotionItemNoList">Order set promotion item no list</param>
		/// <param name="returnOrderItem">Return Order Item Info</param>
		/// <param name="loginOperatorName">ログインオペレーター名</param>
		/// <param name="accessor">Accessor</param>
		public static void RegistReturnOrderItemForReturnExchange(
			Hashtable order,
			int orderItemNo,
			string orderShippingNo,
			Dictionary<string, int> orderSetPromotionItemNoList,
			ReturnOrderItem returnOrderItem,
			string loginOperatorName,
			SqlAccessor accessor)
		{
			var orderItem = new Hashtable
			{
				{ Constants.FIELD_ORDERITEM_ORDER_ID, order[Constants.FIELD_ORDER_ORDER_ID] },
				{ Constants.FIELD_ORDERITEM_ORDER_ITEM_NO, orderItemNo },
				{ Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO, int.Parse(orderShippingNo) },
				{ Constants.FIELD_ORDERITEM_SHOP_ID, returnOrderItem.ShopId },
				{ Constants.FIELD_ORDERITEM_PRODUCT_ID, returnOrderItem.ProductId },
				{ Constants.FIELD_ORDERITEM_VARIATION_ID,returnOrderItem.VariationId },
				{ Constants.FIELD_ORDERITEM_SUPPLIER_ID, returnOrderItem.SupplierId },
				{ Constants.FIELD_ORDERITEM_PRODUCT_NAME, returnOrderItem.ProductName },
				{ Constants.FIELD_ORDERITEM_PRODUCT_NAME_KANA, returnOrderItem.ProductNameKana },
				{ Constants.FIELD_ORDERITEM_PRODUCT_PRICE, returnOrderItem.ProductPrice - returnOrderItem.OptionPrice },
				{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, returnOrderItem.ItemQuantity },
				{ Constants.FIELD_ORDERITEM_PRODUCTSALE_ID, returnOrderItem.ProductSaleId },
				{ Constants.FIELD_ORDERITEM_NOVELTY_ID, returnOrderItem.NoveltyId },
				{ Constants.FIELD_ORDERITEM_RECOMMEND_ID, returnOrderItem.RecommendId },
				{ Constants.FIELD_ORDERITEM_ITEM_PRICE, returnOrderItem.ItemPrice },
				{ Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX, returnOrderItem.ItemPriceTax },
				{ Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN, returnOrderItem.ItemReturnExchangeKbn },
				{ Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS, returnOrderItem.ProductOptionValue },
				{ Constants.FIELD_ORDERITEM_PRODUCT_PRICE_PRETAX, returnOrderItem.ProductPricePretax - returnOrderItem.OptionPricePretax },
				{ Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG, returnOrderItem.ProductTaxIncludedFlg },
				{ Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE, returnOrderItem.ProductTaxRate },
				{ Constants.FIELD_ORDERITEM_PRODUCT_TAX_ROUND_TYPE, returnOrderItem.ProductTaxRoundType },
				{ Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG, returnOrderItem.FixedPurchaseProductFlg },
				{ Constants.FIELD_ORDERITEM_PRODUCT_BUNDLE_ID, returnOrderItem.ProductBundleId },
				{ Constants.FIELD_ORDERITEM_DISCOUNTED_PRICE, returnOrderItem.DiscountedPrice },
				{ Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID, returnOrderItem.SubscriptionBoxCourseId },
				{ Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT, returnOrderItem.SubscriptionBoxFixedAmount },
			};
			if (string.IsNullOrEmpty(returnOrderItem.OrderSetPromotionNo) == false)
			{
				orderItem.Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, int.Parse(returnOrderItem.OrderSetPromotionNo));
				orderItem.Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO, orderSetPromotionItemNoList[returnOrderItem.OrderSetPromotionNo]);
				orderSetPromotionItemNoList[returnOrderItem.OrderSetPromotionNo]++;
			}
			else
			{
				orderItem.Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, null);
				orderItem.Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO, null);
			}

			new OrderService().InsertOrderItemForOrderReturnExchange(orderItem, accessor);
		}

		/// <summary>
		/// Return Global Owner Address
		/// </summary>
		/// <param name="orderInfo">Order info</param>
		/// <param name="returnOrder">Return Order</param>
		/// <returns>Global Owner Address Info</returns>
		public static Hashtable ReturnGlobalOwnerAddress(Hashtable orderInfo, Order returnOrder)
		{
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				orderInfo[Constants.FIELD_ORDEROWNER_ACCESS_COUNTRY_ISO_CODE] = returnOrder.Owner.AccessCountryIsoCode;
				orderInfo[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_CODE] = returnOrder.Owner.DispLanguageCode;
				orderInfo[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID] = returnOrder.Owner.DispLanguageLocaleId;
				orderInfo[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_CODE] = returnOrder.Owner.DispCurrencyCode;
				orderInfo[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID] = returnOrder.Owner.DispCurrencyLocaleId;
			}

			return orderInfo;
		}

		/// <summary>
		/// 注文返金ステータス更新ステートメント取得
		/// </summary>
		/// <param name="status">ステータス値</param>
		/// <returns>ステータス更新ステートメント</returns>
		public static string GetUpdateOrderRepaymentStatusStatement(string status)
		{
			switch (status)
			{
				// 返金無し
				// 未返金
				case Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_NOREPAYMENT:
					return "UpdateOrderRepaymentStatusNoRepayment";

				// 返金済み
				case Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_CONFIRM:
					return "UpdateOrderRepaymentStatusConfrim";

				case Constants.FLG_ORDER_ORDER_REPAYMENT_STATUS_COMPLETE:
					return "UpdateOrderRepaymentStatusComplete";

				default:
					return null;
			}
		}

		/// <summary>
		/// 注文商品実在庫の出荷を行う(出荷する商品数分、実在庫、引当済実在庫数を減算する)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="loginOperatorName">ログインオペレータ名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static ResultKbn UpdateOrderItemRealStockShipped(
			string orderId,
			string shopId,
			string loginOperatorName,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var shippedOnce = false;
			var shippedAll = true;

			// トランザクション中のテーブル更新順を守るため
			//  商品在庫更新前に該当注文情報の更新ロックを取得
			GetUpdlockFromOrderTables(orderId, accessor);

			// 実在庫出荷処理
			// 注文商品数分ループ
			foreach (DataRowView orderItem in GetOrderItems(orderId, shopId, accessor))
			{
				var iItemRealStockReserved = ((int)orderItem[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED] * -1);
				var iItemRealStockShipped = (int)orderItem[Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED];

				// 返品商品の場合はスキップ
				if ((string)orderItem[Constants.FIELD_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN] == Constants.FLG_ORDERITEM_ITEM_RETURN_EXCHANGE_KBN_RETURN)
				{
					continue;
				}
				// 在庫引当されていない場合はスキップ
				if (iItemRealStockReserved == 0)
				{
					shippedAll = false;
					continue;
				}

				// 商品在庫情報セット
				var htInput = new Hashtable
				{
					{ Constants.FIELD_PRODUCTSTOCK_SHOP_ID, orderItem[Constants.FIELD_ORDERITEM_SHOP_ID] },
					{ Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID, orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID] },
					{ Constants.FIELD_PRODUCTSTOCK_VARIATION_ID, orderItem[Constants.FIELD_ORDERITEM_VARIATION_ID] },
					{ Constants.FIELD_PRODUCTSTOCK_STOCK, 0 },
					{ Constants.FIELD_PRODUCTSTOCK_REALSTOCK, iItemRealStockReserved },
					{ Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B, 0 },
					{ Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C, 0 },
					{ Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED, iItemRealStockReserved },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID, orderItem[Constants.FIELD_ORDERITEM_ORDER_ID] },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_STOCK, 0 },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK, iItemRealStockReserved },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_B, 0 },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_C, 0 },
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_ADD_REALSTOCK_RESERVED, iItemRealStockReserved },
					{
						Constants.FIELD_PRODUCTSTOCKHISTORY_ACTION_STATUS,
						Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_FORWARD
					},
					{ Constants.FIELD_PRODUCTSTOCKHISTORY_UPDATE_MEMO, "" },
					{ Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS, Constants.FLG_ORDER_ORDER_SHIPPED_STATUS_SHIPPED },
					{ Constants.FIELD_ORDERITEM_ORDER_ITEM_NO, orderItem[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO] },
					{ Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_SHIPPED, iItemRealStockShipped },
					{ Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED, loginOperatorName }
				};

				// 注文情報更新
				using (var statement = new SqlStatement("Order", "UpdateOrderShippedStatus"))
				{
					statement.ExecStatement(accessor, htInput);
				}

				// 注文商品情報更新
				using (var statement = new SqlStatement("Order", "UpdateItemRealStockShipped"))
				{
					statement.ExecStatement(accessor, htInput);
				}

				// 最終更新日時更新
				new OrderService().UpdateOrderDateChangedByOrderId((string)htInput[Constants.FIELD_PRODUCTSTOCKHISTORY_ORDER_ID], accessor);

				// 実在庫更新
				using (var statement = new SqlStatement("ProductStock", "AddProductStock"))
				{
					statement.ExecStatement(accessor, htInput);
				}

				// 商品在庫履歴情報追加
				using (var statement = new SqlStatement("ProductStock", "InsertProductStockHistory"))
				{
					statement.ExecStatement(accessor, htInput);
				}

				shippedOnce = true;
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, loginOperatorName, accessor);
			}

			// 結果を返す
			if (shippedAll) return ResultKbn.UpdateOK;

			return shippedOnce ? ResultKbn.UpdatePart : ResultKbn.UpdateNG;
		}

		/// <summary>
		/// 再与信可否チェック
		/// </summary>
		/// <param name="paymentKbn">注文決済区分</param>
		/// <returns>true: 再与信可能、false: 再与信不可</returns>
		public static bool CheckCanPaymentReauth(string paymentKbn)
		{
			return (((paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					 && Constants.REAUTH_COMPLETE_CREDITCARD_LIST.Contains(Constants.PAYMENT_CARD_KBN))
					|| ((paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
						&& Constants.REAUTH_COMPLETE_CVSDEF_LIST.Contains(Constants.PAYMENT_CVS_DEF_KBN))
					|| ((paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT))
					|| (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					|| (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					|| (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
					|| (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					|| (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY));
		}

		/// <summary>
		/// 更新対象のユーザーポイント情報が仮ポイントかチェック
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderId">注文ID</param>
		/// <returns>
		/// true:仮ポイントあり
		/// false:仮ポイントなし
		/// </returns>
		public static bool CheckPointTypeTemp(string userId, string orderId)
		{
			return (GetUserPointTemp(userId, orderId).Any());
		}

		/// <summary>
		/// 注文のサイト区分には再与信可否チェック
		/// </summary>
		/// <param name="orderSiteKbn">注文のサイト区分</param>
		/// <returns>true: 再与信可能、false: 再与信不可</returns>
		public static bool CheckCanOrderSiteKbnReauth(string orderSiteKbn)
		{
			return Constants.PAYMENT_REAUTH_ORDER_SITE_KBN.Contains(orderSiteKbn);
		}

		/// <summary>
		/// Invoice Released
		/// </summary>
		/// <param name="order">Order Info</param>
		public static bool InvoiceReleased(Hashtable order)
		{
			var invoiceNo = new TwInvoiceService().GetInvoiceNoForOrder(DateTime.Now.Date.ToString());
			if (string.IsNullOrEmpty(invoiceNo)) return false;

			order[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_NO] = invoiceNo;

			new TwOrderInvoiceService().UpdateInvoiceNoByOrderId(invoiceNo, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]));

			return true;
		}

		/// <summary>
		/// Ec Pay invoice released
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="shipping">Cart shipping</param>
		/// <param name="orderShippingNo">Order shipping no</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="sqlAccessor">The sql accessorder</param>
		/// <returns>Error message</returns>
		public static string EcPayInvoiceReleased(
			string orderId,
			CartShipping shipping,
			int orderShippingNo,
			string lastChanged,
			SqlAccessor sqlAccessor = null)
		{
			if ((Constants.TWINVOICE_ECPAY_ENABLED == false)
				|| (shipping.IsShippingAddrTw == false))
			{
				return "Invoice Released Error!";
			}

			var orderInfo = new OrderService().GetOrderInfoByOrderId(orderId);
			var orderInvoice = new TwOrderInvoiceService().GetOrderInvoice(orderId, orderShippingNo);

			if ((shipping.UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
				&& (shipping.CarryType == Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE))
			{
				var checkBarcodeRequest = new TwInvoiceEcPayApi().CreateRequestObject(
					TwInvoiceEcPayApi.ExecuteTypes.CheckBarcode,
					orderInfo,
					orderInvoice);
				var checkBarcodeResponse = new TwInvoiceEcPayApi().ReceiveResponseObject(
					TwInvoiceEcPayApi.ExecuteTypes.CheckBarcode,
					checkBarcodeRequest);
				var errorMessage = GetEcPayInvoiceMessage(checkBarcodeResponse, true, checkBarcodeResponse.IsSuccess);
				if (errorMessage != string.Empty) return errorMessage;
			}

			if (shipping.UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_DONATE)
			{
				var checkLoveCodeRequest = new TwInvoiceEcPayApi().CreateRequestObject(
					TwInvoiceEcPayApi.ExecuteTypes.CheckLoveCode,
					orderInfo,
					orderInvoice);
				var checkLoveCodeResponse = new TwInvoiceEcPayApi().ReceiveResponseObject(
					TwInvoiceEcPayApi.ExecuteTypes.CheckLoveCode,
					checkLoveCodeRequest);
				var errorMessage = GetEcPayInvoiceMessage(checkLoveCodeResponse, false, checkLoveCodeResponse.IsSuccess);
				if (errorMessage != string.Empty) return errorMessage;
			}

			var issueRequest = new TwInvoiceEcPayApi().CreateRequestObject(
				TwInvoiceEcPayApi.ExecuteTypes.Issue,
				orderInfo,
				orderInvoice);
			var issueResponse = new TwInvoiceEcPayApi().ReceiveResponseObject(
				TwInvoiceEcPayApi.ExecuteTypes.Issue,
				issueRequest);

			if (issueResponse.IsSuccess == false)
			{
				orderInvoice.TwInvoiceNo = issueResponse.Response.Data.InvoiceNo ?? string.Empty;
				orderInvoice.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_NOT_ISSUED;
				new TwOrderInvoiceService().UpdateTwOrderInvoiceForModify(
					orderInvoice,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					sqlAccessor);

				// Send Mail
				var input = new Hashtable()
				{
					{ TwInvoiceEcPayApi.MESSAGESETTING_KEY_RESULT, issueResponse.IsSuccess ? "成功" : "失敗" },
					{ TwInvoiceEcPayApi.MESSAGESETTING_KEY_MESSAGE, issueResponse.Message },
				};

				using (var mailUtil = new MailSendUtility(
					Constants.CONST_DEFAULT_SHOP_ID,
					Constants.CONST_MAIL_ID_SEND_OPERATOR_FROM_TWINVOICE_ECPAY,
					string.Empty,
					input,
					true,
					Constants.MailSendMethod.Manual))
				{
					if (mailUtil.SendMail() == false)
					{
						FileLogger.WriteError(mailUtil.MailSendException);
					}
				}
				return issueResponse.Message;
			}

			orderInvoice.TwInvoiceDate = string.IsNullOrEmpty(issueResponse.Response.Data.InvoiceDate)
				? null
				: (DateTime?)DateTime.Parse(issueResponse.Response.Data.InvoiceDate);
			orderInvoice.TwInvoiceNo = issueResponse.Response.Data.InvoiceNo ?? string.Empty;
			orderInvoice.TwInvoiceStatus = Constants.FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED;
			new TwOrderInvoiceService().UpdateTwOrderInvoiceForModify(
				orderInvoice,
				lastChanged,
				UpdateHistoryAction.DoNotInsert,
				sqlAccessor);
			return string.Empty;
		}

		/// <summary>
		/// Get Ec Pay invoice message
		/// </summary>
		/// <param name="response">Response</param>
		/// <param name="isMobile">Is mobile</param>
		/// <param name="isSuccess">Is success</param>
		/// <returns>Error message</returns>
		private static string GetEcPayInvoiceMessage(
			TwInvoiceEcPayApi.TwInvoiceEcPayResponse response,
			bool isMobile,
			bool isSuccess)
		{
			var errorMessage = string.Empty;

			// Data code not exist
			if (isSuccess
				&& (response.Response.Data.IsExist != TwInvoiceEcPayApi.FLG_IS_EXIST))
			{
				errorMessage = isMobile
					? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MOBILE_CODE_NOT_EXIST)
					: CommerceMessages.GetMessages(CommerceMessages.ERRMSG_DONATION_CODE_NOT_EXIST);
			}
			// EcPay invoice system maintenance error
			else if ((isSuccess == false)
				&& (response.Response.Data.RtnCode
					== TwInvoiceEcPayApi.RETURN_CODE_ECPAY_INVOICE_SYSTEM_MAINTENANCE))
			{
				errorMessage = CommerceMessages.GetMessages(
					CommerceMessages.ERRMSG_ECPAY_INVOICE_SYSTEM_MAINTENANCE);
			}
			return errorMessage;
		}

		/// <summary>
		/// Check Uniform Type
		/// </summary>
		/// <param name="uniformType">uniformType</param>
		/// <param name="isPersonal">isPersonal</param>
		/// <param name="isDonate">isDonate</param>
		/// <param name="isCompany">isCompany</param>
		public static void CheckUniformType(
			string uniformType,
			ref bool isPersonal,
			ref bool isDonate,
			ref bool isCompany)
		{
			switch (uniformType)
			{
				case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
					isPersonal = true;
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
					isCompany = true;
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
					isDonate = true;
					break;

				default:
					break;
			}
		}

		/// <summary>
		/// Check Carry Type
		/// </summary>
		/// <param name="carryType">carryType</param>
		/// <param name="isMobile">isMobile</param>
		/// <param name="isCertificate">isCertificate</param>
		/// <param name="isNoCarryType">isNoCarryType</param>
		public static void CheckCarryType(
			string carryType,
			ref bool isMobile,
			ref bool isCertificate,
			ref bool isNoCarryType)
		{
			switch (carryType)
			{
				case "":
					isNoCarryType = true;
					break;

				case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
					isMobile = true;
					break;

				case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
					isCertificate = true;
					break;

				default:
					break;
			}
		}

		/// <summary>
		/// Check Quantity Existence
		/// </summary>
		/// <returns>Quantity Exist</returns>
		public static decimal CheckQuantityExistence()
		{
			var twInvoiceSercice = new TwInvoiceService();
			var twInvoices = twInvoiceSercice.GetListTwInvoice(DateTime.Now.Date.ToString());
			if (twInvoices == null) return 0m;

			var quantityExist = 0m;
			foreach (var item in twInvoices)
			{
				quantityExist += (item.TwInvoiceNo.HasValue)
					? (item.TwInvoiceNoEnd - item.TwInvoiceNo.Value)
					: (item.TwInvoiceNoEnd - item.TwInvoiceNoStart + 1);
			}

			return quantityExist;
		}

		/// <summary>
		/// 各注文・入金ステータス更新連絡メールを送信
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="mailSendMethod">メール送信方法</param>
		/// <returns>処理結果(成功:true 失敗:false)</returns>
		public static bool SendOrderMail(string orderId, string mailId, Constants.MailSendMethod mailSendMethod)
		{
			// 注文情報取得
			var order = new MailTemplateDataCreaterForOrder(true).GetOrderMailDatas(orderId);
			var orderMobile = new MailTemplateDataCreaterForOrder(false).GetOrderMailDatas(orderId);

			// メール送信
			if (order.Count != 0)
			{
				var sendPc = false;
				var sendMobile = false;
				if (Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED)
				{
					sendPc = ((string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] != "");
					sendMobile = ((string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2] != "");
				}
				else
				{
					if ((string)order[Constants.FIELD_ORDER_ORDER_KBN] != Constants.FLG_ORDER_ORDER_KBN_MOBILE)
					{
						if ((string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] != "")
						{
							sendPc = true;
						}
						else if ((string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2] != "")
						{
							sendMobile = true;
						}
					}
					else if ((string)order[Constants.FIELD_ORDER_ORDER_KBN] == Constants.FLG_ORDER_ORDER_KBN_MOBILE)
					{
						if ((string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2] != "")
						{
							sendMobile = true;
						}
						else if ((string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR] != "")
						{
							sendPc = true;
						}
					}
				}

				string languageCode = null;
				string languageLocaleId = null;
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					languageCode = (string)order[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_CODE];
					languageLocaleId = (string)order[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID];
				}

				// PCメール送信
				if (sendPc)
				{
					using (MailSendUtility msMailSend = new MailSendUtility((string)order[Constants.FIELD_MAILTEMPLATE_SHOP_ID], mailId, (string)order[Constants.FIELD_ORDER_USER_ID], order, true, mailSendMethod, languageCode, languageLocaleId, (string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR]))
					{
						msMailSend.AddTo((string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR]);

						if (msMailSend.SendMail() == false)
						{
							AppLogger.WriteError(msMailSend.MailSendException.ToString());
							return false;
						}
					}
				}

				// モバイルメール送信
				if (sendMobile)
				{
					if (orderMobile.Count != 0)
					{
						using (MailSendUtility msMailSend = new MailSendUtility((string)order[Constants.FIELD_MAILTEMPLATE_SHOP_ID], mailId, (string)orderMobile[Constants.FIELD_ORDER_USER_ID], orderMobile, false, mailSendMethod, userMailAddress: (string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR]))
						{
							msMailSend.AddTo((string)order[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2]);

							if (msMailSend.SendMail() == false)
							{
								AppLogger.WriteError(msMailSend.MailSendException.ToString());
								return false;
							}
						}
					}
				}
			}

			return true;
		}

		/// <summary>
		/// 各注文・入金ステータス更新連絡メールを送信
		/// </summary>
		/// <param name="orderIds">オーダーID一覧</param>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="realShopId">実店舗ID</param>
		/// <param name="mailSendMethod">メール送信方法</param>
		/// <returns>処理結果(成功:true 失敗:false)</returns>
		public static bool SendMailAllOrderToRealShop(
			List<string> orderIds,
			string mailId,
			string realShopId,
			Constants.MailSendMethod mailSendMethod)
		{
			// Get information real shop
			var realShop = DomainFacade.Instance.RealShopService.Get(realShopId);
			var orderModels = DomainFacade.Instance.OrderService.GetAllForOrderMail();

			if ((realShop == null)
				|| string.IsNullOrEmpty(realShop.MailAddr)
				|| (realShop.ValidFlg != Constants.FLG_REALSHOP_VALID_FLG_VALID)
				|| (orderModels == null))
			{
				return false;
			}

			var storePickupOrderLoop = new List<Hashtable>();
			foreach (var orderId in orderIds)
			{
				var orderModel = orderModels.FirstOrDefault(model => model.OrderId == orderId);
				if (orderModel == null) continue;
				var orderItems = DomainFacade.Instance.OrderService
					.GetRelatedOrderItems(orderId)
					.Select(model => model.DataSource)
					.ToList();

				var storePickupLeadTime = orderModel.DataSource[Constants.FIELD_DELIVERYLEADTIME_SHIPPING_LEAD_TIME];
				var orderItemLoop = new MailTemplateDataCreaterForOrder(true)
					.GetOrderItemsLoopsForOrderMailTemplete(
						orderModel,
						orderItems,
						false);

				var storePickupOrder = new Hashtable
				{
					{ Constants.MAILTAG_STORE_PICKUP_ORDER_ID, orderId },
					{ Constants.MAILTAG_ORDER_ITEMS_LOOP, orderItemLoop },
					{ Constants.MAILTAG_STORE_PICKUP_LEAD_TIME, storePickupLeadTime },
				};

				storePickupOrderLoop.Add(storePickupOrder);
			}

			var input = new Hashtable
			{
				{ Constants.MAILTAG_STORE_PICKUP_SHOP_NAME, realShop.Name },
				{ Constants.MAILTAG_STORE_PICKUP_ORDER_COUNT, orderIds.Count },
				{ Constants.MAILTAG_STORE_PICKUP_STOREPICKUP_ORDER_LOOP, storePickupOrderLoop },
			};

			using (var mailSend = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				mailId,
				(string)input[Constants.FIELD_ORDER_USER_ID],
				input,
				true,
				mailSendMethod,
				userMailAddress: realShop.MailAddr))
			{
				mailSend.AddTo(realShop.MailAddr);

				if (mailSend.SendMail() == false)
				{
					AppLogger.WriteError(mailSend.MailSendException.ToString());
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Send mail store pick up information
		/// </summary>
		/// <param name="orderId">Order id</param>
		public static void SendMailStorePickUpInformation(string orderId)
		{
			var input = new MailTemplateDataCreaterForOrder(isPc: true).GetOrderMailDatas(orderId);

			using (var mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_FROM_REAL_SHOP,
				string.Empty,
				input))
			{
				mailSender.AddTo((string)input[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR]);

				if (mailSender.SendMail() == false)
				{
					FileLogger.WriteError(mailSender.MailSendException);
				}
			}
		}

		/// <summary>
		/// Check Id Exists In Xml Store
		/// </summary>
		/// <param name="keyStno">key STNO</param>
		/// <returns>Check or no</returns>
		public static bool CheckIdExistsInXmlStore(string keyStno)
		{
			var xmlTwPelicanAllCvs = XElement.Load(Path.Combine(
				Constants.PHYSICALDIRPATH_CONTENTS,
				Constants.CONVENIENCESTOREMAP_FILE_TWPELICANALLCVS));
			var listStno = xmlTwPelicanAllCvs.Elements("F01CONTENT")
				.Select(itemStno => itemStno.Element("STNO").Value);
			var result = listStno.Any(item => (item == keyStno));

			return result;
		}

		/// <summary>
		/// Check Id Exists In Xml Store
		/// </summary>
		/// <param name="keyStno">key STNO</param>
		/// <returns>Check or no</returns>
		public static bool CheckIdExistsInXmlStoreBatchFixedPurchase(string keyStno)
		{
			var filePath = Path.Combine(
				Constants.PHYSICALDIRPATH_CONTENTS_ROOT,
				Constants.PATH_CONTENTS,
				Constants.CONVENIENCESTOREMAP_FILE_TWPELICANALLCVS);
			var xmlTwPelicanAllCvs = XElement.Load(filePath);
			var listStno = xmlTwPelicanAllCvs.Elements("F01CONTENT")
				.Select(itemStno => itemStno.Element("STNO").Value).ToList();
			var result = listStno.Any(item => (item == keyStno));

			return result;
		}

		/// <summary>
		/// Check Valid Weight For Convenience Store
		/// </summary>
		/// <param name="cart">cart</param>
		/// <param name="shippingReceivingStoreType">Shipping receiving store type</param>
		/// <returns>Valid weight & price: true | Other: false </returns>
		public static bool CheckValidWeightAndPriceForConvenienceStore(CartObject cart, string shippingReceivingStoreType)
		{
			var weight = Convert.ToDouble(cart.Items.Sum(item => item.WeightSubTotal));
			var kiloGram = weight / 1000;
			var convenienceStoreLimitPrice = CurrencyManager.ConvertPrice(cart.PriceSubtotal);
			var convenienceStoreLimitWeight = GetConvenienceStoreLimitWeight(shippingReceivingStoreType);
			var checkKiloGram = (convenienceStoreLimitWeight > ECPayConstants.CONST_CONVENIENCE_STORE_LIMIT_WEIGHT)
				? (Convert.ToDecimal(kiloGram) > convenienceStoreLimitWeight)
				: (Convert.ToDecimal(kiloGram) >= convenienceStoreLimitWeight);
			var result =
				((convenienceStoreLimitPrice > CurrencyManager.ConvertPrice(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE))
					|| checkKiloGram);

			return result;
		}

		/// <summary>
		/// Create Convenience Store Map Url
		/// </summary>
		/// <param name="isSmartPhone">isSmartPhone</param>
		/// <returns>Convenience Store Map Url</returns>
		public static string CreateConvenienceStoreMapUrl(bool isSmartPhone = false)
		{
			var url = isSmartPhone
				? Constants.CONVENIENCESTOREMAP_SMARTPHONE_URL
				: Constants.CONVENIENCESTOREMAP_PC_URL;

			return new UrlCreator(url)
				.AddParam(Constants.REQUEST_KEY_RECEIVINGSTORE_TWPELICAN_SUID, Constants.RECEIVINGSTORE_TWPELICAN_CVSID)
				.AddParam(Constants.REQUEST_KEY_RECEIVINGSTORE_TWPELICAN_PROCESSID, Constants.RECEIVINGSTORE_TWPELICAN_CVSNAME)
				.AddParam(Constants.REQUEST_KEY_RECEIVINGSTORE_TWPELICAN_RTURL, Constants.CONVENIENCESTOREMAP_CVSTEMP)
				.CreateUrl();
		}

		/// <summary>
		/// Check Valid Tel No For Payment Atone And Aftee
		/// </summary>
		/// <param name="paymentId">payment Id</param>
		/// <param name="ownerTel">owner Tel</param>
		/// <returns>True: if valid Or False: if invalid</returns>
		public static bool CheckValidTelNoForPaymentAtoneAndAftee(string paymentId, string ownerTel)
		{
			var isValid = true;
			Regex regex = null;
			switch (paymentId)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
					regex = new Regex(@"([0]|[8][1]|[+][8][1])[7-9][0](\d{8}|-\d{4}-\d{4})");
					isValid = regex.IsMatch(ownerTel);
					return isValid;

				case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
					regex = new Regex(@"[0][9]\d{8}");
					isValid = regex.IsMatch(ownerTel);
					return isValid;

				default:
					return isValid;
			}
		}

		/// <summary>
		/// Create Data For Authorizing Atone Aftee
		/// </summary>
		/// <param name="cart">Cart Object</param>
		/// <param name="isAtone">Is Atone</param>
		/// <returns>Json Data Atone String</returns>
		public static string CreateDataForAuthorizingAtoneAftee(CartObject cart, bool isAtone)
		{
			var userAttribute = UserAttributeOrderInfoCalculator.GetInstance().Calculate(cart.CartUserId);
			var isPhoneNumber = CheckValidTelNoForPaymentAtoneAndAftee(cart.Payment.PaymentId, cart.Owner.Tel1);
			var customerObject = new Hashtable
			{
				{ CONST_CUSTOMMER_CUSTOMER_NAME, cart.Owner.Name },
				{ CONST_CUSTOMMER_CUSTOMER_FAMILY_NAME, cart.Owner.Name1 },
				{ CONST_CUSTOMMER_CUSTOMER_GIVEN_NAME, cart.Owner.Name2 },
				{ CONST_CUSTOMMER_CUSTOMER_NAME_KANA, cart.Owner.NameKana },
				{ CONST_CUSTOMMER_CUSTOMER_FAMILY_NAME_KANA, cart.Owner.NameKana1 },
				{ CONST_CUSTOMMER_CUSTOMER_GIVEN_NAME_KANA, cart.Owner.NameKana2 },
				{
					CONST_CUSTOMMER_PHONE_NUMBER,
					isPhoneNumber
						? cart.Owner.Tel1.Replace("-", string.Empty)
						: string.Empty
				},
				{ CONST_CUSTOMMER_SEX_DIVISION, (cart.Owner.Sex == Constants.FLG_USER_SEX_MALE) ? "1" : "2" },
				{ CONST_CUSTOMMER_COMPANY_NAME,cart.Owner.CompanyName },
				{ CONST_CUSTOMMER_DEPARTMENT, StringUtility.GetWithSpecifiedByteLength(cart.Owner.CompanyPostName, 0, 30, Encoding.UTF8) },
				{ CONST_CUSTOMMER_ZIP_CODE, cart.Owner.Zip },
				{ CONST_CUSTOMMER_ADDRESS, string.Format("{0}{1}{2}{3}", cart.Owner.Addr1, cart.Owner.Addr2, cart.Owner.Addr3, cart.Owner.Addr4) },
				{
					CONST_CUSTOMMER_TEL,
					isPhoneNumber == false
						? cart.Owner.Tel1.Replace("-", string.Empty)
						: string.Empty
				},
				{ CONST_CUSTOMMER_EMAIL, cart.Owner.MailAddr },
				{ CONST_CUSTOMMER_TOTAL_PURCHASE_COUNT, userAttribute.OrderCountOrderAll.ToString() },
				{ CONST_CUSTOMMER_TOTAL_PURCHASE_AMOUNT, userAttribute.OrderAmountOrderAll.ToString("0") },
			};

			if (cart.Owner.Birth.HasValue) customerObject.Add(CONST_CUSTOMMER_BIRTHDAY, cart.Owner.Birth.Value.ToString("yyyy-MM-dd"));

			var shippingTel = cart.Shippings[0].Tel1;
			var destCustomerObject = new
			{
				dest_customer_name = cart.Shippings[0].Name,
				dest_customer_name_kana = cart.Shippings[0].NameKana,
				dest_company_name = cart.Shippings[0].CompanyName,
				dest_department = StringUtility.GetWithSpecifiedByteLength(cart.Shippings[0].CompanyPostName, 0, 30, Encoding.UTF8),
				dest_zip_code = cart.Shippings[0].Zip,
				dest_address = string.Format("{0}{1}{2}{3}", cart.Shippings[0].Addr1, cart.Shippings[0].Addr2, cart.Shippings[0].Addr3, cart.Shippings[0].Addr4),
				dest_tel = string.IsNullOrEmpty(shippingTel)
					? string.Empty
					: Regex.Replace(shippingTel, @"[-]|[(]|[)]", string.Empty)
			};

			var checksum = string.Empty;
			IEnumerable<object> items = null;
			if (isAtone)
			{
				items = CreatedOrderItemsAtone(cart);

				checksum = cart.SettlementAmount.ToString("0")
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_ADDRESS])
					+ (cart.Owner.Birth.HasValue
						? StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_BIRTHDAY])
						: string.Empty)
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_COMPANY_NAME])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_CUSTOMER_FAMILY_NAME])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_CUSTOMER_FAMILY_NAME_KANA])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_CUSTOMER_GIVEN_NAME])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_CUSTOMER_GIVEN_NAME_KANA])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_CUSTOMER_NAME])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_CUSTOMER_NAME_KANA])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_DEPARTMENT])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_EMAIL])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_PHONE_NUMBER])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_SEX_DIVISION])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_TEL])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_TOTAL_PURCHASE_AMOUNT])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_TOTAL_PURCHASE_COUNT])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_ZIP_CODE])
					+ ((cart.UsePoint > 0) ? "ポイント利用あり" : string.Empty)
					+ destCustomerObject.dest_address
					+ destCustomerObject.dest_company_name
					+ destCustomerObject.dest_customer_name
					+ destCustomerObject.dest_customer_name_kana
					+ destCustomerObject.dest_department
					+ destCustomerObject.dest_tel
					+ destCustomerObject.dest_zip_code;

				foreach (AtoneCreatePaymentRequest.Item item in items)
				{
					checksum += item.ItemCount
						+ item.ItemName
						+ item.ItemPrice
						+ item.ShopItemId;
				}
			}
			else
			{
				items = CreatedOrderItemAftee(cart);

				checksum = cart.SettlementAmount.ToString("0")
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_ADDRESS])
					+ (cart.Owner.Birth.HasValue
						? StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_BIRTHDAY])
						: string.Empty)
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_COMPANY_NAME])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_CUSTOMER_FAMILY_NAME])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_CUSTOMER_GIVEN_NAME])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_CUSTOMER_NAME])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_DEPARTMENT])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_EMAIL])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_PHONE_NUMBER])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_SEX_DIVISION])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_TEL])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_TOTAL_PURCHASE_AMOUNT])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_TOTAL_PURCHASE_COUNT])
					+ StringUtility.ToEmpty(customerObject[CONST_CUSTOMMER_ZIP_CODE])
					+ ((cart.UsePoint > 0) ? "ポイント利用あり" : string.Empty)
					+ destCustomerObject.dest_address
					+ destCustomerObject.dest_company_name
					+ destCustomerObject.dest_customer_name
					+ destCustomerObject.dest_customer_name_kana
					+ destCustomerObject.dest_department
					+ destCustomerObject.dest_tel
					+ destCustomerObject.dest_zip_code;

				foreach (AfteeCreatePaymentRequest.Item item in items)
				{
					checksum += item.ItemCount
						+ item.ItemName
						+ item.ItemPrice
						+ item.ShopItemId;
				}
			}

			checksum = CreateChecksumAtoneOrAftee(cart.CartUserId, isAtone, checksum);

			var transactionOptions = CreateTransactionOptionsForRegistOrder(cart.HasFixedPurchase, isAtone, cart.IsDigitalContentsOnly);

			var data = new
			{
				amount = (int)cart.SettlementAmount,
				shop_transaction_no = cart.OrderId,
				user_no = cart.CartUserId,
				sales_settled = cart.IsOrderSalesSettled,
				transaction_options = transactionOptions,
				description_trans = ((cart.UsePoint > 0) ? "ポイント利用あり" : string.Empty),
				checksum = checksum,
				customer = customerObject,
				dest_customers = new[] { destCustomerObject },
				items = items.ToArray()
			};

			var returnData = JsonConvert.SerializeObject(new { data = data });
			if (isAtone)
			{
				AtonePaymentApiFacade.WriteRequestDataLog(
					Constants.PAYMENT_ATONE_SCRIPT_URL,
					returnData);
			}
			else
			{
				AfteePaymentApiFacade.WriteRequestDataLog(
					Constants.PAYMENT_AFTEE_SCRIPT_URL,
					returnData);
			}
			return returnData;
		}

		/// <summary>
		/// Create Transaction Options For Regist Order
		/// </summary>
		/// <param name="hasFixedPurchase">Has FixedPurchase</param>
		/// <param name="isAtone">Is Atone</param>
		/// <param name="hasDigitalContent">Has Digital Content</param>
		/// <returns>Transaction Options</returns>
		private static int[] CreateTransactionOptionsForRegistOrder(
			bool hasFixedPurchase,
			bool isAtone,
			bool hasDigitalContent)
		{
			var transactionOptions = new int[] { };
			var isOrderTempAtoneOrAftee = (isAtone)
				? Constants.PAYMENT_ATONE_TEMPORARYREGISTRATION_ENABLED
				: Constants.PAYMENT_AFTEE_TEMPORARYREGISTRATION_ENABLED;
			var isUpdateAmountAtoneOrAftee = (isAtone)
				? Constants.PAYMENT_ATONE_UPDATEAMOUNT_ENABLED
				: Constants.PAYMENT_AFTEE_UPDATEAMOUNT_ENABLED;

			if ((hasDigitalContent == false)
				&& isOrderTempAtoneOrAftee)
			{
				transactionOptions = isUpdateAmountAtoneOrAftee
					? hasFixedPurchase
						? new int[] { 1, 2, 3 }
						: new int[] { 2, 3 }
					: hasFixedPurchase
						? new int[] { 1, 3 }
						: new int[] { 3 };
			}
			else
			{
				transactionOptions = isUpdateAmountAtoneOrAftee
					? hasFixedPurchase
						? new int[] { 1, 2 }
						: new int[] { 2 }
					: hasFixedPurchase
						? new int[] { 1 }
						: transactionOptions;
			}
			return transactionOptions;
		}

		/// <summary>
		/// Create Transaction Options For Update Order
		/// </summary>
		/// <param name="externalPaymentStatus">External Payment Status</param>
		/// <param name="isOrderSalesSettled">Is Order Sales Settled</param>
		/// <param name="isFixedPurchase">Is FixedPurchase Order</param>
		/// <param name="isAtone">Is Atone</param>
		/// <param name="hasDigitalContent">Has Digital Content</param>
		/// <returns>Transaction Options</returns>
		public static int[] CreateTransactionOptionsForUpdateOrder(
			string externalPaymentStatus,
			bool isOrderSalesSettled,
			bool isFixedPurchase,
			bool isAtone,
			bool hasDigitalContent)
		{
			var transactionOptions = new int[] { };
			var isUpdateAmountAtoneOrAftee = (isAtone)
				? Constants.PAYMENT_ATONE_UPDATEAMOUNT_ENABLED
				: Constants.PAYMENT_AFTEE_UPDATEAMOUNT_ENABLED;

			if ((hasDigitalContent == false)
				&& externalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST)
			{
				transactionOptions = (isOrderSalesSettled
					? (isUpdateAmountAtoneOrAftee
						? (isFixedPurchase
							? new int[] { 1, 2 }
							: new int[] { 2 })
						: (isFixedPurchase
							? new int[] { 1 }
							: transactionOptions))
					: (isUpdateAmountAtoneOrAftee
						? (isFixedPurchase
							? new int[] { 1, 2, 3 }
							: new int[] { 2, 3 })
						: (isFixedPurchase
							? new int[] { 1, 3 }
							: new int[] { 3 })));
			}
			else
			{
				transactionOptions = (isUpdateAmountAtoneOrAftee
					? (isFixedPurchase
						? new int[] { 1, 2 }
						: new int[] { 2 })
					: (isFixedPurchase
						? new int[] { 1 }
						: transactionOptions));
			}
			return transactionOptions;
		}

		/// <summary>
		/// Created Order Item Aftee
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <returns>List Item</returns>
		private static List<AfteeCreatePaymentRequest.Item> CreatedOrderItemAftee(CartObject cart)
		{
			var discountPrice = CalculateDiscountPriceByCart(cart);

			// Create Product Items
			var items = cart.Items.Select(item => new AfteeCreatePaymentRequest.Item
			{
				ShopItemId = item.ProductId,
				ItemName = item.ProductName,
				ItemPrice = item.Price.ToString("0"),
				ItemCount = item.CountSingle.ToString()
			}).ToList();

			// Create Shipping Fee Item
			if (cart.PriceShipping > 0)
			{
				var shippingItem = new AfteeCreatePaymentRequest.Item
				{
					ShopItemId = CONST_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "配送料",
					ItemPrice = cart.PriceShipping.ToString("0"),
					ItemCount = "1"
				};

				items.Add(shippingItem);
			}

			// Create Payment Fee Item
			if ((cart.Payment != null)
				&& (cart.Payment.PriceExchange > 0))
			{
				var paymentFeeItem = new AfteeCreatePaymentRequest.Item
				{
					ShopItemId = CONST_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "決済手数料",
					ItemPrice = cart.Payment.PriceExchange.ToString("0"),
					ItemCount = "1"
				};
				items.Add(paymentFeeItem);
			}

			// Create Point Use Item
			if (cart.UsePoint > 0)
			{
				var pointDiscountItem = new AfteeCreatePaymentRequest.Item
				{
					ShopItemId = CONST_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "ポイント利用額",
					ItemPrice = "-" + cart.UsePoint.ToString(),
					ItemCount = "1"
				};
				items.Add(pointDiscountItem);
			}

			// Create Discount Item
			if (discountPrice > 0)
			{
				var discountItem = new AfteeCreatePaymentRequest.Item
				{
					ShopItemId = CONST_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "割引額",
					ItemPrice = "-" + discountPrice.ToString("0"),
					ItemCount = "1"
				};
				items.Add(discountItem);
			}

			// Case: Update Payment For My Page Need Calculate SettlementAmount
			var settlementCurrency = cart.SettlementCurrency;
			var settlementRate = cart.SettlementRate;
			if (string.IsNullOrEmpty(settlementCurrency))
			{
				settlementCurrency = CurrencyManager.GetSettlementCurrency(cart.Payment.PaymentId);
				settlementRate = CurrencyManager.GetSettlementRate(settlementCurrency);
				cart.SettlementAmount = CurrencyManager.GetSettlementAmount(
					cart.OrderId,
					cart.Payment.PaymentId,
					cart.PriceTotal,
					settlementRate,
					settlementCurrency);
			}

			// Convert Taiwan dollar
			items.ForEach(item => item.ItemPrice = CurrencyManager.GetSettlementAmount(
				decimal.Parse(item.ItemPrice),
				settlementRate,
				settlementCurrency).ToString("0"));

			// Check total amount order match with sum total amount of item
			var totalAmountItem = items.Sum(item => PriceCalculator.GetItemPrice(decimal.Parse(item.ItemPrice), int.Parse(item.ItemCount)));
			if (totalAmountItem != cart.SettlementAmount)
			{
				items.Add(new AfteeCreatePaymentRequest.Item
				{
					ShopItemId = CONST_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "調整金額",
					ItemPrice = (cart.SettlementAmount - totalAmountItem).ToString("0"),
					ItemCount = "1"
				});
			}
			return items;
		}

		/// <summary>
		/// Created Order Items Atone
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <returns>List Item</returns>
		private static List<AtoneCreatePaymentRequest.Item> CreatedOrderItemsAtone(CartObject cart)
		{
			var discountPrice = CalculateDiscountPriceByCart(cart);

			// Create Product Items
			var items = cart.Items.Select(item => new AtoneCreatePaymentRequest.Item
			{
				ShopItemId = item.ProductId,
				ItemName = item.ProductName,
				ItemPrice = item.Price.ToString("0"),
				ItemCount = item.CountSingle.ToString()
			}).ToList();

			// Create Shipping Fee Item
			if (cart.PriceShipping > 0)
			{
				var shippingItem = new AtoneCreatePaymentRequest.Item
				{
					ShopItemId = CONST_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "配送料",
					ItemPrice = cart.PriceShipping.ToString("0"),
					ItemCount = "1"
				};

				items.Add(shippingItem);
			}

			// Create Payment Fee Item
			if ((cart.Payment != null)
				&& (cart.Payment.PriceExchange > 0))
			{
				var paymentFeeItem = new AtoneCreatePaymentRequest.Item
				{
					ShopItemId = CONST_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "決済手数料",
					ItemPrice = cart.Payment.PriceExchange.ToString("0"),
					ItemCount = "1"
				};
				items.Add(paymentFeeItem);
			}

			// Create Point Use Item
			if (cart.UsePoint > 0)
			{
				var pointDiscountItem = new AtoneCreatePaymentRequest.Item
				{
					ShopItemId = CONST_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "ポイント利用額",
					ItemPrice = "-" + cart.UsePoint.ToString(),
					ItemCount = "1"
				};
				items.Add(pointDiscountItem);
			}

			// Create Discount Item
			if (discountPrice > 0)
			{
				var discountItem = new AtoneCreatePaymentRequest.Item
				{
					ShopItemId = CONST_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "割引額",
					ItemPrice = "-" + discountPrice.ToString("0"),
					ItemCount = "1"
				};
				items.Add(discountItem);
			}

			// Case: Update Payment For My Page Need Calculate SettlementAmount
			var settlementCurrency = cart.SettlementCurrency;
			var settlementRate = cart.SettlementRate;
			if (string.IsNullOrEmpty(settlementCurrency))
			{
				settlementCurrency = CurrencyManager.GetSettlementCurrency(cart.Payment.PaymentId);
				settlementRate = CurrencyManager.GetSettlementRate(settlementCurrency);
				cart.SettlementAmount = CurrencyManager.GetSettlementAmount(
					cart.OrderId,
					cart.Payment.PaymentId,
					cart.PriceTotal,
					settlementRate,
					settlementCurrency);
			}

			// Convert Japan dollar
			items.ForEach(item => item.ItemPrice = CurrencyManager.GetSettlementAmount(
				decimal.Parse(item.ItemPrice),
				settlementRate,
				settlementCurrency).ToString("0"));

			// Check total amount order match with sum total amount of item
			var totalAmountItem = items.Sum(item => PriceCalculator.GetItemPrice(decimal.Parse(item.ItemPrice), int.Parse(item.ItemCount)));
			if (totalAmountItem != cart.SettlementAmount)
			{
				items.Add(new AtoneCreatePaymentRequest.Item
				{
					ShopItemId = CONST_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "調整金額",
					ItemPrice = (cart.SettlementAmount - totalAmountItem).ToString("0"),
					ItemCount = "1"
				});
			}
			return items;
		}

		/// <summary>
		/// 割引額合計の計算
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <returns>割引額合計</returns>
		private static decimal CalculateDiscountPriceByCart(CartObject cart)
		{
			var productDiscountAmount = 0m;
			var shippingChargeDiscountAmount = 0m;

			if (cart.SetPromotions != null)
			{
				productDiscountAmount = cart.SetPromotions.ProductDiscountAmount;
				shippingChargeDiscountAmount = cart.SetPromotions.ShippingChargeDiscountAmount;
			}

			var discountPrice = PriceCalculator.GetOrderPriceDiscountTotal(
				cart.MemberRankDiscount,
				productDiscountAmount,
				shippingChargeDiscountAmount,
				0m,
				0m,
				cart.UseCouponPrice,
				cart.FixedPurchaseMemberDiscountAmount,
				cart.FixedPurchaseDiscount);
			return discountPrice;
		}

		/// <summary>
		/// ディープコピー
		/// </summary>
		/// <typeparam name="T">オブジェクトタイプ</typeparam>
		/// <param name="copyFromObj">コピー元オブジェクト</param>
		/// <returns>コピー先オブジェクト</returns>
		public static T DeepCopy<T>(T copyFromObj)
		{
			using (var stream = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, copyFromObj);
				stream.Position = 0;
				return (T)formatter.Deserialize(stream);
			}
		}

		/// <summary>
		/// Create Checksum Atone Or Aftee
		/// </summary>
		/// <param name="cartUserId">Cart User Id</param>
		/// <param name="isAtone">Is Atone</param>
		/// <param name="checksum">Checksum</param>
		/// <returns>Checksum decode 64</returns>
		private static string CreateChecksumAtoneOrAftee(
			string cartUserId,
			bool isAtone,
			string checksum)
		{
			checksum += ("false" + cartUserId);
			checksum = string.Format(
				"{0},{1}",
				(isAtone
					? Constants.PAYMENT_ATONE_SECRET_KEY
					: Constants.PAYMENT_AFTEE_SECRET_KEY),
				checksum);

			// To Sha256
			using (var hash = SHA256Managed.Create())
			{
				if (isAtone)
				{
					byte[] resultAtone = hash.ComputeHash(Encoding.UTF8.GetBytes(checksum));
					// To Base64
					checksum = Convert.ToBase64String(resultAtone);
				}
				else
				{
					var checkSumEncode = new StringBuilder();
					var resultAftee = hash.ComputeHash(Encoding.UTF8.GetBytes(checksum));

					foreach (var byteData in resultAftee)
						checkSumEncode.Append(byteData.ToString("x2"));

					// To Base64
					checksum = Convert.ToBase64String(Encoding.ASCII.GetBytes(checkSumEncode.ToString()));
				}
			}
			return checksum;
		}

		/// <summary>
		/// Has Payment Data Aftee Or Atone
		/// </summary>
		/// <param name="fixedPurchaseId">FixedPurchase Id</param>
		/// <param name="paymentId">Payment Id</param>
		/// <returns>True : If have Any</returns>
		public static bool HasPaymentDataAfteeOrAtone(string fixedPurchaseId, string paymentId)
		{
			var orderRelate = new OrderService().GetAllOrderRelateFixedPurchase(fixedPurchaseId);
			var result = false;
			switch (paymentId)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
					result = orderRelate.Any(order => order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
					result = orderRelate.Any(order => order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
					break;
			}
			return result;
		}

		/// <summary>
		/// Update Cancel Payment For Payment Atone Or Aftee
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="sqlAccessor">Sql Accessor</param>
		public static void UpdateCancelPaymentForPaymentAtoneOrAftee(
			OrderModel order,
			string lastChanged,
			SqlAccessor sqlAccessor)
		{
			// オンライン決済ステータス更新
			new OrderService().UpdateOnlinePaymentStatus(
				order.OrderId,
				Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED,
				lastChanged,
				UpdateHistoryAction.DoNotInsert,
				sqlAccessor);

			var sendingAmount = CurrencyManager.GetSendingAmount(
				order.LastBilledAmount,
				order.SettlementAmount,
				order.SettlementCurrency);

			// キャンセル向け外部決済ステータス系＆メモ更新（オンライン決済ステータス更新は二重更新になるが・・）
			UpdateExternalPaymentStatusesAndMemoForCancel(
				order.OrderId,
				order.OrderPaymentKbn,
				order.PaymentOrderId,
				order.CardTranId,
				sendingAmount,
				order.IsExchangeOrder,
				lastChanged,
				"キャンセル",
				UpdateHistoryAction.DoNotInsert,
				sqlAccessor);
		}

		/// <summary>
		/// Update Cancel Payment
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="sqlAccessor">Sql accessor</param>
		public static void UpdateCancelPayment(
			OrderModel order,
			string lastChanged,
			SqlAccessor sqlAccessor)
		{
			// オンライン決済ステータス更新
			new OrderService().UpdateOnlinePaymentStatus(
				order.OrderId,
				Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED,
				lastChanged,
				UpdateHistoryAction.DoNotInsert,
				sqlAccessor);

			var sendingAmount = CurrencyManager.GetSendingAmount(
				order.LastBilledAmount,
				order.SettlementAmount,
				order.SettlementCurrency);

			// キャンセル向け外部決済ステータス系＆メモ更新（オンライン決済ステータス更新は二重更新になるが・・）
			UpdateExternalPaymentStatusesAndMemoForCancel(
				order.OrderId,
				order.OrderPaymentKbn,
				order.PaymentOrderId,
				order.CardTranId,
				sendingAmount,
				order.IsExchangeOrder,
				lastChanged,
				"キャンセル",
				UpdateHistoryAction.DoNotInsert,
				sqlAccessor);
		}

		/// <summary>
		/// Create External Payment Status For Payment Atone Or Aftee
		/// </summary>
		/// <param name="orderModel">Order Model</param>
		/// <returns>External Payment Status</returns>
		public static void CreateExternalPaymentStatusForPaymentAtoneOrAftee(OrderModel orderModel)
		{
			var paymentsNotAllowUpdateWhenAuthMidst = new List<string>
				{
					Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE
				};

			if (paymentsNotAllowUpdateWhenAuthMidst.Contains(orderModel.OrderPaymentKbn)
				&& (orderModel.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST))
			{
				orderModel.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			}
		}

		/// <summary>
		/// Get Landing Cart Session Key
		/// </summary>
		/// <param name="session">Session</param>
		/// <param name="keyCartListLanding">Key Cart List Landing</param>
		/// <param name="keyLandingCartInputAbsolutePath">Key Landing Cart Input Absolute Path</param>
		/// <param name="keyLandingCartInputLastWriteTime">Key Landing Cart Input Last Write Time</param>
		/// <returns>Landing Cart Session Key</returns>
		public static string GetLandingCartSessionKey(
			HttpSessionState session,
			string keyCartListLanding,
			string keyLandingCartInputAbsolutePath,
			string keyLandingCartInputLastWriteTime)
		{
			var landingCartSessionKey = keyCartListLanding
				+ ((string.IsNullOrEmpty(HttpContext.Current.Request[Constants.REQUEST_KEY_RETURN_URL])
					? string.Format(
						"{0}{1}",
						session[keyLandingCartInputAbsolutePath],
						session[keyLandingCartInputLastWriteTime])
					: HttpContext.Current.Request[Constants.REQUEST_KEY_RETURN_URL]));
			return landingCartSessionKey;
		}

		/// <summary>
		/// Get Shipping Service Related Product
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="shippingType">Shipping type</param>
		/// <param name="shippingMethod">Shipping method</param>
		/// <param name="isConvenienceStore">Is convenience store</param>
		/// <returns>List item array: shipping service related product</returns>
		public static ListItem[] GetShippingServiceRelatedProduct(
			string shopId,
			string shippingType,
			string shippingMethod,
			bool isConvenienceStore)
		{
			var shipping = new ShopShippingService().Get(shopId, shippingType);
			var deliveryCompanyList = shipping.CompanyList
				.Where(item => (item.ShippingKbn == shippingMethod))
				.ToArray();
			var deliveryCompanyService = new DeliveryCompanyService();

			// Add item コンビニ受取（7-ELEVEN・FAMI・HILIFE）
			if (isConvenienceStore)
			{
				var list = deliveryCompanyList
					.Any(item => (item.DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID))
						? new ListItem(
							deliveryCompanyService.Get(Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID).DeliveryCompanyName,
							Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)
						: new ListItem();
				return new[] { list };
			}

			// Add items: delivery service inconvenient store
			var result = deliveryCompanyList
				.Where(item => (item.DeliveryCompanyId != Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID))
				.Select(item =>
					new ListItem(
						deliveryCompanyService.Get(item.DeliveryCompanyId).DeliveryCompanyName,
						item.DeliveryCompanyId))
				.ToArray();
			return result;
		}

		/// <summary>
		/// Check Valid TelNo Taiwan For EcPay
		/// </summary>
		/// <param name="ownerTel">Owner tel</param>
		/// <returns>True: if valid Or False: if invalid</returns>
		public static bool CheckValidTelNoTaiwanForEcPay(string ownerTel)
		{
			var regex = new Regex(@"[0][9]\d{8}");
			var isValid = regex.IsMatch(ownerTel);
			return isValid;
		}

		/// <summary>
		/// Sum product size for EcPay
		/// </summary>
		/// <param name="cartObject">Cart object</param>
		/// <returns>Sum size for product</returns>
		public static int SumProductSizeForEcPay(CartObject cartObject)
		{
			var sumSize = 0;
			foreach (var cartProduct in cartObject.Items)
			{
				switch (cartProduct.ShippingSizeKbn)
				{
					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_XXS:
						sumSize += Constants.FLG_SHIPPING_SIZE_KBN_XXS * cartProduct.Count;
						break;

					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_XS:
						sumSize += Constants.FLG_SHIPPING_SIZE_KBN_XS * cartProduct.Count;
						break;

					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_S:
						sumSize += Constants.FLG_SHIPPING_SIZE_KBN_S * cartProduct.Count;
						break;

					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_M:
					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_L:
						sumSize += Constants.FLG_SHIPPING_SIZE_KBN_ML * cartProduct.Count;
						break;

					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_XL:
						sumSize += Constants.FLG_SHIPPING_SIZE_KBN_XL * cartProduct.Count;
						break;

					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_XXL:
						sumSize += Constants.FLG_SHIPPING_SIZE_KBN_XXL * cartProduct.Count;
						break;

					default:
						continue;
				}
			}
			return sumSize;
		}

		/// <summary>
		/// Get Convenience Store Limit Weight
		/// </summary>
		/// <param name="shippingReceivingStoreType">Shipping receiving store type</param>
		/// <returns>Convenience store limit weight</returns>
		public static decimal GetConvenienceStoreLimitWeight(string shippingReceivingStoreType)
		{
			var convenienceStoreLimitWeight = decimal.Parse(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG[0]);

			// When shipping receiving store type is 7-ELEVEN
			if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
				&& ECPayUtility.CheckShippingReceivingStoreType7Eleven(shippingReceivingStoreType))
			{
				convenienceStoreLimitWeight = decimal.Parse(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG[1]);
			}

			return convenienceStoreLimitWeight;
		}

		/// <summary>
		/// Check Owner Name For EcPay
		/// </summary>
		/// <param name="ownerName">Owner name</param>
		/// <returns>True: if owner name 10 bytes or less</returns>
		public static bool CheckOwnerNameForEcPay(string ownerName)
		{
			var result = Validator.CheckByteLengthMaxError(
				string.Empty,
				ownerName,
				Constants.FLG_MAX_LENGHT_OWNER_NAME_ECPAY);
			return string.IsNullOrEmpty(result);
		}

		/// <summary>
		/// Update Cancel Payment For Ec Pay And Neweb Pay
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="accessor">Sql Accessor</param>
		public static void UpdateCancelPaymentForEcPayAndNewebPay(
			OrderModel order,
			string lastChanged,
			SqlAccessor accessor)
		{
			var sendingAmount = CurrencyManager.GetSendingAmount(
				order.LastBilledAmount,
				order.SettlementAmount,
				order.SettlementCurrency);

			// キャンセル向け外部決済ステータス系＆メモ更新（オンライン決済ステータス更新は二重更新になるが・・）
			UpdateExternalPaymentStatusesAndMemoForCancel(
				order.OrderId,
				order.OrderPaymentKbn,
				order.PaymentOrderId,
				order.CardTranId,
				sendingAmount,
				order.IsExchangeOrder,
				lastChanged,
				"キャンセル",
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}

		/// <summary>
		/// Update Cancel Payment For Boku Pay
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="accessor">Sql Accessor</param>
		public static void UpdateCancelPaymentForBokuPay(
			OrderModel order,
			string lastChanged,
			SqlAccessor accessor)
		{
			var sendingAmount = CurrencyManager.GetSendingAmount(
				order.LastBilledAmount,
				order.SettlementAmount,
				order.SettlementCurrency);

			// キャンセル向け外部決済ステータス系＆メモ更新（オンライン決済ステータス更新は二重更新になるが・・）
			UpdateExternalPaymentStatusesAndMemoForCancel(
				order.OrderId,
				order.OrderPaymentKbn,
				order.PaymentOrderId,
				order.CardTranId,
				sendingAmount,
				order.IsExchangeOrder,
				lastChanged,
				"キャンセル",
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}

		/// <summary>
		/// ユーザーデフォルト配送先区分取得
		/// </summary>
		/// <param name="loginUserId">ログインユーザーID</param>
		/// <returns>デフォルト配送先区分</returns>
		public static string GetUserDefaultOrderSettingShippingKbn(string loginUserId)
		{
			var result = string.Empty;
			// ログインしていない場合且つ2クリック決済オプションがオフの場合、デフォルト配送先は注文者とする
			if (string.IsNullOrEmpty(loginUserId) && Constants.TWOCLICK_OPTION_ENABLE)
			{
				result = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER;
				return result;
			}

			var userDefaultOrderSetting =
				new UserDefaultOrderSettingService().Get(loginUserId) ?? new UserDefaultOrderSettingModel();
			switch (userDefaultOrderSetting.UserShippingNo)
			{
				case null:
				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN:
					result = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER;
					break;

				default:
					result = userDefaultOrderSetting.UserShippingNo.ToString();
					break;
			}

			return result;
		}

		/// <summary>
		/// ユーザーデフォルト支払方法ID取得
		/// </summary>
		/// <param name="loginUserId">ログインユーザーID</param>
		/// <returns>デフォルト支払方法ID</returns>
		public static string GetUserDefaultOrderSettingPaymentId(string loginUserId)
		{
			// ログインしていない且つ2クリック決済オプションがオフの場合、デフォルト配送先は「指定なし」とする
			if (string.IsNullOrEmpty(loginUserId) && Constants.TWOCLICK_OPTION_ENABLE) return null;

			var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(loginUserId);
			var paymentId = (userDefaultOrderSetting != null)
				? userDefaultOrderSetting.PaymentId
				: null;
			return paymentId;
		}

		/// <summary>
		/// Display the taiwan invoice info
		/// </summary>
		/// <param name="countryIsoCode">The country iso code</param>
		/// <returns>If can display: true, otherwise: false</returns>
		public static bool DisplayTwInvoiceInfo(string countryIsoCode = "")
		{
			return (string.IsNullOrEmpty(countryIsoCode))
				? (Constants.TWINVOICE_ENABLED
					|| Constants.TWINVOICE_ECPAY_ENABLED)
				: ((Constants.TWINVOICE_ENABLED
						|| Constants.TWINVOICE_ECPAY_ENABLED)
					&& (countryIsoCode == Constants.COUNTRY_ISO_CODE_TW));
		}

		/// <summary>
		/// The taiwan invoice status that can edit order
		/// </summary>
		/// <param name="twOrderInvoice">Taiwan order invoice</param>
		/// <param name="isNotTwInvoiceEcPay">Is not taiwan invoice ec pay</param>
		/// <returns>If can not edit: true, otherwise: false</returns>
		public static bool TwInvoiceStatusCanNotEditOrder(
			TwOrderInvoiceModel twOrderInvoice,
			bool isNotTwInvoiceEcPay = false)
		{
			var resultTmp = ((twOrderInvoice != null)
				&& (twOrderInvoice.TwInvoiceStatus != Constants.FLG_ORDER_INVOICE_STATUS_NOT_ISSUED)
				&& (twOrderInvoice.TwInvoiceStatus != Constants.FLG_ORDER_INVOICE_STATUS_ISSUED)
				&& (twOrderInvoice.TwInvoiceStatus != Constants.FLG_ORDER_INVOICE_STATUS_ISSUED_LINKED));

			var result = ((isNotTwInvoiceEcPay) || (twOrderInvoice == null))
				? resultTmp
				: resultTmp && (twOrderInvoice.TwInvoiceStatus != Constants.FLG_ORDER_INVOICE_STATUS_CANCEL);
			return result;
		}

		/// <summary>
		/// The taiwan invoice status that can enable control
		/// </summary>
		/// <param name="twOrderInvoice">Taiwan order invoice</param>
		/// <param name="isNotTwInvoiceEcPay">Is not taiwan invoice ec pay</param>
		/// <returns>If can enable control: true, otherwise: false</returns>
		public static bool TwInvoiceStatusCanEnableControl(
			TwOrderInvoiceModel twOrderInvoice,
			bool isNotTwInvoiceEcPay = false)
		{
			var resultTmp = ((twOrderInvoice != null)
				&& ((twOrderInvoice.TwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_NOT_ISSUED)
					|| (twOrderInvoice.TwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_ISSUED)));

			var result = ((isNotTwInvoiceEcPay) || (twOrderInvoice == null))
				? resultTmp
				: resultTmp || (twOrderInvoice.TwInvoiceStatus == Constants.FLG_ORDER_INVOICE_STATUS_CANCEL);
			return result;
		}

		/// <summary>
		/// Get the taiwan invoice data for send mail
		/// </summary>
		/// <param name="orderId">The order id</param>
		/// <param name="shippingNo">The order shipping no</param>
		/// <param name="mailData">The mail datas</param>
		public static void GetTwInvoiceDataForSendMail(string orderId, int shippingNo, Hashtable mailData)
		{
			if (OrderCommon.DisplayTwInvoiceInfo() == false) return;

			var twOrderInvoice = new TwOrderInvoiceService().GetOrderInvoice(orderId, shippingNo);
			if (twOrderInvoice == null) return;

			var languageLocaleId = StringUtility.ToEmpty(mailData[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID]);
			var invoiceDateString = DateTimeUtility.ToString(
				twOrderInvoice.TwInvoiceDate.Value,
				DateTimeUtility.FormatType.LongDateHourMinute2Letter,
				languageLocaleId);
			mailData[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_NO] = twOrderInvoice.TwInvoiceNo ?? string.Empty;
			mailData[Constants.FIELD_TWORDERINVOICE_TW_INVOICE_DATE] = invoiceDateString;
		}

		/// <summary>
		/// Get payment ids can change
		/// </summary>
		/// <param name="paymentId">Payment id</param>
		/// <param name="settingCanChangePaymentIds">Setting can change payment ids</param>
		/// <param name="isPriority">Is priority</param>
		/// <returns>Payment ids</returns>
		public static string[] GetPaymentIdsCanChange(string paymentId, string[] settingCanChangePaymentIds, bool isPriority)
		{
			var indexPayment = settingCanChangePaymentIds.ToList().IndexOf(paymentId);
			var paymentIds = (isPriority && settingCanChangePaymentIds.Contains(paymentId))
				? settingCanChangePaymentIds.Take(indexPayment + 1)
				: settingCanChangePaymentIds;

			return paymentIds.ToArray();
		}

		/// <summary>
		/// AmazonPayペイメントId取得
		/// </summary>
		/// <returns>ペイメントId</returns>
		public static string GetAmazonPayPaymentId()
		{
			var result = Constants.AMAZON_PAYMENT_CV2_ENABLED
				? Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2
				: Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT;
			return result;
		}

		/// <summary>
		/// AmazonPayか？
		/// </summary>
		/// <param name="paymentId">ペイメントId</param>
		/// <returns></returns>
		public static bool IsAmazonPayment(string paymentId)
		{
			var result = (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				|| (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2);
			return result;
		}

		/// <summary>
		/// AmazonPay(CV1)のペイメントIdを変換する
		/// </summary>
		/// <param name="paymentId">ペイメントId</param>
		/// <returns>変換後ペイメントId</returns>
		public static string ConvertAmazonPaymentId(string paymentId)
		{
			var result = (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				? GetAmazonPayPaymentId()
				: paymentId;
			return result;
		}

		/// <summary>
		/// ヤマト後払いSMS認証連携決済注文か
		/// </summary>
		/// <param name="paymentId">ペイメントId</param>
		/// <returns>判定結果</returns>
		public static bool CheckPaymentYamatoKaSms(string paymentId)
		{
			var result = (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF);
			return result;
		}

		/// <summary>
		/// Automatic delivery method setting (considering product bundling)
		/// </summary>
		/// <param name="cartList">Cart list</param>
		/// <param name="userId">User id</param>
		/// <param name="orderPaymentKbn">Order payment Kbn</param>
		/// <param name="beforeShippingMethod">Before shipping method</param>
		/// <param name="advCodeFirst">Adv code first</param>
		/// <param name="advCodeNew">Adv code new</param>
		/// <param name="excludeOrderIds">Exclude order id</param>
		/// <returns>Shipment method</returns>
		public static void SetShippingMethod(
			List<CartObject> cartList,
			string userId,
			string orderPaymentKbn,
			string beforeShippingMethod,
			string advCodeFirst,
			string advCodeNew,
			string[] excludeOrderIds = null)
		{
			using (var bundler = new ProductBundler(
				cartList,
				userId,
				advCodeFirst,
				advCodeNew,
				excludeOrderIds))
			{
				bundler.CartList.ForEach(cart => cart.Shippings
					.ForEach(shipping => shipping.ShippingMethod = GetShippingMethod(cart.Items)));
			}
		}

		/// <summary>
		/// 配送方法自動判定
		/// </summary>
		/// <param name="cartProducts">カート内商品</param>
		/// <returns>配送方法</returns>
		public static string GetShippingMethod(IEnumerable<CartProduct> cartProducts)
		{
			return GetShippingMethod(cartProducts
				.Select(product => new KeyValuePair<string, Tuple<int, string, string>>(
					product.ProductId,
					new Tuple<int, string, string>(
						product.Count,
						product.ShopId,
						product.VariationId))));
		}

		/// <summary>
		/// 配送方法自動判定
		/// </summary>
		/// <param name="orderItems">注文商品</param>
		/// <returns>配送方法</returns>
		public static string GetShippingMethod(IEnumerable<OrderItemModel> orderItems)
		{
			return GetShippingMethod(orderItems
				.Select(orderItem => new KeyValuePair<string, Tuple<int, string, string>>(
					orderItem.ProductId,
					new Tuple<int, string, string>(
						orderItem.ItemQuantity,
						(string)orderItem.ShopId,
						(string)orderItem.VariationId))));
		}

		/// <summary>
		/// 配送方法自動判定
		/// </summary>
		/// <param name="fixedPurchaseItems">定期注文商品</param>
		/// <returns>配送方法</returns>
		public static string GetShippingMethod(IEnumerable<FixedPurchaseItemModel> fixedPurchaseItems)
		{
			return GetShippingMethod(fixedPurchaseItems
				.Select(fixedPurchaseItem => new KeyValuePair<string, Tuple<int, string, string>>(
					fixedPurchaseItem.ProductId,
					new Tuple<int, string, string>(
						fixedPurchaseItem.ItemQuantity,
						(string)fixedPurchaseItem.ShopId,
						(string)fixedPurchaseItem.VariationId))));
		}

		/// <summary>
		/// 配送方法自動判定
		/// </summary>
		/// <param name="orderItems">商品情報</param>
		/// <returns>配送方法</returns>
		public static string GetShippingMethod(IEnumerable<Hashtable> orderItems)
		{
			var itemQuantity = 0;
			return GetShippingMethod(orderItems
				.Select(orderItem => new KeyValuePair<string, Tuple<int, string, string>>(
					(string)orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID],
					new Tuple<int, string, string>(
						int.TryParse((string)orderItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY], out itemQuantity)
							? itemQuantity
							: 0,
						(string)orderItem[Constants.FIELD_ORDERITEM_SHOP_ID],
						(string)orderItem[Constants.FIELD_ORDERITEM_VARIATION_ID]))));
		}

		/// <summary>
		/// 配送方法自動判定
		/// </summary>
		/// <param name="orderItems">Order Items</param>
		/// <returns>配送方法</returns>
		private static string GetShippingMethod(IEnumerable<KeyValuePair<string, Tuple<int, string, string>>> orderItems)
		{
			var products = DomainFacade.Instance.ProductService.GetProducts(
				orderItems.Select(item => item.Value.Item2).FirstOrDefault(),
				orderItems.Select(item => item.Key));

			if (products == null) return string.Empty;

			var totalMailEscalationCount = 0;
			foreach (var product in products)
			{
				var productMailEscalationCount = product.ProductSizeFactor * orderItems
					.Where(item => ((item.Key == product.ProductId)
						&& (item.Value.Item3 == product.VariationId)
						&& (item.Value.Item1 > 0)))
					.Sum(item => item.Value.Item1);
				if ((product.ShippingSizeKbn != Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL)
					|| (productMailEscalationCount < 0))
				{
					return Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
				}
				totalMailEscalationCount += productMailEscalationCount;
			}

			var canShippingMail = ((totalMailEscalationCount <= Constants.MAIL_ESCALATION_COUNT)
				&& (totalMailEscalationCount >= 0));

			var shippingKbn = canShippingMail
				? Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL
				: Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
			return shippingKbn;
		}

		/// <summary>
		/// Update order extend status
		/// </summary>
		/// <param name="cart">Cart Object</param>
		/// <param name="fixedPurchase">Fixed Purchase Model</param>
		public static void UpdateOrderExtendStatus(CartObject cart, FixedPurchaseModel fixedPurchase)
		{
			if ((string.IsNullOrEmpty(Constants.MAIL_ESCALATION_TO_EXPRESS_ORDEREXTENDNO) == false)
				&& (fixedPurchase.Shippings[0].IsMail
					&& (OrderCommon.GetShippingMethod(cart.Items) == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)))
			{
				var fixedPurchaseHistoryCondition =
					new FixedPurchaseHistoryListSearchCondition() { FixedPurchaseId = fixedPurchase.FixedPurchaseId };
				var fixedPurchaseHistory = new FixedPurchaseService().SearchFixedPurchaseHistory(fixedPurchaseHistoryCondition);

				// 注文拡張ステータス更新
				new OrderService().UpdateOrderExtendStatus(
					fixedPurchaseHistory.FirstOrDefault().OrderId,
					int.Parse(Constants.MAIL_ESCALATION_TO_EXPRESS_ORDEREXTENDNO),
					Constants.FLG_ON,
					DateTime.Now,
					fixedPurchase.LastChanged,
					UpdateHistoryAction.Insert);
			}
		}

		/// <summary>
		/// メール便配送サービスエスカレーション
		/// </summary>
		/// <param name="cartProducts">カート内商品</param>
		/// <param name="companyListMail">メール便配送サービス</param>
		/// <returns>配送会社ID</returns>
		public static string DeliveryCompanyMailEscalation(
			IEnumerable<CartProduct> cartProducts,
			ShopShippingCompanyModel[] companyListMail)
		{
			var totalProductSize = cartProducts.Sum(item => item.ProductSizeFactor * item.Count);
			return GetDeliveryCompanyId(companyListMail, totalProductSize);
		}
		/// <summary>
		/// メール便配送サービスエスカレーション
		/// </summary>
		/// <param name="fixedPurchaseItems">定期注文商品</param>
		/// <param name="companyListMail">メール便配送サービス</param>
		/// <returns>配送会社ID</returns>
		public static string DeliveryCompanyMailEscalation(
			IEnumerable<FixedPurchaseItemModel> fixedPurchaseItems,
			ShopShippingCompanyModel[] companyListMail)
		{
			var itemQuantity = 0;
			var totalProductSize = GetTotalProductSizeFactor(fixedPurchaseItems
				.Select(item => new KeyValuePair<string, Tuple<int, string, string>>(item.ProductId,
					new Tuple<int, string, string>(
						(int.TryParse(item.ItemQuantity.ToString(), out itemQuantity)
							? itemQuantity
							: 0),
						item.ShopId,
						item.VariationId))));
			return GetDeliveryCompanyId(companyListMail, totalProductSize);
		}

		/// <summary>
		/// 配送会社ID取得
		/// </summary>
		/// <param name="companyListMail">メール便配送サービス</param>
		/// <param name="totalProductSize">商品サイズ合計係数</param>
		/// <returns>配送会社ID</returns>
		public static string GetDeliveryCompanyId(
			ShopShippingCompanyModel[] companyListMail,
			int totalProductSize)
		{
			var deliveryCompanyList = GetDeliveryCompanyList(companyListMail);

			var deliveryCompany = deliveryCompanyList
				.Where(company => company.DeliveryCompanyMailSizeLimit > (totalProductSize - 1))
				.OrderBy(company => company.DeliveryCompanyMailSizeLimit)
				.FirstOrDefault();
			var result = (deliveryCompany == null)
				? string.Empty
				: deliveryCompany.DeliveryCompanyId;
			return result;
		}

		/// <summary>
		/// 商品サイズ合計係数
		/// </summary>
		/// <param name="orderItems">商品情報</param>
		/// <returns>合計サイズ係数</returns>
		public static int GetTotalProductSizeFactor(
			IEnumerable<KeyValuePair<string, Tuple<int, string, string>>> orderItems)
		{
			var products = new ProductService().GetProducts(
				orderItems.Select(item => item.Value.Item2).FirstOrDefault(),
				orderItems.Select(item => item.Key));

			var totalProductSize = 0;
			if (products == null) return totalProductSize;

			foreach (var product in products)
			{
				var productSize = product.ProductSizeFactor * orderItems
					.Where(item => ((item.Key == product.ProductId)
						&& (item.Value.Item3 == product.VariationId)
						&& (item.Value.Item1 > 0)))
					.Sum(item => item.Value.Item1);
				totalProductSize += productSize;
			}
			return totalProductSize;
		}

		/// <summary>
		/// 配送会社取得
		/// </summary>
		/// <param name="companyList">配送サービス</param>
		/// <returns>配送会社</returns>
		public static DeliveryCompanyModel[] GetDeliveryCompanyList(
			ShopShippingCompanyModel[] companyList)
		{
			var deliveryCompany = new DeliveryCompanyService();
			return companyList
				.Select(company => deliveryCompany.Get(company.DeliveryCompanyId))
				.ToArray();
		}

		/// <summary>
		/// 注文拡張ステータス更新（メール便配送サービスエスカレーション機能）
		/// </summary>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="fixedPurchase">定期購入情報モデル</param>
		/// <param name="extendNo">注文拡張ステータス番号</param>
		public static void UpdateOrderExtendStatusByDeliveryCompanyMailEscalation(
			CartObject cart,
			FixedPurchaseModel fixedPurchase,
			string extendNo)
		{
			if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED
				&& (string.IsNullOrEmpty(extendNo) == false))
			{
				var fixedPurchaseHistoryCondition =
					new FixedPurchaseHistoryListSearchCondition() { FixedPurchaseId = fixedPurchase.FixedPurchaseId };
				var fixedPurchaseHistory = new FixedPurchaseService().SearchFixedPurchaseHistory(fixedPurchaseHistoryCondition);

				// 注文拡張ステータス更新
				new OrderService().UpdateOrderExtendStatus(
					fixedPurchaseHistory.FirstOrDefault().OrderId,
					int.Parse(extendNo),
					Constants.FLG_ON,
					DateTime.Now,
					fixedPurchase.LastChanged,
					UpdateHistoryAction.Insert);
			}
		}

		/// <summary>
		/// 決済連携メモ作成
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID／注文ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="cardTranId">決済連携ID</param>
		/// <param name="actionNameWithoutPaymentName">決済種別名抜きアクション名</param>
		/// <param name="lastBilledAmount">最終請求金額（キャンセルはnull）</param>
		/// <returns>決済連携メモ</returns>
		public static string CreateOrderPaymentMemo(
			string paymentOrderId,
			string paymentId,
			string cardTranId,
			string actionNameWithoutPaymentName,
			decimal? lastBilledAmount)
		{
			var paymentName = string.Empty;
			switch (paymentId)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
					paymentName = "クレジットカード決済";
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
					paymentName = "コンビニ後払い";
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_ORG:
				case Constants.FLG_PAYMENT_PAYMENT_ID_SMATOMETE_ORG:
				case Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS:
				case Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS:
				case Constants.FLG_PAYMENT_PAYMENT_ID_RECRUIT_SBPS:
				case Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS:
					paymentName = "キャリア決済";
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT:
					paymentName = "Amazon Pay決済";
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
					paymentName = "PayPay決済";
					break;
			}
			return CreateOrderPaymentMemo(
				paymentOrderId,
				paymentId,
				cardTranId,
				paymentName + actionNameWithoutPaymentName,
				lastBilledAmount,
				false);
		}

		/// <summary>
		/// 決済連携メモ作成
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID／注文ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="cardTranId">決済連携ID</param>
		/// <param name="actionName">アクション名</param>
		/// <param name="lastBilledAmount">最終請求金額（キャンセルはnull）</param>
		/// <param name="isReauth">再与信の処理であるかどうか</param>
		/// <returns>決済連携メモ</returns>
		public static string CreateOrderPaymentMemo(
			string paymentOrderId,
			string paymentId,
			string cardTranId,
			string actionName,
			decimal? lastBilledAmount,
			bool isReauth = false)
		{
			var settlementCurrency = CurrencyManager.GetSettlementCurrency(paymentId);
			var priceString = lastBilledAmount.HasValue
				? Constants.GLOBAL_OPTION_ENABLE
					? CurrencyManager.ToSettlementCurrencyNotation(
						lastBilledAmount.Value,
						settlementCurrency,
						true)
					: string.Format(" {0}円", lastBilledAmount.ToPriceDecimal())
				: string.Empty;

			var paymentMemo = string.Format(
				"{0} {1}：{2}・{3}・{4}{5} {6}",
				DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
				CommonPage.ReplaceTag("@@DispText.order.CardTranId@@"),
				paymentOrderId,
				cardTranId,
				priceString,
				isReauth
					? string.Format(" {0}：", CommonPage.ReplaceTag("@@DispText.order.Reauth@@"))
					: string.Empty,
				actionName);
			return paymentMemo;
		}

		/// <summary>
		/// APIで継続課金（定期・従量）の解約が行える決済か
		/// </summary>
		/// <param name="paymentKbn">決済区分</param>
		/// <returns>判定結果</returns>
		public static bool IsCancelablePaymentContinuousByApi(string paymentKbn)
		{
			return ((paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS)
				|| (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS)
				|| (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY) && (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.GMO));
		}

		/// <summary>
		/// 決済管理ツールのみで継続課金（定期・従量）の解約が行える決済か（APIがない）
		/// </summary>
		/// <param name="paymentKbn">決済区分</param>
		/// <returns>判定結果</returns>
		public static bool IsOnlyCancelablePaymentContinuousByManual(string paymentKbn)
		{
			return (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS);
		}

		/// <summary>
		/// 継続課金（定期・従量）を利用できる決済か
		/// </summary>
		/// <param name="paymentKbn">決済区分</param>
		/// <returns>判定結果</returns>
		public static bool IsUsablePaymentContinuous(string paymentKbn)
		{
			return ((paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS)
				|| (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS)
				|| (paymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS));
		}

		/// <summary>
		/// 詳細表示用サイト名取得
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="mallName">モール名</param>
		/// <returns>サイト名（モール名＋モールID）</returns>
		public static string CreateSiteNameForDetail(string mallId, string mallName)
		{
			var siteNames = new StringBuilder();
			if ((mallId != Constants.FLG_USER_MALL_ID_OWN_SITE)
				&& (Constants.FLG_USER_MALL_ID_EXTERNAL_ORDER_SITES.Any(site => site == mallId) == false))
			{
				siteNames.Append(mallName);
				siteNames.AppendFormat(
					" ({0}：{1})",
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_SITENAME,
						Constants.VALUETEXT_PARAM_MALLIDNAME,
						Constants.FIELD_ORDER_MALL_ID),
					mallId);
			}
			else
			{
				siteNames.Append(
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_SITENAME,
						Constants.VALUETEXT_PARAM_OWNSITENAME,
						Constants.FLG_USER_MALL_ID_EXTERNAL_ORDER_SITES.Any(site => (site == mallId))
							? mallId
							: Constants.FLG_USER_MALL_ID_OWN_SITE));
			}
			return siteNames.ToString();
		}

		/// <summary>
		/// JSON形式で格納されている返金メモから返金先銀行口座情報のディクショナリを作成
		/// </summary>
		/// <param name="repaymentMemo">返金メモ</param>
		/// <returns>返金先銀行口座情報（JSON形式でない場合はnull）</returns>
		public static Dictionary<string, string> CreateRepaymentBankDictionary(string repaymentMemo)
		{
			try
			{
				return JsonConvert.DeserializeObject<Dictionary<string, string>>(repaymentMemo);
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		/// <summary>
		/// 頒布会数量チェック
		/// </summary>
		/// <param name="subsctiptionBoxCourseId">頒布会コースID</param>
		/// <param name="totalQuantity">購入数量</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckLimitProductOrderForSubscriptionBox(string subsctiptionBoxCourseId, int totalQuantity)
		{
			var subscriptionBoxConfig = new SubscriptionBoxService().GetByCourseId(subsctiptionBoxCourseId);

			if (subscriptionBoxConfig.MinimumPurchaseQuantity.HasValue
				&& subscriptionBoxConfig.MaximumPurchaseQuantity.HasValue
				&& ((totalQuantity > subscriptionBoxConfig.MaximumPurchaseQuantity)
					|| (totalQuantity < subscriptionBoxConfig.MinimumPurchaseQuantity)))
			{
				var result = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CART_QUANTITY)
					.Replace("@@ 1 @@", subscriptionBoxConfig.DisplayName)
					.Replace("@@ 2 @@", subscriptionBoxConfig.MinimumPurchaseQuantity.ToString())
					.Replace("@@ 3 @@", subscriptionBoxConfig.MaximumPurchaseQuantity.ToString());
				return result;
			}

			if (subscriptionBoxConfig.MaximumPurchaseQuantity.HasValue
				&& (totalQuantity > subscriptionBoxConfig.MaximumPurchaseQuantity))
			{
				var result = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CART_QUANTITY_MAX)
					.Replace("@@ 1 @@", subscriptionBoxConfig.DisplayName)
					.Replace("@@ 2 @@", subscriptionBoxConfig.MaximumPurchaseQuantity.ToString());
				return result;
			}

			if (subscriptionBoxConfig.MinimumPurchaseQuantity.HasValue
				&& (totalQuantity < subscriptionBoxConfig.MinimumPurchaseQuantity))
			{
				var result = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CART_QUANTITY_MIN)
					.Replace("@@ 1 @@", subscriptionBoxConfig.DisplayName)
					.Replace("@@ 2 @@", subscriptionBoxConfig.MinimumPurchaseQuantity.ToString());
				return result;
			}

			return string.Empty;
		}

		/// <summary>
		/// 頒布会商品種類数エラーメッセージ取得
		/// </summary>
		/// <param name="subsctiptionBoxCourseId">頒布会コースID</param>
		/// <param name="numberOfProducts">商品種類数</param>
		/// <param name="isFront">フロントか</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetSubscriptionBoxProductOfNumberError(string subsctiptionBoxCourseId, int numberOfProducts, bool isFront = false)
		{
			var subscriptionBoxProductOfNumberError =
				new SubscriptionBoxService().GetSubscriptionBoxProductOfNumberErrorType(
					subsctiptionBoxCourseId,
					numberOfProducts,
					isFront);
			var message = GetSubscriptionBoxProductOfNumberError(subscriptionBoxProductOfNumberError);
			return message;
		}

		/// <summary>
		/// 頒布会商品種類数エラーメッセージ取得
		/// </summary>
		/// <param name="errorType">頒布会商品種類数エラークラス</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetSubscriptionBoxProductOfNumberError(SubscriptionBoxProductOfNumberError errorType)
		{
			var message = "";
			switch (errorType.ErrorType)
			{
				case SubscriptionBoxProductOfNumberError.ErrorTypes.MinLimit:
					message = (string.IsNullOrEmpty(errorType.MaximumNumberOfProducts))
						? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUBSCRIPTION_BOX_MIN_NUMBER_OF_PRODUCTS_ERROR)
							.Replace("@@ 1 @@", errorType.DisplayName)
							.Replace("@@ 2 @@", errorType.MinimumNumberOfProducts)
						: CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUBSCRIPTION_BOX_NUMBER_OF_PRODUCTS_ERROR_DISPLAY_REQUIRED_NUMBER_OF_PRODUCTS)
							.Replace("@@ 1 @@", errorType.DisplayName)
							.Replace("@@ 2 @@", errorType.MinimumNumberOfProducts)
							.Replace("@@ 3 @@", errorType.MaximumNumberOfProducts);
					break;

				case SubscriptionBoxProductOfNumberError.ErrorTypes.MaxLimit:
					message = (string.IsNullOrEmpty(errorType.MinimumNumberOfProducts))
						? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUBSCRIPTION_BOX_MAX_NUMBER_OF_PRODUCTS_ERROR)
							.Replace("@@ 1 @@", errorType.DisplayName)
							.Replace("@@ 2 @@", errorType.MaximumNumberOfProducts)
						: CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUBSCRIPTION_BOX_NUMBER_OF_PRODUCTS_ERROR_DISPLAY_REQUIRED_NUMBER_OF_PRODUCTS)
							.Replace("@@ 1 @@", errorType.DisplayName)
							.Replace("@@ 2 @@", errorType.MinimumNumberOfProducts)
							.Replace("@@ 3 @@", errorType.MaximumNumberOfProducts);
					break;
			}
			return message;
		}

		/// <summary>
		/// 頒布会商品合計金額（税込）エラーメッセージ取得
		/// </summary>
		/// <param name="subsctiptionBoxCourseId">頒布会コースID</param>
		/// <param name="amount">商品合計金額（税込）</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetSubscriptionBoxTotalAmountError(string subsctiptionBoxCourseId, decimal amount)
		{
			var message = "";

			if (string.IsNullOrEmpty(subsctiptionBoxCourseId)) return message;

			var selectedSubscriptionBox = DataCacheControllerFacade
				.GetSubscriptionBoxCacheController()
				.Get(subsctiptionBoxCourseId);
			if (((selectedSubscriptionBox.MinimumAmount != null)
					&& (selectedSubscriptionBox.MaximumAmount != null))
				&& (((selectedSubscriptionBox.MinimumAmount != null)
						&& (amount < selectedSubscriptionBox.MinimumAmount))
					|| ((selectedSubscriptionBox.MaximumAmount != null)
						&& (amount > selectedSubscriptionBox.MaximumAmount))))
			{
				if (selectedSubscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE)
				{
					message = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_AMOUNT_SET_FOR_FIXED);
				}
				else
				{
					message = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_AMOUNT_SET_FOR_NON_FIXED)
						.Replace("@@ 1 @@", CurrencyManager.ToPrice(selectedSubscriptionBox.MinimumAmount))
						.Replace("@@ 2 @@", CurrencyManager.ToPrice(selectedSubscriptionBox.MaximumAmount));
				}
			}
			else if ((selectedSubscriptionBox.MinimumAmount != null)
						&& (amount < selectedSubscriptionBox.MinimumAmount))
			{
				if (selectedSubscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE)
				{
					message = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_AMOUNT_SET_FOR_FIXED);
				}
				else
				{
					message = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_MINIMUM_AMOUNT_SET_FOR_NON_FIXED)
						.Replace("@@ 1 @@", CurrencyManager.ToPrice(selectedSubscriptionBox.MinimumAmount));
				}
			}
			else if ((selectedSubscriptionBox.MaximumAmount != null)
					&& (amount > selectedSubscriptionBox.MaximumAmount))
			{
				if (selectedSubscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE)
				{
					message = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_AMOUNT_SET_FOR_FIXED);
				}
				else
				{
					message = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SUBSCRIPTION_BOX_DOES_NOT_MEET_MAXIMUM_AMOUNT_SET_FOR_NON_FIXED)
						.Replace("@@ 1 @@", CurrencyManager.ToPrice(selectedSubscriptionBox.MaximumAmount));
				}
			}

			return message;
		}

		/// <summary>
		/// 今日が頒布会キャンペーン期間かどうか
		/// </summary>
		/// <param name="subscriptionBoxItem">頒布会商品</param>
		/// <returns>キャンペーン期間：true、キャンペーン期間でない：false</returns>
		public static bool IsSubscriptionBoxCampaignPeriod(SubscriptionBoxItemModel subscriptionBoxItem)
		{
			var isCampaignPeriod = (subscriptionBoxItem != null)
				&& ((((subscriptionBoxItem.CampaignSince != null)
							&& (subscriptionBoxItem.CampaignUntil != null))
						&& (subscriptionBoxItem.CampaignSince <= DateTime.Now)
						&& (subscriptionBoxItem.CampaignUntil >= DateTime.Now))
					|| ((subscriptionBoxItem.CampaignSince != null)
						&& (subscriptionBoxItem.CampaignUntil == null)
						&& (subscriptionBoxItem.CampaignSince <= DateTime.Now))
					|| ((subscriptionBoxItem.CampaignUntil != null)
						&& (subscriptionBoxItem.CampaignSince == null)
						&& (subscriptionBoxItem.CampaignUntil >= DateTime.Now)));

			return isCampaignPeriod;
		}

		/// <summary>
		/// 次回配送日が頒布会キャンペーン期間かどうか
		/// </summary>
		/// <param name="subscriptionBoxItem">頒布会商品</param>
		/// <returns>エラーメッセージキャンペーン期間：true、キャンペーン期間でない：false</returns>
		public static bool IsSubscriptionBoxCampaignPeriodByNextShippingDate(SubscriptionBoxItemModel subscriptionBoxItem, DateTime nextShippingDate)
		{
			var isCampaignPeriod = (subscriptionBoxItem != null)
				&& ((((subscriptionBoxItem.CampaignSince != null)
							&& (subscriptionBoxItem.CampaignUntil != null))
						&& (subscriptionBoxItem.CampaignSince <= nextShippingDate)
						&& (subscriptionBoxItem.CampaignUntil >= nextShippingDate))
					|| ((subscriptionBoxItem.CampaignSince != null)
						&& (subscriptionBoxItem.CampaignUntil == null)
						&& (subscriptionBoxItem.CampaignSince <= nextShippingDate))
					|| ((subscriptionBoxItem.CampaignUntil != null)
						&& (subscriptionBoxItem.CampaignSince == null)
						&& (subscriptionBoxItem.CampaignUntil >= nextShippingDate)));

			return isCampaignPeriod;
		}

		/// <summary>
		/// Can display invoice bundle
		/// </summary>
		/// <returns>True: if can display invoice bundle, otherwise: False</returns>
		public static bool CanDisplayInvoiceBundle()
		{
			var result = (IsInvoiceBundleServiceUsable()
				|| Constants.PAYMENT_NP_AFTERPAY_INVOICEBUNDLE);
			return result;
		}

		/// <summary>
		/// 与信結果取得のための必要な注文
		/// </summary>
		/// <param name="orderModel">注文</param>
		/// <returns>ステータス取得</returns>
		public static bool IsGetGmoCreditStatus(OrderModel orderModel)
		{
			var isGetStatus = false;
			if (((orderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO) || (orderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE))
				&& ((orderModel.ExternalPaymentStatus == Constants.FLG_ORDER_CREDIT_STATUS_INREVIEW) || ((orderModel.ExternalPaymentStatus == Constants.FLG_ORDER_CREDIT_STATUS_DEPOSIT_WAITING))
				&& (string.IsNullOrEmpty(orderModel.CardTranId) == false)
				&& (orderModel.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)))
			{
				isGetStatus = true;
			}
			return isGetStatus;
		}

		/// <summary>
		/// 与信結果確認及び注文更新
		/// </summary>
		/// <param name="orderModel">注文</param>
		/// <returns>ステータス変更</returns>
		public static bool CheckGmoCredit(OrderModel orderModel)
		{
			// Call API
			var isChangeStatus = false;
			var facade = new GmoTransactionApi();
			var request = new GmoRequestGetCreditStatus();
			request.Transaction.GmoTransactionId = orderModel.CardTranId;
			var result = facade.GetCreditResult(request);
			// NG: cancel order
			if (result.IsNG)
			{
				new OrderService().Modify(
					orderModel.OrderId,
					order =>
					{
						order.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED;
						order.OrderCancelDate = DateTime.Now;
						order.ExternalPaymentStatus = result.Result.ToString();
						order.DateChanged = DateTime.Now;
						order.LastChanged = orderModel.LastChanged;
					},
					UpdateHistoryAction.Insert);
				isChangeStatus = true;
			}
			// OK: update order
			else if (result.IsOk)
			{
				new OrderService().Modify(
					orderModel.OrderId,
					order =>
					{
						order.OrderPaymentStatus = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE;
						order.OrderPaymentDate = DateTime.Now;
						order.ExternalPaymentStatus = result.Result.ToString();
						order.DateChanged = DateTime.Now;
						order.LastChanged = orderModel.LastChanged;
					},
					UpdateHistoryAction.Insert);
				isChangeStatus = true;
			}
			// Another status: update credit status
			else if (result.IsResultOk)
			{
				var authorResult = result.TransactionResult.AutoAuthorResult;
				if (result.IsInReview)
				{
					authorResult = Constants.FLG_ORDER_CREDIT_STATUS_INREVIEW;
				}
				else if (result.IsDepositWaiting)
				{
					authorResult = Constants.FLG_ORDER_CREDIT_STATUS_DEPOSIT_WAITING;
				}

				new OrderService().Modify(
					orderModel.OrderId,
					order =>
					{
						order.ExternalPaymentStatus = authorResult;
						order.DateChanged = DateTime.Now;
						order.LastChanged = orderModel.LastChanged;
					},
					UpdateHistoryAction.Insert);
				isChangeStatus = true;
			}
			return isChangeStatus;
		}

		/// <summary>
		/// 請求確認APIの呼び出し
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="cardTranId">決済カード取引ID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>APIレスポンス</returns>
		public static string ExecConfirmBillingGmoPost(string orderId, string lastChanged, string cardTranId, SqlAccessor sqlAccessor = null)
		{
			var orderCheck = new OrderService().Get(orderId, sqlAccessor);
			if (orderCheck.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
			{
				return string.Empty;
			}
			var fixRequestDate = DateTime.Now.ToString("yyyy/MM/dd");
			var confirmRequest = new GmoRequestBillingConfirmation(cardTranId, fixRequestDate);
			var gmoTransactionApi = new GmoTransactionApi();
			var gmoResponseBillingConfirmation = gmoTransactionApi.BillingConfirm(confirmRequest);

			if (gmoResponseBillingConfirmation.Result == ResultCode.OK)
			{
				if (sqlAccessor != null)
				{
					new OrderService().Modify(
						orderId,
						order =>
						{
							order.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
							order.DateChanged = DateTime.Now;
							order.LastChanged = lastChanged;
						},
						UpdateHistoryAction.Insert, sqlAccessor);
				}
				else
				{
					new OrderService().Modify(
						orderId,
						order =>
						{
							order.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
							order.DateChanged = DateTime.Now;
							order.LastChanged = lastChanged;
						},
						UpdateHistoryAction.Insert);
				}

				return string.Empty;
			}
			else
			{
				return string.Join("\r\n", gmoResponseBillingConfirmation.Errors.Error.Select(response => string.Format("{0}:{1}", response.ErrCode, response.ErrorMessage)).ToArray());
			}
		}

		/// <summary>
		/// 返品・交換処理後の最終注文取得
		/// </summary>
		/// <param name="orderNew">新注文</param>
		/// <param name="orderOld">古注文</param>
		/// <returns>最終注文</returns>
		public static OrderModel GetFinalOrderAfterReturnOrExchange(OrderModel orderNew, OrderModel orderOld)
		{
			var orderItemModels = new List<OrderItemModel>();
			foreach (var item in orderOld.Items)
			{
				if (orderItemModels.Exists(NewItem => NewItem.ProductId == item.ProductId && NewItem.VariationId == item.VariationId))
				{
					var itemModel = orderItemModels.First(NewItem => NewItem.ProductId == item.ProductId && NewItem.VariationId == item.VariationId);
					itemModel.ItemQuantity += item.ItemQuantity;
				}
				else
				{
					orderItemModels.Add(item);
				}
			}
			foreach (var item in orderNew.Items)
			{
				if (orderItemModels.Exists(NewItem => NewItem.ProductId == item.ProductId && NewItem.VariationId == item.VariationId))
				{
					var itemModel = orderItemModels.First(NewItem => NewItem.ProductId == item.ProductId && NewItem.VariationId == item.VariationId);
					itemModel.ItemQuantity += item.ItemQuantity;
				}
				else
				{
					orderItemModels.Add(item);
				}
			}
			orderItemModels = orderItemModels.Where(item => item.ItemQuantity > 0).ToList();
			foreach (var item in orderItemModels)
			{
				item.ItemPrice = item.ItemQuantity * item.ProductPrice;
			}

			orderNew.Items = orderItemModels.ToArray();
			orderNew.Shippings[0].Items = orderNew.Items;

			var taxRate = DomainFacade.Instance.OrderService.GetTaxRateIncludeReturnExchange(orderNew.OrderIdOrg);

			orderNew.OrderPriceByTaxRates[0].TaxPriceByRate = taxRate[0].TaxPriceByRate + orderNew.OrderPriceByTaxRates[0].TaxPriceByRate;
			orderNew.OrderPriceByTaxRates[0].PriceTotalByRate = taxRate[0].PriceTotalByRate + orderNew.OrderPriceByTaxRates[0].PriceTotalByRate;

			orderNew.OrderPriceByTaxRates[1].TaxPriceByRate = taxRate[1].TaxPriceByRate + orderNew.OrderPriceByTaxRates[1].TaxPriceByRate;
			orderNew.OrderPriceByTaxRates[1].PriceTotalByRate = taxRate[1].PriceTotalByRate + orderNew.OrderPriceByTaxRates[1].PriceTotalByRate;
			return orderNew;
		}

		/// <summary>
		/// 入力された郵便番号が配送不可エリアに指定されているかチェック
		/// </summary>
		/// <param name="unavailableShippingZip">配送不可エリア郵便番号</param>
		/// <param name="userZip">ユーザーの郵便番号</param>
		/// <returns>配送不可エリアに指定されているかどうか</returns>
		public static bool CheckUnavailableShippingArea(string unavailableShippingZip, string userZip)
		{
			if (string.IsNullOrEmpty(unavailableShippingZip)) return false;
			return unavailableShippingZip.Contains(userZip);
		}

		/// <summary>
		/// 配送不可エリアエラーメッセージを表示
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="errorMessage">配送不可エリアエラーメッセージ</param>
		/// <param name="sOwnerZipError">注文者情報郵便番号エラーメッセージHtmlControler</param>
		/// <param name="sShippingZipError">配送先情報郵便番号エラーメッセージHtmlControler</param>
		/// <returns>配送不可エリアエラーの場合、エラーメッセージw表示</returns>
		public static void ShowUnavailableShippingErrorMessage(
			CartObjectList cart,
			string errorMessage,
			WrappedHtmlGenericControl sOwnerZipError,
			WrappedHtmlGenericControl sShippingZipError)
		{
			// 配送区分でエラーメッセージ表示場所を帰る
			switch (cart.Items[0].Shippings[0].ShippingAddrKbn)
			{
				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
					sShippingZipError.InnerText = HtmlSanitizer.HtmlEncode(errorMessage);
					break;

				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
					sOwnerZipError.InnerText = HtmlSanitizer.HtmlEncode(errorMessage);
					break;
			}
		}

		/// <summary>
		/// 割引や調整金額の計算で端数があり頒布会定額商品が含まれている場合、商品の方に端数を割り当てるかどうか
		/// </summary>
		/// <param name="weightItem">一番金額の高い商品</param>
		/// <param name="weightBox">一番金額の高い頒布会（定額）</param>
		/// <returns>商品に割り当てる場合true</returns>
		private static bool IsAddFractionPriceToProduct(OrderItemModel weightItem, KeyValuePair<string, decimal> weightBox)
		{
			var hasItem = weightItem != null;
			var hasSubscriptionBox = weightBox.Key != null;
			var isItemPriceHigher = hasItem
				&& hasSubscriptionBox
				&& (weightItem.PriceSubtotalAfterDistribution > weightBox.Value);
			var result = (hasItem && (isItemPriceHigher || (hasSubscriptionBox == false)));
			return result;
		}

		/// <summary>
		/// Is store pickup order
		/// </summary>
		/// <param name="realStoreId">Real store id</param>
		/// <returns>If order is store pickup</returns>
		public static bool IsStorePickupOrder(string realStoreId)
		{
			var result = Constants.STORE_PICKUP_OPTION_ENABLED
				&& (string.IsNullOrEmpty(realStoreId) == false);

			return result;
		}

		/// <summary>
		/// 台湾EcPayコンビニ受取オプション有効か
		/// </summary>
		/// <remarks>台湾ファミマ受取・台湾EcPay両方のOPTIONがONの場合EcPayコンビニ受取が適用される</remarks>
		public static bool IsReceivingStoreTwEcPayCvsOptionEnabled()
		{
			var result = (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
				&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED);
			return result;
		}
	}
}
