/*
=========================================================================================================
  Module      : Refund Paidy Pay Action (RefundPaidyPayAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.Paidy;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Refund Paidy Pay Action
	/// </summary>
	class RefundPaidyPayAction : BaseReauthAction<RefundPaidyPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		public RefundPaidyPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Refund, "Paidy決済返金", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Paidy決済返金
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var refundAmount = orderOld.LastBilledAmount - orderNew.LastBilledAmount;

			var result = ExecRefundPaidyPay(orderOld, refundAmount);

			return result;
		}

		/// <summary>
		/// Exec Refund Paidy Pay
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>返金結果情報</returns>
		private ReauthActionResult ExecRefundPaidyPay(OrderModel orderOld, decimal refundAmount)
		{
			var cardTranId = orderOld.CardTranId;
			var paymentMemo = string.Empty;
			if ((orderOld.OnlinePaymentStatus != Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				&& (string.IsNullOrEmpty(orderOld.CardTranId)))
			{
				// Capture Paidy payment
				var captureResponse = PaidyPaymentApiFacade.CapturePayment(orderOld.PaymentOrderId);
				if (captureResponse.HasError)
				{
					return new ReauthActionResult(
						false,
						orderOld.OrderId,
						string.Empty,
						cardTranIdForLog: string.Empty,
						apiErrorMessage: captureResponse.GetApiErrorMessages());
				}

				cardTranId = captureResponse.Payment.Captures[0].Id;
				paymentMemo = CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					captureResponse.Payment.Id,
					captureResponse.Payment.Captures[0].Id,
					orderOld.LastBilledAmount);
			}

			// Refund Paidy payment
			var resultOfRefund = PaidyPaymentApiFacade.RefundPayment(
				cardTranId,
				orderOld.PaymentOrderId,
				refundAmount);
			if (resultOfRefund.HasError)
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: string.Empty,
					apiErrorMessage: resultOfRefund.GetApiErrorMessages());
			}

			// Create payment memo
			var paymentRefundMemo = CreatePaymentMemo(
				orderOld.OrderPaymentKbn,
				resultOfRefund.Payment.Id,
				resultOfRefund.Payment.Refunds[0].Id,
				refundAmount);
			paymentMemo = string.IsNullOrEmpty(paymentMemo)
				? paymentRefundMemo
				: string.Format("{0}\r\n{1}",
					paymentMemo,
					paymentRefundMemo);

			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				paymentMemo,
				paymentOrderId: resultOfRefund.Payment.Id,
				cardTranIdForLog: resultOfRefund.Payment.Refunds[0].Id);
		}
		#endregion

		/// <summary>
		/// 再与信アクションパラメタ
		/// </summary>
		public new class ReauthActionParams : IReauthActionParams
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="orderOld">注文情報(変更前)</param>
			/// <param name="orderNew">注文情報(変更後)</param>
			public ReauthActionParams(OrderModel orderOld, OrderModel orderNew)
			{
				this.OrderOld = orderOld;
				this.OrderNew = orderNew;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報(変更前)</summary>
			public OrderModel OrderOld { get; private set; }
			/// <summary>注文情報(変更後)</summary>
			public OrderModel OrderNew { get; private set; }
			#endregion
		}
	}
}
