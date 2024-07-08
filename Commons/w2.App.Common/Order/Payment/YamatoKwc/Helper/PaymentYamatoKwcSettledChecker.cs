/*
=========================================================================================================
  Module      : ヤマトKWC 入金チェッカー(PaymentYamatoKwcSettledChecker.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Order.Payment.YamatoKwc.Helper
{
	/// <summary>
	/// ヤマトKWC 入金チェッカー
	/// </summary>
	public class PaymentYamatoKwcSettledChecker
	{
		/// <summary>
		/// コンビニチェック
		/// </summary>
		/// <param name="settleMethod">決済手段</param>
		/// <param name="settleDetail">決済詳細</param>
		/// <returns>入金OKか</returns>
		public bool Check(string settleMethod, string settleDetail)
		{
			switch (settleMethod)
			{
				// クレカは精算確定になっていたら入金OK
				case "1":
				case "2":
				case "3":
				case "4":
				case "5":
				case "6":
				case "7":
				case "8":
				case "9":
				case "10":
				case "11":
				case "12":
				case "13":
				case "14":
					return (settleDetail == "31");

				// セブンイレブン・ファミリーマートは速報で入金OKとする
				case "21":
				case "23":
					return (settleDetail == "2");

				// その他コンビニは速報未対応なので、確報時に入金OKとする
				case "22":
				case "24":
				case "25":
				case "26":
					return (settleDetail == "3");
			}
			return false;
		}
	}
}
