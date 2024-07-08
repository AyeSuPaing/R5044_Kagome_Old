/*
=========================================================================================================
  Module      : LINE API 共通レスポンスクラス (ApiResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Net;

namespace w2.App.Common.Line
{
	/// <summary>
	/// 共通レスポンスクラス
	/// </summary>
	public class ApiResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statusCode">ステータスコード</param>
		/// <param name="content">レスポンスデータ</param>
		/// <param name="reason">ステータス理由</param>
		internal ApiResponse(HttpStatusCode statusCode, string content, string reason)
		{
			this.StatusCode = statusCode;
			this.ReasonPhrase = reason ?? string.Empty;
			this.Content = content;
		}

		/// <summary>ステータスコード</summary>
		internal HttpStatusCode StatusCode { get; set; }
		/// <summary>ステータス理由</summary>
		internal string ReasonPhrase { get; set; }
		/// <summary>レスポンスデータ</summary>
		internal string Content { get; set; }
	}
}