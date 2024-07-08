/*
=========================================================================================================
  Module      : LINE直接連携マネージャー(LineDirectConnectManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using w2.App.Common.Gooddeal;
using w2.App.Common.Line.LineDirectMessage;
using w2.App.Common.Line.LineDirectMessage.RequestType;
using w2.Common.Helper;
using w2.Common.Logger;

namespace w2.App.Common.Line.LineDirectConnect
{
	/// <summary>
	/// LINE直接連携マネージャークラス
	/// </summary>
	public class LineDirectConnectManager
	{
		/// <summary> メディアタイプ(JSON形式) </summary>
		private const string APPLICATION_MEDIA_TYPE_JSON = "application/json";
		private readonly Encoding m_encodingPost = Encoding.GetEncoding("UTF-8");

		/// <summary>
		/// アクセストークンの取得
		/// </summary>
		/// <param name="apiUrl">アクセストークン取得URL</param>
		/// <param name="code">認可コード</param>
		/// <returns>内容</returns>
		public string GetAccessToken(string apiUrl, string code)
		{
			var parameters = new Dictionary<string, string>()
			{
				{Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_GRANT_TYPE, Constants.LINE_DIRECT_CONNECT_REQUEST_VALUE_GRANT_TYPE},
				{Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_CODE, code},
				{Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_REDIRECT_URI, Constants.LINE_DIRECT_CONNECT_REQUEST_VALUE_REDIRECT_URI},
				{Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_CLIENT_ID, Constants.LINE_DIRECT_CONNECT_CLIENT_ID},
				{Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_CLIENT_SECRET, Constants.LINE_DIRECT_CONNECT_CLIENT_SECRET}
			};
			var requestString = GenerateRequestParamter(parameters);

			var postData = m_encodingPost.GetBytes(requestString);

			var webRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
			webRequest.Method = WebRequestMethods.Http.Post;
			webRequest.ContentType = @"application/x-www-form-urlencoded";
			webRequest.ContentLength = postData.Length;

			string responseText = null;
			try
			{
				using (var postStream = webRequest.GetRequestStream())
				{
					postStream.Write(postData, 0, postData.Length);
				}

				using (var responseStream = webRequest.GetResponse().GetResponseStream())
				using (var sr = new StreamReader(responseStream, Encoding.UTF8))
				{
					responseText = sr.ReadToEnd();
				}
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					using (var responseStream = ((HttpWebResponse)ex.Response).GetResponseStream())
					using (var sr = new StreamReader(responseStream, Encoding.UTF8))
					{
						responseText = sr.ReadToEnd();
					}
				}
				else
				{
					responseText = SerializeHelper.SerializeJson(new Dictionary<string, object>() { { "error", "system_error" }, { "error_description", ex.ToString() } });
				}
			}
			return responseText;
		}

		/// <summary>
		/// プロバイダーユーザーIDの取得
		/// </summary>
		/// <param name="apiUrl">APIURL</param>
		/// <param name="accessToken">アクセストークン</param>
		/// <returns>内容</returns>
		public string GetProviderUserId(string apiUrl, string accessToken)
		{
			var webRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
			webRequest.Method = WebRequestMethods.Http.Get;
			webRequest.Headers["Authorization"] = string.Format("Bearer {0}", accessToken);

			string responseText = null;
			try
			{
				using (var responseStream = webRequest.GetResponse().GetResponseStream())
				using (var sr = new StreamReader(responseStream, Encoding.UTF8))
				{
					responseText = sr.ReadToEnd();
				}
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					using (var responseStream = ((HttpWebResponse)ex.Response).GetResponseStream())
					using (var sr = new StreamReader(responseStream, Encoding.UTF8))
					{
						responseText = sr.ReadToEnd();
					}
				}
				else
				{
					responseText = SerializeHelper.SerializeJson(new Dictionary<string, object>() { { "error", "system_error" }, { "error_description", ex.ToString() } });
				}
			}
			return responseText;
		}

		/// <summary>
		/// リクエストパラメータ生成
		/// </summary>
		/// <returns>リクエストパラメータ</returns>
		private string GenerateRequestParamter(Dictionary<string,string> parameter)
		{
			var rseult = string.Join("&", parameter.Select(kvp => kvp.Key + "=" + kvp.Value));
			return rseult;
		}

		#region メッセージングAPI
		/// <summary>
		/// LINEプッシュメッセージ送信
		/// </summary>
		/// <param name="lineId">送信先ID（LINEの「userId」「groupId」「roomId」）</param>
		/// <param name="messages">送信するメッセージ</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>エラーメッセージ</returns>
		public LineResponseMessage SendPushMessage(string lineId, ILineMessage[] messages, string userId = null)
		{
			var pushMesssage = new LineRequestPush
			{
				To = lineId,
				Messages = messages
			};

			var response = SendLineMessage("push", pushMesssage, userId);
			return response;
		}

		/// <summary>
		/// LINEマルチキャストメッセージ送信
		/// </summary>
		/// <param name="lineIdArray">送信先ID配列群（LINEの「userId」「groupId」「roomId」）</param>
		/// <param name="messages">送信するメッセージ</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>エラーメッセージ</returns>
		public LineResponseMessage SendMulticastMessage(string[] lineIdArray, ILineMessage[] messages, string userId = null)
		{
			var multicastMesssage = new LineRequestMulticast()
			{
				To = lineIdArray,
				Messages = messages
			};

			var response = SendLineMessage("multicast", multicastMesssage, userId);
			return response;
		}

		/// <summary>
		/// LINE送信共通
		/// </summary>
		/// <typeparam name="TBody">LINE送信モデル</typeparam>
		/// <param name="requestType">リクエスト方法</param>
		/// <param name="message">メッセージ内容</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns></returns>
		private LineResponseMessage SendLineMessage<TBody>(string requestType, TBody message, string userId = null)
			where TBody : new()
		{
			var response = SendPostRequest(requestType, message);
			OutPutLog(response, userId);

			// 送信成功でエラーメッセージを返さない
			if ((response.StatusCode == HttpStatusCode.OK)) return null;

			// レスポンス(JSON)をオブジェクトに変換
			var jasonObj = JObject.Parse(response.Content);
			var responseMsg = new LineResponseMessage(response.StatusCode, jasonObj);
			return responseMsg;
		}

		/// <summary>
		/// POST送信実行
		/// </summary>
		/// <param name="reqestType">リクエストの種類</param>
		/// <param name="body">リクエストボディデータ</param>
		/// <returns>レスポンスデータ</returns>
		private ApiResponse SendPostRequest<TBody>(string reqestType, TBody body)
			where TBody : new()
		{
			var request = CreateHttpPostRequest(reqestType, body);
			var client = CreateClient(Constants.LINE_DIRECT_CHANNEL_ACCESS_TOKEN);
			var result = AsyncHelper.RunSync(() => SendAsync(client, request));
			return result;
		}

		/// <summary>
		/// パラメータを含むリクエストを取得
		/// </summary>
		/// <param name="reqestType">リクエストの種類</param>
		/// <param name="body">リクエストボディデータ</param>
		/// <returns>リクエスト</returns>
		private static HttpRequestMessage CreateHttpPostRequest<TBody>(string reqestType, TBody body)
			where TBody : new()
		{
			var content = CreateStringContent(body);
			var request = new HttpRequestMessage(HttpMethod.Post, reqestType)
			{
				Content = content
			};
			return request;
		}

		/// <summary>
		/// Client生成
		/// </summary>
		/// <param name="channelAccessToken">LINEのチャネルアクセストークン</param>
		/// <returns>HttpClient</returns>
		private static HttpClient CreateClient(string channelAccessToken)
		{
			var client = new HttpClient()
			{
				BaseAddress = new Uri(Constants.LINE_DIRECT_MESSAGING_API_URL)
			};
			client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", channelAccessToken));

			return client;
		}

		/// <summary>
		/// リクエストのBody部分の作成
		/// </summary>
		/// <typeparam name="TBody">送信内容クラス</typeparam>
		/// <param name="messages">送信内容</param>
		/// <returns>HTTPコンテンツ</returns>
		private static StringContent CreateStringContent<TBody>(TBody messages)
		{
			var content = JsonConvert.SerializeObject(
				messages,
				Formatting.None,
				new JsonSerializerSettings
				{
					NullValueHandling = NullValueHandling.Ignore
				});

			return new StringContent(content, Encoding.UTF8, APPLICATION_MEDIA_TYPE_JSON);
		}

		/// <summary>
		/// リクエスト送信(非同期)
		/// </summary>
		/// <param name="httpClient">接続用HTTPクライアント</param>
		/// <param name="request">リクエスト内容</param>
		/// <returns>レスポンスクラス</returns>
		private static async Task<ApiResponse> SendAsync(HttpClient httpClient, HttpRequestMessage request)
		{
			var response = await httpClient.SendAsync(request);
			var content = await response.Content.ReadAsStringAsync();
			var result = new ApiResponse(response.StatusCode, content, response.ReasonPhrase);
			return result;
		}

		/// <summary>
		/// Log出力
		/// </summary>
		/// <param name="res">送信結果</param>
		/// <param name="userId">ユーザーID</param>
		private void OutPutLog(ApiResponse res, string userId)
		{
			var logMsg = (res.StatusCode == HttpStatusCode.OK)
				? string.Format(
					"LINE送信に成功。\r\n　- ユーザーID：{0}",
					string.IsNullOrEmpty(userId) ? "不明" : userId)
				: string.Format(
					"LINE送信に失敗。\r\n　- ユーザーID：{0}\r\n　- ステータスコード：{1}({2})\r\n　- エラー詳細：{3}",
					string.IsNullOrEmpty(userId) ? "不明" : userId,
					res.StatusCode,
					(int)res.StatusCode,
					res.Content);

			// Log出力
			FileLogger.Write("line", logMsg);
		}
		#endregion
	}
}
