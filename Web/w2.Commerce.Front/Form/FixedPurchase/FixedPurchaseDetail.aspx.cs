/*
=========================================================================================================
  Module      : 定期購入情報詳細画面処理(FixedPurchaseDetail.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Amazon.Pay.API.WebStore.CheckoutSession;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UserCreditCard;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.Amazon.Util;
using w2.App.Common.AmazonCv2;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Mail;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Boku;
using w2.App.Common.Order.Payment.Boku.Utils;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Payment.Paidy;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Register;
using w2.App.Common.OrderExtend;
using w2.App.Common.Product;
using w2.App.Common.SendMail;
using w2.App.Common.SubscriptionBox;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.CountryLocation;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Holiday.Helper;
using w2.Domain.Product;
using w2.Domain.ShopShipping;
using w2.Domain.TwFixedPurchaseInvoice;
using w2.Domain.TwUserInvoice;
using w2.Domain.UserShipping;
using w2.Domain.SubscriptionBox;
using w2.Domain.User;
using w2.Common.Extensions;
using w2.Domain.FixedPurchaseProductChangeSetting;

public partial class Form_FixedPurchase_FixedPurchaseDetail : FixedPurchasePage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	# region ラップ済みコントロール宣言

	WrappedLabel WlbOrderNowMesssage { get { return GetWrappedControl<WrappedLabel>("lbOrderNowMessages"); } }
	WrappedLiteral WlPointResetMessages { get { return GetWrappedControl<WrappedLiteral>("lPointResetMessages"); } }
	WrappedCheckBox WcbUpdateNextShippingDate { get { return GetWrappedControl<WrappedCheckBox>("cbUpdateNextShippingDate", false); } }
	protected WrappedCheckBox WcbUpdateNextNextShippingDate
	{
		get
		{
			return GetWrappedControl<WrappedCheckBox>(
				"cbUpdateNextNextShippingDate",
				Constants.FIXEDPURCHASEORDERNOW_NEXT_NEXT_SHIPPING_DATE_UPDATE_DEFAULT);
		}
	}
	WrappedRepeater WrOrderSuccess { get { return GetWrappedControl<WrappedRepeater>("rOrderSuccess"); } }
	WrappedHtmlGenericControl WspFixedPurchaseStatus { get { return GetWrappedControl<WrappedHtmlGenericControl>("spFixedPurchaseStatus"); } }
	WrappedHtmlGenericControl WspPaymentStatus { get { return GetWrappedControl<WrappedHtmlGenericControl>("spPaymentStatus"); } }
	WrappedHtmlGenericControl WdvNextShippingUsePoint { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvNextShippingUsePoint"); } }
	WrappedCheckBox WcbUseAllPointFlg { get { return GetWrappedControl<WrappedCheckBox>("cbUseAllPointFlg"); } }
	WrappedHtmlGenericControl WdvFixedPurchaseDetailCard { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvFixedPurchaseDetailCard"); } }
	WrappedHtmlGenericControl WdvFixedPurchaseCurrentCard { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvFixedPurchaseCurrentCard"); } }
	WrappedRepeater WrItem { get { return GetWrappedControl<WrappedRepeater>("rItem"); } }
	WrappedHtmlGenericControl WdvFixedPurchaseDetailShippingPattern { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvFixedPurchaseDetailShippingPattern"); } }
	WrappedDropDownList WddlFixedPurchaseIntervalMonths { get { return GetWrappedControl<WrappedDropDownList>("ddlFixedPurchaseIntervalMonths"); } }
	WrappedDropDownList WddlFixedPurchaseWeekOfMonth { get { return GetWrappedControl<WrappedDropDownList>("ddlFixedPurchaseWeekOfMonth"); } }
	WrappedDropDownList WddlFixedPurchaseDayOfWeek { get { return GetWrappedControl<WrappedDropDownList>("ddlFixedPurchaseDayOfWeek"); } }
	WrappedDropDownList WddlFixedPurchaseMonth { get { return GetWrappedControl<WrappedDropDownList>("ddlFixedPurchaseMonth"); } }
	WrappedDropDownList WddlFixedPurchaseMonthlyDate { get { return GetWrappedControl<WrappedDropDownList>("ddlFixedPurchaseMonthlyDate"); } }
	WrappedDropDownList WddlFixedPurchaseIntervalDays { get { return GetWrappedControl<WrappedDropDownList>("ddlFixedPurchaseIntervalDays"); } }
	WrappedDropDownList WddlFixedPurchaseEveryNWeek_Week { get { return GetWrappedControl<WrappedDropDownList>("ddlFixedPurchaseEveryNWeek_Week"); } }
	WrappedDropDownList WddlFixedPurchaseEveryNWeek_DayOfWeek { get { return GetWrappedControl<WrappedDropDownList>("ddlFixedPurchaseEveryNWeek_DayOfWeek"); } }
	WrappedDropDownList WddlResumeFixedPurchaseDate { get { return GetWrappedControl<WrappedDropDownList>("ddlResumeFixedPurchaseDate"); } }
	WrappedDropDownList WddlResumeSubscriptionBoxDate { get { return GetWrappedControl<WrappedDropDownList>("ddlResumeSubscriptionBoxDate"); } }
	WrappedHtmlGenericControl WdvResumeFixedPurchaseDate { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvResumeFixedPurchaseDate"); } }
	WrappedRepeater WrItemModify { get { return GetWrappedControl<WrappedRepeater>("rItemModify"); } }
	
	WrappedRadioButton WrbFixedPurchaseDays { get { return GetWrappedControl<WrappedRadioButton>("rbFixedPurchaseDays"); } }
	WrappedRadioButton WrbFixedPurchaseWeekAndDay { get { return GetWrappedControl<WrappedRadioButton>("rbFixedPurchaseWeekAndDay"); } }
	WrappedRadioButton WrbFixedPurchaseIntervalDays { get { return GetWrappedControl<WrappedRadioButton>("rbFixedPurchaseIntervalDays"); } }
	WrappedRadioButton WrbFixedPurchaseEveryNWeek { get { return GetWrappedControl<WrappedRadioButton>("rbFixedPurchaseEveryNWeek"); } }
	WrappedCheckBox WcbResumeFixedPurchase { get { return GetWrappedControl<WrappedCheckBox>("cbResumeFixedPurchase"); } }
	WrappedHtmlGenericControl WdtWeekAndDay { get { return GetWrappedControl<WrappedHtmlGenericControl>("dtWeekAndDay"); } }
	WrappedHtmlGenericControl WddWeekAndDay { get { return GetWrappedControl<WrappedHtmlGenericControl>("ddWeekAndDay"); } }
	WrappedHtmlGenericControl WdtMonthlyDate { get { return GetWrappedControl<WrappedHtmlGenericControl>("dtMonthlyDate"); } }
	WrappedHtmlGenericControl WddMonthlyDate { get { return GetWrappedControl<WrappedHtmlGenericControl>("ddMonthlyDate"); } }
	WrappedHtmlGenericControl WdtIntervalDays { get { return GetWrappedControl<WrappedHtmlGenericControl>("dtIntervalDays"); } }
	WrappedHtmlGenericControl WddIntervalDays { get { return GetWrappedControl<WrappedHtmlGenericControl>("ddIntervalDays"); } }
	WrappedHtmlGenericControl WdtFixedPurchaseEveryNWeek { get { return GetWrappedControl<WrappedHtmlGenericControl>("dtFixedPurchaseEveryNWeek"); } }
	WrappedHtmlGenericControl WddFixedPurchaseEveryNWeek { get { return GetWrappedControl<WrappedHtmlGenericControl>("ddFixedPurchaseEveryNWeek"); } }
	WrappedHtmlGenericControl WdvOrderPaymentPattern { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvOrderPaymentPattern"); } }
	WrappedHtmlGenericControl WdvResumeFixedPurchaseErr { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvResumeFixedPurchaseErr"); } }

	protected WrappedTextBox WtbNextShippingUsePoint { get { return GetWrappedControl<WrappedTextBox>("tbNextShippingUsePoint"); } }
	protected WrappedTextBox WtbShippingName1 { get { return GetWrappedControl<WrappedTextBox>("tbShippingName1"); } }
	protected WrappedTextBox WtbShippingName2 { get { return GetWrappedControl<WrappedTextBox>("tbShippingName2"); } }
	protected WrappedTextBox WtbShippingNameKana1 { get { return GetWrappedControl<WrappedTextBox>("tbShippingNameKana1"); } }
	protected WrappedTextBox WtbShippingNameKana2 { get { return GetWrappedControl<WrappedTextBox>("tbShippingNameKana2"); } }
	protected WrappedDropDownList WddlShippingCountry { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingCountry"); } }
	protected WrappedTextBox WtbShippingZipGlobal { get { return GetWrappedControl<WrappedTextBox>("tbShippingZipGlobal"); } }
	protected WrappedTextBox WtbShippingZip1 { get { return GetWrappedControl<WrappedTextBox>("tbShippingZip1"); } }
	protected WrappedTextBox WtbShippingZip2 { get { return GetWrappedControl<WrappedTextBox>("tbShippingZip2"); } }
	protected WrappedDropDownList WddlShippingAddr1 { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingAddr1"); } }
	protected WrappedTextBox WtbShippingAddr2 { get { return GetWrappedControl<WrappedTextBox>("tbShippingAddr2"); } }
	protected WrappedTextBox WtbShippingAddr3 { get { return GetWrappedControl<WrappedTextBox>("tbShippingAddr3"); } }
	protected WrappedTextBox WtbShippingAddr4 { get { return GetWrappedControl<WrappedTextBox>("tbShippingAddr4"); } }
	protected WrappedDropDownList WddlShippingAddr5 { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingAddr5"); } }
	protected WrappedTextBox WtbShippingAddr5 { get { return GetWrappedControl<WrappedTextBox>("tbShippingAddr5"); } }
	protected WrappedTextBox WtbShippingCompanyName { get { return GetWrappedControl<WrappedTextBox>("tbShippingCompanyName"); } }
	protected WrappedTextBox WtbShippingCompanyPostName { get { return GetWrappedControl<WrappedTextBox>("tbShippingCompanyPostName"); } }
	protected WrappedTextBox WtbShippingTel1 { get { return GetWrappedControl<WrappedTextBox>("tbShippingTel1"); } }
	protected WrappedTextBox WtbShippingTel2 { get { return GetWrappedControl<WrappedTextBox>("tbShippingTel2"); } }
	protected WrappedTextBox WtbShippingTel3 { get { return GetWrappedControl<WrappedTextBox>("tbShippingTel3"); } }
	protected WrappedTextBox WtbShippingTel1Global { get { return GetWrappedControl<WrappedTextBox>("tbShippingTel1Global"); } }
	protected WrappedDropDownList WddlShippingMethod { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingMethod"); } }
	protected WrappedDropDownList WddlDeliveryCompany { get { return GetWrappedControl<WrappedDropDownList>("ddlDeliveryCompany"); } }
	protected WrappedDropDownList WddlShippingTime { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingTime"); } }
	protected WrappedDropDownList WddlShippingMethodForAmazonPay { get { return GetWrappedControl<WrappedDropDownList>(this.AmazonPayRepeaterItem, "ddlShippingMethodForAmazonPay"); } }
	protected WrappedDropDownList WddlDeliveryCompanyForAmazonPay { get { return GetWrappedControl<WrappedDropDownList>(this.AmazonPayRepeaterItem, "ddlDeliveryCompanyForAmazonPay"); } }
	protected WrappedDropDownList WddlShippingTimeForAmazonPay { get { return GetWrappedControl<WrappedDropDownList>(this.AmazonPayRepeaterItem, "ddlShippingTimeForAmazonPay"); } }
	protected WrappedDropDownList WddlShippingTimeAmazonPayCv2 { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingTimeAmazonPayCv2"); } }
	protected WrappedLiteral WlResumeFixedPurchaseMessage { get { return GetWrappedControl<WrappedLiteral>("lResumeFixedPurchaseMessage"); } }
	protected WrappedHtmlGenericControl WhgcConstraintErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>(this.AmazonPayRepeaterItem, "constraintErrorMessage"); } }
	protected WrappedHiddenField WhfAmazonBillingAgreementId { get { return GetWrappedControl<WrappedHiddenField>(this.AmazonPayRepeaterItem, "hfAmazonBillingAgreementId"); } }

	protected WrappedDropDownList WddlNextShippingDate { get { return GetWrappedControl<WrappedDropDownList>("ddlNextShippingDate"); } }
	protected WrappedHtmlGenericControl WdvChangeNextShippingDate { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvChangeNextShippingDate"); } }
	protected WrappedLabel WlNextShippingDateErrorMessage { get { return GetWrappedControl<WrappedLabel>("lNextShippingDateErrorMessage"); } }
	protected WrappedDropDownList WddlShippingAddr2 { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingAddr2"); } }
	protected WrappedDropDownList WddlShippingAddr3 { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingAddr3"); } }

	protected WrappedHtmlGenericControl WdvNextShippingUseCoupon { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvNextShippingUseCoupon"); } }
	protected WrappedHtmlGenericControl WdvCouponCodeInputArea { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvCouponCodeInputArea"); } }
	protected WrappedHtmlGenericControl WdvCouponBox { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvCouponBox"); } }
	protected WrappedDropDownList WddlNextShippingUseCouponList { get { return GetWrappedControl<WrappedDropDownList>("ddlNextShippingUseCouponList"); } }
	protected WrappedTextBox WtbNextShippingUseCouponCode { get { return GetWrappedControl<WrappedTextBox>("tbNextShippingUseCouponCode"); } }
	protected WrappedRadioButtonList WrblCouponInputMethod { get { return GetWrappedControl<WrappedRadioButtonList>("rblCouponInputMethod"); } }
	protected WrappedDropDownList WddlReceiptFlg { get { return GetWrappedControl<WrappedDropDownList>("ddlReceiptFlg"); } }
	protected WrappedTextBox WtbReceiptAddress { get { return GetWrappedControl<WrappedTextBox>("tbReceiptAddress"); } }
	protected WrappedTextBox WtbReceiptProviso { get { return GetWrappedControl<WrappedTextBox>("tbReceiptProviso"); } }
	protected WrappedHtmlGenericControl WdvReceiptAddressProvisoInput { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvReceiptAddressProvisoInput"); } }
	protected WrappedHtmlGenericControl WtrReceiptAddressInput { get { return GetWrappedControl<WrappedHtmlGenericControl>("trReceiptAddressInput"); } }
	protected WrappedHtmlGenericControl WtrReceiptProvisoInput { get { return GetWrappedControl<WrappedHtmlGenericControl>("trReceiptProvisoInput"); } }
	protected WrappedHiddenField WhfPaymentNameSelected { get { return GetWrappedControl<WrappedHiddenField>("hfPaymentNameSelected"); } }
	protected WrappedHiddenField WhfPaymentIdSelected { get { return GetWrappedControl<WrappedHiddenField>("hfPaymentIdSelected"); } }

	protected WrappedDropDownList WddlTaiwanUniformInvoice { get { return GetWrappedControl<WrappedDropDownList>("ddlTaiwanUniformInvoice"); } }
	protected WrappedDropDownList WddlTaiwanUniformInvoiceOptionKbnList { get { return GetWrappedControl<WrappedDropDownList>("ddlTaiwanUniformInvoiceOptionKbnList"); } }
	protected WrappedDropDownList WddlTaiwanCarryType { get { return GetWrappedControl<WrappedDropDownList>("ddlTaiwanCarryType"); } }
	protected WrappedDropDownList WddlTaiwanCarryKbnList { get { return GetWrappedControl<WrappedDropDownList>("ddlTaiwanCarryKbnList"); } }

	protected WrappedHtmlGenericControl WtrInvoiceDispForPersonalType { get { return GetWrappedControl<WrappedHtmlGenericControl>("trInvoiceDispForPersonalType"); } }
	protected WrappedHtmlGenericControl WspTaiwanCarryKbnList { get { return GetWrappedControl<WrappedHtmlGenericControl>("spTaiwanCarryKbnList"); } }
	protected WrappedHtmlGenericControl WdvInvoiceDispForPersonalType { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvInvoiceDispForPersonalType"); } }
	protected WrappedLiteral WlInvoiceCode { get { return GetWrappedControl<WrappedLiteral>("lInvoiceCode"); } }
	protected WrappedHtmlGenericControl WdvInvoiceInputForPersonalType { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvInvoiceInputForPersonalType"); } }
	protected WrappedHtmlGenericControl WdvCarryTypeOption_8 { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvCarryTypeOption_8"); } }
	protected WrappedTextBox WtbCarryTypeOption_8 { get { return GetWrappedControl<WrappedTextBox>("tbCarryTypeOption_8"); } }
	protected WrappedHtmlGenericControl WdvCarryTypeOption_16 { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvCarryTypeOption_16"); } }
	protected WrappedTextBox WtbCarryTypeOption_16 { get { return GetWrappedControl<WrappedTextBox>("tbCarryTypeOption_16"); } }
	protected WrappedHtmlGenericControl WdvInvoiceNoEquipmentMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvInvoiceNoEquipmentMessage"); } }

	protected WrappedHtmlGenericControl WdvInvoiceForm { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvInvoiceForm"); } }
	protected WrappedHtmlGenericControl WdvInvoiceDisp { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvInvoiceDisp"); } }
	protected WrappedHtmlGenericControl WspTaiwanUniformInvoiceOptionKbnList { get { return GetWrappedControl<WrappedHtmlGenericControl>("spTaiwanUniformInvoiceOptionKbnList"); } }
	protected WrappedHtmlGenericControl WdvInvoiceDispForCompanyType { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvInvoiceDispForCompanyType"); } }
	protected WrappedLiteral WlCompanyCode { get { return GetWrappedControl<WrappedLiteral>("lCompanyCode"); } }
	protected WrappedLiteral WlCompanyName { get { return GetWrappedControl<WrappedLiteral>("lCompanyName"); } }
	protected WrappedHtmlGenericControl WdvInvoiceDispForDonateType { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvInvoiceDispForDonateType"); } }
	protected WrappedLiteral WlDonationCode { get { return GetWrappedControl<WrappedLiteral>("lDonationCode"); } }
	protected WrappedHtmlGenericControl WdvInvoiceInput { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvInvoiceInput"); } }
	protected WrappedHtmlGenericControl WdvInvoiceInputForCompanyType { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvInvoiceInputForCompanyType"); } }
	protected WrappedTextBox WtbUniformInvoiceOption1_8 { get { return GetWrappedControl<WrappedTextBox>("tbUniformInvoiceOption1_8"); } }
	protected WrappedTextBox WtbUniformInvoiceOption2 { get { return GetWrappedControl<WrappedTextBox>("tbUniformInvoiceOption2"); } }
	protected WrappedHtmlGenericControl WdvInvoiceInputForDonateType { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvInvoiceInputForDonateType"); } }
	protected WrappedTextBox WtbUniformInvoiceOption1_3 { get { return GetWrappedControl<WrappedTextBox>("tbUniformInvoiceOption1_3"); } }

	protected WrappedHtmlGenericControl WdvUniformInvoiceTypeRegistInput { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvUniformInvoiceTypeRegistInput"); } }
	protected WrappedTextBox WtbUniformInvoiceTypeName { get { return GetWrappedControl<WrappedTextBox>("tbUniformInvoiceTypeName"); } }
	protected WrappedCheckBox WcbSaveToUserInvoice { get { return GetWrappedControl<WrappedCheckBox>("cbSaveToUserInvoice"); } }
	protected WrappedHtmlGenericControl WdvCarryTypeOptionName { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvCarryTypeOptionName"); } }
	protected WrappedCheckBox WcbCarryTypeOptionRegist { get { return GetWrappedControl<WrappedCheckBox>("cbCarryTypeOptionRegist"); } }
	protected WrappedTextBox WtbCarryTypeOptionName { get { return GetWrappedControl<WrappedTextBox>("tbCarryTypeOptionName"); } }

	protected WrappedLinkButton WlbUpdatePayment { get { return GetWrappedControl<WrappedLinkButton>("lbUpdatePayment"); } }

	protected WrappedDropDownList WddlShippingType { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingType", CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW); } }
	protected WrappedHtmlGenericControl WdvShippingInputFormInner { get { return GetWrappedControl<WrappedHtmlGenericControl>("divShippingInputFormInner"); } }
	protected WrappedRepeater WrOrderShippingList { get { return GetWrappedControl<WrappedRepeater>("rOrderShippingList"); } }
	protected WrappedHtmlGenericControl WdvShippingInputFormConvenience { get { return GetWrappedControl<WrappedHtmlGenericControl>("divShippingInputFormConvenience"); } }
	protected WrappedHtmlGenericControl WtbOwnerAddress { get { return GetWrappedControl<WrappedHtmlGenericControl>("tbOwnerAddress"); } }
	protected WrappedHtmlGenericControl WspConvenienceStoreSelect { get { return GetWrappedControl<WrappedHtmlGenericControl>("spConvenienceStoreSelect"); } }
	protected WrappedHtmlGenericControl WdvErrorShippingConvenience { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvErrorShippingConvenience"); } }
	protected WrappedHtmlGenericControl WdvShippingTime { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvShippingTime"); } }

	protected WrappedHiddenField WhfShippingReceivingStoreId { get { return GetWrappedControl<WrappedHiddenField>("hfCvsShopId"); } }
	protected WrappedHiddenField WhfShippingReceivingStoreName { get { return GetWrappedControl<WrappedHiddenField>("hfCvsShopName"); } }
	protected WrappedHiddenField WhfShippingReceivingStoreAddr { get { return GetWrappedControl<WrappedHiddenField>("hfCvsShopAddress"); } }
	protected WrappedHiddenField WhfShippingReceivingStoreTel { get { return GetWrappedControl<WrappedHiddenField>("hfCvsShopTel"); } }
	protected WrappedHiddenField WhfSelectedShopId { get { return GetWrappedControl<WrappedHiddenField>("hfSelectedShopId"); } }
	protected WrappedHiddenField WhfCvsShopFlg { get { return GetWrappedControl<WrappedHiddenField>("hfCvsShopFlg"); } }

	protected WrappedLiteral WlCvsShopId { get { return GetWrappedControl<WrappedLiteral>("lCvsShopId"); } }
	protected WrappedLiteral WlCvsShopName { get { return GetWrappedControl<WrappedLiteral>("lCvsShopName"); } }
	protected WrappedLiteral WlCvsShopAddress { get { return GetWrappedControl<WrappedLiteral>("lCvsShopAddress"); } }
	protected WrappedLiteral WlCvsShopTel { get { return GetWrappedControl<WrappedLiteral>("lCvsShopTel"); } }

	protected WrappedDropDownList WddlShippingReceivingStoreType { get { return GetWrappedControl<WrappedDropDownList>("ddlShippingReceivingStoreType"); } }
	protected WrappedHtmlGenericControl WspConvenienceStoreEcPaySelect { get { return GetWrappedControl<WrappedHtmlGenericControl>("spConvenienceStoreEcPaySelect"); } }
	protected WrappedHtmlGenericControl WdvErrorPaymentAndShippingConvenience { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvErrorPaymentAndShippingConvenience"); } }

	protected WrappedHiddenField WhfProductId { get { return GetWrappedControl<WrappedHiddenField>("hfProductId"); } }
	protected WrappedHiddenField WhfOldVariationId { get { return GetWrappedControl<WrappedHiddenField>("hfOldVariationId"); } }
	protected WrappedHiddenField WhfSelectedVariationId { get { return GetWrappedControl<WrappedHiddenField>("hfSelectedVariationId"); } }
	protected WrappedHiddenField WhfOldSubtotal { get { return GetWrappedControl<WrappedHiddenField>("hfOldSubtotal"); } }
	protected WrappedHiddenField WhfNewSubtotal { get { return GetWrappedControl<WrappedHiddenField>("hfNewSubtotal"); } }
	protected WrappedHiddenField WhfVariationName { get { return GetWrappedControl<WrappedHiddenField>("hfVariationName"); } }
	protected WrappedHiddenField WhfProductOptionTexts { get { return GetWrappedControl<WrappedHiddenField>("hfProductOptionTexts"); } }
	protected WrappedHtmlGenericControl WdvVariationErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvVariationErrorMessage"); } }
	protected WrappedHtmlGenericControl WdvProductOptionValueErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvProductOptionValueErrorMessage"); } }
	protected WrappedLinkButton WlbOrderExtend { get { return GetWrappedControl<WrappedLinkButton>("lbOrderExtend"); } }
	protected WrappedRepeater WrOrderExtendInput { get { return GetWrappedControl<WrappedRepeater>("rOrderExtendInput"); } }
	protected WrappedRepeater WrOrderExtendDisplay { get { return GetWrappedControl<WrappedRepeater>("rOrderExtendDisplay"); } }

	protected WrappedLinkButton WlbSearchShippingAddr { get { return GetWrappedControl<WrappedLinkButton>("lbSearchShippingAddr"); } }
	protected WrappedTextBox WtbShippingZip { get { return GetWrappedControl<WrappedTextBox>("tbShippingZip"); } }
	protected WrappedTextBox WtbShippingTel { get { return GetWrappedControl<WrappedTextBox>("tbShippingTel"); } }
	protected WrappedLinkButton WlbSearchAddrFromShippingZipGlobal { get { return GetWrappedControl<WrappedLinkButton>("lbSearchAddrFromShippingZipGlobal"); } }
	protected WrappedHtmlGenericControl WdvListProduct { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvListProduct"); } }
	protected WrappedHtmlGenericControl WdvModifySubscription { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvModifySubscription"); } }
	protected WrappedHtmlGenericControl WsErrorMessageSubscriptionBox { get { return GetWrappedControl<WrappedHtmlGenericControl>("sErrorMessageSubscriptionBox"); } }
	protected WrappedButton WbtnChangeProduct { get { return GetWrappedControl<WrappedButton>("btnChangeProduct"); } }
	protected WrappedButton WbtnChangeProductWithClearingSelection { get { return GetWrappedControl<WrappedButton>("btnChangeProductWithClearingSelection"); } }
	protected WrappedHtmlGenericControl WdvModifySubscriptionBoxModalBg { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvModifySubscriptionBoxModalBg"); } }
	protected WrappedRepeater WrOrderList { get { return GetWrappedControl<WrappedRepeater>("rOrderList"); } }
	protected WrappedHtmlGenericControl WdvOrderHistoryList { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvOrderHistoryList"); } }
	protected WrappedHiddenField WhfPagerMaxDispCount { get { return GetWrappedControl<WrappedHiddenField>("hfPagerMaxDispCount"); } }
	protected WrappedLinkButton WbtnModifyProducts { get { return GetWrappedControl<WrappedLinkButton>("btnModifyProducts"); } }
	protected WrappedLinkButton WbtnModifyConfirm { get { return GetWrappedControl<WrappedLinkButton>("btnModifyConfirm"); } }
	protected WrappedLinkButton WbtnModifyCancel { get { return GetWrappedControl<WrappedLinkButton>("btnModifyCancel"); } }
	protected WrappedLinkButton WbtnModifyAddProduct { get { return GetWrappedControl<WrappedLinkButton>("btnModifyAddProduct"); } }
	protected WrappedLinkButton WbtnUpdateProduct { get { return GetWrappedControl<WrappedLinkButton>("btnUpdateProduct"); } }
	protected WrappedHtmlGenericControl WdvModifyFixedPurchase { get { return GetWrappedControl<WrappedHtmlGenericControl>("dvModifyFixedPurchase"); } }
	protected WrappedRepeater WrFixedPurchaseModifyProducts { get { return GetWrappedControl<WrappedRepeater>("rFixedPurchaseModifyProducts"); } }
	protected WrappedHtmlGenericControl WsProductModifyErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("sProductModifyErrorMessage"); } }
	protected WrappedLiteral WlNextProductSubTotal { get { return GetWrappedControl<WrappedLiteral>("lNextProductSubTotal"); } }
	protected WrappedLiteral WlNextCouponName { get { return GetWrappedControl<WrappedLiteral>("lNextCouponName"); } }
	protected WrappedLiteral WlNextOrderCouponUse { get { return GetWrappedControl<WrappedLiteral>("lNextOrderCouponUse"); } }
	protected WrappedLiteral WlNextUsePointPrice { get { return GetWrappedControl<WrappedLiteral>("lNextUsePointPrice"); } }
	protected WrappedLiteral WlNextFixedPurchaseDiscountPrice { get { return GetWrappedControl<WrappedLiteral>("lNextFixedPurchaseDiscountPrice"); } }
	protected WrappedLiteral WlNextOrderTotal { get { return GetWrappedControl<WrappedLiteral>("lNextOrderTotal"); } }
	protected WrappedLiteral WlCancelPaypayNotification { get { return GetWrappedControl<WrappedLiteral>("lCancelPaypayNotification"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// ログインチェック（ログイン後は定期購入詳細から）
		CheckLoggedIn(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.RequestFixedPurchaseId));

		// HTTPS通信チェック（HTTPのとき、トップ画面へ）
		CheckHttps();

		// 配送情報入力画面初期処理（共通）
		InitComponentsOrderShipping();

		// 定期購入情報セット
		SetFixedPurchaseInfo();

		// AmazonPay(CV2)初期化
		InitAmazonPay();

		if (!IsPostBack)
		{
			// 表示コンポーネント初期化
			InitializeComponents();

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				// Adjust point and member rank by Cross Point api
				UserUtility.AdjustPointAndMemberRankByCrossPointApi(this.LoginUser.UserId);
			}

			// 画面にセット
			SetValues();

			// 利用可能のクーポン取得
			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				this.UsableCoupons = GetUsableCoupons(
					CreateSimpleCartListForFixedPurchase(this.FixedPurchaseContainer.FixedPurchaseId).Items[0]);
			}

			// データバインド
			DataBind();

			//クレジットカードフォームのコンポーネントをセット
			SetCreditCardComponents(
				this.FixedPurchaseContainer.OrderPaymentKbn,
				this.FixedPurchaseContainer.CreditBranchNo.ToString(),
				this.FixedPurchaseContainer.CardInstallmentsCode);

			// AmazonPayフォームのコンポーネントをセット
			SetAmazonPayComponents();

			// 決済エラーの場合は支払い方法変更時の「キャンセル」ボタンを非表示
			if (this.FixedPurchaseContainer.PaymentStatus == Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_ERROR
				&& this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus == false)
			{
				// 決済種別ラジオボタンクリックイベント
				if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB)
				{
					var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(this.PaymentRepeaterItem ?? this.CreditRepeaterItem, "rbgPayment");
					if (wrbgPayment.InnerControl != null) rbgPayment_OnCheckedChanged(wrbgPayment, EventArgs.Empty);
				}
				else if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL)
				{
					var wddlPayment = GetWrappedControl<WrappedDropDownList>(
						this.PaymentRepeaterItem != null
							? this.PaymentRepeaterItem.Parent.Parent
							: this.CreditRepeaterItem.Parent.Parent,
						"ddlPayment");
					if (wddlPayment.InnerControl != null) rbgPayment_OnCheckedChanged(wddlPayment, EventArgs.Empty);
				}
				
				var wrbtnClosePaymentPatternInfo = GetWrappedControl<WrappedLinkButton>("btnClosePaymentPatternInfo");
				this.WdvOrderPaymentPattern.Visible = true;
				wrbtnClosePaymentPatternInfo.InnerControl.Enabled = false;
			}

			// トークン決済の場合はクライアント検証をオフ
			DisableCreditInputCustomValidatorForGetCreditToken(this.CreditRepeaterItem);

			// 今すぐ注文した時のメッセージをクリア
			this.OrderNowMessagesHtmlEncoded = null;
			// 定期注文登録された注文IDリストをクリア
			this.RegisteredOrderIds = null;
			// 定期購入再開した時のメッセージをクリア
			this.ResumeFixedPurchaseMessageHtmlEncoded = null;
			// ポイントをリセットした時のメッセージをクリア
			this.PointResetMessages = null;

			// Display Taiwan Address
			DisplayTaiwanAddress();

			ActionFromAmazonPay();

			ActionFromBokuPay();

			// 商品追加用セッション初期化
			SessionManager.IsRedirectFromFixedPurchaseDetail = false;
		}
		else
		{
			// トークンが入力されていたら入力画面を切り替える
			SwitchDisplayForCreditTokenInput(this.CreditRepeaterItem);
		}

		// 選択された登録カードセット
		SetUserCreditCard(this.UserCreditCardsUsable);
		RefreshCreditForm();

		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
		{
			CheckSiteDomainAndRedirectWithPostData();
			SetInformationReceivingStore();
		}

		GetOrderByFixedPurchaseId();

		// 商品追加
		if (Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_BACK] != null)
		{
			AddSelectProduct();
			Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_BACK] = null;
			Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_ID] = null;
			Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_VARIATION_ID] = null;
			Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_PRODUCT_OPTION] = null;
		}

		// 商品変更
		if (Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_BACK] != null)
		{
			ChangeSelectProduct();
			Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_ID] = null;
			Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_BACK] = null;
			Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_BEFORE_PRODUCT_ID] = null;
			Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_AFTER_PRODUCT_ID] = null;
			Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_VARIATION_ID] = null;
		}
	}

	/// <summary>
	/// ページ表示終了時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void Page_LoadComplete(object sender, EventArgs e)
	{
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED 
			&& (string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId) == false))
		{
			var fixedAmountFlg = new SubscriptionBoxService().GetByCourseId(this.FixedPurchaseContainer.SubscriptionBoxCourseId).FixedAmountFlg;
			if (fixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_FALSE) SetSubscriptionBoxItemToDisabled();
		}

		// 配送キャンセル期限による頒布会商品変更不可エラー出力
		if (BeforeCancelDeadline == false)
		{
			WsErrorMessageSubscriptionBox.InnerHtml = WebMessages
				.GetMessages(WebMessages.ERRMSG_SUBSCRIPTION_BOX_PRODUCT_CANNOT_BE_CHANGED)
				.Replace("@@ 1 @@", this.ShopShipping.FixedPurchaseCancelDeadline.ToString());
		}
	}

	/// <summary>
	/// AmazonPay初期化
	/// </summary>
	private void InitAmazonPay()
	{
		this.AmazonFacade = new AmazonCv2ApiFacade();

		var amazonCallbackPath = AmazonCv2ApiFacade.CreateCallbackPathForFixedPurchase(
			AmazonCv2Constants.AMAZON_ACTION_STATUS_CREATE_SESSION,
			this.FixedPurchaseContainer.FixedPurchaseId);

		this.AmazonRequest = AmazonCv2Redirect.SignPayloadForReccuring(
			this.FixedPurchaseContainer.Shippings.First().Items.Sum(i => i.Price),
			callbackPath: amazonCallbackPath);

		if ((this.AmazonCheckoutSessionId != null)
			&& (this.AmazonActionStatus == AmazonCv2Constants.AMAZON_ACTION_STATUS_CREATE_SESSION))
		{
			this.AmazonCheckoutSession = this.AmazonFacade.GetCheckoutSession(this.AmazonCheckoutSessionId);
		}
	}

	/// <summary>
	/// AmazonPayCv2から遷移
	/// </summary>
	private void ActionFromAmazonPay()
	{
		if (string.IsNullOrEmpty(this.AmazonCheckoutSessionId)) return;

		switch (Request[AmazonCv2Constants.REQUEST_KEY_AMAZON_ACTION_STATUS])
		{
			case AmazonCv2Constants.AMAZON_ACTION_STATUS_CREATE_SESSION:
				this.WdvOrderPaymentPattern.Visible = true;
				if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB)
				{
					foreach (RepeaterItem wrPaymentItem in this.WrPayment.Items)
					{
						var wrbgTmpPayment = GetWrappedControl<WrappedRadioButtonGroup>(wrPaymentItem, "rbgPayment");
						wrbgTmpPayment.Checked = false;
					}
					var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(this.AmazonPayCv2RepeaterItem, "rbgPayment");
					wrbgPayment.Checked = true;
					if (wrbgPayment.InnerControl != null)
					{
						rbgPayment_OnCheckedChanged(wrbgPayment, EventArgs.Empty);
						var sender = GetWrappedControl<WrappedLinkButton>("lbUpdatePayment");
						btnUpdatePaymentPatternInfo_Click(sender, EventArgs.Empty);
					}
				}
				else if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL)
				{
					var wddlPayment = GetWrappedControl<WrappedDropDownList>(this.AmazonPayCv2RepeaterItem.Parent.Parent, "ddlPayment");
					wddlPayment.SelectedValue = Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2;
					if (wddlPayment.InnerControl != null)
					{
						rbgPayment_OnCheckedChanged(wddlPayment, EventArgs.Empty);
						var sender = GetWrappedControl<WrappedLinkButton>("lbUpdatePayment");
						btnUpdatePaymentPatternInfo_Click(sender, EventArgs.Empty);
					}
				}
				break;
			case AmazonCv2Constants.AMAZON_ACTION_STATUS_AUTH:
				var con = this.AmazonFacade.CompleteCheckoutSession(
					this.AmazonCheckoutSessionId,
					this.FixedPurchaseContainer.Shippings.First().Items.Sum(i => i.Price));
				var conError = AmazonCv2ApiFacade.GetErrorCodeAndMessage(con);

				if (con.Success == false)
				{
					this.WdvOrderPaymentPattern.Visible = true;
					if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB)
					{
						foreach (RepeaterItem wrPaymentItem in this.WrPayment.Items)
						{
							var wrbgTmpPayment = GetWrappedControl<WrappedRadioButtonGroup>(wrPaymentItem, "rbgPayment");
							wrbgTmpPayment.Checked = false;
						}
						var wrbgAmazonPayment = GetWrappedControl<WrappedRadioButtonGroup>(this.AmazonPayCv2RepeaterItem, "rbgPayment");
						wrbgAmazonPayment.Checked = true;
						if (wrbgAmazonPayment.InnerControl != null)
						{
							rbgPayment_OnCheckedChanged(wrbgAmazonPayment, EventArgs.Empty);
						}
					}
					else if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL)
					{
						var wddlAmazonPayment = GetWrappedControl<WrappedDropDownList>(this.AmazonPayCv2RepeaterItem.Parent.Parent, "ddlPayment");
						wddlAmazonPayment.SelectedValue = Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2;
						if (wddlAmazonPayment.InnerControl != null)
						{
							rbgPayment_OnCheckedChanged(wddlAmazonPayment, EventArgs.Empty);
						}
					}

					this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(
						WebMessages.ERRMSG_FRONT_CHANGE_TO_ANOTHER_PAYMENT_FOR_AUTH_ERROR);

					new FixedPurchaseService().UpdateForCreditRegisterFail(
						this.FixedPurchaseContainer.FixedPurchaseId,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.DoNotInsert,
						LogCreator.CreateErrorMessage(conError.ReasonCode, conError.Message));
					return;
				}

				// 配送先更新
				ExecuteUpdatingShippingWithAmazonAddress(
					SessionManager.AmazonShippingAddress,
					this.FixedPurchaseShippingContainer.ShippingMethod,
					this.FixedPurchaseShippingContainer.DeliveryCompanyId,
					this.FixedPurchaseShippingContainer.ShippingTime);

				// 支払い方法更新
				ExecuteUpdatingPayment(
					Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2,
					null,
					string.Empty,
					con.ChargePermissionId);
				break;
		}
	}

	/// <summary>
	/// 配送先情報更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateUserShippingInfo_Click(object sender, EventArgs e)
	{
		// 入力情報作成
		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		var isShippingAddrCountryJp = IsCountryJp(this.WddlShippingCountry.SelectedValue);
		var shipping = GetInputShipping(input.Shippings[0], isShippingAddrCountryJp);
		if (shipping.ShippingReceivingStoreFlg
			== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
		{
			// カスタムバリデータ取得（正常完了でも次の画面に遷移しないためエラー初期化）
			var customValidators = new List<CustomValidator>();
			CreateCustomValidators(this, customValidators);
			foreach (var validator in customValidators)
			{
				var validatorTemp = validator.Parent.FindControl(validator.ControlToValidate);
				if (validatorTemp != null)
				{
					// 初期化
					validator.IsValid = true;
					validator.ErrorMessage = string.Empty;
					((WebControl)validatorTemp).CssClass = ((WebControl)validatorTemp).CssClass
						.Replace(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, string.Empty);
				}
			}
			// エラーチェック＆カスタムバリデータへセット
			var excludeList = new List<string>();
			if (this.WtbShippingTel.HasInnerControl == false)
			{
				excludeList.Add(Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_TEL1);
			}
			var errorMessages = shipping.Validate(excludeList);
			if (errorMessages.Count != 0)
			{
				if (errorMessages.ContainsKey(Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ZIP))
				{
					this.ZipInputErrorMessage = string.Empty;
				}

				// エラーをカスタムバリデータへ
				SetControlViewsForError(
					isShippingAddrCountryJp
						? "FixedPurchaseModifyInput"
						: "FixedPurchaseModifyInputGlobal",
					errorMessages,
					customValidators);
				return;
			}
		}
		else
		{
			if (string.IsNullOrEmpty(shipping.ShippingReceivingStoreId)
				|| ((OrderCommon.CheckIdExistsInXmlStore(shipping.ShippingReceivingStoreId) == false)
					&& (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false)))
			{
				return;
			}
		}

		// Check Country Shipping
		var isShippingAddrCountryTw =
			GlobalAddressUtil.IsCountryTw(shipping.ShippingAddrCountryIsoCode);
		var errorMessage = string.Empty;
		if (Constants.PAYMENT_ATONEOPTION_ENABLED
			|| Constants.PAYMENT_AFTEEOPTION_ENABLED)
		{
			switch (input.OrderPaymentKbn)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
					if (shipping.IsShippingAddrJp == false)
					{
						errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_ATONE);
					}
					break;

				case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
					if (isShippingAddrCountryTw == false)
					{
						errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_AFTEE);
					}
					break;
			}
			if (errorMessage.Length > 0)
			{
				var wsErrorMessageShipping = GetWrappedControl<WrappedHtmlGenericControl>("sErrorMessageShipping");
				wsErrorMessageShipping.InnerText = errorMessage;
				return;
			}
		}

		// 配送先不可チェック
		if (OrderCommon.CheckUnavailableShippingArea(
			this.UnavailableShippingZip,
			shipping.HyphenlessShippingZip)) return;

		// 更新
		var fixedPurchase = input.CreateModel();
		fixedPurchase.Shippings[0].DeliveryCompanyId = WddlDeliveryCompany.HasInnerControl ? this.SelectedDeliveryCompany.DeliveryCompanyId : this.DeliveryCompany.DeliveryCompanyId;
		new FixedPurchaseService().UpdateShipping(
			fixedPurchase.Shippings[0],
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);
		new FixedPurchaseService().UpdateFixedPurchaseStatusStopUnavailableShippingAreaToNormal(
			fixedPurchase.FixedPurchaseId,
			fixedPurchase.LastChanged,
			UpdateHistoryAction.Insert);
		// Update payment convenience store
		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
			&& this.WddlShippingReceivingStoreType.HasInnerControl
			&& (ECPayUtility.GetIsCollection(this.WddlShippingReceivingStoreType.SelectedValue)
				== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON))
		{
			this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreType = this.WddlShippingReceivingStoreType.SelectedValue;
			btnUpdatePaymentPatternInfo_Click(sender, e);
		}

		// メール送信
		SendMailCommon.SendModifyFixedPurchaseMail(fixedPurchase.FixedPurchaseId, SendMailCommon.FixedPurchaseModify.Shipping);

		// 再表示
		Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(fixedPurchase.FixedPurchaseId));
	}

	/// <summary>
	/// Get Input Shipping
	/// </summary>
	/// <returns>Fixed Purchase Shipping Input</returns>
	private FixedPurchaseShippingInput GetInputShipping(FixedPurchaseShippingInput shipping, bool isShippingAddrCountryJp)
	{
		switch (this.WddlShippingType.SelectedValue)
		{
			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
				shipping.ShippingName1 = this.LoginUser.Name1;
				shipping.ShippingName2 = this.LoginUser.Name2;
				shipping.ShippingName = this.LoginUser.Name;
				shipping.ShippingNameKana1 = this.LoginUser.NameKana1;
				shipping.ShippingNameKana2 = this.LoginUser.NameKana2;
				shipping.ShippingNameKana = this.LoginUser.NameKana;
				shipping.ShippingZip = this.LoginUser.Zip;
				shipping.ShippingAddr1 = this.LoginUser.Addr1;
				shipping.ShippingAddr2 = this.LoginUser.Addr2;
				shipping.ShippingAddr3 = this.LoginUser.Addr3;
				shipping.ShippingAddr4 = this.LoginUser.Addr4;
				shipping.ShippingAddr5 = this.LoginUser.Addr5;
				shipping.ShippingCompanyName = this.LoginUser.CompanyName;
				shipping.ShippingCompanyPostName = this.LoginUser.CompanyPostName;
				shipping.ShippingTel1 = this.LoginUser.Tel1;
				shipping.ShippingAddrCountryName = this.LoginUser.AddrCountryName;
				shipping.ShippingAddrCountryIsoCode = this.LoginUser.AddrCountryIsoCode;
				shipping.ShippingReceivingStoreFlg =
					Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
				shipping.ShippingReceivingStoreId = string.Empty;
				shipping.DeliveryCompanyId = this.WddlDeliveryCompany.SelectedValue;
				shipping.ShippingTime = this.WddlShippingTime.SelectedValue;
				shipping.DeliveryCompanyId = this.WddlDeliveryCompany.SelectedValue;
				shipping.ShippingReceivingStoreType = string.Empty;

				return shipping;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
				shipping.ShippingName1 = DataInputUtility.ConvertToFullWidthBySetting(
					this.WtbShippingName1.Text.Trim(),
					isShippingAddrCountryJp);
				shipping.ShippingName2 = DataInputUtility.ConvertToFullWidthBySetting(
					this.WtbShippingName2.Text.Trim(),
					isShippingAddrCountryJp);
				shipping.ShippingName = shipping.ShippingName1 + shipping.ShippingName2;
				shipping.ShippingNameKana1 = StringUtility.ToZenkaku(this.WtbShippingNameKana1.Text.Trim());
				shipping.ShippingNameKana2 = StringUtility.ToZenkaku(this.WtbShippingNameKana2.Text.Trim());
				shipping.ShippingNameKana = string.Format(
					"{0}{1}",
						shipping.ShippingNameKana1,
						shipping.ShippingNameKana2);
				shipping.ShippingAddr1 = this.WddlShippingAddr1.SelectedValue;

				// Taiwan address
				var shippingAddr2 = (this.IsShippingAddrTw && this.WddlShippingAddr2.HasInnerControl)
					? this.WddlShippingAddr2.SelectedValue
					: this.WtbShippingAddr2.Text.Trim();
				var shippingAddr3 = (this.IsShippingAddrTw && this.WddlShippingAddr3.HasInnerControl)
					? this.WddlShippingAddr3.SelectedText
					: this.WtbShippingAddr3.Text.Trim();

				shipping.ShippingAddr2 =
					DataInputUtility.ConvertToFullWidthBySetting(shippingAddr2, isShippingAddrCountryJp);
				shipping.ShippingAddr3 =
					DataInputUtility.ConvertToFullWidthBySetting(shippingAddr3, isShippingAddrCountryJp);
				shipping.ShippingAddr4 = DataInputUtility.ConvertToFullWidthBySetting(
					this.WtbShippingAddr4.Text.Trim(),
					isShippingAddrCountryJp);
				shipping.ShippingCompanyName = DataInputUtility.ConvertToFullWidthBySetting(
					this.WtbShippingCompanyName.Text.Trim(),
					isShippingAddrCountryJp);
				shipping.ShippingCompanyPostName = DataInputUtility.ConvertToFullWidthBySetting(
					this.WtbShippingCompanyPostName.Text.Trim(),
					isShippingAddrCountryJp);

				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					shipping.ShippingAddrCountryIsoCode = this.WddlShippingCountry.SelectedValue;
					shipping.ShippingAddrCountryName = this.WddlShippingCountry.SelectedText;
				}

				if (isShippingAddrCountryJp)
				{
					// Set value for zip code
					var inputZipCode = (this.WtbShippingZip1.HasInnerControl)
						? StringUtility.ToHankaku(this.WtbShippingZip1.Text.Trim())
						: StringUtility.ToHankaku(this.WtbShippingZip.Text.Trim());
					if (this.WtbShippingZip1.HasInnerControl) inputZipCode += ("-" + StringUtility.ToHankaku(this.WtbShippingZip2.Text.Trim()));
					var zipCode = new ZipCode(inputZipCode);
					shipping.ShippingZip1 = zipCode.Zip1;
					shipping.ShippingZip2 = zipCode.Zip2;
					shipping.ShippingZip = (string.IsNullOrEmpty(zipCode.Zip) == false)
						? zipCode.Zip
						: inputZipCode;

					// Set value for telephone
					var inputTel = (this.WtbShippingTel1.HasInnerControl)
						? StringUtility.ToHankaku(this.WtbShippingTel1.Text.Trim())
						: StringUtility.ToHankaku(this.WtbShippingTel.Text.Trim());
					if (this.WtbShippingTel1.HasInnerControl)
					{
						inputTel = UserService.CreatePhoneNo(
							inputTel,
							StringUtility.ToHankaku(this.WtbShippingTel2.Text.Trim()),
							StringUtility.ToHankaku(this.WtbShippingTel3.Text.Trim()));
					}
					var tel = new Tel(inputTel);
					shipping.ShippingTel1_1 = tel.Tel1;
					shipping.ShippingTel1_2 = tel.Tel2;
					shipping.ShippingTel1_3 = tel.Tel3;
					shipping.ShippingTel1 = (string.IsNullOrEmpty(tel.TelNo) == false)
						? tel.TelNo
						: inputTel;
					shipping.ShippingAddr5 = string.Empty;
				}
				else
				{
					shipping.ShippingZip = StringUtility.ToHankaku(this.WtbShippingZipGlobal.Text);
					shipping.ShippingZip1 = string.Empty;
					shipping.ShippingZip2 = string.Empty;
					shipping.ShippingTel1 = StringUtility.ToHankaku(this.WtbShippingTel1Global.Text);
					shipping.ShippingTel1_1 = string.Empty;
					shipping.ShippingTel1_2 = string.Empty;
					shipping.ShippingTel1_3 = string.Empty;
					shipping.ShippingAddr5 = IsCountryUs(this.WddlShippingCountry.SelectedValue)
						? this.WddlShippingAddr5.SelectedText
						: DataInputUtility.ConvertToFullWidthBySetting(
							this.WtbShippingAddr5.Text,
							isShippingAddrCountryJp);

					shipping.ShippingNameKana1 = string.Empty;
					shipping.ShippingNameKana2 = string.Empty;
					shipping.ShippingNameKana = string.Empty;
				}

				shipping.ShippingMethod = this.WddlShippingMethod.SelectedValue;
				shipping.DeliveryCompanyId = this.WddlDeliveryCompany.SelectedValue;
				shipping.ShippingTime = this.WddlShippingTime.SelectedValue;
				shipping.ShippingReceivingStoreFlg =
					Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;

				return shipping;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE:
				shipping.ShippingReceivingStoreId = this.WhfShippingReceivingStoreId.Value;
				shipping.ShippingName = this.WhfShippingReceivingStoreName.Value;
				shipping.ShippingAddr4 = this.WhfShippingReceivingStoreAddr.Value;
				shipping.ShippingTel1 = this.WhfShippingReceivingStoreTel.Value;
				shipping.ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
				shipping.ShippingReceivingStoreType = this.WddlShippingReceivingStoreType.SelectedValue;

				return shipping;

			default:
				var index = -1;
				var repeaterItem = this.WrOrderShippingList.Items.Cast<RepeaterItem>()
					.FirstOrDefault(item => item.Visible == true);
				if (repeaterItem != null) index = repeaterItem.ItemIndex;
				var whfCvsShopId = GetWrappedControl<WrappedHiddenField>(
					this.WrOrderShippingList.Items[index],
					"hfCvsShopId");
				var whfCvsShopName = GetWrappedControl<WrappedHiddenField>(
					this.WrOrderShippingList.Items[index],
					"hfCvsShopName");
				var whfCvsShopAddress = GetWrappedControl<WrappedHiddenField>(
					this.WrOrderShippingList.Items[index],
					"hfCvsShopAddress");
				var whfCvsShopTel = GetWrappedControl<WrappedHiddenField>(
					this.WrOrderShippingList.Items[index],
					"hfCvsShopTel");

				var selectedUserShipping = this.UserShippingAddr[index];
				if (selectedUserShipping.ShippingReceivingStoreFlg
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
				{
					shipping.ShippingName1 = selectedUserShipping.ShippingName1;
					shipping.ShippingName2 = selectedUserShipping.ShippingName2;
					shipping.ShippingName = selectedUserShipping.ShippingName;
					shipping.ShippingNameKana1 = selectedUserShipping.ShippingNameKana1;
					shipping.ShippingNameKana2 = selectedUserShipping.ShippingNameKana2;
					shipping.ShippingNameKana = selectedUserShipping.ShippingNameKana;
					shipping.ShippingZip = selectedUserShipping.ShippingZip;
					shipping.ShippingAddr1 = selectedUserShipping.ShippingAddr1;
					shipping.ShippingAddr2 = selectedUserShipping.ShippingAddr2;
					shipping.ShippingAddr3 = selectedUserShipping.ShippingAddr3;
					shipping.ShippingAddr4 = selectedUserShipping.ShippingAddr4;
					shipping.ShippingAddr5 = selectedUserShipping.ShippingAddr5;
					shipping.ShippingCompanyName = selectedUserShipping.ShippingCompanyName;
					shipping.ShippingCompanyPostName = selectedUserShipping.ShippingCompanyPostName;
					shipping.ShippingTel1 = selectedUserShipping.ShippingTel1;
					shipping.ShippingAddrCountryName = selectedUserShipping.ShippingCountryName;
					shipping.ShippingAddrCountryIsoCode = selectedUserShipping.ShippingCountryIsoCode;
					shipping.ShippingReceivingStoreFlg = selectedUserShipping.ShippingReceivingStoreFlg;
					shipping.ShippingReceivingStoreId = selectedUserShipping.ShippingReceivingStoreId;
					shipping.ShippingMethod = this.WddlShippingMethod.SelectedValue;
					shipping.DeliveryCompanyId = this.WddlDeliveryCompany.SelectedValue;
					shipping.ShippingTime = this.WddlShippingTime.SelectedValue;
					shipping.DeliveryCompanyId = this.WddlDeliveryCompany.SelectedValue;
					shipping.ShippingReceivingStoreType = selectedUserShipping.ShippingReceivingStoreType;
				}
				else
				{
					shipping.ShippingReceivingStoreId = whfCvsShopId.Value;
					shipping.ShippingName = whfCvsShopName.Value;
					shipping.ShippingAddr4 = whfCvsShopAddress.Value;
					shipping.ShippingTel1 = whfCvsShopTel.Value;
					shipping.ShippingReceivingStoreFlg =
						selectedUserShipping.ShippingReceivingStoreFlg;
					shipping.ShippingReceivingStoreType = selectedUserShipping.ShippingReceivingStoreType;
				}

				return shipping;
		}
	}

	/// <summary>
	/// 配送パターン変更ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateShippingPatternInfo_Click(object sender, EventArgs e)
	{
		// 指定した月間隔の値を取得
		var intervalMonth = ((this.WrbFixedPurchaseDays.Checked
				&& (this.WddlFixedPurchaseMonth.InnerControl == null))
			|| (this.WrbFixedPurchaseWeekAndDay.Checked
				&& (this.WddlFixedPurchaseIntervalMonths.InnerControl == null)))
			? "1"
			: this.WrbFixedPurchaseDays.Checked
				? this.WddlFixedPurchaseMonth.SelectedValue
				: this.WddlFixedPurchaseIntervalMonths.SelectedValue;
		// 定期購入区分、定期購入設定１をセット
		// 月間隔日付指定
		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		input.FixedPurchaseKbn = "";
		if (this.WrbFixedPurchaseDays.Checked)
		{
			input.FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE;
			input.FixedPurchaseSetting1 = intervalMonth + "," + this.WddlFixedPurchaseMonthlyDate.SelectedValue;
			input.FixedPurchaseSetting1_1_1 = intervalMonth;
			input.FixedPurchaseSetting1_1_2 = this.WddlFixedPurchaseMonthlyDate.SelectedValue;
		}
		// 月間隔・週・曜日指定
		else if (this.WrbFixedPurchaseWeekAndDay.Checked)
		{
			input.FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY;
			input.FixedPurchaseSetting1 = string.Format(
				"{0},{1},{2}",
				intervalMonth,
				this.WddlFixedPurchaseWeekOfMonth.SelectedValue,
				this.WddlFixedPurchaseDayOfWeek.SelectedValue);
			input.FixedPurchaseSettingIntervalMonths = intervalMonth;
			input.FixedPurchaseSetting1_2_1 = this.WddlFixedPurchaseWeekOfMonth.SelectedValue;
			input.FixedPurchaseSetting1_2_2 = this.WddlFixedPurchaseDayOfWeek.SelectedValue;
		}
		// 配送間隔指定
		else if (this.WrbFixedPurchaseIntervalDays.Checked)
		{
			input.FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS;
			input.FixedPurchaseSetting1 = this.WddlFixedPurchaseIntervalDays.SelectedValue;
			input.FixedPurchaseSetting1_3 = this.WddlFixedPurchaseIntervalDays.SelectedValue;
		}
		// 週間隔・曜日指定
		else if (this.WrbFixedPurchaseEveryNWeek.Checked)
		{
			input.FixedPurchaseKbn = Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY;
			input.FixedPurchaseSetting1 = string.Format(
				"{0},{1}",
				this.WddlFixedPurchaseEveryNWeek_Week.SelectedValue,
				this.WddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue);
			input.FixedPurchaseSetting1_4_1 = this.WddlFixedPurchaseEveryNWeek_Week.SelectedValue;
			input.FixedPurchaseSetting1_4_2 = this.WddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue;
		}

		// パターンが変更されていなければ処理を中断
		if (this.FixedPurchaseContainer.FixedPurchaseSetting1 == input.FixedPurchaseSetting1)
		{
			this.WdvFixedPurchaseDetailShippingPattern.Visible = false;
			return;
		}

		// カスタムバリデータ取得（正常完了でも次の画面に遷移しないためエラー初期化）
		var customValidators = new List<CustomValidator>();
		CreateCustomValidators(this, customValidators);
		foreach (CustomValidator validator in customValidators)
		{
			var validatorTemp = validator.Parent.FindControl(((CustomValidator)validator).ControlToValidate);
			if (validatorTemp != null)
			{
				// 初期化
				validator.IsValid = true;
				validator.ErrorMessage = "";
				((WebControl)validatorTemp).CssClass = ((WebControl)validatorTemp).CssClass.Replace(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, "");
			}
		}

		// エラーチェック＆カスタムバリデータへセット
		var errorMessages = input.Validate();
		// 利用不可配送間隔日・月かどうかのチェック
		var intervalError = string.Empty;
		switch (input.FixedPurchaseKbn)
		{
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
				intervalError = CheckSpecificFixedPurchaseIntervalValues(input.FixedPurchaseSetting1_1_1, input.FixedPurchaseKbn);
				break;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
				intervalError = CheckSpecificFixedPurchaseIntervalValues(input.FixedPurchaseSettingIntervalMonths, input.FixedPurchaseKbn);
				break;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
				intervalError = CheckSpecificFixedPurchaseIntervalValues(input.FixedPurchaseSetting1_3, input.FixedPurchaseKbn);
				break;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
				intervalError = CheckSpecificFixedPurchaseIntervalValues(input.FixedPurchaseSetting1_4_1, input.FixedPurchaseKbn);
				break;
		}
		if (string.IsNullOrEmpty(intervalError) == false)
		{
			errorMessages.Add(
				(input.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
					? Constants.FIXED_PURCHASE_SETTING_MONTH
					: (input.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY)
						? Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS
					: Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS,
				intervalError);
		}

		var wsErrorMessage = GetWrappedControl<WrappedHtmlGenericControl>("sErrorMessage");
		wsErrorMessage.InnerText = "";
		if (errorMessages.Count != 0)
		{
			// エラーをカスタムバリデータへ
			SetControlViewsForError("OrderShipping", errorMessages, customValidators);

			if (string.IsNullOrEmpty(input.FixedPurchaseKbn))
			{
				wsErrorMessage.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SHIPPING_PATTERN_UNSELECTED);
			}
			return;
		}

		// 次回配送日・次々回配送日をセット
		var service = new FixedPurchaseService();
		var lastOrder = new OrderService().GetLastFixedPurchaseOrder(input.FixedPurchaseId);
		var fixedPurchaseCancelDeadline =
			DataCacheControllerFacade.GetShopShippingCacheController().Get(lastOrder.ShippingId).FixedPurchaseCancelDeadline;
		var arrivalScheduleDate = lastOrder.Shippings[0].ShippingDate ?? lastOrder.OrderDate.Value.AddDays(fixedPurchaseCancelDeadline);
		var calculateMode = service.GetCalculationMode(input.FixedPurchaseKbn, Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
		input.NextShippingDate =
			service.CalculateNextShippingDateFromLastShippedDate(
				input.FixedPurchaseKbn,
				input.FixedPurchaseSetting1,
				arrivalScheduleDate,
				this.ShopShipping.FixedPurchaseMinimumShippingSpan,
				this.ShopShipping.FixedPurchaseCancelDeadline).ToString();
		input.NextNextShippingDate =
			service.CalculateFollowingShippingDate(
				input.FixedPurchaseKbn,
				input.FixedPurchaseSetting1,
				DateTime.Parse(input.NextShippingDate),
				0,
				calculateMode).ToString();

		// 更新（更新履歴とともに）
		var fixedPurchase = input.CreateModel();
		service.UpdatePattern(
			fixedPurchase.FixedPurchaseId,
			fixedPurchase.FixedPurchaseKbn,
			fixedPurchase.FixedPurchaseSetting1,
			fixedPurchase.NextShippingDate,
			fixedPurchase.NextNextShippingDate,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);

		// メール送信
		SendMailCommon.SendModifyFixedPurchaseMail(fixedPurchase.FixedPurchaseId, SendMailCommon.FixedPurchaseModify.ShippingDate);

		// 再表示
		Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(fixedPurchase.FixedPurchaseId));
	}

	/// <summary>
	/// キャンセルボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCancelFixedPurchase_Click(object sender, EventArgs e)
	{
		// 確認画面へ遷移
		this.CancelReasonInput = new FixedPurchaseCancelReasonInput
		{
			CancelReasonId = string.Empty,
			CancelMemo = string.Empty,
			CancelReasonName = string.Empty
		};
		this.SuspendReasonInput = new FixedPurchaseSuspendReasonInput();

		Response.Redirect(CreateFixedPurchaseCancelReasonConfirmUrl(
			this.FixedPurchaseContainer.FixedPurchaseId,
			Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_CANCEL));
	}

	/// <summary>
	/// キャンセル（解約理由登録）ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCancelFixedPurchaseReason_Click(object sender, EventArgs e)
	{
		// 確認画面へ遷移
		Response.Redirect(CreateFixedPurchaseCancelReasonInputUrl(
			this.FixedPurchaseContainer.FixedPurchaseId,
			Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_CANCEL));
	}

	/// <summary>
	/// 次回配送スキップボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSkipNextShipping_Click(object sender, EventArgs e)
	{
		// 定期購入スキップ（更新履歴とともに）
		new FixedPurchaseService()
			.SkipOrder(
				this.FixedPurchaseContainer.FixedPurchaseId,
				Constants.FLG_LASTCHANGED_USER,
				this.ShopShipping,
				Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE,
				UpdateHistoryAction.Insert);

		// メール送信
		SendMailCommon.SendModifyFixedPurchaseMail(this.FixedPurchaseContainer.FixedPurchaseId, SendMailCommon.FixedPurchaseModify.SkipNextShipping);

		// Update next product
		if (string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId) == false)
		{
			UpdateNextProductOrCancel(UpdateHistoryAction.Insert);
		}

		// 再表示
		Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
	}

	/// <summary>
	/// カード情報更新ボタンクリック
	/// [V5.11互換]
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateCardInfo_Click(object sender, EventArgs e)
	{
		var success = true;
		var userCardBranchNo = 0;
		var installmentsCode = this.WciCardInputs.WddlInstallments.SelectedValue;

		UserCreditCardInput inputCreditCard = null;

		// 登録済みカードを使用する?
		if (this.WciCardInputs.WddlUserCreditCard.SelectedValue != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
		{
			// クレジットカード枝番、カード支払い回数コードをセット
			userCardBranchNo = int.Parse(this.WciCardInputs.WddlUserCreditCard.SelectedValue);
		}
		// カード情報更新?
		else
		{
			// カード番号入力情報作成
			inputCreditCard = new UserCreditCardInput()
			{
				UserId = this.LoginUserId,
				CardDispName = Constants.CREDITCARD_UNREGIST_FIXEDPURCHASE_DISPLAY_NAME,
				CompanyCode = OrderCommon.CreditCompanySelectable ? this.WciCardInputs.WddlCardCompany.SelectedValue : "",
				CardNo = StringUtility.ToHankaku(this.WciCardInputs.WtbCard1.Text + this.WciCardInputs.WtbCard2.Text + this.WciCardInputs.WtbCard3.Text + this.WciCardInputs.WtbCard4.Text),
				CardNo1 = StringUtility.ToHankaku(this.WciCardInputs.WtbCard1.Text),
				CardNo2 = StringUtility.ToHankaku(this.WciCardInputs.WtbCard2.Text),
				CardNo3 = StringUtility.ToHankaku(this.WciCardInputs.WtbCard3.Text),
				CardNo4 = StringUtility.ToHankaku(this.WciCardInputs.WtbCard4.Text),
				ExpirationMonth = this.WciCardInputs.WddlExpireMonth.Text,
				ExpirationYear = this.WciCardInputs.WddlExpireYear.Text,
				AuthorName = this.WciCardInputs.WtbAuthorName.Text,
				SecurityCode = OrderCommon.CreditSecurityCodeEnable ? StringUtility.ToHankaku(this.WciCardInputs.WtbSecurityCode.Text) : null,
				CreditToken = CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.WciCardInputs.WhfCreditToken.Value),
			};

			// カスタムバリデータ取得（正常完了でも次の画面に遷移しないためエラー初期化）
			var customValidators = new List<CustomValidator>();
			CreateCustomValidators(this, customValidators);
			foreach (CustomValidator validator in customValidators)
			{
				var validatorTemp = validator.Parent.FindControl(((CustomValidator)validator).ControlToValidate);
				if (validatorTemp != null)
				{
					// 初期化
					validator.IsValid = true;
					validator.ErrorMessage = "";
					((WebControl)validatorTemp).CssClass = ((WebControl)validatorTemp).CssClass.Replace(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, "");
				}
			}

			// エラーチェック＆カスタムバリデータへセット
			var errorMessages = inputCreditCard.ValidateForFrontFixedPurchaseRegist();
			if (errorMessages.Count != 0)
			{
				success = false;

				// エラーをカスタムバリデータへ
				SetControlViewsForError("OrderPayment", errorMessages, customValidators);

				// カード番号桁数エラーをカスタムバリデータへ
				if (this.WciCardInputs.WcvCreditCardNo1.IsValid)
				{
					ChangeControlLooksForValidator(
						errorMessages,
						CartPayment.FIELD_CREDIT_CARD_NO + "_length",
						this.WciCardInputs.WcvCreditCardNo1,
						this.WciCardInputs.WtbCard1, this.WciCardInputs.WtbCard2, this.WciCardInputs.WtbCard3, this.WciCardInputs.WtbCard4);
				}

				ResetCreditTokenInfoFromForm(null, Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, false);
			}

			if (success)
			{
				// クレジット与信ロックされていればエラーページへ
				if (CreditAuthAttackBlocker.Instance.IsLocked(this.LoginUserId, Request.UserHostAddress))
				{
					success = false;
					this.ErrorMessage = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CREDIT_AUTH_LOCK));

					ResetCreditTokenInfoFromForm(null, Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, false);
				}
				else
				{
					// カード登録
					var result = new UserCreditCardRegister().Exec(
						inputCreditCard,
						this.IsSmartPhone ? SiteKbn.SmartPhone : SiteKbn.Pc,
						false,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.DoNotInsert);

					PaymentFileLogger.WritePaymentLog(
						result.Success,
						"",
						PaymentFileLogger.PaymentType.Unknown,
						PaymentFileLogger.PaymentProcessingType.FixedPurchaseCreditRegist,
						result.Success ? "" : result.ErrorMessage,
						new Dictionary<string, string>
						{
							{Constants.FIELD_ORDER_CREDIT_BRANCH_NO, result.BranchNo == 0 ? userCardBranchNo.ToString() : result.BranchNo.ToString()},
							{Constants.FIELD_USER_USER_ID, this.LoginUserId}
						});

					if (result.Success)
					{
						userCardBranchNo = result.BranchNo;
					}
					else
					{
						success = false;

						this.ErrorMessage = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARD_AUTH_FAILED));

						// トークンなどクレジットカード情報削除
						ResetCreditTokenInfoFromForm(null, Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING);

						// クレジット与信試行カウント-1
						CreditAuthAttackBlocker.Instance.DecreasePossibleTrialCount(this.LoginUserId, Request.UserHostAddress);
					}
				}
			}
		}

		// 成功?
		if (success)
		{
			// クレジットカード決済与信成功更新（更新履歴とともに）
			new FixedPurchaseService()
				.UpdateForAuthSuccess(
					this.FixedPurchaseContainer.FixedPurchaseId,
					userCardBranchNo,
					installmentsCode,
					Constants.FLG_LASTCHANGED_USER,
					LogCreator.CreateWithUserId(inputCreditCard != null ? inputCreditCard.UserId : ""),
					UpdateHistoryAction.DoNotInsert);

			// 更新履歴登録
			new UpdateHistoryService().InsertForFixedPurchase(this.FixedPurchaseContainer.FixedPurchaseId, Constants.FLG_LASTCHANGED_USER);
			new UpdateHistoryService().InsertForUser(this.FixedPurchaseContainer.UserId, Constants.FLG_LASTCHANGED_USER);

			// メッセージ & 定期購入ステータスCSSセット
			this.CompleteMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARD_AUTH_SUCCESS);
			this.WspPaymentStatus.Attributes["class"] = "paymentStatus_" + Constants.FLG_FIXEDPURCHASE_PAYMENT_STATUS_NORMAL;

			// 画面表示用プロパティ更新
			SetFixedPurchaseInfo();

			// トークンなどクレジットカード情報削除
			ResetCreditTokenInfoFromForm(null, Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING);

			// 完了後画面用
			this.UserCreditCardInfo = new UserCreditCard(new UserCreditCardService().Get(this.LoginUserId, userCardBranchNo));
			dvFixedPurchaseCurrentCard.Visible = true;

			this.IsCreditInputModeOn = false;
		}
		// 失敗?
		else
		{
			var wspanErrorMessageForCreditCard = GetWrappedControl<WrappedHtmlGenericControl>(
				this.CreditRepeaterItem,
				"spanErrorMessageForCreditCard");
			// メッセージセット
			if (wspanErrorMessageForCreditCard.HasInnerControl)
			{
				wspanErrorMessageForCreditCard.InnerHtml = this.ErrorMessage;
				wspanErrorMessageForCreditCard.InnerControl.Style["display"] = "block";
			}
		}

		RefreshCreditFormForV511();
	}

	/// <summary>
	/// カード情報変更フォーム表示/非表示
	/// [V5.11互換]
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayCardInfoForm_Click(object sender, EventArgs e)
	{
		this.IsCreditInputModeOn = (this.IsCreditInputModeOn == false);

		RefreshCreditFormForV511();
	}

	/// <summary>
	/// クレジットフォームをリフレッシュ
	/// [V5.11互換]
	/// </summary>
	private void RefreshCreditFormForV511()
	{
		this.WdvFixedPurchaseDetailCard.Visible = this.IsPaymentCreditCard && this.IsCreditInputModeOn;
		this.WdvFixedPurchaseCurrentCard.Visible = this.IsPaymentCreditCard && (this.IsCreditInputModeOn == false);
		var wdivCreditCardNoToken = GetWrappedControl<WrappedHtmlGenericControl>("divCreditCardNoToken");
		wdivCreditCardNoToken.Visible = this.IsPaymentCreditCard && (HasCreditToken() == false);
	}

	/// <summary>
	/// 配送パターン情報変更フォーム表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayShippingPatternInfoForm_Click(object sender, EventArgs e)
	{
		this.WdvFixedPurchaseDetailShippingPattern.Visible = (this.WdvFixedPurchaseDetailShippingPattern.Visible == false);
		if (this.WdvFixedPurchaseDetailShippingPattern.Visible)
		{
			rbShippingPattern_OnCheckedChanged(this.FixedPurchaseContainer.FixedPurchaseKbn);
			switch (this.FixedPurchaseContainer.FixedPurchaseKbn)
			{
				// 月間隔日付指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
					this.WrbFixedPurchaseDays.Checked = (true && this.WdtMonthlyDate.Visible);
					this.WddlFixedPurchaseMonth.SelectedValue = this.FixedPurchaseContainer.FixedPurchaseSetting1.Split(',')[0];
					this.WddlFixedPurchaseMonthlyDate.SelectedValue = this.FixedPurchaseContainer.FixedPurchaseSetting1.Split(',')[1];
					break;

				// 月間隔・週・曜日指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
					this.WrbFixedPurchaseWeekAndDay.Checked = (true && this.WdtWeekAndDay.Visible);
					var splitedFixedPurchaseSetting = this.FixedPurchaseContainer.FixedPurchaseSetting1.Split(',');
					this.WddlFixedPurchaseIntervalMonths.SelectedValue = splitedFixedPurchaseSetting[0];
					this.WddlFixedPurchaseWeekOfMonth.SelectedValue = splitedFixedPurchaseSetting[1];
					this.WddlFixedPurchaseDayOfWeek.SelectedValue = splitedFixedPurchaseSetting[2];
					break;

				// 配送間隔指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
					this.WrbFixedPurchaseIntervalDays.Checked = (true && this.WdtIntervalDays.Visible);
					this.WddlFixedPurchaseIntervalDays.SelectedValue = this.FixedPurchaseContainer.FixedPurchaseSetting1;
					break;

				// 週間隔・曜日指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
					this.WrbFixedPurchaseEveryNWeek.Checked = this.WdtFixedPurchaseEveryNWeek.Visible;
					var fixedPurchaseSettings = this.FixedPurchaseContainer.FixedPurchaseSetting1.Split(',');
					this.WddlFixedPurchaseEveryNWeek_Week.SelectedValue = fixedPurchaseSettings[0];
					this.WddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue = fixedPurchaseSettings[1];
					break;
			}
		}
	}

	/// <summary>
	/// 配送パターン変更ラジオボタン
	/// </summary>
	/// <param name="shippingPattern">配送パターン</param>
	protected void rbShippingPattern_OnCheckedChanged(string shippingPattern)
	{
		this.WddMonthlyDate.Visible = (shippingPattern == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
			&& this.WdtMonthlyDate.Visible;
		this.WddWeekAndDay.Visible = (shippingPattern == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY)
			&& this.WdtWeekAndDay.Visible;
		this.WddIntervalDays.Visible = (shippingPattern == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS)
			&& this.WdtIntervalDays.Visible;
		this.WddFixedPurchaseEveryNWeek.Visible = (shippingPattern == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY)
			&& this.WdtFixedPurchaseEveryNWeek.Visible;
	}

	/// <summary>
	/// 配送パターン変更フォーム非表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCloseShippingPatternInfo_Click(object sender, EventArgs e)
	{
		this.WdvFixedPurchaseDetailShippingPattern.Visible = false;
	}

	/// <summary>
	/// 月間隔日付指定ラジオボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbFixedPurchaseDays_OnCheckedChanged(object sender, EventArgs e)
	{
		rbShippingPattern_OnCheckedChanged(Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE);
	}

	/// <summary>
	/// 週・曜日指定ラジオボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbFixedPurchaseWeekAndDay_OnCheckedChanged(object sender, EventArgs e)
	{
		rbShippingPattern_OnCheckedChanged(Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY);
	}

	/// <summary>
	/// 配送間隔指定ラジオボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbFixedPurchaseIntervalDays_OnCheckedChanged(object sender, EventArgs e)
	{
		rbShippingPattern_OnCheckedChanged(Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS);
	}

	/// <summary>
	/// 週間隔・曜日指定ラジオボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbFixedPurchaseFixedPurchaseEveryNWeek_OnCheckedChanged(object sender, EventArgs e)
	{
		rbShippingPattern_OnCheckedChanged(Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY);
	}

	/// <summary>
	/// 配送先情報変更フォーム表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayUserShippingInfoForm_Click(object sender, EventArgs e)
	{
		this.IsUserShippingModify = (this.IsUserShippingModify == false);
		// 配送先変更?
		if (this.IsUserShippingModify)
		{
			var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
			var shipping = input.Shippings[0];
			// 値をセット
			this.WtbShippingName1.Text = shipping.ShippingName1;
			this.WtbShippingName2.Text = shipping.ShippingName2;
			this.WtbShippingNameKana1.Text = shipping.ShippingNameKana1;
			this.WtbShippingNameKana2.Text = shipping.ShippingNameKana2;
			foreach (ListItem li in this.WddlShippingCountry.Items)
			{
				li.Selected = (li.Value == shipping.ShippingAddrCountryIsoCode);
			}
			foreach (ListItem li in this.WddlShippingAddr1.Items)
			{
				li.Selected = (li.Value == shipping.ShippingAddr1);
			}
			this.WtbShippingAddr2.Text = shipping.ShippingAddr2;
			this.WtbShippingAddr3.Text = shipping.ShippingAddr3;
			this.WtbShippingAddr4.Text = shipping.ShippingAddr4;

			foreach (ListItem li in this.WddlShippingAddr5.Items)
			{
				li.Selected = (li.Value == shipping.ShippingAddr5);
			}
			this.WtbShippingAddr5.Text = shipping.ShippingAddr5;
			this.WtbShippingCompanyName.Text = shipping.ShippingCompanyName;
			this.WtbShippingCompanyPostName.Text = shipping.ShippingCompanyPostName;

			if (GlobalAddressUtil.IsCountryJp(GetShippingCountryIsoCode()))
			{
				// Set value for telephone
				SetTelTextbox(
					this.WtbShippingTel,
					this.WtbShippingTel1,
					this.WtbShippingTel2,
					this.WtbShippingTel3,
					shipping.ShippingTel1);

				if (string.IsNullOrEmpty(shipping.ShippingZip) == false)
				{
					// Set value for zip code
					SetZipCodeTextbox(
						this.WtbShippingZip,
						this.WtbShippingZip1,
						this.WtbShippingZip2,
						shipping.ShippingZip);
				}
			}
			else
			{
				this.WtbShippingZipGlobal.Text = shipping.ShippingZip;
				this.WtbShippingTel1Global.Text = shipping.ShippingTel1;
			}

			foreach (ListItem li in this.WddlShippingMethod.Items)
			{
				li.Selected = (li.Value == shipping.ShippingMethod);
			}

			foreach (ListItem li in this.WddlDeliveryCompany.Items)
			{
				li.Selected = (li.Value == shipping.DeliveryCompanyId);
			}

			if (this.DeliveryCompany.IsValidShippingTimeSetFlg)
			{
				foreach (ListItem li in this.WddlShippingTime.Items)
				{
					if (shipping.ShippingTime == li.Value)
					{
						li.Selected = true;
					}
				}
			}

			// 国切替初期化
			ddlShippingCountry_SelectedIndexChanged(sender, e);

			// Display Taiwan Address
			DisplayTaiwanAddress();
			// Reset selected value in ddlShippingType
			this.WddlShippingType.SelectedValue = IsDisplayButtonConvenienceStore(this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg)
				? CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE
				: CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW;

			ddlShippingType_SelectedIndexChanged(null, null);

			// Display shipping receiving store type for EcPay
			var shippingReceivingStoreFlg = this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg;
			var shippingReceivingStoreType = this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreType;
			if (IsDisplayButtonConvenienceStore(shippingReceivingStoreFlg)
				&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
				&& (string.IsNullOrEmpty(shippingReceivingStoreType) == false))
			{
				this.WddlShippingReceivingStoreType.SelectedValue = shipping.ShippingReceivingStoreType;
			}
		}
	}

	/// <summary>
	/// 配送先国ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void ddlShippingCountry_SelectedIndexChanged(object sender, EventArgs e)
	{
		ddlShippingCountry_SelectedIndexChangedInner(GetDefaultMasterContentPlaceHolder());
	}

	/// <summary>
	/// 「定期購入を再開する」ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbResumeFixedPurchase_Click(object sender, EventArgs e)
	{
		//次回配送日・次々回配送日を計算
		var fixedPurchaseService = new FixedPurchaseService();
		var calculateMode = fixedPurchaseService.GetCalculationMode(
			this.FixedPurchaseContainer.FixedPurchaseKbn,
			Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
		DateTime selectedShippingDate;
		var nextShippingDate = DateTime.TryParse(WddlResumeFixedPurchaseDate.SelectedValue, out selectedShippingDate)
			? selectedShippingDate
			: fixedPurchaseService.CalculateNextShippingDate(this.FixedPurchaseContainer.FixedPurchaseKbn,
				this.FixedPurchaseContainer.FixedPurchaseSetting1,
				null, this.ShopShipping.FixedPurchaseShippingDaysRequired,
				this.ShopShipping.FixedPurchaseMinimumShippingSpan,
				calculateMode);
		var nextNextShippingDate = fixedPurchaseService.CalculateNextShippingDate(this.FixedPurchaseContainer.FixedPurchaseKbn,
			this.FixedPurchaseContainer.FixedPurchaseSetting1,
			nextShippingDate,
			this.ShopShipping.FixedPurchaseShippingDaysRequired,
			this.ShopShipping.FixedPurchaseMinimumShippingSpan,
			calculateMode);

		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		var shipping = input.Shippings[0];
		var address = (Constants.TW_COUNTRY_SHIPPING_ENABLE
				&& IsCountryTw(shipping.ShippingAddrCountryIsoCode))
			? shipping.ShippingAddr2
			: shipping.ShippingAddr1;

		// 配送希望日に配送可能か計算
		if (OrderCommon.CanCalculateScheduledShippingDate(
			this.ShopId,
			nextShippingDate,
			string.Empty,
			this.DeliveryCompany.DeliveryCompanyId,
			shipping.ShippingAddrCountryIsoCode,
			address,
			shipping.ShippingZip.Replace("-", string.Empty)) == false)
		{
			if (this.WddlResumeFixedPurchaseDate.SelectedValue != ReplaceTag("@@DispText.shipping_date_list.none@@"))
			{
				// エラーメッセージ表示
				this.WdvResumeFixedPurchaseErr.Visible = true;
				this.WdvResumeFixedPurchaseErr.InnerHtml = WebMessages
					.GetMessages(WebMessages.ERRMSG_SHIPPINGDATE_INVALID).Replace(
						"@@ 1 @@",
						DateTimeUtility.ToStringFromRegion(
							HolidayUtil.GetShortestDeliveryDate(
								this.ShopId,
								this.DeliveryCompany.DeliveryCompanyId,
								address,
								shipping.ShippingZip.Replace("-", string.Empty)),
							DateTimeUtility.FormatType.LongDateWeekOfDay2Letter));
				return;
			}

			nextShippingDate = nextNextShippingDate;
			while (OrderCommon.CanCalculateScheduledShippingDate(
				this.ShopId,
				nextShippingDate,
				string.Empty,
				this.DeliveryCompany.DeliveryCompanyId,
				shipping.ShippingAddrCountryIsoCode,
				address,
				shipping.ShippingZip.Replace("-", string.Empty)) == false)
			{
				nextShippingDate = fixedPurchaseService.CalculateNextShippingDate(
					this.FixedPurchaseContainer.FixedPurchaseKbn,
					this.FixedPurchaseContainer.FixedPurchaseSetting1,
					nextShippingDate,
					this.ShopShipping.FixedPurchaseShippingDaysRequired,
					this.ShopShipping.FixedPurchaseMinimumShippingSpan,
					calculateMode);
			}

			nextNextShippingDate = fixedPurchaseService.CalculateNextShippingDate(
				this.FixedPurchaseContainer.FixedPurchaseKbn,
				this.FixedPurchaseContainer.FixedPurchaseSetting1,
				nextShippingDate,
				this.ShopShipping.FixedPurchaseShippingDaysRequired,
				this.ShopShipping.FixedPurchaseMinimumShippingSpan,
				calculateMode);
		}

		// 定期購入再開（更新履歴とともに）
		fixedPurchaseService
			.Resume(
				this.FixedPurchaseContainer.FixedPurchaseId,
				this.FixedPurchaseContainer.UserId,
				Constants.FLG_LASTCHANGED_USER,
				nextShippingDate,
				nextNextShippingDate,
				UpdateHistoryAction.Insert);

		AccountManager.RestoreForFixedPurchaseCancel(this.FixedPurchaseContainer);

		// 完了メッセージをセット
		this.ResumeFixedPurchaseMessageHtmlEncoded = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RESUMED_FIXED_PURCHASE);

		// 定期購入再開メール送信
		ResumeFixedPurchaseSendMail(nextShippingDate, nextNextShippingDate);

		// 同じ画面で画面遷移(※再描画）
		Response.Redirect(Request.Url.AbsolutePath + "?" + Constants.REQUEST_KEY_FIXED_PURCHASE_ID + "=" + Request[Constants.REQUEST_KEY_FIXED_PURCHASE_ID]);
	}

	/// <summary>
	/// 「頒布会コースを再開する」ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbResumeSubscriptionBox_Click(object sender, EventArgs e)
	{
		//次回配送日・次々回配送日を計算
		DateTime selectedShippingDate;
		var nextShippingDate = DateTime.TryParse(WddlResumeSubscriptionBoxDate.SelectedValue, out selectedShippingDate)
			? selectedShippingDate
			: new FixedPurchaseService().CalculateNextShippingDate(
				this.FixedPurchaseContainer.FixedPurchaseKbn,
				this.FixedPurchaseContainer.FixedPurchaseSetting1,
				null,
				this.ShopShipping.FixedPurchaseShippingDaysRequired,
				this.ShopShipping.FixedPurchaseMinimumShippingSpan,
				Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
		var nextNextShippingDate = new FixedPurchaseService().CalculateNextShippingDate(
			this.FixedPurchaseContainer.FixedPurchaseKbn,
			this.FixedPurchaseContainer.FixedPurchaseSetting1,
			nextShippingDate,
			this.ShopShipping.FixedPurchaseShippingDaysRequired,
			this.ShopShipping.FixedPurchaseMinimumShippingSpan,
			Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);

		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		var shipping = input.Shippings[0];
		var address = (Constants.TW_COUNTRY_SHIPPING_ENABLE && IsCountryTw(shipping.ShippingAddrCountryIsoCode))
			? shipping.ShippingAddr2
			: shipping.ShippingAddr1;

		// 配送希望日に配送可能か計算
		var canCalculateScheduledShippingDate = OrderCommon.CanCalculateScheduledShippingDate(
			this.ShopId,
			nextShippingDate,
			string.Empty,
			this.DeliveryCompany.DeliveryCompanyId,
			shipping.ShippingAddrCountryIsoCode,
			address,
			shipping.ShippingZip.Replace("-", string.Empty));
		if (canCalculateScheduledShippingDate == false)
		{
			if (this.WddlResumeSubscriptionBoxDate.SelectedValue != ReplaceTag("@@DispText.shipping_date_list.none@@"))
			{
				// エラーメッセージ表示
				this.WdvResumeFixedPurchaseErr.Visible = true;
				this.WdvResumeFixedPurchaseErr.InnerHtml = WebMessages
					.GetMessages(WebMessages.ERRMSG_SHIPPINGDATE_INVALID).Replace(
						"@@ 1 @@",
						DateTimeUtility.ToStringFromRegion(
							HolidayUtil.GetShortestDeliveryDate(
								this.ShopId,
								this.DeliveryCompany.DeliveryCompanyId,
								address,
								shipping.ShippingZip.Replace("-", string.Empty)),
							DateTimeUtility.FormatType.LongDateWeekOfDay2Letter));
				return;
			}

			nextShippingDate = nextNextShippingDate;
			while (OrderCommon.CanCalculateScheduledShippingDate(
				this.ShopId,
				nextShippingDate,
				string.Empty,
				this.DeliveryCompany.DeliveryCompanyId,
				shipping.ShippingAddrCountryIsoCode,
				address,
				shipping.ShippingZip.Replace("-", string.Empty)) == false)
			{
				nextShippingDate = new FixedPurchaseService().CalculateNextShippingDate(
					this.FixedPurchaseContainer.FixedPurchaseKbn,
					this.FixedPurchaseContainer.FixedPurchaseSetting1,
					nextShippingDate,
					this.ShopShipping.FixedPurchaseShippingDaysRequired,
					this.ShopShipping.FixedPurchaseMinimumShippingSpan,
					Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
			}

			nextNextShippingDate = new FixedPurchaseService().CalculateNextShippingDate(
				this.FixedPurchaseContainer.FixedPurchaseKbn,
				this.FixedPurchaseContainer.FixedPurchaseSetting1,
				nextShippingDate,
				this.ShopShipping.FixedPurchaseShippingDaysRequired,
				this.ShopShipping.FixedPurchaseMinimumShippingSpan,
				Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
		}

		// 定期購入再開（更新履歴とともに）
		new FixedPurchaseService().Resume(
				this.FixedPurchaseContainer.FixedPurchaseId,
				this.FixedPurchaseContainer.UserId,
				Constants.FLG_LASTCHANGED_USER,
				nextShippingDate,
				nextNextShippingDate,
				UpdateHistoryAction.Insert);

		// 完了メッセージをセット
		this.ResumeFixedPurchaseMessageHtmlEncoded = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RESUMED_FIXED_PURCHASE);

		// 定期購入再開メール送信
		ResumeFixedPurchaseSendMail(nextShippingDate, nextNextShippingDate);

		// 同じ画面で画面遷移(※再描画）
		Response.Redirect(Request.Url.AbsolutePath + "?" + Constants.REQUEST_KEY_FIXED_PURCHASE_ID + "=" + Request[Constants.REQUEST_KEY_FIXED_PURCHASE_ID]);
	}

	/// <summary>
	/// 定期購入再開メール送信
	/// </summary>
	/// <param name="nextShippingDate">次回配送日</param>
	/// <param name="nextNextShippingDate">次々回配送日</param>
	private void ResumeFixedPurchaseSendMail(DateTime nextShippingDate, DateTime nextNextShippingDate)
	{
		// メールデータ設定
		var tagPrameter = new MailTemplateDataCreaterForFixedPurchase(true).GetFixedPurchaseMailDatas(this.FixedPurchaseContainer.FixedPurchaseId);

		// メール送信処理
		using (var mailSender = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID
			, Constants.CONST_MAIL_ID_RESUME_FIXEDPURCHASE
			, this.LoginUserId
			, tagPrameter
			, true
			, Constants.MailSendMethod.Auto
			, RegionManager.GetInstance().Region.LanguageCode
			, RegionManager.GetInstance().Region.LanguageLocaleId
			, this.LoginUserMail))
		{
			mailSender.AddTo(this.LoginUserMail);

			// 送信
			if (mailSender.SendMail() == false)
			{
				// エラーログ出力
				AppLogger.WriteError(this.GetType().BaseType.ToString() + " : " + mailSender.MailSendException.ToString());
			}
		}
	}

	/// <summary>
	/// 配送再開日の「キャンセル」ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbResumeFixedPurchaseCancel_Click(object sender, EventArgs e)
	{
		WcbResumeFixedPurchase.Checked = false;
		WdvResumeFixedPurchaseDate.Visible = WcbResumeFixedPurchase.Checked;
	}

	/// <summary>
	/// 「住所検索」ボタン押下イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void lbSearchShippingAddr_Click(object sender, System.EventArgs e)
	{
		this.ZipInputErrorMessage = SearchZipCode(sender, this.UnavailableShippingZip);
	}

	/// <summary>
	/// 「利用ポイント変更」ボタン押下イベント（利用ポイントの更新フォーム表示）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayUpdateNextShippingUsePointForm_Click(object sender, System.EventArgs e)
	{
		this.WdvNextShippingUsePoint.Visible = (this.WdvNextShippingUsePoint.Visible == false);
		if (this.WdvNextShippingUsePoint.Visible)
		{
			WtbNextShippingUsePoint.Text = this.FixedPurchaseContainer.NextShippingUsePoint.ToString();
		}
		// 全ポイント継続利用フラグにチェックがついている場合は0ptで表示する
		if (this.WcbUseAllPointFlg.Checked && this.CanUseAllPointFlg)
		{
			WtbNextShippingUsePoint.Text = "0";
		}
	}

	/// <summary>
	/// 利用ポイント変更フォーム非表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCloseUpdateNextShippingUsePointForm_Click(object sender, System.EventArgs e)
	{
		this.WdvNextShippingUsePoint.Visible = false;
	}

	/// <summary>
	/// 「利用ポイント更新」ボタン押下イベント（利用ポイントをDBに更新）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdateNextShippingUsePoint_Click(object sender, System.EventArgs e)
	{
		// ポイントOPが無効、または、ユーザではない場合、何も処理しない
		if ((Constants.W2MP_POINT_OPTION_ENABLED == false) || (IsLoggedIn == false)) return;

		// カスタムバリデータ取得（正常完了でも次の画面に遷移しないためエラー初期化）
		var input = new Hashtable()
		{{
			Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_USE_POINT, WtbNextShippingUsePoint.Text
		}};
		var errorMessages = Validator.ValidateAndGetErrorContainer("FixedPurchaseModifyInput", input);
		if (errorMessages.Count != 0)
		{
			// カスタムバリデータ取得
			var customValidators = new List<CustomValidator>();
			CreateCustomValidators(this, customValidators);
			// エラーをカスタムバリデータへ
			SetControlViewsForError("FixedPurchaseModifyInput", errorMessages, customValidators);
			return;
		}

		// 次回購入の利用ポイントの更新をエラーチェック
		var usePoint = decimal.Parse(WtbNextShippingUsePoint.Text);
		var useAllPointFlg = WcbUseAllPointFlg.Checked
			? Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON
			: Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_OFF;

		this.NextShippingFixedPurchaseCart = CreateSimpleCartListForFixedPurchase(this.FixedPurchaseContainer.FixedPurchaseId).Items[0];

		var result = NextShippingUsePointUpdateErrorCheck.CheckNextShippingUsePoint(
			this.FixedPurchaseContainer,
			usePoint,
			useAllPointFlg,
			this.NextShippingFixedPurchaseCart.PriceSubtotalForCampaign);

		if (result != string.Empty)
		{
			this.NextShippingUsePointErrorMessage = result;
			return;
		}

		// 次回購入の利用ポイントの更新を実行
		var success = false;
		// 全ポイント継続利用に更新がある場合は、フラグの更新も行う
		if (this.CanUseAllPointFlg && (this.FixedPurchaseContainer.UseAllPointFlg != useAllPointFlg))
		{
			success = new FixedPurchaseService().ApplyNextShippingUseAllPointChange(
				Constants.CONST_DEFAULT_DEPT_ID,
				this.FixedPurchaseContainer,
				usePoint,
				useAllPointFlg,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);
		}
		else
		{
			success = new FixedPurchaseService().ApplyNextShippingUsePointChange(
				Constants.CONST_DEFAULT_DEPT_ID,
				this.FixedPurchaseContainer,
				usePoint,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);
		}

		if (success)
		{
			// メール送信
			SendMailCommon.SendModifyFixedPurchaseMail(this.FixedPurchaseContainer.FixedPurchaseId, SendMailCommon.FixedPurchaseModify.OrderPoint);

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				var point = (this.FixedPurchaseContainer.NextShippingUsePoint - usePoint);
				CrossPointUtility.UpdateCrossPointApiWithWebErrorMessage(
					this.LoginUser,
					point,
					CrossPointUtility.GetValue(
						Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID,
						w2.App.Common.Constants.CROSS_POINT_REASON_KBN_MODIFY));

				UserUtility.AdjustPointAndMemberRankByCrossPointApi(this.LoginUser);
			}

			// 再表示
			this.LoginUserPoint = PointOptionUtility.GetUserPoint((string)this.LoginUserId);
			Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
		}
		else
		{
			// エラー画面へ
			this.NextShippingUsePointErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NEXT_SHIPPING_USE_POINT_UPDATE_ALERT);
		}
	}

	/// <summary>
	/// 「今すぐ注文」ボタン押下イベント（定期注文生成）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnFixedPurchaseOrder_Click(object sender, EventArgs e)
	{
		// 頒布会コースに紐づく定期台帳だが、対象の頒布会が削除されていた場合は定期購入ステータスをその他エラーにして注文を生成しない
		var existSubscriptionBox = CartObject.CheckExistFixedPurchaseLinkSubscriptionBox(
			this.FixedPurchaseContainer.SubscriptionBoxCourseId,
			this.FixedPurchaseContainer.FixedPurchaseId,
			Constants.FLG_LASTCHANGED_USER);

		// 定期台帳に設定されている商品が、紐づいている頒布会コース内の商品に設定されていない場合は定期購入ステータスをその他エラーにして注文を生成しない
		var isValidFixedPurchaseItem = new SubscriptionBoxService().CheckAllItemInSubscriptionBoxItem(
			this.FixedPurchaseContainer.SubscriptionBoxCourseId,
			this.FixedPurchaseContainer.FixedPurchaseId,
			Constants.FLG_LASTCHANGED_USER);

		if ((existSubscriptionBox == false) || (isValidFixedPurchaseItem == false))
		{
			this.OrderNowMessagesHtmlEncoded = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FIXEDPURCHASEORDER_ERROR);
			Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
		}

		var deviceInfo = this.Request[Constants.REQUEST_GMO_DEFERRED_DEVICE_INFO];

		// 注文登録実行
		var orderRegisterFixedPurchase = new OrderRegisterFixedPurchase(
			Constants.FLG_LASTCHANGED_USER,
			true,
			this.WcbUpdateNextShippingDate.Checked,
			new FixedPurchaseMailSendTiming(""));
		var orderRegister = orderRegisterFixedPurchase.ExecByOrderNowFromMyPage(this.FixedPurchaseContainer.FixedPurchaseId, deviceInfo);

		// 生成された注文IDリストをセッションに格納
		this.RegisteredOrderIds = orderRegister.SuccessOrders.Select(order => (string)order[Constants.FIELD_ORDER_ORDER_ID]).ToList();

		// スコア後払いの与信中のメッセージ対応
		if ((this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& (Constants.PAYMENT_CVS_DEF_KBN == w2.App.Common.Constants.PaymentCvsDef.Score)
			&& orderRegister.SuccessOrders.Any(order => (string)order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST))
		{
			orderRegister.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_SCORE_CVSDEFAUTH_ERROR));
		}

		// ベリトランス後払いの与信中のメッセージ対応
		if ((this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
			&& (Constants.PAYMENT_CVS_DEF_KBN == w2.App.Common.Constants.PaymentCvsDef.Veritrans)
			&& orderRegister.SuccessOrders.Any(order => (string)order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST))
		{
			orderRegister.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_VERITRANS_CVSDEFAUTH_ERROR));
		}

		// YamatoKwcクレジットが最後の注文から1年以上たち期限切れになっているか
		if (orderRegister.IsExpiredYamatoKwcCredit)
		{
			orderRegister.ErrorMessages.Add(
				WebSanitizer.HtmlEncode(
					WebMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_YAMATO_KWC_CREDIT_EXPIRED_FOR_FIXED_PURCHASE_DETAIL_ERROR)));
		}

		// エラーメッセージを表示
		if ((orderRegister.AlertMessages.Count > 0) || (orderRegister.ErrorMessages.Count > 0))
		{
			var errorMessages = new List<string>(orderRegister.ErrorMessages);
			
			// 定期購入ステータスは在庫切れ以外かつ決済失敗以外かつ配送不可以外の場合は、エラーメッセージに下記の文言を追加
			if ((orderRegister.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NOSTOCK)
				&& (orderRegister.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_PAYMENTFAILED)
				&& (orderRegister.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_UNAVAILABLE_SHIPPING_AREA))
			{
				errorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_CONTACT_WITH_OPERATOR));
			}

			errorMessages.AddRange(orderRegister.AlertMessages);
			this.OrderNowMessagesHtmlEncoded = StringUtility.ChangeToBrTag(WebSanitizer.HtmlEncode(string.Join("\r\n", errorMessages.ToArray())));
		}
		else if (orderRegister.YamatoKaSmsOrders.Count > 0)
		{
			var wdivSkipedMessage = GetWrappedControl<WrappedHtmlGenericControl>("divSkipedMessage");
			wdivSkipedMessage.Visible = true;
			return;
		}
		else if (string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId) == false)
		{
			var processedFixedPurchase = new FixedPurchaseService().Get(this.FixedPurchaseContainer.FixedPurchaseId);
			if (processedFixedPurchase.FixedPurchaseStatus == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL)
			{
				// 最新のユーザポイントを取得
				this.LoginUserPoint = PointOptionUtility.GetUserPoint((string)this.LoginUserId);
				// メール送信
				SendMailCommon.SendModifyFixedPurchaseMail(this.FixedPurchaseContainer.FixedPurchaseId, SendMailCommon.FixedPurchaseModify.OrderCancell);
			}
		}

		if (string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId))
		{
			Response.Redirect(
				PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
		}

		var subscriptionBox = new SubscriptionBoxService().GetByCourseId(this.FixedPurchaseContainer.SubscriptionBoxCourseId);

		// 頒布会コースが回数指定で次回以降のデフォルト注文がない場合定期購入ステータスを完了にする
		if ((subscriptionBox.IsNumberTime)
			&& (subscriptionBox.IsAutoRenewal == false)
			&& (subscriptionBox.IsIndefinitePeriod == false))
		{
			var maxCount = subscriptionBox.DefaultOrderProducts.Max(defaultItem => defaultItem.Count).Value;
			if (this.FixedPurchaseContainer.SubscriptionBoxOrderCount >= maxCount)
			{
				new FixedPurchaseService().Complete(
					this.FixedPurchaseContainer.FixedPurchaseId,
					Constants.FLG_LASTCHANGED_USER,
					this.FixedPurchaseContainer.NextShippingDate,
					this.FixedPurchaseContainer.NextNextShippingDate,
					UpdateHistoryAction.Insert,
					null);
			}
		}

		// 頒布会コースが期間指定で次回以降のデフォルト注文がない場合定期購入ステータスを完了にする
		if (subscriptionBox.IsNumberTime == false)
		{
			var lastDate = subscriptionBox.DefaultOrderProducts.Max(dp => dp.TermUntil);
			if ((lastDate != null) && (lastDate < this.FixedPurchaseContainer.NextShippingDate))
			{
				new FixedPurchaseService().Complete(
					this.FixedPurchaseContainer.FixedPurchaseId,
					Constants.FLG_LASTCHANGED_USER,
					this.FixedPurchaseContainer.NextShippingDate,
					this.FixedPurchaseContainer.NextNextShippingDate,
					UpdateHistoryAction.Insert,
					null);
			}
		}

		// 再表示
		Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
	}

	/// <summary>
	/// お支払い方法変更ボタンのクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayInputOrderPaymentKbn_Click(object sender, EventArgs e)
	{
		this.WdvOrderPaymentPattern.Visible = (this.WdvOrderPaymentPattern.Visible == false);

		// 決済種別ラジオボタンクリックイベント
		if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB)
		{
			var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(this.PaymentRepeaterItem ?? this.CreditRepeaterItem, "rbgPayment");
			if (wrbgPayment.InnerControl != null) rbgPayment_OnCheckedChanged(wrbgPayment, EventArgs.Empty);
		}
		else if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL)
		{
			var wddlPayment = GetWrappedControl<WrappedDropDownList>(
				this.PaymentRepeaterItem == null
					? this.CreditRepeaterItem == null ? null : this.CreditRepeaterItem.Parent.Parent
					: this.PaymentRepeaterItem.Parent.Parent,
				"ddlPayment");
			if (wddlPayment.InnerControl != null) rbgPayment_OnCheckedChanged(wddlPayment, EventArgs.Empty);
		}
	}

	/// <summary>
	/// 支払い方法 キャンセルボタンのクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnClosePaymentPatternInfo_Click(object sender, EventArgs e)
	{
		this.WdvOrderPaymentPattern.Visible = (this.WdvOrderPaymentPattern.Visible == false);
		this.SelectedDeliveryCompany = this.DeliveryCompany;
	}

	/// <summary>
	/// 支払い方法 更新ボタンのクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdatePaymentPatternInfo_Click(object sender, EventArgs e)
	{
		var cart = CreateSimpleCartListForFixedPurchase(this.FixedPurchaseContainer.FixedPurchaseId).Items[0];

		//支払い方法の取得
		StringBuilder paymentErrorMessage;
		var paymentModel = GetAndValidatePaymentInput(cart, out paymentErrorMessage);
		if (paymentErrorMessage.Length > 0 || paymentModel == null)
		{
			this.WsErrorMessagePayment.InnerHtml = paymentErrorMessage.ToString();
			return;
		}

		// 継続課金（定期・従量）解約を行う
		string apiError;
		if (FixedPurchaseHelper.CancelPaymentContinuous(
			this.FixedPurchaseContainer.FixedPurchaseId,
			this.FixedPurchaseContainer.OrderPaymentKbn,
			this.FixedPurchaseContainer.ExternalPaymentAgreementId,
			Constants.FLG_LASTCHANGED_USER,
			out apiError) == false)
		{
			this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(
				WebMessages.ERRMSG_FRONT_PAYMENT_CONTINUOUS_CANCEL_NG_FOR_CHANGE_PAYMENT_FP);
			return;
		}

		var installmentsCode = "";
		int? branchNo = null;
		if (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			var creditErrorMessage = "";
			var apiErrorMessage = "";

			var userCreditCardInput = CreditCardProcessing(
				Constants.CREDITCARD_UNREGIST_FIXEDPURCHASE_DISPLAY_NAME,
				out creditErrorMessage,
				out apiErrorMessage,
				UpdateHistoryAction.DoNotInsert);

			//支払い回数を設定
			installmentsCode = (IsNewCreditCard())
				? Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten
					? GetWrappedControl<WrappedDropDownList>(this.RakutenCreditCardControll, "dllCreditInstallmentsRakuten").SelectedValue
					: this.WciCardInputs.WddlInstallments.SelectedValue
				: this.WciCardInputs.WdllCreditInstallments2.SelectedValue;

			if ((userCreditCardInput == null) || (string.IsNullOrEmpty(apiErrorMessage) == false))
			{
				new FixedPurchaseService().UpdateForCreditRegisterFail(
					this.FixedPurchaseContainer.FixedPurchaseId,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert,
					apiErrorMessage);
			}

			if (string.IsNullOrEmpty(creditErrorMessage) == false)
			{
				var wspanErrorMessageForCreditCard = GetWrappedControl<WrappedHtmlGenericControl>(
					this.CreditRepeaterItem,
					"spanErrorMessageForCreditCard");
				if (wspanErrorMessageForCreditCard.HasInnerControl)
				{
					wspanErrorMessageForCreditCard.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(creditErrorMessage);
					wspanErrorMessageForCreditCard.InnerControl.Style["display"] = "block";
				}
				return;
			}

			branchNo = int.Parse(userCreditCardInput.BranchNo);

			// 新規クレカでリンク式決済の場合は外部サイトへ遷移
			var wcbRegistCreditCard = GetWrappedControl<WrappedCheckBox>(this.CreditRepeaterItem, "cbRegistCreditCard");
			if (IsNewCreditCard() && this.IsCreditCardLinkPayment())
			{
				Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_PAYMENT_ZEUS_LINK_POINT_GET_NOTICE;

				var param = new Hashtable
				{
					{"is_card_register", ((wcbRegistCreditCard.InnerControl != null) && wcbRegistCreditCard.Checked)},
					{Constants.FIELD_FIXEDPURCHASE_CREDIT_BRANCH_NO, branchNo},
				};
				Session[Constants.SESSION_KEY_PARAM] = param;

				var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_PAYMENT_ZEUS_LINK_POINT_POST)
					.AddParam(Constants.REQUEST_KEY_PAYMENTCREDITCARD_ACTION_TYPE, Constants.ActionTypes.RegisterFixedPurchaseCreditCard.ToString())
					.AddParam(Constants.REQUEST_KEY_FIXED_PURCHASE_ID, this.FixedPurchaseContainer.FixedPurchaseId)
					.CreateUrl();

				Response.Redirect(url);
			}

			//支払い回数を設定
			installmentsCode = (IsNewCreditCard())
				? Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten
					? GetWrappedControl<WrappedDropDownList>(this.RakutenCreditCardControll, "dllCreditInstallmentsRakuten").SelectedValue
					: this.WciCardInputs.WddlInstallments.SelectedValue
				: this.WciCardInputs.WdllCreditInstallments2.SelectedValue;

			// クレジットカード決済与信成功更新
			new FixedPurchaseService().UpdateForAuthSuccess(
				this.FixedPurchaseContainer.FixedPurchaseId,
				branchNo.Value,
				installmentsCode,
				Constants.FLG_LASTCHANGED_USER,
				string.Format(
					ReplaceTag("@@DispText.external_payment_cooperation_log.fixedpurchase_update_For_auth_success@@"),
					this.FixedPurchaseContainer.FixedPurchaseId,
					paymentModel.PaymentId,
					paymentModel.PaymentName),
				UpdateHistoryAction.DoNotInsert);

			//クレジットカードを登録リストに表示させる
			if (IsNewCreditCard() && wcbRegistCreditCard.InnerControl != null && wcbRegistCreditCard.Checked)
			{
				new UserCreditCardService().UpdateDispFlg(
					this.FixedPurchaseContainer.UserId,
					branchNo.Value,
					true,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);
			}
		}

		// AmazonPayでは支払い方法と配送先を同時に更新する
		var externalPaymentAgreementId = string.Empty;
		if (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
		{
			// 配送先情報にエラーがある場合return
			var addressErrorMessage = (string)Session[AmazonConstants.SESSION_KEY_AMAZON_ADDRESS_ERROR_MSG];
			if (string.IsNullOrEmpty(addressErrorMessage) == false) return;

			// 変更後の支払い契約ID取得
			externalPaymentAgreementId = this.WhfAmazonBillingAgreementId.Value;

			if (string.IsNullOrEmpty(externalPaymentAgreementId))
			{
				this.WsErrorMessagePayment.InnerHtml =
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_LOGIN_FOR_AMAZON);
				return;
			}

			// 支払契約を設定
			var sbad = AmazonApiFacade.SetBillingAgreementDetails(externalPaymentAgreementId);
			if (sbad.GetSuccess() == false)
			{
				this.WsErrorMessagePayment.InnerHtml =
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION);

				new FixedPurchaseService().UpdateForCreditRegisterFail(
					this.FixedPurchaseContainer.FixedPurchaseId,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert,
					LogCreator.CreateErrorMessage(sbad.GetErrorCode(), sbad.GetErrorMessage()));
				return;
			}

			if (sbad.GetConstraintIdList().Any())
			{
				var messages = sbad.GetConstraintIdList().Select(
					constraintId => AmazonApiMessageManager.GetBillingAgreementConstraintMessage(constraintId));
				this.WhgcConstraintErrorMessage.InnerHtml = WebSanitizer.HtmlEncodeChangeToBr(string.Join("\r\n", messages));
				return;
			}

			// 支払契約を承認
			var coba = AmazonApiFacade.ConfirmBillingAgreement(externalPaymentAgreementId);
			if (coba.GetSuccess() == false)
			{
				this.WsErrorMessagePayment.InnerHtml =
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION);

				new FixedPurchaseService().UpdateForCreditRegisterFail(
					this.FixedPurchaseContainer.FixedPurchaseId,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert,
					LogCreator.CreateErrorMessage(coba.GetErrorCode(), coba.GetErrorMessage()));
				return;
			}

			// ウィジェットから住所情報取得
			var amazonModel = (AmazonModel)Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
			var token = amazonModel.Token;
			var res = AmazonApiFacade.GetBillingAgreementDetails(externalPaymentAgreementId, token);
			var input = new AmazonAddressInput(res);
			var address = AmazonAddressParser.Parse(input);

			// 配送先更新
			var deliveryCompanyIdForAmazonPay = WddlDeliveryCompanyForAmazonPay.HasInnerControl
				? this.WddlDeliveryCompanyForAmazonPay.SelectedValue
				: this.DeliveryCompany.DeliveryCompanyId;
			ExecuteUpdatingShippingWithAmazonAddress(
				address,
				this.WddlShippingMethodForAmazonPay.SelectedValue,
				deliveryCompanyIdForAmazonPay,
				this.WddlShippingTimeForAmazonPay.SelectedValue);
		}
		// AmazonPay(CV2)
		else if (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
		{
			if (string.IsNullOrEmpty(this.AmazonCheckoutSessionId))
			{
				this.WsErrorMessagePayment.InnerHtml = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_LOGIN_FOR_AMAZON);
				return;
			}

			var amazonCallbackPath = AmazonCv2ApiFacade.CreateCallbackPathForFixedPurchase(
				AmazonCv2Constants.AMAZON_ACTION_STATUS_AUTH,
				this.FixedPurchaseContainer.FixedPurchaseId);

			var checkoutSession = this.AmazonFacade.UpdateCheckoutSessionForFixedPurchase(
				this.AmazonCheckoutSessionId,
				amazonCallbackPath,
				this.FixedPurchaseContainer.Shippings.First().Items.Sum(i => i.Price),
				this.FixedPurchaseContainer.FixedPurchaseId,
				this.FixedPurchaseContainer.FixedPurchaseKbn,
				this.FixedPurchaseContainer.FixedPurchaseSetting1);

			var checkoutSessionError = AmazonCv2ApiFacade.GetErrorCodeAndMessage(checkoutSession);

			if (checkoutSession.Success == false)
			{
				this.WsErrorMessagePayment.InnerHtml =
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_EXCEPTION);

				new FixedPurchaseService().UpdateForCreditRegisterFail(
					this.FixedPurchaseContainer.FixedPurchaseId,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert,
					LogCreator.CreateErrorMessage(checkoutSessionError.ReasonCode, checkoutSessionError.Message));
				return;
			}

			// Amazon配送先住所を保持
			var input = new AmazonAddressInput(this.AmazonCheckoutSession.ShippingAddress);
			SessionManager.AmazonShippingAddress = AmazonAddressParser.Parse(input);

			Response.Redirect(checkoutSession.WebCheckoutDetails.AmazonPayRedirectUrl);
		}
		// PayPal決済
		else if (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
		{
			if (this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
			{
				branchNo = this.FixedPurchaseContainer.CreditBranchNo;
			}
			else if (SessionManager.PayPalCooperationInfo != null)
			{
				var userCreditCard = PayPalUtility.Payment.RegisterAsUserCreditCard(
					this.LoginUserId,
					SessionManager.PayPalCooperationInfo,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.Insert);
				branchNo = userCreditCard.BranchNo;
			}
			else
			{
				this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYPAL_NEEDS_LOGIN_ERROR);
				return;
			}
		}
		else if (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
		{
			if (this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			{
				branchNo = this.FixedPurchaseContainer.CreditBranchNo;
			}
			else
			{
				var paidyTokenId = this.WhfPaidyTokenId.Value;
				if (string.IsNullOrEmpty(paidyTokenId) == false)
				{
					var userCredit = new UserCreditCardService().GetByCooperationId1(paidyTokenId);
					if (PaidyUtility.IsTokenIdExist(paidyTokenId))
					{
						this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(
							WebMessages.ERRMSG_FRONT_PAIDY_TOKEN_ID_EXISTED_ERROR).Replace("@@ 1 @@", paidyTokenId);
						return;
					}

					var userCreditCard = PaidyUtility.RegisterAsUserCreditCard(
						this.LoginUserId,
						paidyTokenId,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.Insert);
					branchNo = userCreditCard.BranchNo;
				}
				else
				{
					this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAIDY_GET_TOKEN_ERROR);
					return;
				}
			}
		}
		else if (paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
		{
			var isValidShippingAndOwner = (cart.GetShipping().IsShippingAddrJp && cart.Owner.IsAddrJp);
			if (isValidShippingAndOwner == false)
			{
				this.WsErrorMessagePayment.InnerHtml = NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_3);
				return;
			}
		}
		else if ((paymentModel.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU)
			&& (this.FixedPurchaseContainer.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
		{
			var optinId = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_PAYMENT_BOKU_OPTIN_ID]);
			if (string.IsNullOrEmpty(optinId))
			{
				var forwardUrl = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_FIXED_PURCHASE_DETAIL)
					.AddParam(Constants.REQUEST_KEY_FIXED_PURCHASE_ID, this.FixedPurchaseContainer.FixedPurchaseId)
					.AddParam("action", "boku")
					.CreateUrl();
				var optin = new PaymentBokuOptinApi().Exec(
					this.FixedPurchaseContainer.AccessCountryIsoCode,
					forwardUrl);

				if (optin == null)
				{
					this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
					AppLogger.WriteError(string.Format(
						"{0} Payment Error: Fixed Purchase ID ={1}",
						BokuConstants.CONST_BOKU_PAYMENT_METHOD_CARRIERBILLING,
						this.FixedPurchaseContainer.FixedPurchaseId));
					return;
				}
				else if (optin.IsSuccess == false)
				{
					this.WsErrorMessagePayment.InnerHtml = optin.Result.Message;
					return;
				}

				Session[Constants.SESSION_KEY_PAYMENT_BOKU_OPTIN_ID] = optin.OptinId;
				Session["boku_payment_id"] = paymentModel.PaymentId;
				Response.Redirect(optin.Hosted.OptinUrl);
			}
			else
			{
				var validate = new PaymentBokuValidateOptinApi().Exec(optinId);
				if (validate == null)
				{
					this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
					AppLogger.WriteError(string.Format(
						"{0} Payment Error: Fixed Purchase ID ={1}",
						paymentModel.PaymentId,
						this.FixedPurchaseContainer.FixedPurchaseId));
					return;
				}
				else if (validate.IsSuccess == false)
				{
					this.WsErrorMessagePayment.InnerHtml = validate.Result.Message;
					return;
				}

				var response = new PaymentBokuConfirmOptinApi().Exec(optinId);
				if (response == null)
				{
					this.WsErrorMessagePayment.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
					AppLogger.WriteError(string.Format(
						"{0} Payment Error: Fixed Purchase ID ={1}",
						paymentModel.PaymentId,
						this.FixedPurchaseContainer.FixedPurchaseId));
					return;
				}
				else if (validate.IsSuccess == false)
				{
					this.WsErrorMessagePayment.InnerHtml = response.Result.Message;
					return;
				}
			}

			externalPaymentAgreementId = optinId;
			Session[Constants.SESSION_KEY_PAYMENT_BOKU_OPTIN_ID] = string.Empty;
			Session["boku_payment_id"] = string.Empty;
		}

		// 現行のAmazon支払い契約をClosed状態にする
		if (this.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
		{
			var clba = AmazonApiFacade.CloseBillingAgreement(
				this.FixedPurchaseContainer.ExternalPaymentAgreementId,
				"情報が変更され、新たに支払い契約を取り直したため。");
			if (clba.GetSuccess() == false) return;
		}

		// 更新実施
		ExecuteUpdatingPayment(paymentModel.PaymentId, branchNo, installmentsCode, externalPaymentAgreementId);
	}

	/// <summary>
	/// 配送希望時間帯 更新ボタンのクリックイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdateShippingTimeAmazonPayCv2_Click(object sender, EventArgs e)
	{
		var fixedPurchaseShipping = new FixedPurchaseShippingModel(this.FixedPurchaseContainer.Shippings[0].DataSource)
		{
			ShippingTime = WddlShippingTimeAmazonPayCv2.SelectedValue.Trim(),
		};

		new FixedPurchaseService().UpdateShipping(
			fixedPurchaseShipping,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);

		Response.Redirect(
			PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
	}

	/// <summary>
	/// 支払い方法更新の実施
	/// </summary>
	/// <param name="paymentId">支払い区分</param>
	/// <param name="branchNo">ブランド番号</param>
	/// <param name="installmentsCode">カード支払い回数コード</param>
	/// <param name="externalPaymentAgreementId">外部支払い契約ID</param>
	private void ExecuteUpdatingPayment(
		string paymentId,
		int? branchNo,
		string installmentsCode,
		string externalPaymentAgreementId)
	{
		// 支払い方法更新
		new FixedPurchaseService().UpdateOrderPayment(
			this.FixedPurchaseContainer.FixedPurchaseId,
			paymentId,
			branchNo,
			installmentsCode,
			externalPaymentAgreementId,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.DoNotInsert);

		// 領収書希望あり、かつ領収書出力しない決済に変更した場合、領収書情報をデフォールト値にリセット
		if (Constants.RECEIPT_OPTION_ENABLED
			&& Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN.Contains(paymentId)
			&& (this.FixedPurchaseContainer.ReceiptFlg == Constants.FLG_ORDER_RECEIPT_FLG_ON))
		{
			new FixedPurchaseService().UpdateReceiptInfo(
				this.FixedPurchaseContainer.FixedPurchaseId,
				Constants.FLG_ORDER_RECEIPT_FLG_OFF,
				string.Empty,
				string.Empty,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert);
		}

		// 更新履歴登録
		new UpdateHistoryService().InsertForFixedPurchase(this.FixedPurchaseContainer.FixedPurchaseId, Constants.FLG_LASTCHANGED_USER);
		new UpdateHistoryService().InsertForUser(this.FixedPurchaseContainer.UserId, Constants.FLG_LASTCHANGED_USER);

		// メール送信
		SendMailCommon.SendModifyFixedPurchaseMail(
			this.FixedPurchaseContainer.FixedPurchaseId,
			SendMailCommon.FixedPurchaseModify.PaymentMethod);

		// 画面再表示
		Response.Redirect(
			PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
	}

	/// <summary>
	/// Amazon配送先で配送先更新の実施
	/// </summary>
	/// <param name="address">Amazon配送先</param>
	/// <param name="shippingMethod">配送方法</param>
	/// <param name="deliveryCompanyId">配送サービスID</param>
	/// <param name="shippingTime">配送時間帯</param>
	private void ExecuteUpdatingShippingWithAmazonAddress(
		AmazonAddressModel address,
		string shippingMethod,
		string deliveryCompanyId,
		string shippingTime)
	{
		// 配送先更新
		var fixedPurchaseShipping = new FixedPurchaseShippingModel
		{
			FixedPurchaseId = this.FixedPurchaseContainer.FixedPurchaseId,
			ShippingName1 = address.Name1,
			ShippingName2 = address.Name2,
			ShippingName = address.Name,
			ShippingNameKana1 = address.NameKana1,
			ShippingNameKana2 = address.NameKana2,
			ShippingNameKana = address.NameKana,
			ShippingZip = address.Zip,
			ShippingAddr1 = address.Addr1,
			ShippingAddr2 = address.Addr2,
			ShippingAddr3 = address.Addr3,
			ShippingAddr4 = address.Addr4,
			ShippingCompanyName = this.FixedPurchaseShippingContainer.ShippingCompanyName,
			ShippingCompanyPostName = this.FixedPurchaseShippingContainer.ShippingCompanyPostName,
			ShippingTel1 = address.Tel1 + "-" + address.Tel2 + "-" + address.Tel3,
			ShippingTime = shippingTime,
			DeliveryCompanyId = deliveryCompanyId,
			ShippingMethod = shippingMethod,
			ShippingCountryIsoCode = Constants.COUNTRY_ISO_CODE_JP,
			ShippingCountryName = "Japan",
			ShippingReceivingStoreType = string.Empty,
			ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF,
			ShippingReceivingStoreId = string.Empty,
		};

		new FixedPurchaseService().UpdateShipping(
			fixedPurchaseShipping,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 決済種別変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void rbgPayment_OnCheckedChanged(object sender, EventArgs e)
	{
		var isRB = (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB);
		foreach (RepeaterItem riPayment in this.WrPayment.Items)
		{
			var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(riPayment, "rbgPayment");
			var wddlPayment = GetWrappedControl<WrappedDropDownList>(riPayment.Parent.Parent, "ddlPayment");
			var whfPaymentName = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentName", string.Empty);
			var whfPaymentPrice = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentPrice", string.Empty);
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(riPayment, "hfPaymentId", "");
			var wddCredit = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddCredit");
			var wddCvsDef = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddCvsDef");
			var wddTriLinkAfterPayPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddTriLinkAfterPayPayment");
			var wddCollect = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddCollect");
			var wddAmazonPay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddAmazonPay");
			var wddAmazonPayCv2 = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddAmazonPayCv2");
			var wddPayPal = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPayPal");
			var wddNoPayment = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddNoPayment");
			var wddPaidy = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPaidy");
			var wucPaidyCheckoutControl = GetWrappedControl<WrappedPaidyCheckoutControl>(riPayment, "ucPaidyCheckoutControl");
			var wddAtonePay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPaymentAtone");
			var wddAfteePay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPaymentAftee");
			var wddNpAfterPay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddNpAfterPay");
			var wddDskDef = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddDskDef");
			var wddLinePay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddLinePay");
			var wddPayPay = GetWrappedControl<WrappedHtmlGenericControl>(riPayment, "ddPayPay");

			wddCredit.Visible = (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
			wddCvsDef.Visible = (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
			wddCollect.Visible = (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT);
			wddAmazonPay.Visible = (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT);
			wddAmazonPayCv2.Visible = (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2);
			wddPayPal.Visible = (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL);
			wddTriLinkAfterPayPayment.Visible = (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY);
			wddNoPayment.Visible = (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT);
			wddPaidy.Visible = (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
			if (isRB ? wrbgPayment.Checked : (wddlPayment.SelectedValue == whfPaymentId.Value))
			{
				wucPaidyCheckoutControl.DisplayUserControl = (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
				this.WhfPaidyPaySelected.Value = wucPaidyCheckoutControl.DisplayUserControl.ToString();
			}
			wddAtonePay.Visible = (isRB
				? (wrbgPayment.HasInnerControl && wrbgPayment.Checked)
				: (wddlPayment.HasInnerControl && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE);
			wddAfteePay.Visible = (isRB
				? (wrbgPayment.HasInnerControl && wrbgPayment.Checked)
				: (wddlPayment.HasInnerControl && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
			wddNpAfterPay.Visible = (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY);
			wddDskDef.Visible = (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF);
			wddLinePay.Visible = (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY);
			wddPayPay.Visible = (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)))
				&& (whfPaymentId.Value == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY);

			if (isRB
				? ((wrbgPayment.InnerControl != null) && wrbgPayment.Checked)
				: ((wddlPayment.InnerControl != null) && (wddlPayment.SelectedValue == whfPaymentId.Value)))
			{
				this.WhfPaymentNameSelected.Value = whfPaymentName.Value;
				this.WhfPaymentIdSelected.Value = whfPaymentId.Value;
			}
			if (Constants.PAYMENT_ATONEOPTION_ENABLED
				|| Constants.PAYMENT_AFTEEOPTION_ENABLED)
			{
				CheckValidTelNoAndCountryForPaymentAtoneAndAftee(this.WhfPaymentIdSelected.Value);
			}
		}
	}

	/// <summary>
	/// Link Button Display Invoice Info Form Click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayTwInvoiceInfoForm_Click(object sender, EventArgs e)
	{
		this.IsTwFixedPurchaseInvoiceModify = (this.IsTwFixedPurchaseInvoiceModify == false);
		if (IsTwFixedPurchaseInvoiceModify == false) return;

		this.WddlTaiwanUniformInvoice.Items.Clear();
		this.WddlTaiwanUniformInvoice.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_TWFIXEDPURCHASEINVOICE,
				Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE));
		var selectedValue = this.TwFixedPurchaseInvoiceModel.TwUniformInvoice;
		if (this.WddlTaiwanUniformInvoice.Items.FindByValue(selectedValue) != null)
		{
			this.WddlTaiwanUniformInvoice.SelectedValue = selectedValue;
		}
		this.WddlTaiwanUniformInvoice.DataBind();
		ddlTaiwanUniformInvoice_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// Drop Down List Taiwan Uniform Invoice Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlTaiwanUniformInvoice_SelectedIndexChanged(object sender, EventArgs e)
	{
		var selectedValueOfUniformInvoice = this.WddlTaiwanUniformInvoice.SelectedValue;
		switch (this.WddlTaiwanUniformInvoice.SelectedValue)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				this.WtrInvoiceDispForPersonalType.Visible = false;
				this.WdvInvoiceForm.Visible = true;
				this.WspTaiwanUniformInvoiceOptionKbnList.Visible = true;
				this.WspTaiwanCarryKbnList.Visible = false;

				this.WddlTaiwanUniformInvoiceOptionKbnList.Items.Clear();
				this.WddlTaiwanUniformInvoiceOptionKbnList.Items.Add(new ListItem(
					ReplaceTag("@@DispText.uniform_invoice_option.new@@"),
					Constants.FLG_TW_CARRY_KBN_NEW));
				if (this.TwUserInvoiceList != null)
				{
					var userInvoices = (selectedValueOfUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY)
						? this.TwUserInvoiceList.Where(item => item.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY)
						: this.TwUserInvoiceList.Where(item => item.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_DONATE);

					this.WddlTaiwanUniformInvoiceOptionKbnList.Items.AddRange(
						userInvoices.Select(item => new ListItem(item.TwInvoiceName, item.TwInvoiceNo.ToString())).ToArray());
				}

				this.WddlTaiwanUniformInvoiceOptionKbnList.DataBind();
				ddlTaiwanUniformInvoiceOptionKbnList_SelectedIndexChanged(sender, e);
				break;

			default:
				this.WtrInvoiceDispForPersonalType.Visible = true;
				this.WdvInvoiceForm.Visible = false;
				this.WspTaiwanUniformInvoiceOptionKbnList.Visible = false;
				this.WspTaiwanCarryKbnList.Visible = true;
				var selectedValue = string.Empty;

				this.WddlTaiwanCarryType.Items.Clear();
				this.WddlTaiwanCarryType.Items.AddRange(
					ValueText.GetValueItemArray(
						Constants.TABLE_TWFIXEDPURCHASEINVOICE,
						Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE));
				selectedValue = this.TwFixedPurchaseInvoiceModel.TwCarryType;
				if (this.WddlTaiwanCarryType.Items.FindByValue(selectedValue) != null)
				{
					this.WddlTaiwanCarryType.SelectedValue = selectedValue;
				}
				this.WddlTaiwanCarryType.DataBind();
				ddlTaiwanCarryType_SelectedIndexChanged(sender, e);
				break;
		}
	}

	/// <summary>
	/// Drop Down List Taiwan Carry Type Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlTaiwanCarryType_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.WddlTaiwanCarryKbnList.Items.Clear();
		this.WddlTaiwanCarryKbnList.Items.Add(new ListItem(
			ReplaceTag("@@DispText.invoice_carry_type_option.new@@"),
			Constants.FLG_TW_CARRY_KBN_NEW));

		switch (this.WddlTaiwanCarryType.SelectedValue)
		{
			case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
				this.WdvCarryTypeOption_16.Visible = false;
				this.WdvCarryTypeOption_8.Visible = true;
				this.WspTaiwanCarryKbnList.Visible = true;
				this.WdvInvoiceNoEquipmentMessage.Visible = false;
				break;

			case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
				this.WdvCarryTypeOption_16.Visible = true;
				this.WdvCarryTypeOption_8.Visible = false;
				this.WspTaiwanCarryKbnList.Visible = true;
				this.WdvInvoiceNoEquipmentMessage.Visible = false;
				break;

			default:
				this.WdvCarryTypeOption_16.Visible = false;
				this.WdvCarryTypeOption_8.Visible = false;
				this.WspTaiwanCarryKbnList.Visible = false;
				this.WdvInvoiceNoEquipmentMessage.Visible = true;
				break;
		}

		if (this.TwUserInvoiceList != null)
		{
			var userInvoices = this.TwUserInvoiceList.Where(item => item.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL);
			userInvoices = userInvoices.Where(item => item.TwCarryType == this.WddlTaiwanCarryType.SelectedValue).ToArray();
			this.WddlTaiwanCarryKbnList.Items.AddRange(
				userInvoices.Select(item => new ListItem(item.TwInvoiceName, item.TwInvoiceNo.ToString())).ToArray());
		}

		this.WddlTaiwanCarryKbnList.DataBind();
		ddlTaiwanCarryKbnList_SelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// Dropdown List Taiwan Carry Kbn List Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlTaiwanCarryKbnList_SelectedIndexChanged(object sender, EventArgs e)
	{
		var selectedValue = WddlTaiwanCarryKbnList.SelectedValue;
		switch (selectedValue)
		{
			case Constants.FLG_TW_CARRY_KBN_NEW:
				this.WdvInvoiceDispForPersonalType.Visible = false;
				this.WdvInvoiceInputForPersonalType.Visible = this.WspTaiwanCarryKbnList.Visible;
				this.WcbCarryTypeOptionRegist.Visible = (string.IsNullOrEmpty(this.WddlTaiwanCarryType.SelectedValue) == false);

				if (this.WddlTaiwanCarryType.SelectedValue == Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE)
				{
					this.WtbCarryTypeOption_8.Text = this.TwFixedPurchaseInvoiceModel.TwCarryTypeOption;
				}
				else
				{
					this.WtbCarryTypeOption_16.Text = this.TwFixedPurchaseInvoiceModel.TwCarryTypeOption;
				}
				break;

			default:
				this.WdvInvoiceDispForPersonalType.Visible = true;
				this.WdvInvoiceInputForPersonalType.Visible = false;
				this.WcbCarryTypeOptionRegist.Visible = false;

				this.WlInvoiceCode.Text = this.TwUserInvoiceList
					.Where(item => item.TwInvoiceNo.ToString() == selectedValue)
					.FirstOrDefault().TwCarryTypeOption;
				break;
		}
	}

	/// <summary>
	/// Drop Down List Taiwan Uniform Invoice Option Kbn List Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlTaiwanUniformInvoiceOptionKbnList_SelectedIndexChanged(object sender, EventArgs e)
	{
		var selectedValue = this.WddlTaiwanUniformInvoiceOptionKbnList.SelectedValue;
		var selectedValueOfUniformInvoice = this.WddlTaiwanUniformInvoice.SelectedValue;
		switch (selectedValue)
		{
			case Constants.FLG_TW_CARRY_KBN_NEW:
				this.WdvInvoiceDisp.Visible = false;
				this.WdvInvoiceInput.Visible = true;
				this.WcbSaveToUserInvoice.Visible = true;

				if (selectedValueOfUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY)
				{
					this.WdvInvoiceInputForDonateType.Visible = false;
					this.WdvInvoiceInputForCompanyType.Visible = true;
					this.WtbUniformInvoiceOption1_8.Text = this.TwFixedPurchaseInvoiceModel.TwUniformInvoiceOption1;
					this.WtbUniformInvoiceOption2.Text = this.TwFixedPurchaseInvoiceModel.TwUniformInvoiceOption2;
				}
				else
				{
					this.WdvInvoiceInputForDonateType.Visible = true;
					this.WdvInvoiceInputForCompanyType.Visible = false;
					this.WtbUniformInvoiceOption1_3.Text = this.TwFixedPurchaseInvoiceModel.TwUniformInvoiceOption1;
				}
				break;

			default:
				this.WdvInvoiceDisp.Visible = true;
				this.WdvInvoiceInput.Visible = false;
				this.WcbSaveToUserInvoice.Visible = false;

				var twUserInvoiceInfo = this.TwUserInvoiceList.Where(item =>
					item.TwInvoiceNo.ToString() == selectedValue).FirstOrDefault();
				if (selectedValueOfUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY)
				{
					this.WdvInvoiceDispForDonateType.Visible = false;
					this.WdvInvoiceDispForCompanyType.Visible = true;
					this.WlCompanyCode.Text = twUserInvoiceInfo.TwUniformInvoiceOption1;
					this.WlCompanyName.Text = twUserInvoiceInfo.TwUniformInvoiceOption2;
				}
				else
				{
					this.WdvInvoiceDispForDonateType.Visible = true;
					this.WdvInvoiceDispForCompanyType.Visible = false;
					this.WlDonationCode.Text = twUserInvoiceInfo.TwUniformInvoiceOption1;
				}
				break;
		}
	}

	/// <summary>
	/// Check Box Save To User Invoice Checked Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void cbSaveToUserInvoice_CheckedChanged(object sender, EventArgs e)
	{
		this.WdvUniformInvoiceTypeRegistInput.Visible = this.WcbSaveToUserInvoice.Checked;
	}

	/// <summary>
	/// Check Box Carry Type Option Regist Checked Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void cbCarryTypeOptionRegist_CheckedChanged(object sender, EventArgs e)
	{
		this.WdvCarryTypeOptionName.Visible = this.WcbCarryTypeOptionRegist.Checked;
	}

	/// <summary>
	/// Link Button Display Invoice Info Form Click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateInvoiceInfo_Click(object sender, EventArgs e)
	{
		var selectedValue = this.WddlTaiwanUniformInvoice.SelectedValue;
		var twFixedPurchaseInvoiceInput = new TwFixedPurchaseInvoiceInput(TwFixedPurchaseInvoiceModel);
		var errorMessages = new Dictionary<string, string>();
		var isSaveToUserInvoice = false;

		switch (selectedValue)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
				twFixedPurchaseInvoiceInput.TwCarryType = string.Empty;
				twFixedPurchaseInvoiceInput.TwCarryTypeOption = string.Empty;
				twFixedPurchaseInvoiceInput.TwUniformInvoice = Constants.FLG_TW_UNIFORM_INVOICE_COMPANY;

				if (WddlTaiwanUniformInvoiceOptionKbnList.SelectedValue == Constants.FLG_TW_CARRY_KBN_NEW)
				{
					twFixedPurchaseInvoiceInput.TwUniformInvoiceOption1 = this.WtbUniformInvoiceOption1_8.Text.Trim();
					twFixedPurchaseInvoiceInput.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE_OPTION1 + "_8"] = twFixedPurchaseInvoiceInput.TwUniformInvoiceOption1;
					twFixedPurchaseInvoiceInput.TwUniformInvoiceOption2 = this.WtbUniformInvoiceOption2.Text.Trim();

					// For save information to user invoice
					if (this.WcbSaveToUserInvoice.Checked)
					{
						twFixedPurchaseInvoiceInput.DataSource["tw_invoice_name_uniform"] = this.WtbUniformInvoiceTypeName.Text.Trim();
						isSaveToUserInvoice = true;
					}

					// Validate
					errorMessages = twFixedPurchaseInvoiceInput.Validate();
				}
				else
				{
					var twUserInvoiceInfo = this.TwUserInvoiceList
						.Where(item => item.TwInvoiceNo.ToString() == this.WddlTaiwanUniformInvoiceOptionKbnList.SelectedValue)
						.FirstOrDefault();
					twFixedPurchaseInvoiceInput.TwUniformInvoiceOption1 = twUserInvoiceInfo.TwUniformInvoiceOption1;
					twFixedPurchaseInvoiceInput.TwUniformInvoiceOption2 = twUserInvoiceInfo.TwUniformInvoiceOption2;
				}
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				twFixedPurchaseInvoiceInput.TwCarryType = string.Empty;
				twFixedPurchaseInvoiceInput.TwCarryTypeOption = string.Empty;
				twFixedPurchaseInvoiceInput.TwUniformInvoice = Constants.FLG_TW_UNIFORM_INVOICE_DONATE;

				if (WddlTaiwanUniformInvoiceOptionKbnList.SelectedValue == Constants.FLG_TW_CARRY_KBN_NEW)
				{
					twFixedPurchaseInvoiceInput.TwUniformInvoiceOption2 = null;
					twFixedPurchaseInvoiceInput.TwUniformInvoiceOption1 = this.WtbUniformInvoiceOption1_3.Text.Trim();
					twFixedPurchaseInvoiceInput.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_UNIFORM_INVOICE_OPTION1 + "_3"] = twFixedPurchaseInvoiceInput.TwUniformInvoiceOption1;

					// For save information to user invoice
					if (this.WcbSaveToUserInvoice.Checked)
					{
						twFixedPurchaseInvoiceInput.DataSource["tw_invoice_name_uniform"] = this.WtbUniformInvoiceTypeName.Text.Trim();
						isSaveToUserInvoice = true;
					}

					// Validate
					errorMessages = twFixedPurchaseInvoiceInput.Validate();

					twFixedPurchaseInvoiceInput.TwUniformInvoiceOption2 = string.Empty;
				}
				else
				{
					var twUserInvoiceInfo = this.TwUserInvoiceList
						.Where(item => item.TwInvoiceNo.ToString() == this.WddlTaiwanUniformInvoiceOptionKbnList.SelectedValue)
						.FirstOrDefault();
					twFixedPurchaseInvoiceInput.TwUniformInvoiceOption1 = twUserInvoiceInfo.TwUniformInvoiceOption1;
				}
				break;

			default:
				twFixedPurchaseInvoiceInput.TwUniformInvoiceOption1 = string.Empty;
				twFixedPurchaseInvoiceInput.TwUniformInvoice = Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL;

				if (WddlTaiwanCarryKbnList.SelectedValue == Constants.FLG_TW_CARRY_KBN_NEW)
				{
					twFixedPurchaseInvoiceInput.TwUniformInvoiceOption2 = null;
					twFixedPurchaseInvoiceInput.TwCarryType = this.WddlTaiwanCarryType.SelectedValue;
					switch (this.WddlTaiwanCarryType.SelectedValue)
					{
						case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
							twFixedPurchaseInvoiceInput.TwCarryTypeOption = this.WtbCarryTypeOption_8.Text.Trim();
							twFixedPurchaseInvoiceInput.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE_OPTION + "_8"] = twFixedPurchaseInvoiceInput.TwCarryTypeOption;
							break;

						case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
							twFixedPurchaseInvoiceInput.TwCarryTypeOption = this.WtbCarryTypeOption_16.Text.Trim();
							twFixedPurchaseInvoiceInput.DataSource[Constants.FIELD_TWFIXEDPURCHASEINVOICE_TW_CARRY_TYPE_OPTION + "_16"] = twFixedPurchaseInvoiceInput.TwCarryTypeOption;
							break;

						default:
							twFixedPurchaseInvoiceInput.TwCarryTypeOption = string.Empty;
							break;
					}

					// For save information to user invoice
					if ((string.IsNullOrEmpty(this.WddlTaiwanCarryType.SelectedValue) == false)
						&& this.WcbCarryTypeOptionRegist.Checked)
					{
						twFixedPurchaseInvoiceInput.DataSource["tw_invoice_name_carry"] = this.WtbCarryTypeOptionName.Text.Trim();
						isSaveToUserInvoice = true;
					}

					// Validate
					errorMessages = twFixedPurchaseInvoiceInput.Validate();

					// Validate carry type
					if ((errorMessages.Count == 0)
						&& (this.WddlTaiwanCarryKbnList.SelectedValue == Constants.FLG_TW_CARRY_KBN_NEW))
					{
						Regex regex = null;
						switch (this.WddlTaiwanCarryType.SelectedValue)
						{
							case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
								regex = new Regex(Constants.REGEX_MOBILE_CARRY_TYPE_OPTION_8);
								if (regex.IsMatch(twFixedPurchaseInvoiceInput.TwCarryTypeOption) == false)
								{
									errorMessages[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION + "_8"]
										= WebMessages.GetMessages(WebMessages.ERRMSG_MOBILE_CARRY_TYPE_OPTION_8);
								}
								break;

							case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
								regex = new Regex(Constants.REGEX_CERTIFICATE_CARRY_TYPE_OPTION_16);
								if (regex.IsMatch(twFixedPurchaseInvoiceInput.TwCarryTypeOption) == false)
								{
									errorMessages[Constants.FIELD_TWUSERINVOICE_TW_CARRY_TYPE_OPTION + "_16"]
										= WebMessages.GetMessages(WebMessages.ERRMSG_CERTIFICATE_CARRY_TYPE_OPTION_16);
								}
								break;
						}
					}

					twFixedPurchaseInvoiceInput.TwUniformInvoiceOption2 = string.Empty;
				}
				else
				{
					var twUserInvoiceInfo = this.TwUserInvoiceList
						.Where(item => item.TwInvoiceNo.ToString() == this.WddlTaiwanCarryKbnList.SelectedValue)
						.FirstOrDefault();
					twFixedPurchaseInvoiceInput.TwCarryType = twUserInvoiceInfo.TwCarryType;
					twFixedPurchaseInvoiceInput.TwCarryTypeOption = twUserInvoiceInfo.TwCarryTypeOption;
				}
				break;
		}

		// Custom Validator
		var customValidators = new List<CustomValidator>();
		CreateCustomValidators(this, customValidators);
		foreach (CustomValidator validator in customValidators)
		{
			var validatorTemp = validator.Parent.FindControl(((CustomValidator)validator).ControlToValidate);
			if (validatorTemp != null)
			{
				// Initialization
				validator.IsValid = true;
				validator.ErrorMessage = string.Empty;
				((WebControl)validatorTemp).CssClass = ((WebControl)validatorTemp).CssClass.Replace(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, string.Empty);
			}
		}

		if (errorMessages.Count != 0)
		{
			// Error Custom Validator
			SetControlViewsForError(
				"OrderShippingGlobal",
				errorMessages,
				customValidators);

			return;
		}
		else
		{
			// Update Taiwan Fixed Purchase Invoice
			new TwFixedPurchaseInvoiceService().UpdateTaiwanFixedPurchaseInvoice(twFixedPurchaseInvoiceInput.CreateModel());

			// For save information to user invoice
			if (isSaveToUserInvoice)
			{
				var twUserInvoiceInput = new TwUserInvoiceInput()
				{
					TwInvoiceNo = "0",
					TwUniformInvoice = twFixedPurchaseInvoiceInput.TwUniformInvoice,
					TwUniformInvoiceOption1 = twFixedPurchaseInvoiceInput.TwUniformInvoiceOption1,
					TwUniformInvoiceOption2 = twFixedPurchaseInvoiceInput.TwUniformInvoiceOption2,
					TwCarryType = twFixedPurchaseInvoiceInput.TwCarryType,
					TwCarryTypeOption = twFixedPurchaseInvoiceInput.TwCarryTypeOption,
					UserId = this.FixedPurchaseContainer.UserId,
					TwInvoiceName = (twFixedPurchaseInvoiceInput.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
						? StringUtility.ToEmpty(twFixedPurchaseInvoiceInput.DataSource["tw_invoice_name_carry"])
						: StringUtility.ToEmpty(twFixedPurchaseInvoiceInput.DataSource["tw_invoice_name_uniform"])
				};

				new TwUserInvoiceService().Insert(twUserInvoiceInput.CreateModel(), Constants.FLG_LASTCHANGED_USER, UpdateHistoryAction.Insert);
			}
		}

		// Redirect
		Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
	}

	#region メソッド
	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 月間隔・週・曜日指定ドロップダウン作成（週）
		this.WddlFixedPurchaseWeekOfMonth.AddItems(
			ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST));

		// 月間隔・週・曜日指定ドロップダウン作成（曜日）
		this.WddlFixedPurchaseDayOfWeek.AddItems(
			ValueText.GetValueItemArray(Constants.TABLE_SHOPSHIPPING, Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST));

		// 都道府県ドロップダウン作成
		this.WddlShippingAddr1.AddItem(new ListItem("", ""));
		foreach (string strPrefecture in Constants.STR_PREFECTURES_LIST)
		{
			this.WddlShippingAddr1.AddItem(new ListItem(strPrefecture));
		}
		// 国ドロップダウン作成
		var shippingAvailableCountries = new CountryLocationService().GetShippingAvailableCountry();
		this.WddlShippingCountry.Items.Add(new ListItem("", ""));
		this.WddlShippingCountry.Items.AddRange(shippingAvailableCountries.Select(c => new ListItem(c.CountryName, c.CountryIsoCode)).ToArray());
		// 州ドロップダウン作成
		this.WddlShippingAddr5.Items.Add(new ListItem("", ""));
		this.WddlShippingAddr5.Items.AddRange(Constants.US_STATES_LIST.Select(state => new ListItem(state)).ToArray());
		//メール便配送サービスエスカレーション機能がONの場合
		if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED)
		{
			var companyList = GetCompanyListMail();
			// 配送方法ドロップダウン作成
			if ((companyList.Any() == false)
				|| ((this.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
					|| this.FixedPurchaseShippingContainer.Items.Any(item => (item.ShippingSizeKbn != Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL))))
			{
				var expressDeliveryText = ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);
				this.WddlShippingMethod.Items.Add(new ListItem(expressDeliveryText, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS));
				this.FixedPurchaseShippingContainer.ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
			}
			else
			{
				this.WddlShippingMethod.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD));
			}
			// 配送サービスドロップダウン作成
			var deliveryCompany = (this.FixedPurchaseShippingContainer.ShippingMethod == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
				? companyList.Select(c => new ListItem(c.DeliveryCompanyName, c.DeliveryCompanyId)).ToArray()
				: DataCacheControllerFacade.GetShopShippingCacheController()
					.Get(this.FixedPurchaseContainer.ShippingType)
					.CompanyList.Where(ss => (ss.ShippingKbn == this.FixedPurchaseShippingContainer.ShippingMethod))
					.Select(cm =>
						{
							var company = new DeliveryCompanyService().Get(cm.DeliveryCompanyId);
							return new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId);
						}
					).ToArray();
			this.WddlDeliveryCompany.Items.AddRange(deliveryCompany);
		}
		else
		{
			// 配送方法ドロップダウン作成
			if (this.FixedPurchaseShippingContainer.Items.Any(item => (item.ShippingSizeKbn != Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL))
				|| (this.FixedPurchaseShippingContainer.Items.Sum(item => item.ItemQuantity) > Constants.MAIL_ESCALATION_COUNT
					|| this.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT))
			{
				var expressDeliveryText = ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);
				this.WddlShippingMethod.Items.Add(new ListItem(expressDeliveryText, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS));
			}
			else
			{
				this.WddlShippingMethod.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD));
			}

			var deliveryCompanyService = new DeliveryCompanyService();
			// 配送サービスドロップダウン作成
				this.WddlDeliveryCompany.Items.AddRange(DataCacheControllerFacade.GetShopShippingCacheController()
					.Get(this.FixedPurchaseContainer.ShippingType)
					.CompanyList.Where(ss => (ss.ShippingKbn == this.FixedPurchaseShippingContainer.ShippingMethod))
					.Select(cm =>
						{
							var company = deliveryCompanyService.Get(cm.DeliveryCompanyId);
							return new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId);
						}
					).ToArray());
		}

		// 配送希望時間帯ドロップダウン作成
		this.WddlShippingTime.Items.Add(new ListItem(ReplaceTag("@@DispText.shipping_time_list.none@@"), ""));
		// 配送希望時間帯設定可能フラグが有効?
		if (this.DeliveryCompany.IsValidShippingTimeSetFlg)
		{
			this.WddlShippingTime.Items.AddRange(this.DeliveryCompany.GetShippingTimeList().Select(kvp => new ListItem(kvp.Value, kvp.Key)).ToArray());
		}
	}

	/// <summary>
	/// 画面に値をセット
	/// </summary>
	private void SetValues()
	{
		var intervalMonth =
			((this.FixedPurchaseContainer.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
				|| (this.FixedPurchaseContainer.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY))
			? this.FixedPurchaseContainer.FixedPurchaseSetting1.Split(',')[0]
			: "";

		var fixedPurchaseKbn1Setting2 = (((this.FixedPurchaseContainer.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
			|| (this.FixedPurchaseContainer.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY))
			? this.FixedPurchaseContainer.FixedPurchaseSetting1.Split(',')[1]
			: "");

		// 月間隔日付指定ドロップダウン作成（月間隔）
		this.WddlFixedPurchaseMonth.AddItems(GetListItemFixedPurchaseInterval(intervalMonth, true, false));

		// 月間隔・週・曜日指定ドロップダウン作成（月間隔）
		this.WddlFixedPurchaseIntervalMonths.AddItems(GetListItemFixedPurchaseInterval(intervalMonth, true, false));

		// 月間隔日付指定ドロップダウン作成（日付）
		this.WddlFixedPurchaseMonthlyDate.Items.AddRange(
				GetListItemFixedPurchaseInterval(
					fixedPurchaseKbn1Setting2, true, true));

		// 配送間隔指定詳細ドロップダウンリスト作成
		var interval = ((this.FixedPurchaseContainer.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS) ? this.FixedPurchaseContainer.FixedPurchaseSetting1 : "");
		this.WddlFixedPurchaseIntervalDays.AddItems(GetListItemFixedPurchaseInterval(interval, false, false));

		// 週間隔・曜日指定ドロップダウンリスト作成（週間隔）
		var selectedEveryNWeek = ((this.FixedPurchaseContainer.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY) ? this.FixedPurchaseContainer.FixedPurchaseSetting1 : string.Empty).Split(',');
		this.WddlFixedPurchaseEveryNWeek_Week.AddItems(GetFixedPurchaseEveryNWeekDropdown(selectedEveryNWeek[0], true));
		// 週間隔・曜日指定ドロップダウンリスト作成（曜日）
		var selectedDayOfWeek = (selectedEveryNWeek.Length > 1) ? selectedEveryNWeek[1] : string.Empty;
		this.WddlFixedPurchaseEveryNWeek_DayOfWeek.AddItems(GetFixedPurchaseEveryNWeekDropdown(selectedDayOfWeek, false));

		// 月間隔日付指定表示制御
		this.WddMonthlyDate.Visible
			= this.WdtMonthlyDate.Visible
			= (this.ShopShipping.IsValidFixedPurchaseKbn1Flg && (this.WddlFixedPurchaseMonth.Items.Count > 1));
		// 月間隔・週・曜日指定表示制御
		this.WddWeekAndDay.Visible
			= this.WdtWeekAndDay.Visible
			= (this.ShopShipping.IsValidFixedPurchaseKbn2Flg
				&& ((this.WddlFixedPurchaseIntervalMonths.Items.Count > 1)
					|| (this.WddlFixedPurchaseIntervalMonths.Items.Count == 0)));
		// 配送間隔指定表示制御
		this.WddIntervalDays.Visible = this.WdtIntervalDays.Visible = this.ShopShipping.IsValidFixedPurchaseKbn3Flg
			&& (Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG
				? (this.WddlFixedPurchaseIntervalDays.Items.Count > 0)
				: (this.WddlFixedPurchaseIntervalDays.Items.Count > 1));
		// 週間隔・曜日指定表示制御
		this.WddFixedPurchaseEveryNWeek.Visible = this.WdtFixedPurchaseEveryNWeek.Visible =
			(this.ShopShipping.IsValidFixedPurchaseKbn4Flg
			&& (this.WddlFixedPurchaseEveryNWeek_Week.Items.Count > 1)
			&& (this.WddlFixedPurchaseEveryNWeek_DayOfWeek.Items.Count > 1));

		// 配送先情報変更入力フォーム
		this.IsUserShippingModify = false;

		// ボタンの表示/非表示の制御
		this.BeforeCancelDeadline =
			(this.FixedPurchaseContainer.NextShippingDate.Value.AddDays((-1) * this.ShopShipping.FixedPurchaseCancelDeadline).CompareTo(DateTime.Today) >= 0);

		// 各ステータスCSS制御
		this.WspFixedPurchaseStatus.Attributes["class"] = "fixedPurchaseStatus_" + this.FixedPurchaseContainer.FixedPurchaseStatus;
		this.WspPaymentStatus.Attributes["class"] = "paymentStatus_" + this.FixedPurchaseContainer.PaymentStatus;

		// 定期購入商品リピーターにセット
		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		this.WrItem.DataSource = input.Shippings[0].Items;
		this.WrItemModify.DataSource = input.Shippings[0].Items.ToList();
		if ((this.InputProductList == null)
			|| (this.InputProductList[0].FixedPurchaseId != this.FixedPurchaseContainer.FixedPurchaseId))
			this.InputProductList = input.Shippings[0].Items.ToList();
		// 定期商品変更設定をセット
		if (Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED)
		{
			SetFixedPurchaseProductChange();
		}
		this.WrFixedPurchaseModifyProducts.DataSource = this.InputProductList;

		// ユーザクレジットカード情報セット
		this.WdvFixedPurchaseCurrentCard.Visible = this.FixedPurchaseContainer.UseUserCreditCard;

		// 今すぐ注文した時のメッセージセット
		this.WlbOrderNowMesssage.Text = this.OrderNowMessagesHtmlEncoded;

		// ポイントリセットした時のメッセージセット
		this.WlPointResetMessages.Text = this.PointResetMessages;

		// 定期購入再開したときのメッセージセット
		WlResumeFixedPurchaseMessage.Text = this.ResumeFixedPurchaseMessageHtmlEncoded;

		// データソース設定
		this.WrOrderSuccess.DataSource = this.RegisteredOrderIds;

		// 定期購入再開日時
		if (WddlResumeFixedPurchaseDate.HasInnerControl)
		{
			var resumeShippingStartEnd = new ShopShippingModel
			{
				ShippingDateSetBegin = this.ShopShipping.IsValidShippingDateSetFlg
					? this.ShopShipping.ShippingDateSetBegin
					: 0,
				ShippingDateSetEnd = this.ShopShipping.IsValidShippingDateSetFlg
					? this.ShopShipping.ShippingDateSetEnd
					: this.ShopShipping.NextShippingMaxChangeDays,
				BusinessDaysForShipping = this.ShopShipping.IsValidShippingDateSetFlg
					? this.ShopShipping.BusinessDaysForShipping
					: this.ShopShipping.FixedPurchaseShippingDaysRequired
			};
			WddlResumeFixedPurchaseDate.InnerControl.DataSource = OrderCommon.GetListItemShippingDate(resumeShippingStartEnd, true);
		}

		// 頒布会購入再開日時
		if (this.WddlResumeSubscriptionBoxDate.HasInnerControl)
		{
			var subscriptionBoxService = new SubscriptionBoxService();
			var subscriptionBox = subscriptionBoxService.GetByCourseId(this.FixedPurchaseContainer.SubscriptionBoxCourseId);

			if (subscriptionBox != null)
			{
				var resumeShippingStartEnd = new ShopShippingModel
				{
					ShippingDateSetBegin = this.ShopShipping.IsValidShippingDateSetFlg
						? this.ShopShipping.ShippingDateSetBegin
						: 0,
					ShippingDateSetEnd = this.ShopShipping.IsValidShippingDateSetFlg
						? this.ShopShipping.ShippingDateSetEnd
						: this.ShopShipping.NextShippingMaxChangeDays,
					BusinessDaysForShipping = this.ShopShipping.IsValidShippingDateSetFlg
						? this.ShopShipping.BusinessDaysForShipping
						: this.ShopShipping.FixedPurchaseShippingDaysRequired
				};

				var shippingDate = OrderCommon.GetListItemShippingDate(resumeShippingStartEnd, true);

				var maxUntil = subscriptionBox.DefaultOrderProducts.Aggregate(
					DateTime.Now,
						(current, product) =>
							current > Convert.ToDateTime(product.TermUntil)
						? current
						: Convert.ToDateTime(product.TermUntil));

				var chooseableShippingDate = new ListItemCollection();
				foreach (ListItem listItemshippingDate in shippingDate)
				{
					DateTime parseShippingDate;
					DateTime.TryParse(listItemshippingDate.Value, out parseShippingDate);
					if (parseShippingDate > maxUntil)
					{
						chooseableShippingDate.Add(listItemshippingDate);
					}
				}

				this.WddlResumeSubscriptionBoxDate.InnerControl.DataSource = chooseableShippingDate;
			}
		}

		//定期購入可能回数設定
		foreach (FixedPurchaseItemInput r in input.Shippings[0].Items)
		{
			if (this.FixedPurchaseCancelableCount == 0 || this.FixedPurchaseCancelableCount < r.FixedPurchaseCancelableCount)
			{
				this.FixedPurchaseCancelableCount = r.FixedPurchaseCancelableCount;
			}
		}

		// 次回注文時のお支払い予定金額
		SetPlannedTotalAmountForTheNextOrder();

		this.IsTwFixedPurchaseInvoiceModify = false;
		var listShippingKbn = this.UserShippingList;
		listShippingKbn.RemoveAt(0);

		var orderExtend = OrderExtendCommon.ConvertOrderExtend(this.FixedPurchaseContainer);
		this.WrOrderExtendDisplay.DataSource = new OrderExtendInput(OrderExtendInput.UseType.Modify, orderExtend).OrderExtendItems;
		this.WrOrderExtendDisplay.DataBind();

		if (this.IsInvalidResumePaypay)
		{
			switch (Constants.PAYMENT_PAYPAY_KBN)
			{
				case w2.App.Common.Constants.PaymentPayPayKbn.GMO:
					WlCancelPaypayNotification.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYMENT_PAPAY_GMO_NOTIFICATION);
					break;

				case w2.App.Common.Constants.PaymentPayPayKbn.VeriTrans:
					WlCancelPaypayNotification.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PURCHASE_RESUMED_PAYMENT_PAPAY_VERITRANS_MESSAGE);
					break;
			}
		}
	}

	/// <summary>
	/// AmazonPayフォームのコンポーネントをセット
	/// </summary>
	private void SetAmazonPayComponents()
	{
		// 配送方法ドロップダウン作成
		if (this.FixedPurchaseShippingContainer.Items.Any(item => (item.ShippingSizeKbn != Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_MAIL))
			|| (this.FixedPurchaseShippingContainer.Items.Sum(item => item.ItemQuantity) > Constants.MAIL_ESCALATION_COUNT))
		{
			var expressDeliveryText = ValueText.GetValueText(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS);
			this.WddlShippingMethodForAmazonPay.Items.Add(new ListItem(expressDeliveryText, Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS));
		}
		else
		{
			this.WddlShippingMethodForAmazonPay.Items.AddRange(ValueText.GetValueItemArray(Constants.TABLE_ORDERSHIPPING, Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD));
		}
		var deliveryCompanyService = new DeliveryCompanyService();
		// 配送サービスのドロップダウン作成
		this.WddlDeliveryCompanyForAmazonPay.Items.AddRange(DataCacheControllerFacade.GetShopShippingCacheController()
			.Get(this.FixedPurchaseContainer.ShippingType)
			.CompanyList.Where(ss => (ss.ShippingKbn == this.FixedPurchaseShippingContainer.ShippingMethod))
			.Select(cm =>
				{
					var company = deliveryCompanyService.Get(cm.DeliveryCompanyId);
					return new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId);
				}
			).ToArray());

		// 配送希望時間帯ドロップダウン作成
		this.WddlShippingTimeForAmazonPay.Items.Add(new ListItem(ReplaceTag("@@DispText.common_message.unspecified@@"), ""));
		this.WddlShippingTimeAmazonPayCv2.Items.Add(new ListItem(ReplaceTag("@@DispText.common_message.unspecified@@"), ""));
		// 配送希望時間帯設定可能フラグが有効?
		if (this.DeliveryCompany.IsValidShippingTimeSetFlg)
		{
			this.WddlShippingTimeForAmazonPay.Items.AddRange(this.DeliveryCompany.GetShippingTimeList().Select(kvp => new ListItem(kvp.Value, kvp.Key)).ToArray());
			this.WddlShippingTimeAmazonPayCv2.Items.AddRange(this.DeliveryCompany.GetShippingTimeList().Select(kvp => new ListItem(kvp.Value, kvp.Key)).ToArray());
		}

		// 初期値選択
		this.WddlShippingMethodForAmazonPay.SelectedValue = this.FixedPurchaseContainer.Shippings[0].ShippingMethod;
		this.WddlDeliveryCompanyForAmazonPay.SelectedValue = this.FixedPurchaseContainer.Shippings[0].ShippingCompany;
		this.WddlShippingTimeForAmazonPay.SelectedValue
			= this.WddlShippingTimeAmazonPayCv2.SelectedValue
				= this.FixedPurchaseContainer.Shippings[0].ShippingTime;
	}

	/// <summary>
	/// 注文履歴詳細ページのURL作成
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>注文履歴詳細ページのURL</returns>
	protected string CreateOrderHistoryDetailUrl(string orderId)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, orderId).CreateUrl();
		return url;
	}

	/// <summary>
	/// 定期配送間隔設定値リスト取得
	/// </summary>
	/// <param name="selectedValue">選択中の値</param>
	/// <param name="isKbn1Setting">定期購入区分１か
	/// （TRUE：定期購入区分１；FALSE：定期購入区分３）</param>
	/// <param name="isDays">日付選択肢</param>
	/// <returns>定期配送間隔設定値リスト</returns>
	protected ListItem[] GetListItemFixedPurchaseInterval(string selectedValue, bool isKbn1Setting, bool isDays)
	{
		// 該当の利用不可配送間隔を取得
		var limitedSettings = new FixedPurchaseInput(this.FixedPurchaseContainer).Shippings[0].Items
			.Where(p => string.IsNullOrEmpty(isKbn1Setting
				? p.LimitedFixedPurchaseKbn1Setting
				: p.LimitedFixedPurchaseKbn3Setting) == false)
			.Select(item =>
				isKbn1Setting ? item.LimitedFixedPurchaseKbn1Setting : item.LimitedFixedPurchaseKbn3Setting)
				.Distinct().ToList();

		// 該当の定期購入区分設定値（Xか月ごとorX日間隔）リスト作成
		var result = isKbn1Setting
			? isDays
				? OrderCommon.GetKbn1FixedPurchaseIntervalListItems(
						OrderCommon.GetFixedPurchaseIntervalSettingExceptLimitedValue(
							limitedSettings,
							isKbn1Setting
								? Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING2
								: Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING,
							this.ShopShipping.ShippingId),
						selectedValue,
						true)

				: OrderCommon.GetKbn1FixedPurchaseIntervalListItems(
					OrderCommon.GetFixedPurchaseIntervalSettingExceptLimitedValue(
						limitedSettings,
						isKbn1Setting
							? Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING
							: Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING,
						this.ShopShipping.ShippingId),
					selectedValue)

			: OrderCommon.GetKbn3FixedPurchaseIntervalListItems(
				OrderCommon.GetFixedPurchaseIntervalSettingExceptLimitedValue(
					limitedSettings,
					isKbn1Setting
						? Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN1_SETTING
						: Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN3_SETTING,
					this.ShopShipping.ShippingId),
				selectedValue);

		return result;
	}

	/// <summary>
	/// 定期購入配送週間隔・曜日設定値ドロップダウンリスト作成
	/// </summary>
	/// <param name="selectedValue">選択中の値</param>
	/// <param name="isIntervalWeek">週間隔か（TRUE：配送週間隔；FALSE：配送曜日）</param>
	/// <returns></returns>
	public ListItem[] GetFixedPurchaseEveryNWeekDropdown(string selectedValue, bool isIntervalWeek)
	{
		// 該当の利用不可配送間隔を取得
		var limitedSettings = isIntervalWeek
			? new FixedPurchaseInput(this.FixedPurchaseContainer).Shippings[0].Items
				.Where(p => string.IsNullOrEmpty(p.LimitedFixedPurchaseKbn4Setting) == false)
				.Select(item => item.LimitedFixedPurchaseKbn4Setting)
				.Distinct().ToList()
			: new List<string>();

		// 有効な週間隔 or 曜日の設定値取得
		var baseSetting = OrderCommon.GetFixedPurchaseIntervalSettingExceptLimitedValue(
			limitedSettings,
			isIntervalWeek
				? Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING1
				: Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_KBN4_SETTING2,
			this.ShopShipping.ShippingId);

		// baseSettingをもとにドロップダウンリスト作成
		var intervalValuesList = (isIntervalWeek)
			? OrderCommon.GetKbn4Setting1FixedPurchaseIntervalListItems(
				baseSetting,
				selectedValue)
			: OrderCommon.GetKbn4Setting2FixedPurchaseIntervalListItems(
				baseSetting,
				selectedValue);

		return intervalValuesList;
	}

	/// <summary>
	/// 選択している配送間隔月・週・日が規定の値かどうかのチェック
	/// </summary>
	/// <param name="selectedValue">選択している配送間隔値</param>
	/// <param name="fixedPurchaseKbn">定期購入区分</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckSpecificFixedPurchaseIntervalValues(string selectedValue, string fixedPurchaseKbn)
	{
		var valid = OrderCommon.CheckSpecificFixedPurchaseIntervalMonthAndDay(
			new FixedPurchaseInput(this.FixedPurchaseContainer).Shippings[0].Items
				.Where(item => (string.IsNullOrEmpty(GetLimitedFixedPurchaseKbnSetting(item, fixedPurchaseKbn)) == false))
				.Select(item => new Hashtable
				{
					{ fixedPurchaseKbn, GetLimitedFixedPurchaseKbnSetting(item, fixedPurchaseKbn) },
				}).Distinct().ToList(),
			selectedValue,
			fixedPurchaseKbn);

		var error = valid
			? string.Empty
			: CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_FIXED_PURCHASE_SPECIFIC_MONTH_DATE_INTERVAL_INVALID)
				.Replace("@@ 1 @@", string.Format("{0}",
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_FIXED_PURCHASE_DETAIL,
						Constants.VALUETEXT_PARAM_MONTH_DAY_INTERVAL_INVALID,
						GetParamName(fixedPurchaseKbn))));

		return error;
	}

	/// <summary>
	/// 利用不可定期購入配送間隔月・週・日取得
	/// </summary>
	/// <param name="item">定期購入情報</param>
	/// <param name="fixedPurchaseKbn">定期購入区分</param>
	/// <returns>利用不可定期配送間隔月・週・月</returns>
	private string GetLimitedFixedPurchaseKbnSetting(FixedPurchaseItemInput item, string fixedPurchaseKbn)
	{
		switch (fixedPurchaseKbn)
		{
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
				return item.LimitedFixedPurchaseKbn1Setting;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
				return item.LimitedFixedPurchaseKbn3Setting;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
				return item.LimitedFixedPurchaseKbn4Setting;

			default:
				return string.Empty;
		}
	}

	/// <summary>
	/// ValueText の Param名を取得
	/// </summary>
	/// <param name="fixedPurchaseKbn">定期購入区分</param>
	/// <returns>ValueText の Param名</returns>
	private string GetParamName(string fixedPurchaseKbn)
	{
		switch (fixedPurchaseKbn)
		{
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
				return Constants.VALUETEXT_PARAM_MONTH;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
				return Constants.VALUETEXT_PARAM_DAY;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
				return Constants.VALUETEXT_PARAM_WEEK;

			default:
				return string.Empty;
		}
	}
	#endregion

	/// <summary>
	/// 配送先の国ISOコード取得
	/// </summary>
	/// <returns></returns>
	protected string GetShippingCountryIsoCode()
	{
		if (this.IsUserShippingModify == false)
		{
			return this.WddlShippingCountry.SelectedValue;
		}
		return this.FixedPurchaseShippingContainer.ShippingCountryIsoCode;
	}

	/// <summary>
	/// 定期購入一時休止ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSuspendFixedPurchase_Click(object sender, EventArgs e)
	{
		// 確認画面へ遷移
		Response.Redirect(CreateFixedPurchaseCancelReasonInputUrl(
			this.FixedPurchaseContainer.FixedPurchaseId,
			Constants.KBN_FIXED_PURCHASE_NEXT_PAGE_KBN_SUSPEND));
	}

	/// <summary>
	/// 次回配送日変更ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnChangeNextShippingDate_Click(object sender, EventArgs e)
	{
		this.WdvChangeNextShippingDate.Visible = (this.WdvChangeNextShippingDate.Visible == false);
	}

	/// <summary>
	/// 次回配送日更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateNextShippingDate_Click(object sender, EventArgs e)
	{
		// 次回配送日の入力チェック
		var errorMessage = CheckNextShippingDate();
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			this.WlNextShippingDateErrorMessage.Text = errorMessage;
			return;
		}

		// 入力情報作成
		var input = new FixedPurchaseInput(this.FixedPurchaseContainer)
		{
			NextShippingDate = this.WddlNextShippingDate.SelectedValue
		};

		var service = new FixedPurchaseService();
		if (this.WcbUpdateNextNextShippingDate.Checked)
		{
			var calculateMode = service.GetCalculationMode(input.FixedPurchaseKbn, Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
			input.NextNextShippingDate = new FixedPurchaseService().CalculateFollowingShippingDate(
				input.FixedPurchaseKbn,
				input.FixedPurchaseSetting1,
				DateTime.Parse(input.NextShippingDate),
				this.ShopShipping.FixedPurchaseMinimumShippingSpan,
				calculateMode).ToString();
		}

		// 更新
		var fixedPurchase = input.CreateModel();
		service.UpdateShippingDate(
			fixedPurchase.FixedPurchaseId,
			fixedPurchase.NextShippingDate,
			fixedPurchase.NextNextShippingDate,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);

		// 期間指定の頒布会の場合、次回配送商品を更新
		var sbService = new SubscriptionBoxService();
		var subscriptionBox = sbService.GetByCourseId(this.FixedPurchaseContainer.SubscriptionBoxCourseId);
		if ((string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId) == false)
			&& (subscriptionBox.IsNumberTime == false))
		{
			var getNextProductsResult = sbService.GetFixedPurchaseNextProduct(
				this.FixedPurchaseContainer.SubscriptionBoxCourseId,
				this.FixedPurchaseContainer.FixedPurchaseId,
				this.FixedPurchaseContainer.MemberRankId,
				this.FixedPurchaseContainer.NextNextShippingDate.Value,
				this.FixedPurchaseContainer.SubscriptionBoxOrderCount,
				this.FixedPurchaseContainer.Shippings[0]);
			service.UpdateNextDeliveryForSubscriptionBox(
				this.FixedPurchaseContainer.FixedPurchaseId,
				Constants.FLG_LASTCHANGED_BATCH,
				Constants.W2MP_ACCESSLOG_ENABLED,
				getNextProductsResult,
				UpdateHistoryAction.Insert);

			switch (getNextProductsResult.Result)
			{
				// 商品切り替え失敗時
				case SubscriptionBoxGetNextProductsResult.ResultTypes.Fail:
					Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
					break;

				// キャンセル時
				case SubscriptionBoxGetNextProductsResult.ResultTypes.Cancel:
					// 最新のユーザポイントを取得
					this.LoginUserPoint = PointOptionUtility.GetUserPoint(this.LoginUserId);
					// メール送信
					SendMailCommon.SendModifyFixedPurchaseMail(this.FixedPurchaseContainer.FixedPurchaseId, SendMailCommon.FixedPurchaseModify.OrderCancell);
					// 解約完了画面へ
					Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
					break;
			}
		}
		//メール送信
		SendMailCommon.SendModifyFixedPurchaseMail(this.FixedPurchaseContainer.FixedPurchaseId, SendMailCommon.FixedPurchaseModify.NextShippingDate);

		// 再表示
		Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
	}

	/// <summary>
	/// 次回配送日変更のキャンセルボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCancelNextShippingDate_Click(object sender, EventArgs e)
	{
		this.WdvChangeNextShippingDate.Visible = false;
	}

	/// <summary>
	/// 次回配送日選択可能な日付リストの算出
	/// </summary>
	/// <returns>次回配送日選択可能な日付リスト</returns>
	/// <remarks>次回配送日選択可能な日付リスト＝今日+「配送所要日数」～次回配送日+「次回配送日選択可能最大日数」</remarks>
	protected ListItemCollection GetChangeNextShippingDateList()
	{
		var nextDate = this.FixedPurchaseContainer.NextShippingDate;
		var nextNextDate = this.FixedPurchaseContainer.NextNextShippingDate;
		var result = new ListItemCollection();
		if ((nextDate.HasValue == false) || (nextNextDate.HasValue == false)) return result;

		// 次回配送日選択可能な日付リストを算出
		var target = DateTime.Now.Date.AddDays(this.ShopShipping.FixedPurchaseShippingDaysRequired);
		var tmpDate = nextDate.Value.Date.AddDays(this.ShopShipping.NextShippingMaxChangeDays);
		if (string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId) == false)
		{
			var subscriptionBoxService = new SubscriptionBoxService();
			var subscriptionBox = subscriptionBoxService.GetByCourseId(this.FixedPurchaseContainer.SubscriptionBoxCourseId);
			if (subscriptionBox.OrderItemDeterminationType == Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD)
			{
				var minSince = DateTime.Now;
				var maxUntil = DateTime.Now;
				foreach (var product in subscriptionBox.DefaultOrderProducts)
				{
					minSince = minSince < Convert.ToDateTime(product.TermSince) ? minSince : Convert.ToDateTime(product.TermSince);
					maxUntil = minSince > Convert.ToDateTime(product.TermUntil) ? minSince : Convert.ToDateTime(product.TermUntil);
				}
				tmpDate = maxUntil > tmpDate ? tmpDate : maxUntil;
				target = minSince > target ? minSince : target;
			}
		}

		// 次々回配送日より後の場合、次々回配送日の前に調整
		var endDate = (tmpDate < nextNextDate) ? tmpDate : nextNextDate.Value.Date.AddDays(-1);
		while (target <= endDate)
		{
			result.Add(
				new ListItem(
					DateTimeUtility.ToStringFromRegion(target, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter), 
					target.ToString("yyyy/MM/dd")));
			target = target.AddDays(1);
		}

		// 次回配送日が作成したリストに存在しない場合、追加
		if (result.FindByValue(nextDate.Value.ToString("yyyy/MM/dd")) == null)
		{
			result.Insert(
				0,
				new ListItem(
					DateTimeUtility.ToStringFromRegion(nextDate, DateTimeUtility.FormatType.LongDateWeekOfDay1Letter),
					nextDate.Value.ToString("yyyy/MM/dd")));
		}

		return result;
	}

	/// <summary>
	/// 次回配送日チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string CheckNextShippingDate()
	{
		// 出荷可能かのチェック
		var shipping = this.FixedPurchaseContainer.Shippings[0];
		var address = (Constants.TW_COUNTRY_SHIPPING_ENABLE
				&& IsCountryTw(shipping.ShippingCountryIsoCode))
			? shipping.ShippingAddr2
			: shipping.ShippingAddr1;
		if (OrderCommon.CanCalculateScheduledShippingDate(
			this.FixedPurchaseContainer.ShopId,
			DateTime.Parse(this.WddlNextShippingDate.SelectedValue),
			shipping.ShippingMethod,
			shipping.DeliveryCompanyId,
			shipping.ShippingCountryIsoCode,
			address,
			shipping.ShippingZip.Replace("-", string.Empty)) == false)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FIXED_PURCHASE_NEXT_SHIPPING_DATE_INVALID)
				.Replace(
					"@@ 1 @@",
					DateTimeUtility.ToStringFromRegion(
						HolidayUtil.GetShortestDeliveryDate(
							this.FixedPurchaseContainer.ShopId,
							shipping.DeliveryCompanyId,
							address,
							shipping.ShippingZip.Replace("-", string.Empty)),
						DateTimeUtility.FormatType.LongDateWeekOfDay1Letter));
		}

		return string.Empty;
	}

	/// <summary>
	/// 次回配送日ドロップダウンの選択肢変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlNextShippingDate_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.WlNextShippingDateErrorMessage.Text = CheckNextShippingDate();

		// 次回配送日・次々回配送日をセット
		if (this.WcbUpdateNextNextShippingDate.Checked)
		{
			var service = new FixedPurchaseService();
			var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
			input.NextShippingDate = this.WddlNextShippingDate.SelectedValue;
			var calculateMode = service.GetCalculationMode(input.FixedPurchaseKbn, Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
			this.FixedPurchaseContainer.NextNextShippingDate =
				service.CalculateFollowingShippingDate(
					input.FixedPurchaseKbn,
					input.FixedPurchaseSetting1,
					DateTime.Parse(input.NextShippingDate),
					this.ShopShipping.FixedPurchaseMinimumShippingSpan,
					calculateMode);
		}
	}

	/// <summary>
	/// Display Taiwan Address
	/// </summary>
	private void DisplayTaiwanAddress()
	{
		if (this.IsShippingAddrTw
			&& this.WddlShippingAddr2.HasInnerControl
			&& this.WddlShippingAddr3.HasInnerControl
			&& this.UseShippingAddress)
		{
			this.WddlShippingAddr2.SelectedValue = this.FixedPurchaseShippingContainer.ShippingAddr2;
			this.WddlShippingAddr3.DataSource = this.UserTwDistrictDict[this.WddlShippingAddr2.SelectedItem.ToString()];
			this.WddlShippingAddr3.DataBind();
			this.WddlShippingAddr3.ForceSelectItemByText(this.FixedPurchaseShippingContainer.ShippingAddr3);
			this.WtbShippingZipGlobal.Text = this.WddlShippingAddr3.SelectedValue.Split('|')[0];
		}
	}

	/// <summary>
	/// Is Display Button Convenience Store
	/// </summary>
	/// <param name="shippingStoreFlg">Shipping Store Flg</param>
	/// <returns>Is Display or not</returns>
	protected bool IsDisplayButtonConvenienceStore(string shippingStoreFlg)
	{
		var result = ((shippingStoreFlg
				== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
			&& Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED);

		return result;
	}

	/// <summary>
	/// Get Possible Shipping Type
	/// </summary>
	/// <param name="shippingStoreFlg">Shipping Store Flg</param>
	/// <returns>Possible Shipping Type</returns>
	protected ListItemCollection GetPossibleShippingType(string shippingStoreFlg)
	{
		var itemCollection = new ListItemCollection();
		var userShippingAddress = new UserShippingService()
			.GetAllOrderByShippingNoDesc(this.LoginUserId)
			.OrderBy(item => item.ShippingNo)
			.ToArray();
		var shippingModel = new ShopShippingService().Get(
			this.ShopId,
			this.ShopShipping.ShippingId);
		if ((shippingStoreFlg
				== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
			&& (this.FixedPurchaseContainer.OrderPaymentKbn
				== Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
			&& Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& CheckItemRelateWithServiceConvenienceStore(shippingModel))
		{
			itemCollection.AddRange(
				userShippingAddress.Where(item => (item.ShippingReceivingStoreFlg
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
				.Select(us => new ListItem(us.Name, us.ShippingNo.ToString())).ToArray());
			itemCollection.Add(new ListItem(
					ReplaceTag("@@DispText.shipping_addr_kbn_list.convenience_store@@"),
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE));
		}
		else
		{
			itemCollection.Add(
				new ListItem(
					ReplaceTag("@@DispText.shipping_addr_kbn_list.new@@"),
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW));
			itemCollection.Add(
				new ListItem(
					ReplaceTag("@@DispText.shipping_addr_kbn_list.owner@@"),
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER));

			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
				&& CheckItemRelateWithServiceConvenienceStore(shippingModel))
			{
				itemCollection.AddRange(
					userShippingAddress.Select(us => new ListItem(
						us.Name,
						us.ShippingNo.ToString()))
						.ToArray());
				itemCollection.Add(
					new ListItem(
						ReplaceTag("@@DispText.shipping_addr_kbn_list.convenience_store@@"),
						CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE));
			}
			else
			{
				var userShippingNormal =
					userShippingAddress.Where(
						item => item.ShippingReceivingStoreFlg
							== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF);

				itemCollection.AddRange(
					userShippingNormal.Select(us => new ListItem(
						us.Name,
						us.ShippingNo.ToString()))
						.ToArray());
			}
		}

		return itemCollection;
	}

	/// <summary>
	/// Shipping type Selected index change
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingType_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.WdvErrorPaymentAndShippingConvenience.Visible = false;
		switch (this.WddlShippingType.SelectedValue)
		{
			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
				this.WdvShippingInputFormInner.Visible = true;
				this.WtbOwnerAddress.Visible = false;
				this.WdvShippingInputFormConvenience.Visible = false;
				this.WrOrderShippingList.Visible = false;
				this.WspConvenienceStoreSelect.Visible = false;
				this.WhfCvsShopFlg.Value =
					Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
				this.WspConvenienceStoreEcPaySelect.Visible = false;
				this.WddlShippingReceivingStoreType.Visible = false;
				break;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
				this.WtbOwnerAddress.Visible = true;
				this.WdvShippingInputFormInner.Visible = false;
				this.WdvShippingInputFormConvenience.Visible = false;
				this.WrOrderShippingList.Visible = false;
				this.WspConvenienceStoreSelect.Visible = false;
				this.WhfCvsShopFlg.Value =
					Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF;
				this.WspConvenienceStoreEcPaySelect.Visible = false;
				this.WddlShippingReceivingStoreType.Visible = false;
				break;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE:
				this.WdvShippingInputFormConvenience.Visible = true;
				this.WtbOwnerAddress.Visible = false;
				this.WdvShippingInputFormInner.Visible = false;
				this.WrOrderShippingList.Visible = false;
				this.WspConvenienceStoreSelect.Visible = (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false);
				this.WspConvenienceStoreEcPaySelect.Visible = Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED;
				this.WddlShippingReceivingStoreType.Visible = Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED;
				this.WddlShippingReceivingStoreType.Items.Clear();
				this.WddlShippingReceivingStoreType.Items.AddRange(ShippingReceivingStoreType());
				this.WddlShippingReceivingStoreType.Items.Cast<ListItem>().ToList()
					.ForEach(li => li.Selected = (li.Value == this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreType));

				if (this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg
					== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
				{
					this.WhfShippingReceivingStoreId.Value = string.Empty;
					this.WhfShippingReceivingStoreName.Value = string.Empty;
					this.WhfShippingReceivingStoreAddr.Value = string.Empty;
					this.WhfShippingReceivingStoreTel.Value = string.Empty;

					this.WlCvsShopId.Text = string.Empty;
					this.WlCvsShopName.Text = string.Empty;
					this.WlCvsShopAddress.Text = string.Empty;
					this.WlCvsShopTel.Text = string.Empty;
				}
				this.WhfCvsShopFlg.Value
					= Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
				break;

			// アドレス帳選択時
			default:
				this.WrOrderShippingList.Visible = true;
				this.WdvShippingInputFormInner.Visible = false;
				this.WtbOwnerAddress.Visible = false;
				this.WdvShippingInputFormConvenience.Visible = false;

				var shippingAddres = this.UserShippingAddr
					.FirstOrDefault(item => item.ShippingNo
						== int.Parse(this.WddlShippingType.SelectedValue));
				if (shippingAddres != null)
				{
					var shippingReceivingStoreFlg = shippingAddres.ShippingReceivingStoreFlg;
					this.WhfCvsShopFlg.Value = shippingReceivingStoreFlg;
					this.WhfSelectedShopId.Value = shippingAddres.ShippingReceivingStoreId;
					this.WspConvenienceStoreSelect.Visible =
						((shippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
							&& (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false));
					this.WspConvenienceStoreEcPaySelect.Visible = false;
					this.WddlShippingReceivingStoreType.Visible =
						((shippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
							&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED);

					if ((shippingAddres.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
						&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
					{
						var shippingReceivingStoreType = shippingAddres.ShippingReceivingStoreType;
						this.WddlShippingReceivingStoreType.Items.Clear();
						this.WddlShippingReceivingStoreType.AddItem(new ListItem(
							ValueText.GetValueText(
								Constants.TABLE_ORDERSHIPPING,
								Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE,
								shippingAddres.ShippingReceivingStoreType),
							shippingReceivingStoreType));
						this.WdvErrorPaymentAndShippingConvenience.Visible = ((this.FixedPurchaseContainer.OrderPaymentKbn
								!= Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
							&& (ECPayUtility.GetIsCollection(shippingReceivingStoreType)
								== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON));
					}
				}

				foreach (RepeaterItem item in this.WrOrderShippingList.Items)
				{
					var shippingNo = ((HiddenField)item.FindControl("hfShippingNo")).Value;
					item.Visible = (shippingNo == this.WddlShippingType.SelectedValue);
				}
				break;
		}

		this.WdvShippingTime.Visible = (this.WhfCvsShopFlg.Value
			== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF);
	}

	/// <summary>
	/// Click open window EcPay
	/// </summary>
	/// <param name="sender">Object</param>
	/// <param name="e">Event Args</param>
	protected new void lbOpenEcPay_Click(object sender, EventArgs e)
	{
		var wddlShippingReceivingStoreType = GetWrappedControl<WrappedDropDownList>(
			(Control)sender,
			"ddlShippingReceivingStoreType");
		var shippingService = wddlShippingReceivingStoreType.SelectedValue;
		this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreFlg = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
		this.FixedPurchaseContainer.Shippings[0].ShippingReceivingStoreType = wddlShippingReceivingStoreType.SelectedValue;

		var baseUrl = string.Format(
			"{0}{1}{2}",
			Constants.PROTOCOL_HTTP,
			(string.IsNullOrEmpty(Constants.WEBHOOK_DOMAIN)
				? Constants.SITE_DOMAIN
				: Constants.WEBHOOK_DOMAIN),
			this.RawUrl);

		var uri = new Uri(baseUrl);
		var newQueryString = HttpUtility.ParseQueryString(uri.Query);
		newQueryString[Constants.REQUEST_KEY_SHIPPING_RECEIVING_STORE_TYPE] = shippingService;

		var url = string.Format("{0}?{1}", uri.GetLeftPart(UriPartial.Path), newQueryString);
		var parameters = ECPayUtility.CreateParametersForOpenConvenienceStoreMap(
			shippingService,
			url,
			this.IsSmartPhone);

		var json = JsonConvert.SerializeObject(parameters);
		var script = "NextPageSelectReceivingStore(JSON.parse('" + json + "'));";
		ScriptManager.RegisterStartupScript(
			this.Page,
			this.GetType(),
			"ReceivingStore",
			script,
			true);
	}

	/// <summary>
	/// Set Information Receiving Store
	/// </summary>
	public void SetInformationReceivingStore()
	{
		if (this.PostParams.Count == 0) return;

		var shippingReceivingStoreTypeSelected = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_SHIPPING_RECEIVING_STORE_TYPE]);
		var isPaymentConvenienceStore = (this.FixedPurchaseContainer.OrderPaymentKbn
			!= Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE);

		// Set convenience store display
		this.WddlShippingType.SelectedValue = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE;
		this.IsUserShippingModify = true;
		this.WspConvenienceStoreSelect.Visible = (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false);
		this.WspConvenienceStoreEcPaySelect.Visible = Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED;

		// Set convenience store data
		this.WhfShippingReceivingStoreId.Value = this.PostParams[ECPayConstants.PARAM_CVSSTOREID];
		this.WhfShippingReceivingStoreName.Value = this.PostParams[ECPayConstants.PARAM_CVSSTORENAME];
		this.WhfShippingReceivingStoreAddr.Value = this.PostParams[ECPayConstants.PARAM_CVSADDRESS];
		this.WhfShippingReceivingStoreTel.Value = this.PostParams[ECPayConstants.PARAM_CVSTELEPHONE];
		this.WhfCvsShopFlg.Value = Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
		this.WddlShippingReceivingStoreType.SelectedValue = shippingReceivingStoreTypeSelected;

		this.WlCvsShopId.Text = this.PostParams[ECPayConstants.PARAM_CVSSTOREID];
		this.WlCvsShopName.Text = this.PostParams[ECPayConstants.PARAM_CVSSTORENAME];
		this.WlCvsShopAddress.Text = this.PostParams[ECPayConstants.PARAM_CVSADDRESS];
		this.WlCvsShopTel.Text = this.PostParams[ECPayConstants.PARAM_CVSTELEPHONE];

		this.WdvShippingInputFormConvenience.Visible = true;
		this.WdvShippingInputFormInner.Visible = false;
		this.WddlShippingReceivingStoreType.Visible = true;
		this.WdvErrorPaymentAndShippingConvenience.Visible = (isPaymentConvenienceStore
			&& (ECPayUtility.GetIsCollection(shippingReceivingStoreTypeSelected)
				== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON));
	}

	/// <summary>
	/// Check Store Id Valid
	/// </summary>
	/// <param name="storeId">Convenience Store Id</param>
	/// <returns>Store is valid or not</returns>
	[WebMethod]
	public static bool CheckStoreIdValid(string storeId)
	{
		var result = (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
			|| OrderCommon.CheckIdExistsInXmlStore(storeId));
		return result;
	}

	/// <summary>
	/// 次回購入の利用ポイントの全適用フラグ変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUseAllPointFlg_Changed(object sender, EventArgs e)
	{
		var useAllPointFlgInputMethod = (CheckBox)sender;
		if (useAllPointFlgInputMethod.Checked && this.CanUseAllPointFlg)
		{
			this.WtbNextShippingUsePoint.Enabled = false;
			// 全ポイント継続利用するため、通常の利用ポイントは0にする
			this.WtbNextShippingUsePoint.Text = "0";
		}
		else
		{
			this.WtbNextShippingUsePoint.Enabled = true;
		}
	}

	/// <summary>
	/// 次回購入の利用ポイントの全適用フラグデータバインド時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUseAllPointFlg_DataBinding(object sender, EventArgs e)
	{
		var useAllPointFlgInputMethod = (CheckBox)sender;
		useAllPointFlgInputMethod.Checked = (this.FixedPurchaseContainer.UseAllPointFlg == Constants.FLG_FIELD_FIXEDPURCHASE_USE_ALL_POINT_FLG_ON);

		if (useAllPointFlgInputMethod.Checked && this.CanUseAllPointFlg)
		{
			this.WtbNextShippingUsePoint.Enabled = false;
			// 全ポイント継続利用するため、通常の利用ポイントは0にする
			this.WtbNextShippingUsePoint.Text = "0";
		}
		else
		{
			this.WtbNextShippingUsePoint.Enabled = true;
		}
	}

	/// <summary>
	/// 「利用クーポン変更」ボタン押下イベント（利用クーポンの更新フォーム表示）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayUpdateNextShippingUseCouponForm_Click(object sender, System.EventArgs e)
	{
		this.WdvNextShippingUseCoupon.Visible = (this.WdvNextShippingUseCoupon.Visible == false);

		this.WddlNextShippingUseCouponList.Visible = (this.WrblCouponInputMethod.SelectedValue != CouponOptionUtility.COUPON_INPUT_METHOD_MANUAL_INPUT);
		this.WdvCouponCodeInputArea.Visible = (this.WrblCouponInputMethod.SelectedValue == CouponOptionUtility.COUPON_INPUT_METHOD_MANUAL_INPUT);
		this.WddlNextShippingUseCouponList.SelectedValue = string.Empty;
		this.WtbNextShippingUseCouponCode.Text = string.Empty;
	}

	/// <summary>
	/// 次回購入に利用するクーポンをキャンセルする
	/// </summary>
	protected void lbCancelNextShippingUseCoupon_Click(object sender, EventArgs e)
	{
		// クーポンOPが無効、または、ユーザではない、または、クーポンキャンセル済みの場合、何も処理しない
		if ((Constants.W2MP_COUPON_OPTION_ENABLED == false)
			|| (this.IsLoggedIn == false)
			|| string.IsNullOrEmpty(this.FixedPurchaseContainer.NextShippingUseCouponId)) return;

		// 利用クーポン中のクーポン情報をキャンセルする
		var success = FixedPurchaseHelper.ChangeNextShippingUseCoupon(
			this.FixedPurchaseContainer,
			null,
			false,
			Constants.FLG_LASTCHANGED_USER);

		if (success)
		{
			// 再表示
			this.Response.Redirect(
				PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
		}
		else
		{
			// エラーメッセージを表示
			this.NextShippingUseCouponErrorMessage =
				CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_NEXT_SHIPPING_USE_COUPON_CANCEL_ALERT);
		}
	}

	/// <summary>
	/// 利用可能クーポンリストアイテム作成
	/// </summary>
	/// <returns>利用可能クーポンリストアイテム</returns>
	protected ListItemCollection GetUsableCouponListItems()
	{
		var list = new ListItemCollection { "" };
		if (this.UsableCoupons.Length == 0) return list;

		list.AddRange(this.UsableCoupons.Select(c => new ListItem(c.CouponDispName, c.CouponCode)).ToArray());
		return list;
	}

	/// <summary>
	/// クーポン入力方法変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblCouponInputMethod_SelectedIndexChanged(object sender, EventArgs e)
	{
		var couponInputMethod = (RadioButtonList)sender;

		this.WddlNextShippingUseCouponList.Visible = (couponInputMethod.Text != CouponOptionUtility.COUPON_INPUT_METHOD_MANUAL_INPUT);
		this.WdvCouponCodeInputArea.Visible = (couponInputMethod.Text == CouponOptionUtility.COUPON_INPUT_METHOD_MANUAL_INPUT);
		this.WtbNextShippingUseCouponCode.Text = string.Empty;
		this.WddlNextShippingUseCouponList.SelectedValue = string.Empty;
	}

	/// <summary>
	/// クーポン入力方法データバインド時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblCouponInputMethod_DataBinding(object sender, EventArgs e)
	{
		var couponInputMethod = (RadioButtonList)sender;
		couponInputMethod.SelectedValue = (this.UsableCoupons.Length > 0)
			? CouponOptionUtility.COUPON_INPUT_METHOD_SELECT
			: CouponOptionUtility.COUPON_INPUT_METHOD_MANUAL_INPUT;
	}

	/// <summary>
	/// クーポンBOXクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbShowCouponBox_Click(object sender, EventArgs e)
	{
		this.WtbNextShippingUseCouponCode.Text = string.Empty;
		this.WddlNextShippingUseCouponList.SelectedValue = string.Empty;
		this.WdvCouponBox.Visible = true;
	}

	/// <summary>
	/// モーダルクーポンBOX 閉じるボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCouponBoxClose_Click(object sender, EventArgs e)
	{
		this.WdvCouponBox.Visible = false;
	}

	/// <summary>
	/// モーダルクーポンBOX クーポン選択時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCouponSelect_Click(object sender, EventArgs e)
	{
		var lbCouponSelect = (LinkButton)sender;
		var selectedCouponCode = (HiddenField)lbCouponSelect.Parent.FindControl("hfCouponBoxCouponCode");
		this.WtbNextShippingUseCouponCode.Text = selectedCouponCode.Value;
		this.WddlNextShippingUseCouponList.SelectedValue = selectedCouponCode.Value;
		this.WdvCouponBox.Visible = false;
	}

	/// <summary>
	/// 「クーポン適用」 ボタン押下イベント（利用クーポンをDBに更新）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateUseCoupon_Click(object sender, EventArgs e)
	{
		// クーポンOPが無効、または、ユーザではない、またはクーポン更新済みの場合、何も処理しない
		if ((Constants.W2MP_COUPON_OPTION_ENABLED == false)
			|| (this.IsLoggedIn == false)
			|| (string.IsNullOrEmpty(this.FixedPurchaseContainer.NextShippingUseCouponId) == false)) return;

		// 入力したクーポンコード取得
		var couponCode = (string.IsNullOrEmpty(this.WtbNextShippingUseCouponCode.Text.Trim()))
			? this.WddlNextShippingUseCouponList.SelectedValue
			: StringUtility.ToHankaku(this.WtbNextShippingUseCouponCode.Text).Trim();

		// 利用クーポンをリセットするか
		var isNotResetUseCoupon = (string.IsNullOrEmpty(couponCode) == false);

		// 空のクーポンコードを入力した場合、かつ次回購入利用クーポンが未設定であり、エラーメッセージを表示する
		if ((isNotResetUseCoupon == false)
			&& string.IsNullOrEmpty(this.FixedPurchaseContainer.NextShippingUseCouponId))
		{
			this.NextShippingUseCouponErrorMessage = CommerceMessages.GetMessages(
				CommerceMessages.ERRMSG_FRONT_NEXT_SHIPPING_USE_COUPON_NO_CHANGE_ERROR);
			return;
		}

		// 次回購入の利用クーポンの更新をエラーチェック
		var errorMessage = string.Empty;
		UserCouponDetailInfo inputCoupon = null;
		if (isNotResetUseCoupon)
		{
			// クーポン詳細情報取得
			var inputCoupons = new CouponService().GetAllUserCouponsFromCouponCode(
				Constants.W2MP_DEPT_ID,
				this.FixedPurchaseContainer.UserId,
				couponCode);

			// クーポンを利用可能のチェック
			inputCoupon = FixedPurchaseHelper.CheckAndGetUseCouponForNextShipping(
				couponCode,
				inputCoupons,
				this.FixedPurchaseContainer,
				CreateSimpleCartListForFixedPurchase(this.FixedPurchaseContainer.FixedPurchaseId),
				out errorMessage);
		}
		if (errorMessage != string.Empty)
		{
			this.NextShippingUseCouponErrorMessage = errorMessage;
			return;
		}

		//次回購入利用クーポンで割引額が満たされる場合は、次回購入利用ポイントを0にする
		if ((inputCoupon != null) && this.FixedPurchaseContainer.NextShippingUsePoint > 0)
		{
			this.NextShippingFixedPurchaseCart = CreateSimpleCartListForFixedPurchase(this.FixedPurchaseContainer.FixedPurchaseId).Items[0];

			var discountable = FixedPurchaseHelper.CheckDiscountableForNextFixedPurchase(
				inputCoupon,
				this.FixedPurchaseContainer.NextShippingUsePoint,
				this.NextShippingFixedPurchaseCart.PriceSubtotalForCampaign);

			if (discountable == false)
			{
				new FixedPurchaseService().ApplyNextShippingUsePointChange(
					Constants.CONST_DEFAULT_DEPT_ID,
					this.FixedPurchaseContainer,
					0,
					this.FixedPurchaseContainer.LastChanged,
					UpdateHistoryAction.Insert);

				this.PointResetMessages = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_NEXT_SHIPPING_USE_POINT_RESET_ALERT);
				this.LoginUserPoint = PointOptionUtility.GetUserPoint((string)this.LoginUserId);
			}
		}

		// 利用クーポン情報更新の実行
		var success = FixedPurchaseHelper.ChangeNextShippingUseCoupon(
			this.FixedPurchaseContainer,
			inputCoupon,
			isNotResetUseCoupon,
			Constants.FLG_LASTCHANGED_USER);

		if (success)
		{
			// 再表示
			Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
		}
		else
		{
			// エラーメッセージを表示
			this.NextShippingUseCouponErrorMessage = CommerceMessages.GetMessages(
				CommerceMessages.ERRMSG_FRONT_NEXT_SHIPPING_USE_COUPON_UPDATE_ALERT);
		}
	}

	/// <summary>
	/// 利用ポクーポン変更フォーム非表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCloseUpdateNextShippingUseCouponForm_Click(object sender, System.EventArgs e)
	{
		this.WdvNextShippingUseCoupon.Visible = false;
	}

	/// <summary>
	/// 領収書情報変更フォーム表示
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDisplayReceiptInfoForm_Click(object sender, EventArgs e)
	{
		this.IsReceiptInfoModify = (this.IsReceiptInfoModify == false);
		if (this.IsReceiptInfoModify == false) return;

		// 領収書情報を入力項目にセット
		this.WddlReceiptFlg.SelectedValue = this.FixedPurchaseContainer.ReceiptFlg;
		this.WtbReceiptAddress.Text = this.FixedPurchaseContainer.ReceiptAddress;
		this.WtbReceiptProviso.Text = this.FixedPurchaseContainer.ReceiptProviso;

		// 宛名・但し書き入力項目表示の制御
		ddlReceiptFlg_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 領収書希望有無ドロップダウンリスト変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void ddlReceiptFlg_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		// 宛名・但し書き入力項目表示の制御
		this.WtrReceiptAddressInput.Visible
			= this.WtrReceiptProvisoInput.Visible
			= this.WdvReceiptAddressProvisoInput.Visible
			= (this.WddlReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON);
	}

	/// <summary>
	/// 領収書情報更新ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateReceiptInfo_Click(object sender, EventArgs e)
	{
		// 領収書対応OPが無効、または、ユーザではない場合、何も処理しない
		if ((Constants.RECEIPT_OPTION_ENABLED == false) || (this.IsLoggedIn == false)) return;

		var hasReceipt = (this.WddlReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON);
		var address = StringUtility.RemoveUnavailableControlCode(this.WtbReceiptAddress.Text.Trim());
		var proviso = StringUtility.RemoveUnavailableControlCode(this.WtbReceiptProviso.Text.Trim());

		// 領収書希望ありの時に、宛名と但し書きの入力チェックを行う
		if (hasReceipt)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_FIXEDPURCHASE_RECEIPT_ADDRESS, address },
				{ Constants.FIELD_FIXEDPURCHASE_RECEIPT_PROVISO, proviso },
			};
			var errorMessages = Validator.ValidateAndGetErrorContainer("ReceiptRegisterModify", input);
			if (errorMessages.Count != 0)
			{
				// カスタムバリデータ取得
				var customValidators = new List<CustomValidator>();
				CreateCustomValidators(this, customValidators);
				// エラーをカスタムバリデータへ
				SetControlViewsForError("ReceiptRegisterModify", errorMessages, customValidators);
				return;
			}
		}

		// 領収書情報更新
		var updated = new FixedPurchaseService().UpdateReceiptInfo(
			this.FixedPurchaseContainer.FixedPurchaseId,
			hasReceipt ? Constants.FLG_ORDER_RECEIPT_FLG_ON : Constants.FLG_ORDER_RECEIPT_FLG_OFF,
			hasReceipt ? address : string.Empty,
			hasReceipt ? proviso : string.Empty,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);
		if (updated > 0)
		{
			// 再表示
			Response.Redirect(
				PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
		}
		else
		{
			// エラーメッセージ表示
			this.ReceiptInfoModifyErrorMessage = WebSanitizer.HtmlEncodeChangeToBr(
				CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_RECEIPT_INFO_UPDATE_ERROR));
		}
	}

	/// <summary>
	/// Check Item Relate With Service Convenience Store
	/// </summary>
	/// <param name="shopShipping">Shop shipping</param>
	/// <returns>True: shipping Kbn is convenience store and relate product </returns>
	protected bool CheckItemRelateWithServiceConvenienceStore(ShopShippingModel shopShipping)
	{
		var deliveryCompanyList = (
			this.FixedPurchaseContainer.Shippings[0].ShippingMethod
					== Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
				? shopShipping.CompanyListExpress
				: shopShipping.CompanyListMail;
		var deliveryCompanyIds = deliveryCompanyList
				.Select(model => model.DeliveryCompanyId)
				.Distinct()
				.ToArray();
		var result = deliveryCompanyIds.Any(item => item
			== Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID);

		return result;
	}

	/// <summary>
	/// Check Valid Tel No And Country For PaymentAtone And Aftee
	/// </summary>
	/// <param name="paymentId">Payment Id</param>
	/// <returns>True: If Valid Tel No And Country Or Flase: If Invalid Tel No And Country </returns>
	protected bool CheckValidTelNoAndCountryForPaymentAtoneAndAftee(string paymentId)
	{
		var isCountryTw
			= (GlobalAddressUtil.IsCountryTw(this.FixedPurchaseContainer.Shippings[0].ShippingCountryIsoCode));
		var isCountryJp
			= (GlobalAddressUtil.IsCountryJp(this.FixedPurchaseContainer.Shippings[0].ShippingCountryIsoCode));
		var errorMessages = string.Empty;
		var result = false;
		var wspanErrorMessageForAtone =
			GetWrappedControl<WrappedHtmlGenericControl>(this.AtoneRepeaterItem, "spanErrorMessageForAtone");
		var wspanErrorMessageForAftee =
			GetWrappedControl<WrappedHtmlGenericControl>(this.AfteeRepeaterItem, "spanErrorMessageForAftee");
		switch (paymentId)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
				if (isCountryTw == false)
				{
					if (string.IsNullOrEmpty(errorMessages) == false) errorMessages += Environment.NewLine;
					errorMessages += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_AFTEE);
				}
				result = string.IsNullOrEmpty(errorMessages);
				if (result == false)
				{
					if (wspanErrorMessageForAftee.HasInnerControl)
					{
						wspanErrorMessageForAftee.InnerHtml =
							WebSanitizer.HtmlEncodeChangeToBr(errorMessages);
						wspanErrorMessageForAftee.InnerControl.Style["display"] = "block";
					}
				}
				return result;

			case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
				if (isCountryJp == false)
				{
					if (string.IsNullOrEmpty(errorMessages) == false) errorMessages += Environment.NewLine;
					errorMessages += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_COUNTRY_FOR_PAYMENT_ATONE);
				}

				result = string.IsNullOrEmpty(errorMessages);
				if (result == false)
				{
					if (wspanErrorMessageForAtone.HasInnerControl)
					{
						wspanErrorMessageForAtone.InnerHtml =
							WebSanitizer.HtmlEncodeChangeToBr(errorMessages);
						wspanErrorMessageForAtone.InnerControl.Style["display"] = "block";
					}
				}
				return result;

			default:
				if (wspanErrorMessageForAtone.HasInnerControl)
					wspanErrorMessageForAtone.InnerControl.Style["display"] = "none";

				if (wspanErrorMessageForAftee.HasInnerControl)
					wspanErrorMessageForAftee.InnerControl.Style["display"] = "none";
				return result;
		}
	}

	/// <summary>
	/// DropDownList shipping receiving store type selected index changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void ddlShippingReceivingStoreType_SelectedIndexChanged(object sender, EventArgs e)
	{
		var wddlShippingReceivingStoreType = GetWrappedControl<WrappedDropDownList>(
			((Control)sender).Parent,
			"ddlShippingReceivingStoreType");
		var isPaymentConvenienceStore = (this.FixedPurchaseContainer.OrderPaymentKbn !=
			Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE);

		this.WdvErrorPaymentAndShippingConvenience.Visible = (isPaymentConvenienceStore
			&& (ECPayUtility.GetIsCollection(wddlShippingReceivingStoreType.SelectedValue)
				== Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON));

		var whfCvsShopId = GetWrappedControl<WrappedHiddenField>(wddlShippingReceivingStoreType.Parent, "hfCvsShopId");
		var whfCvsShopName = GetWrappedControl<WrappedHiddenField>(wddlShippingReceivingStoreType.Parent, "hfCvsShopName");
		var whfCvsShopAddress = GetWrappedControl<WrappedHiddenField>(wddlShippingReceivingStoreType.Parent, "hfCvsShopAddress");
		var whfCvsShopTel = GetWrappedControl<WrappedHiddenField>(wddlShippingReceivingStoreType.Parent, "hfCvsShopTel");
		var wlCvsShopId = GetWrappedControl<WrappedLiteral>(wddlShippingReceivingStoreType.Parent, "lCvsShopId");
		var wlCvsShopName = GetWrappedControl<WrappedLiteral>(wddlShippingReceivingStoreType.Parent, "lCvsShopName");
		var wlCvsShopAddress = GetWrappedControl<WrappedLiteral>(wddlShippingReceivingStoreType.Parent, "lCvsShopAddress");
		var wlCvsShopTel = GetWrappedControl<WrappedLiteral>(wddlShippingReceivingStoreType.Parent, "lCvsShopTel");

		whfCvsShopId.Value = string.Empty;
		whfCvsShopName.Value = string.Empty;
		whfCvsShopAddress.Value = string.Empty;
		whfCvsShopTel.Value = string.Empty;

		wlCvsShopId.Text = string.Empty;
		wlCvsShopName.Text = string.Empty;
		wlCvsShopAddress.Text = string.Empty;
		wlCvsShopTel.Text = string.Empty;
	}

	/// <summary>
	/// 配送方法選択ドロップダウン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingMethod_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var shippingMethod = (((Control)sender).ID == "ddlShippingMethod")
			? this.WddlShippingMethod
			: this.WddlShippingMethodForAmazonPay;

		var shippingCompany = (((Control)sender).ID == "ddlShippingMethod")
			? this.WddlDeliveryCompany
			: this.WddlDeliveryCompanyForAmazonPay;

		var deliveryCompany = new DeliveryCompanyService().Get(
			new ShopShippingService().GetDefaultCompany(
				this.FixedPurchaseContainer.ShippingType,
				shippingMethod.SelectedValue).DeliveryCompanyId);
		if (deliveryCompany == null) return;

		this.SelectedDeliveryCompany = deliveryCompany;
		shippingCompany.Items.Clear();

		// 配送サービスドロップダウン作成
		// メール便配送サービス絞り込み
		var shippingCompanyItem = (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED
			&& (shippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL))
			? GetCompanyListMail()
				.Select(c => new ListItem(c.DeliveryCompanyName, c.DeliveryCompanyId))
				.ToArray()
			: DataCacheControllerFacade.GetShopShippingCacheController()
				.Get(this.FixedPurchaseContainer.ShippingType)
				.CompanyList.Where(ss => ss.ShippingKbn == shippingMethod.SelectedValue)
				.Select(cm => new DeliveryCompanyService().Get(cm.DeliveryCompanyId))
				.Select(kvp => new ListItem(kvp.DeliveryCompanyName, kvp.DeliveryCompanyId)).ToArray();
		shippingCompany.Items.AddRange(shippingCompanyItem);

		// 配送サービスドロップダウンが変更されているかもしれないので変更時メソッド呼び出し
		ddlDeliveryCompanyList_OnSelectedIndexChanged(shippingCompany.InnerControl, e);
	}

	/// <summary>
	/// 配送サービス選択ドロップダウン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void ddlDeliveryCompanyList_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var shippingMethod = (((Control)sender).ID == "ddlDeliveryCompany")
			? this.WddlShippingMethod
			: this.WddlShippingMethodForAmazonPay;

		var shippingCompany = (((Control)sender).ID == "ddlDeliveryCompany")
			? this.WddlDeliveryCompany
			: this.WddlDeliveryCompanyForAmazonPay;

		var shippingTime = (((Control)sender).ID == "ddlDeliveryCompany")
			? this.WddlShippingTime
			: this.WddlShippingTimeForAmazonPay;

		this.SelectedDeliveryCompany = new DeliveryCompanyService().Get(shippingCompany.SelectedValue);
		shippingTime.Items.Clear();

		// メール便なら配送希望時間帯のドロップダウンリストはセットしない
		if (shippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL) return;

		shippingTime.Items.AddRange(this.SelectedDeliveryCompany.GetShippingTimeList().Select(kvp => new ListItem(kvp.Value, kvp.Key)).ToArray());
	}

	/// <summary>
	/// 商品エリアリピータイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rItem_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		// ドロップダウンに表示するバリエーションIDを指定
		var ddlControl = (DropDownList)e.Item.FindControl("ddlProductVariationList");
		if (ddlControl == null) return;
		var variationId = ddlControl.SelectedValue;

		if (this.TopVariationId == variationId)
		{
			ddlControl.SelectedIndex = this.SelectedVariationIndex;
		}

		if (this.DisplayProductOptionValueArea == false)
		{
			var fpItemInput = (FixedPurchaseItemInput)e.Item.DataItem;
			this.DisplayProductOptionValueArea = fpItemInput.ProductOptionSettingsWithSelectedValues.Items.Count > 0;
		}

		var sProductPriceControl = (HtmlGenericControl)e.Item.FindControl("sProductPrice");
		var sOrderSubtotalControl = (HtmlGenericControl)e.Item.FindControl("sOrderSubtotal");
		var sQuantityUpdate = (HtmlGenericControl)e.Item.FindControl("sItemQuantity");
		var hfProductPrice = (HiddenField)e.Item.FindControl("hfProductPrice");
		var hfProductId = (HiddenField)e.Item.FindControl("hfProductId");
		var hfVariationId = (HiddenField)e.Item.FindControl("hfVariationId");

		if (string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId)) return;

		var selectedSubscriptionBox = DataCacheControllerFacade
			.GetSubscriptionBoxCacheController()
			.Get(this.FixedPurchaseContainer.SubscriptionBoxCourseId);

		var subscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
			x => (x.ProductId == hfProductId.Value)
				&& (x.VariationId == hfVariationId.Value));

		// 頒布会キャンペーン期間の場合キャンペーン期間価格を適用
		if (OrderCommon.IsSubscriptionBoxCampaignPeriodByNextShippingDate(subscriptionBoxItem, (DateTime)this.FixedPurchaseContainer.NextShippingDate))
		{
			sProductPriceControl.InnerText = CurrencyManager.ToPrice(subscriptionBoxItem.CampaignPrice.ToPriceDecimal());
			hfProductPrice.Value = subscriptionBoxItem.CampaignPrice.ToPriceString();
			sOrderSubtotalControl.InnerText = CurrencyManager.ToPrice(subscriptionBoxItem.CampaignPrice.ToPriceDecimal() * decimal.Parse(sQuantityUpdate.InnerText));
		}
	}

	/// <summary>
	/// バリエーション変更ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbChangeProductVariation_Click(object sender, EventArgs e)
	{
		// 商品ID・バリエーションID取得
		var linkButton = (LinkButton)sender;
		var productInfo = linkButton.CommandArgument;
		var ids = productInfo.Split(',');
		this.WhfProductId.Value = ids[0];
		var variationId = ids[1];
		this.WhfOldVariationId.Value = variationId;
		this.ModifyProductIndex = (ids.Length > 2) ? int.Parse(ids[2]) : (int?)null;

		// 商品付帯情報の編集情報初期化
		this.WhfProductOptionTexts.Value = string.Empty;
		this.WdvProductOptionValueErrorMessage.Visible = false;

		// 変更前金額の取得
		var target = this.FixedPurchaseContainer.Shippings[0].Items
			.FirstOrDefault(item => item.VariationId == variationId);
		if (target != null)
		{
			this.WhfOldSubtotal.Value = CurrencyManager.ToPrice(target.ItemPrice);
		}

		// ドロップダウンの初期選択バリエーションID取得
		var productVariations = ProductCommon.GetProductInfo(this.ShopId, this.WhfProductId.Value, this.MemberRankId, this.UserFixedPurchaseMemberFlg);
		this.WhfSelectedVariationId.Value = (string)productVariations[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID];
		this.TopVariationId = this.WhfSelectedVariationId.Value;
		this.SelectedVariationIndex = 0;

		// 商品バリエーション更新エリア表示
		VariationUpdateAreaDisplay();
	}

	/// <summary>
	/// 商品バリエーションドロップダウン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlProductVariationList_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		DropDownList control = (DropDownList)sender;
		this.WhfSelectedVariationId.Value = control.SelectedValue;
		this.SelectedVariationIndex = control.SelectedIndex;

		// 商品バリエーション更新エリア表示
		VariationUpdateAreaDisplay();
	}

	/// <summary>
	/// バリエーション更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdateProductVariation_Click(object sender, EventArgs e)
	{
		// 商品情報取得
		var service = new FixedPurchaseService();
		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		var inputItems = input.Shippings[0].Items;
		var targetItemNo = inputItems.First(item => (item.VariationId == this.WhfOldVariationId.Value)).FixedPurchaseItemNo;

		// 重複チェック
		if (inputItems.Any(
			item => ((item.FixedPurchaseItemNo != targetItemNo)
				&& (item.VariationId == this.WhfSelectedVariationId.Value))))
		{
			// エラーメッセージ表示
			this.WdvVariationErrorMessage.Visible = true;
			this.WdvVariationErrorMessage.InnerHtml = WebMessages
				.GetMessages(WebMessages.ERRMSG_FRONT_FIXED_PURCHASE_PRODUCT_VARIATION_DUPLICATE)
				.Replace("@@ 1 @@", this.WhfVariationName.Value);
			return;
		}

		// 更新対象の商品情報取得
		input.Shippings[0].Items = GetItems().ToArray();
		var model = input.CreateModel();

		// 更新(履歴含む）
		service.UpdateItems(
			model.Shippings[0].Items,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);

		// メール送信
		SendMailCommon.SendModifyFixedPurchaseMail(model.Shippings[0].FixedPurchaseId, SendMailCommon.FixedPurchaseModify.Product);

		// 再表示
		Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(model.Shippings[0].FixedPurchaseId));
	}

	/// <summary>
	/// バリエーション更新キャンセル
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCloseUpdateProductVariationForm_Click(object sender, EventArgs e)
	{
		this.ModifyProductIndex = null;
		this.WhfOldVariationId.Value = string.Empty;
		this.WdvVariationErrorMessage.Visible = false;
		var fpItemInputs = GetFixedPurchaseItemInputs(new FixedPurchaseInput(this.FixedPurchaseContainer));
		this.WrItem.DataSource = fpItemInputs;
		this.WrItem.DataBind();
	}

	/// <summary>
	/// 商品付帯情報変更ボタンの表示判定
	/// </summary>
	/// <param name="riItem">定期購入商品リピーターアイテム</param>
	/// <returns>商品付帯情報変更ボタンを表示するか否か</returns>
	protected bool IsDisplayButtonChangeProductOptionValue(RepeaterItem riItem)
	{
		var fpItemInput = (FixedPurchaseItemInput)riItem.DataItem;
		var isDisplayButton = (fpItemInput.ProductOptionSettingsWithSelectedValues.Items.Count > 0)
			&& (this.ModifyProductIndex != riItem.ItemIndex);
		return isDisplayButton;
	}

	/// <summary>
	/// 商品付帯情報入力エリアの表示判定
	/// </summary>
	/// <param name="riItem">定期購入商品リピーターアイテム</param>
	/// <returns>商品付帯情報入力エリアを表示するか否か</returns>
	protected bool IsDisplayProductOptionValueArea(RepeaterItem riItem)
	{
		var fpItemInput = (FixedPurchaseItemInput)riItem.DataItem;
		var isDisplayArea = (fpItemInput.ProductOptionSettingsWithSelectedValues.Items.Count > 0)
			&& (this.ModifyProductIndex == riItem.ItemIndex);
		return isDisplayArea;
	}

	/// <summary>
	/// 商品付帯情報変更ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbChangeProductOptionValue_Click(object sender, EventArgs e)
	{
		var linkButton = (LinkButton)sender;
		var commandArguments = linkButton.CommandArgument.Split(new char[] { ',' }, 2);
		this.ModifyProductIndex = int.Parse(commandArguments[0]);
		this.WhfProductOptionTexts.Value = commandArguments[1];

		// バリエーション情報初期化
		this.WhfOldVariationId.Value = string.Empty;
		this.WdvVariationErrorMessage.Visible = false;

		// バリエーション変更エリア表示のため商品エリア再描画
		if (GetParentRepeaterItem(linkButton, repeaterControlId: "rFixedPurchaseModifyProducts") == null)
		{
			var fpItemInputs = GetFixedPurchaseItemInputs(new FixedPurchaseInput(this.FixedPurchaseContainer));
			this.WrItem.DataSource = fpItemInputs;
			this.WrItem.DataBind();
			return;
		}

		this.WrFixedPurchaseModifyProducts.DataSource = this.InputProductList;
		this.WrFixedPurchaseModifyProducts.DataBind();
	}

	/// <summary>
	/// 商品付帯情報更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdateProductOptionValue_Click(object sender, EventArgs e)
	{
		// 商品情報取得
		var service = new FixedPurchaseService();
		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		var productId = ((LinkButton)sender).CommandArgument;
		var product = new ProductService().Get(this.ShopId, productId);
		var productOptionSettingList = new ProductOptionSettingList(product.ProductOptionSettings);
		if (GetProductOptionValueSetting(productOptionSettingList) == false) return;

		this.WhfProductOptionTexts.Value = ProductOptionSettingHelper.GetSelectedOptionSettingForFixedPurchaseItem(productOptionSettingList);

		var canUsePointsForNextPurchase = CanUsePointsForNextPurchase();
		if (canUsePointsForNextPurchase == false)
		{
			new FixedPurchaseService().ApplyNextShippingUsePointChange(
				Constants.CONST_DEFAULT_DEPT_ID,
				this.FixedPurchaseContainer,
				0,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);
		}

		var canUseCouponForNextPurchase = CanUseCouponForNextPurchase();
		if (canUseCouponForNextPurchase == false)
		{
			FixedPurchaseHelper.ChangeNextShippingUseCoupon(
				this.FixedPurchaseContainer,
				null,
				false,
				Constants.FLG_LASTCHANGED_USER);
		}

		// 更新対象の商品情報取得
		this.InputProductList = GetItems().ToList();
		input.Shippings[0].Items = this.InputProductList.ToArray();
		var model = input.CreateModel();

		this.WhfProductOptionTexts.Value = string.Empty;

		// 更新(履歴含む）
		service.UpdateItems(
			model.Shippings[0].Items,
			Constants.FLG_LASTCHANGED_USER,
			UpdateHistoryAction.Insert);

		// メール送信
		SendMailCommon.SendModifyFixedPurchaseMail(model.Shippings[0].FixedPurchaseId, SendMailCommon.FixedPurchaseModify.Product);

		// 再表示
		if (this.IsPc == false)
		{
			this.ScrollPositionIdAfterProductOptionUpdate = ((LinkButton)sender).Parent.Parent.FindControl("lbChangeProductOptionValue").ClientID;
		}
		Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(model.Shippings[0].FixedPurchaseId));
	}

	/// <summary>
	/// 付帯情報の取得
	/// </summary>
	/// <param name="productOptionSettingList">商品付帯情報一覧クラス</param>
	/// <returns>取得できたか</returns>
	private bool GetProductOptionValueSetting(ProductOptionSettingList productOptionSettingList)
	{
		var errorMessages = new StringBuilder();
		var modifyIndex = (this.ModifyProductIndex != null) ? this.ModifyProductIndex.Value : 0;
		var wrProductOptionValueSettings = GetWrappedControl<WrappedRepeater>(this.WrItem.Items[modifyIndex], "rProductOptionValueSettings");

		// 商品付帯情報取得
		foreach (RepeaterItem riProductOptionValueSetting in wrProductOptionValueSettings.Items)
		{
			var wrCblProductOptionValueSetting = GetWrappedControl<WrappedRepeater>(
				riProductOptionValueSetting,
				"rCblProductOptionValueSetting");
			var wddlProductOptionValueSetting = GetWrappedControl<WrappedDropDownList>(
				riProductOptionValueSetting,
				"ddlProductOptionValueSetting");
			var wtbProductOptionValueSetting = GetWrappedControl<WrappedTextBox>(
				riProductOptionValueSetting,
				"tbProductOptionValueSetting");
			var wlblProductOptionErrorMessage = GetWrappedControl<WrappedLabel>(
				riProductOptionValueSetting,
				"lblProductOptionErrorMessage");
			var wlblProductOptionCheckboxErrorMessage = GetWrappedControl<WrappedLabel>(
				riProductOptionValueSetting,
				"lblProductOptionCheckboxErrorMessage");
			var wlblProductOptionDropdownErrorMessage = GetWrappedControl<WrappedLabel>(
				riProductOptionValueSetting,
				"lblProductOptionDropdownErrorMessage");

			if (wrCblProductOptionValueSetting.Visible)
			{
				var sbSelectedValue = new StringBuilder();
				var checkBoxCount = 0;
				foreach (RepeaterItem riCheckBox in wrCblProductOptionValueSetting.Items)
				{
					var wcbProductOptionValueSetting = GetWrappedControl<WrappedCheckBox>(
						riCheckBox,
						"cbProductOptionValueSetting");
					var whfOriginalText = GetWrappedControl<WrappedHiddenField>(
						riCheckBox,
						"hfCbOriginalValue");
					if (sbSelectedValue.Length != 0)
					{
						sbSelectedValue.Append(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE);
					}

					if (Constants.PRODUCTBUNDLE_OPTION_ENABLED == false)
					{
						sbSelectedValue.Append(
							wcbProductOptionValueSetting.Checked ? wcbProductOptionValueSetting.Text : "");
					}
					else
					{
						sbSelectedValue.Append(
							wcbProductOptionValueSetting.Checked ? whfOriginalText.Value : "");
					}
					if (wcbProductOptionValueSetting.Checked) checkBoxCount++;
				}

				productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].SelectedSettingValue =
					sbSelectedValue.ToString();
				wlblProductOptionCheckboxErrorMessage.Text = (checkBoxCount == 0) && productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].IsNecessary
					? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR).Replace("@@ 1 @@", productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].ValueName)
					: string.Empty;
				errorMessages.Append(wlblProductOptionCheckboxErrorMessage.Text);
			}
			else if (wddlProductOptionValueSetting.Visible)
			{
				productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].SelectedSettingValue =
					wddlProductOptionValueSetting.SelectedValue;
				wlblProductOptionDropdownErrorMessage.Text = (wddlProductOptionValueSetting.SelectedIndex == 0) && productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].IsNecessary
					? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR).Replace("@@ 1 @@", productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].ValueName)
					: string.Empty;
				errorMessages.Append(wlblProductOptionDropdownErrorMessage.Text);
			}
			else if (wtbProductOptionValueSetting.Visible)
			{
				var pos = productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex];
				var checkKbn = "OptionValueValidate";

				// XML ドキュメントの検証を生成します。
				var validatorXml = pos.CreateValidatorXml(checkKbn);
				var param = new Hashtable
				{
					{ pos.ValueName, wtbProductOptionValueSetting.Text }
				};

				wlblProductOptionErrorMessage.Text = Validator.Validate(checkKbn, validatorXml.InnerXml, param);
				errorMessages.Append(wlblProductOptionErrorMessage.Text);

				// 設定値には全角スペースと全角：は入力させない
				if (wtbProductOptionValueSetting.Text.Contains('　') || wtbProductOptionValueSetting.Text.Contains('：'))
				{
					wlblProductOptionErrorMessage.Text += WebMessages
						.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_ERROR).Replace("@@ 1 @@", pos.ValueName);
					errorMessages.Append(
						WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_ERROR)
							.Replace("@@ 1 @@", pos.ValueName));
				}

				if ((errorMessages.Length == 0)
					&& (string.IsNullOrWhiteSpace(wtbProductOptionValueSetting.Text) == false))
				{
					productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].SelectedSettingValue =
						wtbProductOptionValueSetting.Text;
				}
			}
		}

		if (string.IsNullOrEmpty(errorMessages.ToString()) == false)
		{
			this.WdvProductOptionValueErrorMessage.Visible = true;
			this.WdvProductOptionValueErrorMessage.InnerHtml = errorMessages.ToString();
		}

		return string.IsNullOrEmpty(errorMessages.ToString());
	}

	/// <summary>
	/// 選択されている商品付帯情報値を取得する
	/// </summary>
	/// <param name="riItem">付帯情報選択用リピータ</param>
	/// <returns>選択されている付帯情報</returns>
	private string GetSelectedProductOptionValue(RepeaterItem riItem)
	{
		var rCblProductOptionValueSetting = (Repeater)riItem.FindControl("rCblProductOptionValueSetting");
		var ddlProductOptionValueSetting = (DropDownList)riItem.FindControl("ddlProductOptionValueSetting");
		var tbProductOptionValueSetting = (TextBox)riItem.FindControl("tbProductOptionValueSetting");
		if (rCblProductOptionValueSetting.Visible)
		{
			var lSelectedValues = new List<string>();
			foreach (RepeaterItem riCheckBox in rCblProductOptionValueSetting.Items)
			{
				var cbOption = ((CheckBox)(riCheckBox.FindControl("cbProductOptionValueSetting")));
				if (cbOption.Checked == false) continue;

				lSelectedValues.Add(cbOption.Text);
			}
			return string.Join(
				Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE,
				lSelectedValues.ToArray());
		}

		if (ddlProductOptionValueSetting.Visible)
		{
			return ddlProductOptionValueSetting.SelectedValue;
		}

		if (tbProductOptionValueSetting.Visible)
		{
			return tbProductOptionValueSetting.Text;
		}
		return null;
	}

	/// <summary>
	/// 選択情報を反映した商品付帯情報のリストを取得
	/// </summary>
	/// <param name="fpItemInput">定期購入商品情報</param>
	/// <returns>商品付帯情報一覧</returns>
	/// <remarks>手動マージを極力なくすためにデザイン側からの呼び出し用として利用する</remarks>
	protected ProductOptionSettingList GetProductOptionSettingList(FixedPurchaseItemInput fpItemInput)
	{
		return fpItemInput.ProductOptionSettingsWithSelectedValues;
	}

	/// <summary>
	/// 商品付帯情報更新キャンセル
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCloseUpdateProductOptionValueForm_Click(object sender, EventArgs e)
	{
		this.ModifyProductIndex = null;
		this.WdvProductOptionValueErrorMessage.Visible = false;
		this.WhfProductOptionTexts.Value = string.Empty;
		var fpItemInputs = GetFixedPurchaseItemInputs(new FixedPurchaseInput(this.FixedPurchaseContainer));

		this.WrItem.DataSource = fpItemInputs;
		this.WrItem.DataBind();
	}

	/// <summary>
	/// 商品リスト取得
	/// </summary>
	/// <param name="fpInput">定期購入情報入力クラス</param>
	/// <returns>商品リスト</returns>
	private FixedPurchaseItemInput[] GetFixedPurchaseItemInputs(FixedPurchaseInput fpInput)
	{
		var fpItemInputs = fpInput.Shippings[0].Items;
		return fpItemInputs;
	}

	/// <summary>
	/// 商品バリエーションリストアイテム作成
	/// </summary>
	/// <param name="productId">商品ID</param>
	/// <returns>商品バリエーションリストアイテム</returns>
	protected ListItemCollection GetVariationListItems(string productId)
	{
		var list = new ListItemCollection();

		// 商品バリエーション名取得
		var products = ProductCommon.GetProductInfoWithTranslatedName(this.ShopId, productId, this.MemberRankId, this.UserFixedPurchaseMemberFlg);
		foreach (DataRowView product in products)
		{
			var listItem = new ListItem(CreateVariationName(
				(string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
				(string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
				(string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3],
				string.Empty,
				string.Empty,
				Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION),
				(string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]);
				list.Add(listItem);
		}
		return list;
	}

	/// <summary>
	/// 商品情報取得
	/// </summary>
	/// <param name="fixedPurchaseItemInputs">定期商品入力情報</param>
	/// <returns>商品情報入力内容</returns>
	/// <remarks>引数指定で商品の変更内容を明示的に更新する プルダウンによる商品変更時の再計算時指定必須</remarks>
	private IEnumerable<FixedPurchaseItemInput> GetItems(List<FixedPurchaseItemInput> fixedPurchaseItemInputs = null)
	{
		// 商品情報をセット
		var inputs = new FixedPurchaseInput(this.FixedPurchaseContainer);

		if (fixedPurchaseItemInputs != null) inputs.Shippings[0].Items = fixedPurchaseItemInputs.ToArray();

		var index = 0;
		foreach (var input in inputs.Shippings[0].Items)
		{
			// 商品バリエーション変更した情報に更新
			if ((this.ModifyProductIndex == index)
				&& (input.VariationId == this.WhfOldVariationId.Value))
			{
				var product = ProductCommon.GetProductVariationInfo(
					input.ShopId,
					input.ProductId,
					this.WhfSelectedVariationId.Value,
					this.MemberRankId);
				input.VariationId = this.WhfSelectedVariationId.Value;
				input.Name = (string)product[0][Constants.FIELD_PRODUCT_NAME];
				input.VariationName1 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
				input.VariationName2 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
				input.VariationName3 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
				input.Price = (decimal)product[0][Constants.FIELD_PRODUCTVARIATION_PRICE];
				input.SpecialPrice =
					(product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] != DBNull.Value)
						? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE]
						: null;
				input.MemberRankPrice =
					(product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] != DBNull.Value)
						? (decimal?)product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION]
						: null;
				input.FixedPurchasePrice =
					(product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] != DBNull.Value)
						? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE]
						: null;
			}

			// 商品付帯情報を変更した値に更新
			if ((this.ModifyProductIndex == index)
				&& (string.IsNullOrEmpty(this.WhfProductOptionTexts.Value) == false))
			{
				input.ProductOptionTexts = this.WhfProductOptionTexts.Value;
			}

			index++;
			yield return input;
		}
	}

	/// <summary>
	/// バリエーション更新エリア表示
	/// </summary>
	private void VariationUpdateAreaDisplay()
	{
		var changeProduct = GetItems().First(item => (item.VariationId == this.WhfSelectedVariationId.Value));

		// 翻訳済みの商品情報取得
		var product = ProductCommon.GetProductInfoWithTranslatedName(
			this.ShopId,
			changeProduct.ProductId,
			this.MemberRankId,
			this.UserFixedPurchaseMemberFlg);

		// 更新の商品バリエーション名を変更
		var productVariation = product.Cast<DataRowView>().First(
			p => ((string)p[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] == changeProduct.VariationId));
		this.WhfVariationName.Value = CreateVariationName(
			(string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
			(string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
			(string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3],
			string.Empty,
			string.Empty,
			Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION);

		// 変更後金額の取得
		this.NewPrice = changeProduct.GetValidPrice();
		this.WhfNewSubtotal.Value = CurrencyManager.ToPrice(changeProduct.GetItemPrice());

		// バリエーション変更エリア表示のため商品エリア再描画
		this.WdvVariationErrorMessage.Visible = false;
		var fpItemInputs = GetFixedPurchaseItemInputs(new FixedPurchaseInput(this.FixedPurchaseContainer));
		this.WrItem.DataSource = fpItemInputs;
		this.WrItem.DataBind();
	}

	/// <summary>
	/// 注文拡張 変更ボタンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbOrderExtend_OnClick(object sender, EventArgs e)
	{
		this.IsOrderExtendModify = (this.IsOrderExtendModify == false);
		this.WlbOrderExtend.Visible = false;
		var orderExtend = OrderExtendCommon.ConvertOrderExtend(this.FixedPurchaseContainer);
		var input = new OrderExtendInput(OrderExtendInput.UseType.Modify, orderExtend);
		this.WrOrderExtendInput.DataSource = input.OrderExtendItems;
		this.WrOrderExtendInput.DataBind();
		this.Process.SetOrderExtendFromUserExtendObject(this.WrOrderExtendInput, input);
	}

	/// <summary>
	/// 注文拡張 更新ボタンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUpdateOrderExtend_OnClick(object sender, EventArgs e)
	{
		var inputDictionary = this.Process.CreateOrderExtendFromInputData(this.WrOrderExtendInput);
		var input = new OrderExtendInput(OrderExtendInput.UseType.Modify, inputDictionary);
		var errorMessage = input.Validate();
		if (errorMessage.Count == 0)
		{
			this.IsOrderExtendModify = (this.IsOrderExtendModify == false);
			this.WlbOrderExtend.Visible = true;

			var orderExtend = OrderExtendCommon.ConvertOrderExtend(this.FixedPurchaseContainer);

			foreach (var value in inputDictionary.Where(value => orderExtend.ContainsKey(value.Key)))
			{
				orderExtend[value.Key] = inputDictionary[value.Key];
			}

			var inputorderExtend = orderExtend.ToDictionary(v => v.Key, v => v.Value.Value);
			new FixedPurchaseService().UpdateOrderExtend(
				this.FixedPurchaseContainer.FixedPurchaseId,
				Constants.FLG_LASTCHANGED_USER,
				inputorderExtend,
				UpdateHistoryAction.Insert);

			this.WrOrderExtendDisplay.DataSource = input.OrderExtendItems;
			this.WrOrderExtendDisplay.DataBind();
		}
		else
		{
			this.Process.SetOrderExtendErrMessage(this.WrOrderExtendInput, errorMessage);
		}
	}

	/// <summary>
	/// 注文拡張 キャンセルボタンイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbHideOrderExtend_OnClick(object sender, EventArgs e)
	{
		this.IsOrderExtendModify = (this.IsOrderExtendModify == false);
		this.WlbOrderExtend.Visible = true;
	}

	/// <summary>
	/// Change product click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnChangeProduct_Click(object sender, EventArgs e)
	{
		if (Constants.SUBSCRIPTION_BOX_PRODUCT_CHANGE_METHOD.IsPullDown())
		{
			this.WbtnChangeProduct.Visible = false;
			this.WdvListProduct.Visible = false;
			this.WdvModifySubscription.Visible = true;
			return;
		}

		if (Constants.SUBSCRIPTION_BOX_PRODUCT_CHANGE_METHOD.IsModal())
		{
			this.MaintainScrollPositionOnPostBack = true;
			this.WdvModifySubscriptionBoxModalBg.Visible = true;

			if (sender == this.WbtnChangeProductWithClearingSelection.InnerControl)
			{
				this.SubscriptionBoxProductChangeModalUrl = CreateSubscriptionBoxProductChangeModalUrl(
					preserveSelections: false);
			}
			else if (sender == this.WbtnChangeProduct.InnerControl)
			{
				this.SubscriptionBoxProductChangeModalUrl = CreateSubscriptionBoxProductChangeModalUrl(
					preserveSelections: true);
			}
		}
	}

	/// <summary>
	/// モーダル閉じるボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void lCloseSubscriptionBoxProductChangeModal_OnClick(object sender, EventArgs e)
	{
		// モーダル内での変更を適用するため、ページまるごと再読み込みさせる
		Response.Redirect(this.Request.RawUrl);
	}

	/// <summary>
	/// お届け商品変更時にキャンセルボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCloseProduct_Click(object sender, EventArgs e)
	{
		this.WbtnChangeProduct.Visible = true;
		this.WdvListProduct.Visible = true;
		this.WdvModifySubscription.Visible = false;
	}

	/// <summary>
	/// Add product click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>btnCloseShippingPatternInfo_Click
	protected void btnAddProduct_Click(object sender, EventArgs e)
	{
		var items = GetItemsUpdate().ToList();
		var subscriptionBoxService = new SubscriptionBoxService();
		var data = subscriptionBoxService.GetSubscriptionBoxItemAvailable(this.FixedPurchaseContainer.SubscriptionBoxCourseId, StringUtility.ToDateString(this.FixedPurchaseContainer.NextShippingDate.Value, "MM-dd-yyyy"));
		// 必須商品が設定されていた場合任意商品用にデータを補正する
		if ((data != null) && this.SubscriptionBoxGetNextProductsResult.HasNecessaryFlg)
		{
			var optionalListItems = data
				.Where(selectableProduct => this.SubscriptionBoxGetNextProductsResult.NextProducts
					.Any(p => p.VariationId == selectableProduct.VariationId) == false)
				.ToArray();
			if (optionalListItems.Any()) data = optionalListItems;
		}
		if ((data != null) && data.Any())
		{
			var item = new FixedPurchaseItemInput()
			{
				FixedPurchaseId = this.FixedPurchaseContainer.FixedPurchaseId,
				FixedPurchaseShippingNo = this.FixedPurchaseContainer.Shippings[0].FixedPurchaseShippingNo.ToString(),
				ShopId = this.FixedPurchaseContainer.ShopId,
				ProductId = data[0].ProductId,
				VariationId = data[0].VariationId,
				ItemQuantity = "1",
				ItemQuantitySingle = "1",
				ProductOptionTexts = string.Empty,
				Price = 0
			};
			var product = ProductCommon.GetProductVariationInfo(this.FixedPurchaseContainer.ShopId, data[0].ProductId, data[0].VariationId, this.MemberRankId);
			if (product.Count != 0)
			{
				// 商品情報をセット
				item.SupplierId = (string)product[0][Constants.FIELD_PRODUCT_SUPPLIER_ID];
				item.Name = (string)product[0][Constants.FIELD_PRODUCT_NAME];
				item.VariationName1 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
				item.VariationName2 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
				item.VariationName3 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
				item.Price = (decimal)product[0][Constants.FIELD_PRODUCTVARIATION_PRICE];
				item.SpecialPrice = product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] : null;
				item.MemberRankPrice = product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] : null;
				item.FixedPurchasePrice = product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] : null;
				item.ShippingType = (string)product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE];
				item.FixedPurchaseFlg = (string)product[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG];
			}
			items.Add(item);
		}

		this.WrItemModify.DataSource = items;
		this.WrItemModify.DataBind();
	}

	/// <summary>
	/// Add product click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdateProduct_Click(object sender, EventArgs e)
	{
		if (this.WrItemModify.Items.Count == 0)
		{
			var wsErrorMessageShipping = GetWrappedControl<WrappedHtmlGenericControl>("sErrorQuantity");
			wsErrorMessageShipping.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SUBSCRIPTION_BOX_PRODUCT_DELETE_ALL);
			return;
		}
		Hashtable htTemp = new Hashtable();
		foreach (RepeaterItem roii in this.WrItemModify.Items)
		{
			var wddlProductName = GetWrappedControl<WrappedDropDownList>(roii, "ddlProductName");
			var productIdAndVaritionId = wddlProductName.SelectedValue.Split(',');
			string strKey = productIdAndVaritionId[0] + "***" + productIdAndVaritionId[1];
			if (htTemp.Contains(strKey))
			{
				var wsErrorMessageShipping = GetWrappedControl<WrappedHtmlGenericControl>("sErrorQuantity");
				wsErrorMessageShipping.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MANAGER_ORDERITEM_DUPLICATION_ERROR).Replace("@@ 1 @@", productIdAndVaritionId[0]).Replace("@@ 2 @@", productIdAndVaritionId[1]);
				return;
			}
			htTemp.Add(strKey, "");
		}

		decimal totalAmount = 0;
		var totalQuantity = 0;
		var isQuantityError = false;
		foreach (RepeaterItem rptItem in this.WrItemModify.Items)
		{
			var wddlQuantityUpdate = GetWrappedControl<WrappedDropDownList>(rptItem, "ddlQuantityUpdate");
			var whfProductPrice = GetWrappedControl<WrappedHiddenField>(rptItem, "hfProductPrice");
			var quantityString = wddlQuantityUpdate.SelectedValue;
			var quantity = 0;
			if (string.IsNullOrEmpty(quantityString) || (int.TryParse(quantityString, out quantity) == false) || (quantity < 1))
			{
				isQuantityError = true;
			}
			else
			{
				totalQuantity += quantity;
				totalAmount += decimal.Parse(whfProductPrice.Value) * quantity;
			}
		}
		if (isQuantityError)
		{
			var wsErrorMessageShipping = GetWrappedControl<WrappedHtmlGenericControl>("sErrorQuantity");
			wsErrorMessageShipping.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_QUANTITY_UPDATE_ALERT);
			return;
		}

		var wsErrorMessage = OrderCommon.CheckLimitProductOrderForSubscriptionBox(
			this.FixedPurchaseContainer.SubscriptionBoxCourseId,
			totalQuantity);
		wsErrorMessage += OrderCommon.GetSubscriptionBoxProductOfNumberError(
			this.FixedPurchaseContainer.SubscriptionBoxCourseId,
			this.WrItemModify.Items.Count,
			true);
		wsErrorMessage += OrderCommon.GetSubscriptionBoxTotalAmountError(
			this.FixedPurchaseContainer.SubscriptionBoxCourseId,
			totalAmount);
		if (string.IsNullOrEmpty(wsErrorMessage) == false)
		{
			GetWrappedControl<WrappedHtmlGenericControl>("sErrorQuantity").InnerHtml = HtmlSanitizer.HtmlEncodeChangeToBr(wsErrorMessage);
			return;
		}

		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		input.Shippings[0].Items = GetItemsUpdate().ToArray();

		var index = 0;
		foreach (var fixedPurchaseItemInput in input.Shippings[0].Items)
		{
			var beforeCount = (this.InputProductList.Count > index)
				? this.InputProductList[index].ItemQuantity
				: "1";
			var wddlQuantityUpdate = GetWrappedControl<WrappedDropDownList>(this.WrItemModify.Items[index], "ddlQuantityUpdate");

			// 商品の購入限界数チェック
			var productVariation = ProductCommon.GetProductVariationInfo(
				this.FixedPurchaseContainer.ShopId,
				fixedPurchaseItemInput.ProductId,
				fixedPurchaseItemInput.VariationId,
				this.MemberRankId);
			var maxSellQuantity = (int)productVariation[0][Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY] + 1;
			var productName = CreateProductJointName(
				(string)productVariation[0][Constants.FIELD_PRODUCT_NAME],
				(string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
				(string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
				(string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]);
			if (this.CanSalable((int)productVariation[0][Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY], fixedPurchaseItemInput) == false)
			{
				var wsErrorMessageShipping = GetWrappedControl<WrappedHtmlGenericControl>("sErrorQuantity");
				wsErrorMessageShipping.InnerText = WebMessages
					.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_MAX_SELL_QUANTITY_LIMIT_ERROR)
					.Replace("@@ 1 @@", productName)
					.Replace("@@ 2 @@", maxSellQuantity.ToString());
				fixedPurchaseItemInput.ItemQuantity = beforeCount;
				wddlQuantityUpdate.SelectedValue = beforeCount;
				SetPlannedTotalAmountForTheNextOrderModify(input.Shippings[0].Items.ToList());
				return;
			}

			// 決済上限、下限のチェック
			SetPlannedTotalAmountForTheNextOrderModify(input.Shippings[0].Items.ToList());
			var paymentErrorMessage = IsPaymentPriceInRange(
				this.PlannedTotalAmountForNextOrder,
				input.ShopId,
				input.OrderPaymentKbn);
			if (string.IsNullOrEmpty(paymentErrorMessage) == false)
			{
				var wsErrorMessageShipping = GetWrappedControl<WrappedHtmlGenericControl>("sErrorQuantity");
				wsErrorMessageShipping.InnerText = paymentErrorMessage;
				fixedPurchaseItemInput.ItemQuantity = beforeCount;
				wddlQuantityUpdate.SelectedValue = beforeCount;
				SetPlannedTotalAmountForTheNextOrderModify(input.Shippings[0].Items.ToList());
				return;
			}

			index++;
		}

		var model = input.CreateModel();

		new FixedPurchaseService()
				.UpdateItems(
					model.Shippings[0].Items,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);
		new UpdateHistoryService().InsertForFixedPurchase(model.FixedPurchaseId, Constants.FLG_LASTCHANGED_USER);
		new UpdateHistoryService().InsertForUser(model.UserId, Constants.FLG_LASTCHANGED_USER);
		Response.Redirect(Request.Url.AbsolutePath + "?" + Constants.REQUEST_KEY_FIXED_PURCHASE_ID + "=" + Request[Constants.REQUEST_KEY_FIXED_PURCHASE_ID]);
	}


	/// <summary>
	/// 商品変更
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rProductChange_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		var items = GetItemsUpdate().ToList();
		items.RemoveAt(e.Item.ItemIndex);
		this.WrItemModify.DataSource = items;
		this.WrItemModify.DataBind();
	}

	/// <summary>
	/// 頒布会商品リストの作成
	/// </summary>
	/// <returns>商品リスト</returns>
	protected ListItemCollection GetSubscriptionBoxProductList(string productId, string varitionId)
	{
		var subscriptionBoxService = new SubscriptionBoxService();
		var productList = subscriptionBoxService.GetSubscriptionBoxItemList(
			this.FixedPurchaseContainer.ShopId,
			this.FixedPurchaseContainer.SubscriptionBoxCourseId,
			this.FixedPurchaseContainer.NextShippingDate.Value);

		if (productList.Any() == false) return new ListItemCollection();

		// 必須商品が設定されていた場合任意商品用のドロップダウンも作る
		if (this.SubscriptionBoxGetNextProductsResult.HasNecessaryFlg
			&& (this.SubscriptionBoxGetNextProductsResult.NextProducts.Any(product => product.VariationId == varitionId) == false))
		{
			var optionalListItems = productList
				.Where(selectableProduct => this.SubscriptionBoxGetNextProductsResult.NextProducts
					.Any(p => p.VariationId == selectableProduct.VariationId) == false)
				.ToArray();
			if (optionalListItems.Any()) productList = optionalListItems;
		}

		this.SubscriputionBoxProductListItem = new ListItem[productList.Length];
		var productListCount = 0;
		foreach (var productItem in productList)
		{
			var item = new FixedPurchaseItemInput
			{
				Name = productItem.Name,
				VariationName1 = productItem.VariationName1,
				VariationName2 = productItem.VariationName2,
				VariationName3 = productItem.VariationName3
			};
			var productName = item.CreateProductJointName();

			this.SubscriputionBoxProductListItem[productListCount] = new ListItem(
				productName,
				productItem.ProductId + ',' + productItem.VariationId);
			productListCount++;
		}
		return SortsubscriputionBoxProductList(productId, varitionId);
	}

	/// <summary>
	/// 頒布会商品リストの先頭に選択されているのもを先頭に移動
	/// </summary>
	/// <returns>商品リスト</returns>
	protected ListItemCollection SortsubscriputionBoxProductList(string productId, string varitionId)
	{
		var productList = new ListItemCollection();
		productList.AddRange(this.SubscriputionBoxProductListItem);
		var propertyNumber = productList.FindByValue(productId + "," + varitionId);
		if (propertyNumber == null) return productList;
		var indexItem = productList.IndexOf(propertyNumber);
		productList.RemoveAt(indexItem);
		productList.Insert(0, propertyNumber);
		return productList;
	}

	/// <summary>
	/// 頒布会コース設定の変更可能な商品候補があるかどうかを確かめる
	/// </summary>
	/// <returns>頒布会コース設定の変更可能な商品候補があるかどうか</returns>
	protected bool HasAnySubscriptionBoxItemAvailableToSwapWith()
	{
		var sbItems = new SubscriptionBoxService().GetSubscriptionBoxItemAvailable(
			this.FixedPurchaseContainer.SubscriptionBoxCourseId,
			StringUtility.ToDateString(this.FixedPurchaseContainer.NextShippingDate.Value, "MM-dd-yyyy"));
		return sbItems.Any();
	}

	/// <summary>
	/// Linkbutton search address from shipping zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrFromShippingZipGlobal_Click(object sender, EventArgs e)
	{
		if (IsNotCountryJp(StringUtility.ToEmpty(this.ShippingAddrCountryIsoCode)) == false) return;

		BindingAddressByGlobalZipcode(
			this.ShippingAddrCountryIsoCode,
			StringUtility.ToHankaku(this.WtbShippingZipGlobal.Text.Trim()),
			this.WtbShippingAddr2,
			this.WtbShippingAddr4,
			this.WtbShippingAddr5,
			this.WddlShippingAddr2,
			this.WddlShippingAddr3,
			this.WddlShippingAddr5);
	}

	/// <summary>
	/// Action from boku pay
	/// </summary>
	private void ActionFromBokuPay()
	{
		if (Request["action"] != "boku") return;

		var paymentId = StringUtility.ToEmpty(Session["boku_payment_id"]);
		WrappedRadioButtonGroup wrbgPayment;
		this.WdvOrderPaymentPattern.Visible = true;

		foreach (RepeaterItem wrPaymentItem in this.WrPayment.Items)
		{
			var wrbgTmpPayment = GetWrappedControl<WrappedRadioButtonGroup>(wrPaymentItem, "rbgPayment");
			var whfPaymentId = GetWrappedControl<WrappedHiddenField>(wrPaymentItem, "hfPaymentId", string.Empty);

			if (whfPaymentId.Value == paymentId)
			{
				wrbgPayment = wrbgTmpPayment;
				wrbgTmpPayment.Checked = true;
				rbgPayment_OnCheckedChanged(wrbgPayment, EventArgs.Empty);
			}
			else
			{
				wrbgTmpPayment.Checked = false;
			}
		}

		if (string.IsNullOrEmpty(paymentId) == false)
		{
			var sender = GetWrappedControl<WrappedLinkButton>("lbUpdatePayment");
			btnUpdatePaymentPatternInfo_Click(sender, EventArgs.Empty);
		}
	}

	/// <summary>
	/// ReCalculation
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ReCalculation(object sender, EventArgs e)
	{
		var isSuccess = true;
		var parentRepeater = GetParentRepeaterItem((Control)sender, "rItemModify");
		var wddlQuantityUpdate = GetWrappedControl<WrappedDropDownList>(parentRepeater, "ddlQuantityUpdate");
		var quantityString = wddlQuantityUpdate.SelectedValue;
		var beforeCount = (this.InputProductList.Count > parentRepeater.ItemIndex)
			? this.InputProductList[parentRepeater.ItemIndex].ItemQuantity
			: "1";
		var wsErrorQuantity = GetWrappedControl<WrappedHtmlGenericControl>("sErrorQuantity");
		wsErrorQuantity.InnerText = "";
		if (IsNumberValidationError(quantityString))
		{
			wsErrorQuantity.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_QUANTITY_UPDATE_ALERT);
			isSuccess = false;
		}

		var fixedPurchaseItemInput = GetItemsUpdate().ToList();
		this.InputProductList = GetItems(fixedPurchaseItemInput).ToList();
		var productId = this.InputProductList[parentRepeater.ItemIndex].ProductId;
		var variationId = this.InputProductList[parentRepeater.ItemIndex].VariationId;
		var productVariation = ProductCommon.GetProductVariationInfo(
			this.FixedPurchaseContainer.ShopId,
			productId,
			variationId,
			this.MemberRankId);
		var maxSellQuantity = (int)productVariation[0][Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY] + 1;
		if (isSuccess)
		{
			var targetItem = fixedPurchaseItemInput.FirstOrDefault(item => item.VariationId == variationId);
			var productName = CreateProductJointName(
				(string)productVariation[0][Constants.FIELD_PRODUCT_NAME],
				(string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
				(string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
				(string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]);
			if ((targetItem != null) && (this.CanSalable((int)productVariation[0][Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY], targetItem) == false))
			{
				wsErrorQuantity.InnerText = WebMessages
					.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_MAX_SELL_QUANTITY_LIMIT_ERROR)
					.Replace("@@ 1 @@", productName)
					.Replace("@@ 2 @@", maxSellQuantity.ToString());
				isSuccess = false;
			}
		}

		if (isSuccess)
		{
			// 決済上限、下限のチェック
			var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
			SetPlannedTotalAmountForTheNextOrderModify(fixedPurchaseItemInput);
			var paymentErrorMessage = IsPaymentPriceInRange(
				this.PlannedTotalAmountForNextOrder,
				input.ShopId,
				input.OrderPaymentKbn);
			if (string.IsNullOrEmpty(paymentErrorMessage) == false)
			{
				wsErrorQuantity.InnerText = paymentErrorMessage;
				isSuccess = false;
			}
		}
		
		if(isSuccess == false)
		{
			var targetItem = fixedPurchaseItemInput.FirstOrDefault(item => item.VariationId == variationId);
			if (targetItem != null)
			{
				targetItem.ItemQuantity = beforeCount;
				targetItem.ItemQuantitySingle = beforeCount;
			}
			else
			{
				fixedPurchaseItemInput = this.InputProductList;
				var beforeTargetItem = fixedPurchaseItemInput.FirstOrDefault(item => item.VariationId == variationId);
				beforeTargetItem.ItemQuantity = beforeCount;
				beforeTargetItem.ItemQuantitySingle = beforeCount;
			}

			SetPlannedTotalAmountForTheNextOrderModify(fixedPurchaseItemInput);
		}

		this.WrItemModify.DataSource = fixedPurchaseItemInput;
		this.WrItemModify.DataBind();
	}

	/// <summary>
	/// 商品情報取得
	/// </summary>
	/// <param name="productVariation">商品バリエーション情報</param>
	/// <returns>商品情報入力内容</returns>
	/// <remarks>パフォーマンス考慮してproductVariation引数がnullではないときは引数の情報を優先利用する</remarks>
	private IEnumerable<FixedPurchaseItemInput> GetItemsUpdate(DataView productVariation = null)
	{
		var itemNo = 1;
		foreach (RepeaterItem ri in this.WrItemModify.Items)
		{
			var wddlProductName = GetWrappedControl<WrappedDropDownList>(ri, "ddlProductName");
			var wddlQuantityUpdate = GetWrappedControl<WrappedDropDownList>(ri, "ddlQuantityUpdate");

			// 入力情報をセット
			var productIdAndVaritionId = wddlProductName.SelectedValue.Split(',');
			var item = new FixedPurchaseItemInput()
			{
				FixedPurchaseId = this.FixedPurchaseContainer.FixedPurchaseId,
				FixedPurchaseItemNo = itemNo.ToString(),
				FixedPurchaseShippingNo = this.FixedPurchaseContainer.Shippings[0].FixedPurchaseShippingNo.ToString(),
				ShopId = this.FixedPurchaseContainer.ShopId,
				ProductId = productIdAndVaritionId[0],
				VariationId = productIdAndVaritionId[1],
				ItemQuantity = wddlQuantityUpdate.SelectedValue,
				ItemQuantitySingle = wddlQuantityUpdate.SelectedValue,
				ProductOptionTexts = string.Empty,
				Price = 0
			};

			// 商品情報が存在する?
			var product = (productVariation != null)
				? productVariation
				: ProductCommon.GetProductVariationInfo(
					item.ShopId,
					item.ProductId,
					item.VariationId,
					this.MemberRankId);
			if (product.Count != 0)
			{
				// 商品情報をセット
				item.SupplierId = (string)product[0][Constants.FIELD_PRODUCT_SUPPLIER_ID];
				item.Name = (string)product[0][Constants.FIELD_PRODUCT_NAME];
				item.VariationName1 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
				item.VariationName2 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
				item.VariationName3 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
				item.Price = (decimal)product[0][Constants.FIELD_PRODUCTVARIATION_PRICE];
				item.SpecialPrice = product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] : null;
				item.MemberRankPrice = product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] : null;
				item.FixedPurchasePrice = product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] : null;
				item.ShippingType = (string)product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE];
				item.FixedPurchaseFlg = (string)product[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG];
			}
			itemNo++;

			yield return item;
		}
	}

	/// <summary>
	/// 次回商品更新orキャンセル
	/// </summary>
	/// <param name="updateHistoryAction">履歴更新アクション</param>
	public void UpdateNextProductOrCancel(UpdateHistoryAction updateHistoryAction)
	{
		var getNextProductsResult = new SubscriptionBoxService().GetFixedPurchaseNextProduct(
			this.FixedPurchaseContainer.SubscriptionBoxCourseId,
			this.FixedPurchaseContainer.FixedPurchaseId,
			this.FixedPurchaseContainer.MemberRankId,
			this.FixedPurchaseContainer.NextNextShippingDate.Value,
			this.FixedPurchaseContainer.SubscriptionBoxOrderCount + 1,
			this.FixedPurchaseContainer.Shippings[0]);
		var fpService = new FixedPurchaseService();
		fpService.UpdateNextDeliveryForSubscriptionBox(
			this.FixedPurchaseContainer.FixedPurchaseId,
			Constants.FLG_LASTCHANGED_BATCH,
			Constants.W2MP_ACCESSLOG_ENABLED,
			getNextProductsResult,
			updateHistoryAction);

		switch (getNextProductsResult.Result)
		{
			// 商品切り替え失敗時
			case SubscriptionBoxGetNextProductsResult.ResultTypes.Fail:
				Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
				break;

			// キャンセル時
			case SubscriptionBoxGetNextProductsResult.ResultTypes.Cancel:
				// 最新のユーザポイントを取得
				this.LoginUserPoint = PointOptionUtility.GetUserPoint((string)this.LoginUserId);
				// メール送信
				SendMailCommon.SendModifyFixedPurchaseMail(this.FixedPurchaseContainer.FixedPurchaseId, SendMailCommon.FixedPurchaseModify.OrderCancell);
				// 解約完了画面へ
				Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId)); 
				break;
		}

		if (updateHistoryAction == UpdateHistoryAction.DoNotInsert) return;
		var uhService = new UpdateHistoryService();
		uhService.InsertForFixedPurchase(this.FixedPurchaseContainer.FixedPurchaseId, Constants.FLG_LASTCHANGED_USER);
		uhService.InsertForUser(this.FixedPurchaseContainer.UserId, Constants.FLG_LASTCHANGED_USER);
	}

	/// <summary>
	/// 再開できる頒布会コースかどうか
	/// </summary>
	/// <returns>再開できるかどうか</returns>
	protected bool CanResumeCourse(FixedPurchaseContainer container)
	{
		// 定期購入ステータスが完了のものの時のみ判定
		if (container.IsCompleteStatus == false) return false;

		// 回数指定の場合
		var subscriptionBox = new SubscriptionBoxService().GetByCourseId(container.SubscriptionBoxCourseId);
		if (subscriptionBox.IsNumberTime) return false;

		// 期間指定の場合
		var result = subscriptionBox.DefaultOrderProducts.Any(dp => (dp.TermUntil > DateTime.Now) && (dp.TermSince <= DateTime.Now));
		return result;
	}

	/// <summary>
	/// 頒布会必須商品チェック
	/// </summary>
	/// <param name="variationId">バリエーションID</param>
	/// <returns>結果</returns>
	protected bool CanModify(string variationId)
	{
		if (string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId)) return false;
		if (this.SubscriptionBoxGetNextProductsResult.HasNecessaryFlg == false) return true;
		var result = (this.SubscriptionBoxGetNextProductsResult.NextProducts.Any(p => p.VariationId == variationId) == false);
		return result;
	}

	/// <summary>
	/// 頒布会商品追加チェック
	/// </summary>
	/// <returns>結果</returns>
	protected bool CanAddProduct()
	{
		if (this.SubscriptionBoxGetNextProductsResult.HasNecessaryFlg == false) return true;

		var subscriptionBoxItems = new SubscriptionBoxService().GetSubscriptionBoxItemAvailable(
			this.FixedPurchaseContainer.SubscriptionBoxCourseId,
			StringUtility.ToDateString(this.FixedPurchaseContainer.NextShippingDate.Value, "MM-dd-yyyy"));
		var optionalListItems = subscriptionBoxItems.Where(
			selectableProduct =>
				this.SubscriptionBoxGetNextProductsResult.NextProducts.Any(p => p.VariationId == selectableProduct.VariationId) == false)
			.ToArray();
		var result = optionalListItems.Any();
		return result;
	}

	/// <summary>
	/// 頒布会必須商品削除不可メッセージ
	/// </summary>
	/// <returns>メッセージ</returns>
	protected string GetSubscriptionBoxNecessaryMessage()
	{
		var subscriptionName = new SubscriptionBoxService().GetDisplayName(this.FixedPurchaseContainer.SubscriptionBoxCourseId);
		return subscriptionName + "の必須商品のため削除できません。";
	}

	/// <summary>
	/// 頒布会コース設定を満たしていないドロップダウンの項目の無効化
	/// </summary>
	protected void SetSubscriptionBoxItemToDisabled()
	{
		var amountDictionary = new Dictionary<string, decimal>();
		foreach (RepeaterItem rItemModifyItem in this.WrItemModify.Items)
		{
			var wddlProductName = GetWrappedControl<WrappedDropDownList>(rItemModifyItem, "ddlProductName");
			var whfOrderSubtotal = GetWrappedControl<WrappedHiddenField>(rItemModifyItem, "hfOrderSubtotal");
			if (amountDictionary.Keys.FirstOrDefault(x => x == wddlProductName.SelectedValue) == null)
			{
				amountDictionary.Add(wddlProductName.SelectedValue, decimal.Parse(whfOrderSubtotal.Value));
			}
		}

		var subscriptionBoxService = new SubscriptionBoxService();
		var subscriptionBox = subscriptionBoxService.GetByCourseId(this.FixedPurchaseContainer.SubscriptionBoxCourseId);

		foreach (RepeaterItem rItemModifyItem in this.WrItemModify.Items)
		{
			var wddlProductName = GetWrappedControl<WrappedDropDownList>(rItemModifyItem, "ddlProductName");
			var whFixedPurchaseItemNo = GetWrappedControl<WrappedHiddenField>(rItemModifyItem, "hfFixedPurchaseItemNo");

			var amountSum = 0m;
			if (string.IsNullOrEmpty(whFixedPurchaseItemNo.Value))
			{
				amountSum += amountDictionary.Sum(x => x.Value);
			}
			else
			{
				amountSum += amountDictionary
					.Where(x => (x.Key != wddlProductName.SelectedValue))
					.Sum(x => x.Value);
			}

			foreach (var product in subscriptionBox.SelectableProducts)
			{
				if (wddlProductName.Items.FindByValue(product.ProductId + "," + product.VariationId) == null) continue;

				var subscriptionBoxProduct = subscriptionBox.SelectableProducts.FirstOrDefault(
					x => x.ProductId == product.ProductId && x.VariationId == product.VariationId);

				var productPrice = OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxProduct)
					? subscriptionBoxProduct.CampaignPrice
					: product.Price;

				if ((amountSum + productPrice) > subscriptionBox.MaximumAmount)
				{
					wddlProductName.Items.FindByValue(product.ProductId + "," + product.VariationId).Attributes["Disabled"] = "true";
				}
			}
		}
	}

	/// <summary>
	/// 頒布会商品追加エリアリピータイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rItemModify_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var sProductPriceControl = (HtmlGenericControl)e.Item.FindControl("sProductPrice");
		if (sProductPriceControl == null) return;

		var sOrderSubtotalControl = (HtmlGenericControl)e.Item.FindControl("sOrderSubtotal");
		var ddlQuantityUpdateControl = (DropDownList)e.Item.FindControl("ddlQuantityUpdate");
		var hfProductPriceControl = (HiddenField)e.Item.FindControl("hfProductPrice");
		var hfProductIdControl = (HiddenField)e.Item.FindControl("hfProductId");
		var hfVariationIdControl = (HiddenField)e.Item.FindControl("hfVariationId");
		var hfOrderSubtotalControl = (HiddenField)e.Item.FindControl("hfOrderSubtotal");

		if (string.IsNullOrEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId)) return;

		var selectedSubscriptionBox = DataCacheControllerFacade
			.GetSubscriptionBoxCacheController()
			.Get(this.FixedPurchaseContainer.SubscriptionBoxCourseId);

		var subscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
			x => (x.ProductId == hfProductIdControl.Value)
				&& (x.VariationId == hfVariationIdControl.Value));

		// 頒布会キャンペーン期間の場合キャンペーン期間価格を適用
		if (OrderCommon.IsSubscriptionBoxCampaignPeriodByNextShippingDate(subscriptionBoxItem, (DateTime)this.FixedPurchaseContainer.NextShippingDate))
		{
			sProductPriceControl.InnerText = CurrencyManager.ToPrice(subscriptionBoxItem.CampaignPrice.ToPriceDecimal());
			hfProductPriceControl.Value = HtmlSanitizer.HtmlEncode(subscriptionBoxItem.CampaignPrice.ToPriceString());
			if (string.IsNullOrEmpty(ddlQuantityUpdateControl.SelectedValue) == false)
			{
				sOrderSubtotalControl.InnerText = CurrencyManager.ToPrice(
					subscriptionBoxItem.CampaignPrice.ToPriceDecimal()
					* decimal.Parse(ddlQuantityUpdateControl.SelectedValue));

				hfOrderSubtotalControl.Value = HtmlSanitizer.HtmlEncode(
					subscriptionBoxItem.CampaignPrice.ToPriceDecimal()
					* decimal.Parse(ddlQuantityUpdateControl.SelectedValue));
			}
		}

		CreateItemQuantityDropdownList(sender, e);
	}
	
	/// <summary>
	/// 頒布会商品変更モーダルのURLを作成
	/// </summary>
	/// <returns>モーダルURL</returns>
	private string CreateSubscriptionBoxProductChangeModalUrl(bool preserveSelections)
	{
		var creator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_SUBSCRIPTIONBOX_PRODUCT_CHANGE_MODAL)
			.AddParam(
				Constants.REQUEST_KEY_FIXEDPURCHASE_FIXED_PURCHASE_ID,
				this.FixedPurchaseContainer.FixedPurchaseId);

		if (preserveSelections)
		{
			creator.AddParam(Constants.REQUEST_KEY_SUBSCRIPTION_BOX_PRESERVE_SELECTION, "1");
		}

		var result = creator.CreateUrl();
		return result;
	}

	/// <summary>
	/// Can use point for purchase
	/// </summary>
	/// <returns>true: success, false: failure</returns>
	public bool CanUsePointForPurchase()
	{
		if (Constants.W2MP_POINT_OPTION_ENABLED == false) return false;

		var fixedPurchaseItems = GetFixedPurchaseItemInputs(new FixedPurchaseInput(this.FixedPurchaseContainer));
		var subtotal = fixedPurchaseItems.Sum(item => item.ItemPriceIncludedOptionPrice);
		var result = (subtotal >= Constants.POINT_MINIMUM_PURCHASEPRICE);
		return result;
	}

	/// <summary>
	/// 次回購入時にクーポン利用できるか
	/// </summary>
	/// <returns>true: できる, false: できない</returns>
	public bool CanUseCouponForNextPurchase()
	{
		if (Constants.W2MP_COUPON_OPTION_ENABLED == false) return false;
		if (this.FixedPurchaseContainer.NextShippingUseCouponDetail == null) return true;

		var subtotal = GetItems().Sum(item => item.ItemPriceIncludedOptionPrice);
		var result = (subtotal >= this.FixedPurchaseContainer.NextShippingUseCouponDetail.UsablePrice);
		return result;
	}

	/// <summary>
	/// 次回購入時にポイント利用できるか
	/// </summary>
	/// <returns>true: できる, false: できない</returns>
	public bool CanUsePointsForNextPurchase()
	{
		if (Constants.W2MP_POINT_OPTION_ENABLED == false) return false;

		var subtotal = GetItems().Sum(item => item.ItemPriceIncludedOptionPrice);
		var result = (subtotal >= Constants.POINT_MINIMUM_PURCHASEPRICE);
		return result;
	}

	/// <summary>
	/// メール便配送サービス絞り込み
	/// </summary>
	/// <returns>配送会社リスト</returns>
	protected DeliveryCompanyModel[] GetCompanyListMail()
	{
		var itemQuantity = 0;
		var totalProductSize = OrderCommon.GetTotalProductSizeFactor(
			this.FixedPurchaseContainer.Shippings[0].Items.Select(
				item => new KeyValuePair<string, Tuple<int, string, string>>(
					item.ProductId,
					new Tuple<int, string, string>(
						(int.TryParse(item.ItemQuantity.ToString(), out itemQuantity)
							? itemQuantity
							: 0),
						item.ShopId,
						item.VariationId))));
		var companyList = DataCacheControllerFacade.GetShopShippingCacheController()
			.Get(this.FixedPurchaseContainer.ShippingType).CompanyListMail
			.Select(company => new DeliveryCompanyService().Get(company.DeliveryCompanyId))
			.Where(c => c.DeliveryCompanyMailSizeLimit > (totalProductSize - 1))
			.OrderBy(c => c.DeliveryCompanyMailSizeLimit).ToArray();
		return companyList;
	}

	/// <summary>
	/// 定期台帳に紐づく受注情報を取得
	/// </summary>
	private void GetOrderByFixedPurchaseId()
	{
		var iCurrentPageNumber = 1; // カレントページ番号
		if (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]) != "") iCurrentPageNumber = int.Parse(Request[Constants.REQUEST_KEY_PAGE_NO].ToString());

		var strMaxCount = StringUtility.ToHankaku(WhfPagerMaxDispCount.Value);
		var selfPagerMaxDispCount = 0;
		var isPagerMaxDispCountInt = int.TryParse(strMaxCount, out selfPagerMaxDispCount);

		WdvOrderHistoryList.Visible = (selfPagerMaxDispCount <= 0) && isPagerMaxDispCountInt;

		WdvOrderHistoryList.Visible = (string.IsNullOrEmpty(WhfPagerMaxDispCount.Value) == false) && (selfPagerMaxDispCount > 0);

		if (WdvOrderHistoryList.Visible == false) return;

		var bgnColumnNum = selfPagerMaxDispCount * (iCurrentPageNumber - 1) + 1;
		var endColumnNum = selfPagerMaxDispCount * iCurrentPageNumber;

		var orderHistoryList = new OrderService().GetOrderHistoryListByFixedPurchaseId(
		this.LoginUserId,
		bgnColumnNum,
		endColumnNum,
		this.RequestFixedPurchaseId);
		this.WrOrderList.DataSource = orderHistoryList;

		btnCancelFixedPurchase.Visible = this.CanDisplayCancelFixedPurchaseButton;
		btnCancelFixedPurchaseReason.Visible = this.CanDisplayCancelFixedPurchaseButton;
		btnSuspendFixedPurchase.Visible = this.CanDisplaySuspendFixedPurchaseButton;

		//アラートの表示をするか
		if (orderHistoryList.Cast<DataRowView>().Any(drv => Constants.AlertOrderStatusForCancelFixedPurchase_ENABLED.Contains(drv[Constants.FIELD_ORDER_ORDER_STATUS])))
		{
			btnCancelFixedPurchase.OnClientClick =
				"return confirm('"
				+ WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AlertOrderStatusForCancelFixedPurchase).ReplaceCrLf("\\n").Trim()
				+ "')";
			btnCancelFixedPurchaseReason.OnClientClick =
				"return confirm('"
				+ WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AlertOrderStatusForCancelFixedPurchase).ReplaceCrLf("\\n").Trim()
				+ "');";
			btnSuspendFixedPurchase.OnClientClick =
				"return confirm('"
				+ WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_MYPAGE_FIXEDPURCHASE_PAUSE_ALERT).ReplaceCrLf("\\n").Trim()
				+ "');";
		}

		// 子注文の総件数取得
		var totalCount = orderHistoryList.Count != 0 ? int.Parse(orderHistoryList[0].Row["row_count"].ToString()) : 0;

		// 子注文情報の総件数が0件の場合
		if (totalCount == 0)
		{
			WdvOrderHistoryList.Visible = false;
			return;
		}

		// ページャ作成
		var nextUrl = PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId);
		this.PagerHtml = WebPager.CreateProductListPager(totalCount, this.RequestPageNum, nextUrl, selfPagerMaxDispCount);

		// 定期購入情報一覧表セット
		this.WrOrderList.DataBind();
	}

	/// <summary>
	/// 次回注文時のお支払い予定金額をセット
	/// </summary>
	private void SetPlannedTotalAmountForTheNextOrder()
	{
		this.NextShippingFixedPurchaseCart = CreateSimpleCartListForFixedPurchase(this.FixedPurchaseContainer.FixedPurchaseId).Items[0];
		this.IsFreeShippingByCoupon = this.NextShippingFixedPurchaseCart.IsFreeShippingFlgCouponUse();

		this.PlannedTotalAmountForNextOrder = this.NextShippingFixedPurchaseCart.PriceSubtotal
			- this.NextShippingFixedPurchaseCart.UseCouponPriceForProduct
			- ((this.CanUseAllPointFlg && this.FixedPurchaseContainer.IsUseAllPointFlg)
				? 0
				: this.NextShippingFixedPurchaseCart.UsePointPrice)
			- this.NextShippingFixedPurchaseCart.FixedPurchaseDiscount;
	}

	/// <summary>
	/// 配送希望時間帯変更フォーム表示(AmazonPayCv2)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbModifyShippingTimeAmazonPayCv2_Click(object sender, EventArgs e)
	{
		this.IsShippingTimeModifyAmazonPayCv2 = (this.IsShippingTimeModifyAmazonPayCv2 == false);
		if (this.IsShippingTimeModifyAmazonPayCv2 == false) return;
		foreach (ListItem li in this.WddlShippingTimeAmazonPayCv2.Items)
		{
			if (this.FixedPurchaseContainer.Shippings[0].ShippingTime == li.Value)
			{
				li.Selected = true;
			}
		}
	}

	/// <summary>
	/// 商品数量変更ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnModifyProducts_OnClick(object sender, EventArgs e)
	{
		this.InputProductList = GetItems().ToList();
		
		if (Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED)
		{
			SetFixedPurchaseProductChange();
		}

		this.WrFixedPurchaseModifyProducts.DataSource = this.InputProductList;
		this.WrFixedPurchaseModifyProducts.DataBind();

		this.IsProductModify = true;
		this.WdvListProduct.Visible = false;
		this.WbtnModifyProducts.Visible = false;
		this.WdvModifyFixedPurchase.Visible = true;
	}

	/// <summary>
	/// 商品編集ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnModifyConfirm_OnClick(object sender, EventArgs e)
	{
		this.WsProductModifyErrorMessage.InnerText = string.Empty;

		var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
		input.Shippings[0].Items = GetModifyProducts(notDeleteProduct: false).ToArray();

		var deliveryCompanyNew = GetChangeMailDeliveryCompany();
		if (deliveryCompanyNew != input.Shippings[0].DeliveryCompanyId)
		{
			if (this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
			{
				var deliveryCompanyName = new DeliveryCompanyService().Get(this.FixedPurchaseContainer.Shippings[0].DeliveryCompanyId).DeliveryCompanyName;
				this.WsProductModifyErrorMessage.InnerText += CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ORDER_MODIFY_ESCALATION)
					.Replace("@@ 1 @@", deliveryCompanyName);
				return;
			}
			input.Shippings[0].ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
		}

		// 商品付帯情報必須チェック
		foreach (var fixedPurchaseItemInput in input.Shippings[0].Items)
		{
			var productOptionSettings = GetProductOptionSettingList(fixedPurchaseItemInput);
			if ((productOptionSettings != null) && (productOptionSettings.Items.Count > 0))
			{
				foreach (var productOptionSetting in productOptionSettings.Items)
				{
					if (productOptionSetting.IsNecessary && string.IsNullOrEmpty(productOptionSetting.SelectedSettingValue))
					{
						var valueName = productOptionSetting.ValueName;
						this.WsProductModifyErrorMessage.InnerText += string.Format("商品ID：{0} {1}",
							fixedPurchaseItemInput.ProductId,
							WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR).Replace("@@ 1 @@", valueName));
						return;
					}
				}
			}
		}

		// 定期購入可能商品か
		foreach (var fixedPurchaseItemInput in input.Shippings[0].Items)
		{
			if (fixedPurchaseItemInput.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
			{
				this.WsProductModifyErrorMessage.InnerText += WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_FIXED_PURCHASE_INVALID).Replace("@@ 1 @@", fixedPurchaseItemInput.ProductId);
				return;
			}
		}

		foreach (var fixedPurchaseItemInput in input.Shippings[0].Items)
		{
			// 商品の有効チェック
			var product = ProductCommon.GetProductInfoUnuseMemberRankPrice(fixedPurchaseItemInput.ShopId, fixedPurchaseItemInput.ProductId);
			if ((product == null)
				|| product.Count < 1)
			{
				this.WsProductModifyErrorMessage.InnerText += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_DELETE).Replace("@@ 1 @@", fixedPurchaseItemInput.ProductId);
				return;
			}
			if ((string)product[0][Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
			{
				this.WsProductModifyErrorMessage.InnerText += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_INVALID).Replace("@@ 1 @@", fixedPurchaseItemInput.ProductId);
				return;
			}

			// 商品の購入限界数チェック
			var productVariation = ProductCommon.GetProductVariationInfo(
				this.FixedPurchaseContainer.ShopId,
				fixedPurchaseItemInput.ProductId,
				fixedPurchaseItemInput.VariationId,
				this.MemberRankId);
			var maxSellQuantity = (int)productVariation[0][Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY] + 1;
			var productName = CreateProductJointName(
				(string)productVariation[0][Constants.FIELD_PRODUCT_NAME],
				(string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
				(string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
				(string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]);
			if (this.CanSalable((int)productVariation[0][Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY], fixedPurchaseItemInput) == false)
			{
				this.WsProductModifyErrorMessage.InnerText = WebMessages
					.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_MAX_SELL_QUANTITY_LIMIT_ERROR)
					.Replace("@@ 1 @@", productName)
					.Replace("@@ 2 @@", maxSellQuantity.ToString());
				return;
			}
		}

		// 決済上限、下限のチェック
		SetPlannedTotalAmountForTheNextOrderModify(input.Shippings[0].Items.ToList());
		var paymentErrorMessage = IsPaymentPriceInRange(
			this.PlannedTotalAmountForNextOrder,
			input.ShopId,
			input.OrderPaymentKbn);
		if (string.IsNullOrEmpty(paymentErrorMessage) == false)
		{
			this.WsProductModifyErrorMessage.InnerText = paymentErrorMessage;
			return;
		}

		var model = input.CreateModel();

		new FixedPurchaseService()
			.UpdateItems(
				model.Shippings[0].Items,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert);
		new FixedPurchaseService()
			.UpdateShipping(
				model.Shippings[0],
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert);
		new UpdateHistoryService().InsertForFixedPurchase(model.FixedPurchaseId, Constants.FLG_LASTCHANGED_USER);
		new UpdateHistoryService().InsertForUser(model.UserId, Constants.FLG_LASTCHANGED_USER);

		// メール送信
		SendMailCommon.SendModifyFixedPurchaseMail(model.FixedPurchaseId, SendMailCommon.FixedPurchaseModify.Product);
		this.InputProductList = null;

		Response.Redirect(Request.Url.AbsolutePath + "?" + Constants.REQUEST_KEY_FIXED_PURCHASE_ID + "=" + Request[Constants.REQUEST_KEY_FIXED_PURCHASE_ID]);
	}

	/// <summary>
	/// 商品情報取得
	/// </summary>
	/// <returns>商品情報入力内容</returns>
	private IEnumerable<FixedPurchaseItemInput> GetModifyProducts(bool notDeleteProduct)
	{
		var itemNo = 1;
		foreach (RepeaterItem ri in this.WrFixedPurchaseModifyProducts.Items)
		{
			var deleteProduct = GetWrappedControl<WrappedCheckBox>(ri, "cbDeleteProduct");
			if (deleteProduct.HasInnerControl && deleteProduct.Checked && (notDeleteProduct == false))
			{
				continue;
			}

			var whfProductId = GetWrappedControl<WrappedHiddenField>(ri, "hfProductId");
			var whfVariationId = GetWrappedControl<WrappedHiddenField>(ri, "hfVariationId");
			var variationId = string.IsNullOrEmpty(whfVariationId.Value) ? whfProductId.Value : whfVariationId.Value;
			var wddlQuantityUpdate = GetWrappedControl<WrappedDropDownList>(ri, "ddlQuantityUpdate");
			var whfProductOptionTexts = GetWrappedControl<WrappedHiddenField>(ri, "hfProductOptionTexts");

			// 入力情報をセット
			var item = new FixedPurchaseItemInput()
			{
				FixedPurchaseId = this.FixedPurchaseContainer.FixedPurchaseId,
				FixedPurchaseItemNo = itemNo.ToString(),
				FixedPurchaseShippingNo = this.FixedPurchaseContainer.Shippings[0].FixedPurchaseShippingNo.ToString(),
				ShopId = this.FixedPurchaseContainer.ShopId,
				ProductId = whfProductId.Value,
				VariationId = variationId,
				ItemQuantity = wddlQuantityUpdate.SelectedValue,
				ItemQuantitySingle = wddlQuantityUpdate.SelectedValue,
				ProductOptionTexts = string.Empty,
				Price = 0,
				ModifyDeleteTarget = deleteProduct.HasInnerControl && deleteProduct.Checked
			};

			// 商品情報が存在する?
			var product = ProductCommon.GetProductVariationInfo(item.ShopId, item.ProductId, item.VariationId, this.MemberRankId);
			if (product.Count != 0)
			{
				// 商品情報をセット
				item.SupplierId = (string)product[0][Constants.FIELD_PRODUCT_SUPPLIER_ID];
				item.Name = (string)product[0][Constants.FIELD_PRODUCT_NAME];
				item.VariationName1 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1];
				item.VariationName2 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2];
				item.VariationName3 = (string)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3];
				item.Price = (decimal)product[0][Constants.FIELD_PRODUCTVARIATION_PRICE];
				item.SpecialPrice = product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE] : null;
				item.MemberRankPrice = product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION] : null;
				item.FixedPurchasePrice = product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] != DBNull.Value ? (decimal?)product[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE] : null;
				item.ShippingType = (string)product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE];
				item.FixedPurchaseFlg = (string)product[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG];
				item.ProductOptionTexts = whfProductOptionTexts.Value;
			}

			// 定期変更設定
			if (Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED)
			{
				item.ProductChangeSetting = new FixedPurchaseProductChangeSettingService().GetByProductId(
				item.ProductId,
				item.VariationId != item.ProductId
					? item.VariationId
					: string.Empty,
				this.ShopId);
			}

			foreach (var fixedPurchaseItemContainer in this.FixedPurchaseContainer.Shippings[0].Items)
			{
				var itemInput = new FixedPurchaseItemInput(fixedPurchaseItemContainer);
				if (itemInput.IsSameProduct(item))
				{
					item.ItemOrderCount = itemInput.ItemOrderCount;
					item.ItemShippedCount = itemInput.ItemShippedCount;
				}
			}

			itemNo++;

			yield return item;
		}
	}

	/// <summary>
	/// キャンセルボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnModifyCancel_OnClick(object sender, EventArgs e)
	{
		this.IsProductModify = false;
		this.WbtnModifyProducts.Visible = true;
		this.WdvModifyFixedPurchase.Visible = false;
		this.InputProductList = null;

		// 再表示
		Response.Redirect(PageUrlCreatorUtility.CreateFixedPurchaseDetailUrl(this.FixedPurchaseContainer.FixedPurchaseId));
	}

	/// <summary>
	/// 商品追加ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnModifyAddProduct_OnClick(object sender, EventArgs e)
	{
		// 定期購入ID
		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_FIXED_PURCHASE_ID] = this.FixedPurchaseContainer.FixedPurchaseId;
		// 配送種別
		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_SHIPPING_ID] = this.FixedPurchaseContainer.ShippingType;
		// 決済種別
		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PAYMENT_ID] = this.FixedPurchaseContainer.OrderPaymentKbn;

		// 投入済みの商品ID
		var productIds = this.InputProductList
			.Where(item => item.HasVariation == false)
			.Select(prd => prd.ProductId)
			.ToList();

		// 入力用商品リストのバリエーションIDの件数をカウントする
		var inputProductVariationCounts = this.InputProductList
			.GroupBy(item => item.ProductId)
			.ToDictionary(g => g.Key, g => g.Select(item => item.VariationId).Distinct().Count());

		// 商品IDのリストを作成
		var productIdList = inputProductVariationCounts.Keys.ToList();

		// IN句を使用して一度に商品IDごとのバリエーション件数を取得
		var productVariationCounts = ProductService.GetProductVariationCounts(this.ShopId, productIdList);
		
		foreach (var productVariation in productVariationCounts)
		{
			var productId = productVariation.Key;
			var count = productVariation.Value;

			if (inputProductVariationCounts.ContainsKey(productId) == false) continue;
			
			var inputProductVariationCount = (int)inputProductVariationCounts[productId];

			if (count != inputProductVariationCount) continue;
			// 件数が等しい場合、投入済みの商品IDに追加
			productIds.Add(productId);
		}

		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_IDS] = productIds.ToArray();

		// 投入済みのバリエーションID
		var variationIds = this.InputProductList
			.Where(item => item.HasVariation)
			.Select(val => val.VariationId)
			.ToList();
		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_VARIATION_IDS] = variationIds.ToArray();

		SessionManager.IsRedirectFromFixedPurchaseDetail = true;

		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_FIXED_PURCHASE_PRODUCT_ADD);
	}

	/// <summary>
	/// 商品変更ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnFixedPurchaseProductChange_OnClick(object sender, EventArgs e)
	{
		var productId = ((Button)sender).CommandArgument;
		var input = this.InputProductList.FirstOrDefault(inputProduct => inputProduct.ProductId == productId);
		if (input == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(
				WebMessages.ERRMSG_FRONT_FIXED_PURCHASE_ITEM_ALREADY_CHANGED).Replace("@@ 1 @@", productId);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_BEFORE_PRODUCT_ID] = input.ProductId;
		Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_SETTING_ID] = input.ProductChangeSetting.FixedPurchaseProductChangeId;
		btnModifyAddProduct_OnClick(sender, e);
	}

	/// <summary>
	/// 選択商品投入
	/// </summary>
	private void AddSelectProduct()
	{
		if ((Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_FIXED_PURCHASE_ID] == null)
			|| ((string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_FIXED_PURCHASE_ID] != this.FixedPurchaseContainer.FixedPurchaseId)) return;

		var products = ProductCommon.GetProductVariationInfo(
			this.ShopId,
			(string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_ID],
			(string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_VARIATION_ID],
			this.MemberRankId);

		if (products.Count > 0)
		{
			var addProduct = new FixedPurchaseItemInput(this.FixedPurchaseContainer, products[0]);
			addProduct.ProductOptionTexts =
				(string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_PRODUCT_OPTION];

			// 翻訳済みの商品情報取得
			var product = ProductCommon.GetProductInfoWithTranslatedName(
				this.ShopId,
				addProduct.ProductId,
				this.MemberRankId,
				this.UserFixedPurchaseMemberFlg);

			// 更新の商品バリエーション名を変更
			var productVariation = product.Cast<DataRowView>().First(
				p => ((string)p[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] == addProduct.VariationId));
			this.WhfVariationName.Value = CreateVariationName(
				(string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
				(string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
				(string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3],
				string.Empty,
				string.Empty,
				Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION);

			this.WdvVariationErrorMessage.Visible = false;

			if (this.InputProductList.Any(p => p.IsSameProduct(addProduct)) == false)
			{
				this.InputProductList.Add(addProduct);
			}
		}

		// 定期商品変更設定をセット
		if (Constants.FIXEDPURCHASE_PRODUCTCHANGE_ENABLED)
		{
			SetFixedPurchaseProductChange();
		}

		SetPlannedTotalAmountForTheNextOrderModify();
		this.WrFixedPurchaseModifyProducts.DataSource = this.InputProductList;
		this.WrFixedPurchaseModifyProducts.DataBind();
		this.IsProductModify = true;
		this.WbtnModifyProducts.Visible = false;
		this.WdvListProduct.Visible = false;
		this.WdvModifyFixedPurchase.Visible = true;
	}

	/// <summary>
	/// 選択商品に変更
	/// </summary>
	private void ChangeSelectProduct()
	{
		if ((Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_FIXED_PURCHASE_ID] == null)
			|| ((string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_FIXED_PURCHASE_ID] != this.FixedPurchaseContainer.FixedPurchaseId)
			|| (Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_BEFORE_PRODUCT_ID] == null)) return;

		var products = ProductCommon.GetProductVariationInfo(
			this.ShopId,
			(string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_AFTER_PRODUCT_ID],
			(string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_VARIATION_ID],
			this.MemberRankId);

		if (products.Count > 0)
		{
			var changeProduct = new FixedPurchaseItemInput(this.FixedPurchaseContainer, products[0]);
			changeProduct.ProductOptionTexts =
				(string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_ADD_PRODUCT_PRODUCT_OPTION];

			// 翻訳済みの商品情報取得
			var product = ProductCommon.GetProductInfoWithTranslatedName(
				this.ShopId,
				changeProduct.ProductId,
				this.MemberRankId,
				this.UserFixedPurchaseMemberFlg);

			// 更新の商品バリエーション名を変更
			var productVariation = product.Cast<DataRowView>().First(
				p => ((string)p[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID] == changeProduct.VariationId));
			this.WhfVariationName.Value = CreateVariationName(
				(string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
				(string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
				(string)productVariation[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3],
				string.Empty,
				string.Empty,
				Constants.CONST_PRODUCTVARIATIONNAME_PUNCTUATION);

			this.WdvVariationErrorMessage.Visible = false;

			// 投入済み商品リストの更新
			for (var i = 0; i < this.InputProductList.Count; i++)
			{
				if (this.InputProductList[i].ProductId == (string)Session[Constants.SESSION_KEY_FIXED_PURCHASE_PRODUCT_CHANGE_BEFORE_PRODUCT_ID])
				{
					this.InputProductList[i] = changeProduct;
				}
			}
		}

		SetFixedPurchaseProductChange();
		SetPlannedTotalAmountForTheNextOrderModify();
		this.WrFixedPurchaseModifyProducts.DataSource = this.InputProductList;
		this.WrFixedPurchaseModifyProducts.DataBind();
		this.IsProductModify = true;
		this.WbtnModifyProducts.Visible = false;
		this.WdvListProduct.Visible = false;
		this.WdvModifyFixedPurchase.Visible = true;
	}

	/// <summary>
	/// 再計算
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ProductModifyReCalculation(object sender, EventArgs e)
	{
		this.WsProductModifyErrorMessage.InnerText = "";
		var parentRepeater = GetParentRepeaterItem((Control)sender, "rFixedPurchaseModifyProducts");

		var wddlQuantityUpdate = GetWrappedControl<WrappedDropDownList>(parentRepeater, "ddlQuantityUpdate");
		var quantityString = wddlQuantityUpdate.SelectedValue;
		var beforeCount = this.InputProductList[parentRepeater.ItemIndex].ItemQuantity;
		var isSuccess = true;

		if (IsNumberValidationError(quantityString))
		{
			this.WsProductModifyErrorMessage.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_QUANTITY_UPDATE_ALERT);
			isSuccess = false;
		}

		var productList = GetModifyProducts(notDeleteProduct: false).ToList();
		if (isSuccess)
		{
			if (productList.Any() == false)
			{
				this.WsProductModifyErrorMessage.InnerText += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_MODIFY_NOT_PRODUCT);
				isSuccess = false;
			}
		}

		if (isSuccess)
		{
			var productId = this.InputProductList[parentRepeater.ItemIndex].ProductId;
			var variationId = this.InputProductList[parentRepeater.ItemIndex].VariationId;
			var productVariation = ProductCommon.GetProductVariationInfo(
				this.FixedPurchaseContainer.ShopId,
				productId,
				variationId,
				this.MemberRankId);
			var targetItem = productList.FirstOrDefault(item => item.VariationId == variationId);
			var limitErrorMessage = CanSalableMaxSellQuantityLimit(productVariation, targetItem);
			if (string.IsNullOrEmpty(limitErrorMessage) == false)
			{
				this.WsProductModifyErrorMessage.InnerText = limitErrorMessage;
				isSuccess = false;
			}
		}

		if (isSuccess)
		{
			var deliveryCompanyNew = GetChangeMailDeliveryCompany();
			if (deliveryCompanyNew != this.FixedPurchaseContainer.Shippings[0].DeliveryCompanyId)
			{
				if (this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
				{
					var deliveryCompanyName = new DeliveryCompanyService().Get(this.FixedPurchaseContainer.Shippings[0].DeliveryCompanyId).DeliveryCompanyName;
					this.WsProductModifyErrorMessage.InnerText += WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_MODIFY_ESCALATION)
						.Replace("@@ 1 @@", deliveryCompanyName);
					isSuccess = false;
				}
			}
		}

		if (isSuccess)
		{
			this.InputProductList[parentRepeater.ItemIndex].ItemQuantity = quantityString;
			wddlQuantityUpdate.SelectedValue = quantityString;
			SetPlannedTotalAmountForTheNextOrderModify(productList);
		}

		if (isSuccess)
		{
			// 決済上限、下限のチェック
			var input = new FixedPurchaseInput(this.FixedPurchaseContainer);
			var paymentErrorMessage = IsPaymentPriceInRange(
				this.PlannedTotalAmountForNextOrder,
				input.ShopId,
				input.OrderPaymentKbn);
			if (string.IsNullOrEmpty(paymentErrorMessage) == false)
			{
				this.WsProductModifyErrorMessage.InnerText = paymentErrorMessage;
				isSuccess = false;
			}
		}

		if (isSuccess == false)
		{
			var variationId = this.InputProductList[parentRepeater.ItemIndex].VariationId;
			var targetItem = productList.FirstOrDefault(item => item.VariationId == variationId);

			if (targetItem != null)
			{
				targetItem.ItemQuantity = beforeCount;
				targetItem.ItemQuantitySingle = beforeCount;
				SetPlannedTotalAmountForTheNextOrderModify(productList);
			}
			else
			{
				productList = this.InputProductList;
				var beforeTargetItem = productList.FirstOrDefault(item => item.VariationId == variationId);
				beforeTargetItem.ItemQuantity = beforeCount;
				beforeTargetItem.ItemQuantitySingle = beforeCount;
				SetPlannedTotalAmountForTheNextOrderModify(productList);
			}
		}

		this.WrFixedPurchaseModifyProducts.DataSource = productList;
		this.WrFixedPurchaseModifyProducts.DataBind();
	}

	/// <summary>
	/// 次回注文時のお支払い予定金額をセット
	/// </summary>
	/// <param name="productList">定期購入商品情報</param>
	private void SetPlannedTotalAmountForTheNextOrderModify(List<FixedPurchaseItemInput> productList = null)
	{
		this.NextShippingFixedPurchaseCart = CreateSimpleCartListForFixedPurchaseModify(this.FixedPurchaseContainer.FixedPurchaseId, productList).Items[0];
		this.IsFreeShippingByCoupon = this.NextShippingFixedPurchaseCart.IsFreeShippingFlgCouponUse();

		this.PlannedTotalAmountForNextOrder = this.NextShippingFixedPurchaseCart.PriceSubtotal
			- this.NextShippingFixedPurchaseCart.UseCouponPriceForProduct
			- ((this.CanUseAllPointFlg && this.FixedPurchaseContainer.IsUseAllPointFlg)
				? 0
				: this.NextShippingFixedPurchaseCart.UsePointPrice)
			- this.NextShippingFixedPurchaseCart.FixedPurchaseDiscount;

		WlNextProductSubTotal.Text = CurrencyManager.ToPrice(this.NextShippingFixedPurchaseCart.PriceSubtotal);
		WlNextCouponName.Text = GetCouponName(this.NextShippingFixedPurchaseCart);
		WlNextOrderCouponUse.Text = ((this.NextShippingFixedPurchaseCart.UseCouponPriceForProduct > 0) ? "-" : "") + CurrencyManager.ToPrice(this.NextShippingFixedPurchaseCart.UseCouponPriceForProduct);
		WlNextUsePointPrice.Text = ((this.NextShippingFixedPurchaseCart.UsePointPrice > 0) ? "-" : "") + CurrencyManager.ToPrice(this.NextShippingFixedPurchaseCart.UsePointPrice);
		WlNextFixedPurchaseDiscountPrice.Text = ((this.NextShippingFixedPurchaseCart.FixedPurchaseDiscount > 0) ? "-" : "") + CurrencyManager.ToPrice(this.NextShippingFixedPurchaseCart.FixedPurchaseDiscount);
		WlNextOrderTotal.Text = CurrencyManager.ToPrice(this.PlannedTotalAmountForNextOrder);
	}

	/// <summary>
	/// カート情報リストの作成
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <param name="productList">定期購入商品情報</param>
	/// <returns>カート情報リスト</returns>
	protected CartObjectList CreateSimpleCartListForFixedPurchaseModify(string fixedPurchaseId, List<FixedPurchaseItemInput> productList = null)
	{
		var fixedPurchase = new FixedPurchaseService().Get(fixedPurchaseId);
		var user = new UserService().Get(this.LoginUserId);
		if (user == null) return null;

		fixedPurchase.Shippings[0].Items = productList == null
			? this.InputProductList.Select(p => p.CreateModel()).ToArray()
			: productList.Select(p => p.CreateModel()).ToArray();

		var orderRegister = new OrderRegisterFixedPurchaseInner(user.IsMember, Constants.FLG_LASTCHANGED_USER, canSendFixedPurchaseMailToUser: false, fixedPurchaseId: fixedPurchaseId, fixedPurchaseMailSendTiming: null);
		var cartObject = new OrderRegisterFixedPurchase(Constants.FLG_LASTCHANGED_USER, canSendFixedPurchaseMailToUser: false, canUpdateShippingDate: false, fixedPurchaseMailSendTiming: null)
			.CreateCartList(fixedPurchase, user, orderRegister);
		return cartObject;
	}

	/// <summary>
	/// 商品付帯情報変更ボタンの表示判定
	/// </summary>
	/// <param name="fpItemInput">定期購入商品情報</param>
	/// <returns>商品付帯情報変更ボタンを表示するか否か</returns>
	protected bool IsDisplayInputProductOptionChangeButton(FixedPurchaseItemInput fpItemInput)
	{
		var isDisplayButton = (GetProductOptionSettingList(fpItemInput).Items.Any());
		return isDisplayButton;
	}

	/// <summary>
	/// 商品付帯情報入力エリアの表示判定
	/// </summary>
	/// <param name="riItem">定期購入商品リピーターアイテム</param>
	/// <returns>商品付帯情報入力エリアを表示するか否か</returns>
	protected bool IsDisplayInputProductOptionValueArea(RepeaterItem riItem)
	{
		var isDispArea = this.ModifyProductIndex == riItem.ItemIndex;
		return isDispArea;
	}

	/// <summary>
	/// 商品付帯情報変更ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbChangeProductOptionInputItem_Click(object sender, EventArgs e)
	{
		var linkButton = (LinkButton)sender;
		var repeater = GetParentRepeaterItem(linkButton, repeaterControlId: "rFixedPurchaseModifyProducts");
		var wdvProductOptionValue =
			GetWrappedControl<WrappedHtmlGenericControl>(repeater, controlId: "dvProductOptionValue");
		wdvProductOptionValue.Visible = true;

		this.ModifyProductIndex = repeater.ItemIndex;
		this.WrFixedPurchaseModifyProducts.DataSource = this.InputProductList;
		this.WrFixedPurchaseModifyProducts.DataBind();
	}

	/// <summary>
	/// 省略した商品付帯情報表示文字列を取得
	/// </summary>
	/// <param name="originalText">オリジナルテキスト</param>
	/// <param name="maxDisplayLength">最大表示文字数</param>
	/// <returns>オプション価格の省略テキスト</returns>
	/// <remarks>
	/// 例）最大表示文字数が13の場合<br/>
	/// <br/>
	/// ■付帯価格オプション有効時<br/>
	/// 商品付帯情報の表示文字列が"最大表示文字数"を超える場合に、
	/// 価格部分を差し引いた部分を "..." で省略して返す（"最大表示文字数"になるように）<br/>
	///  → 表示例：大盛りべんどう【松竹梅セット】(+2,120円)　→　大...(+2,120円)<br/>
	/// <br/>
	/// ■付帯価格オプション無効時 or オプション価格ではない商品付帯情報<br/>
	/// 商品付帯情報の表示文字列が"最大表示文字数"を超える場合に、
	/// 超えた部分を "..." で省略して返す（"最大表示文字数"になるように）<br/>
	///  → 表示例：大盛りべんどう【松竹梅セット】　→　大盛りべんどう【松竹...
	/// </remarks>
	public string GetAbbreviatedProductOptionTextForDisplay(string originalText, int maxDisplayLength = 13)
	{
		if ((Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false)
			|| (originalText.Contains("(") == false)) return StringUtility.AbbreviateString(originalText, maxDisplayLength);

		if (originalText.Length <= maxDisplayLength) return originalText;

		// オリジナルテキストの文字数がmaxDisplayLengthを超えた際に文字を省略する
		// NOTE: 価格部分以外を省略する（価格部分の省略はNG）
		var priceText = Regex.Match(originalText, @"\(\+\S+\)").Groups[0].Value;
		var priceRemovedText = originalText.Replace(priceText, string.Empty);
		var displayLength = maxDisplayLength - priceText.Length - 3;
		var abbreviatedText = StringUtility.AbbreviateString(priceRemovedText, displayLength <= 0 ? 1 : displayLength);
		return abbreviatedText + priceText;
	}

	/// <summary>
	/// 商品付帯情報更新
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbInputProductOptionValueUpdate_Click(object sender, EventArgs e)
	{
		// 商品情報取得
		var productId = ((LinkButton)sender).CommandArgument;
		var product = new ProductService().Get(this.ShopId, productId);
		var productOptionSettingList = new ProductOptionSettingList(product.ProductOptionSettings);
		var parentRepeaterItem = GetParentRepeaterItem((LinkButton)sender, "rFixedPurchaseModifyProducts");
		if (GetInputProductOptionValueSetting(productOptionSettingList, parentRepeaterItem) == false) return;

		var modifyIndex = this.ModifyProductIndex ?? 0;
		this.InputProductList[modifyIndex].ProductOptionTexts = ProductOptionSettingHelper.GetSelectedOptionSettingForFixedPurchaseItem(productOptionSettingList);
		this.ModifyProductIndex = null;
		this.WrItem.DataSource = this.WrFixedPurchaseModifyProducts.DataSource = this.InputProductList;
		this.WrFixedPurchaseModifyProducts.DataBind();
		this.WrItem.DataBind();

		// 次回注文時のお支払い予定金額を反映する
		SetPlannedTotalAmountForTheNextOrderModify(this.InputProductList);
	}

	/// <summary>
	/// 付帯情報の取得
	/// </summary>
	/// <param name="productOptionSettingList">商品付帯情報一覧クラス</param>
	/// <param name="parentRepeaterItem">親リピーターアイテム</param>
	/// <returns>取得できたか</returns>
	private bool GetInputProductOptionValueSetting(ProductOptionSettingList productOptionSettingList, RepeaterItem parentRepeaterItem)
	{
		var errorMessages = new StringBuilder();
		var wrProductOptionValueSettings = GetWrappedControl<WrappedRepeater>(parentRepeaterItem, "rProductOptionValueSettings");

		// 商品付帯情報取得
		foreach (RepeaterItem riProductOptionValueSetting in wrProductOptionValueSettings.Items)
		{
			var wrCblProductOptionValueSetting = GetWrappedControl<WrappedRepeater>(
				riProductOptionValueSetting,
				"rCblProductOptionValueSetting");
			var wddlProductOptionValueSetting = GetWrappedControl<WrappedDropDownList>(
				riProductOptionValueSetting,
				"ddlProductOptionValueSetting");
			var wtbProductOptionValueSetting = GetWrappedControl<WrappedTextBox>(
				riProductOptionValueSetting,
				"tbProductOptionValueSetting");
			var wlblProductOptionErrorMessage = GetWrappedControl<WrappedLabel>(
				riProductOptionValueSetting,
				"lblProductOptionErrorMessage");
			var wlblProductOptionCheckboxErrorMessage = GetWrappedControl<WrappedLabel>(
				riProductOptionValueSetting,
				"lblProductOptionCheckboxErrorMessage");
			var wlblProductOptionDropdownErrorMessage = GetWrappedControl<WrappedLabel>(
				riProductOptionValueSetting,
				"lblProductOptionDropdownErrorMessage");

			if (wrCblProductOptionValueSetting.Visible)
			{
				var sbSelectedValue = new StringBuilder();
				var checkBoxCount = 0;
				foreach (RepeaterItem riCheckBox in wrCblProductOptionValueSetting.Items)
				{
					var wcbProductOptionValueSetting = GetWrappedControl<WrappedCheckBox>(
						riCheckBox,
						"cbProductOptionValueSetting");
					var whfOriginalText = GetWrappedControl<WrappedHiddenField>(
						riCheckBox,
						"hfCbOriginalValue");
					if (sbSelectedValue.Length != 0)
					{
						sbSelectedValue.Append(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE);
					}

					if (Constants.PRODUCTBUNDLE_OPTION_ENABLED == false)
					{
						sbSelectedValue.Append(
							wcbProductOptionValueSetting.Checked ? wcbProductOptionValueSetting.Text : "");
					}
					else
					{
						sbSelectedValue.Append(
							wcbProductOptionValueSetting.Checked ? whfOriginalText.Value : "");
					}
					if (wcbProductOptionValueSetting.Checked) checkBoxCount++;
				}

				productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].SelectedSettingValue =
					sbSelectedValue.ToString();
				wlblProductOptionCheckboxErrorMessage.Text = (checkBoxCount == 0) && productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].IsNecessary
					? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR).Replace("@@ 1 @@", productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].ValueName)
					: string.Empty;
				errorMessages.Append(wlblProductOptionCheckboxErrorMessage.Text);
			}
			else if (wddlProductOptionValueSetting.Visible)
			{
				productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].SelectedSettingValue =
					wddlProductOptionValueSetting.SelectedValue;
				wlblProductOptionDropdownErrorMessage.Text = (wddlProductOptionValueSetting.SelectedIndex == 0) && productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].IsNecessary
				? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR).Replace("@@ 1 @@", productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].ValueName)
				: string.Empty;
				errorMessages.Append(wlblProductOptionDropdownErrorMessage.Text);
			}
			else if (wtbProductOptionValueSetting.Visible)
			{
				var pos = productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex];
				var checkKbn = "OptionValueValidate";

				// XML ドキュメントの検証を生成します。
				var validatorXml = pos.CreateValidatorXml(checkKbn);
				var param = new Hashtable
				{
					{ pos.ValueName, wtbProductOptionValueSetting.Text }
				};

				wlblProductOptionErrorMessage.Text = Validator.Validate(checkKbn, validatorXml.InnerXml, param);
				errorMessages.Append(wlblProductOptionErrorMessage.Text);

				// 設定値には全角スペースと全角：は入力させない
				if (wtbProductOptionValueSetting.Text.Contains('　') || wtbProductOptionValueSetting.Text.Contains('：'))
				{
					wlblProductOptionErrorMessage.Text += WebMessages
						.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_ERROR).Replace("@@ 1 @@", pos.ValueName);
					errorMessages.Append(
						WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_OPTION_ERROR)
							.Replace("@@ 1 @@", pos.ValueName));
				}

				if ((errorMessages.Length == 0)
					&& (string.IsNullOrWhiteSpace(wtbProductOptionValueSetting.Text) == false))
				{
					productOptionSettingList.Items[riProductOptionValueSetting.ItemIndex].SelectedSettingValue =
						wtbProductOptionValueSetting.Text;
				}
			}
		}

		if (string.IsNullOrEmpty(errorMessages.ToString()) == false)
		{
			this.WdvProductOptionValueErrorMessage.Visible = true;
			this.WdvProductOptionValueErrorMessage.InnerHtml = errorMessages.ToString();
		}

		return string.IsNullOrEmpty(errorMessages.ToString());
	}

	/// <summary>
	/// 商品付帯情報更新キャンセル
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbInputProductOptionValueCancel_Click(object sender, EventArgs e)
	{
		this.ModifyProductIndex = null;
		this.WrFixedPurchaseModifyProducts.DataSource = this.InputProductList;
		this.WrFixedPurchaseModifyProducts.DataBind();
	}

	/// <summary>
	/// メール便のエスカレーション後の配送サービスを取得
	/// </summary>
	/// <returns>結果</returns>
	private string GetChangeMailDeliveryCompany()
	{
		var resultDeliveryCompanyId = this.FixedPurchaseContainer.Shippings[0].DeliveryCompanyId;
		if (this.FixedPurchaseContainer.Shippings[0].IsMail == false) return resultDeliveryCompanyId;

		if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED)
		{
			var items = GetModifyProducts(notDeleteProduct: false).ToArray();
			var shopShipping = new ShopShippingService().Get(this.FixedPurchaseContainer.ShopId, this.FixedPurchaseContainer.ShippingType);
			var deliveryCompanyId = OrderCommon.DeliveryCompanyMailEscalation(items.Select(p => p.CreateModel()), shopShipping.CompanyListMail);

			if(deliveryCompanyId != this.FixedPurchaseContainer.Shippings[0].DeliveryCompanyId)
				resultDeliveryCompanyId = string.IsNullOrEmpty(deliveryCompanyId)
					? shopShipping.CompanyListExpress.FirstOrDefault(item => item.IsDefault).DeliveryCompanyId
					: deliveryCompanyId;
		}

		return resultDeliveryCompanyId;
	}

	/// <summary>
	/// 定期商品変更情報セット
	/// </summary>
	private void SetFixedPurchaseProductChange()
	{
		var fixedPurchaseProductChangeSettingService = new FixedPurchaseProductChangeSettingService();
		foreach (var inputProduct in this.InputProductList)
		{
			inputProduct.ProductChangeSetting = fixedPurchaseProductChangeSettingService.GetByProductId(
				inputProduct.ProductId,
				inputProduct.VariationId != inputProduct.ProductId
					? inputProduct.VariationId
					: string.Empty,
				this.ShopId);
		}
	}

	/// <summary>
	/// 削除チェックボックス押下時
	/// </summary>
	protected void cbDeleteProduct_CheckedChanged(object sender, EventArgs e)
	{
		var hasNotDeleteItem = this.WrFixedPurchaseModifyProducts.Items
			.Cast<RepeaterItem>()
			.Select(ri => GetWrappedControl<WrappedCheckBox>(ri, "cbDeleteProduct"))
			.Any(deleteProduct => (deleteProduct.Checked == false));

		this.WsProductModifyErrorMessage.InnerText = hasNotDeleteItem == false
			? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_MODIFY_NOT_PRODUCT)
			: string.Empty;

		// 情報更新ボタンを非表示
		this.WbtnModifyConfirm.Visible = hasNotDeleteItem;

		// 再計算処理
		ProductModifyReCalculation(sender, e);
	}

	/// <summary>
	/// 定期商品変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rFixedPurchaseModifyProducts_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		CreateItemQuantityDropdownList(sender, e);
	}

	/// <summary>
	/// 商品数変更のドロップダウンリスト作成
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void CreateItemQuantityDropdownList(object sender, RepeaterItemEventArgs e)
	{
		if ((e.Item.ItemType == ListItemType.Item) || (e.Item.ItemType == ListItemType.AlternatingItem))
		{
			var ddlQuantityUpdate = (DropDownList)e.Item.FindControl("ddlQuantityUpdate");
			var maxValueHiddenField = (HiddenField)e.Item.FindControl("hfMaxItemQuantity");
			var itemQuantityHiddenField = (HiddenField)e.Item.FindControl("hfItemQuantity");

			var maxValue = int.Parse(maxValueHiddenField.Value);
			var itemQuantity = int.Parse(itemQuantityHiddenField.Value);
			var itemQuantityOld = 0;
			this.OldFixedPurchaseItemInput = GetFixedPurchaseItemInputs(new FixedPurchaseInput(this.FixedPurchaseContainer));
			if ((this.OldFixedPurchaseItemInput != null)
				&& this.OldFixedPurchaseItemInput.Length > e.Item.ItemIndex)
			{
				int.TryParse(this.OldFixedPurchaseItemInput[e.Item.ItemIndex].ItemQuantity, out itemQuantityOld);
			}

			var selectedValue = Math.Max(maxValue, itemQuantityOld);
			// マックス値を元にドロップダウンリストを作成する
			ddlQuantityUpdate.Items.Clear();
			for (var i = 1; i <= selectedValue; i++)
			{
				ddlQuantityUpdate.Items.Add(new ListItem(i.ToString(), i.ToString()));
			}
			// 選択値をセットする
			ddlQuantityUpdate.SelectedValue = itemQuantity.ToString();
		}
	}

	/// <summary>
	/// 購入可能な商品購入数か
	/// </summary>
	/// <param name="productVariation">商品情報</param>
	/// <param name="targetItem">定期商品入力情報情報</param>
	/// <returns>エラーメッセージ</returns>
	protected string CanSalableMaxSellQuantityLimit(DataView productVariation, FixedPurchaseItemInput targetItem)
	{
		var errorMessage = string.Empty;
		var maxSellQuantity = (int)productVariation[0][Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY] + 1;
		var productName = CreateProductJointName(
			(string)productVariation[0][Constants.FIELD_PRODUCT_NAME],
			(string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1],
			(string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME2],
			(string)productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME3]);
		if ((targetItem != null) && (this.CanSalable((int)productVariation[0][Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY], targetItem) == false))
		{
			errorMessage = WebMessages
				.GetMessages(WebMessages.ERRMSG_FRONT_PRODUCT_MAX_SELL_QUANTITY_LIMIT_ERROR)
				.Replace("@@ 1 @@", productName)
				.Replace("@@ 2 @@", maxSellQuantity.ToString());
		}
		return errorMessage;
	}

	/// <summary>
	/// 購入可能か
	/// </summary>
	/// <param name="maxSellQuantity">商品購入限度数</param>
	/// <param name="targetItem">定期商品入力情報情報</param>
	/// <returns>購入可能か</returns>
	protected bool CanSalable(int maxSellQuantity, FixedPurchaseItemInput targetItem)
	{
		int itemQuantity;
		itemQuantity = int.TryParse(targetItem.ItemQuantity, out itemQuantity) ? itemQuantity : 0;
		if (this.CanSalableMaxSellQuantityLimit(maxSellQuantity, itemQuantity) == false)
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// 数値バリデーションエラーか
	/// </summary>
	/// <param name="quantity">数値</param>
	/// <returns>数値バリデーションエラーか</returns>
	protected bool IsNumberValidationError(string quantity)
	{
		int tmpQuantity;
		var result = (string.IsNullOrEmpty(quantity)
			|| (int.TryParse(quantity, out tmpQuantity) == false)
			|| (tmpQuantity < 1));
		return result;
	}

	#region プロパティ
	/// <summary>クレジットカード決済か [V5.11互換]</summary>
	protected bool IsPaymentCreditCard
	{
		get { return (this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT); }
	}
	/// <summary>クレジットカード入力モードON [V5.11互換]</summary>
	protected bool IsCreditInputModeOn
	{
		get { return ((string)ViewState["IsCreditInputModeOn"] == "1"); }
		set { ViewState["IsCreditInputModeOn"] = value ? "1" : ""; }
	}
	/// <summary>カード情報変更リンク(ボタン)の表示状態 [V5.11互換]</summary>
	protected bool DisplayCardInfoChangeLink
	{
		get { return (bool)ViewState["DisplayCardInfoChangeLink"]; }
		set { ViewState["DisplayCardInfoChangeLink"] = value; }
	}
	/// <summary>配送先情報変更有無</summary>
	protected bool IsUserShippingModify
	{
		get { return (ViewState["IsUserShippingModify"] != null) ? (bool)ViewState["IsUserShippingModify"] : false; }
		set { ViewState["IsUserShippingModify"] = value; }
	}
	/// <summary>定期購入スキップ可能かどうか</summary>
	protected bool BeforeCancelDeadline
	{
		get { return (bool)ViewState["BeforeCancelDeadline"]; }
		set { ViewState["BeforeCancelDeadline"] = value; }
	}
	/// <summary>完了メッセージ</summary>
	protected string CompleteMessage { get; set; }
	/// <summary>郵便番号入力チェックエラーメッセージ</summary>
	protected string ZipInputErrorMessage
	{
		get { return ViewState["ZipInputErrorMessage"] != null ? (string)ViewState["ZipInputErrorMessage"] : ""; }
		set { ViewState["ZipInputErrorMessage"] = value; }
	}
	/// <summary>次回購入の利用ポイント入力チェックエラーメッセージ</summary>
	protected string NextShippingUsePointErrorMessage { get; set; }
	/// <summary>今すぐ注文した時のメッセージ</summary>
	protected string OrderNowMessagesHtmlEncoded
	{
		get { return StringUtility.ToEmpty(Session["OrderNowMessages"]); }
		set { Session["OrderNowMessages"] = value; }
	}
	/// <summary>定期注文登録された注文IDリスト</summary>
	protected List<string> RegisteredOrderIds
	{
		get { return (List<string>)Session["RegisteredOrderIds"] ?? new List<string>(); }
		set { Session["RegisteredOrderIds"] = value; }
	}
	/// <summary>
	/// キャンセル定期再開したときのメッセージ
	/// </summary>
	protected string ResumeFixedPurchaseMessageHtmlEncoded
	{
		get { return StringUtility.ToEmpty(Session["ResumeFixedPurchaseMessageHtmlEncoded"]); }
		set { Session["ResumeFixedPurchaseMessageHtmlEncoded"] = value; }
	}
	/// <summary>ポイントをリセットした時のメッセージ</summary>
	protected string PointResetMessages
	{
		get { return StringUtility.ToEmpty(Session["PointResetMessages"]); }
		set { Session["PointResetMessages"] = value; }
	}
	/// <summary>
	/// PCの場合は区切り線を伸ばすための文字列
	/// </summary>
	protected string BorderString
	{
		get { return this.IsPc ? "-----------" : ""; }
	}
	/// <summary>配送先の住所が日本か</summary>
	public bool IsShippingAddrJp
	{
		get { return GlobalAddressUtil.IsCountryJp(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>配送先の住所がUSか</summary>
	public bool IsShippingAddrUs
	{
		get { return GlobalAddressUtil.IsCountryUs(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>配送先の住所がTWか</summary>
	public bool IsShippingAddrTw
	{
		get { return GlobalAddressUtil.IsCountryTw(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>配送先の住所郵便番号が必須か</summary>
	public bool IsShippingAddrZipNecessary
	{
		get { return GlobalAddressUtil.IsAddrZipcodeNecessary(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>配送先の住所国ISOコード</summary>
	public string ShippingAddrCountryIsoCode
	{
		get { return this.WddlShippingCountry.SelectedValue; }
	}
	/// <summary>利用可能のクーポン詳細情報</summary>
	protected UserCouponDetailInfo[] UsableCoupons
	{
		get { return (UserCouponDetailInfo[])ViewState["UsableCoupons"] ?? new UserCouponDetailInfo[0]; }
		set { ViewState["UsableCoupons"] = value; }
	}
	/// <summary>次回購入の利用クーポン入力チェックエラーメッセージ</summary>
	protected string NextShippingUseCouponErrorMessage { get; set; }
	/// <summary>配送パターン表示するかどうか</summary>
	protected new bool DisplayFixedPurchaseShipping
	{
		get
		{
			if (Constants.FIXED_PURCHASE_USESHIPPINGINTERVALDAYSDEFAULT_FLG && this.ShopShipping.IsValidFixedPurchaseKbn3Flg)
			{
				return (this.CanCancelFixedPurchase 
					&& this.BeforeCancelDeadline
					&& this.ShopShipping.IsFixedPurchaseShippingDisplay
					&& (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false));
			}

			return (this.CanCancelFixedPurchase
				&& this.BeforeCancelDeadline
				&& (this.FixedPurchaseContainer.IsUnavailableShippingAreaStatus == false));
		}
	}
	/// <summary>領収書情報変更有無</summary>
	protected bool IsReceiptInfoModify
	{
		get { return ((this.ViewState["IsReceiptInfoModify"] != null) && (bool)this.ViewState["IsReceiptInfoModify"]); }
		set { this.ViewState["IsReceiptInfoModify"] = value; }
	}
	/// <summary>領収書情報変更可能かの判定</summary>
	protected bool CanModifyReceiptInfo
	{
		get
		{
			return (this.CanCancelFixedPurchase
				&& this.BeforeCancelDeadline
				&& (Constants.NOT_OUTPUT_RECEIPT_PAYMENT_KBN.Contains(this.FixedPurchaseContainer.OrderPaymentKbn) == false));
		}
	}
	/// <summary>領収書情報更新のエラーメッセージ</summary>
	protected string ReceiptInfoModifyErrorMessage { get; set; }
	/// <summary>Number Limit Skip FixedPurchase</summary>
	protected int? NumberLimitSkipFixedPurchase
	{
		get
		{
			return this.FixedPurchaseContainer.Shippings[0].Items
				.Min(item => item.FixedPurchaseLimitedSkippedCount);
		}
	}
	/// <summary>Has Click Skip Next Shipping</summary>
	protected bool HasClickSkipNextShipping
	{
		get
		{
			return ((this.NumberLimitSkipFixedPurchase.HasValue == false)
				|| (this.FixedPurchaseContainer.SkippedCount < this.NumberLimitSkipFixedPurchase));
		}
	}
	/// <summary>Display Skip Next Shipping (message & button)</summary>
	protected bool DisplaySkipNextShipping
	{
		get
		{
			return ((this.NumberLimitSkipFixedPurchase.HasValue == false)
				|| (this.NumberLimitSkipFixedPurchase > 0));
		}
	}
	/// <summary>Is Taiwan Fixed Purchase Invoice Modify</summary>
	protected bool IsTwFixedPurchaseInvoiceModify
	{
		get
		{
			return ((ViewState["IsTwFixedPurchaseInvoiceModify"] != null)
				? (bool)ViewState["IsTwFixedPurchaseInvoiceModify"]
				: false);
		}
		set { ViewState["IsTwFixedPurchaseInvoiceModify"] = value; }
	}
	/// <summary>Is Show Taiwan Fixed Purchase Invoice Information</summary>
	protected bool IsShowTwFixedPurchaseInvoiceInfo
	{
		get
		{
			return (OrderCommon.DisplayTwInvoiceInfo(this.FixedPurchaseContainer.Shippings[0].ShippingCountryIsoCode)
				&& (this.TwFixedPurchaseInvoiceModel != null));
		}
	}
	/// <summary>次回購入の利用クーポン追加可能か</summary>
	protected bool CanAddNextShippingUseCoupon
		{
		get { return ((this.FixedPurchaseContainer.NextShippingUseCouponDetail == null) && this.CanCancelFixedPurchase); }
		}
	/// <summary>次回購入の利用クーポンキャンセル可能か</summary>
	protected bool CanCancelNextShippingUseCoupon
	{
		get { return ((this.FixedPurchaseContainer.NextShippingUseCouponDetail != null) && this.CanCancelFixedPurchase); }
	}
	/// <summary>Has Shipping Time</summary>
	protected string HasShippingTime
	{
		get
		{
			return ((string.IsNullOrEmpty(this.DeliveryCompany.GetShippingTimeMessage(this.FixedPurchaseShippingContainer.ShippingTime)) == false)
				&& (this.FixedPurchaseShippingContainer.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF))
					? this.DeliveryCompany.GetShippingTimeMessage(this.FixedPurchaseShippingContainer.ShippingTime)
					: ReplaceTag("@@DispText.shipping_time_list.none@@");
		}
	}
	/// <summary>アマゾンリクエスト</summary>
	protected AmazonCv2Redirect AmazonRequest { get; set; }
	/// <summary>アマゾンチェックアウトセッション</summary>
	protected CheckoutSessionResponse AmazonCheckoutSession { get; set; }
	/// <summary>アマゾンチェックアウトセッションID</summary>
	protected string AmazonCheckoutSessionId
	{
		get { return Request[AmazonCv2Constants.REQUEST_KEY_AMAZON_CHECKOUT_SESSION_ID]; }
	}
	/// <summary>アマゾンアクションステータス</summary>
	protected string AmazonActionStatus
	{
		get { return Request[AmazonCv2Constants.REQUEST_KEY_AMAZON_ACTION_STATUS]; }
	}
	/// <summary>AmazonCv2ApiFacade</summary>
	private AmazonCv2ApiFacade AmazonFacade { get; set; }
	/// <summary>バリエーション変更可否説明が必要か</summary>
	protected bool IsDisplayMessageAboutChangeVariation
	{
		get { return (this.BeforeCancelDeadline == false); }
	}
	/// <summary>商品付帯情報変更可否説明が必要か</summary>
	protected bool IsDisplayMessageAboutChangeProductOptionValue
	{
		get { return (this.BeforeCancelDeadline == false) && this.DisplayProductOptionValueArea; }
	}
	/// <summary>変更後の単価</summary>
	protected decimal NewPrice { get; set; }
	/// <summary>選択したバリエーションインデックス</summary>
	protected int SelectedVariationIndex { get; set; }
	/// <summary>ドロップダウン先頭のバリエーションID</summary>
	protected string TopVariationId
	{
		get { return (string)ViewState["TopVariationId"]; }
		set { ViewState["TopVariationId"] = value; }
	}
	/// <summary>バリエーション変更エリア表示</summary>
	protected bool DisplayVariationArea { get; set; }
	/// <summary>商品付帯情報変更エリア表示</summary>
	protected bool DisplayProductOptionValueArea { get; set; }
	/// <summary>編集している商品情報のIndex</summary>
	protected int? ModifyProductIndex
	{
		get { return (int?)ViewState["ModifyProductIndex"]; }
		set { ViewState["ModifyProductIndex"] = value; }
	}
	/// <summary>注文拡張項目 変更有無</summary>
	protected bool IsOrderExtendModify
	{
		get { return ((this.ViewState["IsOrderExtendModify"] != null) && (bool)this.ViewState["IsOrderExtendModify"]); }
		set { this.ViewState["IsOrderExtendModify"] = value; }
	}
	/// <summary>定期購入キャンセルボタンが表示可能か</summary>
	protected bool CanDisplayCancelFixedPurchaseButton
	{
		get
		{
			return (this.CanCancelFixedPurchase
				&& this.IsCancelable
				&& (OrderCommon.IsOnlyCancelablePaymentContinuousByManual(this.FixedPurchaseContainer.OrderPaymentKbn) == false));
		}
	}
	/// <summary>定期購入一時休止ボタンが表示可能か</summary>
	protected bool CanDisplaySuspendFixedPurchaseButton
	{
		get { return (this.CanSuspendFixedPurchase && this.IsCancelable); }
	}
	/// <summary>再開不可のPaypay決済か</summary>
	public bool IsInvalidResumePaypay
	{
		get
		{
			return ((this.FixedPurchaseContainer.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)
				&& this.FixedPurchaseContainer.IsCancelFixedPurchaseStatus)
				&& ((Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.GMO)
					|| (Constants.PAYMENT_PAYPAY_KBN == Constants.PaymentPayPayKbn.VeriTrans));
		}
	}
	/// <summary>商品変更可能か</summary>
	protected bool IsAbleToChangeSubscriptionBoxItemOnFront
	{
		get
		{
			if (this.FixedPurchaseContainer.SubscriptionBoxCourseId == "") return false;
			if (this.FixedPurchaseContainer.IsOrderRegister == false) return false;

			var model = new SubscriptionBoxService().GetByCourseId(StringUtility.ToEmpty(this.FixedPurchaseContainer.SubscriptionBoxCourseId));
			return (model.ItemsChangeableByUser == Constants.FLG_SUBSCRIPTIONBOX_ITEMS_CHANGEABLE_BY_USER_TRUE);

		}
		set { this.ViewState["IsOrderExtendModify"] = value; }
	}
	/// <summary>頒布会商品一覧リスト</summary>
	public ListItem[] SubscriputionBoxProductListItem { get; set; }
	/// <summary>次回頒布会商品一覧</summary>
	protected SubscriptionBoxGetNextProductsResult SubscriptionBoxGetNextProductsResult
	{
		get
		{
			return _subscriptionBoxGetNextProductsResult ?? (_subscriptionBoxGetNextProductsResult =
				new SubscriptionBoxService().GetFixedPurchaseNextProduct(
					this.FixedPurchaseContainer.SubscriptionBoxCourseId,
					this.FixedPurchaseContainer.FixedPurchaseId,
					this.FixedPurchaseContainer.MemberRankId,
					this.FixedPurchaseContainer.NextShippingDate.Value,
					this.FixedPurchaseContainer.SubscriptionBoxOrderCount,
					this.FixedPurchaseContainer.Shippings[0]));
		}
	}
	private SubscriptionBoxGetNextProductsResult _subscriptionBoxGetNextProductsResult;
	/// <summary>頒布会商品変更モーダルのURL(iframe)</summary>
	protected string SubscriptionBoxProductChangeModalUrl { get; set; }
	/// <summary>配送不可住所</summary>
	private new string UnavailableShippingZip
	{
		get
		{
			var unavailableShippingZip = new ShopShippingService().GetUnavailableShippingZipFromShippingDelivery(
				this.FixedPurchaseContainer.ShippingType,
				this.FixedPurchaseContainer.Shippings[0].DeliveryCompanyId);
			return unavailableShippingZip;
		}
	}
	/// <summary>リクエスト：ページ番号</summary>
	private int RequestPageNum
	{
		get
		{
			int pageNum;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNum) ? pageNum : 1;
		}
	}
	/// <summary>商品付帯情報更新後のスクロール位置の要素ID</summary>
	/// <remarks>付帯情報更新後にスクロール位置がリセットされないように利用する</remarks>
	public string ScrollPositionIdAfterProductOptionUpdate
	{
		get { return (string)Session["ScrollPositionIdAfterProductOptionUpdate"]; }
		private set { Session["ScrollPositionIdAfterProductOptionUpdate"] = value; }
	}
	/// <summary>ページャーHTML</summary>
	protected string PagerHtml
	{
		get { return (string)ViewState["PagerHtml"]; }
		private set { ViewState["PagerHtml"] = value; }
	}
	/// <summary>クレジットカード登録するチェックボックス表示するかどうか</summary>
	protected bool IsCreditRegistCheckBoxDisplay
	{
		get
		{
			var isCreditRegistable = OrderCommon.GetCreditCardRegistable(this.IsLoggedIn, this.UserCreditCardsUsable.Length);
			var isMaxRegisterCreditCartBiggerThanZero = Constants.MAX_NUM_REGIST_CREDITCARD > 0;
			var isCreditRegistCheckBoxDisplay = isMaxRegisterCreditCartBiggerThanZero && isCreditRegistable;
			return isCreditRegistCheckBoxDisplay;
		}
	}
	/// <summary>クレジットカード遷移するドロップダウン表示するかどうか</summary>
	protected bool IsCreditListDropDownDisplay
	{
		get
		{
			var isCreditListDropDownDisplay = (Constants.MAX_NUM_REGIST_CREDITCARD > 0) && (this.UserCreditCardsUsable.Length > 0);
			return isCreditListDropDownDisplay;
		}
	}	
	/// <summary>クーポンによる配送料が無料かどうか</summary>
	protected bool IsFreeShippingByCoupon { get; set; }
	/// <summary>次回注文時のお支払い予定金額、総合計</summary>
	protected decimal PlannedTotalAmountForNextOrder { get; set; }
	/// <summary>次回購入の定期注文カート</summary>
	protected CartObject NextShippingFixedPurchaseCart { get; set; }
	/// <summary>配送希望時間帯変更有無(AmazonPayCv2)</summary>
	protected bool IsShippingTimeModifyAmazonPayCv2
	{
		get { return ((this.ViewState["IsShippingTimeModifyAmazonPayCv2"] != null) && (bool)this.ViewState["IsShippingTimeModifyAmazonPayCv2"]); }
		set { this.ViewState["IsShippingTimeModifyAmazonPayCv2"] = value; }
	}
	/// <summary>商品数変更状態か</summary>
	protected bool IsProductModify
	{
		get { return (ViewState["IsProductModify"] != null) && (bool)ViewState["IsProductModify"]; }
		set { ViewState["IsProductModify"] = value; }
	}
	/// <summary>商品数変更ボタンを表示するか</summary>
	/// <remarks>表示する条件：頒布会以外、ステータスは通常の場合、定期台帳に紐付けている商品は一つも削除されていない</remarks>
	protected bool IsOrderModifyBtnDisplay
	{
		get { return (this.FixedPurchaseContainer.IsSubsctriptionBox == false)
			&& (this.FixedPurchaseContainer.FixedPurchaseStatus == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL)
			&& (this.FixedPurchaseContainer.HasDeletedAnyFixedPurchaseItem == false); }
	}
	/// <summary>入力用商品リスト</summary>
	protected List<FixedPurchaseItemInput> InputProductList
	{
		get { return (List<FixedPurchaseItemInput>)Session["InputProductList"]; }
		set { Session["InputProductList"] = value; }
	}
	/// <summary>定期商品リスト</summary>
	public FixedPurchaseItemInput[] OldFixedPurchaseItemInput { get; set; }
	#endregion
}
