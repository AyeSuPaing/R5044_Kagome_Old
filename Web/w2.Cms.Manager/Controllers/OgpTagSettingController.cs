/*
=========================================================================================================
 Module      : OGP設定コントローラ(OgpSettingController.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
 Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web;
using System.Web.Mvc;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// OGP設定コントローラ
	/// </summary>
	public class OgpTagSettingController : BaseController
	{
		/// <summary>
		/// 編集アクション
		/// </summary>
		/// <returns>アクションリザルト</returns>
		public ActionResult Modify()
		{
			var vm = this.Service.CreateViewModelForModify();
			return View(vm);
		}

		/// <summary>
		/// 更新アクション
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public JsonResult Update(OgpTagSettingInput inputForDefaultSetting, string imageUrl)
		{
			var message = this.Service.Update(inputForDefaultSetting, imageUrl);
			return Json(message);
		}

		/// <summary>
		/// ファイルアップロードアクション
		/// </summary>
		/// <param name="file">ファイル</param>
		/// <returns>アップロードしたファイルのパス</returns>
		public ActionResult Upload(HttpPostedFileBase file)
		{
			string path;
			var success = this.Service.Upload(file, out path);

			return success
				? Json(path)
				: CreateErrorPageAction(WebMessages.ContentsManagerFileOperationError);
		}

		/// <summary>サービス</summary>
		private OgpTagSettingWorkerService Service { get { return GetDefaultService<OgpTagSettingWorkerService>(); } }
	}
}