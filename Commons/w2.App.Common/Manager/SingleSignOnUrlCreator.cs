/*
=========================================================================================================
  Module      : シングルサインオンURL作成クラス(SingleSignOnUrlCreator.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Web;
using w2.Domain.MenuAuthority.Helper;

namespace w2.App.Common.Manager
{
	/// <summary>
	/// シングルサインオンURL作成クラス
	/// </summary>
	public class SingleSignOnUrlCreator
	{
		#region +Create 作成（WebForms）
		/// <summary>
		/// 作成（WebForms用）
		/// </summary>
		/// <param name="siteType">管理画面タイプ</param>
		/// <param name="nextUrl">遷移先URL</param>
		/// <returns>シングルサインオンURL</returns>
		public static string CreateForWebForms(MenuAuthorityHelper.ManagerSiteType siteType, string nextUrl = null)
		{
			return new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_WEBFORMS_SINGLE_SIGN_ON)
				.AddParam(Constants.REQUEST_KEY_SINGLE_SIGN_ON_MANAGER_SITE_TYPE, siteType.ToString())
				.AddParam(Constants.REQUEST_KEY_MANAGER_NEXTURL, nextUrl)
				.CreateUrl();
		}
		#endregion

		#region +CreateFromCms 作成（MVC用）
		/// <summary>
		/// 作成（MVC用）
		/// </summary>
		/// <param name="siteType">管理画面タイプ</param>
		/// <param name="nextUrl">遷移先URL</param>
		/// <returns>シングルサインオンURL</returns>
		public static string CreateForMvc(MenuAuthorityHelper.ManagerSiteType siteType, string nextUrl = null)
		{
			return new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MAANGER_CMS_SINGLE_SIGN_ON)
				.AddParam(Constants.REQUEST_KEY_SINGLE_SIGN_ON_CMS_SITE_TYPE, siteType.ToString())
				.AddParam(Constants.REQUEST_KEY_MANAGER_NEXTURL, nextUrl)
				.CreateUrl();
		}
		#endregion
	}
}