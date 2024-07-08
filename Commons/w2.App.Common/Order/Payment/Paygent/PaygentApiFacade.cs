/*
=========================================================================================================
  Module      : ペイジェントAPIファサード (PaygentApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using jp.co.ks.merchanttool.connectmodule.Exception;
using jp.co.ks.merchanttool.connectmodule.system;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using w2.App.Common.Order.Payment.Paygent.ATM.Register;
using w2.App.Common.Order.Payment.Paygent.ATM.Register.Dto;
using w2.App.Common.Order.Payment.Paygent.Banknet.Register;
using w2.App.Common.Order.Payment.Paygent.Banknet.Register.Dto;
using w2.App.Common.Order.Payment.Paygent.Cvs.Register;
using w2.App.Common.Order.Payment.Paygent.Cvs.Register.Dto;
using w2.App.Common.Order.Payment.Paygent.DifferenceNotification.Dto;
using w2.App.Common.Order.Payment.Paygent.Foundation;
using w2.App.Common.Order.Payment.Paygent.Paidy.Authorization;
using w2.App.Common.Order.Payment.Paygent.Paidy.Authorization.Dto;
using w2.App.Common.Order.Payment.Paygent.Paidy.AuthorizationCancellation;
using w2.App.Common.Order.Payment.Paygent.Paidy.AuthorizationCancellation.Dto;
using w2.App.Common.Order.Payment.Paygent.Paidy.Refund;
using w2.App.Common.Order.Payment.Paygent.Paidy.Refund.Dto;
using w2.App.Common.Order.Payment.Paygent.Paidy.Settlement;
using w2.App.Common.Order.Payment.Paygent.Paidy.Settlement.Dto;
using w2.App.Common.Order.Payment.Paygent.PaymentInformationInquiry;
using w2.App.Common.Order.Payment.Paygent.PaymentInformationInquiry.Dto;

namespace w2.App.Common.Order.Payment.Paygent
{
	/// <summary>
	/// ペイジェントAPIファサード
	/// </summary>
	public class PaygentApiFacade
	{
		/// <summary>API name: 決済情報差分通知電文</summary>
		private const string API_NAME_PAYMENT_INFORMATION_DIFFERENCE_NOTIFICATION = "決済情報差分通知電文";

		/// <summary>
		/// ペイジェントAPI送信
		/// </summary>
		/// <param name="paygentApi">ペイジェントAPIクラス</param>
		/// <returns>APIレスポンス</returns>
		public static IDictionary SendRequest(PaygentApiHeader paygentApi)
		{
			try
			{
				// ペイジェントAPI用モジュールをインスタンス化 キーと値をすべて渡す
				PaygentB2BModule module = new PaygentB2BModule();
				foreach (var param in paygentApi.RequestParams)
				{
					module.reqPut(param.Key, param.Value);
				}
				PaygentUtility.WritePaygentLog("", paygentApi);
				// リクエストを送信
				module.post();
				// 処理結果 0=正常終了, 1=異常終了
				var resultStatus = module.getResultStatus();
				// 異常終了時、レスポンスコードが取得できる
				var responseCode = module.getResponseCode();
				// 異常終了時、レスポンス詳細が取得できる
				var responseDetail = module.getResponseDetail();

				// レスポンスデータが存在したらまるっと取得して返す
				// 異常終了時のパラメータはレスポンスデータに含まれないので追加してから返す
				var map = module.resNext();
				map.Add(PaygentConstants.RESPONSE_STATUS, resultStatus);
				map.Add(PaygentConstants.RESPONSE_CODE, responseCode);
				map.Add(PaygentConstants.RESPONSE_DETAIL, responseDetail);
				var apiResult = resultStatus == PaygentConstants.PAYGENT_RESPONSE_STATUS_SUCCESS;
				PaygentUtility.WritePaygentLog(responseCode + responseDetail, null, apiResult, map);
				var paymentIdDictionary = new Dictionary<string, string>();
				// payment_idが返却されていればログに落とす 返されていない場合はnull
				if (map["payment_id"] != null)
				{
					paymentIdDictionary.Add(Constants.FIELD_ORDER_CARD_TRAN_ID, map["payment_id"].ToString());
				}
				PaymentFileLogger.WritePaymentLog(
					apiResult,
					Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
					PaymentFileLogger.PaymentType.Paygent,
					PaygentUtility.GetPaygentApiTypeName(paygentApi.ApiType),
					string.Empty,
					paymentIdDictionary);
				return map;
			}
			// 例外キャッチのみ エラーログの出力処理を追加後対応
			catch (PaygentB2BModuleConnectException e)
			{
				var errorCode = e.getCode();
				PaygentUtility.WritePaygentLog("API送信時ペイジェント例外キャッチ：" + errorCode);
			}
			catch (Exception e)
			{
				PaygentUtility.WritePaygentLog("API送信時その他例外キャッチ：" + e.Message);
			}
			return null;
		}

		/// <summary>
		/// Register order
		/// </summary>
		/// <param name="cart">The cart information</param>
		/// <param name="order">Order</param>
		/// <returns>Cvs order register result</returns>
		public OrderRegisterResult RegisterOrder(CartObject cart, Hashtable order)
		{
			var paygentApiResult = new OrderRegister(cart, order).Register();
			return paygentApiResult;
		}

		/// <summary>
		/// Get payment information inquiry
		/// </summary>
		/// <param name="cardTranId">Card tran id</param>
		/// <param name="tradingId">Trading id</param>
		/// <returns>Payment information inquiry</returns>
		public PaymentInformationInquiryResult GetPaymentInformationInquiry(string cardTranId, string tradingId = "")
		{
			var paygentApiResult = new PaymentInformationInquiryApi(cardTranId, tradingId).Get();
			return paygentApiResult;
		}

		/// <summary>
		/// Write difference notification request log
		/// </summary>
		/// <param name="request">Request</param>
		public static void WriteDifferenceNotificationRequestLog(PaygentDifferenceNotificationRequestDataset request)
		{
			var requestString = JsonConvert.SerializeObject(
				request,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});
			PaygentApiService.WriteLog(
				requestString,
				string.Empty,
				API_NAME_PAYMENT_INFORMATION_DIFFERENCE_NOTIFICATION,
				string.Empty,
				null);
		}

		/// <summary>
		/// Write difference notification response log
		/// </summary>
		/// <param name="responseString">The response as string</param>
		public static void WriteDifferenceNotificationResponseLog(string responseString)
		{
			PaygentApiService.WriteResponseLog(
				API_NAME_PAYMENT_INFORMATION_DIFFERENCE_NOTIFICATION,
				responseString);
		}

		/// <summary>
		/// Banknet regist
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <param name="order">Order</param>
		/// <returns>Banknet regist result</returns>
		public BanknetOrderRegisterResult BanknetRegist(CartObject cart, Hashtable order)
		{
			var banknetOrderRegisterResult = new BanknetOrderRegister(cart, order).Register();
			return banknetOrderRegisterResult;
		}

		/// <summary>
		/// Get Paidy authorize
		/// </summary>
		/// <param name="paidyPaymentId">Paidy payment id</param>
		/// <returns>Paidy authorize result</returns>
		public PaidyAuthorizationResult PaidyAuthorize(string paidyPaymentId)
		{
			var paygentApiResult = new PaidyAuthorization(paidyPaymentId).Authorize();
			return paygentApiResult;
		}

		/// <summary>
		/// Get Paidy authorization cancellation result
		/// </summary>
		/// <param name="paymentId">Payment id</param>
		/// <returns>Paidy authorization cancellation result</returns>
		public PaidyAuthorizationCancellationResult PaidyAuthorizationCancellation(string paymentId)
		{
			var paygentApiResult = new PaidyAuthorizationCancellation(paymentId).AuthorizeCancel();
			return paygentApiResult;
		}

		/// <summary>
		/// Get Paidy refund
		/// </summary>
		/// <param name="paymentId">Payment id</param>
		/// <param name="amount">Amount</param>
		/// <returns>Paidy refund result</returns>
		public PaidyRefundResult PaidyRefund(
			string paymentId,
			decimal amount)
		{
			var paygentApiResult = new PaidyRefund(paymentId, amount).Refund();
			return paygentApiResult;
		}

		/// <summary>
		/// Get Paidy settlement
		/// </summary>
		/// <param name="paymentId">Payment id</param>
		/// <returns>Paidy settlement result</returns>
		public PaidySettlementResult PaidySettlement(string paymentId)
		{
			var paygentApiResult = new PaidySettlement(paymentId).Settlement();
			return paygentApiResult;
		}

		/// <summary>
		/// Paygent ATM register
		/// </summary>
		/// <param name="cart">The cart information</param>
		/// <param name="order">Order</param>
		/// <returns>Order register result</returns>
		public AtmOrderRegisterResult PaygentAtmRegister(CartObject cart, Hashtable order)
		{
			var paygentApiResult = new AtmOrderRegister(cart, order).Register();
			return paygentApiResult;
		}
	}
}
