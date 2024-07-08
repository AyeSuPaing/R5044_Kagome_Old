/*
=========================================================================================================
  Module      : Paygent API Logger(PaygentApiLogger.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using w2.App.Common.Order.Payment.Paygent.DifferenceNotification.Dto;
using w2.App.Common.Order.Payment.Paygent.Foundation;
using w2.Common.Logger;

namespace w2.App.Common.Order.Payment.Paygent.Logger
{
	/// <summary>
	/// 各種ファイルログを出力する
	/// </summary>
	public class PaygentApiLogger : FileLogger
	{
		/// <summary>API name: 決済情報差分通知電文</summary>
		private const string API_NAME_PAYMENT_INFORMATION_DIFFERENCE_NOTIFICATION = "決済情報差分通知電文";

		/// <summary>
		/// ログ書き込み処理（ディレクトリパス指定可能）
		/// </summary>
		/// <param name="message">メッセージ</param>
		internal static void Write(string message)
		{
			FileLogger.Write("payment", message, false);
		}

		/// <summary>
		/// 外部決済連携用ログ書き込みメソッド
		/// </summary>
		/// <param name="success">成功時のログであればtrue、成功時でも失敗時でもないただのログであればnull</param>
		/// <param name="paymentKbn">決済種別</param>
		/// <param name="jsonResponse">レスポンス</param>
		/// <param name="apiName">API名</param>
		public static void WritePaymentLog(
			bool? success,
			string paymentKbn,
			string request,
			string jsonResponse,
			string apiName = "")
		{
			var notHadResponse = string.IsNullOrEmpty(jsonResponse);
			var notHadRequest = string.IsNullOrEmpty(request);
			var message = string.Format(
				"{0} {1}{2}{3}{4}{5}{6}{7}{8}{9}",
				apiName,
				notHadResponse
					? "APIリクエスト"
					: "APIレスポンス",
				notHadRequest
					? string.Empty
					: Environment.NewLine,
				notHadRequest
					? string.Empty
					: "[リクエスト]",
				Environment.NewLine,
				request,
				Environment.NewLine,
				notHadResponse
					? string.Empty
					: "[レスポンス]",
				notHadResponse ? string.Empty : Environment.NewLine,
				jsonResponse);
			var resultWord = success.HasValue
				? success.Value
					? "成功"
					: "失敗"
				: "ログ";
			FileLogger.Write(
				"payment",
				$"[{resultWord}] {paymentKbn} Paygent {message}");
		}

		/// <summary>
		/// Write difference notification request log
		/// </summary>
		/// <param name="request">Request</param>
		public static void WriteDifferenceNotificationRequestLog(
			PaygentDifferenceNotificationRequestDataset request)
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
		/// <param name="requestString">The request as string</param>
		/// <param name="responseString">The response as string</param>
		public static void WriteDifferenceNotificationResponseLog(string requestString, string responseString)
		{
			PaygentApiService.WriteResponseLog(
				API_NAME_PAYMENT_INFORMATION_DIFFERENCE_NOTIFICATION,
				requestString,
				responseString);
		}
	}
}
