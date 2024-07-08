/*
=========================================================================================================
  Module      : URL共通処理クラス(UrlUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Text.RegularExpressions;

namespace w2.Domain.ShortUrl.Helper
{
	/// <summary>
	/// ショートURL共通処理クラス
	/// </summary>
	public class UrlUtility
	{
		/// <summary> URLの先頭(プロトコルとドメイン + /) </summary>
		public static readonly string URL_TOP = "http://" + Common.Constants.SITE_DOMAIN + "/";
		/// <summary> URLの先頭(プロトコルとドメイン + /) </summary>
		public static readonly string URL_TOP_S = "https://" + Common.Constants.SITE_DOMAIN + "/";
		/// <summary> ドメインが正しいか確認する正規表現パターン </summary>
		public static readonly string SITE_DOMAIN_REGEX_PATTERN = "^https?://" + Common.Constants.SITE_DOMAIN + "/";

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
			var result = (IsSiteDomain(url)) ? Regex.Replace(url, SITE_DOMAIN_REGEX_PATTERN, "") : url;
			return result;
		}

		/// <summary>
		/// URLにプロトコルとドメインを付与する
		/// </summary>
		/// <param name="url">対象URL</param>
		/// <returns>プロトコルとドメインが付与されたURL</returns>
		public static string AddProtocolAndDomain(string url)
		{
			var result = (Regex.IsMatch(url, "^https?://")) ? url : URL_TOP_S + url;
			return result;
		}

		/// <summary>
		/// URLにプロトコルとドメインを付与し、https://+ドメイン名はhttp://+ドメイン名に変換する
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string AddProtocolAndDomainWithReplace(string url)
		{
			var result = (Regex.IsMatch(url, "^https?://"))
				? Regex.Replace(url, "~" + URL_TOP_S, URL_TOP)
				: URL_TOP + url;
			return result;
		}
	}
}