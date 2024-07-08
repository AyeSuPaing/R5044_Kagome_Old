/*
=========================================================================================================
  Module      : 機能一覧コントローラー(PageIndexListController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ParamModels.PageIndexList;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// 機能一覧コントローラー
	/// </summary>
	public class PageIndexListController :  BaseController
	{
		/// <summary>
		/// インデックス
		/// </summary>
		/// <returns>アクションリザルト</returns>
		public ActionResult Index(PageIndexParamModel param)
		{
			if (string.IsNullOrEmpty(param.Key))
			{
				return CreateErrorPageAction(WebMessages.MasterExportSettingIrregularParameterError);
			}
			var pageIndexList = this.Service.GetPageIndexList(param.Key, this.SessionWrapper.LoginOperatorMenus);
			return View(pageIndexList);
		}

		/// <summary>サービス</summary>
		private PageIndexListWorkerService Service { get { return GetDefaultService<PageIndexListWorkerService>(); } }
	}
}
