/*
=========================================================================================================
  Module      : ソーシャルログイン ソーシャルプロバイダ認証情報(SocialLoginUserAttributeModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace w2.App.Common.User.SocialLogin.Helper
{
	/// <summary>
	/// ユーザーのソーシャルプロバイダ認証情報
	/// </summary>
	[JsonObject]
	public class SocialLoginUserAttributeModel
	{
		#region 定数
		/// <summary>Facebook</summary>
		private const string JSON_KEY_FACEBOOK = "facebook";
		/// <summary>LINE</summary>
		private const string JSON_KEY_LINE = "line";
		/// <summary>Twitter</summary>
		private const string JSON_KEY_TWITTER = "twitter";
		/// <summary>Yahoo</summary>
		private const string JSON_KEY_YAHOO = "yahoo";
		/// <summary>Google</summary>
		private const string JSON_KEY_GOOGLE = "gplus";
		/// <summary>Apple</summary>
		private const string JSON_KEY_APPLE = "apple";
		#endregion

		/// <summary>
		/// ソーシャルプロバイダ認証情報取得
		/// </summary>
		/// <returns>ソーシャルプロバイダ認証情報</returns>
		public ISnsProvider[] GetSocialProviderAccounts()
		{
			var providerInfos = this.Providers
				.Where(provider => provider.Value != null)
				.Select(provider => GetProviderInfo(provider.Key, provider.Value.ToString())).ToArray();
			return providerInfos;
		}

		/// <summary>
		/// ソーシャルプロバイダ認証情報取得
		/// </summary>
		/// <param name="provideName">ソーシャルプロバイダ名</param>
		/// <param name="value">ソーシャルプロバイダ認証情報(JSON)</param>
		/// <returns>ソーシャルプロバイダ認証情報</returns>
		private ISnsProvider GetProviderInfo(string provideName, string value)
		{
			switch (provideName)
			{
				case JSON_KEY_FACEBOOK:
					return JsonConvert.DeserializeObject<SnsProviderFacebook>(value);

				case JSON_KEY_LINE:
					return JsonConvert.DeserializeObject<SnsProviderLine>(value);

				case JSON_KEY_TWITTER:
					return JsonConvert.DeserializeObject<SnsProviderTwitter>(value);

				case JSON_KEY_YAHOO:
					return JsonConvert.DeserializeObject<SnsProviderYahoo>(value);

				case JSON_KEY_GOOGLE:
					return JsonConvert.DeserializeObject<SnsProviderGoogle>(value);

				case JSON_KEY_APPLE:
					return JsonConvert.DeserializeObject<SnsProviderApple>(value);

				default:
					return null;
			}
		}

		/// <summary>リクエストステータス</summary>
		[JsonProperty("status")]
		public string Status { get; set; }

		/// <summary>ソーシャルプロバイダ別認証情報</summary>
		[JsonProperty("providers")]
		private Dictionary<string, Object> Providers { get; set; }
	}
}
