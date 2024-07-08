/*
=========================================================================================================
  Module      : メッセージ提供クラス（ダミー）(MessageProviderDummy.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Common.Util
{
	/// <summary>
	/// メッセージ提供クラス（ダミー）
	/// </summary>
	/// <remarks>
	/// GetMessagesで、引数のメッセージキーをそのまま返却する
	/// </remarks>
	public class MessageProviderDummy : IMessageProvider
	{
		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		/// <param name="replaces">置換パラメータ</param>
		/// <returns>エラーメッセージ</returns>
		public string GetMessages(string messageKey, params string[] replaces)
		{
			return messageKey;
		}
	}
}
