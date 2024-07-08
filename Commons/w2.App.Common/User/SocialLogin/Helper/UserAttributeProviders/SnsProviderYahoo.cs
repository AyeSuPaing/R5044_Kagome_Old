/*
=========================================================================================================
  Module      : ソーシャルログイン Yahoo認証情報(SnsProviderYahoo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using Newtonsoft.Json;

namespace w2.App.Common.User.SocialLogin.Helper
{
	/// <summary>
	/// Yahoo認証情報
	/// </summary>
	[JsonObject]
	public class SnsProviderYahoo : ISnsProvider
	{
		/// <summary>ソーシャルプロバイダ区分</summary>
		public SocialLoginApiProviderType ProviderType { get { return SocialLoginApiProviderType.Yahoo; } }

		/// <summary>ソーシャルプロバイダID</summary>
		public string ProviderId { get { return this.YahooId; } }

		/// <summary>ソーシャルプロバイダID登録用カラム名</summary>
		public string ColumnName { get { return Constants.SOCIAL_PROVIDER_ID_YAHOO; } }

		/// <summary>Yahoo ID</summary>
		[JsonProperty("yahoo_id")]
		private string YahooId { get; set; }
	}
}