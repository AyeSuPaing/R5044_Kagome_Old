/*
=========================================================================================================
  Module      : DateTimeのラッパークラス (DateTimeWrapper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Common.Wrapper
{
	/// <summary>
	/// <see cref="DateTime"/>のラッパークラス
	/// </summary>
	public abstract class DateTimeWrapper
	{
		/// <summary>
		/// <see cref="DateTime.Now"/>のラッパーメソッド
		/// </summary>
		public abstract DateTime Now { get; }

		/// <summary>
		/// インスタンス
		/// </summary>
		public static DateTimeWrapper Instance
		{
			get { return DateTimeWrapper.m_instance; }
			set { DateTimeWrapper.m_instance = value; }
		}
		/// <summary>インスタンス</summary>
		private static DateTimeWrapper m_instance = new ServerDateTimeWrapper();
	}
}
