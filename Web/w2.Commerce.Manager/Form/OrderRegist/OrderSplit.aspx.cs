/*
=========================================================================================================
  Module      : 注文情報登録確認ページ処理(OrderSplit.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Config;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Mail;
using w2.App.Common.NextEngine;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.OrderCombine;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.Holiday.Helper;
using System.Text;
using w2.Domain.DeliveryCompany;
using w2.App.Common.Input.Order;
using w2.Common.Extensions;
using w2.Domain.FixedPurchase.Helper;
using w2.App.Common.Order.Payment.PayPal;
using w2.App.Common.Order.Payment.PayTg;
using w2.App.Common.Web.Process;
using w2.App.Common.ShippingBaseSettings;
using w2.Domain.UserShipping;
using System.Xml;
using w2.App.Common.DataCacheController;
using w2.App.Common.OrderExtend;
using w2.Domain.RealShop;

/// <summary>
/// 注文情報登録確認ページ処理
/// </summary>
public partial class Form_OrderRegist_OrderSplit : OrderRegistPage
{
	public const string SESSION_KEY_PARAM_FOR_ORDER_REGIST_ShippingList = "ShippingList";
	protected const string IS_SHOWING_DELIVERY_DESIGNATION = "ShowDeliveryDesignation";
	/// <summary>Format short date</summary>
	protected const string CONST_FORMAT_SHORT_DATE = "yyyy/MM/dd";
	/// <summary>ハッシュキー：カート商品用の注文同梱有無</summary>
	protected const string CONST_HASHKEY_CART_PRODUCT_IS_ORDER_COMBINED = "cartproduct_is_order_combined";
	/// <summary>Fixed pruchase</summary>
	protected const string CONST_KEY_FIXED_PURCHASE = "fixedpurchase";
	private ShopShippingModel[] shopShippings;
	private string shippingNo;
	string selectedShippingBaseId;
	string selectedShippingNames;
	string selectedOrderId;
	int selectedOrderIndex;
	List<CartObject> newCart = new List<CartObject>();

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 確認画面表示
		if (!IsPostBack)
		{
			RefreshView();

			// 注文同梱から遷移の場合、注文同梱後の情報を入力欄にセット
			if ((this.ActionStatus == Constants.ACTION_STATUS_ORDERCOMBINE)
				&& (this.IsBackFromConfirm == false))
			{
				setOrderList();
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
			}
		}
		else
		{
			setOrderList();
			dvOrderIdErrorMessages.Visible = false;
			string postBackControlID = Request["__EVENTTARGET"];

			selectedShippingNames = hfSelectedShippingNo.Value;
			selectedOrderId = hfSelectedOrderId.Value;

			var newOrderBaseList = (Dictionary<String, String>)this.OrderParams["orderIdByShippingBase"];

			foreach (var orderBase in newOrderBaseList)
			{
				if (orderBase.Value == selectedOrderId)
				{
					selectedShippingBaseId = orderBase.Key;
				}
			}
			shopShippings = DomainFacade.Instance.ShopShippingService.GetShopShippingByShippingBaseId(this.LoginOperatorShopId, selectedShippingBaseId);
			if (postBackControlID.Contains("ddlDeliveryCompany"))
			{
				shopShippings = DomainFacade.Instance.ShopShippingService.GetShopShippingByShippingBaseId(this.LoginOperatorShopId, selectedShippingBaseId);
			}
			if (postBackControlID.Contains("rOrderListForTable"))
			{
				selectedShippingNames = hfSelectedShippingNo.Value;
				selectedOrderIndex = int.Parse(hfSelectedOrderIndex.Value);
				string selectedDeliveryCompanyId = hfSelectedDeliveryCompanyId.Value;
				shippingNo = hfSelectedShopShippingNo.Value;

				if (newCart[selectedOrderIndex].IsFixedPurchase)
				{
					dvShippingFixedPurchase.Visible = true;
					RefreshFixedPurchaseViewFromShippingType(Cart, this.shopShippings);
				}
				else
				{
					dvShippingFixedPurchase.Visible = false;
				}
				dvPayment.Visible = true;
				//RefreshPaymentViewFromShippingType(Cart, this.shopShippings);
				RefreshViewByOrderId(selectedOrderIndex);
				BindShippingRepeater(selectedOrderIndex);
				BindDeliveryName(selectedShippingBaseId);
				//update
				BindProductList(0, selectedOrderIndex);
				RefreshAndUpdateSessionData();
			}
			if (postBackControlID.Contains("rShippingListForTable"))
			{
				setOrderList();
				if (String.IsNullOrEmpty(hfSelectedOrderIndex.Value))
				{
					return;
				}
				selectedOrderIndex = int.Parse(hfSelectedOrderIndex.Value);
				int selectedShopShippingIndex = 0;
				if (!string.IsNullOrEmpty(hfSelectedShopShippingIndex.Value))
				{
					selectedShopShippingIndex = int.Parse(hfSelectedShopShippingIndex.Value);
				}
				BindProductList(selectedShopShippingIndex, selectedOrderIndex);
			}
		}
	}

	public void setOrderList()
	{
		newCart = (List<CartObject>)this.OrderParams["new_cart"];
		rOrderListForTable.DataSource = newCart;
		rOrderListForTable.DataBind();
	}

	/// <summary>
	/// 配送タイプにより配送拠点ID取得
	/// </summary>
	/// <param name="cart">カード情報</param>
	/// <returns>配送拠点ID</returns>
	public string GetShippingBaseId(String orderId)
	{
		var newOrderBaseList = (Dictionary<String, String>)this.OrderParams["orderIdByShippingBase"];

		string shippingBaseId = string.Empty;

		foreach (var orderBase in newOrderBaseList)
		{
			if (orderBase.Value == orderId)
			{
				shippingBaseId = orderBase.Key;
			}
		}

		// 配送種別設定取得
		var shippingBaseSettingManager = ShippingBaseSettingsManager.GetInstance();
		var shippingBaseName = shippingBaseSettingManager.Settings
			.Where(sba => sba.Id == shippingBaseId)
			.Select(sba => sba.Name)
			.FirstOrDefault();
		return shippingBaseName;
	}


	/// <summary>カート</summary>
	protected CartObject Cart
	{
		get { return (CartObject)this.OrderParams["cart"]; }
	}

	private void RefreshView()
	{
		var orderInput = (Hashtable)this.OrderParams["order_input"];
		var user = (Hashtable)this.OrderParams["user_input"];
		var cart = (CartObject)this.OrderParams["cart"];
		string selectedId = selectedShippingNames;
		hfShippingType.Value = selectedId;

		setOrderList();

		Session.Remove(Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT);

		BindDeliveryName(selectedShippingBaseId);

		BindShippingRepeater(0);
		// 注文者情報表示
		lOwnerName.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.Name);
		lOwnerNameKana.Text = GetEncodedHtmlDisplayMessage(cart.Owner.NameKana);
		lOwnerKbn.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_ORDEROWNER,
				Constants.FIELD_ORDEROWNER_OWNER_KBN,
				newCart[0].Owner.OwnerKbn));
		lOwnerTel1.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.Tel1);
		lOwnerTel2.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.Tel2);
		lOwnerMailAddr.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.MailAddr);
		lOwnerMailAddr2.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.MailAddr2);
		lOwnerZip.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.Zip);
		lOwnerAddr1.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.Addr1);
		lOwnerAddr2.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.Addr2);
		lOwnerAddr3.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.Addr3);
		lOwnerAddr4.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.Addr4);
		lOwnerCompanyName.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.CompanyName);
		lOwnerCompanyPostName.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.CompanyPostName);
		lOwnerSex.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_ORDEROWNER,
				Constants.FIELD_ORDEROWNER_OWNER_SEX,
				newCart[0].Owner.Sex));
		lOwnerBirth.Text = GetEncodedHtmlDisplayMessage(
			DateTimeUtility.ToStringForManager(
				newCart[0].Owner.Birth,
				DateTimeUtility.FormatType.LongDate1LetterNoneServerTime));

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			lOwnerAccessCountryIsoCode.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.AccessCountryIsoCode);
			lOwnerDispLanguageCode.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.DispLanguageCode);
			lOwnerDispLanguageLocaleId.Text = GetEncodedHtmlDisplayMessage(
				GlobalConfigUtil.LanguageLocaleIdDisplayFormat(newCart[0].Owner.DispLanguageLocaleId));
			lOwnerDispCurrencyCode.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.DispCurrencyCode);
			lOwnerDispCurrencyLocaleId.Text = GetEncodedHtmlDisplayMessage(
				GlobalConfigUtil.CurrencyLocaleIdDisplayFormat(newCart[0].Owner.DispCurrencyLocaleId));

			lOwnerCountryName.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.AddrCountryName);
			lOwnerAddr5.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.Addr5);
			lOwnerZipGlobal.Text = GetEncodedHtmlDisplayMessage(newCart[0].Owner.Zip);
		}

		BindProductList(0, 0);

		// 注文メモ
		rOrderMemos.DataSource = GetOrderMemoSetting(Constants.FLG_ORDER_MEMO_SETTING_DISP_FLG_PC);
		rOrderMemos.DataBind();
		rSetPromotion.DataSource = newCart[0].SetPromotions.Items;
		rSetPromotion.DataBind();

		// カード会社
		if (OrderCommon.CreditCompanySelectable)
		{
			ddlCreditCardCompany.Items.AddRange(ValueText.GetValueItemArray(
				Constants.TABLE_ORDER,
				OrderCommon.CreditCompanyValueTextFieldName));
		}

		// 楽天連携かつPayTg非利用の場合、新規クレカ入力領域を非表示にする
		this.phCreditCardNotRakuten.Visible = (this.IsNotRakutenAgency || (this.IsUserPayTg && Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten));
		tdCreditNumber.Visible = (this.IsUserPayTg == false);

		var combinedCartList = new CartObjectList(newCart[0].CartUserId, newCart[0].OrderKbn, false);

		// クーポン
		var hasCoupon = ((newCart[0].Coupon != null) && (string.IsNullOrEmpty(newCart[0].Coupon.CouponCode) == false));
		lCouponCode.Text = GetEncodedHtmlDisplayMessage(
			hasCoupon
				? newCart[0].Coupon.CouponCode
				: ReplaceTag("@@DispText.common_message.unspecified@@"));
		dvCouponDetail.Visible = hasCoupon;
		if (hasCoupon)
		{
			lCouponDiscount.Text = GetEncodedHtmlDisplayMessage(CouponOptionUtility.GetCouponDiscountString(newCart[0].Coupon));
			lCouponName.Text = GetEncodedHtmlDisplayMessage(newCart[0].Coupon.CouponName);
			lCouponDispName.Text = GetEncodedHtmlDisplayMessage(newCart[0].Coupon.CouponDispName);
		}

		// ポイント
		lOrderPointUse.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToNumeric(newCart[0].UsePoint));
		lOrderPointAdd.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToNumeric(newCart[0].BuyPoint + newCart[0].FirstBuyPoint));
		var userPoint = PointOptionUtility.GetUserPoint(newCart[0].OrderUserId);
		var userPointUsable = (userPoint != null) ? userPoint.PointUsable : 0;
		lUserPointUsable.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToNumeric(userPointUsable));
		dvUseAllPointFlg.Visible = newCart[0].UseAllPointFlg;


		// 金額系セット
		lOrderPriceSubTotal.Text = GetEncodedHtmlDisplayMessage(newCart[0].PriceSubtotal.ToPriceString(true));
		lOrderPriceShipping.Text = GetEncodedHtmlDisplayMessage(newCart[0].PriceShipping.ToPriceString(true));

		lOrderPriceRegulation.Text = GetEncodedHtmlDisplayMessage(newCart[0].PriceRegulation.ToPriceString(true));
		AddAttributesForControlDisplay(
			trPriceRegulation,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(newCart[0].PriceRegulation < 0));

		lMemberRankDiscount.Text = GetEncodedHtmlDisplayMessage(
			(newCart[0].MemberRankDiscount * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trMemberRankDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(newCart[0].MemberRankDiscount > 0));

		lFixedPurchaseMemberDiscount.Text = GetEncodedHtmlDisplayMessage(
			(newCart[0].FixedPurchaseMemberDiscountAmount * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trFixedPurchaseMemberDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(newCart[0].FixedPurchaseMemberDiscountAmount > 0));

		lPointUsePrice.Text = GetEncodedHtmlDisplayMessage(
			(newCart[0].UsePointPrice * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trPointDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(newCart[0].UsePointPrice > 0));

		lCouponUsePrice.Text = GetEncodedHtmlDisplayMessage(
			(newCart[0].UseCouponPrice * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trCouponDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(newCart[0].UseCouponPrice > 0));

		lFixedPurchaseDiscountPrice.Text = GetEncodedHtmlDisplayMessage(newCart[0].FixedPurchaseDiscount.ToPriceString(true));
		AddAttributesForControlDisplay(
			trFixedPurchaseDiscountPrice,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(newCart[0].FixedPurchaseDiscount > 0));

		lOrderPriceTax.Text = GetEncodedHtmlDisplayMessage(newCart[0].PriceSubtotalTax.ToPriceString(true));
		lOrderPriceTotalBottom.Text
			= lOrderPriceTotal.Text
				= GetEncodedHtmlDisplayMessage(newCart[0].PriceTotal.ToPriceString(true));

		rTotalPriceByTaxRate.DataSource = newCart[0].PriceInfoByTaxRate;
		rTotalPriceByTaxRate.DataBind();

		// クレジット情報入力域を表示
		dvPaymentKbnCredit.Visible = (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);

		// 領収書情報セット
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			lReceiptFlg.Text = GetEncodedHtmlDisplayMessage(
				ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_RECEIPT_FLG,
					newCart[0].ReceiptFlg));
			lReceiptAddress.Text = GetEncodedHtmlDisplayMessage(newCart[0].ReceiptAddress);
			lReceiptProviso.Text = GetEncodedHtmlDisplayMessage(newCart[0].ReceiptProviso);
		}

		// コンバージョン情報
		lInflowContentsType.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_ORDER,
				Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE,
				orderInput[Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE]));
		lInflowContentsId.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_INFLOW_CONTENTS_ID]));

		// 配送情報
		ddlShippingMethod.Items.AddRange(ValueText.GetValueItemArray(
			Constants.TABLE_ORDERSHIPPING,
			Constants.FIELD_ORDERSHIPPING_SHIPPING_METHOD));

		// 配送日時
		//update
		dvShippingDate.Visible = true;
		dvShippingTime.Visible = false;

		var zeroPriceHtmlEncoded = GetEncodedHtmlDisplayMessage(0.ToPriceString(true));
		lOrderPriceExchange.Text = zeroPriceHtmlEncoded;

		// カード有効期限(月)
		ddlCreditExpireMonth.Items.AddRange(this.CreditExpirationMonthListItems);
		ddlCreditExpireMonth.SelectedValue = DateTime.Now.Month.ToString("00");

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
		// カード分割支払い
		dvInstallments.Visible = OrderCommon.CreditInstallmentsSelectable;
		if (OrderCommon.CreditInstallmentsSelectable)
		{
			dllCreditInstallments.Items.AddRange(ValueText.GetValueItemArray(
				Constants.TABLE_ORDER,
				OrderCommon.CreditInstallmentsValueTextFieldName));
		}

		// カード有効期限(年)
		ddlCreditExpireYear.Items.AddRange(this.CreditExpirationYearListItems);
		ddlCreditExpireYear.SelectedValue = DateTime.Now.Year.ToString("00").Substring(2);

		GetGmoCvsType();

		// Get rakuten cvs type
		GetRakutenCvsType();

		tdCreditNumber.Visible = (this.IsUserPayTg == false);
		trCreditExpire.Visible = (this.IsUserPayTg == false);
		tdGetCardInfo.Visible = this.IsUserPayTg;
	}

	/// <summary>注文同梱対応 親注文定期購入有無</summary>
	protected bool CombineParentOrderHasFixedPurchase
	{
		get { return (bool)(ViewState["CombineParentOrderHasFixedPurchase"] ?? false); }
		set { ViewState["CombineParentOrderHasFixedPurchase"] = value; }
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
	/// 配送先ごとのセットプロモーション情報を取得
	/// </summary>
	/// <param name="shippingNo">配送先枝番（１から数える）</param>
	/// <returns>セットプロモーション情報</returns>
	protected CartSetPromotion[] GetOrderSetPromotionsByShipping(string shippingNo)
	{
		if ((this.Cart.SetPromotions == null) || (this.Cart.SetPromotions.Items.Any() == false))
		{
			return Array.Empty<CartSetPromotion>();
		}

		var result = this.Cart.SetPromotions.Items.Where(spItem => (spItem.ShippingNo == int.Parse(shippingNo)))
			.ToArray();
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

	public void BindDeliveryName(String selectedShippingBaseId)
	{
		if (selectedShippingBaseId != null)
		{
			dvShowDeliveryDesignation.Visible = true;
			dvHideDeliveryDesignation.Visible = false;
		}

		shopShippings = DomainFacade.Instance.ShopShippingService.GetShopShippingByShippingBaseId(this.LoginOperatorShopId, selectedShippingBaseId);
		ddlDeliveryCompany.Items.Clear();

		var companyList = new List<ShopShippingCompanyModel>();

		foreach (var shopShipping in shopShippings)
		{
			var companies = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
				? shopShipping.CompanyListExpress
				: shopShipping.CompanyListMail;

			if (companies != null)
			{
				companyList.AddRange(companies);
			}
		}
		var distinctCompanyList = companyList.GroupBy(c => c.DeliveryCompanyId)
											 .Select(g => g.First())
											 .ToList();

		var deliveryCompanyList = this.DeliveryCompanyList
			.Where(company => distinctCompanyList.Any(c => company.DeliveryCompanyId == c.DeliveryCompanyId))
			.Select(company => new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId))
			.ToArray();

		ddlDeliveryCompany.Items.AddRange(deliveryCompanyList);
	}

	public void BindProductList(int selectedShopShippingIndex, int selectedOrderIndex)
	{
		if (selectedShopShippingIndex != null)
		{
			if (newCart != null && selectedOrderIndex >= 0 && selectedOrderIndex <= newCart.Count && selectedShopShippingIndex <= newCart[selectedOrderIndex].Shippings.Count)
			{
				rShippingList.DataSource = new List<CartShipping> { newCart[selectedOrderIndex].Shippings[selectedShopShippingIndex] };
				rShippingList.DataBind();
				rItemList.DataSource = this.newCart[selectedOrderIndex].Shippings[selectedShopShippingIndex].ProductCounts;
				rItemList.DataBind();
			}
			else
			{
				rShippingList.DataSource = null;
				rShippingList.DataBind();
				rItemList.DataSource = null;
				rItemList.DataBind();
			}
		}
		else
		{
			rShippingList.DataSource = new List<CartShipping> { newCart[selectedOrderIndex].Shippings[0] };
			rShippingList.DataBind();
			rItemList.DataSource = this.newCart[selectedOrderIndex].Shippings[selectedShopShippingIndex].ProductCounts;
			rItemList.DataBind();
		}
	}

	/// <summary>
	/// 配送会社を更新する（配送種別から）
	/// </summary>
	/// <param name="shopShipping">配送種別情報</param>
	private void RefreshShippingCompanyViewFromShippingType(ShopShippingModel[] shopShippings)
	{
		var cartShipping = Cart.GetShipping();
		ddlDeliveryCompany.Items.Clear();

		var companyList = new List<ShopShippingCompanyModel>();

		foreach (var shopShipping in shopShippings)
		{
			var companies = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
				? shopShipping.CompanyListExpress
				: shopShipping.CompanyListMail;

			if (companies != null)
			{
				companyList.AddRange(companies);
			}
		}

		var distinctCompanyList = companyList.GroupBy(c => c.DeliveryCompanyId)
										 .Select(g => g.First())
										 .ToList();

		var deliveryCompanyList = this.DeliveryCompanyList
			.Where(company => distinctCompanyList.Any(c => company.DeliveryCompanyId == c.DeliveryCompanyId))
			.Select(company => new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId));

		ddlDeliveryCompany.Items.AddRange(deliveryCompanyList.ToArray());

		tbShippingCheckNo.Text = cartShipping.ShippingCheckNo;
	}

	/// <summary>
	/// 配送方法自動判定ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSetShippingMethod_Click(object sender, EventArgs e)
	{
		RefreshItemsAndPriceView();

		if (string.IsNullOrEmpty(ddlShippingDate.SelectedValue)
			&& string.IsNullOrEmpty(ddlShippingTime.SelectedValue))
		{
			if (this.shopShippings == null) return;

			var companyList = new List<ShopShippingCompanyModel>();
			foreach (var shopShipping in shopShippings)
			{
				var companies = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
					? shopShipping.CompanyListExpress
					: shopShipping.CompanyListMail;

				if (companies != null)
				{
					companyList.AddRange(companies);
				}
			}
			ddlDeliveryCompany.Items.Clear();

			if (companyList.Count > 0 && companyList != null)
			{
				foreach (var item in companyList)
				{
					var company = this.DeliveryCompanyList
						.First(itemCompany => (itemCompany.DeliveryCompanyId == item.DeliveryCompanyId));
					ddlDeliveryCompany.Items.Add(new ListItem(company.DeliveryCompanyName, company.DeliveryCompanyId));
				}
			}
			ddlShippingMethod_SelectedIndexChanged(sender, e);
		}
	}

	/// <summary>カート内商品サイズ係数（メール便配送サービスエスレーション機能用）</summary>
	protected int ProductsSizeForDeliveryCompanyMailEscalation { get; set; }

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
		// 配送種別取得
		if (this.shopShippings.Length <= 0 && this.shopShippings == null)
		{
			ddlDeliveryCompany.Items.Clear();
			return;
		}

		//RefreshFixedPurchaseViewFromShippingType(Cart, this.shopShippings);

		// 配送指定更新
		RefreshDeliverySpecificationFromShippingType(Cart, this.shopShippings, true);

		//// 再計算
		RefreshItemsAndPriceView();

		// 決済情報更新
		//RefreshPaymentViewFromShippingType(Cart, this.shopShippings);
	}

	/// <summary>
	/// 商品・金額表示更新
	/// </summary>
	/// <param name="reOrderItems">元注文の注文商品情報（再注文使用の場合）</param>
	/// <param name="isOrderCombined">注文同梱有無</param>
	private void RefreshItemsAndPriceView(List<Hashtable> reOrderItems = null, bool isOrderCombined = false)
	{
		var shippingId = shopShippings
						.Where(m => m.ShippingBaseId == selectedShippingBaseId).Select(m => m.ShippingId)
						.FirstOrDefault();
		ShopShippingModel shopShipping = shopShippings.Where(m => m.ShippingId == shippingId).FirstOrDefault();

		if (this.shopShippings != null)
		{
			SetFirstAndNextShippingDate(Cart, shopShipping);

			if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
			{
				//RefreshShippingCompanyViewFromShippingType(this.shopShippings);

				if (this.IsBackFromConfirm == false)
				{
					RefreshShippingDateViewFromShippingType(
						shopShippings,
						Cart,
						false,
						false);
				}
			}
			else
			{
				if (ddlShippingDate.Visible
					&& ddlFirstShippingDate.Visible
					&& ddlShippingDate.Items[0].Text == lFirstShippingDate.Text)
				{
					ddlShippingDate.Items.RemoveAt(0);
				}
			}

			var itemErrorMessages = new StringBuilder();
			// 配送希望日時＆定期購入配送パターンドロップダウンセット
			var shippingTypeAlertMessages = RefreshViewFromShippingType(Cart);
			if (string.IsNullOrEmpty(shippingTypeAlertMessages) == false)
			{
				if (itemErrorMessages.Length != 0) itemErrorMessages.Append("<br />");
				itemErrorMessages.Append(shippingTypeAlertMessages);
			}
		}

		// Get gmo cvs type
		GetGmoCvsType();

		//// Get rakuten cvs type
		GetRakutenCvsType();
	}

	/// <summary>
	/// First shipping date drop down selected index changed event
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlFirstShippingDate_SelectedIndexChanged(object sender, EventArgs e)
	{
		Cart.GetShipping().FirstShippingDate = DateTime.Parse(ddlFirstShippingDate.SelectedValue);
		var shippingId = shopShippings
						.Where(m => m.ShippingId == ddlDeliveryCompany.SelectedValue).Select(m => m.ShippingId)
						.FirstOrDefault();
		ShopShippingModel shopShipping = shopShippings.Where(m => m.ShippingId == shippingId).FirstOrDefault();

		SetFirstAndNextShippingDate(Cart, shopShipping);
		RefreshAndUpdateSessionData();
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

		RefreshAndUpdateSessionData();
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

		//// If the payment update does not set next shipping date
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

	private bool IsUpdatePayment { get; set; }


	/// <summary>
	/// 配送種別に紐付く表示の更新
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <rereturns>アラートメッセージ</rereturns>
	private string RefreshViewFromShippingType(CartObject cart)
	{
		var alertMessages = new StringBuilder();

		// 配送種別または定期購入有無が更新されたら配送指定情報更新(注文同梱の場合は例外)
		var shippingTypeChanged = (hfShippingType.Value != this.selectedShippingNames);
		var isChangedToRegularPurchase = bool.Parse(hfHasFixedPurchase.Value)
			|| ((dvShippingFixedPurchase.Visible == false)
				&& cart.HasFixedPurchase
				&& (this.CombineParentOrderHasFixedPurchase == false)
				&& (this.IsOrderCombinedAtOrderCombinePage == false));

		if (shippingTypeChanged)
		{
			// 定期購入有りに変更された？
			if (isChangedToRegularPurchase && cart.HasFixedPurchase)
			{
				alertMessages.Append(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERREGIST_FIXED_PURCHASE_CHANGED));
			}

			if (this.shopShippings == null)
			{
				ddlDeliveryCompany.Items.Clear();
				return string.Empty;
			}

			// 決済情報更新
			RefreshPaymentViewFromShippingType(cart, this.shopShippings);

		}

		if (this.IsOrderCombinedAtOrderRegistPage)
		{
			AddAttributesForControlDisplay(
				dvShippingInfo,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE,
				CONST_KEY_HTMLCONTROL_ATTRIBUTE_STYLE_DISPLAY_NONE,
				true);
		}
		return alertMessages.ToString();
	}

	/// <summary>Is Order Combined At Order Regist Page</summary>
	public bool IsOrderCombinedAtOrderRegistPage
	{
		get { return (this.IsOrderCombined && (this.CombineChildOrderIds == null)); }
	}

	/// <summary>
	/// 配送指定を更新（配送会社、配送指定日、配送指定時間帯）
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="isChangedShippingService">配送サービスの変更が行われたか</param>
	private void RefreshDeliverySpecificationFromShippingType(
		CartObject cart,
		ShopShippingModel[] shopShippings,
		bool isChangedShippingService = false)
	{
		// 配送会社情報更新
		RefreshShippingCompanyViewFromShippingType(shopShippings);

		Boolean expressOrMailFlag = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
									 ? true : false;

		// 配送指定日更新
		RefreshShippingDateViewFromShippingType(shopShippings, cart, isChangedShippingService);

		var deliveryCompanyId = ddlDeliveryCompany.SelectedValue;
		var deliveryCompany = this.DeliveryCompanyList
					.FirstOrDefault(item => (item.DeliveryCompanyId == deliveryCompanyId));
		if (deliveryCompany != null)
		{
			// 配送指定時間帯更新
			RefreshShippingTimeViewFromShippingType(deliveryCompany, expressOrMailFlag, isChangedShippingService);
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

	/// <summary>確認画面から戻ってきたか</summary>
	protected bool IsBackFromConfirm
	{
		get
		{
			return ((Session[Constants.SESSION_KEY_PARAM_FOR_BACK] != null)
				&& (Session[Constants.SESSION_KEY_PARAM_FOR_BACK] is Hashtable));
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
		ShopShippingModel[] shopShippings,
		CartObject cart,
		bool isChangedShippingService = false,
		bool isChangeProductRelateToProductNotRelate = false)
	{

		//update
		var shippingId = shopShippings
						.Where(m => m.ShippingBaseId == selectedShippingBaseId).Select(m => m.ShippingId)
						.FirstOrDefault();
		ShopShippingModel shopShipping = shopShippings.Where(m => m.ShippingId == shippingId).FirstOrDefault();

		bool isExpressDelivery = false;
		if (shopShipping != null)
		{
			if (shopShipping.CompanyListExpress != null)
			{
				isExpressDelivery = shopShipping.CompanyListExpress.Count() > 0 ? true : false;
			}
			RefreshShippingDateViewFromShippingType(
			shopShipping,
			isExpressDelivery,
			cart.GetShipping().ConvenienceStoreFlg,
			shopShipping.FixedPurchaseFlg,
			isChangedShippingService,
			isChangeProductRelateToProductNotRelate);
		}
	}

	/// <summary>注文同梱 親注文定期購入回数</summary>
	protected int CombineParentOrderCount
	{
		get { return (int)ViewState["CombineParentOrderCount"]; }
		set { ViewState["CombineParentOrderCount"] = value; }
	}

	/// <summary>セールID変更有無</summary>
	private bool ChangeSaleId { get; set; }

	/// <summary>
	/// 配送会社選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDeliveryCompany_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (this.shopShippings == null)
		{
			ddlDeliveryCompany.Items.Clear();
			return;
		}

		var selectedValue = ddlDeliveryCompany.SelectedValue;
		hfSelectedDeliveryCompanyId.Value = selectedValue;

		ShopShippingModel shopShipping = shopShippings.FirstOrDefault(x => x.ShopId == shippingNo);

		//// 配送指定日更新
		RefreshShippingDateViewFromShippingType(shopShippings, Cart, true);

		Boolean expressOrMailFlag = (ddlShippingMethod.SelectedValue == Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
									 ? true : false;

		var deliveryCompany = this.DeliveryCompanyList
			.First(item => (item.DeliveryCompanyId == ddlDeliveryCompany.SelectedValue));

		RefreshFixedPurchaseViewFromShippingType(Cart, this.shopShippings);
		RefreshShippingTimeViewFromShippingType(deliveryCompany, expressOrMailFlag, true);
		RefreshAndUpdateDeliverySessionData();
		// 再計算
		RefreshItemsAndPriceView();
	}

	private DateTime? GetShippingDate()
	{
		var shippingDate = (string.IsNullOrEmpty(ddlShippingDate.SelectedValue) == false)
				? DateTime.Parse(ddlShippingDate.SelectedValue)
				: (DateTime?)null;
		return shippingDate;
	}

	/// <summary>
	/// 配送希望日チェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlShippingDate_SelectedIndexChanged(object sender, EventArgs e)
	{
		var cartShipping = Cart.GetShipping();
		cartShipping.ShippingDate = GetShippingDate();
		cartShipping.ShippingMethod = ddlShippingMethod.SelectedValue;
		cartShipping.DeliveryCompanyId = ddlDeliveryCompany.SelectedValue;

		//update
		var shippingId = shopShippings
						.Where(m => m.ShippingBaseId == selectedShippingBaseId).Select(m => m.ShippingId)
						.FirstOrDefault();
		ShopShippingModel shopShipping = shopShippings.Where(m => m.ShippingId == shippingId).FirstOrDefault();

		var shippingDateErrorMessage = string.Empty;
		if ((string.IsNullOrEmpty(ddlShippingDate.SelectedValue) == false)
			&& (OrderCommon.CanCalculateScheduledShippingDate(this.LoginOperatorShopId, cartShipping) == false))
		{
			shippingDateErrorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_SHIPPINGDATE_INVALID)
				.Replace("@@ 1 @@", DateTimeUtility.ToStringForManager(
					HolidayUtil.GetShortestDeliveryDate(
						Cart.ShopId,
						cartShipping.DeliveryCompanyId,
						shopShipping.ShippingBaseId,
						cartShipping.Zip.Replace("-", string.Empty)),
					DateTimeUtility.FormatType.LongDateWeekOfDay2Letter,
					Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE));
		}
		lShippingDateErrorMessages.Text = GetEncodedHtmlDisplayMessage(shippingDateErrorMessage);

		RefreshAndUpdateDeliveryTimeSessionData();
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
		//update
		lShippingDateErrorMessages.Text = string.Empty;
		if ((shopShipping.IsValidShippingDateSetFlg == false) || (isExpressDelivery == false))
		{
			ddlShippingDate.Items.Clear();
		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		Hashtable orderParams = (Hashtable)Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_REGIST_INPUT];
		Hashtable orderSplit = (Hashtable)Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT];

		var newOrderBaseList = (Dictionary<String, String>)this.OrderParams["orderIdByShippingBase"];

		var newOrderIdList = new List<string>();

		foreach (var orderBase in newOrderBaseList)
		{
			newOrderIdList.Add(orderBase.Value);
		}

		if (this.OrderSplitParam != null && this.OrderSplitParam["orderShipping_list"] != null)
		{
			int newCartIndex = 0;
			var orderShipping = (Dictionary<String, CartShipping>)this.OrderSplitParam["orderShipping_list"];
			if (orderShipping != null)
			{
				int orderCount = orderShipping.Count;
				int newOrderIdCount = newOrderIdList.Count;
				if (orderCount == newOrderIdCount)
				{
					foreach (var listItem in orderShipping)
					{
						if (newOrderIdList.Contains(listItem.Key))
						{
							CartShipping cart = listItem.Value;

							if (cart.DeliveryCompanyId == null)
							{
								var orderIdErrorMessage = listItem.Key + "配送サービス is required";
								dvOrderIdErrorMessages.Visible = true;
								IOrderIdErrorMessage.Text = GetEncodedHtmlDisplayMessage(orderIdErrorMessage);
								return;
							}
							else if (cart.FirstShippingDate == null)
							{
								var orderIdErrorMessage = listItem.Key + "初回配送予定日 is required";
								dvOrderIdErrorMessages.Visible = true;
								IOrderIdErrorMessage.Text = GetEncodedHtmlDisplayMessage(orderIdErrorMessage);
								return;
							}
							else if (cart.FixedPurchaseDaysRequired == null)
							{
								var orderIdErrorMessage = listItem.Key + "FixedPurchaseDaysRequired is required";
								dvOrderIdErrorMessages.Visible = true;
								IOrderIdErrorMessage.Text = GetEncodedHtmlDisplayMessage(orderIdErrorMessage);
								return;
							}
							else if (cart.ShippingMethod == null)
							{
								var orderIdErrorMessage = listItem.Key + "配送方法 is required";
								dvOrderIdErrorMessages.Visible = true;
								IOrderIdErrorMessage.Text = GetEncodedHtmlDisplayMessage(orderIdErrorMessage);
								return;
							}
							if (newCart[newCartIndex].IsFixedPurchase)
							{
								if (cart.CartObject.Payment.PaymentId == null)
								{
									var provisionalCreditcardPayment = DomainFacade.Instance.PaymentService.Get(
																	   this.LoginOperatorShopId,
																	   Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID);
									listItem.Value.CartObject.Payment.PaymentId = provisionalCreditcardPayment.PaymentId;
									listItem.Value.CartObject.Payment.PaymentName = provisionalCreditcardPayment.PaymentName;
								}

								if (cart.FixedPurchaseKbn == null)
								{
									var fixedPurchaseSettings = GetFixedPurchaseSettingInputs();
									var shippingFixedPurchaseErrorMessage = Validator.Validate("OrderShippingRegistInput", fixedPurchaseSettings);
									lFixedPurchasePatternErrorMessage.Text = GetEncodedHtmlDisplayMessage(shippingFixedPurchaseErrorMessage);
									dvFixedPurchasePatternErrorMessages.Visible = (shippingFixedPurchaseErrorMessage.Length != 0);
									dvShippingFixedPurchase.Visible = true;
									return;
								}
								else
								{
									if (cart.FixedPurchaseKbn == "01")
									{
										if (cart.FixedPurchaseSetting == null)
										{
											var fixedPurchaseSettings = GetFixedPurchaseSettingInputs();
											var shippingFixedPurchaseErrorMessage = Validator.Validate("OrderShippingRegistInput", fixedPurchaseSettings);
											lFixedPurchasePatternErrorMessage.Text = GetEncodedHtmlDisplayMessage(shippingFixedPurchaseErrorMessage);
											dvFixedPurchasePatternErrorMessages.Visible = (shippingFixedPurchaseErrorMessage.Length != 0);
											dvShippingFixedPurchase.Visible = true;
											return;
										}
									}
									else if (cart.FixedPurchaseKbn == "02")
									{
										if (cart.FixedPurchaseSetting == null)
										{
											var fixedPurchaseSettings = GetFixedPurchaseSettingInputs();
											var shippingFixedPurchaseErrorMessage = Validator.Validate("OrderShippingRegistInput", fixedPurchaseSettings);
											lFixedPurchasePatternErrorMessage.Text = GetEncodedHtmlDisplayMessage(shippingFixedPurchaseErrorMessage);
											dvFixedPurchasePatternErrorMessages.Visible = (shippingFixedPurchaseErrorMessage.Length != 0);
											dvShippingFixedPurchase.Visible = true;
											return;
										}
									}
									else if (cart.FixedPurchaseKbn == "03")
									{
										if (cart.FixedPurchaseSetting == null)
										{
											var fixedPurchaseSettings = GetFixedPurchaseSettingInputs();
											var shippingFixedPurchaseErrorMessage = Validator.Validate("OrderShippingRegistInput", fixedPurchaseSettings);
											lFixedPurchasePatternErrorMessage.Text = GetEncodedHtmlDisplayMessage(shippingFixedPurchaseErrorMessage);
											dvFixedPurchasePatternErrorMessages.Visible = (shippingFixedPurchaseErrorMessage.Length != 0);
											dvShippingFixedPurchase.Visible = true;
											return;
										}
									}
								}
							}
							else
							{
								if (cart.CartObject.Payment.PaymentId == null)
								{
									RefreshPaymentViewFromShippingType(Cart, this.shopShippings);
									listItem.Value.CartObject.Payment.PaymentId = ddlOrderPaymentKbn.SelectedValue;
									listItem.Value.CartObject.Payment.PaymentName = ddlOrderPaymentKbn.SelectedItem.ToString();
								}
								cart.NextNextShippingDate = DateTime.Now;
								cart.NextShippingDate = DateTime.Now;
								cart.FixedPurchaseKbn = "";
								cart.FixedPurchaseSetting = "";

							}
						}
						newCartIndex++;
					}
				}
				else
				{
					var orderIdErrorMessage = "受注IDには次のものが含まれていません";
					dvOrderIdErrorMessages.Visible = true;
					IOrderIdErrorMessage.Text = GetEncodedHtmlDisplayMessage(orderIdErrorMessage);
					return;
				}
			}
		}
		else
		{
			var orderIdErrorMessage = "受注IDには次のものが含まれていません";
			dvOrderIdErrorMessages.Visible = true;
			IOrderIdErrorMessage.Text = GetEncodedHtmlDisplayMessage(orderIdErrorMessage);
			return;
		}
		Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_REGIST_INPUT] = orderParams;
		Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] = orderSplit;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_REGIST_CONFIRM);
	}

	/// <summary>
	/// カート決済情報取得
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <returns>更新後のカート決済情報</returns>
	private CartPayment GetCartPayment(CartObject cart)
	{
		var orderInput = (Hashtable)this.OrderParams["order_input"];
		var userId = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_USER_ID]);
		var orderCreditCardInput = GetOrderCreditCardInputForOrderPage(
			ddlOrderPaymentKbn.SelectedValue,
			userId);
		var cartPayment = CreateCartPayment(cart, orderCreditCardInput);
		cartPayment.PriceExchange = OrderCommon.GetPaymentPrice(
			cart.ShopId,
			cartPayment.PaymentId,
			cart.PriceSubtotal,
			cart.PriceCartTotalWithoutPaymentPrice);

		return cartPayment;
	}

	/// <summary>
	/// カート決済情報作成
	/// </summary>
	/// <param name="cart">カート</param>
	/// <param name="orderCreditCardInput">注文クレジットカード入力</param>
	/// <returns>カート決済情報</returns>
	private CartPayment CreateCartPayment(CartObject cart, OrderCreditCardInput orderCreditCardInput)
	{
		var orderInput = (Hashtable)this.OrderParams["order_input"];
		var userId = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_USER_ID]);
		var payment = new CartPayment();
		if (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			// 仮クレジットカードの場合は置き換え（カード入力できない場合は仮クレジットカード利用と判定）
			var orderPaymentKbnValue = ddlOrderPaymentKbn.SelectedItem.Value;
			var orderPaymentKbnName = ddlOrderPaymentKbn.SelectedItem.Text;
			//update
			//this.CanUseCreditCardNoForm == false
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
					? UserCreditCard.Get(userId, int.Parse(ddlUserCreditCard.SelectedValue))
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
				&& (string.IsNullOrEmpty(userId) == false))
			{
				this.AccountEmail = PayPalUtility.Account.GetCooperateAccountEmail(userId);
				if (string.IsNullOrEmpty(this.AccountEmail) == false)
				{
					var userExtend = DomainFacade.Instance.UserService.GetUserExtend(userId);
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

	/// <summary>
	/// 月間隔日付指定選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbMonthlyPurchase_Date_CheckedChanged(object sender, EventArgs e)
	{
		ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged(sender, e);
		MonthlyPurchaseWeekAndDayChanged();
	}
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

		MonthlyPurchaseWeekAndDayChanged();

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
	/// 週間隔・曜日指定
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbPurchase_EveryNWeek_CheckedChanged(object sender, EventArgs e)
	{
		ddlFixedPurchaseEveryNWeek_OnSelectedIndexChanged(sender, e);
		ddlFirstShippingDate.Visible = false;
		MonthlyPurchaseWeekAndDayChanged();
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

		var selectedvalue = ddlFixedPurchaseEveryNWeek_Week.SelectedValue;

		// 選択している配送間隔日チェック、アラートメッセージのセット
		CheckIntervalValueAndSetAlertMessage(
			ddlFixedPurchaseEveryNWeek_Week.SelectedValue,
			Constants.FIELD_PRODUCT_LIMITED_FIXED_PURCHASE_KBN4_SETTING);

		MonthlyPurchaseWeekAndDayChanged();

	}

	/// <summary>
	/// 定期購入情報を更新する（配送種別から）
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	private void RefreshFixedPurchaseViewFromShippingType(CartObject cart, ShopShippingModel[] shopShippings)
	{
		var shippingId = shopShippings
						.Where(m => m.ShippingBaseId == selectedShippingBaseId).Select(m => m.ShippingId)
						.FirstOrDefault();
		ShopShippingModel shopShipping = shopShippings.Where(m => m.ShippingId == shippingId).FirstOrDefault();

		if (shopShipping != null)
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
		if (dvShippingFixedPurchase.Visible)
		{
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
	/// 週・曜日指定選択イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbMonthlyPurchase_WeekAndDay_CheckedChanged(object sender, EventArgs e)
	{
		ddlMonth_ddlIntervalMonths_OnSelectedIndexChanged(sender, e);
	}

	protected void rbRegularPurchase_IntervalDays_CheckedChanged(object sender, EventArgs e)
	{
		ddlIntervalDays_OnSelectedIndexChanged(sender, e);
		ddlFirstShippingDate.Visible = false;
		MonthlyPurchaseWeekAndDayChanged();
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
		MonthlyPurchaseWeekAndDayChanged();
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
	/// 選択している配送間隔月・日が規定の値かどうかのチェック
	/// かつアラートメッセージのセット
	/// </summary>
	/// <param name="value">選択中の値</param>
	/// <param name="fieldName">チェックする項目名</param>
	private void CheckIntervalValueAndSetAlertMessage(string value, string fieldName)
	{
		this.IsUpdateFixedPurchaseShippingPattern = true;
		RefreshItemsAndPriceView();

		dvFixedPurchasePatternErrorMessages.Visible = (lFixedPurchasePatternErrorMessage.Text.Length > 0);
	}

	/// <summary>Is Update Fixed Purchase Shipping Pattern</summary>
	private bool IsUpdateFixedPurchaseShippingPattern
	{
		get { return (bool)(ViewState["IsUpdateFixedPurchaseShippingPattern"] ?? false); }
		set { ViewState["IsUpdateFixedPurchaseShippingPattern"] = value; }
	}

	/// <summary>注文同梱ページでの注文同梱有無</summary>
	protected bool IsOrderCombinedAtOrderCombinePage
	{
		get { return (this.IsOrderCombined && (this.CombineChildOrderIds != null)); }
	}

	/// <summary>注文同梱対象 子注文IDs</summary>
	protected string[] CombineChildOrderIds
	{
		get { return (string[])ViewState["CombineChildOrderIds"]; }
		set { ViewState["CombineChildOrderIds"] = value; }
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
	/// 注文メモ設定を取得する
	/// </summary>
	/// <param name="displayKbn">表示区分</param>
	/// <returns>注文メモ</returns>
	public DataView GetOrderMemoSetting(string displayKbn)
	{
		return DomainFacade.Instance.OrderMemoSettingService.GetOrderMemoSettingInDataView(displayKbn);
	}

	private void BindShippingRepeater(int orderIndex)
	{
		//var shippings = (shippingNo == null || !shippingNo.Any()) ? Cart.Shippings : Cart.Shippings.Where(s => shippingNo.Contains(s.ShippingId)).ToList();
		rShippingListForTable.DataSource = newCart[orderIndex].Shippings;
		rShippingListForTable.DataBind();
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
		Session.Remove(Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT);
		Session[Constants.SESSION_KEY_PARAM_FOR_BACK] = this.OrderParams;
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ORDER_REGIST_INPUT)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, this.ActionStatus)
			.CreateUrl();
		Response.Redirect(url);
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

	protected Hashtable OrderSplitParam
	{
		get { return (Hashtable)Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT]; }
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

	/// <summary>カードリスト</summary>
	protected UserCreditCard[] CreditCardList
	{
		get { return (UserCreditCard[])(ViewState["CreditCardList"] ?? new UserCreditCard[0]); }
		private set { ViewState["CreditCardList"] = value; }
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
	protected bool IsShippingAddrJp
	{
		get { return IsCountryJp(this.ShippingAddrCountryIsoCode); }
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

	/// <summary>商品が全て頒布会定額コース商品か</summary>
	protected bool IsAllItemsSubscriptionBoxFixedAmount
	{
		get { return ((CartObject)this.OrderParams["cart"]).IsAllItemsSubscriptionBoxFixedAmount; }
	}

	/// <summary>
	/// 決済情報表示を更新する（配送種別から）
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	private void RefreshPaymentViewFromShippingType(CartObject cart, ShopShippingModel[] shopShippings)
	{
		Boolean paymentFlag = false;
		String sameAsOwner = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER;
		var shippingId = shopShippings
						.Where(m => m.ShippingBaseId == selectedShippingBaseId).Select(m => m.ShippingId)
						.FirstOrDefault();
		ShopShippingModel shopShipping = shopShippings.Where(m => m.ShippingId == shippingId).FirstOrDefault();

		foreach (var ship in newCart[selectedOrderIndex].Shippings)
		{
			if (ship.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW)
			{
				paymentFlag = true;
				sameAsOwner = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW;
			}
		}

		var payments = GetPaymentValidListPermission(
			this.LoginOperatorShopId,
			shopShipping.PaymentSelectionFlg,
			shopShipping.PermittedPaymentIds,
			newCart[selectedOrderIndex].IsFixedPurchase,
			paymentFlag);
		payments = payments
			.Where(item => (OrderCommon.CheckPaymentYamatoKaSms(item.PaymentId) == false)
				&& (item.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
			.ToArray();
		var selectedPaymentKbn = ddlOrderPaymentKbn.SelectedValue;

		ddlOrderPaymentKbn.Items.Clear();
		foreach (var payment in payments)
		{
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
			if ((sameAsOwner != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
				&& (sameAsOwner != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW))
			{
				var userShipping = this.UserShippingAddress.FirstOrDefault(item => item.ShippingNo.ToString() == sameAsOwner);
				isUserShippingConvenienceStore = (userShipping != null)
					&& (userShipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);
			}

			if ((sameAsOwner != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
				&& (isUserShippingConvenienceStore == false)
				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE))
			{
				continue;
			}

			if ((sameAsOwner == Constants.SHIPPING_KBN_LIST_STORE_PICKUP)
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

	/// <summary>User Shipping Address</summary>
	protected UserShippingModel[] UserShippingAddress
	{
		get
		{
			return (UserShippingModel[])(ViewState["UserShippingAddress"] ?? new UserShippingModel[0]);
		}
		set { ViewState["UserShippingAddress"] = value; }
	}





	//private void RefreshPaymentViewFromShippingType(CartObject cart, ShopShippingModel shopShipping)
	//{
	//	var payments = GetPaymentValidListPermission(
	//		this.LoginOperatorShopId,
	//		shopShipping.PaymentSelectionFlg,
	//		shopShipping.PermittedPaymentIds,
	//		cart.HasFixedPurchase);
	//	payments = payments
	//		.Where(item => (OrderCommon.CheckPaymentYamatoKaSms(item.PaymentId) == false)
	//			&& (item.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU))
	//		.ToArray();
	//	var selectedPaymentKbn = ddlOrderPaymentKbn.SelectedValue;

	//	ddlOrderPaymentKbn.Items.Clear();
	//	foreach (var payment in payments)
	//	{
	//		if ((cart.IsExpressDelivery == false)
	//			&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)) continue;
	//		if (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS) continue;
	//		if (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT) continue;
	//		if (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2) continue;
	//		if ((this.IsOrderCombined == false)
	//			&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY)) continue;
	//		if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
	//			&& (this.IsOrderCombined == false)) continue;
	//		if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE)
	//			&& (this.IsOrderCombined == false)) continue;
	//		if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY)
	//			&& (cart.IsDigitalContentsOnly)) continue;
	//		if ((this.IsOrderCombined == false)
	//			&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)) continue;
	//		if ((this.IsOrderCombined == false)
	//			&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)) continue;
	//		if ((this.IsOrderCombined == false)
	//			&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY)) continue;

	//		var isUserShippingConvenienceStore = false;
	//		if ((ddlUserShipping.SelectedValue != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
	//			&& (ddlUserShipping.SelectedValue != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW))
	//		{
	//			var userShipping = this.UserShippingAddress.FirstOrDefault(item => item.ShippingNo.ToString() == ddlUserShipping.SelectedValue);
	//			isUserShippingConvenienceStore = (userShipping != null)
	//				&& (userShipping.ShippingReceivingStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON);
	//		}

	//		if ((ddlUserShipping.SelectedValue != CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE)
	//			&& (isUserShippingConvenienceStore == false)
	//			&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE))
	//		{
	//			continue;
	//		}

	//		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
	//			&& (IsPaymentAtConvenienceStore(ddlShippingReceivingStoreType.SelectedValue)
	//				&& (payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)
	//			|| ((IsPaymentAtConvenienceStore(ddlShippingReceivingStoreType.SelectedValue) == false)
	//				&& (payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE))))
	//		{
	//			continue;
	//		}

	//		if ((this.ddlShippingKbnList.SelectedValue == Constants.SHIPPING_KBN_LIST_STORE_PICKUP)
	//			&& ((Constants.SETTING_CAN_STORE_PICKUP_OPTION_PAYMENT_IDS.Contains(payment.PaymentId) == false)
	//				|| Constants.SETTING_CAN_NOT_STORE_PICKUP_OPTION_PAYMENT_IDS.Contains(payment.PaymentId)))
	//		{
	//			continue;
	//		}

	//		var items = new ListItem(payment.PaymentName, payment.PaymentId);
	//		items.Selected = (payment.PaymentId == selectedPaymentKbn);
	//		ddlOrderPaymentKbn.Items.Add(items);
	//		if ((payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
	//			&& (ddlUserCreditCard.Items.Count == 0))
	//		{
	//			ddlUserCreditCard.Items.Add(
	//				new ListItem(
	//					ReplaceTag("@@DispText.credit_card_list.new@@"),
	//					CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW,
	//					this.IsNotRakutenAgency));
	//		}
	//	}

	//	dvPaymentKbnCredit.Visible = (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
	//}

	/// <summary>楽天連携以外か</summary>
	protected bool IsNotRakutenAgency
	{
		get { return (Constants.PAYMENT_CARD_KBN != w2.App.Common.Constants.PaymentCard.Rakuten); }
	}

	/// <summary>
	/// 決済種別選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlOrderPaymentKbn_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		var orderInput = (Hashtable)this.OrderParams["order_input"];
		var userId = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_USER_ID]);
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
			this.AccountEmail = PayPalUtility.Account.GetCooperateAccountEmail(userId);
			lOrderPaymentInfo.Text = GetEncodedHtmlDisplayMessage(
				string.IsNullOrEmpty(this.AccountEmail)
					? WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYPAL_ISNOT_LINK_ACCOUNT)
					: WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PAYPAL_AVAILABLE_ACCOUNT)
						.Replace("@@ 1 @@", this.AccountEmail));
		}
		GetGmoCvsType();
		GetRakutenCvsType();
		this.IsUpdatePayment = false;

		PaymentChanged();
	}

	protected string AccountEmail { get; set; }

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

	protected void ddlUserCreditCard_SelectedIndexChanged(object sender, EventArgs e)
	{
		DisplayCreditInputForm();
		RefreshAndUpdateSessionData();
	}

	/// <summary>
	/// クレジット入力フォーム表示切り替え
	/// </summary>
	private void DisplayCreditInputForm()
	{
		var orderInput = (Hashtable)this.OrderParams["order_input"];
		var userId = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_USER_ID]);
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
					var userCreditCard = DomainFacade.Instance.UserCreditCardService.Get(userId, branchNo);
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
	/// クレジットカード登録チェックボックスチェック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbRegistCreditCard_CheckedChanged(object sender, EventArgs e)
	{
		DisplayCreditInputForm();
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


	/// <summary>注文同梱カート情報</summary>
	protected CartObject OriginOrderCombineCart
	{
		get { return (CartObject)Session[Constants.SESSION_KEY_ORDERCOMBINE_ORIGIN_CART]; }
		set { Session[Constants.SESSION_KEY_ORDERCOMBINE_ORIGIN_CART] = value; }
	}

	public void clearTimeInterval()
	{
		ddlMonth.SelectedValue = String.Empty;
		ddlMonthlyDate.SelectedValue = String.Empty;
		ddlIntervalMonths.SelectedValue = String.Empty;
		ddlWeekOfMonth.SelectedValue = String.Empty;
		ddlDayOfWeek.SelectedValue = String.Empty;
		ddlIntervalMonths.SelectedValue = String.Empty;
		ddlWeekOfMonth.SelectedValue = String.Empty;
		ddlDayOfWeek.SelectedValue = String.Empty;
		ddlIntervalDays.SelectedValue = String.Empty;
	}

	public void RefreshAndUpdateSessionData()
	{
		var orderInput = (Hashtable)this.OrderParams["order_input"];
		var user = (Hashtable)this.OrderParams["user_input"];
		var cart = (CartObject)this.OrderParams["cart"];

		Dictionary<string, MemoDetails> memoOrderDictionary;
		var existingMemoOrderParams = Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] as Hashtable;

		if (existingMemoOrderParams != null && existingMemoOrderParams["memo_data_list"] != null)
		{
			memoOrderDictionary = (Dictionary<string, MemoDetails>)existingMemoOrderParams["memo_data_list"];
		}
		else
		{
			memoOrderDictionary = new Dictionary<string, MemoDetails>();
		}

		if (memoOrderDictionary.ContainsKey(selectedOrderId))
		{
			MemoDetails memoData = memoOrderDictionary[selectedOrderId];
			tbPaymentMemo.Text = memoData.PaymentMemo;
			tbRegulationMemo.Text = memoData.RegulationMemo;
			tbRelationMemo.Text = memoData.RelationMemo;
			tbFixedPurchaseManagementMemo.Text = memoData.FixedPurchaseManagementMemo;
			tbShippingMemo.Text = memoData.ShippingMemo;
			tbManagerMemo.Text = memoData.ManagerMemo;
			memoOrderDictionary[selectedOrderId] = memoData;
		}
		else
		{
			MemoDetails memodata = new MemoDetails();
			tbPaymentMemo.Text = string.Empty;
			tbRegulationMemo.Text = string.Empty;
			tbRelationMemo.Text = string.Empty;
			tbFixedPurchaseManagementMemo.Text = string.Empty;
			tbManagerMemo.Text = string.Empty;
			tbShippingMemo.Text = string.Empty;
			memoOrderDictionary.Add(selectedOrderId, memodata);
		}

		if (existingMemoOrderParams == null)
		{
			existingMemoOrderParams = new Hashtable();
		}

		var oldCartShipping = cart.GetShipping();
		CartObject cartObject = new CartObject();
		CartShipping newCartShipping = new CartShipping(cartObject);
		var selectedDeliveryCompanyId = ddlDeliveryCompany.SelectedValue;
		var shopShippingInfo = DomainFacade.Instance.ShopShippingService.GetShopShippingByShippingBaseId(cart.ShopId, selectedShippingBaseId);

		RefreshPaymentViewFromShippingType(Cart, this.shopShippings);
		newCartShipping.CartObject.Payment = GetCartPayment(cart);

		Dictionary<string, CartShipping> orderShippingDictionary1 = new Dictionary<string, CartShipping>
	{
		{ selectedOrderId, newCartShipping }
	};

		Dictionary<string, CartShipping> orderShippingDictionary;
		var existingOrderParams = Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] as Hashtable;
		if (existingOrderParams != null && existingOrderParams["orderShipping_list"] != null)
		{
			orderShippingDictionary = (Dictionary<string, CartShipping>)existingOrderParams["orderShipping_list"];
		}
		else
		{
			orderShippingDictionary = new Dictionary<string, CartShipping>();
		}

		if (orderShippingDictionary.ContainsKey(selectedOrderId))
		{
			//dvPayment.Visible = true;
			//dvShippingFixedPurchase.Visible = true;
			newCartShipping = orderShippingDictionary[selectedOrderId];
			ddlDeliveryCompany.SelectedValue = newCartShipping.DeliveryCompanyId;
			ddlShippingMethod.SelectedValue = newCartShipping.ShippingMethod;
			tbShippingCheckNo.Text = newCartShipping.ShippingCheckNo;
			if (newCartShipping.ShippingDate.HasValue)
			{
				dvShippingDate.Visible = true;
				string formattedDate = FormatDateWithDayOfWeek(newCartShipping.ShippingDate.Value);
				ListItem item = ddlShippingDate.Items.FindByText(formattedDate);
				if (item != null)
				{
					ddlShippingDate.SelectedValue = item.Value;
					//update
					var shippingId = shopShippings
						.Where(m => m.ShippingBaseId == selectedShippingBaseId).Select(m => m.ShippingId)
						.FirstOrDefault();
					ShopShippingModel shopShipping = shopShippings.Where(m => m.ShippingId == shippingId).FirstOrDefault();
					var shippingDateErrorMessage = string.Empty;
					var cartShipping = Cart.GetShipping();
					shippingDateErrorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_SHIPPINGDATE_INVALID)
							.Replace("@@ 1 @@", DateTimeUtility.ToStringForManager(
								HolidayUtil.GetShortestDeliveryDate(
									Cart.ShopId,
									newCartShipping.DeliveryCompanyId,
									shopShipping.ShippingBaseId,
									cartShipping.Zip.Replace("-", string.Empty)),
								DateTimeUtility.FormatType.LongDateWeekOfDay2Letter,
								Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE));
					lShippingDateErrorMessages.Text = GetEncodedHtmlDisplayMessage(shippingDateErrorMessage);
				}
			}
			if (newCartShipping.FixedPurchaseKbn == "01")
			{
				RefreshFixedPurchaseViewFromShippingTypeBySelectedDeliveryName(Cart, this.shopShippings, newCartShipping.DeliveryCompanyId);

				rbMonthlyPurchase_Date.Checked = true;
				rbMonthlyPurchase_WeekAndDay.Checked = false;
				rbRegularPurchase_IntervalDays.Checked = false;

				var fixedPurchaseKbn = newCartShipping.FixedPurchaseKbn;

				var fixedPurchaseSetting = newCartShipping.FixedPurchaseSetting;

				var daysRequired = newCartShipping.FixedPurchaseDaysRequired;

				var minSpan = newCartShipping.FixedPurchaseMinSpan;

				var shippingDate = (ddlShippingMethod.SelectedValue != Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
					? string.Empty
					: ddlShippingDate.SelectedValue;

				SetNextShippingDate(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				newCartShipping.FirstShippingDate,
				newCartShipping.NextShippingDate,
				newCartShipping.NextNextShippingDate,
				minSpan,
				newCartShipping);

				clearTimeInterval();

				var fixedPurchaseSettings = newCartShipping.FixedPurchaseSetting.Split(',');
				if (fixedPurchaseSettings.Length == 2)
				{
					RefreshFixedPurchaseViewFromShippingTypeBySelectedDeliveryName(Cart, this.shopShippings, newCartShipping.DeliveryCompanyId);
					var monthSetting = fixedPurchaseSettings[0];
					var dateSetting = fixedPurchaseSettings[1];

					ddlMonth.SelectedValue = monthSetting;
					ddlMonthlyDate.SelectedValue = dateSetting;
				}
				if (newCartShipping.FirstShippingDate.ToString() != null)
				{
					string formattedDate = FormatDateWithDayOfWeek(newCartShipping.FirstShippingDate);

					if (formattedDate != null)
					{
						lFirstShippingDate.Text = formattedDate;
					}
				}
				if (newCartShipping.NextShippingDate.ToString() != null)
				{
					string formattedDate = FormatDateWithDayOfWeek(newCartShipping.NextShippingDate);
					if (formattedDate != null)
					{
						lNextShippingDate.Text = formattedDate;
					}
				}
			}
			else if (newCartShipping.FixedPurchaseKbn == "02")
			{
				RefreshFixedPurchaseViewFromShippingTypeBySelectedDeliveryName(Cart, this.shopShippings, newCartShipping.DeliveryCompanyId);
				rbMonthlyPurchase_WeekAndDay.Checked = true;
				rbRegularPurchase_IntervalDays.Checked = false;
				rbMonthlyPurchase_Date.Checked = false;

				var fixedPurchaseKbn = newCartShipping.FixedPurchaseKbn;

				var fixedPurchaseSetting = newCartShipping.FixedPurchaseSetting;

				var daysRequired = newCartShipping.FixedPurchaseDaysRequired;

				var minSpan = newCartShipping.FixedPurchaseMinSpan;

				var shippingDate = (ddlShippingMethod.SelectedValue != Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
					? string.Empty
					: ddlShippingDate.SelectedValue;

				SetNextShippingDate(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				newCartShipping.FirstShippingDate,
				newCartShipping.NextShippingDate,
				newCartShipping.NextNextShippingDate,
				minSpan,
				newCartShipping);
				clearTimeInterval();
				var fixedPurchaseSettings = newCartShipping.FixedPurchaseSetting.Split(',');
				if (fixedPurchaseSettings.Length == 3)
				{
					var monthSetting = fixedPurchaseSettings[0];
					var weekOfMonthSetting = fixedPurchaseSettings[1];
					var dayOfMonthSetting = fixedPurchaseSettings[2];

					if (!string.IsNullOrEmpty(weekOfMonthSetting) && !string.IsNullOrEmpty(dayOfMonthSetting))
					{
						ddlIntervalMonths.SelectedValue = monthSetting;
						ddlWeekOfMonth.SelectedValue = weekOfMonthSetting;
						ddlDayOfWeek.SelectedValue = dayOfMonthSetting;
					}
				}

				if (newCartShipping.FirstShippingDate.ToString() != null)
				{
					string formattedDate = FormatDateWithDayOfWeek(newCartShipping.FirstShippingDate);

					if (formattedDate != null)
					{
						lFirstShippingDate.Text = formattedDate;
					}
				}
				if (newCartShipping.NextShippingDate.ToString() != null)
				{
					string formattedDate = FormatDateWithDayOfWeek(newCartShipping.NextShippingDate);
					if (formattedDate != null)
					{
						lNextShippingDate.Text = formattedDate;
					}
				}
			}

			else if (newCartShipping.FixedPurchaseKbn == "03")
			{
				RefreshFixedPurchaseViewFromShippingTypeBySelectedDeliveryName(Cart, this.shopShippings, newCartShipping.DeliveryCompanyId);
				var fixedPurchaseKbn = newCartShipping.FixedPurchaseKbn;

				var fixedPurchaseSetting = newCartShipping.FixedPurchaseSetting;

				var daysRequired = newCartShipping.FixedPurchaseDaysRequired;

				var minSpan = newCartShipping.FixedPurchaseMinSpan;

				var shippingDate = (ddlShippingMethod.SelectedValue != Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS)
					? string.Empty
					: ddlShippingDate.SelectedValue;

				SetNextShippingDate(
				fixedPurchaseKbn,
				fixedPurchaseSetting,
				newCartShipping.FirstShippingDate,
				newCartShipping.NextShippingDate,
				newCartShipping.NextNextShippingDate,
				minSpan,
				newCartShipping);

				rbRegularPurchase_IntervalDays.Checked = true;
				rbMonthlyPurchase_WeekAndDay.Checked = false;
				rbMonthlyPurchase_Date.Checked = false;
				clearTimeInterval();
				ddlIntervalDays.SelectedValue = newCartShipping.FixedPurchaseSetting;
				if (newCartShipping.FirstShippingDate.ToString() != null)
				{
					string formattedDate = FormatDateWithDayOfWeek(newCartShipping.FirstShippingDate);

					if (formattedDate != null)
					{
						lFirstShippingDate.Text = formattedDate;
					}
				}
				if (newCartShipping.NextShippingDate.ToString() != null)
				{
					string formattedDate = FormatDateWithDayOfWeek(newCartShipping.NextShippingDate);
					if (formattedDate != null)
					{
						lNextShippingDate.Text = formattedDate;
					}
				}
			}
			else
			{
				RefreshFixedPurchaseViewFromShippingType(Cart, this.shopShippings);
				rbMonthlyPurchase_WeekAndDay.Checked = false;
				rbRegularPurchase_IntervalDays.Checked = false;
				rbMonthlyPurchase_Date.Checked = false;
			}
			RefreshPaymentViewFromShippingType(Cart, this.shopShippings);

			if (newCartShipping.CartObject.Payment.PaymentId != null)
			{
				Boolean hasExist = false;
				foreach (var itemss in ddlOrderPaymentKbn.Items)
				{
					var listItem = itemss as ListItem;
					if (listItem != null && listItem.Value == newCartShipping.CartObject.Payment.PaymentId)
					{
						hasExist = true;
					}
				}
				ddlOrderPaymentKbn.SelectedValue = hasExist ? newCartShipping.CartObject.Payment.PaymentId : Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;
			}
			// Get gmo cvs type
			GetGmoCvsType();

			// Get rakuten cvs type
			GetRakutenCvsType();

			dvPaymentKbnCredit.Visible = (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
		}
		else
		{
			clearTimeInterval();
			RefreshPaymentViewFromShippingType(Cart, this.shopShippings);
			if (string.IsNullOrEmpty(ddlDeliveryCompany.SelectedValue))
			{
				if (shopShippingInfo != null)
				{
					selectedDeliveryCompanyId = (cart.GetShipping().IsExpress
						? shopShippingInfo[0].CompanyListExpress
						: shopShippingInfo[0].CompanyListMail).First(model => model.IsDefault).DeliveryCompanyId;
				}
			}
			newCartShipping.DeliveryCompanyId = selectedDeliveryCompanyId;
			newCartShipping.ShippingMethod = ddlShippingMethod.SelectedValue;
			ddlShippingDate.SelectedValue = string.Empty;
			foreach (var itemss in ddlOrderPaymentKbn.Items)
			{
				var listItem = itemss as ListItem;
				if (listItem != null & listItem.Value == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				{
					ddlOrderPaymentKbn.SelectedValue = listItem.Value;
					
				}
			}
			//ddlOrderPaymentKbn.SelectedValue = hasExist ? newCartShipping.CartObject.Payment.PaymentId : Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT;


			tbShippingCheckNo.Text = string.Empty;
			newCartShipping.ShippingCheckNo = tbShippingCheckNo.Text;
			orderShippingDictionary.Add(selectedOrderId, newCartShipping);
			rbRegularPurchase_IntervalDays.Checked = false;
			rbMonthlyPurchase_WeekAndDay.Checked = false;
			rbMonthlyPurchase_Date.Checked = false;
			lNextShippingDate.Text = string.Empty;
			lFirstShippingDate.Text = string.Empty;
			ddlIntervalDays.SelectedValue = string.Empty;
			lShippingDateErrorMessages.Text = string.Empty;
		}

		Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] = new Hashtable
	{
		{ "memo_data_list", memoOrderDictionary },
		{ "orderShipping_list", orderShippingDictionary }
		};
	}
	/// <summary>注文情報パラメタ</summary>
	protected Hashtable NewOrderParams
	{
		get { return (Hashtable)Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT]; }
	}

	protected void tbMemo_TextChanged(object sender, EventArgs e)
	{
		memoDataChange();
	}

	protected void tbShippingCheckNo_TextChanged(object sender, EventArgs e)
	{
		var existingOrderParams = Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] as Hashtable;
		Dictionary<string, CartShipping> orderShippingDictionary = existingOrderParams != null && existingOrderParams["orderShipping_list"] != null
			? (Dictionary<string, CartShipping>)existingOrderParams["orderShipping_list"]
			: new Dictionary<string, CartShipping>();

		CartShipping newCartShipping;
		if (orderShippingDictionary.ContainsKey(selectedOrderId))
		{
			newCartShipping = orderShippingDictionary[selectedOrderId];
		}
		else
		{
			CartObject cartObject = new CartObject();
			newCartShipping = new CartShipping(cartObject);
		}

		newCartShipping.ShippingCheckNo = tbShippingCheckNo.Text;
		orderShippingDictionary[selectedOrderId] = newCartShipping;

		if (existingOrderParams == null)
		{
			existingOrderParams = new Hashtable();
		}
		existingOrderParams["orderShipping_list"] = orderShippingDictionary;
		Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] = existingOrderParams;
	}

	public void memoDataChange()
	{
		MemoDetails updatedMemoData = new MemoDetails
		{
			PaymentMemo = tbPaymentMemo.Text,
			RegulationMemo = tbRegulationMemo.Text,
			RelationMemo = tbRelationMemo.Text,
			FixedPurchaseManagementMemo = tbFixedPurchaseManagementMemo.Text,
			ShippingMemo = tbShippingMemo.Text,
			ManagerMemo = tbManagerMemo.Text,
		};

		var existingOrderParams = Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] as Hashtable;
		Dictionary<string, MemoDetails> memoOrderDictionary = existingOrderParams != null && existingOrderParams["memo_data_list"] != null
			? (Dictionary<string, MemoDetails>)existingOrderParams["memo_data_list"]
			: new Dictionary<string, MemoDetails>();

		if (memoOrderDictionary.ContainsKey(selectedOrderId))
		{
			memoOrderDictionary[selectedOrderId] = updatedMemoData;
		}
		else
		{
			memoOrderDictionary.Add(selectedOrderId, updatedMemoData);
		}

		if (existingOrderParams == null)
		{
			existingOrderParams = new Hashtable();
		}
		existingOrderParams["memo_data_list"] = memoOrderDictionary;
		Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] = existingOrderParams;
	}

	public void RefreshAndUpdateDeliverySessionData()
	{
		string selectedDeliveryCompanyId = ddlDeliveryCompany.SelectedValue;

		var existingOrderParams = Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] as Hashtable;
		Dictionary<string, CartShipping> orderShippingDictionary = existingOrderParams != null && existingOrderParams["orderShipping_list"] != null
			? (Dictionary<string, CartShipping>)existingOrderParams["orderShipping_list"]
			: new Dictionary<string, CartShipping>();

		CartShipping newCartShipping;
		if (orderShippingDictionary.ContainsKey(selectedOrderId))
		{
			newCartShipping = orderShippingDictionary[selectedOrderId];
		}
		else
		{
			CartObject cartObject = new CartObject();
			newCartShipping = new CartShipping(cartObject);
		}

		newCartShipping.DeliveryCompanyId = selectedDeliveryCompanyId;

		orderShippingDictionary[selectedOrderId] = newCartShipping;

		if (existingOrderParams == null)
		{
			existingOrderParams = new Hashtable();
		}
		existingOrderParams["orderShipping_list"] = orderShippingDictionary;
		Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] = existingOrderParams;
	}

	public void RefreshAndUpdateDeliveryTimeSessionData()
	{
		var existingOrderParams = Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] as Hashtable;
		Dictionary<string, CartShipping> orderShippingDictionary = existingOrderParams != null && existingOrderParams["orderShipping_list"] != null
			? (Dictionary<string, CartShipping>)existingOrderParams["orderShipping_list"]
			: new Dictionary<string, CartShipping>();

		CartShipping newCartShipping;
		if (orderShippingDictionary.ContainsKey(selectedOrderId))
		{
			newCartShipping = orderShippingDictionary[selectedOrderId];
		}
		else
		{
			CartObject cartObject = new CartObject();
			newCartShipping = new CartShipping(cartObject);
		}

		if (ddlShippingDate.SelectedValue != null)
		{
			string selectedDateValue = ddlShippingDate.SelectedValue;
			DateTime selectedDate;
			if (DateTime.TryParse(selectedDateValue, out selectedDate))
			{
				newCartShipping.ShippingDate = selectedDate;

			}
		}
		orderShippingDictionary[selectedOrderId] = newCartShipping;

		if (existingOrderParams == null)
		{
			existingOrderParams = new Hashtable();
		}
		existingOrderParams["orderShipping_list"] = orderShippingDictionary;
		Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] = existingOrderParams;
	}

	private string FormatDateWithDayOfWeek(DateTime date)
	{
		string dayOfWeek = date.ToString("ddd", new System.Globalization.CultureInfo("ja-JP"));
		return String.Format("{0}年{1}月{2}日({3})", date.Year, date.Month, date.Day, dayOfWeek);
	}

	public void MonthlyPurchaseWeekAndDayChanged()
	{
		var orderInput = (Hashtable)this.OrderParams["order_input"];
		var user = (Hashtable)this.OrderParams["user_input"];
		var cart = (CartObject)this.OrderParams["cart"];
		var oldCartShipping = cart.GetShipping();
		CartObject cartObject = new CartObject();
		CartShipping newCartShipping = new CartShipping(cartObject);
		var selectedDeliveryCompanyId = ddlDeliveryCompany.SelectedValue;
		var shopShippingInfo = DomainFacade.Instance.ShopShippingService.GetShopShippingByShippingBaseId(cart.ShopId, selectedShippingBaseId);
		var existingOrderParams = Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] as Hashtable;
		Dictionary<string, CartShipping> orderShippingDictionary = existingOrderParams != null && existingOrderParams["orderShipping_list"] != null
			? (Dictionary<string, CartShipping>)existingOrderParams["orderShipping_list"]
			: new Dictionary<string, CartShipping>();

		if (orderShippingDictionary.ContainsKey(selectedOrderId))
		{
			newCartShipping = orderShippingDictionary[selectedOrderId];

			// 情報取得
			var fixedPurchaseSettings = GetFixedPurchaseSettingInputs();

			// 入力チェック
			var shippingFixedPurchaseErrorMessage = Validator.Validate("OrderShippingRegistInput", fixedPurchaseSettings);
			lFixedPurchasePatternErrorMessage.Text = GetEncodedHtmlDisplayMessage(shippingFixedPurchaseErrorMessage);
			dvFixedPurchasePatternErrorMessages.Visible = (shippingFixedPurchaseErrorMessage.Length != 0);

			if (string.IsNullOrEmpty(shippingFixedPurchaseErrorMessage))
			{
				var fixedPurchaseKbn = StringUtility.ToEmpty(
					fixedPurchaseSettings[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN]);
				var fixedPurchaseSetting = StringUtility.ToEmpty(
					fixedPurchaseSettings[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1]);
				var daysRequired = int.Parse(StringUtility.ToEmpty(
					fixedPurchaseSettings[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_SHIPPING_DAYS_REQUIRED]));
				var minSpan = int.Parse(StringUtility.ToEmpty(
					fixedPurchaseSettings[Constants.FIELD_SHOPSHIPPING_FIXED_PURCHASE_MINIMUM_SHIPPING_SPAN]));

				newCartShipping.UpdateFixedPurchaseSetting(
							 fixedPurchaseKbn,
							 fixedPurchaseSetting,
							 daysRequired,
							 minSpan);
				if ((this.IsOrderCombined == false) || ((this.IsOrderCombined) && (cart.GetShipping().ShippingDate == null)))
				{
					newCartShipping.ShippingDate = (string.IsNullOrEmpty(ddlShippingDate.SelectedValue) == false)
							? DateTime.Parse(ddlShippingDate.SelectedValue)
							: (DateTime?)null;
				}

				newCartShipping.UpdateFixedPurchaseSetting(
							fixedPurchaseKbn,
							fixedPurchaseSetting,
							daysRequired,
							minSpan);
				if ((this.IsOrderCombined == false) || ((this.IsOrderCombined) && (newCartShipping.ShippingDate == null)))
				{
					newCartShipping.ShippingDate = (string.IsNullOrEmpty(ddlShippingDate.SelectedValue) == false)
							? DateTime.Parse(ddlShippingDate.SelectedValue)
							: (DateTime?)null;
				}

				var calculationMode = rbPurchase_EveryNWeek.Checked
					? NextShippingCalculationMode.EveryNWeek
					: Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE;

				var cartShipping = newCartShipping;
				var shippingId = shopShippings
							.Where(m => m.ShippingBaseId == selectedShippingBaseId).Select(m => m.ShippingId)
							.FirstOrDefault();
				ShopShippingModel shopShipping = shopShippings.Where(m => m.ShippingId == shippingId).FirstOrDefault();

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
									   oldCartShipping.ShippingDate,
									   cartShipping.ShippingMethod,
									   cartShipping.DeliveryCompanyId,
									   cartShipping.ShippingCountryIsoCode,
									   cartShipping.IsTaiwanCountryShippingEnable
									   ? oldCartShipping.Addr2
					: oldCartShipping.Addr1,
									   oldCartShipping.Zip);
				}

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
				cartShipping.FirstShippingDate = firstShippingDate;

				if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED)
				{
					var cartFixedPurchaseNextShippingProduct = cart.Items
						.FirstOrDefault(cartProduct => cartProduct.CanSwitchProductFixedPurchaseNextShippingSecondTime());
					if (cartFixedPurchaseNextShippingProduct != null)
					{
						newCartShipping.CanSwitchProductFixedPurchaseNextShippingSecondTime = true;
						newCartShipping.UpdateNextShippingItemFixedPurchaseInfos(
									cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseKbn,
									cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseSetting);
						newCartShipping.CalculateNextShippingItemNextNextShippingDate();
					}
				}
			}
			else
			{
				rbMonthlyPurchase_WeekAndDay.Focus();
			}

			if (newCartShipping.ShippingDate.HasValue)
			{
				string formattedDate = FormatDateWithDayOfWeek(newCartShipping.ShippingDate.Value);
				ListItem item = ddlShippingDate.Items.FindByText(formattedDate);
				if (item != null)
				{
					ddlShippingDate.SelectedValue = item.Value;
				}
			}
			else
			{
				ddlShippingDate.SelectedValue = String.Empty;
			}

			var userId = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_USER_ID]);

			var orderCreditCardInput = GetOrderCreditCardInputForOrderPage(
							ddlOrderPaymentKbn.SelectedValue,
							userId);

			// トークンが取得できていないときはエラーとして扱う(バグ#3554対策)
			if (OrderCommon.CreditTokenUse
				&& (orderCreditCardInput.CreditToken == null))
			{

				spanErrorMessageForCreditCard.InnerHtml =
					GetEncodedHtmlDisplayMessage(
						WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
				spanErrorMessageForCreditCard.Style["display"] = "block";
				ddlOrderPaymentKbn.Focus();
			}

			if (this.IsUserPayTg)
			{
				cart.Payment.CreditToken = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans
					? CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(" " + this.VeriTransAccountId)
					: CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.CreditTokenbyPayTg);
				if ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
					&& (ddlUserCreditCard.SelectedValue == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
				{
					cartObject.Payment.CreditExpireMonth = ddlCreditExpireMonth.SelectedValue;
					cartObject.Payment.CreditExpireYear = ddlCreditExpireYear.SelectedValue;
					cartObject.Payment.CreditCardCompany = this.CreditCardCompanyCodebyPayTg;
				}
			}
		}
		else
		{
			newCartShipping = new CartShipping(cartObject);
		}

		orderShippingDictionary[selectedOrderId] = newCartShipping;

		if (existingOrderParams == null)
		{
			existingOrderParams = new Hashtable();
		}
		existingOrderParams["orderShipping_list"] = orderShippingDictionary;
		Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] = existingOrderParams;
	}

	public void PaymentChanged()
	{
		var orderInput = (Hashtable)this.OrderParams["order_input"];
		var user = (Hashtable)this.OrderParams["user_input"];
		var cart = (CartObject)this.OrderParams["cart"];
		var oldCartShipping = cart.GetShipping();
		CartObject cartObject = new CartObject();
		CartShipping newCartShipping = new CartShipping(cartObject);
		var selectedDeliveryCompanyId = ddlDeliveryCompany.SelectedValue;
		var shopShippingInfo = DomainFacade.Instance.ShopShippingService.GetShopShippingByShippingBaseId(cart.ShopId, selectedShippingBaseId);

		var existingOrderParams = Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] as Hashtable;
		Dictionary<string, CartShipping> orderShippingDictionary = existingOrderParams != null && existingOrderParams["orderShipping_list"] != null
			? (Dictionary<string, CartShipping>)existingOrderParams["orderShipping_list"]
			: new Dictionary<string, CartShipping>();

		if (orderShippingDictionary.ContainsKey(selectedOrderId))
		{
			newCartShipping = orderShippingDictionary[selectedOrderId];

			var userId = StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_USER_ID]);

			var orderCreditCardInput = GetOrderCreditCardInputForOrderPage(
							ddlOrderPaymentKbn.SelectedValue,
							userId);

			// トークンが取得できていないときはエラーとして扱う(バグ#3554対策)
			if (OrderCommon.CreditTokenUse
				&& (orderCreditCardInput.CreditToken == null))
			{

				spanErrorMessageForCreditCard.InnerHtml =
					GetEncodedHtmlDisplayMessage(
						WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
				spanErrorMessageForCreditCard.Style["display"] = "block";
				ddlOrderPaymentKbn.Focus();
			}

			if (this.IsUserPayTg)
			{
				cart.Payment.CreditToken = Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.VeriTrans
					? CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(" " + this.VeriTransAccountId)
					: CartPayment.CreditTokenInfoBase.CreateCreditTokenInfo(this.CreditTokenbyPayTg);
				if ((Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten)
					&& (ddlUserCreditCard.SelectedValue == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW))
				{
					cartObject.Payment.CreditExpireMonth = ddlCreditExpireMonth.SelectedValue;
					cartObject.Payment.CreditExpireYear = ddlCreditExpireYear.SelectedValue;
					cartObject.Payment.CreditCardCompany = this.CreditCardCompanyCodebyPayTg;
				}
			}
			newCartShipping.CartObject.Payment = GetCartPayment(cart);
			int orderIndex = int.Parse(hfSelectedOrderIndex.Value);
			RefreshViewByOrderId(orderIndex);
		}
		else
		{
			newCartShipping = new CartShipping(cartObject);
		}

		orderShippingDictionary[selectedOrderId] = newCartShipping;

		if (existingOrderParams == null)
		{
			existingOrderParams = new Hashtable();
		}
		existingOrderParams["orderShipping_list"] = orderShippingDictionary;
		Session[Constants.SESSION_KEY_PARAM_FOR_ORDER_SPLIT] = existingOrderParams;
	}

	/// <summary>
	/// 定期購入情報を更新する（配送種別から）
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <param name="shopShipping">配送種別情報</param>
	private void RefreshFixedPurchaseViewFromShippingTypeBySelectedDeliveryName(CartObject cart, ShopShippingModel[] shopShippings, String selectedDeliveryName)
	{
		var shippingId = shopShippings
						.Where(m => m.ShippingBaseId == selectedShippingBaseId).Select(m => m.ShippingId)
						.FirstOrDefault();
		ShopShippingModel shopShipping = shopShippings.Where(m => m.ShippingId == shippingId).FirstOrDefault();

		if (shopShipping != null)
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

		if (dvShippingFixedPurchase.Visible)
		{
			if (this.IsUpdateFixedPurchaseShippingPattern == false)
			{
				lFirstShippingDate.Text = string.Empty;
				ddlFirstShippingDate.Visible = false;
				lNextShippingDate.Text = string.Empty;
				ddlNextShippingDate.Visible = false;
			}
		}
	}

	private void RefreshViewByOrderId(int orderIdIndex)
	{
		var orderInput = (Hashtable)this.OrderParams["order_input"];
		var user = (Hashtable)this.OrderParams["user_input"];
		var cart = (CartObject)this.OrderParams["cart"];
		string postBackControlID = Request["__EVENTTARGET"];

		if (this.OrderSplitParam != null)
		{
			var orderShipping = (Dictionary<String, CartShipping>)this.OrderSplitParam["orderShipping_list"];

			if (orderShipping != null && orderShipping.ContainsKey(newCart[orderIdIndex].OrderId))
			{
				var dOrderPriceExchange = (orderShipping[newCart[orderIdIndex].OrderId].CartObject.Payment != null)
			   ? orderShipping[newCart[orderIdIndex].OrderId].CartObject.Payment.PriceExchange
			   : 0m;
				lOrderPriceExchange.Text = GetEncodedHtmlDisplayMessage(dOrderPriceExchange.ToPriceString(true));

				if (postBackControlID.Contains("ddlOrderPaymentKbn"))
				{
					Decimal price = newCart[orderIdIndex].PriceTotal;
					if (newCart[orderIdIndex].Payment.PriceExchange > 0)
					{
						newCart[orderIdIndex].PriceTotal = price - newCart[orderIdIndex].Payment.PriceExchange;
						price = newCart[orderIdIndex].PriceTotal;
					}
					newCart[orderIdIndex].Payment.PriceExchange = orderShipping[newCart[orderIdIndex].OrderId].CartObject.Payment.PriceExchange;
					newCart[orderIdIndex].PriceTotal = price + orderShipping[newCart[orderIdIndex].OrderId].CartObject.Payment.PriceExchange;
				}
			}
		}

		string selectedId = selectedShippingNames;
		hfShippingType.Value = selectedId;

		RefreshDeliverySpecificationFromShippingType(Cart, this.shopShippings, true);

		setOrderList();

		BindDeliveryName(selectedShippingBaseId);

		// 注文者情報表示
		lOwnerName.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.Name);
		lOwnerNameKana.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.NameKana);
		lOwnerKbn.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_ORDEROWNER,
				Constants.FIELD_ORDEROWNER_OWNER_KBN,
				newCart[orderIdIndex].Owner.OwnerKbn));
		lOwnerTel1.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.Tel1);
		lOwnerTel2.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.Tel2);
		lOwnerMailAddr.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.MailAddr);
		lOwnerMailAddr2.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.MailAddr2);
		lOwnerZip.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.Zip);
		lOwnerAddr1.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.Addr1);
		lOwnerAddr2.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.Addr2);
		lOwnerAddr3.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.Addr3);
		lOwnerAddr4.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.Addr4);
		lOwnerCompanyName.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.CompanyName);
		lOwnerCompanyPostName.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.CompanyPostName);
		lOwnerSex.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_ORDEROWNER,
				Constants.FIELD_ORDEROWNER_OWNER_SEX,
				newCart[orderIdIndex].Owner.Sex));
		lOwnerBirth.Text = GetEncodedHtmlDisplayMessage(
			DateTimeUtility.ToStringForManager(
				newCart[orderIdIndex].Owner.Birth,
				DateTimeUtility.FormatType.LongDate1LetterNoneServerTime));

		if (Constants.GLOBAL_OPTION_ENABLE)
		{
			lOwnerAccessCountryIsoCode.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.AccessCountryIsoCode);
			lOwnerDispLanguageCode.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.DispLanguageCode);
			lOwnerDispLanguageLocaleId.Text = GetEncodedHtmlDisplayMessage(
				GlobalConfigUtil.LanguageLocaleIdDisplayFormat(newCart[orderIdIndex].Owner.DispLanguageLocaleId));
			lOwnerDispCurrencyCode.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.DispCurrencyCode);
			lOwnerDispCurrencyLocaleId.Text = GetEncodedHtmlDisplayMessage(
				GlobalConfigUtil.CurrencyLocaleIdDisplayFormat(newCart[orderIdIndex].Owner.DispCurrencyLocaleId));

			lOwnerCountryName.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.AddrCountryName);
			lOwnerAddr5.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.Addr5);
			lOwnerZipGlobal.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Owner.Zip);
		}

		// 注文メモ
		rOrderMemos.DataSource = GetOrderMemoSetting(Constants.FLG_ORDER_MEMO_SETTING_DISP_FLG_PC);
		rOrderMemos.DataBind();
		rSetPromotion.DataSource = newCart[orderIdIndex].SetPromotions.Items;
		rSetPromotion.DataBind();

		// カード会社
		if (OrderCommon.CreditCompanySelectable)
		{
			ddlCreditCardCompany.Items.AddRange(ValueText.GetValueItemArray(
				Constants.TABLE_ORDER,
				OrderCommon.CreditCompanyValueTextFieldName));
		}

		// 楽天連携かつPayTg非利用の場合、新規クレカ入力領域を非表示にする
		this.phCreditCardNotRakuten.Visible = (this.IsNotRakutenAgency || (this.IsUserPayTg && Constants.PAYMENT_CARD_KBN == Constants.PaymentCard.Rakuten));
		tdCreditNumber.Visible = (this.IsUserPayTg == false);

		var combinedCartList = new CartObjectList(newCart[orderIdIndex].CartUserId, newCart[orderIdIndex].OrderKbn, false);

		// クーポン
		var hasCoupon = ((newCart[orderIdIndex].Coupon != null) && (string.IsNullOrEmpty(newCart[orderIdIndex].Coupon.CouponCode) == false));
		lCouponCode.Text = GetEncodedHtmlDisplayMessage(
			hasCoupon
				? newCart[orderIdIndex].Coupon.CouponCode
				: ReplaceTag("@@DispText.common_message.unspecified@@"));
		dvCouponDetail.Visible = hasCoupon;
		if (hasCoupon)
		{
			lCouponDiscount.Text = GetEncodedHtmlDisplayMessage(CouponOptionUtility.GetCouponDiscountString(newCart[orderIdIndex].Coupon));
			lCouponName.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Coupon.CouponName);
			lCouponDispName.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].Coupon.CouponDispName);
		}

		// ポイント
		lOrderPointUse.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToNumeric(newCart[orderIdIndex].UsePoint));
		lOrderPointAdd.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToNumeric(newCart[orderIdIndex].BuyPoint + newCart[orderIdIndex].FirstBuyPoint));
		var userPoint = PointOptionUtility.GetUserPoint(newCart[0].OrderUserId);
		var userPointUsable = (userPoint != null) ? userPoint.PointUsable : 0;
		lUserPointUsable.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToNumeric(userPointUsable));
		dvUseAllPointFlg.Visible = newCart[orderIdIndex].UseAllPointFlg;

		// 金額系セット
		lOrderPriceSubTotal.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].PriceSubtotal.ToPriceString(true));
		lOrderPriceShipping.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].PriceShipping.ToPriceString(true));

		lOrderPriceRegulation.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].PriceRegulation.ToPriceString(true));
		AddAttributesForControlDisplay(
			trPriceRegulation,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(newCart[orderIdIndex].PriceRegulation < 0));

		lMemberRankDiscount.Text = GetEncodedHtmlDisplayMessage(
			(newCart[orderIdIndex].MemberRankDiscount * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trMemberRankDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(newCart[orderIdIndex].MemberRankDiscount > 0));

		lFixedPurchaseMemberDiscount.Text = GetEncodedHtmlDisplayMessage(
			(newCart[orderIdIndex].FixedPurchaseMemberDiscountAmount * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trFixedPurchaseMemberDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(newCart[orderIdIndex].FixedPurchaseMemberDiscountAmount > 0));

		lPointUsePrice.Text = GetEncodedHtmlDisplayMessage(
			(newCart[orderIdIndex].UsePointPrice * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trPointDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(newCart[orderIdIndex].UsePointPrice > 0));

		lCouponUsePrice.Text = GetEncodedHtmlDisplayMessage(
			(newCart[orderIdIndex].UseCouponPrice * -1).ToPriceString(true));
		AddAttributesForControlDisplay(
			trCouponDiscount,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(newCart[orderIdIndex].UseCouponPrice > 0));

		lFixedPurchaseDiscountPrice.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].FixedPurchaseDiscount.ToPriceString(true));
		AddAttributesForControlDisplay(
			trFixedPurchaseDiscountPrice,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS,
			CONST_KEY_HTMLCONTROL_ATTRIBUTE_CLASS_DISCOUNT,
			(newCart[orderIdIndex].FixedPurchaseDiscount > 0));


		lOrderPriceTax.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].PriceSubtotalTax.ToPriceString(true));
		lOrderPriceTotalBottom.Text
			= lOrderPriceTotal.Text
				= GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].PriceTotal.ToPriceString(true));

		rTotalPriceByTaxRate.DataSource = newCart[orderIdIndex].PriceInfoByTaxRate;
		rTotalPriceByTaxRate.DataBind();

		// クレジット情報入力域を表示
		dvPaymentKbnCredit.Visible = (ddlOrderPaymentKbn.SelectedValue == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);

		// 領収書情報セット
		if (Constants.RECEIPT_OPTION_ENABLED)
		{
			lReceiptFlg.Text = GetEncodedHtmlDisplayMessage(
				ValueText.GetValueText(
					Constants.TABLE_ORDER,
					Constants.FIELD_ORDER_RECEIPT_FLG,
					newCart[orderIdIndex].ReceiptFlg));
			lReceiptAddress.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].ReceiptAddress);
			lReceiptProviso.Text = GetEncodedHtmlDisplayMessage(newCart[orderIdIndex].ReceiptProviso);
		}

		// コンバージョン情報
		lInflowContentsType.Text = GetEncodedHtmlDisplayMessage(
			ValueText.GetValueText(
				Constants.TABLE_ORDER,
				Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE,
				orderInput[Constants.FIELD_ORDER_INFLOW_CONTENTS_TYPE]));
		lInflowContentsId.Text = GetEncodedHtmlDisplayMessage(StringUtility.ToEmpty(orderInput[Constants.FIELD_ORDER_INFLOW_CONTENTS_ID]));

		// 配送日時
		dvShippingDate.Visible = true;
		dvShippingTime.Visible = false;
		//dvShippingFixedPurchase.Visible = false;
		var zeroPriceHtmlEncoded = GetEncodedHtmlDisplayMessage(0.ToPriceString(true));

		//lOrderPriceExchange.Text = zeroPriceHtmlEncoded;
		//lOrderPriceTotalBottom.Text = zeroPriceHtmlEncoded;

		// カード有効期限(月)
		ddlCreditExpireMonth.Items.AddRange(this.CreditExpirationMonthListItems);
		ddlCreditExpireMonth.SelectedValue = DateTime.Now.Month.ToString("00");

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
		// カード分割支払い
		dvInstallments.Visible = false;

		// カード有効期限(年)
		ddlCreditExpireYear.Items.AddRange(this.CreditExpirationYearListItems);
		ddlCreditExpireYear.SelectedValue = DateTime.Now.Year.ToString("00").Substring(2);

		var selectedPaymentKbn = ddlOrderPaymentKbn.SelectedValue;

		// Get gmo cvs type
		GetGmoCvsType();

		// Get rakuten cvs type
		GetRakutenCvsType();
	}

	/// <summary>
	/// セットプロモーション名取得
	/// </summary>
	/// <param name="shippingNo">配送先枝番</param>
	/// <returns>割引額</returns>
	protected decimal GetOrderSetPromotionDiscountByShipping(string shippingNo)
	{
		var setpromotionByShipping =
			this.newCart[selectedOrderIndex].SetPromotions.Items.Where(sp => sp.ShippingNo.ToString() == shippingNo);
		var result = setpromotionByShipping
			.Sum(sp => sp.ShippingChargeDiscountAmount + sp.ProductDiscountAmount);
		return result;
	}
}
