/*
=========================================================================================================
  Module      : ベリトランス3Dセキュア認証結果取得ページ処理(GetCard3DSecureAuthResult.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using jp.veritrans.tercerog.mdk;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Veritrans;
using w2.Common.Logger;
using w2.Domain.Order;

public partial class Payment_Card3DSecureAuthVeriTrans_GetCard3DSecureAuthResult : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var blSuccess = true;
		var errMessage = string.Empty;

		this.OrderRegister = new OrderRegisterFront(this.IsLoggedIn);
		this.PaymentOrderId = WebSanitizer.HtmlEncode(Request["OrderId"]);
		var orderModel = new OrderService().GetOrderByPaymentOrderId(this.PaymentOrderId);
		var strOrderId = string.Empty;

		if (orderModel == null)
		{
			blSuccess = false;
			errMessage = "注文情報取得エラー：決済注文ID=" + this.PaymentOrderId + VeriTransConst.NEWLINE_CHARACTER;
		}
		else
		{
			strOrderId = orderModel.OrderId;
		}

		if (blSuccess)
		{
			PaymentFileLogger.WritePaymentLog(
				null,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.VeriTrans,
				PaymentFileLogger.PaymentProcessingType.VeritransSecureAuthResultNotification,
				LogCreator.CreateMessage(strOrderId, this.PaymentOrderId));

			RestoreSession(strOrderId);

			// セッションからパラメータ取得
			var strLandingCartSessionKey = (string) Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
			this.CartList = (strLandingCartSessionKey == null)
				? GetCartObjectList()
				: (CartObjectList) Session[strLandingCartSessionKey];
			var htParam = (Hashtable) Session[Constants.SESSION_KEY_PARAM];
			this.DispErrorMessages = (List<string>) htParam["error"];

			this.OrderRegister.SuccessOrders = (List<Hashtable>) htParam["order"];
			this.OrderRegister.VeriTrans3DSecurePaymentOrders = (List<Hashtable>) htParam["veritrans_order_3dsecure"];
			this.OrderRegister.GoogleAnalyticsParams = (List<Hashtable>) htParam["googleanaytics_params"];


			this.PaymentOrder = this.OrderRegister.VeriTrans3DSecurePaymentOrders.Find(o =>
				(string) o[Constants.FIELD_ORDER_ORDER_ID] == strOrderId);

			this.PaymentCart = this.CartList.Items.Find(c => c.OrderId == strOrderId);

			if ((this.PaymentOrder == null) || (this.PaymentCart == null))
			{
				blSuccess = false;
				errMessage = "セッション情報復元エラー：注文ID=" + strOrderId + VeriTransConst.NEWLINE_CHARACTER;
				AppLogger.WriteError(errMessage);
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
				Response.Redirect(
					Constants.PATH_ROOT
					+ Constants.PAGE_FRONT_ERROR
					+ "?"
					+ Constants.REQUEST_KEY_ERRORPAGE_KBN
					+ "="
					+ HttpUtility.UrlEncode(Constants.KBN_REQUEST_ERRORPAGE_GOTOP));
			}

			if ((this.PaymentCart != null) && (this.PaymentCart.PriceTotal != orderModel.OrderPriceTotal))
			{
				RedirectToNextPage(
					null,
					WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_CHANGED),
					Constants.PAGE_FRONT_CART_LIST);
			}
		}

		// 結果通知プログラムにより既に対象受注が仮注文ではなくなっている可能性を考慮
		var order = new OrderService().Get(strOrderId);
		if (order.IsTempOrder == false)
		{
			RemoveCompletedOrdersFromUncompletedOrderList();

			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
		}

		// 外部連携決済処理
		try
		{
			var mpiMstatus = WebSanitizer.HtmlEncode(Request["mpiMstatus"]);
			var cardMstatus = WebSanitizer.HtmlEncode(Request["cardMstatus"]);
			var vResultCode = WebSanitizer.HtmlEncode(Request["vResultCode"]);

			if (AuthHashUtil.CheckAuthHash(
				Request.Params,
				MerchantContext.Config.GetValue(MerchantConfig.MERCHANT_CC_ID),
				MerchantContext.Config.GetValue(MerchantConfig.MERCHANT_SECRET_KEY)) == false)
			{
				blSuccess = false;
				errMessage = "パラメータ情報が改竄されています" + VeriTransConst.NEWLINE_CHARACTER;
			}

			if (blSuccess && (mpiMstatus != VeriTransConst.RESULT_STATUS_OK))
			{
				blSuccess = false;
				errMessage = "本人認証エラー：詳細結果コード=" + vResultCode + VeriTransConst.NEWLINE_CHARACTER;
			}

			if (blSuccess && (cardMstatus != VeriTransConst.RESULT_STATUS_OK))
			{
				blSuccess = false;
				errMessage = "カード認証エラー：詳細結果コード=" + vResultCode + VeriTransConst.NEWLINE_CHARACTER;
			}

			//------------------------------------------------------------------------------
			// ２－Ｘ．失敗時処理（仮注文ロールバック）
			//		・失敗があれば処理をロールバック（＝仮注文のこす）し、例外をとばす
			//------------------------------------------------------------------------------
			if (blSuccess == false)
			{
				// 仮注文情報削除処理(ゲスト削除、ポイント戻しも行う)
				this.TransactionName = "2-X.注文ロールバック処理";

				// ロールバック対象以外に注文がない注文時会員登録ユーザーを削除
				var isUserDelete = (((this.IsLoggedIn == false) || (this.RegisterUser != null))
					&& (this.SuccessOrders.Count == 0)
					&& (new OrderService().GetOrdersByUserId(this.PaymentCart.OrderUserId).Length == 1));

				OrderCommon.RollbackPreOrder(
					this.PaymentOrder,
					this.PaymentCart,
					isUserDelete,
					(int)this.PaymentOrder[Constants.FIELD_USERSHIPPING_SHIPPING_NO],
					this.IsLoggedIn,
					UpdateHistoryAction.Insert);

				if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE && isUserDelete && this.IsLoggedIn)
				{
					this.LoginUserId = null;
				}

				// エラーメッセージ追記
				this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));

				w2.Common.Logger.AppLogger.WriteError(errMessage);
			}
		}
		catch (Exception ex)
		{
			// エラーメッセージ追記
			this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION));

			// ログ出力
			string strLogMessage = OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, this.PaymentOrder, this.PaymentCart);

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				false,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Zeus,
				PaymentFileLogger.PaymentProcessingType.CreditPaymentWithThreeDSecure,
				BaseLogger.CreateExceptionMessage(strLogMessage, ex),
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, strOrderId},
					{Constants.FIELD_ORDER_USER_ID, this.CartList.UserId}
				});
		}

		//------------------------------------------------------
		// ３．注文確定処理
		//	・ここを正常通過すれば何があっても注文完了。
		//------------------------------------------------------
		if (blSuccess)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updateOrder = new OrderService().GetAllUpdateLock(strOrderId, accessor);
				if (updateOrder.IsTempOrder == false)
				{
					blSuccess = false;
					RemoveCompletedOrdersFromUncompletedOrderList();
				}
				else
				{
					//------------------------------------------------------
					// ３－１．注文ステータス更新
					//------------------------------------------------------
					blSuccess = this.OrderRegister.UpdateForOrderComplete(
						this.PaymentOrder,
						this.PaymentCart,
						true,
						UpdateHistoryAction.DoNotInsert,
						false,
						accessor);
				}

				if (blSuccess) accessor.CommitTransaction();
			}
		}

		//------------------------------------------------------
		// ４．後処理
		//------------------------------------------------------
		if (blSuccess)
		{
			// 注文完了時処理
			var isSendMail = new OrderService().GetOrdersByUserId(this.PaymentCart.OrderUserId)
				.Where(userOrder =>
					((userOrder.OrderId != Request[Constants.REQUEST_KEY_ORDER_ID]) &&
					 (userOrder.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP)))
				.Any(userOrder => this.CartList.Items.Select(item => item.OrderId).Contains(userOrder.OrderId));
			if (isSendMail) this.PaymentOrder[Constants.ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE] = null;
			this.OrderRegister.OrderCompleteProcesses(this.PaymentOrder, this.PaymentCart, UpdateHistoryAction.DoNotInsert);

			// 注文完了後処理
			this.OrderRegister.AfterOrderCompleteProcesses(this.PaymentOrder, this.PaymentCart, UpdateHistoryAction.Insert);

			// 注文完了画面用に注文IDを格納
			this.OrderRegister.SuccessOrders.Add(this.PaymentOrder);
		}

		// 3Dセキュア決済注文リストから削除
		if (this.PaymentOrder != null)
		{
			this.OrderRegister.VeriTrans3DSecurePaymentOrders.Remove(this.PaymentOrder);
		}

		//------------------------------------------------------
		// 決済画面へ
		//------------------------------------------------------
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;

		// 画面遷移
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
	}

	/// <summary>
	/// 結果通知プログラムにより注文完了している注文を処理対象から除く
	/// </summary>
	private void RemoveCompletedOrdersFromUncompletedOrderList()
	{
		if (this.PaymentOrder != null)
		{
			this.OrderRegister.SuccessOrders.Add(this.PaymentOrder);
			this.OrderRegister.VeriTrans3DSecurePaymentOrders.Remove(this.PaymentOrder);
		}

		var successOrderIds = this.OrderRegister.SuccessOrders
			.Select(ht => StringUtility.ToEmpty(ht[Constants.FIELD_ORDER_ORDER_ID]))
			.ToArray();
		foreach (var orderId in successOrderIds)
		{
			var cartRemove = this.CartList.Items.FirstOrDefault(cart => cart.OrderId == orderId);
			if (cartRemove == null) continue;

			this.OrderRegister.AddGoogleAnalyticsParams(this.PaymentOrderId, cartRemove);
			this.CartList.DeleteCartVurtual(cartRemove);
		}
	}

	#region プロパティ
	/// <summary>決済注文ID</summary>
	private string PaymentOrderId { get; set; }
	/// <summary>注文登録情報</summary>
	private OrderRegisterFront OrderRegister { get; set; }
	/// <summary>決済処理中の注文情報</summary>
	private Hashtable PaymentOrder { get; set; }
	/// <summary>決済処理中のカート情報</summary>
	private CartObject PaymentCart { get; set; }
	#endregion
}
