/*
=========================================================================================================
  Module      : ユーザークッキーマネージャ(UserCookieManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using w2.Common.Web;

namespace w2.App.Common.Util
{
	/// <summary>
	/// ユーザークッキーマネージャ
	/// </summary>
	public class UserCookieManager
	{
		/// <summary>
		/// ログインIDをCookieへ作成
		/// </summary>
		/// <param name="loginId">入力されたログインID</param>
		/// <param name="consent">保存に同意</param>
		/// <remarks>同意していればログインID追加、そうでなければ負値でCookie削除</remarks>
		public static void CreateCookieForLoginId(string loginId, bool consent)
		{
			CookieManager.SetCookie(
				Constants.COOKIE_KEY_LOGIN_ID,
				loginId,
				Constants.PATH_ROOT,
				DateTime.Now.AddDays(consent ? Constants.LOGIN_ID_COOKIE_LIMIT_DAYS : -1),
				secure: true);
		}

		/// <summary>
		/// CookieからログインID取得
		/// </summary>
		/// <returns>ログインID</returns>
		public static string GetLoginIdFromCookie()
		{
			var cookieLoginId = CookieManager.GetValue(Constants.COOKIE_KEY_LOGIN_ID);
			return cookieLoginId ?? "";
		}

		/// <summary>汎用ユーザID（レコメンドなどで利用）</summary>
		public static string UniqueUserId
		{
			get
			{
				var cookieUserId = CookieManager.GetValue(Constants.COOKIE_KEY_USER_ID);
				return cookieUserId ?? "";
			}
			set
			{
				CookieManager.SetCookie(Constants.COOKIE_KEY_USER_ID, value, Constants.PATH_ROOT, DateTime.Now.AddYears(10));	// とりあえず10年有効
			}
		}
	}
}
