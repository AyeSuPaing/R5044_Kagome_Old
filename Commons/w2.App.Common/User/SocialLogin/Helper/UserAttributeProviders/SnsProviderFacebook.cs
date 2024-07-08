/*
=========================================================================================================
  Module      : ソーシャルログイン Facebook認証情報(SnsProviderFacebook.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using Newtonsoft.Json;

namespace w2.App.Common.User.SocialLogin.Helper
{
	/// <summary>
	/// Facebook認証情報
	/// </summary>
	[JsonObject]
	public class SnsProviderFacebook : ISnsProvider
	{
		/// <summary>ソーシャルプロバイダ区分</summary>
		public SocialLoginApiProviderType ProviderType { get { return SocialLoginApiProviderType.Facebook; } }

		/// <summary>ソーシャルプロバイダID</summary>
		public string ProviderId { get { return this.FacebookId; } }

		/// <summary>ソーシャルプロバイダID登録用カラム名</summary>
		public string ColumnName { get { return Constants.SOCIAL_PROVIDER_ID_FACEBOOK; } }

		/// <summary>Facebook ID</summary>
		[JsonProperty("facebook_id")]
		private string FacebookId { get; set; }
	}
}