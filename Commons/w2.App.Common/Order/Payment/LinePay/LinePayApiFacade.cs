/*
=========================================================================================================
  Module      : Line Pay Api Facade (LinePayApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.LinePay
{
	/// <summary>
	/// LINE Pay APIのファサード
	/// </summary>
	public static class LinePayApiFacade
	{
		#region Constants
		/// <summary>HTTP method: POST</summary>
		private const string HTTP_METHOD_POST = "POST";
		/// <summary>HTTP method: GET</summary>
		private const string HTTP_METHOD_GET = "GET";
		/// <summary>HTTP header: Content-Type</summary>
		private const string HTTP_HEADER_CONTENT_TYPE = "Content-Type";
		/// <summary>HTTP header: X-LINE-ChannelId</summary>
		private const string HTTP_HEADER_XLINE_CHANNEL_ID = "X-LINE-ChannelId";
		/// <summary>HTTP header: X-LINE-Authorization</summary>
		private const string HTTP_HEADER_XLINE_AUTHORIZATION = "X-LINE-Authorization";
		/// <summary>HTTP header: X-LINE-Authorization-Nonce</summary>
		private const string HTTP_HEADER_XLINE_AUTHORIZATION_NONCE = "X-LINE-Authorization-Nonce";
		/// <summary>HTTP header: Content-Type: application/json</summary>
		private const string HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON = "application/json";
		#endregion

		#region Call API Methods
		/// <summary>
		/// Request a payment
		/// </summary>
		/// <param name="request">Request object</param>
		/// <param name="logInfo">ログ情報</param>
		/// <returns>Line response object</returns>
		/// <remarks>
		/// For more information, please visit: <seealso cref="https://pay.line.me/documents/online_v3_en.html#request-api"/>
		/// LINE Pay決済をリクエストします。
		/// このとき、ユーザーの注文情報と決済手段を設定できます。
		/// リクエストに成功するとLINE Pay取引番号が発行されます。この取引番号を利用して、決済完了・返金を行うことができます。
		/// </remarks>
		public static LinePayResponse RequestPayment(LinePayRequestPayment request, LinePayLogInfo logInfo)
		{
			var result = CallApi<LinePayResponse>(
				"request",
				JsonConvert.SerializeObject(
					request,
					Formatting.None,
					new JsonSerializerSettings
					{
						NullValueHandling = NullValueHandling.Ignore
					}),
				logInfo,
				PaymentFileLogger.PaymentProcessingType.Request,
				HTTP_METHOD_POST);
			return result;
		}

		/// <summary>
		/// Confirm a payment
		/// </summary>
		/// <param name="tranId">A transaction ID</param>
		/// <param name="request">A request object</param>
		/// <param name="logInfo">ログ情報</param>
		/// <returns>Line Pay response object</returns>
		/// <remarks>
		/// For more information, please visit: <seealso cref="https://pay.line.me/documents/online_v3_en.html#confirm-api"/>
		/// confirmUrlまたはCheck Payment Status APIによってユーザーが決済要求を承認した後、加盟店側で決済を完了させるためのAPIです。
		/// Request APIの"options.payment.capture"をfalseに設定するとオーソリと売上確定が分離された決済になり、
		/// 決済を完了させても決済ステータスは売上確定待ち(オーソリ)状態のままとなります。
		/// 売上を確定するには、Capture APIを呼び出して売上確定を行う必要があります。
		/// </remarks>
		public static LinePayResponse ConfirmPayment(string tranId, LinePayConfirmPaymentRequest request, LinePayLogInfo logInfo)
		{
			var result = CallApi<LinePayResponse>(
				string.Format("{0}/confirm", tranId),
				JsonConvert.SerializeObject(request),
				logInfo,
				PaymentFileLogger.PaymentProcessingType.Commit,
				HTTP_METHOD_POST);
			return result;
		}

		/// <summary>
		/// Void a payment
		/// </summary>
		/// <param name="tranId">A transaction ID</param>
		/// <param name="logInfo">ログ情報</param>
		/// <returns>Line Pay response object</returns>
		/// <remarks>
		/// For more information, please visit: <seealso cref="https://pay.line.me/documents/online_v3_en.html#void-api"/>
		/// 決済ステータスがオーソリ状態である決済データを無効化するAPIです。
		/// Confirm APIを呼び出して決済完了したオーソリ状態の取引を取り消すことができます。
		/// 取り消しできるのはオーソリ状態の取引だけであり、売上確定済みの取引はRefund APIを使用して返金します。
		/// </remarks>
		public static LinePayResponse VoidApiPayment(string tranId, LinePayLogInfo logInfo)
		{
			var result = CallApi<LinePayResponse>(
				string.Format("authorizations/{0}/void", tranId),
				string.Empty,
				logInfo,
				PaymentFileLogger.PaymentProcessingType.Void,
				HTTP_METHOD_POST);
			return result;
		}

		/// <summary>
		/// Capture a payment
		/// </summary>
		/// <param name="tranId">A transaction ID</param>
		/// <param name="amount">Total amount</param>
		/// <param name="currency">A currency (ISO 4217)</param>
		/// <param name="logInfo">ログ情報</param>
		/// <returns>Line Pay response object</returns>
		/// <remarks>
		/// For more information, please visit: <seealso cref="https://pay.line.me/documents/online_v3_en.html#capture-api"/>
		/// Request APIを使って決済をリクエストする際に"options.payment.capture"をfalseに設定した場合、
		/// Confirm APIで決済を完了させると決済ステータスは売上確定待ち状態になります。
		/// 決済を完全に確定するためには、Capture APIを呼び出して売上確定を行う必要があります。
		/// </remarks>
		public static LinePayResponse CapturePayment(string tranId, decimal amount, string currency, LinePayLogInfo logInfo)
		{
			var result = CallApi<LinePayResponse>(
				string.Format("authorizations/{0}/capture", tranId),
				string.Format("{{\"amount\":\"{0}\",\"currency\":\"{1}\"}}", (int)amount, currency),
				logInfo,
				PaymentFileLogger.PaymentProcessingType.Sale,
				HTTP_METHOD_POST);
			return result;
		}

		/// <summary>
		/// Refund a payment
		/// </summary>
		/// <param name="tranId">A transaction ID</param>
		/// <param name="refundAmount">Total amount of refund</param>
		/// <param name="logInfo">ログ情報</param>
		/// <returns>Line Pay response object</returns>
		/// <remarks>
		/// For more information, please visit: <seealso cref="https://pay.line.me/documents/online_v3_en.html#refund-api"/>
		/// 決済完了(売上確定済み)された取引を返金します。
		/// 返金時は、LINE Payユーザーの決済取引番号を必ず渡す必要があります。一部返金も可能です。
		/// </remarks>
		public static LinePayResponse RefundPayment(string tranId, decimal refundAmount, LinePayLogInfo logInfo)
		{
			var result = CallApi<LinePayResponse>(
				string.Format("{0}/refund", tranId),
				string.Format("{{\"refundAmount\":\"{0}\"}}", (int)refundAmount),
				logInfo,
				PaymentFileLogger.PaymentProcessingType.Refund,
				HTTP_METHOD_POST);
			return result;
		}

		/// <summary>
		/// Preapproved a payment
		/// </summary>
		/// <param name="regKey">Key for automatic payment</param>
		/// <param name="productName">A product name</param>
		/// <param name="amount">Total amount</param>
		/// <param name="currency">A currency (ISO 4217)</param>
		/// <param name="paymentOrderId">An order ID</param>
		/// <param name="isAutoCapture">Is automatic capture</param>
		/// <param name="logInfo">ログ情報</param>
		/// <returns>Line Pay response object</returns>
		/// <remarks>
		/// For more information, please visit: <seealso cref="https://pay.line.me/documents/online_v3_en.html#pay-preapproved-api"/>
		/// Request APIとConfirm APIを使用して予め自動決済を登録しておけば、
		/// Confirm APIで渡されたRegKeyを利用して、ユーザー承認プロセスを経ずに決済を行うことができます。
		/// </remarks>
		public static LinePayResponse PreapprovedPayment(
			string regKey,
			string productName,
			decimal amount,
			string currency,
			string paymentOrderId,
			bool isAutoCapture,
			LinePayLogInfo logInfo)
		{
			var result = CallApi<LinePayResponse>(
				string.Format("preapprovedPay/{0}/payment", regKey),
				string.Format(
					"{{\"productName\":\"{0}\",\"amount\":\"{1}\",\"currency\":\"{2}\",\"orderId\":\"{3}\",\"capture\":\"{4}\"}}",
					productName,
					(int)amount,
					currency,
					paymentOrderId,
					isAutoCapture.ToString().ToLower()),
				logInfo,
				PaymentFileLogger.PaymentProcessingType.ExecPayment,
				HTTP_METHOD_POST);
			return result;
		}

		/// <summary>
		/// Check RegKey APIで自動決済キーのステータスを確認
		/// </summary>
		/// <param name="regKey">自動決済キー</param>
		/// <param name="logInfo">ログ情報</param>
		/// <returns>Line Pay response object</returns>
		/// <remarks>
		/// For more information, please visit: <seealso cref="https://pay.line.me/documents/online_v3_en.html#pay-preapproved-api"/>
		/// Check RegKey APIを利用し、発行済みのRegKeyのステータスを照会します。
		/// RegKeyが有効な場合はRegKeyを利用して、ユーザー承認プロセスを経ずに決済を行うことができます。
		/// </remarks>
		public static LinePayResponse ValidateRegKey(string regKey, LinePayLogInfo logInfo)
		{
			var result = CallApi<LinePayResponse>(
				string.Format("preapprovedPay/{0}/check", regKey),
				string.Format("creditCardAuth={0}", false),
				logInfo,
				PaymentFileLogger.PaymentProcessingType.Examination,
				HTTP_METHOD_GET);
			return result;
		}

		/// <summary>
		/// Call Line pay API
		/// </summary>
		/// <typeparam name="TResponse">Type of response data</typeparam>
		/// <param name="uriPath">The request URL</param>
		/// <param name="requestData">The request data</param>
		/// <param name="logInfo">ログ情報</param>
		/// <param name="paymentProcessingType">決済処理タイプ</param>
		/// <param name="httpMethod">Http Method</param>
		/// <returns>The response data</returns>
		private static TResponse CallApi<TResponse>(
			string uriPath,
			string requestData,
			LinePayLogInfo logInfo,
			PaymentFileLogger.PaymentProcessingType paymentProcessingType,
			string httpMethod) where TResponse : LinePayResponse
		{
			using (var client = new WebClient())
			{
				try
				{
					var url = Constants.PAYMENT_LINEPAY_API_URL + uriPath;

					// Set HTTP headers
					var nonce = Guid.NewGuid().ToString();
					client.Headers.Add(HTTP_HEADER_CONTENT_TYPE, HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON);
					client.Headers.Add(HTTP_HEADER_XLINE_CHANNEL_ID, Constants.PAYMENT_LINEPAY_CHANNEL_ID);
					client.Headers.Add(HTTP_HEADER_XLINE_AUTHORIZATION, CreateSignature(url, requestData, nonce));
					client.Headers.Add(HTTP_HEADER_XLINE_AUTHORIZATION_NONCE, nonce);

					// Call API
					var responseBytes = (httpMethod == HTTP_METHOD_POST)
					? client.UploadData(url, HTTP_METHOD_POST, Encoding.UTF8.GetBytes(requestData))
					: client.DownloadData(url + "?" + requestData);

					// Handle success response
					var responseBody = Encoding.UTF8.GetString(responseBytes);
					var responseInfo = JsonConvert.DeserializeObject<TResponse>(responseBody);
					WritePaymentLog(paymentProcessingType, responseInfo, logInfo, responseInfo.CreateMessageForLog());

					return responseInfo;
				}
				catch (WebException ex)
				{
					WritePaymentLog(paymentProcessingType, null, logInfo, ex.ToString());

					var errorString = GetResponseError(ex);
					return JsonConvert.DeserializeObject<TResponse>(errorString);
				}
			}
		}
		#endregion

		#region Helper methods
		/// <summary>
		/// Create Line Pay payment signature
		/// </summary>
		/// <param name="requestUri">The request URI</param>
		/// <param name="requestBody">The request body</param>
		/// <param name="nonce">The authorization nonce</param>
		/// <returns>Line Pay payment signature</returns>
		private static string CreateSignature(string requestUri, string requestBody, string nonce)
		{
			using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Constants.PAYMENT_LINEPAY_SECRET_KEY)))
			{
				var bytes = hmac.ComputeHash(
					Encoding.UTF8.GetBytes(Constants.PAYMENT_LINEPAY_SECRET_KEY
						+ new Uri(requestUri).AbsolutePath
						+ requestBody
						+ nonce));

				return Convert.ToBase64String(bytes);
			}
		}

		/// <summary>
		/// Get response error
		/// </summary>
		/// <param name="exception">A web exception</param>
		/// <returns>An error message</returns>
		private static string GetResponseError(WebException exception)
		{
			if (exception.Response == null) return string.Empty;

			using (var reader = new StreamReader(exception.Response.GetResponseStream()))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// LinePayログ書き込み
		/// </summary>
		/// <param name="paymentProcessingType">決済処理タイプ</param>
		/// <param name="response">結果（例外の場合はnull）</param>
		/// <param name="logInfo">ログ情報</param>
		/// <param name="detailMessage">詳細メッセージ</param>
		private static void WritePaymentLog(
			PaymentFileLogger.PaymentProcessingType paymentProcessingType,
			LinePayResponse response,
			LinePayLogInfo logInfo,
			string detailMessage)
		{
			PaymentFileLogger.WritePaymentLog(
				(response != null) ? response.IsSuccess : (bool?)null,
				Constants.FLG_PAYMENT_PAYMENT_ID_LINEPAY,
				PaymentFileLogger.PaymentType.LinePay,
				paymentProcessingType,
				detailMessage,
				logInfo.CreateDictionary());
		}

		/// <summary>
		/// LinePayログ情報
		/// </summary>
		public class LinePayLogInfo
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="order">OrderModel</param>
			public LinePayLogInfo(OrderModel order)
				: this(order.OrderId, order.PaymentOrderId, order.CardTranId)
			{
			}
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="order">注文情報</param>
			public LinePayLogInfo(Hashtable order) : this(
				(string)order[Constants.FIELD_ORDER_ORDER_ID],
				(string)order[Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
				(string)order[Constants.FIELD_ORDER_CARD_TRAN_ID])
			{
			}
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="orderId">注文ID</param>
			/// <param name="paymentOrderId">決済注文ID</param>
			/// <param name="transactionId">トランザクションID（リクエスト時は不要）</param>
			public LinePayLogInfo(
				string orderId,
				string paymentOrderId,
				string transactionId)
			{
				this.OrderId = orderId;
				this.PaymentOrderId = paymentOrderId;
				this.TransactionId = transactionId;
			}

			/// <summary>
			/// ディクショナリ作成
			/// </summary>
			/// <returns>ディクショナリ</returns>
			public Dictionary<string, string> CreateDictionary()
			{
				var result = new Dictionary<string, string>();
				if (string.IsNullOrEmpty(this.OrderId) == false) result.Add("OrderId", this.OrderId);
				if (string.IsNullOrEmpty(this.PaymentOrderId) == false) result.Add("PaymentOrderId", this.PaymentOrderId);
				if (string.IsNullOrEmpty(this.TransactionId) == false) result.Add("TransactionId", this.TransactionId);
				return result;
			}

			/// <summary>注文ID</summary>
			public string OrderId { get; set; }
			/// <summary>決済注文ID</summary>
			public string PaymentOrderId { get; set; }
			/// <summary>トランザクションID</summary>
			public string TransactionId { get; set; }
		}
		#endregion
	}
}
