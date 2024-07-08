/*
=========================================================================================================
  Module      : 再与信アクション（NP後払いキャンセル）クラス(CancelNPAfterPayAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（NP後払いキャンセル）クラス
	/// </summary>
	public class CancelNPAfterPayAction : BaseReauthAction<CancelNPAfterPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		public CancelNPAfterPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "NP後払いキャンセル", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// NP後払いキャンセル処理
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.orderOld;
			var requestCancel = NPAfterPayUtility.CreateCancelOrGetPaymentRequestData(orderOld.CardTranId);
			var resultCancel = NPAfterPayApiFacade.CancelOrder(requestCancel);
			if (resultCancel.IsSuccess == false)
			{
				var apiErrorMessage = resultCancel.GetApiErrorMessage(orderOld.IsExecuteReauthFromMyPage);
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: apiErrorMessage);
			}
			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					orderOld.PaymentOrderId,
					orderOld.CardTranId,
					GetSendingAmount(orderOld)),
				paymentOrderId: orderOld.PaymentOrderId,
				cardTranId: orderOld.CardTranId,
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
			/// <param name="orderOld">注文情報（変更前）</param>
			public ReauthActionParams(OrderModel orderOld)
			{
				this.orderOld = orderOld;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報（変更前）</summary>
			public OrderModel orderOld { get; private set; }
			#endregion
		}
	}
}