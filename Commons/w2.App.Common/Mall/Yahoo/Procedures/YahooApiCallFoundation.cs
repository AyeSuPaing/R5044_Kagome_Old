/*
=========================================================================================================
  Module      : YAHOO API API実行基盤クラス (YahooApiCallFoundation.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using w2.App.Common.Mall.Yahoo.Foundation;
using w2.App.Common.Mall.Yahoo.Interfaces;
using w2.Common.Logger;

namespace w2.App.Common.Mall.Yahoo.Procedures
{
	/// <summary>
	/// YAHOO API API実行基盤クラス
	/// </summary>
	public class YahooApiCallFoundation : IYahooApiCallFoundation
	{
		/// <summary>HTTPクライアント</summary>
		private static readonly HttpClient s_httpClient = new HttpClient(); // staticなのが大事

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public YahooApiCallFoundation() { }

		/// <summary>
		/// HTTP GET
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="mediaType">メディアタイプ</param>
		/// <param name="authorization">認証ヘッダ</param>
		/// <returns>結果オブジェクト</returns>
		public async Task<SharedApiResponse> HttpGet(
			string url,
			MediaTypeWithQualityHeaderValue mediaType,
			string authorization)
		{
			// ヘッダー追加
			s_httpClient.DefaultRequestHeaders.Accept.Clear();
			s_httpClient.DefaultRequestHeaders.Accept.Add(mediaType);
			s_httpClient.DefaultRequestHeaders.Remove("Authorization");
			s_httpClient.DefaultRequestHeaders.Add("Authorization", "Basic ");

			// 実行
			var response = new HttpResponseMessage();
			try
			{
				response = await s_httpClient.GetAsync(url);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				throw ex;
			}

			var responseBody = new SharedApiResponse(
				response.StatusCode,
				response.Content?.ReadAsStringAsync().Result,
				response.ReasonPhrase);
			return responseBody;
		}

		/// <summary>
		/// HTTP POST
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="content">コンテント</param>
		/// <param name="encoding">エンコード</param>
		/// <param name="contentType">コンテントタイプ</param>
		/// <param name="authorization">認証ヘッダ</param>
		/// <param name="signature">シグネチャー</param>
		/// <param name="publicKeyVersion">公開鍵バージョン</param>
		/// <returns>結果オブジェクト</returns>
		public async Task<SharedApiResponse> HttpPost(
			string url,
			string content,
			Encoding encoding,
			string contentType,
			string authorization,
			string signature = "",
			string publicKeyVersion = "")
		{
			s_httpClient.DefaultRequestHeaders.Accept.Clear();
			s_httpClient.DefaultRequestHeaders.Remove("Authorization");
			s_httpClient.DefaultRequestHeaders.Add("Authorization", authorization);
			s_httpClient.DefaultRequestHeaders.Remove("X-sws-signature");
			s_httpClient.DefaultRequestHeaders.Remove("X-sws-signature-version");
			if (string.IsNullOrEmpty(signature) == false && string.IsNullOrEmpty(publicKeyVersion) == false)
			{
				s_httpClient.DefaultRequestHeaders.Add("X-sws-signature", signature);
				s_httpClient.DefaultRequestHeaders.Add("X-sws-signature-version", publicKeyVersion);
			}

			var stringContent = new StringContent(content, encoding, contentType);
			var response = new HttpResponseMessage();
			try
			{
				response = await s_httpClient.PostAsync(url, stringContent);
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				throw ex;
			}

			response.Headers.TryGetValues("X-SWS-Authorize-Status", out var xSwsAuthorizeStatusHeader);
			response.EnsureSuccessStatusCode();
			var responseBody = new SharedApiResponse(
				response.StatusCode,
				response.Content?.ReadAsStringAsync().Result,
				response.ReasonPhrase,
				(xSwsAuthorizeStatusHeader != null && xSwsAuthorizeStatusHeader.Any())
					? xSwsAuthorizeStatusHeader.FirstOrDefault() ?? ""
					: "");
			return responseBody;
		}
	}
}
