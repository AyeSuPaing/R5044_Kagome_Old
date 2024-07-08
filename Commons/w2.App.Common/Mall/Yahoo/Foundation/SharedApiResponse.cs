/*
=========================================================================================================
  Module      : YAHOO API 共通レスポンス クラス(SharedApiResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Net;

namespace w2.App.Common.Mall.Yahoo.Foundation
{
	/// <summary>
	/// YAHOO API 共通レスポンス クラス
	/// </summary>
	public class SharedApiResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SharedApiResponse()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statusCode">HTTPレスポンスコード</param>
		/// <param name="content">コンテント</param>
		/// <param name="reasonPhrase">理由語句</param>
		/// <param name="xSwsAuthorizeStatusHeader">リクエストヘッダー "X-SWS-Authorize-Status" (公開鍵認証の結果)</param>
		public SharedApiResponse(
			HttpStatusCode statusCode,
			string content,
			string reasonPhrase,
			string xSwsAuthorizeStatusHeader = "")
		{
			this.StatusCode = statusCode;
			this.Content = content;
			this.ReasonPhrase = reasonPhrase;
			this.XSwsAuthorizeStatusHeader = xSwsAuthorizeStatusHeader;
		}

		/// <summary>HTTPレスポンスコード</summary>
		public HttpStatusCode StatusCode { get; }
		/// <summary>コンテンツ</summary>
		public string Content { get; }
		/// <summary>理由語句</summary>
		public string ReasonPhrase { get; }
		/// <summary>リクエストヘッダー "X-SWS-Authorize-Status" (公開鍵認証の結果)</summary>
		public string XSwsAuthorizeStatusHeader { get; }
	}
}
