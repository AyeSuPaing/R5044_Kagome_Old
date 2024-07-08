/*
=========================================================================================================
  Module      : LINE API サービス基底クラス(ApiServiceBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using w2.App.Common.Line.Logger;

namespace w2.App.Common.Line
{
	/// <summary>
	/// サービス基底クラス
	/// </summary>
	public class ApiServiceBase
	{
		/// <summary>API接続用Http</summary>
		private static readonly HttpClient m_httpClient;
		/// <summary>Process type send</summary>
		public const string PROCESS_TYPE_SEND = "Send";
		/// <summary>Process type receive</summary>
		public const string PROCESS_TYPE_RECEIVE = "Receive";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		static ApiServiceBase()
		{
			m_httpClient = new HttpClient();
		}

		/// <summary>
		/// API実行結果取得
		/// </summary>
		/// <param name="apiName">接続API名</param>
		/// <param name="body">リクエストボディデータ</param>
		/// <returns>レスポンスデータ</returns>
		protected static TResult GetResult<TResult, TBody>(string apiName, TBody body)
			where TResult : ResultBase, new()
			where TBody : new()
		{
			WriteStartLog(apiName, HttpMethod.Post, PROCESS_TYPE_SEND);

			var response = new ApiRequest().GetHttpPostResponse(apiName, m_httpClient, body);
			var result = GetResult<TResult>(response);

			var option = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};
			var request = JsonConvert.SerializeObject(body, option);

			WriteEndLog(apiName, response.StatusCode, response.ReasonPhrase, request, response.Content);

			return result;
		}
		/// <summary>
		/// API実行結果取得
		/// </summary>
		/// <param name="response">レスポンス</param>
		/// <returns>実行結果モデル</returns>
		private static TResult GetResult<TResult>(ApiResponse response)
			where TResult : ResultBase, new()
		{
			TResult result;
			try
			{
				var root = JsonConvert.DeserializeObject<ResultRoot<TResult>>(response.Content);
				result = root.Data ?? new TResult();
			}
			catch
			{
				result = new TResult();
			}
			result.StatusCode = response.StatusCode;
			return result;
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="httpMethod">httpメソッド</param>
		/// <param name="processType">Process type</param>
		protected static void WriteStartLog(string name, HttpMethod httpMethod, string processType)
		{
			ApiLogger.Write(string.Format("{0} {1}", "[開始]", name));
			ApiLogger.Write(string.Format("{0} {1}", "[処理種類]", processType));
			ApiLogger.Write(string.Format("{0} {1}", "[HttpMethod]", httpMethod.Method));
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="name">接続API名</param>
		/// <param name="code">ステータスコード</param>
		/// <param name="message">実行結果</param>
		/// <param name="param">リクエストデータ</param>
		/// <param name="response">レスポンスデータ</param>
		protected static void WriteEndLog(
			string name,
			HttpStatusCode code,
			string message,
			string param,
			string response)
		{
			var output = ApiLogger.GetOutPut(code, message, param, response);
			ApiLogger.Write(string.Format("{0} {1} {2}", "[終了]", name, output));
		}
	}
}
