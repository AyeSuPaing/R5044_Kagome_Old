/*
=========================================================================================================
  Module      : ログインリクエスト(LoginRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.User.Botchan
{
	/// <summary>
	/// ログインリクエスト
	/// </summary>
	[Serializable]
	public class LoginRequest
	{
		/// <summary>メールアドレス</summary>
		[JsonProperty("mail_address")]
		public string MailAddress { get; set; }
		/// <summary>ユーザーIPアドレス</summary>
		[JsonProperty("user_ip_address")]
		public string UserIpAddress { get; set; }
		/// <summary>認証キー</summary>
		[JsonProperty("auth_text")]
		public string AuthText { get; set; }
	}
}
