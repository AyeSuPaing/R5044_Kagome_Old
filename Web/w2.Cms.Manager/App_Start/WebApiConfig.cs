/*
=========================================================================================================
  Module      : RouteConfig(WebApiConfig.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace w2.Cms.Manager
{
	/// <summary>
	/// WebApiConfig
	/// </summary>
	public static class WebApiConfig
	{
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="config">HttpConfiguration</param>
		public static void Register(HttpConfiguration config)
		{
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}
	}
}
