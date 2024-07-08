/*
=========================================================================================================
  Module      : 注文完了ページ向けレコメンド表示出力コントローラ処理(BodyRecommendAtOrderComplete.ascx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Reauth;
using w2.App.Common.Web.WrappedContols;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Recommend;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Common.Extensions;
using w2.Domain;

public partial class Form_Common_BodyRecommendAtOrderComplete : RecommendUserControl
{
	#region ラップ済みコントロール
	WrappedLinkButton WlbOrder { get { return GetWrappedControl<WrappedLinkButton>("lbOrder"); } }
	WrappedRepeater WrFixedPurchaseOrderPrice { get { return GetWrappedControl<WrappedRepeater>("rFixedPurchaseOrderPrice"); } }
	WrappedDropDownList WddlSubscriptionCourseId { get { return GetWrappedControl<WrappedDropDownList>("ddlSubscriptionCourseId", string.Empty); } }
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// レコメンド設定が無効の場合は何もしない
		if (Constants.RECOMMEND_OPTION_ENABLED == false) return;

		if (!IsPostBack)
		{
			// カート情報が渡されていないorレコメンド適用済みの場合なにもしない
			if ((this.OrderList == null)
				|| (this.OrderList.Any(order =>
					order.Cast<DataRowView>().Any(item =>
						string.IsNullOrEmpty((string)item[Constants.FIELD_ORDERITEM_RECOMMEND_ID]) == false))))
			{
				this.Visible = false;
				return;
			}

			// カート情報復元
			CreateCartList();

			// 注文同梱済みorギフト購入or注文ステータスが「注文済み」以外の注文に対してレコメンド表示しない
			if (this.OrderOld.Any(
				order => (string.IsNullOrEmpty(order.CombinedOrgOrderIds) == false)
					|| order.IsGiftOrder
					|| (order.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDERED)))
			{
				this.Visible = false;
				return;
			}

			if (this.OrderOld.Any(order => (OrderCommon.CheckPaymentYamatoKaSms(order.OrderPaymentKbn))))
			{
				this.Visible = false;
				return;
			}

			// レコメンド設定セット
			SetRecommend();
			this.Visible = (this.Recommend != null);

			// Check valid weight and price for convenience store
			if (this.Visible && Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED)
			{
				var isValidPriceAndWeight = this.CartList.Items.Any(cart =>
					cart.Shippings.Any(shipping =>
						((shipping.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_OFF)
							|| (OrderCommon.CheckValidWeightAndPriceForConvenienceStore(cart, shipping.ShippingReceivingStoreType) == false))));
				this.Visible = isValidPriceAndWeight;
			}
		}
	}

	/// <summary>
	/// 注文実行イベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbOrder_Click(object sender, EventArgs e)
	{
		// 注文ステータスが「注文済み」以外の場合は、レコメンド注文を行わずエラーページに遷移する
		var latestOrders = this.OrderOld.Select(order => new OrderService().Get(order.OrderId)).ToList();
		if (latestOrders.Any(order => (order.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDERED)))
		{
			SessionManager.MessageForErrorPage = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER);
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}

		// 頒布会必須商品追加
		var subscriptionBoxCourseId = this.WddlSubscriptionCourseId.SelectedValue;
		if (string.IsNullOrEmpty(subscriptionBoxCourseId) == false)
		{
			this.CartList.AddSubscriptionNecessaryItems(this.Recommend, subscriptionBoxCourseId);
		}

		// 配送方法の見直し
		this.CartList.Items.ForEach(
			cart => cart.Shippings.ForEach(
				shipping => shipping.UpdateShippingMethod(
					DataCacheControllerFacade.GetShopShippingCacheController().Get(cart.ShippingType))));

		this.CartList.CalculateAllCart();

		//決済金額決定
		foreach (var co in this.CartList.Items)
		{
			co.SettlementCurrency = CurrencyManager.GetSettlementCurrency(co.Payment.PaymentId);
			co.SettlementRate = CurrencyManager.GetSettlementRate(co.SettlementCurrency);
			co.SettlementAmount = CurrencyManager.GetSettlementAmount(co.PriceTotal, co.SettlementRate, co.SettlementCurrency);
		}

		// 商品同梱して注文実行
		var registeredOrder = new List<Hashtable>();
		var user = new UserService().Get(this.OrderUserId);
		var isFirstCart = true;
		var excludeOrderIds = this.OrderOld.Select(order => order.OrderId).ToArray();
		using (var productBundler = new ProductBundler(
			this.CartList.Items,
			this.OrderUserId,
			SessionManager.AdvCodeFirst,
			SessionManager.AdvCodeNow,
			excludeOrderIds,
			this.LoginUserHitTargetListIds,
			true))
		{
			var bundledCartList = productBundler.CartList.Where(cart => cart.Items.Any()).ToArray();

			// 別カートですでにAtodeneによる再与信を行っているかどうか
			var alreadyOtherCartReauthByAtodene = false;
			// 決済キャンセルを行わない受注ID
			var disablePaymentCancelOrderId = string.Empty;

			foreach (var cart in bundledCartList)
			{
				cart.DeviceInfo = this.Request[Constants.REQUEST_GMO_DEFERRED_DEVICE_INFO];
				var oldOrder = this.OrderOld.FirstOrDefault(old => (old.OrderId == cart.OrderId));
				var newOrder = (oldOrder != null)
					? cart.CreateNewOrder(oldOrder)
					: cart.CreateNewOrder();

				newOrder.SubscriptionBoxCourseId = cart.SubscriptionBoxCourseId;
				newOrder.AdvcodeFirst = user.AdvcodeFirst;

				foreach (var oi in newOrder.Items)
				{
					var cartProduct = cart.Items.FirstOrDefault(item => (oi.VariationId == item.VariationId));
					if (cartProduct != null) oi.ProductName = cartProduct.ProductJointName;
				}

				if (oldOrder == null)
				{
					cart.OrderId = newOrder.OrderId;
					cart.OrderUserId = user.UserId;

					// For case has Line pay in cart
					if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY)
					{
						cart.IsPreApprovedLinePayPayment = true;
					}

					// 古い注文決済IDを削除しておく
					newOrder.PaymentOrderId = string.Empty;

					// 元の受注からキャンセルされ、なおかつコンビニ後払いのものがあるかどうか確認
					var cvsDefOrder = this.OrderOld.Where(old => old.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
					var existOrderIdCart = bundledCartList.Where(bc => string.IsNullOrEmpty(bc.OrderId) == false);
					var isAtodeneCancelCart = cvsDefOrder.Any(old => existOrderIdCart.Any(c => (c.OrderId == old.OrderId)) == false);

					var isAtodeneReauth = OrderCommon.IsAtodeneReauthByNewOrder(
							this.OrderOld.Where(old => old.OrderId != cart.OrderId).ToList(),
							newOrder.OrderPaymentKbn);
					if (isAtodeneReauth && (alreadyOtherCartReauthByAtodene == false) && isAtodeneCancelCart)
					{
						// 元カートと作成されるカートがAtodeneの場合かつ、元のカートがなくなる場合、再与信を行う
						// キャンセルされるカートの中で最も決済取引金額が高いカートを取得
						var cancelAtodeneOrders = this.OrderOld.Where(old =>
							(old.OrderId != cart.OrderId) && (old.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF));
						var maxAmountCancelOrder = cancelAtodeneOrders.OrderByDescending(old => old.LastBilledAmount).FirstOrDefault();
						// アップセル元の商品はひとつのカートのみなので、一回だけでも再与信したら後は無視
						alreadyOtherCartReauthByAtodene = true;

						ExecuteOrderWithReauth(maxAmountCancelOrder, newOrder, cart, user, isFirstCart, out disablePaymentCancelOrderId);
					}
					else
					{
						ExecuteOrder(newOrder, cart, user, isFirstCart);
					}
				}
				else if (newOrder.Items.Any(item => item.IsRecommendItem)
					|| (newOrder.Items.Length != oldOrder.Items.Length)
					|| newOrder.Items.Any(item =>
						(item.ItemQuantitySingle != ((oldOrder.Items.FirstOrDefault(oldItem => ((oldItem.ProductId == item.ProductId) && (oldItem.VariationId == item.VariationId))) != null)
							? oldOrder.Items.First(oldItem => ((oldItem.ProductId == item.ProductId) && (oldItem.VariationId == item.VariationId))).ItemQuantitySingle
							: 0))))
				{
					UpdateOrder(newOrder, user, cart);
				}
				isFirstCart = false;
				registeredOrder.Add(newOrder.DataSource);
			}

			// 商品がなくなったカートがあればキャンセル処理入れておく
			this.OrderOld.Where(old => bundledCartList.Any(cart => (cart.OrderId == old.OrderId)) == false).ToList()
				.ForEach(order => CancelOrder(order, user, disablePaymentCancelOrderId));
		}

		new RecommendService().UpdateBuyOrderedFlg(
			this.Recommend.ShopId,
			this.Recommend.RecommendId,
			this.OrderUserId,
			this.RecommendHistoryNo,
			Constants.FLG_LASTCHANGED_USER);

		Session[Constants.SESSION_KEY_PARAM] = new Hashtable
		{
			{ "user_id", this.OrderUserId },
			{ "order", registeredOrder },
			{ "error", new List<string>() },
			{ "alert", new List<string>() },
			{ "order_docomo", new Dictionary<string, CartObject>() }
		};
		SessionManager.NextPageForCheck = Constants.PAGE_FRONT_ORDER_COMPLETE;
		var advCodeNow = (string)this.Session[Constants.SESSION_KEY_ADVCODE_NOW];
		Session[Constants.SESSION_KEY_ADVCODE_NOW] = string.Empty;
		Response.Redirect(SessionSecurityManager.GetSecurePageProtocolAndHost()
			+ Constants.PATH_ROOT
			+ Constants.PAGE_FRONT_ORDER_COMPLETE
			+ ((string.IsNullOrEmpty(advCodeNow)
				|| (Constants.ADD_ADVC_TO_REQUEST_PARAMETER_OPTION_ENABLED == false))
					? string.Empty
					: ("?" + Constants.REQUEST_KEY_ADVCODE + "=" + advCodeNow)));
	}

	/// <summary>
	/// カート情報復元
	/// </summary>
	private void CreateCartList()
	{
		var service = new OrderService();
		this.OrderOld = this.OrderList.Select(order => service.Get((string)order[0][Constants.FIELD_ORDER_ORDER_ID])).ToArray();
		this.OrderUserId = this.LoginUserId ?? this.OrderOld.First().UserId;
		this.CartList = CartObjectList.CreateCartObjectListByOrder(
			this.OrderUserId,
			this.IsPc
				? Constants.FLG_ORDER_ORDER_KBN_PC
				: Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE,
			this.OrderOld);
	}

	/// <summary>
	/// レコメンド設定セット
	/// </summary>
	private void SetRecommend()
	{
		// カート情報セット
		if (this.CartList == null) return;
		var targetCartList = this.CartList.Items.ToArray();

		// レコメンド表示をセット
		var buttonId = this.WlbOrder.HasInnerControl
				? this.WlbOrder.InnerControl.UniqueID
				: string.Empty;

		var recommend = SetRecommend(
			targetCartList,
			this.CartList,
			this.OrderUserId,
			buttonId,
			Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_COMPLETE);

		if ((recommend != null) && recommend.IsUpsell)
		{
			var recommendSubscriptionCourses = recommend.Items
				.Where(item => item.IsSubscriptionBox)
				.SelectMany(
					item => DomainFacade.Instance.SubscriptionBoxService.GetAvailableSubscriptionBoxesByProductId(
						item.RecommendItemProductId,
						item.RecommendItemVariationId,
						item.ShopId))
				.Distinct(subscription => subscription.CourseId)
				.Where(subscription => subscription.IsValid)
				.Select(subscription => new ListItem(subscription.DisplayName, subscription.CourseId))
				.ToArray();

			this.WddlSubscriptionCourseId.Items.AddRange(recommendSubscriptionCourses);
			if (this.WddlSubscriptionCourseId.Items.FindByValue(this.CartList.SubscriptionBoxCourseId) != null)
			{
				this.WddlSubscriptionCourseId.SelectedValue = this.CartList.SubscriptionBoxCourseId;
			}
			this.WddlSubscriptionCourseId.Visible = recommendSubscriptionCourses.Any();
		}

		this.CartList.AddRecommendItem(this.Recommend);

		var recommendedCart = this.CartList.Items
			.Where(
				cart => cart.Items.Any(
					item => (item.IsFixedPurchase
						&& (string.IsNullOrEmpty(item.RecommendId) == false))))
			.ToArray();
		this.WrFixedPurchaseOrderPrice.DataSource = recommendedCart;
		this.WrFixedPurchaseOrderPrice.DataBind();

		var cartRecommend = this.CartList.Items
			.SelectMany(cart => cart.Items)
			.Where(item => item.IsRecommendItem)
			.ToArray();

		this.RecommendItemNames = cartRecommend
			.Select(item => item.ProductJointName)
			.ToArray();

		if (cartRecommend.Any() == false) return;

		if (this.Recommend.CanDisplayOrderCompletePage)
		{
			var shippingTypesOld = targetCartList
				.Select(item => item.ShippingType)
				.ToArray();
			var hasPaymentAtoneOrAftee = targetCartList
				.Any(item => (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ATONE)
					|| (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE));
			var isPaymentAtoneOrAfteeAndCrosssellAndOtherShipping = (hasPaymentAtoneOrAftee
				&& this.Recommend.IsCrosssell
				&& cartRecommend.Any(item => shippingTypesOld.Contains(item.ShippingType) == false));

			var hasPaymentEcPay = targetCartList
				.Any(item => (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY));

			var hasPaymentNewebPay = targetCartList
				.Any(item => (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY));

			var hasPaymentBokuPay = targetCartList
				.Any(item => (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CARRIERBILLING_BOKU));

			var hasPaymentPrePay = targetCartList
				.Any(item => (item.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE));

			if (hasPaymentNewebPay
				|| hasPaymentEcPay
				|| isPaymentAtoneOrAfteeAndCrosssellAndOtherShipping
				|| hasPaymentBokuPay
				|| (hasPaymentPrePay && OrderCommon.IsPaymentCvsTypeZeus))
			{
				this.Recommend = null;
			}
		}
	}

	#region 注文登録・更新
	/// <summary>
	/// 注文登録
	/// </summary>
	/// <param name="newOrder">注文情報</param>
	/// <param name="cart">カート情報</param>
	/// <param name="user">ユーザー情報</param>
	/// <param name="isFirstCart">先頭カート？</param>
	private void ExecuteOrder(OrderModel newOrder, CartObject cart, UserModel user, bool isFirstCart)
	{
		var register = new OrderRegisterFront(user.IsMember);
		var result = register.Exec(newOrder.DataSource, cart, false, isFirstCart);

		if ((result == OrderRegisterFront.ResultTypes.Fail)
			|| (result == OrderRegisterFront.ResultTypes.Skip))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER);
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}
	}

	/// <summary>
	/// 再与信を行う注文登録
	/// </summary>
	/// <param name="oldOrder">古い注文情報</param>
	/// <param name="newOrder">新しい注文情報</param>
	/// <param name="cart">カート情報</param>
	/// <param name="user">ユーザー情報</param>
	/// <param name="isFirstCart">先頭カート？</param>
	/// <param name="disableCancelOrderId">決済キャンｓルを行わない受注ID</param>
	private void ExecuteOrderWithReauth(
		OrderModel oldOrder,
		OrderModel newOrder,
		CartObject cart,
		UserModel user,
		bool isFirstCart,
		out string disableCancelOrderId)
	{
		var register = new OrderRegisterFront(user.IsMember);

		var result = register.ExecReauthAndNewOrder(
			oldOrder,
			newOrder.DataSource,
			cart,
			false,
			isFirstCart,
			false,
			true,
			out disableCancelOrderId,
			newOrder);

		if ((result == OrderRegisterFront.ResultTypes.Fail)
			|| (result == OrderRegisterFront.ResultTypes.Skip))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER);
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}
	}

	/// <summary>
	/// 注文情報更新
	/// </summary>
	/// <param name="newOrder">注文情報</param>
	/// <param name="user">ユーザー情報</param>
	/// <param name="cart">カート</param>
	/// <remarks>いずれBodyOrderConfirmRecommend.UpdateOrderと統一したい</remarks>
	private void UpdateOrder(OrderModel newOrder, UserModel user, CartObject cart)
	{
		var oldOrder = this.OrderOld.FirstOrDefault(order => order.OrderId == newOrder.OrderId);

		var updater = new OrderUpdaterFront(
			user,
			newOrder,
			oldOrder,
			user.UserMemo,
			user.UserManagementLevelId,
			Constants.FLG_LASTCHANGED_USER);
		var transactionName = updater.CreateOrderNew(true, true, UpdateHistoryAction.DoNotInsert, cart);
		if (string.IsNullOrEmpty(transactionName) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = transactionName == OrderUpdaterFront.ResultType.OutOfStock.ToString()
				? WebMessages.GetMessages(WebMessages.ERRMSG_PRODUCTSTOCK_OUT_OF_STOCK_ERROR)
				: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER); 
			
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}

		// 外部決済連携実行
		var errorMessage = string.Empty;
		var isExecuteExternalPayment = updater.ExecuteExternalPayment(
			ReauthCreatorFacade.ExecuteTypes.System,
			ReauthCreatorFacade.ExecuteTypes.System,
			UpdateHistoryAction.DoNotInsert,
			out errorMessage);

		if (isExecuteExternalPayment || (string.IsNullOrEmpty(errorMessage) == false))
		{
			OrderCommon.AppendExternalPaymentCooperationLog(
				isExecuteExternalPayment,
				newOrder.OrderId,
				isExecuteExternalPayment ? LogCreator.CreateMessage(newOrder.OrderId, "") : errorMessage,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);
		}

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER);
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}

		// 注文情報更新
		var isSuccess = updater.ExecuteUpdateOrderAndRegisterUpdateHistory(
			false,
			false,
			isExecuteExternalPayment,
			true,
			true,
			OrderHistory.ActionType.FrontRecommend,
			UpdateHistoryAction.Insert,
			cart,
			out errorMessage);
		if (isSuccess != OrderUpdaterFront.ResultType.Success)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = (isSuccess == OrderUpdaterFront.ResultType.OutOfStock)
				? WebMessages.GetMessages(WebMessages.ERRMSG_PRODUCTSTOCK_OUT_OF_STOCK_ERROR)
				: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CREATE_UPDATED_ORDER);
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}

		// 定期台帳登録・更新
		var newOrderCart = this.CartList.Items.First(i => i.OrderId == newOrder.OrderId);
		if (newOrder.IsSubscriptionBox
			|| newOrder.Items.Any(item => item.IsFixedPurchaseItem))
		{
			if (oldOrder.IsFixedPurchaseOrder)
			{
				// 更新
				updater.UpdateFixedPurchaseOrderForRecommendAtOrderComplete(newOrderCart, UpdateHistoryAction.DoNotInsert);
			}
			else
			{
				// 新規
				updater.RegisterFixedPurchaseOrder(newOrderCart, UpdateHistoryAction.DoNotInsert);
			}
		}

		if (Constants.CROSS_POINT_OPTION_ENABLED)
		{
			// Changes on the Cross Point
			var discount = (newOrder.MemberRankDiscountPrice
				+ newOrder.OrderCouponUse
				+ newOrder.SetpromotionProductDiscountAmount
				+ newOrder.FixedPurchaseDiscountPrice
				+ newOrder.FixedPurchaseMemberDiscountAmount
				- newOrder.OrderPriceRegulation);

			var priceTaxIncluded = TaxCalculationUtility.GetPriceTaxIncluded(
				newOrder.OrderPriceSubtotal,
				newOrder.OrderPriceSubtotalTax);

			var input = new PointApiInput
			{
				MemberId = oldOrder.UserId,
				OrderDate = oldOrder.OrderDate,
				PosNo = w2.App.Common.Constants.CROSS_POINT_POS_NO,
				OrderId = oldOrder.OrderId,
				BaseGrantPoint = newOrder.OrderPointAdd,
				SpecialGrantPoint = 0m,
				PriceTotalInTax = (newOrder.OrderPriceSubtotal - discount),
				PriceTotalNoTax = (priceTaxIncluded - discount),
				UsePoint = newOrder.LastOrderPointUse,
				Items = CartObject.GetOrderDetails(newOrder),
				ReasonId = CrossPointUtility.GetValue(Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID, w2.App.Common.Constants.CROSS_POINT_REASON_KBN_OPERATOR),
			};
			var result = new CrossPointPointApiService().Modify(input.GetParam(PointApiInput.RequestType.Modify));

			if (result.IsSuccess == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = MessageManager.GetMessages(
					w2.App.Common.Constants.ERRMSG_CROSSPOINT_LINKAGE_ERROR);

				var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
					.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
					.CreateUrl();
				Response.Redirect(url);
			}
		}

		// メール送信
		updater.SendOrderUpdateMail(newOrderCart, this.IsLoggedIn, this.RecommendItemNames);

		if (Constants.REPEATLINE_OPTION_ENABLED == Constants.RepeatLineOption.CooperationAndMessaging)
		{
			// 「リピートライン」送信
			updater.SendOrderUpdateLineMessage(newOrder.DataSource, newOrderCart, this.IsLoggedIn);
		}
	}

	/// <summary>
	/// 注文キャンセル処理
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="user">ユーザー情報</param>
	/// <param name="disablePaymentCancelOrderId">外部決済キャンセルを行わない受注ID</param>
	private void CancelOrder(OrderModel order, UserModel user, string disablePaymentCancelOrderId)
	{
		order.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED;
		var updater = new OrderUpdaterFront(
			user,
			order,
			Constants.FLG_LASTCHANGED_USER);
		var errorMessage = string.Empty;
		if (updater.ExecuteCanceOrder(
				true,
				OrderHistory.ActionType.FrontRecommend,
				UpdateHistoryAction.Insert,
				out errorMessage,
				disablePaymentCancelOrderId)
			&& order.IsFixedPurchaseOrder)
		{
			var fixedPurchaseService = new FixedPurchaseService();
			var fixedPurchaseContainer = fixedPurchaseService.GetContainer(order.FixedPurchaseId);
			fixedPurchaseService.CancelFixedPurchase(
				fixedPurchaseContainer,
				string.Empty,
				string.Empty,
				Constants.FLG_LASTCHANGED_USER,
				Constants.CONST_DEFAULT_DEPT_ID,
				Constants.W2MP_POINT_OPTION_ENABLED,
				UpdateHistoryAction.DoNotInsert);
			fixedPurchaseService.UpdateInvalidate(
				order.FixedPurchaseId,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.Insert);
		}

		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_RECOMMEND_CANNOT_CANCEL_UPSELL_TARGET_ORDER);
			var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(url);
		}

		// メール送信
		updater.SendOrderCancelMail(CartObject.CreateCartByOrder(order), this.IsLoggedIn, this.RecommendItemNames);
	}
	#endregion

	#region プロパティ
	/// <summary>注文情報</summary>
	/// <remarks>ページ側で設定される</remarks>
	public List<DataView> OrderList { get; set; }
	/// <summary>元注文情報</summary>
	private OrderModel[] OrderOld
	{
		get { return (OrderModel[])ViewState["OrderOld"]; }
		set { ViewState["OrderOld"] = value; }
	}
	/// <summary>カートリスト情報</summary>
	/// <remarks>注文情報から復元する</remarks>
	private CartObjectList CartList
	{
		get { return (CartObjectList)ViewState["CartList"]; }
		set { ViewState["CartList"] = value; }
	}
	/// <summary>注文者ユーザーID</summary>
	private string OrderUserId
	{
		get { return (string)ViewState["OrderUserId"]; }
		set { ViewState["OrderUserId"] = value; }
	}
	/// <summary>レコメンド商品名</summary>
	private string[] RecommendItemNames
	{
		get { return (string[])ViewState["RecommendItemNames"]; }
		set { ViewState["RecommendItemNames"] = value; }
	}
	#endregion
}
