/*
=========================================================================================================
  Module      : DSK後払い与信結果取得(PaymentGetAuthResultDskDeferred.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.DSKDeferred;
using w2.App.Common.Order.Payment.DSKDeferred.GetAuth;
using w2.Common.Logger;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.ExternalPaymentChecker.Commands
{
	/// <summary>
	/// DSK後払い与信結果取得
	/// </summary>
	public class PaymentGetAuthResultDskDeferred
	{
		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			var orders = new OrderService().GetCvsDeferredAuthResultHold();
			foreach (var order in orders)
			{
				var adapter = new DskDeferredGetAuthResultAdapter(order.CardTranId, order.PaymentOrderId, order.LastBilledAmount.ToPriceString());
				var response = adapter.Execute();

				if (response.IsResultOk == false)
				{
					var apiErrorMessage = string.Join("\t", response.Errors.Error
						.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray());

					AppLogger.WriteError(string.Format("DSK後払い与信結果取得失敗：{0}：{1}", apiErrorMessage, order.OrderId));

					OrderCommon.AppendExternalPaymentCooperationLog(
						(string.IsNullOrEmpty(apiErrorMessage) == false),
						order.OrderId,
						apiErrorMessage,
						Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.Insert
					);
					continue;
				}

				switch (response.Transaction.AuthorResult)
				{
					case DskDeferredConst.GET_AUTH_RESULT_AUTH_RESULT_OK:
						order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
						order.ExternalPaymentAuthDate = DateTime.Now;
						break;

					case DskDeferredConst.GET_AUTH_RESULT_AUTH_RESULT_NG:
						order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR;
						break;

					case DskDeferredConst.GET_AUTH_RESULT_AUTH_RESULT_PENDING:
						order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_PEND;
						break;

					case DskDeferredConst.GET_AUTH_RESULT_AUTH_RESULT_HOLD:
						break;
				}

				new OrderService().UpdateOrderStatusAndExternalPaymentStatus(
					order.OrderId,
					order.OrderStatus,
					order.ExternalPaymentStatus,
					order.ExternalPaymentAuthDate,
					DateTime.Now,
					Constants.FLG_LASTCHANGED_BATCH,
					UpdateHistoryAction.Insert
				);

				var apiPendingMessgae = "";
				if (response.Transaction.HoldReason != null)
				{
					apiPendingMessgae = string.Join("\t",
						response.Transaction.HoldReason.Select(x => LogCreator.CreateErrorMessage(x.ReasonCode, x.Reason)).ToArray());
				}

				OrderCommon.AppendExternalPaymentCooperationLog(
					true,
					order.OrderId,
					(string.IsNullOrEmpty(apiPendingMessgae) == false)
						? apiPendingMessgae
						: LogCreator.CrateMessageWithCardTranId(
							order.CardTranId, order.PaymentOrderId),
					Constants.FLG_LASTCHANGED_BATCH,
					UpdateHistoryAction.Insert);
			}
			AppLogger.Write("info", string.Format("DSK後払い与信結果取得　{0}件処理", orders.Length));
		}
	}
}
