/*
=========================================================================================================
  Module      : Paygent API Service クラス(PaygentApiService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System.Collections.Generic;
using w2.App.Common.Order.Payment.Paygent.Logger;

namespace w2.App.Common.Order.Payment.Paygent.Foundation
{
	/// <summary>
	/// Paygent API Service クラス
	/// </summary>
	internal class PaygentApiService
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiName">API名</param>
		/// <param name="parameters">リクエストパラメータ</param>
		public PaygentApiService(
			string apiName,
			Dictionary<string, string> parameters)
		{
			this.ApiName = apiName;
			this.Parameters = parameters;
		}

		/// <summary>
		/// API実行結果取得
		/// </summary>
		/// <typeparam name="TResult">The type of payent api result</typeparam>
		/// <returns>レスポンスデータ</returns>
		public PaygentApiResult GetResult<TResult>()
			where TResult : IPaygentResponse, new()
		{
			var paymentType = string.Empty;
			var jsonRequest = string.Empty;
			var responseJson = string.Empty;
			var isSuccess = true;
			PaygentApiResult result = null;
			try
			{
				var request = new PaygentApiRequest(
					this.ApiName,
					this.Parameters);
				var telegramKind = this.Parameters[PaygentConstants.PAYGENT_API_REQUEST_TELEGRAM_KIND];
				paymentType = GetPaymentTypeApi(telegramKind);
				jsonRequest = JsonConvert.SerializeObject(
					request,
					new JsonSerializerSettings
					{
						Formatting = Formatting.Indented,
						NullValueHandling = NullValueHandling.Ignore
					});

				var response = request.Post();
				if (response != null)
				{
					responseJson = JsonConvert.SerializeObject(
						response,
						new JsonSerializerSettings
						{
							Formatting = Formatting.Indented,
							NullValueHandling = NullValueHandling.Ignore
						});
				}
				var responseResult = JsonConvert.DeserializeObject<TResult>(responseJson);

				result = new PaygentApiResult(responseResult, string.Empty);
				isSuccess = result.IsSuccess();
				return result;
			}
			catch (PaygentException ex)
			{
				result = new PaygentApiResult(ex.Message);
				isSuccess = false;
				responseJson = ex.ToString();

				return result;
			}
			finally
			{
				WriteLog(
					jsonRequest,
					responseJson,
					this.ApiName,
					paymentType,
					isSuccess);
			}
		}

		/// <summary>
		/// Write payment log
		/// </summary>
		/// <param name="request">Request</param>
		/// <param name="response">Response</param>
		/// <param name="apiName">Api name</param>
		/// <param name="paygentPaymentKbn">Paygent payment kbn</param>
		/// <param name="isSuccess">成功かどうか</param>
		internal static void WriteLog(
			string request,
			string response,
			string apiName,
			string paygentPaymentKbn,
			bool? isSuccess)
		{
			var paymentKbn = string.Empty;
			switch (paygentPaymentKbn)
			{
				case PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_CONVENIENCE_STORE:
					paymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE;
					break;

				case PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_PAIDY:
					paymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY;
					break;

				case PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_BANKNET:
					paymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_BANKNET;
					break;

				case PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_ATM:
					paymentKbn = Constants.FLG_PAYMENT_PAYMENT_ID_ATM;
					break;
			}
			PaygentApiLogger.WritePaymentLog(
				isSuccess,
				paymentKbn,
				request,
				response,
				apiName);
		}

		/// <summary>
		/// レスポンスログの記述
		/// </summary>
		/// <param name="apiName">API名</param>
		/// <param name="responseString">Response</param>
		internal static void WriteResponseLog(string apiName, string responseString)
		{
			PaygentApiLogger.Write($"API : {apiName}\r\n Response Param\r\n{responseString}");
		}

		/// <summary>
		/// レスポンスログの記述
		/// </summary>
		/// <param name="apiName">API名</param>
		/// <param name="responseString">Request</param>
		/// <param name="responseString">Response</param>
		internal static void WriteResponseLog(string apiName, string requestString, string responseString)
		{
			PaygentApiLogger.Write($"API : {apiName}\r\n Request Param\r\n{requestString}\r\n Response Param\r\n{responseString}");
		}

		/// <summary>
		/// Get payment type api
		/// </summary>
		/// <param name="telegramKind">Telegram kind</param>
		/// <returns>Payment type</returns>
		private string GetPaymentTypeApi(string telegramKind)
		{
			switch (telegramKind)
			{
				case PaygentConstants.PAYGENT_TELEGRAM_KIND_CONVENIENCE_STORE_ORDER_REGISTER:
					return PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_CONVENIENCE_STORE;

				case PaygentConstants.PAYGENT_TELEGRAM_KIND_PAIDY_AUTHORIZATION_CANCELLATION:
				case PaygentConstants.PAYGENT_TELEGRAM_KIND_PAIDY_SETTLEMENT:
				case PaygentConstants.PAYGENT_TELEGRAM_KIND_PAIDY_REFUND:
				case PaygentConstants.PAYGENT_TELEGRAM_KIND_PAIDY_AUTHORIZATION:
					return PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_PAIDY;

				case PaygentConstants.PAYGENT_TELEGRAM_KIND_BANKNET_ORDER_REGISTER:
					return PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_BANKNET;

				case PaygentConstants.PAYGENT_TELEGRAM_KIND_ATM_ORDER_REGISTER:
					return PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_ATM;
			}
			return string.Empty;
		}

		/// <summary>API名</summary>
		public string ApiName { get; private set; } = string.Empty;
		/// <summary>リクエストパラメータ</summary>
		public Dictionary<string, string> Parameters { get; private set; }
	}
}
