/*
=========================================================================================================
  Module      : 再与信アクション（ベリトランス後払い注文情報更新）クラス(UpdateVeritransAfterPayAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Order.Payment.Veritrans;
using w2.Common.Helper;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（ベリトランス後払い注文情報更新）クラス
	/// </summary>
	public class UpdateVeritransAfterPayAction : BaseReauthAction<UpdateVeritransAfterPayAction.ReauthActionParams>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		public UpdateVeritransAfterPayAction(OrderModel order)
			: base(ActionTypes.Update, "ベリトランス後払い注文情報更新", new ReauthActionParams(order))
		{
		}

		/// <summary>
		/// ベリトランス後払い注文情報更新
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var resultUpdate = new PaymentVeritransCvsDef().OrderModify(reauthActionParams.OrderNew);
			ReauthActionResult reauthActionResult;
			if ((resultUpdate == null)
				|| (resultUpdate.Mstatus != VeriTransConst.RESULT_STATUS_OK)
				|| (resultUpdate.AuthorResult == VeriTransConst.VeritransAuthorResult.Ng.ToText()))
			{
				var errorMessage = resultUpdate?.Errors != null
					? string.Join("\r\n", resultUpdate.Errors.Select(e => $"{e.ErrorCode}：{e.ErrorMessage}").ToArray())
					: CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_VERITRANS_PAYMENT_CHANGE_ERROR);

				reauthActionResult = new ReauthActionResult(
					result: false,
					orderId: reauthActionParams.OrderNew.OrderId,
					paymentMemo: string.Empty,
					cardTranId: reauthActionParams.OrderNew.CardTranId,
					paymentOrderId: reauthActionParams.OrderNew.OrderId,
					cardTranIdForLog: reauthActionParams.OrderNew.CardTranId,
					apiErrorMessage: errorMessage);
				return reauthActionResult;
			}

			reauthActionResult = new ReauthActionResult(
				result: true,
				orderId: reauthActionParams.OrderNew.OrderId,
				paymentMemo: CreatePaymentMemo(
					reauthActionParams.OrderNew.OrderPaymentKbn,
					reauthActionParams.OrderNew.PaymentOrderId,
					resultUpdate.CustTxn,
					reauthActionParams.OrderNew.OrderPriceTotal),
				cardTranId: resultUpdate.CustTxn,
				reauthActionParams.OrderNew.PaymentOrderId,
				cardTranIdForLog: reauthActionParams.OrderNew.CardTranId,
				apiErrorMessage: resultUpdate.AuthorResult == VeriTransConst.VeritransAuthorResult.Hold.ToText()
					? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_VERITRANS_CVSDEFAUTH_ERROR)
					: string.Empty)
			{
				IsAuthResultHold = (resultUpdate.AuthorResult == VeriTransConst.VeritransAuthorResult.Hold.ToText())
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
