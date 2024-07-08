/*
=========================================================================================================
  Module      : 変換ユーティリティクラス(ConvertUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Xml;

namespace w2.Commerce.Batch.LiaiseAmazonMall.Util
{
	/// <summary>
	/// 変換ユーティリティクラス
	/// </summary>
	public static class ConvertUtility
	{
		#region +ConvertStringToDecimal StringからDecimalに変換
		/// <summary>
		/// StringからDecimalに変換
		/// </summary>
		/// <param name="target">変換対象</param>
		/// <returns>変換後</returns>
		/// <remarks>一度Stringに変換してからでないとエラーになる項目があるため</remarks>
		public static decimal ConvertStringToDecimal(string target)
		{
			var before = target.ToString();
			var result = Convert.ToDecimal(before);
			return result;
		}
		#endregion
	}
}
