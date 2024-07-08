/*
=========================================================================================================
  Module      : ネクストエンジン受注伝票更新API レスポンス (NextEngineUpdateOrderApiResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;

namespace w2.App.Common.NextEngine.Response
{
	public class NextEngineUpdateOrderApiResponse: NextEngineApiResponseBase
	{
		/// <summary>アクセス有効期限切れ日時</summary>
		[JsonProperty(NextEngineConstants.RESPONSE_PARAM_ACCESS_TOKEN_END_DATE)]
		public string AccessTokenEndDate { get; set; }
		/// <summary>リフレッシュ有効期限切れ日時</summary>
		[JsonProperty(NextEngineConstants.RESPONSE_PARAM_REFRESH_TOKEN_END_DATE)]
		public string RefreshTokenEndDate { get; set; }
	}
}
