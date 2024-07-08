/*
=========================================================================================================
  Module      : Sales Atone Action(SalesAtoneAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Net;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Atone;
using w2.Common.Logger;
using w2.Domain.Order;
using w2.Domain.User;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Sales Atone Action
	/// </summary>
	public class SalesAtoneAction : BaseReauthAction<SalesAtoneAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SalesAtoneAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Sales, "atone翌月払い売上確定", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Atone 売上確定
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
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

			var result = ExecSalesAtone(orderNew, salesAmount);

			// 決済カード取引IDをセット
			if (result.Result)
			{
				orderNew.CardTranId = result.CardTranId;
				orderNew.PaymentOrderId = result.PaymentOrderId;
			}
			return result;
		}

		/// <summary>
		/// Exec Sales Atone
		/// </summary>
		/// <param name="orderNew">注文情報</param>
		/// <param name="salesAmount">売上確定金額</param>
		/// <returns>売上確定結果情報</returns>
		private ReauthActionResult ExecSalesAtone(OrderModel orderNew, decimal salesAmount)
		{
			try
			{
				var salesLastBilledAmount = CurrencyManager.GetSettlementAmount(
					salesAmount,
					orderNew.SettlementRate,
					orderNew.SettlementCurrency);
				var user = new UserService().Get(orderNew.UserId);
				var tokenId = ((user != null)
					? user.UserExtend.UserExtendDataValue[Constants.FLG_USEREXTEND_USREX_ATONE_TOKEN_ID]
					: string.Empty);

				if (string.IsNullOrEmpty(tokenId) == false)
				{
					var responeCaptureAtone = AtonePaymentApiFacade.CapturePayment(tokenId, orderNew.CardTranId);
					if (responeCaptureAtone.IsSuccess == false)
					{
						var captureAtoneErrorCodes = responeCaptureAtone.Errors.Select(item => item.Code);
						return new ReauthActionResult(
							false,
							orderNew.OrderId,
							string.Empty,
							cardTranIdForLog: orderNew.CardTranId,
							apiErrorMessage: LogCreator.CreateErrorMessage(
								string.Join(",", captureAtoneErrorCodes),
								responeCaptureAtone.Message));
					}
					if (responeCaptureAtone.IsAuthorizationSuccess == false)
					{
						var result = new ReauthActionResult(
							false,
							orderNew.OrderId,
							string.Empty,
							cardTranIdForLog: orderNew.CardTranId,
							apiErrorMessage: responeCaptureAtone.AuthorizationResultNgReasonMessage);
						return result;
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
			}
			catch (WebException webEx)
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_ATONE,
					PaymentFileLogger.PaymentType.Atone,
					PaymentFileLogger.PaymentProcessingType.RealSalesProcessing,
					BaseLogger.CreateExceptionMessage(
						"Atone実売上処理失敗エラー" + "[受注ID : " + orderNew.OrderId + "]",
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
