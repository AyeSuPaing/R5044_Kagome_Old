/*
=========================================================================================================
  Module      : GMOアトカラ与信結果取得(PaymentGetAuthResultGmoAtokara.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.GMOAtokara;
using w2.App.Common.Order.Payment.GMOAtokara.Utils;
using w2.Common.Logger;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.ExternalPaymentChecker.Commands
{
	/// <summary>
	/// GMOアトカラ与信結果取得
	/// </summary>
	public class PaymentGetAuthResultGmoAtokara
	{
		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			var orders = new OrderService().GetOrderForGmoAtokaraAuthResult();
			foreach (var order in orders)
			{
				try
				{
					var getAuthorizationResultApi = new PaymentGmoAtokaraGetAuthorizationResultApi();
					var result = getAuthorizationResultApi.Exec(order.CardTranId);

					if (result == false)
					{
						var apiErrorMessage = getAuthorizationResultApi.ResponseData.Errors.GetErrorMessage();

						AppLogger.WriteError($"GMOアトカラ与信結果取得失敗：{apiErrorMessage}：{order.OrderId}");

						OrderCommon.AppendExternalPaymentCooperationLog(
							isSuccess: string.IsNullOrEmpty(apiErrorMessage) == false,
							orderId: order.OrderId,
							externalPaymentLog: apiErrorMessage,
							lastChanged: Constants.FLG_LASTCHANGED_BATCH,
							updateHistoryAction: UpdateHistoryAction.Insert
						);
						continue;
					}

					switch (getAuthorizationResultApi.ResponseData.TransactionResult.AuthAuthorResult)
					{
						case PaymentGmoAtokaraConstants.AUTHORRESULT_OK:
							ProcessOrderComplete(order);
							break;

						case PaymentGmoAtokaraConstants.AUTHORRESULT_NG:
							break;

						case PaymentGmoAtokaraConstants.AUTHORRESULT_REVIEW:
							switch (getAuthorizationResultApi.ResponseData.TransactionResult.MaulAuthorResult)
							{
								case PaymentGmoAtokaraConstants.AUTHORRESULT_OK:
									ProcessOrderComplete(order);
									break;

								case PaymentGmoAtokaraConstants.AUTHORRESULT_NG:
									order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR;
									order.ExternalPaymentAuthDate = DateTime.Now;

									// 注文確定処理
									new OrderService().UpdateOrderStatusAndExternalPaymentStatus(
										order.OrderId,
										order.OrderStatus,
										order.ExternalPaymentStatus,
										order.ExternalPaymentAuthDate,
										DateTime.Now,
										Constants.FLG_LASTCHANGED_BATCH,
										UpdateHistoryAction.Insert
									);
									OrderCommon.AppendExternalPaymentCooperationLog(
										isSuccess: true,
										orderId: order.OrderId,
										externalPaymentLog: "審査結果：NG",
										lastChanged: Constants.FLG_LASTCHANGED_BATCH,
										updateHistoryAction: UpdateHistoryAction.Insert
									);
									break;

								case PaymentGmoAtokaraConstants.AUTHORRESULT_HOLD:
									order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_PEND;
									order.ExternalPaymentAuthDate = DateTime.Now;

									new OrderService().UpdateOrderStatusAndExternalPaymentStatus(
										order.OrderId,
										order.OrderStatus,
										order.ExternalPaymentStatus,
										order.ExternalPaymentAuthDate,
										DateTime.Now,
										Constants.FLG_LASTCHANGED_BATCH,
										UpdateHistoryAction.Insert
									);
									OrderCommon.AppendExternalPaymentCooperationLog(
										isSuccess: true,
										orderId: order.OrderId,
										externalPaymentLog: "審査保留理由：" + getAuthorizationResultApi.ResponseData.TransactionResult.Reason,
										lastChanged: Constants.FLG_LASTCHANGED_BATCH,
										updateHistoryAction: UpdateHistoryAction.Insert
									);
									break;
							}

							break;
					}
				}
				catch (Exception ex)
				{
					AppLogger.WriteError($"GMOアトカラ与信結果取得失敗：{order.OrderId}", ex);

					OrderCommon.AppendExternalPaymentCooperationLog(
						isSuccess: false,
						orderId: order.OrderId,
						externalPaymentLog: LogCreator.CrateMessageWithCardTranId(
							order.CardTranId,
							order.PaymentOrderId),
						lastChanged: Constants.FLG_LASTCHANGED_BATCH,
						updateHistoryAction: UpdateHistoryAction.Insert
					);
					continue;
				}
			}
			AppLogger.WriteInfo($"GMOアトカラ与信結果取得　{orders.Length}件処理");
		}

		/// <summary>
		/// 注文確定処理
		/// </summary>
		/// <param name="order">注文情報</param>
		private void ProcessOrderComplete(OrderModel order)
		{
			order.OrderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDERED;
			order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
			order.ExternalPaymentAuthDate = DateTime.Now;

			// 注文確定処理
			new OrderService().UpdateOrderStatusAndExternalPaymentStatus(
				order.OrderId,
				order.OrderStatus,
				order.ExternalPaymentStatus,
				order.ExternalPaymentAuthDate,
				DateTime.Now,
				Constants.FLG_LASTCHANGED_BATCH,
				UpdateHistoryAction.Insert
			);

			OrderCommon.AppendExternalPaymentCooperationLog(
				isSuccess: true,
				orderId: order.OrderId,
				externalPaymentLog: LogCreator.CrateMessageWithCardTranId(order.CardTranId, order.PaymentOrderId),
				lastChanged: Constants.FLG_LASTCHANGED_BATCH,
				updateHistoryAction: UpdateHistoryAction.Insert);
		}
	}
}
