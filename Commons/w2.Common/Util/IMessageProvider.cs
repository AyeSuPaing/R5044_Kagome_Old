/*
=========================================================================================================
  Module      : メッセージ提供のインターフェース(IMessageProvider.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Common.Util
{
	/// <summary>
	/// メッセージ提供のインターフェース
	/// </summary>
	public interface IMessageProvider
	{
		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		/// <param name="replaces">置換パラメータ</param>
		/// <returns>エラーメッセージ</returns>
		string GetMessages(string messageKey, params string[] replaces);
	}
}
