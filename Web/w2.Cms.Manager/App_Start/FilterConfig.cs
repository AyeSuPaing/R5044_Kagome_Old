/*
=========================================================================================================
  Module      : FilterConfig(FilterConfig.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web;
using System.Web.Mvc;
using w2.Cms.Manager.Codes;

namespace w2.Cms.Manager
{
	/// <summary>
	/// FilterConfig
	/// </summary>
	public class FilterConfig
	{
		/// <summary>
		/// グローバルフィルタ登録
		/// </summary>
		/// <param name="filters">グローバルフィルタ</param>
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
#if DEBUG
			filters.Add(new HandleErrorAttribute());
#else
			filters.Add(new CmsHandleErrorAttribute());
#endif
		}
	}
}