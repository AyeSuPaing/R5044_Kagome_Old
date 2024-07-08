/*
=========================================================================================================
  Module      : WebApiコネクタクラス(HttpApiConnector.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Net.Security;
using System.Text;

namespace w2.Common.Web
{
	/// <summary>
	/// WebApiコネクタクラス
	/// </summary>
	public class HttpApiConnector : IDisposable
	{
		#region 定数
		/// <summary>タイムアウト(ミリ秒)</summary>
		private const int DEFAULT_TIME_OUT = 30000;
		#endregion

		#region メンバ変数
		/// <summary>リクエスト実行前処理</summary>
		private Action<IHttpApiRequestData> m_onBeforeRequest;
		/// <summary>リクエスト実行後処理</summary>
		private Action<IHttpApiRequestData, string> m_onAfterRequest;
		/// <summary>リクエスト実行時エラー処理</summary>
		private Action<IHttpApiRequestData, string, Exception> m_onRequestError;
		/// <summary>Webリクエスト拡張処理</summary>
		private Action<HttpWebRequest> m_onExtendWebRequest;
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public HttpApiConnector()
		{
			m_onBeforeRequest = null;
			m_onAfterRequest = null;
			m_onRequestError = null;
		}
		#endregion

		#region +Do 実行
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="url">接続先のURL</param>
		/// <param name="requestEncoding">リクエストのエンコード</param>
		/// <param name="requestData">リクエスト値</param>
		/// <param name="userId">基本認証に利用するユーザーID(ユーザーID・パスワードともに空の場合は基本認証は利用しない)</param>
		/// <param name="password">基本認証に利用するパスワード(ユーザーID・パスワードともに空の場合は基本認証は利用しない)</param>
		/// <param name="isSendXml">コンテンツタイプをXMLで送信するか</param>
		/// <returns>レスポンス値</returns>
		/// <exception cref="HttpApiConnectException">Api接続時にエラーが発生した場合</exception>
		public virtual string Do(
			string url,
			Encoding requestEncoding,
			IHttpApiRequestData requestData,
			string userId,
			string password,
			bool isSendXml = false)
		{
			byte[] postData = CreatePostData(requestData, requestEncoding);

			HttpWebRequest request = this.CreateWebRequest(postData, url, userId, password, isSendXml);

			// リクエスト前処理
			if (m_onBeforeRequest != null)
			{
				m_onBeforeRequest(requestData);
			}

			// リクエストへポストデータの書き込み
			using (Stream stream = request.GetRequestStream())
			{
				stream.Write(postData, 0, postData.Length);
			}

			string responseString;
			try
			{
				// リクエストの結果を取得する
				responseString = GetResponse(request, requestEncoding);

			}
			catch (Exception ex)
			{
				// リクエストエラー時処理
				if (m_onRequestError != null)
				{
					m_onRequestError(requestData, url, ex);
				}

				throw new HttpApiConnectException(ex);
			}

			// リクエスト後処理
			if (m_onAfterRequest != null)
			{
				m_onAfterRequest(requestData, responseString);
			}

			return responseString;
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="url">接続先のURL</param>
		/// <param name="requestEncoding">リクエストのエンコード</param>
		/// <param name="requestData">リクエスト値</param>
		/// <param name="isSendXml">コンテンツタイプをXMLで送信するか</param>
		/// <returns>レスポンス値</returns>
		/// <remarks>Api接続時に基本認証を利用しない</remarks>
		public string Do(string url, Encoding requestEncoding, IHttpApiRequestData requestData, bool isSendXml = false)
		{
			return Do(url, requestEncoding, requestData, string.Empty, string.Empty, isSendXml);
		}
		#endregion

		#region -CreateWebRequest WebRequest生成
		/// <summary>
		/// WebRequest生成
		/// </summary>
		/// <param name="postData">ポストする値</param>
		/// <param name="url">接続先のURL</param>
		/// <param name="userId">基本認証のユーザーID(ユーザーID・パスワードともに空の場合は基本認証は利用しない)</param>
		/// <param name="password">基本認証のパスワード(ユーザーID・パスワードともに空の場合は基本認証は利用しない)</param>
		/// <param name="isSendXml">コンテンツタイプをXMLで送信するか</param>
		/// <returns>生成したWebRequest</returns>
		private HttpWebRequest CreateWebRequest(byte[] postData, string url, string userId, string password, bool isSendXml = false)
		{
			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
			webRequest.Method = "POST";
			webRequest.Timeout = DEFAULT_TIME_OUT;
			webRequest.ContentType = isSendXml ? "application/xml" : "application/x-www-form-urlencoded";
			webRequest.ContentLength = postData.Length; // POST送信するデータの長さを指定
			webRequest.PreAuthenticate = string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(password);
			webRequest.Proxy = null; // プロクシは通さない

			if (m_onExtendWebRequest != null)
			{
				m_onExtendWebRequest(webRequest);
			}

			if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(password))
			{
				return webRequest;
			}

			// 基本認証セット
			webRequest.Credentials = new NetworkCredential(userId, password);

			return webRequest;
		}
		#endregion

		#region #CreatePostData ポスト値生成
		/// <summary>
		/// ポスト値生成
		/// </summary>
		/// <param name="requestData">生成元となるリクエストデータ</param>
		/// <param name="encoding">エンコード</param>
		/// <returns>生成したポスト値</returns>
		protected byte[] CreatePostData(IHttpApiRequestData requestData, Encoding encoding)
		{
			string postString = requestData.CreatePostString();
			return encoding.GetBytes(postString);
		}
		#endregion

		#region #GetResponse レスポンス取得
		/// <summary>
		/// レスポンス取得
		/// </summary>
		/// <param name="webRequest">取得元のWebRequest</param>
		/// <param name="encoding">エンコード</param>
		/// <returns>レスポンス値</returns>
		protected virtual string GetResponse(HttpWebRequest webRequest, Encoding encoding)
		{
			using (WebResponse response = webRequest.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream, encoding))
			{
				return reader.ReadToEnd();
			}
		}
		#endregion

		#region +Dispose
		/// <summary>
		/// Deisposeの実装
		/// </summary>
		public void Dispose()
		{
			m_onBeforeRequest = null;
			m_onAfterRequest = null;
			m_onRequestError = null;
			m_onExtendWebRequest = null;
		}
		#endregion

		#region 各種処理設定
		/// <summary>
		/// リクエスト前処理を設定
		/// </summary>
		/// <param name="action">
		/// リクエスト前処理
		/// 第一引数：リクエストするリクエスト値が入っているのでログとかに利用
		/// </param>
		public void SetBeforeRequestProc(Action<IHttpApiRequestData> action)
		{
			m_onBeforeRequest = action;
		}

		/// <summary>
		/// リクエスト後処理を設定
		/// </summary>
		/// <param name="action">
		/// リクエスト後処理
		/// 第一引数：リクエストするリクエスト値が入っているのでログとかに利用
		/// 第二引数：リクエスト結果が入っているのでログとかに利用
		/// </param>
		public void SetAfterRequestProc(Action<IHttpApiRequestData, string> action)
		{
			m_onAfterRequest = action;
		}

		/// <summary>
		/// リクエストエラー時処理を設定
		/// </summary>
		/// <param name="action">
		/// リクエストエラー時処理
		/// 第一引数：リクエストするリクエスト値が入っているのでログとかに利用
		/// 第二引数：リクエストしたURL
		/// 第三引数：発生したExceptionが入っているのでログとかに利用
		/// </param>
		public void SetRequestErrorProc(Action<IHttpApiRequestData, string, Exception> action)
		{
			m_onRequestError = action;
		}

		/// <summary>
		/// Webリクエストの拡張処理を設定
		/// </summary>
		/// <param name="action">
		/// WebRequest拡張処理
		/// 第一引数：WebRequestがわたってくるので好きにプロパティやHttpヘッダ、タイムアウト値等を設定してください
		/// </param>
		public void SetExtendWebRequestProc(Action<HttpWebRequest> action)
		{
			m_onExtendWebRequest = action;
		}
		#endregion

	}
}
