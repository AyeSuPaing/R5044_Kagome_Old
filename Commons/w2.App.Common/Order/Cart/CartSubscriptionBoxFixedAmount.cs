/*
=========================================================================================================
  Module      : 定額頒布会金額クラス (CartSubscriptionBoxFixedAmount.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.App.Common.Order.Cart
{
	/// <summary>
	/// 定額頒布会金額
	/// </summary>
	[Serializable]
	public class CartSubscriptionBoxFixedAmount
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="courseId">頒布会コースID</param>
		/// <param name="price">定額金額</param>
		public CartSubscriptionBoxFixedAmount(string courseId, decimal price)
		{
			this.SubscriptionBoxCourseId = courseId;
			this.DiscountedPrice = price;
			this.PriceSubtotalAfterDistribution = price;
			this.ItemPriceRegulation = 0;
		}

		/// <summary>頒布会コースID</summary>
		public string SubscriptionBoxCourseId { get; set; }
		/// <summary>明細金額（割引後価格）</summary>
		public decimal DiscountedPrice { get; set; }
		/// <summary>商品小計（割引金額の按分処理適用後）</summary>
		public decimal PriceSubtotalAfterDistribution { get; set; }
		/// <summary>調整金額(按分した商品分)</summary>
		public decimal ItemPriceRegulation { get; set; }
		/// <summary>商品小計(調整金額・割引金額の按分処理適用後)</summary>
		public decimal PriceSubtotalAfterDistributionAndRegulation
		{
			get { return this.PriceSubtotalAfterDistribution + this.ItemPriceRegulation; }
		}
	}
}
