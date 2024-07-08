/*
=========================================================================================================
  Module      : ソーシャルログインWebAPI プロバイダとの紐付け解除(SocialLoginDissociate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.User.SocialLogin.Helper;
using w2.App.Common.User.SocialLogin.Util;

namespace w2.App.Common.User.SocialLogin
{
	/// <summary>
	/// プロバイダとの紐付け解除
	/// </summary>
	public class SocialLoginDissociate : SocialLoginBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SocialLoginDissociate()
			: base(SocialLoginApiFunctionType.Dissociate)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="splusUserID">ソーシャルプラスID</param>
		/// <param name="w2UserID">ユーザID</param>
		/// <param name="providerType">プロバイダ区分</param>
		/// <param name="nowarn">true：処理後に個人情報を更新しません。</param>
		/// <param name="nomerge">true：紐付が最後の１つでも強制的に削除して、エラーを発生させない。</param>
		/// <returns>レスポンス</returns>
		public string Exec(
			string apiKey,
			string splusUserID,
			string w2UserID,
			SocialLoginApiProviderType providerType,
			bool nowarn,
			bool nomerge)
		{
			var param = CreateParam(
				apiKey,
				splusUserID,
				w2UserID,
				providerType,
				nowarn,
				nomerge);

			return GetHttpRequest(param);
		}

		/// <summary>
		/// パラメータ生成
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="splusUserID">ソーシャルプラスID</param>
		/// <param name="w2UserID">ユーザID</param>
		/// <param name="providerType">プロバイダ区分</param>
		/// <param name="nowarn">true：処理後に個人情報を更新しません。</param>
		/// <param name="nomerge">true：紐付が最後の１つでも強制的に削除して、エラーを発生させない。</param>
		/// <returns>パラメータ</returns>
		public string[][] CreateParam(
			string apiKey,
			string splusUserID,
			string w2UserID,
			SocialLoginApiProviderType providerType,
			bool nowarn,
			bool nomerge)
		{
			var param = new[]
			{
				new[] {"key", apiKey},
				new[] {"identifier", splusUserID},
				new[] {"primary_key", w2UserID},
				new[] {"target_provider", providerType.ToValue()},
				new[] {"nowarn", nowarn.ToString().ToLower()},
				new[] {"nomerge", nomerge.ToString().ToLower()},
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
		/// <param name="nowarn">true：処理後に個人情報を更新しません。</param>
		/// <param name="nomerge">true：紐付が最後の１つでも強制的に削除して、エラーを発生させない。</param>
		/// <returns>URL</returns>
		public string GetUrl(
			string apiKey,
			string splusUserID,
			string w2UserID,
			SocialLoginApiProviderType providerType,
			bool nowarn,
			bool nomerge)
		{
			return base.Url + "?" + SocialLoginUtil.GetQueryParam(CreateParam(apiKey, splusUserID, w2UserID, providerType, nowarn, nomerge));
		}
	}
}
