/*
=========================================================================================================
  Module      : Sales NewebPay Action (SalesNewebPayAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Net;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.NewebPay;
using w2.Common.Logger;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Sales NewebPay Action
	/// </summary>
	public class SalesNewebPayAction : BaseReauthAction<SalesNewebPayAction.ReauthActionParams>
	{
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reauthActionParams">Reauth Action Params</param>
		public SalesNewebPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Sales, "藍新Pay決済売上確定", reauthActionParams)
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
			var order = reauthActionParams.OrderNew;
			var result = ExecSalesNewebPay(order);

			if (result.Result)
			{
				order.CardTranId = result.CardTranId;
			}
			return result;
		}

		/// <summary>
		/// Exec Sales NewebPay
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>Reauth Action Result</returns>
		private ReauthActionResult ExecSalesNewebPay(OrderModel order)
		{
			try
			{
				var saleLastBilledAmount = CurrencyManager.GetSettlementAmount(
					order.LastBilledAmount,
					order.SettlementRate,
					order.SettlementCurrency);
				var requestCapture = NewebPayUtility.CreateCancelRefundCaptureRequest(order, false);
				var responseCapture = NewebPayApiFacade.SendCancelRefundAndCaptureOrder(requestCapture, false);
				if (responseCapture.IsSuccess == false)
				{
					return new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						cardTranIdForLog: order.CardTranId,
						apiErrorMessage: LogCreator.CreateErrorMessage(
							responseCapture.Response.Status,
							responseCapture.Response.Message));
				}
				return new ReauthActionResult(
					true,
					order.OrderId,
					CreatePaymentMemo(
						order.OrderPaymentKbn,
						order.OrderId,
						order.CardTranId,
						saleLastBilledAmount),
					cardTranId: responseCapture.Response.TradeNo,
					cardTranIdForLog: responseCapture.Response.TradeNo);
			}
			catch (WebException exception)
			{
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_NEWEBPAY,
					PaymentFileLogger.PaymentType.NewebPay,
					PaymentFileLogger.PaymentProcessingType.RealSalesProcessing,
					BaseLogger.CreateExceptionMessage(
						"藍新Pay実売上処理失敗エラー" + "[受注ID : " + order.OrderId + "]",
						exception));
			}
			return new ReauthActionResult(
				false,
				order.OrderId,
				string.Empty,
				cardTranIdForLog: order.CardTranId,
				apiErrorMessage: "カード実売上処理失敗エラー");
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
			/// <param name="orderNew">Order New</param>
			public ReauthActionParams(OrderModel orderNew)
			{
				this.OrderNew = orderNew;
			}
			#endregion

			#region Properties
			/// <summary>Order New</summary>
			public OrderModel OrderNew { get; private set; }
			#endregion
		}
	}
}
