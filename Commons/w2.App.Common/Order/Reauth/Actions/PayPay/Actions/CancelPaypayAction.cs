/*
=========================================================================================================
  Module      : Paypayキャンセルアクション (CancelPaypayGMOAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using jp.veritrans.tercerog.mdk;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Paypay;
using w2.App.Common.Order.Payment.Veritrans;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Paypayキャンセルアクション
	/// </summary>
	public class CancelPaypayAction : BaseReauthAction<CancelPaypayAction.ReauthActionParams>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">Reauth action params</param>
		public CancelPaypayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "Paypay決済キャンセル", reauthActionParams)
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
			var result = ExecCancelPaypay(orderOld);
			return result;
		}

		/// <summary>
		/// Paypayキャンセル実行
		/// </summary>
		/// <param name="orderOld">更新前注文</param>
		/// <returns>再与信結果</returns>
		private ReauthActionResult ExecCancelPaypay(OrderModel orderOld)
		{
			switch (Constants.PAYMENT_PAYPAY_KBN)
			{
				case Constants.PaymentPayPayKbn.SBPS:
					var cancelApi = new PaymentSBPSPaypayCancelApi();
					var result = cancelApi.Exec(
						orderOld.CardTranId,
						orderOld.OrderPriceTotal);

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
					var capture = paypayFacade.CancelPayment(orderOld);

					if (capture.Result.HasFlag(Results.Failed))
					{
						return new ReauthActionResult(
							false,
							orderOld.OrderId,
							string.Empty,
							cardTranIdForLog: orderOld.CardTranId,
							apiErrorMessage: capture.ErrorMessage);
					}
					break;

				case Constants.PaymentPayPayKbn.VeriTrans:
					var paymentVeritransPaypayApi = new PaymentVeritransPaypay();
					var response = (orderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
						? (IResponseDto)paymentVeritransPaypayApi.Refund(orderOld.PaymentOrderId, orderOld.LastBilledAmount)
						: (IResponseDto)paymentVeritransPaypayApi.Cancel(orderOld.PaymentOrderId);
					if (response.Mstatus != VeriTransConst.RESULT_STATUS_OK)
					{
						return new ReauthActionResult(
							false,
							orderOld.OrderId,
							string.Empty,
							paymentOrderId: orderOld.PaymentOrderId,
							cardTranIdForLog: orderOld.CardTranId,
							apiErrorMessage: LogCreator.CreateErrorMessage(response.VResultCode, response.MerrMsg));
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
					orderOld.LastBilledAmount),
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
			public ReauthActionParams(OrderModel orderOld)
			{
				this.OrderOld = orderOld;
			}

			/// <summary>更新前注文</summary>
			public OrderModel OrderOld { get; private set; }
		}
	}
}
