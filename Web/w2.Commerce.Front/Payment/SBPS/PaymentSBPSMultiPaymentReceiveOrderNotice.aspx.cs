/*
=========================================================================================================
  Module      : SBPS マルチ決済 購入結果通知受取ページ(PaymentSBPSMultiPaymentReceiveOrderNotice.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.Common.Logger;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Order.Helper;

public partial class Payment_SBPS_PaymentSBPSMultiPaymentReceiveOrderNotice : OrderCartPageExternalPayment
{
	/// <summary>楽天ペイ顧客決済情報 与信結果</summary>
	private const string RAKUTEN_ID_RES_PAYINFO_KEY_AUTHORIZATION_RESULT = "R01";
	/// <summary>楽天ペイ顧客決済情報 金額変更結果</summary>
	private const string RAKUTEN_ID_RES_PAYINFO_KEY_AMOUNTCHANGE_RESULT = "R02";
	/// <summary>楽天ペイ顧客決済情報 売上結果</summary>
	private const string RAKUTEN_ID_RES_PAYINFO_KEY_SALES_RESULT = "R03";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		string response = null;
		string orderId = Request[Constants.REQUEST_KEY_ORDER_ID];
		try
		{
			// 結果解析。OKであれば注文完了処理実行
			PaymentSBPSMultiPaymentReceiverOrderNotice receiveOrderNotice = new PaymentSBPSMultiPaymentReceiverOrderNotice(Request.Form);
			if (receiveOrderNotice.Action())
			{
				// 楽天ペイで与信結果通知以外の場合は注文ステータス更新をスキップする
				var isSkip = ((receiveOrderNotice.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS)
						&& ((receiveOrderNotice.PayinfoKey == RAKUTEN_ID_RES_PAYINFO_KEY_AMOUNTCHANGE_RESULT)
							|| (receiveOrderNotice.PayinfoKey == RAKUTEN_ID_RES_PAYINFO_KEY_SALES_RESULT)));

				// 注文ステータス更新（キャリア決済は基本OKのみ）（更新履歴とともに）
				if ((receiveOrderNotice.ResResult == PaymentSBPSMultiPaymentReceiverOrderNotice.ResResultType.OK) && (isSkip == false))
				{
					ExecOrderComplete(orderId, receiveOrderNotice, UpdateHistoryAction.Insert);
				}

				// スキップした場合、結果をログに出力
				if (isSkip)
				{
					PaymentFileLogger.WritePaymentLog
					(
						null,
						Constants.FLG_PAYMENT_PAYMENT_ID_RAKUTEN_ID_SBPS,
						PaymentFileLogger.PaymentType.Sbps,
						PaymentFileLogger.PaymentProcessingType.SkipUpdate,
						"",
						new Dictionary<string, string>
						{
							{Constants.FIELD_ORDER_ORDER_ID, orderId},
							{Constants.FIELD_PAYMENT_PAYMENT_ID, receiveOrderNotice.PaymentId},
							{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, receiveOrderNotice.PaymentOrderId},
							{"res_result", receiveOrderNotice.ResResult.Value == PaymentSBPSMultiPaymentReceiverBase.ResResultType.OK ? "true" : "false"},
							{"pay_info_key", receiveOrderNotice.PayinfoKey},
							{"response_message", receiveOrderNotice.ResponseMessage}
						}
					);
				}
			}
			response = receiveOrderNotice.ResponseMessage;
		}
		catch (Exception ex)
		{
			PaymentFileLogger.WritePaymentLog(
				false,
				"",
				PaymentFileLogger.PaymentType.Sbps,
				PaymentFileLogger.PaymentProcessingType.ReceivePurchaseResults,
				string.Format("{0}：{1}", Constants.FIELD_ORDER_ORDER_ID, orderId) + BaseLogger.CreateExceptionMessage(ex));
			response = "NG," + ex.Message;
		}

		Response.Clear();
		Response.Write(response);
		Response.End();
	}

	/// <summary>
	/// 注文完了処理実行
	/// </summary>
	/// <param name="orderId">注文ID</param>
	/// <param name="receiveOrderNotice">購入結果通知</param>
	/// <param name="updateHistoryAction">更新履歴アクション</param>
	/// <returns>レスポンスメッセージ</returns>
	private void ExecOrderComplete(
		string orderId,
		PaymentSBPSMultiPaymentReceiverOrderNotice receiveOrderNotice,
		UpdateHistoryAction updateHistoryAction)
	{
		// セッションデータいったん復元（削除しない）
		var sessionData = SessionSecurityManager.RestoreSessionFromDatabaseForGoToOtherSite(orderId, false);

		Hashtable param = (Hashtable)sessionData[Constants.SESSION_KEY_PARAM];
		List<KeyValuePair<string, Hashtable>> softbankMultiPaymentOrders = (List<KeyValuePair<string, Hashtable>>)param["order_sbps_multi"];
		// 決済処理中の注文情報、カート情報を取得
		string landingCartSessionKey = (string)sessionData[Constants.SESSION_KEY_LANDING_CART_SESSION_KEY];
		this.CartList = (landingCartSessionKey == null)
			? ((sessionData.ContainsKey(Constants.SESSION_KEY_ORDERCOMBINE_CART_LIST)
					&& ((CartObjectList)sessionData[Constants.SESSION_KEY_ORDERCOMBINE_CART_LIST] != null))
				? (CartObjectList)sessionData[Constants.SESSION_KEY_ORDERCOMBINE_CART_LIST]
				: (CartObjectList)sessionData[Constants.SESSION_KEY_CART_LIST])
			: (CartObjectList)sessionData[landingCartSessionKey];
		if ((this.CartList.Items.Count == 0)
			&& (sessionData[Constants.SESSION_KEY_CART_LIST_FOR_PAYPAY] != null))
		{
			this.CartList = (CartObjectList)sessionData[Constants.SESSION_KEY_CART_LIST_FOR_PAYPAY];
		}

		Hashtable order = softbankMultiPaymentOrders.First(o => (o.Key == orderId)).Value;
		CartObject cart = this.CartList.Items.First(c => c.OrderId == orderId);

		// 決済注文IDチェック
		if (receiveOrderNotice.PaymentOrderId != (string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID])
		{
			throw new Exception("購入ID不一致");
		}

		// 定期会員フラグ更新
		if (Constants.FIXEDPURCHASE_MEMBER_CONDITION_INCLUDES_ORDER_PAYMENT_STATUS_COMPLETE
			&& Constants.PAYMENT_SETTING_SBPS_PAYMENT_STATUS_COMPLETE
			&& ((cart.IsFixedPurchaseMember == false) && cart.HasFixedPurchase))
		{
			new UserService().UpdateFixedPurchaseMemberFlg(
				(string)order[Constants.FIELD_ORDER_USER_ID],
				Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert);
		}

		// 注文ステータス更新
		var paymentMemo = OrderCommon.CreateOrderPaymentMemoForAuth(
			orderId,
			receiveOrderNotice.PaymentOrderId,
			cart.Payment.PaymentId,
			receiveOrderNotice.TrackingId,
			cart.PriceTotal);
		order[Constants.FIELD_ORDER_PAYMENT_MEMO] = OrderExternalPaymentUtility.SetExternalPaymentMemo(
			StringUtility.ToEmpty(order[Constants.FIELD_ORDER_PAYMENT_MEMO]),
			paymentMemo);
		var iUpdateOrder = UpdateOrderStatus(
			order,
			cart,
			receiveOrderNotice.TrackingId,
			JudgePaymentStatusComplete(cart),
			UpdateHistoryAction.DoNotInsert);

		// 更新できたらその他処理実行
		if (iUpdateOrder != 0)
		{
			var user = new UserService().Get((string)order[Constants.FIELD_USER_USER_ID]);
			bool isUser = UserService.IsUser(user.UserKbn);

			// 注文完了時処理
			OrderRegisterFront register = new OrderRegisterFront(isUser);

			var isSendMail = new OrderService().GetOrdersByUserId(cart.OrderUserId)
				.Where(userOrder => ((userOrder.OrderId != orderId) && (userOrder.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP)))
				.Any(userOrder => this.CartList.Items.Select(item => item.OrderId).Contains(userOrder.OrderId));
			if (isSendMail) order[Constants.ORDER_KEY_MAIL_FOR_USER_REGISTER_WHEN_ORDER_COMPLETE] = null;
			register.OrderCompleteProcesses(order, cart, UpdateHistoryAction.DoNotInsert);

			// 継続課金の契約管理用トラキングIDを定期台帳に保持
			if (cart.HasFixedPurchase
				&& OrderCommon.IsUsablePaymentContinuous(cart.Payment.PaymentId))
			{
				new FixedPurchaseService().SetExternalPaymentAgreementId(
					(string)order[Constants.FIELD_ORDER_FIXED_PURCHASE_ID],
					receiveOrderNotice.TrackingId,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.Insert,
					fixedPurchaseHistoryKbn:Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_CONTINUOUS_ORDER_REGISTER);
			}

			// 完了後処理
			register.AfterOrderCompleteProcesses(order, cart, updateHistoryAction);

			cart.IsOrderDone = true;
		}
	}

	/// <summary>
	/// 「入金済」にするかの判定
	/// </summary>
	/// <param name="cart">カート</param>
	/// <returns>TRUE:入金済　FALSE：未入金</returns>
	private bool JudgePaymentStatusComplete(CartObject cart)
	{
		// 都度課金の場合、設定値の通りにする
		if ((cart.HasFixedPurchase
			&& OrderCommon.IsUsablePaymentContinuous(cart.Payment.PaymentId)) == false)
		{
			return Constants.PAYMENT_SETTING_SBPS_PAYMENT_STATUS_COMPLETE;
		}

		// 継続課金の場合、支払い方法に応じて判定する
		switch (cart.Payment.PaymentId)
		{
			// 指定売上のみ
			case Constants.FLG_PAYMENT_PAYMENT_ID_SOFTBANKKETAI_SBPS:
			case Constants.FLG_PAYMENT_PAYMENT_ID_AUKANTAN_SBPS:
				return false;

			// 自動売上のみ
			case Constants.FLG_PAYMENT_PAYMENT_ID_DOCOMOKETAI_SBPS:
				return true;

			default:
				return Constants.PAYMENT_SETTING_SBPS_PAYMENT_STATUS_COMPLETE;
		}
	}
}