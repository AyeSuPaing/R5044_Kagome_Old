/*
=========================================================================================================
  Module      : 再与信アクション（スコア後払い注文情報更新）クラス(UpdateScoreAfterPayAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.Order.Payment.Score;
using w2.App.Common.Order.Payment.Score.Order;
using w2.Common.Helper;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（スコア後払い注文情報更新）クラス
	/// </summary>
	public class UpdateScoreAfterPayAction : BaseReauthAction<UpdateScoreAfterPayAction.ReauthActionParams>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		public UpdateScoreAfterPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Update, "スコア後払い注文情報更新", reauthActionParams)
		{
		}

		/// <summary>
		/// スコア後払い注文情報更新
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var order = reauthActionParams.OrderNew;
			var paymentOrderId = order.PaymentOrderId;
			var requestUpdate = new ScoreRequestOrderRegisterModify(order);
			var resultUpdate = new ScoreApiFacade().OrderModify(requestUpdate);

			if ((resultUpdate == null)
				|| (resultUpdate.Result != ScoreResult.Ok.ToText())
				|| (resultUpdate.TransactionResult.IsResultNg))
			{
				var errorMessage = (resultUpdate != null)
					? string.Join("\r\n", resultUpdate.Errors.ErrorList.Select(e => $"{e.ErrorCode}：{e.ErrorMessage}").ToArray())
					: CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_SCORE_PAYMENT_CHANGE_ERROR);

				return new ReauthActionResult(
					result: false,
					orderId: order.OrderId,
					paymentMemo: string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: errorMessage);
			}

			var reauthActionResult = new ReauthActionResult(
				result: true,
				orderId: order.OrderId,
				paymentMemo: CreatePaymentMemo(
					paymentId: order.OrderPaymentKbn,
					paymentOrderId: paymentOrderId,
					cardTranId: resultUpdate.TransactionResult.NissenTransactionId,
					lastBilledAmount: order.OrderPriceTotal),
				paymentOrderId: string.IsNullOrEmpty(value: resultUpdate.TransactionResult.ShopTransactionId)
					? paymentOrderId
					: resultUpdate.TransactionResult.ShopTransactionId,
				cardTranId: resultUpdate.TransactionResult.NissenTransactionId,
				apiErrorMessage: resultUpdate.TransactionResult.IsResultHold
					? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_SCORE_CVSDEFAUTH_ERROR)
					: string.Empty)
			{
				IsAuthResultHold = resultUpdate.TransactionResult.IsResultHold
			};
			return reauthActionResult;
		}

		/// <summary>
		/// 再与信アクションパラメタ
		/// </summary>
		public new class ReauthActionParams : IReauthActionParams
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="orderNew">注文情報（変更後）</param>
			public ReauthActionParams(OrderModel orderNew)
			{
				this.OrderNew = orderNew;
			}

			/// <summary>注文情報（変更後）</summary>
			public OrderModel OrderNew { get; }
		}
	}
}