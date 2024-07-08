/*
=========================================================================================================
  Module      : LINE API 受注商品情報モデル (OrderItemParam.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Line.SendLineMessage
{
	[JsonObject("orderDetails")]
	public class OrderItemParam
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderItemParam()
		{
			this.ItemName = null;
			this.Quantity = null;
			this.UnitPrice = null;
		}
		/// <summary>商品名</summary>
		[JsonProperty("itemName")]
		public string ItemName { get; set; }
		/// <summary>数量</summary>
		[JsonProperty("quantity")]
		public string Quantity { get; set; }
		/// <summary>単価</summary>
		[JsonProperty("unitPrice")]
		public string UnitPrice { get; set; }
	}

}
