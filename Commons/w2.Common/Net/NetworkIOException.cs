/*
=========================================================================================================
  Module      : ネットワークIO例外クラス(NetworkIOException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Common.Net
{
	/// <summary>
	/// ネットワークI/O例外
	/// </summary>
	public class NetworkIOException : Exception
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public NetworkIOException(string message, Exception innerException)
			: base (message, innerException)
		{
		}
	}
}
