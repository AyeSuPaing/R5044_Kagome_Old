/*
=========================================================================================================
  Module      : 既存エンドユーザに対してプロバイダを追加で紐づけ(SocialLoginAuthenticateAssociate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;

namespace w2.App.Common.User.SocialLogin
{
	/// <summary>
	/// 既存エンドユーザに対してプロバイダを追加で紐づけ
	/// </summary>
	public class SocialLoginAuthenticateAssociate : SocialLoginBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="providertype">プロバイダ区分</param>
		public SocialLoginAuthenticateAssociate(SocialLoginApiProviderType providertype)
			: base(SocialLoginApiFunctionType.AuthenticateAssociate, providertype)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="callBackUrl">認証成功時に遷移させたいURL</param>
		/// <param name="errorCallBackUrl">認証失敗時に遷移させたいURL</param>
		/// <param name="token">WebAPI（association_token）のレスポンスに含まれるトークンを設定</param>
		/// <returns>レスポンス</returns>
		public string Exec(
			string callBackUrl,
			string errorCallBackUrl,
			string token)
		{
			var param = CreateParam(
				callBackUrl,
				errorCallBackUrl,
				token);

			return GetHttpRequest(param);
		}

		/// <summary>
		/// パラメータ生成
		/// </summary>
		/// <param name="callBackUrl">認証成功時に遷移させたいURL</param>
		/// <param name="errorCallBackUrl">認証失敗時に遷移させたいURL</param>
		/// <param name="token">WebAPI（association_token）のレスポンスに含まれるトークンを設定</param>
		/// <returns>パラメータ</returns>
		public string[][] CreateParam(
			string callBackUrl,
			string errorCallBackUrl,
			string token)
		{
			var param = new[]
			{
				new[] {"callback", callBackUrl},
				new[] {"callback_if_failed", errorCallBackUrl},
				new[] {"token", token},
			};
			return param;
		}

		/// <summary>
		/// URL取得
		/// </summary>
		/// <param name="callBackUrl">認証成功時に遷移させたいURL</param>
		/// <param name="errorCallBackUrl">認証失敗時に遷移させたいURL</param>
		/// <param name="token">WebAPI（association_token）のレスポンスに含まれるトークンを設定</param>
		/// <returns>URL</returns>
		public string GetUrl(
			string callBackUrl,
			string errorCallBackUrl,
			string token)
		{
			return base.Url + "?" + SocialLoginUtil.GetQueryParam(CreateParam(callBackUrl, errorCallBackUrl, token));
		}
	}
}
