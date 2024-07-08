/*
=========================================================================================================
  Module      : 再与信アクション（Amazon Payキャンセル）クラス(CancelAmazonPayAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Domain.Order;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Util;
using w2.App.Common.Order.Payment;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（Amazon Payキャンセル）クラス
	/// </summary>
	public class CancelAmazonPayAction : BaseReauthAction<CancelAmazonPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CancelAmazonPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "Amazon Payキャンセル", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Amazon Payキャンセル
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var result = ExecCancelAmazonPay(orderOld);
			return result;
		}

		/// <summary>
		/// Amazon Payキャンセル処理(オーソリクローズ)
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelAmazonPay(OrderModel orderOld)
		{
			// オンライン決済ステータスが「売上確定済」の場合は全額返金処理
			if (orderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
			{
				var refund = AmazonApiFacade.Refund(orderOld.CardTranId, orderOld.LastBilledAmount, orderOld.OrderId + "_" + DateTime.Now.ToString("HHmmssfff"));
				if (refund.GetSuccess() == false)
				{
					return new ReauthActionResult(
						false,
						orderOld.OrderId,
						string.Empty,
						cardTranIdForLog: refund.GetAmazonRefundId(),
						apiErrorMessage: LogCreator.CreateErrorMessage(refund.GetErrorCode(), refund.GetErrorMessage()));
				}

				return new ReauthActionResult(
					true,
					orderOld.OrderId,
					CreatePaymentMemo(orderOld.OrderPaymentKbn, orderOld.PaymentOrderId, refund.GetAmazonRefundId(), orderOld.LastBilledAmount),
					cardTranId: refund.GetAmazonRefundId(),
					cardTranIdForLog: refund.GetAmazonRefundId());
			}

			// オンライン決済ステータスが「未連携」の場合はオーソリクローズ処理
			var close = AmazonApiFacade.CloseAuthorization(orderOld.CardTranId);
			if (close.GetSuccess() == false)
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(close.GetErrorCode(), close.GetErrorMessage()));
			}

			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(orderOld.OrderPaymentKbn, orderOld.PaymentOrderId, orderOld.CardTranId, orderOld.LastBilledAmount),
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
