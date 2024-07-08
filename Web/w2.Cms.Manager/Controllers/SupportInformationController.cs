/*
=========================================================================================================
  Module      : サポートサイト情報コントローラ(SupportInformationController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;
using w2.App.Common.SupportSite;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// サポートサイト情報コントローラ
	/// </summary>
	public class SupportInformationController : BaseController
	{
		/// <summary>
		/// インデックス
		/// </summary>
		/// <returns>結果</returns>
		public ActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// コンテンツ取得
		/// </summary>
		/// <returns>結果</returns>
		public ActionResult GetContents()
		{
			var data = new SupportSiteContentsGetter().Get(this.Request["j"]);
			return Content(data, "application/json");
		}

		/// <summary>サービス</summary>
		private SupportInformationWorkerService Service { get { return GetDefaultService<SupportInformationWorkerService>(); } }
	}
}
