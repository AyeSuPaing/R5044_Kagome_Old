/*
=========================================================================================================
  Module      : LINE API 受注情報送信ボディモデル (OrderParam.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Line.SendLineMessage
{
	/// <summary>
	/// LINE API 受注情報送信ボディモデル
	/// </summary>
	public class OrderParam
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderParam()
		{
			this.OrderNumber = null;
			this.OrderedDate = null;
			this.ShippingDate = null;
			this.TotalAmount = null;
			this.Uri = null;
			this.Item = new[] { new OrderItemParam() };
		}

		/// <summary>注文番号</summary>
		[JsonProperty("orderNumber")]
		public string OrderNumber { get; set; }
		/// <summary>注文日</summary>
		[JsonProperty("orderedDate")]
		public string OrderedDate { get; set; }
		/// <summary>出荷日</summary>
		[JsonProperty("shippingDate")]
		public string ShippingDate { get; set; }
		/// <summary>合計金額</summary>
		[JsonProperty("totalAmount")]
		public string TotalAmount { get; set; }
		/// <summary>注文を確認するボタンをタップした際の遷移先URL</summary>
		[JsonProperty("uri")]
		public string Uri { get; set; }
		/// <summary>商品リスト</summary>
		[JsonProperty("orderDetails")]
		public OrderItemParam[] Item { get; set; }
	}
}
