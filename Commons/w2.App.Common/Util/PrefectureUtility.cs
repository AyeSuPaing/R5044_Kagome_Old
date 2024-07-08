/*
=========================================================================================================
  Module      : 都道府県クラス(PrefectureUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;

namespace w2.App.Common.Util
{
	/// <summary>
	/// PrefectureUtility の概要の説明です
	/// </summary>
	public static class PrefectureUtility
	{
		/// <summary>
		/// 都道府県リスト取得
		/// </summary>
		/// <param name="hashString">ハッシュ文字列</param>
		/// <returns>都道府県リスト</returns>
		public static string[] GetPrefectures(string hashString)
		{
			var binString = ConvHexToBin(hashString);
			var prefectures = binString.ToCharArray().Reverse().ToArray();
			var result = new string[0];
			for (var i = 0; i < prefectures.Length; i++)
			{
				if (prefectures[i] == '1')
				{
					result = result.Concat(new[] { Constants.STR_PREFECTURES_LIST[i] }).ToArray();
				}
			}

			return result;
		}

		/// <summary>
		/// ハッシュ文字列取得
		/// </summary>
		/// <param name="prefectures">都道府県リスト</param>
		/// <returns>ハッシュ文字列</returns>
		public static string GetHashString(string[] prefectures)
		{
			if (prefectures.Length == 0) return "";
			var result = Constants.STR_PREFECTURES_LIST.Select(p => prefectures.Contains(p) ? "1" : "0").ToArray()
				.Reverse().ToArray();
			return ConvBinToHex(string.Join("", result));
		}

		/// <summary>
		/// 16進数 -> 2進数
		/// </summary>
		/// <param name="value">16進数</param>
		/// <returns>2進数</returns>
		private static string ConvHexToBin(string value)
		{
			if (string.IsNullOrEmpty(value)) return string.Empty;
			return Convert.ToString(Convert.ToInt64(value, 16), 2);
		}

		/// <summary>
		/// 2進数 -> 16進数
		/// </summary>
		/// <param name="value">2進数</param>
		/// <returns>16進数</returns>
		private static string ConvBinToHex(string value)
		{
			if (string.IsNullOrEmpty(value)) return string.Empty;
			return Convert.ToString(Convert.ToInt64(value, 2), 16);
		}
	}
}