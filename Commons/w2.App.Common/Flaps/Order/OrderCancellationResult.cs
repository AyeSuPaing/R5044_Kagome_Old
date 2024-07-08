/*
=========================================================================================================
  Module      : FLAPS受注キャンセル結果モデル (OrderCancellationResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;

namespace w2.App.Common.Flaps.Order
{
	/// <summary>
	/// FLAPS受注キャンセル結果
	/// </summary>
	[JsonObject(Constants.FLAPS_API_RESULT_ORDER_CANCELLATION)]
	public class OrderCancellationResult : ResultBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderCancellationResult()
		{
			this.Message = string.Empty;
			this.PosNoSerNo = string.Empty;
		}

		/// <summary>メッセージ</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_ORDER_CANCELLATION_MESSAGE)]
		public string Message { get; set; }
		/// <summary>ショップカウンター業績唯一番号</summary>
		[JsonProperty(Constants.FLAPS_API_RESULT_ORDER_CANCELLATION_POST_NO_SER_NO)]
		public string PosNoSerNo { get; set; }
	}
}