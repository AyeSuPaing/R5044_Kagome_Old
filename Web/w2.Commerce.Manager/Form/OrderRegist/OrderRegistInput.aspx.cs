/*
=========================================================================================================
  Module      : 注文情報登録ページ処理(OrderRegistInput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Config;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Input.Order;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Cart;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.LinePay;
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.PayTg;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Helper;
using w2.App.Common.Order.Payment.Veritrans;
using w2.App.Common.Order.Register;
using w2.App.Common.Order.UserCreditCardCooperationInfos;
using w2.App.Common.OrderExtend;
using w2.App.Common.Product;
using w2.App.Common.User;
using w2.App.Common.Util;
using w2.App.Common.Web.Process;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.AdvCode;
using w2.Domain.ContentsLog;
using w2.Domain.CountryLocation;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchase.Helper;
using w2.Domain.Holiday.Helper;
using w2.Domain.Order;
using w2.Domain.Payment;
using w2.Domain.Point;
using w2.Domain.Product;
using w2.Domain.RealShop;
using w2.Domain.ShopShipping;
using w2.Domain.SubscriptionBox;
using w2.Domain.TwUserInvoice;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserManagementLevel;
using w2.Domain.UserShipping;
using Environment = System.Environment;

/// <summary>
/// 注文情報登録ページ処理
/// </summary>
public partial class Form_OrderRegist_OrderRegistInput : OrderRegistPage
{
	/// <summary>Can delete key</summary>
	protected const string CONST_KEY_CAN_DELETE = "CanDelete";
	/// <summary>Format short date</summary>
	protected const string CONST_FORMAT_SHORT_DATE = "yyyy/MM/dd";
	/// <summary>Product option value settings</summary>
	protected const string CONST_KEY_PRODUCT_OPTION_VALUE_SETTINGS = "product_option_value_settings";
	/// <summary>Fixed pruchase</summary>
	protected const string CONST_KEY_FIXED_PURCHASE = "fixedpurchase";
	/// <summary>ハッシュキー：カート商品用の注文同梱有無</summary>
	protected const string CONST_HASHKEY_CART_PRODUCT_IS_ORDER_COMBINED = "cartproduct_is_order_combined";
	/// <summary>Add row numbering</summary>
	protected const string CONST_FIELD_ADD_ROW_NUMBERING = "add_row_numbering";

	/// <summary>Max display show for history</summary>
	protected const int CONST_MAX_DISPLAY_SHOW_FOR_HISTORY = 4;
	/// <summary>Max display show for combine</summary>
	protected const int CONST_MAX_DISPLAY_SHOW_FOR_COMBINE = 1;
	/// <summary>Max display show for novelty</summary>
	protected const int CONST_MAX_DISPLAY_SHOW_FOR_NOVELTY = 5;
	/// <summary>Max display show for search autocomplete</summary>
	protected const int CONST_MAX_DISPLAY_SHOW_FOR_SEARCH_AUTOCOMPLETE = 10;

	/// <summary>Search type: USER</summary>
	protected const string CONST_SEARCH_TYPE_USER = "USER";
	/// <summary>Search type: ADVCODE</summary>
	protected const string CONST_SEARCH_TYPE_ADVCODE = "ADVCODE";
	/// <summary>Search type: COUPON</summary>
	protected const string CONST_SEARCH_TYPE_COUPON = "COUPON";
	/// <summary>Search type: COUPON</summary>
	protected const string CONST_SEARCH_TYPE_PRODUCT = "PRODUCT";
	/// <summary>Max content show for search autocomplete</summary>
	protected const int CONST_MAX_CONTENT_SHOW_FOR_SEARCH_AUTOCOMPLETE = 20;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// コンポーネント初期化
			InitializeComponents();

			// パラメタにユーザーIDがあればデフォルト読み込み
			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_USER_ID]) == false)
			{
				SetOwner(Request[Constants.REQUEST_KEY_USER_ID]);
			}

			// パラメタに再注文の注文IDがあれば再注文情報を設定
			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_REORDER_ID]) == false)
			{
				this.ReOrderId = Request[Constants.REQUEST_KEY_REORDER_ID];
				this.SetReOrderData();
			}
			else if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_REORDER_FIXEDPURCHASE_ID]) == false)
			{
				this.ReFixedPurchaseId = Request[Constants.REQUEST_KEY_REORDER_FIXEDPURCHASE_ID];
				this.SetReOrderDataFixedPurchase();
			}

			// パラメータに電話番号があれば設定
			if (string.IsNullOrEmpty(Request[Constants.REQUEST_KEY_TEL_NO]) == false)
			{
				var telNo = Request[Constants.REQUEST_KEY_TEL_NO];
				if (this.IsOwnerAddrJp)
				{
					var tel = new Tel(telNo, true);
					tbOwnerTel1_1.Text = tel.Tel1;
					tbOwnerTel1_2.Text = tel.Tel2;
					tbOwnerTel1_3.Text = tel.Tel3;
				}
				else
				{
					tbOwnerTel1Global.Text = telNo;
				}
			}

			// 再注文警告メッセージ表示／非表示設定
			dvReOrderWarning.Visible = ((string.IsNullOrEmpty(this.ReOrderId) == false)
				|| (string.IsNullOrEmpty(this.ReFixedPurchaseId) == false));

			// セッションに注文情報があればユーザー情報を画面に設定(確認画面から戻る場合を想定)
			if (this.IsBackFromConfirm)
			{
				SetOrderOwnerFromOrderDatas((Hashtable)Session[Constants.SESSION_KEY_PARAM_FOR_BACK]);
			}

			// 注文同梱から遷移の場合、注文同梱後の情報を入力欄にセット
			if ((this.ActionStatus == Constants.ACTION_STATUS_ORDERCOMBINE)
				&& (this.IsBackFromConfirm == false))
			{
				this.IsOrderCombined = true;
				this.CombineParentOrderId =
					StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDERCOMBINE_PARENT_ORDER_ID]);
				this.CombineChildOrderIds =
					StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDERCOMBINE_CHILD_ORDER_IDs]).Split(',');

				CartObject combinedCart;
				var errorMessage = OrderCombineUtility.CreateCombinedCart(
					this.CombineParentOrderId,
					this.CombineChildOrderIds,
					out combinedCart);

				// 同コース定額頒布会での注文同梱の場合、頒布会コースの数量・種類数・金額チェックを行う
				// （他にエラーが無い場合）
				if (string.IsNullOrEmpty(errorMessage)
					&& (combinedCart.CheckFixedAmountForCombineWithSameCourse() == false))
				{
					errorMessage = WebMessages.GetMessages(
							CommerceMessages.ERRMSG_ORDERCOMBINE_SUBSCRIPTION_BOX_FIXED_AMOUNT_CHECK_INVALID)
						.Replace("@@ 1 @@", combinedCart.SubscriptionBoxErrorMsg);
				}

				if (string.IsNullOrEmpty(errorMessage) == false)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
				this.OriginOrderCombineCart = combinedCart;

				hfPaidyTokenId.Value = combinedCart.Payment.PaidyToken;
				this.CombineParentTranId = combinedCart.OrderCombineParentTranId;
				this.IsCombineOrderSalesSettled = combinedCart.IsOrderSalesSettled;

				if (combinedCart.IsCombineParentOrderHasFixedPurchase)
				{
					this.CombineParentOrderHasFixedPurchase = combinedCart.IsCombineParentOrderHasFixedPurchase;
					this.CombineParentOrderCount = combinedCart.CombineParentOrderFixedPurchaseOrderCount;
					this.CombineParentOrderFixedPurchase = combinedCart.FixedPurchase;
					this.CombineFixedPurchaseDiscountPrice = combinedCart.FixedPurchaseDiscount;
				}

				SetOrderInfoFromCombinedCart(combinedCart);

				// 同梱前の付帯情報が最新の商品マスタから削除されている場合エラー表示
				if (combinedCart.Items.Any(item => item.ProductOptionSettingList.Items.All(po => po.MatchesLatestProductMaster) == false))
				{
					errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_PRODUCT_OPTION_UNMATCHED_ADDITIONAL_INFO);
					dvOrderItemProductOptionErrorMessage.Visible = true;
					lProductOptionErrorMessage.Text = GetEncodedHtmlDisplayMessage(errorMessage);
				}

				if (OrderCombineUtility.IsDetachedFixedPurchaseDiscount(combinedCart, this.CombineChildOrderIds))
				{
					dvOrderItemNoticeMessage.Visible = true;
					lOrderItemNoticeMessage.Text = GetEncodedHtmlDisplayMessage(
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_REGIST_ORDER_ITEM_NOTICE));
				}

				tbAdvCode.Text = (combinedCart.AdvCodeNew ?? string.Empty);
				dvBeforeCombine.Visible = false;
			}

			// ユーザー変更時イベント(クーポン一覧更新など）
			UserChangeEvent();

			if ((this.ActionStatus == Constants.ACTION_STATUS_ORDERCOMBINE)
				&& (this.IsBackFromConfirm == false)
				&& (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT))
			{
				ddlUserCreditCard.SelectedValue = OrderCombineCardInfo.BranchNo.ToString();
				lCreditLastFourDigit.Text = GetEncodedHtmlDisplayMessage(OrderCombineCardInfo.LastFourDigit);
				lCreditExpirationMonth.Text = GetEncodedHtmlDisplayMessage(OrderCombineCardInfo.ExpirationMonth);
				lCreditExpirationYear.Text = GetEncodedHtmlDisplayMessage(OrderCombineCardInfo.ExpirationYear);
				lCreditAuthorName.Text = GetEncodedHtmlDisplayMessage(OrderCombineCardInfo.AuthorName);
			}

			// トークン決済の場合はsubmitしないようにする
			if (OrderCommon.CreditTokenUse)
			{
				SetSubmitButtonBehaviorFalse(this.Form.Controls);
			}

			// 注文情報を画面項目に設定
			if (this.IsBackFromConfirm)
			{
				SetOrderData((Hashtable)Session[Constants.SESSION_KEY_PARAM_FOR_BACK]);
			}
			else
			{
				this.DeletedNoveltyIds = new List<string>();
			}
			Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = null;

			if (this.IsUserPayTg
				&& (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.VeriTrans))
			{
				// PayTg端末状態取得
				GetPayTgDeviceStatus();
			}
		}
		else
		{
			lbAdvName.Text = GetEncodedHtmlDisplayMessage(
				(string.IsNullOrEmpty(hfAdvName.Value) == false)
					? hfAdvName.Value
					: string.Empty);
			dvReOrderWarning.Visible = false;
			if (OrderCommon.DisplayTwInvoiceInfo()
				&& this.IsShippingAddrTw
				&& (string.IsNullOrEmpty(hfUserId.Value) == false)
				&& string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue))
			{
				// Refresh Uniform Invoice Or Carry Type Option
				ddlUniformInvoiceOrCarryTypeOption.DataSource = GetUniformInvoiceOrCarryTypeOption(
					ddlUniformInvoiceType.SelectedValue,
					ddlInvoiceCarryType.SelectedValue);
				ddlUniformInvoiceOrCarryTypeOption.DataBind();
			}
		}

		// トークンが入力されていたら入力画面を切り替える
		SwitchDisplayForCreditTokenInput();

		// 項目メモ一覧取得
		this.FieldMemoSettingList = GetFieldMemoSettingList(Constants.TABLE_ORDER);

		// 配送先と注文者の住所を同じにするチェックボックスの制御
		ControlShippingSameAsOwnerCheckBox();

		// 制限されるユーザー管理レベルを表示
		DispFixedPurchaseLimitUserLevel(ddlUserManagementLevel.SelectedValue);

		// Display Convenience Store Data
		DisplayConvenienceStoreData();
	}

	/// <summary>
	/// サブミットボタンのUseSubmitBehaviorをfalseにする（__doPostBackなどを経由してほしい時に利用）
	/// </summary>
	/// <param name="controls">コントロール</param>
	private void SetSubmitButtonBehaviorFalse(ControlCollection controls)
	{
		foreach (Control control in controls)
		{
			if (control is Button)
			{
				var button = (Button)control;
				if (button.UseSubmitBehavior)
				{
					button.UseSubmitBehavior = false;
				}
			}
			else
			{
				SetSubmitButtonBehaviorFalse(control.Controls);
			}
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		// 注文区分
		foreach (var listItem in ValueText.GetValueItemArray(
			Constants.TABLE_ORDER,
			Constants.FIELD_ORDER_ORDER_KBN))
		{
			// モバイルデータの表示OPがOFFの場合は注文区分がモバイル注文を追加しない
			if ((Constants.DISPLAYMOBILEDATAS_OPTION_ENABLED == false)
				&& (listItem.Value == Constants.FLG_ORDER_ORDER_KBN_MOBILE)) continue;
			ddlOrderKbn.Items.Add(listItem);
		}
		ddlOrderKbn.SelectedValue = Constants.ORDER_DEFALUT_ORDER_KBN;

		// 流入コンテンツタイプ
		ddlInflowContentsType.Items.AddRange(ValueText.GetValueItemArray(
			Constants.TABLE_ORDER,
			Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE));

		// カード会社
		if (OrderCommon.CreditCompanySelectable)
		{
			ddlCreditCardCompany.Items.AddRange(ValueText.GetValueItemArray(
				Constants.TABLE_ORDER,
				OrderCommon.CreditCompanyValueTextFieldName));
		}

		// カード有効期限(月)
		ddlCreditExpireMonth.Items.AddRange(this.CreditExpirationMonthListItems);
		ddlCreditExpireMonth.SelectedValue = DateTime.Now.Month.ToString("00");

		// カード有効期限(年)
		ddlCreditExpireYear.Items.AddRange(this.CreditExpirationYearListItems);
		ddlCreditExpireYear.SelectedValue = DateTime.Now.Year.ToString("00").Substring(2);

		// カード分割支払い
		dvInstallments.Visible = OrderCommon.CreditInstallmentsSelectable;
		if (OrderCommon.CreditInstallmentsSelectable)
		{
			dllCreditInstallments.Items.AddRange(ValueText.GetValueItemArray(
				Constants.TABLE_ORDER,
				OrderCommon.CreditInstallmentsValueTextFieldName));
		}

		// カードセキュリティコード表示・非表示判定
		dvSecurityCode.Visible = (OrderCommon.CreditSecurityCodeEnable && (this.IsUserPayTg == false));

		// クレジット情報入力域を表示
		dvPaymentKbnCredit.Visible = (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);

		// ユーザーID
		lUserId.Text = string.Empty;
		lUserIdNonSet.Text = GetEncodedHtmlDisplayMessage(
			WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USER_ID_NOT_SET_ALERT));
		hfUserId.Value = string.Empty;
		hfMemberRankId.Value = string.Empty;
		hfFixedPurchaseMember.Value = string.Empty;

		// 注文者区分
		ddlOwnerKbn.Items.Add(
			new ListItem(
				ValueText.GetValueText(
					Constants.TABLE_ORDEROWNER,
					Constants.FIELD_ORDEROWNER_OWNER_KBN,
					Constants.FLG_ORDEROWNER_OWNER_KBN_OFFLINE_USER),
				Constants.FLG_ORDEROWNER_OWNER_KBN_OFFLINE_USER));
		ddlOwnerKbn.Items.Add(
			new ListItem(
				ValueText.GetValueText(
					Constants.TABLE_ORDEROWNER,
					Constants.FIELD_ORDEROWNER_OWNER_KBN,
					Constants.FLG_ORDEROWNER_OWNER_KBN_OFFLINE_GUEST),
				Constants.FLG_ORDEROWNER_OWNER_KBN_OFFLINE_GUEST));
		ddlOwnerKbn.SelectedValue = hfOwnerKbn.Value = Constants.ORDER_DEFALUT_OWNER_KBN;
		lOwnerKbn.Visible = false;

		// 都道府県
		ddlOwnerAddr1.Items.Add(string.Empty);
		ddlShippingAddr1.Items.Add(string.Empty);
		foreach (var prefecture in Constants.STR_PREFECTURES_LIST)
		{
			ddlOwnerAddr1.Items.Add(prefecture);
			ddlShippingAddr1.Items.Add(prefecture);
		}

		// 性別
		foreach (ListItem list in ValueText.GetValueItemList(
			Constants.TABLE_ORDEROWNER,
			Constants.FIELD_ORDEROWNER_OWNER_SEX))
		{
			list.Selected = (list.Value == Constants.FLG_USER_SEX_UNKNOWN);
			rblOwnerSex.Items.Add(list);
		}

		// メール配信希望
		foreach (ListItem listItem in ValueText.GetValueItemList(
			Constants.TABLE_USER,
			Constants.FIELD_USER_MAIL_FLG))
		{
			rblMailFlg.Items.Add(listItem);
		}
		rblMailFlg.SelectedValue = Constants.FLG_USER_MAILFLG_UNKNOWN;

		// ユーザー管理レベル
		var models = DomainFacade.Instance.UserManagementLevelService.GetAllList()
			.Select(m => new ListItem(m.UserManagementLevelName, m.UserManagementLevelId))
			.ToArray();
		ddlUserManagementLevel.Items.AddRange(models);

		// 注文商品リピータ(3行は必ず作成）
		AddDefaultRowOrderItem();

		// 注文セットプロモーション
		rOrderSetPromotionProductDiscount.DataSource
			= rOrderSetPromotionShippingDiscount.DataSource
			= rOrderSetPromotionSettlementDiscount.DataSource
			= new CartSetPromotionList();
		rOrderSetPromotionProductDiscount.DataBind();
		rOrderSetPromotionShippingDiscount.DataBind();
		rOrderSetPromotionSettlementDiscount.DataBind();

		// 配送情報
		ddlShippingMethod.Items.AddRange(ValueText.GetValueItemArray(
			Constants.TABLE_ORDERSHIPPING,
			Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD));

		// 配送日時
		dvShippingDate.Visible = false;
		dvShippingTime.Visible = false;
		dvShippingFixedPurchase.Visible = false;
		var zeroPriceHtmlEncoded = GetEncodedHtmlDisplayMessage(0.ToPriceString(true));
		lPriceSubTotal.Text = zeroPriceHtmlEncoded;
		lOrderPriceShipping.Text = zeroPriceHtmlEncoded;
		lMemberRankDiscount.Text = zeroPriceHtmlEncoded;
		lOrderPriceExchange.Text = zeroPriceHtmlEncoded;
		tbOrderPriceRegulation.Text = 0.ToPriceString();
		lFixedPurchaseMemberDiscountAmount.Text = zeroPriceHtmlEncoded;
		lbOrderPriceTax.Text = zeroPriceHtmlEncoded;
		lOrderPriceTotal.Text = zeroPriceHtmlEncoded;
		lCouponUsePrice.Text = zeroPriceHtmlEncoded;
		lPointUsePrice.Text = zeroPriceHtmlEncoded;
		lFixedPurchaseDiscountPrice.Text = zeroPriceHtmlEncoded;
		lOrderPriceTotalBottom.Text = zeroPriceHtmlEncoded;

		//グローバル対応 項目作成
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ddlAccessCountryIsoCode.Items.Add(new ListItem(string.Empty, string.Empty));
			ddlDispCurrencyLocaleId.Items.Add(new ListItem(string.Empty, string.Empty));
			ddlDispLanguageLocaleId.Items.Add(new ListItem(string.Empty, string.Empty));
			ddlAccessCountryIsoCode.Items.AddRange(
				Constants.GLOBAL_CONFIGS
					.GlobalSettings
					.CountryIsoCodes
					.Select(accessCountryIsoCode => new ListItem(
						accessCountryIsoCode,
						accessCountryIsoCode))
					.ToArray());
			ddlDispLanguageLocaleId.Items.AddRange(
				Constants.GLOBAL_CONFIGS
					.GlobalSettings
					.Languages.Select(languages => new ListItem(
						GlobalConfigUtil.LanguageLocaleIdDisplayFormat(languages.LocaleId),
						languages.LocaleId))
					.ToArray());
			ddlDispCurrencyLocaleId.Items.AddRange(
				Constants.GLOBAL_CONFIGS
					.GlobalSettings
					.Currencies
					.SelectMany(currencies =>
						currencies
						.CurrencyLocales
						.Select(currencyLocales => new ListItem(
							GlobalConfigUtil.CurrencyLocaleIdDisplayFormat(currencyLocales.LocaleId),
							currencyLocales.LocaleId)))
					.ToArray());

			var userCountries = DomainFacade.Instance.CountryLocationService.GetCountryNames();
			ddlOwnerCountry.Items.AddRange(userCountries.Select(country => new ListItem(
				country.CountryName,
				country.CountryIsoCode)).ToArray());
			ddlOwnerCountry.SelectedValue = Constants.OPERATIONAL_BASE_ISO_CODE;

			var shippingAvailableCountries = DomainFacade.Instance.CountryLocationService.GetShippingAvailableCountry();
			ddlShippingCountry.Items.AddRange(shippingAvailableCountries.Select(country => new ListItem(
				country.CountryName,
				country.CountryIsoCode)).ToArray());
			ddlShippingCountry.SelectedValue = Constants.OPERATIONAL_BASE_ISO_CODE;

			ddlOwnerAddr5.Items.AddRange(Constants.US_STATES_LIST.Select(state => new ListItem(state)).ToArray());
			ddlShippingAddr5.Items.AddRange(Constants.US_STATES_LIST.Select(state => new ListItem(state)).ToArray());
		}

		// 注文メモ
		rOrderMemos.DataSource = GetOrderMemoSetting(Constants.FLG_ORDER_MEMO_SETTING_DISP_FLG_PC);
		rOrderMemos.DataBind();

		// Get gmo cvs type
		GetGmoCvsType();

		// Get rakuten cvs type
		GetRakutenCvsType();

		// 領収書希望
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			rblReceiptFlg.Items.AddRange(
				ValueText.GetValueItemArray(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_RECEIPT_FLG));
			rblReceiptFlg.SelectedValue = Constants.FLG_ORDER_RECEIPT_FLG_OFF;
			tbReceiptAddress.Enabled
				= tbReceiptProviso.Enabled
				= (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON);
		}

		ddlUniformInvoiceType.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_TWORDERINVOICE,
				Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE));
		ddlInvoiceCarryType.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_TWORDERINVOICE,
				Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE));
		ddlUniformInvoiceOrCarryTypeOption.Items.Add(
			new ListItem(ReplaceTag("@@DispText.uniform_invoice_option.new@@"), string.Empty));
		dvShowOrderHistory.Visible =
			dvShowFixedPurchaseHistory.Visible = false;
		dvHideOrderHistory.Visible =
			dvHideFixedPurchaseHistory.Visible = true;
		LoadShippingReceivingStoreTypeForDisplay();

		// 楽天連携かつPayTg非利用の場合、新規クレカ入力領域を非表示にする
		this.phCreditCardNotRakuten.Visible = (this.IsNotRakutenAgency || (this.IsUserPayTg && Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten));

		Page.Form.Attributes.Add("autocomplete", "off");

		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			var input = new OrderExtendInput(OrderExtendCommon.CreateOrderExtendForManager());
			rOrderExtendInput.DataSource = input.OrderExtendItems;
			rOrderExtendInput.DataBind();
			SetOrderExtendFromUserExtendObject(rOrderExtendInput, input);
		}

		if (this.IsUserPayTg && (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans))
		{
			if (this.IsBackFromConfirm == false)
			{
				this.VeriTransAccountId = string.Empty;
				this.IsSuccessfulCardRegistration = false;
			}
			if (string.IsNullOrEmpty(tbCreditCardNo1.Text)) tbCreditCardNo1.Text = Constants.PAYMENT_SETTING_PAYTG_DEFAULT_CARD_NUMBER;
			ddlCreditExpireMonth.SelectedValue = Constants.PAYMENT_SETTING_PAYTG_DEFAULT_EXPIRATION_MONTH;
			ddlCreditExpireYear.SelectedValue = Constants.PAYMENT_SETTING_PAYTG_DEFAULT_EXPIRATION_YEAR;
		}
		else if (this.IsUserPayTg && (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.VeriTrans))
		{
			if (this.IsBackFromConfirm == false)
			{
				this.IsSuccessfulCardRegistration = false;
			}
		}

			tdCreditNumber.Visible = (this.IsUserPayTg == false);
		trCreditExpire.Visible = (this.IsUserPayTg == false);
		tdGetCardInfo.Visible = this.IsUserPayTg;

		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{
			ddlSubscriptionBox.Items.Add(new ListItem("", ""));
			ddlSubscriptionBox.Items.AddRange(
				DataCacheControllerFacade.GetSubscriptionBoxCacheController().CacheData
					.Select(s => new ListItem(s.ManagementName, s.CourseId))
					.ToArray());
			ddlSubscriptionBox.DataBind();
			this.SubscriptionBoxCourseId = ddlSubscriptionBox.SelectedValue;
		}

		// Get real shop list for selection
		var realShops = new RealShopService().GetAll();
		if (realShops != null)
		{
			this.RealShopModels = realShops.Where(rs => rs.ValidFlg == Constants.FLG_ON).ToArray();
		}
		else
		{
			this.RealShopModels = new RealShopModel[0];
		}

		if (this.RealShopModels.Length > 0)
		{
			this.ddlRealStore.Items.AddRange(this.RealShopModels.Select(rs => new ListItem(rs.Name, rs.Name)).ToArray());
			this.ddlRealStore.SelectedIndex = 0;
			this.ddlRealStore_SelectedIndexChanged(null, EventArgs.Empty);
		}

		// Refresh shipping option
		RefreshShippingOption();
	}

	/// <summary>
	/// Load Shipping Receiving Store Type For Display
	/// </summary>
	protected void LoadShippingReceivingStoreTypeForDisplay()
	{
		ddlShippingReceivingStoreType.DataSource = ValueText.GetValueItemList(
			Constants.TABLE_ORDERSHIPPING,
			Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE);
		ddlShippingReceivingStoreType.DataBind();
	}

	/// <summary>
	/// Clear Convenience Store Information
	/// </summary>
	private void ClearConvenienceStoreInformation()
	{
		lCvsShopNo.Text = string.Empty;
		lCvsShopName.Text = string.Empty;
		lCvsShopAddress.Text = string.Empty;
		lCvsShopTel.Text = string.Empty;
		hfCvsShopNo.Value = string.Empty;
		hfCvsShopName.Value = string.Empty;
		hfCvsShopAddress.Value = string.Empty;
		hfCvsShopTel.Value = string.Empty;
	}

	/// <summary>
	/// Shipping Receiving Store Type Selected IndexChanged
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingReceivingStoreType_SelectedIndexChanged(object sender, EventArgs e)
	{
		ClearConvenienceStoreInformation();

		var orderItems = CreateOrderItemInput();
		if (string.IsNullOrEmpty(CheckItemsAndPricesInput(orderItems)) == false) return;

		var hasError = false;
		var cart = CreateCart(orderItems, ref hasError);

		// 配送種別取得
		var shopShipping = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, cart.ShippingType);
		if (shopShipping == null)
		{
			ddlDeliveryCompany.Items.Clear();
			return;
		}

		// 決済情報更新
		RefreshPaymentViewFromShippingType(cart, shopShipping);
	}

	/// <summary>
	/// User Shipping Selected Index Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserShipping_SelectedIndexChanged(object sender, EventArgs e)
	{
		ddlShippingReceivingStoreType.Visible
			= ddlShippingReceivingStoreType.Enabled
			= btnOpenConvenienceStoreMapEcPay.Visible
			= ((ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
				&& Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
				&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED);
		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
		{
			if ((ddlUserShipping.SelectedValue != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW)
				&& (ddlUserShipping.SelectedValue != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE))
			{
				var userShipping = this.UserShippingAddress
					.FirstOrDefault(shipping => ((shipping.ShippingNo.ToString() == ddlUserShipping.SelectedValue)
						&& (shipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
						&& (string.IsNullOrEmpty(shipping.ShippingReceivingStoreType) == false)));
				if (userShipping != null)
				{
					ddlShippingReceivingStoreType.Visible = true;
					ddlShippingReceivingStoreType.Enabled = false;
					ddlShippingReceivingStoreType.SelectedValue = userShipping.ShippingReceivingStoreType;
				}
			}
			else
			{
				if (this.IsOrderCombinedAtOrderCombinePage == false)
				{
					ClearConvenienceStoreInformation();
					ddlShippingReceivingStoreType.SelectedIndex = 0;
				}
			}
		}

		// 商品・金額表示更新
		if (this.HasItems)
		{
			RefreshItemsAndPriceView();
		}
	}

	#region aspxイベント
	/// <summary>
	/// 注文者区分変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlOwnerKbn_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 区分変更
		hfOwnerKbn.Value = ddlOwnerKbn.SelectedValue;

		// ユーザー変更時イベント
		UserChangeEvent();
	}

	/// <summary>
	/// ユーザ情報取得ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGetUserData_Click(object sender, EventArgs e)
	{
		// 注文者セット
		SetOwner(hfUserId.Value);

		// ユーザー変更時イベント
		UserChangeEvent();

		// 注文者情報の変更をユーザー情報に反映するCheckBoxの初期値
		cbAllowSaveOwnerIntoUser.Checked = Constants.DEFAULTUPDATE_TOUSER_FROMORDEROWNER;

		// 商品・金額表示更新と初回配送日、次回配送日の再計算
		if (this.HasItems)
		{
			this.IsUpdateFixedPurchaseShippingPattern = true;
			RefreshItemsAndPriceView();
		}

		lOrderOwnerErrorMessages.Text = string.Empty;
		dvOrderOwnerErrorMessages.Visible = false;
	}

	/// <summary>
	/// ユーザ情報クリアクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUserClear_Click(object sender, EventArgs e)
	{
		// 注文者情報クリア
		lbUserClear.Visible = false;
		lUserId.Text = string.Empty;
		lUserIdNonSet.Text = GetEncodedHtmlDisplayMessage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USER_ID_NOT_SET_ALERT));
		hfUserId.Value = string.Empty;
		hfFixedPurchaseMember.Value = string.Empty;
		hfMemberRankId.Value = string.Empty;
		lMemberRankName.Text = string.Empty;
		tbOwnerName1.Text = string.Empty;
		tbOwnerName2.Text = string.Empty;
		tbOwnerNameKana1.Text = string.Empty;
		tbOwnerNameKana2.Text = string.Empty;
		lOwnerKbn.Text = string.Empty;
		lOwnerKbn.Visible = false;
		ddlOwnerKbn.Visible = true;
		ddlOwnerKbn.SelectedValue = hfOwnerKbn.Value = Constants.ORDER_DEFALUT_OWNER_KBN;
		tbOwnerMailAddr.Text = string.Empty;
		tbOwnerMailAddr2.Text = string.Empty;
		tbOwnerTel1_1.Text = string.Empty;
		tbOwnerTel1_2.Text = string.Empty;
		tbOwnerTel1_3.Text = string.Empty;
		tbOwnerTel2_1.Text = string.Empty;
		tbOwnerTel2_2.Text = string.Empty;
		tbOwnerTel2_3.Text = string.Empty;
		tbOwnerZip1.Text = string.Empty;
		tbOwnerZip2.Text = string.Empty;
		ddlOwnerAddr1.SelectedIndex = 0;
		tbOwnerAddr2.Text = string.Empty;
		tbOwnerAddr3.Text = string.Empty;
		tbOwnerAddr4.Text = string.Empty;
		tbOwnerCompanyName.Text = string.Empty;
		tbOwnerCompanyPostName.Text = string.Empty;
		foreach (ListItem listItem in rblOwnerSex.Items)
		{
			listItem.Selected = (listItem.Value == Constants.FLG_USER_SEX_UNKNOWN);
		}
		ucOwnerBirth.Year = string.Empty;
		ucOwnerBirth.Month = string.Empty;
		ucOwnerBirth.Day = string.Empty;
		foreach (ListItem list in rblMailFlg.Items)
		{
			list.Selected = (list.Value == Constants.FLG_USER_MAILFLG_UNKNOWN);
		}
		tbUserMemo.Text = string.Empty;
		ddlUserManagementLevel.SelectedIndex = 0;
		dvMemberRank.Visible = false;

		// ユーザー変更時イベント
		UserChangeEvent();

		// 再計算処理（会員ランク価格の適用を行うため）
		if (this.HasItems)
		{
			RefreshItemsAndPriceView();
		}

		this.ddlShippingKbnList.SelectedValue = Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER;
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ddlAccessCountryIsoCode.SelectedValue = string.Empty;
			lDispLanguageCode.Text = string.Empty;
			hfDispLanguageCode.Value = string.Empty;
			ddlDispLanguageLocaleId.SelectedValue = string.Empty;
			lDispCurrencyCode.Text = string.Empty;
			hfDispCurrencyCode.Value = string.Empty;
			ddlDispCurrencyLocaleId.SelectedValue = string.Empty;
			ddlOwnerCountry.SelectedValue = Constants.OPERATIONAL_BASE_ISO_CODE;
		}

		rOrderHistoryList.DataSource = null;
		rOrderHistoryList.DataBind();
		rFixedPurchaseHistoryList.DataSource = null;
		rFixedPurchaseHistoryList.DataBind();
		dvShowOrderHistory.Visible = dvShowFixedPurchaseHistory.Visible = false;
		dvHideOrderHistory.Visible = dvHideFixedPurchaseHistory.Visible = true;
		dvShowDeliveryDesignation.Visible
			= dvShowOrderCombine.Visible
			= dvShowOrderPayment.Visible
			= (this.HasItems || this.HasOwner);
		dvHideOrderPayment.Visible
			= dvHideDeliveryDesignation.Visible
			= dvHideOrderCombine.Visible
			= ((this.HasItems == false) && (this.HasOwner == false));
		lOrderAndFixPurchaseHistoryCount.Text = "0";
		lCount.Text = "0";
	}

	/// <summary>
	/// ユーザー変更イベント
	/// </summary>
	private void UserChangeEvent()
	{
		ChangeAdvCodeEvent();

		// ポイントリセット
		if (this.IsUser == false)
		{
			tbPointUse.Text = "0";
			lUserPointUsable.Text = "0";
			hfUserPointUsable.Value = "0";
		}

		// ユーザークーポンリンクセット
		var userCouponCount = 0;
		if (this.IsUser
			&& (string.IsNullOrEmpty(hfUserId.Value) == false))
		{
			userCouponCount = GetCouponCount(
				this.LoginOperatorDeptId,
				this.CouponUserId,
				this.CouponMailAddress,
				true);
		}
		lUserCouponCount.Text = GetEncodedHtmlDisplayMessage(
			StringUtility.ToNumeric(userCouponCount));

		// 利用可能クーポンセット
		var usableCouponCount = GetCouponCount(
			this.LoginOperatorDeptId,
			this.CouponUserId,
			this.CouponMailAddress,
			false);
		lUsableCoupon.Text = GetEncodedHtmlDisplayMessage(usableCouponCount.ToString());
		ddlUserShipping.Items.Clear();
		var shopShipping = DomainFacade.Instance.ShopShippingService.Get(
			this.LoginOperatorShopId,
			hfShippingType.Value);
		this.UserShippingAddress = DomainFacade.Instance.UserShippingService.GetAllOrderByShippingNoDesc(hfUserId.Value).ToArray();
		BindShippingList(this.UserShippingAddress, shopShipping);
		rpAddressBook.DataSource = this.UserShippingAddress;
		rpAddressBook.DataBind();
		ddlUserShipping.SelectedIndex = 0;

		if (hfUserShipping.Value == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
		{
			ddlUserShipping.SelectedValue = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE;
		}

		dvCheckBoxSaveNewAddress.Visible = this.IsUser;
		dvUserShippingList.Visible = true;
		cbAlowSaveNewAddress.Checked = false;

		// 注文同梱時にクレカが登録されていないユーザーで、クレジット決済されている場合は、上書きされないようにスキップする
		var creditcardUnregistFlg = ddlUserCreditCard.Items.Cast<ListItem>().Any(item => item.Text == Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME);
		if (creditcardUnregistFlg == false)
		{
			// 登録可能な場合、ユーザカード種別取得（注文同梱の場合はカード枝番からも取得）
			if (this.IsUser)
			{
				this.CreditCardList =
					((this.ActionStatus == Constants.ACTION_STATUS_ORDERCOMBINE)
						&& (ddlUserCreditCard.SelectedValue != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
						? UserCreditCard.GetUsableOrByBranchno(hfUserId.Value, int.Parse(ddlUserCreditCard.SelectedValue))
						: UserCreditCard.GetUsable(hfUserId.Value);
			}
			else
			{
				this.CreditCardList = null;
			}

			// つくーる連携、EScottにて通常注文時再与信不可のカードが登録される為除く
			this.CreditCardList = this.CreditCardList
				.Where(ccl => (ccl.CooperationId.Split(',').Length != 2) || (ccl.CooperationId.Split(',')[0] != string.Empty)).ToArray();

			ddlUserCreditCard.DataBind();
			ddlUserCreditCard.Items.Add(
				new ListItem(
					ReplaceTag("@@DispText.credit_card_list.new@@"),
					CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW,
					(this.IsNotRakutenAgency || this.IsUserPayTg)));
			ddlUserCreditCard.SelectedIndex = 0;
			dvRegistCreditCard.Visible
				= dvCreditCardName.Visible
				= OrderCommon.GetCreditCardRegistable(this.IsUser, this.CreditCardList.Length);
			var errorMassage = "";
			if ((IsNotRakutenAgency == false)
				&& (this.CreditCardList.Any() == false)
					|| (this.CreditCardList == null))
			{
				errorMassage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RAKUTEN_CREDIT_NOT_EXIST_USER_CARD);
			}
			DisplayPaymentErrorMessage(errorMassage);

			DisplayCreditInputForm();
			if (this.IsUser == false)
			{
				cbRegistCreditCard.Checked = false;
			}
		}

		// Check show notice message if payment user level not use
		DisplayPaymentUserManagementLevel(
			this.LoginOperatorShopId,
			ddlUserManagementLevel.SelectedValue);

		DisplayPaymentOrderOwnerKbn(this.LoginOperatorShopId, ddlOwnerKbn.SelectedValue);

		// 配送先と注文者の住所を同じにするチェックボックスの制御
		ControlShippingSameAsOwnerCheckBox();

		// 制限されるユーザー管理レベルを表示
		DispFixedPurchaseLimitUserLevel(ddlUserManagementLevel.SelectedValue);

		if (OrderCommon.DisplayTwInvoiceInfo())
		{
			// Refresh Uniform Invoice Or Carry Type Option
			ddlUniformInvoiceOrCarryTypeOption.DataSource = GetUniformInvoiceOrCarryTypeOption(
				ddlUniformInvoiceType.SelectedValue,
				ddlInvoiceCarryType.SelectedValue);
			ddlUniformInvoiceOrCarryTypeOption.DataBind();
			ddlUniformInvoiceOrCarryTypeOption.SelectedValue = string.Empty;
			SetEmptyValueForInvoice();
			SetEnableTextBoxUniformInvoice();
		}

		// Set display or non-display for point and coupon
		SetDisplayForPointAndCoupon();

		// Set display or non-display for user shipping
		AddAttributesForControlDisplay(
			dvUserShipping,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
			(this.HasOwner == false)
				? CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_NONE
				: CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_EMPTY,
			true);
	}

	/// <summary>
	/// 広告コード変更イベント
	/// </summary>
	private void ChangeAdvCodeEvent()
	{
		// 会員でなければ会員ランクリセット
		if (this.IsUser == false)
		{
			hfMemberRankId.Value = string.Empty;
		}
		// 新規の会員登録であればデフォルト会員ランクセット
		else if (string.IsNullOrEmpty(hfUserId.Value))
		{
			string memberRankId;
			string userManagementLevelId;
			UserUtility.GetCorrectUserInfoByAdvCode(tbAdvCode.Text, out memberRankId, out userManagementLevelId);

			hfMemberRankId.Value = memberRankId ?? MemberRankOptionUtility.GetDefaultMemberRank();
			ddlUserManagementLevel.SelectedValue = userManagementLevelId ?? Constants.FLG_USER_USER_MANAGEMENT_LEVEL_NORMAL;
		}
	}

	/// <summary>
	/// 広告コードより補正情報（会員ランク、ユーザー管理レベル）を取得する
	/// </summary>
	/// <param name="advCode">広告コード</param>
	/// <param name="ownerKbn">オーナー区分</param>
	/// <returns>JSONデータ</returns>
	[WebMethod]
	public static string GetCorrectUserInfoByAdvCode(string advCode, string ownerKbn)
	{
		string memberRankId;
		string userManagementLevelId;
		if (UserService.IsUser(ownerKbn) == false)
		{
			memberRankId = "";
			userManagementLevelId = Constants.FLG_USER_USER_MANAGEMENT_LEVEL_NORMAL;
		}
		else
		{
			UserUtility.GetCorrectUserInfoByAdvCode(advCode, out memberRankId, out userManagementLevelId);
			memberRankId = memberRankId ?? MemberRankOptionUtility.GetDefaultMemberRank();
			userManagementLevelId = userManagementLevelId ?? Constants.FLG_USER_USER_MANAGEMENT_LEVEL_NORMAL;
		}

		var result = BasePageHelper.ConvertObjectToJsonString(
			new
			{
				MemberRankId = memberRankId,
				UserManagementLevelId = userManagementLevelId,
			});
		return result;
	}

	/// <summary>
	/// ユーザークーポン数取得
	/// </summary>
	/// <param name="deptId">識別ID</param>
	/// <param name="userId">ユーザーID</param>
	/// <param name="mailAddress">Mail Address</param>
	/// <param name="userCouponsOnly">ユーザーの持つクーポンのみか</param>
	/// <returns>利用可能クーポン数</returns>
	private int GetCouponCount(string deptId, string userId, string mailAddress, bool userCouponsOnly)
	{
		var condition = new CouponListSearchCondition
		{
			DeptId = deptId,
			UserId = userId,
			MailAddress = mailAddress,
			CouponCode = string.Empty,
			CouponName = string.Empty,
			UserCouponOnly = userCouponsOnly ? "1" : "0",
			UsedUserJudgeType = Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE
		};
		var result = DomainFacade.Instance.CouponService.GetAllUserUsableCouponsCount(condition);
		return result;
	}

	/// <summary>
	/// ポイント全適用ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnApplyUsablePointAll_Click(object sender, EventArgs e)
	{
		var orderItems = CreateOrderItemInput();

		// 商品入力チェック
		var errorMessage = CheckItemsAndPricesInput(orderItems);
		if (DisplayItemsErrorMessages(errorMessage))
		{
			btnAddProduct.Focus();
			this.MaintainScrollPositionOnPostBack = false;
			return;
		}

		// 利用可能ポイント取得
		var pointUsable = decimal.Parse(hfUserPointUsable.Value);

		// カート作成
		var cart = CreateCartExceptPointAndCoupon(orderItems);

		// クーポンチェック＆セット（エラーでも何もしない）
		if (Constants.W2MP_COUPON_OPTION_ENABLED)
		{
			CheckAndSetCouponUse(cart);
		}

		// 利用可能ポイント取得
		var pointUse = (cart.PointUsablePrice > pointUsable)
			? pointUsable
			: cart.PointUsablePrice;
		tbPointUse.Text = pointUse.ToPriceString();

		// カート作成＆更新
		RefreshItemsAndPriceView();
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		// 入力チェック
		var hasError = false;
		this.MaintainScrollPositionOnPostBack = false;

		// 決済情報入力チェック
		if ((this.IsOrderCombined == false)
			|| this.IsOrderCombinedAtOrderCombinePage)
		{
			var parameter = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, ddlOrderPaymentKbn.SelectedValue }
			};
			var errorMessage = Validator.Validate("OrderRegistInput", parameter);

			if (string.IsNullOrEmpty(errorMessage))
			{
				if ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (ddlUserCreditCard.SelectedValue == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
					&& this.CanUseCreditCardNoForm)
				{
					// 決済情報入力チェック
					var orderCreditCardInput = GetOrderCreditCardInputForOrderPage(
						ddlOrderPaymentKbn.SelectedValue,
						hfUserId.Value);
					if (this.IsNotRakutenAgency || this.IsUserPayTg)
					{
						errorMessage = orderCreditCardInput.Validate();
						hasError |= DisplayPaymentErrorMessage(errorMessage);
					}
					else
					{
						hasError = true;
						errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_RAKUTEN_CREDIT_NOT_EXIST_USER_CARD);
						DisplayPaymentErrorMessage(errorMessage);
					}

					// トークンが取得できていないときはエラーとして扱う(バグ#3554対策)
					if (OrderCommon.CreditTokenUse
						&& (orderCreditCardInput.CreditToken == null))
					{
						hasError = true;
						spanErrorMessageForCreditCard.InnerHtml =
							GetEncodedHtmlDisplayMessage(
								WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
						spanErrorMessageForCreditCard.Style["display"] = "block";
						ddlOrderPaymentKbn.Focus();
					}
				}
				else if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
				{
					this.AccountEmail = PayPalUtility.Account.GetCooperateAccountEmail(hfUserId.Value);
					if (string.IsNullOrEmpty(this.AccountEmail))
					{
						hasError = true;
						errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYPAL_NEEDS_COOPERATION_ERROR);
						ddlOrderPaymentKbn.Focus();
					}
				}
				// ペイジェントの場合は仮クレカ登録できないようにする
				else if ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					&& (ddlUserCreditCard.SelectedValue == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
					&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Paygent))
				{
					errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYGENT_CREDIT_NOT_EXIST_USER_CARD);
					hasError |= DisplayPaymentErrorMessage(errorMessage);
				}
			}

			if (dvHideOrderPayment.Visible && this.HasItems)
			{
				RefreshItemsAndPriceView();
				return;
			}
			hasError |= DisplayPaymentErrorMessage(errorMessage);
		}

		if (this.IsUserPayTg
			&& (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
			&& (hasError == false))
		{
			var errorMessage = (this.PayTgResponse == null)
				? string.Empty
				: (string)this.PayTgResponse[VeriTransConst.PAYTG_RESPONSE_ERROR] ?? string.Empty;
			hasError |= DisplayPaymentErrorMessage(errorMessage);
		}
		else if (this.IsUserPayTg
			&& (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.VeriTrans)
			&& (this.IsSuccessfulCardRegistration == false)
			&& (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			&& (ddlUserCreditCard.SelectedValue == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
			&& (hasError == false))
		{
			var errorMessage = (this.PayTgResponse == null)
				? string.Empty
				: (string)this.PayTgResponse[PayTgConstants.PAYTG_RESPONSE_ERROR] ?? string.Empty;
			hasError |= DisplayPaymentErrorMessage(errorMessage);
		}

		// 注文アイテム入力チェック
		var orderItems = CreateOrderItemInput();
		{
			var errorMessage = CheckItemsAndPricesInput(orderItems);
			if (orderItems.Any() == false)
			{
				errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_NOPRODUCT);
			}

			if (DisplayItemsErrorMessages(errorMessage))
			{
				tbOrderPriceRegulation.Focus();
				hasError = true;
			}
		}

		// 配送先情報入力チェック
		var shippingReceivingStoreFlg = string.Empty;
		if ((this.IsOrderCombined == false)
			|| this.IsOrderCombinedAtOrderCombinePage)
		{
			var shippingInput = GetShippingInput();
			shippingReceivingStoreFlg = (string)shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG];
			var shippingReceivingStoreType = (string)shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE];
			DisplayConvenienceStoreData();
			var errorMessage = string.Empty;
			var shippingInputForCheck = (Hashtable)shippingInput.Clone();
			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
				&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
				&& (shippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
				&& ECPayUtility.CheckShippingReceivingStoreType7Eleven(shippingReceivingStoreType))
			{
				shippingInputForCheck.Remove(CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_1);
				shippingInputForCheck.Remove(CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_2);
				shippingInputForCheck.Remove(CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_3);
				shippingInputForCheck.Remove(Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1);
			}
			errorMessage = CheckShippingInput(shippingInputForCheck);

			if ((this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_USER_INPUT)
				&& (ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE))
			{
				var convenienceId = StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID]);
				if ((OrderCommon.CheckIdExistsInXmlStoreBatchFixedPurchase(convenienceId) == false)
					&& (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED == false))
				{
					errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_CONVENIENCE_STORE_NOT_VALID);
				}
			}

			if (DisplayShippingErrorMessages(errorMessage))
			{
				tbShippingTel1_1.Focus();
				hasError = true;
			}
		}

		// 注文者情報入力チェック
		var ownerInput = GetOwnerInput();
		{
			var errorMessage = CheckOwnerInput(ownerInput);

			// ユーザー情報更新の場合はログインID重複チェック（UserInputを暫定的に利用）
			if (cbAllowSaveOwnerIntoUser.Checked
				&& Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED
				&& UserService.IsPcSiteOrOfflineUser(hfOwnerKbn.Value))
			{
				var userInputForDuplicationCheck = new UserInput
				{
					LoginId = (string)ownerInput[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR],
					UserId = hfUserId.Value,
				};
				var duplicationMessage = userInputForDuplicationCheck.CheckDuplicationLoginId(
					this.IsOwnerAddrJp
						? UserInput.EnumUserInputValidationKbn.UserModify
						: UserInput.EnumUserInputValidationKbn.UserModifyGlobal,
					true);
				if (string.IsNullOrEmpty(duplicationMessage) == false)
				{
					errorMessage += duplicationMessage;
				}
			}

			// Check owner tel no and owner name for EcPay
			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
				&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
				&& (shippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
			{
				var ownerTelNo1 = (string)ownerInput[Constants.FIELD_ORDEROWNER_OWNER_TEL1];
				var ownerTelNo = (ownerInput.ContainsKey(Constants.FIELD_ORDEROWNER_OWNER_TEL1)
						&& (string.IsNullOrEmpty(ownerTelNo1) == false))
					? ownerTelNo1
					: string.Format("{0}{1}{2}",
						ownerInput[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_1],
						ownerInput[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_2],
						ownerInput[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_3]);

				if (OrderCommon.CheckValidTelNoTaiwanForEcPay(ownerTelNo) == false)
				{
					errorMessage += CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_TEL_NO_NOT_TAIWAN);
				}

				var ownerName = (string)ownerInput[Constants.FIELD_ORDEROWNER_OWNER_NAME];
				if (OrderCommon.CheckOwnerNameForEcPay(ownerName) == false)
				{
					errorMessage += CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_OWNER_NAME_OF_CONVENIENCE_STORE_INVALID);
				}
			}

			if (DisplayOwnerErrorMessages(errorMessage))
			{
				ddlOrderKbn.Focus();
				hasError = true;
			}
		}

		// 後付款(TriLink後払い)を選択した場合、利用できるか判定する
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
			{
				var shippingInput = GetShippingInput();
				var ownerAddressCountryIsoCode =
					StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE]);
				var shippingAddressCountryIsoCode =
					StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE]);
				var errorMessage = CheckUsedPaymentOrderRegistForTriLinkAfterPay(
					ddlOrderPaymentKbn.SelectedValue,
					(this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER)
						? ownerAddressCountryIsoCode
						: shippingAddressCountryIsoCode,
					ownerAddressCountryIsoCode);
				hasError |= DisplayPaymentErrorMessage(errorMessage);
			}
			// Check Shipping Country Iso Code For Paidy Payment
			else if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			{
				var shippingInput = GetShippingInput();
				var ownerAddressCountryIsoCode = StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE]);
				var shippingAddressCountryIsoCode = StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE]);
				var shippingCountryIsoCode = (this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER)
					? ownerAddressCountryIsoCode
					: shippingAddressCountryIsoCode;
				if (IsCountryJp(shippingCountryIsoCode) == false)
				{
					var errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAIDY_COUNTRY_SHIPPING_NOT_JAPAN_ERROR);
					hasError |= DisplayPaymentErrorMessage(errorMessage);
				}
			}
			// Check Country Iso Code For NP After Pay
			else if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
			{
				var shippingInput = GetShippingInput();
				var ownerAddressCountryIsoCode = StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE]);
				var shippingAddressCountryIsoCode = StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE]);
				var shippingCountryIsoCode = (this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER)
					? ownerAddressCountryIsoCode
					: shippingAddressCountryIsoCode;
				if ((IsCountryJp(shippingCountryIsoCode) == false)
					|| (IsCountryJp(ownerAddressCountryIsoCode) == false))
				{
					var errorMessage = NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_3);
					hasError |= DisplayPaymentErrorMessage(errorMessage);
				}
			}
		}

		// 領収書希望の場合のみ、領収書情報入力チェックを行う
		if (Constants.RECEIPT_OPTION_ENABLED
			&& (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON))
		{
			var receiptInput = new Hashtable
			{
				{ Constants.FIELD_ORDER_RECEIPT_ADDRESS, tbReceiptAddress.Text },
				{ Constants.FIELD_ORDER_RECEIPT_PROVISO, tbReceiptProviso.Text },
			};
			hasError |= DisplayReceiptErrorMessage(Validator.Validate("OrderReceipt", receiptInput));
		}

		if (OrderCommon.DisplayTwInvoiceInfo()
			&& this.IsShippingAddrTw)
		{
			var parameter = new Hashtable();
			switch (ddlUniformInvoiceType.SelectedValue)
			{
				case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
					if (ddlInvoiceCarryType.Text == Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE)
					{
						parameter.Add(
							string.Format(
								"{0}_1",
								Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION),
							tbCarryTypeOption1.Text);
					}
					else if (ddlInvoiceCarryType.Text == Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE)
					{
						parameter.Add(
							string.Format(
								"{0}_2",
								Constants.FIELD_TWORDERINVOICE_TW_CARRY_TYPE_OPTION),
							tbCarryTypeOption2.Text);
					}

					if (cbCarryTypeOptionRegist.Checked)
					{
						parameter.Add(
							string.Format(
								"{0}_carry",
								Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NAME),
							tbCarryTypeOptionName.Text);
					}
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
					if (string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue))
					{
						parameter.Add(
							Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1,
							tbUniformInvoiceOption1_1.Text);
						parameter.Add(
							Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION2,
							tbUniformInvoiceOption1_2.Text);

						if (cbSaveToUserInvoice.Checked)
						{
							parameter.Add(
								string.Format(
									"{0}_uniform",
									Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NAME),
								tbUniformInvoiceTypeName.Text);
						}
					}
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
					if (string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue))
					{
						parameter.Add(
							string.Format(
								"{0}_donate",
								Constants.FIELD_TWORDERINVOICE_TW_UNIFORM_INVOICE_OPTION1),
							tbUniformInvoiceOption2.Text);

						if (cbSaveToUserInvoice.Checked)
						{
							parameter.Add(
								string.Format(
									"{0}_uniform",
									Constants.FIELD_TWUSERINVOICE_TW_INVOICE_NAME),
								tbUniformInvoiceTypeName.Text);
						}
					}
					break;
			}
			var errorMessage = Validator.Validate("OrderRegistInput", parameter);
			hasError |= DisplayUniformInvoiceErrorMessage(errorMessage);
		}

		// 注文拡張項目があれば入力チェックを行う
		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			var orderExtend = CreateOrderExtendFromInputData(rOrderExtendInput);
			var errorMessage = new OrderExtendInput(orderExtend).Validate();
			hasError |= DisplayOrderExtendErrorMessages(errorMessage);
		}

		// エラーがあれば抜ける
		if (hasError) return;

		// カート作成
		var cart = CreateCart(orderItems, ref hasError);

		if (Constants.NOVELTY_OPTION_ENABLED && (hasError == false))
		{
			// カートリスト作成
			var cartList = new CartObjectList(cart.CartUserId, cart.OrderKbn, false);
			cartList.AddCartVirtural(cart);

			// カートノベルティリスト作成
			var cartNoveltyList = new CartNoveltyList(cartList);

			// 付与条件外のカート内ノベルティを削除
			cartList.RemoveNoveltyGrantItem(cartNoveltyList);

			// カートに追加された付与アイテムを含むカートノベルティを削除
			cartNoveltyList.RemoveCartNovelty(cartList);

			// Add Product Grant Novelty
			var cartNoveltys = cartNoveltyList.GetCartNovelty(cart.CartId);
			AddProductGrantNovelty(cart, cartNoveltys);
		}
		// 配送先住所が配送不可エリアに指定されているか
		var shopShipping = new ShopShippingService();
		var unavailableShippingZip = shopShipping.GetUnavailableShippingZipFromShippingDelivery(cart.ShippingType, cart.Shippings[0].DeliveryCompanyId);
		var shippingZip = cart.Shippings[0].HyphenlessZip;

		var unavailableShipping = OrderCommon.CheckUnavailableShippingArea(unavailableShippingZip, shippingZip);
		if (unavailableShipping)
		{
			var unavailableShippingAreaErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_UNAVAILABLE_SHIPPING_AREA_ERROR);
			if (this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER)
			{
				if (DisplayOwnerErrorMessages(unavailableShippingAreaErrorMessage))
				{
					ddlOrderKbn.Focus();
					hasError = true;
				}
			}
			else
			{
				if (DisplayShippingErrorMessages(unavailableShippingAreaErrorMessage))
				{
					this.ddlShippingKbnList.Focus();
					hasError = true;
				}
			}
		}

		// 注文同梱ページで同梱を行っている場合、カートに同梱元の注文IDを設定(在庫数チェックに注文済み商品数量を考慮するため)
		if (this.IsOrderCombinedAtOrderCombinePage)
		{
			cart.OrderCombineParentOrderId = string.Format(
				"{0},{1}",
				this.CombineParentOrderId,
				string.Join(",", this.CombineChildOrderIds));
			var parentOrder = DomainFacade.Instance.OrderService.Get(this.CombineParentOrderId);
			cart.IsCombineParentOrderUseCoupon = (parentOrder.Coupons != null);
			cart.Shippings[0].ScheduledShippingDate = parentOrder.Shippings[0].ScheduledShippingDate;
			if (cart.Shippings[0].ShippingDate == null) cart.Shippings[0].ShippingDate = parentOrder.Shippings[0].ShippingDate;
			cart.IsOrderCombinedWithRegisteredSubscription = true;
			this.OriginOrderCombineCart = cart;
		}

		// 商品・金額系チェック
		var itemErrorMessages = CheckOrderItemsAndPrices(cart);
		if (DisplayItemsErrorMessages(itemErrorMessages))
		{
			this.Page.MaintainScrollPositionOnPostBack = false;
			btnReCalculate.Focus();
			return;
		}

		// 注文同梱親注文IDが設定されている場合、カート情報を注文同梱後のものに変える
		// 注文同梱ページでの同梱の場合、ページロードで注文同梱済みカートを作成しているため処理しない
		if ((string.IsNullOrEmpty(this.CombineParentOrderId) == false)
			&& (this.IsOrderCombinedAtOrderRegistPage))
		{
			var parentOrder = DomainFacade.Instance.OrderService.Get(this.CombineParentOrderId);
			CartObject combinedCart;
			lOrderCombineAlertMessage.Text = GetEncodedHtmlDisplayMessage(
				OrderCombineUtility.CreateCombinedCart(parentOrder, cart, out combinedCart));
			if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED == false)
			{
				combinedCart.GetShipping().DeliveryCompanyId = parentOrder.Shippings.First().DeliveryCompanyId;
				combinedCart.GetShipping().ShippingMethod = parentOrder.Shippings.First().ShippingMethod;
			}

			// 注文同梱カート作成でエラーがあった場合、処理中止
			if (string.IsNullOrEmpty(lOrderCombineAlertMessage.Text) == false) return;

			// 注文同梱前のカート情報をセット
			this.OrderCombineBeforeCart = cart;

			// カート情報を注文同梱後のものに変更
			cart = combinedCart;
		}

		// 注文金額チェック（決済手段の金額範囲に含まれているかどうか）
		hasError |= (CheckOrderPrice(cart) == false);

		// 注文同梱の場合、親注文の配送日時を使用するためスキップする
		if ((this.IsOrderCombined == false)
			|| (this.IsOrderCombinedAtOrderCombinePage == true))
		{
			// 配送日時チェック・更新
			hasError |= (CheckShippingDateTimeAndUpdateCartShipping(cart) == false);
		}
		// 配送伝票番号がnullの場合後続処理でエラーが発生するため、空文字をセットする
		else if (string.IsNullOrEmpty(cart.GetShipping().ShippingCheckNo))
		{
			cart.GetShipping().ShippingCheckNo = string.Empty;
		}

		// 注文同梱していない、もしくは注文同梱ありで親注文に定期購入がなく子注文に定期購入がある場合(定期購入設定の入力が必要)に定期購入設定チェック・更新を行う
		// 頒布会の注文同梱時は新規定期、新規頒布会の定期購入設定チェック・更新を行う
		if ((this.IsOrderCombined == false)
			|| (this.IsOrderCombined
				&& cart.IsRegisterFixedPurchaseWhenOrderCombine
				&& (this.IsOrderCombinedAtOrderCombinePage == false)))
		{
			// 定期購入設定チェック・更新
			hasError |= (CheckFixedPurchaseSettingAndUpdateCartShipping(cart) == false);
		}

		var cartShipping = cart.GetShipping();
		var shopShippingModel = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, cart.ShippingType);
		if (shopShippingModel.CanUseFixedPurchaseFirstShippingDateNextMonth(cartShipping.FixedPurchaseKbn))
		{
			cartShipping.ShippingDate = cartShipping.FirstShippingDate;
		}

		// 広告コードと広告名は有効だったら連係が追加される
		hasError |= CheckAdvCode();

		// エラーがあれば抜ける
		if (hasError) return;

		// Use LINE Pay Pre-Approved API
		cart.IsPreApprovedLinePayPayment = true;

		// LINE Pay
		if (Constants.PAYMENT_LINEPAY_OPTION_ENABLED
			&& (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY))
		{
			var regKey = GetLinePayRegKey(hfUserId.Value);

			// Display error
			if (string.IsNullOrEmpty(regKey))
			{
				DisplayPaymentErrorMessage(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_EXISTS_AUTO_KEY_PAYMENT));
				return;
			}
		}

		// ユーザ情報取得
		// ※入力チェックはしない
		var userInput = new Hashtable
		{
			{ Constants.FIELD_USER_USER_MEMO, StringUtility.RemoveUnavailableControlCode(tbUserMemo.Text) },
			{ Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID, ddlUserManagementLevel.SelectedValue },
			{ Constants.FIELD_USER_MAIL_FLG, rblMailFlg.SelectedValue },
		};

		// 再計算
		cart.CalculateWithCartShipping(this.IsOrderCombined);

		// 領収書情報セット
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			cart.ReceiptFlg = rblReceiptFlg.SelectedValue;
			cart.ReceiptAddress = StringUtility.RemoveUnavailableControlCode(tbReceiptAddress.Text);
			cart.ReceiptProviso = StringUtility.RemoveUnavailableControlCode(tbReceiptProviso.Text);
		}

		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			cart.OrderExtend = CreateOrderExtendFromInputData(rOrderExtendInput).ToDictionary(
				i => i.Key,
				i => new CartOrderExtendItem()
				{
					Value = i.Value
				});
		}

		// 注文情報作成
		var orderInput = new Hashtable
		{
			{ Constants.FIELD_ORDER_ORDER_KBN, ddlOrderKbn.SelectedValue },
			{ Constants.FIELD_ORDER_USER_ID, hfUserId.Value },
			{ Constants.FIELD_ORDER_PAYMENT_MEMO, StringUtility.RemoveUnavailableControlCode(tbPaymentMemo.Text) },
			{ Constants.FIELD_ORDER_MANAGEMENT_MEMO, StringUtility.RemoveUnavailableControlCode(tbManagerMemo.Text) },
			{ Constants.FIELD_ORDER_SHIPPING_MEMO, StringUtility.RemoveUnavailableControlCode(tbShippingMemo.Text) },
			{ Constants.FIELD_ORDER_RELATION_MEMO, StringUtility.RemoveUnavailableControlCode(tbRelationMemo.Text) },
			{ Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO,
				StringUtility.RemoveUnavailableControlCode(tbFixedPurchaseManagementMemo.Text) },
			{ Constants.FIELD_ORDER_ADVCODE_NEW, tbAdvCode.Text },
			{ Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE, ddlInflowContentsType.SelectedValue },
			{ Constants.FIELD_ORDER_INFLOW_CONTENTS_ID, tbInflowContentsId.Text },
			{ Constants.FIELD_ORDER_EXTERNAL_PAYMENT_TYPE, string.Empty },
			{ Constants.PAYMENT_GMO_CVS_TYPE, cart.Payment.GmoCvsType },
			{ Constants.PAYMENT_RAKUTEN_CVS_TYPE, cart.Payment.RakutenCvsType },
		};

		var advCodeInfo = DomainFacade.Instance.AdvCodeService.GetAdvCodeFromAdvertisementCode(tbAdvCode.Text);
		var mediaName = (string.IsNullOrEmpty(tbAdvCode.Text) == false)
				&& (advCodeInfo != null)
			? advCodeInfo.MediaName
			: string.Empty;

		orderInput[Constants.FIELD_ADVCODE_MEDIA_NAME] = mediaName;
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{
			orderInput[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID] = ddlSubscriptionBox.SelectedValue;
			orderInput[Constants.FIELD_SUBSCRIPTIONBOX_MANAGEMENT_NAME] = (ddlSubscriptionBox.SelectedItem != null)
				? ddlSubscriptionBox.SelectedItem.Text
				: null;
			orderInput[Constants.FIELD_ORDER_SUBSCRIPTION_BOX_FIXED_AMOUNT] = IsSubscriptionBoxFixedAmount()
				? (decimal?)SessionManager.Session["SubscriptionBoxFixedAmount"]
				: null;
			cart.SubscriptionBoxCourseId = ddlSubscriptionBox.SelectedValue;
			cart.SubscriptionBoxFixedAmount = IsSubscriptionBoxFixedAmount()
				? (decimal?)SessionManager.Session["SubscriptionBoxFixedAmount"]
				: null;
		}
		cart.ReflectMemoToFixedPurchase = cbReflectMemoToFixedPurchase.Checked;

		if (OrderCommon.DisplayTwInvoiceInfo() && this.IsShippingAddrTw)
		{
			cart.GetShipping().UniformInvoiceType = ddlUniformInvoiceType.SelectedValue;
			switch (ddlUniformInvoiceType.SelectedValue)
			{
				case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
					cart.GetShipping().CarryType = ddlInvoiceCarryType.SelectedValue;
					if (ddlInvoiceCarryType.SelectedValue == Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE)
					{
						cart.GetShipping().CarryTypeOptionValue = tbCarryTypeOption1.Text;
					}
					else if (ddlInvoiceCarryType.SelectedValue == Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE)
					{
						cart.GetShipping().CarryTypeOptionValue = tbCarryTypeOption2.Text;
					}
					cart.GetShipping().CarryTypeOption = ddlUniformInvoiceOrCarryTypeOption.SelectedValue;
					cart.GetShipping().UserInvoiceRegistFlg = cbCarryTypeOptionRegist.Checked;
					cart.GetShipping().InvoiceName = ((cbCarryTypeOptionRegist.Checked == false)
						? string.Empty
						: tbCarryTypeOptionName.Text);
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
					cart.GetShipping().UniformInvoiceOption1 = (string.IsNullOrEmpty(tbUniformInvoiceOption1_1.Text)
						? lUniformInvoiceOption1_1.Text
						: tbUniformInvoiceOption1_1.Text);
					cart.GetShipping().UniformInvoiceOption2 = (string.IsNullOrEmpty(tbUniformInvoiceOption1_2.Text)
						? lUniformInvoiceOption1_2.Text
						: tbUniformInvoiceOption1_2.Text);
					cart.GetShipping().UniformInvoiceTypeOption = ddlUniformInvoiceOrCarryTypeOption.SelectedValue;
					cart.GetShipping().UserInvoiceRegistFlg = cbSaveToUserInvoice.Checked;
					cart.GetShipping().InvoiceName = ((cbSaveToUserInvoice.Checked == false)
						? string.Empty
						: tbUniformInvoiceTypeName.Text);
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
					cart.GetShipping().UniformInvoiceOption1 = (string.IsNullOrEmpty(tbUniformInvoiceOption2.Text)
						? lUniformInvoiceOption2.Text
						: tbUniformInvoiceOption2.Text);
					cart.GetShipping().UniformInvoiceTypeOption = ddlUniformInvoiceOrCarryTypeOption.SelectedValue;
					cart.GetShipping().UserInvoiceRegistFlg = cbSaveToUserInvoice.Checked;
					cart.GetShipping().InvoiceName = ((cbSaveToUserInvoice.Checked == false)
						? string.Empty
						: tbUniformInvoiceTypeName.Text);
					break;
			}
		}

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			var regionModel = new RegionModel
			{
				CountryIsoCode = ddlAccessCountryIsoCode.SelectedValue,
				LanguageCode = hfDispLanguageCode.Value,
				LanguageLocaleId = ddlDispLanguageLocaleId.SelectedValue,
				CurrencyCode = hfDispCurrencyCode.Value,
				CurrencyLocaleId = ddlDispCurrencyLocaleId.SelectedValue
			};
			cart.Owner.UpdateRegion(regionModel);
		}

		if ((this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_USER_INPUT)
			&& (ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE))
		{
			cart.GetShipping().ConvenienceStoreId = lCvsShopNo.Text;
			if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
			{
				cart.GetShipping().ShippingReceivingStoreType = ddlShippingReceivingStoreType.SelectedValue;
			}
		}
		else if (cart.GetShipping().ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
		{
			cart.GetShipping().ConvenienceStoreId = string.Empty;
			cart.GetShipping().ShippingReceivingStoreType = string.Empty;
		}

		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
			&& OrderCommon.CheckValidWeightAndPriceForConvenienceStore(cart, cart.GetShipping().ShippingReceivingStoreType)
			&& (cart.Shippings.Any(shipping => (shipping.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))))
		{
			var convenienceStoreLimitKg = OrderCommon.GetConvenienceStoreLimitWeight(cart.GetShipping().ShippingReceivingStoreType);
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SHIPPING_CONVENIENCE_STORE)
				.Replace("@@ 1 @@", CurrencyManager.ToPrice(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE))
				.Replace("@@ 2 @@", convenienceStoreLimitKg.ToString());
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		if (this.IsUserPayTg)
		{
			cart.Payment.CreditToken = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans
				? CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(" " + this.VeriTransAccountId)
				: CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.CreditTokenbyPayTg);
			if ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
				&& (ddlUserCreditCard.SelectedValue == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
			{
				cart.Payment.CreditExpireMonth = ddlCreditExpireMonth.SelectedValue;
				cart.Payment.CreditExpireYear = ddlCreditExpireYear.SelectedValue;
				cart.Payment.CreditCardCompany = this.CreditCardCompanyCodebyPayTg;
			}
		}

		var orderDataInfos = new Hashtable
		{
			{ "order_input", orderInput },
			{ "user_input", userInput },
			{ "cart", cart },
			{ "update_user_flg", cbAllowSaveOwnerIntoUser.Checked },
			{ "is_order_combined", this.IsOrderCombined },
			{ "combine_parent_order_id", this.CombineParentOrderId },
			{ "combine_child_order_ids", this.CombineChildOrderIds },
			{ "combine_parent_tran_id", this.CombineParentTranId },
			{ "is_combine_order_sales_settled", this.IsCombineOrderSalesSettled },
			{ "order_combine_before_cart", this.OrderCombineBeforeCart },
			{ "is_order_combine_page", this.IsOrderCombinedAtOrderCombinePage },
			{ "ddlUserShipping_select_value", ddlUserShipping.SelectedValue },
			{ "use_order_combine", this.UseOrderCombine },
		};
		Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_REGIST_INPUT] = orderDataInfos;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_REGIST_CONFIRM);
	}

	/// <summary>
	/// クーポン「適用」ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnApplyCoupon_Click(object sender, EventArgs e)
	{
		// カート作成＆更新
		RefreshItemsAndPriceView();
	}

	/// <summary>
	/// ポイント「適用」ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnApplyPoint_Click(object sender, EventArgs e)
	{
		// カート作成＆更新
		RefreshItemsAndPriceView();
	}

	/// <summary>
	/// 次回購入の利用ポイントの全適用フラグチェックボックスクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnApplyUseAllPointFlg_Click(object sender, EventArgs e)
	{
		// カート作成＆更新
		RefreshItemsAndPriceView();
	}

	/// <summary>
	/// 再計算ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnReCalculate_Click(object sender, EventArgs e)
	{
		// カート作成＆更新
		RefreshItemsAndPriceView();
	}

	/// <summary>
	/// Button Refresh Payment Click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRefreshPayment_Click(object sender, EventArgs e)
	{
		lOrderShippingErrorMessages.Text = string.Empty;
		dvOrderShippingErrorMessages.Visible = false;
		var orderItems = CreateOrderItemInput();
		if (string.IsNullOrEmpty(CheckItemsAndPricesInput(orderItems)) == false) return;

		var hasError = false;
		var cart = CreateCart(orderItems, ref hasError);
		var shopShipping = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, cart.ShippingType);
		if (shopShipping == null) return;

		RefreshPaymentViewFromShippingType(cart, shopShipping);
		RefreshShippingCompanyViewFromShippingType(shopShipping);

		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
		{
			RefreshShippingDateViewFromShippingType(shopShipping, cart, true);
		}
	}

	/// <summary>
	/// 1種類の頒布会定額コースの商品のみが含まれるか
	/// </summary>
	/// <remarks>通常の定額頒布会 or 同コース同梱の定額頒布会であるかを確認</remarks>
	/// <returns>1種類の頒布会定額コースの商品のみが含まれるであればTRUE</returns>
	protected bool HaveOnlyOneSubscriptionBoxFixedAmountCourseItem()
	{
		// 登録画面内で同梱が行われた場合は同梱扱いにせずに判断する（同梱対象商品は「商品情報」には追加されないため）
		var result = (((this.IsOrderCombined == false)
					|| this.IsOrderCombinedAtOrderRegistPage)
				&& IsSubscriptionBoxFixedAmount())
			|| ((this.OrderCombineBeforeCart != null)
				&& this.OrderCombineBeforeCart.HaveOnlyOneSubscriptionBoxFixedAmountCourseItem());
		return result;
	}

	/// <summary>
	/// 頒布会定額コースか
	/// </summary>
	/// <returns>定額コースであればTrue</returns>
	protected bool IsSubscriptionBoxFixedAmount()
	{
		if (string.IsNullOrEmpty(this.SubscriptionBoxCourseId)) return false;
		var subscriptionBox = new SubscriptionBoxService().GetByCourseId(this.SubscriptionBoxCourseId);
		SessionManager.Session["SubscriptionBoxFixedAmount"] = subscriptionBox.FixedAmount;
		return (subscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE);
	}

	/// <summary>
	/// 頒布会コース選択ドロップダウン変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlSubscriptionBox_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		var courseId = ddlSubscriptionBox.SelectedValue;
		this.SubscriptionBoxCourseId = courseId;

		if (string.IsNullOrEmpty(courseId))
		{
			dvOrderItemNoticeMessage.Visible = false;
			return;
		}

		var selectedSubscriptionBox = DataCacheControllerFacade
			.GetSubscriptionBoxCacheController()
			.Get(courseId);

		this.MinNumberOfProducts = selectedSubscriptionBox.MinimumNumberOfProducts;
		this.MaxNumberOfProducts = selectedSubscriptionBox.MaximumNumberOfProducts;
		this.MinQuantity = selectedSubscriptionBox.MaximumPurchaseQuantity;
		this.MaxQuantity = selectedSubscriptionBox.MinimumPurchaseQuantity;
		this.SubscriptionManagementName = selectedSubscriptionBox.ManagementName;

		var defaultItems = new SubscriptionBoxDefaultItemModel[0];
		switch (selectedSubscriptionBox.OrderItemDeterminationType)
		{
			case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME:
				defaultItems = selectedSubscriptionBox
					.DefaultOrderProducts
						.Where(d => d.Count.HasValue && (d.Count.Value == 1))
						.ToArray();
				break;

			case Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_PERIOD:
				defaultItems = selectedSubscriptionBox
					.DefaultOrderProducts
						.Where(d => d.IsInTerm(DateTime.Now))
						.ToArray();
				break;
		}

		if (defaultItems.Any() == false) return;

		// 必須商品設定判定
		if (defaultItems.Any(defaultOrderProduct => defaultOrderProduct.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID))
		{
			defaultItems = defaultItems
				.Where(defaultOrderProduct => defaultOrderProduct.NecessaryProductFlg == Constants.FLG_SUBSCRIPTIONBOX_NECESSARY_VALID).ToArray();
		}

		var orderItems = new List<Hashtable>();

		foreach (var item in defaultItems)
		{
			var dvProductVariation = GetProductVariation(
				item.ShopId,
				item.ProductId,
				item.VariationId,
				hfMemberRankId.Value);

			if ((dvProductVariation.Count == 0)
				|| ((string)dvProductVariation[0][Constants.FIELD_PRODUCT_VALID_FLG]
					== Constants.FLG_PRODUCT_VALID_FLG_INVALID)) continue;

			// 頒布会キャンペーン期間かどうか
			var shippingSubscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
				x => (x.ProductId == item.ProductId) && (x.VariationId == item.VariationId));
			var isCampaignPeriod = OrderCommon.IsSubscriptionBoxCampaignPeriod(shippingSubscriptionBoxItem);

			var orderItem = new Hashtable
			{
				{ "fixedpurchase", true },
				{ Constants.FIELD_ORDERITEM_SHOP_ID, item.ShopId },
				{ Constants.FIELD_ORDERITEM_PRODUCT_ID, item.ProductId },
				{ Constants.FIELD_ORDERITEM_SUPPLIER_ID, (string)dvProductVariation[0][Constants.FIELD_PRODUCT_SUPPLIER_ID] },
				{ Constants.FIELD_ORDERITEM_VARIATION_ID, item.VariationId },
				{ Constants.FIELD_PRODUCTVARIATION_V_ID, item.VariationId.Substring(item.ProductId.Length) },
				{ Constants.FIELD_ORDERITEM_PRODUCT_NAME, dvProductVariation[0][Constants.FIELD_PRODUCT_NAME] + CreateVariationName(dvProductVariation[0]) },
				{ Constants.FIELD_ORDERITEM_PRODUCT_PRICE, isCampaignPeriod
					? shippingSubscriptionBoxItem.CampaignPrice
					: GetFixedPurchaseProductValidityPrice(dvProductVariation[0], true) },
				{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, item.ItemQuantity },
				{ Constants.FIELD_ORDERITEM_PRODUCTSALE_ID, dvProductVariation[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID] },
				{ Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, string.Empty },
				{ Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME, string.Empty },
				{ Constants.FIELD_ORDERITEM_NOVELTY_ID, string.Empty },
				{ Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG, true },
				{ Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING, string.Empty },
				{ Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING, string.Empty },
				{ Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE, string.Empty },
				{ Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_TYPE, string.Empty },
				{ Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE, string.Empty },
				{ Constants.FIELD_ORDERITEM_ITEM_PRICE, (isCampaignPeriod
					? shippingSubscriptionBoxItem.CampaignPrice
					: (GetFixedPurchaseProductValidityPrice(
						dvProductVariation[0], true)) * item.ItemQuantity).ToString() },
				{ "product_option_value_settings", new ProductOptionSettingList(this.LoginOperatorShopId, item.ProductId) },
				{ Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID, courseId },
				{ Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT, selectedSubscriptionBox.FixedAmount },
			};

			dvOrderItemErrorMessage.Visible = false;

			orderItems.Add(orderItem);
		}

		if (orderItems.Any() == false) return;

		// 制限されるユーザー管理レベルを表示
		DispFixedPurchaseLimitUserLevel(ddlUserManagementLevel.SelectedValue);

		// 取得した値を画面へセット
		BindDataOrderItem(orderItems);

		// 全ポイント継続利用OPがONの場合はチェックボックスを表示
		if (this.CanUseAllPointFlg)
		{
			cbUseAllPointFlg.Visible = true;
		}
		else
		{
			cbUseAllPointFlg.Visible = false;
			cbUseAllPointFlg.Checked = false;
		}

		// カート作成＆更新
		RefreshItemsAndPriceView();
	}

	/// <summary>
	/// 注文商品追加ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnAddProduct_Click(object sender, EventArgs e)
	{
		CreateOrderItemRows(CanAddNewOrderItemRow());
		DisplayItemsErrorMessages(string.Empty);
	}

	/// <summary>
	/// Open Convenience Store Map Ec Pay Click Event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOpenConvenienceStoreMapEcPay_Click(object sender, EventArgs e)
	{
		var shippingReceivingStoreType = ddlShippingReceivingStoreType.SelectedValue;
		if (string.IsNullOrEmpty(shippingReceivingStoreType)) return;

		var script = ECPayUtility.CreateScriptForOpenConvenienceStoreMap(shippingReceivingStoreType);
		ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "OpenConvenienceStoreMap", script, true);
	}

	/// <summary>
	/// 決済種別選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlOrderPaymentKbn_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.IsUpdatePayment = true;
		// 再計算
		RefreshItemsAndPriceView();

		// 表示切り替え
		dvPaymentKbnCredit.Visible =
			(ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
		dvPaymentErrorMessages.Visible = false;

		// 決済メッセージ作成
		lOrderPaymentInfo.Text = string.Empty;
		if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
		{
			this.AccountEmail = PayPalUtility.Account.GetCooperateAccountEmail(hfUserId.Value);
			lOrderPaymentInfo.Text = GetEncodedHtmlDisplayMessage(
				string.IsNullOrEmpty(this.AccountEmail)
					? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYPAL_ISNOT_LINK_ACCOUNT)
					: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYPAL_AVAILABLE_ACCOUNT)
						.Replace("@@ 1 @@", this.AccountEmail));
		}

		// Get gmo cvs type
		GetGmoCvsType();

		// Get rakuten cvs type
		GetRakutenCvsType();
		this.IsUpdatePayment = false;
	}

	/// <summary>
	/// Get gmo cvs type
	/// </summary>
	private void GetGmoCvsType()
	{
		var isGmoCsvType = ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
				&& (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Gmo));
		dvGmoCvsType.Visible = isGmoCsvType;
		if (isGmoCsvType
			&& (this.IsBackFromConfirm == false))
		{
			ddlGmoCvsType.Items.Clear();
			ddlGmoCvsType.Items.AddRange(
				ValueText.GetValueItemArray(
					Constants.TABLE_PAYMENT,
					Constants.PAYMENT_GMO_CVS_TYPE));
		}
	}

	/// <summary>
	/// Event User management level dropdown list selected change
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserManagementLevel_SelectedIndexChanged(object sender, EventArgs e)
	{
		DisplayPaymentUserManagementLevel(
			this.LoginOperatorShopId,
			ddlUserManagementLevel.SelectedValue);

		// 制限されるユーザー管理レベルを表示
		DispFixedPurchaseLimitUserLevel(ddlUserManagementLevel.SelectedValue);
	}

	/// <summary>
	/// 注文商品リピータイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	public void rItemList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if (e.CommandName != "delete") return;

		// 行削除
		var orderItemInfos = CreateOrderItemInput();
		var target = orderItemInfos[int.Parse(e.CommandArgument.ToString())];
		if (Constants.NOVELTY_OPTION_ENABLED
			&& (string.IsNullOrEmpty(StringUtility.ToEmpty(target[Constants.FIELD_ORDERITEM_NOVELTY_ID])) == false))
		{
			this.DeletedNoveltyIds.Add(StringUtility.ToEmpty(target[Constants.FIELD_ORDERITEM_NOVELTY_ID]));
		}

		if ((this.OriginOrderCombineCart != null)
			&& (this.OriginOrderCombineCart.Items.Count > e.Item.ItemIndex))
		{
			this.OriginOrderCombineCart.Items.RemoveAt(e.Item.ItemIndex);
		}
		var itemQuantity = 0;
		target[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] =
			int.TryParse((string)target[Constants.FIELD_ORDERITEM_ITEM_QUANTITY], out itemQuantity)
				? itemQuantity
				: 1;
		orderItemInfos.Remove(target);
		BindDataOrderItem(orderItemInfos);

		// 制限されるユーザー管理レベルを表示
		DispFixedPurchaseLimitUserLevel(ddlUserManagementLevel.SelectedValue);
		var fixedPurchaseCount = orderItemInfos
			.Count(item => (bool)item[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG]);
		if (((bool)target[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG] == false)
			&& (orderItemInfos.Count == fixedPurchaseCount))
		{
			hfHasFixedPurchase.Value = true.ToString();
		}

		// 頒布会数量系のチェック
		CheckSubscriptionBoxQuantity(orderItemInfos);
		// 再計算
		btnReCalculate_Click(null, e);

		// Change the fixed purchase flag to false
		hfHasFixedPurchase.Value = false.ToString();
	}

	/// <summary>
	/// 商品付帯情報設定ボタン（隠しボタン）クリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnProductOptionValueSetting_OnClick(object sender, EventArgs e)
	{
		foreach (RepeaterItem riItem in rItemList.Items)
		{
			// 変更があった商品以外は除外
			if (hfProductItemSelectIndex.Value != riItem.ItemIndex.ToString()) continue;

			var hfProductVariationId = (HiddenField)riItem.FindControl("hfProductVariationId");

			// 定期チェック有無に応じた商品価格に更新
			var dvProductVariation = GetProductVariation(
				this.LoginOperatorShopId,
				((TextBox)riItem.FindControl("tbProductId")).Text,
				hfProductVariationId.Value,
				hfMemberRankId.Value);
			var hasProductInfo = (dvProductVariation.Count > 0);
			var ddlVariationIdList = (DropDownList)riItem.FindControl("ddlVariationIdList");
			var productVariations = DomainFacade.Instance.ProductService.GetProductVariationsByProductId(
				((TextBox)riItem.FindControl("tbProductId")).Text.Trim());
			ddlVariationIdList.DataSource = productVariations
				.Select(item =>
					new ListItem(item.VariationId.Replace(item.ProductId, string.Empty), item.VariationId));
			ddlVariationIdList.DataBind();
			if (ddlVariationIdList.Items.FindByValue(hfProductVariationId.Value) != null)
			{
				ddlVariationIdList.SelectedValue = hfProductVariationId.Value;
			}

			var existSubscriptionBoxCourseId = (string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false)
				? new SubscriptionBoxService()
					.GetByCourseId(this.SubscriptionBoxCourseId).FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE
				: false;

			// 定額の場合は画面には表示しないが、商品価格を入れる
			if ((((CheckBox)riItem.FindControl("cbFixedPurchase")).Checked
				|| existSubscriptionBoxCourseId)
				&& hasProductInfo)
			{
				((TextBox)riItem.FindControl("tbProductPrice")).Text =
					GetFixedPurchaseProductValidityPrice(dvProductVariation[0], true).ToPriceString();
			}

			((HiddenField)riItem.FindControl("hfProductImageHead")).Value = hasProductInfo
				? string.IsNullOrEmpty(StringUtility.ToEmpty(dvProductVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD]))
					? StringUtility.ToEmpty(dvProductVariation[0][Constants.FIELD_PRODUCT_IMAGE_HEAD])
					: StringUtility.ToEmpty(dvProductVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD])
				: string.Empty;
		}

		// 再計算処理
		btnReCalculate_Click(sender, e);
	}

	/// <summary>
	/// 定期購入指定変更処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbFixedPurchase_CheckedChanged(object sender, EventArgs e)
	{
		// 対象列取得
		var riTarget = ((RepeaterItem)((CheckBox)sender).Parent);

		// チェック状態取得
		var isFixedPurchaseValid = ((CheckBox)sender).Checked;

		// 商品情報取得
		var productId = ((TextBox)riTarget.FindControl("tbProductId")).Text;
		var variationId = StringUtility.ToEmpty(((DropDownList)riTarget.FindControl("ddlVariationIdList")).SelectedValue);
		var productVariationInfo = GetProductVariation(
			this.LoginOperatorShopId,
			productId,
			string.IsNullOrEmpty(variationId) ? productId : variationId,
			hfMemberRankId.Value);

		if (productVariationInfo.Count != 0)
		{
			// 商品が定期購入無効なら無効に
			isFixedPurchaseValid = isFixedPurchaseValid
				&& (StringUtility.ToEmpty(productVariationInfo[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG])
					!= Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID);
		}
		else
		{
			isFixedPurchaseValid = false;
		}

		// 商品名に定期商品接頭辞つける
		var productNameTextBox = ((TextBox)riTarget.FindControl("tbProductName"));
		productNameTextBox.Text = CreateFixedPurchaseProductName(productNameTextBox.Text, isFixedPurchaseValid, this.IsSubscriptionBoxValid);
		if (productVariationInfo.Count > 0)
		{
			// 定期チェック有無に応じた商品価格に更新(商品の定期購入可否設定とチェック有無が不整合の場合は更新しない)
			var productFixedPurchaseFlg =
				StringUtility.ToEmpty(productVariationInfo[0][Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]);
			var productPriceTextBox = (TextBox)riTarget.FindControl("tbProductPrice");

			if (isFixedPurchaseValid
				&& (productFixedPurchaseFlg != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID))
			{
				productPriceTextBox.Text =
					GetFixedPurchaseProductValidityPrice(productVariationInfo[0], true).ToPriceString();
			}
			else if ((isFixedPurchaseValid == false)
				&& (productFixedPurchaseFlg != Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY))
			{
				productPriceTextBox.Text = GetProductValidityPrice(productVariationInfo[0]).ToPriceString();
			}
		}

		// 再計算処理時に配送種別に紐付く表示の更新を行うため、
		// 定期購入指定変更フラグをTrueを設定
		hfHasFixedPurchase.Value = true.ToString();

		// 再計算処理
		btnReCalculate_Click(sender, e);

		// 定期購入指定変更フラグをFalseを設定
		hfHasFixedPurchase.Value = false.ToString();

		// Determine cart has any fixed purchase product
		var hasFixedPurchaseProduct = false;
		foreach (RepeaterItem item in rItemList.Items)
		{
			var cbFixedPurchase = (CheckBox)item.FindControl("cbFixedPurchase");
			if (cbFixedPurchase.Checked)
			{
				hasFixedPurchaseProduct = true;
				break;
			}
			if (Constants.DISPLAY_DEFAULTSHIPPINGDATE_ENABLED
				&& (ddlShippingDate.Items.Count != 0)
				&& (ddlShippingDate.Items[0].Value != string.Empty))
			{
				// 定期商品から通常商品に変更時、先頭に初回配送予定日が入ったままになるため削除
				ddlShippingDate.Items.RemoveAt(0);
				ddlShippingDate.Visible = true;
			}
			if ((Constants.DISPLAY_DEFAULTSHIPPINGDATE_ENABLED == false)
				&& (ddlShippingDate.Items.Count >= 2))
			{
				var shippingDateFirstItem = DateTime.Parse(ddlShippingDate.Items[0].Value);
				var shippingDateSecondItem = DateTime.Parse(ddlShippingDate.Items[1].Value);

				if (shippingDateSecondItem <= shippingDateFirstItem)
				{
					ddlShippingDate.Items.RemoveAt(0);
					ddlShippingDate.Visible = true;
				}
			}

			// 制限されるユーザー管理レベルを表示
			DispFixedPurchaseLimitUserLevel(ddlUserManagementLevel.SelectedValue);
		}

		// Display 初回注文のみ反映
		if (hasFixedPurchaseProduct)
		{
			cbReflectMemoToFixedPurchase.Visible = true;
			cbUseAllPointFlg.Visible = true;
		}
		else
		{
			cbReflectMemoToFixedPurchase.Visible = false;
			cbReflectMemoToFixedPurchase.Checked = false;
			cbUseAllPointFlg.Visible = false;
			cbUseAllPointFlg.Checked = false;
		}
	}

	/// <summary>
	/// 制限されるユーザー管理レベルを表示
	/// </summary>
	/// <param name="userLevel">ユーザ管理レベル</param>
	protected void DispFixedPurchaseLimitUserLevel(string userLevel = null)
	{
		var dictionary = new Dictionary<string, string>();
		foreach (RepeaterItem item in rItemList.Items)
		{
			var productId = ((TextBox)item.FindControl("tbProductId")).Text;
			var limitedUserLevel = CheckFixedPurchaseLimitedUserLevel(this.LoginOperatorShopId, productId, userLevel);
			if (((CheckBox)item.FindControl("cbFixedPurchase")).Checked)
			{
				if (limitedUserLevel == false) continue;
				var message = WebMessages
					.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_FIXED_PURCHASE_USER_MANAGERMENT_LEVEL_NOTIFY)
					.Replace("@@ 1 @@", productId);
				if (dictionary.ContainsKey(productId)) continue;

				dictionary.Add(productId, message);
			}
			else
			{
				dictionary.Remove(productId);
			}
		}
		lWarningMessage.Text = GetEncodedHtmlDisplayMessage(string.Join(Environment.NewLine, dictionary.Values));
		dvWarningMessage.Visible = (dictionary.Values.Count != 0);
	}

	/// <summary>
	/// 注文者住所検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOwnerZipSearch_Click(object sender, EventArgs e)
	{
		var zipcodeUtil = new ZipcodeSearchUtility(tbOwnerZip1.Text + tbOwnerZip2.Text);
		if (zipcodeUtil.Success)
		{
			foreach (ListItem li in ddlOwnerAddr1.Items)
			{
				li.Selected = (li.Value == zipcodeUtil.PrefectureName);
			}
			tbOwnerAddr2.Text = zipcodeUtil.CityName + zipcodeUtil.TownName;
		}

		// 配送先を注文者と同じにする場合は初回配送日などを再計算
		if (this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER) ddlIntervalDays_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送先住所検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnShippingZipSearch_Click(object sender, EventArgs e)
	{
		var zipcodeUtil = new ZipcodeSearchUtility(tbShippingZip1.Text + tbShippingZip2.Text);
		if (zipcodeUtil.Success)
		{
			foreach (ListItem listItem in ddlShippingAddr1.Items)
			{
				listItem.Selected = (listItem.Value == zipcodeUtil.PrefectureName);
			}
			tbShippingAddr2.Text = zipcodeUtil.CityName + zipcodeUtil.TownName;
		}

		// 配送先を注文者と同じにするではない場合は初回配送日などを再計算
		if (this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_USER_INPUT) ddlIntervalDays_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// Dropdown list shipping kbn list changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingKbnList_Changed(object sender, EventArgs e)
	{
		ddlIntervalDays_OnSelectedIndexChanged(sender, e);
		btnReCalculate_Click(sender, e);
		dvOrderShippingErrorMessages.Visible = false;
		lOrderShippingErrorMessages.Text = string.Empty;
	}
	#endregion

	#region 注文系オブジェクト作成処理
	/// <summary>
	/// カート作成
	/// </summary>
	/// <param name="orderItems">注文アイテム</param>
	/// <param name="hasError">エラーがあるか</param>
	/// <returns>カート</returns>
	private CartObject CreateCart(List<Hashtable> orderItems, ref bool hasError)
	{
		// カート作成
		var cart = CreateCartExceptPointAndCoupon(orderItems);

		// クーポンチェック＆セット
		if (Constants.W2MP_COUPON_OPTION_ENABLED)
		{
			var errorMessage = CheckAndSetCouponUse(cart);
			hasError |= DisplayCouponErrorMessage(errorMessage) && (cart.IsCouponNotApplicableByOrderCombined == false);
		}

		// ポイント入力チェック
		{
			var errorMessage = CheckPointUseInput();
			hasError |= DisplayPointErrorMessages(errorMessage);

			if (string.IsNullOrEmpty(errorMessage) == false) return cart;
		}

		// ポイントチェック＆セット
		if (Constants.W2MP_POINT_OPTION_ENABLED && this.IsUser)
		{
			var errorMessage = CheckAndSetPointUse(cart, decimal.Parse(tbPointUse.Text.Trim()));
			hasError |= DisplayPointErrorMessages(errorMessage);
		}
		return cart;
	}

	/// <summary>
	/// カート作成（ポイント／クーポン以外）
	/// </summary>
	/// <param name="orderItems">注文アイテム入力情報</param>
	/// <returns>カート</returns>
	private CartObject CreateCartExceptPointAndCoupon(List<Hashtable> orderItems)
	{
		CartObject cart = null;
		var languageCode = string.Empty;
		var languageLocaleId = string.Empty;
		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			languageCode = hfDispLanguageCode.Value;
			languageLocaleId = ddlDispLanguageLocaleId.SelectedValue;
		}

		// カート商品作成
		foreach (var item in orderItems)
		{
			var product = GetProductVariation(
				StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_SHOP_ID]),
				StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_ID]),
				StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_VARIATION_ID]));
			if (product.Count == 0) continue;

			var cartProduct =
				new CartProduct(
					product[0],
					GetCartAddKbn(
						(bool)item[CONST_KEY_FIXED_PURCHASE],
						string.IsNullOrEmpty((string)item[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID]) == false),
					StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID]),
					int.Parse(StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_ITEM_QUANTITY])),
					false);
			cartProduct.SetPrice(decimal.Parse(StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_PRICE])));
			cartProduct.ProductOptionSettingList = (ProductOptionSettingList)item[CONST_KEY_PRODUCT_OPTION_VALUE_SETTINGS];
			cartProduct.NoveltyId = StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_NOVELTY_ID]);
			cartProduct.FixedPurchaseDiscountValue = (string.IsNullOrEmpty((string)item[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE]) == false)
				? decimal.Parse((string)item[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE])
				: (decimal?)null;
			cartProduct.FixedPurchaseDiscountType = (string)item[Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_TYPE];
			cartProduct.LanguageCode = languageCode;
			cartProduct.LanguageLocaleId = languageLocaleId;
			cartProduct.SubscriptionBoxCourseId = (string)item[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID];
			cartProduct.SetPriceForSubscriptionBox();

			bool isOrderCombineProduct;
			bool.TryParse((string)item[CONST_HASHKEY_CART_PRODUCT_IS_ORDER_COMBINED], out isOrderCombineProduct);
			cartProduct.IsOrderCombine = isOrderCombineProduct; 

			if (cart == null)
			{
				cart = new CartObject(
					hfUserId.Value,
					ddlOrderKbn.SelectedValue,
					this.LoginOperatorShopId,
					StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCT_SHIPPING_TYPE]),
					cartProduct.IsDigitalContents,
					false);
				if (string.IsNullOrEmpty(cart.MemberRankId))
				{
					cart.MemberRankId = this.IsUser
						? hfMemberRankId.Value
						: string.Empty;
				}
			}

			// カート内の同一商品を取得
			var sameItem = cart.GetSameProduct(cartProduct);

			if (sameItem == null)
			{
				// 同一商品がなければ追加
				cart.Items.Add(cartProduct);
			}
			else
			{
				// 同一商品があれば数量を加算
				sameItem.SetProductCount(cart.CartId, sameItem.CountSingle + cartProduct.CountSingle);
			}
		}
		if (cart == null)
		{
			cart = new CartObject(
				hfUserId.Value,
				ddlOrderKbn.SelectedValue,
				this.LoginOperatorShopId,
				string.Empty,
				false,
				false);
		}

		// 注文同梱画面からの遷移の場合、注文同梱元親注文の定期購入有無、定期購入回数を引き継ぐ(定期購入割引計算用)
		if (this.IsOrderCombinedAtOrderCombinePage)
		{
			// 注文同梱ページでの同梱の場合、注文同梱元注文IDをセット
			cart.OrderCombineParentOrderId = string.Format("{0},{1}",
				this.CombineParentOrderId,
				string.Join(",", this.CombineChildOrderIds));

			if (this.CombineParentOrderHasFixedPurchase)
			{
				cart.IsCombineParentOrderHasFixedPurchase = this.CombineParentOrderHasFixedPurchase;
				cart.CombineParentOrderFixedPurchaseOrderCount = this.CombineParentOrderCount;
				cart.FixedPurchase = this.CombineParentOrderFixedPurchase;
				cart.FixedPurchaseDiscount = this.CombineFixedPurchaseDiscountPrice;
			}
		}

		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (ddlSubscriptionBox.SelectedValue != string.Empty))
		{
			cart.SubscriptionBoxCourseId = ddlSubscriptionBox.SelectedValue;
		}

		if (IsSubscriptionBoxFixedAmount())
		{
			cart.SubscriptionBoxFixedAmount = (decimal?)SessionManager.Session["SubscriptionBoxFixedAmount"];
		}

		// 配送先計算のために注文者・配送先セット
		this.CartOwner = cart.Owner = CreateCartOwner(GetOwnerInput());
		cart.SetShippingAddressAndShippingDateTime(CreateCartShipping(GetShippingInput(), cart));
		cart.IsFixedPurchaseMember = (hfFixedPurchaseMember.Value == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON);
		cart.UpdateAnotherShippingFlag();
		cart.GetShipping().ShippingMethod = ddlShippingMethod.SelectedValue;

		// 配送サービスIDを設定
		var selectedDeliveryCompanyId = ddlDeliveryCompany.SelectedValue;
		if (string.IsNullOrEmpty(ddlDeliveryCompany.SelectedValue))
		{
			// 配送サービスは未選択の際、デフォルトの配送サービスIDを取得
			var shopShippingInfo = DomainFacade.Instance.ShopShippingService.Get(cart.ShopId, cart.ShippingType);
			if (shopShippingInfo != null)
			{
				selectedDeliveryCompanyId = (cart.GetShipping().IsExpress
					? shopShippingInfo.CompanyListExpress
					: shopShippingInfo.CompanyListMail).First(model => model.IsDefault).DeliveryCompanyId;
			}
		}
		cart.GetShipping().DeliveryCompanyId = selectedDeliveryCompanyId;

		// 配送料金セットして再計算
		if ((string.IsNullOrEmpty(cart.GetShipping().Addr1) == false)
			|| (cart.GetShipping().Zip != "-"))
		{
			cart.CalculateWithCartShipping();
		}
		else
		{
			cart.CalculateWithDefaultShipping();
		}

		// 調整金額セット
		decimal orderPriceRegulation;
		if (decimal.TryParse(tbOrderPriceRegulation.Text.Trim(), out orderPriceRegulation))
		{
			cart.PriceRegulation = orderPriceRegulation;
		}
		cart.RegulationMemo = StringUtility.RemoveUnavailableControlCode(tbRegulationMemo.Text);

		// 決済手数料セット
		cart.Payment = GetCartPayment(cart);

		// Gmo Cvs Type
		if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
			&& (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Gmo))
		{
			cart.Payment.GmoCvsType = StringUtility.ToEmpty(ddlGmoCvsType.SelectedValue);
		}

		// Rakuten cvs type
		if ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
			&& (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten))
		{
			cart.Payment.RakutenCvsType = StringUtility.ToEmpty(ddlRakutenCvsType.SelectedValue);
		}

		// 決済手数料をセットしたカート配送先情報で再計算
		cart.Calculate(false, isPaymentChanged: true);

		// 注文メモセット
		if (this.IsOrderCombinedAtOrderCombinePage)
		{
			cart.OrderMemos = CartObject.CreateCartMemoFromOrderMemo(
				Constants.FLG_ORDER_MEMO_SETTING_DISP_FLG_PC,
				StringUtility.RemoveUnavailableControlCode(tbMemoForCombine.Text),
				cart.Owner);
		}
		else
		{
			// PCで作成
			cart.CreateOrderMemo(Constants.FLG_ORDER_MEMO_SETTING_DISP_FLG_PC);
			var itemIndex = 0;
			foreach (RepeaterItem item in rOrderMemos.Items)
			{
				cart.OrderMemos[itemIndex].InputText =
					StringUtility.RemoveUnavailableControlCode(((TextBox)item.FindControl("tbMemo")).Text);
				itemIndex++;
			}
		}

		// コンバージョン情報をセット
		cart.ContentsLog = new ContentsLogModel
		{
			ContentsType = ddlInflowContentsType.SelectedValue,
			ContentsId = tbInflowContentsId.Text.Trim()
		};
		cart.Payment.PaidyToken = hfPaidyTokenId.Value;
		return cart;
	}

	/// <summary>
	/// 注文者入力情報取得
	/// </summary>
	/// <returns>入力情報</returns>
	private Hashtable GetOwnerInput()
	{
		var ownerName1 =
			DataInputUtility.ConvertToFullWidthBySetting(tbOwnerName1.Text.Trim(), this.IsOwnerAddrJp);
		var ownerName2 =
			DataInputUtility.ConvertToFullWidthBySetting(tbOwnerName2.Text.Trim(), this.IsOwnerAddrJp);
		var ownerNameKana1 = StringUtility.ToZenkaku(tbOwnerNameKana1.Text.Trim());
		var ownerNameKana2 = StringUtility.ToZenkaku(tbOwnerNameKana2.Text.Trim());
		var ownerInput = new Hashtable
		{
			{ Constants.FIELD_ORDEROWNER_OWNER_KBN, hfOwnerKbn.Value },
			{ Constants.FIELD_ORDEROWNER_OWNER_NAME1, ownerName1 },
			{ Constants.FIELD_ORDEROWNER_OWNER_NAME2, ownerName2 },
			{ Constants.FIELD_ORDEROWNER_OWNER_NAME, ownerName1 + ownerName2 },
			{ Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1, ownerNameKana1 },
			{ Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2, ownerNameKana2 },
			{ Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA, ownerNameKana1 + ownerNameKana2 },
			{ Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR,
				StringUtility.ToHankaku(tbOwnerMailAddr.Text.Trim()) },
			{ Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2,
				StringUtility.ToHankaku(tbOwnerMailAddr2.Text.Trim()) },
			{ CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_1,
				StringUtility.ToHankaku(tbOwnerZip1.Text.Trim()) },
			{ CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_2,
				StringUtility.ToHankaku(tbOwnerZip2.Text.Trim()) },
			{ Constants.FIELD_ORDEROWNER_OWNER_ADDR1, ddlOwnerAddr1.SelectedValue },
			{ Constants.FIELD_ORDEROWNER_OWNER_ADDR2,
				DataInputUtility.ConvertToFullWidthBySetting(tbOwnerAddr2.Text.Trim(), this.IsOwnerAddrJp) },
			{ Constants.FIELD_ORDEROWNER_OWNER_ADDR3,
				DataInputUtility.ConvertToFullWidthBySetting(tbOwnerAddr3.Text.Trim(), this.IsOwnerAddrJp) },
			{ Constants.FIELD_ORDEROWNER_OWNER_ADDR4,
				DataInputUtility.ConvertToFullWidthBySetting(tbOwnerAddr4.Text.Trim(), this.IsOwnerAddrJp) },
			{ Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME, tbOwnerCompanyName.Text.Trim() },
			{ Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME, tbOwnerCompanyPostName.Text.Trim() },
			{ CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_1, StringUtility.ToHankaku(tbOwnerTel1_1.Text.Trim()) },
			{ CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_2, StringUtility.ToHankaku(tbOwnerTel1_2.Text.Trim()) },
			{ CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_3, StringUtility.ToHankaku(tbOwnerTel1_3.Text.Trim()) },
			{ Constants.FIELD_ORDEROWNER_OWNER_SEX, rblOwnerSex.SelectedValue },
			{ CartOwner.FIELD_ORDEROWNER_OWNER_BIRTH_YEAR, ucOwnerBirth.Year },
			{ CartOwner.FIELD_ORDEROWNER_OWNER_BIRTH_MONTH, ucOwnerBirth.Month },
			{ CartOwner.FIELD_ORDEROWNER_OWNER_BIRTH_DAY, ucOwnerBirth.Day },
			{ Constants.FIELD_ORDEROWNER_ACCESS_COUNTRY_ISO_CODE,
				(Constants.GLOBAL_OPTION_ENABLE)
					? ddlDispCurrencyLocaleId.SelectedValue
					: string.Empty },
			{ Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_CODE,
				(Constants.GLOBAL_OPTION_ENABLE)
					? hfDispLanguageCode.Value
					: string.Empty },
			{ Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID,
				(Constants.GLOBAL_OPTION_ENABLE)
					? ddlDispLanguageLocaleId.SelectedValue
					: string.Empty },
			{ Constants.FIELD_ORDEROWNER_DISP_CURRENCY_CODE,
				(Constants.GLOBAL_OPTION_ENABLE)
					? hfDispCurrencyCode.Value
					: string.Empty },
			{ Constants.FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID,
				(Constants.GLOBAL_OPTION_ENABLE)
					? ddlDispCurrencyLocaleId.SelectedValue
					: string.Empty },
			{ Constants.FIELD_USER_MAIL_FLG, rblMailFlg.SelectedValue },
			{ Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE, string.Empty },
			{ Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME, string.Empty },
			{ Constants.FIELD_ORDEROWNER_OWNER_ADDR5, string.Empty },
		};

		if ((ucOwnerBirth.Year.Length + ucOwnerBirth.Month.Length + ucOwnerBirth.Day.Length) != 0)
		{
			ownerInput.Add(Constants.FIELD_ORDEROWNER_OWNER_BIRTH, ucOwnerBirth.DateString);
		}

		CreateOwnerTel2(ownerInput);

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE] = ddlOwnerCountry.SelectedValue;
			ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME] = ddlOwnerCountry.SelectedItem.Text;
			var addr5 = this.IsOwnerAddrUs
				? ddlOwnerAddr5.SelectedValue
				: DataInputUtility.ConvertToFullWidthBySetting(tbOwnerAddr5.Text.Trim(), this.IsOwnerAddrJp);
			ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ADDR5] = addr5;
			ownerInput.Add(
				Constants.FIELD_ORDEROWNER_OWNER_ZIP,
				StringUtility.ToHankaku(tbOwnerZipGlobal.Text.Trim()));
			ownerInput.Add(
				Constants.FIELD_ORDEROWNER_OWNER_TEL1,
				StringUtility.ToHankaku(tbOwnerTel1Global.Text.Trim()));

			if (this.IsOwnerAddrJp == false)
			{
				ownerInput[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1] = string.Empty;
				ownerInput[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2] = string.Empty;
				ownerInput[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA] = string.Empty;
			}
		}
		return ownerInput;
	}

	/// <summary>
	/// 配送先入力情報取得
	/// </summary>
	/// <returns>入力情報</returns>
	private Hashtable GetShippingInput()
	{
		var shippingInput = new Hashtable();
		if ((this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER)
			|| (this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_STORE_PICKUP)) return shippingInput;

		var isRegist = (this.IsUser
			&& (ddlUserShipping.SelectedValue
				== CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW)
			&& cbAlowSaveNewAddress.Checked);
		shippingInput.Add(Constants.FIELD_USERSHIPPING_NAME,
			isRegist
				? tbShippingName.Text.Trim()
				: ddlUserShipping.SelectedItem.Text);
		shippingInput.Add(CartShipping.FIELD_USERSHIPPING_REGIST_FLG, isRegist);

		if (ddlUserShipping.SelectedValue
			== CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW)
		{
			var shippingName1 =
				DataInputUtility.ConvertToFullWidthBySetting(tbShippingName1.Text.Trim(), this.IsShippingAddrJp);
			var shippingName2 =
				DataInputUtility.ConvertToFullWidthBySetting(tbShippingName2.Text.Trim(), this.IsShippingAddrJp);
			var shippingNameKana1 = StringUtility.ToZenkaku(tbShippingNameKana1.Text.Trim());
			var shippingNameKana2 = StringUtility.ToZenkaku(tbShippingNameKana2.Text.Trim());
			shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1, shippingName1);
			shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2, shippingName2);
			shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME, shippingName1 + shippingName2);
			shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1, shippingNameKana1);
			shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2, shippingNameKana2);
			shippingInput.Add(
				Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA,
				shippingNameKana1 + shippingNameKana2);
			shippingInput.Add(
				CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_1,
				StringUtility.ToHankaku(tbShippingZip1.Text.Trim()));
			shippingInput.Add(
				CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_2,
				StringUtility.ToHankaku(tbShippingZip2.Text.Trim()));
			shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1, ddlShippingAddr1.SelectedValue);
			shippingInput.Add(
				Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2,
				DataInputUtility.ConvertToFullWidthBySetting(tbShippingAddr2.Text.Trim(), this.IsShippingAddrJp));
			shippingInput.Add(
				Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3,
				DataInputUtility.ConvertToFullWidthBySetting(tbShippingAddr3.Text.Trim(), this.IsShippingAddrJp));
			shippingInput.Add(
				Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4,
				DataInputUtility.ConvertToFullWidthBySetting(tbShippingAddr4.Text.Trim(), this.IsShippingAddrJp));
			shippingInput.Add(
				Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME,
				tbShippingCompanyName.Text.Trim());
			shippingInput.Add(
				Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME,
				tbShippingCompanyPostName.Text.Trim());
			shippingInput.Add(
				CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_1,
				StringUtility.ToHankaku(tbShippingTel1_1.Text.Trim()));
			shippingInput.Add(
				CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_2,
				StringUtility.ToHankaku(tbShippingTel1_2.Text.Trim()));
			shippingInput.Add(
				CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_3,
				StringUtility.ToHankaku(tbShippingTel1_3.Text.Trim()));

			shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP, string.Empty);
			shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1, string.Empty);
			shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE, string.Empty);
			shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME, string.Empty);
			shippingInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5, string.Empty);

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE] =
					ddlShippingCountry.SelectedValue;
				shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME] =
					ddlShippingCountry.SelectedItem.Text;

				if (this.IsShippingAddrJp == false)
				{
					shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP] =
						StringUtility.ToHankaku(tbShippingZipGlobal.Text.Trim());
					shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1] =
						StringUtility.ToHankaku(tbShippingTel1.Text.Trim());
					var addr5 = this.IsShippingAddrUs
						? ddlShippingAddr5.SelectedValue
						: DataInputUtility.ConvertToFullWidthBySetting(
							tbShippingAddr5.Text.Trim(),
							this.IsShippingAddrJp);
					shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5] = addr5;
					shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1] = string.Empty;
					shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2] = string.Empty;
					shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA] = string.Empty;
				}
			}
		}
		else if (ddlUserShipping.SelectedValue
			!= CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
		{
			var userShipping = DomainFacade.Instance.UserShippingService.Get(
				hfUserId.Value,
				int.Parse(ddlUserShipping.SelectedValue));
			if (userShipping.ShippingReceivingStoreFlg
				== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
			{
				foreach (var key in userShipping.DataSource.Keys)
				{
					shippingInput[key] = userShipping.DataSource[key];
				}
				shippingInput.Add(CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_1, userShipping.ShippingZip1);
				shippingInput.Add(CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_2, userShipping.ShippingZip2);
				shippingInput.Add(CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_1, userShipping.ShippingTel1_1);
				shippingInput.Add(CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_2, userShipping.ShippingTel1_2);
				shippingInput.Add(CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_3, userShipping.ShippingTel1_3);
			}
			else
			{
				shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG]
					= Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
				shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID] =
					userShipping.ShippingReceivingStoreId;
				shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1] = userShipping.ShippingName;
				shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4] = userShipping.ShippingAddr4;
				shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1] = userShipping.ShippingTel1;
				shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE] = Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
					? userShipping.ShippingReceivingStoreType
					: string.Empty;
			}
		}

		if ((this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_USER_INPUT)
			&& (ddlUserShipping.SelectedValue
				== CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE))
		{
			shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG]
				= Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON;
			shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID] = hfCvsShopNo.Value;
			shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1] = hfCvsShopName.Value;
			shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4] = hfCvsShopAddress.Value;
			shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1] = hfCvsShopTel.Value;
			shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE] = Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
				? ddlShippingReceivingStoreType.SelectedValue
				: string.Empty;
		}
		return shippingInput;
	}

	/// <summary>
	/// カート注文者作成
	/// </summary>
	/// <param name="ownerInput">注文者入力情報</param>
	private CartOwner CreateCartOwner(Hashtable ownerInput)
	{
		DateTime? birth = null;
		DateTime birthTmp;
		var ownerBirth = StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_BIRTH]);
		if (DateTime.TryParse(ownerBirth, out birthTmp))
		{
			birth = birthTmp;
		}
		var mailFlag =
			(StringUtility.ToEmpty(ownerInput[Constants.FIELD_USER_MAIL_FLG]) == Constants.FLG_USER_MAILFLG_OK);

		return new CartOwner(
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_KBN]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_NAME]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_NAME1]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_NAME2]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA1]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_NAME_KANA2]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ZIP]),
			StringUtility.ToEmpty(ownerInput[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_1]),
			StringUtility.ToEmpty(ownerInput[CartOwner.FIELD_ORDEROWNER_OWNER_ZIP_2]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ADDR1]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ADDR2]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ADDR3]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ADDR4]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ADDR5]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_ISO_CODE]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_ADDR_COUNTRY_NAME]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_NAME]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_COMPANY_POST_NAME]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_TEL1]),
			StringUtility.ToEmpty(ownerInput[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_1]),
			StringUtility.ToEmpty(ownerInput[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_2]),
			StringUtility.ToEmpty(ownerInput[CartOwner.FIELD_ORDEROWNER_OWNER_TEL1_3]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_TEL2]),
			StringUtility.ToEmpty(ownerInput[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1]),
			StringUtility.ToEmpty(ownerInput[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2]),
			StringUtility.ToEmpty(ownerInput[CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3]),
			mailFlag,
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_OWNER_SEX]),
			birth,
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_ACCESS_COUNTRY_ISO_CODE]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_CODE]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_DISP_LANGUAGE_LOCALE_ID]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_CODE]),
			StringUtility.ToEmpty(ownerInput[Constants.FIELD_ORDEROWNER_DISP_CURRENCY_LOCALE_ID]));
	}

	/// <summary>
	/// カート配送先作成
	/// </summary>
	/// <param name="shippingInput">配送先入力情報</param>
	/// <param name="cart">カート情報</param>
	/// <returns>Cart shipping</returns>
	private CartShipping CreateCartShipping(Hashtable shippingInput, CartObject cart)
	{
		var shipping = new CartShipping(cart);
		if (this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER)
		{
			shipping.UpdateShippingAddr(cart.Owner, true);
		}
		else if ((StringUtility.ToEmpty(
				shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_FLG])
			== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
			&& (this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_USER_INPUT))
		{
			shipping.UpdateConvenienceStoreAddr(
				CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE,
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_ID]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_RECEIVING_STORE_TYPE]));
		}
		else if (this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_USER_INPUT)
		{
			shipping.UpdateShippingAddr(
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME1]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME2]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA1]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME_KANA2]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP]),
				StringUtility.ToEmpty(shippingInput[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_1]),
				StringUtility.ToEmpty(shippingInput[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_ZIP_2]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR5]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_NAME]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_NAME]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_COMPANY_POST_NAME]),
				StringUtility.ToEmpty(shippingInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1]),
				StringUtility.ToEmpty(shippingInput[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_1]),
				StringUtility.ToEmpty(shippingInput[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_2]),
				StringUtility.ToEmpty(shippingInput[CartShipping.FIELD_ORDERSHIPPING_SHIPPING_TEL1_3]),
				true,
				CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW);
			shipping.UpdateUserShippingRegistSetting(
				(bool)shippingInput[CartShipping.FIELD_USERSHIPPING_REGIST_FLG],
				shippingInput[Constants.FIELD_USERSHIPPING_NAME].ToString());
		}
		else if ((this.RealShopModels != null) && (this.RealShopModels.Length > 0))
		{
			var model = this.RealShopModels[this.ddlRealStore.SelectedIndex];
			shipping.UpdateStorePickupShippingAddr(
				model.AreaId,
				model.RealShopId,
				model.Name,
				model.NameKana,
				model.Zip,
				model.Addr1,
				model.Addr2,
				model.Addr3,
				model.Addr4,
				model.Addr5,
				model.OpeningHours,
				model.Tel,
				model.CountryIsoCode,
				model.CountryName);
		}
		return shipping;
	}

	/// <summary>
	/// 注文商品情報作成
	/// </summary>
	/// <returns>Items</returns>
	private List<Hashtable> CreateOrderItemInput()
	{
		var items = new List<Hashtable>();

		// 表示されている情報を追加
		foreach (RepeaterItem riItem in rItemList.Items)
		{
			var productId = ((TextBox)riItem.FindControl("tbProductId")).Text.Trim();
			if (string.IsNullOrEmpty(productId) && (rItemList.Items.Count > 1)) continue;

			var isFixedPurchaseValid = (Constants.FIXEDPURCHASE_OPTION_ENABLED
				&& ((CheckBox)riItem.FindControl("cbFixedPurchase")).Checked);
			var productName = CreateFixedPurchaseProductName(
				((TextBox)riItem.FindControl("tbProductName")).Text,
				isFixedPurchaseValid,
				this.IsSubscriptionBoxValid);
			var setPromotionNo = ((HiddenField)riItem.FindControl("hfOrderSetPromotionNo")).Value;
			var setPromotionName = ((HiddenField)riItem.FindControl("hfOrderSetPromotionName")).Value;

			if (productName.Contains(setPromotionNo))
			{
				var replaceText = string.Format("[{0}：{1}]",
					setPromotionNo,
					setPromotionName);
				productName = productName.Replace(replaceText, string.Empty);
			}

			var variationId = ((HiddenField)riItem.FindControl("hfProductVariationId")).Value;
			if (string.IsNullOrEmpty(variationId))
			{
				// Set first variation id if the product has variations
				var productVariations = DomainFacade.Instance.ProductService.GetProductVariationsByProductId(productId);
				variationId = productVariations.Any() ? productVariations.First().VariationId : productId;
			}

			if (this.ChangeSaleId)
			{
				var saleId = ((TextBox)riItem.FindControl("tbProductSaleId")).Text.Trim();
				var variationInfo = GetProductVariation(
						this.LoginOperatorShopId,
						productId,
						variationId,
						hfMemberRankId.Value,
						saleId);
				if (string.IsNullOrEmpty(saleId)) variationInfo[0].Row[Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE] = DBNull.Value;
				var isFirstOrder = ((this.ViewState["CombineParentOrderCount"] == null) || (this.CombineParentOrderCount == 1));
				((TextBox)riItem.FindControl("tbProductPrice")).Text = isFixedPurchaseValid
				? GetFixedPurchaseProductValidityPrice(variationInfo[0], isFirstOrder).ToPriceString()
				: GetProductValidityPrice(variationInfo[0]).ToPriceString();
			}

			var item = new Hashtable
			{
				{ Constants.FIELD_ORDERITEM_SHOP_ID, this.LoginOperatorShopId },
				{ Constants.FIELD_ORDERITEM_PRODUCT_ID, productId },
				{ Constants.FIELD_ORDERITEM_SUPPLIER_ID, ((HiddenField)riItem.FindControl("hfSupplierId")).Value },
				{ Constants.FIELD_ORDERITEM_VARIATION_ID, variationId },
				{ Constants.FIELD_PRODUCTVARIATION_V_ID,
					(string.IsNullOrEmpty(variationId) == false) && (string.IsNullOrEmpty(productId) == false)
						? variationId.Replace(productId, string.Empty)
						: variationId },
				{ Constants.FIELD_ORDERITEM_PRODUCT_NAME, productName },
				{ Constants.FIELD_ORDERITEM_PRODUCT_PRICE, ((TextBox)riItem.FindControl("tbProductPrice")).Text.Trim() },
				{ Constants.KEY_OPTION_INCLUDED_ITEM_PRICE, ((HiddenField)riItem.FindControl("hfOptionPrice")).Value },
				{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, ((TextBox)riItem.FindControl("tbItemQuantity")).Text.Trim() },
				{ Constants.FIELD_ORDERITEM_PRODUCTSALE_ID, ((TextBox)riItem.FindControl("tbProductSaleId")).Text.Trim() },
				{ CONST_KEY_FIXED_PURCHASE,
					Constants.FIXEDPURCHASE_OPTION_ENABLED
						? ((CheckBox)riItem.FindControl("cbFixedPurchase")).Checked
						: false },
				{ Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, setPromotionNo },
				{ Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME, setPromotionName },
				{ Constants.FIELD_ORDERITEM_NOVELTY_ID, ((HiddenField)riItem.FindControl("hfNoveltyId")).Value },
				{ Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG,
					Constants.FIXEDPURCHASE_OPTION_ENABLED
						? ((CheckBox)riItem.FindControl("cbFixedPurchase")).Checked
						: false },
				{ Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING,
					Constants.FIXEDPURCHASE_OPTION_ENABLED
						? ((HiddenField)riItem.FindControl("hfLimitedFixedPurchaseKbn1Setting")).Value
						: string.Empty },
				{ Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING,
					Constants.FIXEDPURCHASE_OPTION_ENABLED
						? ((HiddenField)riItem.FindControl("hfLimitedFixedPurchaseKbn3Setting")).Value
						: string.Empty },
				{ Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING,
					Constants.FIXEDPURCHASE_OPTION_ENABLED
						? ((HiddenField)riItem.FindControl("hfLimitedFixedPurchaseKbn4Setting")).Value
						: string.Empty },
				{ Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE,
					((HiddenField)riItem.FindControl("hfTaxRate")).Value },
				{ Constants.FIELD_PRODUCT_IMAGE_HEAD,
					((HiddenField)riItem.FindControl("hfProductImageHead")).Value },
				{ Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE,
					Constants.FIXEDPURCHASE_OPTION_ENABLED
						? ((HiddenField)riItem.FindControl("hfFixedPurchaseDiscountSettingValue")).Value
						: string.Empty },
				{ Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_TYPE,
					Constants.FIXEDPURCHASE_OPTION_ENABLED
						? ((HiddenField)riItem.FindControl("hfFixedPurchaseDiscountSettingType")).Value
						: string.Empty },
				{ Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID,
					((HiddenField)riItem.FindControl("hfSubscriptionBoxCourseId")).Value },
				{ Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT,
					((HiddenField)riItem.FindControl("hfSubscriptionBoxFixedAmount")).Value },
				{ CONST_HASHKEY_CART_PRODUCT_IS_ORDER_COMBINED,
					((HiddenField)riItem.FindControl("hfIsOrderCombined")).Value },
			};

			// 小計計算
			var productPrice = StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]);
			decimal price;
			if (decimal.TryParse(productPrice, out price) == false)
			{
				price = 0;
			}
			var itemQuantity = StringUtility.ToEmpty(item[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]);
			decimal quantity;
			if (decimal.TryParse(itemQuantity, out quantity) == false)
			{
				quantity = 0;
			}

			var optionPriceText = StringUtility.ToEmpty(item[Constants.KEY_OPTION_INCLUDED_ITEM_PRICE]);
			decimal optionPrice;
			if (decimal.TryParse(optionPriceText, out optionPrice) == false)
			{
				optionPrice = 0;
			}
			item.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE, (price * quantity + optionPrice * quantity).ToString());

			// 商品付帯情報オブジェクトを注文商品ハッシュテーブルへ設定
			item.Add(CONST_KEY_PRODUCT_OPTION_VALUE_SETTINGS, CreateProductOptionSettingList(riItem));
			items.Add(item);
		}
		return items;
	}

	/// <summary>
	/// 商品付帯情報作成
	/// </summary>
	/// <param name="riItem">付帯情報選択領域のリピータアイテム</param>
	/// <returns>商品付帯情報設定リスト</returns>
	private ProductOptionSettingList CreateProductOptionSettingList(RepeaterItem riItem)
	{
		var productId = ((TextBox)riItem.FindControl("tbProductId")).Text;
		var productIdForOptionSetting =
			((HiddenField)riItem.FindControl("hfProductIdForOptionSetting")).Value;
		var settingList =
			new ProductOptionSettingList(this.LoginOperatorShopId, productId);

		// 商品が変更されていたら選択無しの設定を渡す
		if (productId != productIdForOptionSetting)
		{
			productIdForOptionSetting = productId;
			return settingList;
		}

		var rProductOptionValueSettings = ((Repeater)riItem.FindControl("rProductOptionValueSettings"));

		if (this.IsOrderCombined && (this.OriginOrderCombineCart != null))
		{
			var product = this.OriginOrderCombineCart.Items.Where(x => x.ProductId == productId).ToList();
			var rIndex = riItem.ItemIndex;
			if (product.Any())
			{
				var targetProductId = product[0].ProductId;
				var countProduct = new List<int>();
				foreach (RepeaterItem item in rItemList.Items)
				{
					var id = ((TextBox)item.FindControl("tbProductId")).Text.Trim();

					if (id == targetProductId) countProduct.Add(item.ItemIndex); ;
				}

				var targetOptionNumber = countProduct.IndexOf(rIndex);

				if (product.Count > targetOptionNumber)
				{
					// 選択値セット
					foreach (var setting in product[targetOptionNumber].ProductOptionSettingList.Items)
					{
						var index = product[targetOptionNumber].ProductOptionSettingList.Items.IndexOf(setting);
						if ((index < 0)
							|| (rProductOptionValueSettings == null)
							|| (index >= rProductOptionValueSettings.Items.Count))
						{
							continue;
						}

						var riTarget = rProductOptionValueSettings.Items[index];
						var riValue = GetSelectedProductOptionValue(riTarget);
						setting.SelectedSettingValue = riValue;
					}
					return product[targetOptionNumber].ProductOptionSettingList;
				}
			}
		}

		// 選択値セット
		foreach (var setting in settingList.Items)
		{
			var index = settingList.Items.IndexOf(setting);
			if ((index < 0)
				|| (rProductOptionValueSettings == null)
				|| (index >= rProductOptionValueSettings.Items.Count))
			{
				continue;
			}

			var riTarget = rProductOptionValueSettings.Items[index];
			var riValue = GetSelectedProductOptionValue(riTarget);
			setting.SelectedSettingValue = riValue;
		}

		return settingList;
	}


	/// <summary>
	/// カート決済情報作成
	/// </summary>
	/// <param name="cart">カート</param>
	/// <param name="orderCreditCardInput">注文クレジットカード入力</param>
	/// <returns>カート決済情報</returns>
	private CartPayment CreateCartPayment(CartObject cart, OrderCreditCardInput orderCreditCardInput)
	{
		var payment = new CartPayment();
		if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			// 仮クレジットカードの場合は置き換え（カード入力できない場合は仮クレジットカード利用と判定）
			var orderPaymentKbnValue = ddlOrderPaymentKbn.SelectedItem.Value;
			var orderPaymentKbnName = ddlOrderPaymentKbn.SelectedItem.Text;
			if ((ddlUserCreditCard.SelectedValue == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				&& (this.CanUseCreditCardNoForm == false))
			{
				var provisionalCreditcardPayment = DomainFacade.Instance.PaymentService.Get(
					this.LoginOperatorShopId,
					Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID);
				orderPaymentKbnValue = provisionalCreditcardPayment.PaymentId;
				orderPaymentKbnName = provisionalCreditcardPayment.PaymentName;
			}

			payment.UpdateCartPayment(
				orderPaymentKbnValue,
				orderPaymentKbnName,
				orderCreditCardInput.CreditBranchNo,
				orderCreditCardInput.CompanyCode,
				orderCreditCardInput.CardNo1,
				orderCreditCardInput.CardNo2,
				orderCreditCardInput.CardNo3,
				orderCreditCardInput.CardNo4,
				orderCreditCardInput.ExpireMonth,
				orderCreditCardInput.ExpireYear,
				orderCreditCardInput.InstallmentsCode,
				orderCreditCardInput.SecurityCode,
				orderCreditCardInput.AuthorName,
				null,
				false,
				string.Empty,
				creditBincode: orderCreditCardInput.CreditBincode);

			payment.UserCreditCard = (ddlUserCreditCard.SelectedValue
				!= CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
					? UserCreditCard.Get(hfUserId.Value, int.Parse(ddlUserCreditCard.SelectedValue))
					: null;

			if (payment.UserCreditCard != null)
			{
				payment.CreditExpireMonth = payment.UserCreditCard.ExpirationMonth;
				payment.CreditExpireYear = payment.UserCreditCard.ExpirationYear;
			}

			// 登録カード情報セット
			payment.UserCreditCardRegistable = orderCreditCardInput.DoRegister;

			// If not regist card then set credit card name is empty
			payment.UpdateUserCreditCardRegistSetting(
				orderCreditCardInput.DoRegister,
				cbRegistCreditCard.Checked
					? orderCreditCardInput.RegisterCardName
					: string.Empty);

			// Token情報セット
			if (orderCreditCardInput.CreditToken != null)
			{
				payment.CreditToken = orderCreditCardInput.CreditToken;
			}
		}
		else if (ddlOrderPaymentKbn.SelectedItem != null)
		{
			payment.PaymentId = ddlOrderPaymentKbn.SelectedValue;
			payment.PaymentName = ddlOrderPaymentKbn.SelectedItem.Text;

			// ペイパルの場合は連携情報セット
			if ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
				&& (string.IsNullOrEmpty(hfUserId.Value) == false))
			{
				this.AccountEmail = PayPalUtility.Account.GetCooperateAccountEmail(hfUserId.Value);
				if (string.IsNullOrEmpty(this.AccountEmail) == false)
				{
					var userExtend = DomainFacade.Instance.UserService.GetUserExtend(hfUserId.Value);
					cart.PayPalCooperationInfo = new PayPalCooperationInfo(userExtend);
				}
			}
			else if ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
				&& (string.IsNullOrEmpty(hfPaidyTokenId.Value) == false))
			{
				payment.PaidyToken = hfPaidyTokenId.Value;
			}
		}
		return payment;
	}
	#endregion

	#region 注文系オブジェクトチェック処理
	/// <summary>
	/// 注文者入力チェック
	/// </summary>
	/// <param name="input">注文者入力情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckOwnerInput(Hashtable input)
	{
		var validatorName = this.IsOwnerAddrJp
			? "OrderOwnerRegistInput"
			: "OrderOwnerRegistInputGlobal";
		return Validator.Validate(validatorName, input, this.OwnerAddrCountryIsoCode);
	}

	/// <summary>
	/// 配送先入力チェック
	/// </summary>
	/// <param name="input">配送先入力情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckShippingInput(Hashtable input)
	{
		var shippingddrCountryIsoCode =
			StringUtility.ToEmpty(input[Constants.FIELD_ORDERSHIPPING_SHIPPING_COUNTRY_ISO_CODE]);
		var validatorName = IsCountryJp(shippingddrCountryIsoCode)
			? "OrderShippingRegistInput"
			: "OrderShippingRegistInputGlobal";
		return Validator.Validate(validatorName, input, shippingddrCountryIsoCode);
	}

	/// <summary>
	/// 注文商品・価格入力チェック
	/// </summary>
	/// <param name="orderItems">注文商品情報</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckItemsAndPricesInput(List<Hashtable> orderItems)
	{
		// 商品チェック
		var messages = new StringBuilder();
		foreach (var orderItem in orderItems)
		{
			messages.Append(
				Validator.Validate("OrderItemRegistInput", orderItem)
					.Replace(
						"@@ 1 @@",
						string.Format(
							ReplaceTag("@@DispText.common_message.location_no@@"),
							(orderItems.IndexOf(orderItem) + 1),
							string.Empty)));

			// 付帯情報の入力チェック
			var optionSettingList =
				(ProductOptionSettingList)orderItem[CONST_KEY_PRODUCT_OPTION_VALUE_SETTINGS];
			var checkKbn = "OptionValueValidate";
			foreach (ProductOptionSetting optionSetting in optionSettingList)
			{
				var tmpValueName = optionSetting.ValueName;
				optionSetting.ValueName = string.Format(
					ReplaceTag("@@DispText.common_message.location_no@@"),
					(orderItems.IndexOf(orderItem) + 1).ToString(),
					optionSetting.ValueName);
				if (optionSetting.IsTextBox)
				{
					var validatorXml = optionSetting.CreateValidatorXml(checkKbn);
					var param = new Hashtable();
					param[optionSetting.ValueName] = (optionSetting.SelectedSettingValue == null)
						? optionSetting.DefaultValue
						: optionSetting.SelectedSettingValue;
					messages.Append(Validator.Validate(checkKbn, validatorXml.InnerXml, param));
				}
				else if (optionSetting.IsCheckBox)
				{
					var optionSettingCheckBoxList =
						optionSetting.SettingValuesListItemCollection.Cast<ListItem>().ToList();
					if (optionSetting.IsNecessary && (optionSettingCheckBoxList.Count(value => value.Selected) == 0))
					{
						messages.Append(
							WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR)
								.Replace("@@ 1 @@", optionSetting.ValueName));
					}
				}
				else if (optionSetting.IsSelectMenu)
				{
					if (optionSetting.IsNecessary && (optionSetting.SelectedSettingValue == optionSetting.SettingValues.First()))
					{
						messages.Append(
							WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_INPUT_PRODUCT_OPTION_NOT_SELECTED_ITEM_ERROR)
								.Replace("@@ 1 @@", optionSetting.ValueName));
					}
				}
				optionSetting.ValueName = tmpValueName;
			}

			// 商品有効性チェック
			if (messages.Length == 0)
			{
				var product = GetProductVariation(
					this.LoginOperatorShopId,
					StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]),
					StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_VARIATION_ID]));
				if (((product.Count > 0)
						&& (StringUtility.ToEmpty(product[0][Constants.FIELD_PRODUCT_VALID_FLG]) == Constants.FLG_PRODUCT_VALID_FLG_INVALID))
					|| (product.Count == 0))
				{
					messages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID)
						.Replace("@@ 1 @@", StringUtility.ToEmpty(orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID])));
				}
			}

			// 頒布会コース選択時、選択可能商品に含まれているかチェック
			if ((messages.Length == 0)
				&& (string.IsNullOrEmpty(ddlSubscriptionBox.SelectedValue) == false)
				&& ((this.IsOrderCombined == false)
					|| this.IsOrderCombinedWithSameSubscriptionBox))
			{
				var subscription = DataCacheControllerFacade
					.GetSubscriptionBoxCacheController()
					.Get(ddlSubscriptionBox.SelectedValue);

				if (subscription.SelectableProducts.All(s => (s.VariationId != (string)orderItem[Constants.FIELD_ORDERITEM_VARIATION_ID])))
				{
					messages.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID_FOR_SUBSCRIPTION_BOX)
						.Replace("@@ 1 @@", (string)orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]));
				}
			}
		}

		// 同一商品で単価が異なる商品があればエラーにする
		var groupedItems = orderItems.GroupBy(item =>
			(item[Constants.FIELD_ORDERITEM_PRODUCT_ID] + " "
			+ item[Constants.FIELD_ORDERITEM_VARIATION_ID] + " "
			+ item[Constants.FIELD_ORDERITEM_PRODUCTSALE_ID] + " "
			+ ((bool)item[CONST_KEY_FIXED_PURCHASE] ? "1" : "0") + " "
			+ ((ProductOptionSettingList)item[CONST_KEY_PRODUCT_OPTION_VALUE_SETTINGS])
				.GetDisplayProductOptionSettingSelectValues()));
		foreach (var items in groupedItems)
		{
			var itemList = items.ToList();
			var price = StringUtility.ToEmpty(itemList[0][Constants.FIELD_ORDERITEM_PRODUCT_PRICE]);
			for (var index = 1; index < itemList.Count; index++)
			{
				if (StringUtility.ToEmpty(itemList[index][Constants.FIELD_ORDERITEM_PRODUCT_PRICE]) == price) continue;

				var message = new StringBuilder();
				foreach (var item in itemList)
				{
					message.Append(
						(message.Length == 0)
							? string.Empty
							//「と」
							: ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_ORDER_REGIST_INPUT,
								Constants.VALUETEXT_PARAM_ORDER_REGIST_INPUT_MESSAGE,
								Constants.VALUETEXT_PARAM_ORDER_REGIST_INPUT_WHEN))
						.Append(orderItems.IndexOf(item) + 1)
						.Append(
							//「番目」
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_ORDER_REGIST_INPUT,
								Constants.VALUETEXT_PARAM_ORDER_REGIST_INPUT_MESSAGE,
								Constants.VALUETEXT_PARAM_ORDER_REGIST_INPUT_ORDINAL));
				}
				messages.Append(WebMessages.GetMessages(
					WebMessages.ERRMSG_MANAGER_ORDERREGIST_SAME_PRODUCT_PRICE_ERROR)
					.Replace("@@ 1 @@", message.ToString()));
				break;
			}
		}

		// 調整金額補正・チェック
		if (string.IsNullOrEmpty(tbOrderPriceRegulation.Text.Trim()))
		{
			tbOrderPriceRegulation.Text = 0.ToPriceString();
		}
		var input = new Hashtable
		{
			{ Constants.FIELD_ORDER_ORDER_PRICE_REGULATION, tbOrderPriceRegulation.Text.Trim() }
		};
		messages.Append(Validator.Validate("OrderItemRegistInput", input));
		return messages.ToString();
	}

	/// <summary>
	/// 頒布会注文商品・価格入力チェック
	/// </summary>
	/// <param name="orderItems">注文商品情報</param>
	private void CheckSubscriptionBoxItemsAndPricesInput(List<Hashtable> orderItems)
	{
		var messages = string.Empty;

		// 頒布会コース選択時、選択可能商品に含まれているかチェック
		if (string.IsNullOrEmpty(ddlSubscriptionBox.SelectedValue) == false)
		{
			var subscription = DataCacheControllerFacade
				.GetSubscriptionBoxCacheController()
				.Get(ddlSubscriptionBox.SelectedValue);

			foreach (var orderItem in orderItems)
			{
				if (string.IsNullOrEmpty(messages) == false) break;

				if (subscription.SelectableProducts.All(
						s => (s.VariationId != (string)orderItem[Constants.FIELD_ORDERITEM_VARIATION_ID])))
				{
					messages = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID_FOR_SUBSCRIPTION_BOX)
						.Replace("@@ 1 @@", (string)orderItem[Constants.FIELD_ORDERITEM_PRODUCT_ID]);
				}
			}
		}

		// 頒布会コース選択時、商品合計金額下限（税込）・商品合計金額上限（税込）を満たしている
		foreach (var subscriptionGroup in orderItems.GroupBy(item => (string)item[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID]))
		{
			// 頒布会でない場合処理しない
			if (string.IsNullOrEmpty(subscriptionGroup.Key)) continue;

			var totalAmount = subscriptionGroup.Sum(x => x[Constants.FIELD_ORDERITEM_ITEM_PRICE].ToPriceDecimal());
			var message = OrderCommon.GetSubscriptionBoxTotalAmountError(subscriptionGroup.Key, totalAmount.Value);
			if (string.IsNullOrEmpty(message) == false)
			{
				messages += message + "<br/>";
			}
		}

		lSubscriptionBoxOrderItemErrorMessages.Text = messages;
		dvSubscriptionBoxOrderItemErrorMessage.Visible = (string.IsNullOrEmpty(messages) == false);
	}

	/// <summary>
	/// 利用ポイント入力チェック
	/// </summary>
	/// <returns>エラーメッセージ</returns>
	private string CheckPointUseInput()
	{
		if (string.IsNullOrEmpty(tbPointUse.Text.Trim())) tbPointUse.Text = "0";

		var input = new Hashtable
		{
			{ Constants.FIELD_ORDER_ORDER_POINT_USE, tbPointUse.Text.Trim() }
		};
		return Validator.Validate("OrderPointRegistInput", input);
	}

	/// <summary>
	/// 利用ポイントチェック＆セット（入力チェック済みが前提）
	/// </summary>
	/// <param name="cart">カート</param>
	/// <param name="pointUse">利用ポイント</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckAndSetPointUse(CartObject cart, decimal pointUse)
	{
		// 空だったら0にする
		if (string.IsNullOrEmpty(tbPointUse.Text.Trim())) tbPointUse.Text = "0";

		var userPointInfo = PointOptionUtility.GetUserPoint(cart.OrderUserId);
		var userPointUsable = (hfUserPointUsable.Value != "0")
			? decimal.Parse(hfUserPointUsable.Value)
			: ((userPointInfo != null) ? userPointInfo.PointUsable : 0);
		if (userPointUsable != 0)
		{
			lUserPointUsable.Text = StringUtility.ToNumeric(userPointUsable);
			hfUserPointUsable.Value = userPointUsable.ToString();
		}
		var pointUsePrice =
			DomainFacade.Instance.PointService.GetOrderPointUsePrice(pointUse, Constants.FLG_POINT_POINT_KBN_BASE);

		// 注文同梱元利用ポイント計算
		var combinedOrderUsedPoint = 0m;
		if (this.ActionStatus == Constants.ACTION_STATUS_ORDERCOMBINE)
		{
			var orderService = DomainFacade.Instance.OrderService;
			var parentOrder = orderService.Get(this.CombineParentOrderId);
			combinedOrderUsedPoint = parentOrder.OrderPointUse;

			foreach (var childId in this.CombineChildOrderIds)
			{
				var childOrder = orderService.Get(childId);
				combinedOrderUsedPoint += childOrder.OrderPointUse;
			}
		}

		// 利用ポイント > ユーザー本ポイントならエラー(注文同梱の場合、ユーザー本ポイントに同梱元注文の利用ポイントを加える)
		if (userPointUsable < 0) userPointUsable = 0;
		if (pointUse > (userPointUsable + combinedOrderUsedPoint))
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_POINT_USE_MAX_ERROR)
				.Replace("@@ 1 @@", (StringUtility.ToNumeric(userPointUsable) + Constants.CONST_UNIT_POINT_PT));
		}

		// 次回購入の利用ポイントの全適用フラグセット
		if (this.CanUseAllPointFlg)
		{
			cart.UseAllPointFlg = cbUseAllPointFlg.Checked;
		}

		// 利用ポイントセット（再計算は呼び出し元で行う）
		cart.SetUsePoint(pointUse, pointUsePrice);
		cart.CalculateWithCartShipping();

		// ポイント利用額を超えていたらエラー
		if (pointUsePrice > cart.PointUsablePrice)
		{
			return WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_POINT_PRICE_SUBTOTAL_MINUS_ERROR)
				.Replace("@@ 1 @@", cart.PointUsablePrice.ToPriceString(true));
		}

		// 決済手数料を再計算する
		ReCalculatePaymentPriceAfterSetCouponOrPointUse(cart);

		return string.Empty;
	}

	/// <summary>
	/// 利用クーポンチェック＆セット
	/// </summary>
	/// <param name="cart">カート</param>
	/// <returns>エラーメッセージ</returns>
	private string CheckAndSetCouponUse(CartObject cart)
	{
		var couponCode = tbCouponUse.Text.Trim();
		if (string.IsNullOrEmpty(couponCode)) return string.Empty;

		// 入力チェック
		var input = new Hashtable
		{
			{ Constants.FIELD_ORDER_ORDER_COUPON_USE, couponCode }
		};
		var errorMessage = Validator.Validate("OrderCouponRegistInput", input);
		if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

		var counponNo = -1;
		// 同梱元注文で利用したクーポンかをチェック
		var isCombinedOrderCoupon = false;
		if (this.IsOrderCombined)
		{
			var orderService = DomainFacade.Instance.OrderService;
			var combineParentOrder = orderService.Get(this.CombineParentOrderId);

			// 親注文のクーポンと同じの場合
			if (combineParentOrder.Coupons.Any()
				&& (combineParentOrder.Coupons.First().CouponCode.ToUpper() == couponCode.ToUpper()))
			{
				isCombinedOrderCoupon = true;
				counponNo = combineParentOrder.Coupons[0].CouponNo;
			}
			else if (this.CombineChildOrderIds != null)
			{
				var combineChildOrders = this.CombineChildOrderIds
					.Select(id => orderService.Get(id))
					.ToArray();

				// 登録画面のクーポンコードと同じクーポンを利用している子注文を取得
				var targetChildOrder = combineChildOrders.FirstOrDefault(order => order.Coupons.Any() && (order.Coupons.First().CouponCode == couponCode));
				isCombinedOrderCoupon = targetChildOrder != null;

				if (isCombinedOrderCoupon) counponNo = targetChildOrder.Coupons[0].CouponNo;
			}
		}

		// クーポン情報取得
		var couponService = DomainFacade.Instance.CouponService;
		var userCoupons = (isCombinedOrderCoupon)
			? couponService.GetAllUserCouponsFromCouponCodeIncludeUnavailable(
				this.LoginOperatorDeptId,
				this.CouponUserId,
				couponCode)
			: couponService.GetAllUserCouponsFromCouponCode(
				this.LoginOperatorDeptId,
				this.CouponUserId,
				couponCode);

		// 存在チェック
		if (userCoupons.Length == 0)
			return CouponOptionUtility.GetErrorMessage(CouponErrorcode.NoCouponError)
				.Replace("@@ 1 @@", couponCode);

		// 同梱元注文で利用したクーポンの場合チェックをスキップ
		if (isCombinedOrderCoupon == false)
		{
			// 未使用クーポンチェック(回数制限有りクーポンのみ)
			var errorCode = CouponOptionUtility.CheckUseCoupon(
				userCoupons.First(),
				this.CouponUserId,
				cart.Owner.MailAddr);
			if (errorCode != CouponErrorcode.NoError)
				return CouponOptionUtility.GetErrorMessage(errorCode).Replace("@@ 1 @@", couponCode);

			// クーポン有効性チェック
			errorMessage = CouponOptionUtility.CheckCouponValidWithCart(cart, userCoupons.First());
			if (string.IsNullOrEmpty(errorMessage) == false)
				return errorMessage.Replace("@@ 1 @@", couponCode);
		}

		// 利用クーポンセット
		var userCoupon = (isCombinedOrderCoupon && counponNo != -1)
			? userCoupons.FirstOrDefault(c => c.CouponNo == counponNo) ?? userCoupons.First()
			: userCoupons.First();
		cart.Coupon = new CartCoupon(userCoupon);
		cart.CalculateWithCartShipping();

		if (cart.IsCouponNotApplicableByOrderCombined)
		{
			return CouponOptionUtility.GetErrorMessage(CouponErrorcode.CouponNotApplicableByOrderCombined);
		}

		// 決済手数料を再計算する
		ReCalculatePaymentPriceAfterSetCouponOrPointUse(cart);

		return string.Empty;
	}

	/// <summary>
	/// Check show notice message if payment user level not use
	/// </summary>
	/// <param name="shopId">Shop Id</param>
	/// <param name="userLevel">User Managerment Level</param>
	private void DisplayPaymentUserManagementLevel(string shopId, string userLevel)
	{
		var message = (this.IsUser)
			? GetPaymentUserManagementLevelNotUseMessage(shopId, userLevel)
			: string.Empty;
		lbSelectPaymentUserManagementLevelMessage.Text = GetEncodedHtmlDisplayMessage(message);
		dvSelectPaymentUserManagementLevelMessage.Visible = (string.IsNullOrEmpty(message) == false);
		lbPaymentUserManagementLevelMessage.Text = GetEncodedHtmlDisplayMessage(message);
		SetTbdyPaymentNoticeMessageVisibility();
	}

	/// <summary>
	/// 注文者区分選択できない決済種別を文言で表示
	/// </summary>
	/// <param name="shopId">ShopID</param>
	/// <param name="userKbn">注文者区分</param>
	private void DisplayPaymentOrderOwnerKbn(string shopId, string userKbn)
	{
		var message = GetPaymentOrderOwnerKbnNotUseMessage(shopId, userKbn);
		lbSelectPaymentOrderOwnerKbnMessage.Text = GetEncodedHtmlDisplayMessage(message);
		lbPaymentOrderOwnerKbnMessage.Text = GetEncodedHtmlDisplayMessage(message);
		SetTbdyPaymentNoticeMessageVisibility();
	}

	/// <summary>
	/// Display the limited payment messages.
	/// </summary>
	private void DisplayLimitedPaymentMessages()
	{
		var productList = new List<KeyValuePair<string, string>>();
		foreach (RepeaterItem item in rItemList.Items)
		{
			var productId = ((TextBox)item.FindControl("tbProductId")).Text.Trim();
			var variationId = ((DropDownList)item.FindControl("ddlVariationIdList")).SelectedValue;
			if (string.IsNullOrEmpty(productId)) continue;

			variationId = string.IsNullOrEmpty(variationId)
				? productId
				: variationId;
			productList.Add(new KeyValuePair<string, string>(productId, variationId));
		}

		lbPaymentLimitedMessage.Text = GetEncodedHtmlDisplayMessage(GetProductsLimitedPaymentMessage(
			this.LoginOperatorShopId,
			productList).Trim());
		SetTbdyPaymentNoticeMessageVisibility();
	}

	/// <summary>
	/// 決済種別の注意文言を表示、非表示
	/// </summary>
	private void SetTbdyPaymentNoticeMessageVisibility()
	{
		dvPaymentNoticeMessage.Visible = ((string.IsNullOrEmpty(lbPaymentLimitedMessage.Text) == false)
			|| (string.IsNullOrEmpty(lbPaymentUserManagementLevelMessage.Text) == false)
			|| (string.IsNullOrEmpty(lbPaymentOrderOwnerKbnMessage.Text) == false));
	}

	/// <summary>
	///  広告コードと広告名は有効だったら連係が追加される
	/// </summary>
	/// <returns>チェック結果</returns>
	private bool CheckAdvCode()
	{
		var advCode = DomainFacade.Instance.AdvCodeService.GetAdvCodeFromAdvertisementCode(tbAdvCode.Text);
		var hasError = false;
		if ((advCode == null)
			&& (string.IsNullOrEmpty(tbAdvCode.Text.Trim()) == false))
		{
			hasError |= DisplayAdvCodeErrorMessages(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ADVCODE_NO_EXIST_ERROR)
				.Replace("@@ 1 @@", tbAdvCode.Text.Trim()));
		}
		return hasError;
	}

	/// <summary>
	/// 注文金額が決済種別で利用可能かチェック
	/// </summary>
	/// <param name="cart">カート</param>
	/// <returns>チェックOKか</returns>
	private bool CheckOrderPrice(CartObject cart)
	{
		return (DisplayPaymentErrorMessage(
			OrderCommon.CheckPaymentPriceEnabled(cart, cart.Payment.PaymentId)) == false);
	}

	/// <summary>
	/// 領収書情報エラーメッセージ表示
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <returns>メッセージ表示したか</returns>
	private bool DisplayReceiptErrorMessage(string errorMessage)
	{
		lReceiptErrorMessages.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		dvReceiptErrorMessages.Visible = (string.IsNullOrEmpty(errorMessage) == false);
		return dvReceiptErrorMessages.Visible;
	}

	/// <summary>
	/// 配送指定日時チェック＆更新
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <returns>チェック結果</returns>
	private bool CheckShippingDateTimeAndUpdateCartShipping(CartObject cart)
	{
		// 情報取得
		var shippingDateTime = new Hashtable
		{
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE, ddlShippingDate.SelectedValue },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME, ddlShippingTime.SelectedValue },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE + "_text",
				(ddlShippingDate.SelectedItem != null)
					? ddlShippingDate.SelectedItem.Text
					: null },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_TIME + "_text",
				(ddlShippingTime.SelectedItem != null)
					? ddlShippingTime.SelectedItem.Text
					: null },
			{ Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO, tbShippingCheckNo.Text }
		};

		// チェック
		var shippingInfoErrorMessage = Validator.Validate("OrderShippingRegistInput", shippingDateTime);
		lShippingInfoErrorMessages.Text = GetEncodedHtmlDisplayMessage(shippingInfoErrorMessage);
		dvDeliveryErrorMessage.Visible = (string.IsNullOrEmpty(shippingInfoErrorMessage) == false);

		// チェックが終わったら配送指定日のNULLを設定
		if (string.IsNullOrEmpty(ddlShippingDate.SelectedValue))
			shippingDateTime[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE] = cart.Shippings[0].ShippingDate;

		// CartShippingへ更新
		cart.GetShipping().UpdateShippingDateTime(
			dvShippingDate.Visible,
			dvShippingTime.Visible,
			(string.IsNullOrEmpty(ddlShippingDate.SelectedValue) == false)
				? DateTime.Parse(ddlShippingDate.SelectedValue)
				: (DateTime?)shippingDateTime[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE],
			ddlShippingTime.SelectedValue,
			(ddlShippingTime.SelectedItem != null)
				? ddlShippingTime.SelectedItem.Text
				: null);
		cart.GetShipping().ShippingCheckNo = tbShippingCheckNo.Text;

		// 配送方法、配送会社
		cart.GetShipping().ShippingMethod = ddlShippingMethod.SelectedValue;
		cart.GetShipping().DeliveryCompanyId = ddlDeliveryCompany.SelectedValue;
		if (string.IsNullOrEmpty(shippingInfoErrorMessage) == false) ddlShippingDate.Focus();
		return string.IsNullOrEmpty(shippingInfoErrorMessage);
	}

	/// <summary>
	/// Get Fixed Purchase Setting Inputs
	/// </summary>
	/// <returns>Fixed Purchase Setting Inputs</returns>
	private Hashtable GetFixedPurchaseSettingInputs()
	{
		var fixedPurchaseSettings = new Hashtable();
		if (rbMonthlyPurchase_Date.Checked)
		{
			fixedPurchaseSettings.Add(
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN,
				Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE);
			fixedPurchaseSettings.Add(
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1,
				string.Format("{0},{1}",
					ddlMonth.SelectedValue,
					ddlMonthlyDate.SelectedValue));
			fixedPurchaseSettings.Add(
				Constants.FIXED_PURCHASE_SETTING_MONTH,
				ddlMonth.SelectedValue);
			fixedPurchaseSettings.Add(
				Constants.FIXED_PURCHASE_SETTING_MONTHLY_DATE,
				ddlMonthlyDate.SelectedValue);
		}
		else if (rbMonthlyPurchase_WeekAndDay.Checked)
		{
			fixedPurchaseSettings.Add(
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN,
				Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY);
			fixedPurchaseSettings.Add(
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1,
				string.Format("{0},{1},{2}",
					ddlIntervalMonths.SelectedValue,
					ddlWeekOfMonth.SelectedValue,
					ddlDayOfWeek.SelectedValue));
			fixedPurchaseSettings.Add(
				Constants.FIXED_PURCHASE_SETTING_INTERVAL_MONTHS,
				ddlIntervalMonths.SelectedValue);
			fixedPurchaseSettings.Add(
				Constants.FIXED_PURCHASE_SETTING_WEEK_OF_MONTH,
				ddlWeekOfMonth.SelectedValue);
			fixedPurchaseSettings.Add(
				Constants.FIXED_PURCHASE_SETTING_DAY_OF_WEEK,
				ddlDayOfWeek.SelectedValue);
		}
		else if (rbRegularPurchase_IntervalDays.Checked)
		{
			fixedPurchaseSettings.Add(
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1,
				ddlIntervalDays.SelectedValue);
			fixedPurchaseSettings.Add(
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN,
				Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS);
			fixedPurchaseSettings.Add(
				Constants.FIXED_PURCHASE_SETTING_INTERVAL_DAYS,
				ddlIntervalDays.SelectedValue);
		}
		else if (rbPurchase_EveryNWeek.Checked)
		{
			fixedPurchaseSettings.Add(
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN,
				Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY);
			fixedPurchaseSettings.Add(
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1,
				string.Format("{0},{1}",
					ddlFixedPurchaseEveryNWeek_Week.SelectedValue,
					ddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue));
			fixedPurchaseSettings.Add(
				Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_WEEK,
				ddlFixedPurchaseEveryNWeek_Week.SelectedValue);
			fixedPurchaseSettings.Add(
				Constants.FIXED_PURCHASE_SETTING_EVERYNWEEK_DAY_OF_WEEK,
				ddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue);
		}
		else
		{
			fixedPurchaseSettings.Add(
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN,
				string.Empty);
			fixedPurchaseSettings.Add(
				Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1,
				string.Empty);
		}

		// 配送所要日数・最低配送間隔を格納, 次画面で利用
		fixedPurchaseSettings.Add(
			Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED,
			hfDaysRequired.Value);
		fixedPurchaseSettings.Add(
			Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_MINIMUM_SHIPPING_SPAN,
			hfMinSpan.Value);
		return fixedPurchaseSettings;
	}

	/// <summary>
	/// 定期購入設定チェック＆更新
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <returns>チェック結果</returns>
	private bool CheckFixedPurchaseSettingAndUpdateCartShipping(CartObject cart)
	{
		if (cart.HasFixedPurchase == false) return true;

		// 情報取得
		var fixedPurchaseSettings = GetFixedPurchaseSettingInputs();

		// 入力チェック
		var shippingFixedPurchaseErrorMessage = Validator.Validate("OrderShippingRegistInput", fixedPurchaseSettings);
		lFixedPurchasePatternErrorMessage.Text = GetEncodedHtmlDisplayMessage(shippingFixedPurchaseErrorMessage);
		dvFixedPurchasePatternErrorMessages.Visible = (shippingFixedPurchaseErrorMessage.Length != 0);

		if (string.IsNullOrEmpty(shippingFixedPurchaseErrorMessage))
		{
			// CartShippingへ更新
			var fixedPurchaseKbn = StringUtility.ToEmpty(
				fixedPurchaseSettings[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN]);
			var fixedPurchaseSetting = StringUtility.ToEmpty(
				fixedPurchaseSettings[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1]);
			var daysRequired = int.Parse(StringUtility.ToEmpty(
				fixedPurchaseSettings[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED]));
			var minSpan = int.Parse(StringUtility.ToEmpty(
				fixedPurchaseSettings[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_MINIMUM_SHIPPING_SPAN]));
			cart.GetShipping().UpdateFixedPurchaseSetting(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				daysRequired,
				minSpan);
			if ((this.IsOrderCombined == false) || ((this.IsOrderCombined) && (cart.GetShipping().ShippingDate == null)))
			{
				cart.GetShipping().ShippingDate = (string.IsNullOrEmpty(ddlShippingDate.SelectedValue) == false)
				? DateTime.Parse(ddlShippingDate.SelectedValue)
				: (DateTime?)null;
			}

			var calculationMode = rbPurchase_EveryNWeek.Checked
				? NextShippingCalculationMode.EveryNWeek
				: Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE;

			var cartShipping = cart.GetShipping();
			var service = DomainFacade.Instance.FixedPurchaseService;

			DateTime firstShippingDate;
			if (ddlFirstShippingDate.Visible)
			{
				firstShippingDate = DateTime.Parse(ddlFirstShippingDate.SelectedValue);
			}
			else
			{
				firstShippingDate = OrderCommon.GetFirstShippingDateBasedOnToday(
					cart.ShopId,
					daysRequired,
					cartShipping.ShippingDate,
					cartShipping.ShippingMethod,
					cartShipping.DeliveryCompanyId,
					cartShipping.ShippingCountryIsoCode,
					cartShipping.IsTaiwanCountryShippingEnable
						? cartShipping.Addr2
						: cartShipping.Addr1,
					cartShipping.Zip);
			}
			cartShipping.FirstShippingDate = firstShippingDate;

			DateTime nextShippingDate;
			if (ddlNextShippingDate.Visible)
			{
				nextShippingDate = DateTime.Parse(ddlNextShippingDate.SelectedValue);
			}
			else
			{
				nextShippingDate = service.CalculateNextShippingDate(
					fixedPurchaseKbn,
					fixedPurchaseSetting,
					firstShippingDate,
					daysRequired,
					minSpan,
					calculationMode);
			}
			var nextNextShippingDate = service.CalculateFollowingShippingDate(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				nextShippingDate,
				minSpan,
				calculationMode);
			cartShipping.UpdateNextShippingDates(nextShippingDate, nextNextShippingDate);

			if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED)
			{
				var cartFixedPurchaseNextShippingProduct = cart.Items
					.FirstOrDefault(cartProduct => cartProduct.CanSwitchProductFixedPurchaseNextShippingSecondTime());
				if (cartFixedPurchaseNextShippingProduct != null)
				{
					cart.GetShipping().CanSwitchProductFixedPurchaseNextShippingSecondTime = true;
					cart.GetShipping().UpdateNextShippingItemFixedPurchaseInfos(
						cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseKbn,
						cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseSetting);
					cart.GetShipping().CalculateNextShippingItemNextNextShippingDate();
				}
			}
		}
		else
		{
			rbMonthlyPurchase_WeekAndDay.Focus();
		}
		return (string.IsNullOrEmpty(shippingFixedPurchaseErrorMessage));
	}
	#endregion

	#region 表示更新処理
	/// <summary>
	/// 商品・金額表示更新
	/// </summary>
	/// <param name="reOrderItems">元注文の注文商品情報（再注文使用の場合）</param>
	/// <param name="isOrderCombined">注文同梱有無</param>
	private void RefreshItemsAndPriceView(List<Hashtable> reOrderItems = null, bool isOrderCombined = false)
	{
		// 注文アイテム取得
		var orderItems = reOrderItems ?? CreateOrderItemInput();

		// 頒布会数量系のチェック
		CheckSubscriptionBoxQuantity(orderItems);

		// 頒布会商品・価格チェック
		CheckSubscriptionBoxItemsAndPricesInput(orderItems);

		// 注文アイテム・価格入力チェック
		var itemErrorMessages = new StringBuilder(CheckItemsAndPricesInput(orderItems));
		if (DisplayItemsErrorMessages(itemErrorMessages.ToString()))
		{
			// 注文商品データバインド
			BindDataOrderItem(orderItems);
			CreateOrderItemRows();

			if (string.IsNullOrEmpty(tbCouponUse.Text.Trim()) == false)
				DisplayCouponErrorMessage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PLEASE_ENTER_PRODUCT_INFORMATION));
			if (tbPointUse.Text.Trim() != "0")
				this.DisplayPointErrorMessages(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PLEASE_ENTER_PRODUCT_INFORMATION));
			return;
		}

		// 決済手数料計算用
		var priceExchangeChangeBefore = ddlOrderPaymentKbn.SelectedValue;

		// カート作成
		var hasError = false;
		var cart = CreateCart(orderItems, ref hasError);
		cart.CreateOrderMemo(Constants.FLG_ORDER_MEMO_SETTING_DISP_FLG_PC);

		// 注文同梱画面からの遷移の場合、注文同梱元親注文の定期購入有無、定期購入回数を引き継ぐ(定期購入割引計算用)
		if (this.IsOrderCombinedAtOrderCombinePage)
		{
			// 注文同梱ページでの同梱の場合、注文同梱元注文IDをセット
			cart.OrderCombineParentOrderId = string.Format("{0},{1}",
				this.CombineParentOrderId,
				string.Join(",", this.CombineChildOrderIds));

			if (this.CombineParentOrderHasFixedPurchase)
			{
				cart.IsCombineParentOrderHasFixedPurchase = this.CombineParentOrderHasFixedPurchase;
				cart.CombineParentOrderFixedPurchaseOrderCount = this.CombineParentOrderCount;
				cart.FixedPurchaseDiscount = this.CombineFixedPurchaseDiscountPrice;
			}
			cart.CalculateWithCartShipping(isOrderCombined);
		}

		if (Constants.SETPROMOTION_OPTION_ENABLED)
		{
			// 画面から取得した商品リストが変わっている可能性があるので再作成
			orderItems = RecreateOrderItems(cart);
		}

		// ノベルティOP有効?
		if (Constants.NOVELTY_OPTION_ENABLED)
		{
			// カートリスト作成
			var cartList = new CartObjectList(cart.CartUserId, cart.OrderKbn, false);
			cartList.AddCartVirtural(cart);

			// カートノベルティリスト作成
			var cartNoveltyList = new CartNoveltyList(cartList);

			// 付与条件外のカート内ノベルティを削除
			cartList.RemoveNoveltyGrantItem(cartNoveltyList);

			// カートに追加された付与アイテムを含むカートノベルティを削除
			cartNoveltyList.RemoveCartNovelty(cartList);

			// Add Product Grant Novelty
			var cartNovelty = cartNoveltyList.GetCartNovelty(cart.CartId);
			var addedNovertyIds = AddProductGrantNovelty(cart, cartNovelty);

			// 画面から取得した商品リストが変わっている可能性があるので再作成
			orderItems = RecreateOrderItems(cart);

			// ノベルティフォームデータバインド
			var cartNoveltyData = cartNovelty
				.Where(item => (addedNovertyIds.Contains(item.NoveltyId) == false)).ToArray();
			var countLimitNovelty = cartNoveltyData.Sum(item => item.GrantItemList.Count());
			if (countLimitNovelty <= CONST_MAX_DISPLAY_SHOW_FOR_NOVELTY)
			{
				dvOrderNoveltyList.Attributes.Remove("class");
				AddAttributesForControlDisplay(
					dvOrderNoveltyList,
					CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
					"product-list novelty",
					true);
			}
			else
			{
				AddAttributesForControlDisplay(
					dvOrderNoveltyList,
					CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
					"product-list novelty scroll-vertical",
					true);
			}
			dvNovelty.Visible = (countLimitNovelty != 0);
			rNoveltyList.DataSource = cartNoveltyData;
			rNoveltyList.Visible = (cartNoveltyData.Length != 0);
			rNoveltyList.DataBind();

			cart.Calculate(false);
		}

		// 注文商品データバインド
		BindDataOrderItem(orderItems);
		CreateOrderItemRows();

		// 注文セットプロモーション情報データバインド
		rOrderSetPromotionProductDiscount.DataSource
			= rOrderSetPromotionShippingDiscount.DataSource
			= rOrderSetPromotionSettlementDiscount.DataSource
			= cart.SetPromotions;
		rOrderSetPromotionProductDiscount.DataBind();
		rOrderSetPromotionShippingDiscount.DataBind();
		rOrderSetPromotionSettlementDiscount.DataBind();

		// デジタルコンテンツありなしセット
		hfIsDigitalContents.Value = cart.HasDigitalContents
			? Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID
			: Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_INVALID;
		if (Constants.PAYMENT_NP_AFTERPAY_OPTION_ENABLED
			&& cart.IsDigitalContentsOnly)
		{
			ddlOrderPaymentKbn.Items.Remove(
				ddlOrderPaymentKbn.Items.FindByValue(
					Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY));
		}

		// Check for error when refresh shipping option
		RefreshShippingOption();

		// カート商品チェック
		itemErrorMessages.Append(CheckOrderItemsAndPrices(cart));
		if (DisplayItemsErrorMessages(itemErrorMessages.ToString())
			&& (this.IsOrderCombined == false)) return;

		// 配送希望日時＆定期購入配送パターンドロップダウンセット
		var shippingTypeAlertMessages = RefreshViewFromShippingType(cart);
		if (string.IsNullOrEmpty(shippingTypeAlertMessages) == false)
		{
			if (itemErrorMessages.Length != 0) itemErrorMessages.Append("<br />");
			itemErrorMessages.Append(shippingTypeAlertMessages);
		}

		// エラー表示
		DisplayItemsErrorMessages(itemErrorMessages.ToString());

		// 各種金額セット
		lOrderPriceShipping.Text = GetEncodedHtmlDisplayMessage(cart.PriceShipping.ToPriceString(true));
		lPriceSubTotal.Text = GetEncodedHtmlDisplayMessage(cart.PriceSubtotal.ToPriceString(true));

		// 会員ランク割引額設定
		lMemberRankDiscount.Text = GetEncodedHtmlDisplayMessage(
			(cart.MemberRankDiscount * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trMemberRankDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(cart.MemberRankDiscount > 0));

		// 定期購入割引額設定
		lFixedPurchaseDiscountPrice.Text = GetEncodedHtmlDisplayMessage(
			(cart.FixedPurchaseDiscount * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trFixedPurchaseDiscountPrice,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(cart.FixedPurchaseDiscount > 0));

		// 決済情報が変更される場合は、決済手数料を再計算して最新決済情報をセットする
		if (priceExchangeChangeBefore != ddlOrderPaymentKbn.SelectedValue)
		{
			cart.Payment = GetCartPayment(cart);
			dvPaymentErrorMessages.Visible = false;
		}

		// Refresh payment and Tax Rate if there are no products
		if (orderItems.Count == 0)
		{
			cart.Payment = null;
			cart.PriceInfoByTaxRate.Clear();
		}
		// 決済手数料セット
		var dOrderPriceExchange = (cart.Payment != null)
			? cart.Payment.PriceExchange
			: 0m;
		lOrderPriceExchange.Text = GetEncodedHtmlDisplayMessage(dOrderPriceExchange.ToPriceString(true));

		// 調整金額
		tbOrderPriceRegulation.Text = GetEncodedHtmlDisplayMessage(cart.PriceRegulation.ToPriceString());

		// 定期会員割引額
		lFixedPurchaseMemberDiscountAmount.Text = GetEncodedHtmlDisplayMessage(
			(cart.FixedPurchaseMemberDiscountAmount * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trFixedPurchaseMemberDiscountAmount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(cart.FixedPurchaseMemberDiscountAmount > 0));

		// 消費税額セット
		lbOrderPriceTax.Text = GetEncodedHtmlDisplayMessage(cart.PriceSubtotalTax.ToPriceString(true));

		// 支払合計セット
		var orderPriceTotalForDisplay = lOrderPriceTotalBottom.Text;
		lOrderPriceTotal.Text = GetEncodedHtmlDisplayMessage(cart.PriceTotal.ToPriceString(true));
		if (this.IsOrderCombinedAtOrderRegistPage == false)
		{
			lOrderPriceTotalBottom.Text = cart.PriceTotal.ToPriceString(true);
		}
		AddAttributesForControlDisplay(
			trOrderPriceTotal,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(cart.PriceTotal < 0));

		rTotalPriceByTaxRate.DataSource = cart.PriceInfoByTaxRate;
		rTotalPriceByTaxRate.DataBind();

		// ポイント情報画面セット
		var pointBuy = (cart.BuyPoint + cart.FirstBuyPoint);
		lPointBuy.Text = GetEncodedHtmlDisplayMessage(
			StringUtility.ToNumeric((pointBuy > 0) ? pointBuy : 0));
		lPointUsePrice.Text = GetEncodedHtmlDisplayMessage(
			(cart.UsePointPrice * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trPointDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(cart.UsePointPrice > 0));

		// クーポン情報画面セット
		lCouponUsePrice.Text = GetEncodedHtmlDisplayMessage((cart.UseCouponPrice * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trCouponDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(cart.UseCouponPrice > 0));
		lCouponDiscount.Text = GetEncodedHtmlDisplayMessage((cart.Coupon != null)
			? CouponOptionUtility.GetCouponDiscountString(cart.Coupon)
			: string.Empty);
		lCouponName.Text = GetEncodedHtmlDisplayMessage((cart.Coupon != null)
			? cart.Coupon.CouponName
			: string.Empty);
		lCouponDispName.Text = GetEncodedHtmlDisplayMessage((cart.Coupon != null)
			? cart.Coupon.CouponDispName
			: string.Empty);
		dvShowCouponExist.Visible = (cart.Coupon != null);

		// 定期配送パターンセット(注文同梱の場合はセット済みなのでセットしない)
		dvShippingFixedPurchase.Visible
			= ((this.CombineParentOrderHasFixedPurchase == false)
				&& cart.HasFixedPurchase)
			|| (cart.HasSubscriptionBox
				&& (cart.IsOrderCombinedWithSameSubscriptionBoxCourse() == false)
				&& (this.IsOrderCombinedAtOrderCombinePage == false));

		var shopShipping = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, cart.ShippingType);
		if (shopShipping != null)
		{
			if (dvShippingFixedPurchase.Visible
				&& (this.IsUpdateFixedPurchaseShippingPattern || this.IsUpdatePayment))
			{
				SetFirstAndNextShippingDate(cart, shopShipping);
			}

			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
			{
				var shippingKbn = ddlUserShipping.SelectedValue;
				BindShippingList(this.UserShippingAddress, shopShipping);
				var isChangeProductRelateToProductNotRelate = (ddlUserShipping.Items.FindByValue(shippingKbn) == null);

				// If Not Change Product Relate To Product Not Relate Then Set User Shipping Kbn
				if (isChangeProductRelateToProductNotRelate == false)
				{
					ddlUserShipping.SelectedValue = shippingKbn;
				}
				else
				{
					RefreshShippingCompanyViewFromShippingType(shopShipping);
				}

				if (this.IsBackFromConfirm == false)
				{
					RefreshShippingDateViewFromShippingType(
						shopShipping,
						cart,
						false,
						isChangeProductRelateToProductNotRelate);
				}
			}
			else
			{
				this.ShopShipping = shopShipping;
				if ((this.IsCanAddShippingDate == false)
					&& ddlShippingDate.Visible
					&& ddlFirstShippingDate.Visible
					&& ddlShippingDate.Items[0].Text == lFirstShippingDate.Text)
				{
					ddlShippingDate.Items.RemoveAt(0);
				}
				dvShippingDate.Visible = shopShipping.IsValidShippingDateSetFlg && (this.IsCanAddShippingDate == false);
			}

			trSettlementAmount.Visible = ((cart.Payment != null)
				&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE));
			lSettlementAmount.Text = GetEncodedHtmlDisplayMessage((cart.Payment != null)
				? CreateSettlementAmount(
					cart.Payment.PaymentId,
					cart.PriceTotal,
					trSettlementAmount.Visible)
				: string.Empty);
		}

		// 定期購入ありの場合「決済なし」を削除
		if (cart.HasFixedPurchase)
		{
			ddlOrderPaymentKbn.Items.Remove(ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT));
		}

		// Display the limited payment messages
		DisplayLimitedPaymentMessages();

		dvShowDeliveryDesignation.Visible
			= dvShowOrderCombine.Visible
			= dvShowOrderPayment.Visible = (this.HasItems || this.HasOwner);
		dvHideOrderPayment.Visible
			= dvHideDeliveryDesignation.Visible
			= dvHideOrderCombine.Visible = ((this.HasItems == false) && (this.HasOwner == false));

		// Set display or non-display for point and coupon
		SetDisplayForPointAndCoupon();

		// Get gmo cvs type
		GetGmoCvsType();

		// Get rakuten cvs type
		GetRakutenCvsType();

		ddlOrderPaymentKbn.Items.Remove(ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET));
		ddlOrderPaymentKbn.Items.Remove(ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_ATM));
		if (Constants.GLOBAL_OPTION_ENABLE
			&& (Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent))
		{
			ddlOrderPaymentKbn.Items.Remove(ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY));
		}
	}

	/// <summary>
	/// 注文商品リスト再作成
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <returns>注文商品リスト</returns>
	private List<Hashtable> RecreateOrderItems(CartObject cart)
	{
		var orderItems = new List<Hashtable>();

		// 通常商品情報作成
		foreach (var cartProduct in cart.Items.Where(
			cartProduct => (cartProduct.QuantitiyUnallocatedToSet != 0)))
		{
			var item = RecreateOrderItem(cartProduct);
			item.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY, cartProduct.QuantitiyUnallocatedToSet);
			item.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE, (cartProduct.Price * cartProduct.QuantitiyUnallocatedToSet));
			item.Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, string.Empty);
			item.Add(Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME, string.Empty);
			orderItems.Add(item);
		}

		// セットプロモーション商品情報作成
		foreach (CartSetPromotion setpromotion in cart.SetPromotions)
		{
			foreach (var setPromotionItem in setpromotion.Items)
			{
				var item = RecreateOrderItem(setPromotionItem);
				item.Add(Constants.FIELD_ORDERITEM_ITEM_QUANTITY,
					setPromotionItem.QuantityAllocatedToSet[setpromotion.CartSetPromotionNo]);
				item.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE,
					(setPromotionItem.Price * setPromotionItem.QuantityAllocatedToSet[setpromotion.CartSetPromotionNo]));
				item.Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, setpromotion.CartSetPromotionNo.ToString());
				item.Add(Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME, setpromotion.SetpromotionName);
				orderItems.Add(item);
			}
		}
		return orderItems;
	}

	/// <summary>
	/// 注文商品情報再作成
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>注文商品情報</returns>
	private Hashtable RecreateOrderItem(CartProduct product)
	{
		var item = new Hashtable
		{
			{ Constants.FIELD_ORDERITEM_SHOP_ID, this.LoginOperatorShopId },
			{ Constants.FIELD_ORDERITEM_PRODUCT_ID, product.ProductId },
			{ Constants.FIELD_ORDERITEM_SUPPLIER_ID, product.SupplierId },
			{ Constants.FIELD_ORDERITEM_VARIATION_ID, product.VariationId },
			{ Constants.FIELD_PRODUCTVARIATION_V_ID, product.VId },
			{ Constants.FIELD_ORDERITEM_PRODUCT_NAME, product.ProductJointName },
			{ Constants.FIELD_ORDERITEM_PRODUCT_PRICE, product.Price },
			{ Constants.KEY_OPTION_INCLUDED_ITEM_PRICE,product.ProductOptionSettingList.SelectedOptionTotalPrice},
			{ Constants.FIELD_ORDERITEM_PRODUCTSALE_ID, product.ProductSaleId },
			{ CONST_KEY_FIXED_PURCHASE, (Constants.FIXEDPURCHASE_OPTION_ENABLED ? product.IsFixedPurchase : false) },
			{ CONST_KEY_PRODUCT_OPTION_VALUE_SETTINGS, product.ProductOptionSettingList },
			{ Constants.FIELD_ORDERITEM_NOVELTY_ID, product.NoveltyId },
			{ Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING, product.LimitedFixedPurchaseKbn1Setting },
			{ Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING, product.LimitedFixedPurchaseKbn3Setting },
			{ Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING, product.LimitedFixedPurchaseKbn4Setting },
			{ Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE, product.TaxRate },
			{ Constants.FIELD_PRODUCT_IMAGE_HEAD, product.ProductVariationImageHead },
			{ Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE, product.FixedPurchaseDiscountValue.ToString() },
			{ Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_TYPE, product.FixedPurchaseDiscountType },
			{ Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID, product.SubscriptionBoxCourseId },
			{ Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT, product.SubscriptionBoxFixedAmount },
			{ CONST_HASHKEY_CART_PRODUCT_IS_ORDER_COMBINED, product.IsOrderCombine },
		};
		return item;
	}

	/// <summary>
	/// 配送種別に紐付く表示の更新
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <rereturns>アラートメッセージ</rereturns>
	private string RefreshViewFromShippingType(CartObject cart)
	{
		var alertMessages = new StringBuilder();

		// 配送種別または定期購入有無が更新されたら配送指定情報更新(注文同梱の場合は例外)
		var shippingTypeChanged = (hfShippingType.Value != cart.ShippingType);
		var isChangedToRegularPurchase = bool.Parse(hfHasFixedPurchase.Value)
			|| ((dvShippingFixedPurchase.Visible == false)
				&& cart.HasFixedPurchase
				&& (this.CombineParentOrderHasFixedPurchase == false)
				&& (this.IsOrderCombinedAtOrderCombinePage == false));

		var shippingKbnChanged = (hfShippingKbn.Value != this.ddlShippingKbnList.SelectedValue);
		if (shippingTypeChanged
			|| isChangedToRegularPurchase
			|| shippingKbnChanged)
		{
			// 配送種別が変更された？
			if (shippingTypeChanged
				&& ((ddlShippingDate.SelectedValue.Length + ddlShippingTime.SelectedValue.Length) != 0))
			{
				alertMessages.Append(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_SHIPPING_KBN_CHANGED));
			}

			// 定期購入有りに変更された？
			if (isChangedToRegularPurchase && cart.HasFixedPurchase)
			{
				alertMessages.Append(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_FIXED_PURCHASE_CHANGED));
			}
			hfShippingType.Value = cart.ShippingType;
			hfShippingKbn.Value = this.ddlShippingKbnList.SelectedValue;

			// 配送種別取得
			var shopShipping = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, cart.ShippingType);
			if (shopShipping == null)
			{
				ddlDeliveryCompany.Items.Clear();
				return string.Empty;
			}

			// 配送指定更新
			if (shippingTypeChanged)
			{
				RefreshDeliverySpecificationFromShippingType(cart, shopShipping, shippingTypeChanged);
			}

			// 定期購入情報更新
			RefreshFixedPurchaseViewFromShippingType(cart, shopShipping);

			// 決済情報更新
			RefreshPaymentViewFromShippingType(cart, shopShipping);
		}

		// デジタルコンテンツなら配送情報を隠す
		if (this.IsDigitalContents)
		{
			this.ddlShippingKbnList.SelectedValue = Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER;
		}
		if (this.IsDigitalContents == false)
		{
			AddAttributesForControlDisplay(
				dvShippingTo,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_EMPTY,
				true);
			AddAttributesForControlDisplay(
				dvShippingInfo,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_EMPTY,
				true);
		}
		else
		{
			AddAttributesForControlDisplay(
				dvShippingTo,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_NONE,
				true);
			AddAttributesForControlDisplay(
				dvShippingInfo,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_NONE,
				true);
		}

		if (this.IsOrderCombinedAtOrderRegistPage)
		{
			AddAttributesForControlDisplay(
				dvShippingTo,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_NONE,
				true);
			AddAttributesForControlDisplay(
				dvShippingInfo,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_NONE,
				true);
			AddAttributesForControlDisplay(
				dvPayment,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_NONE,
				true);
		}
		return alertMessages.ToString();
	}

	/// <summary>
	/// 配送指定を更新（配送会社、配送指定日、配送指定時間帯）
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="isChangedShippingService">配送サービスの変更が行われたか</param>
	private void RefreshDeliverySpecificationFromShippingType(
		CartObject cart,
		ShopShippingModel shopShipping,
		bool isChangedShippingService = false)
	{
		// 配送会社情報更新
		RefreshShippingCompanyViewFromShippingType(shopShipping);

		// 配送指定日更新
		RefreshShippingDateViewFromShippingType(shopShipping, cart, isChangedShippingService);

		var deliveryCompanyId = GetDeliveryCompanyId(cart.GetShipping());
		var deliveryCompany = this.DeliveryCompanyList
			.FirstOrDefault(item => (item.DeliveryCompanyId == deliveryCompanyId));
		if (deliveryCompany != null)
		{
			// 配送指定時間帯更新
			RefreshShippingTimeViewFromShippingType(deliveryCompany, cart.IsExpressDelivery, isChangedShippingService);
		}
	}

	/// <summary>
	/// 決済情報表示を更新する（配送種別から）
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	private void RefreshPaymentViewFromShippingType(CartObject cart, ShopShippingModel shopShipping)
	{
		var payments = GetPaymentValidListPermission(
			this.LoginOperatorShopId,
			shopShipping.PaymentSelectionFlg,
			shopShipping.PermittedPaymentIds,
			cart.HasFixedPurchase);
		payments = payments
			.Where(item => (OrderCommon.CheckPaymentYamatoKaSms(item.PaymentId) == false)
				&& (item.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
			.ToArray();
		var selectedPaymentKbn = ddlOrderPaymentKbn.SelectedValue;

		ddlOrderPaymentKbn.Items.Clear();
		foreach (var payment in payments)
		{
			if ((cart.IsExpressDelivery == false)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)) continue;
			if (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS) continue;
			if (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) continue;
			if (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2) continue;
			if ((this.IsOrderCombined == false)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)) continue;
			if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
				&& (this.IsOrderCombined == false)) continue;
			if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
				&& (this.IsOrderCombined == false)) continue;
			if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
				&& (cart.IsDigitalContentsOnly)) continue;
			if ((this.IsOrderCombined == false)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)) continue;
			if ((this.IsOrderCombined == false)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)) continue;
			if ((this.IsOrderCombined == false)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)) continue;

			var isUserShippingConvenienceStore = false;
			if ((ddlUserShipping.SelectedValue != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
				&& (ddlUserShipping.SelectedValue != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW))
			{
				var userShipping = this.UserShippingAddress.FirstOrDefault(item => item.ShippingNo.ToString() == ddlUserShipping.SelectedValue);
				isUserShippingConvenienceStore = (userShipping != null)
					&& (userShipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);
			}

			if ((ddlUserShipping.SelectedValue != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
				&& (isUserShippingConvenienceStore == false)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE))
			{
				continue;
			}

			if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
				&& (IsPaymentAtConvenienceStore(ddlShippingReceivingStoreType.SelectedValue)
					&& (payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
				|| ((IsPaymentAtConvenienceStore(ddlShippingReceivingStoreType.SelectedValue) == false)
					&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE))))
			{
				continue;
			}

			if ((this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_STORE_PICKUP)
				&& ((Constants.SETTING_CAN_STORE_PICKUP_OPTION_PAYMENT_IDS.Contains(payment.PaymentId) == false)
					|| Constants.SETTING_CAN_NOT_STORE_PICKUP_OPTION_PAYMENT_IDS.Contains(payment.PaymentId)))
			{
				continue;
			}

			var items = new ListItem(payment.PaymentName, payment.PaymentId);
			items.Selected = (payment.PaymentId == selectedPaymentKbn);
			ddlOrderPaymentKbn.Items.Add(items);
			if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				&& (ddlUserCreditCard.Items.Count == 0))
			{
				ddlUserCreditCard.Items.Add(
					new ListItem(
						ReplaceTag("@@DispText.credit_card_list.new@@"),
						CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW,
						this.IsNotRakutenAgency));
			}
		}

		dvPaymentKbnCredit.Visible = (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
	}

	/// <summary>
	/// 配送会社を更新する（配送種別から）
	/// </summary>
	/// <param name="shopShipping">配送種別情報</param>
	private void RefreshShippingCompanyViewFromShippingType(ShopShippingModel shopShipping)
	{
		ddlDeliveryCompany.Items.Clear();

		var isShippingConvenience = (ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE);
		var shippingNo = 0;
		if ((isShippingConvenience == false)
			&& int.TryParse(ddlUserShipping.SelectedValue, out shippingNo))
		{
			var userShipping = this.UserShippingAddress
				.FirstOrDefault(shipping => (shipping.ShippingNo == shippingNo));
			if (userShipping != null)
			{
				isShippingConvenience = (userShipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);
			}
		}
		var companyList = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
			? shopShipping.CompanyListExpress
			: shopShipping.CompanyListMail;
		companyList = isShippingConvenience
			? companyList.Where(company =>
				(company.DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray()
			: companyList.Where(company =>
				(company.DeliveryCompanyId != Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)).ToArray();

		var companyItemList = this.DeliveryCompanyList
			.Where(company => companyList.Any(c => company.DeliveryCompanyId == c.DeliveryCompanyId))
			.Select(company => new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId));
		ddlDeliveryCompany.Items.AddRange(companyItemList.ToArray());

		// メール便配送サービスエスカレーション処理
		DeliveryCompanyMailEscalation(shopShipping);
		if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED
			&& (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)) return;
		// 初期設定の配送会社を選択させる
		var defaultCompany = companyList.FirstOrDefault(item => item.IsDefault);
		if ((defaultCompany != null)
			&& (ddlDeliveryCompany.Items.FindByValue(defaultCompany.DeliveryCompanyId) != null))
		{
			ddlDeliveryCompany.SelectedValue = companyList.First(item => item.IsDefault).DeliveryCompanyId;
		}
	}

	/// <summary>
	/// メール便配送サービスエスカレーション処理
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	private void DeliveryCompanyMailEscalation(ShopShippingModel shopShipping, CartObject cart = null)
	{
		if ((Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED == false)
			|| (ddlShippingMethod.SelectedValue != Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)) return;

		if (cart != null) this.ProductsSizeForDeliveryCompanyMailEscalation = cart.Items.Sum(item => item.ProductSizeFactor * item.Count);
		var defaultCompanyId = OrderCommon.GetDeliveryCompanyId(shopShipping.CompanyListMail, this.ProductsSizeForDeliveryCompanyMailEscalation);
		if (string.IsNullOrEmpty(defaultCompanyId) == false)
		{
			ddlDeliveryCompany.SelectedValue = defaultCompanyId;
		}
		else
		{
			ddlShippingMethod.SelectedValue = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
		}
	}

	/// <summary>
	/// 配送日帯表示を更新する（配送種別から）
	/// </summary>
	/// <param name="shopShipping">配送種別情報</param>
	/// <param name="cart">Cart</param>
	/// <param name="isChangedShippingService">配送サービスに変更があったか</param>
	/// <param name="isChangeProductRelateToProductNotRelate">Is change product relate to product not relate</param>
	private void RefreshShippingDateViewFromShippingType(
		ShopShippingModel shopShipping,
		CartObject cart,
		bool isChangedShippingService = false,
		bool isChangeProductRelateToProductNotRelate = false)
	{
		RefreshShippingDateViewFromShippingType(
			shopShipping,
			cart.IsExpressDelivery,
			cart.GetShipping().ConvenienceStoreFlg,
			cart.Shippings[0].FixedPurchaseKbn,
			isChangedShippingService,
			isChangeProductRelateToProductNotRelate);
	}
	/// <summary>
	/// 配送日帯表示を更新する（配送種別から）
	/// </summary>
	/// <param name="shopShipping">配送種別情報</param>
	/// <param name="isExpressDelivery">Is express delivery</param>
	/// <param name="convenienceStoreFlg">Convenience store flag</param>
	/// <param name="isChangedShippingService">配送サービスに変更があったか</param>
	/// <param name="isChangeProductRelateToProductNotRelate">Is change product relate to product not relate</param>
	private void RefreshShippingDateViewFromShippingType(
		ShopShippingModel shopShipping,
		bool isExpressDelivery,
		string convenienceStoreFlg,
		string fixedPurchaseKbn,
		bool isChangedShippingService = false,
		bool isChangeProductRelateToProductNotRelate = false)
	{
		if (shopShipping.IsValidShippingDateSetFlg
			&& isExpressDelivery
			&& isChangedShippingService)
		{
			ddlShippingDate.Items.Clear();

			if ((convenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
				&& (isChangeProductRelateToProductNotRelate == false))
			{
				ddlShippingDate.Items.AddRange(OrderCommon.GetShippingDateForConvenienceStore().Cast<ListItem>().ToArray());
			}
			else
			{
				ddlShippingDate.Items.AddRange(OrderCommon.GetListItemShippingDate(shopShipping).Cast<ListItem>().ToArray());
			}
		}
		var canUseFirstShippingDate = shopShipping.CanUseFixedPurchaseFirstShippingDateNextMonth(fixedPurchaseKbn);
		dvShippingDate.Visible = shopShipping.IsValidShippingDateSetFlg
			&& isExpressDelivery
			&& (canUseFirstShippingDate == false);
		if ((shopShipping.IsValidShippingDateSetFlg == false) || (isExpressDelivery == false))
		{
			ddlShippingDate.Items.Clear();
		}
	}

	/// <summary>
	/// 配送時間帯表示を更新する（配送種別から）
	/// </summary>
	/// <param name="deliveryCompany">配送種別情報</param>
	/// <param name="isExpress">Is Express</param>
	/// <param name="isChangedShippingService">配送サービスに変更があったか</param>
	private void RefreshShippingTimeViewFromShippingType(
		DeliveryCompanyModel deliveryCompany,
		bool isExpress,
		bool isChangedShippingService = false)
	{
		dvShippingTime.Visible = false;

		var shippingTimeList = GetShippingTimeList(deliveryCompany).Cast<ListItem>().ToArray();
		if ((isExpress == false)
			|| shippingTimeList.Length <= 0) return;

		if (isChangedShippingService)
		{
			ddlShippingTime.Items.Clear();
			ddlShippingTime.Items.Add(string.Empty);
			ddlShippingTime.Items.AddRange(shippingTimeList);
		}

		dvShippingTime.Visible = true;
	}

	/// <summary>
	/// 定期購入情報を更新する（配送種別から）
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	private void RefreshFixedPurchaseViewFromShippingType(CartObject cart, ShopShippingModel shopShipping)
	{
		if (cart.HasFixedPurchase)
		{
			dtMonthlyPurchase_Date.Visible
				= ddMonthlyPurchase_Date.Visible
				= shopShipping.IsValidFixedPurchaseKbn1Flg;
			dtMonthlyPurchase_WeekAndDay.Visible
				= ddMonthlyPurchase_WeekAndDay.Visible
				= shopShipping.IsValidFixedPurchaseKbn2Flg;
			dtRegularPurchase_IntervalDays.Visible
				= ddRegularPurchase_IntervalDays.Visible
				= shopShipping.IsValidFixedPurchaseKbn3Flg;
			dtPurchase_EveryNWeek.Visible
				= ddPurchase_EveryNWeek.Visible
					= shopShipping.IsValidFixedPurchaseKbn4Flg;

			ddlMonth.Items.Clear();
			ddlMonth.Items.AddRange(
				OrderCommon.GetKbn1FixedPurchaseIntervalListItems(
					shopShipping.FixedPurchaseKbn1Setting,
					string.Empty));

			ddlMonthlyDate.Items.Clear();
			if (string.IsNullOrEmpty(shopShipping.FixedPurchaseKbn1Setting2))
			{
				ddlMonthlyDate.Items.AddRange(ValueText.GetValueItemArray(
					Constants.TABLE_SHOPSHIPPING,
					Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST));
			}
			else
			{
				ddlMonthlyDate.Items.AddRange(GetFixedPurchaseKbnIsDays(shopShipping.FixedPurchaseKbn1Setting2));
			}

			ddlIntervalMonths.Items.Clear();
			ddlIntervalMonths.Items.AddRange(
				OrderCommon.GetKbn1FixedPurchaseIntervalListItems(
					shopShipping.FixedPurchaseKbn1Setting,
					string.Empty));

			ddlWeekOfMonth.Items.Clear();
			ddlWeekOfMonth.Items.AddRange(ValueText.GetValueItemArray(
				Constants.TABLE_SHOPSHIPPING,
				Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_WEEK_LIST));

			ddlDayOfWeek.Items.Clear();
			ddlDayOfWeek.Items.AddRange(ValueText.GetValueItemArray(
				Constants.TABLE_SHOPSHIPPING,
				Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DAY_LIST));

			ddlIntervalDays.Items.Clear();
			ddlIntervalDays.Items.AddRange(
				OrderCommon.GetKbn3FixedPurchaseIntervalListItems(
					shopShipping.FixedPurchaseKbn3Setting
						.Replace("(", string.Empty).Replace(")", string.Empty),
					string.Empty));

			ddlFixedPurchaseEveryNWeek_Week.Items.Clear();
			ddlFixedPurchaseEveryNWeek_Week.Items.AddRange(
				OrderCommon.GetKbn4Setting1FixedPurchaseIntervalListItems(
					shopShipping.FixedPurchaseKbn4Setting1
						.Replace("(", string.Empty).Replace(")", string.Empty),
					string.Empty));

			ddlFixedPurchaseEveryNWeek_DayOfWeek.Items.Clear();
			ddlFixedPurchaseEveryNWeek_DayOfWeek.Items.AddRange(
				OrderCommon.GetKbn4Setting2FixedPurchaseIntervalListItems(
					shopShipping.FixedPurchaseKbn4Setting2,
					string.Empty));

			hfDaysRequired.Value = shopShipping.FixedPurchaseShippingDaysRequired.ToString();
			hfMinSpan.Value = shopShipping.FixedPurchaseMinimumShippingSpan.ToString();
		}

		dvShippingFixedPurchase.Visible
			= ((this.CombineParentOrderHasFixedPurchase == false)
				&& cart.HasFixedPurchase)
			|| (cart.HasSubscriptionBox
				&& (cart.IsOrderCombinedWithSameSubscriptionBoxCourse() == false)
				&& (this.IsOrderCombinedAtOrderCombinePage == false));
		if (dvShippingFixedPurchase.Visible)
		{
			AdjustFirstAndNextShippingDate(cart, shopShipping);
			if (this.IsUpdateFixedPurchaseShippingPattern == false)
			{
				lFirstShippingDate.Text = string.Empty;
				ddlFirstShippingDate.Visible = false;
				lNextShippingDate.Text = string.Empty;
				ddlNextShippingDate.Visible = false;
			}
		}
	}

	/// <summary>
	/// 注文者エラーメッセージ表示
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <returns>メッセージ表示したか</returns>
	private bool DisplayOwnerErrorMessages(string errorMessage)
	{
		lOrderOwnerErrorMessages.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		dvOrderOwnerErrorMessages.Visible = (string.IsNullOrEmpty(errorMessage) == false);
		return dvOrderOwnerErrorMessages.Visible;
	}

	/// <summary>
	/// 注文配送先エラーメッセージ表示
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <returns>メッセージ表示したか</returns>
	private bool DisplayShippingErrorMessages(string errorMessage)
	{
		lOrderShippingErrorMessages.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		dvOrderShippingErrorMessages.Visible = ((this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_USER_INPUT)
			&& (string.IsNullOrEmpty(errorMessage) == false));
		return dvOrderShippingErrorMessages.Visible;
	}

	/// <summary>
	/// 広告コード情報エラーメッセージ表示
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <returns>メッセージ表示したか</returns>
	private bool DisplayAdvCodeErrorMessages(string errorMessage)
	{
		lAdvCodeErrorMessages.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		dvAdvCodeErrorMessage.Visible = true;
		return dvAdvCodeErrorMessage.Visible;
	}

	/// <summary>
	/// 注文商品エラーメッセージ表示
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <returns>メッセージ表示したか</returns>
	private bool DisplayItemsErrorMessages(string errorMessage)
	{
		lOrderItemErrorMessages.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		dvOrderItemErrorMessage.Visible = (string.IsNullOrEmpty(errorMessage) == false);
		return dvOrderItemErrorMessage.Visible;
	}

	/// <summary>
	/// ポイントエラーメッセージ表示
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <returns>メッセージ表示したか</returns>
	private bool DisplayPointErrorMessages(string errorMessage)
	{
		lPointErrorMessage.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		dvPointErrorMessage.Visible = (string.IsNullOrEmpty(errorMessage) == false);
		return dvPointErrorMessage.Visible;
	}

	/// <summary>
	/// クーポンエラーメッセージ表示
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <returns>メッセージ表示したか</returns>
	private bool DisplayCouponErrorMessage(string errorMessage)
	{
		lCouponErrorMessage.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		dvCouponErrorMessage.Visible = (string.IsNullOrEmpty(errorMessage) == false);
		return dvCouponErrorMessage.Visible;
	}

	/// <summary>
	/// 注文拡張項目エラーメッセージ表示
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <returns>メッセージ表示したか</returns>
	private bool DisplayOrderExtendErrorMessages(string errorMessage)
	{
		lOrderExtendErrorMessages.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		dvOrderExtendErrorMessages.Visible = (string.IsNullOrEmpty(errorMessage) == false);
		return dvOrderExtendErrorMessages.Visible;
	}

	/// <summary>
	/// 決済情報エラーメッセージ表示
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	/// <returns>メッセージ表示したか</returns>
	private bool DisplayPaymentErrorMessage(string errorMessage)
	{
		lPaymentErrorMessage.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		dvPaymentErrorMessages.Visible = (string.IsNullOrEmpty(errorMessage) == false);
		if (dvPaymentErrorMessages.Visible) ddlOrderPaymentKbn.Focus();
		return dvPaymentErrorMessages.Visible;
	}

	/// <summary>
	/// Display Uniform Invoice Error Message
	/// </summary>
	/// <param name="errorMessage">Error Message</param>
	/// <returns>Display Error Message</returns>
	private bool DisplayUniformInvoiceErrorMessage(string errorMessage)
	{
		lElectronicBillErrorMessages.Text = GetEncodedHtmlDisplayMessage(errorMessage);
		dvElectronicBillErrorMessages.Visible = (string.IsNullOrEmpty(errorMessage) == false);
		if (dvElectronicBillErrorMessages.Visible) ddlUniformInvoiceType.Focus();
		return dvElectronicBillErrorMessages.Visible;
	}
	#endregion

	/// <summary>
	/// 注文者情報取得
	/// </summary>
	/// <param name="userId">ユーザーID</param>
	/// <param name="orgOrderOwner">元注文の注文者情報（再注文使用の場合）</param>
	private void SetOwner(string userId, OrderOwner orgOrderOwner = null)
	{
		var user = DomainFacade.Instance.UserService.Get(userId);

		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			// Adjust point and member rank by Cross Point api
			UserUtility.AdjustPointAndMemberRankByCrossPointApi(user);
		}

		dvSearchUser.Visible = (this.IsOrderCombined == false);
		if (user != null)
		{
			lbUserClear.Visible = (this.IsOrderCombined == false);
			lUserId.Text = GetEncodedHtmlDisplayMessage(user.UserId);
			lUserIdNonSet.Text = string.Empty;
			hfUserId.Value = user.UserId;
			hfMemberRankId.Value = user.MemberRankId;
			lMemberRankName.Text = GetEncodedHtmlDisplayMessage(MemberRankOptionUtility.GetMemberRankName(user.MemberRankId));
			hfFixedPurchaseMember.Value = user.FixedPurchaseMemberFlg;
			lOwnerTel2.Text = GetEncodedHtmlDisplayMessage(user.Tel2);

			dvMemberRank.Visible = this.IsAvailableRank;
			if (this.IsAvailableRank)
			{
				lUserMemberRankName.Text =
					GetEncodedHtmlDisplayMessage(MemberRankOptionUtility.GetMemberRankName(user.MemberRankId));
				lUserMemberRankBenefit.Text =
					GetEncodedHtmlDisplayMessage(MemberRankBenefitWording.CreateBenefitString(user.MemberRankId));
				lUserMemberRankMemo.Text =
					GetEncodedHtmlDisplayMessage(
						MemberRankBenefitWording.CreateBenefitStringMemo(user.MemberRankId));
				dvMemberRankMemo.Visible =
					Constants.ORDER_MEMBERRANK_MEMO_DISPLAY
						&& (string.IsNullOrEmpty(lUserMemberRankMemo.Text) == false);
			}

			// 顧客区分から注文者区分を取得
			// ※顧客区分に存在し、注文者区分に存在しない区分の場合、注文者区分は「オフラインゲスト」とする
			var ownerKbnValue = user.UserKbn;
			var ownerKbnText = ValueText.GetValueText(
				Constants.TABLE_ORDEROWNER,
				Constants.FIELD_ORDEROWNER_OWNER_KBN,
				ownerKbnValue);
			if (string.IsNullOrEmpty(ownerKbnText))
			{
				ownerKbnValue = Constants.FLG_ORDEROWNER_OWNER_KBN_OFFLINE_GUEST;
				ownerKbnText = ValueText.GetValueText(
					Constants.TABLE_ORDEROWNER,
					Constants.FIELD_ORDEROWNER_OWNER_KBN,
					ownerKbnValue);
			}

			ddlOwnerKbn.Visible = false;
			lOwnerKbn.Visible = true;
			lOwnerKbn.Text = GetEncodedHtmlDisplayMessage(ownerKbnText);
			hfOwnerKbn.Value = ownerKbnValue;

			foreach (ListItem listItem in rblMailFlg.Items)
			{
				listItem.Selected = (listItem.Value == user.MailFlg);
			}
			tbUserMemo.Text = user.UserMemo;
			foreach (ListItem listItem in ddlUserManagementLevel.Items)
			{
				listItem.Selected = (listItem.Value == user.UserManagementLevelId);
			}

			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				// 利用可能ポイントセット
				var userPoint = PointOptionUtility.GetUserPoint(user.UserId);
				var userPointUsable = (userPoint != null) ? userPoint.PointUsable : 0m;
				lUserPointUsable.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToNumeric(userPointUsable));
				hfUserPointUsable.Value = userPointUsable.ToString();
			}

			var ownerAddr1 = string.Empty;
			var ownerSex = string.Empty;
			var ownerBirthYear = string.Empty;
			var ownerBirthMonth = string.Empty;
			var ownerBirthDay = string.Empty;

			// 再注文の場合、元注文の注文者情報からコピーし、設定する
			if (orgOrderOwner != null)
			{
				tbOwnerName1.Text = orgOrderOwner.OwnerName1;
				tbOwnerName2.Text = orgOrderOwner.OwnerName2;
				tbOwnerNameKana1.Text = orgOrderOwner.OwnerNameKana1;
				tbOwnerNameKana2.Text = orgOrderOwner.OwnerNameKana2;

				tbOwnerMailAddr.Text = orgOrderOwner.OwnerMailAddr;
				tbOwnerMailAddr2.Text = orgOrderOwner.OwnerMailAddr2;
				this.CouponMailAddress = orgOrderOwner.OwnerMailAddr;

				// ISOが空でも日本としなければいけない
				if ((IsCountryJp(orgOrderOwner.OwnerAddrCountryIsoCode)
					|| string.IsNullOrEmpty(orgOrderOwner.OwnerAddrCountryIsoCode)))
				{
					var ownerTel1 = orgOrderOwner.OwnerTel1.Split('-');
					tbOwnerTel1_1.Text = ownerTel1[0];
					tbOwnerTel1_2.Text = ownerTel1[1];
					tbOwnerTel1_3.Text = ownerTel1[2];

					var ownerZip = orgOrderOwner.OwnerZip.Split('-');
					tbOwnerZip1.Text = ownerZip[0];
					tbOwnerZip2.Text = ownerZip[1];
				}

				lOwnerTel2.Text = GetEncodedHtmlDisplayMessage(orgOrderOwner.OwnerTel2);
				ownerAddr1 = orgOrderOwner.OwnerAddr1;
				tbOwnerAddr2.Text = orgOrderOwner.OwnerAddr2;
				tbOwnerAddr3.Text = orgOrderOwner.OwnerAddr3;
				tbOwnerAddr4.Text = orgOrderOwner.OwnerAddr4;
				tbOwnerCompanyName.Text = orgOrderOwner.OwnerCompanyName;
				tbOwnerCompanyPostName.Text = orgOrderOwner.OwnerCompanyPostName;

				ownerSex = orgOrderOwner.OwnerSex;

				// 生年月日分析（yyyy/MM/dd形式）
				if (string.IsNullOrEmpty(orgOrderOwner.OwnerBirth) == false)
				{
					var ownerBirth = DateTime.ParseExact(
						orgOrderOwner.OwnerBirth,
						CONST_FORMAT_SHORT_DATE,
						CultureInfo.InvariantCulture);
					ownerBirthYear = ownerBirth.Year.ToString();
					ownerBirthMonth = ownerBirth.Month.ToString();
					ownerBirthDay = ownerBirth.Day.ToString();
				}

				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					ddlAccessCountryIsoCode.SelectedValue = orgOrderOwner.AccessCountryIsoCode;
					lDispLanguageCode.Text = GetEncodedHtmlDisplayMessage(orgOrderOwner.DispLanguageCode);
					hfDispLanguageCode.Value = orgOrderOwner.DispLanguageCode;
					ddlDispLanguageLocaleId.SelectedValue = orgOrderOwner.DispLanguageLocaleId;
					lDispCurrencyCode.Text = GetEncodedHtmlDisplayMessage(orgOrderOwner.DispCurrencyCode);
					hfDispCurrencyCode.Value = orgOrderOwner.DispCurrencyCode;
					ddlDispCurrencyLocaleId.SelectedValue = orgOrderOwner.DispCurrencyLocaleId;

					ddlOwnerCountry.SelectedValue = orgOrderOwner.OwnerAddrCountryIsoCode;
					tbOwnerZipGlobal.Text = orgOrderOwner.OwnerZip;
					tbOwnerTel1Global.Text = orgOrderOwner.OwnerTel1;
					if (IsCountryUs(orgOrderOwner.OwnerAddrCountryIsoCode))
					{
						ddlOwnerAddr5.SelectedValue = orgOrderOwner.OwnerAddr5;
					}
					else
					{
						tbOwnerAddr5.Text = orgOrderOwner.OwnerAddr5;
					}
				}
			}
			else
			// 再注文以外の場合、ユーザー情報を元に設定する
			{
				tbOwnerName1.Text = user.Name1;
				tbOwnerName2.Text = user.Name2;
				tbOwnerNameKana1.Text = user.NameKana1;
				tbOwnerNameKana2.Text = user.NameKana2;

				tbOwnerMailAddr.Text = user.MailAddr;
				tbOwnerMailAddr2.Text = user.MailAddr2;
				this.CouponMailAddress = user.MailAddr;

				tbOwnerTel1_1.Text = user.Tel1_1;
				tbOwnerTel1_2.Text = user.Tel1_2;
				tbOwnerTel1_3.Text = user.Tel1_3;

				tbOwnerZip1.Text = user.Zip1;
				tbOwnerZip2.Text = user.Zip2;

				ownerAddr1 = user.Addr1;
				tbOwnerAddr2.Text = user.Addr2;
				tbOwnerAddr3.Text = user.Addr3;
				tbOwnerAddr4.Text = user.Addr4;
				tbOwnerCompanyName.Text = user.CompanyName;
				tbOwnerCompanyPostName.Text = user.CompanyPostName;

				ownerSex = user.Sex;
				ownerBirthYear = user.BirthYear;
				ownerBirthMonth = user.BirthMonth;
				ownerBirthDay = user.BirthDay;
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					ddlAccessCountryIsoCode.SelectedValue = user.AccessCountryIsoCode;
					lDispLanguageCode.Text = user.DispLanguageCode;
					hfDispLanguageCode.Value = user.DispLanguageCode;
					ddlDispLanguageLocaleId.SelectedValue = user.DispLanguageLocaleId;
					lDispCurrencyCode.Text = user.DispCurrencyCode;
					hfDispCurrencyCode.Value = user.DispCurrencyCode;
					ddlDispCurrencyLocaleId.SelectedValue = user.DispCurrencyLocaleId;

					var userCountries = DomainFacade.Instance.CountryLocationService.GetCountryNames();
					ddlOwnerCountry.SelectedValue = userCountries.Any(country => (country.CountryIsoCode == user.AddrCountryIsoCode))
						? user.AddrCountryIsoCode
						: ddlOwnerCountry.Items[0].Value;

					tbOwnerZipGlobal.Text = user.Zip;
					tbOwnerTel1Global.Text = user.Tel1;

					if (this.IsOwnerAddrUs)
					{
						ddlOwnerAddr5.SelectedValue = user.Addr5;
					}
					else
					{
						tbOwnerAddr5.Text = user.Addr5;
					}
				}

				GetAndSetDisplayOrderAndFixedPurchaseHistoryListByUserId(userId);
			}

			foreach (ListItem listItem in ddlOwnerAddr1.Items)
			{
				listItem.Selected = (listItem.Value == ownerAddr1);
			}
			foreach (ListItem listItem in rblOwnerSex.Items)
			{
				listItem.Selected = (listItem.Value == ownerSex);
			}

			ucOwnerBirth.Year = ownerBirthYear;
			ucOwnerBirth.Month = ownerBirthMonth;
			ucOwnerBirth.Day = ownerBirthDay;

			// 制限されるユーザー管理レベルを表示
			DispFixedPurchaseLimitUserLevel(user.UserManagementLevelId);
		}
		else
		{
			DisplayOwnerErrorMessages(
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_NOUSER));
		}

		if (string.IsNullOrEmpty(hfUserId.Value) == false)
		{
			cbAllowSaveOwnerIntoUser.Checked = Constants.DEFAULTUPDATE_TOUSER_FROMORDEROWNER;
		}
	}

	/// <summary>
	/// 再注文使用の場合、元注文情報を元に配送先情報設定
	/// </summary>
	/// <param name="orderShipping">元注文の配送先情報</param>
	/// <param name="fixedPurchaseShipping">Fixed purchase shipping</param>
	private void SetReOrderShipping(OrderShipping orderShipping, FixedPurchaseShippingModel fixedPurchaseShipping = null)
	{
		// Set shipping fo fixed purchase
		if (fixedPurchaseShipping != null)
		{
			this.ddlShippingKbnList.SelectedValue = Constants.SHIPPING_KBN_LIST_USER_INPUT;

			tbShippingName1.Text = fixedPurchaseShipping.ShippingName1;
			tbShippingName2.Text = fixedPurchaseShipping.ShippingName2;
			tbShippingNameKana1.Text = fixedPurchaseShipping.ShippingNameKana1;
			tbShippingNameKana2.Text = fixedPurchaseShipping.ShippingNameKana2;
			ddlShippingCountry.SelectedValue = fixedPurchaseShipping.ShippingCountryIsoCode;

			if (this.IsShippingAddrJp)
			{
				var shippingZip = fixedPurchaseShipping.ShippingZip.Split('-');
				tbShippingZip1.Text = shippingZip[0];
				tbShippingZip2.Text = shippingZip[1];

				var shippingTel = fixedPurchaseShipping.ShippingTel1.Split('-');
				tbShippingTel1_1.Text = shippingTel[0];
				tbShippingTel1_2.Text = shippingTel[1];
				tbShippingTel1_3.Text = shippingTel[2];
			}

			foreach (ListItem li in ddlShippingAddr1.Items)
			{
				li.Selected = (li.Value == fixedPurchaseShipping.ShippingAddr1.TrimStart('0'));
			}
			tbShippingAddr2.Text = fixedPurchaseShipping.ShippingAddr2;
			tbShippingAddr3.Text = fixedPurchaseShipping.ShippingAddr3;
			tbShippingAddr4.Text = fixedPurchaseShipping.ShippingAddr4;
			tbShippingCompanyName.Text = fixedPurchaseShipping.ShippingCompanyName;
			tbShippingCompanyPostName.Text = fixedPurchaseShipping.ShippingCompanyPostName;

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				if (this.IsShippingAddrJp == false)
				{
					tbShippingTel1.Text = fixedPurchaseShipping.ShippingTel1;
					tbShippingZipGlobal.Text = fixedPurchaseShipping.ShippingZip;

					if (this.IsOwnerAddrUs)
					{
						ddlShippingAddr5.SelectedValue = fixedPurchaseShipping.ShippingAddr5;
					}
					else
					{
						tbShippingAddr5.Text = fixedPurchaseShipping.ShippingAddr5;
					}
				}
			}

			return;
		}

		// 別出荷フラグがOFFの場合、注文者と同じにする
		if (orderShipping.AnotherShippingFlag == Constants.FLG_ORDERSHIPPING_ANOTHER_SHIPPING_FLG_INVALID)
		{
			this.ddlShippingKbnList.SelectedValue = Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER;
		}
		else if (string.IsNullOrEmpty(orderShipping.StorepickupRealShopId) == false)
		{
			this.ddlShippingKbnList.SelectedValue = Constants.SHIPPING_KBN_LIST_STORE_PICKUP;
			this.ddlRealStore.SelectedValue = orderShipping.ShippingName1;
			ddlRealStore_SelectedIndexChanged(null, null);
		}
		else
		// 元注文の配送先情報を元に設定する
		{
			this.ddlShippingKbnList.SelectedValue = Constants.SHIPPING_KBN_LIST_USER_INPUT;

			tbShippingName1.Text = orderShipping.ShippingName1;
			tbShippingName2.Text = orderShipping.ShippingName2;
			tbShippingNameKana1.Text = orderShipping.ShippingNameKana1;
			tbShippingNameKana2.Text = orderShipping.ShippingNameKana2;
			ddlShippingCountry.SelectedValue = orderShipping.ShippingCountryIsoCode;

			if (this.IsShippingAddrJp)
			{
				var shippingZip = orderShipping.ShippingZip.Split('-');
				tbShippingZip1.Text = shippingZip[0];
				tbShippingZip2.Text = shippingZip[1];

				var shippingTel = orderShipping.ShippingTel1.Split('-');
				tbShippingTel1_1.Text = shippingTel[0];
				tbShippingTel1_2.Text = shippingTel[1];
				tbShippingTel1_3.Text = shippingTel[2];
			}

			foreach (ListItem li in ddlShippingAddr1.Items)
			{
				li.Selected = (li.Value == orderShipping.ShippingAddr1.TrimStart('0'));
			}
			tbShippingAddr2.Text = orderShipping.ShippingAddr2;
			tbShippingAddr3.Text = orderShipping.ShippingAddr3;
			tbShippingAddr4.Text = orderShipping.ShippingAddr4;
			tbShippingCompanyName.Text = orderShipping.ShippingCompanyName;
			tbShippingCompanyPostName.Text = orderShipping.ShippingCompanyPostName;

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				if (this.IsShippingAddrJp == false)
				{
					tbShippingTel1.Text = orderShipping.ShippingTel1;
					tbShippingZipGlobal.Text = orderShipping.ShippingZip;

					if (this.IsOwnerAddrUs)
					{
						ddlShippingAddr5.SelectedValue = orderShipping.ShippingAddr5;
					}
					else
					{
						tbShippingAddr5.Text = orderShipping.ShippingAddr5;
					}
				}
			}
		}
	}

	/// <summary>
	/// 再注文使用の場合、元注文情報を元に購入商品を設定
	/// </summary>
	/// <param name="orderItems">元注文の購入商品情報</param>
	private void SetReOrderItems(List<OrderItem> orderItems)
	{
		var orderItemList = new List<Hashtable>();

		foreach (var orgItem in orderItems.Where(item => string.IsNullOrEmpty(item.ProductBundleId)))
		{
			// 商品情報取得
			var variationInfo = GetProductVariation(
				orgItem.ShopId,
				orgItem.ProductId,
				orgItem.VariationId,
				hfMemberRankId.Value,
				orgItem.ProductSaleId);

			var isSubscriptionBoxCampaignPeriod = false;
			var selectedSubscriptionBoxItem = new SubscriptionBoxItemModel();
			// 頒布会キャンペーン期間かどうか
			if (string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false)
			{
				var selectedSubscriptionBox = DataCacheControllerFacade
					.GetSubscriptionBoxCacheController()
					.Get(this.SubscriptionBoxCourseId);
				selectedSubscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
					x => (x.ProductId == orgItem.ProductId) && (x.VariationId == orgItem.VariationId));
				isSubscriptionBoxCampaignPeriod = OrderCommon.IsSubscriptionBoxCampaignPeriod(selectedSubscriptionBoxItem);
			}

			var isFixedPurchaseValid = (orgItem.FixedPurchaseProductFlg == Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON);
			if (variationInfo.Count != 0)
			{
				var orderItem = new Hashtable
				{
					{ Constants.FIELD_ORDERITEM_SHOP_ID, orgItem.ShopId },
					{ Constants.FIELD_ORDERITEM_PRODUCT_ID, orgItem.ProductId },
					{ Constants.FIELD_ORDERITEM_VARIATION_ID, orgItem.VariationId },
					{ Constants.FIELD_ORDERITEM_SUPPLIER_ID, orgItem.SupplierId },
					{ Constants.FIELD_PRODUCTVARIATION_V_ID,
						orgItem.VariationId.Replace(orgItem.ProductId, string.Empty) },
					{ Constants.FIELD_ORDERITEM_PRODUCT_NAME,string.Format("{0}{1}",
						variationInfo[0][Constants.FIELD_PRODUCT_NAME],
						CreateVariationName(variationInfo[0])) },
					{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, orgItem.ItemQuantity },
					{ Constants.FIELD_ORDERITEM_PRODUCT_PRICE,
						isSubscriptionBoxCampaignPeriod
							? selectedSubscriptionBoxItem.CampaignPrice.ToPriceString()
							: isFixedPurchaseValid
								? GetFixedPurchaseProductValidityPrice(variationInfo[0], true).ToPriceString()
								: GetProductValidityPrice(variationInfo[0]).ToPriceString() },
					{ Constants.FIELD_ORDERITEM_PRODUCTSALE_ID,
						StringUtility.ToEmpty(variationInfo[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]) },
					{ CONST_KEY_PRODUCT_OPTION_VALUE_SETTINGS,
						new ProductOptionSettingList(this.LoginOperatorShopId, orgItem.ProductId) },
					{ CONST_KEY_FIXED_PURCHASE, (isFixedPurchaseValid || IsSubscriptionBoxFixedAmount()) },
					{ Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, string.Empty },
					{ Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME, string.Empty },
					{ Constants.FIELD_ORDERITEM_NOVELTY_ID, string.Empty },
					{ Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE, variationInfo[0][Constants.FIELD_PRODUCT_TAX_RATE] },
					{ Constants.FIELD_PRODUCT_IMAGE_HEAD,
						string.IsNullOrEmpty(StringUtility.ToEmpty(variationInfo[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD]))
							? variationInfo[0][Constants.FIELD_PRODUCT_IMAGE_HEAD]
							: variationInfo[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] }
				};
				orderItemList.Add(orderItem);
			}
			// 全ポイント継続利用フラグがONなら表示
			if (this.CanUseAllPointFlg && isFixedPurchaseValid)
			{
				cbUseAllPointFlg.Visible = true;
			}
			else
			{
				cbUseAllPointFlg.Visible = false;
				cbUseAllPointFlg.Checked = false;
			}
		}

		// エラーは一度消す
		dvOrderItemErrorMessage.Visible = false;

		// データバインド
		RefreshItemsAndPriceView(orderItemList);
	}

	/// <summary>
	/// 再注文使用の場合、元注文情報を元に決済種別を設定
	/// </summary>
	/// <param name="rePaymentKbn">元注文の決済種別</param>
	private void SetRePayment(string rePaymentKbn)
	{
		ddlOrderPaymentKbn.SelectedValue = rePaymentKbn;

		ddlOrderPaymentKbn_SelectedIndexChanged(null, null);
	}

	/// <summary>
	/// 選択されている商品付帯情報値を取得する
	/// </summary>
	/// <param name="riProductOptionValueSetting">付帯情報選択用リピータ</param>
	/// <returns>選択されている付帯情報</returns>
	private string GetSelectedProductOptionValue(RepeaterItem riProductOptionValueSetting)
	{
		var rCblProductOptionValueSetting =
			(Repeater)riProductOptionValueSetting.FindControl("rCblProductOptionValueSetting");
		var ddlProductOptionValueSetting =
			(DropDownList)riProductOptionValueSetting.FindControl("ddlProductOptionValueSetting");
		var tbProductOptionValueSetting =
			(TextBox)riProductOptionValueSetting.FindControl("txtProductOptionValueSetting");
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
	/// 注文メモ設定を取得する
	/// </summary>
	/// <param name="displayKbn">表示区分</param>
	/// <returns>注文メモ</returns>
	public DataView GetOrderMemoSetting(string displayKbn)
	{
		return DomainFacade.Instance.OrderMemoSettingService.GetOrderMemoSettingInDataView(displayKbn);
	}

	/// <summary>
	/// カート投入区分取得
	/// </summary>
	/// <param name="isFixedPurchaseItem">定期購入商品か</param>
	/// <param name="isSubscriptionItem">頒布会商品か</param>
	/// <returns>カート投入区分</returns>
	private Constants.AddCartKbn GetCartAddKbn(bool isFixedPurchaseItem, bool isSubscriptionItem)
	{
		// 頒布会が最優先
		if (isSubscriptionItem)
		{
			return Constants.AddCartKbn.SubscriptionBox;
		}

		if (isFixedPurchaseItem)
		{
			return Constants.AddCartKbn.FixedPurchase;
		}
		return Constants.AddCartKbn.Normal;
	}

	/// <summary>
	/// アドレス帳ドロップダウン作成
	/// </summary>
	/// <param name="userShippings">ユーザー配送先情報</param>
	/// <param name="shopShipping">Shop Shipping</param>
	protected void BindShippingList(UserShippingModel[] userShippings, ShopShippingModel shopShipping)
	{
		ddlUserShipping.Items.Clear();
		ddlUserShipping.Items.AddRange(
			//「配送先入力」
			ValueText.GetValueItemArray(
				Constants.VALUETEXT_PARAM_ORDER,
				Constants.VALUETEXT_PARAM_ORDER_USERS_SHIPPING_ITEMS));
		var serviceShipping = false;

		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& (shopShipping != null))
		{
			var deliveryCompanyList = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
				? shopShipping.CompanyListExpress
				: shopShipping.CompanyListMail;
			serviceShipping = deliveryCompanyList
				.Select(company => DomainFacade.Instance.DeliveryCompanyService.Get(company.DeliveryCompanyId))
				.Any(company => (company.DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID));

			if (serviceShipping)
			{
				ddlUserShipping.Items.Add(new ListItem(
					ReplaceTag("@@DispText.shipping_addr_kbn_list.convenience_store@@"),
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE));
				ddlUserShipping.Items.AddRange(userShippings
					.Select(userShipping => new ListItem(userShipping.Name, userShipping.ShippingNo.ToString()))
					.ToArray());
			}
		}

		if (serviceShipping == false)
		{
			var userShippingNormal = userShippings
				.Where(item => (item.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF));
			ddlUserShipping.Items.AddRange(userShippingNormal
				.Select(userShipping => new ListItem(userShipping.Name, userShipping.ShippingNo.ToString()))
				.ToArray());
		}
	}

	/// <summary>
	/// ノベルティ追加フォームイベント
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rNoveltyItem_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if (e.CommandName != "add") return;

		// 隠しフィールドから各値取得
		var novelty = (Repeater)(rNoveltyList.Items[int.Parse(e.CommandArgument.ToString().Split(',')[0])]
			.FindControl("rNoveltyItem"));
		var noveltyItem = novelty.Items[int.Parse(e.CommandArgument.ToString().Split(',')[1])];
		var noveltyId = ((HiddenField)noveltyItem.FindControl("hfNoveltyId")).Value;
		var productId = ((HiddenField)noveltyItem.FindControl("hfProductId")).Value;
		var variationId = ((HiddenField)noveltyItem.FindControl("hfVariationId")).Value;
		var productName = ((HiddenField)noveltyItem.FindControl("hfProductName")).Value;
		var productPrice = ((HiddenField)noveltyItem.FindControl("hfProductPrice")).Value;
		var taxRate = ((HiddenField)noveltyItem.FindControl("hfTaxRate")).Value;
		var productImageHead = ((HiddenField)noveltyItem.FindControl("hfProductImageHead")).Value;
		var memberRankId = hfMemberRankId.Value;
		var shopId = ((HiddenField)noveltyItem.FindControl("hfShopId")).Value;

		// 既エラー状態の場合、処理しない
		var orderItemList = CreateOrderItemInput();
		var itemErrorMessages = CheckItemsAndPricesInput(orderItemList);
		if (DisplayItemsErrorMessages(itemErrorMessages))
		{
			// 注文商品データバインド
			BindDataOrderItem(orderItemList);
			CreateOrderItemRows();

			if (string.IsNullOrEmpty(tbCouponUse.Text.Trim()) == false)
				DisplayCouponErrorMessage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PLEASE_ENTER_PRODUCT_INFORMATION));
			if (tbPointUse.Text.Trim() != "0")
				DisplayPointErrorMessages(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PLEASE_ENTER_PRODUCT_INFORMATION));
			return;
		}

		// Get info product novelty
		var productInfo = DomainFacade.Instance.ProductService.GetProductVariationAtDataRowView(
			shopId,
			productId,
			variationId,
			memberRankId);
		if (productInfo != null)
		{
			// Create info cart product novelty
			var productNovelty = new CartProduct(
				productInfo,
				Constants.AddCartKbn.Normal,
				string.Empty,
				1,
				true,
				new ProductOptionSettingList());
			productPrice = productNovelty.Price.ToPriceString();
		}

		// 注文商品情報にノベルティ商品を追加
		var orderItem = new Hashtable
		{
			{ Constants.FIELD_NOVELTY_NOVELTY_ID, noveltyId },
			{ Constants.FIELD_ORDERITEM_SHOP_ID, this.LoginOperatorShopId },
			{ Constants.FIELD_ORDERITEM_PRODUCT_ID, productId },
			{ Constants.FIELD_ORDERITEM_VARIATION_ID, variationId },
			{ Constants.FIELD_PRODUCTVARIATION_V_ID, variationId.Replace(productId, string.Empty) },
			{ Constants.FIELD_ORDERITEM_PRODUCT_NAME, productName },
			{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, 1 },
			{ Constants.FIELD_ORDERITEM_PRODUCT_PRICE, productPrice },
			{ Constants.FIELD_ORDERITEM_ITEM_PRICE, productPrice },
			{ Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, string.Empty },
			{ CONST_KEY_PRODUCT_OPTION_VALUE_SETTINGS, new ProductOptionSettingList(this.LoginOperatorShopId, productId) },
			{ Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE, taxRate },
			{ Constants.FIELD_PRODUCT_IMAGE_HEAD, productImageHead }
		};
		orderItemList.Add(orderItem);

		// データバインド
		BindDataOrderItem(orderItemList);

		if (Constants.NOVELTY_OPTION_ENABLED)
		{
			this.DeletedNoveltyIds.Remove(noveltyId);
		}

		// カート作成＆更新
		RefreshItemsAndPriceView();
	}

	/// <summary>
	/// ユーザー詳細ポップアップ画面から再注文ボタン押下
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSetReOrderData_Click(object sender, EventArgs e)
	{
		this.ReOrderId = hfReOrderId.Value;
		SetReOrderData();
	}

	/// <summary>
	/// 元注文情報を元に、画面項目を設定する。
	/// </summary>
	private void SetReOrderData()
	{
		// 元注文情報取得
		var dvOrgOrder = OrderCommon.GetOrder(this.ReOrderId);
		if (dvOrgOrder.Count == 0)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 入力チェック
		// 下記のいずれかの条件に合うとエラーになる。
		// ・セット商品を購入した注文
		// ・返品／交換注文
		// ・定期購入のみの商品を購入した注文
		var orgOrder = new Order(dvOrgOrder);
		if ((orgOrder.ReturnExchangeKbn != Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN)
			|| orgOrder.Items.Any(oi => (string.IsNullOrEmpty(oi.ProductSetId) == false))
			|| orgOrder.Items.Any(oi => (oi.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY)))
		{
			// システムエラーになる。
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_UNABLE_REORDER);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 注文者情報設定
		SetOwner(orgOrder.UserId, orgOrder.Owner);

		// 配送先情報設定
		SetReOrderShipping(orgOrder.Shipping);

		// 頒布会情報設定
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (string.IsNullOrEmpty(orgOrder.SubscriptionBoxCourseId) == false))
		{
			ddlSubscriptionBox.SelectedValue = orgOrder.SubscriptionBoxCourseId;
			Session["SubscriptionBoxCourseId"] = orgOrder.SubscriptionBoxCourseId;
		}

		// ■注文購入商品設定
		this.SetReOrderItems(orgOrder.Items);

		// Set re-order payment
		SetRePayment(orgOrder.OrderPaymentKbn);

		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			var input = new OrderExtendInput(OrderExtendCommon.CreateOrderExtendForManager(dvOrgOrder[0]));
			SetOrderExtendFromUserExtendObject(rOrderExtendInput, input);
		}

		// 再注文警告メッセージ表示
		dvReOrderWarning.Visible
			= dvShippingInfo.Visible
			= dvPayment.Visible
			= dvBeforeCombine.Visible = true;
		GetAndSetDisplayOrderAndFixedPurchaseHistoryListByUserId(orgOrder.UserId);
	}

	/// <summary>
	/// 注文者情報を画面項目に設定
	/// </summary>
	/// <param name="orderDataInfos">Order data informations</param>
	private void SetOrderOwnerFromOrderDatas(Hashtable orderDataInfos)
	{
		var orderInput = (Hashtable)orderDataInfos["order_input"];
		var cart = (CartObject)orderDataInfos["cart"];

		// 注文者情報
		var userId = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_USER_ID]);
		lUserId.Text = GetEncodedHtmlDisplayMessage(userId);
		lUserIdNonSet.Text = GetEncodedHtmlDisplayMessage(string.IsNullOrEmpty(userId)
			? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_USER_ID_NOT_SET_ALERT)
			: string.Empty);
		hfUserId.Value = userId;
		lbUserClear.Visible = (string.IsNullOrEmpty(userId) == false);
		hfMemberRankId.Value = MemberRankOptionUtility.GetMemberRankId(userId);
		hfFixedPurchaseMember.Value = cart.IsFixedPurchaseMember
			? Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON
			: Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF;
		lMemberRankName.Text = GetEncodedHtmlDisplayMessage(MemberRankOptionUtility.GetMemberRankName(hfMemberRankId.Value));
		dvMemberRank.Visible = this.IsAvailableRank;
		if (this.IsAvailableRank)
		{
			lUserMemberRankName.Text =
				GetEncodedHtmlDisplayMessage(MemberRankOptionUtility.GetMemberRankName(hfMemberRankId.Value));
			lUserMemberRankBenefit.Text =
				GetEncodedHtmlDisplayMessage(MemberRankBenefitWording.CreateBenefitString(hfMemberRankId.Value));
			lUserMemberRankMemo.Text =
				GetEncodedHtmlDisplayMessage(
					MemberRankBenefitWording.CreateBenefitStringMemo(hfMemberRankId.Value));
			dvMemberRankMemo.Visible =
				Constants.ORDER_MEMBERRANK_MEMO_DISPLAY
					&& (string.IsNullOrEmpty(lUserMemberRankMemo.Text) == false);
		}

		hfDefineShippingConvenienceStore.Value = cart.GetShipping().IsShippingConvenience.ToString();

		var isGuest = (UserService.IsGuest(cart.Owner.OwnerKbn) || (string.IsNullOrEmpty(lUserId.Text)));
		ddlOwnerKbn.SelectedValue = cart.Owner.OwnerKbn;
		ddlOwnerKbn.Visible = isGuest;
		lOwnerKbn.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_USER,
				Constants.FIELD_USER_USER_KBN,
				cart.Owner.OwnerKbn));
		lOwnerKbn.Visible = (isGuest == false);
		hfOwnerKbn.Value = cart.Owner.OwnerKbn;
		tbOwnerName1.Text = cart.Owner.Name1;
		tbOwnerName2.Text = cart.Owner.Name2;
		tbOwnerNameKana1.Text = cart.Owner.NameKana1;
		tbOwnerNameKana2.Text = cart.Owner.NameKana2;
		ddlOwnerKbn.SelectedValue = cart.Owner.OwnerKbn;
		tbOwnerTel1_1.Text = cart.Owner.Tel1_1;
		tbOwnerTel1_2.Text = cart.Owner.Tel1_2;
		tbOwnerTel1_3.Text = cart.Owner.Tel1_3;
		tbOwnerTel2_1.Text = cart.Owner.Tel2_1;
		tbOwnerTel2_2.Text = cart.Owner.Tel2_2;
		tbOwnerTel2_3.Text = cart.Owner.Tel2_3;
		tbOwnerMailAddr.Text = cart.Owner.MailAddr;
		tbOwnerMailAddr2.Text = cart.Owner.MailAddr2;
		tbOwnerZip1.Text = cart.Owner.Zip1;
		tbOwnerZip2.Text = cart.Owner.Zip2;
		ddlOwnerAddr1.SelectedValue = cart.Owner.Addr1;
		tbOwnerAddr2.Text = cart.Owner.Addr2;
		tbOwnerAddr3.Text = cart.Owner.Addr3;
		tbOwnerAddr4.Text = cart.Owner.Addr4;
		tbOwnerCompanyName.Text = cart.Owner.CompanyName;
		tbOwnerCompanyPostName.Text = cart.Owner.CompanyPostName;
		rblOwnerSex.SelectedValue = cart.Owner.Sex;
		ucOwnerBirth.Year = cart.Owner.BirthYear;
		ucOwnerBirth.Month = cart.Owner.BirthMonth;
		ucOwnerBirth.Day = cart.Owner.BirthDay;
		hfShippingType.Value = cart.ShippingType;

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ddlAccessCountryIsoCode.SelectedValue = cart.Owner.AccessCountryIsoCode;
			lDispLanguageCode.Text = GetEncodedHtmlDisplayMessage(cart.Owner.DispLanguageCode);
			hfDispLanguageCode.Value = cart.Owner.DispLanguageCode;
			ddlDispLanguageLocaleId.SelectedValue = cart.Owner.DispLanguageLocaleId;
			lDispCurrencyCode.Text = GetEncodedHtmlDisplayMessage(cart.Owner.DispCurrencyCode);
			hfDispCurrencyCode.Value = cart.Owner.DispCurrencyCode;
			ddlDispCurrencyLocaleId.SelectedValue = cart.Owner.DispCurrencyLocaleId;

			ddlOwnerCountry.SelectedValue = cart.Owner.AddrCountryIsoCode;

			if (this.IsOwnerAddrJp == false)
			{
				tbOwnerZipGlobal.Text = cart.Owner.Zip;
				tbOwnerTel1Global.Text = cart.Owner.Tel1;
				tbOwnerTel2Global.Text = cart.Owner.Tel2;

				if (this.IsOwnerAddrUs)
				{
					ddlOwnerAddr5.SelectedValue = cart.Owner.Addr5;
				}
				else
				{
					tbOwnerAddr5.Text = cart.Owner.Addr5;
				}
			}
		}

		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
		{
			ddlUserShipping.SelectedValue = cart.GetShipping().ShippingAddrKbn;
			ddlShippingReceivingStoreType.SelectedValue = cart.GetShipping().ShippingReceivingStoreType;
		}

		GetAndSetDisplayOrderAndFixedPurchaseHistoryListByUserId(userId);
	}

	/// <summary>
	/// 注文情報を画面項目に設定
	/// </summary>
	/// <param name="orderDataInfos">Order data informations</param>
	private void SetOrderData(Hashtable orderDataInfos)
	{
		var orderInput = (Hashtable)orderDataInfos["order_input"];
		var userInput = (Hashtable)orderDataInfos["user_input"];
		var cart = (CartObject)orderDataInfos["cart"];

		// 引き継いだプロパティ
		this.IsOrderCombined = (bool)orderDataInfos["is_order_combined"];
		this.CombineParentOrderId = StringUtility.ToEmpty(orderDataInfos["combine_parent_order_id"]);
		this.CombineChildOrderIds = (string[])orderDataInfos["combine_child_order_ids"];
		this.OrderCombineBeforeCart = (CartObject)orderDataInfos["order_combine_before_cart"];
		this.UseOrderCombine = (bool)orderDataInfos["use_order_combine"];

		if (this.IsOrderCombined)
		{
			this.CombineParentOrderHasFixedPurchase = cart.IsCombineParentOrderHasFixedPurchase;
			this.CombineParentOrderCount = cart.CombineParentOrderFixedPurchaseOrderCount;
			this.CombineParentOrderFixedPurchase = cart.FixedPurchase;
			cart.OrderCombineParentOrderId = (this.IsOrderCombinedAtOrderCombinePage == false)
				? this.CombineParentOrderId
				: string.Format("{0},{1}", this.CombineParentOrderId, string.Join(",", this.CombineChildOrderIds));
			this.CombineFixedPurchaseDiscountPrice = cart.FixedPurchaseDiscount;
			dvBeforeCombine.Visible = false;
			cbOrderCombineSelect.Checked = this.UseOrderCombine;
		}

		var shopShipping = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, cart.ShippingType);
		RefreshPaymentViewFromShippingType(cart, shopShipping);

		// 基本情報
		ddlOrderKbn.SelectedValue = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_ORDER_KBN]);
		ddlOrderPaymentKbn.SelectedValue = cart.Payment.PaymentId;

		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED)
		{
			ddlSubscriptionBox.SelectedValue = cart.SubscriptionBoxCourseId;
			Session["SubscriptionBoxCourseId"] = cart.SubscriptionBoxCourseId;
		}

		// ユーザー情報
		SetOrderUserInfo(userInput);
		cbAllowSaveOwnerIntoUser.Checked = (bool)orderDataInfos["update_user_flg"];

		// 配送先情報
		var cartShipping = cart.GetShipping();
		if (string.IsNullOrEmpty(cartShipping.RealShopId) == false)
		{
			this.ddlShippingKbnList.SelectedValue = Constants.SHIPPING_KBN_LIST_STORE_PICKUP;
			hfShippingKbn.Value = this.ddlShippingKbnList.SelectedValue;
			if ((this.RealShopModels != null)
				&& this.RealShopModels.Any(rs => rs.RealShopId == cartShipping.RealShopId))
			{
				this.ddlRealStore.SelectedIndex = this.RealShopModels.ToList().FindIndex(rs => rs.RealShopId == cartShipping.RealShopId);
				this.ddlRealStore_SelectedIndexChanged(null, EventArgs.Empty);
			}
		}
		else if (cartShipping.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER)
		{
			this.ddlShippingKbnList.SelectedValue = Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER;
			hfShippingKbn.Value = this.ddlShippingKbnList.SelectedValue;
			dvUserShippingList.Visible = true;
		}
		else
		{
			this.ddlShippingKbnList.SelectedValue = Constants.SHIPPING_KBN_LIST_USER_INPUT;
			hfShippingKbn.Value = this.ddlShippingKbnList.SelectedValue;
			dvUserShippingList.Visible = true;
			var userShippingSelectValue = (string)orderDataInfos["ddlUserShipping_select_value"];
			ddlUserShipping.SelectedValue = userShippingSelectValue;

			// Set Payment and delivery company to convenience store
			RefreshPaymentViewFromShippingType(cart, shopShipping);
			RefreshShippingCompanyViewFromShippingType(shopShipping);
			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
			{
				RefreshShippingDateViewFromShippingType(shopShipping, cart, true);
			}
			if (userShippingSelectValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW)
			{
				SetOrderShippingFromCart(cart);
			}
			else
			{
				SetConvenienceStoreInformation(cartShipping);

				if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
					&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
					&& (cartShipping.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
				{
					ddlShippingReceivingStoreType.SelectedValue = cartShipping.ShippingReceivingStoreType;
					ddlShippingReceivingStoreType.Visible = true;
					ddlShippingReceivingStoreType.Enabled = (userShippingSelectValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE);
				}
				else
				{
					ddlShippingReceivingStoreType.Visible = false;
				}
			}
		}

		// 購入商品情報
		var orderItems = CreateOrderItemFromCart((this.OrderCombineBeforeCart == null)
			? cart
			: this.OrderCombineBeforeCart);
		BindDataOrderItem(orderItems);

		// 値引関連情報
		SetOrderDiscountInfoFromCart(cart);

		// 各種メモ
		if (this.IsOrderCombinedAtOrderCombinePage)
		{
			rOrderMemos.Visible = false;
			var orderMemoSettings = GetOrderMemoSetting(Constants.FLG_ORDER_MEMO_SETTING_DISP_FLG_PC);
			if (orderMemoSettings.Count > 0)
			{
				dvOrderMemoForCombine.Visible = true;
				tbMemoForCombine.Text = cart.GetOrderMemos();
			}
			else
			{
				dvOrderMemoForCombine.Visible = false;
			}
		}
		else
		{
			foreach (RepeaterItem repeaterItem in rOrderMemos.Items)
			{
				var memo = (TextBox)repeaterItem.FindControl("tbMemo");
				if (memo == null) continue;

				memo.Text = string.IsNullOrEmpty(cart.OrderCombineParentOrderId)
					? cart.OrderMemos[repeaterItem.ItemIndex].InputText
					: this.OrderCombineBeforeCart.OrderMemos[repeaterItem.ItemIndex].InputText;
			}
		}

		tbPaymentMemo.Text = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_PAYMENT_MEMO]);
		tbManagerMemo.Text = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_MANAGEMENT_MEMO]);
		tbShippingMemo.Text = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_SHIPPING_MEMO]);
		tbRelationMemo.Text = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_RELATION_MEMO]);
		tbFixedPurchaseManagementMemo.Text
			= StringUtility.ToEmpty(orderInput[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_MANAGEMENT_MEMO]);

		RefreshDeliverySpecificationFromShippingType(cart, shopShipping, true);

		// 配送情報
		SetOrderShippingMethodFromCart(cart);

		// For fixed purchase
		RefreshFixedPurchaseViewFromShippingType(cart, shopShipping);
		SetFixedPurchaseDeliveryPatternFromCart(cart, shopShipping);

		// 注文同梱情報
		if ((string.IsNullOrEmpty(cart.OrderCombineParentOrderId) == false)
			&& (this.IsOrderCombinedAtOrderCombinePage == false))
		{
			// 注文同梱画面からの遷移でない場合、同梱後のカート情報を表示
			var isVisibleShippingFixedPurchase =
				(cart.IsBeforeCombineCartHasFixedPurchase
					& (cart.IsCombineParentOrderHasFixedPurchase == false))
				|| (cart.HasSubscriptionBox
					&& (cart.IsOrderCombinedWithSameSubscriptionBoxCourse() == false));
			SetCombinedCart(new CartObject[] { cart }, isVisibleShippingFixedPurchase);
		}

		// 注文同梱元親注文が定期購入の場合、定期購入回数を設定(定期購入割引計算用)
		if (this.CombineParentOrderHasFixedPurchase)
		{
			this.CombineParentOrderCount = cart.CombineParentOrderFixedPurchaseOrderCount;
			this.CombineFixedPurchaseDiscountPrice = cart.FixedPurchaseDiscount;
		}

		// 決済情報
		SetOrderPaymentFromCart(cart);

		hfCvsShopNo.Value = cartShipping.ConvenienceStoreId;
		hfCvsShopName.Value = cartShipping.Name;
		hfCvsShopAddress.Value = cartShipping.Addr4;
		hfCvsShopTel.Value = cartShipping.Tel1;
		DisplayConvenienceStoreData();

		// 広告コード
		var advCode = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_ADVCODE_NEW]);
		tbAdvCode.Text = advCode;
		var advCodeInfo = DomainFacade.Instance.AdvCodeService.GetAdvCodeFromAdvertisementCode(advCode);
		lbAdvName.Text = GetEncodedHtmlDisplayMessage((advCodeInfo != null)
			? advCodeInfo.MediaName
			: string.Empty);

		var hasFixedPurchaseProduct = false;
		foreach (RepeaterItem item in rItemList.Items)
		{
			var cbFixedPurchase = (CheckBox)item.FindControl("cbFixedPurchase");
			if (cbFixedPurchase.Checked == false) continue;
			hasFixedPurchaseProduct = true;
			break;
		}
		cbReflectMemoToFixedPurchase.Visible = hasFixedPurchaseProduct;
		cbReflectMemoToFixedPurchase.Checked = cart.ReflectMemoToFixedPurchase;

		// 領収書情報
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			rblReceiptFlg.SelectedValue = cart.ReceiptFlg;
			tbReceiptAddress.Text = cart.ReceiptAddress;
			tbReceiptProviso.Text = cart.ReceiptProviso;
			tbReceiptAddress.Enabled
				= tbReceiptProviso.Enabled
				= (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON);
		}

		// Inflow Contents
		ddlInflowContentsType.SelectedValue = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE]);
		tbInflowContentsId.Text = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_INFLOW_CONTENTS_ID]);

		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			var input = new OrderExtendInput(cart.OrderExtend.ToDictionary(i => i.Key, i => i.Value.Value));
			SetOrderExtendFromUserExtendObject(rOrderExtendInput, input);
		}

		if (OrderCommon.DisplayTwInvoiceInfo() && this.IsShippingAddrTw)
		{
			ddlUniformInvoiceType.SelectedValue = cartShipping.UniformInvoiceType;

			ddlUniformInvoiceOrCarryTypeOption.DataSource = GetUniformInvoiceOrCarryTypeOption(
				cartShipping.UniformInvoiceType,
				cartShipping.CarryType);
			ddlUniformInvoiceOrCarryTypeOption.DataBind();
			ddlUniformInvoiceOrCarryTypeOption.Visible = true;

			if (string.IsNullOrEmpty(cartShipping.CarryTypeOption) == false)
			{
				spUniformInvoiceOption1_1.Visible
					= spUniformInvoiceOption1_2.Visible
					= spUniformInvoiceOption2.Visible
					= spInvoiceCarryType.Visible = false;
			}
			else if (string.IsNullOrEmpty(cartShipping.CarryTypeOption)
				|| ((cartShipping.UniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
					&& (cartShipping.CarryType == Constants.FLG_ORDER_TW_CARRY_TYPE_NONE)))
			{
				spUniformInvoiceOption1_1.Visible
					= spUniformInvoiceOption1_2.Visible
					= spUniformInvoiceOption2.Visible
					= spInvoiceCarryType.Visible = true;
			}

			switch (cartShipping.UniformInvoiceType)
			{
				case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
					ddlUniformInvoiceOrCarryTypeOption.SelectedValue = cartShipping.CarryTypeOption;
					ddlInvoiceCarryType.SelectedValue = cartShipping.CarryType;
					if (ddlInvoiceCarryType.SelectedValue == Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE)
					{
						tbCarryTypeOption1.Text = cartShipping.CarryTypeOptionValue;
						tbCarryTypeOption1.Visible = true;
						spInvoiceCarryType.Visible = true;
						cbCarryTypeOptionRegist.Checked
							= dvCarryTypeOptionName.Visible
							= cartShipping.UserInvoiceRegistFlg;
						tbCarryTypeOptionName.Text = cartShipping.InvoiceName;
					}
					else if (ddlInvoiceCarryType.SelectedValue == Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE)
					{
						tbCarryTypeOption2.Text = cartShipping.CarryTypeOptionValue;
						tbCarryTypeOption2.Visible = true;
						spInvoiceCarryType.Visible = true;
						cbCarryTypeOptionRegist.Checked
							= dvCarryTypeOptionName.Visible
							= cartShipping.UserInvoiceRegistFlg;
						tbCarryTypeOptionName.Text = cartShipping.InvoiceName;
					}
					else
					{
						ddlUniformInvoiceOrCarryTypeOption.Visible = false;
					}
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
					ddlUniformInvoiceOrCarryTypeOption.SelectedValue = cartShipping.UniformInvoiceTypeOption;
					lUniformInvoiceOption1_1.Text
						= tbUniformInvoiceOption1_1.Text
						= cartShipping.UniformInvoiceOption1;
					lUniformInvoiceOption1_2.Text
						= tbUniformInvoiceOption1_2.Text
						= cartShipping.UniformInvoiceOption2;
					dvUniformInvoiceOption1_1.Visible = true;
					dvUniformInvoiceOption1_2.Visible = true;
					dvInvoiceCarryType.Visible = false;
					cbSaveToUserInvoice.Checked
						= dvUniformInvoiceTypeRegistInput.Visible
						= cartShipping.UserInvoiceRegistFlg;
					tbUniformInvoiceTypeName.Text = cartShipping.InvoiceName;
					break;

				case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
					ddlUniformInvoiceOrCarryTypeOption.SelectedValue = cartShipping.UniformInvoiceTypeOption;
					lUniformInvoiceOption2.Text
						= tbUniformInvoiceOption2.Text
						= cartShipping.UniformInvoiceOption1;
					dvUniformInvoiceOption2.Visible = true;
					dvInvoiceCarryType.Visible = false;
					cbSaveToUserInvoice.Checked
						= dvUniformInvoiceTypeRegistInput.Visible
						= cartShipping.UserInvoiceRegistFlg;
					tbUniformInvoiceTypeName.Text = cartShipping.InvoiceName;
					break;
			}
			SetEnableTextBoxUniformInvoice();
		}

		// 表示更新後再バインド
		BindDataOrderItem(orderItems);

		// 再計算
		RefreshItemsAndPriceView();
	}

	/// <summary>
	/// Set Convenience Store Information
	/// </summary>
	/// <param name="cartShipping">Cart Shipping</param>
	private void SetConvenienceStoreInformation(CartShipping cartShipping)
	{
		lCvsShopNo.Text = GetEncodedHtmlDisplayMessage(cartShipping.ConvenienceStoreId);
		lCvsShopName.Text = GetEncodedHtmlDisplayMessage(cartShipping.Name);
		lCvsShopAddress.Text = GetEncodedHtmlDisplayMessage(cartShipping.Addr4);
		lCvsShopTel.Text = GetEncodedHtmlDisplayMessage(cartShipping.Tel1);
	}

	/// <summary>
	/// ユーザー情報を画面項目に設定
	/// </summary>
	/// <param name="userInput">ユーザー情報</param>
	private void SetOrderUserInfo(Hashtable userInput)
	{
		tbUserMemo.Text = StringUtility.ToEmpty(userInput[Constants.FIELD_USER_USER_MEMO]);
		var userLevel = StringUtility.ToEmpty(userInput[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID]);
		ddlUserManagementLevel.SelectedValue = userLevel;
		DisplayPaymentUserManagementLevel(this.LoginOperatorShopId, userLevel);
		rblMailFlg.SelectedValue = StringUtility.ToEmpty(userInput[Constants.FIELD_USER_MAIL_FLG]);

		// 制限されるユーザー管理レベルを表示
		DispFixedPurchaseLimitUserLevel(userLevel);

		var userKbn = (string)userInput[Constants.FIELD_USER_USER_KBN];
		DisplayPaymentOrderOwnerKbn(this.LoginOperatorShopId, userKbn);
	}

	/// <summary>
	/// 配送先情報を画面項目に設定
	/// </summary>
	/// <param name="cart">カート</param>
	private void SetOrderShippingFromCart(CartObject cart)
	{
		var cartShipping = cart.GetShipping();
		tbShippingName1.Text = cartShipping.Name1;
		tbShippingName2.Text = cartShipping.Name2;
		tbShippingNameKana1.Text = cartShipping.NameKana1;
		tbShippingNameKana2.Text = cartShipping.NameKana2;
		tbShippingZip1.Text = cartShipping.Zip1;
		tbShippingZip2.Text = cartShipping.Zip2;
		ddlShippingAddr1.SelectedValue = cartShipping.Addr1;
		tbShippingAddr2.Text = cartShipping.Addr2;
		tbShippingAddr3.Text = cartShipping.Addr3;
		tbShippingAddr4.Text = cartShipping.Addr4;
		if (Constants.DISPLAY_CORPORATION_ENABLED)
		{
			tbShippingCompanyName.Text = cartShipping.CompanyName;
			tbShippingCompanyPostName.Text = cartShipping.CompanyPostName;
		}
		tbShippingTel1_1.Text = cartShipping.Tel1_1;
		tbShippingTel1_2.Text = cartShipping.Tel1_2;
		tbShippingTel1_3.Text = cartShipping.Tel1_3;
		cbAlowSaveNewAddress.Checked = cartShipping.UserShippingRegistFlg;
		tbShippingName.Text = cartShipping.UserShippingRegistFlg
			? cartShipping.UserShippingName
			: string.Empty;

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			ddlShippingCountry.SelectedValue = cartShipping.ShippingCountryIsoCode;

			if (this.IsShippingAddrJp == false)
			{
				tbShippingTel1.Text = cartShipping.Tel1;
				tbShippingZipGlobal.Text = cartShipping.Zip;

				if (this.IsShippingAddrUs)
				{
					ddlShippingAddr5.SelectedValue = cartShipping.Addr5;
				}
				else
				{
					tbShippingAddr5.Text = cartShipping.Addr5;
				}
			}
		}

		var shipping = cart.Shippings.First();
		hfCvsShopNo.Value = shipping.ConvenienceStoreId;
		hfCvsShopName.Value = shipping.Name1;
		hfCvsShopAddress.Value = shipping.Addr4;
		hfCvsShopTel.Value = shipping.Tel1;
		DisplayConvenienceStoreData();

		if (shipping.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
		{
			ddlUserShipping.SelectedValue = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE;
		}
	}

	/// <summary>
	/// Display convenience store data
	/// </summary>
	protected void DisplayConvenienceStoreData()
	{
		lCvsShopNo.Text = GetEncodedHtmlDisplayMessage(hfCvsShopNo.Value);
		lCvsShopName.Text = GetEncodedHtmlDisplayMessage(hfCvsShopName.Value);
		lCvsShopAddress.Text = GetEncodedHtmlDisplayMessage(hfCvsShopAddress.Value);
		lCvsShopTel.Text = GetEncodedHtmlDisplayMessage(hfCvsShopTel.Value);
	}

	/// <summary>
	/// 商品情報を画面項目に設定
	/// </summary>
	/// <param name="cart">カート</param>
	private List<Hashtable> CreateOrderItemFromCart(CartObject cart)
	{
		var orderItems = cart.Items
			.Select(item =>
				new Hashtable
				{
					{ Constants.FIELD_ORDERITEM_SHOP_ID, item.ShopId },
					{ Constants.FIELD_ORDERITEM_PRODUCT_ID, item.ProductId },
					{ Constants.FIELD_ORDERITEM_SUPPLIER_ID, item.SupplierId },
					{ Constants.FIELD_ORDERITEM_VARIATION_ID, item.VariationId },
					{Constants.FIELD_PRODUCTVARIATION_V_ID, (item.ProductId == item.VId) ? string.Empty : item.VId },
					{ Constants.FIELD_ORDERITEM_PRODUCT_NAME, item.ProductJointName },
					{ Constants.FIELD_ORDERITEM_PRODUCT_PRICE, item.Price },
					{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, item.Count },
					{ Constants.FIELD_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG, item.IsFixedPurchase },
					{ Constants.FIELD_ORDERITEM_NOVELTY_ID, item.NoveltyId },
					{ Constants.FIELD_ORDERITEM_ITEM_PRICE, item.PriceSubtotal },
					{ Constants.FIELD_CART_PRODUCT_OPTION_TEXTS, item.ProductOptionSettingList },
					{ CONST_KEY_FIXED_PURCHASE, (cart.IsCombineParentOrderHasFixedPurchase
						|| string.IsNullOrEmpty(cart.OrderCombineParentOrderId))
							? item.IsFixedPurchase
							: false },
					{ Constants.FIELD_ORDERITEM_PRODUCTSALE_ID, item.ProductSaleId },
					{ CONST_KEY_PRODUCT_OPTION_VALUE_SETTINGS, item.ProductOptionSettingList },
					{ Constants.FIELD_PRODUCT_IMAGE_HEAD, item.ProductVariationImageHead },
					{ Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_VALUE, item.FixedPurchaseDiscountValue.ToString() },
					{ Constants.FIELD_ORDERITEM_FIXED_PURCHASE_DISCOUNT_TYPE, item.FixedPurchaseDiscountType },
					{ Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID, item.SubscriptionBoxCourseId },
					{ Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT, item.SubscriptionBoxFixedAmount },
					{ CONST_HASHKEY_CART_PRODUCT_IS_ORDER_COMBINED, item.IsOrderCombine },
				}).ToList();
		return orderItems;
	}

	/// <summary>
	/// カート情報から付帯情報作成(注文同梱)
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="cartProduct">カート商品情報</param>
	/// <returns>商品付帯情報一覧</returns>
	private ProductOptionSettingList CreateProductOptionSettingList(
		string shopId,
		CartProduct cartProduct)
	{
		var productOptionSettingList = new ProductOptionSettingList(shopId, cartProduct.ProductId);
		if (cartProduct.ProductOptionSettingList.Items.Count == 0) return productOptionSettingList;

		foreach (var productOptionSetting in productOptionSettingList.Items)
		{
			var targetItem =
				cartProduct.ProductOptionSettingList.Items.FirstOrDefault(
					item => (item.ValueName == productOptionSetting.ValueName));
			productOptionSetting.SelectedSettingValue = ((targetItem != null) ? targetItem.SelectedSettingValue : null);
		}
		return productOptionSettingList;
	}

	/// <summary>
	/// 値引関連情報を画面項目に設定
	/// </summary>
	/// <param name="cart">カート</param>
	private void SetOrderDiscountInfoFromCart(CartObject cart)
	{
		// 会員ランク割引
		lMemberRankDiscount.Text = GetEncodedHtmlDisplayMessage(cart.MemberRankDiscount.ToPriceString(true));

		// 定期会員割引
		lFixedPurchaseMemberDiscountAmount.Text =
			GetEncodedHtmlDisplayMessage(cart.FixedPurchaseMemberDiscountAmount.ToPriceString(true));

		// ポイント利用額
		lPointUsePrice.Text = GetEncodedHtmlDisplayMessage(cart.UsePointPrice.ToPriceString(true));

		// クーポン割引額
		lCouponUsePrice.Text = GetEncodedHtmlDisplayMessage(cart.UseCouponPrice.ToPriceString(true));

		// 定期購入割引額
		lFixedPurchaseDiscountPrice.Text =
			GetEncodedHtmlDisplayMessage(cart.FixedPurchaseDiscount.ToPriceString(true));

		// 調整金額
		tbOrderPriceRegulation.Text = cart.PriceRegulation.ToPriceString();

		// 調整金額メモ
		tbRegulationMemo.Text = cart.RegulationMemo;

		// 適用クーポン
		tbCouponUse.Text = (cart.Coupon != null)
			? cart.Coupon.CouponCode
			: string.Empty;

		// ポイント情報
		tbPointUse.Text = cart.UsePoint.ToPriceString();
	}

	/// <summary>
	/// 配送情報を画面項目に設定
	/// </summary>
	/// <param name="cart">カート</param>
	private void SetOrderShippingMethodFromCart(CartObject cart)
	{
		var cartShipping = cart.GetShipping();
		if (cartShipping.ShippingDate.HasValue)
		{
			var value = cartShipping.ShippingDate.Value.ToString(CONST_FORMAT_SHORT_DATE);
			var item = ddlShippingDate.Items.FindByValue(value);
			if (item != null)
			{
				item.Selected = true;
			}
			else
			{
				var text = DateTimeUtility.ToStringForManager(
					cartShipping.ShippingDate.Value,
					DateTimeUtility.FormatType.LongDateWeekOfDay2Letter);
				ddlShippingDate.Items.Insert(0, new ListItem(text, value));
				ddlShippingDate.Items[0].Selected = true;
			}
		}
		var deliveryCompany = this.DeliveryCompanyList
			.First(item => (item.DeliveryCompanyId == cartShipping.DeliveryCompanyId));
		RefreshShippingTimeViewFromShippingType(deliveryCompany, cartShipping.IsExpress, true);
		ddlShippingTime.SelectedValue = cartShipping.ShippingTime;
		ddlDeliveryCompany.SelectedValue = cartShipping.DeliveryCompanyId;
		ddlShippingMethod.SelectedValue = cartShipping.ShippingMethod;

		if (cart.IsExpressDelivery == false)
		{
			// 配送指定更新
			var shopShipping = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, cart.ShippingType);
			RefreshDeliverySpecificationFromShippingType(cart, shopShipping, true);

			// 確認画面からの遷移かつメール便の場合に配送会社選択値が初期化されるため再度指定
			ddlDeliveryCompany.SelectedValue = cartShipping.DeliveryCompanyId;
		}
		tbShippingCheckNo.Text = cartShipping.ShippingCheckNo;
	}

	/// <summary>
	/// Set fixed purchase delivery pattern from cart
	/// </summary>
	/// <param name="cart">カート</param>
	/// <param name="shopShipping">Shop Shipping</param>
	private void SetFixedPurchaseDeliveryPatternFromCart(CartObject cart, ShopShippingModel shopShipping)
	{
		var cartShipping = cart.GetShipping();
		if (cart.HasFixedPurchase)
		{
			// 定期配送パターン
			switch (cart.GetShipping().FixedPurchaseKbn)
			{
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
					rbMonthlyPurchase_Date.Checked = true;
					ddlMonth.SelectedValue = cartShipping.FixedPurchaseSetting.Split(',')[0];
					ddlMonthlyDate.SelectedValue = cartShipping.FixedPurchaseSetting.Split(',')[1];
					break;

				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
					rbMonthlyPurchase_WeekAndDay.Checked = true;
					var splitedFixedPurchaseSetting = cartShipping.FixedPurchaseSetting.Split(',');
					ddlIntervalMonths.SelectedValue = splitedFixedPurchaseSetting[0];
					ddlWeekOfMonth.SelectedValue = splitedFixedPurchaseSetting[1];
					ddlDayOfWeek.SelectedValue = splitedFixedPurchaseSetting[2];
					break;

				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
					rbRegularPurchase_IntervalDays.Checked = true;
					ddlIntervalDays.SelectedValue = cartShipping.FixedPurchaseSetting
						.Replace("(", string.Empty)
						.Replace(")", string.Empty);
					break;

				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
					rbPurchase_EveryNWeek.Checked = true;
					var fpSettings = cartShipping.FixedPurchaseSetting.Split(',');
					ddlFixedPurchaseEveryNWeek_Week.SelectedValue = fpSettings[0];
					ddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue = fpSettings[1];
					break;

				default:
					break;
			}
			hfDaysRequired.Value = cartShipping.FixedPurchaseDaysRequired.ToString();
			hfMinSpan.Value = cartShipping.FixedPurchaseMinSpan.ToString();
			cartShipping.ShippingDate = GetShippingDate();

			var calculationMode = shopShipping.CanUseFixedPurchaseFirstShippingDateNextMonth(cartShipping.FixedPurchaseKbn)
				? Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE
				: NextShippingCalculationMode.Earliest;

			SetFirstShippingDate(
				cartShipping.FixedPurchaseKbn,
				cartShipping.FixedPurchaseSetting,
				shopShipping.FixedPurchaseMinimumShippingSpan,
				calculationMode,
				shopShipping,
				cartShipping);

			SetNextShippingDate(
				cartShipping.FixedPurchaseKbn,
				cartShipping.FixedPurchaseSetting,
				cartShipping.FirstShippingDate,
				cartShipping.NextShippingDate,
				cartShipping.NextNextShippingDate,
				cartShipping.FixedPurchaseMinSpan,
				cartShipping,
				shopShipping);
		}

		if (cartShipping.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY
			|| cartShipping.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS)
		{
			ddlNextShippingDate.Visible = true;
		}
		else if (cartShipping.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE
			|| cartShipping.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY)
		{
			ddlNextShippingDate.Visible = dvShippingDate.Visible;
		}

		ddlNextShippingDate.Visible = ddlNextShippingDate.Visible
			&& (this.CombineParentOrderHasFixedPurchase == false)
			&& cart.HasFixedPurchase;
	}

	/// <summary>
	/// 決済情報を画面項目に設定
	/// </summary>
	/// <param name="cart">カート</param>
	private void SetOrderPaymentFromCart(CartObject cart)
	{
		ddlOrderPaymentKbn.SelectedValue = cart.Payment.PaymentId;
		switch (ddlOrderPaymentKbn.SelectedValue)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
				{
					dvPaymentKbnCredit.Visible = true;
					if ((string.IsNullOrEmpty(cart.Payment.CreditCardBranchNo) == false)
						&& (this.IsBackFromConfirm == false))
					{
						var creditCard = UserCreditCard.Get(cart.CartUserId, int.Parse(cart.Payment.CreditCardBranchNo));
						var creditCardDispName = (string.IsNullOrEmpty(creditCard.CardDispName))
							? Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME
							: creditCard.CardDispName;
						ddlUserCreditCard.Items.Add(new ListItem(creditCardDispName, creditCard.BranchNo.ToString()));

						foreach (ListItem item in ddlUserCreditCard.Items)
						{
							item.Selected = (item.Value == cart.Payment.CreditCardBranchNo);
						}

						this.OrderCombineCardInfo = creditCard;

						DisplayCreditInputForm();

						if ((string.IsNullOrEmpty(cart.Payment.CreditInstallmentsCode) == false)
							&& (dllCreditInstallments.Items.FindByValue(cart.Payment.CreditInstallmentsCode) != null))
						{
							dllCreditInstallments.SelectedValue = cart.Payment.CreditInstallmentsCode;
						}
					}
					else if (this.IsBackFromConfirm)
					{
						hfCreditToken.Value = (cart.Payment.CreditToken != null)
							? cart.Payment.CreditToken.ToString()
							: string.Empty;

						var existsBranchNo = ddlUserCreditCard.Items
							.Cast<ListItem>()
							.Select(item => item.Value)
							.Contains(cart.Payment.CreditCardBranchNo);
						if (existsBranchNo == false)
						{
							ddlUserCreditCard.Items.Add(
								new ListItem(
									Constants.CREDITCARD_UNREGIST_DEFAULT_DISPLAY_NAME,
									cart.Payment.CreditCardBranchNo));
						}

						ddlUserCreditCard.SelectedValue = cart.Payment.CreditCardBranchNo;
						if ((OrderCommon.CreditTokenUse == false)
							|| (cart.Payment.CreditToken != null))
						{
							ddlCreditCardCompany.SelectedValue = OrderCommon.CreditCompanySelectable
								? cart.Payment.CreditCardCompany
								: string.Empty;
							tbCreditCardNo1.Text = cart.Payment.CreditCardNo1;
							ddlCreditExpireMonth.SelectedValue = cart.Payment.CreditExpireMonth;
							ddlCreditExpireYear.SelectedValue = cart.Payment.CreditExpireYear;
							tbCreditAuthorName.Text = cart.Payment.CreditAuthorName;
						}
						if (cart.Payment.UserCreditCardRegistable)
						{
							cbRegistCreditCard.Checked = true;
							tbUserCreditCardName.Text = cart.Payment.UserCreditCardName;
						}
						dllCreditInstallments.SelectedValue = cart.Payment.CreditInstallmentsCode;
						DisplayCreditInputForm();

						foreach (ListItem item in ddlUserCreditCard.Items)
						{
							item.Selected = (item.Value == cart.Payment.CreditCardBranchNo);
						}
					}
				}
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE:
				{
					if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Gmo)
					{
						ddlGmoCvsType.Items.Clear();
						ddlGmoCvsType.Items.AddRange(
						ValueText.GetValueItemArray(
						Constants.TABLE_PAYMENT,
						Constants.PAYMENT_GMO_CVS_TYPE));

						foreach (ListItem item in ddlGmoCvsType.Items)
						{
							item.Selected = (item.Value == cart.Payment.GmoCvsType);
						}
					}

					if (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten)
					{
						ddlRakutenCvsType.Items.Clear();
						ddlRakutenCvsType.Items.AddRange(
							ValueText.GetValueItemArray(
								Constants.TABLE_PAYMENT,
								Constants.PAYMENT_RAKUTEN_CVS_TYPE));

						foreach (ListItem item in ddlRakutenCvsType.Items)
						{
							item.Selected = (item.Value == cart.Payment.RakutenCvsType);
						}
					}
				}
				break;
		}
		dvPaymentErrorMessages.Visible = false;

		// 決済情報画面更新
		ddlOrderPaymentKbn_SelectedIndexChanged(null, null);
	}

	/// <summary>
	/// ユーザークーポン一覧URL作成
	/// </summary>
	/// <param name="actionStatus">アクションステータス</param>
	/// <returns>URL</returns>
	protected string CreateUserCouponListUrl(string actionStatus)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_USER_COUPON_LIST)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, actionStatus)
			.AddParam(Constants.REQUEST_KEY_USER_ID, this.CouponUserId)
			.AddParam(Constants.REQUEST_KEY_USER_MAIL_ADDR, this.CouponMailAddress)
			.AddParam(Constants.REQUEST_KEY_ORDER_OWNER_KBN, hfOwnerKbn.Value)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// Create the owner tel2.
	/// </summary>
	/// <param name="ownerInfo">The owner information.</param>
	private void CreateOwnerTel2(Hashtable ownerInfo)
	{
		if (string.IsNullOrEmpty(hfUserId.Value))
		{
			if (this.IsOwnerAddrJp)
			{
				// Input tel 2
				var tel2_1 = StringUtility.ToHankaku(tbOwnerTel2_1.Text.Trim());
				var tel2_2 = StringUtility.ToHankaku(tbOwnerTel2_2.Text.Trim());
				var tel2_3 = StringUtility.ToHankaku(tbOwnerTel2_3.Text.Trim());

				ownerInfo.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1, tel2_1);
				ownerInfo.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2, tel2_2);
				ownerInfo.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3, tel2_3);
				ownerInfo.Add(Constants.FIELD_ORDEROWNER_OWNER_TEL2,
					UserService.CreatePhoneNo(tel2_1, tel2_2, tel2_3));

				if ((string.IsNullOrEmpty(tel2_1) == false)
					|| (string.IsNullOrEmpty(tel2_2) == false)
					|| (string.IsNullOrEmpty(tel2_3) == false))
				{
					ownerInfo.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1 + "_for_check", tel2_1);
					ownerInfo.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2 + "_for_check", tel2_2);
					ownerInfo.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3 + "_for_check", tel2_3);
				}
			}
			else
			{
				ownerInfo.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1, string.Empty);
				ownerInfo.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2, string.Empty);
				ownerInfo.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3, string.Empty);
				ownerInfo.Add(Constants.FIELD_ORDEROWNER_OWNER_TEL2,
					StringUtility.ToHankaku(tbOwnerTel2Global.Text.Trim()));
			}
		}
		else
		{
			// Get tel2 for search user
			var tel2 = new Tel(lOwnerTel2.Text);

			ownerInfo.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_1, tel2.Tel1);
			ownerInfo.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_2, tel2.Tel2);
			ownerInfo.Add(CartOwner.FIELD_ORDEROWNER_OWNER_TEL2_3, tel2.Tel3);
			ownerInfo.Add(Constants.FIELD_ORDEROWNER_OWNER_TEL2, lOwnerTel2.Text);
		}
	}

	/// <summary>
	/// 注文同梱済みカートから入力項目に値セット
	/// </summary>
	/// <param name="combinedCart">注文同梱済みカート</param>
	private void SetOrderInfoFromCombinedCart(CartObject combinedCart)
	{
		// 基本情報
		ddlOrderKbn.SelectedValue = combinedCart.OrderKbn;

		// 注文者情報
		SetOwner(combinedCart.OrderUserId);

		// 配送先情報
		combinedCart.UpdateAnotherShippingFlag();
		SetOrderShippingFromCart(combinedCart);
		this.ddlShippingKbnList.SelectedValue = Constants.SHIPPING_KBN_LIST_USER_INPUT;

		// 注文商品情報
		var orderItems = CreateOrderItemFromCart(combinedCart);
		BindDataOrderItem(orderItems);

		// 注文値引情報
		SetOrderDiscountInfoFromCart(combinedCart);

		// 注文メモ
		rOrderMemos.Visible = false;
		var orderMemoSettings = GetOrderMemoSetting(Constants.FLG_ORDER_MEMO_SETTING_DISP_FLG_PC);
		if (orderMemoSettings.Count > 0)
		{
			dvOrderMemoForCombine.Visible = true;
			tbMemoForCombine.Text = combinedCart.GetOrderMemos();
		}
		else
		{
			dvOrderMemoForCombine.Visible = false;
		}

		// 決済連携メモ
		tbPaymentMemo.Text = combinedCart.PaymentMemo;
		tbRegulationMemo.Text = combinedCart.RegulationMemo;
		tbManagerMemo.Text = combinedCart.ManagementMemo;
		tbShippingMemo.Text = combinedCart.ShippingMemo;
		tbRelationMemo.Text = combinedCart.RelationMemo;

		// 領収書情報
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			rblReceiptFlg.SelectedValue = combinedCart.ReceiptFlg;
			tbReceiptAddress.Text = combinedCart.ReceiptAddress;
			tbReceiptProviso.Text = combinedCart.ReceiptProviso;
		}

		// Set user shipping if order combine is convenience store
		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED
			&& Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
			&& (combinedCart.GetShipping().ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE))
		{
			var shopShipping = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, combinedCart.ShippingType);
			BindShippingList(this.UserShippingAddress, shopShipping);
			ddlUserShipping.SelectedValue = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE;
			ddlShippingReceivingStoreType.SelectedValue = combinedCart.GetShipping().ShippingReceivingStoreType;
			RefreshPaymentViewFromShippingType(combinedCart, shopShipping);
		}

		// 表示更新
		RefreshItemsAndPriceView();

		// 配送設定(表示更新後に行う)
		SetOrderShippingMethodFromCart(combinedCart);

		// 決済情報
		SetOrderPaymentFromCart(combinedCart);

		// Remove Paidy payment if order combine payment is not Paidy payment
		var paidyPaymentKbn = ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY);
		if ((ddlOrderPaymentKbn.SelectedValue != Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)
			&& (paidyPaymentKbn != null))
		{
			ddlOrderPaymentKbn.Items.Remove(paidyPaymentKbn);
		}

		// Remove Ec payment if order combine payment is not Ec payment
		var ecPaymentKbn = ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY);
		if (ecPaymentKbn != null)
		{
			ddlOrderPaymentKbn.Items.Remove(ecPaymentKbn);
		}

		// Remove NewebPay Payment If Regist New Order By Order Combine
		var newebPaymentKbn = ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY);
		if (newebPaymentKbn != null)
		{
			ddlOrderPaymentKbn.Items.Remove(newebPaymentKbn);
		}

		// Remove PayPay payment if regist new order by order combine
		var payPayPaymentKbn = ddlOrderPaymentKbn.Items.FindByValue(Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY);
		if (payPayPaymentKbn != null)
		{
			ddlOrderPaymentKbn.Items.Remove(payPayPaymentKbn);
		}

		// 配送パターンは親注文が定期でなく、子注文に定期購入がある場合のみ表示
		// 異なるコース同士での頒布会商品同梱の場合も表示
		dvShippingFixedPurchase.Visible =
			(combinedCart.IsBeforeCombineCartHasFixedPurchase
				&& (combinedCart.IsCombineParentOrderHasFixedPurchase == false))
			|| (combinedCart.HasSubscriptionBox
				&& (combinedCart.IsOrderCombinedWithSameSubscriptionBoxCourse() == false));

		// 表示更新後再バインド
		BindDataOrderItem(orderItems);

		// コンバージョン情報をセット
		if (combinedCart.ContentsLog != null)
		{
			ddlInflowContentsType.SelectedValue = (combinedCart.ContentsLog.ContentsType ?? string.Empty);
			tbInflowContentsId.Text = (combinedCart.ContentsLog.ContentsId ?? string.Empty);
		}

		// 再計算
		RefreshItemsAndPriceView();

		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			var input = new OrderExtendInput(combinedCart.OrderExtend.ToDictionary(v => v.Key, v => v.Value.Value));
			SetOrderExtendFromUserExtendObject(rOrderExtendInput, input);
		}
	}

	#region トークン決済系
	/// <summary>
	/// （トークン決済向け）カード情報編集（再入力）リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbEditCreditCardNoForToken_Click(object sender, EventArgs e)
	{
		// トークンなどクレジットカード情報削除
		ResetCreditTokenInfoFromForm(null, "DUMMY");
		SwitchDisplayForCreditTokenInput();
	}

	/// <summary>
	/// トークンが入力されていたら入力画面を切り替える
	/// </summary>
	protected override void SwitchDisplayForCreditTokenInput()
	{
		if (OrderCommon.CreditTokenUse == false)
		{
			divCreditCardForTokenAcquired.Visible = false;
			return;
		}

		var cardInput = new CommonPageProcess.WrappedCreditCardInputs(this);
		var sessionCartPayment = GetSessionCartPayment();
		if ((sessionCartPayment != null)
			&& (sessionCartPayment.CreditToken != null)
			&& (sessionCartPayment.CreditToken.ExpireDate < DateTime.Now))
		{
			cardInput.WhfCreditToken.Value = string.Empty;
			cardInput.WtbCard1.Text =
				cardInput.WtbCard2.Text =
				cardInput.WtbCard3.Text = string.Empty;
			cardInput.WtbSecurityCode.Text = string.Empty;
			sessionCartPayment.CreditToken = null;
		}

		// 表示切り替え
		divCreditCardNoToken.Visible = (HasCreditToken() == false);
		divCreditCardForTokenAcquired.Visible = HasCreditToken();

		var lastFourDigit = tbCreditCardNo1.Text.Trim()
			.Replace(Constants.CHAR_MASKING_FOR_TOKEN, string.Empty);
		cardInput.WlCreditCardCompanyNameForTokenAcquired.Text = GetEncodedHtmlDisplayMessage(
			cardInput.WddlCardCompany.SelectedText);
		cardInput.WlLastFourDigitForTokenAcquired.Text = GetEncodedHtmlDisplayMessage(lastFourDigit);
		cardInput.WlExpirationMonthForTokenAcquired.Text = GetEncodedHtmlDisplayMessage(
			cardInput.WddlExpireMonth.SelectedValue);
		cardInput.WlExpirationYearForTokenAcquired.Text = GetEncodedHtmlDisplayMessage(
			cardInput.WddlExpireYear.SelectedValue);
		cardInput.WlCreditAuthorNameForTokenAcquired.Text = GetEncodedHtmlDisplayMessage(
			cardInput.WtbAuthorName.Text);
	}

	/// <summary>
	/// セッションカート決済情報取得
	/// </summary>
	/// <returns>カート決済情報</returns>
	private CartPayment GetSessionCartPayment()
	{
		var sessionParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_REGIST_INPUT];
		if ((sessionParam != null)
			&& (sessionParam["cart"] != null))
		{
			return ((CartObject)sessionParam["cart"]).Payment;
		}
		return null;
	}

	/// <summary>
	/// カード情報取得JSスクリプト作成
	/// </summary>
	/// <returns>
	/// カード情報取得スクリプト
	/// 文字列を返すのでevalで動的実行すれば連想配列でカード情報がとれます
	/// </returns>
	protected string CreateGetCardInfoJsScriptForCreditToken()
	{
		if (ddlOrderPaymentKbn.SelectedValue != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) return string.Empty;

		var cardInfoScript = CreateGetCardInfoJsScriptForCreditTokenInner();
		return cardInfoScript;
	}
	#endregion

	/// <summary>
	/// 利用クレジットカードドロップダウン変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUserCreditCard_SelectedIndexChanged(object sender, EventArgs e)
	{
		DisplayCreditInputForm();
	}

	/// <summary>
	/// クレジットカード登録チェックボックスチェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbRegistCreditCard_CheckedChanged(object sender, EventArgs e)
	{
		DisplayCreditInputForm();
	}

	/// <summary>
	/// クレジット入力フォーム表示切り替え
	/// </summary>
	private void DisplayCreditInputForm()
	{
		// 支払回数表示
		dvInstallments.Visible = OrderCommon.CreditInstallmentsSelectable
			&& (this.CanUseCreditCardNoForm
				|| (ddlUserCreditCard.SelectedValue != CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW));

		switch (ddlUserCreditCard.SelectedValue)
		{
			case CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW:
				divUserCreditCard.Visible = false;
				divCreditCardInputNew.Visible = true;
				dvRegistCreditCard.Visible
					= dvCreditCardName.Visible
					= OrderCommon.GetCreditCardRegistable(
						this.IsUser,
						(ddlUserCreditCard.Items.Count - 1));
				if (this.IsUserPayTg)
				{
					btnGetCreditCardInfo.Enabled = (this.IsSuccessfulCardRegistration == false);
					ddlCreditExpireMonth.Enabled = (this.IsSuccessfulCardRegistration == false);
					ddlCreditExpireYear.Enabled = (this.IsSuccessfulCardRegistration == false);
					tbCreditCardNo1.Enabled = (this.IsSuccessfulCardRegistration == false);
					trCreditExpire.Visible = this.IsSuccessfulCardRegistration;
					tdCreditNumber.Visible = this.IsSuccessfulCardRegistration;
					tdGetCardInfo.Visible = (this.IsSuccessfulCardRegistration == false);
				}
				break;

			default:
				int branchNo;
				if (int.TryParse(ddlUserCreditCard.SelectedValue, out branchNo))
				{
					var userCreditCard = DomainFacade.Instance.UserCreditCardService.Get(hfUserId.Value, branchNo);
					lCreditCompany.Text =
						GetEncodedHtmlDisplayMessage(
							ValueText.GetValueText(
								Constants.TABLE_ORDER,
								OrderCommon.CreditCompanyValueTextFieldName,
								userCreditCard.CompanyCode));
					lCreditLastFourDigit.Text = GetEncodedHtmlDisplayMessage(userCreditCard.LastFourDigit);
					lCreditExpirationMonth.Text = GetEncodedHtmlDisplayMessage(userCreditCard.ExpirationMonth);
					lCreditExpirationYear.Text = GetEncodedHtmlDisplayMessage(userCreditCard.ExpirationYear);
					lCreditAuthorName.Text = GetEncodedHtmlDisplayMessage(userCreditCard.AuthorName);
					divUserCreditCard.Visible = true;
					divCreditCardInputNew.Visible = false;
					dvRegistCreditCard.Visible = dvCreditCardName.Visible = false;
				}
				break;
		}
		dvCreditCardName.Visible = cbRegistCreditCard.Checked;
	}

	/// <summary>
	/// 配送方法選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingMethod_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
		{
			ddlFirstShippingDate.Visible = false;
		}
		var orderItems = CreateOrderItemInput();

		// 商品入力チェック
		if (string.IsNullOrEmpty(CheckItemsAndPricesInput(orderItems)) == false) return;

		// カート作成
		var hasError = false;
		var cart = CreateCart(orderItems, ref hasError);

		// 配送種別取得
		var shopShipping = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, cart.ShippingType);
		if (shopShipping == null)
		{
			ddlDeliveryCompany.Items.Clear();
			return;
		}

		// 配送指定更新
		RefreshDeliverySpecificationFromShippingType(cart, shopShipping, true);

		// 再計算
		RefreshItemsAndPriceView();

		// 決済情報更新
		RefreshPaymentViewFromShippingType(cart, shopShipping);

		// Adjust first and next shipping date
		AdjustFirstAndNextShippingDate(cart, shopShipping);
	}

	/// <summary>
	/// 配送会社選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDeliveryCompany_SelectedIndexChanged(object sender, EventArgs e)
	{
		var orderItems = CreateOrderItemInput();

		// 商品入力チェック
		if (string.IsNullOrEmpty(CheckItemsAndPricesInput(orderItems)) == false) return;

		// カート作成
		var hasError = false;
		var cart = CreateCart(orderItems, ref hasError);

		// 配送種別取得
		var shopShipping = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, cart.ShippingType);
		if (shopShipping == null)
		{
			ddlDeliveryCompany.Items.Clear();
			return;
		}

		// 配送指定日更新
		RefreshShippingDateViewFromShippingType(shopShipping, cart, true);

		cart.GetShipping().ShippingMethod = ddlShippingMethod.SelectedValue;
		var deliveryCompany = this.DeliveryCompanyList
			.First(item => (item.DeliveryCompanyId == ddlDeliveryCompany.SelectedValue));
		RefreshShippingTimeViewFromShippingType(deliveryCompany, cart.IsExpressDelivery, true);

		// 再計算
		RefreshItemsAndPriceView();
	}

	/// <summary>
	/// 配送希望日チェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingDate_SelectedIndexChanged(object sender, EventArgs e)
	{
		var orderItems = CreateOrderItemInput();

		// 商品入力チェック
		if (string.IsNullOrEmpty(CheckItemsAndPricesInput(orderItems)) == false) return;

		var hasError = false;
		var cart = CreateCart(orderItems, ref hasError);

		var cartShipping = cart.GetShipping();
		cartShipping.ShippingDate = GetShippingDate();
		cartShipping.ShippingMethod = ddlShippingMethod.SelectedValue;
		cartShipping.DeliveryCompanyId = ddlDeliveryCompany.SelectedValue;

		var shippingDateErrorMessage = string.Empty;
		if ((string.IsNullOrEmpty(ddlShippingDate.SelectedValue) == false)
			&& (OrderCommon.CanCalculateScheduledShippingDate(this.LoginOperatorShopId, cartShipping) == false))
		{
			shippingDateErrorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_SHIPPINGDATE_INVALID)
				.Replace("@@ 1 @@", DateTimeUtility.ToStringForManager(
					HolidayUtil.GetShortestDeliveryDate(
						cart.ShopId,
						cartShipping.DeliveryCompanyId,
						cartShipping.Addr1,
						cartShipping.Zip.Replace("-", string.Empty)),
					DateTimeUtility.FormatType.LongDateWeekOfDay2Letter,
					Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE));
		}
		lShippingDateErrorMessages.Text = GetEncodedHtmlDisplayMessage(shippingDateErrorMessage);
		var shopShipping = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, cart.ShippingType);

		// Adjust first and next shipping date
		AdjustFirstAndNextShippingDate(cart, shopShipping);
	}

	/// <summary>
	/// 子注文・親注文が共に定期購入の場合、親注文の定期購入設定文言を取得
	/// </summary>
	/// <param name="fixedPurchaseKbn">定期購入区分</param>
	/// <param name="fixedPurchaseSetting">定期購入設定</param>
	/// <returns>定期購入設定文言</returns>
	protected string GetFixedPurchasePatternSettingMessage(
		string fixedPurchaseKbn,
		string fixedPurchaseSetting)
	{
		var fpSettingMessage = OrderCommon.CreateFixedPurchaseSettingMessage(
			fixedPurchaseKbn,
			fixedPurchaseSetting);
		return fpSettingMessage;
	}

	/// <summary>
	/// 月間隔日付指定選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbMonthlyPurchase_Date_CheckedChanged(object sender, EventArgs e)
	{
		ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 週・曜日指定選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbMonthlyPurchase_WeekAndDay_CheckedChanged(object sender, EventArgs e)
	{
		ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 週間隔・曜日指定
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbPurchase_EveryNWeek_CheckedChanged(object sender, EventArgs e)
	{
		ddlFixedPurchaseEveryNWeek_OnSelectedIndexChanged(sender, e);
		ddlFirstShippingDate.Visible = false;
	}

	/// <summary>
	/// 配送日間隔指定選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbRegularPurchase_IntervalDays_CheckedChanged(object sender, EventArgs e)
	{
		ddlIntervalDays_OnSelectedIndexChanged(sender, e);
		ddlFirstShippingDate.Visible = false;
	}

	/// <summary>
	/// 注文者情報の郵便番号変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void tbOwnerZip2_OnTextChanged(object sender, EventArgs e)
	{
		// 配送先を注文者と同じにする場合は初回配送日などを再計算
		if (this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER) ddlIntervalDays_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 注文者情報の都道府県変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlOwnerAddr1_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		// 配送先を注文者と同じにする場合は初回配送日などを再計算
		if (this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER) ddlIntervalDays_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送先情報の郵便番号変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void tbShippingZip2_OnTextChanged(object sender, EventArgs e)
	{
		ddlIntervalDays_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送先情報の都道府県変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingAddr1_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		ddlIntervalDays_OnSelectedIndexChanged(sender, e);
	}

	/// <summary>
	/// 配送間隔月ドロップダウン値変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		dvFixedPurchasePatternErrorMessages.Visible = false;
		if (((rbMonthlyPurchase_Date.Checked == false)
				&& (rbMonthlyPurchase_WeekAndDay.Checked == false))
			|| (string.IsNullOrEmpty(ddlMonth.SelectedValue)
				&& string.IsNullOrEmpty(ddlIntervalMonths.SelectedValue))) return;

		// 選択している配送間隔月が規定の値かどうかのチェック
		CheckIntervalValueAndSetAlertMessage(
			rbMonthlyPurchase_Date.Checked
				? ddlMonth.SelectedValue
				: ddlIntervalMonths.SelectedValue,
			Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN1_SETTING);
		if ((this.IsCanAddShippingDate == false)
			&& ddlShippingDate.Visible
			&& ddlFirstShippingDate.Visible
			&& ddlShippingDate.Items[0].Text == lFirstShippingDate.Text)
		{
			ddlShippingDate.Items.RemoveAt(0);
		}
	}

	/// <summary>
	/// 配送間隔日ドロップダウン値変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlIntervalDays_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		dvFixedPurchasePatternErrorMessages.Visible = false;
		if ((rbRegularPurchase_IntervalDays.Checked == false)
			|| string.IsNullOrEmpty(ddlIntervalDays.SelectedValue)) return;

		// 選択している配送間隔日チェック、アラートメッセージのセット
		CheckIntervalValueAndSetAlertMessage(
			ddlIntervalDays.SelectedValue,
			Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN3_SETTING);
	}

	/// <summary>
	/// 週間隔・曜日指定ドロップダウン値変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlFixedPurchaseEveryNWeek_OnSelectedIndexChanged(object sender, EventArgs e)
	{
		dvFixedPurchasePatternErrorMessages.Visible = false;
		if ((rbPurchase_EveryNWeek.Checked == false)
			|| ((string.IsNullOrEmpty(ddlFixedPurchaseEveryNWeek_Week.SelectedValue)
				&& (string.IsNullOrEmpty(ddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue))))) return;

		// 選択している配送間隔日チェック、アラートメッセージのセット
		CheckIntervalValueAndSetAlertMessage(
			ddlFixedPurchaseEveryNWeek_Week.SelectedValue,
			Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING);

	}

	/// <summary>
	/// 選択している配送間隔月・日が規定の値かどうかのチェック
	/// かつアラートメッセージのセット
	/// </summary>
	/// <param name="value">選択中の値</param>
	/// <param name="fieldName">チェックする項目名</param>
	private void CheckIntervalValueAndSetAlertMessage(string value, string fieldName)
	{
		// 注意喚起メッセージを設定
		var messageTemp = new StringBuilder();
		lFixedPurchasePatternErrorMessage.Text = GetEncodedHtmlDisplayMessage(
			(CheckSpecificIntervalMonthAndDay(CreateOrderItemInput(), value, fieldName) == false)
				? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FIXED_PURCHASE_DATE_SPECIFIC_INTERVAL_INVALID)
				: string.Empty);

		this.IsUpdateFixedPurchaseShippingPattern = true;
		RefreshItemsAndPriceView();

		dvFixedPurchasePatternErrorMessages.Visible = (lFixedPurchasePatternErrorMessage.Text.Length > 0);
	}

	/// <summary>
	/// 同梱するボタンクリック時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnOrderCombine_Click(object sender, EventArgs e)
	{
		var childOrderItems = CreateOrderItemInput();
		var errorMessage = CheckItemsAndPricesInput(childOrderItems);

		// 注文情報入力チェック、エラーの場合処理中止
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			DisplayItemsErrorMessages(errorMessage);
			return;
		}

		var hasError = false;
		var childCart = CreateCart(childOrderItems, ref hasError);

		this.CombineParentOrderId = ((HiddenField)((Button)sender).Parent.FindControl("hfParentOrderId")).Value;
		var parentOrder = DomainFacade.Instance.OrderService.Get(this.CombineParentOrderId);

		CartObject combinedCart;
		var encodedErrorMessageForCombine = GetEncodedHtmlDisplayMessage(
			OrderCombineUtility.CreateCombinedCart(parentOrder, childCart, out combinedCart));

		// 同コース定額頒布会での注文同梱の場合、頒布会コースの数量・種類数・金額チェックを行う
		// （他にエラーが無い場合）
		if (string.IsNullOrEmpty(encodedErrorMessageForCombine)
			&& (combinedCart.CheckFixedAmountForCombineWithSameCourse() == false))
		{
			encodedErrorMessageForCombine = HtmlSanitizer.HtmlEncodeChangeToBr(
				WebMessages.GetMessages(
						CommerceMessages.ERRMSG_ORDERCOMBINE_SUBSCRIPTION_BOX_FIXED_AMOUNT_CHECK_INVALID)
					.Replace("@@ 1 @@", combinedCart.SubscriptionBoxErrorMsg));
		}

		lOrderCombineAlertMessage.Text = encodedErrorMessageForCombine;
		dvNoneCombinable.Visible
			= (string.IsNullOrEmpty(lOrderCombineAlertMessage.Text) == false);
		dvCombinable.Visible
			= (string.IsNullOrEmpty(lOrderCombineAlertMessage.Text));

		// 注文同梱カート作成でエラーがあった場合、処理中止
		if (string.IsNullOrEmpty(lOrderCombineAlertMessage.Text) == false) return;

		this.CombineParentOrderHasFixedPurchase = parentOrder.IsFixedPurchaseOrder;
		this.IsOrderCombinedWithSameSubscriptionBox
			= combinedCart.HasSubscriptionBox && combinedCart.IsOrderCombinedWithSameSubscriptionBoxCourse();

		var isVisibleShippingFixedPurchase
			= ((this.CombineParentOrderHasFixedPurchase == false)
				|| (combinedCart.HasSubscriptionBox
					&& (combinedCart.IsOrderCombinedWithSameSubscriptionBoxCourse() == false)))
			&& childCart.HasFixedPurchase;
		SetCombinedCart(new CartObject[] { combinedCart }, isVisibleShippingFixedPurchase);

		tbAdvCode.Text = (combinedCart.AdvCodeNew ?? string.Empty);
		if (string.IsNullOrEmpty(tbAdvCode.Text.Trim()) == false)
		{
			var advCode = DomainFacade.Instance.AdvCodeService.GetAdvCodeFromAdvertisementCode(tbAdvCode.Text);
			lbAdvName.Text = (advCode != null)
				? advCode.MediaName
				: string.Empty;
		}
		lOrderPriceTotalBottom.Text = GetEncodedHtmlDisplayMessage(combinedCart.PriceTotal.ToPriceString(true));
		btnClearOrderCombine.Visible = true;

		var orderInfo = new Order(OrderCommon.GetOrder(this.CombineParentOrderId));
		SetOwner(combinedCart.OrderUserId, orderInfo.Owner);

		// 親注文の領収書情報を画面に設定
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			rblReceiptFlg.SelectedValue = combinedCart.ReceiptFlg;
			tbReceiptAddress.Text = combinedCart.ReceiptAddress;
			tbReceiptProviso.Text = combinedCart.ReceiptProviso;
		}
	}

	/// <summary>
	/// Bind Order Combine
	/// </summary>
	private void BindOrderCombine()
	{
		if (cbOrderCombineSelect.Checked == false)
		{
			lOrderCombineAlertMessage.Text = string.Empty;
			rCombinableOrder.DataSource = null;
			rCombinableOrder.DataBind();
			dvNoneCombinable.Visible = false;
			dvCombinable.Visible = false;
			return;
		}

		if (string.IsNullOrEmpty(hfUserId.Value)
			|| string.IsNullOrEmpty(hfShippingType.Value))
		{
			lOrderCombineAlertMessage.Text = string.Empty;
			rCombinableOrder.DataSource = null;
			rCombinableOrder.DataBind();
			dvNoneCombinable.Visible = false;
			dvCombinable.Visible = false;
			return;
		}

		var childOrderItems = CreateOrderItemInput();
		var errorMessage = CheckItemsAndPricesInput(childOrderItems);
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			DisplayItemsErrorMessages(errorMessage);
			return;
		}

		var hasError = false;
		var childCart = CreateCart(childOrderItems, ref hasError);
		var models = OrderCombineUtility.GetCombinableParentOrders(
			this.LoginOperatorShopId,
			hfUserId.Value,
			hfShippingType.Value,
			childCart,
			false,
			false);
		models = models.Where(item => (item.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)).ToArray();

		lOrderCombineAlertMessage.Text = GetEncodedHtmlDisplayMessage(
			(models.Length == 0)
				? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_REGIST_ORDER_COMBINE_ALERT)
				: string.Empty);
		dvNoneCombinable.Visible = (string.IsNullOrEmpty(lOrderCombineAlertMessage.Text) == false);

		if (models.Length <= CONST_MAX_DISPLAY_SHOW_FOR_COMBINE)
		{
			dvOrderCombineSelectList.Attributes.Remove("class");
		}
		else
		{
			AddAttributesForControlDisplay(
				dvOrderCombineSelectList,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_SCROLL_VERTICAL,
				true);
		}

		rCombinableOrder.DataSource = models;
		rCombinableOrder.DataBind();
		dvCombinable.Visible = (string.IsNullOrEmpty(lOrderCombineAlertMessage.Text));
	}

	/// <summary>
	/// Create Simple Cart List For FixedPurchase
	/// </summary>
	/// <param name="fixedPurchaseId">FixedPurchase Id</param>
	/// <param name="userId">User Id</param>
	/// <returns>Cart Object List</returns>
	protected static CartObjectList CreateSimpleCartListForFixedPurchase(string fixedPurchaseId, string userId)
	{
		var fixedPurchase = DomainFacade.Instance.FixedPurchaseService.Get(fixedPurchaseId);
		var user = DomainFacade.Instance.UserService.Get(userId);
		if (user == null) return null;

		var orderRegister = new OrderRegisterFixedPurchaseInner(user.IsMember, Constants.FLG_LASTCHANGED_USER, false, fixedPurchaseId, null);
		var cartObject = new OrderRegisterFixedPurchase(Constants.FLG_LASTCHANGED_USER, false, false, null)
			.CreateCartList(fixedPurchase, user, orderRegister);
		return cartObject;
	}

	/// <summary>
	/// 注文同梱済みカート表示
	/// </summary>
	/// <param name="cartList">同梱済みカート</param>
	/// <param name="isVisibleShippingFixedPurchase">定期購入配送パターンを表示するか</param>
	private void SetCombinedCart(CartObject[] cartList, bool isVisibleShippingFixedPurchase)
	{
		rCombinedCartInfo.DataSource
			= rCombinedCart.DataSource
			= cartList;
		rCombinedCart.DataBind();
		rCombinedCartInfo.DataBind();
		dvCombinedCart.Visible = true;
		dvBeforeCombine.Visible = false;
		this.IsOrderCombined = true;
		AddAttributesForControlDisplay(
			dvShippingTo,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_NONE,
			true);
		AddAttributesForControlDisplay(
			dvShippingInfo,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_NONE,
			true);
		AddAttributesForControlDisplay(
			dvPayment,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_NONE,
			true);
		dvShippingFixedPurchase.Visible = isVisibleShippingFixedPurchase;
		lOrderPriceTotalBottom.Text = cartList.First().PriceTotal.ToPriceString(true);
	}

	/// <summary>
	/// 同梱解除ボタンクリック時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnClearOrderCombine_Click(object sender, EventArgs e)
	{
		rCombinedCart.DataSource = null;
		rCombinedCart.DataBind();
		cbOrderCombineSelect.Checked = this.UseOrderCombine;

		if (this.UseOrderCombine)
		{
			BindOrderCombine();
		}

		this.IsOrderCombined = false;
		this.CombineParentOrderId = string.Empty;
		this.CombineParentOrderHasFixedPurchase = false;
		dvCombinedCart.Visible = false;
		dvBeforeCombine.Visible = true;
		if (this.IsDigitalContents == false)
		{
			AddAttributesForControlDisplay(
				dvShippingTo,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_EMPTY,
				true);
			AddAttributesForControlDisplay(
				dvShippingInfo,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_EMPTY,
				true);
		}
		AddAttributesForControlDisplay(
			dvPayment,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_EMPTY,
			true);
		lOrderCombineAlertMessage.Text = string.Empty;
		var hasFixedPurchase = false;
		foreach (RepeaterItem riItem in rItemList.Items)
		{
			if (((CheckBox)riItem.FindControl("cbFixedPurchase")).Checked)
			{
				hasFixedPurchase = true;
				break;
			}
		}
		dvShippingFixedPurchase.Visible = hasFixedPurchase;
		foreach (RepeaterItem riItem in rCombinableOrder.Items)
		{
			var btnOrderCombine = ((Button)riItem.FindControl("btnOrderCombine"));
			btnOrderCombine.Visible = true;
		}
		lbUserClear.Visible = (string.IsNullOrEmpty(hfUserId.Value));
		lOrderPriceTotalBottom.Text = lOrderPriceTotal.Text;
		SetOwner(hfUserId.Value);
	}

	/// <summary>
	/// 配送希望時間帯文言取得
	/// </summary>
	/// <param name="shippingCompanyId">配送業者ID</param>
	/// <param name="shippingTimeId">配送希望時間帯ID</param>
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
	/// 配送種別名取得
	/// </summary>
	/// <param name="shippingId">配送種別ID</param>
	/// <returns>配送種別名</returns>
	protected string GetShopShippingName(string shippingId)
	{
		var shopShipping = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, shippingId);
		if (shopShipping == null) return string.Empty;

		return shopShipping.ShopShippingName;
	}

	/// <summary>
	/// クレジットトークン取得＆フォームセットJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	protected string CreateGetCreditTokenAndSetToFormJsScript()
	{
		var script = CreateGetCreditTokenAndSetToFormJsScriptInner();
		return script;
	}

	/// <summary>
	/// クレジットトークン向け フォームマスキングJSスクリプト作成
	/// </summary>
	/// <returns>JSスクリプト</returns>
	protected string CreateMaskFormsForCreditTokenJsScript()
	{
		var maskingScripts = CreateMaskFormsForCreditTokenJsScriptInner();
		return maskingScripts;
	}

	/// <summary>
	/// 配送先と注文者が同じにできるかどうか
	/// </summary>
	/// <returns>True：可能、False：不可</returns>
	private bool EnableUseShippingSameAsOwner()
	{
		// 注文者の国が配送可能な国以外の場合は同じにしてはダメ
		var ownerIso = this.ddlOwnerCountry.SelectedValue;
		if (string.IsNullOrEmpty(ownerIso)) return true;

		var countryLocationService = DomainFacade.Instance.CountryLocationService;
		var models = countryLocationService.GetShippingAvailableCountry();

		// 配送可能な国であればOK
		if (models.Any(countryLocation =>
			(countryLocation.CountryIsoCode == ownerIso))) return true;
		return false;
	}

	/// <summary>
	/// 配送先と注文者が同じにするチェックボックスの制御
	/// </summary>
	private void ControlShippingSameAsOwnerCheckBox()
	{
		// GlobalOptionが無効な場合は特に制御しない
		if (Constants.GLOBAL_OPTION_ENABLE == false) return;

		if (EnableUseShippingSameAsOwner() == false)
		{
			this.ddlShippingKbnList.SelectedValue = Constants.SHIPPING_KBN_LIST_USER_INPUT;
			this.ddlShippingKbnList.Visible = false;
			spShippingSameNgMsg.Visible = true;
			return;
		}

		this.ddlShippingKbnList.Visible = true;
		spShippingSameNgMsg.Visible = false;
	}

	/// <summary>
	/// 該当商品のユーザー管理レベル制限チェック
	/// </summary>
	/// <param name="shopId">ショップId</param>
	/// <param name="productId">商品Id</param>
	/// <param name="userLevel">ユーザ管理レベル</param>
	/// <returns>定期購入制限されているかどうか</returns>
	public static bool CheckFixedPurchaseLimitedUserLevel(
		string shopId,
		string productId,
		string userLevel = null)
	{
		if (userLevel == null) return false;
		var fixedPurchaseLimitedUserLevels = ProductCommon.GetProductInfoUnuseMemberRankPrice(
			shopId,
			productId);
		var fixedPurchaseAbleUserLevel = fixedPurchaseLimitedUserLevels
			.Cast<DataRowView>()
			.Select(row => StringUtility.ToEmpty(row[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS]).Split(','))
			.FirstOrDefault();
		if (fixedPurchaseAbleUserLevel == null) return false;
		return fixedPurchaseAbleUserLevel.Contains(userLevel);
	}

	/// <summary>
	/// Add Product Grant Novelty When Auto Additional Flag is ON
	/// </summary>
	/// <param name="cart">Cart Object</param>
	/// <param name="cartNovelties">Cart Novelty List</param>
	private List<string> AddProductGrantNovelty(CartObject cart, CartNovelty[] cartNovelties)
	{
		var addedNovertyIds = new List<string>();
		foreach (var cartNoveltyItem in cartNovelties.Where(item =>
			((item.GrantItemList.Length > 0)
				&& (item.AutoAdditionalFlg == Constants.FLG_NOVELTY_AUTO_ADDITIONAL_FLG_VALID))))
		{
			if (this.DeletedNoveltyIds.Contains(cartNoveltyItem.NoveltyId)) continue;

			// Get info product novelty
			var productInfo = DomainFacade.Instance.ProductService.GetProductVariationAtDataRowView(
				cartNoveltyItem.ShopId,
				cartNoveltyItem.GrantItemList.First().ProductId,
				cartNoveltyItem.GrantItemList.First().VariationId,
				cart.MemberRankId);
			if (productInfo == null) continue;

			// Create info cart product novelty
			var productNovelty = new CartProduct(
				productInfo,
				Constants.AddCartKbn.Normal,
				string.Empty,
				1,
				true,
				new ProductOptionSettingList());
			productNovelty.NoveltyId = cartNoveltyItem.GrantItemList[0].NoveltyId;

			// カート内の同一商品を取得
			var sameItem = cart.GetSameProductWithoutOptionSetting(productNovelty);
			if (sameItem != null) continue;

			// 同一商品がなければ追加
			cart.Items.Add(productNovelty);
			addedNovertyIds.Add(productNovelty.NoveltyId);
		}
		return addedNovertyIds;
	}

	/// <summary>
	/// 表示通貨ロケールID ドロップダウンリスト変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDispCurrencyLocaleId_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(ddlDispCurrencyLocaleId.SelectedValue))
		{
			lDispCurrencyCode.Text
				= hfDispCurrencyCode.Value
				= string.Empty;
			return;
		}

		var dispCurrencyCode = GlobalConfigUtil.GetCurrencyByLocaleId(
			ddlDispCurrencyLocaleId.SelectedValue).Code;
		lDispCurrencyCode.Text = GetEncodedHtmlDisplayMessage(dispCurrencyCode);
		hfDispCurrencyCode.Value = dispCurrencyCode;
	}

	/// <summary>
	/// 表示言語ロケールID ドロップダウンリスト変更時イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDispLanguageLocaleId_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(ddlDispLanguageLocaleId.SelectedValue))
		{
			lDispLanguageCode.Text = hfDispLanguageCode.Value = string.Empty;
			return;
		}

		var dispLanguageCode = GlobalConfigUtil.GetLanguageByLocaleId(ddlDispLanguageLocaleId.SelectedValue).Code;
		lDispLanguageCode.Text = GetEncodedHtmlDisplayMessage(dispLanguageCode);
		hfDispLanguageCode.Value = dispLanguageCode;
	}

	/// <summary>
	/// 注文者の住所国と配送先国から、台湾後払いが利用可能かを判定しエラーメッセージを返す
	/// </summary>
	/// <param name="paymentId">判定する決済種別ID</param>
	/// <param name="shippingCountryIsoCode">配送先国コード</param>
	/// <param name="ownerCountryIsoCode">注文者の住所国コード</param>
	/// <returns>可能か</returns>
	private string CheckUsedPaymentOrderRegistForTriLinkAfterPay(
		string paymentId,
		string shippingCountryIsoCode,
		string ownerCountryIsoCode)
	{
		var errorMessage = TriLinkAfterPayHelper.CheckUsedPaymentForTriLinkAfterPay(
				paymentId,
				shippingCountryIsoCode,
				ownerCountryIsoCode)
			? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYMENT_USBABLE_COUNTRY_ERROR)
				.Replace("@@ 1 @@", "Taiwan")
			: string.Empty;
		return errorMessage;
	}

	/// <summary>
	/// カート決済情報取得
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <returns>更新後のカート決済情報</returns>
	private CartPayment GetCartPayment(CartObject cart)
	{
		var orderCreditCardInput = GetOrderCreditCardInputForOrderPage(
			ddlOrderPaymentKbn.SelectedValue,
			hfUserId.Value);
		var cartPayment = CreateCartPayment(cart, orderCreditCardInput);
		cartPayment.PriceExchange = OrderCommon.GetPaymentPrice(
			cart.ShopId,
			cartPayment.PaymentId,
			cart.PriceSubtotal,
			cart.PriceCartTotalWithoutPaymentPrice);

		return cartPayment;
	}

	/// <summary>
	/// Bind Data Order Item
	/// </summary>
	/// <param name="orderItems">Order items</param>
	private void BindDataOrderItem(List<Hashtable> orderItems)
	{
		orderItems.ForEach(item => item[CONST_KEY_CAN_DELETE] = (orderItems.Count > 1));
		if (orderItems.Any() == false)
		{
			AddDefaultRowOrderItem();
			return;
		}

		rItemList.DataSource = orderItems;
		rItemList.DataBind();
	}

	/// <summary>
	/// Add default row order item
	/// </summary>
	private void AddDefaultRowOrderItem()
	{
		var orderItems = new List<Hashtable>();
		CreateOrderItemByRow(3, orderItems);
		tbAddRow.Text = "3";
	}

	/// <summary>
	/// Check Can Delete
	/// </summary>
	/// <param name="item">Repeater Item</param>
	/// <returns>Can delete or not</returns>
	protected bool CheckCanDelete(RepeaterItem item)
	{
		return (bool)(((Hashtable)item.DataItem)[CONST_KEY_CAN_DELETE]);
	}

	/// <summary>
	/// Select Change Uniform Invoice Type
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUniformInvoiceType_SelectedIndexChanged(object sender, EventArgs e)
	{
		dvElectronicBillErrorMessages.Visible = false;
		ddlUniformInvoiceOrCarryTypeOption.Visible = true;
		switch (ddlUniformInvoiceType.SelectedValue)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
				dvInvoiceCarryType.Visible = true;
				dvUniformInvoiceOption1_1.Visible = false;
				dvUniformInvoiceOption1_2.Visible = false;
				dvUniformInvoiceOption2.Visible = false;
				if ((ddlInvoiceCarryType.SelectedValue == Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE)
					|| (ddlInvoiceCarryType.SelectedValue == Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE))
				{
					dvUniformInvoiceTypeRegistInput.Visible = true;
				}
				else
				{
					ddlUniformInvoiceOrCarryTypeOption.Visible = false;
				}
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
				dvInvoiceCarryType.Visible = false;
				dvUniformInvoiceOption1_1.Visible = true;
				dvUniformInvoiceOption1_2.Visible = true;
				dvUniformInvoiceOption2.Visible = false;
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				dvInvoiceCarryType.Visible = false;
				dvUniformInvoiceOption1_1.Visible = false;
				dvUniformInvoiceOption1_2.Visible = false;
				dvUniformInvoiceOption2.Visible = true;
				break;
		}

		ddlUniformInvoiceOrCarryTypeOption.DataSource = GetUniformInvoiceOrCarryTypeOption(
			ddlUniformInvoiceType.SelectedValue,
			ddlInvoiceCarryType.SelectedValue);
		ddlUniformInvoiceOrCarryTypeOption.DataBind();

		SetEmptyValueForInvoice();
		SetEnableTextBoxUniformInvoice();
	}

	/// <summary>
	/// Select Change Invoice Carry Type
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlInvoiceCarryType_SelectedIndexChanged(object sender, EventArgs e)
	{
		dvElectronicBillErrorMessages.Visible = false;
		switch (ddlInvoiceCarryType.SelectedValue)
		{
			case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
				tbCarryTypeOption1.Visible = true;
				tbCarryTypeOption2.Visible = false;
				ddlUniformInvoiceOrCarryTypeOption.Visible = true;
				spInvoiceCarryType.Visible = true;
				break;

			case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
				tbCarryTypeOption1.Visible = false;
				tbCarryTypeOption2.Visible = true;
				ddlUniformInvoiceOrCarryTypeOption.Visible = true;
				spInvoiceCarryType.Visible = true;
				break;

			default:
				tbCarryTypeOption1.Visible = false;
				tbCarryTypeOption2.Visible = false;
				ddlUniformInvoiceOrCarryTypeOption.Visible = false;
				spInvoiceCarryType.Visible = false;
				break;
		}

		ddlUniformInvoiceOrCarryTypeOption.DataSource = GetUniformInvoiceOrCarryTypeOption(
			ddlUniformInvoiceType.SelectedValue,
			ddlInvoiceCarryType.SelectedValue);
		ddlUniformInvoiceOrCarryTypeOption.DataBind();

		SetEmptyValueForInvoice();
		SetEnableTextBoxUniformInvoice();
	}

	/// <summary>
	/// Select Change Uniform Invoice Type Or Carry Type Option
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlUniformInvoiceOrCarryTypeOption_SelectedIndexChanged(object sender, EventArgs e)
	{
		var isNewOption = string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue);
		var userInvoice = ((isNewOption == false)
			? DomainFacade.Instance.TwUserInvoiceService.Get(
				hfUserId.Value,
				int.Parse(ddlUniformInvoiceOrCarryTypeOption.SelectedValue))
			: null);
		SetEmptyValueForInvoice();
		SetEnableTextBoxUniformInvoice();

		switch (ddlUniformInvoiceType.SelectedValue)
		{
			case Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL:
				dvInvoiceCarryType.Visible = true;
				dvUniformInvoiceOption1_1.Visible = false;
				dvUniformInvoiceOption1_2.Visible = false;
				dvUniformInvoiceOption2.Visible = false;
				if (ddlInvoiceCarryType.SelectedValue == Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE)
				{
					tbCarryTypeOption1.Text = ((userInvoice == null)
						? string.Empty
						: userInvoice.TwCarryTypeOption);
				}
				else if (ddlInvoiceCarryType.SelectedValue == Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE)
				{
					tbCarryTypeOption2.Text = ((isNewOption || (userInvoice == null))
						? string.Empty
						: userInvoice.TwCarryTypeOption);
				}
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_COMPANY:
				dvInvoiceCarryType.Visible = false;
				dvUniformInvoiceOption1_1.Visible = true;
				dvUniformInvoiceOption1_2.Visible = true;
				dvUniformInvoiceOption2.Visible = false;
				tbUniformInvoiceOption1_1.Text = lUniformInvoiceOption1_1.Text = ((userInvoice == null)
						? string.Empty
						: userInvoice.TwUniformInvoiceOption1);
				tbUniformInvoiceOption1_2.Text = lUniformInvoiceOption1_2.Text = ((userInvoice == null)
						? string.Empty
						: userInvoice.TwUniformInvoiceOption2);

				tbUniformInvoiceOption1_1.Visible = tbUniformInvoiceOption1_2.Visible = (userInvoice == null);
				lUniformInvoiceOption1_1.Visible = lUniformInvoiceOption1_2.Visible = (userInvoice != null);
				break;

			case Constants.FLG_TW_UNIFORM_INVOICE_DONATE:
				dvInvoiceCarryType.Visible = false;
				dvUniformInvoiceOption1_1.Visible = false;
				dvUniformInvoiceOption1_2.Visible = false;
				dvUniformInvoiceOption2.Visible = true;
				tbUniformInvoiceOption2.Text = lUniformInvoiceOption2.Text = ((userInvoice == null)
						? string.Empty
						: userInvoice.TwUniformInvoiceOption1);
				tbUniformInvoiceOption2.Visible = (userInvoice == null);
				lUniformInvoiceOption2.Visible = (userInvoice != null);
				break;
		}
	}

	/// <summary>
	/// Set Empty Value For Invoice
	/// </summary>
	protected void SetEmptyValueForInvoice()
	{
		tbCarryTypeOption1.Text = string.Empty;
		tbCarryTypeOption2.Text = string.Empty;
		tbUniformInvoiceOption1_1.Text = string.Empty;
		tbUniformInvoiceOption1_2.Text = string.Empty;
		tbUniformInvoiceOption2.Text = string.Empty;
		tbUniformInvoiceTypeName.Text = string.Empty;
		tbCarryTypeOptionName.Text = string.Empty;
		lUniformInvoiceOption1_1.Text = string.Empty;
		lUniformInvoiceOption1_2.Text = string.Empty;
		lUniformInvoiceOption2.Text = string.Empty;
		cbSaveToUserInvoice.Checked = false;
		cbCarryTypeOptionRegist.Checked = false;
		dvUniformInvoiceTypeRegistInput.Visible = false;
		dvCarryTypeOptionName.Visible = false;
	}

	/// <summary>
	/// Set Enable Text Box Uniform Invoice
	/// </summary>
	protected void SetEnableTextBoxUniformInvoice()
	{
		if (string.IsNullOrEmpty(ddlUniformInvoiceOrCarryTypeOption.SelectedValue))
		{
			tbCarryTypeOption1.Enabled = true;
			tbCarryTypeOption2.Enabled = true;
			tbUniformInvoiceOption1_1.Visible = true;
			tbUniformInvoiceOption1_2.Visible = true;
			tbUniformInvoiceOption2.Visible = true;
			lUniformInvoiceOption1_1.Visible = false;
			lUniformInvoiceOption1_2.Visible = false;
			lUniformInvoiceOption2.Visible = false;
			if (ddlUniformInvoiceType.SelectedValue == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
			{
				dvUniformInvoiceTypeRegist.Visible = false;
				dvCarryTypeOptionRegist.Visible = (ddlInvoiceCarryType.SelectedValue != Constants.FLG_ORDER_TW_CARRY_TYPE_NONE);
			}
			else
			{
				dvUniformInvoiceTypeRegist.Visible = true;
				dvCarryTypeOptionRegist.Visible = false;
			}
		}
		else
		{
			tbCarryTypeOption1.Enabled = false;
			tbCarryTypeOption2.Enabled = false;
			tbUniformInvoiceOption1_1.Visible = false;
			tbUniformInvoiceOption1_2.Visible = false;
			tbUniformInvoiceOption2.Visible = false;
			lUniformInvoiceOption1_1.Visible = true;
			lUniformInvoiceOption1_2.Visible = true;
			lUniformInvoiceOption2.Visible = true;
			dvUniformInvoiceTypeRegist.Visible = false;
			dvCarryTypeOptionRegist.Visible = false;
		}

		if ((ddlUniformInvoiceType.SelectedValue == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
			&& (ddlInvoiceCarryType.SelectedValue == Constants.FLG_ORDER_TW_CARRY_TYPE_NONE))
		{
			ddlUniformInvoiceOrCarryTypeOption.Visible = false;
		}
		else
		{
			ddlUniformInvoiceOrCarryTypeOption.Visible = true;
		}

		spInvoiceCarryType.Visible = ((tbCarryTypeOption1.Enabled && tbCarryTypeOption1.Visible)
			|| (tbCarryTypeOption2.Enabled && tbCarryTypeOption2.Visible));
		spUniformInvoiceOption1_1.Visible = tbUniformInvoiceOption1_1.Visible;
		spUniformInvoiceOption1_2.Visible = tbUniformInvoiceOption1_2.Visible;
		spUniformInvoiceOption2.Visible = tbUniformInvoiceOption2.Visible;
	}

	/// <summary>
	/// Get Uniform Invoice Or Carry Type Option
	/// </summary>
	/// <param name="invoiceCarryType">Uniform Invoice Type</param>
	/// <param name="invoiceCarryType">Invoice Carry Type</param>
	/// <returns>List Item</returns>
	protected ListItemCollection GetUniformInvoiceOrCarryTypeOption(string uniformInvoiceType, string invoiceCarryType)
	{
		var listItem = new ListItemCollection
		{
			new ListItem(ReplaceTag("@@DispText.invoice_carry_type_option.new@@"), string.Empty),
		};

		var userInvoice = ((string.IsNullOrEmpty(hfUserId.Value) == false)
			? DomainFacade.Instance.TwUserInvoiceService.GetAllUserInvoiceByUserId(hfUserId.Value)
			: null);
		if (userInvoice != null)
		{
			if (uniformInvoiceType == Constants.FLG_TW_UNIFORM_INVOICE_PERSONAL)
			{
				if (uniformInvoiceType != Constants.FLG_ORDER_TW_CARRY_TYPE_NONE)
					listItem.AddRange(userInvoice
						.Where(item => (item.TwUniformInvoice == uniformInvoiceType) && (item.TwCarryType == invoiceCarryType))
						.Select(item => new ListItem(item.TwInvoiceName, item.TwInvoiceNo.ToString())).ToArray());
			}
			else
			{
				listItem.AddRange(userInvoice
					.Where(item => (item.TwUniformInvoice == uniformInvoiceType))
					.Select(item => new ListItem(item.TwInvoiceName, item.TwInvoiceNo.ToString())).ToArray());
			}
		}

		return listItem;
	}

	/// <summary>
	/// 領収書希望がクリックされた場合
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblReceiptFlg_SelectedIndexChanged(object sender, EventArgs e)
	{
		tbReceiptAddress.Enabled
			= tbReceiptProviso.Enabled
			= (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_ON);
		if (rblReceiptFlg.SelectedValue == Constants.FLG_ORDER_RECEIPT_FLG_OFF)
		{
			tbReceiptAddress.Text = tbReceiptProviso.Text = string.Empty;
		}
	}

	/// <summary>
	/// Select Change Save To User Invoice
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbSaveToUserInvoice_CheckedChanged(object sender, EventArgs e)
	{
		dvUniformInvoiceTypeRegistInput.Visible = cbSaveToUserInvoice.Checked;
		ddlUniformInvoiceOrCarryTypeOption.Visible = true;
	}

	/// <summary>
	/// Select Carry Type Option Regist
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbCarryTypeOptionRegist_CheckedChanged(object sender, EventArgs e)
	{
		dvCarryTypeOptionName.Visible = cbCarryTypeOptionRegist.Checked;
	}

	/// <summary>
	/// Get LINE pay reg key
	/// </summary>
	/// <param name="userId">The user ID</param>
	/// <returns>A LINE pay reg key</returns>
	private string GetLinePayRegKey(string userId)
	{
		var userExtend = DomainFacade.Instance.UserService.GetUserExtend(userId);
		var result = ((userExtend != null)
			? StringUtility.ToEmpty(userExtend.UserExtendDataValue[Constants.LINEPAY_USEREXRTEND_COLUMNNAME_REGKEY])
			: string.Empty);
		return result;
	}

	/// <summary>
	/// 配送方法自動判定ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSetShippingMethod_Click(object sender, EventArgs e)
	{
		foreach (RepeaterItem riItem in rItemList.Items)
		{
			if (string.IsNullOrEmpty(((TextBox)riItem.FindControl("tbItemQuantity")).Text)
				&& (string.IsNullOrEmpty(((TextBox)riItem.FindControl("tbProductId")).Text) == false)) return;
		}
		var orderItems = CreateOrderItemInput();
		if (DisplayItemsErrorMessages(CheckItemsAndPricesInput(orderItems))) return;

		var hasError = false;
		var cart = CreateCart(orderItems, ref hasError);

		// 商品情報欄の表示更新を行う
		RefreshItemsAndPriceView();

		// 注文情報作成
		string userId = null;
		var advCodeNew = tbAdvCode.Text;
		var advCodeFirst = advCodeNew;
		if (string.IsNullOrEmpty(hfUserId.Value) == false)
		{
			userId = hfUserId.Value;
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

			// 配送希望日時が指定されていない場合に処理実行
			if (OrderCommon.IsDecideDeliveryMethod(ddlOrderPaymentKbn.SelectedValue)
				&& string.IsNullOrEmpty(ddlShippingDate.SelectedValue)
				&& string.IsNullOrEmpty(ddlShippingTime.SelectedValue))
			{
				ddlShippingMethod.SelectedValue = OrderCommon.GetShippingMethod(bundledCart.Items);
				var shopShipping = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, cart.ShippingType);
				if (shopShipping == null) return;
				var companyList = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
					? shopShipping.CompanyListExpress
					: shopShipping.CompanyListMail;
				ddlDeliveryCompany.Items.Clear();
				foreach (var item in companyList)
				{
					var company = this.DeliveryCompanyList
						.First(itemCompany => (itemCompany.DeliveryCompanyId == item.DeliveryCompanyId));
					ddlDeliveryCompany.Items.Add(new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId));
				}
				ddlDeliveryCompany.SelectedValue = companyList.First(item => item.IsDefault).DeliveryCompanyId;

				// メール便配送サービスエスカレーション処理
				DeliveryCompanyMailEscalation(shopShipping, bundledCart);

				ddlShippingMethod_SelectedIndexChanged(sender, e);
			}
		}
	}

	/// <summary>
	/// Is Payment At Convenience Store
	/// </summary>
	/// <param name="shippingReceivingStoreType">Shipping Receiving Store Type</param>
	/// <returns>True: Payment at convenience store, otherwise: false</returns>
	protected bool IsPaymentAtConvenienceStore(string shippingReceivingStoreType)
	{
		var result = ((shippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART_PAYMENT)
			|| (shippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_7_ELEVENT_PAYMENT)
			|| (shippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE_PAYMENT));
		return result;
	}

	/// <summary>
	/// Create order item by row
	/// </summary>
	/// <param name="rowAddNumbering">Row add numbering</param>
	/// <param name="orderItems">Order Items</param>
	private void CreateOrderItemByRow(int rowAddNumbering, List<Hashtable> orderItems)
	{
		var orderItem = new Hashtable
		{
			{ Constants.FIELD_ORDERITEM_ITEM_PRICE, "0" },
			{ Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, string.Empty }
		};
		for (var index = 0; index < rowAddNumbering; index++)
		{
			orderItems.Add(orderItem);
		}
		// 頒布会数量系のチェック
		CheckSubscriptionBoxQuantity(orderItems);
		BindDataOrderItem(orderItems);

		this.CurrentOrderItemCount = orderItems.Count;
	}

	/// <summary>
	/// Create product join name
	/// </summary>
	/// <param name="productInfo">Product information</param>
	/// <returns>Product join name</returns>
	protected string CreateProductJoinName(object productInfo)
	{
		if (productInfo == null) return string.Empty;

		var originalName = StringUtility.ToEmpty(GetKeyValue(productInfo, Constants.FIELD_ORDERITEM_PRODUCT_NAME));
		var setPromotionNo = StringUtility.ToEmpty(GetKeyValue(productInfo, Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO));
		var setPromotionName = StringUtility.ToEmpty(GetKeyValue(productInfo, Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME));
		if (string.IsNullOrEmpty(setPromotionNo)) return originalName;

		var joinName = string.Format(
			"{0} [{1}：{2}]",
			originalName,
			setPromotionNo,
			setPromotionName);
		return joinName;
	}

	/// <summary>
	/// Set First And Next Shipping Date
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <param name="shopShipping">Shop Shipping</param>
	private void SetFirstAndNextShippingDate(CartObject cart, ShopShippingModel shopShipping)
	{
		if ((cart.HasFixedPurchase == false)
			&& (this.IsUpdateFixedPurchaseShippingPattern == false)) return;

		// Get fixed purchase settings
		var fixedPurchaseSettings = GetFixedPurchaseSettingInputs();

		// 入力チェック
		var shippingFixedPurchaseErrorMessage = Validator.Validate("OrderShippingRegistInput", fixedPurchaseSettings);
		if (string.IsNullOrEmpty(shippingFixedPurchaseErrorMessage) == false) return;

		// Update to Cart Shipping
		var fixedPurchaseKbn =
			StringUtility.ToEmpty(fixedPurchaseSettings[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN]);
		var fixedPurchaseSetting =
			StringUtility.ToEmpty(fixedPurchaseSettings[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1]);
		var daysRequired =
			int.Parse(StringUtility.ToEmpty(
				fixedPurchaseSettings[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED]));
		var minSpan =
			int.Parse(StringUtility.ToEmpty(
				fixedPurchaseSettings[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_MINIMUM_SHIPPING_SPAN]));
		var shippingDate = (ddlShippingMethod.SelectedValue != Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
			? string.Empty
			: ddlShippingDate.SelectedValue;

		var cartShipping = cart.GetShipping();
		cartShipping.ShippingDate = (string.IsNullOrEmpty(shippingDate) == false)
			? DateTime.Parse(shippingDate)
			: (DateTime?)null;
		cartShipping.UpdateFixedPurchaseSetting(
			fixedPurchaseKbn,
			fixedPurchaseSetting,
			daysRequired,
			minSpan);

		var calculationMode = rbPurchase_EveryNWeek.Checked
			? NextShippingCalculationMode.EveryNWeek
			: Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE;
		this.ShopShipping = shopShipping;

		if ((this.IsBackFromConfirm == false)
			&& (this.IsUpdatePayment == false))
		{
			// Calculate first shipping date
			SetFirstShippingDate(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				minSpan,
				calculationMode,
				shopShipping,
				cartShipping);

			// Calculate next shipping date base on first shipping date
			SetNextShippingDate(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				cartShipping.FirstShippingDate,
				daysRequired,
				minSpan,
				calculationMode,
				cartShipping);
		}

		this.IsUpdateFixedPurchaseShippingPattern = false;
	}

	/// <summary>
	/// Jugdement Novelty Variation
	/// </summary>
	/// <param name="grantItem">Novelty Grant Item</param>
	/// <returns>True if has variation</returns>
	protected bool JugdementNoveltyVariation(CartNoveltyGrantItem grantItem)
	{
		return (grantItem.VariationId != grantItem.ProductId);
	}

	/// <summary>
	/// Button Clear Adv Code
	/// </summary>
	/// <param name="sender"></param
	/// <param name="e"></param>
	protected void lbClearAdvCode_Click(object sender, EventArgs e)
	{
		dvAdvCodeErrorMessage.Visible = false;
		lAdvCodeErrorMessages.Text = string.Empty;
		tbAdvCode.Text = string.Empty;
		lbAdvName.Text = string.Empty;
		hfAdvName.Value = string.Empty;
		AddAttributesForControlDisplay(
			lbClearAdvCode,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_NONE,
			true);
		ChangeAdvCodeEvent();
	}

	/// <summary>
	/// Get Order Item Field Html
	/// </summary>
	/// <param name="model">Model</param>
	/// <returns>Items name</returns>
	protected string GetOrderItemFieldHtml(object model)
	{
		var tagItemHtml = new StringBuilder();
		var formatItemHtml = "<li>{0}</li>";
		if (model is OrderModel)
		{
			var orderModel = (OrderModel)model;
			foreach (var item in orderModel.Items)
			{
				tagItemHtml.AppendLine(string.Format(formatItemHtml, item.ProductName));
			}
			return tagItemHtml.ToString();
		}
		else if (model is UserFixedPurchaseListSearchResult)
		{
			var fixedPurchaseModel = (UserFixedPurchaseListSearchResult)model;
			foreach (var item in fixedPurchaseModel.Shippings[0].Items)
			{
				tagItemHtml.AppendLine(string.Format(formatItemHtml, item.Name));
			}
			return tagItemHtml.ToString();
		}
		return string.Empty;
	}

	/// <summary>
	/// Get item price total
	/// </summary>
	/// <param name="model">Model</param>
	/// <returns>Item price total</returns>
	protected decimal GetItemPriceTotal(object model)
	{
		var resutl = 0m;
		if (model is OrderModel)
		{
			resutl = ((OrderModel)model).Shippings
				.First()
				.Items
				.Sum(product => (product.ItemPrice * product.ItemQuantity));
			return resutl;
		}
		else if (model is UserFixedPurchaseListSearchResult)
		{
			resutl = ((UserFixedPurchaseListSearchResult)model).Shippings
				.First()
				.Items
				.Sum(product => product.GetItemPrice());
			return resutl;
		}
		return resutl;
	}

	/// <summary>
	/// Click The Reorder Button In The Order History List
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rOrderHistoryList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		var orderId = StringUtility.ToEmpty(e.CommandArgument);
		if (string.IsNullOrEmpty(orderId)
			|| (e.CommandName != "ReOrder")) return;

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_REGIST_INPUT)
			.AddParam(Constants.REQUEST_KEY_REORDER_ID, orderId)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// Click The Reorder Button In The Fixed Purchase History List
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rFixedPurchaseHistoryList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		var fixedPurchaseId = StringUtility.ToEmpty(e.CommandArgument);
		if (string.IsNullOrEmpty(fixedPurchaseId)
			|| (e.CommandName != "ReOrderFixedPurchase")) return;

		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_REGIST_INPUT)
			.AddParam(Constants.REQUEST_KEY_REORDER_FIXEDPURCHASE_ID, fixedPurchaseId)
			.CreateUrl();
		Response.Redirect(url.ToString());
	}

	/// <summary>
	/// Can Re-order
	/// </summary>
	/// <param name="model">Model</param>
	/// <returns>True: Can Re-order</returns>
	public bool CanReOrder(object model)
	{
		if (model is OrderModel)
		{
			// 下記の注文に対しては、再注文できない。
			//  ・セット商品を購入した注文
			//  ・返品／交換注文
			//  ・定期購入のみの商品を購入した注文
			var order = (OrderModel)model;
			if ((order.ReturnExchangeKbn != Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN)
				|| order.Items.Any(oi => (string.IsNullOrEmpty(oi.ProductSetId) == false))
				|| order.Items.Any(oi => (oi.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY)))
			{
				return false;
			};
			return true;
		}
		else if (model is UserFixedPurchaseListSearchResult)
		{
			// The following fixed purchase orders cannot be reordered.
			// ・ Fixed purchase order is not a status: cancel or cancel temporarily
			// ・ Fixed purchase order has invalid fixed purchase product
			var fixedPurchase = (UserFixedPurchaseListSearchResult)model;
			if (((fixedPurchase.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL)
					&& (fixedPurchase.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL_TEMP))
				|| fixedPurchase.Shippings[0].Items.Any(fpi => (fpi.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)))
			{
				return false;
			}
			return true;
		}
		return false;
	}

	/// <summary>
	/// Check the order combine checkbox
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbOrderCombineSelect_CheckedChangedExchange(object sender, EventArgs e)
	{
		this.UseOrderCombine = true;
		BindOrderCombine();
	}

	/// <summary>
	/// Set re-order data fixed purchase
	/// </summary>
	private void SetReOrderDataFixedPurchase()
	{
		// Get fixed purchase
		var fixedPurchase = DomainFacade.Instance.FixedPurchaseService.GetContainer(this.ReFixedPurchaseId);
		if (fixedPurchase == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// The following fixed purchase orders cannot be reordered.
		// ・ Fixed purchase order is not a status: cancel or cancel temporarily
		// ・ Fixed purchase order has invalid fixed purchase product
		if (((fixedPurchase.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL)
				&& (fixedPurchase.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_CANCEL_TEMP))
			|| fixedPurchase.Shippings[0].Items.Any(fpi => (fpi.FixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_UNABLE_REORDER_FIXEDPURCHASE);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// Set owner
		SetOwner(fixedPurchase.UserId);

		// Set re-order shipping
		SetReOrderShipping(null, fixedPurchase.Shippings[0]);

		// Set re-order items fixed purchase
		SetReOrderItemsFixedPurchase(fixedPurchase.Shippings[0].Items);

		// Set re-order fixed purchase setting
		var shopShipping = DomainFacade.Instance.ShopShippingService.Get(this.LoginOperatorShopId, fixedPurchase.ShippingType);
		SetReOrderFixedPurchaseSetting(fixedPurchase, shopShipping);

		// Set re-order payment
		SetRePayment(fixedPurchase.OrderPaymentKbn);

		if (Constants.ORDER_EXTEND_OPTION_ENABLED)
		{
			var input = new OrderExtendInput(OrderExtendCommon.CreateOrderExtendForManager(fixedPurchase));
			SetOrderExtendFromUserExtendObject(rOrderExtendInput, input);
		}

		// Setting re-order warning message display or non-display
		dvReOrderWarning.Visible
			= dvShippingInfo.Visible
			= dvPayment.Visible
			= dvBeforeCombine.Visible = true;
		GetAndSetDisplayOrderAndFixedPurchaseHistoryListByUserId(fixedPurchase.UserId);
	}

	/// <summary>
	/// Set re-order fixed purchase setting
	/// </summary>
	/// <param name="fixedPurchase">Fixed Purchase</param>
	/// <param name="shopShipping">Shop Shipping</param>
	private void SetReOrderFixedPurchaseSetting(FixedPurchaseModel fixedPurchase, ShopShippingModel shopShipping)
	{
		switch (fixedPurchase.FixedPurchaseKbn)
		{
			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
				if (shopShipping.IsValidFixedPurchaseKbn1Flg)
				{
					rbMonthlyPurchase_Date.Checked = true;
					var settings = fixedPurchase.FixedPurchaseSetting1.Split(',');
					if (ddlMonth.Items.FindByValue(settings[0]) != null)
					{
						ddlMonth.SelectedValue = settings[0];
					}
					if (ddlMonthlyDate.Items.FindByValue(settings[1]) != null)
					{
						ddlMonthlyDate.SelectedValue = settings[1];
					}
					ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged(null, null);
				}
				break;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
				if (shopShipping.IsValidFixedPurchaseKbn2Flg)
				{
					rbMonthlyPurchase_WeekAndDay.Checked = true;
					var settings = fixedPurchase.FixedPurchaseSetting1.Split(',');
					if (ddlIntervalMonths.Items.FindByValue(settings[0]) != null)
					{
						ddlIntervalMonths.SelectedValue = settings[0];
					}
					if (ddlWeekOfMonth.Items.FindByValue(settings[1]) != null)
					{
						ddlWeekOfMonth.SelectedValue = settings[1];
					}
					if (ddlDayOfWeek.Items.FindByValue(settings[2]) != null)
					{
						ddlDayOfWeek.SelectedValue = settings[2];
					}
					ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged(null, null);
				}
				break;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
				if (shopShipping.IsValidFixedPurchaseKbn3Flg
					&& (ddlIntervalDays.Items.FindByValue(fixedPurchase.FixedPurchaseSetting1) != null))
				{
					rbRegularPurchase_IntervalDays.Checked = true;
					ddlIntervalDays.SelectedValue = fixedPurchase.FixedPurchaseSetting1;
					ddlIntervalDays_OnSelectedIndexChanged(null, null);
				}
				break;

			case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
				if (shopShipping.IsValidFixedPurchaseKbn4Flg)
				{
					rbPurchase_EveryNWeek.Checked = true;
					var settings = fixedPurchase.FixedPurchaseSetting1.Split(',');
					if (ddlFixedPurchaseEveryNWeek_Week.Items.FindByValue(settings[0]) != null)
					{
						ddlFixedPurchaseEveryNWeek_Week.SelectedValue = settings[0];
					}
					if (ddlFixedPurchaseEveryNWeek_DayOfWeek.Items.FindByValue(settings[1]) != null)
					{
						ddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue = settings[1];
					}
					ddlFixedPurchaseEveryNWeek_OnSelectedIndexChanged(null, null);
				}
				break;
		}
	}

	/// <summary>
	/// Set ReOrder Items Fixed Purchase
	/// </summary>
	/// <param name="fixedPurchaseItems">Fixed Purchase Items</param>
	private void SetReOrderItemsFixedPurchase(FixedPurchaseItemContainer[] fixedPurchaseItems)
	{
		var fixedPurchaseItemList = new List<Hashtable>();
		foreach (var fixedPurchaseItem in fixedPurchaseItems)
		{
			// Product variation information
			var variationInfo = GetProductVariation(
				fixedPurchaseItem.ShopId,
				fixedPurchaseItem.ProductId,
				fixedPurchaseItem.VariationId,
				hfMemberRankId.Value,
				string.Empty);

			if (variationInfo.Count != 0)
			{
				var orderItem = new Hashtable
				{
					{ Constants.FIELD_ORDERITEM_SHOP_ID, fixedPurchaseItem.ShopId },
					{ Constants.FIELD_ORDERITEM_PRODUCT_ID, fixedPurchaseItem.ProductId },
					{ Constants.FIELD_ORDERITEM_VARIATION_ID, fixedPurchaseItem.VariationId },
					{ Constants.FIELD_ORDERITEM_SUPPLIER_ID, fixedPurchaseItem.SupplierId },
					{ Constants.FIELD_PRODUCTVARIATION_V_ID,
						fixedPurchaseItem.VariationId.Replace(fixedPurchaseItem.ProductId, string.Empty) },
					{ Constants.FIELD_ORDERITEM_PRODUCT_NAME, string.Format("{0}{1}",
						variationInfo[0][Constants.FIELD_PRODUCT_NAME],
						CreateVariationName(variationInfo[0])) },
					{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, fixedPurchaseItem.ItemQuantity.ToString() },
					{ Constants.FIELD_ORDERITEM_PRODUCT_PRICE,
						GetFixedPurchaseProductValidityPrice(variationInfo[0], true).ToPriceString() },
					{ Constants.FIELD_ORDERITEM_PRODUCTSALE_ID,
						StringUtility.ToEmpty(variationInfo[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]) },
					{ CONST_KEY_PRODUCT_OPTION_VALUE_SETTINGS,
						new ProductOptionSettingList(this.LoginOperatorShopId, fixedPurchaseItem.ProductId) },
					{ CONST_KEY_FIXED_PURCHASE, true },
					{ Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, string.Empty },
					{ Constants.FIELD_ORDERSETPROMOTION_SETPROMOTION_NAME, string.Empty },
					{ Constants.FIELD_ORDERITEM_NOVELTY_ID, string.Empty },
					{ Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE, variationInfo[0][Constants.FIELD_PRODUCT_TAX_RATE] },
					{ Constants.FIELD_PRODUCT_IMAGE_HEAD,
						string.IsNullOrEmpty(StringUtility.ToEmpty(variationInfo[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD]))
							? variationInfo[0][Constants.FIELD_PRODUCT_IMAGE_HEAD]
							: variationInfo[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD] }
				};
				fixedPurchaseItemList.Add(orderItem);
			}
		}
		dvOrderItemErrorMessage.Visible = false;
		RefreshItemsAndPriceView(fixedPurchaseItemList);
	}

	/// <summary>
	/// Get and set display order and fixed purchase history list by user id
	/// </summary>
	/// <param name="userId">User Id</param>
	private void GetAndSetDisplayOrderAndFixedPurchaseHistoryListByUserId(string userId)
	{
		var orderHistoryList = DomainFacade.Instance.OrderService
			.GetOrderHistoryList(userId)
			.Take(10);
		rOrderHistoryList.DataSource = orderHistoryList;
		rOrderHistoryList.DataBind();

		var searchCondition = new UserFixedPurchaseListSearchCondition
		{
			UserId = userId
		};
		var fixedPurchaseHistoryList = DomainFacade.Instance.FixedPurchaseService
			.SearchUserFixedPurchase(searchCondition)
			.Take(10);
		rFixedPurchaseHistoryList.DataSource = fixedPurchaseHistoryList;
		rFixedPurchaseHistoryList.DataBind();

		var countOrderHistoryList = orderHistoryList.Count();
		dvShowOrderHistory.Visible = (countOrderHistoryList > 0);
		dvHideOrderHistory.Visible = (countOrderHistoryList == 0);
		if (orderHistoryList.Count() <= CONST_MAX_DISPLAY_SHOW_FOR_HISTORY)
		{
			dvOrderHistoryList.Attributes.Remove("class");
		}
		else
		{
			AddAttributesForControlDisplay(
				dvOrderHistoryList,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_SCROLL_VERTICAL,
				true);
		}

		var countFixedPurchaseHistoryList = fixedPurchaseHistoryList.Count();
		dvShowFixedPurchaseHistory.Visible = (countFixedPurchaseHistoryList > 0);
		dvHideFixedPurchaseHistory.Visible = (countFixedPurchaseHistoryList == 0);
		if (fixedPurchaseHistoryList.Count() <= CONST_MAX_DISPLAY_SHOW_FOR_HISTORY)
		{
			dvFixedPurchaseHistoryList.Attributes.Remove("class");
		}
		else
		{
			AddAttributesForControlDisplay(
				dvFixedPurchaseHistoryList,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_SCROLL_VERTICAL,
				true);
		}
		lOrderAndFixPurchaseHistoryCount.Text = (countOrderHistoryList + countFixedPurchaseHistoryList).ToString();

		var attributeModel = DomainFacade.Instance.UserService.GetUserAttribute(userId);
		if (attributeModel != null)
		{
			lFirstOrderDate.Text = GetEncodedHtmlDisplayMessage(
				DateTimeUtility.ToStringForManager(
					attributeModel.FirstOrderDate,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
					"-"));
			lSecondOrderDate.Text = GetEncodedHtmlDisplayMessage(
				DateTimeUtility.ToStringForManager(
					attributeModel.SecondOrderDate,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
					"-"));
			lLastOrderDate.Text = GetEncodedHtmlDisplayMessage(
				DateTimeUtility.ToStringForManager(
					attributeModel.LastOrderDate,
					DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter,
					"-"));
			lEnrollmentDays.Text = GetEncodedHtmlDisplayMessage(
				attributeModel.EnrollmentDays.HasValue
					? StringUtility.ToNumeric(attributeModel.EnrollmentDays)
					: "-");
			lAwayDays.Text =
				GetEncodedHtmlDisplayMessage(attributeModel.AwayDays.HasValue
					? StringUtility.ToNumeric(attributeModel.AwayDays)
					: "-");
			lOrderAmountOrderAll.Text = GetEncodedHtmlDisplayMessage(
				StringUtility.ToNumeric(attributeModel.OrderAmountOrderAll.ToPriceString()));
			lOrderAmountOrderFp.Text = GetEncodedHtmlDisplayMessage(
				StringUtility.ToNumeric(attributeModel.OrderAmountOrderFp.ToPriceString()));
			lOrderCountOrderAll.Text = GetEncodedHtmlDisplayMessage(
				StringUtility.ToNumeric(attributeModel.OrderCountOrderAll));
			lOrderCountOrderFp.Text = GetEncodedHtmlDisplayMessage(
				StringUtility.ToNumeric(attributeModel.OrderCountOrderFp));
			lOrderAmountShipAll.Text = GetEncodedHtmlDisplayMessage(
				StringUtility.ToNumeric(attributeModel.OrderAmountShipAll.ToPriceString()));
			lOrderAmountShipFp.Text = GetEncodedHtmlDisplayMessage(
				StringUtility.ToNumeric(attributeModel.OrderAmountShipFp.ToPriceString()));
			lOrderCountShipAll.Text = GetEncodedHtmlDisplayMessage(
				StringUtility.ToNumeric(attributeModel.OrderCountShipAll));
			lOrderCountShipFp.Text = GetEncodedHtmlDisplayMessage(
				StringUtility.ToNumeric(attributeModel.OrderCountShipFp));
		}

		var countStatusCaution = orderHistoryList.Count(order =>
			((order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP)
				|| (order.OrderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM)));
		lCount.Text = countStatusCaution.ToString();
	}

	/// <summary>
	/// Is Selected Product
	/// </summary>
	/// <param name="productId">Product Id</param>
	/// <returns>True: if the product id is selected</returns>
	protected bool IsSelectedProduct(object productId)
	{
		var result = (string.IsNullOrEmpty(StringUtility.ToEmpty(productId)) == false);
		return result;
	}

	/// <summary>
	/// Adjust first and next shipping date
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <param name="shopShipping">Shop Shipping</param>
	protected void AdjustFirstAndNextShippingDate(CartObject cart, ShopShippingModel shopShipping)
	{
		if (cart.HasFixedPurchase == false) return;

		if ((rbMonthlyPurchase_Date.Checked)
			&& ((string.IsNullOrEmpty(ddlMonth.SelectedValue) == false)
				|| (string.IsNullOrEmpty(ddlMonth.SelectedValue) == false)))
		{
			this.IsUpdateFixedPurchaseShippingPattern = true;
			SetFirstAndNextShippingDate(cart, shopShipping);
			return;
		}

		if ((rbMonthlyPurchase_WeekAndDay.Checked)
			&& ((string.IsNullOrEmpty(ddlIntervalMonths.SelectedValue) == false)
				|| (string.IsNullOrEmpty(ddlWeekOfMonth.SelectedValue) == false)
				|| (string.IsNullOrEmpty(ddlDayOfWeek.SelectedValue) == false)))
		{
			this.IsUpdateFixedPurchaseShippingPattern = true;
			SetFirstAndNextShippingDate(cart, shopShipping);
			return;
		}

		if ((rbRegularPurchase_IntervalDays.Checked)
			&& ((string.IsNullOrEmpty(ddlIntervalDays.SelectedValue) == false)))
		{
			this.IsUpdateFixedPurchaseShippingPattern = true;
			SetFirstAndNextShippingDate(cart, shopShipping);
			ddlFirstShippingDate.Visible = false;
			return;
		}

		if ((rbPurchase_EveryNWeek.Checked)
			&& ((string.IsNullOrEmpty(ddlFixedPurchaseEveryNWeek_Week.SelectedValue) == false)
				|| (string.IsNullOrEmpty(ddlFixedPurchaseEveryNWeek_DayOfWeek.SelectedValue) == false)))
		{
			this.IsUpdateFixedPurchaseShippingPattern = true;
			SetFirstAndNextShippingDate(cart, shopShipping);
			ddlFirstShippingDate.Visible = false;
			return;
		}
	}

	/// <summary>
	/// Create product search url
	/// </summary>
	/// <returns>Product search url</returns>
	protected string CreateProductSearchUrl()
	{
		var searchUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN, Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, Constants.FLG_PRODUCT_VALID_FLG_VALID);
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (ddlSubscriptionBox.SelectedValue != string.Empty))
		{
			searchUrl.AddParam(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID, ddlSubscriptionBox.SelectedValue);
		}

		var result = searchUrl.CreateUrl();
		return result;
	}

	/// <summary>
	/// Create adv code search url
	/// </summary>
	/// <returns>Adv code search url</returns>
	protected string CreateAdvCodeSearchUrl()
	{
		var searchUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_REGIST_ADVPOPUP)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();
		return searchUrl;
	}

	/// <summary>
	/// Selected change product variation method
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlVariationId_SelectedIndexChanged(object sender, EventArgs e)
	{
		var repeater = (RepeaterItem)((DropDownList)sender).Parent;
		var ddlVariationIdList = ((DropDownList)repeater.FindControl("ddlVariationIdList"));
		var tbProductId = ((TextBox)repeater.FindControl("tbProductId"));
		var hfProductVariationId = ((HiddenField)repeater.FindControl("hfProductVariationId"));
		var tbProductName = ((TextBox)repeater.FindControl("tbProductName"));
		var tbProductPrice = ((TextBox)repeater.FindControl("tbProductPrice"));
		var tbProductSaleId = ((TextBox)repeater.FindControl("tbProductSaleId"));
		var hfProductImageHead = ((HiddenField)repeater.FindControl("hfProductImageHead"));

		// Get product variation information
		var productVariation = GetProductVariation(
			this.LoginOperatorShopId,
			tbProductId.Text.Trim(),
			ddlVariationIdList.SelectedValue,
			hfMemberRankId.Value,
			tbProductSaleId.Text.Trim());
		var errorMessage = ((productVariation.Count == 0)
			? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PRODUCT_INVALID)
				.Replace("@@ 1 @@", tbProductId.Text)
			: string.Empty);

		if (DisplayItemsErrorMessages(errorMessage) == false)
		{
			// Set product variation selected
			hfProductVariationId.Value = ddlVariationIdList.SelectedValue;
			var isFixedPurchaseValid = (Constants.FIXEDPURCHASE_OPTION_ENABLED && ((CheckBox)repeater.FindControl("cbFixedPurchase")).Checked);
			tbProductName.Text = CreateFixedPurchaseProductName(tbProductName.Text, isFixedPurchaseValid, this.IsSubscriptionBoxValid);
			tbProductPrice.Text = isFixedPurchaseValid
				? GetFixedPurchaseProductValidityPrice(productVariation[0], true).ToPriceString()
				: GetProductValidityPrice(productVariation[0]).ToPriceString();
			tbProductSaleId.Text = string.IsNullOrEmpty(tbProductSaleId.Text.Trim())
				? StringUtility.ToEmpty(productVariation[0][Constants.FIELD_ORDERITEM_PRODUCTSALE_ID])
				: tbProductSaleId.Text;
			hfProductImageHead.Value = string.IsNullOrEmpty(StringUtility.ToEmpty(productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD]))
				? StringUtility.ToEmpty(productVariation[0][Constants.FIELD_PRODUCT_IMAGE_HEAD])
				: StringUtility.ToEmpty(productVariation[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD]);

			// Recalculation process
			btnReCalculate_Click(sender, e);
		}
	}

	/// <summary>
	/// rItemList item data bound event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rItemList_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var productId = ((TextBox)e.Item.FindControl("tbProductId")).Text.Trim();
		if (string.IsNullOrEmpty(productId)) return;

		var ddlVariationIdList = (DropDownList)e.Item.FindControl("ddlVariationIdList");
		var productVariations = DomainFacade.Instance.ProductService.GetProductVariationsByProductId(productId);
		ddlVariationIdList.DataSource = productVariations
			.Select(item => new ListItem(item.VariationId.Replace(item.ProductId, string.Empty), item.VariationId));
		ddlVariationIdList.DataBind();
		var hfProductVariationId = (HiddenField)e.Item.FindControl("hfProductVariationId");
		if (ddlVariationIdList.Items.FindByValue(hfProductVariationId.Value) != null)
		{
			ddlVariationIdList.SelectedValue = hfProductVariationId.Value;
		}
	}

	/// <summary>
	/// Can add new order item row
	/// </summary>
	/// <returns>True: Can add new order item row</returns>
	private bool CanAddNewOrderItemRow()
	{
		var addRowNumbering = tbAddRow.Text.Trim();
		var input = new Hashtable
		{
			{ CONST_FIELD_ADD_ROW_NUMBERING, addRowNumbering }
		};
		var errorMessage = Validator.Validate("OrderRegistInput", input);
		return string.IsNullOrEmpty(errorMessage);
	}

	/// <summary>
	/// Create order item rows
	/// </summary>
	/// <param name="isAddNewRow">Is add new row</param>
	protected void CreateOrderItemRows(bool isAddNewRow = false)
	{
		var orderItems = CreateOrderItemInput();
		var itemCount = orderItems.Count;

		var rowAddInput = 0;
		if (isAddNewRow
			&& int.TryParse(tbAddRow.Text.Trim(), out rowAddInput))
		{
			this.CurrentOrderItemCount = rItemList.Items.Count;
			rowAddInput = (this.CurrentOrderItemCount > 1)
				? (rowAddInput + this.CurrentOrderItemCount - itemCount)
				: rowAddInput;
		}
		else
		{
			rowAddInput = (this.CurrentOrderItemCount - itemCount);
		}

		CreateOrderItemByRow(
			((rowAddInput > 0) || (this.HasItems == false))
				? rowAddInput
				: 1,
			orderItems);
	}

	/// <summary>
	/// Search users for autosuggest
	/// </summary>
	/// <param name="searchWord">Search Word</param>
	/// <returns>A JSON data</returns>
	[WebMethod]
	public static string SearchUsersForAutosuggest(string searchWord)
	{
		var users = Array.Empty<UserModel>();
		var result = new List<AutocompleteObject.UserObject>();
		try
		{
			users = DomainFacade.Instance.UserService.SearchUsersForAutoSuggest(searchWord, Constants.AUTO_SUGGEST_MAX_COUNT_FOR_DISPLAY);
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			result = users
			.Select(user => new AutocompleteObject.UserObject
			{
				Id = user.UserId,
				Name = StringUtility.AbbreviateString(
					(string.IsNullOrEmpty(user.NameKana)
						? user.Name
						: string.Format("{0}（{1}）", user.Name, user.NameKana)),
					CONST_MAX_CONTENT_SHOW_FOR_SEARCH_AUTOCOMPLETE),
				Phone = user.Tel1,
				Address = StringUtility.AbbreviateString(
					user.Addr,
					CONST_MAX_CONTENT_SHOW_FOR_SEARCH_AUTOCOMPLETE),
			}).ToList();
		}
		return CreateReportJsonString(new { data = result });
	}

	/// <summary>
	/// Search adv codes for autosuggest
	/// </summary>
	/// <param name="searchWord">Search Word</param>
	/// <returns>A JSON data</returns>
	[WebMethod]
	public static string SearchAdvCodesForAutosuggest(string searchWord)
	{
		var advCodes = Array.Empty<AdvCodeModel>();
		var result = new List<AutocompleteObject.AdvCodeObject>();
		try
		{
			advCodes = DomainFacade.Instance.AdvCodeService.SearchAdvCodesForAutosuggest(searchWord);
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			result = advCodes
			.Select(advCode => new AutocompleteObject.AdvCodeObject
			{
				Id = advCode.AdvertisementCode,
				Name = advCode.MediaName,
			}).ToList();
		}
		return CreateReportJsonString(new { data = result });
	}

	/// <summary>
	/// Search coupons for autosuggest
	/// </summary>
	/// <param name="couponUserId">Coupon user id</param>
	/// <param name="couponMailAddress">Coupon mail address</param>
	/// <param name="searchWord">Search Word</param>
	/// <returns>A JSON data</returns>
	[WebMethod]
	public static string SearchCouponsForAutosuggest(
		string couponUserId,
		string couponMailAddress,
		string searchWord)
	{
		var coupons = Array.Empty<UserCouponDetailInfo>();
		var result = new List<AutocompleteObject.CouponObject>();
		var condition = new CouponListSearchCondition
		{
			DeptId = Constants.W2MP_DEPT_ID,
			UserId = couponUserId,
			MailAddress = couponMailAddress,
			CouponCode = searchWord,
			CouponName = searchWord,
			UsedUserJudgeType = Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE
		};
		try
		{
			coupons = DomainFacade.Instance.CouponService.SearchCouponsForAutosuggest(condition);
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			result = coupons
			.Select(coupon => new AutocompleteObject.CouponObject
			{
				Id = coupon.CouponCode,
				Name = coupon.CouponName,
				Type = ValueText.GetValueText(
					Constants.TABLE_COUPON,
					Constants.FIELD_COUPON_COUPON_TYPE,
					coupon.CouponType),
				Discount = CouponOptionUtility.GetCouponDiscountString(coupon),
				ExpirationDate = string.Format("{0:yyyy/MM/dd}", coupon.ExpireEnd)
			}).ToList();
		}
		return CreateReportJsonString(new { data = result });
	}

	/// <summary>
	/// Search products for autosuggest
	/// </summary>
	/// <param name="memberRankId">Member rank id</param>
	/// <param name="searchWord">Search Word</param>
	/// <returns>A JSON data</returns>
	[WebMethod]
	public static string SearchProductsForAutosuggest(string memberRankId, string searchWord)
	{
		var products = new ProductModel[0];
		var result = new List<AutocompleteObject.ProductObject>();
		try
		{
			products = DomainFacade.Instance.ProductService.SearchProductsForAutosuggest(memberRankId, searchWord);
		}
		catch (Exception ex)
		{
			throw ex;
		}
		finally
		{
			result = products
				.Select(product => new AutocompleteObject.ProductObject
				{
					ProductId = product.ProductId,
					SupplierId = product.SupplierId,
					VariationId = product.VariationId.Replace(product.ProductId, string.Empty),
					Name = ProductCommon.CreateProductJointName(product),
					DisplayPrice = product.Price.ToPriceString(),
					SpecialPrice = product.SpecialPrice.ToPriceString(),
					UnitPrice = GetProductValidityPrice(product.DataSource).ToPriceString(),
					UnitPriceByKeyCurrency = GetProductValidityPrice(product.DataSource).ToPriceString(true),
					SaleId = ((product.DataSource[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID] != DBNull.Value)
						? product.ProductsaleId
						: string.Empty),
					FixedPurchaseId = product.FixedPurchaseFlg,
					LimitedFixedPurchaseKbn1Setting = product.LimitedFixedPurchaseKbn1Setting,
					LimitedFixedPurchaseKbn3Setting = product.LimitedFixedPurchaseKbn3Setting,
					Quantity = (((product.StockManagementKbn != Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_UNMANAGED)
						&& (product.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK] != DBNull.Value))
							? StringUtility.ToNumeric(product.Stock)
							: string.Empty),
				}).ToList();
		}
		return CreateReportJsonString(new { data = result });

	}

	/// <summary>
	/// 利用クーポン・ポイントをカートにセットしてから決済手数料の再計算
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <remarks>クーポン、ポイント使用処理の中に使う</remarks>
	private void ReCalculatePaymentPriceAfterSetCouponOrPointUse(CartObject cart)
	{
		// 決済手数料の計算に商品小計金額のみを使用する場合、再計算が不要
		if ((cart.Payment == null) || Constants.CALCULATE_PAYMENT_PRICE_ONLY_SUBTOTAL)
		{
			return;
		}

		cart.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
			cart.ShopId,
			cart.Payment.PaymentId,
			cart.PriceSubtotal,
			cart.PriceCartTotalWithoutPaymentPrice);
	}

	/// <summary>
	/// カード情報取得ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnGetCardInfo_Click(object sender, EventArgs e)
	{
		var errorMessage = string.Empty;
		dvPaymentErrorMessages.Visible = (string.IsNullOrEmpty(errorMessage) == false);

		// 注文者情報入力チェック
		var hasError = false;
		GetAndCheckOwnerInput(ref hasError);
		if (hasError) errorMessage = WebMessages.GetMessages("");

		// カード情報入力チェック
		if (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Rakuten)
		{
			var orderCreditCardInput = GetOrderCreditCardInputForOrderPage(
				ddlOrderPaymentKbn.SelectedValue,
				hfUserId.Value);
			errorMessage += orderCreditCardInput.Validate(true, WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_VERITRANS_PAYTG_CARD_UNREGISTERD));
			if (DisplayPaymentErrorMessage(errorMessage)) return;
		}
		// オンライン会員でない場合にはZeus連携用のユニックIDを作成するため、先にユーザーIDを作成する
		var userId = (string.IsNullOrEmpty(hfUserId.Value) == false)
			? hfUserId.Value
			: hfNewUserId.Value;
		if (string.IsNullOrEmpty(userId))
		{
			userId = UserService.CreateNewUserId(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.NUMBER_KEY_USER_ID,
				Constants.CONST_USER_ID_HEADER,
				Constants.CONST_USER_ID_LENGTH);
			hfNewUserId.Value = userId;
		}

		if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
		{
			hfPayTgSendId.Value = new UserCreditCardCooperationInfoVeritrans(userId).CooperationId1;
			Session[PayTgConstants.PARAM_CUSTOMERID] = hfPayTgSendId.Value;

			// PayTG連携APIのポストデータ作成
			hfPayTgPostData.Value = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
				? string.Empty
				: new PayTgApiForVeriTrans(hfPayTgSendId.Value).CreatePostData();

			var apiUrl = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
				? CreateRegisterCardVeriTransMockUrl(hfPayTgSendId.Value)
				: Constants.PAYMENT_SETTING_PAYTG_REGISTCREDITURL;

			// PayTG連動でカード登録実行
			ScriptManager.RegisterStartupScript(
				this,
				GetType(),
				"execRegistration",
				string.Format("execCardRegistration('{0}');", apiUrl),
				true);
		}
		else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
		{
			// PayTg WebApiで利用するため決済注文IDを採番
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(this.LoginOperatorShopId);
			hfPayTgSendId.Value = paymentOrderId;

			// PayTG連携APIのポストデータ作成
			hfPayTgPostData.Value = new PayTgApi(paymentOrderId).CreatePostData();

			var apiUrl = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
				? CreateRegisterCardMockUrl()
				: Constants.PAYMENT_SETTING_PAYTG_BASEURL + PayTgConstants.PspShortName.RAKUTEN + PayTgConstants.DealingTypes.URL_CHECKCARD;

			// PayTG連動でカード登録実行
			ScriptManager.RegisterStartupScript(
				this,
				GetType(),
				"execRegistration",
				string.Format("execCardRegistration('{0}');", apiUrl),
				true);
		}
	}

	/// <summary>
	/// PayTG連携のレスポンス処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnProcessPayTgResponse_Click(object sender, EventArgs e)
	{
		if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans)
		{
			var payTg = new PayTgApiForVeriTrans(hfPayTgSendId.Value);
			payTg.ParseResponse(hfPayTgResponse.Value);

			this.IsSuccessfulCardRegistration = false;

			if (payTg.Result.IsSuccess)
			{
				var cardExpire = payTg.Result.Response.CardExpire.Split('/');
				var cardNumber = payTg.Result.Response.CardNumber;
				var payTgResponse = new Hashtable
				{
					{ VeriTransConst.PAYTG_CARD_EXPIRE_MONTH, cardExpire[0] },
					{ VeriTransConst.PAYTG_CARD_EXPIRE_YEAR, cardExpire[1] },
					{ VeriTransConst.PAYTG_CARD_NUMBER, cardNumber },
					{ VeriTransConst.PAYTG_RESPONSE_ERROR, string.Empty },
				};
				this.PayTgResponse = payTgResponse;

				ddlCreditExpireMonth.SelectedValue = (string)this.PayTgResponse[VeriTransConst.PAYTG_CARD_EXPIRE_MONTH];
				ddlCreditExpireYear.SelectedValue = (string)this.PayTgResponse[VeriTransConst.PAYTG_CARD_EXPIRE_YEAR];
				tbCreditCardNo1.Text = cardNumber;

				this.IsSuccessfulCardRegistration = true;
			}
			else
			{
				this.IsSuccessfulCardRegistration = false;
				var payTgResponse = new Hashtable { { VeriTransConst.PAYTG_RESPONSE_ERROR, payTg.Result.ErrorMessages } };
				this.PayTgResponse = payTgResponse;
				DisplayPaymentErrorMessage(payTg.Result.ErrorMessages);
			}
		}
		else if (Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
		{
			var payTg = new PayTgApi(hfPayTgSendId.Value);
			payTg.ParseResponse(hfPayTgResponse.Value);

			this.IsSuccessfulCardRegistration = false;

			if (payTg.Result.IsSuccess)
			{
				var cardExpireMonth = payTg.Result.Response.McAcntNo1;
				var cardExpireYear = payTg.Result.Response.Expire;
				var lastFourDigit = payTg.Result.Response.Last4;
				var cardNumber = "XXXXXXXXXXXX" + lastFourDigit;
				var companyCode = payTg.Result.Response.AcqName;
				var token = payTg.Result.Response.Token;
				var vResultCode = payTg.Result.Response.VResultCode;
				var errorMsg = payTg.Result.Response.ErrorMsg;
				var payTgResponse = new Hashtable
				{
					{ PayTgConstants.PAYTG_CARD_EXPIRE_MONTH, cardExpireMonth },
					{ PayTgConstants.PAYTG_CARD_EXPIRE_YEAR, cardExpireYear },
					{ PayTgConstants.PARAM_LAST4, lastFourDigit },
					{ PayTgConstants.PAYTG_CARD_NUMBER, cardNumber },
					{ PayTgConstants.PARAM_ACQNAME, companyCode },
					{ PayTgConstants.PARAM_TOKEN, token },
					{ PayTgConstants.PAYTG_RESPONSE_ERROR, string.Empty },
				};
				Session[PayTgConstants.PARAM_TOKEN] = hfPayTgSendId.Value = payTg.Result.Response.Token;
				Session[PayTgConstants.PARAM_ACQNAME] = companyCode;
				this.PayTgResponse = payTgResponse;

				ddlCreditExpireMonth.SelectedValue = cardExpireMonth;
				ddlCreditExpireYear.SelectedValue = cardExpireYear.Substring(cardExpireYear.Length - 2);
				tbCreditCardNo1.Text = cardNumber;

				dvPaymentErrorMessages.Visible = false;
				this.IsSuccessfulCardRegistration = true;
			}
			else
			{
				// PCサイト向けに優先したいクレジットカードメッセージ
				var cardErrorMessageForPc = string.Empty;

				this.IsSuccessfulCardRegistration = false;
				var resultCode = payTg.Result.Response.VResultCode;

				if (string.IsNullOrEmpty(resultCode))
				{
					// PayTg端末のエラーの場合はエラーメッセージを統一
					resultCode = PayTgConstants.ERRMSG_PAYTG_UNAVAILABLE;
				}
				var creditError = new CreditErrorMessage();
				creditError.SetCreditErrorMessages(Constants.FILE_XML_RAKUTEN_CREDIT_ERROR_MESSAGE);
				var errorList = creditError.GetValueItemArray();
				cardErrorMessageForPc = (errorList.Any(s => s.Value == resultCode))
					? errorList.First(s => (s.Value == resultCode)).Text
					: string.Empty;

				var payTgResponse = new Hashtable { { PayTgConstants.PAYTG_RESPONSE_ERROR, cardErrorMessageForPc } };
				this.PayTgResponse = payTgResponse;
				DisplayPaymentErrorMessage(cardErrorMessageForPc);
			}
		}

		DisplayCreditInputForm();
	}

	/// <summary>
	/// PayTg端末状態取得
	/// </summary>
	private void GetPayTgDeviceStatus()
	{
		var apiUrl = Constants.PAYMENT_SETTING_PAYTG_MOCK_ENABLED
			? Constants.PATH_ROOT + Constants.PAGE_MANAGER_CHECK_DEVICE_STATUS_MOCK
			: Constants.PAYMENT_SETTING_PAYTG_DEVICE_STATUS_CHECK_URL;

		// PayTG連動でカード登録実行
		ScriptManager.RegisterStartupScript(
			this,
			GetType(),
			"execGetDeviceStatus",
			string.Format("execGetPayTgDeviceStatus('{0}');", apiUrl),
			true);
	}

	/// <summary>
	/// 注文者情報取得＆入力チェック
	/// </summary>
	/// <param name="hasError">エラーがあるか</param>
	/// <returns>注文者情報</returns>
	private Hashtable GetAndCheckOwnerInput(ref bool hasError)
	{
		// 注文者情報入力チェック
		var ownerInput = GetOwnerInput();
		var errorMessage = CheckOwnerInput(ownerInput);

		// ユーザー情報更新の場合はログインID重複チェック（UserInputを暫定的に利用）
		if (cbAllowSaveOwnerIntoUser.Checked
			&& Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED
			&& UserService.IsPcSiteOrOfflineUser(hfOwnerKbn.Value))
		{
			var userInputForDuplicationCheck = new UserInput
			{
				LoginId = (string)ownerInput[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR],
				UserId = hfUserId.Value,
			};
			var duplicationMessage = userInputForDuplicationCheck.CheckDuplicationLoginId(
				this.IsOwnerAddrJp
					? UserInput.EnumUserInputValidationKbn.UserModify
					: UserInput.EnumUserInputValidationKbn.UserModifyGlobal,
				true);
			if (string.IsNullOrEmpty(duplicationMessage) == false)
			{
				errorMessage += duplicationMessage;
			}
		}

		if (DisplayOwnerErrorMessages(errorMessage))
		{
			ddlOrderKbn.Focus();
			hasError = true;
		}

		return ownerInput;
	}

	/// <summary>
	/// Linkbutton search address from owner zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrFromOwnerZipGlobal_Click(object sender, EventArgs e)
	{
		BindingAddressByGlobalZipcode(
			this.OwnerAddrCountryIsoCode,
			StringUtility.ToHankaku(tbOwnerZipGlobal.Text.Trim()),
			tbOwnerAddr2,
			tbOwnerAddr3,
			tbOwnerAddr4,
			tbOwnerAddr5,
			ddlOwnerAddr5);
	}

	/// <summary>
	/// Linkbutton search address from shipping zipcode global click event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbSearchAddrFromShippingZipGlobal_Click(object sender, EventArgs e)
	{
		BindingAddressByGlobalZipcode(
			this.ShippingAddrCountryIsoCode,
			StringUtility.ToHankaku(tbShippingZipGlobal.Text.Trim()),
			tbShippingAddr2,
			tbShippingAddr3,
			tbShippingAddr4,
			tbShippingAddr5,
			ddlShippingAddr5);
	}

	/// <summary>
	/// Set display or non-display for point and coupon
	/// </summary>
	private void SetDisplayForPointAndCoupon()
	{
		var isDisplay = (this.HasItems || this.HasOwner);
		dvShowCoupon.Visible
			= dvShowPoint.Visible
			= isDisplay;

		dvHideCoupon.Visible
			= dvHidePoint.Visible
			= (isDisplay == false);
	}

	/// <summary>
	/// セールID変更入力
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void tbProductSaleId_OnTextChanged(object sender, EventArgs e)
	{
		this.ChangeSaleId = true;
		RefreshItemsAndPriceView();
	}

	/// <summary>
	/// 月間隔日付指定の日付選択肢作成
	/// </summary>
	/// <param name="fixedPurchaseKbn1Setting2">登録された日付選択肢</param>
	/// <returns>日付選択肢</returns>
	protected ListItem[] GetFixedPurchaseKbnIsDays(string fixedPurchaseKbn1Setting2)
	{
		var daysList = new List<ListItem>();
		if (string.IsNullOrEmpty(fixedPurchaseKbn1Setting2))
		{
			return ValueText.GetValueItemArray(
				Constants.TABLE_SHOPSHIPPING,
				Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SETTING_DATE_LIST);
		}
		fixedPurchaseKbn1Setting2 = fixedPurchaseKbn1Setting2.Replace("(", string.Empty).Replace(")", string.Empty);

		daysList.AddRange(
			OrderCommon.GetKbn1FixedPurchaseIntervalListItems(
				fixedPurchaseKbn1Setting2,
				string.Empty,
				true));
		return daysList.ToArray();
	}

	/// <summary>
	/// Get rakuten cvs type
	/// </summary>
	private void GetRakutenCvsType()
	{
		var canDisplayRakutenCvsType = ((ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
			&& (Constants.PAYMENT_CVS_KBN == Constants.PaymentCvs.Rakuten));

		dvRakutenCvsType.Visible = canDisplayRakutenCvsType;

		if ((canDisplayRakutenCvsType == false)
			|| this.IsBackFromConfirm)
		{
			return;
		}

		ddlRakutenCvsType.Items.Clear();
		ddlRakutenCvsType.Items.AddRange(
			ValueText.GetValueItemArray(
				Constants.TABLE_PAYMENT,
				Constants.PAYMENT_RAKUTEN_CVS_TYPE));
	}

	/// <summary>
	/// Get delivery company ID
	/// </summary>
	/// <param name="cartShipping">A cart shipping</param>
	/// <returns>A delivery company ID</returns>
	private string GetDeliveryCompanyId(CartShipping cartShipping)
	{
		var deliveryCompanyId = (this.IsBackFromConfirm == false)
			? ddlDeliveryCompany.SelectedValue
			: cartShipping.DeliveryCompanyId;
		return deliveryCompanyId;
	}

	/// <summary>
	/// Get shipping date
	/// </summary>
	/// <returns>A shipping date</returns>
	private DateTime? GetShippingDate()
	{
		var shippingDate = (string.IsNullOrEmpty(ddlShippingDate.SelectedValue) == false)
				? DateTime.Parse(ddlShippingDate.SelectedValue)
				: (DateTime?)null;
		return shippingDate;
	}

	/// <summary>
	/// Set first shipping date
	/// </summary>
	/// <param name="fixedPurchaseKbn">Fixed purchase kbn</param>
	/// <param name="fixedPurchaseSetting">Fixed purchase setting</param>
	/// <param name="minSpan">Minimum shipping span</param>
	/// <param name="calculationMode">Calculation mode</param>
	/// <param name="shopShipping">Shop shipping</param>
	/// <param name="cartShipping">Cart shipping</param>
	private void SetFirstShippingDate(
		string fixedPurchaseKbn,
		string fixedPurchaseSetting,
		int minSpan,
		NextShippingCalculationMode calculationMode,
		ShopShippingModel shopShipping,
		CartShipping cartShipping)
	{
		// Generate first shipping date options base on shortest shipping date
		var deliveryCompanyId = GetDeliveryCompanyId(cartShipping);
		var shortestShippingDate = HolidayUtil.GetShortestShippingDateBasedOnToday(deliveryCompanyId);
		var dateEnd = Constants.DISPLAY_DEFAULTSHIPPINGDATE_ENABLED
			? (int)shopShipping.ShippingDateSetEnd + 1
			: (int)shopShipping.ShippingDateSetEnd;

		if ((shopShipping.CanUseFixedPurchaseFirstShippingDateNextMonth(fixedPurchaseKbn) == false)
			|| ((fixedPurchaseKbn != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE)
				&& (fixedPurchaseKbn != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY))
			|| ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
		{
			// Default first shipping date
			cartShipping.ShippingDate = null;
			var firstShippingDate = cartShipping.GetFirstShippingDate();
			if (ddlShippingDate.Items.Count > dateEnd)
			{
				ddlShippingDate.Items.RemoveAt(0);
			}

			// Set First shipping date from drop down list shipping date
			if ((ddlShippingDate.SelectedValue != string.Empty)
				&& shopShipping.IsValidShippingDateSetFlg)
			{
				var shippingDate = DateTime.Parse(ddlShippingDate.SelectedValue);
				cartShipping.ShippingDate = shippingDate;
				firstShippingDate = cartShipping.GetFirstShippingDate();
				dvShippingDate.Visible = true;
			}
			lFirstShippingDate.Text = GetEncodedHtmlDisplayMessage(
				DateTimeUtility.ToStringForManager(
					firstShippingDate,
					DateTimeUtility.FormatType.LongDateWeekOfDay2Letter));

			cartShipping.FirstShippingDate = firstShippingDate;
			ddlNextShippingDate.Visible = true;
			return;
		}
		else
		{
			dvShippingDate.Visible = false;
			ddlNextShippingDate.Visible = false;
		}

		var earlyDate = OrderCommon.GetFirstShippingDateBasedOnToday(
				cartShipping.CartObject.ShopId,
				cartShipping.FixedPurchaseDaysRequired,
				null,
				cartShipping.ShippingMethod,
				cartShipping.DeliveryCompanyId,
				cartShipping.ShippingCountryIsoCode,
				cartShipping.IsTaiwanCountryShippingEnable
					? cartShipping.Addr2
					: cartShipping.Addr1,
				cartShipping.Zip);

		// Setting default 1 month for option 1
		var settingOption1 = fixedPurchaseSetting.Remove(0, 1).Insert(0, "1");

		// Set first shipping date option 1 and 2
		var firstShippingDateOption1 =
			DomainFacade.Instance.FixedPurchaseService.CalculateFirstShippingDate(
				fixedPurchaseKbn,
				settingOption1,
				earlyDate.AddMonths(-1),
				minSpan,
				calculationMode);

		if (firstShippingDateOption1 < earlyDate)
		{
			firstShippingDateOption1 =
				DomainFacade.Instance.FixedPurchaseService.CalculateFirstShippingDate(
					fixedPurchaseKbn,
					settingOption1,
					earlyDate,
					minSpan,
					calculationMode);
		}
		else if (firstShippingDateOption1.Month == DateTime.Now.Month)
		{
			firstShippingDateOption1 =
				DomainFacade.Instance.FixedPurchaseService.CalculateFirstShippingDate(
					fixedPurchaseKbn,
					settingOption1,
					firstShippingDateOption1,
					minSpan,
					calculationMode);
		}

		var firstShippingDateOption1Text =
			DateTimeUtility.ToStringForManager(
				firstShippingDateOption1,
				DateTimeUtility.FormatType.LongDateWeekOfDay2Letter);

		var firstShippingDateOption2 =
			DomainFacade.Instance.FixedPurchaseService.CalculateFirstShippingDate(
				fixedPurchaseKbn,
				settingOption1,
				firstShippingDateOption1,
				minSpan,
				calculationMode);
		var nextShippingDateOption2Text =
			DateTimeUtility.ToStringForManager(
				firstShippingDateOption2,
				DateTimeUtility.FormatType.LongDateWeekOfDay2Letter);

		var firstShippingDateOptions = new ListItem[]
		{
			new ListItem(
				firstShippingDateOption1Text,
				firstShippingDateOption1.ToString(CONST_FORMAT_SHORT_DATE)),
			new ListItem(
				nextShippingDateOption2Text,
				firstShippingDateOption2.ToString(CONST_FORMAT_SHORT_DATE)),
		};

		var selectedFirstShippingDate = cartShipping.FirstShippingDate;
		var isSelected = firstShippingDateOptions
			.Any(item => item.Value == selectedFirstShippingDate.ToString(CONST_FORMAT_SHORT_DATE));
		cartShipping.FirstShippingDate = isSelected
			? selectedFirstShippingDate
			: firstShippingDateOption1;

		// Binding to controls
		if (ddlFirstShippingDate.Items.Count == 0
			|| (ddlFirstShippingDate.Items.Contains(firstShippingDateOptions[1])))
		{
			ddlFirstShippingDate.Items.Clear();
			ddlFirstShippingDate.Items.AddRange(firstShippingDateOptions);
		}
		else if (ddlFirstShippingDate.SelectedItem.Value != firstShippingDateOption1.ToString(CONST_FORMAT_SHORT_DATE))
		{
			if (this.IsUpdateFixedPurchaseShippingPattern == false)
			{
				cartShipping.FirstShippingDate = DateTime.Parse(ddlFirstShippingDate.SelectedItem.Value);
			}
			else if (ddlFirstShippingDate.Items.FindByValue(
				cartShipping.FirstShippingDate.ToString(CONST_FORMAT_SHORT_DATE)) == null
					|| this.IsUpdateFixedPurchaseShippingPattern)
			{
				ddlFirstShippingDate.Items.Clear();
				ddlFirstShippingDate.Items.AddRange(firstShippingDateOptions);
			}
		}
		if ((this.IsBackFromConfirm || this.IsUpdatePayment)
			&& (cartShipping.ShippingDate != null))
		{
			cartShipping.FirstShippingDate = (DateTime)cartShipping.ShippingDate;
		}

		ddlFirstShippingDate.Visible = true;
		ddlFirstShippingDate.SelectedValue = cartShipping.FirstShippingDate.ToString(CONST_FORMAT_SHORT_DATE);
		lFirstShippingDate.Text = GetEncodedHtmlDisplayMessage(
			DateTimeUtility.ToStringForManager(
				cartShipping.FirstShippingDate,
				DateTimeUtility.FormatType.LongDateWeekOfDay2Letter));

		// Insert to dropdownlist shipping date
		InsertShippingDate(ddlShippingDate, dateEnd, ddlFirstShippingDate.SelectedItem);
		ddlShippingDate.SelectedIndex = 0;
	}

	/// <summary>
	/// Set next shipping date
	/// </summary>
	/// <param name="fixedPurchaseKbn">Fixed purchase kbn</param>
	/// <param name="fixedPurchaseSetting">Fixed purchase setting</param>
	/// <param name="firstShippingDate">First shipping date</param>
	/// <param name="nextShippingDate">Next shipping date</param>
	/// <param name="nextNextShippingDate">Next next shipping date</param>
	/// <param name="minSpan">Minimum shipping span</param>
	/// <param name="cartShipping">Cart shipping</param>
	/// <param name="shopShipping">Shop shipping model</param>
	private void SetNextShippingDate(
		string fixedPurchaseKbn,
		string fixedPurchaseSetting,
		DateTime firstShippingDate,
		DateTime nextShippingDate,
		DateTime nextNextShippingDate,
		int minSpan,
		CartShipping cartShipping,
		ShopShippingModel shopShipping = null)
	{
		// Shortest delivery date that can be additionally selected
		var shortestShippingDate =
			DomainFacade.Instance.FixedPurchaseService.CalculateFollowingShippingDate(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				firstShippingDate,
				minSpan,
				NextShippingCalculationMode.Earliest);

		//  Generate next shipping date options base on first shipping date
		var nextShippingDateText = DateTimeUtility.ToStringForManager(
				nextShippingDate,
				DateTimeUtility.FormatType.LongDateWeekOfDay2Letter);
		var nextShippingDateOption = new ListItem(
			nextShippingDateText,
			nextShippingDate.ToString(CONST_FORMAT_SHORT_DATE));
		nextShippingDateOption.Selected = true;

		ListItem[] nextShippingDateOptions = null;
		if (shortestShippingDate == nextShippingDate)
		{
			var nextNextShippingDateText = DateTimeUtility.ToStringForManager(
				nextNextShippingDate,
				DateTimeUtility.FormatType.LongDateWeekOfDay2Letter);
			var nextNextShippingDateOption = new ListItem(
				nextNextShippingDateText,
				nextNextShippingDate.ToString(CONST_FORMAT_SHORT_DATE));
			nextShippingDateOptions = new ListItem[] { nextShippingDateOption, nextNextShippingDateOption };
		}
		else
		{
			var shortestShippingDateText = DateTimeUtility.ToStringForManager(
				shortestShippingDate,
				DateTimeUtility.FormatType.LongDateWeekOfDay2Letter);
			var shortestShippingDateOption = new ListItem(
				shortestShippingDateText,
				shortestShippingDate.ToString(CONST_FORMAT_SHORT_DATE));
			nextShippingDateOptions = new ListItem[] { shortestShippingDateOption, nextShippingDateOption };
		}

		var selectedNextShippingDate = cartShipping.NextShippingDate;
		var isSelected = nextShippingDateOptions
			.Any(item => item.Value == selectedNextShippingDate.ToString(CONST_FORMAT_SHORT_DATE));

		// Binding to controls
		lNextShippingDate.Text = GetEncodedHtmlDisplayMessage(nextShippingDateText);
		ddlNextShippingDate.Items.Clear();
		ddlNextShippingDate.Items.AddRange(nextShippingDateOptions);
		if ((shopShipping != null)
			&& (shopShipping.CanUseFixedPurchaseFirstShippingDateNextMonth(fixedPurchaseKbn) == false))
		{
			ddlNextShippingDate.Visible = true;
			ddlNextShippingDate.SelectedValue = isSelected
				? selectedNextShippingDate.ToString(CONST_FORMAT_SHORT_DATE)
				: nextShippingDate.ToString(CONST_FORMAT_SHORT_DATE);
		}
	}

	/// <summary>
	/// Set next shipping date
	/// </summary>
	/// <param name="fixedPurchaseKbn">Fixed purchase kbn</param>
	/// <param name="fixedPurchaseSetting">Fixed purchase setting</param>
	/// <param name="firstShippingDate">First shipping date</param>
	/// <param name="daysRequired">Shipping days required</param>
	/// <param name="minSpan">Minimum shipping span</param>
	/// <param name="calculationMode">Calculation mode</param>
	/// <param name="cartShipping">Cart shipping</param>
	private void SetNextShippingDate(
		string fixedPurchaseKbn,
		string fixedPurchaseSetting,
		DateTime firstShippingDate,
		int daysRequired,
		int minSpan,
		NextShippingCalculationMode calculationMode,
		CartShipping cartShipping)
	{
		var nextShippingDate =
			DomainFacade.Instance.FixedPurchaseService.CalculateNextShippingDate(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				firstShippingDate,
				daysRequired,
				minSpan,
				calculationMode);
		var nextNextShippingDate =
			DomainFacade.Instance.FixedPurchaseService.CalculateNextNextShippingDate(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				firstShippingDate,
				daysRequired,
				minSpan,
				calculationMode);

		// If the payment update does not set next shipping date
		if (this.IsUpdatePayment == false)
		{
			SetNextShippingDate(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				firstShippingDate,
				nextShippingDate,
				nextNextShippingDate,
				minSpan,
				cartShipping);
		}
	}

	/// <summary>
	/// First shipping date drop down selected index changed event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlFirstShippingDate_SelectedIndexChanged(object sender, EventArgs e)
	{
		var orderItems = CreateOrderItemInput();
		if (string.IsNullOrEmpty(CheckItemsAndPricesInput(orderItems)) == false) return;

		var hasError = false;
		var cart = CreateCart(orderItems, ref hasError);
		var shopShipping = DomainFacade.Instance.ShopShippingService.Get(
			this.LoginOperatorShopId,
			cart.ShippingType);

		cart.GetShipping().FirstShippingDate = DateTime.Parse(ddlFirstShippingDate.SelectedValue);
		SetFirstAndNextShippingDate(cart, shopShipping);
	}

	/// <summary>
	/// 配送方法選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlNextShippingDate_SelectedIndexChanged(object sender, EventArgs e)
	{
		var value = ddlNextShippingDate.SelectedValue;
		lNextShippingDate.Text = DateTimeUtility.ToStringForManager(
				value,
				DateTimeUtility.FormatType.LongDateWeekOfDay2Letter);
	}

	/// <summary>
	/// Insert shipping date
	/// </summary>
	/// <param name="ddlShippingDate">Drop down list shipping date</param>
	/// <param name="date">Date</param>
	/// <param name="nextDate">Next date</param>
	private void InsertShippingDate(DropDownList ddlShippingDate, int date, ListItem nextDate)
	{
		if (ddlShippingDate.Items.Count > date && (this.IsBackFromConfirm == false))
		{
			ddlShippingDate.Items.RemoveAt(0);
		}
		if (ddlShippingDate.Items.Count != 0)
		{
			var shippingDates = ddlShippingDate.Items.Cast<ListItem>().ToArray();
			var isSelected = shippingDates.Count(listItem => listItem.Value == nextDate.Value) > 0;
			if (isSelected)
			{
				ddlShippingDate.SelectedValue = nextDate.Value;
			}
			else
			{
				if (this.IsBackFromConfirm)
				{
					ddlShippingDate.SelectedIndex = 0;
				}
				else
				{
					ddlShippingDate.Items.Insert(0, nextDate);
					ddlShippingDate.SelectedIndex = 0;
				}
			}
		}
		else
		{
			ddlShippingDate.Items.Insert(0, nextDate);
			ddlShippingDate.SelectedIndex = 0;
		}
	}

	/// <summary>
	/// 頒布会定額コースのカート商品か
	/// </summary>
	/// <param name="product">商品情報</param>
	/// <returns>頒布会定額コースのカート商品であればTRUE</returns>
	protected bool IsCartProductSubscriptionBoxFixedAmount(object product)
	{
		var fixedAmount = GetKeyValue(product, Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT);
		var result = string.IsNullOrEmpty(StringUtility.ToEmpty(fixedAmount)) == false;
		return result;
	}

	#region プロパティ
	/// <summary>アイテムがあるか</summary>
	private bool HasItems
	{
		get
		{
			var result = (rItemList.Items.Count > 0)
				&& (rItemList.Items
					.Cast<RepeaterItem>()
					.Any(riItem => (string.IsNullOrEmpty(((TextBox)riItem.FindControl("tbProductId")).Text.Trim()) == false)));
			return result;
		}
	}
	/// <summary>会員か判定</summary>
	protected bool IsUser
	{
		get { return UserService.IsUser(hfOwnerKbn.Value); }
	}
	/// <summary>クーポン用ユーザーID</summary>
	/// <remarks>
	/// クーポン取得処理では会員向けかをuser_idの存在で判断している箇所があるが、
	/// 未登録会員（オフライン会員）の注文時に不都合があるためカートに一度登録されたクーポン情報の再取得については
	/// dummyというユーザーIDで取得するようにする。（これを利用しないと会員対象クーポンが取得出来ない）
	/// </remarks>
	protected string CouponUserId
	{
		get
		{
			return (this.IsUser
				? (string.IsNullOrEmpty(hfUserId.Value)
					? CouponOptionUtility.DUMMY_USER_ID_FOR_USER_COUPON_CHECK
					: hfUserId.Value)
				: string.Empty);
		}
	}
	/// <summary>クーポン用メールアドレス</summary>
	/// <remarks>ブラックリスト型クーポン利用済みユーザー判定用</remarks>
	protected string CouponMailAddress
	{
		get { return StringUtility.ToEmpty(ViewState[Constants.FIELD_USER_MAIL_ADDR]); }
		set { ViewState[Constants.FIELD_USER_MAIL_ADDR] = value; }
	}
	/// <summary>カードリスト</summary>
	protected UserCreditCard[] CreditCardList
	{
		get { return (UserCreditCard[])(ViewState["CreditCardList"] ?? new UserCreditCard[0]); }
		private set { ViewState["CreditCardList"] = value; }
	}
	/// <summary>再注文の注文ID</summary>
	protected string ReOrderId
	{
		get { return (string)ViewState["ReOrderId"]; }
		private set { ViewState["ReOrderId"] = value; }
	}
	/// <summary>注文同梱対象 親注文ID</summary>
	protected string CombineParentOrderId
	{
		get { return StringUtility.ToEmpty(ViewState["CombineParentOrderId"]); }
		set { ViewState["CombineParentOrderId"] = value; }
	}
	/// <summary>注文同梱対象 子注文IDs</summary>
	protected string[] CombineChildOrderIds
	{
		get { return (string[])ViewState["CombineChildOrderIds"]; }
		set { ViewState["CombineChildOrderIds"] = value; }
	}
	/// <summary>Combine Parent Tran Id</summary>
	protected string CombineParentTranId
	{
		get { return StringUtility.ToEmpty(ViewState["CombineParentTranId"]); }
		set { ViewState["CombineParentTranId"] = value; }
	}
	/// <summary>Is Combine Order Sales Settled</summary>
	protected bool IsCombineOrderSalesSettled
	{
		get { return ((bool)(ViewState["IsCombineOrderSalesSettled"] ?? false)); }
		set { ViewState["IsCombineOrderSalesSettled"] = value; }
	}
	/// <summary>注文同梱有無</summary>
	protected bool IsOrderCombined
	{
		get { return (bool)(ViewState["IsOrderCombined"] ?? false); }
		set { ViewState["IsOrderCombined"] = value; }
	}
	/// <summary>注文同梱ページでの注文同梱有無</summary>
	protected bool IsOrderCombinedAtOrderCombinePage
	{
		get { return (this.IsOrderCombined && (this.CombineChildOrderIds != null)); }
	}
	/// <summary>注文同梱前カート</summary>
	protected CartObject OrderCombineBeforeCart
	{
		get { return (CartObject)ViewState["OrderCombineBeforeCart"]; }
		set { ViewState["OrderCombineBeforeCart"] = value; }
	}
	/// <summary>注文同梱対応 親注文定期購入有無</summary>
	protected bool CombineParentOrderHasFixedPurchase
	{
		get { return (bool)(ViewState["CombineParentOrderHasFixedPurchase"] ?? false); }
		set { ViewState["CombineParentOrderHasFixedPurchase"] = value; }
	}
	/// <summary>注文同梱 親注文定期購入回数</summary>
	protected int CombineParentOrderCount
	{
		get { return (int)ViewState["CombineParentOrderCount"]; }
		set { ViewState["CombineParentOrderCount"] = value; }
	}
	/// <summary>確認画面から戻ってきたか</summary>
	protected bool IsBackFromConfirm
	{
		get
		{
			return ((Session[Constants.SESSION_KEY_PARAM_FOR_BACK] != null)
				&& (Session[Constants.SESSION_KEY_PARAM_FOR_BACK] is Hashtable));
		}
	}
	/// <summary>会員ランク情報が利用可能か</summary>
	protected bool IsAvailableRank
	{
		get
		{
			return ((Constants.MEMBER_RANK_OPTION_ENABLED)
				&& (MemberRankOptionUtility.GetMemberRankList().Any(rank
					=> rank.MemberRankId == hfMemberRankId.Value)));
		}
	}
	/// <summary>Deleted Novelty Ids</summary>
	protected List<string> DeletedNoveltyIds
	{
		get { return (List<string>)(Session["DeletedNoveltyIds"] ?? new List<string>()); }
		set { Session["DeletedNoveltyIds"] = value; }
	}
	/// <summary>Is Order Combined At Order Regist Page</summary>
	public bool IsOrderCombinedAtOrderRegistPage
	{
		get { return (this.IsOrderCombined && (this.CombineChildOrderIds == null)); }
	}

	/// <summary>
	/// 頒布会販売種類数チェック
	/// </summary>
	/// <param name="numberOfProducts">商品種類数</param>
	/// <param name="subscriptionManagementName">頒布会名</param>
	/// <param name="minNumberOfProducts">最低商品種類数</param>
	/// <param name="maxNumberOfProducts">最大商品種類数</param>
	private string CheckSubscriptionBoxNumberOfProducts(
		int numberOfProducts,
		string subscriptionManagementName,
		int? minNumberOfProducts,
		int? maxNumberOfProducts)
	{
		var message = GetSubscriptionBoxProductOfNumberError(
			subscriptionManagementName,
			numberOfProducts,
			minNumberOfProducts,
			maxNumberOfProducts);
		return message;
	}

	/// <summary>
	/// 頒布会販売数量チェック
	/// </summary>
	/// <param name="orderItems">注文商品</param>
	/// <param name="subscriptionManagementName">頒布会名</param>
	/// <param name="minimumPurchaseQuantity">最低商品数量</param>
	/// <param name="maximumPurchaseQuantity">最大商品数量</param>
	private string CheckSubscriptionBoxLimitProduct(
		List<Hashtable> orderItems,
		string subscriptionManagementName,
		int? minimumPurchaseQuantity,
		int? maximumPurchaseQuantity)
	{
		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED == false)
		{
			return String.Empty;
		}

		var totalQuantity = orderItems.Sum(item =>
		{
			int quantity;
			int.TryParse((string)item[Constants.FIELD_ORDERITEM_ITEM_QUANTITY], out quantity);
			return quantity;
		});

		var message = OrderPage.GetSubscriptionBoxQuantityError(
			totalQuantity,
			maximumPurchaseQuantity,
			minimumPurchaseQuantity,
			subscriptionManagementName);

		return message;
	}

	/// <summary>
	/// 頒布会数量系チェック
	/// </summary>
	/// <param name="orderItems">注文商品</param>
	private void CheckSubscriptionBoxQuantity(List<Hashtable> orderItems)
	{
		var errorMessage = string.Empty;

		var subscriptionBoxCourses = orderItems
			.GroupBy(item => (string)item[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID]);
		foreach (var courseGroup in subscriptionBoxCourses)
		{
			if (string.IsNullOrEmpty(courseGroup.Key)) continue;

			var subscription = DataCacheControllerFacade
				.GetSubscriptionBoxCacheController()
				.Get(courseGroup.Key);

			// 商品種類数チェック
			var numberOfProductsErrorMessage = CheckSubscriptionBoxNumberOfProducts(
				courseGroup.Count(),
				subscription.ManagementName,
				subscription.MinimumNumberOfProducts,
				subscription.MaximumNumberOfProducts);

			// 商品数量チェック
			var limitProductErrorMessage = CheckSubscriptionBoxLimitProduct(
				courseGroup.ToList(),
				subscription.ManagementName,
				subscription.MinimumPurchaseQuantity,
				subscription.MaximumPurchaseQuantity);

			errorMessage += string.IsNullOrEmpty(numberOfProductsErrorMessage) ? "" : numberOfProductsErrorMessage + "<br/>";
			errorMessage += string.IsNullOrEmpty(limitProductErrorMessage) ? "" : limitProductErrorMessage + "<br/>";
		}

		lOrderItemNoticeMessage.Text = errorMessage;
		dvOrderItemNoticeMessage.Visible = string.IsNullOrEmpty(errorMessage) == false;
		return;
	}

	/// <summary>
	/// 商品検索ポップアップURL取得
	/// </summary>
	/// <returns>URL</returns>
	protected string GetProductSearchPopupUrl()
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_PRODUCT_SEARCH)
			.AddParam(Constants.REQUEST_KEY_ORDER_MEMBER_RANK_ID, HttpUtility.UrlEncode(hfMemberRankId.Value))
			.AddParam(Constants.REQUEST_KEY_PRODUCT_VALID_FLG, Constants.FLG_PRODUCT_VALID_FLG_VALID);

		if (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (ddlSubscriptionBox.SelectedValue != string.Empty))
		{
			var subscriptionBoxFlag = string.Format("'{0}','{1}'", Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_VALID, Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY);
			urlCreator.AddParam(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_FLG, subscriptionBoxFlag)
				.AddParam(Constants.REQUEST_KEY_PRODUCT_SUBSCRIPTION_BOX_ID, ddlSubscriptionBox.SelectedValue)
				.AddParam(Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN, Constants.KBN_PRODUCT_SEARCH_SUBSCRIPTION_BOX);
			return urlCreator.CreateUrl();
		}

		urlCreator.AddParam(Constants.REQUEST_KEY_PRODUCT_SEARCH_KBN, HttpUtility.UrlEncode(Constants.KBN_PRODUCT_SEARCH_ORDERPRODUCT));
		return urlCreator.CreateUrl();
	}

	/// <summary>
	/// Refresh dropdown list shipping kbn list
	/// </summary>
	private void RefreshShippingOption()
	{
		var errorMsg = string.Empty;

		// Get shipping option list
		var selected = this.ddlShippingKbnList.SelectedValue;
		var realStoreList = ValueText.GetValueItemArray(Constants.TABLE_ORDER, Constants.FLG_ORDER_SHIPPING_KBN_LIST);

		// Get products that doesn't allow store pickup in cart
		var storePickupFlag = false;
		var ids = this.rItemList
			.Items.Cast<RepeaterItem>()
			.Select(riItem => ((TextBox)riItem.FindControl("tbProductId")).Text.Trim())
			.Where(id => string.IsNullOrEmpty(id) == false)
			.ToArray();

		if (ids.Length > 0)
		{
			storePickupFlag = DomainFacade
				.Instance
				.ProductService
				.GetProductsByProductIds(Constants.CONST_DEFAULT_SHOP_ID, ids)
				.Any(p => p.StorePickupFlg == Constants.FLG_OFF);
		}

		// Get fixed purchase products in cart
		var fixedPurchase = this.rItemList.Items.Cast<RepeaterItem>().Any(riItem => ((CheckBox)riItem.FindControl("cbFixedPurchase")).Checked);

		// Get subscription box products in cart
		var subscriptionBox = string.IsNullOrEmpty(ddlSubscriptionBox.SelectedValue) == false;

		// Check if cart can use store pickup shipping option
		if ((Constants.STORE_PICKUP_OPTION_ENABLED == false)
			|| storePickupFlag
			|| fixedPurchase
			|| subscriptionBox
			|| (this.RealShopModels.Length == 0))
		{
			// Display error if selected value is store pickup
			if (selected == Constants.SHIPPING_KBN_LIST_STORE_PICKUP)
			{
				if (storePickupFlag)
				{
					errorMsg = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_STORE_PICKUP_ERROR_CART_CONTAINS_NON_STORE_PICKUP_PRODUCTS);
				}
				else if (fixedPurchase)
				{
					errorMsg = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_STORE_PICKUP_ERROR_CART_CONTAINS_FIXED_PURCHASE);
				}
				else if (subscriptionBox)
				{
					errorMsg = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_STORE_PICKUP_ERROR_CART_CONTAINS_SUBSCRIPTIONBOX);
				}

				// Change selected value
				selected = Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER;

				// Set real shop list data back to default
				if ((this.RealShopModels != null)
					&& (this.RealShopModels.Length > 0))
				{
					this.ddlRealStore.SelectedIndex = 0;
					this.ddlRealStore_SelectedIndexChanged(null, EventArgs.Empty);
				}
			}
			realStoreList = realStoreList.Where(rs => rs.Value != Constants.SHIPPING_KBN_LIST_STORE_PICKUP).ToArray();
		}

		this.ddlShippingKbnList.Items.Clear();
		this.ddlShippingKbnList.Items.AddRange(realStoreList);
		this.ddlShippingKbnList.SelectedValue = selected;

		if (string.IsNullOrEmpty(errorMsg))
		{
			dvOrderShippingErrorMessages.Visible = false;
		}
		else
		{
			dvOrderShippingErrorMessages.Visible = true;
		}
		lOrderShippingErrorMessages.Text = errorMsg;
	}

	/// <summary>
	/// Dropdown list real store selected index changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlRealStore_SelectedIndexChanged(object sender, EventArgs e)
	{
		var index = this.ddlRealStore.SelectedIndex;
		if ((index < 0) || (index > this.RealShopModels.Length - 1)) return;

		this.lbStoreAddress.Text = string.Format(
			"〒{0}<br />{1} {2}<br />{3}<br />{4}<br />{5}",
			this.RealShopModels[index].Zip,
			this.RealShopModels[index].Addr1,
			this.RealShopModels[index].Addr2,
			this.RealShopModels[index].Addr3,
			this.RealShopModels[index].Addr4,
			this.RealShopModels[index].Addr5);
		this.lbStoreOpeningHours.Text = this.RealShopModels[index].OpeningHours;
		this.lbStoreTel.Text = this.RealShopModels[index].Tel;
	}

	/// <summary>注文者の住所が日本か</summary>
	protected bool IsOwnerAddrJp
	{
		get { return IsCountryJp(this.OwnerAddrCountryIsoCode); }
	}
	/// <summary>注文者の住所がアメリカか</summary>
	protected bool IsOwnerAddrUs
	{
		get { return IsCountryUs(this.OwnerAddrCountryIsoCode); }
	}
	/// <summary>注文者の住所郵便番号が必須か</summary>
	protected bool IsOwnerAddrZipNecessary
	{
		get { return IsAddrZipcodeNecessary(this.OwnerAddrCountryIsoCode); }
	}
	/// <summary>注文者の住所国ISOコード</summary>
	protected string OwnerAddrCountryIsoCode
	{
		get { return ddlOwnerCountry.SelectedValue; }
	}
	/// <summary>配送先の住所が日本か</summary>
	protected bool IsShippingAddrJp
	{
		get { return IsCountryJp(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>配送先の住所がアメリカか</summary>
	protected bool IsShippingAddrUs
	{
		get { return IsCountryUs(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>配送先の住所郵便番号が必須か</summary>
	protected bool IsShippingAddrZipNecessary
	{
		get { return IsAddrZipcodeNecessary(this.ShippingAddrCountryIsoCode); }
	}
	/// <summary>配送先の住所国ISOコード</summary>
	protected string ShippingAddrCountryIsoCode
	{
		get { return ddlShippingCountry.SelectedValue; }
	}
	/// <summary>注文同梱 親注文定期台帳</summary>
	protected FixedPurchaseModel CombineParentOrderFixedPurchase
	{
		get { return (FixedPurchaseModel)this.ViewState["CombineParentOrderFixedPurchase"]; }
		set { this.ViewState["CombineParentOrderFixedPurchase"] = value; }
	}
	/// <summary>注文同梱 注文同梱後の定期購入割引額</summary>
	protected decimal CombineFixedPurchaseDiscountPrice
	{
		get { return (decimal)ViewState["CombineFixedPurchaseDiscountPrice"]; }
		set { ViewState["CombineFixedPurchaseDiscountPrice"] = value; }
	}
	/// <summary>Is the delivery address in Taiwan?</summary>
	protected bool IsShippingAddrTw
	{
		get
		{
			return ((this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_SAME_AS_OWNER)
				? IsCountryTw(this.OwnerAddrCountryIsoCode)
				: ((ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW)
					|| (ddlUserShipping.SelectedValue == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE))
					? IsCountryTw(this.ShippingAddrCountryIsoCode)
					: IsCountryTw(DomainFacade.Instance.UserShippingService.Get(hfUserId.Value, int.Parse(ddlUserShipping.SelectedValue)).ShippingCountryIsoCode));
		}
	}
	/// <summary>User Shipping Address</summary>
	protected UserShippingModel[] UserShippingAddress
	{
		get
		{
			return (UserShippingModel[])(ViewState["UserShippingAddress"] ?? new UserShippingModel[0]);
		}
		set { ViewState["UserShippingAddress"] = value; }
	}
	/// <summary>Is Update Fixed Purchase Shipping Pattern</summary>
	private bool IsUpdateFixedPurchaseShippingPattern
	{
		get { return (bool)(ViewState["IsUpdateFixedPurchaseShippingPattern"] ?? false); }
		set { ViewState["IsUpdateFixedPurchaseShippingPattern"] = value; }
	}
	/// <summary>Has Owner</summary>
	protected bool HasOwner
	{
		get
		{
			return ((string.IsNullOrEmpty(hfUserId.Value) == false)
				|| ((this.CartOwner != null)
					&& (string.IsNullOrEmpty(this.CartOwner.Name) == false)));
		}
	}
	/// <summary>Cart Owner</summary>
	protected CartOwner CartOwner
	{
		get { return (CartOwner)ViewState["CartOwner"]; }
		private set { ViewState["CartOwner"] = value; }
	}
	/// <summary>Fixed Purchase Id For Reorder</summary>
	protected string ReFixedPurchaseId
	{
		get { return (string)ViewState["ReFixedPurchaseId"]; }
		private set { ViewState["ReFixedPurchaseId"] = value; }
	}
	/// <summary>Account Email</summary>
	protected string AccountEmail { get; set; }
	/// <summary>Use Order Combine</summary>
	protected bool UseOrderCombine
	{
		get { return (bool)(ViewState["UseOrderCombine"] ?? false); }
		set { ViewState["UseOrderCombine"] = value; }
	}
	/// <summary>Current order item count</summary>
	protected int CurrentOrderItemCount
	{
		get { return (int)(ViewState["CurrentOrderItemCount"] ?? 1); }
		set { ViewState["CurrentOrderItemCount"] = value; }
	}
	/// <summary>Order regist input root url</summary>
	protected string OrderRegistInputRootUrl
	{
		get { return (Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_REGIST_INPUT); }
	}
	/// <summary>デジタルコンテンツ商品か</summary>
	private bool IsDigitalContents
	{
		get
		{
			return (hfIsDigitalContents.Value == Constants.FLG_PRODUCT_DIGITAL_CONTENTS_FLG_VALID);
		}
	}
	/// <summary>楽天連携以外か</summary>
	protected bool IsNotRakutenAgency
	{
		get { return (Constants.PAYMENT_CARD_KBN != w2.App.Common.Constants.PaymentCard.Rakuten); }
	}
	/// <summary>セールID変更有無</summary>
	private bool ChangeSaleId { get; set; }
	/// <summary>最低商品種類数</summary>
	private int? MinNumberOfProducts
	{
		get { return (int?)SessionManager.Session[Constants.SESSION_KEY_MIN_NUMBER_OF_PRODUCTS]; }
		set { SessionManager.Session[Constants.SESSION_KEY_MIN_NUMBER_OF_PRODUCTS] = value; }
	}
	/// <summary>最大商品種類数</summary>
	private int? MaxNumberOfProducts
	{
		get { return (int?)SessionManager.Session[Constants.SESSION_KEY_MAX_NUMBER_OF_PRODUCTS]; }
		set { SessionManager.Session[Constants.SESSION_KEY_MAX_NUMBER_OF_PRODUCTS] = value; }
	}
	/// <summary>最低購入数量</summary>
	private int? MinQuantity
	{
		get { return (int?)SessionManager.Session[Constants.SESSION_KEY_MIN_QUANTITY]; }
		set { SessionManager.Session[Constants.SESSION_KEY_MIN_QUANTITY] = value; }
	}
	/// <summary>最大購入数量</summary>
	private int? MaxQuantity
	{
		get { return (int?)SessionManager.Session[Constants.SESSION_KEY_MAX_QUANTITY]; }
		set { SessionManager.Session[Constants.SESSION_KEY_MAX_QUANTITY] = value; }
	}
	/// <summary>頒布会管理名</summary>
	private string SubscriptionManagementName
	{
		get { return (string)SessionManager.Session[Constants.SESSION_KEY_SUBSCRIPTION_MANAGEMENT_NAME]; }
		set { SessionManager.Session[Constants.SESSION_KEY_SUBSCRIPTION_MANAGEMENT_NAME] = value; }
	}
	/// <summary>頒布会コースID</summary>
	protected string SubscriptionBoxCourseId
	{
		get { return (string)Session["SubscriptionBoxCourseId"]; }
		set { Session["SubscriptionBoxCourseId"] = value; }
	}
	/// <summary>頒布会カートか</summary>
	protected bool IsSubscriptionBoxValid
	{
		get
		{
			return (Constants.SUBSCRIPTION_BOX_OPTION_ENABLED && (string.IsNullOrEmpty(this.SubscriptionBoxCourseId) == false));
		}
	}
	/// <summary>カート内商品サイズ係数（メール便配送サービスエスレーション機能用）</summary>
	protected int ProductsSizeForDeliveryCompanyMailEscalation { get; set; }
	/// <summary>Is update payment</summary>
	private bool IsUpdatePayment { get; set; }
	/// <summary>同じ頒布会コースの注文同梱か</summary>
	private bool IsOrderCombinedWithSameSubscriptionBox
	{
		get
		{
			return (ViewState["IsOrderCombinedWithSameSubscriptionBox"] != null)
				&& (bool)ViewState["IsOrderCombinedWithSameSubscriptionBox"];
		}
		set { ViewState["IsOrderCombinedWithSameSubscriptionBox"] = value; }
	}
	/// <summary>Real shop model list</summary>
	protected RealShopModel[] RealShopModels
	{
		get { return (RealShopModel[])ViewState["real_shop_models"]; }
		set { ViewState["real_shop_models"] = value; }
	}
	/// <summary>注文同梱カート情報</summary>
	protected CartObject OriginOrderCombineCart
	{
		get { return (CartObject)Session[Constants.SESSION_KEY_ORDERCOMBINE_ORIGIN_CART]; }
		set { Session[Constants.SESSION_KEY_ORDERCOMBINE_ORIGIN_CART] = value; }
	}
	/// <summary>注文同梱時利用クレジットカード情報</summary>
	protected UserCreditCard OrderCombineCardInfo { get; set; }
	/// <summary>Is can add shipping date</summary>
	protected bool IsCanAddShippingDate
	{
		get
		{
			var fixedPurchaseSettings = GetFixedPurchaseSettingInputs();
			if (rbMonthlyPurchase_Date.Checked || rbMonthlyPurchase_WeekAndDay.Checked)
			{
				var shippingFixedPurchaseErrorMessage = Validator.Validate("OrderShippingRegistInput", fixedPurchaseSettings);
				if (string.IsNullOrEmpty(shippingFixedPurchaseErrorMessage) == false)
					return false;
			}
			return this.ShopShipping.CanUseFixedPurchaseFirstShippingDateNextMonth(
				StringUtility.ToEmpty(fixedPurchaseSettings[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN]));
		}
	}
	/// <summary>Shop shipping model</summary>
	protected ShopShippingModel ShopShipping { get; set; }
	/// <summary>PayTgクレジットトークン</summary>
	protected string CreditTokenbyPayTg
	{
		get { return (string)this.Session[PayTgConstants.PARAM_TOKEN]; }
		set { this.Session[PayTgConstants.PARAM_TOKEN] = value; }
	}
	/// <summary>PayTgクレジット会社コード</summary>
	protected string CreditCardCompanyCodebyPayTg
	{
		get { return (string)this.Session[PayTgConstants.PARAM_ACQNAME]; }
		set { this.Session[PayTgConstants.PARAM_ACQNAME] = value; }
	}
	#endregion
}
