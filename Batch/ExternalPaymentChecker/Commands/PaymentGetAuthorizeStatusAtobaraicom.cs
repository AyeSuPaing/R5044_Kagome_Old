/*
=========================================================================================================
  Module      : Payment Get AuthorizeStatus Atobaraicom (PaymentGetAuthorizeStatusAtobaraicom.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.Atobaraicom;
using w2.App.Common.Order.Payment.Atobaraicom.OrderStatus;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.ExternalPaymentChecker.Commands
{
	/// <summary>
	/// Payment get authorize status atobaraicom
	/// </summary>
	public class PaymentGetAuthorizeStatusAtobaraicom
	{
		/// <summary>
		/// Execute
		/// </summary>
		public void Exec()
		{
			try
			{
				// Get order payment ids
				var paymentOrderIds = new OrderService().GetOrderPaymentIdsForAtobaraicomGetAuthorizeStatus();

				var skipCount = 0;
				while (skipCount < paymentOrderIds.Length)
				{
					var requestOrderIds = paymentOrderIds
						.Skip(skipCount)
						.Take(Constants.PAYMENT_ATOBARAICOM_MAX_REQUEST_GET_AUTHORIZE_STATUS)
						.ToArray();
					var request = new AtobaraicomAuthorizeStatusApi();
					var response = request.ExecGetAuthorizeStatus(requestOrderIds);

					// When call API success
					if (response.IsSuccess
						&& string.IsNullOrEmpty(request.ResponseErrorMessage))
					{
						UpdateExternalPaymentStatusProcess(response);
					}

					skipCount += requestOrderIds.Length;
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// Update external payment status process
		/// </summary>
		/// <param name="response">Response data</param>
		private void UpdateExternalPaymentStatusProcess(AtobaraicomAuthorizeStatusResponse response)
		{
			var service = new OrderService();
			foreach (var result in response.Results)
			{
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();
					try
					{
						if (string.IsNullOrEmpty(result.EntOrderId)) continue;

						var order = new OrderService().Get(result.EntOrderId);
						switch (result.OrderStatusCdCode)
						{
							case AtobaraicomConstants.GET_AUTH_RESULT_AUTH_RESULT_HOLD:
								order.ExternalPaymentAuthDate = DateTime.Now;
								break;

							case AtobaraicomConstants.GET_AUTH_RESULT_AUTH_RESULT_OK:
								order.ExternalPaymentAuthDate = DateTime.Now;
								order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
								break;

							case AtobaraicomConstants.GET_AUTH_RESULT_AUTH_RESULT_NG:
								order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR;
								break;

							case AtobaraicomConstants.GET_AUTH_RESULT_AUTH_RESULT_CANCEL:
								order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL;
								var apiCancel = new AtobaraicomCancelationApi();
								var entryApiExecResultCancel = apiCancel.ExecCancel(response.PaymentId);
								if (entryApiExecResultCancel == false) continue;
								break;
						}

						service.UpdateExternalPaymentInfo(
							result.EntOrderId,
							order.ExternalPaymentStatus,
							true,
							order.ExternalPaymentAuthDate,
							string.Empty,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.DoNotInsert,
							accessor);

						accessor.CommitTransaction();
					}
					catch (Exception ex)
					{
						FileLogger.WriteError(ex);
					}
				}
			}
		}
	}
}
