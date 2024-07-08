/*
=========================================================================================================
  Module      : コンテンツマネージャコントローラ(JavaScriptDesignController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Web.Mvc;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// コンテンツマネージャコントローラ
	/// </summary>
	public class JavaScriptDesignController : BaseController
	{
		/// <summary>
		/// インデックス
		/// </summary>
		/// <returns>結果</returns>
		public ActionResult JavaScriptDesign()
		{
			this.Service.SetDesignSetting();
			var vm = this.Service.CreateContentsViewModel();
			return View(vm);
		}

		/// <summary>
		/// TreeViewクリック
		/// </summary>
		/// <param name="clickPath">クリックしたパス</param>
		/// <param name="clickDir">ディレクトリ展開か</param>
		/// <param name="openDirPathList">展開中のディレクトリパスリスト</param>
		/// <returns>結果</returns>
		[HttpPost]
		public ActionResult Click(string clickPath, bool clickDir, string openDirPathList)
		{
			var openDirPath = openDirPathList.Split('：');
			var model = this.Service.Click(openDirPath, clickPath, clickDir);
			return PartialView("_DesignSettingTreeView", model);
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
				this.SessionWrapper.JavascriptDesingMnagerClickCurrent
			};
			return Json(new { data = objList }, JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// JavaScriptファイル作成
		/// </summary>
		/// <param name="isNew">isNew=Trueの場合は、ファイルを作成</param>
		/// <returns>エラーページか作成後ディレクトリのパス</returns>
		[HttpPost]
		public ActionResult CreateJavaScriptFile(bool isNew)
		{
			var objList = new List<object>
			{
				this.Service.CreateFile(isNew),
				this.SessionWrapper.JavascriptDesingMnagerClickCurrent
			};
			return Json(new { data = objList }, JsonRequestBehavior.AllowGet);
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
				this.Service.Delete(),
				this.SessionWrapper.JavascriptDesingMnagerClickCurrent
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
			return Json(this.Service.Rename(rename), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 対象パス更新
		/// </summary>
		/// <returns>表示パス</returns>
		[HttpPost]
		public ActionResult UpdateTargetPath()
		{
			return Json(this.Service.UpdateTargetPath(), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// ダウンロード
		/// </summary>
		/// <returns>zipまたはファイル</returns>
		[HttpPost]
		public ActionResult GetJavaScriptText()
		{
			return Json(this.Service.GetFileText(), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// JavaScriptファイル更新
		/// </summary>
		/// <param name="text">テキスト</param>
		/// <returns>エラーテキスト</returns>
		[HttpPost]
		public ActionResult UpdateJavaScript(string text)
		{
			return Json(this.Service.UpdateFile(text), JsonRequestBehavior.AllowGet);
		}

		/// <summary>
		/// 編集サイトラジオボタン変更処理
		/// </summary>
		/// <param name="path">変更パス</param>
		/// <returns>空文字</returns>
		[HttpPost]
		public ActionResult ChangeDirectory(string path)
		{
			return Json(this.Service.ChangeDirectory(path), JsonRequestBehavior.AllowGet);
		}

		/// <summary>サービス</summary>
		private JavaScriptDesignWorkerService Service { get { return GetDefaultService<JavaScriptDesignWorkerService>(); } }
	}
}
