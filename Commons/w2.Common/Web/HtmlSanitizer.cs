/*
=========================================================================================================
  Module      : HTMLサニタイザ(Sanitizer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using w2.Common.Util;

namespace w2.Common.Web
{
	///*********************************************************************************************
	/// <summary>
	/// HTMLのサニタイジングを行う
	/// </summary>
	///*********************************************************************************************
	public class HtmlSanitizer
	{
		/// <summary>
		/// URL属性HTMLエンコード
		/// </summary>
		/// <param name="objSrc">対象URL</param>
		/// <returns>変換文字列</returns>
		/// <remarks>
		///		<para>URL属性値をHTMLエンコードして返却する。</para>
		///		<para>許可されていない文字列がある場合は空文字を返す</para>
		///		<para></para>
		///		<para>-- http://www.ietf.org/rfc/rfc2396.txt --</para>
		///		<para>uric = reserved | unreserved | escaped</para>
		///		<para>reserved = ";" | "/" | "?" | ":" | "@" | "&amp;" | "=" | "+" | "$" | ","</para>
		///		<para>unreserved = alphanum | mark</para>
		///		<para>mark = "-" | "_" | "." | "!" | "~" | "*" | "'" | "(" | ")"</para>
		///		<para>escaped = "%" hex hex</para>
		/// </remarks>
		public static string UrlAttrHtmlEncode(object objSrc)
		{
			// URLチェック・HTMLエンコードして返す。
			return HtmlEncode(UrlAttrCheck(StringUtility.ToEmpty(objSrc)));
		}

		/// <summary>
		/// URL属性チェック
		/// </summary>
		/// <param name="objSrc">対象URL</param>
		/// <returns>変換文字列</returns>
		/// <remarks>
		/// 	<para>URL属性値を検査して返却する。</para>
		/// 	<para>許可されていない文字列がある場合は空文字を返す</para>
		/// 	<para></para>
		/// 	<para>-- http://www.ietf.org/rfc/rfc2396.txt --</para>
		/// 	<para>uric = reserved | unreserved | escaped</para>
		/// 	<para>reserved = ";" | "/" | "?" | ":" | "@" | "&amp;" | "=" | "+" | "$" | ","</para>
		/// 	<para>unreserved = alphanum | mark</para>
		/// 	<para>mark = "-" | "_" | "." | "!" | "~" | "*" | "'" | "(" | ")"</para>
		/// 	<para>escaped = "%" hex hex</para>
		/// </remarks>
		public static string UrlAttrCheck(object objSrc)
		{
			string strSrc = StringUtility.ToEmpty(objSrc);

			// パターンマッチしないければ空を返す。
			Regex rx = new Regex(@"[^;/?:@&=+\$,A-Za-z0-9\-_.!~*'()%]");
			if (rx.IsMatch(strSrc))
			{
				return "";
			}

			// スキームあり？
			rx = new Regex(@"^([A-Za-z][A-Za-z0-9+\-.]*):");
			Match match = rx.Match(strSrc);
			if (match.Success)
			{
				switch (match.Value)
				{
					case "http:":
					case "https:":
					case "mailto:":
						break;

					default:
						return "";	// 未知のスキーム
				}
			}

			return strSrc;
		}

		/// <summary>
		/// HTMLエンコード
		/// </summary>
		/// <param name="objSrc">対象オブジェクト</param>
		/// <returns>変換文字列</returns>
		public static string HtmlEncode(object objSrc)
		{
			// 「'」で括られたタグ属性内をエンコードすることも考え「'」もエンコードする
			return HttpUtility.HtmlEncode(StringUtility.ToEmpty(objSrc)).Replace("'", "&#39;");
		}

		/// <summary>
		/// HTMLエンコード（改行をBRタグ変換）
		/// </summary>
		/// <param name="objSrc">対象オブジェクト</param>
		/// <returns>変換文字列</returns>
		public static string HtmlEncodeChangeToBr(object objSrc)
		{
			return StringUtility.ChangeToBrTag(HtmlEncode(objSrc));
		}

		/// <summary>
		/// HTMLエンコード（空白文字を変換（半角スペースをnbsp、タブを#009、改行をBRタグ変換）
		/// </summary>
		/// <param name="objSrc">対象オブジェクト</param>
		/// <returns>変換文字列</returns>
		public static string HtmlEncodeChangeToBlank(object objSrc)
		{
			var encoded = HtmlEncode(objSrc);
			var changedSpace = StringUtility.ChangeToNbsp(encoded);
			var changedTab = StringUtility.ChangeToTab(changedSpace);
			return StringUtility.ChangeToBrTag(changedTab);
		}

	}
}
