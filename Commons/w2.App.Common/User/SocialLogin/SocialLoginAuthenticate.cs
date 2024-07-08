/*
=========================================================================================================
  Module      : ソーシャルログインWebAPI ログイン・新規登録　併用(SocialLoginAuthenticate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;

namespace w2.App.Common.User.SocialLogin
{
	/// <summary>
	/// ログイン・新規登録　併用
	/// </summary>
	public class SocialLoginAuthenticate : SocialLoginBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="providertype">プロバイダ区分</param>
		public SocialLoginAuthenticate(SocialLoginApiProviderType providertype)
			: base(SocialLoginApiFunctionType.Authenticate, providertype)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="callBackUrl">認証成功時に遷移させたいURL</param>
		/// <param name="errorCallBackUrl">認証失敗時に遷移させたいURL</param>
		/// <param name="profile">true：基本情報以外に、追加情報もレスポンスに含める場合</param>
		/// <returns>レスポンス</returns>
		public string Exec(
			string callBackUrl,
			string errorCallBackUrl,
			bool profile)
		{
			var param = CreateParam(
				callBackUrl,
				errorCallBackUrl,
				profile);

			return GetHttpRequest(param);
		}

		/// <summary>
		/// パラメータ生成
		/// </summary>
		/// <param name="callBackUrl">認証成功時に遷移させたいURL</param>
		/// <param name="errorCallBackUrl">認証失敗時に遷移させたいURL</param>
		/// <param name="profile">true：基本情報以外に、追加情報もレスポンスに含める場合</param>
		/// <returns>パラメータ</returns>
		public string[][] CreateParam(
			string callBackUrl,
			string errorCallBackUrl,
			bool profile)
		{
			var param = new[]
			{
				new[] {"callback", callBackUrl},
				new[] {"callback_if_failed", errorCallBackUrl},
				(this.ProviderType == SocialLoginApiProviderType.Line && profile)
					? new[] {"extended_items", "email,phone,gender,birthdate,address,real_name"}
					: new[] {"extended_profile", profile.ToString().ToLower()},
			};
			return param;
		}

		/// <summary>
		/// URL取得
		/// </summary>
		/// <param name="callBackUrl">認証成功時に遷移させたいURL</param>
		/// <param name="errorCallBackUrl">認証失敗時に遷移させたいURL</param>
		/// <param name="profile">true：基本情報以外に、追加情報もレスポンスに含める場合</param>
		/// <returns>URL</returns>
		public string GetUrl(
			string callBackUrl,
			string errorCallBackUrl,
			bool profile)
		{
			return base.Url + "?" + SocialLoginUtil.GetQueryParam(CreateParam(callBackUrl, errorCallBackUrl, profile));
		}
	}
}
