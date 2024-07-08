/*
=========================================================================================================
  Module      : サーバー日時提供実装クラス (ServerDateTimeWrapper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Common.Wrapper
{
	/// <summary>
	/// サーバー日時提供実装クラス
	/// </summary>
	public class ServerDateTimeWrapper : DateTimeWrapper
	{
		/// <summary>
		/// <see cref="DateTime.Now"/>のラッパーメソッド
		/// </summary>
		public override DateTime Now
		{
			get { return DateTime.Now; }
		}
	}
}