/*
=========================================================================================================
  Module      : Letro Base Response (LetroBaseResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace Letro
{
	/// <summary>
	/// Letroベースレスポンス
	/// </summary>
	[Serializable]
	public class LetroBaseResponse
	{
		/// <summary>実行結果</summary>
		[JsonProperty("result")]
		public bool Result { get; set; }
		/// <summary>メッセージID</summary>
		[JsonProperty("message_id")]
		public string MessageId { get; set; }
		/// <summary>メッセージ</summary>
		[JsonProperty("message")]
		public string Message { get; set; }
		/// <summary>レスポンスデータ</summary>
		[JsonProperty("data")]
		public object Data { get; set; }
	}
}
