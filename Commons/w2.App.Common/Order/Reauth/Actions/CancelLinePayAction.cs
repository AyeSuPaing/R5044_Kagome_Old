/*
=========================================================================================================
  Module      : Cancel Line Pay Action(CancelLinePayAction.cs)
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
	/// Cancel Line Pay Action
	/// </summary>
	public class CancelLinePayAction : BaseReauthAction<CancelLinePayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">Reauth action params</param>
		public CancelLinePayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "LINEPay決済キャンセル", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// LINE Payキャンセル
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			if (orderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
			{
				var refund = LinePayApiFacade.RefundPayment(
					orderOld.CardTranId,
					orderOld.SettlementAmount,
					new LinePayApiFacade.LinePayLogInfo(orderOld));
				if (refund.IsSuccess == false)
				{
					return new ReauthActionResult(
						false,
						orderOld.OrderId,
						string.Empty,
						cardTranIdForLog: orderOld.CardTranId,
						apiErrorMessage: LogCreator.CreateErrorMessage(refund.ReturnCode, refund.ReturnMessage));
				}

				return new ReauthActionResult(
					true,
					orderOld.OrderId,
					CreatePaymentMemo(
						orderOld.OrderPaymentKbn,
						orderOld.PaymentOrderId,
						orderOld.CardTranId,
						orderOld.SettlementAmount),
					paymentOrderId: orderOld.PaymentOrderId,
					cardTranId: orderOld.CardTranId,
					cardTranIdForLog: orderOld.CardTranId);
			}

			var cancel = LinePayApiFacade.VoidApiPayment(
				orderOld.CardTranId,
				new LinePayApiFacade.LinePayLogInfo(orderOld));
			if (cancel.IsSuccess == false)
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(cancel.ReturnCode, cancel.ReturnMessage));
			}

			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					orderOld.PaymentOrderId,
					orderOld.CardTranId,
					orderOld.SettlementAmount),
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
			/// <param name="orderNew">注文情報（変更後）</param>
			public ReauthActionParams(OrderModel orderOld, OrderModel orderNew)
			{
				this.OrderOld = orderOld;
				this.OrderNew = orderNew;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報(変更前)</summary>
			public OrderModel OrderOld { get; private set; }
			/// <summary>注文情報（変更後）</summary>
			public OrderModel OrderNew { get; private set; }
			#endregion
		}
	}
}
