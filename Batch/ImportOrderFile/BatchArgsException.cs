/*
=========================================================================================================
 Module      : バッチ引数例外(BatchArgsException.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Commerce.Batch.ImportOrderFile
{
	/// <summary>
	/// バッチ引数例外
	/// </summary>
	public class BatchArgsException : Exception
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BatchArgsException()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">メッセージ</param>
		public BatchArgsException(string message)
			: base(message)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="innerException">内部例外</param>
		public BatchArgsException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
