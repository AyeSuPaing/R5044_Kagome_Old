/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API メッセージモデル (RakutenApiMessage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API メッセージモデル
	/// </summary>
	[JsonObject(Constants.RAKUTEN_PAY_API_RESPONSE_MESSAGE_MODEL_LIST)]
	public class RakutenApiMessage
	{
		/// <summary>メッセージ種別</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_MESSAGE_MODEL_MESSAGE_TYPE)]
		public string MessageType { get; set; }
		/// <summary>メッセージコード</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_MESSAGE_MODEL_MESSAGE_CODE)]
		public string MessageCode { get; set; }
		/// <summary>メッセージ</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_MESSAGE_MODEL_MESSAGE)]
		public string Message { get; set; }
		/// <summary>注文番号</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_MESSAGE_MODEL_ORDER_NUMBER)]
		public string OrderNumber { get; set; }
	}
}
