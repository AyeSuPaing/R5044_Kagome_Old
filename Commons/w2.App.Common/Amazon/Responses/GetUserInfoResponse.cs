/*
=========================================================================================================
  Module      : AmazonAPI#GetUserInfoのレスポンス(GetUserInfoResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using System;

namespace w2.App.Common.Amazon.Responses
{
	/// <summary>
	/// AmazonAPI#GetUserInfoのレスポンス
	/// </summary>
	[Serializable]
	[JsonObject]
	public partial class GetUserInfoResponse
	{
		/// <summary>ユーザーID</summary>
		[JsonProperty("user_id")]
		public string UserId { get; set; }

		/// <summary>氏名</summary>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>メールアドレス</summary>
		[JsonProperty("email")]
		public string Email { get; set; }
	}
}