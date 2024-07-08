/*
=========================================================================================================
  Module      : 丸め計算クラス(RoundingCalculationUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Extensions.Currency;
using w2.Common.Util;

namespace w2.App.Common.Util
{
	/// <summary>
	/// 丸め計算クラス %割引の端数処理方法
	/// </summary>
	public class RoundingCalculationUtility
	{
		/// <summary>
		/// %割引の端数計算
		/// </summary>
		/// <param name="priceSubtotal">商品小計</param>
		/// <param name="discountRate">割引率</param>
		/// <returns>合計金額</returns>
		public static decimal GetRoundPercentDiscountFraction(decimal priceSubtotal, decimal discountRate)
		{
			switch (Constants.PERCENTOFF_FRACTION_ROUNDING)
			{
				// 端数切り上げ
				case Constants.FLG_PERCENTOFF_FRACTION_ROUNDING_ROUND_UP:
					return (priceSubtotal * ((discountRate >= 0 ? discountRate : 1) / 100m)).ToPriceDecimal(DecimalUtility.Format.RoundUp).Value;

				// 端数四捨五入
				case Constants.FLG_PERCENTOFF_FRACTION_ROUNDING_ROUND_OFF:
					return (priceSubtotal * ((discountRate >= 0 ? discountRate : 1) / 100m)).ToPriceDecimal(DecimalUtility.Format.Round).Value;

				// 設定なしもしくは設定値ROUNDDOWNで端数切り捨て
				default:
					return (priceSubtotal * ((discountRate >= 0 ? discountRate : 1) / 100m)).ToPriceDecimal(DecimalUtility.Format.RoundDown).Value;
			}
		}

		/// <summary>
		/// 注文単位割引端数計算
		/// </summary>
		/// <param name="cartdiscountedprice">注文単位の割引額</param>
		/// <param name="productquantity">商品数</param>
		/// <returns>商品毎の価格</returns>
		public static decimal GetRoundCartDiscountPrice(decimal cartdiscountedprice, int productquantity)
		{
			if (productquantity == 0) return cartdiscountedprice;

			switch (Constants.DISCOUNTED_PRICE_FRACTION_ROUNDING)
			{
				case Constants.FLG_DISCOUNTED_PRICE_FRACTION_ROUNDING_ROUND_UP:
					return (cartdiscountedprice / productquantity).ToPriceDecimal(DecimalUtility.Format.RoundUp).Value;
				
				case Constants.FLG_DISCOUNTED_PRICE_FRACTION_ROUNDING_ROUND_OFF:
					return (cartdiscountedprice / productquantity).ToPriceDecimal(DecimalUtility.Format.Round).Value;

				case Constants.FLG_DISCOUNTED_PRICE_FRACTION_ROUNDING_ROUND_DOWN:
				default:
					return (cartdiscountedprice / productquantity).ToPriceDecimal(DecimalUtility.Format.RoundDown).Value;
			}
		}
	}
}
