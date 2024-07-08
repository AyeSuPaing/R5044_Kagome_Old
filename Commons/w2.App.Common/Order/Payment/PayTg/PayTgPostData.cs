/*
=========================================================================================================
  Module      : PayTgのAPI送信データ(PayTgPostData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.PayTg
{
	/// <summary>
	/// PayTgのAPI送信データクラス
	/// </summary>
	[JsonObject]
	public class PostDataPayTg
	{
		[JsonProperty(PayTgConstants.PARAM_ORDERID)]
		public string OrderId { get; set; }
		[JsonProperty(PayTgConstants.PARAM_AMOUNT)]
		public string Amount { get; set; }
		[JsonProperty(PayTgConstants.PARAM_JPO)]
		public string Jpo { get; set; }
		[JsonProperty(PayTgConstants.PARAM_COUNT)]
		public string Count { get; set; }
	}
}
