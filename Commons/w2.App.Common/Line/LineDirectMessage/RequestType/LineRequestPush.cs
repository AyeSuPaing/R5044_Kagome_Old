/*
=========================================================================================================
  Module      : LINEプッシュ送信メッセージモデル (LineRequestPush.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Line.LineDirectMessage.RequestType
{
	/// <summary>
	/// LINEプッシュ送信メッセージモデル
	/// </summary>
	public class LineRequestPush : PostBodyBase
	{
		/// <summary> 送信先のID(LINEの「userId」「groupId」「roomId」) </summary>
		[JsonProperty("to")]
		public string To { get; set; }
		/// <summary> メッセージ内容 </summary>
		[JsonProperty("messages")]
		public ILineMessage[] Messages { get; set; }
	}
}
