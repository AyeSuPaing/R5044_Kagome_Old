/*
=========================================================================================================
  Module      : MemberRankOptionUtilityのラッパークラス (MemberRankOptionUtilityWrapper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Option;
using w2.Common.Util;

namespace w2.Common.Wrapper
{
	/// <summary>
	/// <see cref="MemberRankOptionUtility"/>のラッパークラス
	/// </summary>
	public abstract class MemberRankOptionUtilityWrapper
	{
		/// <summary>
		/// <see cref="MemberRankOptionUtility"/>のラッパークラス（実装）
		/// </summary>
		private class MemberRankOptionUtilityWrapperImpl : MemberRankOptionUtilityWrapper
		{
		}

		/// <summary>
		/// <see cref="MemberRankOptionUtility.IsDiscountTarget(string,string)"/>のラッパーメソッド
		/// </summary>
		public virtual bool IsDiscountTarget(string strShopId, string strProductId)
		{
			return MemberRankOptionUtility.IsDiscountTarget(strShopId, strProductId);
		}

		/// <summary>
		/// インスタンス
		/// </summary>
		public static MemberRankOptionUtilityWrapper Instance
		{
			get { return m_instance; }
			set { m_instance = value; }
		}
		/// <summary>インスタンス</summary>
		private static MemberRankOptionUtilityWrapper m_instance = new MemberRankOptionUtilityWrapperImpl();
	}
}