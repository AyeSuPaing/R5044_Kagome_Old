/*
=========================================================================================================
  Module      : Sales Line Pay Action(SalesLinePayAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.LinePay;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Sales Line Pay Action
	/// </summary>
	public class SalesLinePayAction : BaseReauthAction<SalesLinePayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">Reauth action params</param>
		public SalesLinePayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Sales, "LINEPay決済売上確定", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// LINE Pay 売上確定
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderNew = reauthActionParams.OrderNew;
			var salesAmount = orderNew.SettlementAmount;

			// 最終請求金額が0円の場合、エラーとする
			if (salesAmount == 0)
			{
				return new ReauthActionResult(
					false,
					orderNew.OrderId,
					string.Empty,
					apiErrorMessage: "最終請求金額が0円のため、売上確定できません。");
			}

			var response = LinePayApiFacade.CapturePayment(
				orderNew.CardTranId,
				salesAmount,
				orderNew.SettlementCurrency,
				new LinePayApiFacade.LinePayLogInfo(orderNew));
			if (response.IsSuccess == false)
			{
				return new ReauthActionResult(
					false,
					orderNew.OrderId,
					string.Empty,
					cardTranIdForLog: orderNew.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(response.ReturnCode, response.ReturnCode));
			}

			// Update card trand id
			orderNew.CardTranId = response.Info.TransactionId;

			return new ReauthActionResult(
				true,
				orderNew.OrderId,
				CreatePaymentMemo(
					orderNew.OrderPaymentKbn,
					orderNew.PaymentOrderId,
					orderNew.CardTranId,
					salesAmount),
				cardTranId: orderNew.CardTranId,
				paymentOrderId: orderNew.PaymentOrderId,
				cardTranIdForLog: orderNew.CardTranId);
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
			/// <param name="orderNew">Order New</param>
			public ReauthActionParams(OrderModel orderNew)
			{
				this.OrderNew = orderNew;
			}
			#endregion

			#region プロパティ
			/// <summary>Order New</summary>
			public OrderModel OrderNew { get; private set; }
			#endregion
		}
	}
}
