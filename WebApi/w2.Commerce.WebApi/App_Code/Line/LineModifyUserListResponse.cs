/*
=========================================================================================================
  Module      : Line Modify User List Response (LineModifyUserListResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;

/// <summary>
/// LinePay Modify User List Response Object
/// </summary>
[Serializable]
public class LineModifyUserListResponse
{
	/// <summary>Count</summary>
	[JsonProperty(PropertyName = "count")]
	public int Count
	{
		get { return (this.UserIds != null) ? this.UserIds.Length : 0; }
	}
	/// <summary>Offset</summary>
	[JsonProperty(PropertyName = "offset")]
	public int Offset { get; set; }
	/// <summary>Limit</summary>
	[JsonProperty(PropertyName = "limit")]
	public int Limit { get; set; }
	/// <summary>User Ids</summary>
	[JsonProperty(PropertyName = "user_ids")]
	public string[] UserIds { get; set; }
}