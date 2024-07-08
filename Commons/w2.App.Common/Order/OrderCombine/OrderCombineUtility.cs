/*
=========================================================================================================
  Module      : 注文同梱ユーティリティクラス(OrderCombineUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.DataCacheController;
using w2.App.Common.Mail;
using w2.App.Common.Option;
using w2.Common.Extensions;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.Order.OrderCombine
{
	/// <summary>
	/// 注文同梱ユーティリティクラス
	/// </summary>
	public class OrderCombineUtility
	{
		/// <summary>
		/// 注文同梱可能な注文情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shippingType">配送種別ID</param>
		/// <param name="cart">カート</param>
		/// <param name="isFront">フロントか(TRUEの場合はフロント、FALSEの場合管理画面</param>
		/// <param name="isCombinableAmazonPay">決済種別がAmazon Payの親注文と同梱できるか</param>
		/// <returns>注文情報</returns>
		public static OrderModel[] GetCombinableParentOrders(
			string shopId,
			string userId,
			string shippingType,
			CartObject cart,
			bool isFront,
			bool isCombinableAmazonPay)
		{
			var orderCombineSettings = new OrderCombineSettings(isFront);

			var models = new OrderService().GetCombinableOrder(
				shopId,
				userId,
				shippingType,
				orderCombineSettings,
				GetPossibleCombinePaymentIds(cart, userId, isFront, isCombinableAmazonPay),
				Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION,
				cart.HasFixedPurchase);

			var orderedOrders = models.OrderBy(m => m.Shippings[0].ShippingDate)
				.ThenBy(m => m.Shippings[0].ShippingTime)
				.ToArray();

			if (isFront == false)
			{
				orderedOrders = orderedOrders
					.Where(m => (m.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
					.ToArray();
			}

			return orderedOrders;
		}

		/// <summary>
		/// 注文同梱可能な注文数取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shippingType">配送種別ID</param>
		/// <param name="cart">カート</param>
		/// <param name="isFront">フロントか(TRUEの場合はフロント、FALSEの場合管理画面</param>
		/// <param name="isCombinableAmazonPay">決済種別がAmazon Payの親注文と同梱できるか</param>
		/// <returns>同梱可能注文数</returns>
		public static int GetCombinableOrderCount(string userId, string shippingType, CartObject cart, bool isFront, bool isCombinableAmazonPay)
		{
			var orderCombineSettings = new OrderCombineSettings(isFront);

			var count = new OrderService().GetCombineOrderCount(
				userId,
				shippingType,
				orderCombineSettings,
				GetPossibleCombinePaymentIds(cart, userId, isFront, isCombinableAmazonPay),
				Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION,
				cart.HasFixedPurchase);

			return count;
		}

		/// <summary>
		/// 全ユーザの注文同梱可能な注文情報取得(管理画面用)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="startRowNum">取得開始行番号</param>
		/// <param name="endRowNum">取得終了行番号</param>
		/// <returns>注文情報</returns>
		public static OrderModel[] GetCombinableParentOrderWithCondition(
			string shopId,
			string userId,
			string lastChanged,
			int startRowNum,
			int endRowNum)
		{
			var orderCombineSettings = new OrderCombineSettings(false);

			var models = new OrderService().GetCombinableParentOrderWithCondition(
				shopId,
				orderCombineSettings,
				Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION,
				userId,
				lastChanged,
				startRowNum,
				endRowNum);

			var orderedOrders = models
				.Where(m => (m.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
				.Where(m => m.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET)
				.Where(m => m.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ATM)
				.OrderBy(m => m.Shippings[0].ShippingDate)
				.ThenBy(m => m.Shippings[0].ShippingTime).ToArray();

			return orderedOrders;
		}

		/// <summary>
		/// 注文同梱可能な注文数取得(管理画面用)
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>同梱可能注文数</returns>
		public static int GetCombinableParentOrderWithConditionCount(string userId, string lastChanged)
		{
			var orderCombineSettings = new OrderCombineSettings(false);

			var count = new OrderService().GetCombinableParentOrderWithConditionCount(
				orderCombineSettings,
				Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION,
				userId,
				lastChanged);

			return count;
		}

		/// <summary>
		/// 指定した注文に対し注文同梱可能な注文情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文情報</returns>
		public static OrderModel[] GetCombinableChildOrders(string orderId)
		{
			var orderCombineSettings = new OrderCombineSettings(false);
			var order = new OrderService().Get(orderId);

			var models = new OrderService().GetCombinableChildOrder(
				order.ShopId,
				order.UserId,
				order.ShippingId,
				orderCombineSettings,
				Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION,
				order.IsFixedPurchaseOrder,
				orderId,
				order.OrderPaymentKbn);

			// 指定注文IDを除き、配送日時でソート
			var orderedOrders = models
				.Where(m => (m.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
				.OrderBy(m => m.Shippings[0].ShippingDate)
				.ThenBy(m => m.Shippings[0].ShippingTime)
				.ToArray();

			return orderedOrders;
		}

		/// <summary>
		/// 注文同梱可能な決済種別を取得
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="isFront">フロント画面か TRUEの場合フロント画面、FALSEの場合管理画面</param>
		/// <param name="isCombinableAmazonPay">決済種別がAmazon Payの親注文と同梱できるか</param>
		/// <returns>決済種別</returns>
		private static string[] GetPossibleCombinePaymentIds(CartObject cart, string userId, bool isFront, bool isCombinableAmazonPay = false)
		{
			var orderCombineSettings = new OrderCombineSettings(isFront);

			// カートがない場合、設定された注文同梱可能な決済種別を返す
			if (cart == null) return orderCombineSettings.AllowPaymentKbn;

			// カートの金額がゼロの場合、下限金額チェックなしを設定
			var isCheckUsablePriceMin = (cart.PriceCartTotalWithoutPaymentPrice > 0m);

			var validPaymentList = OrderCommon.GetValidPaymentList(cart, userId, isCheckUsablePriceMin);
			var validPaymentIdList = validPaymentList
				.Select(payment => StringUtility.ToEmpty(payment.PaymentId))
				.Where(id => (cart.Items.Any(item => item.LimitedPaymentIds.Contains(id)) == false))
				.Where(id => ((isFront == false) || (OrderCommon.CheckPaymentYamatoKaSms(id) == false)))
				.Where(id => (isCombinableAmazonPay || (id != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) && (id != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)))
				.Where(id => id != Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET)
				.Where(id => id != Constants.FLG_PAYMENT_PAYMENT_ID_ATM)
				.ToArray();

			var possibleCombinePaymentId = Array.FindAll<string>(validPaymentIdList, ((IList<string>)orderCombineSettings.AllowPaymentKbn).Contains);

			// カート商品と設定された注文同梱可能な決済種別で一致するものがない場合、空文字の配列をセット
			if (possibleCombinePaymentId.Length == 0) possibleCombinePaymentId = new string[1] { "" };

			return possibleCombinePaymentId;
		}

		/// <summary>
		/// 注文同梱可能な注文が存在するか
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="cartList">カートリスト</param>
		/// <param name="isFront">フロントか(TRUEの場合はフロント、FALSEの場合管理画面</param>
		/// <param name="isCombinableAmazonPay">決済種別がAmazon Payの親注文と同梱できるか</param>
		/// <returns>注文同梱可否</returns>
		public static bool ExistCombinableOrder(string userId, CartObjectList cartList, bool isFront, bool isCombinableAmazonPay = false)
		{
			// ゲストの場合は注文同梱不可
			if (userId == null) return false;

			// 複数カートは注文同梱不可
			if (cartList.Items.Count != 1) return false;

			// 注文同梱可能な注文があるか
			var count = GetCombinableOrderCount(userId, cartList.Items[0].ShippingType, cartList.Items[0], isFront, isCombinableAmazonPay);

			return (count > 0);
		}

		/// <summary>
		/// 注文が注文同梱可能か
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="isFront">フロント画面か TRUEの場合フロント画面、FALSEの場合管理画面</param>
		/// <returns>注文同梱可能の場合TRUE、注文同梱不可の場合FALSE</returns>
		public static string IsPossibleToOrderCombine(string orderId, bool isFront)
		{
			var orderCombineSettings = new OrderCombineSettings(isFront);
			var order = new OrderService().Get(orderId);

			// 注文が未キャンセルで、かつ入金ステータスが入金確認待ちの場合のみ注文同梱可
			var errMsg = new StringBuilder();
			if (order.IsCancelled) errMsg.Append("注文同梱元注文は既にキャンセル済みのため、注文同梱に失敗しました。\r\n");

			if ((orderCombineSettings.AllowOrderStatus.Any(status => status == order.OrderStatus)) == false)
			{
				errMsg.Append("注文同梱元注文の注文ステータスが");
				foreach (var aos in orderCombineSettings.AllowOrderStatus)
				{
					if (aos == Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
						errMsg.Append(ValueText.GetValueText(
							Constants.TABLE_ORDER,
							Constants.FIELD_ORDER_ORDER_STATUS,
							Constants.FLG_ORDER_ORDER_STATUS_ORDERED));
					if (aos == Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED)
						errMsg.Append(ValueText.GetValueText(
							Constants.TABLE_ORDER,
							Constants.FIELD_ORDER_ORDER_STATUS,
							Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED));
					if (aos == Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED)
						errMsg.Append(ValueText.GetValueText(
							Constants.TABLE_ORDER,
							Constants.FIELD_ORDER_ORDER_STATUS,
							Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED));
				}
				errMsg.Append("以外のため、注文同梱に失敗しました。\r\n");
			}

			if ((orderCombineSettings.AllowPaymentKbn.Any(kbn => kbn == order.OrderPaymentKbn)) == false)
			{
				errMsg.Append("注文同梱元注文の決済種別が");
				foreach (var apk in orderCombineSettings.AllowPaymentKbn)
				{
					errMsg.Append("「" + apk + "」");
				}
				errMsg.Append("以外のため、注文同梱に失敗しました。\r\n");
			}

			if ((orderCombineSettings.AllowOrderPaymentStatus.Any(status => status == order.OrderPaymentStatus)) == false)
			{
				errMsg.Append("注文同梱元注文の入金ステータスが");
				foreach (var aops in orderCombineSettings.AllowOrderPaymentStatus)
				{
					if (aops == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM)
						errMsg.Append(ValueText.GetValueText(
							Constants.TABLE_ORDER,
							Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS,
							Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM));
					if (aops == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)
						errMsg.Append(ValueText.GetValueText(
							Constants.TABLE_ORDER,
							Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS,
							Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE));
					if (aops == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_SHORTAGE)
						errMsg.Append(ValueText.GetValueText(
							Constants.TABLE_ORDER,
							Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS,
							Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_SHORTAGE));
				}
				errMsg.Append("以外のため、注文同梱に失敗しました。\r\n");
			}

			if ((order.OrderDate >= DateTime.Parse(DateTime.Now.AddDays(-orderCombineSettings.AllowOrderDayPassed).ToString("yyyy/MM/dd 00:00:00"))) == false)
			{
				errMsg.Append(string.Format("注文同梱元注文の注文日が{0}日前のため、注文同梱に失敗しました。\r\n", orderCombineSettings.AllowOrderDayPassed));
			}

			if (((orderCombineSettings.AllowShippingDayBefore < 0)
				|| (order.Shippings[0].ShippingDate >= DateTime.Parse(DateTime.Now.AddDays(orderCombineSettings.AllowShippingDayBefore).ToString("yyyy/MM/dd 00:00:00"))))
				== false)
			{
				errMsg.Append(string.Format("注文同梱元注文の配送希望日が{0}日前のため、注文同梱に失敗しました。\r\n", orderCombineSettings.AllowShippingDayBefore));
			}

			return errMsg.ToString();
		}

		/// <summary>
		/// 注文同梱後カート情報作成
		/// </summary>
		/// <param name="parentOrder">親注文情報</param>
		/// <param name="childCart">子注文カート情報</param>
		/// <param name="combineCart">注文同梱後カート情報</param>
		/// <returns>エラーメッセージ</returns>
		public static string CreateCombinedCart(OrderModel parentOrder, CartObject childCart, out CartObject combineCart)
		{
			combineCart = null;

			// 親注文と子注文の配送種別が異なる場合処理中止
			if (parentOrder.ShippingId != childCart.ShippingType) return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ORDERCOMBINE_SHIPPING_TYPE_DISAGREEMENT);

			// 親注文情報の商品が削除済みの場合処理中止
			foreach (var item in parentOrder.Shippings[0].Items)
			{
				var dv = ProductCommon.GetProductVariationInfo(item.ShopId, item.ProductId, item.VariationId, parentOrder.MemberRankId);
				if (dv.Count == 0) return OrderCommon.GetErrorMessage(OrderErrorcode.ProductDeleted, item.VariationId);
			}

			// 注文同梱用カート作成
			var fixedPurchaseId = parentOrder.FixedPurchaseId;
			var fixedPurchase = (string.IsNullOrEmpty(fixedPurchaseId) == false) ? new FixedPurchaseService().Get(fixedPurchaseId) : null;
			combineCart = CartObject.CreateCartForOrderCombine(parentOrder, fixedPurchase);
			combineCart.FixedPurchase = fixedPurchase;

			// 同梱後の定期購入割引額を設定
			combineCart.FixedPurchaseDiscount = parentOrder.FixedPurchaseDiscountPrice + childCart.FixedPurchaseDiscount;

			// 定期会員判定は子注文カートから引き継ぐ
			combineCart.IsFixedPurchaseMember = childCart.IsFixedPurchaseMember;

			// 親注文IDを設定
			combineCart.OrderCombineParentOrderId = parentOrder.OrderId;
			combineCart.CombineParentOrderFixedPurchaseOrderCount = parentOrder.FixedPurchaseOrderCount ?? 0;

			// 注文同梱用カートに親注文・子カートが定期情報を持っていたか設定
			combineCart.IsCombineParentOrderHasFixedPurchase = (string.IsNullOrEmpty(fixedPurchaseId) == false);
			combineCart.IsBeforeCombineCartHasFixedPurchase = childCart.HasFixedPurchase;
			combineCart.IsOrderCombinedWithRegisteredSubscription = childCart.HasFixedPurchase == false;

			// 親注文の定期購入区分・購入設定を注文同梱用カートに設定
			if (string.IsNullOrEmpty(fixedPurchaseId) == false)
			{
				var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(combineCart.ShippingType);
				combineCart.Shippings[0].UpdateFixedPurchaseSetting(
					fixedPurchase.FixedPurchaseKbn,
					fixedPurchase.FixedPurchaseSetting1,
					shopShipping.FixedPurchaseShippingDaysRequired,
					shopShipping.FixedPurchaseMinimumShippingSpan);
			}

			// 頒布会を新規登録するかを設定（コースIDなどの情報は子注文のものを保持）
			combineCart.IsShouldRegistSubscriptionForCombine =
				childCart.IsSubscriptionBox && (childCart.SubscriptionBoxCourseId != combineCart.SubscriptionBoxCourseId);

			if ((combineCart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
				|| (combineCart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				|| (combineCart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
			{
				combineCart.Payment.ExternalPaymentType = parentOrder.ExternalPaymentType;
			}

			combineCart = CreateCombinedCart(
				combineCart,
				childCart,
				parentOrder.AdvcodeFirst,
				parentOrder.AdvcodeNew,
				parentOrder.Shippings[0].ScheduledShippingDate);

			return "";
		}

		/// <summary>
		/// 注文メモのマージ
		/// </summary>
		/// <param name="memo1">メモ1</param>
		/// <param name="memo2">メモ2</param>
		/// <returns>マージ後のメモ</returns>
		private static string JoinMemo(string memo1, string memo2)
		{
			var separater = ((string.IsNullOrEmpty(memo1)) || (string.IsNullOrEmpty(memo2))) ? "" : "\r\n";
			var mergedMemo = StringUtility.ToEmpty(memo1) + separater + StringUtility.ToEmpty(memo2);

			return mergedMemo;
		}

		/// <summary>
		/// 注文同梱後カートへ子カート商品を追加する(併せて同梱物を削除)
		/// </summary>
		/// <param name="combineCart">注文同梱後カート</param>
		/// <param name="childCartProducts">子カート商品</param>
		/// <param name="ignoreChildFixedPurchase">子カートの定期注文を通常注文として扱うか</param>
		private static void CombineCartProduct(CartObject combineCart, List<CartProduct> childCartProducts, bool ignoreChildFixedPurchase)
		{
			// 親注文の同梱物を削除
			foreach (var item in combineCart.Items.Where(item => (item.ProductBundleId != "")).ToArray())
			{
				combineCart.Items.Remove(item);
			}

			// 注文同梱後カート商品に注文同梱元注文IDをセット
			foreach (var combineCartProduct in combineCart.Items)
			{
				combineCartProduct.OrderCombineOrgOrderId = combineCart.OrderCombineParentOrderId;
				combineCartProduct.IsOrderCombine = true;
			}

			foreach (var childProduct in childCartProducts)
			{
				// 商品同梱のものは追加しない(再度同梱を行うため)
				if (childProduct.ProductBundleId != "") continue;

				var combineTargetParentProduct = combineCart.Items.FirstOrDefault(ExistProductInCombinedParentOrder);
				if (combineTargetParentProduct != null)
				{
					combineTargetParentProduct.Count += childProduct.Count;
					combineTargetParentProduct.CountSingle += childProduct.CountSingle;
					combineTargetParentProduct.AddedQuantitySingleByOrderCombine = childProduct.CountSingle;
					if (string.IsNullOrEmpty(childProduct.FixedPurchaseDiscountType) == false)
					{
						combineTargetParentProduct.FixedPurchaseDiscountType = childProduct.FixedPurchaseDiscountType;
						combineTargetParentProduct.FixedPurchaseDiscountValue = childProduct.FixedPurchaseDiscountValue;
					}
				}
				else
				{
					var addedProduct = combineCart.AddVirtural(childProduct, false);
					addedProduct.IsOrderCombine = true;
				}

				// 購入制限チェック対象商品に子注文の商品をセット
				combineCart.TargetProductListForCheckProductOrderLimit.Add(childProduct);

				// 同梱親注文に対象の商品が存在するか
				bool ExistProductInCombinedParentOrder(CartProduct parentProduct)
				{
					return (parentProduct.ShopId == childProduct.ShopId)
						&& (parentProduct.ProductId == childProduct.ProductId)
						&& (parentProduct.VariationId == childProduct.VariationId)
						&& (parentProduct.ProductOptionSettingList?.GetDisplayProductOptionSettingSelectValues()
							== childProduct.ProductOptionSettingList?.GetDisplayProductOptionSettingSelectValues())
						&& (ignoreChildFixedPurchase || (parentProduct.IsFixedPurchase == childProduct.IsFixedPurchase))
						&& (parentProduct.ProductSaleId == childProduct.ProductSaleId)
						&& (parentProduct.SubscriptionBoxCourseId == childProduct.SubscriptionBoxCourseId);
				}
			}

			// 商品の価格を最新のものに更新する
			var isFirst = (combineCart.CombineParentOrderFixedPurchaseOrderCount <= 1);
			var isFixedPurchaseProductCountOption = (Constants.FIXEDPURCHASE_ORDER_DISCOUNT_METHOD == Constants.FLG_FIXEDPURCHASE_PRODUCT_COUNT);
			var service = DomainFacade.Instance.OrderService;
			foreach (var cartProduct in combineCart.Items)
			{
				if (isFixedPurchaseProductCountOption)
				{
					// 親注文の定期商品購入回数を取得
					var fixedPurchaseOrderItemCount = combineCart.OrderCombineParentOrderId.Split(',')
						.Max(combineOrderId => service.GetFixedPurchaseItemOrderCount(
							combineOrderId,
							cartProduct.ProductId,
							cartProduct.VariationId));
					isFirst = (fixedPurchaseOrderItemCount <= 1);
				}
				var dv = ProductCommon.GetProductVariationInfo(
					cartProduct.ShopId,
					cartProduct.ProductId,
					cartProduct.VariationId,
					combineCart.MemberRankId);

				// 親注文が通常注文の場合、同梱カートは通常注文として扱う
				if (ignoreChildFixedPurchase)
				{
					// 頒布会商品の場合は、頒布会コースIDも消す（そうでないと価格更新処理内で定期購入価格が設定されるため）
					if (cartProduct.AddCartKbn == Constants.AddCartKbn.SubscriptionBox)
					{
						cartProduct.SubscriptionBoxCourseId = string.Empty;
					}

					cartProduct.AddCartKbn = Constants.AddCartKbn.Normal;
				}

				cartProduct.SetPrice(dv[0], isFirst);
			}
		}

		/// <summary>
		/// 親注文から定期商品購入回数を取得
		/// </summary>
		/// <param name="parentOrderItems">親注文商品</param>
		/// <param name="cartProduct">カート内商品</param>
		/// <returns>定期商品購入回数</returns>
		public int GetFixedPurchaseItemOrderCountForParentOrder(IEnumerable<OrderItemModel> parentOrderItems, CartProduct cartProduct)
		{
			var parentItem = parentOrderItems
				.FirstOrDefault(i => ((i.ProductId == cartProduct.ProductId)
					&& (i.VariationId == cartProduct.VariationId)));
			var itemOrderCount = (parentItem == null)
				? 1
				: (parentItem.FixedPurchaseItemOrderCount ?? 1);
			return itemOrderCount;
		}

		/// <summary>
		///注文同梱エラー用メール送信
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="userId">ユーザID(ログ用)</param>
		/// <param name="message">メール本文</param>
		private static void SendOrderCombineErrorMail(string orderId, string userId, string message)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID, orderId},
				{"message", message}
			};

			using (var mailSender = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, Constants.CONST_MAIL_ID_ORDERCOMBINE_ERROR_FOR_OPERATOR, userId, ht))
			{
				// メールテンプレートでTOが指定されていない場合、メール送信しない
				if (string.IsNullOrEmpty(mailSender.TmpTo)) return;

				mailSender.SendMail();
			}
		}

		/// <summary>
		/// 注文同梱 親注文キャンセルエラーメール送信
		/// </summary>
		/// <param name="userId">ユーザID(ログ用)</param>
		/// <param name="combinedParentOrderId">注文同梱 親注文ID</param>
		/// <param name="combinedNewOrderId">注文同梱 同梱注文ID</param>
		/// <param name="parentOrderCancelErrMessage">親注文キャンセルエラーメッセージ</param>
		public static void SendOrderCombineParentOrderCancelErrorMail(string userId, string combinedParentOrderId, string combinedNewOrderId, string parentOrderCancelErrMessage)
		{
			var mailMessage = "注文同梱の親注文のキャンセルに失敗しました。\r\n"
			+ "注文同梱は成功し同梱後の注文は作られています。\r\n"
			+ "  親注文ID：" + combinedParentOrderId + "\r\n"
			+ "  同梱後注文ID：" + combinedNewOrderId + "\r\n"
			+ "\r\n"
			+ "親注文キャンセルエラーメッセージ："
			+ parentOrderCancelErrMessage;

			SendOrderCombineErrorMail(combinedParentOrderId, userId, mailMessage);
		}

		/// <summary>
		/// 注文同梱 注文時同梱フラグ(拡張ステータス)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="flg">フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void UpdateOrderCombineFlg(
			string orderId,
			bool flg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			new OrderService().UpdateOrderExtendStatus(
				orderId,
				33,
				(flg) ? Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON : Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF,
				DateTime.Now.Date,
				lastChanged,
				updateHistoryAction,
				accessor);
		}

		/// <summary>
		/// 注文同梱 注文時同梱キャンセルフラグ(拡張ステータス)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="flg">フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void UpdateOrderCombineCancelFlg(
			string orderId,
			bool flg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			new OrderService().UpdateOrderExtendStatus(
				orderId,
				34,
				(flg) ? Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON : Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF,
				DateTime.Now.Date,
				lastChanged,
				updateHistoryAction,
				accessor);
		}

		/// <summary>
		/// 注文同梱 管理者同梱フラグ(拡張ステータス)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="flg">フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void UpdateManagerOrderCombineFlg(
			string orderId,
			bool flg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			new OrderService().UpdateOrderExtendStatus(
				orderId,
				35,
				(flg) ? Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON : Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF,
				DateTime.Now.Date,
				lastChanged,
				updateHistoryAction,
				accessor);
		}

		/// <summary>
		/// 注文同梱元注文キャンセル失敗フラグ(拡張ステータス46)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="flg">フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void UpdateOrderCombineCancelFailedFlg(
			string orderId,
			bool flg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			new OrderService().UpdateOrderExtendStatus(
				orderId,
				46,
				flg ? Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON : Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF,
				DateTime.Now,
				lastChanged,
				updateHistoryAction,
				accessor);
		}

		/// <summary>
		/// 注文同梱 管理者同梱フラグ(拡張ステータス)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="flg">フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public static void UpdateManagerOrderCombineFlg(
			string orderId,
			bool flg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateManagerOrderCombineFlg(orderId, flg, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
			}
		}
		/// <summary>
		/// 注文同梱 管理者同梱キャンセルフラグ(拡張ステータス)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="flg">フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public static int UpdateManagerOrderCombineCancelFlg(
			string orderId,
			bool flg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var count = new OrderService().UpdateOrderExtendStatus(
				orderId,
				36,
				(flg) ? Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON : Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF,
				DateTime.Now.Date,
				lastChanged,
				updateHistoryAction,
				accessor);

			return count;
		}

		/// <summary>
		/// 注文同梱 注文同梱済み注文キャンセル
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="isFront">フロント画面からの操作か(falseは管理画面からの操作)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">アクセサ</param>
		/// <param name="orderCombineCoupon">注文同梱で生成された新注文が利用するクーポン</param>
		/// <param name="disablePaymentCancelOrderId">外部決済キャンセルAPIをスキップする受注ID</param>
		/// <returns>エラーメッセージ</returns>
		public static string OrderCancelForOrderCombine(
			string shopId,
			string orderId,
			string userId,
			bool isFront,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			CartCoupon orderCombineCoupon,
			string disablePaymentCancelOrderId = "")
		{
			var dvOrder = new OrderService().GetOrderForOrderCancel(orderId, accessor);
			if (dvOrder == null) return "注文同梱元注文が見つからずキャンセルに失敗しました。";
			var order = new OrderModel(dvOrder[0]);
			order.LastChanged = lastChanged;
			if (order.IsCancelled) return "注文同梱元注文は既にキャンセル済みのためキャンセルに失敗しました。";

			// 注文同梱元親注文キャンセル付随処理
			OrderCommon.CancelOrderSubProcessForOrderCombine(
				dvOrder[0],
				true,
				Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_ORDER_CANCEL_FOR_COMBINE,
				shopId,
				lastChanged,
				true,
				UpdateHistoryAction.DoNotInsert,
				orderCombineCoupon,
				accessor);

			// 注文同梱元親注文ステータス更新
			using (var statement = new SqlStatement("Order", "UpdateOrderStatusCancel"))
			{
				var input = new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_ID, orderId },
					{ "update_date", DateTime.Now },
					{ Constants.FIELD_ORDER_USER_ID, userId },
					{ Constants.FIELD_ORDER_LAST_CHANGED, lastChanged }
				};
				statement.ExecStatement(accessor, input);
			}

			// 注文同梱キャンセル拡張ステータス更新
			if (isFront)
			{
				UpdateOrderCombineCancelFlg(orderId, true, lastChanged, UpdateHistoryAction.DoNotInsert, accessor);
			}
			else
			{
				UpdateManagerOrderCombineCancelFlg(orderId, true, lastChanged, UpdateHistoryAction.DoNotInsert, accessor);
			}

			// 外部決済以外の場合、処理終了
			var errorMessage = "";
			var isExternalPayment = OrderCommon.IsExternalPayment(order.OrderPaymentKbn);
			if (isExternalPayment && (orderId != disablePaymentCancelOrderId))
			{
				// 注文同梱元親注文 外部決済キャンセル
				var errorMessageTmp = OrderCommon.CancelExternalCooperationPayment(order, accessor);
				if (string.IsNullOrEmpty(errorMessageTmp) == false)
				{
					errorMessage = "外部決済キャンセルに失敗しました。\r\n" + errorMessageTmp;
				}
				else
				{
					// キャンセル向け外部決済ステータス系＆メモ更新
					OrderCommon.UpdateExternalPaymentStatusesAndMemoForCancel(
						orderId,
						lastChanged,
						UpdateHistoryAction.DoNotInsert,
						accessor);
				}
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertAllForOrder(orderId, lastChanged, accessor);
			}
			return errorMessage;
		}

		/// <summary>
		/// 注文同梱 注文同梱済み注文キャンセル(複数注文) コミットあり
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="combinedOrderIds">同梱済み注文ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="isFront">フロント画面からの操作か(falseは管理画面からの操作)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="newOrderId">同梱した受注ID</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="orderCombineCoupon">注文同梱で生成された新注文が利用するクーポン</param>
		/// <param name="disablePaymentCancelOrderId">外部決済キャンセルAPIをスキップする受注ID</param>
		/// <returns>エラーメッセージ</returns>
		public static string OrdersCancelForOrderCombineWithCommit(
			string shopId,
			string[] combinedOrderIds,
			string userId,
			bool isFront,
			string lastChanged,
			string newOrderId,
			UpdateHistoryAction updateHistoryAction,
			CartCoupon orderCombineCoupon,
			string disablePaymentCancelOrderId = "")
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 注文同梱済み注文をキャンセル
				foreach (var combinedOrderId in combinedOrderIds)
				{
					var errMessage = OrderCancelForOrderCombine(
						shopId,
						combinedOrderId,
						userId,
						isFront,
						lastChanged,
						updateHistoryAction,
						accessor,
						orderCombineCoupon,
						disablePaymentCancelOrderId);

					if (errMessage != "")
					{
						accessor.RollbackTransaction();
						return errMessage;
					}
				}

				// 注文同梱元親注文の購入回数更新
				var orderCount = new UserService().Get(userId, accessor).OrderCountOrderRealtime;
				var ht = new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_ID, newOrderId },
					{ Constants.FIELD_ORDER_ORDER_COUNT_ORDER, (orderCount - combinedOrderIds.Length) },
					{ Constants.FIELD_ORDER_LAST_CHANGED, lastChanged },
					{ Constants.FIELD_ORDER_USER_ID,userId },
					{ Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME, orderCount},
					{ "cancelCount",  combinedOrderIds.Length}
				};

				// 注文同梱元親注文の購入回数更新
				using (var statement = new SqlStatement("Order", "UpdateUserOrderCount"))
				{
					statement.ExecStatement(accessor, ht);
				}
				// ユーザーリアルタイム更新
				OrderCommon.UpdateRealTimeOrderCount(ht, Constants.FLG_REAL_TIME_ORDER_COUNT_ACTION_COMBINE, accessor);

				accessor.CommitTransaction();
				return "";
			}
		}

		/// <summary>
		/// 注文同梱完了メール送信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		/// <param name="targetPcMail">PCメール向けか</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		private static void SendOrderCombineCompleteMailToUser(Hashtable order, CartObject cart, bool targetPcMail, string languageCode = null, string languageLocaleId = null)
		{
			var mailAddr = targetPcMail ? cart.Owner.MailAddr : cart.Owner.MailAddr2;

			var input = new MailTemplateDataCreaterByCartAndOrder(targetPcMail).GetOrderMailDatas(order, cart, true);
			input[Constants.FIELD_ORDER_MEMO] = ((string)input[Constants.FIELD_ORDER_MEMO]).Trim();

			using (MailSendUtility mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_ORDERCOMBINE_ORDER_COMPLETE,
				(string)order[Constants.FIELD_ORDER_USER_ID],
				input,
				languageCode,
				languageLocaleId,
				StringUtility.ToEmpty(mailAddr)))
			{
				mailSender.AddTo(StringUtility.ToEmpty(mailAddr));
				mailSender.SendMail();
			}
		}

		/// <summary>
		/// 注文同梱完了メール送信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="cart">カート</param>
		public static void SendOrderCombineCompleteMailToUser(Hashtable order, CartObject cart)
		{
			// PC/MOBILE両方送信？
			if (Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED)
			{
				if (StringUtility.ToEmpty(cart.Owner.MailAddr) != "") SendOrderCombineCompleteMailToUser(order, cart, true, cart.Owner.DispLanguageCode, cart.Owner.DispLanguageLocaleId);
				if (StringUtility.ToEmpty(cart.Owner.MailAddr2) != "") SendOrderCombineCompleteMailToUser(order, cart, false);
			}
			else
			{
				SendOrderCombineCompleteMailToUser(order, cart, true, cart.Owner.DispLanguageCode, cart.Owner.DispLanguageLocaleId);
			}
		}

		/// <summary>
		/// カート商品の注文同梱での変更内容を取得
		/// </summary>
		/// <param name="cartProduct">カート商品</param>
		/// <returns>注文同梱での変更内容</returns>
		public static string GetCartProductChangesByOrderCombine(CartProduct cartProduct)
		{
			// 注文同梱されてない場合、空文字
			if (cartProduct.IsOrderCombine == false) return "";

			var productChanges = "";
			if (string.IsNullOrEmpty(cartProduct.OrderCombineOrgOrderId) == false)
			{
				productChanges = "同梱元注文ID：" + cartProduct.OrderCombineOrgOrderId;
				productChanges += (cartProduct.AddedQuantitySingleByOrderCombine > 0)
					? "[数量：+" + cartProduct.AddedQuantitySingleByOrderCombine.ToString() + "]"
					: "";
			}
			else
			{
				productChanges = "[追加商品]";
			}

			return productChanges;
		}

		/// <summary>
		/// 注文同梱元注文情報取得
		/// </summary>
		/// <param name="combinedOrgOrderIds">注文同梱元注文ID(カンマ区切り)</param>
		/// <returns>注文情報</returns>
		public static OrderModel[] GetCombinedOrders(string combinedOrgOrderIds)
		{
			if (string.IsNullOrEmpty(combinedOrgOrderIds)) return null;

			var orderIds = combinedOrgOrderIds.Split(',');
			var orders = orderIds.Select(id => new OrderService().Get(id)).ToArray();

			return orders;
		}

		/// <summary>
		/// 注文同梱カート作成
		/// </summary>
		/// <param name="combineCart">注文同梱用カート(親注文カート)</param>
		/// <param name="childCart">子注文カート</param>
		/// <param name="advCodeFirst">初回広告コード</param>
		/// <param name="advCodeNew">最新広告コード</param>
		/// <param name="scheduledShippingDate">出荷予定日</param>
		/// <returns>注文同梱済みカート</returns>
		public static CartObject CreateCombinedCart(
			CartObject combineCart,
			CartObject childCart,
			string advCodeFirst,
			string advCodeNew,
			DateTime? scheduledShippingDate)
		{
			// 注文同梱用カートへ子カート商品を同梱
			// 親：既存通常注文 + 子：既存定期注文の場合、同梱後の注文を通常注文として扱う(定期割引は適用しない)ため、子注文の商品を通常注文商品として扱う
			var ignoreChildFixedPurchase = ((combineCart.FixedPurchase == null) && (childCart.FixedPurchase != null));
			CombineCartProduct(combineCart, childCart.Items, ignoreChildFixedPurchase);

			// 配送方法がメール便の場合、メール便のまま配送可能かチェックし、配送できない場合は宅配便に変更する(商品同梱分も考慮する)
			if (combineCart.Shippings[0].ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
			{
				var fixedPurchaseId = (combineCart.FixedPurchase != null)
					? combineCart.FixedPurchase.FixedPurchaseId
					: string.Empty;
				using (var productBundler = new ProductBundler(
					new List<CartObject> { combineCart },
					combineCart.CartUserId,
					advCodeFirst,
					advCodeNew))
				{
					foreach (var cart in productBundler.CartList)
					{
						if (cart.Shippings[0].IsMail == false) continue;
						if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED)
						{
							var shopShipping = new ShopShippingService().Get(cart.ShopId, cart.ShippingType);
							var deliveryCompanyId = OrderCommon.DeliveryCompanyMailEscalation(cart.Items, shopShipping.CompanyListMail);

							cart.Shippings[0].DeliveryCompanyId = (string.IsNullOrEmpty(deliveryCompanyId))
								? shopShipping.CompanyListExpress.FirstOrDefault(item => item.IsDefault).DeliveryCompanyId
								: deliveryCompanyId;
							if (string.IsNullOrEmpty(deliveryCompanyId)) combineCart.Shippings[0].ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;

						}
						else if (OrderCommon.IsAvailableShippingKbnMail(cart.Items) == false)
						{
							combineCart.Shippings[0].ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
							var shopShipping = new ShopShippingService().Get(cart.ShopId, cart.ShippingType);
							cart.Shippings[0].DeliveryCompanyId = shopShipping.CompanyListExpress.FirstOrDefault(item => item.IsDefault).DeliveryCompanyId;
						}
					}
				}
			}

			// ポイント合算
			combineCart.SetUsePoint(combineCart.UsePoint + childCart.UsePoint, combineCart.UsePointPrice + childCart.UsePointPrice);

			// 親注文 > 子注文の優先順位でクーポンを適用
			if ((combineCart.Coupon == null) && (childCart.Coupon != null)) combineCart.Coupon = childCart.Coupon;
			combineCart.IsCombineParentOrderUseCoupon = (combineCart.Coupon != null);

			// クーポン適用判定のため商品合計金額再計算
			combineCart.CalculatePriceSubTotal();

			if (combineCart.Coupon != null)
			{
				var coupons = (combineCart.IsCombineParentOrderUseCoupon)
					? new CouponService().GetAllUserCouponsFromCouponCodeIncludeUnavailable(combineCart.Coupon.DeptId, combineCart.CartUserId, combineCart.Coupon.CouponCode)
					: new CouponService().GetAllUserCouponsFromCouponCode(combineCart.Coupon.DeptId, combineCart.CartUserId, combineCart.Coupon.CouponCode);

				if (coupons.Count() == 0)
				{
					combineCart.Coupon = null;
				}
				else
				{
					// クーポン利用可能条件チェック(対象商品、最低購入金額のチェックのみ)
					var errorMessage = CouponOptionUtility.CheckCouponUseConditions(combineCart, coupons[0]);
					if (string.IsNullOrEmpty(errorMessage) == false) combineCart.Coupon = null;
				}
			}

			// 調整金額合算
			combineCart.PriceRegulation += childCart.PriceRegulation;

			// メモ マージ
			if (childCart.OrderMemos != null) combineCart.OrderMemos.AddRange(childCart.OrderMemos);
			combineCart.RegulationMemo = JoinMemo(combineCart.RegulationMemo, childCart.RegulationMemo);
			combineCart.RelationMemo = JoinMemo(combineCart.RelationMemo, childCart.RelationMemo);
			combineCart.ManagementMemo = JoinMemo(combineCart.ManagementMemo, childCart.ManagementMemo);
			combineCart.ShippingMemo = JoinMemo(combineCart.ShippingMemo, childCart.ShippingMemo);

			// 再計算
			combineCart.CalculateWithCartShipping(true);

			// 決済手数料再設定
			combineCart.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
				combineCart.ShopId,
				combineCart.Payment.PaymentId,
				combineCart.PriceSubtotal,
				combineCart.PriceCartTotalWithoutPaymentPrice);

			var payment = DataCacheControllerFacade.GetPaymentCacheController().Get(combineCart.Payment.PaymentId);
			combineCart.Payment.PaymentName = payment.PaymentName;

			// コンバージョン情報
			if ((combineCart.ContentsLog == null) && (childCart.ContentsLog != null))
			{
				combineCart.ContentsLog = childCart.ContentsLog;
			}
			combineCart.Shippings[0].ScheduledShippingDate = scheduledShippingDate;

			// 子注文の頒布会コースIDのみ保持
			combineCart.SubscriptionBoxCourseId = childCart.SubscriptionBoxCourseId;

			return combineCart;
		}

		/// <summary>
		/// 注文同梱カート作成
		/// </summary>
		/// <param name="parentOrderId">親注文ID</param>
		/// <param name="childOrderIds">子注文IDs</param>
		/// <param name="combinedCart">注文同梱カート</param>
		/// <returns>エラーメッセージ</returns>
		public static string CreateCombinedCart(string parentOrderId, string[] childOrderIds, out CartObject combinedCart)
		{
			combinedCart = null;

			var parentOrder = new OrderService().Get(parentOrderId);

			// 親注文情報の商品が削除済みの場合処理中止
			foreach (var item in parentOrder.Shippings[0].Items)
			{
				var dv = ProductCommon.GetProductVariationInfo(item.ShopId, item.ProductId, item.VariationId, parentOrder.MemberRankId);
				if (dv.Count == 0) return OrderCommon.GetErrorMessage(OrderErrorcode.ProductDeleted, item.VariationId);
			}

			// 注文同梱用カート作成
			var fixedPurchaseId = parentOrder.FixedPurchaseId;
			var fixedPurchase = (fixedPurchaseId != "") ? new FixedPurchaseService().Get(fixedPurchaseId) : null;
			var combineCart = CartObject.CreateCartForOrderCombine(parentOrder, fixedPurchase);
			combineCart.FixedPurchase = fixedPurchase;

			if (parentOrder.IsSubscriptionBoxFixedAmount)
			{
				combineCart.SubscriptionBoxCourseId = parentOrder.SubscriptionBoxCourseId;
				combineCart.SubscriptionBoxFixedAmount = parentOrder.SubscriptionBoxFixedAmount.GetValueOrDefault();
			}

			// 親注文IDを設定
			combineCart.OrderCombineParentOrderId = parentOrder.OrderId;

			// Bind parent tran id
			combineCart.OrderCombineParentTranId = parentOrder.CardTranId;

			// Set Is Order Sales Settled
			combineCart.IsOrderSalesSettled =
				(parentOrder.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED);

			// 注文同梱用カートに親注文が定期情報を持っていたか設定
			combineCart.IsCombineParentOrderHasFixedPurchase = string.IsNullOrEmpty(fixedPurchaseId) == false;

			// 子注文が複数選択可能な場合、既存注文同士の同梱になるためTRUEにする
			combineCart.IsOrderCombinedWithRegisteredSubscription = true;

			// 親注文の定期購入区分・購入設定を注文同梱用カートに設定
			if (string.IsNullOrEmpty(fixedPurchaseId) == false)
			{
				var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(combineCart.ShippingType);
				combineCart.Shippings[0].UpdateFixedPurchaseSetting(
					fixedPurchase.FixedPurchaseKbn,
					fixedPurchase.FixedPurchaseSetting1,
					shopShipping.FixedPurchaseShippingDaysRequired,
					shopShipping.FixedPurchaseMinimumShippingSpan);
			}

			combineCart.CombineParentOrderFixedPurchaseOrderCount = parentOrder.FixedPurchaseOrderCount ?? 0;

			// 同梱後の定期購入割引額を設定
			combineCart.FixedPurchaseDiscount = parentOrder.FixedPurchaseDiscountPrice;

			var orderService = new OrderService();
			var fixedPurchaseService = new FixedPurchaseService();
			foreach (var childOrderId in childOrderIds)
			{
				var childOrder = orderService.Get(childOrderId);

				// 親注文と子注文の配送種別が異なる場合処理中止
				if (parentOrder.ShippingId != childOrder.ShippingId) return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ORDERCOMBINE_SHIPPING_TYPE_DISAGREEMENT);

				// 子注文情報の商品が削除済みの場合処理中止
				foreach (var item in childOrder.Shippings[0].Items)
				{
					var dv = ProductCommon.GetProductVariationInfo(item.ShopId, item.ProductId, item.VariationId, parentOrder.MemberRankId);
					if (dv.Count == 0) return OrderCommon.GetErrorMessage(OrderErrorcode.ProductDeleted, item.VariationId);
				}

				var childOrderFp = (string.IsNullOrEmpty(childOrder.FixedPurchaseId)) ? null : fixedPurchaseService.Get(childOrder.FixedPurchaseId);
				var childCart = CartObject.CreateCartForOrderCombine(childOrder, childOrderFp);
				childCart.FixedPurchase = childOrderFp;

				combineCart.OrderCombineParentOrderId += ',' + childOrder.OrderId;
				combineCart.FixedPurchaseDiscount += childOrder.FixedPurchaseDiscountPrice;

				// 親注文が通常注文、且つ子注文が定期注文の場合、子注文の定期購入回数割引額が調整金額に入る
				if ((combineCart.IsCombineParentOrderHasFixedPurchase == false)
					&& (string.IsNullOrEmpty(childOrder.FixedPurchaseId) == false))
				{
					combineCart.PriceRegulation += (childOrder.FixedPurchaseDiscountPrice * (-1));
				}

				combineCart = CreateCombinedCart(
					combineCart,
					childCart,
					parentOrder.AdvcodeFirst,
					parentOrder.AdvcodeNew,
					parentOrder.Shippings[0].ScheduledShippingDate);
			}

			combinedCart = combineCart;
			return "";
		}

		/// <summary>
		/// 注文同梱 親注文の拡張ステータスを元に同梱済み注文の拡張ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="parentOrderId">親注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新成否 更新成功：TRUE、更新失敗：FALSE</returns>
		public static bool UpdateExtendStatusFromParentOrder(
			string orderId,
			string parentOrderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var orderService = new OrderService();
			var parentOrder = orderService.Get(parentOrderId, accessor);

			// システムで利用するところまで更新する
			for (var i = 0; i < Constants.CONST_ORDER_EXTEND_STATUS_USER_USE_MAX - 1; i++)
			{
				if (parentOrder.ExtendStatus[i].Date != null)
				{
					var count = orderService.UpdateOrderExtendStatus(
						orderId,
						parentOrder.ExtendStatus[i].ExtendStatusNo,
						parentOrder.ExtendStatus[i].Value,
						(DateTime)parentOrder.ExtendStatus[i].Date,
						lastChanged,
						updateHistoryAction,
						accessor);

					if (count == 0) return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 注文同梱 親注文の拡張ステータスを元に同梱済み注文の拡張ステータス更新 コミットあり
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="parentOrderId">親注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public static void UpdateExtendStatusFromParentOrderWithCommit(
			string orderId,
			string parentOrderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateExtendStatusFromParentOrder(orderId, parentOrderId, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
			}
		}

		/// <summary>
		/// 注文同梱 注文同梱後に定期購入割引が適用されなくなったか判定(親注文ベースで注文同梱されるため、親注文は判定対象外)
		/// </summary>
		/// <param name="combinedCart">注文同梱後カート</param>
		/// <param name="childOrderIds">注文同梱子注文ID</param>
		/// <returns>判定結果</returns>
		public static bool IsDetachedFixedPurchaseDiscount(CartObject combinedCart, string[] childOrderIds)
		{
			if (combinedCart.FixedPurchaseDiscount != 0) return false;

			var orderService = new OrderService();
			foreach (var childOrderId in childOrderIds)
			{
				var childOrder = orderService.Get(childOrderId);
				if (childOrder.FixedPurchaseDiscountPrice != 0) return true;
			}

			return false;
		}

		/// <summary>
		/// 注文同梱 子注文が既存定期注文の場合、同梱後の定期購入注文回数がキャンセル分少なくなるため注文回数補正
		/// </summary>
		/// <param name="parentOrderId">親注文ID</param>
		/// <param name="childOrderIds">子注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public static void CorrectFixedPurchaseOrderCount(
			string parentOrderId,
			string[] childOrderIds,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			var orderService = new OrderService();
			var fixedPurchaseService = new FixedPurchaseService();

			// 注文に紐づくすべての元注文を取得
			var childOrderIdList = GetCombineOrgOrderIds(childOrderIds.JoinToString(",")).ToList();

			// 親注文が同梱注文の場合は既に同梱分多くなるのでその元注文の親注文は注文回数補正しない
			var targetCombineOrgOrderIds = new OrderService().GetCombineOrgOrderIds(parentOrderId);
			if ((string.IsNullOrEmpty(targetCombineOrgOrderIds) == false))
			{
				childOrderIdList.RemoveAll(orderId => orderId == parentOrderId);
			}

			// 注文に紐づくすべての定期購入注文回数補正
			var parentOrder = orderService.Get(parentOrderId);
			var isSkip = false;
			foreach (var childOrderId in childOrderIdList)
			{
				// 親注文に紐づく定期の場合スキップ(一回だけスキップする)
				var childOrder = orderService.Get(childOrderId);
				if ((isSkip == false) && (childOrder.FixedPurchaseId == parentOrder.FixedPurchaseId))
				{
					isSkip = true;
					continue;
				}

				if (childOrder.IsFixedPurchaseOrder)
				{
					var fixedPurchase = fixedPurchaseService.Get(childOrder.FixedPurchaseId);
					fixedPurchaseService.UpdateOrderCount(
						fixedPurchase.FixedPurchaseId,
						fixedPurchase.OrderCount + 1,
						lastChanged,
						updateHistoryAction);

					// 商品購入回数更新
					foreach (var item in fixedPurchase.Shippings[0].Items)
					{
						fixedPurchaseService.UpdateItemOrderCountWhenOrdering(
							fixedPurchase.FixedPurchaseId,
							item.VariationId,
							item.ItemOrderCount,
							lastChanged,
							updateHistoryAction);
					}
				}
			}
		}

		/// <summary>
		/// 注文同梱 注文同梱により商品価格に変更があったかチェックする
		/// </summary>
		/// <param name="beforeCart">注文同梱前カート</param>
		/// <param name="combinedCart">注文同梱後カート</param>
		/// <returns>変更有無</returns>
		public static bool IsChangedProductPriceByOrderCombine(CartObject beforeCart, CartObject combinedCart)
		{
			foreach (var beforeItem in beforeCart.Items)
			{
				var combinedProduct = combinedCart.Items.FirstOrDefault(
					parentItem => (parentItem.ShopId == beforeItem.ShopId)
						&& (parentItem.ProductId == beforeItem.ProductId)
						&& (parentItem.VariationId == beforeItem.VariationId)
						&& (((parentItem.ProductOptionSettingList != null) ? parentItem.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() : "")
							== ((beforeItem.ProductOptionSettingList != null) ? beforeItem.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues() : ""))
						&& (parentItem.IsFixedPurchase == beforeItem.IsFixedPurchase)
						&& (parentItem.ProductSaleId == beforeItem.ProductSaleId));

				if (combinedProduct == null) continue;
				if (beforeItem.Price != combinedProduct.Price) return true;
			}

			return false;
		}

		/// <summary>
		/// 注文同梱 注文同梱により定期台帳に変更があるかチェックする
		/// </summary>
		/// <param name="beforeCart">注文同梱前カート</param>
		/// <param name="combinedCart">注文同梱後カート</param>
		/// <returns>変更有無</returns>
		public static bool IsChangedFixedPurchaseByOrderCombine(CartObject beforeCart, CartObject combinedCart)
		{
			var result = ((beforeCart.HasFixedPurchase) && (combinedCart.IsCombineParentOrderHasFixedPurchase));
			return result;
		}

		/// <summary>
		/// 注文に紐づく注文同梱元注文IDを取得
		/// </summary>
		/// <param name="orderId">同梱注文ID</param>
		/// <returns>注文に紐づく注文同梱元注文ID</returns>
		public static string[] GetCombineOrgOrderIds(string combineOrgOrderIds)
		{
			// 同梱注文でない場合処理を終了する
			var allCombineOrgOrderIds = new List<string>();
			if (string.IsNullOrEmpty(combineOrgOrderIds)) return allCombineOrgOrderIds.ToArray();

			// 注文に紐づくすべての注文同梱元注文IDを取得
			allCombineOrgOrderIds.AddRange(combineOrgOrderIds.Split(','));
			foreach (var combineOrgOrderId in combineOrgOrderIds.Split(','))
			{
				var nextTargetCombineOrgOrderIds = new OrderService().GetCombineOrgOrderIds(combineOrgOrderId);
				if (string.IsNullOrEmpty(nextTargetCombineOrgOrderIds)) continue;

				// 同梱注文は補正せず、元注文の方で補正する
				allCombineOrgOrderIds.RemoveAll(orderId => orderId == combineOrgOrderId);
				allCombineOrgOrderIds.AddRange(GetCombineOrgOrderIds(nextTargetCombineOrgOrderIds));
			}
			return allCombineOrgOrderIds.ToArray();
		}
	}
}
