/*
=========================================================================================================
  Module      : 再与信アクション（NP後払い）クラス(ReauthNPAfterPayAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.NPAfterPay;
using w2.Domain.DeliveryCompany.Helper;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（NP後払い）クラス
	/// </summary>
	public class ReauthNPAfterPayAction : BaseReauthAction<ReauthNPAfterPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		public ReauthNPAfterPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reauth, "NP後払い与信", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// NP後払い
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderNew = reauthActionParams.OrderNew;
			var orderOld = reauthActionParams.OrderOld;

			// For modify order
			if (string.IsNullOrEmpty(orderNew.OrderIdOrg)
				&& (orderNew.OrderId == orderOld.OrderId))
			{
				// 最終請求金額が0円の場合、エラーとする
				if (orderNew.LastBilledAmount == 0)
				{
					return new ReauthActionResult(
						false,
						orderNew.OrderId,
						string.Empty,
						apiErrorMessage: "最終請求金額が0円のため、与信できません。");
				}
			}

			// Execute Payment
			return ExecutePayment(orderOld, orderNew);
		}

		/// <summary>
		/// Execute Payment
		/// </summary>
		/// <param name="orderOld">Order Old</param>
		/// <param name="orderNew">Order New</param>
		/// <returns>再与信アクション結果</returns>
		private ReauthActionResult ExecutePayment(
			OrderModel orderOld,
			OrderModel orderNew)
		{
			var formattedErrorMessage = string.Empty;
			var apiErrorMessage = string.Empty;
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(orderNew.ShopId);
			var isNeedUpdateInvoiceBundleFlgOff = (orderOld.OrderPaymentKbn == orderNew.OrderPaymentKbn)
				&& NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(orderOld.ExternalPaymentStatus);
			var isExchangeOrder = ((orderOld.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_UNKNOWN)
				&& (orderNew.ReturnExchangeKbn == Constants.FLG_ORDER_RETURN_EXCHANGE_KBN_EXCHANGE));

			var requestReauth = NPAfterPayUtility.CreateOrderRequestData(
				orderOld,
				paymentOrderId,
				false,
				isNeedUpdateInvoiceBundleFlgOff,
				orderNew);
			var resultReauth = NPAfterPayApiFacade.RegistOrder(requestReauth);
			if (resultReauth.IsSuccess == false)
			{
				formattedErrorMessage = resultReauth.GetApiErrorMessage(orderNew.IsExecuteReauthFromMyPage);
				return new ReauthActionResult(
					false,
					orderNew.OrderId,
					string.Empty,
					cardTranIdForLog: orderNew.CardTranId,
					apiErrorMessage: formattedErrorMessage);
			}
			if (resultReauth.IsAuthoriReviewOk == false)
			{
				var apiAuthoriErrorMessage = resultReauth.GetApiErrorMessage(orderNew.IsExecuteReauthFromMyPage);
				if (resultReauth.IsAuthoriReviewPending)
				{
					apiAuthoriErrorMessage = apiAuthoriErrorMessage
						.Replace("@@ 1 @@", orderNew.Owner.ConcatenateAddressWithoutCountryName())
						.Replace("@@ 2 @@", orderNew.Shippings[0].ConcatenateAddressWithoutCountryName());
				}
				var requestCancel = NPAfterPayUtility.CreateCancelOrGetPaymentRequestData(resultReauth.GetNPTransactionId());
				var resultCancel = NPAfterPayApiFacade.CancelOrder(requestCancel);
				if (resultCancel.IsSuccess == false)
				{
					formattedErrorMessage = resultCancel.GetApiErrorMessage(orderNew.IsExecuteReauthFromMyPage);
					var apiErrorMessageTmp = string.Format("{0}\r\n{1}",
						apiAuthoriErrorMessage,
						formattedErrorMessage);
					apiErrorMessage = apiErrorMessageTmp;
				}
				else
				{
					apiErrorMessage = apiAuthoriErrorMessage;
				}
				return new ReauthActionResult(
					false,
					orderNew.OrderId,
					string.Empty,
					cardTranIdForLog: orderNew.CardTranId,
					apiErrorMessage: apiErrorMessage);
			}

			// For case order old has shipment and order new has invoice bundle flag off
			if (((orderNew.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_OFF)
				|| isNeedUpdateInvoiceBundleFlgOff)
				&& (orderOld.OrderPaymentKbn == orderNew.OrderPaymentKbn)
				&& NPAfterPayUtility.CheckIfExternalPaymentStatusHasBeenPaid(orderOld.ExternalPaymentStatus)
				&& (isExchangeOrder == false))
			{
				// Execute Shipment
				var requestShipment = NPAfterPayUtility.CreateShipmentRequestData(
					resultReauth.GetNPTransactionId(),
					orderOld.Shippings[0].ShippingCheckNo,
					DeliveryCompanyUtil.GetDeliveryCompanyType(
						orderNew.Shippings[0].DeliveryCompanyId,
						orderNew.OrderPaymentKbn),
					string.Empty);
				var resultShipment = NPAfterPayApiFacade.ShipmentOrder(requestShipment);
				if (resultShipment.IsSuccess == false)
				{
					apiErrorMessage = resultShipment.GetApiErrorMessage(orderNew.IsExecuteReauthFromMyPage);
				}
			}
			return new ReauthActionResult(
				true,
				orderNew.OrderId,
				CreatePaymentMemo(
					orderNew.OrderPaymentKbn,
					paymentOrderId,
					resultReauth.GetNPTransactionId(),
					GetSendingAmount(orderNew)),
				paymentOrderId: paymentOrderId,
				cardTranId: resultReauth.GetNPTransactionId(),
				cardTranIdForLog: orderNew.CardTranId,
				apiErrorMessage: apiErrorMessage);
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