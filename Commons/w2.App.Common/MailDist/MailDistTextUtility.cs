/*
=========================================================================================================
  Module      : メール配信文章オブジェクト(MailDistText.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace w2.App.Common.MailDist
{
	///*********************************************************************************************
	/// <summary>
	/// メール配信文章クラス
	/// </summary>
	///*********************************************************************************************
	[Serializable]
	public class MailDistTextUtility
	{
		public readonly static string PATTURN_URL_TEXT = @"https?:\/\/[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+";
		public readonly static string PATTURN_URL_HTML = @"<a\s+[^>]*href\s*=\s*[""'](?<href>https?://[^""']*)[""'][^>]*>((?!</a>).)*</a>";

		/// <summary>
		/// URLパターン取得
		/// </summary>
		/// <param name="blIsHtml">HTML区分</param>
		/// <returns>URLパターン</returns>
		public static string GetPatturnUrl(bool blIsHtml)
		{
			return blIsHtml ? PATTURN_URL_HTML : PATTURN_URL_TEXT;
		}

		/// <summary>
		/// 区切り文字取得
		/// </summary>
		/// <param name="blIsHtml">HTML区分</param>
		/// <returns>区切り文字</returns>
		public static string GetSeparatePattern(bool blIsHtml)
		{
			return blIsHtml ? @"</a>" : "\n";
		}

		/// <summary>
		/// メール配信文章リスト作成
		/// </summary>
		/// <returns>メール配信文章リスト</returns>
		/// <remarks>区切り文字で区切った文章のリストを構築する</remarks>
		public static List<string> CreateMailTextLines(string strMailText, string strSeparatePattern)
		{
			// 一行ずつ格納（最後の要素以外は区切り文字を終端に付与）
			List<string> lMailTextLines = new List<string>(Regex.Split(strMailText, strSeparatePattern));
			for (int iLoop = 0; iLoop < lMailTextLines.Count - 1; iLoop++)
			{
				lMailTextLines[iLoop] += strSeparatePattern;
			}

			return lMailTextLines;
		}
	}
}
