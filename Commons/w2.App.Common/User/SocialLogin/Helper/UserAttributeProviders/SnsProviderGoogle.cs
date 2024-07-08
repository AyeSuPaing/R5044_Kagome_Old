/*
=========================================================================================================
  Module      : ソーシャルログイン Google認証情報(SnsProviderGoogle.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using Newtonsoft.Json;

namespace w2.App.Common.User.SocialLogin.Helper
{
	/// <summary>
	/// Google認証情報
	/// </summary>
	[JsonObject]
	public class SnsProviderGoogle : ISnsProvider
	{
		/// <summary>ソーシャルプロバイダ区分</summary>
		public SocialLoginApiProviderType ProviderType { get { return SocialLoginApiProviderType.Gplus; } }

		/// <summary>ソーシャルプロバイダID</summary>
		public string ProviderId { get { return this.GoogleId; } }

		/// <summary>ソーシャルプロバイダID登録用カラム名</summary>
		public string ColumnName { get { return Constants.SOCIAL_PROVIDER_ID_GOOGLE; } }

		/// <summary>Gplus ID</summary>
		[JsonProperty("gplus_id")]
		private string GoogleId { get; set; }
	}
}