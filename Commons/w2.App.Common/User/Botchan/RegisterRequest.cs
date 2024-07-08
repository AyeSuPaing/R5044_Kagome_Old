/*
=========================================================================================================
  Module      : 会員登録リクエスト(RegisterRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

namespace w2.App.Common.User.Botchan
{
	/// <summary>
	/// 会員登録リクエスト
	/// </summary>
	[Serializable]
	public class RegisterRequest
	{
		/// <summary>会員登録情報</summary>
		[JsonProperty("user_register")]
		public UserRegister UserRegisterObject { get; set; }
		/// <summary>認証キー</summary>
		[JsonProperty("auth_text")]
		public string AuthText { get; set; }
		/// <summary>ユーザーIPアドレス</summary>
		[JsonProperty("user_ip_address")]
		public string UserIpAddress { get; set; }

		/// <summary>
		/// 会員登録オブジェクト
		/// </summary>
		[Serializable]
		public class UserRegister
		{
			/// <summary>氏名1</summary>
			[JsonProperty("name1")]
			public string Name1 { get; set; }
			/// <summary>氏名2</summary>
			[JsonProperty("name2")]
			public string Name2 { get; set; }
			/// <summary>氏名かな1</summary>
			[JsonProperty("name_kana1")]
			public string NameKana1 { get; set; }
			/// <summary>氏名かな2</summary>
			[JsonProperty("name_kana2")]
			public string NameKana2 { get; set; }
			/// <summary>顧客区分</summary>
			[JsonProperty("user_kbn")]
			public string UserKbn { get; set; }
			/// <summary>生年月日</summary>
			[JsonProperty("birth")]
			public string Birth { get; set; }
			/// <summary>性別</summary>
			[JsonProperty("sex")]
			public string Sex { get; set; }
			/// <summary>メールアドレス</summary>
			[JsonProperty("mail_addr")]
			public string MailAddr { get; set; }
			/// <summary>郵便番号</summary>
			[JsonProperty("zip")]
			public string Zip { get; set; }
			/// <summary>住所1</summary>
			[JsonProperty("addr1")]
			public string Addr1 { get; set; }
			/// <summary>住所2</summary>
			[JsonProperty("addr2")]
			public string Addr2 { get; set; }
			/// <summary>住所3</summary>
			[JsonProperty("addr3")]
			public string Addr3 { get; set; }
			/// <summary>住所4</summary>
			[JsonProperty("addr4")]
			public string Addr4 { get; set; }
			/// <summary>電話番号1</summary>
			[JsonProperty("tel1")]
			public string Tel1 { get; set; }
			/// <summary>パスワード</summary>
			[JsonProperty("password")]
			public string Password { get; set; }
			/// <summary>メール配信フラグ</summary>
			[JsonProperty("mail_flg")]
			public string MailFlg { get; set; }
		}
	}
}
