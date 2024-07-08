/*
=========================================================================================================
  Module      : FLAPS API レスポンスクラス(FlapsApiResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Net;

namespace w2.App.Common.Flaps
{
	/// <summary>
	/// 共通レスポンスクラス
	/// </summary>
	public class FlapsApiResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="statusCode">ステータスコード</param>
		/// <param name="content">レスポンスデータ</param>
		/// <param name="error">エラー内容</param>
		internal FlapsApiResponse(HttpStatusCode statusCode, string content, string error)
		{
			this.StatusCode = statusCode;
			this.ErrorMessage = error ?? string.Empty;
			this.Content = content;
		}

		/// <summary>ステータスコード</summary>
		internal HttpStatusCode StatusCode { get; set; }
		/// <summary>エラーメッセージ</summary>
		internal string ErrorMessage { get; set; }
		/// <summary>レスポンスデータ</summary>
		internal string Content { get; set; }
	}
}
