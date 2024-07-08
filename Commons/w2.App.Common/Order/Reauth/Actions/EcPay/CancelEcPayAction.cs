/*
=========================================================================================================
  Module      : Cancel Ec Pay Action (CancelEcPayAction.cs)
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
	///Cancel Ec Pay Action Class
	/// </summary>
	public class CancelEcPayAction : BaseReauthAction<CancelEcPayAction.ReauthActionParams>
	{
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reauthActionParams">reauth Action Params</param>
		public CancelEcPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "EcPay決済キャンセル", reauthActionParams)
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
			var orderOld = reauthActionParams.Order;
			var requestCapture = ECPayUtility.CreateRequestForCancelRefundAndCapturePayment(orderOld, true);
			var responseCapture = ECPayApiFacade.CancelRefundAndCapturePayment(requestCapture);
			var cancelLastBilledAmount = CurrencyManager.GetSettlementAmount(
				orderOld.LastBilledAmount,
				orderOld.SettlementRate,
				orderOld.SettlementCurrency);

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
					cancelLastBilledAmount),
				cardTranId: responseCapture.TradeNo,
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
			/// <param name="order">order</param>
			public ReauthActionParams(OrderModel order)
			{
				this.Order = order;
			}
			#endregion

			#region Properties
			/// <summary>Order</summary>
			public OrderModel Order { get; private set; }
			#endregion
		}
	}
}
