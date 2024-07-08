/*
=========================================================================================================
  Module      : YAHOO API 電話番号値オブジェクト (YahooMallOrderTelValueObject.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text.RegularExpressions;

namespace w2.App.Common.Mall.Yahoo.YahooMallOrders.ValueObjects
{
	/// <summary>
	/// YAHOO API 電話番号値オブジェクト
	/// </summary>
	public class YahooMallOrderTelValueObject
	{
		/// <summary>
		/// YAHOO API 電話番号値オブジェクト
		/// </summary>
		/// <param name="tel">電話番号</param>
		public YahooMallOrderTelValueObject(string tel)
		{
			var errorMessage = $"電話番号が適切なフォーマットに則っていません。zip={tel}";
			if (tel.Length > 16)
			{
				throw new ArgumentException(errorMessage);
			}

			var matchesTelFormat = Regex.IsMatch(tel, "[0-9]+-[0-9]+-[0-9]+");
			if (matchesTelFormat == false)
			{
				// 8文字以下は適切なフォーマットに沿っていない
				if (tel.Replace("-", string.Empty).Length < 8)
				{
					throw new ArgumentException(errorMessage);
				}

				var telWithoutHyphen = tel.Replace("-", string.Empty);
				var tel1 = telWithoutHyphen.Substring(startIndex: 0, length: telWithoutHyphen.Length - 8);
				var tel2 = telWithoutHyphen.Substring(startIndex: telWithoutHyphen.Length - 8, length: 4);
				var tel3 = telWithoutHyphen.Substring(startIndex: telWithoutHyphen.Length - 4, length: 4);
				this.Tel = $"{tel1}-{tel2}-{tel3}";
				this.Tel1 = tel1;
				this.Tel2 = tel2;
				this.Tel3 = tel3;
				return;
			}

			this.Tel = tel;
			var splittedTel = tel.Split("-".ToCharArray());
			this.Tel1 = splittedTel[0];
			this.Tel2 = splittedTel[1];
			this.Tel3 = splittedTel[2];
		}

		/// <summary>
		/// 値があるかどうか
		/// </summary>
		/// <returns>値があるかどうか</returns>
		public bool HasVlaue() => string.IsNullOrEmpty(this.Tel) == false;
		/// <summary>電話番号</summary>
		public string Tel { get; } = "";
		/// <summary>電話番号1</summary>
		public string Tel1 { get; } = "";
		/// <summary>電話番号2</summary>
		public string Tel2 { get; } = "";
		/// <summary>電話番号3</summary>
		public string Tel3 { get; } = "";
	}
}
