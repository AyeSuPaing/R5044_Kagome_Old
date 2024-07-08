/*
=========================================================================================================
  Module      : LINEマルチキャスト送信メッセージモデル (LineRequestMulticast.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Line.LineDirectMessage.RequestType
{
	/// <summary>
	/// LINEマルチキャスト送信メッセージモデル
	/// </summary>
	public class LineRequestMulticast : PostBodyBase
	{
		/// <summary> 送信先のID配列 (LINEの「userId」「groupId」「roomId」) </summary>
		[JsonProperty("to")]
		public string[] To { get; set; }
		/// <summary> メッセージ内容 </summary>
		[JsonProperty("messages")]
		public ILineMessage[] Messages { get; set; }
	}
}
