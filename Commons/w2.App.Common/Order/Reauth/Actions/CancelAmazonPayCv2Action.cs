/*
=========================================================================================================
  Module      : 再与信アクション（Amazon Pay(CV2)キャンセル）クラス(CancelAmazonPayCv2Action.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using w2.Domain.Order;
using w2.App.Common.AmazonCv2;
using w2.App.Common.Order.Payment;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（Amazon Pay(CV2)キャンセル）クラス
	/// </summary>
	public class CancelAmazonPayCv2Action : BaseReauthAction<CancelAmazonPayCv2Action.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CancelAmazonPayCv2Action(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "Amazon Pay(CV2)キャンセル", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Amazon Pay(CV2)キャンセル
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var result = ExecCancelAmazonPayCv2(orderOld);
			return result;
		}

		/// <summary>
		/// Amazon Pay(CV2)キャンセル処理(オーソリクローズ)
		/// </summary>
		/// <param name="orderOld">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelAmazonPayCv2(OrderModel orderOld)
		{
			var facade = new AmazonCv2ApiFacade();
			var charge = facade.GetCharge(orderOld.CardTranId);

			// オンライン決済ステータスが「売上確定済」の場合は全額返金処理
			if (charge.StatusDetails.State == AmazonCv2Constants.FLG_CHARGE_STATUS_CAPTURED)
			{
				var refund = facade.CreateRefund(orderOld.CardTranId, charge.CaptureAmount.Amount);
				var refundError = AmazonCv2ApiFacade.GetErrorCodeAndMessage(refund);
				if ((refund.Success == false)
					&& (refund.Status != AmazonCv2Constants.FLG_REFUND_ERROR_STATUS_TRANSACTION_AMOUNT_EXCEEDED))
				{
					return new ReauthActionResult(
						false,
						orderOld.OrderId,
						string.Empty,
						cardTranIdForLog: refund.RefundId,
						apiErrorMessage: LogCreator.CreateErrorMessage(refundError.ReasonCode, refundError.Message));
				}

				return new ReauthActionResult(
					true,
					orderOld.OrderId,
					CreatePaymentMemo(orderOld.OrderPaymentKbn, orderOld.PaymentOrderId, refund.RefundId, orderOld.LastBilledAmount),
					cardTranId: refund.RefundId,
					cardTranIdForLog: refund.RefundId);
			}

			var cancel = facade.CancelCharge(orderOld.CardTranId);
			var cancelError = AmazonCv2ApiFacade.GetErrorCodeAndMessage(cancel);
			if (cancel.Success == false)
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(cancelError.ReasonCode, cancelError.Message));
			}

			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(orderOld.OrderPaymentKbn, orderOld.PaymentOrderId, orderOld.PaymentOrderId, orderOld.LastBilledAmount),
				cardTranId: orderOld.PaymentOrderId,
				cardTranIdForLog: orderOld.PaymentOrderId);
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
			public ReauthActionParams(OrderModel orderOld)
			{
				this.OrderOld = orderOld;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報</summary>
			public OrderModel OrderOld { get; private set; }
			#endregion
		}
	}
}
