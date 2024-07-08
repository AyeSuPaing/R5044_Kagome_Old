/*
=========================================================================================================
  Module      : ソーシャルログインWebAPI ユーザの結合(SocialLoginMergeUser.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.User.SocialLogin.Helper;

namespace w2.App.Common.User.SocialLogin
{
	/// <summary>
	/// ユーザーの結合
	/// </summary>
	public class SocialLoginMergeUser : SocialLoginBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SocialLoginMergeUser()
			: base(SocialLoginApiFunctionType.MergeUser)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="sourceSocialPlusId">結合元のソーシャルPLUS ID</param>
		/// <param name="destSocialPlusId">取得対象ユーザーID</param>
		/// <returns>レスポンス(JSON形式)</returns>
		public string Exec(
			string apiKey,
			string sourceSocialPlusId,
			string destSocialPlusId)
		{
			var parameter = CreateParam(apiKey, sourceSocialPlusId, destSocialPlusId);
			return GetHttpRequest(parameter);
		}

		/// <summary>
		/// パラメータ作成
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="sourceSocialPlusId">結合元のソーシャルPLUS ID</param>
		/// <param name="destSocialPlusId">取得対象ユーザーID</param>
		/// <returns>パラメータ</returns>
		private string[][] CreateParam(
			string apiKey,
			string sourceSocialPlusId,
			string destSocialPlusId)
		{
			var parameter = new[]
			{
				new [] { "key", apiKey },
				new [] { "source_identifier", sourceSocialPlusId },
				new [] { "dest_identifier", destSocialPlusId }
			};
			return parameter;
		}
	}
}