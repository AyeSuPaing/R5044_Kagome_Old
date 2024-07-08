/*
=========================================================================================================
  Module      : SBPS マルチ決済 エラーページ処理(PaymentSBPSMultiPaymentReceiveOrderError.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;

/// <summary>
/// SBPSエラーページ（SBPSサーバーエラー、または こちらがOrderNoticeでNGを返却したときにここへ来る）
/// </summary>
public partial class Payment_SBPS_PaymentSBPSMultiPaymentReceiveOrderError : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// 結果受け取り（エラー画面の場合はチェックサムが送られてこないのでValidateしない）
		var receiveOrderError = new PaymentSBPSMultiPaymentReceiverOrderError(Request.Form);
		string orderId = Request[Constants.REQUEST_KEY_ORDER_ID];

		// セッション復元＆注文ロールバック
		var order = RestoreSessionAndRollbackOrder(orderId, UpdateHistoryAction.Insert);

		PaymentFileLogger.WritePaymentLog(
			false,
			"",
			PaymentFileLogger.PaymentType.Sbps,
			PaymentFileLogger.PaymentProcessingType.Unknown,
			"マルチ決済エラーページ処理 エラーコード：" + receiveOrderError.ErrorCode);
		// 注文情報復元できなければエラーページへ
		if (order == null) RedirectForOtherError(null);

		// 注文リストから削除・キャンセルならキャンセルリストに追加
		this.SBPSMultiPaymentOrders.RemoveAll(o => o.Key == orderId);
		if (receiveOrderError.ErrorCodeType == PaymentSBPSMultiPaymentReceiverOrderError.ErroCodeTypes.CardCancel)
		{
			this.CancelOrders.Add(order);
		}

		// エラー別処理／画面遷移
		RedirectToErrorPage(receiveOrderError, order);
	}

	/// <summary>
	/// エラー画面へ遷移
	/// </summary>
	/// <param name="receiveOrderError"></param>
	/// <param name="order"></param>
	private void RedirectToErrorPage(PaymentSBPSMultiPaymentReceiverOrderError receiveOrderError, Hashtable order)
	{
		switch (receiveOrderError.ErrorCodeType)
		{
			// カードNG？
			case PaymentSBPSMultiPaymentReceiverOrderError.ErroCodeTypes.CardNG:
				RedirectForCardNG(order);
				break;

			// キャンセル？
			case PaymentSBPSMultiPaymentReceiverOrderError.ErroCodeTypes.CardCancel:
				RedirectToNextPage(order);
				break;

			// その他のエラー？（一意制約違反などもここに来る）
			default:
				RedirectForOtherError(order);
				break;
		}
	}

	/// <summary>
	/// カードNG向け画面遷移
	/// </summary>
	/// <param name="order">注文情報</param>
	private void RedirectForCardNG(Hashtable order)
	{
		RedirectToNextPage(order, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));
	}

	/// <summary>
	/// その他エラー向け画面遷移
	/// </summary>
	/// <param name="order">注文情報</param>
	private void RedirectForOtherError(Hashtable order)
	{
		RedirectToNextPage(order, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_SBPS_PAYMENT_ERROR));
	}
}