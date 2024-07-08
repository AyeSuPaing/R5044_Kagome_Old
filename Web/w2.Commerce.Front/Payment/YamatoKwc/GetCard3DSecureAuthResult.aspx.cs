/*
=========================================================================================================
  Module      : Yamato KWC3Dセキュア認証結果取得ページ処理(GetCard3DSecureAuthResult.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.YamatoKwc;
using w2.Common.Logger;
using w2.Common.Web;
using w2.Domain.UpdateHistory.Helper;

public partial class Payment_YamatoKwc_GetCard3DSecureAuthResult : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		try
		{
			this.TransactionName = "2-1-I.ヤマトKWCクレジット決済処理(3Dセキュア認証)";

			var sessionParam = (Hashtable)this.Session[Constants.SESSION_KEY_PARAM];
			this.DispErrorMessages = (List<string>)sessionParam[Constants.SESSION_KEY_ERROR];

			this.OrderRegister = new OrderRegisterFront(this.IsLoggedIn)
			{
				SuccessOrders = (List<Hashtable>)sessionParam[Constants.SESSION_KEY_PARAM_ORDER],
				YamatoCard3DSecurePaymentOrders =
					(List<Hashtable>)sessionParam[Constants.SESSION_KEY_PAYMENT_CREDIT_YAMATOKWC_ORDER_3DSECURE],
			};

			var landingCartSessionKey = (string)this.Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
			this.CartList = (landingCartSessionKey == null)
				? GetCartObjectList()
				: (CartObjectList)this.Session[landingCartSessionKey];

			// セッションから対象の注文IDを取得
			this.OrderId = Session[Constants.FIELD_ORDER_ORDER_ID].ToString();

			// 決済処理中の注文情報、カート情報を取得
			this.PaymentOrder = this.OrderRegister.YamatoCard3DSecurePaymentOrders.Find(
				paymentOrder => ((string)paymentOrder[Constants.FIELD_ORDER_ORDER_ID] == this.OrderId));

			this.PaymentCart = this.CartList.Items.Find(cart => (cart.OrderId == this.OrderId));

			this.CreditAuthResponse = new PaymentYamatoKwcCredit3DSecureAuthApi().Exec(
				this.Request.Form,
				this.PaymentOrder[Constants.FIELD_ORDER_PAYMENT_ORDER_ID].ToString(),
				this.PaymentOrder[Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_KEY].ToString());

			if (this.CreditAuthResponse.ReturnCode.Equals("0"))
			{
				var paymentStatusCompleteFlg =
					(this.PaymentCart.HasDigitalContents && Constants.DIGITAL_CONTENTS_OPTION_ENABLED)
						? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS
						: Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;
				var successPaymentStatus = paymentStatusCompleteFlg
					? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
					: Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM;

				// 入金ステータス格納
				this.PaymentOrder[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS] = successPaymentStatus;

				// 決済カード取引ID格納
				this.PaymentOrder[Constants.FIELD_ORDER_CARD_TRAN_ID] = this.CreditAuthResponse.CrdCResCd;

				// 注文確定処理
				var success = this.OrderRegister.UpdateForOrderComplete(
					this.PaymentOrder,
					this.PaymentCart,
					true,
					UpdateHistoryAction.DoNotInsert);

				// 後処理
				if (success)
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
						Constants.COOKIE_KEY_GOOGLEANALYTICS_ORDER_ID + (string)this.PaymentOrder[Constants.FIELD_ORDER_ORDER_ID],
						string.Empty,
						Constants.PATH_ROOT,
						DateTime.Now.AddHours(1));
				}
			}
			// 失敗時処理（仮注文ロールバック）
			else
			{
				SetTokenExpired();
				// エラーメッセージ追記
				this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
				this.ErrorMessages.Add(CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_CARDAUTH_ERROR));

				// 情報ログ出力
				FileLogger.WriteInfo(OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, this.PaymentOrder, this.PaymentCart));

				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.Yamatokwc,
					PaymentFileLogger.PaymentProcessingType.CreditPaymentWithThreeDSecure,
					this.CreditAuthResponse.ErrorMessage,
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, this.OrderId},
						{Constants.FIELD_ORDER_USER_ID, this.CartList.UserId}
					});

				// 仮注文情報削除処理(ゲスト削除、ポイント戻しも行う)
				this.TransactionName = "2-X.注文ロールバック処理";

				// ゲスト購入かつ成功注文なしのときゲスト削除
				var lbDeleteGuest = ((this.IsLoggedIn == false) && (this.OrderRegister.SuccessOrders.Count == 0));
				OrderCommon.RollbackPreOrder(
					this.PaymentOrder,
					this.PaymentCart,
					lbDeleteGuest,
					(int)this.PaymentOrder[Constants.FIELD_USERSHIPPING_SHIPPING_NO],
					this.IsLoggedIn,
					UpdateHistoryAction.Insert);
			}
		}
		catch (Exception ex)
		{
			SetTokenExpired();

			this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION));

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				false,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Yamatokwc,
				PaymentFileLogger.PaymentProcessingType.CreditPaymentWithThreeDSecure,
				this.CreditAuthResponse.ErrorMessage,
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, this.OrderId},
					{Constants.FIELD_ORDER_USER_ID, this.CartList.UserId}
				});

			FileLogger.WriteError(ex);
		}

		if (this.CreditAuthResponse.ReturnCode.Equals("0"))
		{
			GoToOrderSettlementPage();
		}
		else
		{
			this.Session[Constants.SESSION_KEY_ERROR_MSG] = 
				(this.CreditAuthResponse.ErrorCode == "A092000005") 
					? WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_THREEDSECURE_UNSUPPORTED)
					: WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);

			// 複数カート時にエラー画面に遷移させるとカートに不整合が出る可能性があるため注文決済画面に遷移させる
			if ((this.CartList != null) && (this.CartList.Items.Count > 1))
			{
				GoToOrderSettlementPage();
			}
			else
			{
				var url = new UrlCreator(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
					.AddParam(Constants.REQUEST_KEY_ERRORPAGE_KBN, Constants.KBN_REQUEST_ERRORPAGE_GOTOP)
					.CreateUrl();

				// エラーページへ遷移
				Response.Redirect(url);
			}
		}
	}

	/// <summary>
	/// トークンを期限切れにする
	/// </summary>
	private void SetTokenExpired()
	{
		// トークンは決済依頼を行った時点でヤマト側で無効化されるためトークンを期限切れにする
		if ((this.PaymentCart != null)
			&& (this.PaymentCart.Payment != null)
			&& (this.PaymentCart.Payment.CreditToken != null))
		{
			this.PaymentCart.Payment.CreditToken.SetTokneExpired();
		}
	}

	/// <summary>
	/// 注文決済画面に遷移
	/// </summary>
	private void GoToOrderSettlementPage()
	{
		// ヤマトKWC3Dセキュア注文リストから削除
		if (this.PaymentOrder != null)
		{
			this.OrderRegister.YamatoCard3DSecurePaymentOrders.Remove(this.PaymentOrder);
		}

		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;

		// 決済画面へ遷移
		Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
	}

	/// <summary>クレジット与信レスポンス 結果（true=0, false=1）</summary>
	private PaymentYamatoKwcCreditAuthResponseData CreditAuthResponse { get; set; }
	/// <summary>注文ID</summary>
	private string OrderId { get; set; }
	/// <summary>注文登録情報</summary>
	private OrderRegisterFront OrderRegister { get; set; }
	/// <summary>決済処理中の注文情報</summary>
	private Hashtable PaymentOrder { get; set; }
	/// <summary>決済処理中のカート情報</summary>
	private CartObject PaymentCart { get; set; }
}
