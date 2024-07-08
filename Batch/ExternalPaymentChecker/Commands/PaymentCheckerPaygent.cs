/*
=========================================================================================================
  Module      : Payment Checker Paygent(PaymentCheckerPaygent.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using w2.App.Common.Order.Payment.Paygent;
using w2.Common.Util;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain;

namespace w2.Commerce.Batch.ExternalPaymentChecker.Commands
{
	/// <summary>
	/// Payment checker paygent
	/// </summary>
	public class PaymentCheckerPaygent : PaymentCheckerBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="paymentId">Payment id</param>
		/// <param name="limitDay">Limit day</param>
		public PaymentCheckerPaygent(string paymentId, int limitDay)
		{
			this.PaymentId = paymentId;
			this.LimitDay = limitDay;
		}

		/// <summary>
		/// Execute
		/// </summary>
		public override void Exec()
		{
			var paymentOrders = GetNonPaymentOrders(
				this.PaymentId,
				DateTime.Now.AddDays(-this.LimitDay));
			foreach (DataRowView paymentOrder in paymentOrders)
			{
				var orderId = StringUtility.ToEmpty(paymentOrder[Constants.FIELD_ORDER_ORDER_ID]);
				var cardTranId = StringUtility.ToEmpty(paymentOrder[Constants.FIELD_ORDER_CARD_TRAN_ID]);
				var response = new PaygentApiFacade().GetPaymentInformationInquiry(cardTranId, orderId);
				if (response.IsSuccess == false)
				{
					if (response.Response?.ResponseCode == PaygentConstants.PAYGENT_API_RESPONSE_STATUS_CODE_NO_MATCHING_PAYMENT_INFORMATION) continue;

					DomainFacade.Instance.OrderService.AppendExternalPaymentCooperationLog(
						orderId,
						string.Format(
							"{0:yyyy/MM/dd HH:mm:ss}\t[失敗]\t決済ステータス通知受取 order_id:{1} 最終更新者: {2}\r\n",
							DateTime.Now,
							orderId,
							Constants.FLG_LASTCHANGED_BATCH),
						Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.DoNotInsert);
					continue;
				}

				PaygentUtility.UpdateOrderForPaygent(
					orderId,
					response.Response.PaymentStatus,
					response.Response.PaymentType,
					response.Response.PaymentInitDate,
					Constants.FLG_LASTCHANGED_BATCH);
			}
		}

		/// <summary>Payment id</summary>
		public string PaymentId { get; set; }
		/// <summary>Limit day</summary>
		public int LimitDay { get; set; }
	}
}
