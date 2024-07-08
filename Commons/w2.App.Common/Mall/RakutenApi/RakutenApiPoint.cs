/*
=========================================================================================================
  Module      : 楽天ペイ受注情報取得API ポイントモデル (RakutenApiSettlement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Mall.RakutenApi
{
	/// <summary>
	/// 楽天ペイ受注API ポイントモデル
	/// </summary>
	[JsonObject(Constants.RAKUTEN_PAY_API_ORDER_MODEL_POINT_MODEL)]
	public class RakutenApiPoint
	{
		/// <summary>ポイント利用額</summary>
		[JsonProperty(Constants.RAKUTEN_PAY_API_POINT_MODEL_USED_POINT)]
		public int? UsedPoint { get; set; }
	}
}