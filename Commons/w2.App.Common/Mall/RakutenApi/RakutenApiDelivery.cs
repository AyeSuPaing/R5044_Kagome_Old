/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API 配送方法モデル (RakutenApiDelivery.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API 配送方法モデル
	/// </summary>
	[JsonObject(Constants.RAKUTEN_PAY_API_ORDER_MODEL_DELIVERY_MODEL)]
	public class RakutenApiDelivery
	{
		/// <summary>配送方法</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_DELIVERY_MODEL_DELIVERY_NAME)]
		public string DeliveryName { get; set; }
		/// <summary>配送区分</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_DELIVERY_MODEL_DELIVERY_CLASS)]
		public int? DeliveryClass { get; set; }
	}
}
