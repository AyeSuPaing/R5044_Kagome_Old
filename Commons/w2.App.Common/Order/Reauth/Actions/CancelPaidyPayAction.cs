/*
=========================================================================================================
  Module      : Cancel Paidy Pay Action (CancelPaidyPayAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Order.Payment.Paidy;
using w2.App.Common.Order.Payment.Paygent;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Cancel Paidy Pay Action Class
	/// </summary>
	public class CancelPaidyPayAction : BaseReauthAction<CancelPaidyPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		public CancelPaidyPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "Paidy決済キャンセル", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Paidy決済キャンセル
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.Order;
			var orderNew = reauthActionParams.OrderNew;
			var paymentOrderId = string.Empty;
			var cardTranId = string.Empty;
			var orderService = DomainFacade.Instance.OrderService;
			switch (Constants.PAYMENT_PAIDY_KBN)
			{
				case Constants.PaymentPaidyKbn.Direct:
					if (orderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
					{
						var resultOfRefund = PaidyPaymentApiFacade.RefundPayment(
							orderOld.CardTranId,
							orderOld.PaymentOrderId,
							orderOld.LastBilledAmount);
						if (resultOfRefund.HasError)
						{
							return new ReauthActionResult(
								result: false,
								orderOld.OrderId,
								paymentMemo: string.Empty,
								cardTranIdForLog: string.Empty,
								apiErrorMessage: resultOfRefund.GetApiErrorMessages());
						}

						orderService.UpdateCardTranId(
							orderOld.OrderId,
							cardTranId: string.Empty,
							orderOld.LastChanged,
							UpdateHistoryAction.DoNotInsert,
							this.Accessor);

						return new ReauthActionResult(
							result: true,
							orderOld.OrderId,
							CreatePaymentMemo(
								orderOld.OrderPaymentKbn,
								resultOfRefund.Payment.Id,
								resultOfRefund.Payment.Refunds[0].Id,
								orderOld.LastBilledAmount),
							paymentOrderId: resultOfRefund.Payment.Id,
							cardTranIdForLog: resultOfRefund.Payment.Refunds[0].Id);
					}

					var resultOfClose = PaidyPaymentApiFacade.ClosePayment(orderOld.PaymentOrderId);
					if (resultOfClose.HasError)
					{
						return new ReauthActionResult(
							result: false,
							orderOld.OrderId,
							paymentMemo: string.Empty,
							cardTranIdForLog: string.Empty,
							apiErrorMessage: resultOfClose.GetApiErrorMessages());
					}
					paymentOrderId = resultOfClose.Payment.Id;

					orderService.UpdateCardTranId(
						orderOld.OrderId,
						cardTranId: string.Empty,
						orderOld.LastChanged,
						UpdateHistoryAction.DoNotInsert,
						this.Accessor);
					break;

				case Constants.PaymentPaidyKbn.Paygent:
					var isPaygentSuccess = true;
					var paygentErrorMessage = string.Empty;
					var paygentApi = new PaygentApiFacade();
					if ((orderOld.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
						|| reauthActionParams.IsReturnExchange
						|| ((orderOld.LastBilledAmount > orderNew.LastBilledAmount)
							&& (orderOld.OrderPaymentKbn == orderNew.OrderPaymentKbn)))
					{
						var refundAmount =  orderOld.LastBilledAmount
							- ((orderOld.OrderPaymentKbn == orderNew.OrderPaymentKbn)
								? orderNew.LastBilledAmount
								: 0);
						var paygentRefundResult = paygentApi.PaidyRefund(orderOld.CardTranId, refundAmount);

						if (paygentRefundResult.IsSuccess == false)
						{
							isPaygentSuccess = false;
							paygentErrorMessage = paygentRefundResult.GetErrorMessage();
						}
						else
						{
							cardTranId = paygentRefundResult.Response.PaymentId;
						}
					}
					else
					{
						var paygentCancelResult = paygentApi.PaidyAuthorizationCancellation(orderOld.CardTranId);
						if (paygentCancelResult.IsSuccess == false)
						{
							isPaygentSuccess = false;
							paygentErrorMessage = paygentCancelResult.GetErrorMessage();
						}
						cardTranId = orderOld.CardTranId;
					}

					if (isPaygentSuccess == false)
					{
						return new ReauthActionResult(
							result: false,
							orderOld.OrderId,
							paymentMemo: string.Empty,
							cardTranIdForLog: string.Empty,
							apiErrorMessage: paygentErrorMessage);
					}

					paymentOrderId = orderOld.PaymentOrderId;
					break;
			}

			return new ReauthActionResult(
				result: true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					paymentOrderId,
					cardTranId: cardTranId,
					orderOld.LastBilledAmount),
				paymentOrderId: paymentOrderId,
				cardTranId: cardTranId);
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
			/// <param name="order">注文情報</param>
			/// <param name="orderNew">注文情報(変更後)</param>
			/// <param name="isReturnExchange">返品交換フラグ</param>
			public ReauthActionParams(
				OrderModel order,
				OrderModel orderNew,
				bool isReturnExchange)
			{
				this.Order = order;
				this.OrderNew = orderNew;
				this.IsReturnExchange = isReturnExchange;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報</summary>
			public OrderModel Order { get; private set; }
			/// <summary>注文情報(変更後)</summary>
			public OrderModel OrderNew { get; private set; }
			/// <summary>返品交換フラグ</summary>
			public bool IsReturnExchange { get; private set; }
			#endregion
		}
	}
}
