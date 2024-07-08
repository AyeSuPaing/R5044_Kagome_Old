/*
=========================================================================================================
  Module      : Cancel NewebPay Action (CancelNewebPayAction.cs)
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
	/// Cancel NewebPay Action Class
	/// </summary>
	public class CancelNewebPayAction : BaseReauthAction<CancelNewebPayAction.ReauthActionParams>
	{
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reauthActionParams">Reauth Action Params</param>
		public CancelNewebPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "藍新Pay決済キャンセル", reauthActionParams)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// On execute
		/// </summary>
		/// <param name="reauthActionParams">Reauth Action Params</param>
		/// <returns>Reauth Action Result</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var result = ExecCancelNewebPay(orderOld);
			return result;
		}

		/// <summary>
		/// Exec Cancel NewebPay
		/// </summary>
		/// <param name="orderOld">Old Order</param>
		/// <returns>Reauth Action Result</returns>
		private ReauthActionResult ExecCancelNewebPay(OrderModel orderOld)
		{
			var requestCancel = NewebPayUtility.CreateCancelRefundCaptureRequest(orderOld, true);
			var resultCancel = NewebPayApiFacade.SendCancelRefundAndCaptureOrder(requestCancel, true);
			var cancelLastBilledAmount = CurrencyManager.GetSettlementAmount(
				orderOld.LastBilledAmount,
				orderOld.SettlementRate,
				orderOld.SettlementCurrency);

			if (resultCancel.IsSuccess == false)
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(
						resultCancel.Response.Status,
						resultCancel.Response.Message));
			}
			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					orderOld.OrderId,
					orderOld.CardTranId,
					cancelLastBilledAmount),
				cardTranId: resultCancel.Response.TradeNo,
				cardTranIdForLog: resultCancel.Response.TradeNo);
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
			/// <param name="orderOld">Old Order</param>
			public ReauthActionParams(OrderModel orderOld)
			{
				this.OrderOld = orderOld;
			}
			#endregion

			#region Properties
			/// <summary>Old Order</summary>
			public OrderModel OrderOld { get; private set; }
			#endregion
		}
	}
}
