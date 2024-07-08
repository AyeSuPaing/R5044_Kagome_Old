/*
=========================================================================================================
  Module      : CMSエラーハンドル(CmsHandleErrorAttribute.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ViewModels.Error;
using w2.Common.Logger;

namespace w2.Cms.Manager.Codes
{
	/// <summary>
	/// CMSエラーハンドル
	/// </summary>
	public class CmsHandleErrorAttribute : HandleErrorAttribute
	{
		/// <summary>
		/// エラー発生処理
		/// </summary>
		/// <param name="filterContext">エラーコンテキスト</param>
		public override void OnException(ExceptionContext filterContext)
		{
			if (filterContext == null)
			{
				throw new ArgumentException("filterContext");
			}

			// Log出力
			FileLogger.WriteError(filterContext.Exception);

			if (filterContext.HttpContext.Request.IsAjaxRequest())
			{
				// Application_Errorは呼ばれない
				HandleAjaxRequestException(filterContext);
			}
			else
			{
				var vm = new IndexViewModel
				{
					ErrorMessagesHtmlEncoded = WebMessages.Get("ERRMSG_SYSTEM_ERROR"),
				};
				filterContext.Result = new ViewResult
				{
					ViewName = @"~/Views/Error/Index.cshtml",
					ViewData = new ViewDataDictionary(vm),

				};
				filterContext.ExceptionHandled = true;
				base.OnException(filterContext);
			}
		}

		/// <summary>
		/// AJAXエラー
		/// </summary>
		/// <param name="filterContext">エラーコンテキスト</param>
		private void HandleAjaxRequestException(ExceptionContext filterContext)
		{
			if (filterContext.ExceptionHandled)
			{
				return;
			}

			filterContext.Result = new JsonResult
			{
				Data = new
				{
					Message = filterContext.Exception.ToString(),
				},
				JsonRequestBehavior = JsonRequestBehavior.AllowGet,
			};
			filterContext.ExceptionHandled = true;
			filterContext.HttpContext.Response.Clear();
			filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
		}
	}
}