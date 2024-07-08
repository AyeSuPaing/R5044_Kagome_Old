/*
=========================================================================================================
  Module      : LINE送信テキストメッセージモデル (LineMessageText.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;

namespace w2.App.Common.Line.LineDirectMessage.MessageType
{
	/// <summary>
	/// LINE送信テキストメッセージモデル
	/// </summary>
	public class LineMessageText : ILineMessage
	{
		/// <summary> メッセージタイプ </summary>
		[JsonProperty("type")]
		public string MessageType { get { return "text"; } }
		/// <summary> 本文 </summary>
		[JsonProperty("text")]
		public string MessageText { get; set; }
	}
}