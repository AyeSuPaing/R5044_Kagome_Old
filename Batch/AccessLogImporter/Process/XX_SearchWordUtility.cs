/*
=========================================================================================================
  Module      : 検索文字列ユーリティリティクラス(XX_SearchWordUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Web;

namespace w2.MarketingPlanner.Batch.AccessLogImporter.Process
{
	class XX_SearchWordUtility
	{
		/// <summary>デコードに失敗したら例外になるUTF8エンコーディング</summary>
		private static readonly Encoding ENCODING_UTF8_DEC_EX;

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static XX_SearchWordUtility()
		{
			ENCODING_UTF8_DEC_EX = (Encoding)Encoding.UTF8.Clone();
			ENCODING_UTF8_DEC_EX.DecoderFallback = DecoderFallback.ExceptionFallback;
		}

		/// <summary>
		/// 検索文字列URLデコード
		/// </summary>
		/// <param name="word">対象文字列</param>
		/// <returns>デコード後文字列（失敗したら空文字列）</returns>
		public static string UrlDecode(string word)
		{
			if (string.IsNullOrEmpty(word)) return "";
			try
			{
				return HttpUtility.UrlDecode(word, ENCODING_UTF8_DEC_EX);
			}
			catch
			{
				return "";
			}
		}
	}
}
