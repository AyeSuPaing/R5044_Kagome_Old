/*
=========================================================================================================
  Module      : PayTgのAPI送信データ(PostDataVeriTrans.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Order.Payment.PayTg
{
	/// <summary>
	/// PayTgのAPI送信データクラス
	/// </summary>
	[JsonObject]
	public class PostDataVeriTrans
	{
		[JsonProperty(PayTgConstants.PARAM_CUSTOMERID)]
		public string CustomerId { get; set; }
	}
}
