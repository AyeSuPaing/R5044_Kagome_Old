/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) WebHelperクラス(WebApiHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Helper
{
	/// <summary>
	/// 後付款(TriLink後払い) WebHelperクラス
	/// </summary>
	public class WebApiHelper
	{
		/// <summary>
		/// POST実行
		/// </summary>
		/// <param name="requestUrl">リクエストURL</param>
		/// <param name="requestData">リクエストデータ(JSON)</param>
		/// <param name="addRequestHeaders">追加リクエストHeader配列</param>
		public static string PostHttpRequest(string requestUrl, string requestData = "", List<KeyValuePair<string, string>> addRequestHeaders = null)
		{
			return ExecHttpRequest(WebRequestMethods.Http.Post, requestUrl, requestData, addRequestHeaders);
		}

		/// <summary>
		/// PUT実行
		/// </summary>
		/// <param name="requestUrl">リクエストURL</param>
		/// <param name="requestData">リクエストデータ(JSON)</param>
		/// <param name="addRequestHeaders">追加リクエストHeader配列</param>
		public static string PutHttpRequest(string requestUrl, string requestData = "", List<KeyValuePair<string, string>> addRequestHeaders = null)
		{
			return ExecHttpRequest(WebRequestMethods.Http.Put, requestUrl, requestData, addRequestHeaders);
		}

		/// <summary>
		/// DELETE実行
		/// </summary>
		/// <param name="requestUrl">リクエストURL</param>
		/// <param name="requestData">リクエストデータ(JSON)</param>
		/// <param name="addRequestHeaders">追加リクエストHeader配列</param>
		public static string DeleteHttpRequest(string requestUrl, string requestData = "", List<KeyValuePair<string, string>> addRequestHeaders = null)
		{
			return ExecHttpRequest("DELETE", requestUrl, requestData, addRequestHeaders);
		}

		/// <summary>
		/// リクエスト実行
		/// </summary>
		/// <param name="httpMethod">HTTPメソッド</param>
		/// <param name="requestUrl">リクエストURL</param>
		/// <param name="requestData">リクエストデータ(JSONデータ)</param>
		/// <param name="addRequestHeaders">追加リクエストHeader配列</param>
		/// <returns>レスポンスデータ(JSONデータ)</returns>
		private static string ExecHttpRequest(string httpMethod, string requestUrl, string requestData, List<KeyValuePair<string, string>> addRequestHeaders = null)
		{
			var byteData = Encoding.UTF8.GetBytes(requestData);

			// リクエスト設定
			var request = (HttpWebRequest)WebRequest.Create(requestUrl);
			request.Headers.Add("x-api-key", Constants.PAYMENT_SETTING_TRILINK_AFTERPAY_API_KEY);

			if (addRequestHeaders != null)
			{
				addRequestHeaders.ForEach(h => { request.Headers.Add(h.Key, h.Value); });
			}

			request.Method = httpMethod;
			request.ContentType = "application/json; charset=UTF-8";
			request.Accept = "application/json";
			request.ContentLength = byteData.Length;

			// 送信データの書き込み
			using (var requestStream = request.GetRequestStream())
			{
				requestStream.Write(byteData, 0, byteData.Length);
			}

			// レスポンス取得
			string responseData = null;
			try
			{
				using (var responseStream = request.GetResponse().GetResponseStream())
				using (var streamReader = new StreamReader(responseStream, Encoding.UTF8))
				{
					responseData = streamReader.ReadToEnd();
				}
			}
			catch (WebException e)
			{
				if (e.Response != null)
				{
					using (var errorReaponseStream = e.Response.GetResponseStream())
					using (var errorStreamReader = new StreamReader(errorReaponseStream, Encoding.UTF8))
					{
						responseData = errorStreamReader.ReadToEnd();
					}
				}
			}

			return responseData;
		}
	}
}
