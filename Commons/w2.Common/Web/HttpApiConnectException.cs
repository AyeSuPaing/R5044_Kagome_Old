/*
=========================================================================================================
  Module      : HttpApi接続時のException(HttpApiConnectException.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Common.Web
{
	/// <summary>
	/// HttpApi接続時のException
	/// </summary>
	public class HttpApiConnectException : Exception
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private HttpApiConnectException()
			: base()
		{

		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="innerException">内部エラー</param>
		public HttpApiConnectException(Exception innerException)
			: base("API接続時にエラーが発生", innerException)
		{

		}
	}
}
