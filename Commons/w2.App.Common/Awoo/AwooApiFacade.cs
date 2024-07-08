/*
=========================================================================================================
  Module      : Awoo連携Facade(AwooApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using w2.App.Common.Awoo.ClassifyProductType;
using w2.App.Common.Awoo.GetPage;
using w2.App.Common.Awoo.GetTags;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace w2.App.Common.Awoo
{
	/// <summary>
	/// Awoo連携Facadeクラス
	/// </summary>
	public class AwooApiFacade
	{
		/// <summary>
		/// ページ取得
		/// </summary>
		/// <param name="getRequest">リクエスト</param>
		/// <returns>レスポンス</returns>
		public static GetPageResponse GetPage(GetPageRequest getRequest)
		{
			var pageUrlAction = string.Format(Constants.AWOO_API_PAGE_ACTION, Constants.AWOO_NUNUNIID);
			var url = Constants.AWOO_API_SERVER + pageUrlAction;
			var result = Get<GetPageResponse>(url, getRequest);
			return result;
		}

		/// <summary>
		/// おすすめタグ取得
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="request">リクエスト</param>
		/// <returns>レスポンス</returns>
		public static GetTagsResponse GetTags(string productId, GetTagsRequest request)
		{
			var pageUrlAction = string.Format(Constants.AWOO_API_TAGS_ACTION, Constants.AWOO_NUNUNIID, productId);
			var url = Constants.AWOO_API_SERVER + pageUrlAction;
			var result = Get<GetTagsResponse>(url, request);
			return result;
		}

		/// <summary>
		/// 商品カテゴリに紐づいた関連タグを取得
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <returns>レスポンス</returns>
		public static ClassifyProductTypeResponse GetTagsByClassifyProductType(ClassifyProductTypeRequest request)
		{
			var pageUrlAction = string.Format(Constants.AWOO_API_CLASSIFYPRODUCTTYPE_ACTION, Constants.AWOO_NUNUNIID);
			var url = Constants.AWOO_API_SERVER + pageUrlAction;
			var result = Post<ClassifyProductTypeRequest, ClassifyProductTypeResponse>(request, url);
			return result;
		}

		/// <summary>
		/// Get
		/// </summary>
		/// <typeparam name="TResponse">レスポンス型</typeparam>
		/// <param name="url">URL</param>
		/// <param name="getRequest">リクエスト</param>
		/// <returns>レスポンス</returns>
		private static TResponse Get<TResponse>(
			string url,
			AwooApiGetRequestBase getRequest)
		{
			var responseData = ExecHttpRequest<TResponse>(
				url + "?" + getRequest.CreatePostString(),
				WebRequestMethods.Http.Get,
				"");
			return responseData;
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
			where TRequest : AwooApiPostRequestBase
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
				BeforeRequestProc(url, requestData);

				// リクエスト設定
				var request = (HttpWebRequest)WebRequest.Create(url);
				request.Method = httpMethod;
				request.Headers["Authorization"] = string.Format("Bearer {0}", Constants.AWOO_AUTHENTICATION_BAERER_TOKEN);
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
				using (var streamReader = new StreamReader(responseStream, Encoding.UTF8))
				{
					responseData = streamReader.ReadToEnd();
				}
				var responseResult = JsonConvert.DeserializeObject<TResponse>(responseData);
				AfterRequestProc(url, responseData);

				return responseResult;
			}
			catch (Exception ex)
			{
				RequestErrorProc(url, ex);
				return default;
			}
		}

		/// <summary>
		/// リクエスト前処理
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="request">リクエストデータ(JSONデータ)</param>
		private static void BeforeRequestProc(string url, string request)
		{
			var text = url + "\t" + request;
			AwooApiLogger.WriteAwooApiLog(
				AwooApiLogger.AwooApiProcessingType.ApiRequestBegin,
				text);
		}

		/// <summary>
		/// リクエスト後処理
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="response">レスポンスデータ(JSONデータ)</param>
		private static void AfterRequestProc(string url, string response)
		{
			var text = url + "\t" + response;
			AwooApiLogger.WriteAwooApiLog(
				AwooApiLogger.AwooApiProcessingType.ApiRequestEnd,
				text);
		}

		/// <summary>
		/// リクエストエラー処理
		/// </summary>
		/// <param name="url">Url</param>
		/// <param name="exception">Exception</param>
		private static void RequestErrorProc(
			string url,
			Exception exception)
		{
			var message = "";
			if (exception is WebException webException)
			{
				if (webException.Status == WebExceptionStatus.ProtocolError)
				{
					try
					{
						using (var responseStream = webException.Response.GetResponseStream())
						using (var streamReader = new StreamReader(responseStream))
						{
							var response = streamReader.ReadToEnd();
							var requestJson = (JObject)JsonConvert.DeserializeObject(response);
							if (requestJson["errmsg"] != null)
							{
								message += requestJson["errmsg"].ToString();
							}

							if (requestJson["result"].Any())
							{
								message += requestJson["result"][0].ToString();
							}
						}
					}
					catch (Exception) { }
				}
			}

			if (string.IsNullOrEmpty(message))
			{
				message = exception.Message;
			}

			AwooApiLogger.WriteAwooApiLog(
				AwooApiLogger.AwooApiProcessingType.ApiRequestError,
				url + "\t" + message);
		}
	}
}
