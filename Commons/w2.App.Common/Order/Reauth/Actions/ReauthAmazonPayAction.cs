/*
=========================================================================================================
  Module      : 再与信アクション（Amazon Pay与信）クラス(ReauthAmazonPayAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;
using w2.Domain.Order;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Util;
using w2.App.Common.Order.Payment;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（Amazon Pay与信）クラス
	/// </summary>
	public class ReauthAmazonPayAction : BaseReauthAction<ReauthAmazonPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReauthAmazonPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reauth, "Amazon Pay与信", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Amazon Pay与信
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var authAmount = orderNew.LastBilledAmount;

			// 変更前注文がAmazon Payかつ増額かつオンライン決済ステータスが「売上確定済」の場合増額分で与信
			if ((orderOld.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT)
				&& (orderNew.LastBilledAmount > orderOld.LastBilledAmount)
				&& (orderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED))
			{
				authAmount = orderNew.LastBilledAmount - orderOld.LastBilledAmount;
			}

			var result = ExecAuthAmazonPay(orderOld, orderNew, authAmount);

			if (result.Result)
			{
				orderNew.CardTranId = result.CardTranId;
				orderNew.PaymentOrderId = result.PaymentOrderId;
			}

			return result;
		}

		/// <summary>
		/// Amazon Pay与信処理
		/// </summary>
		/// <param name="orderOld">注文情報(変更前)</param>
		/// <param name="orderNew">注文情報(変更後)</param>
		/// <param name="authAmount">与信金額</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthAmazonPay(OrderModel orderOld, OrderModel orderNew, decimal authAmount)
		{
			var aut = AmazonApiFacade.Authorize(
				orderNew.PaymentOrderId,
				authAmount,
				orderNew.OrderId + "_" + DateTime.Now.ToString("HHmmssfff"),
				false);
			if ((aut.GetSuccess() == false) || (aut.GetAuthorizationState() == AmazonConstants.State.Declined.ToString()))
			{
				var errorMessage = (aut.GetSuccess() == false)
					? LogCreator.CreateErrorMessage(aut.GetErrorCode(), aut.GetErrorMessage())
					: "何らかの理由により与信が正常に終了しませんでした。";

				// 元注文が仮売り状態だった場合同額で与信を取り直す
				if (orderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_NONE)
				{
					var reauth = AmazonApiFacade.Authorize(
					orderOld.PaymentOrderId,
					orderOld.LastBilledAmount,
					orderOld.OrderId + "_" + DateTime.Now.ToString("HHmmssfff"),
					false);

					return new ReauthActionResult(
					false,
					orderOld.OrderId,
					CreatePaymentMemo(orderOld.OrderPaymentKbn, orderOld.PaymentOrderId, reauth.GetAuthorizationId(), orderOld.LastBilledAmount),
					reauth.GetAuthorizationId(),
					orderOld.PaymentOrderId,
					reauth.GetAuthorizationId(),
					errorMessage);
				}

				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					CreatePaymentMemo(orderOld.OrderPaymentKbn, orderOld.PaymentOrderId, aut.GetAuthorizationId(), orderOld.LastBilledAmount),
					paymentOrderId: orderOld.PaymentOrderId,
					apiErrorMessage: errorMessage);
			}

			return new ReauthActionResult(
				true,
				orderNew.OrderId,
				CreatePaymentMemo(orderNew.OrderPaymentKbn, orderNew.PaymentOrderId, aut.GetAuthorizationId(), authAmount),
				aut.GetAuthorizationId(),
				orderNew.PaymentOrderId,
				aut.GetAuthorizationId());
		}
		#endregion

		#region プロパティ
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
