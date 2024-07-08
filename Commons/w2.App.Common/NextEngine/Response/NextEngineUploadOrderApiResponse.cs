/*
=========================================================================================================
  Module      : ネクストエンジン受注伝票アップロードAPI レスポンス (NextEngineUploadOrderApiResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.NextEngine.Response
{
	/// <summary>
	/// ネクストエンジン連携API 受注伝票アップロードAPIレスポンスデータ
	/// </summary>
	public class NextEngineUploadOrderApiResponse : NextEngineApiResponseBase
	{
		/// <summary>登録したアップロードキューのID</summary>
		[JsonProperty(NextEngineConstants.RESPONSE_PARAM_QUE_ID)]
		public string QueId { get; set; }
	}
}
