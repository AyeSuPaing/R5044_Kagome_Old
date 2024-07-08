/*
=========================================================================================================
  Module      : 注文情報登録確認ページ処理(OrderRegistConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Config;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Mail;
using w2.App.Common.NextEngine;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.FixedPurchaseCombine;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Order.Register;
using w2.App.Common.User;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.MailTemplate;
using w2.Domain.Order;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

/// <summary>
/// 注文情報登録確認ページ処理
/// </summary>
public partial class Form_OrderRegist_OrderRegistConfirm : OrderRegistPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 一度完了している場合は入力ページへ
		if (this.OrderParams == null)
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_REGIST_INPUT);
		}

		// 確認画面表示
		if (!IsPostBack)
		{
			RefreshView();
		}

		// 項目メモ一覧取得
		this.FieldMemoSettingList = GetFieldMemoSettingList(Constants.TABLE_ORDER);

		// 商品購入制限チェック（類似配送先を含む）
		if (Constants.PRODUCT_ORDER_LIMIT_ENABLED)
		{
			CheckProductOrderLimitSimilarShipping();
		}

		Session["SubscriptionBoxFixedAmount"] = null;
		Session["SubscriptionBoxCourseId"] = null;
	}

	/// <summary>
	/// 表示更新
	/// </summary>
	private void RefreshView()
	{
		var orderInput = (Hashtable)this.OrderParams["order_input"];
		var user = (Hashtable)this.OrderParams["user_input"];
		var cart = (CartObject)this.OrderParams["cart"];

		// 基本情報表示
		lOrderKbn.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_ORDER,
				Constants.FIELD_ORDER_ORDER_KBN,
				orderInput[Constants.FIELD_ORDER_ORDER_KBN]));
		lOrderPaymentKbn.Text = GetEncodedHtmlDisplayMessage(cart.Payment.PaymentName);

		// Gmo cvs type
		dvGmoCvsType.Visible =
			((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
				&& (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Gmo));
		lGmoCvsType.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_PAYMENT,
				Constants.PAYMENT_GMO_CVS_TYPE,
				StringUtility.ToEmpty(orderInput[Constants.PAYMENT_GMO_CVS_TYPE])));

		// Rakuten cvs type
		dvRakutenType.Visible = ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
			&& (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten));
		lRakutenCvsType.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_PAYMENT,
				Constants.PAYMENT_RAKUTEN_CVS_TYPE,
				StringUtility.ToEmpty(orderInput[Constants.PAYMENT_RAKUTEN_CVS_TYPE])));

		// 注文者情報表示
		lOwnerName.Text = GetEncodedHtmlDisplayMessage(cart.Owner.Name);
		lOwnerNameKana.Text = GetEncodedHtmlDisplayMessage(cart.Owner.NameKana);
		lOwnerKbn.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_ORDEROWNER,
				Constants.FIELD_ORDEROWNER_OWNER_KBN,
				cart.Owner.OwnerKbn));
		lOwnerTel1.Text = GetEncodedHtmlDisplayMessage(cart.Owner.Tel1);
		lOwnerTel2.Text = GetEncodedHtmlDisplayMessage(cart.Owner.Tel2);
		lOwnerMailAddr.Text = GetEncodedHtmlDisplayMessage(cart.Owner.MailAddr);
		lOwnerMailAddr2.Text = GetEncodedHtmlDisplayMessage(cart.Owner.MailAddr2);
		lOwnerZip.Text = GetEncodedHtmlDisplayMessage(cart.Owner.Zip);
		lOwnerAddr1.Text = GetEncodedHtmlDisplayMessage(cart.Owner.Addr1);
		lOwnerAddr2.Text = GetEncodedHtmlDisplayMessage(cart.Owner.Addr2);
		lOwnerAddr3.Text = GetEncodedHtmlDisplayMessage(cart.Owner.Addr3);
		lOwnerAddr4.Text = GetEncodedHtmlDisplayMessage(cart.Owner.Addr4);
		lOwnerCompanyName.Text = GetEncodedHtmlDisplayMessage(cart.Owner.CompanyName);
		lOwnerCompanyPostName.Text = GetEncodedHtmlDisplayMessage(cart.Owner.CompanyPostName);
		lOwnerSex.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_ORDEROWNER,
				Constants.FIELD_ORDEROWNER_OWNER_SEX,
				cart.Owner.Sex));
		lOwnerBirth.Text = GetEncodedHtmlDisplayMessage(
			DateTimeUtility.ToStringForManager(
				cart.Owner.Birth,
				DateTimeUtility.FormatType.LongDate1LetterNoneServerTime));

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			lOwnerAccessCountryIsoCode.Text = GetEncodedHtmlDisplayMessage(cart.Owner.AccessCountryIsoCode);
			lOwnerDispLanguageCode.Text = GetEncodedHtmlDisplayMessage(cart.Owner.DispLanguageCode);
			lOwnerDispLanguageLocaleId.Text = GetEncodedHtmlDisplayMessage(
				GlobalConfigUtil.LanguageLocaleIdDisplayFormat(cart.Owner.DispLanguageLocaleId));
			lOwnerDispCurrencyCode.Text = GetEncodedHtmlDisplayMessage(cart.Owner.DispCurrencyCode);
			lOwnerDispCurrencyLocaleId.Text = GetEncodedHtmlDisplayMessage(
				GlobalConfigUtil.CurrencyLocaleIdDisplayFormat(cart.Owner.DispCurrencyLocaleId));

			lOwnerCountryName.Text = GetEncodedHtmlDisplayMessage(cart.Owner.AddrCountryName);
			lOwnerAddr5.Text = GetEncodedHtmlDisplayMessage(cart.Owner.Addr5);
			lOwnerZipGlobal.Text = GetEncodedHtmlDisplayMessage(cart.Owner.Zip);
		}

		var userId = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_USER_ID]);
		dvAllowOrderOwnerSaveToUser.Visible = (string.IsNullOrEmpty(userId) == false);

		// ユーザー情報表示
		dvUserId.Visible = (string.IsNullOrEmpty(userId) == false);
		lUserId.Text = GetEncodedHtmlDisplayMessage(userId);
		lAdvCode.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_ADVCODE_NEW]));
		lAdvName.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToEmpty(orderInput[Constants.FIELD_ADVCODE_MEDIA_NAME]));
		lUserMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(user[Constants.FIELD_USER_USER_MEMO]);
		lUserManagementLevel.Text = GetEncodedHtmlDisplayMessage(
			UserManagementLevelUtility.GetUserManagementLevelName(
				StringUtility.ToEmpty(user[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID])));
		lMailFlg.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_USER,
				Constants.FIELD_USER_MAIL_FLG,
				user[Constants.FIELD_USER_MAIL_FLG]));

		// コンバージョン情報
		lInflowContentsType.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_ORDER,
				Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE,
				orderInput[Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE]));
		lInflowContentsId.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_INFLOW_CONTENTS_ID]));

		// 配送先情報表示
		var cartShipping = cart.GetShipping();
		if (string.IsNullOrEmpty(cartShipping.RealShopId) == false)
		{
			dvOrderShippingSameAsOwner.Visible = false;
			dvOrderShipping.Visible = false;
			dvOrderShippingConvenienceStore.Visible = false;
			dvOrderShippingRealStore.Visible = true;
			lbStoreName.Text = GetEncodedHtmlDisplayMessage(cartShipping.RealShopName);
			lbStoreAddress.Text = string.Format("〒{0}<br />{1} {2}<br />{3}<br />{4}<br />{5}",
				GetEncodedHtmlDisplayMessage(cartShipping.Zip),
				GetEncodedHtmlDisplayMessage(cartShipping.Addr1),
				GetEncodedHtmlDisplayMessage(cartShipping.Addr2),
				GetEncodedHtmlDisplayMessage(cartShipping.Addr3),
				GetEncodedHtmlDisplayMessage(cartShipping.Addr4),
				GetEncodedHtmlDisplayMessage(cartShipping.Addr5));
			lbStoreTel.Text = GetEncodedHtmlDisplayMessage(cartShipping.Tel1);
			lbStoreOpeningHours.Text = GetEncodedHtmlDisplayMessage(cartShipping.RealShopOpenningHours);
		}
		else if (cartShipping.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER)
		{
			dvOrderShippingSameAsOwner.Visible = true;
			dvOrderShipping.Visible = false;
			dvOrderShippingConvenienceStore.Visible = false;
			dvOrderShippingRealStore.Visible = false;
		}
		else if (cartShipping.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
		{
			dvOrderShippingSameAsOwner.Visible = false;
			dvOrderShipping.Visible = false;
			dvOrderShippingConvenienceStore.Visible = true;
			dvOrderShippingRealStore.Visible = false;
			lCvsShopNo.Text = GetEncodedHtmlDisplayMessage(cartShipping.ConvenienceStoreId);
			lCvsShopName.Text = GetEncodedHtmlDisplayMessage(cartShipping.Name);
			lCvsShopAddress.Text = GetEncodedHtmlDisplayMessage(cartShipping.Addr4);
			lCvsShopTel.Text = GetEncodedHtmlDisplayMessage(cartShipping.Tel1);
		}
		else
		{
			dvOrderShippingSameAsOwner.Visible = false;
			dvOrderShipping.Visible = true;
			lShippingZip.Text = GetEncodedHtmlDisplayMessage(cartShipping.Zip);
			lShippingAddr1.Text = GetEncodedHtmlDisplayMessage(cartShipping.Addr1);
			lShippingAddr2.Text = GetEncodedHtmlDisplayMessage(cartShipping.Addr2);
			lShippingAddr3.Text = GetEncodedHtmlDisplayMessage(cartShipping.Addr3);
			lShippingAddr4.Text = GetEncodedHtmlDisplayMessage(cartShipping.Addr4);
			lShippingCompanyName.Text = GetEncodedHtmlDisplayMessage(cartShipping.CompanyName);
			lShippingCompanyPostName.Text = GetEncodedHtmlDisplayMessage(cartShipping.CompanyPostName);
			lShippingName.Text = GetEncodedHtmlDisplayMessage(cartShipping.Name);
			lShippingNameKana.Text = GetEncodedHtmlDisplayMessage(cartShipping.NameKana);
			lShippingTel1.Text = GetEncodedHtmlDisplayMessage(cartShipping.Tel1);

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				lShippingCountryName.Text = GetEncodedHtmlDisplayMessage(cartShipping.ShippingCountryName);
				lShippingAddr5.Text = GetEncodedHtmlDisplayMessage(cartShipping.Addr5);
				lShippingZipGlobal.Text = GetEncodedHtmlDisplayMessage(cartShipping.Zip);
			}
			dvOrderShipping.Visible = (cartShipping.ConvenienceStoreFlg
				== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF);
			dvOrderShippingConvenienceStore.Visible = false;

			if (dvOrderShipping.Visible == false)
			{
				dvOrderShippingConvenienceStore.Visible = true;
				lCvsShopNo.Text = GetEncodedHtmlDisplayMessage(cartShipping.ConvenienceStoreId);
				lCvsShopName.Text = GetEncodedHtmlDisplayMessage(cartShipping.Name);
				lCvsShopAddress.Text = GetEncodedHtmlDisplayMessage(cartShipping.Addr4);
				lCvsShopTel.Text = GetEncodedHtmlDisplayMessage(cartShipping.Tel1);
			}
			dvOrderShippingRealStore.Visible = false;
		}

		// 配送指定表示
		dvShippingDateSetFlgValid.Visible = cartShipping.SpecifyShippingDateFlg;
		lShippingDate.Text = GetEncodedHtmlDisplayMessage(
			DateTimeUtility.ToStringForManager(
				cartShipping.ShippingDate,
				DateTimeUtility.FormatType.LongDateWeekOfDay2Letter,
				ReplaceTag("@@DispText.shipping_date_list.none@@")));
		dvShippingTimeSetFlgValid.Visible = cartShipping.SpecifyShippingTimeFlg;
		lShippingMethod.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_ORDERSHIPPING,
				Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD,
				cartShipping.ShippingMethod));
		lDeliveryCompany.Text = GetEncodedHtmlDisplayMessage(GetDeliveryCompanyName(cartShipping.DeliveryCompanyId));
		lShippingTime.Text = GetEncodedHtmlDisplayMessage(
			(string.IsNullOrEmpty(cartShipping.ShippingTime) == false)
				? cartShipping.ShippingTimeMessage
				: ReplaceTag("@@DispText.shipping_time_list.none@@"));
		lShippingCheckNo.Text = cartShipping.ShippingCheckNo;
		if (CanShowOrderShippingAlertMessage(orderInput, cart))
		{
			dvOrderShippingAlert.Visible = true;
			lbOrderShippingAlertMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_CANNOT_SHIPPING_MAIL);
		}
		if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED
			&& CanShowOrderDeliveryCompanyAlertMessage(orderInput, cart))
		{
			dvOrderShippingAlert.Visible = true;
			lbOrderShippingAlertMessage.Text = WebMessages
				.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_CANNOT_SHIPPING_SERVICE)
				.Replace("@@ 1 @@", lDeliveryCompany.Text);
		}

		// 決済情報表示
		if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			this.UseNewCreditCard = (cart.Payment.UserCreditCard == null);
			dvOrderKbnCredit.Visible = true;
			lCreditCardCompany.Text = GetEncodedHtmlDisplayMessage(
				ValueText.GetValueText(
					Constants.TABLE_ORDER,
					OrderCommon.CreditCompanyValueTextFieldName,
					cart.Payment.CreditCardCompany));
			lCreditCardNo.Text = GetEncodedHtmlDisplayMessage(
				(cart.Payment.UserCreditCard != null)
					? string.Format(
						"************{0}",
						cart.Payment.UserCreditCard.LastFourDigit)
					: cart.Payment.CreditCardNo);
			if (cart.Payment.CreditToken != null)
			{
				var expireDate = cart.Payment.CreditToken.ExpireDate;
				if (expireDate.HasValue)
				{
					lCreditCardNo.Text += GetEncodedHtmlDisplayMessage(
						string.Format("　（{0}：{1}）",
							//「トークン有効期限」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_ORDER,
								Constants.VALUETEXT_PARAM_ORDER_REGIST,
								Constants.VALUETEXT_PARAM_ORDER_REGIST_ORDER_TOKEN_EXPIRE_DATE),
							DateTimeUtility.ToStringForManager(
								expireDate,
								DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter)));
				}
			}
			lValidity.Text = GetEncodedHtmlDisplayMessage(
				string.Format("{0}/{1} {2}",
					cart.Payment.CreditExpireMonth,
					cart.Payment.CreditExpireYear,
					lValidity.Text));
			lSecurityCode.Text = GetEncodedHtmlDisplayMessage(cart.Payment.CreditSecurityCode);
			dvSecurityCode.Visible = (string.IsNullOrEmpty(lSecurityCode.Text) == false);
			dvInstallments.Visible = OrderCommon.CreditInstallmentsSelectable;
			if (OrderCommon.CreditInstallmentsSelectable)
			{
				lInstallments.Text = GetEncodedHtmlDisplayMessage(
					ValueText.GetValueText(
						Constants.TABLE_ORDER,
						OrderCommon.CreditInstallmentsValueTextFieldName,
						cart.Payment.CreditInstallmentsCode));
			}
			lName.Text = GetEncodedHtmlDisplayMessage(
				(cart.Payment.UserCreditCard == null)
					? cart.Payment.CreditAuthorName
					: cart.Payment.UserCreditCard.AuthorName);
			dvRegistCreditCard.Visible = cart.Payment.UserCreditCardRegistable;
			lRegistCreditCard.Text = GetEncodedHtmlDisplayMessage(
				cart.Payment.UserCreditCardRegistable
					? ReplaceTag("@@DispText.common_message.register@@")
					: ReplaceTag("@@DispText.common_message.do_not_register@@"));
			lUserCreditCardName.Text = GetEncodedHtmlDisplayMessage(
				(cart.Payment.UserCreditCard != null)
					? cart.Payment.UserCreditCard.CardDispName
					: cart.Payment.UserCreditCardName);
			dvUserCreditCardName.Visible = (string.IsNullOrEmpty(lUserCreditCardName.Text) == false);
		}
		else
		{
			dvOrderKbnCredit.Visible = false;
		}

		// 定期購入情報セット
		if (((cart.IsCombineParentOrderHasFixedPurchase == false)
				|| (cart.HasSubscriptionBox
					&& (cart.IsOrderCombinedWithSameSubscriptionBoxCourse() == false)))
			&& (cartShipping.FixedPurchaseKbn != null))
		{
			dvFixedPurchaseShippingInfo.Visible = true;
			lFixedPurchasePattern.Text = GetEncodedHtmlDisplayMessage(
				OrderCommon.CreateFixedPurchaseSettingMessage(
					cartShipping.FixedPurchaseKbn,
					cartShipping.FixedPurchaseSetting));
			lFixedPurchasePatternTitle.Text = GetEncodedHtmlDisplayMessage(
				GetFixedPurchasePatternTitle(cartShipping.FixedPurchaseKbn));
			lFirstShippingDate.Text = GetEncodedHtmlDisplayMessage(
				DateTimeUtility.ToStringForManager(
					cartShipping.FirstShippingDate,
					DateTimeUtility.FormatType.LongDateWeekOfDay2Letter));
			lNextShippingDate.Text = GetEncodedHtmlDisplayMessage(
				DateTimeUtility.ToStringForManager(
					cartShipping.NextShippingDate,
					DateTimeUtility.FormatType.LongDateWeekOfDay2Letter));
			lNextNextShippingDate.Text = GetEncodedHtmlDisplayMessage(
				DateTimeUtility.ToStringForManager(
					cartShipping.NextNextShippingDate,
					DateTimeUtility.FormatType.LongDateWeekOfDay2Letter));
			lShippingTime2.Text = lShippingTime.Text;
		}
		else
		{
			dvFixedPurchaseShippingInfo.Visible = false;
		}

		// 注文商品
		rItemList.DataSource = cart.Items.FindAll(cp => cp.QuantitiyUnallocatedToSet != 0);
		rItemList.DataBind();
		rSetPromotion.DataSource = cart.SetPromotions.Items;
		rSetPromotion.DataBind();

		// 金額系セット
		lOrderPriceSubTotal.Text = GetEncodedHtmlDisplayMessage(cart.PriceSubtotal.ToPriceString(true));
		lOrderPriceShipping.Text = GetEncodedHtmlDisplayMessage(cart.PriceShipping.ToPriceString(true));
		lOrderPriceExchange.Text = GetEncodedHtmlDisplayMessage(cart.Payment.PriceExchange.ToPriceString(true));

		lOrderPriceRegulation.Text = GetEncodedHtmlDisplayMessage(cart.PriceRegulation.ToPriceString(true));
		AddAttributesForControlDisplay(
			trPriceRegulation,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(cart.PriceRegulation < 0));

		lMemberRankDiscount.Text = GetEncodedHtmlDisplayMessage(
			(cart.MemberRankDiscount * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trMemberRankDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(cart.MemberRankDiscount > 0));

		lFixedPurchaseMemberDiscount.Text = GetEncodedHtmlDisplayMessage(
			(cart.FixedPurchaseMemberDiscountAmount * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trFixedPurchaseMemberDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(cart.FixedPurchaseMemberDiscountAmount > 0));

		lPointUsePrice.Text = GetEncodedHtmlDisplayMessage(
			(cart.UsePointPrice * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trPointDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(cart.UsePointPrice > 0));

		lCouponUsePrice.Text = GetEncodedHtmlDisplayMessage(
			(cart.UseCouponPrice * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trCouponDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(cart.UseCouponPrice > 0));

		lFixedPurchaseDiscountPrice.Text = GetEncodedHtmlDisplayMessage(cart.FixedPurchaseDiscount.ToPriceString(true));
		AddAttributesForControlDisplay(
			trFixedPurchaseDiscountPrice,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(cart.FixedPurchaseDiscount > 0));

		lOrderPriceTax.Text = GetEncodedHtmlDisplayMessage(cart.PriceSubtotalTax.ToPriceString(true));
		lOrderPriceTotalBottom.Text
			= lOrderPriceTotal.Text
				= GetEncodedHtmlDisplayMessage(cart.PriceTotal.ToPriceString(true));

		var combinedCartList = new CartObjectList(cart.CartUserId, cart.OrderKbn, false);

		// Show parent order when combine order at order regist input
		if ((string.IsNullOrEmpty(cart.OrderCombineParentOrderId) == false)
			&& (cart.OrderCombineParentOrderId.Contains(',') == false))
		{
			var parentOrder = DomainFacade.Instance.OrderService.Get(cart.OrderCombineParentOrderId);
			dvOrderCombine.Visible = true;
			combinedCartList.AddCartVirtural(cart);
			this.OrderStatusForCombine = parentOrder.OrderStatus;
			this.OrderCreatedDateForCombine = parentOrder.DateCreated;
			this.OrderPaymentStatusForCombine = parentOrder.OrderPaymentStatus;
			this.OrderItemForCombine = parentOrder.Items;
			this.OrderPriceForCombine = parentOrder.OrderPriceTotal;
			this.OrderCouponForCombine = (parentOrder.Coupons.Length != 0)
				? string.Format(
					"{0}（{1}）",
					// 「利用あり」
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER_REGIST_INPUT,
						Constants.VALUETEXT_PARAM_ORDER_REGIST_INPUT_OTHER_MESSAGE,
						Constants.VALUETEXT_PARAM_ORDER_REGIST_INPUT_AVAILABLE),
					parentOrder.Coupons[0].CouponName)
				: string.Empty;
			rOrderCombine.DataSource = combinedCartList;
			rOrderCombine.DataBind();
		}
		dvCombineArea.Visible = dvOrderCombine.Visible;

		rTotalPriceByTaxRate.DataSource = cart.PriceInfoByTaxRate;
		rTotalPriceByTaxRate.DataBind();

		// 決済金額
		cart.SettlementCurrency = CurrencyManager.GetSettlementCurrency(cart.Payment.PaymentId);
		cart.SettlementRate = CurrencyManager.GetSettlementRate(cart.SettlementCurrency);
		cart.SettlementAmount = CurrencyManager.GetSettlementAmount(
			cart.PriceTotal,
			cart.SettlementRate,
			cart.SettlementCurrency);
		lSettlementAmount.Text = CurrencyManager.ToSettlementCurrencyNotation(
			cart.SettlementAmount,
			cart.SettlementCurrency);

		rOrderSetPromotionProductDiscount.DataSource
			= rOrderSetPromotionShippingChargeDiscount.DataSource
				= rOrderSetPromotionPaymentChargeDiscount.DataSource
					= cart.SetPromotions.Items;
		rOrderSetPromotionProductDiscount.DataBind();
		rOrderSetPromotionShippingChargeDiscount.DataBind();
		rOrderSetPromotionPaymentChargeDiscount.DataBind();

		// 各種メモ
		rOrderMemos.DataSource = cart.OrderMemos;
		rOrderMemos.DataBind();
		this.FieldMemoSettingList = BasePage.GetFieldMemoSettingList(Constants.TABLE_ORDER);
		lPaymentMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(orderInput[Constants.FIELD_ORDER_PAYMENT_MEMO]);
		lManagerMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(orderInput[Constants.FIELD_ORDER_MANAGEMENT_MEMO]);
		lShippingMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(orderInput[Constants.FIELD_ORDER_SHIPPING_MEMO]);
		lFixedPurchaseManagementMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(
			orderInput[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO]);
		lRelationMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(orderInput[Constants.FIELD_ORDER_RELATION_MEMO]);
		lRegulationMemo.Text = WebSanitizer.HtmlEncodeChangeToBr(cart.RegulationMemo);

		// ポイント
		lOrderPointUse.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToNumeric(cart.UsePoint));
		lOrderPointAdd.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToNumeric(cart.BuyPoint + cart.FirstBuyPoint));
		var userPoint = PointOptionUtility.GetUserPoint(cart.OrderUserId);
		var userPointUsable = (userPoint != null) ? userPoint.PointUsable : 0;
		lUserPointUsable.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToNumeric(userPointUsable));
		dvUseAllPointFlg.Visible = cart.UseAllPointFlg;

		// クーポン
		var hasCoupon = ((cart.Coupon != null) && (string.IsNullOrEmpty(cart.Coupon.CouponCode) == false));
		lCouponCode.Text = GetEncodedHtmlDisplayMessage(
			hasCoupon
				? cart.Coupon.CouponCode
				: ReplaceTag("@@DispText.common_message.unspecified@@"));
		dvCouponDetail.Visible = hasCoupon;
		if (hasCoupon)
		{
			lCouponDiscount.Text = GetEncodedHtmlDisplayMessage(CouponOptionUtility.GetCouponDiscountString(cart.Coupon));
			lCouponName.Text = GetEncodedHtmlDisplayMessage(cart.Coupon.CouponName);
			lCouponDispName.Text = GetEncodedHtmlDisplayMessage(cart.Coupon.CouponDispName);
		}

		// デジタルコンテンツなら配送情報を非表示に
		dvShippingTo.Visible = (cart.IsDigitalContentsOnly == false);
		dvShippingInfo.Visible = ((cart.IsDigitalContentsOnly == false)
			&& string.IsNullOrEmpty(cartShipping.RealShopId));

		// 領収書情報セット
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			lReceiptFlg.Text = GetEncodedHtmlDisplayMessage(
				ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_RECEIPT_FLG,
					cart.ReceiptFlg));
			lReceiptAddress.Text = GetEncodedHtmlDisplayMessage(cart.ReceiptAddress);
			lReceiptProviso.Text = GetEncodedHtmlDisplayMessage(cart.ReceiptProviso);
		}

		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			rOrderExtendInput.DataSource = new OrderExtendInput(cart.OrderExtend.ToDictionary(o => o.Key, o => o.Value.Value)).OrderExtendItems;
			rOrderExtendInput.DataBind();
		}

		if (OrderCommon.DisplayTwInvoiceInfo()
			&& this.IsShippingAddrTw)
		{
			lUniformInvoice.Text = ValueText.GetValueText(
				Constants.TABLE_TWORDERINVOICE,
				Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE,
				cartShipping.UniformInvoiceType);
			switch (StringUtility.ToEmpty(cartShipping.UniformInvoiceType))
			{
				case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
				{
					lCarryType.Text = GetEncodedHtmlDisplayMessage(
						((string.IsNullOrEmpty(cartShipping.CarryTypeOptionValue) == false)
							? (string.Format("{0} : {1}",
								ValueText.GetValueText(
									Constants.TABLE_TWORDERINVOICE,
									Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE,
									cartShipping.CarryType),
								cartShipping.CarryTypeOptionValue))
							: ValueText.GetValueText(
								Constants.TABLE_TWORDERINVOICE,
								Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE,
								cartShipping.CarryType)));
					dvUniformInvoicePersonal.Visible = true;
					dvUniformInvoiceCompanyOption1.Visible = false;
					dvUniformInvoiceCompanyOption2.Visible = false;
					dvUniformInvoiceDonate.Visible = false;
				}
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
				{
					lUniformInvoiceOption1.Text = GetEncodedHtmlDisplayMessage(cartShipping.UniformInvoiceOption1);
					lUniformInvoiceOption2.Text = GetEncodedHtmlDisplayMessage(cartShipping.UniformInvoiceOption2);
					dvUniformInvoicePersonal.Visible = false;
					dvUniformInvoiceCompanyOption1.Visible = true;
					dvUniformInvoiceCompanyOption2.Visible = true;
					dvUniformInvoiceDonate.Visible = false;
				}
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				{
					lUniformInvoiceDonate.Text = GetEncodedHtmlDisplayMessage(cartShipping.UniformInvoiceOption1);
					dvUniformInvoicePersonal.Visible = false;
					dvUniformInvoiceCompanyOption1.Visible = false;
					dvUniformInvoiceCompanyOption2.Visible = false;
					dvUniformInvoiceDonate.Visible = true;
				}
					break;
			}
		}
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{
			var subscriptionBoxCourseId = (string)orderInput[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID];
			var subscriptionBoxMamagementName = (string)orderInput[Constants.FIELD_SUBSCRIPTIONBOX_MANAGEMENT_NAME];
			lSubscriptionName.Text = WebSanitizer.HtmlEncode(string.Format("{0} ({1})", subscriptionBoxMamagementName, subscriptionBoxCourseId));
		}

		if ((cart.CanUsePointForPurchase == false)
			&& (cart.UsePoint > 0))
		{
			dvMessagePointMinimum.Visible = true;
			lMessagePointMinimum.Text = GetPointMinimumPurchasePriceErrorMessage(cart.SettlementCurrency);
		}
	}

	/// <summary>
	/// Get fixed purchase pattern title
	/// </summary>
	/// <param name="fixedPurchaseKbn">Fixed purchase kbn</param>
	/// <returns>Fixed purchase pattern title</returns>
	private string GetFixedPurchasePatternTitle(string fixedPurchaseKbn)
	{
		var result = ValueText.GetValueText(
			Constants.TABLE_FIXEDPURCHASE,
			Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN,
			fixedPurchaseKbn);
		return result;
	}

	/// <summary>
	/// Get Fixed Purchase Id
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>Fixed Purchase Id</returns>
	protected string GetFixedPurchaseId(CartObject cart)
	{
		var result = (cart.FixedPurchase != null)
			? cart.FixedPurchase.FixedPurchaseId
			: string.Empty;
		return result;
	}

	/// <summary>
	/// 購入商品を過去に購入したことがあるか（類似配送先含む）
	/// </summary>
	protected void CheckProductOrderLimitSimilarShipping()
	{
		((CartObject)this.OrderParams["cart"]).CheckProductOrderLimit();
		tableNotProductOrderLimitErrorMessages.Visible = this.HasOrderHistorySimilarShipping;
	}

	/// <summary>
	/// 戻るボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnBack_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_PARAM] = null;
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = this.OrderParams;
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_REGIST_INPUT)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegist_Click(object sender, EventArgs e)
	{
		// パラメタ取得
		var orderInput = (Hashtable)this.OrderParams["order_input"];
		var userInput = (Hashtable)this.OrderParams["user_input"];
		var cart = (CartObject)this.OrderParams["cart"];

		// 商品チェック
		var errorMessage = CheckOrderItemsAndPrices(cart);
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage
				+ WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_BACK_TO_REGIST_PAGE);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 注文実行（決済失敗でエラー画面へ）
		var orderId = ExecOrder(orderInput, userInput, cart);

		// 登録完了後、Configに設定した画面に遷移
		Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_REGIST_INPUT] = null;
		if (Constants.ORDER_REGISTER_COMPLETE_PAGE == Constants.OrderRegisterCompletePageType.OrderList)
		{
			var orderListUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_LIST)
				.AddParam(Constants.REQUEST_KEY_FIRSTVIEW, "1")
				.CreateUrl();
			Response.Redirect(orderListUrl);
		}
		var orderDetailUrl = CreateOrderDetailUrl(orderId, false, false);
		Response.Redirect(orderDetailUrl);
	}

	/// <summary>
	/// 注文実行（決済失敗でエラー画面へ）
	/// </summary>
	/// <param name="orderInput">注文入力</param>
	/// <param name="userInput">ユーザー</param>
	/// <param name="cart">カート</param>
	/// <returns>注文ID</returns>
	private string ExecOrder(Hashtable orderInput, Hashtable userInput, CartObject cart)
	{
		// 注文情報作成
		var registUser = string.IsNullOrEmpty((string)orderInput[Constants.FIELD_ORDER_USER_ID]);
		var userId = (registUser == false)
			? StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_USER_ID])
			: UserService.CreateNewUserId(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.NUMBER_KEY_USER_ID,
				Constants.CONST_USER_ID_HEADER,
				Constants.CONST_USER_ID_LENGTH);

		// 注文同梱の場合、親注文が注文同梱不可の場合処理中止
		if (this.IsOrderCombined)
		{
			foreach (var combinedOrderId in cart.OrderCombineParentOrderId.Split(','))
			{
				var errMsg = OrderCombineUtility.IsPossibleToOrderCombine(combinedOrderId, false);
				if (string.IsNullOrEmpty(errMsg) == false)
				{
					// エラー画面へ遷移
					Session[Constants.SESSION_KEY_ERROR_MSG] = errMsg;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
			}
			cart.OrderCombineParentTranId = this.CombineParentTranId;
			cart.IsOrderSalesSettled = this.IsCombineOrderSalesSettled;
		}

		// 元注文の定期情報(定期情報を持っているのは元注文のみと想定)
		var fixedPurchaseId = (this.IsOrderCombined && (cart.FixedPurchase != null))
			? cart.FixedPurchase.FixedPurchaseId
			: string.Empty;

		var userService = DomainFacade.Instance.UserService;
		var userInfo = userService.Get(userId);
		var advCodeNew = (string)orderInput[Constants.FIELD_ORDER_ADVCODE_NEW];
		var advCodeFirst = ((userInfo != null)
			&& (string.IsNullOrEmpty(userInfo.AdvcodeFirst) == false))
				? userInfo.AdvcodeFirst
				: advCodeNew;

		var excludeOrderIds = this.IsOrderCombined
			? cart.OrderCombineParentOrderId.Split(',')
			: null;
		var orderId = string.Empty;
		using (var productBundler = new ProductBundler(
			new List<CartObject> { cart },
			userId,
			advCodeFirst,
			advCodeNew,
			excludeOrderIds))
		{
			var bundledCart = productBundler.CartList.First();
			if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
				&& OrderCommon.CheckValidWeightAndPriceForConvenienceStore(bundledCart, bundledCart.Shippings[0].ShippingReceivingStoreType)
				&& (bundledCart.Shippings.Any(shipping => (shipping.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))))
			{
				var convenienceStoreLimitKg = OrderCommon.GetConvenienceStoreLimitWeight(bundledCart.Shippings[0].ShippingReceivingStoreType);
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHIPPING_CONVENIENCE_STORE)
					.Replace("@@ 1 @@", CurrencyManager.ToPrice(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE))
					.Replace("@@ 2 @@", convenienceStoreLimitKg.ToString());
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
			}

			// 注文情報作成
			var order = CreateOrderInfo(
				bundledCart,
				OrderCommon.CreateOrderId(Constants.CONST_DEFAULT_SHOP_ID),
				userId,
				orderInput,
				userInput,
				advCodeFirst,
				advCodeNew);

			// 管理画面特有の値を設定
			order[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO] =
				orderInput[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO];
			order["update_user_flg"] = this.CanUpdateUser;

			// リアルタイム累計購入回数取得
			var orderCount = ((registUser == false) && (userInfo != null))
				? userInfo.OrderCountOrderRealtime
				: 0;
			order.Add(Constants.FIELD_USER_ORDER_COUNT_ORDER_REALTIME, orderCount);
			order[Constants.FIELD_ORDER_ORDER_SUBSCRIPTION_BOX_ORDER_COUNT] =
				string.IsNullOrEmpty(cart.SubscriptionBoxCourseId) ? 0 : 1;

			// 注文実行
			var isUser = UserService.IsUser(bundledCart.Owner.OwnerKbn);
			if (isUser)
			{
				bundledCart.UpdateCartUserId(userId, registUser);
			}
			var register = new OrderRegisterManager(isUser, this.LoginOperatorName);

			var result = new OrderRegisterBase.ResultTypes();

			var disablePaymentCancelOrderId = string.Empty;
			var preCancelOrderIds = new List<string>();
			var combinedReauthOrders = new List<OrderModel>();
			var isReauth = OrderCommon.IsAtodeneReauthByCombine(
				order,
				excludeOrderIds,
				this.IsOrderCombined,
				out combinedReauthOrders);
			if (isReauth)
			{
				// 注文同梱で同梱されるものにAtodeneがあり、なおかつ新しい受注もAtodeneの場合、再与信を行う
				var maxAmountCombinedOrder = combinedReauthOrders
					.OrderByDescending(o => o.LastBilledAmount)
					.FirstOrDefault();

				// 決済上限額を超えないために、再与信をしないAtodeneの与信は事前にキャンセルする
				var cancelForReauthOrderIds = CancelPreReauthByNewOrder(
					combinedReauthOrders,
					order,
					cart,
					bundledCart.Coupon);
				preCancelOrderIds.AddRange(cancelForReauthOrderIds);

				result = register.ExecReauthAndNewOrder(
					maxAmountCombinedOrder,
					order,
					bundledCart,
					registUser,
					true,
					true,
					false,
					out disablePaymentCancelOrderId);
			}
			else
			{
				result = register.Exec(order, bundledCart, registUser, true);
			}

			// 注文同梱で親子共に定期購入があるもしくは親のみ定期購入がある場合、定期台帳の更新を行う
			if ((result == OrderRegisterBase.ResultTypes.Success)
				&& (bundledCart.IsCombineParentOrderHasFixedPurchase))
			{
				// 注文同梱画面からの注文同梱、または異なるコースでの頒布会同梱の場合
				// 定期台帳の商品の更新は行わない
				result = (bool)this.OrderParams["is_order_combine_page"]
					|| (cart.HasSubscriptionBox
						&& (cart.IsOrderCombinedWithSameSubscriptionBoxCourse() == false))
							? FixedPurchaseCombineUtility.UpdateFixedPurchaseForOrderCombine(
								(string)this.OrderParams["combine_parent_order_id"],
								cart.OrderCombineParentOrderId,
								userId,
								this.LoginOperatorName,
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								UpdateHistoryAction.Insert)
							: FixedPurchaseCombineUtility.UpdateFixedPurchaseAndAddItemForOrderCombine(
								cart.OrderCombineParentOrderId,
								userId,
								this.LoginOperatorName,
								this.OrderCombineBeforeCart,
								(string)order[Constants.FIELD_ORDER_ORDER_ID],
								UpdateHistoryAction.Insert);
			}

			if ((result == OrderRegisterBase.ResultTypes.Success)
				&& (this.IsOrderCombined))
			{
				// 親注文の拡張ステータスの引き継ぎ
				OrderCombineUtility.UpdateExtendStatusFromParentOrderWithCommit(
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					this.CombineParentOrderId,
					this.LoginOperatorName,
					UpdateHistoryAction.DoNotInsert);

				// 注文同梱後の注文の拡張ステータス同梱フラグ更新（更新履歴とともに）
				OrderCombineUtility.UpdateManagerOrderCombineFlg(
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					true,
					this.LoginOperatorName,
					UpdateHistoryAction.Insert);

				// 注文同梱済み注文をキャンセル（更新履歴とともに）
				var cancelOrderIds = cart.OrderCombineParentOrderId.Split(',').Except(preCancelOrderIds);
				var errMessage = OrderCombineUtility.OrdersCancelForOrderCombineWithCommit(
					this.LoginOperatorShopId,
					cancelOrderIds.ToArray(),
					(string)orderInput[Constants.FIELD_ORDER_USER_ID],
					false,
					this.LoginOperatorName,
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					UpdateHistoryAction.Insert,
					bundledCart.Coupon,
					disablePaymentCancelOrderId);

				if (string.IsNullOrEmpty(errMessage) == false)
				{
					OrderCombineUtility.SendOrderCombineParentOrderCancelErrorMail(
						this.LoginOperatorId,
						cart.OrderCombineParentOrderId,
						StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]),
						errMessage);

					// 外部決済連携ログ格納処理
					OrderCommon.AppendExternalPaymentCooperationLog(
						false,
						(string)order[Constants.FIELD_ORDER_ORDER_ID],
						errMessage,
						this.LoginOperatorName,
						UpdateHistoryAction.Insert);

					Session[Constants.SESSION_KEY_ERROR_MSG] = errMessage;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}

				// 子注文に定期台帳がある場合定期台帳の注文回数がキャンセル分マイナスされたままとなるため補正する
				OrderCombineUtility.CorrectFixedPurchaseOrderCount(
					(string)this.OrderParams["combine_parent_order_id"],
					cart.OrderCombineParentOrderId.Split(','),
					this.LoginOperatorName,
					UpdateHistoryAction.Insert);

				UploadNextEngineForOrderCombine(order, cart);
			}

			switch (result)
			{
				case OrderRegisterBase.ResultTypes.Success:
					orderId = (string)order[Constants.FIELD_ORDER_ORDER_ID];
					break;

				case OrderRegisterBase.ResultTypes.Fail:
					// エラー画面へ遷移
					Session[Constants.SESSION_KEY_ERROR_MSG] =
						WebSanitizer.HtmlEncodeChangeToBr(string.Join("\r\n", register.ErrorMessages));
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
					break;

				case OrderRegisterBase.ResultTypes.Skip:
					throw new Exception("注文登録機能は外部決済に対応していません。");
			}

			// 注文完了通知メール(管理者向け)送信
			// NOTE: 他のオプション利用時にEC管理画面からの新規注文登録で注文完了通知メールを送信したい場合はifの条件にORで追記してください
			if (Constants.NE_OPTION_ENABLED
				|| Constants.SEND_ORDER_COMPLETE_EMAIL_FOR_OPERATOR_ENABLED_LIST.Contains(Constants.EnabledOrderCompleteEmailSenderType.Manager.ToString()))
			{
				try
				{
					register.SendOrderMailToOperator(order, cart, isUser);
				}
				catch(Exception ex)
				{
					FileLogger.WriteError(ex.ToString());
					var mailTemplateName = new MailTemplateService()
						.Get(
							LoginOperatorShopId, 
							Constants.CONST_MAIL_ID_ORDER_COMPLETE_FOR_OPERATOR)
						.MailName;
					this.SendMailErrorMessage = WebMessages
						.GetMessages(WebMessages.ERRMSG_MANAGER_ERROR_SEND_EMAIL_BY_MAIL_TEMPLATE)
						.Replace("@@ 1 @@", mailTemplateName)
						.Replace("@@ 2 @@", Constants.CONST_MAIL_ID_ORDER_COMPLETE_FOR_OPERATOR);
					
					// ネクストエンジンを利用している場合は手動でメールを送信するように促すメッセージを表示
					if (Constants.NE_OPTION_ENABLED)
					{
						this.SendMailErrorMessage += WebMessages.GetMessages(
							WebMessages.ERRMSG_MANAGER_ERROR_NEXT_ENGINE_TO_MAIL_FORM);
					}
				}
			}
		}

		// オフライン会員の新規登録であればデフォルト会員ランク付与
		if (registUser
			&& (cart.Owner.OwnerKbn == Constants.FLG_ORDEROWNER_OWNER_KBN_OFFLINE_USER))
		{
			// ユーザー情報を取得する
			var user = userService.Get(userId);

			user.MemberRankId = MemberRankOptionUtility.GetDefaultMemberRank();
			user.LastChanged = this.LoginOperatorName;

			// 更新（更新履歴とともに）
			userService.UpdateWithUserExtend(user, UpdateHistoryAction.Insert);
		}
		return orderId;
	}

	/// <summary>
	/// 注文情報作成
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="orderId">注文ID</param>
	/// <param name="userId">ユーザーID</param>
	/// <param name="orderInput">注文入力情報</param>
	/// <param name="userInput">ユーザー入力情報</param>
	/// <param name="advCodeFirst">初回広告コード</param>
	/// <param name="advCodeNew">最新広告コード</param>
	/// <returns>注文情報</returns>
	private Hashtable CreateOrderInfo(
		CartObject cart,
		string orderId,
		string userId,
		Hashtable orderInput,
		Hashtable userInput,
		string advCodeFirst,
		string advCodeNew)
	{
		// 注文情報取得
		var order = OrderCommon.CreateOrderInfo(
			cart,
			orderId,
			userId,
			string.Empty,
			string.Empty,
			Request.ServerVariables["REMOTE_ADDR"],
			advCodeFirst,
			advCodeNew,
			this.LoginOperatorName);

		var managementMemo = OrderCommon.GetNotFirstTimeFixedPurchaseManagementMemo(
			cart.ManagementMemo,
			cart.ProductOrderLmitOrderIds,
			false);

		// 管理画面独自の値をセット
		order[Constants.FIELD_ORDER_PAYMENT_MEMO] = orderInput[Constants.FIELD_ORDER_PAYMENT_MEMO];
		order[Constants.FIELD_ORDER_MANAGEMENT_MEMO] =
			(string.IsNullOrEmpty((string)orderInput[Constants.FIELD_ORDER_MANAGEMENT_MEMO]))
				? managementMemo
				: orderInput[Constants.FIELD_ORDER_MANAGEMENT_MEMO] +
					(string.IsNullOrEmpty(managementMemo)
						? string.Empty
						: System.Environment.NewLine + managementMemo);
		order[Constants.FIELD_ORDER_SHIPPING_MEMO] = orderInput[Constants.FIELD_ORDER_SHIPPING_MEMO];
		order[Constants.FIELD_ORDER_RELATION_MEMO] = orderInput[Constants.FIELD_ORDER_RELATION_MEMO];
		order[Constants.FIELD_USER_USER_MEMO] = userInput[Constants.FIELD_USER_USER_MEMO];
		order[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID]
			= userInput[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID];
		order[Constants.FIELD_USER_MAIL_FLG] = userInput[Constants.FIELD_USER_MAIL_FLG];
		
		// Gmo cvs type
		order[Constants.PAYMENT_GMO_CVS_TYPE] = StringUtility.ToEmpty(orderInput[Constants.PAYMENT_GMO_CVS_TYPE]);

		// 決済金額をセット
		order[Constants.FIELD_ORDER_SETTLEMENT_CURRENCY] = cart.SettlementCurrency;
		order[Constants.FIELD_ORDER_SETTLEMENT_RATE] = cart.SettlementRate;
		order[Constants.FIELD_ORDER_SETTLEMENT_AMOUNT] = cart.SettlementAmount;
		order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE] = orderInput[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE];
		// コンバージョン情報をセット
		if (cart.ContentsLog != null)
		{
			order[Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE] = cart.ContentsLog.ContentsType;
			order[Constants.FIELD_ORDER_INFLOW_CONTENTS_ID] = cart.ContentsLog.ContentsId;
		}

		return order;
	}

	/// <summary>
	/// 再与信のために事前に与信をキャンセルする
	/// </summary>
	/// <param name="combinedReauthOrders">注文同梱する再与信対象の受注</param>
	/// <param name="order">新しい受注</param>
	/// <param name="cart">カート</param>
	/// <param name="orderCombineCoupon">注文同梱で生成された新注文が利用するクーポン</param>
	/// <returns>キャンセル済みの受注ID</returns>
	private List<string> CancelPreReauthByNewOrder(
		List<OrderModel> combinedReauthOrders,
		Hashtable order,
		CartObject cart,
		CartCoupon orderCombineCoupon)
	{
		var maxAmountCombinedOrder = combinedReauthOrders.OrderByDescending(o => o.LastBilledAmount).FirstOrDefault();
		// 事前にキャンセルするAtodeneの受注IDを取得
		var notReauthAtodeneOrderIds = combinedReauthOrders
			.Where(o => o.OrderId != maxAmountCombinedOrder.OrderId)
			.Select(o => o.OrderId).ToArray();

		// 注文同梱済み注文をキャンセル（更新履歴とともに）
		var errorMessage = OrderCombineUtility.OrdersCancelForOrderCombineWithCommit(
			this.LoginOperatorShopId,
			notReauthAtodeneOrderIds,
			(string)order[Constants.FIELD_ORDER_USER_ID],
			false,
			this.LoginOperatorName,
			(string)order[Constants.FIELD_ORDER_ORDER_ID],
			UpdateHistoryAction.Insert,
			orderCombineCoupon,
			string.Empty);
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			OrderCombineUtility.SendOrderCombineParentOrderCancelErrorMail(
				this.LoginOperatorId,
				cart.OrderCombineParentOrderId,
				(string)order[Constants.FIELD_ORDER_ORDER_ID],
				errorMessage);

			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		return notReauthAtodeneOrderIds.ToList();
	}

	/// <summary>
	/// Get Shipping Time Message
	/// </summary>
	/// <param name="shippingCompanyId">Shipping Company Id</param>
	/// <param name="shippingTimeId">Shipping Time Id</param>
	/// <returns>Shipping Time Message</returns>
	protected string GetShippingTimeMessage(string shippingCompanyId, string shippingTimeId)
	{
		var shippingCompany = DomainFacade.Instance.DeliveryCompanyService.Get(shippingCompanyId);
		if (shippingCompany == null) return string.Empty;

		var timeMessage = shippingCompany.GetShippingTimeMessage(shippingTimeId);
		var result = (string.IsNullOrEmpty(timeMessage) == false)
			? timeMessage
			: ReplaceTag("@@DispText.shipping_time_list.none@@");
		return result;
	}

	/// <summary>
	/// Create product joint name html
	/// </summary>
	/// <param name="product">Product</param>
	/// <returns>Product joint name</returns>
	protected string CreateProductJointNameHtml(CartProduct product)
	{
		var productChanges = string.Empty;
		if (Constants.ORDER_COMBINE_OPTION_ENABLED)
		{
			productChanges = OrderCombineUtility.GetCartProductChangesByOrderCombine(product);
		}
		var productJointName = (string.IsNullOrEmpty(productChanges))
			? GetEncodedHtmlDisplayMessage(product.ProductJointName)
			: string.Format("{0}<br />{1}",
				GetEncodedHtmlDisplayMessage(productChanges),
				GetEncodedHtmlDisplayMessage(product.ProductJointName));
		return productJointName;
	}

	/// <summary>
	/// 配送方法アラート表示するか
	/// </summary>
	/// <param name="orderInput">受注情報</param>
	/// <param name="cart">カート情報</param>
	/// <returns>Trueであれば表示</returns>
	private bool CanShowOrderShippingAlertMessage(Hashtable orderInput, CartObject cart)
	{
		string userId = null;
		var advCodeNew = (string)orderInput[Constants.FIELD_ORDER_ADVCODE_NEW];
		var advCodeFirst = advCodeNew;
		if (string.IsNullOrEmpty((string)orderInput[Constants.FIELD_ORDER_USER_ID]) == false)
		{
			userId = (string)orderInput[Constants.FIELD_ORDER_USER_ID];
			var user = new UserService().Get(userId);
			advCodeFirst = ((user != null) && (string.IsNullOrEmpty(user.AdvcodeFirst) == false))
				? user.AdvcodeFirst
				: advCodeNew;
		}

		// 商品同梱
		var excludeOrderIds = this.IsOrderCombined
			? cart.OrderCombineParentOrderId.Split(',')
			: null;
		using (var productBundler = new ProductBundler(
					new List<CartObject> { cart },
					userId,
					advCodeFirst,
					advCodeNew,
					excludeOrderIds))
		{
			var bundledCart = productBundler.CartList.First();
			// 商品同梱後のカートを対象に再計算を行う
			bundledCart.CalculatePointFamily();

			var beforeShippingMethod = bundledCart.GetShipping().ShippingMethod;
			var afterShippingMethod = OrderCommon.GetShippingMethod(bundledCart.Items);
			var result = ((beforeShippingMethod != afterShippingMethod)
				&& (beforeShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL));
			return result;
		}
	}

	/// <summary>
	/// メール便配送サービスアラート表示するか
	/// </summary>
	/// <param name="orderInput">受注情報</param>
	/// <param name="cart">カート情報</param>
	/// <returns>Trueであれば表示</returns>
	private bool CanShowOrderDeliveryCompanyAlertMessage(Hashtable orderInput, CartObject cart)
	{
		if (cart.GetShipping().IsMail == false) return false;
		string userId = null;
		var advCodeNew = (string)orderInput[Constants.FIELD_ORDER_ADVCODE_NEW];
		var advCodeFirst = advCodeNew;
		if (string.IsNullOrEmpty((string)orderInput[Constants.FIELD_ORDER_USER_ID]) == false)
		{
			userId = (string)orderInput[Constants.FIELD_ORDER_USER_ID];
			var user = new UserService().Get(userId);
			advCodeFirst = ((user != null) && (string.IsNullOrEmpty(user.AdvcodeFirst) == false))
				? user.AdvcodeFirst
				: advCodeNew;
		}

		var deliveryCompanyList = OrderCommon.GetDeliveryCompanyList(
			new ShopShippingService().Get(cart.ShopId, cart.ShippingType).CompanyListMail);

		// 商品同梱
		var excludeOrderIds = this.IsOrderCombined
			? cart.OrderCombineParentOrderId.Split(',')
			: null;
		using (var productBundler = new ProductBundler(
					new List<CartObject> { cart },
					userId,
					advCodeFirst,
					advCodeNew,
					excludeOrderIds))
		{
			var bundledCart = productBundler.CartList.First();
			var totalItemSize = cart.Items.Sum(item => item.ProductSizeFactor * item.Count);
			var sizeLimit = deliveryCompanyList.FirstOrDefault(
				company => company.DeliveryCompanyId == bundledCart.GetShipping().DeliveryCompanyId);
			var result = ((sizeLimit == null) || (totalItemSize > sizeLimit.DeliveryCompanyMailSizeLimit));
			return result;
		}
	}

	/// <summary>
	/// Upload Next Engine for order combine
	/// </summary>
	/// <param name="order">Order</param>
	/// <param name="cart">Cart</param>
	protected void UploadNextEngineForOrderCombine(Hashtable order, CartObject cart)
	{
		if ((this.IsOrderCombined == false)
			|| (Constants.NE_OPTION_ENABLED == false)
			|| ((Constants.NE_COOPERATION_ORDERCOMBINE_ENABLED == false)
				&& (Constants.NE_COOPERATION_CANCEL_ENABLED == false)))
		{
			return;
		}

		var isCancelSuccess = true;
		foreach (var cancelOrderId in cart.OrderCombineParentOrderId.Split(',').Distinct())
		{
			if (OrderCommon.UpdateNextEngineOrderForCancel(cancelOrderId, null).Item3) continue;

			NextEngineApi.SendFailureCancelOrderMail(cancelOrderId, cart.CartUserId);
			isCancelSuccess = false;
		}

		if (isCancelSuccess)
		{
			var input = new MailTemplateDataCreaterByCartAndOrder(false)
				.GetOrderMailDatas(order, cart, true);
			input[Constants.FIELD_ORDER_MEMO] = ((string)input[Constants.FIELD_ORDER_MEMO]).Trim();

			using (var mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_NEXT_ENGINE_ORDER_COMBINE_COMPLETE_FOR_MANAGER,
				cart.CartUserId,
				input,
				true,
				Constants.MailSendMethod.Auto,
				cart.Owner.DispLanguageCode,
				cart.Owner.DispLanguageLocaleId))
			{
				if (mailSender.SendMail() == false)
				{
					AppLogger.WriteError(
						string.Format(
							"{0} : {1}",
							this.GetType().BaseType,
							mailSender.MailSendException));
				}
			}
		}
	}

	/// <summary>注文情報パラメタ</summary>
	protected Hashtable OrderParams
	{
		get { return (Hashtable)Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_REGIST_INPUT]; }
	}
	/// <summary>ユーザー判定(ポイント利用判定）</summary>
	protected bool IsUser
	{
		get { return UserService.IsUser(((CartObject)this.OrderParams["cart"]).Owner.OwnerKbn); }
	}
	/// <summary>ユーザ情報更新判断 </summary>
	protected bool CanUpdateUser
	{
		get { return (bool)this.OrderParams["update_user_flg"]; }
	}
	/// <summary>Combine Parent Order Id</summary>
	protected string CombineParentOrderId
	{
		get { return StringUtility.ToEmpty(this.OrderParams["combine_parent_order_id"]); }
		set { ViewState["combine_parent_order_id"] = value; }
	}
	/// <summary>注文同梱有無</summary>
	protected bool IsOrderCombined
	{
		get { return (bool)(this.OrderParams["is_order_combined"] ?? false); }
		set { ViewState["is_order_combined"] = value; }
	}
	/// <summary>注文同梱対象 注文ID</summary>
	protected CartObject OrderCombineBeforeCart
	{
		get { return (CartObject)this.OrderParams["order_combine_before_cart"]; }
		set { this.OrderParams["order_combine_before_cart"] = value; }
	}
	/// <summary>Combine Parent Tran Id</summary>
	protected string CombineParentTranId
	{
		get { return (string)this.OrderParams["combine_parent_tran_id"]; }
		set { this.OrderParams["combine_parent_tran_id"] = value; }
	}
	/// <summary>Is Combine Order Sales Settled</summary>
	protected bool IsCombineOrderSalesSettled
	{
		get { return (bool)(this.OrderParams["is_combine_order_sales_settled"] ?? false); }
		set { this.OrderParams["is_combine_order_sales_settled"] = value; }
	}
	/// <summary>過去に定期購入の履歴があるか（類似配送先含む）</summary>
	protected bool HasOrderHistorySimilarShipping
	{
		get
		{
			var cart = (CartObject)this.OrderParams["cart"];
			return ((cart.ProductOrderLmitOrderIds.Length > 0) || cart.IsCompliantOrderLimitProduct);
		}
	}
	/// <summary>注文者は日本の住所か</summary>
	protected bool IsOwnerAddrJp
	{
		get { return IsCountryJp(this.OwnerAddrCountryIsoCode); }
	}
	/// <summary>配送先は日本の住所か</summary>
	protected bool IsOwnerAddrUs
	{
		get { return IsCountryUs(this.OwnerAddrCountryIsoCode); }
	}
	/// <summary>注文者住所国ISOコード</summary>
	protected bool IsOwnerAddrZipNecessary
	{
		get { return IsAddrZipcodeNecessary(this.OwnerAddrCountryIsoCode); }
	}
	/// <summary>配送先は日本の住所か</summary>
	protected bool IsShippingAddrJp
	{
		get { return IsCountryJp(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>Is Shipping Address In The United States</summary>
	protected bool IsShippingAddrUs
	{
		get { return IsCountryUs(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>注文者住所国ISOコード</summary>
	protected string OwnerAddrCountryIsoCode
	{
		get { return ((CartObject)this.OrderParams["cart"]).Owner.AddrCountryIsoCode; }
	}
	/// <summary>配送先住所国ISOコード</summary>
	protected string ShippingAddrCountryIsoCode
	{
		get { return ((CartObject)this.OrderParams["cart"]).GetShipping().ShippingCountryIsoCode; }
	}
	/// <summary>新規クレジットカード利用するか</summary>
	public bool UseNewCreditCard
	{
		get { return (bool)(ViewState["UseNewCreditCard"] ?? false); }
		set { ViewState["UseNewCreditCard"] = value; }
	}
	/// <summary>Is shipping address in Taiwan</summary>
	protected bool IsShippingAddrTw
	{
		get { return IsCountryTw(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>Order Status For Combine</summary>
	protected string OrderStatusForCombine { get; set; }
	/// <summary>Order Created Date For Combine</summary>
	protected DateTime OrderCreatedDateForCombine { get; set; }
	/// <summary>Order Payment Status For Combine</summary>
	protected string OrderPaymentStatusForCombine { get; set; }
	/// <summary>Order Item For Combine</summary>
	protected OrderItemModel[] OrderItemForCombine { get; set; }
	/// <summary>Order Price For Combine</summary>
	protected decimal OrderPriceForCombine { get; set; }
	/// <summary>Order Coupon For Combine</summary>
	protected string OrderCouponForCombine { get; set; }
	/// <summary>頒布会定額コースか</summary>
	protected bool IsSubscriptionBoxFixedAmount
	{
		get { return ((CartObject)this.OrderParams["cart"]).IsSubscriptionBoxFixedAmount; }
	}
	/// <summary>1種類の頒布会定額コースの商品のみが含まれるか</summary>
	protected bool HaveOnlyOneSubscriptionBoxFixedAmountCourseItem
	{
		get
		{
			var result = ((CartObject)this.OrderParams["cart"]).HaveOnlyOneSubscriptionBoxFixedAmountCourseItem();
			return result;
		}
	}
	/// <summary>商品が全て頒布会定額コース商品か</summary>
	protected bool IsAllItemsSubscriptionBoxFixedAmount
	{
		get { return ((CartObject)this.OrderParams["cart"]).IsAllItemsSubscriptionBoxFixedAmount; }
	}
}
