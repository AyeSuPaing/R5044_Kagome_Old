/*
=========================================================================================================
  Module      : ソーシャルログイン LINE認証情報(SnsProviderLine.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using Newtonsoft.Json;

namespace w2.App.Common.User.SocialLogin.Helper
{
	/// <summary>
	/// LINE認証情報
	/// </summary>
	[JsonObject]
	public class SnsProviderLine : ISnsProvider
	{
		/// <summary>ソーシャルプロバイダ区分</summary>
		public SocialLoginApiProviderType ProviderType { get { return SocialLoginApiProviderType.Line; } }

		/// <summary>ソーシャルプロバイダID</summary>
		public string ProviderId { get { return this.LineId; } }

		/// <summary>ソーシャルプロバイダID登録用カラム名</summary>
		public string ColumnName { get { return Constants.SOCIAL_PROVIDER_ID_LINE; } }

		/// <summary>LINE ID</summary>
		[JsonProperty("line_id")]
		private string LineId { get; set; }
	}
}