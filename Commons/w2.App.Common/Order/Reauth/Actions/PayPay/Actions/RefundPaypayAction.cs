/*
=========================================================================================================
  Module      : Paypay返金アクション (RefundPaypayAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Paypay;
using w2.App.Common.Order.Payment.Veritrans;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Paypay返金アクション
	/// </summary>
	public class RefundPaypayAction : BaseReauthAction<RefundPaypayAction.ReauthActionParams>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメータ</param>
		public RefundPaypayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Refund, "Paypay決済返金", reauthActionParams)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメータ</param>
		/// <returns>再与信結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var refundAmount = orderOld.LastBilledAmount - orderNew.LastBilledAmount;
			var result = ExecRefundPaypay(orderOld, refundAmount);
			return result;
		}

		/// <summary>
		/// Paypay返金処理
		/// </summary>
		/// <param name="orderOld">更新前注文</param>
		/// <param name="refundAmount">返金金額</param>
		/// <returns>再与信結果</returns>
		private ReauthActionResult ExecRefundPaypay(OrderModel orderOld, decimal refundAmount)
		{
			switch (Constants.PAYMENT_PAYPAY_KBN)
			{
				case Constants.PaymentPayPayKbn.SBPS:
					var cancelApi = new PaymentSBPSPaypayCancelApi();
					var result = cancelApi.Exec(
						orderOld.CardTranId,
						refundAmount);
					if (result == false)
					{
						return new ReauthActionResult(
							false,
							orderOld.OrderId,
							string.Empty,
							cardTranIdForLog: orderOld.CardTranId,
							apiErrorMessage: cancelApi.ResponseData.ResErrMessages);
					}
					break;

				case Constants.PaymentPayPayKbn.GMO:
					var paypayFacade = new PaypayGmoFacade();
					var resultRefund = paypayFacade.RefundPayment(orderOld, refundAmount);

					if (resultRefund.Result.HasFlag(Results.Failed))
					{
						return new ReauthActionResult(
							false,
							orderOld.OrderId,
							string.Empty,
							cardTranIdForLog: orderOld.CardTranId,
							apiErrorMessage: resultRefund.ErrorMessage);
					}
					break;

				case Constants.PaymentPayPayKbn.VeriTrans:
					var paymentVeritransPaypay = new PaymentVeritransPaypay();
					var paymentRefund = paymentVeritransPaypay.Refund(orderOld.PaymentOrderId, refundAmount);
					if (paymentRefund.Mstatus != VeriTransConst.RESULT_STATUS_OK)
					{
						return new ReauthActionResult(
							result: false,
							orderOld.OrderId,
							string.Empty,
							paymentOrderId: orderOld.OrderId,
							apiErrorMessage: LogCreator.CreateErrorMessage(
								paymentRefund.VResultCode,
								paymentRefund.MerrMsg));
					}
					break;
			}

			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAY,
					orderOld.PaymentOrderId,
					orderOld.CardTranId,
					refundAmount),
				cardTranId: orderOld.CardTranId,
				paymentOrderId: orderOld.PaymentOrderId,
				cardTranIdForLog: orderOld.CardTranId);
		}

		/// <summary>
		/// 再与信アクションパラメータ
		/// </summary>
		public new class ReauthActionParams : IReauthActionParams
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="orderOld">更新前注文</param>
			/// <param name="orderNew">更新後注文</param>
			public ReauthActionParams(OrderModel orderOld, OrderModel orderNew)
			{
				this.OrderOld = orderOld;
				this.OrderNew = orderNew;
			}

			/// <summary>更新前注文</summary>
			public OrderModel OrderOld { get; private set; }
			/// <summary>更新後注文</summary>
			public OrderModel OrderNew { get; private set; }
		}
	}
}
