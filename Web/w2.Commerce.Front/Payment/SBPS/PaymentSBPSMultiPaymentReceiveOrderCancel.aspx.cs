/*
=========================================================================================================
  Module      : SBPS マルチ決済 キャンセルページ処理(PaymentSBPSMultiPaymentReceiveOrderCancel.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using w2.Domain.UpdateHistory.Helper;

public partial class Payment_SBPS_PaymentSBPSMultiPaymentReceiveOrderCancel : OrderCartPageExternalPayment
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var orderId = Request[Constants.REQUEST_KEY_ORDER_ID];

		// セッション復元＆注文ロールバック
		var order = RestoreSessionAndRollbackOrder(orderId, UpdateHistoryAction.Insert);
		// 注文情報復元できなければエラーページへ
		if (order == null) RedirectToNextPage(order, WebMessages.GetMessages(WebMessages.ERRMSG_FRONT_CARDAUTH_ERROR));

		// 注文リストから削除＆キャンセルリストに追加
		this.SBPSMultiPaymentOrders.RemoveAll(o => o.Key == orderId);
		this.CancelOrders.Add(order);

		// 注文ID保持用セッションを削除
		this.Session.Remove(Constants.SESSION_KEY_ORDER_ID_CHECK_FOR_LINK_TYPE_PAYMENT);

		// 次の画面へ遷移
		RedirectToNextPage(order);
	}
}