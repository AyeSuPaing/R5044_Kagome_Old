/*
=========================================================================================================
  Module      : 再与信アクション（Amazon Pay(CV2)返金）クラス(RefundAmazonPayCv2Action.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.AmazonCv2;
using w2.App.Common.Order.Payment;
using w2.Domain.Order;
namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（Amazon Pay(CV2)返金）クラス
	/// </summary>
	public class RefundAmazonPayCv2Action : BaseReauthAction<RefundAmazonPayCv2Action.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RefundAmazonPayCv2Action(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Refund, "Amazon Pay(CV2)返金", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Amazon Pay(CV2)返金
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var refundAmount = orderOld.LastBilledAmount - orderNew.LastBilledAmount;

			var result = ExecRefundAmazonPay(orderOld, refundAmount);

			return result;
		}

		/// <summary>
		/// Amazon Pay返金処理
		/// </summary>
		/// <param name="orderOld">注文情報</param>
		/// <param name="refundAmount">返金額</param>
		/// <returns>返金結果情報</returns>
		private ReauthActionResult ExecRefundAmazonPay(OrderModel orderOld, decimal refundAmount)
		{
			var refund = new AmazonCv2ApiFacade().CreateRefund(orderOld.CardTranId, refundAmount);
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

			// 決済注文IDは更新する必要がないので元注文のものをそのまま入れる
			// 決済取引IDは更新したくないので空文字にする
			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(orderOld.OrderPaymentKbn, orderOld.PaymentOrderId, refund.RefundId, refundAmount),
				cardTranId: string.Empty,
				paymentOrderId: orderOld.PaymentOrderId,
				cardTranIdForLog: refund.RefundId);
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
