/*
=========================================================================================================
  Module      : 注文同梱選択画面(OrderCombineSelectList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order;
using w2.App.Common.Order.FixedPurchaseCombine;
using w2.App.Common.Order.OrderCombine;
using w2.App.Common.Web.WrappedContols;
using w2.Domain.Order;
using w2.Domain.RealShop;
using w2.Domain.UserDefaultOrderSetting;

public partial class Form_Order_OrderCombineSelectList : OrderCartPage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (this.IsPreview)
		{
			Preview.PageInvalidateAction(this.Page);
			var dummyOrder = this.CartList.Items[0].CreateNewOrder();
			rCombineParentOrder.DataSource = new[] { dummyOrder };
			rCombineParentOrder.DataBind();
			this.NextButtonText = ReplaceTag("@@DispText.combine_select_next_button.toProcedure@@");
			return;
		}

		// HTTPS通信チェック（HTTP通信の場合、ショッピングカートへ）
		CheckHttps();

		// カートチェック（カートが存在しない場合、エラーページへ）
		CheckCartExists();

		// 親注文ラジオボックス変更時イベント呼び出し
		rbgCombineParentOrder_OnCheckedChanged(sender, EventArgs.Empty);

		if (!IsPostBack)
		{
			// 画面遷移の正当性チェック
			CheckOrderUrlSession();

			this.BindCombineParentOrder();

			GotoNextPageIfCombineParentOrderCountZeroOrMultiCart();
		}
	}

	/// <summary>
	/// 注文同梱可能一覧バインド
	/// </summary>
	private void BindCombineParentOrder()
	{
		var combineParentOrders = OrderCombineUtility.GetCombinableParentOrders(
			this.ShopId,
			this.LoginUserId,
			this.CartList.Items[0].ShippingType,
			this.CartList.Items[0],
			true,
			(this.IsCombinableAmazonPay && SessionManager.OrderCombineFromAmazonPayButton));

		if (Constants.ECPAY_PAYMENT_OPTION_ENABLED
			&& combineParentOrders.Any(item => (item.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY)))
		{
			combineParentOrders = combineParentOrders
				.Where(item => (item.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT))
				.ToArray();
		}

		if (Constants.NEWEBPAY_PAYMENT_OPTION_ENABLED
			&& combineParentOrders
				.Any(item => (item.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY)))
		{
			combineParentOrders = combineParentOrders
				.Where(item => (item.ExternalPaymentType == Constants.FLG_PAYMENT_TYPE_NEWEBPAY_CREDIT))
				.ToArray();
		}

		if (OrderCommon.IsPaymentCvsTypeZeus
			&& combineParentOrders
				.Any(item => (item.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)))
		{
			combineParentOrders = combineParentOrders
				.Where(item => (item.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE))
				.ToArray();
		}

		combineParentOrders = combineParentOrders
			.Where(order => order.Items.All(item => (item.FixedPurchaseProductFlg != Constants.FLG_ORDERITEM_FIXED_PURCHASE_PRODUCT_FLG_ON)
				|| (string.IsNullOrEmpty(order.FixedPurchaseId) == false)))
			.ToArray();

		combineParentOrders = combineParentOrders
			.Where(item => (item.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET))
			.Where(item => (item.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_ATM))
			.ToArray();

		rCombineParentOrder.DataSource = combineParentOrders;
		rCombineParentOrder.DataBind();
	}

	/// <summary>
	/// 注文同梱対象が0件の場合もしくは複数カートの場合次画面に遷移
	/// </summary>
	private void GotoNextPageIfCombineParentOrderCountZeroOrMultiCart()
	{
		if ((rCombineParentOrder.Items.Count != 0) || ((this.CartList.Items.Count == 1) && (rCombineParentOrder.Items.Count == 0)) == false) return;

		this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_SHIPPING_SELECT_SHIPPING;
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = this.CartList.CartNextPage;
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + this.CartList.CartNextPage);
	}

	/// <summary>
	/// 次画面遷移ボタンクリック時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbNext_Click(object sender, EventArgs e)
	{
		var isOrderCombine = (this.ParentOrderId != string.Empty);
		if (isOrderCombine)
		{
			// 親注文情報取得
			var parentOrder = new OrderService().Get(this.ParentOrderId);

			// 子注文に親注文の別出荷フラグを設定
			this.CartList.Items[0].Shippings[0].AnotherShippingFlag = parentOrder.Shippings[0].AnotherShippingFlg;
			this.CartList.Items[0].Shippings[0].ConvenienceStoreFlg = parentOrder.Shippings[0].ShippingReceivingStoreFlg;
			this.CartList.Items[0].Shippings[0].ConvenienceStoreId = parentOrder.Shippings[0].ShippingReceivingStoreId;
			this.CartList.Items[0].Shippings[0].ShippingReceivingStoreType = parentOrder.Shippings[0].ShippingReceivingStoreType;
			var parentOrderStorePickupRealShopId = parentOrder.Shippings[0].StorePickupRealShopId;

			if (this.CartList.Items[0].Shippings[0].ConvenienceStoreFlg
				== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON)
			{
				if (OrderCommon.CheckValidWeightAndPriceForConvenienceStore(
					this.CartList.Items[0],
					this.CartList.Items[0].Shippings[0].ShippingReceivingStoreType))
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] =
						WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SHIPPING_CONVENIENCE_STORE)
							.Replace("@@ 1 @@", CurrencyManager.ToPrice(Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITPRICE))
							.Replace("@@ 2 @@", Constants.RECEIVINGSTORE_TWPELICAN_CVSLIMITKG[0].ToString());

					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
				}
			}

			// 同梱前に注文メモを作成しておく
			CreateOrderMemo();

			// 注文同梱
			CartObject combinedCart;
			var errorMessage = OrderCombineUtility.CreateCombinedCart(parentOrder, this.CartList.Items[0], out combinedCart);

			// 同コースの定額頒布会同士での注文同梱の場合、同梱後カートに対してコースの数量・商品種類数・金額チェックを行う
			if (combinedCart.CheckFixedAmountForCombineWithSameCourse() == false)
			{
				this.ErrorMessageForFixedAmountCourse = WebMessages.GetMessages(
						CommerceMessages.ERRMSG_ORDERCOMBINE_SUBSCRIPTION_BOX_FIXED_AMOUNT_CHECK_INVALID)
					.Replace("@@ 1 @@", combinedCart.SubscriptionBoxErrorMsg);
				return;
			}

			if (errorMessage != "")
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			}

			// 注文メモを親注文のものだけにする
			if ((combinedCart.OrderMemos != null) && combinedCart.OrderMemos.Any())
			{
				var orderMemoList = combinedCart.OrderMemos.Except(this.CartList.Items[0].OrderMemos).ToList();

				combinedCart.OrderMemos.Clear();
				foreach (var orderMemo in orderMemoList)
				{
					combinedCart.OrderMemos.Add(orderMemo);
				}
			}

			// Check owner address and shipping address
			if (CheckOwnerAddressAndShippingAddress(combinedCart) == false) return;

			var combinedCartList = new CartObjectList(combinedCart.CartUserId, combinedCart.OrderKbn, false);
			combinedCartList.AddCartVirtural(combinedCart);
			this.CartList = combinedCartList;

			this.CartList.Items[0].Shippings[0].RealShopId = string.Empty;
			this.CartList.Items[0].Shippings[0].RealShopName = string.Empty;
			this.CartList.Items[0].Shippings[0].RealShopOpenningHours = string.Empty;
			if (string.IsNullOrEmpty(parentOrderStorePickupRealShopId) == false)
			{
				var realShopShipping = new RealShopService().Get(parentOrderStorePickupRealShopId);
				if (realShopShipping != null)
				{
					this.CartList.Items[0].Shippings[0].RealShopId = parentOrderStorePickupRealShopId;
					this.CartList.Items[0].Shippings[0].RealShopName = realShopShipping.Name;
					this.CartList.Items[0].Shippings[0].RealShopOpenningHours = realShopShipping.OpeningHours;
				}
			}

			SessionManager.OrderCombineCartList = this.CartList;

			// 子注文が頒布会 or 親注文が頒布会かつ子注文が頒布会ではない定期 の場合は配送先入力画面に遷移させる
			var needInputShippingWhenCombined = combinedCart.IsShouldRegistSubscriptionForCombine
				|| (parentOrder.HasSubscriptionBoxItem
					&& combinedCart.HasFixedPurchase
					&& (combinedCart.IsOrderCombinedWithSameSubscriptionBoxCourse() == false));
			// Check store pickup order combine
			if (Constants.STORE_PICKUP_OPTION_ENABLED
				&& (string.IsNullOrEmpty(parentOrder.StorePickupStatus) == false)
				&& this.CartList.Items.Any(item => item.Items.Any(cp => cp.StorePickUpFlg == Constants.FLG_PRODUCT_STOREPICKUP_FLG_INVALID)))
			{
				this.CartList.CartNextPage = Constants.PAGE_FRONT_ORDER_SHIPPING;
				Session[Constants.SESSION_KEY_STORE_PICKUP_ORDER_COMBINE] = true;
			}
			else
			{
				this.CartList.CartNextPage = OrderCombineSelectInput.GetNextPage(
					Request.UrlReferrer,
					isCombine: true,
					needInputShippingWhenCombined: needInputShippingWhenCombined,
					parentPaymentKbn: parentOrder.OrderPaymentKbn);
			}
		}
		// 注文同梱しない場合
		else
		{
			if ((this.IsCombinableAmazonPay == false) && Constants.TWOCLICK_OPTION_ENABLE)
			{
				var userDefaultOrderSetting = new UserDefaultOrderSettingService().Get(this.LoginUserId);
				if (userDefaultOrderSetting != null)
				{
					// 一度購入後セッションが続いていると、デフォルト注文方法が取得できなくなるためnullにしておく
					// 決済方法がAmazonPayの場合は、配送先情報を上書きしてしまうのを防ぐため除く
					if ((this.Process.SelectedShippingMethod != null)
						&& (this.Process.IsPaymentIdAmazonPay == false))
					{
						this.Process.SelectedShippingMethod = null;
					}

					// 画面遷移先とデフォルト注文方法を設定
					new UserDefaultOrderManager(this.CartList, userDefaultOrderSetting, this.IsAddRecmendItem)
						.SetNextPageAndDefaultOrderSetting((this.Process.SelectedShippingMethod != null));

					if (this.CartList.HasFixedPurchase == false)
					{
						Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = this.CartList.CartNextPage;
						Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + this.CartList.CartNextPage);
						return;
					}
				}
			}
			this.CartList.CartNextPage = OrderCombineSelectInput.GetNextPage(
				Request.UrlReferrer,
				isCombine: false,
				needInputShippingWhenCombined: false);
		}

		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = this.CartList.CartNextPage;
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + this.CartList.CartNextPage);
	}

	/// <summary>
	/// 親注文ラジオボックス変更時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rbgCombineParentOrder_OnCheckedChanged(object sender, EventArgs e)
	{
		var wrParentOrder = GetWrappedControl<WrappedRepeater>("rCombineParentOrder");
		this.ParentOrderId = string.Empty;
		this.ParentPaymentId = string.Empty;
		this.ErrorMessageForSelectedParentOrder = new KeyValuePair<string, string>();

		// 選択注文同梱 親注文ID取得
		foreach (RepeaterItem riParentOrderItem in wrParentOrder.Items)
		{
			var wrbgCombineParentOrder = GetWrappedControl<WrappedRadioButtonGroup>(riParentOrderItem, "rbgCombineParentOrder");
			if ((wrbgCombineParentOrder.InnerControl != null) && wrbgCombineParentOrder.Checked)
			{
				var whfParentOrderId = GetWrappedControl<WrappedHiddenField>(riParentOrderItem, "hfParentOrderId");
				var whfParentPaymentId = GetWrappedControl<WrappedHiddenField>(riParentOrderItem, "hfParentPaymentId");
				this.ParentOrderId = whfParentOrderId.Value;
				this.ParentPaymentId = whfParentPaymentId.Value;

				// 同梱前カート、選択された親注文が共に頒布会定額コースの場合、同梱後のカートに対して頒布会コースの制限チェックを行う
				if (this.CartList.Items[0].IsSubscriptionBoxFixedAmount)
				{
					var parentOrder = new OrderService().Get(whfParentOrderId.Value);
					if (parentOrder.HasSubscriptionBoxFixedAmountItem == false) break;

					CartObject combinedCart;
					OrderCombineUtility.CreateCombinedCart(parentOrder, this.CartList.Items[0], out combinedCart);

					// 同コースでの同梱でなければチェックは行わない
					var isOrderCombinedWithSameSubscriptionBox = combinedCart.IsOrderCombinedWithSameSubscriptionBoxCourse();
					if ((isOrderCombinedWithSameSubscriptionBox == false)
						|| combinedCart.CheckSubscriptionBoxFixedAmount()) break;

					this.ErrorMessageForSelectedParentOrder = new KeyValuePair<string, string>(
						whfParentOrderId.Value,
						WebMessages.GetMessages(
								WebMessages.ERRMSG_FRONT_ORDERCOMBINE_SELECTED_PARENT_ORDER_CANNOT_COMBINE)
							.Replace("@@ 1 @@", combinedCart.SubscriptionBoxErrorMsg));
				}

				break;
			}
		}

		var isOrderCombine = (this.ParentOrderId != string.Empty);
		if (isOrderCombine)
		{
			this.NextButtonText =
				(this.ParentPaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				&& (this.ParentPaymentId != Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
					? ReplaceTag("@@DispText.combine_select_next_button.toConfirm@@")
					: ReplaceTag("@@DispText.combine_select_next_button.toAmazonInput@@");
		}
		else
		{
			this.NextButtonText = ((this.IsAmazonLoggedIn == false)
				|| (Constants.AMAZON_PAYMENT_OPTION_ENABLED == false)
				|| (SessionManager.OrderCombineFromAmazonPayButton == false))
					? ReplaceTag("@@DispText.combine_select_next_button.toProcedure@@")
					: ReplaceTag("@@DispText.combine_select_next_button.toAmazonInput@@");
		}

		// 選択された同梱親注文にエラーメッセージを表示するため再バインド
		BindCombineParentOrder();
	}

	/// <summary>
	/// 戻るボタンクリック時
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbBack_Click(object sender, EventArgs e)
	{
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_CART_LIST;
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST);
	}

	/// <summary>
	/// Check Owner Address And Shipping Address
	/// </summary>
	/// <param name="combinedCart">Combined Cart</param>
	/// <returns>Is Vaild Address</returns>
	private bool CheckOwnerAddressAndShippingAddress(CartObject combinedCart)
	{
		var result = true;
		var errorMessage = string.Empty;
		switch (combinedCart.Payment.PaymentId)
		{
			case Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE:
				errorMessage =
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_OWNER_ERROR_AFTEE);
				result = IsCountryTw(this.LoginUser.AddrCountryIsoCode)
					&& combinedCart.Shippings[0].IsShippingAddrTw;
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_ATONE:
				errorMessage =
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHECK_OWNER_ERROR_ATONE);
				result = IsCountryJp(this.LoginUser.AddrCountryIsoCode)
					&& combinedCart.Shippings[0].IsShippingAddrJp;
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY:
				errorMessage =
					WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_COUNTRY_FOR_PAYMENT_ECPAY);
				result = (IsCountryTw(this.LoginUser.AddrCountryIsoCode)
					&& combinedCart.Shippings[0].IsShippingAddrTw);
				break;

			case Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY:
				errorMessage =
					WebMessages.GetMessages(WebMessages.ERRMSG_CHECK_COUNTRY_FOR_PAYMENT_NEWEBPAY);
				result = (IsCountryTw(this.LoginUser.AddrCountryIsoCode)
					&& combinedCart.Shippings[0].IsShippingAddrTw);
				break;
		}

		if (result == false)
		{
			var wlbErrorOwnerAddress = GetWrappedControl<WrappedLabel>("lbErrorOwnerAddress");
			wlbErrorOwnerAddress.Visible = true;
			wlbErrorOwnerAddress.Text = errorMessage;
		}
		return result;
	}

	/// <summary>
	/// 定期購入設定文言取得
	/// </summary>
	/// <param name="fixedPurchaseId">定期購入ID</param>
	/// <returns>定期購入設定文言</returns>
	protected string GetFixedPurchaseSettingMessage(string fixedPurchaseId)
	{
		return FixedPurchaseCombineUtility.GetFixedPachasePatternSettingMessage(fixedPurchaseId);
	}

	/// <summary>
	/// 選択された同梱親注文か
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>選択された同梱親注文であればTRUE</returns>
	protected bool IsSelectedParentOrder(string orderId)
	{
		return this.ParentOrderId == orderId;
	}

	/// <summary>
	/// 選択された同梱親注文にエラーがあるか
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>エラーがあればTRUE</returns>
	protected bool IsSelectedParentOrderWithError(string orderId)
	{
		return this.ErrorMessageForSelectedParentOrder.Key == orderId;
	}

	#region プロパティ
	/// <summary>次へ進むイベント格納用</summary>
	protected string NextEvent
	{
		get { return "javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + lbNext.UniqueID + "', '', true, '" + lbNext.ValidationGroup + "', '', false, true))"; }
	}
	/// <summary>戻るイベント格納用</summary>
	protected string BackEvent
	{
		get { return "javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + lbBack.UniqueID + "', '', true, '" + lbBack.ValidationGroup + "', '', false, true))"; }
	}
	/// <summary>注文同梱 同梱可能親注文情報</summary>
	protected OrderModel[] CombineParentOrder { get; set; }
	/// <summary>選択注文同梱 親注文ID</summary>
	protected string ParentOrderId { get; set; }
	/// <summary>選択注文同梱 親注文決済ID</summary>
	protected string ParentPaymentId { get; set; }
	/// <summary>次へ進むボタン文言</summary>
	protected string NextButtonText { get; set; }
	/// <summary>決済種別がAmazon Payの親注文と同梱できるか</summary>
	protected bool IsCombinableAmazonPay
	{
		get { return (this.IsAmazonLoggedIn && Constants.AMAZON_PAYMENT_OPTION_ENABLED); }
	}
	/// <summary>頒布会定額コース制限チェック用エラーメッセージ</summary>
	protected string ErrorMessageForFixedAmountCourse { get; private set; }
	/// <summary>選択された同梱親注文のエラーメッセージ（注文IDをキーとして保持）</summary>
	protected KeyValuePair<string, string> ErrorMessageForSelectedParentOrder { get; private set; }
	#endregion
}
