/*
=========================================================================================================
  Module      : ソーシャルログイン Twitter認証情報(SnsProviderTwitter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using Newtonsoft.Json;

namespace w2.App.Common.User.SocialLogin.Helper
{
	/// <summary>
	/// Twitter認証情報
	/// </summary>
	[JsonObject]
	public class SnsProviderTwitter : ISnsProvider
	{
		/// <summary>ソーシャルプロバイダ区分</summary>
		public SocialLoginApiProviderType ProviderType { get { return SocialLoginApiProviderType.Twitter; } }

		/// <summary>ソーシャルプロバイダID</summary>
		public string ProviderId { get { return this.TwitterId; } }

		/// <summary>ソーシャルプロバイダID登録用カラム名</summary>
		public string ColumnName { get { return Constants.SOCIAL_PROVIDER_ID_TWITTER; } }

		/// <summary>Twitter ID</summary>
		[JsonProperty("twitter_id")]
		private string TwitterId { get; set; }
	}
}