/*
=========================================================================================================
  Module      : 注文お支払い方法選択画面処理(OrderPayment.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Amazon.Util;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.User;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Coupon.Helper;
using w2.Domain.DeliveryCompany;
using w2.Domain.Payment;

public partial class Form_Order_OrderPayment : OrderCartPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	#region ラップ済コントロール宣言
	WrappedLinkButton WlbBack { get { return GetWrappedControl<WrappedLinkButton>("lbBack"); } }
	#endregion

	/// <summary>P
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		this.DispNum = 0;
		this.DispErrorMessage = "";

		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);
			if (this.CartList.Items[0].Items.Count == 0) return;

			InitComponentsOrderPayment();
			this.CreditCardList.Add(new ListItem("", ""));
			this.CreditCompanyList = new ListItemCollection();
			this.CreditCompanyList.Add(new ListItem("", ""));
			this.CreditExpireMonth.Add(new ListItem("", ""));
			this.CreditExpireYear.Add(new ListItem("", ""));
			this.CreditInstallmentsList.Add(new ListItem("", ""));

			this.OpenPointInput = new List<bool>();
			this.OpenCouponInput = new List<bool>();
			foreach (var co in this.CartList.Items)
			{
				this.OpenPointInput.Add(co.UsePoint != 0);
				this.OpenCouponInput.Add(co.Coupon != null);
			}

			Reload();
			return;
		}

		//------------------------------------------------------
		// HTTPS通信チェック（HTTP通信の場合、ショッピングカートへ）
		//------------------------------------------------------
		CheckHttps();

		//------------------------------------------------------
		// カート存在チェック（カートが存在しない場合、エラーページへ）
		//------------------------------------------------------
		CheckCartExists();

		//------------------------------------------------------
		// カート注文者チェック
		// カート注文者情報とログイン状態に不整合がある場合、配送先入力ページへ戻る（注文者再設定）
		//------------------------------------------------------
		CheckCartOwner();

		//------------------------------------------------------
		// コンポーネント初期化
		//------------------------------------------------------
		// ※イベント実行後（ポストバック）にデータバインドされるため、常にコンポーネント初期化を行う
		InitComponentsOrderPayment();

		//------------------------------------------------------
		// トークン有効期限チェック
		//-----------------------------------------------------
		var creditTokenCheck = this.CartList.CheckCreditTokenExpired(true);
		if (creditTokenCheck == false)
		{
			this.DispErrorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_CREDIT_TOKEN_EXPIRED);
		}

		// 注文同梱の場合、カート情報を注文同梱後のカート情報を置き換える
		// OrderPageのPage_InitメソッドでPostBack時もCartListも変更されるため常に行う
		if (this.IsOrderCombined)
		{
			this.CartList = SessionManager.OrderCombineCartList;
			CreateOrderOwner();
		}

		if (!IsPostBack)
		{
			// 注文同梱済みで、決済種別を再選択していない場合に決済エラーメッセージを表示
			if ((this.IsOrderCombined) && (this.IsPaymentReselect == false))
			{
				var shippingCompany = new DeliveryCompanyService().Get(this.CartList.Items[0].Shippings[0].DeliveryCompanyId);
				this.CartList.Items[0].Shippings[0].ShippingTimeMessage = shippingCompany.GetShippingTimeMessage(this.CartList.Items[0].Shippings[0].ShippingTime);

				// 支払い回数設定(クレジットカード以外の場合DataBindに失敗するためnullを設定)
				if (this.CartList.Items[0].Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				{
					this.CartList.Items[0].Payment.CreditInstallmentsCode = null;
				}

				// 藍新Pay支払い回数設定(クレジットカード以外の場合DataBindに失敗するためnullを設定)
				if (this.CartList.Items[0].Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)
				{
					this.CartList.Items[0].Payment.NewebPayCreditInstallmentsCode = null;
				}

				var payment = OrderCommon.GetPayment(this.CartList.Items[0].ShopId, this.CartList.Items[0].Payment.PaymentId);
				var usablePriceMin = (decimal)payment[Constants.FIELD_PAYMENT_USABLE_PRICE_MIN];
				var usablePriceMax = (decimal)payment[Constants.FIELD_PAYMENT_USABLE_PRICE_MAX];
				this.ErrorMessages.Add(
					OrderCommon.GetErrorMessage(
						OrderErrorcode.PaymentUsablePriceOutOfRangeError,
						CurrencyManager.ToPrice(this.CartList.Items[0].PriceTotal),
						CurrencyManager.ToPrice(usablePriceMin),
						CurrencyManager.ToPrice(usablePriceMax)));
			}

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				// Adjust point and member rank by Cross Point api
				UserUtility.AdjustPointAndMemberRankByCrossPointApi(this.LoginUser);
			}

			//------------------------------------------------------
			// 画面遷移の正当性チェック
			//------------------------------------------------------
			CheckOrderUrlSession();

			//------------------------------------------------------
			// カート決済情報作成（デフォルトで配送希望は希望するようにする）
			//------------------------------------------------------
			CreateCartPayment();

			//------------------------------------------------------
			//  クレジットカード番号再表示の際は、上位４桁と下位４桁以外を空文字設定、セキュリティコードを空文字設定
			//------------------------------------------------------
			AdjustCreditCardNo();

			//----------------------------------------------------
			// オープン状態
			//----------------------------------------------------
			this.OpenPointInput = new List<bool>();
			this.OpenCouponInput = new List<bool>();
			foreach (CartObject co in this.CartList.Items)
			{
				this.OpenPointInput.Add(co.UsePoint != 0);
				this.OpenCouponInput.Add(co.Coupon != null);
			}

			// 使用クーポンの正当性チェック
			ValidateUseCoupon();

			//------------------------------------------------------
			// リロード
			//------------------------------------------------------
			Reload();

			// デフォルト注文系エラーメッセージ表示
			ShowErrorMessageForUserDefaultOrderSetting((string)Session[Constants.SESSION_KEY_ORDER_ERROR_MESSAGE]);
		}
		else
		{
			// トークンが入力されていたら入力画面を切り替える
			SwitchDisplayForCreditTokenInput(this.WrCartList);
		}
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	protected void Reload()
	{
		//------------------------------------------------------
		// 有効決済種別取得
		//------------------------------------------------------
		this.ValidPayments = new List<PaymentModel[]>();
		this.DispLimitedPaymentMessages = new Hashtable();
		foreach (CartObject cart in this.CartList)
		{
			if (this.IsOrderCombined == false)
			{
				// 配送先で再計算
				cart.Calculate(false, isShippingChanged: true);
			}

			// 決済種別情報セット（配送種別で決済種別を限定しているのであれば、有効の決済種別のみ取得）
			 var validPaymentList = OrderCommon.GetValidPaymentList(
				cart,
				this.LoginUserId,
				isMultiCart: this.CartList.IsMultiCart);

			// Set limited payments messages for cart
			this.DispLimitedPaymentMessages[this.CartList.Items.IndexOf(cart)] = OrderCommon.GetLimitedPaymentsMessagesForCart(cart, validPaymentList);

			// Get the payments un-limit by product
			var paymentsUnlimit = OrderCommon.GetPaymentsUnLimitByProduct(cart, validPaymentList);

			if (Constants.RAKUTEN_LOGIN_ENABLED)
			{
				// 楽天IDConnectでログインしている場合は楽天ペイを先頭にする
				SortPaymentListForRakutenIdConnectLoggedIn(validPaymentList);
			}

			// AmazonPayは表示しない
			paymentsUnlimit = AmazonUtil.RemoveAmazonPayFromValidPayments(paymentsUnlimit);

			if (Constants.PAYMENT_SMS_DEF_KBN == Constants.PaymentSmsDef.YamatoKa)
			{
				if (((Constants.PAYMENT_SETTING_YAMATO_KA_SMS_USE_FIXEDPURCHASE == false)
						&& cart.Items.Any(item => item.IsFixedPurchase)))
				{
					paymentsUnlimit = paymentsUnlimit
						.Where(item => (OrderCommon.CheckPaymentYamatoKaSms(item.PaymentId) == false)).ToArray();
				}
			}

			// メール便の場合は代引き不可
			var items = paymentsUnlimit.Where(item => ((item.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT) || cart.IsExpressDelivery));
			var cartShipping = cart.Shippings[0];
			var logisticsCollectionFlg = ECPayUtility.GetIsCollection(cartShipping.ShippingReceivingStoreType);

			if (OrderCommon.IsReceivingStoreTwEcPayCvsOptionEnabled()
				&& (logisticsCollectionFlg == Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON))
			{
				items = items.Where(item => (item.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE));
			}
			else if (OrderCommon.IsReceivingStoreTwEcPayCvsOptionEnabled()
					&& ((cartShipping.ConvenienceStoreFlg
							== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
						|| (logisticsCollectionFlg == Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_OFF)))
			{
				items = items.Where(item => item.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE);
			}

			if (cartShipping.IsShippingStorePickup)
			{
				items = OrderCommon.GetPaymentsUnLimitByProduct(cart, validPaymentList)
					.Where(item => Constants.SETTING_CAN_STORE_PICKUP_OPTION_PAYMENT_IDS.Contains(item.PaymentId)
						&& (Constants.SETTING_CAN_NOT_STORE_PICKUP_OPTION_PAYMENT_IDS.Contains(item.PaymentId) == false));
			}

			if (PaygentUtility.CanUsePaidyPayment(cart) == false)
			{
				items = items.Where(item => (item.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY));
			}

			if (PaygentUtility.CanUseBanknetPayment(cart) == false)
			{
				items = items.Where(item => (item.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET));
			}

			this.ValidPayments.Add(items.OrderBy(x => x.DisplayOrder).ToArray());
		}

		// デフォルト支払方法が存在し、レコメンド商品が投入されてない場合、カート1の支払方法を以降のカートにも適用する。
		var forceApplyFirstCartPaymentToAfterCarts = (this.CartList.UserDefaultOrderSettingParm.IsUserDefaultPaymentExist) && (this.IsAddRecmendItem == false);
		SetPayment(forceApplyFirstCartPaymentToAfterCarts);

		// データバインド
		this.WrCartList.DataSource = this.CartList;
		this.WrCartList.DataBind();

		//------------------------------------------------------
		// 各種表示初期化
		//------------------------------------------------------
		foreach (RepeaterItem riCart in WrCartList.Items)
		{
			// ラップ済みコントロール宣言
			var wcbUseSamePaymentAddrAsCart1 = GetWrappedControl<WrappedCheckBox>(riCart, "cbUseSamePaymentAddrAsCart1", false);
			var wrPayment = GetWrappedControl<WrappedRepeater>(riCart, "rPayment");

			// 「カート番号「１」同じお支払いを指定する」チェックイベント
			if (wcbUseSamePaymentAddrAsCart1.InnerControl != null)
			{
				cbUseSamePaymentAddrAsCart1_OnCheckedChanged(wcbUseSamePaymentAddrAsCart1, EventArgs.Empty);
			}

			// 決済種別が未選択の場合、一番上の決済種別を選択（ラジオボタン）
			if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB)
			{
				bool blChecked = false;
				foreach (RepeaterItem riPayment in wrPayment.Items)
				{
					var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(riPayment, "rbgPayment");
					blChecked |= wrbgPayment.Checked;
				}
				if ((blChecked == false) && (wrPayment.Items.Count != 0))
				{
					var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(wrPayment.Items[0], "rbgPayment");
					wrbgPayment.Checked = true;
				}
			}

			// 決済種別ラジオボタンクリックイベント
			foreach (RepeaterItem riPayment in wrPayment.Items)
			{
				if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_RB)
				{
					var wrbgPayment = GetWrappedControl<WrappedRadioButtonGroup>(riPayment, "rbgPayment");
					if (wrbgPayment.InnerControl != null)
					{
						rbgPayment_OnCheckedChanged(wrbgPayment, EventArgs.Empty);
					}
				}
				else if (Constants.PAYMENT_CHOOSE_TYPE == Constants.PAYMENT_CHOOSE_TYPE_DDL)
				{
					var wddlPayment = GetWrappedControl<WrappedDropDownList>(riPayment.Parent.Parent, "ddlPayment");
					if (wddlPayment.InnerControl != null)
					{
						rbgPayment_OnCheckedChanged(wddlPayment, EventArgs.Empty);
					}
				}
				
				var wddlUserCreditCard = GetWrappedControl<WrappedDropDownList>(
					riPayment,
					"ddlUserCreditCard",
					CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW);
				if (wddlUserCreditCard.InnerControl != null)
				{
					ddlUserCreditCard_OnSelectedIndexChanged(wddlUserCreditCard, EventArgs.Empty);
				}

				var wcbRegistCreditCard = GetWrappedControl<WrappedCheckBox>(riPayment, "cbRegistCreditCard");
				if (wcbRegistCreditCard.InnerControl != null)
				{
					cbRegistCreditCard_OnCheckedChanged(wcbRegistCreditCard, EventArgs.Empty);
				}
			}

			// 領収書希望の選択肢変更イベント
			var wddlReceiptFlg = GetWrappedControl<WrappedDropDownList>(riCart, "ddlReceiptFlg");
			if (Constants.RECEIPT_OPTION_ENABLED && wddlReceiptFlg.HasInnerControl)
			{
				ddlReceiptFlg_OnSelectedIndexChanged(wddlReceiptFlg, EventArgs.Empty);
			}

			InitializeCouponComponents(riCart);
		}

		// コントロールにJavascriptセット
		SetJavascriptToControl();

		// 全体用のエラーメッセージを表示用として設定する
		this.DispErrorMessage += this.ErrorMessages.Get();

		// トークン決済利用の場合はカスタムバリデータを一部無効にする
		DisableCreditInputCustomValidatorForGetCreditToken(this.WrCartList);
	}

	/// <summary>
	/// 決済情報更新
	/// </summary>
	/// <param name="defCartPayment">カート決済情報</param>
	/// <param name="cartObj">カートオブジェクト</param>
	private static void UpdateCartPayment(CartPayment defCartPayment, CartObject cartObj)
	{
		cartObj.Payment.UpdateCartPayment(
			defCartPayment.PaymentId,
			defCartPayment.PaymentName,
			defCartPayment.CreditCardBranchNo,
			defCartPayment.CreditCardCompany,
			defCartPayment.CreditCardNo1,
			defCartPayment.CreditCardNo2,
			defCartPayment.CreditCardNo3,
			defCartPayment.CreditCardNo4,
			defCartPayment.CreditExpireMonth,
			defCartPayment.CreditExpireYear,
			defCartPayment.CreditInstallmentsCode,
			defCartPayment.CreditSecurityCode,
			defCartPayment.CreditAuthorName,
			defCartPayment.PaymentObject,
			false,
			defCartPayment.NewebPayCreditInstallmentsCode
		);
	}

	/// <summary>
	/// 適正なクーポンが利用されているか
	/// </summary>
	private void ValidateUseCoupon()
	{
		if (Constants.W2MP_COUPON_OPTION_ENABLED == false) return;
		if (this.CartList.Items.Any(item => item.Coupon != null) == false) return;

		var service = new w2.Domain.Coupon.CouponService();
		foreach (var cart in this.CartList.Items.Where(item => item.IsBlacklistCouponUse()))
		{
			var isUsed = service.CheckUsedCoupon(cart.Coupon.CouponId,
				(Constants.COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE == Constants.FLG_COUPONUSEUSER_BLACKLISTCOUPON_USED_USER_JUDGE_TYPE_MAIL_ADDRESS)
					? cart.Owner.MailAddr
					: cart.OrderUserId);
			if (isUsed)
			{
				this.ErrorMessages.Add(this.CartList.Items.IndexOf(cart),
					CartErrorMessages.ErrorKbn.Coupon,
					CouponOptionUtility.GetErrorMessage(CouponErrorcode.CouponUsedError).Replace("@@ 1 @@", cart.Coupon.CouponCode));
				cart.Coupon = null;
			}
		}
	}

	/// <summary>
	/// コントロールにJavascriptセット
	/// </summary>
	private void SetJavascriptToControl()
	{
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			// ラップ済みコントロール宣言
			var usePoint = this.CartList.Items[riCart.ItemIndex].UsePoint.ToString();
			var couponCode = (this.CartList.Items[riCart.ItemIndex].Coupon != null)
				? this.CartList.Items[riCart.ItemIndex].Coupon.CouponCode
				: "";
			var wtbOrderPointUse = GetWrappedControl<WrappedTextBox>(riCart, "tbOrderPointUse", usePoint);
			var wtbCouponCode = GetWrappedControl<WrappedTextBox>(riCart, "tbCouponCode", couponCode);
			var wlbRecalculateCart = GetWrappedControl<WrappedLinkButton>(riCart, "lbRecalculateCart");

			// テキストボックス向けイベント作成
			var eventCreator = new TextBoxEventScriptCreator(wlbRecalculateCart);
			eventCreator.RegisterInitializeScript(this);

			eventCreator.AddScriptToControl(wtbOrderPointUse);
			eventCreator.AddScriptToControl(wtbCouponCode);
		}
	}

	/// <summary>
	/// （トークン決済向け）カード情報編集リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbEditCreditCardNoForToken_Click(object sender, System.EventArgs e)
	{
		// トークンなどクレジットカード情報削除
		ResetCreditTokenInfoFromForm((RepeaterItem)((LinkButton)sender).Parent.Parent.Parent.Parent);

		// 画面更新
		Reload();
	}

	/// <summary>
	/// re-enter card rakuten
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbEditCreditCardNoForRakutenToken_Click(object sender, System.EventArgs e)
	{
		// トークンなどクレジットカード情報削除
		ResetCreditTokenInfoFromForm((RepeaterItem)((LinkButton)sender).Parent.Parent.Parent);

		// 画面更新
		Reload();
	}

	/// <summary>
	/// 再計算リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRecalculate_Click(object sender, System.EventArgs e)
	{
		// 再計算リンクのCommandArgumentにカートINDEXが格納されている
		Recalucate(int.Parse(((LinkButton)sender).CommandArgument));

		// 領収書情報取得
		if (Constants.RECEIPT_OPTION_ENABLED) SetReceipt();

		// 画面更新
		Reload();
	}

	/// <summary>
	///  再計算処理
	/// </summary>
	/// <param name="cartIndex">入力チェック用カートID</param>
	private void Recalucate(int cartIndex)
	{
		//------------------------------------------------------
		// 入力中カートINDEXセット
		//------------------------------------------------------
		this.CurrentCartIndex = cartIndex;

		//------------------------------------------------------
		// 入力チェックし、エラーがなければその他チェック・更新
		//------------------------------------------------------
		CheckAndSetInputData();

		//------------------------------------------------------
		// カートチェック（入力チェックNGの場合はカートチェックまではしない）
		//------------------------------------------------------
		if (this.ErrorMessages.Count == 0)
		{
			CheckCartData();
		}
	}

	/// <summary>
	/// 入力チェック＆オブジェクトへセット
	/// </summary>
	/// <param name="sender"></param>
	protected void CheckAndSetInputData()
	{
		//------------------------------------------------------
		// 利用ポイント入力チェック
		//------------------------------------------------------
		CheckInputDataForPoint();

		//------------------------------------------------------
		// エラーがなければ入力値セット（商品数はDB反映する）
		//------------------------------------------------------
		if (this.ErrorMessages.Count == 0)
		{
			//------------------------------------------------------
			// 利用クーポンセット
			//------------------------------------------------------
			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				SetUseCouponData(this.WrCartList);
			}

			//------------------------------------------------------
			// 利用ポイント数セット
			//------------------------------------------------------
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				SetUsePointData(this.WrCartList);
			}

			//------------------------------------------------------
			// 再計算
			//------------------------------------------------------
			foreach (CartObject coCart in this.CartList.Items)
			{
				coCart.CalculateWithCartShipping();
			}
		}
	}

	/// <summary>
	/// ポイントを使うリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUsePoint_Click(object sender, System.EventArgs e)
	{
		// ラップ済みコントロール宣言
		var wdivPointBox = GetWrappedControl<WrappedHtmlGenericControl>(((LinkButton)sender).Parent, "divPointBox");

		// 各種表示設定
		this.OpenPointInput[((RepeaterItem)((LinkButton)sender).Parent).ItemIndex] = true;
		wdivPointBox.Visible = true;
		((WebControl)((LinkButton)sender).Parent.FindControl("lbUsePoint")).Visible = false;
	}

	/// <summary>
	/// クーポンを使うリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbUseCoupon_Click(object sender, System.EventArgs e)
	{
		// ラップ済みコントロール宣言
		var wdivCouponBox = GetWrappedControl<WrappedHtmlGenericControl>(((LinkButton)sender).Parent, "divCouponBox");

		// 各種表示設定
		this.OpenCouponInput[((RepeaterItem)((LinkButton)sender).Parent).ItemIndex] = true;
		wdivCouponBox.Visible = true;
		((WebControl)((LinkButton)sender).Parent.FindControl("lbUseCoupon")).Visible = false;
	}

	/// <summary>
	/// カート画面へ戻る
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBackToCart_Click(object sender, System.EventArgs e)
	{
		SessionManager.OrderCombineCartList = null;
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
	}

	/// <summary>
	/// 戻るリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, System.EventArgs e)
	{
		if (this.IsOrderCombined)
		{
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
		}
		else
		{
			// 画面遷移の正当性チェックのため遷移先ページURLを設定
			// 通常注文：注文配送先入力画面、ギフト注文：注文配送先商品選択画面へ遷移
			string page = ((Constants.GIFTORDER_OPTION_ENABLED == false) 
				|| this.ContainsOnlyDigitalContentsInCarts()) 
					? Constants.PAGE_FRONT_ORDER_SHIPPING 
					: Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT_PRODUCT;
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = page;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + page);
		}
	}

	/// <summary>
	/// 次へ進むリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbNext_Click(object sender, System.EventArgs e)
	{
		// 全カート再計算で入力チェックを行う
		if (this.CartList.Items.Count != 0)
		{
			for (int iLoop = 0; iLoop < this.CartList.Items.Count; iLoop++)
			{
				Recalucate(iLoop);
			}

			// エラーの場合はエラーメッセージ表示
			if ((this.ErrorMessages.Get() != "")
				|| (this.ErrorMessages.Count != 0))
			{
				Reload();	// Reloadしないとエラーが画面へ表示されない

				return;
			}
		}

		//------------------------------------------------------
		// お支払い情報をカート情報へセット
		//------------------------------------------------------
		bool hasError = SetPayment(true);

		// 領収書情報をカートにセット
		if (Constants.RECEIPT_OPTION_ENABLED) hasError |= SetReceipt();

		if (hasError)
		{
			SetJavascriptToControl();
			return;
		}

		this.CartList.UpdateShipping();

		// Check Shipping Country Iso Code Can Order With Paidy Pay
		CheckShippingCountryIsoCodeCanOrderWithPaidyPay(this.CartList);

		// Check Country Iso Code Can Order With NP After Pay
		CheckCountryIsoCodeCanOrderWithNPAfterPay(this.CartList);

		//------------------------------------------------------
		// 画面遷移
		//------------------------------------------------------
		if (this.ErrorMessages.Count == 0)
		{
			// Process Check Paidy Token Id Exist
			if ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Direct)
				&& CheckPaidyTokenIdExist())
			{
				var errorMessage = WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_PAIDY_TOKEN_ID_EXISTED_ERROR)
					.Replace("@@ 1 @@", this.WhfPaidyTokenId.Value);
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			if (this.IsOrderCombined) SessionManager.OrderCombinePaymentReselect = true;

			// 画面遷移の正当性チェックのため遷移先ページURLを設定
			SessionManager.NextPageForCheck = Constants.PAGE_FRONT_ORDER_CONFIRM;

			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_CONFIRM);
		}
		else
		{
			Reload();	// Reloadしないとエラーが画面へ表示されない
		}
	}

	/// <summary>
	/// 次回購入の利用ポイントの全適用フラグ変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUseAllPointFlg_Changed(object sender, EventArgs e)
	{
		var useAllPointFlgInputMethod = (CheckBox)sender;
		var ri = (RepeaterItem)useAllPointFlgInputMethod.Parent.Parent;

		this.CartList.Items[ri.ItemIndex].UseAllPointFlg = useAllPointFlgInputMethod.Checked;

		lbRecalculate_Click((useAllPointFlgInputMethod.Parent.FindControl("lbRecalculateCart")), null);
	}

	/// <summary>
	/// 次回購入の利用ポイントの全適用フラグデータバインド時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUseAllPointFlg_DataBinding(object sender, EventArgs e)
	{
		var useAllPointFlgInputMethod = (CheckBox)sender;
		var ri = (RepeaterItem)useAllPointFlgInputMethod.Parent.Parent;

		useAllPointFlgInputMethod.Checked = this.CartList.Items[ri.ItemIndex].UseAllPointFlg;
	}

	/// <summary>
	/// クーポン入力方法変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblCouponInputMethod_SelectedIndexChanged(object sender, EventArgs e)
	{
		var couponInputMethod = (RadioButtonList)sender;
		var ri = (RepeaterItem)couponInputMethod.Parent.Parent.Parent;

		ri.FindControl("ddlCouponList").Visible = (couponInputMethod.Text != CouponOptionUtility.COUPON_INPUT_METHOD_MANUAL_INPUT);
		ri.FindControl("hgcCouponCodeInputArea").Visible = (couponInputMethod.Text == CouponOptionUtility.COUPON_INPUT_METHOD_MANUAL_INPUT);
		((TextBox)ri.FindControl("tbCouponCode")).Text = "";
		((DropDownList)ri.FindControl("ddlCouponList")).SelectedValue = "";

		this.CartList.Items[ri.ItemIndex].CouponInputMethod = (StringUtility.ToEmpty(couponInputMethod.Text) == "")
			? CouponOptionUtility.COUPON_INPUT_METHOD_SELECT
			: couponInputMethod.Text;
		this.CartList.Items[ri.ItemIndex].Coupon = null;

		lbRecalculate_Click((couponInputMethod.Parent.FindControl("lbRecalculateCart")), null);
	}

	/// <summary>
	/// クーポン入力方法データバインド時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblCouponInputMethod_DataBinding(object sender, EventArgs e)
	{
		var couponInputMethod = (RadioButtonList)sender;
		var ri = (RepeaterItem)couponInputMethod.Parent.Parent.Parent;

		couponInputMethod.SelectedValue = (StringUtility.ToEmpty(this.CartList.Items[ri.ItemIndex].CouponInputMethod) == "")
			? CouponOptionUtility.COUPON_INPUT_METHOD_SELECT
			: this.CartList.Items[ri.ItemIndex].CouponInputMethod;
	}

	/// <summary>
	/// クーポンリスト変更
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlCouponList_TextChanged(object sender, EventArgs e)
	{
		var couponCodeList = (DropDownList)sender;
		var couponCodeTextbox = ((TextBox)couponCodeList.Parent.FindControl("tbCouponCode"));
		couponCodeTextbox.Text = couponCodeList.SelectedValue;

		lbRecalculate_Click((couponCodeList.Parent.FindControl("lbRecalculateCart")), null);
	}

	/// <summary>
	/// クーポンBOXクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbShowCouponBox_Click(object sender, EventArgs e)
	{
		var ri = (RepeaterItem)((LinkButton)sender).Parent.Parent;
		((TextBox)ri.FindControl("tbCouponCode")).Text = "";
		((DropDownList)ri.FindControl("ddlCouponList")).SelectedValue = "";
		this.CartList.Items[ri.ItemIndex].CouponBoxVisible = true;

		lbRecalculate_Click((((LinkButton)sender).Parent.FindControl("lbRecalculateCart")), null);
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
		var couponCode = (TextBox)lbCouponSelect.Parent.Parent.Parent.FindControl("tbCouponCode");
		var couponListDdl = (DropDownList)lbCouponSelect.Parent.Parent.Parent.FindControl("ddlCouponList");
		var ri = (RepeaterItem)lbCouponSelect.Parent.Parent.Parent.Parent;

		couponCode.Text = selectedCouponCode.Value;
		couponListDdl.SelectedValue = selectedCouponCode.Value;
		this.CartList.Items[ri.ItemIndex].CouponBoxVisible = false;

		lbRecalculate_Click((((LinkButton)sender).Parent.Parent.Parent.FindControl("lbRecalculateCart")), null);
	}

	/// <summary>
	/// モーダルクーポンBOX 閉じるボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCouponBoxClose_Click(object sender, EventArgs e)
	{
		var ri = (RepeaterItem)((LinkButton)sender).Parent.Parent;
		this.CartList.Items[ri.ItemIndex].CouponBoxVisible = false;

		lbRecalculate_Click((((LinkButton)sender).Parent.Parent.FindControl("lbRecalculateCart")), null);
	}


	/// <summary>
	/// クーポン割引文字列取得
	/// </summary>
	/// <param name="coupon">ユーザークーポン詳細情報</param>
	/// <returns>クーポン割引文字列</returns>
	public static string GetCouponDiscountString(UserCouponDetailInfo coupon)
	{
		var freeShippingMessage = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_COUPON_LIST,
			Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
			Constants.VALUETEXT_PARAM_COUPON_LIST_FREE_SHIPPING);
		var discountPriceMessage = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_COUPON_LIST,
			Constants.VALUETEXT_PARAM_COUPON_LIST_TITLE,
			Constants.VALUETEXT_PARAM_COUPON_LIST_DISCOUNT_PRICE);
		if (coupon.DiscountPrice != null)
		{
			if (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
				return freeShippingMessage + "\n" + discountPriceMessage + CurrencyManager.ToPrice(coupon.DiscountPrice);
			return CurrencyManager.ToPrice(coupon.DiscountPrice);
		}
		if (coupon.DiscountRate != null)
		{
			if (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID)
				return freeShippingMessage + "\n" + discountPriceMessage + StringUtility.ToNumeric(coupon.DiscountRate) + "%";
			return StringUtility.ToNumeric(coupon.DiscountRate) + "%";
		}
		if (CouponOptionUtility.IsFreeShipping(coupon.CouponType) || (coupon.FreeShippingFlg == Constants.FLG_COUPON_FREE_SHIPPING_VALID))
		{
			return freeShippingMessage;
		}
		return "-";
	}

	/// <summary>
	/// カート番号「１」同じお支払いを指定するチェックボックスクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbUseSamePaymentAddrAsCart1_OrderPayment_OnCheckedChanged(object sender, EventArgs e)
	{
		cbUseSamePaymentAddrAsCart1_OnCheckedChanged(sender, e);
	}

	/// <summary>
	/// Can use point for purchase
	/// </summary>
	/// <param name="container">Container</param>
	/// <param name="canOpenPointInput">Can open point input</param>
	/// <returns>True if can use point for purchase, otherwise: False</returns>
	protected bool CanUsePointForPurchase(RepeaterItem container, bool canOpenPointInput = true)
	{
		var cart = (CartObject)container.DataItem;
		var result = (cart.CanUsePointForPurchase
			&& (this.OpenPointInput[container.ItemIndex] == canOpenPointInput));
		return result;
	}

	/// <summary>エラーメッセージ表示用</summary>
	protected string DispErrorMessage { get; set; }
	/// <summary>戻るイベント格納用</summary>
	protected string BackEvent
	{
		get { return "javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + WlbBack.UniqueID + "', '', true, '', '', false, true))"; }
	}
	/// <summary>戻るonclick</summary>
	protected string BackOnClick
	{
		get { return "return true"; }
	}
	/// <summary>次へ進むイベント格納用</summary>
	protected string NextEvent
	{
		get { return "javascript:doPostbackEvenIfCardAuthFailed=false;WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + this.lbNext.UniqueID + "', '', true, '" + this.lbNext.ValidationGroup + "', '', false, true))"; }
	}
	/// <summary>次へ進むonclick</summary>
	protected string NextOnClick
	{
		get
		{
			return (Constants.PAYMENT_PAIDY_OPTION_ENABLED
				&& (Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Direct))
					? "PaidyPayProcess(); return false;"
					: "return true;";
		}
	}
	/// <summary>ポイント入力オープン状態</summary>
	protected List<bool> OpenPointInput
	{
		get { return (List<bool>)ViewState["OpenPointInput"]; }
		set { ViewState["OpenPointInput"] = value; }
	}
	/// <summary>クーポン入力オープン状態</summary>
	protected List<bool> OpenCouponInput
	{
		get { return (List<bool>)ViewState["OpenCouponInput"]; }
		set { ViewState["OpenCouponInput"] = value; }
	}
	/// <summary>Limited Payment Message</summary>
	protected Hashtable DispLimitedPaymentMessages
	{
		get { return (Hashtable)ViewState["LimitedPaymentMessages"]; }
		set { ViewState["LimitedPaymentMessages"] = value; }
	}
}
