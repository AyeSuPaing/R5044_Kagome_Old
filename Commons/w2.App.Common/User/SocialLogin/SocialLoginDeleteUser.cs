/*
=========================================================================================================
  Module      : ソーシャルログインWebAPI ユーザ削除(SocialLoginDeleteUser.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;

namespace w2.App.Common.User.SocialLogin
{
	/// <summary>
	/// ユーザ削除
	/// </summary>
	public class SocialLoginDeleteUser : SocialLoginBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SocialLoginDeleteUser()
			: base(SocialLoginApiFunctionType.DeleteUser)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="splusUserID">ソーシャルプラスID</param>
		/// <param name="w2UserID">ユーザID</param>
		/// <returns>レスポンス</returns>
		public string Exec(
			string apiKey,
			string splusUserID,
			string w2UserID)
		{
			var param = CreateParam(
				apiKey,
				splusUserID,
				w2UserID);

			return GetHttpRequest(param);
		}

		/// <summary>
		/// パラメータ生成
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="splusUserID">ソーシャルプラスID</param>
		/// <param name="w2UserID">ユーザID</param>
		/// <returns>パラメータ</returns>
		public string[][] CreateParam(
			string apiKey,
			string splusUserID,
			string w2UserID)
		{
			var param = new[]
			{
				new[] {"key", apiKey},
				new[] {"identifier", splusUserID},
				new[] {"primary_key", w2UserID},
			};
			return param;
		}

		/// <summary>
		/// URL取得
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="splusUserID">ソーシャルプラスID</param>
		/// <param name="w2UserID">ユーザID</param>
		/// <returns>URL</returns>
		public string GetUrl(
			string apiKey,
			string splusUserID,
			string w2UserID)
		{
			return base.Url + "?" + SocialLoginUtil.GetQueryParam(CreateParam(apiKey, splusUserID, w2UserID));
		}
	}
}
