/*
=========================================================================================================
  Module      : GMOマルチ決済APIコネクタ (ApiConnector.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.Paypay
{
	/// <summary>
	/// GMOマルチ決済APIコネクタ
	/// </summary>
	internal abstract class ApiConnector
	{
		/// <summary>HTTPクライアント</summary>
		private static readonly HttpClient _httpClient = new HttpClient();

		/// <summary>
		/// HTTP-POST実行
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="data">データ</param>
		/// <returns>レスポンス</returns>
		public T Post<T>(string url, IHttpApiRequestData data)
		{
			var postString = "UNKNOWN";
			var responseString = "UNKNOWN";
			var statusCode = (HttpStatusCode)0;

			try
			{
				postString = data.CreatePostString();
				var response = _httpClient.PostAsync(
					url,
					new StringContent(
						postString,
						this.Encoding,
						this.MediaType)).Result;

				statusCode = response.StatusCode;
				responseString = this.Encoding.GetString(response.Content.ReadAsByteArrayAsync().Result);

				var result = Deserialize<T>(responseString);
				return result;
			}
			finally
			{
				if (this.LogAction != null)
				{
					this.LogAction.Invoke(url, postString, responseString, statusCode);
				}
			}
		}

		/// <summary>
		/// デシリアライズ
		/// </summary>
		/// <typeparam name="T">デシリアライズする型</typeparam>
		/// <param name="content">コンテンツ</param>
		/// <returns>結果</returns>
		protected abstract T Deserialize<T>(string content);

		/// <summary>コンテンツエンコーディング</summary>
		protected virtual Encoding Encoding
		{
			get { throw new NotImplementedException(); }
		}
		/// <summary>メディアタイプ</summary>
		protected virtual string MediaType
		{
			get { throw new NotImplementedException(); }
		}
		/// <summary>ログ書き込みアクション()</summary>
		public Action<string, string, string, HttpStatusCode> LogAction { get; set; }
	}

	/// <summary>
	/// IdPass用GMOマルチ決済APIコネクタ
	/// </summary>
	internal class PaypayIdPassApiConnector : ApiConnector
	{
		/// <summary>
		/// デシリアライズ
		/// </summary>
		/// <typeparam name="T">デシリアライズする型</typeparam>
		/// <param name="content">コンテンツ</param>
		/// <returns>結果</returns>
		protected override T Deserialize<T>(string content)
		{
			var result = IdPassDeserializer.Deserialize<T>(content);
			return result;
		}

		/// <summary>コンテンツエンコーディング</summary>
		protected override Encoding Encoding
		{
			get { return Encoding.GetEncoding(932); }
		}
		/// <summary>メディアタイプ</summary>
		protected override string MediaType
		{
			get { return "application/x-www-form-urlencoded"; }
		}
	}

	/// <summary>
	/// JSON用GMOマルチ決済APIコネクタ
	/// </summary>
	internal class PaypayJsonApiConnector : ApiConnector
	{
		/// <summary>
		/// デシリアライズ
		/// </summary>
		/// <typeparam name="T">デシリアライズする型</typeparam>
		/// <param name="content">コンテンツ</param>
		/// <returns>結果</returns>
		protected override T Deserialize<T>(string content)
		{
			try
			{
				// GMOはエラー時に[{"errCode":"E01","errInfo":"E00000001"}]のようなレスポンスを返す。
				// このままではGmoMultiPaymentResultにデシリアライズできないので、ここでJSON補正をしてデシリアライズができるようにする
				var result = JsonConvert.DeserializeObject<T>(
					content.StartsWith("[")
						? string.Format("{{\"errors\":{0}}}", content)
						: content);
				return result;
			}
			catch (JsonSerializationException ex)
			{
				throw new Exception("JSONのデシリアライズで例外が発生しました。\r\nJSON:" + content, ex);
			}
		}

		/// <summary>コンテンツエンコーディング</summary>
		protected override Encoding Encoding
		{
			get { return Encoding.UTF8; }
		}
		/// <summary>メディアタイプ</summary>
		protected override string MediaType
		{
			get { return "application/json"; }
		}
	}
}
