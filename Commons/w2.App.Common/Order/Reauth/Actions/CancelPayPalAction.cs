/*
=========================================================================================================
  Module      : 再与信アクション（PayPalキャンセル）クラス(CancelPayPalAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.Order.Payment;
using w2.Domain.Order;
using w2.App.Common.Order.Payment.PayPal;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（PayPalキャンセル）クラス
	/// </summary>
	public class CancelPayPalAction : BaseReauthAction<CancelPayPalAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CancelPayPalAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "PayPal決済キャンセル", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// PayPalキャンセル
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var order = reauthActionParams.Order;

			var doRefund = ((Constants.PAYPAL_PAYMENT_METHOD == Constants.PayPalPaymentMethod.AUTH_WITH_SUBMIT)
				|| (order.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				|| order.IsDigitalContents);
			var result = PayPalUtility.Payment.VoidOrRefund(order.CardTranId, doRefund);
			if (result.IsSuccess() == false)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: string.Join("\t",result.Errors.DeepAll().Select(error => LogCreator.CreateErrorMessage(error.Code.ToString(),error.Message))));
			}

			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn,order.PaymentOrderId, order.CardTranId, order.LastBilledAmount),
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