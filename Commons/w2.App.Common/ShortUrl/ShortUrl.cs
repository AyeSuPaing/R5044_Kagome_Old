/*
=========================================================================================================
  Module      : ショートURL共通処理クラス(ShortUrl.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System.Text.RegularExpressions;

namespace w2.App.Common.ShortUrl
{
	public class ShortUrl
	{
		/// <summary> URLの先頭(プロトコルとドメイン + /) </summary>
		public static readonly string URL_TOP = "http://" + Constants.SITE_DOMAIN + "/";
		/// <summary> URLの先頭(プロトコルとドメイン + /) </summary>
		public static readonly string URL_TOP_S = "https://" + Constants.SITE_DOMAIN + "/";
		/// <summary> ドメインが正しいか確認する正規表現パターン </summary>
		public static readonly string SITE_DOMAIN_REGEX_PATTERN = "^https?://" + Constants.SITE_DOMAIN + "/";

		/// <summary>
		/// ドメインがフロントサイトと一致するか
		/// </summary>
		/// <param name="url">対象URL</param>
		/// <returns>True:一致 False:一致しない</returns>
		public static bool IsSiteDomain(string url)
		{
			return Regex.IsMatch(url, SITE_DOMAIN_REGEX_PATTERN);
		}

		/// <summary>
		/// URLからプロトコルとドメインを削除する
		/// </summary>
		/// <param name="url">対象URL</param>
		/// <returns>プロトコルとドメインが削除されたURL</returns>
		public static string RemoveProtocolAndDomain(string url)
		{
			if (IsSiteDomain(url))
			{
				return Regex.Replace(url, SITE_DOMAIN_REGEX_PATTERN, "");
			}

			return url;
		}

		/// <summary>
		/// URLにプロトコルとドメインを付与する
		/// </summary>
		/// <param name="url">対象URL</param>
		/// <returns>プロトコルとドメインが付与されたURL</returns>
		public static string AddProtocolAndDomain(string url)
		{
			if (Regex.IsMatch(url, "^https?://"))
			{
				return url;
			}

			return URL_TOP_S + url;
		}

		/// <summary>
		/// URLにプロトコルとドメインを付与し、http://+ドメイン名はhttps://+ドメイン名に変換する
		/// </summary>
		/// <param name="url"></param>
		/// <returns>プロトコルとドメインが付与されたURL</returns>
		public static string AddProtocolAndDomainWithReplace(string url)
		{
			if (Regex.IsMatch(url, "^https?://"))
			{
				return Regex.Replace(url, "~" + URL_TOP, URL_TOP_S);
			}

			return URL_TOP_S + url;
		}
	}
}
