/*
=========================================================================================================
  Module      : コンテンツマネージャコントローラ(ContentsManagerController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Web.Mvc;
using w2.App.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// コンテンツマネージャコントローラ
	/// </summary>
	public class ContentsManagerController : BaseController
	{
		/// <summary>
		/// インデックス
		/// </summary>
		/// <returns>結果</returns>
		public ActionResult ContentsManager()
		{
			this.Service.SetContentsManagerSetting(this.SessionWrapper.Session.SessionID);

			var vm = this.Service.CreateContentsViewModel();
			return View(vm);
		}

		/// <summary>
		/// TreeViewクリック
		/// </summary>
		/// <param name="clickPath">クリックしたパス</param>
		/// <param name="clickDir">ディレクトリ展開か</param>
		/// <param name="comeFromShortCut">ショートカットから来たか</param>
		/// <param name="openDirPathList">展開中のディレクトリパスリスト</param>
		/// <returns>結果</returns>
		[HttpPost]
		public ActionResult Click(string clickPath, bool clickDir, bool comeFromShortCut, string openDirPathList)
		{
			var openDirPath = openDirPathList.Split('：');
			var model = this.Service.Click(clickPath, clickDir, comeFromShortCut, openDirPath);

			return PartialView("TreeView", model);
		}

		/// <summary>
		/// ダウンロード
		/// </summary>
		/// <returns>zipまたはファイル</returns>
		public ActionResult Download()
		{
			this.Service.Download(this.Response);

			return null;
		}

		/// <summary>
		/// フォルダ作成
		/// </summary>
		/// <returns>エラーページか作成後ディレクトリのパス</returns>
		[HttpPost]
		public ActionResult MakeDirectory()
		{
			var objList = new List<object>
			{
				this.Service.MakeDirectory(),
				this.SessionWrapper.ContentsMnagerClickCurrent
			};

			return Json(new { data = objList }, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// ファイルアップロード
		/// </summary>
		/// <param name="input">入力</param>
		/// <returns>エラーメッセージかメッセージ</returns>
		[HttpPost]
		public ActionResult Upload(ContentsManagerInput input)
		{
			var errorMessage = input.Validate(this.SessionWrapper.ContentsMnagerClickCurrent);
			if (string.IsNullOrEmpty(errorMessage))
			{
				errorMessage = this.Service.UploadFile(input);
			}

			return Json(errorMessage, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>エラーページか親ディレクトリのパス</returns>
		[HttpPost]
		public ActionResult Delete()
		{
			var objList = new List<object>
			{
				this.Service.Delete(this.SessionWrapper.ContentsMnagerClickCurrent),
				this.SessionWrapper.ContentsMnagerClickCurrent
			};

			return Json(new { data = objList }, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// リネーム
		/// </summary>
		/// <param name="rename">変更ファイル名</param>
		/// <returns>結果</returns>
		[HttpPost]
		public ActionResult Rename(string rename)
		{
			var message = this.Service.Rename(rename);

			return Json(message, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// リサイズ
		/// </summary>
		/// <param name="imageType">イメージタイプ</param>
		/// <returns>リサイズフォーム</returns>
		[HttpPost]
		public ActionResult ReSize(string imageType)
		{
			var input = this.Service.ReSize(imageType);

			return PartialView(input);
		}

		/// <summary>
		/// 商品画像一括削除
		/// </summary>
		/// <param name="fileNameWithoutSize">削除ファイル名</param>
		/// <returns>結果</returns>
		[HttpPost]
		public ActionResult DeleteProductImages(string fileNameWithoutSize)
		{
			var objList = new List<object>
			{
				this.Service.DeleteProductImages(fileNameWithoutSize),
				this.SessionWrapper.ContentsMnagerClickCurrent
			};
			return Json(new { data = objList }, JsonRequestBehavior.AllowGet);
		}

		/// <summary>サービス</summary>
		private ContentsManagerWorkerService Service { get { return GetDefaultService<ContentsManagerWorkerService>(); } }
	}
}
