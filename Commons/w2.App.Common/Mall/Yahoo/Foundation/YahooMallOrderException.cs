/*
=========================================================================================================
  Module      : Yahooモール注文取込例外クラス(YahooMallOrderException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.App.Common.Mall.Yahoo.Foundation
{
	/// <summary>
	/// Yahooモール注文取込独自の例外を提供する
	/// </summary>
	public class YahooMallOrderException : Exception
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		public YahooMallOrderException(string strMessage)
			: base(strMessage)
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		/// <param name="ex">例外</param>
		public YahooMallOrderException(string strMessage, Exception ex)
			: base(strMessage, ex)
		{
		}
	}
}
