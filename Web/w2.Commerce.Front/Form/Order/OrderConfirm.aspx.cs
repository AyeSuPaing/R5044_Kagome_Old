/*
=========================================================================================================
  Module      : 注文確認画面処理(OrderConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.AmazonCv2;
using w2.App.Common.DataCacheController;
using w2.App.Common.Flaps;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.FixedPurchaseCombine;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.Paygent.Paidy.Checkout;
using w2.App.Common.Product;
using w2.App.Common.User;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.Product;
using w2.Domain.TwUserInvoice;
using w2.Domain.User;
using w2.Domain.UserCreditCard;
using w2.Domain.UserDefaultOrderSetting;
using w2.Domain.UserShipping;

public partial class Form_Order_OrderConfirm : OrderCartPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	/// <summary>決済上限チェック結果 </summary>
	private enum CheckPaymentLimitResult
	{
		// <summary>現在の決済種別で決済可能</summary>
		Possible,
		// <summary>現在の決済種別は決済不可、ただし別の決済種別で決済可能</summary>
		ImpossibleButOtherPaymentMethodIsPossible,
		// <summary>決済可能な決済種別なし</summary>
		Impossible,
	}

	#region ラップ済コントロール宣言
	WrappedLinkButton WlbComplete1 { get { return GetWrappedControl<WrappedLinkButton>("lbComplete1"); } }
	WrappedLinkButton WlbComplete2 { get { return GetWrappedControl<WrappedLinkButton>("lbComplete2"); } }
	WrappedLinkButton WlbCart { get { return GetWrappedControl<WrappedLinkButton>("lbCart"); } }
	WrappedLabel WlblOrderCombineAlert { get { return GetWrappedControl<WrappedLabel>("lblOrderCombineAlert"); } }
	WrappedLabel WlblNotFirstTimeFixedPurchaseAlert { get { return GetWrappedControl<WrappedLabel>("lblNotFirstTimeFixedPurchaseAlert"); } }
	WrappedLabel WlblPaymentAlert { get { return GetWrappedControl<WrappedLabel>("lblPaymentAlert"); } }
	WrappedLabel WlblDeliveryPatternAlert { get { return GetWrappedControl<WrappedLabel>("lblDeliveryPatternAlert"); } }
	WrappedHtmlGenericControl WhgcCompleteMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("hgcCompleteMessage"); } }
	protected WrappedHiddenField WhfAtoneToken { get { return GetWrappedControl<WrappedHiddenField>("hfAtoneToken"); } }
	protected WrappedHiddenField WhfAfteeToken { get { return GetWrappedControl<WrappedHiddenField>("hfAfteeToken"); } }
	protected WrappedLabel WlblPaypayErrorMessage { get { return GetWrappedControl<WrappedLabel>("lblPaypayErrorMessage"); } }
	protected WrappedLiteral WlOrderCombineMessage { get { return GetWrappedControl<WrappedLiteral>("lOrderCombineMessage"); } }
	protected WrappedLiteral WlSubscriptionOrderCombineMessage { get { return GetWrappedControl<WrappedLiteral>("lSubscriptionOrderCombineMessage"); } }
	public WrappedHiddenField WhfPaidyPaymentId { get { return GetWrappedControl<WrappedHiddenField>("hfPaidyPaymentId"); } }
	public WrappedHiddenField WhfPaidySelect { get { return GetWrappedControl<WrappedHiddenField>("hfPaidySelect"); } }
	public WrappedHiddenField WhfPaidyStatus { get { return GetWrappedControl<WrappedHiddenField>("hfPaidyStatus"); } }
	#endregion

    /// <summary>
    /// ページロード
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, System.EventArgs e)
	{
		// LINE PAY画面から遷移した場合（ブラウザバック）画面遷移の正当性チェック用セッションを直す
		if (SessionManager.IsRedirectFromLinePay)
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_CONFIRM;
			SessionManager.IsRedirectFromLinePay = false;
		}
		this.DispNum = 0;

		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);
			CreateCompleteButtonList();
			this.WrCartList.DataSource = this.CartList;

			this.Process.PrepareForDataBindOrderShipping(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);

			CreateFixedPurchaseSettings();

			this.BindingCartList = this.CartList.Items;

			this.WrCartList.DataBind();
			//プレビュー時、AmazonRequestが初期化されず例外が発生するのを防ぐ
			this.AmazonRequest = AmazonCv2Redirect.SignPayloadForOneTime();
			return;
		}

		// 注文同梱されている場合、カート情報を同梱後のカートに変更する
		if (this.IsOrderCombined)
		{
			this.CartList = SessionManager.OrderCombineCartList;
			CreateOrderOwner();
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_CONFIRM;

			this.WlOrderCombineMessage.Visible = true;
			this.WlOrderCombineMessage.Text = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_INHERITED_ORDER_INFO);
			if (this.CartList.Items.Exists(item => item.IsCouponNotApplicableByOrderCombined))
			{
				this.WlSubscriptionOrderCombineMessage.Visible = true;
				this.WlSubscriptionOrderCombineMessage.Text = CouponOptionUtility.GetErrorMessage(CouponErrorcode.CouponNotApplicableByOrderCombined);
			}
		}

		//------------------------------------------------------
		// HTTPS通信チェック（HTTP通信の場合、ショッピングカートへ）
		//------------------------------------------------------
		CheckHttps();

		//------------------------------------------------------
		// カート注文者チェック
		// カート注文者情報とログイン状態に不整合がある場合、配送先入力ページへ戻る（注文者再設定）
		//------------------------------------------------------
		CheckCartOwner();

		// 配送不可エリアチェック
		// 2クリック決済時にも対応するため、確認画面でもチェックする
		CheckCartOwnerShippingArea();
		SessionManager.UnavailableShippingErrorMessage = null;

		// カート存在チェック
		CheckCartExists();

		//------------------------------------------------------
		// 画面遷移の正当性チェック
		//------------------------------------------------------
		if (!IsPostBack)
		{
			CheckOrderUrlSession();
			foreach (var cart in this.CartList.Items)
			{
				if (cart.Payment != null)
				{
					cart.Payment.IsBackFromConfirm = ((cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
						&& (cart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW));
				}
			}
		}

		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			// Adjust point and member rank by Cross Point api
			UserUtility.AdjustPointAndMemberRankByCrossPointApi(this.LoginUser);
		}

		//------------------------------------------------------
		// カート全チェック
		//------------------------------------------------------
		try
		{
			//カート情報セッションに同梱商品が含まれていたら同梱商品を削除
			DeleteBundledProduct();

			if ((this.IsOrderCombined == false) || (this.IsPaymentReselect)) AllCheckCartList();

			// 別タブでカートの編集を行ったことにより、決済情報がブランクとなった場合もカートリストへ遷移させる
			if (this.CartList.Items.Any(cart => cart.Payment == null)) throw new OrderException();
		}
		catch (OrderException)
		{
			Response.Redirect(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
		}

		//------------------------------------------------------
		// トークン有効期限チェック
		//-----------------------------------------------------
		var creditTokenCheck = this.CartList.CheckCreditTokenExpired(false);
		if (creditTokenCheck == false)
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_PAYMENT;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_PAYMENT);
		}

		// Paidy Pay、ECPayは利用可能か
		if ((CheckShippingCountryIsoCodeCanOrderWithPaidyPay(this.CartList) == false)
			|| (CheckCountryIsoCodeCanOrderWithECPay(this.CartList) == false))
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_PAYMENT;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_PAYMENT);
		}

		// Check Country Iso Code Can Order With NP After Pay
		if (CheckCountryIsoCodeCanOrderWithNPAfterPay(this.CartList) == false)
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING);
		}

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// カートリスト情報に決済手数料を紐付け、配送先情報で配送料含め再計算
			//------------------------------------------------------
			foreach (CartObject co in this.CartList.Items)
			{
				// 決済手数料設定
				// 決済手数料は商品合計金額から計算する（ポイント等で割引された金額から計算してはいけない）
				co.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
					co.ShopId,
					co.Payment.PaymentId,
					co.PriceSubtotal,
					co.PriceCartTotalWithoutPaymentPrice);

				// カート配送先情報で再計算
				co.Calculate(false, false, false, true, 0, this.IsOrderCombined);
			}

			//決済金額決定
			foreach (var co in this.CartList.Items)
			{
				co.SettlementCurrency = CurrencyManager.GetSettlementCurrency(co.Payment.PaymentId);
				co.SettlementRate = CurrencyManager.GetSettlementRate(co.SettlementCurrency);
				co.SettlementAmount = CurrencyManager.GetSettlementAmount(co.PriceTotal, co.SettlementRate, co.SettlementCurrency);
			}

			//------------------------------------------------------
			// カートリスト情報を取得
			//------------------------------------------------------
			this.WrCartList.DataSource = this.CartList;

			// データバインド準備
			this.Process.PrepareForDataBindOrderShipping(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);

			// 定期購入カート + 通常注文の注文同梱用に定期購入設定作成
			CreateFixedPurchaseSettings();

			// 別タブでカート内を変更された時用に現在のカート内商品情報を格納する
			this.BindingCartList = this.CartList.Items;

			// データバインド
			this.DataBind();

			if (Constants.TWOCLICK_OPTION_ENABLE)
			{
				try
				{
					var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(this.LoginUserId);
					// デフォルト注文系のエラーチェックをし、異常があれば配送先選択画面またはお支払方法画面へ遷移
					if (userDefaultOrderSetting != null)
					{
						CheckOrderForUserDefaultOrder(
							this.CartList.UserDefaultOrderSettingParm.IsUserDefaultShippingRegister,
							this.CartList.UserDefaultOrderSettingParm.IsUserDefaultPaymentRegister,
							userDefaultOrderSetting);
					}
				}
				catch (OrderException)
				{
					Response.Redirect(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT + (string)Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK]);
				}

				// デフォルト注文方法チェック
				SetCheckBoxForUserDefaultOrderSetting();
			}

			this.WhfPaidySelect.Value = this.CartList.Items.Any(item => item.Payment.IsPaymentPaygentPaidy) ? "True" : "False";
		}

		// 注文完了ボタンリスト作成（ボタンクリック時、ボタン非表示処理向け）
		CreateCompleteButtonList();

		// 変更ボタンの表示切替
		HideChangeButton();

		// 注文同梱でなく、注文同梱での決済再選択を行っていない場合、カートへ戻るボタンおよび注文同梱注意メッセージを非表示
		if ((this.IsOrderCombined == false) && (this.IsPaymentReselect == false))
		{
			this.WlbCart.Visible = false;
			this.WlblOrderCombineAlert.Visible = false;
		}

		var result = CheckPaymentLimit();
		switch (result)
		{
			// 支払い可能
			case CheckPaymentLimitResult.Possible:
				this.WlblPaymentAlert.Visible = false;
				this.WlbComplete1.Visible = true;
				this.WlbComplete2.Visible = true;
				this.WhgcCompleteMessage.Visible = true;
				break;

			// 支払い可能な決済種別なし
			case CheckPaymentLimitResult.Impossible:
				this.WlblPaymentAlert.Visible = true;
				this.WlbComplete1.Visible = false;
				this.WlbComplete2.Visible = false;
				WhgcCompleteMessage.Visible = false;
				break;

			// 現在の決済種別では不可だが別の決済種別で支払い可能
			case CheckPaymentLimitResult.ImpossibleButOtherPaymentMethodIsPossible:
				Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_PAYMENT;
				Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_PAYMENT);
				break;
		}

		// 商品購入制限チェック（類似配送先を含む）
		if (Constants.PRODUCT_ORDER_LIMIT_ENABLED) CheckProductOrderSimilarShipping();

		this.AmazonRequest = this.CartList.HasFixedPurchase
			? AmazonCv2Redirect.SignPayloadForReccuring(this.CartList.PriceCartListTotal)
			: AmazonCv2Redirect.SignPayloadForOneTime();

		// AmazonPayCv2で、通常商品でCheckoutSessionConfigを作ったのに、今のカートに定期商品がある場合(あるいは逆)、もう一度AmazonPayに遷移し同意を取る必要がある
		SessionManager.IsChangedAmazonPayForFixedOrNormal =
			((this.CartList.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
				&& (SessionManager.IsAmazonPayGotRecurringConsent != this.CartList.HasFixedPurchase));

			if (Session[Constants.SESSION_KEY_ERROR_FOR_PAYPAY_PAYMENT] != null)
		{
			this.WlblPaypayErrorMessage.Visible = true;
			var paypayErrMessage = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ERROR_FOR_PAYPAY_PAYMENT]).Split(':');
			var errCode = (paypayErrMessage.Length > 1) ? string.Format("（{0}）", paypayErrMessage[0]) : string.Empty;
			var errMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYPAY_AUTH_ERROR) + errCode;
			this.WlblPaypayErrorMessage.Text = errMessage;
			Session.Remove(Constants.SESSION_KEY_ERROR_FOR_PAYPAY_PAYMENT);
		}
	}

	/// <summary>
	/// 購入商品を過去に購入したことがあるか（類似配送先を含む）
	/// </summary>
	protected void CheckProductOrderSimilarShipping()
	{
		this.CartList.CheckFixedProductOrderLimit();
		if ((Constants.ProductOrderLimitKbn.ProductOrderLimitOff == Constants.PRODUCT_ORDER_LIMIT_KBN_CAN_BUY)
			&& (this.CartList.HasOrderHistorySimilarShipping))
		{
			this.ProductOrderLimitErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_NOT_FIXED_PRODUCT_ORDER_LIMIT);
			this.WlblNotFirstTimeFixedPurchaseAlert.Visible = this.HasOrderHistorySimilarShipping;
			this.WlblNotFirstTimeFixedPurchaseAlert.Text = WebSanitizer.HtmlEncodeChangeToBr(this.ProductOrderLimitErrorMessage);
		}
		else if ((Constants.ProductOrderLimitKbn.ProductOrderLimitOn == Constants.PRODUCT_ORDER_LIMIT_KBN_CAN_BUY)
			&& (this.CartList.HasOrderHistorySimilarShipping))
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
		}
	}

	/// <summary>
	/// デフォルト注文方法系エラーチェック
	/// </summary>
	/// <param name="userDefaultShippingOn">既定の配送先指定あり判定</param>
	/// <param name="userDefaultPaymentOn">既定の支払方法指定あり判定</param>
	/// <param name="userDefaultOrderSetting">デフォルト注文方法情報</param>
	private void CheckOrderForUserDefaultOrder(bool userDefaultShippingOn, bool userDefaultPaymentOn, UserDefaultOrderSettingModel userDefaultOrderSetting)
	{
		// 既に配送系のエラーがある場合は配送先選択画面に遷移してエラーメッセージを表示
		if ((string)Session[Constants.SESSION_KEY_ORDER_ERROR_MESSAGE]
			== CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_DEFAULT_SHIPPING_SETTING_INVALID))
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;
			throw new OrderException();
		}
		// デフォルト配送先指定が無効である場合、配送先選択画面に遷移してエラーメッセージを表示
		if (IsInvaildUserDefaultShipping(userDefaultOrderSetting))
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;
			Session[Constants.SESSION_KEY_ORDER_ERROR_MESSAGE] = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_DEFAULT_SHIPPING_SETTING_INVALID);
			throw new OrderException();
		}

		// デフォルト支払方法でクレジットカードが無効である場合、お支払方法選択画面に遷移してエラーメッセージ表示
		if (IsInvaildUserDefaultPaymentCreditCard(userDefaultOrderSetting))
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_PAYMENT;
			Session[Constants.SESSION_KEY_ORDER_ERROR_MESSAGE] = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_CREDIT_CARD);
			throw new OrderException();
		}
		// メール便が無効であるかの判定
		if (IsInvaildMailDelivery(userDefaultOrderSetting, this.CartList.Items[this.CartList.UserDefaultOrderSettingParm.UserDefaultPaymentCartNo]))
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_PAYMENT;
			Session[Constants.SESSION_KEY_ORDER_ERROR_MESSAGE] = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_COLLECT);
			throw new OrderException();
		}
		// 無効な決済であるかの判定（ユーザー管理レベルの利用不可決済）
		if (IsInvaildPaymentForUserManagementLevel(userDefaultOrderSetting, this.CartList.Items[this.CartList.UserDefaultOrderSettingParm.UserDefaultPaymentCartNo]))
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_PAYMENT;
			Session[Constants.SESSION_KEY_ORDER_ERROR_MESSAGE] = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_USER_DEFAULT_LIMITED_PAYMENT);
			throw new OrderException();
		}
		Session[Constants.SESSION_KEY_ORDER_ERROR_MESSAGE] = null;
	}

	/// <summary>
	/// デフォルト配送先指定が無効であるかの判定
	/// </summary>
	/// <param name="userDefaultOrderSetting">デフォルト注文方法情報</param>
	/// <returns>true：無効、false、有効</returns>
	private bool IsInvaildUserDefaultShipping(UserDefaultOrderSettingModel userDefaultOrderSetting)
	{
		if (userDefaultOrderSetting.UserShippingNo == null) return false;
		if (userDefaultOrderSetting.UserShippingNo == 0) return false;
		var userShipping = new UserShippingService().Get(this.LoginUserId, (int)userDefaultOrderSetting.UserShippingNo);
		return (userShipping == null);
	}

	/// <summary>
	/// デフォルト支払クレジットカードが無効であるか判定
	/// </summary>
	/// <param name="userDefaultOrderSetting">デフォルト注文方法情報</param>
	/// <returns>true：無効、false、有効</returns>
	private bool IsInvaildUserDefaultPaymentCreditCard(UserDefaultOrderSettingModel userDefaultOrderSetting)
	{
		if (userDefaultOrderSetting.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) return false;
		if (userDefaultOrderSetting.CreditBranchNo == null) return false;
		if (this.CartList.Items.Any(cartObj => cartObj.Payment.CreditCardBranchNo != userDefaultOrderSetting.CreditBranchNo.ToString())) return false;
		var userCreditCard = new UserCreditCardService().Get(this.LoginUserId, (int)userDefaultOrderSetting.CreditBranchNo);
		var isInvaildUserDefaultPaymentCreditCard = (((userCreditCard == null) || (userCreditCard.DispFlg == Constants.FLG_USERCREDITCARD_DISP_FLG_OFF))
			&& (string.IsNullOrEmpty(userCreditCard.CardDispName) == false));
		return isInvaildUserDefaultPaymentCreditCard;
	}

	/// <summary>
	/// カートに決済利用不可の商品が含まれているかの判定
	/// </summary>
	/// <param name="userDefaultOrderSetting">デフォルト注文方法情報</param>
	/// <returns>true：無効、false：有効</returns>
	private bool IsContainsLimitedPayment(UserDefaultOrderSettingModel userDefaultOrderSetting)
	{
		if (userDefaultOrderSetting.PaymentId == null) return false;
		var isContainsLimitedPayment = (this.CartList.Cast<CartObject>()
			.Any(cartObj => cartObj.Cast<CartProduct>().Any(cartProduct => cartProduct.LimitedPaymentIds.Contains(userDefaultOrderSetting.PaymentId))));
		return isContainsLimitedPayment;
	}

	/// <summary>
	/// メール便が無効であるかの判定
	/// </summary>
	/// <param name="userDefaultOrderSetting">デフォルト注文方法情報</param>
	/// <param name="cartObj">カートオブジェクト</param>
	/// <returns>true：無効、false、有効</returns>
	private bool IsInvaildMailDelivery(UserDefaultOrderSettingModel userDefaultOrderSetting, CartObject cartObj)
	{
		if (userDefaultOrderSetting.PaymentId == null) return false;
		var isInvaildMailDelivery = ((userDefaultOrderSetting.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
			&& ((cartObj.IsExpressDelivery == false) && (userDefaultOrderSetting.PaymentId == cartObj.Payment.PaymentId)));

		return isInvaildMailDelivery;
	}

	/// <summary>
	/// 無効な決済であるかの判定（ユーザー管理レベルの利用不可決済）
	/// </summary>
	/// <param name="userDefaultOrderSetting">デフォルト注文方法情報</param>
	/// <param name="cartObj">カートオブジェクト</param>
	/// <returns>true：無効、false、有効</returns>
	private bool IsInvaildPaymentForUserManagementLevel(UserDefaultOrderSettingModel userDefaultOrderSetting, CartObject cartObj)
	{
		if (userDefaultOrderSetting.PaymentId == null) return false;

		var user = new UserService().Get(this.LoginUserId);
		if (user == null) return false;

		var payment = DataCacheControllerFacade.GetPaymentCacheController().Get(userDefaultOrderSetting.PaymentId);
		var isInvaildPaymentForUserManagementLevel = payment.UserManagementLevelNotUse.Split(',')
			.Any(userManagementLevelNotUse
				=> ((userManagementLevelNotUse == user.UserManagementLevelId) && (userDefaultOrderSetting.PaymentId == cartObj.Payment.PaymentId)));

		return isInvaildPaymentForUserManagementLevel;
	}

	/// <summary>
	/// 注文完了ボタンリスト作成
	/// </summary>
	/// <remarks>データバインド後に実行する必要がある</remarks>
	private void CreateCompleteButtonList()
	{
		this.CompleteButtonList = new List<LinkButton>();
		if (WlbComplete1.InnerControl != null) this.CompleteButtonList.Add(WlbComplete1.InnerControl);
		if (WlbComplete2.InnerControl != null) this.CompleteButtonList.Add(WlbComplete2.InnerControl);
		if (this.WrCartList.Items.Count >= 1)
		{
			LinkButton completeButton1 = (LinkButton)this.WrCartList.Items[this.WrCartList.Items.Count - 1].FindControl("lbComplete1");
			LinkButton completeButton2 = (LinkButton)this.WrCartList.Items[this.WrCartList.Items.Count - 1].FindControl("lbComplete2");

			if (completeButton1 != null) this.CompleteButtonList.Add(completeButton1);
			if (completeButton2 != null) this.CompleteButtonList.Add(completeButton2);
		}
	}

	/// <summary>
	/// 注文を確定するリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbComplete_Click(object sender, System.EventArgs e)
	{
		var paygentCart = this.CartList.Items.FirstOrDefault(item => item.Payment.IsPaymentPaygentPaidy);
		if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
			&& (paygentCart != null))
		{
			if (this.WhfPaidyStatus.Value == Constants.FLG_PAYMENT_PAIDY_API_STATUS_REJECTED)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] =
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_FAIL_PAIDY_AUTHORIZED);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
			else
			{
				paygentCart.Payment.CardTranId = this.WhfPaidyPaymentId.Value;
			}
		}

		// 再度カート存在チェック
		CheckCartExists(MessageManager.GetMessages(WebMessages.ERRMSG_FRONT_CART_ALREADY_ORDERED));

		// メール便かつ決済種別が代引きではないかチェック
		CheckPaymentAvailableShipping();

		// カート内の商品が別タブで変更されていないかチェック
		// 変更されていた場合は自身のページにリダイレクト
		if (CheckCartListToBindingData() == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG_FOR_CHANGE_CART] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_NO_ADJUSTMENT);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
		}

		// LPカートのセッションキーを持っているとLPカートと判断されてしまうのでクリアしておく
		Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY] = null;

		// 注文同梱されている場合、
		if (this.IsOrderCombined)
		{
			foreach (var cart in this.CartList.Items)
			{
				// 注文同梱元親注文に定期購入がなく、カートに定期購入があった場合、定期配送情報をセットする。セットできない場合処理中止。
				if ((cart.IsCombineParentOrderHasFixedPurchase == false) && (cart.IsBeforeCombineCartHasFixedPurchase))
				{
					if (SetCombinedCartFixedPurchaseDeliveryPattern(this.CartList.Items[0]) == false) return;
				}

				if (cart.Shippings.Any(cs => cs.IsShippingConvenience)
					&& OrderCommon.CheckValidWeightAndPriceForConvenienceStore(
						this.CartList.Items[0],
						this.CartList.Items[0].Shippings[0].ShippingReceivingStoreType))
				{
					var errorLimitWeight = (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
						&& ECPayUtility.CheckShippingReceivingStoreType7Eleven(this.CartList.Items[0].Shippings[0].ShippingReceivingStoreType))
							? Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG[1].ToString()
							: Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG[0].ToString();
					Session[Constants.SESSION_KEY_ERROR_MSG] =
						WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SHIPPING_CONVENIENCE_STORE)
							.Replace("@@ 1 @@", CurrencyManager.ToPrice(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE))
							.Replace("@@ 2 @@", errorLimitWeight);

					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}
		}

		// 定期注文が含まれる場合、配送パターンが設定されているか再確認
		foreach (var cart in this.CartList.Items)
		{
			if (cart.HasFixedPurchase
				&& cart.Shippings.Any(s => string.IsNullOrEmpty(s.FixedPurchaseSetting)))
			{
				var redirectUrl = "";
				if (Constants.GIFTORDER_OPTION_ENABLED)
				{
					// 配送先、配送方法画面へ
					redirectUrl = Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING;
				}
				else
				{
					// 本人情報入力画面へ
					redirectUrl = Constants.PAGE_FRONT_ORDER_SHIPPING;
					if (Constants.TWOCLICK_OPTION_ENABLE)
					{
						this.CartList.UserDefaultOrderSettingParm.IsChangedUserDefaultShipping = true;
						this.CartList.UserDefaultOrderSettingParm.IsChangedUserDefaultInvoice = true;
					}
				}

				// 画面遷移の正当性チェックのため遷移先ページURLを設定
				Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = redirectUrl;

				// 画面遷移
				Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + redirectUrl);
			}

			// FLAPS在庫チェック
			if (Constants.FLAPS_OPTION_ENABLE)
			{
				foreach (var cartGoods in cart.Items)
				{
					var hasStock = new FlapsIntegrationFacade().HasStock(cartGoods.VariationId, cartGoods.Count);
					if (hasStock) continue;
					
					FileLogger.WriteError(
						string.Format(
							"注文数分の在庫がありません。状況に応じて、FLAPS管理画面で在庫を増やしてください。variation_id: {0}, quantity: {1}",
							cartGoods.VariationId,
							cartGoods.Count));
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(
						WebMessages.ERRMSG_FRONT_PRODUCT_NO_STOCK,
						new[] { cartGoods.ProductJointName, });
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}
		}

		// 配送価格未設定エラーが出ているか確認
		CheckGlobalShippingPriceCalcError();

		//ユーザーが退会済みでないか確認
		if (this.LoginUserId != null)
		{
			var user = new UserService().Get(this.LoginUserId);
			if (user.IsDeleted)
			{
				Session.Contents.RemoveAll();
				CookieManager.RemoveCookie(Constants.COOKIE_KEY_AUTH_KEY, Constants.PATH_ROOT);
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_SESSION_VANISHED);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}
		}

		lock (this.CartList)
		{
			// ギフト購入したもののカート商品と配送先情報に紐づく商品情報の整合性チェック
			if (IsConsistentGiftItemsAndShippings(this.CartList) == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_NO_ADJUSTMENT);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// デフォルト注文方法設定フラグをセットする
			if (Constants.TWOCLICK_OPTION_ENABLE) SetUserDefaultOrderSettingFlg();

			// 通常の配送先・支払方法に設定するのチェック状況をクリアする
			this.IsDefaultShippingChecked = null;
			this.IsDefaultPaymentChecked = null;
			this.IsDefaultInvoiceChecked = null;

			// 注文実行（リダイレクト）
			ExecOrder();
		}
	}

	/// <summary>
	/// 通常の配送方法に設定するチェックボックス変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbDefaultShipping_OnCheckedChanged(object sender, System.EventArgs e)
	{
		ChangeCheckBoxForUserDefaultShippingSetting(sender);
	}

	/// <summary>
	/// 通常の支払方法に設定するチェックボックス変更イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbDefaultPayment_OnCheckedChanged(object sender, System.EventArgs e)
	{
		ChangeCheckBoxForUserDefaultPaymentSetting(sender);
	}

	/// <summary>
	/// Check Box Default Invoice Checked Changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbDefaultInvoice_CheckedChanged(object sender, EventArgs e)
	{
		WrappedCheckBox wcbDefaultInvoice = null;
		var senderControl = (CheckBox)sender;
		if (senderControl.Checked)
		{
			foreach (RepeaterItem cart in WrCartList.Items)
			{
				var wrCartShippings = GetWrappedControl<WrappedRepeater>(cart, "rCartShippings");
				foreach (RepeaterItem shipping in wrCartShippings.Items)
				{
					wcbDefaultInvoice = GetWrappedControl<WrappedCheckBox>(shipping, "cbDefaultInvoice");
				}
				if (wcbDefaultInvoice.ClientID != senderControl.ClientID)
				{
					wcbDefaultInvoice.Checked = false;
				}
			}
		}
	}

	/// <summary>
	/// 通常の配送方法に設定するチェックボックス変更時の処理
	/// </summary>
	/// <param name="sender"></param>
	private void ChangeCheckBoxForUserDefaultShippingSetting(object sender)
	{
		WrappedCheckBox wcbDefaultShipping = null;
		foreach (RepeaterItem cl in WrCartList.Items)
		{
			var wrCartShippings = GetWrappedControl<WrappedRepeater>(cl, "rCartShippings");
			foreach (RepeaterItem cs in wrCartShippings.Items)
			{
				wcbDefaultShipping = GetWrappedControl<WrappedCheckBox>(cs, "cbDefaultShipping");
			}
		}

		var senderControl = (CheckBox)sender;
		if (senderControl.Checked)
		{
			foreach (RepeaterItem cl in WrCartList.Items)
			{
				var wrCartShippings = GetWrappedControl<WrappedRepeater>(cl, "rCartShippings");
				foreach (RepeaterItem cs in wrCartShippings.Items)
				{
					wcbDefaultShipping = GetWrappedControl<WrappedCheckBox>(cs, "cbDefaultShipping");
					if (wcbDefaultShipping.ClientID != senderControl.ClientID) wcbDefaultShipping.Checked = false;
				}
			}
		}
	}

	/// <summary>
	/// 通常の支払方法に設定するチェックボックス変更時の処理
	/// </summary>
	/// <param name="sender"></param>
	private void ChangeCheckBoxForUserDefaultPaymentSetting(object sender)
	{
		WrappedCheckBox wcbDefaultPayment = null;
		foreach (RepeaterItem cl in WrCartList.Items)
		{
			var wrCartShippings = GetWrappedControl<WrappedRepeater>(cl, "rCartShippings");
			wcbDefaultPayment = GetWrappedControl<WrappedCheckBox>(cl, "cbDefaultPayment");
		}

		var senderControl = (CheckBox)sender;
		if (senderControl.Checked)
		{
			foreach (RepeaterItem cl in WrCartList.Items)
			{
				var wrCartShippings = GetWrappedControl<WrappedRepeater>(cl, "rCartShippings");
				wcbDefaultPayment = GetWrappedControl<WrappedCheckBox>(cl, "cbDefaultPayment");
				if (wcbDefaultPayment.ClientID != senderControl.ClientID) wcbDefaultPayment.Checked = false;
			}
		}
	}

	/// <summary>
	/// デフォルト注文方法設定フラグをセットする
	/// </summary>
	private void SetUserDefaultOrderSettingFlg()
	{
		WrappedCheckBox wcbDefaultShipping = null;
		WrappedCheckBox wcbDefaultPayment = null;
		var isUserDefaultShippingRegister = false;
		var isUserDefaultPaymentRegister = false;
		this.IsDefaultShippingChecked = false;
		this.IsDefaultPaymentChecked = false;

		WrappedCheckBox wcbDefaultInvoice = null;
		var isUserDefaultInvoiceRegister = false;
		this.IsDefaultInvoiceChecked = false;
		this.CheckedItemNumberForMultipleDeliveryAddresses = 0;

		var userDefaultShippingCartNo = 0;
		foreach (RepeaterItem cl in WrCartList.Items)
		{
			var wrCartShippings = GetWrappedControl<WrappedRepeater>(cl, "rCartShippings");
			foreach (RepeaterItem cs in wrCartShippings.Items)
			{
				wcbDefaultShipping = GetWrappedControl<WrappedCheckBox>(cs, "cbDefaultShipping");
				if (wcbDefaultShipping.Checked)
				{
					isUserDefaultShippingRegister = true;
					this.IsDefaultShippingChecked = true;
					this.CartList.UserDefaultOrderSettingParm.UserDefaultShippingCartNo = userDefaultShippingCartNo;
					break;
				}
				else
				{
					this.CheckedItemNumberForMultipleDeliveryAddresses++;
				}
			}
			userDefaultShippingCartNo++;

			// チェックされているものを見つけたらさっさと抜ける
			if (isUserDefaultShippingRegister)
			{
				break;
			}
			// 別カートになった場合配送先Noをリセット
			else
			{
				this.CheckedItemNumberForMultipleDeliveryAddresses = 0;
			}
		}

		var userDefaultPaymentCartNo = 0;
		foreach (RepeaterItem cl in WrCartList.Items)
		{
			wcbDefaultPayment = GetWrappedControl<WrappedCheckBox>(cl, "cbDefaultPayment");
			if (wcbDefaultPayment.Checked)
			{
				isUserDefaultPaymentRegister = true;
				this.IsDefaultPaymentChecked = true;
				this.CartList.UserDefaultOrderSettingParm.UserDefaultPaymentCartNo = userDefaultPaymentCartNo;
				break;
			}
			userDefaultPaymentCartNo++;
		}

		var userDefaultInvoiceCartNo = 0;
		foreach (RepeaterItem cart in WrCartList.Items)
		{
			var wrCartShippings = GetWrappedControl<WrappedRepeater>(cart, "rCartShippings");
			foreach (RepeaterItem item in wrCartShippings.Items)
			{
				wcbDefaultInvoice = GetWrappedControl<WrappedCheckBox>(item, "cbDefaultInvoice");
				if (wcbDefaultInvoice.Checked)
				{
					isUserDefaultInvoiceRegister = true;
					this.IsDefaultInvoiceChecked = true;
					this.CartList.UserDefaultOrderSettingParm.UserDefaultInvoiceNo = userDefaultInvoiceCartNo;
					break;
				}
			}
			userDefaultInvoiceCartNo++;
		}

		this.CartList.UserDefaultOrderSettingParm.IsUserDefaultShippingRegister = isUserDefaultShippingRegister;
		this.CartList.UserDefaultOrderSettingParm.IsUserDefaultPaymentRegister = isUserDefaultPaymentRegister;
		this.CartList.UserDefaultOrderSettingParm.IsUserDefaultInvoiceRegister = isUserDefaultInvoiceRegister;
	}

	/// <summary>
	/// デフォルト注文方法指定のチェックボックスを設定
	/// </summary>
	protected void SetCheckBoxForUserDefaultOrderSetting()
	{
		var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(this.LoginUserId);
		var paymentId = string.Empty;
		var shippingNo = string.Empty;
		TwUserInvoiceModel invoice = null;

		// デフォルト注文方法情報が存在する場合、デフォルト設定の決済種別ID、配送先枝番を取得
		if (userDefaultOrderSetting != null)
		{
			if (userDefaultOrderSetting.PaymentId != null) paymentId = userDefaultOrderSetting.PaymentId;
			if (userDefaultOrderSetting.UserShippingNo != null) shippingNo = userDefaultOrderSetting.UserShippingNo.ToString();
			if (userDefaultOrderSetting.UserInvoiceNo.HasValue)
			{
				invoice = new TwUserInvoiceService().Get(this.LoginUserId, userDefaultOrderSetting.UserInvoiceNo.Value);
			}
		}

		// デフォルト用ラジオボタン値取得
		var isDefaultShippingChecked = false;
		var isDefaultInvoiceChecked = false;
		var isDefaultPaymentChecked = false;
		foreach (RepeaterItem cl in WrCartList.Items)
		{
			var wrCartShippings = GetWrappedControl<WrappedRepeater>(cl, "rCartShippings");
			foreach (RepeaterItem cs in wrCartShippings.Items)
			{
				var wcbDefaultShipping = GetWrappedControl<WrappedCheckBox>(cs, "cbDefaultShipping");
				var wcbDefaultInvoice = GetWrappedControl<WrappedCheckBox>(cs, "cbDefaultInvoice");

				// cbDefaultShippingがある場合のみチェックをオンにする。
				// （コントロールを削除しているときはチェックを付けたくないため）
				if (wcbDefaultShipping.HasInnerControl)
				{
					// 配送先入力で入力し、配送先情報を保存しない設定の場合非表示
					if ((this.CartList.Items[cl.ItemIndex].Shippings[cs.ItemIndex].ShippingAddrKbn
						== CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW) && this.CartList.Items[cl.ItemIndex]
						.Shippings[cs.ItemIndex].UserShippingRegistFlg == false)
					{
						wcbDefaultShipping.Visible = false;
						continue;
					}

					if (isDefaultShippingChecked == false)
					{
						// this.IsDefaultShippingChecked.HasValueがTrueならこの画面を離れる前のチェック状態にする Falseならデフォルトの配送先の場合チェックする
						isDefaultShippingChecked = this.IsDefaultShippingChecked.HasValue
							? (this.IsDefaultShippingChecked.Value && (cl.ItemIndex
								== this.CartList.UserDefaultOrderSettingParm.UserDefaultShippingCartNo))
							: ((this.CartList.Items[cl.ItemIndex].Shippings[cs.ItemIndex].ShippingAddrKbn
									== CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER)
								|| ((string.IsNullOrEmpty(shippingNo) == false)
									&& (this.CartList.Items[cl.ItemIndex].Shippings[cs.ItemIndex].ShippingNo
										== shippingNo)));
						wcbDefaultShipping.Checked = isDefaultShippingChecked;
					}

					if (this.CartList.Items[cl.ItemIndex].Shippings[cs.ItemIndex].ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP)
					{
						wcbDefaultShipping.Checked = false;
						wcbDefaultShipping.Visible = false;
					}
				}

				// 電子発票情報入力で入力し、電子発票情報を保存しない設定の場合非表示
				var shipping = this.CartList.Items[cl.ItemIndex].Shippings[cs.ItemIndex];
				if ((this.CartList.Items[cl.ItemIndex].Shippings[cs.ItemIndex].UserInvoiceRegistFlg == false)
					&& ((invoice != null)
						&& (invoice.TwCarryTypeOption != shipping.CarryTypeOptionValue))
					|| ((string.IsNullOrEmpty(shipping.InvoiceName)
						&& string.IsNullOrEmpty(shipping.CarryTypeOptionValue))))
				{
					wcbDefaultInvoice.Visible = false;
					continue;
				}

				if (isDefaultInvoiceChecked == false)
				{
					// this.IsDefaultInvoiceChecked.HasValueがTrueならこの画面を離れる前のチェック状態にする Falseならデフォルトの電子発表の場合チェックする
					isDefaultInvoiceChecked = this.IsDefaultInvoiceChecked.HasValue
						? (this.IsDefaultInvoiceChecked.Value
							&& (cl.ItemIndex == this.CartList.UserDefaultOrderSettingParm.UserDefaultInvoiceNo))
						: ((invoice != null)
							&& (shipping.UniformInvoiceType == invoice.TwUniformInvoice)
							&& (shipping.UniformInvoiceOption1 == invoice.TwUniformInvoiceOption1)
							&& (shipping.UniformInvoiceOption2 == invoice.TwUniformInvoiceOption2)
							&& (shipping.CarryType == invoice.TwCarryType)
							&& (shipping.CarryTypeOptionValue == invoice.TwCarryTypeOption));
					wcbDefaultInvoice.Checked = isDefaultInvoiceChecked;
				}
			}

			// 支払方法がクレジットカードかつ新規クレジットカード登録しない場合はデフォルト支払設定チェックボックスを表示しないようにする
			var cartPaymentId = this.CartList.Items[cl.ItemIndex].Payment.PaymentId;
			var userCreditCardRegistFlg = this.CartList.Items[cl.ItemIndex].Payment.UserCreditCardRegistFlg;
			var isNewCreditCard = (this.CartList.Items[cl.ItemIndex].Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW);
			var wcbDefaultPayment = GetWrappedControl<WrappedCheckBox>(cl, "cbDefaultPayment");
			wcbDefaultPayment.Visible = (((cartPaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT) && (userCreditCardRegistFlg == false) && isNewCreditCard) == false);

			if (isDefaultPaymentChecked == false)
			{
				isDefaultPaymentChecked = this.IsDefaultPaymentChecked.HasValue
					? (this.IsDefaultPaymentChecked.Value
						&& (cl.ItemIndex == this.CartList.UserDefaultOrderSettingParm.UserDefaultPaymentCartNo))
					: ((string.IsNullOrEmpty(paymentId) == false) &&
						(this.CartList.Items[cl.ItemIndex].Payment.PaymentId == paymentId));
				wcbDefaultPayment.Checked = isDefaultPaymentChecked;
			}
		}
	}

	/// <summary>
	/// この項目を変更するリンククリック
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rCartShippings_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		rCartList_ItemCommand(source, e);
	}
	/// <summary>
	/// この項目を変更するリンククリック
	/// </summary>
	/// <param name="source"></param>
	/// <param name="e"></param>
	protected void rCartList_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		Hashtable htParam = new Hashtable();
		
		if(Constants.TWOCLICK_OPTION_ENABLE) SetUserDefaultOrderSettingFlg();
		
		if (e.CommandName == "GotoShipping")
		{
			string strPage = null;
			if (this.CartList.Items.Any(cart => OrderCommon.IsAmazonPayment(cart.Payment.PaymentId)))
			{
				// Amazonペイメント入力画面へ
				strPage = Constants.PAGE_FRONT_ORDER_AMAZON_PAYMENT_INPUT;
			}
			else if ((Constants.GIFTORDER_OPTION_ENABLED)
				&& ((string)e.CommandArgument == "Shipping"))
			{
				// 配送先、配送方法画面へ
				strPage = Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING;
			}
			else if ((Constants.GIFTORDER_OPTION_ENABLED)
				&& ((string)e.CommandArgument == "Shipping2"))
			{
				strPage = Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT_PRODUCT;
			}
			else
			{
				// 本人情報入力画面へ
				strPage = Constants.PAGE_FRONT_ORDER_SHIPPING;
				this.CartList.UserDefaultOrderSettingParm.IsChangedUserDefaultShipping = true;
				this.CartList.UserDefaultOrderSettingParm.IsChangedUserDefaultInvoice = true;
			}

			//------------------------------------------------------
			// 画面遷移
			//------------------------------------------------------
			// 画面遷移の正当性チェックのため遷移先ページURLを設定
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = strPage;

			// 画面遷移
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + strPage);
		}
		else if (e.CommandName == "GotoPayment")
		{
			var page = "";

			if (this.CartList.Items.Any(cart => (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				|| (cart.Payment.PaymentId  == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)))
			{
				// Amazonペイメント入力画面へ
				page = Constants.PAGE_FRONT_ORDER_AMAZON_PAYMENT_INPUT;
			}
			else
			{
				//------------------------------------------------------
				// 支払い方法画面へ
				//------------------------------------------------------
				page = Constants.PAGE_FRONT_ORDER_PAYMENT;
				this.CartList.UserDefaultOrderSettingParm.IsChangedUserDefaultPayment = true;
			}

			// 画面遷移の正当性チェックのため遷移先ページURLを設定
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = page;

			// 画面遷移
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + page);
		}
	}

	/// <summary>
	/// 変更するボタン表示制御
	/// </summary>
	protected void HideChangeButton()
	{
		foreach (RepeaterItem rCart in this.WrCartList.Items)
		{
			var whgcChangeUserInfoBtn = GetWrappedControl<WrappedHtmlGenericControl>(rCart, "hgcChangeUserInfoBtn");
			var whgcChangePaymentInfoBtn = GetWrappedControl<WrappedHtmlGenericControl>(rCart, "hgcChangePaymentInfoBtn");
			var whgcChangeCartInfoBtn = GetWrappedControl<WrappedHtmlGenericControl>(rCart, "hgcChangeCartInfoBtn");

			if (this.IsOrderCombined)
			{
				whgcChangeUserInfoBtn.Visible = false;
				whgcChangePaymentInfoBtn.Visible = false;
				whgcChangeCartInfoBtn.Visible = false;

				if (this.IsSmartPhone)
				{
					var whgcChangeFixedPurchaseShippingInfoBtn = GetWrappedControl<WrappedHtmlGenericControl>(rCart, "hgcChangeFixedPurchaseShippingInfoBtn");
					whgcChangeFixedPurchaseShippingInfoBtn.Visible = false;
				}

				var wrShippings = GetWrappedControl<WrappedRepeater>(rCart, "rCartShippings");
				foreach (RepeaterItem rShipping in wrShippings.Items)
				{
					var whgcChangeShippingInfoBtn = GetWrappedControl<WrappedHtmlGenericControl>(rShipping, "hgcChangeShippingInfoBtn");
					whgcChangeShippingInfoBtn.Visible = false;

					if (this.IsSmartPhone == false)
					{
						var whgcChangeFixedPurchaseShippingInfoBtn = GetWrappedControl<WrappedHtmlGenericControl>(rShipping, "hgcChangeFixedPurchaseShippingInfoBtn");
						whgcChangeFixedPurchaseShippingInfoBtn.Visible = false;
					}
				}
			}
		}
	}

	/// <summary>
	/// 注文同梱済みカートに入力された定期購入配送情報を設定
	/// </summary>
	/// <param name="combinedCart">注文同梱済みカート</param>
	/// <returns>定期購入配送周期の未入力有無</returns>
	private bool SetCombinedCartFixedPurchaseDeliveryPattern(CartObject combinedCart)
	{
		foreach (RepeaterItem rCart in this.WrCartList.Items)
		{
			// 月間隔日付指定
			var wrbFpMonthly = GetWrappedControl<WrappedRadioButton>(rCart, "rbFixedPurchaseMonthlyPurchase_Date");
			var wddlFpMonthlyMonth = GetWrappedControl<WrappedDropDownList>(rCart, "ddlFixedPurchaseMonth");
			var wddlFpMonthlyDay = GetWrappedControl<WrappedDropDownList>(rCart, "ddlFixedPurchaseMonthlyDate");

			// 月間隔・週・曜日指定
			var wrbFpWeekAndDay = GetWrappedControl<WrappedRadioButton>(rCart, "rbFixedPurchaseMonthlyPurchase_WeekAndDay");
			var wddlFpIntervalMonths = GetWrappedControl<WrappedDropDownList>(rCart, "ddlFixedPurchaseIntervalMonths");
			var wddlFpWeekAndDayDayOfMonth = GetWrappedControl<WrappedDropDownList>(rCart, "ddlFixedPurchaseWeekOfMonth");
			var wddlFpWeekAndDayDayOfWeek = GetWrappedControl<WrappedDropDownList>(rCart, "ddlFixedPurchaseDayOfWeek");

			// 配送日間隔指定
			var wrbFpInterval = GetWrappedControl<WrappedRadioButton>(rCart, "rbFixedPurchaseRegularPurchase_IntervalDays");
			var wddlFpIntervalDays = GetWrappedControl<WrappedDropDownList>(rCart, "ddlFixedPurchaseIntervalDays");

			// 週間隔・曜日指定
			var wrbFpEveryNWeek = GetWrappedControl<WrappedRadioButton>(rCart, "rbFixedPurchaseEveryNWeek");
			var wddlFpEveryNWeekWeek = GetWrappedControl<WrappedDropDownList>(rCart, "ddlFixedPurchaseEveryNWeek_Week");
			var wddlFpEveryNWeekDayOfWeek = GetWrappedControl<WrappedDropDownList>(rCart, "ddlFixedPurchaseEveryNWeek_DayOfWeek");

			// ラジオボタンの選択がない、ラジオボタンに紐づくドロップダウンリストの選択がない場合、アラートメッセージを表示し処理終了
			if (((wrbFpMonthly.Checked || wrbFpWeekAndDay.Checked || wrbFpInterval.Checked || wrbFpEveryNWeek.Checked) == false)
				|| IsInputedFixedPurchaseDeliveryPattern() == false)
			{
				this.WlblDeliveryPatternAlert.Visible = true;
				return false;
			}

			// 定期購入区分設定
			var fixedPurchaseKbn = wrbFpMonthly.Checked
				? Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE
				: wrbFpWeekAndDay.Checked
					? Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY
					: wrbFpInterval.Checked
						? Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS
						: wrbFpEveryNWeek.Checked
							? Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY
							: string.Empty;

			// 指定した月間隔値を取得
			var intervalMonth = ((wrbFpMonthly.Checked && string.IsNullOrEmpty(wddlFpMonthlyMonth.SelectedValue))
					|| (wrbFpWeekAndDay.Checked && string.IsNullOrEmpty(wddlFpIntervalMonths.SelectedValue)))
				? "1"
				: wrbFpMonthly.Checked
					? wddlFpMonthlyMonth.SelectedValue
					: wddlFpIntervalMonths.SelectedValue;

			// 定期購入設定
			var fixedPurchaseSetting = wrbFpMonthly.Checked
				? string.Format("{0},{1}", intervalMonth, wddlFpMonthlyDay.SelectedValue)
				: wrbFpWeekAndDay.Checked
					? string.Format(
						"{0},{1},{2}",
						intervalMonth,
						wddlFpWeekAndDayDayOfMonth.SelectedValue,
						wddlFpWeekAndDayDayOfWeek.SelectedValue)
					: wrbFpInterval.Checked
						? wddlFpIntervalDays.SelectedValue
						: wrbFpEveryNWeek.Checked
							? string.Format(
								"{0},{1}",
								wddlFpEveryNWeekWeek.SelectedValue,
								wddlFpEveryNWeekDayOfWeek.SelectedValue)
							: string.Empty;

			// 次回配送予定日をカートに格納
			var wddlNextShippingDate = GetWrappedControl<WrappedDropDownList>(rCart, "ddlNextShippingDate");
			DateTime? nextShippingDate = null;
			if (wddlNextShippingDate.InnerControl != null)
			{
				nextShippingDate = DateTime.Parse(wddlNextShippingDate.SelectedValue);
			}

			FixedPurchaseCombineUtility.UpdateCombinedCartFixedPurchaseDeliveryInfo(combinedCart, fixedPurchaseKbn, fixedPurchaseSetting, nextShippingDate);
		}

		this.WlblDeliveryPatternAlert.Visible = false;
		return true;
	}

	/// <summary>
	/// 定期購入 配送パターン 入力チェック
	/// </summary>
	/// <returns>入力エラー有無 True:エラーなし False:エラーあり</returns>
	private bool IsInputedFixedPurchaseDeliveryPattern()
	{
		var result = true;

		var validatorList = new List<CustomValidator>();
		CreateCustomValidators(this, validatorList);
		foreach (var validator in validatorList)
		{
			var validationName = "OrderShipping";
			var control = validator.Parent.FindControl(validator.ControlToValidate);

			// 表示されているコントロールのみ対象とする
			if (control.Visible == false) continue;

			var controlValue = Request[control.UniqueID];
			var message = Validator.ValidateControl(validationName, validator.ControlToValidate, controlValue);

			result = (string.IsNullOrEmpty(message)) ? result : false;

			validator.ErrorMessage = message;
			validator.IsValid = string.IsNullOrEmpty(message);
			var cssClassValue = (string.IsNullOrEmpty(message)) ? "" : Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING;
			if (control is WebControl)
			{
				var webControl = (WebControl)control;
				webControl.CssClass.Replace(Constants.CONST_INPUT_ERROR_CSS_CLASS_STRING, "");
				webControl.CssClass += cssClassValue;
			}
			else if (control is HtmlControl)
			{
				var htmlControl = (HtmlControl)control;
				htmlControl.Attributes.Add("class", "");
				htmlControl.Attributes.Add("class", cssClassValue);
			}
		}

		return result;
	}

	/// <summary>
	/// 決済上限金額チェック
	/// </summary>
	/// <returns>チェック結果</returns>
	private CheckPaymentLimitResult CheckPaymentLimit()
	{
		if (this.CartList.Items.Count == 0) return CheckPaymentLimitResult.Impossible;

		var validPaymentList = OrderCommon.GetValidPaymentList(
			this.CartList.Items[0],
			this.LoginUserId,
			isMultiCart: this.CartList.IsMultiCart);
		var unLimitPayments = OrderCommon.GetPaymentsUnLimitByProduct(this.CartList.Items[0], validPaymentList);

		if (unLimitPayments.Length == 0) return CheckPaymentLimitResult.Impossible;

		var cart = this.CartList.Items[0];
		var payment = OrderCommon.GetPayment(cart.ShopId, cart.Payment.PaymentId);

		if (payment[Constants.FIELD_PAYMENT_USABLE_PRICE_MAX] != DBNull.Value)
		{
			var usablePriceMax = (decimal)payment[Constants.FIELD_PAYMENT_USABLE_PRICE_MAX];
			if (cart.PriceCartTotalWithoutPaymentPrice > usablePriceMax) return CheckPaymentLimitResult.ImpossibleButOtherPaymentMethodIsPossible;
		}

		return CheckPaymentLimitResult.Possible;
	}

	/// <summary>
	/// Get Index Cart Having Payment Atone Or Aftee
	/// </summary>
	/// <param name="isAtone">Is Atone</param>
	/// <returns>List Index</returns>
	[WebMethod]
	public static string GetIndexCartHavingPaymentAtoneOrAftee(bool isAtone)
	{
		var cartList = SessionSecurityManager.GetCartObjectList(
			HttpContext.Current,
			Constants.FLG_ORDER_ORDER_KBN_PC);
		if (cartList == null) return string.Empty;

		var indexs = new List<int>();
		var paymentId = (isAtone
			? Constants.FLG_PAYMENT_PAYMENT_ID_ATONE
			: Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE);
		foreach (var cart in cartList.Items)
		{
			if ((cart.Payment.PaymentId == paymentId)
				&& (string.IsNullOrEmpty(cart.Payment.CardTranId)))
			{
				indexs.Add(cartList.Items.IndexOf(cart));
			}
		}
		var result = JsonConvert.SerializeObject(new { indexs = indexs });
		return result;
	}

	/// <summary>
	/// 配送パターン入力欄の表示が必要か判定
	/// </summary>
	/// <param name="cart">カート情報</param>
	/// <returns>配送パターン入力欄表示有無</returns>
	protected bool IsShowDeliveryPatternInputArea(CartObject cart)
	{
		// 注文同梱されていない場合、表示不要
		if (this.IsOrderCombined == false) return false;

		// 注文同梱前のカートに定期購入商品が含まれ、注文同梱元注文に定期購入商品が含まれない場合、定期購入の配送パターンが表示されないため入力欄を表示
		return ((cart.IsBeforeCombineCartHasFixedPurchase) && (cart.IsCombineParentOrderHasFixedPurchase == false));
	}

	/// <summary>
	/// カートへ戻るリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCart_Click(object sender, EventArgs e)
	{
		SessionManager.OrderCombineCartList = null;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
	}

	/// <summary>
	/// 決済金額取得
	/// </summary>
	/// <param name="cart">カート</param>
	/// <returns>決済金額合計</returns>
	protected string GetSettlementAmount(CartObject cart)
	{
		var settlementAmount = CurrencyManager.ToSettlementCurrencyNotation(
			cart.SettlementAmount,
			cart.SettlementCurrency);
		return settlementAmount;
	}

	/// <summary>
	/// バインドされている値と現在のカートの合計金額、商品を比較する
	/// </summary>
	/// <returns>違ったらfalse、同じならtrue</returns>
	protected bool CheckCartListToBindingData()
	{
		// 合計金額と商品の種類の数が同じか
		if (this.BindingCartList
				.Select((v, i) => new { Value = v, Index = i })
				.Any(x => (x.Value.PriceTotal != this.CartList.Items[x.Index].PriceTotal))
			|| this.BindingCartList
				.Select((v, i) => new { Value = v, Index = i })
				.Any(x => (x.Value.Items.Count != this.CartList.Items[x.Index].Items.Count)))
		{
			return false;
		}

		// 商品が同じか
		foreach (var cart in this.BindingCartList.Select((v, i) => new { Value = v, Index = i }))
		{
			var index = 0;
			foreach (var item in cart.Value.Items)
			{
				if (item.ProductId != this.CartList.Items[cart.Index].Items[index].ProductId) return false;
				index++;
			}
		}

		// 商品付帯情報整合性チェック
		var checkProductOptionSettingResult = new ProductOptionSettingList()
			.CheckCartListToBindingProductOptionSettingList(this.BindingCartList);

		return checkProductOptionSettingResult;
	}

	/// <summary>
	///  カート情報エリアのリピータイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rCart_OnItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var sPriceControl = (HtmlGenericControl)e.Item.FindControl("sPrice");
		if (sPriceControl == null) return;

		var pPriceControl = (HtmlGenericControl)e.Item.FindControl("pPrice");
		var pSubscriptionBoxCampaignPriceControl = (HtmlGenericControl)e.Item.FindControl("pSubscriptionBoxCampaignPrice");
		var sSubscriptionBoxCampaignPriceControl = (HtmlGenericControl)e.Item.FindControl("sSubscriptionBoxCampaignPrice");
		var pSubscriptionBoxCampaignPeriodControl = (HtmlGenericControl)e.Item.FindControl("pSubscriptionBoxCampaignPeriod");
		var sSubscriptionBoxCampaignPeriodSinceControl = (HtmlGenericControl)e.Item.FindControl("sSubscriptionBoxCampaignPeriodSince");
		var sSubscriptionBoxCampaignPeriodUntilControl = (HtmlGenericControl)e.Item.FindControl("sSubscriptionBoxCampaignPeriodUntil");
		var cartProduct = (CartProduct)e.Item.DataItem;

		if (cartProduct.IsSubscriptionBox == false) return;

		var selectedSubscriptionBox = DataCacheControllerFacade
			.GetSubscriptionBoxCacheController()
			.Get(cartProduct.SubscriptionBoxCourseId);

		var subscriptionBoxItem = selectedSubscriptionBox.SelectableProducts.FirstOrDefault(
			x => (x.ProductId == cartProduct.ProductId) && (x.VariationId == cartProduct.VariationId));

		var product = new ProductService().GetProductVariation(this.ShopId, cartProduct.ProductId, cartProduct.VariationId, this.MemberRankId);

		// 頒布会キャンペーン期間の場合キャンペーン期間価格を適用
		if (OrderCommon.IsSubscriptionBoxCampaignPeriod(subscriptionBoxItem))
		{
			pPriceControl.Visible = true;
			sPriceControl.InnerText = CurrencyManager.ToPrice(
					product.FixedPurchasePrice ?? product.Price);
			sSubscriptionBoxCampaignPriceControl.InnerText
				= CurrencyManager.ToPrice(subscriptionBoxItem.CampaignPrice);
			pSubscriptionBoxCampaignPeriodControl.Visible = true;
			if (subscriptionBoxItem.CampaignSince.HasValue)
			{
				sSubscriptionBoxCampaignPeriodSinceControl.InnerText = HtmlSanitizer.HtmlEncode(
					StringUtility.ToEmpty(
						((DateTime)subscriptionBoxItem.CampaignSince).ToString("yyyy年MM月dd日 HH時mm分")));
			}

			if (subscriptionBoxItem.CampaignUntil.HasValue)
			{
				sSubscriptionBoxCampaignPeriodUntilControl.InnerText = HtmlSanitizer.HtmlEncode(
					StringUtility.ToEmpty(
						((DateTime)subscriptionBoxItem.CampaignUntil).ToString("yyyy年MM月dd日 HH時mm分")));
			}
		}

		if (cartProduct.IsSubscriptionBoxFixedAmount())
		{
			pPriceControl.Visible
				= pSubscriptionBoxCampaignPriceControl.Visible
					= pSubscriptionBoxCampaignPeriodControl.Visible
						= false;
		}
	}

	/// <summary>
	/// Get first shipping date
	/// </summary>
	/// <param name="shipping">Cart shipping</param>
	/// <returns>First shipping date</returns>
	public string GetFirstShippingDate(CartShipping shipping)
	{
		var result = ((shipping.ShippingMethod
					== Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_MAIL)
				|| (shipping.FirstShippingDate == DateTime.MinValue)
				|| (shipping.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS)
				|| (shipping.FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY))
			? DateTimeUtility.ToStringFromRegion(
				shipping.GetFirstShippingDate(),
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter)
			: DateTimeUtility.ToStringFromRegion(
				shipping.FirstShippingDate,
				DateTimeUtility.FormatType.LongDateWeekOfDay1Letter);
		return result;
	}

	/// <summary>
	/// 頒布会定額コース情報を表示するか
	/// </summary>
	/// <param name="isFixedAmountItem">頒布会定額コース商品か</param>
	/// <returns>表示するであればTRUE</returns>
	protected bool DisplaySubscriptionBoxFixedAmountCourse(bool isFixedAmountItem)
	{
		return isFixedAmountItem
			&& this.CartList.Items[0].IsOrderCombined
			&& (this.CartList.Items[0].HaveOnlyOneSubscriptionBoxFixedAmountCourseItem() == false);
	}

	/// <summary>
	/// Is store pickup displayed
	/// </summary>
	/// <param name="dataItem">Data Item</param>
	/// <param name="isStorePickup">Is store pickup</param>
	/// <returns>True if cart is digital and shipping address kbn is different from store pickup address kbn, otherwise false</returns>
	protected bool IsStorePickupDisplayed(object dataItem, bool isStorePickup = false)
	{
		var cart = FindCart(dataItem);
		var cartShipping = (CartShipping)dataItem;
		var result = (cart.IsDigitalContentsOnly == false)
			&& (cartShipping.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
			&& (cartShipping.IsShippingStorePickup == isStorePickup);

		return result;
	}

	/// <summary>
	/// paidyログ出力
	/// </summary>
	/// <param name="request">リクエスト</param>
	/// <param name="response">レスポンス</param>
	[WebMethod]
	public static void WritePaidyLog(object request, object response)
	{
		// Paidyログ出力
		new PaidyCheckout(null).OutputLog(
			request.ToString(),
			response.ToString());
	}

	/// <summary>注文完了ボタンリスト</summary>
	protected List<LinkButton> CompleteButtonList { get; private set; }
	/// <summary>注文同梱での商品価格変更有無</summary>
	protected bool IsChangedProductPriceByOrderCombine
	{
		get
		{
			var result = ((this.IsOrderCombined) && (OrderCombineUtility.IsChangedProductPriceByOrderCombine(SessionManager.OrderCombineBeforeCartList.Items[0], this.CartList.Items[0])));
			return result;
		}
	}
	/// <summary>注文同梱による既存定期台帳変更有無</summary>
	protected bool IsChangedFixedPurchaseByOrderCombine
	{
		get
		{
			var result = ((this.IsOrderCombined) && (OrderCombineUtility.IsChangedFixedPurchaseByOrderCombine(SessionManager.OrderCombineBeforeCartList.Items[0], this.CartList.Items[0])));
			return result;
		}
	}
	/// <summary>購入制限エラーメッセージ</summary>
	protected string ProductOrderLimitErrorMessage
	{
		get { return (string)ViewState["ProductOrderLimitErrorMessage"]; }
		set { ViewState["ProductOrderLimitErrorMessage"] = value; }
	}
	/// <summary>アマゾンリクエスト</summary>
	protected AmazonCv2Redirect AmazonRequest { get; set; }

	#region プロパティ
	/// <summary>現在のカート情報リスト格納用</summary>
	protected List<CartObject> BindingCartList
	{
		get { return (List<CartObject>)ViewState["bindingCartList"]; }
		set { ViewState["bindingCartList"] = value; }
	}
	#endregion
}

