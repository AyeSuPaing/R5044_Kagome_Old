/*
=========================================================================================================
  Module      : WYSIWYG HTML編集ウィンドウコントローラ(WysiwygEditorController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Web.Mvc;
using w2.Cms.Manager.ViewModels;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// WYSIWYG HTML編集ウィンドウコントローラ
	/// </summary>
	public class _WysiwygEditorController : BaseController
	{
		/// <summary>
		/// 編集
		/// </summary>
		/// <returns>アクション結果</returns>
		public ActionResult Edit()
		{
			return View(new ViewModelBase());
		}
	}
}