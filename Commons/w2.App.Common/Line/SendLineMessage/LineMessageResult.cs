/*
=========================================================================================================
  Module      : LINE API 送信情報返却値モデル (LineMessageResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Line.SendLineMessage
{
	/// <summary>
	/// LINE API 送信情報返却値モデル
	/// </summary>
	[JsonObject("line_message")]
	public class LineMessageResult : ResultBase
	{
	}
}
