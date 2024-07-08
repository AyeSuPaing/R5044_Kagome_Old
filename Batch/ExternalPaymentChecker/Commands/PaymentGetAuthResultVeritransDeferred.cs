/*
=========================================================================================================
  Module      : ベリトランス後払い与信結果取得 (PaymentGetAuthResultVeritransDeferred.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.Veritrans;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.ExternalPaymentChecker.Commands
{
	/// <summary>
	/// ベリトランス後払い与信結果取得
	/// </summary>
	public class PaymentGetAuthResultVeritransDeferred
	{
		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			var orders = new OrderService().GetCvsDeferredAuthResultHold();
			foreach (var order in orders)
			{
				var response = new PaymentVeritransCvsDef().GetAuthResult(order.PaymentOrderId);

				if (response.Mstatus == VeriTransConst.RESULT_STATUS_NG)
				{
					var apiErrorMessage = response.Errors != null
						? string.Join(
							"\t",
							response.Errors.Select(x => LogCreator.CreateErrorMessage(x.ErrorCode, x.ErrorMessage)).ToArray())
						: LogCreator.CreateErrorMessage(response.VResultCode, response.MerrMsg);

					AppLogger.WriteError($"ベリトランス後払い与信結果取得失敗：{apiErrorMessage}：{order.OrderId}");

					OrderCommon.AppendExternalPaymentCooperationLog(
						string.IsNullOrEmpty(apiErrorMessage) == false,
						order.OrderId,
						apiErrorMessage,
						Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.Insert
					);

					new OrderService().UpdateOrderStatusAndExternalPaymentStatus(
						order.OrderId,
						order.OrderStatus,
						Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR,
						order.ExternalPaymentAuthDate,
						DateTime.Now,
						Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.Insert
					);
					continue;
				}

				if (response.AuthorResult == VeriTransConst.VeritransAuthorResult.Ok.ToText())
				{
					order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
					order.ExternalPaymentAuthDate = DateTime.Now;
				}
				else if (response.AuthorResult == VeriTransConst.VeritransAuthorResult.Ng.ToText())
				{
					order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR;
				}
				else if (response.AuthorResult == VeriTransConst.VeritransAuthorResult.Hold.ToText())
				{
					order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_MIDST;
				}
				else if (response.AuthorResult == VeriTransConst.VeritransAuthorResult.Hr.ToText())
				{
					order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_PEND;
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

				var apiPendingMessgae = string.Empty;
				if (response.HoldReasons != null)
				{
					apiPendingMessgae = string.Join(
						"\t",
						response.HoldReasons.Select(x => LogCreator.CreateErrorMessage(x.ReasonCode, x.Reason)).ToArray());
				}

				OrderCommon.AppendExternalPaymentCooperationLog(
					true,
					order.OrderId,
					string.IsNullOrEmpty(apiPendingMessgae) == false
						? apiPendingMessgae
						: LogCreator.CrateMessageWithCardTranId(order.CardTranId, order.PaymentOrderId),
					Constants.FLG_LASTCHANGED_BATCH,
					UpdateHistoryAction.Insert);
			}
			AppLogger.WriteInfo($"ベリトランス後払い与信結果取得　{orders.Length}件処理");
		}
	}
}
