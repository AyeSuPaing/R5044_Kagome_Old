/*
=========================================================================================================
  Module      : ネクストエンジンAPI レスポンス基底クラス (NextEngineApiResponseBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using w2.Common.Sql;
using w2.Domain.TempDatas;

namespace w2.App.Common.NextEngine.Response
{
	/// <summary>
	/// ネクストエンジン連携API AccessToken取得APIレスポンスデータ
	/// </summary>
	public class NextEngineApiResponseBase
	{
		/// <summary>
		/// DBアクセストークン更新
		/// </summary>
		/// <param name="oldAccessToken">古いアクセストークン</param>
		/// <returns>インスタンス</returns>
		public void UpdateAccessToken(string oldAccessToken)
		{
			// アクセストークンが異なる場合はDBの値を更新
			// 成功レスポンスも・失敗レスポンも アクセストークン/リフレッシュトークンを更新
			if (oldAccessToken != this.AccessToken)
			{
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();
					new TempDatasService()
						.Save(
							TempDatasService.TempType.NextEngineToken,
							NextEngineConstants.PARAM_ACCESS_TOKEN,
							this.AccessToken,
							accessor);
					new TempDatasService()
						.Save(
							TempDatasService.TempType.NextEngineToken,
							NextEngineConstants.PARAM_REFRESH_TOKEN,
							this.RefreshToken,
							accessor);
					accessor.CommitTransaction();
				}
			}
		}

		/// <summary>結果</summary>
		[JsonProperty(NextEngineConstants.RESPONSE_PARAM_RESULT)]
		public string Result { get; set; }
		/// <summary>アクセストークン</summary>
		[JsonProperty(NextEngineConstants.PARAM_ACCESS_TOKEN)]
		public string AccessToken { get; set; }
		/// <summary>リフレッシュトークン</summary>
		[JsonProperty(NextEngineConstants.PARAM_REFRESH_TOKEN)]
		public string RefreshToken { get; set; }
		/// <summary>メッセージコード（成功時以外）</summary>
		[JsonProperty(NextEngineConstants.RESPONSE_PARAM_CODE)]
		public string Code { get; set; }
		/// <summary>メッセージ（成功時以外）</summary>
		[JsonProperty(NextEngineConstants.RESPONSE_PARAM_MESSAGE)]
		public string Message { get; set; }
	}
}
