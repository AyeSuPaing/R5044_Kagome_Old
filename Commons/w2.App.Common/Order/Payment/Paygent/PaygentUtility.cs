/*
=========================================================================================================
  Module      : ペイジェントユーティリティ(PaygentUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Globalization;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Order.Payment.Paygent.Logger;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Payment.Paygent
{
	public class PaygentUtility
	{
		/// <summary>
		/// ペイジェントログ出力
		/// </summary>
		/// <param name="logString">ログ本文</param>
		/// <param name="paygentApi">ペイジェントリクエスト情報</param>
		/// <param name="isSuccess">実行結果</param>
		/// <param name="response">ペイジェントレスポンス情報</param>
		public static void WritePaygentLog(string logString, PaygentApiHeader paygentApi = null, bool? isSuccess = null, IDictionary response = null)
		{
			var message = "[ログ] K10 Paygent ";
			if (paygentApi != null)
			{
				message += GetPaygentApiTypeName(paygentApi.ApiType).ToText() + "　リクエスト　\n";
				foreach(var param in paygentApi.RequestParams)
				{
					message += "	" + param.Key + ":" + param.Value + "\n";
				}
			}
			else
			{
				// 成功失敗が不要なログならメッセージのみを直接出力
				message = "　レスポンス　" + (isSuccess.HasValue ? (isSuccess.Value ? "[成功]　" + logString : "[失敗]　" + logString) : logString);
				if (response != null)
				{
					message+= "\n";
					foreach (DictionaryEntry param in response)
					{
						message += "	" + param.Key + ":" + param.Value + "\n";
					}
				}
			}
				
			FileLogger.Write("Paygent", message, Constants.PHYSICALDIRPATH_LOGFILE);
		}

		/// <summary>
		/// ペイジェント電文種別IDから電文名を取得
		/// </summary>
		/// <param name="apiType">電文種別ID</param>
		/// <returns>電文名</returns>
		public static PaymentFileLogger.PaymentProcessingType GetPaygentApiTypeName(string apiType)
		{
			switch (apiType)
			{
				case PaygentConstants.PAYGENT_APITYPE_CARD_AUTH:
					return PaymentFileLogger.PaymentProcessingType.PaygentCardAuthApi;

				case PaygentConstants.PAYGENT_APITYPE_CARD_AUTH_CANCEL:
					return PaymentFileLogger.PaymentProcessingType.PaygentCardAuthCancelApi;

				case PaygentConstants.PAYGENT_APITYPE_CARD_REALSALE:
					return PaymentFileLogger.PaymentProcessingType.PaygentCardRealSaleApi;

				case PaygentConstants.PAYGENT_APITYPE_CARD_REALSALE_CANCEL:
					return PaymentFileLogger.PaymentProcessingType.PaygentCardRealSaleCancelApi;

				case PaygentConstants.PAYGENT_APITYPE_CARD_REGISTER:
					return PaymentFileLogger.PaymentProcessingType.PaygentCardRegisterApi;

				case PaygentConstants.PAYGENT_APITYPE_CARD_DELETE:
					return PaymentFileLogger.PaymentProcessingType.PaygentCardDeleteApi;

				case PaygentConstants.PAYGENT_APITYPE_CARD_3DSECURE_AUTH:
					return PaymentFileLogger.PaymentProcessingType.PaygentCard3DSecureAuthApi;

				default:
					return PaymentFileLogger.PaymentProcessingType.Unknown;
			}
		}

		/// <summary>
		/// 同時売上時に外部決済ステータス、オンライン決済ステータス、入金ステータスを更新し、決済メモを落とす
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">accessor</param>
		public static void UpdatePaymentStatus(string orderId, SqlAccessor accessor)
		{
			var orderService = new OrderService();
			var order = orderService.Get(orderId, accessor);

			var paygentPaymentMethod = order.IsDigitalContents
				? Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD_FORDIGITALCONTENTS
				: Constants.PAYMENT_PAYGENT_CREDIT_PAYMENTMETHOD;
			// クレジット決済方法が同時売上の場合のみ処理実行
			if (paygentPaymentMethod == Constants.PaygentCreditCardPaymentMethod.Auth) return;

			// 外部決済ステータス・外部決済与信日時を更新
			orderService.UpdateExternalPaymentInfo(
				orderId,
				Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP,
				true,
				DateTime.Now,
				string.Empty,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert,
				accessor);
			// オンライン決済ステータス更新
			orderService.UpdateOnlinePaymentStatus(
				orderId,
				Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED,
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert,
				accessor);

			// 決済連携メモ更新
			orderService.AddPaymentMemo(
				orderId,
				OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
					string.IsNullOrEmpty(order.PaymentOrderId)
						? order.OrderId
						: order.PaymentOrderId,
					order.OrderPaymentKbn,
					order.CardTranId,
					//「売上確定」
					ValueText.GetValueText(
						Constants.TABLE_ORDER,
						Constants.VALUETEXT_PARAM_PAYMENT_MEMO,
						Constants.VALUETEXT_PARAM_SALES_CONFIRMED),
					CurrencyManager.GetSendingAmount(
						order.LastBilledAmount,
						order.SettlementAmount,
						order.SettlementCurrency)),
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}

		/// <summary>
		/// 外部決済ステータスをもとに売上/オーソリキャンセル電文を連携
		/// </summary>
		/// <param name="order">受注情報</param>
		/// <returns>成功/失敗・エラーコード,エラーメッセージのTuple</returns>
		public static Tuple<bool, string> DoCancel(OrderModel order)
		{
			// クレカではない もしくはペイジェントクレカでない場合は失敗とみなす
			if ((order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				|| (Constants.PAYMENT_CARD_KBN != Constants.PaymentCard.Paygent))
			{
				return new Tuple<bool, string>(false, string.Empty);
			}

			var apiResult = false;
			var resultString = string.Empty;
			// 外部決済ステータスによって判断
			// オーソリ済み：オーソリキャンセル電文
			if (order.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP)
			{
				var sendParams = new PaygentApiHeader(PaygentConstants.PAYGENT_APITYPE_CARD_AUTH_CANCEL);
				sendParams.PaymentId = order.CardTranId;
				var result = PaygentApiFacade.SendRequest(sendParams);
				resultString = string.Format("{0},{1}", (string)result[PaygentConstants.RESPONSE_CODE], (string)result[PaygentConstants.RESPONSE_DETAIL]);

				apiResult = (string)result[PaygentConstants.RESPONSE_STATUS] == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS;
			}
			// 売上確定済み：売上キャンセル電文
			else if (order.ExternalPaymentStatus == Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP)
			{
				var sendParams = new PaygentApiHeader(PaygentConstants.PAYGENT_APITYPE_CARD_REALSALE_CANCEL);
				sendParams.PaymentId = order.CardTranId;
				var result = PaygentApiFacade.SendRequest(sendParams);
				resultString = string.Format("{0},{1}", (string)result[PaygentConstants.RESPONSE_CODE], (string)result[PaygentConstants.RESPONSE_DETAIL]);
				
				apiResult = (string)result[PaygentConstants.RESPONSE_STATUS] == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS;
			}
			return new Tuple<bool, string>(apiResult, resultString);
		}

		/// <summary>
		/// ペイジェント注文ロールバック処理
		/// </summary>
		/// <param name="order">処理中の受注情報</param>
		/// <param name="cart">処理中のカートオブジェクト</param>
		/// <remarks> オーソリで失敗した場合は必ずロールバック</remarks>
		/// <remarks> 3Dセキュア利用時に認証タイムアウトの場合はロールバックしない（これを呼び出さない）</remarks>
		public static void RollbackPaygentPreOrder(Hashtable order, CartObject cart)
		{
			OrderCommon.RollbackPreOrder(
				order,
				cart,
				false,
				0,
				true,
				UpdateHistoryAction.Insert);
		}

		/// <summary>
		/// Check is paidy paygent payment
		/// </summary>
		/// <param name="paymentId">Payment id</param>
		/// <returns>True if it is a paidy paygent payment</returns>
		public static bool CheckIsPaidyPaygentPayment(string paymentId)
		{
			return ((Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Paygent)
				&& (paymentId == Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY));
		}

		/// <summary>
		/// Can use paidy payment
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <returns>True if can use paidy payment, otherwise false</returns>
		public static bool CanUsePaidyPayment(CartObject cart)
		{
			if (Constants.PAYMENT_PAIDY_KBN == Constants.PaymentPaidyKbn.Direct) return true;

			var result = ((Constants.GLOBAL_OPTION_ENABLE == false)
				&& Constants.PAYMENT_PAIDY_OPTION_ENABLED
				&& (cart.HasFixedPurchase == false)
				&& (cart.HasSubscriptionBox == false));
			return result;
		}

		/// <summary>
		/// Update order for paygent
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="paymentStatus">Payment status</param>
		/// <param name="paymentType">Payment type</param>
		/// <param name="paymentDate">Payment date</param>
		/// <param name="lastChanged">Last changed</param>
		public static void UpdateOrderForPaygent(
			string orderId,
			string paymentStatus,
			string paymentType,
			string paymentDate,
			string lastChanged)
		{
			var isParsePaymentDateSuccess = DateTime.TryParseExact(
				paymentDate,
				"yyyyMMddHHmmss",
				CultureInfo.InvariantCulture,
				DateTimeStyles.None,
				out var orderPaymentDate);

			switch (paymentStatus)
			{
				case PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_DELETED:
				case PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_NOTICE_DETECTED:
					if (paymentType == PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_PAIDY)
					{
						goto case PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_AUTH_CANCEL;
					}
					DomainFacade.Instance.OrderService.Modify(
						orderId,
						updateOrder =>
						{
							updateOrder.OrderPaymentStatus = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE;
							updateOrder.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP;
							updateOrder.OrderPaymentDate = isParsePaymentDateSuccess
								? orderPaymentDate.Date
								: updateOrder.OrderPaymentDate;
							updateOrder.LastChanged = lastChanged;
							updateOrder.DateChanged = DateTime.Now;
						},
						UpdateHistoryAction.DoNotInsert);
					PaygentApiExternalPaymentCooperationLog.AppendExternalPaymentCheckerLog(
						true,
						orderId,
						paymentType,
						paymentStatus,
						lastChanged,
						UpdateHistoryAction.DoNotInsert);

					if (paymentType == PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_CONVENIENCE_STORE)
					{
						PaygentApiExternalPaymentCooperationLog.AppendExternalPaymentLogForLinkageSuccess(
							true,
							orderId,
							lastChanged,
							UpdateHistoryAction.DoNotInsert);
					}
					break;

				case PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_AUTH_COMP:
				case PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_AUTH_EXPIRED:
				case PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_SALES_CANCEL_EXPIRED:
					var externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP;
					switch (paymentStatus)
					{
						case PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_AUTH_EXPIRED:
							externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_EXPIRED;
							break;

						case PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_SALES_CANCEL_EXPIRED:
							externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_CANCEL_EXPIRED;
							break;
					}
					DomainFacade.Instance.OrderService.Modify(
						orderId,
						updateOrder =>
						{
							updateOrder.OrderPaymentStatus = paymentStatus == PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_SALES_CANCEL_EXPIRED
								? Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE
								: updateOrder.OrderPaymentStatus;
							updateOrder.ExternalPaymentStatus = externalPaymentStatus;
							updateOrder.OrderPaymentDate = isParsePaymentDateSuccess
								? orderPaymentDate.Date
								: updateOrder.OrderPaymentDate;
							updateOrder.LastChanged = lastChanged;
							updateOrder.DateChanged = DateTime.Now;
						},
						UpdateHistoryAction.DoNotInsert);
					PaygentApiExternalPaymentCooperationLog.AppendExternalPaymentCheckerLog(
						isSuccess: true,
						orderId,
						paymentType,
						paymentStatus,
						lastChanged,
						UpdateHistoryAction.DoNotInsert);

					if ((paymentType != PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_PAIDY)
						&& (paymentType != PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_ATM)
						&& (paymentType != PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_BANKNET))
					{
						PaygentApiExternalPaymentCooperationLog.AppendExternalPaymentLogForLinkageSuccess(
							true,
							orderId,
							lastChanged,
							UpdateHistoryAction.DoNotInsert);
					}
					break;

				case PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_AUTH_CANCEL:
				case PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_PERIOD_EXPIRED:
				case PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_APPLICATION_SUSPENDED:
				case PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_NOTICE_CANCELED:
				case PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_SALES_CANCELED:
					if (((paymentType == PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_ATM)
						|| (paymentType == PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_BANKNET))
						&& (paymentStatus == PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_PERIOD_EXPIRED))
					{
						externalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_NONE;
						DomainFacade.Instance.OrderService.Modify(
							orderId,
							updateOrder =>
							{
								updateOrder.ExternalPaymentStatus = externalPaymentStatus;
								updateOrder.OrderPaymentDate = isParsePaymentDateSuccess
									? orderPaymentDate.Date
									: updateOrder.OrderPaymentDate;
								updateOrder.LastChanged = lastChanged;
								updateOrder.DateChanged = DateTime.Now;
							},
							UpdateHistoryAction.DoNotInsert);
					}
					PaygentApiExternalPaymentCooperationLog.AppendExternalPaymentCheckerLog(
						isSuccess: true,
						orderId,
						paymentType,
						paymentStatus,
						lastChanged,
						UpdateHistoryAction.DoNotInsert);
					break;
			}
		}

		/// <summary>
		/// Can use Banknet payment
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <returns>True if can use Banknet payment, otherwise false</returns>
		public static bool CanUseBanknetPayment(CartObject cart)
		{
			var result = ((Constants.GLOBAL_OPTION_ENABLE == false)
				&& Constants.PAYMENT_NETBANKING_OPTION_ENABLED
				&& (Constants.PAYMENT_NETBANKING_KBN == Constants.PaymentBanknetKbn.Paygent)
				&& (cart.HasFixedPurchase == false)
				&& (cart.HasSubscriptionBox == false));

			return result;
		}

		/// <summary>
		/// Can use Atm payment
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <returns>True if can use Atm payment, otherwise false</returns>
		public static bool CanUseAtmPayment(CartObject cart)
		{
			var result = ((Constants.GLOBAL_OPTION_ENABLE == false)
				&& Constants.PAYMENT_ATMOPTION_ENABLED
				&& (Constants.PAYMENT_ATM_KBN == Constants.PaymentATMKbn.Paygent)
				&& ((cart.HasFixedPurchase == false)
					|| (cart.HasSubscriptionBox == false)));

			return result;
		}
	}
}
