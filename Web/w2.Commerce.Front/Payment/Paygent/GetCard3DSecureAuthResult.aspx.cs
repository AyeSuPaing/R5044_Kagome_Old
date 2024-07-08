/*
=========================================================================================================
  Module      : ペイジェント3Dセキュア2.0認証結果取得ページ処理(GetCard3DSecureAuthResult.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.Paygent;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain.Cart;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// ペイジェント3Dセキュア2.0認証結果取得ページ処理
/// </summary>
public partial class Payment_Card3DSecureAuthPaygent_GetCard3DSecureAuthResult : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	protected void Page_Load(object sender, EventArgs e)
	{
		// リクエストデータがあるか
		HasRequstParams();

		// 画面遷移の正当性チェック
		CheckOrderUrlSession();

		// ペイジェント3Dセキュア認証結果をもとに認証
		AuthorizeBasedOn3DSecureResult();
	}

	/// <summary>
	/// リクエストデータがあるか
	/// </summary>
	private void HasRequstParams()
	{
		if (this.Request.Params["result"] == null)
		{
			RedirectToErrorPage();
		}
	}

	/// <summary>
	/// ペイジェント3Dセキュア認証結果をもとに認証
	/// </summary>
	private void AuthorizeBasedOn3DSecureResult()
	{
		this.TransactionName = "2-1-J.PAYGENTクレジット決済処理(3Dセキュア2.0認証)";

		// セッションからデータ復元
		var hasSuccessfullyRestored = RestoreSessionDataset();

		// 3DS2.0認証結果応答電文の内容をもとに認証
		var requestDataset = GetRequestDataset();
		var attemptResult = Authorize(requestDataset);

		// 失敗時はエラーメッセージをセットしてトークン無効化
		// 仮注文ロールバックは失敗時すでにされている
		if (attemptResult == false)
		{
			this.DispErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
			ExpireToken();
		}
		// attemptOK時は与信取得実施
		else
		{
			var authResult = SendAuth()[PaygentConstants.RESPONSE_STATUS].ToString();
			// 与信取得失敗時はロールバック
			if (authResult == PaygentConstants.PAYGENT_RESPONSE_STATUS_FAILURE)
			{
				PaygentUtility.RollbackPaygentPreOrder(this.PaymentOrder, this.PaymentCart);
				this.OrderRegister.PaygentCard3DSecurePaymentOrders.Remove(this.PaymentOrder);
			}
			// 成功注文に登録
			this.OrderRegister.SuccessOrders.Add(this.PaymentOrder);
			this.OrderRegister.PaygentCard3DSecurePaymentOrders.Remove(this.PaymentOrder);
			// 注文確定
			CompleteOrder();
		}

		RedirectToOrderSettlement();
	}

	/// <summary>
	/// 3DS認証後のオーソリ処理
	/// </summary>
	/// <returns>ペイジェントAPIレスポンス</returns>
	private IDictionary SendAuth()
	{
		var authParams = new PaygentCreditCardAuthApi();
		authParams.TradingId = (string)this.PaymentOrder[Constants.FIELD_ORDER_ORDER_ID];
		authParams.PaymentAmount = Math.Floor((decimal)this.PaymentOrder[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL]).ToString();
		authParams.UseType = "2";
		// 支払回数が1回：PaymentClass = 10, SplitCount = ""
		// 支払回数が1回以外：PaymentClass = 61, SplitCount = "paygent_installments"
		// 一括・分割払い以外は対応しない
		if ((string)this.PaymentOrder["paygent_installments"] == "1")
		{
			authParams.PaymentClass = PaygentConstants.PAYGENT_PAYMENT_CLASS_FULL;
			authParams.SplitCount = "";
		}
		else
		{
			authParams.PaymentClass = PaygentConstants.PAYGENT_PAYMENT_CLASS_INSTALLMENTS;
			authParams.SplitCount = (string)this.PaymentOrder["paygent_installments"];
		}
		authParams.Skip3DSecure = "";
		// 登録済みカード：1 新規カード：0
		authParams.StockCardMode =
			string.IsNullOrEmpty((string)this.PaymentOrder[Constants.FIELD_USERCREDITCARD_COOPERATION_ID2])
			? "0"
			: "1";
		// ペイジェント側顧客ID（w2_UserCreditCard.cooperation_id）
		authParams.CustomerId = (string)this.PaymentOrder[Constants.FIELD_USERCREDITCARD_COOPERATION_ID];
		// ペイジェント側顧客カードID（w2_UserCreditCard.cooperation_id2）
		authParams.CustomerCardId = (string)this.PaymentOrder[Constants.FIELD_USERCREDITCARD_COOPERATION_ID2];
		authParams.Skip3DSecure = "";
		// オーソリのみ：0 or null オーソリ+売上：1
		var paymentMethod = Constants.DIGITAL_CONTENTS_OPTION_ENABLED && this.PaymentCart.HasDigitalContents
			? Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS
			: Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD;
		authParams.SalesMode = paymentMethod == Constants.PaygentCreditCardPaymentMethod.Auth
			? "0"
			: "1";
		authParams.SecurityCodeUse = "0";
		authParams.AuthId = (string)((IDictionary)Session["Paygent3DSResult"])["3ds_auth_id"];

		var authResult = PaygentApiFacade.SendRequest(authParams);

		// レスポンス結果が失敗だったらエラー出力
		if ((string)authResult[PaygentConstants.RESPONSE_STATUS] == PaygentConstants.PAYGENT_RESPONSE_STATUS_FAILURE)
		{
			PaygentUtility.RollbackPaygentPreOrder(this.PaymentOrder, this.PaymentCart);
			this.DispErrorMessages.Add("3Dセキュア認証に失敗しました。再度お試しいただくか、他の決済方法をご利用ください。");
		}
		Session[PaygentConstants.PAYGENT_SESSION_AUTH_RESULT] = new Tuple<string, string>(authParams.AuthId, (string)authResult["payment_id"]);
		return authResult;
	}

	/// <summary>
	/// 注文完了処理 各種ステータスを更新してメール送信
	/// </summary>
	private void CompleteOrder()
	{
		var register = new OrderRegisterFront(this.IsLoggedIn);
			var paygentAuthResults = (Tuple<string, string>)Session[PaygentConstants.PAYGENT_SESSION_AUTH_RESULT];
			
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();
				var orderService = new OrderService();
				var updateOrder = orderService.Get(this.PaymentCart.OrderId, accessor);
				// 同時売上モードオンか
				var isPaygentCreditPaymentWithAuth = (this.PaymentCart.HasDigitalContents
					? (Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS == Constants.PaygentCreditCardPaymentMethod.Capture)
					: (Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD == Constants.PaygentCreditCardPaymentMethod.Capture));
				// 注文ステータスを更新
				var count = orderService.UpdateOrderStatus(
					this.PaymentCart.OrderId,
					Constants.FLG_ORDER_ORDER_STATUS_ORDERED,
					DateTime.Now,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert,
					accessor);
				// 外部決済ステータス・外部決済与信日時を更新
				orderService.UpdateExternalPaymentInfo(
					this.PaymentCart.OrderId,
					isPaygentCreditPaymentWithAuth
						? Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP
						: Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP,
					true,
					DateTime.Now,
					string.Empty,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert,
					accessor);
				// オンライン決済ステータスを更新
				orderService.UpdateOnlinePaymentStatus(
					this.PaymentCart.OrderId,
					isPaygentCreditPaymentWithAuth
						? Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED
						: Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert,
					accessor);
				// 3DSecuretranId, cardTranIdを更新
				count = orderService.UpdatePaygentOrder(
					this.PaymentCart.OrderId,
					paygentAuthResults.Item1,
					paygentAuthResults.Item2,
					accessor);
				// 決済連携メモ更新
				orderService.AddPaymentMemo(
					updateOrder.OrderId,
					OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
						string.IsNullOrEmpty(updateOrder.PaymentOrderId)
							? updateOrder.OrderId
							: updateOrder.PaymentOrderId,
						updateOrder.OrderPaymentKbn,
						paygentAuthResults.Item2,
						ValueText.GetValueText(
							Constants.TABLE_ORDER,
							Constants.VALUETEXT_PARAM_PAYMENT_MEMO,
							isPaygentCreditPaymentWithAuth
								? Constants.VALUETEXT_PARAM_SALES_CONFIRMED
								: Constants.VALUETEXT_PARAM_AUTH),
						CurrencyManager.GetSendingAmount(
							updateOrder.LastBilledAmount,
							updateOrder.SettlementAmount,
							updateOrder.SettlementCurrency)),
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				var paygentPaymentMethod = Constants.DIGITAL_CONTENTS_OPTION_ENABLED && this.PaymentCart.HasDigitalContents
					? Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS
					: Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD;
				// オーソリ後に入金ステータスを入金済みにするか判定
				var isPaygentCreditPaymentStatusComplete = this.PaymentCart.HasDigitalContents
					? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
					: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;
				// 同時売上の場合、ここまで来ていたら成功なので入金ステータスを更新
				if (paygentPaymentMethod == Constants.PaygentCreditCardPaymentMethod.Capture
					&& isPaygentCreditPaymentStatusComplete)
				{
					orderService.UpdateOrderPaymentStatusComplete(
						this.PaymentCart.OrderId,
						DateTime.Now,
						Constants.FLG_LASTCHANGED_USER,
						accessor);
				}

				// シリアルキー引き渡し
				if (Constants.DIGITAL_CONTENTS_OPTION_ENABLED
					&& (this.PaymentCart != null)
					&& this.PaymentCart.IsDigitalContentsOnly
					&& (Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS == Constants.PaygentCreditCardPaymentMethod.Capture))
				{
					OrderCommon.DeliverSerialKeyForOrderComplete(
						this.PaymentCart.OrderId,
						Constants.FLG_LASTCHANGED_USER,
						accessor,
						UpdateHistoryAction.DoNotInsert);

					foreach (var cp in this.PaymentCart.Items)
					{
						cp.IsDelivered = true;
					}
				}
				accessor.CommitTransaction();
			}

			// 注文完了時処理
			this.OrderRegister.OrderCompleteProcesses(
				this.PaymentOrder,
				this.PaymentCart,
				UpdateHistoryAction.DoNotInsert);

			// 注文完了後処理
			this.OrderRegister.AfterOrderCompleteProcesses(
				this.PaymentOrder,
				this.PaymentCart,
				UpdateHistoryAction.Insert);

			// 注文完了メールを送信する
			var order = CreateOrderInfo(this.PaymentCart, this.PaymentCart.OrderId, this.LoginUserId);
			register.SendOrderMails(order, this.PaymentCart, true);
	}

	/// <summary>
	/// セッションからデータ復元
	/// </summary>
	/// <returns>データ復元結果</returns>
	private bool RestoreSessionDataset()
	{
		// セッションデータ復元
		var orderId = this.Request[Constants.REQUEST_KEY_ORDER_ID];
		var hasSuccessfullyRestored = RestoreSession(orderId);
		if (hasSuccessfullyRestored == false)
		{
			var msg = string.Format("セッション情報復元エラー。もう一度やり直してください。注文ID={0}", orderId);
			FileLogger.WriteError(msg);
			return false;
		}

		// エラーメッセージがあれば、それを出力
		var param = (Hashtable)this.Session[Constants.SESSION_KEY_PARAM];
		this.DispErrorMessages = (List<string>)param["error"];

		// 注文登録情報取得
		var orderRegister = new OrderRegisterFront(this.IsLoggedIn)
		{
			SuccessOrders = (List<Hashtable>)param["order"],
			PaygentCard3DSecurePaymentOrders = (List<Hashtable>)param["paygent_order_3dsecure"],
		};
		this.OrderRegister = orderRegister;

		// ペイジェント3Dセキュア認証対象の注文があるか
		var hasAnyPaygent3DSecurePaymentOrder = orderRegister.PaygentCard3DSecurePaymentOrders.Any();
		if (hasAnyPaygent3DSecurePaymentOrder == false)
		{
			var msg = string.Format("ペイジェント3Dセキュア認証対象の注文がありません。もう一度やり直してください。注文ID={0}", orderId);
			FileLogger.WriteError(msg);
			return false;
		}

		// 決済処理中の注文情報、カート情報を取得
		var strLandingCartSessionKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
		this.CartList = (strLandingCartSessionKey == null)
			? GetCartObjectList()
			: (CartObjectList)Session[strLandingCartSessionKey];
		this.PaymentOrder =
			orderRegister.GetPaygent3DSecurOrderByOrderId(orderId);
		this.PaymentCart = this.CartList.GetCartWithOrderId(orderId);
		var isSuccessful = ((this.PaymentOrder.Count > 0) && (this.PaymentCart != null));
		if (isSuccessful == false)
		{
			var msg = string.Format("決済処理中の注文情報、カート情報復元エラー。もう一度やり直してください。注文ID={0}", orderId);
			FileLogger.WriteError(msg);
		}
		return isSuccessful;
	}

	/// <summary>
	/// 3Dセキュア2.0認証結果応答電文からリクエストパラメータを取得
	/// </summary>
	public PaygentCreditCard3DSecureAuthApiResult GetRequestDataset()
	{
		// パラメータ取得
		var requestParams = this.Request.Params;
		var requestDataset = new PaygentCreditCard3DSecureAuthApiResult(
			requestParams["result"],
			requestParams["3ds_auth_id"],
			requestParams["card_brand"],
			requestParams["issur_class"],
			requestParams["acq_name"],
			requestParams["acq_id"],
			requestParams["issur_name"],
			requestParams["issur_id"],
			requestParams["hc"],
			requestParams["fingerprint"],
			requestParams["masked_card_number"],
			requestParams["3dsecure_requestor_error_code"],
			requestParams["3dsecure_server_error_code"],
			StringUtility.ToEmpty(requestParams["attempt_kbn"]),
			StringUtility.ToEmpty(requestParams["response_code"]),
			StringUtility.ToEmpty(requestParams["response_detail"]));
		return requestDataset;
	}

	/// <summary>
	/// 3DS2.0認証の結果に基づいたオーソリ実施判断
	/// </summary>
	/// <param name="secureAuthResult">3Dセキュア認証結果</param>
	/// <returns>オーソリ実施可否</returns>
	public bool Authorize(PaygentCreditCard3DSecureAuthApiResult secureAuthResult)
	{
		var paygent3DSAuthResult = new PaygentCreditCard3DSecureResult(secureAuthResult);
		// ハッシュ値の検証
		if (paygent3DSAuthResult.VerifyChecksum() == false) return false;
		var authResult = paygent3DSAuthResult.Authorize();

		// 3Dセキュア実施後ロールバックが必要な場合ロールバック実施
		if (authResult == PaygentConstants.PAYGENT_3DSECURE_RESULT_NG)
		{
			PaygentUtility.RollbackPaygentPreOrder(this.PaymentOrder, this.PaymentCart);
			this.OrderRegister.PaygentCard3DSecurePaymentOrders.Remove(this.PaymentOrder);
		}

		return authResult == PaygentConstants.PAYGENT_3DSECURE_RESULT_OK;
	}

	/// <summary>
	/// 注文決済画面へ遷移
	/// </summary>
	private void RedirectToOrderSettlement()
	{
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		this.Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;

		// 画面遷移
		this.Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
	}

	/// <summary>
	/// エラーページへ遷移
	/// </summary>
	private void RedirectToErrorPage()
	{
		// エラーページへ遷移
		var msg = "\n" + "3Dセキュア認証に失敗しました。再度お試しいただくか、他の決済方法をご利用ください。";
		Session[Constants.SESSION_KEY_ERROR_MSG] = msg;
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
			.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
			.CreateUrl();
		this.Response.Redirect(url);
	}

	/// <summary>
	/// トークンを期限切れにする
	/// </summary>
	private void ExpireToken()
	{
		// 発行されたトークンキーは複数回利用は出来ない仕様のため、一度失敗したら有効期限切れとする
		var carts = this.CartList.Items.Where(cart => (cart != null) && cart.Payment.HasAnyToken());
		foreach (var cart in carts)
		{
			cart.Payment.CreditToken.SetTokneExpired();
		}
	}

	/// <summary>注文登録情報</summary>
	private OrderRegisterFront OrderRegister { get; set; }
	/// <summary>決済処理中の注文情報</summary>
	private Hashtable PaymentOrder { get; set; }
	/// <summary>決済処理中のカート情報</summary>
	private CartObject PaymentCart { get; set; }
}
