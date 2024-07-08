/*
=========================================================================================================
  Module      : ヤマトKWC向け入金チェッカー(PaymentCheckerYamatoKwc.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.YamatoKwc;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;
using w2.Common.Helper;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.ExternalPaymentChecker.Commands
{
	/// <summary>
	/// ヤマトKWC向け入金チェッカー
	/// </summary>
	public class PaymentCheckerYamatoKwc : PaymentCheckerBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paymentId">決済ID</param>
		public PaymentCheckerYamatoKwc(string paymentId)
		{
			this.PaymentId = paymentId;
		}

		/// <summary>
		/// 実行
		/// </summary>
		public override void Exec()
		{
			// 対象注文取得
			var orders = GetNonPaymentOrders(this.PaymentId, DateTime.Now.AddDays(-1 * Constants.SEARCH_DAYS));

			Console.WriteLine(
				string.Format(
					@"{0}向け{1}({2})　対象：{3}件",
					PaymentFileLogger.PaymentType.Yamatokwc,
					"入金／外部決済ステータスを変更しました。",
					this.PaymentId,
					StringUtility.ToNumeric(orders.Count)));

			var tradeInfoApi = new PaymentYamatoKwcTradeInfoApi();
			var settledChecker = new PaymentYamatoKwcSettledChecker();
			foreach (DataRowView drv in orders)
			{
				var orderId = (string)drv[Constants.FIELD_ORDER_ORDER_ID];
				var paymentOrderId = (string)drv[Constants.FIELD_ORDER_PAYMENT_ORDER_ID];
				var paymentId = (string)drv[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN];

				// 取引照会に使うID
				// コンビニ前払いは注文ID、それ以外は決済注文IDを使う
				var tradeInfoUseid = (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE) ? orderId : paymentOrderId;

				// 取引情報取得
				var result = tradeInfoApi.Exec(tradeInfoUseid);

				// 外部決済連携ログ処理
				OrderCommon.AppendExternalPaymentCooperationLog(
					result.Success,
					orderId,
					result.Success
						? LogCreator.CreateMessageWithPaymentId(orderId, paymentOrderId, "")
						: LogCreator.CreateErrorMessage(
							result.ErrorCode,
							result.ErrorMessage,
							PaymentFileLogger.PaymentType.Yamatokwc.ToText() + PaymentFileLogger.PaymentProcessingType.TransactionInformationInquiry.ToText()),
					Constants.FLG_LASTCHANGED_BATCH,
					UpdateHistoryAction.Insert);

				PaymentFileLogger.WritePaymentLog(
					result.Success,
					paymentId,
					PaymentFileLogger.PaymentType.Yamatokwc,
					PaymentFileLogger.PaymentProcessingType.Unknown,
					result.Success
						? ""
						: LogCreator.CreateErrorMessage(
							result.ErrorCode,
							result.ErrorMessage,
							PaymentFileLogger.PaymentType.Yamatokwc.ToText() + PaymentFileLogger.PaymentProcessingType.TransactionInformationInquiry.ToText()),
					new Dictionary<string, string>
					{
						{Constants.FIELD_ORDER_ORDER_ID, orderId},
						{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId},
						{Constants.FOR_LOG_KEY_PAYMENT_ID, paymentId},
					});

				// 入金処理
				if (result.ResultDatas.Any(d => settledChecker.Check(d.SettleMethod, d.StatusInfo)))
				{
					using (var accessor = new SqlAccessor())
					{
						accessor.OpenConnection();
						accessor.BeginTransaction();

						// 入金ステータス更新
						new OrderService().UpdatePaymentStatus(
							orderId,
							Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE,
							DateTime.Now.Date,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.DoNotInsert,
							accessor);
						// 外部決済ステータスを「入金済み」に更新（更新履歴とともに）
						new OrderService().UpdateExternalPaymentStatusPayComplete(
							orderId,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.Insert,
							accessor);

						accessor.CommitTransaction();

						PaymentFileLogger.WritePaymentLog(
							null,
							paymentId,
							PaymentFileLogger.PaymentType.Yamatokwc,
							PaymentFileLogger.PaymentProcessingType.ChangePaymentAndExternalPaymentStatus,
							"",
							new Dictionary<string, string>
							{
								{Constants.FIELD_ORDER_ORDER_ID, orderId},
								{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId},
								{Constants.FIELD_PAYMENT_PAYMENT_ID, paymentId},
							});
					}
				}
			}
		}

		/// <summary>決済ID</summary>
		public string PaymentId { get; set; }
	}
}
