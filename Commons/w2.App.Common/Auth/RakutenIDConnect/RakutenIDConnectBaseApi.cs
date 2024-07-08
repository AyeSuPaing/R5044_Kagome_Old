/*
=========================================================================================================
  Module      : 楽天IDConnect基底APIクラス(RakutenIDConnectBaseApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;
using w2.App.Common.Order.Payment;
using w2.Common.Web;
using w2.Common.Helper;

namespace w2.App.Common.Auth.RakutenIDConnect
{
	/// <summary>API種別</summary>
	public enum ApiType
	{
		/// <summary>トークン</summary>
		Token,
		/// <summary>ユーザー情報取得</summary>
		UserInfo,
	}

	/// <summary>
	/// 楽天IDConnect基底APIクラス
	/// </summary>
	public class RakutenIDConnectBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="type">API種別</param>
		public RakutenIDConnectBaseApi(ApiType type)
		{
			this.Type = type;
		}

		/// <summary>
		/// POSTリクエスト
		/// </summary>
		/// <param name="urlCreator">URL作成インスタンス</param>
		/// <returns>レスポンスデータ</returns>
		protected string PostHttpRequest(UrlCreator urlCreator)
		{
			var urlAndParam = urlCreator.CreateUrl().Split('?');
			var postData = Encoding.UTF8.GetBytes((urlAndParam.Length > 1) ? urlAndParam[1] : string.Empty);
			// POST送信設定
			var webRequest = (HttpWebRequest)WebRequest.Create(urlAndParam[0]);
			webRequest.Method = WebRequestMethods.Http.Post;
			webRequest.ContentType = @"application/x-www-form-urlencoded";
			webRequest.ContentLength = postData.Length;

			// トークン？
			switch (this.Type)
			{
				case ApiType.Token:
					// Basic認証セット（クライアントID:クライアントSecret）
					webRequest.Headers["Authorization"] =
						string.Format(@"Basic {0}", Convert.ToBase64String(
							Encoding.UTF8.GetBytes(
								string.Format(
									"{0}:{1}",
									Constants.RAKUTEN_ID_CONNECT_CLIENT_ID,
									Constants.RAKUTEN_ID_CONNECT_CLIENT_SECRET))));
					break;

				case ApiType.UserInfo:
					// Bearerセット（発行されたAccess Token）
					webRequest.Headers["Authorization"] = @"Bearer " + this.AccessToken;
					break;
			}

			RakutenIDConnectLogger.WriteDebugLog(
				this.TypeName,
				urlCreator.CreateUrl(),
				PaymentFileLogger.PaymentProcessingType.Request);

			RakutenIDConnectLogger.WriteDebugLog(
				this.TypeName,
				postData.Length.ToString(),
				PaymentFileLogger.PaymentProcessingType.Request);

			RakutenIDConnectLogger.WriteDebugLog(
				this.TypeName,
				webRequest.Headers["Authorization"],
				PaymentFileLogger.PaymentProcessingType.Request);

			// レスポンス取得
			string responseText = null;
			try
			{
				// 送信データの書き込み
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
			// デバッグログ
			RakutenIDConnectLogger.WriteDebugLog(this.TypeName, responseText, PaymentFileLogger.PaymentProcessingType.Request);
			return responseText;
		}

		#region プロパティ
		/// <summary>API種別</summary>
		private ApiType Type { get; set; }
		/// <summary>発行されたAccess Token</summary>
		protected string AccessToken { get; set; }
		/// <summary>API種別名称</summary>
		private string TypeName
		{
			get
			{
				switch (this.Type)
				{
					case ApiType.Token:
						return "token";

					case ApiType.UserInfo:
						return "userinfo";
				}
				return string.Empty;
			}
		}
		#endregion
	}
}