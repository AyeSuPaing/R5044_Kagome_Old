/*
=========================================================================================================
  Module      : 再与信アクション（Amazon Pay返金）クラス(RefundAmazonPayAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Util;
using w2.App.Common.Order.Payment;
using w2.Domain.Order;
namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（Amazon Pay返金）クラス
	/// </summary>
	public class RefundAmazonPayAction : BaseReauthAction<RefundAmazonPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RefundAmazonPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Refund, "Amazon Pay返金", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Amazon Pay返金
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
		/// <param name="order">注文情報</param>
		/// <returns>返金結果情報</returns>
		private ReauthActionResult ExecRefundAmazonPay(OrderModel orderOld, decimal refundAmount)
		{
			var response = AmazonApiFacade.Refund(orderOld.CardTranId, refundAmount, orderOld.OrderId + "_" + DateTime.Now.ToString("HHmmssfff"));
			if (response.GetSuccess() == false)
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: response.GetAmazonRefundId(),
					apiErrorMessage: LogCreator.CreateErrorMessage(response.GetErrorCode(), response.GetErrorMessage()));
			}

			// 決済取引ID、決済注文IDは更新する必要がないので元注文のものをそのまま入れる
			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(orderOld.OrderPaymentKbn, orderOld.PaymentOrderId, response.GetAmazonRefundId(), refundAmount),
				cardTranId: orderOld.CardTranId,
				paymentOrderId: orderOld.PaymentOrderId,
				cardTranIdForLog: response.GetAmazonRefundId());
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
