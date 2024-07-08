/*
=========================================================================================================
  Module      : Payment Zeus Cvs Result Receiver (PaymentZeusCvsResultReceiver.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web;
using w2.Common.Logger;
using w2.Domain;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Payment.Zeus
{
	/// <summary>
	/// Payment Zeus Cvs Result Receiver
	/// </summary>
	public class PaymentZeusCvsResultReceiver
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="request">Request</param>
		public PaymentZeusCvsResultReceiver(HttpRequest request)
		{
			Receive(request);
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="request">Request</param>
		private void Receive(HttpRequest request)
		{
			this.ZeusOrderId = request[PaymentZeusCvs.RESPONSE_PAYMENT_ORDER_NO];
			this.OrderId = request[PaymentZeusCvs.REQUEST_PAYMENT_SEND_ID];
			this.Status = request[PaymentZeusCvs.RESPONSE_PAYMENT_PAY_STATUS];
			this.PayDate = DateTime.Now;
		}

		/// <summary>
		/// Execute
		/// </summary>
		/// <returns>True: If update status for payment is success</returns>
		public bool Exec()
		{
			try
			{
				var order = OrderCommon.GetOrder(this.OrderId);
				if ((order.Count == 0)
					|| ((string)order[0][Constants.FIELD_ORDER_CARD_TRAN_ID] != this.ZeusOrderId))
				{
					var externalPaymentCooperationLog = string.Format(
						"該当注文が見つかりませんでした： 注文ID：{0} 決済カード取引ID：{1}",
						this.OrderId,
						this.ZeusOrderId);

					// ログファイル格納処理
					WritePaymentLog(isSuccess: false, externalPaymentCooperationLog: externalPaymentCooperationLog);
					return false;
				}

				// ステータス確定
				var orderPaymentStatus = (string)order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS];
				switch (this.Status)
				{
					case "04": // 入金済
					case "05": // 売上確定
						// 入金処理（未入金→入金済み）
						if (orderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM)
						{
							var updated = DomainFacade.Instance.OrderService.UpdatePaymentStatusForCvs(
								(string)order[0][Constants.FIELD_ORDER_ORDER_ID],
								Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE,
								this.PayDate,
								(string)order[0][Constants.FIELD_ORDER_CARD_TRAN_ID],
								Constants.FLG_LASTCHANGED_CGI,
								UpdateHistoryAction.Insert);

							if (updated == 0) throw new Exception("入金済に更新できませんでした。:" + (string)order[0][Constants.FIELD_ORDER_ORDER_ID]);
						}
						break;

					case "01": // 未入金
					case "02": // 申込エラー
					case "03": // 期日切
					case "06": // 入金取消
					case "11": // キャンセル後入金
					case "12": // キャンセル後売上
					case "13": // キャンセル後取消
						// 入金戻し（入金済み→未入金）
						if (orderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE)
						{
							DomainFacade.Instance.OrderService.UpdatePaymentStatusForCvs(
								(string)order[0][Constants.FIELD_ORDER_ORDER_ID],
								Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM,
								null,
								(string)order[0][Constants.FIELD_ORDER_CARD_TRAN_ID],
								Constants.FLG_LASTCHANGED_CGI,
								UpdateHistoryAction.Insert);
						}
						break;

					default:
						throw new ArgumentOutOfRangeException(nameof(this.Status), this.Status, null);
				}

				WritePaymentLog(isSuccess: true, externalPaymentCooperationLog: string.Empty);
				return true;
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return false;
			}

			// Write payment log
			void WritePaymentLog(bool isSuccess, string externalPaymentCooperationLog)
			{
				PaymentFileLogger.WritePaymentLog(
					success: isSuccess,
					paymentDetailType: string.Empty,
					accountSettlementCompanyName: PaymentFileLogger.PaymentType.Zeus,
					processingContent: PaymentFileLogger.PaymentProcessingType.Receive,
					externalPaymentCooperationLog: externalPaymentCooperationLog,
					idKeyAndValueDictionary: new Dictionary<string, string>
					{
						{ Constants.FIELD_ORDER_ORDER_ID, this.OrderId },
						{ Constants.FIELD_ORDER_CARD_TRAN_ID, this.ZeusOrderId }
					});
			}
		}

		/// <summary>ゼウス注文ID</summary>
		public string ZeusOrderId { get; set; }
		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>ステータス</summary>
		public string Status { get; set; }
		/// <summary>決済日時</summary>
		public DateTime PayDate { get; set; }
	}
}