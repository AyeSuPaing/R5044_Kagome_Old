/*
=========================================================================================================
  Module      : Cancel Aftee Action(CancelAfteeAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Aftee;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Cancel Aftee Action
	/// </summary>
	public class CancelAfteeAction : BaseReauthAction<CancelAfteeAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CancelAfteeAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "aftee翌月払いキャンセル", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// On Execute
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var result = ExecCancelAftee(orderOld, reauthActionParams.ReauthType);
			return result;
		}

		/// <summary>
		/// Exec Cancel Atone
		/// </summary>
		/// <param name="orderOld">Order Old</param>
		/// <param name="reauthTypes">Reauth Types</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelAftee(OrderModel orderOld, ReauthCreatorFacade.ReauthTypes reauthTypes)
		{
			// Check transaction status
			var response = AfteePaymentApiFacade.GetPayment(orderOld.CardTranId);

			var cancelLastBilledAmount = CurrencyManager.GetSettlementAmount(
				orderOld.LastBilledAmount,
				orderOld.SettlementRate,
				orderOld.SettlementCurrency);
			// Not refund when already refund
			if (response.Refunds == null)
			{
				var requestCancelAftee = new AfteeRefundPaymentRequest()
					{
						AmountRefund = cancelLastBilledAmount.ToString("0"),
						DescriptionRefund = string.Empty,
						RefundReason = GetRefundReason(reauthTypes)
					};
				var responeCancelAftee = AfteePaymentApiFacade.RefundPayment(orderOld.CardTranId, requestCancelAftee);

				if (responeCancelAftee.IsSuccess == false)
				{
					var afteeRefundErrorCodes = responeCancelAftee.Errors.Select(item => item.Code);
					return new ReauthActionResult(
						false,
						orderOld.OrderId,
						string.Empty,
						cardTranIdForLog: orderOld.CardTranId,
						apiErrorMessage: LogCreator.CreateErrorMessage(
							string.Join(",", afteeRefundErrorCodes),
							responeCancelAftee.Message));
				}
			}
			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					orderOld.OrderId,
					orderOld.CardTranId,
					cancelLastBilledAmount),
				cardTranId: orderOld.CardTranId,
				cardTranIdForLog: orderOld.CardTranId);
		}

		/// <summary>
		/// Get Refund Reason
		/// </summary>
		/// <param name="reauthTypes">Reauth Types</param>
		/// <returns>Reason</returns>
		private string GetRefundReason(ReauthCreatorFacade.ReauthTypes reauthTypes)
		{
			switch (reauthTypes)
			{
				case ReauthCreatorFacade.ReauthTypes.ChangeAfteeToCredit:
				case ReauthCreatorFacade.ReauthTypes.ChangeAfteeToAmazonPay:
				case ReauthCreatorFacade.ReauthTypes.ChangeAfteeToCvsDef:
				case ReauthCreatorFacade.ReauthTypes.ChangeAfteeToCollectOrOthers:
				case ReauthCreatorFacade.ReauthTypes.ChangeAfteeToCarrier:
				case ReauthCreatorFacade.ReauthTypes.ChangeAfteeToPayPal:
				case ReauthCreatorFacade.ReauthTypes.ChangeAfteeToTriLinkAfterPay:
				case ReauthCreatorFacade.ReauthTypes.ChangeAfteeToNoPayment:
					return "お支払い方法変更";

				case ReauthCreatorFacade.ReauthTypes.AfteeReturnAllItems:
					return "返品";

				case ReauthCreatorFacade.ReauthTypes.NoChangeAftee:
					return string.Empty;

				default:
					return "キャンセル";
			}
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
			/// <param name="reauthType">Reauth Type</param>
			public ReauthActionParams(OrderModel orderOld, ReauthCreatorFacade.ReauthTypes reauthType)
			{
				this.OrderOld = orderOld;
				this.ReauthType = reauthType;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報</summary>
			public OrderModel OrderOld { get; private set; }
			/// <summary>Reauth Type</summary>
			public ReauthCreatorFacade.ReauthTypes ReauthType { get; private set; }
			#endregion
		}
	}
}
