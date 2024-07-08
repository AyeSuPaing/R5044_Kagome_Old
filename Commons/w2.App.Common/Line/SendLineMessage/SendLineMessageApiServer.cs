/*
=========================================================================================================
  Module      : LINE API 情報送信サービスクラス(SendLineMessageApiServer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;

namespace w2.App.Common.Line.SendLineMessage
{
	/// <summary>
	/// 情報送信サービス
	/// </summary>
	public class SendLineMessageApiServer : ApiServiceBase
	{
		/// <summary>
		/// 情報送信サービス
		/// </summary>
		/// <param name="input">送信メッセージ</param>
		/// <returns>返却値</returns>
		public static LineMessageResult SendOrderComplete(LineMessagePost input)
		{
			var result = GetResult<LineMessageResult, List<LineMessagePost>>("regist", new List<LineMessagePost> { input });
			return result;
		}

		/// <summary>
		/// Write receipt information log
		/// </summary>
		/// <param name="request">Request</param>
		/// <param name="response">Response</param>
		/// <param name="responseData">Response data</param>
		public static void WriteReceiptInfoLog(string request, HttpResponse response, object responseData)
		{
			var apiName = "regist";
			WriteStartLog(apiName, HttpMethod.Get, ApiServiceBase.PROCESS_TYPE_RECEIVE);
			WriteEndLog(
				apiName,
				(HttpStatusCode)response.StatusCode,
				response.StatusDescription,
				request,
				JsonConvert.SerializeObject(responseData));
		}
	}
}
