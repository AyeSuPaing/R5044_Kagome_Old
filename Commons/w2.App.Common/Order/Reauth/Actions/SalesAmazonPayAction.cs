/*
=========================================================================================================
  Module      : 再与信アクション（Amazon Pay売上確定）クラス(SalesAmazonPayAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Net;
using w2.App.Common.Amazon;
using w2.App.Common.Amazon.Util;
using w2.App.Common.Order.Payment;
using w2.Common.Logger;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 再与信アクション（Amazon Pay売上確定）クラス
	/// </summary>
	public class SalesAmazonPayAction : BaseReauthAction<SalesAmazonPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SalesAmazonPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Sales, "Amazon Pay売上確定", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Amazon Pay売上確定
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			ReauthActionResult result = null;
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

			result = ExecSalesAmazonPay(orderNew, salesAmount);

			// 決済カード取引IDをセット
			if (result.Result)
			{
				orderNew.CardTranId = result.CardTranId;
				orderNew.PaymentOrderId = result.PaymentOrderId;
			}

			return result;
		}

		/// <summary>
		/// Amazon Pay売上確定処理
		/// </summary>
		/// <param name="orderNew">注文情報</param>
		/// <param name="salesAmount">売上確定金額</param>
		/// <returns>売上確定結果情報</returns>
		private ReauthActionResult ExecSalesAmazonPay(OrderModel orderNew, decimal salesAmount)
		{
			try
			{
				var capture = AmazonApiFacade.Capture(
					orderNew.CardTranId,
					salesAmount,
					orderNew.OrderId + "_" + DateTime.Now.ToString("HHmmssfff"));
				if (capture.GetSuccess() == false)
				{
					return new ReauthActionResult(
						false,
						orderNew.OrderId,
						string.Empty,
						cardTranIdForLog: orderNew.CardTranId,
						apiErrorMessage: LogCreator.CreateErrorMessage(capture.GetErrorCode(), capture.GetErrorMessage()));
				}

				return new ReauthActionResult(
					true,
					orderNew.OrderId,
					CreatePaymentMemo(orderNew.OrderPaymentKbn, orderNew.PaymentOrderId, capture.GetCaptureId(), orderNew.LastBilledAmount),
					cardTranId: capture.GetCaptureId(),
					cardTranIdForLog: capture.GetCaptureId());
			}
			catch (WebException webEx)
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT,
					PaymentFileLogger.PaymentType.Amazon,
					PaymentFileLogger.PaymentProcessingType.RealSalesProcessing,
					BaseLogger.CreateExceptionMessage(
						"Amazon Pay実売上処理失敗エラー" + "[受注ID : " + orderNew.OrderId + "]",
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
