/*
=========================================================================================================
  Module      : ソーシャルログインWebAPI プロバイダ追加用トークン発行(SocialLoginAssociationToken.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;

namespace w2.App.Common.User.SocialLogin
{
	/// <summary>
	/// プロバイダ追加用トークン発行
	/// </summary>
	public class SocialLoginAssociationToken : SocialLoginBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SocialLoginAssociationToken()
			: base(SocialLoginApiFunctionType.AssociationToken)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="splusUserID">ソーシャルプラスID</param>
		/// <param name="w2UserID">ユーザID</param>
		/// <param name="providerType">プロバイダ区分</param>
		/// <returns>レスポンス</returns>
		public string Exec(
			string apiKey,
			string splusUserID,
			string w2UserID,
			SocialLoginApiProviderType providerType)
		{
			var param = CreateParam(
				apiKey,
				splusUserID,
				w2UserID,
				providerType);

			return GetHttpRequest(param);
		}

		/// <summary>
		/// パラメータ生成
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="splusUserID">ソーシャルプラスID</param>
		/// <param name="w2UserID">ユーザID</param>
		/// <param name="providerType">プロバイダ区分</param>
		/// <returns>パラメータ</returns>
		public string[][] CreateParam(
			string apiKey,
			string splusUserID,
			string w2UserID,
			SocialLoginApiProviderType providerType)
		{
			var pk = "identifier";
			var pkValue = splusUserID;
			if (string.IsNullOrEmpty(splusUserID))
			{
				pk = "primary_key";
				pkValue = w2UserID;
			}

			var param = new[]
			{
				new[] {"key", apiKey},
				new[] {pk, pkValue},
				new[] {"target_provider", providerType.ToValue()},
			};
			return param;
		}

		/// <summary>
		/// URL取得
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="splusUserID">ソーシャルプラスID</param>
		/// <param name="w2UserID">ユーザID</param>
		/// <param name="providerType">プロバイダ区分</param>
		/// <returns>URL</returns>
		public string GetUrl(
			string apiKey,
			string splusUserID,
			string w2UserID,
			SocialLoginApiProviderType providerType)
		{
			return base.Url + "?" + SocialLoginUtil.GetQueryParam(CreateParam(apiKey, splusUserID, w2UserID, providerType));
		}
	}
}
