/*
=========================================================================================================
  Module      : 再与信アクション（後付款(TriLink後払い)与信）クラス(ReauthTriLinkAfterPayAction.cs)
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
	/// 再与信アクション（後付款(TriLink後払い)与信）クラス
	/// </summary>
	public class ReauthTriLinkAfterPayAction : BaseReauthAction<ReauthTriLinkAfterPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReauthTriLinkAfterPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reauth, "後付款(TriLink後払い)与信", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 後付款(TriLink後払い)与信
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var order = reauthActionParams.Order;
			var result = ExecAuthTriLinkAfterPay(order);
			if (result.Result)
			{
				order.PaymentOrderId = result.PaymentOrderId;
				order.CardTranId = result.CardTranId;
			}
			return result;
		}

		/// <summary>
		/// 後付款(TriLink後払い)与信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthTriLinkAfterPay(OrderModel order)
		{
			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);
			
			var cardTranId = string.Empty;

			// 注文審査
			var authRequest = new TriLinkAfterPayRegisterRequest(order, paymentOrderId);
			var authResponse = TriLinkAfterPayApiFacade.RegisterOrder(authRequest);
			if ((authResponse.ResponseResult)
				&& (authResponse.Authorization.Result == TriLinkAfterPayConstants.FLG_TW_AFTERPAY_AUTH_OK))
			{
				cardTranId = authResponse.OrderCode;
			}
			else
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					cardTranIdForLog: order.CardTranId,
					apiErrorMessage: (authResponse.IsHttpStatusCodeBadRequest)
						? authResponse.Errors.First().Message
						: authResponse.Message);
			}
			return new ReauthActionResult(
				true,
				order.OrderId,
				CreatePaymentMemo(order.OrderPaymentKbn, paymentOrderId, cardTranId, GetSendingAmount(order)),
				cardTranId,
				paymentOrderId);
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
			public ReauthActionParams(OrderModel order, string httpHeader = null)
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