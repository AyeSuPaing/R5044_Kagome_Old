/*
=========================================================================================================
  Module      : Linq拡張クラス(LinqExtension.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace w2.ExternalAPI.Common.Extension
{
	public static class LinqExtension
	{
		/// <summary>
		/// 接続子で区切られたキー値の文字列（例："key=value"）の配列を文字列辞書型配列に変換する
		/// </summary>
		/// <param name="pairs">文字列配列（LINQ）</param>
		/// <param name="connector">接続子</param>
		/// <param name="forceKeyToLower">キーとなる文字列を小文字にする</param>
		/// <returns>文字列辞書型配列</returns>
		public static Dictionary<string, string> Convert2Dictionary(this IEnumerable<string> pairs, char connector, bool forceKeyToLower = false)
		{
			return pairs.Aggregate<string, Dictionary<string, string>>(new Dictionary<string, string>(),
				(acc, pair) =>
				{
					var match = Regex.Match(pair, string.Format("^(.+){0}(.+)", connector), RegexOptions.RightToLeft);

					if (match.Success)
						acc.Add((forceKeyToLower ? match.Groups[1].Value.ToLower() : match.Groups[1].Value), match.Groups[2].Value);
					// 値が指定されておらず、キーだけ指定された場合。:が指定されている。
					else if (Regex.Match(pair, string.Format("^(.+){0}", connector), RegexOptions.RightToLeft).Success)
						acc.Add((forceKeyToLower ? pair.ToLower() : pair).Substring(0, pair.Length - 1), "");	// キーから:を削除して、空白を追加
					// 値が指定されておらず、キーだけ指定された場合。:も指定されていない。キーだけ。
					else
						acc.Add((forceKeyToLower ? pair.ToLower() : pair), "true"); // 値が指定されていなければ、単に"true"を格納

					return acc;
				});
		}
	}
}
