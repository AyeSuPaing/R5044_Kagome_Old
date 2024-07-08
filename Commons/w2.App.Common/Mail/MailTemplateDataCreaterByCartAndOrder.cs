/*
=========================================================================================================
  Module      : カートと注文から注文メールテンプレートデータを作成（将来的に統合したい）(MailTemplateDataCreaterByCartAndOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.DeliveryCompany;
using w2.Domain.Order;
using w2.Domain.RealShop;
using w2.Domain.User;

namespace w2.App.Common.Mail
{
	/// <summary>
	/// カートと注文から注文メールテンプレートデータを作成（将来的に統合したい）
	/// </summary>
	public class MailTemplateDataCreaterByCartAndOrder : MailTemplateDataCreaterBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="isPc">注文区分がPCか</param>
		public MailTemplateDataCreaterByCartAndOrder(bool isPc)
			: base(isPc)
		{
		}

		/// <summary>
		/// 注文メールデータ取得
		/// </summary>
		/// <param name="orderData">注文情報</param>
		/// <param name="cart">カート情報</param>
		/// <param name="isUser">会員か否か</param>
		/// <param name="shippingNo">配送先番号</param>
		/// <param name="isToNextEngine">ネクストエンジンへのメール送信か</param>
		/// <returns>注文完了メールテンプレート用ハッシュテーブル</returns>
		public Hashtable GetOrderMailDatas(Hashtable orderData, CartObject cart, bool isUser, int shippingNo = 0, bool isToNextEngine = false)
		{
			// キーの重複に注意（先勝ち）

			//------------------------------------------------------
			// 注文情報セット
			//------------------------------------------------------
			var mailDatas = new Hashtable();
			var discountPriceTotal = (decimal)0;
			mailDatas.Add(Constants.FIELD_ORDER_SHOP_ID, cart.ShopId);
			mailDatas.Add(Constants.FIELD_ORDER_USER_ID, (string)orderData[Constants.FIELD_ORDER_USER_ID]);
			var isChildShipping = (shippingNo > 0);
			mailDatas.Add(
				Constants.FIELD_ORDER_ORDER_ID,
				(string)orderData[Constants.FIELD_ORDER_ORDER_ID] + (isChildShipping ? ("-" + shippingNo) : ""));
			// 支払い方法がカード かつ 支払い回数を選択できる場合のみメールに支払い回数表示
			var paymentName = cart.Payment.GetPaymentTranslationName(cart.Owner.DispLanguageCode, cart.Owner.DispLanguageLocaleId);
			if ((Constants.GLOBAL_OPTION_ENABLE == false) && cart.IsBotChanOrder)
			{
				paymentName = cart.Payment.GetPaymentNameTranslationName(
					cart.Owner.DispLanguageCode,
					cart.Owner.DispLanguageLocaleId);
			}
			mailDatas.Add(Constants.FIELD_PAYMENT_PAYMENT_NAME, paymentName);
			if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && OrderCommon.CreditInstallmentsSelectable)
			{
				mailDatas[Constants.FIELD_PAYMENT_PAYMENT_NAME] +=
					"(" + ValueText.GetValueText(Constants.TABLE_ORDER, OrderCommon.CreditInstallmentsValueTextFieldName, cart.Payment.CreditInstallmentsCode) + ")";
			}
			mailDatas.Add(Constants.FIELD_ORDER_SHIPPING_ID, cart.ShippingType);
			mailDatas.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, cart.Payment.PaymentId);
			mailDatas.Add(Constants.FIELD_ORDER_ORDER_DATE,
				DateTimeUtility.ToString(
					orderData[Constants.FIELD_ORDER_ORDER_DATE],
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
					cart.Owner.DispLanguageLocaleId));
			// 管理者向けの日付系のタグ用を追加
			mailDatas.Add(Constants.FIELD_ORDER_ORDER_DATE + STRING_FOR_OPERATOR,
				DateTimeUtility.ToStringForManager(
					orderData[Constants.FIELD_ORDER_ORDER_DATE],
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter));
			// ネクストエンジン向けの日付系のタグ用を追加
			mailDatas.Add(
				Constants.FIELD_ORDER_ORDER_DATE + STRING_FOR_NEXTENGINE,
				DateTimeUtility.ToStringForManager(
					orderData[Constants.FIELD_ORDER_ORDER_DATE],
					DateTimeUtility.FormatType.LongDateHourMinuteSecondNoneServerTime));
			mailDatas.Add(Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL, ConvertPriceForMail(isChildShipping ? 0 : cart.PriceSubtotal));
			mailDatas.Add(Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING, ConvertPriceForMail(isChildShipping ? 0 : cart.PriceShipping));
			mailDatas.Add(Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE, ConvertPriceForMail(isChildShipping ? 0 : cart.Payment.PriceExchange));
			mailDatas.Add(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, ConvertPriceForMail(isChildShipping ? 0 : cart.PriceTotal));

			var subscriptionBoxId = (string)orderData[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID];
			mailDatas.Add(Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID, subscriptionBoxId);

			var subscriptionBox = new DataCacheController.SubscriptionBoxCacheController().CacheData
				.FirstOrDefault(sb => (sb.CourseId == subscriptionBoxId));
			var subscriptionBoxDisplayName = (subscriptionBox != null) ? subscriptionBox.DisplayName : "";
			mailDatas.Add(Constants.MAILTAG_SUBSCRIPTION_BOX_DISPLAY_NAME, subscriptionBoxDisplayName);
			mailDatas.Add(Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT, orderData[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT]);

			var globalPrice = CurrencyManager.ToPrice(
				(string)orderData[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_CODE],
				(string)orderData[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID],
				cart.PriceTotal);
			mailDatas.Add("order_price_total_global", globalPrice);
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				mailDatas.Add(Constants.FIELD_ORDER_ORDER_POINT_ADD, (isUser) ? StringUtility.ToNumeric(cart.BuyPoint + cart.FirstBuyPoint) : "0");
				mailDatas.Add(Constants.FIELD_ORDER_ORDER_POINT_USE, StringUtility.ToNumeric(isChildShipping ? 0 : cart.UsePoint));
				mailDatas.Add(Constants.FIELD_ORDER_ORDER_POINT_USE_YEN, ConvertPriceForMail(isChildShipping ? 0 : cart.UsePointPrice));
				discountPriceTotal += cart.UsePointPrice;
				// ポイント取得
				var userPoint = PointOptionUtility.GetUserPoint((string)orderData[Constants.FIELD_ORDER_USER_ID]);
				// 現在の通常本ポイント
				mailDatas.Add("current_user_point", StringUtility.ToNumeric(userPoint.BasicPoint.PointComp));
				// 現在の利用可能ポイント
				mailDatas.Add("current_user_point_usable", StringUtility.ToNumeric(userPoint.PointUsable));
				// 現在の利用可能期間限定ポイント
				mailDatas.Add(
					"current_user_limited_term_point_usable",
					StringUtility.ToNumeric(
						userPoint.LimitedTermPoint
							.Where(up => up.IsUsableForOrder)
							.Sum(up => up.Point)));
				// 取得ポイント付与後の現在のユーザの保有ポイント
				mailDatas.Add("after_add_point", StringUtility.ToNumeric(userPoint.PointUsable + Convert.ToDecimal(mailDatas[Constants.FIELD_ORDER_ORDER_POINT_ADD])));
			}
			if (Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				mailDatas.Add(Constants.FIELD_ORDER_MEMBER_RANK_DISCOUNT_PRICE, ConvertPriceForMail(cart.MemberRankDiscount));
				discountPriceTotal += cart.MemberRankDiscount;
			}
			if (Constants.MEMBER_RANK_OPTION_ENABLED && Constants.FIXEDPURCHASE_OPTION_ENABLED)
			{
				mailDatas.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_MEMBER_DISCOUNT_AMOUNT, ConvertPriceForMail(cart.FixedPurchaseMemberDiscountAmount));
				discountPriceTotal += cart.FixedPurchaseMemberDiscountAmount;
			}

			var orderMemoFromCart = cart.GetOrderMemos().Trim();
			mailDatas.Add(Constants.FIELD_ORDER_MEMO, string.IsNullOrEmpty(orderMemoFromCart) ? StringUtility.ToEmpty(orderData[Constants.FIELD_ORDER_MEMO]) : orderMemoFromCart);
			mailDatas.Add(Constants.FIELD_ORDER_GIFT_FLG, cart.IsGift ? Constants.FLG_ORDER_GIFT_FLG_ON : Constants.FLG_ORDER_GIFT_FLG_OFF);
			mailDatas.Add(
				Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG,
				cart.ShippingPriceSeparateEstimateFlg
					? Constants.FLG_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID
					: Constants.FLG_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_INVALID);
			mailDatas.Add("separate_estimates_message", GetMessageForSeparateEstimate(cart));
			mailDatas.Add(
				Constants.FIELD_ORDER_DIGITAL_CONTENTS_FLG, cart.IsDigitalContentsOnly
					? Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON
					: Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_OFF);

			for (var i = 1; i <= Constants.CONST_ORDER_EXTEND_STATUS_DBFIELDS_MAX; i++)
			{
				var filedName = Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME + i;
				mailDatas.Add(filedName, orderData[filedName] ?? Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF);
			}

			var user = new UserService().Get((string)orderData[Constants.FIELD_ORDER_USER_ID]);
			mailDatas.Add(Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID, (user != null) ? string.Empty : user.UserManagementLevelId);

			//------------------------------------------------------
			// 注文者情報格納
			//------------------------------------------------------
			SetOrderOwnerToMailTemplate(cart, mailDatas);

			//------------------------------------------------------
			// 注文配送先情報セット
			//------------------------------------------------------
			var shopShipping = OrderCommon.GetShopShipping(cart.ShopId, cart.ShippingType);
			mailDatas[Constants.FIELD_SHOPSHIPPING_SHIPPING_DATE_SET_FLG] = shopShipping.ShippingDateSetFlg;
			mailDatas[Constants.FIELD_SHOPSHIPPING_WRAPPING_PAPER_FLG] = shopShipping.WrappingPaperFlg;
			mailDatas[Constants.FIELD_SHOPSHIPPING_WRAPPING_BAG_FLG] = shopShipping.WrappingBagFlg;
			// 配送種別セット
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD] = (string)cart.Shippings[0].ShippingMethod;
			// 配送種別名セット
			mailDatas["shipping_method_name"] =
				ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD]);

			// 配送会社情報セット
			var deliveryCompany = new DeliveryCompanyService().Get(cart.Shippings[0].DeliveryCompanyId);
			// 配送希望時間帯セット
			mailDatas[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG] = deliveryCompany.ShippingTimeSetFlg;
			// 配送会社IDセット
			mailDatas[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_ID] = deliveryCompany.DeliveryCompanyId;
			// 配送会社名セット
			mailDatas[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_NAME] = deliveryCompany.DeliveryCompanyName;

			//------------------------------------------------------
			// ギフト用の配送・商品の情報作成
			//------------------------------------------------------
			CreateOfShippingAndProductForGift(cart, mailDatas, shippingNo, isToNextEngine);

			//------------------------------------------------------
			// 注文商品情報セット
			//------------------------------------------------------
			if (cart.IsGift == false)
			{
				SetOrderItemToHashtableForOrderMailTemplete(cart, cart.Items, mailDatas);
				SetOrderItemsLoopToHashtableForOrderMailTemplete(cart, cart.Items, mailDatas);
				SetOrderItemToHashtableForNextEngineOrderMailTemplete(cart, cart.Items, mailDatas);
			}
			mailDatas.Add(Constants.FIELD_ORDER_ORDER_PRICE_TAX, ConvertPriceForMail(cart.PriceTax));
			mailDatas.Add(Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL_TAX, ConvertPriceForMail(cart.PriceSubtotalTax));
			mailDatas.Add("ProductPriceTextPrefix", TaxCalculationUtility.GetTaxTypeText());

			//------------------------------------------------------
			// セットプロモーション割引情報セット
			//------------------------------------------------------
			// 商品割引
			var setPromotionProductDiscount =
				cart.SetPromotions.Items.Where(sp => sp.IsDiscountTypeProductDiscount).Select(
					setPromotion => new Hashtable
					{
						{Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME, setPromotion.SetpromotionDispName},
						{Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT, ConvertPriceForMail(setPromotion.ProductDiscountAmount)}
					}).ToList();
			if (setPromotionProductDiscount.Count != 0)
			{
				mailDatas.Add("SetPromotionProductDiscountLoop", setPromotionProductDiscount);
			}

			// 配送料割引
			var setPromotionShippingChargeDiscount =
				cart.SetPromotions.Items.Where(sp => sp.IsDiscountTypeShippingChargeFree).Select(
					setPromotion => new Hashtable
					{
						{Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME, setPromotion.SetpromotionDispName},
						{Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT, ConvertPriceForMail(setPromotion.ShippingChargeDiscountAmount)}
					}).ToList();
			if (setPromotionShippingChargeDiscount.Count != 0)
			{
				mailDatas.Add("SetPromotionShippingChargeDiscountLoop", setPromotionShippingChargeDiscount);
			}

			// 決済手数料割引
			var setPromotionPaymentChargeDiscount =
				cart.SetPromotions.Items.Where(sp => sp.IsDiscountTypePaymentChargeFree).Select(
					setPromotion => new Hashtable
					{
						{Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME, setPromotion.SetpromotionDispName},
						{Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT, ConvertPriceForMail(setPromotion.PaymentChargeDiscountAmount)}
					}).ToList();
			if (setPromotionPaymentChargeDiscount.Count != 0)
			{
				mailDatas.Add("SetPromotionPaymentChargeDiscountLoop", setPromotionPaymentChargeDiscount);
			}

			// セットプロモーション割引額合計
			mailDatas.Add("setpromotion_total_discount_amount", ConvertPriceForMail(cart.SetPromotions.TotalDiscountAmount));
			discountPriceTotal += cart.SetPromotions.TotalDiscountAmount;

			// 決済URLメッセージセット
			var paymentMessage = "";
			if (orderData.ContainsKey("payment_url"))
			{
				paymentMessage += orderData["payment_url"];
			}
			if (orderData.ContainsKey("payment_message_text"))
			{
				paymentMessage += orderData["payment_message_text"].ToString();
			}
			mailDatas.Add("payment_message_text", paymentMessage);
			mailDatas.Add(Constants.FIELD_ORDER_PAYMENT_MEMO, (string)orderData[Constants.FIELD_ORDER_PAYMENT_MEMO]);

			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				//------------------------------------------------------
				// 発行クーポン情報格納
				//------------------------------------------------------
				// 発行したクーポン情報取得
				var no = 1;
				var publishMsgBuilder = new StringBuilder();
				var publishMsgForOperator = new StringBuilder();
				var publishCoupons = new CouponService().GetOrderPublishUserCoupon(
					Constants.W2MP_DEPT_ID,
					(string)orderData[Constants.FIELD_ORDER_USER_ID],
					(string)orderData[Constants.FIELD_ORDER_ORDER_ID]);
				foreach (var coupon in publishCoupons)
				{
					// 管理者向けとエンドユーザー向けとの情報を区別する
					CreatePublishCouponInfo(publishMsgBuilder, coupon, no, cart.Owner.DispLanguageLocaleId);
					CreatePublishCouponInfo(publishMsgForOperator, coupon, no, Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE);

					no++;
				}
				mailDatas.Add("publish_coupons", publishMsgBuilder.ToString());
				// 管理者向けのタグ用を追加
				mailDatas.Add("publish_coupons" + STRING_FOR_OPERATOR, publishMsgForOperator.ToString());

				//------------------------------------------------------
				// 利用クーポン情報格納
				//------------------------------------------------------
				mailDatas.Add(Constants.FIELD_ORDER_ORDER_COUPON_USE, ConvertPriceForMail(cart.UseCouponPrice));	// クーポン割引額
				mailDatas.Add(Constants.FIELD_ORDERCOUPON_COUPON_CODE, (cart.Coupon != null) ? cart.Coupon.CouponCode : "");			// クーポンコード
				mailDatas.Add(Constants.FIELD_ORDERCOUPON_COUPON_DISP_NAME, (cart.Coupon != null) ? cart.Coupon.CouponDispName : "");
				discountPriceTotal += cart.UseCouponPrice;
			}

			//------------------------------------------------------
			// 定期購入情報格納
			//------------------------------------------------------
			if (cart.HasFixedPurchase)
			{
				var order = new OrderService().Get((string)orderData[Constants.FIELD_ORDER_ORDER_ID]);
				mailDatas.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_ID, order.FixedPurchaseId);
				mailDatas.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_ORDER_COUNT, order.FixedPurchaseOrderCount.HasValue ? order.FixedPurchaseOrderCount.Value.ToString() : "");
				mailDatas.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_SHIPPED_COUNT, order.FixedPurchaseShippedCount.HasValue ? order.FixedPurchaseShippedCount.Value.ToString() : "");
				mailDatas.Add("fixed_purchase_pattern", cart.GetShipping().GetNextShippingItemFixedPurchaseShippingPattern());

				// エンドユーザー向けの日付タグ追加
				AddLongDateTagsForUser(
					mailDatas,
					Constants.FIELD_MAIL_FIELD_FIRST_SHIPPING_DATE,
					cart.GetShipping().GetFirstShippingDate(),
					cart.Owner.DispLanguageLocaleId);
				AddLongDateTagsForUser(
					mailDatas,
					Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE,
					cart.GetShipping().NextShippingDate,
					cart.Owner.DispLanguageLocaleId);
				AddLongDateTagsForUser(
					mailDatas,
					Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE,
					cart.GetShipping().NextNextShippingDate,
					cart.Owner.DispLanguageLocaleId);

				// 管理者向けの日付タグ追加
				AddLongDateTagsForManager(
					mailDatas,
					Constants.FIELD_MAIL_FIELD_FIRST_SHIPPING_DATE,
					cart.GetShipping().GetFirstShippingDate());
				AddLongDateTagsForManager(
					mailDatas,
					Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE,
					cart.GetShipping().NextShippingDate);
				AddLongDateTagsForManager(
					mailDatas,
					Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE,
					cart.GetShipping().NextNextShippingDate);

				mailDatas.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_DISCOUNT_PRICE, ConvertPriceForMail(order.FixedPurchaseDiscountPrice));
				mailDatas.Add(Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT, orderData[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT]);

				mailDatas[Constants.TAG_FIXED_PURCHASE_MEMO] = (orderData.ContainsKey(Constants.FIELD_ORDER_MEMO)
					? StringUtility.ToEmpty(orderData[Constants.FIELD_ORDER_MEMO])
					: ((cart.ReflectMemoToFixedPurchase == true)
						? order.Memo
						: string.Empty));

				discountPriceTotal += cart.FixedPurchaseDiscount;
			}

			// 割引額合計
			mailDatas.Add("order_price_discount_total", ConvertPriceForMail(discountPriceTotal));

			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG] = cart.Shippings[0].ConvenienceStoreFlg;

			// 決済金額・決済レート・決済通貨
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				mailDatas.Add(Constants.FIELD_ORDER_SETTLEMENT_CURRENCY, cart.SettlementCurrency);
				mailDatas.Add(Constants.FIELD_ORDER_SETTLEMENT_RATE,
					DecimalUtility.DecimalRound(cart.SettlementRate,
					DecimalUtility.Format.RoundDown, 6));
				mailDatas.Add(
					Constants.FIELD_ORDER_SETTLEMENT_AMOUNT,
					CurrencyManager.ToSettlementCurrencyNotation(cart.SettlementAmount, cart.SettlementCurrency));
				mailDatas.Add(
					"settlement_amount_without_symbol",
					String.Format("{0:#,0}", cart.SettlementAmount));
			}

			// 調整金額に税率毎の返品用金額補正を合算する
			mailDatas[Constants.FIELD_ORDER_ORDER_PRICE_REGULATION] = ConvertPriceForMail(cart.PriceRegulationTotal);

			// 領収書ダウンロードＵＲＬ
			AddReceiptDownloadUrl(
				mailDatas,
				(string)orderData[Constants.FIELD_ORDER_ORDER_ID],
				cart.ReceiptFlg,
				cart.ReceiptAddress,
				cart.ReceiptProviso);

			SettingOrderExtend(mailDatas, cart);

			// 購入回数（注文基準）と注文時会員ランクをセット
			var currentOrder = new OrderService().Get((string)orderData[Constants.FIELD_ORDER_ORDER_ID]);
			mailDatas[Constants.FIELD_ORDER_ORDER_COUNT_ORDER] = currentOrder.OrderCountOrder.HasValue
				? currentOrder.OrderCountOrder.Value.ToString()
				: string.Empty;
			mailDatas[Constants.FIELD_ORDER_MEMBER_RANK_ID] = currentOrder.MemberRankId;

			// 注文商品の商品IDを取得
			var orderItemsProductId = cart.IsGift
				? GetOrderItemsProductIdForGift(cart)
				: GetOrderItemsProductId(cart);
			SetOrderItemsInfo(mailDatas, cart.ShopId, orderItemsProductId.ToArray());

			// 広告コード情報をセット
			mailDatas.Add(Constants.FIELD_ORDER_ADVCODE_FIRST, orderData[Constants.FIELD_ORDER_ADVCODE_FIRST].ToString());
			mailDatas.Add(Constants.FIELD_ORDER_ADVCODE_NEW, orderData[Constants.FIELD_ORDER_ADVCODE_NEW].ToString());

			if (Constants.STORE_PICKUP_OPTION_ENABLED
				&& cart.Shippings[0].IsShippingStorePickup)
			{
				var storePickupRealShopId = cart.Shippings[0].RealShopId;
				if (storePickupRealShopId != null)
				{
					var realShop = new RealShopService().Get(storePickupRealShopId);

					if ((realShop != null)
						&& (realShop.ValidFlg == Constants.FLG_REALSHOP_VALID_FLG_VALID))
					{
						mailDatas.Add(Constants.MAILTAG_STORE_PICKUP_SHOP_NAME, StringUtility.ToEmpty(realShop.Name));
						mailDatas.Add(Constants.MAILTAG_STORE_PICKUP_SHOP_ZIP, StringUtility.ToEmpty(realShop.Zip));
						mailDatas.Add(Constants.MAILTAG_STORE_PICKUP_SHOP_ADDR1, StringUtility.ToEmpty(realShop.Addr1));
						mailDatas.Add(Constants.MAILTAG_STORE_PICKUP_SHOP_ADDR2, StringUtility.ToEmpty(realShop.Addr2));
						mailDatas.Add(Constants.MAILTAG_STORE_PICKUP_SHOP_ADDR3, StringUtility.ToEmpty(realShop.Addr3));
						mailDatas.Add(Constants.MAILTAG_STORE_PICKUP_SHOP_ADDR4, StringUtility.ToEmpty(realShop.Addr4));
						mailDatas.Add(Constants.MAILTAG_STORE_PICKUP_SHOP_TEL, StringUtility.ToEmpty(realShop.Tel));
						mailDatas.Add(Constants.MAILTAG_STORE_PICKUP_SHOP_BUSINESS_HOURS, StringUtility.ToEmpty(realShop.OpeningHours));
					}
				}

				mailDatas.Add(Constants.MAILTAG_STORE_PICKUP_IS_STOREPICKUPORDER, Constants.MAILTAG_STORE_PICKUP_IS_STOREPICKUPORDER);
			}

			AddMailUnsubscribeTags(mailDatas, (string)orderData[Constants.FIELD_ORDER_USER_ID], cart.Owner.MailAddr);

			return mailDatas;
		}

		/// <summary>
		/// 注文者情報格納
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="mailDatas">メールデータ</param>
		private void SetOrderOwnerToMailTemplate(CartObject cart, Hashtable mailDatas)
		{
			var owner = cart.Owner.GetDictionary();
			foreach (var field in owner.Keys.Where(field => (mailDatas.Contains(field) == false)))
			{
				mailDatas.Add(field, owner[field]);
			}
			// 生年月日の時分秒削除
			mailDatas[Constants.FIELD_ORDEROWNER_OWNER_BIRTH] = DateTimeUtility.ToString(
					mailDatas[Constants.FIELD_ORDEROWNER_OWNER_BIRTH],
					DateTimeUtility.FormatType.ShortDate2Letter,
					cart.Owner.DispLanguageLocaleId);

			// 管理者向けのメールテンプレートに利用するため、日付系の項目を追加
			mailDatas.Add(
				Constants.FIELD_ORDEROWNER_OWNER_BIRTH + STRING_FOR_OPERATOR,
				DateTimeUtility.ToStringForManager(
					owner[Constants.FIELD_ORDEROWNER_OWNER_BIRTH],
					DateTimeUtility.FormatType.ShortDate2Letter));

			// ネクストエンジン用注文者電話番号の追加
			mailDatas.Add(
				Constants.FIELD_ORDEROWNER_OWNER_TEL1 + STRING_FOR_NEXTENGINE,
				((string)owner[Constants.FIELD_ORDEROWNER_OWNER_TEL1]).Replace("-", "")
			);
		}

		/// <summary>
		/// 配送先情報のHashtableセット（注文メールテンプレート用）
		/// </summary>
		/// <param name="cartShipping">カート配送先</param>
		/// <param name="mailDatas">メールデータ</param>
		private void SetOrderShippingToHashtableForOrderMailTemplete(CartShipping cartShipping, Hashtable mailDatas)
		{
			mailDatas[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE] = cartShipping.GetScheduledShippingDate();
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE] = cartShipping.GetShippingDate();
			// 管理者向けの日付系のタグ用を追加
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE + STRING_FOR_OPERATOR] = cartShipping.GetShippingDate(true);
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME] = cartShipping.GetShippingTime();

			mailDatas[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO] = cartShipping.CartObject.Shippings.IndexOf(cartShipping) + 1;
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP] = cartShipping.Zip;
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1] = cartShipping.Addr1;
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2] = cartShipping.Addr2;
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3] = cartShipping.Addr3;
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4] = cartShipping.Addr4;
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5] = cartShipping.Addr5;
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME] = cartShipping.ShippingCountryName;
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME] = cartShipping.CompanyName;
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME] = cartShipping.CompanyPostName;
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME] = cartShipping.Name1 + cartShipping.Name2;
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA] = cartShipping.NameKana1 + cartShipping.NameKana2;
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1] = cartShipping.Tel1;
			mailDatas[Constants.FIELD_ORDERSHIPPING_ANOTHER_SHIPPING_FLG] = cartShipping.AnotherShippingFlag;

			if (cartShipping.CartObject.IsGift)
			{
				mailDatas[Constants.FIELD_ORDERSHIPPING_SENDER_ZIP] = cartShipping.SenderZip1 + "-" + cartShipping.SenderZip2;
				mailDatas[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR1] = cartShipping.SenderAddr1;
				mailDatas[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR2] = cartShipping.SenderAddr2;
				mailDatas[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR3] = cartShipping.SenderAddr3;
				mailDatas[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR4] = cartShipping.SenderAddr4;
				mailDatas[Constants.FIELD_ORDERSHIPPING_SENDER_ADDR5] = cartShipping.SenderAddr5;
				mailDatas[Constants.FIELD_ORDERSHIPPING_SENDER_COUNTRY_NAME] = cartShipping.SenderCountryName;
				mailDatas[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_NAME] = cartShipping.SenderCompanyName;
				mailDatas[Constants.FIELD_ORDERSHIPPING_SENDER_COMPANY_POST_NAME] = cartShipping.SenderCompanyPostName;
				mailDatas[Constants.FIELD_ORDERSHIPPING_SENDER_NAME] = cartShipping.SenderName1 + cartShipping.SenderName2;
				mailDatas[Constants.FIELD_ORDERSHIPPING_SENDER_NAME_KANA] = cartShipping.SenderNameKana1 + cartShipping.SenderNameKana2;
				mailDatas[Constants.FIELD_ORDERSHIPPING_SENDER_TEL1] = cartShipping.SenderTel1;

				mailDatas[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE] = (cartShipping.WrappingPaperType != "") ? cartShipping.WrappingPaperType : "指定なし";
				mailDatas[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME] = (cartShipping.WrappingPaperName != "") ? cartShipping.WrappingPaperName : "指定なし";
				mailDatas[Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE] = (cartShipping.WrappingBagType != "") ? cartShipping.WrappingBagType : "指定なし";
			}

			// 配送方法
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD] = (string)cartShipping.ShippingMethod;
			// 配送方法名
			mailDatas["shipping_method_name"] =
				ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD]);

			var deliveryCompany = new DeliveryCompanyService().Get(cartShipping.DeliveryCompanyId);
			// 配送希望時間帯セット
			mailDatas[Constants.FIELD_DELIVERYCOMPANY_SHIPPING_TIME_SET_FLG] = deliveryCompany.ShippingTimeSetFlg;
			// 配送会社IDセット
			mailDatas[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_ID] = deliveryCompany.DeliveryCompanyId;
			// 配送会社名セット
			mailDatas[Constants.FIELD_DELIVERYCOMPANY_DELIVERY_COMPANY_NAME] = deliveryCompany.DeliveryCompanyName;

			// Get taiwan invoice data for send mail
			var orderId = StringUtility.ToEmpty(mailDatas[Constants.FIELD_ORDER_ORDER_ID]);
			var shippingNo = StringUtility.ToEmpty(mailDatas[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO]);
			OrderCommon.GetTwInvoiceDataForSendMail(orderId, int.Parse(shippingNo), mailDatas);
		}

		/// <summary>
		/// 商品情報のHashtableセット（注文メールテンプレート用）
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="productItems">商品アイテム</param>
		/// <param name="mailDatas">メールデータ</param>
		private void SetOrderItemToHashtableForOrderMailTemplete(CartObject cart, IList productItems, Hashtable mailDatas)
		{
			var displayProductItems = (cart.IsGift
				? ((List<CartShipping.ProductCount>)productItems).Select(item => new Tuple<CartProduct, int>(item.Product, item.Count))
				: ((List<CartProduct>)productItems).Select(item => new Tuple<CartProduct, int>(item, item.QuantitiyUnallocatedToSet)))
				.Where(item => item.Item1.OrderHistoryDisplayType == Constants.FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_VALID)
				.ToList();
			var orderItemsString = new StringBuilder();
			var cartItemIndex = 0;
			var cartProductIndex = 0;
			while (cartItemIndex < displayProductItems.Count)
			{
				if (displayProductItems[cartItemIndex].Item1.IsSetItem)
				{
					// セット商品の場合
					orderItemsString.Append(orderItemsString.Length != 0 ? "\r\n" : "");
					orderItemsString.Append("-(").Append(cartProductIndex + 1).Append(")----------" + this.BorderString).Append("\r\n");

					var cartProductSetItemIndex = 0;
					var productSetCurrent = displayProductItems[cartItemIndex].Item1.ProductSet;

					var itemPriceTotal = 0m;
					while (cartProductSetItemIndex < productSetCurrent.Items.Count)
					{
						var productCurrent = productSetCurrent.Items[cartProductSetItemIndex];

						itemPriceTotal += productCurrent.PriceSubtotalSingle;

						orderItemsString.Append(" -(").Append(cartProductIndex + 1).Append("-").Append(cartProductSetItemIndex + 1).Append(")----------" + this.BorderString).Append("\r\n");
						AppendOrderItemStringBuilder(cart, displayProductItems[cartItemIndex].Item1, orderItemsString, false);
						orderItemsString.Append("\r\n");
						cartItemIndex++;
						cartProductSetItemIndex++;
					}

					orderItemsString.Append(" --------------" + this.BorderString).Append("\r\n");
					orderItemsString.Append(" セット価格：").Append(ConvertPriceForMailByConfig(itemPriceTotal)).Append(GetProductTaxText()).Append("\r\n");
					orderItemsString.Append(" 数量      ：").Append(StringUtility.ToNumeric(productSetCurrent.ProductSetCount)).Append("");

					cartProductIndex++;
				}
				else if (cart.IsGift || (displayProductItems[cartItemIndex].Item1.QuantitiyUnallocatedToSet != 0))
				{
					// ギフト注文の場合、またはセットプロモーションではない商品が0ではない場合
					orderItemsString.Append(orderItemsString.Length != 0 ? "\r\n" : "");
					orderItemsString.Append("-(").Append(cartProductIndex + 1).Append(")----------" + this.BorderString).Append("\r\n");

					AppendOrderItemStringBuilder(
						cart,
						displayProductItems[cartItemIndex].Item1,
						orderItemsString,
						true,
						null,
						displayProductItems[cartItemIndex].Item2);

					// デジタルコンテンツ対応
					if (cart.IsDigitalContentsOnly && displayProductItems[cartItemIndex].Item1.IsDelivered)
					{
						for (var i = 0; i < displayProductItems[cartItemIndex].Item1.SerialKeys.Count; i++)
						{
							orderItemsString.Append("\r\n");
							orderItemsString.Append((i == 0) ? "シリアルキー  ：" : "　　　　　　  ：");
							orderItemsString.Append(SerialKeyUtility.GetFormattedKeyString(SerialKeyUtility.DecryptSerialKey(displayProductItems[cartItemIndex].Item1.SerialKeys[i])));
						}
					}

					cartItemIndex++;
					cartProductIndex++;
				}
				else
				{
					cartItemIndex++;
				}
			}

			// セットプロモーション商品を追加（ギフトではない場合のみ）
			if (cart.IsGift == false)
			{
				foreach (CartSetPromotion setPromotion in cart.SetPromotions)
				{
					orderItemsString.Append(orderItemsString.Length != 0 ? "\r\n" : "");
					orderItemsString.Append("-(").Append(cartProductIndex + 1).Append(")----------" + this.BorderString).Append("\r\n");
					orderItemsString.Append(" ").Append(setPromotion.SetpromotionDispName).Append("\r\n");

					var itemIndex = 1;
					var setPromotionItems = new StringBuilder();
					foreach (var item in setPromotion.Items.Where(item => item.OrderHistoryDisplayType == Constants.FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_VALID))
					{
						setPromotionItems.Append(setPromotionItems.Length != 0 ? "\r\n" : "");
						setPromotionItems.Append(" -(").Append(cartProductIndex + 1).Append("-").Append(itemIndex).Append(")----------" + this.BorderString).Append("\r\n");
						AppendOrderItemStringBuilder(cart, item, setPromotionItems, false, setPromotion);

						// デジタルコンテンツ対応
						if (cart.IsDigitalContentsOnly && item.IsDelivered)
						{
							for (var i = 0; i < item.SerialKeys.Count; i++)
							{
								setPromotionItems.Append("\r\n");
								setPromotionItems.Append((i == 0) ? " シリアルキー  ：" : "　　 　　　　  ：");
								setPromotionItems.Append(SerialKeyUtility.GetFormattedKeyString(SerialKeyUtility.DecryptSerialKey(item.SerialKeys[i])));
							}
						}

						itemIndex++;
					}

					orderItemsString.Append(setPromotionItems.ToString());
					cartProductIndex++;
				}
			}
			// 注文商品情報を格納
			mailDatas.Add("order_items", orderItemsString.ToString());
		}

		/// <summary>
		/// 注文商品情報を注文商品情報文字列に追加
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="item">カート商品情報</param>
		/// <param name="orderItemString">注文商品情報文字列</param>
		/// <param name="isNormalProduct">通常商品か</param>
		/// <param name="setPromotion">セットプロモーション</param>
		/// <param name="productCount">商品数量</param>
		private void AppendOrderItemStringBuilder(
			CartObject cart,
			CartProduct item,
			StringBuilder orderItemString,
			bool isNormalProduct,
			CartSetPromotion setPromotion = null,
			int productCount = 0)
		{
			// 通常商品の場合はメールテンプレートに出力する際はインデントを付けない
			var indent = isNormalProduct ? string.Empty : " ";

			orderItemString
				.Append(indent + MailContentsNameHelper.GetContentsName(MailContentsNameHelper.ContentsDispNameType.ProductId, cart))
				.Append(item.VariationId)
				.Append("\r\n")
				.Append(indent + MailContentsNameHelper.GetContentsName(MailContentsNameHelper.ContentsDispNameType.ProductName, cart))
				.Append(item.ProductJointName)
				.Append("\r\n");

			// 付帯情報がある場合は商品名に続けて表示 ※この辺りの改修はCroozMollに影響
			if ((item.ProductOptionSettingList != null) && item.ProductOptionSettingList.IsSelectedProductOptionValueAll)
			{
				orderItemString = ProductOptionSettingHelper.GetProductOptionTextForMailTemplate(
					item.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues(),
					orderItemString);
			}

			orderItemString
				.Append(indent + MailContentsNameHelper.GetContentsName(MailContentsNameHelper.ContentsDispNameType.ProductPrice, cart)).Append(ConvertPriceForMailByConfig(item.Price))
				.Append(GetProductTaxText(cart.Owner.DispLanguageLocaleId))
				.Append("\r\n")
				.Append(indent + MailContentsNameHelper.GetContentsName(MailContentsNameHelper.ContentsDispNameType.Quantity, cart));
			if (item.IsSetItem)
			{
				orderItemString.Append(item.CountSingle);
			}
			else if (setPromotion != null)
			{
				orderItemString.Append(item.QuantityAllocatedToSet[setPromotion.CartSetPromotionNo]);
			}
			else
			{
				orderItemString.Append(productCount);
			}
		}

		/// <summary>
		/// Set order items loop to hashtable for order mail templete
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <param name="productItems">Product items</param>
		/// <param name="mailDatas">Mail datas</param>
		private void SetOrderItemsLoopToHashtableForOrderMailTemplete(
			CartObject cart,
			IList productItems,
			Hashtable mailDatas)
		{
			var displayProductItems = (cart.IsGift
					? ((List<CartShipping.ProductCount>)productItems).Select(item => new Tuple<CartProduct, int>(item.Product, item.Count))
					: ((List<CartProduct>)productItems).Select(item => new Tuple<CartProduct, int>(item, item.QuantitiyUnallocatedToSet)))
				.Where(item => (item.Item1.OrderHistoryDisplayType == Constants.FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_VALID))
				.ToArray();

			// セット商品の場合
			var cartProductSetGroups = displayProductItems
				.Where(item => item.Item1.IsSetItem)
				.GroupBy(item => item.Item1.ProductSet.ProductSetId)
				.ToArray();
			var orderItemsLoopList = new List<Hashtable>();
			var cartProductIndex = 1;
			var productService = DomainFacade.Instance.ProductService;
			foreach (var group in cartProductSetGroups)
			{
				var cartProductSetItemIndex = 1;
				var productSetCurrent = group.First().Item1.ProductSet;
				var setProductList = new List<Hashtable>();
				var itemPriceTotal = 0m;
				foreach (var product in productSetCurrent.Items)
				{
					itemPriceTotal += product.PriceSubtotalSingle;
					var productInfo = productService.GetProductVariation(
						product.ShopId,
						product.ProductId,
						product.VariationId,
						cart.MemberRankId);
					var productUrl = ProductCommon.CreateProductDetailUrlForSendMail(
						productInfo.ShopId,
						productInfo.ProductId,
						productInfo.VariationId,
						productInfo.Name);
					setProductList.Add(new Hashtable
					{
						{ Constants.FIELD_PRODUCT_SHOP_ID, product.ShopId },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_INDEX, cartProductSetItemIndex },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_ID, product.VariationId },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_NAME, product.ProductJointName },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_IMAGE, productInfo.ImageHead },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_VARIATION_IMAGE, productInfo.VariationImageHead },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_OUTLINE, productInfo.Outline },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_DETAIL1, productInfo.DescDetail1 },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_DETAIL2, productInfo.DescDetail2 },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_DETAIL3, productInfo.DescDetail3 },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_DETAIL4, productInfo.DescDetail4 },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_LINK, productUrl },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_PRICE, ConvertPriceForMail(product.Price) },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_QUANTITY, StringUtility.ToNumeric(product.CountSingle) },
						{ Constants.MAILTAG_IS_ORDER_SET_PRODUCT_VARIATION, product.HasVariation },
					});

					cartProductSetItemIndex++;
				}

				orderItemsLoopList.Add(new Hashtable
				{
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_INDEX, cartProductIndex },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_ITEMS, setProductList },
					{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_TOTAL_QUANTITY, StringUtility.ToNumeric(productSetCurrent.ProductSetCount) },
					{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_TOTAL_PRICE, ConvertPriceForMail(itemPriceTotal) },
					{ Constants.MAILTAG_ORDER_ITEM_TYPE, Constants.MAILTAG_ORDER_SET_PRODUCTS_LOOP },
				});

				cartProductIndex++;
			}

			// セットプロモーション商品を追加（ギフトではない場合のみ）
			if (cart.IsGift == false)
			{
				foreach (CartSetPromotion setPromotion in cart.SetPromotions)
				{
					var setPromotionIndex = 1;
					var setPromotionProductList = new List<Hashtable>();
					foreach (var item in setPromotion.Items.Where(
						item => (item.OrderHistoryDisplayType == Constants.FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_VALID)))
					{
						var productOptionSettingSelectValues = string.Empty;
						if ((item.ProductOptionSettingList != null)
						  && (item.ProductOptionSettingList.IsSelectedProductOptionValueAll))
						{
							productOptionSettingSelectValues = ProductOptionSettingHelper.GetDisplayProductOptionTexts(
								item.ProductOptionSettingList.GetJsonStringFromSelectValues());
						}
						var productInfo = productService.GetProductVariation(
							item.ShopId,
							item.ProductId,
							item.VariationId,
							cart.MemberRankId);
						var productUrl = ProductCommon.CreateProductDetailUrlForSendMail(
							productInfo.ShopId,
							productInfo.ProductId,
							productInfo.VariationId,
							productInfo.Name);
						var orderItem = new Hashtable
						{
							{ Constants.FIELD_PRODUCT_SHOP_ID, item.ShopId },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_INDEX, setPromotionIndex },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_ID, item.VariationId },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_NAME, item.ProductJointName },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_IMAGE, productInfo.ImageHead },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_VARIATION_IMAGE, productInfo.VariationImageHead },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_OUTLINE, productInfo.Outline },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_DETAIL1, productInfo.DescDetail1 },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_DETAIL2, productInfo.DescDetail2 },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_DETAIL3, productInfo.DescDetail3 },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_DETAIL4, productInfo.DescDetail4 },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_LINK, productUrl },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_PRICE, ConvertPriceForMail(item.Price) },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_QUANTITY, StringUtility.ToNumeric(item.QuantityAllocatedToSet[setPromotion.CartSetPromotionNo]) },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_OPTION, ProductOptionSettingHelper.GetDisplayProductOptionTexts(productOptionSettingSelectValues) },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_DISP_NAME, setPromotion.SetpromotionDispName },
							{ Constants.MAILTAG_IS_ORDER_SET_PROMOTION_PRODUCT_VARIATION, item.HasVariation },
						};

						// デジタルコンテンツ対応
						if (cart.IsDigitalContentsOnly && item.IsDelivered)
						{
							var serialKeys = item.SerialKeys
								.Select(serialKey => new Hashtable
								{
									{
										Constants.MAILTAG_ORDER_ITEM_PRODUCT_SERIAL_KEY,
										SerialKeyUtility.GetFormattedKeyString(SerialKeyUtility.DecryptSerialKey(serialKey))
									},
								})
								.ToArray();
							if (serialKeys.Length > 0) orderItem.Add(Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCT_SERIAL_KEYS_LOOP, serialKeys);
						}

						setPromotionProductList.Add(orderItem);
						setPromotionIndex++;
					}

					orderItemsLoopList.Add(new Hashtable
					{
						{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_INDEX, cartProductIndex },
						{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_ITEMS, setPromotionProductList },
						{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_DISP_NAME, setPromotion.SetpromotionDispName},
						{ Constants.MAILTAG_ORDER_ITEM_TYPE, Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCTS_LOOP },
					});
					cartProductIndex++;
				}
			}

			// ギフト注文の場合、またはセットプロモーションではない商品が0ではない場合
			var cartProductNormals = displayProductItems
				.Where(item => ((item.Item1.IsSetItem == false)
					&& (item.Item1.QuantitiyUnallocatedToSet != 0)))
				.ToArray();
			foreach (var item in cartProductNormals)
			{
				var productCurrent = item.Item1;
				var productCurrentCount = item.Item2;
				var productOptionSettingSelectValues = string.Empty;
				if ((productCurrent.ProductOptionSettingList != null)
					&& (productCurrent.ProductOptionSettingList.IsSelectedProductOptionValueAll))
				{
					productOptionSettingSelectValues = ProductOptionSettingHelper.GetDisplayProductOptionTexts(
						productCurrent.ProductOptionSettingList.GetJsonStringFromSelectValues());
				}
				var productInfo = productService.GetProductVariation(
					productCurrent.ShopId,
					productCurrent.ProductId,
					productCurrent.VariationId,
					cart.MemberRankId);
				var productUrl = ProductCommon.CreateProductDetailUrlForSendMail(
					productInfo.ShopId,
					productInfo.ProductId,
					productInfo.VariationId,
					productInfo.Name);
				var orderItem = new Hashtable
				{
					{ Constants.FIELD_PRODUCT_SHOP_ID, productCurrent.ShopId },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_INDEX, cartProductIndex },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_ID, productCurrent.VariationId},
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_NAME, productCurrent.ProductJointName },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_IMAGE, productInfo.ImageHead },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_VARIATION_IMAGE, productInfo.VariationImageHead },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_OUTLINE, productInfo.Outline },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_DETAIL1, productInfo.DescDetail1 },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_DETAIL2, productInfo.DescDetail2 },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_DETAIL3, productInfo.DescDetail3 },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_DETAIL4, productInfo.DescDetail4 },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_LINK, productUrl },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_PRICE, ConvertPriceForMail(productCurrent.Price) },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_QUANTITY, StringUtility.ToNumeric(productCurrentCount) },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_OPTION, productOptionSettingSelectValues },
					{ Constants.MAILTAG_IS_ORDER_PRODUCT_VARIATION, productCurrent.HasVariation },
					{ Constants.MAILTAG_ORDER_ITEM_TYPE, Constants.MAILTAG_ORDER_NORMAL_PRODUCT },
				};

				// デジタルコンテンツ対応
				if (cart.IsDigitalContentsOnly && productCurrent.IsDelivered)
				{
					var serialKeys = productCurrent.SerialKeys
						.Select(serialKey => new Hashtable
							{
								{
									Constants.MAILTAG_ORDER_ITEM_PRODUCT_SERIAL_KEY,
									SerialKeyUtility.GetFormattedKeyString(SerialKeyUtility.DecryptSerialKey(serialKey))
								},
							})
						.ToArray();
					if (serialKeys.Length > 0) orderItem.Add(Constants.MAILTAG_ORDER_PRODUCT_SERIAL_KEYS_LOOP, serialKeys);
				}

				orderItemsLoopList.Add(orderItem);
				cartProductIndex++;
			}

			// 注文商品情報を格納
			mailDatas.Add(Constants.MAILTAG_ORDER_ITEMS_LOOP, orderItemsLoopList.ToArray());
		}

		/// <summary>
		/// 注文商品の商品IDリストを取得
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <returns>注文商品の商品IDリスト</returns>
		private List<string> GetOrderItemsProductId(CartObject cart)
		{
			var displayProductItems = cart.Items
				.Where(item => (item.OrderHistoryDisplayType == Constants.FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_VALID))
				.ToArray();

			var result = GetProductIdListByCartProducts(displayProductItems);

			// セットプロモーション商品を追加
			var setPromotionItems = cart.SetPromotions.Items
				.SelectMany(
					setPromotion => setPromotion.Items.Where(
						item => (item.OrderHistoryDisplayType == Constants.FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_VALID)))
				.ToArray();
			if (setPromotionItems.Any()) result.AddRange(setPromotionItems.Select(v => v.ProductId));

			return result;
		}

		/// <summary>
		/// ギフト用の注文商品の商品IDリストを取得
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <returns>注文商品の商品IDリスト</returns>
		private List<string> GetOrderItemsProductIdForGift(CartObject cart)
		{
			var productItems = cart.Shippings.SelectMany(
					cartShipping => cartShipping.ProductCounts
						.Select(item => item.Product)
						.Where(product => (product.OrderHistoryDisplayType == Constants.FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_VALID)))
				.ToArray();

			var result = GetProductIdListByCartProducts(productItems);
			return result;
		}

		/// <summary>
		/// カート内商品情報一覧から商品IDリストを作成
		/// </summary>
		/// <param name="items">カート内商品情報一覧</param>
		/// <returns>商品IDリスト</returns>
		private List<string> GetProductIdListByCartProducts(CartProduct[] items)
		{
			var result = new List<string>();
			foreach (var productItem in items)
			{
				if (productItem.IsSetItem)
				{
					// セット商品の場合
					var productSetCurrent = productItem.ProductSet;
					result.AddRange(productSetCurrent.Items.Select(v => v.ProductId).ToList());
				}
				else if (productItem.QuantitiyUnallocatedToSet != 0)
				{
					result.Add(productItem.ProductId);
				}
			}

			return result;
		}

		/// <summary>
		/// 商品情報のHashtableセット（注文メールテンプレート用 & ネクストエンジン用）
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="productItems">商品アイテム</param>
		/// <param name="mailDatas">メールデータ</param>
		private void SetOrderItemToHashtableForNextEngineOrderMailTemplete(
			CartObject cart,
			IList productItems,
			Hashtable mailDatas)
		{
			var displayProductItems = (cart.IsGift
					? ((List<CartShipping.ProductCount>)productItems)
					.Select(item => new Tuple<CartProduct, int>(item.Product, item.Count))
					: ((List<CartProduct>)productItems)
					.Select(item => new Tuple<CartProduct, int>(item, item.QuantitiyUnallocatedToSet)))
				.ToList();
			var orderItemsText = new StringBuilder();
			var cartItemIndex = 0;
			while (cartItemIndex < displayProductItems.Count)
			{
				if (displayProductItems[cartItemIndex].Item1.IsSetItem)
				{
					// セット商品の場合
					var cartProductSetItemIndex = 0;
					var productSetCurrent = displayProductItems[cartItemIndex].Item1.ProductSet;

					while (cartProductSetItemIndex < productSetCurrent.Items.Count)
					{
						var productCurrent = productSetCurrent.Items[cartProductSetItemIndex];
						var productOptionSettingSelectValues = "";
						if ((productCurrent.ProductOptionSettingList != null)
							&& (productCurrent.ProductOptionSettingList.IsSelectedProductOptionValueAll))
						{
							productOptionSettingSelectValues = ProductOptionSettingHelper.GetDisplayProductOptionTexts(
								productCurrent.ProductOptionSettingList.GetJsonStringFromSelectValues());
						}

						var orderItemText = CreateNextEngineOrderMailTempleteFormat(
							productCurrent.VariationId,
							productCurrent.ProductJointName,
							productOptionSettingSelectValues,
							productCurrent.Price,
							productCurrent.CountSingle,
							productCurrent.PriceSubtotalSingle);
						orderItemsText.Append(orderItemText);

						cartItemIndex++;
						cartProductSetItemIndex++;
					}
				}
				else if (cart.IsGift || (displayProductItems[cartItemIndex].Item1.QuantitiyUnallocatedToSet != 0))
				{
					// ギフト注文の場合、またはセットプロモーションではない商品が0ではない場合
					var productOptionSettingSelectValues = "";
					if ((displayProductItems[cartItemIndex].Item1.ProductOptionSettingList != null)
						&& (displayProductItems[cartItemIndex].Item1.ProductOptionSettingList.IsSelectedProductOptionValueAll))
					{
						productOptionSettingSelectValues = ProductOptionSettingHelper.GetDisplayProductOptionTexts(
							displayProductItems[cartItemIndex].Item1.ProductOptionSettingList.GetJsonStringFromSelectValues());
					}
					var orderItemText = CreateNextEngineOrderMailTempleteFormat(
						displayProductItems[cartItemIndex].Item1.VariationId,
						displayProductItems[cartItemIndex].Item1.ProductJointName,
						productOptionSettingSelectValues,
						displayProductItems[cartItemIndex].Item1.Price,
						displayProductItems[cartItemIndex].Item2,
						displayProductItems[cartItemIndex].Item1.PriceSubtotal);
					orderItemsText.Append(orderItemText);

					cartItemIndex++;
				}
				else
				{
					cartItemIndex++;
				}
			}

			// セットプロモーション商品を追加（ギフトではない場合のみ）
			if (cart.IsGift == false)
			{
				foreach (CartSetPromotion setPromotion in cart.SetPromotions)
				{
					foreach (var item in setPromotion.Items)
					{
						var productOptionSettingSelectValues = "";
						if ((item.ProductOptionSettingList != null)
							&& (item.ProductOptionSettingList.IsSelectedProductOptionValueAll))
						{
							productOptionSettingSelectValues = ProductOptionSettingHelper.GetDisplayProductOptionTexts(
								item.ProductOptionSettingList.GetJsonStringFromSelectValues());
						}
						var orderItemText = CreateNextEngineOrderMailTempleteFormat(
							item.VariationId,
							item.ProductJointName,
							productOptionSettingSelectValues,
							item.Price,
							item.QuantityAllocatedToSet[setPromotion.CartSetPromotionNo],
							item.PriceSubtotal);
						orderItemsText.Append(orderItemText);
					}
				}
			}

			if (orderItemsText.Length > 0)
			{
				orderItemsText.Append("------------------------------------------------------------");
			}

			// 注文商品情報を格納
			mailDatas.Add("order_items_nextengine", orderItemsText.ToString());
		}

		/// <summary>
		/// ギフト用の配送・商品の情報作成
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="mailDatas">メールデータ</param>
		/// <param name="shippingNo">配送先番号</param>
		/// <param name="isToNextEngine">ネクストエンジンへのメール送信か</param>
		private void CreateOfShippingAndProductForGift(CartObject cart, Hashtable mailDatas, int shippingNo = 0, bool isToNextEngine = false)
		{
			if (cart.IsGift)
			{
				var shippingList = new List<Hashtable>();
				if (Constants.NE_OPTION_ENABLED && isToNextEngine)
				{
					SetOrderShippingToHashtableForOrderMailTemplete(cart.Shippings[shippingNo], mailDatas);
					SetOrderItemsLoopToHashtableForOrderMailTemplete(cart, cart.Shippings[shippingNo].ProductCounts, mailDatas);
					SetOrderItemToHashtableForNextEngineOrderMailTemplete(cart, cart.Shippings[shippingNo].ProductCounts, mailDatas);
					shippingList.Add(mailDatas);
				}
				else
				{
					foreach (var cartShipping in cart.Shippings)
					{
						var shipping = new Hashtable
						{
							{ Constants.FIELD_ORDER_ORDER_ID, cart.OrderId }
						};
						SetOrderShippingToHashtableForOrderMailTemplete(cartShipping, shipping);
						SetOrderItemToHashtableForOrderMailTemplete(cart, cartShipping.ProductCounts, shipping);
						SetOrderItemsLoopToHashtableForOrderMailTemplete(cart, cartShipping.ProductCounts, shipping);
						shippingList.Add(shipping);
					}
				}
				mailDatas.Add("GiftShippingLoop", shippingList);
			}
			else
			{
				SetOrderShippingToHashtableForOrderMailTemplete(cart.Shippings[0], mailDatas);
			}
		}

		/// <summary>
		/// 別途見積りのためのメッセージ取得
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <returns>PC,Mobileサイトに合わせたメッセージ</returns>
		private string GetMessageForSeparateEstimate(CartObject cart)
		{
			var message = (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED && cart.ShippingPriceSeparateEstimateFlg)
				? this.IsPc
					? OrderCommon.GetShopShipping(cart.ShopId, cart.ShippingType).ShippingPriceSeparateEstimatesMessage
					: OrderCommon.GetShopShipping(cart.ShopId, cart.ShippingType).ShippingPriceSeparateEstimatesMessageMobile
				: string.Empty;
			return message;
		}
	}
}
