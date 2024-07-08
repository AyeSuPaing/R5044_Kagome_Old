/*
=========================================================================================================
  Module      : 楽天3Dセキュア認証結果取得ページ処理(GetCard3DSecureAuthResult.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Rakuten;
using w2.Common.Logger;
using w2.Domain.Order;
using Newtonsoft.Json;
using w2.Common.Web;
using w2.App.Common.Order.Payment.Rakuten.AuthorizeHtml;
using System.Text;

public partial class Payment_Rakuten_3DSResult : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		this.OrderRegister = new OrderRegisterFront(this.IsLoggedIn);

		var isSuccess = InitComponentsFromSessionData();
		if (isSuccess == false)
		{
			AppLogger.WriteError(string.Format("セッション情報復元エラー：注文ID={0}", this.OrderId));

			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
			var errorUrlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
				.CreateUrl();
			Response.Redirect(errorUrlCreator);
		}

		ExecPaymentResult();

		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
	}

	/// <summary>
	/// セッション情報から註文情報等を復元して設定
	/// </summary>
	/// <returns>TRUE:セッション復元処理成功 FALSE:失敗</returns>
	private bool InitComponentsFromSessionData()
	{
		try
		{
			RestoreSession(this.OrderId);

			CheckOrderUrlSession();

			var paramSession = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
			this.DispErrorMessages = (List<string>)paramSession["error"];
			this.OrderRegister.SuccessOrders = (List<Hashtable>)paramSession["order"];
			this.OrderRegister.RakutenCard3DSecurePaymentOrders = (List<Hashtable>)paramSession["rakuten_order_3dsecure"];
			this.OrderInfo = this.OrderRegister.RakutenCard3DSecurePaymentOrders.Find(order =>
				StringUtility.ToEmpty(order[Constants.FIELD_ORDER_ORDER_ID]) == this.OrderId);

			var landingCartSessionKey = StringUtility.ToEmpty(Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY]);
			this.CartList = string.IsNullOrEmpty(landingCartSessionKey)
				? GetCartObjectList()
				: (CartObjectList)Session[landingCartSessionKey];

			this.CartObject = this.CartList.Items.Find(item => (item.OrderId == this.OrderId));

			var isSuccess = ((this.OrderInfo != null) && (this.CartObject != null));
			return isSuccess;
		}
		catch (Exception ex)
		{
			AppLogger.WriteError(ex);
			return false;
		}
	}

	/// <summary>
	/// 認証後の注文情報変更処理実行
	/// </summary>
	private void ExecPaymentResult()
	{
		try
		{
			WritePaymentLog(null, LogCreator.CreateMessage(this.OrderId, ""));

			this.RakutenApiAuthResult = JsonConvert.DeserializeObject<RakutenAuthorizeHtmlResponse>(
				Encoding.UTF8.GetString(
					Convert.FromBase64String(
						Request.Form[RakutenConstants.THREE_D_SECURE_RESPONSE_PARAMETER_PAYMENT_RESULT])));

			switch (this.RakutenApiAuthResult.ResultType)
			{
				case RakutenConstants.RESULT_TYPE_SUCCESS:
					ExecPaymentSuccess();
					break;

				case RakutenConstants.RESULT_TYPE_FAILURE:
					ExecPaymentError();
					break;

				case RakutenConstants.RESULT_TYPE_PENDING:
					ExecPaymentPending();
					break;

			}
		}
		catch (Exception ex)
		{
			this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION));
			var logMessage = OrderCommon.CreateOrderFailedLogMessage(
				this.TransactionName,
				this.OrderInfo,
				this.CartObject);

			WritePaymentLog(
				false,
				BaseLogger.CreateExceptionMessage(logMessage, ex));
		}

		if (this.OrderInfo != null)
		{
			this.OrderRegister.RakutenCard3DSecurePaymentOrders.Remove(this.OrderInfo);
		}
	}

	/// <summary>
	/// 認証後の注文情報変更処理実行（3Dセキュア認証成功）
	/// </summary>
	private void ExecPaymentSuccess()
	{
		var orderService = new OrderService();
		var paymentStatusCompleteFlg = (this.CartObject.HasDigitalContents && Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
			? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
			: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;
		var successPaymentStatus = paymentStatusCompleteFlg
			? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
			: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;

		this.OrderInfo[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;
		this.OrderInfo[Constants.FIELD_ORDER_CARD_TRAN_ID] = this.RakutenApiAuthResult.AgencyRequestId;
		this.OrderInfo[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = this.RakutenApiAuthResult.PaymentId;

		if (this.CartObject.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
		{
			// カード登録を選択した場合、表示フラグを立てる
			this.CartObject.Payment.UserCreditCard.UpdateDispFlg(
				this.CartObject.Payment.UserCreditCardRegistFlg,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert);
		}

		var isSuccess = this.OrderRegister.UpdateForOrderComplete(
			this.OrderInfo,
			this.CartObject,
			true,
			UpdateHistoryAction.DoNotInsert);

		if (isSuccess)
		{
			var isSendMail = orderService.GetOrdersByUserId(this.CartObject.OrderUserId)
				.Where(userOrder => ((userOrder.OrderId != this.OrderId)
					&& (userOrder.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP)))
				.Any(userOrder => this.CartList.Items
					.Select(item => item.OrderId)
					.Contains(userOrder.OrderId));

			if (isSendMail) this.OrderInfo[Constants.ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE] = null;

			this.OrderRegister.OrderCompleteProcesses(
				this.OrderInfo,
				this.CartObject,
				UpdateHistoryAction.DoNotInsert);

			this.OrderRegister.AfterOrderCompleteProcesses(
				this.OrderInfo,
				this.CartObject,
				UpdateHistoryAction.Insert);

			this.OrderRegister.SuccessOrders.Add(this.OrderInfo);

			WritePaymentLog(true, string.Empty);
		}
	}

	/// <summary>
	/// 認証後の注文情報変更処理実行（3Dセキュア認証失敗）
	/// </summary>
	private void ExecPaymentError()
	{
		var orderService = new OrderService();
		var apiErrorMessage = LogCreator.CreateErrorMessage(
			this.RakutenApiAuthResult.ErrorCode,
			this.RakutenApiAuthResult.ErrorMessage);
		FileLogger.Write("Rakuten", apiErrorMessage, true);

		this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
		if (string.IsNullOrEmpty(this.RakutenApiAuthResult.ErrorMessage) == false)
		{
			this.DispErrorMessages.Add(this.RakutenApiAuthResult.ErrorMessage);
		}

		this.TransactionName = "2-X.注文ロールバック処理";
		var isUserDeleted = (((this.IsLoggedIn == false) || (this.RegisterUser != null))
			&& (this.SuccessOrders.Count == 0)
			&& (orderService.GetOrdersByUserId(this.CartObject.OrderUserId).Length == 1));

		OrderCommon.RollbackPreOrder(
			this.OrderInfo,
			this.CartObject,
			isUserDeleted,
			(int)this.OrderInfo[Constants.FIELD_USERSHIPPING_SHIPPING_NO],
			this.IsLoggedIn,
			UpdateHistoryAction.Insert);

		if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE
			&& this.IsLoggedIn
			&& isUserDeleted)
		{
			this.LoginUserId = null;
		}

		WritePaymentLog(
			false,
			OrderCommon.CreateRakuten3DSecureOrderFailedLogMessage(
				this.OrderInfo,
				apiErrorMessage));
	}

	/// <summary>
	/// 認証後の注文情報変更処理実行（3Dセキュア認証ペンディング）
	/// ※決済が成功している可能性があるので仮注文は残す
	/// </summary>
	private void ExecPaymentPending()
	{
		var apiErrorMessage = LogCreator.CreateErrorMessage(
			this.RakutenApiAuthResult.ErrorCode,
			this.RakutenApiAuthResult.ErrorMessage);
		FileLogger.Write("Rakuten", apiErrorMessage, true);

		this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
		if (string.IsNullOrEmpty(this.RakutenApiAuthResult.ErrorMessage) == false)
		{
			this.DispErrorMessages.Add(this.RakutenApiAuthResult.ErrorMessage);
		}

		WritePaymentLog(
			false,
			OrderCommon.CreateRakuten3DSecureOrderFailedLogMessage(
				this.OrderInfo,
				apiErrorMessage));
	}

	/// <summary>
	/// 楽天3DSecure認証結果受け取りログ出力
	/// </summary>
	/// <param name="isSuccess">処理結果成否（NULLの場合は単純な通信ログ）</param>
	/// <param name="externalPaymentCooperationLog">外部決済連携ログメッセージ</param>
	private void WritePaymentLog(
		bool? isSuccess,
		string externalPaymentCooperationLog)
	{
		PaymentFileLogger.WritePaymentLog(
			isSuccess,
			Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
			PaymentFileLogger.PaymentType.Rakuten,
			PaymentFileLogger.PaymentProcessingType.Rakuten3DSecureAuthResultNotification,
			externalPaymentCooperationLog);
	}

	/// <summary>註文情報</summary>
	private Hashtable OrderInfo { get; set; }
	/// <summary>カート情報</summary>
	private CartObject CartObject { get; set; }
	/// <summary>註文登録クラス</summary>
	private OrderRegisterFront OrderRegister { get; set; }
	/// <summary>註文ID</summary>
	private string OrderId { get { return Request[Constants.REQUEST_KEY_ORDER_ID]; } }
	/// <summary>3Dセキュア認証APIレスポンス</summary>
	private RakutenAuthorizeHtmlResponse RakutenApiAuthResult { get; set; }
}
