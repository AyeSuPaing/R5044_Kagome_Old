/*
=========================================================================================================
  Module      : CouponOptionUtilityのラッパークラス (CouponOptionUtilityWrapper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Option;
using w2.Domain.Coupon;

namespace w2.Common.Wrapper
{
	/// <summary>
	/// <see cref="CouponOptionUtility"/>のラッパークラス
	/// </summary>
	public abstract class CouponOptionUtilityWrapper
	{
		/// <summary>
		/// <see cref="CouponOptionUtility"/>のラッパークラス（実装）
		/// </summary>
		private class CouponOptionUtilityWrapperImpl : CouponOptionUtilityWrapper
		{
		}

		/// <summary>
		/// <see cref="CouponOptionUtility.IsCouponApplyProduct(w2.Domain.Coupon.CouponModel,object)"/>のラッパーメソッド
		/// </summary>
		public virtual bool IsCouponApplyProduct(CouponModel coupon, object product)
		{
			return CouponOptionUtility.IsCouponApplyProduct(coupon, product);
		}

		/// <summary>
		/// インスタンス
		/// </summary>
		public static CouponOptionUtilityWrapper Instance
		{
			get { return m_instance; }
			set { m_instance = value; }
		}
		/// <summary>インスタンス</summary>
		private static CouponOptionUtilityWrapper m_instance = new CouponOptionUtilityWrapperImpl();
	}
}