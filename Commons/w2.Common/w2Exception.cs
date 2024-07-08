/*
=========================================================================================================
  Module      : ｗ２例外クラス(w2Exception.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.Common
{
	///**************************************************************************************
	/// <summary>
	/// w2独自の例外を提供する
	/// </summary>
	///**************************************************************************************
	public class w2Exception : Exception
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		public w2Exception(string strMessage)
			: base(strMessage)
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strMessage">メッセージ</param>
		/// <param name="ex">例外</param>
		public w2Exception(string strMessage, Exception ex)
			: base(strMessage, ex)
		{
		}
	}
}
