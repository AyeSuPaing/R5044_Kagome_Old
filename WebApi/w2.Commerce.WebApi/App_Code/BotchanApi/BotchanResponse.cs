/*
=========================================================================================================
  Module      : Botchan エラーレスポンス(BotchanResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using Newtonsoft.Json;

namespace BotchanApi
{
	/// <summary>
	/// Botchan エラーレスポンス
	/// </summary>
	public class BotchanResponse
	{
		/// <summary>Result</summary>
		[JsonProperty("result")]
		public bool Result { set; get; }
		/// <summary>Message id</summary>
		[JsonProperty("message_id")]
		public string MessageId { set; get; }
		/// <summary>Message</summary>
		[JsonProperty("message")]
		public string Message { set; get; }
		/// <summary>Data</summary>
		[JsonProperty("data")]
		public List<string> Data { set; get; }
	}
}