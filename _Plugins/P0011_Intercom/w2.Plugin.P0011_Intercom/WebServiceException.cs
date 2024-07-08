/*
=========================================================================================================
  Module      : webサービス実行時エラークラス(WebServiceException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : webサービス実行に発生したExceptionをラップするためのクラス。
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Plugin.P0011_Intercom
{
	/// <summary>
	/// webサービス通信時のエラー
	/// </summary>
	public class WebServiceException : Exception 
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public WebServiceException(string message,Exception innerException) : base(message,innerException)
		{
			
		}
	}
}
