/*
=========================================================================================================
  Module      : RouteConfig(RouteConfig.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using w2.App.Common;

namespace w2.Cms.Manager
{
	/// <summary>
	/// RouteConfig
	/// </summary>
	public class RouteConfig
	{
		/// <summary>
		/// ルート登録
		/// </summary>
		/// <param name="routes">ルート</param>
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: Constants.CONTROLLER_W2CMS_MANAGER_LOGIN,
				url: "{controller}/{action}/{id}",
				defaults: new { controller = Constants.CONTROLLER_W2CMS_MANAGER_LOGIN, action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}