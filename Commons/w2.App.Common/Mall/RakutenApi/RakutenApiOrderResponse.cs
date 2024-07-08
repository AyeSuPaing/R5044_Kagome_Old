/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API レスポンス (RakutenApiOrderResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API getOrderAPIからのレスポンスデータ
	/// </summary>
	[JsonObject(Constants.RAKUTEN_PAY_API_RESPONSE_BASE)]
	public class RakutenApiOrderResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="response">初期値(JSON形式)</param>
		public RakutenApiOrderResponse(string response = null)
		{
			var parameter = JsonConvert.DeserializeObject<RakutenApiOrderResponse>(response ?? string.Empty);
			this.MessageModelList = (parameter == null) ? new RakutenApiMessage[0] : parameter.MessageModelList;
			this.OrderModelList = (parameter == null) ? new RakutenApiOrder[0] : parameter.OrderModelList;
		}

		/// <summary>メッセージモデルリスト</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_RESPONSE_MESSAGE_MODEL_LIST)]
		public RakutenApiMessage[] MessageModelList { get; set; }
		/// <summary>受注モデルリスト</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_RESPONSE_ORDER_MODEL_LIST)]
		public RakutenApiOrder[] OrderModelList { get; set; }
	}
}
