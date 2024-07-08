/*
=========================================================================================================
  Module      : ログインチェックフィルタ属性(LoggedInCheckFilterAttribute.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace w2.Cms.Manager.Codes.Binder
{
	/// <summary>
	/// ログインチェックフィルタ属性
	/// </summary>
	public class LoggedInCheckFilterAttribute : ActionFilterAttribute
	{
		/// <summary>ログイン不要ページコントローラ</summary>
		static readonly string[] m_doNotNeedsLoginPageControllers =
		{
			Constants.CONTROLLER_W2CMS_MANAGER_LOGIN,
			Constants.CONTROLLER_W2CMS_MANAGER_ERROR,
			Constants.CONTROLLER_W2CMS_MANAGER_OPERATOR_PASSWORD_CHANGE,
			Constants.CONTROLLER_W2CMS_MANAGER_PAGE_INDEX_LIST,
		};

		/// <summary>権限チェック不要コントローラ（ログインチェックはされる）</summary>
		static readonly string[] m_doNotNeedsAuthorityCheckControllers =
		{
			Constants.CONTROLLER_W2CMS_MANAGER_PAGEPARTS_DESIGN_SUB,
			Constants.CONTROLLER_W2CMS_MANAGER_SINGLE_SIGN_ON,
		};

		/// <summary>
		/// アクション実行時
		/// </summary>
		/// <param name="filterContext">フィルタコンテキスト</param>
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current));
			var controllerName = routeData.Values["controller"].ToString();
			var sessionWrapper = new SessionWrapper();

			// ログイン不要ページコントローラの場合はログインチェック・権限チェックを行わない
			if (m_doNotNeedsLoginPageControllers.Any(
				c => string.Equals(c, controllerName, StringComparison.OrdinalIgnoreCase))) return;

			CheckLoggedIn(filterContext, sessionWrapper);

			// 権限チェック不要コントローラの場合は権限チェックを行わない
			if (m_doNotNeedsAuthorityCheckControllers.Any(
				c => string.Equals(c, controllerName, StringComparison.OrdinalIgnoreCase))) return;

			CheckMenuAuthority(filterContext, controllerName, sessionWrapper);
		}

		/// <summary>
		/// ログインチェック処理（ajaxではない場合はリダイレクトも行う）
		/// </summary>
		/// <param name="filterContext">フィルタコンテキスト</param>
		/// <param name="sessionWrapper">セッションラッパー</param>
		private static void CheckLoggedIn(
			ActionExecutingContext filterContext,
			SessionWrapper sessionWrapper)
		{
			if (sessionWrapper.IsLoggedIn == false)
			{
				var url = UrlUtil.CreateLoginExpiredLoginPageUrl();
				ErrorAction(filterContext, url);
			}
		}

		/// <summary>
		/// メニュー権限チェック
		/// </summary>
		/// <param name="filterContext">フィルタコンテキスト</param>
		/// <param name="controllerName">コントローラー名</param>
		/// <param name="sessionWrapper">セッションラッパー</param>
		private static void CheckMenuAuthority(
			ActionExecutingContext filterContext,
			string controllerName,
			SessionWrapper sessionWrapper)
		{
			if (controllerName.StartsWith("_")) return;
			var matchingString = controllerName + "/";
			var isAuthorizedMenu = sessionWrapper.LoginOperatorMenus.Any(
				largeList => largeList.SmallMenus.Any(smallMenu => smallMenu.MenuPath == matchingString));
			if (isAuthorizedMenu == false)
			{
				var url = UrlUtil.CreateShopOperatorUnaccessableErrorUrl();
				ErrorAction(filterContext, url);
			}
		}

		/// <summary>
		/// エラーアクション
		/// </summary>
		/// <param name="filterContext">フィルタコンテキスト</param>
		/// <param name="errorPageUrl">エラーページURL</param>
		private static void ErrorAction(ActionExecutingContext filterContext, string errorPageUrl)
		{
			if (filterContext.HttpContext.Request.IsAjaxRequest())
			{
				filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				filterContext.Result = new EmptyResult();
			}
			else
			{
				filterContext.HttpContext.Response.Redirect(errorPageUrl);
			}
		}
	}
}