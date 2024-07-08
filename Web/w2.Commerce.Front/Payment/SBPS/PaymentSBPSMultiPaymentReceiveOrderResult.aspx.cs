/*
=========================================================================================================
  Module      : SBPS マルチ決済 購入結果ページ処理(PaymentSBPSMultiPaymentReceiveOrderResult.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;

public partial class Payment_SBPS_PaymentSBPSMultiPaymentReceiveOrderResult : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// 結果受け取り
		//------------------------------------------------------
		PaymentSBPSMultiPaymentReceiverOrderResult receiveOrderResult = new PaymentSBPSMultiPaymentReceiverOrderResult(Request.Form);
		var actionResult = receiveOrderResult.Action();

		PaymentFileLogger.WritePaymentLog(
			actionResult,
			"",
			PaymentFileLogger.PaymentType.Sbps,
			PaymentFileLogger.PaymentProcessingType.Unknown,
			"マルチ決済 購入結果ページ処理");

		if (actionResult == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SBPS_PAYMENT_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		//------------------------------------------------------
		// 注文処理の続きを実行
		//------------------------------------------------------
		bool successFlag = true;
		Hashtable order = null;
		CartObject cart = null;

		string orderId = Request[Constants.REQUEST_KEY_ORDER_ID];

		OrderRegisterFront register = new OrderRegisterFront(this.IsLoggedIn);
		try
		{
			//------------------------------------------------------
			// ファイルからセッション復元
			//------------------------------------------------------
			successFlag = RestoreSession(orderId);

			//------------------------------------------------------
			// セッションから各種情報復元
			//------------------------------------------------------
			if (successFlag)
			{
				// プロパティセット
				GetOrderPropertiesFromSession();

				// セッションからパラメータ取得
				var landingCartSessionKey = (string)Session[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
				this.CartList = (landingCartSessionKey == null)
					? GetCartObjectList()
					: (CartObjectList)Session[landingCartSessionKey];
				if ((this.CartList.Items.Count == 0)
					&& (Session[Constants.SESSION_KEY_CART_LIST_FOR_PAYPAY] != null))
				{
					this.CartList = (CartObjectList)Session[Constants.SESSION_KEY_CART_LIST_FOR_PAYPAY];
				}
				Hashtable param = (Hashtable)Session[Constants.SESSION_KEY_PARAM];
				this.SuccessOrders = (List<Hashtable>)param["order"];
				this.SBPSMultiPaymentOrders = (List<KeyValuePair<string, Hashtable>>)param["order_sbps_multi"];
				register.GoogleAnalyticsParams = (List<Hashtable>)param["googleanaytics_params"];
				this.DispErrorMessages = (List<string>)param["error"];

				//------------------------------------------------------
				// 決済処理中の注文情報、カート情報を取得
				//------------------------------------------------------
				order = this.SBPSMultiPaymentOrders.Find(o =>
					(string)o.Key == orderId).Value;

				cart = this.CartList.Items.Find(c =>
					c.OrderId == orderId);

				if ((order == null) || (cart == null))
				{
					throw new Exception("セッション情報が復元できませんでした。order:" + order.ToString() + " cart:" + cart.ToString());
				}

				// 決済注文IDチェック
				if (receiveOrderResult.PaymentOrderId != (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID])
				{
					throw new Exception("決済注文ID不一致");
				}
			}
		}
		// 例外時
		catch (Exception ex)
		{
			// 失敗
			successFlag = false;
			// ログ出力
			AppLogger.WriteError("セッション情報復元エラー：注文ID=" + orderId, ex);
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_AUTH_EXCEPTION);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR + "?" + Constants.REQUEST_KEY_ERRORPAGE_KBN + "=" + HttpUtility.UrlEncode(Constants.KBN_REQUEST_ERRORPAGE_GOTOP));
		}

		//------------------------------------------------------
		// ４．注文後処理
		//------------------------------------------------------
		if (successFlag)
		{
			// 注文完了画面用に注文IDを格納
			this.SuccessOrders.Add(order);
		}

		// SBPSマルチ決済決済注文リストから削除
		if (order != null)
		{
			this.SBPSMultiPaymentOrders.RemoveAll(o => o.Key == orderId);
		}

		//------------------------------------------------------
		// 次の画面へ
		//------------------------------------------------------
		RedirectToNextPage(order);
	}
}