/*
=========================================================================================================
  Module      : Paypay売上確定アクション (SalesPaypayAction.cs)
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
	/// Paypay売上確定アクション
	/// </summary>
	public class SalesPaypayAction : BaseReauthAction<SalesPaypayAction.ReauthActionParams>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメータ</param>
		public SalesPaypayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Sales, "Paypay決済売上確定", reauthActionParams)
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
			if (orderOld.LastBilledAmount == 0)
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					apiErrorMessage: "最終請求金額が0円のため、売上確定できません。");
			}

			var result = ExecSalesMerpay(orderOld);
			if (result.Result)
			{
				orderOld.CardTranId = result.CardTranId;
				orderOld.PaymentOrderId = result.PaymentOrderId;
			}
			return result;
		}

		/// <summary>
		/// Paypay売上確定
		/// </summary>
		/// <param name="orderOld">更新前注文</param>
		/// <returns>再与信結果</returns>
		private ReauthActionResult ExecSalesMerpay(OrderModel orderOld)
		{
			switch (Constants.PAYMENT_PAYPAY_KBN)
			{
				case Constants.PaymentPayPayKbn.SBPS:
					var saleApi = new PaymentSBPSPaypaySaleApi();
					var result = saleApi.Exec(
						orderOld.CardTranId,
						orderOld.OrderPriceTotal);

					if (result == false)
					{
						return new ReauthActionResult(
							false,
							orderOld.OrderId,
							string.Empty,
							cardTranIdForLog: orderOld.CardTranId,
							apiErrorMessage: saleApi.ResponseData.ResErrMessages);
					}
					break;

				case Constants.PaymentPayPayKbn.GMO:
					var paypayFacade = new PaypayGmoFacade();
					var capture = paypayFacade.CapturePayment(orderOld);

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
					var capturePayment = new PaymentVeritransPaypay().Capture(orderOld);
					if (capturePayment.Mstatus != VeriTransConst.RESULT_STATUS_OK)
					{
						return new ReauthActionResult(
							result: false,
							orderOld.OrderId,
							string.Empty,
							paymentOrderId: orderOld.PaymentOrderId,
							apiErrorMessage: LogCreator.CreateErrorMessage(
								capturePayment.VResultCode,
								capturePayment.MerrMsg));
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
