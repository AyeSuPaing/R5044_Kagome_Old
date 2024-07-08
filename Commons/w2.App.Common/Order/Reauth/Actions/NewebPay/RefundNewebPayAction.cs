/*
=========================================================================================================
  Module      : Refund NewebPay Action (RefundNewebPayAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.NewebPay;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Refund NewebPay Action
	/// </summary>
	public class RefundNewebPayAction : BaseReauthAction<RefundNewebPayAction.ReauthActionParams>
	{
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reauthActionParams">Reauth Action Params</param>
		public RefundNewebPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Refund, "藍新Pay決済返金", reauthActionParams)
		{
		}
		#endregion

		#region Method
		/// <summary>
		/// On Execute
		/// </summary>
		/// <param name="reauthActionParams">Reauth Action Params</param>
		/// <returns>Result</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var isSalesOrder = ((orderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				&& (orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP));
			var isChangePayment = (orderOld.OrderPaymentKbn != orderNew.OrderPaymentKbn);
			var refundAmount = (isSalesOrder && isChangePayment)
				? orderOld.LastBilledAmount
				: (orderOld.LastBilledAmount - orderNew.LastBilledAmount);
			var result = ExecRefundNewebPay(
				orderOld,
				refundAmount,
				(isChangePayment && isSalesOrder),
				orderNew.CardTranId,
				orderNew.PaymentOrderId);
			if (result.Result
				&& (isChangePayment == false))
			{
				orderNew.CardTranId = result.CardTranId;
			}
			return result;
		}

		/// <summary>
		/// Exec Refund NewebPay
		/// </summary>
		/// <param name="orderOld">Old Order</param>
		/// <param name="refundAmount">Refund Amount</param>
		/// <param name="isChangePaymentAfterSales">Is Change Payment After Sales</param>
		/// <param name="orderNewCardTranId">Order New Card Tran Id</param>
		/// <param name="orderNewPaymentOrderId">Order New Payment Order Id</param>
		/// <returns>Reauth Action Result</returns>
		private ReauthActionResult ExecRefundNewebPay(
			OrderModel orderOld,
			decimal refundAmount,
			bool isChangePaymentAfterSales,
			string orderNewCardTranId,
			string orderNewPaymentOrderId)
		{
			var refundLastBilledAmount = CurrencyManager.GetSettlementAmount(
				refundAmount,
				orderOld.SettlementRate,
				orderOld.SettlementCurrency);
			var requestRefund = NewebPayUtility.CreateCancelRefundCaptureRequest(orderOld, false, true, refundLastBilledAmount);
			var responseRefund = NewebPayApiFacade.SendCancelRefundAndCaptureOrder(requestRefund, false);

			if (responseRefund.IsSuccess == false)
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(
						responseRefund.Response.Status,
						responseRefund.Response.Message));
			}
			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					orderOld.OrderId,
					orderOld.CardTranId,
					refundLastBilledAmount),
				cardTranId: (isChangePaymentAfterSales)
					? orderNewCardTranId
					: responseRefund.Response.TradeNo,
				paymentOrderId: (isChangePaymentAfterSales)
					? orderNewPaymentOrderId
					: orderOld.PaymentOrderId,
				cardTranIdForLog: responseRefund.Response.TradeNo);
		}
		#endregion

		/// <summary>
		/// Reauth Action Params
		/// </summary>
		public new class ReauthActionParams : IReauthActionParams
		{
			#region Constructor
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="orderOld">Order Old</param>
			/// <param name="orderNew">Order New</param>
			public ReauthActionParams(
				OrderModel orderOld,
				OrderModel orderNew)
			{
				this.OrderOld = orderOld;
				this.OrderNew = orderNew;
			}
			#endregion

			#region Properties
			/// <summary>Order Old</summary>
			public OrderModel OrderOld { get; private set; }
			/// <summary>Order New</summary>
			public OrderModel OrderNew { get; private set; }
			#endregion
		}
	}
}
