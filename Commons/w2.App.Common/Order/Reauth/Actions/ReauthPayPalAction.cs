/*
=========================================================================================================
  Module      : 再与信アクション（PayPal与信）クラス(ReauthPayPalAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using Braintree;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.PayPal;
using w2.Domain.Order;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（PayPal与信）クラス
	/// </summary>
	public class ReauthPayPalAction : BaseReauthAction<ReauthPayPalAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReauthPayPalAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reauth, "PayPal決済与信", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// PayPal与信
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var order = reauthActionParams.Order;
			var userCreditCard = new UserCreditCard(new UserCreditCardService().Get(order.UserId, order.CreditBranchNo.Value));

			// 最終請求金額が0円の場合、エラーとする
			if (order.LastBilledAmount == 0)
			{
				return new ReauthActionResult(
					false,
					order.OrderId,
					string.Empty,
					apiErrorMessage: "最終請求金額が0円のため、与信できません。");
			}

			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);
			// 登録カード利用
			var result = PayPalUtility.Payment.PayWithCustomerId(
				paymentOrderId,
				userCreditCard.CooperationInfo.PayPalCustomerId,
				userCreditCard.CooperationInfo.PayPalDeviceData,
				order.LastBilledAmount,
				(Constants.PAYPAL_PAYMENT_METHOD == Constants.PayPalPaymentMethod.AUTH_WITH_SUBMIT),
				new PayPalUtility.AddressRequestWrapper(order));
			if (result.IsSuccess() == false)
			{
				var apiErrorMessage = "";
				foreach (ValidationError error in result.Errors.DeepAll())
				{
					apiErrorMessage += LogCreator.CreateErrorMessage(error.Code.ToString(), error.Message) + "\t";
				}

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
				CreatePaymentMemo(order.OrderPaymentKbn, paymentOrderId, result.Target.Id, order.LastBilledAmount),
				cardTranId: result.Target.Id,
				paymentOrderId: paymentOrderId,
				cardTranIdForLog: result.Target.Id);
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