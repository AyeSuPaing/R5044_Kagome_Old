/*
=========================================================================================================
  Module      : 注文外部決済系基底ページ(OrderCartPageExternalPayment.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Order;
using w2.Common.Web;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// OrderCartPageExternalPayment の概要の説明です
/// </summary>
public class OrderCartPageExternalPayment : OrderHistoryPage
{
	/// <summary>ログイン必須判定</summary>
	public override bool NeedsLogin { get { return false; } }	// ログイン不要（OrderHistoryPageを継承しているところをオーバーライド）
	/// <summary>マイページメニュー表示判定</summary>
	public override bool DispMyPageMenu { get { return false; } }	// マイページメニュー表示なし（OrderHistoryPageを継承しているところをオーバーライド）

	/// <summary>
	/// セッション復元
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <remarks>セッションが復元できたか</remarks>
	protected bool RestoreSession(string orderId)
	{
		// セッションが切れていればDBから復元
		bool successFlag = true;
		if (this.AliveSessionParameter == false)
		{
			successFlag = SessionSecurityManager.RestoreSessionFromDatabaseForGoToOtherSite(Session, orderId, true);
		}
		else
		{
			// セッション復元必要ない場合はデータ削除のみ実行
			SessionSecurityManager.DeleteSessionFromDatabaseForGoToOtherSite(orderId);
		}
		// セッションからプロパティセット
		GetOrderPropertiesFromSession();

		return successFlag;
	}

	/// <summary>
	/// セッションから注文プロパティ情報取得
	/// </summary>
	protected void GetOrderPropertiesFromSession()
	{
		Hashtable param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
		this.SuccessOrders = (List<Hashtable>)param["order"];
		this.CancelOrders = (List<Hashtable>)param["cancel_orders"];
		this.SBPSMultiPaymentOrders = (List<KeyValuePair<string, Hashtable>>)param["order_sbps_multi"];
		this.LinePayOrders = (List<KeyValuePair<string, Hashtable>>)param["order_linepay"];
		this.ZeusCard3DSecurePaymentOrder = (List<Hashtable>)param["zeus_order_3dsecure"];
		this.RakutenCard3DSecurePaymentOrder = (List<Hashtable>)param["rakuten_order_3dsecure"];
		this.BokuPaymentOrder = (List<KeyValuePair<string, Hashtable>>)param["order_boku"];
		this.DispErrorMessages = (List<string>)param["error"];
		this.PaypayOrders = (Dictionary<string, Hashtable>)param["paypay_order"];
		this.GmoCard3DSecurePaymentOrder = (List<Hashtable>)param["gmo_order_3dsecure"];
		this.YamatoKwc3DSecureOrders = (List<Hashtable>)param[Constants.SESSION_KEY_PAYMENT_CREDIT_YAMATOKWC_ORDER_3DSECURE];
		this.GmoAtokarraOrders = (List<KeyValuePair<string, Hashtable>>)param["order_gmoatokara"];
	}

	/// <summary>
	/// セッション復元＆注文ロールバック
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>注文ロールバック</returns>
	protected Hashtable RestoreSessionAndRollbackOrder(string orderId, UpdateHistoryAction updateHistoryAction)
	{
		// セッション復元
		RestoreSession(orderId);

		// 注文が完了していたらリダイレクトする
		RedirectIfOrderCompletedForError(orderId);

		// 注文ロールバック
		var order = RollbackOrder(orderId, updateHistoryAction);
		return order;
	}

	/// <summary>
	/// 注文ロールバック
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>ロールバックした注文</returns>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	protected Hashtable RollbackOrder(string orderId, UpdateHistoryAction updateHistoryAction)
	{
		// 該当注文取得
		if ((orderId == null) || (orderId != (string)Session[Constants.SESSION_KEY_ORDER_ID_CHECK_FOR_LINK_TYPE_PAYMENT]))
		{
			orderId = Request[Constants.REQUEST_KEY_ORDER_ID];
		}

		var order = this.SBPSMultiPaymentOrders
			.Concat(this.LinePayOrders).FirstOrDefault(kvp => kvp.Key == orderId).Value;
		if (order == null) return null;

		// 該当カート取得
		this.CartList = GetTargetCartList(order);
		var cart = this.CartList.Items.Find(c => c.OrderId == orderId);

		// 生成されている注文情報からクーポン情報を格納
		if (cart.Coupon == null)
		{
			SetCouponList(cart);
		}

		// 注文ロールバック
		RollbackOrder(order, cart, updateHistoryAction);

		return order;
	}
	/// <summary>
	/// 注文ロールバック処理
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート情報</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	private void RollbackOrder(Hashtable order, CartObject cart, UpdateHistoryAction updateHistoryAction)
	{
		// ロールバック後戻る遷移で再度ロールバックしたときの対策
		if (OrderCommon.GetOrder(cart.OrderId).Count == 0) return;

		// ロールバック対象以外に注文がない注文時会員登録ユーザーを削除
		var isUserDelete = (((this.IsLoggedIn == false) || (this.RegisterUser != null))
			&& (this.SuccessOrders.Count == 0)
			&& (new OrderService().GetOrdersByUserId(cart.OrderUserId).Length == 1));

		OrderCommon.RollbackPreOrder(
			order,
			cart,
			isUserDelete,
			(int)order[Constants.FIELD_USERSHIPPING_SHIPPING_NO],
			this.IsLoggedIn,
			updateHistoryAction);

		if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE && isUserDelete && this.IsLoggedIn)
		{
			this.LoginUserId = null;
		}
	}

	/// <summary>
	/// 該当カートリスト取得（注文情報から通常カートかLPカートか判断）
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <returns>カートリスト</returns>
	protected CartObjectList GetTargetCartList(Hashtable order)
	{
		string landingCartKey = (string)order[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
		if (landingCartKey != null)
		{
			return (CartObjectList)Session[landingCartKey];
		}

		var cartList = GetCartObjectList();
		return cartList;
	}

	/// <summary>
	/// 該当クーポン情報格納
	/// </summary>
	/// <param name="cart">カートリスト</param>
	private void SetCouponList(CartObject cart)
	{
		var cartList = new CartObjectList(cart.OrderUserId, cart.OrderKbn, false);
		var orderModel = new OrderService().Get(cart.OrderId);
		var orderArrayModel = new OrderService().GetRelatedOrders(cart.OrderId);
		var coupon = new OrderService().GetCoupon(cart.OrderId);
		if (coupon == null) return;
		cartList.Items.AddRange(orderArrayModel.Select(c => CartObject.CreateCartByOrder(orderModel)));
		cart.Coupon = cartList.Items[0].Coupon;

		cart.Coupon.CouponId = coupon.CouponId;
		if (coupon.CouponNo != null) cart.Coupon.CouponNo = (int)coupon.CouponNo;
		cart.Coupon.CouponName = coupon.CouponName;
		cart.Coupon.CouponCode = coupon.CouponCode;
		cart.Coupon.CouponDiscription = coupon.CouponDiscription;
		cart.Coupon.CouponDiscriptionMobile = coupon.CouponDiscriptionMobile;
		cart.Coupon.CouponType = coupon.CouponType;
		cart.Coupon.CouponDispName = coupon.CouponDispName;
		cart.Coupon.CouponDispNameMobile = coupon.CouponDispNameMobile;
		cart.Coupon.DateCreated = coupon.DateCreated;
		cart.Coupon.DeptId = coupon.DeptId;
		if (coupon.DiscountPrice != null) cart.Coupon.DiscountPrice = (decimal)coupon.DiscountPrice;
		if (coupon.DiscountRate != null) cart.Coupon.DiscountRate = (decimal)coupon.DiscountRate;
		cart.Coupon.ExceptionalProduct = coupon.ExceptionalProduct;
		if (coupon.ExpireDay != null) cart.Coupon.ExpireDay = (int)coupon.ExpireDay;
		if (coupon.ExpireDateBgn != null) cart.Coupon.ExpireDateBgn = (DateTime)coupon.ExpireDateBgn;
		if (coupon.ExpireDateEnd != null) cart.Coupon.ExpireDateEnd = (DateTime)coupon.ExpireDateEnd;
		cart.Coupon.ProductKbn = coupon.ProductKbn;
		cart.Coupon.PublishDateBgn = coupon.PublishDateBgn;
		cart.Coupon.PublishDateEnd = coupon.PublishDateEnd;
		cart.Coupon.UseFlg = coupon.UseFlg;
		cart.Coupon.UserId = coupon.UserId;
		if (coupon.UsablePrice != null) cart.Coupon.UsablePrice = (decimal)coupon.UsablePrice;
		cart.Coupon.UseTogetherFlg = coupon.UseTogetherFlg;
		cart.Coupon.UserCouponCount = coupon.UserCouponCount;
		cart.Coupon.ValidFlg = coupon.ValidFlg;
	}

	/// <summary>
	/// 次の画面へ遷移
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="errorMessage">エラーメッセージ（エラー画面を挟む場合）</param>
	/// <param name="nextPage">URL</param>
	protected void RedirectToNextPage(Hashtable order, string errorMessage = null, string nextPage = null)
	{
		// 二重クリックなどでorderが空の場合はエラーページへ飛ばす
		if (order == null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage ?? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SBPS_PAYMENT_ERROR);
			var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
			if (string.IsNullOrEmpty(nextPage))
			{
				urlCreator.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP);
			}
			else
			{
				urlCreator.AddParam(Constants.REQUEST_KEY_BACK_URL, this.SecurePageProtocolAndHost + Constants.PATH_ROOT + nextPage);
			}
			Response.Redirect(urlCreator.CreateUrl());
		}

		// 遷移先画面決定（まだ外部決済が残っている or 成功注文があるなら 決済ページへ、それ以外は 注文確認画面へ戻る）
		if ((this.HasUnsettledExternalPayments) || (this.SuccessOrders.Count > 0))
		{
			nextPage = Constants.PAGE_FRONT_ORDER_SETTLEMENT;
			var nextPageForPaypaySbps = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_PAYPAY_SBPS]);
			if (string.IsNullOrEmpty(nextPageForPaypaySbps) == false)
			{
				nextPage = new UrlCreator(nextPageForPaypaySbps)
					.AddParam(Constants.REQUEST_KEY_ORDER_ID, StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]))
					.CreateUrl();

				// Remove session for Paypay
				Session.Remove(Constants.SESSION_KEY_NEXT_PAGE_FOR_PAYPAY_SBPS);
				Session.Remove(Constants.SESSION_KEY_CART_LIST_FOR_PAYPAY);
			}
		}
		else
		{
			nextPage = (order[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY] != null) ? Constants.PAGE_FRONT_LANDING_LANDING_CART_CONFIRM : Constants.PAGE_FRONT_ORDER_CONFIRM;
		}
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = nextPage;

		// 画面遷移
		if (errorMessage != null)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR
				+ "?" + Constants.REQUEST_KEY_BACK_URL + "=" + HttpUtility.UrlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + nextPage));
		}
		else
		{
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + nextPage);
		}
	}

	/// <summary>
	/// エラーなのにもかかわらず注文が完了していたらリダイレクトする
	/// </summary>
	/// <param name="orderId">注文ID</param>
	protected void RedirectIfOrderCompletedForError(string orderId)
	{
		// 通常はあり得ないが、決済サーバー上で戻る遷移などしたりしていると
		// 結果通知でOKとなり仮注文が注文済となっているのにもかかわらず
		// 決済サーバーの一意制約エラーでエラーとして戻ってきてしまうことがある。
		// その際は注文のロールバックを行わず、エラー画面を経てトップ画面に遷移するようにする。
		if (CheckOrderCompleted(orderId))
		{
			// 該当カート削除
			var cart = this.CartList.Items.FirstOrDefault(c => c.OrderId == orderId);
			this.CartList.DeleteCartVurtual(cart);

			// エラーページへリダイレクト
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SBPS_PAYMENT_ERROR_ORDER_COMPLETED);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR
				+ "?" + Constants.REQUEST_KEY_BACK_URL + "=" + HttpUtility.UrlEncode(this.SecurePageProtocolAndHost + Constants.PATH_ROOT));
		}
	}

	/// <summary>
	/// 注文が完了しているか確認する
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <returns>注文が完了しているか</returns>
	private bool CheckOrderCompleted(string orderId)
	{
		var orders = OrderCommon.GetOrder(orderId);
		if (orders.Count == 0) return false;

		var status = (string)orders[0][Constants.FIELD_ORDER_ORDER_STATUS];
		return (status != Constants.FLG_ORDER_ORDER_STATUS_TEMP);
	}

	/// <summary>
	/// 注文ステータス更新
	/// </summary>
	/// <param name="order">注文情報</param>
	/// <param name="cart">カート</param>
	/// <param name="trackingId">トラッキングID</param>
	/// <param name="paymentSatusCompleteExceptCard">決済ステータスを入金済にするか（カード以外）</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>更新件数</returns>
	protected static int UpdateOrderStatus(
		Hashtable order,
		CartObject cart,
		string trackingId,
		bool paymentSatusCompleteExceptCard,
		UpdateHistoryAction updateHistoryAction)
	{
		// 注文状態チェック
		if (order.Count == 0) throw new Exception("注文情報が見つかりませんでした。");
		if ((string)order[Constants.FIELD_ORDER_ORDER_STATUS] == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)
			throw new Exception("注文の有効期限が切れています。");
		if (((string)order[Constants.FIELD_ORDER_ORDER_STATUS] != Constants.FLG_ORDER_ORDER_STATUS_TEMP)
			&& ((string)order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == cart.Payment.PaymentId))
		{
			throw new Exception("注文情報のステータスが正しくありません:" + order[Constants.FIELD_ORDER_ORDER_STATUS]);
		}

		// 入金ステータス決定
		string paymentStatus = null;
		if ((string)order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
		{
			var paymentStatusCompleteFlgCard = (cart.HasDigitalContents && Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
				? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
				: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;
			paymentStatus = paymentStatusCompleteFlgCard
				? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
				: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;
		}
		else
		{
			paymentStatus = paymentSatusCompleteExceptCard
				? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
				: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;
		}

		// 注文ステータス更新（10秒以内に帰らなかったらエラーとする）
		var creditBranchNo = (order.Contains(Constants.FIELD_ORDER_CREDIT_BRANCH_NO)
				&& (string.IsNullOrEmpty(StringUtility.ToEmpty(order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO])) == false))
			? (int?)order[Constants.FIELD_ORDER_CREDIT_BRANCH_NO]
			: null;
		var updated = OrderCommon.UpdateForOrderComplete(
			(string)order[Constants.FIELD_ORDER_ORDER_ID],
			cart,
			(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
			trackingId,
			creditBranchNo,
			(string)order[Constants.FIELD_ORDER_PAYMENT_MEMO],
			paymentStatus,
			(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
			"",
			true,
			updateHistoryAction,
			Constants.CONST_RECEIVE_ORDER_NOTICE_TIMEOUT);

		return updated;
	}

	/// <summary>成功注文</summary>
	protected List<Hashtable> SuccessOrders { get; set; }
	/// <summary>キャンセル注文</summary>
	protected List<Hashtable> CancelOrders { get; set; }
	/// <summary>SBPS注文（決済待ち）</summary>
	protected List<KeyValuePair<string, Hashtable>> SBPSMultiPaymentOrders { get; set; }
	/// <summary>LinePay注文（決済待ち）</summary>
	protected List<KeyValuePair<string, Hashtable>> LinePayOrders { get; set; }
	/// <summary>ゼウス3Dセキュア注文（決済待ち）</summary>
	protected List<Hashtable> ZeusCard3DSecurePaymentOrder { get; set; }
	/// <summary>楽天3Dセキュア注文（決済待ち）</summary>
	protected List<Hashtable> RakutenCard3DSecurePaymentOrder { get; set; }
	/// <summary>GMO3Dセキュア注文（決済待ち）</summary>
	protected List<Hashtable> GmoCard3DSecurePaymentOrder { get; set; }
	/// <summary>ヤマトKWC3Dセキュア注文（決済待ち）</summary>
	protected List<Hashtable> YamatoKwc3DSecureOrders { get; set; }
	/// <summary>未決済の外部決済があるか</summary>
	protected bool HasUnsettledExternalPayments
	{
		get { return (this.SBPSMultiPaymentOrders.Count
			+ this.ZeusCard3DSecurePaymentOrder.Count
			+ this.LinePayOrders.Count
			+ this.BokuPaymentOrder.Count
			+ this.GmoAtokarraOrders.Count != 0); }
	}
	/// <summary>GMOマルチペイメント注文情報</summary>
	public Dictionary<string, Hashtable> PaypayOrders { get; set; }
	/// <summary>Boku payment order</summary>
	protected List<KeyValuePair<string, Hashtable>> BokuPaymentOrder { get; set; }
	/// <summary>GMOアトカラ注文</summary>
	protected List<KeyValuePair<string, Hashtable>> GmoAtokarraOrders { get; set; }
}
