/*
=========================================================================================================
  Module      : クッキーマネージャ(CookieManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;

namespace w2.Common.Web
{
	/// <summary>
	/// クッキーマネージャ
	/// </summary>
	public class CookieManager
	{
		/// <summary>SameSiteMode定義</summary>
		private enum SameSiteMode
		{
			/// <summary>Undefined</summary>
			Undefined = -1,
			/// <summary>None</summary>
			None = 0,
			/// <summary>Lax</summary>
			Lax = 1,
			/// <summary>Strict</summary>
			Strict = 2
		}

		/// <summary>
		/// クッキーの値を取得
		/// </summary>
		/// <param name="name">クッキー名</param>
		/// <returns>クッキーの値（取得できなかった場合はnull）</returns>
		public static string GetValue(string name)
		{
			var cookie = Get(name);
			return (cookie != null) ? cookie.Value : null;
		}

		/// <summary>
		/// レスポンスクッキーの値を取得（通常は利用しないが、レスポンスにセットされた値を確認したい時に利用）
		/// </summary>
		/// <param name="name">クッキー名</param>
		/// <returns>クッキーの値（取得できなかった場合はnull）</returns>
		public static string GetResponseValue(string name)
		{
			var cookie = GetResponse(name);
			return (cookie != null) ? cookie.Value : null;
		}

		/// <summary>
		/// クッキー取得
		/// </summary>
		/// <param name="name">クッキー名</param>
		/// <returns>クッキー</returns>
		public static HttpCookie Get(string name)
		{
			var cookie = RequestCookies[name];
			return cookie;
		}

		/// <summary>
		/// レスポンスクッキー取得（通常は利用しないが、レスポンスにセットされた値を確認したい時に利用）
		/// </summary>
		/// <param name="name">クッキー名</param>
		/// <returns>クッキー</returns>
		public static HttpCookie GetResponse(string name)
		{
			// レスポンスを直接参照すると追加してしまうので、キーの存在を確認してから返す
			if (ResponseCookies.AllKeys.Contains(name))
			{
				var cookie = ResponseCookies[name];
				return cookie;
			}
			return null;
		}

		/// <summary>
		/// クッキー削除
		/// </summary>
		/// <param name="name">クッキー名</param>
		/// <param name="path">対象パス</param>
		public static void RemoveCookie(string name, string path)
		{
			SetCookie(name, "", path, DateTime.Now.AddYears(-1));
		}

		/// <summary>
		/// クッキーセット
		/// </summary>
		/// <param name="name">クッキー名</param>
		/// <param name="value">値</param>
		/// <param name="path">対象パス</param>
		/// <param name="expires">有効期限</param>
		/// <param name="secure">セキュアフラグ</param>
		/// <param name="httpOnly">http onlyか</param>
		public static void SetCookie(
			string name,
			string value,
			string path,
			DateTime? expires = null,
			bool? secure = null,
			bool httpOnly = true)
		{
			var cookie = CreateHttpCookie(name, path, expires, secure, httpOnly);
			cookie.Value = value;

			// CookieはRemoveしてからAddする
			ResponseCookies.Remove(name);
			ResponseCookies.Add(cookie);
		}
		/// <summary>
		/// クッキーセット
		/// </summary>
		/// <param name="name">クッキー名</param>
		/// <param name="values">値</param>
		/// <param name="path">対象パス</param>
		/// <param name="expires">有効期限</param>
		/// <param name="secure">セキュアフラグ</param>
		/// <param name="httpOnly">http onlyか</param>
		public static void SetCookie(
			string name,
			Dictionary<string, string> values,
			string path,
			DateTime? expires = null,
			bool? secure = null,
			bool httpOnly = true)
		{
			var cookie = CreateHttpCookie(name, path, expires, secure, httpOnly);
			foreach (var value in values)
			{
				cookie.Values[value.Key] = value.Value;
			}

			// CookieはRemoveしてからAddする
			ResponseCookies.Remove(name);
			ResponseCookies.Add(cookie);
		}

		/// <summary>
		/// HttpCookieの作成
		/// ※Value値は呼び出し元でセットして下さい
		/// </summary>
		/// <param name="name">クッキー名</param>
		/// <param name="path">対象パス</param>
		/// <param name="expires">有効期限</param>
		/// <param name="secure">セキュアフラグ</param>
		/// <param name="httpOnly">http onlyか</param>
		/// <returns>Cookie</returns>
		private static HttpCookie CreateHttpCookie(
			string name,
			string path,
			DateTime? expires,
			bool? secure,
			bool httpOnly)
		{
			var cookie = new HttpCookie(name)
			{
				Path = path,
				HttpOnly = httpOnly,
				// secure属性の指定がない場合はWeb.configに設定されている値に変更
				Secure = secure
					?? ((HttpCookiesSection)WebConfigurationManager.GetSection("system.web/httpCookies")).RequireSSL,
			};

			// 一旦SameSiteをNoneに統一する(Chrome version80対応)
			// ただし、一部UAの場合はLaxとする(主にiOS12のための対応)
			var sameSite =
				((string.IsNullOrEmpty(HttpContext.Current.Request.UserAgent) == false)
					&& Regex.IsMatch(HttpContext.Current.Request.UserAgent, Constants.DISALLOW_SAMESITE_NONE_USERAGENTSPATTERN))
					? SameSiteMode.Lax
					: SameSiteMode.None;

			ChangeSameSite(cookie, sameSite);

			// 有効期限のセット
			if (expires.HasValue) cookie.Expires = expires.Value;

			return cookie;
		}

		/// <summary>
		/// SameSiteをLaxへ変更し、クッキーを再作成
		/// </summary>
		/// <param name="name">クッキー名</param>
		public static void SetCookieToLax(string name)
		{
			var newCookie = Get(name);
			if (newCookie == null) return;

			ChangeSameSite(newCookie, SameSiteMode.Lax);

			// CookieはRemoveしてからAddする
			ResponseCookies.Remove(name);
			ResponseCookies.Add(newCookie);
		}

		/// <summary>
		/// SameSiteを変更
		/// </summary>
		/// <param name="cookie">クッキー</param>
		/// <param name="mode">SameSite属性</param>
		private static void ChangeSameSite(HttpCookie cookie, SameSiteMode mode)
		{
			var sameSite = cookie.GetType().GetProperty("SameSite");
			if (sameSite != null)
			{
				sameSite.SetValue(cookie, Convert.ToInt32(mode));
			}
		}

		/// <summary>リクエストクッキー</summary>
		public static HttpCookieCollection RequestCookies { get { return HttpContext.Current.Request.Cookies; } }
		/// <summary>レスポンスクッキー</summary>
		public static HttpCookieCollection ResponseCookies { get { return HttpContext.Current.Response.Cookies; } }
	}
}