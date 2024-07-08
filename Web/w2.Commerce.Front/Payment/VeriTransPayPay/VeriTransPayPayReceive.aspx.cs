/*
=========================================================================================================
  Module      : ベリトランスPaypay決済結果受信(VeriTransPayPayReceive.aspx.cs)
  ･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using jp.veritrans.tercerog.mdk;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Paypay;
using w2.App.Common.Order.Payment.Veritrans;
using w2.Common.Extensions;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.Order.Helper;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

public partial class Payment_VeriTransPayPayReceive : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(Request.Form["exec"])) return;

		this.OrderId = this.Request.Form[Constants.REQUEST_KEY_ORDER_ID];

		PaymentFileLogger.WritePaymentLog(
			null,
			Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY,
			PaymentFileLogger.PaymentType.PayPay,
			PaymentFileLogger.PaymentProcessingType.Response,
			LogCreator.CreateMessage(this.OrderId, string.Empty));

		GetParametersFromSession();
		ExecAfterVeriTransPayPayAuth();
	}

	/// <summary>
	/// ベリトランスPayPay認証後の処理を実行
	/// </summary>
	private void ExecAfterVeriTransPayPayAuth()
	{
		// 結果通知プログラムにより
		// 既に対象受注が仮注文ではなくなっている可能性を考慮
		var order = DomainFacade.Instance.OrderService.Get(this.OrderId);

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
					success = this.OrderRegister.UpdateForOrderComplete(
						this.PaymentOrder.DataSource,
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
	/// 外部連携決済処理実行
	/// </summary>
	/// <returns>処理結果</returns>
	private bool ExecExternalCooperationPayment()
	{
		this.TransactionName = "ベリトランスPayPay決済処理";
		var isSuccess = true;
		var errorMessage = string.Empty;
		try
		{
			var resultCode = Request[VeriTransConst.VERITRANS_REQUEST_KEY_VRESULTCODE];
			var status = Request[VeriTransConst.VERITRANS_REQUEST_KEY_MSTATUS];
			if (AuthHashUtil.CheckAuthHash(
				Request.Params,
				MerchantContext.Config.GetValue(MerchantConfig.MERCHANT_CC_ID),
				MerchantContext.Config.GetValue(MerchantConfig.MERCHANT_SECRET_KEY)) == false)
			{
				isSuccess = false;
				errorMessage = "パラメータ情報が改竄されています";
			}

			if (isSuccess && (status != VeriTransConst.RESULT_STATUS_OK))
			{
				isSuccess = false;
				errorMessage = string.Format("決済失敗コード：{0}", resultCode);
			}

			// 随時決済の場合、定期IDで申込む
			if (isSuccess
				&& (this.PaymentOrderId != this.PaymentOrder.PaymentOrderId)
				&& (this.PaymentOrderId != this.PaymentOrder.FixedPurchaseId))
			{
				isSuccess = false;
				errorMessage = string.Format(
					"該当注文が見つかりませんでした： 注文ID：{0} 決済注文ID：{1}",
					this.OrderId,
					this.PaymentOrderId);
			}

			if (isSuccess)
			{
				if (this.PaymentCart.HasFixedPurchase)
				{
					var result = new PaymentVeritransPaypay().ReAuthorize(this.PaymentOrderId, this.PaymentOrder.PaymentOrderId, this.PaymentCart.PriceTotal);
					if (result.Mstatus != VeriTransConst.RESULT_STATUS_OK)
					{
						this.ErrorMessage = result.MerrMsg;
						return false;
					}
					this.PaymentOrder.CardTranId = result.PaypayOrderId;
				}
				else
				{
					this.PaymentOrder.CardTranId = this.PaypayOrderId;
				}

				// 決済連携メモ挿入
				var paymentMemo = OrderCommon.CreateOrderPaymentMemoForAuth(
					this.PaymentOrder.OrderId,
					this.PaymentOrder.PaymentOrderId,
					this.PaymentOrder.OrderPaymentKbn,
					this.PaymentOrder.CardTranId,
					this.PaymentCart.PriceTotal);
				this.PaymentOrder.PaymentMemo = OrderExternalPaymentUtility.SetExternalPaymentMemo(
					StringUtility.ToEmpty(this.PaymentOrder.PaymentMemo),
					paymentMemo);
				WritePaymentLog(isSuccess, externalPaymentCooperationLog: errorMessage);
			}
			else
			{
				// ログ格納処理
				errorMessage = OrderCommon.CreateOrderFailedLogMessage(
					this.TransactionName,
					this.PaymentOrder.DataSource,
					this.PaymentCart,
					errorMessage + GetCurrentStateStringsForLog());

				this.DispErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_AUTH_ERROR));

				// 仮注文情報削除処理(ゲスト削除、ポイント戻しも行う)
				this.TransactionName = "2-X.注文ロールバック処理";

				// ロールバック対象以外に注文がない注文時会員登録ユーザーを削除
				var isUserDelete = (((this.IsLoggedIn == false) || (this.RegisterUser != null))
					&& (this.SuccessOrders.Count == 0)
					&& (new OrderService().GetOrdersByUserId(this.PaymentCart.OrderUserId).Length == 1));
				OrderCommon.RollbackPreOrder(
					this.PaymentOrder.DataSource,
					this.PaymentCart,
					isUserDelete,
					(int)this.PaymentOrder.DataSource[Constants.FIELD_USERSHIPPING_SHIPPING_NO],
					this.IsLoggedIn,
					UpdateHistoryAction.Insert);
				if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE && isUserDelete && this.IsLoggedIn)
				{
					this.LoginUserId = null;
				}
			}

			WritePaymentLog(isSuccess, externalPaymentCooperationLog: errorMessage);
			return isSuccess;
		}
		catch (Exception ex)
		{
			this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION));

			// ログ格納処理
			var logMessage = OrderCommon.CreateOrderFailedLogMessage(
				this.TransactionName,
				this.PaymentOrder.DataSource,
				this.PaymentCart,
				GetCurrentStateStringsForLog());
			WritePaymentLog(
				isSuccess: false,
				externalPaymentCooperationLog: BaseLogger.CreateExceptionMessage(logMessage, ex));

			return false;
		}
	}

	/// <summary>
	/// 後処理実行
	/// </summary>
	private void ExecAfterProcesses()
	{
		var user = new UserService().Get(this.PaymentOrder.UserId);
		var isUser = UserService.IsUser(user.UserKbn);

		var orderRegister = new OrderRegisterFront(isUser);
		orderRegister.OrderCompleteProcesses(
			this.PaymentOrder.DataSource,
			this.PaymentCart,
			UpdateHistoryAction.DoNotInsert);
		orderRegister.AfterOrderCompleteProcesses(
			this.PaymentOrder.DataSource,
			this.PaymentCart,
			UpdateHistoryAction.Insert);

		// 注文完了画面用に注文IDを格納
		if (this.PaymentOrder != null)
		{
			this.OrderRegister.SuccessOrders.Add(this.PaymentOrder.DataSource);
		}
		this.PaymentCart.IsOrderDone = true;

		// GoogleAnalyticsタグ制御用注文IDをクッキーにセット
		CookieManager.SetCookie(
			(Constants.COOKIE_KEY_GOOGLEANALYTICS_ORDER_ID + this.OrderId),
			string.Empty,
			Constants.PATH_ROOT,
			DateTime.Now.AddHours(1));
	}

	/// <summary>
	/// 結果通知プログラムにより注文完了している注文を処理対象から除く
	/// </summary>
	private void RemoveCompletedOrdersFromUncompletedOrderList()
	{
		if (this.PaymentOrder != null)
		{
			this.OrderRegister.SuccessOrders.Add(this.PaymentOrder.DataSource);
			this.OrderRegister.PaypayOrders.Remove(this.OrderId);
		}

		var successOrderIds = this.OrderRegister.SuccessOrders
			.Select(ht => StringUtility.ToEmpty(ht[Constants.FIELD_ORDER_ORDER_ID]))
			.ToArray();
		foreach (var orderId in successOrderIds)
		{
			var cartRemove = this.CartList.Items.FirstOrDefault(cart => cart.OrderId == orderId);
			if (cartRemove == null) continue;

			this.OrderRegister.AddGoogleAnalyticsParams(this.OrderId, cartRemove);
			this.CartList.DeleteCartVurtual(cartRemove);
		}
	}

	/// <summary>
	/// セッションからパラメータ取得
	/// </summary>
	private void GetParametersFromSession()
	{
		var isSuccess = true;
		try
		{
			isSuccess = RestoreSession(this.OrderId);
			CheckOrderUrlSession();

			var landingCartSessionKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
			this.CartList = (landingCartSessionKey == null)
				? GetCartObjectList()
				: (CartObjectList)Session[landingCartSessionKey];
			var param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
			this.DispErrorMessages = (List<string>)param["error"];

			this.OrderRegister = new OrderRegisterFront(this.IsLoggedIn)
			{
				SuccessOrders = (List<Hashtable>)param["order"],
				PaypayOrders = (Dictionary<string, Hashtable>)param["paypay_order"],
				GoogleAnalyticsParams = (List<Hashtable>)param["googleanaytics_params"],
			};

			this.PaymentOrder = new OrderModel(this.OrderRegister.PaypayOrders.FirstOrDefault(order => order.Key == this.OrderId).Value);
			this.PaymentCart = this.CartList.Items.Find(item => item.OrderId == this.OrderId);
			if ((this.PaymentOrder == null) || (this.PaymentCart == null))
			{
				isSuccess = false;
			}
		}
		catch (Exception ex)
		{
			isSuccess = false;
			FileLogger.WriteError(GetCurrentStateStringsForLog(), ex);
		}

		if (isSuccess == false)
		{
			FileLogger.WriteError(string.Format("セッション情報復元エラー：注文ID={0} {1}", this.OrderId, GetCurrentStateStringsForLog()));
			ToErrorPage();
		}
	}

	/// <summary>
	/// エラーページへ遷移
	/// </summary>
	private void ToErrorPage()
	{
		// エラーページへ遷移
		Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_PAYPAY_IRREGULAR_ERROR);
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
			.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 注文決済画面へ遷移
	/// </summary>
	private void GoToOrderSettlement()
	{
		if (this.PaymentOrder != null)
		{
			this.OrderRegister.PaypayOrders.Remove(this.OrderId);
		}

		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;

		// Chromeの場合、ベリトランスPayPayから戻ってきたときにAuthKeyが再発行され、RestoreSessionされたAuthKeyと異なるので、ここで再発行する。
		Context.Session[Constants.SESSION_KEY_AUTH_KEY_FOR_SECURE_SESSION] = null;
		SessionSecurityManager.PublishAuthKeyIfUnpublished(Context);

		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
	}

	/// <summary>
	/// 現在の状態を文字列で取得（ログ用）
	/// </summary>
	/// <returns>現在の状態</returns>
	private string GetCurrentStateStringsForLog()
	{
		var param = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("ua", this.Request.UserAgent),
			new KeyValuePair<string, string>("addr", this.Request.ServerVariables["REMOTE_ADDR"]),
			new KeyValuePair<string, string>("oid", this.RequestOrderId),
			new KeyValuePair<string, string>("uid", this.LoginUserId),
			new KeyValuePair<string, string>("sid", this.Session.SessionID),
		};

		var result = param
			.Select(kvp => string.Format("{0}={1}", kvp.Key, StringUtility.ToValueIfNullOrEmpty(kvp.Value, "UNKNOWN")))
			.JoinToString(" ");
		return result;
	}

	/// <summary>
	/// Write Payment Log
	/// </summary>
	/// <param name="isSuccess">Is suceess</param>
	/// <param name="externalPaymentCooperationLog">External payment cooperation log</param>
	private void WritePaymentLog(bool isSuccess, string externalPaymentCooperationLog)
	{
		PaymentFileLogger.WritePaymentLog(
			success: isSuccess,
			paymentDetailType: string.Empty,
			accountSettlementCompanyName: PaymentFileLogger.PaymentType.PayPay,
			processingContent: PaymentFileLogger.PaymentProcessingType.Response,
			externalPaymentCooperationLog: externalPaymentCooperationLog,
			idKeyAndValueDictionary: new Dictionary<string, string>
			{
				{ Constants.FIELD_ORDER_ORDER_ID, this.OrderId },
				{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, this.PaymentOrderId }
			});
	}

	/// <summary>決済実行対象注文ID</summary>
	protected string RequestOrderId
	{
		get { return Request[PaypayConstants.REQUEST_KEY_ORDERID]; }
	}
	/// <summary>取引ID</summary>
	protected string PaymentOrderId
	{
		get { return Request[VeriTransConst.VERITRANS_REQUEST_KEY_ORDERID]; }
	}
	/// <summary>Paypay取引ID</summary>
	protected string PaypayOrderId
	{
		get { return Request[VeriTransConst.VERITRANS_REQUEST_KEY_PAYPAY_ORDERID]; }
	}
	/// <summary>注文ID</summary>
	private string OrderId { get; set; }
	/// <summary>注文登録情報</summary>
	private OrderRegisterFront OrderRegister { get; set; }
	/// <summary>決済処理中の注文情報</summary>
	private OrderModel PaymentOrder { get; set; }
	/// <summary>決済処理中のカート情報</summary>
	private CartObject PaymentCart { get; set; }
}
