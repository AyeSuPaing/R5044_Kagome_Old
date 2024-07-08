/*
=========================================================================================================
  Module      : Cancel Atone Action(CancelAtoneAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Atone;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Cancel Atone Action
	/// </summary>
	public class CancelAtoneAction : BaseReauthAction<CancelAtoneAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CancelAtoneAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Cancel, "atone翌月払いキャンセル", reauthActionParams)
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
			var result = ExecCancelAtone(orderOld, reauthActionParams.ReauthType);
			return result;
		}

		/// <summary>
		/// Exec Cancel Atone
		/// </summary>
		/// <param name="orderOld">Order Old</param>
		/// <param name="reauthTypes">Reauth Types</param>
		/// <returns>キャンセル結果情報</returns>
		private ReauthActionResult ExecCancelAtone(OrderModel orderOld, ReauthCreatorFacade.ReauthTypes reauthTypes)
		{
			var cancelLastBilledAmount = CurrencyManager.GetSettlementAmount(
				orderOld.LastBilledAmount,
				orderOld.SettlementRate,
				orderOld.SettlementCurrency);
			var requestCancelAtone = new AtoneRefundPaymentRequest()
				{
					AmountRefund = cancelLastBilledAmount.ToString("0"),
					DescriptionRefund = string.Empty,
					RefundReason = GetRefundReason(reauthTypes)
				};
			var responeCancelAtone = AtonePaymentApiFacade.RefundPayment(orderOld.CardTranId, requestCancelAtone);

			if (responeCancelAtone.IsSuccess == false)
			{
				var cancelAtoneErrorCodes = responeCancelAtone.Errors.Select(item => item.Code).ToArray();
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(
						string.Join(",", cancelAtoneErrorCodes),
						responeCancelAtone.Message));
			}
			if (responeCancelAtone.IsAuthorizationSuccess == false)
			{
				var result = new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: responeCancelAtone.AuthorizationResultNgReasonMessage);
				return result;
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
				case ReauthCreatorFacade.ReauthTypes.ChangeAtoneToCredit:
				case ReauthCreatorFacade.ReauthTypes.ChangeAtoneToAmazonPay:
				case ReauthCreatorFacade.ReauthTypes.ChangeAtoneToCvsDef:
				case ReauthCreatorFacade.ReauthTypes.ChangeAtoneToCollectOrOthers:
				case ReauthCreatorFacade.ReauthTypes.ChangeAtoneToCarrier:
				case ReauthCreatorFacade.ReauthTypes.ChangeAtoneToPayPal:
				case ReauthCreatorFacade.ReauthTypes.ChangeAtoneToTriLinkAfterPay:
				case ReauthCreatorFacade.ReauthTypes.ChangeAtoneToNoPayment:
					return "お支払い方法変更";

				case ReauthCreatorFacade.ReauthTypes.AtoneReturnAllItems:
					return "返品";

				case ReauthCreatorFacade.ReauthTypes.NoChangeAtone:
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
