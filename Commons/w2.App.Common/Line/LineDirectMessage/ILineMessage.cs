/*
=========================================================================================================
  Module      : LINE送信メッセージ内容インターフェース (ILineMessage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Line.LineDirectMessage
{
	/// <summary>
	/// LINE送信メッセージ内容インターフェース
	/// </summary>
	public interface ILineMessage
	{
		/// <summary> メッセージタイプ </summary>
		string MessageType { get; }
	}
}
