/*
=========================================================================================================
  Module      : 再与信アクション（後付款(TriLink後払い)キャンセル）クラス(CancelTriLinkAfterPayAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.Domain.Order;
using w2.App.Common.Order.Payment.TriLinkAfterPay;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Request;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（後付款(TriLink後払い)キャンセル）クラス
	/// </summary>
	public class CancelTriLinkAfterPayAction : BaseReauthAction<CancelTriLinkAfterPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CancelTriLinkAfterPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "後付款(TriLink後払い)キャンセル", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 後付款(TriLink後払い)キャンセル処理
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var order = reauthActionParams.Order;
			var cancelResult = TriLinkAfterPayApiFacade.CancelOrder(
				new TriLinkAfterPayCancelRequest(order.CardTranId));
			if (cancelResult.ResponseResult == false)
			{
				var apiErrorMessage = cancelResult.ErrorCode + ":" + string.Join("\t", cancelResult.Errors);
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