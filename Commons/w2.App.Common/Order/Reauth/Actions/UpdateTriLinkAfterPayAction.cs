/*
=========================================================================================================
  Module      : 再与信アクション（後付款(TriLink後払い)注文情報更新）クラス(UpdateTriLinkAfterPayAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.Order.Payment.TriLinkAfterPay;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Request;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（後付款(TriLink後払い)注文情報更新）クラス
	/// </summary>
	public class UpdateTriLinkAfterPayAction : BaseReauthAction<UpdateTriLinkAfterPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UpdateTriLinkAfterPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Update, "後付款(TriLink後払い)注文情報更新", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 後付款(TriLink後払い)注文情報更新
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var order = reauthActionParams.Order;
			var updateResult = TriLinkAfterPayApiFacade.UpdateOrder(
				new TriLinkAfterPayUpdateRequest(order));
			if (updateResult.ResponseResult == false)
			{
				var apiErrorMessage = updateResult.ErrorCode + ":" + string.Join("\t", updateResult.Errors);
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: apiErrorMessage);
			}
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, order.PaymentOrderId, order.CardTranId, GetSendingAmount(order)),
				order.CardTranId,
				order.PaymentOrderId);
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
			public ReauthActionParams(OrderModel order)
			{
				this.Order = order;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報</summary>
			public OrderModel Order { get; private set; }
			#endregion
		}
	}
}