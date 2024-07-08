/*
=========================================================================================================
  Module      : AmazonPayCv2オーソリ結果取得ページ(AmazonPayComplete.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Helper;
using w2.App.Common.AmazonCv2;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Order.Helper;

public partial class AmazonPayComplete : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(Request.Form["exec"])) return;

		var order = new Hashtable();
		CartObject cart = null;

		OrderRegisterFront register = new OrderRegisterFront(this.IsLoggedIn);

		string orderId = Request.Form[Constants.REQUEST_KEY_ORDER_ID];

		PaymentFileLogger.WritePaymentLog(
			null,
			Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2,
			PaymentFileLogger.PaymentType.AmazonCv2,
			PaymentFileLogger.PaymentProcessingType.Response,
			LogCreator.CreateMessage(orderId, ""));

		// セッションチェック及びカート復元
		var isSuccess = CheckAndRestoreSession(orderId, register, ref order, ref cart);

		this.AmazonFacade = new AmazonCv2ApiFacade();

		// 決済
		ExecPayment(cart, order, isSuccess, orderId, register);

		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;

		// 画面遷移
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
	}

	/// <summary>
	/// セッションチェックおよび、セッションからカート復元
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="register">注文登録</param>
	/// <param name="order">注文</param>
	/// <param name="cart">カート</param>
	/// <returns></returns>
	private bool CheckAndRestoreSession(
		string orderId,
		OrderRegisterFront register,
		ref Hashtable order,
		ref CartObject cart)
	{
		var isSuccess = true;

		try
		{
			RestoreSession(orderId);

			if (isSuccess)
			{
				// 画面遷移の正当性チェック
				CheckOrderUrlSession();

				// セッションからパラメータ取得
				string strLandingCartSessionKey = (string)this.Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
				this.CartList = (strLandingCartSessionKey == null)
					? GetCartObjectList()
					: (CartObjectList)this.Session[strLandingCartSessionKey];
				this.AmazonModel = (AmazonModel)this.Session[AmazonConstants.SESSION_KEY_AMAZON_MODEL];
				Hashtable htParam = (Hashtable)this.Session[Constants.SESSION_KEY_PARAM];
				this.DispErrorMessages = (List<string>)htParam["error"];

				register.SuccessOrders = (List<Hashtable>)htParam["order"];
				register.AmazonPayCv2Orders = (List<KeyValuePair<string, Hashtable>>)htParam["order_amazonpay_cv2"];
				register.GoogleAnalyticsParams = (List<Hashtable>)htParam["googleanaytics_params"];

				order = register.AmazonPayCv2Orders.Find(o => o.Key == this.Request[Constants.REQUEST_KEY_ORDER_ID]).Value;

				cart = this.CartList.Items.Find(c => c.OrderId == this.Request[Constants.REQUEST_KEY_ORDER_ID]);

				if ((order == null) || (cart == null))
				{
					isSuccess = false;
				}
			}
		}
		catch (Exception ex)
		{
			isSuccess = false;
			AppLogger.WriteError(ex);
		}

		// セッション復元失敗（セッション切れ、もしくは他ブラウザでの操作）
		if (isSuccess == false)
		{
			AppLogger.WriteError("セッション情報復元エラー：注文ID=" + orderId);

			// エラーページへ
			var cartList = Constants.PATH_ROOT + Constants.PAGE_FRONT_CART_LIST;
			var next = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
				.AddParam(Constants.REQUEST_KEY_BACK_URL, cartList)
				.CreateUrl();

			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHANGE_TO_ANOTHER_PAYMENT_FOR_AUTH_ERROR);
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = cartList;
			Response.Redirect(next);
		}

		return isSuccess;
	}

	/// <summary>
	/// 決済実行
	/// </summary>
	/// <param name="cart">カート</param>
	/// <param name="order">注文</param>
	/// <param name="isSuccess">成功か</param>
	/// <param name="orderId">注文ID</param>
	/// <param name="register">注文登録</param>
	private void ExecPayment(
		CartObject cart,
		Hashtable order,
		bool isSuccess,
		string orderId,
		OrderRegisterFront register)
	{
		try
		{
			//------------------------------------------------------
			// 2.外部連携決済処理
			// 2-6-B.AmazonPayCv2決済処理
			//------------------------------------------------------
			this.TransactionName = "2-6-B.AmazonPayCv2決済処理";

			var con = this.AmazonFacade.CompleteCheckoutSession(this.AmazonCheckoutSessionId, cart.PriceTotal);
			var isConSuccess = con.Success;

			var conError = AmazonCv2ApiFacade.GetErrorCodeAndMessage(con);

			PaymentFileLogger.WritePaymentLog(
				isConSuccess,
				cart.Payment.PaymentId,
				PaymentFileLogger.PaymentType.AmazonCv2,
				PaymentFileLogger.PaymentProcessingType.OrderInfoApproval,
				isConSuccess
					? string.Empty
					: OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						new Hashtable
						{
							{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, con.ChargePermissionId },
							{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
							{ Constants.FIELD_ORDER_USER_ID, (string)order[Constants.FIELD_ORDER_USER_ID] }
						},
						cart,
						LogCreator.CreateErrorMessage(conError.ReasonCode, conError.Message)),
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, con.ChargePermissionId },
					{ Constants.FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID, cart.AmazonOrderReferenceId }
				});

			OrderCommon.AppendExternalPaymentCooperationLog(
				isConSuccess,
				(string)order[Constants.FIELD_ORDER_ORDER_ID],
				isConSuccess
					? string.Format("hfAmazonOrderRefID:{0}", con.ChargePermissionId)
					: LogCreator.CreateErrorMessage(conError.ReasonCode, conError.Message),
				(string)order[Constants.FIELD_ORDER_LAST_CHANGED],
				UpdateHistoryAction.Insert);

			isSuccess = (isConSuccess && isSuccess);

			if (isSuccess)
			{
				// 定期の場合はチャージ（支払）オブジェクト作成、通常注文はチャージオブジェクト取得
				var charge = (cart.HasFixedPurchase)
					? this.AmazonFacade.CreateCharge(con.ChargePermissionId, cart.PriceTotal)
					: this.AmazonFacade.GetCharge(con.ChargeId);

				// 即時決済の場合アマゾン側のステータスが売上確定済みか確認する
				if (Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW)
				{
					var chargeResult = (charge.StatusDetails.State == AmazonCv2Constants.FLG_CHARGE_STATUS_CAPTURED);

					PaymentFileLogger.WritePaymentLog(
						chargeResult,
						cart.Payment.PaymentId,
						PaymentFileLogger.PaymentType.AmazonCv2,
						PaymentFileLogger.PaymentProcessingType.ImmediateSettlement,
						chargeResult
							? string.Empty
							: OrderCommon.CreateOrderFailedLogMessage(
								this.TransactionName,
								order,
								cart,
								"何らかの理由により与信後の即時売上が正常に終了しませんでした。\t" + LogCreator.CreateErrorMessage(
									conError.ReasonCode,
									conError.Message)),
						new Dictionary<string, string>
						{
							{ Constants.FIELD_ORDER_ORDER_ID, (string)order[Constants.FIELD_ORDER_ORDER_ID] },
							{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, con.ChargePermissionId },
							{ Constants.FOR_LOG_KEY_AMAZON_ORDER_REFERENCE_ID, cart.AmazonOrderReferenceId }
						});

					// 売上確定済みになっていない場合
					if (chargeResult == false)
					{
						this.DispErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CHANGE_TO_ANOTHER_PAYMENT_FOR_AUTH_ERROR));
					}

					if (order.ContainsKey(Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS))
					{
						order[Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS] = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
					}
					else
					{
						order.Add(
							Constants.FIELD_ORDER_ONLINE_PAYMENT_STATUS,
							Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED);
					}

					if (order.ContainsKey(Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS))
					{
						order[Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS] =
							Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP;
					}
					else
					{
						order.Add(
							Constants.FIELD_ORDER_EXTERNAL_PAYMENT_STATUS,
							Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP);
					}
				}

				var orderPaymentStatus = Constants.PAYMENT_AMAZON_PAYMENTSTATUSCOMPLETE
					? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
					: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;
				// 入金ステータスを格納
				if (order.ContainsKey(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS))
				{
					order[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = orderPaymentStatus;
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, orderPaymentStatus);
				}

				// Amazon決済IDをセット
				if (order.ContainsKey(Constants.FIELD_ORDER_PAYMENT_ORDER_ID))
				{
					order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID] = charge.ChargePermissionId;
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_PAYMENT_ORDER_ID, charge.ChargePermissionId);
				}

				if (order.ContainsKey(Constants.FIELD_ORDER_CARD_TRAN_ID))
				{
					order[Constants.FIELD_ORDER_CARD_TRAN_ID] = charge.ChargeId;
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, charge.ChargeId);
				}

				// 決済連携メモ挿入
				var paymentMemo = OrderCommon.CreateOrderPaymentMemoForAuth(
					(string)order[Constants.FIELD_ORDER_ORDER_ID],
					(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
					(string)order[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN],
					(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID],
					cart.PriceTotal);

				if (order.ContainsKey(Constants.FIELD_ORDER_PAYMENT_MEMO))
				{
					order[Constants.FIELD_ORDER_PAYMENT_MEMO] = OrderExternalPaymentUtility.SetExternalPaymentMemo(
						StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_MEMO]),
						paymentMemo);
				}
				else
				{
					order.Add(Constants.FIELD_ORDER_PAYMENT_MEMO, paymentMemo);
				}

				if (cart.HasFixedPurchase)
				{
					new FixedPurchaseService().SetExternalPaymentAgreementId(
						(string)order[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID],
						charge.ChargePermissionId,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.Insert);
				}
			}
			//------------------------------------------------------
			// ２－Ｘ．失敗時処理（仮注文ロールバック）
			//		・失敗があれば処理をロールバック（＝仮注文のこす）し、例外をとばす
			//------------------------------------------------------
			if (isSuccess == false)
			{
				// エラーメッセージ追記
				this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CHANGE_TO_ANOTHER_PAYMENT_FOR_AUTH_ERROR));

				// 情報ログ出力
				string strLogMessage = OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart);

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2,
					PaymentFileLogger.PaymentType.AmazonCv2,
					PaymentFileLogger.PaymentProcessingType.ApiRequest,
					strLogMessage,
					new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, orderId },
						{ Constants.FIELD_ORDER_USER_ID, this.CartList.UserId }
					});

				// 仮注文情報削除処理(ゲスト削除、ポイント戻しも行う)
				this.TransactionName = "2-X.注文ロールバック処理";

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
					UpdateHistoryAction.Insert);

				if (Constants.LANDING_CART_USER_REGISTER_WHEN_ORDER_COMPLETE && isUserDelete && this.IsLoggedIn)
				{
					this.LoginUserId = null;
				}
			}
		}
		// 例外時
		catch (Exception ex)
		{
			// 失敗
			isSuccess = false;

			// 決済自体が完了してしまっていることも考えられるので仮注文データを「のこす」
			// また、ロールバック処理でエラーの場合（仮注文を戻せなかった）も
			// データの不整合を防ぐために仮注文データを「のこす」

			// エラーメッセージ追記
			this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION));

			// ログ出力
			string strLogMessage = OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, order, cart);

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				false,
				Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2,
				PaymentFileLogger.PaymentType.AmazonCv2,
				PaymentFileLogger.PaymentProcessingType.ApiError,
				BaseLogger.CreateExceptionMessage(strLogMessage, ex),
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, orderId },
					{ Constants.FIELD_ORDER_USER_ID, this.CartList.UserId }
				});
		}

		//------------------------------------------------------
		// ３．注文確定処理
		//	・ここを正常通過すれば何があっても注文完了。
		//------------------------------------------------------
		if (isSuccess)
		{
			//------------------------------------------------------
			// ３－１．注文ステータス更新
			//------------------------------------------------------
			isSuccess = register.UpdateForOrderComplete(order, cart, true, UpdateHistoryAction.DoNotInsert);
		}

		//------------------------------------------------------
		// ４．後処理
		//------------------------------------------------------
		if (isSuccess)
		{
			// 注文完了時処理
			var isSendMail = new OrderService().GetOrdersByUserId(cart.OrderUserId)
				.Where(userOrder => (userOrder.OrderId != this.Request[Constants.REQUEST_KEY_ORDER_ID]) 
					&& (userOrder.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP))
				.Any(userOrder => this.CartList.Items.Select(item => item.OrderId)
					.Contains(userOrder.OrderId));
			if (isSendMail) order[Constants.ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE] = null;
			register.OrderCompleteProcesses(order, cart, UpdateHistoryAction.DoNotInsert);

			// 注文完了後処理
			register.AfterOrderCompleteProcesses(order, cart, UpdateHistoryAction.Insert);

			// 注文完了画面用に注文IDを格納
			register.SuccessOrders.Add(order);
		}

		if (order != null)
		{
			register.AmazonPayCv2Orders.Remove(
				register.AmazonPayCv2Orders.Find(o => (o.Key == (string)order[Constants.FIELD_ORDER_ORDER_ID])));
		}
	}

	/// <summary>アマゾンモデル</summary>
	private AmazonModel AmazonModel { get; set; }
	/// <summary>AmazonCv2ファサード</summary>
	private AmazonCv2ApiFacade AmazonFacade { get; set; }
	/// <summary>AmazonCv2チェックアウトセッションID</summary>
	private string AmazonCheckoutSessionId
	{
		get { return (string)Session[AmazonCv2Constants.SESSION_KEY_AMAZON_CHECKOUT_SESSION_ID]; }
	}
}
