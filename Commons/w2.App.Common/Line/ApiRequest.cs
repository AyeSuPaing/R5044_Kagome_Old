/*
=========================================================================================================
  Module      : LINE API 共通リクエストクラス (ApiRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using w2.App.Common.Line.Helper;

namespace w2.App.Common.Line
{
	/// <summary>
	/// LINE API 共通リクエストクラス
	/// </summary>
	public class ApiRequest
	{

		/// <summary>JSON形式のメディアタイプ</summary>
		private const string APPLICATION_MEDIA_TYPE_JSON = "application/json";

		/// <summary>
		/// レスポンス文字列を取得
		/// </summary>
		/// <param name="apiUrl">APIのURL</param>
		/// <param name="httpClient">接続用HTTPクライアント</param>
		/// <param name="body">リクエストボディデータ</param>
		/// <returns>レスポンスデータ</returns>
		public ApiResponse GetHttpPostResponse<TBody>(string apiUrl, HttpClient httpClient, TBody body)
			where TBody : new()
		{
			var request = GetHttpPostRequest(Constants.LINE_API_URL_ROOT_PATH + apiUrl, body);
			var result = AsyncHelper.RunSync(() => GetResponseAsync(httpClient, request));
			return result;
		}

		/// <summary>
		/// パラメータを含むリクエストを取得
		/// </summary>
		/// <param name="apiUrl">APIのURL</param>
		/// <param name="body">リクエストボディデータ</param>
		/// <returns>リクエスト</returns>
		private static HttpRequestMessage GetHttpPostRequest<TBody>(string apiUrl, TBody body)
			where TBody : new()
		{
			var option = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};

			var content = JsonConvert.SerializeObject(body, option);
			var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
			{
				Content = new StringContent(
					content,
					Encoding.UTF8,
					APPLICATION_MEDIA_TYPE_JSON)
			};
			request.Headers.Add("X-XLINER-TOKEN", Constants.LINE_API_AUTHENTICATION_KEY);
			return request;
		}

		/// <summary>
		/// リクエストからレスポンス文字列を取得(非同期)
		/// </summary>
		/// <param name="httpClient">接続用HTTPクライアント</param>
		/// <param name="request">リクエスト</param>
		/// <returns>レスポンス文字列</returns>
		private static async Task<ApiResponse> GetResponseAsync(HttpClient httpClient, HttpRequestMessage request)
		{
			var response = await httpClient.SendAsync(request);
			var content = await response.Content.ReadAsStringAsync();
			var result = new ApiResponse(response.StatusCode, content, response.ReasonPhrase);
			return result;
		}
	}
}
