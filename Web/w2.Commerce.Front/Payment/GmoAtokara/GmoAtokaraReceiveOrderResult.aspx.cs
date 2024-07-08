/*
=========================================================================================================
  Module      : GMOアトカラ 結果取得ページ処理(GmoAtokaraReceiveOrderResult.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.GMOAtokara;
using w2.App.Common.Order.Payment.GMOAtokara.Utils;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// GMOアトカラ 結果取得ページ処理
/// </summary>
public partial class Payment_GmoAtokara_GmoAtokaraReceiveOrderResult : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	protected void Page_Load(object sender, EventArgs e)
	{
		// パラメータ取得
		this.OrderId = this.Request[Constants.REQUEST_KEY_ORDER_ID];
		this.GmoTransactionId = this.Request["gmoid"];

		if (string.IsNullOrEmpty(this.OrderId)) return;

		// セッションからパラメータ取得
		GetParametersFromSession();

		if (string.IsNullOrEmpty(this.GmoTransactionId))
		{
			RollbackProcess("error");
			ToOrderConfirmPage();
		}

		// 完了処理を実行
		ExecCompleteProcess();
	}

	/// <summary>
	/// 完了処理を実行
	/// </summary>
	private void ExecCompleteProcess()
	{
		// 結果通知プログラムにより
		// 既に対象受注が仮注文ではなくなっている可能性を考慮
		var order = new OrderService().Get(this.OrderId);
		if (order.IsTempOrder == false)
		{
			RemoveCompletedOrdersFromUncompletedOrderList();

			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;
			Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
		}

		// 外部連携決済処理
		var success = ExecExternalCooperationPayment();

		// 注文確定処理
		if (success)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updateOrder = new OrderService().GetAllUpdateLock(this.OrderId, accessor);
				if (updateOrder.IsTempOrder == false)
				{
					success = false;
					RemoveCompletedOrdersFromUncompletedOrderList();
				}
				else
				{
					FileLogger.WriteDebug(
						"UpdateForOrderComplete GmoAtokaraReceiveOrderResult orderId:"
						+ this.OrderId);

					this.PaymentOrder.Value.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, this.GmoTransactionId);

					success = this.OrderRegister.UpdateForOrderComplete(
						this.PaymentOrder.Value,
						this.PaymentCart,
						true,
						UpdateHistoryAction.DoNotInsert,
						false,
						accessor);
				}

				if (success) accessor.CommitTransaction();
			}
		}

		// 後処理
		if (success) ExecAfterProcesses();

		// 決済画面へ遷移
		GoToOrderSettlement();
	}

	/// <summary>
	/// 結果通知プログラムにより注文完了している注文を処理対象から除く
	/// </summary>
	private void RemoveCompletedOrdersFromUncompletedOrderList()
	{
		if (this.PaymentOrder.Value != null)
		{
			this.OrderRegister.SuccessOrders.Add(this.PaymentOrder.Value);
			this.OrderRegister.GmoAtokaraOrders.Remove(this.PaymentOrder);
		}

		var successOrderIds = this.OrderRegister.SuccessOrders
			.Select(ht => StringUtility.ToEmpty(ht[Constants.FIELD_ORDER_ORDER_ID]))
			.ToArray();
		foreach (var orderId in successOrderIds)
		{
			var cartRemove = this.CartList.Items.FirstOrDefault(cart => cart.OrderId == orderId);
			if (cartRemove == null) continue;

			cartRemove.Payment.CreditToken = null;

			this.OrderRegister.AddGoogleAnalyticsParams(this.OrderId, cartRemove);
			this.CartList.DeleteCartVurtual(cartRemove);
			FileLogger.WriteDebug("既に注文完了しているため、注文確定処理をスキップします：" + cartRemove.OrderId);
		}
	}

	/// <summary>
	/// セッションからパラメータ取得
	/// </summary>
	private void GetParametersFromSession()
	{
		// ファイルからセッション復元
		var success = RestoreSession(this.OrderId);

		// 画面遷移の正当性チェック
		CheckOrderUrlSession();

		var landingCartSessionKey = (string)this.Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
		this.CartList = (landingCartSessionKey == null)
			? GetCartObjectList()
			: (CartObjectList)this.Session[landingCartSessionKey];
		var param = (Hashtable)this.Session[Constants.SESSION_KEY_PARAM];
		this.DispErrorMessages = (List<string>)param["error"];

		this.OrderRegister = new OrderRegisterFront(this.IsLoggedIn)
		{
			SuccessOrders = (List<Hashtable>)param["order"],
			GmoAtokaraOrders = (List<KeyValuePair<string, Hashtable>>)param["order_gmoatokara"],
		};

		// 決済処理中の注文情報、カート情報を取得
		this.PaymentOrder = this.OrderRegister.GmoAtokaraOrders.Find(order =>
			(order.Key == this.Request[Constants.REQUEST_KEY_ORDER_ID]));
		this.PaymentCart = this.CartList.Items.Find(c =>
			(c.OrderId == this.Request[Constants.REQUEST_KEY_ORDER_ID]));
		if (this.PaymentCart == null) success = false;

		if (success == false)
		{
			FileLogger.WriteError("セッション情報復元エラー：注文ID=" + this.OrderId);
			ToErrorPage();
		};
	}

	/// <summary>
	/// 注文確認ページへ遷移
	/// </summary>
	private void ToOrderConfirmPage()
	{
		if (this.OrderRegister.WaitPaymentOrderCount > 0)
		{
			GoToOrderSettlement();
		}

		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_CONFIRM;
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_CONFIRM)
			.CreateUrl();
		this.Response.Redirect(url);
	}

	/// <summary>
	/// エラーページへ遷移
	/// </summary>
	private void ToErrorPage()
	{
		if (this.OrderRegister.WaitPaymentOrderCount > 0)
		{
			GoToOrderSettlement();
		}

		// エラーページへ遷移
		this.Session[Constants.SESSION_KEY_ERROR_MSG] =
			WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
			.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
			.CreateUrl();
		this.Response.Redirect(url);
	}

	/// <summary>
	/// 外部連携決済処理実行
	/// </summary>
	/// <returns>処理結果</returns>
	private bool ExecExternalCooperationPayment()
	{
		this.TransactionName = "2-1-F.GMOアトカラ決済処理";

		try
		{
			var getAuthorizationResultApi = new PaymentGmoAtokaraGetAuthorizationResultApi();
			var result = getAuthorizationResultApi.Exec(this.GmoTransactionId);

			if (result)
			{
				switch (getAuthorizationResultApi.ResponseData.TransactionResult.AuthAuthorResult)
				{
					case PaymentGmoAtokaraConstants.AUTHORRESULT_OK:
						var externalPaymentStatus = this.PaymentCart.IsDigitalContentsOnly
							? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
							: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
						this.PaymentOrder.Value.Add(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, externalPaymentStatus);
						this.PaymentOrder.Value.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM);
						break;

					case PaymentGmoAtokaraConstants.AUTHORRESULT_NG:
						RollbackProcess("結果：NG");
						result = false;
						break;

					case PaymentGmoAtokaraConstants.AUTHORRESULT_REVIEW:
						this.PaymentOrder.Value.Add(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS, Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST);
						this.PaymentOrder.Value.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM);
						break;

					default:
						RollbackProcess("結果：" + getAuthorizationResultApi.ResponseData.TransactionResult.AuthAuthorResult);
						result = false;

						ToOrderConfirmPage();
						break;
				}
			}
			else
			{
				RollbackProcess(getAuthorizationResultApi.GetErrorMessage());
			}

			return result;
		}
		catch (Exception ex)
		{
			this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION));

			FileLogger.WriteError(
				OrderCommon.CreateOrderFailedLogMessage(
					this.TransactionName,
					this.PaymentOrder.Value,
					this.PaymentCart),
				ex);

			return false;
		}
	}

	/// <summary>
	/// ロールバック処理
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	private void RollbackProcess(string errorMessage)
	{
		// GMOアトカラ注文リストから削除
		if (this.PaymentOrder.Value != null)
		{
			this.OrderRegister.GmoAtokaraOrders.Remove(this.PaymentOrder);
		}

		// 失敗時処理（仮注文ロールバック）
		this.ErrorMessages.Add(
			CommerceMessages.GetMessages(CommerceMessages.ERRMSG_GMO_KB_PAYMENT_ALERT));
		this.ErrorMessages.Add(errorMessage);
		FileLogger.WriteError(
			OrderCommon.CreateOrderFailedLogMessage(
				this.TransactionName,
				this.PaymentOrder.Value,
				this.PaymentCart,
				string.Join("\r\n", this.ErrorMessages)));

		// エラーメッセージ追記
		this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_GMO_KB_PAYMENT_ALERT));

		// 情報ログ出力
		var logMessage = OrderCommon.CreateOrderFailedLogMessage(
			this.TransactionName,
			this.PaymentOrder.Value,
			this.PaymentCart);
		FileLogger.WriteInfo(logMessage);

		// 仮注文情報削除処理(ゲスト削除、ポイント戻しも行う)
		this.TransactionName = "2-X.注文ロールバック処理";

		// ゲスト購入かつ成功注文なしのときゲスト削除
		var lbDeleteGuest = ((this.IsLoggedIn == false)
			&& (this.OrderRegister.SuccessOrders.Count == 0)
			&& (this.OrderRegister.WaitPaymentOrderCount == 0));

		OrderCommon.RollbackPreOrder(
			this.PaymentOrder.Value,
			this.PaymentCart,
			lbDeleteGuest,
			(int)this.PaymentOrder.Value[Constants.FIELD_USERSHIPPING_SHIPPING_NO],
			this.IsLoggedIn,
			UpdateHistoryAction.Insert);
	}

	/// <summary>
	/// 後処理実行
	/// </summary>
	private void ExecAfterProcesses()
	{
		if (this.IsOrderCombined)
		{
			SuccessCombinedOrderAdditionProcess(
				this.PaymentOrder.Value,
				this.DispErrorMessages,
				this.PaymentCart.CartUserId,
				this.PaymentCart.Coupon);
		}

		// 注文完了時処理
		this.OrderRegister.OrderCompleteProcesses(
			this.PaymentOrder.Value,
			this.PaymentCart,
			UpdateHistoryAction.DoNotInsert);

		// 注文完了後処理
		this.OrderRegister.AfterOrderCompleteProcesses(
			this.PaymentOrder.Value,
			this.PaymentCart,
			UpdateHistoryAction.Insert);

		// 注文完了画面用に注文IDを格納
		this.OrderRegister.SuccessOrders.Add(this.PaymentOrder.Value);

		// GoogleAnalyticsタグ制御用注文IDをクッキーにセット
		CookieManager.SetCookie(
			(Constants.COOKIE_KEY_GOOGLEANALYTICS_ORDER_ID + (string)this.PaymentOrder.Value[Constants.FIELD_ORDER_ORDER_ID]),
			"",
			Constants.PATH_ROOT,
			DateTime.Now.AddHours(1));
	}

	/// <summary>
	/// 注文決済画面へ遷移
	/// </summary>
	private void GoToOrderSettlement()
	{
		// GMOアトカラ注文リストから削除
		if (this.PaymentOrder.Value != null)
		{
			this.OrderRegister.GmoAtokaraOrders.Remove(this.PaymentOrder);
		}

		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		this.Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;

		// 画面遷移
		this.Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
	}

	#region プロパティ
	/// <summary>注文ID</summary>
	private string OrderId { get; set; }
	/// <summary>決済取引ID</summary>
	private string GmoTransactionId { get; set; }
	/// <summary>注文登録情報</summary>
	private OrderRegisterFront OrderRegister { get; set; }
	/// <summary>決済処理中の注文情報</summary>
	private KeyValuePair<string, Hashtable> PaymentOrder { get; set; }
	/// <summary>決済処理中のカート情報</summary>
	private CartObject PaymentCart { get; set; }
	#endregion
}
