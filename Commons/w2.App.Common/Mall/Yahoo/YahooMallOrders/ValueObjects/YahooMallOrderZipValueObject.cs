/*
=========================================================================================================
  Module      : YAHOO API 郵便番号値オブジェクト (YahooMallOrderZipValueObject.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text.RegularExpressions;

namespace w2.App.Common.Mall.Yahoo.YahooMallOrders.ValueObjects
{
	/// <summary>
	/// YAHOO API 郵便番号値オブジェクト
	/// </summary>
	public class YahooMallOrderZipValueObject
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="zip">電話番号</param>
		public YahooMallOrderZipValueObject(string zip)
		{
			var errorMessage = $"郵便番号が適切なフォーマットに則っていません。zip={zip}";
			if (string.IsNullOrEmpty(zip) || zip.Length > 30)
			{
				throw new ArgumentException(errorMessage);
			}

			var matchesJpStandardZipFormat = Regex.IsMatch(zip, "[0-9]{3}-[0-9]{4}");
			if (matchesJpStandardZipFormat == false)
			{
				if (zip.Length < 5)
				{
					throw new ArgumentException(errorMessage);
				}

				var zipWithoutHyphen = zip.Replace("-", string.Empty);
				var zip1 = zipWithoutHyphen.Substring(startIndex: 0, length: 3);
				var zip2 = zipWithoutHyphen.Substring(startIndex: 3);
				this.Zip = $"{zip1}-{zip2}";
				this.Zip1 = zip1;
				this.Zip2 = zip2;
				return;
			}

			this.Zip = zip;
			this.Zip1 = zip.Substring(startIndex: 0, length: 3);
			this.Zip2 = zip.Substring(startIndex: 4);
		}

		/// <summary>
		/// 値があるかどうか
		/// </summary>
		/// <returns>値があるかどうか</returns>
		public bool HasVlaue() => string.IsNullOrEmpty(this.Zip) == false;
		
		/// <summary>郵便番号</summary>
		public string Zip { get; } = "";
		/// <summary>郵便番号 上3桁</summary>
		public string Zip1 { get; } = "";
		/// <summary>郵便番号 下4桁</summary>
		public string Zip2 { get; } = "";
	}
}
