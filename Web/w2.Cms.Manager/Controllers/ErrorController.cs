/*
=========================================================================================================
  Module      : エラーコントローラー(ErrorController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.ViewModels.Error;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// エラーコントローラー
	/// </summary>
	public class ErrorController : BaseController
	{
		/// <summary>
		/// インデックス
		/// </summary>
		/// <returns></returns>
		public ActionResult Index(IndexViewModel vm)
		{
			// ブラウザーキャッシュ削除
			this.Service.ClearBrowserCache(this.Response);

			// エラーメッセージセット
			vm.ErrorMessagesHtmlEncoded = this.Service.GetErrorMessage(vm.ErrorKbn, this.TempData.ErrorMessage);

			return View(vm);
		}

		/// <summary>
		/// ログイン画面へボタンクリック
		/// </summary>
		/// <param name="value1"></param>
		/// <returns></returns>
		[HttpPost]
		public ActionResult Index(string value1)
		{
			return Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_LOGIN);
		}

		/// <summary>サービス</summary>
		private ErrorWorkerService Service { get { return GetDefaultService<ErrorWorkerService>(); } }
	}
}
