/*
=========================================================================================================
  Module      : ソーシャルログインWebAPI ソーシャルプラスIDとw2のユーザID紐づけ(SocialLoginMap.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;

namespace w2.App.Common.User.SocialLogin
{
	/// <summary>
	/// ソーシャルプラスIDとw2のユーザID紐づけ
	/// </summary>
	public class SocialLoginMap : SocialLoginBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SocialLoginMap()
			: base(SocialLoginApiFunctionType.Map)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="splusUserID">ソーシャルプラスID</param>
		/// <param name="w2UserID">ユーザID</param>
		/// <param name="overwrite">既に紐づけ済みの場合はエラーになりますが、trueの場合は上書きします。</param>
		/// <returns>レスポンス</returns>
		public string Exec(
			string apiKey,
			string splusUserID,
			string w2UserID,
			bool overwrite)
		{
			var param = CreateParam(
				apiKey,
				splusUserID,
				w2UserID,
				overwrite);

			return GetHttpRequest(param);
		}

		/// <summary>
		/// パラメータ生成
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="splusUserID">ソーシャルプラスID</param>
		/// <param name="w2UserID">ユーザID</param>
		/// <param name="overwrite">既に紐づけ済みの場合はエラーになりますが、trueの場合は上書きします。</param>
		/// <returns>パラメータ</returns>
		public string[][] CreateParam(
			string apiKey,
			string splusUserID,
			string w2UserID,
			bool overwrite)
		{
			var param = new[]
			{
				new[] {"key", apiKey},
				new[] {"identifier", splusUserID},
				new[] {"primary_key", w2UserID},
				new[] {"overwrite", overwrite.ToString().ToLower()},
			};
			return param;
		}

		/// <summary>
		/// URL取得
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="splusUserID">ソーシャルプラスID</param>
		/// <param name="w2UserID">ユーザID</param>
		/// <param name="overwrite">既に紐づけ済みの場合はエラーになりますが、trueの場合は上書きします。</param>
		/// <returns>URL</returns>
		public string GetUrl(
			string apiKey,
			string splusUserID,
			string w2UserID,
			bool overwrite)
		{
			return base.Url + "?" + SocialLoginUtil.GetQueryParam(CreateParam(apiKey, splusUserID, w2UserID, overwrite));
		}
	}
}
