/*
=========================================================================================================
  Module      : 決済通貨エクステンションクラス (KeyCurrencyExtensions.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Global.Region.Currency;
using w2.Common.Util;

namespace w2.App.Common.Extensions.Currency
{
	/// <summary>
	/// 決済通貨エクステンションクラス
	/// </summary>
	public static class KeyCurrencyExtensions
	{
		/// <summary>
		/// 基軸通貨価格取得 文字列型 通貨記号、セパレータを付与
		/// </summary>
		/// <param name="price">基軸通貨価格</param>
		/// <param name="withSymbol">通貨フォーマットの有無 true→通貨フォーマット適用あり(カンマ区切り、通貨記号あり) false→通貨フォーマット適用なし</param>
		/// <returns>基軸通貨価格</returns>
		public static string ToPriceString(this object price, bool withSymbol = false)
		{
			return CurrencyManager.ToPriceByKeyCurrency(price, withSymbol);
		}

		/// <summary>
		/// 基軸通貨価格取得 Decimal型
		/// </summary>
		/// <param name="price">基軸通貨価格</param>
		/// <param name="format">Round→四捨五入、RoundDown→切り捨て、RoundUp→切り上げ</param>
		/// <returns>基軸通貨価格</returns>
		public static decimal? ToPriceDecimal(this Object price, DecimalUtility.Format format = DecimalUtility.Format.RoundDown)
		{
			return CurrencyManager.ConvertPriceByKeyCurrency(price, format);
		}
	}
}
