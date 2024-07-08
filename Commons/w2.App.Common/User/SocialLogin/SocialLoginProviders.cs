/*
=========================================================================================================
  Module      : ソーシャルログインWebAPI ログイン可能なプロバイダ一覧(SocialLoginProviders.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;

namespace w2.App.Common.User.SocialLogin
{
	/// <summary>
	/// ログイン可能なプロバイダ一覧
	/// </summary>
	public class SocialLoginProviders : SocialLoginBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SocialLoginProviders()
			: base(SocialLoginApiFunctionType.Providers)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <returns>レスポンス</returns>
		public string Exec(string apiKey)
		{
			var param = CreateParam(apiKey);

			return GetHttpRequest(param);
		}

		/// <summary>
		/// パラメータ生成
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <returns>パラメータ</returns>
		public string[][] CreateParam(string apiKey)
		{
			var param = new[]
			{
				new[] {"key", apiKey},
			};
			return param;
		}

		/// <summary>
		/// URL取得
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <returns>URL</returns>
		public string GetUrl(string apiKey)
		{
			return base.Url + "?" + SocialLoginUtil.GetQueryParam(CreateParam(apiKey));
		}
	}
}
