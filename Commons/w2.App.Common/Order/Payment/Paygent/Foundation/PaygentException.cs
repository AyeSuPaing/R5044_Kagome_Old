/*
=========================================================================================================
  Module      : ペイジェント連携例外(PaygentException.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.Payment.Paygent.Foundation
{
	/// <summary>
	/// ペイジェント連携例外
	/// </summary>
	public class PaygentException : Exception
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">メッセージ</param>
		public PaygentException(string message)
			: base(message)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="ex">例外エラー</param>
		public PaygentException(string message, Exception ex)
			: base(message, ex)
		{
		}
	}
}
