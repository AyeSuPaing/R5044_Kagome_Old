/*
=========================================================================================================
  Module      : Payment SBPS PayPay Receive (PaymentSBPSPayPayReceive.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using SessionWrapper;
using System;
using System.Collections.Generic;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.Paypay;
using w2.Common.Web;
using w2.Domain.Order.Helper;

public partial class Payment_SBPS_PaymentSBPSPayPayReceive : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// Check order id from request
		if (string.IsNullOrEmpty(this.RequestOrderId))
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_IRREGULAR_PARAMETER_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR);
		}

		GetOrderPropertiesFromSession();

		if (!IsPostBack)
		{
			// 画面遷移の正当性チェック
			CheckOrderUrlSession();

			var orderNew = SBPSSessionWrapper.FindSBPSMultiPendingOrder(this.RequestOrderId);

			// 注文完了へ
			// 画面遷移決済なし？
			if (this.SBPSMultiPaymentOrders.Count == 0)
			{
				// 正常終了したカート取得、カートオブジェクト削除（ＤＢ削除済み）
				var successCart = new List<CartObject>();
				foreach (var order in this.SuccessOrders)
				{
					foreach (CartObject cartObject in this.CartList)
					{
						if ((string)order[Constants.FIELD_ORDER_ORDER_ID] == cartObject.OrderId)
						{
							successCart.Add(cartObject);
							this.CartList.DeleteCartVurtual(cartObject);
							break;
						}
					}
				}

				// エラーあり、未完了カートが２以上の場合、アラート追加
				if ((this.ErrorMessages.Count > 0)
					&& (this.CartList.Items.Count > 1))
				{
					this.ErrorMessages.Add(WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_ORDER_UNSETTLED_CART_ALERT));
				}

				// １件も成功しなかった場合はエラー画面へ
				if (this.SuccessOrders.Count == 0)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = string.Join(Environment.NewLine, this.DispErrorMessages.ToArray());

					var errorPageUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ERROR)
						.AddParam(
							Constants.REQUEST_KEY_BACK_URL,
							Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
						.CreateUrl();
					Response.Redirect(errorPageUrl);
				}

				// 実際の更新はマイページ側
				orderNew.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED;
				orderNew.ExternalPaymentAuthDate = DateTime.Now;
				var paymentMemo = OrderCommon.CreateOrderPaymentMemo(
					orderNew.PaymentOrderId,
					orderNew.OrderPaymentKbn,
					orderNew.CardTranId,
					"結果受信",
					orderNew.OrderPriceTotal);
				orderNew.PaymentMemo = OrderExternalPaymentUtility.SetExternalPaymentMemo(
					StringUtility.ToEmpty(orderNew.PaymentMemo),
					paymentMemo);
				orderNew.LastChanged = Constants.FLG_LASTCHANGED_USER;

				SBPSSessionWrapper.AddSbpsMultiPendingOrder(orderNew);
			}

			// 画面遷移の正当性チェックのため遷移先ページURLを設定
			Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL;

			// Go to screen order history detail
			RedirectToOrderHistoryPage();
		}
	}

	/// <summary>
	/// Redirect to order history page
	/// </summary>
	private void RedirectToOrderHistoryPage()
	{
		// 画面遷移の正当性チェックのため遷移先ページURLを設定
		Session[Constants.SESSION_KEY_NEXT_PAGE_FOR_CHECK] = Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL;

		// Go to screen order history detail
		var nextUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_FRONT_ORDER_HISTORY_DETAIL)
			.AddParam(Constants.REQUEST_KEY_ORDER_ID, this.RequestOrderId)
			.AddParam(
				PaypayConstants.REQUEST_KEY_RECEIVE_RESULT,
				((this.DispErrorMessages != null) && (this.DispErrorMessages.Count != 0))
					? PaypayConstants.FLG_SBPS_EXECUTE_RESULT_NG
					: PaypayConstants.FLG_SBPS_EXECUTE_RESULT_OK)
			.CreateUrl();
		Response.Redirect(nextUrl);
	}

	/// <summary>Request order id</summary>
	private string RequestOrderId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ORDER_ID]); }
	}
}
