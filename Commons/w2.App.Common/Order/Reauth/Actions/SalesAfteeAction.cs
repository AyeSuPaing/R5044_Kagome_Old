/*
=========================================================================================================
  Module      : Sales Aftee Action(SalesAfteeAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Net;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Aftee;
using w2.Common.Logger;
using w2.Domain.Order;
using w2.Domain.User;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Sales Aftee Action
	/// </summary>
	public class SalesAfteeAction : BaseReauthAction<SalesAfteeAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SalesAfteeAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Sales, "aftee翌月払い売上確定", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Aftee 売上確定
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var salesAmount = orderNew.LastBilledAmount;

			// 最終請求金額が0円の場合、エラーとする
			if (orderNew.LastBilledAmount == 0)
			{
				return new ReauthActionResult(
					false,
					orderNew.OrderId,
					string.Empty,
					apiErrorMessage: "最終請求金額が0円のため、売上確定できません。");
			}

			// 注文変更前のオンライン決済ステータスが「売上確定済」かつ増額の場合
			if ((orderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED)
				&& (orderOld.LastBilledAmount < orderNew.LastBilledAmount))
			{
				salesAmount -= orderOld.LastBilledAmount;
			}

			var result = ExecSalesAftee(orderNew, salesAmount);

			// 決済カード取引IDをセット
			if (result.Result)
			{
				orderNew.CardTranId = result.CardTranId;
				orderNew.PaymentOrderId = result.PaymentOrderId;
			}
			return result;
		}

		/// <summary>
		/// Exec Sales Aftee
		/// </summary>
		/// <param name="orderNew">注文情報</param>
		/// <param name="salesAmount">売上確定金額</param>
		/// <returns>売上確定結果情報</returns>
		private ReauthActionResult ExecSalesAftee(OrderModel orderNew, decimal salesAmount)
		{
			try
			{
				var salesLastBilledAmount = CurrencyManager.GetSettlementAmount(
					salesAmount,
					orderNew.SettlementRate,
					orderNew.SettlementCurrency);
				var user = new UserService().Get(orderNew.UserId, this.Accessor);
				var tokenId = ((user != null)
					? user.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_AFTEE_TOKEN_ID]
					: string.Empty);

				if (string.IsNullOrEmpty(tokenId) == false)
				{
					var responeCaptureAftee = AfteePaymentApiFacade.CapturePayment(tokenId, orderNew.CardTranId);
					if (responeCaptureAftee.IsSuccess == false)
					{
						var captureAfteeErrorCodes = responeCaptureAftee.Errors.Select(item => item.Code);
						return new ReauthActionResult(
							false,
							orderNew.OrderId,
							string.Empty,
							cardTranIdForLog: orderNew.CardTranId,
							apiErrorMessage: LogCreator.CreateErrorMessage(
								string.Join(",", captureAfteeErrorCodes),
								responeCaptureAftee.Message));
					}
				}
				return new ReauthActionResult(
					true,
					orderNew.OrderId,
					CreatePaymentMemo(
						orderNew.OrderPaymentKbn,
						orderNew.OrderId,
						orderNew.CardTranId,
						salesLastBilledAmount),
					cardTranId: orderNew.CardTranId,
					cardTranIdForLog: orderNew.CardTranId);
			}
			catch (WebException webEx)
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_AFTEE,
					PaymentFileLogger.PaymentType.Aftee,
					PaymentFileLogger.PaymentProcessingType.RealSalesProcessing,
					BaseLogger.CreateExceptionMessage(
						"Aftee実売上処理失敗エラー" + "[受注ID : " + orderNew.OrderId + "]",
						webEx));
			}
			// エラー結果返す
			return new ReauthActionResult(
				false,
				orderNew.OrderId,
				string.Empty,
				cardTranIdForLog: orderNew.CardTranId,
				apiErrorMessage: "カード実売上処理失敗エラー");
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
