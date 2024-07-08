/*
=========================================================================================================
  Module      : シングルサインオンコントローラ(SingleSignOnController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.WorkerServices;
using w2.Domain.MenuAuthority.Helper;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// シングルサインオンコントローラ
	/// </summary>
	public class SingleSignOnController : BaseController
	{
		/// <summary>
		/// インデックス
		/// </summary>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <param name="nurl">遷移先URL</param>
		/// <returns>アクション結果</returns>
		public ActionResult Index(MenuAuthorityHelper.ManagerSiteType managerSiteType, string nurl)
		{
			// シングルサインオンチェック
			var vm = this.Service.CreateSingleSignOnInfo(managerSiteType, nurl);
			if (string.IsNullOrEmpty(vm.ErrorMessage) == false)
			{
				return CreateErrorPageAction(vm.ErrorMessage, errorPageType: Constants.REQUEST_KBN_ERRORPAGE_TYPE_DISP_GOTOLOGIN);
			}

			return View(vm);
		}

		/// <summary>サービス</summary>
		private SingleSignOnWorkerService Service { get { return GetDefaultService<SingleSignOnWorkerService>(); } }
	}
}
