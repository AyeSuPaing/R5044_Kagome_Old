/*
=========================================================================================================
  Module      : NumberingUtilityのラッパークラス (NumberingUtilityWrapper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Util;

namespace w2.Common.Wrapper
{
	/// <summary>
	/// <see cref="NumberingUtility"/>のラッパークラス
	/// </summary>
	public abstract class NumberingUtilityWrapper
	{
		/// <summary>
		/// <see cref="NumberingUtility"/>のラッパークラス（実装）
		/// </summary>
		private class NumberingUtilityWrapperImpl : NumberingUtilityWrapper
		{
		}

		/// <summary>
		/// <see cref="NumberingUtility.CreateNewNumber(string,string)"/>のラッパーメソッド
		/// </summary>
		public virtual long CreateNewNumber(string deptId, string key)
		{
			return NumberingUtility.CreateNewNumber(deptId, key);
		}

		/// <summary>
		/// インスタンス
		/// </summary>
		public static NumberingUtilityWrapper Instance
		{
			get { return m_instance; }
			set { m_instance = value; }
		}
		/// <summary>インスタンス</summary>
		private static NumberingUtilityWrapper m_instance = new NumberingUtilityWrapperImpl();
	}
}