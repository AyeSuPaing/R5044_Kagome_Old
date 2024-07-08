/*
=========================================================================================================
  Module      : 注文配送先入力画面処理(OrderShipping.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Handlers;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Amazon.Util;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.User;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain.ShopShipping;
using w2.Domain.User;
using w2.Domain.UserDefaultOrderSetting;

public partial class Form_Order_OrderShipping : OrderCartPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }	// httpsアクセス

	#region ラップ済コントロール宣言
	WrappedHtmlGenericControl WaGoTopLink { get { return GetWrappedControl<WrappedHtmlGenericControl>("aGoTopLink"); } }
	WrappedLinkButton WlbBack { get { return GetWrappedControl<WrappedLinkButton>("lbBack"); } }
	/// <summary>注文者国情報ドロップダウンリスト</summary>
	protected WrappedDropDownList WddlOwnerCountry { get { return GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerCountry"); } }
	/// <summary>注文者州情報ドロップダウンリスト</summary>
	protected WrappedDropDownList WddlOwnerAddr5 { get { return GetWrappedControl<WrappedDropDownList>("ddlOwnerAddr5"); } }
	protected WrappedCustomValidator WcvOwnerLoginId { get { return GetWrappedControl<WrappedCustomValidator>(Constants.LOGIN_ID_USE_MAILADDRESS_ENABLED ? "cvOwnerMailAddr" : "cvOwnerLoginId"); } }
	protected WrappedCustomValidator WcvOwnerPassword { get { return GetWrappedControl<WrappedCustomValidator>("cvOwnerPassword"); } }
	protected WrappedCustomValidator WcvOwnerPasswordConf { get { return GetWrappedControl<WrappedCustomValidator>("cvOwnerPasswordConf"); } }

	/// <summary>Wrapped textbox owner tel 1_1</summary>
	protected WrappedTextBox WtbOwnerTel1_1 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerTel1_1"); } }
	/// <summary>Wrapped textbox owner tel 1_2</summary>
	protected WrappedTextBox WtbOwnerTel1_2 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerTel1_2"); } }
	/// <summary>Wrapped textbox owner tel 1_3</summary>
	protected WrappedTextBox WtbOwnerTel1_3 { get { return GetWrappedControl<WrappedTextBox>("tbOwnerTel1_3"); } }
	/// <summary>Wrapped textbox owner tel 1</summary>
	protected WrappedTextBox WtbOwnerTel1 { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1"); } }
	/// <summary>Wrapped textbox owner tel 1 global</summary>
	protected WrappedTextBox WtbOwnerTel1Global { get { return GetWrappedControl<WrappedTextBox>(this.FirstRpeaterItem, "tbOwnerTel1Global"); } }
	/// <summary>Wrapped label authentication status</summary>
	protected WrappedLabel WlbAuthenticationStatus { get { return GetWrappedControl<WrappedLabel>(this.FirstRpeaterItem, "lbAuthenticationStatus"); } }
	/// <summary>Wrapped label authentication status global</summary>
	protected WrappedLabel WlbAuthenticationStatusGlobal { get { return GetWrappedControl<WrappedLabel>(this.FirstRpeaterItem, "lbAuthenticationStatusGlobal"); } }
	/// <summary>Wrapped label authentication message</summary>
	protected WrappedLabel WlbAuthenticationMessage { get { return GetWrappedControl<WrappedLabel>(this.FirstRpeaterItem, "lbAuthenticationMessage"); } }
	/// <summary>Wrapped label authentication message global</summary>
	protected WrappedLabel WlbAuthenticationMessageGlobal { get { return GetWrappedControl<WrappedLabel>(this.FirstRpeaterItem, "lbAuthenticationMessageGlobal"); } }
	/// <summary>Wrapped drop down list owner birth year</summary>
	protected WrappedDropDownList WddlOwnerBirthYear { get { return GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthYear"); } }
	/// <summary>Wrapped drop down list owner birth month</summary>
	protected WrappedDropDownList WddlOwnerBirthMonth { get { return GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthMonth"); } }
	/// <summary>Wrapped drop down list owner birth day</summary>
	protected WrappedDropDownList WddlOwnerBirthDay { get { return GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerBirthDay"); } }
	/// <summary>Wrapped radio button list owner sex</summary>
	protected WrappedRadioButtonList WrblOwnerSex { get { return GetWrappedControl<WrappedRadioButtonList>(this.FirstRpeaterItem, "rblOwnerSex", Constants.FLG_USER_SEX_UNKNOWN); } }
	/// <summary>Wrapped drop down list owner address 1</summary>
	protected WrappedDropDownList WddlOwnerAddr1 { get { return GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerAddr1"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);
			if (this.CartList.Items[0].Items.Count == 0) return;

			InitComponentsOrderShipping();

			this.Process.PrepareForDataBindOrderShipping(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);

			CreateFixedPurchaseSettings();

			this.WrCartList.DataSource = this.CartList;
			this.WrCartList.DataBind();

			foreach (RepeaterItem riCart in this.WrCartList.Items)
			{
				var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(riCart, "ddlShippingKbnList");
				ddlShippingKbnList_OnSelectedIndexChanged(wddlShippingKbnList, e);

				var wddlShippingCountry = GetWrappedControl<WrappedDropDownList>(riCart, "ddlShippingCountry");
				wddlShippingCountry.SelectedValue = this.CartList.Items[0].Shippings[0].ShippingCountryIsoCode;
			}
			return;
		}

		RedirectOrderShippingSelect();

		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
		{
			CheckSiteDomainAndRedirectWithPostData();
			SetInformationReceivingStore(this.CartList);
		}

		//------------------------------------------------------
		// HTTPS通信チェック（HTTP通信の場合、ショッピングカートへ）
		//------------------------------------------------------
		CheckHttps();

		//------------------------------------------------------
		// カートチェック（カートが存在しない場合、エラーページへ）
		//------------------------------------------------------
		CheckCartExists();

		if (Constants.GIFTORDER_OPTION_ENABLED == false)
		{
			// 検索結果レイヤーから住所を確定後、ポストバック発生で住所検索のエラーメッセージが再表示されてしまうためPostBack時に再度消す
			ResetAddressSearchResultErrorMessage(Constants.GIFTORDER_OPTION_ENABLED, rCartList);
		}

		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
		{
			UpdateCartShippingByReceivingStore();
		}

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// 画面遷移の正当性チェック
			//------------------------------------------------------
			CheckOrderUrlSession();

			//ユーザーが退会済みでないか確認
			if (string.IsNullOrEmpty(this.LoginUserId) == false)
			{
				var user = new UserService().Get(this.LoginUserId);
				if ((user != null) && user.IsDeleted)
				{
					Session.Contents.RemoveAll();
					CookieManager.RemoveCookie(Constants.COOKIE_KEY_AUTH_KEY, Constants.PATH_ROOT);
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_SESSION_VANISHED);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}

			//------------------------------------------------------
			// カート内商品チェック（三分岐でログインしたときに会員価格が割り当てられたときはカートに戻る）
			//------------------------------------------------------
			try
			{
				ProductCheckCartList();
			}
			catch (OrderException)
			{
				Response.Redirect(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
			}

			//------------------------------------------------------
			// 新たにログインした場合はOwnerリセット
			//------------------------------------------------------
			if (this.IsLoggedIn)
			{
				if ((this.CartList.Owner != null)
					&& ((this.CartList.Owner.OwnerKbn == null)	// 未ログイン時に情報を入力せず、再び来た場合
						|| UserService.IsGuest(this.CartList.Owner.OwnerKbn)))
				{
					this.CartList.SetOwner(null);
				}
			}

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				// Adjust point and member rank by Cross Point api
				UserUtility.AdjustPointAndMemberRankByCrossPointApi(this.LoginUser);
			}

			//------------------------------------------------------
			// 注文者・配送先情報作成
			//------------------------------------------------------
			CreateOrderOwner();

			// 定期会員フラグ設定
			SetFixedPurchaseMemberFlgForCartObject(this.CartList.Items.ToList());

			CreateOrderShipping();

			// Add Cart Novelty When Register New User from page OrderOwnerDecision.aspx
			if (Constants.NOVELTY_OPTION_ENABLED)
			{
				// カートノベルティリスト作成
				var cartNoveltyList = new CartNoveltyList(this.CartList);

				// Add Product Grant Novelty
				var cartItemCountBefore = this.CartList.Items.Count;
				AddProductGrantNovelty(cartNoveltyList);

				// For case add Novelty to other cart
				if (cartItemCountBefore != this.CartList.Items.Count) cartNoveltyList = new CartNoveltyList(this.CartList);

				// 付与条件外のカート内の付与アイテムを削除
				this.CartList.RemoveNoveltyGrantItem(cartNoveltyList);

				// カートに追加された付与アイテムを含むカートノベルティを削除
				cartNoveltyList.RemoveCartNovelty(this.CartList);
			}

			//------------------------------------------------------
			// ユーザー区分（＝注文者区分）セット
			//------------------------------------------------------
			this.UserKbn = this.CartList.Owner.OwnerKbn;

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			InitComponents();

			//------------------------------------------------------
			// カート注文メモ作成
			//------------------------------------------------------
			CreateOrderMemo();

			CreateOrderExtend();

			//------------------------------------------------------
			// データバインド準備
			//------------------------------------------------------
			this.Process.PrepareForDataBindOrderShipping(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);

			// 定期購入設定作成
			CreateFixedPurchaseSettings();

			// To prevent exception when select store pickup then turn off real shop option and reload page
			if (Constants.REALSHOP_OPTION_ENABLED == false)
			{
				foreach(var item in this.CartList.Items )
				{
					if (item.Shippings[0].SenderAddrKbn.ToString() == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP)
						item.Shippings[0].SenderAddrKbn = CartShipping.AddrKbn.Owner;
				}
			}

			//------------------------------------------------------
			// データバインド
			//------------------------------------------------------
			// 注文者情報、注文配送先情報データバインド
			this.WrCartList.DataSource = this.CartList;
			this.WrCartList.DataBind();

			// 配送方法選択
			this.WrCartList.Items
				.Cast<RepeaterItem>()
				.ToList().ForEach(riCart => SelectShippingMethod(riCart, this.CartList.Items[riCart.ItemIndex]));

			// 各種表示初期化(データバインド後に実行する必要がある)
			InitComponentsDispOrderShipping(e);

			// 国切替初期化
			foreach (var riCart in this.rCartList.Items.Cast<RepeaterItem>())
			{
				var wddlOwnerCountry = GetWrappedControl<WrappedDropDownList>(riCart, "ddlOwnerCountry");
				if (wddlOwnerCountry.HasInnerControl) ddlOwnerCountry_SelectedIndexChanged(wddlOwnerCountry.InnerControl, e);
			}

			if (Constants.PERSONAL_AUTHENTICATION_OF_USER_REGISTRATION_OPTION_ENABLED)
			{
				var wtbAuthenticationCode = GetWrappedTextBoxAuthenticationCode(
					this.CartList.Owner.IsAddrJp,
					this.FirstRpeaterItem);
				var wlbGetAuthenticationCode = GetWrappedControlOfLinkButtonAuthenticationCode(
					this.CartList.Owner.IsAddrJp,
					this.FirstRpeaterItem);

				if (this.IsLoggedIn)
				{
					var telNew = this.CartList.Owner.IsAddrJp
						? this.WtbOwnerTel1.Text
						: this.WtbOwnerTel1Global.Text;

					// Check if phone number has not changed
					if (this.CartList.Owner.Tel1 == telNew)
					{
						this.HasAuthenticationCode = true;
						wlbGetAuthenticationCode.Enabled = false;
						this.AuthenticationCode = string.Empty;
					}
				}
				else
				{
					this.HasAuthenticationCode = false;
				}

				if (string.IsNullOrEmpty(this.CartList.AuthenticationCode) == false)
				{
					this.HasAuthenticationCode = this.CartList.HasAuthenticationCode;

					wtbAuthenticationCode.Text
						= this.AuthenticationCode
						= this.CartList.AuthenticationCode;
					wtbAuthenticationCode.Enabled
						= wlbGetAuthenticationCode.Enabled
						= false;
				}
			}

			// 注文者情報・配送先グローバル関連項目設定
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// 注文者情報グローバル関連項目設定
				SetOrderOwnerGlobalColumn();

				// 配送先グローバル関連項目設定
				this.WrCartList.Items
					.Cast<RepeaterItem>()
					.ToList().ForEach(riCart => SetOrderShippingGlobalColumn(riCart, this.CartList.Items[riCart.ItemIndex]));
			}

			// 配送サービス選択初期化
			foreach (RepeaterItem riCart in this.WrCartList.Items)
			{
				SelectDeliveryCompany(riCart, this.CartList.Items[riCart.ItemIndex]);
			}

			// レコメンド商品投入時は、次へボタンクリックを行いエラーを表示させる
			if (this.IsAddRecmendItem)
			{
				lbNext_Click(sender, e);
			}

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// 後付款(TriLink後払い)のエラーが残っている場合、エラーメッセージを初期化する
				if ((CartList.Items[0].Payment != null)
					&& (string.IsNullOrEmpty(CartList.Items[0].Payment.PaymentId) == false)
					&& (CartList.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
					&& (string)Session[Constants.SESSION_KEY_ORDER_ERROR_MESSAGE] == CommerceMessages.GetMessages(
						CommerceMessages.ERRMSG_FRONT_USER_DEFAULT_PAYMENT_SETTING_INVALID_FOR_USER_TRYLINK_AFTERPAY2))
				{
					Session[Constants.SESSION_KEY_ORDER_ERROR_MESSAGE] = null;
				}

				var wddlOwnerCountry = GetWrappedControl<WrappedDropDownList>(this.WrCartList.Items.Cast<RepeaterItem>().ToList()[0], "ddlOwnerCountry");
				if (string.IsNullOrEmpty(AmazonUtil.GetAmazonUserIdByUseId(this.LoginUserId)) == false)
				{
					var whgcCountryAlertMessage = GetWrappedControl<WrappedHtmlGenericControl>(this.WrCartList.Items.Cast<RepeaterItem>().ToList()[0], "countryAlertMessage");

					// Amazonログイン連携ユーザーの場合国をJapan固定にする
					if (wddlOwnerCountry.InnerControl != null) wddlOwnerCountry.InnerControl.Enabled = false;
					whgcCountryAlertMessage.Visible = true;
				}
			}

			// デフォルト注文系エラーメッセージ表示
			ShowErrorMessageForUserDefaultOrderSetting((string)Session[Constants.SESSION_KEY_ORDER_ERROR_MESSAGE]);

			// 配送先不可エリアメッセージがあれば表示する
			ShowShippingAreaErrorMessage(rCartList);
			SessionManager.UnavailableShippingErrorMessage = null;
		}

		// カスタムバリデータ属性値更新
		UpdateAttributeValueForCustomValidator();

		if (Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
		{
			DisplayShopData();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitComponents()
	{
		//----------------------------------------------------
		// 配送情報入力画面初期処理（共通）
		//----------------------------------------------------
		InitComponentsOrderShipping();

		//----------------------------------------------------
		// 次へボタン設定
		//----------------------------------------------------
		// 確認画面から遷移したと判断した場合は次画面は確認画面
		if ((Request.UrlReferrer != null)
			&& (Request.UrlReferrer.ToString().ToLower().Contains(Constants.PAGE_FRONT_ORDER_CONFIRM.ToLower())))
		{
			var isNeedNextPageOrderPayment = ((CheckCartConvenienceStoreAndPayment()
					&& Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
				|| (CheckCountryIsoCodeCanOrderWithNPAfterPay(this.CartList) == false));

			this.CartList.CartNextPage = (isNeedNextPageOrderPayment)
					? Constants.PAGE_FRONT_ORDER_PAYMENT
					: Constants.PAGE_FRONT_ORDER_CONFIRM;

			// レコメンド商品追加時？
			if (this.IsAddRecmendItem)
			{
				this.CartList.CartNextPage = (Constants.GIFTORDER_OPTION_ENABLED 
					&& (this.ContainsOnlyDigitalContentsInCarts() == false))
						? Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING
						: Constants.PAGE_FRONT_ORDER_PAYMENT;
			}
		}
		else if (Constants.GIFTORDER_OPTION_ENABLED)
		{
			this.CartList.CartNextPage = (this.ContainsOnlyDigitalContentsInCarts() == false) 
				? Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING
				: Constants.PAGE_FRONT_ORDER_PAYMENT;
		}
		else
		{
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_PAYMENT;
		}

		// 注文同梱カートで頒布会が含まれる場合、注文確認画面に遷移
		// ※注文同梱の場合、異なる頒布会コース同梱、または定期と頒布会の同梱パターンしかこの画面に来ない想定
		if (this.CartList.Items[0].IsOrderCombined && this.CartList.Items[0].HasSubscriptionBox)
		{
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_CONFIRM;
		}

		// デフォルト支払方法の設定があり、レコメンド商品追加時でない場合、画面遷移先を注文確認ページに設定
		var userDefaultOrderSetting = Constants.TWOCLICK_OPTION_ENABLE ? new UserDefaultOrderSettingService().Get(this.LoginUserId) : null;
		if ((userDefaultOrderSetting != null) && (this.IsAddRecmendItem == false) && (Constants.GIFTORDER_OPTION_ENABLED == false))
		{
			// PayPalで紐づけが外れていた場合は遷移先変更しない
			if ((userDefaultOrderSetting.PaymentId != null)
				&& ((userDefaultOrderSetting.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL) || (SessionManager.PayPalCooperationInfo != null)))
			{
				this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_CONFIRM;
			}
		}
	}

	/// <summary>
	/// Check Cart ConvenienceStore And Payment
	/// </summary>
	/// <returns>True: shipping Kbn is not convenience store and payment id is G33</returns>
	public bool CheckCartConvenienceStoreAndPayment()
	{
		var result = this.CartList.Items.Any(
			cart => cart.Shippings.Any(
				shipping =>
					(shipping.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF))
						&& ((cart.Payment != null)
							&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE)));

		// Check payment: convenience store and shipping receiving store type: Familymart payment, 7-ELEVENT payment, Hi-Life payment
		if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED && (result == false))
		{
			var isConvenienceStoreEcPayPayment = this.CartList.Items.Any(cart =>
				(cart.Shippings.Any(
					shipping =>
						(shipping.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
						&& (ECPayUtility.GetIsCollection(shipping.ShippingReceivingStoreType) == Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON))
				&& ((cart.Payment != null)
					&& (cart.Payment.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE))));

			var isConvenienceStoreEcPay = this.CartList.Items.Any(cart =>
				(cart.Shippings.Any(
					shipping =>
						(shipping.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
						&& (ECPayUtility.GetIsCollection(shipping.ShippingReceivingStoreType) == Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_OFF))
				&& ((cart.Payment != null)
					&& (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE))));

			result = (isConvenienceStoreEcPayPayment || isConvenienceStoreEcPay);
		}

		return result;
	}

	/// <summary>
	/// 戻るリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, System.EventArgs e)
	{
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_CART_LIST;
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
	}

	/// <summary>
	/// 次へリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbNext_Click(object sender, System.EventArgs e)
	{
		//------------------------------------------------------
		// 注文配送情報入力画面次へリンククリック処理（共通）
		//------------------------------------------------------
		// 共通処理失敗時はこれ以降の処理を行わない
		if (lbNext_Click_OrderShipping_Owner(sender, e) == false) return;

		if (Constants.GIFTORDER_OPTION_ENABLED == false)
		{
			if (lbNext_Click_OrderShipping_Shipping(sender, e) == false) return;

			// Display shipping date error message if exising
			if (DisplayShippingDateErrorMessage()) return;

			if (lbNext_Click_OrderShipping_Others(sender, e) == false) return;
		}
		else
		{
			this.CartList.Items.ForEach(
				co =>
				{
					co.Shippings.ForEach(
						cs =>
						{
							// 配送先の同期
							if (cs.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER)
							{
								//「配送先が注文者の住所」となっている配送先を同期
								cs.UpdateShippingAddr(this.CartList.Owner, false);
							}

							// 送り主の同期
							if ((co.Shippings.Count > 1) && cs.IsSameSenderAsShipping1)
							{
								//「配送先1と同じ送り主を指定する」となっている送り主を同期
								cs.UpdateSenderAddr(cs.CartObject.Shippings[0], true);
							}
							else if ((cs.SenderAddrKbn == CartShipping.AddrKbn.Owner)
								&& (cs.IsSameSenderAsShipping1 == false))
							{
								//「注文者を送り主とする」となっている送り主を同期
								cs.UpdateSenderAddr(this.CartList.Owner, false);
							}
						});
				});
		}

		// Display shipping date error message if exising
		if (DisplayShippingDateErrorMessage()) return;

		// Check need redirect to page order payment if cart next page is set page order comfirm
		CheckNeedRedirectToPageOrderPaymentProcess();

		// ギフトカートの場合、この画面では配送先エリアチェックはしない
		if (Constants.GIFTORDER_OPTION_ENABLED == false)
		{
			var hasError = false;
			foreach (var cart in this.CartList.Items)
			{
				var unavailableShippingZip = new ShopShippingService().GetUnavailableShippingZipFromShippingDelivery(
					cart.ShippingType,
					cart.Shippings[0].DeliveryCompanyId);

				if (OrderCommon.CheckUnavailableShippingArea(unavailableShippingZip, cart.Shippings[0].HyphenlessZip))
				{
					hasError = true;
				}
			}

			// 配送不可エリアエラーの場合、エラーメッセージ表示
			if (hasError)
			{
				SessionManager.UnavailableShippingErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ZIPCODE_UNAVAILABLE_SHIPPING_AREA);
				ShowShippingAreaErrorMessage(rCartList);
				return;
			}
		}

		//------------------------------------------------------
		// 画面遷移
		//------------------------------------------------------
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = this.CartList.CartNextPage;
		var isNeedNextPageOrderPayment = ((CheckCartConvenienceStoreAndPayment()
				&& Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED)
			|| (CheckCountryIsoCodeCanOrderWithNPAfterPay(this.CartList) == false)
			|| (CheckCountryIsoCodeCanOrderWithECPay(this.CartList) == false));

		foreach (var item in this.CartList.Items)
		{
			if (item.Shippings.Any(shipping => shipping.ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP)
				&& (item.Payment != null)
				&& Constants.SETTING_CAN_NOT_STORE_PICKUP_OPTION_PAYMENT_IDS.Contains(item.Payment.PaymentId))
			{
				isNeedNextPageOrderPayment = true;
				break;
			}
		}

		if (isNeedNextPageOrderPayment)
		{
			var addItemFlg = this.IsAddRecmendItem ? string.Format("?{0}=1", Constants.REQUEST_KEY_ADD_RECOMMEND_ITEM_FLG) : string.Empty;
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_PAYMENT;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_PAYMENT + addItemFlg);
		}
		else
		{
			//------------------------------------------------------
			// 次のページへ遷移
			//------------------------------------------------------
			var addItemFlg = this.IsAddRecmendItem ? string.Format("?{0}=1", Constants.REQUEST_KEY_ADD_RECOMMEND_ITEM_FLG) : string.Empty;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + this.CartList.CartNextPage + addItemFlg);
		}
	}

	/// <summary>
	/// カスタムバリデータの属性値を変更する（EFOオプションONのとき、カスタムバリデータを無効化する）
	/// </summary>
	public void UpdateAttributeValueForCustomValidator()
	{
		// EFOオプションチェック（有効な場合、カスタムバリデータを無効化）
		if (this.IsEfoOptionEnabled == false)
		{
			SetCustomValidatorControlInformationList(this);
			return;
		}

		var searchTag = new List<string>
		{
			"cvOwnerName1",
			"cvOwnerName2",
			"cvOwnerName1",
			"cvOwnerName2",
			"cvOwnerNameKana1",
			"cvOwnerNameKana2",
			"cvOwnerBirth",
			"cvOwnerSex",
			"cvOwnerMailAddr",
			"cvOwnerMailAddrForCheck",
			"cvOwnerMailAddrConf",
			"cvOwnerMailAddr2",
			"cvOwnerMailAddr2Conf",
			"cvOwnerZip1",
			"cvOwnerZip2",
			"cvOwnerAddr1",
			"cvOwnerAddr2",
			"cvOwnerAddr3",
			"cvOwnerTel1_1",
			"cvOwnerTel1_2",
			"cvOwnerTel1_3",
			"cvOwnerTel2_1",
			"cvOwnerTel2_2",
			"cvOwnerTel2_3",
			"cvOwnerCountry",
			"cvOwnerAddr5Ddl",
			"cvRealShopName"
		};
		var repeaterItem = this.WrCartList.Items.Cast<RepeaterItem>().First();
		var customValidatorControls = searchTag
			.Select(target => GetWrappedControl<WrappedCustomValidator>(repeaterItem, target))
			.ToList();

		var searchRepTag = new List<string>
		{
			"cvShippingName1",
			"cvShippingName2",
			"cvShippingNameKana1",
			"cvShippingNameKana2",
			"cvShippingZip1",
			"cvShippingZip2",
			"cvShippingAddr1",
			"cvShippingAddr2",
			"cvShippingAddr3",
			"cvShippingTel1_1",
			"cvShippingTel1_2",
			"cvShippingTel1_3",
			"cvUserShippingName",
			"cvFixedPurchaseMonth",
			"cvFixedPurchaseMonthlyDate",
			"cvFixedPurchaseWeekOfMonth",
			"cvFixedPurchaseDayOfWeek",
			"cvFixedPurchaseIntervalDays",
			"cvShippingCountry",
			"cvShippingAddr5Ddl",
			"cvRealShopName",
		};
		customValidatorControls.AddRange(
			this.WrCartList.Items
				.Cast<RepeaterItem>()
				.ToList()
				.SelectMany(rpItem =>
					searchRepTag.Select(tag => GetWrappedControl<WrappedCustomValidator>(rpItem, tag))));
		SetDisableAndHideCustomValidatorControlInformationList(customValidatorControls);
	}

	/// <summary>
	/// Link button get authentication code click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbGetAuthenticationCode_Click(object sender, EventArgs e)
	{
		this.Process.lbGetAuthenticationCode_Click(sender, e);
	}

	/// <summary>
	/// Link button check authentication code click
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCheckAuthenticationCode_Click(object sender, EventArgs e)
	{
		this.Process.lbCheckAuthenticationCode_Click(sender, e);
	}

	/// <summary>
	/// 初期値男性で性別を取得（データバインド用）
	/// </summary>
	/// <returns>性別</returns>
	protected string GetCorrectSexForDataBindDefault()
	{
		return this.Process.GetCorrectSexForDataBindDefault();
	}

	/// <summary>
	/// Dropdownlist real shop area on databound
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlRealShopArea_DataBound(object sender, EventArgs e)
	{
		this.Process.ddlRealShopArea_DataBound(sender, e);
	}

	/// <summary>
	/// コンビニ受取店舗情報で配送先情報を更新
	/// </summary>
	/// <remarks>台湾ファミマコンビニ受取利用時</remarks>
	public void UpdateCartShippingByReceivingStore()
	{
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			var whfCvsShopId = GetWrappedControl<WrappedHiddenField>(riCart, "hfCvsShopId");
			var whfCvsShopName = GetWrappedControl<WrappedHiddenField>(riCart, "hfCvsShopName");
			var whfCvsShopAddress = GetWrappedControl<WrappedHiddenField>(riCart, "hfCvsShopAddress");
			var whfCvsShopTel = GetWrappedControl<WrappedHiddenField>(riCart, "hfCvsShopTel");
			if (whfCvsShopId != null)
			{
				var cart = this.CartList.Items[riCart.ItemIndex];
				var cartShipping = cart.Shippings[0];
				cartShipping.UpdateConvenienceStoreAddr(
					CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE,
					whfCvsShopId.Value,
					whfCvsShopName.Value,
					whfCvsShopAddress.Value,
					whfCvsShopTel.Value,
					cartShipping.ShippingReceivingStoreType);
			}
		}
	}

	#region 古い形式のメソッド（非推奨）
	///=============================================================================================
	/// <summary>
	/// 配送日設定可能状態取得
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <returns>indexで指定したカートの配送日設定可能フラグが有効かどうか</returns>
	///=============================================================================================
	[Obsolete("[V5.2] CanInputDateSet() を使用してください")]
	protected bool IsDateSetFlgValid(int index)
	{
		return GetShippingDateSetFlgValid(index);
	}
	///=============================================================================================
	/// <summary>
	/// 配送希望時間帯設定可能状態取得
	/// </summary>
	/// <param name="index">rCartListのインデックス</param>
	/// <returns>indexで指定したカートの配送希望時間帯設定可能フラグが有効かどうか</returns>
	///=============================================================================================
	[Obsolete("[V5.2] CanInputTimeSet() を使用してください")]
	protected bool IsTimeSetFlgValid(int index)
	{
		return GetShippingTimeSetFlgValid(index);
	}
	#endregion

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
		get { return "javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + lbNext.UniqueID + "', '', true, '" + lbNext.ValidationGroup + "', '', false, true))"; }
	}
	/// <summary>次へ進むonclick</summary>
	protected string NextOnClick
	{
		get { return Constants.RECEIVINGSTORE_TWPELICAN_CVSOPTION_ENABLED ? "return CheckBeforeNextPage();" : "return true;"; }
	}
}
