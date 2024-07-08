/*
=========================================================================================================
  Module      : Sales Paidy Pay Action (SalesPaidyPayAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.Paidy;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Sales Paidy Pay Action
	/// </summary>
	public class SalesPaidyPayAction : BaseReauthAction<SalesPaidyPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		public SalesPaidyPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Sales, "Paidy決済売上確定", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Paidy決済売上確定
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderNew = reauthActionParams.OrderNew;

			// 最終請求金額が0円の場合、エラーとする
			if (orderNew.LastBilledAmount == 0)
			{
				return new ReauthActionResult(
					false,
					orderNew.OrderId,
					string.Empty,
					apiErrorMessage: "最終請求金額が0円のため、売上確定できません。");
			}

			// Capture Paidy payment
			var captureResponse = PaidyPaymentApiFacade.CapturePayment(orderNew.PaymentOrderId);
			if (captureResponse.HasError)
			{
				return new ReauthActionResult(
					false,
					orderNew.OrderId,
					string.Empty,
					cardTranIdForLog: string.Empty,
					apiErrorMessage: captureResponse.GetApiErrorMessages());
			}

			// Update card tran id and payment order id for order new
			orderNew.CardTranId = captureResponse.Payment.Captures[0].Id;
			orderNew.PaymentOrderId = captureResponse.Payment.Id;

			return new ReauthActionResult(
				true,
				orderNew.OrderId,
				CreatePaymentMemo(
					orderNew.OrderPaymentKbn,
					captureResponse.Payment.Id,
					captureResponse.Payment.Captures[0].Id,
					orderNew.LastBilledAmount),
				cardTranId: captureResponse.Payment.Captures[0].Id,
				paymentOrderId: captureResponse.Payment.Id,
				cardTranIdForLog: captureResponse.Payment.Captures[0].Id);
		}

		/// <summary>
		/// 決済注文ID、取引ID更新
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="cardTranId">取引ID</param>
		public override void UpdateReauthInfo(string paymentOrderId, string cardTranId)
		{
			((ReauthActionParams)base.ReauthActionParams).OrderNew.PaymentOrderId = paymentOrderId;
			((ReauthActionParams)base.ReauthActionParams).OrderNew.CardTranId = cardTranId;
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
			/// <param name="orderNew">注文情報(変更後)</param>
			public ReauthActionParams(OrderModel orderNew)
			{
				this.OrderNew = orderNew;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報(変更後)</summary>
			public OrderModel OrderNew { get; private set; }
			#endregion
		}
	}
}
