/*
=========================================================================================================
  Module      : Recustomer連携Facade(RecustomerApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using w2.App.Common.Recustomer.OrderImporter;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Recustomer
{
	/// <summary>
	/// Recustomer連携Facadeクラス
	/// </summary>
	public class RecustomerApiFacade
	{
		/// <summary>
		/// OrderImporterAPI連携
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="shippedDate">出荷完了日</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>エラーメッセージ</returns>
		public static string OrderImporter(OrderModel order, string shippedDate, string lastChanged, SqlAccessor accessor)
		{
			var url = Constants.RECUSTOMER_API_ORDER_IMPORTER_URL;
			var request = new OrderImporterRequest(order, shippedDate);
			var response = Post<OrderImporterRequest, OrderImporterResponse>(request, url);
			var result = (StringUtility.ToEmpty(response.Results) == "success");

			var errorMessage = RecustomerApiLogger.WriteRecustomerResponseApiLog(order, lastChanged, result, response, accessor);

			return result ? string.Empty : errorMessage;
		}

		/// <summary>
		/// Post
		/// </summary>
		/// <typeparam name="TRequest">リクエスト型</typeparam>
		/// <typeparam name="TResponse">レスポンス型</typeparam>
		/// <param name="url">URL</param>
		/// <param name="request">リクエスト</param>
		/// <returns>レスポンス</returns>
		private static TResponse Post<TRequest, TResponse>(TRequest request, string url)
			where TRequest : RecustomerApiPostRequestBase
		{
			var responseData = ExecHttpRequest<TResponse>(
				url,
				WebRequestMethods.Http.Post,
				request.CreatePostString());

			return responseData;
		}

		/// <summary>
		/// リクエスト実行
		/// </summary>
		/// <typeparam name="TResponse">レスポンス型</typeparam>
		/// <param name="httpMethod">HTTPメソッド</param>
		/// <param name="url">リクエストURL</param>
		/// <param name="requestData">リクエストデータ(JSONデータ)</param>
		/// <returns>レスポンスデータ</returns>
		private static TResponse ExecHttpRequest<TResponse>(
			string url,
			string httpMethod,
			string requestData)
		{
			try
			{
				BeforeRequestProc(requestData);

				// リクエスト設定
				var request = (HttpWebRequest)WebRequest.Create(url);
				request.Method = httpMethod;
				request.Headers["api_store_key"] = Constants.RECUSTOMER_API_STOER_KEY;
				request.Headers["api_token"] = Constants.RECUSTOMER_API_TOKEN;
				request.Accept = "application/json";
				if (string.IsNullOrEmpty(requestData) == false)
				{
					var requestBody = Encoding.UTF8.GetBytes(requestData);
					request.ContentType = "application/json; charset=UTF-8";
					request.ContentLength = requestBody.Length;
					using (var requestStream = request.GetRequestStream())
					{
						requestStream.Write(requestBody, 0, requestBody.Length);
					}
				}

				// リクエスト送信
				var responseData = "";
				using (var responseStream = request.GetResponse().GetResponseStream())
				using (var streamReader = new StreamReader(responseStream))
				{
					responseData = streamReader.ReadToEnd();
				}
				var responseResult = JsonConvert.DeserializeObject<TResponse>(responseData);

				return responseResult;
			}
			catch (WebException ex)
			{
				// エラーはHTTPエラーで返ってくるため、catchしてレスポンス内容を読み取り外部連携メモとログに落とす
				var responseData = "";
				using (var responseStream = ex.Response.GetResponseStream())
				using (var streamReader = new StreamReader(responseStream))
				{
					responseData = streamReader.ReadToEnd();
				}

				var responseResult = JsonConvert.DeserializeObject<TResponse>(responseData);
				return responseResult;
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				return default;
			}
		}

		/// <summary>
		/// リクエスト前処理
		/// </summary>
		/// <param name="request">リクエストデータ(JSONデータ)</param>
		private static void BeforeRequestProc(string request)
		{
			if (Constants.RECUSTOMER_API_REQUEST_LOG_EXPORT_ENABLED) RecustomerApiLogger.WriteRecustomerRequestApiLog(request);
		}
	}
}
