/*
=========================================================================================================
  Module      : Decimal型ユーティリティクラス(DecimalUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Common.Util
{
	/// <summary>
	/// Decimal型ユーティリティクラス
	/// </summary>
	public class DecimalUtility
	{
		/// <summary>形式</summary>
		public enum Format
		{
			/// <summary>四捨五入</summary>
			Round,
			/// <summary>切り捨て</summary>
			RoundDown,
			/// <summary>切り上げ</summary>
			RoundUp
		};

		/// <summary>
		/// Decimal型の丸め処理
		/// </summary>
		/// <param name="dec">値</param>
		/// <param name="format">形式(四捨五入、切り捨て、切り上げ)</param>
		/// <param name="digits">丸める小数点以下の有効桁</param>
		/// <returns>処理後の値</returns>
		public static decimal DecimalRound(decimal dec, Format format, int digits = 0)
		{
			var coef = Convert.ToDecimal(Math.Pow(10, digits));
			var result = 0m;

			switch (format)
			{
				case Format.Round:
					result = Math.Round(dec, digits, MidpointRounding.AwayFromZero);
					break;

				case Format.RoundDown:
					result = Math.Floor(dec * coef) / coef;
					break;

				case Format.RoundUp:
					result = Math.Ceiling(dec * coef) / coef;
					break;
			}

			result = decimal.Parse(string.Format("{0:F" + digits + "}", result));

			return result;
		}
	}
}
