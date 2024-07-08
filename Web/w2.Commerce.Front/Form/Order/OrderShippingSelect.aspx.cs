/*
=========================================================================================================
  Module      : 注文配送選択画面(OrderShippingSelect.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Amazon.Util;
using w2.App.Common.Order;
using w2.App.Common.User;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.User;

/// <summary>
/// 注文配送選択画面
/// </summary>
public partial class Form_Order_OrderShippingSelect : OrderCartPage
{
	/// <summary>ページアクセスタイプ</summary>
	public override PageAccessTypes PageAccessType { get { return PageAccessTypes.Https; } }

	#region ラップ済コントロール宣言
	/// <summary>Wrapped link button back</summary>
	protected WrappedLinkButton WlbBack { get { return GetWrappedControl<WrappedLinkButton>("lbBack"); } }
	/// <summary>Wrapped link button next</summary>
	protected WrappedLinkButton WlbNext { get { return GetWrappedControl<WrappedLinkButton>("lbNext"); } }
	/// <summary>注文者国情報ドロップダウンリスト</summary>
	protected WrappedDropDownList WddlOwnerCountry { get { return GetWrappedControl<WrappedDropDownList>(this.FirstRpeaterItem, "ddlOwnerCountry"); } }

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
	/// <summary>Wrapped html generic error message</summary>
	protected WrappedHtmlGenericControl WhcErrorMessage { get { return GetWrappedControl<WrappedHtmlGenericControl>("hcErrorMessage"); } }
	/// <summary>Wrapped literal invalid gift cart error message</summary>
	protected WrappedLiteral WlInvalidGiftCartErrorMessage { get { return GetWrappedControl<WrappedLiteral>("lInvalidGiftCartErrorMessage"); } }
	/// <summary>Wrapped hidden field open cart index</summary>
	protected WrappedHiddenField WhfOpenCartIndex { get { return GetWrappedControl<WrappedHiddenField>("hfOpenCartIndex"); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.IsGiftPage = true;
		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);
			if (this.CartList.Items[0].Items.Count == 0) return;

			InitComponentsOrderShipping();

			this.Process.PrepareForDataBindOrderShipping(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);

			CreateFixedPurchaseSettings();

			this.WrCartList.DataSource = this.CartList;
			this.WrCartList.DataBind();

			RefreshAll(e);
			return;
		}

		RedirectOrderShipping();

		if (Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED == false)
		{
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SHIPPING;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SHIPPING);
		}

		// HTTPS通信チェック（HTTP通信の場合、ショッピングカートへ）
		CheckHttps();

		// カートチェック（カートが存在しない場合、エラーページへ）
		CheckCartExists();

		this.Process.CreateShippingMethodListOnDataBind();

		if (!IsPostBack
			|| this.IsPostBackFromAction)
		{
			CheckOrderUrlSession();

			//ユーザーが退会済みでないか確認
			if (string.IsNullOrEmpty(this.LoginUserId) == false)
			{
				var user = DomainFacade.Instance.UserService.Get(this.LoginUserId);
				if ((user != null) && user.IsDeleted)
				{
					Session.Contents.RemoveAll();
					CookieManager.RemoveCookie(Constants.COOKIE_KEY_AUTH_KEY, Constants.PATH_ROOT);
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_SESSION_VANISHED);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}

			// カート内商品チェック（三分岐でログインしたときに会員価格が割り当てられたときはカートに戻る）
			try
			{
				ProductCheckCartList();
			}
			catch (OrderException)
			{
				Response.Redirect(this.UnsecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
			}

			// 新たにログインした場合はOwnerリセット
			if (this.IsLoggedIn)
			{
				if ((this.CartList.Owner != null)
					&& ((this.CartList.Owner.OwnerKbn == null)
						|| UserService.IsGuest(this.CartList.Owner.OwnerKbn)))
				{
					this.CartList.SetOwner(coOwner: null);
				}
			}

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				UserUtility.AdjustPointAndMemberRankByCrossPointApi(this.LoginUser);
			}

			CreateOrderOwner();

			SetFixedPurchaseMemberFlgForCartObject(this.CartList.Items.ToList());

			CreateOrderShipping();

			// Add Cart Novelty When Register New User from page OrderOwnerDecision.aspx
			if (Constants.NOVELTY_OPTION_ENABLED)
			{
				var cartNoveltyList = new CartNoveltyList(this.CartList);
				var cartItemCountBefore = this.CartList.Items.Count;
				AddProductGrantNovelty(cartNoveltyList);

				if (cartItemCountBefore != this.CartList.Items.Count) cartNoveltyList = new CartNoveltyList(this.CartList);

				this.CartList.RemoveNoveltyGrantItem(cartNoveltyList);
				cartNoveltyList.RemoveCartNovelty(this.CartList);
			}

			this.UserKbn = this.CartList.Owner.OwnerKbn;

			InitComponents();
			CreateOrderMemo();

			this.Process.PrepareForDataBindOrderShipping(Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);

			CreateFixedPurchaseSettings();
			if (!IsPostBack)
			{
				this.WrCartList.DataSource = this.CartList;
				this.WrCartList.DataBind();
				this.WrCartList.Items
					.Cast<RepeaterItem>()
					.ToList().ForEach(riCart => SelectShippingMethod(riCart, this.CartList.Items[riCart.ItemIndex]));
			}

			// 各種表示初期化(データバインド後に実行する必要がある)
			InitComponentsDispOrderShipping(e);

			// 国切替初期化
			foreach (var riCart in this.WrCartList.Items.Cast<RepeaterItem>())
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
						? this.WtbOwnerTel1.Text.Trim()
						: this.WtbOwnerTel1Global.Text.Trim();

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

			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				SetOrderOwnerGlobalColumn();

				this.WrCartList.Items
					.Cast<RepeaterItem>()
					.ToList().ForEach(riCart => SetOrderShippingGlobalColumn(riCart, this.CartList.Items[riCart.ItemIndex]));
			}

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
				if ((this.CartList.Items[0].Payment != null)
					&& (string.IsNullOrEmpty(CartList.Items[0].Payment.PaymentId) == false)
					&& (this.CartList.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY)
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
					if (wddlOwnerCountry.HasInnerControl) wddlOwnerCountry.InnerControl.Enabled = false;
					whgcCountryAlertMessage.Visible = true;
				}
			}

			ShowErrorMessageForUserDefaultOrderSetting((string)Session[Constants.SESSION_KEY_ORDER_ERROR_MESSAGE]);
			ShowShippingAreaErrorMessage(this.WrCartList.InnerControl);
			SessionManager.UnavailableShippingErrorMessage = null;

			RefreshAll(e);
			this.IsRefreshCartShippings = false;
			this.IsPostBackFromAction = false;
		}

		UpdateAttributeValueForCustomValidator();
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitComponents()
	{
		InitComponentsOrderShipping();

		// 次へボタン設定
		// 確認画面から遷移したと判断した場合は次画面は確認画面
		if ((Request.UrlReferrer != null)
			&& (Request.UrlReferrer.ToString().IndexOf(Constants.PAGE_FRONT_ORDER_CONFIRM, StringComparison.OrdinalIgnoreCase) != -1))
		{
			var isNeedNextPageOrderPayment = CheckCountryIsoCodeCanOrderWithNPAfterPay(this.CartList) == false;

			this.CartList.CartNextPage = (isNeedNextPageOrderPayment)
				? Constants.PAGE_FRONT_ORDER_PAYMENT
				: Constants.PAGE_FRONT_ORDER_CONFIRM;

			if (this.IsAddRecmendItem
				&& (this.ContainsOnlyDigitalContentsInCarts() == false))
			{
				this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_PAYMENT;
			}
		}
		else
		{
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_PAYMENT;
		}

		// デフォルト支払方法の設定があり、レコメンド商品追加時でない場合、画面遷移先を注文確認ページに設定
		var userDefaultOrderSetting = Constants.TWOCLICK_OPTION_ENABLE
			? DomainFacade.Instance.UserDefaultOrderSettingService.Get(this.LoginUserId)
			: null;
		if ((userDefaultOrderSetting != null)
			&& (this.IsAddRecmendItem == false))
		{
			// PayPalで紐づけが外れていた場合は遷移先変更しない
			if ((userDefaultOrderSetting.PaymentId != null)
				&& ((userDefaultOrderSetting.PaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL)
					|| (SessionManager.PayPalCooperationInfo != null)))
			{
				this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_CONFIRM;
			}
		}

		var openCartIndex = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_ORDER_SHIPPING_SELECT_OPEN_INDEX] ?? "0");
		this.WhfOpenCartIndex.Value = openCartIndex;
		Session[Constants.SESSION_KEY_ORDER_SHIPPING_SELECT_OPEN_INDEX] = openCartIndex;
	}

	/// <summary>
	/// 戻るリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, EventArgs e)
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
	protected void lbNext_Click(object sender, EventArgs e)
	{
		if (GetAndCheckInputData(sender, e) == false) return;

		// 配送先が１つなら自動紐付け
		foreach (var cart in this.CartList.Items)
		{
			if (cart.Shippings.Count == 1)
			{
				cart.Shippings[0].ProductCounts.Clear();
				foreach (var cartProduct in cart.Items)
				{
					cart.Shippings[0].ProductCounts.Add(
						new CartShipping.ProductCount(cartProduct, cartProduct.Count));
				}

				continue;
			}

			if (cart.IsGift == false) continue;

			var firstShipping = cart.Shippings[0];
			for (var shippingIndex = cart.Shippings.Count - 1; shippingIndex > -1; shippingIndex--)
			{
				var currentShipping = cart.Shippings[shippingIndex];
				for (var productCountIndex = currentShipping.ProductCounts.Count - 1; productCountIndex > -1; productCountIndex--)
				{
					if (currentShipping.ProductCounts[productCountIndex].Count != 0) continue;
					currentShipping.ProductCounts.RemoveAt(productCountIndex);
				}

				if (currentShipping.ProductCounts.Count != 0) continue;
				cart.Shippings.RemoveAt(shippingIndex);
			}

			// Link shippinng if there is no product in cart shipping product
			if (cart.Shippings.Count != 0) continue;

			foreach (var cartProduct in cart.Items)
			{
				firstShipping.ProductCounts.Add(
					new CartShipping.ProductCount(cartProduct, cartProduct.Count));
			}
			cart.Shippings.Add(firstShipping);
		}

		var changeToNormalCartTargets = this.CartList.Items
			.Where(cart => cart.IsGift
				&& (cart.Shippings.Count == 1))
			.Select(cart => cart.CartId)
			.ToArray();
		for (var index = changeToNormalCartTargets.Length - 1; index >= 0; index--)
		{
			var targetCart = this.CartList.Items
				.First(cart => cart.CartId == changeToNormalCartTargets.ElementAt(index));
			ChangeToNormalCart(targetCart, -1);
		}

		foreach (var cart in this.CartList.Items)
		{
			if (cart.Shippings.Count != 1) continue;

			cart.Shippings[0].ProductCounts.Clear();
			foreach (var cartProduct in cart.Items)
			{
				cart.Shippings[0].ProductCounts.Add(
					new CartShipping.ProductCount(cartProduct, cartProduct.Count));
			}
		}

		// カート商品未割り当てチェック・削除
		foreach (var cart in this.CartList.Items)
		{
			if (cart.IsGift == false) continue;

			var removeList = new List<CartProduct>();
			foreach (var cartProduct in cart.Items)
			{
				if (cart.Shippings.SelectMany(shipping => shipping.ProductCounts).All(product => product.Product != cartProduct))
				{
					removeList.Add(cartProduct);
				}
			}
			cart.Items.RemoveAll(cp => removeList.Contains(cp));
			cart.CalculateWithCartShipping();
		}

		var nextPage = this.CartList.CartNextPage;
		InitializeComponentOnRebind();
		this.WrCartList.DataSource = this.CartList;
		this.WrCartList.DataBind();
		// Set cart list next page after initialize component
		this.CartList.CartNextPage = nextPage;

		CheckCartOwnerShippingArea();
		CheckCartData();
		if (this.ErrorMessages.Count != 0)
		{
			WhcErrorMessage.InnerText = this.ErrorMessages.Get();
			this.MaintainScrollPositionOnPostBack = false;
			Reload();
		}

		if (DisplayShippingDateErrorMessage()) Reload();

		// すべてのカートを再計算
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			RecalculateProductCount(riCart);
			RefreshCartProducts(riCart);
		}

		var invalidGiftCartErrorMessage = CheckInvalidGiftCartError();
		if (string.IsNullOrEmpty(invalidGiftCartErrorMessage) == false)
		{
			RefreshAll(e);
			this.WlInvalidGiftCartErrorMessage.Text = WebSanitizer.HtmlEncodeChangeToBr(invalidGiftCartErrorMessage);
			this.WlInvalidGiftCartErrorMessage.Visible = true;
			this.WhfOpenCartIndex.Value = "0";
			Session[Constants.SESSION_KEY_ORDER_SHIPPING_SELECT_OPEN_INDEX] = "0";
			return;
		}

		// 決済が使えない状態になっていたら決済入力画面へ遷移する
		if (CheckPayment())
		{
			this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_PAYMENT;
		}

		// Check need redirect to page order payment if cart next page is set page order comfirm
		CheckNeedRedirectToPageOrderPaymentProcess();

		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = this.CartList.CartNextPage;
		Session[Constants.SESSION_KEY_ORDER_SHIPPING_SELECT_OPEN_INDEX] = "0";
		var isNeedNextPageOrderPayment = (CheckCountryIsoCodeCanOrderWithNPAfterPay(this.CartList) == false)
			|| (CheckCountryIsoCodeCanOrderWithECPay(this.CartList) == false);
		if (isNeedNextPageOrderPayment)
		{
			var addItemFlg = this.IsAddRecmendItem
				? string.Format("?{0}=1", Constants.REQUEST_KEY_ADD_RECOMMEND_ITEM_FLG)
				: string.Empty;
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_PAYMENT;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_PAYMENT + addItemFlg);
		}
		else
		{
			var addItemFlg = this.IsAddRecmendItem
				? string.Format("?{0}=1", Constants.REQUEST_KEY_ADD_RECOMMEND_ITEM_FLG)
				: string.Empty;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + this.CartList.CartNextPage + addItemFlg);
		}
	}

	/// <summary>
	/// 配送先追加リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbAddShipping_Click(object sender, EventArgs e)
	{
		lbNext_Click_OrderShipping_Owner(sender, e);

		GetGiftWrappingPaperAndBagInput();
		var lbAddNewShipping = (LinkButton)sender;
		var riCart = (RepeaterItem)GetOuterControl(lbAddNewShipping, typeof(RepeaterItem));
		var cart = this.CartList.Items[riCart.ItemIndex];
		var cartShipping = new CartShipping(cart)
		{
			ShippingAddrKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW,
			ShippingMethod = cart.Shippings[0].ShippingMethod,
			DeliveryCompanyId = cart.Shippings[0].DeliveryCompanyId
		};

		foreach (var cartProduct in cart.Items)
		{
			cartShipping.ProductCounts.Add(new CartShipping.ProductCount(cartProduct, 0));
		}

		// カートへ紐付け（＆メモ作成）
		var cartShippings = GetCurrentDisplayShippings(riCart);
		cartShipping.IsSameSenderAsShipping1 = cartShippings.Count > 0;

		if (cart.IsGift)
		{
			cartShippings.Add(cartShipping);
			cart.Shippings.Add(cartShipping);
		}

		cart.UpdateCartShippingsAddr(cartShippings);
		var isDetach = DetachCart(cart, riCart.ItemIndex);

		foreach (var product in cart.Items)
		{
			product.SetCountBeforeDivide();
		}

		var openIndex = cart.Shippings.Count - 1;
		if (isDetach)
		{
			openIndex++;
		}

		this.WhfOpenCartIndex.Value = openIndex.ToString();
		Session[Constants.SESSION_KEY_ORDER_SHIPPING_SELECT_OPEN_INDEX] = openIndex.ToString();
		ReloadByPageLoad(sender, e);
	}

	/// <summary>
	/// 配送先削除リンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbDeleteShipping_Click(object sender, EventArgs e)
	{
		lbNext_Click_OrderShipping_Owner(sender, e);

		var lbDeleteShipping = (LinkButton)sender;
		var riShipping = (RepeaterItem)GetOuterControl(lbDeleteShipping, typeof(RepeaterItem));
		var riCart = (RepeaterItem)GetOuterControl(riShipping, typeof(RepeaterItem));

		// 配送先削除
		var shippingIndex = int.Parse(lbDeleteShipping.CommandArgument);
		var currentCart = this.CartList.Items[riCart.ItemIndex];
		var cartShippings = GetCurrentDisplayShippings(riCart);
		var deleteShipping = currentCart.Shippings[shippingIndex];
		if (cartShippings.Count > 2)
		{
			var affectCartShippingIndex = shippingIndex == 0 ? 1 : 0;
			var affectCartShipping = currentCart.Shippings[affectCartShippingIndex];
			foreach (var productCount in deleteShipping.ProductCounts)
			{
				var findProductCount = affectCartShipping.ProductCounts
					.FirstOrDefault(affectProductCount =>
						IsSameProduct(affectProductCount.Product, productCount.Product));
				if (findProductCount == null) continue;
				findProductCount.Count += productCount.Count;
			}
		}

		if (shippingIndex < this.CartList.Items[riCart.ItemIndex].Shippings.Count)
		{
			currentCart.Shippings.RemoveAt(shippingIndex);
		}

		cartShippings.RemoveAt(shippingIndex);
		if ((shippingIndex == 0)
			&& (cartShippings.Count > 0))
		{
			// これをしないとのっぺらぼうの送り主表示になる
			currentCart.Shippings[shippingIndex].IsSameSenderAsShipping1
				= cartShippings[shippingIndex].IsSameSenderAsShipping1
				= false;
		}

		Session[Constants.SESSION_KEY_ORDER_SHIPPING_SELECT_OPEN_INDEX] = "0";
		if (ChangeToNormalCart(this.CartList.Items[riCart.ItemIndex], riCart.ItemIndex))
		{
			Reload();
		}

		InitializeComponentOnRebind();
		RecalculateProductCount(riCart);
		this.WrCartList.DataSource = this.CartList;
		this.WrCartList.DataBind();
		RefreshAll(e);
		if (GetAndCheckInputData(sender, e) == false)
		{
			ReloadByPageLoad(sender, e);
			return;
		}

		this.WhfOpenCartIndex.Value = "0";
	}

	/// <summary>
	/// Reload by page load
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ReloadByPageLoad(object sender, EventArgs e)
	{
		this.IsPostBackFromAction = true;
		Page_Load(sender, e);
		if (this.CartList.Owner.IsAddrJp)
		{
			WtbOwnerTel1.Text = this.CartList.Owner.Tel1;
		}
		else
		{
			WtbOwnerTel1Global.Text = this.CartList.Owner.Tel1;
		}
	}

	/// <summary>
	/// 再計算リンククリック（実際はテキストボックスのフォーカス外れ）
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbRecalculateCart_Click(object sender, EventArgs e)
	{
		var isPlusButton = false;
		var isChangeByButton = false;
		if (sender is LinkButton)
		{
			var argument = ((LinkButton)sender).CommandArgument.Split(';');
			if (argument.Length == 0) return;

			isPlusButton = argument[0] == "plus";
			isChangeByButton = (argument[0] == "plus")
				|| (argument[0] == "subtract");
		}

		var riProduct = (sender is LinkButton)
			? GetOuterControl((LinkButton)sender, typeof(RepeaterItem))
			: GetOuterControl((TextBox)sender, typeof(RepeaterItem));
		var riShipping = GetOuterControl(riProduct, typeof(RepeaterItem));
		var riCartList = (RepeaterItem)GetOuterControl(riShipping, typeof(RepeaterItem));
		var wtbCount = GetWrappedControl<WrappedTextBox>(riProduct, "tbProductCount");
		var whcErrorMessage = GetWrappedControl<WrappedHtmlGenericControl>(riShipping, "hcErrorMessage");

		var cart = this.CartList.Items[riCartList.ItemIndex];
		var targetProductCount = (CartShipping.ProductCount)cart.Shippings[riShipping.ItemIndex]
			.ProductCounts[riProduct.ItemIndex];
		int productQuantityInput;
		if (int.TryParse(StringUtility.ToHankaku(wtbCount.Text.Trim()), out productQuantityInput) == false)
		{
			productQuantityInput = targetProductCount.Count;
		}

		if (isChangeByButton)
		{
			productQuantityInput += isPlusButton ? 1 : -1;
			if (productQuantityInput < 0) productQuantityInput = 0;
		}
		else
		{
			productQuantityInput = (productQuantityInput >= 0) ? productQuantityInput : 0;
		}

		// カート内の対象商品合計取得
		var totalProductCount = cart.Shippings.Sum(shipping =>
			shipping.ProductCounts
				.Where(product => product.Product == targetProductCount.Product)
				.Sum(product => product.Count));
		var productCountTotal = totalProductCount - targetProductCount.Count + productQuantityInput;

		// 商品購入制限チェック
		if (productCountTotal > targetProductCount.Product.ProductMaxSellQuantity)
		{
			this.ErrorMessages.Add(
				riCartList.ItemIndex,
				riShipping.ItemIndex,
				OrderCommon.GetErrorMessage(OrderErrorcode.MaxSellQuantityError));
		}

		if (this.ErrorMessages.Count == 0)
		{
			targetProductCount.Count = productQuantityInput;
		}

		ReloadByCartShippings(sender, e);

		if (this.ErrorMessages.Count != 0)
		{
			whcErrorMessage.Visible = true;

			var product = cart.Shippings[riShipping.ItemIndex].ProductCounts[riProduct.ItemIndex].Product;
			whcErrorMessage.InnerText = this.ErrorMessages.Get(riCartList.ItemIndex, riShipping.ItemIndex)
				.Replace("@@ 1 @@", product.ProductName)
				.Replace("@@ 2 @@", product.ProductMaxSellQuantity.ToString("F0"));
		}
		else
		{
			whcErrorMessage.Visible = false;
		}
	}

	/// <summary>
	/// 各配送先の同一商品数量一括更新用の数量変更ボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbCartSummaryProductCountChange_OnClick(object sender, EventArgs e)
	{
		var riCartProduct = GetParentRepeaterItem((LinkButton)sender, "rCartProduct");
		var wtbCartSummaryProductCount = GetWrappedControl<WrappedTextBox>(riCartProduct, "tbCartSummaryProductCount");
		int quantity;
		if (int.TryParse(StringUtility.ToHankaku(wtbCartSummaryProductCount.Text.Trim()), out quantity) == false)
		{
			quantity = 0;
		}

		switch (((LinkButton)sender).CommandArgument)
		{
			case "plus":
				quantity++;
				break;

			case "subtract":
				if (quantity > 0) quantity--;
				break;
		}

		wtbCartSummaryProductCount.Text = quantity.ToString();
	}

	/// <summary>
	/// 各配送先の同一商品数量一括変更ボタン
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbProductCountBulkChange_OnClick(object sender, EventArgs e)
	{
		var riCartProduct = GetParentRepeaterItem((LinkButton)sender, "rCartProduct");
		var wtbCartSummaryProductCount = GetWrappedControl<WrappedTextBox>(riCartProduct, "tbCartSummaryProductCount");
		int quantity;
		if (int.TryParse(StringUtility.ToHankaku(wtbCartSummaryProductCount.Text.Trim()), out quantity) == false)
		{
			quantity = 0;
		}

		var riCartList = GetParentRepeaterItem((LinkButton)sender, "rCartList");
		var cart = this.CartList.Items[riCartList.ItemIndex];
		var targetProductCounts = cart.Shippings
			.SelectMany(shipping => shipping.ProductCounts
				.Where(productCount => productCount.Product.CheckSameProduct(cart.Items[riCartProduct.ItemIndex])));
		foreach (var productCounts in targetProductCounts)
		{
			productCounts.Count = quantity;
		}

		ReloadByCartShippings(sender, e);
	}

	/// <summary>
	/// 配送先情報更新による再計算処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ReloadByCartShippings(object sender, EventArgs e)
	{
		var riCartList = GetParentRepeaterItem((Control)sender, "rCartList");

		RecalculateProductCount(riCartList);
		Session[Constants.SESSION_KEY_ORDER_SHIPPING_SELECT_OPEN_INDEX] = this.WhfOpenCartIndex.Value;
		this.Process.BlockErrorDisplay = true;
		var validationOk = lbNext_Click_OrderShipping_Owner(sender, e);
		validationOk &= lbNext_Click_OrderShipping_ShippingSender(sender, e);
		validationOk &= lbNext_Click_OrderShipping_Shipping(sender, e);
		validationOk &= lbNext_Click_OrderShipping_Others(sender, e);
		this.Process.BlockErrorDisplay = false;
		this.IsRefreshCartShippings = true;

		if (validationOk == false)
		{
			ReloadByPageLoad(sender, e);
			return;
		}

		this.IsRefreshCartShippings = false;
		InitializeComponentOnRebind();
		this.WrCartList.DataSource = this.CartList;
		this.WrCartList.DataBind();
		RefreshAll(e, false);
	}

	/// <summary>
	/// ギフト商品の配送先割振によりカート内の対象商品の合計数が変わっているか
	/// </summary>
	/// <param name="product">商品</param>
	/// <returns>合計数が変わっているか</returns>
	protected bool CheckChangedProductCount(CartProduct product)
	{
		var result = product.CountSingle != product.CountBeforeDivide;
		return result;
	}

	/// <summary>
	/// 配送方法ドロップダウンリスト選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected new void ddlShippingKbnList_OnSelectedIndexChanged(object sender, System.EventArgs e)
	{
		this.Process.ddlShippingKbnList_OnSelectedIndexChanged(sender, e);

		RefreshCartInformation();
	}

	/// <summary>
	/// カスタムバリデータの属性値を変更する（EFOオプションONのとき、カスタムバリデータを無効化する）
	/// </summary>
	protected void UpdateAttributeValueForCustomValidator()
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
	/// 代引きで複数配送先を選択していないかチェック
	/// </summary>
	/// <returns>チェックOKか</returns>
	protected bool CheckPayment()
	{
		var result = this.CartList.Items.Any(cart => cart.Payment == null);
		return result;
	}

	/// <summary>
	/// Get and check input data
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <returns>True if no error, otherwise false</returns>
	protected bool GetAndCheckInputData(object sender, EventArgs e)
	{
		// 注文配送情報入力画面次へリンククリック処理（共通）
		// 共通処理失敗時はこれ以降の処理を行わない
		if (lbNext_Click_OrderShipping_Owner(sender, e) == false) return false;

		// 注文配送情報入力画面 送り主情報セット処理
		if (lbNext_Click_OrderShipping_ShippingSender(sender, e) == false) return false;

		// 注文配送情報入力画面次へリンククリック処理（共通）
		if (lbNext_Click_OrderShipping_Shipping(sender, e) == false) return false;

		// 配送先が１つなら自動紐付け
		foreach (CartObject cart in this.CartList)
		{
			if (cart.Shippings.Count == 1)
			{
				cart.Shippings[0].ProductCounts.Clear();
				foreach (var cartProduct in cart.Items)
				{
					cart.Shippings[0].ProductCounts.Add(new CartShipping.ProductCount(cartProduct, cartProduct.Count));
				}
			}
		}

		CheckCartOwnerShippingArea();
		CheckCartData();
		if (this.ErrorMessages.Count != 0)
		{
			this.WhcErrorMessage.InnerText = this.ErrorMessages.Get();
			this.MaintainScrollPositionOnPostBack = false;
			return false;
		}

		// 注文配送情報入力画面次へリンククリック処理（共通）
		if (lbNext_Click_OrderShipping_Others(sender, e) == false) return false;

		if (DisplayShippingDateErrorMessage()) return false;

		GetGiftWrappingPaperAndBagInput();

		var invalidGiftCartErrorMessage = CheckInvalidGiftCartError();
		if (string.IsNullOrEmpty(invalidGiftCartErrorMessage) == false)
		{
			this.WlInvalidGiftCartErrorMessage.Text = WebSanitizer.HtmlEncodeChangeToBr(invalidGiftCartErrorMessage);
			this.WlInvalidGiftCartErrorMessage.Visible = true;
			return false;
		}

		// すべてのカートを再計算
		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			RecalculateProductCount(riCart);
			RefreshCartProducts(riCart);
		}

		return true;
	}

	/// <summary>
	/// 全画面リフレッシュ
	/// </summary>
	/// <param name="e"></param>
	/// <param name="isNeedRebind">Is need rebind</param>
	private void RefreshAll(EventArgs e, bool isNeedRebind = true)
	{
		// 配送先が１つなら自動紐付け
		foreach (var cart in this.CartList.Items)
		{
			if (cart.Shippings.Count == 0) continue;

			if (cart.IsGift)
			{
				cart.Shippings[0].IsSameSenderAsShipping1 = false;

				if (cart.Shippings.Count > 1)
				{
					foreach (var shipping in cart.Shippings)
					{
						foreach (var cartProduct in cart.Items)
						{
							if (shipping.ProductCounts
								.Any(productCount =>
									productCount.Product == cartProduct)) continue;

							shipping.ProductCounts.Add(
								new CartShipping.ProductCount(cartProduct, 0));
						}
					}
				}
			}
			if ((isNeedRebind == false)
				|| (cart.Shippings.Count != 1)) continue;

			cart.Shippings[0].ProductCounts.Clear();
			foreach (var cartProduct in cart.Items)
			{
				cart.Shippings[0].ProductCounts.Add(
					new CartShipping.ProductCount(cartProduct, cartProduct.Count));
			}
		}

		if (this.IsRefreshCartShippings == false)
		{
			foreach (RepeaterItem riCart in this.WrCartList.Items)
			{
				RefreshCartShippings(riCart, this.CartList.Items[riCart.ItemIndex].Shippings, e);
			}
		}

		foreach (RepeaterItem riCart in this.WrCartList.Items)
		{
			RefreshCartProducts(riCart);
		}

		RefreshCartInformation();
	}

	/// <summary>
	/// Refresh cart shippings
	/// </summary>
	/// <param name="riCart">Ripeater item cart</param>
	/// <param name="cartShippings">Cart shippings</param>
	/// <param name="e"></param>
	private void RefreshCartShippings(RepeaterItem riCart, List<CartShipping> cartShippings, EventArgs e)
	{
		var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
		wrCartShippings.DataSource = cartShippings;
		wrCartShippings.DataBind();

		foreach (RepeaterItem riShipping in wrCartShippings.Items)
		{
			var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlShippingKbnList");
			if (wddlShippingKbnList.HasInnerControl)
			{
				wddlShippingKbnList.SelectedValue = cartShippings[riShipping.ItemIndex].ShippingAddrKbn;
				ddlShippingKbnList_OnSelectedIndexChanged(wddlShippingKbnList.InnerControl, e);
			}

			var wddlSenderCountry = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlSenderCountry");
			if (wddlSenderCountry.HasInnerControl)
			{
				wddlSenderCountry.SelectedValue = cartShippings[riShipping.ItemIndex].SenderCountryIsoCode;
				ddlSenderCountry_SelectedIndexChanged(wddlSenderCountry.InnerControl, e);
			}

			var wddlShippingCountry = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlShippingCountry");
			if (wddlShippingCountry.HasInnerControl)
			{
				wddlShippingCountry.SelectedValue = cartShippings[riShipping.ItemIndex].ShippingCountryIsoCode;
				ddlShippingCountry_SelectedIndexChanged(wddlShippingCountry.InnerControl, e);
			}
		}
	}

	/// <summary>
	/// Refresh cart products
	/// </summary>
	/// <param name="riCart">Repeater item cart</param>
	private void RefreshCartProducts(RepeaterItem riCart)
	{
		var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
		foreach (RepeaterItem riShipping in wrCartShippings.Items)
		{
			// 割り当て済み商品一覧バインド
			var productCounts = this.CartList.Items[riCart.ItemIndex].Shippings[riShipping.ItemIndex].ProductCounts;
			var wrAllocatedProducts = GetWrappedControl<WrappedRepeater>(riShipping, "rAllocatedProducts");
			var wrAllocatedProductsTop = GetWrappedControl<WrappedRepeater>(riShipping, "rAllocatedProductsTop");
			var isDisplayTop = ((this.IsSmartPhone == false)
					&& (this.CartList.Items[riCart.ItemIndex].HasFixedPurchase
						|| this.CartList.Items[riCart.ItemIndex].HasSubscriptionBox))
				|| (this.IsSmartPhone
					&& (this.CartList.Items[riCart.ItemIndex].IsGift == false));
			wrAllocatedProducts.Visible = ((productCounts.Count > 0)
				&& (isDisplayTop == false));
			wrAllocatedProducts.DataSource = productCounts;
			wrAllocatedProductsTop.Visible = ((productCounts.Count > 0)
				&& (wrAllocatedProducts.Visible == false));
			wrAllocatedProductsTop.DataSource = productCounts;
			wrAllocatedProducts.DataBind();
			wrAllocatedProductsTop.DataBind();
		}
	}

	/// <summary>
	/// Refresh cart information
	/// </summary>
	private void RefreshCartInformation()
	{
		foreach (RepeaterItem riCartItem in this.WrCartList.Items)
		{
			var currentCart = this.CartList.Items[riCartItem.ItemIndex];
			var cartTemp = currentCart.Copy();

			var wrCartProduct = GetWrappedControl<WrappedRepeater>(riCartItem, "rCartProduct");
			var cartProductForBinding = new List<CartProduct>();
			foreach (var cartProduct in currentCart.Items)
			{
				foreach (var shipping in currentCart.Shippings)
				{
					var hasQuantityProductsTemp = shipping.ProductCounts
						.Where(productCount => productCount.Count != 0)
						.ToList();
					if (hasQuantityProductsTemp.Any() == false) continue;

					foreach (var hasQuantityProductTemp in hasQuantityProductsTemp)
					{
						var findProduct = cartProductForBinding
							.FirstOrDefault(product =>
								((product.ShopId == hasQuantityProductTemp.Product.ShopId)
								&& (product.ProductId == hasQuantityProductTemp.Product.ProductId)
								&& (product.VariationId == hasQuantityProductTemp.Product.VariationId)));

						if (findProduct != null) continue;
						cartProductForBinding.Add(hasQuantityProductTemp.Product);
					}
				}
			}
			wrCartProduct.DataSource = cartProductForBinding;
			wrCartProduct.DataBind();

			for (var cartProductIndex = currentCart.Items.Count - 1; cartProductIndex > -1; cartProductIndex--)
			{
				var currentProduct = currentCart.Items[cartProductIndex];
				var findProduct = cartProductForBinding
					.FirstOrDefault(product =>
						IsSameProduct(product, currentProduct));

				if (findProduct == null)
				{
					cartTemp.RemoveProduct(
						currentProduct.ShopId,
						currentProduct.ProductId,
						currentProduct.VariationId,
						currentProduct.AddCartKbn,
						currentProduct.ProductSaleId,
						currentProduct.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues());
				}
			}

			var wrCartDetail = GetWrappedControl<WrappedRepeater>(riCartItem, "rCartDetail");
			foreach (var shipping in cartTemp.Shippings)
			{
				UpdateShipping(shipping, cartTemp.Owner);
			}
			cartTemp.CalculateWithCartShipping();
			if (currentCart.Payment != null)
			{
				cartTemp.Payment = currentCart.Payment.Clone();
				cartTemp.Payment.PriceExchange = OrderCommon.GetPaymentPrice(
					cartTemp.ShopId,
					cartTemp.Payment.PaymentId,
					cartTemp.PriceSubtotal,
					cartTemp.PriceCartTotalWithoutPaymentPrice);
			}

			var cartDetailForBinding = new List<CartObject>
			{
				cartTemp
			};
			wrCartDetail.DataSource = cartDetailForBinding;
			wrCartDetail.DataBind();
		}
	}

	/// <summary>
	/// Get current display shippings
	/// </summary>
	/// <param name="riCart">Repeater item cart</param>
	/// <returns>Current display shippings</returns>
	protected List<CartShipping> GetCurrentDisplayShippings(RepeaterItem riCart)
	{
		var cartShippings = new List<CartShipping>();

		var wrCartShippings = GetWrappedControl<WrappedRepeater>(riCart, "rCartShippings");
		foreach (RepeaterItem riShipping in wrCartShippings.Items)
		{
			var wrblSenderSelector = GetWrappedControl<WrappedRadioButtonList>(
				riShipping,
				"rblSenderSelector",
				CartShipping.AddrKbn.Owner.ToString());
			var wddlShippingKbnList = GetWrappedControl<WrappedDropDownList>(
				riShipping,
				"ddlShippingKbnList",
				CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER);
			var wcbSameSenderAsShipping1 = GetWrappedControl<WrappedCheckBox>(
				riShipping,
				"cbSameSenderAsShipping1");

			var cartShipping = new CartShipping(this.CartList.Items[riCart.ItemIndex]);

			if (wcbSameSenderAsShipping1.Checked)
			{
				cartShipping.UpdateSenderAddr(cartShippings[0], true);
			}
			else
			{
				switch ((CartShipping.AddrKbn)Enum.Parse(typeof(CartShipping.AddrKbn), wrblSenderSelector.SelectedValue))
				{
					case CartShipping.AddrKbn.New:
						var wtbSenderName1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderName1");
						var wtbSenderName2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderName2");
						var wtbSenderNameKana1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderNameKana1");
						var wtbSenderNameKana2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderNameKana2");
						var wtbSenderZip = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderZip");
						var wtbSenderZip1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderZip1");
						var wtbSenderZip2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderZip2");
						var wtbSenderZipGlobal = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderZipGlobal");
						var wddlShippingAddr1 = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlSenderAddr1");
						var wtbSenderAddr2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderAddr2");
						var wtbSenderAddr3 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderAddr3");
						var wtbSenderAddr4 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderAddr4");
						var wtbSenderAddr5 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderAddr5");
						var wddlSenderCountry = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlSenderCountry");
						var wtbSenderCompanyName = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderCompanyName");
						var wtbSenderCompanyPostName = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderCompanyPostName");
						var wtbSenderTel1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderTel1");
						var wtbSenderTel1_1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderTel1_1");
						var wtbSenderTel1_2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderTel1_2");
						var wtbSenderTel1_3 = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderTel1_3");
						var wtbSenderTel1Global = GetWrappedControl<WrappedTextBox>(riShipping, "tbSenderTel1Global");

						var senderCountryIsoCode = string.Empty;
						var senderCountryName = string.Empty;
						var senderZip = wtbSenderZip.Text.Trim();
						var senderZip1 = wtbSenderZip1.HasInnerControl
							? wtbSenderZip1.Text.Trim()
							: wtbSenderZip.Text.Trim();
						var senderZip2 = wtbSenderZip2.Text.Trim();
						var senderTel = wtbSenderTel1.Text.Trim();
						var senderTel1 = wtbSenderTel1_1.HasInnerControl
							? wtbSenderTel1_1.Text.Trim()
							: wtbSenderTel1.Text.Trim();
						var senderTel2 = wtbSenderTel1_2.Text.Trim();
						var senderTel3 = wtbSenderTel1_3.Text.Trim();

						// Set value for zip
						if (wtbSenderZip2.HasInnerControl == false)
						{
							var zip = new ZipCode(StringUtility.ToHankaku(senderZip1));
							senderZip = zip.Zip;
							senderZip1 = zip.Zip1;
							senderZip2 = zip.Zip2;
						}

						// Set value for tel
						if (wtbSenderTel1_2.HasInnerControl == false)
						{
							var tel = new Tel(StringUtility.ToHankaku(senderTel1));
							senderTel = tel.TelNo;
							senderTel1 = tel.Tel1;
							senderTel2 = tel.Tel2;
							senderTel3 = tel.Tel3;
						}

						if (Constants.GLOBAL_OPTION_ENABLE)
						{
							senderCountryName = wddlSenderCountry.SelectedText;
							senderCountryIsoCode = wddlSenderCountry.SelectedValue;

							if (IsCountryJp(senderCountryIsoCode) == false)
							{
								senderZip = wtbSenderZipGlobal.Text.Trim();
								senderZip1 = string.Empty;
								senderZip2 = string.Empty;
								senderTel = wtbSenderTel1Global.Text.Trim();
								senderTel1 = string.Empty;
								senderTel2 = string.Empty;
								senderTel3 = string.Empty;
							}
						}

						cartShipping.UpdateSenderAddr(
							name1: wtbSenderName1.Text.Trim(),
							name2: wtbSenderName2.Text.Trim(),
							nameKana1: wtbSenderNameKana1.Text.Trim(),
							nameKana2: wtbSenderNameKana2.Text.Trim(),
							zip: senderZip,
							zip1: senderZip1,
							zip2: senderZip2,
							addr1: wddlShippingAddr1.SelectedValue,
							addr2: wtbSenderAddr2.Text.Trim(),
							addr3: wtbSenderAddr3.Text.Trim(),
							addr4: wtbSenderAddr4.Text.Trim(),
							addr5: wtbSenderAddr5.Text.Trim(),
							addrCountryIsoCode: senderCountryIsoCode,
							addrCountryName: senderCountryName,
							companyName: wtbSenderCompanyName.Text.Trim(),
							companyPostName: wtbSenderCompanyPostName.Text.Trim(),
							tel1: senderTel,
							strTel1_1: senderTel1,
							strTel1_2: senderTel2,
							strTel1_3: senderTel3,
							blIsSameSenderAsCart1: false,
							akSenderAddrKbn: CartShipping.AddrKbn.New);
						break;

					case CartShipping.AddrKbn.Owner:
						cartShipping.UpdateSenderAddr(this.CartList.Owner, false);
						break;
				}
			}

			// 配送先セット
			switch (wddlShippingKbnList.SelectedValue)
			{
				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
					var wtbUserShippingName = GetWrappedControl<WrappedTextBox>(riShipping, "tbUserShippingName");
					var wtbShippingName1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingName1");
					var wtbShippingName2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingName2");
					var wtbShippingNameKana1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingNameKana1");
					var wtbShippingNameKana2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingNameKana2");
					var wtbShippingZip = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingZip");
					var wtbShippingZip1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingZip1");
					var wtbShippingZip2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingZip2");
					var wtbShippingZipGlobal = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingZipGlobal");
					var wddlShippingCountry = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlShippingCountry");
					var wddlShippingAddr1 = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlShippingAddr1");
					var wtbShippingAddr2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingAddr2");
					var wtbShippingAddr3 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingAddr3");
					var wtbShippingAddr4 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingAddr4");
					var wddlShippingAddr5 = GetWrappedControl<WrappedDropDownList>(riShipping, "ddlShippingAddr5");
					var wtbShippingAddr5 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingAddr5");
					var wtbShippingCompanyName = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingCompanyName");
					var wtbShippingCompanyPostName = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingCompanyPostName");
					var wtbShippingTel = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingTel1");
					var wtbShippingTel1_1 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingTel1_1");
					var wtbShippingTel1_2 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingTel1_2");
					var wtbShippingTel1_3 = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingTel1_3");
					var wtbShippingTel1Global = GetWrappedControl<WrappedTextBox>(riShipping, "tbShippingTel1Global");
					var wrblSaveToUserShipping = GetWrappedControl<WrappedRadioButtonList>(riShipping, "rblSaveToUserShipping");

					var shippingCountryIsoCode = string.Empty;
					var shippingCountryName = string.Empty;
					var shippingAddr5 = string.Empty;
					var shippingZip = wtbShippingZip.Text.Trim();
					var shippingZip1 = wtbShippingZip1.HasInnerControl
						? wtbShippingZip1.Text.Trim()
						: wtbShippingZip.Text.Trim();
					var shippingZip2 = wtbShippingZip2.Text.Trim();
					var shippingTel = wtbShippingTel.Text.Trim();
					var shippingTel1 = wtbShippingTel1_1.HasInnerControl
						? wtbShippingTel1_1.Text.Trim()
						: wtbShippingTel.Text.Trim();
					var shippingTel2 = wtbShippingTel1_2.Text.Trim();
					var shippingTel3 = wtbShippingTel1_3.Text.Trim();

					// Set value for zip
					if (wtbShippingZip2.HasInnerControl == false)
					{
						var zip = new ZipCode(StringUtility.ToHankaku(shippingZip1));
						shippingZip = zip.Zip;
						shippingZip1 = zip.Zip1;
						shippingZip2 = zip.Zip2;
					}

					// Set value for tel
					if (wtbShippingTel1_2.HasInnerControl == false)
					{
						var tel = new Tel(StringUtility.ToHankaku(shippingTel1));
						shippingTel = tel.TelNo;
						shippingTel1 = tel.Tel1;
						shippingTel2 = tel.Tel2;
						shippingTel3 = tel.Tel3;
					}

					if (Constants.GLOBAL_OPTION_ENABLE)
					{
						shippingCountryName = wddlShippingCountry.SelectedText;
						shippingCountryIsoCode = wddlShippingCountry.SelectedValue;
						shippingAddr5 = IsCountryUs(shippingCountryIsoCode)
							? wddlShippingAddr5.SelectedText
							: wtbShippingAddr5.Text.Trim();

						if (IsCountryJp(shippingCountryIsoCode) == false)
						{
							shippingZip = wtbShippingZipGlobal.Text.Trim();
							shippingZip1 = string.Empty;
							shippingZip2 = string.Empty;
							shippingTel = wtbShippingTel1Global.Text.Trim();
							shippingTel1 = string.Empty;
							shippingTel2 = string.Empty;
							shippingTel3 = string.Empty;
						}
					}

					cartShipping.UpdateShippingAddr(
						wtbShippingName1.Text.Trim(),
						wtbShippingName2.Text.Trim(),
						wtbShippingNameKana1.Text.Trim(),
						wtbShippingNameKana2.Text.Trim(),
						shippingZip,
						shippingZip1,
						shippingZip2,
						wddlShippingAddr1.SelectedValue,
						wtbShippingAddr2.Text.Trim(),
						wtbShippingAddr3.Text.Trim(),
						wtbShippingAddr4.Text.Trim(),
						shippingAddr5,
						shippingCountryIsoCode,
						shippingCountryName,
						wtbShippingCompanyName.Text.Trim(),
						wtbShippingCompanyPostName.Text.Trim(),
						shippingTel,
						shippingTel1,
						shippingTel2,
						shippingTel3,
						false,
						CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW);
					cartShipping.UpdateUserShippingRegistSetting(
						wrblSaveToUserShipping.SelectedValue == "1",
						wtbUserShippingName.Text.Trim());
					break;

				case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
					cartShipping.UpdateShippingAddr(this.CartList.Owner, false);
					break;

				default:
					cartShipping.ShippingAddrKbn = wddlShippingKbnList.SelectedValue;

					// ユーザ配送先情報画面セット
					var userShipping = DomainFacade.Instance.UserShippingService
						.Get(this.LoginUserId, int.Parse(wddlShippingKbnList.SelectedValue));
					if (userShipping != null)
					{
						cartShipping.UpdateShippingAddr(
							userShipping.ShippingName1,
							userShipping.ShippingName2,
							userShipping.ShippingNameKana1,
							userShipping.ShippingNameKana2,
							userShipping.ShippingZip,
							userShipping.ShippingZip1,
							userShipping.ShippingZip2,
							userShipping.ShippingAddr1,
							userShipping.ShippingAddr2,
							userShipping.ShippingAddr3,
							userShipping.ShippingAddr4,
							userShipping.ShippingAddr5,
							userShipping.ShippingCountryIsoCode,
							userShipping.ShippingCountryName,
							userShipping.ShippingCompanyName,
							userShipping.ShippingCompanyPostName,
							userShipping.ShippingTel1,
							userShipping.ShippingTel1_1,
							userShipping.ShippingTel1_2,
							userShipping.ShippingTel1_3,
							false,
							wddlShippingKbnList.SelectedValue);
					}
					break;
			}

			cartShippings.Add(cartShipping);
		}

		return cartShippings;
	}

	/// <summary>
	/// 商品数再計算
	/// </summary>
	/// <param name="cartControl">カート一覧のコントロール</param>
	private void RecalculateProductCount(RepeaterItem cartControl)
	{
		var cartObject = this.CartList.Items[cartControl.ItemIndex];
		foreach (var cartProduct in cartObject.Items)
		{
			// cartProductをもつProductCountリスト取得＆数の合計計算
			var productCounts = cartObject.Shippings
				.Select(shipping =>
					shipping.ProductCounts.Find(pcount => pcount.Product == cartProduct))
				.Where(pcount => pcount != null);
			var sum = productCounts.Sum(pcount => pcount.Count);
			// DBには保存したくないのでオブジェクトのみ更新
			cartProduct.CountSingle = sum;
			cartProduct.Calculate();
		}

		cartObject.CalculateWithCartShipping();
	}

	/// <summary>
	/// Can add shipping
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>True if can add shipping, otherwise false</returns>
	protected bool CanAddShipping(CartObject cart)
	{
		if (cart.IsGift) return true;

		var result = ((cart.Items.Count != 0)
			&& (cart.HasFixedPurchase == false)
			&& (cart.IsSubscriptionBox == false)
			&& (cart.Shippings.Count == 1));
		if (result == false) return false;

		foreach (var cartProduct in cart.Items)
		{
			var products = GetProduct(
				cartProduct.ShopId,
				cartProduct.ProductId,
				cartProduct.VariationId);
			if (products.Count == 0) continue;

			var giftFlg = StringUtility.ToEmpty(products[0][Constants.FIELD_PRODUCT_GIFT_FLG]);
			if (giftFlg != Constants.FLG_PRODUCT_GIFT_FLG_INVALID) return true;
		}
		return false;
	}

	/// <summary>
	/// 商品情報をプロパティにセット
	/// </summary>
	/// <param name="product">商品情報</param>
	private void SetProduct(CartProduct product)
	{
		this.ShopId = product.ShopId;
		this.ProductId = product.ProductId;
		this.VariationId = product.VariationId;
	}

	/// <summary>
	/// Can change to normal cart
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <returns>True if can change to normal cart, otherwise false</returns>
	protected bool CanChangeToNormalCart(CartObject cart)
	{
		if (cart.HasFixedPurchase
			|| cart.IsSubscriptionBox
			|| (cart.IsGift == false)
			|| (cart.Shippings.Count > 1)) return false;

		foreach (var cartProduct in cart.Items)
		{
			var products = GetProduct(
				cartProduct.ShopId,
				cartProduct.ProductId,
				cartProduct.VariationId);
			if (products.Count == 0) continue;

			var giftFlg = StringUtility.ToEmpty(products[0][Constants.FIELD_PRODUCT_GIFT_FLG]);
			if (giftFlg != Constants.FLG_PRODUCT_GIFT_FLG_ONLY) return true;
		}
		return false;
	}

	/// <summary>
	/// Change to normal cart
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <param name="cartIndex">Cart index</param>
	/// <returns>True if has process, otherwise false</returns>
	protected bool ChangeToNormalCart(CartObject cart, int cartIndex)
	{
		if (CanChangeToNormalCart(cart) == false) return false;

		var currentCartShippingkbn = cart.Shippings[0].ShippingAddrKbn;
		var currentMemo = cart.OrderMemos;
		var cartTemp = cart.Copy();
		var latestCartIdBefore = this.CartList.Items
			.Max(cartListItem => cartListItem.CartId);
		foreach (var cartProduct in cartTemp.Items)
		{
			var products = GetProduct(
				cartProduct.ShopId,
				cartProduct.ProductId,
				cartProduct.VariationId);
			if (products.Count == 0) continue;

			var giftFlg = StringUtility.ToEmpty(products[0][Constants.FIELD_PRODUCT_GIFT_FLG]);
			if (giftFlg == Constants.FLG_PRODUCT_GIFT_FLG_ONLY) continue;

			DeleteProductFromCart(cart, cartProduct, Constants.AddCartKbn.GiftOrder);
			SetProduct(cartProduct);
			AddCart(
				addCartKbn: Constants.AddCartKbn.Normal,
				cartAddProductCount: cartProduct.Count.ToString(),
				redirectAfterAddProduct: string.Empty,
				productOptionSettingList: cartProduct.ProductOptionSettingList);
		}
		var latestCartIdAfter = this.CartList.Items
			.Max(cartListItem => cartListItem.CartId);

		if (latestCartIdAfter != latestCartIdBefore)
		{
			var newCart = this.CartList.Items
				.FirstOrDefault(cartChild => cartChild.CartId == latestCartIdAfter);

			if ((cartTemp.Payment != null)
				&& (newCart.Payment == null))
			{
				newCart.Payment = cartTemp.Payment.Clone();
				newCart.Calculate(false, isPaymentChanged: true);
			}

			if (newCart != null)
			{
				MoveCart(newCart, cartIndex);
				newCart.Shippings[0].WrappingPaperType = string.Empty;
				newCart.Shippings[0].WrappingPaperName = string.Empty;
				newCart.Shippings[0].WrappingBagType = string.Empty;
				newCart.Shippings[0].ShippingAddrKbn = currentCartShippingkbn;
				UpdateShipping(newCart.Shippings[0], cartTemp.Owner);
				newCart.OrderMemos = currentMemo;
				newCart.Coupon = cartTemp.Coupon;
				newCart.UsePoint = cartTemp.UsePoint;
				newCart.UsePointPrice = cartTemp.UsePointPrice;
				newCart.UseCouponPrice = cartTemp.UseCouponPrice;
				newCart.SelectedCoupon = cartTemp.SelectedCoupon;
				newCart.EnteredPaymentPrice = cartTemp.EnteredPaymentPrice;
			}
		}
		return true;
	}


	/// <summary>
	/// Detach cart
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <param name="cartIndex">Cart index</param>
	/// <returns>True if has detach process, otherwise false</returns>
	protected bool DetachCart(CartObject cart, int cartIndex)
	{
		if (cart.IsGift) return false;

		var cartTemp = cart.Copy();
		var latestCartIdBefore = this.CartList.Items
			.Max(cartListItem => cartListItem.CartId);
		var newCartIds = new List<string>();
		var currentCartShipping = cartTemp.Shippings[0].ShippingAddrKbn;
		var currentCartShippingMethod = cartTemp.Shippings[0].ShippingMethod;

		foreach (var cartProduct in cartTemp.Items)
		{
			var products = GetProduct(
				cartProduct.ShopId,
				cartProduct.ProductId,
				cartProduct.VariationId);
			if (products.Count == 0) continue;

			var giftFlg = StringUtility.ToEmpty(products[0][Constants.FIELD_PRODUCT_GIFT_FLG]);
			var addCartKbn = giftFlg == Constants.FLG_PRODUCT_GIFT_FLG_INVALID
				? Constants.AddCartKbn.Normal
				: Constants.AddCartKbn.GiftOrder;

			DeleteProductFromCart(cart, cartProduct, Constants.AddCartKbn.Normal);
			SetProduct(cartProduct);
			AddCart(
				addCartKbn: addCartKbn,
				cartAddProductCount: cartProduct.Count.ToString(),
				redirectAfterAddProduct: string.Empty,
				productOptionSettingList: cartProduct.ProductOptionSettingList);

			var latestCartIdAfter = this.CartList.Items
				.Max(cartListItem => cartListItem.CartId);
			if ((latestCartIdBefore != latestCartIdAfter)
				&& (newCartIds.Contains(latestCartIdAfter) == false))
			{
				newCartIds.Add(latestCartIdAfter);
			}
		}

		foreach (var newCartId in newCartIds)
		{
			var newCart = this.CartList.Items
				.FirstOrDefault(cartListItem =>
					cartListItem.CartId == newCartId);

			newCart.UpdateCartShippingsAddr(cart.Shippings);

			if ((cartTemp.Payment != null)
				&& (newCart.Payment == null))
			{
				newCart.Payment = cartTemp.Payment.Clone();
				newCart.Calculate(false, isPaymentChanged: true);
			}

			if ((newCart != null)
				&& newCart.IsGift)
			{
				MoveCart(newCart, cartIndex);
				newCart.Shippings[0].ShippingAddrKbn = currentCartShipping;
				newCart.Shippings[0].ShippingMethod = currentCartShippingMethod;
				var cartShipping = new CartShipping(newCart)
				{
					ShippingAddrKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW,
					ShippingMethod = newCart.Shippings[0].ShippingMethod,
					DeliveryCompanyId = newCart.Shippings[0].DeliveryCompanyId,
					IsSameSenderAsShipping1 = newCart.Shippings.Count > 0,
				};
				foreach (var cartProduct in newCart.Items)
				{
					cartShipping.ProductCounts.Add(new CartShipping.ProductCount(cartProduct, 0));
				}

				newCart.Shippings[0].ProductCounts.Clear();
				foreach (var cartProduct in newCart.Items)
				{
					var countOther = GetProductCountFromOtherShipping(
						newCart.Shippings.Skip(1).ToList(),
						cartProduct);
					var count = cartProduct.Count - countOther;
					newCart.Shippings[0].ProductCounts.Add(
						new CartShipping.ProductCount(cartProduct, count));
				}
				newCart.Shippings.Add(cartShipping);
				newCart.Coupon = cartTemp.Coupon;
				newCart.UsePoint = cartTemp.UsePoint;
				newCart.UsePointPrice = cartTemp.UsePointPrice;
				newCart.UseCouponPrice = cartTemp.UseCouponPrice;
				newCart.SelectedCoupon = cartTemp.SelectedCoupon;
				newCart.EnteredPaymentPrice = cartTemp.EnteredPaymentPrice;
			}
			Session[Constants.SESSION_KEY_ORDER_SHIPPING_SELECT_OPEN_INDEX] = "1";
		}
		return true;
	}

	/// <summary>
	/// Get product count from other shipping
	/// </summary>
	/// <param name="shippings">Cart shippings</param>
	/// <param name="targetProduct">Target product</param>
	/// <returns>Product count</returns>
	private int GetProductCountFromOtherShipping(List<CartShipping> shippings, CartProduct targetProduct)
	{
		var count = 0;
		foreach (var shipping in shippings)
		{
			var sameProducts = shipping.ProductCounts.Where(item => IsSameProduct(item.Product, targetProduct));
			count += sameProducts.Sum(item => item.Count);
		}
		return count;
	}

	/// <summary>
	/// Delete product from cart
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <param name="targetProduct">Target product</param>
	/// <param name="addCartKbn">Add cart kbn</param>
	protected void DeleteProductFromCart(
		CartObject cart,
		CartProduct targetProduct,
		Constants.AddCartKbn addCartKbn)
	{
		var shopId = targetProduct.ShopId;
		var productId = targetProduct.ProductId;
		var variationId = targetProduct.VariationId;
		var productSaleId = targetProduct.ProductSaleId;
		var productOptionValue = targetProduct.ProductOptionSettingList
			.GetDisplayProductOptionSettingSelectValues();

		// 対象カート取得
		var targetCart = this.CartList.GetCart(cart.CartId);

		// 削除対象商品取得
		if (targetCart == null) return;

		var targetCartProduct = targetCart.GetProduct(
			shopId: shopId,
			productId: productId,
			variationId: variationId,
			isSetItem: false,
			isFixedPurchase: false,
			productSaleId: productSaleId,
			productOptionValue: productOptionValue,
			productBundleId: string.Empty,
			subscriptionBoxCourseId: targetProduct.SubscriptionBoxCourseId);
		if (targetCartProduct == null) return;

		this.CartList.DeleteProduct(
			cart.CartId,
			shopId,
			productId,
			variationId,
			addCartKbn,
			productSaleId,
			productOptionValue);

		if (string.IsNullOrEmpty(targetCartProduct.NoveltyId)) return;

		this.CartList.NoveltyIdsDelete = this.CartList.NoveltyIdsDelete ?? new List<string>();
		this.CartList.NoveltyIdsDelete.Add(targetCartProduct.NoveltyId);
	}

	/// <summary>
	/// Initialize component on rebind
	/// </summary>
	protected void InitializeComponentOnRebind()
	{
		InitComponents();
		CreateFixedPurchaseSettings();
	}

	/// <summary>
	/// Get cart shipping
	/// </summary>
	/// <param name="cartShipping">Cart shipping</param>
	/// <returns>Cart shipping</returns>
	protected CartShipping GetCartShipping(object cartShipping)
	{
		return (CartShipping)cartShipping;
	}

	/// <summary>
	/// Get cart object
	/// </summary>
	/// <param name="cartObject">Cart object</param>
	/// <returns>Cart object</returns>
	protected CartObject GetCartObject(object cartObject)
	{
		return (CartObject)cartObject;
	}

	/// <summary>
	/// Get product count
	/// </summary>
	/// <param name="productCount">Product count</param>
	/// <returns>Product count</returns>
	protected CartShipping.ProductCount GetProductCount(object productCount)
	{
		return (CartShipping.ProductCount)productCount;
	}

	/// <summary>
	/// Get cart product
	/// </summary>
	/// <param name="cartProduct">Cart product</param>
	/// <returns>Cart product</returns>
	protected CartProduct GetCartProduct(object cartProduct)
	{
		return (CartProduct)cartProduct;
	}

	/// <summary>
	/// Can change quantity
	/// </summary>
	/// <param name="productCount">Product count</param>
	/// <param name="isPlusButton">Is plus button</param>
	/// <returns>True if can change quantity, otherwise false</returns>
	protected bool CanChangeQuantity(
		object productCount,
		bool isPlusButton)
	{
		var checkProductCount = GetProductCount(productCount);
		if ((isPlusButton == false)
			&& (checkProductCount.Count < 0)) return false;

		return true;
	}

	/// <summary>
	/// Is same product
	/// </summary>
	/// <param name="firstCartProduct">First cart product</param>
	/// <param name="secondCartProduct">Second cart product</param>
	/// <returns>True if same cart product, otherwise false</returns>
	protected bool IsSameProduct(
		CartProduct firstCartProduct,
		CartProduct secondCartProduct)
	{
		var result = (firstCartProduct.ProductId == secondCartProduct.ProductId)
			&& (firstCartProduct.VariationId == secondCartProduct.VariationId)
			&& (firstCartProduct.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues()
				== secondCartProduct.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues());
		return result;
	}

	/// <summary>
	/// Is display gift wrapping paper and bag
	/// </summary>
	/// <param name="cartTarget">Cart target</param>
	/// <param name="cartIndex">Cart index</param>
	/// <returns>True if display gift wrapping paper and bag, otherwise false</returns>
	protected bool IsDisplayGiftWrappingPaperAndBag(object cartTarget, int cartIndex)
	{
		var isNoshiWrappingValid = (GetWrappingPaperFlgValid(cartIndex)
			|| GetWrappingBagFlgValid(cartIndex));
		var cart = this.Process.FindCart(cartTarget);
		if ((isNoshiWrappingValid == false)
			|| (cart.IsGift == false)
			|| cart.HasFixedPurchase
			|| cart.IsSubscriptionBox) return false;

		if (cart.Shippings.Count > 1) return true;
		foreach (var cartProduct in cart.Items)
		{
			var products = GetProduct(
				cartProduct.ShopId,
				cartProduct.ProductId,
				cartProduct.VariationId);
			if (products.Count == 0) continue;

			var giftFlg = StringUtility.ToEmpty(products[0][Constants.FIELD_PRODUCT_GIFT_FLG]);
			if (giftFlg == Constants.FLG_PRODUCT_GIFT_FLG_ONLY) return true;
		}

		return false;
	}

	/// <summary>
	/// Update shipping
	/// </summary>
	/// <param name="shipping">Shipping</param>
	/// <param name="cartOwner">Cart owner</param>
	protected void UpdateShipping(CartShipping shipping, CartOwner cartOwner)
	{
		switch (shipping.ShippingAddrKbn)
		{
			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW:
				break;

			case CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_OWNER:
				shipping.UpdateShippingAddr(cartOwner, false);
				break;

			default:
				var userShipping = this.UserShippingAddr.FirstOrDefault(userShippingAddr =>
					userShippingAddr.ShippingNo.ToString().Equals(shipping.ShippingAddrKbn));
				shipping.UpdateShippingAddr(
					userShipping.ShippingName1,
					userShipping.ShippingName2,
					userShipping.ShippingNameKana1,
					userShipping.ShippingNameKana2,
					userShipping.ShippingZip,
					userShipping.ShippingZip1,
					userShipping.ShippingZip2,
					userShipping.ShippingAddr1,
					userShipping.ShippingAddr2,
					userShipping.ShippingAddr3,
					userShipping.ShippingAddr4,
					userShipping.ShippingAddr5,
					userShipping.ShippingCountryIsoCode,
					userShipping.ShippingCountryName,
					userShipping.ShippingCompanyName,
					userShipping.ShippingCompanyPostName,
					userShipping.ShippingTel1,
					userShipping.ShippingTel1_1,
					userShipping.ShippingTel1_2,
					userShipping.ShippingTel1_3,
					shipping.IsSameSenderAsShipping1,
					shipping.ShippingAddrKbn);
				break;
		}
	}

	/// <summary>
	/// Move cart
	/// </summary>
	/// <param name="cart">Cart</param>
	/// <param name="newIndex">New index</param>
	public void MoveCart(CartObject cart, int newIndex)
	{
		if ((cart == null)
			|| (newIndex < 0)
			|| (this.CartList.Items.Count <= 1)) return;

		var oldIndex = this.CartList.Items.IndexOf(cart);
		if (oldIndex < 0) return;

		this.CartList.Items.RemoveAt(oldIndex);
		if (newIndex > oldIndex)
		{
			newIndex--;
		}

		this.CartList.Items.Insert(newIndex, cart);
	}

	/// <summary>
	/// Is display cart detail
	/// </summary>
	/// <param name="cartObject">Cart object</param>
	/// <returns>True if can display cart detail, otherwise false</returns>
	protected bool IsDisplayCartDetail(object cartObject)
	{
		var cart = GetCartObject(cartObject);
		if (cart.HasFixedPurchase
			|| cart.HasSubscriptionBox) return false;

		foreach (var product in cart.Items)
		{
			var products = GetProduct(
				product.ShopId,
				product.ProductId,
				product.VariationId);

			var giftFlg = StringUtility.ToEmpty(products[0][Constants.FIELD_PRODUCT_GIFT_FLG]);
			if (giftFlg != Constants.FLG_PRODUCT_GIFT_FLG_INVALID) return true;
		}

		return false;
	}

	/// <summary>
	/// Check invalid gift cart error message
	/// </summary>
	/// <returns>Invalid gift cart error message</returns>
	protected string CheckInvalidGiftCartError()
	{
		var invalidCartMessages = new List<string>();
		for (var cartIndex = 0; cartIndex < this.CartList.Items.Count; cartIndex++)
		{
			var cart = this.CartList.Items[cartIndex];
			if ((cart.IsGift == false)
				|| (cart.Shippings.Count != 1)) continue;

			var hasOnlyGiftProduct = false;
			foreach (var productCount in cart.Shippings[0].ProductCounts)
			{
				var products = GetProduct(
					productCount.Product.ShopId,
					productCount.Product.ProductId,
					productCount.Product.VariationId);

				var giftFlg = StringUtility.ToEmpty(products[0][Constants.FIELD_PRODUCT_GIFT_FLG]);
				if (giftFlg != Constants.FLG_PRODUCT_GIFT_FLG_ONLY) continue;

				hasOnlyGiftProduct = true;
				break;
			}

			if (hasOnlyGiftProduct)
			{
				invalidCartMessages.Add(
					CommerceMessages.GetMessages(
						CommerceMessages.ERRMSG_FRONT_GIFT_CART_INVALID,
						(cartIndex + 1).ToString()));
			}
		}

		if (invalidCartMessages.Count == 0) return string.Empty;
		var errorMessage = string.Join(
			Environment.NewLine,
			invalidCartMessages);
		return errorMessage;
	}

	/// <summary>
	/// Reload page
	/// </summary>
	public void Reload()
	{
		Response.Redirect(this.RawUrl);
	}

	/// <summary>戻るイベント格納用</summary>
	protected string BackEvent
	{
		get { return "javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + this.WlbBack.UniqueID + "', '', true, '', '', false, true))"; }
	}
	/// <summary>戻るonclick</summary>
	protected string BackOnClick
	{
		get { return "return true"; }
	}
	/// <summary>次へ進むイベント格納用</summary>
	protected string NextEvent
	{
		get { return "javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + this.WlbNext.UniqueID + "', '', true, '" + this.WlbNext.ValidationGroup + "', '', false, true))"; }
	}
	/// <summary>Is post back from action</summary>
	protected bool IsPostBackFromAction { get; set; }
	/// <summary>Is refresh cart shippings</summary>
	protected bool IsRefreshCartShippings { get; set; }
}
