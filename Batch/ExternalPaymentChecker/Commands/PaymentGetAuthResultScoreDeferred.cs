/*
=========================================================================================================
  Module      : スコア後払い与信結果取得(PaymentGetAuthResultScoreDeferred.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Score;
using w2.App.Common.Order.Payment.Score.GetAuth;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.ExternalPaymentChecker.Commands
{
	/// <summary>
	/// スコア後払い与信結果取得
	/// </summary>
	public class PaymentGetAuthResultScoreDeferred
	{
		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			var orders = new OrderService().GetCvsDeferredAuthResultHold();
			foreach (var order in orders)
			{
				var facade = new ScoreApiFacade();
				var request = new ScoreGetAuthResultRequest(order);
				var response = facade.GetAuthResult(request);

				if (response.Result == ScoreResult.Ng.ToText())
				{
					var apiErrorMessage = string.Join(
						"\t",
						response.Errors.ErrorList.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray());

					AppLogger.WriteError($"スコア後払い与信結果取得失敗：{apiErrorMessage}：{order.OrderId}");

					OrderCommon.AppendExternalPaymentCooperationLog(
						isSuccess: string.IsNullOrEmpty(apiErrorMessage) == false,
						orderId: order.OrderId,
						externalPaymentLog: apiErrorMessage,
						lastChanged: Constants.FLG_LASTCHANGED_BATCH,
						updateHistoryAction: UpdateHistoryAction.Insert
					);
					continue;
				}

				if (response.TransactionResult.AuthorResult == ScoreAuthorResult.Ok.ToText())
				{
					order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
					order.ExternalPaymentAuthDate = DateTime.Now;
				}
				else if (response.TransactionResult.AuthorResult == ScoreAuthorResult.Ng.ToText())
				{
					order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR;
				}
				else if (response.TransactionResult.AuthorResult == ScoreAuthorResult.Pending.ToText())
				{
					order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_PEND;
				}

				new OrderService().UpdateOrderStatusAndExternalPaymentStatus(
					orderId: order.OrderId,
					orderStatus: order.OrderStatus,
					externalPaymentStatus: order.ExternalPaymentStatus,
					externalPaymentAuthDate: order.ExternalPaymentAuthDate,
					updateDate: DateTime.Now,
					lastChanged: Constants.FLG_LASTCHANGED_BATCH,
					updateHistoryAction: UpdateHistoryAction.Insert
				);

				var apiPendingMessage = string.Empty;
				if (response.TransactionResult.HoldReason != null)
				{
					apiPendingMessage = LogCreator.CreateErrorMessage(
						errorCode: response.TransactionResult.HoldReason.ReasonCode,
						errorMessage: response.TransactionResult.HoldReason.Reason);
				}

				OrderCommon.AppendExternalPaymentCooperationLog(
					isSuccess: true,
					orderId: order.OrderId,
					externalPaymentLog: (string.IsNullOrEmpty(apiPendingMessage) == false)
						? apiPendingMessage
						: LogCreator.CrateMessageWithCardTranId(order.CardTranId, order.PaymentOrderId),
					lastChanged: Constants.FLG_LASTCHANGED_BATCH,
					updateHistoryAction: UpdateHistoryAction.Insert);
			}
			AppLogger.Write("info", $"スコア後払い与信結果取得　{orders.Length}件処理");
		}
	}
}
