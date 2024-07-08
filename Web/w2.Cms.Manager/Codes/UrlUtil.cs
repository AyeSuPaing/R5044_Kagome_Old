/*
=========================================================================================================
  Module      : URLユーティリティ(UrlUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web;
using System.Web.Mvc;
using w2.Common.Web;

namespace w2.Cms.Manager.Codes
{
	/// <summary>
	/// URLユーティリティ
	/// </summary>
	public class UrlUtil
	{
		/// <summary>
		/// 未ログインエラーページURL作成
		/// </summary>
		/// <returns>エラーページURL</returns>
		public static string CreateUnloggedInErrorUrl()
		{
			var url = CreateErrorPageUrl(
				Constants.REQUEST_KBN_ERROR_KBN_UNLOGGEDIN_ERROR,
				Constants.REQUEST_KBN_ERRORPAGE_TYPE_DISP_GOTOLOGIN);
			return url;
		}

		/// <summary>
		/// 店舗オペレータアクセス権なしエラーページURL作成
		/// </summary>
		/// <returns>エラーページURL</returns>
		public static string CreateShopOperatorUnaccessableErrorUrl()
		{
			var url = CreateErrorPageUrl(
				Constants.REQUEST_KBN_ERROR_KBN_404_ERROR, // 404として扱う
				Constants.REQUEST_KBN_ERRORPAGE_TYPE_DISP_GOTOLOGIN);
			return url;
		}

		/// <summary>
		/// CMSオプションOFFエラーページURL作成
		/// </summary>
		/// <returns>エラーページURL</returns>
		public static string CreateUnCmsOptionEnabledErrorUrl()
		{
			var url = CreateErrorPageUrl(Constants.REQUEST_KBN_ERROR_KBN_UNCMS_OPTION_DISABLED_ERROR);
			return url;
		}

		/// <summary>
		/// エラーページURL作成
		/// </summary>
		/// <param name="errorKbn">エラー区分（空の場合はパラメタなし）</param>
		/// <param name="errorPageType">エラーページタイプ</param>
		/// <returns>エラーページURL</returns>
		public static string CreateErrorPageUrl(string errorKbn = null, string errorPageType = null)
		{
			var url = UrlHelper.Action(
				"",
				Constants.CONTROLLER_W2CMS_MANAGER_ERROR,
				new
				{
					ErrorKbn = string.IsNullOrEmpty(errorKbn) ? "" : errorKbn,
					ErrorPageType = string.IsNullOrEmpty(errorPageType) ? "" : errorPageType,
				});
			return url;
		}

		/// <summary>
		/// 機能一覧URL作成
		/// </summary>
		/// <returns>URL</returns>
		public static string CreatepageIndexListUrl()
		{
			var url = Constants.PATH_ROOT + Constants.CONTROLLER_W2CMS_MANAGER_PAGE_INDEX_LIST;
			return url;
		}

		/// <summary>
		/// Create login expired login page url
		/// </summary>
		/// <returns>Login expired login page url</returns>
		public static string CreateLoginExpiredLoginPageUrl()
		{
			var url = UrlHelper.Action(
				string.Empty,
				Constants.CONTROLLER_W2CMS_MANAGER_LOGIN,
				new
				{
					LoginExpiredFlg = Constants.REQUEST_KEY_MANAGER_LOGIN_EXPIRED_FLG_VALID,
					nurl = WebUtility.GetRawUrl(HttpContext.Current.Request),
				});
			return url;
		}

		/// <summary>URLヘルパー</summary>
		private static UrlHelper UrlHelper
		{
			get { return new UrlHelper(HttpContext.Current.Request.RequestContext); }
		}
	}
}