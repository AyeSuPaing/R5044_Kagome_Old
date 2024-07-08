/*
=========================================================================================================
  Module      : Refund Ec Pay Action (RefundEcPayAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.ECPay;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Refund Ec Pay Action Class
	/// </summary>
	public class RefundEcPayAction : BaseReauthAction<RefundEcPayAction.ReauthActionParams>
	{
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reauthActionParams">reauth Action Params</param>
		public RefundEcPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Refund, "ECPay返金", reauthActionParams)
		{
		}
		#endregion

		#region Method
		/// <summary>
		/// On Execute
		/// </summary>
		/// <param name="reauthActionParams">reauth Action Params</param>
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
			var result = ExecRefundEcPay(orderOld, refundAmount);
			if (result.Result)
			{
				orderNew.CardTranId = result.CardTranId;
			}
			return result;
		}

		/// <summary>
		/// Exec Refund EcPay
		/// </summary>
		/// <param name="orderOld">Order Old</param>
		/// <param name="refundAmount">refund Amount</param>
		/// <returns>Result</returns>
		private ReauthActionResult ExecRefundEcPay(OrderModel orderOld, decimal refundAmount)
		{
			var refundLastBilledAmount = CurrencyManager.GetSettlementAmount(
				refundAmount,
				orderOld.SettlementRate,
				orderOld.SettlementCurrency);
			var requestCapture = ECPayUtility.CreateRequestForCancelRefundAndCapturePayment(
				orderOld,
				false,
				true,
				refundLastBilledAmount);
			var responseCapture = ECPayApiFacade.CancelRefundAndCapturePayment(requestCapture);

			if (responseCapture.IsSuccess == false)
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(
						responseCapture.ReturnCode,
						responseCapture.ReturnMessage));
			}
			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					orderOld.OrderId,
					orderOld.CardTranId,
					refundLastBilledAmount),
				cardTranId: responseCapture.TradeNo,
				paymentOrderId: orderOld.PaymentOrderId,
				cardTranIdForLog: responseCapture.TradeNo);
		}
		#endregion

		/// <summary>
		/// ReauthAction Params
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