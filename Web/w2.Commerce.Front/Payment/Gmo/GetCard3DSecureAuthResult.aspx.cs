/*
=========================================================================================================
  Module      : GMO3Dセキュア認証結果取得ページ処理(GetCard3DSecureAuthResult.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.GMO;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// GMO3Dセキュア認証結果取得ページ処理
/// </summary>
public partial class Payment_GMO_GetCard3DSecureAuthResult : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (this.Request.Form["exec"] == null) return;

		// パラメータ取得
		this.OrderId = this.Request.Form[Constants.REQUEST_KEY_ORDER_ID];

		// セッションからパラメータ取得
		GetParametersFromSession();

		// GMO3Dセキュア認証後の処理を実行
		ExecAfterCard3DCardAuth();
	}

	/// <summary>
	/// GMO3Dセキュア認証後の処理を実行
	/// </summary>
	private void ExecAfterCard3DCardAuth()
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
						"UpdateForOrderComplete GetCard3DSecureAuthResult orderId:"
						+ this.OrderId);

					success = this.OrderRegister.UpdateForOrderComplete(
						this.PaymentOrder,
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
		if (this.PaymentOrder != null)
		{
			this.OrderRegister.SuccessOrders.Add(this.PaymentOrder);
			this.OrderRegister.GmoCard3DSecurePaymentOrders.Remove(this.PaymentOrder);
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
			ZeusCard3DSecurePaymentOrders = (List<Hashtable>)param["zeus_order_3dsecure"],
			GmoCard3DSecurePaymentOrders = (List<Hashtable>)param["gmo_order_3dsecure"],
			GoogleAnalyticsParams = (List<Hashtable>)param["googleanaytics_params"],
		};

		// 決済処理中の注文情報、カート情報を取得
		this.PaymentOrder = this.OrderRegister.GmoCard3DSecurePaymentOrders.Find(o =>
			((string)o[Constants.FIELD_ORDER_ORDER_ID] == this.Request[Constants.REQUEST_KEY_ORDER_ID]));
		this.PaymentCart = this.CartList.Items.Find(c =>
			(c.OrderId == this.Request[Constants.REQUEST_KEY_ORDER_ID]));
		if ((this.PaymentOrder == null) && (this.PaymentCart == null)) success = false;

		if (success == false)
		{
			FileLogger.WriteError("セッション情報復元エラー：注文ID=" + this.OrderId);
			ToErrorPage();
		};
	}

	/// <summary>
	/// エラーページへ遷移
	/// </summary>
	private void ToErrorPage()
	{
		// トークンを消す
		SetTokenExpired();

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
		this.TransactionName = "2-1-F.GMOクレジット決済処理";

		try
		{
			var paymentStatusCompleteFlg =
				(this.PaymentCart.HasDigitalContents && Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
					? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
					: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;
			var successPaymentStatus = paymentStatusCompleteFlg
				? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
				: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;

			var card3DSecureAuthResult = this.Request.Form[PaymentGmo.PARAM_PA_RES];
			var card3DSecureTranId = this.Request.Form[PaymentGmo.PARAM_MD];

			var isTds2 = (string.IsNullOrEmpty(card3DSecureTranId) || string.IsNullOrEmpty(card3DSecureAuthResult));

			this.TransactionName += isTds2 ? "(3Dセキュア2.0認証)" : "(3Dセキュア1.0認証)";
			var paymentGmoCredit = new PaymentGmoCredit();
			var result = isTds2
				? paymentGmoCredit.SecureTran2((string)this.PaymentOrder[Constants.FIELD_ORDER_CARD_TRAN_ID])
				: paymentGmoCredit.SecureTran(this.OrderId, card3DSecureAuthResult, card3DSecureTranId);

			// 認証結果送信
			if (result)
			{
				if (this.PaymentCart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
				{
					// 会員登録＆カード登録
					if (paymentGmoCredit.SaveMemberAndTraedCard(
						this.PaymentCart.Payment.UserCreditCard.CooperationInfo.GMOMemberId,
						this.PaymentCart.Owner.Name,
						(string)this.PaymentOrder[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
						this.PaymentCart.Payment.CreditAuthorName))
					{
						// カード登録連番を連携IDに格納
						if (this.PaymentCart.Payment.UserCreditCard.UpdateCooperationId(
							this.PaymentCart.Payment.UserCreditCard.CooperationInfo.GMOMemberId,
							Constants.FLG_LASTCHANGED_USER,
							UpdateHistoryAction.DoNotInsert) == false)
						{
							FileLogger.WriteError(
								string.Format(
									"クレジット情報の登録(連携ID更新に失敗: UserId = {0} BranchNo = {1}",
									this.PaymentCart.CartUserId,
									this.PaymentCart.Payment.UserCreditCard.BranchNo));
						}
						else if (this.PaymentCart.Payment.CreditCardBranchNo == CartPayment.FLG_ORDERPAYMENT_CREDITCARD_BRANCH_NEW)
						{
							// カード登録を選択した場合、表示フラグを立つ
							this.PaymentCart.Payment.UserCreditCard.UpdateDispFlg(
								this.PaymentCart.Payment.UserCreditCardRegistFlg,
								Constants.FLG_LASTCHANGED_USER,
								UpdateHistoryAction.DoNotInsert);
						}
					}
					else
					{
						// 登録だけ失敗した場合はカードレコードだけ削除しておく（決済自体は成功させる）
						this.PaymentCart.Payment.UserCreditCard.Delete(UpdateHistoryAction.DoNotInsert);
					}
				}

				// 入金ステータス格納
				this.PaymentOrder.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, successPaymentStatus);
			}
			else
			{
				// 失敗時処理（仮注文ロールバック）
				SetTokenExpired();

				this.ErrorMessages.Add(
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
				this.ErrorMessages.Add(paymentGmoCredit.ErrorMessages);
				FileLogger.WriteError(
					OrderCommon.CreateOrderFailedLogMessage(
						this.TransactionName,
						this.PaymentOrder,
						this.PaymentCart,
						string.Join("\r\n", this.ErrorMessages)));

				// エラーメッセージ追記
				this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));

				// 情報ログ出力
				var logMessage = OrderCommon.CreateOrderFailedLogMessage(
					this.TransactionName,
					this.PaymentOrder,
					this.PaymentCart);
				FileLogger.WriteInfo(logMessage);

				// 仮注文情報削除処理(ゲスト削除、ポイント戻しも行う)
				this.TransactionName = "2-X.注文ロールバック処理";
				// ゲスト購入かつ成功注文なしのときゲスト削除
				var lbDeleteGuest = (this.IsLoggedIn == false)
					&& (this.OrderRegister.SuccessOrders.Count == 0);

				OrderCommon.RollbackPreOrder(
					this.PaymentOrder,
					this.PaymentCart,
					lbDeleteGuest,
					(int)this.PaymentOrder[Constants.FIELD_USERSHIPPING_SHIPPING_NO],
					this.IsLoggedIn,
					UpdateHistoryAction.Insert);
			}

			return result;
		}
		catch (Exception ex)
		{
			SetTokenExpired();

			// エラーメッセージ追記
			this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION));

			// ログ出力
			FileLogger.WriteError(
				OrderCommon.CreateOrderFailedLogMessage(
					this.TransactionName,
					this.PaymentOrder,
					this.PaymentCart),
				ex);

			return false;
		}
	}

	/// <summary>
	/// 後処理実行
	/// </summary>
	private void ExecAfterProcesses()
	{
		if (this.IsOrderCombined)
		{
			SuccessCombinedOrderAdditionProcess(
				this.PaymentOrder,
				this.DispErrorMessages,
				this.PaymentCart.CartUserId,
				this.PaymentCart.Coupon);
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

		// 注文完了画面用に注文IDを格納
		this.OrderRegister.SuccessOrders.Add(this.PaymentOrder);

		// GoogleAnalyticsタグ制御用注文IDをクッキーにセット
		CookieManager.SetCookie(
			(Constants.COOKIE_KEY_GOOGLEANALYTICS_ORDER_ID + (string)this.PaymentOrder[Constants.FIELD_ORDER_ORDER_ID]),
			"",
			Constants.PATH_ROOT,
			DateTime.Now.AddHours(1));
	}

	/// <summary>
	/// トークンを期限切れにする
	/// </summary>
	private void SetTokenExpired()
	{
		// 発行されたトークンキーは複数回利用は出来ない仕様のため、一度失敗したら有効期限切れとする
		if (this.PaymentCart.Payment.CreditToken != null)
		{
			this.PaymentCart.Payment.CreditToken.SetTokneExpired();
		}
	}

	/// <summary>
	/// 注文決済画面へ遷移
	/// </summary>
	private void GoToOrderSettlement()
	{
		// 3Dセキュア決済注文リストから削除
		if (this.PaymentOrder != null)
		{
			this.OrderRegister.GmoCard3DSecurePaymentOrders.Remove(this.PaymentOrder);
		}

		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		this.Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;

		// Chromeの場合、GMOから戻ってきたときにAuthKeyが再発行され、RestoreSessionされたAuthKeyと異なるので、ここで再発行する。
		this.Context.Session[Constants.SESSION_KEY_AUTH_KEY_FOR_SECURE_SESSION] = null;
		SessionSecurityManager.PublishAuthKeyIfUnpublished(this.Context);

		// 画面遷移
		this.Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
	}

	#region プロパティ
	/// <summary>注文ID</summary>
	private string OrderId { get; set; }
	/// <summary>注文登録情報</summary>
	private OrderRegisterFront OrderRegister { get; set; }
	/// <summary>決済処理中の注文情報</summary>
	private Hashtable PaymentOrder { get; set; }
	/// <summary>決済処理中のカート情報</summary>
	private CartObject PaymentCart { get; set; }
	#endregion
}
