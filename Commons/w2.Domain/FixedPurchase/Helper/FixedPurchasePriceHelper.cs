/*
=========================================================================================================
  Module      : 定期購入金額計算のためのヘルパクラス (FixedPurchasePriceHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.Domain.FixedPurchase.Helper
{
	/// <summary>
	/// 定期購入金額計算のためのヘルパクラス
	/// </summary>
	public class FixedPurchasePriceHelper
	{
		#region +GetValidPrice 優先順位順で値がある金額を取得
		/// <summary>
		/// 優先順位順で値がある金額を取得
		/// </summary>
		/// <param name="fixedPurchasePrice">定期購入価格</param>
		/// <param name="memberRankPrice">会員ランク価格</param>
		/// <param name="specialPrice">特別価格</param>
		/// <param name="price">通常価格</param>
		/// <returns>優先度の高い価格</returns>
		public static decimal GetValidPrice(
			decimal? fixedPurchasePrice,
			decimal? memberRankPrice,
			decimal? specialPrice,
			decimal price)
		{
			return fixedPurchasePrice ?? memberRankPrice ?? specialPrice ?? price;
		}
		#endregion

		#region +GetItemPrice 明細金額（小計）取得
		/// <summary>
		/// 明細金額（小計）取得
		/// </summary>
		/// <returns>明細金額（小計）</returns>
		public static decimal GetItemPrice(int itemQuantity, decimal price)
		{
			return itemQuantity * price;
		}
		#endregion
	}
}
