/*
=========================================================================================================
  Module      : SEO設定コントローラ(SeoMetadatasController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// SEO設定コントローラ
	/// </summary>
	public class SeoMetadatasController : BaseController
	{
		/// <summary>
		/// 編集アクション
		/// </summary>
		/// <returns>アクションリザルト</returns>
		public ActionResult Modify(string pageLayout = Constants.LAYOUT_PATH_DEFAULT)
		{
			var vm = this.Service.CreateViewModelForModify(pageLayout);
			return View(vm);
		}

		/// <summary>
		/// Popup modify
		/// </summary>
		/// <returns>アクションリザルト</returns>
		public ActionResult PopupModify()
		{
			return RedirectToAction("Modify", new { pageLayout = Constants.POPUP_LAYOUT_PATH_DEFAULT });
		}

		/// <summary>
		/// 更新アクション
		/// </summary>
		/// <param name="inputForDefault">全体設定インプット</param>
		/// <param name="inputForProductList">商品一覧設定インプット</param>
		/// <param name="inputForProductDetail">商品詳細設定インプット</param>
		/// <param name="inputForCoordinateTop">コーディネートトップインプット</param>
		/// <param name="inputForCoordinateList">コーディネート一覧インプット</param>
		/// <param name="inputForCoordinateDetail">コーディネート詳細インプット</param>
		/// <returns>アクションリザルト</returns>
		[HttpPost]
		public ActionResult Update(
			SeoMetadatasInput inputForDefault,
			SeoMetadatasInput inputForProductList,
			SeoMetadatasInput inputForProductDetail,
			SeoMetadatasInput inputForCoordinateTop,
			SeoMetadatasInput inputForCoordinateList,
			SeoMetadatasInput inputForCoordinateDetail)
		{
			var errorMessage = this.Service.Update(
				inputForDefault,
				inputForProductList,
				inputForProductDetail,
				inputForCoordinateTop,
				inputForCoordinateList,
				inputForCoordinateDetail);

			return Json(errorMessage);
		}

		/// <summary>サービス</summary>
		private SeoMetadatasWorkerService Service { get { return GetDefaultService<SeoMetadatasWorkerService>(); } }
	}
}