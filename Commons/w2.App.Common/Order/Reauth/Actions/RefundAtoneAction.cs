/*
=========================================================================================================
  Module      : Refund Atone Action(RefundAtoneAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Atone;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// RefundAtoneAction
	/// </summary>
	class RefundAtoneAction : BaseReauthAction<RefundAtoneAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RefundAtoneAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Refund, "Atone 返金", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Atone 返金
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var refundAmount = orderOld.LastBilledAmount - orderNew.LastBilledAmount;

			var descriptionRefund = ValueText.GetValueText(
				Constants.TABLE_ORDER,
				Constants.FIELD_ORDER_RETURN_EXCHANGE_REASON_KBN,
				orderNew.ReturnExchangeReasonKbn);
			var result = ExecRefundAtone(orderOld, refundAmount, descriptionRefund);
			return result;
		}

		/// <summary>
		/// Atone 返金処理
		/// </summary>
		/// <param name="orderOld">注文情報</param>
		/// <param name="refundAmount">Refund Amount</param>
		/// <param name="descriptionRefund">Description Refund</param>
		/// <returns>返金結果情報</returns>
		private ReauthActionResult ExecRefundAtone(OrderModel orderOld, decimal refundAmount, string descriptionRefund)
		{
			if (orderOld.OnlinePaymentStatus != Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED) return null;

			var refundLastBilledAmount = CurrencyManager.GetSettlementAmount(
				refundAmount,
				orderOld.SettlementRate,
				orderOld.SettlementCurrency);
			var requestRefundAtone = new AtoneRefundPaymentRequest()
				{
					AmountRefund = refundLastBilledAmount.ToString("0"),
					DescriptionRefund = descriptionRefund,
					RefundReason = "返品"
				};
			var responeRefundAtone = AtonePaymentApiFacade.RefundPayment(orderOld.CardTranId, requestRefundAtone);

			if (responeRefundAtone.IsSuccess == false)
			{
				var refundAtoneErrorCodes = responeRefundAtone.Errors.Select(item => item.Code);
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(
						string.Join(",", refundAtoneErrorCodes),
						responeRefundAtone.Message));
			}
			if (responeRefundAtone.IsAuthorizationSuccess == false)
			{
				var result = new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: responeRefundAtone.AuthorizationResultNgReasonMessage);
				return result;
			}
			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					orderOld.OrderId,
					orderOld.CardTranId,
					refundLastBilledAmount),
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
