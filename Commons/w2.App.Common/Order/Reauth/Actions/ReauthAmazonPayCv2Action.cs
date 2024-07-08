/*
=========================================================================================================
  Module      : 再与信アクション（Amazon Pay(CV2)与信）クラス(ReauthAmazonPayCv2Action.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.Domain.Order;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Order.Payment;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（Amazon Pay(CV2)与信）クラス
	/// </summary>
	public class ReauthAmazonPayCv2Action : BaseReauthAction<ReauthAmazonPayCv2Action.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReauthAmazonPayCv2Action(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reauth, "Amazon Pay(CV2)与信", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Amazon Pay(CV2)与信
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var authAmount = orderNew.LastBilledAmount;

			var result = ExecAuthAmazonPayCv2(orderOld, orderNew, authAmount);

			if (result.Result)
			{
				orderNew.CardTranId = result.CardTranId;
				orderNew.PaymentOrderId = result.PaymentOrderId;
			}

			return result;
		}

		/// <summary>
		/// Amazon Pay(CV2)与信処理
		/// </summary>
		/// <param name="orderOld">注文情報(変更前)</param>
		/// <param name="orderNew">注文情報(変更後)</param>
		/// <param name="authAmount">与信金額</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthAmazonPayCv2(OrderModel orderOld, OrderModel orderNew, decimal authAmount)
		{
			var facade = new AmazonCv2ApiFacade();
			var oldCharge = facade.GetCharge(orderOld.CardTranId);
			var oldChargeCaptured = (oldCharge.StatusDetails.State == AmazonCv2Constants.FLG_CHARGE_STATUS_CAPTURED);
			var paymentMemo = new StringBuilder();
			if (oldChargeCaptured)
			{
				var refund = facade.CreateRefund(oldCharge.ChargeId, oldCharge.ChargeAmount.Amount);
				paymentMemo.AppendLine(
					OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
						orderOld.OrderPaymentKbn,
						orderOld.PaymentOrderId,
						oldCharge.ChargeId,
						"Amazon Pay(CV2)返金",
						oldCharge.ChargeAmount.Amount,
						false));
				if ((refund.Success == false)
					&& (refund.Status != AmazonCv2Constants.FLG_REFUND_ERROR_STATUS_TRANSACTION_AMOUNT_EXCEEDED))
				{
					return new ReauthActionResult(
						false,
						orderNew.OrderId,
						paymentMemo.ToString(),
						oldCharge.ChargeId,
						orderNew.PaymentOrderId,
						oldCharge.ChargeId);
				}
			}
			else
			{
				facade.CancelCharge(oldCharge.ChargeId);
				paymentMemo.AppendLine(
					OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
						orderOld.OrderPaymentKbn,
						orderOld.PaymentOrderId,
						oldCharge.ChargeId,
						"Amazon Pay(CV2)キャンセル",
						oldCharge.ChargeAmount.Amount,
						false));
			}

			// 返品・交換の商品だった場合,orderIdを元の注文にする
			if (orderOld.OrderReturnExchangeStatus != Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_UNKNOWN)
			{
				orderOld.OrderId = orderOld.OrderId.Substring(0,
					orderOld.OrderId.IndexOf("-", StringComparison.Ordinal));
			}

			var charge = facade.CreateCharge(orderOld.PaymentOrderId, authAmount, orderOld.OrderId, oldChargeCaptured);

			paymentMemo.Append(
				CreatePaymentMemo(
					orderNew.OrderPaymentKbn,
					orderNew.PaymentOrderId,
					oldCharge.ChargeId,
					authAmount));

			var chargeError = AmazonCv2ApiFacade.GetErrorCodeAndMessage(charge);
			if ((charge.Success == false)
				|| (charge.StatusDetails.State == AmazonCv2Constants.AMAZON_ACTION_STATUS_AUTH))
			{
				var errorMessage = (charge.Success == false)
					? LogCreator.CreateErrorMessage(chargeError.ReasonCode, chargeError.Message)
					: CommerceMessages.GetMessages(CommerceMessages.ERRMSG_RECREDIT_AMAZONPAY_RESPONSE);

				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					paymentMemo.ToString(),
					paymentOrderId: orderOld.PaymentOrderId,
					apiErrorMessage: errorMessage);
			}

			return new ReauthActionResult(
				true,
				orderNew.OrderId,
				paymentMemo.ToString(),
				charge.ChargeId,
				orderNew.PaymentOrderId,
				charge.ChargeId);
		}
		#endregion

		#region プロパティ
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
			/// <param name="orderOld">注文情報（変更前）</param>
			/// <param name="orderNew">注文情報（変更後）</param>
			public ReauthActionParams(OrderModel orderOld, OrderModel orderNew)
			{
				this.OrderOld = orderOld;
				this.OrderNew = orderNew;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報（変更前）</summary>
			public OrderModel OrderOld { get; private set; }
			/// <summary>注文情報（変更後）</summary>
			public OrderModel OrderNew { get; private set; }
			#endregion
		}
	}
}
