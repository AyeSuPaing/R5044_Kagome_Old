/*
=========================================================================================================
  Module      : メール解析例外エラー (MailParsingException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Commerce.MallBatch.MailOrderGetter
{
	///**************************************************************************************
	/// <summary>
	/// メール解析に失敗した場合にthrowする
	/// </summary>
	///**************************************************************************************
	public class MailParsingException : Exception
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">メッセージ</param>
		public MailParsingException(string message) : base(message)
		{
			// 基底クラスのコンストラクタをコールするのみ
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="ex">例外エラー</param>
		public MailParsingException(string message, Exception ex) : base(message, ex)
		{
			// 基底クラスのコンストラクタをコールするのみ
		}
	}
}
