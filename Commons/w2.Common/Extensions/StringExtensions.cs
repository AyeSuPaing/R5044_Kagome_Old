/*
=========================================================================================================
  Module      : stringクラス拡張モジュール(StringExtensions.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Common.Extensions
{
	///**************************************************************************************
	/// <summary>
	/// stringクラスに拡張メソッドを追加する
	/// </summary>
	///**************************************************************************************
	public static class StringExtensions
	{
		/// <summary>異体字対応マップ</summary>
		/// <remarks> http://www.h5.dion.ne.jp/~wing-x/ezhtml/ita/lst10-01.html </remarks>
		private static Dictionary<char, char> KANJI_VARIANT_CHAR_MAP = new Dictionary<char, char>()
		{
			{'賴', '頼'}, {'瀨', '瀬'}, {'靑', '青'}, {'栁', '柳'}, {'神', '神'}, {'晴', '晴'}, {'喆', '哲'},
			{'悊', '哲'}, {'寬', '寛'}, {'增', '増'}, {'德', '徳'}, {'敎', '教'}, {'﨑', '崎'}, {'曺', '曹'},
			{'隆', '隆'}, {'兊', '兌'}, {'黑', '黒'}, {'凬', '風'}, {'髙', '高'}, {'薰', '薫'}
		};
		/// <summary>
		/// [ｗ２拡張メソッド] 第二水準以下の文字に変換
		/// </summary>
		/// <param name="strSrc">変換対象</param>
		/// <returns>第二水準外の文字を含まない文字列</returns>
		public static string ToWithinLv2Chars(this string strSrc)
		{
			// 異体字が登録されている場合、第二水準以下の漢字に置換する
			StringBuilder sbSrcString = new StringBuilder(strSrc);
			foreach (KeyValuePair<char, char> kvpCharMap in KANJI_VARIANT_CHAR_MAP)
			{
				sbSrcString.Replace(kvpCharMap.Key, kvpCharMap.Value);
			}

			// 第二水準外の文字を代替文字に変換し返却
			return ReplaceOutOfLv2Chars(sbSrcString.ToString(), '□');
		}

		/// <summary>
		/// 第二水準外の文字を代替文字に変換
		/// </summary>
		/// <param name="strSrc">対象文字列</param>
		/// <param name="strSubstitute">代用文字</param>
		/// <returns>置換後文字列</returns>
		private static string ReplaceOutOfLv2Chars(string strSrc, char chSubstitute)
		{
			StringBuilder sbReplaced = new StringBuilder();
			foreach (char chSrc in strSrc.ToCharArray())
			{
				sbReplaced.Append(IsOutOfLv2CharCode(chSrc) ? chSubstitute : chSrc);
			}
			return sbReplaced.ToString();
		}

		/// <summary>第二水準以下の文字コードの範囲を設定</summary>
		/// <remarks>Key:開始コード Value:終了コード</remarks>
		private static Dictionary<uint, uint> UNDER_LV2_CHAR_RANGES = new Dictionary<uint, uint>()
		{
			{0x8140, 0x879C}, // 非漢字
			{0x889F, 0x9FFD}, // 第一水準漢字＆第二水準の一部
			{0xE040, 0xEAA4}  // 第二水準漢字残り
		};
		/// <summary>
		/// 第二水準範囲外チェック
		/// </summary>
		/// <param name="chSrc">チェック対象文字</param>
		/// <returns>true:範囲外/false:範囲内</returns>
		private static bool IsOutOfLv2CharCode(char chSrc)
		{
			//------------------------------------------------------
			// SJIS文字コードチェック
			//------------------------------------------------------
			foreach (KeyValuePair<uint, uint> kvpCodeRange in UNDER_LV2_CHAR_RANGES)
			{
				uint uiExceptCodeBgn = kvpCodeRange.Key;
				uint uiExceptCodeEnd = kvpCodeRange.Value;

				char chDummy; // IsOutOfCharCodeの引数の為のダミー
				if (w2.Common.Util.Validator.IsOutOfCharCode(chSrc.ToString(), Encoding.GetEncoding("Shift_JIS"), uiExceptCodeBgn, uiExceptCodeEnd, out chDummy) == false)
				{
					// 第二水準以下のためfalseを返す
					return false;
				}
			}

			// 第二水準範囲外のためtrueを返す
			return true;
		}

		/// <summary>
		/// [ｗ２拡張メソッド] 改行コード(EndOfLine)置換
		/// （改行コード："\r\n","\r","\n"）
		/// </summary>
		/// <param name="strSrc">変換対象</param>
		/// <param name="strReplace">置換する文字列</param>
		/// <returns>改行コード変換後文字列</returns>
		public static string ReplaceCrLf(this string strSrc, string strReplace = "")
		{
			return strSrc.ReplaceCrLf(strReplace, strReplace, strReplace);
		}

		/// <summary>
		/// [ｗ２拡張メソッド] 改行コード(EndOfLine)置換
		/// （改行コード："\r\n","\r","\n"）
		/// </summary>
		/// <param name="strSrc">変換対象</param>
		/// <param name="strCrLf">crlfを置換する文字列</param>
		/// <param name="strCr">crを置換する文字列</param>
		/// <param name="strLf">lfを置換する文字列</param>
		/// <returns>改行コード変換後文字列</returns>
		public static string ReplaceCrLf(this string strSrc, string strCrLf, string strCr, string strLf)
		{	
			// nullの場合はnullのまま返す
			return (strSrc != null) ? strSrc.Replace("\r\n", strCrLf).Replace("\r", strCr).Replace("\n", strLf) : strSrc;
		}

		/// <summary>
		/// nullまたは空文字なら別の文字に差し替えて返却します。
		/// </summary>
		/// <param name="src">変換対象</param>
		/// <param name="newValue">変換後の文字列</param>
		/// <returns>変換結果</returns>
		public static string ConvertIfNullEmpty(this string src, string newValue)
		{
			return (string.IsNullOrEmpty(src)) ? newValue : src;
		}

		/// <summary>
		/// <paramref name="src"/>から空白文字を取り除いた文字列を返却します。
		/// </summary>
		/// <param name="src">空白文字を取り除く文字列</param>
		/// <returns>空白文字を取り除いた文字列</returns>
		public static string RemoveWhiteSpaceChars(this string src)
		{
			var result = new string(src.Where(c => (char.IsWhiteSpace(c) == false)).ToArray());
			return result;
		}

		/// <summary>
		/// 文字列の一番左から一致する<param name="value">を取り除いた文字列を返却する</param>
		/// </summary>
		/// <param name="src">操作する文字列</param>
		/// <param name="value">取り除く文字列</param>
		/// <returns>取り除いた文字列</returns>
		public static string RemoveLeft(this string src, string value)
		{
			var result = src.StartsWith(value) ? src.Remove(0, value.Length) : src;
			return result;
		}

		/// <summary>
		/// 対象文字列から指定した文字列以降を削除します。
		/// ただし、対象文字列内に指定した文字列が存在しなかった場合、対象文字列をそのまま返却します。
		/// </summary>
		/// <param name="str">対象文字列</param>
		/// <param name="removeStr">指定文字列</param>
		/// <returns>対象文字列から指定文字列を削除した文字列</returns>
		public static string RemoveRight(this string str, string removeStr)
		{
			var length = str.IndexOf(removeStr, StringComparison.InvariantCulture);
			return (length < 0) ? str : str.Substring(0, length);
		}

		/// <summary>
		/// Join to string
		/// </summary>
		/// <typeparam name="T">IEnumerable</typeparam>
		/// <param name="source">Source</param>
		/// <param name="filter"></param>
		/// <param name="separator">Separator</param>
		/// <returns>String after join</returns>
		public static string JoinToString<T>(this IEnumerable<T> source, Func<T, string> filter, string separator = null)
		{
			var result = source.Select(filter).JoinToStringWithSeparator(separator);
			return result;
		}

		/// <summary>
		/// Join to string with separator
		/// </summary>
		/// <typeparam name="T">IEnumerable</typeparam>
		/// <param name="source">Source</param>
		/// <param name="separator">Separator</param>
		/// <returns>String after join</returns>
		public static string JoinToStringWithSeparator<T>(this IEnumerable<T> source, string separator = null)
		{
			var iterator = (typeof(T) == typeof(string))
				? source.Cast<string>()
				: source.Select(x => x.ToString());

			var result = (separator != null)
				? string.Join(separator, iterator)
				: string.Concat(iterator);
			return result;
		}

		/// <summary>
		/// Remove QueryString By Key
		/// </summary>
		/// <param name="url"> string with url format type</param>
		/// <param name="key">String key of the entry to remove</param>
		/// <returns> return url string without key </returns>
		public static string RemoveQueryStringByKey(this string url, string key)
		{
			var uri = new Uri(url);

			// this gets all the query string key value pairs as a collection
			var newQueryString = System.Web.HttpUtility.ParseQueryString(uri.Query);

			// this removes the key if exists
			newQueryString.Remove(key);

			// this gets the page path from root without QueryString
			string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

			return newQueryString.Count > 0
				? String.Format("{0}?{1}", pagePathWithoutQueryString, newQueryString)
				: pagePathWithoutQueryString;
		}
	}
}
