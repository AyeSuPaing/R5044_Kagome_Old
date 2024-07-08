/*
=========================================================================================================
  Module      : ソーシャルログインWebAPI 認証対象のソーシャルプラスID取得(SocialLoginAuthenticatedUser.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;

namespace w2.App.Common.User.SocialLogin
{
	/// <summary>
	/// 認証対象のソーシャルプラスID取得
	/// </summary>
	public class SocialLoginAuthenticatedUser : SocialLoginBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SocialLoginAuthenticatedUser()
			: base(SocialLoginApiFunctionType.AuthenticatedUser)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="token">ソーシャルプラスで発行されたワンタイムトークン</param>
		/// <param name="preserveToken">true：ワンタイムトークンを削除しない</param>
		/// <param name="addProfile">true：個人情報(profile)をレスポンスに含めます</param>
		/// <param name="deleteProfile">true：WebAPI コール後にソーシャルプラス上の個人情報を削除</param>
		/// <returns>レスポンス</returns>
		public string Exec(
			string apiKey,
			string token,
			bool preserveToken,
			bool addProfile,
			bool deleteProfile)
		{
			var param = CreateParam(
				apiKey,
				token,
				preserveToken,
				addProfile,
				deleteProfile);

			return GetHttpRequest(param);
		}

		/// <summary>
		/// パラメータ生成
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="token">ソーシャルプラスで発行されたワンタイムトークン</param>
		/// <param name="preserveToken">true：ワンタイムトークンを削除しない</param>
		/// <param name="addProfile">true：個人情報(profile)をレスポンスに含めます</param>
		/// <param name="deleteProfile">true：WebAPI コール後にソーシャルプラス上の個人情報を削除</param>
		/// <returns>パラメータ</returns>
		public string[][] CreateParam(
			string apiKey,
			string token,
			bool preserveToken,
			bool addProfile,
			bool deleteProfile)
		{
			var param = new[]
			{
				new[] {"key", apiKey},
				new[] {"token", token},
				new[] {"preserve_token", preserveToken.ToString().ToLower()},
				new[] {"add_profile", addProfile.ToString().ToLower()},
				new[] {"delete_profile", deleteProfile.ToString().ToLower()},
			};
			return param;
		}

		/// <summary>
		/// URL取得
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="token">ソーシャルプラスで発行されたワンタイムトークン</param>
		/// <param name="preserveToken">true：ワンタイムトークンを削除しない</param>
		/// <param name="addProfile">true：個人情報(profile)をレスポンスに含めます</param>
		/// <param name="deleteProfile">true：WebAPI コール後にソーシャルプラス上の個人情報を削除</param>
		/// <returns>URL</returns>
		public string GetUrl(
			string apiKey,
			string token,
			bool preserveToken,
			bool addProfile,
			bool deleteProfile)
		{
			return base.Url + "?" + SocialLoginUtil.GetQueryParam(CreateParam(apiKey, token, preserveToken, addProfile, deleteProfile));
		}
	}
}