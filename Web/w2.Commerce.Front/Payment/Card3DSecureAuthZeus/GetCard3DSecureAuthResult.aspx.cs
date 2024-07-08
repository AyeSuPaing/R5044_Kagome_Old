/*
=========================================================================================================
  Module      : ゼウス3Dセキュア認証結果取得ページ処理(GetCard3DSecureAuthResult.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Zeus;
using w2.Common.Logger;
using w2.Domain.Order;

public partial class Payment_Card3DSecureAuthZeus_GetCard3DSecureAuthResult : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// セキュア2.0対応、本人認証結果を取得するため
		var secureAuthResult = JsonConvert.DeserializeObject<dynamic>(GetJsonData());

		//------------------------------------------------------
		// 同一ページからのリダイレクトの場合に処理を実行
		//------------------------------------------------------
		if ((Request.Form["exec"] != null)
			|| (Constants.PAYMENT_SETTING_ZEUS_3DSECURE2 && (secureAuthResult != null)))
		{
			bool blSuccess = true;
			Hashtable htOrder = null;
			CartObject coCart = null;

			OrderRegisterFront register = new OrderRegisterFront(this.IsLoggedIn);

			//------------------------------------------------------
			// パラメータ取得
			//------------------------------------------------------
			string strCard3DSecureAuthResult = Request.Form["PaRes"];
			string strCard3DSecureTranID = Request.Form["MD"];
			string strOrderId = Request.Form[Constants.REQUEST_KEY_ORDER_ID];

			if (Constants.PAYMENT_SETTING_ZEUS_3DSECURE2 && (secureAuthResult != null))
			{
				strCard3DSecureAuthResult = secureAuthResult.PaRes;
				strCard3DSecureTranID = secureAuthResult.MD;
				strOrderId = Request.QueryString.Get(Constants.REQUEST_KEY_ORDER_ID);
			}

			PaymentFileLogger.WritePaymentLog(
				null,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Zeus,
				PaymentFileLogger.PaymentProcessingType.Zeus3DSecureAuthResultNotification,
				LogCreator.CreateMessage(strOrderId, ""));

			try
			{
				//------------------------------------------------------
				// ファイルからセッション復元
				//------------------------------------------------------
				RestoreSession(strOrderId);

				if (blSuccess)
				{
					// 画面遷移の正当性チェック
					CheckOrderUrlSession();

					// セッションからパラメータ取得
					string strLandingCartSessionKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
					this.CartList = (strLandingCartSessionKey == null) ? GetCartObjectList() : (CartObjectList)Session[strLandingCartSessionKey];
					Hashtable htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
					this.DispErrorMessages = (List<string>)htParam["error"];

					register.SuccessOrders = (List<Hashtable>)htParam["order"];
					register.ZeusCard3DSecurePaymentOrders = (List<Hashtable>)htParam["zeus_order_3dsecure"];
					register.GoogleAnalyticsParams = (List<Hashtable>)htParam["googleanaytics_params"];

					//------------------------------------------------------
					// 決済処理中の注文情報、カート情報を取得
					//------------------------------------------------------
					htOrder = register.ZeusCard3DSecurePaymentOrders.Find(o =>
						(string)o[Constants.FIELD_ORDER_ORDER_ID] == Request[Constants.REQUEST_KEY_ORDER_ID]);

					coCart = this.CartList.Items.Find(c =>
						c.OrderId == Request[Constants.REQUEST_KEY_ORDER_ID]);

					if ((htOrder == null) || (coCart == null))
					{
						blSuccess = false;
					}
				}
			}
			// 例外時
			catch (Exception ex)
			{
				// 失敗
				blSuccess = false;
				AppLogger.WriteError(ex);
			}

			if (blSuccess)
			{
				// カート内容と注文内容の金額を比較
				var orderModel = new OrderService().Get(strOrderId);
				if (coCart.PriceTotal != orderModel.OrderPriceTotal)
				{
					RedirectToNextPage(null, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CART_CHANGED), Constants.PAGE_FRONT_CART_LIST);
				}
			}

			// ここまででエラーがあればエラーページへ（仮注文は残す）
			if (blSuccess == false)
			{
				AppLogger.WriteError("セッション情報復元エラー：注文ID=" + strOrderId);
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR + "?" + Constants.REQUEST_KEY_ERRORPAGE_KBN + "=" + HttpUtility.UrlEncode(Constants.KBN_REQUEST_ERRORPAGE_GOTOP));
			}

			try
			{
				//------------------------------------------------------
				// ２．外部連携決済処理
				// ２－１－Ｂ．カード・ゼウス決算処理（つづき）
				//------------------------------------------------------
				// ZEUSクレジット決済方法設定を取得
				Constants.PaymentCreditCardPaymentMethod? paymentMethod = (Constants.DIGITAL_CONTENTS_OPTION_ENABLED && coCart.HasDigitalContents) ? Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD_FORDIGITALCONTENTS : Constants.PAYMENT_SETTING_ZEUS_PAYMENTMETHOD;

				bool paymentStatusCompleteFlg = (coCart.HasDigitalContents && Constants.DIGITAL_CONTENTS_OPTION_ENABLED) ? Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE_FORDIGITALCONTENTS : Constants.PAYMENT_CARD_PATMENT_STAUS_COMPLETE;
				var pzcPaymentZeusCredit = new ZeusCreditFor3DSecure();

				this.TransactionName = "2-1-B.ゼウスクレジット決済処理(3Dセキュア認証)";

				// 認証結果送信
				if (pzcPaymentZeusCredit.Check3DSecureAuthResult(strOrderId, strCard3DSecureAuthResult, strCard3DSecureTranID))
				{
					// 認証成功、オーソリ処理
					if (pzcPaymentZeusCredit.Auth(strCard3DSecureTranID))
					{
						// ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							true,
							Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
							PaymentFileLogger.PaymentType.Zeus,
							PaymentFileLogger.PaymentProcessingType.CreditPaymentWithThreeDSecure,
							"認証成功、オーソリ成功",
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, strOrderId},
								{Constants.FIELD_ORDER_USER_ID, this.CartList.UserId}
							});

						// オーソリ成功
						blSuccess = true;

						// 入金ステータス格納
						htOrder.Add(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS, paymentStatusCompleteFlg ? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE : Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM);

						// 決済カード取引IDへZeusオーダーID格納
						htOrder.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, pzcPaymentZeusCredit.ZeusOrderId);
						if (paymentMethod == Constants.PaymentCreditCardPaymentMethod.PAYMENT_AFTER_AUTH)
						{
							htOrder[Constants.FIELD_ORDER_CARD_TRAN_ID] = "";
						}
					}
					else
					{
						// オーソリ失敗
						blSuccess = false;

						// ログ格納処理
						PaymentFileLogger.WritePaymentLog(
							false,
							Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
							PaymentFileLogger.PaymentType.Zeus,
							PaymentFileLogger.PaymentProcessingType.CreditPaymentWithThreeDSecure,
							pzcPaymentZeusCredit.ErrorMessage,
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, strOrderId},
								{Constants.FIELD_ORDER_USER_ID, this.CartList.UserId}
							});
					}
				}
				else
				{
					// 認証失敗
					blSuccess = false;

					// ログ格納処理
					PaymentFileLogger.WritePaymentLog(
						false,
						Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
						PaymentFileLogger.PaymentType.Zeus,
						PaymentFileLogger.PaymentProcessingType.CreditPaymentWithThreeDSecure,
						pzcPaymentZeusCredit.ErrorMessage,
						new Dictionary<string, string>
						{
							{Constants.FIELD_ORDER_ORDER_ID, strOrderId},
							{Constants.FIELD_ORDER_USER_ID, this.CartList.UserId}
						});
				}

				//------------------------------------------------------
				// ２－Ｘ．失敗時処理（仮注文ロールバック）
				//		・失敗があれば処理をロールバック（＝仮注文のこす）し、例外をとばす
				//------------------------------------------------------
				if (blSuccess == false)
				{
					// エラーメッセージ追記
					this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));

					// 情報ログ出力
					string strLogMessage = OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, htOrder, coCart);

					// ログ格納処理
					PaymentFileLogger.WritePaymentLog(
						false,
						Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
						PaymentFileLogger.PaymentType.Zeus,
						PaymentFileLogger.PaymentProcessingType.CreditPaymentWithThreeDSecure,
						strLogMessage,
						new Dictionary<string, string>
						{
							{Constants.FIELD_ORDER_ORDER_ID, strOrderId},
							{Constants.FIELD_ORDER_USER_ID, this.CartList.UserId}
						});

					// 仮注文情報削除処理(ゲスト削除、ポイント戻しも行う)
					this.TransactionName = "2-X.注文ロールバック処理";

					// ロールバック対象以外に注文がない注文時会員登録ユーザーを削除
					var isUserDelete = (((this.IsLoggedIn == false) || (this.RegisterUser != null))
						&& (this.SuccessOrders.Count == 0)
						&& (new OrderService().GetOrdersByUserId(coCart.OrderUserId).Length == 1));

					OrderCommon.RollbackPreOrder(
						htOrder,
						coCart,
						isUserDelete,
						(int)htOrder[Constants.FIELD_USERSHIPPING_SHIPPING_NO],
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
				blSuccess = false;

				// 決済自体が完了してしまっていることも考えられるので仮注文データを「のこす」
				// また、ロールバック処理でエラーの場合（仮注文を戻せなかった）も
				// データの不整合を防ぐために仮注文データを「のこす」

				// エラーメッセージ追記
				this.DispErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION));

				// ログ出力
				string strLogMessage = OrderCommon.CreateOrderFailedLogMessage(this.TransactionName, htOrder, coCart);

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
				//------------------------------------------------------
				// ３－１．注文ステータス更新
				//------------------------------------------------------
				blSuccess = register.UpdateForOrderComplete(htOrder, coCart, true, UpdateHistoryAction.DoNotInsert);
			}

			//------------------------------------------------------
			// ４．後処理
			//------------------------------------------------------
			if (blSuccess)
			{
				// 注文完了時処理
				var isSendMail = new OrderService().GetOrdersByUserId(coCart.OrderUserId)
					.Where(userOrder => ((userOrder.OrderId != Request[Constants.REQUEST_KEY_ORDER_ID]) && (userOrder.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP)))
					.Any(userOrder => this.CartList.Items.Select(item => item.OrderId).Contains(userOrder.OrderId));
				if (isSendMail) htOrder[Constants.ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE] = null;
				register.OrderCompleteProcesses(htOrder, coCart, UpdateHistoryAction.DoNotInsert);

				// 注文完了後処理
				register.AfterOrderCompleteProcesses(htOrder, coCart, UpdateHistoryAction.Insert);

				// 注文完了画面用に注文IDを格納
				register.SuccessOrders.Add(htOrder);
			}

			// 3Dセキュア決済注文リストから削除
			if (htOrder != null)
			{
				register.ZeusCard3DSecurePaymentOrders.Remove(htOrder);
			}

			//------------------------------------------------------
			// 決済画面へ
			//------------------------------------------------------
			// 画面遷移の正当性チェックのため遷移先ページURLを設定
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_SETTLEMENT;

			// jsの処理による場合は画面遷移しない、ブラウザによる処理の場合のみ遷移する
			// jsの処理の場合はsecureAuthResult(セキュア2.0本人認証結果)に値が入る
			if (secureAuthResult == null) Response.Redirect(this.SecurePageProtocolAndHost + Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_SETTLEMENT);
		}
	}

	/// <summary>
	/// Json形式のデータを取得する
	/// </summary>
	/// <returns>Json形式データ</returns>
	private string GetJsonData()
	{
		// 同一ページからのリダイレクトの場合は早期リターン
		if (Request.Form["exec"] != null) return string.Empty;

		var result = string.Empty;
		using (var reader = new StreamReader(Request.InputStream))
		{
			result = reader.ReadToEnd();
		}
		return result;
	}
}
