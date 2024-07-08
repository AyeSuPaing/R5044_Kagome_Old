/*
=========================================================================================================
  Module      : 再与信アクション（NP後払い注文情報更新）クラス(UpdateNPAfterPayAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（NP後払い注文情報更新）クラス
	/// </summary>
	public class UpdateNPAfterPayAction : BaseReauthAction<UpdateNPAfterPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		public UpdateNPAfterPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Update, "NP後払い注文情報更新", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// NP後払い注文情報更新
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderNew = reauthActionParams.OrderNew;
			var orderOld = reauthActionParams.OrderOld;

			// For modify order
			if (string.IsNullOrEmpty(orderNew.OrderIdOrg)
				&& (orderNew.OrderId == orderOld.OrderId))
			{
				// 最終請求金額が0円の場合、エラーとする
				if (orderNew.LastBilledAmount == 0)
				{
					return new ReauthActionResult(
						false,
						orderNew.OrderId,
						string.Empty,
						apiErrorMessage: "最終請求金額が0円のため、与信できません。");
				}
			}

			return ExecutePayment(orderNew, orderOld);
		}

		/// <summary>
		/// Execute Payment
		/// </summary>
		/// <param name="order">order</param>
		/// <param name="orderOld">変更前注文情報</param>
		/// <returns>再与信アクション結果</returns>
		private ReauthActionResult ExecutePayment(OrderModel order, OrderModel orderOld)
		{
			var paymentOrderId = order.PaymentOrderId;
			var formattedErrorMessage = string.Empty;
			var requestUpdate = NPAfterPayUtility.CreateOrderRequestData(
				orderOld,
				paymentOrderId,
				true,
				false,
				order);
			var resultUpdate = NPAfterPayApiFacade.UpdateOrder(requestUpdate);
			if (resultUpdate.IsSuccess == false)
			{
				formattedErrorMessage = resultUpdate.GetApiErrorMessage(order.IsExecuteReauthFromMyPage);
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: formattedErrorMessage);
			}
			if (resultUpdate.IsAuthoriReviewOk == false)
			{
				var apiAuthoriErrorMessage = resultUpdate.GetApiErrorMessage(order.IsExecuteReauthFromMyPage);
				if (resultUpdate.IsAuthoriReviewPending)
				{
					apiAuthoriErrorMessage = apiAuthoriErrorMessage
						.Replace("@@ 1 @@", order.Owner.ConcatenateAddressWithoutCountryName())
						.Replace("@@ 2 @@", order.Shippings[0].ConcatenateAddressWithoutCountryName());
				}
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: apiAuthoriErrorMessage);
			}
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(
					order.OrderPaymentKbn,
					paymentOrderId,
					resultUpdate.GetNPTransactionId(),
					GetSendingAmount(order)),
				paymentOrderId: paymentOrderId,
				cardTranId: resultUpdate.GetNPTransactionId(),
				cardTranIdForLog: order.CardTranId);
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