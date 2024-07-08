/*
=========================================================================================================
  Module      : Cancel Boku Action(CancelBokuAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Boku;
using w2.App.Common.Order.Payment.Boku.Utils;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Cancel Boku Action
	/// </summary>
	public class CancelBokuAction : BaseReauthAction<CancelBokuAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">Reauth action params</param>
		public CancelBokuAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "Bokuキャンセル", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Boku Cancel
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var result = ExecCancelBoku(orderOld, orderNew);

			return result;
		}

		/// <summary>
		/// Exec cancel boku
		/// </summary>
		/// <param name="orderOld">Order old</param>
		/// <param name="orderNew">Order new</param>
		/// <returns>返金結果情報</returns>
		private ReauthActionResult ExecCancelBoku(OrderModel orderOld, OrderModel orderNew)
		{
			// Refund charge
			var refundId = string.Format("refund{0}", orderOld.CardTranId);
			var refund = new PaymentBokuRefundChargeApi().Exec(
				null,
				orderOld.CardTranId,
				refundId,
				BokuConstants.CONST_BOKU_REASON_CODE_GOOD_WILL,
				null,
				(orderOld.OrderTaxIncludedFlg == Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX));

			if (refund == null)
			{
				return new ReauthActionResult(
				false,
				orderOld.OrderId,
				string.Empty,
				apiErrorMessage: LogCreator.CreateErrorMessage(
					string.Empty,
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR)));
			}

			if ((refund.IsSuccess == false)
				|| (refund.RefundStatus == BokuConstants.CONST_BOKU_REFUND_STATUS_FAILED))
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(
						refund.Result.ReasonCode,
						refund.Result.Message));
			}

			// Cancel optin
			if ((orderOld.IsFixedPurchaseOrder == false)
				&& ((orderOld.OrderPaymentKbn != orderNew.OrderPaymentKbn)
					|| orderNew.IsReturnOrder))
			{
				var cancel = new PaymentBokuCancelOptinApi().Exec(orderOld.PaymentOrderId);

				if (cancel == null)
				{
					return new ReauthActionResult(
						false,
						orderOld.OrderId,
						string.Empty,
						apiErrorMessage: LogCreator.CreateErrorMessage(
							string.Empty,
							CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR)));
				}

				if (cancel.IsSuccess == false)
				{
					return new ReauthActionResult(
						false,
						orderOld.OrderId,
						string.Empty,
						apiErrorMessage: LogCreator.CreateErrorMessage(
							cancel.Result.ReasonCode,
							cancel.Result.Message));
				}
			}

			// 決済取引ID、決済注文IDは更新する必要がないので元注文のものをそのまま入れる
			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					orderOld.PaymentOrderId,
					refund.ChargeId,
					orderOld.LastBilledAmount),
				paymentOrderId: orderOld.PaymentOrderId,
				cardTranIdForLog: string.Empty);
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
