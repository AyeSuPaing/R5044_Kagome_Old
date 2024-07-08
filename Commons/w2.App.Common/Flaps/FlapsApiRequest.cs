/*
=========================================================================================================
  Module      : FLAPS API サービスクラス(FlapsApiResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using w2.App.Common.Flaps.Logger;
using w2.Common.Logger;

namespace w2.App.Common.Flaps
{
	/// <summary>
	/// FLAPS API 共通リクエストクラス
	/// </summary>
	internal class FlapsApiRequest
	{
		/// <summary>
		/// レスポンス文字列を取得
		/// </summary>
		/// <param name="requestXml">リクエスト文</param>
		/// <param name="apiName">API名</param>
		/// <returns>レスポンスデータ</returns>
		internal string GetResponse(XDocument requestXml, string apiName)
		{
			WriteStartLog(apiName);

			// 送信用XML作成
			var sendData = CreateRequestXml(requestXml);
			var request = GetRequest(sendData, apiName);

			return request;
		}

		/// <summary>
		/// POST送信用リクエスト
		/// </summary>
		/// <param name="document">リクエストXML</param>
		/// <returns>POST送信用リクエスト</returns>
		protected string CreateRequestXml(XDocument document)
		{
			return document.ToString();
		}

		/// <summary>
		/// リクエストを取得
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="apiName">API名</param>
		/// <returns>リクエスト</returns>
		private static string GetRequest(string request, string apiName)
		{
			var postData = Encoding.UTF8.GetBytes(request);

			// MD5
			var md5 = MD5.Create();
			var sb = new StringBuilder();
			var postMd5Data = md5.ComputeHash(Encoding.UTF8.GetBytes(Constants.FLAPS_API_TOKEN + request));
			foreach (var b in postMd5Data)
			{
				sb.Append(b.ToString("x2"));
			}

			try
			{
				// POST送信設定
				var webRequest = (HttpWebRequest)WebRequest.Create(Constants.FLAPS_API_URL);
				webRequest.Method = WebRequestMethods.Http.Post;
				webRequest.ContentType = "text/xml";
				webRequest.ContentLength = postData.Length;
				webRequest.Headers["signature"] = sb.ToString();

				// レスポンス取得
				using (var stream = webRequest.GetRequestStream())
				{
					stream.Write(postData, 0, postData.Length);

					using (var httpResponse = (HttpWebResponse)webRequest.GetResponse())
					{
						var statusCode = httpResponse.StatusCode;
						using (var reader = new StreamReader(httpResponse.GetResponseStream()))
						{
							var responseText = reader.ReadToEnd();
							WriteEndLog(apiName, statusCode, request, responseText);

							return responseText;
						}
					}
				}
			}
			catch (Exception exception)
			{
				FileLogger.WriteError(string.Format("{0} - {1}", Constants.FLAPS_API_URL, exception.Message));
				return string.Empty;
			}
		}

		/// <summary>
		/// ログ出力（開始）
		/// </summary>
		/// <param name="name">接続API名</param>
		protected static void WriteStartLog(string name)
		{
			ApiLogger.Write(string.Format("{0} {1}", "[開始]", name));
		}

		/// <summary>
		/// ログ出力（終了）
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="code">ステータスコード</param>
		/// <param name="request">リクエスト</param>
		/// <param name="response">レスポンスデータ</param>
		protected static void WriteEndLog(string name, HttpStatusCode code, string request, string response)
		{
			var output = string.Format(
				"[{1}]{0}[RequestParam]{0}{2}{0}[Response]{0}{3}",
				Environment.NewLine,
				code,
				request,
				response);
			ApiLogger.Write(string.Format("{0} {1} {2}", "[終了]", name, output));
		}
	}
}
