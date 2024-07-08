/*
=========================================================================================================
  Module      : JsonファイルをAPIへ送信(SendJsonToApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.EScott.Helper;

namespace w2.App.Common.OperationLog
{
	/// <summary>
	/// POSTとして送信する
	/// </summary>
	public class SendJsonToApi
	{
		/// <summary>送信パラメータ</summary>
		private readonly string m_parameter;
		/// <summary>リクエストURL</summary>
		private readonly string m_requestUrl;
		/// <summary>エンコーディング</summary>
		private readonly Encoding m_encodingPost = Encoding.GetEncoding("UTF-8");

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="requestUrl">リクエストURL</param>
		/// <param name="parameter">パラメタ</param>
		public SendJsonToApi(string requestUrl, string parameter)
		{
			m_requestUrl = requestUrl;
			m_parameter = parameter;
		}

		/// <summary>
		/// POST送信
		/// </summary>
		/// <returns>response text</returns>
		public string PostHttp()
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create(m_requestUrl);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";

			using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				var json = m_parameter;
				streamWriter.Write(json);
				streamWriter.Close();
			}
			var result = "";
			using (var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse())
			{
				using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), m_encodingPost))
				{
					result = streamReader.ReadToEnd();
				}
			}
			return result;
		}
	}
}
