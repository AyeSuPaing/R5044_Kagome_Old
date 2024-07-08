/*
=========================================================================================================
  Module      : ペイジェントAPI 外部決済連携ログ (PaygentApiExternalPaymentCooperationLog.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.Extensions.Currency;
using w2.Common.Sql;
using w2.Domain;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Payment.Paygent.Logger
{
	/// <summary>
	/// ペイジェントAPI 外部決済連携ログ
	/// </summary>
	public class PaygentApiExternalPaymentCooperationLog
	{
		/// <summary>Api response payment status names</summary>
		public static readonly Dictionary<string, string> s_apiResponsePaymentStatusNames = new Dictionary<string, string>
		{
			{ PaygentConstants.PAYGENT_API_HASHCODE_ERROR, "ハッシュ値エラー" },
			{ PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_APPLIED, "申込済" },
			{ PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_PERIOD_EXPIRED, "支払期限切" },
			{ PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_APPLICATION_SUSPENDED, "申込中断" },
			{ PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_AUTH_COMP, "オーソリOK" },
			{ PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_AUTH_CANCEL, "オーソリ取消済" },
			{ PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_AUTH_EXPIRED, "オーソリ期限切れ" },
			{ PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_DELETED, "消込済" },
			{ PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_SALES_CANCEL_EXPIRED, "消込済（売上取消期限切）" },
			{ PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_NOTICE_DETECTED, "速報検知済" },
			{ PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_SALES_CANCELED, "売上取消済" },
			{ PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_NOTICE_CANCELED, "速報取消済" },
		};

		/// <summary>
		/// Appending external payment checker log
		/// </summary>
		/// <param name="isSuccess">Is success</param>
		/// <param name="orderId">Order id</param>
		/// <param name="paymentType">Payment type</param>
		/// <param name="paymentStatus">Payment status</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Accessor</param>
		public static void AppendExternalPaymentCheckerLog(
			bool isSuccess,
			string orderId,
			string paymentType,
			string paymentStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			string logFormat;
			switch (paymentType)
			{
				case PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_CONVENIENCE_STORE:
					var cvsLog = string.Format(
						"{0:yyyy/MM/dd HH:mm:ss}\t[{1}]\t{2}[{3}]\r\n",
						DateTime.Now,
						isSuccess ? "成功" : "失敗",
						"決済ステータス通知受取",
						s_apiResponsePaymentStatusNames[paymentStatus]);

					DomainFacade.Instance.OrderService.AppendExternalPaymentCooperationLog(
						orderId,
						cvsLog,
						lastChanged,
						updateHistoryAction,
						accessor);
					return;

				case PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_PAIDY:
					logFormat = "{0:yyyy/MM/dd HH:mm:ss}\t[{1}]\t決済取引ID:{2}・決済注文ID:{3}・{4}\t{5} 最終更新者:{6}\r\n";
					break;

				case PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_BANKNET:
				case PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_ATM:
					logFormat = "{0:yyyy/MM/dd HH:mm:ss}\t[{1}]\t{2}[{3}] order_id:{4} 最終更新者: {5}\r\n";
					break;

				default:
					logFormat = "{0:yyyy/MM/dd HH:mm:ss}\t[{1}]\t{2}[{3}]\r\n";
					break;
			}

			string organizedLog;
			if (paymentType != PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_PAIDY)
			{
				organizedLog = string.Format(
					logFormat,
					DateTime.Now,
					isSuccess ? "成功" : "失敗",
					"決済ステータス通知受取",
					s_apiResponsePaymentStatusNames[paymentStatus],
					orderId,
					lastChanged);
			}
			else
			{
				var order = DomainFacade.Instance.OrderService.Get(orderId);
				var priceString = order.LastBilledAmount > 0
					? string.Format("{0}円", order.LastBilledAmount.ToPriceString())
					: string.Empty;
				organizedLog = string.Format(
					logFormat,
					DateTime.Now,
					isSuccess ? "成功" : "失敗",
					order.CardTranId,
					order.PaymentOrderId,
					priceString,
					s_apiResponsePaymentStatusNames[paymentStatus],
					lastChanged);
			}

			DomainFacade.Instance.OrderService.AppendExternalPaymentCooperationLog(
				orderId,
				organizedLog,
				lastChanged,
				updateHistoryAction,
				accessor);
		}

		/// <summary>
		/// Append external payment log for linkage success
		/// </summary>
		/// <param name="isSuccess">Is success</param>
		/// <param name="orderId">Order id</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Accessor</param>
		public static void AppendExternalPaymentLogForLinkageSuccess(
			bool isSuccess,
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			var organizedLog = string.Format(
				"{0:yyyy/MM/dd HH:mm:ss}\t[{1}]\t{2}\r\n",
				DateTime.Now,
				isSuccess ? "成功" : "失敗",
				$"order_id：{orderId} 最終更新者：{lastChanged}");
			DomainFacade.Instance.OrderService.AppendExternalPaymentCooperationLog(
				orderId,
				organizedLog,
				lastChanged,
				updateHistoryAction,
				accessor);
		}
	}
}
