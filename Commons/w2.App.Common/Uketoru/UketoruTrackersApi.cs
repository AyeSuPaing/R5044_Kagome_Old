/*
=========================================================================================================
  Module      : ウケトル連携処理 (UketoruTrackersApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace w2.App.Common.Uketoru
{
	/// <summary>
	/// ウケトル連携処理
	/// </summary>
	public class UketoruTrackersApi
	{
		/// <summary>
		/// POST送信
		/// </summary>
		/// <param name="csvFileName">csvファイルの名前</param>
		/// <param name="postData">csvファイルのデータ</param>
		/// <returns>レスポンス</returns>
		public async Task<UketoruResponse> PostHttp(string csvFileName, string postData)
		{
			var postUrl = string.Format("{0}?shop_token={1}", Constants.UKETORU_TRACKERS_API_URL, Constants.UKETORU_TRACKERS_API_SHOP_TOKEN);
			using (var httpClient = new HttpClient())
			using (var request = new HttpRequestMessage(new HttpMethod("POST"), postUrl))
			{
				var multipartContent = new MultipartFormDataContent();
				multipartContent.Add(new ByteArrayContent(Encoding.UTF8.GetBytes(postData)), "csv_file", csvFileName);
				request.Content = multipartContent;

				var result = UketoruAsyncHelper.RunSync(() => GetResponseAsync(httpClient, request));
				return result;
			}
		}

		/// <summary>
		/// リクエストからレスポンス文字列を取得(非同期)
		/// </summary>
		/// <param name="httpClient">接続用HTTPクライアント</param>
		/// <param name="request">リクエスト</param>
		/// <returns>レスポンス文字列</returns>
		private static async Task<UketoruResponse> GetResponseAsync(
			HttpClient httpClient,
			HttpRequestMessage request)
		{
			var response = await httpClient.SendAsync(request);
			var content = await response.Content.ReadAsStringAsync();
			var result = JsonConvert.DeserializeObject<UketoruResponse>(content);
			return result;
		}

		/// <summary>
		/// レスポンス情報
		/// </summary>
		[Serializable]
		public class UketoruResponse
		{
			/// <summary>追跡番号取り込み件数</summary>
			[JsonProperty(PropertyName = "total_count")]
			public string TotalCount { get; set; }
			/// <summary>取り込みに成功した件数</summary>
			[JsonProperty(PropertyName = "success_count")]
			public string SuccessCount { get; set; }
			/// <summary>ステータス</summary>
			[JsonProperty(PropertyName = "status")]
			public string Status { get; set; }
			/// <summary>既にDBに登録されていた伝票番号配列</summary>
			[JsonProperty(PropertyName = "duplicated_tracking_numbers")]
			public List<Dictionary<string, string>> DuplicatedTrackingNumbers { get; set; }
			/// <summary>エラーメッセージ</summary>
			[JsonProperty(PropertyName = "error_message")]
			public string ErrorMessage { get; set; }
		}
	}
}