/*
=========================================================================================================
  Module      : 注文履歴詳細画面処理(OrderHistoryPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Order;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Atobaraicom;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Helper;
using w2.App.Common.Order.Reauth;
using w2.App.Common.SendMail;
using w2.Common.Extensions;
using w2.Common.Logger;
using w2.Domain.FixedPurchase;
using w2.Domain.Holiday.Helper;
using w2.Domain.InvoiceDskDeferred;
using w2.Domain.NameTranslationSetting;
using w2.Domain.NameTranslationSetting.Helper;
using w2.Domain.Order;
using w2.Domain.Payment;
using w2.Domain.ShopShipping;
using w2.Domain.TwOrderInvoice;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;

/// <summary>
/// 注文履歴詳細画面処理
/// </summary>
public class OrderHistoryPage : HistoryPage
{
	/// <summary>表示済み定額頒布会コースID</summary>
	private readonly List<string> _displayedFixedAmountCourseId = new List<string>();

	/// <summary>
	/// 表示データの取得
	/// </summary>
	protected void SetOrderHistory(OrderModel order = null)
	{
		// パラメタ取得
		var orderId = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ID]);

		// 注文情報セット (取得できない場合はエラー)
		var orderItems = new OrderService().GetOrderHistoryDetailInDataView(orderId, this.LoginUserId, this.MemberRankId);
		if (orderItems.Count == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDERHISTORY_UNDISP);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		this.OldItems = orderItems.Cast<DataRowView>()
			.Select(orderItem => new OrderItemModel(orderItem))
			.ToArray();
		this.OrderModel = new OrderService().Get((string)orderItems[0][Constants.FIELD_ORDER_ORDER_ID]);
		this.ShopShippingModel = new ShopShippingModel(orderItems[0]);

		if (Constants.PAYMENT_GMO_POST_ENABLED)
		{
			// GMO payment: get credit status
			var isGetGmoStatus = OrderCommon.IsGetGmoCreditStatus(this.OrderModel);
			if (isGetGmoStatus)
			{
				var isChangeStatus = OrderCommon.CheckGmoCredit(this.OrderModel);
				if (isChangeStatus)
				{
					//get data after change status
					this.OrderModel = new OrderService().Get((string)orderItems[0][Constants.FIELD_ORDER_ORDER_ID]);
				}
			}
		}

		//支払い方法のセット
		this.PaymentModel = DataCacheControllerFacade.GetPaymentCacheController().Get(this.OrderModel.OrderPaymentKbn);

		//現在利用しているユーザクレジットカード情報のセット
		if ((this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (this.OrderModel.CreditBranchNo != null))
		{
			this.UserCreditCardInfo = UserCreditCard.Get(this.OrderModel.UserId, (int)this.OrderModel.CreditBranchNo);
		}

		//ユーザの持っている利用可能なクレジットカード群
		this.UserCreditCardsUsable = UserCreditCard.GetUsable(this.LoginUserId);

		// 利用可能な支払い方法選択肢をセット
		this.ValidPayments = LoadPaymentValidList();

		// 注文セットプロモーション情報セット
		this.OrderSetPromotions = new List<Hashtable>();
		foreach (Hashtable orderItem in orderItems.ToHashtableList()
			.OrderBy(item => (item[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]).ToString()).ToArray())
		{
			if ((StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]) != "")
			    && ((int)orderItem[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_ITEM_NO] == 1))
			{
				this.OrderSetPromotions.Add(orderItem);
			}
		}

		//定期購入かどうか
		this.IsFixedPurchase = ((string.IsNullOrEmpty(this.OrderModel.FixedPurchaseId) == false)
			&& Constants.FIXEDPURCHASE_OPTION_ENABLED);

		//定期購入情報セット
		this.FixedPurchaseModel = new FixedPurchaseModel();
		if (this.IsFixedPurchase)
		{
			this.FixedPurchaseModel = new FixedPurchaseService().Get(this.OrderModel.FixedPurchaseId);
		}

		// Is Update Shipping Fixed Purchase
		if ((this.FixedPurchaseModel != null)
			&& (this.FixedPurchaseModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
			&& (this.OrderModel.Shippings[0].ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF))
		{
			this.IsUpdateShippingFixedPurchase = false;
		}
		else
		{
			this.IsUpdateShippingFixedPurchase = true;
		}

		// 注文配送先情報セット
		CreateShippingItem(orderItems);

		// 更新可否をセット
		SetModify();

		// 翻訳情報設定
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			var languageCode = RegionManager.GetInstance().Region.LanguageCode;
			var languageLocaleId = RegionManager.GetInstance().Region.LanguageLocaleId;

			this.PaymentModel.PaymentName = NameTranslationCommon.GetTranslationName(
				this.PaymentModel.PaymentId,
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_PAYMENT,
				Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_PAYMENT_PAYMENT_NAME,
				this.PaymentModel.PaymentName);
			this.OrderSetPromotions = SetOrderSetPromotionsTranslationData(languageCode, languageLocaleId);
			this.OrderShippingItems = SetOrderShippingItemTranslationData(languageCode, languageLocaleId);
		}

		// For case taiwan invoce enable
		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			this.TwOrderInvoiceModel = new TwOrderInvoiceService().GetOrderInvoice(
				this.OrderModel.OrderId,
				this.OrderModel.Shippings[0].OrderShippingNo);
		}
	}

	/// <summary>
	/// 更新の可否情報をセット
	/// </summary>
	private void SetModify()
	{
		//全体の更新可否
		string enableMessage;
		this.IsModifyOrder = CanModifyOrder(out enableMessage);

		// 入金ステータスによる金額変動の可能性がある変更処理の可否
		var useModifyPaymentStatus = CheckUseModifyPaymentStatus();

		var isPaygentPayment = CheckIsPaygentPayment();

		var canCancelPaidyPayment = CheckCanCancelPaidyPayment();

		// 注文のキャンセル可否
		this.IsModifyCancel = ((this.OrderModel.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
			&& (DateTime.Now < this.OrderCancellationTime)
			&& useModifyPaymentStatus
			&& canCancelPaidyPayment
			&& (isPaygentPayment == false));

		if (canCancelPaidyPayment == false)
		{
			this.IsModifyCancelMessage = (this.OrderModel.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
				? WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_MODIFY_NG_ORDER_STATUS_ALERT,
					ValueText.GetValueText(
						Constants.TABLE_ORDER,
						Constants.FIELD_ORDER_ORDER_STATUS,
						this.OrderModel.OrderStatus))
				: WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_MODIFY_NG_NO_EXIST_PAYMENT_ALERT,
					this.PaymentModel.PaymentName);
		}
		else if (isPaygentPayment)
		{
			this.IsModifyCancelMessage = (this.OrderModel.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
				? WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_ORDER_CANCEL_NG_STATUS_ALERT,
					ValueText.GetValueText(
						Constants.TABLE_ORDER,
						Constants.FIELD_ORDER_ORDER_STATUS,
						this.OrderModel.OrderStatus))
				: WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_ORDER_CANCEL_NG_PAYMENT_ALERT,
					this.PaymentModel.PaymentName);
		}
		else if (this.OrderModel.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
		{
			this.IsModifyCancelMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MODIFY_NG_ORDER_STATUS_ALERT)
				.Replace(
					"@@ 1 @@",
					ValueText.GetValueText(
						Constants.TABLE_ORDER,
						Constants.FIELD_ORDER_ORDER_STATUS,
						this.OrderModel.OrderStatus));
		}
		else
		{
			this.OrderCancelTimeMessage = WebMessages.GetMessages(WebMessages.ERRMSG_ORDER_CANCEL_TIME)
				.Replace("@@ 1 @@", this.OrderCancellationTime.ToString());
		}

		// 支払い方法の変更可否
		this.CanModifyPayment = this.IsModifyOrder
			&& (this.ValidPayments[0].Length > 0)
			&& useModifyPaymentStatus
			&& canCancelPaidyPayment
			&& (isPaygentPayment == false);

		if (this.CanModifyPayment == false)
		{
			this.ExplanationOrderPaymentKbn = WebMessages.GetMessages(
				WebMessages.ERRMSG_FRONT_MODIFY_NG_NO_EXIST_PAYMENT_ALERT)
					.Replace("@@ 1 @@", this.PaymentModel.PaymentName);

			if (useModifyPaymentStatus == false)
			{
				this.ExplanationOrderPaymentKbn = WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_MODIFY_NG_PAYMENT_STATUS_COMPLETE_ALERT);

				// 入金済みで変更できない場合注文キャンセル出来なくする
				this.OrderCancelTimeMessage =
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MODIFY_NG_PAYMENT_STATUS_COMPLETE_ALERT);
			}
		}

		// 支払い方法による金額変動の可能性がある変更処理の可否
		this.CanUseModifyPayment = CheckUseModifyPayment();

		// 利用ポイント更新可否
		this.IsModifyUsePoint = this.IsModifyOrder
			&& this.CanUseModifyPayment
			&& useModifyPaymentStatus
			&& (PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderModel.OrderPaymentKbn) == false)
			&& (isPaygentPayment == false);

		if (this.IsModifyUsePoint == false)
		{
			this.ExplanationPointUse = WebMessages.GetMessages(
				WebMessages.ERRMSG_FRONT_MODIFY_NG_PAYMENT_ALERT)
					.Replace("@@ 1 @@", this.PaymentModel.PaymentName);

			if (useModifyPaymentStatus == false)
			{
				this.ExplanationPointUse = WebMessages.GetMessages(
						WebMessages.ERRMSG_FRONT_MODIFY_NG_PAYMENT_STATUS_COMPLETE_ALERT);
			}
		}

		// お届け先更新可否
		this.IsModifyShipping = this.IsModifyOrder
			&& this.CanUseModifyPayment
			&& useModifyPaymentStatus
			&& (PaygentUtility.CheckIsPaidyPaygentPayment(this.OrderModel.OrderPaymentKbn) == false)
			&& (isPaygentPayment == false);

		if (this.IsModifyShipping == false)
		{
			this.ExplanationShipping = WebMessages.GetMessages(
				WebMessages.ERRMSG_FRONT_MODIFY_NG_PAYMENT_ALERT)
					.Replace("@@ 1 @@", this.PaymentModel.PaymentName);

			if (useModifyPaymentStatus == false)
			{
				this.ExplanationShipping = WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_MODIFY_NG_PAYMENT_STATUS_COMPLETE_ALERT);
			}
		}

		// 更新可能でメール便でなければ更新を許可
		this.IsModifyShippingDates = new List<bool>();
		this.IsModifyShippingTimes = new List<bool>();
		this.ExplanationShippingDates = new List<string>();
		this.ExplanationShippingTimes = new List<string>();
		foreach (var orderSshippingModel in this.OrderModel.Shippings)
		{
			var shippingMethodAlert = WebMessages.GetMessages(
				WebMessages.ERRMSG_FRONT_MODIFY_NG_SHIPPING_METHOD_ALERT)
					.Replace("@@ 1 @@", ValueText.GetValueText(
						Constants.TABLE_ORDERSHIPPING,
						Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD,
						orderSshippingModel.ShippingMethod));

			var isModifyShippingDate = (this.IsModifyOrder && (orderSshippingModel.ShippingMethod != Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL));
			this.IsModifyShippingDates.Add(isModifyShippingDate);
			this.ExplanationShippingDates.Add(((isModifyShippingDate == false) ? shippingMethodAlert : ""));

			var isModifyShippingTime = (this.IsModifyOrder && (orderSshippingModel.ShippingMethod != Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL));
			this.IsModifyShippingTimes.Add(isModifyShippingTime);
			this.ExplanationShippingTimes.Add(((isModifyShippingTime == false) ? shippingMethodAlert : ""));
		}

		if (this.IsModifyOrder == false)
		{
			this.ExplanationOrderPaymentKbn = enableMessage;
			this.ExplanationPointUse = enableMessage;
			this.ExplanationShipping = enableMessage;
			this.ExplanationShippingDates = this.ExplanationShippingDates.Select(s => enableMessage).ToList();
			this.ExplanationShippingTimes = this.ExplanationShippingTimes.Select(s => enableMessage).ToList();
			this.ExplanationOrderExtend = enableMessage;
		}

		if (CheckOrderItemDataRegistration() == false)
		{
			this.ExplanationOrderPaymentKbn = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MODIFY_NG_DAY_OVER_ALERT);
			this.ExplanationPointUse = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MODIFY_NG_DAY_OVER_ALERT);
			this.ExplanationShipping = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MODIFY_NG_DAY_OVER_ALERT);
			this.ExplanationShippingDates = this.ExplanationShippingDates.Select(s => s = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MODIFY_NG_DAY_OVER_ALERT)).ToList();
			this.ExplanationShippingTimes = this.ExplanationShippingTimes.Select(s => s = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MODIFY_NG_DAY_OVER_ALERT)).ToList();
			this.IsModifyOrder = false;
			this.IsModifyUsePoint = false;
			this.IsModifyShipping = false;
			this.IsModifyShippingDates = this.IsModifyShippingDates.Select(s => false).ToList();
			this.IsModifyShippingTimes = this.IsModifyShippingTimes.Select(s => false).ToList();
		}
	}

	/// <summary>
	/// 注文情報が変更可能か
	/// </summary>
	/// <param name="enableMessage">out 変更不可メッセージ</param>
	/// <returns>変更可能か 可能:true 不可能:false</returns>
	private bool CanModifyOrder(out string enableMessage)
	{
		bool dateChackResult;
		if (Constants.MYPAGE_ORDER_MODIFY_BY_DATE == null)
		{
			dateChackResult = true;
		}
		else
		{
			//最も近い日付を算出
			var shippingNearDate = this.OrderModel.Shippings.Where(s => s.ShippingDate != null).OrderBy(s => s.ShippingDate.Value).FirstOrDefault();
			if (shippingNearDate != null)
			{
				var requiredShippingDays = this.ShopShippingModel.ShippingDateSetBegin.Value - 1;
				var shippingDate = shippingNearDate.ShippingDate.Value;
				// 変更できなくなる日 (配送希望日 - 配送にかかる日数(配送希望日を含む) - 指定営業日数)を算出
				var unchangeableDateBegin = HolidayUtil.GetDateOfBusinessDay(
					shippingDate.AddDays(-1 * requiredShippingDays),
					Constants.MYPAGE_ORDER_MODIFY_BY_DATE.Value,
					false);
				// 「本日日付」 が「変更できなくなる日」より前のときtrue
				dateChackResult = (DateTime.Now < unchangeableDateBegin);
			}
			else
			{
				dateChackResult = false;
			}
		}
		enableMessage = "";
		if (this.OrderModel.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
		{
			enableMessage = WebMessages.GetMessages(
				WebMessages.ERRMSG_FRONT_MODIFY_NG_ORDER_STATUS_ALERT)
					.Replace("@@ 1 @@", ValueText.GetValueText(
						Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_STATUS, this.OrderModel.OrderStatus));
		}

		if (dateChackResult == false)
		{
			enableMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MODIFY_NG_DAY_OVER_ALERT);
		}

		if (this.OrderModel.MallId != Constants.FLG_ORDER_MALL_ID_OWN_SITE)
		{
			enableMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MODIFY_NG_MALL_ALERT);
		}

		var result = (this.OrderModel.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDERED)
			&& dateChackResult
			&& (this.OrderModel.MallId == Constants.FLG_ORDER_MALL_ID_OWN_SITE);

		if (result
			&& (this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY))
		{
			result = (this.OrderModel.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP);
		}

		if (result
			&& (this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA))
		{
			result = (this.OrderModel.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP);
		}

		return result;
	}

	/// <summary>
	/// 再計算に必要な登録時のデータがorderItemに存在しているか
	/// 
	/// ※ マイページからの注文変更機能はアップデートにより途中から導入された機能となる。
	/// 更新時に登録時のpoint1, point1_kbn, limited_payment_ids, plural_shipping_price_free_flg, shipping_size_kbnがデータとして
	/// 残っていない場合、更新不可とする。
	/// その際の判断基準として最終請求金額フラグを利用する
	/// V5.12バージョンアップ以前 → null
	/// V5.12バージョンアップ以後 → 0 or 1
	/// </summary>
	/// <returns>更新可能:true 更新不可:false</returns>
	private bool CheckOrderItemDataRegistration()
	{
		var result = (string.IsNullOrEmpty(this.OrderModel.LastAuthFlg) == false);
		return result;
	}

	/// <summary>
	/// 利用可能な支払い方法リストの取得
	/// </summary>
	/// <returns>有効決済種別</returns>
	private List<PaymentModel[]> LoadPaymentValidList()
	{
		// 決済種別情報セット（配送種別で決済種別を限定しているのであれば、有効の決済種別のみ取得）
		var validPaymentList = OrderCommon.GetValidPaymentListForOrder(
			this.OrderModel,
			this.ShopShippingModel.PaymentSelectionFlg,
			this.ShopShippingModel.PermittedPaymentIds,
			this.LoginUserId);

		// 商品ごとの決済種別制限を取得
		var paymentsUnlimitIds = new List<string>();
		var paymentsUnlimitNames = new List<string>();
		this.OrderModel.Shippings.SelectMany(osm => osm.Items.Select(
			item =>
			{
				paymentsUnlimitIds.AddRange(item.LimitedPaymentIds.Split(',').ToList());
				paymentsUnlimitNames.Add(item.ProductName);
				return item;
			}).Where(item => (item != null)).ToArray()).ToArray();

		if (Constants.ECPAY_PAYMENT_OPTION_ENABLED
			&& this.OrderModel.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)
		{
			validPaymentList = validPaymentList
				.Where(validPayment => (validPayment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY))
				.ToArray();
		}

		if (Constants.PAYMENT_GMOATOKARA_ENABLED
			&& (this.OrderModel.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA))
		{
			validPaymentList = validPaymentList
				.Where(validPayment => (validPayment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA))
				.ToArray();
		}

		var paymentIdsCanChange = OrderCommon.GetPaymentIdsCanChange(
			this.OrderModel.OrderPaymentKbn,
			Constants.SETTING_CAN_CHANGE_ORDER_PAYMENT_IDS,
			Constants.ORDER_PAYMENT_IDS_PRIORITY_OPTION_ENABLED);

		if (Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED
			&& (this.OrderModel.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
		{
			validPaymentList = validPaymentList
				.Where(validPayment => (validPayment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY))
				.ToArray();
		}

		// 決済種別の制限
		var limitFlg = false;
		var validPayments = new List<PaymentModel[]>
		{
			validPaymentList.Where(
				payment =>
				{
					switch (payment.PaymentId)
					{
						case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
						case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
						case Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF:
						case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL:
						case Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF:
						case Constants.FLG_PAYMENT_PAYMENT_ID_PAYASYOUGO:
						case Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE:
							if (paymentsUnlimitIds.Any(p => (p == payment.PaymentId)))
							{
								limitFlg = true;
								return false;
							}

							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT:
							if ((Constants.AMAZON_PAYMENT_OPTION_ENABLED == false)
								|| paymentsUnlimitIds
									.Any(p => (p == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)))
							{
								limitFlg = true;
								return false;
							}

							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2:
							if ((Constants.AMAZON_PAYMENT_OPTION_ENABLED == false)
								|| paymentsUnlimitIds
									.Any(p => (p == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)))
							{
								limitFlg = true;
								return false;
							}

							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY:
							if (paymentsUnlimitIds.Select(p => p)
								.Any(p => (p == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)))
							{
								limitFlg = true;
								return false;
							}

							//注文者の住所か配送先が台湾以外の場合、後付款(TriLink後払い)選択肢を除外
							if (TriLinkAfterPayHelper.CheckUsedPaymentForTriLinkAfterPay(
								payment.PaymentId,
								this.OrderModel.Shippings.Select(s => s.ShippingCountryIsoCode)
									.Concat(new[] { this.OrderModel.Owner.OwnerAddrCountryIsoCode }).ToArray()))
							{
								return false;
							}

							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT:
							if (paymentsUnlimitIds
								.Any(p => (p == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)))
							{
								limitFlg = true;
								return false;
							}

							//メール便の場合代引き選択肢を除外
							if (this.OrderModel.Shippings.Any(
								s => (s.ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)))
							{
								return false;
							}

							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT:
							if (this.OrderModel.OrderPriceTotal != 0) return false;
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY:
							switch (Constants.PAYMENT_PAIDY_KBN)
							{
								case Constants.PaymentPaidyKbn.Direct:
									if ((Constants.PAYMENT_PAIDY_OPTION_ENABLED == false)
										&& paymentsUnlimitIds.Any(p => (p == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)))
									{
										limitFlg = true;
										return false;
									}
									break;

								case Constants.PaymentPaidyKbn.Paygent:
									if (Constants.GLOBAL_OPTION_ENABLE
										|| (Constants.PAYMENT_PAIDY_OPTION_ENABLED == false)
										|| paymentsUnlimitIds.Any(p => (p == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)))
									{
										limitFlg = true;
										return false;
									}
									break;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE:
							if (paymentsUnlimitIds.Select(item => item)
								.Any(item => item == payment.PaymentId))
							{
								limitFlg = true;
								return false;
							}
							var shippingList = (OrderShippingModel[])this.OrderModel.Shippings.Clone();
							var shippingReceivingStoreFlg = shippingList[0].ShippingReceivingStoreFlg;
							var shippingReceivingStoreType = shippingList[0].ShippingReceivingStoreType;
							if ((shippingList.Length > 0)
								&& ((shippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
									|| (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
										&& (ECPayUtility.GetIsCollection(shippingReceivingStoreType)
											== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_OFF))))
							{
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
							var hasUnlimitedPaymentAtone =
								paymentsUnlimitIds.Select(item => item)
									.Any(item => item == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
							if ((Constants.PAYMENT_ATONEOPTION_ENABLED == false)
								|| hasUnlimitedPaymentAtone)
							{
								limitFlg = true;
								return false;
							}
							break;
		
						case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
							var hasUnlimitedPaymentAftee =
								paymentsUnlimitIds.Select(item => item)
									.Any(item => item == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
							if ((Constants.PAYMENT_AFTEEOPTION_ENABLED == false)
								|| hasUnlimitedPaymentAftee)
							{
								limitFlg = true;
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY:
							if ((Constants.PAYMENT_LINEPAY_OPTION_ENABLED == false)
								|| paymentsUnlimitIds
									.Any(paymentId => (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)))
							{
								limitFlg = true;
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
							if ((Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED == false)
								|| paymentsUnlimitIds.Any(p => (p == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)))
							{
								limitFlg = true;
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
							if ((Constants.ECPAY_PAYMENT_OPTION_ENABLED == false)
								|| paymentsUnlimitIds.Any(paymentId => (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)))
							{
								limitFlg = true;
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
							if ((Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED == false)
								|| paymentsUnlimitIds.Any(paymentId => (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)))
							{
								limitFlg = true;
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
							if ((Constants.PAYMENT_PAYPAYOPTION_ENABLED == false)
								|| (paymentsUnlimitIds.Any(paymentId => (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY))))
							{
								limitFlg = true;
								return false;
							}

							if (this.PaymentModel.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY) return false;

							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU:
							if ((Constants.PAYMENT_BOKU_OPTION_ENABLED == false)
								|| paymentsUnlimitIds.Any(paymentId => (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)))
							{
								limitFlg = true;
								return false;
							}
							break;

						case Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA:
							if ((Constants.PAYMENT_GMOATOKARA_ENABLED == false)
								|| paymentsUnlimitIds.Any(paymentId => (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA)))
							{
								limitFlg = true;
								return false;
							}
							break;

						default:
							// 通常：マイページから変更可能な決済種別IDの追加設定に含まれていた場合利用可能にする
							return Constants.SETTING_PARTICULAR_USABLE_ORDER_PAYMENT_IDS_WHEN_CHANGE_ADDITIONAL_SETTING.Contains(payment.PaymentId);
					}

					return true;
				}).Where(payment => IsValidModifyFront(payment.PaymentId, paymentIdsCanChange))
					.OrderBy(item => item.DisplayOrder)
					.ToArray()
		};

		////Cart情報より支払い方法の制限時メッセージのセット
		this.DispLimitedPaymentMessages = new Hashtable();
		this.DispLimitedPaymentMessages[0] = "";
		if (limitFlg)
		{
			this.DispLimitedPaymentMessages[0] = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_PRODUCT_LIMITED_PAYMENT).Replace("@@ 1 @@", String.Join(",", paymentsUnlimitNames.Distinct()));
		}
		return validPayments;
	}

	/// <summary>
	/// 現在設定されている支払い方法がマイページにて対応している支払い方法か判定
	/// </summary>
	/// <returns>対応:true 未対応:false</returns>
	private bool CheckUseModifyPayment()
	{
		switch (this.PaymentModel.PaymentId)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
			case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
			case Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY:
			case Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT:
			case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT:
			case Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2:
			case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL:
			case Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY:
			case Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE:
			case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
			case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
			case Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY:
			case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
			case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
			case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
			case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY:
			case Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU:
			case Constants.FLG_PAYMENT_PAYMENT_ID_GMOATOKARA:
				return true;

			default:
				return false;
		}
	}

	/// <summary>
	/// 入金ステータスによる金額変動が伴う変更処理の可否
	/// </summary>
	/// <returns>可:true 否:false</returns>
	protected bool CheckUseModifyPaymentStatus()
	{
		var result = ((this.PaymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
				&& ((this.OrderModel.OrderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)
					|| (this.OrderModel.OrderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_SHORTAGE))) == false;

		return result;
	}

	/// <summary>
	/// 注文配送先・商品List作成
	/// </summary>
	/// <param name="shippingItems">注文情報DataTable</param>
	/// <remarks>
	/// SQLで取得したデータは
	///		注文情報－配送先１－商品１
	///		注文情報－配送先１－商品２
	///		注文情報－配送先２－商品１
	///		注文情報－配送先２－商品２
	///		・・・
	///		というような構造になっている。
	///	
	///		これを配送先－商品の階層化したRepeaterに当てはめるため、
	///			－[配送先List]		・・・１つめのリピータにDataBind
	///				＋[商品List]	・・・２つめのリピータにDataBind
	///		のデータ構造に加工を行う。
	///		具体的には以下のようになる。
	///
	///		・配送先List (=OrderShippingItems)
	///			－配送先・商品データ格納Hashtable (=currentOrderShipping)
	///				－[row]    配送先DataRowView
	///				－[childs] 商品データList(DataRowView)
	///					－取得DataRow（使い回す）
	/// </remarks>
	private void CreateShippingItem(DataView shippingItems)
	{
		this.OrderItemSerialKeys = new Dictionary<string, DataView>();
		this.OrderShippingItems = new List<Hashtable>();
		List<DataRowView> shippingItemsList = null;
		var orderShippingNo = -1;	// キーブレイク用配送先枝番格納変数
		foreach (DataRowView item in shippingItems)
		{
			// キーブレイク？（新しい配送先発見）
			if (orderShippingNo != (int)item[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO])
			{
				orderShippingNo = (int)item[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO];

				var currentOrderShipping = new Hashtable();
				shippingItemsList = new List<DataRowView>();
				currentOrderShipping.Add("childs", shippingItemsList);
				currentOrderShipping.Add("row", item);

				this.OrderShippingItems.Add(currentOrderShipping);
			}

			// 「購入商品毎 ｘ 数量」に応じたシリアルキーを取得
			this.OrderItemSerialKeys.Add(
				(string)item[Constants.FIELD_ORDER_ORDER_ID] + item[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO].ToString(),
				OrderCommon.GetSerialKeyList(item));

			shippingItemsList.Add(item);
		}
	}

	#region -SetOrderSetPromotionsTranslationData 注文セットプロモーションリスト翻訳情報設定
	/// <summary>
	/// 注文セットプロモーションリスト翻訳情報設定
	/// </summary>
	/// <param name="languageCode">言語コード</param>
	/// <param name="languageLocaleId">言語ロケールID</param>
	/// <returns>翻訳情報設定後注文セットプロモーションリスト</returns>
	private List<Hashtable> SetOrderSetPromotionsTranslationData(string languageCode, string languageLocaleId)
	{
		// 翻訳設定情報取得
		var setPromotionIds = this.OrderSetPromotions.Select(
			setPromotion => (string)setPromotion[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID]).ToList();
		if (setPromotionIds.Any() == false) return this.OrderSetPromotions;

		var translationSettings = GetOrderSetPromotionTranslationSettings(setPromotionIds, languageCode, languageLocaleId);
		if (translationSettings.Any() == false) return this.OrderSetPromotions;

		// 翻訳情報設定
		var orderSetPromotions = this.OrderSetPromotions.Select(
			orderSetPromotion => SetOrderSetPromotionsTranslationData(orderSetPromotion, translationSettings)).ToList();

		return orderSetPromotions;
	}
	/// <summary>
	/// 注文セットプロモーション翻訳情報設定
	/// </summary>
	/// <param name="orderSetPromotion">注文セットプロモーション</param>
	/// <param name="translationSettings">翻訳設定情報</param>
	/// <returns>注文セットプロモーション情報</returns>
	private Hashtable SetOrderSetPromotionsTranslationData(Hashtable orderSetPromotion, NameTranslationSettingModel[] translationSettings)
	{
		// 表示用セットプロモーション名
		var setPromotionId = (string)orderSetPromotion[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_ID];
		var translationSetting = translationSettings.FirstOrDefault(setting =>
			((setting.MasterId1 == setPromotionId)
				&& (setting.TranslationTargetColumn == Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_SETPROMOTION_SETPROMOTION_DISP_NAME)));

		if (translationSetting != null)
		{
			orderSetPromotion[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME] = translationSetting.AfterTranslationalName;
		}
		return orderSetPromotion;
	}
	#endregion

	#region -GetOrderSetPromotionTranslationSettings 注文セットプロモーション翻訳設定情報取得
	/// <summary>
	/// 注文セットプロモーション翻訳設定情報取得
	/// </summary>
	/// <param name="setPromotionIds">セットプロモーションIDリスト</param>
	/// <param name="languageCode">言語コード</param>
	/// <param name="languageLocaleId">言語ロケールID</param>
	/// <returns>注文セットプロモーション翻訳設定情報</returns>
	private NameTranslationSettingModel[] GetOrderSetPromotionTranslationSettings(List<string> setPromotionIds, string languageCode, string languageLocaleId)
	{
		var searchCondition = new NameTranslationSettingSearchCondition
		{
			DataKbn = Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_SETPROMOTION,
			MasterId1List = setPromotionIds,
			LanguageCode = languageCode,
			LanguageLocaleId = languageLocaleId,
		};

		var translationSettings = new NameTranslationSettingService().GetTranslationSettingsByMultipleMasterId1(searchCondition);
		return translationSettings;
	}
	#endregion

	#region -SetOrderShippingItemTranslationData 注文配送先情報翻訳情報設定
	/// <summary>
	/// 注文配送先情報翻訳情報設定
	/// </summary>
	/// <param name="languageCode">言語コード</param>
	/// <param name="languageLocaleId">言語ロケールID</param>
	/// <returns>翻訳情報設定後注文配送先情報リスト</returns>
	private List<Hashtable> SetOrderShippingItemTranslationData(string languageCode, string languageLocaleId)
	{
		// 翻訳情報設定
		var orderShippingItems = this.OrderShippingItems.Select(orderShippingItem => SetOrderShippingItemTranslationData(orderShippingItem, languageCode, languageLocaleId)).ToList();
		return orderShippingItems;
	}
	/// <summary>
	/// 注文配送先情報翻訳情報設定
	/// </summary>
	/// <param name="orderShippingItem">注文配送先情報</param>
	/// <param name="languageCode">言語コード</param>
	/// <param name="languageLocaleId">言語ロケールID</param>
	/// <returns>注文配送先情報</returns>
	private Hashtable SetOrderShippingItemTranslationData(Hashtable orderShippingItem, string languageCode, string languageLocaleId)
	{
		var childs = (List<DataRowView>)orderShippingItem["childs"];
		orderShippingItem["childs"] = childs.Select(child => SetOrderShippingItemTranslationData(child, languageCode, languageLocaleId)).ToList();

		return orderShippingItem;
	}
	/// <summary>
	/// 注文配送先情報翻訳情報設定
	/// </summary>
	/// <param name="orderShippingItem">注文配送先情報</param>
	/// <param name="languageCode">言語コード</param>
	/// <param name="languageLocaleId">言語ロケールID</param>
	/// <returns>翻訳情報設定後注文配送先情報</returns>
	private DataRowView SetOrderShippingItemTranslationData(DataRowView orderShippingItem, string languageCode, string languageLocaleId)
	{
		// 商品名
		var productTranslationName = NameTranslationCommon.GetOrderItemProductTranslationName(
			(string)orderShippingItem[Constants.FIELD_ORDERITEM_PRODUCT_NAME],
			(string)orderShippingItem[Constants.FIELD_ORDERITEM_PRODUCT_ID],
			(string)orderShippingItem[Constants.FIELD_ORDERITEM_VARIATION_ID],
			languageCode,
			languageLocaleId);

		orderShippingItem[Constants.FIELD_ORDERITEM_PRODUCT_NAME] = productTranslationName;

		// セップロが適用されていない場合は抜ける
		if (orderShippingItem[Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO] == DBNull.Value) return orderShippingItem;

		// セットプロモーション表示名
		var orderId = (string)orderShippingItem[Constants.FIELD_ORDERSETPROMOTION_ORDER_ID];
		var orderSetPromotionNo = (int)orderShippingItem[Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO];

		var setPromotionDispName = (string)(this.OrderSetPromotions.FirstOrDefault(
			setPromotion => ((orderId == (string)setPromotion[Constants.FIELD_ORDERSETPROMOTION_ORDER_ID])
				&& (orderSetPromotionNo == (int)setPromotion[Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO])))[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME]);
		orderShippingItem[Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_DISP_NAME] = setPromotionDispName;

		return orderShippingItem;
	}
	#endregion

	/// <summary>
	/// 注文変更処理
	/// </summary>
	/// <param name="orderOld">注文情報（変更前）</param>
	/// <param name="orderNew">注文情報（変更後）</param>
	/// <param name="paymentId">決済ID</param>
	/// <param name="isCardRegister">クレカ登録するか</param>
	/// <param name="isUpdateFixedPurchase">定期更新するか</param>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <param name="externalPaymentApiErrorMassage">External payment API error message</param>
	protected void ExecuteChangeOrder(
		OrderModel orderOld,
		OrderModel orderNew,
		string paymentId,
		bool isCardRegister,
		bool isUpdateFixedPurchase,
		out string errorMessage,
		out string externalPaymentApiErrorMassage)
	{
		errorMessage = "";
		// 外部決済連携実行
		var isExecuteExternalPayment = ExecuteExternalPayment(orderOld, orderNew, out externalPaymentApiErrorMassage);

		var hasExternalPaymentApiErrorMassage = (string.IsNullOrEmpty(externalPaymentApiErrorMassage) == false);

		// 外部決済連携実施時のみログを残す
		if (isExecuteExternalPayment)
		{
			string externalApiLog;

			if (hasExternalPaymentApiErrorMassage)
			{
				externalApiLog = externalPaymentApiErrorMassage;
			}
			else if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
				&& (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY))
			{
				var cancelMessage = orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
					? PaygentConstants.PAYGENT_PAIDY_REFUND_LOG_MESSAGE
					: PaygentConstants.PAYGENT_PAIDY_CANCEL_LOG_MESSAGE;

				externalApiLog = LogCreator.CreateMessageWithPaymentId(
					orderOld.CardTranId,
					orderOld.PaymentOrderId,
					orderOld.LastBilledAmount.ToPriceString(),
					cancelMessage);
			}
			else
			{
				externalApiLog = LogCreator.CreateMessageWithPaymentId(
					orderOld.OrderId,
					string.Empty,
					orderOld.OrderPaymentKbn);
			}

			OrderCommon.AppendExternalPaymentCooperationLog(
				hasExternalPaymentApiErrorMassage == false,
				orderOld.OrderId,
				externalApiLog,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);
		}

		if (hasExternalPaymentApiErrorMassage)
		{
			//再与信失敗エラー表示
			if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR);
			}
			else if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			{
				errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CVSAUTH_ERROR);
			}
			else if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				errorMessage =
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_TRYLINKAFTERPAYAUTH_ERROR);
			}
			else if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				errorMessage = externalPaymentApiErrorMassage;
			}
			else if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& (Constants.PAYMENT_PAYPAY_KBN == w2.App.Common.Constants.PaymentPayPayKbn.VeriTrans))
			{
				errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_CANCEL_PAYMENT_FAILED);
			}
			else
			{
				errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
			}

			return;
		}

		using (var accessor = new SqlAccessor())
		{
			// トランザクション開始
			accessor.OpenConnection();
			accessor.BeginTransaction();
			try
			{
				//注文データを更新
				if (ExecuteUpdateOrderAndRegisterUpdateHistory(orderNew, UpdateHistoryAction.DoNotInsert, accessor, isExecuteExternalPayment) == false)
				{
					throw new Exception("注文情報の更新に失敗しました。");
				}

				var updatedOrderPriceInfoByTaxRate =
					new OrderService().UpdateOrderPriceInfoByTaxRateModify(orderNew.OrderPriceByTaxRates, accessor);
				if (updatedOrderPriceInfoByTaxRate == 0)
				{
					throw new Exception("税率毎価格情報の更新に失敗しました。");
				}
				// セットプロモーションに決済料金割引があれば金額を合わせる
				if (orderNew.SetPromotions.Any()
					&& GetSetPromotionPaymentChargeFree() == Constants.FLG_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON)
				{
					foreach (var setpromotion in orderNew.SetPromotions)
					{
						setpromotion.PaymentChargeDiscountAmount = orderNew.OrderPriceExchange;
						var updateSetPromotionCount = new OrderService().UpdateSetPromotionForModify(new[] { setpromotion }, accessor);
						if (updateSetPromotionCount == 0) throw new Exception("注文セットプロモーションの更新に失敗しました。");
					}
				}

				if (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				{
					//クレジットカード枝番の更新
					var creditBranchCount = new OrderService().UpdateCreditBranchNo(
						orderNew.OrderId,
						orderNew.CreditBranchNo.Value,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.DoNotInsert,
						accessor);
					if (creditBranchCount == 0)
					{
						throw new Exception("クレジットカード枝番の更新に失敗しました。");
					}

					//クレジットカードを登録リストに表示させる
					if (isCardRegister)
					{
						var creditCardUpdateResult = new UserCreditCardService().UpdateDispFlg(
							orderNew.UserId,
							orderNew.CreditBranchNo.Value,
							true,
							Constants.FLG_LASTCHANGED_USER,
							UpdateHistoryAction.DoNotInsert,
							accessor);

						if (creditCardUpdateResult == false)
						{
							throw new Exception("ユーザークレジットカードの更新に失敗しました。");
						}
					}
				}

				// 領収書希望あり、かつ領収書出力しない決済に変更した場合、領収書情報をデフォールト値にリセット
				if (Constants.RECEIPT_OPTION_ENABLED
					&& Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN.Contains(orderNew.OrderPaymentKbn)
					&& (orderNew.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON))
				{
					new OrderService().UpdateOrderReceiptInfo(
						this.OrderModel.OrderId,
						Constants.FLG_ORDER_RECEIPT_FLG_OFF,
						null,
						string.Empty,
						string.Empty,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.DoNotInsert,
						accessor);
				}

				accessor.CommitTransaction();
			}
			catch (Exception ex)
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					orderNew.OrderPaymentKbn,
					PaymentFileLogger.PaymentType.Unknown,
					PaymentFileLogger.PaymentProcessingType.UpdateCreditInfo,
					BaseLogger.CreateExceptionMessage(ex),
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_USER_ID, orderNew.UserId},
						{Constants.FIELD_ORDER_ORDER_ID, orderNew.OrderId},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, orderNew.PaymentOrderId}
					});
				accessor.RollbackTransaction();
				errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_INSERT_ERROR);
				return;
			}
		}

		//定期台帳の更新
		if (isUpdateFixedPurchase)
		{
			// 継続課金の解約を行う
			string apiError;
			var success = FixedPurchaseHelper.CancelPaymentContinuous(
				this.FixedPurchaseModel.FixedPurchaseId,
				this.FixedPurchaseModel.OrderPaymentKbn,
				this.FixedPurchaseModel.ExternalPaymentAgreementId,
				Constants.FLG_LASTCHANGED_USER,
				out apiError);
			if (success == false)
			{
				this.PaymentContinousCancelErrorMessage = WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_PAYMENT_CONTINUOUS_CANCEL_NG_FOR_CHANGE_PAYMENT_FP);
			}
			else
			{
				new FixedPurchaseService().UpdateOrderPayment(
					this.OrderModel.FixedPurchaseId,
					orderNew.OrderPaymentKbn,
					orderNew.CreditBranchNo,
					orderNew.CardInstallmentsCode,
					"",
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);

				if (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				{
					new FixedPurchaseService()
						.UpdateForAuthSuccess(
							this.OrderModel.FixedPurchaseId,
							orderNew.CreditBranchNo.Value,
							orderNew.CardInstallmentsCode,
							Constants.FLG_LASTCHANGED_USER,
							LogCreator.CreateMessage(orderNew.OrderId, orderNew.PaymentOrderId),
							UpdateHistoryAction.DoNotInsert);
				}

				// 領収書出力しない決済に変更した場合、領収書情報をデフォールト値にリセット
				if (Constants.RECEIPT_OPTION_ENABLED
					&& Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN.Contains(orderNew.OrderPaymentKbn))
				{
					new FixedPurchaseService().UpdateReceiptInfo(
						this.OrderModel.FixedPurchaseId,
						Constants.FLG_ORDER_RECEIPT_FLG_OFF,
						string.Empty,
						string.Empty,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.DoNotInsert);
				}
			}
		}

		// 更新履歴登録
		new UpdateHistoryService().InsertAllForOrder(orderNew.OrderId, Constants.FLG_LASTCHANGED_USER);

		// メール送信
		SendMailCommon.SendModifyPurchaseHistoryMail(this.OrderModel.OrderId, SendMailCommon.PurchaseHistoryModify.PaymentMethod);
	}

	/// <summary>
	/// 外部決済連携実行
	/// </summary>
	/// <param name="orderOld">注文情報（変更前）</param>
	/// <param name="orderNew">注文情報（変更後）</param>
	/// <param name="apiErrorMassage">out APIエラーメッセージ</param>
	/// <returns>外部決済連携実行したか？</returns>
	protected bool ExecuteExternalPayment(OrderModel orderOld, OrderModel orderNew, out string apiErrorMassage)
	{
		apiErrorMassage = "";

		if (Constants.PAYMENT_REAUTH_ENABLED)
		{
			// Using for case order has NP after pay
			orderOld.IsExecuteReauthFromMyPage = (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			orderNew.IsExecuteReauthFromMyPage = (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);

			// 外部連携実行
			var reauth = new ReauthCreatorFacade(
				orderOld,
				orderNew,
				ReauthCreatorFacade.ExecuteTypes.System,
				ReauthCreatorFacade.ExecuteTypes.System,
				ReauthCreatorFacade.OrderActionTypes.Modify).CreateReauth();

			// 外部決済連携を実施しない場合は終了
			if (reauth.HasAnyAction == false)
			{
				if (orderNew.IsUpdateAtonePaymentFromMyPage
					|| orderNew.IsUpdateAfteePaymentFromMyPage)
				{
					orderNew.ExternalPaymentAuthDate = DateTime.Now;
					OrderCommon.CreateExternalPaymentStatusForPaymentAtoneOrAftee(orderNew);
				}

				if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
					&& (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten))
				{
					orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
				}

				if ((orderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
					&& PaygentUtility.CheckIsPaidyPaygentPayment(orderNew.OrderPaymentKbn))
				{
					orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
				}
				return false;
			};

			if ((orderOld.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
				&& (orderOld.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP)
				&& ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (orderOld.OrderPaymentKbn == orderNew.OrderPaymentKbn))
				&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom))
			{
				var requestAPIModifyAllItem = new AtobaraicomModifyOrderApi();
				var responseModifyAllItem = requestAPIModifyAllItem.ExecModifyOrderAllItem(orderNew);

				if (requestAPIModifyAllItem.IsAuthorizeHold)
				{
					orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
				}
				else if (requestAPIModifyAllItem.IsAuthorizeNG)
				{
					orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR;
				}
				else if (requestAPIModifyAllItem.Status == AtobaraicomConstants.ATOBARAICOM_API_RESULT_STATUS_ERROR)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = StringUtility.ChangeToBrTag(requestAPIModifyAllItem.Messages);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				var paymentMemo = OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
					orderNew.PaymentOrderId,
					orderNew.OrderPaymentKbn,
					orderNew.CardTranId,
					AtobaraicomConstants.ATOBARAICOM_PAYMENT_MEMO_TEXT_AUTH_CONFIRMED,
					orderNew.OrderPriceTotal);

				orderNew.PaymentMemo += string.Format("\r\n{0}", paymentMemo);

				return responseModifyAllItem;
			}

			var reauthResult = reauth.Execute();

			// 与信のみに失敗している場合エラー画面へ
			if (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Failure)
			{
				// Atodeneの場合、与信エラーにする
				if (reauth.AuthLostForError)
				{
					var service = new OrderService();
					service.UpdateExternalPaymentInfoForAuthError(
						orderNew.OrderId,
						reauthResult.ErrorMessages,
						orderNew.LastChanged,
						UpdateHistoryAction.Insert);
				}
				apiErrorMassage = StringUtility.ChangeToBrTag(reauthResult.ApiErrorMessages);
				return false;
			}

			if ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.FailureButAuthSuccess)
				&& PaygentUtility.CheckIsPaidyPaygentPayment(orderOld.OrderPaymentKbn))
			{
				apiErrorMassage = StringUtility.ChangeToBrTag(reauthResult.ApiErrorMessages);
				return false;
			}

			// Create payment memo for the order new with payout PayPay
			var hasChangeOtherPaymentToPayPay = ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& (orderNew.OrderPaymentKbn != orderOld.OrderPaymentKbn));

			// 決済メモがある場合は決済情報を更新する（無い場合は更新しない）
			if (string.IsNullOrEmpty(reauthResult.PaymentMemo) == false)
			{
				// 決済連携メモセット
				orderNew.PaymentMemo = orderNew.PaymentMemo
					+ (string.IsNullOrEmpty(orderNew.PaymentMemo) ? string.Empty : "\r\n")
					+ reauthResult.PaymentMemo;

				// Atone決済連携メモセット（別決済からの変更時のみ）
				if ((orderOld.OrderPaymentKbn != orderNew.OrderPaymentKbn)
					&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE))
				{
					var addPaymentMemo = OrderCommon.CreateOrderPaymentMemoForAuth(
						orderNew.OrderId,
						string.Empty,
						orderNew.OrderPaymentKbn,
						orderNew.CardTranId,
						orderNew.OrderPriceTotal);

					orderNew.PaymentMemo = orderNew.PaymentMemo
						+ (string.IsNullOrEmpty(orderNew.PaymentMemo)
							? orderNew.PaymentMemo + "\r\n"
							: string.Empty)
						+ addPaymentMemo;
				}

				// 何かしらアクションを持っていたら、決済取引ID・決済注文IDセット（キャンセルのみの場合は空が格納される想定）
				if (orderNew.IsUpdateAtonePaymentFromMyPage
					|| orderNew.IsUpdateAfteePaymentFromMyPage)
				{
					orderNew.ExternalPaymentAuthDate = DateTime.Now;
					OrderCommon.CreateExternalPaymentStatusForPaymentAtoneOrAftee(orderNew);
				}
				else if (reauth.HasAnyAction
					&& (hasChangeOtherPaymentToPayPay == false)
					&& (orderNew.IsUpdateBokuPaymentFromMyPage == false))
				{
					orderNew.CardTranId = reauthResult.CardTranId;
					orderNew.PaymentOrderId = (string.IsNullOrEmpty(reauthResult.PaymentOrderId) == false)
						? reauthResult.PaymentOrderId
						: orderNew.PaymentOrderId;

					if ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
						&& ((Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.GMO)
							|| (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans)))
					{
						orderNew.ExternalPaymentStatus = (reauth.HasRefund)
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_REFUND
							: (reauth.HasSales)
								? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
								: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_PEND;

						if (reauth.HasSales) orderNew.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
					}
				}
				// 再与信・減額・請求書再発行を持っていたら外部決済情報セット
				if (reauth.HasReauthOrReduceOrReprint)
				{
					orderNew.ExternalPaymentAuthDate = reauth.GetUpdateReauthDate(
						orderOld.ExternalPaymentAuthDate,
						orderOld.OrderPaymentKbn,
						orderNew.OrderPaymentKbn);
					orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
					OrderCommon.CreateExternalPaymentStatusForPaymentAtoneOrAftee(orderNew);

					// For case order has NP after payment, invoice bundle flag off and order old has shipment
					if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
						&& (orderOld.OrderPaymentKbn == orderNew.OrderPaymentKbn)
						&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP)
						&& (orderNew.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF)
						&& string.IsNullOrEmpty(reauthResult.ApiErrorMessages))
					{
						orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP;
					}

					if ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
						&& (orderOld.OrderPaymentKbn == orderNew.OrderPaymentKbn)
						&& string.IsNullOrEmpty(reauthResult.ApiErrorMessages))
					{
						orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
					}
				}
				// キャンセルのみであれば外部済情報リセット
				else if (reauth.HasOnlyCancel
					&& (orderNew.IsUpdateAtonePaymentFromMyPage == false)
					&& (orderNew.IsUpdateAfteePaymentFromMyPage == false)
					&& (orderNew.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
					&& (hasChangeOtherPaymentToPayPay == false)
					&& (orderNew.IsUpdateBokuPaymentFromMyPage == false))
				{
					orderNew.ExternalPaymentAuthDate = null;
					orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
					orderNew.CardTranId = string.Empty;
					orderNew.PaymentOrderId = string.Empty;
				}

				if (reauth.HasCancel
					&& (orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Atobaraicom)
					&& (reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success))
				{
					orderNew.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE;
				}
			}
			// 外部決済以外に変更された場合は決済取引IDを空、外部決済情報リセット
			else if ((orderOld.OrderPaymentKbn != orderNew.OrderPaymentKbn) && (orderNew.IsExternalPayment == false))
			{
				orderNew.CardTranId = string.Empty;
				orderNew.PaymentOrderId = string.Empty;
				orderNew.ExternalPaymentAuthDate = null;
				orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
			}

			if ((orderOld.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
				&& PaygentUtility.CheckIsPaidyPaygentPayment(orderNew.OrderPaymentKbn))
			{
				orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
			}

			// 過去にDSK後払いの請求書印字データ取得していた場合はここで削除
			if ((reauthResult.ResultDetail == ReauthResult.ResultDetailTypes.Success)
				&& (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
				&& (Constants.PAYMENT_CVS_DEF_KBN == Constants.PaymentCvsDef.Dsk)
				&& (reauth.HasOnlyCancel == false))
			{
				var invoiceDskDeferredService = new InvoiceDskDeferredService();
				var invoice = invoiceDskDeferredService.Get(orderNew.OrderId);
				if (invoice != null)
				{
					invoiceDskDeferredService.Delete(orderNew.OrderId);
				}
			}

			if (reauthResult.IsAuthResultHold)
			{
				orderNew.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
			}
			return true;
		}
		return false;
	}

	/// <summary>
	/// 注文情報更新
	/// </summary>
	/// <param name="orderNew">注文情報(更新後)</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <param name="accessor">SQLアクセサ</param>
	/// <param name="isExternalPayment">Is External Payment</param>
	/// <param name="isChangePoint">利用ポイントの変更か</param>
	/// <returns>成功:true 失敗:false</returns>
	protected bool ExecuteUpdateOrderAndRegisterUpdateHistory(
		OrderModel orderNew,
		UpdateHistoryAction updateHistoryAction,
		SqlAccessor accessor,
		bool isExternalPayment = false,
		bool isChangePoint = false)
	{
		orderNew.LastChanged = Constants.FLG_LASTCHANGED_USER;
		// 注文履歴登録
		var orderHistory = new OrderHistory
		{
			OrderId = orderNew.OrderId,
			Action = OrderHistory.ActionType.EcOrderModify,
			OpearatorName = Constants.FLG_LASTCHANGED_USER,
			Accessor = accessor,
		};
		orderHistory.HistoryBegin();

		// 注文登録
		var success = (new OrderService().UpdateForModify(
			orderNew,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.DoNotInsert,
			accessor) == 1);

		// 注文商品登録
		if (success && isChangePoint)
		{
			var orderItems = CartObject.CreateCartByOrder(orderNew).CreateOrderItems(orderNew);
			success = (new OrderService().UpdateItemForModify(
				orderItems,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert,
				accessor) > 0);
		}

		if (success && isExternalPayment)
		{
			UpdateOnlinePaymentStatus(orderNew, accessor);
		}

		if ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
			|| (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
			|| ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.GMO))
			|| (orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
		{
			success = (new OrderService().Modify(
				orderNew.OrderId,
				model =>
				{
					model.OnlinePaymentStatus = orderNew.OnlinePaymentStatus;
				},
				updateHistoryAction,
				accessor) > 0);
		}

		// 成功時処理
		if (success) orderHistory.HistoryComplete();

		return success;
	}

	/// <summary>
	/// 決済手数料の無料フラグを取得
	/// </summary>
	/// <returns>決済手数料の無料フラグ</returns>
	protected string GetSetPromotionPaymentChargeFree()
	{
		var paymentChargeFreeFlg = this.OrderSetPromotions
			.FirstOrDefault(
				item => (string)item[Constants.FIELD_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG] == Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON);
		return ((paymentChargeFreeFlg == null)
			? Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_OFF
			: Constants.FLG_SETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON);
	}

	/// <summary>
	/// Update Online Payment Status
	/// </summary>
	/// <param name="orderNew">Order New</param>
	/// <param name="accessor">SQL Accessor</param>
	private void UpdateOnlinePaymentStatus(OrderModel orderNew, SqlAccessor accessor)
	{
		if ((orderNew.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			&& (orderNew.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED))
		{
			new OrderService().UpdateOnlinePaymentStatus(
				orderNew.OrderId,
				Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}
	}

	/// <summary>
	/// Get Shipping Receiving Store Type Value
	/// </summary>
	/// <param name="isConvenienceStore">Is Convenience Store</param>
	/// <param name="shippingReceivingStoreType">Shipping Receiving Store Type</param>
	/// <returns>Shipping Receiving Store Type</returns>
	protected string GetShippingReceivingStoreTypeValue(bool isConvenienceStore, string shippingReceivingStoreType)
	{
		var result = (isConvenienceStore
				&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
				&& (string.IsNullOrEmpty(shippingReceivingStoreType) == false))
			? shippingReceivingStoreType
			: null;
		return result;
	}

	/// <summary>
	/// 注文アイテムごと小計金額可視化
	/// </summary>
	/// <param name="setPromotionNo">セットプロモーション番号</param>
	/// <param name="fixedAmount">頒布会定額価格</param>
	/// <param name="setPromotionItemNo">セットプロモーションアイテム番号</param>
	/// <returns>見えるであればTrue</returns>
	protected bool IsOrderSubtotalVisible(string setPromotionNo, object fixedAmount, int? setPromotionItemNo = null)
	{
		var result = false;
		if (setPromotionItemNo == null)
		{
			result = (IsOrderItemSubscriptionBoxFixedAmount(fixedAmount) == false)
				&& ((this.OrderModel.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON) || (setPromotionNo == ""));
		}
		else
		{
			result = (IsOrderItemSubscriptionBoxFixedAmount(fixedAmount) == false)
				&& (this.OrderModel.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_OFF)
				&& (setPromotionNo != "")
				&& (setPromotionItemNo == 1);
		}

		return result;
	}

	/// <summary>
	/// 頒布会定額コース内での最初の商品か
	/// </summary>
	/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
	/// <param name="fixedAmount">頒布会定額価格</param>
	/// <remarks>頒布会定額コース商品を含んだ同梱注文時のみ適用</remarks>
	/// <returns>頒布会定額コース内での最初の商品であればTRUE</returns>
	protected bool IsFirstItemInFixedAmountCourse(string subscriptionBoxCourseId, object fixedAmount)
	{
		if ((IsOrderItemSubscriptionBoxFixedAmount(fixedAmount) == false)
			|| this.OrderModel.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem()
			|| _displayedFixedAmountCourseId.Contains(subscriptionBoxCourseId)) return false;

		_displayedFixedAmountCourseId.Add(subscriptionBoxCourseId);
		return true;
	}

	/// <summary>
	/// 定額頒布会コース情報を表示するか
	/// </summary>
	/// <param name="orderItem">注文商品情報</param>
	/// <returns>表示するならTRUE</returns>
	protected bool DisplaySubscriptionBoxFixedAmountCourse(DataRowView orderItem)
	{
		// 同コースではない定額頒布会が注文同梱されているかつ、その定額頒布会の中で最後の商品かどうか判断
		if ((this.OrderModel.IsOrderCombined == false)
			|| this.OrderModel.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem()) return false;

		var isFixedAmountItem = IsOrderItemSubscriptionBoxFixedAmount(
			orderItem[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX]);
		if (isFixedAmountItem == false) return false;

		var result = IsLastItemOnOrderLine(orderItem);
		return result;
	}

	/// <summary>
	/// 注文明細行の最後の商品か
	/// </summary>
	/// <param name="orderItem">注文商品情報</param>
	/// <remarks>
	/// 注文明細行は原則1商品1行ずつで表示される<br/>
	/// しかし、頒布会定額コースが同梱されている注文の場合は、頒布会定額コースごとにまとめて1行で表示される
	/// </remarks>
	/// <returns>最後の商品であればTRUE</returns>
	protected bool IsLastItemOnOrderLine(DataRowView orderItem)
	{
		// 頒布会定額コースの商品以外、もしくは頒布会定額コースが1種類の場合は1行表示のため、ここで処理を抜ける
		if (this.OrderModel.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem()
			|| (IsOrderItemSubscriptionBoxFixedAmount(
				orderItem[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX]) == false)) return true;

		// 同じ頒布会定額コース内の商品で最後に表示される商品であればTRUEを返す
		var lastCourseItem = this.OrderModel.Items.Last(item =>
			item.SubscriptionBoxCourseId == (string)orderItem[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID_WITH_PREFIX]);
		var result = lastCourseItem.ProductId == (string)orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID];
		return result;
	}

	/// <summary>
	/// 定額頒布会コース情報表示のグリッド取得
	/// </summary>
	/// <param name="orderItem">注文商品情報</param>
	/// <returns>グリッド</returns>
	protected string GetFixedAmountCourseAreaGrid(DataRowView orderItem)
	{
		var fixedAmountItemCount = this.OrderModel.GetFixedAmountCourseItemCount(
			(string)orderItem[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID_WITH_PREFIX]);
		return string.Format(
			"grid-row: {0} / {1};",
			((int)orderItem[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO] + 1) - fixedAmountItemCount,
			(int)orderItem[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO] + 1);
	}

	/// <summary>
	/// 頒布会定額コースの注文商品か
	/// </summary>
	/// <param name="fixedAmount">頒布会定額価格</param>
	/// <returns>頒布会定額コースの注文商品であればTRUE</returns>
	protected bool IsOrderItemSubscriptionBoxFixedAmount(object fixedAmount)
	{
		var result = string.IsNullOrEmpty(StringUtility.ToEmpty(fixedAmount)) == false;
		return result;
	}

	/// <summary>
	/// Check is paidy payment with empty card tran id
	/// </summary>
	/// <returns>True if it is a paidy payment with empty card tran id</returns>
	protected bool CheckIsPaidyPaymentWithEmptyCardTranId()
	{
		return ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
			&& (this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			&& string.IsNullOrEmpty(this.OrderModel.CardTranId));
	}

	/// <summary>
	/// Check can cancel paidy payment
	/// </summary>
	/// <returns>True if it is a cancellable paidy payment</returns>
	protected bool CheckCanCancelPaidyPayment()
	{
		if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
			&& (this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY))
		{
			return ((string.IsNullOrEmpty(this.OrderModel.CardTranId) == false)
				&& (this.OrderModel.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_EXPIRED)
				&& (this.OrderModel.ExternalPaymentStatus != Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_CANCEL_EXPIRED));
		}
		return true;
	}

	/// <summary>
	/// Check is paygent payment
	/// </summary>
	/// <returns>True if it is a paygent payment</returns>
	protected bool CheckIsPaygentPayment()
	{
		return (this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET)
			|| (this.OrderModel.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ATM);
	}

	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return true; } }	// ログイン必須
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return true; } }	// マイページメニュー表示
	/// <summary>注文情報</summary>
	protected OrderModel OrderModel { get; set; }
	/// <summary>店舗情報</summary>
	protected ShopShippingModel ShopShippingModel { get; private set; }
	/// <summary>定期購入情報</summary>
	protected FixedPurchaseModel FixedPurchaseModel { get; private set; }
	/// <summary>支払い方法</summary>
	protected PaymentModel PaymentModel { get; private set; }
	/// <summary>ユーザの持っている利用可能なクレジットカード群</summary>
	protected UserCreditCard[] UserCreditCardsUsable { get; private set; }
	/// <summary>現在利用しているユーザクレジットカード情報</summary>
	protected UserCreditCard UserCreditCardInfo { get; private set; }
	/// <summary>注文配送先情報</summary>
	protected List<Hashtable> OrderShippingItems { get; private set; }
	/// <summary>注文セットプロモーションリスト</summary>
	protected List<Hashtable> OrderSetPromotions { get; set; }
	/// <summary>注文商品毎のシリアルキー情報</summary>
	protected Dictionary<string, DataView> OrderItemSerialKeys { get; set; }
	/// <summary>配送希望日の変更可否</summary>
	protected List<bool> IsModifyShippingDates { get; private set; }
	/// <summary>配送時間帯の変更可否</summary>
	protected List<bool> IsModifyShippingTimes { get; private set; }
	/// <summary>注文情報全体の変更可否</summary>
	protected bool IsModifyOrder { get; private set; }
	/// <summary>お支払い方の変更可否</summary>
	protected bool CanModifyPayment { get; set; }
	/// <summary>お届け先の変更可否</summary>
	protected bool IsModifyShipping { get; private set; }
	/// <summary>利用ポイントの変更可否</summary>
	protected bool IsModifyUsePoint { get; private set; }
	/// <summary>定期購入からの注文かどうか</summary>
	protected bool IsFixedPurchase { get; private set; }
	/// <summary>支払い方法の制限時メッセージ</summary>
	protected Hashtable DispLimitedPaymentMessages { get; set; }
	/// <summary>支払い方法変更 クリック不可の理由</summary>
	protected string ExplanationOrderPaymentKbn { get; set; }
	/// <summary>利用ポイント変更 クリック不可の理由</summary>
	protected string ExplanationPointUse { get; set; }
	/// <summary>お届け先変更 クリック不可の理由</summary>
	protected string ExplanationShipping { get; set; }
	/// <summary>配送希望日変更 クリック不可の理由</summary>
	protected List<string> ExplanationShippingDates { get; set; }
	/// <summary>配送時間帯変更 クリック不可の理由</summary>
	protected List<string> ExplanationShippingTimes { get; set; }
	/// <summary>キャンセルできるか可否</summary>
	protected bool IsModifyCancel { get; set; }
	/// <summary>キャンセル可能かメッセージ</summary>
	protected string IsModifyCancelMessage { get; set; }
	/// <summary>キャンセル可能時間メッセージ</summary>
	protected string OrderCancelTimeMessage { get; set; }
	/// <summary>Taiwan Order Invoice Model</summary>
	protected TwOrderInvoiceModel TwOrderInvoiceModel
	{
		get { return (TwOrderInvoiceModel)ViewState["TwOrderInvoiceModel"]; }
		set { ViewState["TwOrderInvoiceModel"] = value; }
	}
	/// <summary>Is Update Fixed Purchase Shipping</summary>
	protected bool IsUpdateShippingFixedPurchase { get; set; }
	/// <summary>Is Display Input Order Payment Kbn</summary>
	protected bool IsDisplayInputOrderPaymentKbn
	{
		get
		{
			var result = (this.OrderModel.DigitalContentsFlg == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_INVALID)
				&& this.IsModifyOrder && this.CanModifyPayment
				&& ((this.OrderModel.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
					|| (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)
					|| (ECPayUtility.GetIsCollection(this.OrderModel.Shippings[0].ShippingReceivingStoreType)
						== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_OFF));
			return result;
		}
	}
	/// <summary>継続課金解約エラーメッセージ</summary>
	protected string PaymentContinousCancelErrorMessage
	{
		get { return (string)Session["PaymentContinousCancelError"] ?? string.Empty; }
		set { Session["PaymentContinousCancelError"] = value; }
	}
	/// <summary>キャンセル可能な時間</summary>
	protected DateTime OrderCancellationTime
	{
		get
		{
			return ((DateTime)this.OrderModel.OrderDate).AddMinutes(Constants.ORDER_HISTORY_DETAIL_ORDER_CANCEL_TIME);
		}
	}
	/// <summary>お支払い金額の変更可否</summary>
	protected bool CanUseModifyPayment { get; set; }
	/// <summary>更新前商品情報</summary>
	protected OrderItemModel[] OldItems { get; set; }
	/// <summary>Explanation order extend</summary>
	protected string ExplanationOrderExtend { get; set; }
}
