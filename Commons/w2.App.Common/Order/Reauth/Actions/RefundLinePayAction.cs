/*
=========================================================================================================
  Module      : Refund Line Action(RefundLinePayAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.LinePay;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Refund Line Pay Action
	/// </summary>
	public class RefundLinePayAction : BaseReauthAction<RefundLinePayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">Reauth action params</param>
		public RefundLinePayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Refund, "LINEPay決済返金", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// LINE Pay 返金
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var refundAmount = (orderOld.SettlementAmount < orderNew.SettlementAmount)
				? orderOld.SettlementAmount
				: orderOld.SettlementAmount - orderNew.SettlementAmount;
			var result = ExecRefundLine(orderOld, refundAmount);
			return result;
		}

		/// <summary>
		/// Execute Refund LINE Pay
		/// </summary>
		/// <param name="orderOld">Order Old</param>
		/// <param name="refundAmount">Refund Amount</param>
		/// <returns>再与信アクション結果</returns>
		private ReauthActionResult ExecRefundLine(OrderModel orderOld, decimal refundAmount)
		{
			var response = LinePayApiFacade.RefundPayment(
				orderOld.CardTranId,
				refundAmount,
				new LinePayApiFacade.LinePayLogInfo(orderOld));
			if (response.IsSuccess == false)
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(response.ReturnCode, response.ReturnCode));
			}

			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					orderOld.PaymentOrderId,
					orderOld.CardTranId,
					refundAmount),
				cardTranId: orderOld.CardTranId,
				paymentOrderId: orderOld.PaymentOrderId,
				cardTranIdForLog: orderOld.CardTranId);
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
