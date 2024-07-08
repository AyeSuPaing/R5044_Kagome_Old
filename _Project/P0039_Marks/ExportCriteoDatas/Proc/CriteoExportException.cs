/*
=========================================================================================================
  Module      : Criteo例外(CriteoExportException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Commerce.Batch.ExportCriteoDatas.Proc
{
	/// <summary>
	/// Criteo例外
	/// </summary>
	public class CriteoExportException : Exception
    {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">例外の原因を説明するエラーメッセージ</param>
		/// <param name="innerException">現在の例外の原因である例外。内部例外が指定されていない場合は、null 参照 (Visual Basic の場合は Nothing)。</param>
		public CriteoExportException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}