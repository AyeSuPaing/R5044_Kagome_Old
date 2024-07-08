/*
=========================================================================================================
  Module      : 注文取込例外クラス(ImportOrderException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Commerce.MallBatch.LiaiseLohacoMall
{
	/// <summary>
	/// 注文取込例外クラス
	/// </summary>
	public class ImportOrderException : Exception
	{
		#region +ImportOrderException コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ImportOrderException()
			: base()
		{
			// 基底クラスのコンストラクタをコールするのみ
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">エラーメッセージ</param>
		public ImportOrderException(String message)
			: base(message)
		{
			// 基底クラスのコンストラクタをコールするのみ
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">エラーメッセージ</param>
		/// <param name="exception">例外エラー</param>
		public ImportOrderException(String message, Exception exception)
			: base(message, exception)
		{
			// 基底クラスのコンストラクタをコールするのみ
		}
		#endregion
	}
}
