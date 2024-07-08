/*
=========================================================================================================
  Module      : Line Orders Get Response (LineOrdersGetResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

/// <summary>
/// Line pay orders get response
/// </summary>
[Serializable]
public class LineOrdersGetResponse
{
	/// <summary>Count</summary>
	[JsonProperty(PropertyName = "count")]
	public int Count
	{
		get { return (this.Orders != null) ? this.Orders.Length : 0; }
	}
	/// <summary>Offset</summary>
	[JsonProperty(PropertyName = "offset")]
	public int Offset { get; set; }
	/// <summary>Limit</summary>
	[JsonProperty(PropertyName = "limit")]
	public int Limit { get; set; }
	/// <summary>Orders</summary>
	[JsonProperty(PropertyName = "orders")]
	public LineLastestOrderGetResponse[] Orders { get; set; }
	/// <summary>Status</summary>
	[JsonProperty(PropertyName = "status")]
	public int Status { get; set; }
}