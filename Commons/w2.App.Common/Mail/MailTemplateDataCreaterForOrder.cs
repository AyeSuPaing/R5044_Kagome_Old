/*
=========================================================================================================
  Module      : 注文から注文メールテンプレートデータを作成(MailTemplateDataCreaterForOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Global;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.NewebPay;
using w2.App.Common.Product;
using w2.App.Common.Util;
using w2.App.Common.Web.Page;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.RealShop;
using w2.Domain.User;

namespace w2.App.Common.Mail
{
	/// <summary>
	/// 注文から注文メールテンプレートデータを作成
	/// </summary>
	public class MailTemplateDataCreaterForOrder : MailTemplateDataCreaterBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="isPc">注文区分がPCか</param>
		public MailTemplateDataCreaterForOrder(bool isPc)
			: base(isPc)
		{
		}

		/// <summary>
		/// 注文メールデータ取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>メールテンプレート用ハッシュテーブル</returns>
		public Hashtable GetOrderMailDatas(string orderId)
		{
			// 注文情報取得
			var orderDatasModel = new OrderService().GetForOrderMail(orderId);
			var orderDatas = orderDatasModel.Select(od => od.DataSource).ToArray();
			
			if (orderDatas.Length == 0) return new Hashtable();

			// PC・モバイル判定してボーダー文字決定
			if (Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED == false)
			{
				this.IsPc = ((string)orderDatas[0][Constants.FIELD_ORDER_ORDER_KBN] != Constants.FLG_ORDER_ORDER_KBN_MOBILE);
			}
			// 初期化し直す
			Initialize(this.IsPc);

			// Hashtableに値をセット
			var mailDatas = new Hashtable();
			foreach (var key in orderDatas[0].Keys)
			{
				// 数値(int)及びdecimalで金額以外の場合はカンマ区切り文字をセット
				// decimalで金額の場合は決済通貨に合わせて小数点以下を補正
				// それ以外はそのまま格納
				mailDatas.Add(
					key,
					(orderDatas[0][key] is int
						|| ((string)key == Constants.FIELD_ORDER_ORDER_POINT_USE)
						|| ((string)key == Constants.FIELD_ORDER_ORDER_POINT_ADD)
						|| ((string)key == Constants.FIELD_ORDER_LAST_ORDER_POINT_USE))
						? StringUtility.ToNumeric(orderDatas[0][key])
						: (orderDatas[0][key] is decimal)
							? ConvertPriceForMail(orderDatas[0][key])
							: StringUtility.ToEmpty(orderDatas[0][key]));
			}

			var globalPrice = CurrencyManager.ToPrice(
				(string)orderDatas[0][Constants.FIELD_ORDEROWNER_DISP_CURRENCY_CODE],
				(string)orderDatas[0][Constants.FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID],
				(decimal)orderDatas[0][Constants.FIELD_ORDER_ORDER_PRICE_TOTAL]);
			mailDatas.Add("order_price_total_global", globalPrice);
			// 注文者情報　区分値
			mailDatas.Add(
				Constants.FIELD_ORDEROWNER_OWNER_KBN + "_text",
				ValueText.GetValueText(
					Constants.TABLE_ORDEROWNER,
					Constants.FIELD_ORDEROWNER_OWNER_KBN,
					(string)mailDatas[Constants.FIELD_ORDEROWNER_OWNER_KBN]));
			mailDatas.Add(
				Constants.FIELD_ORDEROWNER_OWNER_SEX + "_text",
				ValueText.GetValueText(
					Constants.TABLE_ORDEROWNER,
					Constants.FIELD_ORDEROWNER_OWNER_SEX,
					(string)mailDatas[Constants.FIELD_ORDEROWNER_OWNER_SEX]));

			// 該当な日付形式へ変換のため、注文者の言語ロケールIDを取得
			var languageLocaleId = (string)orderDatas[0][Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID];

			// 生年月日の時分秒削除
			mailDatas[Constants.FIELD_USER_BIRTH] = DateTimeUtility.ToString(
					mailDatas[Constants.FIELD_USER_BIRTH],
					DateTimeUtility.FormatType.ShortDate2Letter,
					languageLocaleId);
			mailDatas[Constants.FIELD_ORDEROWNER_OWNER_BIRTH] = DateTimeUtility.ToString(
				mailDatas[Constants.FIELD_ORDEROWNER_OWNER_BIRTH],
				DateTimeUtility.FormatType.ShortDate2Letter,
				languageLocaleId);
			// 注文者年月日に関する管理者向けのタグ用を追加
			mailDatas[Constants.FIELD_ORDEROWNER_OWNER_BIRTH + STRING_FOR_OPERATOR] =
				DateTimeUtility.ToStringForManager(
					orderDatas[0][Constants.FIELD_ORDEROWNER_OWNER_BIRTH],
					DateTimeUtility.FormatType.ShortDate2Letter);

			// 注文日を該当な形式に変換
			mailDatas[Constants.FIELD_ORDER_ORDER_DATE] = DateTimeUtility.ToString(
				mailDatas[Constants.FIELD_ORDER_ORDER_DATE],
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
				languageLocaleId);
			// 注文日に関する管理者向けのタグ用を追加
			mailDatas[Constants.FIELD_ORDER_ORDER_DATE + STRING_FOR_OPERATOR] = DateTimeUtility.ToStringForManager(
				orderDatas[0][Constants.FIELD_ORDER_ORDER_DATE],
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			// ネクストエンジン用注文日時
			mailDatas.Add(
				Constants.FIELD_ORDER_ORDER_DATE + STRING_FOR_NEXTENGINE,
				DateTimeUtility.ToStringForManager(
					orderDatas[0][Constants.FIELD_ORDER_ORDER_DATE],
					DateTimeUtility.FormatType.LongDateHourMinuteSecondNoneServerTime));

			// ネクストエンジン用注文者電話番号
			mailDatas.Add(
				Constants.FIELD_ORDEROWNER_OWNER_TEL1 + STRING_FOR_NEXTENGINE,
				((string)orderDatas[0][Constants.FIELD_ORDEROWNER_OWNER_TEL1]).Replace("-", "")
			);

			// 購入履歴一覧画面URL
			mailDatas.Add(
				"order_history_list_url",
				(string.IsNullOrEmpty(Constants.URL_FRONT_PC)) ?
					"" :
					Constants.URL_FRONT_PC.Replace(Constants.PROTOCOL_HTTP, Constants.PROTOCOL_HTTPS) + Constants.PAGE_FRONT_ORDER_HISTORY_LIST);

			// 注文メモの入力がなければ「(指定なし)」とする
			if (((string)mailDatas[Constants.FIELD_ORDER_MEMO]).Length == 0)
			{
				mailDatas[Constants.FIELD_ORDER_MEMO] = string.Format("({0})", CommonPage.ReplaceTagByLocaleId("@@DispText.common_message.unspecified@@", languageLocaleId));
			}

			// カート分割回数格納
			if ((string)mailDatas[Constants.FIELD_ORDER_CARD_INSTRUMENTS] != "")
			{
				mailDatas[Constants.FIELD_PAYMENT_PAYMENT_NAME]
					= (string)mailDatas[Constants.FIELD_PAYMENT_PAYMENT_NAME] + "(" + (string)mailDatas[Constants.FIELD_ORDER_CARD_INSTRUMENTS] + ")";
			}

			// メール送信先指定（注文区分で判定）
			mailDatas.Add("is_pc", this.IsPc);

			// ギフト配送先・商品情報作成
			var isGift = ((string)mailDatas[Constants.FIELD_ORDER_GIFT_FLG] == Constants.FLG_ORDER_GIFT_FLG_ON);
			if (isGift)
			{
				// 先ずはループするためのデータを作成する
				var shippingProducts = new List<List<Hashtable>>();
				List<Hashtable> shippingProduct = null;
				var orderShippingNoCurrent = 0;
				foreach (var item in orderDatas)
				{
					var orderShippingNo = (int)item[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO];
					if (orderShippingNoCurrent != orderShippingNo)
					{
						orderShippingNoCurrent = orderShippingNo;

						shippingProduct = new List<Hashtable>();
						shippingProducts.Add(shippingProduct);
					}
					shippingProduct.Add(item);

					// Get taiwan invoice data for send mail
					OrderCommon.GetTwInvoiceDataForSendMail(orderId, orderShippingNo, mailDatas);
				}

				var shippings = new List<Hashtable>();
				foreach (var items in shippingProducts)
				{
					// 配送先セット
					var shipping = new Hashtable();
					foreach (var key in items[0].Keys)
					{
						shipping.Add(
							key,
							((items[0][key] is decimal) || (items[0][key] is int))
								? StringUtility.ToNumeric(items[0][key])
								: StringUtility.ToEmpty(items[0][key]));
					}
					shipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE]
						= ((string)shipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE] != "")
							? shipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_TYPE]
							: "指定なし";
					shipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME]
						= ((string)shipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME] != "")
							? shipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_PAPER_NAME]
							: "指定なし";
					shipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE]
						= ((string)shipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE] != "")
							? shipping[Constants.FIELD_ORDERSHIPPING_WRAPPING_BAG_TYPE]
							: "指定なし";

					// 配送希望時間帯セット
					SetOrderShippingDateTimeForOrderMailTemplete(shipping, languageLocaleId);

					// 配送種別名セット
					shipping.Add("shipping_method_name",
						ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, (string)shipping[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD]));

					// 注文商品セット
					shipping.Add("order_items", GetOrderItemsStringForOrderMailTemplete(orderDatasModel[0], items, true));
					shipping.Add(
						Constants.MAILTAG_ORDER_ITEMS_LOOP,
						GetOrderItemsLoopsForOrderMailTemplete(
							orderDatasModel[0],
							items,
							true));
					shipping.Add(
						"order_items_nextengine",
						GetOrderItemsStringForNexteEgineOrderMailTemplete(
							items,
							true));

					shippings.Add(shipping);
				}
				mailDatas.Add("GiftShippingLoop", shippings);
			}
			else
			{
				// 注文商品セット
				var orderItems = orderDatas.ToList();
				mailDatas.Add("order_items", GetOrderItemsStringForOrderMailTemplete(orderDatasModel[0], orderItems, false));
				mailDatas.Add(
					Constants.MAILTAG_ORDER_ITEMS_LOOP,
					GetOrderItemsLoopsForOrderMailTemplete(
						orderDatasModel[0],
						orderItems,
						false));
				mailDatas.Add(
					"order_items_nextengine",
					GetOrderItemsStringForNexteEgineOrderMailTemplete(
						orderItems,
						false));

				// 定期購入情報セット
				var fixedPurchaseId = StringUtility.ToEmpty(mailDatas[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID]);
				if (fixedPurchaseId != "")
				{
					var service = new FixedPurchaseService();
					var fixedPurchase = service.Get(fixedPurchaseId);
					mailDatas.Add("fixed_purchase_pattern", OrderCommon.CreateFixedPurchaseSettingMessage(fixedPurchase));
					var tmpShippingDate = StringUtility.ToEmpty(mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE]);
					var shippingDate = string.IsNullOrEmpty(tmpShippingDate) ? (DateTime?)null : DateTime.Parse(tmpShippingDate);

					// 初回配送予定日
					DateTime firstShippingDate;
					var isTaiwanCountryShippingEnable = (Constants.TW_COUNTRY_SHIPPING_ENABLE
						&& GlobalAddressUtil.IsCountryTw(mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE].ToString()));
					var prefecture = isTaiwanCountryShippingEnable
						? mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2].ToString()
						: mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1].ToString();
					if (orderDatasModel[0].OrderDate.HasValue)
					{
						firstShippingDate = OrderCommon.GetFirstShippingDateBasedOnFisrtOrderDate(
							orderDatasModel[0].ShopId,
							int.Parse(
								mailDatas[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED].ToString()),
							shippingDate,
							mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD].ToString(),
							mailDatas[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID].ToString(),
							mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE].ToString(),
							prefecture,
							mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP].ToString(),
							(DateTime)orderDatasModel[0].OrderDate);
					}
					else
					{
						firstShippingDate = OrderCommon.GetFirstShippingDateBasedOnToday(
							orderDatasModel[0].ShopId,
							int.Parse(
								mailDatas[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED].ToString()),
							shippingDate,
							mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD].ToString(),
							mailDatas[Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID].ToString(),
							mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE].ToString(),
							prefecture,
							mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP].ToString());
					}

					AddLongDateTagsForUser(
						mailDatas,
						Constants.FIELD_MAIL_FIELD_FIRST_SHIPPING_DATE,
						firstShippingDate,
						languageLocaleId);
					AddLongDateTagsForUser(
						mailDatas,
						Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE,
						fixedPurchase.NextShippingDate.Value,
						languageLocaleId);
					AddLongDateTagsForUser(
						mailDatas,
						Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE,
						fixedPurchase.NextNextShippingDate.Value,
						languageLocaleId);

					// 日付系の管理者向けのタグ用を追加
					AddLongDateTagsForManager(
						mailDatas,
						Constants.FIELD_MAIL_FIELD_FIRST_SHIPPING_DATE,
						firstShippingDate);
					AddLongDateTagsForManager(
						mailDatas,
						Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE,
						fixedPurchase.NextShippingDate.Value);
					AddLongDateTagsForManager(
						mailDatas,
						Constants.FIELD_FIXEDPURCHASE_NEXT_NEXT_SHIPPING_DATE,
						fixedPurchase.NextNextShippingDate.Value);

					mailDatas[Constants.TAG_FIXED_PURCHASE_MEMO] = fixedPurchase.Memo;
				}

				// 配送希望時間帯セット
				SetOrderShippingDateTimeForOrderMailTemplete(mailDatas, languageLocaleId);

				// 配送種別名セット
				mailDatas.Add("shipping_method_name", ValueText.GetValueText(
						Constants.TABLE_ORDERSHIPPING,
						Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD,
						(string)mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD]));

				// Get taiwan invoice data for send mail
				var orderShippingNo = (int)orderDatas[0][Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO];
				OrderCommon.GetTwInvoiceDataForSendMail(orderId, orderShippingNo, mailDatas);
			}
			// 配送料別途見積もり表示メッセージ
			mailDatas.Add("separate_estimates_message", GetMessageForSeparateEstimate(orderDatas));

			var orderSetPromotions = orderDatas.Where(oi => oi[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO].ToString() == "1");
			// セットプロモーション割引情報セット(商品割引分)
			var setPromotionProductDiscount = orderSetPromotions
				.Where(oi => (string)StringUtility.ToValue(oi[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_FLG], "0") == Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_FLG_ON)
				.Select(orderItem => new Hashtable
				{
					{Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME, orderItem[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME]},
					{Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT, ConvertPriceForMail(orderItem[Constants.FIELD_ORDERSETPROMOTION_PRODUCT_DISCOUNT_AMOUNT])}
				}).ToList();

			if (setPromotionProductDiscount.Count != 0)
			{
				mailDatas.Add("SetPromotionProductDiscountLoop", setPromotionProductDiscount);
			}

			// セットプロモーション割引情報セット(配送料割引分)
			var setPromotionShippingChargeDiscount = orderSetPromotions
				.Where(oi => (string)StringUtility.ToValue(oi[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG], "0") == Constants.FLG_SETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON)
				.Select(orderItem => new Hashtable
				{
					{Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME, orderItem[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME]},
					{Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT, ConvertPriceForMail(orderItem[Constants.FIELD_ORDERSETPROMOTION_SHIPPING_CHARGE_DISCOUNT_AMOUNT])
				}}).ToList();

			if (setPromotionShippingChargeDiscount.Count != 0)
			{
				mailDatas.Add("SetPromotionShippingChargeDiscountLoop", setPromotionShippingChargeDiscount);
			}

			// セットプロモーション割引情報セット(決済手数料割引分)
			var setPromotionPaymentChargeDiscount = orderSetPromotions
				.Where(oi => (string)StringUtility.ToValue(oi[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG], "0") == Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON)
				.Select(orderItem => new Hashtable
				{
					{Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME, orderItem[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME]},
					{Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT, ConvertPriceForMail(orderItem[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_DISCOUNT_AMOUNT])}
				}).ToList();

			if (setPromotionPaymentChargeDiscount.Count != 0)
			{
				mailDatas.Add("SetPromotionPaymentChargeDiscountLoop", setPromotionPaymentChargeDiscount);
			}

			// セットプロモーション割引額合計
			mailDatas.Add("setpromotion_total_discount_amount", ConvertPriceForMail(orderDatasModel[0].SetpromotionTotalDiscountAmount));

			//------------------------------------------------------
			// 決済URLメッセージセット
			//------------------------------------------------------
			var paymentMessage = new StringBuilder();
			if (mailDatas.ContainsKey("payment_url"))
			{
				paymentMessage.Append(mailDatas["payment_url"]);
			}
			if (mailDatas.ContainsKey("payment_message_text"))
			{
				paymentMessage.Append(mailDatas["payment_message_text"].ToString());
			}
			if (StringUtility.ToEmpty(orderDatas[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN])
				== Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
			{
				var externalPaymentType = StringUtility.ToEmpty(orderDatas[0][Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE]);
				var paymentMemo = StringUtility.ToEmpty(orderDatas[0][Constants.FIELD_ORDER_PAYMENT_MEMO]);
				paymentMessage.Append(ECPayUtility.GetPaymentMessageText(externalPaymentType, paymentMemo));
			}
			if (StringUtility.ToEmpty(orderDatas[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN])
				== Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
			{
				var externalPaymentType = StringUtility.ToEmpty(orderDatas[0][Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE]);
				var paymentMemo = StringUtility.ToEmpty(orderDatas[0][Constants.FIELD_ORDER_PAYMENT_MEMO]);
				paymentMessage.Append(NewebPayUtility.GetPaymentMessageText(externalPaymentType, paymentMemo));
			}
			mailDatas.Add("payment_message_text", paymentMessage.ToString());

			//------------------------------------------------------
			// 発行クーポン情報作成
			//------------------------------------------------------
			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				var publishCouponIndex = 1;
				var publishMsgBuilder = new StringBuilder();
				var publishMsgForOperator = new StringBuilder();
				var publishCoupons = new CouponService().GetOrderPublishUserCoupon(
					Constants.W2MP_DEPT_ID,
					(string)mailDatas[Constants.FIELD_ORDER_USER_ID],
					(string)mailDatas[Constants.FIELD_ORDER_ORDER_ID]);
				foreach (var publishCoupon in publishCoupons)
				{
					// 管理者向けとエンドユーザー向けとの情報を区別する
					CreatePublishCouponInfo(publishMsgBuilder, publishCoupon, publishCouponIndex, languageLocaleId);
					CreatePublishCouponInfo(publishMsgForOperator, publishCoupon, publishCouponIndex, Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE);

					publishCouponIndex++;
				}
				mailDatas.Add("publish_coupons", publishMsgBuilder.ToString());
				// 管理者向けのタグ用を追加
				mailDatas.Add("publish_coupons" + STRING_FOR_OPERATOR, publishMsgForOperator.ToString());
			}

			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				// ポイント取得
				var userPoint = PointOptionUtility.GetUserPoint((string)orderDatas[0][Constants.FIELD_USERPOINT_USER_ID]);
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
				mailDatas.Add("after_add_point", StringUtility.ToNumeric(userPoint.PointUsable + Convert.ToDecimal(orderDatas[0][Constants.FIELD_ORDER_ORDER_POINT_ADD])));
			}

			// 割引額合計
			mailDatas.Add("order_price_discount_total", ConvertPriceForMail(orderDatasModel[0].OrderPriceDiscountTotal));

			// 商品税区分
			mailDatas.Add("productPriceTextPrefix", TaxCalculationUtility.GetTaxTypeText());
			// 調整金額に税率毎の返品用金額補正を合算する
			var priceRegulationWithCorrection = orderDatasModel.First().OrderPriceRegulation
				+ new OrderService().GetPriceInfoByTaxRateAll(orderId).Sum(priceInfo => priceInfo.ReturnPriceCorrectionByRate);
			mailDatas[Constants.FIELD_ORDER_ORDER_PRICE_REGULATION] = ConvertPriceForMail(priceRegulationWithCorrection);
			// 領収書ダウンロードＵＲＬ
			AddReceiptDownloadUrl(
				mailDatas,
				(string)orderDatas[0][Constants.FIELD_ORDER_ORDER_ID],
				(string)orderDatas[0][Constants.FIELD_ORDER_RECEIPT_FLG],
				(string)orderDatas[0][Constants.FIELD_ORDER_RECEIPT_ADDRESS],
				(string)orderDatas[0][Constants.FIELD_ORDER_RECEIPT_PROVISO]);

			var subscriptionBoxId = (string)orderDatas[0][Constants.FIELD_SUBSCRIPTIONBOX_SUBSCRIPTION_BOX_COURSE_ID];
			var subscriptionBox = new DataCacheController.SubscriptionBoxCacheController().CacheData
				.FirstOrDefault(sb => (sb.CourseId == subscriptionBoxId));
			var subscriptionBoxDisplayName = (subscriptionBox != null) ? subscriptionBox.DisplayName : "";
			mailDatas.Add(Constants.MAILTAG_SUBSCRIPTION_BOX_DISPLAY_NAME, subscriptionBoxDisplayName);

			SettingOrderExtend(mailDatas, orderDatas[0]);

			var orderItemsProductId = orderDatas
				.Where(
					item => ((string)item[Constants.FIELD_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE]
						== Constants.FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_VALID))
				.Select(o => (string)o[Constants.FIELD_ORDERITEM_PRODUCT_ID])
				.ToArray();
			SetOrderItemsInfo(mailDatas, (string)orderDatas[0][Constants.FIELD_ORDER_SHOP_ID], orderItemsProductId);

			var storePickupRealShopId = (string)orderDatas[0][Constants.FIELD_ORDERSHIPPING_STOREPICKUP_REAL_SHOP_ID];
			if (Constants.STORE_PICKUP_OPTION_ENABLED
				&& (string.IsNullOrEmpty(storePickupRealShopId) == false))
			{
				var realShop = new RealShopService().Get(storePickupRealShopId.ToString());

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
					mailDatas.Add(Constants.MAILTAG_STORE_PICKUP_IS_STOREPICKUPORDER, Constants.MAILTAG_STORE_PICKUP_IS_STOREPICKUPORDER);
				}
			}

			var user = new UserService().Get(orderDatasModel.First().UserId);
			AddMailUnsubscribeTags(mailDatas, user.UserId, user.MailAddr);

			return mailDatas;
		}

		/// <summary>
		/// 注文メールテンプレート用に、注文商品情報の文字列を取得する。
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="orderItems">注文商品</param>
		/// <param name="isGift">ギフト注文かどうか</param>
		/// <returns>注文商品情報文字列</returns>
		private string GetOrderItemsStringForOrderMailTemplete(OrderModel order, List<Hashtable> orderItems, bool isGift)
		{
			//------------------------------------------------------
			// デジタルコンテンツ用にシリアルキー取得
			//------------------------------------------------------
			var orderSerialKeys = new OrderService().GetOrderSerialKeyForOrderMail(order.OrderId).Select(os => os.DataSource).ToArray();

			//------------------------------------------------------
			// 注文商品情報文字列の組み立て
			//------------------------------------------------------
			var displayOrderItems = orderItems.Where(item => (string)item[Constants.FIELD_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE] == Constants.FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_VALID).ToList();
			var orderItemsString = new StringBuilder();
			var index = 0;
			var orderItemNo = 1;
			while (index < displayOrderItems.Count)
			{
				orderItemsString.Append("-(").Append(orderItemNo).Append(")----------" + this.BorderString).Append("\r\n");
				if (((string)displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]).Length != 0)
				{
					// セット商品
					var loop2 = 1;
					var productSetId = StringUtility.ToEmpty(displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]);
					var productSetNo = StringUtility.ToEmpty(displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_SET_NO]);

					var itemPriceTotal = 0m;
					while ((index < displayOrderItems.Count)
						&& (productSetId == StringUtility.ToEmpty(displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]))
						&& (productSetNo == StringUtility.ToEmpty(displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_SET_NO])))
					{
						itemPriceTotal += (decimal)displayOrderItems[index][Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE];

						orderItemsString.Append(" -(").Append(orderItemNo).Append("-").Append(loop2).Append(")----------" + this.BorderString).Append("\r\n");
						AppendOrderItemsStringBuilder(order, orderItemsString, displayOrderItems[index], isNormalProduct: false);
						orderItemsString.Append("\r\n");
						index++;
						loop2++;
					}

					orderItemsString.Append(" --------------" + this.BorderString).Append("\r\n");
					orderItemsString.Append(" セット価格：").Append(ConvertPriceForMailByConfig(itemPriceTotal)).Append(GetProductTaxText()).Append("\r\n");
					orderItemsString.Append(" 数量      ：").Append(StringUtility.ToNumeric(displayOrderItems[index - 1][Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT])).Append("\r\n");
				}
				else if ((isGift == false) && (displayOrderItems[index][Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] != DBNull.Value))
				{
					// セットプロモーション商品
					var orderSetPromotionNo = (int)displayOrderItems[index][Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO];

					if ((int)displayOrderItems[index][Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO] == 1)
					{
						orderItemsString.Append(" ").Append(displayOrderItems[index][Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME]).Append("\r\n");
					}

					int currentOrderSetPromotionNo;
					while ((index < displayOrderItems.Count)
						&& int.TryParse(displayOrderItems[index][Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO].ToString(), out currentOrderSetPromotionNo)
						&& (currentOrderSetPromotionNo == orderSetPromotionNo))
					{
						orderItemsString.Append(" -(").Append(orderItemNo).Append("-").Append(displayOrderItems[index][Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO]).Append(")----------" + this.BorderString).Append("\r\n");

						AppendOrderItemsStringBuilder(order, orderItemsString, displayOrderItems[index], isNormalProduct: false);

						// デジタルコンテンツ対応
						foreach (var ht in orderSerialKeys.Where(ht => ((int)ht[Constants.FIELD_SERIALKEY_ORDER_ITEM_NO] == index + 1)))
						{
							orderItemsString.Append(" シリアルキー　　：");
							orderItemsString.Append(SerialKeyUtility.GetFormattedKeyString(SerialKeyUtility.DecryptSerialKey((string)ht[Constants.FIELD_SERIALKEY_SERIAL_KEY])));
							orderItemsString.Append("\r\n");
						}
						index++;
					}
				}
				else
				{
					// 通常商品
					AppendOrderItemsStringBuilder(order, orderItemsString, displayOrderItems[index], isNormalProduct: true);

					// デジタルコンテンツ対応
					foreach (var orderSerialKey in orderSerialKeys
						.Where(orderSerialKey => ((int)orderSerialKey[Constants.FIELD_SERIALKEY_ORDER_ITEM_NO] == index + 1)))
					{
						var serialKey = (string)orderSerialKey[Constants.FIELD_SERIALKEY_SERIAL_KEY];
						orderItemsString.Append("シリアルキー　　：");
						orderItemsString.Append(SerialKeyUtility.GetFormattedKeyString(SerialKeyUtility.DecryptSerialKey(serialKey)));
						orderItemsString.Append("\r\n");
					}

					index++;
				}
				orderItemNo++;
			}
			// 注文商品情報を返却
			return orderItemsString.ToString().Remove(orderItemsString.ToString().LastIndexOf("\r\n"));
		}

		/// <summary>
		/// 注文商品情報を注文商品情報文字列に追加
		/// </summary>
		/// <param name="order">注文情報モデル</param>
		/// <param name="orderItemsString">注文商品情報文字列</param>
		/// <param name="displayOrderItem">表示注文商品</param>
		/// <param name="isNormalProduct">通常商品か</param>
		/// <remarks>
		/// 通常商品の場合、行頭にインデントを入れるとCrossMoll連携に失敗する為"通常商品か"で判定しています
		/// </remarks>
		private void AppendOrderItemsStringBuilder(OrderModel order, StringBuilder orderItemsString, Hashtable　displayOrderItem, bool isNormalProduct)
		{
			// 通常商品の場合はメールテンプレートに出力する際はインデントを付けない
			var indent = isNormalProduct ? string.Empty : " ";

			orderItemsString
				.Append(indent + MailContentsNameHelper.GetContentsName(MailContentsNameHelper.ContentsDispNameType.ProductId, order))
				.Append((string)displayOrderItem[Constants.FIELD_ORDERITEM_VARIATION_ID])
				.Append("\r\n")
				.Append(indent + MailContentsNameHelper.GetContentsName(MailContentsNameHelper.ContentsDispNameType.ProductName, order))
				.Append((string)displayOrderItem[Constants.FIELD_ORDERITEM_PRODUCT_NAME])
				.Append("\r\n");

			// 付帯情報がある場合は商品名に続けて表示 ※この辺りの改修はCroozMollに影響
			orderItemsString = ProductOptionSettingHelper.GetProductOptionTextForMailTemplate(
				(string)displayOrderItem[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS],
				orderItemsString);
			orderItemsString
				.Append(indent + MailContentsNameHelper.GetContentsName(MailContentsNameHelper.ContentsDispNameType.ProductPrice, order))
				.Append(ConvertPriceForMailByConfig(displayOrderItem[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]))
				.Append(GetProductTaxText(new OrderOwnerModel(order.DataSource).DispLanguageLocaleId))
				.Append("\r\n")
				.Append(indent + MailContentsNameHelper.GetContentsName(MailContentsNameHelper.ContentsDispNameType.Quantity, order))
				.Append(StringUtility.ToNumeric(displayOrderItem[Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE]))
				.Append("\r\n");
		}

		/// <summary>
		/// Get order items loops for order mail templete
		/// </summary>
		/// <param name="order">Order model</param>
		/// <param name="orderItems">Order items</param>
		/// <param name="isGift">Is gift</param>
		/// <returns>Order items loops</returns>
		public Hashtable[] GetOrderItemsLoopsForOrderMailTemplete(OrderModel order, List<Hashtable> orderItems, bool isGift)
		{
			// デジタルコンテンツ用にシリアルキー取得
			var orderSerialKeys = DomainFacade.Instance.OrderService.GetOrderSerialKeyForOrderMail(order.OrderId)
				.Select(os => os.DataSource)
				.ToArray();

			// 注文商品情報文字列の組み立て
			var displayOrderItems = orderItems
				.Where(
					item => (string)item[Constants.FIELD_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE] == Constants.FLG_ORDERITEM_ORDER_HISTORY_DISPLAY_TYPE_VALID)
				.ToArray();

			// セット商品
			var orderSetProductGroups = displayOrderItems
				.Where(
					item => (string.IsNullOrEmpty(StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID])) == false))
				.GroupBy(item => StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]))
				.ToArray();
			var orderItemsLoopList = new List<Hashtable>();
			var orderItemNo = 1;
			var productService = DomainFacade.Instance.ProductService;
			foreach (var group in orderSetProductGroups)
			{
				var setProductIndex = 1;
				var setProductList = new List<Hashtable>();
				var itemPriceTotal = 0m;
				foreach (var item in group)
				{
					itemPriceTotal += (decimal)item[Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE];
					var productInfo = productService.GetProductVariation(
						StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_SHOP_ID]),
						StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_ID]),
						StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_VARIATION_ID]),
						StringUtility.ToEmpty(item[Constants.FIELD_ORDER_MEMBER_RANK_ID]));
					var productUrl = ProductCommon.CreateProductDetailUrlForSendMail(
						productInfo.ShopId,
						productInfo.ProductId,
						productInfo.VariationId,
						productInfo.Name);
					setProductList.Add(new Hashtable
					{
						{ Constants.FIELD_PRODUCT_SHOP_ID, item[Constants.FIELD_ORDERITEM_SHOP_ID] },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_INDEX, setProductIndex },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_ID, item[Constants.FIELD_ORDERITEM_VARIATION_ID] },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_NAME, item[Constants.FIELD_ORDERITEM_PRODUCT_NAME] },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_IMAGE, productInfo.ImageHead },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_VARIATION_IMAGE, productInfo.VariationImageHead },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_OUTLINE, productInfo.Outline },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_DETAIL1, productInfo.DescDetail1 },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_DETAIL2, productInfo.DescDetail2 },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_DETAIL3, productInfo.DescDetail3 },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_DETAIL4, productInfo.DescDetail4 },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_LINK, productUrl },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_PRICE, ConvertPriceForMail(item[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]) },
						{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_QUANTITY, StringUtility.ToNumeric(item[Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE]) },
						{ Constants.MAILTAG_IS_ORDER_SET_PRODUCT_VARIATION, productInfo.HasProductVariation },
					});

					setProductIndex++;
				}
				orderItemsLoopList.Add(new Hashtable
				{
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_INDEX, orderItemNo },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_ITEMS, setProductList },
					{
						Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_TOTAL_QUANTITY,
						StringUtility.ToNumeric((group.Count() > 1) ? group.First()[Constants.FIELD_ORDERITEM_PRODUCT_SET_COUNT] : 0) },
					{ Constants.MAILTAG_ORDER_ITEM_SET_PRODUCT_TOTAL_PRICE, ConvertPriceForMail(itemPriceTotal) },
					{ Constants.MAILTAG_ORDER_ITEM_TYPE, Constants.MAILTAG_ORDER_SET_PRODUCTS_LOOP },
				});

				orderItemNo++;
			}

			// セットプロモーション商品
			if (isGift == false)
			{
				var orderSetPromotionProductGroups = displayOrderItems
					.Where(
						item => (string.IsNullOrEmpty(StringUtility.ToEmpty(item[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID])) == false))
					.GroupBy(item => StringUtility.ToEmpty(item[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID]))
					.ToArray();
				foreach (var group in orderSetPromotionProductGroups)
				{
					var setPromotionProductIndex = 1;
					var setPromotionProductList = new List<Hashtable>();
					foreach (var item in group)
					{
						var productOptionSettingSelectValues = StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS]);
						var productInfo = productService.GetProductVariation(
							StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_SHOP_ID]),
							StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_ID]),
							StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_VARIATION_ID]),
							StringUtility.ToEmpty(item[Constants.FIELD_ORDER_MEMBER_RANK_ID]));
						var productUrl = ProductCommon.CreateProductDetailUrlForSendMail(
							productInfo.ShopId,
							productInfo.ProductId,
							productInfo.VariationId,
							productInfo.Name);
						var orderItem = new Hashtable
						{
							{ Constants.FIELD_PRODUCT_SHOP_ID, item[Constants.FIELD_ORDERITEM_SHOP_ID] },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_INDEX, setPromotionProductIndex },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_ID, item[Constants.FIELD_ORDERITEM_VARIATION_ID] },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_NAME, item[Constants.FIELD_ORDERITEM_PRODUCT_NAME] },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_IMAGE, productInfo.ImageHead },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_VARIATION_IMAGE, productInfo.VariationImageHead },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_OUTLINE, productInfo.Outline },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_DETAIL1, productInfo.DescDetail1 },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_DETAIL2, productInfo.DescDetail2 },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_DETAIL3, productInfo.DescDetail3 },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_DETAIL4, productInfo.DescDetail4 },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_LINK, productUrl },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_PRICE, ConvertPriceForMail(item[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]) },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_QUANTITY, StringUtility.ToNumeric(item[Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE]) },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_OPTION, ProductOptionSettingHelper.GetDisplayProductOptionTexts(productOptionSettingSelectValues) },
							{ Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_DISP_NAME, item[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME] },
							{ Constants.MAILTAG_IS_ORDER_SET_PROMOTION_PRODUCT_VARIATION, productInfo.HasProductVariation },
						};

						// デジタルコンテンツ対応
						var serialKeys = orderSerialKeys
							.Where(
								serialKey => ((StringUtility.ToEmpty(serialKey[Constants.FIELD_SERIALKEY_PRODUCT_ID]) == StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_ID]))
									&& (StringUtility.ToEmpty(serialKey[Constants.FIELD_SERIALKEY_VARIATION_ID]) == StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_VARIATION_ID]))))
							.Select(serialKey => new Hashtable
							{
								{
									Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_SERIAL_KEY,
									SerialKeyUtility.DecryptSerialKey((string)serialKey[Constants.FIELD_SERIALKEY_SERIAL_KEY])
								},
							})
							.ToArray();
						if (serialKeys.Length > 0) orderItem.Add(Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCT_SERIAL_KEYS_LOOP, serialKeys);

						setPromotionProductList.Add(orderItem);
						setPromotionProductIndex++;
					}

					orderItemsLoopList.Add(new Hashtable
					{
						{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_INDEX, orderItemNo },
						{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_ITEMS, setPromotionProductList },
						{
							Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_DISP_NAME,
							setPromotionProductList.Any()
								? setPromotionProductList[0][Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_DISP_NAME]
								: string.Empty },
						{ Constants.MAILTAG_ORDER_ITEM_TYPE, Constants.MAILTAG_ORDER_SET_PROMOTION_PRODUCTS_LOOP },
					});

					orderItemNo++;
				}
			}

			// 通常商品
			var orderNormalProducts = displayOrderItems
				.Where(item => (string.IsNullOrEmpty(StringUtility.ToEmpty(item[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID]))
					&& string.IsNullOrEmpty(StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]))))
				.ToArray();
			foreach (var item in orderNormalProducts)
			{
				var productOptionSettingSelectValues = ProductOptionSettingHelper.GetDisplayProductOptionTexts(
					StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS]));
				var productInfo = productService.GetProductVariation(
					StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_SHOP_ID]),
					StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_ID]),
					StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_VARIATION_ID]),
					StringUtility.ToEmpty(item[Constants.FIELD_ORDER_MEMBER_RANK_ID])) ?? new ProductModel();

				var productUrl = ProductCommon.CreateProductDetailUrlForSendMail(
					StringUtility.ToEmpty(productInfo.ShopId),
					StringUtility.ToEmpty(productInfo.ProductId),
					StringUtility.ToEmpty(productInfo.VariationId),
					StringUtility.ToEmpty(productInfo.Name));
				var orderItem = new Hashtable
				{
					{ Constants.FIELD_PRODUCT_SHOP_ID, item[Constants.FIELD_ORDERITEM_SHOP_ID] },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_INDEX, orderItemNo },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_ID, item[Constants.FIELD_ORDERITEM_VARIATION_ID] },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_NAME, item[Constants.FIELD_ORDERITEM_PRODUCT_NAME] },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_IMAGE, StringUtility.ToEmpty(productInfo.ImageHead) },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_VARIATION_IMAGE, StringUtility.ToEmpty(productInfo.VariationImageHead) },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_OUTLINE, StringUtility.ToEmpty(productInfo.Outline) },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_DETAIL1, StringUtility.ToEmpty(productInfo.DescDetail1) },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_DETAIL2, StringUtility.ToEmpty(productInfo.DescDetail2) },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_DETAIL3, StringUtility.ToEmpty(productInfo.DescDetail3) },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_DETAIL4, StringUtility.ToEmpty(productInfo.DescDetail4) },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_LINK, productUrl },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_PRICE, ConvertPriceForMail(item[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]) },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_QUANTITY, StringUtility.ToNumeric(item[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]) },
					{ Constants.MAILTAG_ORDER_ITEM_PRODUCT_OPTION, productOptionSettingSelectValues },
					{ Constants.MAILTAG_IS_ORDER_PRODUCT_VARIATION, productInfo.HasProductVariation },
					{ Constants.MAILTAG_ORDER_ITEM_TYPE, Constants.MAILTAG_ORDER_NORMAL_PRODUCT },
				};

				// デジタルコンテンツ対応
				var serialKeys = orderSerialKeys
					.Where(
						serialKey => ((StringUtility.ToEmpty(serialKey[Constants.FIELD_SERIALKEY_PRODUCT_ID]) == StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_ID]))
							&& (StringUtility.ToEmpty(serialKey[Constants.FIELD_SERIALKEY_VARIATION_ID]) == StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_VARIATION_ID]))))
					.Select(serialKey => new Hashtable
					{
						{
							Constants.MAILTAG_ORDER_ITEM_SETPROMOTION_PRODUCT_SERIAL_KEY,
							SerialKeyUtility.DecryptSerialKey((string)serialKey[Constants.FIELD_SERIALKEY_SERIAL_KEY])
						},
					})
					.ToArray();
				if (serialKeys.Length > 0) orderItem.Add(Constants.MAILTAG_ORDER_PRODUCT_SERIAL_KEYS_LOOP, serialKeys);

				orderItemsLoopList.Add(orderItem);
				orderItemNo++;
			}

			return orderItemsLoopList.ToArray();
		}

		/// <summary>
		/// 注文メールテンプレート用に、注文商品情報の文字列を取得する。(ネクストエンジン用)
		/// </summary>
		/// <param name="orderItems">注文商品</param>
		/// <param name="isGift">ギフト注文かどうか</param>
		/// <returns>注文商品情報文字列</returns>
		private string GetOrderItemsStringForNexteEgineOrderMailTemplete(
			List<Hashtable> orderItems,
			bool isGift)
		{
			//------------------------------------------------------
			// 注文商品情報文字列の組み立て
			//------------------------------------------------------
			var displayOrderItems = orderItems.ToList();
			var orderItemTexts = new StringBuilder();
			var index = 0;
			while (index < displayOrderItems.Count)
			{
				if (((string)displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]).Length != 0)
				{
					// セット商品
					var productSetId = StringUtility.ToEmpty(
						displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]);
					var productSetNo = StringUtility.ToEmpty(
						displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_SET_NO]);

					while ((index < displayOrderItems.Count)
						&& (productSetId == StringUtility.ToEmpty(displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_SET_ID]))
						&& (productSetNo == StringUtility.ToEmpty(displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_SET_NO])))
					{
						var orderItemText = CreateNextEngineOrderMailTempleteFormat(
							(string)displayOrderItems[index][Constants.FIELD_ORDERITEM_VARIATION_ID],
							(string)displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_NAME],
							(string)displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS],
							(decimal)displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_PRICE],
							(int)displayOrderItems[index][Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE],
							(decimal)displayOrderItems[index][Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE]);
						orderItemTexts.Append(orderItemText);

						index++;
					}
				}
				else if ((isGift == false)
					&& (displayOrderItems[index][Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] != DBNull.Value))
				{
					// セットプロモーション商品
					var orderSetPromotionNo =
						(int)displayOrderItems[index][Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO];

					int currentOrderSetPromotionNo;
					while ((index < displayOrderItems.Count)
						&& int.TryParse(displayOrderItems[index][Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO].ToString(), out currentOrderSetPromotionNo)
						&& (currentOrderSetPromotionNo == orderSetPromotionNo))
					{
						var orderItemText = CreateNextEngineOrderMailTempleteFormat(
							(string)displayOrderItems[index][Constants.FIELD_ORDERITEM_VARIATION_ID],
							(string)displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_NAME],
							(string)displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS],
							(decimal)displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_PRICE],
							(int)displayOrderItems[index][Constants.FIELD_ORDERITEM_ITEM_QUANTITY_SINGLE],
							(decimal)displayOrderItems[index][Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE]);
						orderItemTexts.Append(orderItemText);

						index++;
					}
				}
				else
				{
					// 通常商品
					var orderItemText = CreateNextEngineOrderMailTempleteFormat(
						(string)displayOrderItems[index][Constants.FIELD_ORDERITEM_VARIATION_ID],
						(string)displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_NAME],
						(string)displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_OPTION_TEXTS],
						(decimal)displayOrderItems[index][Constants.FIELD_ORDERITEM_PRODUCT_PRICE],
						(int)displayOrderItems[index][Constants.FIELD_ORDERITEM_ITEM_QUANTITY],
						(decimal)displayOrderItems[index][Constants.FIELD_ORDERITEM_ITEM_PRICE]);
					orderItemTexts.Append(orderItemText);

					index++;
				}
			}

			if (orderItemTexts.Length > 0)
			{
				orderItemTexts.Append("------------------------------------------------------------");
			}

			// 注文商品情報を返却
			return orderItemTexts.ToString().Remove(orderItemTexts.ToString().LastIndexOf("\r\n", StringComparison.Ordinal));
		}

		/// <summary>
		/// 配送希望日時セット（注文メールテンプレート用）
		/// </summary>
		/// <param name="mailDatas">メールデータ</param>
		/// <param name="languageLocaleId">注文者の言語ロケールID</param>
		private void SetOrderShippingDateTimeForOrderMailTemplete(Hashtable mailDatas, string languageLocaleId = "")
		{
			// 日付形式で未変換の配送希望日
			var shippingDateOriginal = StringUtility.ToEmpty(mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE]);
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE] = DateTimeUtility.ToString(
				shippingDateOriginal,
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter,
				languageLocaleId,
				CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@"));
			// 配送希望日に関する管理者向けのタグ用を追加
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE + STRING_FOR_OPERATOR] =
				DateTimeUtility.ToStringForManager(
					shippingDateOriginal,
					DateTimeUtility.FormatType.LongDateWeekOfDay1Letter,
					CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@"));

			// Set scheduled shipping date into mail template
			mailDatas[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE]
				= (string.IsNullOrEmpty(StringUtility.ToEmpty(mailDatas[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE])))
					? CommonPage.ReplaceTag("@@DispText.shipping_date_list.none@@")
					: DateTimeUtility.ToString(StringUtility.ToEmpty(mailDatas[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE]),
						DateTimeUtility.FormatType.LongDateWeekOfDay1Letter,
						languageLocaleId);

			var shippingTimeId = StringUtility.ToEmpty(mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME]);
			mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME] = "";
			for (var iLoop = 1; iLoop <= 10; iLoop++)
			{
				if (StringUtility.ToEmpty(mailDatas["shipping_time_id" + iLoop.ToString()]) == shippingTimeId)
				{
					mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME] = mailDatas["shipping_time_message" + iLoop];
					break;
				}
			}
			if (StringUtility.ToEmpty(mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME]) == "")
			{
				mailDatas[Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME] = CommonPage.ReplaceTag("@@DispText.shipping_time_list.none@@");
			}
		}

		/// <summary>
		/// 別途見積りのためのメッセージ取得
		/// </summary>
		/// <param name="orders">注文情報</param>
		/// <returns>PC,Mobileサイトに合わせたメッセージ</returns>
		private string GetMessageForSeparateEstimate(Hashtable[] orders)
		{
			var message = (Constants.SHIPPINGPRICE_SEPARATE_ESTIMATE_ENABLED
				&& ((string)orders[0][Constants.FIELD_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG] == Constants.FLG_ORDER_SHIPPING_PRICE_SEPARATE_ESTIMATES_FLG_VALID))
					? this.IsPc
						? OrderCommon.GetShopShipping((string)orders[0][Constants.FIELD_ORDER_SHOP_ID], (string)orders[0][Constants.FIELD_ORDER_SHIPPING_ID]).ShippingPriceSeparateEstimatesMessage
						: OrderCommon.GetShopShipping((string)orders[0][Constants.FIELD_ORDER_SHOP_ID], (string)orders[0][Constants.FIELD_ORDER_SHIPPING_ID]).ShippingPriceSeparateEstimatesMessageMobile
					: string.Empty;
			return message;
		}
	}
}
