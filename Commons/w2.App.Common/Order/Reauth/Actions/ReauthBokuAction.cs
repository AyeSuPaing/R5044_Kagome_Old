/*
=========================================================================================================
  Module      : Reauth Boku Action(ReauthBokuAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Boku;
using w2.App.Common.Order.Payment.Boku.Utils;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Reauth Boku Action
	/// </summary>
	public class ReauthBokuAction : BaseReauthAction<ReauthBokuAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">Reauth action params</param>
		public ReauthBokuAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reauth, "Boku与信", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Reauth Boku
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;

			var result = ExecReauthBoku(orderOld, orderNew);

			return result;
		}

		/// <summary>
		/// Exec Reauth boku
		/// </summary>
		/// <param name="orderOld">Order Old</param>
		/// <param name="orderNew">Order New</param>
		/// <returns>返金結果情報</returns>
		private ReauthActionResult ExecReauthBoku(OrderModel orderOld, OrderModel orderNew)
		{
			var hasError = false;
			var message = string.Empty;
			var productNames = string.Join(
				",",
				orderNew.Items.Select(item => item.ProductName));
			var charge = new PaymentBokuChargeApi().Exec(
				orderNew.SettlementCurrency,
				string.Empty,
				productNames,
				orderNew.OrderId,
				orderNew.PaymentOrderId,
				orderNew.SettlementAmount.ToString(),
				(orderNew.OrderTaxIncludedFlg == Constants.FLG_ORDER_ORDER_TAX_INCLUDED_PRETAX),
				orderNew.RemoteAddr,
				orderNew.IsFixedPurchaseOrder,
				(orderNew.IsFixedPurchaseOrder && (orderNew.FixedPurchaseOrderCount > 1)),
				(orderNew.IsFixedPurchaseOrder ? orderNew.FixedPurchaseKbn : string.Empty),
				(orderNew.IsFixedPurchaseOrder ? orderNew.FixedPurchaseSetting1 : string.Empty));

			if (charge == null)
			{
				hasError = true;
				message = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_BOKU_PAYMENT_ERROR);
			}
			else if ((charge.IsSuccess == false)
				|| (charge.ChargeStatus != BokuConstants.CONST_BOKU_CHARGE_STATUS_SUCCESS))
			{
				hasError = true;
				message = (charge.ChargeStatus == BokuConstants.CONST_BOKU_CHARGE_STATUS_IN_PROGRESS)
					? CommerceMessages.GetMessages(CommerceMessages.ERRMSG_BOKU_PAYMENT_PROCESS_TIME_OUT)
					: charge.Result.Message;
			}

			if (hasError)
			{
				return new ReauthActionResult(
					false,
					orderOld.OrderId,
					string.Empty,
					cardTranIdForLog: orderOld.CardTranId,
					apiErrorMessage: LogCreator.CreateErrorMessage(
						string.Empty,
						message));
			}

			// 決済取引ID、決済注文IDは更新する必要がないので元注文のものをそのまま入れる
			return new ReauthActionResult(
				true,
				orderOld.OrderId,
				CreatePaymentMemo(
					orderOld.OrderPaymentKbn,
					orderOld.PaymentOrderId,
					charge.ChargeId,
					orderNew.LastBilledAmount),
				paymentOrderId: orderOld.PaymentOrderId,
				cardTranId: charge.ChargeId,
				cardTranIdForLog: charge.ChargeId);
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
