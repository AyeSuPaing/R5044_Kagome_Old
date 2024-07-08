/*
=========================================================================================================
  Module      : Sales Ec Pay Action (SalesEcPayAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Net;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.ECPay;
using w2.Common.Logger;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Sales Ec Pay Action
	/// </summary>
	public class SalesEcPayAction : BaseReauthAction<SalesEcPayAction.ReauthActionParams>
	{
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="reauthActionParams">reauth Action Params</param>
		public SalesEcPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Sales, "ECPay決済売上確定", reauthActionParams)
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
			var order = reauthActionParams.OrderNew;
			var result = ExecSalesEcPay(order);

			if (result.Result)
			{
				order.CardTranId = result.CardTranId;
			}
			return result;
		}

		/// <summary>
		/// Exec Sales EcPay
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>Result</returns>
		private ReauthActionResult ExecSalesEcPay(OrderModel order)
		{
			try
			{
				var saleLastBilledAmount = CurrencyManager.GetSettlementAmount(
					order.LastBilledAmount,
					order.SettlementRate,
					order.SettlementCurrency);
				var requestCapture = ECPayUtility.CreateRequestForCancelRefundAndCapturePayment(order);
				var responseCapture = ECPayApiFacade.CancelRefundAndCapturePayment(requestCapture);
				if (responseCapture.IsSuccess == false)
				{
					return new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						cardTranIdForLog: order.CardTranId,
						apiErrorMessage: LogCreator.CreateErrorMessage(
							responseCapture.ReturnCode,
							responseCapture.ReturnMessage));
				}
				return new ReauthActionResult(
					true,
					order.OrderId,
					CreatePaymentMemo(
						order.OrderPaymentKbn,
						order.OrderId,
						order.CardTranId,
						saleLastBilledAmount),
					cardTranId: responseCapture.TradeNo,
					cardTranIdForLog: responseCapture.TradeNo);
			}
			catch (WebException webEx)
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_ECPAY,
					PaymentFileLogger.PaymentType.EcPay,
					PaymentFileLogger.PaymentProcessingType.RealSalesProcessing,
					BaseLogger.CreateExceptionMessage(
						"ECPay実売上処理失敗エラー" + "[受注ID : " + order.OrderId + "]",
						webEx));
			}
			// エラー結果返す
			return new ReauthActionResult(
				false,
				order.OrderId,
				string.Empty,
				cardTranIdForLog: order.CardTranId,
				apiErrorMessage: "カード実売上処理失敗エラー");
		}
		#endregion

		/// <summary>
		/// 再与信アクションパラメタ
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