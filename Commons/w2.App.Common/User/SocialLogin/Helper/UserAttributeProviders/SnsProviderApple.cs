/*
=========================================================================================================
  Module      : ソーシャルログイン Apple認証情報(SnsProviderApple.cs)
 ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright  w2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.User.SocialLogin.Helper
{
	/// <summary>
	/// Apple認証情報
	/// </summary>
	public class SnsProviderApple : ISnsProvider
	{
		/// <summary>ソーシャルプロバイダ区分</summary>
		public SocialLoginApiProviderType ProviderType { get { return SocialLoginApiProviderType.Apple; } }

		/// <summary>ソーシャルプロバイダID</summary>
		public string ProviderId { get { return this.AppleId; } }

		/// <summary>ソーシャルプロバイダID登録用カラム名</summary>
		public string ColumnName { get { return Constants.SOCIAL_PROVIDER_ID_APPLE; } }

		/// <summary>Apple ID</summary>
		[JsonProperty("apple_id")]
		private string AppleId { get; set; }
	}
}
