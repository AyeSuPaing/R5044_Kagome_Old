/*
=========================================================================================================
  Module      : Line Error Response (LineErrorResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

/// <summary>
/// Line error response
/// </summary>
[Serializable]
public class LineErrorResponse
{
	/// <summary>Status</summary>
	[JsonProperty(PropertyName = "Status")]
	public int Status { get; set; }
	/// <summary>Reason</summary>
	[JsonProperty(PropertyName = "Reason")]
	public string Reason { get; set; }
}